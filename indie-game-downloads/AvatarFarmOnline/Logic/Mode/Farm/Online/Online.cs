using System;
using System.Collections.Generic;
using System.Linq;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class Online : IDisposable
{
	public enum SessionProperty
	{
		SessionType,
		FriendPermissions,
		PublicPermissions,
		Level,
		Coins,
		Cash
	}

	public enum SessionTypes
	{
		P2PShare = 1,
		MultiplayerGameV1,
		MultiplayerGameVLatest
	}

	public enum HostMode
	{
		Public,
		Private
	}

	public enum MultiplayerStates
	{
		Inactive,
		ReceivingData,
		Loading,
		Playing
	}

	public class NetworkGamerData
	{
		private AvatarFarmOnline.Logic.Stage.Player player;

		private AvatarFarmOnline.Logic.Stage.PlayerSelection selection;

		public AvatarFarmOnline.Logic.Stage.Player Player
		{
			get
			{
				return player;
			}
			set
			{
				player = value;
			}
		}

		public AvatarFarmOnline.Logic.Stage.PlayerSelection Selection
		{
			get
			{
				return selection;
			}
			set
			{
				selection = value;
			}
		}
	}

	public enum QuickJoinStates
	{
		SearchingGames,
		TryingToJoin,
		NotFound
	}

	public const int MAX_ONLINE_PLAYERS = 16;

	private MultiplayerStates multiplayerState;

	private Session session;

	private SessionType sessionType;

	private Queue<AvatarFarmOnline.Logic.Mode.Farm.Online.IOnlineTask> postLoadTasks = new Queue<AvatarFarmOnline.Logic.Mode.Farm.Online.IOnlineTask>();

	private ILocalNetworkGamer host;

	protected static IPacketWriter packetWriter;

	protected static IPacketReader packetReader;

	private bool enableUpdates;

	private AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData persistentData;

	private QuickJoinStates quickJoinState;

	private int quickJoinAttempts;

	private int quickJoinFoundGames;

	public MultiplayerStates MultiplayerState => multiplayerState;

	public bool Active => session != null;

	public SessionType SessionType => sessionType;

	public SessionState SessionState
	{
		get
		{
			if (Active)
			{
				return session.SessionState;
			}
			return SessionState.Ended;
		}
	}

	public int PlayerNumber
	{
		get
		{
			if (!Active)
			{
				return 0;
			}
			return session.AllGamersCount;
		}
	}

	public bool IsHost
	{
		get
		{
			if (session != null)
			{
				return session.IsHost;
			}
			return false;
		}
	}

	public ILocalNetworkGamer LocalGamer => session.GetLocalGamer(0);

	public ILocalNetworkGamer HostLocalGamer => host;

	public INetworkGamer RemoteHost => session.Host;

	public IPacketWriter PacketWriter => packetWriter;

	public IPacketReader PacketReader => packetReader;

	public AvatarFarmOnline.Logic.PlayerPermissions PublicPermissions
	{
		get
		{
			if (session != null)
			{
				return (AvatarFarmOnline.Logic.PlayerPermissions)session.SessionProperties[2].Value;
			}
			return AvatarFarmOnline.Logic.PlayerPermissions.None;
		}
	}

	public AvatarFarmOnline.Logic.PlayerPermissions FriendPermissions
	{
		get
		{
			if (session != null)
			{
				return (AvatarFarmOnline.Logic.PlayerPermissions)session.SessionProperties[1].Value;
			}
			return AvatarFarmOnline.Logic.PlayerPermissions.None;
		}
	}

	public AvatarFarmOnline.Logic.Stage.Stage Stage
	{
		get
		{
			if (persistentData.CurrentGame is AvatarFarmOnline.Logic.Mode.Farm.FarmGameData farmGameData)
			{
				return farmGameData.Stage;
			}
			return null;
		}
	}

	public AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData PersistentData => persistentData;

	public QuickJoinStates QuickJoinState => quickJoinState;

	public int QuickJoinAttempts => quickJoinAttempts;

	public int QuickJoinFoundGames => quickJoinFoundGames;

	public static event Action<SessionEndReason> OnSessionEnd;

	public event Action OnBeginLoad;

	public static event Action<ISignedInGamer> OnInviteAccepted;

	public INetworkGamer GetNetworkGamer(int index)
	{
		return session.GetGamer(index);
	}

	public Online(AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData pgd, SessionType sessionType)
	{
		persistentData = pgd;
		this.sessionType = sessionType;
	}

	public void StageLoaded()
	{
		if (session.IsHost)
		{
			multiplayerState = MultiplayerStates.Playing;
			session.StartGame();
			enableUpdates = true;
		}
		else
		{
			multiplayerState = MultiplayerStates.Playing;
			SendPlayerLoaded();
		}
		while (postLoadTasks.Count > 0)
		{
			postLoadTasks.Dequeue().Process(this);
		}
	}

	public static void SetupInvites()
	{
		NetworkInterface.Instance.RegisterInviteAccepted(InviteAccepted);
	}

	private static void InviteAccepted(object sender, InviteAcceptedArgs args)
	{
		if (OnInviteAccepted != null)
		{
			OnInviteAccepted(args.Gamer);
		}
	}

	public bool JoinInvited(out SessionJoinError error)
	{
		error = SessionJoinError.SessionNotJoinable;
		List<ISignedInGamer> list = new List<ISignedInGamer>();
		foreach (PlayerIndex playerIndex in persistentData.Setup.PlayerIndices)
		{
			list.Add(PlatformInterface.Instance.GetGamer(playerIndex));
		}
		try
		{
			try
			{
				session = NetworkInterface.Instance.JoinInvited(list);
			}
			catch (NetworkSessionJoinException ex)
			{
				error = ex.JoinError;
				return false;
			}
			catch (Exception)
			{
				error = SessionJoinError.SessionNotFound;
				return false;
			}
			if (session.SessionProperties[0].Value != 3)
			{
				session.Dispose();
				session = null;
				return false;
			}
			JoinedSession();
			return true;
		}
		catch (Exception)
		{
			if (session != null)
			{
				session.Dispose();
			}
			session = null;
			return false;
		}
	}

	public bool JoinAddress(string address, out SessionJoinError error)
	{
		error = SessionJoinError.SessionNotJoinable;
		List<ISignedInGamer> list = new List<ISignedInGamer>();
		foreach (PlayerIndex playerIndex in persistentData.Setup.PlayerIndices)
		{
			list.Add(PlatformInterface.Instance.GetGamer(playerIndex));
		}
		try
		{
			try
			{
				session = NetworkInterface.Instance.JoinByAddress(list, address);
			}
			catch (NetworkSessionJoinException ex)
			{
				error = ex.JoinError;
				return false;
			}
			catch (Exception)
			{
				error = SessionJoinError.SessionNotFound;
				return false;
			}
			if (session.SessionProperties[0].Value != 3)
			{
				session.Dispose();
				session = null;
				return false;
			}
			JoinedSession();
			return true;
		}
		catch (Exception)
		{
			if (session != null)
			{
				session.Dispose();
			}
			session = null;
			return false;
		}
	}

	public IAvailableSessionCollection FindGames()
	{
		List<ISignedInGamer> list = new List<ISignedInGamer>();
		foreach (PlayerIndex playerIndex in persistentData.Setup.PlayerIndices)
		{
			list.Add(PlatformInterface.Instance.GetGamer(playerIndex));
		}
		ISessionProperties sessionProperties = NetworkInterface.Instance.CreateSessionProperties();
		sessionProperties[0] = 3;
		try
		{
			return NetworkInterface.Instance.Find(sessionType, list, sessionProperties);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public bool QuickJoin()
	{
		if (session != null)
		{
			return false;
		}
		quickJoinState = QuickJoinStates.SearchingGames;
		quickJoinAttempts = 0;
		IAvailableSessionCollection availableSessionCollection = FindGames();
		if (availableSessionCollection != null)
		{
			List<IAvailableSession> list = new List<IAvailableSession>(availableSessionCollection.Sessions.Where((IAvailableSession ans) => ans.OpenPublicGamerSlots >= 1));
			list.Sort(delegate(IAvailableSession ans1, IAvailableSession ans2)
			{
				int num = 0;
				num = ans2.IsAverageRoundtripTimeAvailable.CompareTo(ans1.IsAverageRoundtripTimeAvailable);
				return (num != 0) ? num : ans1.AverageRoundtripTime.CompareTo(ans2.AverageRoundtripTime);
			});
			quickJoinFoundGames = list.Count;
			quickJoinState = QuickJoinStates.TryingToJoin;
			foreach (IAvailableSession item in list)
			{
				try
				{
					session = NetworkInterface.Instance.Join(item);
				}
				catch (Exception)
				{
					if (session != null)
					{
						session.Dispose();
					}
					session = null;
				}
				if (session == null)
				{
					quickJoinAttempts++;
					continue;
				}
				break;
			}
		}
		if (session != null)
		{
			JoinedSession();
			return true;
		}
		quickJoinState = QuickJoinStates.NotFound;
		return false;
	}

	public bool TryJoin(IAvailableSession ans, out SessionJoinError errorCause)
	{
		errorCause = SessionJoinError.SessionNotJoinable;
		if (session != null)
		{
			return false;
		}
		try
		{
			session = NetworkInterface.Instance.Join(ans);
		}
		catch (NetworkSessionJoinException ex)
		{
			errorCause = ex.JoinError;
			return false;
		}
		catch (Exception)
		{
			errorCause = SessionJoinError.SessionNotFound;
			return false;
		}
		if (session != null)
		{
			JoinedSession();
		}
		return session != null;
	}

	private void JoinedSession()
	{
		hookEvents();
		enableUpdates = true;
		multiplayerState = MultiplayerStates.ReceivingData;
	}

	public bool SetupMultiplayer(HostMode hostMode, AvatarFarmOnline.Logic.PlayerPermissions friendPermissions, AvatarFarmOnline.Logic.PlayerPermissions publicPermissions)
	{
		ISessionProperties sessionProperties = NetworkInterface.Instance.CreateSessionProperties();
		sessionProperties[0] = 3;
		sessionProperties[1] = (int)friendPermissions;
		sessionProperties[2] = (int)publicPermissions;
		ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(persistentData.Setup.PlayerIndex);
		List<ISignedInGamer> list = new List<ISignedInGamer>(1);
		list.Add(gamer);
		int num = 0;
		num = hostMode switch
		{
			HostMode.Private => 16 - list.Count, 
			_ => 0, 
		};
		try
		{
			session = NetworkInterface.Instance.Create(sessionType, list, 16, num, sessionProperties);
		}
		catch (Exception)
		{
			return false;
		}
		session.AllowHostMigration = false;
		session.AllowJoinInProgress = true;
		for (int i = 0; i < session.LocalGamersCount; i++)
		{
			ILocalNetworkGamer localGamer = session.GetLocalGamer(i);
			if (localGamer.IsHost)
			{
				host = localGamer;
				break;
			}
		}
		multiplayerState = MultiplayerStates.Loading;
		hookEvents();
		return true;
	}

	public void SetPermissions(AvatarFarmOnline.Logic.PlayerPermissions friendPermissions, AvatarFarmOnline.Logic.PlayerPermissions publicPermissions)
	{
		if (IsHost && session != null)
		{
			session.SessionProperties[1] = (int)friendPermissions;
			session.SessionProperties[2] = (int)publicPermissions;
			AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket packet = AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket.GetPacket(friendPermissions, publicPermissions);
			packet.Send(this, LocalGamer);
		}
	}

	public void UpdatedPermissions(AvatarFarmOnline.Logic.PlayerPermissions friendPermissions, AvatarFarmOnline.Logic.PlayerPermissions publicPermissions)
	{
		try
		{
			if (!IsHost)
			{
				AvatarFarmOnline.Logic.Stage.Player playerFor = GetPlayerFor(LocalGamer.Id);
				if (playerFor != null)
				{
					bool flag = LocalGamer.SignedInGamer.IsFriend(RemoteHost.Gamer.Gamertag);
					playerFor.SetPermissions(flag ? friendPermissions : publicPermissions);
				}
			}
		}
		catch
		{
		}
	}

	public void StartStage()
	{
	}

	public INetworkGamer GetGamer(short networkId)
	{
		return session.FindGamerById((byte)networkId);
	}

	public void Update()
	{
		if (!enableUpdates)
		{
			return;
		}
		try
		{
			session.Update();
		}
		catch (Exception)
		{
			try
			{
				session.Dispose();
				sessionEnded(session, new SessionEndedEventArgs(SessionEndReason.Disconnected));
			}
			finally
			{
				session = null;
			}
		}
		if (session == null)
		{
			return;
		}
		if (IsHost && Stage != null)
		{
			session.SessionProperties[3] = Stage.FarmData.PlayerData.Level;
			session.SessionProperties[4] = Stage.FarmData.PlayerData.Coins;
			session.SessionProperties[5] = Stage.FarmData.PlayerData.Cash;
		}
		for (int i = 0; i < session.LocalGamersCount; i++)
		{
			ILocalNetworkGamer localGamer = session.GetLocalGamer(i);
			while (localGamer.IsDataAvailable)
			{
				AvatarFarmOnline.Logic.Mode.Farm.Online.Packet.Receive(this, localGamer)?.Process(this);
			}
		}
	}

	private void hookEvents()
	{
		packetReader = session.CreatePacketReader();
		packetWriter = session.CreatePacketWriter();
		session.GamerJoined += gamerJoined;
		session.GameStarted += gameStarted;
		session.GameEnded += gameEnded;
		session.GamerLeft += gamerLeft;
		session.SessionEnded += sessionEnded;
	}

	private void sessionEnded(object sender, SessionEndedEventArgs args)
	{
		try
		{
			if (args.EndReason != SessionEndReason.ClientSignedOut && OnSessionEnd != null)
			{
				OnSessionEnd(args.EndReason);
			}
			Reset();
		}
		catch (Exception e)
		{
			BaseGame.Instance.NextGameSection = new ExceptionSection(e);
		}
	}

	private void gameEnded(object sender, GameEndedEventArgs e)
	{
	}

	private void gamerJoined(object sender, GamerJoinedEventArgs e)
	{
		try
		{
			if (IsHost)
			{
				SendFarmData(e.Gamer);
			}
		}
		catch (Exception e2)
		{
			BaseGame.Instance.NextGameSection = new ExceptionSection(e2);
		}
	}

	private void gameStarted(object sender, GameStartedEventArgs e)
	{
	}

	private void gamerLeft(object sender, GamerLeftEventArgs e)
	{
		try
		{
			if (multiplayerState != MultiplayerStates.Playing)
			{
				return;
			}
			if (Stage != null)
			{
				AvatarFarmOnline.Logic.Stage.Player player = GetDataFor(e.Gamer).Player;
				if (player != null)
				{
					Stage.RemovePlayer(player);
				}
			}
			e.Gamer.Tag = null;
		}
		catch (Exception e2)
		{
			BaseGame.Instance.NextGameSection = new ExceptionSection(e2);
		}
	}

	public void SendPartyInvites()
	{
		try
		{
			if (!PlatformInterface.Instance.IsGuideVisible)
			{
				LocalGamer.SendPartyInvites();
			}
		}
		catch (Exception)
		{
		}
	}

	public void FillTags(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		foreach (AvatarFarmOnline.Logic.Stage.Player player in stage.Players)
		{
			if (player.Selection is AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection)
			{
				AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection networkPlayerSelection = (AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection)player.Selection;
				GetDataFor(networkPlayerSelection.NetworkGamer).Player = player;
			}
			else if (player.Selection is AvatarFarmOnline.Logic.Stage.LocalPlayerSelection)
			{
				AvatarFarmOnline.Logic.Stage.LocalPlayerSelection cs = (AvatarFarmOnline.Logic.Stage.LocalPlayerSelection)player.Selection;
				GetDataFor(GetGamerFor(cs)).Player = player;
			}
		}
	}

	private void ReleaseTags()
	{
		int playerNumber = PlayerNumber;
		for (int i = 0; i < playerNumber; i++)
		{
			INetworkGamer networkGamer = GetNetworkGamer(i);
			GetDataFor(networkGamer).Player = null;
		}
	}

	public static NetworkGamerData GetDataFor(INetworkGamer gamer)
	{
		if (gamer.Tag == null)
		{
			gamer.Tag = new NetworkGamerData();
		}
		return (NetworkGamerData)gamer.Tag;
	}

	public INetworkGamer GetGamerFor(AvatarFarmOnline.Logic.Stage.PlayerSelection cs)
	{
		if (cs is AvatarFarmOnline.Logic.Stage.LocalPlayerSelection localPlayerSelection)
		{
			return GetLocalGamer(localPlayerSelection.PlayerIndex);
		}
		if (cs is AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection networkPlayerSelection)
		{
			return networkPlayerSelection.NetworkGamer;
		}
		return null;
	}

	public AvatarFarmOnline.Logic.Stage.Player GetPlayerFor(short id)
	{
		if (Stage == null)
		{
			return null;
		}
		foreach (AvatarFarmOnline.Logic.Stage.Player player in Stage.Players)
		{
			if (player.Selection.Id == id)
			{
				return player;
			}
		}
		return null;
	}

	public ILocalNetworkGamer GetLocalGamer(int index)
	{
		return session.GetLocalGamer(index);
	}

	public ILocalNetworkGamer GetLocalGamer(PlayerIndex playerIndex)
	{
		for (int i = 0; i < session.LocalGamersCount; i++)
		{
			ILocalNetworkGamer localGamer = session.GetLocalGamer(i);
			if (localGamer.SignedInGamer.PlayerIndex == playerIndex)
			{
				return localGamer;
			}
		}
		return null;
	}

	public void SendPlayerUpdate(AvatarFarmOnline.Logic.Stage.LocalPlayer player)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket packet = AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket.GetPacket(player);
		packet.Send(this, LocalGamer);
	}

	private void SendPlayerLoaded()
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket packet = AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket.GetPacket(Stage.LocalPlayer);
		packet.Send(this, LocalGamer);
	}

	public void PlayerLoadedReceived(INetworkGamer gamer, short id, Vector2 position, int level)
	{
		if (multiplayerState == MultiplayerStates.Playing)
		{
			NetworkGamerData dataFor = GetDataFor(gamer);
			if (dataFor.Player == null)
			{
				AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection networkPlayerSelection = (AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection)(dataFor.Selection = new AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection(gamer));
				AvatarFarmOnline.Logic.Stage.PlayerSpawnData psd = new AvatarFarmOnline.Logic.Stage.PlayerSpawnData(networkPlayerSelection, position, level);
				AvatarFarmOnline.Logic.Stage.NetworkPlayer player = Stage.AddSelection(psd) as AvatarFarmOnline.Logic.Stage.NetworkPlayer;
				GetDataFor(networkPlayerSelection.NetworkGamer).Player = player;
			}
		}
	}

	private void SendFarmData(INetworkGamer gamer)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.FarmDataPacket farmDataPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.FarmDataPacket(Stage);
		farmDataPacket.Send(this, LocalGamer, gamer);
	}

	public void SendPlayerLevelInfo(AvatarFarmOnline.Logic.Stage.LocalPlayer player)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket packet = AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket.GetPacket(player);
		packet.Send(this, LocalGamer);
	}

	public void FarmDataReceived(AvatarFarmOnline.Logic.Stage.FarmData farmData, List<AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData> remotePlayers)
	{
		if (!IsHost && multiplayerState == MultiplayerStates.ReceivingData)
		{
			persistentData.SetFarm(new AvatarFarmOnline.Logic.Mode.Farm.RemoteFarmHeader(farmData, remotePlayers));
			if (OnBeginLoad != null)
			{
				OnBeginLoad();
			}
			multiplayerState = MultiplayerStates.Loading;
		}
	}

	public void SendTick()
	{
		if (IsHost)
		{
			AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket packet = AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket.GetPacket(Stage.FarmData);
			packet.Send(this, LocalGamer);
		}
	}

	public void SendStartWork(AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> workTiles)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket packet = AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket.GetPacket(workType, item, workTiles);
		packet.Send(this, LocalGamer);
	}

	public void SendEndWork(short playerId, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> workTiles, int rotation, bool usingTools)
	{
		if (IsHost)
		{
			AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket packet = AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket.GetPacket(playerId, workType, item, workTiles, usingTools, rotation);
			packet.Send(this, LocalGamer);
		}
		else
		{
			AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket packet2 = AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket.GetPacket(playerId, workType, item, workTiles, usingTools, rotation);
			packet2.Send(this, LocalGamer, session.Host);
		}
	}

	public void EnqueueTask(AvatarFarmOnline.Logic.Mode.Farm.Online.IOnlineTask task)
	{
		if (multiplayerState == MultiplayerStates.Playing)
		{
			task.Process(this);
		}
		else
		{
			postLoadTasks.Enqueue(task);
		}
	}

	public void Reset()
	{
		Dispose(totalDispose: false);
	}

	private void Dispose(bool totalDispose)
	{
		enableUpdates = false;
		if (this.session != null)
		{
			Session session = this.session;
			this.session = null;
			session.Dispose();
		}
		host = null;
	}

	public void Dispose()
	{
		Dispose(totalDispose: true);
	}
}
