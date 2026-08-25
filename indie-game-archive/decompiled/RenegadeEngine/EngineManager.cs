using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using RenegadeEngine.Gameplay;
using RenegadeEngine.Graphics;
using RenegadeEngine.MenuSystem;

namespace RenegadeEngine;

public class EngineManager : Microsoft.Xna.Framework.Game
{
	private static GraphicsDeviceManager graphics;

	private static SpriteBatch spriteBatch;

	private static EngineState currentState = EngineState.MainMenu;

	private static List<MenuScreen> screens = new List<MenuScreen>();

	private static MenuScreen activeScreen;

	private static MenuScreen prevScreen;

	private static MenuScreen nextScreen;

	private SpriteFont font;

	private static RenegadeEngine.Gameplay.Game game;

	public static GraphicsDeviceManager GetGraphicsDeviceManager => graphics;

	public static GraphicsDevice GetGraphicsDevice => graphics.GraphicsDevice;

	public static SpriteBatch GetSpriteBatch => spriteBatch;

	public static bool GameInProgress { get; private set; }

	public static EngineState State => currentState;

	public static void ResetGameplay(PlayerIndex controllingPlayer)
	{
		game.Initialize();
		game.LoadContent();
		game.ControllingPlayer = controllingPlayer;
	}

	public static void StartGameplay(PlayerIndex controllingPlayer)
	{
		if (!GameInProgress)
		{
			GameInProgress = true;
			game.Initialize();
			game.LoadContent();
		}
		game.ScreenState = ScreenState.TransitionIn;
		game.ControllingPlayer = controllingPlayer;
	}

	public static void EndGameplay(PlayerIndex controllingPlayer)
	{
		game.Dispose();
		game.UnloadContent();
		GameInProgress = false;
	}

	public static void AddMenuScreen(PlayerIndex controllingPlayer, MenuScreen screen)
	{
		if (screen == null)
		{
			return;
		}
		screen.ControllingPlayer = controllingPlayer;
		screen.Deactivated += On_ActiveScreenDeactivated;
		screen.Disposed += On_ActiveScreenDisposed;
		screens.Add(screen);
		nextScreen = screen;
		if (activeScreen != null)
		{
			if (!activeScreen.IsExiting)
			{
				prevScreen = activeScreen;
			}
			if (screen.IsPopUp)
			{
				activeScreen.ChangeState(ScreenState.TransitionToBackground);
			}
			else
			{
				activeScreen.ChangeState(ScreenState.TransitionOut);
			}
		}
		if (game.ScreenState == ScreenState.Active)
		{
			if (screen.IsPopUp)
			{
				game.ScreenState = ScreenState.TransitionToBackground;
			}
			else
			{
				game.ScreenState = ScreenState.TransitionOut;
			}
		}
	}

	public EngineManager()
	{
		ErrorLogger.LogError("< EngineManager opening >");
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		base.Components.Add(new GamerServicesComponent(this));
	}

	~EngineManager()
	{
		ErrorLogger.LogError("< EngineManager closing >");
		ErrorLogger.PrintLog();
	}

	protected override void Initialize()
	{
		ErrorLogger.LogError("< EngineManager initializing >");
		DisplayModeCollection supportedDisplayModes = base.GraphicsDevice.Adapter.SupportedDisplayModes;
		List<DisplayMode> list = new List<DisplayMode>();
		foreach (DisplayMode item in supportedDisplayModes)
		{
			if (item.Format == SurfaceFormat.Color)
			{
				list.Add(item);
			}
		}
		Global.SetDisplayModes(list);
		Global.VSync = true;
		Global.FullScreen = true;
		Global.BloomEffect = true;
		if (base.GraphicsDevice.DisplayMode.Height > 480)
		{
			Global.SetScreenDimensions(base.GraphicsDevice.DisplayMode.Width, base.GraphicsDevice.DisplayMode.Height);
		}
		graphics.PreferMultiSampling = true;
		graphics.SynchronizeWithVerticalRetrace = Global.VSync;
		graphics.PreferredBackBufferWidth = Global.ScreenWidth;
		graphics.PreferredBackBufferHeight = Global.ScreenHeight;
		graphics.IsFullScreen = Global.FullScreen;
		graphics.ApplyChanges();
		base.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
		base.Initialize();
	}

	protected override void LoadContent()
	{
		ErrorLogger.LogError("< Loading Content >");
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		AssetManager.LoadContent(base.Content);
		BloomEffect.Initialize(base.GraphicsDevice);
		SoundMgr.Initialize();
		screens.Add(new MainMenuScreen());
		activeScreen = screens[0];
		activeScreen.Deactivated += On_ActiveScreenDeactivated;
		activeScreen.Disposed += On_ActiveScreenDisposed;
		game = new RenegadeEngine.Gameplay.Game();
		game.Deactivated += On_GameplayScreenDeactivated;
		game.Disposed += On_GameplayScreenDisposed;
		AssetManager.GetAsset(FontKeys.MenuFont, ref font);
		ErrorLogger.PrintLog();
		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		ErrorLogger.LogError("< Unloading Content >");
		base.Content.Unload();
		Vibration.StopAllVibrations();
		SoundMgr.Dispose();
	}

	protected override void Update(GameTime gameTime)
	{
		if (currentState == EngineState.Exiting)
		{
			Exit();
		}
		if (!Guide.IsVisible)
		{
			Input.Begin();
			if (currentState == EngineState.MainMenu)
			{
				if (activeScreen.ScreenState == ScreenState.Active)
				{
					activeScreen.UpdateInput(gameTime);
				}
				activeScreen.Update(gameTime);
			}
			else if (currentState == EngineState.Gameplay)
			{
				game.Update(gameTime);
			}
			Input.End();
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Transparent);
		if (currentState == EngineState.Gameplay)
		{
			game.Draw(gameTime);
		}
		else
		{
			if (game.ScreenState == ScreenState.Inactive)
			{
				game.Draw(gameTime);
			}
			if (screens.Count > 0)
			{
				for (int num = screens.Count - 1; num >= 0; num--)
				{
					if (screens[num].ScreenState == ScreenState.Inactive)
					{
						screens[num].Draw(gameTime);
					}
				}
				activeScreen.Draw(gameTime);
			}
		}
		base.Draw(gameTime);
	}

	private static void On_ActiveScreenDeactivated(object sender, EventArgs e)
	{
		if (nextScreen != null)
		{
			activeScreen = nextScreen;
			nextScreen = null;
		}
	}

	private static void On_ActiveScreenDisposed(object sender, EventArgs e)
	{
		MenuScreen menuScreen = activeScreen;
		if (prevScreen != null)
		{
			prevScreen.ControllingPlayer = activeScreen.ControllingPlayer;
			activeScreen = prevScreen;
			prevScreen = null;
			activeScreen.ChangeState(ScreenState.TransitionIn);
		}
		screens.Remove(menuScreen);
		if (screens.Count == 0)
		{
			if (GameInProgress)
			{
				prevScreen = null;
				activeScreen = null;
				nextScreen = null;
				currentState = EngineState.Gameplay;
				StartGameplay(menuScreen.ControllingPlayer);
			}
			else
			{
				currentState = EngineState.Exiting;
			}
		}
	}

	private static void On_GameplayScreenDeactivated(object sender, EventArgs e)
	{
		currentState = EngineState.MainMenu;
		if (nextScreen != null)
		{
			activeScreen = nextScreen;
			nextScreen = null;
		}
	}

	private static void On_GameplayScreenDisposed(object sender, EventArgs e)
	{
		game = new RenegadeEngine.Gameplay.Game();
		GameInProgress = false;
		currentState = EngineState.MainMenu;
	}
}
