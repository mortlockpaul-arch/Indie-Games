using System;

namespace Quasar.GameUtils.Network;

public class NetworkSessionJoinException : Exception
{
	public SessionJoinError JoinError { get; private set; }

	public NetworkSessionJoinException(SessionJoinError error)
	{
		JoinError = error;
	}
}
