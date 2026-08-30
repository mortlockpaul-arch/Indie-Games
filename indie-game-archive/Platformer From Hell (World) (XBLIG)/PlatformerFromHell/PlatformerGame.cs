using System;
using System.IO;
using System.Xml.Serialization;
using EasyStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PlatformerFromHell;

public class PlatformerGame : Game
{
	public enum GameStates
	{
		PressToStart,
		StartMenu,
		TrialDirectory,
		WorldMapDirectory,
		WorldMap,
		Normal,
		Loading,
		Credits,
		Controls,
		Cutscene,
		IntroScroll,
		ExitScroll,
		Confirm,
		Paused
	}

	private enum StorageState
	{
		NotSaving,
		SelectStorageDevice,
		SelectingStorageDevice,
		OpenStorageContainer,
		OpeningStorageContainer,
		ReadyToUse
	}

	public const bool TESTING_MODE = false;

	public const bool PLAYTEST = false;

	public bool donttryagain = false;

	public static long vibrateTimer = -1L;

	public bool levelfinishedLoading;

	public bool levelpauseTime;

	public int levelpauseTimer;

	public long startMenuFadeInTimer;

	public int testTimer;

	public SoundEffect startMenuClick;

	public SoundEffect startGame;

	public SoundEffect noaccess;

	public SoundEffect chaChing;

	public SoundEffect introPart1;

	public SoundEffect introPart2;

	public SoundEffectInstance startMenuClickInstance;

	public SoundEffectInstance startGameInstance;

	public SoundEffectInstance noaccessInstance;

	public SoundEffectInstance chaChingInstance;

	public SoundEffectInstance introPart1Instance;

	public SoundEffectInstance introPart2Instance;

	public float world1Volume = 0.25f;

	public float world2Volume = 0.3f;

	public float world3Volume = 0.5f;

	public float world4Volume = 0.5f;

	public float world5Volume = 0.3f;

	public float beatTheGameVolume = 1f;

	public float startMenuVolume = 0.2f;

	public float controlsVolume = 0.3f;

	public float creditsVolume = 0.3f;

	public float worldMapVolume = 0.2f;

	public float startMenuClickVolume = 1f;

	public float startGameVolume = 0.3f;

	public float noaccessVolume = 0.3f;

	public float chaChingVolume = 1f;

	public float introPart1Volume = 0.3f;

	public float introPart2Volume = 0.3f;

	public GameStates GameState;

	public static PlayerIndex playerIndex;

	private string filename = "savegame.dat";

	private SaveGameData saveData;

	private SaveGameData loadedData;

	private AnimationPlayer sprite;

	private Animation runAnimation;

	private Animation headshotAnimation;

	private Animation loadAnimation;

	public bool beatTheGame;

	public string ConfirmationString;

	public Texture2D confirm;

	public string toConfirm;

	public int confirmTracker = 0;

	public int[] confirmX = new int[2] { 595, 595 };

	public int[] confirmY = new int[2] { 360, 390 };

	public string[] trialNames = new string[5] { "Preview Level 1", "Preview Level 2", "Preview Level 3", "Preview Level 4", "Preview Level 5" };

	public int[] trialLevels = new int[5] { 0, 1, 2, 0, 1 };

	public string[] worldNames = new string[5] { "World 1: The Fiery Inferno", "World 2: The Feverous Jungle", "World 3: The Frosty Tower", "World 4: The Mechanical Dystopia", "World 5: Game Over" };

	public string[] world1Levels = new string[8] { "To The World Map", "Level 1: Lollop", "Level 2: Inward", "Level 3: Rerun", "Level 4: Trinity", "Level 5: Bounce Outside the Box", "Level $: Clutches of Claustrophobia", "To World 2: The Feverous Jungle" };

	public string[] world2Levels = new string[8] { "To World 1: The Fiery Inferno", "Level 1: Tie Guy Has Hops", "Level 2: Platforms for Dais", "Level 3: Wavy World", "Level 4: Criss Cross Chasm", "Level 5: Upsurge", "Level $: Platform Limbo", "To World 3: The Frosty Tower" };

	public string[] world3Levels = new string[8] { "To World 2: The Feverous Jungle", "Level 1: Fluctuate", "Level 2: Pit of Greed", "Level 3: Ascension", "Level 4: Descension", "Level 5: Nosedive", "Level $: Bottom Backtrack", "To World 4: The Mechanical Dystopia" };

	public string[] world4Levels = new string[8] { "To World 3: The Frosty Tower", "Level 1: Unhinged", "Level 2: Son of a Switch", "Level 3: Daring Detour", "Level 4: Forsaken Scrapland", "Level 5: Devious District", "Level $: The Chimney Stack", "To World 5: Game Over" };

	public string[] world5Levels = new string[8] { "To World 4: The Mechanical Dystopia", "Level 1: Deep Passage", "Level 2: There and Back Again", "Level 3: Restless Agony", "Level 4: Derailed", "Level 5: Alteration", "Level $: Eradicator", "To The Credits" };

	public int[] world1Locks = new int[6] { 1, 1, 1, 1, 1, 0 };

	public int[] world2Locks = new int[6];

	public int[] world3Locks = new int[6];

	public int[] world4Locks = new int[6];

	public int[] world5Locks = new int[6];

	public int worldMapTracker = 1;

	public int[] world1Moneys = new int[6];

	public int[] world2Moneys = new int[6];

	public int[] world3Moneys = new int[6];

	public int[] world4Moneys = new int[6];

	public int[] world5Moneys = new int[6];

	public int[] world1Scores = new int[6] { -1, -1, -1, -1, -1, -1 };

	public int[] world2Scores = new int[6] { -1, -1, -1, -1, -1, -1 };

	public int[] world3Scores = new int[6] { -1, -1, -1, -1, -1, -1 };

	public int[] world4Scores = new int[6] { -1, -1, -1, -1, -1, -1 };

	public int[] world5Scores = new int[6] { -1, -1, -1, -1, -1, -1 };

	public float introScrolly = 300f;

	public Texture2D introScroll;

	public Texture2D introScroll2;

	public Texture2D PressToSkip;

	public Texture2D world2scroll;

	public Texture2D world2scroll2;

	public Texture2D world3scroll;

	public Texture2D world3scroll2;

	public Texture2D world4scroll;

	public Texture2D world4scroll2;

	public Texture2D world5scroll;

	public Texture2D world5scroll2;

	public float exitScrolly = 300f;

	public Texture2D exitScroll;

	public Texture2D exitScroll2;

	public Texture2D exitScrollMoney;

	public Texture2D exitScrollMoney2;

	public int blowuptimer = 0;

	public int pauseblowuptimer = 0;

	public int confirmblowuptimer = 0;

	public int worldNumber;

	public int Deaths = 0;

	public int Score = 0;

	public bool resetWorldMapTracker = false;

	public Texture2D worldMapDirectory;

	public Texture2D trialDirectory;

	public Texture2D world1;

	public Texture2D world2;

	public Texture2D world3;

	public Texture2D world4;

	public Texture2D world5;

	public Texture2D selected;

	public Texture2D completed;

	public Texture2D money;

	public Texture2D locked;

	public Texture2D loading;

	public Texture2D controls;

	public bool firstLoad = true;

	public bool justLoaded;

	public bool firstgame = true;

	public string currentWorld;

	public bool prevWorldComplete = false;

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private Texture2D pressToContinue;

	private Texture2D startMenu;

	private Texture2D startMenuOptions;

	private Texture2D startMenuOptionsTrial;

	private Texture2D creditsPage1;

	private Texture2D creditsPage2;

	private bool creditsNextPage = false;

	private int worldMapDirectoryTracker;

	private int startMenuTracker;

	private int[] worldMapDirectoryTrackerX = new int[5] { 313, 453, 593, 733, 873 };

	private int[] worldMapDirectoryTrackerY = new int[5] { 312, 312, 312, 312, 312 };

	private int[] worldMapTrackerX = new int[8] { 113, 253, 393, 533, 673, 813, 953, 1093 };

	private int[] worldMapTrackerY = new int[8] { 312, 312, 312, 312, 312, 312, 312, 312 };

	private float enterTimer = 30f;

	private Rectangle creditRectangle = new Rectangle(0, 800, 1280, 1440);

	public Texture2D backplate;

	private int trialDirectoryTracker;

	private Texture2D pauseMenu;

	private Texture2D pauseMenuTrial;

	private int pauseMenuTracker;

	private int[] pauseMenuTrialX = new int[5] { 770, 770, 770, 770, 770 };

	private int[] pauseMenuTrialY = new int[5] { 345, 375, 405, 435, 465 };

	private int[] pauseMenuX = new int[4] { 770, 770, 770, 770 };

	private int[] pauseMenuY = new int[4] { 345, 375, 405, 435 };

	private string controlsGoto = "WorldMap";

	public int cutsceneTracker;

	public Texture2D cutsceneTexture;

	public string[] cutscenes;

	public int[] timers;

	public string currentGoTo;

	public Texture2D blackout;

	public int cutsceneStart;

	public bool cutsceneInitialize;

	public bool cutsceneSkip;

	public long cutsceneTimer;

	private SpriteFont hudFont;

	private Texture2D winOverlay;

	private Texture2D winMoneyOverlay;

	private int levelIndex = -1;

	private Level level;

	private bool wasContinuePressed;

	private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30.0);

	private GamePadState gamePadState;

	private KeyboardState keyboardState;

	private KeyboardState previouskeyboardState;

	private GamePadState previousgamepadState;

	public PlatformerGame()
	{
		graphics = new GraphicsDeviceManager(this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		base.Content.RootDirectory = "Content";
		base.Components.Add(new GamerServicesComponent(this));
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		hudFont = base.Content.Load<SpriteFont>("Fonts/PlatformerFromHellFont");
		winOverlay = base.Content.Load<Texture2D>("Overlays/you_win_xbox");
		winMoneyOverlay = base.Content.Load<Texture2D>("Overlays/you_win_money_xbox");
		startMenu = base.Content.Load<Texture2D>("Menus/StartMenu");
		startMenuOptions = base.Content.Load<Texture2D>("Menus/StartMenuOptions");
		startMenuOptionsTrial = base.Content.Load<Texture2D>("Menus/StartMenuOptionsTrial");
		pauseMenu = base.Content.Load<Texture2D>("Menus/Paused");
		pauseMenuTrial = base.Content.Load<Texture2D>("Menus/PauseTrial");
		creditsPage1 = base.Content.Load<Texture2D>("Menus/credits_page1");
		creditsPage2 = base.Content.Load<Texture2D>("Menus/credits_page2");
		loading = base.Content.Load<Texture2D>("Menus/loading");
		blackout = base.Content.Load<Texture2D>("Menus/blackout");
		confirm = base.Content.Load<Texture2D>("Menus/Confirm");
		introPart1 = base.Content.Load<SoundEffect>("Sounds/introPart1");
		introPart1Instance = introPart1.CreateInstance();
		introPart1Instance.Volume = introPart1Volume;
		introPart2 = base.Content.Load<SoundEffect>("Sounds/introPart2");
		introPart2Instance = introPart2.CreateInstance();
		introPart2Instance.Volume = introPart2Volume;
		startMenuClick = base.Content.Load<SoundEffect>("Sounds/startMenuClick");
		startMenuClickInstance = startMenuClick.CreateInstance();
		startMenuClickInstance.Volume = startMenuClickVolume;
		startGame = base.Content.Load<SoundEffect>("Sounds/startGame");
		startGameInstance = startGame.CreateInstance();
		startGameInstance.Volume = startGameVolume;
		noaccess = base.Content.Load<SoundEffect>("Sounds/noaccess");
		noaccessInstance = noaccess.CreateInstance();
		noaccessInstance.Volume = noaccessVolume;
		chaChing = base.Content.Load<SoundEffect>("Sounds/chaChing");
		chaChingInstance = chaChing.CreateInstance();
		chaChingInstance.Volume = chaChingVolume;
		PressToSkip = base.Content.Load<Texture2D>("Menus/PressToSkip");
		pressToContinue = base.Content.Load<Texture2D>("Menus/PressToContinueXbox");
		controls = base.Content.Load<Texture2D>("Menus/controls");
		backplate = base.Content.Load<Texture2D>("Menus/backplate");
		runAnimation = new Animation(base.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, isLooping: true);
		headshotAnimation = new Animation(base.Content.Load<Texture2D>("Sprites/Player/Headshot_menu"), 0.1f, isLooping: false);
		loadAnimation = new Animation(base.Content.Load<Texture2D>("Sprites/Player/burn"), 0.1f, isLooping: true);
		sprite.PlayAnimation(runAnimation);
		worldMapDirectory = base.Content.Load<Texture2D>("Menus/worldmapdirectory");
		trialDirectory = base.Content.Load<Texture2D>("Menus/worldmapdirectory");
		world1 = base.Content.Load<Texture2D>("Menus/world1");
		world2 = base.Content.Load<Texture2D>("Menus/world2");
		world3 = base.Content.Load<Texture2D>("Menus/world3");
		world4 = base.Content.Load<Texture2D>("Menus/world4");
		world5 = base.Content.Load<Texture2D>("Menus/world5");
		completed = base.Content.Load<Texture2D>("Menus/complete");
		selected = base.Content.Load<Texture2D>("Menus/selected");
		locked = base.Content.Load<Texture2D>("Menus/locked");
		money = base.Content.Load<Texture2D>("Menus/money");
		world2scroll = base.Content.Load<Texture2D>("Menus/World2Scroll");
		world2scroll2 = base.Content.Load<Texture2D>("Menus/World2Scroll2");
		world3scroll = base.Content.Load<Texture2D>("Menus/World3Scroll");
		world3scroll2 = base.Content.Load<Texture2D>("Menus/World3Scroll2");
		world4scroll = base.Content.Load<Texture2D>("Menus/World4Scroll");
		world4scroll2 = base.Content.Load<Texture2D>("Menus/World4Scroll2");
		world5scroll = base.Content.Load<Texture2D>("Menus/World5Scroll");
		world5scroll2 = base.Content.Load<Texture2D>("Menus/World5Scroll2");
		exitScroll = base.Content.Load<Texture2D>("Menus/exitScroll");
		exitScroll2 = base.Content.Load<Texture2D>("Menus/exitScroll2");
		exitScrollMoney = base.Content.Load<Texture2D>("Menus/exitScrollMoney");
		exitScrollMoney2 = base.Content.Load<Texture2D>("Menus/exitScrollMoney2");
		cutscenes = new string[2] { "hoosiergames", "uno" };
		timers = new int[2] { 4000, 6000 };
		currentGoTo = "PressToStart";
		cutsceneInitialize = true;
		GameState = GameStates.Cutscene;
		saveData = new SaveGameData();
		saveData.Score = 400;
		saveData.Deaths = 2;
		loadedData = null;
	}

	public static void StartVibration(long time)
	{
		GamePad.SetVibration(playerIndex, 1f, 1f);
		vibrateTimer = time;
	}

	public static void EndVibration()
	{
		GamePad.SetVibration(playerIndex, 0f, 0f);
		vibrateTimer = -1L;
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		HandleInput(gameTime);
		if (vibrateTimer > 0)
		{
			vibrateTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		else if (vibrateTimer <= 0 && vibrateTimer != -1)
		{
			EndVibration();
		}
		if (GameState != GameStates.Normal && vibrateTimer != -1)
		{
			EndVibration();
		}
		if (resetWorldMapTracker)
		{
			worldMapTracker = 1;
			resetWorldMapTracker = false;
		}
		if (cutsceneInitialize)
		{
			cutsceneStart = (int)gameTime.TotalGameTime.TotalMilliseconds;
			cutsceneInitialize = false;
		}
		if (GameState == GameStates.Normal)
		{
			if (!base.IsActive)
			{
				MediaPlayer.Volume = 0f;
				GameState = GameStates.Paused;
			}
			if (firstLoad)
			{
				try
				{
					MediaPlayer.IsRepeating = true;
					if (worldNumber == 1)
					{
						MediaPlayer.Volume = world1Volume;
					}
					else if (worldNumber == 2)
					{
						MediaPlayer.Volume = world2Volume;
					}
					else if (worldNumber == 3)
					{
						MediaPlayer.Volume = world3Volume;
					}
					else if (worldNumber == 4)
					{
						MediaPlayer.Volume = world4Volume;
					}
					else
					{
						MediaPlayer.Volume = world5Volume;
					}
					string assetName = "Sounds/" + worldNumber + "/Music";
					MediaPlayer.Play(base.Content.Load<Song>(assetName));
					firstLoad = false;
				}
				catch
				{
				}
			}
			level.Update(gameTime, keyboardState, gamePadState);
			Score = level.Score;
			Deaths = level.deaths;
		}
		else if (GameState == GameStates.Cutscene)
		{
			if (cutsceneTracker == 0)
			{
				if (firstLoad)
				{
					introPart1Instance.Play();
					firstLoad = false;
				}
			}
			else if (cutsceneTracker == 1 && firstLoad)
			{
				introPart2Instance.Play();
				firstLoad = false;
			}
			if (cutsceneTracker < timers.Length)
			{
				if (cutsceneTimer >= timers[cutsceneTracker])
				{
					cutsceneStart = (int)gameTime.TotalGameTime.TotalMilliseconds;
					cutsceneTracker++;
					firstLoad = true;
				}
				else if (!cutsceneSkip)
				{
					cutsceneTexture = base.Content.Load<Texture2D>("Menus/Cutscenes/" + cutscenes[cutsceneTracker]);
				}
			}
			cutsceneTimer = (int)gameTime.TotalGameTime.TotalMilliseconds - cutsceneStart;
		}
		else
		{
			if (GameState == GameStates.Credits)
			{
				if (!firstLoad)
				{
					return;
				}
				try
				{
					MediaPlayer.IsRepeating = true;
					if (beatTheGame)
					{
						MediaPlayer.Volume = beatTheGameVolume;
						MediaPlayer.Play(base.Content.Load<Song>("Sounds/BeatTheGame"));
					}
					else
					{
						MediaPlayer.Volume = creditsVolume;
						MediaPlayer.Play(base.Content.Load<Song>("Sounds/Credits"));
					}
					firstLoad = false;
					return;
				}
				catch
				{
					return;
				}
			}
			if (GameState == GameStates.ExitScroll)
			{
				if (firstLoad)
				{
					try
					{
						MediaPlayer.IsRepeating = true;
						MediaPlayer.Volume = beatTheGameVolume;
						MediaPlayer.Play(base.Content.Load<Song>("Sounds/BeatTheGame"));
						firstLoad = false;
					}
					catch
					{
					}
				}
				if (exitScrolly > -1700f)
				{
					exitScrolly -= 0.75f;
					return;
				}
				GameState = GameStates.Credits;
				creditsNextPage = false;
			}
			else if (GameState == GameStates.PressToStart || GameState == GameStates.StartMenu)
			{
				if (firstLoad)
				{
					try
					{
						MediaPlayer.IsRepeating = true;
						MediaPlayer.Volume = startMenuVolume;
						MediaPlayer.Play(base.Content.Load<Song>("Sounds/StartMenuMusic"));
						firstLoad = false;
					}
					catch
					{
					}
				}
			}
			else if (GameState == GameStates.IntroScroll)
			{
				if (firstLoad)
				{
					try
					{
						MediaPlayer.IsRepeating = false;
						MediaPlayer.Volume = startMenuVolume;
						MediaPlayer.Play(base.Content.Load<Song>("Sounds/IntroScroll"));
						firstLoad = false;
						donttryagain = true;
					}
					catch
					{
					}
				}
				if (MediaPlayer.State == MediaState.Stopped && donttryagain)
				{
				}
			}
			else if (GameState == GameStates.Controls)
			{
				if (firstLoad)
				{
					try
					{
						MediaPlayer.IsRepeating = true;
						MediaPlayer.Volume = controlsVolume;
						MediaPlayer.Play(base.Content.Load<Song>("Sounds/ControlsMusic"));
						firstLoad = false;
					}
					catch
					{
					}
				}
			}
			else if ((GameState == GameStates.WorldMapDirectory || GameState == GameStates.WorldMap) && firstLoad)
			{
				MediaPlayer.Stop();
				try
				{
					MediaPlayer.IsRepeating = true;
					MediaPlayer.Volume = worldMapVolume;
					MediaPlayer.Play(base.Content.Load<Song>("Sounds/worldMapMusic"));
					firstLoad = false;
				}
				catch
				{
				}
			}
		}
	}

	private void SaveData()
	{
		if (!Global.SaveDevice.IsReady)
		{
			return;
		}
		Global.SaveDevice.SaveAsync(Global.containerName, Global.fileName_options, delegate(Stream stream)
		{
			using (new StreamWriter(stream))
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
				xmlSerializer.Serialize(stream, saveData);
			}
		});
	}

	public void assignLoadedData(SaveGameData loadedData)
	{
		justLoaded = true;
		Score = loadedData.Score;
		Deaths = loadedData.Deaths;
		world1Locks = loadedData.world1Locks;
		world2Locks = loadedData.world2Locks;
		world3Locks = loadedData.world3Locks;
		world4Locks = loadedData.world4Locks;
		world5Locks = loadedData.world5Locks;
		world1Moneys = loadedData.world1Moneys;
		world2Moneys = loadedData.world2Moneys;
		world3Moneys = loadedData.world3Moneys;
		world4Moneys = loadedData.world4Moneys;
		world5Moneys = loadedData.world5Moneys;
		world1Scores = loadedData.world1Scores;
		world2Scores = loadedData.world2Scores;
		world3Scores = loadedData.world3Scores;
		world4Scores = loadedData.world4Scores;
		world5Scores = loadedData.world5Scores;
		worldMapTracker = 1;
		worldMapDirectoryTracker = 0;
	}

	private void LoadData()
	{
		if (Global.SaveDevice.FileExists(Global.containerName, Global.fileName_options))
		{
			Global.SaveDevice.Load(Global.containerName, Global.fileName_options, delegate(Stream stream)
			{
				using (new StreamReader(stream))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
					loadedData = (SaveGameData)xmlSerializer.Deserialize(stream);
					assignLoadedData(loadedData);
				}
			});
		}
		else
		{
			firstgame = true;
		}
	}

	public void saveAllData()
	{
		saveData.Score = Score;
		saveData.Deaths = Deaths;
		saveData.world1Locks = world1Locks;
		saveData.world2Locks = world2Locks;
		saveData.world3Locks = world3Locks;
		saveData.world4Locks = world4Locks;
		saveData.world5Locks = world5Locks;
		saveData.world1Moneys = world1Moneys;
		saveData.world2Moneys = world2Moneys;
		saveData.world3Moneys = world3Moneys;
		saveData.world4Moneys = world4Moneys;
		saveData.world5Moneys = world5Moneys;
		saveData.world1Scores = world1Scores;
		saveData.world2Scores = world2Scores;
		saveData.world3Scores = world3Scores;
		saveData.world4Scores = world4Scores;
		saveData.world5Scores = world5Scores;
		SaveData();
	}

	public void nextWorld()
	{
		worldNumber++;
		worldMapTracker = 1;
		resetWorldMapTracker = true;
	}

	private void HandleInput(GameTime gameTime)
	{
		enterTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;
		unlockNextLevel();
		keyboardState = Keyboard.GetState();
		if (GameState != GameStates.PressToStart)
		{
			gamePadState = GamePad.GetState(playerIndex);
			if (!gamePadState.IsConnected)
			{
				GameState = GameStates.Paused;
			}
		}
		bool flag = keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Enter) || gamePadState.IsButtonDown(Buttons.A);
		bool flag2 = (keyboardState.IsKeyDown(Keys.Enter) && !previouskeyboardState.IsKeyDown(Keys.Enter)) || (gamePadState.IsButtonDown(Buttons.Start) && !previousgamepadState.IsButtonDown(Buttons.Start)) || (gamePadState.IsButtonDown(Buttons.A) && !previousgamepadState.IsButtonDown(Buttons.A));
		bool flag3 = (keyboardState.IsKeyDown(Keys.Up) && !previouskeyboardState.IsKeyDown(Keys.Up)) || (keyboardState.IsKeyDown(Keys.W) && !previouskeyboardState.IsKeyDown(Keys.W)) || (gamePadState.IsButtonDown(Buttons.DPadUp) && !previousgamepadState.IsButtonDown(Buttons.DPadUp)) || (gamePadState.ThumbSticks.Left.Y > 0f && previousgamepadState.ThumbSticks.Left.Y <= 0f);
		bool flag4 = (keyboardState.IsKeyDown(Keys.Space) && !previouskeyboardState.IsKeyDown(Keys.Space)) || (gamePadState.IsButtonDown(Buttons.A) && !previousgamepadState.IsButtonDown(Buttons.A));
		bool flag5 = (keyboardState.IsKeyDown(Keys.Down) && !previouskeyboardState.IsKeyDown(Keys.Down)) || (keyboardState.IsKeyDown(Keys.S) && !previouskeyboardState.IsKeyDown(Keys.S)) || (gamePadState.IsButtonDown(Buttons.DPadDown) && !previousgamepadState.IsButtonDown(Buttons.DPadDown)) || (gamePadState.ThumbSticks.Left.Y < 0f && previousgamepadState.ThumbSticks.Left.Y >= 0f);
		bool flag6 = (keyboardState.IsKeyDown(Keys.Left) && !previouskeyboardState.IsKeyDown(Keys.Left)) || (keyboardState.IsKeyDown(Keys.A) && !previouskeyboardState.IsKeyDown(Keys.A)) || (gamePadState.IsButtonDown(Buttons.DPadLeft) && !previousgamepadState.IsButtonDown(Buttons.DPadLeft)) || (gamePadState.ThumbSticks.Left.X < 0f && previousgamepadState.ThumbSticks.Left.X >= 0f);
		bool flag7 = (keyboardState.IsKeyDown(Keys.Right) && !previouskeyboardState.IsKeyDown(Keys.Right)) || (keyboardState.IsKeyDown(Keys.D) && !previouskeyboardState.IsKeyDown(Keys.D)) || (gamePadState.IsButtonDown(Buttons.DPadRight) && !previousgamepadState.IsButtonDown(Buttons.DPadRight)) || (gamePadState.ThumbSticks.Left.X > 0f && previousgamepadState.ThumbSticks.Left.X <= 0f);
		bool flag8 = (keyboardState.IsKeyDown(Keys.Escape) && !previouskeyboardState.IsKeyDown(Keys.Escape)) || (gamePadState.IsButtonDown(Buttons.B) && !previousgamepadState.IsButtonDown(Buttons.B));
		if (GameState == GameStates.StartMenu)
		{
			if (startMenuFadeInTimer > 0)
			{
				startMenuFadeInTimer -= gameTime.ElapsedGameTime.Milliseconds;
			}
			blowuptimer--;
			if (blowuptimer <= 0)
			{
				if (flag3)
				{
					startMenuTracker--;
					if (startMenuTracker < 0)
					{
						startMenuTracker = 4;
					}
					startMenuClickInstance.Play();
				}
				if (flag5)
				{
					startMenuTracker++;
					if (startMenuTracker > 4)
					{
						startMenuTracker = 0;
					}
					startMenuClickInstance.Play();
				}
			}
			if ((flag2 || flag4) && blowuptimer <= 0)
			{
				blowuptimer = 100;
			}
			if (blowuptimer == 0)
			{
				if (startMenuTracker == 0)
				{
					if (Guide.IsTrialMode)
					{
						Deaths = 0;
						Score = 0;
						justLoaded = true;
						trialDirectoryTracker = 0;
						MediaPlayer.Stop();
						firstLoad = true;
						introScrolly = 725f;
						introScroll = base.Content.Load<Texture2D>("Menus/introscroll");
						introScroll2 = base.Content.Load<Texture2D>("Menus/introScroll2");
						GameState = GameStates.IntroScroll;
						startGameInstance.Play();
					}
					else if (Gamer.SignedInGamers[playerIndex] == null)
					{
						if (!Guide.IsVisible)
						{
							Guide.ShowSignIn(1, onlineOnly: false);
						}
					}
					else
					{
						MediaPlayer.Stop();
						firstLoad = true;
						firstgame = false;
						LoadData();
						GameState = GameStates.WorldMapDirectory;
						startGameInstance.Play();
					}
				}
				if (startMenuTracker == 1)
				{
					if (Guide.IsTrialMode)
					{
						if (Gamer.SignedInGamers[playerIndex] == null)
						{
							if (!Guide.IsVisible)
							{
								Guide.ShowSignIn(1, onlineOnly: false);
							}
						}
						else if (Gamer.SignedInGamers[playerIndex].Privileges.AllowPurchaseContent)
						{
							Guide.ShowMarketplace(playerIndex);
						}
						else
						{
							noaccessInstance.Play();
						}
					}
					else
					{
						Deaths = 0;
						Score = 0;
						justLoaded = true;
						worldMapTracker = 1;
						worldMapDirectoryTracker = 0;
						world1Locks = new int[6] { 1, 1, 1, 1, 1, 0 };
						int[] array = new int[6];
						world2Locks = array;
						array = new int[6];
						world3Locks = array;
						array = new int[6];
						world4Locks = array;
						array = new int[6];
						world5Locks = array;
						array = new int[6];
						world1Moneys = array;
						array = new int[6];
						world2Moneys = array;
						array = new int[6];
						world3Moneys = array;
						array = new int[6];
						world4Moneys = array;
						array = new int[6];
						world5Moneys = array;
						world1Scores = new int[6] { -1, -1, -1, -1, -1, -1 };
						world2Scores = new int[6] { -1, -1, -1, -1, -1, -1 };
						world3Scores = new int[6] { -1, -1, -1, -1, -1, -1 };
						world4Scores = new int[6] { -1, -1, -1, -1, -1, -1 };
						world5Scores = new int[6] { -1, -1, -1, -1, -1, -1 };
						MediaPlayer.Stop();
						firstLoad = true;
						introScrolly = 725f;
						introScroll = base.Content.Load<Texture2D>("Menus/introscroll");
						introScroll2 = base.Content.Load<Texture2D>("Menus/introScroll2");
						GameState = GameStates.IntroScroll;
						startGameInstance.Play();
					}
				}
				if (startMenuTracker == 2)
				{
					MediaPlayer.Stop();
					firstLoad = true;
					GameState = GameStates.Controls;
					controlsGoto = "StartMenu";
				}
				if (startMenuTracker == 3)
				{
					MediaPlayer.Stop();
					firstLoad = true;
					GameState = GameStates.Credits;
					creditsNextPage = false;
				}
				if (startMenuTracker == 4)
				{
					GameState = GameStates.Confirm;
					ConfirmationString = "Exit";
					toConfirm = "StartMenu";
				}
				startMenuTracker = 0;
			}
		}
		else if (GameState == GameStates.IntroScroll)
		{
			if (flag2 || flag4)
			{
				MediaPlayer.Stop();
				firstLoad = true;
				if (Guide.IsTrialMode)
				{
					GameState = GameStates.TrialDirectory;
				}
				else
				{
					GameState = GameStates.WorldMapDirectory;
				}
			}
			int num = -1800;
			num = -2200;
			if (introScrolly > (float)num)
			{
				introScrolly -= 0.95f;
			}
			else
			{
				MediaPlayer.Stop();
				firstLoad = true;
				if (Guide.IsTrialMode)
				{
					GameState = GameStates.TrialDirectory;
				}
				else
				{
					GameState = GameStates.WorldMapDirectory;
				}
			}
		}
		else if (GameState == GameStates.ExitScroll)
		{
			if (flag2 || flag4)
			{
				GameState = GameStates.Credits;
				creditsNextPage = false;
			}
		}
		else if (GameState != GameStates.Cutscene)
		{
			if (GameState == GameStates.Credits)
			{
				if (flag2 || flag4)
				{
					if (creditsNextPage)
					{
						GameState = GameStates.StartMenu;
						MediaPlayer.Stop();
						firstLoad = true;
						enterTimer = 0f;
					}
					else
					{
						creditsNextPage = true;
					}
				}
			}
			else if (GameState == GameStates.Controls)
			{
				if (flag2 || flag4)
				{
					MediaPlayer.Stop();
					firstLoad = true;
					if (controlsGoto == "Normal")
					{
						GameState = GameStates.Normal;
					}
					else if (controlsGoto == "StartMenu")
					{
						GameState = GameStates.StartMenu;
					}
				}
			}
			else if (GameState == GameStates.WorldMapDirectory)
			{
				if (flag8)
				{
					GameState = GameStates.StartMenu;
					firstLoad = true;
				}
				if (flag6)
				{
					worldMapDirectoryTracker--;
					if (worldMapDirectoryTracker < 0)
					{
						worldMapDirectoryTracker = 0;
					}
				}
				if (flag7)
				{
					worldMapDirectoryTracker++;
					if (worldMapDirectoryTracker > 4)
					{
						worldMapDirectoryTracker = 4;
					}
					if (worldMapDirectoryTracker == 1 && count(world2Locks) < 1)
					{
						worldMapDirectoryTracker--;
						noaccessInstance.Play();
					}
					if (worldMapDirectoryTracker == 2 && count(world3Locks) < 1)
					{
						worldMapDirectoryTracker--;
						noaccessInstance.Play();
					}
					if (worldMapDirectoryTracker == 3 && count(world4Locks) < 1)
					{
						worldMapDirectoryTracker--;
						noaccessInstance.Play();
					}
					if (worldMapDirectoryTracker == 4 && count(world5Locks) < 1)
					{
						worldMapDirectoryTracker--;
						noaccessInstance.Play();
					}
				}
				if ((flag2 || flag4) && ((worldMapDirectoryTracker == 0 && count(world1Locks) >= 1) || (worldMapDirectoryTracker == 1 && count(world2Locks) >= 1) || (worldMapDirectoryTracker == 2 && count(world3Locks) >= 1) || (worldMapDirectoryTracker == 3 && count(world4Locks) >= 1) || (worldMapDirectoryTracker == 4 && count(world5Locks) >= 1)))
				{
					GameState = GameStates.WorldMap;
					worldMapTracker = 1;
					worldNumber = worldMapDirectoryTracker + 1;
				}
			}
			else if (GameState == GameStates.TrialDirectory)
			{
				if (flag8)
				{
					GameState = GameStates.StartMenu;
					firstLoad = true;
				}
				if (flag6)
				{
					trialDirectoryTracker--;
					if (trialDirectoryTracker < 0)
					{
						trialDirectoryTracker = 0;
					}
				}
				if (flag7)
				{
					trialDirectoryTracker++;
					if (trialDirectoryTracker > 4)
					{
						trialDirectoryTracker = 4;
					}
				}
				if (flag2 || flag4)
				{
					worldNumber = trialDirectoryTracker + 1;
					worldMapTracker = trialLevels[trialDirectoryTracker] + 1;
					GameState = GameStates.Loading;
					testTimer = 100;
					Draw(gameTime);
				}
			}
			else if (GameState == GameStates.WorldMap)
			{
				if (level != null)
				{
					level.Dispose();
				}
				if (flag6)
				{
					worldMapTracker--;
					if (worldMapTracker < 0)
					{
						worldMapTracker = 0;
					}
					if (worldMapTracker == 6)
					{
						if (worldNumber == 1 && world1Locks[5] == 0)
						{
							worldMapTracker = 5;
						}
						if (worldNumber == 2 && world2Locks[5] == 0)
						{
							worldMapTracker = 5;
						}
						if (worldNumber == 3 && world3Locks[5] == 0)
						{
							worldMapTracker = 5;
						}
						if (worldNumber == 4 && world4Locks[5] == 0)
						{
							worldMapTracker = 5;
						}
						if (worldNumber == 5 && world5Locks[5] == 0)
						{
							worldMapTracker = 5;
						}
					}
				}
				if (flag7)
				{
					worldMapTracker++;
					if (worldMapTracker > 7)
					{
						worldMapTracker = 7;
					}
					if (worldMapTracker == 6)
					{
						if (worldNumber == 1 && world1Locks[5] == 0)
						{
							worldMapTracker = 7;
						}
						if (worldNumber == 2 && world2Locks[5] == 0)
						{
							worldMapTracker = 7;
						}
						if (worldNumber == 3 && world3Locks[5] == 0)
						{
							worldMapTracker = 7;
						}
						if (worldNumber == 4 && world4Locks[5] == 0)
						{
							worldMapTracker = 7;
						}
						if (worldNumber == 5 && world5Locks[5] == 0)
						{
							worldMapTracker = 7;
						}
					}
				}
				if (flag8)
				{
					GameState = GameStates.WorldMapDirectory;
				}
				if (flag2 || flag4)
				{
					if (worldMapTracker == 0)
					{
						if (worldNumber == 1)
						{
							GameState = GameStates.WorldMapDirectory;
							worldMapDirectoryTracker = 0;
						}
						else
						{
							worldNumber--;
							worldMapTracker = 1;
						}
					}
					else if (worldMapTracker == 7)
					{
						if (worldNumber == 5)
						{
							MediaPlayer.Stop();
							firstLoad = true;
							beatTheGame = true;
							exitScrolly = 185f;
							GameState = GameStates.ExitScroll;
						}
						else if (worldNumber == 1)
						{
							if (count(world1Scores) >= 3)
							{
								MediaPlayer.Stop();
								firstLoad = true;
								donttryagain = false;
								introScrolly = 215f;
								introScroll = world2scroll;
								introScroll2 = world2scroll2;
								GameState = GameStates.IntroScroll;
								worldMapDirectoryTracker++;
							}
							else
							{
								noaccessInstance.Play();
							}
						}
						else if (worldNumber == 2)
						{
							if (count(world2Scores) >= 3)
							{
								MediaPlayer.Stop();
								firstLoad = true;
								donttryagain = false;
								introScrolly = 390f;
								introScroll = world3scroll;
								introScroll2 = world3scroll2;
								GameState = GameStates.IntroScroll;
								worldMapDirectoryTracker++;
							}
							else
							{
								noaccessInstance.Play();
							}
						}
						else if (worldNumber == 3)
						{
							if (count(world3Scores) >= 3)
							{
								MediaPlayer.Stop();
								firstLoad = true;
								donttryagain = false;
								introScrolly = 515f;
								introScroll = world4scroll;
								introScroll2 = world4scroll2;
								GameState = GameStates.IntroScroll;
								worldMapDirectoryTracker++;
							}
							else
							{
								noaccessInstance.Play();
							}
						}
						else if (worldNumber == 4)
						{
							if (count(world4Scores) >= 3)
							{
								MediaPlayer.Stop();
								firstLoad = true;
								donttryagain = false;
								introScrolly = 720f;
								introScroll = world5scroll;
								introScroll2 = world5scroll2;
								GameState = GameStates.IntroScroll;
								worldMapDirectoryTracker++;
							}
							else
							{
								noaccessInstance.Play();
							}
						}
					}
					else
					{
						GameState = GameStates.Loading;
						testTimer = 100;
						Draw(gameTime);
					}
				}
			}
			else if (GameState == GameStates.Paused)
			{
				if (pauseblowuptimer > -5)
				{
					pauseblowuptimer--;
				}
				if (pauseblowuptimer <= 0)
				{
					if (flag3)
					{
						pauseMenuTracker--;
						if (Guide.IsTrialMode)
						{
							if (pauseMenuTracker < 0)
							{
								pauseMenuTracker = pauseMenuTrialX.Length - 1;
							}
						}
						else if (pauseMenuTracker < 0)
						{
							pauseMenuTracker = pauseMenuX.Length - 1;
						}
					}
					if (flag5)
					{
						pauseMenuTracker++;
						if (Guide.IsTrialMode)
						{
							if (pauseMenuTracker > pauseMenuTrialX.Length - 1)
							{
								pauseMenuTracker = 0;
							}
						}
						else if (pauseMenuTracker > pauseMenuX.Length - 1)
						{
							pauseMenuTracker = 0;
						}
					}
					if (flag8)
					{
						adjustWorldVolume();
						MediaPlayer.IsRepeating = true;
						GameState = GameStates.Normal;
						pauseMenuTracker = 0;
					}
				}
				else if (pauseblowuptimer == 1)
				{
					if (pauseMenuTracker == 0)
					{
						adjustWorldVolume();
						MediaPlayer.IsRepeating = true;
						GameState = GameStates.Normal;
					}
					else if (pauseMenuTracker == 1)
					{
						if (Guide.IsTrialMode)
						{
							if (Gamer.SignedInGamers[playerIndex] == null)
							{
								if (!Guide.IsVisible)
								{
									Guide.ShowSignIn(1, onlineOnly: false);
								}
							}
							else if (Gamer.SignedInGamers[playerIndex].Privileges.AllowPurchaseContent)
							{
								Guide.ShowMarketplace(playerIndex);
							}
							else
							{
								noaccessInstance.Play();
							}
						}
						else
						{
							MediaPlayer.Stop();
							firstLoad = true;
							GameState = GameStates.WorldMap;
						}
					}
					else if (pauseMenuTracker == 2)
					{
						if (Guide.IsTrialMode)
						{
							MediaPlayer.Stop();
							firstLoad = true;
							GameState = GameStates.TrialDirectory;
						}
						else
						{
							firstLoad = true;
							MediaPlayer.Stop();
							GameState = GameStates.Confirm;
							ConfirmationString = "StartMenu";
							toConfirm = "Pause";
						}
					}
					else if (pauseMenuTracker == 3)
					{
						if (Guide.IsTrialMode)
						{
							firstLoad = true;
							MediaPlayer.Stop();
							GameState = GameStates.Confirm;
							ConfirmationString = "StartMenu";
							toConfirm = "Pause";
						}
						else
						{
							GameState = GameStates.Confirm;
							ConfirmationString = "Exit";
							toConfirm = "Pause";
						}
					}
					else if (pauseMenuTracker == 4)
					{
						GameState = GameStates.Confirm;
						ConfirmationString = "Exit";
						toConfirm = "Pause";
					}
					pauseMenuTracker = 0;
				}
				if (flag2 || flag4)
				{
					pauseblowuptimer = 60;
				}
			}
			else if (GameState == GameStates.Confirm)
			{
				if (confirmblowuptimer > -5)
				{
					confirmblowuptimer--;
				}
				if (confirmblowuptimer <= 0)
				{
					if (flag3)
					{
						confirmTracker--;
						if (confirmTracker < 0)
						{
							confirmTracker = 0;
						}
					}
					if (flag5)
					{
						confirmTracker++;
						if (confirmTracker > 1)
						{
							confirmTracker = 1;
						}
					}
					if (flag8)
					{
						if (toConfirm == "Pause")
						{
							GameState = GameStates.Paused;
						}
						if (toConfirm == "StartMenu")
						{
							GameState = GameStates.StartMenu;
						}
						confirmTracker = 0;
					}
				}
				else if (confirmblowuptimer == 1)
				{
					if (confirmTracker == 0)
					{
						if (toConfirm == "Pause")
						{
							GameState = GameStates.Paused;
						}
						if (toConfirm == "StartMenu")
						{
							GameState = GameStates.StartMenu;
						}
					}
					else if (confirmTracker == 1)
					{
						if (ConfirmationString == "Exit")
						{
							Close();
						}
						else if (ConfirmationString == "StartMenu")
						{
							GameState = GameStates.StartMenu;
							level.Dispose();
							confirmTracker = 0;
						}
					}
				}
				if (flag2 || flag4)
				{
					confirmblowuptimer = 60;
				}
			}
			else if (GameState == GameStates.Normal)
			{
				if (testTimer <= 0)
				{
					if (level.deathTimer > 30)
					{
						ReloadCurrentLevel(level.deaths, level.TimeRemaining);
						level.StartNewLife();
					}
					if (level.ReachedExit)
					{
						MediaPlayer.Stop();
						if (level.Player.victoryDanceInstance.State != SoundState.Playing)
						{
							level.Player.victoryDanceInstance.Play();
						}
					}
					if (flag8 || (gamePadState.IsButtonDown(Buttons.Start) && !previousgamepadState.IsButtonDown(Buttons.Start)))
					{
						level.Player.pausedSoundInstance.Play();
						GameState = GameStates.Paused;
						controlsGoto = "Normal";
					}
					if (!wasContinuePressed && flag && level.TimeRemaining == TimeSpan.Zero)
					{
						if (level.ReachedExit)
						{
							if (level.Player.victoryDanceInstance.State == SoundState.Playing)
							{
								level.Player.victoryDanceInstance.Stop();
							}
							firstLoad = true;
							moneyUnlock(worldNumber);
							storeScore(level.Score - level.testScore, level.worldNumber, level.levelNumber);
							if (Guide.IsTrialMode)
							{
								GameState = GameStates.TrialDirectory;
							}
							else
							{
								GameState = GameStates.WorldMap;
							}
							if (Gamer.SignedInGamers[playerIndex] == null)
							{
								if (!Guide.IsVisible)
								{
									Guide.ShowSignIn(1, onlineOnly: false);
								}
							}
							else
							{
								saveAllData();
							}
						}
						else
						{
							ReloadCurrentLevel(level.deaths, level.TimeRemaining);
						}
					}
				}
			}
			else if (GameState == GameStates.PressToStart)
			{
				if (keyboardState.GetPressedKeys().Length > 0 && previouskeyboardState.GetPressedKeys().Length == 0)
				{
					startMenuFadeInTimer = 1000L;
					GameState = GameStates.StartMenu;
				}
				if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
				{
					previousgamepadState = GamePad.GetState(PlayerIndex.One);
					gamePadState = GamePad.GetState(PlayerIndex.One);
					playerIndex = PlayerIndex.One;
					startMenuFadeInTimer = 1000L;
					GameState = GameStates.StartMenu;
				}
				if (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.A))
				{
					previousgamepadState = GamePad.GetState(PlayerIndex.Two);
					gamePadState = GamePad.GetState(PlayerIndex.Two);
					playerIndex = PlayerIndex.Two;
					startMenuFadeInTimer = 1000L;
					GameState = GameStates.StartMenu;
				}
				if (GamePad.GetState(PlayerIndex.Three).IsButtonDown(Buttons.A))
				{
					previousgamepadState = GamePad.GetState(PlayerIndex.Three);
					gamePadState = GamePad.GetState(PlayerIndex.Three);
					playerIndex = PlayerIndex.Three;
					startMenuFadeInTimer = 1000L;
					GameState = GameStates.StartMenu;
				}
				if (GamePad.GetState(PlayerIndex.Four).IsButtonDown(Buttons.A))
				{
					previousgamepadState = GamePad.GetState(PlayerIndex.Four);
					gamePadState = GamePad.GetState(PlayerIndex.Four);
					playerIndex = PlayerIndex.Four;
					startMenuFadeInTimer = 1000L;
					GameState = GameStates.StartMenu;
				}
			}
		}
		previousgamepadState = gamePadState;
		previouskeyboardState = keyboardState;
		wasContinuePressed = flag;
	}

	public void Close()
	{
		Exit();
	}

	public void adjustWorldVolume()
	{
		if (worldNumber == 1)
		{
			MediaPlayer.Volume = world1Volume;
		}
		else if (worldNumber == 2)
		{
			MediaPlayer.Volume = world2Volume;
		}
		else if (worldNumber == 3)
		{
			MediaPlayer.Volume = world3Volume;
		}
		else if (worldNumber == 4)
		{
			MediaPlayer.Volume = world4Volume;
		}
		else if (worldNumber == 5)
		{
			MediaPlayer.Volume = world5Volume;
		}
	}

	public void storeScore(int score, int world, int level)
	{
		if (world == 1 && score > world1Scores[level])
		{
			world1Scores[level] = score;
		}
		if (world == 2 && score > world2Scores[level])
		{
			world2Scores[level] = score;
		}
		if (world == 3 && score > world3Scores[level])
		{
			world3Scores[level] = score;
		}
		if (world == 4 && score > world4Scores[level])
		{
			world4Scores[level] = score;
		}
		if (world == 5 && score > world5Scores[level])
		{
			world5Scores[level] = score;
		}
	}

	public int count(int[] array)
	{
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] >= 1)
			{
				num++;
			}
		}
		return num;
	}

	private void moneyUnlock(int worldNumber)
	{
		if (worldNumber == 1 && count(world1Moneys) >= 5)
		{
			world1Locks[5] = 1;
		}
		if (worldNumber == 2 && count(world2Moneys) >= 5)
		{
			world2Locks[5] = 1;
		}
		if (worldNumber == 3 && count(world3Moneys) >= 5)
		{
			world3Locks[5] = 1;
		}
		if (worldNumber == 4 && count(world4Moneys) >= 5)
		{
			world4Locks[5] = 1;
		}
		if (worldNumber == 5 && count(world5Moneys) >= 5)
		{
			world5Locks[5] = 1;
		}
	}

	public void unlockNextLevel()
	{
		if (worldNumber == 1 && count(world1Scores) >= 3)
		{
			world2Locks[0] = 1;
			world2Locks[1] = 1;
			world2Locks[2] = 1;
			world2Locks[3] = 1;
			world2Locks[4] = 1;
		}
		if (worldNumber == 2 && count(world2Scores) >= 3)
		{
			world3Locks[0] = 1;
			world3Locks[1] = 1;
			world3Locks[2] = 1;
			world3Locks[3] = 1;
			world3Locks[4] = 1;
		}
		if (worldNumber == 3 && count(world3Scores) >= 3)
		{
			world4Locks[0] = 1;
			world4Locks[1] = 1;
			world4Locks[2] = 1;
			world4Locks[3] = 1;
			world4Locks[4] = 1;
		}
		if (worldNumber == 4 && count(world4Scores) >= 3)
		{
			world5Locks[0] = 1;
			world5Locks[1] = 1;
			world5Locks[2] = 1;
			world5Locks[3] = 1;
			world5Locks[4] = 1;
		}
	}

	private void LoadNextLevel()
	{
		levelIndex++;
		loadNewLevel(worldNumber, levelIndex);
	}

	private void loadNewLevel(int worldNumber, int levelTracker)
	{
		GameState = GameStates.Loading;
		levelIndex = levelTracker;
		int points;
		int deaths;
		if (firstgame)
		{
			points = 0;
			deaths = 0;
			firstgame = false;
		}
		else if (justLoaded)
		{
			points = Score;
			deaths = Deaths;
		}
		else
		{
			points = level.Score;
			deaths = level.deaths;
		}
		if (level != null)
		{
			level.Dispose();
		}
		string text = $"Content/Levels/{worldNumber}/";
		text = text + levelIndex + ".txt";
		if (File.Exists(text))
		{
			using (TitleContainer.OpenStream(text))
			{
				level = new Level(base.Services, text, levelIndex, points, deaths, worldNumber, this);
				return;
			}
		}
		text = $"Content/Levels/world1/{0}.txt";
		levelIndex = 0;
		level = new Level(base.Services, text, levelIndex, points, deaths, worldNumber, this);
	}

	private void ReloadCurrentLevel(int deaths, TimeSpan timeLeft)
	{
		level.deathTimer = 0;
	}

	public bool hasMoney(int levelNumber)
	{
		if (worldNumber == 1)
		{
			return world1Moneys[levelNumber] == 1;
		}
		if (worldNumber == 2)
		{
			return world2Moneys[levelNumber] == 1;
		}
		if (worldNumber == 3)
		{
			return world3Moneys[levelNumber] == 1;
		}
		if (worldNumber == 4)
		{
			return world4Moneys[levelNumber] == 1;
		}
		return world5Moneys[levelNumber] == 1;
	}

	public int calculateTotalScore()
	{
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			if (world1Scores[i] > 0)
			{
				num += world1Scores[i];
			}
			if (world2Scores[i] > 0)
			{
				num += world2Scores[i];
			}
			if (world3Scores[i] > 0)
			{
				num += world3Scores[i];
			}
			if (world4Scores[i] > 0)
			{
				num += world4Scores[i];
			}
			if (world5Scores[i] > 0)
			{
				num += world5Scores[i];
			}
		}
		return num;
	}

	public void gotMoney()
	{
		if (worldNumber == 1)
		{
			world1Moneys[level.levelNumber] = 1;
		}
		if (worldNumber == 2)
		{
			world2Moneys[level.levelNumber] = 1;
		}
		if (worldNumber == 3)
		{
			world3Moneys[level.levelNumber] = 1;
		}
		if (worldNumber == 4)
		{
			world4Moneys[level.levelNumber] = 1;
		}
		if (worldNumber == 5)
		{
			world5Moneys[level.levelNumber] = 1;
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		graphics.GraphicsDevice.Clear(Color.Black);
		if (GameState == GameStates.Normal && testTimer <= 0)
		{
			level.Draw(gameTime, spriteBatch);
		}
		DrawHud(gameTime);
		base.Draw(gameTime);
	}

	public int countAllMoney()
	{
		int num = 0;
		num += count(world1Moneys);
		num += count(world2Moneys);
		num += count(world3Moneys);
		num += count(world4Moneys);
		return num + count(world5Moneys);
	}

	public void DrawHud(GameTime gameTime)
	{
		spriteBatch.Begin();
		Viewport viewport = base.GraphicsDevice.Viewport;
		Rectangle titleSafeArea = base.GraphicsDevice.Viewport.TitleSafeArea;
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(0f, 0f);
		Vector2 vector3 = new Vector2((float)viewport.X + (float)viewport.Width / 2f, (float)viewport.Y + (float)viewport.Height / 2f);
		if (GameState == GameStates.PressToStart)
		{
			if (gameTime.TotalGameTime.Seconds % 2 == 1)
			{
				spriteBatch.Draw(pressToContinue, new Vector2(0f, 0f), Color.White);
			}
		}
		else if (GameState == GameStates.StartMenu)
		{
			Vector2 vector4 = new Vector2(0f, 0f);
			if (startMenuTracker == 0)
			{
				vector4 = new Vector2(780f, 435f);
			}
			else if (startMenuTracker == 1)
			{
				vector4 = new Vector2(780f, 470f);
			}
			else if (startMenuTracker == 2)
			{
				vector4 = new Vector2(780f, 505f);
			}
			else if (startMenuTracker == 3)
			{
				vector4 = new Vector2(780f, 540f);
			}
			else if (startMenuTracker == 4)
			{
				vector4 = new Vector2(780f, 575f);
			}
			spriteBatch.Draw(startMenu, new Vector2(0f, 0f), Color.White);
			if (Guide.IsTrialMode)
			{
				spriteBatch.Draw(startMenuOptionsTrial, new Vector2(-5f, 0f), Color.White);
			}
			else
			{
				spriteBatch.Draw(startMenuOptions, new Vector2(-5f, 0f), Color.White);
			}
			if (blowuptimer <= 0)
			{
				sprite.PlayAnimation(runAnimation);
				sprite.Draw(gameTime, spriteBatch, vector4, SpriteEffects.None);
			}
			else
			{
				sprite.PlayAnimation(headshotAnimation);
				sprite.Draw(gameTime, spriteBatch, vector4 + new Vector2(0f, 32f), SpriteEffects.None);
			}
			if (startMenuFadeInTimer > 0)
			{
				spriteBatch.Draw(blackout, new Vector2(0f, 0f), new Color(0, 0, 0, (int)startMenuFadeInTimer / 4));
			}
		}
		else if (GameState == GameStates.Controls)
		{
			spriteBatch.Draw(controls, vector, Color.White);
		}
		else if (GameState == GameStates.WorldMapDirectory)
		{
			spriteBatch.Draw(blackout, vector, Color.White);
			if (count(world2Locks) <= 0)
			{
				spriteBatch.Draw(locked, new Vector2(worldMapDirectoryTrackerX[1], worldMapDirectoryTrackerY[1]), Color.White);
			}
			if (count(world3Locks) <= 0)
			{
				spriteBatch.Draw(locked, new Vector2(worldMapDirectoryTrackerX[2], worldMapDirectoryTrackerY[2]), Color.White);
			}
			if (count(world4Locks) <= 0)
			{
				spriteBatch.Draw(locked, new Vector2(worldMapDirectoryTrackerX[3], worldMapDirectoryTrackerY[3]), Color.White);
			}
			if (count(world5Locks) <= 0)
			{
				spriteBatch.Draw(locked, new Vector2(worldMapDirectoryTrackerX[4], worldMapDirectoryTrackerY[4]), Color.White);
			}
			spriteBatch.Draw(selected, new Vector2(worldMapDirectoryTrackerX[worldMapDirectoryTracker], worldMapDirectoryTrackerY[worldMapDirectoryTracker]), Color.White);
			spriteBatch.Draw(worldMapDirectory, vector, Color.White);
			spriteBatch.DrawString(hudFont, worldNames[worldMapDirectoryTracker].ToString(), new Vector2(530f, 450f), Color.White);
		}
		else if (GameState == GameStates.TrialDirectory)
		{
			spriteBatch.Draw(blackout, vector, Color.White);
			spriteBatch.Draw(selected, new Vector2(worldMapDirectoryTrackerX[trialDirectoryTracker], worldMapDirectoryTrackerY[trialDirectoryTracker]), Color.White);
			spriteBatch.Draw(trialDirectory, vector, Color.White);
			spriteBatch.DrawString(hudFont, trialNames[trialDirectoryTracker].ToString(), new Vector2(530f, 450f), Color.White);
		}
		else if (GameState == GameStates.Credits)
		{
			if (!creditsNextPage)
			{
				spriteBatch.Draw(creditsPage1, vector, Color.White);
			}
			else
			{
				spriteBatch.Draw(creditsPage2, vector, Color.White);
			}
		}
		else if (GameState == GameStates.Cutscene)
		{
			bool flag = true;
			spriteBatch.Draw(cutsceneTexture, vector + new Vector2(0f, 0f), Color.White);
			if (cutsceneTracker == cutscenes.Length)
			{
				if (cutsceneTimer <= 1000)
				{
					int a = 255 - (int)cutsceneTimer / 4;
					spriteBatch.Draw(blackout, vector, new Color(255, 255, 255, a));
				}
				else if (cutsceneTimer >= 1001)
				{
					cutsceneTracker = 0;
					if (currentGoTo == "PressToStart")
					{
						SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
						base.Components.Add(sharedSaveDevice);
						Global.SaveDevice = sharedSaveDevice;
						sharedSaveDevice.DeviceSelectorCanceled += delegate(object s, SaveDeviceEventArgs e)
						{
							e.Response = SaveDeviceEventResponse.Force;
						};
						sharedSaveDevice.DeviceDisconnected += delegate(object s, SaveDeviceEventArgs e)
						{
							e.Response = SaveDeviceEventResponse.Force;
						};
						sharedSaveDevice.PromptForDevice();
						sharedSaveDevice.DeviceSelected += delegate(object s, EventArgs e)
						{
							Global.SaveDevice = (SaveDevice)s;
						};
						GameState = GameStates.PressToStart;
					}
				}
			}
			else if (cutsceneTimer >= timers[cutsceneTracker] - 1000)
			{
				int a = ((int)cutsceneTimer + 1000 - timers[cutsceneTracker]) / 4;
				spriteBatch.Draw(blackout, vector, new Color(255, 255, 255, a));
			}
			else if (cutsceneTimer <= 1000)
			{
				int a = 255 - (int)cutsceneTimer / 4;
				spriteBatch.Draw(blackout, vector, new Color(255, 255, 255, a));
			}
			if (cutsceneTracker == cutscenes.Length)
			{
				cutsceneTexture = blackout;
			}
			if (cutsceneTracker == cutscenes.Length - 1 && cutsceneTimer >= timers[cutsceneTracker] - 300)
			{
				cutsceneTexture = base.Content.Load<Texture2D>("Menus/blackout");
			}
		}
		else if (GameState == GameStates.Paused)
		{
			if (Guide.IsTrialMode)
			{
				spriteBatch.Draw(pauseMenuTrial, vector, Color.White);
				if (pauseblowuptimer <= 0)
				{
					sprite.PlayAnimation(runAnimation);
					sprite.Draw(gameTime, spriteBatch, new Vector2(pauseMenuTrialX[pauseMenuTracker], pauseMenuTrialY[pauseMenuTracker]), SpriteEffects.None);
				}
				else
				{
					sprite.PlayAnimation(headshotAnimation);
					sprite.Draw(gameTime, spriteBatch, new Vector2(pauseMenuTrialX[pauseMenuTracker], pauseMenuTrialY[pauseMenuTracker] + 32), SpriteEffects.None);
				}
			}
			else
			{
				spriteBatch.Draw(pauseMenu, vector, Color.White);
				if (pauseblowuptimer <= 0)
				{
					sprite.PlayAnimation(runAnimation);
					sprite.Draw(gameTime, spriteBatch, new Vector2(pauseMenuX[pauseMenuTracker], pauseMenuY[pauseMenuTracker]), SpriteEffects.None);
				}
				else
				{
					sprite.PlayAnimation(headshotAnimation);
					sprite.Draw(gameTime, spriteBatch, new Vector2(pauseMenuX[pauseMenuTracker], pauseMenuY[pauseMenuTracker] + 32), SpriteEffects.None);
				}
			}
		}
		else if (GameState == GameStates.Confirm)
		{
			spriteBatch.Draw(confirm, vector, Color.White);
			if (confirmblowuptimer <= 0)
			{
				sprite.PlayAnimation(runAnimation);
				sprite.Draw(gameTime, spriteBatch, new Vector2(confirmX[confirmTracker], confirmY[confirmTracker]), SpriteEffects.None);
			}
			else
			{
				sprite.PlayAnimation(headshotAnimation);
				sprite.Draw(gameTime, spriteBatch, new Vector2(confirmX[confirmTracker], confirmY[confirmTracker] + 32), SpriteEffects.None);
			}
		}
		else if (GameState == GameStates.Normal)
		{
			spriteBatch.Draw(backplate, vector2, Color.White);
			string text = "BONUS: " + (level.timeRemaining.Minutes * 60 + level.timeRemaining.Seconds);
			Color white = Color.White;
			DrawShadowedString(hudFont, text, vector2 + new Vector2(60f, 10f), white);
			float y = hudFont.MeasureString(text).Y;
			DrawShadowedString(hudFont, "SCORE: " + level.Score, vector2 + new Vector2(395f, 10f), Color.White);
			DrawShadowedString(hudFont, "DEATHS: " + level.deaths, vector2 + new Vector2(235f, 10f), Color.White);
			Texture2D texture2D = null;
			if (level.TimeRemaining == TimeSpan.Zero && level.ReachedExit)
			{
				if (worldNumber == 1)
				{
					texture2D = ((world1Moneys[level.levelNumber] != 1) ? winOverlay : winMoneyOverlay);
				}
				if (worldNumber == 2)
				{
					texture2D = ((world2Moneys[level.levelNumber] != 1) ? winOverlay : winMoneyOverlay);
				}
				if (worldNumber == 3)
				{
					texture2D = ((world3Moneys[level.levelNumber] != 1) ? winOverlay : winMoneyOverlay);
				}
				if (worldNumber == 4)
				{
					texture2D = ((world4Moneys[level.levelNumber] != 1) ? winOverlay : winMoneyOverlay);
				}
				if (worldNumber == 5)
				{
					texture2D = ((world5Moneys[level.levelNumber] != 1) ? winOverlay : winMoneyOverlay);
				}
			}
			if (texture2D != null)
			{
				Vector2 vector5 = new Vector2(texture2D.Width, texture2D.Height);
				spriteBatch.Draw(texture2D, vector3 - vector5 / 2f, Color.White);
			}
		}
		if (GameState == GameStates.WorldMap)
		{
			spriteBatch.Draw(blackout, vector, Color.White);
			if (worldNumber == 1)
			{
				for (int num = 0; num < world1Scores.Length; num++)
				{
					if (world1Locks[num] != 1)
					{
						spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[0], worldMapTrackerY[0]), Color.White);
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.Draw(selected, new Vector2(worldMapTrackerX[worldMapTracker], worldMapTrackerY[worldMapTracker]), Color.White);
				spriteBatch.Draw(world1, vector, Color.White);
				spriteBatch.DrawString(hudFont, "Moneys: " + count(world1Moneys) + "/5", new Vector2(280f, 548f), Color.White);
				for (int num = 0; num < world1Scores.Length; num++)
				{
					if (world1Scores[num] >= 0)
					{
						spriteBatch.Draw(completed, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
					if (world1Moneys[num] == 1)
					{
						spriteBatch.Draw(money, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.DrawString(hudFont, count(world1Scores) + "/3", new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.DrawString(hudFont, world1Levels[worldMapTracker].ToString(), new Vector2(530f, 450f), Color.White);
			}
			if (worldNumber == 2)
			{
				for (int num = 0; num < world2Scores.Length; num++)
				{
					if (world2Locks[num] != 1)
					{
						spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[0], worldMapTrackerY[0]), Color.White);
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.Draw(selected, new Vector2(worldMapTrackerX[worldMapTracker], worldMapTrackerY[worldMapTracker]), Color.White);
				spriteBatch.Draw(world2, vector, Color.White);
				spriteBatch.DrawString(hudFont, "Moneys: " + count(world2Moneys) + "/5", new Vector2(280f, 548f), Color.White);
				for (int num = 0; num < world2Scores.Length; num++)
				{
					if (world2Scores[num] >= 0)
					{
						spriteBatch.Draw(completed, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
					if (world2Moneys[num] == 1)
					{
						spriteBatch.Draw(money, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.DrawString(hudFont, count(world2Scores) + "/3", new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.DrawString(hudFont, world2Levels[worldMapTracker].ToString(), new Vector2(530f, 450f), Color.White);
			}
			if (worldNumber == 3)
			{
				for (int num = 0; num < world3Scores.Length; num++)
				{
					if (world3Locks[num] != 1)
					{
						spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[0], worldMapTrackerY[0]), Color.White);
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.Draw(selected, new Vector2(worldMapTrackerX[worldMapTracker], worldMapTrackerY[worldMapTracker]), Color.White);
				spriteBatch.Draw(world3, vector, Color.White);
				spriteBatch.DrawString(hudFont, "Moneys: " + count(world3Moneys) + "/5", new Vector2(280f, 548f), Color.White);
				for (int num = 0; num < world3Scores.Length; num++)
				{
					if (world3Scores[num] >= 0)
					{
						spriteBatch.Draw(completed, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
					if (world3Moneys[num] == 1)
					{
						spriteBatch.Draw(money, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.DrawString(hudFont, count(world3Scores) + "/3", new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.DrawString(hudFont, world3Levels[worldMapTracker].ToString(), new Vector2(530f, 450f), Color.White);
			}
			if (worldNumber == 4)
			{
				for (int num = 0; num < world4Scores.Length; num++)
				{
					if (world4Locks[num] != 1)
					{
						spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[0], worldMapTrackerY[0]), Color.White);
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.Draw(selected, new Vector2(worldMapTrackerX[worldMapTracker], worldMapTrackerY[worldMapTracker]), Color.White);
				spriteBatch.Draw(world4, vector, Color.White);
				spriteBatch.DrawString(hudFont, "Moneys: " + count(world4Moneys) + "/5", new Vector2(280f, 548f), Color.White);
				for (int num = 0; num < world4Scores.Length; num++)
				{
					if (world4Scores[num] >= 0)
					{
						spriteBatch.Draw(completed, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
					if (world4Moneys[num] == 1)
					{
						spriteBatch.Draw(money, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.DrawString(hudFont, count(world4Scores) + "/3", new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.DrawString(hudFont, world4Levels[worldMapTracker].ToString(), new Vector2(530f, 450f), Color.White);
			}
			if (worldNumber == 5)
			{
				for (int num = 0; num < world5Scores.Length; num++)
				{
					if (world5Locks[num] != 1)
					{
						spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[0], worldMapTrackerY[0]), Color.White);
				spriteBatch.Draw(locked, new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.Draw(selected, new Vector2(worldMapTrackerX[worldMapTracker], worldMapTrackerY[worldMapTracker]), Color.White);
				spriteBatch.Draw(world5, vector, Color.White);
				spriteBatch.DrawString(hudFont, "Moneys: " + count(world5Moneys) + "/5", new Vector2(280f, 548f), Color.White);
				for (int num = 0; num < world5Scores.Length; num++)
				{
					if (world5Scores[num] >= 0)
					{
						spriteBatch.Draw(completed, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
					if (world5Moneys[num] == 1)
					{
						spriteBatch.Draw(money, new Vector2(worldMapTrackerX[num + 1], worldMapTrackerY[num + 1]), Color.White);
					}
				}
				spriteBatch.DrawString(hudFont, count(world5Scores) + "/3", new Vector2(worldMapTrackerX[7], worldMapTrackerY[7]), Color.White);
				spriteBatch.DrawString(hudFont, world5Levels[worldMapTracker].ToString(), new Vector2(530f, 450f), Color.White);
			}
			spriteBatch.DrawString(hudFont, "Deaths: " + Deaths, new Vector2(280f, 518f), Color.White);
			Score = calculateTotalScore();
			spriteBatch.DrawString(hudFont, "Total Score: " + Score, new Vector2(880f, 202f), Color.White);
			if (worldMapTracker >= 1 && worldMapTracker <= 6)
			{
				string text2 = "0";
				if (worldNumber == 1)
				{
					text2 = ((world1Scores[worldMapTracker - 1] <= 0) ? "0" : world1Scores[worldMapTracker - 1].ToString());
				}
				else if (worldNumber == 2)
				{
					int num2 = world2Scores[worldMapTracker - 1];
					text2 = ((num2 <= 0) ? "0" : num2.ToString());
				}
				else if (worldNumber == 3)
				{
					text2 = ((world3Scores[worldMapTracker - 1] <= 0) ? "0" : world3Scores[worldMapTracker - 1].ToString());
				}
				else if (worldNumber == 4)
				{
					text2 = ((world4Scores[worldMapTracker - 1] <= 0) ? "0" : world4Scores[worldMapTracker - 1].ToString());
				}
				else if (worldNumber == 5)
				{
					text2 = ((world5Scores[worldMapTracker - 1] <= 0) ? "0" : world5Scores[worldMapTracker - 1].ToString());
				}
				spriteBatch.DrawString(hudFont, "Highscore: " + text2, new Vector2(280f, 573f), Color.White);
			}
			else
			{
				spriteBatch.DrawString(hudFont, "Highscore: NA", new Vector2(280f, 573f), Color.White);
			}
		}
		else if (GameState == GameStates.IntroScroll)
		{
			spriteBatch.Draw(introScroll, new Vector2(0f, introScrolly), Color.White);
			spriteBatch.Draw(introScroll2, new Vector2(0f, introScrolly + 1816f), Color.White);
			spriteBatch.Draw(PressToSkip, new Vector2(-60f, 0f), Color.White);
		}
		else if (GameState == GameStates.ExitScroll)
		{
			int num3 = countAllMoney();
			if (num3 >= 25)
			{
				spriteBatch.Draw(exitScrollMoney, new Vector2(0f, exitScrolly), Color.White);
				spriteBatch.Draw(exitScrollMoney2, new Vector2(0f, exitScrolly + 1816f), Color.White);
			}
			else
			{
				spriteBatch.Draw(exitScroll, new Vector2(0f, exitScrolly), Color.White);
				spriteBatch.Draw(exitScroll2, new Vector2(0f, exitScrolly + 1816f), Color.White);
			}
			spriteBatch.Draw(PressToSkip, new Vector2(-60f, 0f), Color.White);
		}
		else if (GameState == GameStates.Loading)
		{
			spriteBatch.Draw(loading, titleSafeArea, Color.White);
			if (testTimer > 0)
			{
				testTimer--;
				if (testTimer <= 0)
				{
					if (worldMapTracker == 6)
					{
						if (worldNumber == 1)
						{
							if (world1Locks[5] == 1)
							{
								loadNewLevel(worldNumber, worldMapTracker - 1);
								MediaPlayer.Stop();
								firstLoad = true;
							}
						}
						else if (worldNumber == 2 && world2Locks[5] == 1)
						{
							loadNewLevel(worldNumber, worldMapTracker - 1);
							MediaPlayer.Stop();
							firstLoad = true;
						}
						if (worldNumber == 3 && world3Locks[5] == 1)
						{
							loadNewLevel(worldNumber, worldMapTracker - 1);
							MediaPlayer.Stop();
							firstLoad = true;
						}
						if (worldNumber == 4 && world4Locks[5] == 1)
						{
							loadNewLevel(worldNumber, worldMapTracker - 1);
							MediaPlayer.Stop();
							firstLoad = true;
						}
						if (worldNumber == 5 && world5Locks[5] == 1)
						{
							loadNewLevel(worldNumber, worldMapTracker - 1);
							MediaPlayer.Stop();
							firstLoad = true;
						}
					}
					else
					{
						GameState = GameStates.Loading;
						loadNewLevel(worldNumber, worldMapTracker - 1);
						MediaPlayer.Stop();
						firstLoad = true;
					}
				}
			}
			if (levelfinishedLoading)
			{
				levelpauseTime = true;
				levelpauseTimer = 5;
				GameState = GameStates.Normal;
				levelfinishedLoading = false;
			}
			if (levelpauseTime)
			{
				levelpauseTimer--;
				if (levelpauseTimer <= 0 && testTimer <= 0)
				{
					GameState = GameStates.Normal;
				}
			}
		}
		spriteBatch.End();
	}

	private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black);
		spriteBatch.DrawString(font, value, position, color);
	}
}
