using System.Xml.Linq;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal class DecorationDefinition : AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition
{
	public DecorationDefinition()
		: base(ItemCategory.Decoration)
	{
	}

	protected override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
	}
}
