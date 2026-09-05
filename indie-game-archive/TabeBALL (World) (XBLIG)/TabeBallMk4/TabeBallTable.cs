using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TabeBallMk4;

public class TabeBallTable : GameComponent
{
	private enum gameModes
	{
		startScreen,
		rootMenu,
		gameSetupMenu,
		controls,
		hints,
		pauseMenu,
		kickOff,
		playing,
		credits,
		finalScore
	}

	public enum whoControls
	{
		P1 = 1,
		P2,
		P3,
		P4,
		AI1_Novice,
		AI2_Normal,
		AI3_Hard
	}

	public int gameMode;

	public int menuController;

	private int showFinalScoreMin;

	private bool victoryPlayed;

	private float ballRadius;

	private float playerBaseRadius;

	private float pitchXMax;

	private float pitchXMin;

	private float pitchZMax;

	private float pitchZMin;

	private float postFromCorner;

	private float rowGap;

	private float[] rowPositionsZ;

	private float goaliesDefendersOffSet;

	private float centerbacksOffSet;

	private float midfieldOffSet;

	private float midfieldCentres;

	private float forwardsOffSet;

	private int[] clock;

	public int timeLimitIndex;

	private int goalCountDown;

	private bool redGoal;

	private bool blueGoal;

	public int redScore;

	public int blueScore;

	private float ballSpeed;

	private float ballSpeedStun;

	private float ballSpeedMin;

	private float ballSpeedMaxNormal;

	private float actualBallMoveSpeed;

	private float speedSettingMuiltiplier;

	private float impactDeceleration;

	private float wallImpactDeceleration;

	private float frictionDeceleration;

	private int stunTime;

	private int stun;

	private float ballBearing;

	private Vector3 ballPosition;

	private float handle0and3Rotation;

	private float handle1and5Rotation;

	private float handle2and6Rotation;

	private float handle4and7Rotation;

	private float handle0and3RotationLastFrame;

	private float handle1and5RotationLastFrame;

	private float handle2and6RotationLastFrame;

	private float handle4and7RotationLastFrame;

	private int framesToTrack;

	private int framesToTrackItterator;

	private float[] handle0and3rotTracker;

	private float[] handle1and5rotTracker;

	private float[] handle2and6rotTracker;

	private float[] handle4and7rotTracker;

	private float[] handle0and3valTracker;

	private float[] handle1and5valTracker;

	private float[] handle2and6valTracker;

	private float[] handle4and7valTracker;

	private float maxHandleXValue;

	private float handle0and3Xvalue;

	private float handle1and5Xvalue;

	private float handle2and6Xvalue;

	private float handle4and7Xvalue;

	private Vector3 kickerPos;

	private int lastKicker;

	private int lastKickerTTL;

	private int setLastKickerTTL;

	public int redStickStyle;

	public int blueStickStyle;

	public int redController;

	public int blueController;

	private GamePadState gp1State;

	private GamePadState gp2State;

	private GamePadState gp3State;

	private GamePadState gp4State;

	private bool row03stun;

	private bool row15stun;

	private bool row26stun;

	private bool row47stun;

	private Vector3 modelPosition;

	private Vector3 cameraPosition;

	private Vector3 cameraLookAt;

	private Vector3 desiredCameraPosition;

	private Vector3 desiredCameraLookAt;

	private int cameraMode;

	private bool yPressed;

	private Model redGuy;

	private Model blueGuy;

	private Model redGoalie;

	private Model blueGoalie;

	private Model pipe;

	private Model ball;

	private Model pitch;

	private Model room;

	private Model post;

	private Model redHandle;

	private Model blueHandle;

	private Model lenseTint;

	private float aspectRatio;

	private float farPlaneDist;

	private float ballSpin;

	private SpriteFont Font1;

	private SpriteBatch spriteBatch;

	private Texture2D pauseMenu;

	private Texture2D blankTexture;

	private Texture2D pointerT;

	private Texture2D scoreboardT;

	private Texture2D clockT;

	private Model testMarker;

	private Vector3 markerPos;

	private string debugString;

	private int pauser;

	private bool isPaused;

	private bool isPausedLock;

	private bool unPauseNextRelease;

	private int pointerLag;

	private int setPointerLag;

	private Matrix viewMatrix;

	private Matrix projectionMatrix;

	private Vector3 playerTrans;

	private Vector3 playerTrans2;

	private bool pausePointerOnResume;

	private float ballRotX;

	private float ballRotZ;

	private SoundEffect tabeBallSong1;

	private bool songPlaying;

	private SoundEffectInstance pauseMusic;

	public SoundEffect kickSoft;

	public SoundEffect kickMed;

	public SoundEffect kickHard;

	public SoundEffect wallBounce;

	private SoundEffect redGoalSound;

	private SoundEffect blueGoalSound;

	private float CPU_moveSpeed;

	private int CPU_decisionMin;

	private int CPU_decisionMax;

	private Random CPU_Random;

	private float CPU_easyShootSpeed;

	private float CPU_hardShootSpeed;

	private float CPU_RED_speedRandomizer;

	private float CPU_RED_speedRandomizer2;

	private int CPU_RED_timeToDecision;

	private int CPU_RED_timeToDecision2;

	private bool CPU_RED_test1;

	private bool CPU_RED_test2;

	private bool CPU_RED_shoot1;

	private bool CPU_RED_shoot2;

	private bool CPU_RED_rollback;

	private float CPU_BLUE_speedRandomizer;

	private float CPU_BLUE_speedRandomizer2;

	private int CPU_BLUE_timeToDecision;

	private int CPU_BLUE_timeToDecision2;

	private bool CPU_BLUE_test1;

	private bool CPU_BLUE_test2;

	private bool CPU_BLUE_shoot1;

	private bool CPU_BLUE_shoot2;

	private bool CPU_BLUE_rollback;

	private int ttl_redCockup;

	private float CPU_redCockup;

	private int ttl_redCockup2;

	private float CPU_redCockup2;

	private int ttl_cockup;

	private float CPU_cockup;

	private int ttl_cockup2;

	private float CPU_cockup2;

	private int CPU_BLUE_stunCounter;

	private int CPU_RED_stunCounter;

	private float CPU_move03;

	private float CPU_move15;

	private float CPU_move47;

	private float CPU_move26;

	public TabeBallTable(Game game)
	{
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		gameMode = 0;
		menuController = 1;
		showFinalScoreMin = 30;
		victoryPlayed = false;
		ballRadius = 100f;
		playerBaseRadius = 120f;
		pitchXMax = 4800f;
		pitchXMin = 0f;
		pitchZMax = 9600f;
		pitchZMin = 0f;
		postFromCorner = 1500f;
		rowGap = 1200f;
		rowPositionsZ = new float[8];
		goaliesDefendersOffSet = 500f;
		centerbacksOffSet = 1000f;
		midfieldOffSet = 250f;
		midfieldCentres = 900f;
		forwardsOffSet = 600f;
		int[] array = new int[6];
		clock = array;
		timeLimitIndex = 0;
		goalCountDown = 0;
		redGoal = false;
		blueGoal = false;
		redScore = 0;
		blueScore = 0;
		ballSpeed = 20f;
		ballSpeedStun = 1.5f;
		ballSpeedMin = 10f;
		ballSpeedMaxNormal = 100f;
		speedSettingMuiltiplier = 1f;
		impactDeceleration = 20f;
		wallImpactDeceleration = 4f;
		frictionDeceleration = 0.5f;
		stunTime = 60;
		stun = 0;
		ballBearing = 2.2f;
		ballPosition = new Vector3(2400f, 100f, 4800f);
		framesToTrack = 10;
		framesToTrackItterator = 0;
		handle0and3rotTracker = new float[10];
		handle1and5rotTracker = new float[10];
		handle2and6rotTracker = new float[10];
		handle4and7rotTracker = new float[10];
		handle0and3valTracker = new float[10];
		handle1and5valTracker = new float[10];
		handle2and6valTracker = new float[10];
		handle4and7valTracker = new float[10];
		maxHandleXValue = 1600f;
		handle0and3Xvalue = 0f;
		handle1and5Xvalue = 0f;
		handle2and6Xvalue = 0f;
		handle4and7Xvalue = 0f;
		kickerPos = new Vector3(0f, 0f, 0f);
		lastKicker = 0;
		lastKickerTTL = 0;
		setLastKickerTTL = 10;
		redStickStyle = 0;
		blueStickStyle = 0;
		row03stun = false;
		row15stun = false;
		row26stun = false;
		row47stun = false;
		modelPosition = Vector3.Zero;
		cameraPosition = new Vector3(0f, 3000f, 5000f);
		cameraLookAt = new Vector3(0f, 10f, 0f);
		desiredCameraPosition = new Vector3(2399f, 10000f, 4800f);
		desiredCameraLookAt = new Vector3(0f, 10f, 0f);
		cameraMode = 2;
		yPressed = false;
		farPlaneDist = 40000f;
		ballSpin = 0f;
		markerPos = new Vector3(0f, 0f, 0f);
		pauser = 0;
		isPaused = false;
		isPausedLock = false;
		unPauseNextRelease = false;
		pointerLag = 0;
		setPointerLag = 10;
		playerTrans = new Vector3(0f, -450f, 0f);
		playerTrans2 = new Vector3(0f, 450f, 0f);
		pausePointerOnResume = true;
		ballRotX = 0f;
		ballRotZ = 0f;
		songPlaying = false;
		CPU_moveSpeed = 15f;
		CPU_decisionMin = 30;
		CPU_decisionMax = 120;
		CPU_Random = new Random();
		CPU_easyShootSpeed = 0.2f;
		CPU_hardShootSpeed = 0.35f;
		CPU_RED_speedRandomizer = 1f;
		CPU_RED_speedRandomizer2 = 1f;
		CPU_RED_timeToDecision = 0;
		CPU_RED_timeToDecision2 = 0;
		CPU_RED_test1 = true;
		CPU_RED_test2 = false;
		CPU_RED_shoot1 = false;
		CPU_RED_shoot2 = false;
		CPU_RED_rollback = false;
		CPU_BLUE_speedRandomizer = 1f;
		CPU_BLUE_speedRandomizer2 = 1f;
		CPU_BLUE_timeToDecision = 0;
		CPU_BLUE_timeToDecision2 = 0;
		CPU_BLUE_test1 = true;
		CPU_BLUE_test2 = false;
		CPU_BLUE_shoot1 = false;
		CPU_BLUE_shoot2 = false;
		CPU_BLUE_rollback = false;
		ttl_redCockup = 0;
		CPU_redCockup = 0.1f;
		ttl_redCockup2 = 0;
		CPU_redCockup2 = 0.1f;
		ttl_cockup = 0;
		CPU_cockup = 0.1f;
		ttl_cockup2 = 0;
		CPU_cockup2 = 0.1f;
		CPU_BLUE_stunCounter = 0;
		CPU_RED_stunCounter = 0;
		CPU_move03 = 0f;
		CPU_move15 = 0f;
		CPU_move47 = 0f;
		CPU_move26 = 0f;
		((GameComponent)this)._002Ector(game);
	}

	public override void Initialize()
	{
		((GameComponent)this).Initialize();
	}

	public void LoadModels()
	{
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		redGuy = ((GameComponent)this).Game.Content.Load<Model>("Models\\RedPlayer");
		redGoalie = ((GameComponent)this).Game.Content.Load<Model>("Models\\RedGoalie");
		blueGuy = ((GameComponent)this).Game.Content.Load<Model>("Models\\BluePlayer");
		blueGoalie = ((GameComponent)this).Game.Content.Load<Model>("Models\\BlueGoalie");
		redHandle = ((GameComponent)this).Game.Content.Load<Model>("Models\\RedHandle");
		blueHandle = ((GameComponent)this).Game.Content.Load<Model>("Models\\BlueHandle");
		pipe = ((GameComponent)this).Game.Content.Load<Model>("Models\\P1Pipe");
		ball = ((GameComponent)this).Game.Content.Load<Model>("Models\\texturedBall");
		pitch = ((GameComponent)this).Game.Content.Load<Model>("Models\\PitchBox");
		room = ((GameComponent)this).Game.Content.Load<Model>("Models\\Room");
		post = ((GameComponent)this).Game.Content.Load<Model>("Models\\P1Post");
		Font1 = ((GameComponent)this).Game.Content.Load<SpriteFont>("Courier New");
		spriteBatch = new SpriteBatch(((GameComponent)this).Game.GraphicsDevice);
		pauseMenu = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\PauseMenu");
		blankTexture = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\MainMenu");
		pointerT = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\PointerMK1");
		scoreboardT = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\scoreBackground");
		clockT = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\clockBackground");
		testMarker = ((GameComponent)this).Game.Content.Load<Model>("Models\\testMarker");
		debugString = "no data";
		redController = 1;
		blueController = 6;
		rowPositionsZ[0] = rowGap / 2f;
		for (int i = 1; i < rowPositionsZ.Length; i++)
		{
			rowPositionsZ[i] = rowPositionsZ[i - 1] + rowGap;
		}
		Viewport viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		aspectRatio = ((Viewport)(ref viewport)).AspectRatio;
		loadSounds();
	}

	public bool SetSpeed()
	{
		return true;
	}

	public bool pausePressed(int whoPaused)
	{
		if (!isPausedLock)
		{
			if (!isPaused)
			{
				isPausedLock = true;
				unPauseNextRelease = false;
				pauser = whoPaused;
				isPaused = true;
			}
			else if (whoPaused == pauser)
			{
				unPauseNextRelease = true;
			}
		}
		return true;
	}

	public bool pauseReleased(int whoReleased)
	{
		if (whoReleased == pauser)
		{
			isPausedLock = false;
			if (unPauseNextRelease)
			{
				isPaused = false;
			}
		}
		return true;
	}

	public override void Update(GameTime gameTime)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Invalid comparison between Unknown and I4
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Invalid comparison between Unknown and I4
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Invalid comparison between Unknown and I4
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Invalid comparison between Unknown and I4
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Invalid comparison between Unknown and I4
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Invalid comparison between Unknown and I4
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Invalid comparison between Unknown and I4
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Invalid comparison between Unknown and I4
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Invalid comparison between Unknown and I4
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Invalid comparison between Unknown and I4
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Invalid comparison between Unknown and I4
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Invalid comparison between Unknown and I4
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Invalid comparison between Unknown and I4
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Invalid comparison between Unknown and I4
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Invalid comparison between Unknown and I4
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Invalid comparison between Unknown and I4
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Invalid comparison between Unknown and I4
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Invalid comparison between Unknown and I4
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Invalid comparison between Unknown and I4
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Invalid comparison between Unknown and I4
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Invalid comparison between Unknown and I4
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Invalid comparison between Unknown and I4
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Invalid comparison between Unknown and I4
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Invalid comparison between Unknown and I4
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Invalid comparison between Unknown and I4
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e05: Invalid comparison between Unknown and I4
		//IL_115c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1161: Unknown result type (might be due to invalid IL or missing references)
		//IL_1165: Unknown result type (might be due to invalid IL or missing references)
		//IL_117c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1181: Unknown result type (might be due to invalid IL or missing references)
		//IL_1185: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10de: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1103: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_110c: Unknown result type (might be due to invalid IL or missing references)
		//IL_112f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1134: Unknown result type (might be due to invalid IL or missing references)
		//IL_1138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1004: Invalid comparison between Unknown and I4
		//IL_0e1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2c: Invalid comparison between Unknown and I4
		//IL_256b: Unknown result type (might be due to invalid IL or missing references)
		//IL_135b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1360: Unknown result type (might be due to invalid IL or missing references)
		//IL_1364: Unknown result type (might be due to invalid IL or missing references)
		//IL_137b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1380: Unknown result type (might be due to invalid IL or missing references)
		//IL_1384: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12be: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1302: Unknown result type (might be due to invalid IL or missing references)
		//IL_1307: Unknown result type (might be due to invalid IL or missing references)
		//IL_130b: Unknown result type (might be due to invalid IL or missing references)
		//IL_132e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1333: Unknown result type (might be due to invalid IL or missing references)
		//IL_1337: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1203: Invalid comparison between Unknown and I4
		//IL_101c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1021: Unknown result type (might be due to invalid IL or missing references)
		//IL_1025: Unknown result type (might be due to invalid IL or missing references)
		//IL_102b: Invalid comparison between Unknown and I4
		//IL_0e44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e53: Invalid comparison between Unknown and I4
		//IL_267c: Unknown result type (might be due to invalid IL or missing references)
		//IL_155a: Unknown result type (might be due to invalid IL or missing references)
		//IL_155f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1563: Unknown result type (might be due to invalid IL or missing references)
		//IL_157a: Unknown result type (might be due to invalid IL or missing references)
		//IL_157f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1583: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1501: Unknown result type (might be due to invalid IL or missing references)
		//IL_1506: Unknown result type (might be due to invalid IL or missing references)
		//IL_150a: Unknown result type (might be due to invalid IL or missing references)
		//IL_152d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1532: Unknown result type (might be due to invalid IL or missing references)
		//IL_1536: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1402: Invalid comparison between Unknown and I4
		//IL_121b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1220: Unknown result type (might be due to invalid IL or missing references)
		//IL_1224: Unknown result type (might be due to invalid IL or missing references)
		//IL_122a: Invalid comparison between Unknown and I4
		//IL_1043: Unknown result type (might be due to invalid IL or missing references)
		//IL_1048: Unknown result type (might be due to invalid IL or missing references)
		//IL_104c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1052: Invalid comparison between Unknown and I4
		//IL_0e6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7a: Invalid comparison between Unknown and I4
		//IL_278e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1759: Unknown result type (might be due to invalid IL or missing references)
		//IL_175e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1762: Unknown result type (might be due to invalid IL or missing references)
		//IL_1779: Unknown result type (might be due to invalid IL or missing references)
		//IL_177e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1782: Unknown result type (might be due to invalid IL or missing references)
		//IL_179f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_16db: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1700: Unknown result type (might be due to invalid IL or missing references)
		//IL_1705: Unknown result type (might be due to invalid IL or missing references)
		//IL_1709: Unknown result type (might be due to invalid IL or missing references)
		//IL_172c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1731: Unknown result type (might be due to invalid IL or missing references)
		//IL_1735: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1601: Invalid comparison between Unknown and I4
		//IL_141a: Unknown result type (might be due to invalid IL or missing references)
		//IL_141f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1423: Unknown result type (might be due to invalid IL or missing references)
		//IL_1429: Invalid comparison between Unknown and I4
		//IL_1242: Unknown result type (might be due to invalid IL or missing references)
		//IL_1247: Unknown result type (might be due to invalid IL or missing references)
		//IL_124b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1251: Invalid comparison between Unknown and I4
		//IL_106a: Unknown result type (might be due to invalid IL or missing references)
		//IL_106f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1073: Unknown result type (might be due to invalid IL or missing references)
		//IL_1079: Invalid comparison between Unknown and I4
		//IL_1958: Unknown result type (might be due to invalid IL or missing references)
		//IL_195d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1961: Unknown result type (might be due to invalid IL or missing references)
		//IL_1978: Unknown result type (might be due to invalid IL or missing references)
		//IL_197d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1981: Unknown result type (might be due to invalid IL or missing references)
		//IL_199e: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_18da: Unknown result type (might be due to invalid IL or missing references)
		//IL_18df: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1904: Unknown result type (might be due to invalid IL or missing references)
		//IL_1908: Unknown result type (might be due to invalid IL or missing references)
		//IL_192b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1930: Unknown result type (might be due to invalid IL or missing references)
		//IL_1934: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1800: Invalid comparison between Unknown and I4
		//IL_1619: Unknown result type (might be due to invalid IL or missing references)
		//IL_161e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1622: Unknown result type (might be due to invalid IL or missing references)
		//IL_1628: Invalid comparison between Unknown and I4
		//IL_1441: Unknown result type (might be due to invalid IL or missing references)
		//IL_1446: Unknown result type (might be due to invalid IL or missing references)
		//IL_144a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1450: Invalid comparison between Unknown and I4
		//IL_1269: Unknown result type (might be due to invalid IL or missing references)
		//IL_126e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1272: Unknown result type (might be due to invalid IL or missing references)
		//IL_1278: Invalid comparison between Unknown and I4
		//IL_2918: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bce: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b33: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ff: Invalid comparison between Unknown and I4
		//IL_1818: Unknown result type (might be due to invalid IL or missing references)
		//IL_181d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1821: Unknown result type (might be due to invalid IL or missing references)
		//IL_1827: Invalid comparison between Unknown and I4
		//IL_1640: Unknown result type (might be due to invalid IL or missing references)
		//IL_1645: Unknown result type (might be due to invalid IL or missing references)
		//IL_1649: Unknown result type (might be due to invalid IL or missing references)
		//IL_164f: Invalid comparison between Unknown and I4
		//IL_1468: Unknown result type (might be due to invalid IL or missing references)
		//IL_146d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1471: Unknown result type (might be due to invalid IL or missing references)
		//IL_1477: Invalid comparison between Unknown and I4
		//IL_2a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bfe: Invalid comparison between Unknown and I4
		//IL_1a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a26: Invalid comparison between Unknown and I4
		//IL_183f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1844: Unknown result type (might be due to invalid IL or missing references)
		//IL_1848: Unknown result type (might be due to invalid IL or missing references)
		//IL_184e: Invalid comparison between Unknown and I4
		//IL_1667: Unknown result type (might be due to invalid IL or missing references)
		//IL_166c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1670: Unknown result type (might be due to invalid IL or missing references)
		//IL_1676: Invalid comparison between Unknown and I4
		//IL_1c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c25: Invalid comparison between Unknown and I4
		//IL_1a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a47: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4d: Invalid comparison between Unknown and I4
		//IL_1866: Unknown result type (might be due to invalid IL or missing references)
		//IL_186b: Unknown result type (might be due to invalid IL or missing references)
		//IL_186f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1875: Invalid comparison between Unknown and I4
		//IL_1c3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c42: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c46: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4c: Invalid comparison between Unknown and I4
		//IL_1a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a74: Invalid comparison between Unknown and I4
		//IL_2bbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c69: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c73: Invalid comparison between Unknown and I4
		//IL_2cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ef2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3083: Unknown result type (might be due to invalid IL or missing references)
		//IL_3197: Unknown result type (might be due to invalid IL or missing references)
		//IL_3329: Unknown result type (might be due to invalid IL or missing references)
		//IL_343d: Unknown result type (might be due to invalid IL or missing references)
		//IL_35cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_36dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_37f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_390b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f21: Unknown result type (might be due to invalid IL or missing references)
		gp1State = GamePad.GetState((PlayerIndex)0);
		gp2State = GamePad.GetState((PlayerIndex)1);
		gp3State = GamePad.GetState((PlayerIndex)2);
		gp4State = GamePad.GetState((PlayerIndex)3);
		GamePadButtons buttons = ((GamePadState)(ref gp1State)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).Y != 1)
		{
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Y != 1)
			{
				buttons = ((GamePadState)(ref gp3State)).Buttons;
				if ((int)((GamePadButtons)(ref buttons)).Y != 1)
				{
					buttons = ((GamePadState)(ref gp4State)).Buttons;
					if ((int)((GamePadButtons)(ref buttons)).Y != 1)
					{
						goto IL_00d7;
					}
				}
			}
		}
		if (!yPressed)
		{
			yPressed = true;
			cameraMode++;
			if (cameraMode > 3)
			{
				cameraMode = 1;
			}
		}
		goto IL_00d7;
		IL_0718:
		pointerLag = setPointerLag;
		if (!pausePointerOnResume)
		{
			pausePointerOnResume = true;
			kickSoft.Play();
		}
		else
		{
			wallBounce.Play();
		}
		goto IL_1f08;
		IL_1f08:
		if (ballPosition.X < pitchXMin + ballRadius)
		{
			ballBearing = (float)Math.PI - ballBearing;
			ballPosition.X = pitchXMin + ballRadius;
			ballSpeed -= wallImpactDeceleration;
			wallBounce.Play();
			lastKicker = 0;
		}
		if (ballPosition.X > pitchXMax - ballRadius)
		{
			ballBearing = (float)Math.PI - ballBearing;
			ballPosition.X = pitchXMax - ballRadius;
			ballSpeed -= wallImpactDeceleration;
			wallBounce.Play();
			lastKicker = 0;
		}
		if (ballPosition.Z < pitchZMin + ballRadius)
		{
			if (gameMode == 7 && !blueGoal && ballPosition.X > postFromCorner + ballRadius && ballPosition.X < pitchXMax - postFromCorner - ballRadius)
			{
				blueGoal = true;
				goalCountDown = 90;
				blueGoalSound.Play();
			}
			if (blueGoal)
			{
				if (ballPosition.Z < -400f)
				{
					ballPosition.Z = -400f;
					ballSpeed = -0.1f;
					wallBounce.Play();
				}
				goalCountDown--;
				if (goalCountDown < 1)
				{
					blueGoal = false;
					blueScore++;
					ballPosition.X = pitchXMax / 2f;
					ballPosition.Z = pitchZMax / 2f;
					ballSpeed = ballSpeedStun;
					stun = stunTime * 2;
				}
			}
			else
			{
				ballBearing = 0f - ballBearing;
				ballPosition.Z = ballRadius;
				ballSpeed -= wallImpactDeceleration;
				wallBounce.Play();
			}
			lastKicker = 0;
		}
		if (ballPosition.Z > pitchZMax - ballRadius)
		{
			if (gameMode == 7 && !redGoal && ballPosition.X > postFromCorner + ballRadius && ballPosition.X < pitchXMax - postFromCorner - ballRadius)
			{
				redGoal = true;
				goalCountDown = 90;
				redGoalSound.Play();
			}
			if (redGoal)
			{
				if (ballPosition.Z > pitchZMax + 600f)
				{
					ballPosition.Z = pitchZMax + 600f;
					ballSpeed = -0.1f;
					wallBounce.Play();
				}
				goalCountDown--;
				if (goalCountDown < 1)
				{
					redGoal = false;
					redScore++;
					ballPosition.X = pitchXMax / 2f;
					ballPosition.Z = pitchZMax / 2f;
					ballSpeed = ballSpeedStun;
					stun = stunTime * 2;
				}
			}
			else
			{
				ballBearing = 0f - ballBearing;
				ballPosition.Z = pitchZMax - ballRadius;
				ballSpeed -= wallImpactDeceleration;
				wallBounce.Play();
			}
			lastKicker = 0;
		}
		if (lastKickerTTL > 0)
		{
			lastKickerTTL--;
		}
		if (lastKickerTTL == 0)
		{
			lastKicker = 0;
		}
		float num = 300f;
		float num2 = 400f;
		markerPos.X = pitchXMax / 2f;
		markerPos.Y = 400f + (float)Math.Cos(handle0and3Rotation) * -300f;
		markerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * -300f;
		markerPos.Y = ballPosition.Y;
		markerPos.Z = ballPosition.Z;
		if (ballPosition.Z < rowGap)
		{
			if (lastKicker != 1)
			{
				kickerPos.X = handle0and3Xvalue + goaliesDefendersOffSet;
				kickerPos.Z = rowPositionsZ[0] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[0] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
					playerKick(kickerPos, 0);
					lastKicker = 1;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 2)
			{
				kickerPos.X = handle0and3Xvalue + (pitchXMax - maxHandleXValue) / 2f;
				kickerPos.Z = rowPositionsZ[0] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[0] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
					playerKick(kickerPos, 0);
					lastKicker = 2;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 3)
			{
				kickerPos.X = handle0and3Xvalue + (pitchXMax - maxHandleXValue) - goaliesDefendersOffSet;
				kickerPos.Z = rowPositionsZ[0] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[0] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
					playerKick(kickerPos, 0);
					lastKicker = 3;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		if ((ballPosition.Z > rowGap * 1.5f && ballPosition.Z < rowGap * 2f) || (ballPosition.Z > rowGap && ballPosition.Z < rowGap * 2f && ballSpeed < ballSpeedMin * 3f))
		{
			if (lastKicker != 4)
			{
				kickerPos.X = handle1and5Xvalue + centerbacksOffSet;
				kickerPos.Z = rowPositionsZ[1] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[1] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
					playerKick(kickerPos, 1);
					lastKicker = 4;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 5)
			{
				kickerPos.X = handle1and5Xvalue + (pitchXMax - maxHandleXValue) - centerbacksOffSet;
				kickerPos.Z = rowPositionsZ[1] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[1] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
					playerKick(kickerPos, 1);
					lastKicker = 5;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		if ((ballPosition.Z > rowGap * 3.5f && ballPosition.Z < rowGap * 4f) || (ballPosition.Z > rowGap * 3f && ballPosition.Z < rowGap * 4f && ballSpeed < ballSpeedMin * 3f))
		{
			if (lastKicker != 9)
			{
				kickerPos.X = handle0and3Xvalue + midfieldOffSet;
				kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
					playerKick(kickerPos, 3);
					lastKicker = 9;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 8)
			{
				kickerPos.X = handle0and3Xvalue + midfieldOffSet + midfieldCentres;
				kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
					playerKick(kickerPos, 3);
					lastKicker = 8;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 7)
			{
				kickerPos.X = handle0and3Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres;
				kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
					playerKick(kickerPos, 3);
					lastKicker = 7;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 6)
			{
				kickerPos.X = handle0and3Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres + midfieldCentres;
				kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[3] + (float)Math.Sin(handle0and3Rotation) * (0f - num);
					playerKick(kickerPos, 3);
					lastKicker = 6;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		if ((ballPosition.Z > rowGap * 5.5f && ballPosition.Z < rowGap * 6f) || (ballPosition.Z > rowGap * 5f && ballPosition.Z < rowGap * 6f && ballSpeed < ballSpeedMin * 3f))
		{
			if (lastKicker != 10)
			{
				kickerPos.X = handle1and5Xvalue + forwardsOffSet;
				kickerPos.Z = rowPositionsZ[5] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[5] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
					playerKick(kickerPos, 5);
					lastKicker = 10;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 11)
			{
				kickerPos.X = handle1and5Xvalue + (pitchXMax - maxHandleXValue) - forwardsOffSet;
				kickerPos.Z = rowPositionsZ[5] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[5] + (float)Math.Sin(handle1and5Rotation) * (0f - num);
					playerKick(kickerPos, 5);
					lastKicker = 11;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		if ((ballPosition.Z > rowGap * 2f && ballPosition.Z < rowGap * 2.5f) || (ballPosition.Z > rowGap * 2f && ballPosition.Z < rowGap * 3f && ballSpeed < ballSpeedMin * 3f))
		{
			if (lastKicker != 12)
			{
				kickerPos.X = handle2and6Xvalue + forwardsOffSet;
				kickerPos.Z = rowPositionsZ[2] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[2] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
					playerKick(kickerPos, 2);
					lastKicker = 12;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 13)
			{
				kickerPos.X = handle2and6Xvalue + (pitchXMax - maxHandleXValue) - forwardsOffSet;
				kickerPos.Z = rowPositionsZ[2] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[2] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
					playerKick(kickerPos, 2);
					lastKicker = 13;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		if ((ballPosition.Z > rowGap * 4f && ballPosition.Z < rowGap * 4.5f) || (ballPosition.Z > rowGap * 4f && ballPosition.Z < rowGap * 5f && ballSpeed < ballSpeedMin * 3f))
		{
			if (lastKicker != 14)
			{
				kickerPos.X = handle4and7Xvalue + midfieldOffSet;
				kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
					playerKick(kickerPos, 4);
					lastKicker = 14;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 15)
			{
				kickerPos.X = handle4and7Xvalue + midfieldOffSet + midfieldCentres;
				kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
					playerKick(kickerPos, 4);
					lastKicker = 15;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 16)
			{
				kickerPos.X = handle4and7Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres;
				kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
					playerKick(kickerPos, 4);
					lastKicker = 16;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 17)
			{
				kickerPos.X = handle4and7Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres + midfieldCentres;
				kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[4] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
					playerKick(kickerPos, 4);
					lastKicker = 17;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		if ((ballPosition.Z > rowGap * 6f && ballPosition.Z < rowGap * 6.5f) || (ballPosition.Z > rowGap * 6f && ballPosition.Z < rowGap * 7f && ballSpeed < ballSpeedMin * 3f))
		{
			if (lastKicker != 18)
			{
				kickerPos.X = handle2and6Xvalue + centerbacksOffSet;
				kickerPos.Z = rowPositionsZ[6] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[6] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
					playerKick(kickerPos, 6);
					lastKicker = 18;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 19)
			{
				kickerPos.X = handle2and6Xvalue + (pitchXMax - maxHandleXValue) - centerbacksOffSet;
				kickerPos.Z = rowPositionsZ[6] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[6] + (float)Math.Sin(handle2and6Rotation) * (0f - num);
					playerKick(kickerPos, 6);
					lastKicker = 19;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		if (ballPosition.Z > rowGap * 7f && ballPosition.Z < rowGap * 8f)
		{
			if (lastKicker != 20)
			{
				kickerPos.X = handle4and7Xvalue + goaliesDefendersOffSet;
				kickerPos.Z = rowPositionsZ[7] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[7] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
					playerKick(kickerPos, 7);
					lastKicker = 20;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 21)
			{
				kickerPos.X = handle4and7Xvalue + (pitchXMax - maxHandleXValue) / 2f;
				kickerPos.Z = rowPositionsZ[7] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[7] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
					playerKick(kickerPos, 7);
					lastKicker = 21;
					lastKickerTTL = setLastKickerTTL;
				}
			}
			if (lastKicker != 22)
			{
				kickerPos.X = handle4and7Xvalue + (pitchXMax - maxHandleXValue) - goaliesDefendersOffSet;
				kickerPos.Z = rowPositionsZ[7] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
				float num3 = (float)Math.Sqrt(Math.Pow(kickerPos.X - ballPosition.X, 2.0) + Math.Pow(kickerPos.Z - ballPosition.Z, 2.0));
				if (num3 < ballRadius + playerBaseRadius)
				{
					kickerPos.Z = rowPositionsZ[7] + (float)Math.Sin(handle4and7Rotation) * (0f - num);
					playerKick(kickerPos, 7);
					lastKicker = 22;
					lastKickerTTL = setLastKickerTTL;
				}
			}
		}
		float num4 = 50f;
		if (isPaused)
		{
			num4 = 1f;
		}
		switch (cameraMode)
		{
		case 1:
			cameraPosition.X = -2000f;
			cameraPosition.Y = 5000f;
			cameraPosition.Z = pitchZMax / 2f;
			cameraLookAt.X = ballPosition.X;
			cameraLookAt.Z = ballPosition.Z;
			break;
		case 2:
			if (ballPosition.Z < pitchZMax / 3f)
			{
				desiredCameraPosition.Z = pitchZMax * 0.25f;
				desiredCameraPosition.Y = 3500f;
			}
			if (ballPosition.Z > pitchZMax / 3f && ballPosition.Z < pitchZMax * 2f / 3f)
			{
				desiredCameraPosition.Z = pitchZMax * 0.5f;
				desiredCameraPosition.Y = 5000f;
			}
			if (ballPosition.Z > pitchZMax * 2f / 3f && ballPosition.Z < pitchZMax)
			{
				desiredCameraPosition.Z = pitchZMax * 0.75f;
				desiredCameraPosition.Y = 3500f;
			}
			if (cameraPosition.Z < desiredCameraPosition.Z)
			{
				ref Vector3 reference = ref cameraPosition;
				reference.Z += num4;
			}
			if (cameraPosition.Z > desiredCameraPosition.Z)
			{
				ref Vector3 reference2 = ref cameraPosition;
				reference2.Z += 0f - num4;
			}
			if (cameraPosition.Y < desiredCameraPosition.Y)
			{
				ref Vector3 reference3 = ref cameraPosition;
				reference3.Y += num4;
			}
			if (cameraPosition.Y > desiredCameraPosition.Y)
			{
				ref Vector3 reference4 = ref cameraPosition;
				reference4.Y += 0f - num4;
			}
			cameraPosition.X = -1000f + ballPosition.X / 3f;
			cameraLookAt.X = pitchXMax * 0.15f + 0.5f * ballPosition.X;
			cameraLookAt.Z = ballPosition.Z;
			break;
		case 3:
			if (ballPosition.X > pitchXMax)
			{
				cameraPosition.X = ballPosition.X + (ballPosition.X - pitchXMax / 2f) * 0.75f;
			}
			else
			{
				cameraPosition.X = ballPosition.X - (ballPosition.X - pitchXMax / 2f) * 0.75f;
			}
			cameraPosition.Y = 5000f;
			cameraPosition.Z = pitchZMax + 2000f;
			cameraLookAt.X = ballPosition.X;
			cameraLookAt.Z = ballPosition.Z;
			break;
		}
		((GameComponent)this).Update(gameTime);
		return;
		IL_050a:
		if (pauser != 3)
		{
			goto IL_054f;
		}
		GamePadDPad dPad = ((GamePadState)(ref gp3State)).DPad;
		GamePadThumbSticks thumbSticks;
		if ((int)((GamePadDPad)(ref dPad)).Down != 1)
		{
			thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5))
			{
				goto IL_054f;
			}
		}
		goto IL_05a6;
		IL_0631:
		if (pauser != 2)
		{
			goto IL_067c;
		}
		dPad = ((GamePadState)(ref gp2State)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Up != 1)
		{
			thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.5))
			{
				goto IL_067c;
			}
		}
		goto IL_0718;
		IL_054f:
		if (pauser == 4)
		{
			dPad = ((GamePadState)(ref gp4State)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Down != 1)
			{
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5))
				{
					goto IL_05e6;
				}
			}
			goto IL_05a6;
		}
		goto IL_05e6;
		IL_05e6:
		if (pauser != 1)
		{
			goto IL_0631;
		}
		dPad = ((GamePadState)(ref gp1State)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Up != 1)
		{
			thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.5))
			{
				goto IL_0631;
			}
		}
		goto IL_0718;
		IL_00d7:
		buttons = ((GamePadState)(ref gp1State)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).Y == 0)
		{
			yPressed = false;
		}
		if (gameMode == 9)
		{
			if (!victoryPlayed)
			{
				if (redScore > blueScore)
				{
					redGoalSound.Play();
				}
				if (redScore < blueScore)
				{
					blueGoalSound.Play();
				}
				victoryPlayed = true;
			}
			if (showFinalScoreMin > 0)
			{
				showFinalScoreMin--;
			}
			else
			{
				buttons = ((GamePadState)(ref gp1State)).Buttons;
				if ((int)((GamePadButtons)(ref buttons)).A != 1)
				{
					buttons = ((GamePadState)(ref gp2State)).Buttons;
					if ((int)((GamePadButtons)(ref buttons)).A != 1)
					{
						buttons = ((GamePadState)(ref gp3State)).Buttons;
						if ((int)((GamePadButtons)(ref buttons)).A != 1)
						{
							buttons = ((GamePadState)(ref gp4State)).Buttons;
							if ((int)((GamePadButtons)(ref buttons)).A != 1)
							{
								goto IL_0208;
							}
						}
					}
				}
				gameMode = 1;
			}
		}
		goto IL_0208;
		IL_06c1:
		if (pauser == 4)
		{
			dPad = ((GamePadState)(ref gp4State)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Up != 1)
			{
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.5))
				{
					goto IL_1f08;
				}
			}
			goto IL_0718;
		}
		goto IL_1f08;
		IL_04bf:
		if (pauser != 2)
		{
			goto IL_050a;
		}
		dPad = ((GamePadState)(ref gp2State)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Down != 1)
		{
			thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5))
			{
				goto IL_050a;
			}
		}
		goto IL_05a6;
		IL_0208:
		if (gameMode == 6 || gameMode == 7)
		{
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				pausePressed(1);
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 0)
			{
				pauseReleased(1);
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				pausePressed(2);
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 0)
			{
				pauseReleased(2);
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				pausePressed(3);
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 0)
			{
				pauseReleased(3);
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				pausePressed(4);
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 0)
			{
				pauseReleased(4);
			}
		}
		if (isPaused)
		{
			if (pauser == 1)
			{
				buttons = ((GamePadState)(ref gp1State)).Buttons;
				if ((int)((GamePadButtons)(ref buttons)).A == 1)
				{
					goto IL_040d;
				}
			}
			if (pauser == 2)
			{
				buttons = ((GamePadState)(ref gp2State)).Buttons;
				if ((int)((GamePadButtons)(ref buttons)).A == 1)
				{
					goto IL_040d;
				}
			}
			if (pauser == 3)
			{
				buttons = ((GamePadState)(ref gp3State)).Buttons;
				if ((int)((GamePadButtons)(ref buttons)).A == 1)
				{
					goto IL_040d;
				}
			}
			if (pauser == 4)
			{
				buttons = ((GamePadState)(ref gp4State)).Buttons;
				if ((int)((GamePadButtons)(ref buttons)).A == 1)
				{
					goto IL_040d;
				}
			}
			goto IL_044e;
		}
		if (gameMode == 7)
		{
			clock[0]++;
		}
		if (clock[0] > 59)
		{
			clock[0] = 0;
			clock[1]++;
		}
		if (clock[1] > 9)
		{
			clock[1] = 0;
			clock[2]++;
		}
		if (clock[2] > 5)
		{
			clock[2] = 0;
			clock[3]++;
			if (timeLimitIndex == 0 && clock[3] == 2)
			{
				gameMode = 9;
			}
			if (timeLimitIndex == 1 && clock[3] == 5)
			{
				gameMode = 9;
			}
			if (timeLimitIndex == 4 && clock[3] == 5 && clock[4] == 4)
			{
				gameMode = 9;
			}
		}
		if (clock[3] > 9)
		{
			clock[3] = 0;
			clock[4]++;
			if (timeLimitIndex == 2 && clock[4] == 1)
			{
				gameMode = 9;
			}
			if (timeLimitIndex == 3 && clock[4] == 2)
			{
				gameMode = 9;
			}
			if (timeLimitIndex == 5 && clock[4] == 9)
			{
				gameMode = 9;
			}
		}
		if (gameMode != 6)
		{
			if (stun > 0)
			{
				ballSpeed = ballSpeedStun;
				actualBallMoveSpeed = ballSpeed * speedSettingMuiltiplier;
				ref Vector3 reference5 = ref ballPosition;
				reference5.X += actualBallMoveSpeed * (float)Math.Cos(ballBearing);
				ref Vector3 reference6 = ref ballPosition;
				reference6.Z += actualBallMoveSpeed * (float)Math.Sin(ballBearing);
				stun--;
			}
			else
			{
				actualBallMoveSpeed = ballSpeed * speedSettingMuiltiplier;
				ref Vector3 reference7 = ref ballPosition;
				reference7.X += actualBallMoveSpeed * (float)Math.Cos(ballBearing);
				ref Vector3 reference8 = ref ballPosition;
				reference8.Z += actualBallMoveSpeed * (float)Math.Sin(ballBearing);
				if (ballSpeed > ballSpeedMaxNormal)
				{
					ballSpeed -= 0.1f;
				}
				if (ballSpeed < ballSpeedMin && !redGoal && !blueGoal)
				{
					ballSpeed += 0.5f;
				}
				if (ballSpeed > (ballSpeedMaxNormal - ballSpeedMin) / 2f)
				{
					ballSpeed -= frictionDeceleration;
				}
			}
			if (ballBearing > 0f && ballBearing < 0.9f)
			{
				ballBearing += 0.005f;
			}
			if (ballBearing < (float)Math.PI && ballBearing > 2.241593f)
			{
				ballBearing -= 0.005f;
			}
			if (ballBearing > (float)Math.PI && ballBearing < 4.0415926f)
			{
				ballBearing += 0.005f;
			}
			if (ballBearing < (float)Math.PI * 2f && ballBearing > 5.3831854f)
			{
				ballBearing -= 0.005f;
			}
		}
		if (redController == 5)
		{
			CPU_redEasyPlay();
		}
		if (redController == 6)
		{
			CPU_redNormalPlay();
		}
		if (redController == 7)
		{
			CPU_redHardPlay();
		}
		if (blueController == 5)
		{
			CPU_blueEasyPlay();
		}
		if (blueController == 6)
		{
			CPU_blueNormalPlay();
		}
		if (blueController == 7)
		{
			CPU_blueHardPlay();
		}
		if (redController == 1)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle0and3Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle1and5Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num5 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle0and3Xvalue = num5 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num6 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle1and5Xvalue = num6 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle0and3Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle1and5Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num7 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle0and3Xvalue = num7 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num8 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle1and5Xvalue = num8 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row03stun = true;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row15stun = true;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row03stun = false;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row15stun = false;
			}
		}
		if (blueController == 1)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle2and6Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle4and7Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num9 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle2and6Xvalue = num9 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num10 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle4and7Xvalue = num10 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle2and6Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle4and7Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num11 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle2and6Xvalue = num11 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num12 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
				handle4and7Xvalue = num12 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row26stun = true;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row47stun = true;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row26stun = false;
			}
			buttons = ((GamePadState)(ref gp1State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row47stun = false;
			}
		}
		if (redController == 2)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle0and3Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle1and5Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num13 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle0and3Xvalue = num13 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num14 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle1and5Xvalue = num14 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle0and3Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle1and5Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num15 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle0and3Xvalue = num15 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num16 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle1and5Xvalue = num16 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row03stun = true;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row15stun = true;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row03stun = false;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row15stun = false;
			}
		}
		if (blueController == 2)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle2and6Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle4and7Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num17 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle2and6Xvalue = num17 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num18 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle4and7Xvalue = num18 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle2and6Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle4and7Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num19 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle2and6Xvalue = num19 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num20 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp2State)).ThumbSticks;
				handle4and7Xvalue = num20 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row26stun = true;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row47stun = true;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row26stun = false;
			}
			buttons = ((GamePadState)(ref gp2State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row47stun = false;
			}
		}
		if (redController == 3)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle0and3Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle1and5Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num21 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle0and3Xvalue = num21 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num22 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle1and5Xvalue = num22 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle0and3Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle1and5Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num23 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle0and3Xvalue = num23 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num24 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle1and5Xvalue = num24 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row03stun = true;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row15stun = true;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row03stun = false;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row15stun = false;
			}
		}
		if (blueController == 3)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle2and6Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle4and7Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num25 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle2and6Xvalue = num25 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num26 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle4and7Xvalue = num26 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle2and6Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle4and7Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num27 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle2and6Xvalue = num27 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num28 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
				handle4and7Xvalue = num28 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row26stun = true;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row47stun = true;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row26stun = false;
			}
			buttons = ((GamePadState)(ref gp3State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row47stun = false;
			}
		}
		if (redController == 4)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle0and3Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle1and5Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num29 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle0and3Xvalue = num29 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num30 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle1and5Xvalue = num30 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle0and3Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle1and5Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num31 = handle0and3Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle0and3Xvalue = num31 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num32 = handle1and5Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle1and5Xvalue = num32 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row03stun = true;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row15stun = true;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row03stun = false;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row15stun = false;
			}
		}
		if (blueController == 4)
		{
			if (cameraMode == 3)
			{
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle2and6Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Left.Y;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle4and7Rotation = ((GamePadThumbSticks)(ref thumbSticks)).Right.Y;
				float num33 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle2and6Xvalue = num33 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 50f;
				float num34 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle4and7Xvalue = num34 + ((GamePadThumbSticks)(ref thumbSticks)).Right.X * 50f;
			}
			else
			{
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle2and6Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Left.X;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle4and7Rotation = 0f - ((GamePadThumbSticks)(ref thumbSticks)).Right.X;
				float num35 = handle2and6Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle2and6Xvalue = num35 + ((GamePadThumbSticks)(ref thumbSticks)).Left.Y * 50f;
				float num36 = handle4and7Xvalue;
				thumbSticks = ((GamePadState)(ref gp4State)).ThumbSticks;
				handle4and7Xvalue = num36 + ((GamePadThumbSticks)(ref thumbSticks)).Right.Y * 50f;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 1)
			{
				row26stun = true;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 1)
			{
				row47stun = true;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).LeftShoulder == 0)
			{
				row26stun = false;
			}
			buttons = ((GamePadState)(ref gp4State)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).RightShoulder == 0)
			{
				row47stun = false;
			}
		}
		if (redStickStyle == 1)
		{
			handle1and5Rotation = handle0and3Rotation;
			handle1and5Xvalue = handle0and3Xvalue;
		}
		if (redStickStyle == 2)
		{
			handle0and3Rotation = handle1and5Rotation;
			handle0and3Xvalue = handle1and5Xvalue;
		}
		if (blueStickStyle == 2)
		{
			handle2and6Rotation = handle4and7Rotation;
			handle2and6Xvalue = handle4and7Xvalue;
		}
		if (blueStickStyle == 1)
		{
			handle4and7Rotation = handle2and6Rotation;
			handle4and7Xvalue = handle2and6Xvalue;
		}
		handle0and3rotTracker[framesToTrackItterator] = handle0and3Rotation;
		handle1and5rotTracker[framesToTrackItterator] = handle1and5Rotation;
		handle2and6rotTracker[framesToTrackItterator] = handle2and6Rotation;
		handle4and7rotTracker[framesToTrackItterator] = handle4and7Rotation;
		handle0and3valTracker[framesToTrackItterator] = handle0and3Xvalue;
		handle1and5valTracker[framesToTrackItterator] = handle1and5Xvalue;
		handle2and6valTracker[framesToTrackItterator] = handle2and6Xvalue;
		handle4and7valTracker[framesToTrackItterator] = handle4and7Xvalue;
		framesToTrackItterator++;
		if (framesToTrackItterator >= framesToTrack)
		{
			framesToTrackItterator = 0;
		}
		if (handle0and3Xvalue < 0f)
		{
			handle0and3Xvalue = 0f;
		}
		if (handle0and3Xvalue > maxHandleXValue)
		{
			handle0and3Xvalue = maxHandleXValue;
		}
		if (handle1and5Xvalue < 0f)
		{
			handle1and5Xvalue = 0f;
		}
		if (handle1and5Xvalue > maxHandleXValue)
		{
			handle1and5Xvalue = maxHandleXValue;
		}
		if (handle2and6Xvalue < 0f)
		{
			handle2and6Xvalue = 0f;
		}
		if (handle2and6Xvalue > maxHandleXValue)
		{
			handle2and6Xvalue = maxHandleXValue;
		}
		if (handle4and7Xvalue < 0f)
		{
			handle4and7Xvalue = 0f;
		}
		if (handle4and7Xvalue > maxHandleXValue)
		{
			handle4and7Xvalue = maxHandleXValue;
		}
		goto IL_1f08;
		IL_067c:
		if (pauser != 3)
		{
			goto IL_06c1;
		}
		dPad = ((GamePadState)(ref gp3State)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Up != 1)
		{
			thumbSticks = ((GamePadState)(ref gp3State)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.5))
			{
				goto IL_06c1;
			}
		}
		goto IL_0718;
		IL_040d:
		if (pausePointerOnResume)
		{
			isPaused = false;
			isPausedLock = false;
		}
		else
		{
			isPaused = false;
			isPausedLock = false;
			pausePointerOnResume = true;
			gameMode = 1;
		}
		goto IL_044e;
		IL_05a6:
		pointerLag = setPointerLag;
		if (pausePointerOnResume)
		{
			pausePointerOnResume = false;
			kickSoft.Play();
		}
		else
		{
			wallBounce.Play();
		}
		goto IL_05e6;
		IL_044e:
		if (pointerLag > 0)
		{
			pointerLag--;
			goto IL_1f08;
		}
		if (pauser != 1)
		{
			goto IL_04bf;
		}
		dPad = ((GamePadState)(ref gp1State)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Down != 1)
		{
			thumbSticks = ((GamePadState)(ref gp1State)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5))
			{
				goto IL_04bf;
			}
		}
		goto IL_05a6;
	}

	private void playerKick(Vector3 playerPosition, int row)
	{
		float num = -10f;
		if (row == 2 || row == 4 || row == 6 || row == 7)
		{
			num = 0f - num;
		}
		ballBearing = (float)Math.Atan((playerPosition.Z + num - ballPosition.Z) / (playerPosition.X - ballPosition.X));
		if (playerPosition.X > ballPosition.X)
		{
			ballBearing = (float)Math.PI + ballBearing;
		}
		for (int i = 0; i < 100; i++)
		{
			float num2 = (float)Math.Sqrt(Math.Pow(playerPosition.X - ballPosition.X, 2.0) + Math.Pow(playerPosition.Z - ballPosition.Z, 2.0));
			if (num2 < ballRadius + playerBaseRadius)
			{
				ref Vector3 reference = ref ballPosition;
				reference.X += ballSpeedMin * (float)Math.Cos(ballBearing);
				ref Vector3 reference2 = ref ballPosition;
				reference2.Z += ballSpeedMin * (float)Math.Sin(ballBearing);
				continue;
			}
			break;
		}
		float num3 = 1f;
		float num4 = -1f;
		float num5 = maxHandleXValue;
		float num6 = 0f;
		if (row == 0 || row == 3)
		{
			if (row03stun)
			{
				ballSpeed = ballSpeedStun;
				stun = stunTime;
			}
			else
			{
				stun = 0;
				for (int i = 0; i < framesToTrack; i++)
				{
					if (handle0and3rotTracker[i] < num3)
					{
						num3 = handle0and3rotTracker[i];
					}
					if (handle0and3rotTracker[i] > num4)
					{
						num4 = handle0and3rotTracker[i];
					}
					if (handle0and3valTracker[i] < num5)
					{
						num5 = handle0and3valTracker[i];
					}
					if (handle0and3valTracker[i] > num6)
					{
						num6 = handle0and3valTracker[i];
					}
				}
			}
		}
		if (row == 1 || row == 5)
		{
			if (row15stun)
			{
				ballSpeed = ballSpeedStun;
				stun = stunTime;
			}
			else
			{
				stun = 0;
				for (int i = 0; i < framesToTrack; i++)
				{
					if (handle1and5rotTracker[i] < num3)
					{
						num3 = handle1and5rotTracker[i];
					}
					if (handle1and5rotTracker[i] > num4)
					{
						num4 = handle1and5rotTracker[i];
					}
					if (handle1and5valTracker[i] < num5)
					{
						num5 = handle1and5valTracker[i];
					}
					if (handle1and5valTracker[i] > num6)
					{
						num6 = handle1and5valTracker[i];
					}
				}
			}
		}
		if (row == 2 || row == 6)
		{
			if (row26stun)
			{
				ballSpeed = ballSpeedStun;
				stun = stunTime;
			}
			else
			{
				stun = 0;
				for (int i = 0; i < framesToTrack; i++)
				{
					if (handle2and6rotTracker[i] < num3)
					{
						num3 = handle2and6rotTracker[i];
					}
					if (handle2and6rotTracker[i] > num4)
					{
						num4 = handle2and6rotTracker[i];
					}
					if (handle2and6valTracker[i] < num5)
					{
						num5 = handle2and6valTracker[i];
					}
					if (handle2and6valTracker[i] > num6)
					{
						num6 = handle2and6valTracker[i];
					}
				}
			}
		}
		if (row == 4 || row == 7)
		{
			if (row47stun)
			{
				ballSpeed = ballSpeedStun;
				stun = stunTime;
			}
			else
			{
				stun = 0;
				for (int i = 0; i < framesToTrack; i++)
				{
					if (handle4and7rotTracker[i] < num3)
					{
						num3 = handle4and7rotTracker[i];
					}
					if (handle4and7rotTracker[i] > num4)
					{
						num4 = handle4and7rotTracker[i];
					}
					if (handle4and7valTracker[i] < num5)
					{
						num5 = handle4and7valTracker[i];
					}
					if (handle4and7valTracker[i] > num6)
					{
						num6 = handle4and7valTracker[i];
					}
				}
			}
		}
		float num7 = (num4 - num3) / 2f * (float)Math.Cos((double)ballBearing + Math.PI / 2.0);
		float num8 = (num6 - num5) / 500f * (float)Math.Sin((double)ballBearing + Math.PI / 2.0);
		if (num7 < 0f)
		{
			num7 = 0f - num7;
		}
		if (num8 < 0f)
		{
			num8 = 0f - num8;
		}
		float num9 = num7 + num8;
		ballSpeed += num9 * ballSpeedMaxNormal - impactDeceleration;
		if (ballSpeed < ballSpeedMin)
		{
			ballSpeed = ballSpeedMin;
		}
		debugString = "Ang comp   :  " + num7 + "\nVal comp   :  " + num8 + "\nKick Speed :  " + num9;
		if (ballSpeed > ballSpeedMaxNormal)
		{
			kickHard.Play();
		}
		else if (ballSpeed > ballSpeedMaxNormal / 3f)
		{
			kickMed.Play();
		}
		else
		{
			kickSoft.Play();
		}
	}

	public void DrawTable()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
		projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
		drawBall();
		drawPitch();
		drawHandles();
		drawPlayers();
		if (isPaused)
		{
			drawPause();
		}
		Rectangle titleSafeArea = GetTitleSafeArea(0.95f);
		Rectangle val = default(Rectangle);
		((Rectangle)(ref val))._002Ector(((Rectangle)(ref titleSafeArea)).Left, ((Rectangle)(ref titleSafeArea)).Top, 350, 130);
		string text = " Red Team :  " + redScore + "\n Blue Team :  " + blueScore;
		string text2 = " " + clock[4] + clock[3] + " : " + clock[2] + clock[1];
		int num = 160;
		int num2 = 80;
		Rectangle val2 = default(Rectangle);
		((Rectangle)(ref val2))._002Ector(((Rectangle)(ref titleSafeArea)).Right - num, ((Rectangle)(ref titleSafeArea)).Top, num, num2);
		spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
		spriteBatch.Draw(scoreboardT, val, Color.White);
		spriteBatch.DrawString(Font1, text, new Vector2((float)((Rectangle)(ref titleSafeArea)).Left + 20f, (float)((Rectangle)(ref titleSafeArea)).Top + 15f), Color.Black);
		spriteBatch.Draw(clockT, val2, Color.White);
		spriteBatch.DrawString(Font1, text2, new Vector2((float)((Rectangle)(ref val2)).Right - 140f, (float)((Rectangle)(ref val2)).Top + 15f), Color.Black);
		spriteBatch.End();
	}

	public void drawPause()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		Rectangle titleSafeArea = GetTitleSafeArea(1f);
		int num = (int)((double)titleSafeArea.Height * 0.05575158786167961);
		int num2 = (int)((double)num * 1.4050632911392404);
		int num3 = titleSafeArea.Width / 3;
		int num4 = (int)((double)titleSafeArea.Height * 0.61);
		if (pausePointerOnResume)
		{
			num4 = (int)((double)titleSafeArea.Height * 0.45);
		}
		spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
		SpriteBatch obj = spriteBatch;
		Texture2D obj2 = blankTexture;
		Viewport viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		obj.Draw(obj2, new Rectangle(0, 0, width, ((Viewport)(ref viewport)).Height), new Color((byte)0, (byte)0, (byte)0, (byte)100));
		spriteBatch.Draw(pauseMenu, new Rectangle(titleSafeArea.Width / 2 - titleSafeArea.Height / 2, 0, titleSafeArea.Height, titleSafeArea.Height), Color.White);
		spriteBatch.Draw(pointerT, new Rectangle(num3, num4, num, num2), Color.White);
		spriteBatch.End();
	}

	public unsafe void drawTestMarker()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		Matrix[] array = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)testMarker.Bones).Count];
		testMarker.CopyAbsoluteBoneTransformsTo(array);
		Enumerator enumerator = testMarker.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(markerPos);
						val.View = viewMatrix;
						val.Projection = projectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public unsafe void drawBall()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		Matrix[] array = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)ball.Bones).Count];
		ballRotX = ballPosition.Z / (ballRadius * 2f * (float)Math.PI) * ((float)Math.PI * 2f);
		ballRotZ = ballPosition.X / (ballRadius * 2f * (float)Math.PI) * ((float)Math.PI * 2f);
		ballSpin += ballSpeed / ballSpeedMaxNormal / 10f;
		if ((double)ballSpin > 6.2831854820251465)
		{
			ballSpin = 0f;
		}
		ball.CopyAbsoluteBoneTransformsTo(array);
		Enumerator enumerator = ball.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationZ((0f - ballRotZ) / 2f) * Matrix.CreateRotationX(ballRotX / 2f) * Matrix.CreateTranslation(ballPosition);
						val.View = viewMatrix;
						val.Projection = projectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public unsafe void drawPitch()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Expected O, but got Unknown
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Expected O, but got Unknown
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		Matrix[] array = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)post.Bones).Count];
		pitch.CopyAbsoluteBoneTransformsTo(array);
		Enumerator enumerator = pitch.Meshes.GetEnumerator();
		Enumerator enumerator2;
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(modelPosition);
						val.View = viewMatrix;
						val.Projection = projectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		post.CopyAbsoluteBoneTransformsTo(array);
		enumerator = post.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(postFromCorner, 0f, 0f));
						val.View = viewMatrix;
						val.Projection = projectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(pitchXMax - postFromCorner + 70f, 0f, 0f));
						val.View = viewMatrix;
						val.Projection = projectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(postFromCorner, 0f, pitchZMax));
						val.View = viewMatrix;
						val.Projection = projectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(pitchXMax - postFromCorner + 70f, 0f, pitchZMax));
						val.View = viewMatrix;
						val.Projection = projectionMatrix;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		pipe.CopyAbsoluteBoneTransformsTo(array);
		for (int i = 0; i < rowPositionsZ.Length; i++)
		{
			enumerator = pipe.Meshes.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator)).MoveNext())
				{
					ModelMesh current = ((Enumerator)(ref enumerator)).Current;
					enumerator2 = current.Effects.GetEnumerator();
					try
					{
						while (((Enumerator)(ref enumerator2)).MoveNext())
						{
							BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
							val.EnableDefaultLighting();
							val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(0f, 0f, rowPositionsZ[i]));
							val.View = viewMatrix;
							val.Projection = projectionMatrix;
						}
					}
					finally
					{
						((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
					}
					current.Draw();
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	public unsafe void drawHandles()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Expected O, but got Unknown
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Expected O, but got Unknown
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Expected O, but got Unknown
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Expected O, but got Unknown
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0729: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Expected O, but got Unknown
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0874: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Expected O, but got Unknown
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		Matrix[] array = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)redHandle.Bones).Count];
		redHandle.CopyAbsoluteBoneTransformsTo(array);
		Enumerator enumerator = redHandle.Meshes.GetEnumerator();
		Enumerator enumerator2;
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue, 0f, rowPositionsZ[0]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redHandle.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue, 0f, rowPositionsZ[3]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redHandle.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle1and5Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle1and5Xvalue, 0f, rowPositionsZ[1]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redHandle.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle1and5Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle1and5Xvalue, 0f, rowPositionsZ[5]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		blueHandle.CopyAbsoluteBoneTransformsTo(array);
		enumerator = blueHandle.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle2and6Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle2and6Xvalue, 0f, rowPositionsZ[2]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		blueHandle.CopyAbsoluteBoneTransformsTo(array);
		enumerator = blueHandle.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle2and6Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle2and6Xvalue, 0f, rowPositionsZ[6]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		blueHandle.CopyAbsoluteBoneTransformsTo(array);
		enumerator = blueHandle.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue, 0f, rowPositionsZ[4]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		blueHandle.CopyAbsoluteBoneTransformsTo(array);
		enumerator = blueHandle.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue, 0f, rowPositionsZ[7]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public unsafe void drawPlayers()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Expected O, but got Unknown
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Expected O, but got Unknown
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Expected O, but got Unknown
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Expected O, but got Unknown
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_090e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_0918: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09db: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b28: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f4: Expected O, but got Unknown
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b57: Expected O, but got Unknown
		//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc1: Expected O, but got Unknown
		//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e16: Expected O, but got Unknown
		//IL_0e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f86: Expected O, but got Unknown
		//IL_0fa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1002: Unknown result type (might be due to invalid IL or missing references)
		//IL_1007: Unknown result type (might be due to invalid IL or missing references)
		//IL_100c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1019: Unknown result type (might be due to invalid IL or missing references)
		//IL_101f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1024: Unknown result type (might be due to invalid IL or missing references)
		//IL_1029: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1209: Unknown result type (might be due to invalid IL or missing references)
		//IL_120e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e8: Expected O, but got Unknown
		//IL_1102: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_110d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1112: Unknown result type (might be due to invalid IL or missing references)
		//IL_1123: Unknown result type (might be due to invalid IL or missing references)
		//IL_1128: Unknown result type (might be due to invalid IL or missing references)
		//IL_112e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1133: Unknown result type (might be due to invalid IL or missing references)
		//IL_1138: Unknown result type (might be due to invalid IL or missing references)
		//IL_1157: Unknown result type (might be due to invalid IL or missing references)
		//IL_115c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1161: Unknown result type (might be due to invalid IL or missing references)
		//IL_116e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1174: Unknown result type (might be due to invalid IL or missing references)
		//IL_1179: Unknown result type (might be due to invalid IL or missing references)
		//IL_117e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1224: Unknown result type (might be due to invalid IL or missing references)
		//IL_1229: Unknown result type (might be due to invalid IL or missing references)
		//IL_136c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1371: Unknown result type (might be due to invalid IL or missing references)
		//IL_1237: Unknown result type (might be due to invalid IL or missing references)
		//IL_123d: Expected O, but got Unknown
		//IL_1257: Unknown result type (might be due to invalid IL or missing references)
		//IL_125d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1262: Unknown result type (might be due to invalid IL or missing references)
		//IL_1267: Unknown result type (might be due to invalid IL or missing references)
		//IL_1278: Unknown result type (might be due to invalid IL or missing references)
		//IL_127d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1283: Unknown result type (might be due to invalid IL or missing references)
		//IL_1288: Unknown result type (might be due to invalid IL or missing references)
		//IL_128d: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1308: Unknown result type (might be due to invalid IL or missing references)
		//IL_1387: Unknown result type (might be due to invalid IL or missing references)
		//IL_138c: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_139a: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a0: Expected O, but got Unknown
		//IL_13ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_13db: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_140f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1414: Unknown result type (might be due to invalid IL or missing references)
		//IL_1419: Unknown result type (might be due to invalid IL or missing references)
		//IL_1426: Unknown result type (might be due to invalid IL or missing references)
		//IL_142c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1431: Unknown result type (might be due to invalid IL or missing references)
		//IL_1436: Unknown result type (might be due to invalid IL or missing references)
		//IL_145d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1624: Unknown result type (might be due to invalid IL or missing references)
		//IL_1629: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f5: Expected O, but got Unknown
		//IL_150f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1515: Unknown result type (might be due to invalid IL or missing references)
		//IL_151a: Unknown result type (might be due to invalid IL or missing references)
		//IL_151f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1530: Unknown result type (might be due to invalid IL or missing references)
		//IL_1535: Unknown result type (might be due to invalid IL or missing references)
		//IL_153b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1540: Unknown result type (might be due to invalid IL or missing references)
		//IL_1545: Unknown result type (might be due to invalid IL or missing references)
		//IL_1572: Unknown result type (might be due to invalid IL or missing references)
		//IL_1577: Unknown result type (might be due to invalid IL or missing references)
		//IL_157c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1589: Unknown result type (might be due to invalid IL or missing references)
		//IL_158f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1594: Unknown result type (might be due to invalid IL or missing references)
		//IL_1599: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_163f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1644: Unknown result type (might be due to invalid IL or missing references)
		//IL_1779: Unknown result type (might be due to invalid IL or missing references)
		//IL_177e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1652: Unknown result type (might be due to invalid IL or missing references)
		//IL_1658: Expected O, but got Unknown
		//IL_1672: Unknown result type (might be due to invalid IL or missing references)
		//IL_1678: Unknown result type (might be due to invalid IL or missing references)
		//IL_167d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1682: Unknown result type (might be due to invalid IL or missing references)
		//IL_1693: Unknown result type (might be due to invalid IL or missing references)
		//IL_1698: Unknown result type (might be due to invalid IL or missing references)
		//IL_169e: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_16cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16de: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1715: Unknown result type (might be due to invalid IL or missing references)
		//IL_1794: Unknown result type (might be due to invalid IL or missing references)
		//IL_1799: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_18da: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ad: Expected O, but got Unknown
		//IL_17c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1823: Unknown result type (might be due to invalid IL or missing references)
		//IL_1828: Unknown result type (might be due to invalid IL or missing references)
		//IL_182d: Unknown result type (might be due to invalid IL or missing references)
		//IL_183a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1840: Unknown result type (might be due to invalid IL or missing references)
		//IL_1845: Unknown result type (might be due to invalid IL or missing references)
		//IL_184a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1871: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1903: Unknown result type (might be due to invalid IL or missing references)
		//IL_1909: Expected O, but got Unknown
		//IL_1923: Unknown result type (might be due to invalid IL or missing references)
		//IL_1929: Unknown result type (might be due to invalid IL or missing references)
		//IL_192e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1933: Unknown result type (might be due to invalid IL or missing references)
		//IL_1944: Unknown result type (might be due to invalid IL or missing references)
		//IL_1949: Unknown result type (might be due to invalid IL or missing references)
		//IL_194f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1954: Unknown result type (might be due to invalid IL or missing references)
		//IL_1959: Unknown result type (might be due to invalid IL or missing references)
		//IL_1986: Unknown result type (might be due to invalid IL or missing references)
		//IL_198b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1990: Unknown result type (might be due to invalid IL or missing references)
		//IL_199d: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6c: Expected O, but got Unknown
		//IL_1a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd6: Expected O, but got Unknown
		//IL_1bf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c67: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d25: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d2b: Expected O, but got Unknown
		//IL_1d45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d55: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d66: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d71: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dca: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df6: Unknown result type (might be due to invalid IL or missing references)
		Matrix[] array = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)redGoalie.Bones).Count];
		redGoalie.CopyAbsoluteBoneTransformsTo(array);
		Enumerator enumerator = redGoalie.Meshes.GetEnumerator();
		Enumerator enumerator2;
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue + (pitchXMax - maxHandleXValue) / 2f, 0f, rowPositionsZ[0]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		redGuy.CopyAbsoluteBoneTransformsTo(array);
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue + goaliesDefendersOffSet, 0f, rowPositionsZ[0]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue + (pitchXMax - maxHandleXValue) - goaliesDefendersOffSet, 0f, rowPositionsZ[0]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle1and5Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle1and5Xvalue + centerbacksOffSet, 0f, rowPositionsZ[1]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle1and5Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle1and5Xvalue + (pitchXMax - maxHandleXValue) - centerbacksOffSet, 0f, rowPositionsZ[1]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue + midfieldOffSet, 0f, rowPositionsZ[3]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue + midfieldOffSet + midfieldCentres, 0f, rowPositionsZ[3]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres, 0f, rowPositionsZ[3]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle0and3Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle0and3Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres + midfieldCentres, 0f, rowPositionsZ[3]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle1and5Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle1and5Xvalue + forwardsOffSet, 0f, rowPositionsZ[5]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = redGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle1and5Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle1and5Xvalue + (pitchXMax - maxHandleXValue) - forwardsOffSet, 0f, rowPositionsZ[5]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		blueGuy.CopyAbsoluteBoneTransformsTo(array);
		enumerator = blueGoalie.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue + (pitchXMax - maxHandleXValue) / 2f, 0f, rowPositionsZ[7]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue + goaliesDefendersOffSet, 0f, rowPositionsZ[7]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue + (pitchXMax - maxHandleXValue) - goaliesDefendersOffSet, 0f, rowPositionsZ[7]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle2and6Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle2and6Xvalue + centerbacksOffSet, 0f, rowPositionsZ[6]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle2and6Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle2and6Xvalue + (pitchXMax - maxHandleXValue) - centerbacksOffSet, 0f, rowPositionsZ[6]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue + midfieldOffSet, 0f, rowPositionsZ[4]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue + midfieldOffSet + midfieldCentres, 0f, rowPositionsZ[4]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres, 0f, rowPositionsZ[4]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle4and7Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle4and7Xvalue + midfieldOffSet + midfieldCentres + midfieldCentres + midfieldCentres, 0f, rowPositionsZ[4]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle2and6Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle2and6Xvalue + forwardsOffSet, 0f, rowPositionsZ[2]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		enumerator = blueGuy.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				enumerator2 = current.Effects.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						BasicEffect val = (BasicEffect)((Enumerator)(ref enumerator2)).Current;
						val.EnableDefaultLighting();
						val.World = array[current.ParentBone.Index] * Matrix.CreateTranslation(playerTrans) * Matrix.CreateRotationX(handle2and6Rotation * 0.8f) * Matrix.CreateTranslation(playerTrans2) * Matrix.CreateTranslation(new Vector3(handle2and6Xvalue + (pitchXMax - maxHandleXValue) - forwardsOffSet, 0f, rowPositionsZ[2]));
						val.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
						val.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 1f, farPlaneDist);
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
				current.Draw();
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	protected Rectangle GetTitleSafeArea(float percent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		int x = ((Viewport)(ref viewport)).X;
		viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		int y = ((Viewport)(ref viewport)).Y;
		viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		Rectangle result = default(Rectangle);
		((Rectangle)(ref result))._002Ector(x, y, width, ((Viewport)(ref viewport)).Height);
		float num = (1f - percent) / 2f;
		result.X = (int)(num * (float)result.Width);
		result.Y = (int)(num * (float)result.Height);
		result.Width = (int)(percent * (float)result.Width);
		result.Height = (int)(percent * (float)result.Height);
		return result;
	}

	public void resetTable()
	{
		redScore = 0;
		blueScore = 0;
		clock[0] = 0;
		clock[1] = 0;
		clock[2] = 0;
		clock[3] = 0;
		clock[4] = 0;
		clock[5] = 0;
		showFinalScoreMin = 30;
		victoryPlayed = false;
		ballPosition.X = pitchXMax / 2f;
		ballPosition.Z = pitchZMax / 2f;
		ballBearing = (float)CPU_Random.Next(-10, 10) / 12f + (float)Math.PI / 2f;
		if (CPU_Random.Next(0, 2) < 1)
		{
			ballBearing += (float)Math.PI;
		}
		cameraMode = 2;
		ballSpeed = ballSpeedStun;
		stun = stunTime * 2;
	}

	private void loadSounds()
	{
		tabeBallSong1 = ((GameComponent)this).Game.Content.Load<SoundEffect>("sounds\\tabeBallSong");
		kickSoft = ((GameComponent)this).Game.Content.Load<SoundEffect>("sounds\\popKickSoft");
		kickMed = ((GameComponent)this).Game.Content.Load<SoundEffect>("sounds\\popKickMed");
		kickHard = ((GameComponent)this).Game.Content.Load<SoundEffect>("sounds\\popKickHard");
		wallBounce = ((GameComponent)this).Game.Content.Load<SoundEffect>("sounds\\popWallBounce");
		redGoalSound = ((GameComponent)this).Game.Content.Load<SoundEffect>("sounds\\redGoal");
		blueGoalSound = ((GameComponent)this).Game.Content.Load<SoundEffect>("sounds\\blueGoal");
		pauseMusic = tabeBallSong1.Play(0.5f, 0f, 0f, true);
		pauseMusic.Pause();
	}

	private void CPU_redEasyPlay()
	{
		if (CPU_RED_timeToDecision < 1)
		{
			CPU_RED_test1 = !CPU_RED_test1;
			CPU_RED_timeToDecision = CPU_Random.Next(CPU_decisionMin, CPU_decisionMax);
			CPU_RED_speedRandomizer = (float)CPU_Random.Next(50, 199) / 100f;
		}
		else
		{
			CPU_RED_timeToDecision--;
			if (CPU_RED_test1)
			{
				handle0and3Xvalue += CPU_moveSpeed * CPU_RED_speedRandomizer;
			}
			else
			{
				handle0and3Xvalue -= CPU_moveSpeed * CPU_RED_speedRandomizer;
			}
		}
		if (CPU_RED_timeToDecision2 < 1)
		{
			CPU_RED_test2 = !CPU_RED_test2;
			CPU_RED_timeToDecision2 = CPU_Random.Next(CPU_decisionMin, CPU_decisionMax);
			CPU_RED_speedRandomizer2 = (float)CPU_Random.Next(50, 199) / 100f;
		}
		else
		{
			CPU_RED_timeToDecision2--;
			if (CPU_RED_test2)
			{
				handle1and5Xvalue += CPU_moveSpeed * CPU_RED_speedRandomizer2;
			}
			else
			{
				handle1and5Xvalue -= CPU_moveSpeed * CPU_RED_speedRandomizer2;
			}
		}
		if (!CPU_RED_rollback)
		{
			if ((ballPosition.Z > rowPositionsZ[1] && ballPosition.Z < rowPositionsZ[1] + ballRadius * 3f) || (ballPosition.Z > rowPositionsZ[5] && ballPosition.Z < rowPositionsZ[5] + ballRadius * 3f))
			{
				handle1and5Rotation -= CPU_easyShootSpeed;
				if (handle1and5Rotation < -1f)
				{
					handle1and5Rotation = -1f;
					CPU_RED_rollback = true;
				}
			}
			else if ((ballPosition.Z > rowPositionsZ[1] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[1] + ballRadius * 5f) || (ballPosition.Z > rowPositionsZ[5] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[5] + ballRadius * 5f))
			{
				handle1and5Rotation += CPU_easyShootSpeed / 2f;
				if (handle1and5Rotation > 1f)
				{
					handle1and5Rotation = 1f;
				}
			}
			else
			{
				handle1and5Rotation = 0f;
			}
			if ((ballPosition.Z > rowPositionsZ[0] && ballPosition.Z < rowPositionsZ[0] + ballRadius * 3f) || (ballPosition.Z > rowPositionsZ[3] && ballPosition.Z < rowPositionsZ[3] + ballRadius * 3f))
			{
				handle0and3Rotation -= CPU_easyShootSpeed;
				if (handle0and3Rotation < -1f)
				{
					handle0and3Rotation = -1f;
					CPU_RED_rollback = true;
				}
			}
			else if ((ballPosition.Z > rowPositionsZ[0] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[0] + ballRadius * 5f) || (ballPosition.Z > rowPositionsZ[3] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[3] + ballRadius * 5f))
			{
				handle0and3Rotation += CPU_easyShootSpeed / 2f;
				if (handle0and3Rotation > 1f)
				{
					handle0and3Rotation = 1f;
				}
			}
			else
			{
				handle0and3Rotation = 0f;
			}
		}
		else
		{
			handle0and3Rotation += 0.05f;
			handle1and5Rotation += 0.05f;
			if (handle0and3Rotation > 0f)
			{
				handle0and3Rotation = 0f;
			}
			if (handle1and5Rotation > 0f)
			{
				handle1and5Rotation = 0f;
			}
			if (handle0and3Rotation == 0f && handle1and5Rotation == 0f)
			{
				CPU_RED_rollback = false;
			}
		}
	}

	private void CPU_blueEasyPlay()
	{
		if (CPU_BLUE_timeToDecision < 1)
		{
			CPU_BLUE_test1 = !CPU_BLUE_test1;
			CPU_BLUE_timeToDecision = CPU_Random.Next(CPU_decisionMin, CPU_decisionMax);
			CPU_BLUE_speedRandomizer = (float)CPU_Random.Next(50, 199) / 100f;
		}
		else
		{
			CPU_BLUE_timeToDecision--;
			if (CPU_BLUE_test1)
			{
				handle2and6Xvalue += CPU_moveSpeed * CPU_BLUE_speedRandomizer;
			}
			else
			{
				handle2and6Xvalue -= CPU_moveSpeed * CPU_BLUE_speedRandomizer;
			}
		}
		if (CPU_BLUE_timeToDecision2 < 1)
		{
			CPU_BLUE_test2 = !CPU_BLUE_test2;
			CPU_BLUE_timeToDecision2 = CPU_Random.Next(CPU_decisionMin, CPU_decisionMax);
			CPU_BLUE_speedRandomizer2 = (float)CPU_Random.Next(50, 199) / 100f;
		}
		else
		{
			CPU_BLUE_timeToDecision2--;
			if (CPU_BLUE_test2)
			{
				handle4and7Xvalue += CPU_moveSpeed * CPU_BLUE_speedRandomizer2;
			}
			else
			{
				handle4and7Xvalue -= CPU_moveSpeed * CPU_BLUE_speedRandomizer2;
			}
		}
		if (!CPU_BLUE_rollback)
		{
			if ((ballPosition.Z < rowPositionsZ[4] && ballPosition.Z > rowPositionsZ[4] - ballRadius * 3f) || (ballPosition.Z < rowPositionsZ[7] && ballPosition.Z > rowPositionsZ[7] - ballRadius * 3f))
			{
				handle4and7Rotation += CPU_easyShootSpeed;
				if (handle4and7Rotation > 1f)
				{
					handle4and7Rotation = 1f;
					CPU_BLUE_rollback = true;
				}
			}
			else if ((ballPosition.Z < rowPositionsZ[4] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[4] - ballRadius * 5f) || (ballPosition.Z < rowPositionsZ[7] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[7] - ballRadius * 5f))
			{
				handle4and7Rotation -= CPU_easyShootSpeed / 2f;
				if (handle4and7Rotation < -1f)
				{
					handle4and7Rotation = -1f;
				}
			}
			else
			{
				handle4and7Rotation = 0f;
			}
			if ((ballPosition.Z < rowPositionsZ[2] && ballPosition.Z > rowPositionsZ[2] - ballRadius * 3f) || (ballPosition.Z < rowPositionsZ[6] && ballPosition.Z > rowPositionsZ[6] - ballRadius * 3f))
			{
				handle2and6Rotation += CPU_easyShootSpeed;
				if (handle2and6Rotation > 1f)
				{
					handle2and6Rotation = 1f;
					CPU_BLUE_rollback = true;
				}
			}
			else if ((ballPosition.Z < rowPositionsZ[2] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[2] - ballRadius * 5f) || (ballPosition.Z < rowPositionsZ[6] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[6] - ballRadius * 5f))
			{
				handle2and6Rotation -= CPU_easyShootSpeed / 2f;
				if (handle2and6Rotation < -1f)
				{
					handle2and6Rotation = -1f;
				}
			}
			else
			{
				handle2and6Rotation = 0f;
			}
		}
		else
		{
			handle4and7Rotation -= 0.05f;
			handle2and6Rotation -= 0.05f;
			if (handle4and7Rotation < 0f)
			{
				handle4and7Rotation = 0f;
			}
			if (handle2and6Rotation < 0f)
			{
				handle2and6Rotation = 0f;
			}
			if (handle2and6Rotation == 0f && handle4and7Rotation == 0f)
			{
				CPU_BLUE_rollback = false;
			}
		}
	}

	private void CPU_redNormalPlay()
	{
		if (ttl_redCockup2 < 1)
		{
			ttl_redCockup2 = CPU_Random.Next(10, 120);
			CPU_redCockup2 = CPU_Random.Next(10, 790);
			if (CPU_redCockup2 > 390f)
			{
				CPU_redCockup2 -= 400f;
			}
			CPU_redCockup2 *= 1.5f;
		}
		else
		{
			ttl_redCockup2--;
		}
		float num = 0f;
		num = ((ballPosition.Z > rowPositionsZ[5]) ? ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - forwardsOffSet) + (float)ttl_redCockup2 / 2f - 100f) : (ballPosition.X - maxHandleXValue + (forwardsOffSet - maxHandleXValue) + (float)ttl_redCockup2 / 2f + 100f)) : ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - centerbacksOffSet) + CPU_redCockup2) : (ballPosition.X - maxHandleXValue + (centerbacksOffSet - maxHandleXValue) + CPU_redCockup2)));
		if (ballPosition.Z < rowPositionsZ[5] + 100f && ballPosition.Z > pitchZMax / 2f && ballBearing < (float)Math.PI)
		{
			num = ballPosition.X - maxHandleXValue;
		}
		if (ballPosition.Z < rowPositionsZ[1])
		{
			num = ballPosition.X - maxHandleXValue;
		}
		if (handle1and5Xvalue < num)
		{
			handle1and5Xvalue += CPU_moveSpeed * 2f;
			if (handle1and5Xvalue > num)
			{
				handle1and5Xvalue = num;
			}
		}
		else
		{
			handle1and5Xvalue -= CPU_moveSpeed * 2f;
			if (handle1and5Xvalue < num)
			{
				handle1and5Xvalue = num;
			}
		}
		if (CPU_RED_timeToDecision < 1)
		{
			CPU_RED_timeToDecision = CPU_Random.Next(CPU_decisionMin, CPU_decisionMax) / 2;
			CPU_move03 = (float)CPU_Random.Next(50, 100) / 50f * CPU_moveSpeed;
			if (ballPosition.X < handle0and3Xvalue + (pitchXMax - maxHandleXValue) / 2f)
			{
				CPU_move03 = 0f - CPU_move03;
			}
		}
		else
		{
			CPU_RED_timeToDecision--;
			if (CPU_move03 < 0f)
			{
				if (ballPosition.X < handle0and3Xvalue + (pitchXMax - maxHandleXValue) / 2f)
				{
					handle0and3Xvalue += CPU_move03;
				}
				else
				{
					handle0and3Xvalue += CPU_move03 - 1f;
				}
			}
			else if (ballPosition.X > handle0and3Xvalue + (pitchXMax - maxHandleXValue) / 2f)
			{
				handle0and3Xvalue += CPU_move03;
			}
			else
			{
				handle0and3Xvalue += CPU_move03 + 1f;
			}
		}
		if (handle0and3Xvalue > maxHandleXValue)
		{
			handle0and3Xvalue = maxHandleXValue;
		}
		if (handle0and3Xvalue < 0f)
		{
			handle0and3Xvalue = 0f;
		}
		if (handle1and5Xvalue > maxHandleXValue)
		{
			handle1and5Xvalue = maxHandleXValue;
		}
		if (handle1and5Xvalue < 0f)
		{
			handle1and5Xvalue = 0f;
		}
		if (!CPU_RED_rollback)
		{
			if (ballSpeed > ballSpeedMaxNormal / 1.5f && ((double)ballBearing > Math.PI || ballBearing < 0f))
			{
				row15stun = true;
				row03stun = true;
				handle1and5Rotation = -0.05f;
				handle0and3Rotation = -0.05f;
				CPU_RED_stunCounter = 10;
				return;
			}
			row15stun = false;
			row03stun = false;
			if (CPU_RED_stunCounter > 0)
			{
				CPU_RED_stunCounter--;
				return;
			}
			if ((ballPosition.Z > rowPositionsZ[1] && ballPosition.Z < rowPositionsZ[1] + ballRadius * 3f) || (ballPosition.Z > rowPositionsZ[5] && ballPosition.Z < rowPositionsZ[5] + ballRadius * 3f))
			{
				handle1and5Rotation -= CPU_easyShootSpeed;
				if (handle1and5Rotation < -1f)
				{
					handle1and5Rotation = -1f;
					CPU_RED_rollback = true;
				}
			}
			else if ((ballPosition.Z > rowPositionsZ[1] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[1] + ballRadius * 5f) || (ballPosition.Z > rowPositionsZ[5] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[5] + ballRadius * 5f))
			{
				handle1and5Rotation += CPU_hardShootSpeed / 2f;
				if (handle1and5Rotation > 1f)
				{
					handle1and5Rotation = 1f;
				}
			}
			else
			{
				handle1and5Rotation = 0f;
			}
			if ((ballPosition.Z > rowPositionsZ[0] && ballPosition.Z < rowPositionsZ[0] + ballRadius * 3f) || (ballPosition.Z > rowPositionsZ[3] && ballPosition.Z < rowPositionsZ[3] + ballRadius * 3f))
			{
				handle0and3Rotation -= CPU_hardShootSpeed;
				if (handle0and3Rotation < -1f)
				{
					handle0and3Rotation = -1f;
					CPU_RED_rollback = true;
				}
			}
			else if ((ballPosition.Z > rowPositionsZ[0] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[0] + ballRadius * 5f) || (ballPosition.Z > rowPositionsZ[3] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[3] + ballRadius * 5f))
			{
				handle0and3Rotation += CPU_easyShootSpeed / 2f;
				if (handle0and3Rotation > 1f)
				{
					handle0and3Rotation = 1f;
				}
			}
			else
			{
				handle0and3Rotation = 0f;
			}
		}
		else
		{
			handle0and3Rotation += 0.05f;
			handle1and5Rotation += 0.05f;
			if (handle0and3Rotation > 0f)
			{
				handle0and3Rotation = 0f;
			}
			if (handle1and5Rotation > 0f)
			{
				handle1and5Rotation = 0f;
			}
			if (handle0and3Rotation == 0f && handle1and5Rotation == 0f)
			{
				CPU_RED_rollback = false;
			}
		}
	}

	private void CPU_blueNormalPlay()
	{
		if (ttl_cockup < 1)
		{
			ttl_cockup = CPU_Random.Next(10, 120);
			CPU_cockup = CPU_Random.Next(10, 790);
			if (CPU_cockup > 390f)
			{
				CPU_cockup -= 400f;
			}
			CPU_cockup *= 1.5f;
		}
		else
		{
			ttl_cockup--;
		}
		float num = 0f;
		num = ((ballPosition.Z < rowPositionsZ[2]) ? ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - forwardsOffSet) + CPU_cockup / 2f - 100f) : (ballPosition.X - maxHandleXValue + (forwardsOffSet - maxHandleXValue) + CPU_cockup / 2f + 100f)) : ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - centerbacksOffSet) + CPU_cockup) : (ballPosition.X - maxHandleXValue + (centerbacksOffSet - maxHandleXValue) + CPU_cockup)));
		if (ballPosition.Z > rowPositionsZ[2] - 100f && ballPosition.Z < pitchZMax / 2f && ballBearing > (float)Math.PI)
		{
			num = ballPosition.X - maxHandleXValue;
		}
		if (ballPosition.Z > rowPositionsZ[6])
		{
			num = ballPosition.X - maxHandleXValue;
		}
		if (handle2and6Xvalue < num)
		{
			handle2and6Xvalue += CPU_moveSpeed * 2f;
			if (handle2and6Xvalue > num)
			{
				handle2and6Xvalue = num;
			}
		}
		else
		{
			handle2and6Xvalue -= CPU_moveSpeed * 2f;
			if (handle2and6Xvalue < num)
			{
				handle2and6Xvalue = num;
			}
		}
		if (CPU_BLUE_timeToDecision < 1)
		{
			CPU_BLUE_timeToDecision = CPU_Random.Next(CPU_decisionMin, CPU_decisionMax) / 2;
			CPU_move47 = (float)CPU_Random.Next(50, 100) / 50f * CPU_moveSpeed;
			if (ballPosition.X < handle4and7Xvalue + (pitchXMax - maxHandleXValue) / 2f)
			{
				CPU_move47 = 0f - CPU_move47;
			}
		}
		else
		{
			CPU_BLUE_timeToDecision--;
			if (CPU_move47 < 0f)
			{
				if (ballPosition.X < handle4and7Xvalue + (pitchXMax - maxHandleXValue) / 2f)
				{
					handle4and7Xvalue += CPU_move47;
				}
				else
				{
					handle4and7Xvalue += CPU_move47 - 1f;
				}
			}
			else if (ballPosition.X > handle4and7Xvalue + (pitchXMax - maxHandleXValue) / 2f)
			{
				handle4and7Xvalue += CPU_move47;
			}
			else
			{
				handle4and7Xvalue += CPU_move47 + 1f;
			}
		}
		if (handle4and7Xvalue > maxHandleXValue)
		{
			handle4and7Xvalue = maxHandleXValue;
		}
		if (handle4and7Xvalue < 0f)
		{
			handle4and7Xvalue = 0f;
		}
		if (handle2and6Xvalue > maxHandleXValue)
		{
			handle2and6Xvalue = maxHandleXValue;
		}
		if (handle2and6Xvalue < 0f)
		{
			handle2and6Xvalue = 0f;
		}
		if (!CPU_BLUE_rollback)
		{
			if (ballSpeed > ballSpeedMaxNormal / 1.5f && (double)ballBearing < Math.PI && ballBearing > 0f)
			{
				row26stun = true;
				row47stun = true;
				handle2and6Rotation = -0.05f;
				handle4and7Rotation = -0.05f;
				CPU_BLUE_stunCounter = 10;
				return;
			}
			row26stun = false;
			row47stun = false;
			if (CPU_BLUE_stunCounter > 0)
			{
				CPU_BLUE_stunCounter--;
				return;
			}
			if ((ballPosition.Z < rowPositionsZ[4] && ballPosition.Z > rowPositionsZ[4] - ballRadius * 3f) || (ballPosition.Z < rowPositionsZ[7] && ballPosition.Z > rowPositionsZ[7] - ballRadius * 3f))
			{
				handle4and7Rotation += CPU_easyShootSpeed;
				if (handle4and7Rotation > 1f)
				{
					handle4and7Rotation = 1f;
					CPU_BLUE_rollback = true;
				}
			}
			else if ((ballPosition.Z < rowPositionsZ[4] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[4] - ballRadius * 5f) || (ballPosition.Z < rowPositionsZ[7] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[7] - ballRadius * 5f))
			{
				handle4and7Rotation -= CPU_easyShootSpeed / 2f;
				if (handle4and7Rotation < -1f)
				{
					handle4and7Rotation = -1f;
				}
			}
			else
			{
				handle4and7Rotation = 0f;
			}
			if ((ballPosition.Z < rowPositionsZ[2] && ballPosition.Z > rowPositionsZ[2] - ballRadius * 3f) || (ballPosition.Z < rowPositionsZ[6] && ballPosition.Z > rowPositionsZ[6] - ballRadius * 3f))
			{
				handle2and6Rotation += CPU_hardShootSpeed;
				if (handle2and6Rotation > 1f)
				{
					handle2and6Rotation = 1f;
					CPU_BLUE_rollback = true;
				}
			}
			else if ((ballPosition.Z < rowPositionsZ[2] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[2] - ballRadius * 5f) || (ballPosition.Z < rowPositionsZ[6] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[6] - ballRadius * 5f))
			{
				handle2and6Rotation -= CPU_hardShootSpeed / 2f;
				if (handle2and6Rotation < -1f)
				{
					handle2and6Rotation = -1f;
				}
			}
			else
			{
				handle2and6Rotation = 0f;
			}
		}
		else
		{
			handle4and7Rotation -= 0.05f;
			handle2and6Rotation -= 0.05f;
			if (handle4and7Rotation < 0f)
			{
				handle4and7Rotation = 0f;
			}
			if (handle2and6Rotation < 0f)
			{
				handle2and6Rotation = 0f;
			}
			if (handle2and6Rotation == 0f && handle4and7Rotation == 0f)
			{
				CPU_BLUE_rollback = false;
			}
		}
	}

	private void CPU_redHardPlay()
	{
		if (ttl_redCockup < 1)
		{
			ttl_redCockup = CPU_Random.Next(10, 120);
			CPU_redCockup = CPU_Random.Next(10, 790);
			if (CPU_redCockup > 390f)
			{
				CPU_redCockup -= 400f;
			}
		}
		else
		{
			ttl_redCockup--;
		}
		if (ttl_redCockup2 < 1)
		{
			ttl_redCockup2 = CPU_Random.Next(10, 120);
			CPU_redCockup2 = CPU_Random.Next(10, 390);
			if (CPU_redCockup2 > 190f)
			{
				CPU_redCockup2 -= 200f;
			}
		}
		else
		{
			ttl_redCockup2--;
		}
		float num = 0f;
		float num2 = 0f;
		num = ((!(ballPosition.Z < rowPositionsZ[3])) ? ((maxHandleXValue / 2f + (ballPosition.X - maxHandleXValue) * 3f) / 4f + CPU_redCockup * 2f) : ((!(ballBearing > (float)Math.PI) && !(ballBearing < 0f)) ? ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X + maxHandleXValue - midfieldCentres * 1.5f) : (ballPosition.X + midfieldCentres * 1.5f)) : ((maxHandleXValue / 2f + (ballPosition.X - maxHandleXValue) * 3f) / 4f)));
		if (ballPosition.Z < rowPositionsZ[0])
		{
			num = maxHandleXValue / 2f;
		}
		num2 = ((ballPosition.Z > rowPositionsZ[5]) ? ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - forwardsOffSet) + (float)ttl_redCockup2 / 2f - 100f) : (ballPosition.X - maxHandleXValue + (forwardsOffSet - maxHandleXValue) + (float)ttl_redCockup2 / 2f + 100f)) : ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - centerbacksOffSet) + CPU_redCockup) : (ballPosition.X - maxHandleXValue + (centerbacksOffSet - maxHandleXValue) + CPU_redCockup)));
		if (ballPosition.Z < rowPositionsZ[5] + 100f && ballPosition.Z > pitchZMax / 2f && ballBearing < (float)Math.PI)
		{
			num2 = ballPosition.X - maxHandleXValue;
		}
		if (ballPosition.Z < rowPositionsZ[1])
		{
			num2 = ballPosition.X - maxHandleXValue;
		}
		if (handle0and3Xvalue < num)
		{
			handle0and3Xvalue += CPU_moveSpeed * 2f;
			if (handle0and3Xvalue > num)
			{
				handle0and3Xvalue = num;
			}
		}
		else
		{
			handle0and3Xvalue -= CPU_moveSpeed * 2f;
			if (handle0and3Xvalue < num)
			{
				handle0and3Xvalue = num;
			}
		}
		if (handle0and3Xvalue > maxHandleXValue)
		{
			handle0and3Xvalue = maxHandleXValue;
		}
		if (handle0and3Xvalue < 0f)
		{
			handle0and3Xvalue = 0f;
		}
		if (handle1and5Xvalue < num2)
		{
			handle1and5Xvalue += CPU_moveSpeed * 3f;
			if (handle1and5Xvalue > num2)
			{
				handle1and5Xvalue = num2;
			}
		}
		else
		{
			handle1and5Xvalue -= CPU_moveSpeed * 3f;
			if (handle1and5Xvalue < num2)
			{
				handle1and5Xvalue = num2;
			}
		}
		if (handle1and5Xvalue > maxHandleXValue)
		{
			handle1and5Xvalue = maxHandleXValue;
		}
		if (handle1and5Xvalue < 0f)
		{
			handle1and5Xvalue = 0f;
		}
		if (!CPU_RED_rollback)
		{
			if (ballSpeed > ballSpeedMaxNormal / 1.5f && ((double)ballBearing > Math.PI || ballBearing < 0f))
			{
				row15stun = true;
				row03stun = true;
				handle1and5Rotation = -0.05f;
				handle0and3Rotation = -0.05f;
				CPU_RED_stunCounter = 10;
				return;
			}
			row15stun = false;
			row03stun = false;
			if (CPU_RED_stunCounter > 0)
			{
				CPU_RED_stunCounter--;
				return;
			}
			if ((ballPosition.Z > rowPositionsZ[1] && ballPosition.Z < rowPositionsZ[1] + ballRadius * 3f) || (ballPosition.Z > rowPositionsZ[5] && ballPosition.Z < rowPositionsZ[5] + ballRadius * 3f))
			{
				handle1and5Rotation -= CPU_easyShootSpeed;
				if (handle1and5Rotation < -1f)
				{
					handle1and5Rotation = -1f;
					CPU_RED_rollback = true;
				}
			}
			else if ((ballPosition.Z > rowPositionsZ[1] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[1] + ballRadius * 5f) || (ballPosition.Z > rowPositionsZ[5] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[5] + ballRadius * 5f))
			{
				handle1and5Rotation += CPU_hardShootSpeed / 2f;
				if (handle1and5Rotation > 1f)
				{
					handle1and5Rotation = 1f;
				}
			}
			else
			{
				handle1and5Rotation = 0f;
			}
			if ((ballPosition.Z > rowPositionsZ[0] && ballPosition.Z < rowPositionsZ[0] + ballRadius * 3f) || (ballPosition.Z > rowPositionsZ[3] && ballPosition.Z < rowPositionsZ[3] + ballRadius * 3f))
			{
				handle0and3Rotation -= CPU_hardShootSpeed;
				if (handle0and3Rotation < -1f)
				{
					handle0and3Rotation = -1f;
					CPU_RED_rollback = true;
				}
			}
			else if ((ballPosition.Z > rowPositionsZ[0] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[0] + ballRadius * 5f) || (ballPosition.Z > rowPositionsZ[3] + ballRadius * 3f && ballPosition.Z < rowPositionsZ[3] + ballRadius * 5f))
			{
				handle0and3Rotation += CPU_easyShootSpeed / 2f;
				if (handle0and3Rotation > 1f)
				{
					handle0and3Rotation = 1f;
				}
			}
			else
			{
				handle0and3Rotation = 0f;
			}
		}
		else
		{
			handle0and3Rotation += 0.05f;
			handle1and5Rotation += 0.05f;
			if (handle0and3Rotation > 0f)
			{
				handle0and3Rotation = 0f;
			}
			if (handle1and5Rotation > 0f)
			{
				handle1and5Rotation = 0f;
			}
			if (handle0and3Rotation == 0f && handle1and5Rotation == 0f)
			{
				CPU_RED_rollback = false;
			}
		}
	}

	private void CPU_blueHardPlay()
	{
		if (ttl_cockup < 1)
		{
			ttl_cockup = CPU_Random.Next(10, 120);
			CPU_cockup = CPU_Random.Next(10, 790);
			if (CPU_cockup > 390f)
			{
				CPU_cockup -= 400f;
			}
		}
		else
		{
			ttl_cockup--;
		}
		if (ttl_cockup2 < 1)
		{
			ttl_cockup2 = CPU_Random.Next(10, 120);
			CPU_cockup2 = CPU_Random.Next(10, 390);
			if (CPU_cockup2 > 190f)
			{
				CPU_cockup2 -= 200f;
			}
		}
		else
		{
			ttl_cockup2--;
		}
		float num = 0f;
		float num2 = 0f;
		num = ((!(ballPosition.Z > rowPositionsZ[4])) ? ((maxHandleXValue / 2f + (ballPosition.X - maxHandleXValue) * 3f) / 4f + CPU_cockup * 2f) : ((ballBearing < (float)Math.PI) ? ((maxHandleXValue / 2f + (ballPosition.X - maxHandleXValue) * 3f) / 4f) : ((!(ballPosition.X < pitchXMax / 2f)) ? (ballPosition.X + maxHandleXValue - midfieldCentres * 1.5f) : (ballPosition.X + midfieldCentres * 1.5f))));
		if (ballPosition.Z > rowPositionsZ[7])
		{
			num = maxHandleXValue / 2f;
		}
		num2 = ((ballPosition.Z < rowPositionsZ[2]) ? ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - forwardsOffSet) + CPU_cockup2 / 2f - 100f) : (ballPosition.X - maxHandleXValue + (forwardsOffSet - maxHandleXValue) + CPU_cockup2 / 2f + 100f)) : ((!(ballPosition.X > pitchXMax / 2f)) ? (ballPosition.X - maxHandleXValue + (maxHandleXValue - centerbacksOffSet) + CPU_cockup) : (ballPosition.X - maxHandleXValue + (centerbacksOffSet - maxHandleXValue) + CPU_cockup)));
		if (ballPosition.Z > rowPositionsZ[2] - 100f && ballPosition.Z < pitchZMax / 2f && ballBearing > (float)Math.PI)
		{
			num2 = ballPosition.X - maxHandleXValue;
		}
		if (ballPosition.Z > rowPositionsZ[6])
		{
			num2 = ballPosition.X - maxHandleXValue;
		}
		if (handle4and7Xvalue < num)
		{
			handle4and7Xvalue += CPU_moveSpeed * 2f;
			if (handle4and7Xvalue > num)
			{
				handle4and7Xvalue = num;
			}
		}
		else
		{
			handle4and7Xvalue -= CPU_moveSpeed * 2f;
			if (handle4and7Xvalue < num)
			{
				handle4and7Xvalue = num;
			}
		}
		if (handle4and7Xvalue > maxHandleXValue)
		{
			handle4and7Xvalue = maxHandleXValue;
		}
		if (handle4and7Xvalue < 0f)
		{
			handle4and7Xvalue = 0f;
		}
		if (handle2and6Xvalue < num2)
		{
			handle2and6Xvalue += CPU_moveSpeed * 3f;
			if (handle2and6Xvalue > num2)
			{
				handle2and6Xvalue = num2;
			}
		}
		else
		{
			handle2and6Xvalue -= CPU_moveSpeed * 3f;
			if (handle2and6Xvalue < num2)
			{
				handle2and6Xvalue = num2;
			}
		}
		if (handle2and6Xvalue > maxHandleXValue)
		{
			handle2and6Xvalue = maxHandleXValue;
		}
		if (handle2and6Xvalue < 0f)
		{
			handle2and6Xvalue = 0f;
		}
		if (!CPU_BLUE_rollback)
		{
			if (ballSpeed > ballSpeedMaxNormal / 1.5f && (double)ballBearing < Math.PI && ballBearing > 0f)
			{
				row26stun = true;
				row47stun = true;
				handle2and6Rotation = -0.05f;
				handle4and7Rotation = -0.05f;
				CPU_BLUE_stunCounter = 10;
				return;
			}
			row26stun = false;
			row47stun = false;
			if (CPU_BLUE_stunCounter > 0)
			{
				CPU_BLUE_stunCounter--;
				return;
			}
			if ((ballPosition.Z < rowPositionsZ[4] && ballPosition.Z > rowPositionsZ[4] - ballRadius * 3f) || (ballPosition.Z < rowPositionsZ[7] && ballPosition.Z > rowPositionsZ[7] - ballRadius * 3f))
			{
				handle4and7Rotation += CPU_easyShootSpeed;
				if (handle4and7Rotation > 1f)
				{
					handle4and7Rotation = 1f;
					CPU_BLUE_rollback = true;
				}
			}
			else if ((ballPosition.Z < rowPositionsZ[4] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[4] - ballRadius * 5f) || (ballPosition.Z < rowPositionsZ[7] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[7] - ballRadius * 5f))
			{
				handle4and7Rotation -= CPU_easyShootSpeed / 2f;
				if (handle4and7Rotation < -1f)
				{
					handle4and7Rotation = -1f;
				}
			}
			else
			{
				handle4and7Rotation = 0f;
			}
			if ((ballPosition.Z < rowPositionsZ[2] && ballPosition.Z > rowPositionsZ[2] - ballRadius * 3f) || (ballPosition.Z < rowPositionsZ[6] && ballPosition.Z > rowPositionsZ[6] - ballRadius * 3f))
			{
				handle2and6Rotation += CPU_hardShootSpeed;
				if (handle2and6Rotation > 1f)
				{
					handle2and6Rotation = 1f;
					CPU_BLUE_rollback = true;
				}
			}
			else if ((ballPosition.Z < rowPositionsZ[2] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[2] - ballRadius * 5f) || (ballPosition.Z < rowPositionsZ[6] - ballRadius * 3f && ballPosition.Z > rowPositionsZ[6] - ballRadius * 5f))
			{
				handle2and6Rotation -= CPU_hardShootSpeed / 2f;
				if (handle2and6Rotation < -1f)
				{
					handle2and6Rotation = -1f;
				}
			}
			else
			{
				handle2and6Rotation = 0f;
			}
		}
		else
		{
			handle4and7Rotation -= 0.05f;
			handle2and6Rotation -= 0.05f;
			if (handle4and7Rotation < 0f)
			{
				handle4and7Rotation = 0f;
			}
			if (handle2and6Rotation < 0f)
			{
				handle2and6Rotation = 0f;
			}
			if (handle2and6Rotation == 0f && handle4and7Rotation == 0f)
			{
				CPU_BLUE_rollback = false;
			}
		}
	}
}
