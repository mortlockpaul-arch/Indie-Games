using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class UpdatePermissionsPacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	private static AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket sendPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket();

	private AvatarFarmOnline.Logic.PlayerPermissions friendPermissions;

	private AvatarFarmOnline.Logic.PlayerPermissions publicPermissions;

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket receivePacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket();

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket GetPacket(AvatarFarmOnline.Logic.PlayerPermissions friendPermissions, AvatarFarmOnline.Logic.PlayerPermissions publicPermissions)
	{
		sendPacket.friendPermissions = friendPermissions;
		sendPacket.publicPermissions = publicPermissions;
		return sendPacket;
	}

	protected UpdatePermissionsPacket()
		: base(NetworkPacketType.UpdatePermissions, SendDataOptions.ReliableInOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.PacketWriter.Write((short)friendPermissions);
		multiplayer.PacketWriter.Write((short)publicPermissions);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		receivePacket.friendPermissions = (AvatarFarmOnline.Logic.PlayerPermissions)multiplayer.PacketReader.ReadInt16();
		receivePacket.publicPermissions = (AvatarFarmOnline.Logic.PlayerPermissions)multiplayer.PacketReader.ReadInt16();
		return receivePacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.UpdatedPermissions(friendPermissions, publicPermissions);
	}
}
