using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Environment;

public class SkyItem : Entity3D
{
	public enum Type
	{
		NebulaA,
		NebulaB,
		NebulaC,
		NebulaD,
		NebulaE,
		PlanetA,
		PlanetB,
		SunA,
		PlanetC
	}

	private const string EFFECT_PARAM_TIME = "time";

	private const string EFFECT_PARAM_TIMETOTAL = "timeTotal";

	private static string[] PATH_MATERIAL = new string[9] { "StaticFlat:Path=Materials/Sky/NebulaA", "StaticFlat:Path=Materials/Sky/NebulaB", "StaticFlat:Path=Materials/Sky/NebulaC", "StaticFlat:Path=Materials/Sky/NebulaD", "StaticFlat:Path=Materials/Sky/NebulaE", "Planet:Path=Materials/Sky/PlanetA:Ks=0.4:RimAmount=0.5:RimMix=0.5:RimColor=0.1|0.8|0.5:CloudColor=0.39453125|0.05078125|0.41796875|1", "Planet:Path=Materials/Sky/PlanetB:Ks=0.4:RimAmount=0.5:RimMix=0.5:RimColor=0|0.1|1:CloudColor=1|1|1|0.8", "PlanetSun:Path=Materials/Sky/SunA:RimAmount=0.3:RimMix=0.5:RimColor=0.8|0.5|0:CloudColor=1|0.5|0.1|0.8:CloudValue=1", "Planet:Path=Materials/Sky/PlanetC:Ks=0.4:RimAmount=0.3:RimMix=0.5:RimColor=0.1|0|0.5:CloudColor=0.5|0.8|1|0.8" };

	private static string[] PATH_MODEL = new string[9] { "Content/Models/Universe/SkyItem/Model", "Content/Models/Universe/SkyItem/Model", "Content/Models/Universe/SkyItem/Model", "Content/Models/Universe/SkyItem/Model", "Content/Models/Universe/SkyItem/Model", "Content/Models/Universe/Hemisphere/Model", "Content/Models/Universe/Hemisphere/Model", "Content/Models/Universe/Hemisphere/Model", "Content/Models/Universe/Hemisphere/Model" };

	private static float[] TIME = new float[9] { 0f, 0f, 0f, 0f, 0f, 30000f, 50000f, 6000f, 50000f };

	public MaxModelPart part;

	private MaxModel model;

	private EffectParameter paramTime;

	private Sky sky;

	private Type type;

	private EntityStack renderStack;

	private Material.State renderState;

	public SkyItem(Sky oSky, Type xType, Vector3 vPosition, float xScale, float xRotation, string xRenderStackName)
	{
		sky = oSky;
		type = xType;
		scene = sky.scene;
		renderStack = scene.RenderStacks_FromName(xRenderStackName);
		renderState = renderStack.renderState;
		position = vPosition;
		scale = new Vector3(xScale, xScale, xScale);
		rotation = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(Vector3.Zero, position, Vector3.Up, Vector3.Forward)));
		rotation *= Quaternion.CreateFromAxisAngle(Vector3.Forward, xRotation);
		Load();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>(PATH_MODEL[(int)type]).Clone();
		part = model.modelParts[0];
		part.materialData = PATH_MATERIAL[(int)type];
		model.Build(this);
		if (TIME[(int)type] > 0f)
		{
			paramTime = part.material.effect.Parameters["time"];
			part.material.effect.Parameters["timeTotal"].SetValue(TIME[(int)type]);
		}
		renderStack.Add(guid.value, this);
		scene.lights.SetEffect(ref part.material.effect);
		base.Load();
	}

	public override void Dispose()
	{
		renderStack.Remove(guid.value, this);
		base.Dispose();
		part.material.Dispose();
	}

	public void Update(GameTime oGameTime)
	{
		if (paramTime != null)
		{
			paramTime.SetValue((float)oGameTime.TotalGameTime.TotalMilliseconds);
		}
	}

	public override void Render(GameTime oGameTime)
	{
		if (part.material.effect != null && !part.material.effect.IsDisposed)
		{
			model.Render(scene.cameras.camera);
		}
	}

	public void RenderMap(Camera oCam)
	{
		if (!part.material.effect.IsDisposed)
		{
			Material.RenderStates_Set(renderState);
			model.Render(oCam);
		}
	}
}
