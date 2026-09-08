using AvatarFarmOnline.Logic.Stage.Buildings;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal abstract class BaseBuildingContents : AvatarFarmOnline.Logic.Stage.TileContents
{
	private AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding baseBuilding;

	public AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding BaseBuilding
	{
		get
		{
			return baseBuilding;
		}
		protected set
		{
			baseBuilding = value;
		}
	}

	protected BaseBuildingContents(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, TileContentsType contentsType)
		: base(farmTile, contentsType)
	{
	}

	protected BaseBuildingContents(AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding baseBuilding, AvatarFarmOnline.Logic.Stage.FarmTile farmTile, TileContentsType contentsType)
		: base(farmTile, contentsType)
	{
		this.baseBuilding = baseBuilding;
	}
}
