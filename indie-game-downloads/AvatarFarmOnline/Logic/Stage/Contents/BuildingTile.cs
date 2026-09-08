using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal class BuildingTile : AvatarFarmOnline.Logic.Stage.Contents.BaseBuildingContents
{
	private AvatarFarmOnline.Logic.Stage.Buildings.Building building;

	private int buildingId;

	public override int HackCheck => 30 + buildingId * 5;

	public AvatarFarmOnline.Logic.Stage.Buildings.Building Building => building;

	public int BuildingId => buildingId;

	public BuildingTile(AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Building)
	{
	}

	public BuildingTile(AvatarFarmOnline.Logic.Stage.Buildings.Building building, AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(building, farmTile, TileContentsType.Building)
	{
		buildingId = building.Id;
		this.building = building;
	}

	public override void Tick(int ticksPassed)
	{
	}

	public override bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		switch (workType)
		{
		case AvatarFarmOnline.Logic.WorkType.GatherBuilding:
			if (building.CanBeGathered(out failReason))
			{
				building.Gather(player);
				return true;
			}
			break;
		case AvatarFarmOnline.Logic.WorkType.Recycle:
			building.Recycle(player);
			return true;
		}
		return false;
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
		buildingId = xe.ParseIntAttribute("building");
		building = farmTile.FarmData.GetBuilding(buildingId);
		base.BaseBuilding = building;
	}

	public override void ToXml(XElement xe)
	{
		base.ToXml(xe);
		xe.SetIntAttribute("building", buildingId);
	}
}
