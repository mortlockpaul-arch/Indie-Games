using System;
using AvatarFarmOnline.Logic;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Elements;
using Quasar.Elements.Cameras;
using Quasar.GameUtils.Tasks;
using Quasar.Global;
using Quasar.Xml;

namespace AvatarFarmOnline.Scenes;

internal class BGScene : Scene
{
	private static AvatarFarmOnline.Scenes.BGScene instance;

	private AvatarFarmOnline.Logic.Seasons season;

	private LookAtCamera camera;

	private bool isLoaded;

	private Element loadedScene;

	public static AvatarFarmOnline.Scenes.BGScene Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AvatarFarmOnline.Scenes.BGScene();
			}
			return instance;
		}
	}

	public AvatarFarmOnline.Logic.Seasons Season => season;

	public bool IsLoaded => isLoaded;

	private void OnDispose()
	{
		Dispose();
		instance = null;
	}

	private BGScene()
	{
		Engine.RegisterDisposeHandler(OnDispose);
		season = (AvatarFarmOnline.Logic.Seasons)(1 << GameMath.Random.Next(4));
		Vector3 vector = AvatarFarmOnline.Logic.GameGlobals.SeasonColor(season);
		Light light = new Light(vector);
		light.Attenuation = new Vector3(1f, 0f, 0f);
		light.Transform.Translation = new Vector3(20f, 40f, -20f);
		light.Specular = vector;
		light.Diffuse = Vector3.Lerp(Vector3.One, vector, 0.6f) * 0.75f;
		light.Ambient = Vector3.Lerp(Vector3.One, vector, 0.9f) * 0.65f;
		addLight(light);
		camera = new LookAtCamera(Vector3.UnitX, new Vector3(0f, 1f, 0f));
		Camera = camera;
		Add(Camera);
		try
		{
			TaskManager.Post(LoadScene, null, LoadFinished);
		}
		catch (Exception)
		{
		}
	}

	private void LoadScene(object parameters)
	{
		loadedScene = ElementLoader.Load("Scenes/BGScene");
	}

	private void LoadFinished(object parameters)
	{
		Add(loadedScene);
		isLoaded = true;
	}

	public override void Update()
	{
		camera.Transform.Translation = new Vector3(2f * (float)Math.Sin(Timer.DefaultTimer.TotalTimeSeconds * 0.025f), 1.2f, 2f * (float)Math.Cos(Timer.DefaultTimer.TotalTimeSeconds * 0.025f));
		base.Update();
	}
}
