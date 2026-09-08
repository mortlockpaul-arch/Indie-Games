using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Elements;
using Quasar.Items;
using Quasar.Scenes;
using Quasar.Shaders;

namespace AvatarFarmOnline.Scenes;

internal class ANFinalScene : PostprocessScene
{
	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private Camera gameCamera;

	private Postprocess vignette;

	public Vector4 Color
	{
		set
		{
			PostProcess.Material.DiffuseWithAlpha = value;
		}
	}

	public ANFinalScene(Camera gameCamera, Texture2D texture, AvatarFarmOnline.Logic.Stage.Stage stage)
		: base(texture, ShaderManager.Shaders["PassThrough"])
	{
		this.stage = stage;
		this.gameCamera = gameCamera;
		PostProcess.Material.AddVector4Parameter(Vector4.Zero);
		PostProcess.Material.AddFloatParameter(0f);
		PostProcess.Material.AddFloatParameter(0f);
		vignette = new Postprocess(null, ShaderManager.Shaders["Vignette"]);
		vignette.Material.AddFloatParameter(0f);
	}

	public ANFinalScene(Camera gameCamera, Texture2D texture, AvatarFarmOnline.Logic.Seasons season)
		: this(gameCamera, texture, null)
	{
		PostProcess.Diffuse = AvatarFarmOnline.Logic.GameGlobals.SeasonColor(season);
	}

	public override void Update()
	{
		if (stage != null)
		{
			PostProcess.Diffuse = Vector3.Lerp(PostProcess.Diffuse, AvatarFarmOnline.Logic.GameGlobals.SeasonColor(stage.FarmData.CurrentSeason), 0.01f);
		}
		base.Update();
	}
}
