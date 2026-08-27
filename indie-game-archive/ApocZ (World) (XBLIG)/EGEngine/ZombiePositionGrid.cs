using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ZombiePositionGrid
{
	public class NetworkSyncEntry
	{
		public int timer;

		public ushort uid;

		public Vector3 vForce;

		public int nGamers;

		public NetworkSyncEntry(ushort id, ref Vector3 v)
		{
			timer = 32;
			uid = id;
			vForce = v;
			if (ZombieLODEntry.NetSession.IsHost)
			{
				nGamers = ZombieLODEntry.NetSession.RemoteGamers.Count;
			}
			else
			{
				nGamers = 1;
			}
		}

		public bool TimerExpired()
		{
			timer--;
			if (timer <= 0)
			{
				return true;
			}
			return false;
		}

		public void TimerReset()
		{
			timer = 32;
		}

		public bool ConfirmSync(ushort id)
		{
			if (uid == id)
			{
				nGamers--;
				return true;
			}
			return false;
		}
	}

	public const int xGridSize = 2048;

	public const int zGridSize = 2048;

	public const int xDim = 131072;

	public const int zDim = 131072;

	public const int xMin = -65536;

	public const int zMin = -65536;

	public const int xMax = 65536;

	public const int zMax = 65536;

	public const int nXGrid = 64;

	public const int nZGrid = 64;

	private static ZombieLODEntry[] allzombiesinworld;

	private static List<NetworkSyncEntry> RecvNetworkSyncList = new List<NetworkSyncEntry>();

	private static List<NetworkSyncEntry> SentNetworkSyncList = new List<NetworkSyncEntry>();

	private static int numZombiesInVicinity = 0;

	private static int numZombiesDrawModel = 0;

	public static Cue ZombieHordeSound;

	private static int RouteTest = 0;

	private static Vector3 tmpDir = Vector3.Zero;

	private static Vector3 tmpVecFrom = Vector3.Zero;

	private static Vector3 tmpVecTo = Vector3.Zero;

	private static Vector3[] tmpNavRoute = new Vector3[dtStatNavMesh.MAX_POLYS];

	private static ZombieLODEntry[] tmpRemoveIndices = new ZombieLODEntry[32];

	private static Vector3 vecToPlayer = Vector3.Zero;

	private static float testDiv = 1.2f;

	private static Vector3 vecToLocal = Vector3.Zero;

	public static ZombieLODEntry[] AllZombiesInWorld
	{
		get
		{
			return allzombiesinworld;
		}
		set
		{
		}
	}

	public static void Create()
	{
		ZombieHordeSound = EndGameEngine.SoundBnk.GetCue("ambienthorde00");
		ZombieHordeSound.Play();
		ZombieHordeSound.SetVariable("Distance", 20000f);
	}

	public static void Reset()
	{
		numZombiesInVicinity = 0;
		numZombiesDrawModel = 0;
		RecvNetworkSyncList.Clear();
		SentNetworkSyncList.Clear();
		float num = 131072 / AllZombiesInWorld.Length;
		for (int i = 0; i < AllZombiesInWorld.Length; i++)
		{
			if (AllZombiesInWorld[i] != null)
			{
				AllZombiesInWorld[i].pTarget = null;
				AllZombiesInWorld[i].zFlags &= 15;
				AllZombiesInWorld[i].zFlags |= 16;
				AllZombiesInWorld[i].bState = 14;
				AllZombiesInWorld[i].bAnimation = 98;
				AllZombiesInWorld[i].pos.X = -65536f + num * (float)i;
				AllZombiesInWorld[i].pos.Z = -65536f;
				AllZombiesInWorld[i].pos.Y = 0f;
			}
		}
	}

	public static void SetMaxBots(int n)
	{
		allzombiesinworld = new ZombieLODEntry[n];
	}

	public static void Add(ZombieLODEntry e)
	{
		int num = e._uid - 1;
		if (num >= 0 && num < AllZombiesInWorld.Length)
		{
			e.zFlags &= 31;
			AllZombiesInWorld[num] = e;
		}
	}

	public static void Remove(ZombieLODEntry e)
	{
	}

	public static void Kill(ZombieLODEntry e, ref Vector3 v, byte senderId, DamegePacketType deathType, bool broadcast)
	{
		e.zFlags |= 16;
		ZombieLODEntry.BotBotCollision.Remove(e);
		if (ZombieLODEntry.NetSession != null && broadcast)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			packetWriter.Write((byte)124);
			packetWriter.Write(senderId);
			packetWriter.Write(e._uid);
			packetWriter.Write((byte)deathType);
			packetWriter.Write(v);
			if (ZombieLODEntry.NetSession.IsHost)
			{
				LocalNetworkGamer localNetworkGamer = (LocalNetworkGamer)ZombieLODEntry.NetSession.Host;
				localNetworkGamer.SendData(packetWriter, SendDataOptions.Reliable);
			}
			else
			{
				LocalNetworkGamer localNetworkGamer2 = ZombieLODEntry.NetSession.LocalGamers[0];
				localNetworkGamer2.SendData(packetWriter, SendDataOptions.Reliable, ZombieLODEntry.NetSession.Host);
			}
			SentNetworkSyncList.Add(new NetworkSyncEntry(e._uid, ref v));
		}
	}

	public static void GetSearchExtents(ref Vector3 pos, ref Vector4 extents, int testExtent)
	{
		int num = (int)((pos.X + 65536f) / 2048f) - testExtent;
		int num2 = (int)((pos.Z + 65536f) / 2048f) - testExtent;
		int num3 = num + testExtent * 2;
		int num4 = num2 + testExtent * 2;
		extents.X = ((num > 0) ? num : 0);
		extents.Y = ((num2 > 0) ? num2 : 0);
		extents.Z = ((num3 < 64) ? num3 : 63);
		extents.W = ((num4 < 64) ? num4 : 63);
	}

	public static void GetGridXZ(ref Vector3 p, ref int x, ref int z)
	{
		x = (int)((p.X + 65536f) / 2048f);
		z = (int)((p.Z + 65536f) / 2048f);
	}

	public static void UpdatePosition(ZombieLODEntry e)
	{
		AllZombiesInWorld[e._uid - 1] = e;
	}

	public static void ConfirmSync(ushort uid)
	{
		for (int i = 0; i < SentNetworkSyncList.Count && !SentNetworkSyncList[i].ConfirmSync(uid); i++)
		{
		}
	}

	public static void UpdateHost(int qIndex, NetworkSession netSession)
	{
		numZombiesInVicinity = 0;
		numZombiesDrawModel = 0;
		dtStatNavMesh.MaxRoutesThisUpdate = 8;
		ZombieLODEntry.LocalPlayer = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		ZombieLODEntry.NetSession = netSession;
		if (EGENetWorkNext.HostMigrateTimer > 0f)
		{
			return;
		}
		for (int i = 0; i < AllZombiesInWorld.Length; i++)
		{
			ZombieLODEntry zombieLODEntry = AllZombiesInWorld[i];
			if (zombieLODEntry == null || (zombieLODEntry.zFlags & 0x10) > 0)
			{
				continue;
			}
			zombieLODEntry.Update(EndGameEngine.currentTimeStep, qIndex);
			if (ZombieLODEntry.NetSession != null)
			{
				zombieLODEntry.DisSqrToTarget = float.MaxValue;
				zombieLODEntry.pTarget = null;
				for (int j = 0; j < netSession.AllGamers.Count; j++)
				{
					NetworkGamer networkGamer = netSession.AllGamers[j];
					PlayerBase playerBase = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
					if (playerBase != null && playerBase.BloodLevel > 0f && playerBase.Spawned)
					{
						tmpVecTo = playerBase.vecPosition - zombieLODEntry.pos;
						tmpVecTo.Y = 0f;
						float num = tmpVecTo.LengthSquared();
						if (num < zombieLODEntry.DisSqrToTarget)
						{
							zombieLODEntry.DisSqrToTarget = num;
							zombieLODEntry.pTarget = playerBase;
							vecToPlayer = tmpVecTo;
						}
					}
				}
			}
			else
			{
				tmpVecTo = ZombieLODEntry.LocalPlayer.vecPosition - zombieLODEntry.pos;
				tmpVecTo.Y = 0f;
				zombieLODEntry.DisSqrToTarget = tmpVecTo.LengthSquared();
				zombieLODEntry.pTarget = ZombieLODEntry.LocalPlayer;
			}
			if (zombieLODEntry.DisSqrToTarget > 100000000f)
			{
				zombieLODEntry.zFlags &= 15;
				continue;
			}
			zombieLODEntry.zFlags |= 128;
			vecToLocal = ZombieLODEntry.LocalPlayer.vecPosition - zombieLODEntry.pos;
			float y = vecToLocal.Y;
			vecToLocal.Y = 0f;
			float num2 = vecToLocal.LengthSquared();
			if (y > -100f && y < 100f && num2 <= 6400f)
			{
				float num3 = (1f - num2 / 6400f) * 0.5f;
				ZombieLODEntry.LocalPlayer.vecPosition += vecToLocal * num3;
				zombieLODEntry.pos -= vecToLocal * num3;
			}
			if (num2 < 100000000f)
			{
				if (num2 <= 4000000f)
				{
					zombieLODEntry.zFlags |= 64;
					numZombiesDrawModel++;
				}
				else
				{
					zombieLODEntry.zFlags &= 191;
				}
			}
			if (zombieLODEntry.DisSqrToTarget <= 4000000f)
			{
				zombieLODEntry.zFlags |= 64;
			}
			if (num2 > 16000000f)
			{
				numZombiesInVicinity++;
			}
			zombieLODEntry.bTimer -= 0.033334f;
			if (zombieLODEntry.bState == 14)
			{
				ZombieStateHost.UpdateWander(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 13)
			{
				ZombieStateHost.UpdateIdle(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 15)
			{
				ZombieStateHost.UpdateHunt(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 17)
			{
				ZombieStateHost.UpdateAttack(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 16)
			{
				ZombieStateHost.UpdateSearch(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 18)
			{
				ZombieStateHost.UpdateHit(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bTimer < 0f)
			{
				zombieLODEntry.bTimer = 2f + (float)EndGameEngine.randGenerator.NextDouble() * 2f;
			}
		}
		RouteTest++;
		RouteTest = ((RouteTest < 64) ? RouteTest : 0);
		float num4 = (float)((numZombiesInVicinity < 100) ? numZombiesInVicinity : 100) / 150f;
		float num5 = (float)((numZombiesDrawModel < 6) ? numZombiesDrawModel : 6) / 10f;
		ZombieHordeSound.SetVariable("Distance", (1f - (num4 + num5)) * 20000f);
		UpdateSentNetworkSync();
	}

	public static void UpdateClient(int qIndex, NetworkSession netSession)
	{
		numZombiesInVicinity = 0;
		numZombiesDrawModel = 0;
		dtStatNavMesh.MaxRoutesThisUpdate = 8;
		ZombieLODEntry.LocalPlayer = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		ZombieLODEntry.NetSession = netSession;
		if (EGENetWorkNext.HostMigrateTimer > 0f)
		{
			return;
		}
		for (int i = 0; i < AllZombiesInWorld.Length; i++)
		{
			ZombieLODEntry zombieLODEntry = AllZombiesInWorld[i];
			if (zombieLODEntry == null || (zombieLODEntry.zFlags & 0x10) > 0)
			{
				continue;
			}
			zombieLODEntry.Update(EndGameEngine.currentTimeStep, qIndex);
			zombieLODEntry.DisSqrToTarget = float.MaxValue;
			for (int j = 0; j < netSession.AllGamers.Count; j++)
			{
				NetworkGamer networkGamer = netSession.AllGamers[j];
				PlayerBase playerBase = ((networkGamer != null) ? (networkGamer.Tag as PlayerBase) : null);
				if (playerBase != null && playerBase.Health > 0f && playerBase.Spawned)
				{
					tmpVecTo = playerBase.vecPosition - zombieLODEntry.pos;
					tmpVecTo.Y = 0f;
					float num = tmpVecTo.LengthSquared();
					if (num < zombieLODEntry.DisSqrToTarget)
					{
						zombieLODEntry.DisSqrToTarget = num;
						zombieLODEntry.pTarget = playerBase;
						vecToPlayer = tmpVecTo;
					}
				}
			}
			vecToLocal = ZombieLODEntry.LocalPlayer.vecPosition - zombieLODEntry.pos;
			float y = vecToLocal.Y;
			vecToLocal.Y = 0f;
			float num2 = vecToLocal.LengthSquared();
			if (num2 > 100000000f)
			{
				zombieLODEntry.zFlags &= 15;
				continue;
			}
			zombieLODEntry.zFlags |= 128;
			if (num2 <= 4000000f)
			{
				zombieLODEntry.zFlags |= 64;
				numZombiesDrawModel++;
			}
			else
			{
				zombieLODEntry.zFlags &= 191;
			}
			if (y > -100f && y < 100f && num2 <= 6400f && num2 <= 6400f)
			{
				float num3 = (1f - num2 / 6400f) * 0.5f;
				ZombieLODEntry.LocalPlayer.vecPosition += vecToLocal * num3;
				zombieLODEntry.pos -= vecToLocal * num3;
			}
			zombieLODEntry.bTimer -= 0.033334f;
			if (num2 > 16000000f)
			{
				numZombiesInVicinity++;
			}
			if (zombieLODEntry.bState == 14)
			{
				ZombieStateClient.UpdateWander(zombieLODEntry, qIndex, num2);
			}
			else if (zombieLODEntry.bState == 13)
			{
				ZombieStateClient.UpdateIdle(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 15)
			{
				ZombieStateClient.UpdateHunt(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 17)
			{
				ZombieStateClient.UpdateAttack(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 16)
			{
				ZombieStateClient.UpdateSearch(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bState == 18)
			{
				ZombieStateClient.UpdateHit(zombieLODEntry, qIndex);
			}
			else if (zombieLODEntry.bTimer < 0f)
			{
				zombieLODEntry.bTimer = 2f + (float)EndGameEngine.randGenerator.NextDouble() * 2f;
			}
		}
		RouteTest++;
		RouteTest = ((RouteTest < 64) ? RouteTest : 0);
		float num4 = (float)((numZombiesInVicinity < 100) ? numZombiesInVicinity : 100) / 150f;
		float num5 = (float)((numZombiesDrawModel < 6) ? numZombiesDrawModel : 6) / 10f;
		ZombieHordeSound.SetVariable("Distance", (1f - (num4 + num5)) * 20000f);
		UpdateSentNetworkSync();
	}

	public static void UpdateSentNetworkSync()
	{
		if (EGENetWorkNext.networkSession == null)
		{
			return;
		}
		for (int i = 0; i < SentNetworkSyncList.Count; i++)
		{
			if (!SentNetworkSyncList[i].TimerExpired())
			{
				continue;
			}
			if (SentNetworkSyncList[i].nGamers > 0)
			{
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)124);
				packetWriter.Write(ZombieLODEntry.NetSession.LocalGamers[0].Id);
				packetWriter.Write(SentNetworkSyncList[i].uid);
				packetWriter.Write((byte)1);
				packetWriter.Write(SentNetworkSyncList[i].vForce);
				if (ZombieLODEntry.NetSession.IsHost)
				{
					LocalNetworkGamer localNetworkGamer = (LocalNetworkGamer)ZombieLODEntry.NetSession.Host;
					localNetworkGamer.SendData(packetWriter, SendDataOptions.InOrder);
				}
				else
				{
					LocalNetworkGamer localNetworkGamer2 = ZombieLODEntry.NetSession.LocalGamers[0];
					localNetworkGamer2.SendData(packetWriter, SendDataOptions.InOrder, ZombieLODEntry.NetSession.Host);
				}
				SentNetworkSyncList[i].TimerReset();
			}
			else
			{
				SentNetworkSyncList.RemoveAt(i);
			}
		}
	}
}
