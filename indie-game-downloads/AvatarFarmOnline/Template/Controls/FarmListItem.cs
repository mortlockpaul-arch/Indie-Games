using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Mode.Farm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Template.Controls;

internal class FarmListItem : RenderItem
{
	public class FarmDetailsItem : RenderItem
	{
		private TextBoxMesh name;

		private TextBoxMesh levelText;

		private TextBoxMesh dateText;

		private TextBoxMesh moneyText;

		private TextBoxMesh onlineText;

		private Sized2DRectangleMesh icon;

		private BorderedRectangle bg;

		private BorderedRectangle selectedBg;

		private BorderedRectangle dateBg;

		private bool selected;

		public FarmDetailsItem(Layout2D.LayoutData layout)
		{
			float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
			selectedBg = new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], layout, coordBorder);
			selectedBg.Diffuse = new Vector3(0f, 1f, 1f);
			selectedBg.Alpha = 0f;
			addMesh(selectedBg);
			Layout2D.InsideBorderLayout(layout, 4f, out layout);
			bg = new BorderedRectangle(TextureManager.Textures["GUI/ShopItemBg"], layout, coordBorder);
			bg.FirstMaterial.Textures.Add(TextureManager.Textures["GUI/WoodTileTex"]);
			bg.Shader = ShaderManager.Shaders["GUIMask"];
			addMesh(bg);
			Layout2D.InsideBorderLayout(layout, 8f, out layout);
			Layout2D.HorizontalFixedFloatLayout(layout, layout.Height, 30f, out var fixedLayout, out layout);
			icon = new Sized2DRectangleMesh(fixedLayout, TextureManager.Textures["ShopIcons/Buildings/Barn"]);
			addMesh(icon);
			levelText = new TextBoxMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.MenuFont, new TextBoxDrawProperties(fixedLayout, 1f, HorizontalAlignment.Right, VerticalAlignment.Bottom), 4, useStringBuilder: true);
			levelText.Offset += new Vector2(0f, 5f);
			addMesh(levelText);
			Layout2D.VerticalRelativeSizeLayout(layout, 0.5f, 0f, out var topLayout, out var bottomLayout);
			Layout2D.HorizontalRelativeSizeLayout(topLayout, 0.8f, 0f, out var leftLayout, out var rightLayout);
			name = new TextBoxMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.MenuFont, new TextBoxDrawProperties(leftLayout, 1f, HorizontalAlignment.Left, VerticalAlignment.Bottom), 96, useStringBuilder: true);
			addMesh(name);
			dateBg = new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], rightLayout, coordBorder);
			dateBg.Diffuse = Vector3.Zero;
			addMesh(dateBg);
			dateText = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(rightLayout, 1f, HorizontalAlignment.Center, VerticalAlignment.Center), 15, useStringBuilder: true);
			dateText.Offset += new Vector2(0f, -2f);
			addMesh(dateText);
			moneyText = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(bottomLayout, 1f, HorizontalAlignment.Left, VerticalAlignment.Center), 50, useStringBuilder: true);
			addMesh(moneyText);
			onlineText = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(bottomLayout, 1f, HorizontalAlignment.Right, VerticalAlignment.Center), 30, useStringBuilder: true);
			onlineText.Offset += new Vector2(-2f, 0f);
			addMesh(onlineText);
		}

		private void changeIcon(int level)
		{
			string text = "ShopIcons/Decorations/HayPile";
			if (level > 20)
			{
				text = "ShopIcons/Buildings/Bakery";
			}
			else if (level > 10)
			{
				text = "ShopIcons/Buildings/Barn";
			}
			else if (level > 5)
			{
				text = "ShopIcons/Decorations/HayStack";
			}
			icon.Texture = TextureManager.Textures[text];
		}

		public void SetFarm(int index, AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader data, bool isSelected)
		{
			Visible = true;
			int level = data.Level;
			changeIcon(level);
			name.StringBuilder.Length = 0;
			name.StringBuilder.Append(data.Name);
			levelText.StringBuilder.Length = 0;
			levelText.StringBuilder.AppendNumber(level);
			dateText.StringBuilder.Length = 0;
			dateText.StringBuilder.Append(data.LastSave.ToShortDateString());
			dateBg.Alpha = 0.6f;
			moneyText.StringBuilder.Length = 0;
			moneyText.StringBuilder.AppendNumber(data.Coins, AppendNumberOptions.NumberGroup);
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CoinChar);
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.AppendNumber(data.Cash, AppendNumberOptions.NumberGroup);
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CashChar);
			onlineText.StringBuilder.Length = 0;
			switch (data.PlayMode)
			{
			case PlayMode.Private:
				onlineText.StringBuilder.Append("INVITE_ONLY".Translate());
				onlineText.Diffuse = new Vector3(0.3f, 1f, 1f);
				break;
			case PlayMode.Local:
				onlineText.StringBuilder.Append("OFFLINE".Translate());
				onlineText.Diffuse = Vector3.One;
				break;
			case PlayMode.Public:
				onlineText.StringBuilder.Append("ONLINE".Translate());
				onlineText.Diffuse = new Vector3(0.3f, 1f, 0.3f);
				break;
			}
			update(isSelected);
		}

		public void SetCreateFarm(bool isSelected)
		{
			Visible = true;
			icon.Texture = TextureManager.Textures["ShopIcons/Grass"];
			name.StringBuilder.Length = 0;
			name.StringBuilder.Append("CREATE_NEW_FARM".Translate());
			levelText.StringBuilder.Length = 0;
			onlineText.StringBuilder.Length = 0;
			moneyText.StringBuilder.Length = 0;
			dateText.StringBuilder.Length = 0;
			dateBg.Alpha = 0f;
			update(isSelected);
		}

		public void SetImportFarm(bool isSelected)
		{
			Visible = true;
			icon.Texture = TextureManager.Textures["ShopIcons/Plowed"];
			name.StringBuilder.Length = 0;
			name.StringBuilder.Append("IMPORT_FARM".Translate());
			moneyText.StringBuilder.Length = 0;
			moneyText.StringBuilder.Append("IMPORT_FARM_CODE".Translate());
			levelText.StringBuilder.Length = 0;
			onlineText.StringBuilder.Length = 0;
			dateText.StringBuilder.Length = 0;
			dateBg.Alpha = 0f;
			update(isSelected);
		}

		private void update(bool isSelected)
		{
			TextBoxMesh textBoxMesh = name;
			Vector3 diffuse = (levelText.Diffuse = (isSelected ? Vector3.One : GameMath.RGBToVector(150, 220, 60)));
			textBoxMesh.Diffuse = diffuse;
			selected = isSelected;
		}

		protected override void DoUpdate()
		{
			selectedBg.Alpha = (selected ? (0.5f + 0.5f * Math.Abs((float)Math.Sin(Timer.DefaultTimer.TotalTimeSeconds * 5f))) : 0f);
			base.DoUpdate();
		}

		public void Hide()
		{
			Visible = false;
		}
	}

	private AvatarFarmOnline.Template.Controls.FarmList farmList;

	private List<FarmDetailsItem> foundGames = new List<FarmDetailsItem>(4);

	private global::ScrollbarItem scrollbar;

	public FarmListItem(AvatarFarmOnline.Template.Controls.FarmList farmList)
	{
		Transform.Scale = new Vector3(0.9f);
		this.farmList = farmList;
		Layout2D.LayoutData container = new Layout2D.LayoutData(new Vector2(0f, -15f), new Vector2(Engine.GUIWidth * 0.8f, Engine.GUIHeight * 0.65f));
		Layout2D.HorizontalFixedFloatLayout(container, 44f, 9f, out var fixedLayout, out var floatLayout);
		float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
		scrollbar = new global::ScrollbarItem(fixedLayout, vertical: true);
		scrollbar.TotalItems = farmList.ResultCount;
		scrollbar.MarkerSize = 4;
		addChild(scrollbar);
		addMesh(new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], floatLayout, coordBorder)
		{
			Diffuse = Vector3.Zero,
			Alpha = 0.5f
		});
		Layout2D.InsideBorderLayout(floatLayout, 8f, out floatLayout);
		addMesh(new BorderedRectangle(TextureManager.Textures["GUI/ExtBorderMask"], floatLayout, coordBorder)
		{
			FirstMaterial = 
			{
				Textures = { (Texture)TextureManager.Textures["GUI/WoodLightTileTex"] }
			},
			Shader = ShaderManager.Shaders["GUIMask"]
		});
		Layout2D.InsideBorderLayout(floatLayout, 16f, out floatLayout);
		for (int i = 0; i < 4; i++)
		{
			Layout2D.GridLayout(floatLayout, 1, 4, 0f, i, out var cellLayout);
			FarmDetailsItem farmDetailsItem = new FarmDetailsItem(cellLayout);
			foundGames.Add(farmDetailsItem);
			addChild(farmDetailsItem);
		}
		farmList.OnMove += OnMove;
		farmList.OnSelected += OnSelected;
		UpdateData();
	}

	private void OnSelected(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader obj)
	{
		GameTemplate.InteractionAudio.Start();
	}

	private void OnMove()
	{
		GameTemplate.MoveAudio.Start();
		UpdateData();
	}

	protected override void DoUpdate()
	{
		transform.Scale = Vector3.Lerp(transform.Scale, Vector3.One, 0.2f);
		base.DoUpdate();
	}

	private void UpdateData()
	{
		List<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> entries = farmList.GetEntries();
		scrollbar.BaseItem = farmList.BaseIndex;
		int num = 0;
		for (num = 0; num < entries.Count; num++)
		{
			if (num + farmList.BaseIndex == farmList.EffectiveCount)
			{
				foundGames[num].SetCreateFarm(farmList.BaseIndex + num == farmList.SelectedIndex);
			}
			else if (num + farmList.BaseIndex == farmList.EffectiveCount + 1)
			{
				foundGames[num].SetImportFarm(farmList.BaseIndex + num == farmList.SelectedIndex);
			}
			else
			{
				foundGames[num].SetFarm(farmList.BaseIndex + num, entries[num], farmList.BaseIndex + num == farmList.SelectedIndex);
			}
		}
		for (; num < foundGames.Count; num++)
		{
			foundGames[num].Hide();
		}
	}
}
