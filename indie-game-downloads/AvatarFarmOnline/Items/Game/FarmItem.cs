using System.Collections.Generic;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Contents;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game;

internal class FarmItem : RenderItem
{
	private const int TILE_MARGIN = 3;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private TileMesh tileMesh;

	private List<AvatarFarmOnline.Items.Game.BuildingItem> buildingItems;

	private AvatarFarmOnline.Items.Game.FarmTileItem[,] farmTiles;

	private RenderItem buildingsItem;

	private RenderItem tilesItem;

	public FarmItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		DebugColor = Color.Yellow;
		this.stage = stage;
		tilesItem = new RenderItem();
		addChild(tilesItem);
		tileMesh = new TileMesh(stage.FarmData.FarmSize, new Vector2(2f), new Int2(2), TextureManager.Textures["GroundTiles"]);
		tileMesh.FirstMaterial.Shininess = 10f;
		tileMesh.FirstMaterial.Ambient = new Vector3(1f);
		tileMesh.FirstMaterial.Diffuse = new Vector3(1f);
		tileMesh.FirstMaterial.Specular = new Vector3(0f);
		addMesh(tileMesh);
		farmTiles = new AvatarFarmOnline.Items.Game.FarmTileItem[AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.X + 6, AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.Y + 6];
		for (int i = 0; i < AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.X + 6; i++)
		{
			for (int j = 0; j < AvatarFarmOnline.Logic.GameGlobals.MaxFarmSize.Y + 6; j++)
			{
				if (GameMath.Between(3, stage.FarmData.FarmSize.X - 1 + 3, i) && GameMath.Between(3, stage.FarmData.FarmSize.Y - 1 + 3, j))
				{
					farmTiles[i, j] = new AvatarFarmOnline.Items.Game.FarmTileItem(stage, stage.FarmData.Tiles[i - 3, j - 3]);
				}
				else
				{
					farmTiles[i, j] = new AvatarFarmOnline.Items.Game.FarmTileItem(stage, new Int2(i - 3, j - 3));
				}
			}
		}
		buildingItems = new List<AvatarFarmOnline.Items.Game.BuildingItem>(64);
		buildingsItem = new RenderItem();
		addChild(buildingsItem);
		addChild(new AvatarFarmOnline.Items.Game.SelectedTileItem(stage));
		stage.FarmData.OnBuildingPlaced += OnBuildingPlaced;
		stage.FarmData.OnBuildingRemoved += OnBuildingRemoved;
		stage.FarmData.OnDecorationPlaced += OnDecorationPlaced;
		stage.FarmData.OnDecorationRemoved += OnDecorationRemoved;
		stage.FarmData.OnToolPlaced += OnToolPlaced;
		stage.FarmData.OnToolRemoved += OnToolRemoved;
		stage.FarmData.OnTileChangedContents += OnTileChangedContents;
		stage.FarmData.OnIncreaseSize += FarmData_OnIncreaseSize;
		stage.LocalPlayer.OnCameraStateChanged += OnCameraStateChanged;
		UpdateTileData();
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in stage.FarmData.Buildings)
		{
			OnBuildingPlaced(null, building);
		}
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration in stage.FarmData.Decorations)
		{
			OnDecorationPlaced(null, decoration);
		}
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in stage.FarmData.Tools)
		{
			OnToolPlaced(null, tool);
		}
		stage.LocalPlayer.OnChangeHighlightedTile += Stage_OnChangeHighlightedTile;
		Stage_OnChangeHighlightedTile(stage.LocalPlayer.HighlightedTile);
	}

	private void FarmData_OnIncreaseSize(Int2 oldSize, Int2 newSize)
	{
		for (int i = 3 + oldSize.X; i < newSize.X + 3; i++)
		{
			for (int j = 3; j < oldSize.Y + 3; j++)
			{
				farmTiles[i, j].SetTile(stage.FarmData.Tiles[i - 3, j - 3]);
			}
		}
		for (int k = 3; k < newSize.X + 3; k++)
		{
			for (int l = 3 + oldSize.Y; l < newSize.Y + 3; l++)
			{
				farmTiles[k, l].SetTile(stage.FarmData.Tiles[k - 3, l - 3]);
			}
		}
		for (int m = 2; m < newSize.X + 3 + 1; m++)
		{
			int num = 2;
			farmTiles[m, num].SetTile(null);
			num = newSize.Y + 3;
			farmTiles[m, num].SetTile(null);
		}
		for (int n = 3; n < newSize.Y + 3; n++)
		{
			int num2 = 2;
			farmTiles[num2, n].SetTile(null);
			num2 = newSize.X + 3;
			farmTiles[num2, n].SetTile(null);
		}
		tileMesh.Resize(newSize);
		UpdateTileData();
	}

	private void OnCameraStateChanged(AvatarFarmOnline.Logic.Stage.Player obj)
	{
		Stage_OnChangeHighlightedTile(stage.LocalPlayer.HighlightedTile);
	}

	private void Stage_OnChangeHighlightedTile(AvatarFarmOnline.Logic.Stage.FarmTile obj)
	{
		tilesItem.clearChildren();
		Vector3 value = stage.LocalPlayer.WorldPosition;
		int length = farmTiles.GetLength(0);
		int length2 = farmTiles.GetLength(1);
		bool flag = stage.LocalPlayer.CameraState == AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World;
		float result;
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				AvatarFarmOnline.Items.Game.FarmTileItem farmTileItem = farmTiles[i, j];
				Vector3 value2 = farmTileItem.TilePosition;
				Vector3.Distance(ref value, ref value2, out result);
				if (flag || result <= farmTileItem.ViewDistance * 2f)
				{
					tilesItem.addChild(farmTileItem);
				}
			}
		}
		buildingsItem.clearChildren();
		foreach (AvatarFarmOnline.Items.Game.BuildingItem buildingItem in buildingItems)
		{
			Vector3 value3 = buildingItem.CenterPosition;
			Vector3.Distance(ref value, ref value3, out result);
			if (flag || result <= 30f)
			{
				buildingsItem.addChild(buildingItem);
			}
		}
	}

	private void OnBuildingPlaced(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Building b)
	{
		AvatarFarmOnline.Items.Game.BuildingItem buildingItem = new AvatarFarmOnline.Items.Game.BuildingItem(stage, b);
		buildingItems.Add(buildingItem);
		buildingsItem.addChild(buildingItem);
	}

	private void OnBuildingRemoved(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Building b)
	{
		for (int i = 0; i < buildingItems.Count; i++)
		{
			AvatarFarmOnline.Items.Game.BuildingItem buildingItem = buildingItems[i];
			if (buildingItem.Building == b)
			{
				buildingItems.RemoveAt(i);
				buildingsItem.removeChild(buildingItem);
				break;
			}
		}
	}

	private void OnDecorationPlaced(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Decoration d)
	{
		AvatarFarmOnline.Items.Game.BuildingItem buildingItem = new AvatarFarmOnline.Items.Game.BuildingItem(stage, d);
		buildingsItem.addChild(buildingItem);
		buildingItems.Add(buildingItem);
	}

	private void OnToolPlaced(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Tool t)
	{
		AvatarFarmOnline.Items.Game.BuildingItem buildingItem = new AvatarFarmOnline.Items.Game.BuildingItem(stage, t);
		buildingsItem.addChild(buildingItem);
		buildingItems.Add(buildingItem);
	}

	private void OnToolRemoved(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Tool obj)
	{
		for (int i = 0; i < buildingItems.Count; i++)
		{
			AvatarFarmOnline.Items.Game.BuildingItem buildingItem = buildingItems[i];
			if (buildingItem.BaseBuilding == obj)
			{
				buildingItems.RemoveAt(i);
				buildingsItem.removeChild(buildingItem);
				break;
			}
		}
	}

	private void UpdateTileData()
	{
		for (int i = 0; i < stage.FarmData.FarmSize.X; i++)
		{
			for (int j = 0; j < stage.FarmData.FarmSize.Y; j++)
			{
				AvatarFarmOnline.Logic.Stage.FarmTile farmTile = stage.FarmData.Tiles[i, j];
				short num;
				if (farmTile.IsEmpty)
				{
					num = 0;
				}
				else
				{
					switch (farmTile.Contents.Type)
					{
					case TileContentsType.Tree:
					case TileContentsType.Building:
					case TileContentsType.Decorative:
					case TileContentsType.Animal:
						num = 3;
						break;
					default:
						num = 0;
						break;
					case TileContentsType.Plant:
						num = 2;
						break;
					case TileContentsType.Land:
						num = ((AvatarFarmOnline.Logic.Stage.Contents.Land)farmTile.Contents).State switch
						{
							AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered => 1, 
							_ => 2, 
						};
						break;
					}
				}
				tileMesh.TileIndices[j * stage.FarmData.FarmSize.X + i] = num;
			}
		}
		tileMesh.UpdateTileData();
	}

	private void OnTileChangedContents(AvatarFarmOnline.Logic.Stage.FarmTile ft, AvatarFarmOnline.Logic.Stage.TileContents tc)
	{
		int num;
		if (ft.IsEmpty)
		{
			num = 0;
		}
		else
		{
			switch (ft.Contents.Type)
			{
			case TileContentsType.Tree:
			case TileContentsType.Building:
			case TileContentsType.Decorative:
			case TileContentsType.Animal:
				num = 3;
				break;
			default:
				num = 0;
				break;
			case TileContentsType.Plant:
				num = 2;
				break;
			case TileContentsType.Land:
				num = ((AvatarFarmOnline.Logic.Stage.Contents.Land)ft.Contents).State switch
				{
					AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered => 1, 
					_ => 2, 
				};
				break;
			}
		}
		tileMesh.TileIndices[ft.Tile.Y * stage.FarmData.FarmSize.X + ft.Tile.X] = num;
		tileMesh.UpdateTileData();
		Stage_OnChangeHighlightedTile(null);
	}

	private void OnDecorationRemoved(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Decoration d)
	{
		for (int i = 0; i < buildingItems.Count; i++)
		{
			AvatarFarmOnline.Items.Game.BuildingItem buildingItem = buildingItems[i];
			if (buildingItem.BaseBuilding == d)
			{
				buildingItems.RemoveAt(i);
				buildingsItem.removeChild(buildingItem);
				break;
			}
		}
	}
}
