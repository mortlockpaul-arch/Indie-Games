using System;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Meshes;
using Quasar.GameUtils.Network;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class PlayerListItem : RenderItem
{
	private class PlayerLine : RenderItem
	{
		private Sized2DRectangleMesh pic;

		private TextBoxMesh name;

		private TextMesh level;

		public PlayerLine(Layout2D.LayoutData layout)
		{
			Layout2D.HorizontalFixedFloatLayout(layout, layout.size.Y, 8f, out var fixedLayout, out var floatLayout);
			Layout2D.LayoutData layoutData = floatLayout;
			layoutData.size.Y = 44f;
			Layout2D.InsideHorizontalBorderLayout(floatLayout, 22f, out floatLayout);
			BorderedRectangle m = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], layoutData, new float[4] { 22f, 22f, 22f, 22f });
			addMesh(m);
			pic = new Sized2DRectangleMesh(fixedLayout, null);
			pic.Shader = ShaderManager.Shaders["Mask"];
			pic.FirstMaterial.SetTexture(1, TextureManager.Textures["HUD/CircleMask"]);
			addMesh(pic);
			Layout2D.OutsideBorderLayout(fixedLayout, 10f, out fixedLayout);
			Sized2DRectangleMesh m2 = new Sized2DRectangleMesh(fixedLayout, TextureManager.Textures["HUD/HUDCircle"]);
			addMesh(m2);
			floatLayout.position += new Vector2(0f, -2f);
			name = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(floatLayout, 0.8f, HorizontalAlignment.Left, VerticalAlignment.Center), 32, useStringBuilder: false);
			addMesh(name);
			level = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDTitleFont, new TextDrawProperties(fixedLayout.BottomRight + new Vector2(-4f, 16f), 0.6f, HorizontalAlignment.Right), 8, useStringBuilder: true);
			addMesh(level);
			Visible = false;
		}

		public void Hide()
		{
			Visible = false;
		}

		public void SetData(AvatarFarmOnline.Logic.Stage.Player player, bool isHost)
		{
			pic.Texture = player.Selection.Texture;
			name.Text = player.Selection.PlayerName;
			name.Diffuse = (isHost ? new Vector3(1f, 1f, 0f) : Vector3.One);
			level.StringBuilder.Length = 0;
			level.StringBuilder.AppendNumber(player.Level);
			Visible = true;
		}
	}

	private const int PLAYER_COUNT = 16;

	private const float BAR_BORDER = 22f;

	private PlayerLine[] lines;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	public PlayerListItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		lines = new PlayerLine[16];
		this.stage = stage;
		Layout2D.LayoutData result = new Layout2D.LayoutData(Engine.GUISize * 0.7f);
		HUDRectangle m = new HUDRectangle(result);
		addMesh(m);
		Layout2D.InsideBorderLayout(result, 25f, out result);
		Layout2D.InsideHorizontalBorderLayout(result, 5f, out result);
		for (int i = 0; i < 16; i++)
		{
			Layout2D.GridLayout(result, 2, 8, 10f, i, out var cellLayout);
			lines[i] = new PlayerLine(cellLayout);
			addChild(lines[i]);
		}
	}

	protected override void DoUpdate()
	{
		Visible = stage.LocalPlayer.InputGroup.BackState();
		if (Visible && GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData)
		{
			int num = Math.Min(16, farmPersistentGameData.Online.PlayerNumber);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				INetworkGamer networkGamer = farmPersistentGameData.Online.GetNetworkGamer(i);
				AvatarFarmOnline.Logic.Stage.Player playerFor = farmPersistentGameData.Online.GetPlayerFor(networkGamer.Id);
				if (playerFor != null)
				{
					lines[num2++].SetData(playerFor, networkGamer.IsHost);
				}
			}
			for (int j = num2; j < 16; j++)
			{
				lines[num2++].Hide();
			}
		}
		base.DoUpdate();
	}
}
