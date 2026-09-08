using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal class DecorationTile : AvatarFarmOnline.Logic.Stage.Contents.BaseBuildingContents
{
	private AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration;

	private int decorationId;

	public override int HackCheck => 40 + decorationId * 5;

	public AvatarFarmOnline.Logic.Stage.Buildings.Decoration Decoration => decoration;

	public int DecorationId => decorationId;

	public DecorationTile(AvatarFarmOnline.Logic.Stage.FarmTile farmData)
		: base(farmData, TileContentsType.Decorative)
	{
	}

	public DecorationTile(AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration, AvatarFarmOnline.Logic.Stage.FarmTile farmData)
		: base(decoration, farmData, TileContentsType.Decorative)
	{
		decorationId = decoration.Id;
		this.decoration = decoration;
	}

	public override void Tick(int ticksPassed)
	{
	}

	public override bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		if (workType == AvatarFarmOnline.Logic.WorkType.Recycle)
		{
			decoration.Recycle(player);
			return true;
		}
		return false;
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
		decorationId = xe.ParseIntAttribute("decoration");
		decoration = farmTile.FarmData.GetDecoration(decorationId);
		base.BaseBuilding = decoration;
	}

	public override void ToXml(XElement xe)
	{
		base.ToXml(xe);
		xe.SetIntAttribute("decoration", decorationId);
	}
}
