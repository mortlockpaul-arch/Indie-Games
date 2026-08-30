using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using ProjectMercury;
using ProjectMercury.Renderers;

namespace Platformer1;

public class Level : IDisposable
{
	public struct SavedData(int count)
	{
		public string LevelName = " ";

		public bool Dueling = false;

		public int[] ObjectCount = new int[count];

		public int[] ObjectType = new int[count];

		public string[] ObjectSubType = new string[count];

		public Vector2[] ObjectPosition = new Vector2[count];

		public float[] ObjectRotation = new float[count];

		public int Count = count;
	}

	public const float Grav = 100f;

	private const int EntityLayer = 1;

	private const int PointsPerSecond = 5;

	private const float GroundBody_Width = 20000f;

	private const float GroundBody_Height = 300f;

	public float PhysicsScaleDown = 0.2f;

	public float PhysicsScaleUp = 5f;

	public Fixture ActiveZoneFixture;

	public PlatformerGame mainGame;

	public Fixture Spawn_Fixture;

	public Thread Thread_Physics;

	public Thread Thread_Particle;

	public Thread Thread_Camera;

	public Thread Thread_Music;

	public bool FirstTime;

	public bool Thread_Physics_Done = true;

	public float Physics_Speed = 5f;

	private TimeSpan gameTime_Physics_Old;

	private TimeSpan Scene_Update_Pacer;

	private TimeSpan Scene_Update_Pacer_2;

	public string TesterData_PhysicsThread_1 = "  ";

	public string TesterData_PhysicsThread_3 = "  ";

	private int TesterPhysics_Cycle;

	public double TesterData_PhysicsThread_2;

	public float Physics_Speed_Divider = 1f;

	public float Physics_SlowTime_Step;

	public int PhysicsSleepMilli = 2;

	private GameTime gameTime_Physics;

	private float PlayerDistApart_Float;

	public float Gravity = 100f;

	public World _world = new World(new Vector2(0f, 100f));

	public Body exitBody;

	public bool exitReached;

	public bool Paused;

	private bool wasContinue1Pressed;

	private bool wasContinue2Pressed;

	private bool wasContinue3Pressed;

	private bool wasContinue4Pressed;

	private SpriteFont PauseFont;

	private SpriteFont HudFont;

	private SpriteFont Hud2Font;

	private int PauseMenuIndexer;

	private int PauseMenuIndexerMax = 4;

	private int PlayerPausedIndex;

	private bool PauseMenuButtonAWasPressed;

	public Texture2D MasterScene;

	public float MasterScale = 1f;

	public float Factor = 1.25f;

	public SpriteFont Font;

	public SpriteEffects spriteEffect;

	public string Data;

	public string Data2;

	private Tile[,] tiles;

	private Layer[] layers;

	private Terrain[] terrains;

	private bool DrawTerrainThread_First = true;

	private Thread DrawTerrainThread;

	private Clouds[] clouds;

	private Clouds[] Fogs;

	private Color BackgroundColor;

	public Matrix cameraTransform;

	public Matrix cameraTransformForParticles;

	public Matrix cameraTransformHud;

	public Matrix cameraTransformPause;

	public double Global_Scale_Width;

	public double Global_Scale_Height;

	public float cameraPosition;

	public float cameraHeightPosition;

	private float CameraPositionNewY;

	private float CameraPositionNewX;

	private int LastMilliSeconds;

	public SpriteBatch spriteBatchLevel;

	public Texture2D PauseMenuTexture;

	private Texture2D PauseMenuBackgroundStripTexture;

	private Texture2D PauseMenuControllerLayoutTexture;

	private Texture2D PauseMenuPlayerStatsTexture;

	private Texture2D PauseMenuPlayerStatsTexture_Daru;

	private Texture2D PauseMenuPlayerStatsTexture_Ernest;

	private Texture2D PauseMenuPlayerStatsTexture_Oscar;

	private Texture2D PauseMenuPlayerStatsTexture_Rick;

	private Texture2D PauseMenuPlayerStatsTexture_Vinny;

	public bool Blood = true;

	public bool FriendlyFireToggle = true;

	public bool BodiesOnMap;

	public bool MusicToggle = true;

	public bool SoundEffectToggle = true;

	public List<Player1> Player1 = new List<Player1>();

	private float Player1PositionX;

	private float Player1PositionY;

	private float Player1movement;

	private Vector2 start1;

	public PlayerIndex PlayerIndexer_Pub;

	public bool P1DpadRightpressed;

	public bool P1DpadRightWaspressed;

	public bool P1DpadLeftpressed;

	public bool P1DpadLeftWaspressed;

	public bool P1DpadUppressed;

	public bool P1DpadUpWaspressed;

	public bool P1DpadDownpressed;

	public bool P1DpadDownWaspressed;

	public bool P1ShoulderRightWaspressed;

	public bool P1ShoulderRightpressed;

	public bool P1ShoulderLeftWaspressed;

	public bool P1ShoulderLeftpressed;

	private Vector2 AveragePlayerPosition;

	private Vector2 AveragePlayerPosition_Ease;

	public Vector2 PlayerPosition_Vec;

	private float AveragePlayerDist;

	private Color Player1Color;

	private Color Player2Color;

	private Color Player3Color;

	private Color Player4Color;

	private Color Player1HPColor;

	private Color Player2HPColor;

	private Color Player3HPColor;

	private Color Player4HPColor;

	private Color Player1ManaColor;

	private Color Player2ManaColor;

	private Color Player3ManaColor;

	private Color Player4ManaColor;

	public int PlayersInGameIndex;

	public int Player1Lives = -1;

	public int Player2Lives = -1;

	public int Player3Lives = -1;

	public int Player4Lives = -1;

	public bool Player1Dead = true;

	public bool Player2Dead = true;

	public bool Player3Dead = true;

	public bool Player4Dead = true;

	public int PlayersLives;

	private int Duel_Lives = 10;

	private int Co_Op_Lives = 5;

	public Vector2 CamVector;

	private double P1Old_Vibration_Time_Left;

	private double P2Old_Vibration_Time_Left;

	private double P3Old_Vibration_Time_Left;

	private double P4Old_Vibration_Time_Left;

	private double P1Old_Vibration_Time_Right;

	private double P2Old_Vibration_Time_Right;

	private double P3Old_Vibration_Time_Right;

	private double P4Old_Vibration_Time_Right;

	private int P1Vibration_Time_Left = 10;

	private int P2Vibration_Time_Left = 10;

	private int P3Vibration_Time_Left = 10;

	private int P4Vibration_Time_Left = 10;

	private int P1Vibration_Time_Right = 10;

	private int P2Vibration_Time_Right = 10;

	private int P3Vibration_Time_Right = 10;

	private int P4Vibration_Time_Right = 10;

	private float P1Vibration_Speed_Left;

	private float P2Vibration_Speed_Left;

	private float P3Vibration_Speed_Left;

	private float P4Vibration_Speed_Left;

	private float P1Vibration_Speed_Right;

	private float P2Vibration_Speed_Right;

	private float P3Vibration_Speed_Right;

	private float P4Vibration_Speed_Right;

	public int BodyCount = -1;

	public int Player1Index;

	public int Player2Index;

	public int Player3Index;

	public int Player4Index;

	public bool AnyAlive;

	public bool Player1InGame;

	public bool Player2InGame;

	public bool Player3InGame;

	public bool Player4InGame;

	public bool Player1AliveOnce;

	public bool Player2AliveOnce;

	public bool Player3AliveOnce;

	public bool Player4AliveOnce;

	public bool RespawnPlayer1_GO;

	public bool RespawnPlayer2_GO;

	public bool RespawnPlayer3_GO;

	public bool RespawnPlayer4_GO;

	public Vector2 Player1Position;

	public Vector2 Player2Position;

	public Vector2 Player3Position;

	public Vector2 Player4Position;

	private Effect desaturateEffect;

	private Effect disappearEffect;

	private Effect normalmapEffect;

	private Effect refractionEffect;

	private Texture2D catTexture;

	private Texture2D catNormalmapTexture;

	private Texture2D glacierTexture;

	private Texture2D waterfallTexture;

	private Texture2D ShadowTexture;

	private Vector2 ShadowTextureOrigin;

	private float ShadowScale;

	private List<Lands> Lands = new List<Lands>();

	public int L;

	public int l;

	private float LandPositionX;

	private float LandPositionY;

	private List<Blocks> Blocks = new List<Blocks>();

	public int B;

	public int b;

	private float BlockPositionX;

	private float BlockPositionY;

	private List<Sharps> Sharps = new List<Sharps>();

	public int SH;

	public int sh;

	private float SharpPositionX;

	private float SharpPositionY;

	private List<Brick> Bricks = new List<Brick>();

	public int S;

	public int s;

	private float BrickPositionX;

	private float BrickPositionY;

	private List<Kinetics> Kinetics = new List<Kinetics>();

	public int K;

	public int k;

	private float KineticsPositionX;

	private float KineticsPositionY;

	private List<Vector2> points = new List<Vector2>();

	private List<Vector2> normals = new List<Vector2>();

	private List<Enemy> Enemys = new List<Enemy>();

	public int EM;

	public int em;

	private float EnemyPositionX;

	private float EnemyPositionY;

	private Vector2 start;

	private Vector2 exit;

	private static readonly Point InvalidPosition = new Point(-1, -1);

	public Random random = new Random(354668);

	private int score;

	public Texture2D SpawnBrush;

	public Vector2 ExitPosition;

	public Texture2D ExitBrush;

	public float ExitRange = 200f;

	public bool Exit_Reached_First;

	public bool IsExitRange;

	private bool IsExitRange1;

	private bool IsExitRange2;

	private bool IsExitRange3;

	private bool IsExitRange4;

	public ParticleEffect particleEffectExit;

	public ParticleEffect particleEffectStart;

	public ParticleEffect particleEffectFog;

	public ParticleEffect MineExplodeEffect;

	public SpriteBatchRenderer renderer;

	public Vector2 cameraTransformOld;

	private bool reachedExit;

	private TimeSpan timeRemaining;

	public ContentManager Content;

	private SoundEffect exitReachedSound;

	private List<SoundEffect> Songs = new List<SoundEffect>();

	private Song Song0;

	private Song Song1;

	private Song Song2;

	private Song Song3;

	private Song Song4;

	private Song Song5;

	private Song Song6;

	private Song Song7;

	private Song Song8;

	private int SongQueue;

	private Vector2 GroundBody_Position = new Vector2(10000f, 1000f);

	private Fixture GroundBody;

	private Texture2D GroundPlainStripBrush;

	private Vector2 GroundPlainStripBrush_Origin;

	public IAsyncResult result2;

	public SavedData LevelData;

	public bool[] Object_InGame;

	private int Count2;

	private float maxCameraPosition;

	private float maxHeightCameraPosition;

	public bool Exit;

	public RenderTarget2D DecalRenderer;

	public ContentManager LevelContent;

	public float KillBounds_Left_Side;

	public float KillBounds_Right_Side;

	public float KillBounds_Upper_Side;

	public float KillBounds_Lower_Side;

	public float KillBoundsMargin = 600f;

	public Texture2D _DartLightningBallTexture;

	public Texture2D _DartBoneSawTexture;

	public Texture2D _DartKineticTexture;

	public Texture2D _DartHarpoonTexture;

	public Texture2D _DartRockBallTexture;

	public Texture2D _DartBurrTexture;

	public Texture2D _Weapon_Brush;

	public ParticleEffect FireEffectLeft;

	public ParticleEffect FireEffectRight;

	public ParticleEffect FreezeEffectLeft;

	public ParticleEffect FreezeEffectRight;

	public ParticleEffect HealEffect;

	public ParticleEffect particleEffectKineticShield;

	public ParticleEffect particleEffectBleed;

	public ParticleEffect particleEffectBleeding;

	public ParticleEffect particleEffectUnconcious;

	public ParticleEffect particleEffectBloodSquirting;

	public ParticleEffect particleEffectSpirit;

	public ParticleEffect particleEffectKineticEx;

	public ParticleEffect particleEffectStasisEx;

	public ParticleEffect particleEffectCannonBallEx;

	public ParticleEffect particleEffectTeleFog;

	public SoundEffect SoundHeadPopOff;

	public SoundEffect SoundGotHit;

	public SoundEffect SoundWalking;

	public SoundEffect SoundJump;

	public SoundEffect SoundDeadBodyHit;

	public SoundEffect SoundImpailed;

	public Texture2D P1LeftHandUtilityBrush;

	public Texture2D P1RightHandUtilityBrush;

	public SoundEffect P1DartBoneSound;

	public SoundEffect P1DartHarpoonSound;

	public SoundEffect P1CannonBallSound;

	public SoundEffect P1LightningBallSound;

	public Texture2D P1_bodyBrush;

	public Texture2D P1_headBrush;

	public Texture2D P1_leftUpperArmBrush;

	public Texture2D P1_rightUpperArmBrush;

	public Texture2D P1_leftHandBrush;

	public Texture2D P1_rightHandBrush;

	public Texture2D P1_leftThighBrush;

	public Texture2D P1_rightThighBrush;

	public Texture2D P1_SightBrush;

	public Texture2D P1_TelekinisisBrush;

	public Texture2D P2LeftHandUtilityBrush;

	public Texture2D P2RightHandUtilityBrush;

	public SoundEffect P2DartBoneSound;

	public SoundEffect P2DartHarpoonSound;

	public SoundEffect P2CannonBallSound;

	public SoundEffect P2LightningBallSound;

	public Texture2D P2_bodyBrush;

	public Texture2D P2_headBrush;

	public Texture2D P2_leftUpperArmBrush;

	public Texture2D P2_rightUpperArmBrush;

	public Texture2D P2_leftHandBrush;

	public Texture2D P2_rightHandBrush;

	public Texture2D P2_leftThighBrush;

	public Texture2D P2_rightThighBrush;

	public Texture2D P2_SightBrush;

	public Texture2D P2_TelekinisisBrush;

	public Texture2D P3LeftHandUtilityBrush;

	public Texture2D P3RightHandUtilityBrush;

	public SoundEffect P3DartBoneSound;

	public SoundEffect P3DartHarpoonSound;

	public SoundEffect P3CannonBallSound;

	public SoundEffect P3LightningBallSound;

	public Texture2D P3_bodyBrush;

	public Texture2D P3_headBrush;

	public Texture2D P3_leftUpperArmBrush;

	public Texture2D P3_rightUpperArmBrush;

	public Texture2D P3_leftHandBrush;

	public Texture2D P3_rightHandBrush;

	public Texture2D P3_leftThighBrush;

	public Texture2D P3_rightThighBrush;

	public Texture2D P3_SightBrush;

	public Texture2D P3_TelekinisisBrush;

	public Texture2D P4LeftHandUtilityBrush;

	public Texture2D P4RightHandUtilityBrush;

	public SoundEffect P4DartBoneSound;

	public SoundEffect P4DartHarpoonSound;

	public SoundEffect P4CannonBallSound;

	public SoundEffect P4LightningBallSound;

	public Texture2D P4_bodyBrush;

	public Texture2D P4_headBrush;

	public Texture2D P4_leftUpperArmBrush;

	public Texture2D P4_rightUpperArmBrush;

	public Texture2D P4_leftHandBrush;

	public Texture2D P4_rightHandBrush;

	public Texture2D P4_leftThighBrush;

	public Texture2D P4_rightThighBrush;

	public Texture2D P4_SightBrush;

	public Texture2D P4_TelekinisisBrush;

	public int Score => score;

	public bool ReachedExit => reachedExit;

	public TimeSpan TimeRemaining => timeRemaining;

	public int Width => tiles.GetLength(0);

	public int Height => tiles.GetLength(1);

	public Level(PlatformerGame Game, IServiceProvider serviceProvider, string path, SpriteBatch spriteBatch)
	{
		spriteBatchLevel = spriteBatch;
		mainGame = Game;
		MusicToggle = mainGame.MusicToggle;
		SoundEffectToggle = mainGame.SoundEffectToggle;
		Blood = mainGame.BloodToggle;
		FriendlyFireToggle = mainGame.FriendlyFireToggle;
		Content = new ContentManager(serviceProvider, "Content");
		Font = Content.Load<SpriteFont>("Fonts/menufont1");
		PauseFont = Content.Load<SpriteFont>("Fonts/menufont2");
		HudFont = Content.Load<SpriteFont>("Fonts/Hud");
		Hud2Font = Content.Load<SpriteFont>("Fonts/Hud2");
		AveragePlayerPosition_Ease = new Vector2(0f, 0f);
		mainGame.Level_Exit_Reached = false;
		Data = $"Game Paused";
		Global_Scale_Width = (double)mainGame.BackBufferWidth / 1360.0;
		Global_Scale_Height = (double)mainGame.BackBufferHeight / 768.0;
		KillBoundsMargin *= mainGame.Global_Scaler;
		float num = 3f;
		ActiveZoneFixture = FixtureFactory.CreateRectangle(_world, mainGame.BackBufferWidth * PhysicsScaleDown * num, mainGame.BackBufferHeight * PhysicsScaleDown * num, 1E-06f);
		ActiveZoneFixture.Body.Position = new Vector2(0f, 0f);
		ActiveZoneFixture.Body.BodyType = BodyType.Static;
		ActiveZoneFixture.UserData = 0;
		ActiveZoneFixture.Body.UserData = 0;
		ActiveZoneFixture.Density = 1E-07f * PhysicsScaleDown;
		ActiveZoneFixture.CollisionCategories = CollisionCategory.None;
		ActiveZoneFixture.CollidesWith = CollisionCategory.None;
		exitReachedSound = Content.Load<SoundEffect>("SoundEffects/Grenade3");
		particleEffectExit = Content.Load<ParticleEffect>("Effects/Particle/Exit");
		particleEffectStart = Content.Load<ParticleEffect>("Effects/Particle/Start");
		particleEffectFog = Content.Load<ParticleEffect>("Effects/Particle/Clouds");
		renderer = new SpriteBatchRenderer
		{
			GraphicsDeviceService = mainGame.graphics
		};
		particleEffectExit.Initialise();
		particleEffectExit.LoadContent(Content);
		particleEffectStart.Initialise();
		particleEffectStart.LoadContent(Content);
		particleEffectFog.Initialise();
		particleEffectFog.LoadContent(Content);
		MineExplodeEffect = Content.Load<ParticleEffect>("Effects/Particle/MineEx");
		MineExplodeEffect.Initialise();
		MineExplodeEffect.LoadContent(Content);
		renderer.LoadContent(Content);
		timeRemaining = TimeSpan.FromMinutes(2.0);
		LoadLevel(path, spriteBatch);
		layers = new Layer[8];
		clouds = new Clouds[1];
		Fogs = new Clouds[2];
		layers[0] = new Layer(Content, "Backgrounds/Background_1", 0f, 0.01f, 0);
		layers[1] = new Layer(Content, "Backgrounds/GroundPlain", 0f, 0f, 4);
		desaturateEffect = Content.Load<Effect>("FX/desaturate");
		disappearEffect = Content.Load<Effect>("FX/disappear");
		normalmapEffect = Content.Load<Effect>("FX/normalmap");
		refractionEffect = Content.Load<Effect>("FX/refraction");
		Song0 = Content.Load<Song>("Music/0");
		Song1 = Content.Load<Song>("Music/1");
		Song2 = Content.Load<Song>("Music/2");
		Song3 = Content.Load<Song>("Music/3");
		Song4 = Content.Load<Song>("Music/4");
		Song5 = Content.Load<Song>("Music/5");
		Song6 = Content.Load<Song>("Music/6");
		Song7 = Content.Load<Song>("Music/7");
		Song8 = Content.Load<Song>("Music/8");
		Player1Color = Game.Player1Color;
		Player2Color = Game.Player2Color;
		Player3Color = Game.Player3Color;
		Player4Color = Game.Player4Color;
		PauseMenuTexture = Content.Load<Texture2D>("Menus/Pause/Intermission");
		PauseMenuBackgroundStripTexture = Content.Load<Texture2D>("Menus/Pause/PauseBackgroundStrip");
		PauseMenuControllerLayoutTexture = Content.Load<Texture2D>("Menus/Pause/LevelControllerLayout");
		PauseMenuPlayerStatsTexture_Daru = Content.Load<Texture2D>("Menus/Pause/LevelDaruStats");
		PauseMenuPlayerStatsTexture_Ernest = Content.Load<Texture2D>("Menus/Pause/LevelErnestStats");
		PauseMenuPlayerStatsTexture_Oscar = Content.Load<Texture2D>("Menus/Pause/LevelOscarStats");
		PauseMenuPlayerStatsTexture_Rick = Content.Load<Texture2D>("Menus/Pause/LevelRickStats");
		PauseMenuPlayerStatsTexture_Vinny = Content.Load<Texture2D>("Menus/Pause/LevelVinnyStats");
		ExitBrush = Content.Load<Texture2D>("LevelBuilder/BButton");
		SpawnBrush = Content.Load<Texture2D>("LevelBuilder/YButton");
		BackgroundColor = new Color(255, 255, 255, 255);
		ShadowTexture = Content.Load<Texture2D>("Shadows/Shadow1");
		ShadowTextureOrigin = new Vector2(ShadowTexture.Width / 2, ShadowTexture.Height / 2 - 5);
		waterfallTexture = Content.Load<Texture2D>("FX/waterfall");
		if (mainGame.Duel)
		{
			if (mainGame.Player1InGame)
			{
				Player1Lives = Duel_Lives;
				Player1Dead = false;
			}
			if (mainGame.Player2InGame)
			{
				Player2Lives = Duel_Lives;
				Player2Dead = false;
			}
			if (mainGame.Player3InGame)
			{
				Player3Lives = Duel_Lives;
				Player3Dead = false;
			}
			if (mainGame.Player4InGame)
			{
				Player4Lives = Duel_Lives;
				Player4Dead = false;
			}
		}
		else if (mainGame.Co_Op)
		{
			if (mainGame.Player1InGame)
			{
				PlayersLives += Co_Op_Lives;
				Player1Dead = false;
			}
			if (mainGame.Player2InGame)
			{
				PlayersLives += Co_Op_Lives;
				Player2Dead = false;
			}
			if (mainGame.Player3InGame)
			{
				PlayersLives += Co_Op_Lives;
				Player3Dead = false;
			}
			if (mainGame.Player4InGame)
			{
				PlayersLives += Co_Op_Lives;
				Player4Dead = false;
			}
		}
		Player1.Add(new Player1(this, mainGame, new Vector2(0f, 0f), _world, 1, mainGame.Player1Species, Player1Color));
		if (Player1[0] != null)
		{
			Player1[0].DestroyAll(_world);
			Player1[0] = null;
		}
		BodyCount++;
	}

	public void Load_Player_Assets()
	{
		_DartLightningBallTexture = Content.Load<Texture2D>("Darts/DartBallLightning");
		_DartBoneSawTexture = Content.Load<Texture2D>("Darts/DartBone");
		_DartKineticTexture = Content.Load<Texture2D>("Darts/DartKinetic");
		_DartHarpoonTexture = Content.Load<Texture2D>("Darts/DartHarpoon");
		_DartRockBallTexture = Content.Load<Texture2D>("Darts/DartRock");
		_DartBurrTexture = Content.Load<Texture2D>("Darts/DartStraw");
		_Weapon_Brush = Content.Load<Texture2D>("Weapons/Mace");
		particleEffectKineticEx = Content.Load<ParticleEffect>("Effects/Particle/KineticEx");
		particleEffectStasisEx = Content.Load<ParticleEffect>("Effects/Particle/StasisEx");
		particleEffectKineticShield = Content.Load<ParticleEffect>("Effects/Particle/KineticShield");
		particleEffectBleed = Content.Load<ParticleEffect>("Effects/Particle/Bleed");
		particleEffectBleeding = Content.Load<ParticleEffect>("Effects/Particle/Bleeding");
		particleEffectBloodSquirting = Content.Load<ParticleEffect>("Effects/Particle/BloodSquirting");
		particleEffectSpirit = Content.Load<ParticleEffect>("Effects/Particle/Spirit");
		FireEffectLeft = Content.Load<ParticleEffect>("Effects/Particle/Fire");
		FireEffectRight = Content.Load<ParticleEffect>("Effects/Particle/Fire");
		FreezeEffectLeft = Content.Load<ParticleEffect>("Effects/Particle/Freeze");
		FreezeEffectRight = Content.Load<ParticleEffect>("Effects/Particle/Freeze");
		HealEffect = Content.Load<ParticleEffect>("Effects/Particle/Heal");
		particleEffectUnconcious = Content.Load<ParticleEffect>("Effects/Particle/SmokingCorpse");
		particleEffectCannonBallEx = Content.Load<ParticleEffect>("Effects/Particle/LightningBall");
		particleEffectTeleFog = Content.Load<ParticleEffect>("Effects/Particle/TeleFog");
		SoundHeadPopOff = Content.Load<SoundEffect>("SoundEffects/Head pop off");
		SoundJump = Content.Load<SoundEffect>("SoundEffects/Jump");
		SoundGotHit = Content.Load<SoundEffect>("SoundEffects/DeadBodyHit");
		SoundDeadBodyHit = Content.Load<SoundEffect>("SoundEffects/DeadBodyHit");
		SoundImpailed = Content.Load<SoundEffect>("SoundEffects/BloodSplash");
		SoundWalking = Content.Load<SoundEffect>("SoundEffects/Walk");
		P1_SightBrush = Content.Load<Texture2D>("Sights/Red");
		P1_TelekinisisBrush = Content.Load<Texture2D>("LevelBuilder/CenterDot");
		string text = "Sprites/" + mainGame.Player1Species + "/";
		P1DartBoneSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
		P1DartBoneSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
		P1DartHarpoonSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
		P1CannonBallSound = Content.Load<SoundEffect>("SoundEffects/explosion");
		P1LightningBallSound = Content.Load<SoundEffect>("SoundEffects/LightningBall");
		P1_bodyBrush = Content.Load<Texture2D>(text + "body");
		P1_headBrush = Content.Load<Texture2D>(text + "head");
		P1_leftUpperArmBrush = Content.Load<Texture2D>(text + "leftArm");
		P1_rightUpperArmBrush = Content.Load<Texture2D>(text + "rightArm");
		P1_leftHandBrush = Content.Load<Texture2D>(text + "leftHand");
		P1_rightHandBrush = Content.Load<Texture2D>(text + "rightHand");
		P1_leftThighBrush = Content.Load<Texture2D>(text + "leftLeg");
		P1_rightThighBrush = Content.Load<Texture2D>(text + "rightLeg");
		if (mainGame.Player2InGame)
		{
			P2_SightBrush = Content.Load<Texture2D>("Sights/Red");
			P2_TelekinisisBrush = Content.Load<Texture2D>("LevelBuilder/CenterDot");
			text = "Sprites/" + mainGame.Player2Species + "/";
			P2DartBoneSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
			P2DartHarpoonSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
			P2CannonBallSound = Content.Load<SoundEffect>("SoundEffects/explosion");
			P2LightningBallSound = Content.Load<SoundEffect>("SoundEffects/LightningBall");
			P2_bodyBrush = Content.Load<Texture2D>(text + "body");
			P2_headBrush = Content.Load<Texture2D>(text + "head");
			P2_leftUpperArmBrush = Content.Load<Texture2D>(text + "leftArm");
			P2_rightUpperArmBrush = Content.Load<Texture2D>(text + "rightArm");
			P2_leftHandBrush = Content.Load<Texture2D>(text + "leftHand");
			P2_rightHandBrush = Content.Load<Texture2D>(text + "rightHand");
			P2_leftThighBrush = Content.Load<Texture2D>(text + "leftLeg");
			P2_rightThighBrush = Content.Load<Texture2D>(text + "rightLeg");
		}
		if (mainGame.Player3InGame)
		{
			P3_SightBrush = Content.Load<Texture2D>("Sights/Red");
			P3_TelekinisisBrush = Content.Load<Texture2D>("LevelBuilder/CenterDot");
			text = "Sprites/" + mainGame.Player3Species + "/";
			P3DartBoneSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
			P3DartHarpoonSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
			P3CannonBallSound = Content.Load<SoundEffect>("SoundEffects/explosion");
			P3LightningBallSound = Content.Load<SoundEffect>("SoundEffects/LightningBall");
			P3_bodyBrush = Content.Load<Texture2D>(text + "body");
			P3_headBrush = Content.Load<Texture2D>(text + "head");
			P3_leftUpperArmBrush = Content.Load<Texture2D>(text + "leftArm");
			P3_rightUpperArmBrush = Content.Load<Texture2D>(text + "rightArm");
			P3_leftHandBrush = Content.Load<Texture2D>(text + "leftHand");
			P3_rightHandBrush = Content.Load<Texture2D>(text + "rightHand");
			P3_leftThighBrush = Content.Load<Texture2D>(text + "leftLeg");
			P3_rightThighBrush = Content.Load<Texture2D>(text + "rightLeg");
		}
		if (mainGame.Player4InGame)
		{
			P4_SightBrush = Content.Load<Texture2D>("Sights/Red");
			P4_TelekinisisBrush = Content.Load<Texture2D>("LevelBuilder/CenterDot");
			text = "Sprites/" + mainGame.Player4Species + "/";
			P4DartBoneSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
			P4DartHarpoonSound = Content.Load<SoundEffect>("SoundEffects/hitpipe");
			P4CannonBallSound = Content.Load<SoundEffect>("SoundEffects/explosion");
			P4LightningBallSound = Content.Load<SoundEffect>("SoundEffects/LightningBall");
			P4_bodyBrush = Content.Load<Texture2D>(text + "body");
			P4_headBrush = Content.Load<Texture2D>(text + "head");
			P4_leftUpperArmBrush = Content.Load<Texture2D>(text + "leftArm");
			P4_rightUpperArmBrush = Content.Load<Texture2D>(text + "rightArm");
			P4_leftHandBrush = Content.Load<Texture2D>(text + "leftHand");
			P4_rightHandBrush = Content.Load<Texture2D>(text + "rightHand");
			P4_leftThighBrush = Content.Load<Texture2D>(text + "leftLeg");
			P4_rightThighBrush = Content.Load<Texture2D>(text + "rightLeg");
		}
	}

	private void LoadLevel(string path, SpriteBatch spriteBatch)
	{
		try
		{
			Load_Player_Assets();
			GroundBody = FixtureFactory.CreateRectangle(_world, 20000f, 300f, 100f);
			GroundBody.Body.Position = GroundBody_Position * PhysicsScaleDown;
			GroundBody.Body.Rotation = 0f;
			GroundBody.Friction = 1f;
			GroundBody.Body.SleepingAllowed = true;
			GroundBody.Body.BodyType = BodyType.Static;
			GroundBody.CollisionCategories = CollisionCategory.Cat30;
			GroundBody.CollisionGroup = 365;
			GroundBody.UserData = 7999;
			GroundBody.Body.UserData = 7999;
			LevelData = LoadData(path);
			Object_InGame = new bool[LevelData.Count];
			Count2 = LevelData.Count;
			for (int i = 0; i < LevelData.Count; i++)
			{
				if (LevelData.ObjectType[i] != 0)
				{
					if (LevelData.ObjectType[i] == 1)
					{
						B++;
						Blocks.Add(new Blocks(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], i));
					}
					if (LevelData.ObjectType[i] == 2)
					{
						S++;
						Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, i));
					}
					if (LevelData.ObjectType[i] == 3)
					{
						if (LevelData.ObjectSubType[i] == "0")
						{
							S++;
							Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, i));
						}
						else if (LevelData.ObjectSubType[i] == "1")
						{
							S++;
							Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, i));
						}
						else if (LevelData.ObjectSubType[i] == "2")
						{
							S++;
							Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, i));
						}
						else if (LevelData.ObjectSubType[i] == "0")
						{
							S++;
							Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, 0));
						}
						else if (LevelData.ObjectSubType[i] == "1")
						{
							S++;
							Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, 0));
						}
						else if (LevelData.ObjectSubType[i] == "2")
						{
							S++;
							Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, 0));
						}
						else
						{
							SH++;
							Sharps.Add(new Sharps(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], 0));
						}
					}
					if (LevelData.ObjectType[i] == 4)
					{
						K++;
						Kinetics.Add(new Kinetics(Content, null, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], renderer, i));
					}
					if (LevelData.ObjectType[i] == 5)
					{
						ExitPosition = LevelData.ObjectPosition[i];
					}
					Object_InGame[i] = true;
				}
				if (LevelData.ObjectPosition[i].X < KillBounds_Left_Side)
				{
					KillBounds_Left_Side = -10000f;
				}
				if (LevelData.ObjectPosition[i].X > KillBounds_Right_Side)
				{
					KillBounds_Right_Side = 16000f;
				}
				if (LevelData.ObjectPosition[i].Y < KillBounds_Upper_Side)
				{
					KillBounds_Upper_Side = LevelData.ObjectPosition[i].Y;
				}
				if (LevelData.ObjectPosition[i].Y > KillBounds_Lower_Side)
				{
					KillBounds_Lower_Side = 200f;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void LoadLevel_Saved(string path, Stream stream, SpriteBatch spriteBatch)
	{
		Load_Player_Assets();
		LevelData = LoadData(path);
		Object_InGame = new bool[LevelData.Count];
		Count2 = LevelData.Count;
		for (int i = 0; i < LevelData.Count; i++)
		{
			float backBufferWidth = mainGame.BackBufferWidth;
			float backBufferHeight = mainGame.BackBufferHeight;
			if (LevelData.ObjectPosition[i].X >= PlayerPosition_Vec.X + backBufferWidth || LevelData.ObjectPosition[i].X <= PlayerPosition_Vec.X - backBufferWidth || LevelData.ObjectPosition[i].Y >= PlayerPosition_Vec.Y + backBufferHeight || LevelData.ObjectPosition[i].Y <= PlayerPosition_Vec.Y - backBufferHeight)
			{
				continue;
			}
			if (LevelData.ObjectType[i] != 0)
			{
				if (LevelData.ObjectType[i] == 1)
				{
					L++;
					Lands.Add(new Lands(Content, this, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], i));
				}
				if (LevelData.ObjectType[i] == 2)
				{
					S++;
					Bricks.Add(new Brick(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, i));
				}
				if (LevelData.ObjectType[i] == 3)
				{
					B++;
					Blocks.Add(new Blocks(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], i));
				}
				if (LevelData.ObjectType[i] == 5)
				{
					SH++;
					Sharps.Add(new Sharps(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], i));
				}
				if (LevelData.ObjectType[i] == 6)
				{
					K++;
					Kinetics.Add(new Kinetics(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], renderer, i));
				}
				if (LevelData.ObjectType[i] == 7)
				{
					EM++;
					Enemys.Add(new Enemy(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], i));
				}
				if (LevelData.ObjectType[i] == 9)
				{
					ExitPosition = LevelData.ObjectPosition[i];
				}
				Object_InGame[i] = true;
			}
			if (LevelData.ObjectPosition[i].X < KillBounds_Left_Side)
			{
				KillBounds_Left_Side = LevelData.ObjectPosition[i].X * PhysicsScaleDown;
			}
			if (LevelData.ObjectPosition[i].X > KillBounds_Right_Side)
			{
				KillBounds_Right_Side = LevelData.ObjectPosition[i].X * PhysicsScaleDown;
			}
			if (LevelData.ObjectPosition[i].Y < KillBounds_Upper_Side)
			{
				KillBounds_Upper_Side = LevelData.ObjectPosition[i].Y * PhysicsScaleDown;
			}
			if (LevelData.ObjectPosition[i].Y > KillBounds_Lower_Side)
			{
				KillBounds_Lower_Side = LevelData.ObjectPosition[i].Y * PhysicsScaleDown;
			}
		}
	}

	public static StorageContainer OpenContainer(StorageDevice storageDevice, string saveGameName)
	{
		if (storageDevice != null && storageDevice.IsConnected)
		{
			IAsyncResult asyncResult = storageDevice.BeginOpenContainer(saveGameName, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer result = storageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			return result;
		}
		return null;
	}

	public SavedData LoadData(string path)
	{
		SavedData result = default(SavedData);
		try
		{
			if (mainGame.storageDevice.IsConnected)
			{
				StorageContainer storageContainer = OpenContainer(mainGame.storageDevice, "Totof_Levels");
				if (storageContainer == null)
				{
					mainGame.InLevelMode = false;
					mainGame.InMainMenuMode = true;
					mainGame.MainMenuFadeIn = true;
					mainGame.MainMenuFadeOut = false;
					MediaPlayer.Stop();
					MediaPlayer.Play(Song0);
				}
				else
				{
					using (storageContainer)
					{
						using Stream stream = storageContainer.OpenFile(path, FileMode.Open);
						try
						{
							if (mainGame.storageDevice.IsConnected)
							{
								XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedData));
								result = (SavedData)xmlSerializer.Deserialize(stream);
								return result;
							}
						}
						catch (Exception)
						{
						}
						finally
						{
							stream.Close();
						}
					}
				}
			}
		}
		catch (StorageDeviceNotConnectedException)
		{
		}
		return result;
	}

	private Tile LoadTile(string name, TileCollision collision)
	{
		return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
	}

	private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
	{
		int num = random.Next(variationCount);
		return LoadTile(baseName + num, collision);
	}

	private Tile LoadPlayerTile(int x, int y)
	{
		return new Tile(null, TileCollision.Passable);
	}

	private Tile LoadExitTile(int x, int y)
	{
		exit = new Vector2(x * 64, y * 48);
		return LoadTile("Exit", TileCollision.Passable);
	}

	private Tile LoadLandTile(int x, int y)
	{
		L++;
		_ = GetBounds(x, y).Center;
		return new Tile(null, TileCollision.Passable);
	}

	private Tile LoadBlockTile(int x, int y)
	{
		B++;
		_ = GetBounds(x, y).Center;
		return new Tile(null, TileCollision.Impassable);
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		if (Thread_Music != null)
		{
			Thread_Music.Abort();
		}
		if (Thread_Physics != null)
		{
			Thread_Physics.Abort();
		}
		if (Thread_Camera != null)
		{
			Thread_Camera.Abort();
		}
	}

	public void Pause(bool gamePaused, int Player)
	{
		if (!gamePaused)
		{
			PlayerPausedIndex = 0;
		}
		else
		{
			PlayerPausedIndex = Player;
		}
		Paused = gamePaused;
	}

	public void BloodMode(bool BloodMode)
	{
		Blood = BloodMode;
	}

	public void CheckIfExitReached()
	{
		if (BodyCount > -1)
		{
			if (Player1[Player1Index].Alive && exit.Length() - Player1[Player1Index]._bodyBodyPosition.Length() < 100f)
			{
				exitReached = true;
			}
			if (Player1[Player2Index].Alive && exit.Length() - Player1[Player2Index]._bodyBodyPosition.Length() < 100f)
			{
				exitReached = true;
			}
			if (Player1[Player3Index].Alive && exit.Length() - Player1[Player3Index]._bodyBodyPosition.Length() < 100f)
			{
				exitReached = true;
			}
			if (Player1[Player4Index].Alive && exit.Length() - Player1[Player4Index]._bodyBodyPosition.Length() < 100f)
			{
				exitReached = true;
			}
		}
	}

	public TileCollision GetCollision(int x, int y)
	{
		if (x < 0 || x >= Width)
		{
			return TileCollision.Impassable;
		}
		if (y < 0 || y >= Height)
		{
			return TileCollision.Passable;
		}
		return tiles[x, y].Collision;
	}

	public Rectangle GetBounds(int x, int y)
	{
		return new Rectangle(x * 64, y * 48, 64, 48);
	}

	public void AddObjectToLevel(int LevelDataIndex)
	{
		if (LevelData.ObjectType[LevelDataIndex] == 0)
		{
			return;
		}
		if (LevelData.ObjectType[LevelDataIndex] == 1)
		{
			for (int i = 0; i < B; i++)
			{
				if (Blocks[i] != null && Blocks[i].LevelDataIndex == LevelDataIndex)
				{
					Blocks[i].Active = true;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] == 2)
		{
			for (int j = 0; j < S; j++)
			{
				if (Bricks[j] != null && Bricks[j].LevelDataIndex == LevelDataIndex)
				{
					Bricks[j].Active = true;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] == 3)
		{
			for (int k = 0; k < SH; k++)
			{
				if (Sharps[k] != null && Sharps[k].LevelDataIndex == LevelDataIndex)
				{
					Sharps[k].Active = true;
				}
			}
			for (int l = 0; l < S; l++)
			{
				if (Bricks[l] != null && Bricks[l].LevelDataIndex == LevelDataIndex)
				{
					Bricks[l].Active = true;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] != 4)
		{
			return;
		}
		for (int m = 0; m < K; m++)
		{
			if (Kinetics[m] != null && Kinetics[m].LevelDataIndex == LevelDataIndex)
			{
				Kinetics[m].Active = true;
			}
		}
	}

	public void RemoveObjectFromLevel(int LevelDataIndex)
	{
		if (LevelData.ObjectType[LevelDataIndex] == 0)
		{
			return;
		}
		if (LevelData.ObjectType[LevelDataIndex] == 1)
		{
			for (int i = 0; i < B; i++)
			{
				if (Blocks[i] != null && Blocks[i].LevelDataIndex == LevelDataIndex)
				{
					Blocks[i].Active = false;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] == 2)
		{
			for (int j = 0; j < S; j++)
			{
				if (Bricks[j] != null && Bricks[j].LevelDataIndex == LevelDataIndex)
				{
					Bricks[j].Active = false;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] == 3)
		{
			for (int k = 0; k < S; k++)
			{
				if (Bricks[k] != null && Bricks[k].LevelDataIndex == LevelDataIndex)
				{
					Bricks[k].Active = false;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] == 3)
		{
			for (int l = 0; l < SH; l++)
			{
				if (Sharps[l] != null && Sharps[l].LevelDataIndex == LevelDataIndex)
				{
					Sharps[l].Active = false;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] != 4)
		{
			return;
		}
		for (int m = 0; m < K; m++)
		{
			if (Kinetics[m] != null && Kinetics[m].LevelDataIndex == LevelDataIndex)
			{
				Kinetics[m].Active = false;
			}
		}
	}

	public void UpdateObjectPositionInLevel(int LevelDataIndex)
	{
		if (LevelData.ObjectType[LevelDataIndex] == 0)
		{
			return;
		}
		if (LevelData.ObjectType[LevelDataIndex] == 2)
		{
			for (int i = 0; i < S; i++)
			{
				if (Bricks[i] != null && Bricks[i].LevelDataIndex == LevelDataIndex && LevelData.ObjectPosition[LevelDataIndex] != Bricks[i].BrickBody.Body.Position)
				{
					ref Vector2 reference = ref LevelData.ObjectPosition[LevelDataIndex];
					reference = Bricks[i].BrickBody.Body.Position;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] == 3)
		{
			for (int j = 0; j < S; j++)
			{
				if (Bricks[j] != null && Bricks[j].LevelDataIndex == LevelDataIndex && LevelData.ObjectPosition[LevelDataIndex] != Bricks[j].BrickBody.Body.Position)
				{
					ref Vector2 reference2 = ref LevelData.ObjectPosition[LevelDataIndex];
					reference2 = Bricks[j].BrickBody.Body.Position;
				}
			}
		}
		if (LevelData.ObjectType[LevelDataIndex] != 3)
		{
			return;
		}
		for (int k = 0; k < SH; k++)
		{
			if (Sharps[k] != null && Sharps[k].LevelDataIndex == LevelDataIndex && LevelData.ObjectPosition[LevelDataIndex] != Sharps[k].SharpBody.Body.Position)
			{
				ref Vector2 reference3 = ref LevelData.ObjectPosition[LevelDataIndex];
				reference3 = Sharps[k].SharpBody.Body.Position;
			}
		}
	}

	public void Update_Physics_1()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		int num = 0;
		while (num < 200)
		{
			if (Paused || Exit_Reached_First)
			{
				continue;
			}
			if (Scene_Update_Pacer.TotalMilliseconds + 100.0 < gameTime_Physics.TotalGameTime.TotalMilliseconds)
			{
				if (mainGame.IsHD)
				{
					for (int i = 0; i < LevelData.Count; i++)
					{
						float num2 = mainGame.BackBufferWidth * 4.25f;
						float num3 = mainGame.BackBufferHeight * 4.25f;
						if (LevelData.ObjectType[i] == 2 || LevelData.ObjectType[i] == 3)
						{
							UpdateObjectPositionInLevel(i);
						}
						if (LevelData.ObjectType[i] == 1)
						{
							if (LevelData.ObjectPosition[i].X >= PlayerPosition_Vec.X + num2)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].X <= PlayerPosition_Vec.X - num2)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y >= PlayerPosition_Vec.Y + num3)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y <= PlayerPosition_Vec.Y - num3)
							{
								RemoveObjectFromLevel(i);
							}
							else
							{
								AddObjectToLevel(i);
							}
						}
						else if (LevelData.ObjectType[i] == 2)
						{
							if (LevelData.ObjectPosition[i].X >= (PlayerPosition_Vec.X + num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].X <= (PlayerPosition_Vec.X - num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y >= (PlayerPosition_Vec.Y + num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y <= (PlayerPosition_Vec.Y - num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else
							{
								AddObjectToLevel(i);
							}
						}
						else if (LevelData.ObjectType[i] == 3)
						{
							if (LevelData.ObjectPosition[i].X >= (PlayerPosition_Vec.X + num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].X <= (PlayerPosition_Vec.X - num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y >= (PlayerPosition_Vec.Y + num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y <= (PlayerPosition_Vec.Y - num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else
							{
								AddObjectToLevel(i);
							}
						}
						else if (LevelData.ObjectPosition[i].X >= (PlayerPosition_Vec.X + num2 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else if (LevelData.ObjectPosition[i].X <= (PlayerPosition_Vec.X - num2 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else if (LevelData.ObjectPosition[i].Y >= (PlayerPosition_Vec.Y + num3 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else if (LevelData.ObjectPosition[i].Y <= (PlayerPosition_Vec.Y - num3 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else
						{
							AddObjectToLevel(i);
						}
					}
				}
				else
				{
					for (int j = 0; j < LevelData.Count; j++)
					{
						float num4 = mainGame.BackBufferWidth * 4.25f;
						float num5 = mainGame.BackBufferHeight * 5.25f;
						if (LevelData.ObjectType[j] == 2 || LevelData.ObjectType[j] == 3)
						{
							UpdateObjectPositionInLevel(j);
						}
						if (LevelData.ObjectType[j] == 1)
						{
							if (LevelData.ObjectPosition[j].X >= PlayerPosition_Vec.X + num4)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].X <= PlayerPosition_Vec.X - num4)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y >= PlayerPosition_Vec.Y + num5)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y <= PlayerPosition_Vec.Y - num5)
							{
								RemoveObjectFromLevel(j);
							}
							else
							{
								AddObjectToLevel(j);
							}
						}
						else if (LevelData.ObjectType[j] == 2)
						{
							if (LevelData.ObjectPosition[j].X >= (PlayerPosition_Vec.X + num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].X <= (PlayerPosition_Vec.X - num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y >= (PlayerPosition_Vec.Y + num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y <= (PlayerPosition_Vec.Y - num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else
							{
								AddObjectToLevel(j);
							}
						}
						else if (LevelData.ObjectType[j] == 3)
						{
							if (LevelData.ObjectPosition[j].X >= (PlayerPosition_Vec.X + num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].X <= (PlayerPosition_Vec.X - num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y >= (PlayerPosition_Vec.Y + num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y <= (PlayerPosition_Vec.Y - num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else
							{
								AddObjectToLevel(j);
							}
						}
						else if (LevelData.ObjectPosition[j].X >= (PlayerPosition_Vec.X + num4 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else if (LevelData.ObjectPosition[j].X <= (PlayerPosition_Vec.X - num4 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else if (LevelData.ObjectPosition[j].Y >= (PlayerPosition_Vec.Y + num5 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else if (LevelData.ObjectPosition[j].Y <= (PlayerPosition_Vec.Y - num5 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else
						{
							AddObjectToLevel(j);
						}
					}
				}
				Scene_Update_Pacer = gameTime_Physics.TotalGameTime;
				Update_2(gameTime_Physics);
			}
			if (gameTime_Physics.TotalGameTime.TotalMilliseconds - gameTime_Physics_Old.TotalMilliseconds > 16.399999618530273)
			{
				for (int k = 0; k < Bricks.Count; k++)
				{
					if (Bricks[k] != null)
					{
						Bricks[k].Update(gameTime_Physics, _world);
					}
				}
				PlayerPosition();
				for (int l = 0; l < Player1.Count; l++)
				{
					if (Player1[l] != null)
					{
						Player1[l].Update_Physics(gameTime_Physics, _world);
					}
				}
				gameTime_Physics_Old = gameTime_Physics.TotalGameTime;
				Physics_Speed = 15f;
				_world.Step(Physics_Speed * 0.001f);
			}
			else
			{
				_ = gameTime_Physics.TotalGameTime.TotalMilliseconds - gameTime_Physics_Old.TotalMilliseconds;
				_ = 16.0;
			}
		}
	}

	public void Update_Physics_1_Old_Saved_08_04_13()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		int num = 0;
		while (num < 200)
		{
			if (Paused || Exit_Reached_First)
			{
				continue;
			}
			if (Scene_Update_Pacer.TotalMilliseconds + 100.0 < gameTime_Physics.TotalGameTime.TotalMilliseconds)
			{
				if (mainGame.IsHD)
				{
					for (int i = 0; i < LevelData.Count; i++)
					{
						float num2 = mainGame.BackBufferWidth * 4.25f;
						float num3 = mainGame.BackBufferHeight * 4.25f;
						if (LevelData.ObjectType[i] == 2 || LevelData.ObjectType[i] == 3)
						{
							UpdateObjectPositionInLevel(i);
						}
						if (LevelData.ObjectType[i] == 1)
						{
							if (LevelData.ObjectPosition[i].X >= PlayerPosition_Vec.X + num2)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].X <= PlayerPosition_Vec.X - num2)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y >= PlayerPosition_Vec.Y + num3)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y <= PlayerPosition_Vec.Y - num3)
							{
								RemoveObjectFromLevel(i);
							}
							else
							{
								AddObjectToLevel(i);
							}
						}
						else if (LevelData.ObjectType[i] == 2)
						{
							if (LevelData.ObjectPosition[i].X >= (PlayerPosition_Vec.X + num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].X <= (PlayerPosition_Vec.X - num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y >= (PlayerPosition_Vec.Y + num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y <= (PlayerPosition_Vec.Y - num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else
							{
								AddObjectToLevel(i);
							}
						}
						else if (LevelData.ObjectType[i] == 3)
						{
							if (LevelData.ObjectPosition[i].X >= (PlayerPosition_Vec.X + num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].X <= (PlayerPosition_Vec.X - num2 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y >= (PlayerPosition_Vec.Y + num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else if (LevelData.ObjectPosition[i].Y <= (PlayerPosition_Vec.Y - num3 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(i);
							}
							else
							{
								AddObjectToLevel(i);
							}
						}
						else if (LevelData.ObjectPosition[i].X >= (PlayerPosition_Vec.X + num2 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else if (LevelData.ObjectPosition[i].X <= (PlayerPosition_Vec.X - num2 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else if (LevelData.ObjectPosition[i].Y >= (PlayerPosition_Vec.Y + num3 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else if (LevelData.ObjectPosition[i].Y <= (PlayerPosition_Vec.Y - num3 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(i);
						}
						else
						{
							AddObjectToLevel(i);
						}
					}
				}
				else
				{
					for (int j = 0; j < LevelData.Count; j++)
					{
						float num4 = mainGame.BackBufferWidth * 4.25f;
						float num5 = mainGame.BackBufferHeight * 5.25f;
						if (LevelData.ObjectType[j] == 2 || LevelData.ObjectType[j] == 3)
						{
							UpdateObjectPositionInLevel(j);
						}
						if (LevelData.ObjectType[j] == 1)
						{
							if (LevelData.ObjectPosition[j].X >= PlayerPosition_Vec.X + num4)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].X <= PlayerPosition_Vec.X - num4)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y >= PlayerPosition_Vec.Y + num5)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y <= PlayerPosition_Vec.Y - num5)
							{
								RemoveObjectFromLevel(j);
							}
							else
							{
								AddObjectToLevel(j);
							}
						}
						else if (LevelData.ObjectType[j] == 2)
						{
							if (LevelData.ObjectPosition[j].X >= (PlayerPosition_Vec.X + num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].X <= (PlayerPosition_Vec.X - num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y >= (PlayerPosition_Vec.Y + num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y <= (PlayerPosition_Vec.Y - num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else
							{
								AddObjectToLevel(j);
							}
						}
						else if (LevelData.ObjectType[j] == 3)
						{
							if (LevelData.ObjectPosition[j].X >= (PlayerPosition_Vec.X + num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].X <= (PlayerPosition_Vec.X - num4 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y >= (PlayerPosition_Vec.Y + num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else if (LevelData.ObjectPosition[j].Y <= (PlayerPosition_Vec.Y - num5 * 0.85f) * PhysicsScaleDown)
							{
								RemoveObjectFromLevel(j);
							}
							else
							{
								AddObjectToLevel(j);
							}
						}
						else if (LevelData.ObjectPosition[j].X >= (PlayerPosition_Vec.X + num4 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else if (LevelData.ObjectPosition[j].X <= (PlayerPosition_Vec.X - num4 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else if (LevelData.ObjectPosition[j].Y >= (PlayerPosition_Vec.Y + num5 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else if (LevelData.ObjectPosition[j].Y <= (PlayerPosition_Vec.Y - num5 * 0.95f) * PhysicsScaleDown)
						{
							RemoveObjectFromLevel(j);
						}
						else
						{
							AddObjectToLevel(j);
						}
					}
				}
				Scene_Update_Pacer = gameTime_Physics.TotalGameTime;
				Update_2(gameTime_Physics);
			}
			for (int k = 0; k < Bricks.Count; k++)
			{
				if (Bricks[k] != null)
				{
					Bricks[k].Update(gameTime_Physics, _world);
				}
			}
			PlayerPosition();
			for (int l = 0; l < Player1.Count; l++)
			{
				if (Player1[l] != null)
				{
					Player1[l].Update_Physics(gameTime_Physics, _world);
				}
			}
			if (gameTime_Physics.TotalGameTime.TotalMilliseconds - gameTime_Physics_Old.TotalMilliseconds > 16.399999618530273)
			{
				PhysicsSleepMilli--;
				if (PhysicsSleepMilli < 0)
				{
					PhysicsSleepMilli = 0;
				}
			}
			else if (gameTime_Physics.TotalGameTime.TotalMilliseconds - gameTime_Physics_Old.TotalMilliseconds < 16.0)
			{
				PhysicsSleepMilli++;
			}
			if (gameTime_Physics_Old.TotalMilliseconds + 1.0 < gameTime_Physics.TotalGameTime.TotalMilliseconds)
			{
				if (TesterPhysics_Cycle == 0)
				{
					TesterData_PhysicsThread_3 = (gameTime_Physics.TotalGameTime.TotalMilliseconds - gameTime_Physics_Old.TotalMilliseconds).ToString();
					gameTime_Physics_Old = gameTime_Physics.TotalGameTime;
					TesterPhysics_Cycle = 0;
					TesterData_PhysicsThread_1 = TesterPhysics_Cycle.ToString();
				}
				else
				{
					TesterData_PhysicsThread_1 = TesterPhysics_Cycle.ToString();
					gameTime_Physics_Old = gameTime_Physics.TotalGameTime;
					TesterPhysics_Cycle = 0;
					TesterData_PhysicsThread_3 = (gameTime_Physics.TotalGameTime.TotalMilliseconds - gameTime_Physics_Old.TotalMilliseconds).ToString();
				}
			}
			else
			{
				TesterPhysics_Cycle++;
			}
			Physics_Speed = 15f;
			TesterData_PhysicsThread_2 = PhysicsSleepMilli;
			_world.Step(Physics_Speed * 0.001f);
			if (PhysicsSleepMilli >= 1)
			{
				Thread.Sleep(PhysicsSleepMilli);
			}
		}
	}

	public void Update_Physics_1_Saved()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		int num = 0;
		while (num < 200)
		{
			if (Paused)
			{
				continue;
			}
			for (int i = 0; i < LevelData.Count; i++)
			{
				float backBufferWidth = mainGame.BackBufferWidth;
				float backBufferHeight = mainGame.BackBufferHeight;
				if (LevelData.ObjectPosition[i].X >= PlayerPosition_Vec.X + backBufferWidth)
				{
					RemoveObjectFromLevel(i);
				}
				else if (LevelData.ObjectPosition[i].X <= PlayerPosition_Vec.X - backBufferWidth)
				{
					RemoveObjectFromLevel(i);
				}
				else if (LevelData.ObjectPosition[i].Y >= PlayerPosition_Vec.Y + backBufferHeight)
				{
					RemoveObjectFromLevel(i);
				}
				else if (LevelData.ObjectPosition[i].Y <= PlayerPosition_Vec.Y - backBufferHeight)
				{
					RemoveObjectFromLevel(i);
				}
				else if (!Object_InGame[i] && LevelData.ObjectType[i] != 0)
				{
					if (LevelData.ObjectType[i] == 1)
					{
						L++;
						Lands.Add(new Lands(Content, this, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], i));
						Object_InGame[i] = true;
					}
					if (LevelData.ObjectType[i] == 2)
					{
						S++;
						Bricks.Add(new Brick(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], renderer, i));
						Object_InGame[i] = true;
					}
					if (LevelData.ObjectType[i] == 3)
					{
						B++;
						Blocks.Add(new Blocks(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectType[i], LevelData.ObjectRotation[i], i));
						Object_InGame[i] = true;
					}
					if (LevelData.ObjectType[i] == 5)
					{
						SH++;
						Sharps.Add(new Sharps(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], i));
						Object_InGame[i] = true;
					}
					if (LevelData.ObjectType[i] == 6)
					{
						K++;
						Kinetics.Add(new Kinetics(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], renderer, i));
						Object_InGame[i] = true;
					}
					if (LevelData.ObjectType[i] == 7)
					{
						EM++;
						Enemys.Add(new Enemy(Content, this, mainGame, LevelData.ObjectPosition[i], _world, LevelData.ObjectSubType[i], LevelData.ObjectRotation[i], i));
						Object_InGame[i] = true;
					}
				}
			}
			Update_2(gameTime_Physics);
			for (int j = 0; j < Player1.Count; j++)
			{
				if (Player1[j] != null)
				{
					Player1[j].Update_Physics(gameTime_Physics, _world);
				}
			}
			_world.Step(Physics_Speed * 0.001f);
			Thread.Sleep(PhysicsSleepMilli);
		}
	}

	public void Update_Physics_2(GameTime gameTime)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		_world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
		for (int i = 0; i < Player1.Count; i++)
		{
			if (Player1[i] != null)
			{
				Player1[i].Update_Physics(gameTime, _world);
			}
		}
	}

	public void Update_OLD_2(GameTime gameTime)
	{
		if (!FirstTime)
		{
			FirstTime = true;
			Thread.Sleep(500);
			Thread_Physics = new Thread((ThreadStart)delegate
			{
				Update_Physics_1();
			});
			Thread_Physics.Start();
		}
		if (!Paused)
		{
			if (BodyCount == -1)
			{
				BodyCount = 0;
				Player1.Add(new Player1(this, mainGame, new Vector2(cameraPosition + 640f, cameraHeightPosition + 360f), _world, 1, mainGame.Player1Species, Player1Color));
				if (Player1[0] != null)
				{
					Player1[0].DestroyAll(_world);
					Player1[0] = null;
				}
			}
			if (Thread_Physics_Done)
			{
				foreach (Brick brick in Bricks)
				{
					brick.Update(gameTime, _world);
				}
				foreach (Kinetics kinetic in Kinetics)
				{
					kinetic?.Update(gameTime, _world);
				}
				foreach (Sharps sharp in Sharps)
				{
					sharp?.Update(gameTime, _world);
				}
				foreach (Enemy enemy in Enemys)
				{
					enemy?.Update(gameTime, _world);
				}
				foreach (Player1 item in Player1)
				{
					item?.Update(gameTime);
				}
			}
		}
		if (mainGame.Duel)
		{
			if (Player1Lives < 0 && !Player1InGame && mainGame.Player1InGame)
			{
				Player1Dead = true;
			}
			if (Player2Lives < 0 && !Player2InGame && mainGame.Player2InGame)
			{
				Player2Dead = true;
			}
			if (Player3Lives < 0 && !Player3InGame && mainGame.Player3InGame)
			{
				Player3Dead = true;
			}
			if (Player4Lives < 0 && !Player4InGame && mainGame.Player4InGame)
			{
				Player4Dead = true;
			}
			if (!Player1Dead && Player2Dead && Player3Dead && Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 1;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
			else if (Player1Dead && !Player2Dead && Player3Dead && Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 2;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
			else if (Player1Dead && Player2Dead && !Player3Dead && Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 3;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
			else if (Player1Dead && Player2Dead && Player3Dead && !Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 4;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
		}
		else if (mainGame.Co_Op && PlayersLives < 0)
		{
			mainGame.InLevelMode = false;
			mainGame.InMainMenuMode = true;
			mainGame.MainMenuFadeIn = true;
			mainGame.MainMenuFadeOut = false;
			MediaPlayer.Stop();
			MediaPlayer.Play(Song0);
			mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		for (int num = 0; num < Player1.Count; num++)
		{
			if (Player1Index == num || Player2Index == num || Player3Index == num || Player4Index == num)
			{
				continue;
			}
			if (BodiesOnMap)
			{
				if (Player1[num] != null && Player1[num].DeadTimer == Player1[num].DeadTimerMax)
				{
					Player1[num].DestroyAll(_world);
					Player1[num] = null;
				}
			}
			else if (Player1[num] != null)
			{
				Player1[num].DestroyAll(_world);
				Player1[num] = null;
			}
		}
		Update_Music(gameTime);
		float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
		particleEffectExit.Update(deltaSeconds);
		particleEffectStart.Update(deltaSeconds);
		particleEffectFog.Update(deltaSeconds);
		MineExplodeEffect.Update(deltaSeconds);
		if (Paused)
		{
			return;
		}
		if (Keyboard.GetState().IsKeyDown(Keys.F12))
		{
			MediaPlayer.Stop();
			mainGame.InMainMenuMode = true;
			mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		int val = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0);
		val = Math.Min(val, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
		timeRemaining -= TimeSpan.FromSeconds(val);
		score += val * 5;
		_ = ExitPosition != new Vector2(0f, 0f);
		if (mainGame.Duel)
		{
			if (Player1InGame)
			{
				if (Player1[Player1Index] != null && !Player1[Player1Index].Alive)
				{
					if (Player1Lives > 0)
					{
						CheckRespawnPlayer1();
					}
					else
					{
						Player1InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer1();
			}
			if (Player2InGame)
			{
				if (Player1[Player2Index] != null && !Player1[Player2Index].Alive)
				{
					if (Player2Lives > 0)
					{
						CheckRespawnPlayer2();
					}
					else
					{
						Player2InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer2();
			}
			if (Player3InGame)
			{
				if (Player1[Player3Index] != null && !Player1[Player3Index].Alive)
				{
					if (Player3Lives > 0)
					{
						CheckRespawnPlayer3();
					}
					else
					{
						Player3InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer3();
			}
			if (Player4InGame)
			{
				if (Player1[Player4Index] != null && !Player1[Player4Index].Alive)
				{
					if (Player4Lives > 0)
					{
						CheckRespawnPlayer4();
					}
					else
					{
						Player4InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer4();
			}
		}
		else if (mainGame.Co_Op)
		{
			if (Player1InGame)
			{
				if (Player1[Player1Index] != null && !Player1[Player1Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer1();
					}
					else
					{
						Player1InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer1();
			}
			if (Player2InGame)
			{
				if (Player1[Player2Index] != null && !Player1[Player2Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer2();
					}
					else
					{
						Player2InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer2();
			}
			if (Player3InGame)
			{
				if (Player1[Player3Index] != null && !Player1[Player3Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer3();
					}
					else
					{
						Player3InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer3();
			}
			if (Player4InGame)
			{
				if (Player1[Player4Index] != null && !Player1[Player4Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer4();
					}
					else
					{
						Player4InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer4();
			}
		}
		if (Player1InGame || Player2InGame || Player3InGame || Player4InGame)
		{
			AnyAlive = true;
		}
		if (!Player1InGame)
		{
			CheckRespawnPlayer1();
		}
		if (!Player2InGame)
		{
			CheckRespawnPlayer2();
		}
		if (!Player3InGame)
		{
			CheckRespawnPlayer3();
		}
		if (!Player4InGame)
		{
			CheckRespawnPlayer4();
		}
		UpdateClouds(gameTime);
		ExitRange = 20f;
		if (Player1InGame)
		{
			Vector2 vector = new Vector2(0f, 0f);
			if (Player1[Player1Index] != null)
			{
				_ = Player1[Player1Index]._bodyBodyPosition;
				vector = Player1[Player1Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector.X < ExitRange && vector.X > 0f - ExitRange && vector.Y < ExitRange && vector.Y > 0f - ExitRange)
			{
				Exit_Reached_First = true;
			}
		}
		if (Player2InGame)
		{
			Vector2 vector2 = new Vector2(0f, 0f);
			if (Player1[Player2Index] != null)
			{
				_ = Player1[Player2Index]._bodyBodyPosition;
				vector2 = Player1[Player2Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector2.X < ExitRange && vector2.X > 0f - ExitRange && vector2.Y < ExitRange && vector2.Y > 0f - ExitRange)
			{
				Exit_Reached_First = true;
			}
		}
		if (Player3InGame)
		{
			Vector2 vector3 = new Vector2(0f, 0f);
			if (Player1[Player3Index] != null)
			{
				_ = Player1[Player3Index]._bodyBodyPosition;
				vector3 = Player1[Player3Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector3.X < ExitRange && vector3.X > 0f - ExitRange && vector3.Y < ExitRange && vector3.Y > 0f - ExitRange)
			{
				Exit_Reached_First = true;
			}
		}
		if (Player4InGame)
		{
			Vector2 vector4 = new Vector2(0f, 0f);
			if (Player1[Player4Index] != null)
			{
				_ = Player1[Player4Index]._bodyBodyPosition;
				vector4 = Player1[Player4Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector4.X < ExitRange && vector4.X > 0f - ExitRange && vector4.Y < ExitRange && vector4.Y > 0f - ExitRange)
			{
				Exit_Reached_First = true;
			}
		}
	}

	public void Update_Night_N_Day()
	{
		BackgroundColor = Color.SkyBlue;
	}

	public void Update(GameTime gameTime)
	{
		gameTime_Physics = gameTime;
		if (!FirstTime)
		{
			Thread.Sleep(100);
			PhysicsSleepMilli = 2;
			gameTime_Physics_Old = gameTime_Physics.TotalGameTime;
			Scene_Update_Pacer = gameTime_Physics.TotalGameTime;
			Scene_Update_Pacer_2 = gameTime_Physics.TotalGameTime;
			FirstTime = true;
			Thread_Physics = new Thread((ThreadStart)delegate
			{
				Update_Physics_1();
			});
			Thread_Physics.Start();
			Thread_Music = new Thread((ThreadStart)delegate
			{
				Update_Music(gameTime);
			});
			Thread_Music.Start();
			Thread_Camera = new Thread((ThreadStart)delegate
			{
				ScrollCamera(mainGame.graphics.GraphicsDevice.Viewport);
			});
			Thread_Camera.Start();
		}
		if (!(Scene_Update_Pacer_2.TotalMilliseconds + 16.0 < gameTime_Physics.TotalGameTime.TotalMilliseconds))
		{
			return;
		}
		PlayerDistApart();
		Update_Night_N_Day();
		if (mainGame.Duel)
		{
			if (Player1Lives <= 0 && !Player1InGame && mainGame.Player1InGame)
			{
				Player1Dead = true;
			}
			if (Player2Lives <= 0 && !Player2InGame && mainGame.Player2InGame)
			{
				Player2Dead = true;
			}
			if (Player3Lives <= 0 && !Player3InGame && mainGame.Player3InGame)
			{
				Player3Dead = true;
			}
			if (Player4Lives <= 0 && !Player4InGame && mainGame.Player4InGame)
			{
				Player4Dead = true;
			}
			if (!Player1Dead && Player2Dead && Player3Dead && Player4Dead)
			{
				Exit_Reached_First = true;
			}
			else if (Player1Dead && !Player2Dead && Player3Dead && Player4Dead)
			{
				Exit_Reached_First = true;
			}
			else if (Player1Dead && Player2Dead && !Player3Dead && Player4Dead)
			{
				Exit_Reached_First = true;
			}
			else if (Player1Dead && Player2Dead && Player3Dead && !Player4Dead)
			{
				Exit_Reached_First = true;
			}
		}
		else if (PlayersLives < 0)
		{
			Exit_Reached_First = true;
		}
		if (Exit_Reached_First)
		{
			GamePadState state = GamePad.GetState(PlayerIndex.One);
			GamePadState state2 = GamePad.GetState(PlayerIndex.Two);
			GamePadState state3 = GamePad.GetState(PlayerIndex.Three);
			GamePadState state4 = GamePad.GetState(PlayerIndex.Four);
			if (mainGame.Duel)
			{
				if (mainGame.MainMenuLevelIndexer + 1 <= mainGame.AllLevelNames.Unlocked_Dueling.Length)
				{
					mainGame.AllLevelNames.Unlocked_Dueling[mainGame.MainMenuLevelIndexer + 1] = true;
					mainGame.Save_LevelName_Data();
				}
				if (state.Buttons.A == ButtonState.Pressed)
				{
					mainGame.InLevelMode = false;
					mainGame.InMainMenuMode = true;
					mainGame.MainMenuFadeIn = true;
					mainGame.MainMenuFadeOut = false;
					MediaPlayer.Stop();
					MediaPlayer.Play(Song0);
					mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
				}
				if (state2.Buttons.A == ButtonState.Pressed)
				{
					mainGame.InLevelMode = false;
					mainGame.InMainMenuMode = true;
					mainGame.MainMenuFadeIn = true;
					mainGame.MainMenuFadeOut = false;
					MediaPlayer.Stop();
					MediaPlayer.Play(Song0);
					mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
				}
				if (state3.Buttons.A == ButtonState.Pressed)
				{
					mainGame.InLevelMode = false;
					mainGame.InMainMenuMode = true;
					mainGame.MainMenuFadeIn = true;
					mainGame.MainMenuFadeOut = false;
					MediaPlayer.Stop();
					MediaPlayer.Play(Song0);
					mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
				}
				if (state4.Buttons.A == ButtonState.Pressed)
				{
					mainGame.InLevelMode = false;
					mainGame.InMainMenuMode = true;
					mainGame.MainMenuFadeIn = true;
					mainGame.MainMenuFadeOut = false;
					MediaPlayer.Stop();
					MediaPlayer.Play(Song0);
					mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
				}
			}
			else if (mainGame.Co_Op)
			{
				if (PlayersLives < 0)
				{
					if (state.Buttons.A == ButtonState.Pressed)
					{
						mainGame.InLevelMode = false;
						mainGame.InMainMenuMode = true;
						mainGame.MainMenuFadeIn = true;
						mainGame.MainMenuFadeOut = false;
						MediaPlayer.Stop();
						MediaPlayer.Play(Song0);
						mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
					}
					if (state2.Buttons.A == ButtonState.Pressed)
					{
						mainGame.InLevelMode = false;
						mainGame.InMainMenuMode = true;
						mainGame.MainMenuFadeIn = true;
						mainGame.MainMenuFadeOut = false;
						MediaPlayer.Stop();
						MediaPlayer.Play(Song0);
						mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
					}
					if (state3.Buttons.A == ButtonState.Pressed)
					{
						mainGame.InLevelMode = false;
						mainGame.InMainMenuMode = true;
						mainGame.MainMenuFadeIn = true;
						mainGame.MainMenuFadeOut = false;
						MediaPlayer.Stop();
						MediaPlayer.Play(Song0);
						mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
					}
					if (state4.Buttons.A == ButtonState.Pressed)
					{
						mainGame.InLevelMode = false;
						mainGame.InMainMenuMode = true;
						mainGame.MainMenuFadeIn = true;
						mainGame.MainMenuFadeOut = false;
						MediaPlayer.Stop();
						MediaPlayer.Play(Song0);
						mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
					}
				}
				else
				{
					if (state.Buttons.A == ButtonState.Pressed)
					{
						OnExitReached();
					}
					if (state2.Buttons.A == ButtonState.Pressed)
					{
						OnExitReached();
					}
					if (state3.Buttons.A == ButtonState.Pressed)
					{
						OnExitReached();
					}
					if (state4.Buttons.A == ButtonState.Pressed)
					{
						OnExitReached();
					}
				}
			}
		}
		else if (!Paused && Thread_Physics_Done && Player1.Count == BodyCount + 1)
		{
			for (int num = 0; num < Player1.Count; num++)
			{
				if (Player1[num] != null && !Exit_Reached_First)
				{
					Player1[num].Update(gameTime);
				}
			}
		}
		Update_Vibration(gameTime);
		float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
		particleEffectExit.Update(deltaSeconds);
		particleEffectStart.Update(deltaSeconds);
		particleEffectFog.Update(deltaSeconds);
		MineExplodeEffect.Update(deltaSeconds);
		if (!Paused)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.F12))
			{
				MediaPlayer.Stop();
				mainGame.InMainMenuMode = true;
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
			int val = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0);
			val = Math.Min(val, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
			timeRemaining -= TimeSpan.FromSeconds(val);
			score += val * 5;
			if (ExitPosition != new Vector2(0f, 0f))
			{
				particleEffectExit.Trigger(ExitPosition);
			}
			particleEffectStart.Trigger(new Vector2(0f, 0f));
			particleEffectFog.Trigger(new Vector2(mainGame.GraphicsDevice.Viewport.Width / 2, -50f));
			UpdateClouds(gameTime);
		}
		Scene_Update_Pacer_2 = gameTime_Physics.TotalGameTime;
	}

	public void Update_2(GameTime gameTime)
	{
		if (!Paused)
		{
			if (BodyCount == -1)
			{
				BodyCount = 0;
				Player1.Add(new Player1(this, mainGame, new Vector2(cameraPosition + 640f, cameraHeightPosition + 360f), _world, 1, mainGame.Player1Species, Player1Color));
				if (Player1[0] != null)
				{
					Player1[0].DestroyAll(_world);
					Player1[0] = null;
				}
			}
			for (int i = 0; i < Lands.Count; i++)
			{
				if (Lands[i] != null)
				{
					Lands[i].Update(gameTime, _world);
				}
			}
			for (int j = 0; j < Blocks.Count; j++)
			{
				if (Blocks[j] != null)
				{
					Blocks[j].Update(gameTime, _world);
				}
			}
			for (int k = 0; k < Kinetics.Count; k++)
			{
				if (Kinetics[k] != null)
				{
					Kinetics[k].Update(gameTime, _world);
				}
			}
			for (int l = 0; l < Sharps.Count; l++)
			{
				if (Sharps[l] != null)
				{
					Sharps[l].Update(gameTime, _world);
				}
			}
			for (int m = 0; m < Enemys.Count; m++)
			{
				if (Enemys[m] != null)
				{
					Enemys[m].Update(gameTime, _world);
				}
			}
		}
		for (int n = 0; n < Player1.Count; n++)
		{
			if (Player1Index == n || Player2Index == n || Player3Index == n || Player4Index == n)
			{
				continue;
			}
			if (BodiesOnMap)
			{
				if (Player1[n] != null)
				{
					_ = Player1[n].RemovedDarts;
					if (!Player1[n].RemovedDarts)
					{
						Player1[n].RemoveAllDarts(_world);
					}
					if (Player1[n].DeadTimer == Player1[n].DeadTimerMax)
					{
						Player1[n].DestroyAll(_world);
						Player1[n] = null;
					}
				}
			}
			else if (Player1[n] != null)
			{
				Player1[n].DestroyAll(_world);
				Player1[n] = null;
			}
		}
		if (Paused)
		{
			return;
		}
		if (mainGame.Duel)
		{
			if (Player1InGame)
			{
				if (Player1[Player1Index] != null && !Player1[Player1Index].Alive)
				{
					if (Player1Lives > 0)
					{
						CheckRespawnPlayer1();
					}
					else
					{
						Player1InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer1();
			}
			if (Player2InGame)
			{
				if (Player1[Player2Index] != null && !Player1[Player2Index].Alive)
				{
					if (Player2Lives > 0)
					{
						CheckRespawnPlayer2();
					}
					else
					{
						Player2InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer2();
			}
			if (Player3InGame)
			{
				if (Player1[Player3Index] != null && !Player1[Player3Index].Alive)
				{
					if (Player3Lives > 0)
					{
						CheckRespawnPlayer3();
					}
					else
					{
						Player3InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer3();
			}
			if (Player4InGame)
			{
				if (Player1[Player4Index] != null && !Player1[Player4Index].Alive)
				{
					if (Player4Lives > 0)
					{
						CheckRespawnPlayer4();
					}
					else
					{
						Player4InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer4();
			}
		}
		else if (mainGame.Co_Op)
		{
			if (Player1InGame || Player2InGame || Player3InGame || Player4InGame || PlayersLives > 0)
			{
				if (Player1InGame)
				{
					if (Player1[Player1Index] != null && !Player1[Player1Index].Alive)
					{
						if (PlayersLives > 0)
						{
							CheckRespawnPlayer1();
						}
						else
						{
							Player1InGame = false;
							PlayersInGameIndex--;
						}
					}
				}
				else
				{
					CheckRespawnPlayer1();
				}
				if (Player2InGame)
				{
					if (Player1[Player2Index] != null && !Player1[Player2Index].Alive)
					{
						if (PlayersLives > 0)
						{
							CheckRespawnPlayer2();
						}
						else
						{
							Player2InGame = false;
							PlayersInGameIndex--;
						}
					}
				}
				else
				{
					CheckRespawnPlayer2();
				}
				if (Player3InGame)
				{
					if (Player1[Player3Index] != null && !Player1[Player3Index].Alive)
					{
						if (PlayersLives > 0)
						{
							CheckRespawnPlayer3();
						}
						else
						{
							Player3InGame = false;
							PlayersInGameIndex--;
						}
					}
				}
				else
				{
					CheckRespawnPlayer3();
				}
				if (Player4InGame)
				{
					if (Player1[Player4Index] != null && !Player1[Player4Index].Alive)
					{
						if (PlayersLives > 0)
						{
							CheckRespawnPlayer4();
						}
						else
						{
							Player4InGame = false;
							PlayersInGameIndex--;
						}
					}
				}
				else
				{
					CheckRespawnPlayer4();
				}
			}
			else
			{
				PlayersLives = -1;
			}
		}
		if (Player1InGame || Player2InGame || Player3InGame || Player4InGame)
		{
			AnyAlive = true;
		}
		if (!Player1InGame)
		{
			CheckRespawnPlayer1();
		}
		if (!Player2InGame)
		{
			CheckRespawnPlayer2();
		}
		if (!Player3InGame)
		{
			CheckRespawnPlayer3();
		}
		if (!Player4InGame)
		{
			CheckRespawnPlayer4();
		}
		ExitRange = 200f;
		if (Player1InGame)
		{
			Vector2 vector = new Vector2(0f, 0f);
			if (Player1[Player1Index] != null)
			{
				_ = Player1[Player1Index]._bodyBodyPosition;
				vector = Player1[Player1Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector.X < ExitRange)
			{
				if (vector.X > 0f - ExitRange)
				{
					if (vector.Y < ExitRange)
					{
						if (vector.Y > 0f - ExitRange)
						{
							IsExitRange1 = true;
							if (Player1.Count > Player1Index && Player1[Player1Index] != null && Player1[Player1Index].Exiting)
							{
								Exit_Reached_First = true;
							}
						}
						else
						{
							IsExitRange1 = false;
						}
					}
					else
					{
						IsExitRange1 = false;
					}
				}
				else
				{
					IsExitRange1 = false;
				}
			}
			else
			{
				IsExitRange1 = false;
			}
		}
		if (Player2InGame)
		{
			Vector2 vector2 = new Vector2(0f, 0f);
			if (Player1[Player2Index] != null)
			{
				_ = Player1[Player2Index]._bodyBodyPosition;
				vector2 = Player1[Player2Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector2.X < ExitRange)
			{
				if (vector2.X > 0f - ExitRange)
				{
					if (vector2.Y < ExitRange)
					{
						if (vector2.Y > 0f - ExitRange)
						{
							IsExitRange2 = true;
							if (Player1.Count > Player2Index && Player1[Player2Index] != null && Player1[Player2Index].Exiting)
							{
								Exit_Reached_First = true;
							}
						}
						else
						{
							IsExitRange2 = false;
						}
					}
					else
					{
						IsExitRange2 = false;
					}
				}
				else
				{
					IsExitRange2 = false;
				}
			}
			else
			{
				IsExitRange2 = false;
			}
		}
		if (Player3InGame)
		{
			Vector2 vector3 = new Vector2(0f, 0f);
			if (Player1[Player3Index] != null)
			{
				_ = Player1[Player3Index]._bodyBodyPosition;
				vector3 = Player1[Player3Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector3.X < ExitRange)
			{
				if (vector3.X > 0f - ExitRange)
				{
					if (vector3.Y < ExitRange)
					{
						if (vector3.Y > 0f - ExitRange)
						{
							IsExitRange3 = true;
							if (Player1.Count > Player3Index && Player1[Player3Index] != null && Player1[Player3Index].Exiting)
							{
								Exit_Reached_First = true;
							}
						}
						else
						{
							IsExitRange3 = false;
						}
					}
					else
					{
						IsExitRange3 = false;
					}
				}
				else
				{
					IsExitRange3 = false;
				}
			}
			else
			{
				IsExitRange3 = false;
			}
		}
		if (Player4InGame)
		{
			Vector2 vector4 = new Vector2(0f, 0f);
			if (Player1[Player4Index] != null)
			{
				_ = Player1[Player4Index]._bodyBodyPosition;
				vector4 = Player1[Player4Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			}
			if (vector4.X < ExitRange)
			{
				if (vector4.X > 0f - ExitRange)
				{
					if (vector4.Y < ExitRange)
					{
						if (vector4.Y > 0f - ExitRange)
						{
							IsExitRange4 = true;
							if (Player1.Count > Player4Index && Player1[Player4Index] != null && Player1[Player4Index].Exiting)
							{
								Exit_Reached_First = true;
							}
						}
						else
						{
							IsExitRange4 = false;
						}
					}
					else
					{
						IsExitRange4 = false;
					}
				}
				else
				{
					IsExitRange4 = false;
				}
			}
			else
			{
				IsExitRange4 = false;
			}
		}
		if (!IsExitRange1 && !IsExitRange2 && !IsExitRange3 && !IsExitRange4)
		{
			IsExitRange = false;
		}
		else
		{
			IsExitRange = true;
		}
	}

	public void Update_OLD(GameTime gameTime)
	{
		ActiveZoneFixture.Body.Position = new Vector2(cameraPosition * PhysicsScaleDown, cameraHeightPosition * PhysicsScaleDown);
		foreach (Body body in _world.BodyList)
		{
			Vector2 point = body.Position;
			if (ActiveZoneFixture.TestPoint(ref point))
			{
				body.Awake = true;
				body.SleepingAllowed = false;
			}
			else
			{
				body.SleepingAllowed = true;
				body.Awake = false;
			}
		}
		if (mainGame.Duel)
		{
			if (Player1Lives < 0 && !Player1InGame && mainGame.Player1InGame)
			{
				Player1Dead = true;
			}
			if (Player2Lives < 0 && !Player2InGame && mainGame.Player2InGame)
			{
				Player2Dead = true;
			}
			if (Player3Lives < 0 && !Player3InGame && mainGame.Player3InGame)
			{
				Player3Dead = true;
			}
			if (Player4Lives < 0 && !Player4InGame && mainGame.Player4InGame)
			{
				Player4Dead = true;
			}
			if (!Player1Dead && Player2Dead && Player3Dead && Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 1;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
			else if (Player1Dead && !Player2Dead && Player3Dead && Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 2;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
			else if (Player1Dead && Player2Dead && !Player3Dead && Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 3;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
			else if (Player1Dead && Player2Dead && Player3Dead && !Player4Dead)
			{
				mainGame.InLevelMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.WinnerDuel = 4;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			}
		}
		else if (mainGame.Co_Op && PlayersLives < 0)
		{
			mainGame.InLevelMode = false;
			mainGame.InMainMenuMode = true;
			mainGame.MainMenuFadeIn = true;
			mainGame.MainMenuFadeOut = false;
			MediaPlayer.Stop();
			MediaPlayer.Play(Song0);
			mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		if (BodyCount == -1)
		{
			BodyCount = 0;
			Player1.Add(new Player1(this, mainGame, new Vector2(cameraPosition + 640f, cameraHeightPosition + 360f), _world, 1, mainGame.Player1Species, Player1Color));
			if (Player1[0] != null)
			{
				Player1[0].DestroyAll(_world);
				Player1[0] = null;
			}
		}
		for (int i = 0; i < Player1.Count; i++)
		{
			if (Player1Index == i || Player2Index == i || Player3Index == i || Player4Index == i)
			{
				continue;
			}
			if (BodiesOnMap)
			{
				if (Player1[i] != null && Player1[i].DeadTimer == Player1[i].DeadTimerMax)
				{
					Player1[i].DestroyAll(_world);
					Player1[i] = null;
				}
			}
			else if (Player1[i] != null)
			{
				Player1[i].DestroyAll(_world);
				Player1[i] = null;
			}
		}
		Update_Music(gameTime);
		float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
		particleEffectExit.Update(deltaSeconds);
		MineExplodeEffect.Update(deltaSeconds);
		if (Paused)
		{
			return;
		}
		if (Keyboard.GetState().IsKeyDown(Keys.F12))
		{
			MediaPlayer.Stop();
			mainGame.InMainMenuMode = true;
			mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		mainGame.Loaded = true;
		int val = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0);
		val = Math.Min(val, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
		timeRemaining -= TimeSpan.FromSeconds(val);
		score += val * 5;
		if (ExitPosition != new Vector2(0f, 0f))
		{
			particleEffectExit[0].TriggerOffset = ExitPosition;
			particleEffectExit.Trigger(new Vector2(0f, 0f));
		}
		int num = 0;
		while (num < Bricks.Count)
		{
			if (Bricks[num] != null)
			{
				Bricks[num].Update(gameTime, _world);
			}
		}
		for (int j = 0; j < Kinetics.Count; j++)
		{
			if (Kinetics[j] != null)
			{
				Kinetics[j].Update(gameTime, _world);
			}
		}
		for (int k = 0; k < Sharps.Count; k++)
		{
			if (Sharps[k] != null)
			{
				Sharps[k].Update(gameTime, _world);
			}
		}
		for (int l = 0; l < Enemys.Count; l++)
		{
			if (Enemys[l] != null)
			{
				Enemys[l].Update(gameTime, _world);
			}
		}
		if (mainGame.Duel)
		{
			if (Player1InGame)
			{
				if (!Player1[Player1Index].Alive)
				{
					if (Player1Lives > 0)
					{
						CheckRespawnPlayer1();
					}
					else
					{
						Player1InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer1();
			}
			if (Player2InGame)
			{
				if (!Player1[Player2Index].Alive)
				{
					if (Player2Lives > 0)
					{
						CheckRespawnPlayer2();
					}
					else
					{
						Player2InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer2();
			}
			if (Player3InGame)
			{
				if (!Player1[Player3Index].Alive)
				{
					if (Player3Lives > 0)
					{
						CheckRespawnPlayer3();
					}
					else
					{
						Player3InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer3();
			}
			if (Player4InGame)
			{
				if (!Player1[Player4Index].Alive)
				{
					if (Player4Lives > 0)
					{
						CheckRespawnPlayer4();
					}
					else
					{
						Player4InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer4();
			}
		}
		else if (mainGame.Co_Op)
		{
			if (Player1InGame)
			{
				if (!Player1[Player1Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer1();
					}
					else
					{
						Player1InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer1();
			}
			if (Player2InGame)
			{
				if (!Player1[Player2Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer2();
					}
					else
					{
						Player2InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer2();
			}
			if (Player3InGame)
			{
				if (!Player1[Player3Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer3();
					}
					else
					{
						Player3InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer3();
			}
			if (Player4InGame)
			{
				if (!Player1[Player4Index].Alive)
				{
					if (PlayersLives > 0)
					{
						CheckRespawnPlayer4();
					}
					else
					{
						Player4InGame = false;
						PlayersInGameIndex--;
					}
				}
			}
			else
			{
				CheckRespawnPlayer4();
			}
		}
		if (Player1InGame || Player2InGame || Player3InGame || Player4InGame)
		{
			AnyAlive = true;
		}
		if (!Player1InGame)
		{
			CheckRespawnPlayer1();
		}
		if (!Player2InGame)
		{
			CheckRespawnPlayer2();
		}
		if (!Player3InGame)
		{
			CheckRespawnPlayer3();
		}
		if (!Player4InGame)
		{
			CheckRespawnPlayer4();
		}
		foreach (Player1 item in Player1)
		{
			item?.Update(gameTime);
		}
		UpdateClouds(gameTime);
		ExitRange = 20f;
		if (Player1InGame)
		{
			Vector2 vector = new Vector2(0f, 0f);
			_ = Player1[Player1Index]._bodyBodyPosition;
			vector = Player1[Player1Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			if (vector.X < ExitRange && vector.X > 0f - ExitRange && vector.Y < ExitRange && vector.Y > 0f - ExitRange)
			{
				OnExitReached();
			}
		}
		if (Player2InGame)
		{
			Vector2 vector2 = new Vector2(0f, 0f);
			_ = Player1[Player2Index]._bodyBodyPosition;
			vector2 = Player1[Player2Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			if (vector2.X < ExitRange && vector2.X > 0f - ExitRange && vector2.Y < ExitRange && vector2.Y > 0f - ExitRange)
			{
				OnExitReached();
			}
		}
		if (Player3InGame)
		{
			Vector2 vector3 = new Vector2(0f, 0f);
			_ = Player1[Player3Index]._bodyBodyPosition;
			vector3 = Player1[Player3Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			if (vector3.X < ExitRange && vector3.X > 0f - ExitRange && vector3.Y < ExitRange && vector3.Y > 0f - ExitRange)
			{
				OnExitReached();
			}
		}
		if (Player4InGame)
		{
			Vector2 vector4 = new Vector2(0f, 0f);
			_ = Player1[Player4Index]._bodyBodyPosition;
			vector4 = Player1[Player4Index]._bodyBodyPosition * PhysicsScaleUp - ExitPosition;
			if (vector4.X < ExitRange && vector4.X > 0f - ExitRange && vector4.Y < ExitRange && vector4.Y > 0f - ExitRange)
			{
				OnExitReached();
			}
		}
	}

	private void Update_Music(GameTime gameTime)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
		int num = 0;
		while (num < 200)
		{
			Thread.Sleep(1000);
			if (MusicToggle)
			{
				if (MediaPlayer.State.Equals(MediaState.Stopped))
				{
					SongQueue = random.Next(9);
					if (SongQueue == 1)
					{
						MediaPlayer.Play(Song0);
					}
					else if (SongQueue == 2)
					{
						MediaPlayer.Play(Song1);
					}
					else if (SongQueue == 3)
					{
						MediaPlayer.Play(Song2);
					}
					else if (SongQueue == 4)
					{
						MediaPlayer.Play(Song3);
					}
					else if (SongQueue == 5)
					{
						MediaPlayer.Play(Song4);
					}
					else if (SongQueue == 6)
					{
						MediaPlayer.Play(Song5);
					}
					else if (SongQueue == 7)
					{
						MediaPlayer.Play(Song6);
					}
					else if (SongQueue == 8)
					{
						MediaPlayer.Play(Song7);
					}
					else if (SongQueue == 9)
					{
						MediaPlayer.Play(Song8);
					}
				}
				if (Paused)
				{
					MediaPlayer.Pause();
				}
				else
				{
					MediaPlayer.Resume();
				}
			}
			else
			{
				MediaPlayer.Stop();
			}
		}
	}

	private void UpdateClouds(GameTime gameTime)
	{
		int num = (int)Math.Round(gameTime.TotalGameTime.TotalMilliseconds);
		if (num - LastMilliSeconds > 1)
		{
			LastMilliSeconds = num;
		}
	}

	private void UpdateBlocks(GameTime gameTime)
	{
		for (int i = 0; i < Blocks.Count; i++)
		{
			if (Blocks[i] != null)
			{
				Blocks blocks = Blocks[i];
				blocks.Update(gameTime, _world);
			}
		}
	}

	private void Update_Vibration(GameTime gameTime)
	{
		float leftMotor = 0f;
		float leftMotor2 = 0f;
		float leftMotor3 = 0f;
		float leftMotor4 = 0f;
		float rightMotor = 0f;
		float rightMotor2 = 0f;
		float rightMotor3 = 0f;
		float rightMotor4 = 0f;
		if (P1Old_Vibration_Time_Left + (double)P1Vibration_Time_Left > gameTime.TotalGameTime.TotalMilliseconds)
		{
			leftMotor = P1Vibration_Speed_Left;
		}
		else
		{
			P1Old_Vibration_Time_Left = gameTime.TotalGameTime.TotalMilliseconds;
			P1Vibration_Time_Left = 0;
		}
		if (P1Old_Vibration_Time_Right + (double)P1Vibration_Time_Right > gameTime.TotalGameTime.TotalMilliseconds)
		{
			rightMotor = P1Vibration_Speed_Right;
		}
		else
		{
			P1Old_Vibration_Time_Right = gameTime.TotalGameTime.TotalMilliseconds;
			P1Vibration_Time_Right = 0;
		}
		if (P2Old_Vibration_Time_Left + (double)P2Vibration_Time_Left > gameTime.TotalGameTime.TotalMilliseconds)
		{
			leftMotor2 = P2Vibration_Speed_Left;
		}
		else
		{
			P2Old_Vibration_Time_Left = gameTime.TotalGameTime.TotalMilliseconds;
			P2Vibration_Time_Left = 0;
		}
		if (P2Old_Vibration_Time_Right + (double)P2Vibration_Time_Right > gameTime.TotalGameTime.TotalMilliseconds)
		{
			rightMotor2 = P2Vibration_Speed_Right;
		}
		else
		{
			P2Old_Vibration_Time_Right = gameTime.TotalGameTime.TotalMilliseconds;
			P2Vibration_Time_Right = 0;
		}
		if (P3Old_Vibration_Time_Left + (double)P3Vibration_Time_Left > gameTime.TotalGameTime.TotalMilliseconds)
		{
			leftMotor3 = P3Vibration_Speed_Left;
		}
		else
		{
			P3Old_Vibration_Time_Left = gameTime.TotalGameTime.TotalMilliseconds;
			P3Vibration_Time_Left = 0;
		}
		if (P3Old_Vibration_Time_Right + (double)P3Vibration_Time_Right > gameTime.TotalGameTime.TotalMilliseconds)
		{
			rightMotor3 = P3Vibration_Speed_Right;
		}
		else
		{
			P3Old_Vibration_Time_Right = gameTime.TotalGameTime.TotalMilliseconds;
			P3Vibration_Time_Right = 0;
		}
		if (P4Old_Vibration_Time_Left + (double)P4Vibration_Time_Left > gameTime.TotalGameTime.TotalMilliseconds)
		{
			leftMotor4 = P4Vibration_Speed_Left;
		}
		else
		{
			P4Old_Vibration_Time_Left = gameTime.TotalGameTime.TotalMilliseconds;
			P4Vibration_Time_Left = 0;
		}
		if (P4Old_Vibration_Time_Right + (double)P4Vibration_Time_Right > gameTime.TotalGameTime.TotalMilliseconds)
		{
			rightMotor4 = P4Vibration_Speed_Right;
		}
		else
		{
			P4Old_Vibration_Time_Right = gameTime.TotalGameTime.TotalMilliseconds;
			P4Vibration_Time_Right = 0;
		}
		GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
		GamePad.SetVibration(PlayerIndex.Two, leftMotor2, rightMotor2);
		GamePad.SetVibration(PlayerIndex.Three, leftMotor3, rightMotor3);
		GamePad.SetVibration(PlayerIndex.Four, leftMotor4, rightMotor4);
	}

	public void Vibration_Pulse_Left(PlayerIndex SentIndex, int Vibration_Duration_In_Milliseconds, float Vibration_Speed_From_0_to_1)
	{
		if (SentIndex == PlayerIndex.One)
		{
			P1Vibration_Time_Left = Vibration_Duration_In_Milliseconds;
			P1Vibration_Speed_Left = Vibration_Speed_From_0_to_1;
		}
		if (SentIndex == PlayerIndex.Two)
		{
			P2Vibration_Time_Left = Vibration_Duration_In_Milliseconds;
			P2Vibration_Speed_Left = Vibration_Speed_From_0_to_1;
		}
		if (SentIndex == PlayerIndex.Three)
		{
			P3Vibration_Time_Left = Vibration_Duration_In_Milliseconds;
			P3Vibration_Speed_Left = Vibration_Speed_From_0_to_1;
		}
		if (SentIndex == PlayerIndex.Four)
		{
			P4Vibration_Time_Left = Vibration_Duration_In_Milliseconds;
			P4Vibration_Speed_Left = Vibration_Speed_From_0_to_1;
		}
	}

	public void Vibration_Pulse_Right(PlayerIndex SentIndex, int Vibration_Duration_In_Milliseconds, float Vibration_Speed_From_0_to_1)
	{
		if (SentIndex == PlayerIndex.One)
		{
			P1Vibration_Time_Right = Vibration_Duration_In_Milliseconds;
			P1Vibration_Speed_Right = Vibration_Speed_From_0_to_1;
		}
		if (SentIndex == PlayerIndex.Two)
		{
			P2Vibration_Time_Right = Vibration_Duration_In_Milliseconds;
			P2Vibration_Speed_Right = Vibration_Speed_From_0_to_1;
		}
		if (SentIndex == PlayerIndex.Three)
		{
			P3Vibration_Time_Right = Vibration_Duration_In_Milliseconds;
			P3Vibration_Speed_Right = Vibration_Speed_From_0_to_1;
		}
		if (SentIndex == PlayerIndex.Four)
		{
			P4Vibration_Time_Right = Vibration_Duration_In_Milliseconds;
			P4Vibration_Speed_Right = Vibration_Speed_From_0_to_1;
		}
	}

	private void OnExitReached()
	{
		reachedExit = true;
	}

	public void StartNewLife()
	{
	}

	public void DrawTerrains(PlatformerGame Game, GameTime gameTime, SpriteBatch spriteBatch)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
		int num = 200;
		while (num > 0)
		{
			terrains[0].Draw(spriteBatch, this, cameraPosition, cameraHeightPosition, Color.Black, new Vector2(0f, 0f));
		}
	}

	public void Draw(PlatformerGame Game, GameTime gameTime, SpriteBatch spriteBatch)
	{
		Matrix transformMatrix = Matrix.CreateTranslation(0f, 0f - cameraHeightPosition - (0f - mainGame.Original_Window.Y - (0f - mainGame.Original_Window.Y) * MasterScale), 0f);
		cameraTransform = Matrix.CreateTranslation(0f - cameraPosition - (0f - mainGame.Original_Window.X - (0f - mainGame.Original_Window.X) * MasterScale), 0f - cameraHeightPosition - (0f - mainGame.Original_Window.Y - (0f - mainGame.Original_Window.Y) * MasterScale), 0f);
		Matrix matrix = Matrix.CreateScale(MasterScale);
		cameraTransform *= matrix;
		transformMatrix *= matrix;
		cameraTransformForParticles = cameraTransform;
		cameraTransformHud = Matrix.CreateTranslation(spriteBatch.GraphicsDevice.Viewport.Width - spriteBatch.GraphicsDevice.Viewport.Width / 8, spriteBatch.GraphicsDevice.Viewport.Height - spriteBatch.GraphicsDevice.Viewport.Height / 8, 0f);
		Matrix matrix2 = Matrix.CreateScale(mainGame.Global_Scaler);
		cameraTransformHud *= matrix2;
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		layers[0].Draw(spriteBatch, null, this, cameraPosition, cameraHeightPosition, BackgroundColor, new Vector2(0f, -200f));
		spriteBatch.End();
		renderer.RenderEffect(particleEffectFog);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, transformMatrix);
		layers[1].Draw(spriteBatch, null, this, cameraPosition, cameraHeightPosition, Color.White, new Vector2(0f, 286f));
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
		if (mainGame.IsHD)
		{
			DrawShadowedString_DATA(spriteBatch, Font, "Warning: You will Die if you go much farther!", new Vector2(-13000f * mainGame.Global_Scaler, mainGame.graphics.GraphicsDevice.Viewport.Height / 2 + -400), Color.Red);
			DrawShadowedString_DATA(spriteBatch, Font, "Warning: You will Die if you go much farther!", new Vector2(41000f * mainGame.Global_Scaler, mainGame.graphics.GraphicsDevice.Viewport.Height / 2 + -400), Color.Red);
		}
		else
		{
			DrawShadowedString_DATA(spriteBatch, Font, "Warning: You will Die if you go much farther!", new Vector2(-13000f * mainGame.Global_Scaler, mainGame.graphics.GraphicsDevice.Viewport.Height / 2 + -200), Color.Red);
			DrawShadowedString_DATA(spriteBatch, Font, "Warning: You will Die if you go much farther!", new Vector2(41000f * mainGame.Global_Scaler, mainGame.graphics.GraphicsDevice.Viewport.Height / 2 + -200), Color.Red);
		}
		if (!mainGame.LevelFromBuilder)
		{
			DrawTiles(spriteBatch);
		}
		if (Thread_Physics_Done)
		{
			this.l = 0;
			int num = 0;
			while (num < Lands.Count)
			{
				if (Lands[num] != null)
				{
					int i = this.l + 1;
					Lands[num].Draw(gameTime, spriteBatch, i);
					num++;
				}
			}
		}
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, cameraTransformForParticles);
		renderer.RenderEffect(particleEffectExit, spriteBatch);
		renderer.RenderEffect(particleEffectStart, spriteBatch);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
		if (Thread_Physics_Done)
		{
			K = 0;
			int num2 = 0;
			while (num2 < Kinetics.Count)
			{
				if (Kinetics[num2] != null)
				{
					int i2 = K + 1;
					Kinetics[num2].Draw(gameTime, spriteBatch, i2);
					num2++;
				}
			}
			spriteBatch.End();
			for (int j = 0; j < Player1.Count; j++)
			{
				if (Player1[j] != null && Player1[j].Alive)
				{
					Player1[j].DrawWiggledLines(gameTime, spriteBatch, Game, cameraTransform);
				}
			}
			for (int k = 0; k < Player1.Count; k++)
			{
				if (Player1[k] != null)
				{
					Player1[k].Draw(gameTime, spriteBatch, Game, cameraTransform);
				}
			}
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
			sh = 0;
			int num3 = 0;
			while (num3 < Sharps.Count)
			{
				if (Sharps[num3] != null)
				{
					int i3 = sh + 1;
					Sharps[num3].Draw(gameTime, spriteBatch, i3);
					num3++;
				}
			}
			b = 0;
			int num4 = 0;
			while (num4 < Blocks.Count)
			{
				if (Blocks[num4] != null)
				{
					int i4 = b + 1;
					Blocks[num4].Draw(gameTime, spriteBatch, i4);
					num4++;
				}
			}
			s = 0;
			int num5 = 0;
			while (num5 < Bricks.Count)
			{
				if (Bricks[num5] != null)
				{
					int i5 = s + 1;
					Bricks[num5].Draw(gameTime, spriteBatch, i5);
					num5++;
				}
			}
			em = 0;
			for (int l = 0; l < Enemys.Count; l++)
			{
				if (Enemys[l] != null)
				{
					Enemys[l].Draw(gameTime, spriteBatch, Game);
				}
			}
			spriteBatch.End();
			for (int m = 0; m < Player1.Count; m++)
			{
				if (Player1[m] != null && Player1[m].BleedingTimer < Player1[m].BleedingTimerMax)
				{
					Player1[m].DrawParticles(cameraTransformForParticles, spriteBatch);
				}
			}
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
			for (int n = 0; n < Player1.Count; n++)
			{
				if (Player1[n] != null)
				{
					Player1[n].DrawMagic(spriteBatch);
				}
			}
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
			for (int num6 = 0; num6 < Enemys.Count; num6++)
			{
				if (Enemys[num6] != null)
				{
					Enemys[num6].DrawMagic(spriteBatch);
				}
			}
			if (IsExitRange)
			{
				spriteBatch.Draw(ExitBrush, ExitPosition, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.3f, SpriteEffects.None, 1f);
			}
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, cameraTransformForParticles);
			renderer.RenderEffect(MineExplodeEffect, spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			if (Player1Index < Player1.Count && Player1InGame && Player1[Player1Index] != null)
			{
				try
				{
					if (Player1[Player1Index] != null)
					{
						if (Player1[Player1Index].Dead)
						{
							Player1Color = Color.Red;
							Player1Color.A = (byte)random.Next(255);
						}
						else if (Player1[Player1Index].Unconscious)
						{
							Player1Color = Color.GhostWhite;
							Player1Color.A = (byte)random.Next(255);
						}
						else
						{
							Player1Color = Color.White;
						}
					}
				}
				catch
				{
				}
			}
			if (Player2Index < Player1.Count && Player2InGame && Player1[Player2Index] != null)
			{
				try
				{
					if (Player1[Player2Index] != null)
					{
						if (Player1[Player2Index].Dead)
						{
							Player2Color = Color.Red;
							Player2Color.A = (byte)random.Next(255);
						}
						else if (Player1[Player2Index].Unconscious)
						{
							Player2Color = Color.GhostWhite;
							Player2Color.A = (byte)random.Next(255);
						}
						else
						{
							Player2Color = Color.White;
						}
					}
				}
				catch
				{
				}
			}
			if (Player3Index < Player1.Count && Player3InGame && Player1[Player3Index] != null)
			{
				try
				{
					if (Player1[Player3Index] != null)
					{
						if (Player1[Player3Index].Dead)
						{
							Player3Color = Color.Red;
							Player3Color.A = (byte)random.Next(255);
						}
						else if (Player1[Player3Index].Unconscious)
						{
							Player3Color = Color.GhostWhite;
							Player3Color.A = (byte)random.Next(255);
						}
						else
						{
							Player3Color = Color.White;
						}
					}
				}
				catch
				{
				}
			}
			if (Player4Index < Player1.Count && Player4InGame && Player1[Player4Index] != null)
			{
				try
				{
					if (Player1[Player4Index] != null)
					{
						if (Player1[Player4Index].Dead)
						{
							Player4Color = Color.Red;
							Player4Color.A = (byte)random.Next(255);
						}
						else if (Player1[Player4Index].Unconscious)
						{
							Player4Color = Color.GhostWhite;
							Player4Color.A = (byte)random.Next(255);
						}
						else
						{
							Player4Color = Color.White;
						}
					}
				}
				catch
				{
				}
			}
			Vector2 vector = new Vector2(140f * mainGame.Global_Scaler, 80f * mainGame.Global_Scaler);
			if (!mainGame.IsHD)
			{
				vector = new Vector2(200f * mainGame.Global_Scaler, 160f * mainGame.Global_Scaler);
			}
			Vector2 vector2 = vector + new Vector2(mainGame.Original_Window.X / 4f * 0f, 0f);
			Vector2 vector3 = vector + new Vector2(mainGame.Original_Window.X / 4f * 1f, 0f);
			Vector2 vector4 = vector + new Vector2(mainGame.Original_Window.X / 4f * 2f, 0f);
			Vector2 vector5 = vector + new Vector2(mainGame.Original_Window.X / 4f * 3f, 0f);
			if (mainGame.Duel)
			{
				if (Player1InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player1ProfileName + ": " + Player1Lives, vector2, Player1Color);
					if (Player1Index < Player1.Count && Player1[Player1Index] != null && Player1[Player1Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector6 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector2;
						Vector2 value = vector6 + new Vector2(Player1[Player1Index].PlayerHPBody, 0f);
						Player1HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation = (float)Math.Atan2(value.Y - vector6.Y, value.X - vector6.X);
						float num7 = Vector2.Distance(vector6, value);
						Texture2D texture2D = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D, vector6, null, Player1HPColor, rotation, Vector2.Zero, new Vector2(num7 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector7 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector2;
						Vector2 value2 = vector7 + new Vector2(Player1[Player1Index].PlayerMana, 0f);
						Player1ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player1Index].ManaMax / (float)Math.Max((int)Player1[Player1Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation2 = (float)Math.Atan2(value2.Y - vector7.Y, value2.X - vector7.X);
						float num8 = Vector2.Distance(vector7, value2);
						Texture2D texture2D2 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D2.SetData(new Color[1] { Player1ManaColor });
						spriteBatch.Draw(texture2D2, vector7, null, Player1ManaColor, rotation2, Vector2.Zero, new Vector2(num8 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector2;
					}
				}
				else if (mainGame.Player1InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector2, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
				if (Player2InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player2ProfileName + ": " + Player2Lives, vector3, Player2Color);
					if (Player2Index < Player1.Count && Player1[Player2Index] != null && Player1[Player2Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector8 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector3;
						Vector2 value3 = vector8 + new Vector2(Player1[Player2Index].PlayerHPBody, 0f);
						Player2HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation3 = (float)Math.Atan2(value3.Y - vector8.Y, value3.X - vector8.X);
						float num9 = Vector2.Distance(vector8, value3);
						Texture2D texture2D3 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D3.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D3, vector8, null, Player2HPColor, rotation3, Vector2.Zero, new Vector2(num9 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector9 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector3;
						Vector2 value4 = vector9 + new Vector2(Player1[Player2Index].PlayerMana, 0f);
						Player2ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player2Index].ManaMax / (float)Math.Max((int)Player1[Player2Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation4 = (float)Math.Atan2(value4.Y - vector9.Y, value4.X - vector9.X);
						float num10 = Vector2.Distance(vector9, value4);
						Texture2D texture2D4 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D4.SetData(new Color[1] { Player2ManaColor });
						spriteBatch.Draw(texture2D4, vector9, null, Player2ManaColor, rotation4, Vector2.Zero, new Vector2(num10 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector3;
					}
				}
				else if (mainGame.Player2InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector3, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
				if (Player3InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player3ProfileName + ": " + Player3Lives, vector4, Player3Color);
					if (Player3Index < Player1.Count && Player1[Player3Index] != null && Player1[Player3Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector10 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector4;
						Vector2 value5 = vector10 + new Vector2(Player1[Player3Index].PlayerHPBody, 0f);
						Player3HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation5 = (float)Math.Atan2(value5.Y - vector10.Y, value5.X - vector10.X);
						float num11 = Vector2.Distance(vector10, value5);
						Texture2D texture2D5 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D5.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D5, vector10, null, Player3HPColor, rotation5, Vector2.Zero, new Vector2(num11 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector11 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector4;
						Vector2 value6 = vector11 + new Vector2(Player1[Player3Index].PlayerMana, 0f);
						Player3ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player3Index].ManaMax / (float)Math.Max((int)Player1[Player3Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation6 = (float)Math.Atan2(value6.Y - vector11.Y, value6.X - vector11.X);
						float num12 = Vector2.Distance(vector11, value6);
						Texture2D texture2D6 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D6.SetData(new Color[1] { Player3ManaColor });
						spriteBatch.Draw(texture2D6, vector11, null, Player3ManaColor, rotation6, Vector2.Zero, new Vector2(num12 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector4;
					}
				}
				else if (mainGame.Player3InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector4, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
				if (Player4InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player4ProfileName + ": " + Player4Lives, vector5, Player4Color);
					if (Player4Index < Player1.Count && Player1[Player4Index] != null && Player1[Player4Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector12 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector5;
						Vector2 value7 = vector12 + new Vector2(Player1[Player4Index].PlayerHPBody, 0f);
						Player4HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation7 = (float)Math.Atan2(value7.Y - vector12.Y, value7.X - vector12.X);
						float num13 = Vector2.Distance(vector12, value7);
						Texture2D texture2D7 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D7.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D7, vector12, null, Player4HPColor, rotation7, Vector2.Zero, new Vector2(num13 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector13 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector5;
						Vector2 value8 = vector13 + new Vector2(Player1[Player4Index].PlayerMana, 0f);
						Player4ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player4Index].ManaMax / (float)Math.Max((int)Player1[Player4Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation8 = (float)Math.Atan2(value8.Y - vector13.Y, value8.X - vector13.X);
						float num14 = Vector2.Distance(vector13, value8);
						Texture2D texture2D8 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D8.SetData(new Color[1] { Player4ManaColor });
						spriteBatch.Draw(texture2D8, vector13, null, Player4ManaColor, rotation8, Vector2.Zero, new Vector2(num14 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector5;
					}
				}
				else if (mainGame.Player4InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector5, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
			}
			else if (mainGame.Co_Op)
			{
				if (Player1InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player1ProfileName + ": " + PlayersLives, vector2, Player1Color);
					if (Player1Index < Player1.Count && Player1[Player1Index] != null && Player1[Player1Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector14 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector2;
						Vector2 value9 = vector14 + new Vector2(Player1[Player1Index].PlayerHPBody, 0f);
						Player1HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation9 = (float)Math.Atan2(value9.Y - vector14.Y, value9.X - vector14.X);
						float num15 = Vector2.Distance(vector14, value9);
						Texture2D texture2D9 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D9.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D9, vector14, null, Player1HPColor, rotation9, Vector2.Zero, new Vector2(num15 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector15 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector2;
						Vector2 value10 = vector15 + new Vector2(Player1[Player1Index].PlayerMana, 0f);
						Player1ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player1Index].ManaMax / (float)Math.Max((int)Player1[Player1Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation10 = (float)Math.Atan2(value10.Y - vector15.Y, value10.X - vector15.X);
						float num16 = Vector2.Distance(vector15, value10);
						Texture2D texture2D10 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D10.SetData(new Color[1] { Player1ManaColor });
						spriteBatch.Draw(texture2D10, vector15, null, Player1ManaColor, rotation10, Vector2.Zero, new Vector2(num16 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector2;
					}
				}
				else if (mainGame.Player1InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector2, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
				if (Player2InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player2ProfileName + ": " + PlayersLives, vector3, Player2Color);
					if (Player2Index < Player1.Count && Player1[Player2Index] != null && Player1[Player2Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector16 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector3;
						Vector2 value11 = vector16 + new Vector2(Player1[Player2Index].PlayerHPBody, 0f);
						Player2HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation11 = (float)Math.Atan2(value11.Y - vector16.Y, value11.X - vector16.X);
						float num17 = Vector2.Distance(vector16, value11);
						Texture2D texture2D11 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D11.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D11, vector16, null, Player2HPColor, rotation11, Vector2.Zero, new Vector2(num17 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector17 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector3;
						Vector2 value12 = vector17 + new Vector2(Player1[Player2Index].PlayerMana, 0f);
						Player2ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player2Index].ManaMax / (float)Math.Max((int)Player1[Player2Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation12 = (float)Math.Atan2(value12.Y - vector17.Y, value12.X - vector17.X);
						float num18 = Vector2.Distance(vector17, value12);
						Texture2D texture2D12 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D12.SetData(new Color[1] { Player2ManaColor });
						spriteBatch.Draw(texture2D12, vector17, null, Player2ManaColor, rotation12, Vector2.Zero, new Vector2(num18 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector3;
					}
				}
				else if (mainGame.Player2InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector3, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
				if (Player3InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player3ProfileName + ": " + PlayersLives, vector4, Player3Color);
					if (Player3Index < Player1.Count && Player1[Player3Index] != null && Player1[Player3Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector18 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector4;
						Vector2 value13 = vector18 + new Vector2(Player1[Player3Index].PlayerHPBody, 0f);
						Player3HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation13 = (float)Math.Atan2(value13.Y - vector18.Y, value13.X - vector18.X);
						float num19 = Vector2.Distance(vector18, value13);
						Texture2D texture2D13 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D13.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D13, vector18, null, Player3HPColor, rotation13, Vector2.Zero, new Vector2(num19 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector19 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector4;
						Vector2 value14 = vector19 + new Vector2(Player1[Player3Index].PlayerMana, 0f);
						Player3ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player3Index].ManaMax / (float)Math.Max((int)Player1[Player3Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation14 = (float)Math.Atan2(value14.Y - vector19.Y, value14.X - vector19.X);
						float num20 = Vector2.Distance(vector19, value14);
						Texture2D texture2D14 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D14.SetData(new Color[1] { Player3ManaColor });
						spriteBatch.Draw(texture2D14, vector19, null, Player3ManaColor, rotation14, Vector2.Zero, new Vector2(num20 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector4;
					}
				}
				else if (mainGame.Player3InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector4, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
				if (Player4InGame)
				{
					DrawShadowedString(spriteBatch, Font, mainGame.Player4ProfileName + ": " + PlayersLives, vector5, Player4Color);
					if (Player4Index < Player1.Count && Player1[Player4Index] != null && Player1[Player4Index].Alive && Player1.Count == BodyCount + 1)
					{
						Vector2 vector20 = new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector5;
						Vector2 value15 = vector20 + new Vector2(Player1[Player4Index].PlayerHPBody, 0f);
						Player4HPColor = new Color(144f, 238f, 255f, 1f);
						float rotation15 = (float)Math.Atan2(value15.Y - vector20.Y, value15.X - vector20.X);
						float num21 = Vector2.Distance(vector20, value15);
						Texture2D texture2D15 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D15.SetData(new Color[1] { Color.White });
						spriteBatch.Draw(texture2D15, vector20, null, Player4HPColor, rotation15, Vector2.Zero, new Vector2(num21 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						Vector2 vector21 = new Vector2(40f * mainGame.Global_Scaler, 130f * mainGame.Global_Scaler) + vector5;
						Vector2 value16 = vector21 + new Vector2(Player1[Player4Index].PlayerMana, 0f);
						Player4ManaColor = new Color(255f, 0f, 0f, MathHelper.Clamp(0.4f + 1f / (Player1[Player4Index].ManaMax / (float)Math.Max((int)Player1[Player4Index].PlayerMana, 0.001)), 0.0001f, 1f));
						float rotation16 = (float)Math.Atan2(value16.Y - vector21.Y, value16.X - vector21.X);
						float num22 = Vector2.Distance(vector21, value16);
						Texture2D texture2D16 = new Texture2D(mainGame.graphics.GraphicsDevice, 1, 1);
						texture2D16.SetData(new Color[1] { Player4ManaColor });
						spriteBatch.Draw(texture2D16, vector21, null, Player4ManaColor, rotation16, Vector2.Zero, new Vector2(num22 * mainGame.Global_Scaler, 8f * mainGame.Global_Scaler), SpriteEffects.None, 0f);
						_ = new Vector2(90f * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + vector5;
					}
				}
				else if (mainGame.Player4InGame)
				{
					spriteBatch.Draw(SpawnBrush, new Vector2(40f * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + vector5, null, new Color(Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 100f, 255f), Pulsate(gameTime, 10f, 0.5f, 1f)), 0f, new Vector2(ExitBrush.Width / 2, ExitBrush.Height / 2), 0.15f, SpriteEffects.None, 1f);
				}
			}
		}
		spriteBatch.End();
		if (Paused && !Exit_Reached_First)
		{
			DrawPauseMenu(spriteBatch, gameTime);
		}
		if (Paused && !Exit_Reached_First)
		{
			if (PlayerPausedIndex == 1)
			{
				PauseMenuInput(PlayerIndex.One, gameTime);
			}
			else if (PlayerPausedIndex == 2)
			{
				PauseMenuInput(PlayerIndex.Two, gameTime);
			}
			else if (PlayerPausedIndex == 3)
			{
				PauseMenuInput(PlayerIndex.Three, gameTime);
			}
			else if (PlayerPausedIndex == 4)
			{
				PauseMenuInput(PlayerIndex.Four, gameTime);
			}
		}
		if (Exit_Reached_First)
		{
			DrawEndGameOverlayMenu(spriteBatch);
		}
	}

	public void DrawPauseMenu(SpriteBatch spriteBatch, GameTime gameTime)
	{
		cameraTransformPause = Matrix.CreateTranslation(mainGame.True_Screen_Center.X, mainGame.True_Screen_Center.Y, 0f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransformPause);
		int num = PauseMenuBackgroundStripTexture.Width * 20;
		float y = 0f;
		Vector2 vector = new Vector2(-1000f, -500f);
		for (int i = 0; i < 20; i++)
		{
			spriteBatch.Draw(PauseMenuBackgroundStripTexture, new Vector2(num * i, y) + vector, null, Color.White, 0f, new Vector2(PauseMenuBackgroundStripTexture.Width / 2, PauseMenuBackgroundStripTexture.Width / 2), 20f, SpriteEffects.None, 1f);
		}
		int num2 = 130;
		Texture2D texture2D = mainGame.Player1MenuTexture;
		float num3 = 0f;
		if (PlayerIndexer_Pub == PlayerIndex.One)
		{
			texture2D = mainGame.Player1MenuTexture;
			num3 = mainGame.Player1Species;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Two)
		{
			texture2D = mainGame.Player2MenuTexture;
			num3 = mainGame.Player2Species;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Three)
		{
			texture2D = mainGame.Player3MenuTexture;
			num3 = mainGame.Player3Species;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Four)
		{
			texture2D = mainGame.Player4MenuTexture;
			num3 = mainGame.Player4Species;
		}
		if (mainGame.IsHD)
		{
			spriteBatch.Draw(texture2D, new Vector2((float)(-num2) * mainGame.Global_Scaler * mainGame.Global_Scaler, -600f * mainGame.Global_Scaler * mainGame.Global_Scaler), null, Color.White, 0f, new Vector2(texture2D.Width / 2, texture2D.Height / 2), 3f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		}
		else
		{
			spriteBatch.Draw(texture2D, new Vector2((float)(-num2) * mainGame.Global_Scaler * mainGame.Global_Scaler, -1200f * mainGame.Global_Scaler * mainGame.Global_Scaler), null, Color.White, 0f, new Vector2(texture2D.Width / 2, texture2D.Height / 2), 3f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		}
		spriteBatch.Draw(PauseMenuControllerLayoutTexture, new Vector2(-200f * mainGame.Global_Scaler, 30f * mainGame.Global_Scaler), null, Color.White, 0f, new Vector2(PauseMenuTexture.Width / 2, PauseMenuTexture.Height / 2), 1f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		if (num3 == 0f)
		{
			PauseMenuPlayerStatsTexture = PauseMenuPlayerStatsTexture_Daru;
		}
		if (num3 == 4f)
		{
			PauseMenuPlayerStatsTexture = PauseMenuPlayerStatsTexture_Ernest;
		}
		if (num3 == 1f)
		{
			PauseMenuPlayerStatsTexture = PauseMenuPlayerStatsTexture_Oscar;
		}
		if (num3 == 2f)
		{
			PauseMenuPlayerStatsTexture = PauseMenuPlayerStatsTexture_Rick;
		}
		if (num3 == 3f)
		{
			PauseMenuPlayerStatsTexture = PauseMenuPlayerStatsTexture_Vinny;
		}
		spriteBatch.Draw(PauseMenuPlayerStatsTexture, new Vector2(700f * mainGame.Global_Scaler, 150f * mainGame.Global_Scaler), null, Color.White, 0f, new Vector2(PauseMenuPlayerStatsTexture.Width / 2, PauseMenuPlayerStatsTexture.Height / 2), 1.1f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		int maxValue = 1;
		float num4 = Pulsate(gameTime, 5f, 100f, 255f);
		if (num4 > 255f)
		{
			num4 = 255f;
		}
		else if (num4 < 0f)
		{
			num4 = 0f;
		}
		Color color = new Color((byte)num4, 0, 0, (byte)num4);
		Color red;
		if (MusicToggle)
		{
			red = Color.Red;
			red = new Color(mainGame.Music_Volume, (int)red.G, (int)red.B, mainGame.Music_Volume);
		}
		else
		{
			red = Color.Black;
		}
		Color red2;
		if (SoundEffectToggle)
		{
			red2 = Color.Red;
			red2 = new Color(mainGame.Sound_Effect_Volume, (int)red2.G, (int)red2.B, mainGame.Sound_Effect_Volume);
		}
		else
		{
			red2 = Color.Black;
		}
		Color color2 = ((!Blood) ? Color.Black : Color.Red);
		if (PauseMenuIndexer == 0)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Resume", new Vector2((float)(-num2) * mainGame.Global_Scaler, -100f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Resume", new Vector2((float)(-num2) * mainGame.Global_Scaler, -100f * mainGame.Global_Scaler), Color.Red);
		}
		if (MusicToggle)
		{
			if (PauseMenuIndexer == 1)
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((float)(-num2) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((float)(-num2) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler), red);
			}
		}
		else if (PauseMenuIndexer == 1)
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((float)(-num2) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
		}
		else
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((float)(-num2) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler), red);
		}
		if (SoundEffectToggle)
		{
			if (PauseMenuIndexer == 2)
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((float)(-num2) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((float)(-num2) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler), red2);
			}
		}
		else if (PauseMenuIndexer == 2)
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((float)(-num2) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
		}
		else
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((float)(-num2) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler), red2);
		}
		if (Blood)
		{
			if (PauseMenuIndexer == 3)
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((float)(-num2) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((float)(-num2) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler), color2);
			}
		}
		else if (PauseMenuIndexer == 3)
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((float)(-num2) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
		}
		else
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((float)(-num2) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler), color2);
		}
		if (PauseMenuIndexer == 4)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Main Menu", new Vector2((float)(-num2) * mainGame.Global_Scaler, 300f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Main Menu", new Vector2((float)(-num2) * mainGame.Global_Scaler, 300f * mainGame.Global_Scaler), Color.Red);
		}
		spriteBatch.End();
	}

	public void DrawPauseMenu_Old(SpriteBatch spriteBatch)
	{
		cameraTransformPause = Matrix.CreateTranslation(mainGame.True_Screen_Center.X, mainGame.True_Screen_Center.Y, 0f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransformPause);
		spriteBatch.Draw(PauseMenuTexture, new Vector2(0f, 30f), null, Color.White, 0f, new Vector2(PauseMenuTexture.Width / 2, PauseMenuTexture.Height / 2), 1f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		int maxValue = 2;
		Color red;
		if (MusicToggle)
		{
			red = Color.Red;
			red = new Color((int)red.R, (int)red.G, (int)red.B, mainGame.Music_Volume);
		}
		else
		{
			red = Color.Black;
		}
		Color red2;
		if (SoundEffectToggle)
		{
			red2 = Color.Red;
			red2 = new Color((int)red2.R, (int)red2.G, (int)red2.B, mainGame.Sound_Effect_Volume);
		}
		else
		{
			red2 = Color.Black;
		}
		Color color = ((!Blood) ? Color.Black : Color.Red);
		if (PauseMenuIndexer == 0)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Resume", new Vector2((0f - PauseFont.MeasureString("Resume").X / 2f) * mainGame.Global_Scaler, -100f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Resume", new Vector2((0f - PauseFont.MeasureString("Resume").X / 2f) * mainGame.Global_Scaler, -100f * mainGame.Global_Scaler), Color.Red);
		}
		if (MusicToggle)
		{
			if (PauseMenuIndexer == 1)
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((0f - PauseFont.MeasureString("Music").X / 2f) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((0f - PauseFont.MeasureString("Music").X / 2f) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler), red);
			}
		}
		else if (PauseMenuIndexer == 1)
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((0f - PauseFont.MeasureString("Music").X / 2f) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red);
		}
		else
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Music", new Vector2((0f - PauseFont.MeasureString("Music").X / 2f) * mainGame.Global_Scaler, 0f * mainGame.Global_Scaler), red);
		}
		if (SoundEffectToggle)
		{
			if (PauseMenuIndexer == 2)
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((0f - PauseFont.MeasureString("Sound_Effects").X / 2f) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red2);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((0f - PauseFont.MeasureString("Sound_Effects").X / 2f) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler), red2);
			}
		}
		else if (PauseMenuIndexer == 2)
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((0f - PauseFont.MeasureString("Sound_Effects").X / 2f) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red2);
		}
		else
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Sound Effects", new Vector2((0f - PauseFont.MeasureString("Sound_Effects").X / 2f) * mainGame.Global_Scaler, 100f * mainGame.Global_Scaler), red2);
		}
		if (Blood)
		{
			if (PauseMenuIndexer == 3)
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((0f - PauseFont.MeasureString("Blood").X / 2f) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((0f - PauseFont.MeasureString("Blood").X / 2f) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler), color);
			}
		}
		else if (PauseMenuIndexer == 3)
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((0f - PauseFont.MeasureString("Blood").X / 2f) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), color);
		}
		else
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Blood", new Vector2((0f - PauseFont.MeasureString("Blood").X / 2f) * mainGame.Global_Scaler, 200f * mainGame.Global_Scaler), color);
		}
		if (PauseMenuIndexer == 4)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Main Menu", new Vector2((0f - PauseFont.MeasureString("Main_Menu").X / 2f) * mainGame.Global_Scaler, 300f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Main Menu", new Vector2((0f - PauseFont.MeasureString("Main_Menu").X / 2f) * mainGame.Global_Scaler, 300f * mainGame.Global_Scaler), Color.Red);
		}
		spriteBatch.End();
	}

	public void DrawEndGameOverlayMenu(SpriteBatch spriteBatch)
	{
		cameraTransformPause = Matrix.CreateTranslation(mainGame.True_Screen_Center.X, mainGame.True_Screen_Center.Y, 0f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransformPause);
		int num = PauseMenuBackgroundStripTexture.Width * 20;
		float y = 0f;
		Vector2 vector = new Vector2(-1000f, -500f);
		for (int i = 0; i < 20; i++)
		{
			spriteBatch.Draw(PauseMenuBackgroundStripTexture, new Vector2(num * i, y) + vector, null, Color.White, 0f, new Vector2(PauseMenuBackgroundStripTexture.Width / 2, PauseMenuBackgroundStripTexture.Width / 2), 20f, SpriteEffects.None, 1f);
		}
		int maxValue = 2;
		if (mainGame.Duel)
		{
			if (Player1InGame && !Player1Dead)
			{
				DrawShadowedString_Pause(spriteBatch, HudFont, "Player 1 Wins!!!", new Vector2(-600f * mainGame.Global_Scaler, -300f * mainGame.Global_Scaler), Color.Red);
			}
			if (Player2InGame && !Player2Dead)
			{
				DrawShadowedString_Pause(spriteBatch, HudFont, "Player 2 Wins!!!", new Vector2(-600f * mainGame.Global_Scaler, -300f * mainGame.Global_Scaler), Color.Red);
			}
			if (Player3InGame && !Player3Dead)
			{
				DrawShadowedString_Pause(spriteBatch, HudFont, "Player 3 Wins!!!", new Vector2(-600f * mainGame.Global_Scaler, -300f * mainGame.Global_Scaler), Color.Red);
			}
			if (Player4InGame && !Player4Dead)
			{
				DrawShadowedString_Pause(spriteBatch, HudFont, "Player 4 Wins!!!", new Vector2(-600f * mainGame.Global_Scaler, -300f * mainGame.Global_Scaler), Color.Red);
			}
			if (!Player1InGame && !Player2InGame && !Player3InGame && !Player4InGame)
			{
				DrawShadowedString_Pause(spriteBatch, HudFont, "This is a Dueling level.", new Vector2(-800f * mainGame.Global_Scaler, -500f * mainGame.Global_Scaler), Color.Red);
				DrawShadowedString_Pause(spriteBatch, HudFont, "You're supposed to fight friends in here.", new Vector2(-800f * mainGame.Global_Scaler, -400f * mainGame.Global_Scaler), Color.Red);
				DrawShadowedString_Pause(spriteBatch, HudFont, "Challenge a friend to a Duel,", new Vector2(-800f * mainGame.Global_Scaler, -300f * mainGame.Global_Scaler), Color.Red);
				DrawShadowedString_Pause(spriteBatch, HudFont, "then you can play this level, Duh!", new Vector2(-800f * mainGame.Global_Scaler, -200f * mainGame.Global_Scaler), Color.Red);
				DrawShadowedString_Pause(spriteBatch, HudFont, "Your silly.....", new Vector2(-800f * mainGame.Global_Scaler, -100f * mainGame.Global_Scaler), Color.Red);
			}
			DrawShadowedString_Pause(spriteBatch, Hud2Font, "Press the A button to go to Main Menu.", new Vector2(-500f * mainGame.Global_Scaler, 500f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
			if (mainGame.MainMenuLevelIndexer + 1 <= mainGame.AllLevelNames.Unlocked_Dueling.Length)
			{
				mainGame.AllLevelNames.Unlocked_Dueling[mainGame.MainMenuLevelIndexer + 1] = true;
				mainGame.Save_LevelName_Data();
			}
		}
		else if (PlayersLives < 0)
		{
			DrawShadowedString_Pause(spriteBatch, HudFont, "Sorry you died!!! Try again.", new Vector2(-600f * mainGame.Global_Scaler, -300f * mainGame.Global_Scaler), Color.Red);
			DrawShadowedString_Pause(spriteBatch, Hud2Font, "Press the A button to go to Main Menu.", new Vector2(-500f * mainGame.Global_Scaler, 500f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, HudFont, "You Made It!!! Well done.", new Vector2(-600f * mainGame.Global_Scaler, -300f * mainGame.Global_Scaler), Color.Red);
			DrawShadowedString_Pause(spriteBatch, Hud2Font, "Press the A button to go to next level.", new Vector2(-500f * mainGame.Global_Scaler, 500f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
		}
		spriteBatch.End();
	}

	public void DrawMasterScene(PlatformerGame Game, GameTime gameTime, SpriteBatch spriteBatch)
	{
	}

	private void PauseMenuInput(PlayerIndex Player, GameTime gameTime)
	{
		if (Exit_Reached_First)
		{
			return;
		}
		PlayerIndexer_Pub = Player;
		GamePadState state = GamePad.GetState(Player);
		if (state.Buttons.A == ButtonState.Pressed && !PauseMenuButtonAWasPressed)
		{
			mainGame.MenuClickSound.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			PauseMenuButtonAWasPressed = true;
			if (PauseMenuIndexer == 0)
			{
				Paused = false;
				mainGame.InPauseMode = false;
			}
			if (PauseMenuIndexer == 1)
			{
				if (MusicToggle)
				{
					MusicToggle = false;
				}
				else if (!MusicToggle)
				{
					MusicToggle = true;
				}
			}
			_ = PauseMenuIndexer;
			_ = 2;
			if (PauseMenuIndexer == 3)
			{
				if (Blood)
				{
					BloodMode(BloodMode: false);
				}
				else if (!Blood)
				{
					BloodMode(BloodMode: true);
				}
			}
			if (PauseMenuIndexer == 4)
			{
				mainGame.InLevelMode = false;
				mainGame.InPauseMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.StartGame = false;
				if (mainGame.Player1InGame)
				{
					mainGame.P1MainMenuProgression--;
				}
				if (mainGame.Player2InGame)
				{
					mainGame.P2MainMenuProgression--;
				}
				if (mainGame.Player3InGame)
				{
					mainGame.P3MainMenuProgression--;
				}
				if (mainGame.Player4InGame)
				{
					mainGame.P4MainMenuProgression--;
				}
				mainGame.MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song1);
			}
		}
		PauseMenuButtonAWasPressed = state.Buttons.A == ButtonState.Pressed;
		if (state.Buttons.B == ButtonState.Pressed)
		{
			mainGame.MenuClickSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			Pause(gamePaused: false, 0);
			mainGame.InPauseMode = false;
		}
		if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
		{
			mainGame.MenuMoveSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			P1DpadUppressed = true;
			PauseMenuIndexer--;
		}
		P1DpadUpWaspressed = P1DpadUppressed;
		if (state.DPad.Up == ButtonState.Released)
		{
			P1DpadUppressed = false;
		}
		if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
		{
			mainGame.MenuMoveSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			P1DpadDownpressed = true;
			PauseMenuIndexer++;
		}
		P1DpadDownWaspressed = P1DpadDownpressed;
		if (state.DPad.Down == ButtonState.Released)
		{
			P1DpadDownpressed = false;
		}
		if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
		{
			P1DpadRightpressed = true;
			_ = PauseMenuIndexer;
			if (PauseMenuIndexer == 1)
			{
				mainGame.Music_Volume += mainGame.Volume_Step;
				if (mainGame.Music_Volume > 1f)
				{
					mainGame.Music_Volume = 1f;
				}
				MediaPlayer.Volume = mainGame.Music_Volume;
				mainGame.MenuMoveSound.Play(mainGame.Music_Volume, 0.5f, 0f);
			}
			if (PauseMenuIndexer == 2)
			{
				mainGame.Sound_Effect_Volume += mainGame.Volume_Step;
				if (mainGame.Sound_Effect_Volume > 1f)
				{
					mainGame.Sound_Effect_Volume = 1f;
				}
				mainGame.MenuMoveSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			}
			_ = PauseMenuIndexer;
			_ = 3;
			_ = PauseMenuIndexer;
			_ = 3;
		}
		P1DpadRightWaspressed = P1DpadRightpressed;
		if (state.DPad.Right == ButtonState.Released)
		{
			P1DpadRightpressed = false;
		}
		if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
		{
			P1DpadLeftpressed = true;
			_ = PauseMenuIndexer;
			if (PauseMenuIndexer == 1)
			{
				mainGame.Music_Volume -= mainGame.Volume_Step;
				if (mainGame.Music_Volume < 0f)
				{
					mainGame.Music_Volume = 0f;
				}
				MediaPlayer.Volume = mainGame.Music_Volume;
				mainGame.MenuMoveSound.Play(mainGame.Music_Volume, 0.5f, 0f);
			}
			if (PauseMenuIndexer == 2)
			{
				mainGame.Sound_Effect_Volume -= mainGame.Volume_Step;
				if (mainGame.Sound_Effect_Volume < 0f)
				{
					mainGame.Sound_Effect_Volume = 0f;
				}
				mainGame.MenuMoveSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			}
			_ = PauseMenuIndexer;
			_ = 3;
			_ = PauseMenuIndexer;
			_ = 3;
		}
		P1DpadLeftWaspressed = P1DpadLeftpressed;
		if (state.DPad.Left == ButtonState.Released)
		{
			P1DpadLeftpressed = false;
		}
		if (PauseMenuIndexer > PauseMenuIndexerMax)
		{
			PauseMenuIndexer = 0;
		}
		if (PauseMenuIndexer < 0)
		{
			PauseMenuIndexer = PauseMenuIndexerMax;
		}
	}

	private void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawGlowInvertedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 0f), Color.White, 0f, new Vector2(0f, 0f), mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(-1f, 0f), Color.White, 0f, new Vector2(0f, 0f), mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, 1f), Color.White, 0f, new Vector2(0f, 0f), mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, -1f), Color.White, 0f, new Vector2(0f, 0f), mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position, Color.Black, 0f, new Vector2(0f, 0f), mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawShadowedString_Pause(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawShadowedString_DATA(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), 2f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), 2f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawGlowInvertedString_Pause(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 0f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(-1f, 0f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, 1f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, -1f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position, Color.Black, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void Update_and_Draw_Shadows(SpriteBatch spriteBatch)
	{
		Body body;
		foreach (Body body2 in _world.BodyList)
		{
			body = body2;
			_world.RayCast(delegate(Fixture f, Vector2 p, Vector2 n, float fr)
			{
				_ = f.Body;
				Vector2 vector = p;
				if (f.UserData != null)
				{
					int num = (int)f.UserData;
					if (num == 1)
					{
						int num2 = (int)body.UserData;
						if (num2 == 9)
						{
							float num3 = vector.Y - body.Position.Y;
							ShadowScale = 4f / (num3 * 1f);
							spriteBatch.Draw(ShadowTexture, new Vector2(0f, -5f) + p * new Vector2(PhysicsScaleUp, PhysicsScaleUp), null, Color.White, 0f, ShadowTextureOrigin, ShadowScale, SpriteEffects.None, 1f);
						}
						if (num2 == 8)
						{
							float num4 = vector.Y - body.Position.Y;
							ShadowScale = 1f / (num4 * 1f);
							spriteBatch.Draw(ShadowTexture, new Vector2(0f, -2f) + p * new Vector2(PhysicsScaleUp, PhysicsScaleUp), null, Color.White, 0f, ShadowTextureOrigin, ShadowScale, SpriteEffects.None, 1f);
						}
					}
				}
				return 1f;
			}, body.Position, body.Position + new Vector2(0f, 1000f));
		}
	}

	private void DrawTiles(SpriteBatch spriteBatch)
	{
		int num = (int)Math.Floor(cameraPosition / 64f);
		int val = num + spriteBatch.GraphicsDevice.Viewport.Width / 64;
		val = Math.Min(val, Width - 1);
		int num2 = (int)Math.Floor(cameraPosition / 48f);
		int val2 = num2 + spriteBatch.GraphicsDevice.Viewport.Height / 48;
		val2 = Math.Min(val2, Height - 1);
		for (int i = 0; i < Height; i++)
		{
			for (int j = num; j <= val; j++)
			{
				Texture2D texture = tiles[j, i].Texture;
				if (texture != null)
				{
					Vector2 position = new Vector2(j, i) * Tile.Size;
					spriteBatch.Draw(texture, position, Color.White);
				}
			}
		}
	}

	public void ScrollCamera(Viewport viewport)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		int num = 0;
		while (num < 200)
		{
			if (mainGame.SplitScreen)
			{
				if (AnyAlive)
				{
					if (mainGame.SplitScreenindex == 1 || mainGame.SplitScreenindex == 3 || mainGame.SplitScreenindex == 5)
					{
						if (Player1InGame)
						{
							_ = Player1[Player1Index].PlayerPosition;
							CamVector = Player1[Player1Index].PlayerPosition * PhysicsScaleUp;
							CameraPositionNewX = CamVector.X;
							CameraPositionNewY = CamVector.Y;
						}
					}
					else if (mainGame.SplitScreenindex == 2 || mainGame.SplitScreenindex == 4 || mainGame.SplitScreenindex == 6)
					{
						if (Player2InGame && Player1[Player2Index]._headBody.Body != null)
						{
							CamVector = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
							CameraPositionNewX = CamVector.X;
							CameraPositionNewY = CamVector.Y;
						}
					}
					else if (mainGame.SplitScreenindex == 7)
					{
						if (Player3InGame && Player1[Player3Index]._headBody.Body != null)
						{
							CamVector = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
							CameraPositionNewX = CamVector.X;
							CameraPositionNewY = CamVector.Y;
						}
					}
					else if (mainGame.SplitScreenindex == 8 && Player4InGame && Player1[Player4Index]._headBody.Body != null)
					{
						CamVector = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
						CameraPositionNewX = CamVector.X;
						CameraPositionNewY = CamVector.Y;
					}
				}
			}
			else if (AnyAlive && AnyAlive)
			{
				if (mainGame.IsHD)
				{
					float num2 = 0.0003f;
					float num3 = 0.00025f;
					float num4 = 0.01f;
					float num5 = 0.1f;
					float num6 = 0.6f - PlayerDistApart_Float * num2;
					num6 += num5;
					if (num6 < MasterScale - num4)
					{
						MasterScale -= num3;
					}
					else if (num6 > MasterScale + num4)
					{
						MasterScale += num3;
					}
					MasterScale = MathHelper.Clamp(MasterScale, 0.9f, 2f);
					float num7 = 5E-06f;
					float num8 = 5E-06f;
					float num9 = (PlayerPosition_Vec.X - CamVector.X) * num7;
					float num10 = (PlayerPosition_Vec.Y - CamVector.Y) * num8;
					if (PlayerPosition_Vec.X > CamVector.X)
					{
						CamVector.X += num9;
					}
					else if (PlayerPosition_Vec.X < CamVector.X)
					{
						CamVector.X += num9;
					}
					if (PlayerPosition_Vec.Y > CamVector.Y)
					{
						CamVector.Y += num10;
					}
					else if (PlayerPosition_Vec.Y < CamVector.Y)
					{
						CamVector.Y += num10;
					}
					CameraPositionNewX = CamVector.X;
					CameraPositionNewY = CamVector.Y;
				}
				else
				{
					float num11 = 0.0003f;
					float num12 = 0.00075f;
					float num13 = 0.01f;
					float num14 = 0.1f;
					float num15 = 0.6f - PlayerDistApart_Float * num11;
					num15 += num14;
					if (num15 < MasterScale - num13)
					{
						MasterScale -= num12;
					}
					else if (num15 > MasterScale + num13)
					{
						MasterScale += num12;
					}
					MasterScale = MathHelper.Clamp(MasterScale, 0.3f, 0.48f);
					float num16 = 5E-06f;
					float num17 = 5E-06f;
					float num18 = (PlayerPosition_Vec.X - CamVector.X) * num16;
					float num19 = (PlayerPosition_Vec.Y - CamVector.Y) * num17;
					if (PlayerPosition_Vec.X > CamVector.X)
					{
						CamVector.X += num18;
					}
					else if (PlayerPosition_Vec.X < CamVector.X)
					{
						CamVector.X += num18;
					}
					if (PlayerPosition_Vec.Y > CamVector.Y)
					{
						CamVector.Y += num19;
					}
					else if (PlayerPosition_Vec.Y < CamVector.Y)
					{
						CamVector.Y += num19;
					}
					CameraPositionNewX = CamVector.X;
					CameraPositionNewY = CamVector.Y;
				}
			}
			float num20 = (float)viewport.Width * 0.49f;
			float num21 = cameraPosition + num20;
			float num22 = cameraPosition + (float)viewport.Width - num20;
			float num23 = 0f;
			if (CameraPositionNewX < num21)
			{
				num23 = CameraPositionNewX - num21;
			}
			else if (CameraPositionNewX > num22)
			{
				num23 = CameraPositionNewX - num22;
			}
			if (!mainGame.LevelFromBuilder)
			{
				maxCameraPosition = 64 * (Width * 100) - viewport.Width;
			}
			else
			{
				maxCameraPosition = 100000f;
			}
			cameraPosition = MathHelper.Clamp(cameraPosition + num23, 0f - maxCameraPosition, maxCameraPosition);
			float num24 = (float)viewport.Height * 0.49f;
			float num25 = cameraHeightPosition + num24;
			float num26 = cameraHeightPosition + (float)viewport.Height - num24;
			float num27 = 0f;
			if (CameraPositionNewY < num25)
			{
				num27 = CameraPositionNewY - num25;
			}
			else if (CameraPositionNewY > num26)
			{
				num27 = CameraPositionNewY - num26;
			}
			if (!mainGame.LevelFromBuilder)
			{
				maxHeightCameraPosition = 48 * Height - viewport.Height;
			}
			else
			{
				maxHeightCameraPosition = 100000f;
			}
			cameraHeightPosition += num27;
			cameraHeightPosition = MathHelper.Clamp(cameraHeightPosition + num27, 0f - maxHeightCameraPosition, 0f);
		}
	}

	public void CheckRespawnPlayer1()
	{
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		if (Player1InGame && Player1AliveOnce && !Player1Dead)
		{
			if (!Player1[Player1Index].ReSpawn || !mainGame.Player1InGame)
			{
				return;
			}
			if (mainGame.Duel)
			{
				if (Player1Lives <= -1)
				{
					return;
				}
				RespawnPlayer1();
				if (!Player1InGame && !Player1AliveOnce)
				{
					Player1AliveOnce = true;
					Player1InGame = true;
					if (mainGame.Duel)
					{
						Player1Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
			else
			{
				if (PlayersLives <= -1)
				{
					return;
				}
				RespawnPlayer1();
				if (!Player1InGame && !Player1AliveOnce)
				{
					Player1AliveOnce = true;
					Player1InGame = true;
					if (mainGame.Duel)
					{
						Player1Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
		}
		else if (state.IsButtonDown(Buttons.Y) && mainGame.Player1InGame && !Player1InGame && !Player1AliveOnce)
		{
			RespawnPlayer1();
			Player1AliveOnce = true;
			Player1InGame = true;
			if (mainGame.Duel)
			{
				Player1Lives = Duel_Lives;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives += Co_Op_Lives;
			}
			PlayersInGameIndex++;
		}
	}

	public void CheckRespawnPlayer2()
	{
		GamePadState state = GamePad.GetState(PlayerIndex.Two);
		if (Player2InGame && Player2AliveOnce && !Player2Dead)
		{
			if (!Player1[Player2Index].ReSpawn || !mainGame.Player2InGame)
			{
				return;
			}
			if (mainGame.Duel)
			{
				if (Player2Lives <= -1)
				{
					return;
				}
				RespawnPlayer2();
				if (!Player2InGame && !Player2AliveOnce)
				{
					Player2AliveOnce = true;
					Player2InGame = true;
					if (mainGame.Duel)
					{
						Player2Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
			else
			{
				if (PlayersLives <= -1)
				{
					return;
				}
				RespawnPlayer2();
				if (!Player2InGame && !Player2AliveOnce)
				{
					Player2AliveOnce = true;
					Player2InGame = true;
					if (mainGame.Duel)
					{
						Player2Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
		}
		else if (state.IsButtonDown(Buttons.Y) && mainGame.Player2InGame && !Player2InGame && !Player2AliveOnce)
		{
			RespawnPlayer2();
			Player2AliveOnce = true;
			Player2InGame = true;
			if (mainGame.Duel)
			{
				Player2Lives = Duel_Lives;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives += Co_Op_Lives;
			}
			PlayersInGameIndex++;
		}
	}

	public void CheckRespawnPlayer3()
	{
		GamePadState state = GamePad.GetState(PlayerIndex.Three);
		if (Player3InGame && Player3AliveOnce && !Player3Dead)
		{
			if (!Player1[Player3Index].ReSpawn || !mainGame.Player3InGame)
			{
				return;
			}
			if (mainGame.Duel)
			{
				if (Player3Lives <= -1)
				{
					return;
				}
				RespawnPlayer3();
				if (!Player3InGame && !Player3AliveOnce)
				{
					Player3AliveOnce = true;
					Player3InGame = true;
					if (mainGame.Duel)
					{
						Player3Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
			else
			{
				if (PlayersLives <= -1)
				{
					return;
				}
				RespawnPlayer3();
				if (!Player3InGame && !Player3AliveOnce)
				{
					Player3AliveOnce = true;
					Player3InGame = true;
					if (mainGame.Duel)
					{
						Player3Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
		}
		else if (state.IsButtonDown(Buttons.Y) && mainGame.Player3InGame && !Player3InGame && !Player3AliveOnce)
		{
			RespawnPlayer3();
			Player3AliveOnce = true;
			Player3InGame = true;
			if (mainGame.Duel)
			{
				Player3Lives = Duel_Lives;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives += Co_Op_Lives;
			}
			PlayersInGameIndex++;
		}
	}

	public void CheckRespawnPlayer4()
	{
		GamePadState state = GamePad.GetState(PlayerIndex.Four);
		if (Player4InGame && Player4AliveOnce && !Player4Dead)
		{
			if (!Player1[Player4Index].ReSpawn || !mainGame.Player4InGame)
			{
				return;
			}
			if (mainGame.Duel)
			{
				if (Player4Lives <= -1)
				{
					return;
				}
				RespawnPlayer4();
				if (!Player4InGame && !Player4AliveOnce)
				{
					Player4AliveOnce = true;
					Player4InGame = true;
					if (mainGame.Duel)
					{
						Player4Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
			else
			{
				if (PlayersLives <= -1)
				{
					return;
				}
				RespawnPlayer4();
				if (!Player4InGame && !Player4AliveOnce)
				{
					Player4AliveOnce = true;
					Player4InGame = true;
					if (mainGame.Duel)
					{
						Player4Lives = Duel_Lives;
					}
					else if (mainGame.Co_Op)
					{
						PlayersLives += Co_Op_Lives;
					}
					PlayersInGameIndex++;
				}
			}
		}
		else if (state.IsButtonDown(Buttons.Y) && mainGame.Player4InGame && !Player4InGame && !Player4AliveOnce)
		{
			RespawnPlayer4();
			Player4AliveOnce = true;
			Player4InGame = true;
			if (mainGame.Duel)
			{
				Player4Lives = Duel_Lives;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives += Co_Op_Lives;
			}
			PlayersInGameIndex++;
		}
	}

	public Vector2 Find_Safe_Respawn_Point(Vector2 Player_Position)
	{
		Vector2 vector = Player_Position * PhysicsScaleDown;
		int num = 0;
		int num2 = 10;
		int num3 = 4;
		int num4 = 0;
		int num5 = 10;
		int num6 = 0;
		bool flag = false;
		while (!flag)
		{
			bool flag2 = false;
			foreach (Body body in _world.BodyList)
			{
				bool flag3 = false;
				foreach (Fixture fixture in body.FixtureList)
				{
					Vector2 point = vector + new Vector2(num5, num5);
					if (fixture.TestPoint(ref point) && fixture != null && fixture.Body != null && fixture.Body.FixtureList != null)
					{
						flag2 = true;
						flag3 = true;
						break;
					}
					point = vector + new Vector2(-num5, num5);
					if (fixture.TestPoint(ref point) && fixture != null && fixture.Body != null && fixture.Body.FixtureList != null)
					{
						flag2 = true;
						flag3 = true;
						break;
					}
					point = vector + new Vector2(-num5, -num5);
					if (fixture.TestPoint(ref point) && fixture != null && fixture.Body != null && fixture.Body.FixtureList != null)
					{
						flag2 = true;
						flag3 = true;
						break;
					}
					point = vector + new Vector2(num5, -num5);
					if (fixture.TestPoint(ref point) && fixture != null && fixture.Body != null && fixture.Body.FixtureList != null)
					{
						flag2 = true;
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					break;
				}
			}
			if (flag2)
			{
				switch (num6)
				{
				case 0:
					vector.X += num2 * num;
					break;
				case 1:
					vector.Y += num2 * num;
					break;
				case 2:
					vector.X -= num2 * num;
					break;
				case 3:
					vector.Y -= num2 * num;
					num++;
					vector.X -= num2 * num / 2;
					vector.Y -= num2 * num / 2;
					num6 = 0;
					break;
				}
				num4 = 0;
			}
			else
			{
				num4++;
			}
			if (num4 >= num3)
			{
				flag = true;
			}
		}
		return vector;
	}

	public void RespawnPlayer1()
	{
		if (AnyAlive)
		{
			Player1Color = Color.White;
			if (mainGame.Duel)
			{
				Player1Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector = new Vector2(0f, 0f);
			vector = ((Player1[Player1Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player1Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1Index = BodyCount;
			Player1.Add(new Player1(this, mainGame, vector, _world, 1, mainGame.Player1Species, Player1Color));
			Player1Index = BodyCount;
		}
		if (BodyCount >= 1)
		{
			return;
		}
		if (!Player2InGame || !Player3InGame || !Player4InGame)
		{
			Player1Color = Color.White;
			if (mainGame.Duel)
			{
				Player1Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			BodyCount++;
			new Vector2(cameraPosition + 640f, cameraHeightPosition + 360f);
			Player1.Add(new Player1(this, mainGame, new Vector2(0f, 0f), _world, 1, mainGame.Player1Species, Player1Color));
			Player1Index = BodyCount;
			if (Player1[0] != null)
			{
				Player1[0].DestroyAll(_world);
				Player1[0] = null;
			}
		}
		else
		{
			Player1Color = Color.White;
			if (mainGame.Duel)
			{
				Player1Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector2 = new Vector2(0f, 0f);
			vector2 = ((Player1[Player1Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player1Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1Index = BodyCount;
			Player1.Add(new Player1(this, mainGame, vector2, _world, 1, mainGame.Player1Species, Player1Color));
			Player1Index = BodyCount;
		}
	}

	public void RespawnPlayer2()
	{
		if (AnyAlive)
		{
			Player2Color = Color.White;
			if (mainGame.Duel)
			{
				Player2Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector = new Vector2(0f, 0f);
			vector = ((Player1[Player2Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player2Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1.Add(new Player1(this, mainGame, vector, _world, 2, mainGame.Player2Species, Player2Color));
			Player2Index = BodyCount;
		}
		if (BodyCount >= 1)
		{
			return;
		}
		if (!Player1InGame || !Player3InGame || !Player4InGame)
		{
			Player2Color = Color.White;
			if (mainGame.Duel)
			{
				Player2Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			BodyCount++;
			new Vector2(cameraPosition + 640f, cameraHeightPosition + 360f);
			Player1.Add(new Player1(this, mainGame, new Vector2(0f, 0f), _world, 2, mainGame.Player2Species, Player2Color));
			Player2Index = BodyCount;
			if (Player1[0] != null)
			{
				Player1[0].DestroyAll(_world);
				Player1[0] = null;
			}
		}
		else
		{
			Player2Color = Color.White;
			if (mainGame.Duel)
			{
				Player2Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector2 = new Vector2(0f, 0f);
			vector2 = ((Player1[Player2Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player2Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1.Add(new Player1(this, mainGame, vector2, _world, 2, mainGame.Player2Species, Player2Color));
			Player2Index = BodyCount;
		}
	}

	public void RespawnPlayer3()
	{
		if (AnyAlive)
		{
			Player3Color = Color.White;
			if (mainGame.Duel)
			{
				Player3Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector = new Vector2(0f, 0f);
			vector = ((Player1[Player3Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player3Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1.Add(new Player1(this, mainGame, vector, _world, 3, mainGame.Player3Species, Player3Color));
			Player3Index = BodyCount;
		}
		if (BodyCount >= 1)
		{
			return;
		}
		if (!Player1InGame || !Player2InGame || !Player4InGame)
		{
			Player3Color = Color.White;
			if (mainGame.Duel)
			{
				Player3Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			BodyCount++;
			new Vector2(cameraPosition + 640f, cameraHeightPosition + 360f);
			Player1.Add(new Player1(this, mainGame, new Vector2(0f, 0f), _world, 3, mainGame.Player3Species, Player3Color));
			Player3Index = BodyCount;
			if (Player1[0] != null)
			{
				Player1[0].DestroyAll(_world);
				Player1[0] = null;
			}
		}
		else
		{
			Player3Color = Color.White;
			if (mainGame.Duel)
			{
				Player3Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector2 = new Vector2(0f, 0f);
			vector2 = ((Player1[Player3Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player3Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1.Add(new Player1(this, mainGame, vector2, _world, 3, mainGame.Player3Species, Player3Color));
			Player3Index = BodyCount;
		}
	}

	public void RespawnPlayer4()
	{
		if (AnyAlive)
		{
			Player4Color = Color.White;
			if (mainGame.Duel)
			{
				Player4Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector = new Vector2(0f, 0f);
			vector = ((Player1[Player4Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player4Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1.Add(new Player1(this, mainGame, vector, _world, 4, mainGame.Player4Species, Player4Color));
			Player4Index = BodyCount;
		}
		if (BodyCount >= 1)
		{
			return;
		}
		if (!Player1InGame || !Player2InGame || !Player3InGame)
		{
			Player4Color = Color.White;
			if (mainGame.Duel)
			{
				Player4Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			BodyCount++;
			new Vector2(cameraPosition + 640f, cameraHeightPosition + 360f);
			Player1.Add(new Player1(this, mainGame, new Vector2(0f, 0f), _world, 4, mainGame.Player4Species, Player4Color));
			Player4Index = BodyCount;
			if (Player1[0] != null)
			{
				Player1[0].DestroyAll(_world);
				Player1[0] = null;
			}
		}
		else
		{
			Player4Color = Color.White;
			if (mainGame.Duel)
			{
				Player4Lives--;
			}
			else if (mainGame.Co_Op)
			{
				PlayersLives--;
			}
			Vector2 vector2 = new Vector2(0f, 0f);
			vector2 = ((Player1[Player4Index] == null) ? new Vector2(PlayerPosition_Vec.X, PlayerPosition_Vec.Y + -100f) : (Player1[Player4Index].PlayerPosition * PhysicsScaleUp));
			BodyCount++;
			Player1.Add(new Player1(this, mainGame, vector2, _world, 4, mainGame.Player4Species, Player4Color));
			Player4Index = BodyCount;
		}
	}

	public void PlayerPosition_OLD()
	{
		if (Player1InGame)
		{
			if (!Player1[Player1Index].Dead)
			{
				if (Player1[Player1Index] != null && Player1[Player1Index]._headBody.Body != null)
				{
					Player1Position = Player1[Player1Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
			else
			{
				if (Player2InGame && !Player1[Player2Index].Dead && Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
				{
					Player1Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player3InGame && !Player1[Player3Index].Dead && Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
				{
					Player1Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player4InGame && !Player1[Player4Index].Dead && Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
				{
					Player1Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
		}
		else
		{
			if (Player2InGame && !Player1[Player2Index].Dead && Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
			{
				Player1Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player3InGame && !Player1[Player3Index].Dead && Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
			{
				Player1Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player4InGame && !Player1[Player4Index].Dead && Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
			{
				Player1Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
			}
		}
		if (Player2InGame)
		{
			if (!Player1[Player2Index].Dead)
			{
				if (Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
				{
					Player2Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
			else
			{
				if (Player1InGame && !Player1[Player1Index].Dead && Player1[Player1Index] != null && Player1[Player1Index]._headBody.Body != null)
				{
					Player2Position = Player1[Player1Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player3InGame && !Player1[Player3Index].Dead && Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
				{
					Player2Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player4InGame && !Player1[Player4Index].Dead && Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
				{
					Player2Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
		}
		else
		{
			if (Player1InGame && !Player1[Player1Index].Dead && Player1[Player1Index] != null && Player1[Player1Index]._headBody.Body != null)
			{
				Player2Position = Player1[Player1Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player3InGame && !Player1[Player3Index].Dead && Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
			{
				Player2Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player4InGame && !Player1[Player4Index].Dead && Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
			{
				Player2Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
			}
		}
		if (Player3InGame)
		{
			if (!Player1[Player3Index].Dead)
			{
				if (Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
				{
					Player3Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
			else
			{
				if (Player2InGame && !Player1[Player2Index].Dead && Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
				{
					Player3Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player1InGame && !Player1[Player1Index].Dead && Player1[Player1Index] != null && Player1[Player1Index]._headBody.Body != null)
				{
					Player3Position = Player1[Player1Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player4InGame && !Player1[Player4Index].Dead && Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
				{
					Player3Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
		}
		else
		{
			if (Player2InGame && !Player1[Player2Index].Dead && Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
			{
				Player2Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player1InGame && !Player1[Player1Index].Dead && Player1[Player1Index] != null && Player1[Player1Index]._headBody.Body != null)
			{
				Player2Position = Player1[Player1Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player4InGame && !Player1[Player4Index].Dead && Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
			{
				Player2Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
			}
		}
		if (Player4InGame)
		{
			if (!Player1[Player4Index].Dead)
			{
				if (Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
				{
					Player4Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
			else
			{
				if (Player2InGame && !Player1[Player2Index].Dead && Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
				{
					Player4Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player3InGame && !Player1[Player3Index].Dead && Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
				{
					Player4Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
				}
				if (Player1InGame && !Player1[Player1Index].Dead && Player1[Player1Index] != null && Player1[Player1Index]._headBody.Body != null)
				{
					Player4Position = Player1[Player1Index]._headBody.Body.Position * PhysicsScaleUp;
				}
			}
		}
		else
		{
			if (Player2InGame && !Player1[Player2Index].Dead && Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
			{
				Player4Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player3InGame && !Player1[Player3Index].Dead && Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
			{
				Player4Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
			}
			if (Player1InGame && !Player1[Player1Index].Dead && Player1[Player1Index] != null && Player1[Player1Index]._headBody.Body != null)
			{
				Player4Position = Player1[Player1Index]._headBody.Body.Position * PhysicsScaleUp;
			}
		}
		AveragePlayerPosition.X = (Player1Position.X + Player2Position.X + Player3Position.X + Player4Position.X) / (float)PlayersInGameIndex;
		AveragePlayerPosition.Y = (Player1Position.Y + Player2Position.Y + Player3Position.Y + Player4Position.Y) / (float)PlayersInGameIndex;
		PlayerPosition_Vec = AveragePlayerPosition;
	}

	public void PlayerPosition()
	{
		if (Player1InGame)
		{
			if (Player1.Count > Player1Index && Player1[Player1Index] != null)
			{
				_ = Player1[Player1Index].PlayerPosition;
				Player1Position = Player1[Player1Index].PlayerPosition * PhysicsScaleUp;
			}
		}
		else
		{
			Player1Position = new Vector2(0f, 0f);
		}
		if (Player2InGame)
		{
			if (Player1.Count > Player2Index && Player1[Player2Index] != null)
			{
				_ = Player1[Player2Index].PlayerPosition;
				Player2Position = Player1[Player2Index].PlayerPosition * PhysicsScaleUp;
			}
		}
		else
		{
			Player2Position = new Vector2(0f, 0f);
		}
		if (Player3InGame)
		{
			if (Player1.Count > Player3Index && Player1[Player3Index] != null)
			{
				_ = Player1[Player3Index].PlayerPosition;
				Player3Position = Player1[Player3Index].PlayerPosition * PhysicsScaleUp;
			}
		}
		else
		{
			Player3Position = new Vector2(0f, 0f);
		}
		if (Player4InGame)
		{
			if (Player1.Count > Player4Index && Player1[Player4Index] != null)
			{
				_ = Player1[Player4Index].PlayerPosition;
				Player4Position = Player1[Player4Index].PlayerPosition * PhysicsScaleUp;
			}
		}
		else
		{
			Player4Position = new Vector2(0f, 0f);
		}
		AveragePlayerPosition.X = (Player1Position.X + Player2Position.X + Player3Position.X + Player4Position.X) / (float)PlayersInGameIndex;
		AveragePlayerPosition.Y = (Player1Position.Y + Player2Position.Y + Player3Position.Y + Player4Position.Y) / (float)PlayersInGameIndex;
		float num = 100f;
		float num2 = 0.05f;
		float num3 = 0.1f;
		if (AveragePlayerPosition.X > PlayerPosition_Vec.X + num)
		{
			AveragePlayerPosition_Ease.X += (AveragePlayerPosition.X - (PlayerPosition_Vec.X + num)) * num2;
		}
		else if (AveragePlayerPosition.X < PlayerPosition_Vec.X - num)
		{
			AveragePlayerPosition_Ease.X += (AveragePlayerPosition.X - (PlayerPosition_Vec.X + num)) * num2;
		}
		if (AveragePlayerPosition.Y > PlayerPosition_Vec.Y + num)
		{
			AveragePlayerPosition_Ease.Y += (AveragePlayerPosition.Y - (PlayerPosition_Vec.Y + num)) * num3;
		}
		else if (AveragePlayerPosition.Y < PlayerPosition_Vec.Y - num)
		{
			AveragePlayerPosition_Ease.Y += (AveragePlayerPosition.Y - (PlayerPosition_Vec.Y + num)) * num3;
		}
		PlayerPosition_Vec = AveragePlayerPosition_Ease;
		if (PlayersInGameIndex == 0)
		{
			PlayerPosition_Vec = new Vector2(0f, 0f);
		}
	}

	public void PlayerDistApart()
	{
		if (Player1InGame)
		{
			if (Player1.Count > Player1Index && Player1[Player1Index] != null)
			{
				_ = Player1[Player1Index].PlayerPosition;
				Player1Position = Player1[Player1Index].PlayerPosition * PhysicsScaleUp;
			}
		}
		else
		{
			Player1Position = new Vector2(0f, 0f);
		}
		if (Player2InGame)
		{
			if (Player1.Count > Player2Index && Player1[Player2Index] != null && Player1[Player2Index]._headBody.Body != null)
			{
				Player2Position = Player1[Player2Index]._headBody.Body.Position * PhysicsScaleUp;
			}
		}
		else
		{
			Player2Position = new Vector2(0f, 0f);
		}
		if (Player3InGame)
		{
			if (Player1.Count > Player3Index && Player1[Player3Index] != null && Player1[Player3Index]._headBody.Body != null)
			{
				Player3Position = Player1[Player3Index]._headBody.Body.Position * PhysicsScaleUp;
			}
		}
		else
		{
			Player3Position = new Vector2(0f, 0f);
		}
		if (Player4InGame)
		{
			if (Player1.Count > Player4Index && Player1[Player4Index] != null && Player1[Player4Index]._headBody.Body != null)
			{
				Player4Position = Player1[Player4Index]._headBody.Body.Position * PhysicsScaleUp;
			}
		}
		else
		{
			Player4Position = new Vector2(0f, 0f);
		}
		Vector2 vector = new Vector2(0f, 0f);
		float playerDistApart_Float = 1f;
		if (Player1InGame && Player2InGame && Player3InGame && Player4InGame)
		{
			vector.X = Math.Max(MathHelper.Distance(Player1Position.X, Player2Position.X), MathHelper.Distance(Player1Position.X, Player3Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player1Position.X, Player4Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player3Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player4Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player3Position.X, Player4Position.X));
			vector.Y = Math.Max(MathHelper.Distance(Player1Position.Y, Player2Position.Y), MathHelper.Distance(Player1Position.Y, Player3Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player1Position.Y, Player4Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player3Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player4Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player3Position.Y, Player4Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (!Player1InGame && Player2InGame && Player3InGame && Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player3Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player4Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player3Position.X, Player4Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player3Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player4Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player3Position.Y, Player4Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (Player1InGame && !Player2InGame && Player3InGame && Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player1Position.X, Player3Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player1Position.X, Player4Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player3Position.X, Player4Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player1Position.Y, Player3Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player1Position.Y, Player4Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player3Position.Y, Player4Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (Player1InGame && Player2InGame && !Player3InGame && Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player1Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player4Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player1Position.X, Player4Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player3Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player4Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player3Position.Y, Player4Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (Player1InGame && Player2InGame && Player3InGame && !Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player3Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player1Position.X));
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player3Position.X, Player1Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player3Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player1Position.Y));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player3Position.Y, Player1Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (Player1InGame && Player2InGame && !Player3InGame && !Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player1Position.X, Player2Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player1Position.Y, Player2Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (Player1InGame && !Player2InGame && Player3InGame && !Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player1Position.X, Player3Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player1Position.Y, Player3Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (Player1InGame && !Player2InGame && !Player3InGame && Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player1Position.X, Player4Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player1Position.Y, Player4Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (!Player1InGame && Player2InGame && Player3InGame && !Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player3Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player3Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (!Player1InGame && Player2InGame && !Player3InGame && Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player2Position.X, Player4Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player2Position.Y, Player4Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		else if (!Player1InGame && !Player2InGame && Player3InGame && Player4InGame)
		{
			vector.X = Math.Max(vector.X, MathHelper.Distance(Player3Position.X, Player4Position.X));
			vector.Y = Math.Max(vector.Y, MathHelper.Distance(Player3Position.Y, Player4Position.Y));
			playerDistApart_Float = Math.Max(vector.X, vector.Y);
		}
		PlayerDistApart_Float = playerDistApart_Float;
	}

	public Vector2 RespawnPlayerPosition()
	{
		if (Player1InGame)
		{
			if (Player1[Player1Index].Alive)
			{
				Player1Position = Player1[Player1Index]._bodyBody.Body.Position;
			}
		}
		else
		{
			Player1Position = new Vector2(0f, 0f);
		}
		if (Player2InGame)
		{
			if (Player1[Player2Index].Alive)
			{
				Player2Position = Player1[Player2Index]._bodyBody.Body.Position;
			}
		}
		else
		{
			Player2Position = new Vector2(0f, 0f);
		}
		if (Player3InGame)
		{
			if (Player1[Player3Index].Alive)
			{
				Player3Position = Player1[Player3Index]._bodyBody.Body.Position;
			}
		}
		else
		{
			Player3Position = new Vector2(0f, 0f);
		}
		if (Player4InGame)
		{
			if (Player1[Player4Index].Alive)
			{
				Player4Position = Player1[Player4Index]._bodyBody.Body.Position;
			}
		}
		else
		{
			Player4Position = new Vector2(0f, 0f);
		}
		if (PlayersInGameIndex > 0)
		{
			AveragePlayerPosition.X = (Player1Position.X + Player2Position.X + Player3Position.X + Player4Position.X) / (float)PlayersInGameIndex;
			AveragePlayerPosition.Y = (Player1Position.Y + Player2Position.Y + Player3Position.Y + Player4Position.Y) / (float)PlayersInGameIndex;
		}
		return AveragePlayerPosition;
	}

	private static float Pulsate(GameTime gameTime, float speed, float min, float max)
	{
		double a = gameTime.TotalGameTime.TotalSeconds * (double)speed;
		return min + ((float)Math.Sin(a) + 1f) / 2f * (max - min);
	}
}
