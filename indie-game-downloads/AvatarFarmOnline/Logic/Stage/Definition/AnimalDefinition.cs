using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal class AnimalDefinition : AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition
{
	private int feedAmount;

	private int plantXp;

	private int gatherXp;

	private int feedXp;

	private AvatarFarmOnline.Logic.Money gatherMoney;

	private AvatarFarmOnline.Logic.Money feedMoney;

	private int feedInterval;

	public int FeedAmount => feedAmount;

	public int PlantXp => plantXp;

	public int GatherXp => gatherXp;

	public int FeedXp => feedXp;

	public AvatarFarmOnline.Logic.Money GatherMoney => gatherMoney;

	public AvatarFarmOnline.Logic.Money FeedMoney => feedMoney;

	public int FeedInterval => feedInterval;

	public AnimalDefinition()
		: base(ItemCategory.Animal)
	{
	}

	protected override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		feedAmount = xe.ParseIntAttribute("feed_amount");
		feedInterval = xe.ParseIntAttribute("feed");
		plantXp = xe.ParseIntAttribute("plant_xp");
		gatherXp = xe.ParseIntAttribute("gather_xp", plantXp * 3);
		feedXp = xe.ParseIntAttribute("feed_xp");
		gatherMoney = AvatarFarmOnline.Logic.Money.Parse(xe, "Gather");
		feedMoney = AvatarFarmOnline.Logic.Money.Parse(xe, "Feed");
	}
}
