using System;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class ButtonsHUDItem : RenderItem
{
	public const float DIAMETER = 120f;

	public const float BAR_HEIGHT = 44f;

	private const float BAR_BORDER = 22f;

	private const float LOWER_BAR_WIDTH = 190f;

	private const float INSIDE_BORDER = 5f;

	private const float INSIDE_HORIZONTAL_BORDER = 5f;

	private static readonly Vector2 BUTTON_SIZE = new Vector2(110f, 44f);

	private RenderItem shopItem;

	private RenderItem recycleItem;

	private RenderItem cancelItem;

	private Sized2DRectangleMesh toolsMesh;

	private Sized2DRectangleMesh runMesh;

	private Sized2DRectangleMesh contextualIcon;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private float lastUpdateTime;

	public ButtonsHUDItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		Layout2D.LayoutData result = new Layout2D.LayoutData(new Vector2(Engine.GUIWidth * 0.45f - 95f, (0f - Engine.GUIHeight) * 0.45f + 22f), new Vector2(190f, 44f));
		Layout2D.LayoutData layout = new Layout2D.LayoutData(new Vector2(Engine.GUIWidth * 0.45f - 60f, (0f - Engine.GUIHeight) * 0.45f + 60f + 44f), new Vector2(120f));
		BorderedRectangle m = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], result, new float[4] { 22f, 22f, 22f, 22f });
		addMesh(m);
		Layout2D.InsideBorderLayout(result, 5f, out result);
		Layout2D.InsideHorizontalBorderLayout(result, 5f, out result);
		Layout2D.GridLayout(result, 2, 1, 8f, 1, out var cellLayout);
		Layout2D.KeepAspectRatio(cellLayout, 2f, out cellLayout);
		Layout2D.GridLayout(cellLayout, 2, 1, 0f, 1, out var cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh.SetTile(4, 4, 4);
		addMesh(sized2DRectangleMesh);
		Layout2D.GridLayout(cellLayout, 2, 1, 0f, 0, out cellLayout2);
		runMesh = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		runMesh.SetTile(6, 4, 4);
		addMesh(runMesh);
		Layout2D.GridLayout(result, 2, 1, 8f, 0, out cellLayout);
		Layout2D.KeepAspectRatio(cellLayout, 2f, out cellLayout);
		Layout2D.GridLayout(cellLayout, 2, 1, 0f, 1, out cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh2 = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh2.SetTile(3, 4, 4);
		addMesh(sized2DRectangleMesh2);
		Layout2D.GridLayout(cellLayout, 2, 1, 0f, 0, out cellLayout2);
		toolsMesh = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		toolsMesh.SetTile(10, 4, 4);
		addMesh(toolsMesh);
		Sized2DRectangleMesh m2 = new Sized2DRectangleMesh(layout, TextureManager.Textures["HUD/ButtonBg"]);
		addMesh(m2);
		Sized2DRectangleMesh m3 = new Sized2DRectangleMesh(layout, TextureManager.Textures["HUD/HUDCircle"]);
		addMesh(m3);
		contextualIcon = new Sized2DRectangleMesh(layout, TextureManager.Textures["HUD/HUDBigIcons"]);
		contextualIcon.SetTile(0, 4, 4);
		addMesh(contextualIcon);
		Layout2D.LayoutData layout2 = new Layout2D.LayoutData(layout.position + new Vector2((float)Math.Sin(0.7853981852531433) * 120f * 0.5f, (0f - (float)Math.Sin(0.7853981852531433)) * 120f * 0.5f), new Vector2(48f));
		Sized2DRectangleMesh sized2DRectangleMesh3 = new Sized2DRectangleMesh(layout2, TextureManager.Textures["HUD/HUDBigIcons"]);
		sized2DRectangleMesh3.SetTile(11, 4, 4);
		addMesh(sized2DRectangleMesh3);
		Layout2D.LayoutData result2 = new Layout2D.LayoutData(layout.position + new Vector2(-40f - BUTTON_SIZE.X * 0.5f, 52f), BUTTON_SIZE);
		shopItem = new RenderItem();
		BorderedRectangle m4 = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], result2, new float[4] { 22f, 22f, 22f, 22f });
		shopItem.addMesh(m4);
		Layout2D.InsideBorderLayout(result2, 5f, out result2);
		Layout2D.InsideHorizontalBorderLayout(result2, 5f, out result2);
		Layout2D.KeepAspectRatio(result2, 2f, out result2);
		Layout2D.GridLayout(result2, 2, 1, 0f, 1, out cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh4 = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh4.SetTile(1, 4, 4);
		shopItem.addMesh(sized2DRectangleMesh4);
		Layout2D.GridLayout(result2, 2, 1, 0f, 0, out cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh5 = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh5.SetTile(5, 4, 4);
		shopItem.addMesh(sized2DRectangleMesh5);
		addChild(shopItem);
		Layout2D.LayoutData result3 = new Layout2D.LayoutData(layout.position + new Vector2(-64f - BUTTON_SIZE.X * 0.5f, 4f), BUTTON_SIZE);
		recycleItem = new RenderItem();
		BorderedRectangle m5 = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], result3, new float[4] { 22f, 22f, 22f, 22f });
		recycleItem.addMesh(m5);
		Layout2D.InsideBorderLayout(result3, 5f, out result3);
		Layout2D.InsideHorizontalBorderLayout(result3, 5f, out result3);
		Layout2D.KeepAspectRatio(result3, 2f, out result3);
		Layout2D.GridLayout(result3, 2, 1, 0f, 1, out cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh6 = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh6.SetTile(0, 4, 4);
		recycleItem.addMesh(sized2DRectangleMesh6);
		Layout2D.GridLayout(result3, 2, 1, 0f, 0, out cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh7 = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh7.SetTile(8, 4, 4);
		recycleItem.addMesh(sized2DRectangleMesh7);
		addChild(recycleItem);
		Layout2D.LayoutData result4 = new Layout2D.LayoutData(layout.position + new Vector2(-50f - BUTTON_SIZE.X * 0.5f, -60f + BUTTON_SIZE.Y * 0.5f), BUTTON_SIZE);
		cancelItem = new RenderItem();
		BorderedRectangle m6 = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], result4, new float[4] { 22f, 22f, 22f, 22f });
		cancelItem.addMesh(m6);
		Layout2D.InsideBorderLayout(result4, 5f, out result4);
		Layout2D.InsideHorizontalBorderLayout(result4, 5f, out result4);
		Layout2D.KeepAspectRatio(result4, 2f, out result4);
		Layout2D.GridLayout(result4, 2, 1, 0f, 1, out cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh8 = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh8.SetTile(2, 4, 4);
		cancelItem.addMesh(sized2DRectangleMesh8);
		Layout2D.GridLayout(result4, 2, 1, 0f, 0, out cellLayout2);
		Sized2DRectangleMesh sized2DRectangleMesh9 = new Sized2DRectangleMesh(cellLayout2, TextureManager.Textures["HUD/ButtonIcons"]);
		sized2DRectangleMesh9.SetTile(7, 4, 4);
		cancelItem.addMesh(sized2DRectangleMesh9);
		addChild(cancelItem);
		stage.LocalPlayer.OnChangeHighlightedTile += updateSelectedTile;
		stage.LocalPlayer.OnUseToolsChanged += Player_OnUseToolsChanged;
	}

	private void Player_OnUseToolsChanged(AvatarFarmOnline.Logic.Stage.Player obj)
	{
		GameTemplate.MoveAudio.Start();
		toolsMesh.SetTile(obj.UsingTools ? 9 : 10, 4, 4);
	}

	private void updateSelectedTile(AvatarFarmOnline.Logic.Stage.FarmTile obj)
	{
		if (stage.LocalPlayer.CameraState == AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World || stage.IsOnShop)
		{
			Visible = false;
			shopItem.Visible = false;
			cancelItem.Visible = false;
			recycleItem.Visible = false;
			return;
		}
		Visible = true;
		shopItem.Visible = true;
		lastUpdateTime = Timer.DefaultTimer.TotalTimeSeconds;
		if (obj == null || obj.IsEmpty)
		{
			recycleItem.Visible = false;
			contextualIcon.SetTile(9, 4, 4);
			if (stage.LocalPlayer.ControlState != ControlState.Planting)
			{
				contextualIcon.SetTile(1, 4, 4);
			}
			else
			{
				switch (stage.LocalPlayer.ShopItem.Category)
				{
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
					contextualIcon.SetTile(3, 4, 4);
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
					contextualIcon.SetTile(10, 4, 4);
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
					contextualIcon.SetTile(8, 4, 4);
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
					contextualIcon.SetTile(0, 4, 4);
					break;
				}
			}
		}
		else
		{
			recycleItem.Visible = true;
			if (stage.LocalPlayer.ControlState == ControlState.Planting)
			{
				contextualIcon.SetTile(9, 4, 4);
				if (obj.Contents.Type == TileContentsType.Land)
				{
					AvatarFarmOnline.Logic.Stage.Contents.Land land = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Land;
					if (land.State == AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Plowed && stage.LocalPlayer.ShopItem != null && stage.LocalPlayer.ShopItem.Category == AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant)
					{
						contextualIcon.SetTile(0, 4, 4);
					}
				}
			}
			else
			{
				AvatarFarmOnline.Logic.FailedActionReason failReason;
				switch (obj.Contents.Type)
				{
				case TileContentsType.Land:
				{
					AvatarFarmOnline.Logic.Stage.Contents.Land land2 = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Land;
					if (land2.State == AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered)
					{
						contextualIcon.SetTile(1, 4, 4);
					}
					else
					{
						contextualIcon.SetTile(7, 4, 4);
					}
					break;
				}
				case TileContentsType.Building:
				{
					AvatarFarmOnline.Logic.Stage.Contents.BuildingTile buildingTile = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.BuildingTile;
					if (buildingTile.Building.CanBeGathered(out failReason))
					{
						contextualIcon.SetTile(6, 4, 4);
					}
					else
					{
						contextualIcon.SetTile(9, 4, 4);
					}
					break;
				}
				case TileContentsType.Tool:
				{
					AvatarFarmOnline.Logic.Stage.Contents.ToolTile toolTile = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.ToolTile;
					if (toolTile.Tool.CanBeRefilled(out failReason))
					{
						contextualIcon.SetTile(5, 4, 4);
					}
					else
					{
						contextualIcon.SetTile(9, 4, 4);
					}
					break;
				}
				case TileContentsType.Decorative:
					_ = obj.Contents;
					contextualIcon.SetTile(9, 4, 4);
					break;
				case TileContentsType.Tree:
				{
					AvatarFarmOnline.Logic.Stage.Contents.Tree tree = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Tree;
					if (tree.State == AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState.ReadyToGather)
					{
						contextualIcon.SetTile(6, 4, 4);
					}
					else
					{
						contextualIcon.SetTile(9, 4, 4);
					}
					break;
				}
				case TileContentsType.Animal:
				{
					AvatarFarmOnline.Logic.Stage.Contents.Animal animal = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Animal;
					if (animal.State == AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.ReadyToGather)
					{
						contextualIcon.SetTile(6, 4, 4);
					}
					else if (animal.State == AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.NeedsFood)
					{
						contextualIcon.SetTile(4, 4, 4);
					}
					else
					{
						contextualIcon.SetTile(9, 4, 4);
					}
					break;
				}
				case TileContentsType.Plant:
				{
					AvatarFarmOnline.Logic.Stage.Contents.Plant plant = obj.Contents as AvatarFarmOnline.Logic.Stage.Contents.Plant;
					if (plant.State == AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.ReadyToGather)
					{
						contextualIcon.SetTile(6, 4, 4);
					}
					else
					{
						contextualIcon.SetTile(2, 4, 4);
					}
					break;
				}
				}
			}
		}
		if (stage.LocalPlayer.ControlState == ControlState.Planting)
		{
			recycleItem.Visible = false;
		}
		cancelItem.Visible = stage.LocalPlayer.ControlState == ControlState.Planting;
	}

	protected override void DoUpdate()
	{
		if (Timer.DefaultTimer.TotalTimeSeconds - lastUpdateTime > 0.1f)
		{
			updateSelectedTile(stage.LocalPlayer.HighlightedTile);
		}
		runMesh.SetTile(stage.LocalPlayer.IsRunning ? 11 : 6, 4, 4);
		base.DoUpdate();
	}
}
