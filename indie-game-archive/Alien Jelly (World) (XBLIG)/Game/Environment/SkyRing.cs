using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Environment;

public class SkyRing : Entity3D
{
	public enum Type
	{
		Clouds
	}

	private const string PATH_MODEL = "Content/Models/Universe/SkyRing/Model";

	private const string EFFECT_PARAM_TILE = "tile";

	private const string EFFECT_PARAM_TINT = "tint";

	private const string EFFECT_PARAM_INTENSITY = "intensity";

	private const string EFFECT_PARAM_TIME = "time";

	private const string EFFECT_PARAM_TIMETOTAL = "timeTotal";

	private const string EFFECT_PARAM_UVSTACK = "uvStack";

	private static string[] PATH_MATERIAL = new string[9] { "SkyRing:Path=Materials/Sky/RingA", "SkyRing:Path=Materials/Sky/RingB", "SkyRing:Path=Materials/Sky/RingA", "SkyRing:Path=Materials/Sky/RingC", "SkyRing:Path=Materials/Sky/RingA", "SkyRing:Path=Materials/Sky/RingA", "SkyRingDSN:Path=Materials/Sky/RingSpaceJunk:RimAmount=0.5:RimMix=0.3:Ks=2:SpecExpon=55", "SkyRing:Path=Materials/Sky/RingC", "SkyRingDSN:Path=Materials/Sky/RingAsteroids:Bump=1:RimAmount=0:RimMix=0:Ks=0.5:SpecExpon=55" };

	private static Vector2[] TILE = new Vector2[9]
	{
		new Vector2(8f, 1f),
		new Vector2(9f, 2f),
		new Vector2(8f, 1f),
		new Vector2(8f, 1f),
		new Vector2(4f, 1f),
		new Vector2(8f, 1f),
		new Vector2(14f, 1f),
		new Vector2(8f, 1f),
		new Vector2(8f, 1f)
	};

	private static bool[] LIGHTING = new bool[9] { false, false, false, false, false, false, true, false, true };

	private static Color[] TINT = new Color[9]
	{
		new Color(255, 0, 255, 255),
		new Color(255, 234, 213, 255),
		new Color(135, 206, 63, 255),
		new Color(97, 5, 83, 255),
		new Color(27, 134, 217, 255),
		new Color(86, 76, 48, 255),
		new Color(0, 0, 0, 0),
		new Color(255, 0, 90, 255),
		new Color(0, 0, 0, 0)
	};

	private static float[] INTENSITY = new float[9] { 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1.5f, 1f };

	private static float[] TIME = new float[9] { 30000f, 0f, 15000f, 10000f, 30000f, 30000f, 0f, 10000f, 0f };

	private static float[] UVSTACK = new float[9] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f };

	public MaxModelPart part;

	private MaxModel model;

	private EffectParameter paramTime;

	private Sky sky;

	private Type type;

	private EntityStack renderStack;

	private Material.State renderState;

	private float speed;

	public SkyRing(Sky oSky, Type xType, Vector3 vAxis, Vector3 vPosition, float xRotation, float xRadius, float xHeight, float xSpeed, string xRenderStackName)
	{
		sky = oSky;
		type = xType;
		speed = xSpeed;
		scene = sky.scene;
		renderStack = scene.RenderStacks_FromName(xRenderStackName);
		renderState = renderStack.renderState;
		position = vPosition;
		scale = new Vector3(xRadius, xHeight, xRadius);
		rotation = Quaternion.CreateFromAxisAngle(vAxis, xRotation);
		Load();
	}

	public override void Load()
	{
		model = GameEngine.SceneContent.Load<MaxModel>("Content/Models/Universe/SkyRing/Model").Clone();
		part = model.modelParts[0];
		part.materialData = PATH_MATERIAL[(int)type];
		model.Build(this);
		paramTime = part.material.effect.Parameters["time"];
		part.material.effect.Parameters["tile"].SetValue(TILE[(int)type]);
		part.material.effect.Parameters["tint"].SetValue(TINT[(int)type].ToVector4());
		part.material.effect.Parameters["intensity"].SetValue(INTENSITY[(int)type]);
		part.material.effect.Parameters["timeTotal"].SetValue(TIME[(int)type]);
		part.material.effect.Parameters["uvStack"].SetValue(UVSTACK[(int)type]);
		if (LIGHTING[(int)type])
		{
			scene.lights.SetEffect(ref part.material.effect);
		}
		renderStack.Add(guid.value, this);
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
		paramTime.SetValue((float)oGameTime.TotalGameTime.TotalMilliseconds);
		if (speed != 0f)
		{
			rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, (float)oGameTime.ElapsedGameTime.Milliseconds * speed);
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
		_ = GameEngine.Graphics.GraphicsDevice;
		if (!part.material.effect.IsDisposed)
		{
			Material.RenderStates_Set(renderState);
			model.Render(oCam);
		}
	}
}
