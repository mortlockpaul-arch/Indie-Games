using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class PlantHUDItem : RenderItem
{
	private const float DIAMETER = 140f;

	private const float WATER_DIAMETER = 72f;

	private const int BAR_HEIGHT = 44;

	private const int BAR_BORDER = 22;

	private AvatarFarmOnline.Items.Game.HUD.ProgressCircle growthCircle;

	private Sized2DRectangleMesh itemIcon;

	private Sized2DRectangleMesh circleIcon;

	private Sized2DRectangleMesh itemBg;

	private AvatarFarmOnline.Items.Game.HUD.ProgressCircle waterCircle;

	private Sized2DRectangleMesh waterIcon;

	private Sized2DRectangleMesh waterCircleIcon;

	private RenderItem waterItem;

	private TextMesh itemName;

	private BorderedRectangle nameBg;

	private TextMesh timeText;

	private BorderedRectangle timeBg;

	private RenderItem timeItem;

	private Vector2 nameBgPosition;

	private Vector2 timeBgPosition;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private float lastUpdateTime;

	public PlantHUDItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		Layout2D.LayoutData result = new Layout2D.LayoutData(new Vector2((0f - Engine.GUIWidth) * 0.45f + 70f, (0f - Engine.GUIHeight) * 0.45f + 70f), new Vector2(140f));
		growthCircle = new AvatarFarmOnline.Items.Game.HUD.ProgressCircle(result, GameMath.RGBToVector(248, 170, 24));
		growthCircle.SetBGColor(GameMath.RGBToVector(5, 65, 93));
		addMesh(growthCircle);
		Layout2D.InsideBorderLayout(result, 10f, out var result2);
		itemBg = new Sized2DRectangleMesh(result2, TextureManager.Textures["HUD/ButtonBg"]);
		addMesh(itemBg);
		Layout2D.InsideBorderLayout(result, 27f, out result);
		itemIcon = new Sized2DRectangleMesh(result, TextureManager.Textures["HUD/FarmIcon"]);
		itemIcon.Shader = ShaderManager.Shaders["Mask"];
		itemIcon.FirstMaterial.SetTexture(1, TextureManager.Textures["HUD/CircleMask"]);
		addMesh(itemIcon);
		Layout2D.OutsideBorderLayout(result, 17f, out result);
		circleIcon = new Sized2DRectangleMesh(result, TextureManager.Textures["HUD/HUDCircle"]);
		addMesh(circleIcon);
		nameBgPosition = new Vector2(result.position.X + 70f - 25f, (0f - Engine.GUIHeight) * 0.45f + 140f - 4f);
		Layout2D.LayoutData layoutData = new Layout2D.LayoutData(nameBgPosition + new Vector2(60f, 0f), new Vector2(120f, 44f));
		nameBg = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], layoutData, new float[4] { 22f, 22f, 22f, 22f });
		addMesh(nameBg);
		itemName = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont, new TextDrawProperties(layoutData.Left + new Vector2(20f, 5f), HorizontalAlignment.Left), 40, useStringBuilder: false);
		addMesh(itemName);
		timeBgPosition = nameBgPosition + new Vector2(25f, -44f);
		Layout2D.LayoutData layoutData2 = new Layout2D.LayoutData(timeBgPosition + new Vector2(75f, 0f), new Vector2(150f, 44f));
		timeItem = new RenderItem();
		timeBg = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], layoutData2, new float[4] { 22f, 22f, 22f, 22f });
		timeItem.addMesh(timeBg);
		timeText = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont, new TextDrawProperties(layoutData2.Left + new Vector2(20f, 5f), HorizontalAlignment.Left), 30, useStringBuilder: true);
		timeItem.addMesh(timeText);
		addChild(timeItem);
		timeItem.Visible = false;
		waterItem = new RenderItem();
		Layout2D.LayoutData result3 = new Layout2D.LayoutData(new Vector2((0f - Engine.GUIWidth) * 0.45f + 140f + 36f, (0f - Engine.GUIHeight) * 0.45f + 36f), new Vector2(72f));
		waterCircle = new AvatarFarmOnline.Items.Game.HUD.ProgressCircle(result3, GameMath.RGBToVector(10, 143, 203));
		waterCircle.SetBGColor(GameMath.RGBToVector(0, 35, 78));
		waterItem.addMesh(waterCircle);
		Layout2D.InsideBorderLayout(result3, 16f, out result3);
		waterIcon = new Sized2DRectangleMesh(result3, TextureManager.Textures["HUD/WaterIcon"]);
		waterIcon.Shader = ShaderManager.Shaders["Mask"];
		waterIcon.FirstMaterial.SetTexture(1, TextureManager.Textures["HUD/WaterMask"]);
		waterIcon.SetTile(0, 2, 1);
		waterItem.addMesh(waterIcon);
		Layout2D.OutsideBorderLayout(result3, 10f, out result3);
		waterCircleIcon = new Sized2DRectangleMesh(result3, TextureManager.Textures["HUD/HUDCircle"]);
		waterItem.addMesh(waterCircleIcon);
		addChild(waterItem);
		waterItem.Visible = false;
		stage.LocalPlayer.OnChangeHighlightedTile += updateSelectedTile;
		updateSelectedTile(stage.LocalPlayer.HighlightedTile);
	}

	private void updateSelectedTile(AvatarFarmOnline.Logic.Stage.FarmTile obj)
	{
		if (stage.LocalPlayer.CameraState == AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World || stage.IsOnShop)
		{
			Visible = false;
			return;
		}
		Visible = true;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		lastUpdateTime = Timer.DefaultTimer.TotalTimeSeconds;
		string nameString = "";
		if (stage.LocalPlayer.ControlState == ControlState.Planting)
		{
			updatePurchaseItem(stage.LocalPlayer.ShopItem, ref nameString);
			flag2 = true;
		}
		else if (obj == null || obj.IsEmpty)
		{
			nameString = "Empty land";
			itemIcon.Texture = TextureManager.Textures["ShopIcons/Grass"];
		}
		else
		{
			switch (obj.Contents.Type)
			{
			case TileContentsType.Land:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Land land = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Land;
				if (land.State == AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered)
				{
					nameString = "Harvested land";
					itemIcon.Texture = TextureManager.Textures["ShopIcons/Harvested"];
				}
				else
				{
					nameString = "Plowed land";
					itemIcon.Texture = TextureManager.Textures["ShopIcons/Plowed"];
				}
				break;
			}
			case TileContentsType.Building:
			{
				AvatarFarmOnline.Logic.Stage.Contents.BuildingTile buildingTile = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.BuildingTile;
				AvatarFarmOnline.Logic.Stage.Buildings.Building building = buildingTile.Building;
				if (building.Definition.CanBeGathered)
				{
					if (building.GatherState == AvatarFarmOnline.Logic.Stage.Buildings.Building.BuildingGatherState.Waiting)
					{
						growthCircle.SetProgress(building.GrowthProgress);
						timeText.StringBuilder.Length = 0;
						timeText.StringBuilder.Append('\u0bba');
						timeText.StringBuilder.Append(' ');
						AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(building.RemainingTime, timeText.StringBuilder);
						flag2 = true;
						flag3 = true;
					}
					else if (building.GatherState == AvatarFarmOnline.Logic.Stage.Buildings.Building.BuildingGatherState.ReadyToGather)
					{
						timeText.StringBuilder.Length = 0;
						timeText.StringBuilder.Append('\u0bc3');
						timeText.StringBuilder.Append(' ');
						timeText.StringBuilder.AppendNumber(building.Definition.GatherAmount.Amount, AppendNumberOptions.NumberGroup);
						timeText.StringBuilder.Append(building.Definition.GatherAmount.MoneyChar);
						flag2 = true;
					}
				}
				else if (building.Definition.CanAccumulateItems)
				{
					if (building.AccumulationState == AvatarFarmOnline.Logic.Stage.Buildings.Building.BuildingAccumulationState.Accumulating)
					{
						growthCircle.SetProgress((float)building.AccumulatedItems / (float)building.Definition.ItemAccumulationCount);
						timeText.StringBuilder.Length = 0;
						timeText.StringBuilder.Append('\u0bbb');
						timeText.StringBuilder.Append(' ');
						timeText.StringBuilder.AppendNumber(building.AccumulatedItems);
						timeText.StringBuilder.Append('/');
						timeText.StringBuilder.AppendNumber(building.Definition.ItemAccumulationCount);
						flag2 = true;
						flag3 = true;
					}
					else if (building.AccumulationState == AvatarFarmOnline.Logic.Stage.Buildings.Building.BuildingAccumulationState.ReadyToGather)
					{
						timeText.StringBuilder.Length = 0;
						timeText.StringBuilder.Append('\u0bc3');
						timeText.StringBuilder.Append(' ');
						timeText.StringBuilder.AppendNumber(building.Definition.ItemAccumulationAmount.Amount, AppendNumberOptions.NumberGroup);
						timeText.StringBuilder.Append(building.Definition.ItemAccumulationAmount.MoneyChar);
						flag2 = true;
					}
				}
				else if (building.Definition.CanGamble)
				{
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(building.Definition.GamblePrice.Amount, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append(building.Definition.GamblePrice.MoneyChar);
					flag2 = true;
				}
				nameString = buildingTile.Building.Definition.Name.Translate();
				SetItemTexture(building.Definition);
				break;
			}
			case TileContentsType.Tool:
			{
				AvatarFarmOnline.Logic.Stage.Contents.ToolTile toolTile = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.ToolTile;
				AvatarFarmOnline.Logic.Stage.Buildings.Tool tool = toolTile.Tool;
				nameString = toolTile.Tool.Definition.Name.Translate();
				if (tool.FuelProgress > 0f)
				{
					growthCircle.SetProgress(tool.FuelProgress);
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bbc');
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(tool.RemainingFuel, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append("/");
					timeText.StringBuilder.AppendNumber(tool.Definition.MaxFuel, AppendNumberOptions.NumberGroup);
					flag2 = true;
					flag3 = true;
				}
				else
				{
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bbc');
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(tool.Definition.RefillPrice.Amount, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append(tool.Definition.RefillPrice.MoneyChar);
					flag2 = true;
				}
				SetItemTexture(tool.Definition);
				break;
			}
			case TileContentsType.Decorative:
			{
				AvatarFarmOnline.Logic.Stage.Contents.DecorationTile decorationTile = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.DecorationTile;
				nameString = decorationTile.Decoration.Definition.Name.Translate();
				SetItemTexture(decorationTile.Decoration.Definition);
				break;
			}
			case TileContentsType.Tree:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Tree tree = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Tree;
				nameString = tree.Definition.Name.Translate();
				SetItemTexture(tree.Definition);
				flag2 = true;
				timeText.StringBuilder.Length = 0;
				if (tree.State == AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState.ReadyToGather)
				{
					timeText.StringBuilder.Append('\u0bc3');
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(tree.Definition.GatherMoney.Amount, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append(tree.Definition.GatherMoney.MoneyChar);
				}
				else
				{
					timeText.StringBuilder.Append('\u0bba');
					timeText.StringBuilder.Append(' ');
					AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(tree.TimeUntilNextFruit, timeText.StringBuilder);
				}
				break;
			}
			case TileContentsType.Animal:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Animal animal = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Animal;
				nameString = animal.Definition.Name.Translate();
				growthCircle.SetProgress(animal.GrowthProgress);
				if (animal.State == AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.Growing)
				{
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bba');
					timeText.StringBuilder.Append(' ');
					AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(animal.RemainingTime, timeText.StringBuilder);
					flag2 = true;
					flag3 = true;
				}
				else if (animal.State == AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.NeedsFood)
				{
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bc2');
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(animal.Definition.FeedMoney.Amount, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append(animal.Definition.FeedMoney.MoneyChar);
					flag2 = true;
					flag3 = true;
				}
				else
				{
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bc3');
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(animal.Definition.GatherMoney.Amount, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append(animal.Definition.GatherMoney.MoneyChar);
					flag2 = true;
				}
				waterCircle.SetProgress(animal.FoodProgress);
				waterIcon.SetTile(1, 2, 1);
				waterCircle.SetFGColor(GameMath.RGBToVector(204, 0, 21));
				waterCircle.SetBGColor(GameMath.RGBToVector(85, 0, 4));
				flag = true;
				SetItemTexture(animal.Definition);
				break;
			}
			case TileContentsType.Plant:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Plant plant = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Plant;
				nameString = plant.Definition.Name.Translate();
				if (plant.State == AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Growing)
				{
					growthCircle.SetProgress(plant.GrowthProgress);
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bba');
					timeText.StringBuilder.Append(' ');
					AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(plant.RemainingTime, timeText.StringBuilder);
					waterCircle.SetProgress(plant.WaterProgress);
					waterIcon.SetTile(0, 2, 1);
					waterCircle.SetFGColor(GameMath.RGBToVector(10, 143, 203));
					waterCircle.SetBGColor(GameMath.RGBToVector(0, 35, 78));
					flag = true;
					flag2 = true;
					flag3 = true;
				}
				else if (plant.State == AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.ReadyToGather)
				{
					growthCircle.SetProgress(1f);
					flag3 = true;
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bc3');
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(plant.Definition.GatherMoney.Amount, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append(plant.Definition.GatherMoney.MoneyChar);
					flag2 = true;
				}
				else if (plant.State == AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered)
				{
					timeText.StringBuilder.Length = 0;
					timeText.StringBuilder.Append('\u0bc3');
					timeText.StringBuilder.Append(' ');
					timeText.StringBuilder.AppendNumber(plant.Definition.GatherWitheredMoney.Amount, AppendNumberOptions.NumberGroup);
					timeText.StringBuilder.Append(plant.Definition.GatherMoney.MoneyChar);
					flag2 = true;
				}
				SetItemTexture(plant.Definition);
				break;
			}
			}
		}
		int num = AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont.MeasureString(nameString) + 44;
		Layout2D.LayoutData layout = new Layout2D.LayoutData(nameBgPosition + new Vector2((float)num * 0.5f, 0f), new Vector2(num, 44f));
		itemName.Text = nameString;
		nameBg.Layout = layout;
		num = AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont.MeasureString(timeText.StringBuilder) + 44;
		Layout2D.LayoutData layout2 = new Layout2D.LayoutData(timeBgPosition + new Vector2((float)num * 0.5f, 0f), new Vector2(num, 44f));
		timeBg.Layout = layout2;
		waterItem.Visible = flag;
		timeItem.Visible = flag2;
		if (flag3)
		{
			growthCircle.SetBGColor(GameMath.RGBToVector(5, 65, 93));
			growthCircle.Alpha = 1f;
		}
		else
		{
			growthCircle.SetProgress(0f);
			growthCircle.SetBGColor(Vector3.Zero);
			growthCircle.Alpha = 0.5f;
		}
	}

	private void updatePurchaseItem(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, ref string nameString)
	{
		if (item != null)
		{
			nameString = item.Name.Translate();
			SetItemTexture(item);
			timeText.StringBuilder.Length = 0;
			timeText.StringBuilder.Append(item.Price.MoneyChar);
			timeText.StringBuilder.Append(' ');
			timeText.StringBuilder.AppendNumber(item.Price.Amount, AppendNumberOptions.NumberGroup);
		}
	}

	private void SetItemTexture(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
	{
		switch (item.Category)
		{
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant:
			itemIcon.Texture = TextureManager.Textures["ShopIcons/Plants/" + item.Id];
			break;
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
			itemIcon.Texture = TextureManager.Textures["ShopIcons/Buildings/" + item.Id];
			break;
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
			itemIcon.Texture = TextureManager.Textures["ShopIcons/Decorations/" + item.Id];
			break;
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
			itemIcon.Texture = TextureManager.Textures["ShopIcons/Animals/" + item.Id];
			break;
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
			itemIcon.Texture = TextureManager.Textures["ShopIcons/Trees/" + item.Id];
			break;
		case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
			itemIcon.Texture = TextureManager.Textures["ShopIcons/Tools/" + item.Id];
			break;
		}
	}

	protected override void DoUpdate()
	{
		if (Timer.DefaultTimer.TotalTimeSeconds - lastUpdateTime > 0.1f)
		{
			updateSelectedTile(stage.LocalPlayer.HighlightedTile);
		}
		base.DoUpdate();
	}
}
