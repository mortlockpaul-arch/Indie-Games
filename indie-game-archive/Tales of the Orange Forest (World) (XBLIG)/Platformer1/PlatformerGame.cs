using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using ProjectMercury;
using ProjectMercury.Renderers;

namespace Platformer1;

public class PlatformerGame : Game
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

	public struct LevelNames(int count)
	{
		public string[] LevelName = new string[count];

		public bool[] Unlocked_Gauntlet_Run = new bool[count];

		public bool[] Dueling = new bool[count];

		public bool[] Unlocked_Dueling = new bool[count];

		public bool[] Custom = new bool[count];

		public bool[] Unlocked_Custom = new bool[count];

		public int Count = count;
	}

	public struct PlayerProfileData(int count)
	{
		public string[] Name = new string[1];

		public float[] PlayerSpecies = new float[1];

		public float[] Flair = new float[count];
	}

	public struct PlayerProfile(int count)
	{
		public string[] Name = new string[1];

		public float[] PlayerSpecies = new float[1];

		public float[] Flair = new float[count];

		public float[] Exp = new float[1];
	}

	public struct LoadingHints(int count)
	{
		public string[] Hints = new string[count];
	}

	public struct LevelIndex(int count)
	{
		public string[] Name = new string[count];

		public float[] LevelType = new float[count];

		public float[] Difficulty = new float[count];

		public float[] Exp = new float[count];
	}

	private const Buttons ContinueButton = Buttons.A;

	public Thread Thread_Loading;

	public Thread Thread_Update_Xbox;

	public bool IsHD;

	public bool Level_Exit_Reached;

	public ParticleEffect particleEffectLoading;

	public ParticleEffect particleEffectBlood;

	public Renderer renderer;

	public Matrix cameraTransform;

	public Viewport[] Viewports;

	public bool SplitScreen;

	public bool SplitScreenHoriz = true;

	public int SplitScreenindex;

	public GraphicsDeviceManager graphics;

	public GameTime gameTimeGlobal;

	private GraphicsDeviceManager graphics2;

	private SpriteBatch spriteBatch;

	private double FPS;

	private int numOfFrames;

	public double FTPTimeOld;

	private SpriteFont hudFont;

	private SpriteFont hud2Font;

	private SpriteFont loadingFont;

	private SpriteFont HintsFont;

	private SpriteFont HintsFont2;

	private bool FullScreenPressed;

	private bool wasFullScreenPressed;

	private Texture2D winOverlay;

	private Texture2D loseOverlay;

	private Texture2D diedOverlay;

	public bool gamePaused;

	public bool Duel;

	public bool Co_Op;

	public int WinnerDuel;

	private bool TransSkipP1WasPressed;

	private bool TransSkipP2WasPressed;

	private bool TransSkipP3WasPressed;

	private bool TransSkipP4WasPressed;

	private bool TransSkip;

	public IAsyncResult result2;

	public StorageDevice storageDevice;

	private int StorageDeviceCheckTimer;

	private bool DeviceSelectorRequested = true;

	public IAsyncResult result3;

	public Effect desaturateEffect;

	public Effect disappearEffect;

	public Effect normalmapEffect;

	public Effect refractionEffect;

	private Texture2D RnRTexture;

	private SoundEffect RnRSound;

	private bool RnRFirst = true;

	private Texture2D FarMercTexture;

	public Texture2D RnRBurnTexture;

	public Texture2D RockBurnTexture;

	private Texture2D SilentFilmCircleTexture;

	private bool TransEnd;

	private Song SongMainMenu;

	private bool MenuMusicFirst = true;

	public SoundEffect MenuClickSound;

	public SoundEffect MenuMoveSound;

	public float Sound_Effect_Volume = 0.1f;

	public float Music_Volume = 0.5f;

	public float Volume_Step = 0.05f;

	public int Species_Count = 4;

	public int ProfileMax = 3;

	public Texture2D[] PlayerSpriteSheet;

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

	public int P1ProfileIndex;

	public bool P2DpadRightpressed;

	public bool P2DpadRightWaspressed;

	public bool P2DpadLeftpressed;

	public bool P2DpadLeftWaspressed;

	public bool P2DpadUppressed;

	public bool P2DpadUpWaspressed;

	public bool P2DpadDownpressed;

	public bool P2DpadDownWaspressed;

	public bool P2ShoulderRightWaspressed;

	public bool P2ShoulderRightpressed;

	public bool P2ShoulderLeftWaspressed;

	public bool P2ShoulderLeftpressed;

	public int P2ProfileIndex;

	public bool P3DpadRightpressed;

	public bool P3DpadRightWaspressed;

	public bool P3DpadLeftpressed;

	public bool P3DpadLeftWaspressed;

	public bool P3DpadUppressed;

	public bool P3DpadUpWaspressed;

	public bool P3DpadDownpressed;

	public bool P3DpadDownWaspressed;

	public bool P3ShoulderRightWaspressed;

	public bool P3ShoulderRightpressed;

	public bool P3ShoulderLeftWaspressed;

	public bool P3ShoulderLeftpressed;

	public int P3ProfileIndex;

	public bool P4DpadRightpressed;

	public bool P4DpadRightWaspressed;

	public bool P4DpadLeftpressed;

	public bool P4DpadLeftWaspressed;

	public bool P4DpadUppressed;

	public bool P4DpadUpWaspressed;

	public bool P4DpadDownpressed;

	public bool P4DpadDownWaspressed;

	public bool P4ShoulderRightWaspressed;

	public bool P4ShoulderRightpressed;

	public bool P4ShoulderLeftWaspressed;

	public bool P4ShoulderLeftpressed;

	public int P4ProfileIndex;

	public Vector2 Player1Position;

	public Vector2 Player2Position;

	public Vector2 Player3Position;

	public Vector2 Player4Position;

	public Color Player1Color = Color.White;

	public Color Player2Color = Color.White;

	public Color Player3Color = Color.White;

	public Color Player4Color = Color.White;

	public int PlayersInGameindex;

	public bool Player1InGame;

	public bool Player2InGame;

	public bool Player3InGame;

	public bool Player4InGame;

	public bool Player1Ready = true;

	public bool Player2Ready = true;

	public bool Player3Ready = true;

	public bool Player4Ready = true;

	public bool P1AWasPressed;

	public bool P2AWasPressed;

	public bool P3AWasPressed;

	public bool P4AWasPressed;

	public bool P1BWasPressed;

	public bool P2BWasPressed;

	public bool P3BWasPressed;

	public bool P4BWasPressed;

	public int MainMenuProgressionMax = 4;

	public int P1MainMenuProgression;

	public int P2MainMenuProgression;

	public int P3MainMenuProgression;

	public int P4MainMenuProgression;

	public bool P1InControlOfMainMenu;

	public bool P2InControlOfMainMenu;

	public bool P3InControlOfMainMenu;

	public bool P4InControlOfMainMenu;

	public Texture2D Player1MenuTexture;

	public Texture2D Player2MenuTexture;

	public Texture2D Player3MenuTexture;

	public Texture2D Player4MenuTexture;

	private Texture2D BackDropTexture;

	public Texture2D Player1SpriteSheet;

	public Texture2D Player2SpriteSheet;

	public Texture2D Player3SpriteSheet;

	public Texture2D Player4SpriteSheet;

	public string Player1ProfileName;

	public string Player2ProfileName;

	public string Player3ProfileName;

	public string Player4ProfileName;

	public float Player1Species;

	public float Player2Species;

	public float Player3Species;

	public float Player4Species;

	private bool FlairChanged;

	public int FlairIndexMax = 8;

	public int FlairMax = 7;

	private int P1FlairIndex = 8;

	private int P1Flair;

	public int[] P1FlairOld;

	public int[] P1FlairOld_Tag;

	public int[] P2FlairOld_Tag;

	public int[] P3FlairOld_Tag;

	public int[] P4FlairOld_Tag;

	private string P1Text;

	private int P2FlairIndex = 8;

	private int P2Flair;

	public int[] P2FlairOld;

	private string P2Text;

	private int P3FlairIndex = 8;

	private int P3Flair;

	public int[] P3FlairOld;

	private string P3Text;

	private int P4FlairIndex = 8;

	private int P4Flair;

	public int[] P4FlairOld;

	private string P4Text;

	private bool wasContinue1Pressed;

	private bool wasContinue2Pressed;

	private bool wasContinue3Pressed;

	private bool wasContinue4Pressed;

	private bool wasContinuePressed;

	private bool wasNextLevelPressed;

	private bool wasBloodPressed;

	private int levelIndex = -1;

	public Level level;

	public LevelBuilder levelBuilder;

	private int levelBuilderIndex;

	public float PhysicsScaler = 1000f;

	public bool InMainMenuMode;

	public bool InPauseMode;

	public bool InOptionsMode;

	public bool InBloodMode = true;

	public bool InLevelMode;

	public bool InFirstTrans1 = true;

	public bool InFirstTrans2;

	private float Trans_1_Delay = 10f;

	private float Trans_2_Delay = 10f;

	private float Trans_MM_Delay = 3f;

	private float Trans_Game_Delay = 2f;

	private Thread PreLoadTread;

	private bool PreLoadTread_First = true;

	public float MainManuFadeTimeOld;

	public bool MainMenuFadeIn;

	public bool MainMenuFadeOut;

	private bool MainMenuLoaded;

	private bool UpdatedOnce;

	private bool NotFirst;

	public bool LevelFadeIn;

	public bool LevelFadeOut;

	private float LevelFadeTimeOld;

	public string levelBuilderPath;

	public int LoadLevelIndexer;

	public bool InLevelBuilderMode;

	public bool Loaded;

	public Texture2D MainMenuTexture;

	public Texture2D LoadingTexture;

	public Texture2D HintsBackdrop;

	public Vector2 LoadingPosition;

	private float LoadingRot;

	private float LoadingRotRate = 0.1f;

	public Texture2D PauseMenuTexture;

	public int MainMenuState;

	public int MainMenuIndexer;

	public int MaxMainMenuIndexer = 5;

	public int MainMenuLevelIndexer;

	public int GauntletRunLevelIndexerEnd = 19;

	public int DuelingLevelIndexerEnd = 25;

	public int MainMenuLevelIndexerMax = 18;

	public int MainMenuIndexerOption;

	public int MainMenuIndexerOptionMax = 2;

	public int MainMenuLevelBuilderIndexer;

	public int MainMenuLevelBuilderIndexerMax = 1;

	public string LevelBuilderString = "  ";

	public bool BloodToggle = true;

	public bool MusicToggle = true;

	public bool SoundEffectToggle = true;

	public bool FriendlyFireToggle = true;

	public string MainMenuPath;

	public Random random = new Random(354668);

	public bool StartGame;

	private bool Update_Levels_On_Xbox_DONE;

	private bool Update_Levels_On_Xbox_WORKING;

	public bool StartLevelBuilder;

	public bool LevelFromBuilder;

	public StorageContainer storageContainer;

	private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30.0);

	public GamerServicesComponent GSC;

	public GameTime MainGameTime;

	public LevelNames AllLevelNames;

	private LoadingHints HintsData;

	public string Hint_Of_The_Load;

	public int HintMax = 12;

	public int TargetFrameRate = 60;

	public float BackBufferWidth = 1920f;

	public float BackBufferHeight = 1080f;

	public float Global_Scaler = 1f;

	public Vector2 Offset_Static;

	public Vector2 Offset_Active;

	public Vector2 Original_Window;

	public Vector2 True_Screen_Center;

	public PlatformerGame()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 1 });
		graphics = new GraphicsDeviceManager(this);
		graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
		graphics.PreferredBackBufferWidth = (int)BackBufferWidth;
		graphics.PreferredBackBufferHeight = (int)BackBufferHeight;
		graphics.PreferMultiSampling = true;
		GSC = new GamerServicesComponent(this);
		GSC.Enabled = true;
		base.Components.Add(GSC);
		base.Window.AllowUserResizing = false;
		base.Window.Title = "Tales of the Orange Forest: The Skrealing Festival";
		base.IsMouseVisible = false;
		Viewports = new Viewport[9];
		levelBuilderPath = "Content/LevelBuilder/2/0.txt";
		levelBuilderPath = "00.txt";
		PlayerSpriteSheet = new Texture2D[Species_Count];
		base.Content.RootDirectory = "Content";
		base.TargetElapsedTime = TimeSpan.FromTicks(10000000L / (long)TargetFrameRate);
		StorageDevice.DeviceChanged += storageDeviceChangedEVENT;
	}

	public LevelNames LoadData(string path, StorageContainer Container)
	{
		LevelNames result = default(LevelNames);
		string text = " ";
		text = "LevelNames.txt";
		try
		{
			if (Container != null)
			{
				using (Container)
				{
					Stream stream;
					using (stream = Container.OpenFile(text, FileMode.Open))
					{
						try
						{
							XmlSerializer xmlSerializer = new XmlSerializer(typeof(LevelNames));
							result = (LevelNames)xmlSerializer.Deserialize(stream);
							return result;
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
				}
			}
			InLevelMode = false;
			InMainMenuMode = true;
			MainMenuFadeIn = true;
			MainMenuFadeOut = false;
			MediaPlayer.Stop();
			MediaPlayer.Play(SongMainMenu);
		}
		catch (StorageDeviceNotConnectedException)
		{
			storageDeviceRemoved2();
		}
		return result;
	}

	private void storageDeviceChangedEVENT(object o, EventArgs e)
	{
		try
		{
			if (storageDevice == null || storageDevice.IsConnected)
			{
				return;
			}
			storageDevice = null;
			Guide.BeginShowMessageBox("Storage Device Changed", "The storage device has be removed.", new string[1] { "Return To Main Menu" }, 0, MessageBoxIcon.Warning, delegate(IAsyncResult result)
			{
				Guide.EndShowMessageBox(result);
				if (Player1InGame)
				{
					P1MainMenuProgression = 0;
				}
				if (Player2InGame)
				{
					P2MainMenuProgression = 0;
				}
				if (Player3InGame)
				{
					P3MainMenuProgression = 0;
				}
				if (Player4InGame)
				{
					P4MainMenuProgression = 0;
				}
				DeviceSelectorRequested = true;
				MainManuFadeTimeOld = (float)gameTimeGlobal.TotalGameTime.TotalSeconds;
				MainMenuFadeIn = true;
				MainMenuFadeOut = false;
				InMainMenuMode = true;
				InPauseMode = false;
				InLevelMode = false;
				InLevelBuilderMode = false;
				MainMenuLoaded = true;
				StartGame = false;
				StartLevelBuilder = false;
				MediaPlayer.Stop();
				MediaPlayer.Play(SongMainMenu);
			}, null);
		}
		catch (Exception)
		{
			storageDeviceRemoved();
		}
	}

	private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
		DisplayMode currentDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
		e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = currentDisplayMode.Format;
		e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = currentDisplayMode.Width;
		e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = currentDisplayMode.Height - 40;
		BackBufferWidth = (float)currentDisplayMode.Width * 0.7f;
		BackBufferHeight = (float)(currentDisplayMode.Height - 40) * 0.7f;
		Viewport viewport = new Viewport
		{
			X = 0,
			Y = 0,
			Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth,
			Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight
		};
		Viewports[0] = viewport;
		viewport.X = 0;
		viewport.Y = 0;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		Viewports[1] = viewport;
		viewport.X = 0;
		viewport.Y = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		Viewports[2] = viewport;
		viewport.X = 0;
		viewport.Y = 0;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight;
		Viewports[3] = viewport;
		viewport.X = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Y = 0;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight;
		Viewports[4] = viewport;
		viewport.X = 0;
		viewport.Y = 0;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		Viewports[5] = viewport;
		viewport.X = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Y = 0;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		Viewports[6] = viewport;
		viewport.X = 0;
		viewport.Y = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		Viewports[7] = viewport;
		viewport.X = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Y = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		viewport.Width = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth / 2;
		viewport.Height = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight / 2;
		Viewports[8] = viewport;
	}

	public bool Trans_1(SpriteBatch spriteBatch, GameTime gameTime)
	{
		HandleTransInput();
		if (gameTime.TotalGameTime.TotalSeconds < (double)Trans_1_Delay)
		{
			if (TransSkip)
			{
				Trans_1_Delay = 0f;
				TransSkip = false;
				InFirstTrans1 = false;
				InFirstTrans2 = true;
				return true;
			}
			InFirstTrans1 = true;
			return false;
		}
		InFirstTrans1 = false;
		InFirstTrans2 = true;
		return true;
	}

	public bool Trans_2(SpriteBatch spriteBatch, GameTime gameTime)
	{
		HandleTransInput();
		if (gameTime.TotalGameTime.TotalSeconds < (double)(Trans_2_Delay + Trans_1_Delay))
		{
			if (TransSkip)
			{
				Trans_2_Delay = 0f;
				InFirstTrans2 = false;
				return true;
			}
			InFirstTrans2 = true;
			return false;
		}
		if (!MainMenuLoaded)
		{
			LoadMainMenu(gameTime);
		}
		InFirstTrans2 = false;
		return true;
	}

	public bool Trans_MM(SpriteBatch spriteBatch, GameTime gameTime)
	{
		if (gameTime.TotalGameTime.TotalSeconds - (double)MainManuFadeTimeOld < (double)Trans_MM_Delay)
		{
			return false;
		}
		return true;
	}

	public bool Trans_ToGame(SpriteBatch spriteBatch, GameTime gameTime)
	{
		if (gameTime.TotalGameTime.TotalSeconds - (double)MainManuFadeTimeOld < (double)Trans_Game_Delay)
		{
			return false;
		}
		return true;
	}

	protected override void LoadContent()
	{
		base.Components.Add(new GamerServicesComponent(this));
		hudFont = base.Content.Load<SpriteFont>("Fonts/Hud");
		hud2Font = base.Content.Load<SpriteFont>("Fonts/Hud2");
		loadingFont = base.Content.Load<SpriteFont>("Fonts/Loading");
		HintsFont = base.Content.Load<SpriteFont>("Fonts/Hud");
		HintsFont2 = base.Content.Load<SpriteFont>("Fonts/Hud3");
		LoadingTexture = base.Content.Load<Texture2D>("Sprites/0/head");
		HintsBackdrop = base.Content.Load<Texture2D>("Hints/HintsBackdrop");
		RnRTexture = base.Content.Load<Texture2D>("Credit/RnR");
		RnRSound = base.Content.Load<SoundEffect>("SoundEffects/RnRSound");
		MenuClickSound = base.Content.Load<SoundEffect>("SoundEffects/MenuClick");
		MenuMoveSound = base.Content.Load<SoundEffect>("SoundEffects/MenuMove");
		FarMercTexture = base.Content.Load<Texture2D>("Credit/FarNMerc");
		disappearEffect = base.Content.Load<Effect>("FX/disappear");
		normalmapEffect = base.Content.Load<Effect>("FX/normalmap");
		refractionEffect = base.Content.Load<Effect>("FX/refraction");
		RnRBurnTexture = base.Content.Load<Texture2D>("FX/SilentFilmCircleTexture");
		SilentFilmCircleTexture = base.Content.Load<Texture2D>("FX/SilentFilmCircleTexture");
		RockBurnTexture = base.Content.Load<Texture2D>("FX/waterfall");
		SongMainMenu = base.Content.Load<Song>("Music/3");
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		graphics.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		LoadingRot += 0.1f;
		spriteBatch.Draw(LoadingTexture, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2), null, Color.White, LoadingRot, new Vector2(LoadingTexture.Width / 2, LoadingTexture.Height / 2), 2f, SpriteEffects.None, 1f);
		int maxValue = 1;
		DrawShadowedString(loadingFont, "Loading.", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 75, graphics.GraphicsDevice.Viewport.Height / 2 - 200) + new Vector2(random.Next(maxValue), random.Next(maxValue)), Color.Red);
		spriteBatch.End();
		Loaded = true;
		particleEffectLoading = base.Content.Load<ParticleEffect>("Effects/Particle/LoadingFog");
		particleEffectBlood = base.Content.Load<ParticleEffect>("Effects/Particle/BloodSquirting");
		renderer = new SpriteBatchRenderer
		{
			GraphicsDeviceService = graphics
		};
		particleEffectLoading.Initialise();
		particleEffectLoading.LoadContent(base.Content);
		particleEffectBlood.Initialise();
		particleEffectBlood.LoadContent(base.Content);
		renderer.LoadContent(base.Content);
		StorageDevice.DeviceChanged += storageDeviceChangedEVENT;
	}

	public void storageDeviceRemoved()
	{
		if (Player1InGame)
		{
			P1MainMenuProgression = 0;
		}
		if (Player2InGame)
		{
			P2MainMenuProgression = 0;
		}
		if (Player3InGame)
		{
			P3MainMenuProgression = 0;
		}
		if (Player4InGame)
		{
			P4MainMenuProgression = 0;
		}
		DeviceSelectorRequested = true;
		MainManuFadeTimeOld = (float)gameTimeGlobal.TotalGameTime.TotalSeconds;
		MainMenuFadeIn = true;
		MainMenuFadeOut = false;
		InMainMenuMode = true;
		InPauseMode = false;
		InLevelMode = false;
		InLevelBuilderMode = false;
		MainMenuLoaded = true;
		StartGame = false;
		StartLevelBuilder = false;
		MediaPlayer.Stop();
		MediaPlayer.Play(SongMainMenu);
		Update_Levels_On_Xbox_DONE = false;
		result2 = null;
		DeviceSelectorRequested = false;
	}

	public void storageDeviceRemoved2()
	{
		Update_Levels_On_Xbox_DONE = false;
		result2 = null;
		DeviceSelectorRequested = false;
	}

	protected override void Update(GameTime gameTime)
	{
		GSC.Update(gameTime);
		gameTimeGlobal = gameTime;
		StorageDevice.DeviceChanged += storageDeviceChangedEVENT;
		if (Update_Levels_On_Xbox_WORKING)
		{
			Update_Levels_On_Xbox(storageDevice);
			if (!Update_Levels_On_Xbox_DONE)
			{
				storageDevice = null;
				result2 = null;
			}
			Update_Levels_On_Xbox_WORKING = false;
		}
		else if (StorageDeviceCheckTimer > 200)
		{
			if (storageDevice == null)
			{
				DeviceSelectorRequested = true;
			}
			else if (storageDevice.IsConnected)
			{
				DeviceSelectorRequested = false;
			}
			else
			{
				DeviceSelectorRequested = true;
			}
			if (result2 == null)
			{
				DeviceSelectorRequested = true;
			}
			if (!Guide.IsVisible && DeviceSelectorRequested)
			{
				DeviceSelectorRequested = false;
				result2 = StorageDevice.BeginShowSelector(GetDevice, 0);
			}
			StorageDeviceCheckTimer = 0;
		}
		else
		{
			StorageDeviceCheckTimer++;
		}
		if (Guide.IsVisible)
		{
			return;
		}
		UpdateLoadingScreen(gameTime);
		MainGameTime = gameTime;
		if (!UpdatedOnce)
		{
			AllLevelNames = new LevelNames(50);
			_ = $"Content/LevelBuilder/LevelNames.txt";
			HintsData = LoadHints();
			IsHD = graphics.GraphicsDevice.Viewport.Width > 1000;
			graphics.PreferredBackBufferWidth = (int)BackBufferWidth;
			graphics.PreferredBackBufferHeight = (int)BackBufferHeight;
			Global_Scaler = BackBufferHeight / 1080f;
			Offset_Static = new Vector2(960f - BackBufferWidth / 2f, 540f - BackBufferHeight / 2f);
			Original_Window = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
			True_Screen_Center = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
			UpdatedOnce = true;
		}
		if (!Trans_1(spriteBatch, gameTime) || !Trans_2(spriteBatch, gameTime))
		{
			return;
		}
		if (gameTime.TotalGameTime.TotalSeconds > FTPTimeOld)
		{
			FPS = numOfFrames;
			numOfFrames = 0;
			FTPTimeOld = gameTime.TotalGameTime.TotalSeconds + 1.0;
		}
		if (InMainMenuMode)
		{
			Hint_Of_The_Load = HintsData.Hints[random.Next(HintMax)];
			if (level != null)
			{
				level.Dispose();
				level = null;
			}
			if (levelBuilder != null)
			{
				levelBuilder.Dispose();
			}
			if (Loaded)
			{
				if (Trans_MM(spriteBatch, gameTime) && Update_Levels_On_Xbox_DONE)
				{
					HandleMainMenuInput2(gameTime);
				}
				if (StartGame)
				{
					if (MainManuFadeTimeOld + Trans_MM_Delay + 2f < (float)gameTime.TotalGameTime.TotalSeconds)
					{
						MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
						MainMenuFadeIn = false;
						MainMenuFadeOut = true;
					}
					if (Trans_MM(spriteBatch, gameTime) && Loaded)
					{
						Loaded = false;
						Thread_Loading = new Thread((ThreadStart)delegate
						{
							LoadLevelFromBuilder(MainMenuLevelIndexer);
						});
						Thread_Loading.Start();
					}
				}
				else if (StartLevelBuilder)
				{
					if (MainManuFadeTimeOld + Trans_MM_Delay + 2f < (float)gameTime.TotalGameTime.TotalSeconds)
					{
						MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
						MainMenuFadeIn = false;
						MainMenuFadeOut = true;
					}
					if (Trans_MM(spriteBatch, gameTime) && Loaded)
					{
						Loaded = false;
						Thread_Loading = new Thread((ThreadStart)delegate
						{
							LoadNewLevelInBuilder();
						});
						Thread_Loading.Start();
					}
				}
			}
			base.Update(gameTime);
		}
		else if (InPauseMode)
		{
			HandleInput();
			if (level != null)
			{
				level.Update(gameTime);
			}
			base.Update(gameTime);
		}
		else if (InLevelMode)
		{
			if (level != null && level.ReachedExit && !Level_Exit_Reached)
			{
				Level_Exit_Reached = true;
				if (!Guide.IsTrialMode)
				{
					MainMenuLevelIndexer++;
				}
				if (MainMenuLevelIndexer <= MainMenuLevelIndexerMax)
				{
					if (MainMenuIndexer == 0)
					{
						AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer] = true;
						AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer] = true;
					}
					else if (MainMenuIndexer == 1)
					{
						AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer] = true;
					}
					else
					{
						_ = MainMenuIndexer;
						_ = 2;
					}
					if (!Guide.IsTrialMode)
					{
						Save_LevelName_Data();
					}
					if (MainMenuIndexer == 0)
					{
						LoadLevelFromBuilder(MainMenuLevelIndexer);
					}
					else if (MainMenuIndexer == 1)
					{
						LoadLevelFromBuilder(MainMenuLevelIndexer);
					}
					else if (MainMenuIndexer == 2)
					{
						InLevelMode = false;
						InMainMenuMode = true;
						MainMenuFadeIn = true;
						MainMenuFadeOut = false;
						MediaPlayer.Stop();
						MediaPlayer.Play(SongMainMenu);
						MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
					}
				}
				else
				{
					InLevelMode = false;
					InMainMenuMode = true;
					MainMenuFadeIn = true;
					MainMenuFadeOut = false;
					MediaPlayer.Stop();
					MediaPlayer.Play(SongMainMenu);
					MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
				}
			}
			if (level != null)
			{
				level.Update(gameTime);
			}
			if (InMainMenuMode)
			{
				if (Player1InGame)
				{
					Player1Ready = true;
				}
				if (Player2InGame)
				{
					Player2Ready = true;
				}
				if (Player3InGame)
				{
					Player3Ready = true;
				}
				if (Player4InGame)
				{
					Player4Ready = true;
				}
				ReLoadMainMenu(gameTime);
				InMainMenuMode = true;
				InPauseMode = false;
				InLevelMode = false;
				InLevelBuilderMode = false;
				MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
				MainMenuFadeIn = true;
				MainMenuFadeOut = false;
			}
			if (level != null)
			{
				if (!level.Exit_Reached_First && Loaded)
				{
					HandleInput();
					HandleInput1();
					HandleInput2();
					HandleInput3();
					HandleInput4();
				}
			}
			else if (levelBuilder != null && Loaded)
			{
				HandleInput();
				HandleInput1();
				HandleInput2();
				HandleInput3();
				HandleInput4();
			}
			base.Update(gameTime);
		}
		else
		{
			if (!InLevelBuilderMode)
			{
				return;
			}
			if (levelBuilder != null)
			{
				if (!levelBuilder.Exit)
				{
					levelBuilder.Update(gameTime);
				}
				else
				{
					levelBuilder.Dispose();
					LoadMainMenu(gameTime);
					InMainMenuMode = true;
					InPauseMode = false;
					InLevelMode = false;
					InLevelBuilderMode = false;
					MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
					MainMenuFadeIn = true;
					MainMenuFadeOut = false;
				}
			}
			base.Update(gameTime);
		}
	}

	private void GetDevice(IAsyncResult result)
	{
		storageDevice = StorageDevice.EndShowSelector(result);
		if (storageDevice != null)
		{
			Update_Levels_On_Xbox_WORKING = true;
		}
	}

	public void Update_Levels_On_Xbox(StorageDevice storageDevice)
	{
		this.storageDevice = storageDevice;
		string text = " ";
		string text2 = " ";
		text = "Content/LevelBuilder/LevelNames.txt";
		text2 = "LevelNames.txt";
		Update_Levels_On_Xbox_DONE = false;
		using StorageContainer storageContainer = OpenContainer(storageDevice, "Totof_Levels");
		if (storageDevice.IsConnected)
		{
			if (!storageContainer.FileExists(text2))
			{
				Stream stream;
				LevelNames levelNames;
				using (stream = TitleContainer.OpenStream(text))
				{
					try
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(LevelNames));
						levelNames = (LevelNames)xmlSerializer.Deserialize(stream);
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
				using Stream stream2 = storageContainer.CreateFile(text2);
				try
				{
					if (storageDevice.IsConnected)
					{
						new XmlSerializer(typeof(LevelNames)).Serialize(stream2, levelNames);
					}
				}
				catch (Exception)
				{
				}
			}
		}
		else
		{
			storageDeviceRemoved2();
		}
		for (int i = 0; i < 3; i++)
		{
			SavedData savedData;
			switch (i)
			{
			case 0:
			{
				for (int k = 0; k < 19; k++)
				{
					if (storageDevice.IsConnected)
					{
						text = $"Content/LevelBuilder/{i}/{k}.txt";
						text2 = $"{i}{k}.txt";
						if (storageContainer.FileExists(text2))
						{
							continue;
						}
						Stream stream5;
						using (stream5 = TitleContainer.OpenStream(text))
						{
							try
							{
								XmlSerializer xmlSerializer3 = new XmlSerializer(typeof(SavedData));
								savedData = (SavedData)xmlSerializer3.Deserialize(stream5);
							}
							catch (Exception ex5)
							{
								throw new Exception(ex5.ToString());
							}
							finally
							{
								stream5.Close();
							}
						}
						if (!storageDevice.IsConnected)
						{
							continue;
						}
						using (Stream stream6 = storageContainer.CreateFile(text2))
						{
							try
							{
								if (storageDevice.IsConnected)
								{
									new XmlSerializer(typeof(SavedData)).Serialize(stream6, savedData);
								}
							}
							catch (Exception)
							{
							}
						}
						continue;
					}
					storageDeviceRemoved2();
					break;
				}
				break;
			}
			case 1:
			{
				for (int l = 0; l < 6; l++)
				{
					if (storageDevice.IsConnected)
					{
						text = $"Content/LevelBuilder/{i}/{l}.txt";
						text2 = $"{i}{l}.txt";
						if (storageContainer.FileExists(text2))
						{
							continue;
						}
						Stream stream7;
						using (stream7 = TitleContainer.OpenStream(text))
						{
							try
							{
								XmlSerializer xmlSerializer4 = new XmlSerializer(typeof(SavedData));
								savedData = (SavedData)xmlSerializer4.Deserialize(stream7);
							}
							finally
							{
								stream7.Close();
							}
						}
						using (Stream stream8 = storageContainer.CreateFile(text2))
						{
							try
							{
								if (storageDevice.IsConnected)
								{
									new XmlSerializer(typeof(SavedData)).Serialize(stream8, savedData);
								}
							}
							catch (Exception)
							{
							}
						}
						continue;
					}
					storageDeviceRemoved2();
					break;
				}
				break;
			}
			case 2:
			{
				for (int j = 0; j < 10; j++)
				{
					if (storageDevice.IsConnected)
					{
						text = $"Content/LevelBuilder/{i}/{j}.txt";
						text2 = $"{i}{j}.txt";
						if (storageContainer.FileExists(text2))
						{
							continue;
						}
						Stream stream3;
						using (stream3 = TitleContainer.OpenStream(text))
						{
							try
							{
								XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(SavedData));
								savedData = (SavedData)xmlSerializer2.Deserialize(stream3);
							}
							catch (Exception ex3)
							{
								throw new Exception(ex3.ToString());
							}
							finally
							{
								stream3.Close();
							}
						}
						using (Stream stream4 = storageContainer.CreateFile(text2))
						{
							try
							{
								if (storageDevice.IsConnected)
								{
									new XmlSerializer(typeof(SavedData)).Serialize(stream4, savedData);
								}
							}
							catch (Exception)
							{
							}
						}
						continue;
					}
					storageDeviceRemoved2();
					break;
				}
				break;
			}
			}
		}
		if (storageDevice.IsConnected)
		{
			AllLevelNames = LoadData("LevelNames.txt", storageContainer);
			Update_Levels_On_Xbox_DONE = true;
		}
		else
		{
			storageDeviceRemoved2();
		}
	}

	public void Save_LevelName_Data()
	{
		try
		{
			string file = $"LevelNames.txt";
			LevelNames allLevelNames = AllLevelNames;
			StorageContainer storageContainer = OpenContainer(storageDevice, "Totof_Levels");
			if (storageContainer == null)
			{
				InLevelMode = false;
				InMainMenuMode = true;
				MainMenuFadeIn = true;
				MainMenuFadeOut = false;
				MediaPlayer.Stop();
				MediaPlayer.Play(SongMainMenu);
				return;
			}
			using (storageContainer)
			{
				using Stream stream = storageContainer.CreateFile(file);
				new XmlSerializer(typeof(LevelNames)).Serialize(stream, allLevelNames);
			}
		}
		catch (StorageDeviceNotConnectedException)
		{
			storageDeviceRemoved();
		}
	}

	public void SaveProfileData(string subPath)
	{
		PlayerProfileData playerProfileData = new PlayerProfileData(FlairIndexMax + 1);
		playerProfileData.Flair[0] = P1FlairOld[0];
		playerProfileData.Flair[1] = P1FlairOld[1];
		playerProfileData.Flair[2] = P1FlairOld[2];
		playerProfileData.Flair[3] = P1FlairOld[3];
		playerProfileData.Flair[4] = P1FlairOld[4];
		playerProfileData.Flair[5] = P1FlairOld[5];
		playerProfileData.Flair[6] = P1FlairOld[6];
		playerProfileData.Flair[7] = P1FlairOld[7];
		playerProfileData.Flair[8] = P1FlairOld[8];
		playerProfileData.Flair[9] = P1FlairOld[9];
		playerProfileData.Name[0] = "0";
		playerProfileData.PlayerSpecies[0] = Player1Species;
		string path = $"Content/Profiles/{subPath}.txt";
		FileStream fileStream = File.Open(path, FileMode.Create);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlayerProfileData));
			xmlSerializer.Serialize(fileStream, playerProfileData);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public void SaveHintData(string subPath)
	{
		LoadingHints loadingHints = new LoadingHints(20);
		for (int i = 0; i < 20; i++)
		{
			loadingHints.Hints[i] = "Hint: ";
		}
		string path = $"Content/Hints/{subPath}.txt";
		FileStream fileStream = File.Open(path, FileMode.Create);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(LoadingHints));
			xmlSerializer.Serialize(fileStream, loadingHints);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public void CreatProfileData(string subPath)
	{
		PlayerProfileData playerProfileData = new PlayerProfileData(20);
		playerProfileData.Flair[0] = 0f;
		playerProfileData.Flair[1] = 0f;
		playerProfileData.Flair[2] = 0f;
		playerProfileData.Flair[3] = 0f;
		playerProfileData.Flair[4] = 0f;
		playerProfileData.Flair[5] = 0f;
		playerProfileData.Flair[6] = 0f;
		playerProfileData.Flair[7] = 0f;
		playerProfileData.Flair[8] = 0f;
		playerProfileData.Flair[9] = 0f;
		playerProfileData.Name[0] = "0";
		playerProfileData.PlayerSpecies[0] = 0f;
		FileStream fileStream = File.Open("Content/Profiles/Nothing.txt", FileMode.Create);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlayerProfileData));
			xmlSerializer.Serialize(fileStream, playerProfileData);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public void CreatLevelIndex(string subPath)
	{
		LevelIndex levelIndex = new LevelIndex(100);
		for (int i = 0; i > 0; i++)
		{
			levelIndex.Name[i] = "";
			levelIndex.LevelType[i] = 0f;
			levelIndex.Difficulty[i] = 0f;
			levelIndex.Exp[i] = 0f;
		}
		FileStream fileStream = File.Open("Content/LevelBuilder/index.txt", FileMode.Create);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(LevelIndex));
			xmlSerializer.Serialize(fileStream, levelIndex);
		}
		finally
		{
			fileStream.Close();
		}
	}

	public static PlayerProfileData LoadProfile(string subPath)
	{
		string name = $"Content/Profiles/{subPath}.txt";
		Stream stream = TitleContainer.OpenStream(name);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PlayerProfileData));
			return (PlayerProfileData)xmlSerializer.Deserialize(stream);
		}
		finally
		{
			stream.Close();
		}
	}

	public static LoadingHints LoadHints()
	{
		string name = "Content/Hints/Hints.txt";
		Stream stream = TitleContainer.OpenStream(name);
		try
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(LoadingHints));
			return (LoadingHints)xmlSerializer.Deserialize(stream);
		}
		finally
		{
			stream.Close();
		}
	}

	public void LoadMainMenu(GameTime gameTime)
	{
		MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
		MainMenuFadeIn = true;
		MainMenuFadeOut = false;
		InMainMenuMode = true;
		InPauseMode = false;
		InLevelMode = false;
		InLevelBuilderMode = false;
		MainMenuLoaded = true;
		StartGame = false;
		StartLevelBuilder = false;
		MainMenuTexture = base.Content.Load<Texture2D>("Menus/Main/1");
		Player1MenuTexture = base.Content.Load<Texture2D>("Menus/Main/head");
		Player2MenuTexture = base.Content.Load<Texture2D>("Menus/Main/head");
		Player3MenuTexture = base.Content.Load<Texture2D>("Menus/Main/head");
		Player4MenuTexture = base.Content.Load<Texture2D>("Menus/Main/head");
		BackDropTexture = base.Content.Load<Texture2D>("Menus/Main/Backdrop");
		Player1ProfileName = " ";
		Player2ProfileName = " ";
		Player3ProfileName = " ";
		Player4ProfileName = " ";
		P1FlairOld = new int[FlairIndexMax + 1];
		P1FlairOld_Tag = new int[FlairIndexMax + 1];
		P2FlairOld = new int[FlairIndexMax + 1];
		P2FlairOld_Tag = new int[FlairIndexMax + 1];
		P3FlairOld = new int[FlairIndexMax + 1];
		P3FlairOld_Tag = new int[FlairIndexMax + 1];
		P4FlairOld = new int[FlairIndexMax + 1];
		P4FlairOld_Tag = new int[FlairIndexMax + 1];
		for (int i = 1; i < FlairIndexMax; i++)
		{
			P1FlairOld[i] = 0;
		}
		for (int j = 1; j < FlairIndexMax; j++)
		{
			P2FlairOld[j] = 0;
		}
		for (int k = 1; k < FlairIndexMax; k++)
		{
			P3FlairOld[k] = 0;
		}
		for (int l = 1; l < FlairIndexMax; l++)
		{
			P4FlairOld[l] = 0;
		}
		if (MainMenuState > 0)
		{
			MainMenuState = 0;
		}
	}

	public void ReLoadMainMenu(GameTime gameTime)
	{
		MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
		MainMenuFadeIn = true;
		MainMenuFadeOut = false;
		InMainMenuMode = true;
		InPauseMode = false;
		InLevelMode = false;
		InLevelBuilderMode = false;
		MainMenuLoaded = true;
		StartGame = false;
		StartLevelBuilder = false;
		if (Player1InGame)
		{
			P1MainMenuProgression = 2;
		}
		if (Player2InGame)
		{
			P2MainMenuProgression = 2;
		}
		if (Player3InGame)
		{
			P3MainMenuProgression = 2;
		}
		if (Player4InGame)
		{
			P4MainMenuProgression = 2;
		}
		HandleMainMenuInput2(gameTime);
	}

	public void LoadNextLevelFromBuilder()
	{
		MainMenuLevelIndexer++;
		LoadLevelFromBuilder(MainMenuLevelIndexer);
	}

	public void LoadLevelFromBuilder(int LoadlevelBuilderIndex)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		if (Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (Player1InGame || Player2InGame || Player3InGame || Player4InGame)
			{
				PlayersInGameindex = 0;
				if (Player1InGame)
				{
					PlayersInGameindex++;
				}
				if (Player2InGame)
				{
					PlayersInGameindex++;
				}
				if (Player3InGame)
				{
					PlayersInGameindex++;
				}
				if (Player4InGame)
				{
					PlayersInGameindex++;
				}
				graphics.GraphicsDevice.Clear(Color.Black);
				Loaded = false;
				InLevelBuilderMode = false;
				InLevelMode = true;
				InMainMenuMode = false;
				InPauseMode = false;
				if (MainMenuIndexer == 0)
				{
					Duel = false;
					Co_Op = true;
				}
				else if (MainMenuIndexer == 1)
				{
					Duel = true;
					Co_Op = false;
				}
				else if (MainMenuIndexer == 2)
				{
					if (AllLevelNames.Dueling[LoadlevelBuilderIndex + DuelingLevelIndexerEnd])
					{
						Duel = true;
						Co_Op = false;
					}
					else
					{
						Duel = false;
						Co_Op = true;
					}
				}
				_ = $"Content/LevelBuilder/{MainMenuIndexer}/{LoadlevelBuilderIndex}.txt";
				string path = $"{MainMenuIndexer}{LoadlevelBuilderIndex}.txt";
				int num = 0;
				if (MainMenuIndexer == 0)
				{
					num = 20;
				}
				else if (MainMenuIndexer == 1)
				{
					num = 6;
				}
				else if (MainMenuIndexer == 2)
				{
					num = 10;
				}
				if (LoadlevelBuilderIndex < num)
				{
					levelIndex = -1;
					if (level != null)
					{
						level.Dispose();
						level = null;
					}
					LevelFromBuilder = true;
					MainMenuLevelIndexer = LoadlevelBuilderIndex;
					MediaPlayer.Stop();
					level = new Level(this, base.Services, path, spriteBatch);
					Loaded = true;
					return;
				}
				Loaded = true;
				if (Player1InGame)
				{
					Player1Ready = true;
				}
				if (Player2InGame)
				{
					Player2Ready = true;
				}
				if (Player3InGame)
				{
					Player3Ready = true;
				}
				if (Player4InGame)
				{
					Player4Ready = true;
				}
				ReLoadMainMenu(MainGameTime);
				InMainMenuMode = true;
				InPauseMode = false;
				InLevelMode = false;
				InLevelBuilderMode = false;
				MainManuFadeTimeOld = (float)MainGameTime.TotalGameTime.TotalSeconds;
				MainMenuFadeIn = true;
				MainMenuFadeOut = false;
			}
			else
			{
				StartGame = false;
			}
		}
		else
		{
			StartGame = false;
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

	public void LoadLevelBuilder()
	{
		graphics.GraphicsDevice.Clear(Color.Black);
		Loaded = false;
		InLevelBuilderMode = true;
		InLevelMode = false;
		InMainMenuMode = false;
		InPauseMode = false;
		StartLevelBuilder = true;
		StartGame = false;
		if (levelBuilder != null)
		{
			levelBuilder.Dispose();
		}
		PlayerIndex playerIndexer = PlayerIndex.One;
		if (P1InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.One;
		}
		if (P2InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.Two;
		}
		if (P3InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.Three;
		}
		if (P4InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.Four;
		}
		string level_Name = "00.txt";
		MediaPlayer.Stop();
		levelBuilder = new LevelBuilder(this, base.Services, level_Name, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), spriteBatch, playerIndexer);
		Loaded = true;
	}

	public void LoadNewLevelInBuilder()
	{
		graphics.GraphicsDevice.Clear(Color.Black);
		Loaded = false;
		InLevelBuilderMode = true;
		InLevelMode = false;
		InMainMenuMode = false;
		InPauseMode = false;
		StartLevelBuilder = true;
		StartGame = false;
		if (levelBuilder != null)
		{
			levelBuilder.Dispose();
		}
		PlayerIndex playerIndexer = PlayerIndex.One;
		if (P1InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.One;
		}
		if (P2InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.Two;
		}
		if (P3InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.Three;
		}
		if (P4InControlOfMainMenu)
		{
			playerIndexer = PlayerIndex.Four;
		}
		string level_Name = "20.txt";
		MediaPlayer.Stop();
		levelBuilder = new LevelBuilder(this, base.Services, level_Name, AllLevelNames.LevelName[26].ToString(), spriteBatch, playerIndexer);
		try
		{
			if (storageDevice == null)
			{
				storageDeviceRemoved();
			}
			else if (!storageDevice.IsConnected)
			{
				storageDeviceRemoved();
			}
		}
		catch (Exception)
		{
			storageDeviceRemoved2();
		}
		Loaded = true;
	}

	private void ReloadCurrentLevel()
	{
		levelIndex--;
		LoadNextLevelFromBuilder();
	}

	private void HandleMainMenuInput2_NO_Flair(GameTime gameTime)
	{
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		GamePadState state2 = GamePad.GetState(PlayerIndex.Two);
		GamePadState state3 = GamePad.GetState(PlayerIndex.Three);
		GamePadState state4 = GamePad.GetState(PlayerIndex.Four);
		Keyboard.GetState();
		if (!P1AWasPressed && state.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P1AWasPressed = true;
			P1MainMenuProgression++;
			if (P1MainMenuProgression > MainMenuProgressionMax)
			{
				P1MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P1MainMenuProgression = (int)MathHelper.Clamp(P1MainMenuProgression, 0f, 3f);
			}
		}
		if (state.Buttons.A == ButtonState.Released)
		{
			P1AWasPressed = false;
		}
		if (!P1BWasPressed && state.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P1BWasPressed = true;
			P1MainMenuProgression--;
			if (P1MainMenuProgression < 0)
			{
				P1MainMenuProgression = 0;
			}
		}
		if (state.Buttons.B == ButtonState.Released)
		{
			P1BWasPressed = false;
		}
		if (P1MainMenuProgression == 0)
		{
			Player1InGame = false;
			Player1Ready = true;
		}
		else if (P1MainMenuProgression == 1)
		{
			Player1InGame = true;
			Player1Ready = false;
			P1InControlOfMainMenu = false;
			if (Player1ProfileName == " ")
			{
				PlayerProfileData playerProfileData = LoadProfile($"{P1ProfileIndex}");
				for (int i = 0; i < FlairIndexMax + 1; i++)
				{
					Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
					P1FlairOld_Tag[i] = i;
					P1FlairOld[i] = (int)playerProfileData.Flair[i];
				}
				Player1ProfileName = playerProfileData.Name[0];
				Player1Species = playerProfileData.PlayerSpecies[0];
			}
			if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P1DpadRightpressed = true;
				P1ProfileIndex++;
				if (P1ProfileIndex > ProfileMax)
				{
					P1ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData2 = LoadProfile($"{P1ProfileIndex}");
				for (int j = 0; j < FlairIndexMax + 1; j++)
				{
					Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
					P1FlairOld_Tag[j] = j;
					P1FlairOld[j] = (int)playerProfileData2.Flair[j];
				}
				Player1ProfileName = playerProfileData2.Name[0];
				Player1Species = playerProfileData2.PlayerSpecies[0];
			}
			P1DpadRightWaspressed = P1DpadRightpressed;
			if (state.DPad.Right == ButtonState.Released)
			{
				P1DpadRightpressed = false;
			}
			if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P1DpadLeftpressed = true;
				P1ProfileIndex--;
				if (P1ProfileIndex < 0)
				{
					P1ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData3 = LoadProfile($"{P1ProfileIndex}");
				for (int k = 0; k < FlairIndexMax + 1; k++)
				{
					Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
					P1FlairOld_Tag[k] = k;
					P1FlairOld[k] = (int)playerProfileData3.Flair[k];
				}
				Player1ProfileName = playerProfileData3.Name[0];
				Player1Species = playerProfileData3.PlayerSpecies[0];
			}
			P1DpadLeftWaspressed = P1DpadLeftpressed;
			if (state.DPad.Left == ButtonState.Released)
			{
				P1DpadLeftpressed = false;
			}
			P1Text = "   ";
			string assetName = "Sprites/" + Player1Species + "/head";
			Player1MenuTexture = base.Content.Load<Texture2D>(assetName);
		}
		else if (P1MainMenuProgression == 2)
		{
			Player1InGame = true;
			Player1Ready = true;
			if (P2MainMenuProgression < 2 && P3MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P1InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P1InControlOfMainMenu)
			{
				if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P1DpadUppressed = true;
					MainMenuIndexer--;
				}
				P1DpadUpWaspressed = P1DpadUppressed;
				if (state.DPad.Up == ButtonState.Released)
				{
					P1DpadUppressed = false;
				}
				if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P1DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P1DpadDownWaspressed = P1DpadDownpressed;
				if (state.DPad.Down == ButtonState.Released)
				{
					P1DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P1MainMenuProgression == 3)
		{
			Player1InGame = true;
			Player1Ready = true;
			if (P1InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P1DpadUpWaspressed = P1DpadUppressed;
					if (state.DPad.Up == ButtonState.Released)
					{
						P1DpadUppressed = false;
					}
					if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P1DpadDownWaspressed = P1DpadDownpressed;
					if (state.DPad.Down == ButtonState.Released)
					{
						P1DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P1DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P1DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P1DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P1DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P1DpadRightpressed = true;
							BloodToggle = true;
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P1DpadLeftpressed = true;
							BloodToggle = false;
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P1MainMenuProgression == 4 && P1InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P1MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P1MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P1MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P2AWasPressed && state2.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P2AWasPressed = true;
			P2MainMenuProgression++;
			if (P2MainMenuProgression > MainMenuProgressionMax)
			{
				P2MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P2MainMenuProgression = (int)MathHelper.Clamp(P2MainMenuProgression, 0f, 3f);
			}
		}
		if (state2.Buttons.A == ButtonState.Released)
		{
			P2AWasPressed = false;
		}
		if (!P2BWasPressed && state2.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P2BWasPressed = true;
			P2MainMenuProgression--;
			if (P2MainMenuProgression < 0)
			{
				P2MainMenuProgression = 0;
			}
		}
		if (state2.Buttons.B == ButtonState.Released)
		{
			P2BWasPressed = false;
		}
		if (P2MainMenuProgression == 0)
		{
			Player2InGame = false;
			Player2Ready = true;
		}
		else if (P2MainMenuProgression == 1)
		{
			Player2InGame = true;
			Player2Ready = false;
			P2InControlOfMainMenu = false;
			if (Player2ProfileName == " ")
			{
				PlayerProfileData playerProfileData4 = LoadProfile($"{P2ProfileIndex}");
				for (int l = 0; l < FlairIndexMax + 1; l++)
				{
					Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
					P2FlairOld_Tag[l] = l;
					P2FlairOld[l] = (int)playerProfileData4.Flair[l];
				}
				Player2ProfileName = playerProfileData4.Name[0];
				Player2Species = playerProfileData4.PlayerSpecies[0];
			}
			if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P2DpadRightpressed = true;
				P2ProfileIndex++;
				if (P2ProfileIndex > ProfileMax)
				{
					P2ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData5 = LoadProfile($"{P2ProfileIndex}");
				for (int m = 0; m < FlairIndexMax + 1; m++)
				{
					Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
					P2FlairOld_Tag[m] = m;
					P2FlairOld[m] = (int)playerProfileData5.Flair[m];
				}
				Player2ProfileName = playerProfileData5.Name[0];
				Player2Species = playerProfileData5.PlayerSpecies[0];
			}
			P2DpadRightWaspressed = P2DpadRightpressed;
			if (state2.DPad.Right == ButtonState.Released)
			{
				P2DpadRightpressed = false;
			}
			if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P2DpadLeftpressed = true;
				P2ProfileIndex--;
				if (P2ProfileIndex < 0)
				{
					P2ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData6 = LoadProfile($"{P2ProfileIndex}");
				for (int n = 0; n < FlairIndexMax + 1; n++)
				{
					Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
					P2FlairOld_Tag[n] = n;
					P2FlairOld[n] = (int)playerProfileData6.Flair[n];
				}
				Player2ProfileName = playerProfileData6.Name[0];
				Player2Species = playerProfileData6.PlayerSpecies[0];
			}
			P2DpadLeftWaspressed = P2DpadLeftpressed;
			if (state2.DPad.Left == ButtonState.Released)
			{
				P2DpadLeftpressed = false;
			}
			P2Text = "    ";
			string assetName2 = "Sprites/" + Player2Species + "/head";
			Player2MenuTexture = base.Content.Load<Texture2D>(assetName2);
		}
		else if (P2MainMenuProgression == 2)
		{
			Player2InGame = true;
			Player2Ready = true;
			if (P1MainMenuProgression < 2 && P3MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P2InControlOfMainMenu = true;
				P1InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P2InControlOfMainMenu)
			{
				if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P2DpadUppressed = true;
					MainMenuIndexer--;
				}
				P2DpadUpWaspressed = P2DpadUppressed;
				if (state2.DPad.Up == ButtonState.Released)
				{
					P2DpadUppressed = false;
				}
				if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P2DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P2DpadDownWaspressed = P2DpadDownpressed;
				if (state2.DPad.Down == ButtonState.Released)
				{
					P2DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P2MainMenuProgression == 3)
		{
			Player2InGame = true;
			Player2Ready = true;
			if (P2InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P2DpadUpWaspressed = P2DpadUppressed;
					if (state2.DPad.Up == ButtonState.Released)
					{
						P2DpadUppressed = false;
					}
					if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P2DpadDownWaspressed = P2DpadDownpressed;
					if (state2.DPad.Down == ButtonState.Released)
					{
						P2DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P2DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P2DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P2DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P2DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P2DpadRightpressed = true;
							BloodToggle = true;
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P2DpadLeftpressed = true;
							BloodToggle = false;
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P2MainMenuProgression == 4 && P2InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P2MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P2MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P2MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P3AWasPressed && state3.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P3AWasPressed = true;
			P3MainMenuProgression++;
			if (P3MainMenuProgression > MainMenuProgressionMax)
			{
				P3MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P3MainMenuProgression = (int)MathHelper.Clamp(P3MainMenuProgression, 0f, 3f);
			}
		}
		if (state3.Buttons.A == ButtonState.Released)
		{
			P3AWasPressed = false;
		}
		if (!P3BWasPressed && state3.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P3BWasPressed = true;
			P3MainMenuProgression--;
			if (P3MainMenuProgression < 0)
			{
				P3MainMenuProgression = 0;
			}
		}
		if (state3.Buttons.B == ButtonState.Released)
		{
			P3BWasPressed = false;
		}
		if (P3MainMenuProgression == 0)
		{
			Player3InGame = false;
			Player3Ready = true;
		}
		else if (P3MainMenuProgression == 1)
		{
			Player3InGame = true;
			Player3Ready = false;
			P3InControlOfMainMenu = false;
			if (Player3ProfileName == " ")
			{
				PlayerProfileData playerProfileData7 = LoadProfile($"{P3ProfileIndex}");
				for (int num = 0; num < FlairIndexMax + 1; num++)
				{
					Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
					P3FlairOld_Tag[num] = num;
					P3FlairOld[num] = (int)playerProfileData7.Flair[num];
				}
				Player3ProfileName = playerProfileData7.Name[0];
				Player3Species = playerProfileData7.PlayerSpecies[0];
			}
			if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P3DpadRightpressed = true;
				P3ProfileIndex++;
				if (P3ProfileIndex > ProfileMax)
				{
					P3ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData8 = LoadProfile($"{P3ProfileIndex}");
				for (int num2 = 0; num2 < FlairIndexMax + 1; num2++)
				{
					Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
					P3FlairOld_Tag[num2] = num2;
					P3FlairOld[num2] = (int)playerProfileData8.Flair[num2];
				}
				Player3ProfileName = playerProfileData8.Name[0];
				Player3Species = playerProfileData8.PlayerSpecies[0];
			}
			P3DpadRightWaspressed = P3DpadRightpressed;
			if (state3.DPad.Right == ButtonState.Released)
			{
				P3DpadRightpressed = false;
			}
			if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P3DpadLeftpressed = true;
				P3ProfileIndex--;
				if (P3ProfileIndex < 0)
				{
					P3ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData9 = LoadProfile($"{P3ProfileIndex}");
				for (int num3 = 0; num3 < FlairIndexMax + 1; num3++)
				{
					Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
					P3FlairOld_Tag[num3] = num3;
					P3FlairOld[num3] = (int)playerProfileData9.Flair[num3];
				}
				Player3ProfileName = playerProfileData9.Name[0];
				Player3Species = playerProfileData9.PlayerSpecies[0];
			}
			P3DpadLeftWaspressed = P3DpadLeftpressed;
			if (state3.DPad.Left == ButtonState.Released)
			{
				P3DpadLeftpressed = false;
			}
			P3Text = "    ";
			string assetName3 = "Sprites/" + Player3Species + "/head";
			Player3MenuTexture = base.Content.Load<Texture2D>(assetName3);
		}
		else if (P3MainMenuProgression == 2)
		{
			Player3InGame = true;
			Player3Ready = true;
			if (P1MainMenuProgression < 2 && P2MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P3InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P1InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P3InControlOfMainMenu)
			{
				if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P3DpadUppressed = true;
					MainMenuIndexer--;
				}
				P3DpadUpWaspressed = P3DpadUppressed;
				if (state3.DPad.Up == ButtonState.Released)
				{
					P3DpadUppressed = false;
				}
				if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P3DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P3DpadDownWaspressed = P3DpadDownpressed;
				if (state3.DPad.Down == ButtonState.Released)
				{
					P3DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P3MainMenuProgression == 3)
		{
			Player3InGame = true;
			Player3Ready = true;
			if (P3InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P3DpadUpWaspressed = P3DpadUppressed;
					if (state3.DPad.Up == ButtonState.Released)
					{
						P3DpadUppressed = false;
					}
					if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P3DpadDownWaspressed = P3DpadDownpressed;
					if (state3.DPad.Down == ButtonState.Released)
					{
						P3DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P3DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P3DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P3DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P3DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P3DpadRightpressed = true;
							BloodToggle = true;
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P3DpadLeftpressed = true;
							BloodToggle = false;
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P3MainMenuProgression == 4 && P3InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P3MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P3MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P3MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P4AWasPressed && state4.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P4AWasPressed = true;
			P4MainMenuProgression++;
			if (P4MainMenuProgression > MainMenuProgressionMax)
			{
				P4MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P4MainMenuProgression = (int)MathHelper.Clamp(P4MainMenuProgression, 0f, 3f);
			}
		}
		if (state4.Buttons.A == ButtonState.Released)
		{
			P4AWasPressed = false;
		}
		if (!P4BWasPressed && state4.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P4BWasPressed = true;
			P4MainMenuProgression--;
			if (P4MainMenuProgression < 0)
			{
				P4MainMenuProgression = 0;
			}
		}
		if (state4.Buttons.B == ButtonState.Released)
		{
			P4BWasPressed = false;
		}
		if (P4MainMenuProgression == 0)
		{
			Player4InGame = false;
			Player4Ready = true;
		}
		else if (P4MainMenuProgression == 1)
		{
			Player4InGame = true;
			Player4Ready = false;
			P4InControlOfMainMenu = false;
			if (Player4ProfileName == " ")
			{
				PlayerProfileData playerProfileData10 = LoadProfile($"{P4ProfileIndex}");
				for (int num4 = 0; num4 < FlairIndexMax + 1; num4++)
				{
					Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
					P4FlairOld_Tag[num4] = num4;
					P4FlairOld[num4] = (int)playerProfileData10.Flair[num4];
				}
				Player4ProfileName = playerProfileData10.Name[0];
				Player4Species = playerProfileData10.PlayerSpecies[0];
			}
			if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P4DpadRightpressed = true;
				P4ProfileIndex++;
				if (P4ProfileIndex > ProfileMax)
				{
					P4ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData11 = LoadProfile($"{P4ProfileIndex}");
				for (int num5 = 0; num5 < FlairIndexMax + 1; num5++)
				{
					Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
					P4FlairOld_Tag[num5] = num5;
					P4FlairOld[num5] = (int)playerProfileData11.Flair[num5];
				}
				Player4ProfileName = playerProfileData11.Name[0];
				Player4Species = playerProfileData11.PlayerSpecies[0];
			}
			P4DpadRightWaspressed = P4DpadRightpressed;
			if (state4.DPad.Right == ButtonState.Released)
			{
				P4DpadRightpressed = false;
			}
			if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P4DpadLeftpressed = true;
				P4ProfileIndex--;
				if (P4ProfileIndex < 0)
				{
					P4ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData12 = LoadProfile($"{P4ProfileIndex}");
				for (int num6 = 0; num6 < FlairIndexMax + 1; num6++)
				{
					Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
					P4FlairOld_Tag[num6] = num6;
					P4FlairOld[num6] = (int)playerProfileData12.Flair[num6];
				}
				Player4ProfileName = playerProfileData12.Name[0];
				Player4Species = playerProfileData12.PlayerSpecies[0];
			}
			P4DpadLeftWaspressed = P4DpadLeftpressed;
			if (state4.DPad.Left == ButtonState.Released)
			{
				P4DpadLeftpressed = false;
			}
			P4Text = "   ";
			string assetName4 = "Sprites/" + Player4Species + "/head";
			Player4MenuTexture = base.Content.Load<Texture2D>(assetName4);
		}
		else if (P4MainMenuProgression == 2)
		{
			Player4InGame = true;
			Player4Ready = true;
			if (P1MainMenuProgression < 2 && P2MainMenuProgression < 2 && P3MainMenuProgression < 2)
			{
				P4InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P1InControlOfMainMenu = false;
			}
			if (P4InControlOfMainMenu)
			{
				if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadUppressed = true;
					MainMenuIndexer--;
				}
				P4DpadUpWaspressed = P4DpadUppressed;
				if (state4.DPad.Up == ButtonState.Released)
				{
					P4DpadUppressed = false;
				}
				if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P4DpadDownWaspressed = P4DpadDownpressed;
				if (state4.DPad.Down == ButtonState.Released)
				{
					P4DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P4MainMenuProgression == 3)
		{
			Player4InGame = true;
			Player4Ready = true;
			if (!P4InControlOfMainMenu)
			{
				return;
			}
			if (MainMenuIndexer == 0)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 3)
			{
				MainMenuFadeIn = false;
				MainMenuFadeOut = true;
				StartLevelBuilder = true;
			}
			else if (MainMenuIndexer == 4)
			{
				if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadUppressed = true;
					MainMenuIndexerOption--;
				}
				P4DpadUpWaspressed = P4DpadUppressed;
				if (state4.DPad.Up == ButtonState.Released)
				{
					P4DpadUppressed = false;
				}
				if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadDownpressed = true;
					MainMenuIndexerOption++;
				}
				P4DpadDownWaspressed = P4DpadDownpressed;
				if (state4.DPad.Down == ButtonState.Released)
				{
					P4DpadDownpressed = false;
				}
				if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
				{
					MainMenuIndexerOption = 0;
				}
				if (MainMenuIndexerOption < 0)
				{
					MainMenuIndexerOption = MainMenuIndexerOptionMax;
				}
				if (MainMenuIndexerOption == 0)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						Music_Volume += Volume_Step;
						P4DpadRightpressed = true;
						if (Music_Volume > 1f)
						{
							Music_Volume = 1f;
							MusicToggle = true;
						}
						MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						Music_Volume -= Volume_Step;
						P4DpadLeftpressed = true;
						if (Music_Volume < 0f)
						{
							Music_Volume = 0f;
							MusicToggle = false;
						}
						MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
				else if (MainMenuIndexerOption == 1)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						Sound_Effect_Volume += Volume_Step;
						P4DpadRightpressed = true;
						if (Sound_Effect_Volume > 1f)
						{
							Sound_Effect_Volume = 1f;
							SoundEffectToggle = true;
						}
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						Sound_Effect_Volume -= Volume_Step;
						P4DpadLeftpressed = true;
						if (Sound_Effect_Volume < 0f)
						{
							Sound_Effect_Volume = 0f;
							SoundEffectToggle = false;
						}
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
				else if (MainMenuIndexerOption == 2)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P4DpadRightpressed = true;
						BloodToggle = true;
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P4DpadLeftpressed = true;
						BloodToggle = false;
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
			}
			else if (MainMenuIndexer == 5)
			{
				MainMenuFadeIn = false;
				MainMenuFadeOut = true;
				ExitGame(gameTime);
			}
		}
		else
		{
			if (P4MainMenuProgression != 4 || !P4InControlOfMainMenu || !Player1Ready || !Player2Ready || !Player3Ready || !Player4Ready)
			{
				return;
			}
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
					return;
				}
				P4MainMenuProgression--;
				MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
					return;
				}
				P4MainMenuProgression--;
				MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
					return;
				}
				P4MainMenuProgression--;
				MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
	}

	private void HandleMainMenuInput2(GameTime gameTime)
	{
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		GamePadState state2 = GamePad.GetState(PlayerIndex.Two);
		GamePadState state3 = GamePad.GetState(PlayerIndex.Three);
		GamePadState state4 = GamePad.GetState(PlayerIndex.Four);
		Keyboard.GetState();
		if (!P1AWasPressed && state.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P1AWasPressed = true;
			P1MainMenuProgression++;
			if (P1MainMenuProgression > MainMenuProgressionMax)
			{
				P1MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P1MainMenuProgression = (int)MathHelper.Clamp(P1MainMenuProgression, 0f, 3f);
			}
		}
		if (state.Buttons.A == ButtonState.Released)
		{
			P1AWasPressed = false;
		}
		if (!P1BWasPressed && state.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P1BWasPressed = true;
			P1MainMenuProgression--;
			if (P1MainMenuProgression < 0)
			{
				P1MainMenuProgression = 0;
			}
		}
		if (state.Buttons.B == ButtonState.Released)
		{
			P1BWasPressed = false;
		}
		if (P1MainMenuProgression == 0)
		{
			Player1InGame = false;
			Player1Ready = true;
		}
		else if (P1MainMenuProgression == 1)
		{
			Player1InGame = true;
			Player1Ready = false;
			P1InControlOfMainMenu = false;
			if (Player1ProfileName == " ")
			{
				PlayerProfileData playerProfileData = LoadProfile($"{P1ProfileIndex}");
				for (int i = 0; i < FlairIndexMax + 1; i++)
				{
					Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
					P1FlairOld_Tag[i] = i;
					P1FlairOld[i] = (int)playerProfileData.Flair[i];
				}
				Player1ProfileName = playerProfileData.Name[0];
				Player1Species = playerProfileData.PlayerSpecies[0];
			}
			if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
			{
				if (P1FlairIndex == 8)
				{
					P1ProfileIndex++;
					if (P1ProfileIndex > ProfileMax)
					{
						P1ProfileIndex = 0;
					}
					PlayerProfileData playerProfileData2 = LoadProfile($"{P1ProfileIndex}");
					for (int j = 0; j < FlairIndexMax + 1; j++)
					{
						Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
						P1FlairOld_Tag[j] = j;
						P1FlairOld[j] = (int)playerProfileData2.Flair[j];
					}
					Player1ProfileName = playerProfileData2.Name[0];
					Player1Species = playerProfileData2.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P1Flair < FlairMax)
					{
						P1Flair++;
					}
					P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
					P1FlairOld[P1FlairIndex] = P1Flair;
				}
				P1DpadRightpressed = true;
			}
			P1DpadRightWaspressed = P1DpadRightpressed;
			if (state.DPad.Right == ButtonState.Released)
			{
				P1DpadRightpressed = false;
			}
			if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
			{
				if (P1FlairIndex == 8)
				{
					P1ProfileIndex--;
					if (P1ProfileIndex < 0)
					{
						P1ProfileIndex = ProfileMax;
					}
					PlayerProfileData playerProfileData3 = LoadProfile($"{P1ProfileIndex}");
					for (int k = 0; k < FlairIndexMax + 1; k++)
					{
						Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
						P1FlairOld_Tag[k] = k;
						P1FlairOld[k] = (int)playerProfileData3.Flair[k];
					}
					Player1ProfileName = playerProfileData3.Name[0];
					Player1Species = playerProfileData3.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P1Flair > 0)
					{
						P1Flair--;
					}
					_ = "Flair/" + Player1Species.ToString() + "/" + P1FlairIndex + "/" + P1Flair;
					P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
					P1FlairOld[P1FlairIndex] = P1Flair;
				}
				P1DpadLeftpressed = true;
			}
			P1DpadLeftWaspressed = P1DpadLeftpressed;
			if (state.DPad.Left == ButtonState.Released)
			{
				P1DpadLeftpressed = false;
			}
			if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P1DpadUppressed = true;
				P1FlairIndex--;
				if (P1FlairIndex < 0)
				{
					P1FlairIndex = FlairIndexMax;
				}
				P1Flair = P1FlairOld[P1FlairIndex];
				P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
			}
			P1DpadUpWaspressed = P1DpadUppressed;
			if (state.DPad.Up == ButtonState.Released)
			{
				P1DpadUppressed = false;
			}
			if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P1DpadDownpressed = true;
				P1FlairIndex++;
				if (P1FlairIndex > FlairIndexMax)
				{
					P1FlairIndex = 0;
				}
				P1Flair = P1FlairOld[P1FlairIndex];
				P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
			}
			P1DpadDownWaspressed = P1DpadDownpressed;
			if (state.DPad.Down == ButtonState.Released)
			{
				P1DpadDownpressed = false;
			}
			string assetName = "Sprites/" + Player1Species + "/head";
			Player1MenuTexture = base.Content.Load<Texture2D>(assetName);
			if (P1FlairIndex == 8)
			{
				P1Text = "Class";
			}
			if (P1FlairIndex == 0)
			{
				P1Text = "Eyes";
			}
			if (P1FlairIndex == 1)
			{
				P1Text = "Beard";
			}
			if (P1FlairIndex == 2)
			{
				P1Text = "Mouth";
			}
			if (P1FlairIndex == 3)
			{
				P1Text = "Hair";
			}
			if (P1FlairIndex == 4)
			{
				P1Text = "Mask";
			}
			if (P1FlairIndex == 5)
			{
				P1Text = "Hat";
			}
			if (P1FlairIndex == 6)
			{
				P1Text = "BodyFront";
			}
			if (P1FlairIndex == 7)
			{
				P1Text = "BodyBack";
			}
		}
		else if (P1MainMenuProgression == 2)
		{
			Player1InGame = true;
			Player1Ready = true;
			if (P2MainMenuProgression < 2 && P3MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P1InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P1InControlOfMainMenu)
			{
				if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P1DpadUppressed = true;
					MainMenuIndexer--;
				}
				P1DpadUpWaspressed = P1DpadUppressed;
				if (state.DPad.Up == ButtonState.Released)
				{
					P1DpadUppressed = false;
				}
				if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P1DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P1DpadDownWaspressed = P1DpadDownpressed;
				if (state.DPad.Down == ButtonState.Released)
				{
					P1DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P1MainMenuProgression == 3)
		{
			Player1InGame = true;
			Player1Ready = true;
			if (P1InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P1DpadUpWaspressed = P1DpadUppressed;
					if (state.DPad.Up == ButtonState.Released)
					{
						P1DpadUppressed = false;
					}
					if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P1DpadDownWaspressed = P1DpadDownpressed;
					if (state.DPad.Down == ButtonState.Released)
					{
						P1DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P1DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P1DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P1DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P1DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P1DpadRightpressed = true;
							BloodToggle = true;
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P1DpadLeftpressed = true;
							BloodToggle = false;
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P1MainMenuProgression == 4 && P1InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P1MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P1MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P1MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P2AWasPressed && state2.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P2AWasPressed = true;
			P2MainMenuProgression++;
			if (P2MainMenuProgression > MainMenuProgressionMax)
			{
				P2MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P2MainMenuProgression = (int)MathHelper.Clamp(P2MainMenuProgression, 0f, 3f);
			}
		}
		if (state2.Buttons.A == ButtonState.Released)
		{
			P2AWasPressed = false;
		}
		if (!P2BWasPressed && state2.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P2BWasPressed = true;
			P2MainMenuProgression--;
			if (P2MainMenuProgression < 0)
			{
				P2MainMenuProgression = 0;
			}
		}
		if (state2.Buttons.B == ButtonState.Released)
		{
			P2BWasPressed = false;
		}
		if (P2MainMenuProgression == 0)
		{
			Player2InGame = false;
			Player2Ready = true;
		}
		else if (P2MainMenuProgression == 1)
		{
			Player2InGame = true;
			Player2Ready = false;
			P2InControlOfMainMenu = false;
			if (Player2ProfileName == " ")
			{
				PlayerProfileData playerProfileData4 = LoadProfile($"{P2ProfileIndex}");
				for (int l = 0; l < FlairIndexMax + 1; l++)
				{
					Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
					P2FlairOld_Tag[l] = l;
					P2FlairOld[l] = (int)playerProfileData4.Flair[l];
				}
				Player2ProfileName = playerProfileData4.Name[0];
				Player2Species = playerProfileData4.PlayerSpecies[0];
			}
			if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
			{
				if (P2FlairIndex == 8)
				{
					P2ProfileIndex++;
					if (P2ProfileIndex > ProfileMax)
					{
						P2ProfileIndex = 0;
					}
					PlayerProfileData playerProfileData5 = LoadProfile($"{P2ProfileIndex}");
					for (int m = 0; m < FlairIndexMax + 1; m++)
					{
						Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
						P2FlairOld_Tag[m] = m;
						P2FlairOld[m] = (int)playerProfileData5.Flair[m];
					}
					Player2ProfileName = playerProfileData5.Name[0];
					Player2Species = playerProfileData5.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P2Flair < FlairMax)
					{
						P2Flair++;
					}
					P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
					P2FlairOld[P2FlairIndex] = P2Flair;
				}
				P2DpadRightpressed = true;
			}
			P2DpadRightWaspressed = P2DpadRightpressed;
			if (state2.DPad.Right == ButtonState.Released)
			{
				P2DpadRightpressed = false;
			}
			if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
			{
				if (P2FlairIndex == 8)
				{
					P2ProfileIndex--;
					if (P2ProfileIndex < 0)
					{
						P2ProfileIndex = ProfileMax;
					}
					PlayerProfileData playerProfileData6 = LoadProfile($"{P2ProfileIndex}");
					for (int n = 0; n < FlairIndexMax + 1; n++)
					{
						Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
						P2FlairOld_Tag[n] = n;
						P2FlairOld[n] = (int)playerProfileData6.Flair[n];
					}
					Player2ProfileName = playerProfileData6.Name[0];
					Player2Species = playerProfileData6.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P2Flair > 0)
					{
						P2Flair--;
					}
					_ = "Flair/" + Player2Species.ToString() + "/" + P2FlairIndex + "/" + P2Flair;
					P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
					P2FlairOld[P2FlairIndex] = P2Flair;
				}
				P2DpadLeftpressed = true;
			}
			P2DpadLeftWaspressed = P2DpadLeftpressed;
			if (state2.DPad.Left == ButtonState.Released)
			{
				P2DpadLeftpressed = false;
			}
			if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P2DpadUppressed = true;
				P2FlairIndex--;
				if (P2FlairIndex < 0)
				{
					P2FlairIndex = FlairIndexMax;
				}
				P2Flair = P2FlairOld[P2FlairIndex];
				P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
			}
			P2DpadUpWaspressed = P2DpadUppressed;
			if (state2.DPad.Up == ButtonState.Released)
			{
				P2DpadUppressed = false;
			}
			if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P2DpadDownpressed = true;
				P2FlairIndex++;
				if (P2FlairIndex > FlairIndexMax)
				{
					P2FlairIndex = 0;
				}
				P2Flair = P2FlairOld[P2FlairIndex];
				P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
			}
			P2DpadDownWaspressed = P2DpadDownpressed;
			if (state2.DPad.Down == ButtonState.Released)
			{
				P2DpadDownpressed = false;
			}
			string assetName2 = "Sprites/" + Player2Species + "/head";
			Player2MenuTexture = base.Content.Load<Texture2D>(assetName2);
			if (P2FlairIndex == 8)
			{
				P2Text = "Class";
			}
			if (P2FlairIndex == 0)
			{
				P2Text = "Eyes";
			}
			if (P2FlairIndex == 1)
			{
				P2Text = "Beard";
			}
			if (P2FlairIndex == 2)
			{
				P2Text = "Mouth";
			}
			if (P2FlairIndex == 3)
			{
				P2Text = "Hair";
			}
			if (P2FlairIndex == 4)
			{
				P2Text = "Mask";
			}
			if (P2FlairIndex == 5)
			{
				P2Text = "Hat";
			}
			if (P2FlairIndex == 6)
			{
				P2Text = "BodyFront";
			}
			if (P2FlairIndex == 7)
			{
				P2Text = "BodyBack";
			}
		}
		else if (P2MainMenuProgression == 2)
		{
			Player2InGame = true;
			Player2Ready = true;
			if (P1MainMenuProgression < 2 && P3MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P2InControlOfMainMenu = true;
				P1InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P2InControlOfMainMenu)
			{
				if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P2DpadUppressed = true;
					MainMenuIndexer--;
				}
				P2DpadUpWaspressed = P2DpadUppressed;
				if (state2.DPad.Up == ButtonState.Released)
				{
					P2DpadUppressed = false;
				}
				if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P2DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P2DpadDownWaspressed = P2DpadDownpressed;
				if (state2.DPad.Down == ButtonState.Released)
				{
					P2DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P2MainMenuProgression == 3)
		{
			Player2InGame = true;
			Player2Ready = true;
			if (P2InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P2DpadUpWaspressed = P2DpadUppressed;
					if (state2.DPad.Up == ButtonState.Released)
					{
						P2DpadUppressed = false;
					}
					if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P2DpadDownWaspressed = P2DpadDownpressed;
					if (state2.DPad.Down == ButtonState.Released)
					{
						P2DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P2DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P2DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P2DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P2DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P2DpadRightpressed = true;
							BloodToggle = true;
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P2DpadLeftpressed = true;
							BloodToggle = false;
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P2MainMenuProgression == 4 && P2InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P2MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P2MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P2MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P3AWasPressed && state3.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P3AWasPressed = true;
			P3MainMenuProgression++;
			if (P3MainMenuProgression > MainMenuProgressionMax)
			{
				P3MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P3MainMenuProgression = (int)MathHelper.Clamp(P3MainMenuProgression, 0f, 3f);
			}
		}
		if (state3.Buttons.A == ButtonState.Released)
		{
			P3AWasPressed = false;
		}
		if (!P3BWasPressed && state3.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P3BWasPressed = true;
			P3MainMenuProgression--;
			if (P3MainMenuProgression < 0)
			{
				P3MainMenuProgression = 0;
			}
		}
		if (state3.Buttons.B == ButtonState.Released)
		{
			P3BWasPressed = false;
		}
		if (P3MainMenuProgression == 0)
		{
			Player3InGame = false;
			Player3Ready = true;
		}
		else if (P3MainMenuProgression == 1)
		{
			Player3InGame = true;
			Player3Ready = false;
			P3InControlOfMainMenu = false;
			if (Player3ProfileName == " ")
			{
				PlayerProfileData playerProfileData7 = LoadProfile($"{P3ProfileIndex}");
				for (int num = 0; num < FlairIndexMax + 1; num++)
				{
					Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
					P3FlairOld_Tag[num] = num;
					P3FlairOld[num] = (int)playerProfileData7.Flair[num];
				}
				Player3ProfileName = playerProfileData7.Name[0];
				Player3Species = playerProfileData7.PlayerSpecies[0];
			}
			if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
			{
				if (P3FlairIndex == 8)
				{
					P3ProfileIndex++;
					if (P3ProfileIndex > ProfileMax)
					{
						P3ProfileIndex = 0;
					}
					PlayerProfileData playerProfileData8 = LoadProfile($"{P3ProfileIndex}");
					for (int num2 = 0; num2 < FlairIndexMax + 1; num2++)
					{
						Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
						P3FlairOld_Tag[num2] = num2;
						P3FlairOld[num2] = (int)playerProfileData8.Flair[num2];
					}
					Player3ProfileName = playerProfileData8.Name[0];
					Player3Species = playerProfileData8.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P3Flair < FlairMax)
					{
						P3Flair++;
					}
					P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
					P3FlairOld[P3FlairIndex] = P3Flair;
				}
				P3DpadRightpressed = true;
			}
			P3DpadRightWaspressed = P3DpadRightpressed;
			if (state3.DPad.Right == ButtonState.Released)
			{
				P3DpadRightpressed = false;
			}
			if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
			{
				if (P3FlairIndex == 8)
				{
					P3ProfileIndex--;
					if (P3ProfileIndex < 0)
					{
						P3ProfileIndex = ProfileMax;
					}
					PlayerProfileData playerProfileData9 = LoadProfile($"{P3ProfileIndex}");
					for (int num3 = 0; num3 < FlairIndexMax + 1; num3++)
					{
						Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
						P3FlairOld_Tag[num3] = num3;
						P3FlairOld[num3] = (int)playerProfileData9.Flair[num3];
					}
					Player3ProfileName = playerProfileData9.Name[0];
					Player3Species = playerProfileData9.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P3Flair > 0)
					{
						P3Flair--;
					}
					_ = "Flair/" + Player3Species.ToString() + "/" + P3FlairIndex + "/" + P3Flair;
					P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
					P3FlairOld[P3FlairIndex] = P3Flair;
				}
				P3DpadLeftpressed = true;
			}
			P3DpadLeftWaspressed = P3DpadLeftpressed;
			if (state3.DPad.Left == ButtonState.Released)
			{
				P3DpadLeftpressed = false;
			}
			if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P3DpadUppressed = true;
				P3FlairIndex--;
				if (P3FlairIndex < 0)
				{
					P3FlairIndex = FlairIndexMax;
				}
				P3Flair = P3FlairOld[P3FlairIndex];
				P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
			}
			P3DpadUpWaspressed = P3DpadUppressed;
			if (state3.DPad.Up == ButtonState.Released)
			{
				P3DpadUppressed = false;
			}
			if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P3DpadDownpressed = true;
				P3FlairIndex++;
				if (P3FlairIndex > FlairIndexMax)
				{
					P3FlairIndex = 0;
				}
				P3Flair = P3FlairOld[P3FlairIndex];
				P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
			}
			P3DpadDownWaspressed = P3DpadDownpressed;
			if (state3.DPad.Down == ButtonState.Released)
			{
				P3DpadDownpressed = false;
			}
			string assetName3 = "Sprites/" + Player3Species + "/head";
			Player3MenuTexture = base.Content.Load<Texture2D>(assetName3);
			if (P3FlairIndex == 8)
			{
				P3Text = "Class";
			}
			if (P3FlairIndex == 0)
			{
				P3Text = "Eyes";
			}
			if (P3FlairIndex == 1)
			{
				P3Text = "Beard";
			}
			if (P3FlairIndex == 2)
			{
				P3Text = "Mouth";
			}
			if (P3FlairIndex == 3)
			{
				P3Text = "Hair";
			}
			if (P3FlairIndex == 4)
			{
				P3Text = "Mask";
			}
			if (P3FlairIndex == 5)
			{
				P3Text = "Hat";
			}
			if (P3FlairIndex == 6)
			{
				P3Text = "BodyFront";
			}
			if (P3FlairIndex == 7)
			{
				P3Text = "BodyBack";
			}
		}
		else if (P3MainMenuProgression == 2)
		{
			Player3InGame = true;
			Player3Ready = true;
			if (P1MainMenuProgression < 2 && P2MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P3InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P1InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P3InControlOfMainMenu)
			{
				if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P3DpadUppressed = true;
					MainMenuIndexer--;
				}
				P3DpadUpWaspressed = P3DpadUppressed;
				if (state3.DPad.Up == ButtonState.Released)
				{
					P3DpadUppressed = false;
				}
				if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P3DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P3DpadDownWaspressed = P3DpadDownpressed;
				if (state3.DPad.Down == ButtonState.Released)
				{
					P3DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P3MainMenuProgression == 3)
		{
			Player3InGame = true;
			Player3Ready = true;
			if (P3InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuIndexer == 0)
					{
						MainMenuLevelIndexerMax = 18;
					}
					else if (MainMenuIndexer == 1)
					{
						MainMenuLevelIndexerMax = 5;
					}
					if (MainMenuIndexer == 2)
					{
						MainMenuLevelIndexerMax = 9;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P3DpadUpWaspressed = P3DpadUppressed;
					if (state3.DPad.Up == ButtonState.Released)
					{
						P3DpadUppressed = false;
					}
					if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P3DpadDownWaspressed = P3DpadDownpressed;
					if (state3.DPad.Down == ButtonState.Released)
					{
						P3DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P3DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P3DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P3DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P3DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P3DpadRightpressed = true;
							BloodToggle = true;
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P3DpadLeftpressed = true;
							BloodToggle = false;
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P3MainMenuProgression == 4 && P3InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P3MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P3MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
				}
				else
				{
					P3MainMenuProgression--;
					MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
				}
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P4AWasPressed && state4.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P4AWasPressed = true;
			P4MainMenuProgression++;
			if (P4MainMenuProgression > MainMenuProgressionMax)
			{
				P4MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P4MainMenuProgression = (int)MathHelper.Clamp(P4MainMenuProgression, 0f, 3f);
			}
		}
		if (state4.Buttons.A == ButtonState.Released)
		{
			P4AWasPressed = false;
		}
		if (!P4BWasPressed && state4.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P4BWasPressed = true;
			P4MainMenuProgression--;
			if (P4MainMenuProgression < 0)
			{
				P4MainMenuProgression = 0;
			}
		}
		if (state4.Buttons.B == ButtonState.Released)
		{
			P4BWasPressed = false;
		}
		if (P4MainMenuProgression == 0)
		{
			Player4InGame = false;
			Player4Ready = true;
		}
		else if (P4MainMenuProgression == 1)
		{
			Player4InGame = true;
			Player4Ready = false;
			P4InControlOfMainMenu = false;
			if (Player4ProfileName == " ")
			{
				PlayerProfileData playerProfileData10 = LoadProfile($"{P4ProfileIndex}");
				for (int num4 = 0; num4 < FlairIndexMax + 1; num4++)
				{
					Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
					P4FlairOld_Tag[num4] = num4;
					P4FlairOld[num4] = (int)playerProfileData10.Flair[num4];
				}
				Player4ProfileName = playerProfileData10.Name[0];
				Player4Species = playerProfileData10.PlayerSpecies[0];
			}
			if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
			{
				if (P4FlairIndex == 8)
				{
					P4ProfileIndex++;
					if (P4ProfileIndex > ProfileMax)
					{
						P4ProfileIndex = 0;
					}
					PlayerProfileData playerProfileData11 = LoadProfile($"{P4ProfileIndex}");
					for (int num5 = 0; num5 < FlairIndexMax + 1; num5++)
					{
						Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
						P4FlairOld_Tag[num5] = num5;
						P4FlairOld[num5] = (int)playerProfileData11.Flair[num5];
					}
					Player4ProfileName = playerProfileData11.Name[0];
					Player4Species = playerProfileData11.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P4Flair < FlairMax)
					{
						P4Flair++;
					}
					P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
					P4FlairOld[P4FlairIndex] = P4Flair;
				}
				P4DpadRightpressed = true;
			}
			P4DpadRightWaspressed = P4DpadRightpressed;
			if (state4.DPad.Right == ButtonState.Released)
			{
				P4DpadRightpressed = false;
			}
			if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
			{
				if (P4FlairIndex == 8)
				{
					P4ProfileIndex--;
					if (P4ProfileIndex < 0)
					{
						P4ProfileIndex = ProfileMax;
					}
					PlayerProfileData playerProfileData12 = LoadProfile($"{P4ProfileIndex}");
					for (int num6 = 0; num6 < FlairIndexMax + 1; num6++)
					{
						Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
						P4FlairOld_Tag[num6] = num6;
						P4FlairOld[num6] = (int)playerProfileData12.Flair[num6];
					}
					Player4ProfileName = playerProfileData12.Name[0];
					Player4Species = playerProfileData12.PlayerSpecies[0];
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				}
				else
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					if (P4Flair > 0)
					{
						P4Flair--;
					}
					_ = "Flair/" + Player4Species.ToString() + "/" + P4FlairIndex + "/" + P4Flair;
					P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
					P4FlairOld[P4FlairIndex] = P4Flair;
				}
				P4DpadLeftpressed = true;
			}
			P4DpadLeftWaspressed = P4DpadLeftpressed;
			if (state4.DPad.Left == ButtonState.Released)
			{
				P4DpadLeftpressed = false;
			}
			if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P4DpadUppressed = true;
				P4FlairIndex--;
				if (P4FlairIndex < 0)
				{
					P4FlairIndex = FlairIndexMax;
				}
				P4Flair = P4FlairOld[P4FlairIndex];
				P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
			}
			P4DpadUpWaspressed = P4DpadUppressed;
			if (state4.DPad.Up == ButtonState.Released)
			{
				P4DpadUppressed = false;
			}
			if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P4DpadDownpressed = true;
				P4FlairIndex++;
				if (P4FlairIndex > FlairIndexMax)
				{
					P4FlairIndex = 0;
				}
				P4Flair = P4FlairOld[P4FlairIndex];
				P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
			}
			P4DpadDownWaspressed = P4DpadDownpressed;
			if (state4.DPad.Down == ButtonState.Released)
			{
				P4DpadDownpressed = false;
			}
			string assetName4 = "Sprites/" + Player4Species + "/head";
			Player4MenuTexture = base.Content.Load<Texture2D>(assetName4);
			if (P4FlairIndex == 8)
			{
				P4Text = "Class";
			}
			if (P4FlairIndex == 0)
			{
				P4Text = "Eyes";
			}
			if (P4FlairIndex == 1)
			{
				P4Text = "Beard";
			}
			if (P4FlairIndex == 2)
			{
				P4Text = "Mouth";
			}
			if (P4FlairIndex == 3)
			{
				P4Text = "Hair";
			}
			if (P4FlairIndex == 4)
			{
				P4Text = "Mask";
			}
			if (P4FlairIndex == 5)
			{
				P4Text = "Hat";
			}
			if (P4FlairIndex == 6)
			{
				P4Text = "BodyFront";
			}
			if (P4FlairIndex == 7)
			{
				P4Text = "BodyBack";
			}
		}
		else if (P4MainMenuProgression == 2)
		{
			Player4InGame = true;
			Player4Ready = true;
			if (P1MainMenuProgression < 2 && P2MainMenuProgression < 2 && P3MainMenuProgression < 2)
			{
				P4InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P1InControlOfMainMenu = false;
			}
			if (P4InControlOfMainMenu)
			{
				if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadUppressed = true;
					MainMenuIndexer--;
				}
				P4DpadUpWaspressed = P4DpadUppressed;
				if (state4.DPad.Up == ButtonState.Released)
				{
					P4DpadUppressed = false;
				}
				if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P4DpadDownWaspressed = P4DpadDownpressed;
				if (state4.DPad.Down == ButtonState.Released)
				{
					P4DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P4MainMenuProgression == 3)
		{
			Player4InGame = true;
			Player4Ready = true;
			if (!P4InControlOfMainMenu)
			{
				return;
			}
			if (MainMenuIndexer == 0)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuIndexer == 0)
				{
					MainMenuLevelIndexerMax = 18;
				}
				else if (MainMenuIndexer == 1)
				{
					MainMenuLevelIndexerMax = 5;
				}
				if (MainMenuIndexer == 2)
				{
					MainMenuLevelIndexerMax = 9;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuIndexer == 0)
				{
					MainMenuLevelIndexerMax = 18;
				}
				else if (MainMenuIndexer == 1)
				{
					MainMenuLevelIndexerMax = 5;
				}
				if (MainMenuIndexer == 2)
				{
					MainMenuLevelIndexerMax = 9;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuIndexer == 0)
				{
					MainMenuLevelIndexerMax = 18;
				}
				else if (MainMenuIndexer == 1)
				{
					MainMenuLevelIndexerMax = 5;
				}
				if (MainMenuIndexer == 2)
				{
					MainMenuLevelIndexerMax = 9;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 3)
			{
				MainMenuFadeIn = false;
				MainMenuFadeOut = true;
				StartLevelBuilder = true;
			}
			else if (MainMenuIndexer == 4)
			{
				if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadUppressed = true;
					MainMenuIndexerOption--;
				}
				P4DpadUpWaspressed = P4DpadUppressed;
				if (state4.DPad.Up == ButtonState.Released)
				{
					P4DpadUppressed = false;
				}
				if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadDownpressed = true;
					MainMenuIndexerOption++;
				}
				P4DpadDownWaspressed = P4DpadDownpressed;
				if (state4.DPad.Down == ButtonState.Released)
				{
					P4DpadDownpressed = false;
				}
				if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
				{
					MainMenuIndexerOption = 0;
				}
				if (MainMenuIndexerOption < 0)
				{
					MainMenuIndexerOption = MainMenuIndexerOptionMax;
				}
				if (MainMenuIndexerOption == 0)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						Music_Volume += Volume_Step;
						P4DpadRightpressed = true;
						if (Music_Volume > 1f)
						{
							Music_Volume = 1f;
							MusicToggle = true;
						}
						MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						Music_Volume -= Volume_Step;
						P4DpadLeftpressed = true;
						if (Music_Volume < 0f)
						{
							Music_Volume = 0f;
							MusicToggle = false;
						}
						MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
				else if (MainMenuIndexerOption == 1)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						Sound_Effect_Volume += Volume_Step;
						P4DpadRightpressed = true;
						if (Sound_Effect_Volume > 1f)
						{
							Sound_Effect_Volume = 1f;
							SoundEffectToggle = true;
						}
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						Sound_Effect_Volume -= Volume_Step;
						P4DpadLeftpressed = true;
						if (Sound_Effect_Volume < 0f)
						{
							Sound_Effect_Volume = 0f;
							SoundEffectToggle = false;
						}
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
				else if (MainMenuIndexerOption == 2)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P4DpadRightpressed = true;
						BloodToggle = true;
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P4DpadLeftpressed = true;
						BloodToggle = false;
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
			}
			else if (MainMenuIndexer == 5)
			{
				MainMenuFadeIn = false;
				MainMenuFadeOut = true;
				ExitGame(gameTime);
			}
		}
		else
		{
			if (P4MainMenuProgression != 4 || !P4InControlOfMainMenu || !Player1Ready || !Player2Ready || !Player3Ready || !Player4Ready)
			{
				return;
			}
			if (MainMenuIndexer == 0)
			{
				if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
				{
					StartGame = true;
					return;
				}
				P4MainMenuProgression--;
				MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
			}
			else if (MainMenuIndexer == 1)
			{
				if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
				{
					StartGame = true;
					return;
				}
				P4MainMenuProgression--;
				MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
			}
			else if (MainMenuIndexer == 2)
			{
				if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
				{
					StartGame = true;
					return;
				}
				P4MainMenuProgression--;
				MenuClickSound.Play(Sound_Effect_Volume, -1f, 0f);
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
	}

	private void HandleMainMenuInput2_Flair_OLD(GameTime gameTime)
	{
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		GamePadState state2 = GamePad.GetState(PlayerIndex.Two);
		GamePadState state3 = GamePad.GetState(PlayerIndex.Three);
		GamePadState state4 = GamePad.GetState(PlayerIndex.Four);
		Keyboard.GetState();
		if (!P1AWasPressed && state.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P1AWasPressed = true;
			P1MainMenuProgression++;
			if (P1MainMenuProgression > MainMenuProgressionMax)
			{
				P1MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P1MainMenuProgression = (int)MathHelper.Clamp(P1MainMenuProgression, 0f, 3f);
			}
		}
		if (state.Buttons.A == ButtonState.Released)
		{
			P1AWasPressed = false;
		}
		if (!P1BWasPressed && state.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P1BWasPressed = true;
			P1MainMenuProgression--;
			if (P1MainMenuProgression < 0)
			{
				P1MainMenuProgression = 0;
			}
		}
		if (state.Buttons.B == ButtonState.Released)
		{
			P1BWasPressed = false;
		}
		if (P1MainMenuProgression == 0)
		{
			Player1InGame = false;
			Player1Ready = true;
		}
		else if (P1MainMenuProgression == 1)
		{
			Player1InGame = true;
			Player1Ready = false;
			P1InControlOfMainMenu = false;
			if (Player1ProfileName == " ")
			{
				PlayerProfileData playerProfileData = LoadProfile($"{P1ProfileIndex}");
				for (int i = 0; i < FlairIndexMax + 1; i++)
				{
					Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
					P1FlairOld_Tag[i] = i;
					P1FlairOld[i] = (int)playerProfileData.Flair[i];
				}
				Player1ProfileName = playerProfileData.Name[0];
				Player1Species = playerProfileData.PlayerSpecies[0];
			}
			if (!P1ShoulderRightWaspressed && state.Buttons.RightShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P1ShoulderRightpressed = true;
				P1ProfileIndex++;
				if (P1ProfileIndex > ProfileMax)
				{
					P1ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData2 = LoadProfile($"{P1ProfileIndex}");
				for (int j = 0; j < FlairIndexMax + 1; j++)
				{
					Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
					P1FlairOld_Tag[j] = j;
					P1FlairOld[j] = (int)playerProfileData2.Flair[j];
				}
				Player1ProfileName = playerProfileData2.Name[0];
				Player1Species = playerProfileData2.PlayerSpecies[0];
			}
			P1ShoulderRightWaspressed = P1ShoulderRightpressed;
			if (state.Buttons.RightShoulder == ButtonState.Released)
			{
				P1ShoulderRightpressed = false;
			}
			if (!P1ShoulderLeftWaspressed && state.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P1ShoulderLeftpressed = true;
				P1ProfileIndex--;
				if (P1ProfileIndex < 0)
				{
					P1ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData3 = LoadProfile($"{P1ProfileIndex}");
				for (int k = 0; k < FlairIndexMax + 1; k++)
				{
					Player1SpriteSheet = PlayerSpriteSheet[P1ProfileIndex];
					P1FlairOld_Tag[k] = k;
					P1FlairOld[k] = (int)playerProfileData3.Flair[k];
				}
				Player1ProfileName = playerProfileData3.Name[0];
				Player1Species = playerProfileData3.PlayerSpecies[0];
			}
			P1ShoulderLeftWaspressed = P1ShoulderLeftpressed;
			if (state.Buttons.LeftShoulder == ButtonState.Released)
			{
				P1ShoulderLeftpressed = false;
			}
			if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P1DpadRightpressed = true;
				if (P1Flair < FlairMax)
				{
					P1Flair++;
				}
				P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
				P1FlairOld[P1FlairIndex] = P1Flair;
			}
			P1DpadRightWaspressed = P1DpadRightpressed;
			if (state.DPad.Right == ButtonState.Released)
			{
				P1DpadRightpressed = false;
			}
			if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P1DpadLeftpressed = true;
				if (P1Flair > 0)
				{
					P1Flair--;
				}
				_ = "Flair/" + Player1Species.ToString() + "/" + P1FlairIndex + "/" + P1Flair;
				P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
				P1FlairOld[P1FlairIndex] = P1Flair;
			}
			P1DpadLeftWaspressed = P1DpadLeftpressed;
			if (state.DPad.Left == ButtonState.Released)
			{
				P1DpadLeftpressed = false;
			}
			if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P1DpadUppressed = true;
				if (P1FlairIndex > 0)
				{
					P1FlairIndex--;
				}
				P1Flair = P1FlairOld[P1FlairIndex];
				P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
			}
			P1DpadUpWaspressed = P1DpadUppressed;
			if (state.DPad.Up == ButtonState.Released)
			{
				P1DpadUppressed = false;
			}
			if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P1DpadDownpressed = true;
				if (P1FlairIndex < FlairIndexMax)
				{
					P1FlairIndex++;
				}
				P1Flair = P1FlairOld[P1FlairIndex];
				P1FlairOld_Tag[P1FlairIndex] = P1FlairIndex;
			}
			P1DpadDownWaspressed = P1DpadDownpressed;
			if (state.DPad.Down == ButtonState.Released)
			{
				P1DpadDownpressed = false;
			}
			string assetName = "Sprites/" + Player1Species + "/head";
			Player1MenuTexture = base.Content.Load<Texture2D>(assetName);
			if (P1FlairIndex == 0)
			{
				P1Text = "Eyes";
			}
			if (P1FlairIndex == 1)
			{
				P1Text = "Beard";
			}
			if (P1FlairIndex == 2)
			{
				P1Text = "Mouth";
			}
			if (P1FlairIndex == 3)
			{
				P1Text = "Hair";
			}
			if (P1FlairIndex == 4)
			{
				P1Text = "Mask";
			}
			if (P1FlairIndex == 5)
			{
				P1Text = "Hat";
			}
			if (P1FlairIndex == 6)
			{
				P1Text = "BodyFront";
			}
			if (P1FlairIndex == 7)
			{
				P1Text = "BodyBack";
			}
			if (P1FlairIndex == 8)
			{
				P1Text = "LegsFront";
			}
			if (P1FlairIndex == 9)
			{
				P1Text = "LegsBack";
			}
		}
		else if (P1MainMenuProgression == 2)
		{
			Player1InGame = true;
			Player1Ready = true;
			if (P2MainMenuProgression < 2 && P3MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P1InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P1InControlOfMainMenu)
			{
				if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P1DpadUppressed = true;
					MainMenuIndexer--;
				}
				P1DpadUpWaspressed = P1DpadUppressed;
				if (state.DPad.Up == ButtonState.Released)
				{
					P1DpadUppressed = false;
				}
				if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P1DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P1DpadDownWaspressed = P1DpadDownpressed;
				if (state.DPad.Down == ButtonState.Released)
				{
					P1DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P1MainMenuProgression == 3)
		{
			Player1InGame = true;
			Player1Ready = true;
			if (P1InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P1DpadRightWaspressed = P1DpadRightpressed;
					if (state.DPad.Right == ButtonState.Released)
					{
						P1DpadRightpressed = false;
					}
					if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P1DpadLeftWaspressed = P1DpadLeftpressed;
					if (state.DPad.Left == ButtonState.Released)
					{
						P1DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P1DpadUpWaspressed && state.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P1DpadUpWaspressed = P1DpadUppressed;
					if (state.DPad.Up == ButtonState.Released)
					{
						P1DpadUppressed = false;
					}
					if (!P1DpadDownWaspressed && state.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P1DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P1DpadDownWaspressed = P1DpadDownpressed;
					if (state.DPad.Down == ButtonState.Released)
					{
						P1DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P1DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P1DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P1DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P1DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P1DpadRightWaspressed && state.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P1DpadRightpressed = true;
							BloodToggle = true;
						}
						P1DpadRightWaspressed = P1DpadRightpressed;
						if (state.DPad.Right == ButtonState.Released)
						{
							P1DpadRightpressed = false;
						}
						if (!P1DpadLeftWaspressed && state.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P1DpadLeftpressed = true;
							BloodToggle = false;
						}
						P1DpadLeftWaspressed = P1DpadLeftpressed;
						if (state.DPad.Left == ButtonState.Released)
						{
							P1DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P1MainMenuProgression == 4 && P1InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 1)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 2)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P2AWasPressed && state2.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P2AWasPressed = true;
			P2MainMenuProgression++;
			if (P2MainMenuProgression > MainMenuProgressionMax)
			{
				P2MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P2MainMenuProgression = (int)MathHelper.Clamp(P2MainMenuProgression, 0f, 3f);
			}
		}
		if (state2.Buttons.A == ButtonState.Released)
		{
			P2AWasPressed = false;
		}
		if (!P2BWasPressed && state2.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P2BWasPressed = true;
			P2MainMenuProgression--;
			if (P2MainMenuProgression < 0)
			{
				P2MainMenuProgression = 0;
			}
		}
		if (state2.Buttons.B == ButtonState.Released)
		{
			P2BWasPressed = false;
		}
		if (P2MainMenuProgression == 0)
		{
			Player2InGame = false;
			Player2Ready = true;
		}
		else if (P2MainMenuProgression == 1)
		{
			Player2InGame = true;
			Player2Ready = false;
			P2InControlOfMainMenu = false;
			if (Player2ProfileName == " ")
			{
				PlayerProfileData playerProfileData4 = LoadProfile($"{P2ProfileIndex}");
				for (int l = 0; l < FlairIndexMax + 1; l++)
				{
					Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
					P2FlairOld_Tag[l] = l;
					P2FlairOld[l] = (int)playerProfileData4.Flair[l];
				}
				Player2ProfileName = playerProfileData4.Name[0];
				Player2Species = playerProfileData4.PlayerSpecies[0];
			}
			if (!P2ShoulderRightWaspressed && state2.Buttons.RightShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P2ShoulderRightpressed = true;
				P2ProfileIndex++;
				if (P2ProfileIndex > ProfileMax)
				{
					P2ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData5 = LoadProfile($"{P2ProfileIndex}");
				for (int m = 0; m < FlairIndexMax + 1; m++)
				{
					Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
					P2FlairOld_Tag[m] = m;
					P2FlairOld[m] = (int)playerProfileData5.Flair[m];
				}
				Player2ProfileName = playerProfileData5.Name[0];
				Player2Species = playerProfileData5.PlayerSpecies[0];
			}
			P2ShoulderRightWaspressed = P2ShoulderRightpressed;
			if (state2.Buttons.RightShoulder == ButtonState.Released)
			{
				P2ShoulderRightpressed = false;
			}
			if (!P2ShoulderLeftWaspressed && state2.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P2ShoulderLeftpressed = true;
				P2ProfileIndex--;
				if (P2ProfileIndex < 0)
				{
					P2ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData6 = LoadProfile($"{P2ProfileIndex}");
				for (int n = 0; n < FlairIndexMax + 1; n++)
				{
					Player2SpriteSheet = PlayerSpriteSheet[P2ProfileIndex];
					P2FlairOld_Tag[n] = n;
					P2FlairOld[n] = (int)playerProfileData6.Flair[n];
				}
				Player2ProfileName = playerProfileData6.Name[0];
				Player2Species = playerProfileData6.PlayerSpecies[0];
			}
			P2ShoulderLeftWaspressed = P2ShoulderLeftpressed;
			if (state2.Buttons.LeftShoulder == ButtonState.Released)
			{
				P2ShoulderLeftpressed = false;
			}
			if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P2DpadRightpressed = true;
				if (P2Flair < FlairMax)
				{
					P2Flair++;
				}
				P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
				P2FlairOld[P2FlairIndex] = P2Flair;
			}
			P2DpadRightWaspressed = P2DpadRightpressed;
			if (state2.DPad.Right == ButtonState.Released)
			{
				P2DpadRightpressed = false;
			}
			if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P2DpadLeftpressed = true;
				if (P2Flair > 0)
				{
					P2Flair--;
				}
				_ = "Flair/" + Player2Species.ToString() + "/" + P2FlairIndex + "/" + P2Flair;
				P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
				P2FlairOld[P2FlairIndex] = P2Flair;
			}
			P2DpadLeftWaspressed = P2DpadLeftpressed;
			if (state2.DPad.Left == ButtonState.Released)
			{
				P2DpadLeftpressed = false;
			}
			if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P2DpadUppressed = true;
				if (P2FlairIndex > 0)
				{
					P2FlairIndex--;
				}
				P2Flair = P2FlairOld[P2FlairIndex];
				P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
			}
			P2DpadUpWaspressed = P2DpadUppressed;
			if (state2.DPad.Up == ButtonState.Released)
			{
				P2DpadUppressed = false;
			}
			if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P2DpadDownpressed = true;
				if (P2FlairIndex < FlairIndexMax)
				{
					P2FlairIndex++;
				}
				P2Flair = P2FlairOld[P2FlairIndex];
				P2FlairOld_Tag[P2FlairIndex] = P2FlairIndex;
			}
			P2DpadDownWaspressed = P2DpadDownpressed;
			if (state2.DPad.Down == ButtonState.Released)
			{
				P2DpadDownpressed = false;
			}
			string assetName2 = "Sprites/" + Player2Species + "/head";
			Player2MenuTexture = base.Content.Load<Texture2D>(assetName2);
			if (P2FlairIndex == 0)
			{
				P2Text = "Eyes";
			}
			if (P2FlairIndex == 1)
			{
				P2Text = "Beard";
			}
			if (P2FlairIndex == 2)
			{
				P2Text = "Mouth";
			}
			if (P2FlairIndex == 3)
			{
				P2Text = "Hair";
			}
			if (P2FlairIndex == 4)
			{
				P2Text = "Mask";
			}
			if (P2FlairIndex == 5)
			{
				P2Text = "Hat";
			}
			if (P2FlairIndex == 6)
			{
				P2Text = "BodyFront";
			}
			if (P2FlairIndex == 7)
			{
				P2Text = "BodyBack";
			}
			if (P2FlairIndex == 8)
			{
				P2Text = "LegsFront";
			}
			if (P2FlairIndex == 9)
			{
				P2Text = "LegsBack";
			}
		}
		else if (P2MainMenuProgression == 2)
		{
			Player2InGame = true;
			Player2Ready = true;
			if (P1MainMenuProgression < 2 && P3MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P2InControlOfMainMenu = true;
				P1InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P2InControlOfMainMenu)
			{
				if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P2DpadUppressed = true;
					MainMenuIndexer--;
				}
				P2DpadUpWaspressed = P2DpadUppressed;
				if (state2.DPad.Up == ButtonState.Released)
				{
					P2DpadUppressed = false;
				}
				if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P2DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P2DpadDownWaspressed = P2DpadDownpressed;
				if (state2.DPad.Down == ButtonState.Released)
				{
					P2DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P2MainMenuProgression == 3)
		{
			Player2InGame = true;
			Player2Ready = true;
			if (P2InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P2DpadRightWaspressed = P2DpadRightpressed;
					if (state2.DPad.Right == ButtonState.Released)
					{
						P2DpadRightpressed = false;
					}
					if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P2DpadLeftWaspressed = P2DpadLeftpressed;
					if (state2.DPad.Left == ButtonState.Released)
					{
						P2DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P2DpadUpWaspressed && state2.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P2DpadUpWaspressed = P2DpadUppressed;
					if (state2.DPad.Up == ButtonState.Released)
					{
						P2DpadUppressed = false;
					}
					if (!P2DpadDownWaspressed && state2.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P2DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P2DpadDownWaspressed = P2DpadDownpressed;
					if (state2.DPad.Down == ButtonState.Released)
					{
						P2DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P2DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P2DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P2DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P2DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P2DpadRightWaspressed && state2.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P2DpadRightpressed = true;
							BloodToggle = true;
						}
						P2DpadRightWaspressed = P2DpadRightpressed;
						if (state2.DPad.Right == ButtonState.Released)
						{
							P2DpadRightpressed = false;
						}
						if (!P2DpadLeftWaspressed && state2.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P2DpadLeftpressed = true;
							BloodToggle = false;
						}
						P2DpadLeftWaspressed = P2DpadLeftpressed;
						if (state2.DPad.Left == ButtonState.Released)
						{
							P2DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P2MainMenuProgression == 4 && P2InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 1)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 2)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P3AWasPressed && state3.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P3AWasPressed = true;
			P3MainMenuProgression++;
			if (P3MainMenuProgression > MainMenuProgressionMax)
			{
				P3MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P3MainMenuProgression = (int)MathHelper.Clamp(P3MainMenuProgression, 0f, 3f);
			}
		}
		if (state3.Buttons.A == ButtonState.Released)
		{
			P3AWasPressed = false;
		}
		if (!P3BWasPressed && state3.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P3BWasPressed = true;
			P3MainMenuProgression--;
			if (P3MainMenuProgression < 0)
			{
				P3MainMenuProgression = 0;
			}
		}
		if (state3.Buttons.B == ButtonState.Released)
		{
			P3BWasPressed = false;
		}
		if (P3MainMenuProgression == 0)
		{
			Player3InGame = false;
			Player3Ready = true;
		}
		else if (P3MainMenuProgression == 1)
		{
			Player3InGame = true;
			Player3Ready = false;
			P3InControlOfMainMenu = false;
			if (Player3ProfileName == " ")
			{
				PlayerProfileData playerProfileData7 = LoadProfile($"{P3ProfileIndex}");
				for (int num = 0; num < FlairIndexMax + 1; num++)
				{
					Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
					P3FlairOld_Tag[num] = num;
					P3FlairOld[num] = (int)playerProfileData7.Flair[num];
				}
				Player3ProfileName = playerProfileData7.Name[0];
				Player3Species = playerProfileData7.PlayerSpecies[0];
			}
			if (!P3ShoulderRightWaspressed && state3.Buttons.RightShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P3ShoulderRightpressed = true;
				P3ProfileIndex++;
				if (P3ProfileIndex > ProfileMax)
				{
					P3ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData8 = LoadProfile($"{P3ProfileIndex}");
				for (int num2 = 0; num2 < FlairIndexMax + 1; num2++)
				{
					Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
					P3FlairOld_Tag[num2] = num2;
					P3FlairOld[num2] = (int)playerProfileData8.Flair[num2];
				}
				Player3ProfileName = playerProfileData8.Name[0];
				Player3Species = playerProfileData8.PlayerSpecies[0];
			}
			P3ShoulderRightWaspressed = P3ShoulderRightpressed;
			if (state3.Buttons.RightShoulder == ButtonState.Released)
			{
				P3ShoulderRightpressed = false;
			}
			if (!P3ShoulderLeftWaspressed && state3.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P3ShoulderLeftpressed = true;
				P3ProfileIndex--;
				if (P3ProfileIndex < 0)
				{
					P3ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData9 = LoadProfile($"{P3ProfileIndex}");
				for (int num3 = 0; num3 < FlairIndexMax + 1; num3++)
				{
					Player3SpriteSheet = PlayerSpriteSheet[P3ProfileIndex];
					P3FlairOld_Tag[num3] = num3;
					P3FlairOld[num3] = (int)playerProfileData9.Flair[num3];
				}
				Player3ProfileName = playerProfileData9.Name[0];
				Player3Species = playerProfileData9.PlayerSpecies[0];
			}
			P3ShoulderLeftWaspressed = P3ShoulderLeftpressed;
			if (state3.Buttons.LeftShoulder == ButtonState.Released)
			{
				P3ShoulderLeftpressed = false;
			}
			if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P3DpadRightpressed = true;
				if (P3Flair < FlairMax)
				{
					P3Flair++;
				}
				P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
				P3FlairOld[P3FlairIndex] = P3Flair;
			}
			P3DpadRightWaspressed = P3DpadRightpressed;
			if (state3.DPad.Right == ButtonState.Released)
			{
				P3DpadRightpressed = false;
			}
			if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P3DpadLeftpressed = true;
				if (P3Flair > 0)
				{
					P3Flair--;
				}
				_ = "Flair/" + Player3Species.ToString() + "/" + P3FlairIndex + "/" + P3Flair;
				P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
				P3FlairOld[P3FlairIndex] = P3Flair;
			}
			P3DpadLeftWaspressed = P3DpadLeftpressed;
			if (state3.DPad.Left == ButtonState.Released)
			{
				P3DpadLeftpressed = false;
			}
			if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P3DpadUppressed = true;
				if (P3FlairIndex > 0)
				{
					P3FlairIndex--;
				}
				P3Flair = P3FlairOld[P3FlairIndex];
				P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
			}
			P3DpadUpWaspressed = P3DpadUppressed;
			if (state3.DPad.Up == ButtonState.Released)
			{
				P3DpadUppressed = false;
			}
			if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P3DpadDownpressed = true;
				if (P3FlairIndex < FlairIndexMax)
				{
					P3FlairIndex++;
				}
				P3Flair = P3FlairOld[P3FlairIndex];
				P3FlairOld_Tag[P3FlairIndex] = P3FlairIndex;
			}
			P3DpadDownWaspressed = P3DpadDownpressed;
			if (state3.DPad.Down == ButtonState.Released)
			{
				P3DpadDownpressed = false;
			}
			string assetName3 = "Sprites/" + Player3Species + "/head";
			Player3MenuTexture = base.Content.Load<Texture2D>(assetName3);
			if (P3FlairIndex == 0)
			{
				P3Text = "Eyes";
			}
			if (P3FlairIndex == 1)
			{
				P3Text = "Beard";
			}
			if (P3FlairIndex == 2)
			{
				P3Text = "Mouth";
			}
			if (P3FlairIndex == 3)
			{
				P3Text = "Hair";
			}
			if (P3FlairIndex == 4)
			{
				P3Text = "Mask";
			}
			if (P3FlairIndex == 5)
			{
				P3Text = "Hat";
			}
			if (P3FlairIndex == 6)
			{
				P3Text = "BodyFront";
			}
			if (P3FlairIndex == 7)
			{
				P3Text = "BodyBack";
			}
			if (P3FlairIndex == 8)
			{
				P3Text = "LegsFront";
			}
			if (P3FlairIndex == 9)
			{
				P3Text = "LegsBack";
			}
		}
		else if (P3MainMenuProgression == 2)
		{
			Player3InGame = true;
			Player3Ready = true;
			if (P1MainMenuProgression < 2 && P2MainMenuProgression < 2 && P4MainMenuProgression < 2)
			{
				P3InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P1InControlOfMainMenu = false;
				P4InControlOfMainMenu = false;
			}
			if (P3InControlOfMainMenu)
			{
				if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P3DpadUppressed = true;
					MainMenuIndexer--;
				}
				P3DpadUpWaspressed = P3DpadUppressed;
				if (state3.DPad.Up == ButtonState.Released)
				{
					P3DpadUppressed = false;
				}
				if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P3DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P3DpadDownWaspressed = P3DpadDownpressed;
				if (state3.DPad.Down == ButtonState.Released)
				{
					P3DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P3MainMenuProgression == 3)
		{
			Player3InGame = true;
			Player3Ready = true;
			if (P3InControlOfMainMenu)
			{
				if (MainMenuIndexer == 0)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 1)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 2)
				{
					if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadRightpressed = true;
						MainMenuLevelIndexer++;
					}
					P3DpadRightWaspressed = P3DpadRightpressed;
					if (state3.DPad.Right == ButtonState.Released)
					{
						P3DpadRightpressed = false;
					}
					if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadLeftpressed = true;
						MainMenuLevelIndexer--;
					}
					P3DpadLeftWaspressed = P3DpadLeftpressed;
					if (state3.DPad.Left == ButtonState.Released)
					{
						P3DpadLeftpressed = false;
					}
					if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
					{
						MainMenuLevelIndexer = 0;
					}
					if (MainMenuLevelIndexer < 0)
					{
						MainMenuLevelIndexer = MainMenuLevelIndexerMax;
					}
				}
				else if (MainMenuIndexer == 3)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					StartLevelBuilder = true;
				}
				else if (MainMenuIndexer == 4)
				{
					if (!P3DpadUpWaspressed && state3.DPad.Up == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadUppressed = true;
						MainMenuIndexerOption--;
					}
					P3DpadUpWaspressed = P3DpadUppressed;
					if (state3.DPad.Up == ButtonState.Released)
					{
						P3DpadUppressed = false;
					}
					if (!P3DpadDownWaspressed && state3.DPad.Down == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P3DpadDownpressed = true;
						MainMenuIndexerOption++;
					}
					P3DpadDownWaspressed = P3DpadDownpressed;
					if (state3.DPad.Down == ButtonState.Released)
					{
						P3DpadDownpressed = false;
					}
					if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
					{
						MainMenuIndexerOption = 0;
					}
					if (MainMenuIndexerOption < 0)
					{
						MainMenuIndexerOption = MainMenuIndexerOptionMax;
					}
					if (MainMenuIndexerOption == 0)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							Music_Volume += Volume_Step;
							P3DpadRightpressed = true;
							if (Music_Volume > 1f)
							{
								Music_Volume = 1f;
								MusicToggle = true;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							Music_Volume -= Volume_Step;
							P3DpadLeftpressed = true;
							if (Music_Volume < 0f)
							{
								Music_Volume = 0f;
								MusicToggle = false;
							}
							MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							Sound_Effect_Volume += Volume_Step;
							P3DpadRightpressed = true;
							if (Sound_Effect_Volume > 1f)
							{
								Sound_Effect_Volume = 1f;
								SoundEffectToggle = true;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							Sound_Effect_Volume -= Volume_Step;
							P3DpadLeftpressed = true;
							if (Sound_Effect_Volume < 0f)
							{
								Sound_Effect_Volume = 0f;
								SoundEffectToggle = false;
							}
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						if (!P3DpadRightWaspressed && state3.DPad.Right == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P3DpadRightpressed = true;
							BloodToggle = true;
						}
						P3DpadRightWaspressed = P3DpadRightpressed;
						if (state3.DPad.Right == ButtonState.Released)
						{
							P3DpadRightpressed = false;
						}
						if (!P3DpadLeftWaspressed && state3.DPad.Left == ButtonState.Pressed)
						{
							MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
							P3DpadLeftpressed = true;
							BloodToggle = false;
						}
						P3DpadLeftWaspressed = P3DpadLeftpressed;
						if (state3.DPad.Left == ButtonState.Released)
						{
							P3DpadLeftpressed = false;
						}
					}
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		else if (P3MainMenuProgression == 4 && P3InControlOfMainMenu && Player1Ready && Player2Ready && Player3Ready && Player4Ready)
		{
			if (MainMenuIndexer == 0)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 1)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 2)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
		if (!P4AWasPressed && state4.Buttons.A == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
			P4AWasPressed = true;
			P4MainMenuProgression++;
			if (P4MainMenuProgression > MainMenuProgressionMax)
			{
				P4MainMenuProgression = MainMenuProgressionMax;
				MenuClickSound.Play(Sound_Effect_Volume, 0.1f, 0f);
			}
			if (MainMenuIndexer == 4)
			{
				P4MainMenuProgression = (int)MathHelper.Clamp(P4MainMenuProgression, 0f, 3f);
			}
		}
		if (state4.Buttons.A == ButtonState.Released)
		{
			P4AWasPressed = false;
		}
		if (!P4BWasPressed && state4.Buttons.B == ButtonState.Pressed)
		{
			MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
			P4BWasPressed = true;
			P4MainMenuProgression--;
			if (P4MainMenuProgression < 0)
			{
				P4MainMenuProgression = 0;
			}
		}
		if (state4.Buttons.B == ButtonState.Released)
		{
			P4BWasPressed = false;
		}
		if (P4MainMenuProgression == 0)
		{
			Player4InGame = false;
			Player4Ready = true;
		}
		else if (P4MainMenuProgression == 1)
		{
			Player4InGame = true;
			Player4Ready = false;
			P4InControlOfMainMenu = false;
			if (Player4ProfileName == " ")
			{
				PlayerProfileData playerProfileData10 = LoadProfile($"{P4ProfileIndex}");
				for (int num4 = 0; num4 < FlairIndexMax + 1; num4++)
				{
					Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
					P4FlairOld_Tag[num4] = num4;
					P4FlairOld[num4] = (int)playerProfileData10.Flair[num4];
				}
				Player4ProfileName = playerProfileData10.Name[0];
				Player4Species = playerProfileData10.PlayerSpecies[0];
			}
			if (!P4ShoulderRightWaspressed && state4.Buttons.RightShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P4ShoulderRightpressed = true;
				P4ProfileIndex++;
				if (P4ProfileIndex > ProfileMax)
				{
					P4ProfileIndex = 0;
				}
				PlayerProfileData playerProfileData11 = LoadProfile($"{P4ProfileIndex}");
				for (int num5 = 0; num5 < FlairIndexMax + 1; num5++)
				{
					Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
					P4FlairOld_Tag[num5] = num5;
					P4FlairOld[num5] = (int)playerProfileData11.Flair[num5];
				}
				Player4ProfileName = playerProfileData11.Name[0];
				Player4Species = playerProfileData11.PlayerSpecies[0];
			}
			P4ShoulderRightWaspressed = P4ShoulderRightpressed;
			if (state4.Buttons.RightShoulder == ButtonState.Released)
			{
				P4ShoulderRightpressed = false;
			}
			if (!P4ShoulderLeftWaspressed && state4.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				MenuClickSound.Play(Sound_Effect_Volume, (float)(random.NextDouble() / 3.0) - 0.15f, 0f);
				P4ShoulderLeftpressed = true;
				P4ProfileIndex--;
				if (P4ProfileIndex < 0)
				{
					P4ProfileIndex = ProfileMax;
				}
				PlayerProfileData playerProfileData12 = LoadProfile($"{P4ProfileIndex}");
				for (int num6 = 0; num6 < FlairIndexMax + 1; num6++)
				{
					Player4SpriteSheet = PlayerSpriteSheet[P4ProfileIndex];
					P4FlairOld_Tag[num6] = num6;
					P4FlairOld[num6] = (int)playerProfileData12.Flair[num6];
				}
				Player4ProfileName = playerProfileData12.Name[0];
				Player4Species = playerProfileData12.PlayerSpecies[0];
			}
			P4ShoulderLeftWaspressed = P4ShoulderLeftpressed;
			if (state4.Buttons.LeftShoulder == ButtonState.Released)
			{
				P4ShoulderLeftpressed = false;
			}
			if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P4DpadRightpressed = true;
				if (P4Flair < FlairMax)
				{
					P4Flair++;
				}
				P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
				P4FlairOld[P4FlairIndex] = P4Flair;
			}
			P4DpadRightWaspressed = P4DpadRightpressed;
			if (state4.DPad.Right == ButtonState.Released)
			{
				P4DpadRightpressed = false;
			}
			if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P4DpadLeftpressed = true;
				if (P4Flair > 0)
				{
					P4Flair--;
				}
				_ = "Flair/" + Player4Species.ToString() + "/" + P4FlairIndex + "/" + P4Flair;
				P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
				P4FlairOld[P4FlairIndex] = P4Flair;
			}
			P4DpadLeftWaspressed = P4DpadLeftpressed;
			if (state4.DPad.Left == ButtonState.Released)
			{
				P4DpadLeftpressed = false;
			}
			if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P4DpadUppressed = true;
				if (P4FlairIndex > 0)
				{
					P4FlairIndex--;
				}
				P4Flair = P4FlairOld[P4FlairIndex];
				P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
			}
			P4DpadUpWaspressed = P4DpadUppressed;
			if (state4.DPad.Up == ButtonState.Released)
			{
				P4DpadUppressed = false;
			}
			if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
			{
				MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				P4DpadDownpressed = true;
				if (P4FlairIndex < FlairIndexMax)
				{
					P4FlairIndex++;
				}
				P4Flair = P4FlairOld[P4FlairIndex];
				P4FlairOld_Tag[P4FlairIndex] = P4FlairIndex;
			}
			P4DpadDownWaspressed = P4DpadDownpressed;
			if (state4.DPad.Down == ButtonState.Released)
			{
				P4DpadDownpressed = false;
			}
			string assetName4 = "Sprites/" + Player4Species + "/head";
			Player4MenuTexture = base.Content.Load<Texture2D>(assetName4);
			if (P4FlairIndex == 0)
			{
				P4Text = "Eyes";
			}
			if (P4FlairIndex == 1)
			{
				P4Text = "Beard";
			}
			if (P4FlairIndex == 2)
			{
				P4Text = "Mouth";
			}
			if (P4FlairIndex == 3)
			{
				P4Text = "Hair";
			}
			if (P4FlairIndex == 4)
			{
				P4Text = "Mask";
			}
			if (P4FlairIndex == 5)
			{
				P4Text = "Hat";
			}
			if (P4FlairIndex == 6)
			{
				P4Text = "BodyFront";
			}
			if (P4FlairIndex == 7)
			{
				P4Text = "BodyBack";
			}
			if (P4FlairIndex == 8)
			{
				P4Text = "LegsFront";
			}
			if (P4FlairIndex == 9)
			{
				P4Text = "LegsBack";
			}
		}
		else if (P4MainMenuProgression == 2)
		{
			Player4InGame = true;
			Player4Ready = true;
			if (P1MainMenuProgression < 2 && P2MainMenuProgression < 2 && P3MainMenuProgression < 2)
			{
				P4InControlOfMainMenu = true;
				P2InControlOfMainMenu = false;
				P3InControlOfMainMenu = false;
				P1InControlOfMainMenu = false;
			}
			if (P4InControlOfMainMenu)
			{
				if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadUppressed = true;
					MainMenuIndexer--;
				}
				P4DpadUpWaspressed = P4DpadUppressed;
				if (state4.DPad.Up == ButtonState.Released)
				{
					P4DpadUppressed = false;
				}
				if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadDownpressed = true;
					MainMenuIndexer++;
				}
				P4DpadDownWaspressed = P4DpadDownpressed;
				if (state4.DPad.Down == ButtonState.Released)
				{
					P4DpadDownpressed = false;
				}
				if (MainMenuIndexer > MaxMainMenuIndexer)
				{
					MainMenuIndexer = 0;
				}
				if (MainMenuIndexer < 0)
				{
					MainMenuIndexer = MaxMainMenuIndexer;
				}
			}
		}
		else if (P4MainMenuProgression == 3)
		{
			Player4InGame = true;
			Player4Ready = true;
			if (!P4InControlOfMainMenu)
			{
				return;
			}
			if (MainMenuIndexer == 0)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 1)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 2)
			{
				if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadRightpressed = true;
					MainMenuLevelIndexer++;
				}
				P4DpadRightWaspressed = P4DpadRightpressed;
				if (state4.DPad.Right == ButtonState.Released)
				{
					P4DpadRightpressed = false;
				}
				if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadLeftpressed = true;
					MainMenuLevelIndexer--;
				}
				P4DpadLeftWaspressed = P4DpadLeftpressed;
				if (state4.DPad.Left == ButtonState.Released)
				{
					P4DpadLeftpressed = false;
				}
				if (MainMenuLevelIndexer > MainMenuLevelIndexerMax)
				{
					MainMenuLevelIndexer = 0;
				}
				if (MainMenuLevelIndexer < 0)
				{
					MainMenuLevelIndexer = MainMenuLevelIndexerMax;
				}
			}
			else if (MainMenuIndexer == 3)
			{
				MainMenuFadeIn = false;
				MainMenuFadeOut = true;
				StartLevelBuilder = true;
			}
			else if (MainMenuIndexer == 4)
			{
				if (!P4DpadUpWaspressed && state4.DPad.Up == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadUppressed = true;
					MainMenuIndexerOption--;
				}
				P4DpadUpWaspressed = P4DpadUppressed;
				if (state4.DPad.Up == ButtonState.Released)
				{
					P4DpadUppressed = false;
				}
				if (!P4DpadDownWaspressed && state4.DPad.Down == ButtonState.Pressed)
				{
					MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					P4DpadDownpressed = true;
					MainMenuIndexerOption++;
				}
				P4DpadDownWaspressed = P4DpadDownpressed;
				if (state4.DPad.Down == ButtonState.Released)
				{
					P4DpadDownpressed = false;
				}
				if (MainMenuIndexerOption > MainMenuIndexerOptionMax)
				{
					MainMenuIndexerOption = 0;
				}
				if (MainMenuIndexerOption < 0)
				{
					MainMenuIndexerOption = MainMenuIndexerOptionMax;
				}
				if (MainMenuIndexerOption == 0)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						Music_Volume += Volume_Step;
						P4DpadRightpressed = true;
						if (Music_Volume > 1f)
						{
							Music_Volume = 1f;
							MusicToggle = true;
						}
						MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						Music_Volume -= Volume_Step;
						P4DpadLeftpressed = true;
						if (Music_Volume < 0f)
						{
							Music_Volume = 0f;
							MusicToggle = false;
						}
						MenuMoveSound.Play(Music_Volume, 0.5f, 0f);
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
				else if (MainMenuIndexerOption == 1)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						Sound_Effect_Volume += Volume_Step;
						P4DpadRightpressed = true;
						if (Sound_Effect_Volume > 1f)
						{
							Sound_Effect_Volume = 1f;
							SoundEffectToggle = true;
						}
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						Sound_Effect_Volume -= Volume_Step;
						P4DpadLeftpressed = true;
						if (Sound_Effect_Volume < 0f)
						{
							Sound_Effect_Volume = 0f;
							SoundEffectToggle = false;
						}
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
				else if (MainMenuIndexerOption == 2)
				{
					if (!P4DpadRightWaspressed && state4.DPad.Right == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P4DpadRightpressed = true;
						BloodToggle = true;
					}
					P4DpadRightWaspressed = P4DpadRightpressed;
					if (state4.DPad.Right == ButtonState.Released)
					{
						P4DpadRightpressed = false;
					}
					if (!P4DpadLeftWaspressed && state4.DPad.Left == ButtonState.Pressed)
					{
						MenuMoveSound.Play(Sound_Effect_Volume, 0.5f, 0f);
						P4DpadLeftpressed = true;
						BloodToggle = false;
					}
					P4DpadLeftWaspressed = P4DpadLeftpressed;
					if (state4.DPad.Left == ButtonState.Released)
					{
						P4DpadLeftpressed = false;
					}
				}
			}
			else if (MainMenuIndexer == 5)
			{
				MainMenuFadeIn = false;
				MainMenuFadeOut = true;
				ExitGame(gameTime);
			}
		}
		else
		{
			if (P4MainMenuProgression != 4 || !P4InControlOfMainMenu || !Player1Ready || !Player2Ready || !Player3Ready || !Player4Ready)
			{
				return;
			}
			if (MainMenuIndexer == 0)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 1)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer == 2)
			{
				StartGame = true;
			}
			else if (MainMenuIndexer != 3)
			{
				if (MainMenuIndexer == 4)
				{
					InOptionsMode = true;
				}
				else if (MainMenuIndexer == 5)
				{
					MainMenuFadeIn = false;
					MainMenuFadeOut = true;
					ExitGame(gameTime);
				}
			}
		}
	}

	private void HandleTransInput()
	{
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		GamePadState state2 = GamePad.GetState(PlayerIndex.Two);
		GamePadState state3 = GamePad.GetState(PlayerIndex.Three);
		GamePadState state4 = GamePad.GetState(PlayerIndex.Four);
		KeyboardState state5 = Keyboard.GetState();
		if (state.Buttons.Start == ButtonState.Pressed && !TransSkipP1WasPressed)
		{
			TransSkipP1WasPressed = true;
		}
		if (state.Buttons.Start == ButtonState.Released)
		{
			TransSkipP1WasPressed = false;
		}
		if (state5.IsKeyDown(Keys.Space) && !TransSkipP1WasPressed)
		{
			TransSkipP1WasPressed = true;
		}
		if (state5.IsKeyUp(Keys.Space))
		{
			TransSkipP1WasPressed = false;
		}
		if (state2.Buttons.Start == ButtonState.Pressed && !TransSkipP2WasPressed)
		{
			TransSkipP2WasPressed = true;
		}
		if (state2.Buttons.Start == ButtonState.Released)
		{
			TransSkipP2WasPressed = false;
		}
		if (state3.Buttons.Start == ButtonState.Pressed && !TransSkipP3WasPressed)
		{
			TransSkipP3WasPressed = true;
		}
		if (state3.Buttons.Start == ButtonState.Released)
		{
			TransSkipP3WasPressed = false;
		}
		if (state4.Buttons.Start == ButtonState.Pressed && !TransSkipP4WasPressed)
		{
			TransSkipP4WasPressed = true;
		}
		if (state4.Buttons.Start == ButtonState.Released)
		{
			TransSkipP4WasPressed = false;
		}
	}

	private void HandleInput()
	{
		KeyboardState state = Keyboard.GetState();
		GamePad.GetState(PlayerIndex.One);
		GamePad.GetState(PlayerIndex.Two);
		GamePad.GetState(PlayerIndex.Three);
		GamePad.GetState(PlayerIndex.Four);
		bool flag = state.IsKeyDown(Keys.Pause);
		if (!wasContinuePressed && flag)
		{
			if (InPauseMode)
			{
				InPauseMode = false;
			}
			else
			{
				InPauseMode = true;
			}
			if (level != null)
			{
				level.Pause(InPauseMode, 1);
			}
		}
		wasContinuePressed = flag;
		bool flag2 = state.IsKeyDown(Keys.Add);
		if (!wasNextLevelPressed && flag2)
		{
			LoadNextLevelFromBuilder();
		}
		wasNextLevelPressed = flag2;
		bool flag3 = state.IsKeyDown(Keys.End);
		if (!wasBloodPressed && flag3)
		{
			if (InBloodMode)
			{
				InBloodMode = false;
			}
			else
			{
				InBloodMode = true;
			}
			if (level != null)
			{
				level.BloodMode(InBloodMode);
			}
		}
		wasBloodPressed = flag3;
	}

	private void HandleInput1()
	{
		Keyboard.GetState();
		bool flag = GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start);
		if (!wasContinue1Pressed && flag)
		{
			if (InPauseMode)
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				InPauseMode = false;
			}
			else
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0f, 0f);
				InPauseMode = true;
			}
			if (level != null)
			{
				level.Pause(InPauseMode, 1);
			}
		}
		wasContinue1Pressed = flag;
	}

	private void HandleInput2()
	{
		bool flag = GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start);
		if (!wasContinue2Pressed && flag)
		{
			if (InPauseMode)
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				InPauseMode = false;
			}
			else
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0f, 0f);
				InPauseMode = true;
			}
			if (level != null)
			{
				level.Pause(InPauseMode, 2);
			}
		}
		wasContinue2Pressed = flag;
	}

	private void HandleInput3()
	{
		bool flag = GamePad.GetState(PlayerIndex.Three).IsButtonDown(Buttons.Start);
		if (!wasContinue3Pressed && flag)
		{
			if (InPauseMode)
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				InPauseMode = false;
			}
			else
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0f, 0f);
				InPauseMode = true;
			}
			if (level != null)
			{
				level.Pause(InPauseMode, 3);
			}
		}
		wasContinue3Pressed = flag;
	}

	private void HandleInput4()
	{
		bool flag = GamePad.GetState(PlayerIndex.Four).IsButtonDown(Buttons.Start);
		if (!wasContinue4Pressed && flag)
		{
			if (InPauseMode)
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0.5f, 0f);
				InPauseMode = false;
			}
			else
			{
				MenuClickSound.Play(Sound_Effect_Volume, 0f, 0f);
				InPauseMode = true;
			}
			if (level != null)
			{
				level.Pause(InPauseMode, 4);
			}
		}
		wasContinue4Pressed = flag;
	}

	protected override void Draw(GameTime gameTime)
	{
		graphics.GraphicsDevice.Clear(Color.Black);
		if (Guide.IsVisible)
		{
			return;
		}
		if (!InFirstTrans1)
		{
			if (!InFirstTrans2)
			{
				if (InLevelMode)
				{
					DrawLevel(gameTime);
				}
				if (InLevelBuilderMode && levelBuilder != null)
				{
					levelBuilder.Draw(this, gameTime, spriteBatch);
				}
				if (InMainMenuMode)
				{
					if (MenuMusicFirst)
					{
						MenuMusicFirst = false;
						MediaPlayer.IsRepeating = true;
						MediaPlayer.Volume = Music_Volume;
						MediaPlayer.Play(SongMainMenu);
					}
					DrawMainMenu(gameTime);
				}
				if (!Loaded)
				{
					DrawLoadingScreen(gameTime);
				}
			}
			else
			{
				DrawFarNMerc(gameTime);
			}
		}
		else
		{
			if (RnRFirst)
			{
				RnRSound.Play(1f, 0f, 0f);
				RnRFirst = false;
			}
			DrawRnRStudios(gameTime);
		}
		base.Draw(gameTime);
	}

	private void DrawLevel(GameTime gameTime)
	{
		if (SplitScreen)
		{
			DrawLevelSplitScreen(gameTime);
		}
		else if (level != null)
		{
			level.Draw(this, gameTime, spriteBatch);
		}
	}

	private void DrawLevelSplitScreen(GameTime gameTime)
	{
		if (level != null)
		{
			level.Draw(this, gameTime, spriteBatch);
		}
		if (PlayersInGameindex == 1)
		{
			base.GraphicsDevice.Viewport = Viewports[0];
			level.DrawMasterScene(this, gameTime, spriteBatch);
			SplitScreenindex = 1;
		}
		else if (PlayersInGameindex == 2 && SplitScreenHoriz)
		{
			for (int i = 1; i < 3; i++)
			{
				base.GraphicsDevice.Viewport = Viewports[i];
				if (level != null)
				{
					level.DrawMasterScene(this, gameTime, spriteBatch);
				}
				SplitScreenindex = i;
			}
		}
		else if (PlayersInGameindex == 2 && !SplitScreenHoriz)
		{
			for (int j = 3; j < 5; j++)
			{
				base.GraphicsDevice.Viewport = Viewports[j];
				if (level != null)
				{
					level.DrawMasterScene(this, gameTime, spriteBatch);
				}
				SplitScreenindex = j;
			}
		}
		else if (PlayersInGameindex == 3)
		{
			for (int k = 5; k < 9; k++)
			{
				base.GraphicsDevice.Viewport = Viewports[k];
				if (level != null)
				{
					level.DrawMasterScene(this, gameTime, spriteBatch);
				}
				SplitScreenindex = k;
			}
		}
		else if (PlayersInGameindex == 4)
		{
			for (int l = 5; l < 9; l++)
			{
				base.GraphicsDevice.Viewport = Viewports[l];
				if (level != null)
				{
					level.DrawMasterScene(this, gameTime, spriteBatch);
				}
				SplitScreenindex = l;
			}
		}
		base.GraphicsDevice.Viewport = Viewports[0];
	}

	private void DrawMainMenu(GameTime gameTime)
	{
		float num = 0f;
		graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		disappearEffect.CurrentTechnique.Passes[0].Apply();
		graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
		if (MainMenuFadeIn)
		{
			num = ((float)gameTime.TotalGameTime.TotalSeconds - MainManuFadeTimeOld) / Trans_MM_Delay * 255f;
			if (num > 255f)
			{
				num = 255f;
			}
			else if (num < 0f)
			{
				num = 0f;
			}
		}
		else if (MainMenuFadeOut)
		{
			num = ((float)gameTime.TotalGameTime.TotalSeconds - MainManuFadeTimeOld) / Trans_MM_Delay * 255f;
			num = 255f - num;
			if (num > 255f)
			{
				num = 255f;
			}
			else if (num < 0f)
			{
				num = 0f;
			}
		}
		Color color = new Color(255, 255, 255, (byte)num);
		Color color2 = new Color(255, 0, 0, (byte)num);
		float num2 = Pulsate(gameTime, 5f, 100f, 255f);
		if (num2 > 255f)
		{
			num2 = 255f;
		}
		else if (num2 < 0f)
		{
			num2 = 0f;
		}
		Color color3 = new Color((byte)num2, 0, 0, (byte)num2);
		Vector2 true_Screen_Center = True_Screen_Center;
		spriteBatch.Draw(MainMenuTexture, true_Screen_Center, null, color, 0f, new Vector2(MainMenuTexture.Width / 2, MainMenuTexture.Height / 2), 1.5f * Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.End();
		disappearEffect.CurrentTechnique.Passes[0].Apply();
		graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		Player1Position = new Vector2(-750f * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center;
		Player2Position = new Vector2(770f * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center;
		Player3Position = new Vector2(-400f * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center;
		Player4Position = new Vector2(400f * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center;
		int num3 = 1;
		if (!Update_Levels_On_Xbox_DONE)
		{
			DrawShadowedString(hudFont, "Creating files on your Storage Device.", new Vector2((0f - hudFont.MeasureString("Creating files on your Storage Device.").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
			DrawShadowedString(hudFont, "Please Wait...", new Vector2((0f - hudFont.MeasureString("Please Wait...").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
		}
		else if (!Player1InGame && !Player2InGame && !Player3InGame && !Player4InGame)
		{
			DrawShadowedString(hudFont, "Press A to come in.", new Vector2((0f - hudFont.MeasureString("Press A to come in.").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
		}
		if (P1InControlOfMainMenu)
		{
			if (P1MainMenuProgression == 2)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center, color2);
				}
			}
			else if (P1MainMenuProgression == 3)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					Color red;
					if (MusicToggle)
					{
						red = Color.Red;
						red = new Color(Music_Volume, (int)red.G, (int)red.B, Music_Volume);
					}
					else
					{
						red = Color.Black;
					}
					Color red2;
					if (SoundEffectToggle)
					{
						red2 = Color.Red;
						red2 = new Color(Sound_Effect_Volume, (int)red2.G, (int)red2.B, Sound_Effect_Volume);
					}
					else
					{
						red2 = Color.Black;
					}
					Color color4 = ((!BloodToggle) ? Color.Black : Color.Red);
					if (MusicToggle)
					{
						if (MainMenuIndexerOption == 0)
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red);
						}
						else
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, red);
						}
					}
					else if (MainMenuIndexerOption == 0)
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (SoundEffectToggle)
					{
						if (MainMenuIndexerOption == 1)
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red2);
						}
						else
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, red2);
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (BloodToggle)
					{
						if (MainMenuIndexerOption == 2)
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), color4);
						}
						else
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, color4);
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Ok. Bye", new Vector2((0f - hudFont.MeasureString("Ok. Bye").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
			}
			else if (P1MainMenuProgression == 4)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer != 4)
				{
					_ = MainMenuIndexer;
					_ = 5;
				}
			}
		}
		if (P2InControlOfMainMenu)
		{
			if (P2MainMenuProgression == 2)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center, color2);
				}
			}
			else if (P2MainMenuProgression == 3)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					Color red3;
					if (MusicToggle)
					{
						red3 = Color.Red;
						red3 = new Color(Music_Volume, (int)red3.G, (int)red3.B, Music_Volume);
					}
					else
					{
						red3 = Color.Black;
					}
					Color red4;
					if (SoundEffectToggle)
					{
						red4 = Color.Red;
						red4 = new Color(Sound_Effect_Volume, (int)red4.G, (int)red4.B, Sound_Effect_Volume);
					}
					else
					{
						red4 = Color.Black;
					}
					Color color5 = ((!BloodToggle) ? Color.Black : Color.Red);
					if (MusicToggle)
					{
						if (MainMenuIndexerOption == 0)
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red3);
						}
						else
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, red3);
						}
					}
					else if (MainMenuIndexerOption == 0)
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (SoundEffectToggle)
					{
						if (MainMenuIndexerOption == 1)
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red4);
						}
						else
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, red4);
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (BloodToggle)
					{
						if (MainMenuIndexerOption == 2)
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), color5);
						}
						else
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, color5);
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Ok. Bye", new Vector2((0f - hudFont.MeasureString("Ok. Bye").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
			}
			else if (P2MainMenuProgression == 4)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer != 4)
				{
					_ = MainMenuIndexer;
					_ = 5;
				}
			}
		}
		if (P3InControlOfMainMenu)
		{
			if (P3MainMenuProgression == 2)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center, color2);
				}
			}
			else if (P3MainMenuProgression == 3)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					Color red5;
					if (MusicToggle)
					{
						red5 = Color.Red;
						red5 = new Color(Music_Volume, (int)red5.G, (int)red5.B, Music_Volume);
					}
					else
					{
						red5 = Color.Black;
					}
					Color red6;
					if (SoundEffectToggle)
					{
						red6 = Color.Red;
						red6 = new Color(Sound_Effect_Volume, (int)red6.G, (int)red6.B, Sound_Effect_Volume);
					}
					else
					{
						red6 = Color.Black;
					}
					Color color6 = ((!BloodToggle) ? Color.Black : Color.Red);
					if (MusicToggle)
					{
						if (MainMenuIndexerOption == 0)
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red5);
						}
						else
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, red5);
						}
					}
					else if (MainMenuIndexerOption == 0)
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (SoundEffectToggle)
					{
						if (MainMenuIndexerOption == 1)
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red6);
						}
						else
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, red6);
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (BloodToggle)
					{
						if (MainMenuIndexerOption == 2)
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), color6);
						}
						else
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, color6);
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Ok. Bye", new Vector2((0f - hudFont.MeasureString("Ok. Bye").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
			}
			else if (P3MainMenuProgression == 4)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer != 4)
				{
					_ = MainMenuIndexer;
					_ = 5;
				}
			}
		}
		if (P4InControlOfMainMenu)
		{
			if (P4MainMenuProgression == 2)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, 0f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, 100f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, 200f * Global_Scaler) + true_Screen_Center, color2);
				}
				if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
				}
				else
				{
					DrawShadowedString(hudFont, "Exit", new Vector2((0f - hudFont.MeasureString("Exit").X / 2f) * Global_Scaler, 300f * Global_Scaler) + true_Screen_Center, color2);
				}
			}
			else if (P4MainMenuProgression == 3)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, "Gauntlet Run", new Vector2((0f - hudFont.MeasureString("Gauntlet Run").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Gauntlet_Run[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, "Dueling Arena", new Vector2((0f - hudFont.MeasureString("Dueling Arena").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Dueling[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, "Custom Levels", new Vector2((0f - hudFont.MeasureString("Custom Levels").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					if (AllLevelNames.Unlocked_Custom[MainMenuLevelIndexer])
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
					}
					else
					{
						DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -100f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 4)
				{
					DrawShadowedString(hudFont, "Options", new Vector2((0f - hudFont.MeasureString("Options").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color2);
					Color red7;
					if (MusicToggle)
					{
						red7 = Color.Red;
						red7 = new Color(Music_Volume, (int)red7.G, (int)red7.B, Music_Volume);
					}
					else
					{
						red7 = Color.Black;
					}
					Color red8;
					if (SoundEffectToggle)
					{
						red8 = Color.Red;
						red8 = new Color(Sound_Effect_Volume, (int)red8.G, (int)red8.B, Sound_Effect_Volume);
					}
					else
					{
						red8 = Color.Black;
					}
					Color color7 = ((!BloodToggle) ? Color.Black : Color.Red);
					if (MusicToggle)
					{
						if (MainMenuIndexerOption == 0)
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red7);
						}
						else
						{
							DrawShadowedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, red7);
						}
					}
					else if (MainMenuIndexerOption == 0)
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Music", new Vector2((0f - hudFont.MeasureString("Music").X / 2f) * Global_Scaler, -50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (SoundEffectToggle)
					{
						if (MainMenuIndexerOption == 1)
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), red8);
						}
						else
						{
							DrawShadowedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, red8);
						}
					}
					else if (MainMenuIndexerOption == 1)
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Sound Effects", new Vector2((0f - hudFont.MeasureString("Sound Effects").X / 2f) * Global_Scaler, 50f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
					if (BloodToggle)
					{
						if (MainMenuIndexerOption == 2)
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), color7);
						}
						else
						{
							DrawShadowedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, color7);
						}
					}
					else if (MainMenuIndexerOption == 2)
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3 + 1), random.Next(num3 + 1)), Color.DarkRed);
					}
					else
					{
						DrawGlowInvertedString(hudFont, "Blood", new Vector2((0f - hudFont.MeasureString("Blood").X / 2f) * Global_Scaler, 150f * Global_Scaler) + true_Screen_Center, Color.DarkRed);
					}
				}
				else if (MainMenuIndexer == 5)
				{
					DrawShadowedString(hudFont, "Ok. Bye", new Vector2((0f - hudFont.MeasureString("Ok. Bye").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
			}
			else if (P4MainMenuProgression == 4)
			{
				if (MainMenuIndexer == 0)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 1)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 2)
				{
					DrawShadowedString(hudFont, AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString(), new Vector2((0f - hudFont.MeasureString(AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString()).X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer == 3)
				{
					DrawShadowedString(hudFont, "Level Builder", new Vector2((0f - hudFont.MeasureString("Level Builder").X / 2f) * Global_Scaler, -200f * Global_Scaler) + true_Screen_Center, color3);
				}
				else if (MainMenuIndexer != 4)
				{
					_ = MainMenuIndexer;
					_ = 5;
				}
			}
		}
		spriteBatch.End();
		if (Player1InGame)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			if (!Player1Ready)
			{
				spriteBatch.Draw(BackDropTexture, Player1Position + new Vector2(0f, 50f * Global_Scaler), null, color, 0f, new Vector2(BackDropTexture.Width / 2, BackDropTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(Player1SpriteSheet, Player1Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P1FlairOld[7], 1050, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player1SpriteSheet, Player1Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P1FlairOld[6], 900, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player1MenuTexture, Player1Position, null, color, 0f, new Vector2(Player1MenuTexture.Width / 2, Player1MenuTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			spriteBatch.Draw(Player1SpriteSheet, Player1Position, new Rectangle(150 * P1FlairOld[0], 0, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player1SpriteSheet, Player1Position, new Rectangle(150 * P1FlairOld[1], 150, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player1SpriteSheet, Player1Position, new Rectangle(150 * P1FlairOld[2], 300, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player1SpriteSheet, Player1Position, new Rectangle(150 * P1FlairOld[3], 450, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player1SpriteSheet, Player1Position, new Rectangle(150 * P1FlairOld[4], 600, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player1SpriteSheet, Player1Position, new Rectangle(150 * P1FlairOld[5], 750, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.End();
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			if (!Player1Ready)
			{
				DrawShadowedString(hudFont, P1Text, Player1Position + new Vector2((0f - hudFont.MeasureString(P1Text).X / 2f) * Global_Scaler + (float)random.Next(num3), 160f * Global_Scaler + (float)random.Next(num3)), color3);
				DrawShadowedString(hudFont, Player1ProfileName, Player1Position + new Vector2((0f - hudFont.MeasureString(Player1ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color3);
			}
			else
			{
				DrawShadowedString(hudFont, Player1ProfileName, Player1Position + new Vector2((0f - hudFont.MeasureString(Player1ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color2);
			}
			spriteBatch.End();
		}
		if (Player2InGame)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			if (!Player2Ready)
			{
				spriteBatch.Draw(BackDropTexture, Player2Position + new Vector2(0f, 50f * Global_Scaler), null, color, 0f, new Vector2(BackDropTexture.Width / 2, BackDropTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(Player2SpriteSheet, Player2Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P2FlairOld[7], 1050, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player2SpriteSheet, Player2Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P2FlairOld[6], 900, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player2MenuTexture, Player2Position, null, color, 0f, new Vector2(Player2MenuTexture.Width / 2, Player2MenuTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			spriteBatch.Draw(Player2SpriteSheet, Player2Position, new Rectangle(150 * P2FlairOld[0], 0, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player2SpriteSheet, Player2Position, new Rectangle(150 * P2FlairOld[1], 150, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player2SpriteSheet, Player2Position, new Rectangle(150 * P2FlairOld[2], 300, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player2SpriteSheet, Player2Position, new Rectangle(150 * P2FlairOld[3], 450, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player2SpriteSheet, Player2Position, new Rectangle(150 * P2FlairOld[4], 600, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player2SpriteSheet, Player2Position, new Rectangle(150 * P2FlairOld[5], 750, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.End();
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			if (!Player2Ready)
			{
				DrawShadowedString(hudFont, P2Text, Player2Position + new Vector2((0f - hudFont.MeasureString(P2Text).X / 2f) * Global_Scaler + (float)random.Next(num3), 160f * Global_Scaler + (float)random.Next(num3)), color3);
				DrawShadowedString(hudFont, Player2ProfileName, Player2Position + new Vector2((0f - hudFont.MeasureString(Player2ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color3);
			}
			else
			{
				DrawShadowedString(hudFont, Player2ProfileName, Player2Position + new Vector2((0f - hudFont.MeasureString(Player2ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color2);
			}
			spriteBatch.End();
		}
		if (Player3InGame)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			if (!Player3Ready)
			{
				spriteBatch.Draw(BackDropTexture, Player3Position + new Vector2(0f, 50f * Global_Scaler), null, color, 0f, new Vector2(BackDropTexture.Width / 2, BackDropTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(Player3SpriteSheet, Player3Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P3FlairOld[7], 1050, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player3SpriteSheet, Player3Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P3FlairOld[6], 900, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player3MenuTexture, Player3Position, null, color, 0f, new Vector2(Player3MenuTexture.Width / 2, Player3MenuTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			spriteBatch.Draw(Player3SpriteSheet, Player3Position, new Rectangle(150 * P3FlairOld[0], 0, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player3SpriteSheet, Player3Position, new Rectangle(150 * P3FlairOld[1], 150, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player3SpriteSheet, Player3Position, new Rectangle(150 * P3FlairOld[2], 300, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player3SpriteSheet, Player3Position, new Rectangle(150 * P3FlairOld[3], 450, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player3SpriteSheet, Player3Position, new Rectangle(150 * P3FlairOld[4], 600, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player3SpriteSheet, Player3Position, new Rectangle(150 * P3FlairOld[5], 750, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.End();
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			if (!Player3Ready)
			{
				DrawShadowedString(hudFont, P3Text, Player3Position + new Vector2((0f - hudFont.MeasureString(P3Text).X / 2f) * Global_Scaler + (float)random.Next(num3), 160f * Global_Scaler + (float)random.Next(num3)), color3);
				DrawShadowedString(hudFont, Player3ProfileName, Player3Position + new Vector2((0f - hudFont.MeasureString(Player3ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color3);
			}
			else
			{
				DrawShadowedString(hudFont, Player3ProfileName, Player3Position + new Vector2((0f - hudFont.MeasureString(Player3ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color2);
			}
			spriteBatch.End();
		}
		if (Player4InGame)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			if (!Player4Ready)
			{
				spriteBatch.Draw(BackDropTexture, Player4Position + new Vector2(0f, 50f * Global_Scaler), null, color, 0f, new Vector2(BackDropTexture.Width / 2, BackDropTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(Player4SpriteSheet, Player4Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P4FlairOld[7], 1050, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player4SpriteSheet, Player4Position + new Vector2(0f, 150f * Global_Scaler), new Rectangle(150 * P4FlairOld[6], 900, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player4MenuTexture, Player4Position, null, color, 0f, new Vector2(Player4MenuTexture.Width / 2, Player4MenuTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 0f);
			spriteBatch.Draw(Player4SpriteSheet, Player4Position, new Rectangle(150 * P4FlairOld[0], 0, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player4SpriteSheet, Player4Position, new Rectangle(150 * P4FlairOld[1], 150, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player4SpriteSheet, Player4Position, new Rectangle(150 * P4FlairOld[2], 300, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player4SpriteSheet, Player4Position, new Rectangle(150 * P4FlairOld[3], 450, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player4SpriteSheet, Player4Position, new Rectangle(150 * P4FlairOld[4], 600, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(Player4SpriteSheet, Player4Position, new Rectangle(150 * P4FlairOld[5], 750, 150, 150), color, 0f, new Vector2(75f, 75f), 4f * Global_Scaler, SpriteEffects.None, 1f);
			spriteBatch.End();
			disappearEffect.CurrentTechnique.Passes[0].Apply();
			graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			if (!Player4Ready)
			{
				DrawShadowedString(hudFont, P4Text, Player4Position + new Vector2((0f - hudFont.MeasureString(P4Text).X / 2f) * Global_Scaler + (float)random.Next(num3), 160f * Global_Scaler + (float)random.Next(num3)), color3);
				DrawShadowedString(hudFont, Player4ProfileName, Player4Position + new Vector2((0f - hudFont.MeasureString(Player4ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color3);
			}
			else
			{
				DrawShadowedString(hudFont, Player4ProfileName, Player4Position + new Vector2((0f - hudFont.MeasureString(Player4ProfileName).X / 2f) * Global_Scaler + (float)random.Next(num3), 260f * Global_Scaler + (float)random.Next(num3)), color2);
			}
			spriteBatch.End();
		}
		if (Guide.IsTrialMode)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			DrawShadowedString(hudFont, "Trial Version", new Vector2((0f - hudFont.MeasureString("Trial Version").X / 2f + -800f) * Global_Scaler, -550f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
			DrawShadowedString(hudFont, "Trial Version", new Vector2((0f - hudFont.MeasureString("Trial Version").X / 2f + 800f) * Global_Scaler, -550f * Global_Scaler) + true_Screen_Center + new Vector2(random.Next(num3), random.Next(num3)), color3);
			spriteBatch.End();
		}
	}

	private void DrawHud()
	{
		spriteBatch.Begin();
		Rectangle titleSafeArea = base.GraphicsDevice.Viewport.TitleSafeArea;
		new Vector2(titleSafeArea.X, titleSafeArea.Y);
		new Vector2((float)titleSafeArea.X + (float)titleSafeArea.Width / 2f, (float)titleSafeArea.Y + (float)titleSafeArea.Height / 2f);
		if (level != null)
		{
			_ = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
		}
		if (level != null)
		{
			if (level.TimeRemaining > WarningTime || level.ReachedExit || (int)level.TimeRemaining.TotalSeconds % 2 == 0)
			{
				_ = Color.Yellow;
			}
			else
			{
				_ = Color.Red;
			}
		}
		spriteBatch.End();
	}

	private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawShadowedString_Smaller(SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), Color.Black, 0f, new Vector2(0f, 0f), 0.5f * Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 1f), color, 0f, new Vector2(0f, 0f), 0.5f * Global_Scaler, SpriteEffects.None, 1f);
	}

	private void DrawGlowInvertedString(SpriteFont font, string value, Vector2 position, Color color)
	{
		spriteBatch.DrawString(font, value, position + new Vector2(1f, 0f), color, 0f, new Vector2(0f, 0f), Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(-1f, 0f), color, 0f, new Vector2(0f, 0f), Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, 1f), color, 0f, new Vector2(0f, 0f), Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position + new Vector2(0f, -1f), color, 0f, new Vector2(0f, 0f), Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.DrawString(font, value, position, Color.Black, 0f, new Vector2(0f, 0f), Global_Scaler, SpriteEffects.None, 1f);
	}

	private void UpdateLoadingScreen(GameTime gameTime)
	{
		float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
		particleEffectLoading.Update(deltaSeconds);
		particleEffectLoading.Trigger(new Vector2(BackBufferWidth / 2f - BackBufferWidth / 4f, BackBufferHeight + BackBufferHeight / 2f));
		GC.KeepAlive(level);
	}

	private float SwingSkull()
	{
		if (LoadingRot > 1f)
		{
			LoadingRotRate = -0.1f;
		}
		else if (LoadingRot < -1f)
		{
			LoadingRotRate = 0.1f;
		}
		return LoadingRot;
	}

	public void DrawLoadingScreen(GameTime gameTime)
	{
		Vector2 loadingPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
		LoadingPosition = loadingPosition;
		LoadingPosition += new Vector2(0f, 200f * Global_Scaler);
		cameraTransform = Matrix.CreateTranslation(BackBufferWidth / 4f, BackBufferHeight / 4f, 0f);
		Matrix matrix = Matrix.CreateScale(Global_Scaler);
		cameraTransform *= matrix;
		graphics.GraphicsDevice.Clear(Color.Black);
		renderer.RenderEffect(particleEffectLoading);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		float num = 120f * Global_Scaler;
		Vector2 vector = new Vector2(0f, 0f - (float)graphics.GraphicsDevice.Viewport.Height * Global_Scaler + (float)HintsBackdrop.Height + num);
		LoadingRot += 0.1f;
		spriteBatch.Draw(LoadingTexture, LoadingPosition, null, Color.White, SwingSkull(), new Vector2(LoadingTexture.Width / 2, LoadingTexture.Height / 2), 4f * Global_Scaler, SpriteEffects.None, 1f);
		if (graphics.GraphicsDevice.Viewport.Width > 700)
		{
			vector = LoadingPosition + new Vector2(-(graphics.GraphicsDevice.Viewport.Width / 2), (float)(graphics.GraphicsDevice.Viewport.Height / 3) * Global_Scaler);
			vector.X = graphics.GraphicsDevice.Viewport.Width / 2;
			DrawShadowedString(HintsFont, Hint_Of_The_Load, vector + new Vector2((0f - HintsFont.MeasureString(Hint_Of_The_Load).X / 2f) * Global_Scaler, HintsFont.MeasureString(Hint_Of_The_Load).Y / 2f * Global_Scaler), Color.White);
		}
		else
		{
			vector = LoadingPosition + new Vector2(-(graphics.GraphicsDevice.Viewport.Width / 2), (float)(graphics.GraphicsDevice.Viewport.Height / 2) * Global_Scaler);
			vector.X = graphics.GraphicsDevice.Viewport.Width / 2;
			DrawShadowedString(HintsFont2, Hint_Of_The_Load, vector + new Vector2((0f - HintsFont2.MeasureString(Hint_Of_The_Load).X / 2f) * Global_Scaler, HintsFont2.MeasureString(Hint_Of_The_Load).Y / 2f * Global_Scaler), Color.White);
		}
		string text = " ";
		if (MainMenuIndexer == 0)
		{
			text = AllLevelNames.LevelName[MainMenuLevelIndexer].ToString();
			DrawShadowedString(loadingFont, "Loading ", LoadingPosition + new Vector2((0f - loadingFont.MeasureString("Loading").X / 2f) * Global_Scaler, (0f - ((float)LoadingTexture.Height * 7f + loadingFont.MeasureString("Loading: ").Y / 2f)) * Global_Scaler), Color.Red);
			DrawShadowedString(loadingFont, text, LoadingPosition + new Vector2((0f - loadingFont.MeasureString(text).X / 2f) * Global_Scaler, (0f - ((float)LoadingTexture.Height * 4f + loadingFont.MeasureString(text).Y / 2f)) * Global_Scaler), Color.Red);
		}
		if (MainMenuIndexer == 1)
		{
			text = AllLevelNames.LevelName[MainMenuLevelIndexer + GauntletRunLevelIndexerEnd].ToString();
			DrawShadowedString(loadingFont, "Loading ", LoadingPosition + new Vector2((0f - loadingFont.MeasureString("Loading").X / 2f) * Global_Scaler, (0f - ((float)LoadingTexture.Height * 7f + loadingFont.MeasureString("Loading: ").Y / 2f)) * Global_Scaler), Color.Red);
			DrawShadowedString(loadingFont, text, LoadingPosition + new Vector2((0f - loadingFont.MeasureString(text).X / 2f) * Global_Scaler, (0f - ((float)LoadingTexture.Height * 4f + loadingFont.MeasureString(text).Y / 2f)) * Global_Scaler), Color.Red);
		}
		if (MainMenuIndexer == 2)
		{
			text = AllLevelNames.LevelName[MainMenuLevelIndexer + DuelingLevelIndexerEnd].ToString();
			DrawShadowedString(loadingFont, "Loading ", LoadingPosition + new Vector2((0f - loadingFont.MeasureString("Loading").X / 2f) * Global_Scaler, (0f - ((float)LoadingTexture.Height * 5f + loadingFont.MeasureString("Loading: ").Y / 2f)) * Global_Scaler), Color.Red);
			DrawShadowedString(loadingFont, text, LoadingPosition + new Vector2((0f - loadingFont.MeasureString(text).X / 2f) * Global_Scaler, (0f - ((float)LoadingTexture.Height * 3f + loadingFont.MeasureString(text).Y / 2f)) * Global_Scaler), Color.Red);
		}
		if (MainMenuIndexer == 3)
		{
			text = "Level Builder";
			DrawShadowedString(loadingFont, "Loading: " + text, LoadingPosition + new Vector2((0f - loadingFont.MeasureString("Loading:" + text).X / 2f + -20f) * Global_Scaler, (0f - ((float)LoadingTexture.Height * 4f + loadingFont.MeasureString("Loading: " + text).Y / 2f)) * Global_Scaler), Color.Red);
		}
		spriteBatch.End();
		base.Draw(gameTime);
	}

	public void DrawRnRStudios(GameTime gameTime)
	{
		if (PreLoadTread_First)
		{
			PreLoadTread_First = false;
			PreLoadTread = new Thread((ThreadStart)delegate
			{
				PreLoadMenu();
			});
			PreLoadTread.Start();
		}
		graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
		disappearEffect.Parameters["OverlayScroll"].SetValue(MoveUp(gameTime, 0.1f) * 0.25f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, disappearEffect);
		float num = (float)gameTime.TotalGameTime.TotalSeconds / Trans_1_Delay * 765f;
		if ((double)num > 382.5)
		{
			num = 765f - num;
		}
		if (num > 255f)
		{
			num = 255f;
		}
		else if (num < 0f)
		{
			num = 0f;
		}
		spriteBatch.Draw(RnRTexture, True_Screen_Center, null, new Color(255, 255, 255, (byte)num), 0f, new Vector2(RnRTexture.Width / 2, RnRTexture.Height / 2), 1.8f * Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.End();
		base.Draw(gameTime);
	}

	public void DrawFarNMerc(GameTime gameTime)
	{
		graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		graphics.GraphicsDevice.Textures[1] = RnRBurnTexture;
		disappearEffect.CurrentTechnique.Passes[0].Apply();
		float num = ((float)gameTime.TotalGameTime.TotalSeconds - Trans_1_Delay) / Trans_2_Delay * 765f;
		if ((double)num > 382.5)
		{
			num = 765f - num;
		}
		if (num > 255f)
		{
			num = 255f;
		}
		else if (num < 0f)
		{
			num = 0f;
		}
		spriteBatch.Draw(FarMercTexture, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2), null, new Color(255, 255, 255, (byte)num), 0f, new Vector2(FarMercTexture.Width / 2, FarMercTexture.Height / 2), 2f * Global_Scaler, SpriteEffects.None, 1f);
		spriteBatch.End();
		base.Draw(gameTime);
	}

	public void PreLoadMenu()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		for (int i = 0; i < Species_Count; i++)
		{
			string assetName = "Flair/" + i + "/All";
			PlayerSpriteSheet[i] = null;
			PlayerSpriteSheet[i] = base.Content.Load<Texture2D>(assetName);
			Player1SpriteSheet = PlayerSpriteSheet[i];
			Player2SpriteSheet = PlayerSpriteSheet[i];
			Player3SpriteSheet = PlayerSpriteSheet[i];
			Player4SpriteSheet = PlayerSpriteSheet[i];
		}
		Thread.CurrentThread.Join();
	}

	private static Vector2 MoveUp(GameTime gameTime, float speed)
	{
		double num = gameTime.TotalGameTime.TotalSeconds * (double)speed;
		float x = 0f;
		float y = (float)num;
		return new Vector2(x, y);
	}

	private static Vector2 ZoonOut(GameTime gameTime, float speed)
	{
		_ = gameTime.TotalGameTime.TotalSeconds;
		float x = 0f;
		float y = 0f;
		return new Vector2(x, y);
	}

	private static float Pulsate(GameTime gameTime, float speed, float min, float max)
	{
		double a = gameTime.TotalGameTime.TotalSeconds * (double)speed;
		return min + ((float)Math.Sin(a) + 1f) / 2f * (max - min);
	}

	public void ExitGame(GameTime gameTime)
	{
		if (MainManuFadeTimeOld + Trans_MM_Delay + 2f < (float)gameTime.TotalGameTime.TotalSeconds)
		{
			MainManuFadeTimeOld = (float)gameTime.TotalGameTime.TotalSeconds;
			MainMenuFadeIn = false;
			MainMenuFadeOut = true;
			MediaPlayer.Stop();
		}
		if (Trans_MM(spriteBatch, gameTime))
		{
			if (Thread_Loading != null)
			{
				Thread_Loading.Abort();
			}
			if (PreLoadTread != null)
			{
				PreLoadTread.Abort();
			}
			Exit();
		}
	}
}
