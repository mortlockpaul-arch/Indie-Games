using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EasyStorage;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Defenders;

public class Game1 : Game
{
	private enum objective
	{
		none,
		round0,
		round2,
		round3
	}

	public class soundQeue
	{
		public SoundEffect sound;

		public int frame;

		private bool play = true;

		private int delay = 0;

		private float pitch = 0f;

		private int counter = 0;

		public soundQeue(SoundEffect se, float pitch, int delay)
		{
			sound = se;
			frame = (int)((float)se.Duration.TotalMilliseconds / 1000f) * 60;
			this.pitch = pitch;
			this.delay = delay;
		}

		public bool update(float vol, float pan)
		{
			if (play && delay <= 0)
			{
				if (!sound.IsDisposed)
				{
					sound.Play(vol, pitch, pan);
				}
				play = false;
			}
			counter++;
			frame--;
			delay--;
			if (frame <= 0 && counter > 180)
			{
				return true;
			}
			return false;
		}
	}

	private const string savingEditorPath = "C:\\Data\\Dropbox\\XNA\\DOTLC\\Prelude to Pixel Wars\\Prelude to Pixel WarsContent\\";

	public const ushort MaxAssets = 999;

	private const int levelCount = 12;

	private const int colonyHitDelay = 1000;

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	public static GraphicsDevice gd;

	private bool cheating = false;

	private bool showData = false;

	private bool demo = false;

	private bool editor = false;

	private bool develop = false;

	private bool final = true;

	private string kpSite = "http://www.knittedpixels.com";

	private string buyTheGame = "http://defenders.knittedpixels.com/buythegame.html";

	private bool isInitializing = true;

	private bool isLoad = true;

	private bool skipUpdate = false;

	private bool resumeUpdate = false;

	private Hammer.Camera2d camera = new Hammer.Camera2d();

	private int maxDemoLevel = 3;

	private IAsyncSaveDevice saveDevice;

	public List<AssetManager> assetManager = new List<AssetManager>(15);

	public List<AssetManager> assetManagerChallenge = new List<AssetManager>(30);

	public Award awards;

	private List<string[]> messageInfo = new List<string[]>(10);

	private Vector2 MessageInfoPos;

	private bool relicMessage = true;

	private bool chargeMessage = true;

	private bool orbMessage = true;

	private bool healthMessage = true;

	private bool enemyMessage = true;

	private bool basicsMessage = true;

	private bool eliminateMessage = true;

	private bool moveMessage = true;

	private ushort tutorialItems = 0;

	private ushort tutorialCounter = 0;

	private List<string> nodes = new List<string>(200);

	private Level[] level = new Level[15];

	private string[] files;

	private string rootPath;

	private int filesChar;

	private List<UIelement> UIelements;

	private int UIselected;

	private int UIenemSel = -1;

	private int UIshowProperties = -1;

	private int UIeditorSize;

	private int UIeditorFrame;

	private int UIeditorType;

	private int UIeditorSecQtt;

	private int UIeditorPriQtt;

	private int UIeditorInc;

	private int UIeditorEnd;

	private int UIeditorCore;

	private int UIeditorAstr;

	private bool updateFrame = true;

	private Vector2 editorPos = new Vector2(320f, 240f);

	private float editorZoom = 0.4f;

	private float messageInfoCounter = 0f;

	public static int currentLevel = 0;

	private int latestLevel = 0;

	private int nextLevel = 0;

	private float fadeToBlack = 0f;

	private float fade = 0f;

	private float pausePercent = -50f;

	private float pauseTarget = -50f;

	private string[] disclaimerText;

	private float disclaimerTransp;

	private ushort disclaimerTimer;

	private int gatherMsg = 0;

	private int gatherMsgDelay = 1200;

	private int colonyHit = 0;

	private Texture2D txBullet01;

	private Texture2D txBullet02;

	private Texture2D txBullet03;

	private Texture2D txFireball;

	private Texture2D txPlayerChallenge;

	private Texture2D[] txFighter;

	private Texture2D[] txIngeneer;

	private Texture2D txHUD;

	private Texture2D txHexagonBarrier;

	private Texture2D txTurretBase;

	private Texture2D txTurretGun;

	private Texture2D txHive;

	private Texture2D txSanctuary;

	private Texture2D txDrone;

	private Texture2D txAwardSelected;

	private Texture2D txHines;

	private Texture2D txNymeriah;

	private Texture2D txHerschel;

	private Texture2D txDanae;

	private Texture2D txClarke;

	private Texture2D txGeaMoon;

	private Texture2D txCalypso;

	private Texture2D txBradbury;

	private Texture2D txEosRest;

	private Texture2D txOlbers4;

	private Texture2D txEneas;

	private Texture2D txPrometheus;

	private Texture2D txSimulatorRoom;

	private Texture2D txBossBase;

	private Texture2D txBossGlow;

	private Texture2D txBossCore;

	private Texture2D txBossSpikes1;

	private Texture2D txBossSpikes2;

	private Texture2D txBossSpikes3;

	private Texture2D txBossSpikes4;

	private Texture2D txStars;

	private Texture2D txStars2;

	private Texture2D[] txSidescrollerB = new Texture2D[9];

	private Texture2D txSidescrollerBackground;

	private Texture2D txBorder;

	private Texture2D txHexagonsGrid;

	private Texture2D txAsteroid;

	private Texture2D txEnemyClass01;

	private Texture2D txEnemyClass02;

	private Texture2D txEnemyClass03;

	private Texture2D txEnemyClass06;

	private Texture2D txEnemyClass07;

	private Texture2D txEnemyClass08;

	private Texture2D txEnemyClass12;

	private Texture2D txEnemyNod;

	private Texture2D txRing;

	private Texture2D txExpansion;

	private Texture2D txSpark;

	private Texture2D txOrbsTrails;

	private Texture2D txRingClouds;

	private Texture2D txSparks;

	private Texture2D txAwardsScreen;

	private Texture2D txMessage;

	private Texture2D txMenuBackground;

	private Texture2D txMenuFront;

	private Texture2D txTittleOff;

	private Texture2D txTittleOn;

	private Texture2D txSelectBox;

	private Texture2D[] txCharactBox = new Texture2D[4];

	private Texture2D txKP;

	private Texture2D txColony;

	private Texture2D txColonyHUD;

	private Texture2D txColonyCORE;

	private Texture2D txBossLife;

	private Texture2D txExploss;

	public Texture2D whitePixel;

	private Texture2D txJump;

	private Texture2D txNoise;

	private Texture2D txDots;

	private Texture2D txCoins;

	private Texture2D txOrbs;

	private Texture2D txRelic;

	private Texture2D txRelicIcon;

	private Texture2D txHealth;

	private Texture2D txEmp;

	private Texture2D txBomb;

	private Texture2D txItemBomb;

	private Texture2D txUI;

	private Texture2D txUIHealth;

	private Texture2D txUIBlue;

	private Texture2D txGalaxyMap;

	private Texture2D txGalaxyUI;

	private Texture2D txSelectExt;

	private Texture2D txSelectInt;

	private Texture2D txInterlaced;

	private Texture2D txWhiteBar;

	private Texture2D txSelectBack;

	private Texture2D txSelectBar;

	private Texture2D txSelectHUD;

	private Texture2D txPlanetHines;

	private Texture2D txPlanetHinesBrf;

	private Texture2D txPlanetNymeriah;

	private Texture2D txPlanetNymeriahBrf;

	private Texture2D txPlanetHerschel;

	private Texture2D txPlanetHerschelBrf;

	private Texture2D txPlanetDanae;

	private Texture2D txPlanetDanaeBrf;

	private Texture2D txPlanetClarke;

	private Texture2D txPlanetClarkeBrf;

	private Texture2D txPlanetGeaMoon;

	private Texture2D txPlanetGeaMoonBrf;

	private Texture2D txPlanetCalypso;

	private Texture2D txPlanetCalypsoBrf;

	private Texture2D txPlanetBradbury;

	private Texture2D txPlanetBradburyBrf;

	private Texture2D txPlanetEos;

	private Texture2D txPlanetEosBrf;

	private Texture2D txPlanetOlbers;

	private Texture2D txPlanetOlbersBrf;

	private Texture2D txPlanetEneas;

	private Texture2D txPlanetEneasBrf;

	private Texture2D txPlanetPrometheus;

	private Texture2D txPlanetPrometheusBrf;

	private Texture2D txBlast;

	private Texture2D txGalaxyClouds;

	private Texture2D txBar;

	private Texture2D txCircle;

	private Texture2D txArrow;

	private Texture2D txTarget;

	private Texture2D txLens_a1;

	private Texture2D txLens_a2;

	private Texture2D txLens_a3;

	private Texture2D txLens_a4;

	private Texture2D txLens_glow1;

	private Texture2D txLens_rays1;

	private Texture2D txLens_rays2;

	private Texture2D txLensDirt1;

	private Texture2D txLensDirt2;

	private Texture2D txBlackHole;

	private Texture2D txBuy1;

	private Texture2D txBuy2;

	private Texture2D txButtons;

	public SpriteFont gameFont;

	public SpriteFont menuFont;

	public SpriteFont tittlesFont;

	private Sprite background;

	private Sprite stars;

	private Sprite stars2;

	private Sprite[] sidescrollerB = new Sprite[4];

	private Sprite border;

	private Sprite hexagonsGrid;

	private Sprite HUD;

	private Sprite menuBackground;

	private Sprite selectBackground;

	private Sprite selectBar;

	private Sprite selectBar2;

	private Sprite selectHUD;

	private List<Sprite> selectBox;

	private Sprite selectBox1;

	private Sprite selectBox2;

	private Sprite selectBox3;

	private Sprite selectBox4;

	private Sprite kplogo;

	private Sprite tittle;

	private Sprite galaxyMap;

	private Sprite galaxyUI;

	private Sprite selectPlanetExt;

	private Sprite selectPlanetInt;

	private Primitive2D draw2d;

	private List<AnimatedSprite> exploss;

	private List<Blast> blast;

	private SignedInGamer gamer;

	private MouseState currentMouseState;

	private MouseState oldMouseState;

	private KeyboardState currentKeyboardState;

	private KeyboardState oldKeyboardState;

	private GamePadState currentGamePadState;

	private GamePadState oldGamePadState;

	private GamePadCapabilities gamepadinfo = default(GamePadCapabilities);

	private uint controlDelay = 0u;

	private bool left = false;

	private bool right = false;

	private bool up = false;

	private bool down = false;

	private float mouseTransp = 0f;

	private float mouseTranspTarget = 0f;

	private float mouseTimer = 0f;

	private float mouseAngle = 0f;

	private float mouseSize = 1f;

	private Vector2 mousePos = Vector2.One * 400f;

	private List<Intro> intro;

	private List<Intro> howToPlay;

	private List<Intro> controls;

	private List<Intro> ending;

	private List<Intro> buyGame;

	private int introSlide = 0;

	private int howToSlide = 0;

	private int controlsSlide = 0;

	private int endingSlide = 0;

	private DateTime currTime;

	private DateTime startTime;

	private TimeSpan elapsedTime;

	private TimeSpan elapsedPauseTime;

	private uint minutes = 0u;

	private ushort seconds = 0;

	public static GameState gameState;

	private GameState gameStateNext;

	private GameState gameStatePlay;

	private GameState gameStateOld;

	private int endGame;

	private objective winCondition = objective.none;

	private Colony colony;

	private Player[] player;

	private Character[] characters;

	private int[] characterSel;

	private Player playerChallenge = new Player(1);

	private int numPlayers;

	private List<MenuItem> mainMenu = new List<MenuItem>(20);

	private List<MenuItem> playMenu = new List<MenuItem>(20);

	private List<MenuItem> optionsMenu;

	private List<MenuItem> pauseMenu;

	private List<MenuItem> survivalStatsMenu;

	private float menuActive = 0f;

	private int toGalaxyID;

	private SelectableManager challengeUI;

	private SelectableManager challengeList;

	private int challengeNumber = 0;

	private bool challengeClear = false;

	private bool bonusClear = false;

	private bool unlockChubbyRain = false;

	private bool unlockSidescroller = false;

	private bool unlockMeteroids = false;

	private bool unlockBoss = false;

	private List<Message> messages;

	private Confirmation confirm;

	private Color mSelectedColor = Color.LightCyan;

	private Color mNotSelectedColor = Color.DarkCyan;

	public Texture2D txBlack;

	public Texture2D txEmpty;

	private ParticleSystem particleSystem;

	private List<Bullet> bullets;

	private List<Bullet> enemyBullets;

	private List<Enemy> enemies;

	private List<Pickup> coins;

	private List<Pickup> pickups;

	private List<Construction> construction;

	private float[] f = new float[9];

	private List<Lens>[] lens = new List<Lens>[20];

	private int menuIndex = 3;

	private int playMenuIndex = 3;

	private int optionsMenuIndex = 3;

	private int pauseMenuIndex = 3;

	private int survivalStatsMenuIndex = 3;

	private int oldDireccion;

	private int direccion;

	private bool mouseClicked = false;

	private bool mouseRightClicked = false;

	private int menuDelay = 0;

	private Random random;

	private int maxEnemies = 20;

	private int topEnemies = 100;

	private int waveTotal = 0;

	private int waveNumber = 0;

	private Vector2 positionBegin = new Vector2(0f, 0f);

	private Vector2 positionEnd = new Vector2(500f, 400f);

	private Vector2 cameraPosition = new Vector2(0f, 0f);

	private float cameraZoom;

	private PlayerIndex controllingPlayer = PlayerIndex.One;

	private PlayerIndex controllingPlayerMenus = PlayerIndex.One;

	private SoundEffect laserSound;

	private SoundEffect beepHSound;

	private SoundEffect beepSSound;

	private SoundEffect explosionSound;

	private SoundEffect coinSound;

	private List<soundQeue> coinSounds = new List<soundQeue>(1);

	private SoundEffect messageSound;

	private SoundEffect sndColonyUnderA;

	private SoundEffect sndRelicAquired;

	private SoundEffect sndBringOrbs;

	private SoundEffect sndCoreFull;

	private SoundEffect sndProtectColony;

	private string currentSongName;

	private Song currentSong;

	private Song Colonial_trance;

	private Song musicDarkMatter;

	private Song EpicFinale;

	private Song In_a_heart_beat;

	private Song musicJarre80sTheme;

	private Song KnowYourEnemy;

	private Song musicLevel01;

	private Song musicMainTheme;

	private Song Marching;

	private Song MoonStrings;

	private Song Pit;

	private Song musicReachTheStarsTheme;

	private Song FaceYourFears;

	private Song HeIsAlive;

	private Song HeartAndDanger;

	private Song LongIsTheWay;

	private Song TimeToRun;

	private Song UnknownBellow;

	private Song WeirdDimensions;

	private bool GOfullScreen = false;

	private float GOmusicVolume = 75f;

	private float GOsoundFXvolume = 100f;

	private float GOHUDopacity = 100f;

	private int GOresolutionX = 1280;

	private int GOresolutionY = 720;

	public static int GOresolutionIndex;

	private float vibrationLeft;

	private float vibrationRight;

	private int GOvibration = 1;

	private float keyboardPlayer;

	private float GOdifficulty = 50f;

	private bool firstTime = true;

	public static float difficulty = 0.5f;

	private bool completeCoopLevel = false;

	private bool completeCoopBoss = false;

	private string sidescrollerSong = "In_a_heart_beat";

	private uint frame;

	private uint checkUpdate = 0u;

	private uint bossFrame = 0u;

	private uint round = 0u;

	private float frameTime = 0f;

	private int frameCounter = 0;

	private int currentFrameRate = 0;

	private int numOrbs = 0;

	private List<DisplayMode> displayModes = new List<DisplayMode>(500);

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		currTime = DateTime.Now;
		startTime = currTime;
		displayModes = GetDisplayModes(base.GraphicsDevice);
		gd = base.GraphicsDevice;
		if (Program.arguments.Length > 0 && !final && Program.arguments[0] == "EDITOR")
		{
			editor = true;
			demo = false;
			develop = true;
			cheating = true;
		}
		EasyStorageSettings.SetSupportedLanguages(Language.English);
		SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
		base.Components.Add(sharedSaveDevice);
		saveDevice = sharedSaveDevice;
		sharedSaveDevice.DeviceSelectorCanceled += delegate(object s, SaveDeviceEventArgs e)
		{
			e.Response = SaveDeviceEventResponse.Force;
		};
		sharedSaveDevice.DeviceDisconnected += delegate(object s, SaveDeviceEventArgs e)
		{
			e.Response = SaveDeviceEventResponse.Force;
		};
		sharedSaveDevice.PromptForDevice();
		base.Components.Add(new GamerServicesComponent(this));
		saveDevice.SaveCompleted += saveDevice_SaveCompleted;
		graphics.PreferredBackBufferWidth = GOresolutionX;
		graphics.PreferredBackBufferHeight = GOresolutionY;
		graphics.IsFullScreen = true;
		graphics.PreferMultiSampling = true;
		base.IsMouseVisible = false;
		graphics.ApplyChanges();
		for (int num = 0; num < 15; num++)
		{
			assetManager.Add(new AssetManager(b: true));
		}
		for (int num = 0; num < 30; num++)
		{
			assetManagerChallenge.Add(new AssetManager(b: true));
		}
		Guide.SimulateTrialMode = false;
		CreateIntro();
		CreateHowToPlay();
		CreateControls();
		CreateEnding();
		colony = new Colony();
		resetMessageInfo();
		if (editor)
		{
			rootPath = "C:\\Data\\Dropbox\\XNA\\DOTLC\\Prelude to Pixel Wars\\Prelude to Pixel WarsContent\\";
		}
		else
		{
			rootPath = base.Content.RootDirectory + "\\";
		}
		string text = rootPath + "Data";
		files = new string[100];
		filesChar = text.Length + 1;
		try
		{
			files = Directory.GetFiles(text, "C*");
		}
		catch
		{
		}
		try
		{
			level = new Level[15];
		}
		catch
		{
		}
		readProgress();
		particleSystem = new ParticleSystem();
		try
		{
			bullets = new List<Bullet>(250);
		}
		catch
		{
		}
		try
		{
			enemyBullets = new List<Bullet>(100);
		}
		catch
		{
		}
		try
		{
			enemies = new List<Enemy>(200);
		}
		catch
		{
		}
		try
		{
			pickups = new List<Pickup>(32);
			coins = new List<Pickup>(200);
		}
		catch
		{
		}
		try
		{
			construction = new List<Construction>(200);
		}
		catch
		{
		}
		try
		{
			for (int num2 = 0; num2 < lens.Length; num2++)
			{
				lens[num2] = new List<Lens>(5);
			}
		}
		catch
		{
		}
		try
		{
			messages = new List<Message>(10);
		}
		catch
		{
		}
		confirm = new Confirmation();
		random = new Random();
		disclaimerText = new string[40];
		for (int num = 0; num < disclaimerText.Length; num++)
		{
			disclaimerText[num] = "";
		}
		int num3 = 0;
		disclaimerText[num3] = "Defenders of the Last Colony";
		num3++;
		disclaimerText[num3] = "Developed by Knitted Pixels (KP), (c) 2012,";
		num3++;
		disclaimerText[num3] = kpSite;
		num3++;
		disclaimerText[num3] = "";
		num3++;
		disclaimerText[num3] = "";
		num3++;
		disclaimerText[num3] = "";
		num3++;
		disclaimerText[num3] = "";
		num3++;
		disclaimerTransp = 0f;
		disclaimerTimer = 0;
		cameraZoom = 0.1f;
		camera.Zoom = 0.1f;
		HUD = new Sprite();
		background = new Sprite();
		stars = new Sprite();
		stars2 = new Sprite();
		sidescrollerB[0] = new Sprite();
		sidescrollerB[1] = new Sprite();
		sidescrollerB[2] = new Sprite();
		sidescrollerB[3] = new Sprite();
		border = new Sprite();
		hexagonsGrid = new Sprite();
		menuBackground = new Sprite();
		selectBackground = new Sprite();
		selectBar = new Sprite();
		selectBar2 = new Sprite();
		selectHUD = new Sprite();
		try
		{
			selectBox = new List<Sprite>(4);
		}
		catch
		{
		}
		selectBox1 = new Sprite();
		selectBox2 = new Sprite();
		selectBox3 = new Sprite();
		selectBox4 = new Sprite();
		kplogo = new Sprite();
		galaxyMap = new Sprite();
		galaxyUI = new Sprite();
		selectPlanetExt = new Sprite();
		selectPlanetInt = new Sprite();
		tittle = new Sprite();
		tittle.Initialize(base.Content.Load<Texture2D>("Graphics/StartScreen/tittle"), new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height / 4), new Vector2(1f, 1f), 0f);
		tittle.origin = new Vector2(tittle.Width / 2, tittle.Height / 2);
		draw2d = new Primitive2D(base.GraphicsDevice);
		try
		{
			blast = new List<Blast>(20);
		}
		catch
		{
		}
		frame = 0u;
		gameStateNext = GameState.disclaimer;
		gameState = GameState.disclaimer;
		if (editor)
		{
			cheating = true;
		}
		if (cheating)
		{
			gameStateNext = GameState.mainMenu;
		}
		try
		{
			UIelements = new List<UIelement>(20);
		}
		catch
		{
		}
		GC.Collect();
		base.Initialize();
		ReadAll();
		isInitializing = false;
	}

	private void resetMessageInfo()
	{
		messageInfo.RemoveRange(0, messageInfo.Count);
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		txBullet01 = base.Content.Load<Texture2D>("Graphics/Bullets/Bullet01");
		txBullet02 = base.Content.Load<Texture2D>("Graphics/Bullets/Bullet02");
		txBullet03 = base.Content.Load<Texture2D>("Graphics/Bullets/Bullet03");
		txFireball = base.Content.Load<Texture2D>("Graphics/Bullets/Fireball");
		List<Texture2D> list = new List<Texture2D>(20);
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievements"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_100down"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_1000down"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_10000down"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_ColonySurvive"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Complete"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Coop"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Engineer"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Explorer"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Fighter"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_FinalBoss"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_ChubbyRain"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Relic"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_ModeUnlocked"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Sidescroller"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_Survival"));
		list.Add(base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_locked"));
		txAwardSelected = base.Content.Load<Texture2D>("Graphics/Achievements/Achievement_selected");
		awards = new Award("DOTLC", list);
		readAwards();
		txHexagonBarrier = base.Content.Load<Texture2D>("Graphics/levels/hexagonBarrier");
		txTurretBase = base.Content.Load<Texture2D>("Graphics/levels/turretBase");
		txTurretGun = base.Content.Load<Texture2D>("Graphics/levels/turretGun");
		txHive = base.Content.Load<Texture2D>("Graphics/levels/hive");
		txSanctuary = base.Content.Load<Texture2D>("Graphics/levels/sanctuary");
		txDrone = base.Content.Load<Texture2D>("Graphics/levels/Drone");
		whitePixel = base.Content.Load<Texture2D>("Graphics/Common/white");
		txCoins = base.Content.Load<Texture2D>("Graphics/Pickups/coins");
		txOrbs = base.Content.Load<Texture2D>("Graphics/Pickups/orbs");
		txRelic = base.Content.Load<Texture2D>("Graphics/Pickups/relic");
		txRelicIcon = base.Content.Load<Texture2D>("Graphics/Pickups/relicIcon");
		txHealth = base.Content.Load<Texture2D>("Graphics/Pickups/health");
		txEmp = base.Content.Load<Texture2D>("Graphics/Pickups/emp");
		txItemBomb = base.Content.Load<Texture2D>("Graphics/Pickups/Item_bomb");
		txBomb = base.Content.Load<Texture2D>("Graphics/Pickups/bomb");
		txBlack = base.Content.Load<Texture2D>("Graphics/common/black");
		txEmpty = base.Content.Load<Texture2D>("Graphics/common/empty");
		txColony = base.Content.Load<Texture2D>("Graphics/Players/colony");
		txColonyHUD = base.Content.Load<Texture2D>("Graphics/UI/Colony_HUD");
		txColonyCORE = base.Content.Load<Texture2D>("Graphics/UI/Core_HUD");
		txBossLife = base.Content.Load<Texture2D>("Graphics/UI/Boss_HUD");
		colony.Initialize(txColony, Vector2.Zero);
		txUI = base.Content.Load<Texture2D>("Graphics/UI/ui");
		txUIHealth = base.Content.Load<Texture2D>("Graphics/UI/uiHealth");
		txUIBlue = base.Content.Load<Texture2D>("Graphics/UI/uiBlue");
		txBlast = base.Content.Load<Texture2D>("Graphics/FX/blast");
		txGalaxyClouds = base.Content.Load<Texture2D>("Graphics/FX/GalaxyClouds");
		txBar = base.Content.Load<Texture2D>("Graphics/UI/bar");
		txCircle = base.Content.Load<Texture2D>("Graphics/UI/circle");
		txArrow = base.Content.Load<Texture2D>("Graphics/UI/arrow");
		txTarget = base.Content.Load<Texture2D>("Graphics/UI/target");
		txLens_a1 = base.Content.Load<Texture2D>("Graphics/FX/Lens/Lens_a1");
		txLens_a2 = base.Content.Load<Texture2D>("Graphics/FX/Lens/Lens_a2");
		txLens_a3 = base.Content.Load<Texture2D>("Graphics/FX/Lens/Lens_a3");
		txLens_a4 = base.Content.Load<Texture2D>("Graphics/FX/Lens/Lens_a4");
		txLens_glow1 = base.Content.Load<Texture2D>("Graphics/FX/Lens/Lens_glow1");
		txLens_rays1 = base.Content.Load<Texture2D>("Graphics/FX/Lens/Lens_rays1");
		txLens_rays2 = base.Content.Load<Texture2D>("Graphics/FX/Lens/Lens_rays2");
		txLensDirt1 = base.Content.Load<Texture2D>("Graphics/FX/Lens/LensDirt1");
		txLensDirt2 = base.Content.Load<Texture2D>("Graphics/FX/Lens/LensDirt2");
		txBlackHole = base.Content.Load<Texture2D>("Graphics/Screens/BlackHole");
		txBuy1 = base.Content.Load<Texture2D>("Graphics/Screens/Buy1");
		txBuy2 = base.Content.Load<Texture2D>("Graphics/Screens/Buy2");
		txButtons = base.Content.Load<Texture2D>("Graphics/UI/Buttons");
		txPlayerChallenge = base.Content.Load<Texture2D>("Graphics/Players/Ingeneer1");
		txFighter = new Texture2D[4];
		txIngeneer = new Texture2D[4];
		txFighter[0] = base.Content.Load<Texture2D>("Graphics/Players/Fighter1");
		txFighter[1] = base.Content.Load<Texture2D>("Graphics/Players/Fighter2");
		txFighter[2] = base.Content.Load<Texture2D>("Graphics/Players/Fighter3");
		txFighter[3] = base.Content.Load<Texture2D>("Graphics/Players/Fighter4");
		txIngeneer[0] = base.Content.Load<Texture2D>("Graphics/Players/Ingeneer1");
		txIngeneer[1] = base.Content.Load<Texture2D>("Graphics/Players/Ingeneer2");
		txIngeneer[2] = base.Content.Load<Texture2D>("Graphics/Players/Ingeneer3");
		txIngeneer[3] = base.Content.Load<Texture2D>("Graphics/Players/Ingeneer4");
		txHUD = base.Content.Load<Texture2D>("Graphics/UI/HUD");
		txHines = base.Content.Load<Texture2D>("Graphics/Levels/Hines");
		txNymeriah = base.Content.Load<Texture2D>("Graphics/Levels/Nymeriah");
		txHerschel = base.Content.Load<Texture2D>("Graphics/Levels/Herschel");
		txDanae = base.Content.Load<Texture2D>("Graphics/Levels/danae");
		txClarke = base.Content.Load<Texture2D>("Graphics/Levels/Clarke");
		txGeaMoon = base.Content.Load<Texture2D>("Graphics/Levels/GeaMoon");
		txCalypso = base.Content.Load<Texture2D>("Graphics/Levels/Calypso");
		txBradbury = base.Content.Load<Texture2D>("Graphics/Levels/Bradbury");
		txEosRest = base.Content.Load<Texture2D>("Graphics/Levels/EosRest");
		txOlbers4 = base.Content.Load<Texture2D>("Graphics/Levels/Olbers4");
		txEneas = base.Content.Load<Texture2D>("Graphics/Levels/eneas");
		txPrometheus = base.Content.Load<Texture2D>("Graphics/Levels/prometheus");
		txSimulatorRoom = base.Content.Load<Texture2D>("Graphics/Levels/Simulator Room");
		txBossBase = base.Content.Load<Texture2D>("Graphics/Levels/base");
		txBossGlow = base.Content.Load<Texture2D>("Graphics/Levels/bossGlow");
		txBossCore = base.Content.Load<Texture2D>("Graphics/Levels/core");
		txBossSpikes1 = base.Content.Load<Texture2D>("Graphics/Levels/spikes1");
		txBossSpikes2 = base.Content.Load<Texture2D>("Graphics/Levels/spikes2");
		txBossSpikes3 = base.Content.Load<Texture2D>("Graphics/Levels/spikes3");
		txBossSpikes4 = base.Content.Load<Texture2D>("Graphics/Levels/spikes4");
		background.Initialize(txDanae, new Vector2(0f, 0f), new Vector2(0f, 0f), 0f);
		txStars = base.Content.Load<Texture2D>("Graphics/Levels/Stars01");
		txStars2 = base.Content.Load<Texture2D>("Graphics/Levels/Stars02");
		for (ushort num = 0; num < txSidescrollerB.Length; num++)
		{
			txSidescrollerB[num] = base.Content.Load<Texture2D>("Graphics/Levels/SideScroller/Sidescroller0" + Convert.ToString(num + 1));
			if (num < 4)
			{
				sidescrollerB[num].Initialize(txSidescrollerB[num], new Vector2(txSidescrollerB[num].Width * (num + 1), -100f), Vector2.One, 0f);
			}
		}
		txSidescrollerBackground = base.Content.Load<Texture2D>("Graphics/Levels/SideScroller/Sidescroller");
		txBorder = base.Content.Load<Texture2D>("Graphics/Levels/border");
		txHexagonsGrid = base.Content.Load<Texture2D>("Graphics/Levels/hexagonsGrid");
		txAsteroid = base.Content.Load<Texture2D>("Graphics/Enemies/asteroid");
		txEnemyClass01 = base.Content.Load<Texture2D>("Graphics/Enemies/class01");
		txEnemyClass02 = base.Content.Load<Texture2D>("Graphics/Enemies/class02");
		txEnemyClass03 = base.Content.Load<Texture2D>("Graphics/Enemies/class03");
		txEnemyClass06 = base.Content.Load<Texture2D>("Graphics/Enemies/class06");
		txEnemyClass07 = base.Content.Load<Texture2D>("Graphics/Enemies/class07");
		txEnemyClass08 = base.Content.Load<Texture2D>("Graphics/Enemies/class08");
		txEnemyClass12 = base.Content.Load<Texture2D>("Graphics/Enemies/Class_12");
		txEnemyNod = base.Content.Load<Texture2D>("Graphics/Enemies/nod");
		txRing = base.Content.Load<Texture2D>("Graphics/FX/ring");
		txExpansion = base.Content.Load<Texture2D>("Graphics/FX/expansion");
		txSpark = base.Content.Load<Texture2D>("Graphics/FX/spark");
		txOrbsTrails = base.Content.Load<Texture2D>("Graphics/FX/orbTrails");
		txRingClouds = base.Content.Load<Texture2D>("Graphics/FX/ringClouds");
		txSparks = base.Content.Load<Texture2D>("Graphics/FX/rays");
		txExploss = base.Content.Load<Texture2D>("Graphics/FX/ballExploss");
		txMenuBackground = base.Content.Load<Texture2D>("Graphics/Menu/menuBackground");
		txMenuFront = base.Content.Load<Texture2D>("Graphics/Screens/MENU_FrontLayer");
		txTittleOff = base.Content.Load<Texture2D>("Graphics/Screens/Glow_off");
		txTittleOn = base.Content.Load<Texture2D>("Graphics/Screens/Glow_on");
		txAwardsScreen = base.Content.Load<Texture2D>("Graphics/Screens/Awards_screen");
		txMessage = base.Content.Load<Texture2D>("Graphics/Screens/message");
		txSelectBox = base.Content.Load<Texture2D>("Graphics/Screens/SelectBox");
		txCharactBox[0] = base.Content.Load<Texture2D>("Graphics/Screens/Mark");
		txCharactBox[1] = base.Content.Load<Texture2D>("Graphics/Screens/Michelle");
		txCharactBox[2] = base.Content.Load<Texture2D>("Graphics/Screens/James");
		txCharactBox[3] = base.Content.Load<Texture2D>("Graphics/Screens/Vela");
		int num2 = 350;
		for (int i = 0; i < 4; i++)
		{
			float num3 = 0f;
			switch (i)
			{
			case 0:
				num3 = -1f;
				break;
			case 1:
				num3 = -0.333f;
				break;
			case 2:
				num3 = 0.333f;
				break;
			case 3:
				num3 = 1f;
				break;
			}
			Sprite sprite = new Sprite();
			sprite.Initialize(txSelectBox, new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) + (float)num2 * num3 - 80f, 100f), new Vector2(1f, 1f), 0f);
			selectBox.Add(sprite);
			sprite = null;
		}
		repositionSelectBox();
		txGalaxyMap = base.Content.Load<Texture2D>("Graphics/Menu/Galaxy_map");
		galaxyMap.Initialize(txGalaxyMap, new Vector2(txGalaxyMap.Width / -2, txGalaxyMap.Height / -2), Vector2.One, 0f);
		txGalaxyUI = base.Content.Load<Texture2D>("Graphics/Menu/galaxyUI");
		galaxyUI.Initialize(txGalaxyUI, Vector2.Zero, new Vector2(base.GraphicsDevice.Viewport.Width / txGalaxyMap.Width, base.GraphicsDevice.Viewport.Height / txGalaxyMap.Height), 0f);
		txSelectExt = base.Content.Load<Texture2D>("Graphics/Menu/selectExt");
		txSelectInt = base.Content.Load<Texture2D>("Graphics/Menu/selectInt");
		selectPlanetExt.Initialize(txSelectExt, Vector2.Zero, new Vector2(2f), 0f);
		selectPlanetInt.Initialize(txSelectInt, Vector2.Zero, new Vector2(1.8f), 0f);
		selectPlanetExt.origin = new Vector2(txSelectExt.Width / 2, txSelectExt.Height / 2);
		selectPlanetInt.origin = new Vector2(txSelectInt.Width / 2, txSelectInt.Height / 2);
		txInterlaced = txEmpty;
		txSelectBack = base.Content.Load<Texture2D>("Graphics/Screens/SelectBack");
		txSelectBar = base.Content.Load<Texture2D>("Graphics/Screens/SelectBar");
		txSelectHUD = base.Content.Load<Texture2D>("Graphics/Screens/SelectHUD");
		txWhiteBar = base.Content.Load<Texture2D>("Graphics/Menu/whiteBar");
		txPlanetHines = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Hines");
		txPlanetHinesBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/HinesBrf");
		txPlanetNymeriah = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Nymeriah");
		txPlanetNymeriahBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/NymeriahBrf");
		txPlanetHerschel = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Herschel");
		txPlanetHerschelBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/HerschelBrf");
		txPlanetDanae = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Danae");
		txPlanetDanaeBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/DanaeBrf");
		txPlanetClarke = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Clarke");
		txPlanetClarkeBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/ClarkeBrf");
		txPlanetGeaMoon = base.Content.Load<Texture2D>("Graphics/Menu/Planets/GeaMoon");
		txPlanetGeaMoonBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/GeaMoonBrf");
		txPlanetCalypso = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Calypso");
		txPlanetCalypsoBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/CalypsoBrf");
		txPlanetBradbury = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Bradbury");
		txPlanetBradburyBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/BradburyBrf");
		txPlanetEos = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Eos");
		txPlanetEosBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/EosBrf");
		txPlanetOlbers = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Olbers4");
		txPlanetOlbersBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Olbers4Brf");
		txPlanetEneas = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Eneas");
		txPlanetEneasBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/EneasBrf");
		txPlanetPrometheus = base.Content.Load<Texture2D>("Graphics/Menu/Planets/Prometheus");
		txPlanetPrometheusBrf = base.Content.Load<Texture2D>("Graphics/Menu/Planets/PrometheusBrf");
		txKP = base.Content.Load<Texture2D>("Graphics/StartScreen/KP_big");
		kplogo.Initialize(txKP, new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height / 2), new Vector2(1f, 1f), 0f);
		kplogo.size = new Vector2(0.8f);
		kplogo.transparency = 0f;
		kplogo.origin = new Vector2(kplogo.Width / 2, kplogo.Height / 2);
		txDots = base.Content.Load<Texture2D>("Graphics/FX/dots");
		txNoise = base.Content.Load<Texture2D>("Graphics/FX/exploss");
		txJump = base.Content.Load<Texture2D>("Graphics/FX/jump");
		laserSound = base.Content.Load<SoundEffect>("sound/FX/laserFire");
		beepHSound = base.Content.Load<SoundEffect>("sound/UI/beepH");
		beepSSound = base.Content.Load<SoundEffect>("sound/UI/beepS");
		explosionSound = base.Content.Load<SoundEffect>("sound/FX/explosion");
		coinSound = base.Content.Load<SoundEffect>("sound/FX/coins");
		messageSound = base.Content.Load<SoundEffect>("sound/FX/message");
		sndColonyUnderA = base.Content.Load<SoundEffect>("sound/Messages/ColonyUnderAttack");
		sndRelicAquired = base.Content.Load<SoundEffect>("sound/Messages/relicAcquired");
		sndBringOrbs = base.Content.Load<SoundEffect>("sound/Messages/BringOrbs");
		sndCoreFull = base.Content.Load<SoundEffect>("sound/Messages/coreFull");
		sndProtectColony = base.Content.Load<SoundEffect>("sound/Messages/protectColony");
		Colonial_trance = base.Content.Load<Song>("sound/Music/Colonial_trance");
		musicDarkMatter = base.Content.Load<Song>("sound/Music/DarkMatter");
		EpicFinale = base.Content.Load<Song>("sound/Music/EpicFinale");
		In_a_heart_beat = base.Content.Load<Song>("sound/Music/In_a_heart_beat");
		musicJarre80sTheme = base.Content.Load<Song>("sound/Music/jarre80s");
		KnowYourEnemy = base.Content.Load<Song>("sound/Music/KnowYourEnemy");
		musicLevel01 = base.Content.Load<Song>("sound/Music/Level01_regular");
		musicMainTheme = base.Content.Load<Song>("sound/Music/MainTheme");
		Marching = base.Content.Load<Song>("sound/Music/Marching");
		MoonStrings = base.Content.Load<Song>("sound/Music/MoonStrings");
		Pit = base.Content.Load<Song>("sound/Music/Pit");
		musicReachTheStarsTheme = base.Content.Load<Song>("sound/Music/Reach_The_Stars");
		FaceYourFears = base.Content.Load<Song>("sound/Music/ManuelMarino/FaceYourFears");
		HeIsAlive = base.Content.Load<Song>("sound/Music/ManuelMarino/HeIsAlive");
		HeartAndDanger = base.Content.Load<Song>("sound/Music/ManuelMarino/HeartAndDanger");
		LongIsTheWay = base.Content.Load<Song>("sound/Music/ManuelMarino/LongIsTheWay");
		TimeToRun = base.Content.Load<Song>("sound/Music/ManuelMarino/TimeToRun");
		UnknownBellow = base.Content.Load<Song>("sound/Music/ManuelMarino/UnknownBelow");
		WeirdDimensions = base.Content.Load<Song>("sound/Music/ManuelMarino/WeirdDimensions");
		gameFont = base.Content.Load<SpriteFont>("Fonts/GameFont");
		menuFont = base.Content.Load<SpriteFont>("Fonts/fontTWcenMT");
		tittlesFont = base.Content.Load<SpriteFont>("Fonts/bigTittles");
		selectBackground.Initialize(txSelectBack, new Vector2(0f, 0f), new Vector2(1f, 1f), 0f);
		selectBar.Initialize(txSelectBar, new Vector2(0f, 0f), new Vector2(1f, 1f), 0f);
		selectBar2.Initialize(txSelectBar, new Vector2(0f, 0f), new Vector2(1f, 1f), 0f);
		selectHUD.Initialize(txSelectHUD, new Vector2(0f, 0f), new Vector2(1f, 1f), 0f);
		particleSystem.Initialize(txNoise, txDots, explosionSound);
		try
		{
			exploss = new List<AnimatedSprite>(20);
		}
		catch
		{
		}
		createGalaxy();
		readOptions();
		createMainMenu();
		createPlayMenu();
		createOptionsMenu();
		createPauseMenu();
		createSurvivalStatsMenu();
		CreateSelectChallengeUI();
		CreateEditor();
		createCharacters();
		isLoad = false;
	}

	private void InitializeSaveDevice()
	{
		EasyStorageSettings.SetSupportedLanguages(Language.English, Language.Spanish, Language.French, Language.Japanese, Language.German, Language.Italian);
		SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
		base.Components.Add(sharedSaveDevice);
		saveDevice = sharedSaveDevice;
		sharedSaveDevice.DeviceSelectorCanceled += delegate(object s, SaveDeviceEventArgs e)
		{
			e.Response = SaveDeviceEventResponse.Force;
		};
		sharedSaveDevice.DeviceDisconnected += delegate(object s, SaveDeviceEventArgs e)
		{
			e.Response = SaveDeviceEventResponse.Force;
		};
		sharedSaveDevice.PromptForDevice();
		base.Components.Add(new GamerServicesComponent(this));
		saveDevice.SaveCompleted += saveDevice_SaveCompleted;
	}

	private void saveDevice_SaveCompleted(object sender, FileActionCompletedEventArgs args)
	{
	}

	private void writeAwards()
	{
		bool flag = false;
		if (Guide.IsTrialMode || demo || !saveDevice.IsReady)
		{
			return;
		}
		saveDevice.SaveAsync("DOTLC", "DOTLCAwards", delegate(Stream stream)
		{
			using StreamWriter streamWriter = new StreamWriter(stream);
			for (int i = 0; i < awards.Data.Count; i++)
			{
				streamWriter.WriteLine(awards.Data[i].name);
				streamWriter.WriteLine(Convert.ToString(awards.Data[i].unlocked));
			}
		});
	}

	private void readAwards()
	{
		bool flag = false;
		flag = Guide.IsTrialMode;
		if (!saveDevice.IsReady || saveDevice.IsBusy)
		{
			return;
		}
		if (saveDevice.FileExists("DOTLC", "DOTLCAwards"))
		{
			saveDevice.Load("DOTLC", "DOTLCAwards", delegate(Stream stream)
			{
				using StreamReader streamReader = new StreamReader(stream);
				try
				{
					for (int i = 0; i < awards.Data.Count; i++)
					{
						awards.Data[i].name = streamReader.ReadLine();
						awards.Data[i].unlocked = bool.Parse(streamReader.ReadLine());
					}
				}
				catch
				{
				}
			});
		}
		else
		{
			for (int num = 0; num < awards.Data.Count; num++)
			{
				awards.Data[num].unlocked = false;
			}
		}
	}

	private void writeCharacters()
	{
		bool flag = false;
		if (Guide.IsTrialMode || demo || !saveDevice.IsReady)
		{
			return;
		}
		saveDevice.SaveAsync("DOTLC", "DOTLCCharacters", delegate(Stream stream)
		{
			using StreamWriter streamWriter = new StreamWriter(stream);
			for (int i = 0; i < characters.Length; i++)
			{
				streamWriter.WriteLine(characters[i].name);
				streamWriter.WriteLine(characters[i].shipClass);
				streamWriter.WriteLine(Convert.ToString(characters[i].color.R));
				streamWriter.WriteLine(Convert.ToString(characters[i].color.G));
				streamWriter.WriteLine(Convert.ToString(characters[i].color.B));
				streamWriter.WriteLine(characters[i].abilityType);
				for (int j = 0; j < 14; j++)
				{
					streamWriter.WriteLine(Convert.ToString(characters[i].relics[j]));
				}
				streamWriter.WriteLine(Convert.ToString((int)(characters[i].ability[0] * 100f)));
				streamWriter.WriteLine(Convert.ToString((int)(characters[i].ability[1] * 100f)));
				streamWriter.WriteLine(Convert.ToString((int)(characters[i].ability[2] * 100f)));
				streamWriter.WriteLine(Convert.ToString((int)(characters[i].ability[3] * 100f)));
				streamWriter.WriteLine(Convert.ToString(characters[i].numberOfKills));
				streamWriter.WriteLine(Convert.ToString(characters[i].level));
				streamWriter.WriteLine(Convert.ToString(characters[i].experience));
				streamWriter.WriteLine(Convert.ToString(characters[i].nextLevel));
			}
		});
	}

	private void readCharacters()
	{
		bool flag = false;
		flag = Guide.IsTrialMode;
		if (!saveDevice.IsReady || saveDevice.IsBusy)
		{
			return;
		}
		bool flag2 = false;
		try
		{
			flag2 = saveDevice.FileExists("DOTLC", "DOTLCCharacters");
		}
		catch
		{
		}
		if (flag2)
		{
			saveDevice.Load("DOTLC", "DOTLCCharacters", delegate(Stream stream)
			{
				using StreamReader streamReader = new StreamReader(stream);
				try
				{
					for (int i = 0; i < characters.Length; i++)
					{
						characters[i].name = streamReader.ReadLine();
						characters[i].shipClass = streamReader.ReadLine();
						characters[i].color.R = byte.Parse(streamReader.ReadLine());
						characters[i].color.G = byte.Parse(streamReader.ReadLine());
						characters[i].color.B = byte.Parse(streamReader.ReadLine());
						characters[i].abilityType = streamReader.ReadLine();
						for (int j = 0; j < 14; j++)
						{
							characters[i].relics[j] = ushort.Parse(streamReader.ReadLine());
						}
						characters[i].ability[0] = int.Parse(streamReader.ReadLine()) / 100;
						characters[i].ability[1] = int.Parse(streamReader.ReadLine()) / 100;
						characters[i].ability[2] = int.Parse(streamReader.ReadLine()) / 100;
						characters[i].ability[3] = int.Parse(streamReader.ReadLine()) / 100;
						characters[i].numberOfKills = uint.Parse(streamReader.ReadLine());
						characters[i].level = ushort.Parse(streamReader.ReadLine());
						characters[i].experience = int.Parse(streamReader.ReadLine());
						characters[i].nextLevel = int.Parse(streamReader.ReadLine());
					}
				}
				catch
				{
				}
			});
		}
		else
		{
			createCharacters();
		}
	}

	private void writeProgress()
	{
		bool flag = false;
		if (Guide.IsTrialMode || demo || editor || !saveDevice.IsReady)
		{
			return;
		}
		saveDevice.SaveAsync("DOTLC", "DOTLCProgress", delegate(Stream stream)
		{
			using StreamWriter streamWriter = new StreamWriter(stream);
			for (int i = 0; i < level.Length; i++)
			{
				streamWriter.WriteLine(Convert.ToString(level[i].locked));
			}
			streamWriter.WriteLine(Convert.ToString(latestLevel));
			for (int i = 0; i < challengeList.selectables.Count; i++)
			{
				streamWriter.WriteLine(Convert.ToString(challengeList.selectables[i].unlock));
			}
		});
	}

	private void readProgress()
	{
		bool flag = false;
		flag = Guide.IsTrialMode;
		if (!saveDevice.IsReady || saveDevice.IsBusy)
		{
			return;
		}
		if (saveDevice.FileExists("DOTLC", "DOTLCProgress"))
		{
			saveDevice.Load("DOTLC", "DOTLCProgress", delegate(Stream stream)
			{
				using StreamReader streamReader = new StreamReader(stream);
				try
				{
					for (int i = 0; i < level.Length; i++)
					{
						level[i].locked = bool.Parse(streamReader.ReadLine());
					}
				}
				catch
				{
				}
				try
				{
					latestLevel = int.Parse(streamReader.ReadLine());
				}
				catch
				{
				}
				try
				{
					for (int i = 0; i < challengeList.selectables.Count; i++)
					{
						challengeList.selectables[i].unlock = bool.Parse(streamReader.ReadLine());
					}
				}
				catch
				{
				}
			});
		}
		else
		{
			setDefaultProgress(write: false);
		}
	}

	private void writeUnlockables()
	{
		bool flag = false;
		if (Guide.IsTrialMode || demo || editor || !saveDevice.IsReady)
		{
			return;
		}
		saveDevice.SaveAsync("DOTLC", "DOTLCUnlockables", delegate(Stream stream)
		{
			using StreamWriter streamWriter = new StreamWriter(stream);
			streamWriter.WriteLine(Convert.ToString(unlockChubbyRain));
			streamWriter.WriteLine(Convert.ToString(unlockSidescroller));
			streamWriter.WriteLine(Convert.ToString(unlockMeteroids));
			streamWriter.WriteLine(Convert.ToString(unlockBoss));
		});
	}

	private void readUnlockables()
	{
		bool flag = false;
		flag = Guide.IsTrialMode;
		if (!saveDevice.IsReady || saveDevice.IsBusy)
		{
			return;
		}
		if (saveDevice.FileExists("DOTLC", "DOTLCUnlockables"))
		{
			saveDevice.Load("DOTLC", "DOTLCUnlockables", delegate(Stream stream)
			{
				using StreamReader streamReader = new StreamReader(stream);
				try
				{
					unlockChubbyRain = bool.Parse(streamReader.ReadLine());
					unlockSidescroller = bool.Parse(streamReader.ReadLine());
					unlockMeteroids = bool.Parse(streamReader.ReadLine());
					unlockBoss = bool.Parse(streamReader.ReadLine());
				}
				catch
				{
				}
			});
		}
		else
		{
			unlockChubbyRain = false;
			unlockSidescroller = false;
			unlockMeteroids = false;
			unlockBoss = false;
		}
	}

	private void writeOptions()
	{
		if (!saveDevice.IsReady)
		{
			return;
		}
		saveDevice.SaveAsync("DOTLC", "DOTLCOptions", delegate(Stream stream)
		{
			using StreamWriter streamWriter = new StreamWriter(stream);
			streamWriter.WriteLine(Convert.ToString(graphics.IsFullScreen));
			streamWriter.WriteLine(Convert.ToString((int)GOmusicVolume));
			streamWriter.WriteLine(Convert.ToString((int)GOsoundFXvolume));
			streamWriter.WriteLine(Convert.ToString((int)GOHUDopacity));
			streamWriter.WriteLine(Convert.ToString(graphics.PreferredBackBufferWidth));
			streamWriter.WriteLine(Convert.ToString(graphics.PreferredBackBufferHeight));
			streamWriter.WriteLine(Convert.ToString(GOvibration));
			streamWriter.WriteLine(Convert.ToString((int)keyboardPlayer));
			streamWriter.WriteLine(Convert.ToString((int)GOdifficulty));
			streamWriter.WriteLine(Convert.ToString((int)(difficulty * 100f)));
			streamWriter.WriteLine(Convert.ToString(firstTime));
		});
	}

	private void readOptions()
	{
		if (saveDevice.IsReady && !saveDevice.IsBusy)
		{
			try
			{
				if (saveDevice.FileExists("DOTLC", "DOTLCOptions"))
				{
					saveDevice.Load("DOTLC", "DOTLCOptions", delegate(Stream stream)
					{
						using StreamReader streamReader = new StreamReader(stream);
						try
						{
							GOfullScreen = bool.Parse(streamReader.ReadLine());
							GOmusicVolume = int.Parse(streamReader.ReadLine());
							GOsoundFXvolume = int.Parse(streamReader.ReadLine());
							GOHUDopacity = int.Parse(streamReader.ReadLine());
							GOresolutionX = int.Parse(streamReader.ReadLine());
							GOresolutionY = int.Parse(streamReader.ReadLine());
							GOvibration = int.Parse(streamReader.ReadLine());
							keyboardPlayer = int.Parse(streamReader.ReadLine());
							GOdifficulty = int.Parse(streamReader.ReadLine());
							difficulty = (float)int.Parse(streamReader.ReadLine()) / 100f;
							firstTime = bool.Parse(streamReader.ReadLine());
						}
						catch
						{
						}
					});
				}
				else
				{
					setDefaultOptions(write: true);
				}
				return;
			}
			catch
			{
				return;
			}
		}
		setDefaultOptions(write: true);
	}

	private void setDefaultOptions()
	{
		setDefaultOptions(write: true);
	}

	private void setDefaultOptions(bool write)
	{
		GOfullScreen = true;
		GOmusicVolume = 75f;
		GOsoundFXvolume = 100f;
		GOHUDopacity = 100f;
		GOresolutionX = 1280;
		GOresolutionY = 720;
		GOvibration = 1;
		keyboardPlayer = 1f;
		GOdifficulty = 50f;
		difficulty = 0.5f;
		firstTime = true;
		if (write)
		{
			writeOptions();
		}
	}

	private void setDefaultProgress()
	{
		setDefaultProgress(write: true);
	}

	private void setDefaultProgress(bool write)
	{
		createCharacters();
		for (int i = 0; i < 4; i++)
		{
			player[i].Reset();
			characters[i].Reset();
			player[i].HardReset();
		}
		for (int i = 1; i < challengeList.selectables.Count; i++)
		{
			challengeList.selectables[i].unlock = false;
		}
		challengeList.selectables[0].unlock = true;
		currentLevel = 0;
		setMessages(b: false);
		for (int j = 2; j < level.Length; j++)
		{
			level[j].locked = true;
		}
		copyCharactersToPlayers();
		if (write)
		{
			writeCharacters();
			writeProgress();
		}
	}

	private void repositionSelectBox()
	{
		int num = 1;
		int num2 = 6;
		if (base.GraphicsDevice.Viewport.Width < 1024)
		{
			num = 0;
			num2 = 4;
		}
		if (base.GraphicsDevice.Viewport.Width > 1280)
		{
			num = 2;
			num2 = 8;
		}
		for (int i = 0; i < 4; i++)
		{
			Vector2 zero = Vector2.Zero;
			zero.Y = base.GraphicsDevice.Viewport.Height / 2 - txCharactBox[0].Height / 2;
			int num3 = base.GraphicsDevice.Viewport.Width / num2;
			zero.X = (float)(num3 * (num + i)) + (float)num3 * 0.5f - (float)(txCharactBox[0].Width / 2);
			selectBox[i].position = zero;
			selectBox[i].size = new Vector2(1f, 1f);
		}
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		skipUpdate = Guide.IsVisible;
		if (Guide.IsTrialMode)
		{
			demo = true;
		}
		else
		{
			demo = false;
		}
		if (!skipUpdate && !isInitializing && !isLoad)
		{
			if (updateFrame)
			{
				frame++;
			}
			if (frame > 100 && gameState != gameStateNext)
			{
				gameState = gameStateNext;
			}
			if (resumeUpdate)
			{
				MediaPlayer.Resume();
				resumeUpdate = false;
			}
			if (gameState != GameState.pause)
			{
				elapsedPauseTime = elapsedTime;
				if (numPlayers > 0)
				{
					currTime = DateTime.Now;
				}
				if (numPlayers > 0)
				{
					elapsedTime = currTime - startTime;
				}
				if (elapsedTime.Seconds != elapsedPauseTime.Seconds)
				{
					seconds++;
				}
				if (seconds >= 60)
				{
					seconds = 0;
					minutes++;
				}
			}
			else
			{
				currTime = DateTime.Now;
				startTime = currTime;
				elapsedTime = currTime - startTime;
				elapsedPauseTime = elapsedTime;
			}
			switch (gameState)
			{
			case GameState.mainMenu:
			case GameState.optionsMenu:
			case GameState.playMenu:
			case GameState.galaxyMap:
			case GameState.win:
			case GameState.lose:
			case GameState.howtoplay:
			case GameState.credits:
			case GameState.score:
			case GameState.SurvivalStats:
			case GameState.LooseLevel:
				controllingPlayer = controllingPlayerMenus;
				break;
			}
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(controllingPlayer);
			if (cheating)
			{
				for (int i = 0; i < player.Length; i++)
				{
					player[i].credits = 1000f;
					player[i].Health = player[i].maximunHealth;
					player[i].experience++;
					player[i].SA = 10;
				}
				colony.healthTarget = colony.MaximunHealth;
			}
			if (confirm.text == "")
			{
				if (gameState == gameStateNext)
				{
					switch (gameState)
					{
					case GameState.exit:
						if (demo)
						{
							gameStateNext = GameState.buyGame1;
						}
						else
						{
							Exit();
						}
						break;
					case GameState.intro:
						UpdateIntro();
						menuActive = 0f;
						break;
					case GameState.ending:
						UpdateEnding();
						menuActive = 0f;
						break;
					case GameState.galaxyMap:
						UpdateGalaxyMap();
						break;
					case GameState.selectPlayer:
						UpdateSelectPlayer();
						break;
					case GameState.message:
						UpdateMessageInfo(gameTime);
						break;
					case GameState.Campaign:
						UpdateCampaign(gameTime);
						menuActive = 0f;
						break;
					case GameState.Versus:
						UpdateVSmode(gameTime);
						menuActive = 0f;
						break;
					case GameState.Survival:
						UpdateSurvivalMode(gameTime);
						menuActive = 0f;
						break;
					case GameState.Meteroids:
						UpdateMeteroidsMode(gameTime);
						menuActive = 0f;
						break;
					case GameState.prepareFinalBoss:
						UpdatePrepareFinalBoss();
						menuActive = 0f;
						break;
					case GameState.finalBoss:
						UpdateFinalBoss(gameTime);
						menuActive = 0f;
						break;
					case GameState.ChubbyRain:
						UpdateChubbyRainMode(gameTime);
						menuActive = 0f;
						break;
					case GameState.Sidescroller:
						UpdateSidescrollerMode(gameTime);
						menuActive = 0f;
						break;
					case GameState.Challenge:
						UpdateChallenge(gameTime);
						menuActive = 0f;
						break;
					case GameState.howtoplay:
						UpdateHowToPlay();
						break;
					case GameState.controls:
						UpdateControls();
						break;
					case GameState.awards:
						UpdateAwards();
						break;
					case GameState.selectChallenge:
						UpdateSelectChallenge();
						break;
					case GameState.credits:
						updateCredits();
						menuActive = 0f;
						break;
					case GameState.logo:
						updateLogo();
						menuActive = 0f;
						break;
					case GameState.disclaimer:
						updateDisclaimer();
						break;
					case GameState.startScreen:
						UpdateStartScreen();
						menuActive = 0f;
						break;
					case GameState.clean:
						UpdateCleanGarbage();
						menuActive = 0f;
						break;
					case GameState.mainMenu:
						UpdateMenu();
						menuActive = MathHelper.SmoothStep(menuActive, 1f, 0.25f);
						break;
					case GameState.playMenu:
						UpdatePlayMenu();
						menuActive = MathHelper.SmoothStep(menuActive, 1f, 0.25f);
						break;
					case GameState.optionsMenu:
						UpdateOptionsMenu();
						menuActive = MathHelper.SmoothStep(menuActive, 1f, 0.25f);
						break;
					case GameState.optionsReseted:
						UpdateOptionReseted();
						menuActive = MathHelper.SmoothStep(menuActive, 1f, 0.25f);
						break;
					case GameState.progressReseted:
						UpdateProgressReseted();
						menuActive = MathHelper.SmoothStep(menuActive, 1f, 0.25f);
						break;
					case GameState.challengeFinished:
						UpdateChallengeFinished();
						menuActive = MathHelper.SmoothStep(menuActive, 1f, 0.15f);
						break;
					case GameState.score:
						switch (gameStatePlay)
						{
						case GameState.Campaign:
							UpdateCamera(gameTime);
							UpdateLooseCampaign();
							break;
						case GameState.Survival:
							UpdateSurvivalStats();
							break;
						}
						break;
					case GameState.pause:
						if (gameStatePlay == GameState.Campaign)
						{
							pauseMenu[toGalaxyID].selectable = true;
						}
						if (gameStatePlay == GameState.Survival || gameStatePlay == GameState.ChubbyRain || gameStatePlay == GameState.Sidescroller)
						{
							pauseMenu[toGalaxyID].selectable = false;
						}
						updatePause();
						menuActive = 0f;
						break;
					case GameState.buyGame1:
						UpdateBuyGame(exit: false);
						break;
					case GameState.buyGame2:
						UpdateBuyGame(exit: true);
						break;
					case GameState.endDemo1:
					case GameState.endDemo2:
						UpdateEndDemo();
						break;
					case GameState.BonusChubbyRain:
					case GameState.BonusMeteroids:
					case GameState.BonusSidescroller:
						updateBonus();
						break;
					case GameState.BonusClear:
					case GameState.BonusFailed:
						updateBonusClear();
						break;
					default:
						gameStateNext = GameState.disclaimer;
						break;
					}
				}
				else
				{
					checkUpdate++;
					if (checkUpdate > 30)
					{
						gameState = gameStateNext;
						checkUpdate = 0u;
					}
				}
			}
			else
			{
				gameStateNext = confirm.Update(controllingPlayer, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f));
				if (gameStateNext != gameState)
				{
					controllingPlayerMenus = controllingPlayer;
				}
				UpdateMouseUI();
			}
			if (cheating)
			{
				for (int i = 0; i < player.Length; i++)
				{
					player[i].credits = 1000f;
					player[i].Health = player[i].maximunHealth;
				}
			}
			if (gameState == gameStateNext)
			{
				fade = 0f;
			}
			else
			{
				fade = 1f;
				if (gameState != GameState.disclaimer && gameState != GameState.logo)
				{
					frame = 0u;
				}
				pausePercent = -50f;
				pauseTarget = -50f;
				resetMenus();
				if (gameStateNext != GameState.Challenge)
				{
					reset();
				}
			}
			fadeToBlack = MathHelper.Lerp(fadeToBlack, fade, 0.3f);
			if (fadeToBlack >= 0.99f && gameState != GameState.disclaimer && gameState != GameState.logo)
			{
				frame = 0u;
				round = 0u;
				sidescrollerSong = "In_a_heart_beat";
				fadeToBlack = 1f;
				writeAwards();
				writeProgress();
				writeCharacters();
				writeUnlockables();
				gameState = gameStateNext;
				changeMenuBackground();
				resetCamera(gameStateNext == GameState.galaxyMap);
				awards.selected = 0;
				controlDelay = 0u;
				GC.Collect();
			}
			awards.Update();
			oldMouseState = currentMouseState;
			oldKeyboardState = currentKeyboardState;
			oldGamePadState = currentGamePadState;
		}
		else
		{
			MediaPlayer.Pause();
			resumeUpdate = true;
		}
		base.Update(gameTime);
	}

	private void UpdateHowToPlay()
	{
		UpdateMouseUI();
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.1f);
		if (menuActive > 0.9f)
		{
			if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState != currentGamePadState && (currentGamePadState.Buttons.B == ButtonState.Pressed || currentGamePadState.Buttons.Back == ButtonState.Pressed)))
			{
				frame = 0u;
				gameStateNext = GameState.mainMenu;
				menuActive = 0f;
				howToSlide = 0;
			}
			if ((oldKeyboardState != currentKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Enter) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Right))) || (oldGamePadState != currentGamePadState && (currentGamePadState.ThumbSticks.Left.X > 0.5f || currentGamePadState.Buttons.A == ButtonState.Pressed || currentGamePadState.DPad.Right == ButtonState.Pressed)) || (currentMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed))
			{
				howToSlide++;
			}
			if ((oldKeyboardState != currentKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Back) || currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.Escape))) || (oldGamePadState != currentGamePadState && (currentGamePadState.ThumbSticks.Left.X < -0.5f || currentGamePadState.DPad.Left == ButtonState.Pressed)) || (currentMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton != ButtonState.Pressed))
			{
				howToSlide--;
			}
		}
		if (howToSlide >= howToPlay.Count)
		{
			howToSlide = 0;
		}
		if (howToSlide < 0)
		{
			howToSlide = howToPlay.Count - 1;
		}
		for (int i = 0; i < howToPlay.Count; i++)
		{
			if (howToSlide == i)
			{
				howToPlay[i].transp = MathHelper.Lerp(howToPlay[i].transp, 1f, 0.1f);
			}
			else
			{
				howToPlay[i].transp = MathHelper.Lerp(howToPlay[i].transp, 0f, 0.1f);
			}
		}
	}

	private void UpdateControls()
	{
		UpdateMouseUI();
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.1f);
		if (menuActive > 0.9f)
		{
			if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState != currentGamePadState && (currentGamePadState.Buttons.B == ButtonState.Pressed || currentGamePadState.Buttons.Back == ButtonState.Pressed)))
			{
				frame = 0u;
				gameStateNext = GameState.mainMenu;
				menuActive = 0f;
				controlsSlide = 0;
			}
			if ((oldKeyboardState != currentKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Enter) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Right))) || (oldGamePadState != currentGamePadState && (currentGamePadState.ThumbSticks.Left.X > 0.5f || currentGamePadState.Buttons.A == ButtonState.Pressed || currentGamePadState.DPad.Right == ButtonState.Pressed)) || (currentMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton != ButtonState.Pressed))
			{
				controlsSlide++;
			}
			if ((oldKeyboardState != currentKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Back) || currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.Escape))) || (oldGamePadState != currentGamePadState && (currentGamePadState.ThumbSticks.Left.X < -0.5f || currentGamePadState.DPad.Left == ButtonState.Pressed)) || (currentMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton != ButtonState.Pressed))
			{
				controlsSlide--;
			}
		}
		if (controlsSlide >= controls.Count)
		{
			controlsSlide = 0;
		}
		if (controlsSlide < 0)
		{
			controlsSlide = controls.Count - 1;
		}
		for (int i = 0; i < controls.Count; i++)
		{
			if (controlsSlide == i)
			{
				controls[i].transp = MathHelper.Lerp(controls[i].transp, 1f, 0.1f);
			}
			else
			{
				controls[i].transp = MathHelper.Lerp(controls[i].transp, 0f, 0.1f);
			}
		}
	}

	private void UpdateAwards()
	{
		UpdateMouseUI();
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.1f);
		ControlThumbsticks();
		if (mouseTransp > 0.1f)
		{
			for (int i = 0; i < awards.Data.Count; i++)
			{
				if (awards.isMouseOver(i, new Vector2(currentMouseState.X, currentMouseState.Y)))
				{
					awards.selected = i;
				}
			}
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Left) && currentKeyboardState.IsKeyDown(Keys.Left)) || (oldGamePadState.DPad.Left != ButtonState.Pressed && currentGamePadState.DPad.Left == ButtonState.Pressed) || left)
		{
			int selected = awards.selected;
			if (selected == 4 || selected == 9 || selected == 14)
			{
				awards.selected -= 4;
			}
			else
			{
				awards.selected++;
			}
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Right) && currentKeyboardState.IsKeyDown(Keys.Right)) || (oldGamePadState.DPad.Right != ButtonState.Pressed && currentGamePadState.DPad.Right == ButtonState.Pressed) || right)
		{
			int selected = awards.selected;
			if (selected == 0 || selected == 5 || selected == 10)
			{
				awards.selected += 4;
			}
			else
			{
				awards.selected--;
			}
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState.IsKeyDown(Keys.Up)) || (oldGamePadState.DPad.Up != ButtonState.Pressed && currentGamePadState.DPad.Up == ButtonState.Pressed) || up)
		{
			awards.selected -= 5;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState.IsKeyDown(Keys.Down)) || (oldGamePadState.DPad.Down != ButtonState.Pressed && currentGamePadState.DPad.Down == ButtonState.Pressed) || down)
		{
			awards.selected += 5;
		}
		if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState != currentGamePadState && currentGamePadState.Buttons.B == ButtonState.Pressed) || currentMouseState.RightButton == ButtonState.Pressed)
		{
			frame = 0u;
			gameStateNext = GameState.mainMenu;
			menuActive = 0f;
		}
	}

	private void UpdateBuyGame(bool exit)
	{
		UpdateMouseUI();
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.05f);
		if (!(menuActive > 0.85f))
		{
			return;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyDown(Keys.Enter)) || (!oldKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyDown(Keys.Space)) || (oldGamePadState.Buttons.Start != ButtonState.Pressed && currentGamePadState.Buttons.Start == ButtonState.Pressed) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || currentMouseState.LeftButton == ButtonState.Pressed)
		{
			frame = 0u;
			gameStateNext = GameState.buyGame2;
			menuActive = 0f;
			if (exit)
			{
				Exit();
			}
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed) || currentMouseState.RightButton == ButtonState.Pressed)
		{
			frame = 0u;
			gameStateNext = GameState.mainMenu;
			menuActive = 0f;
		}
		if (Guide.IsTrialMode && currentGamePadState.IsButtonDown(Buttons.Y))
		{
			try
			{
				Guide.ShowMarketplace(controllingPlayer);
			}
			catch
			{
			}
		}
	}

	private void UpdateEndDemo()
	{
		UpdateMouseUI();
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.05f);
		if (!(menuActive > 0.85f))
		{
			return;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyDown(Keys.Enter)) || (!oldKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyDown(Keys.Space)) || (oldGamePadState.Buttons.Start != ButtonState.Pressed && currentGamePadState.Buttons.Start == ButtonState.Pressed) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || currentMouseState.LeftButton == ButtonState.Pressed)
		{
			frame = 0u;
			if (gameState == GameState.endDemo2)
			{
				gameStateNext = GameState.mainMenu;
			}
			else
			{
				gameStateNext = GameState.endDemo2;
			}
			menuActive = 0f;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed) || currentMouseState.RightButton == ButtonState.Pressed)
		{
			frame = 0u;
			if (gameState == GameState.endDemo2)
			{
				gameStateNext = GameState.mainMenu;
			}
			else
			{
				gameStateNext = GameState.endDemo2;
			}
			menuActive = 0f;
		}
		if (Guide.IsTrialMode && currentGamePadState.IsButtonDown(Buttons.Y))
		{
			try
			{
				Guide.ShowMarketplace(controllingPlayer);
			}
			catch
			{
			}
		}
	}

	private void ControlThumbsticks()
	{
		if (controlDelay != 0)
		{
			controlDelay--;
		}
		left = false;
		right = false;
		up = false;
		down = false;
		if (currentGamePadState.ThumbSticks.Left.X < -0.1f && controlDelay == 0)
		{
			controlDelay = 20u;
			left = true;
		}
		if (currentGamePadState.ThumbSticks.Left.X > 0.1f && controlDelay == 0)
		{
			controlDelay = 20u;
			right = true;
		}
		if (currentGamePadState.ThumbSticks.Left.Y < -0.1f && controlDelay == 0)
		{
			controlDelay = 20u;
			down = true;
		}
		if (currentGamePadState.ThumbSticks.Left.Y > 0.1f && controlDelay == 0)
		{
			controlDelay = 20u;
			up = true;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Left) && currentKeyboardState.IsKeyDown(Keys.Left)) || (oldGamePadState.DPad.Left != ButtonState.Pressed && currentGamePadState.DPad.Left == ButtonState.Pressed))
		{
			left = true;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Right) && currentKeyboardState.IsKeyDown(Keys.Right)) || (oldGamePadState.DPad.Right != ButtonState.Pressed && currentGamePadState.DPad.Right == ButtonState.Pressed))
		{
			right = true;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState.IsKeyDown(Keys.Up)) || (oldGamePadState.DPad.Up != ButtonState.Pressed && currentGamePadState.DPad.Up == ButtonState.Pressed))
		{
			up = true;
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState.IsKeyDown(Keys.Down)) || (oldGamePadState.DPad.Down != ButtonState.Pressed && currentGamePadState.DPad.Down == ButtonState.Pressed))
		{
			down = true;
		}
	}

	private void UpdateSelectChallenge()
	{
		UpdateSelectBackground();
		UpdateMouseUI();
		endGame = 0;
		reset();
		maxEnemies = topEnemies * 2;
		challengeClear = false;
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.05f);
		if (GOmusicVolume > 0f)
		{
			PlaySong("TimeToRun");
		}
		if (editor)
		{
			for (int i = 0; i < challengeList.selectables.Count; i++)
			{
				challengeList.selectables[i].unlock = true;
			}
		}
		int num = challengeList.Selected(new Vector2(currentMouseState.X, currentMouseState.Y), mouseTransp);
		challengeUI.Selected(new Vector2(currentMouseState.X, currentMouseState.Y), mouseTransp);
		if (oldMouseState.LeftButton != ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed && num >= 0)
		{
			challengeNumber = num;
			createChallengeLevel();
			gameStateNext = GameState.Challenge;
			menuActive = 0f;
		}
		ControlThumbsticks();
		if (down)
		{
			challengeNumber++;
		}
		if (up)
		{
			challengeNumber--;
		}
		if (right)
		{
			challengeNumber += 10;
		}
		if (left)
		{
			challengeNumber -= 10;
		}
		if (challengeNumber >= 30)
		{
			challengeNumber -= 30;
		}
		if (challengeNumber < 0)
		{
			challengeNumber += 30;
		}
		if (Guide.IsTrialMode && currentGamePadState.IsButtonDown(Buttons.Y))
		{
			try
			{
				Guide.ShowMarketplace(controllingPlayer);
			}
			catch
			{
			}
		}
		if ((!oldKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed))
		{
			frame = 0u;
			gameStateNext = GameState.playMenu;
			menuActive = 0f;
		}
		if ((challengeList.selectables[challengeNumber].unlock || editor) && ((!oldKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyDown(Keys.Enter)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed)))
		{
			createChallengeLevel();
			gameStateNext = GameState.Challenge;
			menuActive = 0f;
		}
	}

	private bool Left()
	{
		return (!oldKeyboardState.IsKeyDown(Keys.Left) && currentKeyboardState.IsKeyDown(Keys.Left)) || (oldGamePadState.DPad.Left != ButtonState.Pressed && currentGamePadState.DPad.Left == ButtonState.Pressed);
	}

	private bool Right()
	{
		return (!oldKeyboardState.IsKeyDown(Keys.Right) && currentKeyboardState.IsKeyDown(Keys.Right)) || (oldGamePadState.DPad.Right != ButtonState.Pressed && currentGamePadState.DPad.Right == ButtonState.Pressed);
	}

	private bool Up()
	{
		return (!oldKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState.IsKeyDown(Keys.Up)) || (oldGamePadState.DPad.Up != ButtonState.Pressed && currentGamePadState.DPad.Up == ButtonState.Pressed);
	}

	private bool Down()
	{
		return (!oldKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState.IsKeyDown(Keys.Down)) || (oldGamePadState.DPad.Down != ButtonState.Pressed && currentGamePadState.DPad.Down == ButtonState.Pressed);
	}

	private void updateCredits()
	{
		PlaySong("ReachTheStars");
		if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState != currentGamePadState && currentGamePadState.Buttons.B == ButtonState.Pressed))
		{
			beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
			gameStateNext = GameState.mainMenu;
		}
	}

	private void updateDisclaimer()
	{
		if (frame > 15 && frame < 21 && frame % 2 == 0)
		{
			ReadAll();
		}
		if (disclaimerTimer > 20)
		{
			for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
			{
				if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed || (currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Escape) || currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed)
				{
					frame = 0u;
					gameStateNext = GameState.intro;
					disclaimerTimer = 1000;
					break;
				}
			}
		}
		disclaimerTimer++;
		if (disclaimerTimer < 280)
		{
			disclaimerTransp = MathHelper.Lerp(disclaimerTransp, 1f, 0.3f);
		}
		else
		{
			if (disclaimerTransp > 0f)
			{
				disclaimerTransp -= 0.01f;
			}
			if (disclaimerTransp <= 0.02f)
			{
				frame = 0u;
				gameStateNext = GameState.intro;
			}
		}
		ResetVibration();
	}

	private void ReadAll()
	{
		bool flag = false;
		flag = Guide.IsTrialMode;
		readOptions();
		if (!flag)
		{
			readCharacters();
			readProgress();
			readAwards();
			readUnlockables();
		}
	}

	public void changeMenuBackground()
	{
		switch (random.Next(12))
		{
		case 0:
			menuBackground.texture = txHines;
			break;
		case 1:
			menuBackground.texture = txNymeriah;
			break;
		case 2:
			menuBackground.texture = txHerschel;
			break;
		case 3:
			menuBackground.texture = txDanae;
			break;
		case 4:
			menuBackground.texture = txClarke;
			break;
		case 5:
			menuBackground.texture = txGeaMoon;
			break;
		case 6:
			menuBackground.texture = txCalypso;
			break;
		case 7:
			menuBackground.texture = txBradbury;
			break;
		case 8:
			menuBackground.texture = txEosRest;
			break;
		case 9:
			menuBackground.texture = txOlbers4;
			break;
		case 10:
			menuBackground.texture = txEneas;
			break;
		case 11:
			menuBackground.texture = txPrometheus;
			break;
		default:
			background.texture = txMenuBackground;
			break;
		}
	}

	public void updateLogo()
	{
		updateFrame = true;
		ResetVibration();
		PlaySong("MainTheme");
		if (frame < 250)
		{
			kplogo.transparency = MathHelper.Lerp(kplogo.transparency, 1f, 0.05f);
			kplogo.size = new Vector2(MathHelper.Lerp(kplogo.size.X, 1f, 0.05f));
		}
		else
		{
			kplogo.transparency = MathHelper.Lerp(kplogo.transparency, 0f, 0.05f);
			kplogo.size = new Vector2(MathHelper.Lerp(kplogo.size.X, 1.2f, 0.05f));
		}
		kplogo.position = new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height / 2);
		kplogo.Update();
		if (frame > 250 && kplogo.transparency < 0.05f)
		{
			kplogo.size = new Vector2(0.8f);
			kplogo.transparency = 0f;
			gameStateNext = GameState.intro;
		}
		if (frame > 30)
		{
			for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
			{
				if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed || (currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Escape) || currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed)
				{
					kplogo.size = new Vector2(0.8f);
					kplogo.transparency = 0f;
					kplogo.Update();
					frame = 0u;
					gameStateNext = GameState.intro;
					break;
				}
			}
		}
		if (frame > 250 && kplogo.transparency < 0.05f)
		{
			kplogo.size = new Vector2(0.8f);
			kplogo.transparency = 0f;
			kplogo.Update();
			frame = 0u;
			gameStateNext = GameState.intro;
		}
	}

	public void UpdateStartScreen()
	{
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(controllingPlayer);
		updateFrame = true;
		ResetVibration();
		UpdateMouseUI();
		if (GOmusicVolume > 0f)
		{
			PlaySong("MainTheme");
		}
		if (frame > 1550)
		{
			frame = 0u;
			gameStateNext = GameState.logo;
		}
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed)
			{
				beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
				controllingPlayer = playerIndex;
				controllingPlayerMenus = playerIndex;
				gamer = Gamer.SignedInGamers[controllingPlayer];
				if (gamer == null)
				{
					Guide.ShowSignIn(1, onlineOnly: false);
				}
			}
		}
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(controllingPlayer);
		if ((oldMouseState.LeftButton != currentMouseState.LeftButton && currentMouseState.LeftButton == ButtonState.Pressed) || (oldMouseState.RightButton != currentMouseState.RightButton && currentMouseState.RightButton == ButtonState.Pressed) || (oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState != currentGamePadState && (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentGamePadState.Buttons.A == ButtonState.Pressed)))
		{
			beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
			frame = 0u;
			tittle.position = Vector2.Zero;
			tittle.transparency = 0f;
			gameStateNext = GameState.mainMenu;
			gameState = GameState.mainMenu;
		}
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
	}

	private void updateBonus()
	{
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.015f);
		if (menuActive > 0.9f)
		{
			gameStateNext = gameStatePlay;
		}
	}

	private void updateBonusClear()
	{
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.015f);
		if (menuActive > 0.9f)
		{
			winCondition = objective.none;
			gameStateNext = gameStatePlay;
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].wasActive)
			{
				player[i].Health = player[i].maximunHealth;
				player[i].Active = true;
			}
		}
	}

	private void updatePause()
	{
		pausePercent = MathHelper.Lerp(pausePercent, pauseTarget, 0.5f);
		if (pausePercent <= -49f)
		{
			gameState = gameStatePlay;
			gameStateNext = gameStatePlay;
			resetMenus();
			updateVolume(1f);
		}
		if (pausePercent >= 99f)
		{
			if (currentGamePadState.Buttons != oldGamePadState.Buttons && (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentGamePadState.Buttons.Back == ButtonState.Pressed || currentGamePadState.Buttons.B == ButtonState.Pressed))
			{
				pauseTarget = -50f;
			}
			if (oldKeyboardState.IsKeyUp(Keys.Escape) && currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				pauseTarget = -50f;
			}
			UpdatePauseMenu();
			updateVolume(0.25f);
		}
	}

	public void UpdateGalaxyMap()
	{
		PlaySong("MainTheme");
		updateFrame = true;
		UpdateMouseUI();
		camera.limits = Rectangle.Empty;
		if (Guide.IsTrialMode && currentGamePadState.IsButtonDown(Buttons.Y))
		{
			try
			{
				Guide.ShowMarketplace(controllingPlayer);
			}
			catch
			{
			}
		}
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.1f);
		round = 0u;
		winCondition = objective.none;
		colony.damaged = false;
		menuDelay++;
		float num = 231f;
		float num2 = -143f;
		colony.energy = 0f;
		colony.energyTarget = 0f;
		colony.healthTarget = colony.MaximunHealth;
		colony.health = colony.MaximunHealth;
		int num3 = 100;
		bool flag = false;
		for (int i = 0; i < level.Length; i++)
		{
			if (!level[i].locked)
			{
				num3 = i;
			}
			level[i].Update(i == currentLevel);
			if (currentMouseState.LeftButton != oldMouseState.LeftButton && currentMouseState.LeftButton == ButtonState.Pressed && Vector2.Distance(level[i].position, camera.get_mouse_vpos(base.GraphicsDevice)) < 50f && (!level[currentLevel].briefing || i == currentLevel))
			{
				if (i == currentLevel)
				{
					flag = true;
				}
				currentLevel = i;
				nextLevel = i;
			}
		}
		num3++;
		if (num3 > 12)
		{
			num3 = 12;
		}
		if (currentLevel > num3)
		{
			currentLevel = num3;
		}
		if (nextLevel > num3)
		{
			nextLevel = num3;
		}
		camera.position.X = MathHelper.SmoothStep(camera.position.X, level[currentLevel].position.X / 2f, 0.1f);
		camera.position.Y = MathHelper.SmoothStep(camera.position.Y, level[currentLevel].position.Y / 2f, 0.055f);
		camera.Rotation = 0f;
		if (level[currentLevel].briefing)
		{
			camera.Zoom = MathHelper.SmoothStep(camera.Zoom, level[currentLevel].cameraZoom * 2f, 0.15f);
			selectPlanetExt.size = new Vector2(MathHelper.SmoothStep(selectPlanetExt.size.X, 0.8f, 0.075f));
			selectPlanetInt.size = new Vector2(MathHelper.SmoothStep(selectPlanetInt.size.X, 1.1f, 0.075f));
			num = -321f;
			num2 = 583f;
		}
		else
		{
			camera.Zoom = MathHelper.SmoothStep(camera.Zoom, level[currentLevel].cameraZoom, 0.075f);
			selectPlanetExt.size = new Vector2(MathHelper.SmoothStep(selectPlanetExt.size.X, 2f, 0.15f));
			selectPlanetInt.size = new Vector2(MathHelper.SmoothStep(selectPlanetInt.size.X, 1.8f, 0.15f));
			num = 231f;
			num2 = -143f;
		}
		stars.position = camera.position / 2f - new Vector2(stars.Width / 2, stars.Height / 2);
		stars2.position = camera.position / 3f - new Vector2(stars2.Width / 2, stars2.Height / 2);
		UpdateLens(12);
		selectPlanetExt.position.X = MathHelper.SmoothStep(selectPlanetExt.position.X, level[currentLevel].position.X, 0.35f);
		selectPlanetExt.position.Y = MathHelper.SmoothStep(selectPlanetExt.position.Y, level[currentLevel].position.Y, 0.35f);
		selectPlanetInt.position.X = MathHelper.SmoothStep(selectPlanetInt.position.X, level[currentLevel].position.X, 0.15f);
		selectPlanetInt.position.Y = MathHelper.SmoothStep(selectPlanetInt.position.Y, level[currentLevel].position.Y, 0.15f);
		selectPlanetExt.angle = MathHelper.WrapAngle((float)frame / num);
		selectPlanetInt.angle = MathHelper.WrapAngle((float)frame / num2);
		if (frame > 40 && currentLevel != nextLevel)
		{
			beepHSound.Play(GOsoundFXvolume / 400f, -1f, 0f);
			currentLevel = nextLevel;
		}
		if (frame > 10)
		{
			if (!level[currentLevel].briefing)
			{
				if (menuDelay > 10 && ((currentGamePadState != oldGamePadState && (currentGamePadState.DPad.Left == ButtonState.Pressed || currentGamePadState.ThumbSticks.Left.X < -0.5f)) || (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Left))))
				{
					direccion = -1;
					menuDelay = 0;
				}
				if (menuDelay > 10 && ((currentGamePadState != oldGamePadState && (currentGamePadState.DPad.Right == ButtonState.Pressed || currentGamePadState.ThumbSticks.Left.X > 0.5f)) || (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Right))))
				{
					direccion = 1;
					menuDelay = 0;
				}
				if (direccion == -1)
				{
					if (currentLevel > 0)
					{
						beepHSound.Play(GOsoundFXvolume / 400f, -0.5f, 0f);
					}
					nextLevel--;
					direccion = 0;
				}
				if (direccion == 1)
				{
					if (currentLevel < 11 && !level[currentLevel].locked)
					{
						beepHSound.Play(GOsoundFXvolume / 400f, 0f, 0f);
						nextLevel++;
					}
					direccion = 0;
				}
			}
			if ((currentMouseState != oldMouseState && flag) || (currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.A == ButtonState.Pressed) || (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)))
			{
				beepSSound.Play(GOsoundFXvolume / 200f, 1f, 0f);
				if (!level[currentLevel].briefing)
				{
					level[currentLevel].briefing = true;
					frame = 0u;
				}
				else if (!level[currentLevel].locked)
				{
					assetManager[currentLevel].asset = ReadWorld();
					frame = 0u;
					for (int i = 0; i < 12; i++)
					{
						level[i].ResetLevel();
					}
					createLevel(characters: false);
					if (currentLevel == 0)
					{
						setMessages();
					}
					gameStatePlay = GameState.Campaign;
					gameStateNext = GameState.Campaign;
				}
			}
			if ((currentMouseState.RightButton != oldMouseState.RightButton && currentMouseState.RightButton == ButtonState.Pressed) || (currentGamePadState != oldGamePadState && currentGamePadState.Buttons.B == ButtonState.Pressed) || (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape)))
			{
				beepSSound.Play(GOsoundFXvolume / 200f, 0.5f, 0f);
				if (!level[currentLevel].briefing)
				{
					resetPlayers();
					frame = 0u;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.galaxyMap;
				}
				else
				{
					level[currentLevel].briefing = false;
					frame = 0u;
				}
			}
		}
		if (demo)
		{
			currentLevel = (int)MathHelper.Clamp(currentLevel, 0f, maxDemoLevel);
			nextLevel = (int)MathHelper.Clamp(nextLevel, 0f, maxDemoLevel);
		}
		else
		{
			currentLevel = (int)MathHelper.Clamp(currentLevel, 0f, 11f);
			nextLevel = (int)MathHelper.Clamp(nextLevel, 0f, 11f);
		}
	}

	private void UpdateSelectPlayer()
	{
		updateFrame = true;
		keyboardPlayer = MathHelper.Clamp(keyboardPlayer, 1f, 4f);
		menuActive = MathHelper.Lerp(menuActive, 1f, 0.1f);
		UpdateSelectBackground();
		copyCharactersToPlayers();
		for (int i = 0; i < 4; i++)
		{
			characterSel[i] = -1;
		}
		for (int i = 0; i < 4; i++)
		{
			if (player[i].selected)
			{
				characterSel[player[i].number] = i;
			}
		}
		if (GOmusicVolume > 0f)
		{
			PlaySong("MainTheme");
		}
		UpdateMouseUI();
		if (frame <= 30)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (!player[0].Active && !player[1].Active && !player[2].Active && !player[3].Active && ((currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed) || (currentGamePadState != oldGamePadState && currentGamePadState.Buttons.B == ButtonState.Pressed) || (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape))))
			{
				beepSSound.Play(GOsoundFXvolume / 200f, -0.5f, 0f);
				oldKeyboardState = currentKeyboardState;
				oldGamePadState = currentGamePadState;
				frame = 0u;
				gameStateNext = GameState.playMenu;
			}
			if (player[i].ready)
			{
				beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
				player[i].ready = false;
				player[i].selected = false;
				frame = 0u;
				camera.Zoom = 0.25f;
				direccion = 0;
				fadeToBlack = 0.5f;
				controllingPlayer = player[i].index;
				controllingPlayerMenus = controllingPlayer;
				gameStateNext = gameStatePlay;
				switch (gameStatePlay)
				{
				case GameState.Campaign:
					createLevel(characters: false);
					if (currentLevel == 14)
					{
						gameStateNext = GameState.Campaign;
					}
					else
					{
						gameStateNext = GameState.galaxyMap;
					}
					break;
				case GameState.finalBoss:
					createLevel(characters: false);
					break;
				case GameState.Survival:
					createSurvivalLevel(characters: false);
					break;
				case GameState.ChubbyRain:
					gameStateNext = GameState.BonusChubbyRain;
					break;
				case GameState.Meteroids:
					gameStateNext = GameState.BonusMeteroids;
					break;
				case GameState.Sidescroller:
					gameStateNext = GameState.BonusSidescroller;
					break;
				default:
					createSurvivalLevel(characters: false);
					break;
				}
			}
			else
			{
				if (player[i].number < 0)
				{
					player[i].number = 3;
				}
				if (player[i].number > 3)
				{
					player[i].number = 0;
				}
				if (!player[i].ready)
				{
					player[i].UpdateCharSelection((float)i == keyboardPlayer - 1f, new Vector2(currentMouseState.X, currentMouseState.Y), characterSel[player[i].number] >= 0);
				}
				if (characterSel[player[i].number] >= 0)
				{
					player[i].number += player[i].oldDireccion;
				}
				if (player[i].number < 0)
				{
					player[i].number = 3;
				}
				if (player[i].number > 3)
				{
					player[i].number = 0;
				}
				switch (player[i].characters[player[i].number].shipClass)
				{
				case "Fighter":
					player[i].texture = txFighter[i];
					break;
				case "Defender":
					player[i].texture = txIngeneer[i];
					break;
				default:
					player[i].texture = txIngeneer[i];
					break;
				}
			}
		}
	}

	private void UpdateSelectBackground()
	{
		selectBackground.size = new Vector2((float)base.GraphicsDevice.Viewport.Width / (float)txSelectBack.Width, (float)base.GraphicsDevice.Viewport.Height / (float)txSelectBack.Height);
		selectHUD.size = new Vector2((float)base.GraphicsDevice.Viewport.Width / (float)txSelectHUD.Width, (float)base.GraphicsDevice.Viewport.Height / (float)txSelectHUD.Height);
		selectBar.position.X++;
		if (selectBar.position.X > (float)(base.GraphicsDevice.Viewport.Width + selectBar.Width))
		{
			selectBar.position.X = -selectBar.Width;
		}
		selectBar.size = new Vector2(1f, (float)(base.GraphicsDevice.Viewport.Height / 2) * menuActive);
		selectBar2.position.X += 1.77f;
		if (selectBar2.position.X > (float)(base.GraphicsDevice.Viewport.Width + selectBar2.Width))
		{
			selectBar2.position.X = -selectBar2.Width;
		}
		selectBar2.size = new Vector2(1f, (float)(base.GraphicsDevice.Viewport.Height / 2) * menuActive);
		selectBar.transparency = menuActive;
		selectBar2.transparency = menuActive;
		selectBackground.transparency = menuActive;
		selectHUD.transparency = menuActive;
	}

	private void UpdateSelectPlayerOLD()
	{
		updateFrame = true;
		keyboardPlayer = MathHelper.Clamp(keyboardPlayer, 1f, 4f);
		selectBackground.size = new Vector2((float)base.GraphicsDevice.Viewport.Width / (float)txSelectBack.Width, (float)base.GraphicsDevice.Viewport.Height / (float)txSelectBack.Height);
		if (GOmusicVolume > 0f)
		{
			PlaySong("MainTheme");
		}
		UpdateMouseUI();
		if (frame <= 30)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (!player[0].Active && !player[1].Active && !player[2].Active && !player[3].Active && ((currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed) || (currentGamePadState != oldGamePadState && currentGamePadState.Buttons.B == ButtonState.Pressed) || (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape))))
			{
				beepSSound.Play(GOsoundFXvolume / 200f, -0.5f, 0f);
				oldKeyboardState = currentKeyboardState;
				oldGamePadState = currentGamePadState;
				frame = 0u;
				gameStateNext = GameState.playMenu;
			}
			switch (player[i].characters[player[i].number].shipClass)
			{
			case "Fighter":
				player[i].texture = txFighter[i];
				break;
			case "Defender":
				player[i].texture = txIngeneer[i];
				break;
			default:
				player[i].texture = txIngeneer[i];
				break;
			}
		}
	}

	private void UpdateMessageInfo(GameTime gameTime)
	{
		updateFrame = false;
		messageInfoCounter = MathHelper.SmoothStep(messageInfoCounter, 100f, 0.25f);
		MessageInfoPos = new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f * (messageInfoCounter / 100f));
		UpdateMouseUI();
		Rectangle value = new Rectangle(currentMouseState.X - 1, currentMouseState.Y - 1, 2, 2);
		Rectangle rectangle = new Rectangle((int)(MessageInfoPos.X + (float)(txMessage.Width / 2)) - 40, (int)(MessageInfoPos.Y - (float)(txMessage.Height / 2)), 40, 40);
		if (messageInfoCounter > 60f)
		{
			if ((currentGamePadState != oldGamePadState && (currentGamePadState.Buttons.A == ButtonState.Pressed || currentGamePadState.Buttons.B == ButtonState.Pressed)) || (currentMouseState.RightButton != oldMouseState.RightButton && currentMouseState.RightButton == ButtonState.Pressed) || (currentKeyboardState != oldKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Enter) || currentKeyboardState.IsKeyDown(Keys.Escape))))
			{
				messageInfo.RemoveAt(0);
			}
			if (rectangle.Intersects(value) && currentMouseState.LeftButton == ButtonState.Pressed)
			{
				messageInfo.RemoveAt(0);
			}
		}
		if (messageInfo.Count == 0)
		{
			gameState = gameStatePlay;
			gameStateNext = gameState;
			updateFrame = true;
		}
	}

	private void UpdateCampaign(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			PlaySong(level[currentLevel].music);
		}
		if (editor)
		{
			updateFrame = true;
			UpdateEditorIngame(gameTime);
			UpdateItems(gameTime);
			UpdateFXs(gameTime);
		}
		else
		{
			base.IsMouseVisible = false;
			UpdateBullet(gameTime);
			UpdateEnemyBullet(gameTime);
			UpdateItems(gameTime);
			UpdateConstructions(gameTime);
			colony.Update(gameTime);
			UpdateCamera(gameTime);
			UpdateBackground(gameTime);
			UpdateFXs(gameTime);
			maxEnemies = topEnemies;
			UpdateLevel(gameTime);
			updateBlast(gameTime);
			maxEnemies = topEnemies;
			UpdateEnemies(gameTime);
		}
		if (messageInfoCounter > 0f)
		{
			messageInfoCounter -= 5f;
		}
		UpdateMessageMode();
		if (currentGamePadState.Buttons != oldGamePadState.Buttons && (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentGamePadState.Buttons.Back == ButtonState.Pressed))
		{
			pauseTarget = 100f;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		if (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		if (editor)
		{
			endGame = 0;
			return;
		}
		if (currentLevel == 14)
		{
			UpdateTutorial();
		}
		if (endGame > -100)
		{
			UpdatePlayer(gameTime);
		}
		if (colony.exploding > 0 || numPlayers <= 0)
		{
			endGame++;
			if (endGame > 0)
			{
				if (colony.health <= 0f)
				{
					if (endGame == 1)
					{
						messages.Add(new Message("Colony Destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
					}
					if (endGame % 10 == 0)
					{
						AddExploss(colony.position + new Vector2(random.Next(-colony.Width / 4, colony.Width / 4), random.Next(-colony.Height / 4, colony.Height / 4)));
					}
					if (endGame % 8 == 0)
					{
						particleSystem.createExplosion(colony.position + new Vector2(random.Next(-colony.Width / 4, colony.Width / 4), random.Next(-colony.Height / 4, colony.Height / 4)), camera.getScreenPosition(colony.position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
					}
				}
				for (int i = 0; i < player.Length; i++)
				{
					if (player[i].Health <= 0f)
					{
						if (endGame == 1)
						{
							messages.Add(new Message("Player " + (i + 1) + " destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
						}
						if (endGame % 20 == 0)
						{
							AddExploss(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)));
						}
						if (endGame % 15 == 0)
						{
							particleSystem.createExplosion(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)), camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
						}
					}
				}
			}
			if (endGame > 240)
			{
				frame = 0u;
				gameStateNext = GameState.score;
				gameState = GameState.score;
			}
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Active && (GamePad.GetState(player[i].index).Buttons.Start == ButtonState.Pressed || GamePad.GetState(player[i].index).Buttons.Back == ButtonState.Pressed))
			{
				controllingPlayer = player[i].index;
			}
		}
	}

	private void UpdateTutorial()
	{
		if (frame % 30 == 0)
		{
			for (int i = 0; i < player.Length; i++)
			{
				if (player[i].Health < player[i].maximunHealth)
				{
					player[i].Health++;
				}
			}
		}
		if (frame % 600 == 0)
		{
			tutorialItems++;
			switch (tutorialItems)
			{
			case 1:
				AddItem(Pickup.item.relic, calculateSpawningPoint(), 50);
				break;
			case 2:
				AddItem(Pickup.item.health, calculateSpawningPoint(), 50);
				break;
			case 3:
				if (!enemyMessage)
				{
					string[] array = new string[40];
					array[0] = "An enemy has appeared!\n";
					array[1] = "\n";
					array[2] = "\n";
					array[3] = "You can shoot using\n";
					array[4] = "\n";
					array[5] = "the right thumbstick in the gamepad\n";
					messageInfo.Add(array);
					enemyMessage = true;
				}
				if (enemies.Count < 3)
				{
					AddEnemy(RandomPosition(), 0f, 1);
				}
				break;
			default:
			{
				for (int j = 0; j < 10; j++)
				{
					AddItem(calculateSpawningPoint());
				}
				tutorialItems = 0;
				break;
			}
			}
		}
		if (frame == 10 && moveMessage)
		{
			string[] array = new string[40];
			array[0] = "Welcome to Defenders of the Last Colony!\n";
			array[1] = "\n";
			array[2] = "\n";
			array[3] = "move your ship using\n";
			array[4] = "\n";
			array[5] = "the left thumbstick on the gamepad\n";
			messageInfo.Add(array);
			moveMessage = false;
		}
		if (chargeMessage && healthMessage && relicMessage && orbMessage && enemyMessage)
		{
			tutorialCounter++;
			if (tutorialCounter > 300 && !basicsMessage)
			{
				string[] array = new string[40]
				{
					"Congratulations!\n", "\n", "\n", "You now know the basics of\n", "\n", "Defenders of the Last Colony\n", "\n", "and you are ready to play the main campaign.\n", null, null,
					null, null, null, null, null, null, null, null, null, null,
					null, null, null, null, null, null, null, null, null, null,
					null, null, null, null, null, null, null, null, null, null
				};
				messageInfo.Add(array);
				basicsMessage = true;
			}
			if (tutorialCounter > 900 && enemies.Count > 0 && !eliminateMessage)
			{
				string[] array = new string[40];
				array[0] = "Eliminate the enemies\n";
				array[1] = "\n";
				array[2] = "\n";
				array[3] = "to end the tutorial\n";
				messageInfo.Add(array);
				eliminateMessage = true;
			}
			if (tutorialCounter > 1800 && eliminateMessage && enemies.Count == 0)
			{
				string[] array = new string[40];
				array[0] = "Great!\n";
				array[1] = "\n";
				array[2] = "\n";
				array[3] = "now let's play the main campaign!\n";
				messageInfo.Add(array);
				eliminateMessage = true;
				gameStateNext = GameState.playMenu;
			}
		}
		if (!chargeMessage && colony.energy > 0.01f)
		{
			string[] array = new string[40]
			{
				"Congratulations!\n", "\n", "\n", "You brought your first Blue Orb to the Colony's Core,\n", "\n", "when the Colony's Core will reach 100%\n", "\n", "we'll be ready to jump to the next sector,\n", null, null,
				null, null, null, null, null, null, null, null, null, null,
				null, null, null, null, null, null, null, null, null, null,
				null, null, null, null, null, null, null, null, null, null
			};
			messageInfo.Add(array);
			chargeMessage = true;
		}
	}

	private void UpdateChallenge(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			if (challengeNumber % 2 == 0)
			{
				PlaySong("Colonial_trance");
			}
			else
			{
				PlaySong("Marching");
			}
		}
		if (editor)
		{
			updateFrame = true;
			base.IsMouseVisible = true;
			assetManagerChallenge[challengeNumber] = UpdateEditor(assetManagerChallenge[challengeNumber]);
			UpdateCameraEditor(gameTime);
			UpdateBackground(gameTime);
			if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				gameStateNext = GameState.galaxyMap;
			}
			UpdateItemsChallenge(gameTime);
			UpdateFXs(gameTime);
		}
		else
		{
			base.IsMouseVisible = false;
			UpdateBullet(gameTime);
			UpdateEnemyBullet(gameTime);
			UpdateItemsChallenge(gameTime);
			UpdateConstructions(gameTime);
			UpdateCameraChallenge(gameTime);
			UpdateBackground(gameTime);
			UpdateFXs(gameTime);
			maxEnemies = topEnemies * 2;
			UpdateLevelChallenge(gameTime);
			updateBlast(gameTime);
			maxEnemies = topEnemies * 2;
			UpdateEnemies(gameTime);
		}
		UpdateMessageMode();
		if (currentGamePadState.Buttons != oldGamePadState.Buttons && (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentGamePadState.Buttons.Back == ButtonState.Pressed))
		{
			pauseTarget = 100f;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		if (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
			if (editor)
			{
				gameStateNext = GameState.selectChallenge;
				gameState = GameState.selectChallenge;
			}
		}
		if (editor)
		{
			endGame = 0;
			return;
		}
		if (endGame > -100)
		{
			UpdatePlayerChallenge(gameTime);
		}
		if (playerChallenge.Health <= 0f)
		{
			endGame++;
			if (endGame > 0 && playerChallenge.Health <= 0f)
			{
				if (endGame == 170)
				{
					string[] array = new string[40];
					array[0] = "Challenge failed!";
					array[1] = "\n";
					array[2] = "\n";
					messageInfo.Add(array);
				}
				if (endGame % 20 == 0)
				{
					AddExploss(playerChallenge.position + new Vector2(random.Next(-playerChallenge.Width / 4, playerChallenge.Width / 4), random.Next(-playerChallenge.Height / 4, playerChallenge.Height / 4)));
				}
				if (endGame % 10 == 0)
				{
					particleSystem.createExplosion(playerChallenge.position + new Vector2(random.Next(-playerChallenge.Width / 4, playerChallenge.Width / 4), random.Next(-playerChallenge.Height / 4, playerChallenge.Height / 4)), camera.getScreenPosition(playerChallenge.position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
			if (endGame > 180)
			{
				frame = 0u;
				challengeClear = false;
				gameStateNext = GameState.challengeFinished;
			}
		}
		if (playerChallenge.Active && (GamePad.GetState(playerChallenge.index).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerChallenge.index).Buttons.Back == ButtonState.Pressed))
		{
			controllingPlayer = playerChallenge.index;
		}
	}

	private void UpdateFinalBoss(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			PlaySong("EpicFinale");
		}
		if (updateFrame)
		{
			bossFrame++;
		}
		if (editor)
		{
			base.IsMouseVisible = true;
			assetManager[currentLevel] = UpdateEditor(assetManager[currentLevel]);
			UpdateCameraEditor(gameTime);
			UpdateBackground(gameTime);
			if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				gameStateNext = GameState.galaxyMap;
			}
			UpdateItems(gameTime);
			UpdateFXs(gameTime);
		}
		else
		{
			updateFrame = true;
			base.IsMouseVisible = false;
			UpdateBullet(gameTime);
			UpdateEnemyBullet(gameTime);
			UpdateItems(gameTime);
			UpdateConstructions(gameTime);
			colony.Update(gameTime);
			UpdateCamera(gameTime);
			UpdateBackground(gameTime);
			UpdateFXs(gameTime);
			maxEnemies = (int)frame / 200;
			UpdateLevel(gameTime);
			updateBlast(gameTime);
			maxEnemies = (int)frame / 200;
			UpdateEnemies(gameTime);
		}
		UpdateMessageMode();
		if (currentGamePadState.Buttons != oldGamePadState.Buttons && (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentGamePadState.Buttons.Back == ButtonState.Pressed))
		{
			pauseTarget = 100f;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		if (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		if (endGame > -100)
		{
			UpdatePlayer(gameTime);
		}
		if (endGame < 0 && completeCoopLevel && awards.Unlock("Cooperative"))
		{
			writeAwards();
		}
		if (awards.isUnlock("Fighter") && awards.isUnlock("Engineer") && awards.Unlock("100% Complete"))
		{
			writeAwards();
		}
		if (colony.exploding > 0 || numPlayers <= 0)
		{
			endGame++;
			if (endGame > 0)
			{
				if (colony.health <= 0f)
				{
					if (endGame == 1)
					{
						messages.Add(new Message("Colony Destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
					}
					if (endGame % 10 == 0)
					{
						AddExploss(colony.position + new Vector2(random.Next(-colony.Width / 4, colony.Width / 4), random.Next(-colony.Height / 4, colony.Height / 4)));
					}
					if (endGame % 8 == 0)
					{
						particleSystem.createExplosion(colony.position + new Vector2(random.Next(-colony.Width / 4, colony.Width / 4), random.Next(-colony.Height / 4, colony.Height / 4)), camera.getScreenPosition(colony.position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
					}
				}
				for (int i = 0; i < player.Length; i++)
				{
					if (player[i].Health <= 0f && player[i].wasActive)
					{
						if (endGame == 1)
						{
							messages.Add(new Message("Player " + (i + 1) + " destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
						}
						if (endGame % 20 == 0)
						{
							AddExploss(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)));
						}
						if (endGame % 15 == 0)
						{
							particleSystem.createExplosion(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)), camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
						}
					}
				}
			}
			if (endGame > 240)
			{
				frame = 0u;
				gameStateNext = GameState.score;
				gameState = GameState.score;
				if (currentLevel == 13)
				{
					gameStateNext = GameState.playMenu;
				}
			}
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Active && (GamePad.GetState(player[i].index).Buttons.Start == ButtonState.Pressed || GamePad.GetState(player[i].index).Buttons.Back == ButtonState.Pressed))
			{
				controllingPlayer = player[i].index;
			}
		}
		if (!enemies[0].Active)
		{
			if (awards.Unlock("Boss Killer"))
			{
				writeAwards();
			}
			endGame--;
			try
			{
				if (enemies.Count > 1)
				{
					enemies.RemoveRange(1, enemies.Count - 1);
				}
			}
			catch
			{
			}
		}
		if (endGame < -240)
		{
			gameStateNext = GameState.ending;
		}
	}

	private void UpdateMessageMode()
	{
		if (messageInfo.Count > 0)
		{
			gameState = GameState.message;
			gameStateNext = GameState.message;
		}
	}

	private void UpdateEditorIngame(GameTime gameTime)
	{
		base.IsMouseVisible = true;
		assetManager[currentLevel] = UpdateEditor(assetManager[currentLevel]);
		UpdateCameraEditor(gameTime);
		UpdateBackground(gameTime);
		if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			gameStateNext = GameState.galaxyMap;
		}
	}

	private void UpdateVSmode(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			PlaySong("Jarre80s");
		}
		if (frame > 600 + 100 * round * round)
		{
			frame = 0u;
			round++;
		}
		updateFrame = true;
		base.IsMouseVisible = false;
		UpdateBullet(gameTime);
		UpdateEnemyBullet(gameTime);
		UpdateItems(gameTime);
		UpdateConstructions(gameTime);
		UpdateCamera(gameTime);
		UpdateBackground(gameTime);
		particleSystem.UpdateParticles(gameTime);
		UpdateFXs(gameTime);
		UpdateLevelSurvival(gameTime);
		updateBlast(gameTime);
		UpdateEnemies(gameTime);
		if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			frame = 0u;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		UpdatePlayer(gameTime);
		if (endGame == 0)
		{
			colony.position = cameraPosition * 2f;
		}
		if (demo && endGame == 0 && seconds == 0 && frame % 50 == 0 && messages.Count < 1)
		{
			messages.Add(new Message("You can only play for 3 minutes in Demo", 300, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
		}
		if (numPlayers > 0 && (minutes < 3 || !demo))
		{
			return;
		}
		endGame++;
		if (endGame == 1 && messages.Count < 2)
		{
			if (demo)
			{
				if (minutes >= 3)
				{
					messages.Add(new Message("Time is up!", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
				}
				else
				{
					messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
				}
			}
			if (!demo && numPlayers <= 0)
			{
				messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
			}
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Destroyed > 0f)
			{
				if (endGame == 1)
				{
					messages.Add(new Message("Player " + (i + 1) + " destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
				}
				if (endGame % 15 == 0)
				{
					AddExploss(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)));
				}
				if (endGame % 20 == 0)
				{
					particleSystem.createExplosion(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)), camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
		}
		if (endGame > 200)
		{
			frame = 0u;
			gameStateNext = GameState.score;
			gameState = GameState.score;
		}
	}

	private void UpdateSurvivalMode(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			PlaySong("Jarre80s");
		}
		if (frame > 600 + 100 * round * round)
		{
			frame = 0u;
			round++;
		}
		updateFrame = true;
		base.IsMouseVisible = false;
		UpdateBullet(gameTime);
		UpdateEnemyBullet(gameTime);
		UpdateItems(gameTime);
		UpdateConstructions(gameTime);
		UpdateCamera(gameTime);
		UpdateBackground(gameTime);
		particleSystem.UpdateParticles(gameTime);
		UpdateFXs(gameTime);
		UpdateLevelSurvival(gameTime);
		updateBlast(gameTime);
		UpdateEnemies(gameTime);
		if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			frame = 0u;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		UpdatePlayer(gameTime);
		if (endGame == 0)
		{
			colony.position = cameraPosition * 2f;
		}
		if (demo && endGame == 0 && seconds == 0 && frame % 50 == 0 && messages.Count < 1)
		{
			messages.Add(new Message("You can only play for 3 minutes in Demo", 300, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
		}
		if (numPlayers > 0 && (minutes < 3 || !demo))
		{
			return;
		}
		endGame++;
		if (endGame == 1 && messages.Count < 2)
		{
			if (demo)
			{
				if (minutes >= 3)
				{
					messages.Add(new Message("Time is up!", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
				}
				else
				{
					messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
				}
			}
			if (!demo && numPlayers <= 0)
			{
				messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
			}
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Destroyed > 0f)
			{
				if (endGame == 1)
				{
					messages.Add(new Message("Player " + (i + 1) + " destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
				}
				if (endGame % 15 == 0)
				{
					AddExploss(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)));
				}
				if (endGame % 20 == 0)
				{
					particleSystem.createExplosion(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)), camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
		}
		if (endGame > 200)
		{
			frame = 0u;
			gameStateNext = GameState.score;
			gameState = GameState.score;
		}
	}

	private void UpdateMeteroidsMode(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			PlaySong("Jarre80s");
		}
		if (frame > 600 + 100 * round)
		{
			frame = 0u;
			round++;
		}
		updateFrame = true;
		base.IsMouseVisible = false;
		UpdateBullet(gameTime);
		UpdateEnemyBullet(gameTime);
		UpdateItems(gameTime);
		UpdateConstructions(gameTime);
		UpdateCameraMeteroids(gameTime);
		UpdateBackground(gameTime);
		particleSystem.UpdateParticles(gameTime);
		UpdateFXs(gameTime);
		UpdateLevelMeteroids(gameTime);
		updateBlast(gameTime);
		UpdateEnemies(gameTime);
		if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			frame = 0u;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		UpdatePlayerMeteroids(gameTime);
		if (endGame == 0)
		{
			colony.position = cameraPosition * 2f;
		}
		if (demo && endGame == 0 && seconds == 0 && frame % 50 == 0 && messages.Count < 1)
		{
			messages.Add(new Message("You can only play for 3 minutes in Demo", 300, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
		}
		if (numPlayers > 0 && (minutes < 3 || !demo))
		{
			return;
		}
		endGame++;
		if (endGame == 1 && messages.Count < 2)
		{
			if (demo)
			{
				if (minutes >= 3)
				{
					messages.Add(new Message("Time is up!", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
				}
				else
				{
					messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
				}
			}
			if (!demo && numPlayers <= 0)
			{
				messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
			}
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Destroyed > 0f)
			{
				if (endGame == 1)
				{
					messages.Add(new Message("Player " + (i + 1) + " destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
				}
				if (endGame % 15 == 0)
				{
					AddExploss(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)));
				}
				if (endGame % 20 == 0)
				{
					particleSystem.createExplosion(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)), camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
		}
		if (endGame > 200)
		{
			frame = 0u;
			bonusClear = false;
			gameStateNext = GameState.BonusClear;
			gameStatePlay = GameState.galaxyMap;
		}
	}

	private void UpdateChubbyRainMode(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			PlaySong("Jarre80s");
		}
		updateFrame = true;
		base.IsMouseVisible = false;
		UpdateBullet(gameTime);
		UpdateEnemies(gameTime);
		UpdateEnemyBullet(gameTime);
		UpdateItems(gameTime);
		UpdateCameraChubbyRain(gameTime);
		UpdateBackgroundChubbyRain(gameTime);
		UpdateFXs(gameTime);
		UpdateLevelChubbyRain(gameTime);
		updateBlast(gameTime);
		if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			frame = 0u;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		UpdatePlayerChubbyRain(gameTime);
		if (demo && endGame == 0 && seconds == 0 && frame % 50 == 0 && messages.Count < 1)
		{
			messages.Add(new Message("You can only play for 3 minutes in Demo", 300, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
		}
		if (numPlayers >= 1 && (minutes < 3 || !demo))
		{
			return;
		}
		endGame++;
		if (endGame == 1 && messages.Count < 2)
		{
			if (numPlayers < 1 && !demo)
			{
				messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
			}
			if (minutes >= 3 && demo)
			{
				messages.Add(new Message("Time is up!", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
			}
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Destroyed > 0f && player[i].wasActive)
			{
				if (endGame == 1)
				{
					messages.Add(new Message("Player " + (i + 1) + " destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
				}
				if (endGame % 20 == 0)
				{
					AddExploss(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)));
				}
				if (endGame % 10 == 0)
				{
					particleSystem.createExplosion(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)), camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
		}
		if (endGame <= 200)
		{
			return;
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].wasActive)
			{
				player[i].Health = player[i].maximunHealth;
			}
		}
		frame = 0u;
		bonusClear = false;
		gameStateNext = GameState.BonusClear;
		gameStatePlay = GameState.galaxyMap;
	}

	private void UpdateSidescrollerMode(GameTime gameTime)
	{
		if (GOmusicVolume > 0f)
		{
			PlaySong(sidescrollerSong);
		}
		frame++;
		if (frame > 3000 + 100 * round * round)
		{
			frame = 0u;
			round++;
			if (round % 2 == 0)
			{
				switch (random.Next(4))
				{
				case 0:
					sidescrollerSong = "In_a_heart_beat";
					PlaySong(sidescrollerSong);
					break;
				case 1:
					sidescrollerSong = "HeIsAlive";
					PlaySong(sidescrollerSong);
					break;
				case 2:
					sidescrollerSong = "TimeToRun";
					PlaySong(sidescrollerSong);
					break;
				default:
					sidescrollerSong = "In_a_heart_beat";
					PlaySong(sidescrollerSong);
					break;
				}
			}
		}
		updateFrame = false;
		base.IsMouseVisible = false;
		UpdateBullet(gameTime);
		UpdateEnemies(gameTime);
		UpdateEnemyBullet(gameTime);
		UpdateItems(gameTime);
		UpdateCameraSidescroller(gameTime);
		UpdateBackgroundSidescroller(gameTime);
		UpdateFXs(gameTime);
		UpdateLevelSidescroller(gameTime);
		updateBlast(gameTime);
		UpdatePlayerSideScroller(gameTime);
		if (currentGamePadState.Buttons.Start == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
		{
			pauseTarget = 100f;
			gameStateNext = GameState.pause;
			gameState = GameState.pause;
		}
		if (demo && endGame == 0 && seconds == 0 && frame % 50 == 0 && messages.Count < 1)
		{
			messages.Add(new Message("You can only play for 3 minutes in Demo", 300, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
		}
		if (numPlayers >= 1 && (minutes < 3 || !demo))
		{
			return;
		}
		endGame++;
		if (endGame == 1 && messages.Count < 2)
		{
			if (numPlayers < 1 && !demo)
			{
				messages.Add(new Message("Game Over", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
			}
			if (minutes >= 3 && demo)
			{
				messages.Add(new Message("Time is up!", 200, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.5f), messageSound, GOsoundFXvolume));
			}
		}
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Destroyed > 0f)
			{
				if (endGame == 1)
				{
					messages.Add(new Message("Player " + (i + 1) + " destroyed!", 100, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 400f), messageSound, GOsoundFXvolume));
				}
				if (endGame % 20 == 0)
				{
					AddExploss(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)));
				}
				if (endGame % 10 == 0)
				{
					particleSystem.createExplosion(player[i].position + new Vector2(random.Next(-player[i].Width / 4, player[i].Width / 4), random.Next(-player[i].Height / 4, player[i].Height / 4)), camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
		}
		if (endGame > 200)
		{
			frame = 0u;
			bonusClear = false;
			gameStateNext = GameState.BonusClear;
			gameStatePlay = GameState.galaxyMap;
		}
	}

	private void UpdatePlayer(GameTime gameTime)
	{
		if (editor)
		{
			return;
		}
		UpdateMouse();
		numPlayers = 0;
		for (int i = 0; i < player.Length; i++)
		{
			string text = player[i].Update(1280, 720, GOvibration, (float)i == keyboardPlayer - 1f, player[i].index);
			if (player[i].levelUpdated)
			{
				createBlast(i);
				player[i].levelUpdated = false;
				bloom(player[i].position);
			}
			if (player[i].Health <= 0f)
			{
				if (frame % 2 == 0)
				{
					particleSystem.createExplosion(player[i].position, camera.getScreenPosition(player[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
				else
				{
					AddExploss(player[i].position);
				}
			}
			if (player[i].Destroyed == 1f)
			{
				createBlast(i + 10);
				bloom(player[i].position);
			}
			if (!player[i].Active)
			{
				continue;
			}
			numPlayers++;
			player[i].minutes = minutes;
			player[i].seconds = seconds;
			if (player[i].missiles > 0)
			{
				AddMisiles(i);
			}
			for (int j = 0; j < lens[currentLevel].Count; j++)
			{
				if (Vector2.Distance(player[i].position, lens[currentLevel][j].position) < player[i].size() * 0.75f)
				{
					lens[currentLevel][j].visible = false;
				}
			}
			if (player[i].nRelics > 3 && currentLevel != 14 && awards.Unlock("Collector"))
			{
				writeAwards();
			}
			if (player[i].nRelics > 9 && currentLevel != 14 && awards.Unlock("Explorer"))
			{
				writeAwards();
			}
			if (text != "" && messageInfoCounter <= 0f)
			{
				if (gameState == GameState.ChubbyRain)
				{
					if (currentGamePadState.Buttons != oldGamePadState.Buttons)
					{
						AddBullet(isPlayer: true, i);
					}
				}
				else
				{
					switch (text)
					{
					case "SHIELD":
						AddConstruction(constructionType.barrier, i);
						break;
					case "TURRET":
						AddConstruction(constructionType.turret, i);
						break;
					case "HIVE":
						AddConstruction(constructionType.hive, i);
						break;
					case "SANCTUARY":
						AddConstruction(constructionType.sanctuary, i);
						break;
					}
				}
			}
			Vector2 vector = player[i].UpdateShooting((float)i == keyboardPlayer - 1f, camera.get_mouse_vpos(base.GraphicsDevice), player[i].index);
			if (gameState == GameState.ChubbyRain)
			{
				vector = Vector2.Zero;
				player[i].position.Y = 600f;
				player[i].position.X = MathHelper.Clamp(player[i].position.X, 0f, 640f);
				player[i].angle = -(float)Math.PI / 2f;
				player[i].credits = player[i].maximunCredits;
			}
			try
			{
				if ((Math.Abs(vector.X) > 0.1f || Math.Abs(vector.Y) > 0.1f) && frame % (int)player[i].shootRate == 0)
				{
					AddBullet(isPlayer: true, i);
				}
			}
			catch
			{
			}
			if (player[i].abilityTimer == 1 && player[i].characters[player[i].number].abilityType == "Laser Blades" && player[i].level > 1)
			{
				for (int j = 0; j < 628; j += 20)
				{
					bullets.Add(new Bullet(i, "NORMAL", player[i].position + new Vector2((float)(Math.Cos((float)j / 100f - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin((float)j / 100f - (float)Math.PI / 2f) * 20.0)), (float)j / 100f, txBullet01, player[i].shootColor, (float)random.Next(-100, 100) / 100f + 7.5f, new Vector2(4f, 1f), 40, player[i].shootDamage * 2f));
				}
				bloom(player[i].position);
			}
			if (player[i].UpdateSpecialAbility())
			{
				if (gameState == GameState.ChubbyRain)
				{
					AddBullet(isPlayer: true, i);
				}
				switch (player[i].characters[player[i].number].abilityType)
				{
				case "Hell Storm":
					player[i].missiles = (ushort)(75 + (player[i].level * player[i].level + 1) * 10);
					break;
				case "EMP":
					createBlast(i, player[i].position, 1);
					break;
				case "Laser Blades":
					AddRadial(i);
					break;
				default:
					createBlast(i);
					break;
				}
			}
			ushort num = 1;
			num = 3;
			if (Math.Abs(player[i].accelerationX2 + player[i].accelerationY2 + player[i].accelerationX + player[i].accelerationY) > 0.01f && frame % num == 0)
			{
				switch (player[i].characters[player[i].number].shipClass)
				{
				case "Fighter":
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle + (float)Math.PI * 5f / 9f) * 18.0), (int)(Math.Sin(player[i].angle + (float)Math.PI * 5f / 9f) * 18.0)), txCoins, 1f, 5f, 0.1f, 0f, 0.01f, player[i].shootColor * 3f);
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle - (float)Math.PI * 5f / 9f) * 18.0), (int)(Math.Sin(player[i].angle - (float)Math.PI * 5f / 9f) * 18.0)), txCoins, 1f, 5f, 0.1f, 0f, 0.01f, player[i].shootColor * 3f);
					break;
				case "Defender":
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle + (float)Math.PI) * 18.0), (int)(Math.Sin(player[i].angle + (float)Math.PI) * 18.0)), txCoins, 2f, 6f, 0.035f, 0f, 0.01f, player[i].shootColor * 3f);
					break;
				default:
					particleSystem.AddTrails(player[i].position, txCoins, 2f, 5f, 0.3f, player[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.5f));
					break;
				}
			}
		}
	}

	private void UpdatePlayerMeteroids(GameTime gameTime)
	{
		if (editor)
		{
			return;
		}
		UpdateMouse();
		numPlayers = 0;
		for (int i = 0; i < player.Length; i++)
		{
			string text = player[i].UpdateMeteroids(1280, 720, GOvibration, (float)i == keyboardPlayer - 1f, player[i].index);
			if (player[i].levelUpdated)
			{
				createBlast(i);
				player[i].levelUpdated = false;
				bloom(player[i].position);
			}
			if (player[i].Destroyed == 1f)
			{
				createBlast(i + 10);
				bloom(player[i].position);
			}
			if (!player[i].Active)
			{
				continue;
			}
			numPlayers++;
			if (player[i].missiles > 0)
			{
				AddMisiles(i);
			}
			for (int j = 0; j < lens[currentLevel].Count; j++)
			{
				if (Vector2.Distance(player[i].position, lens[currentLevel][j].position) < player[i].size() * 0.75f)
				{
					lens[currentLevel][j].visible = false;
				}
			}
			if (text != "" && messageInfoCounter <= 0f)
			{
				if (gameState == GameState.ChubbyRain)
				{
					if (currentGamePadState.Buttons != oldGamePadState.Buttons)
					{
						AddBullet(isPlayer: true, i);
					}
				}
				else
				{
					switch (text)
					{
					case "SHIELD":
						AddConstruction(constructionType.barrier, i);
						break;
					case "TURRET":
						AddConstruction(constructionType.turret, i);
						break;
					case "HIVE":
						AddConstruction(constructionType.hive, i);
						break;
					case "SANCTUARY":
						AddConstruction(constructionType.sanctuary, i);
						break;
					}
				}
			}
			Vector2 vector = player[i].UpdateShootingMeteroids((float)i == keyboardPlayer - 1f, camera.get_mouse_vpos(base.GraphicsDevice), player[i].index);
			if (gameState == GameState.ChubbyRain)
			{
				vector = Vector2.Zero;
				player[i].position.Y = 600f;
				player[i].position.X = MathHelper.Clamp(player[i].position.X, 0f, 640f);
				player[i].angle = -(float)Math.PI / 2f;
				player[i].credits = player[i].maximunCredits;
			}
			try
			{
				if ((Math.Abs(vector.X) > 0.1f || Math.Abs(vector.Y) > 0.1f) && frame % (int)player[i].shootRate == 0)
				{
					AddBullet(isPlayer: true, i);
				}
			}
			catch
			{
			}
			if (player[i].abilityTimer == 1 && player[i].characters[player[i].number].abilityType == "Laser Blades" && player[i].level > 1)
			{
				for (int j = 0; j < 628; j += 20)
				{
					bullets.Add(new Bullet(i, "NORMAL", player[i].position + new Vector2((float)(Math.Cos((float)j / 100f - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin((float)j / 100f - (float)Math.PI / 2f) * 20.0)), (float)j / 100f, txBullet01, player[i].shootColor, (float)random.Next(-100, 100) / 100f + 7.5f, new Vector2(4f, 1f), 40, player[i].shootDamage * 2f));
				}
				bloom(player[i].position);
			}
			if (player[i].UpdateSpecialAbility())
			{
				if (gameState == GameState.ChubbyRain)
				{
					AddBullet(isPlayer: true, i);
				}
				switch (player[i].characters[player[i].number].abilityType)
				{
				case "Hell Storm":
					player[i].missiles = (ushort)(75 + (player[i].level * player[i].level + 1) * 10);
					break;
				case "EMP":
					createBlast(i, player[i].position, 1);
					break;
				case "Laser Blades":
					AddRadial(i);
					break;
				default:
					createBlast(i);
					break;
				}
			}
			ushort num = 1;
			num = 3;
			if (Math.Abs(player[i].accelerationX2 + player[i].accelerationY2 + player[i].accelerationX + player[i].accelerationY) > 0.01f && frame % num == 0)
			{
				switch (player[i].characters[player[i].number].shipClass)
				{
				case "Fighter":
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle + (float)Math.PI * 5f / 9f) * 18.0), (int)(Math.Sin(player[i].angle + (float)Math.PI * 5f / 9f) * 18.0)), txCoins, 1f, 5f, 0.1f, 0f, 0.01f, player[i].shootColor * 3f);
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle - (float)Math.PI * 5f / 9f) * 18.0), (int)(Math.Sin(player[i].angle - (float)Math.PI * 5f / 9f) * 18.0)), txCoins, 1f, 5f, 0.1f, 0f, 0.01f, player[i].shootColor * 3f);
					break;
				case "Defender":
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle + (float)Math.PI) * 18.0), (int)(Math.Sin(player[i].angle + (float)Math.PI) * 18.0)), txCoins, 2f, 6f, 0.035f, 0f, 0.01f, player[i].shootColor * 3f);
					break;
				default:
					particleSystem.AddTrails(player[i].position, txCoins, 2f, 5f, 0.3f, player[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.5f));
					break;
				}
			}
		}
	}

	private void UpdatePlayerChallenge(GameTime gameTime)
	{
		colony.position = new Vector2(playerChallenge.position.X, playerChallenge.position.Y);
		playerChallenge.speed = 10f;
		string text = playerChallenge.UpdateChallenge(1280, 720, GOvibration, useKeyboardControls: true);
		if (playerChallenge.Destroyed == 1f)
		{
			createBlast(-3, playerChallenge.position);
			bloom(playerChallenge.position);
		}
		if (playerChallenge.Active)
		{
			numPlayers = 1;
			if (playerChallenge.boost > 1.1f && Math.Abs(playerChallenge.accelerationX + playerChallenge.accelerationY + playerChallenge.accelerationX2 + playerChallenge.accelerationY2) > 0.01f)
			{
				bloom(playerChallenge.position);
			}
			for (int i = 0; i < lens[currentLevel].Count; i++)
			{
				if (Vector2.Distance(playerChallenge.position, lens[currentLevel][i].position) < playerChallenge.size() * 0.75f)
				{
					lens[currentLevel][i].visible = false;
				}
			}
			for (int j = 0; j < enemies.Count; j++)
			{
				if (enemies[j].spawning > 0.9f && Vector2.Distance(playerChallenge.position, enemies[j].position) <= playerChallenge.size() * 0.25f + enemies[j].size() * 0.25f)
				{
					playerChallenge.Health = 0f;
					bloom(playerChallenge.position);
					AddExploss(playerChallenge.position);
					if (blast.Count == 0 && playerChallenge.beingHit < 100 && playerChallenge.Health > 0f)
					{
						createBlast(-3, playerChallenge.position);
					}
				}
			}
			ushort num = 5;
			num = 5;
			if (playerChallenge.boost > 1.1f)
			{
				num = 1;
			}
			if (Math.Abs(playerChallenge.accelerationX2 + playerChallenge.accelerationY2 + playerChallenge.accelerationX + playerChallenge.accelerationY) > 0.01f && frame % num == 0)
			{
				particleSystem.AddTrails(playerChallenge.position, playerChallenge.texture, playerChallenge.scale - 0.2f, playerChallenge.scale + playerChallenge.boost / 10f, 0.03f, playerChallenge.angle, 0f, new Color(0.7f, 0.9f + playerChallenge.boost / 30f, 1f + playerChallenge.boost / 20f, 0.5f + playerChallenge.boost / 10f));
			}
		}
		else
		{
			numPlayers = 0;
		}
	}

	private void UpdatePlayerChubbyRain(GameTime gameTime)
	{
		numPlayers = 0;
		for (int i = 0; i < player.Length; i++)
		{
			string text = player[i].Update(1280, 720, GOvibration, (float)i == keyboardPlayer - 1f, player[i].index);
			player[i].position.Y = 540f;
			player[i].position.X = MathHelper.Clamp(player[i].position.X, 0f, 640f);
			player[i].angle = -(float)Math.PI / 2f;
			if (!player[i].Active)
			{
				continue;
			}
			numPlayers++;
			player[i].angle = -(float)Math.PI / 2f;
			player[i].credits = player[i].maximunCredits;
			player[i].SA = 1;
			if ((text != "" || player[i].UpdateSpecialAbility()) && currentGamePadState.Buttons != oldGamePadState.Buttons)
			{
				AddBullet(isPlayer: true, i);
			}
			Vector2 vector = player[i].UpdateShootingMeteroids((float)i == keyboardPlayer - 1f, camera.get_mouse_vpos(base.GraphicsDevice), player[i].index);
			if ((Math.Abs(vector.X) > 0.1f || Math.Abs(vector.Y) > 0.1f) && (float)frame % player[i].shootRate == 0f)
			{
				AddBullet(isPlayer: true, i);
			}
			player[i].angle = -(float)Math.PI / 2f;
			if (Math.Abs(player[i].accelerationX2 + player[i].accelerationY2 + player[i].accelerationX + player[i].accelerationY) > 0.01f && frame % 3 == 0)
			{
				switch (player[i].characters[player[i].number].shipClass)
				{
				case "Fighter":
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle + (float)Math.PI * 5f / 9f) * 20.0), (int)(Math.Sin(player[i].angle + (float)Math.PI * 5f / 9f) * 20.0)), txCoins, 1.8f, 3.6f, 0.25f, 0f, 0.01f, player[i].shootColor);
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle - (float)Math.PI * 5f / 9f) * 20.0), (int)(Math.Sin(player[i].angle - (float)Math.PI * 5f / 9f) * 20.0)), txCoins, 1.8f, 3.6f, 0.25f, 0f, 0.01f, player[i].shootColor);
					break;
				case "Defender":
					particleSystem.AddTrails(player[i].position, txCoins, 2f, 5f, 0.2f, player[i].angle + (float)random.Next(3600) / 100f, 0.01f, player[i].shootColor);
					break;
				default:
					particleSystem.AddTrails(player[i].position, txCoins, 2f, 5f, 0.3f, player[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.5f));
					break;
				}
			}
		}
	}

	private void UpdatePlayerSideScroller(GameTime gameTime)
	{
		numPlayers = 0;
		for (int i = 0; i < player.Length; i++)
		{
			string text = player[i].Update(1280, 720, GOvibration, (float)i == keyboardPlayer - 1f, player[i].index);
			player[i].position.Y = MathHelper.Clamp(player[i].position.Y, 20f, 620f);
			player[i].position.X = MathHelper.Clamp(player[i].position.X, -190f, 830f);
			player[i].angle = 0f;
			if (!player[i].Active)
			{
				continue;
			}
			numPlayers++;
			if ((text != "" || player[i].UpdateSpecialAbility()) && currentGamePadState.Buttons != oldGamePadState.Buttons)
			{
				AddBullet(isPlayer: true, i);
			}
			player[i].angle = 0f;
			player[i].credits = player[i].maximunCredits;
			player[i].SA = 1;
			Vector2 vector = player[i].UpdateShootingSideScroller((float)i == keyboardPlayer - 1f, camera.get_mouse_vpos(base.GraphicsDevice), player[i].index);
			if (Math.Abs(vector.X) > 0.1f || Math.Abs(vector.Y) > 0.1f)
			{
				AddBullet(isPlayer: true, i);
			}
			if (Math.Abs(player[i].accelerationX2 + player[i].accelerationY2 + player[i].accelerationX + player[i].accelerationY) > 0.01f && frame % 3 == 0)
			{
				switch (player[i].characters[player[i].number].shipClass)
				{
				case "Fighter":
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle + (float)Math.PI * 5f / 9f) * 20.0), (int)(Math.Sin(player[i].angle + (float)Math.PI * 5f / 9f) * 20.0)), txCoins, 1.8f, 3.6f, 0.25f, 0f, 0.01f, player[i].shootColor);
					particleSystem.AddTrails(player[i].position + new Vector2((int)(Math.Cos(player[i].angle - (float)Math.PI * 5f / 9f) * 20.0), (int)(Math.Sin(player[i].angle - (float)Math.PI * 5f / 9f) * 20.0)), txCoins, 1.8f, 3.6f, 0.25f, 0f, 0.01f, player[i].shootColor);
					break;
				case "Defender":
					particleSystem.AddTrails(player[i].position, txCoins, 2f, 5f, 0.2f, player[i].angle + (float)random.Next(3600) / 100f, 0.01f, player[i].shootColor);
					break;
				default:
					particleSystem.AddTrails(player[i].position, txCoins, 2f, 5f, 0.3f, player[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.5f));
					break;
				}
			}
		}
	}

	private void UpdateMenu()
	{
		PlaySong("MainTheme");
		updateFrame = true;
		Vector2 vector = new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, (float)base.GraphicsDevice.Viewport.Height * 0.15f);
		tittle.position = new Vector2(MathHelper.Lerp(tittle.position.X, vector.X, 0.25f), vector.Y);
		tittle.color = new Color(menuActive, menuActive, menuActive, menuActive);
		tittle.transparency = menuActive;
		if (Guide.IsTrialMode && currentGamePadState.IsButtonDown(Buttons.Y))
		{
			try
			{
				Guide.ShowMarketplace(controllingPlayer);
			}
			catch
			{
			}
		}
		int num = ControlMenu(ref menuIndex, ref mainMenu);
		if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
		{
			beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
			switch (mainMenu[menuIndex].text)
			{
			case "Options":
				frame = 0u;
				gameStateOld = gameState;
				gameStateNext = GameState.optionsMenu;
				gameState = GameState.optionsMenu;
				menuActive = 0f;
				readOptions();
				break;
			case "Play":
				frame = 0u;
				resetPlayers();
				gameStateOld = gameState;
				gameStateNext = GameState.playMenu;
				gameState = GameState.playMenu;
				gameStatePlay = GameState.mainMenu;
				menuActive = 0f;
				readUnlockables();
				readProgress();
				break;
			case "Campaign":
				frame = 0u;
				gameStateOld = gameState;
				gameStateNext = GameState.selectPlayer;
				gameStatePlay = GameState.galaxyMap;
				menuActive = 0f;
				readProgress();
				break;
			case "Survival Mode":
				createSurvivalLevel(characters: true);
				frame = 0u;
				gameStateNext = GameState.selectPlayer;
				gameStatePlay = GameState.Survival;
				menuActive = 0f;
				readProgress();
				break;
			case "Chubby Rain Mode":
				createSurvivalLevel(characters: true);
				frame = 0u;
				gameStateNext = GameState.selectPlayer;
				gameStatePlay = GameState.ChubbyRain;
				menuActive = 0f;
				readProgress();
				break;
			case "Create online campaign":
				NetCreateServer();
				createLevel(characters: true);
				frame = 0u;
				gameStateNext = GameState.selectPlayer;
				menuActive = 0f;
				break;
			case "Join online campaign":
				NetClientConnect();
				createLevel(characters: true);
				frame = 0u;
				gameStateNext = GameState.selectPlayer;
				menuActive = 0f;
				break;
			case "Back":
				frame = 0u;
				gameStateNext = GameState.startScreen;
				menuIndex--;
				num--;
				menuActive = 0f;
				break;
			case "How to Play":
				frame = 0u;
				gameStateNext = GameState.howtoplay;
				break;
			case "Controls":
				frame = 0u;
				gameStateNext = GameState.controls;
				menuActive = 0f;
				break;
			case "Awards":
				frame = 0u;
				readAwards();
				gameStateNext = GameState.awards;
				menuActive = 0f;
				readAwards();
				break;
			case "Credits":
				frame = 0u;
				gameStateNext = GameState.credits;
				menuActive = 0f;
				break;
			case "Exit":
				confirm.CreateConfirmation("Are you sure you want to exit the game?", GameState.exit, gameState);
				menuActive = 0f;
				break;
			}
		}
		if ((oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed) || (!oldKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyDown(Keys.Escape)))
		{
			frame = 0u;
			gameStateNext = GameState.startScreen;
			gameState = GameState.startScreen;
			resetMenus();
		}
		for (int i = 0; i < mainMenu.Count; i++)
		{
			mainMenu[i].Update((float)num * ((float)Math.PI / (float)mainMenu.Count));
		}
		mainMenu[menuIndex].selected = true;
	}

	private int ControlMenu(ref int mindex, ref List<MenuItem> menu)
	{
		ResetVibration();
		int result = 0;
		direccion = 0;
		UpdateMouseUI();
		ControlThumbsticks();
		Rectangle rectangle = new Rectangle((int)(menu[mindex].positionFinal.X - (float)menu[mindex].Width * 0.01f), (int)(menu[mindex].positionFinal.Y - (float)menu[mindex].Height * 0.4f), (int)((float)menu[mindex].Width * 1.2f), (int)((float)menu[mindex].Height * 1.2f));
		Rectangle value = new Rectangle(currentMouseState.X - 1, currentMouseState.Y - 1, 2, 2);
		Rectangle rectangle2 = new Rectangle((int)(menu[mindex].positionFinal.X - (float)menu[mindex].Width * 2f), (int)(menu[mindex].positionFinal.Y + (float)menu[mindex].Height * 0.5f), (int)((float)menu[mindex].Width * 3f), (int)((float)menu[mindex].Height * 5f));
		Rectangle rectangle3 = new Rectangle((int)(menu[mindex].positionFinal.X - (float)menu[mindex].Width * 2f), (int)(menu[mindex].positionFinal.Y - (float)menu[mindex].Height * 5.5f), (int)((float)menu[mindex].Width * 3f), (int)((float)menu[mindex].Height * 5f));
		if (rectangle.Intersects(value))
		{
			mouseSize = 1f;
		}
		if (rectangle3.Intersects(value))
		{
			mouseAngle = -(float)Math.PI / 2f;
		}
		if (rectangle2.Intersects(value))
		{
			mouseAngle = (float)Math.PI / 2f;
		}
		mouseClicked = false;
		mouseRightClicked = false;
		if (currentMouseState.LeftButton != oldMouseState.LeftButton && currentMouseState.LeftButton == ButtonState.Pressed && rectangle.Intersects(value))
		{
			mouseClicked = true;
			mouseSize = 1.5f;
		}
		if (currentMouseState.RightButton != oldMouseState.RightButton && currentMouseState.RightButton == ButtonState.Pressed && rectangle.Intersects(value))
		{
			mouseRightClicked = true;
			mouseSize = 0.25f;
		}
		if (currentMouseState.LeftButton != oldMouseState.LeftButton && currentMouseState.LeftButton == ButtonState.Pressed && rectangle2.Intersects(value))
		{
			direccion = -1;
		}
		if (currentMouseState.LeftButton != oldMouseState.LeftButton && currentMouseState.LeftButton == ButtonState.Pressed && rectangle3.Intersects(value))
		{
			direccion = 1;
		}
		if ((oldKeyboardState != currentKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))) || (oldGamePadState.DPad.Down != ButtonState.Pressed && currentGamePadState.DPad.Down == ButtonState.Pressed) || currentGamePadState.ThumbSticks.Left.Y < -0.5f)
		{
			direccion = -1;
		}
		if ((oldKeyboardState != currentKeyboardState && (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))) || (oldGamePadState.DPad.Up != ButtonState.Pressed && currentGamePadState.DPad.Up == ButtonState.Pressed) || currentGamePadState.ThumbSticks.Left.Y > 0.5f)
		{
			direccion = 1;
		}
		if (currentMouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue && menu[mindex].isStopped() && oldDireccion == 0)
		{
			direccion = 1;
		}
		if (currentMouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue && menu[mindex].isStopped() && oldDireccion == 0)
		{
			direccion = -1;
		}
		if (oldDireccion != direccion && direccion == -1)
		{
			mindex--;
			result = -1;
			beepHSound.Play(GOsoundFXvolume / 400f, -0.5f, 0f);
		}
		if (oldDireccion != direccion && direccion == 1)
		{
			mindex++;
			result = 1;
			beepHSound.Play(GOsoundFXvolume / 400f, -1f, 0f);
		}
		oldDireccion = direccion;
		if (mindex > menu.Count - 1)
		{
			mindex = 0;
		}
		if (mindex < 0)
		{
			mindex = menu.Count - 1;
		}
		return result;
	}

	private void UpdateOptionsMenu()
	{
		PlaySong("MainTheme");
		updateFrame = true;
		int num = 0;
		if (menuActive > 0.98f)
		{
			num = ControlMenu(ref optionsMenuIndex, ref optionsMenu);
			switch (optionsMenu[optionsMenuIndex].text)
			{
			case "Music: ":
				if (GOmusicVolume > 0f && frame > 5 && (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed || left))
				{
					GOmusicVolume -= 5f;
					optionsMenu[optionsMenuIndex].value = GOmusicVolume;
					MediaPlayer.Volume = GOmusicVolume / 100f;
					MediaPlayer.Resume();
					frame = 0u;
				}
				if (GOmusicVolume < 100f && frame > 5 && (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed || right))
				{
					GOmusicVolume += 5f;
					optionsMenu[optionsMenuIndex].value = GOmusicVolume;
					MediaPlayer.Volume = GOmusicVolume / 100f;
					MediaPlayer.Resume();
					frame = 0u;
				}
				if (GOmusicVolume < 0f)
				{
					GOmusicVolume = 0f;
				}
				if (GOmusicVolume > 100f)
				{
					GOmusicVolume = 100f;
				}
				optionsMenu[optionsMenuIndex].value = GOmusicVolume;
				MediaPlayer.Volume = GOmusicVolume / 100f;
				break;
			case "Difficulty":
				if (difficulty == 1f && ((currentKeyboardState.IsKeyDown(Keys.Left) && oldKeyboardState.IsKeyUp(Keys.Left)) || (currentGamePadState.DPad.Left == ButtonState.Pressed && oldGamePadState.DPad.Left != ButtonState.Pressed) || left))
				{
					difficulty = 0.5f;
				}
				if (difficulty == 1.8f && ((currentKeyboardState.IsKeyDown(Keys.Left) && oldKeyboardState.IsKeyUp(Keys.Left)) || (currentGamePadState.DPad.Left == ButtonState.Pressed && oldGamePadState.DPad.Left != ButtonState.Pressed) || left))
				{
					difficulty = 1f;
				}
				if (difficulty == 1f && ((currentKeyboardState.IsKeyDown(Keys.Right) && oldKeyboardState.IsKeyUp(Keys.Right)) || (currentGamePadState.DPad.Right == ButtonState.Pressed && oldGamePadState.DPad.Right != ButtonState.Pressed) || right))
				{
					difficulty = 1.8f;
				}
				if (difficulty == 0.5f && ((currentKeyboardState.IsKeyDown(Keys.Right) && oldKeyboardState.IsKeyUp(Keys.Right)) || (currentGamePadState.DPad.Right == ButtonState.Pressed && oldGamePadState.DPad.Right != ButtonState.Pressed) || right))
				{
					difficulty = 1f;
				}
				optionsMenu[optionsMenuIndex].value = difficulty;
				GOdifficulty = difficulty * 100f;
				break;
			case "Sound FX: ":
				if (GOsoundFXvolume > 0f && ((currentKeyboardState.IsKeyDown(Keys.Left) && frame > 5) || (currentGamePadState.DPad.Left == ButtonState.Pressed && oldGamePadState.DPad.Left != ButtonState.Pressed) || left))
				{
					GOsoundFXvolume -= 5f;
					frame = 0u;
				}
				if ((GOsoundFXvolume < 100f && ((currentKeyboardState.IsKeyDown(Keys.Right) && frame > 5) || (currentGamePadState.DPad.Right == ButtonState.Pressed && oldGamePadState.DPad.Right != ButtonState.Pressed))) || right)
				{
					GOsoundFXvolume += 5f;
					frame = 0u;
				}
				GOsoundFXvolume = MathHelper.Clamp(GOsoundFXvolume, 0f, 100f);
				optionsMenu[optionsMenuIndex].value = GOsoundFXvolume;
				break;
			case "Keyboard is Player":
				if (frame > 10 && keyboardPlayer > 1f && (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed || left))
				{
					beepHSound.Play(GOsoundFXvolume / 400f, -1f, 0f);
					keyboardPlayer--;
					frame = 0u;
				}
				if (frame > 10 && keyboardPlayer < 4f && (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed || right))
				{
					beepHSound.Play(GOsoundFXvolume / 400f, -0.5f, 0f);
					keyboardPlayer++;
					frame = 0u;
				}
				if (mouseClicked)
				{
					keyboardPlayer++;
				}
				if (mouseRightClicked)
				{
					keyboardPlayer--;
				}
				if (keyboardPlayer > 4f)
				{
					keyboardPlayer = 1f;
				}
				if (keyboardPlayer < 1f)
				{
					keyboardPlayer = 4f;
				}
				optionsMenu[optionsMenuIndex].value = keyboardPlayer;
				break;
			case "Back":
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					writeOptions();
					beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
					frame = 0u;
					tittle.position = Vector2.Zero;
					tittle.transparency = 0f;
					optionsMenuIndex--;
					num = -1;
					menuActive = 0f;
					resetMenus();
					gameStateNext = gameStateOld;
					gameState = gameStateOld;
				}
				break;
			case "Full Screen":
				if (graphics.IsFullScreen)
				{
					optionsMenu[optionsMenuIndex].value = 1f;
				}
				else
				{
					optionsMenu[optionsMenuIndex].value = 0f;
				}
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					if (graphics.IsFullScreen)
					{
						graphics.IsFullScreen = false;
					}
					else
					{
						graphics.IsFullScreen = true;
					}
					graphics.ApplyChanges();
				}
				break;
			case "Reset options to Default":
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					frame = 0u;
					confirm.CreateConfirmation("Are you sure you want to\nset the options to default?", GameState.optionsReseted, gameState);
					writeOptions();
				}
				break;
			case "Remove stored Data":
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					frame = 0u;
					confirm.CreateConfirmation("Are you sure you want to\nremove all your progress in the game?", GameState.progressReseted, gameState);
					writeProgress();
					writeCharacters();
				}
				break;
			case "Resolution":
			{
				double num2 = displayModes[GOresolutionIndex].Width;
				double num3 = (double)displayModes[GOresolutionIndex].Height / 10000.0;
				optionsMenu[optionsMenuIndex].value = (float)(num2 + num3);
				if ((currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Left)) || (currentGamePadState.DPad != oldGamePadState.DPad && currentGamePadState.DPad.Left == ButtonState.Pressed) || left)
				{
					GOresolutionIndex--;
				}
				if ((currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Right)) || (currentGamePadState.DPad != oldGamePadState.DPad && currentGamePadState.DPad.Right == ButtonState.Pressed) || right)
				{
					GOresolutionIndex++;
				}
				if (GOresolutionIndex < 0)
				{
					GOresolutionIndex = displayModes.Count - 1;
				}
				if (GOresolutionIndex >= displayModes.Count)
				{
					GOresolutionIndex = 0;
				}
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					graphics.PreferredBackBufferWidth = displayModes[GOresolutionIndex].Width;
					graphics.PreferredBackBufferHeight = displayModes[GOresolutionIndex].Height;
					graphics.ApplyChanges();
					resetMenus();
					repositionSelectBox();
				}
				break;
			}
			case "HUD Transparency: ":
				optionsMenu[optionsMenuIndex].value = GOHUDopacity;
				if ((currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Left)) || (currentGamePadState.DPad != oldGamePadState.DPad && currentGamePadState.DPad.Left == ButtonState.Pressed) || left)
				{
					GOHUDopacity -= 5f;
				}
				if ((currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Right)) || (currentGamePadState.DPad != oldGamePadState.DPad && currentGamePadState.DPad.Right == ButtonState.Pressed) || right)
				{
					GOHUDopacity += 5f;
				}
				if (GOHUDopacity < 0f)
				{
					GOHUDopacity = 0f;
				}
				if (GOHUDopacity >= 100f)
				{
					GOHUDopacity = 100f;
				}
				optionsMenu[optionsMenuIndex].value = GOHUDopacity;
				break;
			case "Controller Vibration":
				optionsMenu[optionsMenuIndex].value = GOvibration;
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					if (GOvibration == 1)
					{
						GOvibration = 0;
					}
					else
					{
						GOvibration = 1;
						vibrationLeft = 0.5f;
						vibrationRight = 0.5f;
					}
				}
				optionsMenu[optionsMenuIndex].value = GOvibration;
				break;
			}
			if ((oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed) || (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape)))
			{
				writeOptions();
				beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
				frame = 0u;
				tittle.position = Vector2.Zero;
				tittle.transparency = 0f;
				gameStateNext = gameStateOld;
				gameState = gameStateOld;
				resetMenus();
			}
		}
		for (int i = 0; i < optionsMenu.Count; i++)
		{
			optionsMenu[i].Update((float)num * ((float)Math.PI / (float)optionsMenu.Count));
		}
		optionsMenu[optionsMenuIndex].selected = true;
	}

	private void UpdateProgressReseted()
	{
		if (frame < 5)
		{
			setDefaultProgress(write: false);
		}
		if (frame > 120)
		{
			gameStateNext = GameState.optionsMenu;
		}
	}

	private void UpdateOptionReseted()
	{
		if (frame < 5)
		{
			setDefaultOptions(write: false);
		}
		if (frame > 120)
		{
			gameStateNext = GameState.optionsMenu;
		}
	}

	private void UpdateChallengeFinished()
	{
		endGame = 0;
		reset();
		maxEnemies = topEnemies * 2;
		UpdateSelectBackground();
		if (GOmusicVolume > 0f)
		{
			PlaySong("TimeToRun");
		}
		if (!(menuActive > 0.5f))
		{
			return;
		}
		if (challengeClear)
		{
			if (challengeNumber < challengeList.selectables.Count - 1 && ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || (oldMouseState.LeftButton != ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed)))
			{
				if (demo)
				{
					if (challengeNumber < maxDemoLevel * 2)
					{
						challengeNumber++;
						createChallengeLevel();
						gameStateNext = GameState.Challenge;
						menuActive = 0f;
						challengeClear = false;
						writeProgress();
					}
				}
				else
				{
					challengeNumber++;
					createChallengeLevel();
					gameStateNext = GameState.Challenge;
					menuActive = 0f;
					challengeClear = false;
					writeProgress();
				}
			}
			if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Space)) || (oldGamePadState.Buttons.X != ButtonState.Pressed && currentGamePadState.Buttons.X == ButtonState.Pressed) || (oldMouseState.MiddleButton != ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Pressed))
			{
				createChallengeLevel();
				gameStateNext = GameState.Challenge;
				menuActive = 0f;
				challengeClear = false;
				writeProgress();
			}
		}
		else if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || (oldMouseState.LeftButton != ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed))
		{
			createChallengeLevel();
			gameStateNext = GameState.Challenge;
			menuActive = 0f;
			challengeClear = false;
			writeProgress();
		}
		if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape)) || (oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed) || (oldMouseState.RightButton != ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed))
		{
			gameStateNext = GameState.selectChallenge;
			menuActive = 0f;
			challengeClear = false;
			writeProgress();
		}
	}

	private void UpdateSurvivalStatsMenu()
	{
		if (menuActive < 1f)
		{
			menuActive = MathHelper.SmoothStep(menuActive, 1.01f, 0.15f);
			menuActive = MathHelper.Clamp(menuActive, 0f, 1f);
		}
		int num = ControlMenu(ref survivalStatsMenuIndex, ref survivalStatsMenu);
		if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
		{
			beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
			switch (survivalStatsMenu[survivalStatsMenuIndex].text)
			{
			case "Play Again":
				createSurvivalLevel(characters: false);
				resetPlayersSurvival();
				frame = 0u;
				gameStateNext = GameState.Survival;
				gameStatePlay = GameState.Survival;
				menuActive = 0f;
				resetMenus();
				break;
			case "Character Selection":
				createSurvivalLevel(characters: true);
				resetPlayers();
				frame = 0u;
				gameStateNext = GameState.selectPlayer;
				gameStatePlay = GameState.Survival;
				menuActive = 0f;
				resetMenus();
				break;
			case "Main Menu":
				createSurvivalLevel(characters: true);
				resetPlayers();
				frame = 0u;
				gameStateNext = GameState.mainMenu;
				gameStatePlay = GameState.Survival;
				menuActive = 0f;
				resetMenus();
				break;
			case "Select Game Mode":
				createSurvivalLevel(characters: true);
				resetPlayers();
				frame = 0u;
				gameStateNext = GameState.playMenu;
				gameStatePlay = GameState.Survival;
				menuActive = 0f;
				resetMenus();
				break;
			case "Options Menu":
				createSurvivalLevel(characters: true);
				resetPlayers();
				frame = 0u;
				gameStateOld = gameState;
				gameStateNext = GameState.optionsMenu;
				gameStatePlay = GameState.Survival;
				menuActive = 0f;
				resetMenus();
				break;
			}
		}
		for (int i = 0; i < survivalStatsMenu.Count; i++)
		{
			survivalStatsMenu[i].Update((float)num * ((float)Math.PI / (float)survivalStatsMenu.Count));
		}
		if (survivalStatsMenuIndex >= survivalStatsMenu.Count)
		{
			survivalStatsMenuIndex = 0;
		}
		if (survivalStatsMenuIndex < 0)
		{
			survivalStatsMenuIndex = survivalStatsMenu.Count - 1;
		}
		survivalStatsMenuIndex = (int)MathHelper.Clamp(survivalStatsMenuIndex, 0f, survivalStatsMenu.Count - 1);
		survivalStatsMenu[survivalStatsMenuIndex].selected = true;
	}

	private void resetMenus()
	{
		menuActive = 0f;
		Vector2 pos = new Vector2((float)base.GraphicsDevice.Viewport.Width / 2.8f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		for (int i = 0; i < mainMenu.Count; i++)
		{
			mainMenu[i].wide = mainMenu.Count * 14;
			mainMenu[i].reset(pos);
		}
		pos = new Vector2((float)base.GraphicsDevice.Viewport.Width / 3f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		for (int i = 0; i < playMenu.Count; i++)
		{
			playMenu[i].wide = playMenu.Count * 14;
			playMenu[i].reset(pos);
		}
		pos = new Vector2((float)base.GraphicsDevice.Viewport.Width / 4f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		for (int i = 0; i < optionsMenu.Count; i++)
		{
			optionsMenu[i].wide = optionsMenu.Count * 14;
			optionsMenu[i].reset(pos);
		}
		pos = new Vector2((float)base.GraphicsDevice.Viewport.Width / 3f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		for (int i = 0; i < survivalStatsMenu.Count; i++)
		{
			survivalStatsMenu[i].wide = survivalStatsMenu.Count * 14;
			survivalStatsMenu[i].reset(pos);
		}
		pos = new Vector2((float)base.GraphicsDevice.Viewport.Width / 3f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		for (int i = 0; i < pauseMenu.Count; i++)
		{
			pauseMenu[i].wide = pauseMenu.Count * 14;
			pauseMenu[i].reset(pos);
		}
	}

	private void UpdatePauseMenu()
	{
		int num = ControlMenu(ref pauseMenuIndex, ref pauseMenu);
		if (pauseMenu[pauseMenuIndex].selectable)
		{
			switch (pauseMenu[pauseMenuIndex].text)
			{
			case "Music: ":
				if (pauseMenu[pauseMenuIndex].value > 0f && (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed))
				{
					pauseMenu[pauseMenuIndex].value--;
					GOmusicVolume = pauseMenu[pauseMenuIndex].value;
					MediaPlayer.Volume = GOmusicVolume / 100f;
					MediaPlayer.Resume();
				}
				if (pauseMenu[pauseMenuIndex].value < 100f && (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed))
				{
					pauseMenu[pauseMenuIndex].value++;
					GOmusicVolume = pauseMenu[pauseMenuIndex].value;
					MediaPlayer.Volume = GOmusicVolume / 100f;
					MediaPlayer.Resume();
				}
				if (pauseMenu[pauseMenuIndex].value < 0f)
				{
					pauseMenu[pauseMenuIndex].value = 0f;
				}
				GOmusicVolume = pauseMenu[pauseMenuIndex].value;
				MediaPlayer.Volume = GOmusicVolume / 100f;
				break;
			case "Sound FX: ":
				if (pauseMenu[pauseMenuIndex].value > 0f && (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed))
				{
					GOsoundFXvolume = pauseMenu[pauseMenuIndex].value--;
				}
				if (pauseMenu[pauseMenuIndex].value < 100f && (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed))
				{
					GOsoundFXvolume = pauseMenu[pauseMenuIndex].value++;
				}
				MathHelper.Clamp(pauseMenu[pauseMenuIndex].value, 0f, 100f);
				GOsoundFXvolume = pauseMenu[pauseMenuIndex].value;
				break;
			case "Keyboard is Player":
				if (frame > 10 && pauseMenu[pauseMenuIndex].value > 1f && (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed))
				{
					beepHSound.Play(GOsoundFXvolume / 200f, -1f, 0f);
					pauseMenu[pauseMenuIndex].value--;
					frame = 0u;
				}
				if (frame > 10 && pauseMenu[pauseMenuIndex].value < 4f && (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed))
				{
					beepHSound.Play(GOsoundFXvolume / 200f, -0.5f, 0f);
					pauseMenu[pauseMenuIndex].value++;
					frame = 0u;
				}
				keyboardPlayer = pauseMenu[pauseMenuIndex].value;
				break;
			case "Exit to Galaxy Map":
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					confirm.CreateConfirmation("Are you sure you want to exit to the Galaxy Map?", GameState.galaxyMap, GameState.pause);
				}
				break;
			case "Exit to Main Menu":
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					confirm.CreateConfirmation("Are you sure you want to exit to the Main Menu?", GameState.startScreen, GameState.pause);
				}
				break;
			case "Resume":
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
					pauseTarget = -50f;
				}
				break;
			case "Full Screen":
				if (graphics.IsFullScreen)
				{
					pauseMenu[pauseMenuIndex].value = 1f;
				}
				else
				{
					pauseMenu[pauseMenuIndex].value = 0f;
				}
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					if (graphics.IsFullScreen)
					{
						graphics.IsFullScreen = false;
					}
					else
					{
						graphics.IsFullScreen = true;
					}
					graphics.ApplyChanges();
				}
				break;
			case "HUD Transparency: ":
				pauseMenu[pauseMenuIndex].value = GOHUDopacity;
				if ((currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Left)) || (currentGamePadState.DPad != oldGamePadState.DPad && currentGamePadState.DPad.Left == ButtonState.Pressed))
				{
					GOHUDopacity -= 5f;
				}
				if ((currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Right)) || (currentGamePadState.DPad != oldGamePadState.DPad && currentGamePadState.DPad.Right == ButtonState.Pressed))
				{
					GOHUDopacity += 5f;
				}
				if (GOHUDopacity < 0f)
				{
					GOHUDopacity = 0f;
				}
				if (GOHUDopacity >= 100f)
				{
					GOHUDopacity = 100f;
				}
				pauseMenu[pauseMenuIndex].value = GOHUDopacity;
				break;
			case "Controller Vibration":
				pauseMenu[pauseMenuIndex].value = GOvibration;
				if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
				{
					if (GOvibration == 1)
					{
						GOvibration = 0;
					}
					else
					{
						GOvibration = 1;
						vibrationLeft = 0.5f;
						vibrationRight = 0.5f;
					}
				}
				pauseMenu[pauseMenuIndex].value = GOvibration;
				break;
			}
		}
		for (int i = 0; i < pauseMenu.Count; i++)
		{
			pauseMenu[i].Update((float)num * ((float)Math.PI / (float)pauseMenu.Count));
		}
		pauseMenu[pauseMenuIndex].selected = true;
	}

	private void setMessages()
	{
		setMessages(b: false);
	}

	private void setMessages(bool b)
	{
		relicMessage = b;
		chargeMessage = b;
		orbMessage = b;
		healthMessage = b;
		enemyMessage = b;
		basicsMessage = b;
		eliminateMessage = b;
		moveMessage = b;
		tutorialItems = 0;
		tutorialCounter = 0;
	}

	private void UpdatePlayMenu()
	{
		PlaySong("MainTheme");
		updateFrame = true;
		setMessages(b: true);
		int num = ControlMenu(ref playMenuIndex, ref playMenu);
		for (int i = 0; i < playMenu.Count; i++)
		{
			switch (playMenu[i].text)
			{
			case "Chubby Rain Mode":
				playMenu[i].selectable = unlockChubbyRain;
				break;
			case "Meteroids Mode":
				playMenu[i].selectable = unlockMeteroids;
				break;
			case "Sidescroller":
				playMenu[i].selectable = unlockSidescroller;
				break;
			case "Boss Fight":
				playMenu[i].selectable = unlockBoss;
				break;
			}
		}
		if ((oldKeyboardState != currentKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || (oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || mouseClicked)
		{
			repositionSelectBox();
			readCharacters();
			if (playMenu[playMenuIndex].selectable)
			{
				switch (playMenu[playMenuIndex].text)
				{
				case "Campaign":
					currentLevel = latestLevel;
					nextLevel = latestLevel;
					iniCharacters(initializeCharacters: false);
					frame = 0u;
					gameStateOld = gameState;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.galaxyMap;
					menuActive = 0f;
					break;
				case "Play Tutorial":
					setMessages(b: false);
					currentLevel = 14;
					iniCharacters(initializeCharacters: false);
					frame = 0u;
					gameStateOld = gameState;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.Campaign;
					menuActive = 0f;
					break;
				case "Challenge Mode":
					CreateSelectChallengeUI();
					frame = 0u;
					gameStateOld = gameState;
					gameStateNext = GameState.selectChallenge;
					gameStatePlay = GameState.Challenge;
					menuActive = 0f;
					break;
				case "Survival Mode":
					createSurvivalLevel(characters: true);
					frame = 0u;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.Survival;
					menuActive = 0f;
					break;
				case "Meteroids Mode":
					createSurvivalLevel(characters: true);
					frame = 0u;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.Meteroids;
					menuActive = 0f;
					break;
				case "Chubby Rain Mode":
					createSurvivalLevel(characters: true);
					frame = 0u;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.ChubbyRain;
					menuActive = 0f;
					break;
				case "Sidescroller":
					createSurvivalLevel(characters: true);
					frame = 0u;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.Sidescroller;
					menuActive = 0f;
					break;
				case "Boss Fight":
					iniCharacters(initializeCharacters: true, addRelics: true);
					currentLevel = 13;
					frame = 0u;
					colony.health = colony.MaximunHealth;
					gameStateNext = GameState.selectPlayer;
					gameStatePlay = GameState.finalBoss;
					menuActive = 0f;
					break;
				case "Create online campaign":
					NetCreateServer();
					frame = 0u;
					gameStateNext = GameState.selectPlayer;
					menuActive = 0f;
					break;
				case "Join online campaign":
					NetClientConnect();
					frame = 0u;
					gameStateNext = GameState.selectPlayer;
					break;
				case "Back":
					num = -3;
					playMenuIndex -= 3;
					tittle.position = Vector2.Zero;
					tittle.transparency = 0f;
					gameStateNext = GameState.mainMenu;
					gameState = GameState.mainMenu;
					resetMenus();
					menuActive = 0f;
					break;
				}
			}
		}
		if ((oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed) || (!oldKeyboardState.IsKeyDown(Keys.Escape) && currentKeyboardState.IsKeyDown(Keys.Escape)))
		{
			beepSSound.Play(GOsoundFXvolume / 200f, 0f, 0f);
			tittle.position = Vector2.Zero;
			tittle.transparency = 0f;
			gameStateNext = GameState.mainMenu;
			gameState = GameState.mainMenu;
			resetMenus();
		}
		for (int i = 0; i < playMenu.Count; i++)
		{
			playMenu[i].Update((float)num * ((float)Math.PI / (float)playMenu.Count));
		}
		playMenu[playMenuIndex].selected = true;
	}

	private void CreateSelectChallengeUI()
	{
		int num = base.GraphicsDevice.Viewport.TitleSafeArea.Left;
		if (num < 50)
		{
			num = 50;
		}
		int num2 = base.GraphicsDevice.Viewport.TitleSafeArea.Right;
		if (num2 > base.GraphicsDevice.Viewport.Width - 100)
		{
			num2 = base.GraphicsDevice.Viewport.Width - 100;
		}
		int num3 = base.GraphicsDevice.Viewport.TitleSafeArea.Top;
		if (num3 < 50)
		{
			num3 = 50;
		}
		int num4 = base.GraphicsDevice.Viewport.TitleSafeArea.Bottom;
		if (num4 > base.GraphicsDevice.Viewport.Height - 50)
		{
			num4 = base.GraphicsDevice.Viewport.Height - 50;
		}
		int num5 = (int)((float)base.GraphicsDevice.Viewport.Height * 0.2f);
		if (num5 < num3)
		{
			num5 = num3;
		}
		int num6 = (int)((float)(base.GraphicsDevice.Viewport.Width / 2) - gameFont.MeasureString("Challenge_00000Challenge_00000Challenge_00").X / 2f) - 10;
		challengeList = new SelectableManager(40);
		challengeUI = new SelectableManager(4);
		for (int i = 0; i < 10; i++)
		{
			challengeList.Add(new Selectable(gameFont, files[i].Substring(filesChar, 12), new Vector2(num6, num5 + i * 40), Color.Gray));
		}
		for (int i = 10; i < 20; i++)
		{
			challengeList.Add(new Selectable(gameFont, files[i].Substring(filesChar, 12), new Vector2(num6 + 250, num5 + (i - 10) * 40), Color.Gray));
		}
		for (int i = 20; i < files.Length; i++)
		{
			challengeList.Add(new Selectable(gameFont, files[i].Substring(filesChar, 12), new Vector2(num6 + 500, num5 + (i - 20) * 40), Color.Gray));
		}
		challengeList.selectables[0].unlock = true;
		if (editor)
		{
			for (int i = 0; i < files.Length; i++)
			{
				challengeList.selectables[i].unlock = true;
			}
		}
		challengeUI.Add(new Selectable(gameFont, "Back", new Vector2(num, num4), Color.Gray));
		challengeUI.Add(new Selectable(gameFont, "Play", new Vector2(num2, num4), Color.Gray));
		readProgress();
	}

	private void UpdateCleanGarbage()
	{
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(controllingPlayer);
		GC.Collect();
		gameState = GameState.Campaign;
		gameStateNext = GameState.Campaign;
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
	}

	private void UpdateLooseCampaign()
	{
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].wasActive)
			{
				player[i].Health = player[i].maximunHealth;
				player[i].Active = true;
			}
		}
		if (frame > 10 && ((oldGamePadState != currentGamePadState && (currentGamePadState.Buttons.Back == ButtonState.Pressed || currentGamePadState.Buttons.Start == ButtonState.Pressed || currentGamePadState.Buttons.A == ButtonState.Pressed || currentGamePadState.Buttons.B == ButtonState.Pressed)) || (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed) || (oldKeyboardState != currentKeyboardState && (Keyboard.GetState().IsKeyDown(Keys.Escape) || Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space)))))
		{
			frame = 0u;
			gameStateNext = GameState.galaxyMap;
		}
		if (frame > 400)
		{
			frame = 0u;
			gameStateNext = GameState.galaxyMap;
		}
		if (vibrationLeft > 0f)
		{
			vibrationLeft -= 0.01f;
		}
		if (vibrationRight > 0f)
		{
			vibrationRight -= 0.01f;
		}
		vibrationLeft = MathHelper.Clamp(vibrationLeft, 0f, 1f);
		vibrationRight = MathHelper.Clamp(vibrationRight, 0f, 1f);
		UpdateLens();
		ResetVibration();
	}

	private static void ResetVibration()
	{
		try
		{
			GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
			GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);
			GamePad.SetVibration(PlayerIndex.Three, 0f, 0f);
			GamePad.SetVibration(PlayerIndex.Four, 0f, 0f);
		}
		catch
		{
		}
	}

	private void UpdateSurvivalStats()
	{
		UpdateSelectBackground();
		UpdateSurvivalStatsMenu();
		if (vibrationLeft > 0f)
		{
			vibrationLeft -= 0.01f;
		}
		if (vibrationRight > 0f)
		{
			vibrationRight -= 0.01f;
		}
		vibrationLeft = MathHelper.Clamp(vibrationLeft, 0f, 1f);
		vibrationRight = MathHelper.Clamp(vibrationRight, 0f, 1f);
		GamePad.SetVibration(PlayerIndex.One, vibrationLeft, vibrationRight);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		switch (gameState)
		{
		case GameState.disclaimer:
			drawDisclaimer();
			break;
		case GameState.intro:
			DrawIntro(spriteBatch);
			break;
		case GameState.ending:
			DrawEnding(spriteBatch);
			break;
		case GameState.galaxyMap:
			DrawGalaxyMap(spriteBatch);
			DrawMouseUI();
			break;
		case GameState.selectPlayer:
			DrawSelectPlayer();
			DrawMouseUI();
			break;
		case GameState.Campaign:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			if (editor)
			{
				DrawEditor(assetManager[currentLevel]);
			}
			else
			{
				DrawCampaign();
			}
			break;
		case GameState.Challenge:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			if (editor)
			{
				DrawEditor(assetManagerChallenge[challengeNumber]);
			}
			else
			{
				DrawChallenge();
			}
			break;
		case GameState.Versus:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			if (editor)
			{
				DrawEditor(assetManagerChallenge[challengeNumber]);
			}
			else
			{
				DrawVSmode();
			}
			break;
		case GameState.finalBoss:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			if (editor)
			{
				DrawEditor(assetManager[currentLevel]);
			}
			else
			{
				DrawFinalBoss();
			}
			break;
		case GameState.Survival:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			DrawSurvival();
			break;
		case GameState.Meteroids:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			DrawMeteroids();
			break;
		case GameState.ChubbyRain:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			DrawChubbyRain();
			break;
		case GameState.Sidescroller:
			frameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			frameCounter++;
			if (frameTime > 1000f)
			{
				currentFrameRate = frameCounter;
				frameTime = 0f;
				frameCounter = 0;
			}
			DrawSidescroller();
			break;
		case GameState.logo:
			DrawLogo();
			break;
		case GameState.howtoplay:
			DrawHowToPlay();
			DrawMouseUI();
			break;
		case GameState.controls:
			DrawControls();
			DrawMouseUI();
			break;
		case GameState.awards:
			DrawAwards();
			DrawMouseUI();
			break;
		case GameState.selectChallenge:
			DrawSelectChallenge();
			DrawMouseUI();
			break;
		case GameState.credits:
			DrawCredits();
			DrawMouseUI();
			break;
		case GameState.clean:
			DrawCampaign();
			break;
		case GameState.startScreen:
			DrawMenu();
			DrawStartScreen();
			DrawMouseUI();
			break;
		case GameState.mainMenu:
			DrawMenu();
			DrawMouseUI();
			break;
		case GameState.message:
			switch (gameStatePlay)
			{
			case GameState.Campaign:
				DrawCampaign();
				break;
			case GameState.Survival:
				DrawSurvival();
				break;
			case GameState.Sidescroller:
				DrawSidescroller();
				break;
			case GameState.Meteroids:
				DrawMeteroids();
				break;
			case GameState.ChubbyRain:
				DrawChubbyRain();
				break;
			default:
				DrawSurvival();
				break;
			}
			DrawMessageInfo();
			DrawMouseUI();
			break;
		case GameState.playMenu:
			DrawPlayMenu();
			DrawMouseUI();
			break;
		case GameState.optionsMenu:
			DrawOptionsMenu();
			DrawMouseUI();
			break;
		case GameState.optionsReseted:
			DrawOptionsReseted();
			DrawMouseUI();
			break;
		case GameState.progressReseted:
			DrawProgressReseted();
			DrawMouseUI();
			break;
		case GameState.challengeFinished:
			DrawChallengeFinished();
			DrawMouseUI();
			break;
		case GameState.score:
			switch (gameStatePlay)
			{
			case GameState.Campaign:
				DrawCampaign();
				DrawLooseCampaign();
				break;
			case GameState.Survival:
				DrawSurvival();
				DrawSurvivalStats();
				break;
			}
			DrawMouseUI();
			break;
		case GameState.pause:
			switch (gameStatePlay)
			{
			case GameState.Campaign:
				DrawCampaign();
				break;
			case GameState.Challenge:
				DrawChallenge();
				break;
			case GameState.Survival:
				DrawSurvival();
				break;
			case GameState.ChubbyRain:
				DrawChubbyRain();
				break;
			case GameState.Sidescroller:
				DrawSidescroller();
				break;
			case GameState.Meteroids:
				DrawMeteroids();
				break;
			case GameState.finalBoss:
				DrawFinalBoss();
				break;
			default:
				DrawCampaign();
				break;
			}
			DrawPauseMenu();
			DrawMouseUI();
			break;
		case GameState.buyGame1:
			DrawBuyGame1();
			break;
		case GameState.buyGame2:
			DrawBuyGame2();
			break;
		case GameState.endDemo1:
		case GameState.endDemo2:
			DrawEndDemo();
			break;
		case GameState.BonusChubbyRain:
		case GameState.BonusMeteroids:
		case GameState.BonusSidescroller:
			DrawBonus();
			break;
		case GameState.BonusClear:
		case GameState.BonusFailed:
			DrawBonusClear();
			break;
		}
		if (confirm.text != "")
		{
			spriteBatch.Begin();
			spriteBatch.Draw(txBlack, new Vector2(0f, 0f), null, new Color(0f, 0f, 0f, confirm.transp * 0.5f), 0f, new Vector2(0f, 0f), new Vector2(base.GraphicsDevice.Viewport.Width / txBlack.Width, base.GraphicsDevice.Viewport.Height / txBlack.Height), SpriteEffects.None, 0f);
			spriteBatch.Draw(txBlack, new Vector2(base.GraphicsDevice.Viewport.Width / 2, 270f), null, new Color(0f, 0f, 0f, confirm.transp * 0.75f), 0f, new Vector2(txBlack.Width / 2, txBlack.Height / 2), new Vector2(menuFont.MeasureString(confirm.text).X / 9f, 18f), SpriteEffects.None, 0f);
			confirm.Draw(spriteBatch, menuFont);
			spriteBatch.End();
			DrawMouseUI();
		}
		spriteBatch.Begin();
		if (demo && (gameState != GameState.mainMenu || gameState != GameState.optionsMenu || gameState != GameState.playMenu || gameState != GameState.awards))
		{
			spriteBatch.DrawString(menuFont, "Demo", new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.7f, (float)base.GraphicsDevice.Viewport.Height * 0.8f), new Color(0.7f, 0.1f, 0.1f, 0.3f) * menuActive, -0.4f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
		}
		spriteBatch.Draw(txBlack, new Vector2(0f, 0f), null, new Color(0f, 0f, 0f, fadeToBlack), 0f, new Vector2(0f, 0f), new Vector2(base.GraphicsDevice.Viewport.Width / txBlack.Width, base.GraphicsDevice.Viewport.Height / txBlack.Height), SpriteEffects.None, 0f);
		if (showData && player.Length > 0)
		{
			spriteBatch.DrawString(gameFont, " 0: " + player[0].orbs + " / " + player[0].maxOrbs, Vector2.UnitY * 20f, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
		spriteBatch.End();
		awards.Draw(spriteBatch, gameFont, base.GraphicsDevice);
		base.Draw(gameTime);
	}

	private void DrawProgressReseted()
	{
		DrawOptionsMenu();
		spriteBatch.Begin();
		spriteBatch.Draw(txBlack, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White * menuActive * 0.5f);
		spriteBatch.DrawString(menuFont, "Progress removed", new Vector2(base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height) / 2f - menuFont.MeasureString("Progress removed") / 2f, Color.White * menuActive * 0.5f);
		spriteBatch.End();
	}

	private void DrawOptionsReseted()
	{
		DrawOptionsMenu();
		spriteBatch.Begin();
		spriteBatch.Draw(txBlack, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White * menuActive * 0.5f);
		spriteBatch.DrawString(menuFont, "Options set to default", new Vector2(base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height) / 2f - menuFont.MeasureString("Options set to default") / 2f, Color.White * menuActive * 0.5f);
		spriteBatch.End();
	}

	private void DrawChallengeFinished()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		selectBar.Draw(spriteBatch, 0.2f, 0.3f);
		selectBackground.Draw(spriteBatch, 1f, 0.2f);
		selectBar2.Draw(spriteBatch, 0.1f, 0.15f);
		selectHUD.Draw(spriteBatch, 1f, 0.1f);
		spriteBatch.End();
		spriteBatch.Begin();
		Vector2 vector = new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) - gameFont.MeasureString("(A) Play next challenge").X / 2f, base.GraphicsDevice.Viewport.Height / 2);
		if (challengeClear)
		{
			spriteBatch.DrawString(menuFont, "Challenge Clear!", new Vector2(base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height / 3) / 2f - menuFont.MeasureString("Challenge Clear!") / 2f, Color.White * menuActive * 0.5f);
			if (challengeNumber < challengeList.selectables.Count - 1)
			{
				if (demo)
				{
					if (challengeNumber < maxDemoLevel * 2)
					{
						Text.print(spriteBatch, gameFont, "(A) Play next challenge", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
					}
				}
				else
				{
					Text.print(spriteBatch, gameFont, "(A) Play next challenge", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
				}
			}
			Text.print(spriteBatch, gameFont, "(X) Play again", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		}
		else
		{
			spriteBatch.DrawString(menuFont, "Challenge failed!", new Vector2(base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height / 3) / 2f - menuFont.MeasureString("Challenge failed!") / 2f, Color.White * menuActive * 0.5f);
			Text.print(spriteBatch, gameFont, "(A) Play again", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		}
		Text.print(spriteBatch, gameFont, "(B) Back to challenge selection", new Vector2(vector.X * menuActive, vector.Y + 50f), Color.White * menuActive, txButtons);
		spriteBatch.End();
	}

	private void DrawHowToPlay()
	{
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		int num = base.GraphicsDevice.Viewport.Width / 2;
		for (int i = 0; i < howToPlay.Count; i++)
		{
			spriteBatch.Draw(howToPlay[i].tx, new Rectangle(base.GraphicsDevice.Viewport.Width / 2 - (int)((float)num * howToPlay[i].transp), 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White * howToPlay[i].transp, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		Text.print(spriteBatch, gameFont, "(A) for the next slide", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(B) Back to main menu", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
	}

	private void DrawControls()
	{
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		int num = base.GraphicsDevice.Viewport.Width / 2;
		for (int i = 0; i < controls.Count; i++)
		{
			spriteBatch.Draw(controls[i].tx, new Rectangle(base.GraphicsDevice.Viewport.Width / 2 - (int)((float)num * controls[i].transp), 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White * controls[i].transp, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		Text.print(spriteBatch, gameFont, "(A) for the next slide", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(B) Back to main menu", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
	}

	private void DrawAwards()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		spriteBatch.Draw(txAwardsScreen, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		spriteBatch.End();
		awards.DrawDebug(spriteBatch, gameFont, menuFont, base.GraphicsDevice, menuActive);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
		spriteBatch.Draw(txAwardSelected, awards.Data[awards.selected].pos - new Vector2(txAwardSelected.Width, txAwardSelected.Height) / 2f, Color.White * menuActive * menuActive * menuActive);
		spriteBatch.DrawString(gameFont, awards.getPoints() + " of " + awards.totalPoints + " total points,  " + awards.getPercentage() + "% awards completed", new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.35f, (float)base.GraphicsDevice.Viewport.Height * 0.177f), Color.LightCyan);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		Text.print(spriteBatch, gameFont, "(B) Back to main menu", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
	}

	private void DrawEndDemo()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		spriteBatch.Draw(txBuy1, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		Text.print(spriteBatch, gameFont, "(Y) Buy the game", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(A)(B) Continue", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
		DrawMouseUI();
	}

	private void DrawBuyGame1()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		spriteBatch.Draw(txBuy1, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		spriteBatch.DrawString(tittlesFont, "End of the Demo", new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) - tittlesFont.MeasureString("End of the Demo").X / 2f, menuActive * 50f), Color.White * menuActive);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		Text.print(spriteBatch, gameFont, "(Y) Buy the game", new Vector2(vector.X * menuActive, vector.Y - 100f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(A) Continue", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(B) Back to main menu", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
		DrawMouseUI();
	}

	private void DrawBuyGame2()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		spriteBatch.Draw(txBuy2, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		Text.print(spriteBatch, gameFont, "(Y) Buy the game", new Vector2(vector.X * menuActive, vector.Y - 100f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(A) Exit", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(B) Back to main menu", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
		DrawMouseUI();
	}

	private void DrawSelectChallenge()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		selectBar.Draw(spriteBatch, 0.2f, 0.3f);
		selectBackground.Draw(spriteBatch, 1f, 0.2f);
		selectBar2.Draw(spriteBatch, 0.1f, 0.15f);
		selectHUD.Draw(spriteBatch, 1f, 0.1f);
		Rectangle rec = new Rectangle(challengeList.selectables[0].rec.Left - 10, challengeList.selectables[0].rec.Top - 10, challengeList.selectables[challengeList.selectables.Count - 1].rec.Right - challengeList.selectables[0].rec.Left + 20, challengeList.selectables[challengeList.selectables.Count - 1].rec.Bottom - challengeList.selectables[0].rec.Top + 20);
		draw2d.DrawPixel(spriteBatch, rec, Color.Black * 0.75f * menuActive);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		challengeList.Draw(spriteBatch, unlockable: true, challengeNumber);
		Vector2 position = new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) - tittlesFont.MeasureString("Select Challenge").X / 2f, (float)base.GraphicsDevice.Viewport.TitleSafeArea.Top * menuActive);
		if (position.Y < 50f * menuActive)
		{
			position.Y = 50f * menuActive;
		}
		spriteBatch.DrawString(tittlesFont, "Select Challenge", position, Color.LightCyan * menuActive);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		if (demo)
		{
			Text.print(spriteBatch, gameFont, "(Y) Buy full game to play all the challenges", new Vector2(vector.X * menuActive, vector.Y - 100f), Color.White * menuActive, txButtons);
		}
		Text.print(spriteBatch, gameFont, "(A) Play", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(B) Back", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
	}

	private void DrawLooseCampaign()
	{
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		spriteBatch.Draw(txBlack, new Vector2(0f, 0f), null, new Color(0f, 0f, 0f, 0.5f), 0f, new Vector2(0f, 0f), new Vector2(base.GraphicsDevice.Viewport.Width / txBlack.Width, base.GraphicsDevice.Viewport.Height / txBlack.Height), SpriteEffects.None, 0f);
		spriteBatch.DrawString(gameFont, "You have lost the battle", new Vector2(base.GraphicsDevice.Viewport.Width / 2, (float)base.GraphicsDevice.Viewport.Height * 0.3f), Color.LightCyan, 0f, new Vector2(gameFont.MeasureString("You lost the battle").X / 2f, gameFont.MeasureString("END").Y / 2f), 1f, SpriteEffects.None, 1f);
		string text = "Press";
		text += " (A) or Start";
		text += " to continue";
		Vector2 vector = new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height / 2);
		Text.print(spriteBatch, gameFont, text, vector - gameFont.MeasureString(text) / 2f, Color.White, txButtons);
		spriteBatch.End();
	}

	private void DrawCredits()
	{
		DrawMenu();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		spriteBatch.Draw(txBlack, new Vector2(0f, 0f), null, new Color(0f, 0f, 0f, 0.5f), 0f, new Vector2(0f, 0f), new Vector2(base.GraphicsDevice.Viewport.Width / txBlack.Width, base.GraphicsDevice.Viewport.Height / txBlack.Height), SpriteEffects.None, 1f);
		float scale = 0.75f;
		float num = base.GraphicsDevice.Viewport.Width / 2 - 300;
		float num2 = base.GraphicsDevice.Viewport.Height - frame;
		if (base.GraphicsDevice.Viewport.Width < 1200)
		{
			num = 50f;
		}
		if (base.GraphicsDevice.Viewport.Width < 799)
		{
			num = 5f;
			scale = 0.5f;
		}
		spriteBatch.DrawString(menuFont, "A game by", new Vector2(num, num2), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Sergio Santos\nVictor Santos", new Vector2(num + 150f, num2 += 100f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Programming by", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Sergio Santos", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Art by", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Victor Santos\nSergio Santos", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Game design by", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Victor Santos\nSergio Santos", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Music and Sound by", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Sergio Santos\nVictor Santos\nManuel Marino", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "http://manuelmarinofficial.com/\nTracks:", new Vector2(num + 150f, num2 += 100f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "He is alive\nTime to run", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Looperman.com", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Alividlife\nplanetjazzbass\ncentrist\nMio\nDj4Real\nSpivkurl\nMinor2go\nRoseerin\nRatty\nSpivkurl\nDjcuFool", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Testing by", new Vector2(num, num2 += 350f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Beatriz Pedroche\nSandra Linares", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Thanks for their support to", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Our family and friends", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Special thanks to", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "David Schoneveld\nAndrew Tamandl", new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, "Thanks for playing!", new Vector2(num, num2 += 150f), Color.Cyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.DrawString(menuFont, kpSite, new Vector2(num + 150f, num2 += 50f), Color.LightCyan, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.End();
		if (num2 < (float)(base.GraphicsDevice.Viewport.Height / 2))
		{
			gameStateNext = GameState.mainMenu;
		}
	}

	private void DrawSurvivalStats()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		selectBar.Draw(spriteBatch, 0.2f, 0.3f);
		selectBackground.Draw(spriteBatch, 1f, 0.2f);
		selectBar2.Draw(spriteBatch, 0.1f, 0.15f);
		selectHUD.Draw(spriteBatch, 1f, 0.1f);
		spriteBatch.End();
		List<Vector2> list = new List<Vector2>(4);
		Vector2 vector = Vector2.UnitY * 30f;
		Vector2 vector2 = Vector2.One * 10f;
		Vector2 vector3 = Vector2.UnitX * 10f;
		float num = 40f;
		num = 0f;
		list.Add(new Vector2(num + (float)base.GraphicsDevice.Viewport.TitleSafeArea.Left, num + (float)base.GraphicsDevice.Viewport.TitleSafeArea.Top) * menuActive * menuActive * menuActive * menuActive);
		list.Add(new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.7f - num, num + (float)base.GraphicsDevice.Viewport.TitleSafeArea.Top) * menuActive * menuActive * menuActive);
		list.Add(new Vector2(num + (float)base.GraphicsDevice.Viewport.TitleSafeArea.Left, base.GraphicsDevice.Viewport.Height / 2) * menuActive * menuActive);
		list.Add(new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.7f - num, base.GraphicsDevice.Viewport.Height / 2) * menuActive);
		for (int i = 0; i < player.Length; i++)
		{
			int num2 = 0;
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			draw2d.DrawPixel(spriteBatch, new Rectangle((int)list[i].X, (int)list[i].Y, (int)((float)base.GraphicsDevice.Viewport.Width * 0.3f * menuActive), (int)((float)base.GraphicsDevice.Viewport.Height * 0.4f * menuActive)), Color.Black * 0.5f * menuActive);
			draw2d.DrawRectangle(spriteBatch, new Rectangle((int)list[i].X, (int)list[i].Y, (int)((float)base.GraphicsDevice.Viewport.Width * 0.3f * menuActive), (int)((float)base.GraphicsDevice.Viewport.Height * 0.4f * menuActive)), Color.Gray * menuActive, 2);
			spriteBatch.End();
			if (!player[i].wasActive)
			{
				continue;
			}
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			draw2d.DrawRectangle(spriteBatch, new Rectangle((int)list[i].X, (int)list[i].Y, (int)((float)base.GraphicsDevice.Viewport.Width * 0.3f * menuActive), (int)((float)base.GraphicsDevice.Viewport.Height * 0.4f * menuActive)), player[i].shootColor * menuActive, 2);
			spriteBatch.DrawString(menuFont, "-= PLAYER " + (i + 1) + " =-", list[i] + vector * num2 + vector2 + vector3 * 5f, player[i].shootColor * menuActive, 0f, Vector2.Zero, 1f * menuActive, SpriteEffects.None, 0.5f);
			num2++;
			num2++;
			while (player[i].minutes >= 60)
			{
				if (player[i].minutes >= 60)
				{
					player[i].minutes -= 60u;
					player[i].hours++;
				}
			}
			spriteBatch.DrawString(gameFont, "TIME " + player[i].hours + ":" + player[i].minutes + ":" + player[i].seconds, list[i] + vector * num2 + vector2, Color.LightCyan, 0f, Vector2.Zero, 0.6f * menuActive, SpriteEffects.None, 0.5f);
			num2++;
			spriteBatch.DrawString(gameFont, "Survived " + round + " rounds", list[i] + vector * num2 + vector2, Color.LightCyan, 0f, Vector2.Zero, 0.6f * menuActive, SpriteEffects.None, 0.5f);
			num2++;
			spriteBatch.DrawString(gameFont, "Scored " + player[i].score + " points", list[i] + vector * num2 + vector2, Color.LightCyan, 0f, Vector2.Zero, 0.6f * menuActive, SpriteEffects.None, 0.5f);
			num2++;
			spriteBatch.DrawString(gameFont, "Reached level " + player[i].level, list[i] + vector * num2 + vector2, Color.LightCyan, 0f, Vector2.Zero, 0.6f * menuActive, SpriteEffects.None, 0.5f);
			spriteBatch.End();
		}
		DrawSurvivalStatsMenu();
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		spriteBatch.DrawString(tittlesFont, "SURVIVAL", new Vector2(base.GraphicsDevice.Viewport.Width / 2, 100f * menuActive * menuActive), Color.LightCyan * menuActive, 0f, new Vector2(tittlesFont.MeasureString("SURVIVAL").X / 2f, gameFont.MeasureString("SURVIVAL").Y / 2f), 1.2f, SpriteEffects.None, 1f);
		spriteBatch.DrawString(menuFont, "Stats", new Vector2(base.GraphicsDevice.Viewport.Width / 2, 150f * menuActive * menuActive), Color.LightGray * menuActive, 0f, new Vector2(menuFont.MeasureString("Stats").X / 2f, gameFont.MeasureString("Stats").Y / 2f), 0.6f, SpriteEffects.None, 1f);
		if (gameState == GameState.SurvivalStats)
		{
			Vector2 vector4 = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
			Text.print(spriteBatch, gameFont, "(A) Select", new Vector2(vector4.X * menuActive, vector4.Y - 50f), Color.White * menuActive, txButtons);
			Text.print(spriteBatch, gameFont, "(B) Cancel/Back", new Vector2(vector4.X * menuActive, vector4.Y), Color.White * menuActive, txButtons);
		}
		spriteBatch.End();
	}

	private void drawDisclaimer()
	{
		spriteBatch.Begin();
		Vector2 vector = new Vector2(base.GraphicsDevice.Viewport.TitleSafeArea.Center.X, base.GraphicsDevice.Viewport.TitleSafeArea.Top);
		for (int i = 0; i < disclaimerText.Length; i++)
		{
			spriteBatch.DrawString(gameFont, disclaimerText[i], vector + new Vector2(0f, (i + 2) * 40), Color.LightGray * disclaimerTransp, 0f, new Vector2(gameFont.MeasureString(disclaimerText[i]).X / 2f, gameFont.MeasureString(disclaimerText[i]).Y / 2f), 0.75f, SpriteEffects.None, 1f);
		}
		spriteBatch.End();
	}

	private void DrawBonus()
	{
		spriteBatch.Begin();
		Vector2 vector = new Vector2(base.GraphicsDevice.Viewport.TitleSafeArea.Center.X, base.GraphicsDevice.Viewport.TitleSafeArea.Top);
		spriteBatch.Draw(txBlackHole, base.GraphicsDevice.Viewport.Bounds, Color.White * 0.5f);
		spriteBatch.DrawString(tittlesFont, "BONUS", vector + new Vector2(0f, 100f), Color.White, 0f, new Vector2(tittlesFont.MeasureString("BONUS").X / 2f, 0f), 0.75f, SpriteEffects.None, 1f);
		if (menuActive > 0.1f)
		{
			spriteBatch.DrawString(tittlesFont, gameStatePlay.ToString(), vector + new Vector2(0f, 200f + menuActive * 100f), Color.White * menuActive, 0f, new Vector2(tittlesFont.MeasureString(gameStatePlay.ToString()).X / 2f, 0f), 0.75f, SpriteEffects.None, 1f);
		}
		spriteBatch.End();
	}

	private void DrawBonusClear()
	{
		spriteBatch.Begin();
		Vector2 vector = new Vector2(base.GraphicsDevice.Viewport.TitleSafeArea.Center.X, base.GraphicsDevice.Viewport.TitleSafeArea.Top);
		spriteBatch.Draw(txBlackHole, base.GraphicsDevice.Viewport.Bounds, Color.White * 0.5f);
		spriteBatch.DrawString(tittlesFont, "BONUS", vector + new Vector2(0f, 100f), Color.White, 0f, new Vector2(tittlesFont.MeasureString("BONUS").X / 2f, 0f), 0.75f, SpriteEffects.None, 1f);
		string text = "Failed!";
		if (bonusClear)
		{
			text = "Clear!";
		}
		if (menuActive > 0.1f)
		{
			spriteBatch.DrawString(tittlesFont, text, vector + new Vector2(0f, 200f + menuActive * 100f), Color.White * menuActive, 0f, new Vector2(tittlesFont.MeasureString(text).X / 2f, 0f), 0.75f, SpriteEffects.None, 1f);
		}
		spriteBatch.End();
	}

	private void DrawSelectPlayer()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		selectBar.Draw(spriteBatch, 0.2f, 0.3f);
		selectBackground.Draw(spriteBatch, 1f, 0.2f);
		selectBar2.Draw(spriteBatch, 0.1f, 0.15f);
		selectHUD.Draw(spriteBatch, 1f, 0.1f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		for (int i = 0; i < 4; i++)
		{
			selectBox[i].Draw(spriteBatch, 1f);
			player[i].DrawSelect(spriteBatch, selectBox[i].position + new Vector2(selectBox[i].Width / 2, selectBox[i].Height / 2), txCharactBox, player[i].number, player[i].shootColor, frame);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		spriteBatch.DrawString(tittlesFont, "Select Character", new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) - tittlesFont.MeasureString("Select Character").X / 2f, -75f + menuActive * 100f), Color.LightCyan);
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		Text.print(spriteBatch, gameFont, "(A) Select", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
		Text.print(spriteBatch, gameFont, "(B) Cancel/Back", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		spriteBatch.End();
	}

	private void CreateEditor()
	{
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "_____________", new Vector2(0f, 59f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "_____________", new Vector2(0f, 60f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+Enemy]", new Vector2(0f, 80f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+Lens]", new Vector2(0f, 100f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+EnemyBase]", new Vector2(0f, 120f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+Block]", new Vector2(0f, 140f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+PathNode]", new Vector2(0f, 160f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+Orb]", new Vector2(0f, 180f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+Relic]", new Vector2(0f, 200f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+Blue]", new Vector2(0f, 220f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[+Message]", new Vector2(0f, 240f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "_____________", new Vector2(0f, 254f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "_____________", new Vector2(0f, 255f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "name", new Vector2(0f, 280f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "-", new Vector2(100f, 280f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "desc", new Vector2(0f, 300f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "-", new Vector2(100f, 300f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "size", new Vector2(0f, 320f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorSize = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 320f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "type", new Vector2(0f, 340f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorType = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 340f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "frame", new Vector2(0f, 360f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorFrame = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 360f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "text", new Vector2(0f, 380f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "-", new Vector2(100f, 380f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "color1", new Vector2(0f, 400f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 400f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "color2", new Vector2(0f, 420f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 420f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "color3", new Vector2(0f, 440f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 440f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "Sec/Qtt", new Vector2(0f, 460f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorSecQtt = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 460f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "Pri/Qtt", new Vector2(0f, 480f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorPriQtt = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(100f, 480f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "_____________", new Vector2(0f, 499f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "_____________", new Vector2(0f, 500f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "[SAVE]", new Vector2(0f, 0f), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "[<-]", new Vector2(0f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "[<<]", new Vector2(30f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "[O]", new Vector2(60f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: true, "[>]", new Vector2(90f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "[>>]", new Vector2(120f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "INC", new Vector2(160f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorInc = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "10", new Vector2(200f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "END", new Vector2(260f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorEnd = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "100", new Vector2(300f, base.GraphicsDevice.Viewport.Height - 20), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "CORE", new Vector2(160f, base.GraphicsDevice.Viewport.Height - 40), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorCore = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "100", new Vector2(200f, base.GraphicsDevice.Viewport.Height - 40), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIelements.Add(new UIelement(UIelements.Count, usable: true, keepSelected: false, "Astr", new Vector2(260f, base.GraphicsDevice.Viewport.Height - 40), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIeditorAstr = UIelements.Count;
		UIelements.Add(new UIelement(UIelements.Count, usable: false, keepSelected: false, "1", new Vector2(300f, base.GraphicsDevice.Viewport.Height - 40), gameFont, txWhiteBar, 0.5f, Vector2.Zero));
		UIselected = -1;
	}

	public AssetManager UpdateEditor(AssetManager am)
	{
		updateFrame = false;
		for (int i = 0; i < UIelements.Count; i++)
		{
			if (UIelements[i].Update() >= 0)
			{
				UIselected = UIelements[i].ID;
			}
			UIelements[i].selected = false;
		}
		if (currentLevel == 11)
		{
			background.texture = txPrometheus;
		}
		if (UIselected >= 0)
		{
			switch (UIelements[UIselected].text)
			{
			case "[SAVE]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
				{
					if (gameStatePlay == GameState.Challenge)
					{
						SaveChallenge();
					}
					else
					{
						SaveWorld();
					}
				}
				break;
			case "size":
				if (UIshowProperties < 0)
				{
					if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
					{
						UIelements[UIeditorSize].text = (int.Parse(UIelements[UIeditorSize].text) + 1).ToString();
					}
					if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
					{
						UIelements[UIeditorSize].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorSize].text), 0f, 999f).ToString();
					}
					break;
				}
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
				{
					am.asset[UIshowProperties].size++;
				}
				if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
				{
					am.asset[UIshowProperties].size--;
				}
				UIelements[UIeditorSize].text = am.asset[UIshowProperties].size.ToString();
				break;
			case "type":
				if (UIshowProperties < 0)
				{
					if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
					{
						UIelements[UIeditorType].text = (int.Parse(UIelements[UIeditorType].text) + 1).ToString();
					}
					if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
					{
						UIelements[UIeditorType].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorType].text) - 1, 0f, 999f).ToString();
					}
					break;
				}
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
				{
					if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
					{
						am.asset[UIshowProperties].type += 10;
					}
					else
					{
						am.asset[UIshowProperties].type++;
					}
				}
				if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
				{
					if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
					{
						am.asset[UIshowProperties].type -= 10;
					}
					else
					{
						am.asset[UIshowProperties].type--;
					}
				}
				UIelements[UIeditorType].text = am.asset[UIshowProperties].type.ToString();
				break;
			case "frame":
				if (UIshowProperties < 0)
				{
					if (currentMouseState.LeftButton == ButtonState.Pressed)
					{
						UIelements[UIeditorFrame].text = (int.Parse(UIelements[UIeditorFrame].text) + 1).ToString();
					}
					if (currentMouseState.RightButton == ButtonState.Pressed)
					{
						UIelements[UIeditorFrame].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorFrame].text) - 1, 0f, 999999f).ToString();
					}
					break;
				}
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
				{
					if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
					{
						am.asset[UIshowProperties].frame += 10u;
					}
					else
					{
						am.asset[UIshowProperties].frame++;
					}
				}
				if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
				{
					if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
					{
						am.asset[UIshowProperties].frame -= 10u;
					}
					else
					{
						am.asset[UIshowProperties].frame--;
					}
				}
				UIelements[UIeditorFrame].text = am.asset[UIshowProperties].frame.ToString();
				break;
			case "Sec/Qtt":
				if (UIshowProperties < 0)
				{
					if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
					{
						UIelements[UIeditorSecQtt].text = (int.Parse(UIelements[UIeditorSecQtt].text) + 1).ToString();
					}
					if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
					{
						UIelements[UIeditorSecQtt].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorSecQtt].text) - 1, 1f, 999f).ToString();
					}
					break;
				}
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
				{
					am.asset[UIshowProperties].numSec++;
				}
				if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
				{
					am.asset[UIshowProperties].numSec--;
				}
				UIelements[UIeditorSecQtt].text = am.asset[UIshowProperties].numSec.ToString();
				break;
			case "Pri/Qtt":
				if (UIshowProperties < 0)
				{
					if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
					{
						UIelements[UIeditorPriQtt].text = (int.Parse(UIelements[UIeditorPriQtt].text) + 1).ToString();
					}
					if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
					{
						UIelements[UIeditorPriQtt].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorPriQtt].text) - 1, 1f, 999f).ToString();
					}
					break;
				}
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
				{
					am.asset[UIshowProperties].numPri++;
				}
				if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
				{
					am.asset[UIshowProperties].numPri--;
				}
				UIelements[UIeditorPriQtt].text = am.asset[UIshowProperties].numPri.ToString();
				break;
			case "INC":
				if (UIshowProperties >= 0)
				{
					break;
				}
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
				{
					if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
					{
						UIelements[UIeditorInc].text = (int.Parse(UIelements[UIeditorInc].text) + 10).ToString();
					}
					else
					{
						UIelements[UIeditorInc].text = (int.Parse(UIelements[UIeditorInc].text) + 1).ToString();
					}
				}
				if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
				{
					if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
					{
						UIelements[UIeditorInc].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorInc].text) - 10, 0f, 999999f).ToString();
					}
					else
					{
						UIelements[UIeditorInc].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorInc].text) - 1, 0f, 999999f).ToString();
					}
				}
				break;
			case "END":
				if (UIshowProperties < 0)
				{
					if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
					{
						UIelements[UIeditorEnd].text = (int.Parse(UIelements[UIeditorEnd].text) + 1).ToString();
					}
					if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
					{
						UIelements[UIeditorEnd].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorEnd].text) - 1, 0f, 999999f).ToString();
					}
				}
				break;
			case "CORE":
				if (UIshowProperties < 0)
				{
					if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
					{
						UIelements[UIeditorCore].text = (int.Parse(UIelements[UIeditorCore].text) + 1).ToString();
					}
					if (currentMouseState != oldMouseState && currentMouseState.RightButton == ButtonState.Pressed)
					{
						UIelements[UIeditorCore].text = MathHelper.Clamp(int.Parse(UIelements[UIeditorCore].text) - 1, 0f, 999999f).ToString();
					}
				}
				break;
			case "[+Lens]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.lensFlare, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+EnemyBase]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.enemyBase, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+Asteroid]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.asteroid, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+Block]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.block, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+PathNode]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.pathNode, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+Orb]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.orb, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+Blue]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.blueMatter, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+Relic]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.relic, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+Message]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.message, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[+Enemy]":
				if (currentMouseState != oldMouseState && currentMouseState.LeftButton == ButtonState.Pressed && UIelements[UIselected].inScreen)
				{
					am.Add(LevelData.enemy, camera.get_mouse_vpos(base.GraphicsDevice), "", "", 0f, 1f, int.Parse(UIelements[UIeditorType].text), frame, "", Color.White, Color.White, Color.White, int.Parse(UIelements[UIeditorSecQtt].text), int.Parse(UIelements[UIeditorPriQtt].text));
					frame += uint.Parse(UIelements[UIeditorInc].text);
				}
				break;
			case "[<<]":
				updateFrame = true;
				frame -= 2u;
				break;
			case "[>]":
				updateFrame = true;
				break;
			case "[<-]":
				frame = 0u;
				break;
			case "[>>]":
				updateFrame = true;
				break;
			}
			if (currentKeyboardState.IsKeyDown(Keys.LeftControl) && currentKeyboardState.IsKeyDown(Keys.S))
			{
				SaveWorld();
			}
			if (UIelements[UIselected].keepSelected)
			{
				UIelements[UIselected].selected = true;
			}
			else
			{
				UIelements[UIselected].selected = false;
				UIselected = -1;
			}
		}
		UIenemSel = am.Selection(camera.get_mouse_vpos(base.GraphicsDevice));
		if (currentMouseState.RightButton == ButtonState.Pressed && currentMouseState.X > 150 && currentMouseState.Y < base.GraphicsDevice.Viewport.Height - 50)
		{
			UIselected = -1;
			UIenemSel = -1;
			UIshowProperties = -1;
		}
		if (currentMouseState.LeftButton == ButtonState.Pressed)
		{
			if (UIshowProperties < 0 || oldMouseState.LeftButton == ButtonState.Pressed || oldMouseState != currentMouseState)
			{
			}
			if (UIshowProperties < 0 || oldMouseState.LeftButton != ButtonState.Pressed)
			{
			}
			if (UIshowProperties >= 0 && oldMouseState.LeftButton == ButtonState.Pressed && currentMouseState.X > 150 && currentMouseState.Y < base.GraphicsDevice.Viewport.Height - 50)
			{
				am.asset[UIshowProperties].position = camera.get_mouse_vpos(base.GraphicsDevice);
			}
			if (UIshowProperties >= 0)
			{
				UpdateEditorInfo(am);
			}
			if (currentMouseState.LeftButton != oldMouseState.LeftButton && currentMouseState.X > 150 && currentMouseState.Y < base.GraphicsDevice.Viewport.Height - 50)
			{
				UIshowProperties = UIenemSel;
			}
		}
		if (UIshowProperties >= 0 && (currentKeyboardState.IsKeyDown(Keys.Delete) || am.asset[UIshowProperties].position.X > 1920f))
		{
			DeleteUIenem(am);
		}
		return am;
	}

	private void UpdateEditorInfo(AssetManager am)
	{
		UIelements[UIeditorSize].text = am.asset[UIshowProperties].size.ToString();
		UIelements[UIeditorType].text = am.asset[UIshowProperties].type.ToString();
		UIelements[UIeditorFrame].text = am.asset[UIshowProperties].frame.ToString();
		UIelements[UIeditorSecQtt].text = am.asset[UIshowProperties].numSec.ToString();
		UIelements[UIeditorPriQtt].text = am.asset[UIshowProperties].numPri.ToString();
	}

	private void DeleteUIenem(AssetManager am)
	{
		am.asset.RemoveAt(UIshowProperties);
		UIshowProperties = -1;
	}

	private void DrawEditor(AssetManager am)
	{
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		background.Draw(spriteBatch, 1f, 0.2001f);
		stars.Draw(spriteBatch, 10f, 0.2002f);
		stars2.Draw(spriteBatch, 10f, 0.2003f);
		hexagonsGrid.Draw(spriteBatch, 0.5f, 1f);
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		spriteBatch.End();
		spriteBatch.Begin();
		for (int i = 0; i < am.asset.Count; i++)
		{
			DrawSelection(i, am);
		}
		spriteBatch.Draw(txBlack, new Rectangle(0, 0, 150, base.GraphicsDevice.Viewport.Height), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(0, base.GraphicsDevice.Viewport.Height - 50, base.GraphicsDevice.Viewport.Width, 50), Color.White);
		if (UIshowProperties >= 0)
		{
			DrawProperties(am);
		}
		string text = "wrd: " + (int)camera.get_mouse_vpos(base.GraphicsDevice).X + "." + (int)camera.get_mouse_vpos(base.GraphicsDevice).Y + " scr: " + currentMouseState.X + "." + currentMouseState.Y;
		spriteBatch.DrawString(gameFont, text, new Vector2(base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White * 0.5f, 0f, gameFont.MeasureString(text), 0.5f, SpriteEffects.None, 0.5f);
		spriteBatch.DrawString(gameFont, "Frame: " + frame, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, base.GraphicsDevice.Viewport.Height - 30), Color.White * 0.75f, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
		if (gameState == GameState.Challenge)
		{
			spriteBatch.DrawString(gameFont, "Challenge: " + files[challengeNumber].Substring(filesChar, 12) + " - #" + challengeNumber, new Vector2(10f, 30f), Color.White * 0.75f, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
		}
		else
		{
			spriteBatch.DrawString(gameFont, "World: " + getLevel() + " - #" + currentLevel, new Vector2(10f, 30f), Color.White * 0.75f, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
		}
		for (int i = 0; i < UIelements.Count; i++)
		{
			UIelements[i].Draw(spriteBatch);
		}
		string text2 = " * * * \n";
		for (int i = 0; i < am.asset.Count; i++)
		{
			object obj = text2;
			text2 = string.Concat(obj, " - ", am.asset[i].levelData, "\n");
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		spriteBatch.End();
	}

	private void DrawSelection(int i, AssetManager am)
	{
		float num = 0.5f;
		Texture2D texture2D = txEnemyClass01;
		Rectangle value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
		Vector2 vector = new Vector2(texture2D.Width, texture2D.Height) * 0.5f;
		switch (am.asset[i].levelData)
		{
		case LevelData.enemy:
			switch (am.asset[i].type)
			{
			case 0:
				texture2D = txAsteroid;
				break;
			case 1:
				texture2D = txEnemyClass01;
				break;
			case 2:
				texture2D = txEnemyClass02;
				break;
			case 3:
				texture2D = txEnemyClass03;
				break;
			case 6:
				texture2D = txEnemyClass06;
				break;
			case 7:
				texture2D = txEnemyClass07;
				break;
			case 8:
				texture2D = txEnemyClass08;
				break;
			case 12:
				texture2D = txEnemyClass12;
				break;
			}
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			vector = new Vector2(texture2D.Width, texture2D.Height) * 0.5f;
			break;
		case LevelData.colony:
			texture2D = txColony;
			num = 1f;
			value = new Rectangle(0, 0, texture2D.Width / 5, texture2D.Height);
			vector = new Vector2(texture2D.Width / 5, texture2D.Height) * 0.5f;
			break;
		case LevelData.playerSpawn:
			texture2D = txFighter[am.asset[i].type - 1];
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.asteroid:
			texture2D = txAsteroid;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.enemyBase:
			texture2D = txColony;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.block:
			texture2D = txBlast;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.pathNode:
			texture2D = txRing;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.blueMatter:
			texture2D = txCoins;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.relic:
			texture2D = txRelicIcon;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.lensFlare:
			texture2D = txLens_glow1;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.orb:
			texture2D = txOrbs;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		case LevelData.message:
			texture2D = txBar;
			value = new Rectangle(0, 0, texture2D.Width, texture2D.Height);
			break;
		}
		spriteBatch.Draw(texture2D, camera.getScreenPosition(am.asset[i].position, base.GraphicsDevice), value, Color.White, 0f, new Vector2(value.Width, value.Height) * 0.5f, editorZoom * num, SpriteEffects.None, (float)i / 1000f);
		if (am.isMouseOver(i, camera.get_mouse_vpos(base.GraphicsDevice)))
		{
			spriteBatch.DrawString(gameFont, "| |", camera.getScreenPosition(am.asset[i].position, base.GraphicsDevice), Color.LightCyan, 0f, gameFont.MeasureString("| |") * 0.5f, (float)(texture2D.Width / 60) * editorZoom, SpriteEffects.None, 1f);
			spriteBatch.DrawString(gameFont, "| |", camera.getScreenPosition(am.asset[i].position, base.GraphicsDevice), Color.LightCyan, (float)Math.PI / 2f, gameFont.MeasureString("| |") * 0.5f, (float)(texture2D.Height / 60) * editorZoom, SpriteEffects.None, 1f);
		}
	}

	private void DrawProperties(AssetManager am)
	{
		spriteBatch.DrawString(gameFont, "O", camera.getScreenPosition(am.asset[UIshowProperties].position, base.GraphicsDevice), Color.LightCyan, 0f, gameFont.MeasureString("O") * 0.5f, 0.6f + editorZoom, SpriteEffects.None, 0.5f);
		spriteBatch.DrawString(gameFont, string.Concat(am.asset[UIshowProperties].levelData), new Vector2(0f, 520f), Color.LightCyan, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.5f);
	}

	private void DrawMessageInfo()
	{
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		Vector2 vector = new Vector2(1f);
		float num = messageInfoCounter / 100f;
		float num2 = 1f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		Vector2 zero = Vector2.Zero;
		string text = " Press (A) or Enter to continue";
		text = "Press (A) to continue";
		spriteBatch.Draw(txMessage, MessageInfoPos - new Vector2((float)txMessage.Width * 0.5f, (float)txMessage.Height * 0.5f), Color.White);
		for (int i = 0; i < messageInfo[0].Length; i++)
		{
			if (messageInfo[0][i] != null)
			{
				num5 += gameFont.MeasureString(messageInfo[0][i]).Y;
			}
		}
		for (int i = 0; i < messageInfo[0].Length; i++)
		{
			if (messageInfo[0][i] != null)
			{
				Text.print(spriteBatch, gameFont, messageInfo[0][i], MessageInfoPos + new Vector2(0f, i * 20) - new Vector2(0f, (int)num5 / 8) - new Vector2(gameFont.MeasureString(messageInfo[0][i]).X / 2f, gameFont.MeasureString(messageInfo[0][i]).Y / 2f), new Color(num * 0.65f, num * 0.9f, num, num) * (num2 * 0.5f + 0.25f), txButtons);
				zero = gameFont.MeasureString(messageInfo[0][i]) * vector * new Vector2(0.11f, 0.2f);
				if (zero.X > num3)
				{
					num3 = zero.X;
				}
				num4 += zero.Y;
			}
		}
		num4 /= 4f;
		if (num4 > (float)(txMessage.Height / 4))
		{
			num4 = txMessage.Height / 4;
		}
		zero = new Vector2(num3, num4);
		spriteBatch.Draw(txBlack, MessageInfoPos, null, new Color(num, num, num, num) * 0.5f * num2, 0f, new Vector2((float)txBlack.Width / 2f, (float)txBlack.Height / 2f), zero, SpriteEffects.None, 0.1f);
		Text.print(spriteBatch, gameFont, text, MessageInfoPos + new Vector2(txMessage.Width, txMessage.Height) / 2f - (gameFont.MeasureString(text) + Vector2.One * 30f), new Color(num * 0.65f, num * 0.9f, num, num) * (num2 * 0.5f + 0.25f), txButtons);
		spriteBatch.End();
		DrawMouseUI();
	}

	private void DrawVSmode()
	{
		base.GraphicsDevice.Clear(level[currentLevel].color);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		background.Draw(spriteBatch, 1f, 0.2001f);
		stars.Draw(spriteBatch, 10f, 0.2002f);
		stars2.Draw(spriteBatch, 10f, 0.2003f);
		colony.Draw(spriteBatch);
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		DrawItems();
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type != constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
		}
		for (int i = 0; i < player.Length; i++)
		{
			player[i].Draw(spriteBatch, colony.position, GOHUDopacity / 50f);
		}
		if (currentLevel == 14)
		{
			hexagonsGrid.Draw(spriteBatch, GOHUDopacity / 50f, 1f);
		}
		else
		{
			hexagonsGrid.Draw(spriteBatch, GOHUDopacity / 1000f, 1f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type == constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < bullets.Count; i++)
		{
			bullets[i].Draw(spriteBatch, 2);
		}
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Draw(spriteBatch, 1);
		}
		particleSystem.DrawTrails(spriteBatch);
		particleSystem.Draw(spriteBatch);
		DrawFXs(spriteBatch);
		DrawBlast();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
		if (!editor)
		{
			DrawHUD();
			DrawColonyHUD();
		}
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, Matrix.Identity);
		if (!editor)
		{
			DrawColonyBars();
		}
		particleSystem.DrawItemTrails(spriteBatch);
		DrawLens();
		spriteBatch.End();
	}

	private void DrawCampaign()
	{
		base.GraphicsDevice.Clear(level[currentLevel].color);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		background.Draw(spriteBatch, 1f, 0.2001f);
		stars.Draw(spriteBatch, 10f, 0.2002f);
		stars2.Draw(spriteBatch, 10f, 0.2003f);
		colony.Draw(spriteBatch);
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		DrawItems();
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type != constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
		}
		for (int i = 0; i < player.Length; i++)
		{
			player[i].Draw(spriteBatch, colony.position, GOHUDopacity);
		}
		if (currentLevel == 14)
		{
			hexagonsGrid.Draw(spriteBatch, GOHUDopacity / 50f, 1f);
		}
		else
		{
			hexagonsGrid.Draw(spriteBatch, GOHUDopacity / 1000f, 1f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type == constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < bullets.Count; i++)
		{
			bullets[i].Draw(spriteBatch, 2);
		}
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Draw(spriteBatch, 1);
		}
		particleSystem.DrawTrails(spriteBatch);
		particleSystem.Draw(spriteBatch);
		DrawFXs(spriteBatch);
		DrawBlast();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
		if (!editor)
		{
			DrawHUD();
			DrawColonyHUD();
		}
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, Matrix.Identity);
		if (!editor)
		{
			DrawColonyBars();
		}
		particleSystem.DrawItemTrails(spriteBatch);
		DrawLens();
		spriteBatch.End();
	}

	private void DrawChallenge()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		stars.Draw(spriteBatch, 10f, 0.2002f);
		stars2.Draw(spriteBatch, 10f, 0.2003f);
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		DrawItems();
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type != constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw(spriteBatch, playerChallenge.Active, playerChallenge.position, playerChallenge.Active, playerChallenge.position, playerChallenge.Active, playerChallenge.position, playerChallenge.Active, playerChallenge.position, GOHUDopacity / 100f);
		}
		playerChallenge.Draw(spriteBatch, Vector2.Zero, 0f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type == constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < bullets.Count; i++)
		{
			bullets[i].Draw(spriteBatch, 2);
		}
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Draw(spriteBatch, 1);
		}
		particleSystem.DrawTrails(spriteBatch);
		particleSystem.Draw(spriteBatch);
		DrawFXs(spriteBatch);
		DrawBlast();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.DrawString(menuFont, coins.Count + " remaining", new Vector2(base.GraphicsDevice.Viewport.Width / 2, 100f), Color.LightCyan, 0f, menuFont.MeasureString(coins.Count + " remaining") / 2f, 1f, SpriteEffects.None, 0f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, Matrix.Identity);
		particleSystem.DrawItemTrails(spriteBatch);
		spriteBatch.End();
	}

	private void DrawItems()
	{
		string text = "";
		for (int i = 0; i < pickups.Count; i++)
		{
			pickups[i].Draw(spriteBatch);
			if (currentLevel != 0 || gameState != GameState.Campaign)
			{
				continue;
			}
			switch (pickups[i].pickupType)
			{
			case Pickup.item.orb:
				if (colony.energy < colony.maximunEnergy / 2f)
				{
					spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f), Color.White, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0f);
					spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f) - Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
					spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f) + Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
				}
				break;
			case Pickup.item.health:
				if (round <= 6)
				{
					spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f), Color.White, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0f);
					spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f) - Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
					spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f) + Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
				}
				break;
			case Pickup.item.relic:
				spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f), Color.White, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0f);
				spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f) - Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
				spriteBatch.DrawString(menuFont, text, pickups[i].position - new Vector2(0f, 30f) + Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
				break;
			}
		}
		if (currentLevel == 0 && gameState == GameState.Campaign && construction.Count <= 0 && round <= 5 && frame < 500)
		{
			for (int j = 0; j < player.Length; j++)
			{
				if (player[j].Active)
				{
					spriteBatch.DrawString(menuFont, text, player[j].position + new Vector2(0f, 50f), Color.White, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0f);
					spriteBatch.DrawString(menuFont, text, player[j].position + new Vector2(0f, 50f) - Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
					spriteBatch.DrawString(menuFont, text, player[j].position + new Vector2(0f, 50f) + Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
					spriteBatch.DrawString(menuFont, text, player[j].position - new Vector2(0f, 50f), Color.White, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0f);
					spriteBatch.DrawString(menuFont, text, player[j].position - new Vector2(0f, 50f) - Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
					spriteBatch.DrawString(menuFont, text, player[j].position - new Vector2(0f, 50f) + Vector2.One, Color.Black, 0f, menuFont.MeasureString(text) * 0.5f, 0.5f, SpriteEffects.None, 0.1f);
				}
			}
		}
		for (int i = 0; i < coins.Count; i++)
		{
			coins[i].Draw(spriteBatch);
		}
	}

	private void DrawFinalBoss()
	{
		base.GraphicsDevice.Clear(level[currentLevel].color);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		colony.Draw(spriteBatch);
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		if (!editor)
		{
			DrawItems();
			for (int i = 0; i < construction.Count; i++)
			{
				if (construction[i].type != constructionType.barrier)
				{
					construction[i].Draw(spriteBatch);
				}
			}
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
			}
			for (int i = 0; i < player.Length; i++)
			{
				player[i].Draw(spriteBatch, colony.position, GOHUDopacity / 50f);
			}
		}
		spriteBatch.Draw(txBossCore, new Vector2(640f, 400f), null, Color.White * (1f + (float)Math.Cos((float)bossFrame / 93f) * 0.01f), (float)bossFrame / -399f, new Vector2(txBossCore.Width / 2, txBossCore.Height / 2), 0.9f, SpriteEffects.None, 0.57f);
		spriteBatch.Draw(txBossCore, new Vector2(640f, 400f), null, Color.White * 0.3f, (float)bossFrame / 33f, new Vector2(txBossCore.Width / 2, txBossCore.Height / 2), 1.2f * (1f + (float)Math.Sin((float)bossFrame / 78f) * 0.2f), SpriteEffects.None, 0.56f);
		spriteBatch.Draw(txBossSpikes3, new Vector2(640f, 400f), null, Color.White, (float)bossFrame / -193f, new Vector2((float)(txBossSpikes1.Width / 2) * (1f + (float)Math.Sin((float)bossFrame / 73f) * 0.01f), (float)(txBossSpikes1.Height / 2) * (1f + (float)Math.Sin((float)bossFrame / 57f) * 0.01f)), 1f + (float)Math.Sin((float)bossFrame / 110f) * 0.1f, SpriteEffects.None, 0.55f);
		spriteBatch.Draw(txBossSpikes4, new Vector2(640f, 400f), null, Color.White, (float)bossFrame / 353f, new Vector2(txBossSpikes1.Width / 2, txBossSpikes1.Height / 2), 1.2f, SpriteEffects.None, 0.55f);
		spriteBatch.Draw(txBossSpikes4, new Vector2(640f, 400f), null, Color.White, (float)bossFrame / 111f, new Vector2(txBossSpikes1.Width / 2, txBossSpikes1.Height / 2), 1.4f, SpriteEffects.None, 0.55f);
		spriteBatch.Draw(txBossSpikes3, new Vector2(640f, 400f), null, Color.White, (float)bossFrame / -54f, new Vector2(txBossSpikes1.Width / 2, txBossSpikes1.Height / 2), 1.4f, SpriteEffects.None, 0.54f);
		spriteBatch.Draw(txBossSpikes2, new Vector2(640f, 400f), null, Color.White, (float)bossFrame / 266f, new Vector2(txBossSpikes1.Width / 2, txBossSpikes1.Height / 2), 1.4f, SpriteEffects.None, 0.53f);
		spriteBatch.Draw(txBossSpikes1, new Vector2(640f, 400f), null, Color.White, (float)bossFrame / -157f, new Vector2(txBossSpikes1.Width / 2, txBossSpikes1.Height / 2), 1.4f, SpriteEffects.None, 0.52f);
		spriteBatch.Draw(txBossBase, new Vector2(640f, 400f), null, Color.White, (float)(0L - (long)bossFrame) / 591f * (1f + (float)Math.Cos((float)bossFrame / 113f) * 0.01f), new Vector2(txBossBase.Width / 2, txBossBase.Height / 2), 1.45f, SpriteEffects.None, 0.51f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		if (!editor)
		{
			for (int i = 0; i < construction.Count; i++)
			{
				if (construction[i].type == constructionType.barrier)
				{
					construction[i].Draw(spriteBatch);
				}
			}
			for (int i = 0; i < bullets.Count; i++)
			{
				bullets[i].Draw(spriteBatch, 2);
			}
			for (int i = 0; i < enemyBullets.Count; i++)
			{
				enemyBullets[i].Draw(spriteBatch, 1);
			}
			particleSystem.DrawTrails(spriteBatch);
			particleSystem.Draw(spriteBatch);
			DrawFXs(spriteBatch);
			DrawBlast();
			spriteBatch.Draw(txBossGlow, new Vector2(640f, 400f), null, Color.White * 0.5f, 0f, new Vector2(txBossGlow.Width / 2, txBossGlow.Height / 2), 2f, SpriteEffects.None, 0.5f);
			spriteBatch.Draw(txBossGlow, new Vector2(640f, 400f), null, Color.White * 0.5f, 0f, new Vector2(txBossGlow.Width / 2, txBossGlow.Height / 2), 10f, SpriteEffects.None, 1f);
			if (enemies.Count > 0 && enemies[0].jump > 65)
			{
				enemies[0].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
			}
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
		if (!editor)
		{
			DrawHUD();
			DrawColonyHUD();
		}
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, Matrix.Identity);
		if (!editor)
		{
			if (enemies.Count > 0)
			{
				enemies[0].DrawNodBar(spriteBatch, base.GraphicsDevice, GOHUDopacity, whitePixel, txColonyCORE);
			}
			DrawColonyHealth();
		}
		particleSystem.DrawItemTrails(spriteBatch);
		DrawLens();
		spriteBatch.End();
	}

	private void DrawSurvival()
	{
		base.GraphicsDevice.Clear(new Color(0.1f, 0.1f, 0.1f));
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		stars.Draw(spriteBatch, 10f, 0.2002f);
		stars2.Draw(spriteBatch, 10f, 0.2003f);
		hexagonsGrid.Draw(spriteBatch, GOHUDopacity / 500f);
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		DrawItems();
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type != constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
		}
		for (int i = 0; i < 4; i++)
		{
			player[i].Draw(spriteBatch, new Vector2(-10000f), GOHUDopacity / 50f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type == constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < bullets.Count; i++)
		{
			bullets[i].Draw(spriteBatch, 2);
		}
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Draw(spriteBatch, 1);
		}
		particleSystem.DrawTrails(spriteBatch);
		particleSystem.Draw(spriteBatch);
		DrawFXs(spriteBatch);
		DrawBlast();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
		DrawLens();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		DrawHUD();
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.End();
	}

	private void DrawMeteroids()
	{
		base.GraphicsDevice.Clear(new Color(0.1f, 0.1f, 0.1f));
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		stars.Draw(spriteBatch, 10f, 0.2002f);
		stars2.Draw(spriteBatch, 10f, 0.2003f);
		hexagonsGrid.Draw(spriteBatch, GOHUDopacity / 500f);
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		DrawItems();
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type != constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
		}
		for (int i = 0; i < 4; i++)
		{
			player[i].Draw(spriteBatch, new Vector2(-10000f), GOHUDopacity / 50f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type == constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < bullets.Count; i++)
		{
			bullets[i].Draw(spriteBatch, 2);
		}
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Draw(spriteBatch, 1);
		}
		particleSystem.DrawTrails(spriteBatch);
		particleSystem.Draw(spriteBatch);
		DrawFXs(spriteBatch);
		DrawBlast();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
		DrawLens();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		DrawHUD();
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.End();
	}

	private void DrawChubbyRain()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		spriteBatch.Draw(txGeaMoon, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		background.DrawScrollY(spriteBatch, 1f);
		stars.DrawScrollY(spriteBatch, 1f);
		stars2.DrawScrollY(spriteBatch, 1f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type == constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < bullets.Count; i++)
		{
			bullets[i].Draw(spriteBatch, 2);
		}
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Draw(spriteBatch, 1);
		}
		particleSystem.DrawTrails(spriteBatch);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		DrawItems();
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type != constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
		}
		for (int i = 0; i < 4; i++)
		{
			player[i].Draw(spriteBatch, Vector2.Zero, GOHUDopacity / 50f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		particleSystem.Draw(spriteBatch);
		DrawFXs(spriteBatch);
		DrawBlast();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		draw2d.DrawPixel(spriteBatch, new Rectangle(0, 0, (int)((float)base.GraphicsDevice.Viewport.Width * 0.2f), base.GraphicsDevice.Viewport.Height), Color.Black);
		draw2d.DrawPixel(spriteBatch, new Rectangle((int)((float)base.GraphicsDevice.Viewport.Width * 0.8f), 0, (int)((float)base.GraphicsDevice.Viewport.Width * 0.2f), base.GraphicsDevice.Viewport.Height), Color.Black);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		DrawHUD(lines: false);
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.DrawString(menuFont, "Round " + round, new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) - menuFont.MeasureString("Round " + round).X / 2f, menuFont.MeasureString("Round " + round).Y + 10f), Color.LightCyan);
		spriteBatch.End();
	}

	private void DrawSidescroller()
	{
		base.GraphicsDevice.Clear(Color.DarkSlateBlue * 0.2f);
		spriteBatch.Begin();
		spriteBatch.Draw(txSidescrollerBackground, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		stars.DrawScrollX(spriteBatch, 1f);
		stars2.DrawScrollX(spriteBatch, 1f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		for (int i = 0; i < sidescrollerB.Length; i++)
		{
			sidescrollerB[i].Draw(spriteBatch, 1f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		border.Draw(spriteBatch, 1f);
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type == constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < bullets.Count; i++)
		{
			bullets[i].Draw(spriteBatch, 2);
		}
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Draw(spriteBatch, 1);
		}
		particleSystem.DrawTrails(spriteBatch);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		DrawItems();
		for (int i = 0; i < construction.Count; i++)
		{
			if (construction[i].type != constructionType.barrier)
			{
				construction[i].Draw(spriteBatch);
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].Draw(spriteBatch, player[0].Active, player[0].position, player[1].Active, player[1].position, player[2].Active, player[2].position, player[3].Active, player[3].position, GOHUDopacity / 100f);
		}
		for (int i = 0; i < 4; i++)
		{
			player[i].Draw(spriteBatch, new Vector2(-10000f), GOHUDopacity / 50f);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		particleSystem.Draw(spriteBatch);
		DrawFXs(spriteBatch);
		DrawBlast();
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		spriteBatch.Draw(txBlack, new Rectangle(-640, 1080, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-640, -5160, 2560, 4800), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(-5640, -5160, 5000, 10400), Color.White);
		spriteBatch.Draw(txBlack, new Rectangle(1920, -5160, 5000, 10400), Color.White);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
		DrawHUD(lines: false);
		for (int i = 0; i < messages.Count; i++)
		{
			messages[i].Draw(spriteBatch, GOHUDopacity / 100f);
		}
		spriteBatch.DrawString(menuFont, "Round " + round, new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) - menuFont.MeasureString("Round " + round).X / 2f, menuFont.MeasureString("Round " + round).Y + 10f), Color.LightCyan);
		spriteBatch.End();
	}

	private void UpdateMouse()
	{
		if (mouseTimer > 0f)
		{
			mouseTimer--;
		}
		if (mouseTimer <= 0f)
		{
			mouseTranspTarget = 0f;
		}
		else
		{
			mouseTranspTarget = 1f;
		}
		if (mouseTransp < mouseTranspTarget)
		{
			mouseTransp = MathHelper.Lerp(mouseTransp, mouseTranspTarget, 0.2f);
		}
		else
		{
			mouseTransp = MathHelper.Lerp(mouseTransp, mouseTranspTarget, 0.05f);
		}
		if (player.Length > 0)
		{
			keyboardPlayer = MathHelper.Clamp(keyboardPlayer, 1f, 4f);
			Vector2 vector = camera.get_mouse_vpos(base.GraphicsDevice) - player[(int)keyboardPlayer - 1].position;
			mouseAngle = (float)Math.Atan2(vector.Y, vector.X);
			mousePos = camera.get_mouse_vpos(base.GraphicsDevice);
			mouseSize = 1f;
		}
	}

	private void UpdateMouseUI()
	{
		UpdateMouse();
		mousePos = new Vector2(currentMouseState.X, currentMouseState.Y);
		mouseAngle = 3.926991f;
		mouseSize = 0.5f;
		ResetVibration();
	}

	private void DrawMouse()
	{
		if (gameState != GameState.pause && gameState != GameState.message && gameState != GameState.score)
		{
			spriteBatch.Draw(txTarget, mousePos, null, new Color(mouseTransp, mouseTransp, mouseTransp, mouseTransp - 0.1f), mouseAngle, new Vector2(txTarget.Width / 2, txTarget.Height / 2), mouseSize, SpriteEffects.None, 0f);
		}
	}

	private void DrawMouseUI()
	{
		if (mouseTransp > 0f)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(txTarget, mousePos, null, new Color(mouseTransp, mouseTransp, mouseTransp, mouseTransp - 0.1f), mouseAngle, new Vector2(txTarget.Width / 2, txTarget.Height / 2), mouseSize, SpriteEffects.None, 0f);
			spriteBatch.End();
		}
	}

	private void DrawLens()
	{
		DrawLens(currentLevel);
	}

	private void DrawLens(int level)
	{
		for (int i = 0; i < lens[level].Count; i++)
		{
			lens[level][i].Draw(spriteBatch);
		}
	}

	private void DrawColonyHUD()
	{
		Color color = new Color(GOHUDopacity / 100f, GOHUDopacity / 100f, GOHUDopacity / 100f, GOHUDopacity / 100f);
		Vector2 position = new Vector2(base.GraphicsDevice.Viewport.Width / 2, 40f);
		Vector2 position2 = new Vector2(base.GraphicsDevice.Viewport.Width / 2 - 4, base.GraphicsDevice.Viewport.Height - 38);
		spriteBatch.Draw(txColonyHUD, position, null, color, 0f, new Vector2((float)txColonyHUD.Width / 2f, 0f), 1f, SpriteEffects.None, 0.5f);
		if (gameState == GameState.finalBoss)
		{
			spriteBatch.Draw(txBossLife, position2, null, color, 0f, new Vector2((float)txColonyCORE.Width / 2f, txColonyCORE.Height), 1f, SpriteEffects.None, 0.5f);
		}
		else
		{
			spriteBatch.Draw(txColonyCORE, position2, null, color, 0f, new Vector2((float)txColonyCORE.Width / 2f, txColonyCORE.Height), 1f, SpriteEffects.None, 0.5f);
		}
	}

	private void DrawColonyBars()
	{
		float value = 1f;
		value = MathHelper.Clamp(value, 0f, 1.2f - colony.growH / 5f);
		Color color = new Color(0.101960786f * colony.growH, 16f / 85f * value, value) * (GOHUDopacity / 50f);
		Color color2 = new Color(0.101960786f, 16f / 85f, 1f) * (GOHUDopacity / 50f) * colony.growE;
		Vector2 vector = new Vector2(base.GraphicsDevice.Viewport.Width / 2 - txColonyHUD.Width / 2 + 235, 65f);
		Vector2 vector2 = new Vector2(base.GraphicsDevice.Viewport.Width / 2 - txColonyCORE.Width / 2 + 235, base.GraphicsDevice.Viewport.Height - txColonyCORE.Height - 40 + 25);
		Rectangle destinationRectangle = new Rectangle((int)vector.X, (int)vector.Y, (int)(colony.health / colony.MaximunHealth * 550f), 18);
		Rectangle destinationRectangle2 = new Rectangle((int)vector2.X, (int)vector2.Y, (int)(colony.energy / colony.maximunEnergy * 550f), 18);
		spriteBatch.Draw(whitePixel, destinationRectangle, color);
		if (gameState == GameState.finalBoss)
		{
			spriteBatch.Draw(whitePixel, destinationRectangle2, Color.Red);
		}
		else
		{
			spriteBatch.Draw(whitePixel, destinationRectangle2, color2);
		}
	}

	private void DrawColonyHealth()
	{
		float value = 1f;
		value = MathHelper.Clamp(value, 0f, 1.2f - colony.growH / 5f);
		Color color = new Color(0.101960786f * colony.growH, 16f / 85f * value, value) * (GOHUDopacity / 50f);
		Vector2 vector = new Vector2(base.GraphicsDevice.Viewport.Width / 2 - txColonyHUD.Width / 2 + 235, 65f);
		Rectangle destinationRectangle = new Rectangle((int)vector.X, (int)vector.Y, (int)(colony.health / colony.MaximunHealth * 550f), 18);
		spriteBatch.Draw(whitePixel, destinationRectangle, color);
	}

	private void DrawHUD()
	{
		DrawHUD(lines: true);
	}

	private void DrawHUD(bool lines)
	{
		GameState gameState = gameStatePlay;
		if (gameState == GameState.Challenge)
		{
			playerChallenge.DrawUI(spriteBatch, base.GraphicsDevice.Viewport.Width, GOHUDopacity / 100f);
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				player[i].DrawUI(spriteBatch, base.GraphicsDevice.Viewport.Width, GOHUDopacity / 100f);
			}
		}
		if (lines)
		{
			spriteBatch.Draw(txHUD, Vector2.Zero, null, new Color(GOHUDopacity / 100f, GOHUDopacity / 100f, GOHUDopacity / 100f, GOHUDopacity / 100f), 0f, Vector2.Zero, new Vector2((float)base.GraphicsDevice.Viewport.Width / (float)txHUD.Width, (float)base.GraphicsDevice.Viewport.Height / (float)txHUD.Height), SpriteEffects.None, 1f);
		}
		if (showData)
		{
			spriteBatch.DrawString(gameFont, "X, Y: " + player[0].position.X + " , " + player[0].position.Y, new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height - 200), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
			spriteBatch.DrawString(gameFont, "Pad: " + gamepadinfo.GamePadType, new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height - 300), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
			spriteBatch.DrawString(gameFont, "H: " + Convert.ToString(gamepadinfo.GetHashCode()), new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height - 350), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
			spriteBatch.DrawString(gameFont, "T: " + Convert.ToString(gamepadinfo.GetType()), new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height - 250), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
			PresentationParameters presentationParameters = new PresentationParameters();
			DisplayModeCollection supportedDisplayModes = graphics.GraphicsDevice.Adapter.SupportedDisplayModes;
		}
		if (cheating || showData)
		{
			spriteBatch.DrawString(menuFont, "FPS:   " + currentFrameRate, new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height - 100), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
		}
		string text = "Time " + $"{minutes:00}" + ":" + $"{seconds:00}";
		if (Game1.gameState == GameState.Survival)
		{
			Vector2 vector = new Vector2(base.GraphicsDevice.Viewport.Width / 2, (float)base.GraphicsDevice.Viewport.TitleSafeArea.Top + menuFont.MeasureString(text).Y);
			spriteBatch.DrawString(menuFont, text, vector + new Vector2(2f, 2f), Color.Black * GOHUDopacity * 1.25f, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
			spriteBatch.DrawString(menuFont, text, vector + new Vector2(-2f, -2f), Color.Black * GOHUDopacity * 1.25f, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
			spriteBatch.DrawString(menuFont, text, vector + new Vector2(0f, 2f), Color.Black * GOHUDopacity * 1.25f, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
			spriteBatch.DrawString(menuFont, text, vector + new Vector2(0f, -2f), Color.Black * GOHUDopacity * 1.25f, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
			spriteBatch.DrawString(menuFont, text, vector + new Vector2(2f, 0f), Color.Black * GOHUDopacity * 1.25f, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
			spriteBatch.DrawString(menuFont, text, vector + new Vector2(-2f, 0f), Color.Black * GOHUDopacity * 1.25f, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
			spriteBatch.DrawString(menuFont, text, vector, Color.LightCyan * GOHUDopacity * 1.25f, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 0.5f);
		}
	}

	public static List<DisplayMode> GetDisplayModes(GraphicsDevice GraphicsDevice)
	{
		List<DisplayMode> list = new List<DisplayMode>(500);
		GOresolutionIndex = 0;
		int num = 0;
		foreach (DisplayMode supportedDisplayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
		{
			try
			{
				if (supportedDisplayMode.Width == GraphicsDevice.Viewport.Width && supportedDisplayMode.Height == GraphicsDevice.Viewport.Height)
				{
					GOresolutionIndex = num;
				}
			}
			catch
			{
			}
			list.Add(supportedDisplayMode);
			num++;
		}
		return list;
	}

	private void DrawMenu()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		menuBackground.Draw(spriteBatch, 1f);
		menuBackground.size = new Vector2((float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height, (float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height);
		Rectangle destinationRectangle = new Rectangle((int)((float)base.GraphicsDevice.Viewport.Width * (0.5f - menuActive * 0.3f)), 0, (int)((float)base.GraphicsDevice.Viewport.Width * (menuActive * 0.6f)), base.GraphicsDevice.Viewport.Height);
		spriteBatch.Draw(txBlack, destinationRectangle, Color.Black * menuActive * 0.5f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		spriteBatch.Draw(txMenuFront, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White * menuActive);
		if (gameState == GameState.mainMenu)
		{
			for (int i = 0; i < mainMenu.Count; i++)
			{
				mainMenu[i].Draw(spriteBatch, base.GraphicsDevice);
			}
		}
		Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
		if (gameState == GameState.mainMenu)
		{
			Text.print(spriteBatch, gameFont, "(A) Select", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
			Text.print(spriteBatch, gameFont, "(B) Cancel/Back", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		}
		if (Guide.IsTrialMode)
		{
			Text.print(spriteBatch, gameFont, "Press (Y) to buy the game", new Vector2(vector.X * menuActive, vector.Y - 100f), Color.White * menuActive, txButtons);
		}
		spriteBatch.End();
	}

	private void DrawOptionsMenu()
	{
		spriteBatch.Begin();
		menuBackground.Draw(spriteBatch, 1f);
		menuBackground.size = new Vector2((float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height, (float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height);
		Rectangle destinationRectangle = new Rectangle((int)((float)base.GraphicsDevice.Viewport.Width * (0.5f - menuActive * 0.3f)), 0, (int)((float)base.GraphicsDevice.Viewport.Width * (menuActive * 0.6f)), base.GraphicsDevice.Viewport.Height);
		spriteBatch.Draw(txBlack, destinationRectangle, Color.Black * menuActive * 0.5f);
		spriteBatch.Draw(txMenuFront, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White * menuActive);
		if (gameState == GameState.optionsMenu)
		{
			for (int i = 0; i < optionsMenu.Count; i++)
			{
				optionsMenu[i].Draw(spriteBatch, base.GraphicsDevice);
			}
			Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
			Text.print(spriteBatch, gameFont, "(A) Select", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
			Text.print(spriteBatch, gameFont, "(B) Cancel/Back", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
		}
		spriteBatch.DrawString(tittlesFont, "Options", new Vector2(menuActive * menuActive * ((float)base.GraphicsDevice.Viewport.Width * 0.5f), (float)base.GraphicsDevice.Viewport.Height * 0.2f), Color.LightCyan * menuActive, 0f, tittlesFont.MeasureString("Options") * 0.5f, 1f, SpriteEffects.None, 0f);
		spriteBatch.End();
	}

	private void DrawPlayMenu()
	{
		spriteBatch.Begin();
		menuBackground.Draw(spriteBatch, 1f);
		menuBackground.size = new Vector2((float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height, (float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height);
		Rectangle destinationRectangle = new Rectangle((int)((float)base.GraphicsDevice.Viewport.Width * (0.5f - menuActive * 0.3f)), 0, (int)((float)base.GraphicsDevice.Viewport.Width * (menuActive * 0.6f)), base.GraphicsDevice.Viewport.Height);
		spriteBatch.Draw(txBlack, destinationRectangle, Color.Black * menuActive * 0.5f);
		spriteBatch.Draw(txMenuFront, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White * menuActive);
		if (gameState == GameState.playMenu)
		{
			Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
			Text.print(spriteBatch, gameFont, "(A) Select", new Vector2(vector.X * menuActive, vector.Y - 50f), Color.White * menuActive, txButtons);
			Text.print(spriteBatch, gameFont, "(B) Cancel/Back", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
			for (int i = 0; i < playMenu.Count; i++)
			{
				playMenu[i].Draw(spriteBatch, base.GraphicsDevice);
			}
		}
		spriteBatch.DrawString(tittlesFont, "Play Mode", new Vector2(menuActive * menuActive * ((float)base.GraphicsDevice.Viewport.Width * 0.5f), (float)base.GraphicsDevice.Viewport.Height * 0.2f), Color.LightCyan * menuActive, 0f, tittlesFont.MeasureString("Play Mode") * 0.5f, 1f, SpriteEffects.None, 0f);
		spriteBatch.End();
	}

	private void DrawSurvivalStatsMenu()
	{
		spriteBatch.Begin();
		Rectangle destinationRectangle = new Rectangle((int)((float)base.GraphicsDevice.Viewport.Width * 0.5f - (float)base.GraphicsDevice.Viewport.Width * 0.249f * menuActive * 0.5f), 0, (int)((float)base.GraphicsDevice.Viewport.Width * 0.249f * menuActive), base.GraphicsDevice.Viewport.Height);
		spriteBatch.Draw(txBlack, destinationRectangle, Color.Black * menuActive * 0.5f);
		for (int i = 0; i < survivalStatsMenu.Count; i++)
		{
			survivalStatsMenu[i].Draw(spriteBatch, base.GraphicsDevice);
		}
		spriteBatch.End();
	}

	private void DrawPauseMenu()
	{
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		spriteBatch.Draw(txBlack, new Vector2(0f, 0f), null, new Color(0f, 0f, 0f, (pausePercent + 50f) / 150f * 0.5f), 0f, new Vector2(0f, 0f), new Vector2(base.GraphicsDevice.Viewport.Width / txBlack.Width, base.GraphicsDevice.Viewport.Height / txBlack.Height), SpriteEffects.None, 0f);
		spriteBatch.Draw(txMenuFront, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), Color.White * menuActive);
		spriteBatch.DrawString(menuFont, "PAUSE", new Vector2(base.GraphicsDevice.Viewport.Width / 2, pausePercent), Color.LightCyan, 0f, new Vector2(gameFont.MeasureString("PAUSE").X / 2f, gameFont.MeasureString("A").Y / 2f), 1f, SpriteEffects.None, 1f);
		spriteBatch.End();
		spriteBatch.Begin();
		if (pausePercent >= 99f)
		{
			for (int i = 0; i < pauseMenu.Count; i++)
			{
				pauseMenu[i].Draw(spriteBatch, base.GraphicsDevice);
			}
		}
		spriteBatch.End();
		DrawMouseUI();
	}

	private void DrawStartScreen()
	{
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		spriteBatch.Draw(txBlack, new Vector2(0f, 0f), null, new Color(0f, 0f, 0f, 0.75f), 0f, new Vector2(0f, 0f), new Vector2(base.GraphicsDevice.Viewport.Width / txBlack.Width, base.GraphicsDevice.Viewport.Height / txBlack.Height), SpriteEffects.None, 0f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
		spriteBatch.Draw(txTittleOff, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), new Color(0.9f + (float)Math.Cos((float)frame / 33f) * 0.1f, 0.9f + (float)Math.Cos((float)frame / 33f) * 0.1f, 0.9f + (float)Math.Cos((float)frame / 33f) * 0.1f, 0.9f + (float)Math.Cos((float)frame / 33f) * 0.1f));
		spriteBatch.Draw(txTittleOn, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), new Color(0.5f + (float)Math.Sin((float)frame / 83f) * 0.2f, 0.5f + (float)Math.Sin((float)frame / 83f) * 0.2f, 0.5f + (float)Math.Sin((float)frame / 83f) * 0.2f, 0.5f + (float)Math.Sin((float)frame / 83f) * 0.2f));
		float num = (int)MathHelper.Clamp((float)(Math.Sin((float)frame / 30f) * 2.0) + 1.5f, 0f, 1f);
		string text = "PRESS ENTER";
		text = "PRESS START";
		spriteBatch.DrawString(menuFont, text, new Vector2(base.GraphicsDevice.Viewport.Width / 2, (float)base.GraphicsDevice.Viewport.Height * 0.65f), new Color(num / 2f, num / 1.1f, num, num), 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
		spriteBatch.End();
	}

	private void DrawGalaxyMap(SpriteBatch spriteBatch)
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		spriteBatch.Draw(txGalaxyClouds, Vector2.Zero, null, Color.White * 0.5f, (float)(0L - (long)frame) / 351f, new Vector2(txGalaxyClouds.Width, txGalaxyClouds.Height) / 2f, 2.2f, SpriteEffects.None, 1f);
		stars2.Draw(spriteBatch, 1f);
		stars.Draw(spriteBatch, 1f);
		galaxyMap.Draw(spriteBatch, 1f);
		stars.Draw(spriteBatch, 1f);
		stars2.Draw(spriteBatch, 1f);
		particleSystem.DrawTrails(spriteBatch);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		for (int i = 0; i < 12; i++)
		{
			level[i].Draw(spriteBatch);
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive, null, null, null, null, camera.get_transformation(base.GraphicsDevice));
		spriteBatch.Draw(txGalaxyClouds, Vector2.Zero, null, Color.White * 0.5f, (float)(0L - (long)frame) / 501f, new Vector2(txGalaxyClouds.Width, txGalaxyClouds.Height) / 2f, 2.5f, SpriteEffects.None, 1f);
		selectPlanetExt.Draw(spriteBatch, 1f);
		selectPlanetInt.Draw(spriteBatch, 1f);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Additive);
		DrawLens(12);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		for (int i = 0; i < 12; i++)
		{
			level[currentLevel].DrawBrf(spriteBatch, new Vector2(base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height));
			spriteBatch.Draw(txGalaxyUI, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), new Color(0.5f, 0.5f, 0.5f, 0.5f));
			Vector2 vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Left, 50f, 1000f), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
			Text.print(spriteBatch, gameFont, "(B) Cancel/Back", new Vector2(vector.X * menuActive, vector.Y), Color.White * menuActive, txButtons);
			string text = (level[currentLevel].briefing ? "Press (A) button to play" : "Press (A) button for briefing");
			if (!level[currentLevel].locked)
			{
				spriteBatch.DrawString(menuFont, level[currentLevel].name, new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.TitleSafeArea.Top + 30), Color.LightCyan, 0f, new Vector2(menuFont.MeasureString(level[currentLevel].name).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
				Text.print(spriteBatch, gameFont, text, new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) * menuActive - gameFont.MeasureString(text).X / 2f, vector.Y - 25f), Color.White * menuActive, txButtons);
			}
			else
			{
				spriteBatch.DrawString(menuFont, level[currentLevel].name, new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.TitleSafeArea.Top + 30), Color.DarkGray, 0f, new Vector2(menuFont.MeasureString(level[currentLevel].name).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
				spriteBatch.DrawString(menuFont, "Locked!", new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.TitleSafeArea.Top + 55), Color.DarkGray, 0f, new Vector2(menuFont.MeasureString(level[currentLevel].name).X / 2f, 0f), 0.7f, SpriteEffects.None, 1f);
				if (level[currentLevel].briefing)
				{
					text = "Level Locked";
					spriteBatch.DrawString(menuFont, text, new Vector2(base.GraphicsDevice.Viewport.Width / 2, (float)base.GraphicsDevice.Viewport.Height * 0.35f), Color.White, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
					spriteBatch.DrawString(menuFont, text, new Vector2(base.GraphicsDevice.Viewport.Width / 2 + 3, (float)base.GraphicsDevice.Viewport.Height * 0.35f + 3f), Color.Black, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 0f);
					text = "Play the previous level to unlock";
					if (demo && currentLevel >= maxDemoLevel)
					{
						text = "Not available in Demo version";
					}
					spriteBatch.DrawString(menuFont, text, new Vector2(base.GraphicsDevice.Viewport.Width / 2, (float)base.GraphicsDevice.Viewport.Height * 0.6f), Color.White, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 1f);
					spriteBatch.DrawString(menuFont, text, new Vector2(base.GraphicsDevice.Viewport.Width / 2 + 3, (float)base.GraphicsDevice.Viewport.Height * 0.6f + 3f), Color.Black, 0f, new Vector2(menuFont.MeasureString(text).X / 2f, 0f), 1f, SpriteEffects.None, 0f);
				}
			}
			vector = new Vector2(MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Right, 50f, base.GraphicsDevice.Viewport.Width - 50), MathHelper.Clamp(base.GraphicsDevice.Viewport.TitleSafeArea.Bottom, 100f, base.GraphicsDevice.Viewport.Height - 50));
			if (Guide.IsTrialMode)
			{
				Text.print(spriteBatch, gameFont, "Press (Y) to buy the game", new Vector2(vector.X - gameFont.MeasureString("Press (Y) to buy the game").X, vector.Y), Color.White * menuActive, txButtons);
			}
			if (!level[currentLevel].briefing)
			{
				if (currentLevel < 11 && !level[currentLevel + 1].locked)
				{
					spriteBatch.DrawString(gameFont, ">", new Vector2(base.GraphicsDevice.Viewport.Width / 2, 10f), Color.LightCyan, 0f, new Vector2(0f - menuFont.MeasureString(level[currentLevel].name).X - 10f, 0f), new Vector2(0.75f, 3f), SpriteEffects.None, 1f);
				}
				if (currentLevel > 0 && !level[currentLevel - 1].locked)
				{
					spriteBatch.DrawString(gameFont, "<", new Vector2(base.GraphicsDevice.Viewport.Width / 2, 10f), Color.LightCyan, 0f, new Vector2(menuFont.MeasureString(level[currentLevel].name).X + 10f, 0f), new Vector2(0.75f, 3f), SpriteEffects.None, 1f);
				}
			}
		}
		spriteBatch.End();
	}

	private void DrawBlast()
	{
		for (int i = 0; i < blast.Count; i++)
		{
			blast[i].Draw(spriteBatch);
		}
	}

	public void createMainMenu()
	{
		try
		{
			menuBackground.Initialize(txMenuBackground, new Vector2(0f, 0f), new Vector2(0f, 0f), 0f);
		}
		catch
		{
		}
		menuBackground.size = new Vector2((float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height, (float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height);
		Vector2 position = new Vector2((float)base.GraphicsDevice.Viewport.Width / 2.75f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		mainMenu.Add(new MenuItem(menuFont, "Options", position, selectable: true, -1000f, 0));
		mainMenu.Add(new MenuItem(menuFont, "Credits", position, selectable: true, -1000f, 0));
		mainMenu.Add(new MenuItem(menuFont, "Awards", position, selectable: true, -1000f, 0));
		mainMenu.Add(new MenuItem(menuFont, "Play", "Select one of the many game modes", position, selectable: true, -1000f, 0));
		mainMenu.Add(new MenuItem(menuFont, "How to Play", position, selectable: true, -1000f, 0));
		mainMenu.Add(new MenuItem(menuFont, "Controls", position, selectable: true, -1000f, 0));
		mainMenu.Add(new MenuItem(menuFont, "Exit", "Leave the game", position, selectable: true, -1000f, 0));
		for (int i = 0; i < mainMenu.Count; i++)
		{
			mainMenu[i].angle = (float)(i + 1) * ((float)Math.PI / (float)mainMenu.Count);
		}
		menuIndex = (int)((float)(mainMenu.Count / 2) + 0.1f);
	}

	public void createPlayMenu()
	{
		readUnlockables();
		try
		{
			menuBackground.Initialize(txMenuBackground, new Vector2(0f, 0f), new Vector2(0f, 0f), 0f);
		}
		catch
		{
		}
		menuBackground.size = new Vector2((float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height, (float)base.GraphicsDevice.Viewport.Height / (float)menuBackground.Height);
		Vector2 position = new Vector2((float)base.GraphicsDevice.Viewport.Width / 2.75f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		playMenu.Add(new MenuItem(menuFont, "Meteroids Mode", "Hardcore old school", "Play Campaign to unlock this mode", position, unlockMeteroids, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Sidescroller", "Play in a traditional way", "Play Campaign to unlock this mode", position, unlockSidescroller, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Chubby Rain Mode", "Play in an old school way", "Play Campaign to unlock this mode", position, unlockChubbyRain, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Survival Mode", "Play against a flood of endless enemies", position, selectable: true, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Campaign", "Unlock new worlds and new game modes", position, selectable: true, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Play Tutorial", "learn the basics of Defenders of the Last Colony", position, selectable: true, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Challenge Mode", "", "Play Campaign to unlock this mode", position, selectable: true, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Back", position, selectable: true, -1000f, 0));
		playMenu.Add(new MenuItem(menuFont, "Boss Fight", "Play the final stage", "Finish Campaign to play against the Final Boss", position, selectable: true, -1000f, 0));
		for (int i = 0; i < playMenu.Count; i++)
		{
			playMenu[i].angle = (float)(i + 1) * ((float)Math.PI / (float)playMenu.Count);
			if (editor)
			{
				playMenu[i].selectable = true;
			}
		}
		playMenuIndex = (int)((float)(playMenu.Count / 2) + 0.1f);
	}

	public void createOptionsMenu()
	{
		Vector2 position = new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		try
		{
			optionsMenu = new List<MenuItem>(8);
		}
		catch
		{
		}
		readOptions();
		optionsMenu.Add(new MenuItem(menuFont, "HUD Transparency: ", position, selectable: true, GOHUDopacity, 1));
		optionsMenu.Add(new MenuItem(menuFont, "Music: ", position, selectable: true, GOmusicVolume, 1));
		optionsMenu.Add(new MenuItem(menuFont, "Sound FX: ", position, selectable: true, GOsoundFXvolume, 1));
		optionsMenu.Add(new MenuItem(menuFont, "Controller Vibration", position, selectable: true, GOvibration, 2));
		optionsMenu.Add(new MenuItem(menuFont, "Difficulty", "Change game difficulty", position, selectable: true, difficulty, 5));
		optionsMenu.Add(new MenuItem(menuFont, "Reset options to Default", "It will overwrite your current options", position, selectable: true, 0f, 6));
		optionsMenu.Add(new MenuItem(menuFont, "Remove stored Data", "It removes all your progress,\nso you can start the game from scratch", position, selectable: true, 0f, 6));
		optionsMenu.Add(new MenuItem(menuFont, "Back", position, selectable: true, -1000f, 0));
		for (int i = 0; i < optionsMenu.Count; i++)
		{
			optionsMenu[i].wide = 200f;
			optionsMenu[i].angle = (float)(i + 1) * ((float)Math.PI / (float)optionsMenu.Count);
		}
		optionsMenuIndex = (int)((float)(optionsMenu.Count / 2) + 0.55f);
	}

	public void createSurvivalStatsMenu()
	{
		Vector2 position = new Vector2((float)base.GraphicsDevice.Viewport.Width / 2.75f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		try
		{
			survivalStatsMenu = new List<MenuItem>(5);
		}
		catch
		{
		}
		survivalStatsMenu.Add(new MenuItem(menuFont, "Main Menu", position, selectable: true, -1000f, 0));
		survivalStatsMenu.Add(new MenuItem(menuFont, "Play Again", position, selectable: true, -1000f, 0));
		survivalStatsMenu.Add(new MenuItem(menuFont, "Character Selection", position, selectable: true, -1000f, 0));
		survivalStatsMenu.Add(new MenuItem(menuFont, "Select Game Mode", position, selectable: true, -1000f, 0));
		for (int i = 0; i < survivalStatsMenu.Count; i++)
		{
			survivalStatsMenu[i].angle = (float)(i + 1) * ((float)Math.PI / (float)survivalStatsMenu.Count);
		}
		survivalStatsMenuIndex = (int)((float)(survivalStatsMenu.Count / 2) + 0.55f);
	}

	public void createPauseMenu()
	{
		Vector2 position = new Vector2((float)base.GraphicsDevice.Viewport.Width / 2.75f, (float)base.GraphicsDevice.Viewport.Height / 1.8f);
		try
		{
			pauseMenu = new List<MenuItem>(8);
		}
		catch
		{
		}
		pauseMenu.Add(new MenuItem(menuFont, "HUD Transparency: ", position, selectable: true, GOHUDopacity, 1));
		pauseMenu.Add(new MenuItem(menuFont, "Music: ", position, selectable: true, GOmusicVolume, 1));
		pauseMenu.Add(new MenuItem(menuFont, "Resume", position, selectable: true, -1000f, 0));
		pauseMenu.Add(new MenuItem(menuFont, "Sound FX: ", position, selectable: true, GOsoundFXvolume, 1));
		pauseMenu.Add(new MenuItem(menuFont, "Controller Vibration", position, selectable: true, GOvibration, 2));
		toGalaxyID = pauseMenu.Count;
		pauseMenu.Add(new MenuItem(menuFont, "Exit to Galaxy Map", position, selectable: true, -1000f, 0));
		pauseMenu.Add(new MenuItem(menuFont, "Exit to Main Menu", position, selectable: true, -1000f, 0));
		for (int i = 0; i < pauseMenu.Count; i++)
		{
			pauseMenu[i].angle = (float)(i + 1) * ((float)Math.PI / (float)pauseMenu.Count);
		}
		pauseMenuIndex = (int)((float)(pauseMenu.Count / 2) + 0.55f);
	}

	public Vector2 randomBorderPos()
	{
		Vector2 zero = Vector2.Zero;
		switch (random.Next(4))
		{
		case 0:
			zero.Y = -400f;
			zero.X = random.Next(-640, 1920);
			break;
		case 1:
			zero.Y = 1200f;
			zero.X = random.Next(-640, 1920);
			break;
		case 2:
			zero.X = -640f;
			zero.Y = random.Next(-400, 120);
			break;
		case 3:
			zero.X = 1920f;
			zero.Y = random.Next(-400, 120);
			break;
		default:
			zero.Y = -400f;
			zero.X = random.Next(-640, 1920);
			break;
		}
		return zero;
	}

	public void createEnemies(GameTime gameTime)
	{
		createEnemies(gameTime, calculateSpawning: true, Vector2.Zero);
	}

	public void createEnemies(GameTime gameTime, Vector2 pos)
	{
		createEnemies(gameTime, calculateSpawning: false, pos);
	}

	public void createEnemies(GameTime gameTime, bool calculateSpawning, Vector2 pos)
	{
		maxEnemies = numPlayers * 4 + (int)(round * round);
		if (maxEnemies < 20)
		{
			maxEnemies = 20;
		}
		if (currentLevel == 13)
		{
			maxEnemies = (int)frame / 200 + (int)round;
		}
		if (maxEnemies > topEnemies)
		{
			maxEnemies = topEnemies;
		}
		waveTotal--;
		waveTotal = (int)MathHelper.Clamp(waveTotal, 0f, maxEnemies);
		positionBegin = pos;
		if (calculateSpawning)
		{
			calculateSpawningPoint();
			pos = positionBegin;
		}
		else
		{
			positionBegin = pos;
		}
		if (frame % 300 == 0 && level[currentLevel].asteroids >= 0 && random.Next(100) < 10)
		{
			AddEnemy(randomBorderPos(), (float)random.Next(720) / 100f, 0, (float)random.Next(100) / 10f + 2f);
		}
		int num = random.Next(10);
		int num2 = 0;
		int num3 = 1;
		if (num >= 7 || num == 1)
		{
			num2 = 3;
			num3 = (int)(3 + round) + numPlayers;
			num3 = (int)MathHelper.Clamp(num3, 0f, 15f);
		}
		ushort num4 = (ushort)MathHelper.Clamp(random.Next((int)((float)numPlayers + (float)round * 0.4f)), num2, num3);
		for (int i = 0; i < num4; i++)
		{
			if (random.Next(100) < 50 && frame % 10 == 0 && enemies.Count < maxEnemies)
			{
				float angle = (float)random.Next(1000) / 100f;
				AddEnemy(positionBegin, angle, num);
			}
		}
		calculateSpawningPoint();
	}

	public void CreateChubbyRain(GameTime gameTime)
	{
		maxEnemies = 100;
		for (int i = 0; (float)i < MathHelper.Clamp((round + 2) / 2, 1f, 3f); i++)
		{
			Vector2 vector = new Vector2(100 + i * i * 50, 200 - i * 75);
			for (int j = 0; j < 10 - i * i * 2; j++)
			{
				float angle = (float)random.Next(1000) / 100f;
				if (i == 0 && round > 14)
				{
					AddEnemy(vector - Vector2.UnitY * 25f, angle, 11);
				}
				if (i == 0 && round > 24)
				{
					AddEnemy(vector + Vector2.UnitY * 25f, angle, 11);
				}
				if (i < 2)
				{
					AddEnemy(vector, angle, 100);
				}
				else if (round > 9)
				{
					if ((round - 10) % 2 == 0)
					{
						AddEnemy(vector, angle, 3);
					}
					if ((round - 10) % 3 == 0)
					{
						AddEnemy(vector, angle, 2);
					}
					if ((round - 10) % 5 == 0)
					{
						AddEnemy(vector, angle, 100);
					}
				}
				vector.X += 50f;
			}
		}
	}

	public Vector2 calculateSpawningPoint()
	{
		if (random.Next(1000) > 990)
		{
			do
			{
				positionBegin = new Vector2(random.Next(-200, (int)((float)base.GraphicsDevice.Viewport.Width * 1.4f)), random.Next(-200, (int)((float)base.GraphicsDevice.Viewport.Height * 1.4f)));
			}
			while (Vector2.Distance(positionBegin, colony.position) < (float)colony.Width / 1.5f);
		}
		positionBegin.X = MathHelper.Clamp(positionBegin.X, -540f, 1820f);
		positionBegin.Y = MathHelper.Clamp(positionBegin.Y, -300f, 1100f);
		if (gameState == GameState.Sidescroller)
		{
			positionBegin.X = 900f;
			positionBegin.Y = MathHelper.Clamp((float)Math.Sin((float)frame / 15f) * 160f + 320f, 160f, 480f);
		}
		return positionBegin;
	}

	public Vector2 RandomPosition()
	{
		Vector2 vector = Vector2.Zero;
		if (gameState == GameState.Campaign || gameState == GameState.finalBoss)
		{
			do
			{
				vector = new Vector2(random.Next(-200, (int)((float)base.GraphicsDevice.Viewport.Width * 1.4f)), random.Next(-200, (int)((float)base.GraphicsDevice.Viewport.Height * 1.4f)));
			}
			while (Vector2.Distance(vector, colony.position) < (float)colony.Width / 1.5f);
		}
		else
		{
			vector = new Vector2(random.Next(-200, (int)((float)base.GraphicsDevice.Viewport.Width * 1.4f)), random.Next(-200, (int)((float)base.GraphicsDevice.Viewport.Height * 1.4f)));
		}
		vector.X = MathHelper.Clamp(vector.X, -540f, 1820f);
		vector.Y = MathHelper.Clamp(vector.Y, -300f, 1100f);
		if (gameState == GameState.Sidescroller)
		{
			vector.X = 900f;
			vector.Y = MathHelper.Clamp((float)Math.Sin((float)frame / 15f) * 160f + 320f, 160f, 480f);
		}
		return vector;
	}

	public void createGalaxy()
	{
		level[0] = new Level(txPlanetHines, txPlanetHinesBrf, "Hines", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[1] = new Level(txPlanetNymeriah, txPlanetNymeriahBrf, "Nymeriah", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[2] = new Level(txPlanetHerschel, txPlanetHerschelBrf, "Herschel", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[3] = new Level(txPlanetDanae, txPlanetDanaeBrf, "Danae", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[4] = new Level(txPlanetClarke, txPlanetClarkeBrf, "Clarke", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[5] = new Level(txPlanetGeaMoon, txPlanetGeaMoonBrf, "Gea Moon", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[6] = new Level(txPlanetCalypso, txPlanetCalypsoBrf, "Calypso", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[7] = new Level(txPlanetBradbury, txPlanetBradburyBrf, "Bradbury", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[8] = new Level(txPlanetEos, txPlanetEosBrf, "Eos rests", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[9] = new Level(txPlanetOlbers, txPlanetOlbersBrf, "Olbers 4", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[10] = new Level(txPlanetEneas, txPlanetEneasBrf, "Eneas", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[11] = new Level(txPlanetPrometheus, txPlanetPrometheusBrf, "Prometheus", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[12] = new Level(txPlanetPrometheus, txPlanetPrometheusBrf, "Simulator Room", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[13] = new Level(txPlanetPrometheus, txPlanetPrometheusBrf, "Final Boss", txInterlaced, txWhiteBar, base.GraphicsDevice);
		level[14] = new Level(txSimulatorRoom, txEmpty, "Tutorial Level", txEmpty, txWhiteBar, base.GraphicsDevice);
		lens[12].Add(new Lens(new Vector2(10f, 20f), 0f, 0f, 0f, 0f, 2f, txLens_glow1, txLens_a1, txLens_a4, txLens_a4, txLens_a4, txLensDirt2));
	}

	public void createCharacters()
	{
		try
		{
			characterSel = new int[4];
		}
		catch
		{
		}
		for (int i = 0; i < 4; i++)
		{
			characterSel[i] = -1;
		}
		try
		{
			characters = new Character[4];
		}
		catch
		{
		}
		try
		{
			player = new Player[4];
		}
		catch
		{
		}
		player[0] = new Player(base.GraphicsDevice, txFighter[0], menuFont, 0, fighter: true, PlayerIndex.One, txUI, txUIHealth, txUIBlue, txCircle, txBar);
		player[1] = new Player(base.GraphicsDevice, txFighter[1], menuFont, 1, fighter: true, PlayerIndex.Two, txUI, txUIHealth, txUIBlue, txCircle, txBar);
		player[2] = new Player(base.GraphicsDevice, txFighter[2], menuFont, 2, fighter: true, PlayerIndex.Three, txUI, txUIHealth, txUIBlue, txCircle, txBar);
		player[3] = new Player(base.GraphicsDevice, txFighter[3], menuFont, 3, fighter: true, PlayerIndex.Four, txUI, txUIHealth, txUIBlue, txCircle, txBar);
		List<float> list = new List<float>(4);
		for (int j = 0; j < 4; j++)
		{
			list.Add(0f);
		}
		Character[] array = characters;
		Color lightCyan = Color.LightCyan;
		ushort[] relics = new ushort[15];
		array[0] = new Character("Mark R.", "Fighter", lightCyan, "Hell Storm", relics, new List<float>(list) { 0f, 0f, 0f, 0f }, 0u, 0, 0, 750);
		Character[] array2 = characters;
		Color color = new Color(1f, 0.37f, 0f);
		relics = new ushort[15];
		array2[1] = new Character("Michelle W.", "Defender", color, "Sonic Bomb", relics, new List<float>(list) { 0f, 0f, 0f, 0f }, 0u, 0, 0, 750);
		Character[] array3 = characters;
		Color color2 = new Color(0f, 0.86f, 0.01f);
		relics = new ushort[15];
		array3[2] = new Character("James R.", "Fighter", color2, "EMP", relics, new List<float>(list) { 0f, 0f, 0f, 0f }, 0u, 0, 0, 750);
		Character[] array4 = characters;
		Color color3 = new Color(0.86f, 0f, 0.84f);
		relics = new ushort[15];
		array4[3] = new Character("Johnny V.", "Defender", color3, "Laser Blades", relics, new List<float>(list) { 0f, 0f, 0f, 0f }, 0u, 0, 0, 750);
		readCharacters();
		copyCharactersToPlayers();
	}

	private void copyCharactersToPlayers()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				List<float> list = new List<float>(4);
				list.Add(characters[j].ability[0]);
				list.Add(characters[j].ability[1]);
				list.Add(characters[j].ability[2]);
				list.Add(characters[j].ability[3]);
				player[i].characters[j] = new Character(characters[j].name, characters[j].shipClass, characters[j].color, characters[j].abilityType, characters[j].relics, new List<float>(list), characters[j].numberOfKills, characters[j].level, characters[j].experience, characters[j].nextLevel);
			}
		}
	}

	private void copyPlayersToCharacters()
	{
		for (int i = 0; i < 4; i++)
		{
			if (player[i].Active)
			{
				List<float> list = new List<float>(4);
				list.Add(player[i].ability[0]);
				list.Add(player[i].ability[1]);
				list.Add(player[i].ability[2]);
				list.Add(player[i].ability[3]);
				ushort[] array = new ushort[15];
				ushort[] array2 = array;
				for (int j = 0; j < 14; j++)
				{
					array2[j] = player[i].characters[player[i].number].relics[j];
				}
				characters[player[i].number] = new Character(player[i].name, player[i].shipClass, player[i].color, player[i].abilityType, array2, new List<float>(list), player[i].numberOfKills, player[i].level, player[i].experience, player[i].nextLevel);
			}
		}
	}

	private void resetPlayers()
	{
		copyCharactersToPlayers();
	}

	private void resetPlayersSurvival()
	{
		bool[] array = new bool[4];
		for (int i = 0; i < player.Length; i++)
		{
			array[i] = player[i].wasActive;
		}
		copyCharactersToPlayers();
		for (int i = 0; i < player.Length; i++)
		{
			player[i].wasActive = array[i];
			player[i].Active = array[i];
			player[i].Reset();
		}
	}

	public void reset()
	{
		if (gameState != GameState.disclaimer && gameState != GameState.logo)
		{
			frame = 0u;
		}
		round = 0u;
		gatherMsgDelay = 1000 + currentLevel * 200;
		colony.damaged = false;
		enemies.RemoveRange(0, enemies.Count);
		enemyBullets.RemoveRange(0, enemyBullets.Count);
		messages.RemoveRange(0, messages.Count);
		bullets.RemoveRange(0, bullets.Count);
		pickups.RemoveRange(0, pickups.Count);
		coins.RemoveRange(0, coins.Count);
		construction.RemoveRange(0, construction.Count);
		particleSystem.particles.RemoveRange(0, particleSystem.particles.Count);
		particleSystem.playerTrails.RemoveRange(0, particleSystem.playerTrails.Count);
		particleSystem.itemTrails.RemoveRange(0, particleSystem.itemTrails.Count);
		particleSystem.Initialize(txNoise, txDots, explosionSound);
		blast.RemoveRange(0, blast.Count);
		if (gameStatePlay == GameState.Campaign && gameState == gameStateNext)
		{
			lens[currentLevel].RemoveRange(0, lens[currentLevel].Count);
		}
		colony.health = colony.MaximunHealth;
		colony.healthTarget = colony.MaximunHealth;
		colony.energy = 0f;
		colony.energyTarget = 0f;
		playerChallenge.Health = playerChallenge.maximunHealth;
		resetCamera(gameStateNext == GameState.galaxyMap);
		endGame = 0;
	}

	private void cleanupEditor()
	{
		enemies.RemoveRange(0, enemies.Count);
		enemyBullets.RemoveRange(0, enemyBullets.Count);
		messages.RemoveRange(0, messages.Count);
		bullets.RemoveRange(0, bullets.Count);
		pickups.RemoveRange(0, pickups.Count);
		coins.RemoveRange(0, coins.Count);
		construction.RemoveRange(0, construction.Count);
		particleSystem.particles.RemoveRange(0, particleSystem.particles.Count);
		particleSystem.playerTrails.RemoveRange(0, particleSystem.playerTrails.Count);
		particleSystem.itemTrails.RemoveRange(0, particleSystem.itemTrails.Count);
		particleSystem.Initialize(txNoise, txDots, explosionSound);
		blast.RemoveRange(0, blast.Count);
	}

	public void resetCamera(bool isGalaxy)
	{
		if (isGalaxy)
		{
			camera.position = new Vector2(50f, 0f);
			cameraZoom = 0.02f;
			camera.Zoom = 0.02f;
			camera.Rotation = 0f;
		}
		else
		{
			cameraZoom = 0.01f;
			camera.Zoom = 0.01f;
			camera.position = new Vector2(640f, 400f);
			camera.Rotation = 0f;
		}
	}

	public void iniCharacters(bool initializeCharacters)
	{
		iniCharacters(initializeCharacters, addRelics: false);
	}

	public void iniCharacters(bool initializeCharacters, bool addRelics)
	{
		initializeCharacters = false;
		if (initializeCharacters)
		{
			createCharacters();
			return;
		}
		for (int i = 0; i < player.Length; i++)
		{
			player[i].Reset();
		}
	}

	public void createLevel(bool characters)
	{
		reset();
		if (develop || editor)
		{
			assetManager[currentLevel].asset = ReadWorld();
		}
		iniCharacters(initializeCharacters: false);
		camera.limits = new Rectangle(-40, -35, 700, 435);
		colony.damaged = false;
		for (int i = 0; i < assetManager[currentLevel].asset.Count; i++)
		{
			Vector2 position = assetManager[currentLevel].asset[i].position;
			switch (assetManager[currentLevel].asset[i].levelData)
			{
			case LevelData.colony:
				colony.Initialize(txColony, position);
				lens[currentLevel].Add(new Lens(position, 0f, 0f, 0f, 0f, 1f, txLens_glow1, txLens_a1, txLens_a2, txLens_a4, txLens_a4, txLensDirt2));
				break;
			case LevelData.playerSpawn:
				player[assetManager[currentLevel].asset[i].type - 1].Reset(position);
				break;
			case LevelData.relic:
				pickups.Add(new Pickup(txRelic, position, position, Pickup.item.relic, 15, 2, random.Next(10000), base.GraphicsDevice));
				break;
			case LevelData.orb:
				pickups.Add(new Pickup(txOrbs, position, position, Pickup.item.orb, 1, 1, random.Next(10000), base.GraphicsDevice));
				break;
			case LevelData.lensFlare:
				lens[currentLevel].Add(new Lens(position, (float)(int)assetManager[currentLevel].asset[i].color.R / 255f, (float)(int)assetManager[currentLevel].asset[i].color.G / 255f, (float)(int)assetManager[currentLevel].asset[i].color.B / 255f, (float)(int)assetManager[currentLevel].asset[i].color.A / 255f, assetManager[currentLevel].asset[i].size, txLens_glow1, txLens_a1, txLens_a2, txLens_a3, txLens_a4, txLensDirt1));
				break;
			}
		}
		positionBegin = new Vector2(random.Next(100, base.GraphicsDevice.Viewport.Width - 100), random.Next(100, base.GraphicsDevice.Viewport.Height - 100));
		waveTotal = 0;
		maxEnemies = 20;
		for (int i = 0; i < 100; i++)
		{
			AddItem(Pickup.item.coins, new Vector2(random.Next(base.GraphicsDevice.Viewport.Width / -2, (int)((float)base.GraphicsDevice.Viewport.Width * 1.5f)), random.Next(base.GraphicsDevice.Viewport.Height / -2, (int)((float)base.GraphicsDevice.Viewport.Height * 1.5f))), 1);
		}
		HUD.Initialize(txHUD, new Vector2(0f, 0f), new Vector2(0f, 0f), 0f);
		HUD.size = new Vector2(base.GraphicsDevice.Viewport.Width / HUD.texture.Width, base.GraphicsDevice.Viewport.Height / HUD.texture.Height);
		Texture2D texture2D = txStars2;
		switch (currentLevel)
		{
		case 0:
			texture2D = txHines;
			break;
		case 1:
			texture2D = txNymeriah;
			break;
		case 2:
			texture2D = txHerschel;
			break;
		case 3:
			texture2D = txDanae;
			break;
		case 4:
			texture2D = txClarke;
			break;
		case 5:
			texture2D = txGeaMoon;
			break;
		case 6:
			texture2D = txCalypso;
			break;
		case 7:
			texture2D = txBradbury;
			break;
		case 8:
			texture2D = txEosRest;
			break;
		case 9:
			texture2D = txOlbers4;
			break;
		case 10:
			texture2D = txEneas;
			break;
		case 11:
			texture2D = txPrometheus;
			break;
		case 12:
			texture2D = txPrometheus;
			break;
		case 13:
			texture2D = txStars2;
			lens[currentLevel].RemoveRange(0, lens[currentLevel].Count);
			lens[currentLevel].Add(new Lens(new Vector2(640f, 400f), 1f, 0.2f, 0f, 0f, 2f, txLens_glow1, txLens_a1, txLens_a2, txLens_a4, txLens_a4, txLensDirt1));
			lens[currentLevel].Add(new Lens(colony.position, 0f, 0f, 0f, 0f, 1f, txLens_glow1, txLens_a1, txLens_a2, txLens_a4, txLens_a4, txLensDirt2));
			lens[currentLevel].Add(new Lens(new Vector2(640f, 400f), 0f, 0f, 0f, 0f, 1f, txLens_glow1, txLens_a1, txLens_a2, txLens_a4, txLens_a4, txLensDirt2));
			break;
		default:
			texture2D = txSimulatorRoom;
			break;
		}
		background.Initialize(texture2D, Vector2.Zero, Vector2.Zero, 0f);
		background.size = new Vector2(1280 / background.Width * 2, 720 / background.Height * 2);
		stars.Initialize(txStars, Vector2.Zero, Vector2.Zero, 0f);
		stars.size = new Vector2((float)(1280 / stars.Width) * 2f, (float)(720 / stars.Height) * 2f);
		stars2.Initialize(txStars2, Vector2.Zero, Vector2.Zero, 0f);
		stars2.size = new Vector2((float)(1280 / stars2.Width) * 2f, (float)(720 / stars2.Height) * 2f);
		border.Initialize(txBorder, Vector2.Zero, Vector2.Zero, 0f);
		border.size = new Vector2((float)(1280 / border.Width) * 2f, (float)(720 / border.Height) * 2f);
		border.position = new Vector2(-640f, -360f);
		hexagonsGrid.Initialize(txHexagonsGrid, Vector2.Zero, Vector2.Zero, 0f);
		hexagonsGrid.size = new Vector2((float)(1280 / txHexagonsGrid.Width) * 2f, (float)(720 / txHexagonsGrid.Height) * 2f);
		hexagonsGrid.position = border.position;
		hexagonsGrid.depth = 0f;
		hexagonsGrid.transparency = 0f;
	}

	public void createChallengeLevel()
	{
		reset();
		assetManagerChallenge[challengeNumber].asset = ReadChallenge();
		playerChallenge = new Player(1);
		playerChallenge.Initialize(base.GraphicsDevice, txIngeneer[0], menuFont, 0, fighter: false, controllingPlayer, txUI, txUIHealth, txUIBlue, txCircle, txBar);
		camera.limits = new Rectangle(-40, -35, 700, 435);
		for (int i = 0; i < assetManagerChallenge[challengeNumber].asset.Count; i++)
		{
			Vector2 position = assetManagerChallenge[challengeNumber].asset[i].position;
			bloom(position);
			switch (assetManagerChallenge[challengeNumber].asset[i].levelData)
			{
			case LevelData.colony:
				colony.Initialize(txColony, position);
				colony.Active = false;
				break;
			case LevelData.playerSpawn:
				playerChallenge.Reset(position);
				break;
			case LevelData.relic:
				pickups.Add(new Pickup(txRelic, position, position, Pickup.item.relic, 15, 2, random.Next(10000), base.GraphicsDevice));
				break;
			case LevelData.orb:
				pickups.Add(new Pickup(txOrbs, position, position, Pickup.item.orb, 1, 1, random.Next(10000), base.GraphicsDevice));
				break;
			case LevelData.lensFlare:
				lens[challengeNumber].Add(new Lens(position, (float)(int)assetManagerChallenge[challengeNumber].asset[i].color.R / 255f, (float)(int)assetManagerChallenge[challengeNumber].asset[i].color.G / 255f, (float)(int)assetManagerChallenge[challengeNumber].asset[i].color.B / 255f, (float)(int)assetManagerChallenge[challengeNumber].asset[i].color.A / 255f, assetManagerChallenge[challengeNumber].asset[i].size, txLens_glow1, txLens_a1, txLens_a2, txLens_a3, txLens_a4, txLensDirt1));
				break;
			case LevelData.blueMatter:
				AddItem(Pickup.item.coins, position, 1);
				break;
			case LevelData.pathNode:
				AddItem(Pickup.item.pathNode, position, 1);
				break;
			case LevelData.enemy:
			{
				float angle = (float)random.Next(1000) / 100f;
				AddEnemy(position, angle, assetManagerChallenge[challengeNumber].asset[i].type);
				assetManagerChallenge[challengeNumber].asset[i].numPri--;
				break;
			}
			}
		}
		playerChallenge.Active = true;
		positionBegin = new Vector2(random.Next(100, base.GraphicsDevice.Viewport.Width - 100), random.Next(100, base.GraphicsDevice.Viewport.Height - 100));
		waveTotal = 0;
		maxEnemies = topEnemies;
		HUD.Initialize(txHUD, new Vector2(0f, 0f), new Vector2(0f, 0f), 0f);
		HUD.size = new Vector2(base.GraphicsDevice.Viewport.Width / HUD.texture.Width, base.GraphicsDevice.Viewport.Height / HUD.texture.Height);
		Texture2D texture = txStars2;
		background.Initialize(texture, Vector2.Zero, Vector2.Zero, 0f);
		background.size = new Vector2(1280 / background.Width * 2, 720 / background.Height * 2);
		stars.Initialize(txStars, Vector2.Zero, Vector2.Zero, 0f);
		stars.size = new Vector2((float)(1280 / stars.Width) * 2f, (float)(720 / stars.Height) * 2f);
		stars2.Initialize(txStars2, Vector2.Zero, Vector2.Zero, 0f);
		stars2.size = new Vector2((float)(1280 / stars2.Width) * 2f, (float)(720 / stars2.Height) * 2f);
		border.Initialize(txBorder, Vector2.Zero, Vector2.Zero, 0f);
		border.size = new Vector2((float)(1280 / border.Width) * 2f, (float)(720 / border.Height) * 2f);
		border.position = new Vector2(-640f, -360f);
		hexagonsGrid.Initialize(txHexagonsGrid, Vector2.Zero, Vector2.Zero, 0f);
		hexagonsGrid.size = new Vector2((float)(1280 / txHexagonsGrid.Width) * 2f, (float)(720 / txHexagonsGrid.Height) * 2f);
		hexagonsGrid.position = border.position;
		hexagonsGrid.depth = 0f;
		hexagonsGrid.transparency = 0f;
	}

	public void createSurvivalLevel(bool characters)
	{
		iniCharacters(initializeCharacters: false);
		camera.limits = new Rectangle(-40, -35, 700, 435);
		reset();
		colony.Initialize(txColony, Vector2.Zero);
		colony.health = 0f;
		colony.healthTarget = 0f;
		colony.Active = false;
		player[0].Reset(Vector2.One * 400f);
		player[1].Reset(Vector2.One * 400f);
		player[2].Reset(Vector2.One * 400f);
		player[3].Reset(Vector2.One * 400f);
		for (ushort num = 0; num < sidescrollerB.Length; num++)
		{
			sidescrollerB[num].Initialize(txSidescrollerB[num], new Vector2(txSidescrollerB[num].Width * (num + 1), -100f), Vector2.One, 0f);
		}
		round = 0u;
		positionBegin = new Vector2(random.Next(100, base.GraphicsDevice.Viewport.Width - 100), random.Next(100, base.GraphicsDevice.Viewport.Height - 100));
		waveTotal = 0;
		maxEnemies = 5;
		for (int i = 0; i < 100; i++)
		{
			AddItem(new Vector2(random.Next(base.GraphicsDevice.Viewport.Width / -2, (int)((float)base.GraphicsDevice.Viewport.Width * 1.5f)), random.Next(base.GraphicsDevice.Viewport.Height / -2, (int)((float)base.GraphicsDevice.Viewport.Height * 1.5f))));
		}
		HUD.Initialize(txHUD, new Vector2(0f, 0f), new Vector2(0f, 0f), 0f);
		HUD.size = new Vector2(base.GraphicsDevice.Viewport.Width / HUD.texture.Width, base.GraphicsDevice.Viewport.Height / HUD.texture.Height);
		Texture2D texture = txStars;
		background.Initialize(texture, Vector2.Zero, Vector2.Zero, 0f);
		background.size = new Vector2(1280 / background.Width * 2, 720 / background.Height * 2);
		stars.Initialize(txStars, Vector2.Zero, Vector2.Zero, 0f);
		stars.size = new Vector2((float)(1280 / stars.Width) * 2f, (float)(720 / stars.Height) * 2f);
		stars2.Initialize(txStars2, Vector2.Zero, Vector2.Zero, 0f);
		stars2.size = new Vector2((float)(1280 / stars2.Width) * 2f, (float)(720 / stars2.Height) * 2f);
		border.Initialize(txBorder, Vector2.Zero, Vector2.Zero, 0f);
		border.size = new Vector2((float)(1280 / border.Width) * 2f, (float)(720 / border.Height) * 2f);
		border.position = new Vector2(-640f, -360f);
		hexagonsGrid.Initialize(txHexagonsGrid, Vector2.Zero, Vector2.Zero, 0f);
		hexagonsGrid.size = new Vector2((float)(1280 / txHexagonsGrid.Width) * 2f, (float)(720 / txHexagonsGrid.Height) * 2f);
		hexagonsGrid.position = border.position;
		hexagonsGrid.depth = 0f;
		hexagonsGrid.transparency = 0f;
		currTime = DateTime.Now;
		startTime = currTime;
		elapsedPauseTime = currTime - startTime;
		minutes = 0u;
		seconds = 0;
	}

	public void createBlast(int id)
	{
		createBlast(id, Vector2.Zero, 0);
	}

	public void createBlast(int id, Vector2 position)
	{
		createBlast(id, Vector2.Zero, 0);
	}

	public void createBlast(int id, Vector2 position, int type)
	{
		if (this.blast.Count <= 0 || type != 0)
		{
			Blast blast = new Blast();
			switch (id)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
				blast.Initialize(txBlast, player[id].position);
				blast.id = id;
				blast.size = Vector2.Zero;
				blast.maximunSize = 3f + 0.5f * (float)(int)player[id].level;
				break;
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
				blast.Initialize(txBlast, player[id - 10].position);
				blast.id = id - 10;
				blast.size = Vector2.Zero;
				blast.maximunSize = 3f;
				break;
			case -2:
				blast.Initialize(txBlast, colony.position);
				blast.id = id;
				blast.size = Vector2.Zero;
				blast.maximunSize = 3f;
				break;
			case -3:
				blast.Initialize(txBlast, position);
				blast.id = id;
				blast.size = Vector2.Zero;
				blast.maximunSize = 3f;
				break;
			case -500:
				blast.Initialize(txBlast, position);
				blast.id = id;
				blast.size = Vector2.Zero;
				blast.maximunSize = 1f;
				break;
			default:
				blast.Initialize(txBlast, colony.position);
				blast.id = id;
				blast.size = Vector2.Zero;
				blast.maximunSize = 10f;
				break;
			}
			float x = camera.getScreenPosition(blast.position, base.GraphicsDevice).X;
			float pan = MathHelper.Clamp(x * 4f / (float)base.GraphicsDevice.Viewport.Width - 2f, -1f, 1f);
			float volume = MathHelper.Clamp((0.03f + (float)random.Next(3) / 100f) * (GOsoundFXvolume / 100f), 0f, 1f);
			try
			{
				explosionSound.Play(volume, (float)random.Next(-100, -50) / 100f, pan);
			}
			catch
			{
			}
			blast.type = (ushort)type;
			AddExploss(blast.position);
			this.blast.Add(blast);
			blast = null;
		}
	}

	public void updateBlast(GameTime gameTime)
	{
		for (int i = 0; i < blast.Count; i++)
		{
			blast[i].Update();
			if (!blast[i].Active)
			{
				blast.RemoveAt(i);
				continue;
			}
			if (blast[i].size.X + blast[i].size.Y >= 3f && blast[i].type != 1)
			{
				camera.Zoom -= (float)random.Next(-1, 5) / 200f + blast[i].size.X * 0.1f * 0.001f;
				camera.position += new Vector2(random.Next(-5, 5), random.Next(-5, 5));
			}
			bloom(blast[i].position, (blast[i].size.X + blast[i].size.Y) / 2f, 5);
		}
	}

	private int newTarget(bool includeColony)
	{
		if (gameState == GameState.Challenge || gameStateNext == GameState.selectChallenge || gameState == GameState.challengeFinished || gameStateNext == GameState.challengeFinished)
		{
			return 0;
		}
		int num;
		if (includeColony)
		{
			num = -1;
			if (numPlayers > 0 && random.Next(-100, 50) < 0)
			{
				num = random.Next(4);
				if (gameState != GameState.Challenge)
				{
					while (!player[num].Active && gameState != GameState.Challenge && gameState != GameState.challengeFinished)
					{
						num = ((gameState != GameState.Challenge && gameState != GameState.challengeFinished) ? random.Next(4) : 0);
					}
				}
				else
				{
					num = -1;
				}
			}
		}
		else if (numPlayers > 0)
		{
			int i = 0;
			num = random.Next(4);
			if (gameState != GameState.Challenge && gameState != GameState.challengeFinished)
			{
				for (; !player[num].Active || i < 100; i++)
				{
					num = random.Next(4);
				}
			}
			else
			{
				num = -1;
			}
		}
		else
		{
			num = 0;
		}
		return num;
	}

	public void AddEnemy(Vector2 pos, float angle, int enemyType, int follow)
	{
		AddEnemy(pos, angle, enemyType, 3f, follow);
	}

	public void AddEnemy(Vector2 pos, float angle, int enemyType)
	{
		AddEnemy(pos, angle, enemyType, 3f, -1);
	}

	public void AddEnemy(Vector2 pos, float angle, int enemyType, float health)
	{
		AddEnemy(pos, angle, enemyType, health, -1);
	}

	public void AddEnemy(Vector2 pos, float angle, int enemyType, float health, int follow)
	{
		if (enemies.Count >= maxEnemies || follow >= 0)
		{
			return;
		}
		bloom(pos);
		int target = ((enemyType != 7 && enemyType != 8) ? newTarget(includeColony: true) : newTarget(includeColony: false));
		Enemy enemy = new Enemy();
		switch (enemyType)
		{
		case 0:
			enemy.Initialize(txAsteroid, txNoise, txDots, pos, new Vector2((float)random.Next(-300, 300) / 100f, (float)random.Next(-300, 300) / 100f), (float)random.Next(400) / 100f, enemyType, target, txArrow, random.Next(), health);
			break;
		case 1:
			enemy.Initialize(txEnemyClass01, txRing, txSparks, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 2:
			enemy.Initialize(txEnemyClass02, txRing, txSparks, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 12:
			enemy.Initialize(txEnemyClass12, txRing, txSparks, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 3:
			enemy.Initialize(txEnemyClass03, txRingClouds, txSparks, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 4:
			enemy.Initialize(txEnemyClass03, txRingClouds, txRingClouds, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 6:
			enemy.Initialize(txEnemyClass06, txRingClouds, txRingClouds, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 7:
			enemy.Initialize(txEnemyClass07, txRingClouds, txRingClouds, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 8:
			enemy.Initialize(txEnemyClass08, txRingClouds, txRingClouds, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		case 9:
			enemy.Initialize(txEnemyClass08, txSparks, txSparks, pos, enemyType, enemies.Count, txArrow, random.Next());
			break;
		case 11:
			enemy.Initialize(txEnemyClass06, txSparks, txEmpty, pos, enemyType, enemies.Count, txEmpty, random.Next());
			break;
		case 100:
			enemy.Initialize(txEnemyClass01, txRingClouds, txRingClouds, pos, Vector2.Zero, (float)random.Next(150, 200) / 100f, enemyType, target, txEmpty, random.Next());
			break;
		default:
			enemy.Initialize(txEnemyClass01, txRing, txSparks, pos, Vector2.Zero, (float)random.Next(650, 700) / 100f, enemyType, target, txArrow, random.Next());
			break;
		}
		enemy.scion = follow;
		enemy.angle = angle;
		if (enemies.Count < topEnemies)
		{
			if (enemyType > 0)
			{
				enemies.Add(enemy);
			}
			else if (random.Next(100) < 25)
			{
				enemies.Add(enemy);
			}
		}
		enemy = null;
	}

	public void UpdateEnemies(GameTime gameTime)
	{
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Active)
			{
				player[i].wasActive = true;
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			int num = enemies[i].pnumber;
			Vector2 vector = Vector2.Zero;
			if (num < 0 && num != -999)
			{
				if (gameState == GameState.Campaign)
				{
					vector = colony.position;
				}
				else
				{
					num = random.Next(4);
					enemies[i].pnumber = num;
				}
			}
			if (num >= 0)
			{
				num = (int)MathHelper.Clamp(enemies[i].pnumber, 0f, 3f);
				if (gameState != GameState.Challenge)
				{
					if (!player[num].Active)
					{
						for (int j = 0; j < 3; j++)
						{
							if (player[j].Active)
							{
								num = j;
							}
						}
					}
					vector = player[num].position;
				}
				else
				{
					num = 0;
					vector = playerChallenge.position;
				}
			}
			if (numPlayers <= 0 || num == -999)
			{
				if (num != -999)
				{
					vector = new Vector2(random.Next(-540, 1180), random.Next(-260, 980));
				}
				enemies[i].pnumber = -999;
				vector = enemies[i].destiny;
			}
			if (gameStatePlay == GameState.Challenge)
			{
				vector = playerChallenge.position;
			}
			if (enemies[i].enemyType == 0)
			{
				enemies[i].UpdateAsteroid();
			}
			else
			{
				enemies[i].Update(num, vector, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height);
			}
			if (gameState == GameState.Meteroids)
			{
				if (enemies[i].position.X < (float)(-enemies[i].texture.Width))
				{
					enemies[i].position.X = 1280 + enemies[i].texture.Width;
				}
				if (enemies[i].position.X > (float)(1280 + enemies[i].texture.Width))
				{
					enemies[i].position.X = -enemies[i].texture.Width;
				}
				if (enemies[i].position.Y < 0f)
				{
					enemies[i].position.Y = 800f;
				}
				if (enemies[i].position.Y > 800f)
				{
					enemies[i].position.Y = 0f;
				}
			}
			if (enemies[i].spawning >= 1f && enemies[i].Active)
			{
				if (gameState == GameState.Sidescroller)
				{
					enemies[i].position.X--;
					if (enemies[i].position.X < 300f)
					{
						enemies[i].position.X--;
					}
					if (enemies[i].position.X < 100f)
					{
						enemies[i].position.X--;
					}
					if (enemies[i].position.X < 0f)
					{
						enemies[i].position.X--;
					}
					if (enemies[i].position.X < -50f)
					{
						enemies[i].position.X--;
					}
					if (enemies[i].position.X < -100f)
					{
						enemies[i].position.X--;
					}
					if (enemies[i].position.X < -200f)
					{
						enemies[i].position.X -= 10f;
					}
					if (enemies[i].position.X < -500f)
					{
						enemies[i].Active = false;
					}
				}
				switch (enemies[i].enemyType)
				{
				case 4:
					if (frame % enemies[i].spawnRatio == 0 && enemies[i].frozen <= 0f)
					{
						AddEnemy(enemies[i].position, enemies[i].angle, 1);
					}
					break;
				case 6:
				{
					Vector2 position = enemies[i].position;
					if (enemies[i].life % enemies[i].spawnRatio == 0 && enemies[i].Health > 1f && random.Next(100) < 50 && enemies.Count < maxEnemies / 4 && enemies[i].frozen <= 0f)
					{
						position.X += (int)(Math.Cos(enemies[i].angle) * (double)enemies[i].size());
						position.Y += (int)(Math.Sin(enemies[i].angle) * (double)enemies[i].size());
						enemies[i].angle += (float)Math.PI / 4f;
						if (position.X < 1920f && position.X > -640f && position.Y < 1080f && position.Y > -360f)
						{
							AddEnemy(position, enemies[i].angle - (float)Math.PI / 2f, 6);
						}
					}
					if (enemies[i].life > 1560 && enemies[i].Active && enemies[i].frozen <= 0f)
					{
						if (random.Next(100) < 20)
						{
							AddEnemy(enemies[i].position, enemies[i].angle, 1);
							AddEnemy(enemies[i].position, enemies[i].angle, 1);
						}
						enemies[i].Active = false;
					}
					break;
				}
				case 8:
					if (enemies[i].Health <= 0f && random.Next(100) < 50 && enemies[i].frozen <= 0f)
					{
						for (int m = 0; m < 6; m++)
						{
							AddEnemyBullet(enemies[i].position, enemies[i].drawingAngle += (float)Math.PI / 3f, enemies[i].shootingDamage * 0.5f);
						}
					}
					break;
				case 9:
					if (frame % (int)((2f - difficulty) * (2f - difficulty) * 40f) == 0 && enemies[i].frozen <= 0f)
					{
						AddEnemy(enemies[i].position, enemies[i].angle, 11);
					}
					break;
				case 11:
					if (gameState == GameState.Sidescroller || gameState == GameState.Survival || round > 10)
					{
						enemies[i].angle = Math2.TurnToFace(enemies[i].position, vector, enemies[i].angle, 0.025f);
						if (frame % 30 == 0 && random.Next(100) < 10)
						{
							AddEnemyBullet(enemies[i].position, enemies[i].angle, 0.5f);
						}
						if (round > 20 && frame % 50 == 0 && MathHelper.Clamp(round - 5, 0f, 50f) > (float)random.Next(5, 20))
						{
							AddEnemyBullet(enemies[i].position, enemies[i].angle, 0.5f);
						}
					}
					break;
				case 12:
					if (enemies[i].life > 300)
					{
						enemyBullets.Add(new Bullet(-1, "fireball", enemies[i].position, enemies[i].angle, txFireball, Color.White, 3f, new Vector2(1f, 1f), 500, enemies[i].Damage));
						enemies[i].life = 0u;
					}
					break;
				case 99:
					particleSystem.AddTrails(enemies[i].position, txNoise, 1f, 15f, 0.03f, (float)random.Next(3600) / 100f, (float)random.Next(-100, 100) / 1000f, new Color(0.5f, 0.2f, 0.1f, 1f));
					if (enemies[i].jump > 0)
					{
						particleSystem.AddTrails(enemies[i].position, txJump, 2f, 0.01f, 0.02f, (float)random.Next(8000) / 1000f, 0f, Color.White * 0.25f);
					}
					if (enemies[i].jump > 25)
					{
						particleSystem.AddTrails(enemies[i].position, txJump, 2f, 0.01f, 0.03f, (float)random.Next(8000) / 1000f, 0f, Color.White * 0.5f);
					}
					if (enemies[i].jump > 50)
					{
						particleSystem.AddTrails(enemies[i].position, txJump, 2f, 0.01f, 0.04f, (float)random.Next(8000) / 1000f, 0f, Color.White);
					}
					if (enemies[i].jump > 75)
					{
						bloom(enemies[i].position);
					}
					if (enemies[i].jump > 90)
					{
						bloom(enemies[i].position, 20f, 6, 10);
					}
					if (enemies[i].jump > 95)
					{
						for (int k = 0; k < 10; k++)
						{
							AddItem(enemies[i].position + new Vector2(random.Next((int)((float)txEnemyNod.Width * -0.1f), (int)((float)txEnemyNod.Width * 0.1f))));
						}
					}
					if (enemies[i].jump > 97)
					{
						float num2 = (float)random.Next(8000) / 1000f;
						for (int l = 0; l < 8; l++)
						{
							AddEnemyBullet(enemies[i].position, num2 += (float)Math.PI / 4f, 1f);
						}
						bloom(enemies[i].position);
					}
					enemies[i].angle = Math2.TurnToFace(enemies[i].position, colony.position, enemies[i].angle, 1f);
					switch (enemies[i].state)
					{
					case EnemState.ring:
					{
						float num2 = (float)random.Next(8000) / 1000f;
						AddEnemyBullet(enemies[i].position, enemies[i].drawingAngle += 0.26931787f, 0.1f);
						break;
					}
					case EnemState.shoot:
						AddEnemyBullet(txBullet03, enemies[i].position + new Vector2(random.Next((int)((float)txEnemyNod.Width * -0.03f), (int)((float)txEnemyNod.Width * 0.03f))), enemies[i].angle, 0.05f, 200, (float)random.Next(40, 250) / 50f + 25f);
						AddEnemyBullet(txBullet03, enemies[i].position, enemies[i].angle + (float)random.Next(-100, 100) / 5000f, 0.05f, 200, (float)random.Next(50, 300) / 40f + 25f);
						AddEnemyBullet(txBullet03, enemies[i].position, enemies[i].angle, 0.05f, 200, (float)random.Next(70, 350) / 30f + 40f);
						AddEnemyBullet(txBullet03, enemies[i].position, enemies[i].angle, 0.05f, 200, (float)random.Next(70, 350) / 50f + 20f);
						break;
					case EnemState.prepareRing:
					case EnemState.prepare:
						particleSystem.AddTrails(enemies[i].position, txJump, 1.5f, 0.01f, 0.02f, (float)random.Next(8000) / 1000f, 0f, Color.White * 0.25f);
						particleSystem.AddTrails(enemies[i].position, txSpark, 5f, 0.001f, 0.05f, (float)random.Next(8000) / 1000f, 0f, new Color(3f, 2f, 0.5f, 1f));
						particleSystem.AddTrails(enemies[i].position, txRing, 3f, 0.01f, 0.02f, (float)random.Next(8000) / 1000f, 0f, Color.White * 0.25f);
						break;
					case EnemState.spawn:
						if (enemies[i].jump < 1 && enemies[i].frozen <= 0f)
						{
							createEnemies(gameTime, enemies[i].position + new Vector2(random.Next((int)((float)txEnemyNod.Width * -0.4f), (int)((float)txEnemyNod.Width * 0.4f)), random.Next((int)((float)txEnemyNod.Height * -0.25f), (int)((float)txEnemyNod.Height * 0.25f))));
						}
						break;
					case EnemState.snakes:
						if (enemies[i].jump < 1 && enemies[i].frozen <= 0f && frame % 10 == 0)
						{
							AddEnemy(enemies[i].position, enemies[i].angle, 9);
						}
						break;
					}
					break;
				}
				if (enemies[i].isShooting && frame % 10 == 0 && enemies[i].frozen <= 0f)
				{
					AddEnemyBullet(enemies[i].position, enemies[i].drawingAngle + (float)random.Next(-10, 10) / 100f, enemies[i].shootingDamage);
				}
				if (gameState == GameState.Campaign)
				{
					for (int n = 0; n < pickups.Count; n++)
					{
						if (pickups[n].pickupType == Pickup.item.orb && Vector2.Distance(enemies[i].position, pickups[n].position) < (float)(enemies[i].Width + enemies[i].Height) / 4f)
						{
							pickups[n].target = -1;
							if (pickups[n].qeue >= 0)
							{
								bloom(pickups[n].position, 3);
								player[pickups[n].qeue].orbs--;
								pickups[n].qeue = -1;
							}
						}
					}
				}
				for (int n = 0; n < lens[currentLevel].Count; n++)
				{
					if (Vector2.Distance(enemies[i].position, lens[currentLevel][n].position) < enemies[i].size() / 2f && enemies[i].enemyType != 99)
					{
						lens[currentLevel][n].visible = false;
					}
				}
				for (int num3 = 0; num3 < blast.Count; num3++)
				{
					if (!(Vector2.Distance(blast[num3].position, enemies[i].position) <= blast[num3].grow * (float)blast[num3].Width / 2f))
					{
						continue;
					}
					if (blast[num3].type == 0)
					{
						if (enemies[i].enemyType != 99)
						{
							AddExploss(enemies[i].position);
							enemies[i].Health -= 100f;
						}
						else
						{
							enemies[i].Health -= 10f;
						}
					}
					else
					{
						bloom(enemies[i].position);
						enemies[i].frozen = 300f;
					}
				}
				for (int m = 0; m < bullets.Count; m++)
				{
					if (i >= enemies.Count)
					{
						continue;
					}
					if (Vector2.Distance(enemies[i].position, bullets[m].position) < enemies[i].size() / 2f)
					{
						enemies[i].Health -= bullets[m].Damage;
						AddExploss(bullets[m].position);
						particleSystem.createExplosion(bullets[m].position, camera.getScreenPosition(bullets[m].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
						if (enemies[i].enemyType == 0)
						{
							AddItem(enemies[i].position);
						}
						if (bullets[m].id != -1 && currentLevel != 14)
						{
							if (enemies[i].Health <= 0f)
							{
								player[bullets[m].id].numberOfKills++;
							}
							if (player[bullets[m].id].numberOfKills >= 100 && player[bullets[m].id].numberOfKills < 110 && awards.Unlock("100 down"))
							{
								writeAwards();
							}
							if (player[bullets[m].id].numberOfKills >= 1000 && player[bullets[m].id].numberOfKills < 1010 && awards.Unlock("1000 down"))
							{
								writeAwards();
							}
							if (player[bullets[m].id].numberOfKills >= 10000 && player[bullets[m].id].numberOfKills < 10010 && awards.Unlock("10000 down"))
							{
								writeAwards();
							}
							player[bullets[m].id].score += enemies[i].score;
							if (enemies[i].enemyType != 99)
							{
								player[bullets[m].id].experience += enemies[i].experience;
							}
						}
						if (random.Next(100) < 50)
						{
							bloom(bullets[m].position, 3);
						}
						else
						{
							bloom(bullets[m].position, 1);
						}
						bullets[m].Active = false;
						m = bullets.Count + 1;
					}
					if (enemies[i].enemyType != 9)
					{
						continue;
					}
					for (int num4 = 0; num4 < enemies[i].followers.Count; num4++)
					{
						if (enemies[i].followers.Count > 0 && bullets.Count > 0 && m < bullets.Count && Vector2.Distance(enemies[i].followers[num4].position, bullets[m].position) < 10f)
						{
							enemies[i].followers[num4].Health -= bullets[m].Damage + (float)(int)((float)(int)player[0].level * 0.5f);
							AddExploss(bullets[m].position);
							particleSystem.createExplosion(bullets[m].position, camera.getScreenPosition(bullets[m].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
							if (bullets[m].id != -1)
							{
								player[bullets[m].id].score += enemies[i].followers[num4].score;
								player[bullets[m].id].experience += enemies[i].followers[num4].experience;
							}
							if (random.Next(100) < 50)
							{
								bloom(bullets[m].position, 3);
							}
							else
							{
								bloom(bullets[m].position, 1);
							}
							bullets[m].Active = false;
							m = bullets.Count + 1;
						}
					}
				}
				if (i < enemies.Count && gameState != GameState.Challenge)
				{
					for (int n = 0; n < 4; n++)
					{
						if (!player[n].Active)
						{
							continue;
						}
						if (Vector2.Distance(enemies[i].position, player[n].position) <= player[n].size() * 0.25f + enemies[i].size() * 0.25f)
						{
							if (enemies[i].enemyType != 99)
							{
								if (blast.Count == 0 && player[n].Health > 0f)
								{
									createBlast(n + 10);
								}
								player[n].hit(enemies[i].Damage * (GOdifficulty / 100f));
								enemies[i].Health = 0f;
								enemies[i].Active = false;
							}
							bloom(enemies[i].position);
						}
						if (enemies[i].enemyType != 9)
						{
							continue;
						}
						for (int num4 = 0; num4 < enemies[i].followers.Count; num4++)
						{
							if (Vector2.Distance(enemies[i].followers[num4].position, player[n].position) <= player[n].size() * 0.25f + enemies[i].followers[num4].size() * 0.25f)
							{
								if (blast.Count == 0 && player[n].Health > 0f)
								{
									createBlast(n + 10);
								}
								player[n].hit(enemies[i].Damage * (GOdifficulty / 100f));
								enemies[i].followers[num4].Health = 0f;
								enemies[i].followers[num4].Active = false;
							}
						}
					}
				}
				if (enemies[i].Active && gameState == GameState.Campaign)
				{
					if (Vector2.Distance(enemies[i].position, colony.position) < (float)(enemies[i].Width / 4))
					{
						colony.healthTarget -= enemies[i].Health;
						colony.damaged = true;
						enemies[i].Health = 0f;
						enemies[i].Active = false;
						if (colonyHit <= 1)
						{
							colonyHit = 1000;
						}
					}
					if (enemies[i].enemyType == 9)
					{
						for (int num4 = 0; num4 < enemies[i].followers.Count; num4++)
						{
							if (Vector2.Distance(enemies[i].followers[num4].position, colony.position) < (float)(enemies[i].Width / 4))
							{
								colony.healthTarget -= enemies[i].followers[num4].Health;
								colony.damaged = true;
								enemies[i].followers[num4].Health = 0f;
								enemies[i].followers[num4].Active = false;
								if (colonyHit <= 1)
								{
									colonyHit = 1000;
								}
							}
						}
					}
				}
			}
			if (i >= enemies.Count || enemies[i].Active)
			{
				continue;
			}
			if (enemies[i].enemyType != 99)
			{
				if (gameState != GameState.Challenge)
				{
					for (int k = 0; k < enemies[i].Energy; k++)
					{
						AddItem(Pickup.item.coins, enemies[i].position, k * 10);
					}
					int num5 = random.Next(100);
					int num6 = 2;
					if (difficulty < 1f)
					{
						num6 = 6;
					}
					if (difficulty > 1f)
					{
						num6 = 1;
					}
					if (gameState == GameState.Sidescroller)
					{
						if (num6 > 2)
						{
							num6 /= 3;
						}
						else
						{
							num6 /= 2;
						}
					}
					if (num5 < 2)
					{
						if (random.Next(100) > 25)
						{
							if (gameState == GameState.Sidescroller)
							{
								if (random.Next(100) < 40)
								{
									AddItem(Pickup.item.health, enemies[i].position);
								}
							}
							else
							{
								AddItem(Pickup.item.health, enemies[i].position);
							}
						}
						else if (enemies[i].enemyType > 1 && Vector2.Distance(enemies[i].position, colony.position) > 200f && gameState == GameState.Campaign)
						{
							AddItem(Pickup.item.orb, enemies[i].position);
						}
					}
					num6 = 96;
					if (difficulty < 1f)
					{
						num6 = 90;
					}
					if (difficulty > 1f)
					{
						num6 = 98;
					}
					if (gameState == GameState.Sidescroller)
					{
						if (difficulty > 1f)
						{
							num6 = 98;
						}
						if (num6 == 96)
						{
							num6++;
						}
					}
					if (num5 > num6)
					{
						if (random.Next(100) > 35)
						{
							if (gameState == GameState.Sidescroller)
							{
								if (random.Next(100) < 20)
								{
									AddItem(Pickup.item.emp, enemies[i].position);
								}
							}
							else
							{
								AddItem(Pickup.item.emp, enemies[i].position);
							}
						}
						else if (gameState == GameState.Sidescroller)
						{
							if (random.Next(100) < 20)
							{
								AddItem(Pickup.item.bomb, enemies[i].position);
							}
						}
						else
						{
							AddItem(Pickup.item.bomb, enemies[i].position);
						}
					}
				}
				particleSystem.createExplosion(enemies[i].position, camera.getScreenPosition(enemies[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				if (enemies[i].enemyType == 2 || enemies[i].enemyType == 3)
				{
					particleSystem.createExplosion(enemies[i].position + new Vector2(random.Next(-50, 50), random.Next(-50, 50)), camera.getScreenPosition(enemies[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
				if (enemies[i].enemyType == 3)
				{
					particleSystem.createExplosion(enemies[i].position + new Vector2(random.Next(-100, 100), random.Next(-100, 100)), camera.getScreenPosition(enemies[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
			switch (enemies[i].enemyType)
			{
			case 0:
				if (enemies[i].maximunHealth > 0.1f)
				{
					int num7 = random.Next(4) + 2;
					float health = enemies[i].maximunHealth / (float)num7;
					for (int num8 = 0; num8 < num7; num8++)
					{
						Vector2 vector2 = enemies[i].position + new Vector2(random.Next(-num7 * 25, num7 * 25), random.Next(-num7 * 25, num7 * 25));
						particleSystem.createExplosion(vector2, camera.getScreenPosition(vector2, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
						AddEnemy(vector2, (float)random.Next(720) / 100f, 0, health);
					}
				}
				break;
			case 9:
			case 11:
				createBlast(-500, enemies[i].position);
				break;
			case 99:
				if (random.Next(100) < 50)
				{
					switch ((ushort)random.Next(4))
					{
					case 1:
						createBlast(-500, enemies[i].position);
						break;
					case 2:
						AddExploss(enemies[0].position);
						break;
					default:
						bloom(enemies[0].position);
						break;
					}
				}
				break;
			}
			if (enemies[i].enemyType != 99)
			{
				bloom(enemies[i].position);
				enemies.RemoveAt(i);
			}
		}
	}

	private void AddItem(Vector2 pos)
	{
		AddItem(Pickup.item.coins, pos, 50);
	}

	private void AddItem(Pickup.item type, Vector2 pos)
	{
		AddItem(type, pos, 50);
	}

	private void AddItem(Pickup.item type, Vector2 pos, int space)
	{
		float num = 1000000f;
		switch (type)
		{
		case Pickup.item.bomb:
			pickups.Add(new Pickup(txBomb, pos, pos + new Vector2(random.Next(-space, space), random.Next(-space, space)), type, 1, 1, random.Next(10000), base.GraphicsDevice));
			break;
		case Pickup.item.emp:
			pickups.Add(new Pickup(txEmp, pos, pos + new Vector2(random.Next(-space, space), random.Next(-space, space)), type, 1, 1, random.Next(10000), base.GraphicsDevice));
			break;
		case Pickup.item.relic:
			pickups.Add(new Pickup(txRelic, pos, pos, type, 15, 2, random.Next(10000), base.GraphicsDevice));
			break;
		case Pickup.item.health:
			pickups.Add(new Pickup(txHealth, pos, pos + new Vector2(random.Next(-space, space), random.Next(-space, space)), type, 1, 1, random.Next(10000), base.GraphicsDevice));
			break;
		case Pickup.item.pathNode:
			pickups.Add(new Pickup(txRing, pos, pos, type, 1, 1, random.Next(10000), base.GraphicsDevice));
			break;
		case Pickup.item.orb:
		{
			if (!(Vector2.Distance(pos, colony.position) > 200f) || gameState != GameState.Campaign)
			{
				break;
			}
			for (int i = 0; i < pickups.Count; i++)
			{
				float num2 = Vector2.Distance(pos, pickups[i].position);
				if (num2 < num)
				{
					num = num2;
				}
			}
			if (num >= 200f || pickups.Count == 0)
			{
				pickups.Add(new Pickup(txOrbs, pos, pos + new Vector2(random.Next(-space, space), random.Next(-space, space)), type, 1, 1, random.Next(10000), base.GraphicsDevice));
			}
			break;
		}
		default:
			if (gameState != GameState.Challenge)
			{
				coins.Add(new Pickup(txCoins, pos, pos + new Vector2(random.Next(-space, space), random.Next(-space, space)), Pickup.item.coins, 1, 1, random.Next(10000), base.GraphicsDevice));
			}
			else
			{
				coins.Add(new Pickup(txOrbs, pos, pos, Pickup.item.coins, 1, 1, 0, base.GraphicsDevice));
			}
			break;
		}
	}

	public void AddMisiles(int id)
	{
		Texture2D texture = txBullet01;
		float num = camera.getScreenPosition(player[(int)MathHelper.Clamp(id, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
		float pan = MathHelper.Clamp(num + (float)random.Next(-50, 50) / 100f, -1f, 1f);
		laserSound.Play(GOsoundFXvolume / 1000f, (float)random.Next(-50, 0) / 100f, pan);
		byte b = 1;
		if (gameState == GameState.ChubbyRain)
		{
			b = 3;
		}
		float num2 = (float)random.Next(-700, 700) / 100f;
		Vector2 position = player[id].position;
		position = new Vector2(position.X + (float)Math.Cos(num2) * 500f, position.Y + (float)Math.Sin(num2) * 500f);
		ushort num3 = 2;
		for (int i = 0; i < num3; i++)
		{
			bullets.Add(new Bullet(id, "Pmissile", player[id].position, (float)random.Next(628) / 100f, texture, new Color(0.3f, 0.7f, 1f, 1f), (float)random.Next(40, 80) / 10f, new Vector2(1f, 1f) * (int)b, random.Next(80, 120), player[id].shootDamage, position));
		}
	}

	public void AddRadial(int id)
	{
		Texture2D texture = txBullet01;
		float num = camera.getScreenPosition(player[(int)MathHelper.Clamp(id, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
		float pan = MathHelper.Clamp(num + (float)random.Next(-50, 50) / 100f, -1f, 1f);
		laserSound.Play(GOsoundFXvolume / 1000f, (float)random.Next(-100, -50) / 100f, pan);
		byte b = 1;
		if (gameState == GameState.ChubbyRain)
		{
			b = 3;
		}
		float num2 = (float)random.Next(-700, 700) / 100f;
		Vector2 position = player[id].position;
		position = new Vector2(position.X + (float)Math.Cos(num2) * 500f, position.Y + (float)Math.Sin(num2) * 500f);
		bloom(player[id].position);
		ushort num3 = (ushort)(40 + player[id].level * 2);
		for (int i = 0; i < num3; i++)
		{
			bullets.Add(new Bullet(id, "Radial", player[id].position, (float)random.Next(628) / 100f, texture, new Color(0.3f, 0.7f, 1f, 1f), (float)random.Next(40, 80) / 10f, new Vector2(1f, 1f) * (int)b, (int)MathHelper.Clamp(random.Next(400 + player[id].level * 60, 500 + player[id].level * 70), 500f, 1200f), player[id].shootDamage * 5f, position));
		}
		bloom(player[id].position);
		for (int i = 0; i < 628; i += 20)
		{
			bullets.Add(new Bullet(id, "NORMAL", player[id].position + new Vector2((float)(Math.Cos((float)i / 100f - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin((float)i / 100f - (float)Math.PI / 2f) * 20.0)), (float)i / 100f, texture, player[id].shootColor, (float)random.Next(-100, 100) / 100f + 7.5f, new Vector2(4f, 1f), 40 * b, player[id].shootDamage * 2f));
		}
		bloom(player[id].position);
	}

	public void AddBullet(bool isPlayer, int id)
	{
		byte b = 1;
		if (gameState == GameState.ChubbyRain)
		{
			b = 3;
		}
		if (gameState == GameState.Sidescroller)
		{
			b = 3;
		}
		Texture2D texture = txBullet01;
		if (isPlayer)
		{
			switch (player[id].shootingType)
			{
			case "WIDE":
			{
				float value = camera.getScreenPosition(player[(int)MathHelper.Clamp(id, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
				float pan = MathHelper.Clamp(value, -1f, 1f);
				laserSound.Play(GOsoundFXvolume / 100f, (float)random.Next(-100, -50) / 100f, pan);
				Vector2 position = player[id].position;
				position = new Vector2(position.X + (float)Math.Cos(player[id].angle) * 500f, position.Y + (float)Math.Sin(player[id].angle) * 500f);
				ushort num = (ushort)random.Next(player[id].level);
				if (num < 1)
				{
					num = 1;
				}
				for (int i = 0; i < num; i++)
				{
					bullets.Add(new Bullet(id, "Pmissile", player[id].position, (float)random.Next(628) / 100f, texture, new Color(0.3f, 0.7f, 1f, 1f), (float)random.Next(40, 80) / 10f, new Vector2(1f, 1f) * (int)b, random.Next(80, 120) * b, player[id].shootDamage, position));
				}
				bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle - (float)Math.PI / 2f) * 20.0)), 1);
				bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle + (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle + (float)Math.PI / 2f) * 20.0)), 1);
				break;
			}
			case "LONG":
			{
				texture = txBullet03;
				float value = camera.getScreenPosition(player[(int)MathHelper.Clamp(id, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
				float pan = MathHelper.Clamp(value, -1f, 1f);
				laserSound.Play(GOsoundFXvolume / 100f, (float)random.Next(-10, 10) / 100f, pan);
				bullets.Add(new Bullet(id, player[id].shootingType, player[id].position, player[id].angle, texture, player[id].shootColor, 50f, new Vector2(2f, 1f), 20 * b, player[id].shootDamage * 0.5f));
				bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle - (float)Math.PI / 2f) * 20.0)), 1);
				bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle + (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle + (float)Math.PI / 2f) * 20.0)), 1);
				break;
			}
			case "NORMAL":
			{
				float value = camera.getScreenPosition(player[(int)MathHelper.Clamp(id, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
				float pan = MathHelper.Clamp(value, -1f, 1f);
				laserSound.Play(GOsoundFXvolume / 100f, (float)random.Next(-10, 10) / 100f, pan);
				switch (player[id].level)
				{
				case 0:
					bullets.Add(new Bullet(id, player[id].shootingType, player[id].position + new Vector2((float)(Math.Cos(player[id].angle) * 5.0), (float)(Math.Sin(player[id].angle) * 5.0)), player[id].angle, texture, player[id].shootColor, (float)random.Next(-100, 100) / 100f + 25f, new Vector2(4f, 3f), 10 * b, player[id].shootDamage * 1.5f));
					bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle) * 5.0), (float)(Math.Sin(player[id].angle) * 5.0)), 1);
					break;
				case 1:
					bullets.Add(new Bullet(id, player[id].shootingType, player[id].position + new Vector2((float)(Math.Cos(player[id].angle - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle - (float)Math.PI / 2f) * 20.0)), player[id].angle, texture, player[id].shootColor, (float)random.Next(-100, 100) / 100f + 25f, new Vector2(4f, 3f), 10 * b, player[id].shootDamage));
					bullets.Add(new Bullet(id, player[id].shootingType, player[id].position + new Vector2((float)(Math.Cos(player[id].angle + (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle + (float)Math.PI / 2f) * 20.0)), player[id].angle, texture, player[id].shootColor, (float)random.Next(-100, 100) / 100f + 25f, new Vector2(4f, 3f), 10 * b, player[id].shootDamage));
					bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle - (float)Math.PI / 2f) * 20.0)), 1);
					bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle + (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle + (float)Math.PI / 2f) * 20.0)), 1);
					break;
				default:
					bullets.Add(new Bullet(id, player[id].shootingType, player[id].position + new Vector2((float)(Math.Cos(player[id].angle - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle - (float)Math.PI / 2f) * 20.0)), player[id].angle, texture, player[id].shootColor, (float)random.Next(-100, 100) / 100f + 25f, new Vector2(4f, 3f), 10 * b, player[id].shootDamage * 0.5f));
					bullets.Add(new Bullet(id, player[id].shootingType, player[id].position + new Vector2((float)(Math.Cos(player[id].angle + (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle + (float)Math.PI / 2f) * 20.0)), player[id].angle, texture, player[id].shootColor, (float)random.Next(-100, 100) / 100f + 25f, new Vector2(4f, 3f), 10 * b, player[id].shootDamage * 0.5f));
					bullets.Add(new Bullet(id, player[id].shootingType, player[id].position + new Vector2((float)(Math.Cos(player[id].angle) * 5.0), (float)(Math.Sin(player[id].angle) * 5.0)), player[id].angle, texture, player[id].shootColor, (float)random.Next(-100, 100) / 100f + 25f, new Vector2(4f, 3f), 10 * b, player[id].shootDamage));
					bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle - (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle - (float)Math.PI / 2f) * 20.0)), 1);
					bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle + (float)Math.PI / 2f) * 20.0), (float)(Math.Sin(player[id].angle + (float)Math.PI / 2f) * 20.0)), 1);
					break;
				}
				break;
			}
			default:
			{
				float value = camera.getScreenPosition(player[(int)MathHelper.Clamp(id, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
				float pan = MathHelper.Clamp(value, -1f, 1f);
				laserSound.Play(GOsoundFXvolume / 100f, (float)random.Next(-10, 10) / 100f, pan);
				bullets.Add(new Bullet(id, player[id].shootingType, player[id].position, player[id].angle, texture, player[id].shootColor, (float)random.Next(-200, 200) / 100f + 40f, new Vector2(4f, 1f), 12 * b, player[id].shootDamage));
				bloom(player[id].position + new Vector2((float)(Math.Cos(player[id].angle) * 20.0), (float)(Math.Sin(player[id].angle) * 20.0)), 1);
				break;
			}
			}
		}
		else
		{
			float value = camera.getScreenPosition(player[(int)MathHelper.Clamp(id, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
			float pan = MathHelper.Clamp(value, -1f, 1f);
			laserSound.Play(GOsoundFXvolume / 100f, (float)random.Next(-10, 10) / 100f, pan);
			bullets.Add(new Bullet(-1, "", construction[id].position, construction[id].angle, txBullet01, construction[id].shootColor, (float)random.Next(-200, 200) / 100f + 25f, new Vector2(3.5f, 1f), 12 * b, construction[id].Damage));
			if (construction[id].level > 1)
			{
				bullets.Add(new Bullet(-1, "", construction[id].position, construction[id].angle + (float)random.Next(-400, 400) / 2000f, txBullet01, new Color(0.3f, 0.7f, 1f, 1f), (float)random.Next(-200, 200) / 200f + 20f, new Vector2(1f, 0.75f), 10 * b, construction[id].Damage));
			}
			if (construction[id].level > 2)
			{
				bullets.Add(new Bullet(-1, "", construction[id].position, construction[id].angle + (float)random.Next(-800, 800) / 2000f, txBullet01, new Color(0.1f, 0.5f, 0.9f, 0.5f), (float)random.Next(-200, 200) / 200f + 20f, new Vector2(1f, 0.75f), 8 * b, construction[id].Damage));
			}
		}
	}

	public void UpdateFXs(GameTime gameTime)
	{
		particleSystem.UpdateParticles(gameTime);
		for (int i = 0; i < exploss.Count; i++)
		{
			exploss[i].Update(gameTime);
			if (!exploss[i].Active)
			{
				exploss.RemoveAt(i);
			}
		}
		UpdateLens();
	}

	public void DrawFXs(SpriteBatch spriteBatch)
	{
		for (int i = 0; i < exploss.Count; i++)
		{
			exploss[i].Draw(spriteBatch);
		}
	}

	public void AddExploss(Vector2 position)
	{
		Animation animation = new Animation();
		animation.Initialize(txExploss, Vector2.Zero, 64, 64, 16, 1, 10, Color.White, (float)random.Next(2, 7) / 4f, looping: false);
		AnimatedSprite animatedSprite = new AnimatedSprite();
		animatedSprite.Initialize(animation, position, (float)random.Next(700) / 100f);
		exploss.Add(animatedSprite);
		animatedSprite = null;
	}

	public void AddConstruction(constructionType type, int playerID)
	{
		AddConstruction(type, playerID, player[playerID].position);
	}

	public void AddConstruction(constructionType type, int playerID, Vector2 pos)
	{
		int num = 31;
		int num2 = 28;
		Vector2 vector = new Vector2(((float)(int)(pos.X / (float)num) + 0.5f) * (float)num, ((float)(int)(pos.Y / (float)num2) + 0.5f) * (float)num2);
		if ((int)(vector.Y / (float)num2) % 2 == 0)
		{
			vector.X += num / 2;
		}
		Texture2D t = txEmpty;
		Texture2D t2 = txEmpty;
		switch (type)
		{
		case constructionType.barrier:
			t = txHexagonBarrier;
			t2 = txHexagonBarrier;
			break;
		case constructionType.turret:
			t = txTurretBase;
			t2 = txTurretGun;
			break;
		case constructionType.hive:
			t = txHive;
			t2 = txHive;
			break;
		case constructionType.sanctuary:
			t = txSanctuary;
			t2 = txSanctuary;
			break;
		case constructionType.drone:
			t = txEmpty;
			t2 = txDrone;
			break;
		}
		bool flag = false;
		if (type != constructionType.drone)
		{
			for (int i = 0; i < construction.Count; i++)
			{
				if (construction[i].position == vector)
				{
					flag = true;
				}
			}
			if (!flag && type != constructionType.nothing && player[playerID].credits > Construction.Cost(type) * player[playerID].creditsMul)
			{
				player[playerID].credits -= Construction.Cost(type) * player[playerID].creditsMul;
				construction.Add(new Construction(vector, type, t, t2));
				bloom(vector, 3);
			}
		}
		else
		{
			construction.Add(new Construction(pos, type, t, t2));
			bloom(pos);
		}
	}

	private void bloom(Vector2 pos)
	{
		bloom(pos, (float)random.Next(40) / 10f, random.Next(4), random.Next(7));
	}

	private void bloom(Vector2 pos, float size)
	{
		bloom(pos, size, random.Next(4), random.Next(7));
	}

	private void bloom(Vector2 pos, ushort type)
	{
		bloom(pos, (float)random.Next(40) / 10f, type, random.Next(7));
	}

	private void bloom(Vector2 pos, float size, int type)
	{
		bloom(pos, size, type, random.Next(7));
	}

	private void bloom(Vector2 pos, float size, int type, int intensity)
	{
		switch (type)
		{
		case 1:
			particleSystem.AddTrails(pos, txSpark, 0f, size + (float)random.Next(20) / 10f, 0.1f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txSpark, 0f, size + (float)random.Next(20) / 10f, 0.2f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txSpark, 0f, size + (float)random.Next(20) / 10f, 0.4f, 0f, 0f, Color.LightCyan * intensity);
			break;
		case 2:
			particleSystem.AddTrails(pos, txDots, 0f, size, 0.01f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txDots, 0f, size, 0.05f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txExpansion, 0f, size, 0.1f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txExpansion, 0f, size, 0.3f, 0f, 0f, Color.LightCyan * intensity);
			break;
		case 3:
			particleSystem.AddTrails(pos, txDots, 0f, size, 0.01f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txDots, 0f, size, 0.05f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txDots, 0f, size, 0.1f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txSpark, 0f, size, 0.3f, 0f, 0f, Color.LightCyan * intensity);
			break;
		case 4:
		{
			for (int i = 1; i < 20; i++)
			{
				particleSystem.AddTrails(pos, txDots, 0f, size * 2f, (float)i / 100f, 0f, 0f, Color.LightCyan * intensity);
			}
			break;
		}
		case 5:
		{
			for (int i = 1; i < 5; i++)
			{
				particleSystem.AddTrails(pos, txDots, 0f, size * 0.5f, (float)i / 20f, 0f, 0f, Color.LightCyan * intensity);
			}
			break;
		}
		case 6:
			particleSystem.AddTrails(pos, txDots, 0f, size, 0.1f, (float)random.Next(-720, 720) / 100f, 0f, Color.LightCyan * intensity * 0.5f);
			break;
		case 7:
		{
			for (int i = 1; i < 3; i++)
			{
				particleSystem.AddTrails(pos, txDots, 0f, size * 0.5f, (float)i / 20f, (float)random.Next(720) / 100f, 0f, Color.LightCyan * intensity);
			}
			break;
		}
		default:
			particleSystem.AddTrails(pos, txExpansion, 0f, size, 0.1f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txExpansion, 0f, size, 0.15f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txExpansion, 0f, size, 0.2f, 0f, 0f, Color.LightCyan * intensity);
			particleSystem.AddTrails(pos, txExpansion, 0f, size, 0.4f, 0f, 0f, Color.LightCyan * intensity);
			break;
		}
	}

	public void UpdateBullet(GameTime gameTime)
	{
		for (int i = 0; i < bullets.Count; i++)
		{
			if (bullets[i].id >= 0 && bullets[i].id < 4)
			{
				bullets[i].Update(player[bullets[i].id].position);
			}
			else
			{
				bullets[i].Update(Vector2.One * 400f);
			}
			if (random.Next(200) == 1)
			{
				bloom(bullets[i].position, 1);
			}
			if (random.Next(200) == 198)
			{
				bloom(bullets[i].position, 3);
			}
			if (gameState == GameState.Sidescroller && camera.getScreenPosition(bullets[i].position, base.GraphicsDevice).X > (float)(base.GraphicsDevice.Viewport.Width + bullets[i].Width))
			{
				bullets[i].Active = false;
			}
			if (gameState == GameState.ChubbyRain && camera.getScreenPosition(bullets[i].position, base.GraphicsDevice).Y < (float)(-bullets[i].Height))
			{
				bullets[i].Active = false;
			}
			if (bullets[i].Active)
			{
				continue;
			}
			string type = bullets[i].type;
			if (type != null && type == "Pmissile")
			{
				particleSystem.createExplosion(bullets[i].position, camera.getScreenPosition(bullets[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
			}
			else if (random.Next(100) < 20)
			{
				if (random.Next(20) < 10)
				{
					bloom(bullets[i].position, 1);
				}
				else
				{
					bloom(bullets[i].position, 4f, 6, 1);
				}
			}
			bullets.RemoveAt(i);
		}
	}

	public void AddEnemyBullet(Vector2 pos, float angle, float damage)
	{
		AddEnemyBullet(pos, angle, damage, 100);
	}

	public void AddEnemyBullet(Vector2 pos, float angle, float damage, int life)
	{
		AddEnemyBullet(txBullet01, pos, angle, damage, life, (float)random.Next(-20, 20) / 100f + 8f);
	}

	public void AddEnemyBullet(Texture2D texture, Vector2 pos, float angle, float damage, int life, float speed)
	{
		enemyBullets.Add(new Bullet(-1, "", pos, angle, texture, new Color(1f, 0.05f, 0f, 1f), speed / 2f, new Vector2(2f, 2f), life * 2, damage));
	}

	public void UpdateEnemyBullet(GameTime gameTime)
	{
		for (int i = 0; i < enemyBullets.Count; i++)
		{
			enemyBullets[i].Update(player[0].position);
			for (int j = 0; j < construction.Count; j++)
			{
				if (Vector2.Distance(construction[j].position, enemyBullets[i].position) < (float)construction[j].Width / 2f)
				{
					construction[j].Health -= enemyBullets[i].Damage;
					enemyBullets[i].Active = false;
					particleSystem.createExplosion(enemyBullets[i].position, camera.getScreenPosition(enemyBullets[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
			for (int j = 0; j < 3; j++)
			{
				if (player[j].Active && Vector2.Distance(player[j].position, enemyBullets[i].position) < (float)player[j].Width / 2f && player[j].beingHit < 1)
				{
					player[j].hit(enemyBullets[i].Damage);
					enemyBullets[i].Active = false;
					particleSystem.createExplosion(enemyBullets[i].position, camera.getScreenPosition(enemyBullets[i].position, base.GraphicsDevice), base.GraphicsDevice, GOsoundFXvolume);
				}
			}
			if (enemyBullets[i].Active && Vector2.Distance(colony.position, enemyBullets[i].position) < (float)(enemyBullets[i].Width / 2))
			{
				colony.healthTarget -= enemyBullets[i].Damage;
				colony.damaged = true;
				enemyBullets[i].Active = false;
				if (colonyHit <= 1)
				{
					colonyHit = 1000;
				}
			}
			if (!enemyBullets[i].Active)
			{
				enemyBullets.RemoveAt(i);
			}
		}
	}

	public void UpdateConstructions(GameTime gameTime)
	{
		for (int i = 0; i < construction.Count; i++)
		{
			float num = 100000f;
			for (int j = 0; j < enemies.Count; j++)
			{
				float num2 = Vector2.Distance(enemies[j].position, construction[i].position);
				if (num2 < num)
				{
					num = num2;
					construction[i].target = enemies[j].position;
				}
				if (num2 <= construction[i].Scale() / 4f + enemies[j].size() / 4f)
				{
					float health = construction[i].Health;
					construction[i].Health -= enemies[j].Health;
					enemies[j].Health -= health;
					if (enemies[j].Health < 0f)
					{
						enemies[j].Active = false;
					}
				}
			}
			construction[i].Update(Vector2.Distance(construction[i].target, construction[i].position) < 300f);
			switch (construction[i].type)
			{
			case constructionType.sanctuary:
			{
				for (int j = 0; j < player.Length; j++)
				{
					if (player[j].Active && player[j].Health < player[j].maximunHealth && construction[i].frame > 30f && Vector2.Distance(player[j].position, construction[i].position) < player[j].size() / 2f + construction[i].Scale() / 2f)
					{
						construction[i].Repair(j, player[j].position);
						player[j].Health = player[j].Health + 0.1f;
						particleSystem.AddItemTrails(construction[i].position, player[j].position, txRingClouds, 0.01f, 2f, 0.05f + (float)random.Next(5, 15) / 100f, (float)random.Next(3600) / 100f, 0.05f, player[j].shootColor);
					}
				}
				break;
			}
			case constructionType.hive:
				if ((int)construction[i].frame % construction[i].rate == 0)
				{
					AddConstruction(constructionType.drone, i, construction[i].position);
				}
				break;
			}
			if (construction[i].type == constructionType.turret && enemies.Count > 0 && Vector2.Distance(construction[i].target, construction[i].position) < 250f && frame % 20 == i % 10)
			{
				AddBullet(isPlayer: false, i);
			}
			if (!construction[i].Active)
			{
				construction.RemoveAt(i);
			}
		}
	}

	public void UpdateLevel(GameTime gameTime)
	{
		if (enemies.Count == 0 && currentLevel == 13 && round == 0)
		{
			enemies.Add(new Enemy(txEnemyNod, txBlast, txRingClouds, new Vector2(1600f, 900f), new Vector2(1600f, 900f), 0.1f, 99, -1, txArrow, 1, 1000f));
			colony.position = new Vector2(500f, 260f);
		}
		if (currentLevel == 13)
		{
			lens[currentLevel][1].position = colony.position;
			lens[currentLevel][2].position = enemies[0].position;
		}
		int num = 0;
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Active)
			{
				num++;
			}
		}
		if (maxEnemies > 30 + num * 20 + (int)(20 * round))
		{
			maxEnemies = 30 + num * 20 + (int)(20 * round);
		}
		if (maxEnemies < 20)
		{
			maxEnemies = 20;
		}
		if (currentLevel == 13)
		{
			maxEnemies = (int)frame / 300 + (int)round / 10;
		}
		if (maxEnemies > topEnemies)
		{
			maxEnemies = topEnemies;
		}
		waveTotal--;
		waveTotal = (int)MathHelper.Clamp(waveTotal, 0f, maxEnemies);
		calculateSpawningPoint();
		if (currentLevel >= 9 && currentLevel < 13)
		{
			createEnemies(gameTime, calculateSpawning: true, positionBegin);
		}
		if (currentLevel == 13 && difficulty > 1f)
		{
			createEnemies(gameTime, calculateSpawning: true, positionBegin);
		}
		if (frame % 300 == 0 && random.Next(100) < 10)
		{
			AddEnemy(positionBegin, (float)random.Next(720) / 100f, 0, (float)random.Next(100) / 10f + 2f);
		}
		if (currentLevel > 5 && currentLevel != 14 && frame % 700 == 0 && num > 1)
		{
			AddEnemy(RandomPosition(), (float)random.Next(700) / 100f, 12);
		}
		if (currentLevel > 5 && currentLevel != 14 && frame % 900 == 0 && num > 2)
		{
			AddEnemy(RandomPosition(), (float)random.Next(700) / 100f, 12);
		}
		if (currentLevel > 5 && currentLevel != 14 && frame % 1100 == 0 && num > 3)
		{
			AddEnemy(RandomPosition(), (float)random.Next(700) / 100f, 12);
		}
		if (currentLevel > 5 && currentLevel != 14 && frame % 180 == 0)
		{
			AddEnemy(RandomPosition(), (float)random.Next(700) / 100f, 12);
		}
		if (currentLevel > 7 && currentLevel != 14 && frame % 300 == 0)
		{
			AddEnemy(RandomPosition(), (float)random.Next(700) / 100f, 12);
		}
		if (currentLevel > 9 && currentLevel != 14 && frame % 420 == 0)
		{
			AddEnemy(RandomPosition(), (float)random.Next(700) / 100f, 12);
		}
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < assetManager[currentLevel].asset.Count; i++)
		{
			switch (assetManager[currentLevel].asset[i].levelData)
			{
			case LevelData.orb:
				num2 = assetManager[currentLevel].asset[i].numPri;
				num3 += num2;
				if (frame > assetManager[currentLevel].asset[i].frame && num2 > 0)
				{
					AddItem(Pickup.item.orb, assetManager[currentLevel].asset[i].position, 10);
					assetManager[currentLevel].asset[i].numPri--;
				}
				break;
			case LevelData.message:
				num2 = assetManager[currentLevel].asset[i].numPri;
				num3 += num2;
				if (frame > assetManager[currentLevel].asset[i].frame && num2 > 0)
				{
					string[] array = new string[40];
					array[0] = assetManager[currentLevel].asset[i].text;
					messageInfo.Add(array);
					assetManager[currentLevel].asset[i].numPri--;
				}
				break;
			case LevelData.relic:
				num2 = assetManager[currentLevel].asset[i].numPri;
				num3 += num2;
				if (frame > assetManager[currentLevel].asset[i].frame && num2 > 0)
				{
					AddItem(Pickup.item.relic, assetManager[currentLevel].asset[i].position, 1);
					assetManager[currentLevel].asset[i].numPri = 0;
					assetManager[currentLevel].asset[i].numSec = 0;
				}
				break;
			case LevelData.blueMatter:
				num2 = assetManager[currentLevel].asset[i].numPri;
				num3 += num2;
				if (frame > assetManager[currentLevel].asset[i].frame && num2 > 0)
				{
					AddItem(Pickup.item.coins, assetManager[currentLevel].asset[i].position, 1);
					assetManager[currentLevel].asset[i].numPri--;
				}
				break;
			case LevelData.enemy:
				num2 = assetManager[currentLevel].asset[i].numPri;
				num3 += num2;
				if (frame > assetManager[currentLevel].asset[i].frame && frame % 10 == 0 && num2 > 0)
				{
					float angle = (float)random.Next(1000) / 100f;
					AddEnemy(assetManager[currentLevel].asset[i].position, angle, assetManager[currentLevel].asset[i].type);
					assetManager[currentLevel].asset[i].numPri--;
				}
				break;
			}
		}
		if (num3 == 0 && (float)frame > (float)Math.PI * 40f)
		{
			while (numOrbs < 1)
			{
				calculateSpawningPoint();
				AddItem(Pickup.item.orb, positionBegin + new Vector2(random.Next(-300, 300), random.Next(-300, 300)), 300);
				numOrbs++;
			}
			frame = 0u;
			round++;
			for (int i = 0; i < assetManager[currentLevel].asset.Count; i++)
			{
				assetManager[currentLevel].asset[i].Reset();
			}
		}
		calculateSpawningPoint();
		gatherMsg++;
		if (frame == 60 && round == 0 && messages.Count == 0)
		{
			messages.Add(new Message("Protect the Colony", 500, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), sndProtectColony, GOsoundFXvolume));
		}
		if (gatherMsg > gatherMsgDelay && messages.Count <= 0 && frame > 200 && colony.energy <= colony.maximunEnergy / 2f)
		{
			messages.Add(new Message("Gather blue orbs to charge Colony's Core", 600, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), sndBringOrbs, GOsoundFXvolume));
			gatherMsg = 0;
			gatherMsgDelay = (int)((float)gatherMsgDelay * 1.5f);
		}
		if (colonyHit == 1000 && messages.Count == 0)
		{
			messages.Add(new Message("Colony under attack", 300, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), sndColonyUnderA, GOsoundFXvolume / 2f));
			colonyHit--;
		}
		if (colonyHit > 0 && colonyHit != 1000)
		{
			colonyHit--;
		}
		for (int i = 0; i < messages.Count; i++)
		{
			if (messages[i].Active)
			{
				messages[i].Update(i);
			}
			else
			{
				messages.RemoveAt(i);
			}
		}
		if (!(colony.energy >= colony.maximunEnergy * 0.99f))
		{
			return;
		}
		maxEnemies = 0;
		if (endGame == 0)
		{
			createBlast(-1);
			messages.Add(new Message("Maximun energy in Core reached!", 150, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), sndCoreFull, GOsoundFXvolume));
			if (currentLevel < 11)
			{
				if (demo)
				{
					if (currentLevel < maxDemoLevel - 1)
					{
						level[currentLevel + 1].locked = false;
					}
				}
				else
				{
					level[currentLevel + 1].locked = false;
				}
				if (currentLevel < 11)
				{
					nextLevel = currentLevel + 1;
				}
			}
		}
		endGame--;
		if (endGame % 80 == 0)
		{
			createBlast(-1);
		}
		if (endGame == -90)
		{
			messages.Add(new Message("Prepare for hyperjump!", 150, gameFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.5f, 200f), messageSound, GOsoundFXvolume));
		}
		if (endGame >= -240)
		{
			return;
		}
		if (currentLevel != 14)
		{
			copyPlayersToCharacters();
			if (!colony.damaged && awards.Unlock("Protector"))
			{
				writeAwards();
			}
		}
		switch (currentLevel)
		{
		case 2:
			frame = 0u;
			round = 0u;
			winCondition = objective.round3;
			gameStateNext = GameState.BonusChubbyRain;
			gameStatePlay = GameState.ChubbyRain;
			if (awards.Unlock("Chubby Rain"))
			{
				writeAwards();
			}
			unlockChubbyRain = true;
			break;
		case 5:
			frame = 0u;
			round = 0u;
			winCondition = objective.round2;
			gameStateNext = GameState.BonusSidescroller;
			gameStatePlay = GameState.Sidescroller;
			if (awards.Unlock("Sidescroller"))
			{
				writeAwards();
			}
			unlockSidescroller = true;
			break;
		case 8:
			frame = 0u;
			round = 0u;
			winCondition = objective.round3;
			gameStateNext = GameState.BonusMeteroids;
			gameStatePlay = GameState.Meteroids;
			if (awards.Unlock("insert"))
			{
				writeAwards();
			}
			unlockMeteroids = true;
			break;
		case 11:
		{
			for (int j = 0; j < player.Length; j++)
			{
				if (player[j].fighter)
				{
					if (awards.Unlock("Fighter"))
					{
						writeAwards();
					}
				}
				else if (awards.Unlock("Engineer"))
				{
					writeAwards();
				}
			}
			if (num > 1)
			{
				completeCoopLevel = true;
			}
			colony.health = colony.MaximunHealth;
			gameStateNext = GameState.prepareFinalBoss;
			menuActive = 0f;
			unlockBoss = true;
			break;
		}
		default:
		{
			for (int i = 0; i < player.Length; i++)
			{
				if (!player[i].damaged && currentLevel != 14 && awards.Unlock("Survivor"))
				{
					writeAwards();
				}
			}
			gameStateNext = GameState.galaxyMap;
			break;
		}
		}
		writeUnlockables();
	}

	public void UpdatePrepareFinalBoss()
	{
		frame = 0u;
		currentLevel = 13;
		createLevel(characters: false);
		iniCharacters(initializeCharacters: false, addRelics: false);
		gameStateNext = GameState.finalBoss;
		gameStatePlay = GameState.finalBoss;
		menuActive = 0f;
	}

	public void UpdateLevelChallenge(GameTime gameTime)
	{
		maxEnemies = topEnemies * 2;
		waveTotal--;
		waveTotal = (int)MathHelper.Clamp(waveTotal, 0f, maxEnemies);
		calculateSpawningPoint();
		if (frame % 300 == 0 && random.Next(100) < 10)
		{
			AddEnemy(positionBegin, (float)random.Next(720) / 100f, 0, (float)random.Next(100) / 10f + 2f);
		}
		if (coins.Count > 0 && endGame >= 0)
		{
			return;
		}
		playerChallenge.Health = playerChallenge.maximunHealth;
		challengeClear = true;
		if (endGame == 0 && challengeNumber + 1 <= challengeList.selectables.Count - 1)
		{
			if (demo)
			{
				if (challengeNumber < maxDemoLevel * 2)
				{
					challengeList.selectables[challengeNumber + 1].unlock = true;
				}
			}
			else
			{
				challengeList.selectables[challengeNumber + 1].unlock = true;
			}
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			bloom(enemies[i].position);
		}
		if (endGame == -120)
		{
			string[] array = new string[40];
			array[0] = "Challenge Cleared!";
			array[1] = "\n";
			array[2] = "\n";
			messageInfo.Add(array);
		}
		enemies.RemoveRange(0, enemies.Count);
		endGame--;
		if (endGame == -140)
		{
			gameStateNext = GameState.challengeFinished;
		}
	}

	public void UpdateLevelSurvival(GameTime gameTime)
	{
		int num = 0;
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Active)
			{
				num++;
			}
		}
		if (maxEnemies > 30 + num * 20 + (int)(20 * round))
		{
			maxEnemies = 30 + num * 20 + (int)(20 * round);
		}
		if (maxEnemies > topEnemies)
		{
			maxEnemies = topEnemies;
		}
		waveTotal--;
		waveTotal = (int)MathHelper.Clamp(waveTotal, 0f, maxEnemies);
		calculateSpawningPoint();
		createEnemies(gameTime, calculateSpawning: true, positionBegin);
		if (frame == 30 && round == 0)
		{
			if (gameState == GameState.Campaign)
			{
				messages.Add(new Message("Defend the Colony", 400, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), messageSound, GOsoundFXvolume));
			}
			if (gameState == GameState.Survival)
			{
				messages.Add(new Message("Survive endless enemies", 400, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), messageSound, GOsoundFXvolume));
			}
		}
		for (int i = 0; i < messages.Count; i++)
		{
			if (messages[i].Active)
			{
				messages[i].Update(i);
			}
			else
			{
				messages.RemoveAt(i);
			}
		}
	}

	public void UpdateLevelMeteroids(GameTime gameTime)
	{
		maxEnemies = (int)MathHelper.Clamp((round + 1) * 4, (int)MathHelper.Clamp(numPlayers * 2, 4f, 8f), topEnemies);
		waveTotal--;
		waveTotal = (int)MathHelper.Clamp(waveTotal, 0f, maxEnemies);
		updateFrame = true;
		if (random.Next(100) < 10)
		{
			AddEnemy(randomBorderPos(), (float)random.Next(720) / 100f, 0, (float)random.Next(100) / 10f + 2f);
		}
		if (frame == 30 && round == 0)
		{
			messages.Add(new Message("Dodge and blow the asteroids!", 400, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), messageSound, GOsoundFXvolume));
		}
		for (int i = 0; i < messages.Count; i++)
		{
			if (messages[i].Active)
			{
				messages[i].Update(i);
			}
			else
			{
				messages.RemoveAt(i);
			}
		}
		if (winCondition == objective.round3 && round > 2)
		{
			bonusClear = true;
			gameStateNext = GameState.BonusClear;
			gameStatePlay = GameState.galaxyMap;
		}
	}

	public void UpdateLevelChubbyRain(GameTime gameTime)
	{
		int num = 0;
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Active)
			{
				num++;
			}
		}
		if (maxEnemies > 30 + num * 20 + (int)(20 * round))
		{
			maxEnemies = 30 + num * 20 + (int)(20 * round);
		}
		if (maxEnemies > topEnemies)
		{
			maxEnemies = topEnemies;
		}
		waveTotal--;
		waveTotal = (int)MathHelper.Clamp(waveTotal, 0f, maxEnemies);
		calculateSpawningPoint();
		if (enemies.Count == 0)
		{
			CreateChubbyRain(gameTime);
			frame = 0u;
			round++;
		}
		if (winCondition == objective.round3 && round > 3)
		{
			bonusClear = true;
			gameStateNext = GameState.BonusClear;
			if (demo)
			{
				gameStatePlay = GameState.endDemo1;
			}
			else
			{
				gameStatePlay = GameState.galaxyMap;
			}
		}
		calculateSpawningPoint();
		for (int i = 0; i < messages.Count; i++)
		{
			if (messages[i].Active)
			{
				messages[i].Update(i);
			}
			else
			{
				messages.RemoveAt(i);
			}
		}
	}

	public void UpdateLevelSidescroller(GameTime gameTime)
	{
		int num = 0;
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i].Active)
			{
				num++;
			}
		}
		if (maxEnemies > 30 + num * 20 + (int)(20 * round))
		{
			maxEnemies = 30 + num * 20 + (int)(20 * round);
		}
		if (maxEnemies > topEnemies)
		{
			maxEnemies = topEnemies;
		}
		waveTotal--;
		waveTotal = (int)MathHelper.Clamp(waveTotal, 0f, maxEnemies);
		calculateSpawningPoint();
		createEnemies(gameTime, positionBegin);
		if (winCondition == objective.round2 && round > 1)
		{
			bonusClear = true;
			gameStateNext = GameState.BonusClear;
			gameStatePlay = GameState.galaxyMap;
		}
		if (sidescrollerB[0].texture == txSidescrollerB[1] && frame % 5 == 0)
		{
			AddEnemy(positionBegin, (float)Math.PI, 11);
		}
		if (sidescrollerB[0].texture == txSidescrollerB[4] && frame % 2 == 0)
		{
			AddEnemy(positionBegin, (float)Math.PI, 1);
		}
		if (sidescrollerB[0].texture == txSidescrollerB[7] && frame % 2 == 0)
		{
			AddEnemy(positionBegin, (float)Math.PI, 6);
		}
		if (sidescrollerB[0].texture == txSidescrollerB[8] && random.Next(100) < 10)
		{
			AddEnemyBullet(positionBegin, (float)Math.PI + ((float)random.Next(200) - 100f) / 100f, 1f);
		}
		if (round > 10 && random.Next(100) < 5)
		{
			AddEnemyBullet(positionBegin, (float)Math.PI + ((float)random.Next(200) - 100f) / 100f, 1f);
		}
		if (round > 20 && random.Next(100) < 5)
		{
			AddEnemyBullet(positionBegin, (float)Math.PI + ((float)random.Next(200) - 100f) / 100f, 1f);
		}
		for (int i = 0; i < messages.Count; i++)
		{
			if (messages[i].Active)
			{
				messages[i].Update(i);
			}
			else
			{
				messages.RemoveAt(i);
			}
		}
		for (int i = 0; i < sidescrollerB.Length; i++)
		{
			sidescrollerB[i].position.Y = -50f;
			sidescrollerB[i].size = new Vector2(1f, 1f);
			sidescrollerB[i].position.X -= 1.5f;
			if (sidescrollerB[i].position.X < 0f - (float)sidescrollerB[i].Width * 1.5f)
			{
				sidescrollerB[i].position.X += sidescrollerB[i].Width * 3;
				sidescrollerB[i].texture = txSidescrollerB[random.Next(txSidescrollerB.Length)];
			}
		}
	}

	public void UpdateBackground(GameTime gameTime)
	{
		hexagonsGrid.transparency = MathHelper.Lerp(hexagonsGrid.transparency, 0.2f * (GOHUDopacity / 100f), 0.001f);
		background.position = Vector2.Zero - new Vector2(background.Width / 2, background.Height / 2);
		stars.position = camera.position / 2f - new Vector2(stars.Width / 2, stars.Height / 2);
		stars2.position = camera.position / 3f - new Vector2(stars2.Width / 2, stars2.Height / 2);
		if (colony.Active && frame % 10 == 0)
		{
			switch (random.Next(20))
			{
			case 1:
				bloom(colony.position, (float)random.Next(500) / 100f, 1);
				break;
			case 2:
				bloom(colony.position, (float)random.Next(50) / 100f, 2);
				break;
			case 3:
				bloom(colony.position, (float)random.Next(500) / 100f, 3);
				break;
			case 4:
				bloom(colony.position, (float)random.Next(800) / 100f, 1);
				break;
			case 5:
				bloom(colony.position, (float)random.Next(100) / 100f, 1);
				break;
			}
		}
		if (colony.Active && frame % 4 == 0)
		{
			particleSystem.AddTrails(colony.position + new Vector2(random.Next(0, 40), random.Next(-20, 20)), txNoise, 1f, 10f, 0.2f, (float)random.Next(3600) / 100f, 0.01f, new Color(1f - colony.health / colony.MaximunHealth, colony.health / colony.MaximunHealth * 0.8f, colony.health / colony.MaximunHealth, 0.9f));
		}
	}

	public void UpdateBackgroundChubbyRain(GameTime gameTime)
	{
		background.texture = txStars;
		background.position = Vector2.Zero - new Vector2(background.Width / 2, background.Height / 2);
		stars.position = camera.position / 2f - new Vector2(stars.Width / 2, stars.Height / 2);
		stars2.position = camera.position / 3f - new Vector2(stars2.Width / 2, stars2.Height / 2);
		background.scroll++;
		background.UpdateScrollY();
		background.position.Y += background.scroll;
		stars.scroll += 3f;
		stars.UpdateScrollY();
		stars.position.Y += stars.scroll;
		stars2.scroll += 5f;
		stars2.UpdateScrollY();
		stars2.position.Y += stars2.scroll;
	}

	public void UpdateBackgroundSidescroller(GameTime gameTime)
	{
		background.position = Vector2.Zero - new Vector2(background.Width / 2, background.Height / 2);
		stars.position = camera.position / 2f - new Vector2(stars.Width / 2, stars.Height / 2);
		stars2.position = camera.position / 3f - new Vector2(stars2.Width / 2, stars2.Height / 2);
		background.scroll += 0.5f;
		background.UpdateScrollX();
		background.position.X -= background.scroll;
		stars.scroll += 1.5f;
		stars.UpdateScrollX();
		stars.position.X -= stars.scroll;
		stars2.scroll += 2.5f;
		stars2.UpdateScrollX();
		stars2.position.X -= stars2.scroll;
	}

	public void UpdateCamera(GameTime gameTime)
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			num += Math.Abs(player[i].accelerationX2) + Math.Abs(player[i].accelerationY2);
		}
		if (numPlayers > 0)
		{
			if (num > 0.1f)
			{
				cameraZoom -= 0.04f;
			}
			else
			{
				cameraZoom += 0.005f;
			}
		}
		switch (numPlayers)
		{
		case 1:
			cameraZoom = MathHelper.Clamp(cameraZoom, 0.9f, 1.5f);
			break;
		case 2:
			cameraZoom = MathHelper.Clamp(cameraZoom, 0.7f, 1.1f);
			break;
		case 3:
			cameraZoom = MathHelper.Clamp(cameraZoom, 0.65f, 1.05f);
			break;
		case 4:
			cameraZoom = MathHelper.Clamp(cameraZoom, 0.6f, 1f);
			break;
		}
		float num2 = 0.2f;
		if (numPlayers > 0 && endGame == 0)
		{
			cameraPosition.X = 0f;
			cameraPosition.Y = 0f;
			for (int i = 0; i < player.Length; i++)
			{
				if (player[i].Active)
				{
					cameraPosition += player[i].position / 2f;
				}
			}
			cameraPosition /= (float)numPlayers;
			cameraPosition = colony.position / 2f * 0.05f + cameraPosition * 0.95f;
		}
		else
		{
			cameraPosition = colony.position / 2f;
			num2 = 0.05f;
			cameraZoom = 2f;
		}
		if (endGame < 0)
		{
			cameraPosition = colony.position / 2f;
			num2 = 0.01f;
			cameraZoom = 0.5f;
		}
		if (endGame > 0)
		{
			cameraPosition = colony.position / 2f;
			num2 = 0.002f;
			cameraZoom = 2f;
		}
		if (gameState == GameState.ChubbyRain)
		{
			cameraZoom = MathHelper.Clamp(cameraZoom, 1.1f, 1.1f);
			num2 = 0.3f;
			cameraPosition.Y = 160f;
			cameraPosition.X = 160f;
		}
		camera.position.X = MathHelper.Lerp(camera.position.X, cameraPosition.X, num2);
		camera.position.Y = MathHelper.Lerp(camera.position.Y, cameraPosition.Y, num2);
		camera.Zoom = MathHelper.Lerp(camera.Zoom, cameraZoom * ((float)base.GraphicsDevice.Viewport.Width / 1280f), num2 * 0.2f);
		camera.Rotation = MathHelper.Lerp(camera.Rotation, 0f, num2 * 0.2f);
	}

	public void UpdateCameraMeteroids(GameTime gameTime)
	{
		camera.position = new Vector2(320f, 200f);
		camera.Zoom = MathHelper.Lerp(camera.Zoom, 1f, 0.02f);
		camera.Rotation = 0f;
	}

	public void UpdateCameraChallenge(GameTime gameTime)
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			num += Math.Abs(playerChallenge.accelerationX2) + Math.Abs(playerChallenge.accelerationY2);
		}
		if (numPlayers > 0)
		{
			if (num > 0.1f)
			{
				cameraZoom -= 0.04f;
			}
			else
			{
				cameraZoom += 0.005f;
			}
		}
		cameraZoom = MathHelper.Clamp(cameraZoom, 0.8f, 1f);
		float num2 = 0.2f;
		if (numPlayers > 0 && endGame == 0)
		{
			cameraPosition.X = 0f;
			cameraPosition.Y = 0f;
			if (playerChallenge.Active)
			{
				cameraPosition += playerChallenge.position;
			}
			cameraPosition /= 2f;
		}
		else
		{
			cameraPosition = colony.position / 2f;
			num2 = 0.05f;
			cameraZoom = 2f;
		}
		if (endGame < 0)
		{
			cameraPosition = colony.position / 2f;
			num2 = 0.01f;
			cameraZoom = 0.5f;
		}
		if (endGame > 0)
		{
			cameraPosition = colony.position / 2f;
			num2 = 0.002f;
			cameraZoom = 2f;
		}
		camera.position.X = MathHelper.Lerp(camera.position.X, cameraPosition.X, num2);
		camera.position.Y = MathHelper.Lerp(camera.position.Y, cameraPosition.Y, num2);
		camera.Zoom = MathHelper.Lerp(camera.Zoom, cameraZoom * ((float)base.GraphicsDevice.Viewport.Width / 1280f), num2 * 0.2f);
		camera.Rotation = MathHelper.Lerp(camera.Rotation, 0f, num2 * 0.2f);
	}

	public void UpdateCameraEditor(GameTime gameTime)
	{
		if (currentMouseState.MiddleButton == ButtonState.Pressed)
		{
			editorPos += (new Vector2(oldMouseState.X, oldMouseState.Y) - new Vector2(currentMouseState.X, currentMouseState.Y)) / editorZoom;
		}
		editorZoom += ((float)currentMouseState.ScrollWheelValue / 10f - (float)oldMouseState.ScrollWheelValue / 10f) / 100f;
		editorZoom = MathHelper.Clamp(editorZoom, 0.3f, 3f);
		camera.position.X = MathHelper.Lerp(camera.position.X, editorPos.X, 0.5f);
		camera.position.Y = MathHelper.Lerp(camera.position.Y, editorPos.Y, 0.5f);
		camera.Zoom = MathHelper.Lerp(camera.Zoom, editorZoom, 0.5f);
		camera.Rotation = 0f;
	}

	public void UpdateCameraChubbyRain(GameTime gameTime)
	{
		float num = 0.5f;
		cameraZoom = MathHelper.Clamp(cameraZoom, 1.1f, 1.1f);
		cameraPosition.Y = 160f;
		cameraPosition.X = 160f;
		camera.Rotation = 0f;
		camera.position.X = MathHelper.Lerp(camera.position.X, cameraPosition.X, num);
		camera.position.Y = MathHelper.Lerp(camera.position.Y, cameraPosition.Y, num);
		camera.Zoom = MathHelper.Lerp(camera.Zoom, cameraZoom * ((float)base.GraphicsDevice.Viewport.Width / 1280f), num * 0.25f);
		camera.Rotation = MathHelper.Lerp(camera.Rotation, 0f, num * 0.5f);
	}

	public void UpdateCameraSidescroller(GameTime gameTime)
	{
		float num = 0.3f;
		cameraZoom = MathHelper.Clamp(cameraZoom, 1.2f, 1.2f);
		cameraPosition.Y = 160f;
		cameraPosition.X = 160f;
		camera.Rotation = 0f;
		camera.position.X = MathHelper.Lerp(camera.position.X, cameraPosition.X, num);
		camera.position.Y = MathHelper.Lerp(camera.position.Y, cameraPosition.Y, num);
		camera.Zoom = MathHelper.Lerp(camera.Zoom, cameraZoom * ((float)base.GraphicsDevice.Viewport.Width / 1280f), num * 0.25f);
		camera.Rotation = MathHelper.Lerp(camera.Rotation, 0f, num * 0.5f);
	}

	private void UpdateItems(GameTime gameTime)
	{
		float num = 50f;
		numOrbs = 0;
		for (int i = 0; i < pickups.Count; i++)
		{
			if (gameState == GameState.Sidescroller && pickups[i].position.X < -100f)
			{
				pickups[i].Active = false;
			}
			switch (pickups[i].pickupType)
			{
			case Pickup.item.orb:
				num = 100f;
				break;
			case Pickup.item.bomb:
				num = 90f;
				break;
			case Pickup.item.emp:
				num = 130f;
				if (gameState == GameState.ChubbyRain)
				{
					pickups[i].Active = false;
				}
				break;
			}
			int num2 = pickups[i].target;
			if (num2 == -1)
			{
				pickups[i].Update(gameTime, Vector2.Zero, colony.position);
				for (int j = 0; j < player.Length; j++)
				{
					if (!player[j].Active || !(Vector2.Distance(player[j].position, pickups[i].position) < num))
					{
						continue;
					}
					bloom(pickups[i].position, 1);
					if (pickups[i].pickupType == Pickup.item.orb)
					{
						if (player[j].orbs < player[j].maxOrbs)
						{
							bloom(pickups[i].position, 2);
							num = Vector2.Distance(player[j].position, pickups[i].position);
							num2 = j;
							pickups[i].target = num2;
							pickups[i].qeue = num2;
							pickups[i].frame = 0f;
							pickups[i].interest = 1f;
							player[num2].orbs++;
							if (!orbMessage && currentLevel == 14)
							{
								string[] array = new string[40];
								array[0] = "You just grab and Orb!";
								array[1] = "\n";
								array[2] = "\n";
								array[3] = "it will be following you as you fly,";
								array[4] = "\n";
								array[5] = "bring all of them to the Colony's Core.";
								array[6] = "\n";
								messageInfo.Add(array);
								orbMessage = true;
							}
						}
					}
					else
					{
						num = Vector2.Distance(player[j].position, pickups[i].position);
						num2 = j;
						pickups[i].target = num2;
					}
				}
				if (num2 == -1)
				{
					pickups[i].Update(gameTime, Vector2.Zero, colony.position);
				}
			}
			else
			{
				if (pickups[i].pickupType == Pickup.item.orb)
				{
					pickups[i].Update(gameTime, player[num2].position + new Vector2((float)Math.Cos(player[num2].angle) * -45f, (float)Math.Sin(player[num2].angle) * -45f), colony.position);
				}
				else
				{
					pickups[i].Update(gameTime, player[num2].position, colony.position);
					bloom(pickups[i].position, 3);
				}
				if (Vector2.Distance(player[num2].position, pickups[i].position) < MathHelper.Clamp((pickups[i].Scale() + player[num2].size()) / 2f, 20f, 1000f))
				{
					bloom(player[num2].position);
					switch (pickups[i].pickupType)
					{
					case Pickup.item.coins:
						player[num2].credits++;
						break;
					case Pickup.item.bomb:
						createBlast(num2, player[num2].position);
						break;
					case Pickup.item.emp:
					{
						bloom(player[num2].position);
						bloom(player[num2].position, 5f, 1, 1);
						bloom(player[num2].position, 5f, 2, 1);
						bloom(player[num2].position, 5f, 3, 1);
						bloom(player[num2].position, 5f, 4, 1);
						bloom(player[num2].position, 5f, 5, 1);
						if (gameState != GameState.ChubbyRain)
						{
							createBlast(num2, player[num2].position, 1);
						}
						float value = camera.getScreenPosition(player[(int)MathHelper.Clamp(num2, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
						float pan = MathHelper.Clamp(value, -1f, 1f);
						beepSSound.Play(GOsoundFXvolume / 100f, -1f, pan);
						break;
					}
					case Pickup.item.health:
						player[num2].Health += player[num2].maximunHealth / 3f;
						if (!healthMessage && currentLevel == 14)
						{
							string[] array = new string[40];
							array[0] = "You got a health package!";
							array[1] = "\n";
							array[2] = "\n";
							array[3] = "it will restore your health.";
							messageInfo.Add(array);
							healthMessage = true;
						}
						break;
					case Pickup.item.relic:
					{
						player[num2].relics = 1;
						messages.Add(new Message("New Relic acquired, Player " + (num2 + 1), 300, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), sndRelicAquired, GOsoundFXvolume));
						for (int j = 0; j < 150; j++)
						{
							particleSystem.AddItemTrails(camera.getScreenPosition(player[num2].position, base.GraphicsDevice), player[num2].UIpos, txRingClouds, 0.001f, 1.5f, 0.02f + (float)random.Next(5, 15) / 100f, (float)random.Next(3600) / 100f, 0.05f, player[num2].shootColor);
						}
						if (!relicMessage && currentLevel == 14)
						{
							string[] array = new string[40]
							{
								"That's a Relic!", "\n", "\n", "Relics gives you new abilities,\n", "\n", "like build shields, hives or shot missiles\n", "\n", "using the buttons (A) for shields,\n", "\n", "(X) for turrets, (Y) for hives, (B) for sanctuaries\n",
								"\n", "or if you are a fighter you can exchange shot type\n", "\n", "by pressing (X) (Y) (B)\n", null, null, null, null, null, null,
								null, null, null, null, null, null, null, null, null, null,
								null, null, null, null, null, null, null, null, null, null
							};
							messageInfo.Add(array);
							relicMessage = true;
						}
						break;
					}
					}
					if (pickups[i].pickupType != Pickup.item.orb)
					{
						bloom(player[num2].position);
						bloom(pickups[i].position);
						pickups[i].Active = false;
					}
				}
			}
			if (pickups[i].qeue >= 0 && pickups[i].target < 0)
			{
				player[pickups[i].qeue].orbs--;
				pickups[i].qeue = -1;
			}
			if (pickups[i].pickupType == Pickup.item.orb)
			{
				if (random.Next(100) < 20)
				{
					particleSystem.AddTrails(pickups[i].position, txRingClouds, 0.1f, 1.5f, 0.05f, pickups[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.5f));
				}
				particleSystem.AddTrails(pickups[i].position, txOrbsTrails, 0.07f, 0.5f, 0.09f, pickups[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.35f));
				if (Vector2.Distance(pickups[i].position, colony.position) < 65f)
				{
					colony.energyTarget++;
					if (pickups[i].qeue >= 0)
					{
						player[pickups[i].qeue].orbs--;
						pickups[i].qeue = -1;
					}
					pickups[i].Active = false;
					if (messages.Count < 1 || colony.energyTarget / colony.maximunEnergy == 0.25f || colony.energyTarget / colony.maximunEnergy == 0.5f || colony.energyTarget / colony.maximunEnergy == 0.75f || colony.energyTarget / colony.maximunEnergy == 1f)
					{
						messages.Add(new Message("Core energy at " + (int)(colony.energyTarget / colony.maximunEnergy * 100f) + "%", 100, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), messageSound, GOsoundFXvolume));
					}
					gatherMsg = 0;
					gatherMsgDelay += 100;
					for (int j = 0; j < 100; j++)
					{
						particleSystem.AddItemTrails(camera.getScreenPosition(colony.position, base.GraphicsDevice), new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.3f, base.GraphicsDevice.Viewport.Height), txOrbs, 0.01f, 1.5f, 0.05f + (float)random.Next(5, 15) / 100f, (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.8f, 1f, 1f));
					}
					if (blast.Count <= 0)
					{
						createBlast(-2);
					}
				}
				numOrbs++;
			}
			if (i > 100)
			{
				pickups[i].time /= 2;
			}
			if (!pickups[i].Active)
			{
				pickups.RemoveAt(i);
			}
		}
		for (int k = 0; k < coinSounds.Count; k++)
		{
			if (coinSounds[k].update(GOsoundFXvolume / 1000f, 0f))
			{
				coinSounds.RemoveAt(k);
			}
		}
		for (int i = 0; i < coins.Count; i++)
		{
			num = 120f;
			if (gameState == GameState.Challenge)
			{
				num = 40f;
			}
			if (gameState == GameState.Sidescroller && coins[i].position.X < -100f)
			{
				coins[i].Active = false;
			}
			int num2 = coins[i].target;
			if (num2 == -1)
			{
				for (int j = 0; j < player.Length; j++)
				{
					if (player[j].Active && Vector2.Distance(player[j].position, coins[i].position) < num)
					{
						bloom(coins[i].position, 1);
						num = Vector2.Distance(player[j].position, coins[i].position);
						num2 = j;
						coins[i].target = num2;
					}
				}
				if (num2 == -1)
				{
					coins[i].Update(gameTime, Vector2.Zero, colony.position);
				}
			}
			else
			{
				bloom(coins[i].position, 3f, 6, 1);
				coins[i].Update(gameTime, player[num2].position, colony.position);
				if (Vector2.Distance(player[num2].position, coins[i].position) < coins[i].Scale() / 2f)
				{
					bloom(coins[i].position, 1);
					bloom(coins[i].position, 3);
					player[num2].credits++;
					coins[i].Active = false;
					coinSounds.Add(new soundQeue(coinSound, (float)random.Next(70, 80) / -100f, coinSounds.Count));
				}
			}
			if (i > 100)
			{
				coins[i].time /= 2;
			}
			if (!coins[i].Active)
			{
				coins.RemoveAt(i);
			}
		}
	}

	private void UpdateItemsChallenge(GameTime gameTime)
	{
		float num = 100f;
		numOrbs = 0;
		for (int i = 0; i < pickups.Count; i++)
		{
			if (pickups[i].pickupType == Pickup.item.pathNode)
			{
				bloom(pickups[i].position, 7);
				pickups[i].Active = true;
			}
			switch (pickups[i].pickupType)
			{
			case Pickup.item.orb:
				num = 100f;
				break;
			case Pickup.item.bomb:
				num = 90f;
				break;
			case Pickup.item.emp:
				num = 130f;
				break;
			}
			int num2 = pickups[i].target;
			if (num2 == -1)
			{
				pickups[i].Update(gameTime, Vector2.Zero, colony.position);
				if (playerChallenge.Active && Vector2.Distance(playerChallenge.position, pickups[i].position) < num)
				{
					bloom(pickups[i].position, 1);
					if (pickups[i].pickupType == Pickup.item.orb)
					{
						if (playerChallenge.orbs < playerChallenge.maxOrbs)
						{
							bloom(pickups[i].position, 2);
							num = Vector2.Distance(playerChallenge.position, pickups[i].position);
							num2 = 0;
							pickups[i].target = num2;
							pickups[i].qeue = num2;
							pickups[i].frame = 0f;
							pickups[i].interest = 1f;
							playerChallenge.orbs++;
							if (!orbMessage && currentLevel == 14)
							{
								string[] array = new string[40];
								array[0] = "You just grab and Orb!";
								array[1] = "\n";
								array[2] = "\n";
								array[3] = "it will be following you as you fly,";
								array[4] = "\n";
								array[5] = "bring all of them to the Colony's Core.";
								array[6] = "\n";
								messageInfo.Add(array);
								orbMessage = true;
							}
						}
					}
					else
					{
						num = Vector2.Distance(playerChallenge.position, pickups[i].position);
						num2 = 0;
						pickups[i].target = num2;
					}
				}
				if (num2 == -1)
				{
					pickups[i].Update(gameTime, Vector2.Zero, colony.position);
				}
			}
			else
			{
				if (pickups[i].pickupType == Pickup.item.orb)
				{
					pickups[i].Update(gameTime, playerChallenge.position + new Vector2((float)Math.Cos(playerChallenge.angle) * -45f, (float)Math.Sin(playerChallenge.angle) * -45f), colony.position);
				}
				else
				{
					pickups[i].Update(gameTime, playerChallenge.position, colony.position);
					if (pickups[i].pickupType != Pickup.item.pathNode)
					{
						bloom(pickups[i].position, 3);
					}
				}
				if (Vector2.Distance(playerChallenge.position, pickups[i].position) < (pickups[i].Scale() + playerChallenge.size()) / 2f)
				{
					bloom(playerChallenge.position);
					switch (pickups[i].pickupType)
					{
					case Pickup.item.pathNode:
						playerChallenge.boost = 15f;
						break;
					case Pickup.item.coins:
						playerChallenge.credits++;
						break;
					case Pickup.item.bomb:
						createBlast(num2, playerChallenge.position);
						break;
					case Pickup.item.emp:
					{
						bloom(playerChallenge.position);
						bloom(playerChallenge.position, 5f, 1, 1);
						bloom(playerChallenge.position, 5f, 2, 1);
						bloom(playerChallenge.position, 5f, 3, 1);
						bloom(playerChallenge.position, 5f, 4, 1);
						bloom(playerChallenge.position, 5f, 5, 1);
						createBlast(num2, playerChallenge.position, 1);
						float value = camera.getScreenPosition(player[(int)MathHelper.Clamp(num2, 0f, 3f)].position, base.GraphicsDevice).X * 2f / (float)base.GraphicsDevice.Viewport.Width - 1f;
						float pan = MathHelper.Clamp(value, -1f, 1f);
						beepSSound.Play(GOsoundFXvolume / 100f, -1f, pan);
						break;
					}
					case Pickup.item.health:
						playerChallenge.Health += playerChallenge.maximunHealth / 3f;
						if (!healthMessage && currentLevel == 14)
						{
							string[] array = new string[40];
							array[0] = "You got a health package!";
							array[1] = "\n";
							array[2] = "\n";
							array[3] = "it will restore your health.";
							messageInfo.Add(array);
							healthMessage = true;
						}
						break;
					case Pickup.item.relic:
					{
						playerChallenge.relics = 1;
						messages.Add(new Message("New Relic acquired, Player " + (num2 + 1), 300, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), sndRelicAquired, GOsoundFXvolume));
						for (int j = 0; j < 150; j++)
						{
							particleSystem.AddItemTrails(camera.getScreenPosition(playerChallenge.position, base.GraphicsDevice), playerChallenge.UIpos, txRingClouds, 0.001f, 1.5f, 0.02f + (float)random.Next(5, 15) / 100f, (float)random.Next(3600) / 100f, 0.05f, playerChallenge.shootColor);
						}
						if (!relicMessage && currentLevel == 14)
						{
							string[] array = new string[40]
							{
								"That's a Relic!", "\n", "\n", "Relics gives you new abilities,\n", "\n", "like build shields, hives or shot missiles\n", "\n", "using the buttons (A) for shields,\n", "\n", "(X) for turrets, (Y) for hives, (B) for sanctuaries\n",
								"\n", "or if you are a fighter you can exchange shot type\n", "\n", "by pressing (X) (Y) (B)\n", null, null, null, null, null, null,
								null, null, null, null, null, null, null, null, null, null,
								null, null, null, null, null, null, null, null, null, null
							};
							messageInfo.Add(array);
							relicMessage = true;
						}
						break;
					}
					}
					if (pickups[i].pickupType != Pickup.item.orb && pickups[i].pickupType != Pickup.item.pathNode)
					{
						bloom(playerChallenge.position);
						bloom(pickups[i].position);
						pickups[i].Active = false;
					}
				}
			}
			if (pickups[i].qeue >= 0 && pickups[i].target < 0)
			{
				player[pickups[i].qeue].orbs--;
				pickups[i].qeue = -1;
			}
			if (pickups[i].pickupType == Pickup.item.orb)
			{
				if (random.Next(100) < 20)
				{
					particleSystem.AddTrails(pickups[i].position, txRingClouds, 0.1f, 1.5f, 0.05f, pickups[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.5f));
				}
				particleSystem.AddTrails(pickups[i].position, txOrbsTrails, 0.07f, 0.5f, 0.09f, pickups[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.9f, 1f, 0.35f));
				if (Vector2.Distance(pickups[i].position, colony.position) < 65f)
				{
					colony.energyTarget++;
					if (pickups[i].qeue >= 0)
					{
						player[pickups[i].qeue].orbs--;
						pickups[i].qeue = -1;
					}
					pickups[i].Active = false;
					if (messages.Count < 1 || colony.energyTarget / colony.maximunEnergy == 0.25f || colony.energyTarget / colony.maximunEnergy == 0.5f || colony.energyTarget / colony.maximunEnergy == 0.75f || colony.energyTarget / colony.maximunEnergy == 1f)
					{
						messages.Add(new Message("Core energy at " + (int)(colony.energyTarget / colony.maximunEnergy * 100f) + "%", 100, menuFont, txBlack, new Vector2((float)base.GraphicsDevice.Viewport.Width / 2f, 200f), messageSound, GOsoundFXvolume));
					}
					gatherMsg = 0;
					gatherMsgDelay += 100;
					for (int j = 0; j < 100; j++)
					{
						particleSystem.AddItemTrails(camera.getScreenPosition(colony.position, base.GraphicsDevice), new Vector2((float)base.GraphicsDevice.Viewport.Width * 0.3f, base.GraphicsDevice.Viewport.Height), txOrbs, 0.01f, 1.5f, 0.05f + (float)random.Next(5, 15) / 100f, (float)random.Next(3600) / 100f, 0.01f, new Color(0.5f, 0.8f, 1f, 1f));
					}
					if (blast.Count <= 0)
					{
						createBlast(-2);
					}
				}
				numOrbs++;
			}
			if (i > 100)
			{
				pickups[i].time /= 2;
			}
			if (!pickups[i].Active)
			{
				pickups.RemoveAt(i);
			}
		}
		for (int k = 0; k < coinSounds.Count; k++)
		{
			if (coinSounds[k].update(GOsoundFXvolume / 1000f, 0f))
			{
				coinSounds.RemoveAt(k);
			}
		}
		for (int i = 0; i < coins.Count; i++)
		{
			num = 120f;
			if (gameState == GameState.Challenge)
			{
				num = 40f;
			}
			particleSystem.AddTrails(coins[i].position, txOrbsTrails, 0f, 0.4f, 0.1f, coins[i].angle + (float)random.Next(3600) / 100f, 0.01f, new Color(0.6f, 0.8f, 0.9f, 0.3f));
			int num2 = coins[i].target;
			if (num2 == -1)
			{
				if (playerChallenge.Active && Vector2.Distance(playerChallenge.position, coins[i].position) < num)
				{
					bloom(coins[i].position, 1);
					num = Vector2.Distance(playerChallenge.position, coins[i].position);
					num2 = 0;
					coins[i].target = num2;
				}
				if (num2 == -1)
				{
					coins[i].Update(gameTime, Vector2.Zero, colony.position);
				}
			}
			else
			{
				bloom(coins[i].position, 3f, 6, 1);
				coins[i].Update(gameTime, playerChallenge.position, colony.position);
				if (Vector2.Distance(playerChallenge.position, coins[i].position) < (playerChallenge.size() + coins[i].Scale()) * 0.65f)
				{
					bloom(coins[i].position, 1);
					bloom(coins[i].position, 3);
					playerChallenge.credits++;
					coins[i].Active = false;
					coinSounds.Add(new soundQeue(coinSound, (float)random.Next(70, 80) / -100f, coinSounds.Count));
					AddEnemy(coins[i].position, (float)random.Next(700) / 100f, 1);
				}
			}
			if (i > 100)
			{
				coins[i].time /= 2;
			}
			if (!coins[i].Active)
			{
				coins.RemoveAt(i);
			}
		}
	}

	private void PlaySong(string songName)
	{
		if (GOmusicVolume > 0f && songName != currentSongName)
		{
			currentSongName = songName;
			switch (songName)
			{
			case "Colonial_trance":
				PlayMusic(Colonial_trance);
				break;
			case "DarkMatter":
				PlayMusic(musicDarkMatter);
				break;
			case "EpicFinale":
				PlayMusic(EpicFinale);
				break;
			case "In_a_heart_beat":
				PlayMusic(In_a_heart_beat);
				break;
			case "Jarre80s":
				PlayMusic(musicJarre80sTheme);
				break;
			case "KnowYourEnemy":
				PlayMusic(KnowYourEnemy);
				break;
			case "musicLevel01":
				PlayMusic(musicLevel01);
				break;
			case "MainTheme":
				PlayMusic(musicMainTheme);
				break;
			case "Marching":
				PlayMusic(Marching);
				break;
			case "MoonStrings":
				PlayMusic(MoonStrings);
				break;
			case "Pit":
				PlayMusic(Pit);
				break;
			case "ReachTheStars":
				PlayMusic(musicReachTheStarsTheme);
				break;
			case "FaceYourFears":
				PlayMusic(FaceYourFears);
				break;
			case "HeIsAlive":
				PlayMusic(HeIsAlive);
				break;
			case "HeartAndDanger":
				PlayMusic(HeartAndDanger);
				break;
			case "LongIsTheWay":
				PlayMusic(LongIsTheWay);
				break;
			case "TimeToRun":
				PlayMusic(TimeToRun);
				break;
			case "UnknownBellow":
				PlayMusic(UnknownBellow);
				break;
			case "WeirdDimensions":
				PlayMusic(WeirdDimensions);
				break;
			case "none":
				MediaPlayer.Stop();
				currentSongName = songName;
				currentSong = null;
				break;
			default:
				PlayMusic(musicLevel01);
				break;
			}
		}
		currentSongName = songName;
	}

	private void PlayMusic(Song song)
	{
		MediaPlayer.Volume = GOmusicVolume / 100f;
		MediaPlayer.Resume();
		if (GOmusicVolume > 0f && song != currentSong)
		{
			try
			{
				if (song != currentSong)
				{
					MediaPlayer.Play(song);
				}
				MediaPlayer.IsRepeating = true;
				currentSong = song;
			}
			catch
			{
			}
		}
		currentSong = song;
	}

	private void UpdateLens()
	{
		UpdateLens(currentLevel);
	}

	private void UpdateLens(int level)
	{
		for (int i = 0; i < lens[level].Count; i++)
		{
			lens[level][i].Update(camera.getScreenPosition(lens[level][i].position, base.GraphicsDevice), new Vector2(base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height));
		}
	}

	private void updateVolume(float multiplier)
	{
		MediaPlayer.Volume = GOmusicVolume / 100f * multiplier;
		MediaPlayer.Resume();
	}

	public void CreateIntro()
	{
		try
		{
			intro = new List<Intro>(7);
		}
		catch
		{
		}
		intro.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Intro_01")));
		intro.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Intro_02")));
		intro.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Intro_03")));
		intro.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Intro_04")));
		intro.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Intro_05")));
		intro.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Intro_06")));
	}

	public void CreateHowToPlay()
	{
		try
		{
			howToPlay = new List<Intro>(8);
		}
		catch
		{
		}
		howToPlay.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/HTP_orbs")));
		howToPlay.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/HTP_construct")));
		howToPlay.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/HTP_modes")));
	}

	public void CreateControls()
	{
		try
		{
			controls = new List<Intro>(8);
		}
		catch
		{
		}
		controls.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Control_360F")));
		controls.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Control_360E")));
	}

	public void CreateEnding()
	{
		try
		{
			ending = new List<Intro>(7);
		}
		catch
		{
		}
		ending.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Ending_01")));
		ending.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Ending_02")));
		ending.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Ending_03")));
		ending.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Ending_04")));
		ending.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Ending_05")));
		ending.Add(new Intro(base.Content.Load<Texture2D>("Graphics/Screens/Ending_06")));
	}

	public void UpdateEnding()
	{
		updateFrame = true;
		if (GOmusicVolume > 0f)
		{
			PlaySong("ReachTheStars");
		}
		if (endingSlide >= intro.Count)
		{
			ResetEnding();
			disclaimerTimer = 0;
			gameStateNext = GameState.credits;
		}
		ResetVibration();
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed || (currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || currentKeyboardState.IsKeyDown(Keys.Escape) || currentMouseState.LeftButton == ButtonState.Pressed)
			{
				ResetEnding();
				disclaimerTimer = 0;
				gameStateNext = GameState.credits;
				break;
			}
		}
		for (int i = 0; i < ending.Count; i++)
		{
			if (endingSlide == i)
			{
				ending[i].transp = MathHelper.Lerp(ending[i].transp, 1.1f, 0.01f);
				if (ending[i].transp >= 0.99f && frame > 180)
				{
					endingSlide++;
					frame = 0u;
				}
			}
		}
	}

	public void UpdateIntro()
	{
		updateFrame = true;
		if (GOmusicVolume > 0f)
		{
			PlaySong("MainTheme");
		}
		if (introSlide >= intro.Count)
		{
			ResetIntro();
			gameStateNext = GameState.startScreen;
		}
		ResetVibration();
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed || GamePad.GetState(playerIndex).Buttons.B == ButtonState.Pressed || (currentKeyboardState.IsKeyDown(Keys.Enter) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Escape) || currentMouseState.LeftButton == ButtonState.Pressed || currentMouseState.RightButton == ButtonState.Pressed)
			{
				ResetIntro();
				gameStateNext = GameState.startScreen;
				break;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (introSlide == i)
			{
				intro[i].transp = MathHelper.Lerp(intro[i].transp, 1.1f, 0.01f);
				if (intro[i].transp >= 0.99f && frame > 100)
				{
					introSlide++;
					frame = 0u;
				}
			}
		}
	}

	public void ResetIntro()
	{
		frame = 0u;
		introSlide = 0;
		for (int i = 0; i < 6; i++)
		{
			intro[i].transp = 0f;
		}
	}

	public void ResetEnding()
	{
		frame = 0u;
		endingSlide = 0;
		for (int i = 0; i < ending.Count; i++)
		{
			ending[i].transp = 0f;
		}
	}

	public void DrawLogo()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
		kplogo.Draw(spriteBatch, kplogo.transparency);
		spriteBatch.End();
	}

	public void DrawIntro(SpriteBatch sb)
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		for (int i = 0; i < intro.Count; i++)
		{
			sb.Draw(intro[i].tx, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White * intro[i].transp, 0f, Vector2.Zero, SpriteEffects.None, (float)i / 10f);
		}
		spriteBatch.End();
	}

	public void DrawEnding(SpriteBatch sb)
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		for (int i = 0; i < ending.Count; i++)
		{
			sb.Draw(ending[i].tx, new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height), null, Color.White * ending[i].transp, 0f, Vector2.Zero, SpriteEffects.None, (float)i / 10f);
		}
		spriteBatch.End();
	}

	public void SaveWorld()
	{
	}

	public void SaveChallenge()
	{
	}

	public void SaveLevel()
	{
	}

	public string getLevel()
	{
		return getLevel(currentLevel);
	}

	public string getLevel(int lv)
	{
		return lv switch
		{
			0 => "Hines", 
			1 => "Nymeriah", 
			2 => "Herschel", 
			3 => "Danae", 
			4 => "Clarke", 
			5 => "Gea Moon", 
			6 => "Calypso", 
			7 => "Bradbury", 
			8 => "Eos rests", 
			9 => "Olbers 4", 
			10 => "Eneas", 
			11 => "Prometheus", 
			13 => "The Source", 
			_ => "Simulator Room", 
		};
	}

	public List<Asset> ReadWorld()
	{
		string text = getLevel(currentLevel);
		List<Asset> result = new List<Asset>(999);
		try
		{
			Stream stream = TitleContainer.OpenStream("Content/Data/Levels/" + text + ".xml");
			XDocument xDocument = XDocument.Load(stream);
			result = (from asset in xDocument.Descendants("Asset")
				select new Asset
				{
					levelData = Asset.Parse(asset.Element("levelData").Value),
					name = asset.Element("name").Value,
					desc = asset.Element("desc").Value,
					position = stringToVector2(asset.Element("position").Value),
					angle = Convert.ToInt32(asset.Element("angle").Value),
					size = Convert.ToInt32(asset.Element("size").Value),
					type = Convert.ToInt32(asset.Element("type").Value),
					frame = (uint)Convert.ToInt32(asset.Element("frame").Value),
					text = asset.Element("text").Value,
					color = stringToColor(asset.Element("color").Value),
					color2 = stringToColor(asset.Element("color2").Value),
					color3 = stringToColor(asset.Element("color3").Value),
					numSec = Convert.ToInt32(asset.Element("numSec").Value),
					numPri = Convert.ToInt32(asset.Element("numPri").Value)
				}).ToList();
		}
		catch
		{
		}
		return result;
	}

	public List<Asset> ReadChallenge()
	{
		return ReadChallenge(challengeNumber);
	}

	public List<Asset> ReadChallenge(int challengeNumber)
	{
		string text = files[challengeNumber].Substring(filesChar, 12);
		List<Asset> result = new List<Asset>(999);
		try
		{
			Stream stream = TitleContainer.OpenStream("Content/Data/" + text + ".xml");
			XDocument xDocument = XDocument.Load(stream);
			result = (from asset in xDocument.Descendants("Asset")
				select new Asset
				{
					levelData = Asset.Parse(asset.Element("levelData").Value),
					name = asset.Element("name").Value,
					desc = asset.Element("desc").Value,
					position = stringToVector2(asset.Element("position").Value),
					angle = Convert.ToInt32(asset.Element("angle").Value),
					size = Convert.ToInt32(asset.Element("size").Value),
					type = Convert.ToInt32(asset.Element("type").Value),
					frame = (uint)Convert.ToInt32(asset.Element("frame").Value),
					text = asset.Element("text").Value,
					color = stringToColor(asset.Element("color").Value),
					color2 = stringToColor(asset.Element("color2").Value),
					color3 = stringToColor(asset.Element("color3").Value),
					numSec = Convert.ToInt32(asset.Element("numSec").Value),
					numPri = Convert.ToInt32(asset.Element("numPri").Value)
				}).ToList();
		}
		catch
		{
		}
		return result;
	}

	private Vector2 stringToVector2(string str)
	{
		Vector2 result = Vector2.Zero;
		string[] array = str.Split(' ', ':', '\t');
		if (array.Length > 1)
		{
			result = new Vector2(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
		}
		return result;
	}

	private Color stringToColor(string str)
	{
		Color result = Color.Black * 0f;
		string[] array = str.Split(' ', ':', '\t');
		if (array.Length > 3)
		{
			result = new Color(Convert.ToByte(array[0]), Convert.ToByte(array[1]), Convert.ToByte(array[2]), Convert.ToByte(array[3]));
		}
		return result;
	}

	public void NetConfig()
	{
	}

	public void NetCreateServer()
	{
	}

	public void NetClientConnect()
	{
	}

	public void NetReadMessages()
	{
	}

	public void NetSendMessage()
	{
	}

	public void NetServerShutdown()
	{
	}

	public void NetClientShutdown()
	{
	}
}
