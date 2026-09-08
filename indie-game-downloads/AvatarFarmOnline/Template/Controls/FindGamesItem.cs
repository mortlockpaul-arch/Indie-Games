using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Mode.Farm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Network;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Template.Controls;

internal class FindGamesItem : RenderItem
{
	public class FoundGameDetailsItem : RenderItem
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

		public FoundGameDetailsItem(Layout2D.LayoutData layout)
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
			Layout2D.HorizontalRelativeSizeLayout(topLayout, 0.88f, 0f, out var leftLayout, out var rightLayout);
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

		public void SetResult(IAvailableSession data, bool isSelected)
		{
			selected = isSelected;
			Visible = true;
			TextBoxMesh textBoxMesh = name;
			Vector3 diffuse = (levelText.Diffuse = (isSelected ? Vector3.One : GameMath.RGBToVector(150, 220, 60)));
			textBoxMesh.Diffuse = diffuse;
			int? num = data.SessionProperties[3];
			int num2 = 1;
			if (num.HasValue)
			{
				num2 = num.Value;
			}
			changeIcon(num2);
			name.StringBuilder.Length = 0;
			name.StringBuilder.Append(data.HostGamertag);
			levelText.StringBuilder.Length = 0;
			levelText.StringBuilder.AppendNumber(num2);
			int number = data.CurrentGamerCount + data.OpenPublicGamerSlots + data.OpenPrivateGamerSlots;
			dateText.StringBuilder.Length = 0;
			dateText.StringBuilder.AppendNumber(data.CurrentGamerCount, 2, AppendNumberOptions.FixedSize);
			dateText.StringBuilder.Append("/");
			dateText.StringBuilder.AppendNumber(number);
			dateBg.Alpha = 0.6f;
			int number2 = 0;
			int number3 = 0;
			int? num3 = data.SessionProperties[4];
			int? num4 = data.SessionProperties[5];
			if (num3.HasValue)
			{
				number2 = num3.Value;
			}
			if (num4.HasValue)
			{
				number3 = num4.Value;
			}
			moneyText.StringBuilder.Length = 0;
			moneyText.StringBuilder.AppendNumber(number2, AppendNumberOptions.NumberGroup);
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CoinChar);
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.AppendNumber(number3, AppendNumberOptions.NumberGroup);
			moneyText.StringBuilder.Append(' ');
			moneyText.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CashChar);
			onlineText.StringBuilder.Length = 0;
			AvatarFarmOnline.Logic.PlayerPermissions playerPermissions = AvatarFarmOnline.Logic.PlayerPermissions.Full;
			AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
			ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(farmPersistentGameData.Setup.PlayerIndex);
			if (gamer != null)
			{
				playerPermissions = (AvatarFarmOnline.Logic.PlayerPermissions)((!gamer.IsFriend(data.HostGamertag)) ? data.SessionProperties[2].Value : data.SessionProperties[1].Value);
			}
			switch (playerPermissions)
			{
			case AvatarFarmOnline.Logic.PlayerPermissions.None:
				onlineText.StringBuilder.Append("GUEST_PERMISSIONS".Translate());
				onlineText.Diffuse = new Vector3(1f, 0.5f, 0.5f);
				break;
			case AvatarFarmOnline.Logic.PlayerPermissions.Harvest:
				onlineText.StringBuilder.Append("HARVEST_PERMISSIONS".Translate());
				onlineText.Diffuse = new Vector3(1f, 1f, 0.3f);
				break;
			case AvatarFarmOnline.Logic.PlayerPermissions.Build:
				onlineText.StringBuilder.Append("BUILD_PERMISSIONS".Translate());
				onlineText.Diffuse = new Vector3(0.3f, 1f, 1f);
				break;
			case AvatarFarmOnline.Logic.PlayerPermissions.Full:
				onlineText.StringBuilder.Append("FULL_PERMISSIONS".Translate());
				onlineText.Diffuse = new Vector3(0.3f, 1f, 0.3f);
				break;
			}
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

	private AvatarFarmOnline.Template.Controls.FindGames findGames;

	private List<FoundGameDetailsItem> foundGames = new List<FoundGameDetailsItem>(4);

	private RenderItem statusItem;

	private TextBoxMesh statusText;

	private BorderedRectangle bgStatusMesh;

	private RenderItem foundGamesItem;

	private global::ScrollbarItem scrollbar;

	public FindGamesItem(AvatarFarmOnline.Template.Controls.FindGames findGames)
	{
		this.findGames = findGames;
		foundGamesItem = new RenderItem();
		addChild(foundGamesItem);
		Layout2D.LayoutData layoutData = new Layout2D.LayoutData(new Vector2(0f, -15f), new Vector2(Engine.GUIWidth * 0.8f, Engine.GUIHeight * 0.65f));
		Layout2D.HorizontalFixedFloatLayout(layoutData, 44f, 9f, out var fixedLayout, out var floatLayout);
		float[] coordBorder = new float[4] { 16f, 16f, 16f, 16f };
		scrollbar = new global::ScrollbarItem(fixedLayout, vertical: true);
		scrollbar.MarkerSize = 4;
		foundGamesItem.addChild(scrollbar);
		BorderedRectangle m = new BorderedRectangle(TextureManager.Textures["GUI/RoundBorderTex"], floatLayout, coordBorder)
		{
			Diffuse = Vector3.Zero,
			Alpha = 0.5f
		};
		foundGamesItem.addMesh(m);
		Layout2D.InsideBorderLayout(floatLayout, 8f, out floatLayout);
		BorderedRectangle m2 = new BorderedRectangle(TextureManager.Textures["GUI/ExtBorderMask"], floatLayout, coordBorder)
		{
			FirstMaterial = 
			{
				Textures = { (Texture)TextureManager.Textures["GUI/WoodLightTileTex"] }
			},
			Shader = ShaderManager.Shaders["GUIMask"]
		};
		foundGamesItem.addMesh(m2);
		Layout2D.InsideBorderLayout(floatLayout, 16f, out floatLayout);
		for (int i = 0; i < 4; i++)
		{
			Layout2D.GridLayout(floatLayout, 1, 4, 0f, i, out var cellLayout);
			FoundGameDetailsItem foundGameDetailsItem = new FoundGameDetailsItem(cellLayout);
			foundGames.Add(foundGameDetailsItem);
			foundGamesItem.addChild(foundGameDetailsItem);
		}
		float[] coordBorder2 = new float[4] { 16f, 0f, 16f, 0f };
		bgStatusMesh = new BorderedRectangle(TextureManager.Textures["GUI/HorizontalGradientMask"], new Vector2(layoutData.Width, layoutData.Height / 5f), coordBorder2);
		bgStatusMesh.Alpha = 0.5f;
		bgStatusMesh.Offset = layoutData.position + new Vector2(0f, 10f);
		bgStatusMesh.Diffuse = Vector3.Zero;
		statusText = new TextBoxMesh(GameTemplate.TitleFont, new TextBoxDrawProperties(layoutData, 1f, HorizontalAlignment.Center, VerticalAlignment.Center), 64, useStringBuilder: false);
		statusItem = new RenderItem(bgStatusMesh);
		statusItem.addMesh(statusText);
		addChild(statusItem);
		findGames.OnMove += challengeList_OnMove;
		findGames.OnNewResults += OnNewResults;
		findGames.OnSelected += findGames_OnSelected;
	}

	private void findGames_OnSelected(IAvailableSession obj)
	{
		GameTemplate.InteractionAudio.Start();
	}

	private void OnNewResults(AvatarFarmOnline.Template.Controls.FindGames findGames)
	{
		UpdateData();
	}

	private void challengeList_OnMove(AvatarFarmOnline.Template.Controls.FindGames obj)
	{
		GameTemplate.MoveAudio.Start();
		UpdateData();
	}

	private void UpdateData()
	{
		List<IAvailableSession> entries = findGames.GetEntries();
		scrollbar.TotalItems = findGames.ResultCount;
		scrollbar.BaseItem = findGames.BaseIndex;
		int num = 0;
		for (num = 0; num < entries.Count; num++)
		{
			foundGames[num].SetResult(entries[num], findGames.BaseIndex + num == findGames.SelectedIndex);
		}
		for (; num < foundGames.Count; num++)
		{
			foundGames[num].Hide();
		}
	}

	protected override void DoUpdate()
	{
		bool flag = false;
		switch (findGames.State)
		{
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.Finding:
			foundGamesItem.Visible = false;
			statusText.Text = "SEARCHING".Translate();
			statusItem.Visible = true;
			flag = true;
			break;
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.Found:
			foundGamesItem.Visible = true;
			statusItem.Visible = false;
			break;
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.NotFound:
			foundGamesItem.Visible = false;
			statusText.Text = "NOT_FOUND".Translate();
			statusItem.Visible = true;
			break;
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.Joining:
			foundGamesItem.Visible = false;
			statusText.Text = "JOINING_GAME".Translate();
			statusItem.Visible = true;
			flag = true;
			break;
		case AvatarFarmOnline.Template.Controls.FindGames.FindGamesState.ErrorJoining:
			foundGamesItem.Visible = false;
			statusText.Text = "ERROR_JOINING".Translate();
			statusItem.Visible = true;
			break;
		}
		if (flag && statusItem.Visible)
		{
			statusText.Alpha = 0.5f + 0.5f * Math.Abs((float)Math.Sin(Timer.DefaultTimer.TotalTimeSeconds * 3f));
		}
		else
		{
			statusText.Alpha = 1f;
		}
		bgStatusMesh.Alpha = statusText.Alpha * 0.5f;
		base.DoUpdate();
	}
}
