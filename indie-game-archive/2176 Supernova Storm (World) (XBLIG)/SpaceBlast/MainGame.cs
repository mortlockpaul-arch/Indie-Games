using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

	private Random m_Random;

	private string m_CurrentLevel;

	private ViewInfo m_LeftView;

	private ViewInfo m_RightView;

	private Vector2 m_BackgroundOffset;

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

	private GamePadState m_LastPadState;

	private KeyboardState m_LastKeyState;

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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		m_Random = new Random();
		m_LeftView = new ViewInfo();
		m_RightView = new ViewInfo();
		m_BackgroundOffset = default(Vector2);
		m_LastPadState = default(GamePadState);
		m_LastKeyState = default(KeyboardState);
		((Game)this)._002Ector();
		Instance = this;
		m_Graphics = new GraphicsDeviceManager((Game)(object)this);
		((Game)this).Content.RootDirectory = "Content";
		ContentMan = ((Game)this).Content;
		AudioMan.LoadContent();
		TitlePath = StorageContainer.TitleLocation;
		m_LeftView.CameraPosition = new Vector3(0f, 0f, GameConstants.CameraHeight);
		m_LeftView.ScreenCenter = default(Vector3);
		m_LeftView.PlayerID = 0;
		m_RightView.CameraPosition = new Vector3(0f, 0f, GameConstants.CameraHeight);
		m_RightView.ScreenCenter = default(Vector3);
		m_RightView.PlayerID = 1;
		NetMan = new NetworkManager((Game)(object)this);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)NetMan);
		m_ScreenManager = new ScreenManager((Game)(object)this);
		((DrawableGameComponent)m_ScreenManager).DrawOrder = 20;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)m_ScreenManager);
		((DrawableGameComponent)m_ScreenManager).Visible = true;
		m_MessageList = new MessageWindow((Game)(object)this);
		((DrawableGameComponent)m_MessageList).DrawOrder = 30;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)m_MessageList);
		((DrawableGameComponent)m_MessageList).Visible = true;
		m_IntroComponent = new IntroScreenComponent(this);
		((DrawableGameComponent)m_IntroComponent).DrawOrder = 200;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)m_IntroComponent);
		ParticleMan = new ParticleSystemManager((Game)(object)this, ((Game)this).Content);
	}

	protected override void Initialize()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		Is1080HD = false;
		DisplayMode displayMode = m_Graphics.GraphicsDevice.DisplayMode;
		if (((DisplayMode)(ref displayMode)).Height == 1080)
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
		((Game)this).IsFixedTimeStep = true;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		NetMan.GameStartedEvent += NetworkGameStartedEvent;
		NetMan.GameEndedEvent += NetworkGameEndedEvent;
		NetMan.GamerJoinedEvent += NetworkGamerJoinedEvent;
		NetMan.GamerLeftEvent += NetworkGamerLeftEvent;
		NetMan.SessionEndedEvent += NetworkSessionEndedEvent;
		MaxPixelShader = ((Game)this).GraphicsDevice.GraphicsDeviceCapabilities.MaxPixelShaderProfile;
		MaxVertexShader = ((Game)this).GraphicsDevice.GraphicsDeviceCapabilities.MaxVertexShaderProfile;
		((Game)this).Initialize();
	}

	private void InitLevel()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)GraphicsDeviceManager.DefaultBackBufferWidth / (float)GraphicsDeviceManager.DefaultBackBufferHeight;
		if (m_bSplitScreen)
		{
			num = (float)GraphicsDeviceManager.DefaultBackBufferWidth / 2f / (float)GraphicsDeviceManager.DefaultBackBufferHeight;
		}
		ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), num, GameConstants.CameraHeight - 10000f, GameConstants.CameraHeight + 10000f);
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
		Utils.SetRichPresence((GamerPresenceMode)7, null);
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
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
		{
			gamer = (Gamer)(object)Gamer.SignedInGamers[(PlayerIndex)0];
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
		Utils.SetRichPresence((GamerPresenceMode)7, null);
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
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
		{
			gamer = (Gamer)(object)Gamer.SignedInGamers[(PlayerIndex)0];
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
		Utils.SetRichPresence((GamerPresenceMode)45, null);
		m_bSplitScreen = true;
		InitLevel();
		Players.Clear();
		ShipColor shipColor = (ShipColor)m_Random.Next(7);
		ShipColor shipColor2;
		for (shipColor2 = (ShipColor)m_Random.Next(7); shipColor2 == shipColor; shipColor2 = (ShipColor)m_Random.Next(7))
		{
		}
		Gamer gamer = null;
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
		{
			gamer = (Gamer)(object)Gamer.SignedInGamers[(PlayerIndex)0];
		}
		byte id = Players.AddHumanPlayer(gamer, shipColor, null, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		Gamer gamer2 = null;
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 1)
		{
			gamer2 = (Gamer)(object)((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[1];
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
		Utils.SetRichPresence((GamerPresenceMode)45, null);
		m_bSplitScreen = true;
		InitLevel();
		Players.Clear();
		Gamer gamer = null;
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
		{
			gamer = (Gamer)(object)Gamer.SignedInGamers[(PlayerIndex)0];
		}
		byte id = Players.AddHumanPlayer(gamer, ShipColor.Green, ETeam.Green, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		Gamer gamer2 = null;
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 1)
		{
			gamer2 = (Gamer)(object)((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[1];
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
		Utils.SetRichPresence((GamerPresenceMode)45, null);
		m_bSplitScreen = true;
		InitLevel();
		Players.Clear();
		Gamer gamer = null;
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
		{
			gamer = (Gamer)(object)Gamer.SignedInGamers[(PlayerIndex)0];
		}
		byte id = Players.AddHumanPlayer(gamer, ShipColor.Blue, null, primaryPlayer: true);
		LeftPlayer = (HumanPlayer)Players.GetPlayer(id);
		Gamer gamer2 = null;
		if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count > 0)
		{
			gamer2 = (Gamer)(object)Gamer.SignedInGamers[(PlayerIndex)0];
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
		Utils.SetRichPresence((GamerPresenceMode)2, null);
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
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		CurrentGameSettings = options;
		PreGameSetup();
		HideMenu();
		Utils.SetRichPresence((GamerPresenceMode)2, null);
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
			((Collection<IGameComponent>)(object)((Game)this).Components).Remove((IGameComponent)(object)m_HUDComponent);
			((GameComponent)m_HUDComponent).Dispose();
			m_HUDComponent = null;
		}
		m_HUDComponent = (DrawableGameComponent)(object)new FullHUDComponent((Game)(object)this, loadContentNow: true);
		m_HUDComponent.DrawOrder = 100;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)m_HUDComponent);
		((FullHUDComponent)(object)m_HUDComponent).ShowPlayerHUD();
		((GameComponent)m_HUDComponent).Enabled = false;
	}

	private void ShowSplitScreenHUD()
	{
		if (m_HUDComponent != null)
		{
			((Collection<IGameComponent>)(object)((Game)this).Components).Remove((IGameComponent)(object)m_HUDComponent);
			((GameComponent)m_HUDComponent).Dispose();
			m_HUDComponent = null;
		}
		m_HUDComponent = (DrawableGameComponent)(object)new SplitScreenHUDComponent((Game)(object)this, loadContentNow: true);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)m_HUDComponent);
		((SplitScreenHUDComponent)(object)m_HUDComponent).ShowPlayerHUD(LeftPlayer, RightPlayer);
		((GameComponent)m_HUDComponent).Enabled = false;
	}

	private void HideHUD()
	{
		if (m_HUDComponent != null)
		{
			((Collection<IGameComponent>)(object)((Game)this).Components).Remove((IGameComponent)(object)m_HUDComponent);
			((GameComponent)m_HUDComponent).Dispose();
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		m_MainViewport = ((Game)this).GraphicsDevice.Viewport;
		m_LeftView.Viewport = m_MainViewport;
		m_RightView.Viewport = m_MainViewport;
		if (m_bSplitScreen)
		{
			((Viewport)(ref m_LeftView.Viewport)).Width = ((Viewport)(ref m_LeftView.Viewport)).Width / 2;
			((Viewport)(ref m_RightView.Viewport)).Width = ((Viewport)(ref m_RightView.Viewport)).Width / 2;
			((Viewport)(ref m_RightView.Viewport)).X = ((Viewport)(ref m_LeftView.Viewport)).Width + 1;
		}
		if (!m_bSplitScreen)
		{
			m_LeftView.CameraTrackLeftEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Width * 0.25f);
			m_LeftView.CameraTrackRightEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Width * 0.75f);
			m_LeftView.CameraTrackTopEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Height * 0.25f);
			m_LeftView.CameraTrackBottomEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Height * 0.75f);
			m_LeftView.ForceViewRecalc = true;
		}
		else
		{
			m_LeftView.CameraTrackLeftEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Width * 0.4f);
			m_LeftView.CameraTrackRightEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Width * 0.6f);
			m_LeftView.CameraTrackTopEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Height * 0.25f);
			m_LeftView.CameraTrackBottomEdge = (int)((float)((Viewport)(ref m_LeftView.Viewport)).Height * 0.75f);
			m_LeftView.ForceViewRecalc = true;
			m_RightView.CameraTrackRightEdge = (int)((float)((Viewport)(ref m_RightView.Viewport)).Width * 0.4f);
			m_RightView.CameraTrackRightEdge = (int)((float)((Viewport)(ref m_RightView.Viewport)).Width * 0.6f);
			m_RightView.CameraTrackTopEdge = (int)((float)((Viewport)(ref m_RightView.Viewport)).Height * 0.25f);
			m_RightView.CameraTrackBottomEdge = (int)((float)((Viewport)(ref m_RightView.Viewport)).Height * 0.75f);
			m_RightView.ForceViewRecalc = true;
		}
		m_LeftView.ScreenCenter.X = ((Viewport)(ref m_LeftView.Viewport)).Width / 2;
		m_LeftView.ScreenCenter.Y = ((Viewport)(ref m_LeftView.Viewport)).Height / 2;
		m_LeftView.ScreenCenter.Z = 0f;
		if (m_bSplitScreen)
		{
			m_RightView.ScreenCenter.X = ((Viewport)(ref m_RightView.Viewport)).Width / 2;
			m_RightView.ScreenCenter.Y = ((Viewport)(ref m_RightView.Viewport)).Height / 2;
			m_RightView.ScreenCenter.Z = 0f;
		}
		CalcViewableWorld(m_LeftView);
		if (m_bSplitScreen)
		{
			CalcViewableWorld(m_RightView);
		}
		Vector3 val = ((Viewport)(ref m_LeftView.Viewport)).Unproject(new Vector3(0f, 0f, 0f), ProjectionMatrix, m_LeftView.ViewMatrix, Matrix.Identity);
		ScreenToWorld = ((Viewport)(ref m_LeftView.Viewport)).Unproject(new Vector3(1f, 0f, 0f), ProjectionMatrix, m_LeftView.ViewMatrix, Matrix.Identity).X - val.X;
		m_PosTrialModeDOG = default(Vector2);
		m_PosTrialModeDOG.X = (float)((Viewport)(ref m_MainViewport)).Width / 2f - (float)m_TrialModeDOG.Width / 2f;
		ref Vector2 posTrialModeDOG = ref m_PosTrialModeDOG;
		Rectangle titleSafeArea = ((Viewport)(ref m_MainViewport)).TitleSafeArea;
		posTrialModeDOG.Y = ((Rectangle)(ref titleSafeArea)).Bottom - m_TrialModeDOG.Height;
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Invalid comparison between Unknown and I4
		m_SpriteBatch = new SpriteBatch(((Game)this).GraphicsDevice);
		m_PrimativeBatch = new PrimitiveBatch(((Game)this).GraphicsDevice);
		m_TexBackgroundLayer2 = ((Game)this).Content.Load<Texture2D>("Textures/Stars1");
		m_TexBackgroundLayer3 = ((Game)this).Content.Load<Texture2D>("Textures/Stars2");
		m_TexBackgroundLayer4 = ((Game)this).Content.Load<Texture2D>("Textures/Stars3");
		m_TexBackgroundLayer5 = ((Game)this).Content.Load<Texture2D>("Textures/Stars4");
		m_TrialModeDOG = ((Game)this).Content.Load<Texture2D>("Textures/TrialModeDOG");
		m_DebugFont = ((Game)this).Content.Load<SpriteFont>("Fonts/HUDSmallFont");
		if ((int)MaxPixelShader < 8)
		{
			m_StarScollerFX = ((Game)this).Content.Load<Effect>("Effects/StarScrollerPS2");
		}
		else
		{
			m_StarScollerFX = ((Game)this).Content.Load<Effect>("Effects/StarScroller");
		}
		m_StarScrollerFXOrigin = m_StarScollerFX.Parameters["TexOffset"];
		m_DarkAlphaFX = ((Game)this).Content.Load<Effect>("Effects/DarkAlpha");
		m_LightingFX = ContentMan.Load<Effect>("Effects/Lighting");
		m_LightingFXWorld = m_LightingFX.Parameters["World"];
		m_LightingFXView = m_LightingFX.Parameters["View"];
		m_LightingFXLightPositions = (EffectParameter[])(object)new EffectParameter[8];
		m_LightingFXLightColours = (EffectParameter[])(object)new EffectParameter[8];
		for (int i = 0; i < 8; i++)
		{
			m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["range"].SetValue(200000f);
			m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["falloff"].SetValue(4f);
			m_LightingFXLightPositions[i] = m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["position"];
			m_LightingFXLightColours[i] = m_LightingFX.Parameters["lights"].Elements[i].StructureMembers["color"];
		}
		ParticleMan.LoadContent();
		((Game)this).LoadContent();
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		return new RenderTarget2D(device, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight, numberLevels, (SurfaceFormat)1, device.PresentationParameters.MultiSampleType, device.PresentationParameters.MultiSampleQuality);
	}

	protected override void Update(GameTime gameTime1)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (m_IntroComponent != null)
		{
			((GameComponent)m_IntroComponent).Update(gameTime1);
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
		KeyboardState state = Keyboard.GetState();
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)116))
		{
			ParticleMan.CreateExplosion(LeftPlayer.TheShip.Position, Vector3.Zero);
		}
		AudioMan.Update();
		((Game)this).Update(gameTime1);
	}

	private void HandleCameraTracking(Player player, ViewInfo viewInfo)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Viewport)(ref viewInfo.Viewport)).Project(player.TheShip.Position, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
		int num = 0;
		int num2 = 0;
		if (val.X < (float)viewInfo.CameraTrackLeftEdge)
		{
			num = (int)val.X - viewInfo.CameraTrackLeftEdge;
		}
		else if (val.X > (float)viewInfo.CameraTrackRightEdge)
		{
			num = (int)val.X - viewInfo.CameraTrackRightEdge;
		}
		if (val.Y < (float)viewInfo.CameraTrackTopEdge)
		{
			num2 = (int)val.Y - viewInfo.CameraTrackTopEdge;
		}
		else if (val.Y > (float)viewInfo.CameraTrackBottomEdge)
		{
			num2 = (int)val.Y - viewInfo.CameraTrackBottomEdge;
		}
		if (num != 0 || num2 != 0 || viewInfo.ForceViewRecalc)
		{
			Vector3 screenCenter = viewInfo.ScreenCenter;
			screenCenter.X += (float)num;
			screenCenter.Y += (float)num2;
			viewInfo.CameraPosition = ((Viewport)(ref viewInfo.Viewport)).Unproject(screenCenter, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		GamePadState player1Input = InputManager.GetPlayer1Input();
		KeyboardState state = Keyboard.GetState();
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)19) && ((KeyboardState)(ref m_LastKeyState)).IsKeyUp((Keys)19))
		{
			goto IL_004d;
		}
		GamePadButtons buttons = ((GamePadState)(ref player1Input)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).Start == 1)
		{
			GamePadButtons buttons2 = ((GamePadState)(ref m_LastPadState)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).Start == 0)
			{
				goto IL_004d;
			}
		}
		goto IL_0078;
		IL_004d:
		if (m_ScreenManager.CurrentScreenType == ScreenType.PauseMenu)
		{
			ResumeGame();
		}
		else if (m_ScreenManager.CurrentScreenType == ScreenType.None)
		{
			PauseGame(forceFreeze: false, showMenu: true);
		}
		goto IL_0078;
		IL_0078:
		m_LastPadState = player1Input;
		m_LastKeyState = state;
	}

	private void CalcViewableWorld(ViewInfo viewInfo)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)((Viewport)(ref viewInfo.Viewport)).X, (float)((Viewport)(ref viewInfo.Viewport)).Height, 0f);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector((float)(((Viewport)(ref viewInfo.Viewport)).X + ((Viewport)(ref viewInfo.Viewport)).Width), 0f, 0f);
		viewInfo.WorldTopLeft = ((Viewport)(ref viewInfo.Viewport)).Unproject(val, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
		viewInfo.WorldBottomRight = ((Viewport)(ref viewInfo.Viewport)).Unproject(val2, ProjectionMatrix, viewInfo.ViewMatrix, Matrix.Identity);
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
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
				if (object.ReferenceEquals(localPlayer, value) || !value.IsActive || !((BoundingSphere)(ref boundingSphere)).Intersects(value.TheShip.GetBoundingSphere()))
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
						Ship theShip = localPlayer.TheShip;
						theShip.Position += 1.1f * localPlayer.TheShip.Velocity;
						localPlayer.Die(null);
						NetMan.SendPlayersCollidedPacket(localPlayer.PlayerID, value.PlayerID, ref v, ref energy2);
					}
					continue;
				}
				Vector3 pos3 = localPlayer.TheShip.Position;
				Vector3 pos4 = value.TheShip.Position;
				Utils.ElasticCollision(ref pos3, ref localPlayer.TheShip.Velocity, ref pos4, ref value.TheShip.Velocity, out var energy3, out var energy4);
				Ship theShip2 = localPlayer.TheShip;
				theShip2.Position += 1.1f * localPlayer.TheShip.Velocity;
				Ship theShip3 = value.TheShip;
				theShip3.Position += 1.1f * value.TheShip.Velocity;
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
				if (object.ReferenceEquals(player, value2) || !((BoundingSphere)(ref boundingSphere2)).Intersects(value2.TheShip.GetBoundingSphere()) || !value2.IsActive)
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
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
			m_SpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
			m_DarkAlphaFX.Begin();
			m_DarkAlphaFX.CurrentTechnique.Passes[0].Begin();
			m_SpriteBatch.Draw(m_TrialModeDOG, m_PosTrialModeDOG, Color.White);
			m_DarkAlphaFX.CurrentTechnique.Passes[0].End();
			m_DarkAlphaFX.End();
			m_SpriteBatch.End();
		}
		((Game)this).Draw(gameTime);
	}

	private void DrawSide(ViewInfo viewInfo)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		m_Graphics.GraphicsDevice.Viewport = viewInfo.Viewport;
		ViewMatrix = viewInfo.ViewMatrix;
		m_BackgroundOffset.X = (float)((double)viewInfo.WorldTopLeft.X / 500000.0) + 0.3f;
		m_BackgroundOffset.Y = (float)((0.0 - (double)viewInfo.WorldTopLeft.Y) / 500000.0) + 0.5f;
		m_StarScrollerFXOrigin.SetValue(m_BackgroundOffset);
		m_SpriteBatch.Begin((SpriteBlendMode)0, (SpriteSortMode)0, (SaveStateMode)0);
		((Game)this).GraphicsDevice.Textures[1] = (Texture)(object)m_TexBackgroundLayer2;
		((Game)this).GraphicsDevice.Textures[2] = (Texture)(object)m_TexBackgroundLayer3;
		((Game)this).GraphicsDevice.Textures[3] = (Texture)(object)m_TexBackgroundLayer4;
		((Game)this).GraphicsDevice.Textures[4] = (Texture)(object)m_TexBackgroundLayer5;
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
		Vector4 value = default(Vector4);
		foreach (Player value2 in Players.PlayerMap.Values)
		{
			((Vector4)(ref value))._002Ector(value2.TheShip.Position, 1f);
			value.Z = 1000f;
			m_LightingFXLightPositions[num].SetValue(value);
			EffectParameter obj = m_LightingFXLightColours[num];
			Color lightColour = value2.GetLightColour();
			obj.SetValue(((Color)(ref lightColour)).ToVector4());
			num++;
		}
		LevelData.StaticWorldObjects.Draw(viewInfo.WorldTopLeft, viewInfo.WorldBottomRight);
		Players.Draw(viewInfo.PlayerID);
		LevelData.PowerUps.Draw();
		ParticleMan.Draw(ref ViewMatrix, ref ProjectionMatrix);
	}

	public Vector3 GetCameraPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
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
		((Collection<IGameComponent>)(object)((Game)this).Components).Remove((IGameComponent)(object)m_IntroComponent);
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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = LeftPlayer.TheShip.Position - soundSource;
		if (((Vector3)(ref val)).Length() < 100000f)
		{
			return true;
		}
		if (RightPlayer != null)
		{
			val = RightPlayer.TheShip.Position - soundSource;
			if (((Vector3)(ref val)).Length() < 100000f)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		JobMan.Shutdown();
		((Game)this).OnExiting(sender, args);
	}
}
