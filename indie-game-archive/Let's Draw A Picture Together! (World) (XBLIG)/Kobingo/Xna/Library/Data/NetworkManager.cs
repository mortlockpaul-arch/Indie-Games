using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace Kobingo.Xna.Library.Data;

public class NetworkManager
{
	private class WaitingNetworkSession
	{
		[CompilerGenerated]
		private NetworkSessionType _003CSessionType_003Ek__BackingField;

		public NetworkSessionType SessionType
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _003CSessionType_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_003CSessionType_003Ek__BackingField = value;
			}
		}

		public NetworkSessionProperties SessionProperties { get; set; }

		public IEnumerable<SignedInGamer> LocalGamers { get; set; }

		public int MaxGamers { get; set; }

		public int PrivateGamerSlots { get; set; }

		public bool IsInvited { get; set; }
	}

	private static WaitingNetworkSession WaitingSession { get; set; }

	public static NetworkSession Session { get; private set; }

	public static bool IsBusy { get; private set; }

	public static bool IsCanceled { get; private set; }

	public static event EventHandler Created;

	public static event EventHandler Closed;

	public static void FindCreate(NetworkSessionType sessionType, NetworkSessionProperties sessionProperties, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		IsCanceled = false;
		if (Session != null)
		{
			Close();
		}
		if (IsBusy)
		{
			WaitingNetworkSession waitingNetworkSession = new WaitingNetworkSession();
			waitingNetworkSession.SessionType = sessionType;
			waitingNetworkSession.SessionProperties = sessionProperties;
			waitingNetworkSession.LocalGamers = localGamers;
			waitingNetworkSession.MaxGamers = maxGamers;
			waitingNetworkSession.PrivateGamerSlots = privateGamerSlots;
			WaitingSession = waitingNetworkSession;
			return;
		}
		IsBusy = true;
		NetworkSession.BeginFind(sessionType, localGamers, sessionProperties, (AsyncCallback)delegate(IAsyncResult findResult)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			AvailableNetworkSessionCollection val = NetworkSession.EndFind(findResult);
			if (!HandleWaitingSessionAndCanceled())
			{
				if (((ReadOnlyCollection<AvailableNetworkSession>)(object)val).Count > 0)
				{
					NetworkSession.BeginJoin(((ReadOnlyCollection<AvailableNetworkSession>)(object)val)[0], (AsyncCallback)delegate(IAsyncResult joinResult)
					{
						//IL_0023: Unknown result type (might be due to invalid IL or missing references)
						try
						{
							Session = NetworkSession.EndJoin(joinResult);
							if (!HandleWaitingSessionAndCanceled())
							{
								OnCreated();
							}
						}
						catch
						{
							IsBusy = false;
							FindCreate(sessionType, sessionProperties, localGamers, maxGamers, privateGamerSlots);
						}
					}, (object)null);
				}
				else
				{
					NetworkSession.BeginCreate(sessionType, localGamers, maxGamers, privateGamerSlots, sessionProperties, (AsyncCallback)delegate(IAsyncResult result)
					{
						Session = NetworkSession.EndCreate(result);
						if (!HandleWaitingSessionAndCanceled())
						{
							OnCreated();
						}
					}, (object)null);
				}
			}
		}, (object)null);
	}

	public static void Create(NetworkSessionType sessionType, NetworkSessionProperties sessionProperties, IEnumerable<SignedInGamer> localGamers, int maxGamers, int privateGamerSlots)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		IsCanceled = false;
		if (Session != null)
		{
			Close();
		}
		if (IsBusy)
		{
			WaitingNetworkSession waitingNetworkSession = new WaitingNetworkSession();
			waitingNetworkSession.SessionType = sessionType;
			waitingNetworkSession.SessionProperties = sessionProperties;
			waitingNetworkSession.LocalGamers = localGamers;
			waitingNetworkSession.MaxGamers = maxGamers;
			waitingNetworkSession.PrivateGamerSlots = privateGamerSlots;
			WaitingSession = waitingNetworkSession;
			return;
		}
		IsBusy = true;
		NetworkSession.BeginCreate(sessionType, localGamers, maxGamers, privateGamerSlots, sessionProperties, (AsyncCallback)delegate(IAsyncResult result)
		{
			Session = NetworkSession.EndCreate(result);
			if (!HandleWaitingSessionAndCanceled())
			{
				OnCreated();
			}
		}, (object)null);
	}

	public static void JoinInvited(IEnumerable<SignedInGamer> localGamers)
	{
		IsCanceled = false;
		if (Session != null)
		{
			Close();
		}
		if (IsBusy)
		{
			WaitingNetworkSession waitingNetworkSession = new WaitingNetworkSession();
			waitingNetworkSession.LocalGamers = localGamers;
			waitingNetworkSession.IsInvited = true;
			WaitingSession = waitingNetworkSession;
			return;
		}
		IsBusy = true;
		NetworkSession.BeginJoinInvited(localGamers, (AsyncCallback)delegate(IAsyncResult result)
		{
			Session = NetworkSession.EndJoinInvited(result);
			if (!HandleWaitingSessionAndCanceled())
			{
				OnCreated();
			}
		}, (object)null);
	}

	private static void OnCreated()
	{
		if (Created != null)
		{
			Created(null, EventArgs.Empty);
		}
		IsBusy = false;
	}

	private static void OnClosed()
	{
		if (Closed != null)
		{
			Closed(null, EventArgs.Empty);
		}
	}

	private static bool HandleWaitingSessionAndCanceled()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (WaitingSession != null)
		{
			IsBusy = false;
			if (WaitingSession.IsInvited)
			{
				JoinInvited(WaitingSession.LocalGamers);
			}
			else
			{
				FindCreate(WaitingSession.SessionType, WaitingSession.SessionProperties, WaitingSession.LocalGamers, WaitingSession.MaxGamers, WaitingSession.PrivateGamerSlots);
			}
			WaitingSession = null;
			return true;
		}
		if (IsCanceled)
		{
			IsBusy = false;
			Close();
			return true;
		}
		return false;
	}

	public static void Close()
	{
		if (Session != null)
		{
			OnClosed();
			Session.Dispose();
			Session = null;
		}
		else
		{
			IsCanceled = true;
		}
		WaitingSession = null;
	}
}
