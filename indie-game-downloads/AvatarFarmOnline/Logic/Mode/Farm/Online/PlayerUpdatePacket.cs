using AvatarFarmOnline.Logic.Stage;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class PlayerUpdatePacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	private static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket sendPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket();

	private short id;

	private AvatarFarmOnline.Logic.Stage.PlayerSnapshot snapshot;

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket receivePacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket();

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket GetPacket(AvatarFarmOnline.Logic.Stage.Player player)
	{
		sendPacket.id = player.Selection.Id;
		sendPacket.snapshot = player.GetSnapshot();
		return sendPacket;
	}

	protected PlayerUpdatePacket()
		: base(NetworkPacketType.PlayerUpdate, SendDataOptions.InOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.PacketWriter.Write(id);
		snapshot.WritePacket(multiplayer.PacketWriter);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, INetworkGamer sender)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket playerUpdatePacket = receivePacket;
		playerUpdatePacket.id = multiplayer.PacketReader.ReadInt16();
		AvatarFarmOnline.Logic.Stage.PlayerSnapshot playerSnapshot = default(AvatarFarmOnline.Logic.Stage.PlayerSnapshot);
		playerSnapshot.ReadPacket(multiplayer.PacketReader);
		playerUpdatePacket.snapshot = playerSnapshot;
		return playerUpdatePacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.GetPlayerFor(id)?.UpdateData(ref snapshot, fullUpdate: false);
	}
}
