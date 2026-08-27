using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace EGEngine;

public class LevelBaseMenu : Menu
{
	private const int NumCreditStr0 = 4;

	private const int NumCreditStr1 = 6;

	private const int NumCreditStr2 = 6;

	private const int NumCreditThanks = 32;

	private const int tdNumCreditStr0 = 4;

	private const int tdNumCreditStr1 = 9;

	private const int tdNumCreditStr2 = 9;

	public static bool SavePlayerDataScheduled = false;

	public static bool LoadPlayerDataScheduled = false;

	public static AIBase AvRai;

	private static int debugCount = 0;

	private static bool LoadingSplashDone = false;

	private static float LoadingTimer = 0f;

	private static float LoadingMusicTimer = 0f;

	private static float LoadingSplashAlpha = 0f;

	private static float AvRSplashFrequency = 10f;

	public static bool isTrialMode = false;

	public static bool isLocalMode = false;

	public static GameMode gameMode = GameMode.Menu;

	public static bool returnQuickMatch = true;

	public static bool LoadLevelScheduled = false;

	public static bool UpdateThreadInLoadLevelWait = false;

	public static string LoadLevelName = "";

	public static float debugElapsedSecounds = 0f;

	public static int debugUpdateFPS = 0;

	public static int debugDrawFPS = 0;

	public static int debugPhysicsCounter = 0;

	public static int debugUpdateCounter = 0;

	public static int debugDrawCounter = 0;

	public static int debugIdleUpdateCounter = 0;

	public static bool debugSecondHasElapsed = false;

	public static Terrain tmpTerrain = new Terrain();

	public static TerrainVegetation tmpTerrainVegitation = new TerrainVegetation();

	public static TerrainRoads tmpTerrainRoad = new TerrainRoads();

	public static TerrainRoads tmpTerrainDirtRoad = new TerrainRoads();

	public static TerrainStreams tmpTerrainStreams = new TerrainStreams();

	public static TerrainDynamicPath tmpTerrainBFFootPrints = new TerrainDynamicPath();

	public static HeightMapPhysics tmpHeightMap = new HeightMapPhysics();

	public static GraphicsDevice gdLoadScreen;

	public static GameTime loadStartTime;

	public static bool LoadContentEnabled = false;

	public static bool SkipCreditsEnabled = false;

	public static bool ExitLevel = false;

	public static bool ThreadRunning = false;

	public static bool ThreadMenuRunning = false;

	public static bool ThreadMenuDone = false;

	public static bool PostBloomEnabled = false;

	public static dtStatNavMesh NavigationMesh = new dtStatNavMesh();

	public static bool UpdateThreadRunning = false;

	public static bool FPSCameraActive = false;

	public static InputBase InputUpdate = new InputBase();

	public static int LoadProgressCounter = 0;

	public static Viewport PIPViewport;

	public static float aspectRatio;

	public static Viewport viewport;

	public static bool MutexIsSet = false;

	public static long UpdateMutex = 0L;

	public static long PhysicsMutex = 0L;

	public static int DataQueueUpdate = 0;

	public static int DataQueueRender = 0;

	public static int[] DataQueueStatus = new int[2];

	public static DataQueue[] mDataQueue;

	public static float PhysicsThreadTimer = 0f;

	public static Matrix matTextureProj = Matrix.Identity;

	public static LevelLoadState LoadState = LevelLoadState.NotLoaded;

	public static Vector3 FogColor = new Vector3(0.225f, 0.3f, 0.425f);

	public static RenderTarget2D shadowRenderTarget;

	public static RenderTarget2D[] shadowRenderTarget2;

	public static RenderTarget2D compositeRenderTarget;

	public static RenderTarget2D TemparyCompositeRenderTarget;

	public static RenderTarget2D NormalRenderTarget;

	public static RenderTarget2D MaterialRenderTarget;

	public static RenderTarget2D NormalRenderTargetSplitScreen;

	public static RenderTarget2D DepthRenderTarget;

	public static RenderTarget2D DiffuseRenderTarget;

	public static RenderTarget2D alphaDepthTarget;

	public static RenderTarget2D[] bloomRenderTarget = new RenderTarget2D[6];

	public static RenderTargetBinding[] RenderTargetBindings = new RenderTargetBinding[3];

	public static RenderTargetBinding[] Bloom0TargetBindings = new RenderTargetBinding[2];

	public static TextureCube EnvMap;

	public new static Texture2D backgraoundTexture = null;

	public static Texture2D texCircle;

	public static Texture2D texOrange;

	public static Texture2D texBlack;

	public static Texture2D texBrown;

	public static Texture2D texGray;

	public static Texture2D texWhite;

	public static Texture2D texHitMarker;

	public static Texture2D texNewBackground;

	public static Texture2D texBloodOverlay;

	public static Texture2D texDebugPathing;

	public static Texture2D texWaterNormal;

	public static Texture2D texWaterDetail;

	public static Texture2D texSS0;

	public static Texture2D texSS1;

	public static Texture2D texSS2;

	public static Texture2D texSS3;

	public static Texture2D texSS4;

	public static Texture2D texSS5;

	public static Texture2D texSS6;

	public static Effect EffectDirectionalLight;

	public static EffectParameter vecEyePosition;

	public static EffectParameter vecDirectLightPosition;

	public static EffectParameter vecDirectLightColor;

	public static EffectParameter vecAmbientColor;

	public static EffectParameter matViewProj;

	public static EffectParameter matInvViewProj;

	public static EffectParameter matInvView;

	public static EffectParameter matInvProj;

	public static EffectParameter NormalTexture;

	public static EffectParameter DiffuseTexture;

	public static EffectParameter DepthTexture;

	public static EffectParameter MaterialsTexture;

	public static EffectParameter EnvMap0;

	public static EffectParameter vecFrustumCorners;

	public static EffectTechnique T_DirectLight;

	public static ePointLights PointLights = new ePointLights();

	public static PostProcessEffects PostEffectsClass = new PostProcessEffects();

	private static VS_PostStruct[] postVertices;

	public static VertexBuffer postVertexBuffer;

	public static VertexBuffer postCoOpVertexBuffer;

	public static PlayerBase[] Players = new PlayerBase[4];

	public static PhysicsBase physBase = new PhysicsBase();

	public static Terrain terrain = new Terrain();

	public static particles Particles = new particles();

	public static StickersClass Stickers = new StickersClass();

	public static EmitterClass Emitters = new EmitterClass();

	private static float[] CreditStr0Len = new float[4];

	private static string[] CreditStr0 = new string[4] { "Developed By Sick Kreations Studio", "Produced By Ruben Salazar", "End Game Engine", "End Game Physics" };

	private static string[] CreditStr1 = new string[6] { "Programming", "Rigging", "Animation", "Box Art", "Webpage", "Company Logo" };

	private static float[] CreditStr2Len = new float[6];

	private static string[] CreditStr2 = new string[6] { "Kevin Kelley", "Kevin Kelley", "Kevin Kelley", "Niel Fontaine", "Jason Hughes", "Paul Brink" };

	private static float[] CreditThanksLen = new float[32];

	private static string[] CreditThanks = new string[32]
	{
		"Thanks To", "", "CG Textures", "TurboSquid", "", "TurboSquid Modelers", "3D_Garden", "aj_trax", "BuilderBob", "canadru",
		"ChrisDeKitchen", "Costinus", "csveen", "DHHH", "Digital", "DigitalX", "dinuka", "EarlEstes", "Flewda", "GCMax",
		"Giimann", "mzubak", "pinarci", "richardsee", "Stasma", "synty", "thegreyman1", "theQiwiMan", "WickedDesigns", "",
		"Additional Modeling & Texturing", "Kevin Kelley"
	};

	private static float[] tdCreditStr0Len = new float[4];

	private static string[] tdCreditStr0 = new string[4] { "A Game Warden Production", "Developed By Sick Kreations Studio", "End Game Engine", "End Game Physics" };

	private static string[] tdCreditStr1 = new string[9] { "Game Concept", "Modeling", "Rigging", "Animation", "Texturing", "Programming", "Company Splash", "Special Effects", "Website" };

	private static float[] tdCreditStr2Len = new float[9];

	private static string[] tdCreditStr2 = new string[9] { "Brandon Charles", "Brandon Charles", "Brandon Charles", "Brandon Charles", "Brandon Charles", "Kevin Kelley", "John Conelea", "John Conelea", "Ruben Salazar" };

	public static PlayerInGameMenu InGameMenu = new PlayerInGameMenu(GameMenus.FPSGame);

	public static bool GenericPause = false;

	private static bool OneOffPlayerLoad = false;

	private static bool NetPlayersInitialized = false;

	private static Vector2 debugStringPos = Vector2.Zero;

	private static float ComputeShaderSavetimer = 0f;

	public static bool CanDrawShadowMap = false;

	public static bool isShadowMapDrawn = false;

	private Vector3 tmpEyePos = Vector3.Zero;

	private Vector3[] frustumCorners = new Vector3[4];

	private static float healthDamage0 = 0f;

	private Model tmpSphere;

	public static Vector2 CreditsPosition = Vector2.Zero;

	private static int loadAlpha = 0;

	private static float loadTimer = 0f;

	private static float loadTimer2 = 0f;

	private static int loadProgress = 0;

	private static int loadProgressDir = 1;

	private static string[] igmOptions = new string[3] { "Resume", "Invert Y", "Exit Level" };

	public int vecPosIndex;

	public Vector3[] camPos = new Vector3[7]
	{
		new Vector3(-1400f, 300f, 4400f),
		new Vector3(-1400f, 300f, 1100f),
		new Vector3(-1400f, 300f, -1030f),
		new Vector3(1080f, 300f, -900f),
		new Vector3(1200f, 300f, 3800f),
		new Vector3(-200f, 300f, 5000f),
		new Vector3(-1000f, 300f, 4800f)
	};

	public Vector3[] camLok = new Vector3[7]
	{
		new Vector3(-1073f, 0f, 3000f),
		new Vector3(0f, 0f, 2500f),
		new Vector3(0f, 0f, 2500f),
		new Vector3(0f, 0f, -100f),
		new Vector3(2000f, 0f, 1200f),
		new Vector3(-1073f, 0f, 3000f),
		new Vector3(-1073f, 0f, 3000f)
	};

	public Vector3 deltaPos = Vector3.Zero;

	public Vector3 lookPos = new Vector3(-1400f, 300f, 4400f);

	public static event EventHandler<MenuEntry> LoadLevelCallback;

	public static bool IsPaused()
	{
		if (!FPSGameMenu.isVisable && !Guide.IsVisible)
		{
			return GenericPause;
		}
		return true;
	}

	public static void PrepareLoadLevel()
	{
		LoadPlayerDataScheduled = true;
		DataEncoder.DataBufferIsLoaded = false;
		ApocZSaveDataCls.DeployingTentsToServer = false;
	}

	public LevelBaseMenu(GameMenus id)
		: base(id)
	{
	}

	public static PlayerBase GetPlayerByGamerTag(string gTag)
	{
		for (int i = 0; i < 4; i++)
		{
			if (Players[i].gamerTag == gTag)
			{
				return Players[i];
			}
		}
		return null;
	}

	public static void LoadRenderTargets()
	{
		GC.GetTotalMemory(forceFullCollection: false);
		EndGameEngine.ContentMgr.OutputMemoryUse();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		Thread.Sleep(1);
		shadowRenderTarget2 = new RenderTarget2D[5];
		shadowRenderTarget2[0] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, EndGameEngine.GameSettings.ShadowTextureSize, EndGameEngine.GameSettings.ShadowTextureSize, mipMap: false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
		shadowRenderTarget2[1] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, 1024, 1024, mipMap: false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
		shadowRenderTarget2[2] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, 512, 512, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		shadowRenderTarget2[3] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, 512, 512, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		compositeRenderTarget = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, EndGameEngine.GameSettings.GBufferSizeX, EndGameEngine.GameSettings.GBufferSizeY, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		GC.GetTotalMemory(forceFullCollection: false);
		NormalRenderTarget = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, EndGameEngine.GameSettings.GBufferSizeX, EndGameEngine.GameSettings.GBufferSizeY, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		MaterialRenderTarget = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, EndGameEngine.GameSettings.GBufferSizeX, EndGameEngine.GameSettings.GBufferSizeY, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		DiffuseRenderTarget = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, EndGameEngine.GameSettings.GBufferSizeX, EndGameEngine.GameSettings.GBufferSizeY, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		DepthRenderTarget = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, EndGameEngine.GameSettings.GBufferSizeX, EndGameEngine.GameSettings.GBufferSizeY, mipMap: false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
		int width = 512;
		int height = 288;
		bloomRenderTarget[0] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		bloomRenderTarget[1] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		bloomRenderTarget[2] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		bloomRenderTarget[3] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		bloomRenderTarget[4] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, 256, 128, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		bloomRenderTarget[5] = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, EndGameEngine.GameSettings.GBufferSizeX, EndGameEngine.GameSettings.GBufferSizeY, mipMap: false, SurfaceFormat.Rgba1010102, DepthFormat.Depth24Stencil8);
		ref RenderTargetBinding reference = ref RenderTargetBindings[0];
		reference = NormalRenderTarget;
		ref RenderTargetBinding reference2 = ref RenderTargetBindings[1];
		reference2 = DepthRenderTarget;
		ref RenderTargetBinding reference3 = ref RenderTargetBindings[2];
		reference3 = DiffuseRenderTarget;
		ref RenderTargetBinding reference4 = ref Bloom0TargetBindings[0];
		reference4 = bloomRenderTarget[0];
		ref RenderTargetBinding reference5 = ref Bloom0TargetBindings[1];
		reference5 = bloomRenderTarget[1];
	}

	public static void LoadBaseContent()
	{
		SetupPostVertices();
		for (int i = 0; i < 2; i++)
		{
			DataQueueStatus[i] = 0;
		}
		texCircle = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\circle");
		texOrange = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\orange");
		texBlack = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\black");
		texBrown = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\brown");
		texGray = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\gray");
		texWhite = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\white");
		texHitMarker = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\hitMarker");
		texDebugPathing = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\debugpathing");
		PIPViewport = EndGameEngine.DefualtViewport;
		PIPViewport.Height = 480;
		PIPViewport.Width = 510;
		PIPViewport.X = EndGameEngine.DefualtViewport.Width - PIPViewport.Width - 100;
		PIPViewport.Y = EndGameEngine.DefualtViewport.Height - PIPViewport.Height - 100;
		float m = 0.5f + 0.5f / (float)EndGameEngine.GameSettings.ShadowTextureSize;
		float m2 = 0.5f + 0.5f / (float)EndGameEngine.GameSettings.ShadowTextureSize;
		_ = 0.5f / (float)EndGameEngine.GameSettings.ShadowTextureSize;
		matTextureProj.M11 = 0.5f;
		matTextureProj.M12 = 0f;
		matTextureProj.M13 = 0f;
		matTextureProj.M14 = 0f;
		matTextureProj.M21 = 0f;
		matTextureProj.M22 = -0.5f;
		matTextureProj.M23 = 0f;
		matTextureProj.M24 = 0f;
		matTextureProj.M31 = 0f;
		matTextureProj.M32 = 0f;
		matTextureProj.M33 = 1f;
		matTextureProj.M34 = 0f;
		matTextureProj.M41 = m;
		matTextureProj.M42 = m2;
		matTextureProj.M43 = 0f;
		matTextureProj.M44 = 1f;
		Players[0] = new PlayerBase();
		Players[1] = new PlayerBase();
		Players[2] = new PlayerBase();
		Players[3] = new PlayerBase();
		debugUpdateCounter = 0;
		debugPhysicsCounter = 0;
		for (int j = 0; j < 4; j++)
		{
			CreditStr0Len[j] = Menu.defaultFont.MeasureString(CreditStr0[j]).X;
		}
		for (int k = 0; k < 6; k++)
		{
			CreditStr2Len[k] = Menu.defaultFont.MeasureString(CreditStr2[k]).X;
		}
		for (int l = 0; l < 32; l++)
		{
			CreditThanksLen[l] = Menu.defaultFont.MeasureString(CreditThanks[l]).X;
		}
		for (int n = 0; n < 4; n++)
		{
			tdCreditStr0Len[n] = Menu.defaultFont.MeasureString(tdCreditStr0[n]).X;
		}
		for (int num = 0; num < 9; num++)
		{
			tdCreditStr2Len[num] = Menu.defaultFont.MeasureString(tdCreditStr2[num]).X;
		}
	}

	public static void ResetForMatch()
	{
		Players[0].mRagdoll.IsValid = false;
		Players[1].mRagdoll.IsValid = false;
		Players[2].mRagdoll.IsValid = false;
		Players[3].mRagdoll.IsValid = false;
		EGENetWorkNext.ResetNetworkPlayersRagdoll();
	}

	public static void ResetNetPlayers()
	{
	}

	public static PlayerBase GetNetPlayerIndexed(int i)
	{
		return null;
	}

	public static void LoadPlayersContent()
	{
		if (!OneOffPlayerLoad)
		{
			OneOffPlayerLoad = true;
			Players[(int)EndGameEngine.controllingPlayer.Value].LoadContent(0);
			Players[1].LoadContent(1);
			Players[2].LoadContent(2);
			Players[3].LoadContent(3);
		}
	}

	public static void HackLoadPlayers()
	{
		Players[(int)EndGameEngine.controllingPlayer.Value] = new PlayerBase();
		Players[1] = new PlayerBase();
		Players[2] = new PlayerBase();
		Players[3] = new PlayerBase();
		PlayerBase.PreLoad();
		LoadPlayersContent();
	}

	public override void LoadContent()
	{
		drawMenuBackdrop = false;
		EffectDirectionalLight = EndGameEngine.ContentMgr.Load<Effect>("shaders\\DirectLight");
		vecEyePosition = EffectDirectionalLight.Parameters["vecEyePosition"];
		vecDirectLightPosition = EffectDirectionalLight.Parameters["vecDirectLightPosition"];
		vecDirectLightColor = EffectDirectionalLight.Parameters["vecDirectLightColor"];
		vecAmbientColor = EffectDirectionalLight.Parameters["vecAmbientColor"];
		matViewProj = EffectDirectionalLight.Parameters["matViewProj"];
		matInvViewProj = EffectDirectionalLight.Parameters["matInvViewProj"];
		matInvView = EffectDirectionalLight.Parameters["matInvView"];
		matInvProj = EffectDirectionalLight.Parameters["matInvProj"];
		NormalTexture = EffectDirectionalLight.Parameters["NormalTexture"];
		DiffuseTexture = EffectDirectionalLight.Parameters["DiffuseTexture"];
		DepthTexture = EffectDirectionalLight.Parameters["DepthTexture"];
		MaterialsTexture = EffectDirectionalLight.Parameters["MaterialsTexture"];
		EnvMap0 = EffectDirectionalLight.Parameters["EnvMap0"];
		vecFrustumCorners = EffectDirectionalLight.Parameters["vecFrustumCorners"];
		T_DirectLight = EffectDirectionalLight.Techniques["T_DirectLight"];
		base.LoadContent();
		LoadProgressCounter++;
		LoadPlayersContent();
		LoadProgressCounter++;
		Stickers.Initialize();
		LoadProgressCounter++;
		Particles.Initialize();
		LoadProgressCounter++;
		Emitters.LoadContent();
		MediaEmitterClass.LoadContent();
		for (int i = 0; i < 4; i++)
		{
			Players[i].currentGamePadState = GamePad.GetState(Players[i].playerIndex);
			Players[i].lastGamePadState = Players[i].currentGamePadState;
		}
		LoadProgressCounter++;
		SetPlayerGamerTags();
		LoadProgressCounter++;
		InGameMenu.LoadContent();
		LoadProgressCounter++;
		LoadProgressCounter++;
	}

	public static void EnterLevel()
	{
		InGameMenu.State = MenuState.Hidden;
	}

	public static void LoadPlayers()
	{
		SetPlayerGamerTags();
		for (int i = 0; i < 4; i++)
		{
			Players[i].playerIndex = (PlayerIndex)i;
			Players[i].SetViewport(coop: true, i);
			Players[i].LoadPlayerStatistics();
			Players[i].ToggledRespawn = false;
		}
	}

	public static void SavePlayers()
	{
		if (Players[(int)EndGameEngine.controllingPlayer.Value] == null)
		{
			HackLoadPlayers();
		}
		if (gameMode != GameMode.CoOpPlayer)
		{
			Players[(int)EndGameEngine.controllingPlayer.Value].SavePlayerStatistics();
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			Players[i].SavePlayerStatistics();
		}
	}

	public static void SetPlayerGamerTags()
	{
		if (Players[(int)EndGameEngine.controllingPlayer.Value] == null)
		{
			HackLoadPlayers();
		}
		for (int i = 0; i < 4; i++)
		{
			Players[i].playerTag.gamerPicture = null;
			Players[i].playerTag.gamerProfile = null;
		}
		foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
		{
			if (signedInGamer.PlayerIndex == EndGameEngine.controllingPlayer)
			{
				Players[(int)signedInGamer.PlayerIndex].playerIndex = signedInGamer.PlayerIndex;
				Players[(int)signedInGamer.PlayerIndex].gamerTag = signedInGamer.Gamertag;
				Players[(int)signedInGamer.PlayerIndex].IsValid = true;
				break;
			}
		}
		if (!Players[(int)EndGameEngine.controllingPlayer.Value].IsValid)
		{
			Players[(int)EndGameEngine.controllingPlayer.Value].IsValid = true;
			if (Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag == "Guest")
			{
				Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag = "  " + Players[(int)EndGameEngine.controllingPlayer.Value].gamerTag;
			}
		}
	}

	public override void UnLoadContent()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		ExitLevel = false;
		base.MakeActive(e);
	}

	public override void Update(float eTime)
	{
		currentTime += eTime;
		if (!ThreadRunning && !ExitLevel)
		{
			ThreadRunning = true;
			Thread thread = new Thread(UpdateThreadRun);
			thread.Name = "UpdateThreadRun";
			thread.Start();
			Thread.Sleep(1);
		}
		if (state == MenuState.TransitionOn)
		{
			transitionAlpha = (byte)(currentTime / transitionTime * 255f);
			if (currentTime >= transitionTime)
			{
				state = MenuState.Active;
				transitionAlpha = byte.MaxValue;
			}
		}
		else if (state == MenuState.TransitionOff)
		{
			transitionAlpha = (byte)(255f - currentTime / transitionTime * 255f);
			if (currentTime >= transitionTime)
			{
				state = MenuState.Hidden;
				transitionAlpha = 0;
			}
		}
		base.Update(eTime);
	}

	public override void Draw()
	{
		base.Draw();
		debugDrawCounter++;
		debugStringPos.X = 64f;
		debugStringPos.Y = 300f;
		Menu.spriteBatch.Begin();
		string s = "Update FPS " + debugUpdateFPS + "\n\nDraw FPS " + debugDrawFPS;
		Menu.spriteBatch.DrawString(Menu.defaultFont, s, debugStringPos, Color.Yellow);
		Menu.spriteBatch.End();
	}

	public virtual void DrawLevel()
	{
		if (DataQueueStatus[DataQueueRender] == 1)
		{
			EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
			debugDrawCounter++;
			ProcessComputeShader(DataQueueRender);
			if (FPSCameraActive)
			{
				DrawGameLevel(DataQueueRender);
			}
			EndGameEngine.menuMgr.Draw();
			DataQueueStatus[DataQueueRender] = 0;
			int num = DataQueueRender + 1;
			if (num >= 2)
			{
				num = 0;
			}
			DataQueueRender = num;
		}
		else
		{
			PresentRenderTarget();
			EndGameEngine.menuMgr.Draw();
		}
	}

	public virtual void ProcessComputeShader(int qIndex)
	{
		if (LoadPlayerDataScheduled)
		{
			Effect materialEffect = EndGameEngine.MaterialEffect;
			GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
			graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
			graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[6] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[7] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[8] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[9] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[10] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[11] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[12] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[13] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[14] = SamplerState.PointWrap;
			graphicsDevice.SamplerStates[15] = SamplerState.PointWrap;
			Storage.LoadData();
			LoadPlayerDataScheduled = false;
		}
		if (SavePlayerDataScheduled)
		{
			Effect materialEffect2 = EndGameEngine.MaterialEffect;
			GraphicsDevice graphicsDevice2 = materialEffect2.GraphicsDevice;
			graphicsDevice2.VertexSamplerStates[0] = SamplerState.PointClamp;
			graphicsDevice2.VertexSamplerStates[1] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[0] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[1] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[2] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[3] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[4] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[5] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[6] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[7] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[8] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[9] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[10] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[11] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[12] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[13] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[14] = SamplerState.PointWrap;
			graphicsDevice2.SamplerStates[15] = SamplerState.PointWrap;
			Storage.SaveData(qIndex);
			SavePlayerDataScheduled = false;
		}
	}

	public virtual void DrawGameLevel(int qIndex)
	{
	}

	public virtual void DrawDepthMap(int qIndex)
	{
	}

	public virtual void DrawShadowMap(int qIndex)
	{
	}

	public virtual void UpdateThreadPhysics()
	{
	}

	public virtual void UpdateGameLevel(int qIndex)
	{
		EndGameEngine.enableInputUpdate = true;
	}

	public virtual void UpdateThreadRun()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		EndGameEngine.enableInputUpdate = false;
		while (ThreadRunning)
		{
			debugIdleUpdateCounter++;
			UpdateThread();
		}
		EndGameEngine.enableInputUpdate = true;
		EndGameEngine.menuMgr.MakeActive(GameMenus.Start);
	}

	public virtual void UpdateThread()
	{
		InputUpdate.BeginUpdate(EndGameEngine.currentEleapsedTime);
		if (InputUpdate.menuInput == MenuInput.MenuSelect)
		{
			ExitLevel = true;
			ThreadRunning = false;
		}
	}

	private static void SetupPostVertices()
	{
		postVertexBuffer = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VS_PostStruct), 4, BufferUsage.None);
		Vector3 pos = new Vector3(-1f, 1f, 0f);
		Vector3 pos2 = new Vector3(1f, 1f, 0f);
		Vector3 pos3 = new Vector3(-1f, -1f, 0f);
		Vector3 pos4 = new Vector3(1f, -1f, 0f);
		new Color(255, 255, 255, 255);
		float num = 0f;
		float num2 = 0f;
		postVertices = new VS_PostStruct[4];
		ref VS_PostStruct reference = ref postVertices[0];
		reference = new VS_PostStruct(pos, new Vector2(0f + num, 0f + num2), 0f);
		ref VS_PostStruct reference2 = ref postVertices[1];
		reference2 = new VS_PostStruct(pos2, new Vector2(1f - num, 0f + num2), 1f);
		ref VS_PostStruct reference3 = ref postVertices[2];
		reference3 = new VS_PostStruct(pos3, new Vector2(0f + num, 1f - num2), 3f);
		ref VS_PostStruct reference4 = ref postVertices[3];
		reference4 = new VS_PostStruct(pos4, new Vector2(1f - num, 1f - num2), 2f);
		postVertexBuffer.SetData(postVertices);
		postVertices = new VS_PostStruct[8];
		pos = new Vector3(-0.6f, 1f, 0f);
		pos2 = new Vector3(0.6f, 1f, 0f);
		pos3 = new Vector3(-0.6f, 0f, 0f);
		pos4 = new Vector3(0.6f, 0f, 0f);
		ref VS_PostStruct reference5 = ref postVertices[0];
		reference5 = new VS_PostStruct(pos, new Vector2(0f, 0f), 0f);
		ref VS_PostStruct reference6 = ref postVertices[1];
		reference6 = new VS_PostStruct(pos2, new Vector2(1f, 0f), 1f);
		ref VS_PostStruct reference7 = ref postVertices[2];
		reference7 = new VS_PostStruct(pos3, new Vector2(0f, 0.5f), 3f);
		ref VS_PostStruct reference8 = ref postVertices[3];
		reference8 = new VS_PostStruct(pos4, new Vector2(1f, 0.5f), 2f);
		pos = new Vector3(-0.6f, 0f, 0f);
		pos2 = new Vector3(0.6f, 0f, 0f);
		pos3 = new Vector3(-0.6f, -1f, 0f);
		pos4 = new Vector3(0.6f, -1f, 0f);
		ref VS_PostStruct reference9 = ref postVertices[4];
		reference9 = new VS_PostStruct(pos, new Vector2(0f, 0.5f), 0f);
		ref VS_PostStruct reference10 = ref postVertices[5];
		reference10 = new VS_PostStruct(pos2, new Vector2(1f, 0.5f), 1f);
		ref VS_PostStruct reference11 = ref postVertices[6];
		reference11 = new VS_PostStruct(pos3, new Vector2(0f, 1f), 3f);
		ref VS_PostStruct reference12 = ref postVertices[7];
		reference12 = new VS_PostStruct(pos4, new Vector2(1f, 1f), 2f);
		postCoOpVertexBuffer = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VS_PostStruct), 8, BufferUsage.None);
		postCoOpVertexBuffer.SetData(postVertices);
	}

	public void RenderParticlesDefered()
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		_ = materialEffect.GraphicsDevice;
	}

	public void PresentRenderTarget()
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		int num = DataQueueRender;
		if (num > 1)
		{
			num = 0;
		}
		if (EndGameEngine.menuMgr.IsActive(GameMenus.MainMenu) || EndGameEngine.menuMgr.IsActive(GameMenus.XBoxLiveMenu))
		{
			graphicsDevice.SetRenderTarget(compositeRenderTarget);
			graphicsDevice.Clear(ClearOptions.Target, Color.Black, 1f, 0);
		}
		graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
		graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[6] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[7] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[8] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[9] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[10] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[11] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[12] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[13] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[14] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[15] = SamplerState.PointWrap;
		PostEffectsClass.Bloom(num);
		PostEffectsClass.Particles(Players[(int)EndGameEngine.controllingPlayer.Value], num);
		if (FPSCameraActive)
		{
			Players[(int)EndGameEngine.controllingPlayer.Value].vpViewPort = EndGameEngine.DefualtViewport;
			Players[(int)EndGameEngine.controllingPlayer.Value].DrawPost(num, compositeRenderTarget, bloomRenderTarget[0]);
			DrawInGameMenu(0, Players[(int)EndGameEngine.controllingPlayer.Value]);
			PostMenuUI.Draw(num, Players[(int)EndGameEngine.controllingPlayer.Value]);
		}
		if (Players[(int)EndGameEngine.controllingPlayer.Value].Spawned)
		{
			DrawPost(num);
		}
	}

	public void PresentMenuRenderTarget()
	{
		Effect materialEffect = EndGameEngine.MaterialEffect;
		_ = materialEffect.GraphicsDevice;
		int num = DataQueueRender;
		if (num > 1)
		{
			num = 0;
		}
		PostEffectsClass.Bloom(num);
		PostEffectsClass.Particles(Players[(int)EndGameEngine.controllingPlayer.Value], num);
	}

	public void DrawSecondViewport(Texture2D sprite)
	{
		Rectangle a = new Rectangle(PIPViewport.X, PIPViewport.Y, PIPViewport.Width, PIPViewport.Height);
		Menu.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		Menu.spriteBatch.Draw(sprite, a, null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0);
		Menu.spriteBatch.End();
	}

	public void DrawDebugTexture(Texture2D sprite, int i, int j)
	{
		Rectangle a = new Rectangle(i * 322, j * 322, 320, 320);
		Menu.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		Menu.spriteBatch.Draw(sprite, a, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
		Menu.spriteBatch.End();
	}

	public void DrawLoadingMessage(GameTime gameTime, int timer)
	{
		loadTimer += timer;
		if (loadTimer > 0.1f)
		{
			loadTimer = 0f;
			loadProgress += loadProgressDir;
			if (loadProgress > 5)
			{
				loadProgressDir = -1;
			}
			else if (loadProgress < 1)
			{
				loadProgressDir = 1;
			}
		}
		loadTimer2 += (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		if (loadTimer2 > 15f)
		{
			loadTimer2 = 0f;
			LoadProgressCounter++;
		}
		string text = "Loading";
		for (int i = 0; i < loadProgress; i++)
		{
			text += ".";
		}
		Vector2 zero = Vector2.Zero;
		Vector2 zero2 = Vector2.Zero;
		zero.X = 580f;
		zero.Y = 520f;
		zero2 = zero;
		zero2.X++;
		zero2.Y++;
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Viewport viewport2 = new Viewport
		{
			X = 0,
			Y = 0,
			Width = EndGameEngine.GameSettings.BackBufferSizeX,
			Height = EndGameEngine.GameSettings.BackBufferSizeY
		};
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = viewport2;
		Menu.spriteBatch.Begin();
		if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
		{
			loadAlpha += 6;
			loadAlpha = ((loadAlpha > 255) ? 255 : loadAlpha);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			if (LoadingMusicTimer > AvRSplashFrequency)
			{
				LoadingMusicTimer = 0f;
				if (LoadingSplashAlpha + 1f > 4f)
				{
					LoadingSplashAlpha = 4f;
					SkipCreditsEnabled = false;
				}
				else
				{
					LoadingSplashAlpha++;
				}
			}
			float num = ((AvRSplashFrequency - LoadingMusicTimer > 1f) ? 1f : (AvRSplashFrequency - LoadingMusicTimer));
			num *= (float)loadAlpha;
			Rectangle a = new Rectangle(190, 110, 900, 500);
			if (LoadingSplashAlpha == 0f)
			{
				if (loadAlpha < 255)
				{
					Menu.spriteBatch.Draw(texSS0, a, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				}
				else
				{
					Menu.spriteBatch.Draw(texSS1, a, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
					Menu.spriteBatch.Draw(texSS0, a, new Color((byte)num, (byte)num, (byte)num, (byte)num));
				}
			}
			if (LoadingSplashAlpha == 1f)
			{
				Menu.spriteBatch.Draw(texSS2, a, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS1, a, new Color((byte)num, (byte)num, (byte)num, (byte)num));
			}
			if (LoadingSplashAlpha == 2f)
			{
				Menu.spriteBatch.Draw(texSS3, a, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS2, a, new Color((byte)num, (byte)num, (byte)num, (byte)num));
			}
			if (LoadingSplashAlpha == 3f)
			{
				Menu.spriteBatch.Draw(texSS3, a, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS3, a, new Color((byte)num, (byte)num, (byte)num, (byte)num));
			}
			if (LoadingSplashAlpha == 4f)
			{
				Menu.spriteBatch.Draw(texSS3, a, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
			}
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor"))
		{
			loadAlpha += 6;
			loadAlpha = ((loadAlpha > 255) ? 255 : loadAlpha);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			if (LoadingMusicTimer > AvRSplashFrequency)
			{
				AvRSplashFrequency = 6f;
				LoadingMusicTimer = 0f;
				if (LoadingSplashAlpha + 1f > 6f)
				{
					LoadingSplashAlpha = 6f;
					SkipCreditsEnabled = false;
				}
				else
				{
					LoadingSplashAlpha++;
				}
			}
			float num2 = ((AvRSplashFrequency - LoadingMusicTimer > 1f) ? 1f : (AvRSplashFrequency - LoadingMusicTimer));
			num2 *= (float)loadAlpha;
			Rectangle a2 = new Rectangle(320, 40, 640, 640);
			if (LoadingSplashAlpha == 0f)
			{
				if (loadAlpha < 255)
				{
					Menu.spriteBatch.Draw(texSS0, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				}
				else
				{
					Menu.spriteBatch.Draw(texSS1, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
					Menu.spriteBatch.Draw(texSS0, a2, new Color((byte)num2, (byte)num2, (byte)num2, (byte)num2));
				}
			}
			if (LoadingSplashAlpha == 1f)
			{
				Menu.spriteBatch.Draw(texSS2, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS1, a2, new Color((byte)num2, (byte)num2, (byte)num2, (byte)num2));
			}
			if (LoadingSplashAlpha == 2f)
			{
				Menu.spriteBatch.Draw(texSS3, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS2, a2, new Color((byte)num2, (byte)num2, (byte)num2, (byte)num2));
			}
			if (LoadingSplashAlpha == 3f)
			{
				Menu.spriteBatch.Draw(texSS4, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS3, a2, new Color((byte)num2, (byte)num2, (byte)num2, (byte)num2));
			}
			if (LoadingSplashAlpha == 4f)
			{
				Menu.spriteBatch.Draw(texSS5, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS4, a2, new Color((byte)num2, (byte)num2, (byte)num2, (byte)num2));
			}
			if (LoadingSplashAlpha == 5f)
			{
				Menu.spriteBatch.Draw(texSS6, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS5, a2, new Color((byte)num2, (byte)num2, (byte)num2, (byte)num2));
			}
			if (LoadingSplashAlpha == 6f)
			{
				Menu.spriteBatch.Draw(texSS6, a2, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				if (loadAlpha >= 255)
				{
					LoadingSplashDone = true;
				}
			}
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ"))
		{
			loadAlpha += 6;
			loadAlpha = ((loadAlpha > 255) ? 255 : loadAlpha);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			int height = (int)((float)EndGameEngine.DefualtViewport.TitleSafeArea.Width / (float)Menu.titleTexture.Width * (float)Menu.titleTexture.Height);
			Menu.spriteBatch.Draw(d: new Rectangle(EndGameEngine.DefualtViewport.TitleSafeArea.X, EndGameEngine.DefualtViewport.TitleSafeArea.Y, EndGameEngine.DefualtViewport.TitleSafeArea.Width, height), s: new Rectangle(4, 4, Menu.titleTexture.Width - 8, Menu.titleTexture.Height - 8), t: Menu.titleTexture, c: Color.White);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("TowerDefense"))
		{
			loadAlpha += 6;
			loadAlpha = ((loadAlpha > 255) ? 255 : loadAlpha);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.AlphaBlend;
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
			if (LoadingMusicTimer > AvRSplashFrequency)
			{
				if (LoadingSplashAlpha == 1f)
				{
					AvRSplashFrequency = 16f;
				}
				else
				{
					AvRSplashFrequency = 12f;
				}
				LoadingMusicTimer = 0f;
				if (LoadingSplashAlpha + 1f > 3f)
				{
					LoadingSplashAlpha = 3f;
				}
				else
				{
					LoadingSplashAlpha++;
				}
			}
			float num3 = ((AvRSplashFrequency - LoadingMusicTimer > 1f) ? 1f : (AvRSplashFrequency - LoadingMusicTimer));
			num3 *= (float)loadAlpha;
			Rectangle a3 = viewport2.TitleSafeArea;
			if (LoadingSplashAlpha == 0f)
			{
				SkipCreditsEnabled = true;
				if (loadAlpha < 255)
				{
					Menu.spriteBatch.Draw(texSS0, a3, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				}
				else
				{
					Menu.spriteBatch.Draw(texSS1, a3, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
					Menu.spriteBatch.Draw(texSS0, a3, new Color((byte)num3, (byte)num3, (byte)num3, (byte)num3));
				}
			}
			if (LoadingSplashAlpha == 1f)
			{
				SkipCreditsEnabled = true;
				Menu.spriteBatch.Draw(texSS2, a3, new Color((byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha, (byte)loadAlpha));
				Menu.spriteBatch.Draw(texSS1, a3, new Color((byte)num3, (byte)num3, (byte)num3, (byte)num3));
			}
			if (LoadingSplashAlpha == 2f)
			{
				SkipCreditsEnabled = true;
				Menu.spriteBatch.Draw(texBlack, a3, Color.Black);
				Menu.spriteBatch.Draw(texSS2, a3, new Color((byte)num3, (byte)num3, (byte)num3, (byte)num3));
			}
			if (LoadingSplashAlpha == 3f)
			{
				SkipCreditsEnabled = false;
			}
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("ToyPlane"))
		{
			if (EndGameEngine.videoPlayer.State == MediaState.Playing)
			{
				Texture2D texture = EndGameEngine.videoPlayer.GetTexture();
				Rectangle d = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea;
				float num4 = (float)d.Width / (float)d.Height;
				int num5 = (int)(52f * num4);
				d.X += num5 / 2;
				d.Width -= num5;
				d.Height -= 52;
				Menu.spriteBatch.Draw(texture, d, texture.Bounds, Color.White);
			}
			else
			{
				Vector2 zero3 = Vector2.Zero;
				string text2 = "";
				string text3 = "";
				if (Guide.IsTrialMode)
				{
					text2 = "This Game Automaticly Saves Your Progress";
					text3 = "After Each Level In Full Version";
				}
				else
				{
					text2 = "This Game Automaticly Saves Your Progress";
					text3 = "After Each Level";
				}
				zero3.X = 640f - Menu.defaultFont.MeasureString(text2).X * 0.5f;
				zero3.Y = 340f;
				Menu.spriteBatch.DrawString(Menu.defaultFont, text2, zero3, Color.Yellow);
				zero3.X = 640f - Menu.defaultFont.MeasureString(text3).X * 0.5f;
				zero3.Y = 372f;
				Menu.spriteBatch.DrawString(Menu.defaultFont, text3, zero3, Color.Yellow);
			}
		}
		else if (EndGameEngine.videoPlayer.State == MediaState.Playing)
		{
			Texture2D texture2 = EndGameEngine.videoPlayer.GetTexture();
			new Rectangle(0, 0, 1280, 720);
			Menu.spriteBatch.Draw(texture2, EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea, texture2.Bounds, Color.White);
		}
		else
		{
			Vector2 zero4 = Vector2.Zero;
			string text4 = "";
			string text5 = "";
			if (Guide.IsTrialMode)
			{
				text4 = "This Game Automaticly Saves Your Progress";
				text5 = "After Each Level In Full Version";
			}
			else
			{
				text4 = "This Game Automaticly Saves Your Progress";
				text5 = "After Each Level";
			}
			zero4.X = 640f - Menu.defaultFont.MeasureString(text4).X * 0.5f;
			zero4.Y = 340f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text4, zero4, Color.LightGray);
			zero4.X = 640f - Menu.defaultFont.MeasureString(text5).X * 0.5f;
			zero4.Y = 372f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text5, zero4, Color.LightGray);
		}
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = viewport;
		if (LoadProgressCounter > 25)
		{
			LoadProgressCounter = 25;
		}
		if (EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor"))
		{
			if (SkipCreditsEnabled)
			{
				zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X;
				zero.X -= Menu.defaultFont.MeasureString("Press      To Skip").X * 0.5f;
				int x = (int)(zero.X + Menu.defaultFont.MeasureString("Press ").X);
				Menu.spriteBatch.Draw(Menu.aButton, new Rectangle(x, (int)zero.Y, 32, 32), Color.White);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Press      To Skip", zero + new Vector2(2f, 2f), Color.Black);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Press      To Skip", zero, Color.LightGray);
			}
			else
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero + new Vector2(2f, 2f), Color.Black);
				Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray);
				Menu.spriteBatch.Draw(texGray, new Rectangle(437, 558, 406, 12), new Color(40, 40, 40, 120));
				int width = (int)((float)LoadProgressCounter * 4f / 100f * 400f);
				Menu.spriteBatch.Draw(texBrown, new Rectangle(440, 560, width, 8), Color.White);
			}
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
		{
			if (SkipCreditsEnabled)
			{
				zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X;
				zero.X -= Menu.defaultFont.MeasureString("Press    To Skip").X * 0.5f;
				int x2 = (int)(zero.X + Menu.defaultFont.MeasureString("Press ").X);
				Menu.spriteBatch.Draw(Menu.aButton, new Rectangle(x2, (int)zero.Y, 24, 24), Color.White);
				Color lightGreen = Color.LightGreen;
				lightGreen.R = 60;
				lightGreen.G = 180;
				lightGreen.B = 60;
				lightGreen.A = byte.MaxValue;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Press    To Skip", zero + new Vector2(2f, 2f), Color.Black);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Press    To Skip", zero, lightGreen);
			}
			else
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.Green);
				Menu.spriteBatch.Draw(texGray, new Rectangle(437, 558, 406, 12), new Color(40, 40, 40, 120));
				int width2 = (int)((float)LoadProgressCounter * 4f / 100f * 400f);
				Menu.spriteBatch.Draw(texBrown, new Rectangle(440, 560, width2, 8), Color.White);
			}
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("TowerDefense"))
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero + new Vector2(2f, 2f), Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray);
			Menu.spriteBatch.Draw(texGray, new Rectangle(437, 558, 406, 12), new Color(40, 40, 40, 120));
			int width3 = (int)((float)LoadProgressCounter * 4f / 100f * 400f);
			Menu.spriteBatch.Draw(texBrown, new Rectangle(440, 560, width3, 8), Color.White);
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("ToyPlane"))
		{
			if (SkipCreditsEnabled && EndGameEngine.videoPlayer.State != MediaState.Playing)
			{
				zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X;
				zero.X -= Menu.defaultFont.MeasureString("Press      To Skip").X * 0.5f;
				zero.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 48;
				int x3 = (int)(zero.X + Menu.defaultFont.MeasureString("Press ").X);
				Menu.spriteBatch.Draw(Menu.aButton, new Rectangle(x3, (int)(zero.Y + 2f), 36, 36), Color.White);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Press      To Skip", zero, Color.Yellow);
			}
			else
			{
				zero.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 48;
				Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.Yellow);
				Menu.spriteBatch.Draw(texGray, new Rectangle(437, EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 12, 406, 12), new Color(40, 40, 40, 120));
				int width4 = (int)((float)LoadProgressCounter * 4f / 100f * 400f);
				Menu.spriteBatch.Draw(texBrown, new Rectangle(440, EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 10, width4, 8), Color.White);
			}
		}
		else if (SkipCreditsEnabled)
		{
			zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X;
			zero.X -= Menu.defaultFont.MeasureString("Press      To Skip").X * 0.5f;
			int x4 = (int)(zero.X + Menu.defaultFont.MeasureString("Press ").X);
			Menu.spriteBatch.Draw(Menu.aButton, new Rectangle(x4, (int)zero.Y, 32, 32), Color.White);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Press      To Skip", zero + new Vector2(4f, 4f), Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Press      To Skip", zero, Color.LightGray);
		}
		else
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray);
			Menu.spriteBatch.Draw(texGray, new Rectangle(437, 558, 406, 12), new Color(40, 40, 40, 120));
			int width5 = (int)((float)LoadProgressCounter * 4f / 100f * 400f);
			Menu.spriteBatch.Draw(texBrown, new Rectangle(440, 560, width5, 8), Color.White);
		}
		Menu.spriteBatch.End();
	}

	public GameTime GetGameTime(ref long lastTime)
	{
		long timestamp = Stopwatch.GetTimestamp();
		long num = timestamp - lastTime;
		lastTime = timestamp;
		TimeSpan timeSpan = TimeSpan.FromTicks(num * 10000000 / Stopwatch.Frequency);
		return new GameTime(loadStartTime.TotalGameTime + timeSpan, timeSpan);
	}

	public void PrepareUpdateload()
	{
		debugCount = 0;
		LoadingTimer = 0f;
		if (Menu.menuMusic == null)
		{
			if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
			{
				texSS0 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\ss0");
				texSS1 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\ss1");
				texSS2 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\ss2");
				texSS3 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\ss3");
			}
			else if (!EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor") && !EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ") && EndGameEngine.GameSettings.GameName.Contains("TowerDefense"))
			{
				AvRSplashFrequency = 16f;
				texSS0 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\radio_1");
				texSS1 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\radio_2");
				texSS2 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\radio_3");
			}
		}
		CreditsPosition.X = 640f;
		CreditsPosition.Y = 500f;
	}

	public void UpdateLoad(GameTime gameTime)
	{
		float num = 25f;
		LoadingTimer += gameTime.ElapsedGameTime.Milliseconds;
		if (LoadingTimer > num)
		{
			LoadingTimer -= num;
			if (LoadingTimer > num)
			{
				LoadingTimer = 0f;
			}
			debugCount++;
		}
		if (EndGameEngine.GameSettings.GameName.Contains("_AvR_") || EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor") || EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ") || EndGameEngine.GameSettings.GameName.Contains("TowerDefense"))
		{
			LoadingMusicTimer += (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		}
	}

	public void UpdateLoadThread()
	{
		debugCount = 0;
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		float num = 0f;
		int num2 = 0;
		Stopwatch.GetTimestamp();
		long lastTime = Stopwatch.GetTimestamp();
		if (EndGameEngine.videoPlayer.IsDisposed)
		{
			EndGameEngine.videoPlayer = new VideoPlayer();
		}
		EndGameEngine.videoPlayer.Play(EndGameEngine.ContentMgr.Load<Video>("NewStory_01"));
		CreditsPosition.X = 640f;
		CreditsPosition.Y = 580f;
		while (LoadState != LevelLoadState.Loaded)
		{
			GameTime gameTime = GetGameTime(ref lastTime);
			float num3 = 25f;
			num += (float)gameTime.ElapsedGameTime.Milliseconds;
			if (num > num3)
			{
				num -= num3;
				if (num > num3)
				{
					num = 0f;
				}
				debugCount++;
				DrawLoadScreen(gameTime);
			}
			num2 += gameTime.ElapsedGameTime.Milliseconds;
			if (num2 > 15)
			{
				MessagePump.Update((float)num2 * 0.001f);
				EndGameEngine.AudioEng.Update();
				num2 = 0;
			}
		}
	}

	public void DrawLoadScreen(GameTime gameTime)
	{
		gdLoadScreen = EndGameEngine.GraphicMgr.GraphicsDevice;
		if (gdLoadScreen == null || gdLoadScreen.IsDisposed)
		{
			return;
		}
		try
		{
			gdLoadScreen.Clear(Color.Black);
			DrawLoadingMessage(gameTime, 1);
			MessagePump.Draw();
			gdLoadScreen.Present();
		}
		catch
		{
			gdLoadScreen = null;
		}
	}

	public static bool ToggleInGameMenu(int qIndex, PlayerBase player)
	{
		bool result = false;
		switch (player.MenuState)
		{
		case PlayerMenuState.InGame:
			result = true;
			player.MenuState = PlayerMenuState.InMenu;
			break;
		case PlayerMenuState.InMenu:
			player.MenuState = PlayerMenuState.InGame;
			break;
		case PlayerMenuState.WaitAllQuit:
			result = true;
			player.MenuState = PlayerMenuState.InMenu;
			break;
		}
		return result;
	}

	public static void UpdateInGameMenu(int qIndex, PlayerBase player)
	{
		_ = player.MenuState;
		_ = 2;
	}

	private static void ExitInGameMenuFunc(object sender, MenuEntry e)
	{
		if (InGameMenu.playerRef != null)
		{
			ExitLevelFunc(InGameMenu.playerRef);
		}
	}

	public static void ExitLevelFunc(PlayerBase playerRef)
	{
		ExitLevel = true;
		ThreadRunning = false;
		playerRef.SavePlayerStatistics();
	}

	public static void DrawInGameMenu(int qIndex, PlayerBase player)
	{
	}

	public void UpdateThreadNew()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition = camPos[vecPosIndex] + Vector3.UnitX;
		Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection = camLok[vecPosIndex] - camPos[vecPosIndex];
		Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection.Normalize();
		Players[(int)EndGameEngine.controllingPlayer.Value].MatchCoolDownTimer = -1f;
		Players[(int)EndGameEngine.controllingPlayer.Value].DeathTimer = 1f;
		while (UpdateThreadRunning)
		{
			debugUpdateCounter++;
			if (LoadState != LevelLoadState.Loaded || DataQueueStatus[DataQueueUpdate] != 0)
			{
				continue;
			}
			if (LoadLevelScheduled)
			{
				RunScheduleLevelLoad();
			}
			InputUpdate.BeginUpdate(EndGameEngine.currentEleapsedTime);
			ConfirmMessage.Update(0.015f);
			ErrorMessage.Update(0.015f);
			MessagePump.Update(0.015f);
			for (int i = 0; i < 4; i++)
			{
				if (Players[i].IsValid)
				{
					Menu.ActivePlayer = Players[i];
					EndGameEngine.menuMgr.Update(EndGameEngine.currentTimeStep);
				}
			}
			if (FPSCameraActive)
			{
				UpdateGameLevel(DataQueueUpdate);
			}
			EGENetWorkNext.Update(EndGameEngine.currentTimeStep, DataQueueUpdate);
			EndGameEngine.AudioEng.Update();
			if (StartMenu.ApocThemeMusic != null && !StartMenu.ApocThemeMusic.IsDisposed && StartMenu.ApocThemeMusic.IsPlaying)
			{
				if (StartMenu.ApocThemeMusicRampUp)
				{
					StartMenu.ApocThemeMusicVolume = ((StartMenu.ApocThemeMusicVolume > 220f) ? (StartMenu.ApocThemeMusicVolume - 220f) : 0f);
					StartMenu.ApocThemeMusic.SetVariable("Distance", StartMenu.ApocThemeMusicVolume);
				}
				else if (StartMenu.ApocThemeMusicVolume < 20000f)
				{
					StartMenu.ApocThemeMusicVolume += 100f;
					StartMenu.ApocThemeMusic.SetVariable("Distance", StartMenu.ApocThemeMusicVolume);
				}
				else
				{
					StartMenu.PlayThemeMusic(e: false);
				}
			}
			DataQueueStatus[DataQueueUpdate] = 1;
			int num = DataQueueUpdate + 1;
			if (num >= 2)
			{
				num = 0;
			}
			DataQueueUpdate = num;
		}
	}

	public virtual void UpdateGameLogic(float eTime, int qIndex)
	{
	}

	public virtual void RunScheduleLevelLoad()
	{
		MenuEntry e = new MenuEntry();
		LoadLevelCallback(null, e);
	}

	public virtual void ScheduleNewLevelLoad(string name, EventHandler<MenuEntry> cb)
	{
		LoadLevelName = name;
		LoadLevelCallback += cb;
		LoadLevelScheduled = true;
		UpdateThreadInLoadLevelWait = false;
	}

	public void UpdateMenuThreadRun()
	{
		ThreadMenuDone = false;
		ThreadMenuRunning = true;
		Thread thread = new Thread(UpdateMenuThread);
		thread.Name = "UpdateMenuThread";
		thread.Start();
		Thread.Sleep(1);
	}

	public void UpdateMenuThread()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition = camPos[vecPosIndex] + Vector3.UnitX;
		Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection = camLok[vecPosIndex] - camPos[vecPosIndex];
		Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection.Normalize();
		while (ThreadMenuRunning)
		{
			UpdateMenuInnerLoop(0);
		}
		ThreadMenuDone = true;
		EndGameEngine.enableClearTarget = true;
	}

	public virtual void UpdateMenuReset()
	{
	}

	public virtual void UpdateMenuInnerLoop(int qIndex)
	{
		deltaPos = camPos[vecPosIndex] - Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition;
		lookPos = Vector3.Lerp(lookPos, camLok[vecPosIndex], 0.01f);
		if (deltaPos.Length() < 60f)
		{
			vecPosIndex++;
			if (vecPosIndex > 6)
			{
				vecPosIndex = 0;
			}
		}
		deltaPos.Normalize();
		Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition += deltaPos * 4f;
		Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection = lookPos - Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition;
		Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection.Normalize();
		Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view = Matrix.CreateLookAt(Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition, Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition + Players[(int)EndGameEngine.controllingPlayer.Value].vecDirection * 1000f, Vector3.UnitY);
		Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].Matrix = Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].view * Players[(int)EndGameEngine.controllingPlayer.Value].mDataQueue[qIndex].projection;
		mDataQueue[qIndex].world = Matrix.Identity;
		LevelOutside.Update(qIndex, Players[(int)EndGameEngine.controllingPlayer.Value], 0);
		Emitters.Update(0.01667f);
		Particles.Update((float)EndGameEngine.currentEleapsedTime.ElapsedGameTime.Milliseconds * 0.001f, qIndex);
		Particles.UpdatePlayer(Players[(int)EndGameEngine.controllingPlayer.Value], qIndex);
	}

	public virtual void DrawMenuLevel(int qIndex)
	{
	}

	public virtual void DrawPost(int qIndex)
	{
		AvRai.DrawPost(qIndex, Players[(int)EndGameEngine.controllingPlayer.Value]);
	}

	public virtual void DrawGameLogic(PlayerBase playerRef, int qIndex)
	{
	}
}
