namespace Microsoft.Xna.Framework.Net;

public sealed class AvailableNetworkSession
{
	public int CurrentGamerCount { get; private set; }

	public string HostGamertag { get; private set; }

	public int OpenPrivateGamerSlots { get; private set; }

	public int OpenPublicGamerSlots { get; private set; }

	public QualityOfService QualityOfService { get; private set; }

	public NetworkSessionProperties SessionProperties { get; private set; }

	internal AvailableNetworkSession(int numGamers, string host, int privateSlots, int publicSlots, NetworkSessionProperties properties, QualityOfService qos)
	{
		CurrentGamerCount = numGamers;
		HostGamertag = host;
		OpenPrivateGamerSlots = privateSlots;
		OpenPublicGamerSlots = publicSlots;
		SessionProperties = properties;
		QualityOfService = qos;
	}
}
