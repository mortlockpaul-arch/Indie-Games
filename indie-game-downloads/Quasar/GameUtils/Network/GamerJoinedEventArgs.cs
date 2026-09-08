namespace Quasar.GameUtils.Network;

public class GamerJoinedEventArgs
{
	public INetworkGamer Gamer { get; private set; }

	public GamerJoinedEventArgs(INetworkGamer gamer)
	{
		Gamer = gamer;
	}
}
