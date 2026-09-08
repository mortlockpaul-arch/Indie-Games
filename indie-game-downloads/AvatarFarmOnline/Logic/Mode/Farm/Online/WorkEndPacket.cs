using System.Collections.Generic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class WorkEndPacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	private class WorkEndTask : AvatarFarmOnline.Logic.Mode.Farm.Online.IOnlineTask
	{
		private List<Int2> farmTiles = new List<Int2>();

		private List<AvatarFarmOnline.Logic.Stage.FarmTile> farmTiles2 = new List<AvatarFarmOnline.Logic.Stage.FarmTile>();

		private short id;

		private AvatarFarmOnline.Logic.WorkType workType;

		private AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item;

		private bool usingTools;

		private int rotation;

		public void SetValues(short id, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<Int2> farmTiles, bool usingTools, int rotation)
		{
			this.id = id;
			this.workType = workType;
			this.item = item;
			this.usingTools = usingTools;
			this.rotation = rotation;
			this.farmTiles.Clear();
			this.farmTiles.AddRange(farmTiles);
		}

		public void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
		{
			AvatarFarmOnline.Logic.Stage.Player playerFor = multiplayer.GetPlayerFor(id);
			farmTiles2.Clear();
			foreach (Int2 farmTile in farmTiles)
			{
				farmTiles2.Add(multiplayer.Stage.FarmData.GetTile(farmTile));
			}
			if (playerFor != null)
			{
				playerFor.EndWork(workType, item, farmTiles2, rotation, usingTools);
			}
			else
			{
				multiplayer.Stage.LocalPlayer.EndWork(workType, item, farmTiles2, rotation, usingTools);
			}
			if (multiplayer.IsHost)
			{
				multiplayer.SendEndWork(id, workType, item, farmTiles2, rotation, usingTools);
			}
		}
	}

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket sendPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket();

	private short id;

	private AvatarFarmOnline.Logic.WorkType workType;

	private AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item;

	private bool usingTools;

	private int rotation;

	private List<Int2> farmTiles = new List<Int2>();

	private static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket receivePacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket();

	private WorkEndTask workEndTask = new WorkEndTask();

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket GetPacket(short playerId, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> farmTiles, bool usingTools, int rotation)
	{
		sendPacket.id = playerId;
		sendPacket.workType = workType;
		sendPacket.item = item;
		sendPacket.farmTiles.Clear();
		foreach (AvatarFarmOnline.Logic.Stage.FarmTile farmTile in farmTiles)
		{
			sendPacket.farmTiles.Add(farmTile.Tile);
		}
		sendPacket.usingTools = usingTools;
		sendPacket.rotation = rotation;
		return sendPacket;
	}

	protected WorkEndPacket()
		: base(NetworkPacketType.WorkEnd, SendDataOptions.ReliableInOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.PacketWriter.Write(id);
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
		multiplayer.PacketWriter.Write(usingTools);
		multiplayer.PacketWriter.Write((byte)rotation);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		IPacketReader packetReader = multiplayer.PacketReader;
		AvatarFarmOnline.Logic.Mode.Farm.Online.WorkEndPacket workEndPacket = receivePacket;
		workEndPacket.id = packetReader.ReadInt16();
		workEndPacket.workType = (AvatarFarmOnline.Logic.WorkType)packetReader.ReadInt16();
		if (packetReader.ReadBoolean())
		{
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory category = (AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory)packetReader.ReadInt16();
			string name = packetReader.ReadString();
			workEndPacket.item = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetItem(category, name);
		}
		int num = packetReader.ReadInt16();
		workEndPacket.farmTiles.Clear();
		for (int i = 0; i < num; i++)
		{
			workEndPacket.farmTiles.Add(packetReader.ReadInt2());
		}
		workEndPacket.usingTools = packetReader.ReadBoolean();
		workEndPacket.rotation = packetReader.ReadByte();
		return workEndPacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		workEndTask.SetValues(id, workType, item, farmTiles, usingTools, rotation);
		if (multiplayer.MultiplayerState == AvatarFarmOnline.Logic.Mode.Farm.Online.Online.MultiplayerStates.Playing)
		{
			workEndTask.Process(multiplayer);
			return;
		}
		multiplayer.EnqueueTask(workEndTask);
		workEndTask = new WorkEndTask();
	}
}
