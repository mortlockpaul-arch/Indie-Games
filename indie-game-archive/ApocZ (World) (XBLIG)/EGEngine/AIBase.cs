using System;
using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class AIBase
{
	public static int AIObjectSegmentIndex = -1;

	public static bool WaveSpawned = false;

	public static bool WaveCoolingDown = false;

	public static float ClosestBotDistanceSqr = float.MaxValue;

	public static Vector3 camOverridePos = new Vector3(-1111f, 300f, 1062f);

	public static Vector3 camOverrideDir = Vector3.UnitZ;

	public static WorldItemsCls AllWorldItems = new WorldItemsCls();

	public static ConsumableCls AllConsumables = new ConsumableCls();

	public static EquipmentCls AllEquipmentItems = new EquipmentCls();

	public static WeaponsCls AllWeapons = new WeaponsCls();

	public static ClanCls Clans = new ClanCls();

	public static int MaxZombies = 36;

	public static List<BaseData> AllZombies = new List<BaseData>();

	public static int NumOfVehicles = 5;

	public static VehicleCls[] AllVehicles = new VehicleCls[NumOfVehicles];

	public static InventoryCls PlayerInventory = new InventoryCls();

	public static List<ClientTransferDatatCls> ScheduledWorldDownloads = new List<ClientTransferDatatCls>();

	public static float TimeOfDayMultiplyer = 0.7f;

	protected static float UpdateTimer = 0f;

	protected static bool Initialized = false;

	protected static AIBase Owner = null;

	protected Random RandGenerator = new Random();

	public static float DispHelpInfo = 0f;

	public static float BlackFadeTimer = -1f;

	public static float LocalOffLineMessage = 0f;

	public static float SpectatorDispTimer = 0f;

	private static HalfVector2 readPHV2 = default(HalfVector2);

	private static HalfVector4 readPHV4 = default(HalfVector4);

	private static NormalizedByte4 readNB4_0 = default(NormalizedByte4);

	private static NormalizedByte4 readNB4_1 = default(NormalizedByte4);

	private static Vector2 readUnpackerV2 = Vector2.Zero;

	private static Vector4 readUnpackerV4 = Vector4.Zero;

	private static Cue VehImpactSnd = null;

	private static Vector3 tmpPos = Vector3.Zero;

	public virtual void Initialize()
	{
		if (!Initialized)
		{
			Initialized = true;
			AllWorldItems.LoadContent();
			PlayerInventory.Load("");
			Clans.LoadContent();
		}
	}

	~AIBase()
	{
	}

	public static void LoadWorldItemAssets()
	{
		AllConsumables.Load("");
		AllEquipmentItems.Load("");
		AllWeapons.Load("");
		PlayerInventory.Load("");
	}

	public static bool IsVehicleMenuOpen()
	{
		bool flag = false;
		if (VehicleCls.VehicleMenuOpen)
		{
			for (int i = 0; i < NumOfVehicles; i++)
			{
				if (AllVehicles[i].InCanAttachDistance)
				{
					flag = true;
				}
				else
				{
					AllVehicles[i].DisplayCanAttachText = false;
				}
			}
		}
		VehicleCls.VehicleMenuOpen = flag;
		return flag;
	}

	public static void AddAIObjects(int e)
	{
		AIObjectSegmentIndex = e;
	}

	public virtual void StartWave(float eTime)
	{
	}

	public virtual void EndWave(float eTime)
	{
	}

	public virtual void ResetWave()
	{
		WaveSpawned = false;
	}

	public virtual void PlayerInTrigger(TriggerFlags f, PlayerBase playerRef)
	{
	}

	public virtual void SpawnBot(int nBotsToSpawn)
	{
	}

	public virtual void Update(float etime, int qIndex)
	{
		LevelObjectives.Update();
		GenericMessages.Update();
	}

	public virtual void UpdateRagdoll(float etime, int qIndex)
	{
	}

	public virtual void Draw(PlayerBase playerRef, int qIndex)
	{
	}

	public virtual void DrawShadowMap(PlayerBase playerRef, ref Matrix LightViewProj, ref Vector3 lightPos, int qIndex)
	{
	}

	public virtual void DrawAlpha(PlayerBase playerRef, int qIndex)
	{
	}

	public virtual void DrawPostDistortion(PlayerBase playerRef, int qIndex)
	{
	}

	public virtual void DrawPost(int qIndex, PlayerBase playerRef)
	{
	}

	public virtual void DrawInLevelUI(int qIndex)
	{
	}

	public static bool RayCastGeometry(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPos, ref Vector3 hitNorm)
	{
		if (Owner != null)
		{
			return Owner.OwnerRayCastGeometry(qIndex, ref origin, ref direction, ref hitPos, ref hitNorm);
		}
		return false;
	}

	public virtual bool OwnerRayCastGeometry(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPos, ref Vector3 hitNorm)
	{
		return false;
	}

	public static bool RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, WeaponClass weapon, ref float testDistance)
	{
		if (Owner != null)
		{
			return Owner.OwnerRayCast(qIndex, ref origin, ref direction, weapon, ref testDistance);
		}
		return false;
	}

	public virtual bool OwnerRayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, WeaponClass weapon, ref float testDistance)
	{
		return false;
	}

	public static void GrenadeExplode(int qIndex, ref Vector3 origin)
	{
		if (Owner != null)
		{
			Owner.OwnerGrenadeExplode(qIndex, ref origin);
		}
	}

	public virtual void OwnerGrenadeExplode(int qIndex, ref Vector3 origin)
	{
	}

	public static void FlameThrower(int qIndex, ref Vector3 origin)
	{
		if (Owner != null)
		{
			Owner.OwnerFlameThrower(qIndex, ref origin);
		}
	}

	public virtual void OwnerFlameThrower(int qIndex, ref Vector3 origin)
	{
	}

	public static void Javlin(int qIndex, ref Vector3 origin, ref Vector3 normal, bool fired)
	{
		if (Owner != null)
		{
			Owner.OwnerJavlin(qIndex, ref origin, ref normal, fired);
		}
	}

	public virtual void OwnerJavlin(int qIndex, ref Vector3 origin, ref Vector3 normal, bool fired)
	{
	}

	public static void BaitBomb(int qIndex, ref Vector3 position, bool fired)
	{
		if (Owner != null)
		{
			Owner.OwnerBaitBomb(qIndex, ref position, fired);
		}
	}

	public virtual void OwnerBaitBomb(int qIndex, ref Vector3 position, bool fired)
	{
	}

	public static void PlayerOutTrigger(TriggerFlags f, PlayerBase playerRef)
	{
		if ((f | TriggerFlags.AISafeHouse) > TriggerFlags.Clear)
		{
			playerRef.triggerFlags &= (TriggerFlags)(-17);
			AIStateMachine.SetAttackPlayerEnable(e: true);
		}
	}

	public static bool PlayerMeleeAttack(int qIndex, ref Vector3 origin, ref Vector3 direction, WeaponType meleeWep)
	{
		if (Owner != null)
		{
			return Owner.OwnerPlayerMeleeAttack(qIndex, ref origin, ref direction, meleeWep);
		}
		return false;
	}

	public virtual bool OwnerPlayerMeleeAttack(int qIndex, ref Vector3 origin, ref Vector3 direction, WeaponType meleeWep)
	{
		return false;
	}

	public static bool PlayerAttackKnife(int qIndex, ref Vector3 origin, ref Vector3 direction)
	{
		if (Owner != null)
		{
			return Owner.OwnerPlayerAttackKnife(qIndex, ref origin, ref direction);
		}
		return false;
	}

	public virtual bool OwnerPlayerAttackKnife(int qIndex, ref Vector3 origin, ref Vector3 direction)
	{
		return false;
	}

	public static bool PlayerAttackSword(int qIndex, ref Vector3 origin, ref Vector3 direction)
	{
		if (Owner != null)
		{
			return Owner.OwnerPlayerAttackSword(qIndex, ref origin, ref direction);
		}
		return false;
	}

	public virtual bool OwnerPlayerAttackSword(int qIndex, ref Vector3 origin, ref Vector3 direction)
	{
		return false;
	}

	public static void PlayerDeath(PlayerBaseState playerRef)
	{
		if (Owner != null)
		{
			if ((PlayerBase)playerRef == LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value])
			{
				WorldItemsCls.TentMenuOpen = false;
				VehicleCls.VehicleMenuOpen = false;
				PlayerInventory.DropAll((PlayerBase)playerRef);
				Storage.DeleteFile(Storage.PlayerInventoryFilename);
				Storage.SavePlayerStatus();
				DetachPlayerFromVehicle((PlayerBase)playerRef, 0, 0);
			}
			Owner.OwnerPlayerDeath((PlayerBase)playerRef);
		}
	}

	public virtual void OwnerPlayerDeath(PlayerBase playerRef)
	{
	}

	public static void KillSound()
	{
		if (Owner != null)
		{
			Owner.OwnerKillSound();
		}
	}

	public virtual void OwnerKillSound()
	{
	}

	public static BaseData GetAimAssistVector(int qIndex, PlayerBase playerRef, ref Vector3 direction)
	{
		if (Owner != null)
		{
			return Owner.OwnerGetAimAssistVector(qIndex, playerRef, ref direction);
		}
		return null;
	}

	public virtual BaseData OwnerGetAimAssistVector(int qIndex, PlayerBase playerRef, ref Vector3 direction)
	{
		return null;
	}

	public static Matrix OverrideCamera(int qIndex, PlayerBase playerRef)
	{
		if (Owner != null)
		{
			return Owner.OwnerOverrideCamera(qIndex, playerRef);
		}
		return Matrix.Identity;
	}

	public virtual Matrix OwnerOverrideCamera(int qIndex, PlayerBase playerRef)
	{
		return Matrix.Identity;
	}

	public static bool IsWeaponValid(PlayerBase playerRef, WeaponType wepType)
	{
		if (Owner != null)
		{
			return Owner.OwnerIsWeaponValid(playerRef, wepType);
		}
		return false;
	}

	public virtual bool OwnerIsWeaponValid(PlayerBase playerRef, WeaponType wepType)
	{
		return false;
	}

	public static void UpdateMusicVolume()
	{
		if (Owner != null)
		{
			Owner.OwnerUpdateMusicVolume();
		}
	}

	public virtual void OwnerUpdateMusicVolume()
	{
	}

	public static void HostCreateWorld(NetworkGamer gamer)
	{
		if (Owner != null)
		{
			Owner.OwnerHostCreateWorld(gamer);
		}
	}

	public virtual void OwnerHostCreateWorld(NetworkGamer gamer)
	{
	}

	public static void GamerJoinedSession(NetworkGamer gamer)
	{
		if (Owner != null)
		{
			Owner.OwnerGamerJoinedSession(gamer);
		}
	}

	public virtual void OwnerGamerJoinedSession(NetworkGamer gamer)
	{
	}

	public static void ResetVehicles()
	{
		DestroyVehicles();
		HostCreateVehicle(null);
	}

	public static void DestroyVehicles()
	{
		for (int i = 0; i < NumOfVehicles; i++)
		{
			if (AllVehicles[i] != null)
			{
				AllVehicles[i].Destroy();
			}
		}
	}

	public static void HostCreateVehicle(NetworkGamer gamer)
	{
		if (Owner != null)
		{
			Owner.OwnerHostCreateVehicle(gamer);
		}
	}

	public virtual void OwnerHostCreateVehicle(NetworkGamer gamer)
	{
	}

	public static void ResetZombies()
	{
		ZombiePositionGrid.Reset();
		for (int i = 0; i < MaxZombies; i++)
		{
			BaseData baseData = AllZombies[i];
			baseData.Health = 0;
			baseData.mRagdoll.IsValid = false;
			baseData.mRagdoll.SetRagdoll = false;
			baseData.Reset();
		}
	}

	public static void HostSpawnZombies(NetworkGamer gamer, WorldAreaCls world, ref int botIndex, int botCount)
	{
		if (Owner != null)
		{
			Owner.OwnerHostSpawnZombies(gamer, world, ref botIndex, botCount);
		}
	}

	public virtual void OwnerHostSpawnZombies(NetworkGamer gamer, WorldAreaCls world, ref int botIndex, int botCount)
	{
	}

	public static void HostSendZombieToClient(PacketReader pReader, NetworkGamer gamer)
	{
		if (Owner != null)
		{
			Owner.OwnerHostSendZombieToClient(pReader, gamer);
		}
	}

	public virtual void OwnerHostSendZombieToClient(PacketReader pReader, NetworkGamer gamer)
	{
	}

	public static void ZombieUpdateDestination(PacketReader pReader, NetworkGamer gamer)
	{
		if (Owner != null)
		{
			Owner.OwnerZombieUpdateDestination(pReader, gamer);
		}
	}

	public virtual void OwnerZombieUpdateDestination(PacketReader pReader, NetworkGamer gamer)
	{
	}

	public static void ZombieUpdatePosition(PacketReader pReader, NetworkGamer gamer, ePacketTypes pType)
	{
		if (Owner != null)
		{
			Owner.OwnerZombieUpdatePosition(pReader, gamer, pType);
		}
	}

	public virtual void OwnerZombieUpdatePosition(PacketReader pReader, NetworkGamer gamer, ePacketTypes pType)
	{
	}

	public static void ZombieDeath(PacketReader pReader, byte senderId, bool netBroadcast)
	{
		if (Owner != null)
		{
			Owner.OwnerZombieDeath(pReader, senderId, netBroadcast);
		}
	}

	public virtual void OwnerZombieDeath(PacketReader pReader, byte senderId, bool netBroadcast)
	{
	}

	public static void ZombieSetLOS(ushort uid, byte disQuant, NetworkGamer sender)
	{
		if (Owner != null)
		{
			Owner.OwnerZombieSetLOS(uid, disQuant, sender);
		}
	}

	public virtual void OwnerZombieSetLOS(ushort uid, byte disQuant, NetworkGamer sender)
	{
	}

	public static bool ZombieRayCastToPlayer(BaseData e, int qIndex, bool directionTest, PlayerBase focusPlayerRef)
	{
		if (Owner != null)
		{
			return Owner.OwnerZombieRayCastToPlayer(ref e.Position, ref e.Direction, e.CurrentPathingData, qIndex, directionTest, focusPlayerRef);
		}
		return false;
	}

	public static bool ZombieRayCastToPlayer(ref Vector3 pos, ref Vector3 dir, dtStatNavMesh.dtStatNavMeshHeader pathing, int qIndex, bool directionTest, PlayerBase focusPlayerRef)
	{
		if (Owner != null)
		{
			return Owner.OwnerZombieRayCastToPlayer(ref pos, ref dir, pathing, qIndex, directionTest, focusPlayerRef);
		}
		return false;
	}

	public virtual bool OwnerZombieRayCastToPlayer(ref Vector3 pos, ref Vector3 dir, dtStatNavMesh.dtStatNavMeshHeader pathing, int qIndex, bool directionTest, PlayerBase focusPlayerRef)
	{
		return false;
	}

	public static void ZombieUpdatePathing(ushort uid, ushort pathingIndex)
	{
		if (Owner != null)
		{
			Owner.OwnerZombieUpdatePathing(uid, pathingIndex);
		}
	}

	public virtual void OwnerZombieUpdatePathing(ushort uid, ushort pathingIndex)
	{
	}

	public static dtStatNavMesh.dtStatNavMeshHeader GetPathingReference(ref Vector3 p)
	{
		if (Owner != null)
		{
			return Owner.OwnerGetPathingReference(ref p);
		}
		return null;
	}

	public virtual dtStatNavMesh.dtStatNavMeshHeader OwnerGetPathingReference(ref Vector3 p)
	{
		return null;
	}

	public static void ZombieNewState(byte sender, ushort uid, byte anim, byte state)
	{
		if (Owner != null)
		{
			Owner.OwnerZombieNewState(sender, uid, anim, state);
		}
	}

	public virtual void OwnerZombieNewState(byte sender, ushort uid, byte anim, byte state)
	{
	}

	public static void CreateVehicleByType(NetworkGamer gamer, ItemCls item)
	{
		if (Owner != null)
		{
			Owner.OwnerCreateVehicleByType(gamer, item);
		}
	}

	public virtual void OwnerCreateVehicleByType(NetworkGamer gamer, ItemCls item)
	{
	}

	public static bool CanPlayerAttachToVehicle(NetworkGamer gamer, ushort uid, int vSeat)
	{
		for (int i = 0; i < NumOfVehicles; i++)
		{
			if (AllVehicles[i] != null && AllVehicles[i].VehicleItemRef.uid == uid && AllVehicles[i].IsSeatAvailable(vSeat) && gamer.Tag is PlayerBase playerBase)
			{
				playerBase.IsAttached0 = true;
				playerBase.vehicleSeat = vSeat;
				AllVehicles[i].AttachedPlayer[playerBase.vehicleSeat] = playerBase;
				AllVehicles[i].PlayDoorSound();
				return true;
			}
		}
		return false;
	}

	public static void AttachPlayerToVehicle(byte gamerId, ushort uid, int vSeat)
	{
		for (int i = 0; i < NumOfVehicles; i++)
		{
			if (AllVehicles[i] == null || AllVehicles[i].VehicleItemRef.uid != uid)
			{
				continue;
			}
			NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId);
			if (networkGamer != null)
			{
				PlayerBase playerBase = networkGamer.Tag as PlayerBase;
				if (networkGamer.IsLocal)
				{
					playerBase.IsAttached0 = true;
					AllVehicles[i].AttachedPlayer[vSeat] = playerBase;
					AllVehicles[i].PlayerAttachHardSetCamera(playerBase);
				}
				else
				{
					playerBase.IsAttached0 = true;
					playerBase.vehicleSeat = vSeat;
					AllVehicles[i].AttachedPlayer[playerBase.vehicleSeat] = playerBase;
				}
				AllVehicles[i].PlayDoorSound();
				break;
			}
		}
	}

	public static ushort DetachPlayerFromVehicle(byte gamerId, ushort uid, byte headLights)
	{
		NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId);
		return DetachPlayerFromVehicle(networkGamer.Tag as PlayerBase, uid, headLights);
	}

	public static ushort DetachPlayerFromVehicle(NetworkGamer netGamer, ushort uid, byte headLights)
	{
		return DetachPlayerFromVehicle(netGamer.Tag as PlayerBase, uid, headLights);
	}

	public static ushort DetachPlayerFromVehicle(PlayerBase playerRef, ushort uid, byte headLights)
	{
		int num = -1;
		if (playerRef != null)
		{
			for (int i = 0; i < NumOfVehicles; i++)
			{
				if (AllVehicles[i] == null)
				{
					continue;
				}
				if (EGENetWorkNext.networkSession != null)
				{
					if (AllVehicles[i].AttachedPlayer[playerRef.vehicleSeat] == playerRef)
					{
						playerRef.IsAttached0 = false;
						AllVehicles[i].AttachedPlayer[playerRef.vehicleSeat] = null;
						AllVehicles[i].HeadLightOn = headLights == 1;
						num = -1;
						if (playerRef == LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value])
						{
							AllVehicles[i].DetachPlayer(playerRef, LevelBaseMenu.DataQueueUpdate);
						}
						else
						{
							AllVehicles[i].RemoteDetachPlayer(playerRef, LevelBaseMenu.DataQueueUpdate);
						}
						break;
					}
				}
				else if (playerRef.IsAttached0 && AllVehicles[i].AttachedPlayer[playerRef.vehicleSeat] == playerRef)
				{
					playerRef.IsAttached0 = false;
					AllVehicles[i].AttachedPlayer[playerRef.vehicleSeat] = null;
					AllVehicles[i].HeadLightOn = headLights == 1;
					AllVehicles[i].EngineStart = false;
					AllVehicles[i].EngineRunning = false;
					if (!AllVehicles[i].EngineSound.IsDisposed)
					{
						AllVehicles[i].EngineSound.Dispose();
					}
					if (!AllVehicles[i].DoorSound.IsDisposed)
					{
						AllVehicles[i].DoorSound.Dispose();
					}
					num = 0;
				}
				if (num >= 0)
				{
					AllVehicles[i].EngineStart = false;
					AllVehicles[i].EngineRunning = false;
					if (!AllVehicles[i].EngineSound.IsDisposed)
					{
						AllVehicles[i].EngineSound.Dispose();
					}
					if (!AllVehicles[i].DoorSound.IsDisposed)
					{
						AllVehicles[i].DoorSound.Dispose();
					}
					AllVehicles[i].PlayDoorSound();
					break;
				}
			}
		}
		return (ushort)num;
	}

	public static void DetachRemotePlayerFromVehicle(NetworkGamer netGamer, ushort uid, byte headLights, int vSeat)
	{
		if (!(netGamer.Tag is PlayerBase playerBase))
		{
			return;
		}
		for (int i = 0; i < NumOfVehicles; i++)
		{
			if (AllVehicles[i] != null && AllVehicles[i].AttachedPlayer[vSeat] == playerBase)
			{
				playerBase.IsAttached0 = false;
				playerBase.vehicleSeat = vSeat;
				AllVehicles[i].AttachedPlayer[playerBase.vehicleSeat] = null;
				if (headLights != byte.MaxValue)
				{
					AllVehicles[i].HeadLightOn = headLights == 1;
				}
				AllVehicles[i].RemoteDetachPlayer(playerBase, LevelBaseMenu.DataQueueUpdate);
				break;
			}
		}
	}

	public static void VehicleNetworkTranslation(byte gamerId, ushort uid, Vector3 pos, Vector3 dir, float speed, float steer, float reverse, byte headLights)
	{
		for (int i = 0; i < NumOfVehicles; i++)
		{
			if (AllVehicles[i] != null && AllVehicles[i].VehicleItemRef.uid == uid)
			{
				NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId);
				if (networkGamer != null && !networkGamer.IsLocal)
				{
					_ = networkGamer.Tag;
					AllVehicles[i].Position = pos;
					AllVehicles[i].Direction = dir;
					AllVehicles[i].Speed = speed;
					AllVehicles[i].TireSteer = steer;
					AllVehicles[i].Reverse = reverse;
					AllVehicles[i].HeadLightOn = headLights == 1;
					AllVehicles[i].NetworkMessageRecievedTimer = 0f;
					break;
				}
			}
		}
	}

	public static void UpdateVehicleData(PacketReader pReader, int vehicleIndex)
	{
		if (Owner != null)
		{
			Owner.OwnerUpdateVehicleData(pReader, vehicleIndex);
		}
	}

	public virtual void OwnerUpdateVehicleData(PacketReader pReader, int vehicleIndex)
	{
	}

	public static void VehicleSpawn(PacketReader pReader, int vehicleIndex)
	{
		if (AllVehicles[vehicleIndex] == null)
		{
			return;
		}
		byte b = pReader.ReadByte();
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef != null && LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef.Id == b)
		{
			ushort uid = pReader.ReadUInt16();
			if (AllVehicles[vehicleIndex].VehicleItemRef == null)
			{
				AllVehicles[vehicleIndex].VehicleItemRef = new ItemCls();
			}
			AllVehicles[vehicleIndex].VehicleItemRef.uid = uid;
			int rFrontWheelQuality = pReader.ReadByte();
			int lFrontWheelQuality = pReader.ReadByte();
			int rRearWheelQuality = pReader.ReadByte();
			int lRearWheelQuality = pReader.ReadByte();
			float fuelLevel = (int)pReader.ReadByte();
			int vehicleDamage = pReader.ReadByte();
			byte gamerId = pReader.ReadByte();
			byte gamerId2 = pReader.ReadByte();
			byte gamerId3 = pReader.ReadByte();
			byte gamerId4 = pReader.ReadByte();
			NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId);
			AllVehicles[vehicleIndex].AttachedPlayer[0] = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
			networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId2);
			AllVehicles[vehicleIndex].AttachedPlayer[1] = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
			networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId3);
			AllVehicles[vehicleIndex].AttachedPlayer[2] = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
			networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId4);
			AllVehicles[vehicleIndex].AttachedPlayer[3] = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
			Vector3 position = pReader.ReadVector3();
			byte b2 = pReader.ReadByte();
			readNB4_0.PackedValue = pReader.ReadUInt32();
			readPHV2.PackedValue = pReader.ReadUInt32();
			byte b3 = pReader.ReadByte();
			Vector4 vector = readNB4_0.ToVector4();
			readPHV2.ToVector2();
			Vector3 zero = Vector3.Zero;
			zero.X = vector.X;
			zero.Y = vector.Y;
			zero.Z = vector.Z;
			AllVehicles[vehicleIndex].Position = position;
			AllVehicles[vehicleIndex].Direction = zero;
			AllVehicles[vehicleIndex].Reverse = ((float)(int)b2 - 127f) * 0.007874f;
			AllVehicles[vehicleIndex].HeadLightOn = b3 == 1;
			AllVehicles[vehicleIndex].Speed = 1f;
			AllVehicles[vehicleIndex].Set();
			AllVehicles[vehicleIndex].UpdateTransform(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 1f, 0, earlyOut: false);
			AllVehicles[vehicleIndex].UpdateTransform(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 1f, 1, earlyOut: false);
			AllVehicles[vehicleIndex].RFrontWheelQuality = rFrontWheelQuality;
			AllVehicles[vehicleIndex].LFrontWheelQuality = lFrontWheelQuality;
			AllVehicles[vehicleIndex].RRearWheelQuality = rRearWheelQuality;
			AllVehicles[vehicleIndex].LRearWheelQuality = lRearWheelQuality;
			AllVehicles[vehicleIndex].FuelLevel = fuelLevel;
			AllVehicles[vehicleIndex].VehicleDamage = vehicleDamage;
			AllVehicles[vehicleIndex].isSpawned = true;
		}
		else
		{
			pReader.ReadUInt16();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadByte();
			pReader.ReadVector3();
			pReader.ReadByte();
			readNB4_0.PackedValue = pReader.ReadUInt32();
			readPHV2.PackedValue = pReader.ReadUInt32();
			pReader.ReadByte();
			readNB4_0.ToVector4();
			readPHV2.ToVector2();
		}
	}

	public static void VehicleKillZombie(ref Vector3 p, ref Vector3 n)
	{
		float num = (p - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[0]).LengthSquared();
		if (num < 400000000f)
		{
			num = num / 400000000f * 20000f;
		}
		tmpPos = p;
		tmpPos.Y += 60f;
		particles.SpawnBulletHitMutant(ref tmpPos, ref n);
		tmpPos.X += 20f;
		tmpPos.Y += 20f;
		tmpPos.Z += 20f;
		particles.SpawnBulletHitMutant(ref tmpPos, ref n);
		tmpPos.X -= 40f;
		tmpPos.Z -= 40f;
		particles.SpawnBulletHitMutant(ref tmpPos, ref n);
		if (VehImpactSnd != null)
		{
			VehImpactSnd.Dispose();
		}
		int num2 = EndGameEngine.randGenerator.Next(0, 100);
		if (num2 < 25)
		{
			VehImpactSnd = EndGameEngine.SoundBnk.GetCue("Vehicle Impact Body00");
		}
		else if (num2 < 50)
		{
			VehImpactSnd = EndGameEngine.SoundBnk.GetCue("Vehicle Impact Body01");
		}
		else if (num2 < 75)
		{
			VehImpactSnd = EndGameEngine.SoundBnk.GetCue("Vehicle Impact Body02");
		}
		else
		{
			VehImpactSnd = EndGameEngine.SoundBnk.GetCue("Vehicle Impact Body03");
		}
		VehImpactSnd.Play();
		VehImpactSnd.SetVariable("Distance", num);
	}

	public static VehicleCls GetAttachedVehicle(PlayerBase e)
	{
		for (int i = 0; i < AllVehicles.Length; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				if (AllVehicles[i].AttachedPlayer[j] == e && e.vehicleSeat == j)
				{
					return AllVehicles[i];
				}
			}
		}
		return null;
	}

	public static void RemovePlayerFromNetworkEvents(NetworkGamer gamer)
	{
		for (int i = 0; i < ScheduledWorldDownloads.Count; i++)
		{
			if (ScheduledWorldDownloads[i].client == gamer)
			{
				ScheduledWorldDownloads.RemoveAt(i);
				break;
			}
		}
	}

	public static bool SphereCollision(ref BoundingSphere sphere, int qIndex, bool testWalkable)
	{
		if (Owner != null)
		{
			return Owner.OwnerSphereCollision(ref sphere, qIndex, testWalkable);
		}
		return false;
	}

	public virtual bool OwnerSphereCollision(ref BoundingSphere sphere, int qIndex, bool testWalkable)
	{
		return false;
	}

	public static float? WalkableHeight(ref Vector3 pos, int qIndex)
	{
		if (Owner != null)
		{
			return Owner.OwnerWalkableHeight(ref pos, qIndex);
		}
		return null;
	}

	public virtual float? OwnerWalkableHeight(ref Vector3 pos, int qIndex)
	{
		return null;
	}
}
