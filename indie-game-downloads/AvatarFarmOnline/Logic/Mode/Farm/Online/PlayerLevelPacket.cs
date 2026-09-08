using AvatarFarmOnline.Logic.Stage;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class PlayerLevelPacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	private static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket sendPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket();

	private short id;

	private int level;

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket receivePacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket();

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket GetPacket(AvatarFarmOnline.Logic.Stage.LocalPlayer player)
	{
		sendPacket.level = player.Level;
		sendPacket.id = player.Selection.Id;
		return sendPacket;
	}

	protected PlayerLevelPacket()
		: base(NetworkPacketType.PlayerLevel, SendDataOptions.ReliableInOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.PacketWriter.Write(id);
		multiplayer.PacketWriter.Write((short)level);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, INetworkGamer sender)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket playerLevelPacket = receivePacket;
		playerLevelPacket.id = multiplayer.PacketReader.ReadInt16();
		playerLevelPacket.level = multiplayer.PacketReader.ReadInt16();
		return playerLevelPacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		if (multiplayer.GetPlayerFor(id) is AvatarFarmOnline.Logic.Stage.NetworkPlayer networkPlayer)
		{
			networkPlayer.SetLevel(level);
		}
	}
}
