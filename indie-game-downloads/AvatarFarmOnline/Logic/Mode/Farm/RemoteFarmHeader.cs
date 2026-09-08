using System.Collections.Generic;
using AvatarFarmOnline.Logic.Mode.Farm.Online;
using AvatarFarmOnline.Logic.Stage;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal class RemoteFarmHeader : AvatarFarmOnline.Logic.Mode.Farm.FarmHeader
{
	private AvatarFarmOnline.Logic.Stage.FarmData farmData;

	private List<AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData> remotePlayers;

	public AvatarFarmOnline.Logic.Stage.FarmData FarmData => farmData;

	public List<AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData> RemotePlayers => remotePlayers;

	public RemoteFarmHeader(AvatarFarmOnline.Logic.Stage.FarmData farmData, List<AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData> remotePlayers)
	{
		this.farmData = farmData;
		level = farmData.PlayerData.Level;
		cash = farmData.PlayerData.Cash;
		coins = farmData.PlayerData.Coins;
		this.remotePlayers = remotePlayers;
	}

	public override AvatarFarmOnline.Logic.Stage.FarmData LoadFarm()
	{
		return farmData;
	}
}
