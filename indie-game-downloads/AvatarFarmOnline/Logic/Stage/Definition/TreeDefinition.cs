using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal class TreeDefinition : AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition
{
	private AvatarFarmOnline.Logic.Seasons gatherSeasons;

	private int plantXp;

	private int gatherXp;

	private string plantCategory;

	private string plantCategoryText;

	private AvatarFarmOnline.Logic.Money gatherMoney;

	public AvatarFarmOnline.Logic.Seasons GatherSeasons => gatherSeasons;

	public int PlantXp => plantXp;

	public int GatherXp => gatherXp;

	public string PlantCategory => plantCategory;

	public string PlantCategoryText => plantCategoryText;

	public AvatarFarmOnline.Logic.Money GatherMoney => gatherMoney;

	public TreeDefinition()
		: base(ItemCategory.Tree)
	{
	}

	protected override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		gatherSeasons = AvatarFarmOnline.Logic.Parsing.ParseSeasons(xe.GetAttribute("gather_seasons"));
		plantCategory = xe.GetAttribute("category");
		plantCategoryText = "PLANT_CATEGORY_" + plantCategory.ToUpper();
		plantXp = xe.ParseIntAttribute("plant_xp");
		gatherXp = xe.ParseIntAttribute("gather_xp", (plantXp + 1) / 2);
		gatherMoney = AvatarFarmOnline.Logic.Money.Parse(xe, "Gather");
	}
}
