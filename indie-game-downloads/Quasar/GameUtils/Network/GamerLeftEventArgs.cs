namespace Quasar.GameUtils.Network;

public class GamerLeftEventArgs
{
	public INetworkGamer Gamer { get; private set; }

	public GamerLeftEventArgs(INetworkGamer gamer)
	{
		Gamer = gamer;
	}
}
