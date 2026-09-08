using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Contents;

internal class ToolTile : AvatarFarmOnline.Logic.Stage.Contents.BaseBuildingContents
{
	private AvatarFarmOnline.Logic.Stage.Buildings.Tool tool;

	private int toolId;

	public override int HackCheck => 50 + toolId * 5;

	public AvatarFarmOnline.Logic.Stage.Buildings.Tool Tool => tool;

	public int ToolId => toolId;

	public ToolTile(AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(farmTile, TileContentsType.Tool)
	{
	}

	public ToolTile(AvatarFarmOnline.Logic.Stage.Buildings.Tool tool, AvatarFarmOnline.Logic.Stage.FarmTile farmTile)
		: base(tool, farmTile, TileContentsType.Tool)
	{
		toolId = tool.Id;
		this.tool = tool;
	}

	public override void Tick(int ticksPassed)
	{
	}

	public override bool Work(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		switch (workType)
		{
		case AvatarFarmOnline.Logic.WorkType.Refill:
			if (tool.CanBeRefilled(out failReason))
			{
				tool.Refill(player);
				return true;
			}
			break;
		case AvatarFarmOnline.Logic.WorkType.Recycle:
			tool.Recycle(player);
			return true;
		}
		return false;
	}

	public override void FromXml(XElement xe)
	{
		base.FromXml(xe);
		toolId = xe.ParseIntAttribute("tool");
		tool = farmTile.FarmData.GetTool(toolId);
		base.BaseBuilding = tool;
	}

	public override void ToXml(XElement xe)
	{
		base.ToXml(xe);
		xe.SetIntAttribute("tool", toolId);
	}
}
