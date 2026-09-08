using System;
using System.Collections.Generic;
using Quasar.GameUtils.Game;

namespace Quasar.GameUtils.Network;

public abstract class NetworkInterface
{
	protected static NetworkInterface instance;

	public static NetworkInterface Instance => instance;

	public abstract int FoundSessions { get; }

	public abstract void RegisterInviteAccepted(Action<object, InviteAcceptedArgs> eventHandler);

	public abstract Session JoinInvited(List<ISignedInGamer> gamers);

	public abstract IAvailableSessionCollection Find(SessionType sessionType, List<ISignedInGamer> players, ISessionProperties properties);

	public abstract Session Create(SessionType sessionType, List<ISignedInGamer> players, int playerNumber, int privateSlots, ISessionProperties properties);

	public abstract Session Join(IAvailableSession availableSession);

	public abstract Session JoinByAddress(List<ISignedInGamer> players, string text);

	public abstract ISessionProperties CreateSessionProperties();
}
