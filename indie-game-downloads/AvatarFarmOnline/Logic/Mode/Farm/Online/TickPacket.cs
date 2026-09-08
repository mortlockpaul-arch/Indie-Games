using System;
using AvatarFarmOnline.Logic.Stage;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class TickPacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	public class TickTask : AvatarFarmOnline.Logic.Mode.Farm.Online.IOnlineTask
	{
		private int tick;

		public TickTask(int tick)
		{
			this.tick = tick;
		}

		public void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
		{
			multiplayer.Stage.FarmData.Tick(Math.Max(0, tick - multiplayer.Stage.FarmData.WorldTicks));
		}
	}

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket sendPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket();

	private int tick;

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket receivePacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket();

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket GetPacket(AvatarFarmOnline.Logic.Stage.FarmData farmData)
	{
		sendPacket.tick = farmData.WorldTicks;
		return sendPacket;
	}

	protected TickPacket()
		: base(NetworkPacketType.Tick, SendDataOptions.ReliableInOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.PacketWriter.Write(tick);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, INetworkGamer sender)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket tickPacket = receivePacket;
		tickPacket.tick = multiplayer.PacketReader.ReadInt32();
		return tickPacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		if (multiplayer.MultiplayerState == AvatarFarmOnline.Logic.Mode.Farm.Online.Online.MultiplayerStates.Playing)
		{
			multiplayer.Stage.FarmData.Tick(Math.Max(0, tick - multiplayer.Stage.FarmData.WorldTicks));
		}
		else
		{
			multiplayer.EnqueueTask(new TickTask(tick));
		}
	}
}
