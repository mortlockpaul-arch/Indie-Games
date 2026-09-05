using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using MusicPlayer;
using Renderer;
using Screens;

namespace BabyMakerExtreme2;

public class Game1 : Game
{
	private GraphicsDeviceManager graphics;

	private TimeTracker m_updateTime;

	private TimeTracker m_drawTime;

	private static bool m_bExit;

	private static bool isTrial = true;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		m_bExit = false;
		base.Components.Add(new GamerServicesComponent(this));
		if (GraphicsAdapter.DefaultAdapter.IsWideScreen)
		{
			graphics.PreferredBackBufferHeight = 720;
			graphics.PreferredBackBufferWidth = 1280;
		}
		else
		{
			graphics.PreferredBackBufferHeight = 768;
			graphics.PreferredBackBufferWidth = 1024;
		}
		graphics.SynchronizeWithVerticalRetrace = true;
		base.IsFixedTimeStep = false;
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		m_updateTime = new TimeTracker();
		m_drawTime = new TimeTracker();
		TextureContainer.Initialize(base.GraphicsDevice, base.Content);
		base.IsMouseVisible = true;
		ControlManager.Initialize();
		SceneRenderer.Initialize(base.GraphicsDevice, base.Content);
		SoundManager.Initialize(base.Content);
		ScreenStorage.Initialize();
		SaveManager.Init();
		GameScreen gameScreen = new GameScreen();
		new TitleScreen(showLogos: true, gameScreen.GetPlayer());
		new LoadScreen(gameScreen.GetScene().GetSceneObjectSpawner());
	}

	public static void AddPropCount(int i)
	{
	}

	public static void WritePropCounts()
	{
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		if (m_bExit)
		{
			Exit();
		}
		m_updateTime.Update(gameTime);
		Mp3MusicPlayer.Update(m_updateTime, Guide.IsVisible);
		SaveManager.HandleSaveLoadOptions();
		if (!Guide.IsVisible)
		{
			SceneRenderer.Update(m_updateTime);
			ControlManager.UpdateInput(gameTime);
			ScreenStorage.HandleInput(m_updateTime);
			ScreenStorage.Update(m_updateTime);
			SoundManager.Update(m_updateTime);
		}
		else if (ScreenStorage.PeekScreen() is GameScreen)
		{
			new PauseScreen(((GameScreen)ScreenStorage.PeekScreen()).GetScene());
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		m_drawTime.Update(gameTime);
		base.GraphicsDevice.Clear(Color.CornflowerBlue);
		ScreenStorage.Draw(m_updateTime);
		base.Draw(gameTime);
	}

	public static void ExitGame()
	{
		m_bExit = true;
	}

	public static void ShowPurchaseScreen(int controlIndex)
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(controlIndex);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer == null || !signedInGamer.IsSignedInToLive || !signedInGamer.Privileges.AllowPurchaseContent)
		{
			new GenericErrScreen(controlIndex, "The controller you are trying\nto use is not using an account\nthat can purchase XBox Indie Games.\nPlease use an XBox Live account\ncapable of purchasing content.");
		}
		else
		{
			Guide.ShowMarketplace(playerIndex);
		}
	}

	public static bool IsTrial()
	{
		return Guide.IsTrialMode;
	}

	public static bool CanSendMessageToFriend()
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer == null || !signedInGamer.IsSignedInToLive || signedInGamer.Privileges.AllowCommunication == GamerPrivilegeSetting.Blocked)
		{
			return false;
		}
		return true;
	}

	public static void SendMessageToFriend(string text)
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer == null || !signedInGamer.IsSignedInToLive || signedInGamer.Privileges.AllowCommunication == GamerPrivilegeSetting.Blocked)
		{
			new GenericErrScreen(ControlManager.ActiveMenuIndex, "The controller you are\ntrying to use is not using\nan account that can message\nother users.\nPlease use an XBox Live account\ncapable of messaging.");
		}
		else
		{
			Guide.ShowComposeMessage(ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex), text, null);
		}
	}

	public static string GetPlayerName()
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer == null)
		{
			return "Player";
		}
		return signedInGamer.Gamertag;
	}
}
