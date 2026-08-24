using System;
using System.Collections.Generic;
using GKEngine;
using GKEngine.Utils;
using Game.Atoms;
using Game.Audio;
using Game.Data;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Game;

public class GameMain : GameEngine
{
	public const int SCREEN_LIMIT_UPPER_X = 1920;

	public const int SCREEN_LIMIT_UPPER_Y = 1080;

	public const int SCREEN_LIMIT_LOWER_X = 1024;

	public const int SCREEN_LIMIT_LOWER_Y = 720;

	public new static GameMain instance;

	public static string RENDERSTACK_SOLID = "Solid";

	public static string RENDERSTACK_ALPHA_HARD = "Alpha Hard";

	public static string RENDERSTACK_ADD_FIRST = "Add First";

	public static string RENDERSTACK_ALPHA_SORTED = "Alpha Sorted";

	public static string RENDERSTACK_ALPHA_UNSORTED = "Alpha Unsorted";

	public static string RENDERSTACK_MANUAL = "Manual";

	public static string RENDERSTACK_ADD = "Add";

	public static string RENDERSTACK_ALPHA_LAST = "Alpha Last";

	public static string RENDERSTACK_UI = "UI";

	public static string RENDERSTACK_DIALOGS = "Dialogs";

	public static string RENDERSTACK_GAMMA = "Gamma";

	public static int REGISTER_SHADOW = 4;

	public static int REGISTER_DISTORT = 5;

	public BuildScene sceneBuild;

	public PlayScene scenePlay;

	public MenuScene sceneMenu;

	public StoryScene sceneStory;

	public IntroScene sceneIntro;

	public GamerServicesComponent gamerServices;

	public GameMain()
		: base(new Point(1280, 720))
	{
		instance = this;
		gamerServices = new GamerServicesComponent(instance);
		instance.Components.Add(gamerServices);
		SignedInGamer.SignedIn += Event_SignedInGamer_SignedIn;
		SignedInGamer.SignedOut += Event_SignedInGamer_SignedOut;
		base.IsFixedTimeStep = true;
		GameEngine.Graphics.SynchronizeWithVerticalRetrace = true;
	}

	public void Init()
	{
		MathUtils.Init();
		sceneBuild = new BuildScene();
		Scene_Add(sceneBuild);
		scenePlay = new PlayScene();
		Scene_Add(scenePlay);
		sceneMenu = new MenuScene();
		Scene_Add(sceneMenu);
		sceneStory = new StoryScene();
		Scene_Add(sceneStory);
		sceneIntro = new IntroScene();
		Scene_Add(sceneIntro);
		Run();
	}

	protected override void LoadLoadingScreen()
	{
		defaultLoading = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Loading");
	}

	protected override void Initialize()
	{
		base.Initialize();
		gamerServices.Update(new GameTime(new TimeSpan(1L), new TimeSpan(1L)));
		Initialize_Video();
		AtomCatalog.Init();
		AudioManager.Initialise("Content/Audio");
		DataManager.Load(delegate
		{
			sceneIntro.errors = false;
			Scene_Set(sceneIntro);
			GameEngine.scene.Load();
		}, delegate
		{
			sceneIntro.errors = true;
			Scene_Set(sceneIntro);
			GameEngine.scene.Load();
		});
	}

	private void Initialize_Video()
	{
		base.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
		float aspectRatio = base.GraphicsDevice.DisplayMode.AspectRatio;
		int num = base.GraphicsDevice.DisplayMode.Width;
		int num2 = base.GraphicsDevice.DisplayMode.Height;
		if (num > 1920)
		{
			num = 1920;
			num2 = (int)(1920f / aspectRatio);
		}
		if (num2 > 1080)
		{
			num2 = 1080;
			num = (int)(1080f * aspectRatio);
		}
		if (num < 1024)
		{
			num = 1024;
			num2 = (int)(1024f / aspectRatio);
		}
		if (num2 < 720)
		{
			num2 = 720;
			num = (int)(720f * aspectRatio);
		}
		Video_SetRes(new Point(num, num2));
	}

	protected override void Update(GameTime oGameTime)
	{
		if (!Guide.IsVisible)
		{
			base.Update(oGameTime);
		}
		else
		{
			gamerServices.Update(oGameTime);
		}
		DataManager.Update(oGameTime);
	}

	public override void Input_Set()
	{
		base.Input_Set();
	}

	public override void Input_Update(GameTime oGameTime)
	{
		base.Input_Update(oGameTime);
	}

	protected override void EndRun()
	{
		AudioManager.Unload();
		base.EndRun();
	}

	protected override void Event_SceneLoadError(Exception error)
	{
		Guide.BeginShowMessageBox("Hmm, looks like we have a problem", "Alien Jelly is having a tough time loading something. Head over to http://www.facebook.com/alienjelly and we will be happy to help you out. The game will now close.", new List<string> { "Okay" }, 0, MessageBoxIcon.Alert, delegate(IAsyncResult result)
		{
			Guide.EndShowMessageBox(result);
			Exit();
		}, null);
	}

	private void Event_SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
	{
		if (GameEngine.scene != null)
		{
			GameEngine.scene.Event_SignedInGamer_SignedIn(sender, e);
		}
	}

	private void Event_SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
	{
		if (GameEngine.scene != null)
		{
			GameEngine.scene.Event_SignedInGamer_SignedOut(sender, e);
		}
	}
}
