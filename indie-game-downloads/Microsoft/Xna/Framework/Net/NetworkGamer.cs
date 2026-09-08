using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

public class NetworkGamer : Gamer
{
	public bool HasLeftSession { get; private set; }

	public bool HasVoice { get; private set; }

	public byte Id => 0;

	public bool IsGuest { get; private set; }

	public bool IsHost => true;

	public bool IsLocal => this is LocalNetworkGamer;

	public bool IsMutedByLocalUser { get; private set; }

	public bool IsPrivateSlot { get; private set; }

	public bool IsReady { get; set; }

	public bool IsTalking { get; private set; }

	public NetworkMachine Machine { get; set; }

	public TimeSpan RoundtripTime { get; private set; }

	public NetworkSession Session { get; private set; }

	internal NetworkGamer(NetworkSession session)
		: base("Stub Gamer", "Stub Gamer")
	{
		Session = session;
		HasLeftSession = false;
		HasVoice = false;
		IsGuest = false;
		IsMutedByLocalUser = false;
		IsPrivateSlot = false;
		IsReady = false;
		IsTalking = false;
		Machine = new NetworkMachine();
		RoundtripTime = TimeSpan.Zero;
	}
}
