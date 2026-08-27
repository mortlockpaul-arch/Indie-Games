using System;
using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using SkinnedModel;

namespace EGEngine;

public class PlayerBase : PlayerBaseState
{
	public enum RenderPass
	{
		GBufferPass,
		CompositePass,
		DirectLightPass,
		ForwardPass,
		PresentPass,
		FPSDrawPass,
		PostScopePass,
		PostProcessPass,
		MenuGBufferPass,
		MenuForwardPass
	}

	private const float WALK_SPEED = 180f;

	public const int MAX_LINEOFSIGHT_ENTRIES = 16;

	public const int MAX_ATTACKINGBOTS = 8;

	public const int PLAYERDIS_QUANTIZER = 16;

	public const float KNIFE_THRUST = 20f;

	public const float RESPAWN_DELAY_TIME = 2f;

	private const int MaxSkinnedParts = 7;

	private static Random rand = new Random(3);

	public static bool AvR_Hack = false;

	public static bool EoDSurvival_Hack = false;

	public static bool ApocalypseZ_Hack = false;

	public static bool ToyPlane_Hack = false;

	public static bool ByPassFPS = false;

	public static float FarZPlane = 60000f;

	public static float NearZPlane = 1f;

	public static float FogStart = 2200f;

	public static float FogEnd = 4200f;

	public static float FadeToBlack = 1f;

	public bool FlashLightOn;

	public Color flashLightColor;

	public Vector3 flashLightDir;

	public Vector3 flashLightPos;

	public Matrix[] matFlashLight = new Matrix[2];

	public int CurrentDay;

	public float FoodLevel = 100f;

	public float WaterLevel = 100f;

	public float BloodLevel = 100f;

	public float BloodLoss;

	public float PainPillTimer;

	public int CurrentBacpPack;

	public float CurrentDetectionDistance = 1000f;

	public dtStatNavMesh.dtStatNavMeshHeader CurrentPathingData;

	public WorldAreaCls CurrentCollisionArea;

	public byte[] PlayerDistanceQuant = new byte[16];

	public ushort[] PlayerLineOfSight = new ushort[16];

	public ushort[] AttackingBot_UID = new ushort[8];

	public int numberCollisionTest;

	public bool OverrideLevelOutsideCollision;

	public int CurrentBulletsFiredCount;

	public int CurrentBulletsHitCount;

	public int CurrentTimeScore;

	public int CurrentTargetScore;

	public int CurrentRatioScore;

	public int CurrentTotalScore;

	public float Zoom = (float)Math.PI / 4f;

	public float AspectRatio = 1f;

	public int vehicleSeat;

	public bool IsAttached0;

	public bool OverrideInput;

	public bool OverrideCamera;

	public bool OverrideButtonX;

	public bool OverrideButtonTriggerRight;

	public bool OverrideButtonLeftShoulder;

	public bool OverridePosition;

	public bool OverrideProjection;

	public float ZoomOverride = (float)Math.PI / 4f;

	public Vector3 OverrideUp = Vector3.Zero;

	public Vector3 OverrideDir = Vector3.Zero;

	public Vector3 OverrideRight = Vector3.Zero;

	public Vector3 OverridePos = Vector3.Zero;

	public TriggerFlags triggerFlags;

	public TriggerTypes CurrentTrigger;

	public bool TriggerDown;

	public float FireTimer;

	public float FireRate = 0.1f;

	public Cue fireSND0;

	public Cue whizSND0;

	private static string[] BulletWhizSounds = new string[3] { "SingleWhizBy00", "SingleWhizBy01", "SingleWhizBy02" };

	private float[] MuzzleSoundDelay = new float[6];

	private float[] MuzzleDistanceDelay = new float[6];

	private Cue MeleeSwishSound;

	public bool onWalkable;

	public float JumpYValue;

	public float JumpYTime;

	public float CrouchY;

	public int inMenuIndex;

	public PlayerStance Stance;

	public bool drawHalographicUI;

	public bool AimAssist = true;

	public bool Sighted;

	public bool[] isSighted = new bool[2];

	public float SpawnOverRideTimer;

	public float InvertY = 1f;

	public float Yaw;

	public float Pitch;

	public float YawSwayX;

	public float YawSwayY;

	public float GravityAccel;

	public float MoveX;

	public float MoveY;

	public float FootStepTimer;

	public float RunTimer;

	public float RunCoolDownTimer;

	public float Endurance = 8f;

	public float RUN_COOL_DOWN = 3f;

	public float fKnifeThust;

	private float MeleeAttackTimer;

	public float NoiseLevel;

	public float VisibleLevel;

	public float LeftArmRotationY;

	public float LeftArmRotationX;

	public Vector3 vecCameraUp = Vector3.UnitY;

	public Vector3 vecCameraUpSway = Vector3.UnitY;

	public Vector3 vecHead3rdPersonPos = Vector3.Zero;

	public Vector3 vecLastDirection = Vector3.UnitZ;

	public float CharacterYaw;

	public Matrix matYaw;

	public Matrix matPitch;

	public Matrix matView;

	public Matrix menuView;

	public Matrix menuProj;

	public DataQueue[] mDataQueue = new DataQueue[2];

	public Matrix[] matProjection = new Matrix[2];

	public static Matrix[] matSkyDomeProjection = new Matrix[2];

	public Vector3 vecLightPos = Vector3.Zero;

	public Matrix matLightView = Matrix.Identity;

	public Matrix matLightProj = Matrix.Identity;

	public Matrix matLightView2 = Matrix.Identity;

	public Matrix matLightProj2 = Matrix.Identity;

	public Viewport vpViewPort;

	public bool ClearInput;

	public int CoOpPlayerIndex;

	public PlayerIndex playerIndex;

	public GamePadState currentGamePadState;

	public GamePadState lastGamePadState;

	public MenuInput menuInput;

	public MenuInput menuDPadInput;

	public MenuInput menuInputContinuos;

	public MenuInput menuInputRightStick;

	public BotPhysics physics;

	public Matrix mat3rdPaerson = Matrix.Identity;

	private static WeaponHalographicUI UIHalographic;

	public Vector3[] vecHeadPosition = new Vector3[2];

	private VS_ReticleStruct[] reticleVertices;

	private static VertexBuffer reticleVertexBuff;

	public static Texture2D BottomUI;

	public static Texture2D BulletUI;

	public static Texture2D DPadRightIconUI;

	public static Texture2D DPadLeftIconUI;

	public static Texture2D ThrowKnifeIconUI;

	public static Texture2D FragIconUI;

	public static Texture2D SmookeIconUI;

	public static Texture2D NaderIconUI;

	public static Texture2D NadeReticleUI;

	public float ShirtIndex;

	public float PantstIndex;

	private static bool TexturesInit = false;

	public static Texture2D SurvivorShirtDiffuse1;

	public static Texture2D SurvivorShirtDiffuse2;

	public static Texture2D SurvivorShirtNormal1;

	public static Texture2D SurvivorShirtNormal2;

	public static Texture2D SurvivorPantsDiffuse1;

	public static Texture2D SurvivorPantsDiffuse2;

	public static Texture2D SurvivorPantsNormal1;

	public static Texture2D SurvivorPantsNormal2;

	public float fHitIndicatorTimer;

	public BoundingFrustum[] bFrustum = new BoundingFrustum[2];

	public Cue OutOfBreatheSnd;

	public Cue FootStepSound;

	private static Vector3 VecUnitX = Vector3.UnitX;

	private static Vector3 VecUnitY = Vector3.UnitY;

	private static Vector3 VecUnitZ = Vector3.UnitZ;

	public List<FPSCharacterData> fpsCharacterData = new List<FPSCharacterData>();

	private static string[] footStepGrassSoundCues = new string[11]
	{
		"GrassStep1", "GrassStep2", "GrassStep3", "GrassStep4", "GrassStep5", "GrassStep6", "GrassStep7", "GrassStep3", "GrassStep5", "GrassStep4",
		"GrassStep6"
	};

	private static string[] footStepDirtSoundCues = new string[11]
	{
		"StepDirt1", "StepDirt2", "StepDirt3", "StepDirt4", "StepDirt5", "StepDirt6", "StepDirt7", "StepDirt3", "StepDirt5", "StepDirt4",
		"StepDirt6"
	};

	private static SkinnedEffectParams[] characterEffects;

	private static bool Initailized = false;

	private static bool OneOffReadPlayerStatsLive = true;

	private static bool OneOffReadPlayerStatsLocal = true;

	private static bool OneOffLeaderboardInsert = true;

	private bool ContentInitialized;

	public Vector4 UVDisplacement = new Vector4(1f, 1f, 0f, 0f);

	public float DrawMuzzleFlashAlpha;

	public bool ShotFired;

	private static PlayerBase otherPlayerLOS;

	private Vector3 tmpUDDir = Vector3.Zero;

	private Vector3 tmpUDRight = Vector3.Zero;

	private Vector3 tmpUDPlrPos = Vector3.Zero;

	private static Cue[] tmpSndCue = new Cue[5];

	public static Matrix tmpPlayerScale = Matrix.CreateScale(0.75f);

	private static int footStepIndex = 0;

	private static int[] stepSequence = new int[8] { 0, 1, 2, 3, 4, 2, 1, 3 };

	public static bool DispRunInSpectator = false;

	public static bool runInSpectator = false;

	public static int NetworkUpdateFrameCount = 0;

	private static float SightOnFireTimer = 0f;

	public static bool RampThirdPersonCameraFireUp = false;

	public static float ThirdPersonCameraFire = 0f;

	private static float ThirdPersonCameraZoomOnMove = 0f;

	public static float CurrentViewDis = 200f;

	public static int TmpViewDis = 200;

	public static int TmpViewHeight = 50;

	public static int TmpRightShift = -10;

	public static int SightedViewHeight = 2;

	public static int SightedViewDis = 80;

	public static int SightedRightShift = -8;

	public static int ThirdPersonTorsoAngle = 6;

	private static Vector3 tmpCamDir0 = Vector3.Zero;

	private static Vector3 tmpTargetPos = Vector3.Zero;

	private static Vector3 hitPosition = Vector3.Zero;

	private static Vector3 hitNormal = Vector3.Zero;

	private static Ray tmpRay = default(Ray);

	public static Vector3 ThirdPersonCameraPos = Vector3.Zero;

	public float saveStatusTimer = 30f;

	private static float zoomSwivelTimer = 0f;

	private static Vector3 zoomSwivel = Vector3.Zero;

	private static Vector3 zoomSwivelAt = Vector3.Zero;

	public float zoomCamdivisor = 2500f;

	public float zoomCamOverride = (float)Math.PI / 4f;

	private static Vector3 tmpVec3rdPlayer = Vector3.Zero;

	private static Vector3 tmpVec3rdRight = Vector3.Zero;

	private static Matrix tmpMat3rdPlayerYaw = Matrix.Identity;

	private static Matrix tmpMat3rdPlayer = Matrix.Identity;

	private static Matrix tmpHeadFestTmpHead = Matrix.CreateScale(2f);

	private static Matrix[] tmpMat3rdPerson = new Matrix[36];

	private static ContainmentType tmp3rdPersonContianment = ContainmentType.Disjoint;

	private static BoundingSphere tmp3rdPersonSphere = new BoundingSphere(Vector3.Zero, 80f);

	private static Matrix tmpMatBigHack = Matrix.Identity;

	public Matrix thirdPersonHeadmat = Matrix.Identity;

	private Matrix tmp3rdPersonHand = Matrix.Identity;

	private float shadowScale = 16f;

	private static Vector3 lightLookAt = Vector3.Zero;

	private static Vector3 lightDirection = Vector3.Zero;

	private static Vector3 lightPosition = Vector3.Zero;

	private float m11;

	private float m12;

	private float m13;

	private float m14;

	private float m21;

	private float m22;

	private float m23;

	private float m24;

	private float m31;

	private float m32;

	private float m33;

	private float m34;

	private float m41;

	private float m42;

	private float m43;

	private float m44;

	private float lookAtDis;

	private float lookAtDis2 = 2600f;

	private float scaleX = 10f;

	private float scaleY = 10f;

	private Vector3 lightViewDir = Vector3.UnitZ;

	private static Matrix drawTexProj = Matrix.Identity;

	private static Matrix tmpSight = Matrix.Identity;

	private static Matrix drawRearSight = Matrix.CreateRotationX(MathHelper.ToRadians(-90f));

	private static Matrix drawFrontSight = Matrix.CreateRotationX(MathHelper.ToRadians(90f));

	private static Matrix drawtmpMatrix = Matrix.Identity;

	private static Matrix drawtmpRight = Matrix.Identity;

	private Vector3 tmpUIWeaponOffset = Vector3.Zero;

	private Vector3 tmpUIWeaponTransSave = Vector3.Zero;

	private static Vector3 sightedOffset = new Vector3(-0.5f, -20f, 5f);

	private static Vector3 sightedSniperOffset = new Vector3(-2f, -20f, 1f);

	private static Vector3 sightedPistolOffset = new Vector3(-2f, 10f, -1f);

	public static float unlockMessageTimer = 0f;

	public static float unlockMsgXOffset = 0f;

	public static string unlockMessageStr = "";

	private static Color colorNades = new Color(180, 180, 180, 180);

	private static Color colorHitMarker = new Color(255, 0, 0, 255);

	private static Color markerColor = new Color(0, 180, 0, 180);

	private static Vector2 tmpProjectionDirection;

	private static Vector2 screenPosition = Vector2.Zero;

	private static Vector4 projectedPosition = Vector4.Zero;

	private static Rectangle playerMarker = new Rectangle(0, 0, 16, 2);

	private static Rectangle nadeRec = new Rectangle(0, 520, 22, 36);

	private static Rectangle hitMarkerRec = new Rectangle(631, 352, 19, 17);

	private static Rectangle recGeneric = new Rectangle(0, 0, 0, 0);

	private static PlayerBaseState otherPlayerLOSDraw;

	private static ModelMesh drawMesh;

	private static ModelMeshPart drawMeshPart;

	private static Vector2 MuzzleHeat = Vector2.Zero;

	private static Vector2 vecTexOffset = Vector2.Zero;

	public static Vector3 CoOpOffset = new Vector3(0f, -88f, 0f);

	private static Vector3 BigHackSpawnBlinkDrawYuck = Vector3.Zero;

	private static float tmpNPlrProximity = 64f;

	private static float tmpNPlrProximitySqr = 4096f;

	private static Vector3 tmpVecToEnemy = Vector3.Zero;

	private static Vector3 tmpPos = Vector3.Zero;

	private static Vector3 tmpLastPos = Vector3.Zero;

	private static Vector3 tmpVecMovement = Vector3.Zero;

	private static BoundingSphere tmpSphere = default(BoundingSphere);

	private static CollisionStruct tmpCollision = default(CollisionStruct);

	private static IntersectSegmentParams SegmentParams = default(IntersectSegmentParams);

	public static int numDamageGenerated = 0;

	public static Vector3 tmpHitPosition = Vector3.Zero;

	private static float reticleScale = 0.006f;

	private static Matrix reticleMat = Matrix.CreateScale(reticleScale, reticleScale, reticleScale);

	private static float reticleMinSize = 1f;

	private static float reticleMaxSize = 4f;

	private static Vector2[] tmpA = new Vector2[4]
	{
		new Vector2(0f - reticleMinSize, 0f),
		new Vector2(reticleMinSize, 0f),
		new Vector2(0f, 0f - reticleMinSize),
		new Vector2(0f, reticleMinSize)
	};

	private static Vector2[] tmpB = new Vector2[4]
	{
		new Vector2(0f - reticleMaxSize, 0f),
		new Vector2(reticleMaxSize, 0f),
		new Vector2(0f, 0f - reticleMaxSize),
		new Vector2(0f, reticleMaxSize)
	};

	private static Vector2[] tmpC = new Vector2[4];

	public PlayerBase()
	{
		IsValid = false;
	}

	public void NextCharacter()
	{
		CharacterIndex++;
		if (CharacterIndex >= PlayerBaseState.characterBase.Length)
		{
			CharacterIndex = 0;
		}
		SetCharacter(CharacterIndex, ShirtIndex, PantstIndex);
	}

	public void SetCharacter()
	{
		SetCharacter(CharacterIndex, ShirtIndex, PantstIndex);
	}

	public void SetCharacter(byte e, float si, float pi)
	{
		CharacterIndex = e;
		currentCharacterIndex = e;
		SetCurrentCharacter(e);
		cPlayer.SetBaseAnimation(WeaponAnim.CoOpIdleEmpty);
		ShirtIndex = si;
		PantstIndex = pi;
	}

	public static void PreLoad()
	{
		if (Initailized)
		{
			return;
		}
		Initailized = true;
		BottomUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\bottomui");
		BulletUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\bulletui");
		DPadRightIconUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\dpright");
		DPadLeftIconUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\dpleft");
		ThrowKnifeIconUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\throwknife");
		FragIconUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\fragicon");
		SmookeIconUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\smokeicon");
		NaderIconUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\nader");
		NadeReticleUI = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\nadereticle");
		List<CharacterData> list = EndGameEngine.GameAssetMgr.Load<List<CharacterData>>("data\\CharacterDataXml");
		Animation.LoadAnimations(list);
		PlayerBaseState.characterBase = new Model[list.Count];
		characterEffects = new SkinnedEffectParams[list.Count * 7];
		for (int i = 0; i < list.Count; i++)
		{
			string assetName = "models\\characters\\" + list[i].Name;
			PlayerBaseState.characterBase[i] = EndGameEngine.GameAssetMgr.Load<Model>(assetName);
			Matrix matrix = Matrix.CreateScale(list[i].Scale);
			SkinningData skinningData = ((SkinnedAnimationData)PlayerBaseState.characterBase[i].Tag).skinningData;
			for (int j = 0; j < skinningData.InverseBindPose.Count; j++)
			{
				skinningData.InverseBindPose[j] = skinningData.InverseBindPose[j] * matrix;
			}
			for (int k = 0; k < PlayerBaseState.characterBase[i].Meshes.Count; k++)
			{
				drawMesh = PlayerBaseState.characterBase[i].Meshes[k];
				for (int l = 0; l < drawMesh.MeshParts.Count; l++)
				{
					drawMeshPart = drawMesh.MeshParts[l];
					characterEffects[i * 7 + k] = new SkinnedEffectParams(drawMeshPart.Effect);
				}
			}
		}
		List<FPSCharacterData> list2 = EndGameEngine.GameAssetMgr.Load<List<FPSCharacterData>>("data\\FPSCharacterDataXml");
		PlayerBaseState.fpsHandsBase = new Model[list2.Count];
		for (int m = 0; m < list2.Count; m++)
		{
			PlayerBaseState.fpsHandsBase[m] = EndGameEngine.GameAssetMgr.Load<Model>("models\\characters\\" + list2[m].Name);
			for (int n = 0; n < PlayerBaseState.fpsHandsBase[m].Meshes.Count; n++)
			{
				ModelMesh modelMesh = PlayerBaseState.fpsHandsBase[m].Meshes[n];
				for (int num = 0; num < modelMesh.MeshParts.Count; num++)
				{
					ModelMeshPart modelMeshPart = modelMesh.MeshParts[num];
					modelMeshPart.Tag = new WeaponEffectParams(modelMeshPart.Effect, null);
				}
			}
		}
	}

	public SkinningData GetSkinningData()
	{
		if (character == null)
		{
			character = PlayerBaseState.characterBase[0];
		}
		return character.Tag as SkinningData;
	}

	public void SetViewport(bool coop, int index)
	{
		vpViewPort = EndGameEngine.DefualtViewport;
		if (!coop)
		{
			vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
			vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
		}
		if (coop)
		{
			vpViewPort.Width = 512;
			vpViewPort.Height = 256;
			if (index == 0)
			{
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
			}
			else
			{
				vpViewPort.X = 0;
				vpViewPort.Y = 256;
			}
		}
		AspectRatio = (float)vpViewPort.Width / (float)vpViewPort.Height;
		for (int i = 0; i < 32; i++)
		{
			losOtherPlayers[i].player = null;
		}
		playerTag.Set(base.gamerTag);
	}

	public void LoadPlayerStatistics()
	{
		string text = EndGameEngine.GameSettings.GameName + "Stats";
		if (LevelBaseMenu.gameMode == GameMode.XboxLive)
		{
			text += "Live";
			if (OneOffReadPlayerStatsLive)
			{
				OneOffLeaderboardInsert = true;
				OneOffReadPlayerStatsLocal = true;
				OneOffReadPlayerStatsLive = false;
			}
		}
		else
		{
			text += "Local";
			if (OneOffReadPlayerStatsLocal)
			{
				OneOffLeaderboardInsert = true;
				OneOffReadPlayerStatsLive = true;
				OneOffReadPlayerStatsLocal = false;
			}
		}
		if (OneOffLeaderboardInsert && LevelBaseMenu.gameMode == GameMode.XboxLive)
		{
			OneOffLeaderboardInsert = false;
		}
	}

	public void SavePlayerStatistics()
	{
		string text = EndGameEngine.GameSettings.GameName + "Stats";
		if (LevelBaseMenu.gameMode == GameMode.XboxLive && !OneOffReadPlayerStatsLive)
		{
			text += "Live";
		}
		else if (!OneOffReadPlayerStatsLocal)
		{
			text += "Local";
		}
	}

	public virtual void LoadContent(int index)
	{
		if (ContentInitialized)
		{
			return;
		}
		ContentInitialized = true;
		for (int i = 0; i < 2; i++)
		{
			Render3rdPerson[i] = false;
			RenderRagdoll[i] = false;
			bFrustum[i] = new BoundingFrustum(Matrix.Identity);
		}
		isSighted[0] = false;
		isSighted[1] = false;
		ref Matrix reference = ref matFlashLight[0];
		reference = Matrix.Identity;
		ref Matrix reference2 = ref matFlashLight[1];
		reference2 = Matrix.Identity;
		for (int j = 0; j < 2; j++)
		{
			mDataQueue[j] = new DataQueue();
			ref Matrix reference3 = ref matProjection[j];
			reference3 = Matrix.Identity;
			mDataQueue[j].lightView2 = new Matrix[2];
			mDataQueue[j].lightProj2 = new Matrix[2];
		}
		ref Vector3 reference4 = ref vecNetworkPositions[0];
		reference4 = Vector3.Zero;
		ref Vector3 reference5 = ref vecNetworkPositions[1];
		reference5 = Vector3.Zero;
		ref Vector3 reference6 = ref vecNetworkPositions[2];
		reference6 = Vector3.Zero;
		ref Vector3 reference7 = ref vecNetworkPositions[3];
		reference7 = Vector3.Zero;
		for (int k = 0; k < PlayerBaseState.fpsHandsBase.Length; k++)
		{
			for (int l = 0; l < PlayerBaseState.fpsHandsBase[k].Meshes.Count; l++)
			{
				ModelMesh modelMesh = PlayerBaseState.fpsHandsBase[k].Meshes[l];
				for (int m = 0; m < modelMesh.MeshParts.Count; m++)
				{
					ModelMeshPart modelMeshPart = modelMesh.MeshParts[m];
					((WeaponEffectParams)modelMeshPart.Tag).SetConstants();
				}
			}
		}
		fpsWeapon.LoadContent(index);
		fpsWeapon.Owner = this;
		fpsWeapon.fpsAmin.Owner = this;
		if (index == 0)
		{
			fpsWeapon.SetLocalFPSAnimationKeys();
		}
		fireSND0 = EndGameEngine.SoundBnk.GetCue(fpsWeapon.CurrentWeapon.WeaponShotSound0);
		whizSND0 = EndGameEngine.SoundBnk.GetCue(BulletWhizSounds[0]);
		for (int n = 0; n < 6; n++)
		{
			MuzzleSoundDelay[n] = -1f;
			MuzzleDistanceDelay[n] = -1f;
		}
		MeleeSwishSound = EndGameEngine.SoundBnk.GetCue("DoubleArmSwish00");
		if (!TexturesInit)
		{
			SurvivorShirtDiffuse1 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorShirts1");
			SurvivorShirtDiffuse2 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorShirts2");
			SurvivorShirtNormal1 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorShirts1_norm");
			SurvivorShirtNormal2 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorShirts2_norm");
			SurvivorPantsDiffuse1 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorPants1");
			SurvivorPantsDiffuse2 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorPants2");
			SurvivorPantsNormal1 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorPants1_norm");
			SurvivorPantsNormal2 = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\characters\\SurvivorPants2_norm");
		}
		CharacterIndex = 1;
		currentCharacterIndex = CharacterIndex;
		character = PlayerBaseState.characterBase[CharacterIndex];
		cPlayer.Initialize(character, 0);
		cPlayer.Owner = this;
		cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
		SetCharacter(1, 3f, 3f);
		physics = new BotPhysics(EndGameEngine.GameAssetMgr.Load<Model>("models\\characters\\char_physics"));
		mRagdoll.SetSkinData(PlayerBaseState.characterBase[0], 0.7f);
		SetupReticle();
		RespawnTimer = RESPAWN_TIME;
		if (EndGameEngine.GameSettings.GameName == "_AvR_")
		{
			UIHalographic = new WeaponHalographicUI();
			UIHalographic.Load("models\\weapons\\UIHalographic");
		}
		if (EndGameEngine.GameSettings.GameName == "ApocalypseZ")
		{
			CoOpOffset = new Vector3(0f, -58f, 0f);
		}
		for (int num = 0; num < 16; num++)
		{
			PlayerLineOfSight[num] = 0;
		}
	}

	public virtual bool SpawnPlayer()
	{
		if (!Spawned)
		{
			_ = RespawnTimer;
			_ = RESPAWN_TIME;
		}
		return true;
	}

	public virtual void SpawnRequest()
	{
		SpawnRequested = true;
		Spawn();
	}

	public virtual void SpawnRandom()
	{
		SpawnPoints.GetRandomSpawnPoint(SpawnPointType.Deathmatch, ref SpawnPosition, ref SpawnDirection);
		Spawn();
	}

	public virtual void Spawn(ref Vector3 p, ref Vector3 d)
	{
		SpawnPosition = p;
		SpawnDirection = d;
		Spawn();
	}

	public virtual void Spawn()
	{
		if (LevelBaseMenu.LoadPlayerDataScheduled || Spawned || !SpawnRequested || RespawnTimer < RESPAWN_TIME)
		{
			return;
		}
		if (EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ"))
		{
			AIBase.DispHelpInfo = 16f;
			AIBase.BlackFadeTimer = 4f;
			VehicleCls.VehicleMenuOpen = false;
			SpawnDirection = Vector3.UnitZ;
			IsAttached0 = false;
			FlashLightOn = false;
			flashLightDir = Vector3.UnitX;
			flashLightPos = Vector3.Zero;
			OverrideInput = false;
			OverrideCamera = false;
			OverrideButtonX = false;
			OverrideButtonTriggerRight = false;
			OverrideButtonLeftShoulder = false;
			OverridePosition = false;
			OverrideProjection = false;
			CurrentViewDis = TmpViewDis;
			NetworkUpdateFrameCount = -1;
			if (Guide.IsTrialMode || LevelBaseMenu.isTrialMode)
			{
				SpawnPosition = new Vector3(43840f, 1600f, -16820f);
				SpawnPosition.X += (float)(EndGameEngine.randGenerator.NextDouble() - 0.5) * 600f;
				SpawnPosition.Z += (float)(EndGameEngine.randGenerator.NextDouble() - 0.5) * 600f;
			}
			else
			{
				switch (EndGameEngine.randGenerator.Next(0, 3))
				{
				case 0:
					SpawnPosition = new Vector3(49200f, 20000f, -21800f) - new Vector3(46600f, 20000f, -1700f);
					SpawnPosition = new Vector3(46600f, 20000f, -1700f) + SpawnPosition * (float)EndGameEngine.randGenerator.NextDouble();
					break;
				case 1:
					SpawnPosition = new Vector3(47400f, 20000f, -40300f) - new Vector3(54750f, 20000f, -41800f);
					SpawnPosition = new Vector3(54750f, 20000f, -41800f) + SpawnPosition * (float)EndGameEngine.randGenerator.NextDouble();
					break;
				default:
					SpawnPosition = new Vector3(37850f, 20000f, -52800f) - new Vector3(41650f, 20000f, -42730f);
					SpawnPosition = new Vector3(41650f, 20000f, -42730f) + SpawnPosition * (float)EndGameEngine.randGenerator.NextDouble();
					break;
				}
			}
			SpawnPosition.Y = HeightMapPhysics.GetHeight(ref SpawnPosition);
			SpawnDirection = SpawnPosition * -1f;
			SpawnDirection.Normalize();
			EndGameEngine.SoundBnk.GetCue("SpawnInSound").Play();
		}
		vecPosition = SpawnPosition;
		vecCharacterDir = SpawnDirection;
		vecDirection = SpawnDirection;
		FadeToBlack = 1f;
		OverrideButtonX = false;
		OverrideButtonTriggerRight = false;
		OverridePosition = false;
		vecRight = Vector3.Cross(vecDirection, Vector3.UnitY);
		vecUp = Vector3.UnitY;
		vecPosition.Y += 18f;
		if (SpawnSetAngles)
		{
			Angles.X = MathHelper.ToDegrees((float)math.AngleBetweenVectors(Vector3.UnitZ, vecDirection));
			if (Vector3.Dot(vecDirection, Vector3.UnitZ) < 0f)
			{
				Angles.X *= -1f;
			}
		}
		Angles.Y = 0f;
		Angles.Z = 0f;
		AngleTorsoCharacter = 0f;
		vecPosition.Y += 50f;
		Health = 100f;
		GravityAccel = 0f;
		MenuSelected = 0;
		MenuState = PlayerMenuState.InGame;
		if (EndGameEngine.GameSettings.GameName.Contains("TowerDefense") || EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
		{
			NumberFragGrenades = 0;
			NumberNaderGrenades = 0;
			NumberSmokeGrenades = 0;
			NumberThrowingKnife = 0;
		}
		else if (EndGameEngine.GameSettings.GameName.Contains("EndOfDays_Survivor"))
		{
			fpsWeapon.ResetSpawn();
			NumberFragGrenades = 1;
			NumberNaderGrenades = 0;
			NumberSmokeGrenades = 0;
			NumberThrowingKnife = 0;
			for (int i = 0; i < FPSWeaponBase.weapon.Count; i++)
			{
				if (FPSWeaponBase.weapon[i].WepType == WeaponType.FiftyCal || FPSWeaponBase.weapon[i].WepType == WeaponType.NineMil)
				{
					FPSWeaponBase.weapon[i].BulletsTotal = 90;
				}
				else
				{
					FPSWeaponBase.weapon[i].BulletsTotal = 50;
				}
			}
		}
		else if (ApocalypseZ_Hack)
		{
			AIBase.LoadWorldItemAssets();
			float num = MathHelper.ToDegrees(MyMath.AngleBetweenVectors(SpawnDirection, Vector3.UnitZ));
			if (SpawnDirection.X < 0f)
			{
				num = 360f - num;
			}
			Angles.X = num;
			Angles.Y = 0f;
			Angles.Z = 0f;
			NumberFragGrenades = 0;
			NumberNaderGrenades = 0;
			NumberSmokeGrenades = 0;
			NumberThrowingKnife = 0;
			fpsWeapon.SetWeapon(WeaponType.EmptyHands);
		}
		else
		{
			fpsWeapon.ResetSpawn();
			NumberFragGrenades = 1;
			NumberNaderGrenades = 1;
			NumberThrowingKnife = 1;
			if (SmokeGrenadesUnlocked)
			{
				NumberSmokeGrenades = 1;
			}
			else
			{
				NumberSmokeGrenades = 0;
			}
		}
		CurrentDay = 0;
		FoodLevel = 100f;
		WaterLevel = 100f;
		BloodLevel = 100f;
		BloodLoss = 0f;
		for (int j = 0; j < 8; j++)
		{
			AttackingBot_UID[j] = 0;
		}
		SpawnOverRideTimer = 0f;
		if (ApocalypseZ_Hack)
		{
			AIStateMachine.SetAttackPlayerEnable(e: true);
		}
		else
		{
			AIStateMachine.SetAttackPlayerEnable(e: false);
		}
		triggerFlags = TriggerFlags.AISafeHouse;
		Sighted = false;
		isSighted[0] = false;
		isSighted[1] = false;
		SpecialOperations();
		saveStatusTimer = 25f;
		SpawnRequested = false;
		saveStatusTimer = 25f;
		Storage.LoadPlayerInfo();
		if (Storage.LoadCharacterStats())
		{
			Storage.NewLoadInventory();
			cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
		}
		else
		{
			fpsWeapon.SetWeapon(WeaponType.EmptyHands);
			InventoryItemCls inventoryItemCls = new InventoryItemCls();
			inventoryItemCls.item = new ItemCls();
			WeaponType weaponType = WeaponType.Hatchet;
			for (int k = 0; k < WeaponsCls.availableWeapons.Length; k++)
			{
				if (WeaponsCls.availableWeapons[k] == weaponType)
				{
					((ItemCls)inventoryItemCls.item).desc = (ushort)k;
					break;
				}
			}
			((ItemCls)inventoryItemCls.item).desc |= 512;
			((ItemCls)inventoryItemCls.item).uid = WorldItemsCls.UniqueId;
			((ItemCls)inventoryItemCls.item).pos = Vector3.Zero;
			inventoryItemCls.desc = ((ItemCls)inventoryItemCls.item).desc;
			AIBase.PlayerInventory.AddItem(InventorySlot.Pockets, inventoryItemCls);
			InventoryItemCls inventoryItemCls2 = new InventoryItemCls();
			inventoryItemCls2.item = new ItemCls();
			((ItemCls)inventoryItemCls2.item).desc = 7;
			((ItemCls)inventoryItemCls2.item).desc |= 1024;
			((ItemCls)inventoryItemCls2.item).uid = WorldItemsCls.UniqueId;
			((ItemCls)inventoryItemCls2.item).pos = Vector3.Zero;
			inventoryItemCls2.desc = ((ItemCls)inventoryItemCls2.item).desc;
			AIBase.PlayerInventory.AddItem(InventorySlot.Pockets, inventoryItemCls2);
			Storage.NewSaveInventory();
		}
		mDataQueue[0].world = tmpPlayerScale;
		mDataQueue[1].world = tmpPlayerScale;
		mDataQueue[0].world.Translation = vecPosition;
		mDataQueue[1].world.Translation = vecPosition;
		if (EGENetWorkNext.networkSession != null)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)133);
			if (EGENetWorkNext.networkSession.IsHost)
			{
				packetWriter.Write(NetGamerRef.Id);
				packetWriter.Write(vecPosition);
				packetWriter.Write(vecDirection);
				packetWriter.Write(Angles);
				packetWriter.Write(BloodLevel);
				packetWriter.Write(BloodLoss);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
			}
			else
			{
				packetWriter.Write(vecPosition);
				packetWriter.Write(vecDirection);
				packetWriter.Write(Angles);
				packetWriter.Write(BloodLevel);
				packetWriter.Write(BloodLoss);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder, EGENetWorkNext.networkSession.Host);
			}
		}
		if (!Guide.IsTrialMode)
		{
			if (!LevelBaseMenu.isLocalMode)
			{
				EndGameEngine.UpdatePresence(GamerPresenceMode.Multiplayer);
			}
			else
			{
				EndGameEngine.UpdatePresence(GamerPresenceMode.SinglePlayer);
			}
		}
		else
		{
			EndGameEngine.UpdatePresence(GamerPresenceMode.SinglePlayer);
		}
		Spawned = true;
		SpawnRequested = false;
		PlayerFlags |= FPS_NET_FLAGS.Spawned;
	}

	public void SetViewPortTestCoOp(int width, int height, int qIndex)
	{
		UVDisplacement.X = 1f;
		UVDisplacement.Y = 1f;
		UVDisplacement.Z = 0f;
		UVDisplacement.W = 0f;
		vpViewPort.X = 0;
		vpViewPort.Y = 0;
		vpViewPort.Width = width;
		vpViewPort.Height = height;
		AspectRatio = (float)vpViewPort.Width / (float)vpViewPort.Height;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = vpViewPort;
	}

	public void SetViewPortTestCoOp(RenderPass pass, int qIndex)
	{
		if (IsSplitScreen)
		{
			switch (pass)
			{
			case RenderPass.GBufferPass:
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
				if (playerIndex == EndGameEngine.controllingPlayer)
				{
					vpViewPort.X = EndGameEngine.GameSettings.GBufferSizeX / 8;
					vpViewPort.Y = 2;
					vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX - EndGameEngine.GameSettings.GBufferSizeX / 3;
					vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY / 2 - 4;
				}
				if (playerIndex == EndGameEngine.guestPlayer)
				{
					vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX - EndGameEngine.GameSettings.GBufferSizeX / 3;
					vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY / 2 - 4;
					vpViewPort.X = EndGameEngine.GameSettings.GBufferSizeX - (vpViewPort.Width + EndGameEngine.GameSettings.GBufferSizeX / 8);
					vpViewPort.Y = EndGameEngine.GameSettings.GBufferSizeY / 2 + 2;
				}
				if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
				{
					vpViewPort.X = (vpViewPort.Width - vpViewPort.Width / 2) / 2;
					vpViewPort.Y = (vpViewPort.Height - vpViewPort.Height / 2) / 2;
					vpViewPort.Width /= 2;
					vpViewPort.Height /= 2;
				}
				break;
			case RenderPass.ForwardPass:
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
				if (playerIndex == EndGameEngine.controllingPlayer)
				{
					vpViewPort.X = EndGameEngine.GameSettings.RenderTargetSizeX / 8;
					vpViewPort.Y = 2;
					vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX - EndGameEngine.GameSettings.RenderTargetSizeX / 3;
					vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY / 2 - 4;
				}
				if (playerIndex == EndGameEngine.guestPlayer)
				{
					vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX - EndGameEngine.GameSettings.RenderTargetSizeX / 3;
					vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY / 2 - 4;
					vpViewPort.X = EndGameEngine.GameSettings.RenderTargetSizeX - (vpViewPort.Width + EndGameEngine.GameSettings.RenderTargetSizeX / 8);
					vpViewPort.Y = EndGameEngine.GameSettings.RenderTargetSizeY / 2 + 2;
				}
				if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
				{
					UVDisplacement.X = 0.5f;
					UVDisplacement.Y = 0.5f;
					UVDisplacement.Z = 0.25f;
					UVDisplacement.W = 0.25f;
				}
				break;
			}
		}
		else
		{
			switch (pass)
			{
			case RenderPass.GBufferPass:
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
				vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX;
				vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY;
				if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
				{
					vpViewPort.X = (vpViewPort.Width - vpViewPort.Width / 2) / 2;
					vpViewPort.Y = (vpViewPort.Height - vpViewPort.Height / 2) / 2;
					vpViewPort.Width /= 2;
					vpViewPort.Height /= 2;
				}
				break;
			case RenderPass.ForwardPass:
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
				vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
				vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
				if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
				{
					UVDisplacement.X = 0.5f;
					UVDisplacement.Y = 0.5f;
					UVDisplacement.Z = 0.25f;
					UVDisplacement.W = 0.25f;
				}
				break;
			}
		}
		AspectRatio = (float)vpViewPort.Width / (float)vpViewPort.Height;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = vpViewPort;
	}

	public void SetViewPortForPass(RenderPass pass, int qIndex)
	{
		switch (pass)
		{
		case RenderPass.GBufferPass:
			UVDisplacement.X = 1f;
			UVDisplacement.Y = 1f;
			UVDisplacement.Z = 0f;
			UVDisplacement.W = 0f;
			if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
			{
				vpViewPort.X = (EndGameEngine.GameSettings.GBufferSizeX - EndGameEngine.GameSettings.GBufferSizeX / 2) / 2;
				vpViewPort.Y = (EndGameEngine.GameSettings.GBufferSizeY - EndGameEngine.GameSettings.GBufferSizeY / 2) / 2;
				vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX / 2;
				vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY / 2;
			}
			else
			{
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
				vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX;
				vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY;
			}
			break;
		case RenderPass.CompositePass:
			vpViewPort.X = 0;
			vpViewPort.Y = 0;
			vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX;
			vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY;
			if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
			{
				UVDisplacement.X = 0.5f;
				UVDisplacement.Y = 0.5f;
				UVDisplacement.Z = 0.25f;
				UVDisplacement.W = 0.25f;
			}
			else
			{
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
			}
			break;
		case RenderPass.ForwardPass:
			if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
			{
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
				vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
				vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
				UVDisplacement.X = 0.5f;
				UVDisplacement.Y = 0.5f;
				UVDisplacement.Z = 0.25f;
				UVDisplacement.W = 0.25f;
			}
			else
			{
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
				vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
				vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
			}
			break;
		case RenderPass.FPSDrawPass:
			UVDisplacement.X = 1f;
			UVDisplacement.Y = 1f;
			UVDisplacement.Z = 0f;
			UVDisplacement.W = 0f;
			vpViewPort.X = 0;
			vpViewPort.Y = 0;
			vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX;
			vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY;
			break;
		case RenderPass.PostScopePass:
			vpViewPort.X = 0;
			vpViewPort.Y = 0;
			vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX;
			vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY;
			if (isSighted[qIndex] && fpsWeapon.CurrentWeapon.Attachment == WeaponAttachment.SniperScope)
			{
				UVDisplacement.X = 0.5f;
				UVDisplacement.Y = 0.5f;
				UVDisplacement.Z = 0.25f;
				UVDisplacement.W = 0.25f;
			}
			else
			{
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
			}
			break;
		case RenderPass.MenuGBufferPass:
			UVDisplacement.X = 1f;
			UVDisplacement.Y = 1f;
			UVDisplacement.Z = 0f;
			UVDisplacement.W = 0f;
			vpViewPort.X = 0;
			vpViewPort.Y = 0;
			vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX;
			vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY;
			break;
		default:
			switch (pass)
			{
			case RenderPass.MenuGBufferPass:
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
				vpViewPort.Width = EndGameEngine.GameSettings.GBufferSizeX;
				vpViewPort.Height = EndGameEngine.GameSettings.GBufferSizeY;
				break;
			case RenderPass.MenuForwardPass:
				UVDisplacement.X = 1f;
				UVDisplacement.Y = 1f;
				UVDisplacement.Z = 0f;
				UVDisplacement.W = 0f;
				vpViewPort.X = 0;
				vpViewPort.Y = 0;
				vpViewPort.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
				vpViewPort.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
				break;
			}
			break;
		}
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = vpViewPort;
	}

	public void WeaponFired()
	{
		ZombieAlertScalar = 1f;
		triggerFlags &= (TriggerFlags)(-17);
		AIStateMachine.SetAttackPlayerEnable(e: true);
		DrawMuzzleFlashAlpha = 1f;
		ShotFired = true;
	}

	public WeaponClass GetPrimaryWeapon()
	{
		return fpsWeapon.GetWeaponReference(PrimaryWeapon);
	}

	public WeaponClass GetSecondaryWeapon()
	{
		return fpsWeapon.GetWeaponReference(SecondaryWeapon);
	}

	public void SetPrimaryWeapon()
	{
		if (fpsWeapon.CurWeaponType != PrimaryWeapon)
		{
			fpsWeapon.SetWeapon(PrimaryWeapon);
			cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
		}
	}

	public void SetPrimaryWeapon(string e)
	{
		for (int i = 0; i < 44; i++)
		{
			if (e == ((WeaponType)i).ToString())
			{
				SetPrimaryWeapon((WeaponType)i);
				cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
				break;
			}
		}
	}

	public void SetPrimaryWeapon(WeaponType e)
	{
		fpsWeapon.SetWeapon(e);
		PrimaryWeapon = e;
		cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
	}

	public void SetSecondaryWeapon()
	{
		fpsWeapon.SetWeapon(SecondaryWeapon);
	}

	public void SetSecondaryWeapon(string e)
	{
		for (int i = 0; i < 44; i++)
		{
			if (e == ((WeaponType)i).ToString())
			{
				SetSecondaryWeapon((WeaponType)i);
				break;
			}
		}
	}

	public void SetSecondaryWeapon(WeaponType e)
	{
		fpsWeapon.SetWeapon(e);
		SecondaryWeapon = e;
		cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
	}

	public void Set3rdPersonSwitchWeapon(WeaponType e)
	{
		cPlayer.PlayMergedAnimation(WeaponAnim.CoOpSwap);
	}

	public void Set3rdPersonBaseanim(WeaponAnim e)
	{
		cPlayer.SetBaseAnimation(e);
	}

	public void UpdateLineOfSight(int qIndex, float eTime)
	{
	}

	public bool RespawnTimeActive()
	{
		if (DeathTimer <= 0f)
		{
			return RespawnTimer < RESPAWN_TIME;
		}
		return false;
	}

	public bool inRespawn()
	{
		return ToggledRespawn;
	}

	public void SetNetworkPlayer(NetworkGamer gamer)
	{
		gamer.Tag = this;
		base.gamerTag = gamer.Gamertag;
		IsHost = gamer.IsHost;
		NetGamerId = gamer.Id;
		NetworkUpdateTimer = (float)EndGameEngine.randGenerator.NextDouble();
		Health = 100f;
		vecPosition = Vector3.Zero;
		Spawned = true;
		IsAttached0 = false;
	}

	public virtual void Update(GameTime gameTime, int qIndex)
	{
		float fFIXED_TIME_STEP = EndGameEngine.fFIXED_TIME_STEP;
		SpawnOverRideTimer += 0.015f;
		MatchCoolDownTimer -= 0.0075f;
		DeathTimer -= fFIXED_TIME_STEP;
		if (DeathTimer <= 0f && ToggledRespawn && !AvRStartMessage)
		{
			RespawnTimer += fFIXED_TIME_STEP;
		}
		float yaw = 0f;
		float pitch = 0f;
		float num = 180f;
		ZombieAlertScalar -= fFIXED_TIME_STEP * 0.05f;
		ZombieAlertScalar = ((ZombieAlertScalar < 0f) ? 0f : ZombieAlertScalar);
		tmpCommandoSpeed = CommandoSpeed;
		tmpRunSpeed = RunSpeed;
		tmpRunEndurance = RunEndurance;
		tmpWeaponAccuracey = WeaponAccuracey;
		if (AvR_Hack)
		{
			tmpCommandoSpeed = 0f;
			tmpRunSpeed = 0.7f;
			tmpRunEndurance = 2f;
			tmpWeaponAccuracey = 1f;
		}
		if (EoDSurvival_Hack)
		{
			if (currentWeaponType == WeaponType.LightMachineGun)
			{
				tmpCommandoSpeed = 0.4f;
				tmpRunSpeed = 0.4f;
			}
			else if (currentWeaponType == WeaponType.NewTech || currentWeaponType == WeaponType.European || currentWeaponType == WeaponType.USA)
			{
				tmpCommandoSpeed = 0.7f;
				tmpRunSpeed = 0.6f;
			}
			else
			{
				tmpCommandoSpeed = 1f;
				tmpRunSpeed = 0.8f;
			}
			tmpRunEndurance = 2f;
			tmpWeaponAccuracey = 0.8f;
		}
		if (ApocalypseZ_Hack)
		{
			tmpCommandoSpeed = 1f;
			tmpRunSpeed = 1f;
			tmpRunEndurance = 2f;
			tmpWeaponAccuracey = 0.8f;
		}
		Speed = 0f;
		SideStep = 0f;
		PlayerFlags = (Spawned ? (PlayerFlags | FPS_NET_FLAGS.Spawned) : (PlayerFlags & (FPS_NET_FLAGS)(-2)));
		if (LevelBaseMenu.LoadState != LevelLoadState.Loaded)
		{
			return;
		}
		if (EGENetWorkNext.networkSession != null)
		{
			NetworkUpdateFrameCount--;
			if (NetworkUpdateFrameCount < 0)
			{
				CurrentBacpPack = AIBase.PlayerInventory.GetCurrentBackPack;
				NetworkUpdateFrameCount = 900;
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)135);
				packetWriter.Write(EGENetWorkNext.networkSession.LocalGamers[0].Id);
				packetWriter.Write(CharacterIndex);
				packetWriter.Write((byte)ShirtIndex);
				packetWriter.Write((byte)PantstIndex);
				packetWriter.Write((byte)CurrentBacpPack);
				packetWriter.Write((byte)(FlashLightOn ? 1u : 0u));
				packetWriter.Write((byte)CurrentDay);
			}
		}
		if (ByPassFPS)
		{
			if (!Guide.IsVisible)
			{
				if (OverrideProjection)
				{
					Zoom = MathHelper.SmoothStep(Zoom, ZoomOverride, fFIXED_TIME_STEP * 20f);
				}
				else
				{
					Zoom = MathHelper.SmoothStep(Zoom, fpsWeapon.FOV + (float)Math.PI / 12f, fFIXED_TIME_STEP * 20f);
				}
				ref Matrix reference = ref matProjection[qIndex];
				reference = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, FarZPlane);
				ref Matrix reference2 = ref matSkyDomeProjection[qIndex];
				reference2 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, 1000000f);
				bFrustum[qIndex].Matrix = mDataQueue[qIndex].view * mDataQueue[qIndex].projection;
				lastGamePadState = InputBase.playerGamePad[(int)playerIndex].lastGamePadState;
				currentGamePadState = InputBase.playerGamePad[(int)playerIndex].currentGamePadState;
				bool flag = currentGamePadState.IsButtonDown(Buttons.Start) && !lastGamePadState.IsButtonDown(Buttons.Start);
				bool flag2 = currentGamePadState.IsButtonDown(Buttons.Back) && !lastGamePadState.IsButtonDown(Buttons.Back);
				if (flag || flag2)
				{
					FPSGameMenu.SetVisable(flag2);
				}
				UpdateCameraAndQueue(fFIXED_TIME_STEP, qIndex);
			}
			return;
		}
		float value = 0f;
		float value2 = 0f;
		if (fpsWeapon.CurrentWeapon.AttachmentTwo == WeaponAttachment.NadeLauncher && (fpsWeapon.CurrentWeapon.NaderToggled || fpsWeapon.HackReloadBlendoutReached || fpsWeapon.CurrentFPSAnimation.AnimType == AnimationType.Idle || fpsWeapon.CurrentFPSAnimation.AnimType == AnimationType.Walk || fpsWeapon.CurrentFPSAnimation.AnimType == AnimationType.Run))
		{
			value = 5f;
			value2 = 9f;
		}
		LeftArmRotationY = MathHelper.Lerp(LeftArmRotationY, value, fFIXED_TIME_STEP * 10f);
		LeftArmRotationX = MathHelper.Lerp(LeftArmRotationX, value2, fFIXED_TIME_STEP * 10f);
		if (!Spawned && DeathTimer > 0f)
		{
			SetRagdoll(gameTime);
			Matrix identity = Matrix.Identity;
			identity = mRagdoll.RagdollWorldPose[13];
			vecDirection = identity.Translation - (vecPosition + Vector3.UnitY * 16f);
			vecDirection *= 100f;
			if (vecDirection.LengthSquared() > 4f)
			{
				vecDirection.Normalize();
			}
			else
			{
				vecDirection = Vector3.UnitZ;
			}
			if (ThirdPersonCamera)
			{
				tmpRunEndurance = 8f;
				ref Vector3 reference3 = ref vecHeadPosition[qIndex];
				reference3 = vecPosition;
				vecHeadPosition[qIndex].Y += 120f;
				vecHeadPosition[qIndex] += vecDirection * 500f;
				vecHeadPosition[qIndex].X = 0f;
				vecHeadPosition[qIndex].Z = 0f;
			}
			else
			{
				vecHeadPosition[qIndex].X = 0f;
				vecHeadPosition[qIndex].Z = 0f;
				vecHeadPosition[qIndex].Y = vecPosition.Y;
			}
			matView = Matrix.CreateLookAt(vecHeadPosition[qIndex], vecHeadPosition[qIndex] + vecDirection * 1000f, Vector3.UnitY);
			mDataQueue[qIndex].view = matView;
			bFrustum[qIndex].Matrix = mDataQueue[qIndex].view * mDataQueue[qIndex].projection;
			return;
		}
		if (!Spawned)
		{
			if (RespawnTimer >= RESPAWN_TIME)
			{
				SpawnRequest();
			}
			if (!Spawned)
			{
				lastGamePadState = InputBase.playerGamePad[(int)playerIndex].lastGamePadState;
				currentGamePadState = InputBase.playerGamePad[(int)playerIndex].currentGamePadState;
				if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
				{
					vecPosition.X = 1564f;
					vecPosition.Y = 172f;
					vecPosition.Z = -1890f;
					vecDirection.X = -0.7006f;
					vecDirection.Y = -0.0693f;
					vecDirection.Z = 0.71f;
					CharacterYaw = 0f;
					Angles.X = -43.6881f;
					Angles.Y = 0f;
					Angles.Z = 0f;
					TriggerDown = false;
					Sighted = false;
					isSighted[qIndex] = Sighted;
					fpsWeapon.Update(gameTime, qIndex, this);
					vecCameraUp = Vector3.Lerp(vecCameraUp, vecCameraUpSway, fFIXED_TIME_STEP * 20f);
					vecCameraUp.Normalize();
					Zoom = MathHelper.SmoothStep(Zoom, (float)Math.PI / 3f, fFIXED_TIME_STEP * 20f);
					ref Matrix reference4 = ref matProjection[qIndex];
					reference4 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, FarZPlane);
					Matrix.CreateRotationY(MathHelper.ToRadians(Angles.X), out matYaw);
					Vector3.Transform(ref VecUnitZ, ref matYaw, out vecDirection);
					Vector3.Cross(ref VecUnitY, ref vecDirection, out vecRight);
					Matrix.CreateFromAxisAngle(ref vecRight, MathHelper.ToRadians(Angles.Y), out matPitch);
					Vector3.Transform(ref vecDirection, ref matPitch, out vecDirection);
					vecFlatDirection = vecDirection;
					vecFlatDirection.Y = 0f;
					vecFlatDirection.Normalize();
					vecDirection.Normalize();
					Vector3 vector = Vector3.Cross(-vecDirection, Vector3.UnitY);
					vector.Normalize();
					mDataQueue[qIndex].world = Matrix.Identity;
					mDataQueue[qIndex].world.Right = vector;
					mDataQueue[qIndex].world.Forward = -vecDirection;
					mDataQueue[qIndex].world.Up = Vector3.Cross(vector, -vecDirection);
					mDataQueue[qIndex].world *= tmpPlayerScale;
					mDataQueue[qIndex].world.Translation = vecPosition + new Vector3(0f, CrouchY, 0f);
					SpawnPosition = vecPosition;
					SpawnPosition.Y = 50f;
					SpawnDirection = vecDirection;
					SpawnSetAngles = false;
				}
				else
				{
					vecPosition = SpawnPosition;
					vecDirection = SpawnDirection;
					SpawnSetAngles = true;
					mDataQueue[qIndex].cameraPos = vecPosition;
					mDataQueue[qIndex].cameralookAt = vecDirection;
					mDataQueue[qIndex].cameraDirN = vecDirection;
				}
				mDataQueue[qIndex].view = Matrix.CreateLookAt(vecPosition, vecPosition + vecDirection * 1000f, Vector3.UnitY);
				bFrustum[qIndex].Matrix = mDataQueue[qIndex].view * mDataQueue[qIndex].projection;
				return;
			}
		}
		saveStatusTimer -= 0.03334f;
		if (saveStatusTimer < 0f)
		{
			saveStatusTimer = 10f;
			Storage.SavePlayerStatus();
		}
		if (RESPAWN_TIME + 3f - RespawnTimer > 0f)
		{
			Health = 100f;
		}
		currentWeaponType = fpsWeapon.CurWeaponType;
		if (PainPillTimer > 0f || (WaterLevel > 30f && BloodLevel > 50f && BloodLoss == 0f))
		{
			Endurance = 8f + tmpRunEndurance * 8f;
		}
		else
		{
			Endurance = 8f + tmpRunEndurance * 2f;
		}
		UpdateHealth(fFIXED_TIME_STEP);
		if (!Guide.IsVisible)
		{
			lastGamePadState = InputBase.playerGamePad[(int)playerIndex].lastGamePadState;
			currentGamePadState = InputBase.playerGamePad[(int)playerIndex].currentGamePadState;
			bool flag3 = currentGamePadState.IsButtonDown(Buttons.Start) && !lastGamePadState.IsButtonDown(Buttons.Start);
			bool flag4 = currentGamePadState.IsButtonDown(Buttons.Back) && !lastGamePadState.IsButtonDown(Buttons.Back);
			if (flag3 || flag4)
			{
				if (flag3 && !FPSGameMenu.isVisable)
				{
					if (InventoryCls.InventoryOpen)
					{
						AIBase.PlayerInventory.CloseInventory();
					}
					InventoryCls.InventoryOpen = false;
				}
				FPSGameMenu.SetVisable(flag4);
			}
		}
		if (FPSGameMenu.isVisable || Guide.IsVisible)
		{
			SetRagdoll(gameTime);
			ref Matrix reference5 = ref matProjection[qIndex];
			reference5 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, FarZPlane);
			ref Matrix reference6 = ref matSkyDomeProjection[qIndex];
			reference6 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, 1000000f);
			bFrustum[qIndex].Matrix = mDataQueue[qIndex].view * mDataQueue[qIndex].projection;
			fpsWeapon.Update(gameTime, qIndex, this);
			UpdateCameraAndQueue(fFIXED_TIME_STEP, qIndex);
			return;
		}
		if (ClearInput || LevelBaseMenu.IsPaused())
		{
			ClearInput = false;
			if (OverrideProjection)
			{
				Zoom = MathHelper.SmoothStep(Zoom, ZoomOverride, fFIXED_TIME_STEP * 20f);
			}
			else
			{
				Zoom = MathHelper.SmoothStep(Zoom, fpsWeapon.FOV + (float)Math.PI / 12f, fFIXED_TIME_STEP * 20f);
			}
			ref Matrix reference7 = ref matProjection[qIndex];
			reference7 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, FarZPlane);
			ref Matrix reference8 = ref matSkyDomeProjection[qIndex];
			reference8 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, 1000000f);
			bFrustum[qIndex].Matrix = mDataQueue[qIndex].view * mDataQueue[qIndex].projection;
			UpdateCameraAndQueue(fFIXED_TIME_STEP, qIndex);
			return;
		}
		if (InventoryCls.InventoryOpen || AIBase.IsVehicleMenuOpen())
		{
			OverrideInput = true;
		}
		UpdateInput(fFIXED_TIME_STEP, qIndex, ref yaw, ref pitch);
		float num2 = HeightMapPhysics.GetHeight(ref vecPosition) + 62f;
		if (num2 > vecPosition.Y)
		{
			num2 -= 2f;
			GravityAccel = 0f;
			vecPosition.Y = num2;
		}
		else
		{
			GravityAccel += 5f * EndGameEngine.fFIXED_TIME_STEP;
			vecPosition.Y -= GravityAccel;
		}
		if (ThirdPersonCamera)
		{
			bool flag5 = Sighted;
			if (currentGamePadState.IsButtonDown(Buttons.LeftTrigger))
			{
				Sighted = true;
				ThirdPersonCameraFire = 1f;
			}
			else if (currentGamePadState.IsButtonDown(Buttons.RightTrigger))
			{
				if (currentWeaponType == WeaponType.EmptyHands || currentWeaponType == WeaponType.Hatchet)
				{
					flag5 = false;
					ThirdPersonCameraFire = 0f;
				}
				else
				{
					flag5 = true;
					ThirdPersonCameraFire += 0.033f;
					RampThirdPersonCameraFireUp = true;
				}
			}
			else if (RampThirdPersonCameraFireUp)
			{
				ThirdPersonCameraFire += 0.033f;
				if (ThirdPersonCameraFire > 1f)
				{
					RampThirdPersonCameraFireUp = false;
				}
			}
			else if (currentWeaponType == WeaponType.Sniper)
			{
				ThirdPersonCameraFire = 0f;
			}
			else
			{
				ThirdPersonCameraFire -= 0.033f;
			}
			if (ThirdPersonCameraFire > 0f && currentWeaponType != WeaponType.EmptyHands && currentWeaponType != WeaponType.Hatchet)
			{
				Sighted = true;
			}
			ThirdPersonCameraFire = ((ThirdPersonCameraFire < 0f) ? 0f : ThirdPersonCameraFire);
			ThirdPersonCameraFire = ((ThirdPersonCameraFire > 1f) ? 1f : ThirdPersonCameraFire);
			CameraAngles.X -= yaw * 0.5f;
			CameraAngles.Y -= pitch;
			if (CameraAngles.X > 360f)
			{
				CameraAngles.X -= 360f;
			}
			else if (CameraAngles.X < 0f)
			{
				CameraAngles.X += 360f;
			}
			if (CameraAngles.Y > 80f)
			{
				CameraAngles.Y = 80f;
			}
			else if (CameraAngles.Y < -80f)
			{
				CameraAngles.Y = -80f;
			}
			if (flag5)
			{
				if (CameraAngles.Y < 0f)
				{
					Angles.Y = CameraAngles.Y * 1.4f - (float)ThirdPersonTorsoAngle;
				}
				else
				{
					Angles.Y = CameraAngles.Y * 1.4f - (float)ThirdPersonTorsoAngle;
				}
				if (Angles.Y > 80f)
				{
					Angles.Y = 80f;
				}
				else if (Angles.Y < -80f)
				{
					Angles.Y = -80f;
				}
			}
			else
			{
				Angles.Y = 0f;
			}
			Matrix.CreateRotationY(MathHelper.ToRadians(CameraAngles.X + fpsWeapon.RecoilUp), out matYaw);
			Vector3.Transform(ref VecUnitZ, ref matYaw, out CameraDirection);
			Vector3.Cross(ref VecUnitY, ref CameraDirection, out vecRight);
			Matrix.CreateFromAxisAngle(ref vecRight, MathHelper.ToRadians(CameraAngles.Y + fpsWeapon.RecoilSide), out matPitch);
			Vector3.Transform(ref CameraDirection, ref matPitch, out CameraDirection);
			if (flag5)
			{
				MoveY = InputLeftStick.Y;
				MoveX = InputLeftStick.X;
			}
			else
			{
				MoveY = Math.Abs(InputLeftStick.Y);
				MoveX = Math.Abs(InputLeftStick.X);
			}
			if (MoveX > 0.01f || MoveY > 0.01f)
			{
				CameraSet = false;
				float num3 = Angles.X;
				Angles.X = 0f;
				Vector3 zero = Vector3.Zero;
				zero.X = InputLeftStick.X;
				zero.Z = InputLeftStick.Y;
				zero.Normalize();
				float radians = MyMath.AngleBetweenVectors(zero, Vector3.UnitZ);
				float x = CameraAngles.X;
				if (flag5)
				{
					x = CameraAngles.X;
				}
				else
				{
					if (InputLeftStick.X < 0f)
					{
						Angles.X = MathHelper.ToDegrees(radians);
					}
					else
					{
						Angles.X = 180f + (180f - MathHelper.ToDegrees(radians));
					}
					x += Angles.X;
				}
				x = ((x > 360f) ? (x - 360f) : x);
				x = ((x < 0f) ? (x + 360f) : x);
				if (x > 180f)
				{
					if (num3 < x - 180f)
					{
						num3 = 360f + num3;
					}
				}
				else if (num3 > x + 180f)
				{
					num3 = 0f - (360f - num3);
				}
				if (flag5 && ThirdPersonCameraFire > 0.4f)
				{
					Angles.X = MathHelper.Lerp(num3, x, 0.15f);
				}
				else
				{
					Angles.X = MathHelper.Lerp(num3, x, 0.3f);
				}
				if (Angles.X > 360f)
				{
					Angles.X -= 360f;
				}
				else if (Angles.X < 0f)
				{
					Angles.X += 360f;
				}
			}
			else
			{
				if (flag5)
				{
					float num4 = Angles.X;
					Angles.X = 0f;
					float x2 = CameraAngles.X;
					x2 += Angles.X;
					x2 = ((x2 > 360f) ? (x2 - 360f) : x2);
					x2 = ((x2 < 0f) ? (x2 + 360f) : x2);
					if (x2 > 180f)
					{
						if (num4 < x2 - 180f)
						{
							num4 = 360f + num4;
						}
					}
					else if (num4 > x2 + 180f)
					{
						num4 = 0f - (360f - num4);
					}
					Angles.X = MathHelper.Lerp(num4, x2, 0.35f);
					if (Angles.X > 360f)
					{
						Angles.X -= 360f;
					}
					else if (Angles.X < 0f)
					{
						Angles.X += 360f;
					}
				}
				CameraSet = true;
			}
			if (flag5)
			{
				if (MoveY < 0f)
				{
					num *= 0.6f;
				}
				Speed = MoveY * fFIXED_TIME_STEP;
				SideStep = MoveX * fFIXED_TIME_STEP;
			}
			else
			{
				MoveY += MoveX;
				MoveX = 0f;
				Speed = MoveY * fFIXED_TIME_STEP;
				SideStep = 0f;
			}
		}
		else
		{
			if (MoveY < 0f)
			{
				num *= 0.6f;
			}
			Speed = MoveY * fFIXED_TIME_STEP;
			SideStep = MoveX * fFIXED_TIME_STEP;
		}
		TriggerDown = false;
		FireTimer -= fFIXED_TIME_STEP;
		if (!OverrideButtonTriggerRight && currentGamePadState.IsButtonDown(Buttons.RightTrigger))
		{
			bool flag6 = !lastGamePadState.IsButtonDown(Buttons.RightTrigger);
			if (currentWeaponType == WeaponType.EmptyHands)
			{
				if (flag6 && cPlayer.PlayMergedAnimation(WeaponAnim.CoOpRightPunch, EndGameEngine.FIXED_TIME_STEP + (int)(0.5f * (float)EndGameEngine.FIXED_TIME_STEP)))
				{
					tmpMergeAnim = WeaponAnim.CoOpRightPunch;
					cPlayer.PlayMergedAnimation(tmpMergeAnim);
					fpsWeapon.fpsAmin.PlayAnimation(WeaponAnim.RightPunch, force: false, EndGameEngine.FIXED_TIME_STEP + (int)(0.5f * (float)EndGameEngine.FIXED_TIME_STEP));
					ScheduleMeleeAttack();
					MeleeSwishSound.Dispose();
					MeleeSwishSound = EndGameEngine.SoundBnk.GetCue("DoubleArmSwish00");
					MeleeSwishSound.Play();
				}
			}
			else if (currentWeaponType == WeaponType.Hatchet)
			{
				if (flag6 && cPlayer.PlayMergedAnimation(WeaponAnim.CoOpAxeSwing, EndGameEngine.FIXED_TIME_STEP + (int)(0.5f * (float)EndGameEngine.FIXED_TIME_STEP)))
				{
					tmpMergeAnim = WeaponAnim.CoOpAxeSwing;
					cPlayer.PlayMergedAnimation(tmpMergeAnim);
					fpsWeapon.fpsAmin.PlayAnimation(WeaponAnim.FPSAxeSwing, force: false, EndGameEngine.FIXED_TIME_STEP + (int)(0.5f * (float)EndGameEngine.FIXED_TIME_STEP));
					ScheduleMeleeAttack();
					MeleeSwishSound.Dispose();
					MeleeSwishSound = EndGameEngine.SoundBnk.GetCue("AxeSwish00");
					MeleeSwishSound.Play();
				}
			}
			else
			{
				TriggerDown = true;
			}
		}
		if (MeleeAttackTimer > 0f)
		{
			MeleeAttackTimer -= fFIXED_TIME_STEP;
			if (MeleeAttackTimer <= 0f)
			{
				MeleeAttackFunc();
			}
		}
		if (Sighted && !OverrideInput)
		{
			if (currentGamePadState.IsButtonDown(Buttons.DPadUp) && !lastGamePadState.IsButtonDown(Buttons.DPadUp))
			{
				fpsWeapon.ScopeMagnifyLevelUp();
			}
			if (currentGamePadState.IsButtonDown(Buttons.DPadDown) && !lastGamePadState.IsButtonDown(Buttons.DPadDown))
			{
				fpsWeapon.ScopeMagnifyLevelDown();
			}
		}
		bool flag7 = false;
		bool flag8 = Stance == PlayerStance.Crouch;
		if (!OverrideInput && currentGamePadState.IsButtonDown(Buttons.B) && !lastGamePadState.IsButtonDown(Buttons.B) && fpsWeapon.Crouch())
		{
			flag8 = !flag8;
			flag7 = true;
			fpsWeapon.Crouch();
			if (flag8)
			{
				RunToggled = false;
				cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchAnim);
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchAnim, force: false);
			}
			else
			{
				cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
			}
		}
		if (!OverrideInput && currentGamePadState.IsButtonDown(Buttons.LeftStick) && !lastGamePadState.IsButtonDown(Buttons.LeftStick))
		{
			RunToggled = !RunToggled;
		}
		PainPillTimer -= fFIXED_TIME_STEP;
		RunCoolDownTimer += fFIXED_TIME_STEP;
		if (Math.Abs(Speed) > 0.001f || Math.Abs(SideStep) > 0.001f)
		{
			Stance = PlayerStance.Walk;
			if (RunToggled && flag8)
			{
				flag8 = false;
				fpsWeapon.Crouch();
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchAnim, force: false);
			}
			if (!OverrideInput && !flag8 && !TriggerDown && !Sighted && !currentGamePadState.IsButtonDown(Buttons.LeftTrigger) && fpsWeapon.CanRun && RunToggled && MoveY > -0.5f)
			{
				if (RunTimer <= Endurance && RunCoolDownTimer > RUN_COOL_DOWN)
				{
					Stance = PlayerStance.Run;
					num = 280f + tmpRunSpeed * 180f;
					RunTimer += fFIXED_TIME_STEP;
				}
				if (PainPillTimer > 0f || (WaterLevel > 30f && BloodLevel > 30f))
				{
					RunTimer = Endurance - 1f;
				}
				if (RunTimer > Endurance)
				{
					RunTimer = Endurance;
					RunCoolDownTimer = 0f;
					if (OutOfBreatheSnd != null)
					{
						OutOfBreatheSnd.Stop(AudioStopOptions.Immediate);
						OutOfBreatheSnd.Dispose();
					}
					OutOfBreatheSnd = EndGameEngine.SoundBnk.GetCue("out_of_breathe");
					OutOfBreatheSnd.Play();
				}
			}
			if (Stance != PlayerStance.Run)
			{
				RunToggled = false;
				if (flag8)
				{
					Speed *= 0.5f;
					SideStep *= 0.5f;
					Stance = PlayerStance.Crouch;
					if (flag7)
					{
						fpsWeapon.Crouch();
						if (flag8)
						{
							cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchAnim, force: false);
						}
					}
				}
				else if (Sighted)
				{
					if (ThirdPersonCamera)
					{
						Speed *= 0.65f;
						SideStep *= 0.65f;
					}
					else
					{
						Speed *= 0.5f;
						SideStep *= 0.5f;
					}
				}
			}
			float num5 = ((Math.Abs(Speed) > Math.Abs(SideStep)) ? Math.Abs(Speed * 2f) : Math.Abs(SideStep * 2f));
			if (Stance == PlayerStance.Run && MoveY > 0f)
			{
				num5 *= 1.35f;
			}
			FootStepTimer += num5;
			if (!IsAttached0 && FootStepTimer > 1f)
			{
				FootStepTimer--;
				if (FootStepSound != null)
				{
					FootStepSound.Stop(AudioStopOptions.Immediate);
					FootStepSound.Dispose();
				}
				uint alphaMap = HeightMapPhysics.GetAlphaMap(ref vecHeadPosition[qIndex]);
				if ((alphaMap & 0xFF00FFFFu) != 0)
				{
					footStepIndex = ((footStepIndex + 1 < footStepDirtSoundCues.Length) ? (footStepIndex + 1) : 0);
					FootStepSound = EndGameEngine.SoundBnk.GetCue(footStepDirtSoundCues[footStepIndex]);
				}
				else
				{
					footStepIndex = ((footStepIndex + 1 < footStepGrassSoundCues.Length) ? (footStepIndex + 1) : 0);
					FootStepSound = EndGameEngine.SoundBnk.GetCue(footStepGrassSoundCues[footStepIndex]);
				}
				FootStepSound.Play();
			}
		}
		else if (flag8)
		{
			Stance = PlayerStance.Crouch;
			if (flag7)
			{
				fpsWeapon.Crouch();
			}
			cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchAnim, force: false);
		}
		else
		{
			Stance = PlayerStance.Idle;
		}
		if (Stance != PlayerStance.Run)
		{
			RunToggled = false;
			RunTimer -= fFIXED_TIME_STEP;
			RunTimer = ((RunTimer < 0f) ? 0f : RunTimer);
		}
		Speed *= num;
		SideStep *= num * 0.7f;
		_ = Angles.Y;
		if (!ThirdPersonCamera)
		{
			Angles.X -= yaw;
			Angles.Y -= pitch;
			if (!OverridePosition)
			{
				if (Angles.X > 360f)
				{
					Angles.X -= 360f;
				}
				else if (Angles.X < 0f)
				{
					Angles.X += 360f;
				}
				if (Angles.Y > 80f)
				{
					Angles.Y = 80f;
				}
				else if (Angles.Y < -80f)
				{
					Angles.Y = -80f;
				}
			}
		}
		FireTimer -= fFIXED_TIME_STEP;
		fpsWeapon.Update(gameTime, qIndex, this);
		if (TriggerDown && !OverridePosition)
		{
			vecCameraUp = Vector3.Lerp(vecCameraUp, vecCameraUpSway, fFIXED_TIME_STEP * 20f);
		}
		else
		{
			vecCameraUp = Vector3.Lerp(vecCameraUp, Vector3.UnitY, fFIXED_TIME_STEP * 20f);
		}
		vecCameraUp.Normalize();
		if (OverrideProjection)
		{
			Zoom = MathHelper.SmoothStep(Zoom, ZoomOverride, fFIXED_TIME_STEP * 20f);
		}
		else if (ThirdPersonCamera)
		{
			float num6 = cPlayer.CurrentAnimationState.FOV + 0.1f;
			if (Stance == PlayerStance.Run)
			{
				ThirdPersonCameraZoomOnMove += 0.01f;
				ThirdPersonCameraZoomOnMove = ((ThirdPersonCameraZoomOnMove > 0.12f) ? 0.12f : ThirdPersonCameraZoomOnMove);
			}
			else
			{
				ThirdPersonCameraZoomOnMove -= 0.01f;
				ThirdPersonCameraZoomOnMove = ((ThirdPersonCameraZoomOnMove < 0f) ? 0f : ThirdPersonCameraZoomOnMove);
			}
			num6 += ThirdPersonCameraZoomOnMove;
			if (Sighted)
			{
				if (currentWeaponType != WeaponType.EmptyHands && currentWeaponType != WeaponType.Hatchet)
				{
					Zoom = MathHelper.SmoothStep(Zoom, fpsWeapon.FOV, fFIXED_TIME_STEP * 20f);
				}
				else
				{
					Zoom = MathHelper.SmoothStep(Zoom, fpsWeapon.FOV - 0.21f, fFIXED_TIME_STEP * 20f);
				}
			}
			else
			{
				Zoom = MathHelper.SmoothStep(Zoom, num6, fFIXED_TIME_STEP * 10f);
			}
			Zoom = ((Zoom < 0.08f) ? 0.08f : Zoom);
		}
		else
		{
			Zoom = MathHelper.SmoothStep(Zoom, fpsWeapon.FOV + (float)Math.PI / 12f, fFIXED_TIME_STEP * 20f);
		}
		ref Matrix reference9 = ref matProjection[qIndex];
		reference9 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, FarZPlane);
		ref Matrix reference10 = ref matSkyDomeProjection[qIndex];
		reference10 = Matrix.CreatePerspectiveFieldOfView(Zoom, AspectRatio, NearZPlane, 1000000f);
		if (OverridePosition)
		{
			vecDirection = OverrideDir;
			vecRight = OverrideRight;
		}
		else
		{
			Matrix.CreateRotationY(MathHelper.ToRadians(Angles.X + fpsWeapon.RecoilUp), out matYaw);
			Vector3.Transform(ref VecUnitZ, ref matYaw, out vecDirection);
			Vector3.Cross(ref VecUnitY, ref vecDirection, out vecRight);
			Matrix.CreateFromAxisAngle(ref vecRight, MathHelper.ToRadians(Angles.Y + fpsWeapon.RecoilSide), out matPitch);
			Vector3.Transform(ref vecDirection, ref matPitch, out vecDirection);
			if (!ThirdPersonCamera)
			{
				CameraDirection = vecDirection;
			}
		}
		vecFlatDirection = vecDirection;
		vecFlatDirection.Y = 0f;
		vecFlatDirection.Normalize();
		vecDirection.Normalize();
		tmpPrevPosition = vecPosition;
		if (!OverrideCamera && !OverridePosition)
		{
			numFramesSinceLastUpdate++;
			if (EndGameEngine.GameSettings.EnableGravity)
			{
				if (!fpsWeapon.InJump || onWalkable)
				{
					vecCurrentPosition = vecPosition;
					lastSpeed = Speed;
					vecLastDirection = vecFlatDirection;
					vecPosition.X += vecFlatDirection.X * Speed;
					vecPosition.Z += vecFlatDirection.Z * Speed;
					vecPosition.X -= vecRight.X * SideStep;
					vecPosition.Z -= vecRight.Z * SideStep;
					vecTargetPosition = vecPosition - vecCurrentPosition;
				}
				else
				{
					vecCurrentPosition = vecPosition;
					vecPosition.X += vecLastDirection.X * lastSpeed;
					vecPosition.Z += vecLastDirection.Z * lastSpeed;
					vecTargetPosition = vecPosition - vecCurrentPosition;
				}
			}
			else
			{
				lastSpeed = Speed;
				vecLastDirection = vecDirection;
				vecPosition += vecDirection * Speed;
				vecPosition.X -= vecRight.X * SideStep;
				vecPosition.Z -= vecRight.Z * SideStep;
			}
			UpdatePlayerMove(fFIXED_TIME_STEP, qIndex);
		}
		UpdateCameraAndQueue(fFIXED_TIME_STEP, qIndex);
		if ((vecPosition - tmpPrevPosition).LengthSquared() < 25f)
		{
			RunToggled = false;
		}
		vecMoveDirection = vecFlatDirection;
		if (Stance == PlayerStance.Idle)
		{
			if (Sighted)
			{
				if (cPlayer.CurrentAnimationStackIndex(0) != fpsWeapon.CurrentWeapon.CoOpSightedAnim)
				{
					cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpSightedAnim);
				}
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpSightedAnim, force: false);
			}
			else
			{
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim, force: false);
			}
		}
		else if (Stance == PlayerStance.Crouch)
		{
			if (cPlayer.CurrentAnimationStackIndex(0) != fpsWeapon.CurrentWeapon.CoOpCrouchAnim)
			{
				cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchAnim);
			}
			vecMoveDirection = vecPosition - tmpPrevPosition;
			if (vecMoveDirection.LengthSquared() > 1f)
			{
				if (MoveY < 0f)
				{
					cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchWalkBackAnim, force: false);
				}
				else
				{
					cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchWalkAnim, force: false);
				}
			}
			else
			{
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpCrouchAnim, force: false);
			}
			vecMoveDirection = vecFlatDirection;
		}
		else if (Stance == PlayerStance.Run)
		{
			if (MoveY < 0f)
			{
				Stance = PlayerStance.Walk;
				if (Sighted)
				{
					if (cPlayer.CurrentAnimationStackIndex(0) != fpsWeapon.CurrentWeapon.CoOpSightedAnim)
					{
						cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpSightedAnim);
					}
					cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkSightedAnim, force: false);
				}
				else
				{
					cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkBackAnim, force: false);
				}
			}
			else
			{
				if (cPlayer.CurrentAnimationStackIndex(0) != fpsWeapon.CurrentWeapon.CoOpIdleAnim)
				{
					cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpIdleAnim);
				}
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpRunAnim, force: false);
			}
		}
		else if (Math.Abs(MoveX * 0.7f) < Math.Abs(MoveY))
		{
			if (MoveY < 0f)
			{
				if (Sighted)
				{
					if (cPlayer.CurrentAnimationStackIndex(0) != fpsWeapon.CurrentWeapon.CoOpSightedAnim)
					{
						cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpSightedAnim);
					}
					cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkSightedAnim, force: false);
				}
				else
				{
					cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkBackAnim, force: false);
				}
			}
			else if (Sighted)
			{
				if (cPlayer.CurrentAnimationStackIndex(0) != fpsWeapon.CurrentWeapon.CoOpSightedAnim)
				{
					cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpSightedAnim);
				}
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkSightedAnim, force: false);
			}
			else
			{
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkAnim, force: false);
			}
		}
		else
		{
			vecMoveDirection = vecPosition - tmpPrevPosition;
			if (vecMoveDirection.LengthSquared() > 1f)
			{
				if (MoveX < 0f)
				{
					vecMoveDirection = Vector3.Cross(vecMoveDirection, Vector3.UnitY);
				}
				else
				{
					vecMoveDirection = Vector3.Cross(vecMoveDirection, -Vector3.UnitY);
				}
			}
			else
			{
				vecMoveDirection = vecFlatDirection;
			}
			if (cPlayer.CurrentAnimationStackIndex(0) != fpsWeapon.CurrentWeapon.CoOpSightedAnim)
			{
				cPlayer.SetBaseAnimation(fpsWeapon.CurrentWeapon.CoOpSightedAnim);
			}
			if (MoveX < 0f)
			{
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkStrafeLeftAnim, force: false);
			}
			else
			{
				cPlayer.PlayAnimation(fpsWeapon.CurrentWeapon.CoOpWalkStrafeRightAnim, force: false);
			}
		}
		if (vecMoveDirection.LengthSquared() < 1f)
		{
			vecMoveDirection = vecFlatDirection;
		}
		vecMoveDirection.Y = 0f;
		vecCharacterDir = Vector3.Lerp(vecCharacterDir, vecMoveDirection, fFIXED_TIME_STEP * 20f);
		vecCharacterDir.Normalize();
		float value3 = 0f - (float)math.SignedAngle2DInPlaneXZ(ref vecCharacterDir, ref vecFlatDirection);
		AngleTorsoCharacter = MathHelper.Lerp(AngleTorsoCharacter, value3, fFIXED_TIME_STEP * 20f);
		UpdateThirdPersonCharacter(gameTime, qIndex, isRemotePlayer: false);
		bFrustum[qIndex].Matrix = mDataQueue[qIndex].view * mDataQueue[qIndex].projection;
		PlayerFlags = ((InputRightStick.X < -0.2f) ? (PlayerFlags | FPS_NET_FLAGS.RightStickNegX) : PlayerFlags);
		PlayerFlags = ((InputRightStick.X > 0.2f) ? (PlayerFlags | FPS_NET_FLAGS.RightStickPosX) : PlayerFlags);
		PlayerFlags = ((InputRightStick.Y < -0.2f) ? (PlayerFlags | FPS_NET_FLAGS.RightStickNegY) : PlayerFlags);
		PlayerFlags = ((InputRightStick.Y > 0.2f) ? (PlayerFlags | FPS_NET_FLAGS.RightStickPosY) : PlayerFlags);
		if (EndGameEngine.GameSettings.EnableGravity && vecPosition.Y < -1000f)
		{
			Vector3 damageDir = Vector3.Zero;
			ProcessDeath(DamegePacketType.None, ref damageDir);
		}
		isSighted[qIndex] = Sighted;
	}

	private void ScheduleMeleeAttack()
	{
		MeleeAttackTimer = 0.3f;
	}

	private void MeleeAttackFunc()
	{
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		AIBase.PlayerMeleeAttack(0, ref vecPosition, ref vecDirection, playerBase.currentWeaponType);
	}

	private void UpdateCameraAndQueue(float eTimeMS, int qIndex)
	{
		if (!OverridePosition)
		{
			tmpUDDir = -vecDirection;
			tmpUDRight = Vector3.Cross(tmpUDDir, Vector3.UnitY);
			tmpUDRight.Normalize();
		}
		else
		{
			tmpUDDir = -vecDirection;
			tmpUDRight = Vector3.Cross(tmpUDDir, OverrideUp);
			tmpUDRight.Normalize();
		}
		if (OverridePosition)
		{
			vecPosition = OverridePos;
			mDataQueue[qIndex].world = Matrix.Identity;
			mDataQueue[qIndex].world.Forward = OverrideDir;
			mDataQueue[qIndex].world.Up = OverrideUp;
			mDataQueue[qIndex].world.Right = OverrideRight;
			mDataQueue[qIndex].world *= tmpPlayerScale;
		}
		else
		{
			mDataQueue[qIndex].world = Matrix.Identity;
			mDataQueue[qIndex].world.Right = tmpUDRight;
			mDataQueue[qIndex].world.Forward = tmpUDDir;
			mDataQueue[qIndex].world.Up = Vector3.Cross(tmpUDRight, tmpUDDir);
			mDataQueue[qIndex].world *= tmpPlayerScale;
		}
		if (Stance == PlayerStance.Crouch)
		{
			CrouchY = MathHelper.Lerp(CrouchY, 0f, 10f * eTimeMS);
		}
		else
		{
			CrouchY = MathHelper.Lerp(CrouchY, 34f, 10f * eTimeMS);
		}
		tmpUDPlrPos = vecPosition;
		tmpUDPlrPos.Y += CrouchY;
		mDataQueue[qIndex].world.Translation = tmpUDPlrPos;
		AnimBlend = Math.Abs(MoveY);
		if (AnimBlend < Math.Abs(MoveX))
		{
			AnimBlend = Math.Abs(MoveX);
		}
		if (ApocalypseZ_Hack)
		{
			if (OverrideCamera)
			{
				ref Vector3 reference = ref vecHeadPosition[qIndex];
				reference = vecPosition;
				matView = AIBase.OverrideCamera(qIndex, this);
			}
			else if (OverridePosition)
			{
				ref Vector3 reference2 = ref vecHeadPosition[qIndex];
				reference2 = vecPosition;
				matView = Matrix.CreateLookAt(OverridePos, OverridePos + OverrideDir * 1000f, OverrideUp);
			}
			else if (ThirdPersonCamera)
			{
				tmpTargetPos = vecPosition;
				if (Sighted || Stance == PlayerStance.Crouch)
				{
					tmpTargetPos = thirdPersonHeadmat.Translation + vecPosition;
					tmpTargetPos.Y -= TmpViewHeight / 2;
				}
				else
				{
					tmpTargetPos.Y += TmpViewHeight;
				}
				tmpRay.Direction = CameraDirection * (0f - CurrentViewDis);
				tmpRay.Direction.Normalize();
				tmpTargetPos += Vector3.Cross(tmpRay.Direction, fpsWeapon.HeadUp) * TmpRightShift;
				tmpRay.Position = tmpTargetPos;
				if (Sighted)
				{
					tmpTargetPos += fpsWeapon.HeadUp * SightedViewHeight;
					tmpTargetPos += Vector3.Cross(tmpRay.Direction, fpsWeapon.HeadUp) * SightedRightShift;
					tmpTargetPos += tmpRay.Direction * SightedViewDis;
				}
				else
				{
					tmpCamDir0 = Vector3.Cross(tmpRay.Direction, fpsWeapon.HeadUp);
					tmpCamDir0.Normalize();
					tmpTargetPos += tmpCamDir0 * TmpRightShift;
					tmpTargetPos += tmpRay.Direction * CurrentViewDis;
				}
				ThirdPersonCameraPos.X = MathHelper.Lerp(ThirdPersonCameraPos.X, tmpTargetPos.X, 0.75f);
				ThirdPersonCameraPos.Z = MathHelper.Lerp(ThirdPersonCameraPos.Z, tmpTargetPos.Z, 0.75f);
				ThirdPersonCameraPos.Y = MathHelper.Lerp(ThirdPersonCameraPos.Y, tmpTargetPos.Y, 0.5f);
				if (CurrentCollisionArea != null)
				{
					float hitDistance = CurrentViewDis;
					hitPosition = Vector3.Zero;
					hitNormal = Vector3.Zero;
					if (CurrentCollisionArea.RayCast(qIndex, ref tmpRay, ref hitPosition, ref hitNormal, ref hitDistance) && hitDistance < CurrentViewDis)
					{
						if (hitDistance < 32f)
						{
							vecPosition += hitNormal * (32f - hitDistance);
						}
						ThirdPersonCameraPos = hitPosition + hitNormal * 5f;
					}
				}
				float num = HeightMapPhysics.GetHeight(ref ThirdPersonCameraPos) + 48f;
				ThirdPersonCameraPos.Y = ((num > ThirdPersonCameraPos.Y) ? num : ThirdPersonCameraPos.Y);
				ref Vector3 reference3 = ref vecHeadPosition[qIndex];
				reference3 = ThirdPersonCameraPos;
				vecHead3rdPersonPos = vecHeadPosition[qIndex];
				vecHead3rdPersonPos.X = 0f;
				vecHead3rdPersonPos.Z = 0f;
				matView = Matrix.CreateLookAt(vecHead3rdPersonPos, vecHead3rdPersonPos + CameraDirection * 1000f, Vector3.UnitY);
			}
			else
			{
				if (cPlayer.mergeAnimPlayer.CurrentClip != null && cPlayer.mergeAnimPlayer.CurrentClip.AnimType == AnimationType.Jump)
				{
					vecHeadPosition[qIndex].X = 0f;
					vecHeadPosition[qIndex].Z = 0f;
					vecHeadPosition[qIndex].Y = fpsWeapon.HeadPosition.Y;
				}
				else
				{
					vecHeadPosition[qIndex].X = 0f;
					vecHeadPosition[qIndex].Z = 0f;
					vecHeadPosition[qIndex].Y = fpsWeapon.HeadPosition.Y;
				}
				matView = Matrix.CreateLookAt(vecHeadPosition[qIndex], vecHeadPosition[qIndex] + fpsWeapon.HeadDirection * 1000f, vecCameraUp);
				if (cPlayer.CurrentAnimation == WeaponAnim.CoOpJump)
				{
					ref Vector3 reference4 = ref vecHeadPosition[qIndex];
					reference4 = thirdPersonHeadmat.Translation;
				}
				else
				{
					ref Vector3 reference5 = ref vecHeadPosition[qIndex];
					reference5 = fpsWeapon.HeadPosition;
				}
			}
		}
		else if (OverrideCamera)
		{
			matView = AIBase.OverrideCamera(qIndex, this);
		}
		else if (OverridePosition)
		{
			matView = Matrix.CreateLookAt(OverridePos, OverridePos + OverrideDir * 1000f, OverrideUp);
		}
		else if (ThirdPersonCamera)
		{
			Vector3 vector = vecPosition + Vector3.Cross(vecDirection, Vector3.UnitY) * 20f;
			vector.Y += 80f;
			vector += vecDirection * (0f - CurrentViewDis);
			matView = Matrix.CreateLookAt(vector, vector + vecDirection * 1000f, Vector3.UnitY);
		}
		else
		{
			matView = Matrix.CreateLookAt(fpsWeapon.HeadPosition, fpsWeapon.HeadPosition + fpsWeapon.HeadDirection * 1000f, vecCameraUp);
		}
		CalculateLightMatrices(qIndex);
		mDataQueue[qIndex].eyePosition = fpsWeapon.HeadPosition;
		mDataQueue[qIndex].cameraEyePos = Vector3.Transform(-matView.Translation, Matrix.Transpose(matView));
		mDataQueue[qIndex].cameraPos = vecPosition;
		mDataQueue[qIndex].cameralookAt = vecDirection;
		mDataQueue[qIndex].cameraDirN = vecDirection;
		mDataQueue[qIndex].cameraUp = vecCameraUp;
		mDataQueue[qIndex].view = matView;
		mDataQueue[qIndex].projection = matProjection[qIndex];
		mDataQueue[qIndex].viewProj = matView * matProjection[qIndex];
		mDataQueue[qIndex].invViewProj = Matrix.Invert(matView * matProjection[qIndex]);
		mDataQueue[qIndex].lightView = matLightView;
		mDataQueue[qIndex].lightProj = matLightProj;
		mDataQueue[qIndex].lightEyePos = Vector3.Transform(-matLightView.Translation, Matrix.Transpose(matLightView));
	}

	public void KillSound()
	{
		fpsWeapon.KillSound();
	}

	public override void ProcessDeath(DamegePacketType damageType, ref Vector3 damageDir)
	{
		if (NumberLives > 0)
		{
			NumberLives--;
		}
		saveStatusTimer = 120f;
		fpsWeapon.KillSound();
		if (MatchCoolDownTimer > 0f)
		{
			Health = 100f;
		}
		else if (Spawned)
		{
			Health = 0f;
			BloodLevel = 0f;
			CurrentDay = 0;
			Spawned = false;
			SpawnRequested = false;
			ToggledRespawn = true;
			DeathTimer = 6f;
			vecPosition -= vecDirection * 48f;
			PacketRecievDelayTimer = 1f;
			NumKillStreakGrenade = 0;
			NumKillStreakBalistic = 0;
			NumKillStreakKnife = 0;
			mRagdoll.SetRagdoll = true;
			mRagdoll.DamageType = damageType;
			mRagdoll.DamageDirection = damageDir;
			PlayerFlags |= FPS_NET_FLAGS.Death;
			PlayerFlags &= (FPS_NET_FLAGS)(-2);
			AIBase.PlayerDeath(this);
			if (EGENetWorkNext.networkSession != null)
			{
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)132);
				packetWriter.Write(NetGamerRef.Id);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder, EGENetWorkNext.networkSession.Host);
			}
		}
	}

	public void CameraOverRideZoom(ref Vector3 cameraPos, ref Vector3 cameraAt, ref Vector3 cameraShake)
	{
		zoomSwivelTimer -= 0.03f;
		if (zoomSwivelTimer < 0f)
		{
			zoomSwivelTimer = (float)EndGameEngine.randGenerator.NextDouble() * 0.5f;
			zoomSwivel.X = (float)EndGameEngine.randGenerator.NextDouble() * 200f;
			zoomSwivel.Y = (float)EndGameEngine.randGenerator.NextDouble() * 100f;
			zoomSwivel.Z = (float)EndGameEngine.randGenerator.NextDouble() * 200f;
		}
		zoomSwivelAt = Vector3.Lerp(zoomSwivelAt, zoomSwivel, 0.01f);
		cameraShake = zoomSwivelAt;
		float num = (cameraPos - cameraAt).Length() / zoomCamdivisor;
		num *= num;
		zoomCamOverride = MathHelper.Lerp(zoomCamOverride, (float)Math.PI / (4f + num), 0.025f);
		OverrideProjection = true;
		ZoomOverride = zoomCamOverride;
	}

	public override void UpdateHealth(float eTimeMS)
	{
		base.UpdateHealth(eTimeMS);
		if (LevelBaseMenu.debugSecondHasElapsed)
		{
			float num = WaterLevel - ((Stance == PlayerStance.Run) ? 0.12f : 0.0725f) * AIBase.TimeOfDayMultiplyer;
			WaterLevel = ((num < 0f) ? 0f : num);
			float num2 = FoodLevel - ((Stance == PlayerStance.Run) ? 0.06f : 0.04f) * AIBase.TimeOfDayMultiplyer;
			float num3 = BloodLevel - BloodLoss * AIBase.TimeOfDayMultiplyer;
			if (WaterLevel < 1f)
			{
				num3 -= 0.04f * AIBase.TimeOfDayMultiplyer;
				num2 -= 0.015f * AIBase.TimeOfDayMultiplyer;
			}
			if (FoodLevel < 1f)
			{
				num3 -= 0.015f * AIBase.TimeOfDayMultiplyer;
			}
			BloodLevel = ((num3 < 0f) ? 0f : num3);
			FoodLevel = ((num2 < 0f) ? 0f : num2);
			if (BloodLevel < 1f)
			{
				Vector3 damageDir = Vector3.Zero;
				ProcessDeath(DamegePacketType.None, ref damageDir);
			}
		}
	}

	public void UpdateInput(float eTimeMS, int qIndex, ref float yaw, ref float pitch)
	{
		if (OverrideInput || InventoryCls.InventoryOpen)
		{
			MoveX = 0f;
			MoveY = 0f;
			InputLeftStick = Vector2.Zero;
			InputRightStick = Vector2.Zero;
			return;
		}
		if (!FPSGameMenu.isCurrentScore || !LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].IsModerator)
		{
			if (fpsWeapon.CurrentWeapon.AttachmentTwo == WeaponAttachment.NadeLauncher && !OverrideButtonLeftShoulder && currentGamePadState.IsButtonDown(Buttons.LeftShoulder) && !lastGamePadState.IsButtonDown(Buttons.LeftShoulder) && NumberNaderGrenades > 0)
			{
				fpsWeapon.CurrentWeapon.NaderToggled = !fpsWeapon.CurrentWeapon.NaderToggled;
			}
			if (currentGamePadState.IsButtonDown(Buttons.A) && !lastGamePadState.IsButtonDown(Buttons.A) && JumpYTime <= 0f && fpsWeapon.Jump())
			{
				cPlayer.PlayMergedAnimation(fpsWeapon.CurrentWeapon.CoOpJumpAnim);
				JumpYValue = 0f;
				JumpYTime = 1f;
			}
			if (JumpYTime > 0.5f)
			{
				JumpYTime -= eTimeMS;
				JumpYValue -= 0.25f;
				if (JumpYValue < 0f)
				{
					JumpYValue = 0f;
				}
			}
			if (JumpYTime > 0f)
			{
				JumpYTime -= eTimeMS;
				if (Stance == PlayerStance.Crouch)
				{
					Stance = PlayerStance.Walk;
				}
			}
			if (EndGameEngine.GameSettings.GameName.Contains("ApocalypseZ"))
			{
				if (currentGamePadState.IsButtonDown(Buttons.DPadDown) && !lastGamePadState.IsButtonDown(Buttons.DPadDown))
				{
					if (ThirdPersonCamera)
					{
						ThirdPersonCamera = false;
						Angles.Y = 16f;
						CameraAngles.Y = 16f;
						AIBase.BlackFadeTimer = 1f;
					}
					else
					{
						ThirdPersonCamera = true;
						Angles.Y = 16f;
						CameraAngles.X = Angles.X;
						CameraAngles.Y = 21f;
						CameraDirection = vecDirection;
						ThirdPersonCameraPos = vecPosition;
						ThirdPersonCameraPos.Y += 80f;
						thirdPersonHeadmat = Matrix.Identity;
						AIBase.BlackFadeTimer = 1f;
					}
				}
				if (currentGamePadState.IsButtonDown(Buttons.DPadRight) && !lastGamePadState.IsButtonDown(Buttons.DPadRight))
				{
					if (AIBase.PlayerInventory.HaveItem(1024, 7))
					{
						ToggleFlashLight();
					}
					else
					{
						FlashLightOn = false;
						NetworkUpdateFrameCount = -1;
					}
				}
			}
			if (currentGamePadState.IsButtonDown(Buttons.Y) && !lastGamePadState.IsButtonDown(Buttons.Y))
			{
				int count = AIBase.PlayerInventory.InventoryArray[1].list.Count;
				int i;
				for (i = 0; i < count; i++)
				{
					InventoryItemCls inventoryItemCls = AIBase.PlayerInventory.InventoryArray[1].list[i];
					if (inventoryItemCls.desc != 0 && WeaponsCls.GetWeaponType(inventoryItemCls.ItemType) == fpsWeapon.CurrentWeapon.WepType)
					{
						break;
					}
				}
				for (int j = 0; j < count; j++)
				{
					i++;
					if (i == count)
					{
						fpsWeapon.SetWeapon(WeaponType.EmptyHands);
						break;
					}
					if (i > count)
					{
						i = 0;
					}
					InventoryItemCls inventoryItemCls2 = AIBase.PlayerInventory.InventoryArray[1].list[i];
					if (inventoryItemCls2.desc != 0 && WeaponsCls.GetWeaponType(inventoryItemCls2.ItemType) != fpsWeapon.CurrentWeapon.WepType)
					{
						AIBase.PlayerInventory.UseItem(inventoryItemCls2, this);
						break;
					}
				}
			}
			if (NumberThrowingKnife > 0 && currentGamePadState.IsButtonDown(Buttons.DPadLeft) && !lastGamePadState.IsButtonDown(Buttons.DPadLeft))
			{
				WeaponType e = WeaponType.ThrowingKnife;
				if (fpsWeapon.CurrentWeapon.WepType == WeaponType.ThrowingKnife)
				{
					e = PrimaryWeapon;
				}
				if (fpsWeapon.SwitchWeapon(e))
				{
					tmpMergeAnim = WeaponAnim.CoOpSwap;
					cPlayer.PlayMergedAnimation(WeaponAnim.CoOpSwap);
				}
			}
			if (!IsAttached0 && currentGamePadState.IsButtonDown(Buttons.X) && !lastGamePadState.IsButtonDown(Buttons.X) && !OverrideButtonX && fpsWeapon.ReloadWeapon())
			{
				tmpMergeAnim = WeaponAnim.CoOpReload;
				cPlayer.PlayMergedAnimation(WeaponAnim.CoOpReload);
			}
		}
		InputRightStick.X = Math.Abs(currentGamePadState.ThumbSticks.Right.X);
		InputRightStick.Y = Math.Abs(currentGamePadState.ThumbSticks.Right.Y);
		InputRightStick.X = InputRightStick.X * InputRightStick.X * PlayerControllerSensitivity;
		InputRightStick.Y = InputRightStick.Y * InputRightStick.Y * PlayerControllerSensitivity;
		if (currentGamePadState.ThumbSticks.Right.X < 0f)
		{
			InputRightStick.X *= -1f;
		}
		if (currentGamePadState.ThumbSticks.Right.Y < 0f)
		{
			InputRightStick.Y *= -1f;
		}
		InputRightStick.Y *= InvertY;
		bool sighted = Sighted;
		if (ThirdPersonCamera)
		{
			Sighted = currentGamePadState.IsButtonDown(Buttons.LeftTrigger);
		}
		else
		{
			Sighted = fpsWeapon.fpsAmin.CurrentAnimationState.AnimType == AnimationType.Sights;
		}
		if (AimAssist && !sighted && Sighted)
		{
			Vector3 direction = Vector3.Zero;
			AimAssistTarget = AIBase.GetAimAssistVector(qIndex, this, ref direction);
			if (AimAssistTarget != null)
			{
				SegmentParams.OnlyWalkable = true;
				SegmentParams.SegmentDirection = direction;
				SegmentParams.SegmentLength = 12000f;
				SegmentParams.SegmentStart = vecPosition;
				SegmentParams.SegmentEnd = vecPosition + direction * SegmentParams.SegmentLength;
				SegmentParams.PreComputeParameters();
				MaterialType materialType = LevelOutside.RayCast(qIndex, ref SegmentParams, spawnSparks: true);
				float num = (AimAssistTarget.Position - vecPosition).LengthSquared();
				if (materialType == MaterialType.Undefined || num < SegmentParams.hitDistance * SegmentParams.hitDistance)
				{
					AimAssistTimer = 0.5f;
					Vector3 vector = direction;
					Vector3 second = vecDirection;
					vector.Y = 0f;
					second.Y = 0f;
					float radians = MyMath.AngleBetweenVectors(vector, second);
					if (Vector3.Dot(vecRight, vector) < 0f)
					{
						Angles.X -= MathHelper.ToDegrees(radians);
					}
					else
					{
						Angles.X += MathHelper.ToDegrees(radians);
					}
					vector.Y = direction.Y;
					second.Y = vecDirection.Y;
					float num2 = vector.Y - second.Y;
					if (num2 < 0f)
					{
						Angles.Y -= MathHelper.ToDegrees(num2);
					}
					else
					{
						Angles.Y -= MathHelper.ToDegrees(num2);
					}
				}
				else
				{
					AimAssistTimer = 0f;
					AimAssistTarget = null;
				}
			}
		}
		else if (AimAssist && AimAssistTarget != null)
		{
			AimAssistTimer -= eTimeMS;
			if (AimAssistTimer <= 0f)
			{
				AimAssistTarget = null;
			}
			else
			{
				Vector3 vector2 = AimAssistTarget.Position - vecPosition;
				if (AimAssistTarget.CurrentAnimation != WeaponAnim.CoOpCrouch)
				{
					vector2.Y += 110f;
				}
				else
				{
					vector2.Y += 60f;
				}
				vector2.Normalize();
				float num3 = vector2.Y - vecDirection.Y;
				if (num3 < 0f)
				{
					Angles.Y -= MathHelper.ToDegrees(num3);
				}
				else
				{
					Angles.Y -= MathHelper.ToDegrees(num3);
				}
				Vector3 second2 = vecDirection;
				second2.Y = 0f;
				vector2.Y = 0f;
				float radians2 = MyMath.AngleBetweenVectors(vector2, second2);
				if (Vector3.Dot(vecRight, vector2) < 0f)
				{
					Angles.X -= MathHelper.ToDegrees(radians2);
				}
				else
				{
					Angles.X += MathHelper.ToDegrees(radians2);
				}
			}
		}
		float num4 = 10f;
		float num5 = 6.5f;
		float num6 = 3f;
		if (Sighted)
		{
			if (ThirdPersonCamera)
			{
				num5 = 4f;
				num6 = 1.5f;
				yaw = InputRightStick.X * num5;
				pitch = InputRightStick.Y * num6;
				fBlurFactor = MathHelper.Lerp(fBlurFactor, 0f, eTimeMS * num4);
			}
			else if (fpsWeapon.ScopeMagnificationLevel == 0)
			{
				num5 = 2f;
				num6 = 0.9f;
				yaw = InputRightStick.X * num5;
				pitch = InputRightStick.Y * num6;
				fBlurFactor = MathHelper.Lerp(fBlurFactor, 0f, eTimeMS * num4);
			}
			else if (fpsWeapon.ScopeMagnificationLevel == 1)
			{
				num5 = 1.5f;
				num6 = 1f;
				yaw = InputRightStick.X * num5;
				pitch = InputRightStick.Y * num6;
				fBlurFactor = MathHelper.Lerp(fBlurFactor, 1f, eTimeMS * num4);
			}
			else if (fpsWeapon.ScopeMagnificationLevel == 2)
			{
				num5 = 0.5f;
				num6 = 0.25f;
				yaw = InputRightStick.X * num5;
				pitch = InputRightStick.Y * num6;
				fBlurFactor = MathHelper.Lerp(fBlurFactor, 1f, eTimeMS * num4);
			}
			else if (fpsWeapon.ScopeMagnificationLevel == 3)
			{
				num5 = 0.25f;
				num6 = 0.1f;
				yaw = InputRightStick.X * num5;
				pitch = InputRightStick.Y * num6;
				fBlurFactor = MathHelper.Lerp(fBlurFactor, 1f, eTimeMS * num4);
			}
		}
		else
		{
			yaw = InputRightStick.X * num5;
			pitch = InputRightStick.Y * num6;
			fBlurFactor = MathHelper.Lerp(fBlurFactor, 0f, eTimeMS * num4);
		}
		if (ThirdPersonCamera)
		{
			InputLeftStick.X = MathHelper.Lerp(InputLeftStick.X, currentGamePadState.ThumbSticks.Left.X, eTimeMS * 5f);
			InputLeftStick.Y = MathHelper.Lerp(InputLeftStick.Y, currentGamePadState.ThumbSticks.Left.Y, eTimeMS * 5f);
		}
		else
		{
			InputLeftStick.X = currentGamePadState.ThumbSticks.Left.X;
			if (Stance == PlayerStance.Run)
			{
				InputLeftStick.X *= 0.75f;
			}
			MoveX = MathHelper.SmoothStep(MoveX, InputLeftStick.X, eTimeMS * 15f);
			InputLeftStick.Y = currentGamePadState.ThumbSticks.Left.Y;
			MoveY = MathHelper.SmoothStep(MoveY, InputLeftStick.Y, eTimeMS * 15f);
		}
		if (vecPosition.Y < 1200f)
		{
			float num7 = 1f - (1200f - vecPosition.Y) * 0.01f;
			num7 = ((num7 < 0.1f) ? 0.1f : num7);
			InputLeftStick *= num7;
			MoveX *= num7;
			MoveY *= num7;
			if (Stance == PlayerStance.Crouch)
			{
				if (vecPosition.Y < 1188f)
				{
					BloodLevel = ((BloodLevel - 0.15f > 0f) ? (BloodLevel - 0.15f) : 0f);
				}
			}
			else if (vecPosition.Y < 1135f)
			{
				BloodLevel = ((BloodLevel - 0.15f > 0f) ? (BloodLevel - 0.15f) : 0f);
			}
		}
		if (Math.Abs(MoveX) < 0.0001f)
		{
			MoveX = 0f;
		}
		if (Math.Abs(MoveY) < 0.0001f)
		{
			MoveY = 0f;
		}
		if (CurrentTrigger == TriggerTypes.Ladder && MoveY > 0f)
		{
			vecPosition.Y += MoveY * 1200f * eTimeMS;
			MoveX = 0f;
			MoveY = 0f;
		}
		else if (CurrentTrigger == TriggerTypes.LadderBottom)
		{
			Angles.X = 50f;
			if (MoveY > 0f)
			{
				vecPosition.Y += MoveY * 1200f * eTimeMS;
				MoveX = 0f;
				MoveY = 0f;
			}
		}
	}

	public void ToggleFlashLight()
	{
		FlashLightOn = !FlashLightOn;
		NetworkUpdateFrameCount = -1;
		Menu.PlaySelect();
	}

	public void UpdateThirdPersonCharacter(GameTime gameTime, int qIndex, bool isRemotePlayer)
	{
		float num = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		for (int i = 0; i < 6; i++)
		{
			if (!(MuzzleSoundDelay[i] >= 0f))
			{
				continue;
			}
			MuzzleSoundDelay[i] -= 0.03334f;
			if (MuzzleSoundDelay[i] < 0f)
			{
				DrawMuzzleFlashAlpha = 1f;
				if (!fireSND0.IsDisposed)
				{
					fireSND0.Stop(AudioStopOptions.Immediate);
					fireSND0.Dispose();
				}
				fireSND0 = EndGameEngine.SoundBnk.GetCue(fpsWeapon.CurrentWeapon.WeaponShotSound0);
				fireSND0.Play();
				fireSND0.SetVariable("Distance", MuzzleDistanceDelay[i]);
			}
		}
		PacketRecievDelayTimer -= num;
		SetRagdoll(gameTime);
		if (Spawned)
		{
			RespawnDelayTimer += num;
			if (this != LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value])
			{
				tmp3rdPersonSphere.Center = vecPosition;
				tmp3rdPersonSphere.Center.X -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].X;
				tmp3rdPersonSphere.Center.Z -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[qIndex].Z;
				tmp3rdPersonContianment = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[qIndex].Contains(tmp3rdPersonSphere);
				if (tmp3rdPersonContianment == ContainmentType.Contains || tmp3rdPersonContianment == ContainmentType.Intersects)
				{
					Render3rdPerson[qIndex] = true;
				}
				else
				{
					Render3rdPerson[qIndex] = false;
				}
				CurrentDetectionDistance = ((CurrentDetectionDistance > 1000f) ? (CurrentDetectionDistance - 5f) : 1000f);
				if (ShotFired)
				{
					ShotFired = false;
					CurrentDetectionDistance = 2000f;
				}
			}
			else
			{
				Render3rdPerson[qIndex] = true;
				CurrentDetectionDistance = ((CurrentDetectionDistance > 1000f) ? (CurrentDetectionDistance - 5f) : 1000f);
				if (ShotFired)
				{
					CurrentDetectionDistance = 2000f;
				}
			}
			if (!Render3rdPerson[qIndex])
			{
				Set3rdPersonHandPosition();
				mat3rdPaerson = tmpPlayerScale;
				cPlayer.Update(gameTime.ElapsedGameTime, ref mat3rdPaerson, qIndex, AnimBlend);
			}
			else
			{
				c3rdPersonFacePunchPitch = MathHelper.Lerp(c3rdPersonFacePunchPitch, 0f, num * 8f);
				c3rdPersonFacePunchYaw = MathHelper.Lerp(c3rdPersonFacePunchYaw, 0f, num * 6f);
				c3rdPersonFacePunchYaw2 = MathHelper.Lerp(c3rdPersonFacePunchYaw2, 0f, num * 6f);
				if (Math.Abs(c3rdPersonFacePunchYaw2) > 0.001f)
				{
					Matrix.CreateRotationY(c3rdPersonFacePunchYaw2, out tmpMat3rdPlayerYaw);
					tmpMat3rdPlayerYaw *= tmpMat3rdPlayer;
					cPlayer.ApplyUserTransform(13, 13, ref tmpMat3rdPlayerYaw);
				}
				c3rdPersonFireWeaponRecoil = MathHelper.Lerp(c3rdPersonFireWeaponRecoil, 0f, num * 20f);
				float num2 = Angles.Y * 0.25f;
				Matrix.CreateRotationX(MathHelper.ToRadians(num2 + c3rdPersonFireWeaponRecoil + c3rdPersonFacePunchPitch), out tmpMat3rdPlayer);
				cPlayer.ApplyUserTransform(10, 10, ref tmpMat3rdPlayer);
				cPlayer.ApplyUserTransform(11, 11, ref tmpMat3rdPlayer);
				Matrix.CreateRotationY(AngleTorsoCharacter + c3rdPersonFacePunchYaw, out tmpMat3rdPlayerYaw);
				tmpMat3rdPlayerYaw *= tmpMat3rdPlayer;
				cPlayer.ApplyUserTransform(9, 9, ref tmpMat3rdPlayerYaw);
				Set3rdPersonHandPosition();
				mat3rdPaerson = Matrix.Identity;
				tmpVec3rdPlayer = vecCharacterDir * -1f;
				tmpVec3rdRight = Vector3.Cross(tmpVec3rdPlayer, Vector3.UnitY);
				mat3rdPaerson.Forward = tmpVec3rdPlayer;
				mat3rdPaerson.Right = tmpVec3rdRight;
				mat3rdPaerson.Up = Vector3.Cross(tmpVec3rdRight, tmpVec3rdPlayer);
				mat3rdPaerson *= tmpPlayerScale;
				mat3rdPaerson.Translation = Vector3.Zero;
				cPlayer.Update(gameTime.ElapsedGameTime, ref mat3rdPaerson, qIndex, AnimBlend);
			}
			if (FlashLightOn && !IsAttached0)
			{
				drawtmpRight = Matrix.Identity;
				cPlayer.GetWorldTransformBlend(qIndex, 13, out matFlashLight[qIndex]);
				math.RemoveScaling(ref matFlashLight[qIndex]);
				tmpPos = matFlashLight[qIndex].Forward * 100f;
				tmpPos.Y -= 40f;
				flashLightDir = Vector3.Lerp(flashLightDir * 100f, tmpPos, 0.35f);
				flashLightDir.Normalize();
				tmpVec3rdPlayer = matFlashLight[qIndex].Translation + vecPosition + CoOpOffset;
				tmpVec3rdPlayer += matFlashLight[qIndex].Forward * -8f;
				tmpVec3rdPlayer += matFlashLight[qIndex].Down * 16f;
				tmpVec3rdPlayer += matFlashLight[qIndex].Right * 8f;
				matFlashLight[qIndex].Forward = flashLightDir;
				matFlashLight[qIndex].Down = -Vector3.UnitY;
				matFlashLight[qIndex].Right = Vector3.Cross(flashLightDir, -Vector3.UnitY);
				matFlashLight[qIndex].Translation = tmpVec3rdPlayer + flashLightDir * -10f;
				flashLightColor = Color.White;
				flashLightPos = matFlashLight[qIndex].Translation;
				Vector3 spotParams = new Vector3(0.8f, 0.97f, 0.98f);
				LevelBaseMenu.PointLights.AddDynamicSpotLight(ref flashLightPos, ref flashLightDir, ref spotParams, ref flashLightColor, 4000f, 2000f, qIndex);
			}
			if (isRemotePlayer)
			{
				if (fpsWeapon.CurWeaponType != currentWeaponType)
				{
					fpsWeapon.SetWeapon(currentWeaponType);
				}
				fpsWeapon.FireTimer -= num * 2f;
				if (((PlayerFlags & FPS_NET_FLAGS.FireWeapon) > FPS_NET_FLAGS.Clear || (PlayerFlags & FPS_NET_FLAGS.FireAuto) > FPS_NET_FLAGS.Clear) && fpsWeapon.FireTimer < 0f)
				{
					ShotFired = true;
					fpsWeapon.FireTimer = fpsWeapon.CurrentWeapon.FireRate;
				}
				if (ShotFired)
				{
					vecDirection = Vector3.TransformNormal(vecCharacterDir, Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.UnitY, vecCharacterDir), MathHelper.ToRadians(Angles.Y)));
					c3rdPersonFireWeaponRecoil = -1f;
					if (currentWeaponType == WeaponType.Shotgun)
					{
						particles.SpawnMuzzleFlashShotty(ref vec3rdPersonMuzzlePos, ref vecDirection, fps: false);
					}
					else
					{
						particles.SpawnMuzzleFlash2(ref vec3rdPersonMuzzlePos, fps: false);
					}
					particles.SpawnMuzzleSmoke(ref vec3rdPersonMuzzlePos, ref vecDirection, fps: false);
					fpsWeapon.TriggerHeldDown = true;
					Vector3 vector = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - vecPosition;
					float num3 = vector.LengthSquared();
					if (num3 < 400000000f)
					{
						num3 = num3 / 400000000f * 20000f;
					}
					for (int j = 0; j < 6; j++)
					{
						if (MuzzleSoundDelay[j] < 0f)
						{
							MuzzleSoundDelay[j] = 0.0001f * num3;
							MuzzleDistanceDelay[j] = num3;
							break;
						}
					}
					if (num3 > 500f)
					{
						int num4 = EndGameEngine.randGenerator.Next(0, BulletWhizSounds.Length + 2);
						if (num4 < BulletWhizSounds.Length && Vector3.Dot(vector, vecDirection) > 0f)
						{
							vector.Normalize();
							float num5 = Vector3.Dot(vector, vecDirection);
							if (num5 > 0.98f)
							{
								if (!whizSND0.IsDisposed)
								{
									whizSND0.Stop(AudioStopOptions.Immediate);
									whizSND0.Dispose();
								}
								whizSND0 = EndGameEngine.SoundBnk.GetCue(BulletWhizSounds[num4]);
								whizSND0.Play();
								whizSND0.SetVariable("Distance", (1f - num5) * 50f * 18000f);
							}
						}
					}
					PlayerFlags &= (FPS_NET_FLAGS)(-5);
				}
			}
		}
		else if (isRemotePlayer)
		{
			mat3rdPaerson = tmpPlayerScale;
			mat3rdPaerson.Translation = Vector3.UnitZ + Vector3.UnitY * -2000f;
			cPlayer.Update(gameTime.ElapsedGameTime, ref mat3rdPaerson, qIndex, AnimBlend);
			if (!fireSND0.IsDisposed)
			{
				fireSND0.Stop(AudioStopOptions.Immediate);
				fireSND0.Dispose();
			}
		}
		if (ThirdPersonCamera)
		{
			thirdPersonHeadmat = cPlayer.WorldTransformBlend[qIndex][10];
			return;
		}
		thirdPersonHeadmat = cPlayer.WorldTransformBlend[qIndex][13];
		thirdPersonHeadmat.Translation = thirdPersonHeadmat.Translation + vecPosition + CoOpOffset;
	}

	public void Set3rdPersonHandPosition()
	{
		tmp3rdPersonHand = Matrix.Identity;
		if ((fpsWeapon.CurrentWeapon.WepType == WeaponType.FiftyCal || fpsWeapon.CurrentWeapon.WepType == WeaponType.NineMil) && Stance != PlayerStance.Run)
		{
			float radians = MathHelper.ToRadians(45f);
			float radians2 = MathHelper.ToRadians(-55f);
			Matrix.CreateRotationX(radians, out tmp3rdPersonHand);
			cPlayer.ApplyUserTransform(17, 17, ref tmp3rdPersonHand);
			Matrix.CreateRotationX(radians2, out tmp3rdPersonHand);
			cPlayer.ApplyUserTransform(18, 18, ref tmp3rdPersonHand);
			float radians3 = MathHelper.ToRadians(-5f);
			Matrix.CreateRotationX(radians3, out tmp3rdPersonHand);
			cPlayer.ApplyUserTransform(14, 14, ref tmp3rdPersonHand);
		}
		else
		{
			cPlayer.ApplyUserTransform(18, 18, ref tmp3rdPersonHand);
			cPlayer.ApplyUserTransform(14, 14, ref tmp3rdPersonHand);
			cPlayer.ApplyUserTransform(15, 15, ref tmp3rdPersonHand);
			float radians4 = MathHelper.ToRadians(4f);
			Matrix.CreateRotationY(radians4, out tmp3rdPersonHand);
			cPlayer.ApplyUserTransform(17, 17, ref tmp3rdPersonHand);
		}
	}

	public void UpdateRagdoll()
	{
		if (mRagdoll.IsValid)
		{
			mRagdoll.Update();
		}
	}

	public void SetRagdoll(GameTime gameTime)
	{
		int num = LevelBaseMenu.DataQueueUpdate;
		if (num > 1)
		{
			num = 0;
		}
		if (mRagdoll.IsValid)
		{
			tmp3rdPersonSphere.Center = mRagdoll.RagdollWorldPose[0].Translation;
			tmp3rdPersonSphere.Center.X -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[num].X;
			tmp3rdPersonSphere.Center.Z -= LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[num].Z;
			tmp3rdPersonContianment = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].bFrustum[num].Contains(tmp3rdPersonSphere);
			if (tmp3rdPersonContianment == ContainmentType.Contains || tmp3rdPersonContianment == ContainmentType.Intersects)
			{
				RenderRagdoll[num] = true;
			}
			else
			{
				RenderRagdoll[num] = false;
			}
		}
		if (mRagdoll.SetRagdoll)
		{
			Matrix world = mat3rdPaerson;
			world.Translation = world.Translation + vecPosition + CoOpOffset;
			int q = ((num == 0) ? 1 : 0);
			mRagdoll.ResetSkinData(character, currentCharacterIndex);
			mRagdoll.Spawn(world, cPlayer.GetBoneTransforms(num), cPlayer.GetBoneTransforms(q));
		}
	}

	public void CalculateLightMatrices(int qIndex)
	{
		float num = 256f;
		float num2 = 12500f;
		if (ApocalypseZ_Hack)
		{
			lightDirection = Vector3.Zero;
			lightDirection.X = LevelOutside.SunPosition.X;
			lightDirection.Y = LevelOutside.SunPosition.Y;
			lightDirection.Z = LevelOutside.SunPosition.Z;
			lightDirection.Normalize();
			if (ThirdPersonCamera)
			{
				lightViewDir = CameraDirection * 100f;
				lightViewDir.Y = 0f;
				lightViewDir.Normalize();
			}
			else
			{
				lightViewDir = vecFlatDirection;
			}
			lightLookAt.X = 0f;
			lightLookAt.Z = 0f;
			lightLookAt.Y = vecPosition.Y;
			lightLookAt += lightViewDir * lookAtDis;
			lightPosition = lightLookAt + lightDirection * num2;
			matLightView = Matrix.CreateLookAt(lightPosition, lightLookAt, Vector3.UnitY);
			ref Matrix reference = ref mDataQueue[qIndex].lightView2[0];
			reference = matLightView;
			lightLookAt.X = 0f;
			lightLookAt.Z = 0f;
			lightLookAt.Y = vecPosition.Y;
			lightLookAt += lightViewDir * lookAtDis2;
			lightPosition = lightLookAt + lightDirection * num2;
			matLightView = Matrix.CreateLookAt(lightPosition, lightLookAt, Vector3.UnitY);
			ref Matrix reference2 = ref mDataQueue[qIndex].lightView2[1];
			reference2 = matLightView;
			shadowScale = 2f;
			float zFarPlane = num2 * 1.25f;
			float zNearPlane = num2 * 0.25f;
			matLightProj = Matrix.CreateOrthographicOffCenter(0f - num * shadowScale, num * shadowScale, num * shadowScale, 0f - num * shadowScale, zNearPlane, zFarPlane);
			ref Matrix reference3 = ref mDataQueue[qIndex].lightProj2[0];
			reference3 = matLightProj;
			num = 256f;
			matLightProj = Matrix.CreateOrthographicOffCenter(0f - num * scaleX, num * scaleX, num * scaleY, 0f - num * scaleY, zNearPlane, zFarPlane);
			if (ThirdPersonCamera)
			{
				matLightProj *= Matrix.CreateRotationZ(MathHelper.ToRadians(CameraAngles.X));
			}
			else
			{
				matLightProj *= Matrix.CreateRotationZ(MathHelper.ToRadians(Angles.X));
			}
			ref Matrix reference4 = ref mDataQueue[qIndex].lightProj2[1];
			reference4 = matLightProj;
		}
		else
		{
			lightDirection = Vector3.Zero;
			lightDirection.X = LevelOutside.SunPosition.X - vecPosition.X;
			lightDirection.Y = LevelOutside.SunPosition.Y - vecPosition.Y;
			lightDirection.Z = LevelOutside.SunPosition.Z - vecPosition.Z;
			lightDirection *= -1f;
			lightDirection.Normalize();
			lightLookAt = vecPosition;
			lightPosition = lightLookAt + lightDirection * (0f - num2);
			matLightView = Matrix.CreateLookAt(lightPosition, lightLookAt, Vector3.UnitY);
			float zFarPlane2 = num2 * 2f;
			float zNearPlane2 = num2 * 0f;
			matLightProj = Matrix.CreateOrthographicOffCenter(0f - num * 4.5f, num * 4.5f, num * 4.5f, 0f - num * 4.5f, zNearPlane2, zFarPlane2);
		}
	}

	public virtual void Draw(int qIndex)
	{
		if (MenuState != PlayerMenuState.InGame || !Spawned)
		{
			return;
		}
		fpsWeapon.DrawRockets(qIndex, this);
		if (OverrideCamera || OverridePosition)
		{
			return;
		}
		if (FPSGameMenu.isVisable || Guide.IsVisible)
		{
			if (!ThirdPersonCamera)
			{
				fpsWeapon.Draw(0, this);
			}
		}
		else if (!ThirdPersonCamera)
		{
			fpsWeapon.Draw(qIndex, this);
		}
	}

	public virtual void DrawPostLens(int qIndex)
	{
		if (MenuState != PlayerMenuState.InGame || !Spawned)
		{
			return;
		}
		if (FPSGameMenu.isVisable || Guide.IsVisible)
		{
			fpsWeapon.DrawPostLens(0, this);
			return;
		}
		fpsWeapon.DrawPostLens(qIndex, this);
		if (drawHalographicUI)
		{
			int bone = 5;
			Matrix m = Matrix.Identity;
			fpsWeapon.fpsAmin.GetWorldTransformBlend(qIndex, bone, out m);
			math.RemoveScaling(ref m);
			Matrix identity = Matrix.Identity;
			identity = Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f);
			identity *= tmpPlayerScale;
			identity *= m;
			if (fpsWeapon.CurrentWeapon.WepType == WeaponType.AlienPistol)
			{
				tmpUIWeaponOffset = Vector3.Lerp(tmpUIWeaponOffset, sightedPistolOffset, 0.35f);
			}
			else if (!Sighted)
			{
				tmpUIWeaponOffset = Vector3.Lerp(tmpUIWeaponOffset, Vector3.Zero, 0.35f);
			}
			else if (fpsWeapon.CurrentWeapon.WepType == WeaponType.AlienSniper)
			{
				tmpUIWeaponOffset = Vector3.Lerp(tmpUIWeaponOffset, sightedSniperOffset, 0.35f);
			}
			else
			{
				tmpUIWeaponOffset = Vector3.Lerp(tmpUIWeaponOffset, sightedOffset, 0.35f);
			}
			tmpUIWeaponTransSave = identity.Translation;
			identity.Translation = Vector3.Zero;
			identity.Translation = Vector3.Transform(tmpUIWeaponOffset, identity) + tmpUIWeaponTransSave;
			Matrix matVP = mDataQueue[qIndex].view;
			matVP *= fpsWeapon.WeaponProjection[qIndex];
			UIHalographic.matWorld[qIndex] = identity;
			UIHalographic.Draw(ref matVP, qIndex, this);
		}
	}

	public virtual void DrawMuzzleFlash(int qIndex)
	{
		if (MenuState != PlayerMenuState.InGame || !Spawned)
		{
			return;
		}
		if (FPSGameMenu.isVisable || Guide.IsVisible)
		{
			if (DrawMuzzleFlashAlpha > 0f)
			{
				fpsWeapon.DrawMuzzleFlash(0, this, DrawMuzzleFlashAlpha);
				DrawMuzzleFlashAlpha -= EndGameEngine.currentTimeStep * 30f;
			}
		}
		else if (DrawMuzzleFlashAlpha > 0f)
		{
			fpsWeapon.matMuzzleFlash = Matrix.Identity;
			fpsWeapon.DrawMuzzleFlash(qIndex, this, DrawMuzzleFlashAlpha);
			DrawMuzzleFlashAlpha -= EndGameEngine.currentTimeStep * 30f;
		}
	}

	public virtual void DrawNetMuzzleFlash(int qIndex)
	{
		if (Spawned && DrawMuzzleFlashAlpha > 0f)
		{
			if (Render3rdPerson[qIndex])
			{
				fpsWeapon.DrawMuzzleFlash(0, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], DrawMuzzleFlashAlpha);
			}
			DrawMuzzleFlashAlpha -= EndGameEngine.currentTimeStep * 30f;
		}
	}

	public void DrawPost(int qIndex, Texture2D scene, Texture2D bloom)
	{
		_ = (float)vpViewPort.TitleSafeArea.Width / (float)EndGameEngine.GameSettings.RenderTargetSizeX;
		if (unlockMessageTimer > 0f)
		{
			unlockMessageTimer -= EndGameEngine.fFIXED_TIME_STEP;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, unlockMessageStr, new Vector2(642f - unlockMsgXOffset, 202f), new Color(0, 0, 0, 200));
			Menu.spriteBatch.DrawString(Menu.defaultFont, unlockMessageStr, new Vector2(640f - unlockMsgXOffset, 200f), new Color(0, 200, 0, 200));
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Unlocked!", new Vector2(582f, 238f), new Color(0, 0, 0, 200));
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Unlocked!", new Vector2(580f, 236f), new Color(0, 200, 0, 200));
			Menu.spriteBatch.End();
		}
		if (MatchCoolDownTimer < 0f && (!(DeathTimer <= 0f) || !(RespawnTimer < RESPAWN_TIME)) && MenuState == PlayerMenuState.InGame)
		{
			fpsWeapon.DrawScope(qIndex, this, scene, bloom);
		}
	}

	public void DrawPlayer(int qIndex, PlayerBase viewer)
	{
		if (!IsAttached0 && (PlayerFlags & FPS_NET_FLAGS.Spawned) > FPS_NET_FLAGS.Clear)
		{
			drawtmpMatrix = Matrix.Identity;
			GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
			graphicsDevice.BlendState = BlendState.Opaque;
			graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			drawtmpMatrix.Translation = CoOpOffset;
			if (ApocalypseZ_Hack)
			{
				tmpPos = vecPosition + CoOpOffset;
				tmpPos.X -= viewer.vecHeadPosition[qIndex].X;
				tmpPos.Z -= viewer.vecHeadPosition[qIndex].Z;
				drawtmpMatrix.Translation = tmpPos;
			}
			for (int i = 0; i < character.Meshes.Count; i++)
			{
				drawMesh = character.Meshes[i];
				for (int j = 0; j < drawMesh.MeshParts.Count; j++)
				{
					drawMeshPart = drawMesh.MeshParts[j];
					if (drawMesh.Name.Contains("shirt"))
					{
						if (ShirtIndex < 3f)
						{
							drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse1);
							drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal1);
							vecTexOffset.X = ShirtIndex * 0.3333f;
							drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
						}
						else
						{
							drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse2);
							drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal2);
							vecTexOffset.X = (ShirtIndex - 3f) * 0.3333f;
							drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
						}
					}
					else if (drawMesh.Name.Contains("pants"))
					{
						if (PantstIndex < 3f)
						{
							drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse1);
							drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal1);
							vecTexOffset.X = PantstIndex * 0.3333f;
							drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
						}
						else
						{
							drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse2);
							drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal2);
							vecTexOffset.X = (PantstIndex - 3f) * 0.3333f;
							drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
						}
					}
					else
					{
						vecTexOffset.X = 0f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
					drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
					characterEffects[currentCharacterIndex * 7 + i].matBones.SetValue(cPlayer.GetSkinTransforms(qIndex));
					characterEffects[currentCharacterIndex * 7 + i].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
					characterEffects[currentCharacterIndex * 7 + i].matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
					drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					drawMeshPart.Effect.CurrentTechnique.Passes[4].Apply();
					drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
			drawtmpRight = Matrix.Identity;
			cPlayer.GetWorldTransformBlend(qIndex, 16, out drawtmpRight);
			math.RemoveScaling(ref drawtmpRight);
			drawtmpRight.Translation = drawtmpRight.Translation + vecPosition + CoOpOffset;
			fpsWeapon.DrawPlayerWeapon(qIndex, this, viewer, drawtmpRight, MuzzleHeat);
		}
		if (mRagdoll.IsValid)
		{
			DrawRagdoll(qIndex, viewer);
		}
	}

	public void DrawPlayerShadow(PlayerBase viewer, ref Matrix lightViewProj, ref Vector3 lightPos, int qIndex)
	{
		if (IsAttached0 || (PlayerFlags & FPS_NET_FLAGS.Spawned) <= FPS_NET_FLAGS.Clear)
		{
			return;
		}
		drawtmpMatrix = Matrix.CreateScale(1.07f);
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		drawtmpMatrix.Translation = CoOpOffset;
		if (ApocalypseZ_Hack)
		{
			tmpPos = vecPosition + CoOpOffset;
			tmpPos.X -= viewer.vecHeadPosition[qIndex].X;
			tmpPos.Z -= viewer.vecHeadPosition[qIndex].Z;
			drawtmpMatrix.Translation = tmpPos;
		}
		for (int i = 0; i < character.Meshes.Count; i++)
		{
			drawMesh = character.Meshes[i];
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(lightPos);
				characterEffects[currentCharacterIndex * 7 + i].matBones.SetValue(cPlayer.GetSkinTransforms(qIndex));
				characterEffects[currentCharacterIndex * 7 + i].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
				characterEffects[currentCharacterIndex * 7 + i].matViewProj.SetValue(lightViewProj);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				drawMeshPart.Effect.CurrentTechnique.Passes[5].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public void DrawNetPlayer(int qIndex, PlayerBase viewer)
	{
		if (!IsAttached0 && Spawned)
		{
			drawtmpMatrix = Matrix.CreateScale(1.082f);
			drawtmpMatrix = Matrix.Identity;
			GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
			graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
			tmpPos = vecPosition + CoOpOffset;
			tmpPos.X -= viewer.vecHeadPosition[qIndex].X;
			tmpPos.Z -= viewer.vecHeadPosition[qIndex].Z;
			drawtmpMatrix.Translation = tmpPos;
			if (Render3rdPerson[qIndex])
			{
				for (int i = 0; i < character.Meshes.Count; i++)
				{
					drawMesh = character.Meshes[i];
					for (int j = 0; j < drawMesh.MeshParts.Count; j++)
					{
						drawMeshPart = drawMesh.MeshParts[j];
						if (drawMesh.Name.Contains("shirt"))
						{
							if (ShirtIndex < 3f)
							{
								drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse1);
								drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal1);
								vecTexOffset.X = ShirtIndex * 0.3333f;
								drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
							}
							else
							{
								drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse2);
								drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal2);
								vecTexOffset.X = (ShirtIndex - 3f) * 0.3333f;
								drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
							}
						}
						else if (drawMesh.Name.Contains("pants"))
						{
							if (PantstIndex < 3f)
							{
								drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse1);
								drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal1);
								vecTexOffset.X = PantstIndex * 0.3333f;
								drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
							}
							else
							{
								drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse2);
								drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal2);
								vecTexOffset.X = (PantstIndex - 3f) * 0.3333f;
								drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
							}
						}
						else
						{
							vecTexOffset.X = 0f;
							drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
						}
						drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
						characterEffects[currentCharacterIndex * 7 + i].matBones.SetValue(cPlayer.GetSkinTransforms(qIndex));
						characterEffects[currentCharacterIndex * 7 + i].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
						characterEffects[currentCharacterIndex * 7 + i].matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
						drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						drawMeshPart.Effect.CurrentTechnique.Passes[4].Apply();
						drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					}
				}
			}
			if (Render3rdPerson[qIndex] && currentWeaponType != WeaponType.EmptyHands)
			{
				drawtmpRight = Matrix.Identity;
				cPlayer.GetWorldTransformBlend(qIndex, 16, out drawtmpRight);
				math.RemoveScaling(ref drawtmpRight);
				drawtmpRight.Translation = drawtmpRight.Translation + vecPosition + CoOpOffset;
				fpsWeapon.CurWeaponType = currentWeaponType;
				fpsWeapon.DrawPlayerWeapon(qIndex, this, viewer, drawtmpRight, MuzzleHeat);
			}
			else
			{
				drawtmpRight = Matrix.Identity;
				drawtmpRight.Translation = vecPosition + CoOpOffset;
			}
			vec3rdPersonMuzzlePos = (fpsWeapon.CurrentWeapon.GetBoneTransform(WeaponPart.Muzzle) * drawtmpRight).Translation;
		}
		if (mRagdoll.IsValid)
		{
			DrawRagdoll(qIndex, viewer);
		}
	}

	public void DrawFlashLightGlare(int qIndex, PlayerBase viewer)
	{
		if (FlashLightOn && !IsAttached0)
		{
			int num = 7;
			ref Matrix reference = ref EquipmentCls.itemsModels[num].matWorld[qIndex];
			reference = matFlashLight[qIndex];
			EquipmentCls.itemsModels[num].DrawCameraSpaceAlpha(viewer, qIndex, 1f);
		}
	}

	public void DrawRagdoll(int qIndex, PlayerBase viewer)
	{
		if (!RenderRagdoll[qIndex])
		{
			return;
		}
		int num = mRagdoll.currentCharacterIndex;
		drawtmpMatrix = Matrix.Identity;
		if (ApocalypseZ_Hack)
		{
			drawtmpMatrix.Translation = new Vector3(0f - viewer.vecHeadPosition[qIndex].X, 0f, 0f - viewer.vecHeadPosition[qIndex].Z);
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		for (int i = 0; i < mRagdoll.currentCharacter.Meshes.Count; i++)
		{
			drawMesh = mRagdoll.currentCharacter.Meshes[i];
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				characterEffects[num * 7 + i].matBones.SetValue(mRagdoll.RagdollSkinPose);
				if (drawMesh.Name.Contains("shirt"))
				{
					if (ShirtIndex < 3f)
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse1);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal1);
						vecTexOffset.X = ShirtIndex * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
					else
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse2);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal2);
						vecTexOffset.X = (ShirtIndex - 3f) * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
				}
				else if (drawMesh.Name.Contains("pants"))
				{
					if (PantstIndex < 3f)
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse1);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal1);
						vecTexOffset.X = PantstIndex * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
					else
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse2);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal2);
						vecTexOffset.X = (PantstIndex - 3f) * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
				}
				else
				{
					vecTexOffset.X = 0f;
					drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
				}
				characterEffects[num * 7 + i].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
				characterEffects[num * 7 + i].matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
				Vector3 value = Vector3.Transform(-viewer.mDataQueue[qIndex].view.Translation, Matrix.Transpose(viewer.mDataQueue[qIndex].view));
				drawMeshPart.Effect.Parameters["vecEyePosition"].SetValue(value);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				drawMeshPart.Effect.CurrentTechnique.Passes[4].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public void DrawMenuPlayer(float yaw)
	{
		Viewport viewport = new Viewport(LevelBaseMenu.DiffuseRenderTarget.Bounds);
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = viewport;
		drawtmpMatrix = Matrix.Identity;
		Vector3 value = new Vector3(-2000f, 2000f, 5000f);
		Vector4 value2 = new Vector4(1f, 1f, 1f, 1f);
		Vector4 value3 = new Vector4(0.2f, 0.2f, 0.2f, 1f);
		menuProj = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1.7777778f, NearZPlane, FarZPlane);
		Vector3 cameraPosition = new Vector3(0f, 40f, 210f);
		menuView = Matrix.CreateLookAt(cameraPosition, new Vector3(0f, 30f, 0f), Vector3.UnitY);
		for (int i = 0; i < character.Meshes.Count; i++)
		{
			drawMesh = character.Meshes[i];
			for (int j = 0; j < drawMesh.MeshParts.Count; j++)
			{
				drawMeshPart = drawMesh.MeshParts[j];
				if (drawMesh.Name.Contains("shirt"))
				{
					if (ShirtIndex < 3f)
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse1);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal1);
						vecTexOffset.X = ShirtIndex * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
					else
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorShirtDiffuse2);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorShirtNormal2);
						vecTexOffset.X = (ShirtIndex - 3f) * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
				}
				else if (drawMesh.Name.Contains("pants"))
				{
					if (PantstIndex < 3f)
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse1);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal1);
						vecTexOffset.X = PantstIndex * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
					else
					{
						drawMeshPart.Effect.Parameters["TexDiffuse"].SetValue(SurvivorPantsDiffuse2);
						drawMeshPart.Effect.Parameters["TexNormal"].SetValue(SurvivorPantsNormal2);
						vecTexOffset.X = (PantstIndex - 3f) * 0.3333f;
						drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
					}
				}
				else
				{
					vecTexOffset.X = 0f;
					drawMeshPart.Effect.Parameters["vecTexOffset"].SetValue(vecTexOffset);
				}
				characterEffects[currentCharacterIndex * 7 + i].vecLightPosition.SetValue(value);
				characterEffects[currentCharacterIndex * 7 + i].fSpecularPower.SetValue(2f);
				characterEffects[currentCharacterIndex * 7 + i].vecLightColor.SetValue(value2);
				characterEffects[currentCharacterIndex * 7 + i].vecAmbientLightColor.SetValue(value3);
				characterEffects[currentCharacterIndex * 7 + i].matBones.SetValue(cPlayer.GetSkinTransforms(0));
				characterEffects[currentCharacterIndex * 7 + i].matSkinnedWorldTransform.SetValue(drawtmpMatrix);
				characterEffects[currentCharacterIndex * 7 + i].matView.SetValue(menuView);
				characterEffects[currentCharacterIndex * 7 + i].matProj.SetValue(menuProj);
				drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
				drawMeshPart.Effect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
				drawMeshPart.Effect.CurrentTechnique.Passes[3].Apply();
				drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
			}
		}
	}

	public void DrawWeaponPreview(WeaponType e)
	{
		WeaponType curWeaponType = fpsWeapon.CurWeaponType;
		fpsWeapon.SetWeapon(e);
		Vector3 lightPos = new Vector3(-2000f, 2000f, 5000f);
		Vector4 lightColor = new Vector4(1f, 1f, 1f, 1f);
		Vector4 ambientColor = new Vector4(0.6f, 0.6f, 0.6f, 1f);
		menuProj = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1.7777778f, NearZPlane, FarZPlane);
		menuView = Matrix.CreateLookAt(new Vector3(-20f, 50f, 330f), new Vector3(-20f, 0f, 0f), Vector3.UnitY);
		Matrix m = Matrix.Identity;
		cPlayer.GetWorldTransformBlend(0, 16, out m);
		math.RemoveScaling(ref m);
		m.Translation = m.Translation;
		fpsWeapon.DrawWeaponPreviewMenu(0, this, m, lightColor, ambientColor, lightPos);
		fpsWeapon.SetWeapon(curWeaponType);
	}

	public void DrawAttachmentPreview(WeaponAttachment e)
	{
		Vector3 lightPos = new Vector3(-2000f, 2000f, 5000f);
		Vector4 lightColor = new Vector4(1f, 1f, 1f, 1f);
		Vector4 ambientColor = new Vector4(0.6f, 0.6f, 0.6f, 1f);
		Vector3 cameraPosition = new Vector3(-20f, 50f, 330f);
		Vector3 cameraTarget = new Vector3(-20f, 0f, 0f);
		menuView = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.UnitY);
		menuProj = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1.7777778f, NearZPlane, FarZPlane);
		if (e == WeaponAttachment.NadeLauncher)
		{
			fpsWeapon.DrawM203PreviewMenu(0, this, lightColor, ambientColor, lightPos);
			return;
		}
		WeaponType curWeaponType = fpsWeapon.CurWeaponType;
		WeaponAttachment attachment = fpsWeapon.CurrentWeapon.Attachment;
		fpsWeapon.SetWeapon(WeaponType.European);
		fpsWeapon.CurrentWeapon.Attachment = e;
		fpsWeapon.DrawAttachmentPreviewMenu(0, this, lightColor, ambientColor, lightPos);
		fpsWeapon.SetWeapon(curWeaponType);
		fpsWeapon.CurrentWeapon.Attachment = attachment;
	}

	public virtual void UpdatePlayerMove(float eTime, int qIndex)
	{
		if (!EndGameEngine.GameSettings.EnableCollision || OverrideCamera || OverridePosition || OverrideInput)
		{
			return;
		}
		float num = 16f;
		float num2 = 0f;
		if (Stance == PlayerStance.Idle)
		{
			NoiseLevel = 0f;
			VisibleLevel = 0.7f;
		}
		else if (Stance == PlayerStance.Crouch)
		{
			if (Speed > 0f)
			{
				NoiseLevel = 0.002f;
				VisibleLevel = 0.4f;
			}
		}
		else if (Stance == PlayerStance.Walk)
		{
			NoiseLevel = 0.00925f;
			VisibleLevel = 0.8f;
		}
		else if (Stance == PlayerStance.Run)
		{
			NoiseLevel = 0.05f;
			VisibleLevel = 1f;
		}
		if (OverrideLevelOutsideCollision)
		{
			if (!onWalkable && JumpYTime <= 0.5f)
			{
				GravityAccel += 16f * EndGameEngine.fFIXED_TIME_STEP;
			}
			if (JumpYTime > 0.5f)
			{
				vecPosition.Y += JumpYValue * 1.5f;
			}
			num = 16f;
			if (JumpYTime > 0f)
			{
				num = 16f;
			}
			tmpPos = vecPosition;
			tmpPos.Y -= GravityAccel;
			tmpLastPos = tmpPrevPosition;
			tmpLastPos.Y -= GravityAccel;
			tmpVecMovement = tmpPos - tmpLastPos;
			num2 = tmpVecMovement.LengthSquared();
			if (num2 > 676f)
			{
				tmpVecMovement.Normalize();
				tmpPos = tmpLastPos + tmpVecMovement * 26f;
			}
			Vector3.Lerp(ref vecPosition, ref tmpPos, 0.75f, out vecPosition);
			return;
		}
		if (JumpYTime > 0.5f)
		{
			vecPosition.Y += JumpYValue;
		}
		num = 16f;
		if (JumpYTime > 0f)
		{
			num = 16f;
		}
		tmpPos = vecPosition;
		tmpPos.Y -= num;
		tmpPos.Y -= GravityAccel;
		tmpLastPos = tmpPrevPosition;
		tmpLastPos.Y -= num;
		tmpLastPos.Y -= GravityAccel;
		fKnifeThust = MathHelper.Lerp(fKnifeThust, 0f, eTime * 10f);
		if (fKnifeThust > 1f)
		{
			SegmentParams.SegmentDirection = vecDirection;
			SegmentParams.SegmentLength = 1000f;
			SegmentParams.SegmentDirection.Y = 0f;
			SegmentParams.SegmentDirection.Normalize();
			SegmentParams.OnlyWalkable = true;
			SegmentParams.SegmentStart = tmpLastPos;
			SegmentParams.SegmentEnd = tmpLastPos + SegmentParams.SegmentDirection * SegmentParams.SegmentLength;
			SegmentParams.PreComputeParameters();
			LevelOutside.RayCast(qIndex, ref SegmentParams, spawnSparks: false);
			if (LevelOutside.RaycastHitDistance > 120f)
			{
				tmpPos = tmpLastPos + SegmentParams.SegmentDirection * (20f - fKnifeThust);
			}
		}
		tmpCollision.onWalkable = false;
		tmpCollision.hitTrigger = CurrentTrigger;
		tmpSphere.Center = tmpPos;
		tmpSphere.Radius = 28f;
		tmpVecMovement = tmpPos - tmpLastPos;
		num2 = tmpVecMovement.LengthSquared();
		if (num2 > 676f)
		{
			tmpVecMovement.Normalize();
			tmpSphere.Center = tmpLastPos + tmpVecMovement * 26f;
		}
		LevelOutside.IntersectFPSCharacter(ref tmpSphere, ref tmpLastPos, ref tmpCollision, Stance == PlayerStance.Crouch, num);
		CurrentTrigger = tmpCollision.hitTrigger;
		if (CurrentTrigger == TriggerTypes.Ladder || CurrentTrigger == TriggerTypes.LadderBottom)
		{
			tmpPos.Y += num;
			if (CurrentTrigger == TriggerTypes.Ladder)
			{
				tmpPos.X = MathHelper.Lerp(tmpPos.X, tmpCollision.hitPosition.X, eTime * 5.5f);
				tmpPos.Z = MathHelper.Lerp(tmpPos.Z, tmpCollision.hitPosition.Z, eTime * 5.5f);
			}
			GravityAccel = 0f;
			return;
		}
		tmpPos = tmpSphere.Center;
		tmpPos.Y += num;
		if ((tmpCollision.onWalkable || !EndGameEngine.GameSettings.EnableGravity || !(JumpYTime <= 0.5f)) && tmpCollision.onWalkable)
		{
			if (!onWalkable && GravityAccel > 8f)
			{
				fpsWeapon.vecHeadSwayTarget.X = GravityAccel / 75f;
			}
			onWalkable = true;
			GravityAccel = 0f;
		}
		if (!tmpCollision.onWalkable)
		{
			onWalkable = tmpCollision.onWalkable;
		}
	}

	public void ApplyDamage(int damage)
	{
		mRagdoll.SetRagdoll = true;
	}

	public void ApplyDamageLocal(int damage)
	{
		if (BloodLevel > 0f)
		{
			if (damage == 6 && BloodLoss < 0.2f && EndGameEngine.randGenerator.Next(0, 100) < 75)
			{
				BloodLoss += 0.025f;
			}
			BloodLevel = ((BloodLevel - (float)damage >= 0f) ? (BloodLevel - (float)damage) : 0f);
			if (EGENetWorkNext.networkSession != null)
			{
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)129);
				packetWriter.Write(NetGamerRef.Id);
				packetWriter.Write(damage);
				packetWriter.Write((BloodLoss > 0f) ? ((byte)1) : ((byte)0));
			}
		}
	}

	public virtual int RayCast(Vector3 origin, ref Vector3 direction, ref DamegePacketType damageType, float scaling)
	{
		int num = 0;
		Matrix worldTran = Matrix.Identity;
		worldTran.Translation = vecPosition + CoOpOffset;
		damageType = DamegePacketType.None;
		Matrix[] skinTransforms = cPlayer.GetSkinTransforms(0);
		num = physics.RayCast(ref origin, ref direction, ref tmpHitPosition, ref worldTran, skinTransforms, scaling);
		if (Health > 0f && num > 0)
		{
			particles.SpawnBulletHitMutant(ref tmpHitPosition, ref direction);
			damageType = DamegePacketType.Body;
			if (BotPhysics.LastHitWasHeadShot)
			{
				BotPhysics.LastHitWasHeadShot = false;
				damageType = DamegePacketType.HeadShot;
			}
		}
		return num;
	}

	private void SetupReticle()
	{
		Vector3 position = new Vector3(-0.5f, 0.1f, 0f) * 4f;
		Vector3 position2 = new Vector3(0.5f, 0.1f, 0f) * 4f;
		Vector3 position3 = new Vector3(-0.5f, -0.1f, 0f) * 4f;
		Vector3 position4 = new Vector3(0.5f, -0.1f, 0f) * 4f;
		Vector3 position5 = new Vector3(-0.1f, 0.5f, 0f) * 4f;
		Vector3 position6 = new Vector3(0.1f, 0.5f, 0f) * 4f;
		Vector3 position7 = new Vector3(-0.1f, -0.5f, 0f) * 4f;
		Vector3 position8 = new Vector3(0.1f, -0.5f, 0f) * 4f;
		Vector2 texCoord = new Vector2(0f, 0f);
		Vector2 texCoord2 = new Vector2(1f, 0f);
		Vector2 texCoord3 = new Vector2(0f, 1f);
		Vector2 texCoord4 = new Vector2(1f, 1f);
		reticleVertices = new VS_ReticleStruct[22];
		ref VS_ReticleStruct reference = ref reticleVertices[0];
		reference = new VS_ReticleStruct(position, 0f, texCoord);
		ref VS_ReticleStruct reference2 = ref reticleVertices[1];
		reference2 = new VS_ReticleStruct(position2, 0f, texCoord2);
		ref VS_ReticleStruct reference3 = ref reticleVertices[2];
		reference3 = new VS_ReticleStruct(position3, 0f, texCoord3);
		ref VS_ReticleStruct reference4 = ref reticleVertices[3];
		reference4 = new VS_ReticleStruct(position4, 0f, texCoord4);
		ref VS_ReticleStruct reference5 = ref reticleVertices[4];
		reference5 = new VS_ReticleStruct(position4, 0f, texCoord4);
		ref VS_ReticleStruct reference6 = ref reticleVertices[5];
		reference6 = new VS_ReticleStruct(position, 1f, texCoord);
		ref VS_ReticleStruct reference7 = ref reticleVertices[6];
		reference7 = new VS_ReticleStruct(position, 1f, texCoord);
		ref VS_ReticleStruct reference8 = ref reticleVertices[7];
		reference8 = new VS_ReticleStruct(position2, 1f, texCoord2);
		ref VS_ReticleStruct reference9 = ref reticleVertices[8];
		reference9 = new VS_ReticleStruct(position3, 1f, texCoord3);
		ref VS_ReticleStruct reference10 = ref reticleVertices[9];
		reference10 = new VS_ReticleStruct(position4, 1f, texCoord4);
		ref VS_ReticleStruct reference11 = ref reticleVertices[10];
		reference11 = new VS_ReticleStruct(position4, 1f, texCoord4);
		ref VS_ReticleStruct reference12 = ref reticleVertices[11];
		reference12 = new VS_ReticleStruct(position5, 2f, texCoord);
		ref VS_ReticleStruct reference13 = ref reticleVertices[12];
		reference13 = new VS_ReticleStruct(position5, 2f, texCoord);
		ref VS_ReticleStruct reference14 = ref reticleVertices[13];
		reference14 = new VS_ReticleStruct(position6, 2f, texCoord2);
		ref VS_ReticleStruct reference15 = ref reticleVertices[14];
		reference15 = new VS_ReticleStruct(position7, 2f, texCoord3);
		ref VS_ReticleStruct reference16 = ref reticleVertices[15];
		reference16 = new VS_ReticleStruct(position8, 2f, texCoord4);
		ref VS_ReticleStruct reference17 = ref reticleVertices[16];
		reference17 = new VS_ReticleStruct(position8, 2f, texCoord4);
		ref VS_ReticleStruct reference18 = ref reticleVertices[17];
		reference18 = new VS_ReticleStruct(position5, 3f, texCoord);
		ref VS_ReticleStruct reference19 = ref reticleVertices[18];
		reference19 = new VS_ReticleStruct(position5, 3f, texCoord);
		ref VS_ReticleStruct reference20 = ref reticleVertices[19];
		reference20 = new VS_ReticleStruct(position6, 3f, texCoord2);
		ref VS_ReticleStruct reference21 = ref reticleVertices[20];
		reference21 = new VS_ReticleStruct(position7, 3f, texCoord3);
		ref VS_ReticleStruct reference22 = ref reticleVertices[21];
		reference22 = new VS_ReticleStruct(position8, 3f, texCoord4);
		reticleVertexBuff = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, VS_ReticleStruct.VertexDeclaration, 22, BufferUsage.None);
		reticleVertexBuff.SetData(reticleVertices);
	}

	public void SetReticleDrawParameters(float matScale, float minSize, float maxSize)
	{
		reticleScale = matScale;
		reticleMinSize = minSize;
		reticleMaxSize = maxSize;
		reticleMat = Matrix.CreateScale(reticleScale, reticleScale, reticleScale);
		tmpA[0].X = 0f - reticleMinSize;
		tmpA[1].X = reticleMinSize;
		tmpA[2].Y = 0f - reticleMinSize;
		tmpA[3].Y = reticleMinSize;
		tmpB[0].X = 0f - reticleMaxSize;
		tmpB[1].X = reticleMaxSize;
		tmpB[2].Y = 0f - reticleMaxSize;
		tmpB[3].Y = reticleMaxSize;
	}

	public void DrawReticle(float aspecRatio, PlayerBase playerRef)
	{
		_ = EndGameEngine.GraphicMgr.GraphicsDevice;
		reticleMat.M22 = aspecRatio * reticleScale;
		float num = 1f + (1f - playerRef.fpsWeapon.BulletAccuracy);
		float num2 = ((playerRef.Speed > playerRef.SideStep) ? playerRef.Speed : playerRef.SideStep) * 0.2f;
		num2 = ((num2 < 0f) ? 0f : num2);
		ref Vector2 reference = ref tmpC[0];
		reference = tmpA[0] + tmpB[0] * num2 + tmpB[0] * num;
		ref Vector2 reference2 = ref tmpC[1];
		reference2 = tmpA[1] + tmpB[1] * num2 + tmpB[1] * num;
		ref Vector2 reference3 = ref tmpC[2];
		reference3 = tmpA[2] + tmpB[2] * num2 + tmpB[2] * num;
		ref Vector2 reference4 = ref tmpC[3];
		reference4 = tmpA[3] + tmpB[3] * num2 + tmpB[3] * num;
		Effect materialEffect = EndGameEngine.MaterialEffect;
		EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
		GraphicsDevice graphicsDevice = materialEffect.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(reticleVertexBuff);
		materialEffect.CurrentTechnique = materialParams.T_DrawReticle;
		materialParams.matWorld.SetValue(reticleMat);
		materialParams.reticlePosition.SetValue(tmpC);
		materialEffect.CurrentTechnique.Passes[0].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 20);
	}

	private void SpecialOperations()
	{
		IsModerator = ((base.gamerTag == "mgKelley" || base.gamerTag == "M Zacatecas X") ? true : false);
		IsModerator = base.gamerTag == "ImOldFrank" || base.gamerTag == "SKreationDEV2" || IsModerator;
	}
}
