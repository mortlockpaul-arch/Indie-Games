using System.Collections.Generic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class WorkStartPacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	private static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket sendPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket();

	private AvatarFarmOnline.Logic.WorkType workType;

	private AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item;

	private List<Int2> farmTiles = new List<Int2>();

	private List<AvatarFarmOnline.Logic.Stage.FarmTile> farmTiles2 = new List<AvatarFarmOnline.Logic.Stage.FarmTile>();

	private INetworkGamer sender;

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket receivePacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket();

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket GetPacket(AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> farmTiles)
	{
		sendPacket.workType = workType;
		sendPacket.item = item;
		sendPacket.farmTiles.Clear();
		foreach (AvatarFarmOnline.Logic.Stage.FarmTile farmTile in farmTiles)
		{
			sendPacket.farmTiles.Add(farmTile.Tile);
		}
		return sendPacket;
	}

	protected WorkStartPacket()
		: base(NetworkPacketType.WorkStart, SendDataOptions.ReliableInOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.PacketWriter.Write((short)workType);
		if (item != null)
		{
			multiplayer.PacketWriter.Write(value: true);
			multiplayer.PacketWriter.Write((short)item.Category);
			multiplayer.PacketWriter.Write(item.Id);
		}
		else
		{
			multiplayer.PacketWriter.Write(value: false);
		}
		multiplayer.PacketWriter.Write((short)farmTiles.Count);
		foreach (Int2 farmTile in farmTiles)
		{
			multiplayer.PacketWriter.Write(farmTile);
		}
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer, INetworkGamer sender)
	{
		IPacketReader packetReader = multiplayer.PacketReader;
		AvatarFarmOnline.Logic.Mode.Farm.Online.WorkStartPacket workStartPacket = receivePacket;
		workStartPacket.workType = (AvatarFarmOnline.Logic.WorkType)packetReader.ReadInt16();
		if (packetReader.ReadBoolean())
		{
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory category = (AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory)packetReader.ReadInt16();
			string name = packetReader.ReadString();
			workStartPacket.item = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetItem(category, name);
		}
		int num = packetReader.ReadInt16();
		workStartPacket.farmTiles.Clear();
		for (int i = 0; i < num; i++)
		{
			workStartPacket.farmTiles.Add(packetReader.ReadInt2());
		}
		workStartPacket.sender = sender;
		return workStartPacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		if (!(multiplayer.GetPlayerFor(sender.Id) is AvatarFarmOnline.Logic.Stage.NetworkPlayer networkPlayer) || multiplayer.MultiplayerState != AvatarFarmOnline.Logic.Mode.Farm.Online.Online.MultiplayerStates.Playing)
		{
			return;
		}
		farmTiles2.Clear();
		foreach (Int2 farmTile in farmTiles)
		{
			farmTiles2.Add(multiplayer.Stage.FarmData.GetTile(farmTile));
		}
		networkPlayer.StartWork(workType, item, farmTiles2);
	}
}
