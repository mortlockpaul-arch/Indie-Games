using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal class PlantDefinition : AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition
{
	private AvatarFarmOnline.Logic.Seasons plantSeasons;

	private int growTime;

	private int gatherTime;

	private int plantXp;

	private int gatherXp;

	private AvatarFarmOnline.Logic.Money gatherMoney;

	private AvatarFarmOnline.Logic.Money gatherWitheredMoney;

	private string plantCategory;

	private string plantCategoryText;

	private int maxWaterLevel;

	public AvatarFarmOnline.Logic.Seasons PlantSeasons => plantSeasons;

	public int GrowTime => growTime;

	public int GatherTime => gatherTime;

	public int PlantXp => plantXp;

	public int GatherXp => gatherXp;

	public AvatarFarmOnline.Logic.Money GatherMoney => gatherMoney;

	public AvatarFarmOnline.Logic.Money GatherWitheredMoney => gatherWitheredMoney;

	public string PlantCategory => plantCategory;

	public string PlantCategoryText => plantCategoryText;

	public int MaxWaterLevel => maxWaterLevel;

	public PlantDefinition()
		: base(ItemCategory.Plant)
	{
	}

	protected override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		plantSeasons = AvatarFarmOnline.Logic.Parsing.ParseSeasons(xe.GetAttribute("seasons"));
		growTime = xe.ParseIntAttribute("grow");
		plantCategory = xe.GetAttribute("category");
		plantCategoryText = "PLANT_CATEGORY_" + plantCategory.ToUpper();
		gatherTime = xe.ParseIntAttribute("gather", growTime);
		plantXp = xe.ParseIntAttribute("plant_xp");
		gatherXp = xe.ParseIntAttribute("gather_xp", plantXp * 3);
		gatherMoney = AvatarFarmOnline.Logic.Money.Parse(xe, "Gather");
		gatherWitheredMoney = gatherMoney / 5;
		maxWaterLevel = xe.ParseIntAttribute("max_water", growTime / 5);
	}
}
