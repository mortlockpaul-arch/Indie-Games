using System;
using System.Collections.Generic;
using DataContent;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace GameEngine;

public class SimpleZombieAI : AIBase
{
	private const int MaxCollisionStep = 12;

	private const float MIN_DIS_BOTS = 32f;

	private const float MIN_DISSQR_BOTS = 1024f;

	private static Texture2D HitIndicator;

	private static Texture2D CameraLensScope;

	private static Texture2D CameraRecMsg;

	private static Texture2D DragunovScope;

	private static Texture2D Home2DIcon;

	private static Texture2D GasStation2DIcon;

	private static Texture2D Gas2DIcon;

	private static Texture2D CompassDial;

	private static botLOS BotsLOSCls = new botLOS();

	public static int NumberBotsCurrentAlive = 0;

	public static int CurrentWave = 0;

	public static int MaxBotThisWave = 100;

	public static int NumBotKilledThisWave = 0;

	public static int TotalMonies = 2;

	public static InstancePropsManager InstanceProps = new InstancePropsManager();

	public static List<WaveMechanics> WaveMechs;

	public static List<WorldAreaCls> MobileHomeParks;

	private static Vector3[] FootPrintsHit = new Vector3[4];

	public static bool SleepingBag = false;

	public static bool CampingSupplies = false;

	public static bool DigitalCamera = false;

	public static bool HDCamRecorder = false;

	public static bool NVGoogles = false;

	public static bool ThermalCamera = false;

	public static bool HuntingRifle = false;

	public static bool OffRoadVehicle = false;

	public static bool BigfootDead = false;

	private Cue SwishHitSound;

	private static bool BotCollisionTest = true;

	private static int botGetsCollisionIndex = 0;

	private static Vector3 aiNormal = Vector3.Zero;

	private static Vector3 vecToOther = Vector3.Zero;

	private static Vector3 vecSteerAccum = Vector3.Zero;

	private static Vector3 tmpVec = Vector3.Zero;

	private static ZombieLODEntry[] removeZombiePositions = new ZombieLODEntry[38];

	private Vector3 tmpContactVec = Vector3.Zero;

	private Matrix[] tmpInstanceData = new Matrix[32];

	private static Matrix world = Matrix.CreateScale(0.7f);

	private int y;

	private float hourcounter;

	private float recTimer;

	private float blinktimer;

	private float weaponMagDispTimer;

	private Color wepStatShade = Color.Black;

	private Color wepStatDiff = Color.LightGray;

	private Color ammoTagColor = Color.Black;

	private Color blkFade = Color.White;

	private Vector2 uiPos = Vector2.Zero;

	private Vector2 shadowPos = new Vector2(2f, 2f);

	private Vector2 ProjDir = Vector2.Zero;

	private Vector3 headingVec = Vector3.Zero;

	private Rectangle compassRec = default(Rectangle);

	private Rectangle compassRecDst = default(Rectangle);

	private Rectangle recRecMsg = new Rectangle(200, 570, 128, 64);

	private float ballonTimer;

	private float ballonTimerDirection = 1f;

	private Vector3 projectedPosition = Vector3.Zero;

	private Vector2 screenPosition = Vector2.Zero;

	private Color icon2DColor = Color.Black;

	private Rectangle recGeneric = new Rectangle(0, 0, 160, 160);

	private static float recordTimer = -1f;

	private static float raycastHitDistance = float.MaxValue;

	private static Vector3 raycasHitPosition = Vector3.Zero;

	private static Vector3 raycasHitNormal = Vector3.Zero;

	private static Vector3 saveHitPosition = Vector3.Zero;

	private static Vector3 saveHitNormal = Vector3.Zero;

	private static Matrix raycastTmpMat = Matrix.Identity;

	private static Ray raycastRay = default(Ray);

	private static BoundingSphere tmpBSphere = default(BoundingSphere);

	private Vector3 tmpForceDir = Vector3.Zero;

	private static Vector3 nAngle = Vector3.Zero;

	private static Vector3 inDirection = Vector3.Zero;

	private Vector3 tmpPos = Vector3.Zero;

	private Vector3 tmpDir = Vector3.UnitZ;

	private float camBFZoom = 0.0005f;

	private Vector3 targetCamPos = Vector3.Zero;

	private Vector3 targetCamDir = -Vector3.UnitZ;

	private Vector3 zoomSwivelAt = Vector3.Zero;

	private static HalfVector2 readPHV2 = default(HalfVector2);

	private static HalfVector4 readPHV4 = default(HalfVector4);

	private static NormalizedByte4 readNB4_0 = default(NormalizedByte4);

	private static NormalizedByte4 readNB4_1 = default(NormalizedByte4);

	private static Vector2 readUnpackerV2 = Vector2.Zero;

	private static Vector4 readUnpackerV4 = Vector4.Zero;

	private static float sendZombieIntervalTimer = 0f;

	private Vector3 closestPntToPathing = Vector3.Zero;

	private static Vector3 dirToPlayer = Vector3.Zero;

	public override void Initialize()
	{
		if (AIBase.Initialized)
		{
			return;
		}
		base.Initialize();
		Vector3 position = Vector3.Zero;
		Vector3 normal = Vector3.Zero;
		AIBase.Initialized = true;
		AIBase.Owner = this;
		WaveMechs = EndGameEngine.GameAssetMgr.Load<List<WaveMechanics>>("data\\WaveMechanics");
		InstanceProps.Load();
		AIBase.LoadWorldItemAssets();
		AIStateMachine.allStates[15] = new ZombieHuntPlayer(AIBotStates.ZombieHuntPlayer);
		AIStateMachine.allStates[14] = new ZombieWander(AIBotStates.ZombieWander);
		AIStateMachine.allStates[16] = new ZombieSearchPlayer(AIBotStates.ZombieSearchPlayer);
		AIStateMachine.allStates[17] = new ZombieAttackPlayer(AIBotStates.ZombieAttackPlayer);
		AIStateMachine.allStates[18] = new ZombieHit(AIBotStates.ZombieHit);
		for (int i = 0; i < AIBase.MaxZombies; i++)
		{
			ZombieBot zombieBot = new ZombieBot(useWeapon: false);
			zombieBot._uid = BaseData.GetUniqueId;
			zombieBot.Health = 0;
			AIBase.AllZombies.Add(zombieBot);
		}
		HitIndicator = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\hitindicator");
		MobileHomeParks = new List<WorldAreaCls>();
		Matrix identity = Matrix.Identity;
		position.X = 33000f;
		position.Z = 18000f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 100f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls = new WorldAreaCls();
		worldAreaCls.NumZombieSpawns = 50;
		worldAreaCls.Load("TrailerPark00", identity, 128f, 800f);
		MobileHomeParks.Add(worldAreaCls);
		identity = Matrix.Identity;
		position.X = 26140f;
		position.Z = 1050f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) - 0f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls2 = new WorldAreaCls();
		worldAreaCls2.NumZombieSpawns = 50;
		worldAreaCls2.Load("Prosetin00", identity, 128f, 2048f);
		MobileHomeParks.Add(worldAreaCls2);
		identity = Matrix.Identity;
		position.X = 5654f;
		position.Z = -28980f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) - 0f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls3 = new WorldAreaCls();
		worldAreaCls3.NumZombieSpawns = 200;
		worldAreaCls3.Load("Borek00", identity, 128f, 1500f);
		MobileHomeParks.Add(worldAreaCls3);
		identity = Matrix.Identity;
		position.X = -41780f;
		position.Z = -48100f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) - 0f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls4 = new WorldAreaCls();
		worldAreaCls4.NumZombieSpawns = 30;
		worldAreaCls4.Load("HLinena00", identity, 128f, 1024f);
		MobileHomeParks.Add(worldAreaCls4);
		identity = Matrix.Identity;
		position.X = -50800f;
		position.Z = 24550f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 0f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls5 = new WorldAreaCls();
		worldAreaCls5.NumZombieSpawns = 120;
		worldAreaCls5.Load("Celsky00", identity, 128f, 1024f);
		MobileHomeParks.Add(worldAreaCls5);
		identity = Matrix.Identity;
		position.X = -31420f;
		position.Z = -31800f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 0f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls6 = new WorldAreaCls();
		worldAreaCls6.NumZombieSpawns = 100;
		worldAreaCls6.Load("Javory00", identity, 128f, 1024f);
		MobileHomeParks.Add(worldAreaCls6);
		identity = Matrix.Identity;
		position.X = -5950f;
		position.Z = -5470f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 0f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls7 = new WorldAreaCls();
		worldAreaCls7.NumZombieSpawns = 500;
		worldAreaCls7.Load("Poustka00", identity, 128f, 2000f);
		MobileHomeParks.Add(worldAreaCls7);
		identity = Matrix.Identity;
		position.X = -32000f;
		position.Z = -50000f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 0f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls8 = new WorldAreaCls();
		worldAreaCls8.NumZombieSpawns = 30;
		worldAreaCls8.Load("PlaneCrash00", identity, 128f, 1000f);
		MobileHomeParks.Add(worldAreaCls8);
		LevelOutside.Emitters = new eLevelEmitter[2];
		eLevelEmitter eLevelEmitter2 = new eLevelEmitter();
		eLevelEmitter2 = new eLevelEmitter();
		position.X = -30280f;
		position.Z = -51120f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 0f;
		eLevelEmitter2.Position = position;
		eLevelEmitter2.Name = "PlaneFire";
		eLevelEmitter2.Direction = Vector3.UnitY;
		eLevelEmitter2.eType = EmitterType.FireLooping;
		eLevelEmitter2.Scale = 280f;
		LevelOutside.Emitters[0] = eLevelEmitter2;
		eLevelEmitter2 = new eLevelEmitter();
		position.X = -28080f;
		position.Z = -49450f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 0f;
		eLevelEmitter2.Position = position;
		eLevelEmitter2.Name = "PlaneFire";
		eLevelEmitter2.Direction = Vector3.UnitY;
		eLevelEmitter2.eType = EmitterType.FireLooping;
		eLevelEmitter2.Scale = 256f;
		LevelOutside.Emitters[1] = eLevelEmitter2;
		identity = Matrix.Identity;
		position.X = -14400f;
		position.Z = -43860f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 1000f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls9 = new WorldAreaCls();
		worldAreaCls9.NumZombieSpawns = 30;
		worldAreaCls9.Load("MilitaryFacility00", identity, 128f, 1024f);
		MobileHomeParks.Add(worldAreaCls9);
		identity = Matrix.Identity;
		position.X = 33600f;
		position.Z = 40700f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) + 1000f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls10 = new WorldAreaCls();
		worldAreaCls10.NumZombieSpawns = 100;
		worldAreaCls10.Load("ZadniLhota00", identity, 128f, 1200f);
		MobileHomeParks.Add(worldAreaCls10);
		identity = Matrix.Identity;
		position.X = 34200f;
		position.Z = -28850f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) - 280f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls11 = new WorldAreaCls();
		worldAreaCls11.NumZombieSpawns = 30;
		worldAreaCls11.Load("Houses00", identity, 512f, 512f);
		MobileHomeParks.Add(worldAreaCls11);
		identity = Matrix.Identity;
		position.X = 32102f;
		position.Z = -51275f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) - 280f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls12 = new WorldAreaCls();
		worldAreaCls12.NumZombieSpawns = 30;
		worldAreaCls12.Load("Warehouse00", identity, 512f, 800f);
		MobileHomeParks.Add(worldAreaCls12);
		identity = Matrix.Identity;
		position.X = 34200f;
		position.Z = -28850f;
		position.Y = HeightMapPhysics.GetHeight(ref position, out normal) - 280f;
		identity.Translation = position;
		WorldAreaCls worldAreaCls13 = new WorldAreaCls();
		worldAreaCls13.NumZombieSpawns = 600;
		worldAreaCls13.Load("CityBlock00", identity, 512f, 1000f);
		MobileHomeParks.Add(worldAreaCls13);
		DragunovScope = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\DragunovScope");
		Home2DIcon = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\Home2DIcon");
		GasStation2DIcon = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\GeneralStore2DIcon");
		Gas2DIcon = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\Gas2DIcon");
		CompassDial = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\Compass");
		ref Vector3 reference = ref FootPrintsHit[0];
		reference = new Vector3(60691f, 10119f, 27541f);
		ref Vector3 reference2 = ref FootPrintsHit[1];
		reference2 = new Vector3(60910f, 10061f, 27450f);
		ref Vector3 reference3 = ref FootPrintsHit[2];
		reference3 = new Vector3(61146f, 9975f, 27388f);
		ref Vector3 reference4 = ref FootPrintsHit[3];
		reference4 = new Vector3(61388f, 9875f, 27305f);
		int num = 0;
		foreach (WorldAreaCls mobileHomePark in MobileHomeParks)
		{
			num += mobileHomePark.ZombieSpawnPos.Count;
		}
		ZombiePositionGrid.Create();
		ZombiePositionGrid.SetMaxBots(num);
		BotsLOSCls.Initialize();
		SwishHitSound = EndGameEngine.SoundBnk.GetCue("AxeHit00");
	}

	~SimpleZombieAI()
	{
		_ = Vector3.Zero;
		_ = Vector3.Zero;
		base.Finalize();
		foreach (WorldAreaCls mobileHomePark in MobileHomeParks)
		{
			AIBase.AllWorldItems.Initialize(mobileHomePark);
		}
		foreach (WorldAreaCls mobileHomePark2 in MobileHomeParks)
		{
			mobileHomePark2.Finalize();
		}
	}

	public override void ResetWave()
	{
		AIBase.TimeOfDayMultiplyer = 0.6f;
		AIBase.WaveSpawned = false;
		MaxBotThisWave = 100;
		NumBotKilledThisWave = 0;
		CurrentWave = 0;
		TotalMonies = 2;
		AIStateMachine.SetAttackPlayerEnable(e: false);
		GenericMessages.Clear();
		LevelObjectives.Clear();
		AIBase.PlayerInventory.Reset();
		TotalMonies = 20000;
		LevelOutside.SetSkyDome(new Vector3(21725f, 11044f, -7782f), new Color(25, 25, 38), new Color(1, 1, 3), 1);
		foreach (WorldAreaCls mobileHomePark in MobileHomeParks)
		{
			mobileHomePark.Reset();
		}
	}

	public override void SpawnBot(int nBotsToSpawn)
	{
		Vector3 zero = Vector3.Zero;
		Vector3 zero2 = Vector3.Zero;
		SpawnPoints.GetNumberOfSpawnPoints(SpawnPointType.Box);
		for (int i = 0; i < AIBase.AllZombies.Count; i++)
		{
			if (nBotsToSpawn <= 0)
			{
				break;
			}
			ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[i];
			if (zombieBot.Health <= 0 && !zombieBot.mRagdoll.IsValid && !zombieBot.mRagdoll.SetRagdoll)
			{
				Vector3 normal = Vector3.UnitY;
				zero = zombieBot.Position;
				zombieBot.Position.Y = HeightMapPhysics.GetHeight(ref zero, out normal);
				ref Vector3 reference = ref zombieBot.NavMeshRoute[0];
				reference = zombieBot.Position;
				zombieBot.NavMeshStart = 0;
				zombieBot.NavMeshCount = 0;
				zombieBot.Health = 100;
				zombieBot.State = AIBotStates.ZombieWander;
				zombieBot.TransitionTime = 1f;
				zombieBot.mRagdoll.IsValid = false;
				zombieBot.Direction = zero2;
				zombieBot.MoveDirection = Vector3.UnitX;
				zombieBot.SpeedScalar = 0.7f + (float)RandGenerator.NextDouble() * 0.3f;
				zombieBot.SpeedScalar *= WaveMechs[CurrentWave].RunSpeedScalar;
				nBotsToSpawn--;
				zombieBot.Spawn();
			}
		}
	}

	public override void Update(float etime, int qIndex)
	{
		UpdateNetWorkEvents();
		ApocZSaveDataCls.Update();
		AIBase.Clans.Update(0.03334f, qIndex);
		base.Update(etime, qIndex);
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		playerBase.OverrideInput = false;
		playerBase.OverrideProjection = false;
		bool flag = false;
		for (int i = 0; i < AIBase.NumOfVehicles; i++)
		{
			if (AIBase.AllVehicles[i] != null)
			{
				AIBase.AllVehicles[i].Update(etime, qIndex, playerBase, canAttach: true, i);
				if (AIBase.AllVehicles[i].AttachedPlayer[playerBase.vehicleSeat] == playerBase)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			playerBase.OverrideCamera = true;
			playerBase.OverridePosition = true;
			playerBase.OverrideButtonTriggerRight = true;
			playerBase.OverrideProjection = true;
			playerBase.ZoomOverride = (float)Math.PI / 4f;
		}
		else
		{
			playerBase.OverrideCamera = false;
			playerBase.OverridePosition = false;
			playerBase.OverrideButtonTriggerRight = false;
			playerBase.OverrideProjection = false;
		}
		for (int j = 0; j < AIBase.NumOfVehicles; j++)
		{
			if (AIBase.AllVehicles[j] == null || AIBase.AllVehicles[j].AttachedPlayer[playerBase.vehicleSeat] != playerBase)
			{
				continue;
			}
			for (int k = 0; k < AIBase.NumOfVehicles; k++)
			{
				if (j != k)
				{
					AIBase.AllVehicles[j].CollisionWithOther(AIBase.AllVehicles[k]);
				}
			}
		}
		AIBase.AllWorldItems.Update(etime, qIndex, playerBase);
		AIBase.PlayerInventory.Update(etime, qIndex, playerBase);
		NetworkSession networkSession = ((EGENetWorkNext.networkSession != null) ? EGENetWorkNext.networkSession : null);
		if (networkSession != null)
		{
			if (networkSession.IsHost)
			{
				ZombiePositionGrid.UpdateHost(qIndex, networkSession);
			}
			else
			{
				ZombiePositionGrid.UpdateClient(qIndex, networkSession);
			}
		}
		else
		{
			ZombiePositionGrid.UpdateHost(qIndex, null);
		}
		int num = 0;
		NumberBotsCurrentAlive = 0;
		ZombieWander.ClosestDisSqr = float.MaxValue;
		AIStateMachine.NumberBotsHuntingPlayer = 0;
		dtStatNavMesh.MaxRoutesThisUpdate = 1;
		for (int l = 0; l < AIBase.AllZombies.Count; l++)
		{
			int num2 = 0;
			ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[l];
			if (zombieBot.Health > 0)
			{
				zombieBot.NeedHeightMapUpdate = true;
				NumberBotsCurrentAlive++;
				if (zombieBot.BotHordeRef != null)
				{
					tmpDir = Vector3.Zero;
					tmpBSphere.Center = zombieBot.BotHordeRef.pos;
					tmpBSphere.Radius = 48f;
					zombieBot.Position = zombieBot.BotHordeRef.pos;
					for (int m = 0; m < AIBase.NumOfVehicles; m++)
					{
						if (AIBase.AllVehicles[m] == null || !AIBase.AllVehicles[m].SphereColision(ref tmpBSphere, ref tmpDir, qIndex))
						{
							continue;
						}
						AIBase.AllVehicles[m].Speed *= 0.995f;
						if (AIBase.AllVehicles[m].Speed > 20f)
						{
							AIBase.VehicleKillZombie(ref zombieBot.Position, ref tmpDir);
							if (EGENetWorkNext.networkSession != null)
							{
								ZombiePositionGrid.Kill(zombieBot.BotHordeRef, ref tmpDir, ZombieLODEntry.NetSession.LocalGamers[0].Id, DamegePacketType.Vehicle, broadcast: true);
							}
							else
							{
								ZombiePositionGrid.Kill(zombieBot.BotHordeRef, ref tmpDir, 0, DamegePacketType.Vehicle, broadcast: true);
							}
							tmpDir *= AIBase.AllVehicles[m].Speed * 0.75f;
							zombieBot.KillZombie(ref tmpDir);
						}
						else
						{
							float num3 = ((AIBase.AllVehicles[m].Speed < 20f) ? 20f : AIBase.AllVehicles[m].Speed);
							zombieBot.BotHordeRef.pos += tmpDir * num3 * 1.5f;
							zombieBot.Position = zombieBot.BotHordeRef.pos;
						}
					}
				}
				if (zombieBot.CurrentAnimation != (WeaponAnim)zombieBot.BotHordeRef.bAnimation)
				{
					zombieBot.PlayAnimation((WeaponAnim)zombieBot.BotHordeRef.bAnimation, randStart: true);
					zombieBot.BotHordeRef.SetAnimation(zombieBot.CurrentAnimationState.AnimationTexture.Height, randStart: true);
				}
				vecSteerAccum = Vector3.Zero;
				zombieBot.NeighborSpeedAdjust = 1f;
				for (int n = 0; n < AIBase.MaxZombies; n++)
				{
					if (num2 >= 12)
					{
						break;
					}
					if (l == n || (AIBase.AllZombies[n].Health <= 0 && (!AIBase.AllZombies[n].mRagdoll.IsValid || !(AIBase.AllZombies[n].mRagdoll.SleepTimer < 1f))))
					{
						continue;
					}
					if (AIBase.AllZombies[n].mRagdoll.IsValid)
					{
						vecToOther = AIBase.AllZombies[n].mRagdoll.RagdollSkinPose[0].Translation - zombieBot.Position;
					}
					else
					{
						vecToOther = AIBase.AllZombies[n].Position - zombieBot.Position;
					}
					if (!(Math.Abs(vecToOther.Y) < 80f))
					{
						continue;
					}
					vecToOther.Y = 0f;
					float num4 = vecToOther.LengthSquared();
					if (num4 < 16384f)
					{
						float result = 0f;
						Vector3.Dot(ref vecToOther, ref zombieBot.Direction, out result);
						if (result > 0f)
						{
							zombieBot.NeighborDisSqrList[num2] = num4;
							ref Vector3 reference = ref zombieBot.NeighborVectorList[num2];
							reference = vecToOther;
							num2++;
						}
					}
				}
				int value = (int)EndGameEngine.controllingPlayer.Value;
				AIBase.AllZombies[l].PlayerDistanceSqr[qIndex] = float.MaxValue;
				if (LevelBaseMenu.Players[value].IsValid && LevelBaseMenu.Players[value].Spawned)
				{
					tmpContactVec.X = zombieBot.Position.X - LevelBaseMenu.Players[value].vecPosition.X;
					tmpContactVec.Z = zombieBot.Position.Z - LevelBaseMenu.Players[value].vecPosition.Z;
					tmpContactVec.Y = zombieBot.Position.Y - (LevelBaseMenu.Players[value].vecPosition.Y - 60f);
					Math.Abs(tmpContactVec.Y);
					tmpContactVec.Y = 0f;
					float num5 = tmpContactVec.LengthSquared();
					zombieBot.PlayerDistanceSqr[qIndex] = num5;
					if (num5 < 4000000f && zombieBot.State == AIBotStates.HuntPlayer)
					{
						num++;
					}
				}
			}
			zombieBot.nNeighbors = num2;
			zombieBot.Update(etime, qIndex, zombieBot.CollisionIndex == 0 || zombieBot.State == AIBotStates.HuntPlayer);
			if (zombieBot.Health > 0)
			{
				float num6 = 4000000f;
				tmpVec = zombieBot.Position - playerBase.vecPosition;
				if (Vector3.Dot(tmpVec, playerBase.CameraDirection) < 0f)
				{
					num6 *= 0.5f;
				}
				if (zombieBot.PlayerDistanceSqr[qIndex] >= num6)
				{
					NumberBotsCurrentAlive--;
					uint zFlags = zombieBot.BotHordeRef.zFlags;
					zFlags &= 0xDF;
					zombieBot.BotHordeRef.zFlags = (byte)zFlags;
					zombieBot.Health = 0;
				}
			}
		}
		ZombieHuntPlayer.NumInThisState = 0;
		for (int num7 = 0; num7 < AIBase.AllZombies.Count; num7++)
		{
			ZombieBot zombieBot2 = (ZombieBot)AIBase.AllZombies[num7];
			if (zombieBot2.Health > 0 && zombieBot2.State == AIBotStates.ZombieHuntPlayer)
			{
				ZombieHuntPlayer.NumInThisState++;
			}
		}
		botGetsCollisionIndex = ((botGetsCollisionIndex != 1) ? 1 : 0);
		bool flag2 = false;
		for (int num8 = 0; num8 < AIBase.NumOfVehicles; num8++)
		{
			if (AIBase.AllVehicles[num8] != null)
			{
				flag2 = flag2 || AIBase.AllVehicles[num8].AttachedPlayer[playerBase.vehicleSeat] == playerBase || AIBase.AllVehicles[num8].DisplayCanAttachText;
			}
		}
		GameMenuScreenCls.MenusActive = false;
		BotsLOSCls.Update(etime, qIndex);
		int num9 = 0;
		removeZombiePositions[num9] = null;
		int num10 = ZombiePositionGrid.AllZombiesInWorld.Length;
		for (int num11 = 0; num11 < num10; num11++)
		{
			ZombieLODEntry zombieLODEntry = ZombiePositionGrid.AllZombiesInWorld[num11];
			if (zombieLODEntry != null && (zombieLODEntry.zFlags & 0x10) <= 0 && (zombieLODEntry.zFlags & 0x20) <= 0)
			{
				Vector3 zero = Vector3.Zero;
				zero = zombieLODEntry.pos;
				float num12 = (zero - playerBase.vecPosition).LengthSquared();
				if (BotsLOSCls.Add(playerBase, qIndex, zombieLODEntry) && num12 < 4000000f)
				{
					SpawnApocZombie(playerBase, zombieLODEntry, qIndex);
				}
			}
		}
		int treePositions = LevelBaseMenu.tmpTerrainVegitation.GetTreePositions(ref playerBase.vecPosition, qIndex, playerBase);
		for (int num13 = 0; num13 < treePositions; num13++)
		{
			tmpDir = playerBase.vecPosition - TerrainVegetation.GetTreePosList[num13];
			tmpDir.Y = 0f;
			if (tmpDir.LengthSquared() < 6400f)
			{
				playerBase.vecPosition.X += tmpDir.X * (1f - tmpDir.LengthSquared() / 6400f);
				playerBase.vecPosition.Z += tmpDir.Z * (1f - tmpDir.LengthSquared() / 6400f);
			}
		}
		InstancePropsManager.ResetModelsInstances(qIndex);
		bool flag3 = false;
		Vector3 zero2 = Vector3.Zero;
		Vector3 zero3 = Vector3.Zero;
		_ = Vector3.UnitY;
		_ = Vector3.Zero;
		BotCollisionTest = !BotCollisionTest;
		BoundingSphere sphere = default(BoundingSphere);
		for (int num14 = 0; num14 < MobileHomeParks.Count; num14++)
		{
			MobileHomeParks[num14].Update(etime, qIndex, playerBase);
			zero2 = MobileHomeParks[num14].Min;
			zero3 = MobileHomeParks[num14].Max;
			if (playerBase.vecPosition.X >= zero2.X && playerBase.vecPosition.X <= zero3.X && playerBase.vecPosition.Z >= zero2.Z && playerBase.vecPosition.Z <= zero3.Z)
			{
				flag3 = true;
				playerBase.CurrentPathingData = MobileHomeParks[num14].PathingData;
				playerBase.CurrentCollisionArea = MobileHomeParks[num14];
				bool onWalkable = false;
				bool inCollision = false;
				playerBase.OverrideLevelOutsideCollision = true;
				sphere.Center = playerBase.vecPosition;
				sphere.Center.Y -= 18f;
				sphere.Radius = 28f + Math.Abs(playerBase.Speed);
				PropModelBase.RayCastDist = 42f;
				PropModelBase.TestRayCast = true;
				MobileHomeParks[num14].SphereCollision(qIndex, ref sphere, ref onWalkable, ref inCollision, isPlayer: true, testWalkable: true);
				PropModelBase.TestRayCast = false;
				sphere.Center.Y += 16f;
				playerBase.vecPosition = Vector3.Lerp(playerBase.vecPosition, sphere.Center, 0.75f);
				if (onWalkable)
				{
					if (!playerBase.onWalkable && playerBase.GravityAccel > 8f)
					{
						playerBase.fpsWeapon.vecHeadSwayTarget.X = playerBase.GravityAccel / 75f;
					}
					playerBase.GravityAccel = 0f;
				}
				playerBase.onWalkable = onWalkable;
			}
			PropModelBase.TestRayCast = true;
			PlayerBase playerBase2 = null;
			int index = 0;
			while ((playerBase2 = EGENetWorkNext.NextNetPlayerReference(ref index)) != null)
			{
				sphere.Center = playerBase2.vecPosition;
				sphere.Center.Y -= 18f;
				sphere.Radius = 36f;
				if (sphere.Center.X >= zero2.X && sphere.Center.X <= zero3.X && sphere.Center.Z >= zero2.Z && sphere.Center.Z <= zero3.Z)
				{
					bool onWalkable2 = false;
					bool inCollision2 = false;
					playerBase2.CurrentPathingData = MobileHomeParks[num14].PathingData;
					PropModelBase.RayCastDist = 38f;
					MobileHomeParks[num14].SphereCollision(qIndex, ref sphere, ref onWalkable2, ref inCollision2, isPlayer: true, testWalkable: true);
					sphere.Center.Y += 18f;
					playerBase2.vecPosition = Vector3.Lerp(playerBase2.vecPosition, sphere.Center, 0.8f);
					if (onWalkable2)
					{
						playerBase2.GravityAccel = 0f;
					}
				}
				index++;
			}
			PropModelBase.TestRayCast = false;
			PropModelBase.RayCastDist = 33f;
			PropModelBase.TestRayCast = true;
			for (int num15 = 0; num15 < AIBase.AllZombies.Count; num15++)
			{
				ZombieBot zombieBot3 = (ZombieBot)AIBase.AllZombies[num15];
				if (zombieBot3.Health <= 0 || !(zombieBot3.Position.X >= zero2.X) || !(zombieBot3.Position.X <= zero3.X) || !(zombieBot3.Position.Z >= zero2.Z) || !(zombieBot3.Position.Z <= zero3.Z))
				{
					continue;
				}
				bool onWalkable3 = false;
				bool inCollision3 = false;
				sphere.Center = zombieBot3.Position;
				sphere.Center.Y += 38f;
				sphere.Radius = 28f;
				zombieBot3.NeedHeightMapUpdate = false;
				if (EGENetWorkNext.networkSession != null)
				{
					if (playerBase.NetGamerRef.IsHost && zombieBot3.CurrentPathingDataSourceIndex != num14)
					{
						zombieBot3.CurrentPathingDataSourceIndex = num14;
						PacketWriter packetWriter = EGENetWorkNext.packetWriter;
						packetWriter.Write((byte)126);
						packetWriter.Write(zombieBot3._uid);
						packetWriter.Write((ushort)num14);
					}
				}
				else if (zombieBot3.CurrentPathingDataSourceIndex != num14)
				{
					zombieBot3.CurrentPathingDataSourceIndex = num14;
				}
				PropModelBase.RayCastCollision = false;
				MobileHomeParks[num14].SphereCollision(qIndex, ref sphere, ref onWalkable3, ref inCollision3, isPlayer: false, testWalkable: true);
				sphere.Center.Y -= 38f;
				if (PropModelBase.RayCastCollision)
				{
					zombieBot3.GravityAccumulator = 0f;
				}
				else
				{
					zombieBot3.GravityAccumulator += etime * 8f;
					sphere.Center.Y -= zombieBot3.GravityAccumulator;
					float num16 = HeightMapPhysics.GetHeight(ref zombieBot3.Position) + 4f;
					if (sphere.Center.Y < num16)
					{
						sphere.Center.Y = num16;
					}
				}
				zombieBot3.Position = Vector3.Lerp(zombieBot3.Position, sphere.Center, 0.75f);
				zombieBot3.BotHordeRef.pos = zombieBot3.Position;
			}
			PropModelBase.TestRayCast = false;
		}
		for (int num17 = 0; num17 < AIBase.AllZombies.Count; num17++)
		{
			ZombieBot zombieBot4 = (ZombieBot)AIBase.AllZombies[num17];
			if (zombieBot4.BotHordeRef != null && zombieBot4.Health > 0 && zombieBot4.NeedHeightMapUpdate)
			{
				zombieBot4.BotHordeRef.pos.Y = HeightMapPhysics.GetHeight(ref zombieBot4.BotHordeRef.pos) + 4f;
				zombieBot4.Position = zombieBot4.BotHordeRef.pos;
			}
		}
		InstancePropsManager.UpdateModelsInstances(qIndex);
	}

	private void UpdateBotPlayersContact()
	{
		int count = AIBase.AllZombies.Count;
		for (int i = 0; i < 4; i++)
		{
			if (!LevelBaseMenu.Players[i].IsValid || !LevelBaseMenu.Players[i].Spawned)
			{
				continue;
			}
			for (int j = 0; j < count; j++)
			{
				if (AIBase.AllZombies[j].Health <= 0)
				{
					continue;
				}
				tmpContactVec = AIBase.AllZombies[j].Position - LevelBaseMenu.Players[i].vecPosition;
				if (Math.Abs(tmpContactVec.Y) < 80f)
				{
					tmpContactVec.Y = 0f;
					float num = tmpContactVec.LengthSquared();
					if (num < 6400f)
					{
						num /= 6400f;
						tmpContactVec *= 1f - num;
						LevelBaseMenu.Players[i].vecPosition += tmpContactVec * -0.75f;
						AIBase.AllZombies[j].Position += tmpContactVec * 0.25f;
					}
				}
			}
		}
	}

	public override bool OwnerSphereCollision(ref BoundingSphere sphere, int qIndex, bool testWalkable)
	{
		bool inCollision = false;
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			bool onWalkable = false;
			PropModelBase.TestRayCast = false;
			MobileHomeParks[i].SphereCollision(qIndex, ref sphere, ref onWalkable, ref inCollision, isPlayer: false, testWalkable);
			PropModelBase.TestRayCast = false;
		}
		return inCollision;
	}

	public override float? OwnerWalkableHeight(ref Vector3 pos, int qIndex)
	{
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			float? result = MobileHomeParks[i].WalkableHeight(ref pos, qIndex);
			if (result.HasValue)
			{
				return result;
			}
		}
		return null;
	}

	public override void Draw(PlayerBase playerRef, int qIndex)
	{
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			if (MobileHomeParks[i].InFrustum[qIndex])
			{
				MobileHomeParks[i].DrawCameraSpace(playerRef, qIndex, 1f);
			}
		}
		InstancePropsManager.DrawModelsInstances(playerRef, qIndex);
		for (int j = 0; j < AIBase.NumOfVehicles; j++)
		{
			if (AIBase.AllVehicles[j] != null)
			{
				AIBase.AllVehicles[j].Draw(playerRef, qIndex);
			}
		}
		AIBase.AllWorldItems.Draw(playerRef, qIndex);
		BotsLOSCls.Draw(playerRef, qIndex);
		for (WeaponAnim weaponAnim = WeaponAnim.ZombieIdle; weaponAnim < WeaponAnim.ZombieKeepWalk; weaponAnim++)
		{
			for (int k = 0; k < 5; k++)
			{
				bool flag = false;
				for (int l = 0; l < AIBase.AllZombies.Count; l++)
				{
					ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[l];
					if (zombieBot.ModelIndex == k && zombieBot.CurrentAnimation == weaponAnim)
					{
						if (!flag)
						{
							flag = true;
							zombieBot.PreDraw(playerRef, qIndex, k);
						}
						zombieBot.Draw(playerRef, qIndex, k);
					}
				}
			}
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;
		graphicsDevice.VertexSamplerStates[1] = SamplerState.PointClamp;
		graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
		for (int m = 0; m < AIBase.AllZombies.Count; m++)
		{
			ZombieBot zombieBot2 = (ZombieBot)AIBase.AllZombies[m];
			if (zombieBot2.mRagdoll.IsValid)
			{
				zombieBot2.DrawRagdoll(qIndex, playerRef);
			}
		}
		AIBase.PlayerInventory.Draw(playerRef, qIndex);
	}

	public override void DrawShadowMap(PlayerBase playerRef, ref Matrix LightViewProj, ref Vector3 lightPos, int qIndex)
	{
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			if (MobileHomeParks[i].InFrustum[qIndex])
			{
				MobileHomeParks[i].DrawShadowMap(playerRef, ref LightViewProj, ref lightPos, qIndex, lod: false);
			}
		}
		InstancePropsManager.DrawModelShadowInstances(playerRef, ref LightViewProj, ref lightPos, qIndex);
		AIBase.AllWorldItems.DrawShadowMap(playerRef, ref LightViewProj, ref lightPos, qIndex);
		for (int j = 0; j < AIBase.NumOfVehicles; j++)
		{
			if (AIBase.AllVehicles[j] != null)
			{
				AIBase.AllVehicles[j].DrawShadowMap(playerRef, ref LightViewProj, ref lightPos, qIndex, lod: false);
			}
		}
		for (WeaponAnim weaponAnim = WeaponAnim.ZombieIdle; weaponAnim < WeaponAnim.ZombieKeepWalk; weaponAnim++)
		{
			bool flag = false;
			for (int k = 0; k < AIBase.AllZombies.Count; k++)
			{
				ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[k];
				if (zombieBot.CurrentAnimation == weaponAnim)
				{
					if (!flag)
					{
						flag = true;
						zombieBot.PreDrawShadow(playerRef, ref LightViewProj, ref lightPos, qIndex);
					}
					zombieBot.DrawShadow(playerRef, qIndex);
				}
			}
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.VertexSamplerStates[0] = SamplerState.PointWrap;
		graphicsDevice.VertexSamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[4] = SamplerState.PointWrap;
		graphicsDevice.SamplerStates[5] = SamplerState.PointWrap;
	}

	public new static void DrawAlpha(PlayerBase playerRef, int qIndex)
	{
		for (int i = 0; i < AIBase.NumOfVehicles; i++)
		{
			if (AIBase.AllVehicles[i] != null)
			{
				AIBase.AllVehicles[i].DrawAlpha(playerRef, qIndex);
			}
		}
	}

	public override void DrawPost(int qIndex, PlayerBase playerRef)
	{
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		AIBase.SpectatorDispTimer -= 0.03334f;
		if (PlayerBase.runInSpectator)
		{
			if (AIBase.SpectatorDispTimer > 0f)
			{
				Vector2 zero = Vector2.Zero;
				Vector2 vector = new Vector2(2f, 2f);
				zero.X = viewport.TitleSafeArea.Left;
				Menu.spriteBatch.Begin();
				zero.Y = viewport.TitleSafeArea.Bottom - 96;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Left Bumper: Move Up", zero + vector, Color.Black);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Left Bumper: Move Up", zero, Color.LightGray);
				zero.Y += 32f;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Right Bumper: Move Down", zero + vector, Color.Black);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Right Bumper: Move Down", zero, Color.LightGray);
				zero.Y += 32f;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Press A For FPS Camera", zero + vector, Color.Black);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Press A For FPS Camera", zero, Color.LightGray);
				Menu.spriteBatch.End();
			}
			return;
		}
		if (PlayerBase.DispRunInSpectator && AIBase.SpectatorDispTimer > 0f)
		{
			Vector2 zero2 = Vector2.Zero;
			Vector2 vector2 = new Vector2(2f, 2f);
			zero2.X = viewport.TitleSafeArea.Left;
			zero2.Y = viewport.TitleSafeArea.Bottom;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Press A For Spectator Camera", zero2 + vector2, Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Press A For Spectator Camera", zero2, Color.LightGray);
			Menu.spriteBatch.End();
		}
		blinktimer -= 0.03334f;
		if (playerRef.DeathTimer > 0f)
		{
			uiPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString("You Are Died").X * 0.5f * 2f;
			uiPos.Y = viewport.TitleSafeArea.Center.Y;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, viewport.Bounds, Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "You Are Dead", uiPos, Color.LightGray, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
			return;
		}
		GenericMessages.DrawPost(qIndex, playerRef);
		BotsLOSCls.DrawPost(qIndex, playerRef);
		AIBase.Clans.DrawPost(qIndex);
		bool flag = false;
		for (int i = 0; i < AIBase.NumOfVehicles; i++)
		{
			if (AIBase.AllVehicles[i] != null)
			{
				flag = flag || AIBase.AllVehicles[i].AttachedPlayer[playerRef.vehicleSeat] == playerRef || AIBase.AllVehicles[i].DisplayCanAttachText;
			}
		}
		if (!flag)
		{
			if (playerRef.isSighted[qIndex] && (playerRef.currentWeaponType == WeaponType.HDCamera || playerRef.currentWeaponType == WeaponType.CellPhonCamera))
			{
				Menu.spriteBatch.Begin();
				Menu.spriteBatch.Draw(CameraLensScope, viewport.Bounds, Color.White);
				if (playerRef.currentWeaponType == WeaponType.HDCamera)
				{
					recTimer -= 0.03f;
					if (recTimer < 0f)
					{
						recTimer = 1f;
					}
					if (recTimer > 0.5f && playerRef.currentGamePadState.IsButtonDown(Buttons.RightTrigger))
					{
						Menu.spriteBatch.Draw(CameraRecMsg, recRecMsg, Color.Red);
					}
				}
				Menu.spriteBatch.End();
				return;
			}
			y -= 8;
			if (y < 0)
			{
				y = 0;
			}
			if (playerRef.isSighted[qIndex] && playerRef.currentWeaponType == WeaponType.Sniper)
			{
				if (playerRef.ShotFired)
				{
					float num = ((float)EndGameEngine.randGenerator.NextDouble() - 0.5f) * 2f;
					y = 40;
					playerRef.Angles.Y -= 0.25f;
					playerRef.Angles.X += num;
				}
				int num2 = y * 2;
				Menu.spriteBatch.Begin();
				Menu.spriteBatch.Draw(DragunovScope, new Rectangle(-(num2 / 2), -(y / 2), 1280 + num2, 720 + y), Color.White);
				Menu.spriteBatch.End();
				playerRef.DrawMuzzleFlashAlpha = 0f;
			}
			AIBase.DispHelpInfo -= 0.0334f;
			if (AIBase.DispHelpInfo > 0f && !InventoryCls.InventoryOpen)
			{
				float num3 = ((AIBase.DispHelpInfo > 1f) ? 1f : AIBase.DispHelpInfo);
				uiPos.X = viewport.TitleSafeArea.Right - 220;
				uiPos.Y = viewport.TitleSafeArea.Bottom - 144;
				recRecMsg.X = (int)(uiPos.X - 42f);
				recRecMsg.Y = (int)(uiPos.Y + 2f);
				recRecMsg.Width = 34;
				recRecMsg.Height = 34;
				Menu.spriteBatch.Begin();
				Color white = Color.White;
				Color black = Color.Black;
				black.A = (byte)(255f * num3);
				white.A = (byte)(211f * num3);
				white.R = white.A;
				white.G = white.A;
				white.B = white.A;
				Menu.spriteBatch.Draw(Menu.startButton, recRecMsg, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Menu", uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Menu", uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				uiPos.Y += 36f;
				recRecMsg.Y = (int)(uiPos.Y + 2f);
				Menu.spriteBatch.Draw(Menu.backButton, recRecMsg, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Inventory", uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Inventory", uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				uiPos.Y += 36f;
				recRecMsg.Y = (int)(uiPos.Y + 2f);
				Menu.spriteBatch.Draw(Menu.yButton, recRecMsg, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Next Weapon", uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Next Weapon", uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				uiPos.Y += 36f;
				recRecMsg.Y = (int)(uiPos.Y + 2f);
				Menu.spriteBatch.Draw(Menu.dpRight, recRecMsg, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Flash Light", uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Flash Light", uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.End();
			}
		}
		if (!InventoryCls.InventoryOpen)
		{
			for (int j = 0; j < AIBase.NumOfVehicles; j++)
			{
				if (AIBase.AllVehicles[j] != null)
				{
					AIBase.AllVehicles[j].DrawPost(playerRef, qIndex);
				}
			}
		}
		AIBase.AllWorldItems.DrawPost(playerRef, qIndex);
		AIBase.PlayerInventory.DrawPost(playerRef, qIndex);
		ApocZSaveDataCls.DrawPost(qIndex);
		Menu.spriteBatch.Begin();
		if (!InventoryCls.InventoryOpen)
		{
			if (EGENetWorkNext.networkSession != null || Guide.IsTrialMode || LevelBaseMenu.isTrialMode)
			{
				hourcounter += 0.00015f * AIBase.TimeOfDayMultiplyer;
				if (hourcounter > 227f / (276f * (float)Math.PI))
				{
					hourcounter -= 227f / (276f * (float)Math.PI);
					playerRef.CurrentDay++;
				}
				uiPos.X = viewport.TitleSafeArea.Right - 140;
				uiPos.Y = viewport.TitleSafeArea.Top;
				string b = "Day " + (playerRef.CurrentDay / 24 + 1);
				Menu.spriteBatch.DrawString(Menu.defaultFont, b, uiPos + shadowPos, Color.Black, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, b, uiPos, Color.LightGray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
				if (!Guide.IsTrialMode && LevelBaseMenu.isTrialMode)
				{
					uiPos.Y += 40f;
					string b2 = "Offline";
					Menu.spriteBatch.DrawString(Menu.defaultFont, b2, uiPos + shadowPos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, b2, uiPos, Color.LightGray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				}
			}
			else
			{
				uiPos.X = viewport.TitleSafeArea.Right - 140;
				uiPos.Y = viewport.TitleSafeArea.Top;
				string b3 = "Offline";
				Menu.spriteBatch.DrawString(Menu.defaultFont, b3, uiPos + shadowPos, Color.Black, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, b3, uiPos, Color.LightGray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
			}
			if (AIBase.PlayerInventory.HaveCompass())
			{
				headingVec = playerRef.vecDirection;
				headingVec.Y = 0f;
				headingVec.Normalize();
				float z = headingVec.Z;
				float x = headingVec.X;
				float num4 = MyMath.AngleBetweenVectors(headingVec, -Vector3.UnitZ);
				compassRec.X = 256;
				if (z >= 0f)
				{
					num4 /= (float)Math.PI;
					if (x >= 0f)
					{
						compassRec.X += (int)(num4 * 256f);
					}
					else
					{
						compassRec.X -= (int)(num4 * 256f);
					}
				}
				else
				{
					num4 /= (float)Math.PI;
					if (x >= 0f)
					{
						compassRec.X += (int)(num4 * 256f);
					}
					else
					{
						compassRec.X -= (int)(num4 * 256f);
					}
				}
				compassRec.Y = 0;
				compassRec.X += 108;
				compassRec.Width = 296;
				compassRec.Height = 64;
				compassRecDst = compassRec;
				compassRecDst.X = viewport.TitleSafeArea.Center.X - 148;
				compassRecDst.Y = viewport.TitleSafeArea.Top;
				compassRecDst.Width = 296;
				compassRecDst.Height = 64;
				Menu.spriteBatch.Draw(CompassDial, compassRecDst, compassRec, Color.White);
			}
		}
		string text = ((playerRef.BloodLoss > 0f) ? "Bleeding " : "Blood  ");
		text += (int)playerRef.BloodLevel;
		string s = "Water  " + (int)playerRef.WaterLevel;
		string s2 = "Food  " + (int)playerRef.FoodLevel;
		uiPos.X = viewport.TitleSafeArea.Left;
		uiPos.Y = viewport.TitleSafeArea.Bottom - 100;
		if (playerRef.BloodLoss > 0f)
		{
			if (blinktimer > 0.1f)
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, text, uiPos + shadowPos, Color.Black);
				Menu.spriteBatch.DrawString(Menu.defaultFont, text, uiPos, Color.DarkRed);
			}
		}
		else
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, uiPos + shadowPos, Color.Black);
			Menu.spriteBatch.DrawString(Menu.defaultFont, text, uiPos, Color.LightGray);
		}
		uiPos.Y += 32f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, s, uiPos + shadowPos, Color.Black);
		if (playerRef.WaterLevel < 10f)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, s, uiPos, Color.DarkRed);
		}
		else if (playerRef.WaterLevel < 30f)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, s, uiPos, Color.Yellow);
		}
		else
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, s, uiPos, Color.LightGray);
		}
		uiPos.Y += 32f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, s2, uiPos + shadowPos, Color.Black);
		if (playerRef.FoodLevel < 10f)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, s2, uiPos, Color.DarkRed);
		}
		else if (playerRef.FoodLevel < 30f)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, s2, uiPos, Color.Yellow);
		}
		else
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, s2, uiPos, Color.LightGray);
		}
		weaponMagDispTimer -= 0.0334f;
		if (playerRef.ShotFired || weaponMagDispTimer > 0f)
		{
			if (playerRef.ShotFired)
			{
				weaponMagDispTimer = 8f;
			}
			if (playerRef.currentWeaponType != WeaponType.EmptyHands && playerRef.currentWeaponType != WeaponType.Hatchet)
			{
				uiPos.X = viewport.TitleSafeArea.Center.X - 40;
				uiPos.Y = viewport.TitleSafeArea.Bottom - 32;
				string s3 = playerRef.fpsWeapon.CurrentWeapon.BulletsInMag + "/" + playerRef.fpsWeapon.CurrentWeapon.BulletsMagMax;
				float num5 = ((weaponMagDispTimer > 1f) ? 1f : ((weaponMagDispTimer < 0f) ? 0f : weaponMagDispTimer));
				wepStatShade = Color.Black;
				wepStatShade.A = (byte)(num5 * 255f);
				wepStatDiff.A = (byte)(num5 * 255f);
				wepStatDiff.R = (byte)(num5 * 211f);
				wepStatDiff.G = (byte)(num5 * 211f);
				wepStatDiff.B = (byte)(num5 * 211f);
				Menu.spriteBatch.DrawString(Menu.defaultFont, s3, uiPos + shadowPos, wepStatShade);
				Menu.spriteBatch.DrawString(Menu.defaultFont, s3, uiPos, wepStatDiff);
			}
		}
		Menu.spriteBatch.End();
		base.DrawPost(qIndex, playerRef);
		bool flag2 = false;
		for (int k = 0; k < AIBase.NumOfVehicles; k++)
		{
			if (AIBase.AllVehicles[k] != null)
			{
				flag2 = flag2 || AIBase.AllVehicles[k].AttachedPlayer[playerRef.vehicleSeat] == playerRef || AIBase.AllVehicles[k].DisplayCanAttachText;
			}
		}
		Vector2 e = new Vector2(256f, 256f);
		Rectangle a = new Rectangle(640, 360, 512, 512);
		Rectangle value = new Rectangle(0, 0, 512, 512);
		Menu.spriteBatch.Begin();
		for (int l = 0; l < 16; l++)
		{
			if (AIStateMachine.HitIndicatorArray[l].AlphaTimer > 0f)
			{
				float alphaTimer = AIStateMachine.HitIndicatorArray[l].AlphaTimer;
				AIStateMachine.HitIndicatorArray[l].AlphaTimer -= 0.05f;
				Color lightGray = Color.LightGray;
				lightGray.A = (byte)((float)(int)lightGray.A * alphaTimer);
				lightGray.R = (byte)((float)(int)lightGray.R * alphaTimer);
				lightGray.G = (byte)((float)(int)lightGray.G * alphaTimer);
				lightGray.B = (byte)((float)(int)lightGray.B * alphaTimer);
				Menu.spriteBatch.Draw(HitIndicator, a, value, lightGray, AIStateMachine.HitIndicatorArray[l].DirectionAngle, e, SpriteEffects.None, 0);
			}
		}
		Menu.spriteBatch.End();
		if (!playerRef.IsAttached0 && !InventoryCls.InventoryOpen)
		{
			if (playerRef.ThirdPersonCamera)
			{
				if (playerRef.Sighted && playerRef.currentWeaponType != WeaponType.Sniper)
				{
					playerRef.DrawReticle(playerRef.vpViewPort.AspectRatio, playerRef);
				}
			}
			else if (playerRef.currentWeaponType == WeaponType.EmptyHands || playerRef.currentWeaponType == WeaponType.Hatchet)
			{
				playerRef.DrawReticle(playerRef.vpViewPort.AspectRatio, playerRef);
			}
			else if (!playerRef.Sighted)
			{
				playerRef.DrawReticle(playerRef.vpViewPort.AspectRatio, playerRef);
			}
		}
		EGENetWorkNext.DrawPost(qIndex);
		AIBase.LocalOffLineMessage -= 0.03f;
		if (AIBase.LocalOffLineMessage > 0f)
		{
			string text2 = "";
			string text3 = "";
			string text4 = "";
			int num6 = 40;
			int height = 100;
			if (LevelBaseMenu.isTrialMode)
			{
				text2 = "Inventory And Tents Not Saved In Trial Mode";
				text3 = "Press 'Back' to Open your Inventory";
				text4 = "This Is An Online Game And Does Not Pause When A Menu Is Open";
				num6 = 0;
				height = 180;
				AIBase.LocalOffLineMessage += 0.005f;
			}
			else
			{
				text2 = "Inventory And Tents Will Only Apply To Offline Character";
			}
			uiPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text2).X * 0.5f * 1f;
			uiPos.Y = viewport.TitleSafeArea.Center.Y + num6;
			Rectangle bounds = viewport.Bounds;
			bounds.Y = (int)(uiPos.Y - 20f);
			bounds.Height = height;
			Menu.spriteBatch.Begin();
			Color black2 = Color.Black;
			black2.A = (byte)((AIBase.LocalOffLineMessage > 1f) ? 120f : (AIBase.LocalOffLineMessage * 120f));
			Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, bounds, black2);
			black2.A = (byte)((AIBase.LocalOffLineMessage > 1f) ? 255f : (AIBase.LocalOffLineMessage * 255f));
			byte b4 = (black2.R = (byte)((AIBase.LocalOffLineMessage > 1f) ? 211f : (AIBase.LocalOffLineMessage * 211f)));
			b4 = (black2.B = b4);
			black2.G = b4;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text2, uiPos, black2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			uiPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text3).X * 0.5f * 1f;
			uiPos.Y += 38f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text3, uiPos, black2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			uiPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(text4).X * 0.5f * 1f;
			uiPos.Y += 38f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, text4, uiPos, black2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
		}
		if (blinktimer < 0f)
		{
			blinktimer = 0.5f;
		}
		AIBase.BlackFadeTimer -= 0.055f;
		if (AIBase.BlackFadeTimer > 0f)
		{
			float num7 = ((AIBase.BlackFadeTimer < 1f) ? AIBase.BlackFadeTimer : 1f);
			blkFade.A = (byte)(255f * num7);
			blkFade.R = (byte)(255f * num7);
			blkFade.G = (byte)(255f * num7);
			blkFade.B = (byte)(255f * num7);
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, viewport.Bounds, blkFade);
			Menu.spriteBatch.End();
		}
	}

	private void Project2DIcon(Vector3 pos, PlayerBase playerRef, Texture2D icon, int qIndex, bool ballon)
	{
		ProjDir.X = pos.X - playerRef.vecPosition.X;
		ProjDir.Y = pos.Z - playerRef.vecPosition.Z;
		float num = ProjDir.X * playerRef.vecDirection.X + ProjDir.Y * playerRef.vecDirection.Z;
		if (!(num > 0f))
		{
			return;
		}
		float num2 = ProjDir.LengthSquared();
		float num3 = 1f - num2 / 1.6E+09f;
		num3 = ((num3 < 0f) ? 0f : num3);
		pos.X -= playerRef.vecHeadPosition[qIndex].X;
		pos.Z -= playerRef.vecHeadPosition[qIndex].Z;
		projectedPosition = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Project(pos, playerRef.mDataQueue[qIndex].projection, playerRef.mDataQueue[qIndex].view, Matrix.Identity);
		screenPosition = new Vector2(projectedPosition.X - (float)playerRef.vpViewPort.X, projectedPosition.Y - (float)playerRef.vpViewPort.Y);
		float num4 = 120f * num3 + 40f;
		if (ballon)
		{
			ballonTimer += 0.05f * ballonTimerDirection;
			if (ballonTimer > 1f)
			{
				ballonTimer = 1f;
				ballonTimerDirection = -1f;
			}
			if (ballonTimer < 0f)
			{
				ballonTimer = 0f;
				ballonTimerDirection = 1f;
			}
			num4 += ballonTimer * 40f;
		}
		recGeneric.X = (int)(screenPosition.X - num4 / 2f);
		recGeneric.Y = (int)(screenPosition.Y - num4 / 2f);
		recGeneric.Width = (int)num4;
		recGeneric.Height = (int)num4;
		ref Color reference = ref icon2DColor;
		ref Color reference2 = ref icon2DColor;
		ref Color reference3 = ref icon2DColor;
		byte b = (icon2DColor.A = (byte)(num3 * 128f + 128f));
		byte b3 = (reference3.B = b);
		byte r = (reference2.G = b3);
		reference.R = r;
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.Draw(icon, recGeneric, icon2DColor);
		Menu.spriteBatch.End();
	}

	public override bool OwnerRayCastGeometry(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPos, ref Vector3 hitNorm)
	{
		float hitDistance = float.MaxValue;
		raycastHitDistance = float.MaxValue;
		hitPos = origin + direction * 1000000f;
		raycastRay.Position = origin;
		raycastRay.Direction = direction;
		HeightMapPhysics.RayCast(ref raycastRay, ref saveHitPosition, ref saveHitNormal, ref hitDistance);
		if (hitDistance < float.MaxValue)
		{
			hitDistance *= hitDistance;
			hitPos = saveHitPosition;
			hitNorm = saveHitNormal;
		}
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			raycastRay.Position = origin;
			raycastRay.Direction = direction;
			if (MobileHomeParks[i].RayCast(qIndex, ref raycastRay, ref raycasHitPosition, ref raycasHitNormal, ref raycastHitDistance))
			{
				float num = raycastHitDistance * raycastHitDistance;
				if (num < hitDistance)
				{
					hitDistance = num;
					hitPos = raycasHitPosition;
					hitNorm = raycasHitNormal;
					return true;
				}
			}
		}
		return hitDistance < float.MaxValue;
	}

	public override bool OwnerRayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, WeaponClass weapon, ref float worldHitDisSqr)
	{
		worldHitDisSqr = float.MaxValue;
		raycastHitDistance = float.MaxValue;
		raycastRay.Position = origin;
		raycastRay.Direction = direction;
		HeightMapPhysics.RayCast(ref raycastRay, ref saveHitPosition, ref saveHitNormal, ref worldHitDisSqr);
		if (worldHitDisSqr < float.MaxValue)
		{
			worldHitDisSqr *= worldHitDisSqr;
		}
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			raycastRay.Position = origin;
			raycastRay.Direction = direction;
			if (MobileHomeParks[i].RayCast(qIndex, ref raycastRay, ref raycasHitPosition, ref raycasHitNormal, ref raycastHitDistance))
			{
				float num = raycastHitDistance * raycastHitDistance;
				if (num < worldHitDisSqr)
				{
					worldHitDisSqr = num;
					saveHitPosition = raycasHitPosition;
					saveHitNormal = raycasHitNormal;
					break;
				}
			}
		}
		int index = 0;
		int num2 = 0;
		int count = AIBase.AllZombies.Count;
		for (int j = 0; j < count; j++)
		{
			if (AIBase.AllZombies[j].Health <= 0 || !AIBase.AllZombies[j].Render[qIndex])
			{
				continue;
			}
			int num3 = AIBase.AllZombies[j].RayCast(qIndex, ref origin, ref direction, weapon);
			if (num3 > 0)
			{
				float num4 = (origin - ZombieBot.RayCastHitPosition).LengthSquared();
				if (num4 < worldHitDisSqr)
				{
					worldHitDisSqr = num4;
					index = j;
					num2 = num3;
				}
			}
		}
		if (num2 > 0)
		{
			Vector3 velocity = Vector3.UnitY;
			Vector3 spawnPos = ZombieBot.RayCastHitPosition;
			particles.SpawnBulletHitMutant(ref spawnPos, ref velocity);
			if (weapon.WepType == WeaponType.Shotgun)
			{
				float num5 = worldHitDisSqr / 1440000f;
				num5 = ((num5 > 1f) ? 1f : num5);
				num2 = (int)(200f * (1f - num5));
			}
			else if (weapon.WepType == WeaponType.Sniper)
			{
				num2 += 50;
			}
			AIBase.AllZombies[index].Health -= num2;
			if (AIBase.AllZombies[index].Health <= 0)
			{
				ZombieLODEntry botHordeRef = AIBase.AllZombies[index].BotHordeRef;
				if (botHordeRef != null)
				{
					if (EGENetWorkNext.networkSession != null)
					{
						ZombiePositionGrid.Kill(botHordeRef, ref direction, ZombieLODEntry.NetSession.LocalGamers[0].Id, DamegePacketType.Body, broadcast: true);
					}
					else
					{
						ZombiePositionGrid.Kill(botHordeRef, ref direction, 0, DamegePacketType.Body, broadcast: true);
					}
				}
				if (weapon.WepType == WeaponType.Shotgun)
				{
					direction *= 10f;
				}
				NumBotKilledThisWave++;
				AIBase.AllZombies[index].KillZombie(ref direction);
			}
		}
		else
		{
			num2 = 0;
			ZombieLODEntry zombieLODEntry = null;
			Vector3 unitY = Vector3.UnitY;
			PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
			int num6 = 0;
			removeZombiePositions[num6] = null;
			int num7 = ZombiePositionGrid.AllZombiesInWorld.Length;
			for (int k = 0; k < num7; k++)
			{
				ZombieLODEntry zombieLODEntry2 = ZombiePositionGrid.AllZombiesInWorld[k];
				if (zombieLODEntry2 == null || (zombieLODEntry2.zFlags & 0x10) > 0 || (zombieLODEntry2.zFlags & 0x20) != 0)
				{
					continue;
				}
				unitY = zombieLODEntry2.pos;
				unitY -= playerBase.vecPosition;
				float num8 = unitY.X * playerBase.CameraDirection.X + unitY.Y * playerBase.CameraDirection.Y + unitY.Z * playerBase.CameraDirection.Z;
				if (!(num8 > 0f))
				{
					continue;
				}
				raycastTmpMat = Matrix.Identity;
				unitY = zombieLODEntry2.pos;
				raycastTmpMat.Translation = unitY;
				int num9 = ((ZombieBot)AIBase.AllZombies[0]).RayCast(qIndex, ref origin, ref direction, ref raycastTmpMat, weapon);
				if (num9 > 0)
				{
					float num10 = (origin - ZombieBot.RayCastHitPosition).LengthSquared();
					if (num10 < worldHitDisSqr)
					{
						worldHitDisSqr = num10;
						num2 = num9 + 40;
						zombieLODEntry = zombieLODEntry2;
					}
				}
			}
			if (num2 > 0)
			{
				int num11 = -1;
				float num12 = float.MinValue;
				for (int l = 0; l < AIBase.MaxZombies; l++)
				{
					if (AIBase.AllZombies[l].Health > 0)
					{
						continue;
					}
					num11 = ((num11 < 0) ? l : num11);
					tmpVec = AIBase.AllZombies[l].Position - playerBase.vecPosition;
					tmpVec.Normalize();
					float num13 = tmpVec.X * playerBase.CameraDirection.X + tmpVec.Y * playerBase.CameraDirection.Y + tmpVec.Z * playerBase.CameraDirection.Z;
					if (num13 < 0.5f)
					{
						num13 = tmpVec.LengthSquared();
						if (num13 > num12)
						{
							num12 = num13;
							num11 = l;
						}
					}
				}
				if (zombieLODEntry != null)
				{
					((ZombieBot)AIBase.AllZombies[num11]).ModelIndex = zombieLODEntry.zFlags & 0xF;
					unitY = zombieLODEntry.pos;
					((ZombieBot)AIBase.AllZombies[num11]).WorldTransform[0].Translation = unitY;
					((ZombieBot)AIBase.AllZombies[num11]).WorldTransform[1].Translation = unitY;
					((ZombieBot)AIBase.AllZombies[num11]).mRagdoll.SetRagdoll = true;
					if (EGENetWorkNext.networkSession != null)
					{
						ZombiePositionGrid.Kill(zombieLODEntry, ref direction, ZombieLODEntry.NetSession.LocalGamers[0].Id, DamegePacketType.Body, broadcast: true);
					}
					else
					{
						ZombiePositionGrid.Kill(zombieLODEntry, ref direction, 0, DamegePacketType.Body, broadcast: true);
					}
				}
			}
		}
		if (num2 <= 0)
		{
			particles.SpawnBulletHitRock(ref saveHitPosition, ref saveHitNormal);
		}
		return false;
	}

	public override void OwnerGrenadeExplode(int qIndex, ref Vector3 origin)
	{
	}

	public override bool OwnerPlayerAttackKnife(int qIndex, ref Vector3 origin, ref Vector3 direction)
	{
		bool result = false;
		_ = AIBase.AllZombies.Count;
		for (int i = 0; i < AIBase.MaxZombies; i++)
		{
			if (AIBase.AllZombies[i].Health <= 0)
			{
				continue;
			}
			tmpForceDir = AIBase.AllZombies[i].Position - origin;
			float num = tmpForceDir.LengthSquared();
			if (!(num < 40000f))
			{
				continue;
			}
			tmpForceDir.Normalize();
			if (!(Vector3.Dot(tmpForceDir, direction) > 0.8f))
			{
				continue;
			}
			result = true;
			ZombieLODEntry botHordeRef = AIBase.AllZombies[i].BotHordeRef;
			if (botHordeRef != null)
			{
				if (EGENetWorkNext.networkSession != null)
				{
					ZombiePositionGrid.Kill(botHordeRef, ref tmpForceDir, ZombieLODEntry.NetSession.LocalGamers[0].Id, DamegePacketType.Body, broadcast: true);
				}
				else
				{
					ZombiePositionGrid.Kill(botHordeRef, ref tmpForceDir, 0, DamegePacketType.Body, broadcast: true);
				}
			}
			tmpForceDir.Normalize();
			AIBase.AllZombies[i].KillZombie(ref tmpForceDir);
		}
		return result;
	}

	public override bool OwnerPlayerMeleeAttack(int qIndex, ref Vector3 origin, ref Vector3 direction0, WeaponType meleeWep)
	{
		bool flag = false;
		_ = AIBase.AllZombies.Count;
		float num = 19600f;
		float num2 = 10000f;
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		if (playerBase.Sighted)
		{
			num = 19600f;
			num2 = 10000f;
			inDirection = playerBase.CameraDirection * 100f;
			inDirection.Y = 0f;
			inDirection.Normalize();
			tmpPos = PlayerBase.ThirdPersonCameraPos - Vector3.Cross(inDirection, Vector3.UnitY) * PlayerBase.TmpRightShift;
			tmpPos.Y -= 60f;
			tmpPos += inDirection * 100f;
		}
		else
		{
			inDirection = direction0 * 100f;
			inDirection.Y = 0f;
			inDirection.Normalize();
			tmpPos = origin;
		}
		float num3 = (playerBase.ThirdPersonCamera ? 0.8f : 0.825f);
		float num4 = (playerBase.ThirdPersonCamera ? 0.94f : 0.825f);
		num = (playerBase.ThirdPersonCamera ? num : ((playerBase.Speed < 0f) ? (num * 1.1f) : num));
		float num5 = float.MaxValue;
		ZombieBot zombieBot = null;
		for (int i = 0; i < AIBase.MaxZombies; i++)
		{
			if (AIBase.AllZombies[i].Health <= 0)
			{
				continue;
			}
			tmpForceDir = AIBase.AllZombies[i].Position - tmpPos;
			tmpForceDir.Y = ((Math.Abs(tmpForceDir.Y) < 110f) ? 0f : tmpForceDir.Y);
			float num6 = tmpForceDir.LengthSquared();
			if (num6 < num5 && num6 < num)
			{
				tmpForceDir.Normalize();
				if (Vector3.Dot(tmpForceDir, inDirection) > num3)
				{
					num5 = num6;
					zombieBot = (ZombieBot)AIBase.AllZombies[i];
					nAngle = tmpForceDir;
				}
			}
		}
		if (zombieBot != null)
		{
			zombieBot.Health -= ((meleeWep == WeaponType.Hatchet) ? 100 : 10);
			if (zombieBot.Health <= 0)
			{
				ZombieLODEntry botHordeRef = zombieBot.BotHordeRef;
				if (botHordeRef != null)
				{
					if (ZombieLODEntry.NetSession != null)
					{
						ZombiePositionGrid.Kill(botHordeRef, ref tmpForceDir, ZombieLODEntry.NetSession.LocalGamers[0].Id, DamegePacketType.Body, broadcast: true);
					}
					else
					{
						ZombiePositionGrid.Kill(botHordeRef, ref tmpForceDir, 0, DamegePacketType.Body, broadcast: true);
					}
				}
				tmpForceDir.Normalize();
				zombieBot.KillZombie(ref tmpForceDir);
			}
			else if (zombieBot.State != AIBotStates.ZombieHit)
			{
				zombieBot.BotHordeRef.EnterState(AIBotStates.ZombieHit);
				if (ZombieLODEntry.NetSession != null)
				{
					PacketWriter packetWriter = EGENetWorkNext.packetWriter;
					if (ZombieLODEntry.NetSession.IsHost)
					{
						packetWriter.Write((byte)122);
						packetWriter.Write(zombieBot.BotHordeRef._uid);
						packetWriter.Write(zombieBot.BotHordeRef.bAnimation);
						packetWriter.Write(zombieBot.BotHordeRef.bState);
						packetWriter.Write(ZombieLODEntry.NetSession.Host.Id);
					}
					else
					{
						packetWriter.Write((byte)123);
						packetWriter.Write(zombieBot.BotHordeRef._uid);
						packetWriter.Write(zombieBot.BotHordeRef.bAnimation);
						packetWriter.Write((byte)18);
					}
				}
			}
			switch (meleeWep)
			{
			case WeaponType.EmptyHands:
				SwishHitSound.Dispose();
				SwishHitSound = EndGameEngine.SoundBnk.GetCue("FacePunch00");
				SwishHitSound.Play();
				break;
			case WeaponType.Hatchet:
				SwishHitSound.Dispose();
				SwishHitSound = EndGameEngine.SoundBnk.GetCue("AxeHit00");
				SwishHitSound.Play();
				break;
			}
			tmpForceDir = Vector3.UnitY;
			tmpDir = tmpPos;
			if (!playerBase.Sighted)
			{
				tmpDir.Y += 60f;
			}
			tmpDir += inDirection * 80f;
			particles.SpawnBulletHitMutant(ref tmpDir, ref tmpForceDir);
			flag = true;
		}
		if (EGENetWorkNext.networkSession != null && !flag)
		{
			PlayerBase playerBase2 = null;
			int index = 0;
			while ((playerBase2 = EGENetWorkNext.NextNetPlayerReference(ref index)) != null)
			{
				if (!playerBase2.IsAttached0)
				{
					tmpForceDir = playerBase2.vecPosition - tmpPos;
					tmpForceDir.Y = 0f;
					float num7 = tmpForceDir.LengthSquared();
					if (num7 < num2)
					{
						bool flag2 = false;
						tmpForceDir.Normalize();
						if (Vector3.Dot(tmpForceDir, inDirection) > num4)
						{
							flag2 = true;
						}
						if (flag2)
						{
							int num8 = ((meleeWep == WeaponType.Hatchet) ? 40 : 10);
							float num9 = playerBase2.BloodLevel - (float)num8;
							playerBase2.BloodLevel = ((num9 < 0f) ? 0f : num9);
							byte value = ((playerBase2.BloodLoss > 0f) ? ((byte)1) : ((byte)0));
							PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
							packetWriter2.Write((byte)130);
							packetWriter2.Write((byte)5);
							packetWriter2.Write(playerBase2.NetGamerId);
							packetWriter2.Write(num8);
							packetWriter2.Write(value);
							switch (meleeWep)
							{
							case WeaponType.EmptyHands:
								SwishHitSound.Dispose();
								SwishHitSound = EndGameEngine.SoundBnk.GetCue("FacePunch00");
								SwishHitSound.Play();
								break;
							case WeaponType.Hatchet:
								SwishHitSound.Dispose();
								SwishHitSound = EndGameEngine.SoundBnk.GetCue("AxeHit00");
								SwishHitSound.Play();
								break;
							}
							tmpForceDir = Vector3.UnitY;
							tmpDir = tmpPos;
							if (!playerBase.Sighted)
							{
								tmpDir.Y += 60f;
							}
							tmpDir += inDirection * 80f;
							particles.SpawnBulletHitMutant(ref tmpDir, ref tmpForceDir);
							playerBase2.c3rdPersonFacePunchPitch = -8f;
							playerBase2.c3rdPersonFacePunchYaw = -0.35f;
							playerBase2.c3rdPersonFacePunchYaw2 = -1f;
						}
					}
				}
				index++;
			}
		}
		return flag;
	}

	public override Matrix OwnerOverrideCamera(int qIndex, PlayerBase playerRef)
	{
		playerRef.OverridePos = AIBase.camOverridePos;
		playerRef.OverrideDir = AIBase.camOverrideDir;
		playerRef.OverrideUp = Vector3.UnitY;
		playerRef.OverrideRight = Vector3.Cross(AIBase.camOverrideDir, Vector3.UnitY);
		playerRef.vecPosition = AIBase.camOverridePos;
		playerRef.vecDirection = AIBase.camOverrideDir;
		tmpPos = AIBase.camOverridePos;
		tmpPos.X = 0f;
		tmpPos.Z = 0f;
		return Matrix.CreateLookAt(tmpPos, tmpPos + AIBase.camOverrideDir * 1000f, Vector3.UnitY);
	}

	public override void OwnerGamerJoinedSession(NetworkGamer gamer)
	{
		if (EGENetWorkNext.networkSession == null || !EGENetWorkNext.networkSession.IsHost)
		{
			return;
		}
		for (int i = 0; i < AIBase.ScheduledWorldDownloads.Count; i++)
		{
			if (AIBase.ScheduledWorldDownloads[i].client == gamer)
			{
				MessagePump.AddMessage("Gamer Already Scheduled For World Download: " + gamer.Gamertag);
				return;
			}
		}
		PacketWriter packetWriter = EGENetWorkNext.packetWriter;
		ClientTransferDatatCls clientTransferDatatCls = new ClientTransferDatatCls();
		clientTransferDatatCls.client = gamer;
		AIBase.ScheduledWorldDownloads.Add(clientTransferDatatCls);
		packetWriter.Write((byte)104);
		packetWriter.Write(gamer.Id);
		packetWriter.Write((byte)134);
		packetWriter.Write(LevelOutside.SunAngle);
		((LocalNetworkGamer)EGENetWorkNext.networkSession.Host).SendData(packetWriter, SendDataOptions.ReliableInOrder);
	}

	private void UpdateNetWorkEvents()
	{
		for (int i = 0; i < AIBase.ScheduledWorldDownloads.Count; i++)
		{
			if (!(EGENetWorkNext.HostMigrateTimer <= 1f))
			{
				break;
			}
			bool flag = false;
			bool flag2 = false;
			SendVehiclesToClient(AIBase.ScheduledWorldDownloads[i].client, ref AIBase.ScheduledWorldDownloads[i].VehiclesCount);
			if (AIBase.AllWorldItems.UpdateWorldToClient(AIBase.ScheduledWorldDownloads[i].client, ref AIBase.ScheduledWorldDownloads[i].WorldItemAreaIndex, ref AIBase.ScheduledWorldDownloads[i].AreaItemIndex))
			{
				if (AIBase.ScheduledWorldDownloads[i].SendTentLoad)
				{
					AIBase.ScheduledWorldDownloads[i].SendTentLoad = false;
					SendWorldCreateTent(AIBase.ScheduledWorldDownloads[i].client);
				}
				if (SendZombiesToClient(AIBase.ScheduledWorldDownloads[i].client, ref AIBase.ScheduledWorldDownloads[i].ZombieGridX, ref AIBase.ScheduledWorldDownloads[i].ZombieGridZ, ref AIBase.ScheduledWorldDownloads[i].ZombieIndex))
				{
					SendWorldCreateDone(AIBase.ScheduledWorldDownloads[i].client);
					AIBase.ScheduledWorldDownloads.RemoveAt(i);
				}
			}
		}
	}

	public override void OwnerHostCreateWorld(NetworkGamer gamer)
	{
		EGENetWorkNext.HostCreatingWorld = true;
		AIBase.ScheduledWorldDownloads.Clear();
		AIBase.AllWorldItems.Reset();
		AIBase.ResetZombies();
		AIBase.ResetVehicles();
		LevelBaseMenu.PrepareLoadLevel();
		ApocZSaveDataCls.SyncingToServer = false;
		ApocZSaveDataCls.ScheduleWorldItemLoad = true;
		int num = AIBase.MaxZombies / MobileHomeParks.Count;
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			AIBase.AllWorldItems.Setup(gamer, MobileHomeParks[i]);
		}
		ApocZSaveDataCls.SpawnTestEquipment();
		int num2 = 0;
		foreach (WorldAreaCls mobileHomePark in MobileHomeParks)
		{
			foreach (Vector3 zombieSpawnPo in mobileHomePark.ZombieSpawnPos)
			{
				ZombieLODEntry zombieLODEntry = ZombiePositionGrid.AllZombiesInWorld[num2++];
				if (zombieLODEntry == null)
				{
					zombieLODEntry = new ZombieLODEntry();
				}
				zombieLODEntry.zFlags = (byte)EndGameEngine.randGenerator.Next(5);
				zombieLODEntry.pos = zombieSpawnPo;
				ZombiePositionGrid.Add(zombieLODEntry);
			}
		}
		AIBase.HostCreateVehicle(gamer);
	}

	public override void OwnerHostCreateVehicle(NetworkGamer gamer)
	{
		Vector3 normal = Vector3.Zero;
		ItemCls itemCls = new ItemCls();
		itemCls.uid = WorldItemsCls.UniqueId;
		itemCls.desc = 2050;
		itemCls.pos = new Vector3(42480f, 0f, -26500f);
		itemCls.pos.Y = HeightMapPhysics.GetHeight(ref itemCls.pos, out normal);
		OwnerCreateVehicleByType(gamer, itemCls);
		AIBase.AllWorldItems.AddVehicle(gamer, itemCls);
		itemCls = new ItemCls();
		itemCls.uid = WorldItemsCls.UniqueId;
		itemCls.desc = 2052;
		if (Guide.IsTrialMode || LevelBaseMenu.isTrialMode)
		{
			itemCls.pos = new Vector3(42369f, 0f, -7423f);
		}
		else
		{
			itemCls.pos = new Vector3(28800f, 0f, -15760f);
		}
		itemCls.pos.Y = HeightMapPhysics.GetHeight(ref itemCls.pos, out normal);
		OwnerCreateVehicleByType(gamer, itemCls);
		AIBase.AllWorldItems.AddVehicle(gamer, itemCls);
		itemCls = new ItemCls();
		itemCls.uid = WorldItemsCls.UniqueId;
		itemCls.desc = 2049;
		itemCls.pos = new Vector3(-1870f, 0f, -9100f);
		itemCls.pos.Y = HeightMapPhysics.GetHeight(ref itemCls.pos, out normal);
		OwnerCreateVehicleByType(gamer, itemCls);
		AIBase.AllWorldItems.AddVehicle(gamer, itemCls);
		itemCls = new ItemCls();
		itemCls.uid = WorldItemsCls.UniqueId;
		itemCls.desc = 2053;
		itemCls.pos = new Vector3(-34830f, 0f, 17800f);
		itemCls.pos.Y = HeightMapPhysics.GetHeight(ref itemCls.pos, out normal);
		OwnerCreateVehicleByType(gamer, itemCls);
		AIBase.AllWorldItems.AddVehicle(gamer, itemCls);
		itemCls = new ItemCls();
		itemCls.uid = WorldItemsCls.UniqueId;
		itemCls.desc = 2051;
		itemCls.pos = new Vector3(1022f, 0f, -26500f);
		itemCls.pos.Y = HeightMapPhysics.GetHeight(ref itemCls.pos, out normal);
		OwnerCreateVehicleByType(gamer, itemCls);
		AIBase.AllWorldItems.AddVehicle(gamer, itemCls);
	}

	public override void OwnerCreateVehicleByType(NetworkGamer gamer, ItemCls item)
	{
		_ = Vector3.Zero;
		if (item.ItemType == 2)
		{
			if (AIBase.AllVehicles[0] == null)
			{
				AIBase.AllVehicles[0] = new VehicleCls(item);
				AIBase.AllVehicles[0].Direction = -Vector3.UnitZ;
				AIBase.AllVehicles[0].Load("models\\vehicles\\OldTruck");
			}
			else
			{
				AIBase.AllVehicles[0].SetByItemType(item);
				AIBase.AllVehicles[0].Destroy();
				AIBase.AllVehicles[0].Direction = -Vector3.UnitZ;
				AIBase.AllVehicles[0].Set();
			}
			AIBase.AllVehicles[0].maxSpeed = 65f;
			AIBase.AllVehicles[0].EngineSoundName = "VehicleEngineIdle00";
		}
		else if (item.ItemType == 4)
		{
			if (AIBase.AllVehicles[1] == null)
			{
				AIBase.AllVehicles[1] = new VehicleCls(item);
				if (Guide.IsTrialMode || LevelBaseMenu.isTrialMode)
				{
					AIBase.AllVehicles[1].Direction = Vector3.UnitX;
				}
				else
				{
					AIBase.AllVehicles[1].Direction = Vector3.UnitX;
				}
				AIBase.AllVehicles[1].Load("models\\vehicles\\Jeep");
			}
			else
			{
				AIBase.AllVehicles[1].SetByItemType(item);
				AIBase.AllVehicles[1].Destroy();
				if (Guide.IsTrialMode || LevelBaseMenu.isTrialMode)
				{
					AIBase.AllVehicles[1].Direction = Vector3.UnitX;
				}
				else
				{
					AIBase.AllVehicles[1].Direction = Vector3.UnitX;
				}
				AIBase.AllVehicles[1].Set();
			}
			AIBase.AllVehicles[1].maxSpeed = 65f;
			AIBase.AllVehicles[1].IsOffRoadVehicle = true;
			AIBase.AllVehicles[1].EngineSoundName = "VehicleEngineIdle00";
			if (Guide.IsTrialMode || LevelBaseMenu.isTrialMode)
			{
				AIBase.AllVehicles[1].HeadLightOn = true;
			}
		}
		else if (item.ItemType == 1)
		{
			if (AIBase.AllVehicles[2] == null)
			{
				AIBase.AllVehicles[2] = new VehicleCls(item);
				AIBase.AllVehicles[2].Direction = new Vector3(-0.6902457f, 0f, 0.7235751f);
				AIBase.AllVehicles[2].Load("models\\vehicles\\PoliceCar");
			}
			else
			{
				AIBase.AllVehicles[2].SetByItemType(item);
				AIBase.AllVehicles[2].Destroy();
				AIBase.AllVehicles[2].Direction = new Vector3(-0.6902457f, 0f, 0.7235751f);
				AIBase.AllVehicles[2].Set();
			}
			AIBase.AllVehicles[2].maxSpeed = 80f;
			AIBase.AllVehicles[2].EngineSoundName = "PoliceCharger";
		}
		else if (item.ItemType == 5)
		{
			if (AIBase.AllVehicles[3] == null)
			{
				AIBase.AllVehicles[3] = new VehicleCls(item);
				AIBase.AllVehicles[3].Direction = Vector3.UnitX;
				AIBase.AllVehicles[3].Load("models\\vehicles\\Jeep");
			}
			else
			{
				AIBase.AllVehicles[3].SetByItemType(item);
				AIBase.AllVehicles[3].Destroy();
				AIBase.AllVehicles[3].Direction = Vector3.UnitX;
				AIBase.AllVehicles[3].Set();
			}
			AIBase.AllVehicles[3].maxSpeed = 65f;
			AIBase.AllVehicles[3].IsOffRoadVehicle = true;
			AIBase.AllVehicles[3].EngineSoundName = "VehicleEngineIdle00";
		}
		if (item.ItemType == 3)
		{
			if (AIBase.AllVehicles[4] == null)
			{
				AIBase.AllVehicles[4] = new VehicleCls(item);
				AIBase.AllVehicles[4].Direction = Vector3.UnitX;
				AIBase.AllVehicles[4].Load("models\\vehicles\\OldTruck");
			}
			else
			{
				AIBase.AllVehicles[4].SetByItemType(item);
				AIBase.AllVehicles[4].Destroy();
				AIBase.AllVehicles[4].Direction = Vector3.UnitX;
				AIBase.AllVehicles[4].Set();
			}
			AIBase.AllVehicles[4].maxSpeed = 65f;
			AIBase.AllVehicles[4].EngineSoundName = "VehicleEngineIdle00";
		}
	}

	public override void OwnerUpdateVehicleData(PacketReader pReader, int vehicleIndex)
	{
		if (AIBase.AllVehicles[vehicleIndex] != null)
		{
			ushort uid = pReader.ReadUInt16();
			if (AIBase.AllVehicles[vehicleIndex].VehicleItemRef == null)
			{
				AIBase.AllVehicles[vehicleIndex].VehicleItemRef = new ItemCls();
			}
			AIBase.AllVehicles[vehicleIndex].VehicleItemRef.uid = uid;
			AIBase.AllVehicles[vehicleIndex].RFrontWheelQuality = pReader.ReadByte();
			AIBase.AllVehicles[vehicleIndex].LFrontWheelQuality = pReader.ReadByte();
			AIBase.AllVehicles[vehicleIndex].RRearWheelQuality = pReader.ReadByte();
			AIBase.AllVehicles[vehicleIndex].LRearWheelQuality = pReader.ReadByte();
			AIBase.AllVehicles[vehicleIndex].FuelLevel = (int)pReader.ReadByte();
			AIBase.AllVehicles[vehicleIndex].VehicleDamage = pReader.ReadByte();
		}
	}

	public override void OwnerHostSpawnZombies(NetworkGamer gamer, WorldAreaCls world, ref int botIndex, int botCount)
	{
		int num = 0;
		while (botIndex < botCount)
		{
			int index = botIndex;
			ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[index];
			Vector3 pos = Vector3.Zero;
			Vector3 normal = Vector3.UnitY;
			world.GetNextSpawnPosition(ref pos);
			pos.Y = HeightMapPhysics.GetHeight(ref pos, out normal);
			zombieBot.NavMeshRoute[0] = pos;
			zombieBot.NavMeshRoute[0].X -= world.matWorld[0].Translation.X;
			zombieBot.NavMeshRoute[0].Z -= world.matWorld[0].Translation.Z;
			zombieBot.NavMeshStart = 0;
			zombieBot.NavMeshCount = 0;
			zombieBot.SpawnPosition = pos;
			zombieBot.Position = pos;
			zombieBot.LastPosition = Vector3.Zero;
			zombieBot.ForcedToIdleByCollision = false;
			zombieBot.CurrentPathingData = world.PathingData;
			zombieBot.CollisionIndex = num;
			num = ((num < 12) ? (num + 1) : 0);
			zombieBot.Health = 100;
			zombieBot.State = AIBotStates.ZombieWander;
			zombieBot.TransitionTime = 1f;
			zombieBot.mRagdoll.IsValid = false;
			MyMath.RandomVector(ref zombieBot.Direction);
			zombieBot.Direction.Normalize();
			zombieBot.MoveDirection = zombieBot.Direction;
			zombieBot.SpeedScalar = 0.7f + (float)RandGenerator.NextDouble() * 0.3f;
			zombieBot.SpeedScalar *= 1f;
			zombieBot.Spawn();
			botIndex++;
		}
	}

	private bool SpawnApocZombie(PlayerBase playerRef, ZombieLODEntry e, int qIndex)
	{
		int num = 0;
		float num2 = float.MinValue;
		ZombieBot zombieBot = null;
		Vector3 zero = Vector3.Zero;
		_ = Vector3.UnitY;
		int num3 = (int)((float)AIBase.MaxZombies * 0.8f);
		for (int i = 0; i < AIBase.MaxZombies; i++)
		{
			if (NumberBotsCurrentAlive >= num3)
			{
				break;
			}
			if (AIBase.AllZombies[i].Health <= 0)
			{
				float num4 = (AIBase.AllZombies[i].Position - playerRef.vecHeadPosition[qIndex]).LengthSquared();
				if (num4 > num2)
				{
					num2 = num4;
					num = i;
					zombieBot = (ZombieBot)AIBase.AllZombies[i];
				}
			}
		}
		if (zombieBot == null)
		{
			num2 = float.MinValue;
			for (int j = 0; j < AIBase.MaxZombies; j++)
			{
				if (AIBase.AllZombies[j].Health <= 0)
				{
					continue;
				}
				zero = AIBase.AllZombies[j].Position - playerRef.vecHeadPosition[qIndex];
				if (Vector3.Dot(zero, playerRef.CameraDirection) < 0f)
				{
					float num5 = zero.LengthSquared();
					if (num5 > num2)
					{
						num2 = num5;
						num = j;
						zombieBot = (ZombieBot)AIBase.AllZombies[j];
					}
				}
			}
			if (zombieBot != null)
			{
				NumberBotsCurrentAlive--;
				uint zFlags = zombieBot.BotHordeRef.zFlags;
				zFlags &= 0xDF;
				zombieBot.BotHordeRef.zFlags = (byte)zFlags;
			}
		}
		if (zombieBot != null)
		{
			e.zFlags |= 32;
			e.zFlags |= 32;
			zombieBot.PlayAnimation((WeaponAnim)e.bAnimation, randStart: true);
			e.SetAnimation(zombieBot.CurrentAnimationState.AnimationTexture.Height, randStart: true);
			zombieBot.ModelIndex = e.zFlags & 0xF;
			zombieBot.BotHordeRef = e;
			ref Vector3 reference = ref zombieBot.NavMeshRoute[0];
			reference = e.pos;
			zombieBot.NavMeshRoute[0].X = 0f;
			zombieBot.NavMeshRoute[0].Z = 0f;
			zombieBot.NavMeshStart = 0;
			zombieBot.NavMeshCount = 0;
			zombieBot.SpawnPosition = e.pos;
			zombieBot.Position = e.pos;
			zombieBot.LastPosition = e.pos;
			zombieBot.ForcedToIdleByCollision = false;
			zombieBot.CurrentPathingData = null;
			num = ((num < 12) ? (num + 1) : 0);
			zombieBot.CollisionIndex = num;
			zombieBot.Health = 100;
			zombieBot.State = AIBotStates.ZombieWander;
			zombieBot.TransitionTime = 1f;
			zombieBot.mRagdoll.IsValid = false;
			zombieBot.Direction = playerRef.vecPosition - zombieBot.Position;
			zombieBot.Direction.Y = 0f;
			zombieBot.Direction.Normalize();
			zombieBot.MoveDirection = zombieBot.Direction;
			zombieBot.SpeedScalar = 0.7f + (float)RandGenerator.NextDouble() * 0.3f;
			zombieBot.SpeedScalar *= 1f;
			zombieBot.Spawn();
			NumberBotsCurrentAlive++;
			return true;
		}
		return false;
	}

	public override void OwnerHostSendZombieToClient(PacketReader pReader, NetworkGamer gamer)
	{
		byte gamerId = pReader.ReadByte();
		byte bState = pReader.ReadByte();
		byte zFlags = pReader.ReadByte();
		ushort uid = pReader.ReadUInt16();
		Vector3 pos = pReader.ReadVector3();
		NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId);
		if (networkGamer != null && networkGamer.IsLocal)
		{
			ZombieLODEntry zombieLODEntry = new ZombieLODEntry();
			zombieLODEntry.bState = bState;
			zombieLODEntry.zFlags = zFlags;
			zombieLODEntry._uid = uid;
			zombieLODEntry.pos = pos;
			ZombiePositionGrid.Add(zombieLODEntry);
		}
	}

	public bool SendZombiesToClient(NetworkGamer gamer, ref int gx, ref int gz, ref int i)
	{
		int num = 0;
		int num2 = ZombiePositionGrid.AllZombiesInWorld.Length;
		PacketWriter packetWriter = EGENetWorkNext.packetWriter;
		sendZombieIntervalTimer -= 0.0333f;
		if (sendZombieIntervalTimer < 0f)
		{
			sendZombieIntervalTimer = 1f;
			while (i < num2)
			{
				if (ZombiePositionGrid.AllZombiesInWorld[i] != null)
				{
					packetWriter.Write((byte)119);
					packetWriter.Write(gamer.Id);
					packetWriter.Write(ZombiePositionGrid.AllZombiesInWorld[i].bState);
					packetWriter.Write(ZombiePositionGrid.AllZombiesInWorld[i].zFlags);
					packetWriter.Write(ZombiePositionGrid.AllZombiesInWorld[i]._uid);
					packetWriter.Write(ZombiePositionGrid.AllZombiesInWorld[i].pos);
					num++;
					if (num > 32)
					{
						break;
					}
				}
				i++;
			}
			if (num > 0 && packetWriter.Length > 0)
			{
				EGENetWorkNext.ServerSendToClient(packetWriter, gamer);
			}
		}
		return i >= ZombiePositionGrid.AllZombiesInWorld.Length;
	}

	private void SendWorldCreateTent(NetworkGamer gamer)
	{
		if (EGENetWorkNext.networkSession != null)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)112);
			packetWriter.Write(gamer.Id);
			EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
		}
	}

	private void SendWorldCreateDone(NetworkGamer gamer)
	{
		if (EGENetWorkNext.networkSession != null)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)111);
			packetWriter.Write(gamer.Id);
			EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
		}
	}

	private bool SendVehiclesToClient(NetworkGamer gamer, ref int index)
	{
		if (index >= AIBase.NumOfVehicles)
		{
			return true;
		}
		if (EGENetWorkNext.networkSession != null)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			for (int i = 0; i < 1; i++)
			{
				if (index >= AIBase.NumOfVehicles)
				{
					break;
				}
				AIBase.AllVehicles[index].SendVehicleSpawnPacket(packetWriter, index, gamer.Id);
				index++;
			}
		}
		return true;
	}

	public override void OwnerZombieUpdateDestination(PacketReader pReader, NetworkGamer gamer)
	{
		ushort num = pReader.ReadUInt16();
		float wanderTimer = pReader.ReadSingle();
		Vector3 targetPosition = pReader.ReadVector3();
		for (int i = 0; i < AIBase.MaxZombies; i++)
		{
			ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[i];
			if (zombieBot._uid == num)
			{
				zombieBot.TargetPosition = targetPosition;
				zombieBot.WanderTimer = wanderTimer;
				break;
			}
		}
	}

	public override void OwnerZombieDeath(PacketReader pReader, byte senderId, bool netBroadcast)
	{
		ushort num = pReader.ReadUInt16();
		byte b = pReader.ReadByte();
		Vector3 v = pReader.ReadVector3();
		ZombieLODEntry zombieLODEntry = ZombiePositionGrid.AllZombiesInWorld[num - 1];
		if (zombieLODEntry == null)
		{
			return;
		}
		if (b == 7)
		{
			Vector3 n = Vector3.UnitY;
			AIBase.VehicleKillZombie(ref zombieLODEntry.pos, ref n);
		}
		ZombiePositionGrid.Kill(zombieLODEntry, ref v, senderId, (DamegePacketType)b, netBroadcast);
		for (int i = 0; i < AIBase.MaxZombies; i++)
		{
			ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[i];
			if (zombieBot.Health > 0 && zombieBot.BotHordeRef != null && zombieBot.BotHordeRef._uid == num)
			{
				zombieBot.KillZombie(ref v);
				break;
			}
		}
		PacketWriter packetWriter = EGENetWorkNext.packetWriter;
		packetWriter.Write((byte)128);
		packetWriter.Write(num);
	}

	public override void OwnerZombieSetLOS(ushort uid, byte disQuant, NetworkGamer sender)
	{
		if (sender == null || !(sender.Tag is PlayerBase playerBase))
		{
			return;
		}
		ZombieLODEntry zombieLODEntry = ZombiePositionGrid.AllZombiesInWorld[uid - 1];
		if (zombieLODEntry == null)
		{
			return;
		}
		byte zFlags = zombieLODEntry.zFlags;
		for (int i = 0; i < 16; i++)
		{
			if (playerBase.PlayerLineOfSight[i] == 0 || playerBase.PlayerLineOfSight[i] == uid)
			{
				playerBase.PlayerLineOfSight[i] = uid;
				playerBase.PlayerDistanceQuant[i] = disQuant;
				break;
			}
			if ((zFlags & 0x80) == 0 || (zFlags & 0x40) == 0)
			{
				playerBase.PlayerLineOfSight[i] = uid;
				playerBase.PlayerDistanceQuant[i] = disQuant;
				break;
			}
			if (playerBase.PlayerDistanceQuant[i] > disQuant)
			{
				playerBase.PlayerLineOfSight[i] = uid;
				playerBase.PlayerDistanceQuant[i] = disQuant;
				break;
			}
		}
	}

	public override void OwnerZombieUpdatePosition(PacketReader pReader, NetworkGamer gamer, ePacketTypes pType)
	{
		ushort num = pReader.ReadUInt16();
		uint offset = pReader.ReadUInt32();
		byte quant = pReader.ReadByte();
		byte bAnimation = pReader.ReadByte();
		byte b = pReader.ReadByte();
		byte b2 = 0;
		byte b3 = 0;
		if (pType == ePacketTypes.ZombieUpdatePosition)
		{
			b2 = pReader.ReadByte();
			b3 = pReader.ReadByte();
		}
		ZombieLODEntry zombieLODEntry = ZombiePositionGrid.AllZombiesInWorld[num - 1];
		if (zombieLODEntry == null)
		{
			return;
		}
		if (b != zombieLODEntry.bState)
		{
			zombieLODEntry.EnterState((AIBotStates)b);
		}
		zombieLODEntry.bAnimation = bAnimation;
		Vector3 pos = Vector3.Zero;
		HeightMapPhysics.ExspandQuantizedPosition(ref pos, ref offset, ref quant);
		pos.Y = HeightMapPhysics.GetHeight(ref pos);
		if (pType == ePacketTypes.ZombieUpdatePosition)
		{
			float x = (float)(int)b2 * (1f / 127f) - 1f;
			float z = (float)(int)b3 * (1f / 127f) - 1f;
			zombieLODEntry.dir.X = x;
			zombieLODEntry.dir.Z = z;
			pos -= zombieLODEntry.pos;
			float num2 = pos.LengthSquared();
			if (num2 > 10000f)
			{
				zombieLODEntry.pos += pos * 0.75f;
			}
			ZombiePositionGrid.UpdatePosition(zombieLODEntry);
		}
		else
		{
			zombieLODEntry.routeIndex = 0;
			zombieLODEntry.routeCount = 1;
			zombieLODEntry.route[zombieLODEntry.routeIndex] = pos;
		}
	}

	public override void OwnerZombieUpdatePathing(ushort uid, ushort pathingIndex)
	{
		for (int i = 0; i < AIBase.MaxZombies; i++)
		{
			ZombieBot zombieBot = (ZombieBot)AIBase.AllZombies[i];
			if (zombieBot._uid == uid)
			{
				zombieBot.CurrentPathingData = MobileHomeParks[pathingIndex].PathingData;
				break;
			}
		}
	}

	public override dtStatNavMesh.dtStatNavMeshHeader OwnerGetPathingReference(ref Vector3 p)
	{
		float num = float.MaxValue;
		dtStatNavMesh.dtStatNavMeshHeader result = null;
		for (int i = 0; i < MobileHomeParks.Count; i++)
		{
			bool flag = false;
			if (p.X >= MobileHomeParks[i].PathingData.worldMin.X && p.X <= MobileHomeParks[i].PathingData.worldMax.X)
			{
				flag = true;
			}
			bool flag2 = false;
			if (p.Z >= MobileHomeParks[i].PathingData.worldMin.Z && p.Z <= MobileHomeParks[i].PathingData.worldMax.Z)
			{
				flag2 = true;
			}
			if (flag && flag2)
			{
				result = MobileHomeParks[i].PathingData;
				break;
			}
			closestPntToPathing = p;
			if (p.X < MobileHomeParks[i].PathingData.worldMin.X)
			{
				closestPntToPathing.X = MobileHomeParks[i].PathingData.worldMin.X;
			}
			else if (p.X > MobileHomeParks[i].PathingData.worldMax.X)
			{
				closestPntToPathing.X = MobileHomeParks[i].PathingData.worldMax.X;
			}
			if (p.Z < MobileHomeParks[i].PathingData.worldMin.Z)
			{
				closestPntToPathing.Z = MobileHomeParks[i].PathingData.worldMin.Z;
			}
			else if (p.Z > MobileHomeParks[i].PathingData.worldMax.Z)
			{
				closestPntToPathing.Z = MobileHomeParks[i].PathingData.worldMax.Z;
			}
			float num2 = (p - closestPntToPathing).LengthSquared();
			if (num2 < num)
			{
				num = num2;
				result = MobileHomeParks[i].PathingData;
			}
		}
		return result;
	}

	public override void OwnerZombieNewState(byte sender, ushort uid, byte anim, byte state)
	{
		ZombieLODEntry zombieLODEntry = ZombiePositionGrid.AllZombiesInWorld[uid - 1];
		if (zombieLODEntry != null && state != zombieLODEntry.bState)
		{
			zombieLODEntry.EnterState((AIBotStates)state, sender);
		}
	}

	public override bool OwnerZombieRayCastToPlayer(ref Vector3 pos, ref Vector3 dir, dtStatNavMesh.dtStatNavMeshHeader pathing, int qIndex, bool directionTest, PlayerBase focusPlayerRef)
	{
		PlayerBase playerBase = null;
		playerBase = ((focusPlayerRef == null) ? LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value] : focusPlayerRef);
		dirToPlayer = playerBase.vecPosition - pos;
		if (playerBase.Stance == PlayerStance.Crouch)
		{
			dirToPlayer.Y -= 40f;
		}
		else
		{
			dirToPlayer.Y += 0f;
		}
		float num = dirToPlayer.LengthSquared();
		float num2 = playerBase.NoiseLevel * 2250000f;
		if (num < num2)
		{
			return true;
		}
		bool flag = false;
		if (directionTest)
		{
			directionTest = false;
			if (Vector3.Dot(dirToPlayer, dir) > 0f)
			{
				flag = true;
				dirToPlayer.Normalize();
				if (Vector3.Dot(dirToPlayer, dir) > 0.6f)
				{
					directionTest = true;
				}
			}
		}
		else
		{
			directionTest = true;
		}
		if (directionTest && num < 2250000f)
		{
			if (pathing != null)
			{
				for (int i = 0; i < MobileHomeParks.Count; i++)
				{
					if (pathing == MobileHomeParks[i].PathingData)
					{
						raycastHitDistance = float.MaxValue;
						if (!flag)
						{
							dirToPlayer.Normalize();
						}
						raycastRay.Position = pos;
						raycastRay.Position.Y += 60f;
						raycastRay.Direction = dirToPlayer;
						if (!MobileHomeParks[i].RayCast(qIndex, ref raycastRay, ref raycasHitPosition, ref raycasHitNormal, ref raycastHitDistance))
						{
							return true;
						}
						if (raycastHitDistance * raycastHitDistance > num)
						{
							raycastHitDistance = 0f;
							return true;
						}
					}
				}
				return false;
			}
			return true;
		}
		return false;
	}
}
