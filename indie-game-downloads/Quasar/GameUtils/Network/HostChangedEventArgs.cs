namespace Quasar.GameUtils.Network;

public class HostChangedEventArgs
{
	public INetworkGamer NewHost { get; private set; }

	public HostChangedEventArgs(INetworkGamer gamer)
	{
		NewHost = gamer;
	}
}
