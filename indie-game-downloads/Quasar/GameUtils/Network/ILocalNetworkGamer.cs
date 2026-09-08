using Quasar.GameUtils.Game;

namespace Quasar.GameUtils.Network;

public interface ILocalNetworkGamer : INetworkGamer
{
	bool IsDataAvailable { get; }

	new bool IsReady { get; set; }

	ISignedInGamer SignedInGamer { get; }

	void SendData(IPacketWriter packetWriter, SendDataOptions sendOptions);

	void SendData(IPacketWriter packetWriter, SendDataOptions sendOptions, INetworkGamer receiver);

	int ReceiveData(IPacketReader packetReader, out INetworkGamer sender);

	void SendPartyInvites();
}
