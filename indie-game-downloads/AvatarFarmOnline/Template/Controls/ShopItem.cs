using System;
using System.Collections.Generic;
using AvatarFarmOnline.Items.Game;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar;
using Quasar.Elements;
using Quasar.Elements.Cameras;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.XBLIG.CrossPromotion;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Render;
using Quasar.Render.Passes;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Template.Controls;

internal class ShopItem : RenderItem
{
	private class ItemData : RenderItem
	{
		private Sized2DRectangleMesh icon;

		private Sized2DRectangleMesh cantIcon;

		private BorderedRectangle bgMesh;

		private BorderedRectangle selectedMesh;

		private TextBoxMesh growth;

		private TextBoxMesh price;

		private int index;

		private RenderItem growthItem;

		private AvatarFarmOnline.Template.Controls.Shop shop;

		private Vector3 selectedOffset;

		public ItemData(AvatarFarmOnline.Template.Controls.Shop shop, Layout2D.LayoutData layout, int index)
		{
			this.shop = shop;
			this.index = index;
			selectedOffset = new Vector3(0f, layout.Height * 0.05f, 0f);
			float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
			selectedMesh = new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], new Vector2(layout.Width * 1.05f, layout.Height * 1.05f), coordBorder);
			selectedMesh.Offset = layout.position;
			selectedMesh.Diffuse = new Vector3(0f, 1f, 1f);
			selectedMesh.Alpha = 0f;
			addMesh(selectedMesh);
			bgMesh = new BorderedRectangle(TextureManager.Textures["GUI/ShopItemBg"], layout, coordBorder);
			bgMesh.FirstMaterial.Textures.Add(TextureManager.Textures["GUI/WoodTileTex"]);
			bgMesh.Shader = ShaderManager.Shaders["GUIMask"];
			addMesh(bgMesh);
			Layout2D.InsideBorderLayout(layout, 4f, out layout);
			Layout2D.VerticalFixedFloatLayout(layout, layout.Width, 0f, out var fixedLayout, out var floatLayout);
			icon = new Sized2DRectangleMesh(fixedLayout, null);
			addMesh(icon);
			cantIcon = new Sized2DRectangleMesh(fixedLayout, TextureManager.Textures["HUD/HUDBigIcons"]);
			cantIcon.Alpha = 0f;
			cantIcon.SetTile(9, 4, 4);
			addMesh(cantIcon);
			Layout2D.VerticalFixedFloatLayout(layout, layout.Height * 0.6f, 0f, out var fixedLayout2, out floatLayout);
			Layout2D.GridLayout(floatLayout, 1, 2, 0f, 0, out fixedLayout2);
			Layout2D.GridLayout(floatLayout, 1, 2, 0f, 1, out floatLayout);
			growth = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(fixedLayout2, 0.6f / GameMath.Interpolate(Math.Min(1f, Engine.GUIScale), 1f, 0.25f), HorizontalAlignment.Center, VerticalAlignment.Center), 24, useStringBuilder: true);
			BorderedRectangle m = new BorderedRectangle(TextureManager.Textures["GUI/HorizontalGradientMask"], fixedLayout2, coordBorder)
			{
				Diffuse = Vector3.Zero,
				Alpha = 0.25f
			};
			growthItem = new RenderItem();
			growthItem.addMesh(m);
			growthItem.addMesh(growth);
			addChild(growthItem);
			price = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(floatLayout, 0.6f / GameMath.Interpolate(Math.Min(1f, Engine.GUIScale), 1f, 0.25f), HorizontalAlignment.Center, VerticalAlignment.Center), 24, useStringBuilder: true);
			addMesh(price);
		}

		public void UpdateData()
		{
			if (shop.Items.Count - shop.BaseIndex > index)
			{
				Visible = true;
				AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition itemDefinition = shop.Items[shop.BaseIndex + index];
				bool flag = shop.CanBuy(itemDefinition, out var _);
				growthItem.Visible = false;
				switch (itemDefinition.Category)
				{
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant:
					growthItem.Visible = true;
					icon.Texture = TextureManager.Textures["ShopIcons/Plants/" + itemDefinition.Id];
					growth.StringBuilder.Length = 0;
					growth.StringBuilder.Append('\u0bba');
					growth.StringBuilder.Append(' ');
					AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(((AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)itemDefinition).GrowTime, growth.StringBuilder);
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
					icon.Texture = TextureManager.Textures["ShopIcons/Buildings/" + itemDefinition.Id];
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
					icon.Texture = TextureManager.Textures["ShopIcons/Decorations/" + itemDefinition.Id];
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
					growthItem.Visible = true;
					icon.Texture = TextureManager.Textures["ShopIcons/Animals/" + itemDefinition.Id];
					growth.StringBuilder.Length = 0;
					growth.StringBuilder.Append('\u0bba');
					growth.StringBuilder.Append(' ');
					AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(((AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)itemDefinition).FeedInterval, growth.StringBuilder);
					growth.StringBuilder.Append(" x");
					growth.StringBuilder.AppendNumber(((AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)itemDefinition).FeedAmount);
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
					icon.Texture = TextureManager.Textures["ShopIcons/Trees/" + itemDefinition.Id];
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
					icon.Texture = TextureManager.Textures["ShopIcons/Tools/" + itemDefinition.Id];
					break;
				}
				selectedMesh.Alpha = ((shop.CurrentIndex % 6 == index) ? 0.8f : 0f);
				cantIcon.Alpha = ((!flag) ? 1 : 0);
				price.StringBuilder.Length = 0;
				price.StringBuilder.Append(itemDefinition.Price.MoneyChar);
				price.StringBuilder.Append(' ');
				price.StringBuilder.AppendNumber(itemDefinition.Price.Amount, AppendNumberOptions.NumberGroup);
			}
			else
			{
				Visible = false;
			}
		}

		protected override void DoUpdate()
		{
			Transform.Translation = Vector3.Lerp(Transform.Translation, (selectedMesh.Alpha > 0f) ? selectedOffset : Vector3.Zero, 0.1f);
			base.DoUpdate();
		}
	}

	private class ItemDetails : RenderItem
	{
		private class SphereCamera : LookAtCamera
		{
			private float angle;

			private float height = 2f;

			private float targetHeight = 2f;

			private float distance = 4.7f;

			public SphereCamera(Vector2 size)
				: base(new Vector3(4.7f, 2f, 4.7f), Vector3.Zero)
			{
				SetViewportSize(size.X / size.Y);
			}

			public void SetNewItem(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition definition)
			{
				switch (definition.Category)
				{
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
					targetHeight = 0.8f;
					height = 1.8f;
					distance = 4.5f;
					break;
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
				{
					float num = 1f + (float)(((AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition)definition).Size.MaxAbsCoord - 1) * 0.75f;
					height = 2f * num;
					targetHeight = 1f * num;
					distance = 3.7f * num;
					break;
				}
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
				{
					float num = 1f + (float)(((AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition)definition).Size.MaxAbsCoord - 1) * 0.75f;
					height = 2f * num;
					targetHeight = 1f * num;
					distance = 3.7f * num;
					break;
				}
				case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
					height = 3f;
					targetHeight = 1.25f;
					distance = 3.8f;
					break;
				default:
					height = 1.5f;
					targetHeight = 0.5f;
					distance = 3f;
					break;
				}
			}

			protected override void DoUpdate()
			{
				angle += Timer.DefaultTimer.LastIntervalSeconds * 0.25f;
				Vector2 vector = GameMath.VectorFromAngle(angle, distance);
				Transform.Translation = new Vector3(vector.X, height, vector.Y);
				Target = new Vector3(0f, targetHeight, 0f);
				base.DoUpdate();
			}
		}

		private bool dirty = true;

		private AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition currentItem;

		private AvatarFarmOnline.Template.Controls.Shop shop;

		private Layout2D.LayoutData contentLayout;

		private Sized2DRectangleMesh previewMesh;

		private List<InfoPanel> infoPanels = new List<InfoPanel>();

		private Scene previewScene;

		private RenderItem previewItem;

		private RenderProcess previewProcess;

		private SphereCamera previewCamera;

		private Quasar.Meshes.TileMesh floorMesh;

		private TextBoxMesh nameMesh;

		private TextBoxMesh levelMesh;

		private BorderedRectangle levelBgMesh;

		private RenderItem levelItem;

		private TextBoxMesh buildingNeededMesh;

		private BorderedRectangle buildingNeededBgMesh;

		private RenderItem buildingNeededItem;

		public AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition CurrentItem
		{
			set
			{
				currentItem = value;
				dirty = true;
			}
		}

		public ItemDetails(AvatarFarmOnline.Template.Controls.Shop shop, Layout2D.LayoutData layout)
		{
			this.shop = shop;
			float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
			addMesh(new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], layout, coordBorder)
			{
				Diffuse = Vector3.Zero,
				Alpha = 0.5f
			});
			Layout2D.InsideBorderLayout(layout, 8f, out layout);
			addMesh(new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], layout, coordBorder)
			{
				Diffuse = Vector3.Zero,
				Alpha = 0.5f
			});
			Layout2D.VerticalFixedFloatLayout(layout, layout.Width, 0f, out var fixedLayout, out var floatLayout);
			Layout2D.InsideBorderLayout(floatLayout, 4f, out contentLayout);
			lock (Engine.Instance.RenderLock)
			{
				RenderTarget2D renderTarget = AntialiasRenderPass.CreateRenderTarget(new Vector2(256f), 4);
				previewScene = new Scene();
				previewCamera = new SphereCamera(fixedLayout.size);
				previewScene.Camera = previewCamera;
				Light light = new Light();
				light.Diffuse = Vector3.Lerp(Vector3.One, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(AvatarFarmOnline.Logic.Seasons.Summer), 0.6f) * 0.85f;
				light.Ambient = Vector3.Lerp(Vector3.One, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(AvatarFarmOnline.Logic.Seasons.Summer), 0.9f) * 0.6f;
				light.Transform.Translation = new Vector3(40f, 70f, 40f);
				light.UpdateRotationFromTranslation();
				previewScene.addLight(light);
				previewScene.Add(previewCamera);
				floorMesh = new Quasar.Meshes.TileMesh(new Int2(5), new Vector2(2f), new Int2(2), TextureManager.Textures["GroundTiles"]);
				floorMesh.Shader = ShaderManager.Shaders["FarmTiles"];
				floorMesh.FirstMaterial.SetForcedAlpha(alpha: false);
				floorMesh.UseXZCoords = true;
				floorMesh.FirstMaterial.Ambient = new Vector3(1f);
				floorMesh.FirstMaterial.Diffuse = new Vector3(1f);
				floorMesh.Offset = new Vector2(-5f, 5f);
				RenderItem renderItem = new RenderItem(floorMesh);
				previewScene.Add(renderItem);
				float[] coordBorder2 = new float[4] { 0.5f, 0.5f, 0.5f, 0.5f };
				BorderedRectangle borderedRectangle = new BorderedRectangle(TextureManager.Textures["SquaredShadowInverted"], new Vector2(10f), coordBorder2);
				borderedRectangle.Shader = ShaderManager.Shaders["AlphaMask"];
				borderedRectangle.Diffuse = new Vector3(0f, 0.549f, 0.863f);
				borderedRectangle.UseXZCoords = true;
				borderedRectangle.FirstMaterial.SetForcedAlpha(alpha: false);
				renderItem.addMesh(borderedRectangle);
				previewItem = new RenderItem();
				previewScene.Add(previewItem);
				previewProcess = new RenderProcess(renderTarget, previewScene);
				previewProcess.FinalRenderPass.MustClearColor = true;
				previewProcess.FinalRenderPass.BackgroundColor = new Color(0, 0, 0, 0);
				previewProcess.FinalRenderPass.MustClearDepth = true;
				BaseGame.Instance.CurrentGameSection.AddScene(previewScene, isDefault: false);
				BaseGame.Instance.CurrentGameSection.AddExtraRenderProcess(previewProcess);
			}
			Layout2D.InsideBorderLayout(fixedLayout, 10f, out var result);
			Sized2DRectangleMesh m = new Sized2DRectangleMesh(result, TextureManager.Textures["GUI/PreviewBG"]);
			addMesh(m);
			previewMesh = new Sized2DRectangleMesh(result, previewProcess.RenderTarget);
			addMesh(previewMesh);
			Layout2D.VerticalFixedFloatLayout(result, 30f, 0f, out var fixedLayout2, out var floatLayout2);
			BorderedRectangle borderedRectangle2 = new BorderedRectangle(TextureManager.Textures["GUI/HorizontalGradientMask"], fixedLayout2, coordBorder);
			borderedRectangle2.Offset -= new Vector2(0f, 5f);
			borderedRectangle2.Diffuse = Vector3.Zero;
			borderedRectangle2.Alpha = 0.25f;
			addMesh(borderedRectangle2);
			nameMesh = new TextBoxMesh(BitmapFontManager.Fonts["Standard"], new TextBoxDrawProperties(fixedLayout2, 0.8f / GameMath.Interpolate(Math.Min(1f, Engine.GUIScale), 1f, 0.25f), HorizontalAlignment.Center, VerticalAlignment.Bottom), 60, useStringBuilder: true);
			addMesh(nameMesh);
			Layout2D.VerticalFixedFloatLayout(result, result.Height - 30f, 0f, out floatLayout2, out var floatLayout3);
			levelBgMesh = new BorderedRectangle(TextureManager.Textures["GUI/HorizontalGradientMask"], floatLayout3, coordBorder);
			levelBgMesh.Diffuse = Vector3.Zero;
			levelBgMesh.Alpha = 0.5f;
			levelMesh = new TextBoxMesh(BitmapFontManager.Fonts["Standard"], new TextBoxDrawProperties(floatLayout3, 0.7f, HorizontalAlignment.Center, VerticalAlignment.Center), 30, useStringBuilder: true);
			levelMesh.Diffuse = Vector3.UnitX;
			levelItem = new RenderItem(levelBgMesh);
			levelItem.addMesh(levelMesh);
			addChild(levelItem);
			buildingNeededBgMesh = new BorderedRectangle(TextureManager.Textures["GUI/HorizontalGradientMask"], floatLayout3, coordBorder);
			buildingNeededBgMesh.Diffuse = Vector3.Zero;
			buildingNeededBgMesh.Alpha = 0.5f;
			buildingNeededMesh = new TextBoxMesh(BitmapFontManager.Fonts["Standard"], new TextBoxDrawProperties(floatLayout3, 0.7f, HorizontalAlignment.Center, VerticalAlignment.Center), 30, useStringBuilder: true);
			buildingNeededMesh.Diffuse = Vector3.UnitX;
			buildingNeededItem = new RenderItem(buildingNeededBgMesh);
			buildingNeededItem.addMesh(buildingNeededMesh);
			buildingNeededItem.Transform.Translation = new Vector3(0f, floatLayout3.Height, 0f);
			addChild(buildingNeededItem);
			addMesh(new BorderedRectangle(TextureManager.Textures["GUI/ExtBorderMask"], fixedLayout, coordBorder)
			{
				FirstMaterial = 
				{
					Textures = { (Texture)TextureManager.Textures["GUI/WoodTileTex"] }
				},
				Shader = ShaderManager.Shaders["GUIMask"]
			});
			Vector2 size = new Vector2(contentLayout.Width, contentLayout.Height / 5f);
			BuyPanel buyPanel = new BuyPanel(size, shop);
			infoPanels.Add(buyPanel);
			addChild(buyPanel);
			PlantOnPanel plantOnPanel = new PlantOnPanel(size, shop);
			infoPanels.Add(plantOnPanel);
			addChild(plantOnPanel);
			HarvestPanel harvestPanel = new HarvestPanel(size, shop);
			infoPanels.Add(harvestPanel);
			addChild(harvestPanel);
			HarvestEachPanel harvestEachPanel = new HarvestEachPanel(size, shop);
			infoPanels.Add(harvestEachPanel);
			addChild(harvestEachPanel);
			FeedEachPanel feedEachPanel = new FeedEachPanel(size);
			infoPanels.Add(feedEachPanel);
			addChild(feedEachPanel);
			FeedPanel feedPanel = new FeedPanel(size);
			infoPanels.Add(feedPanel);
			addChild(feedPanel);
			FuelPanel fuelPanel = new FuelPanel(size);
			infoPanels.Add(fuelPanel);
			addChild(fuelPanel);
			RefuelPanel refuelPanel = new RefuelPanel(size, shop);
			infoPanels.Add(refuelPanel);
			addChild(refuelPanel);
			ProducesPanel producesPanel = new ProducesPanel(size);
			infoPanels.Add(producesPanel);
			addChild(producesPanel);
			GamblePanel gamblePanel = new GamblePanel(size);
			infoPanels.Add(gamblePanel);
			addChild(gamblePanel);
			SizePanel sizePanel = new SizePanel(size);
			infoPanels.Add(sizePanel);
			addChild(sizePanel);
			WorkAreaPanel workAreaPanel = new WorkAreaPanel(size);
			infoPanels.Add(workAreaPanel);
			addChild(workAreaPanel);
			UnlocksPanel unlocksPanel = new UnlocksPanel(size);
			infoPanels.Add(unlocksPanel);
			addChild(unlocksPanel);
		}

		protected override void DoUpdate()
		{
			if (currentItem != null && dirty)
			{
				float num = contentLayout.Height / 5f;
				Vector2 v = contentLayout.Top - new Vector2(0f, num * 0.5f);
				InfoPanel infoPanel = null;
				foreach (InfoPanel infoPanel2 in infoPanels)
				{
					if (infoPanel2.CanBeUsed(currentItem))
					{
						infoPanel2.Show(currentItem);
						infoPanel2.Transform.Translation = v.ToVector3();
						v -= new Vector2(0f, num);
						infoPanel = infoPanel2;
						infoPanel2.IsLast = false;
					}
					else
					{
						infoPanel2.Visible = false;
					}
				}
				if (infoPanel != null)
				{
					infoPanel.IsLast = true;
				}
				previewItem.clearMeshes();
				AvatarFarmOnline.Items.Game.FarmTileItem.LoadMesh(currentItem.Category, currentItem.Id, previewItem);
				previewCamera.SetNewItem(currentItem);
				previewCamera.Update();
				int[] array = new int[floorMesh.TileIndices.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = 0;
					if ((double)i == Math.Floor((float)array.Length * 0.5f))
					{
						switch (currentItem.Category)
						{
						case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
						case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
						case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
						case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
							array[i] = 3;
							break;
						case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant:
							array[i] = 2;
							break;
						}
					}
				}
				floorMesh.TileIndices = array;
				nameMesh.StringBuilder.Length = 0;
				nameMesh.StringBuilder.Append(currentItem.Name.Translate());
				levelItem.Visible = currentItem.MinLevel > 1 && shop.FarmData.PlayerData.Level < currentItem.MinLevel;
				if (levelItem.Visible)
				{
					levelMesh.StringBuilder.Length = 0;
					levelMesh.StringBuilder.Append("FARM_LEVEL".Translate());
					levelMesh.StringBuilder.Append(' ');
					levelMesh.StringBuilder.AppendNumber(currentItem.MinLevel);
				}
				buildingNeededItem.Visible = currentItem.IsBuildingNeeded && !shop.FarmData.HasBuilding(currentItem.BuildingNeeded);
				if (buildingNeededItem.Visible)
				{
					buildingNeededMesh.StringBuilder.Length = 0;
					buildingNeededMesh.StringBuilder.Append("NEEDS".Translate());
					buildingNeededMesh.StringBuilder.Append(' ');
					buildingNeededMesh.StringBuilder.Append(AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetBuilding(currentItem.BuildingNeeded).Name.Translate());
					buildingNeededItem.Transform.Translation = (levelItem.Visible ? new Vector3(0f, levelBgMesh.Size.Y, 0f) : Vector3.Zero);
				}
				dirty = false;
			}
			previewProcess.Enabled = Visible;
			base.DoUpdate();
		}

		private void UpdateTexture()
		{
			switch (currentItem.Category)
			{
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
				previewMesh.Texture = TextureManager.Textures["ShopIcons/Animals/" + currentItem.Id];
				break;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
				previewMesh.Texture = TextureManager.Textures["ShopIcons/Buildings/" + currentItem.Id];
				break;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
				previewMesh.Texture = TextureManager.Textures["ShopIcons/Decorations/" + currentItem.Id];
				break;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant:
				previewMesh.Texture = TextureManager.Textures["ShopIcons/Plants/" + currentItem.Id];
				break;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
				previewMesh.Texture = TextureManager.Textures["ShopIcons/Tools/" + currentItem.Id];
				break;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
				previewMesh.Texture = TextureManager.Textures["ShopIcons/Trees/" + currentItem.Id];
				break;
			}
		}

		public override void Dispose()
		{
			if (previewProcess != null)
			{
				previewProcess.Dispose();
				previewProcess = null;
			}
			if (previewScene != null)
			{
				previewScene.Dispose();
				previewScene = null;
			}
			base.Dispose();
		}
	}

	private class CategoryHeader : RenderItem
	{
		private Sized2DRectangleMesh[] icons;

		private BorderedRectangle selectionMesh;

		public AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory Category
		{
			set
			{
				selectionMesh.Offset = icons[0].Offset + new Vector2(icons[0].Size.X * (float)value, -6f);
			}
		}

		public CategoryHeader(Layout2D.LayoutData layout)
		{
			Layout2D.HorizontalFixedFloatLayout(layout, 35f, 5f, out var fixedLayout, out layout);
			Layout2D.HorizontalFloatFixedLayout(layout, 35f, 5f, out var fixedLayout2, out layout);
			float[] coordBorder = new float[4] { 8f, 8f, 8f, 4f };
			selectionMesh = new BorderedRectangle(TextureManager.Textures["GUI/TabMask"], new Vector2(1.05f * (layout.Width / 6f), layout.Height), coordBorder);
			selectionMesh.FirstMaterial.Textures.Add(TextureManager.Textures["GUI/WoodTileTex"]);
			selectionMesh.Shader = ShaderManager.Shaders["GUIMask"];
			addMesh(selectionMesh);
			icons = new Sized2DRectangleMesh[6];
			for (int i = 0; i < 6; i++)
			{
				Layout2D.GridLayout(layout, 6, 1, 0f, i, out var cellLayout);
				Layout2D.KeepAspectRatio(cellLayout, 1f, out cellLayout);
				icons[i] = new Sized2DRectangleMesh(cellLayout, TextureManager.Textures["GUI/Categories"]);
				icons[i].SetTile(i, 8, 1);
				addMesh(icons[i]);
			}
			addMesh(new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(fixedLayout, HorizontalAlignment.Center, VerticalAlignment.Center), 2, useStringBuilder: false)
			{
				Text = InputManager.GetInputGlyph(InputManager.MenuInputCodes.PrevPage).ToString()
			});
			addMesh(new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(fixedLayout2, HorizontalAlignment.Center, VerticalAlignment.Center), 2, useStringBuilder: false)
			{
				Text = InputManager.GetInputGlyph(InputManager.MenuInputCodes.NextPage).ToString()
			});
		}
	}

	private class ItemList : RenderItem
	{
		private AvatarFarmOnline.Template.Controls.Shop shop;

		private List<ItemData> items = new List<ItemData>();

		public ItemList(AvatarFarmOnline.Template.Controls.Shop shop, Layout2D.LayoutData layout)
		{
			float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
			Layout2D.InsideBorderLayout(layout, 6f, out var result);
			addMesh(new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], result, coordBorder)
			{
				Diffuse = Vector3.Zero,
				Alpha = 0.5f
			});
			addMesh(new BorderedRectangle(TextureManager.Textures["GUI/ExtBorderMask"], result, coordBorder)
			{
				FirstMaterial = 
				{
					Textures = { (Texture)TextureManager.Textures["GUI/WoodTileTex"] }
				},
				Shader = ShaderManager.Shaders["GUIMask"]
			});
			this.shop = shop;
			Layout2D.InsideBorderLayout(result, 20f, out var result2);
			for (int i = 0; i < 6; i++)
			{
				Layout2D.GridLayout(result2, 2, 3, 9f, i, out var cellLayout);
				ItemData itemData = new ItemData(shop, cellLayout, i);
				items.Add(itemData);
				addChild(itemData);
			}
			shop.OnChange += OnChange;
		}

		private void OnChange()
		{
			foreach (ItemData item in items)
			{
				item.UpdateData();
			}
		}
	}

	private abstract class InfoPanel : RenderItem
	{
		private Sized2DRectangleMesh border;

		protected TextBoxMesh titleMesh;

		protected TextBoxMesh dataMesh;

		private Vector3 titleColor = new Vector3(0f, 1f, 1f);

		protected Vector3 TitleColor => titleColor;

		public bool IsLast
		{
			set
			{
				border.Alpha = ((!value) ? 1 : 0);
			}
		}

		public InfoPanel(Vector2 size)
		{
			Visible = false;
			titleMesh = new TextBoxMesh(BitmapFontManager.Fonts["Standard"], new TextBoxDrawProperties(size, 0.65f / GameMath.Interpolate(Math.Min(1f, Engine.GUIScale), 1f, 0.25f), HorizontalAlignment.Left, VerticalAlignment.Center), 30, useStringBuilder: false);
			titleMesh.Diffuse = TitleColor;
			addMesh(titleMesh);
			dataMesh = new TextBoxMesh(BitmapFontManager.Fonts["Standard"], new TextBoxDrawProperties(size, 0.65f / GameMath.Interpolate(Math.Min(1f, Engine.GUIScale), 1f, 0.25f), HorizontalAlignment.Right, VerticalAlignment.Center), 30, useStringBuilder: true);
			addMesh(dataMesh);
			border = new Sized2DRectangleMesh(new Vector2(size.X, 2f), new Vector3(0.5f));
			border.Offset = new Vector2(0f, -0.5f * size.Y);
			addMesh(border);
		}

		public abstract bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item);

		public void Show(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			Visible = true;
			dataMesh.StringBuilder.Length = 0;
			DoShow(item);
		}

		protected abstract void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item);
	}

	private class BuyPanel : InfoPanel
	{
		private AvatarFarmOnline.Template.Controls.Shop shop;

		public BuyPanel(Vector2 size, AvatarFarmOnline.Template.Controls.Shop shop)
			: base(size)
		{
			this.shop = shop;
			titleMesh.Text = "BUY".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			return true;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			dataMesh.StringBuilder.Append('-');
			dataMesh.StringBuilder.AppendNumber(item.Price.Amount, AppendNumberOptions.NumberGroup);
			dataMesh.StringBuilder.Append(item.Price.MoneyChar);
			if (item.Price.Type == AvatarFarmOnline.Logic.Money.MoneyType.Coins && shop.FarmData.PlayerData.Coins < item.Price.Amount)
			{
				titleMesh.Diffuse = Vector3.UnitX;
			}
			else if (item.Price.Type == AvatarFarmOnline.Logic.Money.MoneyType.Cash && shop.FarmData.PlayerData.Cash < item.Price.Amount)
			{
				titleMesh.Diffuse = Vector3.UnitX;
			}
			else
			{
				titleMesh.Diffuse = base.TitleColor;
			}
			int num = 0;
			if (item is AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)
			{
				num = ((AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)item).PlantXp;
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)
			{
				num = ((AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)item).PlantXp;
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)
			{
				num = ((AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)item).PlantXp;
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)
			{
				num = ((AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)item).BuildXp;
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)
			{
				num = ((AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)item).BuildXp;
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)
			{
				num = ((AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item).BuildXp;
			}
			if (num > 0)
			{
				dataMesh.StringBuilder.Append("   ");
				dataMesh.StringBuilder.AppendNumber(num);
				dataMesh.StringBuilder.Append('\u0bbd');
			}
		}
	}

	private class HarvestPanel : InfoPanel
	{
		private AvatarFarmOnline.Template.Controls.Shop shop;

		private int xp;

		private AvatarFarmOnline.Logic.Money money;

		public HarvestPanel(Vector2 size, AvatarFarmOnline.Template.Controls.Shop shop)
			: base(size)
		{
			this.shop = shop;
			titleMesh.Text = "HARVEST".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			xp = 0;
			if (item is AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)
			{
				xp = ((AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)item).GatherXp;
				money = ((AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)item).GatherMoney;
				return true;
			}
			if (item is AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)
			{
				xp = ((AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)item).GatherXp;
				money = ((AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)item).GatherMoney;
				return true;
			}
			if (item is AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)
			{
				xp = ((AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)item).GatherXp;
				money = ((AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)item).GatherMoney;
				return true;
			}
			if (item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)
			{
				AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition buildingDefinition = (AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item;
				if (buildingDefinition.CanBeGathered)
				{
					xp = buildingDefinition.GatherXp;
					money = buildingDefinition.GatherAmount;
					return true;
				}
				if (buildingDefinition.CanAccumulateItems)
				{
					xp = buildingDefinition.ItemAccumulationXp;
					money = buildingDefinition.ItemAccumulationAmount;
					return true;
				}
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (money.Amount > 0)
			{
				dataMesh.StringBuilder.AppendNumber(money.Amount, AppendNumberOptions.NumberGroup);
				dataMesh.StringBuilder.Append(money.MoneyChar);
			}
			if (xp > 0)
			{
				dataMesh.StringBuilder.Append("   ");
				dataMesh.StringBuilder.AppendNumber(xp, AppendNumberOptions.NumberGroup);
				dataMesh.StringBuilder.Append('\u0bbd');
			}
		}
	}

	private class HarvestEachPanel : InfoPanel
	{
		private AvatarFarmOnline.Template.Controls.Shop shop;

		public HarvestEachPanel(Vector2 size, AvatarFarmOnline.Template.Controls.Shop shop)
			: base(size)
		{
			this.shop = shop;
			titleMesh.Text = "HARVEST_EACH".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition || item is AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition || item is AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)
			{
				return true;
			}
			if (item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)
			{
				AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition buildingDefinition = (AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item;
				if (buildingDefinition.CanAccumulateItems || buildingDefinition.CanBeGathered)
				{
					return true;
				}
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)
			{
				dataMesh.StringBuilder.Append('\u0bba');
				dataMesh.StringBuilder.Append(' ');
				AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(((AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)item).GrowTime, dataMesh.StringBuilder);
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)
			{
				AvatarFarmOnline.Logic.Parsing.GetSeasonsText(((AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)item).GatherSeasons, dataMesh.StringBuilder);
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)
			{
				AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition animalDefinition = (AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)item;
				dataMesh.StringBuilder.AppendNumber(animalDefinition.FeedAmount);
				dataMesh.StringBuilder.Append(' ');
				dataMesh.StringBuilder.Append("FEEDS".Translate());
			}
			else
			{
				if (!(item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition))
				{
					return;
				}
				AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition buildingDefinition = (AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item;
				if (buildingDefinition.CanAccumulateItems)
				{
					dataMesh.StringBuilder.AppendNumber(buildingDefinition.ItemAccumulationCount);
					dataMesh.StringBuilder.Append(' ');
					dataMesh.StringBuilder.Append(buildingDefinition.ItemAccumulationCategoryText.Translate());
					if (buildingDefinition.ItemAccumulationCount > 1)
					{
						dataMesh.StringBuilder.Append('s');
					}
				}
				else if (buildingDefinition.CanBeGathered)
				{
					dataMesh.StringBuilder.Append('\u0bba');
					dataMesh.StringBuilder.Append(' ');
					AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(((AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item).GatherPeriod, dataMesh.StringBuilder);
				}
			}
		}
	}

	private class GamblePanel : InfoPanel
	{
		public GamblePanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "PLAY_PRICE".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)
			{
				AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition buildingDefinition = (AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item;
				if (buildingDefinition.CanGamble)
				{
					return true;
				}
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)
			{
				AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition buildingDefinition = (AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item;
				if (buildingDefinition.CanGamble)
				{
					dataMesh.StringBuilder.AppendNumber(buildingDefinition.GamblePrice.Amount, AppendNumberOptions.NumberGroup);
					dataMesh.StringBuilder.Append(buildingDefinition.GamblePrice.MoneyChar);
				}
			}
		}
	}

	private class PlantOnPanel : InfoPanel
	{
		private AvatarFarmOnline.Template.Controls.Shop shop;

		public PlantOnPanel(Vector2 size, AvatarFarmOnline.Template.Controls.Shop shop)
			: base(size)
		{
			this.shop = shop;
			titleMesh.Text = "PLANT_ON".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition plantDefinition = (AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)item;
			if ((plantDefinition.PlantSeasons & shop.FarmData.CurrentSeason) != AvatarFarmOnline.Logic.Seasons.None)
			{
				titleMesh.Diffuse = base.TitleColor;
			}
			else
			{
				titleMesh.Diffuse = Vector3.UnitX;
			}
			AvatarFarmOnline.Logic.Parsing.GetSeasonsText(plantDefinition.PlantSeasons, dataMesh.StringBuilder);
		}
	}

	private class FeedEachPanel : InfoPanel
	{
		public FeedEachPanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "FEED_EACH".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition animalDefinition = (AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)item;
			dataMesh.StringBuilder.Append('\u0bba');
			dataMesh.StringBuilder.Append(' ');
			AvatarFarmOnline.Logic.Parsing.SetTimeTextShort(animalDefinition.FeedInterval, dataMesh.StringBuilder);
		}
	}

	private class FeedPanel : InfoPanel
	{
		public FeedPanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "FEED".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition animalDefinition = (AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition)item;
			dataMesh.StringBuilder.AppendNumber(animalDefinition.FeedMoney.Amount, AppendNumberOptions.NumberGroup);
			dataMesh.StringBuilder.Append(animalDefinition.FeedMoney.MoneyChar);
			if (animalDefinition.FeedXp > 0)
			{
				dataMesh.StringBuilder.Append("   ");
				dataMesh.StringBuilder.AppendNumber(animalDefinition.FeedXp);
				dataMesh.StringBuilder.Append('\u0bbd');
			}
		}
	}

	private class FuelPanel : InfoPanel
	{
		public FuelPanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "FUEL_CAPACITY".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition toolDefinition = (AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)item;
			dataMesh.StringBuilder.AppendNumber(toolDefinition.MaxFuel);
		}
	}

	private class RefuelPanel : InfoPanel
	{
		private AvatarFarmOnline.Template.Controls.Shop shop;

		public RefuelPanel(Vector2 size, AvatarFarmOnline.Template.Controls.Shop shop)
			: base(size)
		{
			this.shop = shop;
			titleMesh.Text = "REFUEL".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition toolDefinition = (AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)item;
			dataMesh.StringBuilder.Append("- ");
			dataMesh.StringBuilder.AppendNumber(toolDefinition.RefillPrice.Amount);
			dataMesh.StringBuilder.Append(toolDefinition.RefillPrice.MoneyChar);
			if (toolDefinition.RefillXp > 0)
			{
				dataMesh.StringBuilder.Append("   ");
				dataMesh.StringBuilder.AppendNumber(toolDefinition.RefillXp);
				dataMesh.StringBuilder.Append('\u0bbd');
			}
			if (toolDefinition.RefillPrice.Type == AvatarFarmOnline.Logic.Money.MoneyType.Coins && shop.FarmData.PlayerData.Coins < toolDefinition.RefillPrice.Amount)
			{
				titleMesh.Diffuse = Vector3.UnitX;
			}
			else if (toolDefinition.RefillPrice.Type == AvatarFarmOnline.Logic.Money.MoneyType.Cash && shop.FarmData.PlayerData.Cash < toolDefinition.RefillPrice.Amount)
			{
				titleMesh.Diffuse = Vector3.UnitX;
			}
			else
			{
				titleMesh.Diffuse = base.TitleColor;
			}
		}
	}

	private class ProducesPanel : InfoPanel
	{
		public ProducesPanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "PRODUCES".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition || item is AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)
			{
				dataMesh.StringBuilder.Append(((AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition)item).PlantCategoryText.Translate());
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)
			{
				dataMesh.StringBuilder.Append(((AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition)item).PlantCategoryText.Translate());
			}
		}
	}

	private class SizePanel : InfoPanel
	{
		public SizePanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "SIZE".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition || item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			Int2 @int = default(Int2);
			if (item is AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)
			{
				@int = ((AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)item).Size;
			}
			else if (item is AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)
			{
				@int = ((AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition)item).Size;
			}
			dataMesh.StringBuilder.AppendNumber(@int.X);
			dataMesh.StringBuilder.Append("x");
			dataMesh.StringBuilder.AppendNumber(@int.Y);
		}
	}

	private class WorkAreaPanel : InfoPanel
	{
		public WorkAreaPanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "WORK_AREA".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			Int2 @int = new Int2(((AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition)item).ToolSize);
			AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition toolDefinition = item as AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition;
			AvatarFarmOnline.Logic.Parsing.GetToolTypeText(toolDefinition.ToolType, dataMesh.StringBuilder);
			dataMesh.StringBuilder.Append(' ');
			dataMesh.StringBuilder.AppendNumber(@int.X);
			dataMesh.StringBuilder.Append('x');
			dataMesh.StringBuilder.AppendNumber(@int.Y);
		}
	}

	private class UnlocksPanel : InfoPanel
	{
		public UnlocksPanel(Vector2 size)
			: base(size)
		{
			titleMesh.Text = "UNLOCKS_WITH".Translate();
		}

		public override bool CanBeUsed(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			if (item is AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition && ((AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)item).NeedsUnlockedGame)
			{
				return true;
			}
			return false;
		}

		protected override void DoShow(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item)
		{
			dataMesh.StringBuilder.Append(CrossPromotionManager.Instance.GetGameName(((AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition)item).GameNeeded));
		}
	}

	private const int SHOP_ITEM_COUNT = 6;

	private const int DETAIL_PANELS_COUNT = 5;

	private AvatarFarmOnline.Template.Controls.Shop shop;

	private ItemDetails itemDetails;

	private ItemList itemList;

	private global::ScrollbarItem scrollbar;

	private CategoryHeader header;

	public ShopItem(AvatarFarmOnline.Template.Controls.Shop shop)
	{
		this.shop = shop;
		float num = 720f;
		num *= 0.78f / GameMath.Interpolate(Math.Min(1f, Engine.GUIScale), 1f, 0.6f);
		Layout2D.LayoutData container = new Layout2D.LayoutData(new Vector2((0f - num) * 0.03f, num * 0.03f), new Vector2(num * 1.25f, num));
		Layout2D.HorizontalFixedFloatLayout(container, 380f, 9f, out var fixedLayout, out var floatLayout);
		Layout2D.HorizontalFixedFloatLayout(fixedLayout, 40f, 9f, out var fixedLayout2, out var floatLayout2);
		float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
		addMesh(new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], floatLayout2, coordBorder)
		{
			Diffuse = Vector3.Zero,
			Alpha = 0.5f
		});
		Layout2D.VerticalFixedFloatLayout(fixedLayout2, 51f, 0f, out var _, out var floatLayout3);
		scrollbar = new global::ScrollbarItem(floatLayout3, vertical: true);
		scrollbar.MarkerSize = 3;
		addChild(scrollbar);
		Layout2D.VerticalFixedFloatLayout(floatLayout2, 52f, 0f, out var fixedLayout4, out var floatLayout4);
		itemList = new ItemList(shop, floatLayout4);
		addChild(itemList);
		Layout2D.VerticalFixedFloatLayout(floatLayout2, 58f, 0f, out fixedLayout4, out floatLayout4);
		header = new CategoryHeader(fixedLayout4);
		addChild(header);
		Layout2D.VerticalFixedFloatLayout(floatLayout, 52f, 6f, out var fixedLayout5, out var floatLayout5);
		float[] coordBorder2 = new float[4] { 0f, 0f, 0f, 8f };
		addMesh(new BorderedRectangle(TextureManager.Textures["GUI/LowerShadowMask"], fixedLayout5, coordBorder2)
		{
			FirstMaterial = 
			{
				Textures = { (Texture)TextureManager.Textures["GUI/WoodTileTex"] }
			},
			Shader = ShaderManager.Shaders["GUIMask"]
		});
		addMesh(new TextMesh(BitmapFontManager.Fonts["Menu"], new TextDrawProperties(Vector2.Zero, 1.25f, HorizontalAlignment.Center), 20, useStringBuilder: false)
		{
			Offset = fixedLayout5.Bottom + new Vector2(0f, fixedLayout5.Height * 0.8f),
			Text = "SHOP".Translate()
		});
		itemDetails = new ItemDetails(shop, floatLayout5);
		addChild(itemDetails);
		shop.OnChange += OnChange;
		shop.OnAppear += shop_OnAppear;
		itemDetails.CurrentItem = shop.CurrentItem;
	}

	private void shop_OnAppear()
	{
		transform.Scale = new Vector3(1.1f);
	}

	protected override void DoUpdate()
	{
		transform.Scale = Vector3.Lerp(transform.Scale, Vector3.One, 0.2f);
		base.DoUpdate();
	}

	private void OnChange()
	{
		header.Category = shop.CurrentCategory;
		scrollbar.TotalItems = (shop.Items.Count + 2 - 1) / 2;
		scrollbar.BaseItem = shop.BaseIndex / 2;
		itemDetails.CurrentItem = shop.CurrentItem;
		GameTemplate.MoveAudio.Start();
	}
}
