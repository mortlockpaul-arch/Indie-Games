using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal abstract class BaseBuildingDefinition : AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition
{
	private Int2 size;

	private int buildXp;

	private bool hasCollision;

	private bool hasShadow;

	public Int2 Size => size;

	public int BuildXp => buildXp;

	public bool HasCollision => hasCollision;

	public bool HasShadow => hasShadow;

	public BaseBuildingDefinition(ItemCategory category)
		: base(category)
	{
	}

	protected override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		size = xe.ParseInt2Attribute("size", Int2.One);
		buildXp = xe.ParseIntAttribute("build_xp");
		hasShadow = xe.ParseBoolAttribute("hasShadow", defaultValue: true);
		hasCollision = xe.ParseBoolAttribute("hasCollision", defaultValue: true);
	}
}
