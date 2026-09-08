using System;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;
using Quasar.Xml;

namespace AvatarFarmOnline.Items.Game;

internal class SelectedTileItem : RenderItem
{
	private class Tile
	{
		public BorderedRectangle mesh;

		public RenderItem item;
	}

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Tile[] tiles;

	private RenderItem previewMesh;

	private AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition lastItem;

	public SelectedTileItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		tiles = new Tile[9];
		for (int i = 0; i < tiles.Length; i++)
		{
			tiles[i] = new Tile();
			tiles[i].mesh = new BorderedRectangle(TextureManager.Textures["SelectedTile"], new Vector2(2f), Vector2.Zero, new float[4] { 0.4f, 0.4f, 0.4f, 0.4f }, new float[4] { 32f, 32f, 32f, 32f });
			tiles[i].mesh.UseXZCoords = true;
			tiles[i].mesh.Ambient = new Vector3(1.2f);
			tiles[i].mesh.Diffuse = Vector3.One;
			tiles[i].mesh.FirstMaterial.RenderPriority = (Material.Priority)51;
			tiles[i].mesh.FirstMaterial.SetForcedAlpha(alpha: true);
			tiles[i].mesh.Shader = ShaderManager.Shaders["BaseNoNormal"];
			tiles[i].item = new RenderItem(tiles[i].mesh);
			addChild(tiles[i].item);
		}
		previewMesh = new RenderItem();
		addChild(previewMesh);
	}

	protected override void DoUpdate()
	{
		AvatarFarmOnline.Logic.Stage.FarmTile highlightedTile = stage.LocalPlayer.HighlightedTile;
		Visible = highlightedTile != null && stage.LocalPlayer.CameraState != AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World;
		previewMesh.Visible = false;
		if (highlightedTile != null)
		{
			int num = 0;
			foreach (AvatarFarmOnline.Logic.Stage.FarmTile workableTile in stage.LocalPlayer.WorkableTiles)
			{
				tiles[num].item.Visible = true;
				BorderedRectangle mesh = tiles[num].mesh;
				mesh.Size = new Vector2(2f);
				mesh.Offset = Vector2.Zero;
				RenderItem item = tiles[num].item;
				item.Transform.Translation = AvatarFarmOnline.Logic.GameGlobals.TileWorldPosition(workableTile.Tile) + new Vector3(1f, 0.01f, 1f);
				switch (stage.LocalPlayer.ControlState)
				{
				case ControlState.Standard:
					if (!workableTile.IsEmpty)
					{
						switch (workableTile.Contents.Type)
						{
						case TileContentsType.Building:
						case TileContentsType.Decorative:
						case TileContentsType.Tool:
						{
							AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding baseBuilding = ((AvatarFarmOnline.Logic.Stage.Contents.BaseBuildingContents)workableTile.Contents).BaseBuilding;
							AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition definition = baseBuilding.Definition;
							int rotation = baseBuilding.Rotation;
							int num4 = ((rotation % 2 == 0) ? definition.Size.X : definition.Size.Y);
							int num5 = ((rotation % 2 == 0) ? definition.Size.Y : definition.Size.X);
							bool flag3 = rotation == 1 || rotation == 2;
							bool flag4 = rotation == 2 || rotation == 3;
							mesh.Size = new Vector2((float)num4 * 2f, (float)num5 * 2f);
							mesh.Offset = new Vector2((float)(num4 - 1) * 0.5f * 2f * (float)((!flag3) ? 1 : (-1)), (float)(num5 - 1) * 0.5f * 2f * (float)((!flag4) ? 1 : (-1)));
							item.Transform.Translation = AvatarFarmOnline.Logic.GameGlobals.TileWorldPosition(baseBuilding.TopLeftTile) + new Vector3(1f, 0.01f, 1f);
							break;
						}
						}
					}
					break;
				case ControlState.Planting:
					switch (stage.LocalPlayer.ShopItem.Category)
					{
					case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
					case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
					case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
					{
						AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition baseBuildingDefinition = stage.LocalPlayer.ShopItem as AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition;
						int orientationRotation = stage.LocalPlayer.OrientationRotation;
						int num2 = ((orientationRotation % 2 == 0) ? baseBuildingDefinition.Size.X : baseBuildingDefinition.Size.Y);
						int num3 = ((orientationRotation % 2 == 0) ? baseBuildingDefinition.Size.Y : baseBuildingDefinition.Size.X);
						bool flag = orientationRotation == 1 || orientationRotation == 2;
						bool flag2 = orientationRotation == 2 || orientationRotation == 3;
						mesh.Size = new Vector2((float)num2 * 2f, (float)num3 * 2f);
						mesh.Offset = new Vector2((float)(num2 - 1) * 0.5f * 2f * (float)((!flag) ? 1 : (-1)), (float)(num3 - 1) * 0.5f * 2f * (float)((!flag2) ? 1 : (-1)));
						if (lastItem != stage.LocalPlayer.ShopItem)
						{
							previewMesh.clearChildren();
							previewMesh.clearMeshes();
							switch (stage.LocalPlayer.ShopItem.Category)
							{
							case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
								ModelLoader.LoadModelDefinition("Buildings/" + stage.LocalPlayer.ShopItem.Id + "Model", previewMesh);
								break;
							case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
								ModelLoader.LoadModelDefinition("Decorations/" + stage.LocalPlayer.ShopItem.Id + "Model", previewMesh);
								break;
							case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
								ModelLoader.LoadModelDefinition("Tools/" + stage.LocalPlayer.ShopItem.Id + "Model", previewMesh);
								break;
							}
							previewMesh.Alpha = 0.4f;
							lastItem = stage.LocalPlayer.ShopItem;
						}
						previewMesh.Transform.Translation = item.Transform.Translation + new Vector3(mesh.Offset.X, 0f, mesh.Offset.Y);
						previewMesh.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI - (float)orientationRotation * ((float)Math.PI / 2f));
						previewMesh.Visible = true;
						break;
					}
					case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
					case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
						if (lastItem != stage.LocalPlayer.ShopItem)
						{
							previewMesh.clearChildren();
							previewMesh.clearMeshes();
							switch (stage.LocalPlayer.ShopItem.Category)
							{
							case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
								ModelLoader.LoadModelDefinition("Animals/" + stage.LocalPlayer.ShopItem.Id + "Model", previewMesh);
								break;
							case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
								ModelLoader.LoadModelDefinition("Trees/" + stage.LocalPlayer.ShopItem.Id + "Model", previewMesh);
								break;
							}
							previewMesh.Alpha = 0.4f;
							lastItem = stage.LocalPlayer.ShopItem;
						}
						previewMesh.Transform.Translation = item.Transform.Translation + new Vector3(mesh.Offset.X, 0f, mesh.Offset.Y);
						previewMesh.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0f);
						previewMesh.Visible = true;
						break;
					}
					item.Transform.Translation = AvatarFarmOnline.Logic.GameGlobals.TileWorldPosition(workableTile.Tile) + new Vector3(1f, 0.01f, 1f);
					break;
				}
				if (num == 0)
				{
					bool flag5 = stage.LocalPlayer.CanPerformContextualAction(out var _);
					mesh.Ambient = (flag5 ? new Vector3(1.2f) : Vector3.UnitX);
					mesh.Diffuse = (flag5 ? Vector3.One : Vector3.UnitX);
					previewMesh.Diffuse = (flag5 ? Vector3.One : Vector3.UnitX);
				}
				num++;
			}
			for (int i = num; i < tiles.Length; i++)
			{
				tiles[i].item.Visible = false;
			}
		}
		base.DoUpdate();
	}
}
