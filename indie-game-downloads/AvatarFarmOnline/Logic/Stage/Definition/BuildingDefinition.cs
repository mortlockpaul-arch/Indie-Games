using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal class BuildingDefinition : AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition
{
	private bool canBeGathered;

	private int gatherPeriod;

	private AvatarFarmOnline.Logic.Money gatherAmount;

	private int gatherXp;

	private bool canAccumulateItems;

	private string itemAccumulationCategory;

	private string itemAccumulationCategoryText;

	private int itemAccumulationCount;

	private AvatarFarmOnline.Logic.Money itemAccumulationAmount;

	private int itemAccumulationXp;

	private bool canGamble;

	private AvatarFarmOnline.Logic.Money gamblePrice;

	private List<KeyValuePair<int, AvatarFarmOnline.Logic.Money>> gamblePrizes;

	public bool CanBeGathered => canBeGathered;

	public int GatherPeriod => gatherPeriod;

	public AvatarFarmOnline.Logic.Money GatherAmount => gatherAmount;

	public int GatherXp => gatherXp;

	public bool CanAccumulateItems => canAccumulateItems;

	public string ItemAccumulationCategory => itemAccumulationCategory;

	public string ItemAccumulationCategoryText => itemAccumulationCategoryText;

	public int ItemAccumulationCount => itemAccumulationCount;

	public AvatarFarmOnline.Logic.Money ItemAccumulationAmount => itemAccumulationAmount;

	public int ItemAccumulationXp => itemAccumulationXp;

	public bool CanGamble => canGamble;

	public AvatarFarmOnline.Logic.Money GamblePrice => gamblePrice;

	public List<KeyValuePair<int, AvatarFarmOnline.Logic.Money>> GamblePrizes => gamblePrizes;

	public BuildingDefinition()
		: base(ItemCategory.Building)
	{
	}

	protected override void ParseXml(XElement xe)
	{
		base.ParseXml(xe);
		XElement xElement = xe.Element("Gather");
		if (xElement != null)
		{
			canBeGathered = true;
			gatherPeriod = xElement.ParseIntAttribute("period");
			gatherAmount = AvatarFarmOnline.Logic.Money.Parse(xElement, "Money");
			gatherXp = xElement.ParseIntAttribute("xp");
		}
		else
		{
			canBeGathered = false;
		}
		XElement xElement2 = xe.Element("Accumulation");
		if (xElement2 != null)
		{
			canAccumulateItems = true;
			itemAccumulationCategory = xElement2.GetAttribute("category");
			itemAccumulationCategoryText = "PLANT_CATEGORY_" + itemAccumulationCategory.ToUpper();
			itemAccumulationCount = xElement2.ParseIntAttribute("count");
			itemAccumulationAmount = AvatarFarmOnline.Logic.Money.Parse(xElement2, "Money");
			itemAccumulationXp = xElement2.ParseIntAttribute("xp");
		}
		else
		{
			canAccumulateItems = false;
		}
		XElement xElement3 = xe.Element("Gamble");
		if (xElement3 != null)
		{
			canGamble = true;
			gamblePrice = AvatarFarmOnline.Logic.Money.Parse(xElement3, "Price");
			gamblePrizes = new List<KeyValuePair<int, AvatarFarmOnline.Logic.Money>>();
			{
				foreach (XElement item in xElement3.Elements("Prize"))
				{
					int key = item.ParseIntAttribute("chance", 0);
					AvatarFarmOnline.Logic.Money value = AvatarFarmOnline.Logic.Money.Parse(item, "Money");
					gamblePrizes.Add(new KeyValuePair<int, AvatarFarmOnline.Logic.Money>(key, value));
				}
				return;
			}
		}
		canGamble = false;
	}
}
