using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal class ToolDefinition : AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition
{
	private ToolTypes toolType;

	private int maxFuel;

	private int toolSize;

	private AvatarFarmOnline.Logic.Money refillPrice;

	private int refillXp;

	public ToolTypes ToolType => toolType;

	public int MaxFuel => maxFuel;

	public int ToolSize => toolSize;

	public AvatarFarmOnline.Logic.Money RefillPrice => refillPrice;

	public int RefillXp => refillXp;

	public ToolDefinition()
		: base(ItemCategory.Tool)
	{
	}

	protected override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		toolType = AvatarFarmOnline.Logic.Parsing.ParseToolTypes(xe.GetAttribute("type"));
		maxFuel = xe.ParseIntAttribute("max_fuel");
		refillXp = xe.ParseIntAttribute("refill_xp");
		toolSize = xe.ParseIntAttribute("tool_size", 1);
		refillPrice = AvatarFarmOnline.Logic.Money.Parse(xe, "Refill");
	}
}
