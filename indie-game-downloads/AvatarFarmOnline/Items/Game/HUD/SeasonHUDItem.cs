using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class SeasonHUDItem : RenderItem
{
	private const float RADIUS = 105f;

	private AvatarFarmOnline.Items.Game.HUD.ProgressCircle seasonCircle;

	private Sized2DRectangleMesh seasonIcon;

	private Sized2DRectangleMesh circleIcon;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private bool lastOnShop;

	public SeasonHUDItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		Transform.Translation = new Vector3(0f, Engine.GUIHeight * 0.45f - 52.5f, 0f);
		Layout2D.LayoutData result = new Layout2D.LayoutData(new Vector2(105f));
		seasonCircle = new AvatarFarmOnline.Items.Game.HUD.ProgressCircle(result, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(AvatarFarmOnline.Logic.Seasons.Summer));
		addMesh(seasonCircle);
		Layout2D.InsideBorderLayout(result, 24f, out result);
		seasonIcon = new Sized2DRectangleMesh(result, TextureManager.Textures["HUD/Seasons"]);
		seasonIcon.SetTile(0, 2, 2);
		seasonIcon.Shader = ShaderManager.Shaders["Mask"];
		seasonIcon.FirstMaterial.SetTexture(1, TextureManager.Textures["HUD/SeasonMask"]);
		addMesh(seasonIcon);
		Layout2D.OutsideBorderLayout(result, 14f, out result);
		circleIcon = new Sized2DRectangleMesh(result, TextureManager.Textures["HUD/HUDCircle"]);
		addMesh(circleIcon);
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
			seasonCircle.SetProgress(stage.FarmData.SeasonProgress);
			seasonCircle.SetBGColor(AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason) * 0.7f);
			seasonCircle.SetFGColor(AvatarFarmOnline.Logic.GameGlobals.SeasonColor(AvatarFarmOnline.Logic.Parsing.NextSeason(stage.FarmData.CurrentSeason)));
			switch (stage.FarmData.CurrentSeason)
			{
			case AvatarFarmOnline.Logic.Seasons.Winter:
				seasonIcon.SetTile(0, 2, 2);
				break;
			case AvatarFarmOnline.Logic.Seasons.Spring:
				seasonIcon.SetTile(1, 2, 2);
				break;
			case AvatarFarmOnline.Logic.Seasons.Summer:
				seasonIcon.SetTile(2, 2, 2);
				break;
			case AvatarFarmOnline.Logic.Seasons.Fall:
				seasonIcon.SetTile(3, 2, 2);
				break;
			}
			bool isOnShop = stage.IsOnShop;
			if (lastOnShop != isOnShop)
			{
				lastOnShop = isOnShop;
				if (!isOnShop)
				{
					Transform.Translation = new Vector3(0f, Engine.GUIHeight * 0.45f - 52.5f, 0f);
				}
				else
				{
					Transform.Translation = new Vector3((0f - Engine.GUIWidth) * 0.45f + 52.5f, Engine.GUIHeight * 0.45f - 140f - 52.5f, 0f);
				}
			}
		}
		base.DoUpdate();
	}
}
