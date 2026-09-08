using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class PlayerHUDItem : RenderItem
{
	public const float RADIUS = 140f;

	private AvatarFarmOnline.Items.Game.HUD.ProgressCircle xpCircle;

	private Sized2DRectangleMesh playerIcon;

	private Sized2DRectangleMesh circleIcon;

	private TextMesh levelText;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	public PlayerHUDItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		Layout2D.LayoutData result = new Layout2D.LayoutData(new Vector2((0f - Engine.GUIWidth) * 0.45f + 70f, Engine.GUIHeight * 0.45f - 70f), new Vector2(140f));
		xpCircle = new AvatarFarmOnline.Items.Game.HUD.ProgressCircle(result, GameMath.RGBToVector(254, 175, 25));
		xpCircle.SetBGColor(GameMath.RGBToVector(5, 65, 93));
		addMesh(xpCircle);
		Layout2D.InsideBorderLayout(result, 27f, out result);
		playerIcon = new Sized2DRectangleMesh(result, stage.LocalPlayer.Selection.Texture);
		playerIcon.Shader = ShaderManager.Shaders["Mask"];
		playerIcon.FirstMaterial.SetTexture(1, TextureManager.Textures["HUD/CircleMask"]);
		addMesh(playerIcon);
		Layout2D.OutsideBorderLayout(result, 17f, out result);
		circleIcon = new Sized2DRectangleMesh(result, TextureManager.Textures["HUD/HUDCircle"]);
		addMesh(circleIcon);
		levelText = new TextMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDTitleFont, new TextDrawProperties(result.BottomRight + new Vector2((0f - result.size.X) * 0.2f, result.size.Y * 0.3f), HorizontalAlignment.Right), 5, useStringBuilder: true);
		levelText.Diffuse = GameMath.RGBToVector(140, 187, 220);
		addMesh(levelText);
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
			levelText.StringBuilder.AppendNumber(stage.LocalPlayer.Level);
			int level = stage.LocalPlayer.Level;
			if (level >= 100)
			{
				xpCircle.SetProgress(0f);
			}
			else
			{
				int xp = stage.LocalPlayer.PlayerSelection.PlayerExperience.Xp;
				int num = AvatarFarmOnline.Logic.GameGlobals.PlayerLevelUpXp(level - 1);
				int num2 = AvatarFarmOnline.Logic.GameGlobals.PlayerLevelUpXp(level);
				float progress = (float)(xp - num) / (float)(num2 - num);
				xpCircle.SetProgress(progress);
			}
		}
		base.DoUpdate();
	}
}
