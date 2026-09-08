using System;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal abstract class Packet
{
	private NetworkPacketType packetType;

	private SendDataOptions sendDataOptions;

	public NetworkPacketType PacketType => packetType;

	public SendDataOptions SendOptions => sendDataOptions;

	protected Packet(NetworkPacketType packetType, SendDataOptions sendDataOptions)
	{
		this.packetType = packetType;
		this.sendDataOptions = sendDataOptions;
	}

	protected abstract void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer);

	public void Send(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, ILocalNetworkGamer gamer, INetworkGamer recipient)
	{
		multiplayer.PacketWriter.Write((int)PacketType);
		writePacket(multiplayer);
		try
		{
			gamer.SendData(multiplayer.PacketWriter, SendOptions, recipient);
		}
		catch (Exception)
		{
		}
	}

	public abstract void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer);

	public void Send(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, ILocalNetworkGamer gamer)
	{
		multiplayer.PacketWriter.Write((int)PacketType);
		writePacket(multiplayer);
		gamer.SendData(multiplayer.PacketWriter, SendOptions);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.Packet Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, ILocalNetworkGamer g)
	{
		g.ReceiveData(multiplayer.PacketReader, out var sender);
		NetworkPacketType networkPacketType = (NetworkPacketType)multiplayer.PacketReader.ReadInt32();
		if (sender.IsLocal && !AllowLocalSenders(networkPacketType))
		{
			return null;
		}
		if (!AllowMultipleNotifications(networkPacketType) && multiplayer.GetLocalGamer(0) != g)
		{
			return null;
		}
		return networkPacketType switch
		{
			NetworkPacketType.FarmData => AvatarFarmOnline.Logic.Mode.Farm.Online.FarmDataPacket.Receive(multiplayer), 
			NetworkPacketType.PlayerLoaded => AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLoadedPacket.Receive(multiplayer, sender), 
			NetworkPacketType.PlayerUpdate => AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerUpdatePacket.Receive(multiplayer, sender), 
			NetworkPacketType.Tick => AvatarFarmOnline.Logic.Mode.Farm.Online.TickPacket.Receive(multiplayer, sender), 
			NetworkPacketType.WorkStart => AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket.Receive(multiplayer, sender), 
			NetworkPacketType.WorkEnd => AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket.Receive(multiplayer), 
			NetworkPacketType.PlayerLevel => AvatarFarmOnline.Logic.Mode.Farm.Online.PlayerLevelPacket.Receive(multiplayer, sender), 
			NetworkPacketType.UpdatePermissions => AvatarFarmOnline.Logic.Mode.Farm.Online.UpdatePermissionsPacket.Receive(multiplayer), 
			_ => null, 
		};
	}

	public static bool AllowLocalSenders(NetworkPacketType type)
	{
		return false;
	}

	public static bool AllowMultipleNotifications(NetworkPacketType type)
	{
		return false;
	}
}
