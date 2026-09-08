using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class FarmHUDItem : RenderItem
{
	private const float DIAMETER = 140f;

	private const int BAR_HEIGHT = 44;

	private const float BAR_BORDER = 22f;

	private AvatarFarmOnline.Items.Game.HUD.ProgressCircle xpCircle;

	private Sized2DRectangleMesh farmIcon;

	private Sized2DRectangleMesh circleIcon;

	private TextMesh levelText;

	private TextMesh coinsText;

	private TextMesh cashText;

	private BorderedRectangle coinsBg;

	private BorderedRectangle cashBg;

	private Vector2 coinsBgPosition;

	private Vector2 cashBgPosition;

	private Vector2 coinsBgShopPosition;

	private Vector2 cashBgShopPosition;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private bool lastOnShop;

	public FarmHUDItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		Layout2D.LayoutData result = new Layout2D.LayoutData(new Vector2(Engine.GUIWidth * 0.45f - 70f, Engine.GUIHeight * 0.45f - 70f), new Vector2(140f));
		xpCircle = new AvatarFarmOnline.Items.Game.HUD.ProgressCircle(result, GameMath.RGBToVector(254, 175, 25));
		xpCircle.SetBGColor(GameMath.RGBToVector(5, 65, 93));
		addMesh(xpCircle);
		Layout2D.InsideBorderLayout(result, 27f, out result);
		farmIcon = new Sized2DRectangleMesh(result, TextureManager.Textures["HUD/FarmIcon"]);
		farmIcon.Shader = ShaderManager.Shaders["Mask"];
		farmIcon.FirstMaterial.SetTexture(1, TextureManager.Textures["HUD/CircleMask"]);
		addMesh(farmIcon);
		Layout2D.OutsideBorderLayout(result, 17f, out result);
		circleIcon = new Sized2DRectangleMesh(result, TextureManager.Textures["HUD/HUDCircle"]);
		addMesh(circleIcon);
		levelText = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDTitleFont, new TextDrawProperties(result.BottomRight + new Vector2((0f - result.size.X) * 0.2f, result.size.Y * 0.3f), HorizontalAlignment.Right), 5, useStringBuilder: true);
		levelText.Diffuse = GameMath.RGBToVector(byte.MaxValue, 227, 201);
		addMesh(levelText);
		coinsBgPosition = new Vector2(result.position.X - 70f + 5f, Engine.GUIHeight * 0.45f - 28f);
		Layout2D.LayoutData layoutData = new Layout2D.LayoutData(coinsBgPosition - new Vector2(60f, 0f), new Vector2(120f, 44f));
		coinsBg = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], layoutData, new float[4] { 22f, 22f, 22f, 22f });
		addMesh(coinsBg);
		cashBgPosition = coinsBgPosition - new Vector2(10f, 44f);
		Layout2D.LayoutData layoutData2 = new Layout2D.LayoutData(cashBgPosition - new Vector2(60f, 0f), new Vector2(120f, 44f));
		cashBg = new BorderedRectangle(TextureManager.Textures["HUD/HUDBG"], layoutData2, new float[4] { 22f, 22f, 22f, 22f });
		addMesh(cashBg);
		coinsText = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont, new TextDrawProperties(layoutData.Right + new Vector2(-20f, 5f), HorizontalAlignment.Right), 30, useStringBuilder: true);
		addMesh(coinsText);
		cashText = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont, new TextDrawProperties(layoutData2.Right + new Vector2(-20f, 5f), HorizontalAlignment.Right), 30, useStringBuilder: true);
		addMesh(cashText);
		coinsBgShopPosition = new Vector2(Engine.GUIWidth * 0.45f, result.position.Y - 70f - 22f);
		cashBgShopPosition = coinsBgShopPosition - new Vector2(0f, 44f);
		stage.FarmData.PlayerData.OnCashChange += PlayerData_OnCashChange;
		stage.FarmData.PlayerData.OnCoinsChange += PlayerData_OnCoinsChange;
		PlayerData_OnCashChange(stage.FarmData.PlayerData);
		PlayerData_OnCoinsChange(stage.FarmData.PlayerData);
	}

	private void PlayerData_OnCoinsChange(AvatarFarmOnline.Logic.Stage.PlayerData obj)
	{
		coinsText.StringBuilder.Length = 0;
		coinsText.StringBuilder.AppendNumber(obj.Coins, AppendNumberOptions.NumberGroup);
		coinsText.StringBuilder.Append(' ');
		coinsText.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CoinChar);
		int num = GameTemplate.StandardFont.MeasureString(coinsText.StringBuilder) + 44;
		Vector2 vector = (lastOnShop ? coinsBgShopPosition : coinsBgPosition);
		Layout2D.LayoutData layout = new Layout2D.LayoutData(vector - new Vector2((float)num * 0.5f, 0f), new Vector2(num, 44f));
		coinsText.Offset = vector + new Vector2(-20f, 5f);
		coinsBg.Layout = layout;
	}

	private void PlayerData_OnCashChange(AvatarFarmOnline.Logic.Stage.PlayerData obj)
	{
		cashText.StringBuilder.Length = 0;
		cashText.StringBuilder.AppendNumber(obj.Cash, AppendNumberOptions.NumberGroup);
		cashText.StringBuilder.Append(' ');
		cashText.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CashChar);
		int num = GameTemplate.StandardFont.MeasureString(cashText.StringBuilder) + 44;
		Vector2 vector = (lastOnShop ? cashBgShopPosition : cashBgPosition);
		Layout2D.LayoutData layout = new Layout2D.LayoutData(vector - new Vector2((float)num * 0.5f, 0f), new Vector2(num, 44f));
		cashText.Offset = vector + new Vector2(-20f, 5f);
		cashBg.Layout = layout;
	}

	protected override void DoUpdate()
	{
		if (stage.LocalPlayer.CameraState == AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World)
		{
			Visible = false;
		}
		else
		{
			Visible = true;
			levelText.StringBuilder.Length = 0;
			int level = stage.FarmData.PlayerData.Level;
			levelText.StringBuilder.AppendNumber(level);
			if (level >= 100)
			{
				xpCircle.SetProgress(0f);
			}
			else
			{
				int xp = stage.FarmData.PlayerData.Xp;
				int num = AvatarFarmOnline.Logic.GameGlobals.FarmLevelUpXp(level - 1);
				int num2 = AvatarFarmOnline.Logic.GameGlobals.FarmLevelUpXp(level);
				float progress = (float)(xp - num) / (float)(num2 - num);
				xpCircle.SetProgress(progress);
			}
		}
		bool isOnShop = stage.IsOnShop;
		if (isOnShop != lastOnShop)
		{
			lastOnShop = isOnShop;
			PlayerData_OnCashChange(stage.FarmData.PlayerData);
			PlayerData_OnCoinsChange(stage.FarmData.PlayerData);
		}
		base.DoUpdate();
	}
}
