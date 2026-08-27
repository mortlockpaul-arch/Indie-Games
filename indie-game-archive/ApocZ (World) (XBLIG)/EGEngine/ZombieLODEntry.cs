using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class ZombieLODEntry
{
	public const float BotWalkSpeed = 1f;

	public const float BotRunSpeed = 8f;

	public const float MinBotDistanceSqr = 3600f;

	public const float LocalPlayerCollision_Radius = 80f;

	public const float BotAttackPlayerDisSqr = 10000f;

	public const ushort INVIEW_MASK = 128;

	public const ushort MODVIEW_MASK = 64;

	public const ushort MODVIEW_CLEAR = 191;

	public const ushort RENDERMODEL_MASK = 32;

	public const ushort RENDERMODEL_CLEAR = 223;

	public const ushort ISDEAD_MASK = 16;

	public const ushort ISDEAD_CLEAR = 239;

	public const ushort MODELTYPE_MASK = 15;

	public const ushort FLAGS_MASK = 240;

	public static int maxRoute = 12;

	public static int HalfWorldX = 65536;

	public static int HalfWorldZ = 65536;

	public static PlayerBase LocalPlayer = null;

	public static NetworkSession NetSession = null;

	public static List<ZombieLODEntry> BotBotCollision = new List<ZombieLODEntry>(256);

	public static bool HuntPlayerEnabled = true;

	public static bool BotWalkEnabled = true;

	public byte zFlags;

	public float bTimer;

	public byte bState = 14;

	public byte bAnimation = 98;

	public PlayerBase pTarget;

	public Vector3 pTargetLastPosition = Vector3.Zero;

	public float DisSqrToTarget = float.MaxValue;

	public dtStatNavMesh.dtStatNavMeshHeader currentPathingData;

	public Vector3 pos = Vector3.Zero;

	public Vector3 dir = Vector3.UnitZ;

	public float[] FrameIndex = new float[2];

	public ushort AnimTextureHeight;

	public ushort CurrentFrameLoopCount;

	public byte routeCount;

	public byte routeIndex;

	public Vector3[] route = new Vector3[maxRoute];

	public static Cue ZombieAttackSound;

	public static string[] ZombieAttackCueNames = new string[4] { "attackzombie00", "attackzombie01", "attackzombie02", "attackzombie03" };

	public static int ZombieAttackHitIndex = 0;

	public static Cue[] ZombieAttackHitSound;

	public static string[] ZombieAttackHitCueNames = new string[4] { "AttackHit03", "AttackHit02", "AttackHit01", "AttackHit00" };

	public ushort _uid;

	private static ushort UID = 0;

	private static bool IsInit = false;

	private static Vector3 targetVec = Vector3.Zero;

	public ZombieLODEntry()
	{
		UID++;
		_uid = UID;
		bTimer = (float)EndGameEngine.randGenerator.NextDouble() * 8f;
		FrameIndex[0] = 0f;
		FrameIndex[1] = 0f;
		if (!IsInit)
		{
			IsInit = true;
			ZombieAttackSound = EndGameEngine.SoundBnk.GetCue(ZombieAttackCueNames[0]);
			ZombieAttackHitSound = new Cue[3];
			ZombieAttackHitSound[0] = EndGameEngine.SoundBnk.GetCue(ZombieAttackHitCueNames[0]);
			ZombieAttackHitSound[1] = EndGameEngine.SoundBnk.GetCue(ZombieAttackHitCueNames[1]);
			ZombieAttackHitSound[2] = EndGameEngine.SoundBnk.GetCue(ZombieAttackHitCueNames[2]);
		}
	}

	public PlayerBase ClosestLOSOnNetPlayer()
	{
		float num = float.MaxValue;
		PlayerBase result = null;
		for (int i = 0; i < NetSession.AllGamers.Count; i++)
		{
			NetworkGamer networkGamer = NetSession.AllGamers[i];
			PlayerBase playerBase = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
			if (playerBase == null)
			{
				continue;
			}
			for (int j = 0; j < 16; j++)
			{
				if (playerBase.PlayerLineOfSight[j] == _uid)
				{
					float num2 = (playerBase.vecPosition - pos).LengthSquared();
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
		return result;
	}

	public void UpdateLOSToPlayer(int qIndex)
	{
		dtStatNavMesh.dtStatNavMeshHeader pathingReference = AIBase.GetPathingReference(ref pos);
		if (!AIBase.ZombieRayCastToPlayer(ref pos, ref dir, pathingReference, qIndex, directionTest: true, pTarget))
		{
			return;
		}
		byte b = (byte)((LocalPlayer.vecPosition - pos).Length() / 16f);
		if (NetSession.IsHost)
		{
			for (int i = 0; i < 16; i++)
			{
				if (LocalPlayer.PlayerLineOfSight[i] == 0 || LocalPlayer.PlayerLineOfSight[i] == _uid)
				{
					LocalPlayer.PlayerLineOfSight[i] = _uid;
					LocalPlayer.PlayerDistanceQuant[i] = b;
					break;
				}
				if ((zFlags & 0x80) == 0 || (zFlags & 0x40) == 0)
				{
					LocalPlayer.PlayerLineOfSight[i] = _uid;
					LocalPlayer.PlayerDistanceQuant[i] = b;
					break;
				}
				if (LocalPlayer.PlayerDistanceQuant[i] > b)
				{
					LocalPlayer.PlayerLineOfSight[i] = _uid;
					LocalPlayer.PlayerDistanceQuant[i] = b;
					break;
				}
			}
		}
		else
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)125);
			packetWriter.Write(_uid);
			packetWriter.Write(b);
		}
	}

	public bool EnterState(AIBotStates s)
	{
		if (NetSession != null)
		{
			return EnterState(s, NetSession.Host.Id);
		}
		return EnterState(s, 0);
	}

	public bool EnterState(AIBotStates s, byte NetGameSenderId)
	{
		bool flag = false;
		ExitState();
		switch (s)
		{
		case AIBotStates.ZombieHuntPlayer:
			if (ConfirmBotCanAttackTarget() || bState == 17)
			{
				bTimer = 1f;
				routeIndex = routeCount;
				bState = 15;
				bAnimation = 99;
				BotBotCollision.Add(this);
				if (!ZombieAttackSound.IsPlaying)
				{
					ZombieAttackSound.Stop(AudioStopOptions.AsAuthored);
					ZombieAttackSound.Dispose();
					int num = EndGameEngine.randGenerator.Next(0, 4);
					ZombieAttackSound = EndGameEngine.SoundBnk.GetCue(ZombieAttackCueNames[num]);
					ZombieAttackSound.Play();
					float num2 = (pos - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition).LengthSquared();
					num2 = num2 / 400000000f * 40000f;
					ZombieAttackSound.SetVariable("Distance", num2);
				}
				flag = true;
			}
			break;
		case AIBotStates.ZombieWander:
			if (bState != 13)
			{
				routeIndex = routeCount;
			}
			bTimer = 8f;
			bState = 14;
			bAnimation = 98;
			flag = true;
			break;
		case AIBotStates.ZombieIdle:
			bTimer = 4 + EndGameEngine.randGenerator.Next(0, 5);
			bState = 13;
			bAnimation = 97;
			flag = true;
			break;
		case AIBotStates.ZombieSearchPlayer:
			bTimer = 8f;
			bTimer = 8f;
			bState = 16;
			bAnimation = 102;
			flag = true;
			break;
		case AIBotStates.ZombieAttackPlayer:
			bTimer = 0.25f;
			bState = 17;
			bAnimation = (byte)EndGameEngine.randGenerator.Next(100, 102);
			BotBotCollision.Add(this);
			flag = true;
			break;
		case AIBotStates.ZombieHit:
			bTimer = 1.5f;
			bState = 18;
			bAnimation = (byte)EndGameEngine.randGenerator.Next(103, 105);
			flag = true;
			break;
		}
		if (flag && NetSession != null && NetSession.IsHost)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)122);
			packetWriter.Write(_uid);
			packetWriter.Write(bAnimation);
			packetWriter.Write(bState);
			packetWriter.Write(NetGameSenderId);
		}
		return flag;
	}

	public void ExitState()
	{
		if (bState == 15)
		{
			BotBotCollision.Remove(this);
		}
		else if (bState == 14)
		{
			BotBotCollision.Remove(this);
		}
		else if (bState == 16)
		{
			BotBotCollision.Remove(this);
		}
		else if (bState == 17)
		{
			BotBotCollision.Remove(this);
		}
	}

	public void Update(float eTime, int qIndex)
	{
		int num = ((qIndex != 1) ? 1 : 0);
		FrameIndex[qIndex] = FrameIndex[num] + 0.5f;
		CurrentFrameLoopCount = 0;
		if (FrameIndex[qIndex] >= (float)(int)AnimTextureHeight)
		{
			FrameIndex[qIndex] = 0f;
			CurrentFrameLoopCount++;
		}
	}

	public float SetAnimation(int animTexHeight, bool randStart)
	{
		float result = FrameIndex[0];
		AnimTextureHeight = (ushort)animTexHeight;
		if (randStart)
		{
			FrameIndex[0] = EndGameEngine.randGenerator.Next(0, AnimTextureHeight);
		}
		else
		{
			FrameIndex[0] = 0f;
		}
		FrameIndex[1] = FrameIndex[0];
		CurrentFrameLoopCount = 0;
		return result;
	}

	private bool ConfirmBotCanAttackTarget()
	{
		if (NetSession != null && !NetSession.IsHost)
		{
			return true;
		}
		if (DisSqrToTarget > pTarget.CurrentDetectionDistance * pTarget.CurrentDetectionDistance)
		{
			return false;
		}
		if (NetSession != null && NetSession.IsHost)
		{
			for (int i = 0; i < 8; i++)
			{
				if (pTarget.AttackingBot_UID[i] == 0 || pTarget.AttackingBot_UID[i] == _uid)
				{
					pTarget.AttackingBot_UID[i] = _uid;
					return true;
				}
			}
			float num = (pos - pTarget.vecPosition).LengthSquared();
			for (int j = 0; j < 8; j++)
			{
				int num2 = pTarget.AttackingBot_UID[j];
				ZombieLODEntry zombieLODEntry = ZombiePositionGrid.AllZombiesInWorld[num2 - 1];
				if (zombieLODEntry != null)
				{
					if ((zombieLODEntry.zFlags & 0x10) > 0)
					{
						zombieLODEntry.EnterState(AIBotStates.ZombieWander);
						pTarget.AttackingBot_UID[j] = _uid;
						return true;
					}
					targetVec = zombieLODEntry.pos - pTarget.vecPosition;
					float num3 = targetVec.LengthSquared();
					if (num3 > num + 40000f && Vector3.Dot(targetVec, pTarget.vecCharacterDir) < 0f)
					{
						zombieLODEntry.EnterState(AIBotStates.ZombieWander);
						pTarget.AttackingBot_UID[j] = _uid;
						return true;
					}
				}
			}
			return false;
		}
		for (int k = 0; k < 8; k++)
		{
			if (pTarget.AttackingBot_UID[k] == 0 || pTarget.AttackingBot_UID[k] == _uid)
			{
				pTarget.AttackingBot_UID[k] = _uid;
				return true;
			}
		}
		float num4 = (pos - pTarget.vecPosition).LengthSquared();
		for (int l = 0; l < 8; l++)
		{
			int num5 = pTarget.AttackingBot_UID[l];
			ZombieLODEntry zombieLODEntry2 = ZombiePositionGrid.AllZombiesInWorld[num5 - 1];
			if (zombieLODEntry2 != null)
			{
				if ((zombieLODEntry2.zFlags & 0x10) > 0)
				{
					zombieLODEntry2.EnterState(AIBotStates.ZombieWander);
					pTarget.AttackingBot_UID[l] = _uid;
					return true;
				}
				targetVec = zombieLODEntry2.pos - pTarget.vecPosition;
				float num6 = targetVec.LengthSquared();
				if (num6 > num4 + 40000f && Vector3.Dot(targetVec, pTarget.vecCharacterDir) < 0f)
				{
					zombieLODEntry2.EnterState(AIBotStates.ZombieWander);
					pTarget.AttackingBot_UID[l] = _uid;
					return true;
				}
			}
		}
		return false;
	}

	public void BotAttackTarget()
	{
		if (pTarget.IsAttached0)
		{
			return;
		}
		ZombieAttackHitIndex = ((ZombieAttackHitIndex + 1 < 3) ? (ZombieAttackHitIndex + 1) : 0);
		if (!ZombieAttackHitSound[ZombieAttackHitIndex].IsPlaying)
		{
			ZombieAttackHitSound[ZombieAttackHitIndex].Stop(AudioStopOptions.AsAuthored);
			ZombieAttackHitSound[ZombieAttackHitIndex].Dispose();
			int num = EndGameEngine.randGenerator.Next(0, 4);
			ZombieAttackHitSound[ZombieAttackHitIndex] = EndGameEngine.SoundBnk.GetCue(ZombieAttackHitCueNames[num]);
			ZombieAttackHitSound[ZombieAttackHitIndex].Play();
			targetVec = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecPosition - pos;
			float num2 = targetVec.LengthSquared();
			num2 = num2 / 400000000f * 40000f;
			ZombieAttackHitSound[ZombieAttackHitIndex].SetVariable("Distance", num2);
			float num3 = Math.Abs(pos.Y - pTarget.vecPosition.Y);
			if (!(num3 > 150f) && pTarget == LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value])
			{
				LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].ApplyDamageLocal(num * 2);
				AIStateMachine.AddAttackIndicator(ref pos);
			}
		}
	}
}
