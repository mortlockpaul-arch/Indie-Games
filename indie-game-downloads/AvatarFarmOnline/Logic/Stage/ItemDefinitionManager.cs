using System.Collections.Generic;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage.Definition;
using Quasar.ContentPipeline;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage;

internal class ItemDefinitionManager
{
	private const string DEFINITIONS_FILE = "shop_items";

	private static AvatarFarmOnline.Logic.Stage.ItemDefinitionManager instance;

	private List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition> definitionsList;

	private List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition>[] categoryDefinitionsList;

	private Dictionary<string, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition>[] categoryDefinitions;

	public static AvatarFarmOnline.Logic.Stage.ItemDefinitionManager Instance => instance;

	public List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition> Definitions => definitionsList;

	public List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition> GetCategoryDefinitions(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory category)
	{
		return categoryDefinitionsList[(int)category];
	}

	public AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition GetPlant(string name)
	{
		return (AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)categoryDefinitions[0][name];
	}

	public AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition GetBuilding(string name)
	{
		return (AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)categoryDefinitions[3][name];
	}

	public AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition GetTool(string name)
	{
		return (AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)categoryDefinitions[4][name];
	}

	public AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition GetAnimal(string name)
	{
		return (AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)categoryDefinitions[2][name];
	}

	public AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition GetTree(string name)
	{
		return (AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)categoryDefinitions[1][name];
	}

	public AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition GetDecoration(string name)
	{
		return (AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)categoryDefinitions[5][name];
	}

	public AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition GetItem(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory category, string name)
	{
		return categoryDefinitions[(int)category][name];
	}

	static ItemDefinitionManager()
	{
		instance = new AvatarFarmOnline.Logic.Stage.ItemDefinitionManager();
	}

	private ItemDefinitionManager()
	{
		LoadItems();
	}

	private void LoadItems()
	{
		definitionsList = new List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition>();
		categoryDefinitionsList = new List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition>[6];
		categoryDefinitions = new Dictionary<string, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition>[6];
		for (int i = 0; i < 6; i++)
		{
			categoryDefinitionsList[i] = new List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition>();
			categoryDefinitions[i] = new Dictionary<string, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition>();
		}
		XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("shop_items");
		XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
		foreach (XElement item in xDocument.Root.Elements())
		{
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition itemDefinition = AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.Parse(item);
			if (itemDefinition != null)
			{
				definitionsList.Add(itemDefinition);
				categoryDefinitions[(int)itemDefinition.Category].Add(itemDefinition.Id, itemDefinition);
				categoryDefinitionsList[(int)itemDefinition.Category].Add(itemDefinition);
			}
		}
	}
}
