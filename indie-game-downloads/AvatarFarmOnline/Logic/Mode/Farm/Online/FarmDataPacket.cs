using System.Collections.Generic;
using AvatarFarmOnline.Logic.Stage;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm.Online;

internal class FarmDataPacket : AvatarFarmOnline.Logic.Mode.Farm.Online.Packet
{
	private AvatarFarmOnline.Logic.Stage.FarmData farmData;

	private List<AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData> spawnData;

	public FarmDataPacket(AvatarFarmOnline.Logic.Stage.Stage stage)
		: base(NetworkPacketType.FarmData, SendDataOptions.ReliableInOrder)
	{
		farmData = stage.FarmData;
		spawnData = new List<AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData>(stage.Players.Count);
		foreach (AvatarFarmOnline.Logic.Stage.Player player in stage.Players)
		{
			AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData item = new AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData(player);
			spawnData.Add(item);
		}
	}

	private FarmDataPacket()
		: base(NetworkPacketType.FarmData, SendDataOptions.ReliableInOrder)
	{
	}

	protected override void writePacket(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		farmData.WritePacket(multiplayer.PacketWriter);
		multiplayer.PacketWriter.Write(spawnData.Count);
		foreach (AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData spawnDatum in spawnData)
		{
			spawnDatum.WritePacket(multiplayer.PacketWriter);
		}
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.Online.FarmDataPacket Receive(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		AvatarFarmOnline.Logic.Mode.Farm.Online.FarmDataPacket farmDataPacket = new AvatarFarmOnline.Logic.Mode.Farm.Online.FarmDataPacket();
		AvatarFarmOnline.Logic.Stage.FarmData farmData = AvatarFarmOnline.Logic.Stage.FarmData.CreateFromPacket(multiplayer.PacketReader);
		farmDataPacket.farmData = farmData;
		int num = multiplayer.PacketReader.ReadInt32();
		farmDataPacket.spawnData = new List<AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData>(num);
		for (int i = 0; i < num; i++)
		{
			farmDataPacket.spawnData.Add(AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData.ReadPacket(multiplayer.PacketReader));
		}
		return farmDataPacket;
	}

	public override void Process(AvatarFarmOnline.Logic.Mode.Farm.Online.Online multiplayer)
	{
		multiplayer.FarmDataReceived(farmData, spawnData);
	}
}
