using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

public class Level
{
	public ContentManager content;

	public Texture Height;

	public SceneInterface sceneInterface;

	public Scene scene;

	public SceneEnvironment env;

	public CustomCamera mCamera;

	public CustomCamera firstperson;

	private Random random = new Random();

	public SceneState sceneState;

	public Model skySphere;

	public SceneObject sky;

	public SceneObject WaterPlane;

	public Plane reflectionplane;

	public Texture2D refltext;

	private SynapseGaming.LightingSystem.Lights.DirectionalLight Dirlight;

	private AmbientLight ambientlight;

	private int levelnumber;

	public float totalTime;

	public Level(ContentManager cm, SceneInterface si, SceneState SS, int ln)
	{
		levelnumber = ln;
		sceneInterface = si;
		sceneState = SS;
		content = cm;
	}

	public void LoadContent()
	{
		scene = content.Load<Scene>("Scenes/scene" + levelnumber + "/Scene1");
		env = content.Load<SceneEnvironment>("Scenes/scene" + levelnumber + "/Environment");
		skySphere = content.Load<Model>("Scenes/scene" + levelnumber + "/SkySphere");
		sky = new SceneObject(skySphere);
		sky.Visibility = ObjectVisibility.Rendered;
		mCamera = new CustomCamera(env.VisibleDistance, 1.7f);
		firstperson = new CustomCamera(env.VisibleDistance, 1.7f);
	}

	public void initialize()
	{
		sceneInterface.ObjectManager.Submit(sky);
		sceneInterface.LightManager.Find<SynapseGaming.LightingSystem.Lights.DirectionalLight>("Sun", onlysearchdynamicobjects: false, out Dirlight);
		sceneInterface.LightManager.Find<AmbientLight>("AmbientLighting", onlysearchdynamicobjects: false, out ambientlight);
	}

	public void UnloadContent()
	{
		content.Unload();
	}

	public void Update(GameTime gameTime)
	{
		sky.World = Matrix.CreateScale(10f) * Matrix.CreateTranslation(new Vector3(sceneState.ViewToWorld.Translation.X, -45f, sceneState.ViewToWorld.Translation.Z));
		totalTime += (float)gameTime.ElapsedGameTime.Milliseconds * 0.00025f;
		if (WaterPlane != null)
		{
			WaterPlane.World = Matrix.CreateScale(new Vector3(25f, 1f, 25f)) * Matrix.CreateTranslation(new Vector3(0f, 1f, 0f));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["LightDirection"].SetValue(new Vector3(0f - Dirlight.Direction.X, Dirlight.Direction.Y, 0f - Dirlight.Direction.Z));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["EyePosition"].SetValue(sceneState.ViewToWorld.Translation);
			WaterPlane.RenderableMeshes[0].Effect.Parameters["AmbientColor"].SetValue(new Vector4(ambientlight.DiffuseColor.X, ambientlight.DiffuseColor.Y, ambientlight.DiffuseColor.Z, 1f));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["SpecularColor"].SetValue(new Vector4(Dirlight.DiffuseColor.X, Dirlight.DiffuseColor.Y, Dirlight.DiffuseColor.Z, 1f));
			WaterPlane.RenderableMeshes[0].Effect.Parameters["time"].SetValue(totalTime);
		}
		mCamera.update(gameTime);
		firstperson.update(gameTime);
	}
}
