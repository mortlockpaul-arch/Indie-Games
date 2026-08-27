using System;
using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class BaseData
{
	public class SkinnedInstanceEffectParams
	{
		public EffectParameter EnvMap0;

		public EffectParameter TextureShadowMap;

		public EffectParameter vecEyePosition;

		public EffectParameter vecLightColor;

		public EffectParameter vecAmbientLightColor;

		public EffectParameter vecLightPosition;

		public EffectParameter fSpecularPower;

		public EffectParameter fReflectiveness;

		public EffectParameter matTexProj;

		public EffectParameter matView;

		public EffectParameter matProj;

		public EffectParameter matViewProj;

		public EffectParameter vecFPSLightPos;

		public EffectParameter vecFPSLightColor;

		public EffectParameter matSkinnedWorldTransform;

		public EffectParameter matBones;

		public EffectParameter currentFrame;

		public EffectParameter shadowMapLOD;

		public EffectParameter fadeOutScalar;

		public EffectParameter InstanceTransforms;

		public EffectParameter InstanceAnimFrame;

		public EffectParameter VertexCount;

		public EffectParameter vecMuzzleFlash;

		public EffectParameter AnimationTextureMap;

		public EffectParameter animationBoneFrame;

		public EffectTechnique AnimInstance;

		public EffectParameter textureBloom0;

		public EffectParameter textureBloom1;

		public EffectParameter textureComposite;

		public EffectParameter textureDepth;

		public EffectParameter fogStart;

		public EffectParameter fogEnd;

		public EffectParameter viewPortOverlayScalar;

		public EffectParameter ThermalHeat;

		public SkinnedInstanceEffectParams(Effect e)
		{
			EnvMap0 = e.Parameters["EnvMap0"];
			TextureShadowMap = e.Parameters["TextureShadowMap"];
			vecEyePosition = e.Parameters["vecEyePosition"];
			vecLightColor = e.Parameters["vecLightColor"];
			vecAmbientLightColor = e.Parameters["vecAmbientLightColor"];
			vecLightPosition = e.Parameters["vecLightPosition"];
			fSpecularPower = e.Parameters["fSpecularPower"];
			fReflectiveness = e.Parameters["fReflectiveness"];
			matTexProj = e.Parameters["matTexProj"];
			matView = e.Parameters["matView"];
			matProj = e.Parameters["matProj"];
			matViewProj = e.Parameters["matViewProj"];
			vecFPSLightPos = e.Parameters["vecFPSLightPos"];
			vecFPSLightColor = e.Parameters["vecFPSLightColor"];
			matSkinnedWorldTransform = e.Parameters["matSkinnedWorldTransform"];
			matBones = e.Parameters["matBones"];
			currentFrame = e.Parameters["currentFrame"];
			shadowMapLOD = e.Parameters["shadowMapLOD"];
			fadeOutScalar = e.Parameters["fadeOutScalar"];
			InstanceTransforms = e.Parameters["InstanceTransforms"];
			InstanceAnimFrame = e.Parameters["InstanceAnimFrame"];
			VertexCount = e.Parameters["VertexCount"];
			vecMuzzleFlash = e.Parameters["vecMuzzleFlash"];
			AnimationTextureMap = e.Parameters["AnimationTextureMap"];
			animationBoneFrame = e.Parameters["animationBoneFrame"];
			AnimInstance = e.Techniques["T_SkinnedAnimationInstance"];
			textureBloom0 = e.Parameters["textureBloom0"];
			textureBloom1 = e.Parameters["textureBloom1"];
			textureComposite = e.Parameters["textureComposite"];
			textureDepth = e.Parameters["textureDepth"];
			fogStart = e.Parameters["fogStart"];
			fogEnd = e.Parameters["fogEnd"];
			viewPortOverlayScalar = e.Parameters["viewPortOverlayScalar"];
			ThermalHeat = e.Parameters["ThermalHeat"];
			SetConstants();
		}

		public void SetConstants()
		{
			Vector3 zero = Vector3.Zero;
			zero.X = LevelOutside.SunPosition.X;
			zero.Y = LevelOutside.SunPosition.Y;
			zero.Z = LevelOutside.SunPosition.Z;
			Vector4 value = new Vector4(1f, 1f, 1f, 1f);
			Vector4 value2 = new Vector4(0.35f, 0.35f, 0.4f, 1f);
			if (EnvMap0 != null)
			{
				EnvMap0.SetValue(LevelBaseMenu.EnvMap);
			}
			vecLightPosition.SetValue(zero);
			fSpecularPower.SetValue(2f);
			vecLightColor.SetValue(value);
			vecAmbientLightColor.SetValue(value2);
		}
	}

	public const int MaxBaseModels = 5;

	public const float UPDATE_TIME_STEP_MAX = 1.5f;

	public const float UPDATE_TIME_STEP_MIN = 0.2f;

	public const float WAIT_NOSIGHT_TIME = 2f;

	public const float ATTACK_DISTANCE_MIN = 250000f;

	public const float ZOMBIE_NETWORK_UPDATE_TIMESTEP = 3.5f;

	public const float ZOMBIE_UPDATE_TIMESTEP_MAX = 2.5f;

	public const float ZOMBIE_NOISE_DISSQR_MAX = 2250000f;

	public const float ZOMBIE_SIGHT_DISSQR_MAX = 2250000f;

	public const float ZOMBIE_ATTACK_DISSQR_MIN = 90000f;

	public const float ZOMBIE_ATTACK_DISSQR_MAX = 1000000f;

	public const float TARGET_DISSQR_UPDATE = 30250000f;

	public const float TARGET_DISSQR_RUN = 1000000f;

	public const float TARGET_DISSQR_WALK = 490000f;

	public const float TARGET_DISSQR_MIN = 57600f;

	public const float TARGET_DISSQR_ATTACK = 14400f;

	public const float ALIEN_MIN_DISSQR_ATTACK = 115600f;

	public const float ALIEN_DISSQR_ROLL = 1440000f;

	public const int MAX_NEIGHBORS = 12;

	public const float NEIGHBOR_DIS = 64f;

	public const float NEIGHBOR_DISQR = 4096f;

	public const float NEIGHBOR_STEER_DISQR = 16384f;

	public const int MAX_BOTS_INCOMBAT = 2;

	public const float TARGET_MAXIMUM_DIS = 5800f;

	public const float WEP_CATEGORY_SNIPER_DIS = 4930f;

	public const float WEP_CATEGORY_SUPPORT_DIS = 4640f;

	public const float WEP_CATEGORY_ASSUALT_DIS = 2900f;

	public const float WEP_CATEGORY_SUBMACHINE_DIS = 2900f;

	public const float WEP_CATEGORY_PISTOL_DIS = 2320f;

	public const float WEP_CATEGORY_EQUIPMENT_DIS = 2320f;

	public const float WEP_CATEGORY_SHOTGUN_DIS = 1740.0001f;

	private static ushort UniqueId = 0;

	public ushort _uid;

	public int Health;

	public AIBotStates State;

	public AIStateMachine BotState;

	public InternalStates InternalState;

	public int ModelIndex;

	public int CurrentPathingDataSourceIndex;

	public dtStatNavMesh.dtStatNavMeshHeader CurrentPathingData;

	public bool[] Render = new bool[2];

	public Ragdoll mRagdoll = new Ragdoll();

	public bool InFrustum;

	public float Speed;

	public float SpeedScalar = 1f;

	public float DistTargetSqr = float.MaxValue;

	public float TargetAquireTimer;

	public float UpdateMoveDistanceTimer;

	public float InflictDamageTimer;

	public Vector3 BotOffset = new Vector3(0f, 0f, 0f);

	public Vector3 WeaponOffset = new Vector3(0f, 90f, 0f);

	public Vector3 Position;

	public Vector3 LastPosition;

	public Vector3 Direction;

	public Vector3 TargetDirection;

	public Vector3 TargetPosition;

	public Vector3 TargetLastPosition;

	public Vector3 MoveDirection;

	public Vector3 MovePosition;

	public Vector3 TargetPelvisDirection;

	public PlayerBase TargetPlayer;

	public ZombieLODEntry BotHordeRef;

	public float[] PlayerDistanceSqr = new float[2];

	public int SelectedAttackPoint;

	public int NavMeshStart;

	public int NavMeshCount;

	public Vector3[] NavMeshRoute = new Vector3[dtStatNavMesh.MAX_POLYS];

	public static ushort[] routePolys = new ushort[dtStatNavMesh.MAX_POLYS];

	public bool inShadowLOD = true;

	public float shadowMapLOD;

	public WeaponAnim CurrentAnimation;

	public WeaponAnim PreviosAnimation;

	public float PrevFrameIndex;

	public Texture2D AnimTexture;

	public float[] FrameIndex2 = new float[2];

	public WeaponAnim CurrentAnimation2;

	public Texture2D AnimTexture2;

	public Animation AnimPlayer = new Animation();

	public int nNeighbors;

	public float NeighborSpeedAdjust;

	public float[] NeighborDisSqrList = new float[12];

	public Vector3[] NeighborVectorList = new Vector3[12];

	public TriggerFlags triggerFlags;

	public GeometryFlags geometryFlags;

	public Vector3 geometryNormal;

	public bool GetCollision;

	public bool ApplyGravity;

	public float GravityAccumulator;

	public float UpdateTimer;

	public bool UpdateTimerTripped;

	public float UpdateNetworkTimeStep;

	public float UpdateNetworkTimer;

	public bool UpdateNetworkTimerTripped;

	public float IdleTimer;

	public float WanderTimer;

	public bool ForcedToIdleByCollision;

	public int WanderGenericIndex;

	public float HuntPlayerTimer;

	public int HuntGenericIndex;

	public bool NeedHeightMapUpdate = true;

	public bool BeingAttacked;

	public BaseData OtherBotToAttack;

	public int NumBotsAttacking;

	public int NumBotsPursuing;

	public float foutValue;

	public float foutValueDirection = 1f;

	public Vector4 ThermalHeat = new Vector4(1f, 1f, 1f, 0f);

	public bool popoutFlag;

	public int coverPositionIndex;

	public bool coverPositionRequested;

	public bool coverPositionAquired;

	public bool coverPositionFailed;

	public bool coverRePosition;

	public float attackRollTimer;

	public float popoutTimer;

	public float coverTimer;

	public float transitionStateTimer;

	public Vector3 popoutPosition;

	public Vector3 coverPosition;

	public Vector3 coverDirection;

	public Vector3 combatRollDirection;

	public float DistanceScalar;

	public static float TimeStepMultiplyer = 4f;

	public static Random RandGenerator = new Random();

	public static int NumberBotInCombat = 0;

	protected static IntersectSegmentParams segParams;

	public static List<TargetStructure> TagetsList = new List<TargetStructure>();

	public static List<BaseData> AllBotsList = new List<BaseData>();

	public BotWeapon Weapon;

	public bool HaveSight;

	public float FireResponseTimer;

	public float PullTriggerTimer;

	public float WaitNoSightTimer;

	private Vector3 vecDir = Vector3.Zero;

	private Vector3 vecBotHeight = Vector3.Zero;

	private Vector3 vecPlayerHeight = Vector3.Zero;

	private float tstboth = 90f;

	private float tstbotl;

	private float tstplrh = 40f;

	private float tstplrl;

	private static Vector3 tmpRollDir = Vector3.Zero;

	private static Vector3 tmpRollPos = Vector3.Zero;

	public static ushort GetUniqueId
	{
		get
		{
			UniqueId++;
			return UniqueId;
		}
		set
		{
			UniqueId = 0;
		}
	}

	public FPSAnimationState CurrentAnimationState => AnimPlayer.m_Anims0[(int)CurrentAnimation];

	public FPSAnimationState PreviosAnimationState => AnimPlayer.m_Anims0[(int)PreviosAnimation];

	public BaseData(bool useWeapon)
	{
		Health = 0;
		WanderTimer = 0f;
		InFrustum = false;
		UpdateTimer = 0f;
		UpdateTimerTripped = false;
		SelectedAttackPoint = -1;
		NavMeshStart = 0;
		NavMeshCount = 0;
		TargetAquireTimer = 0f;
		Direction = Vector3.UnitX;
		TargetDirection = Vector3.UnitX;
		MoveDirection = Vector3.UnitX;
		TargetPelvisDirection = Vector3.UnitX;
		PlayerDistanceSqr[0] = float.MaxValue;
		PlayerDistanceSqr[1] = float.MaxValue;
		for (int i = 0; i < 2; i++)
		{
			FrameIndex2[i] = 0f;
		}
		if (useWeapon)
		{
			Weapon = new BotWeapon(this);
			Weapon.CurrentWeaponType = WeaponType.NewTech;
			Weapon.FireWeapon = false;
		}
		popoutFlag = false;
		coverPositionIndex = -1;
		coverPositionRequested = false;
		coverPositionAquired = false;
		coverPositionFailed = false;
		coverRePosition = false;
		attackRollTimer = 0f;
		popoutPosition = Vector3.Zero;
		coverPosition = Vector3.Zero;
		coverDirection = Vector3.Zero;
		combatRollDirection = Vector3.Zero;
		transitionStateTimer = 0f;
		AllBotsList.Add(this);
	}

	public virtual void Initialize(Vector3 position)
	{
	}

	public virtual void Spawn()
	{
		SelectedAttackPoint = -1;
		UpdateTimer = (float)RandGenerator.NextDouble() * 1.5f;
		Speed = 0f;
		NavMeshStart = 0;
		NavMeshCount = 0;
		BotOffset = Vector3.Zero;
		TargetPelvisDirection = Direction;
		UpdateTimer = (float)EndGameEngine.randGenerator.NextDouble() * 2.5f;
		UpdateNetworkTimer = (float)EndGameEngine.randGenerator.NextDouble() * 3.5f;
		UpdateTimerTripped = false;
		UpdateNetworkTimerTripped = false;
		FireResponseTimer = 0f;
		popoutFlag = false;
		coverPositionIndex = -1;
		coverPositionRequested = false;
		coverPositionAquired = false;
		coverPositionFailed = false;
		coverRePosition = false;
		attackRollTimer = 0f;
		popoutPosition = Vector3.Zero;
		coverPosition = Vector3.Zero;
		coverDirection = Vector3.Zero;
		combatRollDirection = Vector3.Zero;
		popoutTimer = 0f;
		coverTimer = 0f;
		transitionStateTimer = 0f;
	}

	public void Update(float etime, int qIndex, bool getCollision)
	{
	}

	public void UpdateBase(float eTime, int qIndex)
	{
		attackRollTimer -= eTime;
		DistanceScalar = DistTargetSqr / 1000000f;
		DistanceScalar = ((DistanceScalar < 1f) ? DistanceScalar : 1f);
		UpdateTimer += eTime;
		UpdateTimerTripped = false;
		float num = DistanceScalar * 1.5f + 0.2f;
		if (UpdateTimer >= num)
		{
			UpdateTimer = 0f;
			UpdateTimerTripped = true;
		}
		if (BotHordeRef.CurrentFrameLoopCount > 0)
		{
			if (CurrentAnimation == WeaponAnim.CoOpReload)
			{
				if (Weapon != null)
				{
					Weapon.Reload();
				}
				CurrentAnimation = WeaponAnim.CoOpIdle;
				AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
				AnimTexture = CurrentAnimationState.AnimationTexture;
			}
			if (CurrentAnimation == WeaponAnim.CoOpRollLeft || CurrentAnimation == WeaponAnim.CoOpRollRight)
			{
				if (State == AIBotStates.FindCoverPoint)
				{
					CurrentAnimation = WeaponAnim.CoOpRun;
					AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
					AnimTexture = CurrentAnimationState.AnimationTexture;
				}
				else if (State == AIBotStates.AttackRollIntoStaticPoint)
				{
					CurrentAnimation = WeaponAnim.CoOpCrouch;
					AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
					AnimTexture = CurrentAnimationState.AnimationTexture;
				}
				else
				{
					CurrentAnimation = WeaponAnim.CoOpIdle;
					AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
					AnimTexture = CurrentAnimationState.AnimationTexture;
				}
			}
			if (CurrentAnimation == WeaponAnim.CoOpSideStepLeft || CurrentAnimation == WeaponAnim.CoOpSideStepRight)
			{
				if (State == AIBotStates.AttackCoverPoint)
				{
					CurrentAnimation = WeaponAnim.CoOpIdle;
					AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
					AnimTexture = CurrentAnimationState.AnimationTexture;
				}
				else
				{
					CurrentAnimation = WeaponAnim.CoOpIdle;
					AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
					AnimTexture = CurrentAnimationState.AnimationTexture;
				}
			}
		}
		if (Weapon != null)
		{
			Weapon.UpdateWeapon(eTime, qIndex);
		}
	}

	public void PlayAnimation(WeaponAnim anim, bool randStart)
	{
		PreviosAnimation = CurrentAnimation;
		CurrentAnimation = anim;
		AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
		AnimTexture = CurrentAnimationState.AnimationTexture;
	}

	public PlayerBase ClosestLOSOnNetPlayer()
	{
		float num = float.MaxValue;
		PlayerBase result = null;
		if (EGENetWorkNext.networkSession != null)
		{
			for (int i = 0; i < EGENetWorkNext.networkSession.AllGamers.Count; i++)
			{
				NetworkGamer networkGamer = EGENetWorkNext.networkSession.AllGamers[i];
				if (networkGamer == null || !(EGENetWorkNext.networkSession.AllGamers[i].Tag is PlayerBase playerBase))
				{
					continue;
				}
				for (int j = 0; j < 16; j++)
				{
					if (playerBase.PlayerLineOfSight[j] == BotHordeRef._uid)
					{
						float num2 = (playerBase.vecPosition - Position).LengthSquared();
						if (num2 < num)
						{
							playerBase.PlayerLineOfSight[j] = 0;
							num = num2;
							result = playerBase;
						}
						break;
					}
				}
			}
		}
		return result;
	}

	public void UpdateAnimation()
	{
		if (BotHordeRef.bState == 17)
		{
			PlayAnimation((WeaponAnim)BotHordeRef.bAnimation, randStart: false);
			PrevFrameIndex = BotHordeRef.SetAnimation(AnimTexture.Height, randStart: false);
		}
		else if (BotHordeRef.bState == 18)
		{
			PlayAnimation((WeaponAnim)BotHordeRef.bAnimation, randStart: false);
			PrevFrameIndex = BotHordeRef.SetAnimation(AnimTexture.Height, randStart: false);
		}
		else if (BotHordeRef.bState == 16)
		{
			BotHordeRef.bTimer = 8f;
			PlayAnimation((WeaponAnim)BotHordeRef.bAnimation, randStart: true);
			PrevFrameIndex = BotHordeRef.SetAnimation(AnimTexture.Height, randStart: true);
		}
		else
		{
			PlayAnimation((WeaponAnim)BotHordeRef.bAnimation, randStart: true);
			PrevFrameIndex = BotHordeRef.SetAnimation(AnimTexture.Height, randStart: true);
		}
	}

	public virtual void TriggerHit(TriggerFlags eFlags)
	{
	}

	public virtual void KillZombie(ref Vector3 direction)
	{
	}

	public virtual void KillZombie()
	{
		BotOffset = Vector3.Zero;
		TargetAquireTimer = 0f;
		if (SelectedAttackPoint >= 0)
		{
			SpawnPoints.ToggleAttackPoint(SelectedAttackPoint, e: false);
			SelectedAttackPoint = -1;
		}
	}

	public virtual void Reset()
	{
		TargetAquireTimer = 0f;
		SelectedAttackPoint = -1;
		InflictDamageTimer = 0f;
	}

	public virtual int RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, WeaponClass weapon)
	{
		return 0;
	}

	public virtual void CoverPositionAquired_CallBack(CoverPointRequestCls e)
	{
		if (!e.moveCloser)
		{
			coverPositionRequested = false;
			coverPositionAquired = false;
			coverPositionFailed = true;
		}
		if (e.curResultIndex >= 0)
		{
			NavMeshStart = 0;
			NavMeshCount = LevelBaseMenu.NavigationMesh.GetPath(CurrentPathingData, ref Position, ref e.coverPosition, routePolys, NavMeshRoute, randomDestination: true);
			if (NavMeshCount > 0)
			{
				if (coverPositionAquired)
				{
					CoverPoints.SetOccupiedFlag(coverPositionIndex, e: false);
				}
				coverPositionIndex = e.curResultIndex;
				coverPositionAquired = true;
				coverPositionFailed = false;
				transitionStateTimer = 0f;
				coverPosition = e.coverPosition;
				coverDirection = e.coverDirection;
				CoverPoints.SetOccupiedFlag(coverPositionIndex, e: true);
				CoverPoints.GetPopoutPosition(coverPositionIndex, ref popoutPosition);
				if (e.moveCloser)
				{
					WaitNoSightTimer = 0f;
					e.requestOwner.BotState.SetInternalReference(e.requestOwner);
					e.requestOwner.BotState.ExitState(AIStateMachine.allStates[7]);
				}
			}
		}
		else
		{
			coverRePosition = false;
		}
	}

	public virtual bool TryPullTrigger(float eTime)
	{
		Weapon.FireWeapon = false;
		if (Weapon.BulletsInMag <= 0)
		{
			return false;
		}
		if (TargetPlayer != null)
		{
			FireResponseTimer -= eTime;
			if (FireResponseTimer < 0f)
			{
				PullTriggerTimer -= eTime;
				if (PullTriggerTimer > 0f)
				{
					UpdateTimer += eTime * 4f;
					if (HaveSight)
					{
						Weapon.FireWeapon = true;
					}
				}
				else
				{
					FireResponseTimer = (float)RandGenerator.NextDouble() * DistanceScalar;
					if (Weapon.CurrentWeapon.fireMode == WeaponFireMode.SemiAuto)
					{
						PullTriggerTimer = eTime * 4f;
					}
					else
					{
						PullTriggerTimer = 1f + (float)RandGenerator.NextDouble();
					}
				}
			}
		}
		return true;
	}

	public virtual void Reload()
	{
		if (Weapon.BulletsInMag <= 0 && CurrentAnimation != WeaponAnim.CoOpReload && !Weapon.IsShooting())
		{
			CurrentAnimation = WeaponAnim.CoOpReload;
			AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
			AnimTexture = CurrentAnimationState.AnimationTexture;
		}
	}

	public virtual bool CombatRoll()
	{
		combatRollDirection = TargetDirection;
		combatRollDirection.Y = 0f;
		combatRollDirection.Normalize();
		combatRollDirection = Vector3.Cross(combatRollDirection, Vector3.UnitY);
		WeaponAnim currentAnimation = WeaponAnim.CoOpRollRight;
		if (Vector3.Dot(combatRollDirection, MoveDirection) < 0f)
		{
			combatRollDirection *= -1f;
		}
		else
		{
			currentAnimation = WeaponAnim.CoOpRollLeft;
		}
		tmpRollPos = Position + combatRollDirection * -280f;
		if (LevelBaseMenu.NavigationMesh.findNearestPoly(ref tmpRollPos, ref LevelBaseMenu.NavigationMesh.PickExtents) != 0)
		{
			segParams.OnlyWalkable = true;
			segParams.SegmentDirection = -combatRollDirection;
			segParams.SegmentLength = 320f;
			segParams.SegmentStart = Position;
			segParams.SegmentStart.Y += 40f;
			segParams.SegmentEnd = segParams.SegmentStart + segParams.SegmentDirection * segParams.SegmentLength;
			segParams.PreComputeParameters();
			if (LevelOutside.RayCast(0, ref segParams, spawnSparks: false) != MaterialType.Undefined)
			{
				attackRollTimer = (float)RandGenerator.NextDouble() * 8f + 16f;
				Weapon.Reload();
				CurrentAnimation = currentAnimation;
				AnimPlayer.PlayAnimation(CurrentAnimation, force: true);
				AnimTexture = CurrentAnimationState.AnimationTexture;
				WaitNoSightTimer = 0f;
				BotState.SetInternalReference(this);
				BotState.ExitState(AIStateMachine.allStates[10]);
				return true;
			}
		}
		return false;
	}
}
