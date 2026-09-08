using System.IO;
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

	private static bool NETWORK_PREDICTION_ON;

	private static int MAX_FRAME_LAG_RECORD;

	public static Model[] characterBase;

	public static Model[] fpsHandsBase;

	private string gt;

	public UploadLeaderboardState MyLeaderboardState;

	public byte CharacterIndex;

	public byte currentCharacterIndex;

	public FPS_NET_FLAGS PlayerFlags;

	public FPS_NET_FLAGS ServerFlags;

	public int CurrentTeam;

	public float Speed;

	public float SideStep;

	public float lastSpeed;

	public float AngleTorsoCharacter;

	public float AnimBlend;

	public Vector3 Angles;

	public Vector3 vecCharacterDir;

	public Vector3 vecDirection;

	public Vector3 vecFlatDirection;

	public Vector3 vecMoveDirection;

	public Vector3 vecRight;

	public Vector3 vecUp;

	public Vector3 vecPosition;

	public Vector3 tmpPrevPosition;

	public Vector3 vec3rdPersonMuzzlePos;

	public Vector3 SpawnPosition;

	public Vector3 SpawnDirection;

	public float c3rdPersonFireWeaponRecoil;

	public float c3rdPersonFacePunchPitch;

	public float c3rdPersonFacePunchYaw;

	public float c3rdPersonFacePunchYaw2;

	public bool CameraSet;

	public Vector3 CameraAngles;

	public Vector3 CameraDirection;

	public float PlayerInputPredictionTimer;

	public float NetworkUpdateTimer;

	public bool IsHost;

	public byte NetGamerId;

	public NetworkGamer NetGamerRef;

	public int NumberLives;

	public float Health;

	public float HealthRecovery;

	public bool isFirstSpawn;

	public bool ThermalScope;

	public bool ThirdPersonCamera;

	public bool RunToggled;

	public bool ToggledRespawn;

	public float AimAssistTimer;

	public BaseData AimAssistTarget;

	public bool TargetPraticeMessage;

	public bool AvRStartMessage;

	public Vector2 InputRightStick;

	public Vector2 InputLeftStick;

	public float PlayerControllerSensitivity;

	public WeaponType currentWeaponType;

	public WeaponAnim tmpMergeAnim;

	public WeaponAnim tmpAnimation;

	public Model character;

	public Animation cPlayer;

	public FPSWeaponBase fpsWeapon;

	public WeaponType PrimaryWeapon;

	public WeaponType SecondaryWeapon;

	public int NumberFragGrenades;

	public int NumberSmokeGrenades;

	public int NumberNaderGrenades;

	public int NumberThrowingKnife;

	public PlayerGamerTag playerTag;

	public float losTimer;

	public LOSDataStruct[] losOtherPlayers;

	public float ZombieAlertScalar;

	public bool isReady;

	public bool isNetworkPlayer;

	public bool IsModerator;

	public bool ModeratorDrawAllGamerTags;

	public int recordNFramesIndex;

	public int[] recordFramesSinceLastUpdate;

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

	public Vector3[] vecNetworkPositions;

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

	public int TotalPoints;

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

	public float MatchCoolDownTimer;

	public float RespawnTimer;

	public float RespawnDelayTimer;

	public float DeathTimer;

	public float PacketRecievDelayTimer;

	public float RESPAWN_TIME;

	public bool[] Render3rdPerson;

	public bool[] RenderRagdoll;

	public float fBlurFactor;

	public static int numBytesSent;

	public static int numBytesRecv;

	public static int writeCounter;

	public static int numWritesPerSec;

	public Ragdoll mRagdoll;

	private static HalfVector2 readPak0;

	private static HalfVector4 readPak1;

	private static HalfVector4 readPak2;

	private static HalfVector4 readPak3;

	private static Vector2 readUnpacker0;

	private static Vector4 readUnpacker1;

	private static Matrix userTransform;

	private static MediaStruct tmpMediaStruct;

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
		gt = "Guest";
		CurrentTeam = -1;
		Angles = Vector3.Zero;
		vecCharacterDir = Vector3.UnitZ;
		vecDirection = Vector3.UnitZ;
		vecFlatDirection = Vector3.UnitZ;
		vecMoveDirection = Vector3.Zero;
		tmpPrevPosition = Vector3.Zero;
		vec3rdPersonMuzzlePos = Vector3.Zero;
		SpawnPosition = Vector3.Zero;
		SpawnDirection = Vector3.Zero;
		CameraAngles = Vector3.Zero;
		CameraDirection = Vector3.UnitZ;
		NumberLives = 1;
		Health = 100f;
		HealthRecovery = 5f;
		isFirstSpawn = true;
		InputRightStick = Vector2.Zero;
		InputLeftStick = Vector2.Zero;
		PlayerControllerSensitivity = 1f;
		currentWeaponType = WeaponType.NineMil;
		tmpMergeAnim = WeaponAnim.Invalid;
		tmpAnimation = WeaponAnim.CoOpIdle;
		cPlayer = new Animation();
		fpsWeapon = new FPSWeaponBase();
		PrimaryWeapon = WeaponType.NumOfWeapons;
		SecondaryWeapon = WeaponType.NineMil;
		NumberFragGrenades = 1;
		NumberSmokeGrenades = 1;
		NumberNaderGrenades = 1;
		NumberThrowingKnife = 1;
		playerTag = new PlayerGamerTag();
		losOtherPlayers = new LOSDataStruct[32];
		recordFramesSinceLastUpdate = new int[MAX_FRAME_LAG_RECORD];
		vecNetworkPositions = new Vector3[4];
		TotalPoints = 20000;
		MatchCoolDownTimer = -1f;
		RESPAWN_TIME = 3f;
		Render3rdPerson = new bool[2];
		RenderRagdoll = new bool[2];
		mRagdoll = new Ragdoll();
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
		currentCharacterIndex = ((BinaryReader)packet).ReadByte();
		NumKillsThisMatch = ((BinaryReader)packet).ReadByte();
		NumDeathsThisMatch = ((BinaryReader)packet).ReadByte();
		PlayerFlags = (FPS_NET_FLAGS)((BinaryReader)packet).ReadUInt32();
		currentWeaponType = (WeaponType)((BinaryReader)packet).ReadByte();
		tmpMergeAnim = (WeaponAnim)((BinaryReader)packet).ReadSByte();
		tmpAnimation = (WeaponAnim)((BinaryReader)packet).ReadSByte();
		readPak0.PackedValue = ((BinaryReader)packet).ReadUInt32();
		readPak1.PackedValue = ((BinaryReader)packet).ReadUInt64();
		readPak2.PackedValue = ((BinaryReader)packet).ReadUInt64();
		readPak3.PackedValue = ((BinaryReader)packet).ReadUInt64();
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
		((BinaryWriter)packet).Write(currentCharacterIndex);
		((BinaryWriter)packet).Write((byte)NumKillsThisMatch);
		((BinaryWriter)packet).Write((byte)NumDeathsThisMatch);
		((BinaryWriter)packet).Write((uint)PlayerFlags);
		((BinaryWriter)packet).Write((byte)fpsWeapon.CurrentWeapon.WepType);
		((BinaryWriter)packet).Write((sbyte)tmpMergeAnim);
		((BinaryWriter)packet).Write((sbyte)cPlayer.CurrentAnimation);
		HalfVector2 halfVector = default(HalfVector2);
		halfVector = new HalfVector2(AnimBlend, AngleTorsoCharacter);
		HalfVector4 halfVector2 = default(HalfVector4);
		halfVector2 = new HalfVector4(Angles.X, Angles.Y, Angles.Z, vecPosition.X);
		HalfVector4 halfVector3 = default(HalfVector4);
		halfVector3 = new HalfVector4(vecPosition.Y, vecPosition.Z, vecCharacterDir.X, vecCharacterDir.Y);
		HalfVector4 halfVector4 = default(HalfVector4);
		halfVector4 = new HalfVector4(vecCharacterDir.Z, vecMoveDirection.X, vecMoveDirection.Y, vecMoveDirection.Z);
		((BinaryWriter)packet).Write(halfVector.PackedValue);
		((BinaryWriter)packet).Write(halfVector2.PackedValue);
		((BinaryWriter)packet).Write(halfVector3.PackedValue);
		((BinaryWriter)packet).Write(halfVector4.PackedValue);
		tmpMergeAnim = WeaponAnim.Invalid;
	}

	public void ReadServerPacket(PacketReader packet, bool updateAnimation)
	{
		Health = (int)((BinaryReader)packet).ReadByte();
		currentCharacterIndex = ((BinaryReader)packet).ReadByte();
		NumKillsThisMatch = ((BinaryReader)packet).ReadByte();
		NumDeathsThisMatch = ((BinaryReader)packet).ReadByte();
		ServerFlags = (FPS_NET_FLAGS)((BinaryReader)packet).ReadUInt32();
		PlayerFlags = (FPS_NET_FLAGS)((BinaryReader)packet).ReadUInt32();
		currentWeaponType = (WeaponType)((BinaryReader)packet).ReadByte();
		tmpMergeAnim = (WeaponAnim)((BinaryReader)packet).ReadSByte();
		tmpAnimation = (WeaponAnim)((BinaryReader)packet).ReadSByte();
		readPak0.PackedValue = ((BinaryReader)packet).ReadUInt32();
		readPak1.PackedValue = ((BinaryReader)packet).ReadUInt64();
		readPak2.PackedValue = ((BinaryReader)packet).ReadUInt64();
		readPak3.PackedValue = ((BinaryReader)packet).ReadUInt64();
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
		((BinaryWriter)packet).Write((byte)Health);
		((BinaryWriter)packet).Write(currentCharacterIndex);
		((BinaryWriter)packet).Write((byte)NumKillsThisMatch);
		((BinaryWriter)packet).Write((byte)NumDeathsThisMatch);
		((BinaryWriter)packet).Write((uint)ServerFlags);
		((BinaryWriter)packet).Write((uint)PlayerFlags);
		((BinaryWriter)packet).Write((byte)fpsWeapon.CurrentWeapon.WepType);
		((BinaryWriter)packet).Write((sbyte)tmpMergeAnim);
		((BinaryWriter)packet).Write((sbyte)cPlayer.CurrentAnimation);
		HalfVector2 halfVector = default(HalfVector2);
		halfVector = new HalfVector2(AnimBlend, AngleTorsoCharacter);
		HalfVector4 halfVector2 = default(HalfVector4);
		halfVector2 = new HalfVector4(Angles.X, Angles.Y, Angles.Z, vecPosition.X);
		HalfVector4 halfVector3 = default(HalfVector4);
		halfVector3 = new HalfVector4(vecPosition.Y, vecPosition.Z, vecCharacterDir.X, vecCharacterDir.Y);
		HalfVector4 halfVector4 = default(HalfVector4);
		halfVector4 = new HalfVector4(vecCharacterDir.Z, vecMoveDirection.X, vecMoveDirection.Y, vecMoveDirection.Z);
		((BinaryWriter)packet).Write(halfVector.PackedValue);
		((BinaryWriter)packet).Write(halfVector2.PackedValue);
		((BinaryWriter)packet).Write(halfVector3.PackedValue);
		((BinaryWriter)packet).Write(halfVector4.PackedValue);
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

	static PlayerBaseState()
	{
		NETWORK_PREDICTION_ON = true;
		MAX_FRAME_LAG_RECORD = 64;
		numBytesSent = 0;
		numBytesRecv = 0;
		writeCounter = 0;
		numWritesPerSec = 0;
		readPak0 = default(HalfVector2);
		readPak1 = default(HalfVector4);
		readPak2 = default(HalfVector4);
		readPak3 = default(HalfVector4);
		readUnpacker0 = Vector2.Zero;
		readUnpacker1 = Vector4.Zero;
		userTransform = Matrix.Identity;
		tmpMediaStruct = default(MediaStruct);
	}
}
