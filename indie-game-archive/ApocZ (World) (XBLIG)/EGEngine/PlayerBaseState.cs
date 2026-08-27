using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class PlayerBaseState
{
	public bool IsValid;

	public bool IsSplitScreen;

	private static bool NETWORK_PREDICTION_ON = true;

	private static int MAX_FRAME_LAG_RECORD = 64;

	public static Model[] characterBase;

	public static Model[] fpsHandsBase;

	private string gt = "Guest";

	public UploadLeaderboardState MyLeaderboardState;

	public byte CharacterIndex;

	public byte currentCharacterIndex;

	public FPS_NET_FLAGS PlayerFlags;

	public FPS_NET_FLAGS ServerFlags;

	public int CurrentTeam = -1;

	public float Speed;

	public float SideStep;

	public float lastSpeed;

	public float AngleTorsoCharacter;

	public float AnimBlend;

	public Vector3 Angles = Vector3.Zero;

	public Vector3 vecCharacterDir = Vector3.UnitZ;

	public Vector3 vecDirection = Vector3.UnitZ;

	public Vector3 vecFlatDirection = Vector3.UnitZ;

	public Vector3 vecMoveDirection = Vector3.Zero;

	public Vector3 vecRight;

	public Vector3 vecUp;

	public Vector3 vecPosition;

	public Vector3 tmpPrevPosition = Vector3.Zero;

	public Vector3 vec3rdPersonMuzzlePos = Vector3.Zero;

	public Vector3 SpawnPosition = Vector3.Zero;

	public Vector3 SpawnDirection = Vector3.Zero;

	public float c3rdPersonFireWeaponRecoil;

	public float c3rdPersonFacePunchPitch;

	public float c3rdPersonFacePunchYaw;

	public float c3rdPersonFacePunchYaw2;

	public bool CameraSet;

	public Vector3 CameraAngles = Vector3.Zero;

	public Vector3 CameraDirection = Vector3.UnitZ;

	public float PlayerInputPredictionTimer;

	public float NetworkUpdateTimer;

	public bool IsHost;

	public byte NetGamerId;

	public NetworkGamer NetGamerRef;

	public int NumberLives = 1;

	public float Health = 100f;

	public float HealthRecovery = 5f;

	public bool isFirstSpawn = true;

	public bool ThermalScope;

	public bool ThirdPersonCamera;

	public bool RunToggled;

	public bool ToggledRespawn;

	public float AimAssistTimer;

	public BaseData AimAssistTarget;

	public bool TargetPraticeMessage;

	public bool AvRStartMessage;

	public Vector2 InputRightStick = Vector2.Zero;

	public Vector2 InputLeftStick = Vector2.Zero;

	public float PlayerControllerSensitivity = 1f;

	public WeaponType currentWeaponType = WeaponType.NineMil;

	public WeaponAnim tmpMergeAnim = WeaponAnim.Invalid;

	public WeaponAnim tmpAnimation = WeaponAnim.CoOpIdle;

	public Model character;

	public Animation cPlayer = new Animation();

	public FPSWeaponBase fpsWeapon = new FPSWeaponBase();

	public WeaponType PrimaryWeapon = WeaponType.NumOfWeapons;

	public WeaponType SecondaryWeapon = WeaponType.NineMil;

	public int NumberFragGrenades = 1;

	public int NumberSmokeGrenades = 1;

	public int NumberNaderGrenades = 1;

	public int NumberThrowingKnife = 1;

	public PlayerGamerTag playerTag = new PlayerGamerTag();

	public float losTimer;

	public LOSDataStruct[] losOtherPlayers = new LOSDataStruct[32];

	public float ZombieAlertScalar;

	public bool isReady;

	public bool isNetworkPlayer;

	public bool IsModerator;

	public bool ModeratorDrawAllGamerTags;

	public int recordNFramesIndex;

	public int[] recordFramesSinceLastUpdate = new int[MAX_FRAME_LAG_RECORD];

	public int numFramesSinceLastUpdate;

	public int sumFramesSinceLastUpdate;

	public float currentTimeStep;

	public int TargetFrameCounter;

	public float TargetAngleTorso;

	public Vector3 vecTargetAngles;

	public Vector3 vecCurrentPosition;

	public Vector3 vecTargetPosition;

	public Vector3 vecTargetCharDirection;

	public float PreviousSpeed;

	public float PreviousSideStep;

	public float PreviouslastSpeed;

	public Vector3[] vecNetworkPositions = new Vector3[4];

	public int NumPointsThisMatch;

	public int NumKillStreakBalistic;

	public int NumKillStreakGrenade;

	public int NumKillStreakKnife;

	public int NumKillsThisMatch;

	public int NumDeathsThisMatch;

	public bool SniperScopeUnlocked;

	public bool HolographicSightUnlocked;

	public bool SmokeGrenadesUnlocked;

	public int TrialScore;

	public int SurvivorScore;

	public int TotalNumberHeadShots;

	public int TotalNumberKills;

	public int TotalNumberDeaths;

	public int TotalPoints = 20000;

	public int NumberKnifeKills;

	public int NumberPistolKills;

	public int NumberRifleKills;

	public int NumberGrenadeKills;

	public float PlayerArmor;

	public float CommandoSpeed;

	public float RunEndurance;

	public float RunSpeed;

	public float WeaponAccuracey;

	public float WeaponDamage;

	public float tmpCommandoSpeed;

	public float tmpRunEndurance;

	public float tmpRunSpeed;

	public float tmpWeaponAccuracey;

	public int MenuSelected;

	public PlayerMenuState MenuState;

	public bool SpawnSetAngles;

	public bool Spawned;

	public bool SpawnRequested;

	public float MatchCoolDownTimer = -1f;

	public float RespawnTimer;

	public float RespawnDelayTimer;

	public float DeathTimer;

	public float PacketRecievDelayTimer;

	public float RESPAWN_TIME = 3f;

	public bool[] Render3rdPerson = new bool[2];

	public bool[] RenderRagdoll = new bool[2];

	public float fBlurFactor;

	public static int numBytesSent = 0;

	public static int numBytesRecv = 0;

	public static int writeCounter = 0;

	public static int numWritesPerSec = 0;

	public Ragdoll mRagdoll = new Ragdoll();

	private static HalfVector2 readPak0 = default(HalfVector2);

	private static HalfVector4 readPak1 = default(HalfVector4);

	private static HalfVector4 readPak2 = default(HalfVector4);

	private static HalfVector4 readPak3 = default(HalfVector4);

	private static Vector2 readUnpacker0 = Vector2.Zero;

	private static Vector4 readUnpacker1 = Vector4.Zero;

	private static Matrix userTransform = Matrix.Identity;

	private static MediaStruct tmpMediaStruct = default(MediaStruct);

	public string gamerTag
	{
		get
		{
			return gt;
		}
		set
		{
			gt = value;
		}
	}

	public PlayerBaseState()
	{
		for (int i = 0; i < MAX_FRAME_LAG_RECORD; i++)
		{
			recordFramesSinceLastUpdate[i] = 0;
		}
	}

	public void ValidateNewPosition()
	{
		SpawnPosition = vecPosition;
		vecCharacterDir = vecDirection;
		SpawnDirection = vecDirection;
		vecRight = Vector3.Cross(vecDirection, Vector3.UnitY);
		vecUp = Vector3.UnitY;
	}

	public void ReadPlayerPacket(PacketReader packet, bool updateAnimation)
	{
		numBytesRecv = packet.Length;
		currentCharacterIndex = packet.ReadByte();
		NumKillsThisMatch = packet.ReadByte();
		NumDeathsThisMatch = packet.ReadByte();
		PlayerFlags = (FPS_NET_FLAGS)packet.ReadUInt32();
		currentWeaponType = (WeaponType)packet.ReadByte();
		tmpMergeAnim = (WeaponAnim)packet.ReadSByte();
		tmpAnimation = (WeaponAnim)packet.ReadSByte();
		readPak0.PackedValue = packet.ReadUInt32();
		readPak1.PackedValue = packet.ReadUInt64();
		readPak2.PackedValue = packet.ReadUInt64();
		readPak3.PackedValue = packet.ReadUInt64();
		readUnpacker0 = readPak0.ToVector2();
		AnimBlend = readUnpacker0.X;
		AngleTorsoCharacter = readUnpacker0.Y;
		readUnpacker1 = readPak1.ToVector4();
		Angles.X = readUnpacker1.X;
		Angles.Y = readUnpacker1.Y;
		Angles.Z = readUnpacker1.Z;
		vecPosition.X = readUnpacker1.W;
		readUnpacker1 = readPak2.ToVector4();
		vecPosition.Y = readUnpacker1.X;
		vecPosition.Z = readUnpacker1.Y;
		vecCharacterDir.X = readUnpacker1.Z;
		vecCharacterDir.Y = readUnpacker1.W;
		readUnpacker1 = readPak3.ToVector4();
		vecCharacterDir.Z = readUnpacker1.X;
		vecMoveDirection.X = readUnpacker1.Y;
		vecMoveDirection.Y = readUnpacker1.Z;
		vecMoveDirection.Z = readUnpacker1.W;
	}

	public void ClientWritePlayerPacket(PacketWriter packet)
	{
		packet.Write(currentCharacterIndex);
		packet.Write((byte)NumKillsThisMatch);
		packet.Write((byte)NumDeathsThisMatch);
		packet.Write((uint)PlayerFlags);
		packet.Write((byte)fpsWeapon.CurrentWeapon.WepType);
		packet.Write((sbyte)tmpMergeAnim);
		packet.Write((sbyte)cPlayer.CurrentAnimation);
		HalfVector2 halfVector = new HalfVector2(AnimBlend, AngleTorsoCharacter);
		HalfVector4 halfVector2 = new HalfVector4(Angles.X, Angles.Y, Angles.Z, vecPosition.X);
		HalfVector4 halfVector3 = new HalfVector4(vecPosition.Y, vecPosition.Z, vecCharacterDir.X, vecCharacterDir.Y);
		HalfVector4 halfVector4 = new HalfVector4(vecCharacterDir.Z, vecMoveDirection.X, vecMoveDirection.Y, vecMoveDirection.Z);
		packet.Write(halfVector.PackedValue);
		packet.Write(halfVector2.PackedValue);
		packet.Write(halfVector3.PackedValue);
		packet.Write(halfVector4.PackedValue);
		tmpMergeAnim = WeaponAnim.Invalid;
	}

	public void ReadServerPacket(PacketReader packet, bool updateAnimation)
	{
		Health = (int)packet.ReadByte();
		currentCharacterIndex = packet.ReadByte();
		NumKillsThisMatch = packet.ReadByte();
		NumDeathsThisMatch = packet.ReadByte();
		ServerFlags = (FPS_NET_FLAGS)packet.ReadUInt32();
		PlayerFlags = (FPS_NET_FLAGS)packet.ReadUInt32();
		currentWeaponType = (WeaponType)packet.ReadByte();
		tmpMergeAnim = (WeaponAnim)packet.ReadSByte();
		tmpAnimation = (WeaponAnim)packet.ReadSByte();
		readPak0.PackedValue = packet.ReadUInt32();
		readPak1.PackedValue = packet.ReadUInt64();
		readPak2.PackedValue = packet.ReadUInt64();
		readPak3.PackedValue = packet.ReadUInt64();
		readUnpacker0 = readPak0.ToVector2();
		AnimBlend = readUnpacker0.X;
		AngleTorsoCharacter = readUnpacker0.Y;
		readUnpacker1 = readPak1.ToVector4();
		Angles.X = readUnpacker1.X;
		Angles.Y = readUnpacker1.Y;
		Angles.Z = readUnpacker1.Z;
		vecPosition.X = readUnpacker1.W;
		readUnpacker1 = readPak2.ToVector4();
		vecPosition.Y = readUnpacker1.X;
		vecPosition.Z = readUnpacker1.Y;
		vecCharacterDir.X = readUnpacker1.Z;
		vecCharacterDir.Y = readUnpacker1.W;
		readUnpacker1 = readPak3.ToVector4();
		vecCharacterDir.Z = readUnpacker1.X;
		vecMoveDirection.X = readUnpacker1.Y;
		vecMoveDirection.Y = readUnpacker1.Z;
		vecMoveDirection.Z = readUnpacker1.W;
	}

	public void ServerWritePlayerPacket(PacketWriter packet)
	{
		packet.Write((byte)Health);
		packet.Write(currentCharacterIndex);
		packet.Write((byte)NumKillsThisMatch);
		packet.Write((byte)NumDeathsThisMatch);
		packet.Write((uint)ServerFlags);
		packet.Write((uint)PlayerFlags);
		packet.Write((byte)fpsWeapon.CurrentWeapon.WepType);
		packet.Write((sbyte)tmpMergeAnim);
		packet.Write((sbyte)cPlayer.CurrentAnimation);
		HalfVector2 halfVector = new HalfVector2(AnimBlend, AngleTorsoCharacter);
		HalfVector4 halfVector2 = new HalfVector4(Angles.X, Angles.Y, Angles.Z, vecPosition.X);
		HalfVector4 halfVector3 = new HalfVector4(vecPosition.Y, vecPosition.Z, vecCharacterDir.X, vecCharacterDir.Y);
		HalfVector4 halfVector4 = new HalfVector4(vecCharacterDir.Z, vecMoveDirection.X, vecMoveDirection.Y, vecMoveDirection.Z);
		packet.Write(halfVector.PackedValue);
		packet.Write(halfVector2.PackedValue);
		packet.Write(halfVector3.PackedValue);
		packet.Write(halfVector4.PackedValue);
		tmpMergeAnim = WeaponAnim.Invalid;
		PlayerFlags = FPS_NET_FLAGS.Clear;
	}

	public void ServerUpdatePlayer(PlayerBaseState e)
	{
		if (!(PacketRecievDelayTimer > 0f))
		{
			PlayerFlags = e.PlayerFlags;
			currentWeaponType = e.currentWeaponType;
			tmpMergeAnim = e.tmpMergeAnim;
			tmpAnimation = e.tmpAnimation;
			AnimBlend = e.AnimBlend;
			AngleTorsoCharacter = e.AngleTorsoCharacter;
			Angles = e.Angles;
			vecPosition = e.vecPosition;
			vecCharacterDir = e.vecCharacterDir;
			vecMoveDirection = e.vecMoveDirection;
		}
	}

	public void ServerUpdateNetPlayer(PlayerBaseState e, bool updateAnimation)
	{
		if (PacketRecievDelayTimer > 0f)
		{
			return;
		}
		if (NETWORK_PREDICTION_ON)
		{
			sumFramesSinceLastUpdate -= recordFramesSinceLastUpdate[recordNFramesIndex];
			sumFramesSinceLastUpdate += numFramesSinceLastUpdate;
			recordFramesSinceLastUpdate[recordNFramesIndex] = numFramesSinceLastUpdate;
			numFramesSinceLastUpdate = 0;
			recordNFramesIndex++;
			if (recordNFramesIndex >= MAX_FRAME_LAG_RECORD)
			{
				recordNFramesIndex = 0;
				sumFramesSinceLastUpdate = 0;
				for (int i = 0; i < MAX_FRAME_LAG_RECORD; i++)
				{
					sumFramesSinceLastUpdate += recordFramesSinceLastUpdate[i];
				}
			}
		}
		if (Spawned)
		{
			if (NETWORK_PREDICTION_ON)
			{
				vecTargetPosition = e.vecPosition - vecPosition;
				if (vecTargetPosition.Length() > 1000f)
				{
					vecPosition = e.vecPosition;
					vecTargetPosition = vecPosition;
				}
				else
				{
					vecTargetPosition += vecPosition;
				}
				vecTargetCharDirection = e.vecCharacterDir;
				TargetAngleTorso = e.AngleTorsoCharacter;
				vecTargetAngles = e.Angles;
			}
			else
			{
				AngleTorsoCharacter = e.AngleTorsoCharacter;
				Angles = e.Angles;
				vecTargetPosition = e.vecPosition;
				vecPosition = e.vecPosition;
				vecCharacterDir = e.vecCharacterDir;
			}
			NumKillsThisMatch = e.NumKillsThisMatch;
			NumDeathsThisMatch = e.NumDeathsThisMatch;
			PlayerFlags = e.PlayerFlags;
			ServerFlags |= (PlayerFlags & FPS_NET_FLAGS.FireAuto) | (PlayerFlags & FPS_NET_FLAGS.FireWeapon);
			currentWeaponType = e.currentWeaponType;
			fpsWeapon.SetWeapon(currentWeaponType);
			tmpMergeAnim = e.tmpMergeAnim;
			tmpAnimation = e.tmpAnimation;
			AnimBlend = e.AnimBlend;
			vecMoveDirection = e.vecMoveDirection;
			if (currentCharacterIndex != e.currentCharacterIndex)
			{
				SetCurrentCharacter(e.currentCharacterIndex);
			}
		}
		if (updateAnimation)
		{
			cPlayer.PlayAnimation(tmpAnimation, force: false);
			if (tmpMergeAnim != WeaponAnim.Invalid)
			{
				cPlayer.PlayMergedAnimation(tmpMergeAnim);
			}
		}
	}

	public void ClientUpdateNetPlayer(PlayerBaseState e, bool updateAnimation)
	{
		if (PacketRecievDelayTimer > 0f)
		{
			return;
		}
		if (NETWORK_PREDICTION_ON)
		{
			sumFramesSinceLastUpdate -= recordFramesSinceLastUpdate[recordNFramesIndex];
			sumFramesSinceLastUpdate += numFramesSinceLastUpdate;
			recordFramesSinceLastUpdate[recordNFramesIndex] = numFramesSinceLastUpdate;
			numFramesSinceLastUpdate = 0;
			recordNFramesIndex++;
			if (recordNFramesIndex >= MAX_FRAME_LAG_RECORD)
			{
				recordNFramesIndex = 0;
				sumFramesSinceLastUpdate = 0;
				for (int i = 0; i < MAX_FRAME_LAG_RECORD; i++)
				{
					sumFramesSinceLastUpdate += recordFramesSinceLastUpdate[i];
				}
			}
		}
		ServerFlags = e.ServerFlags;
		if (Spawned)
		{
			if (NETWORK_PREDICTION_ON)
			{
				vecTargetPosition = e.vecPosition - vecPosition;
				if (vecTargetPosition.Length() > 1000f)
				{
					vecPosition = e.vecPosition;
					vecTargetPosition = vecPosition;
				}
				else
				{
					vecTargetPosition += vecPosition;
				}
				vecTargetCharDirection = e.vecCharacterDir;
				TargetAngleTorso = e.AngleTorsoCharacter;
				vecTargetAngles = e.Angles;
			}
			else
			{
				AngleTorsoCharacter = e.AngleTorsoCharacter;
				Angles = e.Angles;
				vecTargetPosition = e.vecPosition;
				vecPosition = e.vecPosition;
				vecCharacterDir = e.vecCharacterDir;
			}
			NumKillsThisMatch = e.NumKillsThisMatch;
			NumDeathsThisMatch = e.NumDeathsThisMatch;
			PlayerFlags = e.PlayerFlags;
			PlayerFlags |= (ServerFlags & FPS_NET_FLAGS.FireAuto) | (ServerFlags & FPS_NET_FLAGS.FireWeapon);
			currentWeaponType = e.currentWeaponType;
			fpsWeapon.SetWeapon(currentWeaponType);
			tmpMergeAnim = e.tmpMergeAnim;
			tmpAnimation = e.tmpAnimation;
			AnimBlend = e.AnimBlend;
			vecMoveDirection = e.vecMoveDirection;
			if (currentCharacterIndex != e.currentCharacterIndex)
			{
				SetCurrentCharacter(e.currentCharacterIndex);
			}
		}
		if (updateAnimation)
		{
			cPlayer.PlayAnimation(tmpAnimation, force: false);
			if (tmpMergeAnim != WeaponAnim.Invalid)
			{
				cPlayer.PlayMergedAnimation(tmpMergeAnim);
				tmpMergeAnim = WeaponAnim.Invalid;
			}
		}
		e.tmpMergeAnim = WeaponAnim.Invalid;
		ServerFlags &= (FPS_NET_FLAGS)(-13);
	}

	public void SetCurrentCharacter(byte e)
	{
		if (e >= 0 && e < characterBase.Length)
		{
			currentCharacterIndex = e;
			character = characterBase[currentCharacterIndex];
			cPlayer.SetCharacter(character, currentCharacterIndex);
			cPlayer.SetBaseAnimation(WeaponAnim.CoOpIdleEmpty);
			fpsWeapon.hands = fpsHandsBase[currentCharacterIndex];
			fpsWeapon.fpsAmin.SetCharacter(fpsHandsBase[currentCharacterIndex], 0);
			fpsWeapon.fpsAmin.SetBaseAnimation(fpsWeapon.CurrentWeapon.IdleAnim);
		}
	}

	public virtual void ProcessDeath(DamegePacketType damageType, ref Vector3 damageDir)
	{
	}

	public virtual void UpdateHealth(float eTimeMS)
	{
		if (Health > 0f || Spawned)
		{
			Health += eTimeMS * HealthRecovery;
			Health = ((Health > 100f) ? 100f : Health);
		}
		else
		{
			Health = 0f;
		}
	}

	public virtual void ResetMatch()
	{
		isFirstSpawn = true;
		NumKillStreakGrenade = 0;
		NumKillStreakBalistic = 0;
		NumKillStreakKnife = 0;
		NumKillsThisMatch = 0;
		NumDeathsThisMatch = 0;
	}

	public virtual void Reset()
	{
		Spawned = false;
		SpawnRequested = false;
		ToggledRespawn = false;
		IsModerator = false;
		ModeratorDrawAllGamerTags = false;
		FPSGameMenu.isCurrentScore = false;
		MenuState = PlayerMenuState.Idle;
		MenuSelected = 0;
		fpsWeapon.Reset();
		NumberThrowingKnife = 1;
		NumberFragGrenades = 1;
		NumberNaderGrenades = 1;
		MatchCoolDownTimer = -1f;
		if (SmokeGrenadesUnlocked)
		{
			NumberSmokeGrenades = 1;
		}
		else
		{
			NumberSmokeGrenades = 0;
		}
		NumKillStreakGrenade = 0;
		NumKillStreakBalistic = 0;
		NumKillStreakKnife = 0;
		NumKillsThisMatch = 0;
		NumDeathsThisMatch = 0;
		DeathTimer = -1f;
	}
}
