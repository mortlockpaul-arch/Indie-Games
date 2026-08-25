using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using ParticleSys;
using SpaceBlast.AI;
using SpaceBlast.AsyncJobManager;
using SpaceBlast.Networking;
using SpaceBlast.PathFinding;
using SpaceBlast.Screens;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class MainGame : Game
{
	private class ViewInfo
	{
		public Vector3 CameraPosition;

		public Matrix ViewMatrix;

		public Vector3 WorldTopLeft;

		public Vector3 WorldBottomRight;

		public Viewport Viewport;

		public int CameraTrackLeftEdge;

		public int CameraTrackRightEdge;

		public int CameraTrackTopEdge;

		public int CameraTrackBottomEdge;

		public bool ForceViewRecalc;

		public Vector3 ScreenCenter;

		public byte PlayerID;
	}

	private const int constTrialDemoLevel = 3;

	public static MainGame Instance = null;

	public static NetworkManager NetMan = null;

	public static GameLevel LevelData = new GameLevel();

	public static JobManager JobMan = new JobManager();

	public static AudioManager AudioMan = new AudioManager();

	public static ParticleSystemManager ParticleMan;

	public static PlayerList Players = new PlayerList();

	public static PlannedPath DebugPlannedPath = null;

	public static Line DebugLine = null;

	public static StaticWorldObject DebugObj = null;

	public static string TitlePath;

	public static string DebugMsg = "";

	public static ContentManager ContentMan = null;

	public static Matrix ProjectionMatrix;

	public static Matrix ViewMatrix;

	public static ShaderProfile MaxVertexShader;

	public static ShaderProfile MaxPixelShader;

	public static bool Is1080HD = false;

	public static GameOptions CurrentGameSettings = new DemoGameOptions();

	public static bool IsDemoMode;

	private GraphicsDeviceManager m_Graphics;

	private SpriteBatch m_SpriteBatch;

	private PrimitiveBatch m_PrimativeBatch;

	private Texture2D m_TexBackgroundLayer2;

	private Texture2D m_TexBackgroundLayer3;

	private Texture2D m_TexBackgroundLayer4;

	private Texture2D m_TexBackgroundLayer5;

	private Texture2D m_TrialModeDOG;

	private SpriteFont m_DebugFont;

	private Vector2 m_PosTrialModeDOG;

	private Random m_Random = new Random();

	private string m_CurrentLevel;

	private ViewInfo m_LeftView = new ViewInfo();

	private ViewInfo m_RightView = new ViewInfo();

	private Vector2 m_BackgroundOffset = default(Vector2);

	private Effect m_StarScollerFX;

	private EffectParameter m_StarScrollerFXOrigin;

	private Effect m_LightingFX;

	private EffectParameter m_LightingFXWorld;

	private EffectParameter m_LightingFXView;

	private EffectParameter[] m_LightingFXLightPositions;

	private EffectParameter[] m_LightingFXLightColours;

	private Effect m_DarkAlphaFX;

	private bool m_bSplitScreen;

	private Viewport m_MainViewport;

	private GamePadState m_LastPadState = default(GamePadState);

	private KeyboardState m_LastKeyState = default(KeyboardState);

	public LocalPlayer LeftPlayer;

	public LocalPlayer RightPlayer;

	public bool IsPaused;

	public bool IsFrozen;

	public static float ScreenToWorld;

	private bool m_ShowDebug;

	private ScreenManager m_ScreenManager;

	private IntroScreenComponent m_IntroComponent;

	private DrawableGameComponent m_HUDComponent;

	private MessageWindow m_MessageList;

	private BloomComponent m_BloomComponent;

	public bool IsMenuVisible => m_ScreenManager.CurrentScreenType != ScreenType.None;

	public MainGame()
	{
		Instance = this;
		m_Graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		ContentMan = base.Content;
		AudioMan.LoadContent();
		TitlePath = StorageContainer.TitleLocation;
		m_LeftView.CameraPosition = new Vector3(0f, 0f, GameConstants.CameraHeight);
		m_LeftView.ScreenCenter = default(Vector3);
		m_LeftView.PlayerID = 0;
		m_RightView.CameraPosition = new Vector3(0f, 0f, GameConstants.CameraHeight);
		m_RightView.ScreenCenter = default(Vector3);
		m_RightView.PlayerID = 1;
		NetMan = new NetworkManager(this);
		base.Components.Add(NetMan);
		m_ScreenManager = new ScreenManager(this);
		m_ScreenManager.DrawOrder = 20;
		base.Components.Add(m_ScreenManager);
		m_ScreenManager.Visible = true;
		m_MessageList = new MessageWindow(this);
		m_MessageList.DrawOrder = 30;
		base.Components.Add(m_MessageList);
		m_MessageList.Visible = true;
		m_IntroComponent = new IntroScreenComponent(this);
		m_IntroComponent.DrawOrder = 200;
		base.Components.Add(m_IntroComponent);
		ParticleMan = new ParticleSystemManager(this, base.Content);
	}

	protected override void Initialize()
	{
		Is1080HD = false;
		if (m_Graphics.GraphicsDevice.DisplayMode.Height == 1080)
		{
			DebugMsg = "Using 1920x1080 resolution";
			m_Graphics.PreferredBackBufferWidth = 1920;
			m_Graphics.PreferredBackBufferHeight = 1080;
			Is1080HD = true;
		}
		else
		{
			DebugMsg = "Using 1280x720 resolution";
			m_Graphics.PreferredBackBufferWidth = 1280;
			m_Graphics.PreferredBackBufferHeight = 720;
		}
		m_Graphics.IsFullScreen = true;
		m_Graphics.PreferMultiSampling = true;
		m_Graphics.ApplyChanges();
		base.IsFixedTimeStep = true;
		base.Components.Add(new GamerServicesComponent(this));
		NetMan.GameStartedEvent += NetworkGameStartedEvent;
		NetMan.GameEndedEvent += NetworkGameEndedEvent;
		NetMan.GamerJoinedEvent += NetworkGamerJoinedEvent;
		NetMan.GamerLeftEvent += NetworkGamerLeftEvent;
		NetMan.SessionEndedEvent += NetworkSessionEndedEvent;
		MaxPixelShader = base.GraphicsDevice.GraphicsDeviceCapabilities.MaxPixelShaderProfile;
		MaxVertexShader = base.GraphicsDevice.GraphicsDeviceCapabilities.MaxVertexShaderProfile;
		base.Initialize();
	}

	private void InitLevel()
	{
		float aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth / (float)GraphicsDeviceManager.DefaultBackBufferHeight;
		if (m_bSplitScreen)
		{
			aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth / 2f / (float)GraphicsDeviceManager.DefaultBackBufferHeight;
		}
		ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, GameConstants.CameraHeight - 10000f, GameConstants.CameraHeight + 10000f);
		m_LeftView.ViewMatrix = Matrix.CreateLookAt(m_LeftView.CameraPosition, Vector3.Zero, Vector3.Up);
		m_RightView.ViewMatrix = Matrix.CreateLookAt(m_RightView.CameraPosition, Vector3.Zero, Vector3.Up);
		CalcScreenVars();
		if (NetMan.IsNetworkGame)
		{
			LevelData.LoadLevel(NetMan.LevelNumber);
		}
		else if (IsDemoMode && Guide.IsTrialMode)
		{
			LevelData.LoadLevel(3);
		}
		else
		{
			LevelData.LoadLevel();
		}
		ResumeGame();
	}

	private void PreGameSetup()
	{
		AudioMan.Reset();
		m_bSplitScreen = false;
		IsPaused = false;
		IsFrozen = false;
		NetMan.IsNetworkGame = false;
		IsDemoMode = false;
	}

	public void StartNewGame(GameOptions gameOptions)
	{
		if (gameOptions is DemoGameOptions)
		{
			StartNewDemoGame((DemoGameOptions)gameOptions);
		}
		else if (gameOptions is SinglePlayerGameOptions)
		{
			StartNewSinglePlayerGame((SinglePlayerGameOptions)gameOptions);
		}
		else if (gameOptions is SinglePlayerTeamGameOptions)
		{
			StartNewSinglePlayerTeamGame((SinglePlayerTeamGameOptions)gameOptions);
		}
		else if (gameOptions is SinglePlayerCustomOptions)
		{
			StartNewCustomSinglePlayerGame((SinglePlayerCustomOptions)gameOptions);
		}
		else if (gameOptions is SplitScreenGameOptions)
		{
			StartNewSplitScreenGame((SplitScreenGameOptions)gameOptions);
		}
		else if (gameOptions is SplitScreenCoOpGameOptions)
		{
			StartNewSplitScreenCoOpGame((SplitScreenCoOpGameOptions)gameOptions);
		}
		else if (gameOptions is SplitScreenAllVsAllGameOptions)
		{
			StartNewSplitScreenAllVsAllGame((SplitScreenAllVsAllGameOptions)gameOptions);
		}
		else if (gameOptions is SplitScreenCustomGameOptions)
		{
			StartNewCustomSplitScreenGame((SplitScreenCustomGameOptions)gameOptions);
		}
		else if (gameOptions is SystemLinkGameOptions)
		{
			StartNewSystemLinkGame((SystemLinkGameOptions)gameOptions);
		}
		else if (gameOptions is SystemLinkCustomGameOptions)
		{
			StartNewCustomSystemLinkGame((SystemLinkCustomGameOptions)gameOptions);
		}
		else if (gameOptions is MultiplayerGameOptions)
		{
			StartNewMultiplayerGame((MultiplayerGameOptions)gameOptions);
		}
		else if (gameOptions is MultiplayerCustomGameOptions)
		{
			StartNewCustomMultiplayerGame((MultiplayerCustomGameOptions)gameOptions);
		}
	}

	private void StartNewDemoGame(DemoGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		IsDemoMode = true;
		InitLevel();
		HideHUD();
		Players.Clear();
		byte b = Players.AddAIPlayer(ShipColor.Green, null, AISkill.VeryEasy);
		b = Players.AddAIPlayer(ShipColor.Blue, null, AISkill.Easy);
		b = Players.AddAIPlayer(ShipColor.Cyan, null, AISkill.Easy);
		b = Players.AddAIPlayer(ShipColor.White, null, AISkill.Medium);
		b = Players.AddAIPlayer(ShipColor.Orange, null, AISkill.Hard);
		b = Players.AddAIPlayer(ShipColor.Purple, null, AISkill.Hard);
		b = Players.AddAIPlayer(ShipColor.Red, null, AISkill.VeryHard);
		b = Players.AddAIPlayer(ShipColor.Yellow, null, AISkill.VeryHard);
		LeftPlayer = (LocalPlayer)Players.GetPlayer(b);
		RightPlayer = null;
	}

	private void StartNewSinglePlayerGame(SinglePlayerGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		HideMenu();
		Utils.SetRichPresence(GamerPresenceMode.VersusComputer, null);
		InitLevel();
		Players.Clear();
		List<ShipColor> list = new List<ShipColor>();
		list.Add(ShipColor.Blue);
		list.Add(ShipColor.Cyan);
		list.Add(ShipColor.Green);
		list.Add(ShipColor.Orange);
		list.Add(ShipColor.Purple);
		list.Add(ShipColor.Red);
		list.Add(ShipColor.White);
		list.Add(ShipColor.Yellow);
		int index = m_Random.Next(list.Count - 1);
		ShipColor value = list[index];
		list.RemoveAt(index);
		Gamer gamer = null;
		if (Gamer.SignedInGamers.Count > 0)
		{
			gamer = Gamer.SignedInGamers[PlayerIndex.One];
		}
		byte id = Players.AddHumanPlayer(gamer, value, null, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		RightPlayer = null;
		switch (options.Skill)
		{
		case Difficulty.VeryEasy:
			Players.AddAIPlayer(list[0], null, AISkill.VeryEasy);
			break;
		case Difficulty.Easy:
			Players.AddAIPlayer(list[0], null, AISkill.Easy);
			Players.AddAIPlayer(list[1], null, AISkill.Easy);
			break;
		case Difficulty.Medium:
			Players.AddAIPlayer(list[0], null, AISkill.Medium);
			Players.AddAIPlayer(list[1], null, AISkill.Medium);
			Players.AddAIPlayer(list[2], null, AISkill.Medium);
			break;
		case Difficulty.Hard:
			Players.AddAIPlayer(list[0], null, AISkill.Hard);
			Players.AddAIPlayer(list[1], null, AISkill.Hard);
			Players.AddAIPlayer(list[2], null, AISkill.Hard);
			Players.AddAIPlayer(list[3], null, AISkill.Hard);
			break;
		case Difficulty.VeryHard:
			Players.AddAIPlayer(list[0], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[1], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[2], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[3], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[4], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[5], null, AISkill.VeryHard);
			break;
		case Difficulty.Extreme:
			Players.AddAIPlayer(list[0], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[1], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[2], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[3], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[4], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[5], null, AISkill.VeryHard);
			Players.AddAIPlayer(list[6], null, AISkill.VeryHard);
			break;
		}
		ShowFullHUD();
	}

	private void StartNewSinglePlayerTeamGame(SinglePlayerTeamGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		HideMenu();
		Utils.SetRichPresence(GamerPresenceMode.VersusComputer, null);
		InitLevel();
		Players.Clear();
		List<ShipColor> list = new List<ShipColor>();
		list.Add(ShipColor.Blue);
		list.Add(ShipColor.Cyan);
		list.Add(ShipColor.Green);
		list.Add(ShipColor.Orange);
		list.Add(ShipColor.Purple);
		list.Add(ShipColor.Red);
		list.Add(ShipColor.White);
		list.Add(ShipColor.Yellow);
		int index = m_Random.Next(list.Count - 1);
		ShipColor value = list[index];
		list.RemoveAt(index);
		Gamer gamer = null;
		if (Gamer.SignedInGamers.Count > 0)
		{
			gamer = Gamer.SignedInGamers[PlayerIndex.One];
		}
		byte id = Players.AddHumanPlayer(gamer, value, ETeam.Blue, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		RightPlayer = null;
		switch (options.Skill)
		{
		case Difficulty.VeryEasy:
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Green, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Green, AISkill.VeryHard);
			break;
		case Difficulty.Easy:
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Yellow, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Yellow, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Yellow, AISkill.VeryHard);
			break;
		case Difficulty.Medium:
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Orange, AISkill.VeryHard);
			break;
		case Difficulty.Hard:
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Purple, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Purple, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Purple, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Purple, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Purple, AISkill.VeryHard);
			break;
		case Difficulty.VeryHard:
			Players.AddAIPlayer(list[0], ETeam.Blue, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Cyan, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Cyan, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Cyan, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Cyan, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Cyan, AISkill.VeryHard);
			Players.AddAIPlayer(list[0], ETeam.Cyan, AISkill.VeryHard);
			break;
		case Difficulty.Extreme:
			Players.AddAIPlayer(list[0], ETeam.Red, AISkill.VeryEasy);
			Players.AddAIPlayer(list[0], ETeam.Red, AISkill.VeryEasy);
			Players.AddAIPlayer(list[0], ETeam.Red, AISkill.VeryEasy);
			Players.AddAIPlayer(list[0], ETeam.Red, AISkill.VeryEasy);
			Players.AddAIPlayer(list[0], ETeam.Red, AISkill.VeryEasy);
			Players.AddAIPlayer(list[0], ETeam.Red, AISkill.VeryEasy);
			Players.AddAIPlayer(list[0], ETeam.Red, AISkill.VeryEasy);
			break;
		}
		ShowFullHUD();
	}

	private void StartNewCustomSinglePlayerGame(SinglePlayerCustomOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
	}

	private void StartNewSplitScreenGame(SplitScreenGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		HideMenu();
		Utils.SetRichPresence(GamerPresenceMode.PlayingWithFriends, null);
		m_bSplitScreen = true;
		InitLevel();
		Players.Clear();
		ShipColor shipColor = (ShipColor)m_Random.Next(7);
		ShipColor shipColor2;
		for (shipColor2 = (ShipColor)m_Random.Next(7); shipColor2 == shipColor; shipColor2 = (ShipColor)m_Random.Next(7))
		{
		}
		Gamer gamer = null;
		if (Gamer.SignedInGamers.Count > 0)
		{
			gamer = Gamer.SignedInGamers[PlayerIndex.One];
		}
		byte id = Players.AddHumanPlayer(gamer, shipColor, null, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		Gamer gamer2 = null;
		if (Gamer.SignedInGamers.Count > 1)
		{
			gamer2 = Gamer.SignedInGamers[1];
		}
		id = Players.AddHumanPlayer(gamer2, shipColor2, null, primaryPlayer: false);
		RightPlayer = (HumanPlayer)Players.GetPlayer(id);
		ShowSplitScreenHUD();
		if (!InputManager.Player2Controller.HasValue)
		{
			m_ScreenManager.ShowScreen(ScreenType.Player2ControllerScreen);
		}
	}

	private void StartNewSplitScreenCoOpGame(SplitScreenCoOpGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		HideMenu();
		Utils.SetRichPresence(GamerPresenceMode.PlayingWithFriends, null);
		m_bSplitScreen = true;
		InitLevel();
		Players.Clear();
		Gamer gamer = null;
		if (Gamer.SignedInGamers.Count > 0)
		{
			gamer = Gamer.SignedInGamers[PlayerIndex.One];
		}
		byte id = Players.AddHumanPlayer(gamer, ShipColor.Green, ETeam.Green, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		Gamer gamer2 = null;
		if (Gamer.SignedInGamers.Count > 1)
		{
			gamer2 = Gamer.SignedInGamers[1];
		}
		id = Players.AddHumanPlayer(gamer2, ShipColor.Purple, ETeam.Green, primaryPlayer: false);
		RightPlayer = (HumanPlayer)Players.GetPlayer(id);
		switch (options.Skill)
		{
		case Difficulty.VeryEasy:
			Players.AddAIPlayer(ShipColor.Yellow, ETeam.Orange, AISkill.VeryHard);
			break;
		case Difficulty.Easy:
			Players.AddAIPlayer(ShipColor.Yellow, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Red, ETeam.Orange, AISkill.VeryHard);
			break;
		case Difficulty.Medium:
			Players.AddAIPlayer(ShipColor.Yellow, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Red, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, ETeam.Orange, AISkill.VeryHard);
			break;
		case Difficulty.Hard:
			Players.AddAIPlayer(ShipColor.Yellow, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Red, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Blue, ETeam.Orange, AISkill.VeryHard);
			break;
		case Difficulty.VeryHard:
			Players.AddAIPlayer(ShipColor.Yellow, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Red, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Blue, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Cyan, ETeam.Orange, AISkill.VeryHard);
			break;
		case Difficulty.Extreme:
			Players.AddAIPlayer(ShipColor.Yellow, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Red, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Blue, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Cyan, ETeam.Orange, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.White, ETeam.Orange, AISkill.VeryHard);
			break;
		}
		ShowSplitScreenHUD();
		if (!InputManager.Player2Controller.HasValue)
		{
			m_ScreenManager.ShowScreen(ScreenType.Player2ControllerScreen);
		}
	}

	private void StartNewSplitScreenAllVsAllGame(SplitScreenAllVsAllGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		HideMenu();
		Utils.SetRichPresence(GamerPresenceMode.PlayingWithFriends, null);
		m_bSplitScreen = true;
		InitLevel();
		Players.Clear();
		Gamer gamer = null;
		if (Gamer.SignedInGamers.Count > 0)
		{
			gamer = Gamer.SignedInGamers[PlayerIndex.One];
		}
		byte id = Players.AddHumanPlayer(gamer, ShipColor.Blue, null, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		Gamer gamer2 = null;
		if (Gamer.SignedInGamers.Count > 0)
		{
			gamer2 = Gamer.SignedInGamers[PlayerIndex.One];
		}
		id = Players.AddHumanPlayer(gamer2, ShipColor.Red, null, primaryPlayer: false);
		RightPlayer = (HumanPlayer)Players.GetPlayer(id);
		switch (options.Skill)
		{
		case Difficulty.VeryEasy:
			Players.AddAIPlayer(ShipColor.Yellow, null, AISkill.VeryHard);
			break;
		case Difficulty.Easy:
			Players.AddAIPlayer(ShipColor.Yellow, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Green, null, AISkill.VeryHard);
			break;
		case Difficulty.Medium:
			Players.AddAIPlayer(ShipColor.Yellow, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Green, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, null, AISkill.VeryHard);
			break;
		case Difficulty.Hard:
			Players.AddAIPlayer(ShipColor.Yellow, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Green, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Purple, null, AISkill.VeryHard);
			break;
		case Difficulty.VeryHard:
			Players.AddAIPlayer(ShipColor.Yellow, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Green, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Purple, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Cyan, null, AISkill.VeryHard);
			break;
		case Difficulty.Extreme:
			Players.AddAIPlayer(ShipColor.Yellow, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Green, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Orange, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Purple, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.Cyan, null, AISkill.VeryHard);
			Players.AddAIPlayer(ShipColor.White, null, AISkill.VeryHard);
			break;
		}
		ShowSplitScreenHUD();
		if (!InputManager.Player2Controller.HasValue)
		{
			m_ScreenManager.ShowScreen(ScreenType.Player2ControllerScreen);
		}
	}

	private void StartNewCustomSplitScreenGame(SplitScreenCustomGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
	}

	private void StartNewSystemLinkGame(SystemLinkGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		NetMan.IsNetworkGame = true;
		HideMenu();
		Utils.SetRichPresence(GamerPresenceMode.Multiplayer, null);
		InitLevel();
		Players.Clear();
		RightPlayer = null;
		LeftPlayer = null;
		LocalNetworkGamer localGamer = NetMan.GetLocalGamer();
		byte b = Players.AddLocalPlayer(localGamer, null, null);
		m_LeftView.PlayerID = b;
		LeftPlayer = (LocalPlayer)Players.GetPlayer(b);
		ShowFullHUD();
	}

	private void StartNewCustomSystemLinkGame(SystemLinkCustomGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
	}

	private void StartNewMultiplayerGame(MultiplayerGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
		HideMenu();
		Utils.SetRichPresence(GamerPresenceMode.Multiplayer, null);
		InitLevel();
		Players.Clear();
		RightPlayer = null;
		foreach (Gamer gamer in options.m_Gamers)
		{
			if (gamer is LocalNetworkGamer)
			{
				byte b = Players.AddLocalPlayer((LocalNetworkGamer)gamer, null, null);
				m_LeftView.PlayerID = b;
				LeftPlayer = (LocalPlayer)Players.GetPlayer(b);
			}
			else
			{
				Players.AddRemotePlayer((NetworkGamer)gamer, null, null);
			}
		}
		ShowFullHUD();
	}

	private void StartNewCustomMultiplayerGame(MultiplayerCustomGameOptions options)
	{
		CurrentGameSettings = options;
		PreGameSetup();
	}

	public void StartNextLevel()
	{
		StartNewGame(CurrentGameSettings);
	}

	private void ShowFullHUD()
	{
		if (m_HUDComponent != null)
		{
			base.Components.Remove(m_HUDComponent);
			m_HUDComponent.Dispose();
			m_HUDComponent = null;
		}
		m_HUDComponent = new FullHUDComponent(this, loadContentNow: true);
		m_HUDComponent.DrawOrder = 100;
		base.Components.Add(m_HUDComponent);
		((FullHUDComponent)m_HUDComponent).ShowPlayerHUD();
		m_HUDComponent.Enabled = false;
	}

	private void ShowSplitScreenHUD()
	{
		if (m_HUDComponent != null)
		{
			base.Components.Remove(m_HUDComponent);
			m_HUDComponent.Dispose();
			m_HUDComponent = null;
		}
		m_HUDComponent = new SplitScreenHUDComponent(this, loadContentNow: true);
		base.Components.Add(m_HUDComponent);
		((SplitScreenHUDComponent)m_HUDComponent).ShowPlayerHUD(LeftPlayer, RightPlayer);
		m_HUDComponent.Enabled = false;
	}

	private void HideHUD()
	{
		if (m_HUDComponent != null)
		{
			base.Components.Remove(m_HUDComponent);
			m_HUDComponent.Dispose();
			m_HUDComponent = null;
		}
	}

	public void AddToMessageWindow(string msg)
	{
		if (!m_bSplitScreen)
		{
			m_MessageList.AddMessage(msg);
		}
	}

	public void LeaveGame()
	{
		if (NetMan.IsNetworkGame)
		{
			NetMan.LeaveNetworkGame();
		}
		ShowMainScreen(showmenu: true);
	}

	private void ToggleFullscreen()
	{
		m_Graphics.ToggleFullScreen();
		CalcScreenVars();
		m_ScreenManager.HandleScreenResize();
	}

	private void CalcScreenVars()
	{
		m_MainViewport = base.GraphicsDevice.Viewport;
		m_LeftView.Viewport = m_MainViewport;
		m_RightView.Viewport = m_MainViewport;
		if (m_bSplitScreen)
		{
			m_LeftView.Viewport.Width = m_LeftView.Viewport.Width / 2;
			m_RightView.Viewport.Width = m_RightView.Viewport.Width / 2;
			m_RightView.Viewport.X = m_LeftView.Viewport.Width + 1;
		}
		if (!m_bSplitScreen)
		{
			m_LeftView.CameraTrackLeftEdge = (int)((float)m_LeftView.Viewport.Width * 0.25f);
			m_LeftView.CameraTrackRightEdge = (int)((float)m_LeftView.Viewport.Width * 0.75f);
			m_LeftView.CameraTrackTopEdge = (int)((float)m_LeftView.Viewport.Height * 0.25f);
			m_LeftView.CameraTrackBottomEdge = (int)((float)m_LeftView.Viewport.Height * 0.75f);
			m_LeftView.ForceViewRecalc = true;
		}
		else
		{
			m_LeftView.CameraTrackLeftEdge = (int)((float)m_LeftView.Viewport.Width * 0.4f);
			m_LeftView.CameraTrackRightEdge = (int)((float)m_LeftView.Viewport.Width * 0.6f);
			m_LeftView.CameraTrackTopEdge = (int)((float)m_LeftView.Viewport.Height * 0.25f);
			m_LeftView.CameraTrackBottomEdge = (int)((float)m_LeftView.Viewport.Height * 0.75f);
			m_LeftView.ForceViewRecalc = true;
			m_RightView.CameraTrackRightEdge = (int)((float)m_RightView.Viewport.Width * 0.4f);
			m_RightView.CameraTrackRightEdge = (int)((float)m_RightView.Viewport.Width * 0.6f);
			m_RightView.CameraTrackTopEdge = (int)((float)m_RightView.Viewport.Height * 0.25f);
			m_RightView.CameraTrackBottomEdge = (int)((float)m_RightView.Viewport.Height * 0.75f);
			m_RightView.ForceViewRecalc = true;
		}
		m_LeftView.ScreenCenter.X = m_LeftView.Viewport.Width / 2;
		m_LeftView.ScreenCenter.Y = m_LeftView.Viewport.Height / 2;
		m_LeftView.ScreenCenter.Z = 0f;
		if (m_bSplitScreen)
		{
			m_RightView.ScreenCenter.X = m_RightView.Viewport.Width / 2;
			m_RightView.ScreenCenter.Y = m_RightView.Viewport.Height / 2;
			m_RightView.ScreenCenter.Z = 0f;
		}
		CalcViewableWorld(m_LeftView);
		if (m_bSplitScreen)
		{
			CalcViewableWorld(m_RightView);
		}
		Vector3 vector = m_LeftView.Viewport.Unproject(new Vector3(0f, 0f, 0f), ProjectionMatrix, m_LeftView.ViewMatrix, Matrix.Identity);
		ScreenToWorld = m_LeftView.Viewport.Unproject(new Vector3(1f, 0f, 0f), ProjectionMatrix, m_LeftView.ViewMatrix, Matrix.Identity).X - vector.X;
		m_PosTrialModeDOG = default(Vector2);
		m_PosTrialModeDOG.X = (float)m_MainViewport.Width / 2f - (float)m_TrialModeDOG.Width / 2f;
		m_PosTrialModeDOG.Y = m_MainViewport.TitleSafeArea.Bottom - m_TrialModeDOG.Height;
	}

	protected override void LoadContent()
	{
		m_SpriteBatch = new SpriteBatch(base.GraphicsDevice);
		m_PrimativeBatch = new PrimitiveBatch(base.GraphicsDevice);
		m_TexBackgroundLayer2 = base.Content.Load<Texture2D>("Textures/Stars1");
		m_TexBackgroundLayer3 = base.Content.Load<Texture2D>("Textures/Stars2");
		m_TexBackgroundLayer4 = base.Content.Load<Texture2D>("Textures/Stars3");
		m_TexBackgroundLayer5 = base.Content.Load<Texture2D>("Textures/Stars4");
		m_TrialModeDOG = base.Content.Load<Texture2D>("Textures/TrialModeDOG");
		m_DebugFont = base.Content.Load<SpriteFont>("Fonts/HUDSmallFont");
		if (MaxPixelShader < ShaderProfile.PS_3_0)
		{
			m_StarScollerFX = base.Content.Load<Effect>("Effects/StarScrollerPS2");
		}
		else
		{
			m_StarScollerFX = base.Content.Load<Effect>("Effects/StarScroller");
		}
		m_StarScrollerFXOrigin = m_StarScollerFX.Parameters["TexOffset"];
		m_DarkAlphaFX = base.Content.Load<Effect>("Effects/DarkAlpha");
		m_LightingFX = ContentMan.Load<Effect>("Effects/Lighting");
		m_LightingFXWorld = m_LightingFX.Parameters["World"];
		m_LightingFXView = m_LightingFX.Parameters["View"];
		m_LightingFXLightPositions = new EffectParameter[8];
		m_LightingFXLightColours = new EffectParameter[8];
		for (int i = 0; i < 8; i++)
		{
			m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["range"].SetValue(200000f);
			m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["falloff"].SetValue(4f);
			m_LightingFXLightPositions[i] = m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["position"];
			m_LightingFXLightColours[i] = m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["color"];
		}
		ParticleMan.LoadContent();
		base.LoadContent();
		ShowMainScreen(showmenu: false);
	}

	public void ShowMainScreen(bool showmenu)
	{
		StartNewGame(new DemoGameOptions());
		if (showmenu)
		{
			ShowMainMenu();
		}
	}

	protected override void UnloadContent()
	{
	}

	public RenderTarget2D CloneRenderTarget(GraphicsDevice device, int numberLevels)
	{
		return new RenderTarget2D(device, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight, numberLevels, SurfaceFormat.Color, device.PresentationParameters.MultiSampleType, device.PresentationParameters.MultiSampleQuality);
	}

	protected override void Update(GameTime gameTime1)
	{
		if (m_IntroComponent != null)
		{
			m_IntroComponent.Update(gameTime1);
			if (m_IntroComponent == null || m_IntroComponent.Stage < IntroStage.FadingOut)
			{
				return;
			}
		}
		TimeManager.UpdateTime(gameTime1);
		if (Guide.IsVisible && !IsPaused)
		{
			if (m_ScreenManager.CurrentScreenType == ScreenType.PrivateGameScreen)
			{
				PauseGame(forceFreeze: false, showMenu: false);
			}
			else
			{
				PauseGame(forceFreeze: false, showMenu: true);
			}
		}
		if (!Guide.IsVisible)
		{
			HandleInput();
		}
		if (!IsFrozen)
		{
			Players.Update();
			HandleCameraTracking(LeftPlayer, m_LeftView);
			if (m_bSplitScreen)
			{
				HandleCameraTracking(RightPlayer, m_RightView);
			}
			LevelData.PowerUps.Update();
			CheckForCollisions();
			ParticleMan.Update(gameTime1);
		}
		if (Keyboard.GetState().IsKeyDown(Keys.F5))
		{
			ParticleMan.CreateExplosion(LeftPlayer.TheShip.Position, Vector3.Zero);
		}
		AudioMan.Update();
		base.Update(gameTime1);
	}

	private void HandleCameraTracking(Player player, ViewInfo viewInfo)
	{
		Vector3 vector = viewInfo.Viewport.Project(player.TheShip.Position, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
		int num = 0;
		int num2 = 0;
		if (vector.X < (float)viewInfo.CameraTrackLeftEdge)
		{
			num = (int)vector.X - viewInfo.CameraTrackLeftEdge;
		}
		else if (vector.X > (float)viewInfo.CameraTrackRightEdge)
		{
			num = (int)vector.X - viewInfo.CameraTrackRightEdge;
		}
		if (vector.Y < (float)viewInfo.CameraTrackTopEdge)
		{
			num2 = (int)vector.Y - viewInfo.CameraTrackTopEdge;
		}
		else if (vector.Y > (float)viewInfo.CameraTrackBottomEdge)
		{
			num2 = (int)vector.Y - viewInfo.CameraTrackBottomEdge;
		}
		if (num != 0 || num2 != 0 || viewInfo.ForceViewRecalc)
		{
			Vector3 screenCenter = viewInfo.ScreenCenter;
			screenCenter.X += num;
			screenCenter.Y += num2;
			viewInfo.CameraPosition = viewInfo.Viewport.Unproject(screenCenter, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
			viewInfo.CameraPosition.Z = GameConstants.CameraHeight;
			Vector3 cameraPosition = viewInfo.CameraPosition;
			cameraPosition.Z = 0f;
			viewInfo.ViewMatrix = Matrix.CreateLookAt(viewInfo.CameraPosition, cameraPosition, Vector3.Up);
			CalcViewableWorld(viewInfo);
			viewInfo.ForceViewRecalc = false;
		}
	}

	protected void HandleInput()
	{
		GamePadState player1Input = InputManager.GetPlayer1Input();
		KeyboardState state = Keyboard.GetState();
		if ((state.IsKeyDown(Keys.Pause) && m_LastKeyState.IsKeyUp(Keys.Pause)) || (player1Input.Buttons.Start == ButtonState.Pressed && m_LastPadState.Buttons.Start == ButtonState.Released))
		{
			if (m_ScreenManager.CurrentScreenType == ScreenType.PauseMenu)
			{
				ResumeGame();
			}
			else if (m_ScreenManager.CurrentScreenType == ScreenType.None)
			{
				PauseGame(forceFreeze: false, showMenu: true);
			}
		}
		m_LastPadState = player1Input;
		m_LastKeyState = state;
	}

	private void CalcViewableWorld(ViewInfo viewInfo)
	{
		Vector3 source = new Vector3(viewInfo.Viewport.X, viewInfo.Viewport.Height, 0f);
		Vector3 source2 = new Vector3(viewInfo.Viewport.X + viewInfo.Viewport.Width, 0f, 0f);
		viewInfo.WorldTopLeft = viewInfo.Viewport.Unproject(source, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
		viewInfo.WorldBottomRight = viewInfo.Viewport.Unproject(source2, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
	}

	private void CheckForCollisions()
	{
		foreach (Player value in Players.PlayerMap.Values)
		{
			CheckPlayerCollisions(value);
		}
	}

	private void CheckPlayerCollisions(Player player)
	{
		Vector3 collisionNormal = Vector3.Zero;
		LocalPlayer localPlayer = null;
		if (player is LocalPlayer)
		{
			localPlayer = (LocalPlayer)player;
			if (player is HumanPlayer)
			{
				_ = (HumanPlayer)player;
			}
		}
		if (localPlayer != null && localPlayer.IsActive)
		{
			BoundingSphere boundingSphere = localPlayer.TheShip.GetBoundingSphere();
			foreach (Player value in Players.PlayerMap.Values)
			{
				if (object.ReferenceEquals(localPlayer, value) || !value.IsActive || !boundingSphere.Intersects(value.TheShip.GetBoundingSphere()))
				{
					continue;
				}
				if (value is RemotePlayer)
				{
					if (localPlayer.PlayerID < value.PlayerID)
					{
						Vector3 pos = localPlayer.TheShip.Position;
						Vector3 pos2 = value.TheShip.Position;
						Vector3 v = value.TheShip.Velocity;
						Utils.ElasticCollision(ref pos, ref localPlayer.TheShip.Velocity, ref pos2, ref v, out var _, out var energy2);
						localPlayer.TheShip.Position += 1.1f * localPlayer.TheShip.Velocity;
						localPlayer.Die(null);
						NetMan.SendPlayersCollidedPacket(localPlayer.PlayerID, value.PlayerID, ref v, ref energy2);
					}
					continue;
				}
				Vector3 pos3 = localPlayer.TheShip.Position;
				Vector3 pos4 = value.TheShip.Position;
				Utils.ElasticCollision(ref pos3, ref localPlayer.TheShip.Velocity, ref pos4, ref value.TheShip.Velocity, out var energy3, out var energy4);
				localPlayer.TheShip.Position += 1.1f * localPlayer.TheShip.Velocity;
				value.TheShip.Position += 1.1f * value.TheShip.Velocity;
				if (localPlayer.TheShip.ApplyDamage((int)energy3))
				{
					localPlayer.Die(null);
				}
				if (value.TheShip.ApplyDamage((int)energy4))
				{
					value.Die(null);
				}
			}
			if (LevelData.StaticWorldObjects.CollisionTest(boundingSphere, ref collisionNormal))
			{
				if (localPlayer.TheShip.HandleCollision(ref collisionNormal))
				{
					localPlayer.Die(null);
				}
			}
			else
			{
				LevelData.DynamicWorldObjects.CollisionTest(boundingSphere);
			}
			LevelData.PowerUps.PlayerCollisionTest(localPlayer);
		}
		List<WeaponRound> activeRounds = player.TheShip.Weapons.ActiveRounds;
		for (int num = activeRounds.Count - 1; num >= 0; num--)
		{
			bool flag = false;
			WeaponRound weaponRound = activeRounds[num];
			BoundingSphere boundingSphere2 = weaponRound.GetBoundingSphere();
			if (!(weaponRound is EMPRound))
			{
				if (LevelData.StaticWorldObjects.CollisionTest(boundingSphere2, ref collisionNormal))
				{
					ParticleMan.CreateSmallExplosion(boundingSphere2.Center, Vector3.Zero);
					flag = true;
				}
				else
				{
					LevelData.DynamicWorldObjects.CollisionTest(boundingSphere2);
				}
			}
			foreach (Player value2 in Players.PlayerMap.Values)
			{
				if (object.ReferenceEquals(player, value2) || !boundingSphere2.Intersects(value2.TheShip.GetBoundingSphere()) || !value2.IsActive)
				{
					continue;
				}
				if (value2 is LocalPlayer)
				{
					if (weaponRound is EMPRound)
					{
						float duration = ((EMPRound)weaponRound).GetPowerCutDuration() * (float)((!player.IsMegaDamageActive) ? 1 : 2);
						value2.ApplyEMP(duration);
						continue;
					}
					int damage = weaponRound.GetHitDamage() * ((!player.IsMegaDamageActive) ? 1 : 3);
					if (value2.TheShip.ApplyDamage(damage))
					{
						value2.Die(player);
						if (localPlayer != null && NetMan.IsNetworkGame)
						{
							Players.IncreasePlayerScore(localPlayer, value2);
						}
					}
					ParticleMan.CreateSmallExplosion(boundingSphere2.Center, Vector3.Zero);
					flag = true;
				}
				else
				{
					ParticleMan.CreateSmallExplosion(boundingSphere2.Center, Vector3.Zero);
					flag = true;
				}
			}
			if (flag)
			{
				activeRounds.RemoveAt(num);
			}
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		m_Graphics.GraphicsDevice.Viewport = m_MainViewport;
		m_Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
		DrawSide(m_LeftView);
		if (m_bSplitScreen)
		{
			DrawSide(m_RightView);
		}
		m_Graphics.GraphicsDevice.Viewport = m_MainViewport;
		if (Guide.IsTrialMode)
		{
			m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
			m_DarkAlphaFX.Begin();
			m_DarkAlphaFX.CurrentTechnique.Passes[0].Begin();
			m_SpriteBatch.Draw(m_TrialModeDOG, m_PosTrialModeDOG, Color.White);
			m_DarkAlphaFX.CurrentTechnique.Passes[0].End();
			m_DarkAlphaFX.End();
			m_SpriteBatch.End();
		}
		base.Draw(gameTime);
	}

	private void DrawSide(ViewInfo viewInfo)
	{
		m_Graphics.GraphicsDevice.Viewport = viewInfo.Viewport;
		ViewMatrix = viewInfo.ViewMatrix;
		m_BackgroundOffset.X = (float)((double)viewInfo.WorldTopLeft.X / 500000.0) + 0.3f;
		m_BackgroundOffset.Y = (float)((0.0 - (double)viewInfo.WorldTopLeft.Y) / 500000.0) + 0.5f;
		m_StarScrollerFXOrigin.SetValue(m_BackgroundOffset);
		m_SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
		base.GraphicsDevice.Textures[1] = m_TexBackgroundLayer2;
		base.GraphicsDevice.Textures[2] = m_TexBackgroundLayer3;
		base.GraphicsDevice.Textures[3] = m_TexBackgroundLayer4;
		base.GraphicsDevice.Textures[4] = m_TexBackgroundLayer5;
		m_StarScollerFX.Begin();
		m_StarScollerFX.CurrentTechnique.Passes[0].Begin();
		m_SpriteBatch.Draw(LevelData.BackgroundTex, Vector2.Zero, Color.White);
		m_StarScollerFX.CurrentTechnique.Passes[0].End();
		m_StarScollerFX.End();
		m_SpriteBatch.End();
		m_LightingFXView.SetValue(ViewMatrix);
		m_LightingFX.Parameters["cameraPosition"].SetValue(GetCameraPos());
		m_LightingFX.Parameters["numLights"].SetValue(2);
		int num = 0;
		foreach (Player value2 in Players.PlayerMap.Values)
		{
			Vector4 value = new Vector4(value2.TheShip.Position, 1f);
			value.Z = 1000f;
			m_LightingFXLightPositions[num].SetValue(value);
			m_LightingFXLightColours[num].SetValue(value2.GetLightColour().ToVector4());
			num++;
		}
		LevelData.StaticWorldObjects.Draw(viewInfo.WorldTopLeft, viewInfo.WorldBottomRight);
		Players.Draw(viewInfo.PlayerID);
		LevelData.PowerUps.Draw();
		ParticleMan.Draw(ref ViewMatrix, ref ProjectionMatrix);
	}

	public Vector3 GetCameraPos()
	{
		return m_LeftView.CameraPosition;
	}

	private void ShowMainMenu()
	{
		IsPaused = true;
		m_ScreenManager.ShowScreen(ScreenType.MainMenu);
	}

	public void ShowGameOverPage()
	{
		AudioMan.Reset();
		if (NetMan.IsNetworkGame && NetMan.IsHost)
		{
			NetMan.SetGameOver();
		}
		PauseGame(forceFreeze: true, showMenu: false);
		m_ScreenManager.ShowScreen(ScreenType.GameResults);
	}

	private void HideMenu()
	{
		m_ScreenManager.HideScreen();
	}

	private void NetworkGameStartedEvent(object sender, GameStartedEventArgs e)
	{
	}

	private void NetworkGameEndedEvent(object sender, GameEndedEventArgs e)
	{
	}

	private void NetworkGamerJoinedEvent(object sender, GamerJoinedEventArgs e)
	{
	}

	private void NetworkGamerLeftEvent(object sender, GamerLeftEventArgs e)
	{
	}

	private void NetworkSessionEndedEvent(object sender, NetworkSessionEndedEventArgs e)
	{
		LeaveGame();
	}

	public void PauseGame(bool forceFreeze, bool showMenu)
	{
		IsPaused = true;
		if (showMenu)
		{
			m_ScreenManager.ShowScreen(ScreenType.PauseMenu);
		}
		if (forceFreeze || !NetMan.IsNetworkGame)
		{
			IsFrozen = true;
			TimeManager.Pause();
		}
	}

	public void ShowControllerDisconnectedScreen(bool primaryPlayer)
	{
		IsPaused = true;
		if (!NetMan.IsNetworkGame)
		{
			IsFrozen = true;
			TimeManager.Pause();
		}
		ControllerDisconnectedScreen controllerDisconnectedScreen = (ControllerDisconnectedScreen)m_ScreenManager.GetScreen(ScreenType.ControllerDisconnected);
		controllerDisconnectedScreen.SetController(primaryPlayer);
		m_ScreenManager.ShowScreen(ScreenType.ControllerDisconnected);
	}

	public void ResumeGame()
	{
		TimeManager.Resume();
		m_ScreenManager.HideScreen();
		IsPaused = false;
		IsFrozen = false;
	}

	public void IntroFinished()
	{
		base.Components.Remove(m_IntroComponent);
		m_IntroComponent = null;
		if (InputManager.Player1Controller.HasValue)
		{
			ShowMainMenu();
		}
		else
		{
			m_ScreenManager.ShowScreen(ScreenType.PressStart);
		}
	}

	public bool IsWithinAudibleRange(Vector3 soundSource)
	{
		if ((LeftPlayer.TheShip.Position - soundSource).Length() < 100000f)
		{
			return true;
		}
		if (RightPlayer != null && (RightPlayer.TheShip.Position - soundSource).Length() < 100000f)
		{
			return true;
		}
		return false;
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		JobMan.Shutdown();
		base.OnExiting(sender, args);
	}
}
