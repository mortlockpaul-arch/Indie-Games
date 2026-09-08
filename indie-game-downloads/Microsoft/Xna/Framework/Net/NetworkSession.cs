using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

public sealed class NetworkSession : IDisposable
{
	internal enum NetworkEventType
	{
		PacketSend,
		GamerJoin,
		GamerLeave,
		HostChange,
		StateChange
	}

	internal struct NetworkEvent
	{
		public NetworkEventType Type;

		public NetworkGamer Gamer;

		public byte[] Packet;

		public SendDataOptions Reliable;

		public NetworkSessionState State;

		public NetworkSessionEndReason Reason;
	}

	internal class NetworkSessionAction : IAsyncResult
	{
		public readonly AsyncCallback Callback;

		public readonly int MaxLocalGamers;

		public readonly IEnumerable<SignedInGamer> LocalGamers;

		public readonly int MaxPrivateSlots;

		public readonly NetworkSessionProperties SessionProperties;

		public readonly NetworkSessionType SessionType;

		public object AsyncState { get; private set; }

		public bool CompletedSynchronously => false;

		public bool IsCompleted { get; internal set; }

		public WaitHandle AsyncWaitHandle { get; private set; }

		public NetworkSessionAction(object state, AsyncCallback callback, int maxLocal, IEnumerable<SignedInGamer> localGamers, int maxPrivateSlots, NetworkSessionProperties properties, NetworkSessionType type)
		{
			AsyncState = state;
			Callback = callback;
			IsCompleted = false;
			AsyncWaitHandle = new ManualResetEvent(initialState: true);
			MaxLocalGamers = maxLocal;
			LocalGamers = localGamers;
			MaxPrivateSlots = maxPrivateSlots;
			SessionProperties = properties;
			SessionType = type;
		}
	}

	public const int MaxSupportedGamers = 31;

	public const int MaxPreviousGamers = 100;

	private int maxLocalGamers;

	private Queue<NetworkEvent> networkEvents;

	private static NetworkSessionAction activeAction;

	private static NetworkSession activeSession;

	public bool IsDisposed { get; private set; }

	public GamerCollection<NetworkGamer> AllGamers { get; private set; }

	public GamerCollection<LocalNetworkGamer> LocalGamers { get; private set; }

	public GamerCollection<NetworkGamer> RemoteGamers { get; private set; }

	public GamerCollection<NetworkGamer> PreviousGamers { get; private set; }

	public bool AllowHostMigration { get; set; }

	public bool AllowJoinInProgress { get; set; }

	public int BytesPerSecondReceived { get; private set; }

	public int BytesPerSecondSent { get; private set; }

	public NetworkGamer Host { get; private set; }

	public bool IsEveryoneReady
	{
		get
		{
			foreach (LocalNetworkGamer localGamer in LocalGamers)
			{
				if (!localGamer.IsReady)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool IsHost
	{
		get
		{
			foreach (LocalNetworkGamer localGamer in LocalGamers)
			{
				if (localGamer.IsHost)
				{
					return true;
				}
			}
			return false;
		}
	}

	public int MaxGamers { get; set; }

	public int PrivateGamerSlots { get; set; }

	public NetworkSessionProperties SessionProperties { get; private set; }

	public NetworkSessionState SessionState { get; private set; }

	public NetworkSessionType SessionType { get; private set; }

	public TimeSpan SimulatedLatency { get; set; }

	public float SimulatedPacketLoss { get; set; }

	public event EventHandler<GameStartedEventArgs> GameStarted;

	public event EventHandler<GameEndedEventArgs> GameEnded;

	public event EventHandler<GamerJoinedEventArgs> GamerJoined;

	public event EventHandler<GamerLeftEventArgs> GamerLeft;

	public event EventHandler<HostChangedEventArgs> HostChanged;

	public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

	public event EventHandler<WriteLeaderboardsEventArgs> WriteArbitratedLeaderboard;

	public event EventHandler<WriteLeaderboardsEventArgs> WriteUnarbitratedLeaderboard;

	public event EventHandler<WriteLeaderboardsEventArgs> WriteTrueSkill;

	public static event EventHandler<InviteAcceptedEventArgs> InviteAccepted;

	internal NetworkSession(NetworkSessionProperties properties, NetworkSessionType type, int maxGamers, int privateGamerSlots, int maxLocal, IEnumerable<SignedInGamer> localGamers)
	{
		SessionProperties = properties;
		SessionType = type;
		MaxGamers = maxGamers;
		PrivateGamerSlots = privateGamerSlots;
		List<LocalNetworkGamer> list = new List<LocalNetworkGamer>();
		if (localGamers == null)
		{
			maxLocalGamers = maxLocal;
			for (int i = 0; i < Gamer.SignedInGamers.Count && i < maxLocalGamers; i++)
			{
				if (!Gamer.SignedInGamers[i].IsGuest)
				{
					list.Add(new LocalNetworkGamer(Gamer.SignedInGamers[i], this));
				}
			}
		}
		else
		{
			maxLocalGamers = 0;
			foreach (SignedInGamer localGamer in localGamers)
			{
				list.Add(new LocalNetworkGamer(localGamer, this));
				maxLocalGamers++;
			}
		}
		LocalGamers = new GamerCollection<LocalNetworkGamer>(list);
		List<NetworkGamer> collection = new List<NetworkGamer>();
		RemoteGamers = new GamerCollection<NetworkGamer>(collection);
		List<NetworkGamer> list2 = new List<NetworkGamer>();
		list2.AddRange(collection);
		list2.AddRange(list);
		AllGamers = new GamerCollection<NetworkGamer>(list2);
		PreviousGamers = new GamerCollection<NetworkGamer>(new List<NetworkGamer>());
		Host = LocalGamers[0];
		if (IsHost)
		{
			AllowHostMigration = false;
			AllowJoinInProgress = false;
			SessionState = NetworkSessionState.Lobby;
		}
		networkEvents = new Queue<NetworkEvent>();
		foreach (NetworkGamer allGamer in AllGamers)
		{
			NetworkEvent evt = new NetworkEvent
			{
				Type = NetworkEventType.GamerJoin,
				Gamer = allGamer
			};
			SendNetworkEvent(evt);
		}
		SimulatedLatency = TimeSpan.Zero;
		SimulatedPacketLoss = 0f;
		IsDisposed = false;
		BytesPerSecondReceived = 0;
		BytesPerSecondSent = 0;
	}

	public void Dispose()
	{
		foreach (LocalNetworkGamer localGamer in LocalGamers)
		{
			localGamer.packetQueue.Clear();
		}
		activeSession = null;
		IsDisposed = true;
	}

	public void Update()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("this");
		}
		while (networkEvents.Count > 0)
		{
			NetworkEvent networkEvent = networkEvents.Dequeue();
			if (networkEvent.Type == NetworkEventType.PacketSend)
			{
				continue;
			}
			if (networkEvent.Type == NetworkEventType.GamerJoin)
			{
				if (GamerJoined != null)
				{
					GamerJoined(this, new GamerJoinedEventArgs(networkEvent.Gamer));
				}
			}
			else if (networkEvent.Type == NetworkEventType.GamerLeave)
			{
				if (GamerLeft != null)
				{
					GamerLeft(this, new GamerLeftEventArgs(networkEvent.Gamer));
				}
			}
			else if (networkEvent.Type == NetworkEventType.HostChange)
			{
				if (HostChanged != null)
				{
					HostChanged(this, new HostChangedEventArgs(Host, networkEvent.Gamer));
				}
				Host = networkEvent.Gamer;
			}
			else
			{
				if (networkEvent.Type != NetworkEventType.StateChange)
				{
					continue;
				}
				if (networkEvent.State == NetworkSessionState.Playing)
				{
					if (GameStarted != null)
					{
						GameStarted(this, new GameStartedEventArgs());
					}
				}
				else if (networkEvent.State == NetworkSessionState.Lobby)
				{
					if (GameEnded != null)
					{
						GameEnded(this, new GameEndedEventArgs());
					}
				}
				else
				{
					SessionEnded(this, new NetworkSessionEndedEventArgs(networkEvent.Reason));
				}
				SessionState = networkEvent.State;
			}
		}
	}

	public void AddLocalGamer(SignedInGamer gamer)
	{
		if (LocalGamers.Count == maxLocalGamers)
		{
			throw new InvalidOperationException("LocalGamer max limit!");
		}
		LocalNetworkGamer item = new LocalNetworkGamer(gamer, this);
		LocalGamers.collection.Add(item);
		AllGamers.collection.Add(item);
	}

	public NetworkGamer FindGamerById(byte gameId)
	{
		foreach (NetworkGamer allGamer in AllGamers)
		{
			if (allGamer.Id == gameId)
			{
				return allGamer;
			}
		}
		return null;
	}

	public void ResetReady()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("this");
		}
		if (!IsHost)
		{
			throw new InvalidOperationException("This NetworkSession is not the host");
		}
		foreach (NetworkGamer allGamer in AllGamers)
		{
			allGamer.IsReady = false;
		}
	}

	public void StartGame()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("this");
		}
		if (!IsHost)
		{
			throw new InvalidOperationException("This NetworkSession is not the host");
		}
		if (SessionState != NetworkSessionState.Lobby)
		{
			throw new InvalidOperationException("NetworkSession is not Lobby");
		}
		NetworkEvent evt = new NetworkEvent
		{
			Type = NetworkEventType.StateChange,
			State = NetworkSessionState.Playing
		};
		SendNetworkEvent(evt);
	}

	public void EndGame()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("this");
		}
		if (!IsHost)
		{
			throw new InvalidOperationException("This NetworkSession is not the host");
		}
		if (SessionState != NetworkSessionState.Playing)
		{
			throw new InvalidOperationException("NetworkSession is not Playing");
		}
		NetworkEvent evt = new NetworkEvent
		{
			Type = NetworkEventType.StateChange,
			State = NetworkSessionState.Lobby
		};
		SendNetworkEvent(evt);
	}

	internal void SendNetworkEvent(NetworkEvent evt)
	{
		networkEvents.Enqueue(evt);
	}

	public static NetworkSession Create(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers)
	{
		IAsyncResult asyncResult = BeginCreate(sessionType, maxLocalGamers, maxGamers, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndCreate(asyncResult);
	}

	public static NetworkSession Create(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties)
	{
		IAsyncResult asyncResult = BeginCreate(sessionType, maxLocalGamers, maxGamers, privateGamerSlots, sessionProperties, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndCreate(asyncResult);
	}

	public static NetworkSession Create(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties)
	{
		IAsyncResult asyncResult = BeginCreate(sessionType, localGamers, maxGamers, privateGamerSlots, sessionProperties, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndCreate(asyncResult);
	}

	public static IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, AsyncCallback callback, object asyncState)
	{
		if (maxLocalGamers < 1 || maxLocalGamers > 4)
		{
			throw new ArgumentOutOfRangeException("maxLocalGamers");
		}
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		activeAction = new NetworkSessionAction(asyncState, callback, maxLocalGamers, null, 0, new NetworkSessionProperties(), sessionType);
		return activeAction;
	}

	public static IAsyncResult BeginCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, AsyncCallback callback, object asyncState)
	{
		if (maxLocalGamers < 1 || maxLocalGamers > 4)
		{
			throw new ArgumentOutOfRangeException("maxLocalGamers");
		}
		if (privateGamerSlots < 0 || privateGamerSlots > maxGamers)
		{
			throw new ArgumentOutOfRangeException("privateGamerSlots");
		}
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		activeAction = new NetworkSessionAction(asyncState, callback, maxLocalGamers, null, privateGamerSlots, sessionProperties, sessionType);
		return activeAction;
	}

	public static IAsyncResult BeginCreate(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, AsyncCallback callback, object asyncState)
	{
		if (privateGamerSlots < 0 || privateGamerSlots > maxGamers)
		{
			throw new ArgumentOutOfRangeException("privateGamerSlots");
		}
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		activeAction = new NetworkSessionAction(asyncState, callback, 0, localGamers, privateGamerSlots, sessionProperties, sessionType);
		return activeAction;
	}

	public static NetworkSession EndCreate(IAsyncResult result)
	{
		if (result != activeAction)
		{
			throw new ArgumentException("result");
		}
		activeSession = new NetworkSession(activeAction.SessionProperties, activeAction.SessionType, 69, activeAction.MaxPrivateSlots, activeAction.MaxLocalGamers, activeAction.LocalGamers);
		activeAction = null;
		return activeSession;
	}

	public static AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties)
	{
		IAsyncResult asyncResult = BeginFind(sessionType, maxLocalGamers, searchProperties, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndFind(asyncResult);
	}

	public static AvailableNetworkSessionCollection Find(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, NetworkSessionProperties searchProperties)
	{
		IAsyncResult asyncResult = BeginFind(sessionType, localGamers, searchProperties, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndFind(asyncResult);
	}

	public static IAsyncResult BeginFind(NetworkSessionType sessionType, int maxLocalGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
	{
		if (sessionType == NetworkSessionType.Local)
		{
			throw new ArgumentException("sessionType");
		}
		if (maxLocalGamers < 1 || maxLocalGamers > 4)
		{
			throw new ArgumentOutOfRangeException("maxLocalGamers");
		}
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		activeAction = new NetworkSessionAction(asyncState, callback, maxLocalGamers, null, 0, searchProperties, sessionType);
		return activeAction;
	}

	public static IAsyncResult BeginFind(NetworkSessionType sessionType, IEnumerable<SignedInGamer> localGamers, NetworkSessionProperties searchProperties, AsyncCallback callback, object asyncState)
	{
		if (sessionType == NetworkSessionType.Local)
		{
			throw new ArgumentException("sessionType");
		}
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		int num = 0;
		foreach (SignedInGamer localGamer in localGamers)
		{
			num++;
		}
		activeAction = new NetworkSessionAction(asyncState, callback, num, localGamers, 0, searchProperties, sessionType);
		return activeAction;
	}

	public static AvailableNetworkSessionCollection EndFind(IAsyncResult result)
	{
		if (result != activeAction)
		{
			throw new ArgumentException("result");
		}
		List<AvailableNetworkSession> collection = new List<AvailableNetworkSession>();
		activeAction = null;
		return new AvailableNetworkSessionCollection(collection);
	}

	public static NetworkSession Join(AvailableNetworkSession availableSession)
	{
		IAsyncResult asyncResult = BeginJoin(availableSession, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndJoin(asyncResult);
	}

	public static IAsyncResult BeginJoin(AvailableNetworkSession availableSession, AsyncCallback callback, object asyncState)
	{
		if (availableSession == null)
		{
			throw new ArgumentNullException("availableSession");
		}
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		activeAction = new NetworkSessionAction(asyncState, callback, 4, null, 0, null, NetworkSessionType.PlayerMatch);
		return activeAction;
	}

	public static NetworkSession EndJoin(IAsyncResult result)
	{
		if (result != activeAction)
		{
			throw new ArgumentException("result");
		}
		int maxLocal = activeAction.MaxLocalGamers;
		IEnumerable<SignedInGamer> localGamers = activeAction.LocalGamers;
		activeAction = null;
		activeSession = new NetworkSession(null, NetworkSessionType.PlayerMatch, 31, 4, maxLocal, localGamers);
		return activeSession;
	}

	public static NetworkSession JoinInvited(int maxLocalGamers)
	{
		IAsyncResult asyncResult = BeginJoinInvited(maxLocalGamers, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndJoinInvited(asyncResult);
	}

	public static NetworkSession JoinInvited(IEnumerable<SignedInGamer> localGamers)
	{
		IAsyncResult asyncResult = BeginJoinInvited(localGamers, null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				activeAction.IsCompleted = true;
			}
		}
		return EndJoinInvited(asyncResult);
	}

	public static IAsyncResult BeginJoinInvited(int maxLocalGamers, AsyncCallback callback, object asyncState)
	{
		if (maxLocalGamers < 1 || maxLocalGamers > 4)
		{
			throw new ArgumentOutOfRangeException("maxLocalGamers");
		}
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		activeAction = new NetworkSessionAction(asyncState, callback, maxLocalGamers, null, 0, null, NetworkSessionType.PlayerMatch);
		return activeAction;
	}

	public static IAsyncResult BeginJoinInvited(IEnumerable<SignedInGamer> localGamers, AsyncCallback callback, object asyncState)
	{
		if (activeAction != null || activeSession != null)
		{
			throw new InvalidOperationException();
		}
		activeAction = new NetworkSessionAction(asyncState, callback, 0, localGamers, 0, null, NetworkSessionType.PlayerMatch);
		return activeAction;
	}

	public static NetworkSession EndJoinInvited(IAsyncResult result)
	{
		if (result != activeAction)
		{
			throw new ArgumentException("result");
		}
		int maxLocal = activeAction.MaxLocalGamers;
		IEnumerable<SignedInGamer> localGamers = activeAction.LocalGamers;
		activeAction = null;
		activeSession = new NetworkSession(null, NetworkSessionType.PlayerMatch, 31, 4, maxLocal, localGamers);
		return activeSession;
	}
}
