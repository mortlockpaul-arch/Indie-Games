using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage.Definition;

internal abstract class ItemDefinition
{
	public enum ItemCategory
	{
		Plant,
		Tree,
		Animal,
		Building,
		Tool,
		Decoration,
		Count
	}

	private bool availableInTrial;

	private bool hasDescription;

	private string description;

	private string id;

	private bool needsUnlockedGame;

	private string gameNeeded;

	private string name;

	private int minLevel;

	private AvatarFarmOnline.Logic.Money price;

	private AvatarFarmOnline.Logic.Money recycleMoney;

	private ItemCategory category;

	private string buildingNeeded;

	public bool AvailableInTrial => availableInTrial;

	public bool HasDescription => hasDescription;

	public string Description => description;

	public string Id => id;

	public bool NeedsUnlockedGame => needsUnlockedGame;

	public string GameNeeded => gameNeeded;

	public string Name => name;

	public int MinLevel => minLevel;

	public AvatarFarmOnline.Logic.Money Price => price;

	public AvatarFarmOnline.Logic.Money RecycleMoney => recycleMoney;

	public ItemCategory Category => category;

	public bool IsBuildingNeeded => buildingNeeded.Length > 0;

	public string BuildingNeeded => buildingNeeded;

	public ItemDefinition(ItemCategory category)
	{
		this.category = category;
	}

	protected virtual void ParseXml(XElement xe)
	{
		id = xe.GetAttribute("id");
		name = category.ToString().ToUpper() + '_' + id.ToUpper() + "_NAME";
		availableInTrial = xe.ParseBoolAttribute("trial_available");
		description = category.ToString().ToUpper() + '_' + id.ToUpper() + "_DESCRIPTION";
		buildingNeeded = xe.GetAttribute("buildingNeeded");
		hasDescription = xe.ParseBoolAttribute("hasDescription", defaultValue: false);
		gameNeeded = xe.GetAttribute("gameNeeded");
		if (gameNeeded.Length > 0)
		{
			needsUnlockedGame = true;
		}
		minLevel = xe.ParseIntAttribute("minLevel");
		price = AvatarFarmOnline.Logic.Money.Parse(xe, "Price");
		recycleMoney = AvatarFarmOnline.Logic.Money.Parse(xe, "Recycle", price / 2);
	}

	public static AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition Parse(XElement xe)
	{
		ItemCategory itemCategory = GetCategory(xe.Name.LocalName);
		AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition itemDefinition = null;
		itemDefinition = itemCategory switch
		{
			ItemCategory.Plant => new AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition(), 
			ItemCategory.Decoration => new AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition(), 
			ItemCategory.Tree => new AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition(), 
			ItemCategory.Tool => new AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition(), 
			ItemCategory.Animal => new AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition(), 
			_ => new AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition(), 
		};
		itemDefinition.ParseXml(xe);
		return itemDefinition;
	}

	private static ItemCategory GetCategory(string elementName)
	{
		return elementName switch
		{
			"Plant" => ItemCategory.Plant, 
			"Tree" => ItemCategory.Tree, 
			"Building" => ItemCategory.Building, 
			"Tool" => ItemCategory.Tool, 
			"Animal" => ItemCategory.Animal, 
			_ => ItemCategory.Decoration, 
		};
	}

	public static string GetCategoryTitleText(ItemCategory category)
	{
		return category switch
		{
			ItemCategory.Plant => "CATEGORY_PLANTS", 
			ItemCategory.Tree => "CATEGORY_TREES", 
			ItemCategory.Building => "CATEGORY_BUILDINGS", 
			ItemCategory.Decoration => "CATEGORY_DECORATION", 
			ItemCategory.Tool => "CATEGORY_TOOLS", 
			ItemCategory.Animal => "CATEGORY_ANIMALS", 
			_ => "", 
		};
	}
}
