using System.Collections.Generic;
using AvatarFarmOnline.Logic.Mode.Farm;

namespace AvatarFarmOnline.Logic.Stage;

internal class StageParameters
{
	private List<AvatarFarmOnline.Logic.Stage.PlayerSpawnData> playerSelections = new List<AvatarFarmOnline.Logic.Stage.PlayerSpawnData>();

	private AvatarFarmOnline.Logic.Mode.Farm.FarmHeader farmHeader;

	public List<AvatarFarmOnline.Logic.Stage.PlayerSpawnData> PlayerSpawns => playerSelections;

	public AvatarFarmOnline.Logic.Mode.Farm.FarmHeader FarmHeader
	{
		get
		{
			return farmHeader;
		}
		set
		{
			farmHeader = value;
		}
	}

	public void AddSelection(AvatarFarmOnline.Logic.Stage.PlayerSpawnData ps)
	{
		playerSelections.Add(ps);
	}
}
