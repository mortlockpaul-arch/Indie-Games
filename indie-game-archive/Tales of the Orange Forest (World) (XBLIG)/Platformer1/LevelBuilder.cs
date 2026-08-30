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
using ProjectMercury.Emitters;
using ProjectMercury.Renderers;

namespace Platformer1;

public class LevelBuilder : IDisposable
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

	public const float PhysicsScaleDown = 0.2f;

	public const float Grav = 1000f;

	private const int EntityLayer = 1;

	private const float GroundBody_Width = 20000f;

	private const float GroundBody_Height = 300f;

	private bool Add;

	private bool WasAddPressed;

	private bool WasSavePressed;

	private bool WasPhysicsPressed;

	private bool Saving;

	private bool Xbox;

	private float ZoomScaler = 1f;

	private float ZoomScaler_Step = 0.005f;

	private bool WasZoomInPressed;

	private bool WasZoomOutPressed;

	public float PhysicsScaleUp = 5f;

	public PlatformerGame mainGame;

	private PlayerIndex PlayerIndexer_Pub;

	private PlayerIndex PlayerInControl;

	public float Gravity = 1000f;

	public World _world = new World(new Vector2(0f, 1000f));

	public Texture2D CurserBrush;

	public Texture2D CurserBrushCrossHairs;

	public Texture2D Instruction_Brush;

	public string Curser_String_1 = " ";

	public string Curser_String_2 = " ";

	public SpriteFont CurserFont;

	public Texture2D CenterDot;

	public Texture2D SpawnPointTexture;

	public float CurserRotation;

	public Texture2D CurserDefault;

	public Texture2D BrickTexture;

	public Texture2D KineticTexture;

	public Texture2D MineTexture;

	public Texture2D SharpTexture;

	public Texture2D EnemyTexture;

	public Texture2D BlockTexture;

	public Texture2D LandTexture;

	public Texture2D NothingTexture;

	public Texture2D ExitTexture;

	public bool PhysicsPaused;

	public bool MusicToggle;

	public bool FirstTime;

	public bool Blood = true;

	public bool SoundEffectToggle = true;

	public SpriteFont Font;

	public SpriteFont HudFont;

	public SpriteEffects spriteEffect;

	public string DataString;

	public string Data2;

	public bool Paused;

	private SpriteFont PauseFont;

	private int PauseMenuIndexer;

	private int PauseMenuIndexerMax = 3;

	private int PlayerPausedIndex;

	private bool DpadRightWaspressed;

	private bool DpadRightpressed;

	private bool DpadLeftWaspressed;

	private bool DpadLeftpressed;

	private int LoadLevelIndexer = 25;

	private int LoadLevelIndexerMax = 34;

	public string LevelPath = " ";

	private bool PauseMenuButtonAWasPressed;

	public bool P1DpadUppressed;

	public bool P1DpadUpWaspressed;

	public bool P1DpadDownpressed;

	public bool P1DpadDownWaspressed;

	public Texture2D PauseMenuTexture;

	private Texture2D PauseMenuBackgroundStripTexture;

	private Texture2D PauseMenuControllerLayoutTexture;

	private Texture2D PauseMenuSideBarTexture;

	public bool wasContinue1Pressed;

	private Layer[] layers;

	private Clouds[] clouds;

	private Color BackgroundColor;

	public float cameraPosition;

	public float OriginalcameraPosition;

	private float maxCameraPosition;

	private float maxHeightCameraPosition;

	public float cameraHeightPosition;

	public float OriginalcameraHeightPosition;

	private float CameraPositionNewY;

	private float CameraPositionNewX;

	private int LastMilliSeconds;

	public Vector2 CamVector;

	public float newScaler;

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

	private Vector2 GroundBody_Position = new Vector2(10000f, 1000f);

	private Fixture GroundBody;

	private Texture2D GroundPlainStripBrush;

	private Vector2 GroundPlainStripBrush_Origin;

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

	private List<Enemy> Enemys = new List<Enemy>();

	public int EM;

	public int em;

	private float EnemyPositionX;

	private float EnemyPositionY;

	private List<Brick> Bricks = new List<Brick>();

	public int S;

	public int s;

	private float BrickPositionX;

	private float BrickPositionY;

	private Vector2 ExitPosition;

	private List<Kinetics> Kinetics = new List<Kinetics>();

	public int K;

	public int k;

	private float KineticsPositionX;

	private float KineticsPositionY;

	private List<Vector2> points = new List<Vector2>();

	private List<Vector2> normals = new List<Vector2>();

	private Body[] SaveBodies;

	private Vector2 start;

	private Vector2 exit;

	public Vector2 cameraTransformOld;

	public Matrix cameraTransformForParticles;

	private static readonly Point InvalidPosition = new Point(-1, -1);

	public Random random = new Random();

	private ContentManager content;

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

	public int LevelCount = 10;

	public string SavedLevel_Name;

	public string Level_String_Name;

	public string InfoString;

	public bool Exit;

	public bool GameSaveRequested;

	public int result;

	public IAsyncResult result2;

	private bool Dueling;

	public PlatformerGame.LevelNames AllLevelNames;

	private int Count2;

	private int ObjectCount;

	public SavedData LevelData;

	public int LoadingProgress;

	public int ObjectTypeMain;

	public string ObjectTypeSubMain = "0";

	public Vector2 CurserPosition;

	private SpriteEffects HorizontalOrientation;

	public Vector2 MousePosition;

	public Vector2 MousePositionOld;

	public Vector2 LeftJoyStickPosition;

	public Vector2 LeftJoyStickPositionOld;

	public Vector2 RightJoyStickPosition;

	public Vector2 RightJoyStickPositionOld;

	public bool MouseLeftIsPressed;

	public bool MouseRightIsPressed;

	public MouseState mouse;

	public bool Key_S_IsPressed;

	public bool Key_B_IsPressed;

	public bool Key_Add_IsPressed;

	public bool Key_Subtract_IsPressed;

	public bool Key_F1_IsPressed;

	public bool Key_L_IsPressed;

	public bool Key_R_IsPressed;

	public bool Key_P_IsPressed;

	public Texture2D MouseTexture;

	public bool PausePressed;

	public bool wasPausePressed;

	public bool indexWasUp;

	public bool indexWasDown;

	public bool indexWasLeft;

	public bool indexWasRight;

	public readonly string ParticleEffecstDir = "/Effects/Particle/";

	public ParticleEffect particleEffectAdd;

	public ParticleEffect particleEffectRemove;

	public ParticleEffect particleEffectSave;

	public ParticleEffect particleEffectExit;

	public ParticleEffect particleEffectStart;

	public Emitter FireEmitter;

	public SpriteBatchRenderer renderer;

	public ContentManager Content => content;

	public LevelBuilder(PlatformerGame Game, IServiceProvider serviceProvider, string Level_Name, string Level_String_Name, SpriteBatch spriteBatch, PlayerIndex PlayerIndexer)
	{
		mainGame = Game;
		AllLevelNames = mainGame.AllLevelNames;
		PlayerIndexer_Pub = PlayerIndexer;
		if (PlayerIndexer_Pub == PlayerIndex.One)
		{
			PlayerPausedIndex = 1;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Two)
		{
			PlayerPausedIndex = 2;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Three)
		{
			PlayerPausedIndex = 3;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Four)
		{
			PlayerPausedIndex = 4;
		}
		content = new ContentManager(serviceProvider, "Content");
		Font = Content.Load<SpriteFont>("Fonts/menufont1");
		HudFont = Content.Load<SpriteFont>("Fonts/Hud2");
		SavedLevel_Name = Level_Name;
		HorizontalOrientation = SpriteEffects.None;
		this.Level_String_Name = Level_String_Name;
		renderer = new SpriteBatchRenderer
		{
			GraphicsDeviceService = mainGame.graphics
		};
		LevelData = new SavedData(700);
		for (int i = 0; i < LevelData.Count; i++)
		{
			LevelData.ObjectType[i] = 0;
			LevelData.ObjectSubType[i] = "0";
			LevelData.ObjectCount[i] = 0;
			ref Vector2 reference = ref LevelData.ObjectPosition[i];
			reference = new Vector2(0f, 0f);
			LevelData.ObjectRotation[i] = 0f;
		}
		LevelData.LevelName = " ";
		LevelData.Dueling = false;
		LevelPath = mainGame.levelBuilderPath;
		LoadLevel();
		layers = new Layer[8];
		layers[0] = new Layer(Content, "Backgrounds/Background_1", 0f, 0.01f, 0);
		layers[1] = new Layer(Content, "Backgrounds/GroundPlain", 0f, 0f, 4);
		particleEffectAdd = Content.Load<ParticleEffect>("Effects/Particle/Add");
		particleEffectRemove = Content.Load<ParticleEffect>("Effects/Particle/Remove");
		particleEffectSave = Content.Load<ParticleEffect>("Effects/Particle/Save");
		particleEffectExit = Content.Load<ParticleEffect>("Effects/Particle/Exit");
		particleEffectStart = Content.Load<ParticleEffect>("Effects/Particle/Start");
		PauseMenuTexture = Content.Load<Texture2D>("Menus/Pause/Intermission");
		PauseMenuBackgroundStripTexture = Content.Load<Texture2D>("Menus/Pause/PauseBackgroundStrip");
		PauseMenuSideBarTexture = Content.Load<Texture2D>("Menus/Pause/LevelBuilderSideBar");
		PauseMenuControllerLayoutTexture = Content.Load<Texture2D>("Menus/Pause/LevelBuilderControllerLayout");
		PauseFont = Content.Load<SpriteFont>("Fonts/menufont2");
		CurserFont = Content.Load<SpriteFont>("Fonts/CurserFont");
		particleEffectAdd.Initialise();
		particleEffectAdd.LoadContent(Content);
		particleEffectRemove.Initialise();
		particleEffectRemove.LoadContent(Content);
		particleEffectSave.Initialise();
		particleEffectSave.LoadContent(Content);
		particleEffectExit.Initialise();
		particleEffectExit.LoadContent(Content);
		particleEffectStart.Initialise();
		particleEffectStart.LoadContent(Content);
		renderer.LoadContent(Content);
		clouds = new Clouds[1];
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
		BackgroundColor = new Color(255, 255, 255, 255);
		ShadowTexture = Content.Load<Texture2D>("Shadows/Shadow1");
		ShadowTextureOrigin = new Vector2(ShadowTexture.Width / 2, ShadowTexture.Height / 2 - 5);
		BrickTexture = Content.Load<Texture2D>("Bricks/0/0");
		BlockTexture = Content.Load<Texture2D>("Blocks/0");
		MineTexture = Content.Load<Texture2D>("Bricks/Mines/0");
		SharpTexture = Content.Load<Texture2D>("Sharps/0");
		KineticTexture = Content.Load<Texture2D>("Kinetics/0");
		NothingTexture = Content.Load<Texture2D>("Nothing");
		CurserDefault = Content.Load<Texture2D>("Flair/Nothing/Nothing");
		Instruction_Brush = Content.Load<Texture2D>("Menus/Instruction");
		CurserBrush = Content.Load<Texture2D>("Flair/Nothing/Nothing");
		CurserBrushCrossHairs = Content.Load<Texture2D>("Curser/Rose");
		waterfallTexture = Content.Load<Texture2D>("FX/waterfall");
		CenterDot = Content.Load<Texture2D>("LevelBuilder/CenterDot");
		SpawnPointTexture = Content.Load<Texture2D>("Curser/SpawnPoint");
		Xbox = true;
		if (mainGame.P1InControlOfMainMenu)
		{
			PlayerInControl = PlayerIndex.One;
		}
		if (mainGame.P2InControlOfMainMenu)
		{
			PlayerInControl = PlayerIndex.Two;
		}
		if (mainGame.P3InControlOfMainMenu)
		{
			PlayerInControl = PlayerIndex.Three;
		}
		if (mainGame.P4InControlOfMainMenu)
		{
			PlayerInControl = PlayerIndex.Four;
		}
	}

	private void LoadLevel()
	{
		SavedData savedData = LoadData(LevelPath);
		GroundBody = FixtureFactory.CreateRectangle(_world, 20000f, 300f, 100f);
		GroundBody.Body.Position = GroundBody_Position * 0.2f;
		GroundBody.Body.Rotation = 0f;
		GroundBody.Friction = 1f;
		GroundBody.Body.SleepingAllowed = true;
		GroundBody.Body.BodyType = BodyType.Static;
		GroundBody.CollisionCategories = CollisionCategory.Cat30;
		GroundBody.CollisionGroup = 365;
		GroundBody.UserData = 7999;
		GroundBody.Body.UserData = 7999;
		LevelData = new SavedData(savedData.Count + LevelData.Count);
		LevelData.LevelName = savedData.LevelName;
		LevelData.Dueling = savedData.Dueling;
		for (int i = 0; i < savedData.Count; i++)
		{
			LevelData.ObjectCount[i] = savedData.ObjectCount[i];
			ref Vector2 reference = ref LevelData.ObjectPosition[i];
			reference = savedData.ObjectPosition[i];
			LevelData.ObjectRotation[i] = savedData.ObjectRotation[i];
			LevelData.ObjectSubType[i] = savedData.ObjectSubType[i];
			LevelData.ObjectType[i] = savedData.ObjectType[i];
		}
		Count2 = savedData.Count;
		ObjectCount = savedData.Count;
		for (int j = 0; j < savedData.Count; j++)
		{
			if (LevelData.ObjectType[j] == 0)
			{
				continue;
			}
			if (LevelData.ObjectType[j] == 1)
			{
				B++;
				Blocks.Add(new Blocks(content, null, mainGame, LevelData.ObjectPosition[j], _world, LevelData.ObjectSubType[j], LevelData.ObjectType[j], LevelData.ObjectRotation[j], 0));
			}
			if (LevelData.ObjectType[j] == 2)
			{
				S++;
				Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[j], _world, LevelData.ObjectSubType[j], LevelData.ObjectType[j], LevelData.ObjectRotation[j], renderer, 0));
			}
			if (LevelData.ObjectType[j] == 3)
			{
				if (LevelData.ObjectSubType[j] == "0")
				{
					S++;
					Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[j], _world, LevelData.ObjectSubType[j], LevelData.ObjectType[j], LevelData.ObjectRotation[j], renderer, 0));
				}
				else if (LevelData.ObjectSubType[j] == "1")
				{
					S++;
					Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[j], _world, LevelData.ObjectSubType[j], LevelData.ObjectType[j], LevelData.ObjectRotation[j], renderer, 0));
				}
				else if (LevelData.ObjectSubType[j] == "2")
				{
					S++;
					Bricks.Add(new Brick(Content, null, mainGame, LevelData.ObjectPosition[j], _world, LevelData.ObjectSubType[j], LevelData.ObjectType[j], LevelData.ObjectRotation[j], renderer, 0));
				}
				else
				{
					SH++;
					Sharps.Add(new Sharps(content, null, mainGame, LevelData.ObjectPosition[j], _world, LevelData.ObjectSubType[j], LevelData.ObjectRotation[j], 0));
				}
			}
			if (LevelData.ObjectType[j] == 4)
			{
				K++;
				Kinetics.Add(new Kinetics(content, null, mainGame, LevelData.ObjectPosition[j], _world, LevelData.ObjectSubType[j], LevelData.ObjectRotation[j], renderer, 0));
			}
			if (LevelData.ObjectType[j] == 5)
			{
				ExitPosition = LevelData.ObjectPosition[j];
			}
		}
	}

	private void AddData(int ObjectType, string ObjectSubType, Vector2 ObjectPosition, int i)
	{
		if (ObjectCount > 400 || ObjectCount >= LevelData.Count)
		{
			return;
		}
		particleEffectAdd.Trigger(ObjectPosition * newScaler);
		ObjectPosition += new Vector2(cameraPosition, cameraHeightPosition);
		LevelData.ObjectType[i] = ObjectType;
		LevelData.ObjectSubType[i] = ObjectSubType;
		LevelData.ObjectPosition[i] = ObjectPosition;
		LevelData.ObjectRotation[i] = CurserRotation;
		if (LevelData.ObjectType[i] != 0)
		{
			ObjectCount++;
		}
		if (LevelData.ObjectType[i] == 1)
		{
			LevelData.ObjectSubType[i] = ObjectSubType;
			LoadBlock(LevelData, ObjectType, ObjectSubType);
		}
		if (LevelData.ObjectType[i] == 2)
		{
			LevelData.ObjectSubType[i] = ObjectSubType;
			LoadBrick(LevelData, ObjectType, ObjectSubType);
		}
		if (LevelData.ObjectType[i] == 3)
		{
			if (LevelData.ObjectSubType[i] == "0")
			{
				LevelData.ObjectSubType[i] = ObjectSubType;
				LoadBrick(LevelData, ObjectType, ObjectSubType);
			}
			else if (LevelData.ObjectSubType[i] == "1")
			{
				LevelData.ObjectSubType[i] = ObjectSubType;
				LoadBrick(LevelData, ObjectType, ObjectSubType);
			}
			else if (LevelData.ObjectSubType[i] == "2")
			{
				LevelData.ObjectSubType[i] = ObjectSubType;
				LoadBrick(LevelData, ObjectType, ObjectSubType);
			}
			else
			{
				LevelData.ObjectSubType[i] = ObjectSubType;
				LoadSharp(LevelData, ObjectType, ObjectSubType);
			}
		}
		if (LevelData.ObjectType[i] == 4)
		{
			LevelData.ObjectSubType[i] = ObjectSubType;
			LoadKinetic(LevelData, ObjectType, ObjectSubType);
		}
		if (LevelData.ObjectType[i] == 5)
		{
			LoadExit(LevelData, ObjectType);
		}
	}

	private void RemoveData(int ObjectType, string ObjectSubType, Vector2 CurserPosition, int i)
	{
		particleEffectRemove.Trigger(CurserPosition * newScaler);
		CurserPosition += new Vector2(cameraPosition, cameraHeightPosition);
		CurserPosition *= new Vector2(0.2f, 0.2f);
		foreach (Brick brick in Bricks)
		{
			Vector2 vector = brick.BrickBody.Body.Position - CurserPosition;
			if (vector.X < 1f && vector.X > -1f && vector.Y < 1f && vector.Y > -1f && brick.BrickBody != null && brick.BrickBody.Body != null && brick.BrickBody.Body.FixtureList != null)
			{
				_world.RemoveBody(brick.BrickBody.Body);
				brick.Active = false;
				brick.texture = Content.Load<Texture2D>("Nothing");
			}
		}
		foreach (Blocks block in Blocks)
		{
			Vector2 vector2 = block.BlockBody.Body.Position - CurserPosition;
			if (vector2.X < 1f && vector2.X > -1f && vector2.Y < 1f && vector2.Y > -1f && block.BlockBody != null && block.BlockBody.Body != null && block.BlockBody.Body.FixtureList != null)
			{
				_world.RemoveBody(block.BlockBody.Body);
				block.Active = false;
				block.texture = Content.Load<Texture2D>("Nothing");
			}
		}
		foreach (Sharps sharp in Sharps)
		{
			Vector2 vector3 = sharp.SharpBody.Body.Position - CurserPosition;
			if (vector3.X < 1f && vector3.X > -1f && vector3.Y < 1f && vector3.Y > -1f && sharp.SharpBody != null && sharp.SharpBody.Body != null && sharp.SharpBody.Body.FixtureList != null)
			{
				_world.RemoveBody(sharp.SharpBody.Body);
				sharp.Active = false;
				sharp.texture = Content.Load<Texture2D>("Nothing");
			}
		}
		foreach (Kinetics kinetic in Kinetics)
		{
			Vector2 vector4 = kinetic.KineticBody.Body.Position - CurserPosition;
			if (vector4.X < 1f && vector4.X > -1f && vector4.Y < 1f && vector4.Y > -1f && kinetic.KineticBody != null && kinetic.KineticBody.Body != null && kinetic.KineticBody.Body.FixtureList != null)
			{
				_world.RemoveBody(kinetic.KineticBody.Body);
				kinetic.Active = false;
				kinetic.texture = Content.Load<Texture2D>("Nothing");
			}
		}
		foreach (Enemy enemy in Enemys)
		{
			Vector2 vector5 = enemy._bodyBody.Body.Position - CurserPosition;
			if (vector5.X < 1f && vector5.X > -1f && vector5.Y < 1f && vector5.Y > -1f && enemy._bodyBody != null && enemy._bodyBody.Body != null && enemy._bodyBody.Body.FixtureList != null)
			{
				_world.RemoveBody(enemy._bodyBody.Body);
				enemy.Active = false;
				enemy._bodyBrush = Content.Load<Texture2D>("Nothing");
			}
		}
		foreach (Lands land in Lands)
		{
			Vector2 vector6 = land.LandBody.Position - CurserPosition;
			if (vector6.X < 1f && vector6.X > -1f && vector6.Y < 1f && vector6.Y > -1f && land.LandBody != null)
			{
				_ = land.LandBody.FixtureList;
			}
		}
	}

	private void RemoveData2(int ObjectType, string ObjectSubType, Vector2 CurserPosition, int i)
	{
		if (ObjectCount <= 0)
		{
			return;
		}
		particleEffectRemove.Trigger(CurserPosition * newScaler);
		CurserPosition += new Vector2(cameraPosition, cameraHeightPosition);
		CurserPosition *= new Vector2(0.2f, 0.2f);
		foreach (Body body in _world.BodyList)
		{
			bool flag = false;
			foreach (Fixture fixture in body.FixtureList)
			{
				if (fixture.TestPoint(ref CurserPosition) && fixture != null && fixture.Body != null && fixture.Body.FixtureList != null && fixture.Body.UserData != null && (int)fixture.Body.UserData != 7999)
				{
					_world.RemoveBody(fixture.Body);
					ObjectCount--;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
	}

	private void UpdateLoadScreen(int LoadingProgress, SpriteBatch spriteBatch)
	{
		DataString = $"Loading {LoadingProgress}%";
		spriteBatch.Begin();
		spriteBatch.DrawString(Font, DataString, new Vector2(spriteBatch.GraphicsDevice.Viewport.Width / 2 - 200, spriteBatch.GraphicsDevice.Viewport.Height / 2), Color.White, 0f, new Vector2(0f, 0f), 3f, SpriteEffects.None, 1f);
		spriteBatch.End();
	}

	private bool LoadLand(SavedData data, int i, string ObjectSubType)
	{
		return true;
	}

	private bool LoadBrick(SavedData data, int i, string ObjectSubType)
	{
		S++;
		Bricks.Add(new Brick(Content, null, mainGame, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition), _world, ObjectSubType, i, CurserRotation, renderer, 0));
		return true;
	}

	private bool LoadBlock(SavedData data, int i, string ObjectSubType)
	{
		B++;
		Blocks.Add(new Blocks(content, null, mainGame, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition), _world, ObjectSubType, i, CurserRotation, 0));
		return true;
	}

	private bool LoadSharp(SavedData data, int i, string ObjectSubType)
	{
		Sharps.Add(new Sharps(content, null, mainGame, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition), _world, ObjectSubType, CurserRotation, 0));
		return true;
	}

	private bool LoadEnemy(SavedData data, int i, string ObjectSubType)
	{
		Enemys.Add(new Enemy(content, null, mainGame, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition), _world, ObjectSubType, CurserRotation, 0));
		return true;
	}

	private bool LoadKinetic(SavedData data, int i, string ObjectSubType)
	{
		Kinetics.Add(new Kinetics(content, null, mainGame, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition), _world, ObjectSubType, CurserRotation, renderer, 0));
		return true;
	}

	private bool LoadExit(SavedData data, int i)
	{
		ExitPosition = CurserPosition + new Vector2(cameraPosition, cameraHeightPosition);
		return true;
	}

	public void SaveData(SavedData data, string path, string Level_Name, int Size)
	{
		int num = 10000;
		SavedData savedData = new SavedData(num);
		for (int i = 0; i < num; i++)
		{
			savedData.LevelName = "New Level";
			savedData.Dueling = Dueling;
			savedData.ObjectType[i] = 0;
			savedData.ObjectSubType[i] = "0";
			savedData.ObjectCount[i] = 0;
			ref Vector2 reference = ref savedData.ObjectPosition[i];
			reference = new Vector2(0f, 0f);
			savedData.ObjectRotation[i] = 0f;
		}
		int num2 = 0;
		if (ExitPosition != new Vector2(0f, 0f))
		{
			num2++;
			savedData.ObjectType[num2] = 9;
			savedData.ObjectSubType[num2] = "0";
			ref Vector2 reference2 = ref savedData.ObjectPosition[num2];
			reference2 = ExitPosition;
			savedData.ObjectRotation[num2] = 0f;
		}
		foreach (Blocks block in Blocks)
		{
			if (block.Active && block.BlockBody != null && block.BlockBody.Body != null && block.BlockBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = block.ObjectType;
				savedData.ObjectSubType[num2] = block.ObjectSubType;
				ref Vector2 reference3 = ref savedData.ObjectPosition[num2];
				reference3 = block.BlockBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = block.BlockBody.Body.Rotation;
			}
		}
		foreach (Sharps sharp in Sharps)
		{
			if (sharp.Active && sharp.SharpBody != null && sharp.SharpBody.Body != null && sharp.SharpBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = sharp.ObjectType;
				savedData.ObjectSubType[num2] = sharp.ObjectSubType;
				ref Vector2 reference4 = ref savedData.ObjectPosition[num2];
				reference4 = sharp.SharpBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = sharp.SharpBody.Body.Rotation;
			}
		}
		foreach (Enemy enemy in Enemys)
		{
			if (enemy.Active && enemy._bodyBody != null && enemy._bodyBody.Body != null && enemy._bodyBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = enemy.ObjectType;
				savedData.ObjectSubType[num2] = enemy.EnemyType;
				ref Vector2 reference5 = ref savedData.ObjectPosition[num2];
				reference5 = enemy._bodyBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = enemy._bodyBody.Body.Rotation;
			}
		}
		foreach (Brick brick in Bricks)
		{
			if (brick.Active && brick.BrickBody != null && brick.BrickBody.Body != null && brick.BrickBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = brick.ObjectType;
				savedData.ObjectSubType[num2] = brick.ObjectTypeSub;
				ref Vector2 reference6 = ref savedData.ObjectPosition[num2];
				reference6 = brick.BrickBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = brick.BrickBody.Body.Rotation;
			}
		}
		foreach (Kinetics kinetic in Kinetics)
		{
			if (kinetic.Active && kinetic.KineticBody != null && kinetic.KineticBody.Body != null && kinetic.KineticBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = kinetic.ObjectType;
				savedData.ObjectSubType[num2] = kinetic.ObjectTypeSub;
				ref Vector2 reference7 = ref savedData.ObjectPosition[num2];
				reference7 = kinetic.Position;
				savedData.ObjectRotation[num2] = kinetic.KineticBody.Body.Rotation;
			}
		}
		foreach (Lands land in Lands)
		{
			if (land.Active && land.LandBody != null && land.LandBody.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = land.ObjectType;
				savedData.ObjectSubType[num2] = land.ObjectSubType;
				ref Vector2 reference8 = ref savedData.ObjectPosition[num2];
				reference8 = land.LandBody.Position * PhysicsScaleUp;
			}
		}
		FileStream fileStream = File.Open(path, FileMode.Create);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedData));
			xmlSerializer.Serialize(fileStream, savedData);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public static StorageContainer OpenContainer(StorageDevice storageDevice, string saveGameName)
	{
		if (storageDevice != null && storageDevice.IsConnected)
		{
			IAsyncResult asyncResult = storageDevice.BeginOpenContainer(saveGameName, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = storageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			return storageContainer;
		}
		return null;
	}

	public void SaveDataXbox(SavedData data, string Level_Name)
	{
		if (Paused)
		{
			return;
		}
		int num = 10;
		foreach (Blocks block in Blocks)
		{
			if (block.Active && block.BlockBody != null && block.BlockBody.Body != null && block.BlockBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Sharps sharp in Sharps)
		{
			if (sharp.Active && sharp.SharpBody != null && sharp.SharpBody.Body != null && sharp.SharpBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Enemy enemy in Enemys)
		{
			if (enemy.Active && enemy._bodyBody != null && enemy._bodyBody.Body != null && enemy._bodyBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Brick brick in Bricks)
		{
			if (brick.Active && brick.BrickBody != null && brick.BrickBody.Body != null && brick.BrickBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Kinetics kinetic in Kinetics)
		{
			if (kinetic.Active && kinetic.KineticBody != null && kinetic.KineticBody.Body != null && kinetic.KineticBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Lands land in Lands)
		{
			if (land.Active && land.LandBody != null && land.LandBody.FixtureList != null)
			{
				num++;
			}
		}
		SavedData savedData = new SavedData(num + 1);
		savedData.LevelName = Level_Name;
		for (int i = 0; i < num + 1; i++)
		{
			savedData.Dueling = Dueling;
			savedData.ObjectType[i] = 0;
			savedData.ObjectSubType[i] = "0";
			savedData.ObjectCount[i] = 0;
			ref Vector2 reference = ref savedData.ObjectPosition[i];
			reference = new Vector2(0f, 0f);
			savedData.ObjectRotation[i] = 0f;
		}
		int num2 = 0;
		if (ExitPosition != new Vector2(0f, 0f))
		{
			num2++;
			savedData.ObjectType[num2] = 5;
			savedData.ObjectSubType[num2] = "0";
			ref Vector2 reference2 = ref savedData.ObjectPosition[num2];
			reference2 = ExitPosition;
			savedData.ObjectRotation[num2] = 0f;
		}
		foreach (Blocks block2 in Blocks)
		{
			if (block2.Active && block2.BlockBody != null && block2.BlockBody.Body != null && block2.BlockBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = block2.ObjectType;
				savedData.ObjectSubType[num2] = block2.ObjectSubType;
				ref Vector2 reference3 = ref savedData.ObjectPosition[num2];
				reference3 = block2.BlockBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = block2.BlockBody.Body.Rotation;
			}
		}
		foreach (Sharps sharp2 in Sharps)
		{
			if (sharp2.Active && sharp2.SharpBody != null && sharp2.SharpBody.Body != null && sharp2.SharpBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = sharp2.ObjectType;
				savedData.ObjectSubType[num2] = sharp2.ObjectSubType;
				ref Vector2 reference4 = ref savedData.ObjectPosition[num2];
				reference4 = sharp2.SharpBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = sharp2.SharpBody.Body.Rotation;
			}
		}
		foreach (Enemy enemy2 in Enemys)
		{
			if (enemy2.Active && enemy2._bodyBody != null && enemy2._bodyBody.Body != null && enemy2._bodyBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = enemy2.ObjectType;
				savedData.ObjectSubType[num2] = enemy2.EnemyType;
				ref Vector2 reference5 = ref savedData.ObjectPosition[num2];
				reference5 = enemy2._bodyBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = enemy2._bodyBody.Body.Rotation;
			}
		}
		foreach (Brick brick2 in Bricks)
		{
			if (brick2.Active && brick2.BrickBody != null && brick2.BrickBody.Body != null && brick2.BrickBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = brick2.ObjectType;
				savedData.ObjectSubType[num2] = brick2.ObjectTypeSub;
				ref Vector2 reference6 = ref savedData.ObjectPosition[num2];
				reference6 = brick2.BrickBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = brick2.BrickBody.Body.Rotation;
			}
		}
		foreach (Kinetics kinetic2 in Kinetics)
		{
			if (kinetic2.Active && kinetic2.KineticBody != null && kinetic2.KineticBody.Body != null && kinetic2.KineticBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = kinetic2.ObjectType;
				savedData.ObjectSubType[num2] = kinetic2.ObjectTypeSub;
				ref Vector2 reference7 = ref savedData.ObjectPosition[num2];
				reference7 = kinetic2.Position;
				savedData.ObjectRotation[num2] = kinetic2.KineticBody.Body.Rotation;
			}
		}
		foreach (Lands land2 in Lands)
		{
			if (land2.Active && land2.LandBody != null && land2.LandBody.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = land2.ObjectType;
				savedData.ObjectSubType[num2] = land2.ObjectSubType;
				ref Vector2 reference8 = ref savedData.ObjectPosition[num2];
				reference8 = land2.LandBody.Position * PhysicsScaleUp;
			}
		}
		if (mainGame.storageDevice.IsConnected)
		{
			StorageContainer storageContainer = OpenContainer(mainGame.storageDevice, "Totof_Levels");
			if (storageContainer == null)
			{
				mainGame.InLevelBuilderMode = false;
				mainGame.InPauseMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.StartLevelBuilder = false;
				MediaPlayer.Stop();
				MediaPlayer.Play(Song0);
			}
			else
			{
				using (storageContainer)
				{
					using Stream stream = storageContainer.CreateFile(mainGame.levelBuilderPath);
					try
					{
						if (mainGame.storageDevice.IsConnected)
						{
							new XmlSerializer(typeof(SavedData)).Serialize(stream, savedData);
						}
					}
					catch (Exception)
					{
					}
					if (LoadLevelIndexer < mainGame.GauntletRunLevelIndexerEnd)
					{
						AllLevelNames.Dueling[mainGame.MainMenuLevelIndexer] = savedData.Dueling;
					}
					else if (LoadLevelIndexer < mainGame.DuelingLevelIndexerEnd)
					{
						AllLevelNames.Dueling[mainGame.MainMenuLevelIndexer + mainGame.GauntletRunLevelIndexerEnd] = savedData.Dueling;
					}
					else if (LoadLevelIndexer >= mainGame.DuelingLevelIndexerEnd)
					{
						AllLevelNames.Dueling[mainGame.MainMenuLevelIndexer + mainGame.DuelingLevelIndexerEnd] = savedData.Dueling;
					}
				}
			}
		}
		else
		{
			mainGame.storageDeviceRemoved();
		}
		Saving = false;
	}

	public void SaveDataWindows(SavedData data, string Level_Name)
	{
		int num = 10;
		foreach (Blocks block in Blocks)
		{
			if (block.Active && block.BlockBody != null && block.BlockBody.Body != null && block.BlockBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Sharps sharp in Sharps)
		{
			if (sharp.Active && sharp.SharpBody != null && sharp.SharpBody.Body != null && sharp.SharpBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Enemy enemy in Enemys)
		{
			if (enemy.Active && enemy._bodyBody != null && enemy._bodyBody.Body != null && enemy._bodyBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Brick brick in Bricks)
		{
			if (brick.Active && brick.BrickBody != null && brick.BrickBody.Body != null && brick.BrickBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Kinetics kinetic in Kinetics)
		{
			if (kinetic.Active && kinetic.KineticBody != null && kinetic.KineticBody.Body != null && kinetic.KineticBody.Body.FixtureList != null)
			{
				num++;
			}
		}
		foreach (Lands land in Lands)
		{
			if (land.Active && land.LandBody != null && land.LandBody.FixtureList != null)
			{
				num++;
			}
		}
		SavedData savedData = new SavedData(num + 1);
		savedData.LevelName = Level_Name;
		for (int i = 0; i < num + 1; i++)
		{
			savedData.Dueling = Dueling;
			savedData.ObjectType[i] = 0;
			savedData.ObjectSubType[i] = "0";
			savedData.ObjectCount[i] = 0;
			ref Vector2 reference = ref savedData.ObjectPosition[i];
			reference = new Vector2(0f, 0f);
			savedData.ObjectRotation[i] = 0f;
		}
		int num2 = 0;
		if (ExitPosition != new Vector2(0f, 0f))
		{
			num2++;
			savedData.ObjectType[num2] = 5;
			savedData.ObjectSubType[num2] = "0";
			ref Vector2 reference2 = ref savedData.ObjectPosition[num2];
			reference2 = ExitPosition;
			savedData.ObjectRotation[num2] = 0f;
		}
		foreach (Blocks block2 in Blocks)
		{
			if (block2.Active && block2.BlockBody != null && block2.BlockBody.Body != null && block2.BlockBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = block2.ObjectType;
				savedData.ObjectSubType[num2] = block2.ObjectSubType;
				ref Vector2 reference3 = ref savedData.ObjectPosition[num2];
				reference3 = block2.BlockBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = block2.BlockBody.Body.Rotation;
			}
		}
		foreach (Sharps sharp2 in Sharps)
		{
			if (sharp2.Active && sharp2.SharpBody != null && sharp2.SharpBody.Body != null && sharp2.SharpBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = sharp2.ObjectType;
				savedData.ObjectSubType[num2] = sharp2.ObjectSubType;
				ref Vector2 reference4 = ref savedData.ObjectPosition[num2];
				reference4 = sharp2.SharpBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = sharp2.SharpBody.Body.Rotation;
			}
		}
		foreach (Enemy enemy2 in Enemys)
		{
			if (enemy2.Active && enemy2._bodyBody != null && enemy2._bodyBody.Body != null && enemy2._bodyBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = enemy2.ObjectType;
				savedData.ObjectSubType[num2] = enemy2.EnemyType;
				ref Vector2 reference5 = ref savedData.ObjectPosition[num2];
				reference5 = enemy2._bodyBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = enemy2._bodyBody.Body.Rotation;
			}
		}
		foreach (Brick brick2 in Bricks)
		{
			if (brick2.Active && brick2.BrickBody != null && brick2.BrickBody.Body != null && brick2.BrickBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = brick2.ObjectType;
				savedData.ObjectSubType[num2] = brick2.ObjectTypeSub;
				ref Vector2 reference6 = ref savedData.ObjectPosition[num2];
				reference6 = brick2.BrickBody.Body.Position * PhysicsScaleUp;
				savedData.ObjectRotation[num2] = brick2.BrickBody.Body.Rotation;
			}
		}
		foreach (Kinetics kinetic2 in Kinetics)
		{
			if (kinetic2.Active && kinetic2.KineticBody != null && kinetic2.KineticBody.Body != null && kinetic2.KineticBody.Body.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = kinetic2.ObjectType;
				savedData.ObjectSubType[num2] = kinetic2.ObjectTypeSub;
				ref Vector2 reference7 = ref savedData.ObjectPosition[num2];
				reference7 = kinetic2.Position;
				savedData.ObjectRotation[num2] = kinetic2.KineticBody.Body.Rotation;
			}
		}
		foreach (Lands land2 in Lands)
		{
			if (land2.Active && land2.LandBody != null && land2.LandBody.FixtureList != null)
			{
				num2++;
				savedData.ObjectType[num2] = land2.ObjectType;
				savedData.ObjectSubType[num2] = land2.ObjectSubType;
				ref Vector2 reference8 = ref savedData.ObjectPosition[num2];
				reference8 = land2.LandBody.Position * PhysicsScaleUp;
			}
		}
		string levelBuilderPath = mainGame.levelBuilderPath;
		FileStream fileStream = File.Create(levelBuilderPath);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedData));
			xmlSerializer.Serialize(fileStream, savedData);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public SavedData LoadData(string path)
	{
		SavedData savedData = default(SavedData);
		try
		{
			StorageContainer storageContainer = OpenContainer(mainGame.storageDevice, "Totof_Levels");
			if (storageContainer == null)
			{
				mainGame.InLevelBuilderMode = false;
				mainGame.InPauseMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.StartLevelBuilder = false;
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
							savedData = (SavedData)xmlSerializer.Deserialize(stream);
							return savedData;
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
		catch (StorageDeviceNotConnectedException)
		{
			mainGame.storageDeviceRemoved();
		}
		return savedData;
	}

	public SavedData LoadData_OLD(string path, Stream stream)
	{
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SavedData));
			return (SavedData)xmlSerializer.Deserialize(stream);
		}
		catch (Exception ex)
		{
			throw new Exception(ex.ToString());
		}
		finally
		{
			stream.Close();
		}
	}

	public static ParticleEffect LoadParticleEffect(string path)
	{
		FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ParticleEffect));
			return (ParticleEffect)xmlSerializer.Deserialize(fileStream);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public void Dispose()
	{
	}

	public void Update(GameTime gameTime)
	{
		if (!FirstTime)
		{
			FirstTime = true;
			OriginalcameraPosition = cameraPosition;
			OriginalcameraHeightPosition = cameraHeightPosition;
		}
		Update_Curser();
		float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
		particleEffectAdd.Update(deltaSeconds);
		particleEffectRemove.Update(deltaSeconds);
		particleEffectSave.Update(deltaSeconds);
		particleEffectExit.Update(deltaSeconds);
		particleEffectStart.Update(deltaSeconds);
		if (!Paused)
		{
			HandleInput();
		}
		if (ExitPosition != new Vector2(0f, 0f))
		{
			particleEffectExit[0].TriggerOffset = ExitPosition;
			particleEffectExit.Trigger(new Vector2(0f, 0f));
		}
		particleEffectStart.Trigger(new Vector2(0f, 0f));
		if (PhysicsPaused)
		{
			_world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
			mainGame.Loaded = true;
			UpdateBrick(gameTime);
			UpdateKinetics(gameTime);
			UpdateClouds(gameTime);
		}
		else
		{
			mainGame.Loaded = true;
		}
	}

	public void Update_Curser()
	{
		if (ObjectTypeMain == 0)
		{
			Curser_String_2 = "0:";
			Curser_String_1 = "0:";
			CurserBrush = CurserDefault;
		}
		else if (ObjectTypeMain == 1)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Block";
				BlockTexture = Content.Load<Texture2D>("Blocks/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Big Block";
				BlockTexture = Content.Load<Texture2D>("Blocks/Big/0");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Beam";
				BlockTexture = Content.Load<Texture2D>("Blocks/Beams/0");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Long Beam";
				BlockTexture = Content.Load<Texture2D>("Blocks/Beams/1");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Ball";
				BlockTexture = Content.Load<Texture2D>("Blocks/Ball/0");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Big Ball";
				BlockTexture = Content.Load<Texture2D>("Blocks/Ball/Big/0");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Chain Bridge";
				BlockTexture = Content.Load<Texture2D>("Blocks/Chains/Bridge/0");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Arrow";
				BlockTexture = Content.Load<Texture2D>("Blocks/Arrow");
			}
			else
			{
				Curser_String_2 = "0: Block";
				BlockTexture = Content.Load<Texture2D>("Blocks/0");
			}
			Curser_String_1 = "1: Blocks";
			CurserBrush = BlockTexture;
		}
		else if (ObjectTypeMain == 2)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Brick";
				BrickTexture = Content.Load<Texture2D>("Bricks/0/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Big Brick";
				BrickTexture = Content.Load<Texture2D>("Bricks/Big/0");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Brick Beam";
				BrickTexture = Content.Load<Texture2D>("Bricks/Beam/0");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Brick Ball";
				BrickTexture = Content.Load<Texture2D>("Bricks/Ball/0");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Stone";
				BrickTexture = Content.Load<Texture2D>("Bricks/Stone/0");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Big Stone";
				BrickTexture = Content.Load<Texture2D>("Bricks/Stone/Big/0");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Stone Beam";
				BrickTexture = Content.Load<Texture2D>("Bricks/Stone/Beams/0");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Stone Ball";
				BrickTexture = Content.Load<Texture2D>("Bricks/Stone/Ball/0");
			}
			else
			{
				Curser_String_2 = "0: Regular";
				BrickTexture = Content.Load<Texture2D>("Bricks/0/0");
			}
			Curser_String_1 = "2: Bricks";
			CurserBrush = BrickTexture;
		}
		else if (ObjectTypeMain == 3)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Mine";
				MineTexture = Content.Load<Texture2D>("Bricks/Mines/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Spiked";
				MineTexture = Content.Load<Texture2D>("Bricks/Spike/0");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Grinder";
				MineTexture = Content.Load<Texture2D>("Bricks/Grinder/0");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Long Spike";
				MineTexture = Content.Load<Texture2D>("Sharps/0");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Long Spike Pop Out";
				MineTexture = Content.Load<Texture2D>("Sharps/2");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Saw Pop Out";
				MineTexture = Content.Load<Texture2D>("Sharps/3");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Needle Dart Launcher";
				MineTexture = Content.Load<Texture2D>("Sharps/4");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Static Saw Blade";
				MineTexture = Content.Load<Texture2D>("Sharps/5");
				if (CurserRotation > 0f)
				{
					HorizontalOrientation = SpriteEffects.None;
				}
				else
				{
					HorizontalOrientation = SpriteEffects.FlipHorizontally;
				}
			}
			else
			{
				Curser_String_2 = "0: Mine";
				MineTexture = Content.Load<Texture2D>("Bricks/Mines/0");
			}
			Curser_String_1 = "3: Hazards";
			CurserBrush = MineTexture;
		}
		else if (ObjectTypeMain == 4)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Moving Platform: Short Horizontal, Starts moving Right";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Moving Platform: Short Horizontal, Starts moving Left";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Moving Platform: Short Vertical, Starts moving Down";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Moving Platform: Short Vertical, Starts moving Up";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Bounce Pad";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/BouncePad");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Force Field: Push";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/ForceField");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Force Field: Multiplier";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/ForceX");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Moving Platform: Short Vertical, Starts moving Up, FAST";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else
			{
				Curser_String_2 = "0: Moving Platform: Short Horizontal, Starts moving Right";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			Curser_String_1 = "4: Platforms";
			CurserBrush = KineticTexture;
		}
		if (ObjectTypeMain == 5)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else
			{
				Curser_String_2 = "1: This is were you leave!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			Curser_String_1 = "5: Exit";
			CurserBrush = ExitTexture;
		}
	}

	public void Update_Curser_Saved()
	{
		if (ObjectTypeMain == 0)
		{
			Curser_String_2 = "0:";
			Curser_String_1 = "0:";
			CurserBrush = CurserDefault;
		}
		else if (ObjectTypeMain == 1)
		{
			Curser_String_1 = "1:  Unused";
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0:";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1:";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2:";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3:";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4:";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "Nothing 5";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Nothing";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Nothing";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				LandTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				LandTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "1";
				LandTexture = CurserDefault;
			}
			ObjectTypeMain = 2;
		}
		else if (ObjectTypeMain == 2)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Regular";
				BrickTexture = Content.Load<Texture2D>("Bricks/0/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Pumpkin Ball";
				BrickTexture = Content.Load<Texture2D>("Bricks/Ball/0");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Mine";
				BrickTexture = Content.Load<Texture2D>("Bricks/Mines/0");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Stone";
				BrickTexture = Content.Load<Texture2D>("Bricks/Stone/0");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Stone Beam";
				BrickTexture = Content.Load<Texture2D>("Bricks/Stone/Beams/0");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Spiked";
				BrickTexture = Content.Load<Texture2D>("Bricks/Spike/0");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Grinder";
				BrickTexture = Content.Load<Texture2D>("Bricks/Grinder/0");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Nothing";
				BrickTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				BrickTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				BrickTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "0: Regular";
				BrickTexture = Content.Load<Texture2D>("Bricks/0/0");
			}
			Curser_String_1 = "2: Bricks";
			CurserBrush = BrickTexture;
		}
		else if (ObjectTypeMain == 3)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Regular";
				BlockTexture = Content.Load<Texture2D>("Blocks/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Beam";
				BlockTexture = Content.Load<Texture2D>("Blocks/Beams/0");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Chain";
				BlockTexture = Content.Load<Texture2D>("Blocks/Chains/0");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Chain Bridge";
				BlockTexture = Content.Load<Texture2D>("Blocks/Chains/Bridge/0");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Nothing";
				BrickTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Nothing";
				BrickTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Nothing";
				BrickTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Nothing";
				BrickTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				BrickTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				BrickTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "0: Regular";
				BlockTexture = Content.Load<Texture2D>("Blocks/0");
			}
			Curser_String_1 = "3: Blocks";
			CurserBrush = BlockTexture;
		}
		else if (ObjectTypeMain == 4)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: BOOOOM! nice.....";
				MineTexture = Content.Load<Texture2D>("Bricks/Mines/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				MineTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				MineTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "0: BOOOOM! nice.....";
				MineTexture = Content.Load<Texture2D>("Bricks/Mines/0");
			}
			Curser_String_1 = "4: Mines";
			CurserBrush = MineTexture;
		}
		else if (ObjectTypeMain == 5)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Long Spike";
				SharpTexture = Content.Load<Texture2D>("Sharps/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Short Spike";
				SharpTexture = Content.Load<Texture2D>("Sharps/1");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Long Spike Pop Out";
				SharpTexture = Content.Load<Texture2D>("Sharps/2");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Saw Pop Out";
				SharpTexture = Content.Load<Texture2D>("Sharps/3");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Needle Dart Launcher";
				SharpTexture = Content.Load<Texture2D>("Sharps/4");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Saw Blade";
				SharpTexture = Content.Load<Texture2D>("Sharps/5");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Nothing";
				SharpTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Nothing";
				SharpTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				SharpTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				SharpTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "0: Long Spike";
				SharpTexture = Content.Load<Texture2D>("Sharps/0");
			}
			CurserBrush = SharpTexture;
			Curser_String_1 = "5: Sharps";
		}
		else if (ObjectTypeMain == 6)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Moving Platform: Short Horizontal, Starts moving Right";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Moving Platform: Short Horizontal, Starts moving Left";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Moving Platform: Short Horizontal, Starts moving Right, FAST";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Moving Platform: Short Horizontal, Starts moving Left, FAST";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Moving Platform: Short Vertical, Starts moving Down";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Moving Platform: Short Vertical, Starts moving Up";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Moving Platform: Short Vertical, Starts moving Down, FAST";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Moving Platform: Short Vertical, Starts moving Up, FAST";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				KineticTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "0: Moving Platform: Short Horizontal, Starts moving Right";
				KineticTexture = Content.Load<Texture2D>("Kinetics/Platforms/0");
			}
			Curser_String_1 = "6: Kinetics";
			CurserBrush = KineticTexture;
		}
		else if (ObjectTypeMain == 7)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Rat";
				EnemyTexture = Content.Load<Texture2D>("Sprites/Enemy/0/body");
			}
			if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Bat";
				EnemyTexture = Content.Load<Texture2D>("Sprites/Enemy/1/body");
			}
			if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "2: Were Limer";
				EnemyTexture = Content.Load<Texture2D>("Sprites/Enemy/2/body");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Nothing";
				EnemyTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Nothing";
				EnemyTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Nothing";
				EnemyTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Nothing";
				EnemyTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Nothing";
				EnemyTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				EnemyTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				EnemyTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "0: Rat";
				EnemyTexture = Content.Load<Texture2D>("Sprites/Enemy/0/body");
			}
			Curser_String_1 = "7: Enemys";
			CurserBrush = EnemyTexture;
		}
		if (ObjectTypeMain == 8)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "0: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "1: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "3: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "4: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "5: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "6: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "7: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "8: Nothing";
				NothingTexture = CurserDefault;
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "9: Nothing";
				NothingTexture = CurserDefault;
			}
			else
			{
				Curser_String_2 = "0: Nothing";
				NothingTexture = CurserDefault;
			}
			Curser_String_1 = "8: Nothing";
			CurserBrush = EnemyTexture;
			ObjectTypeMain = 9;
		}
		if (ObjectTypeMain == 9)
		{
			if (ObjectTypeSubMain == "0")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "1")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "2")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "3")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "4")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "5")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "6")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "7")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "8")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else if (ObjectTypeSubMain == "9")
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			else
			{
				Curser_String_2 = "1: This is were you leave, stupid!";
				ExitTexture = Content.Load<Texture2D>("LevelBuilder/Exit");
			}
			Curser_String_1 = "9: Exit";
			CurserBrush = EnemyTexture;
		}
	}

	private void PauseMenuInput(PlayerIndex Player, GameTime gameTime)
	{
		GamePadState state = GamePad.GetState(Player);
		Keyboard.GetState();
		if (state.Buttons.A == ButtonState.Pressed && !PauseMenuButtonAWasPressed)
		{
			mainGame.MenuClickSound.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			PauseMenuButtonAWasPressed = true;
			if (PauseMenuIndexer == 0)
			{
				Paused = false;
			}
			if (PauseMenuIndexer == 1)
			{
				if (Dueling)
				{
					Dueling = false;
				}
				else
				{
					Dueling = true;
				}
			}
			if (PauseMenuIndexer == 2)
			{
				if (Xbox)
				{
					if (LoadLevelIndexer < mainGame.GauntletRunLevelIndexerEnd)
					{
						if (AllLevelNames.Unlocked_Gauntlet_Run[LoadLevelIndexer + 1])
						{
							LevelPath = "0" + LoadLevelIndexer + ".txt";
							mainGame.levelBuilderPath = LevelPath;
							mainGame.LoadLevelIndexer = LoadLevelIndexer;
							mainGame.LoadNewLevelInBuilder();
						}
					}
					else if (LoadLevelIndexer < mainGame.DuelingLevelIndexerEnd)
					{
						if (AllLevelNames.Unlocked_Dueling[LoadLevelIndexer - mainGame.GauntletRunLevelIndexerEnd])
						{
							LevelPath = "1" + (LoadLevelIndexer - mainGame.GauntletRunLevelIndexerEnd) + ".txt";
							mainGame.levelBuilderPath = LevelPath;
							mainGame.LoadLevelIndexer = LoadLevelIndexer;
							mainGame.LoadNewLevelInBuilder();
						}
					}
					else if (LoadLevelIndexer >= mainGame.DuelingLevelIndexerEnd && AllLevelNames.Unlocked_Custom[LoadLevelIndexer - mainGame.DuelingLevelIndexerEnd])
					{
						LevelPath = "2" + (LoadLevelIndexer - mainGame.DuelingLevelIndexerEnd) + ".txt";
						mainGame.levelBuilderPath = LevelPath;
						mainGame.LoadLevelIndexer = LoadLevelIndexer;
						mainGame.LoadNewLevelInBuilder();
					}
				}
				else if (LoadLevelIndexer < mainGame.GauntletRunLevelIndexerEnd)
				{
					if (AllLevelNames.Unlocked_Gauntlet_Run[LoadLevelIndexer + 1])
					{
						LevelPath = "Content/LevelBuilder/0/" + LoadLevelIndexer + ".txt";
						mainGame.levelBuilderPath = LevelPath;
						mainGame.LoadLevelIndexer = LoadLevelIndexer;
						mainGame.LoadNewLevelInBuilder();
					}
				}
				else if (LoadLevelIndexer < mainGame.DuelingLevelIndexerEnd)
				{
					if (AllLevelNames.Unlocked_Dueling[LoadLevelIndexer - mainGame.GauntletRunLevelIndexerEnd])
					{
						LevelPath = "Content/LevelBuilder/1/" + (LoadLevelIndexer - mainGame.GauntletRunLevelIndexerEnd) + ".txt";
						mainGame.levelBuilderPath = LevelPath;
						mainGame.LoadLevelIndexer = LoadLevelIndexer;
						mainGame.LoadNewLevelInBuilder();
					}
				}
				else if (LoadLevelIndexer >= mainGame.DuelingLevelIndexerEnd && AllLevelNames.Unlocked_Custom[LoadLevelIndexer - mainGame.DuelingLevelIndexerEnd])
				{
					LevelPath = "Content/LevelBuilder/2/" + (LoadLevelIndexer - mainGame.DuelingLevelIndexerEnd) + ".txt";
					mainGame.levelBuilderPath = LevelPath;
					mainGame.LoadLevelIndexer = LoadLevelIndexer;
					mainGame.LoadNewLevelInBuilder();
				}
			}
			if (PauseMenuIndexer == 3)
			{
				mainGame.InLevelBuilderMode = false;
				mainGame.InPauseMode = false;
				mainGame.InMainMenuMode = true;
				mainGame.MainMenuFadeIn = true;
				mainGame.MainMenuFadeOut = false;
				mainGame.StartLevelBuilder = false;
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
			mainGame.MenuClickSound.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			Paused = false;
			mainGame.InPauseMode = false;
		}
		if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
		{
			mainGame.MenuMoveSound.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
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
			mainGame.MenuMoveSound.Play(mainGame.Sound_Effect_Volume, 0f, 0f);
			P1DpadDownpressed = true;
			PauseMenuIndexer++;
		}
		P1DpadDownWaspressed = P1DpadDownpressed;
		if (state.DPad.Down == ButtonState.Released)
		{
			P1DpadDownpressed = false;
		}
		if (PauseMenuIndexer > PauseMenuIndexerMax)
		{
			PauseMenuIndexer = 0;
		}
		if (PauseMenuIndexer < 0)
		{
			PauseMenuIndexer = PauseMenuIndexerMax;
		}
		if (PauseMenuIndexer == 2)
		{
			if ((!DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed) || (!DpadRightWaspressed && state.ThumbSticks.Left.X < -0.5f))
			{
				mainGame.MenuMoveSound.Play(mainGame.Sound_Effect_Volume, -0.5f, 0f);
				DpadRightpressed = true;
				LoadLevelIndexer++;
			}
			DpadRightWaspressed = DpadRightpressed;
			if (state.DPad.Right == ButtonState.Released)
			{
				DpadRightpressed = false;
			}
			if ((!DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed) || (!DpadLeftWaspressed && state.ThumbSticks.Left.X > 0.5f))
			{
				mainGame.MenuMoveSound.Play(mainGame.Music_Volume, -0.5f, 0f);
				DpadLeftpressed = true;
				LoadLevelIndexer--;
			}
			DpadLeftWaspressed = DpadLeftpressed;
			if (state.DPad.Left == ButtonState.Released)
			{
				DpadLeftpressed = false;
			}
			if (LoadLevelIndexer > LoadLevelIndexerMax)
			{
				LoadLevelIndexer = 0;
			}
			if (LoadLevelIndexer < 0)
			{
				LoadLevelIndexer = LoadLevelIndexerMax;
			}
		}
	}

	private void HandleInput()
	{
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(PlayerInControl);
		if (!Saving)
		{
			bool flag = state2.IsButtonDown(Buttons.Start);
			if (!wasContinue1Pressed && flag)
			{
				if (Paused)
				{
					Paused = false;
				}
				else
				{
					Paused = true;
				}
			}
			wasContinue1Pressed = flag;
		}
		mouse = Mouse.GetState();
		MousePosition = new Vector2(mouse.X, mouse.Y);
		LeftJoyStickPosition = new Vector2(state2.ThumbSticks.Left.X, state2.ThumbSticks.Left.Y);
		RightJoyStickPosition = new Vector2(state2.ThumbSticks.Right.X, state2.ThumbSticks.Right.Y);
		float num = 10f / ZoomScaler;
		float num2 = 20f / ZoomScaler;
		CurserPosition += LeftJoyStickPosition * new Vector2(num, 0f - num);
		if (CurserPosition.X > (float)mainGame.graphics.GraphicsDevice.Viewport.Width / newScaler)
		{
			CurserPosition.X = (float)mainGame.graphics.GraphicsDevice.Viewport.Width / newScaler;
		}
		else if (CurserPosition.X < 0f)
		{
			CurserPosition.X = 0f;
		}
		if (CurserPosition.Y > (float)mainGame.graphics.GraphicsDevice.Viewport.Height / newScaler)
		{
			CurserPosition.Y = (float)mainGame.graphics.GraphicsDevice.Viewport.Height / newScaler;
		}
		else if (CurserPosition.Y < 0f)
		{
			CurserPosition.Y = 0f;
		}
		CameraPositionNewX += RightJoyStickPosition.X * num2;
		CameraPositionNewY += RightJoyStickPosition.Y * (0f - num2);
		if (state.IsKeyDown(Keys.Left))
		{
			CameraPositionNewX -= 10f;
		}
		if (state.IsKeyDown(Keys.Right))
		{
			CameraPositionNewX += 10f;
		}
		if (state.IsKeyDown(Keys.Up))
		{
			CameraPositionNewY -= 10f;
		}
		if (state.IsKeyDown(Keys.Down))
		{
			CameraPositionNewY += 10f;
		}
		if (!WasZoomInPressed && state2.Buttons.RightShoulder == ButtonState.Pressed)
		{
			ZoomScaler += ZoomScaler_Step;
			mainGame.MenuClickSound.Play(MathHelper.Clamp(mainGame.Music_Volume / 100f, 0f, 1f), 1f, 0f);
		}
		if (state2.Buttons.RightShoulder == ButtonState.Released)
		{
			WasZoomInPressed = false;
		}
		if (!WasZoomOutPressed && state2.Buttons.LeftShoulder == ButtonState.Pressed)
		{
			ZoomScaler -= ZoomScaler_Step;
			mainGame.MenuClickSound.Play(MathHelper.Clamp(mainGame.Music_Volume / 100f, 0f, 1f), 1f, 0f);
		}
		if (state2.Buttons.LeftShoulder == ButtonState.Released)
		{
			WasZoomOutPressed = false;
		}
		if (mainGame.IsHD)
		{
			ZoomScaler = MathHelper.Clamp(ZoomScaler, 0.5f, 1.4f);
		}
		else
		{
			ZoomScaler = MathHelper.Clamp(ZoomScaler, 0.5f, 2.09f);
		}
		if (!WasSavePressed && state2.Buttons.X == ButtonState.Pressed)
		{
			mainGame.MenuClickSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			WasSavePressed = true;
			Saving = true;
			if (Xbox)
			{
				SaveDataXbox(LevelData, SavedLevel_Name);
				particleEffectSave.Trigger(new Vector2(0f, 0f));
			}
			else
			{
				SaveDataWindows(LevelData, SavedLevel_Name);
				particleEffectSave.Trigger(new Vector2(0f, 0f));
			}
		}
		if (state2.Buttons.X == ButtonState.Released)
		{
			WasSavePressed = false;
		}
		if (!WasPhysicsPressed && state2.Buttons.Y == ButtonState.Pressed)
		{
			mainGame.MenuClickSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			WasPhysicsPressed = true;
			if (!PhysicsPaused)
			{
				PhysicsPaused = true;
			}
			else
			{
				PhysicsPaused = false;
			}
		}
		if (state2.Buttons.Y == ButtonState.Released)
		{
			WasPhysicsPressed = false;
		}
		if (!WasAddPressed && state2.Buttons.A == ButtonState.Pressed)
		{
			mainGame.MenuClickSound.Play(mainGame.Sound_Effect_Volume, 0.5f, 0f);
			WasAddPressed = true;
			Add = state2.Buttons.A == ButtonState.Pressed;
			MouseLeftIsPressed = true;
			AddData(ObjectTypeMain, ObjectTypeSubMain, CurserPosition, ObjectCount);
		}
		if (state2.Buttons.A == ButtonState.Released)
		{
			WasAddPressed = false;
		}
		if (!Add)
		{
			MouseLeftIsPressed = false;
		}
		bool flag2 = false;
		if (!flag2)
		{
			flag2 = state2.Buttons.B == ButtonState.Pressed;
		}
		if (flag2)
		{
			mainGame.MenuClickSound.Play(MathHelper.Clamp(mainGame.Music_Volume / 100f, 0f, 1f), -0.5f, 0f);
			MouseRightIsPressed = true;
			RemoveData2(ObjectTypeMain, "0", CurserPosition, ObjectCount);
		}
		if (!flag2)
		{
			MouseRightIsPressed = false;
		}
		if (state.IsKeyDown(Keys.OemPeriod) || state2.Triggers.Right > 0.1f)
		{
			CurserRotation += state2.Triggers.Right * 0.03f;
			mainGame.MenuMoveSound.Play(MathHelper.Clamp(mainGame.Music_Volume / 100f, 0f, 1f), 1f, 0f);
		}
		if (state.IsKeyDown(Keys.OemComma) || state2.Triggers.Left > 0.1f)
		{
			CurserRotation -= state2.Triggers.Left * 0.03f;
			mainGame.MenuMoveSound.Play(MathHelper.Clamp(mainGame.Music_Volume / 100f, 0f, 1f), 1f, 0f);
		}
		bool flag3 = state2.DPad.Up == ButtonState.Pressed;
		bool flag4 = state2.DPad.Down == ButtonState.Pressed;
		bool flag5 = state2.DPad.Left == ButtonState.Pressed;
		bool flag6 = state2.DPad.Right == ButtonState.Pressed;
		if (flag3 && !indexWasUp)
		{
			ObjectTypeMain--;
			CurserRotation = 0f;
			if (ObjectTypeMain == -1)
			{
				ObjectTypeMain = 5;
			}
			indexWasUp = true;
		}
		if (flag4 && !indexWasDown)
		{
			ObjectTypeMain++;
			CurserRotation = 0f;
			if (ObjectTypeMain == 6)
			{
				ObjectTypeMain = 0;
			}
			indexWasDown = true;
		}
		if (flag5 && !indexWasLeft)
		{
			if (ObjectTypeSubMain == "0")
			{
				ObjectTypeSubMain = "7";
				indexWasLeft = true;
			}
			else if (ObjectTypeSubMain == "1")
			{
				ObjectTypeSubMain = "0";
				indexWasLeft = true;
			}
			else if (ObjectTypeSubMain == "2")
			{
				ObjectTypeSubMain = "1";
				indexWasLeft = true;
			}
			else if (ObjectTypeSubMain == "3")
			{
				ObjectTypeSubMain = "2";
				indexWasLeft = true;
			}
			else if (ObjectTypeSubMain == "4")
			{
				ObjectTypeSubMain = "3";
				indexWasLeft = true;
			}
			else if (ObjectTypeSubMain == "5")
			{
				ObjectTypeSubMain = "4";
				indexWasLeft = true;
			}
			else if (ObjectTypeSubMain == "6")
			{
				ObjectTypeSubMain = "5";
				indexWasLeft = true;
			}
			else if (ObjectTypeSubMain == "7")
			{
				ObjectTypeSubMain = "6";
				indexWasLeft = true;
			}
			else
			{
				ObjectTypeSubMain = "0";
				indexWasLeft = true;
			}
			indexWasLeft = true;
		}
		if (flag6 && !indexWasRight)
		{
			if (ObjectTypeSubMain == "0")
			{
				ObjectTypeSubMain = "1";
				indexWasRight = true;
			}
			else if (ObjectTypeSubMain == "1")
			{
				ObjectTypeSubMain = "2";
				indexWasRight = true;
			}
			else if (ObjectTypeSubMain == "2")
			{
				ObjectTypeSubMain = "3";
				indexWasRight = true;
			}
			else if (ObjectTypeSubMain == "3")
			{
				ObjectTypeSubMain = "4";
				indexWasRight = true;
			}
			else if (ObjectTypeSubMain == "4")
			{
				ObjectTypeSubMain = "5";
				indexWasRight = true;
			}
			else if (ObjectTypeSubMain == "5")
			{
				ObjectTypeSubMain = "6";
				indexWasRight = true;
			}
			else if (ObjectTypeSubMain == "6")
			{
				ObjectTypeSubMain = "7";
				indexWasRight = true;
			}
			else if (ObjectTypeSubMain == "7")
			{
				ObjectTypeSubMain = "0";
				indexWasRight = true;
			}
			else
			{
				ObjectTypeSubMain = "0";
				indexWasRight = true;
			}
			indexWasRight = true;
		}
		if (!flag3)
		{
			indexWasUp = false;
		}
		if (!flag4)
		{
			indexWasDown = false;
		}
		if (!flag5)
		{
			indexWasLeft = false;
		}
		if (!flag6)
		{
			indexWasRight = false;
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

	private void UpdateBrick(GameTime gameTime)
	{
		for (int i = 0; i < Bricks.Count; i++)
		{
			Brick brick = Bricks[i];
			brick.Update(gameTime, _world);
		}
	}

	private void UpdateBlocks(GameTime gameTime)
	{
		for (int i = 0; i < Blocks.Count; i++)
		{
			Blocks blocks = Blocks[i];
			blocks.Update(gameTime, _world);
		}
	}

	private void UpdateKinetics(GameTime gameTime)
	{
		for (int i = 0; i < Kinetics.Count; i++)
		{
			Kinetics kinetics = Kinetics[i];
			kinetics.Update(gameTime, _world);
		}
	}

	public void Draw(PlatformerGame Game, GameTime gameTime, SpriteBatch spriteBatch)
	{
		newScaler = mainGame.Global_Scaler * ZoomScaler;
		ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
		Matrix transformMatrix = Matrix.CreateTranslation(0f - cameraPosition, 0f - cameraHeightPosition, 0f);
		Matrix transformMatrix2 = Matrix.CreateTranslation(1f, 0f - cameraHeightPosition, 0f);
		Matrix matrix = Matrix.CreateScale(newScaler);
		transformMatrix *= matrix;
		transformMatrix2 *= matrix;
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		layers[0].Draw(spriteBatch, this, null, cameraPosition, cameraHeightPosition, BackgroundColor, new Vector2(0f, -200f));
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, transformMatrix);
		renderer.RenderEffect(particleEffectStart, spriteBatch);
		renderer.RenderEffect(particleEffectExit, spriteBatch);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, transformMatrix2);
		layers[1].Draw(spriteBatch, this, null, cameraPosition, cameraHeightPosition, Color.White, new Vector2(0f, 286f));
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, transformMatrix);
		l = 0;
		foreach (Lands land in Lands)
		{
			if (land.LandBody != null)
			{
				_ = land.LandBody.FixtureList;
			}
		}
		b = 0;
		foreach (Blocks block in Blocks)
		{
			if (block.BlockBody != null && block.BlockBody.Body != null && block.BlockBody.Body.FixtureList != null)
			{
				int i = b + 1;
				block.Draw(gameTime, spriteBatch, i);
			}
		}
		sh = 0;
		foreach (Sharps sharp in Sharps)
		{
			if (sharp.SharpBody != null && sharp.SharpBody.Body != null && sharp.SharpBody.Body.FixtureList != null)
			{
				int i2 = sh + 1;
				sharp.Draw(gameTime, spriteBatch, i2);
			}
		}
		em = 0;
		foreach (Enemy enemy in Enemys)
		{
			if (enemy._bodyBody != null && enemy._bodyBody.Body != null && enemy._bodyBody.Body.FixtureList != null)
			{
				enemy.Draw(gameTime, spriteBatch, mainGame);
			}
		}
		S = 0;
		foreach (Brick brick in Bricks)
		{
			if (brick.BrickBody != null && brick.BrickBody.Body != null && brick.BrickBody.Body.FixtureList != null)
			{
				int i3 = S + 1;
				brick.Draw(gameTime, spriteBatch, i3);
			}
		}
		K = 0;
		foreach (Kinetics kinetic in Kinetics)
		{
			if (kinetic.KineticBody != null && kinetic.KineticBody.Body != null && kinetic.KineticBody.Body.FixtureList != null)
			{
				int i4 = K + 1;
				kinetic.Draw(gameTime, spriteBatch, i4);
			}
		}
		l = 0;
		foreach (Lands land2 in Lands)
		{
			if (land2.LandBody != null && land2.LandBody.FixtureList != null)
			{
				spriteBatch.Draw(CenterDot, land2.LandBody.Position * PhysicsScaleUp, null, new Color(255, 255, 255), 0f, new Vector2(CenterDot.Width / 2, CenterDot.Height / 2), 1f, SpriteEffects.None, 1f);
			}
		}
		b = 0;
		foreach (Blocks block2 in Blocks)
		{
			if (block2.BlockBody != null && block2.BlockBody.Body != null && block2.BlockBody.Body.FixtureList != null)
			{
				spriteBatch.Draw(CenterDot, block2.BlockBody.Body.Position * PhysicsScaleUp, null, new Color(255, 255, 255), 0f, new Vector2(CenterDot.Width / 2, CenterDot.Height / 2), 1f, SpriteEffects.None, 1f);
			}
		}
		sh = 0;
		foreach (Sharps sharp2 in Sharps)
		{
			if (sharp2.SharpBody != null && sharp2.SharpBody.Body != null && sharp2.SharpBody.Body.FixtureList != null)
			{
				spriteBatch.Draw(CenterDot, sharp2.SharpBody.Body.Position * PhysicsScaleUp, null, new Color(255, 255, 255), 0f, new Vector2(CenterDot.Width / 2, CenterDot.Height / 2), 1f, SpriteEffects.None, 1f);
			}
		}
		em = 0;
		foreach (Enemy enemy2 in Enemys)
		{
			if (enemy2._bodyBody != null && enemy2._bodyBody.Body != null && enemy2._bodyBody.Body.FixtureList != null)
			{
				spriteBatch.Draw(CenterDot, enemy2._bodyBody.Body.Position * PhysicsScaleUp, null, new Color(255, 255, 255), 0f, new Vector2(CenterDot.Width / 2, CenterDot.Height / 2), 1f, SpriteEffects.None, 1f);
			}
		}
		S = 0;
		foreach (Brick brick2 in Bricks)
		{
			if (brick2.BrickBody != null && brick2.BrickBody.Body != null && brick2.BrickBody.Body.FixtureList != null)
			{
				spriteBatch.Draw(CenterDot, brick2.BrickBody.Body.Position * PhysicsScaleUp, null, new Color(255, 255, 255), 0f, new Vector2(CenterDot.Width / 2, CenterDot.Height / 2), 1f, SpriteEffects.None, 1f);
			}
		}
		K = 0;
		foreach (Kinetics kinetic2 in Kinetics)
		{
			if (kinetic2.KineticBody != null && kinetic2.KineticBody.Body != null && kinetic2.KineticBody.Body.FixtureList != null)
			{
				spriteBatch.Draw(CenterDot, kinetic2.Position, null, new Color(255, 255, 255), 0f, new Vector2(CenterDot.Width / 2, CenterDot.Height / 2), 1f, SpriteEffects.None, 1f);
			}
		}
		spriteBatch.Draw(CurserBrush, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition), null, new Color(255f, 255f, 255f, 0.5f), CurserRotation, new Vector2(CurserBrush.Width / 2, CurserBrush.Height / 2), 1f, HorizontalOrientation, 1f);
		spriteBatch.Draw(CurserBrushCrossHairs, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition), null, new Color(255f, 255f, 255f, 1f), CurserRotation, new Vector2(CurserBrushCrossHairs.Width / 2, CurserBrushCrossHairs.Height / 2), 0.5f, SpriteEffects.None, 1f);
		DrawShadowedString(spriteBatch, CurserFont, Curser_String_1, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition) + new Vector2(50f, 0f), Color.Red);
		DrawShadowedString(spriteBatch, CurserFont, Curser_String_2, CurserPosition + new Vector2(cameraPosition, cameraHeightPosition) + new Vector2(50f, 30f), Color.Red);
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
		renderer.RenderEffect(particleEffectAdd, spriteBatch);
		renderer.RenderEffect(particleEffectRemove, spriteBatch);
		spriteBatch.End();
		Matrix transformMatrix3 = Matrix.CreateScale(0.65f);
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, transformMatrix3);
		renderer.RenderEffect(particleEffectSave, spriteBatch);
		spriteBatch.End();
		Matrix transformMatrix4 = Matrix.CreateTranslation(mainGame.True_Screen_Center.X, mainGame.True_Screen_Center.Y, 0f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, transformMatrix4);
		Vector2 vector = new Vector2(140f * mainGame.Global_Scaler, 80f * mainGame.Global_Scaler);
		if (!mainGame.IsHD)
		{
			vector = new Vector2(200f * mainGame.Global_Scaler, 160f * mainGame.Global_Scaler);
		}
		float x = vector.X + (float)(-mainGame.graphics.GraphicsDevice.Viewport.Width) * 0.85f / 2f;
		float y = (float)mainGame.graphics.GraphicsDevice.Viewport.Height * 0.8f / 2f - vector.Y;
		float x2 = 0f;
		float y2 = vector.Y + ((float)(-mainGame.graphics.GraphicsDevice.Viewport.Height / 2) + 125f * mainGame.Global_Scaler);
		int maxValue = 2;
		spriteBatch.Draw(Instruction_Brush, new Vector2(x2, y2), null, new Color(255f, 255f, 255f, 0.85f), 0f, new Vector2(Instruction_Brush.Width / 2, Instruction_Brush.Height / 2), 1f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		if (ObjectCount < 200)
		{
			DrawShadowedString_Hud(spriteBatch, HudFont, "Object Count = " + ObjectCount, new Vector2(x, y), Color.Red);
		}
		else if (ObjectCount < 275)
		{
			float num = Pulsate(gameTime, 10f, 100f, 255f);
			if (num > 255f)
			{
				num = 255f;
			}
			else if (num < 0f)
			{
				num = 0f;
			}
			DrawShadowedString_Hud(color: new Color(255, 0, 0, (byte)num), spriteBatch: spriteBatch, font: HudFont, value: "Object Count = " + ObjectCount + "  This may run slowly with 4 players.", position: new Vector2(x, y) + new Vector2(mainGame.random.Next(maxValue), mainGame.random.Next(maxValue)));
		}
		else if (ObjectCount < 325)
		{
			float num2 = Pulsate(gameTime, 40f, 100f, 255f);
			if (num2 > 255f)
			{
				num2 = 255f;
			}
			else if (num2 < 0f)
			{
				num2 = 0f;
			}
			DrawShadowedString_Hud(color: new Color(255, 0, 0, (byte)num2), spriteBatch: spriteBatch, font: HudFont, value: "Object Count = " + ObjectCount + "  This may run slowly with 2 players.", position: new Vector2(x, y) + new Vector2(mainGame.random.Next(maxValue), mainGame.random.Next(maxValue)));
		}
		else
		{
			float num3 = Pulsate(gameTime, 90f, 100f, 255f);
			if (num3 > 255f)
			{
				num3 = 255f;
			}
			else if (num3 < 0f)
			{
				num3 = 0f;
			}
			DrawShadowedString_Hud(color: new Color(255, 0, 0, (byte)num3), spriteBatch: spriteBatch, font: HudFont, value: "Object Count = " + ObjectCount + "  Maximum limit reached!!", position: new Vector2(x, y) + new Vector2(mainGame.random.Next(maxValue), mainGame.random.Next(maxValue)));
		}
		spriteBatch.End();
		if (Paused && !Saving)
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
			DrawPauseMenu(spriteBatch);
		}
	}

	private static float Pulsate(GameTime gameTime, float speed, float min, float max)
	{
		double a = gameTime.TotalGameTime.TotalSeconds * (double)speed;
		return min + ((float)Math.Sin(a) + 1f) / 2f * (max - min);
	}

	public void DrawPauseMenu(SpriteBatch spriteBatch)
	{
		Matrix transformMatrix = Matrix.CreateTranslation(mainGame.True_Screen_Center.X, mainGame.True_Screen_Center.Y, 0f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, transformMatrix);
		int num = PauseMenuBackgroundStripTexture.Width * 20;
		float y = 0f;
		Vector2 vector = new Vector2(-1000f, -500f);
		for (int i = 0; i < 20; i++)
		{
			spriteBatch.Draw(PauseMenuBackgroundStripTexture, new Vector2(num * i, y) + vector, null, Color.White, 0f, new Vector2(PauseMenuBackgroundStripTexture.Width / 2, PauseMenuBackgroundStripTexture.Width / 2), 20f, SpriteEffects.None, 1f);
		}
		spriteBatch.Draw(PauseMenuControllerLayoutTexture, new Vector2(-200f * mainGame.Global_Scaler, 0f), null, Color.White, 0f, new Vector2(PauseMenuTexture.Width / 2, PauseMenuTexture.Height / 2), 1f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		spriteBatch.Draw(PauseMenuSideBarTexture, new Vector2(650f * mainGame.Global_Scaler, 0f), null, Color.White, 0f, new Vector2(PauseMenuSideBarTexture.Width / 2, PauseMenuSideBarTexture.Height / 2), 1f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		Texture2D texture2D = mainGame.Player1MenuTexture;
		if (PlayerIndexer_Pub == PlayerIndex.One)
		{
			texture2D = mainGame.Player1MenuTexture;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Two)
		{
			texture2D = mainGame.Player2MenuTexture;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Three)
		{
			texture2D = mainGame.Player3MenuTexture;
		}
		if (PlayerIndexer_Pub == PlayerIndex.Four)
		{
			texture2D = mainGame.Player4MenuTexture;
		}
		if (mainGame.IsHD)
		{
			spriteBatch.Draw(texture2D, new Vector2(-20f, -600f * mainGame.Global_Scaler * mainGame.Global_Scaler), null, Color.White, 0f, new Vector2(texture2D.Width / 2, texture2D.Height / 2), 3f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		}
		else
		{
			spriteBatch.Draw(texture2D, new Vector2(-20f, -1200f * mainGame.Global_Scaler * mainGame.Global_Scaler), null, Color.White, 0f, new Vector2(texture2D.Width / 2, texture2D.Height / 2), 3f * mainGame.Global_Scaler, SpriteEffects.None, 0f);
		}
		int maxValue = 2;
		if (PhysicsPaused)
		{
			_ = Color.Red;
		}
		else
		{
			_ = Color.Black;
		}
		_ = Color.Red;
		Color red = Color.Red;
		if (PauseMenuIndexer == 0)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Resume", new Vector2(-15f + (0f - PauseFont.MeasureString("Resume").X / 4f), -150f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Resume", new Vector2(-15f + (0f - PauseFont.MeasureString("Resume").X / 4f), -150f * mainGame.Global_Scaler), red);
		}
		if (PauseMenuIndexer == 1)
		{
			if (Dueling)
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, "Dueling", new Vector2(-15f + (0f - PauseFont.MeasureString("Resume").X / 4f), -50f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
			}
			else
			{
				DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Dueling", new Vector2(-15f + (0f - PauseFont.MeasureString("Resume").X / 4f), -50f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
			}
		}
		else if (Dueling)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Dueling", new Vector2(-15f + (0f - PauseFont.MeasureString("Resume").X / 4f), -50f * mainGame.Global_Scaler), Color.Red);
		}
		else
		{
			DrawGlowInvertedString_Pause(spriteBatch, PauseFont, "Dueling", new Vector2(-15f + (0f - PauseFont.MeasureString("Resume").X / 4f), -50f * mainGame.Global_Scaler), Color.Red);
		}
		if (PauseMenuIndexer == 2)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Load", new Vector2(-15f + (0f - PauseFont.MeasureString("Load").X / 4f), 50f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red);
			if (LoadLevelIndexer < mainGame.GauntletRunLevelIndexerEnd)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[LoadLevelIndexer + 1])
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red);
				}
				else
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.DarkRed);
				}
			}
			else if (LoadLevelIndexer < mainGame.DuelingLevelIndexerEnd)
			{
				if (AllLevelNames.Unlocked_Dueling[LoadLevelIndexer - mainGame.GauntletRunLevelIndexerEnd])
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red);
				}
				else
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.DarkRed);
				}
			}
			else if (AllLevelNames.Unlocked_Custom[LoadLevelIndexer - mainGame.DuelingLevelIndexerEnd])
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.DarkRed);
			}
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Load", new Vector2(-15f + (0f - PauseFont.MeasureString("Load").X / 4f), 50f * mainGame.Global_Scaler), Color.Red);
			if (LoadLevelIndexer < mainGame.GauntletRunLevelIndexerEnd)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[LoadLevelIndexer + 1])
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler), red);
				}
				else
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler), Color.DarkRed);
				}
			}
			else if (LoadLevelIndexer < mainGame.DuelingLevelIndexerEnd)
			{
				if (AllLevelNames.Unlocked_Dueling[LoadLevelIndexer - mainGame.GauntletRunLevelIndexerEnd])
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler), red);
				}
				else
				{
					DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler), Color.DarkRed);
				}
			}
			else if (AllLevelNames.Unlocked_Custom[LoadLevelIndexer - mainGame.DuelingLevelIndexerEnd])
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler), red);
			}
			else
			{
				DrawShadowedString_Pause(spriteBatch, PauseFont, AllLevelNames.LevelName[LoadLevelIndexer].ToString(), new Vector2(-15f + (0f - PauseFont.MeasureString(AllLevelNames.LevelName[LoadLevelIndexer].ToString()).X / 4f), 150f * mainGame.Global_Scaler), Color.DarkRed);
			}
		}
		if (PauseMenuIndexer == 3)
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Main Manu", new Vector2(-10f + (0f - PauseFont.MeasureString("Main_Manu").X / 4f), 250f * mainGame.Global_Scaler) + new Vector2(random.Next(maxValue), random.Next(maxValue)), red);
		}
		else
		{
			DrawShadowedString_Pause(spriteBatch, PauseFont, "Main Manu", new Vector2(-10f + (0f - PauseFont.MeasureString("Main_Manu").X / 4f), 250f * mainGame.Global_Scaler), Color.Red);
		}
		spriteBatch.End();
	}

	private void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black);
		spriteBatch.DrawString(font, value, position, color);
	}

	private void DrawShadowedString_Hud(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), 0.75f, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), 0.75f, SpriteEffects.None, 1f);
	}

	private void DrawShadowedString_Pause(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawShadowedString_Pause_Smaller(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), 1f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), 1f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawGlowInvertedString_Pause(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 0f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(-1f, 0f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, 1f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, -1f), Color.White, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position, Color.Black, 0f, new Vector2(0f, 0f), 1.5f * mainGame.Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawGlowInvertedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 0f), Color.White);
		spriteBatch.DrawString(font, value, position + new Vector2(-1f, 0f), Color.White);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, 1f), Color.White);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, -1f), Color.White);
		spriteBatch.DrawString(font, value, position, Color.Black);
	}

	private void ScrollCamera(Viewport viewport)
	{
		float num = (float)viewport.Width * 0.35f;
		float num2 = cameraPosition + num;
		float num3 = cameraPosition + (float)viewport.Width - num;
		float num4 = 0f;
		if (CameraPositionNewX < num2)
		{
			num4 = CameraPositionNewX - num2;
		}
		else if (CameraPositionNewX > num3)
		{
			num4 = CameraPositionNewX - num3;
		}
		maxCameraPosition = 100000f;
		_ = mainGame.Global_Scaler;
		cameraPosition = MathHelper.Clamp(cameraPosition + num4, 0f - maxCameraPosition, maxCameraPosition);
		float num5 = (float)viewport.Height * 0.35f;
		float num6 = cameraHeightPosition + num5;
		float num7 = cameraHeightPosition + (float)viewport.Height - num5;
		float num8 = 0f;
		if (CameraPositionNewY < num6)
		{
			num8 = CameraPositionNewY - num6;
		}
		else if (CameraPositionNewY > num7)
		{
			num8 = CameraPositionNewY - num7;
		}
		maxHeightCameraPosition = 10000f;
		float num9 = 1.5f;
		float max = -1455f / (ZoomScaler * num9);
		cameraHeightPosition += num8;
		cameraHeightPosition = MathHelper.Clamp(cameraHeightPosition + num8, 0f - maxHeightCameraPosition, max);
	}

	private void ScrollCameraOld(Viewport viewport)
	{
		float num = (float)viewport.Width * 0.35f;
		_ = viewport.Width;
		CamVector = new Vector2(mouse.X, mouse.Y);
		float num2 = 0f;
		num2 = CameraPositionNewX;
		float max = 640000 - viewport.Width;
		cameraPosition = MathHelper.Clamp(cameraPosition + num2, 0f, max);
		float num3 = (float)viewport.Height * 0.35f;
		_ = viewport.Height;
		float num4 = 0f;
		num4 = CameraPositionNewY;
		float num5 = 4800000 - viewport.Height;
		cameraHeightPosition += num4;
		cameraHeightPosition = MathHelper.Clamp(cameraHeightPosition + num4, 0f - num5, 5000f);
		CameraPositionNewX = 0f;
		CameraPositionNewY = 0f;
	}
}
