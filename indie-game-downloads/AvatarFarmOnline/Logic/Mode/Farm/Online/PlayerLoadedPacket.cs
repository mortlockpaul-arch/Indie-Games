using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class PlayerLoadedPacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	private class PlayerLoadedTask : AvatarFarmOnline.Logic.Mode.Farm.Online.IOnlineTask
	{
		private short id;

		private INetworkGamer sender;

		private Vector2 position;

		private int level;

		public void SetValues(INetworkGamer sender, short id, Vector2 position, int level)
		{
			this.id = id;
			this.position = position;
			this.sender = sender;
			this.level = level;
		}

		public void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
		{
			multiplayer.PlayerLoadedReceived(sender, id, position, level);
		}
	}

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket sendPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket();

	private short id;

	private Vector2 position;

	private INetworkGamer sender;

	private int level;

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket receivePacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket();

	private PlayerLoadedTask playerLoadedTask = new PlayerLoadedTask();

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket GetPacket(AvatarFarmOnline.Logic.Stage.LocalPlayer player)
	{
		sendPacket.id = player.Selection.Id;
		sendPacket.position = player.Position;
		sendPacket.level = player.Level;
		return sendPacket;
	}

	protected PlayerLoadedPacket()
		: base(NetworkPacketType.PlayerLoaded, SendDataOptions.ReliableInOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.PacketWriter.Write(id);
		multiplayer.PacketWriter.Write(position);
		multiplayer.PacketWriter.Write((short)level);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, INetworkGamer sender)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket playerLoadedPacket = receivePacket;
		playerLoadedPacket.id = multiplayer.PacketReader.ReadInt16();
		playerLoadedPacket.sender = sender;
		playerLoadedPacket.position = multiplayer.PacketReader.ReadVector2();
		playerLoadedPacket.level = multiplayer.PacketReader.ReadInt16();
		return playerLoadedPacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		playerLoadedTask.SetValues(sender, id, position, level);
		if (multiplayer.MultiplayerState == AvatarFarmOnline.Logic.Mode.Farm.Online.Online.MultiplayerStates.Playing)
		{
			playerLoadedTask.Process(multiplayer);
			return;
		}
		multiplayer.EnqueueTask(playerLoadedTask);
		playerLoadedTask = new PlayerLoadedTask();
	}
}
