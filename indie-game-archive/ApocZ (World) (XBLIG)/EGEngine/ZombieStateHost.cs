using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class ZombieStateHost
{
	private static Vector3 tmpDir = Vector3.Zero;

	private static Vector3 tmpNorm = Vector3.UnitY;

	private static Vector3 targetMoveDirection = Vector3.Zero;

	private static Vector3 tmpVecFrom = Vector3.Zero;

	private static Vector3 tmpVecTo = Vector3.Zero;

	private static Vector3[] tmpNavRoute = new Vector3[dtStatNavMesh.MAX_POLYS];

	public static bool UpdateWander(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.DisSqrToTarget > 16000000f)
		{
			return result;
		}
		if (e.DisSqrToTarget <= 4000000f)
		{
			if (e.bTimer > 8f)
			{
				e.bTimer = 8f;
			}
		}
		else if (e.bTimer > 8f)
		{
			e.bTimer = 8f;
		}
		if (e.pTarget != null && ZombieLODEntry.HuntPlayerEnabled && e.EnterState(AIBotStates.ZombieHuntPlayer))
		{
			return result;
		}
		if (e.bTimer < 0f)
		{
			e.bTimer = 8f;
			byte quant = 0;
			uint offset = 0u;
			HeightMapPhysics.GetQuantizedPosition(ref e.pos, ref offset, ref quant);
			if (EGENetWorkNext.networkSession != null)
			{
				byte value = (byte)((e.dir.X + 1f) * 127f);
				byte value2 = (byte)((e.dir.Z + 1f) * 127f);
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)120);
				packetWriter.Write(e._uid);
				packetWriter.Write(offset);
				packetWriter.Write(quant);
				packetWriter.Write(e.bAnimation);
				packetWriter.Write(e.bState);
				packetWriter.Write(value);
				packetWriter.Write(value2);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
			}
		}
		if (e.routeIndex < e.routeCount)
		{
			if (!ZombieLODEntry.BotWalkEnabled)
			{
				return result;
			}
			tmpDir = e.route[e.routeIndex] - e.pos;
			tmpDir.Y = 0f;
			float num = tmpDir.LengthSquared();
			if (num > 16f)
			{
				tmpDir.Normalize();
				float num2 = 1f - num * 0.0004883f;
				num2 = ((num2 < 0.07f) ? 0.07f : num2);
				e.dir = Vector3.Lerp(e.dir * 100f, tmpDir * 100f, num2);
				e.dir.Normalize();
				e.pos.X += e.dir.X * 1f;
				e.pos.Z += e.dir.Z * 1f;
				if ((e.zFlags & 0x40) == 0)
				{
					e.pos.Y = HeightMapPhysics.GetHeight(ref e.pos);
				}
			}
			else
			{
				e.routeIndex++;
				if (e.routeIndex < e.routeCount)
				{
					byte quant2 = 0;
					uint offset2 = 0u;
					HeightMapPhysics.GetQuantizedPosition(ref e.route[e.routeIndex], ref offset2, ref quant2);
					if (EGENetWorkNext.networkSession != null)
					{
						PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
						packetWriter2.Write((byte)121);
						packetWriter2.Write(e._uid);
						packetWriter2.Write(offset2);
						packetWriter2.Write(quant2);
						packetWriter2.Write(e.bAnimation);
						packetWriter2.Write(e.bState);
						EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter2, SendDataOptions.InOrder);
					}
					e.EnterState(AIBotStates.ZombieIdle);
				}
			}
			return result;
		}
		if (e.DisSqrToTarget < 1000000f)
		{
			TryGetRoute(e, randomDestination: true);
		}
		else
		{
			TryGetRoute(e, randomDestination: true);
		}
		if (e.routeIndex >= e.routeCount)
		{
			e.EnterState(AIBotStates.ZombieIdle);
		}
		return result;
	}

	public static bool UpdateHunt(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.pTarget == null)
		{
			e.EnterState(AIBotStates.ZombieWander);
			return result;
		}
		if (e.DisSqrToTarget > e.pTarget.CurrentDetectionDistance * e.pTarget.CurrentDetectionDistance + 250000f)
		{
			e.EnterState(AIBotStates.ZombieWander);
			return false;
		}
		if (e.DisSqrToTarget < 10000f)
		{
			e.EnterState(AIBotStates.ZombieAttackPlayer);
			return result;
		}
		targetMoveDirection = e.pTarget.vecPosition - e.pTargetLastPosition;
		if (targetMoveDirection.LengthSquared() > 10000f || e.routeIndex >= e.routeCount)
		{
			e.pTargetLastPosition = e.pTarget.vecPosition;
			bool flag = e.pTarget != null;
			PlayerBase pTarget = e.pTarget;
			if (pTarget == null)
			{
				pTarget = e.pTarget;
			}
			if (flag)
			{
				e.pTarget = pTarget;
				e.pTargetLastPosition = e.pTarget.vecPosition;
				tmpVecTo = e.pTarget.vecPosition;
				tmpVecTo.Y = 0f;
				tmpVecFrom = e.pos;
				tmpVecFrom.Y = 0f;
				dtStatNavMesh.dtStatNavMeshHeader pathingReference = AIBase.GetPathingReference(ref tmpVecTo);
				pathingReference = ((pathingReference == null) ? e.currentPathingData : pathingReference);
				if (e.pTarget.Spawned)
				{
					if (pathingReference != null)
					{
						int num = LevelBaseMenu.NavigationMesh.PathInBounds(pathingReference, ref tmpVecFrom, ref tmpVecTo);
						if (num == 2 || num == 3)
						{
							e.routeCount = 1;
							e.routeIndex = 0;
							ref Vector3 reference = ref e.route[0];
							reference = tmpVecTo;
						}
						else
						{
							dtStatNavMesh.MaxRoutesThisUpdate = 1;
							e.routeIndex = 0;
							e.routeCount = (byte)LevelBaseMenu.NavigationMesh.GetPath(pathingReference, ref tmpVecFrom, ref tmpVecTo, BaseData.routePolys, tmpNavRoute, randomDestination: true);
							if (e.routeCount > 0)
							{
								e.routeIndex = 0;
								if (e.routeCount > ZombieLODEntry.maxRoute)
								{
									e.routeCount = (byte)ZombieLODEntry.maxRoute;
								}
								for (int i = 0; i < e.routeCount; i++)
								{
									ref Vector3 reference2 = ref e.route[i];
									reference2 = tmpNavRoute[i];
								}
							}
						}
						byte quant = 0;
						uint offset = 0u;
						HeightMapPhysics.GetQuantizedPosition(ref tmpVecTo, ref offset, ref quant);
						if (EGENetWorkNext.networkSession != null)
						{
							PacketWriter packetWriter = EGENetWorkNext.packetWriter;
							packetWriter.Write((byte)121);
							packetWriter.Write(e._uid);
							packetWriter.Write(offset);
							packetWriter.Write(quant);
							packetWriter.Write(e.bAnimation);
							packetWriter.Write(e.bState);
							EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
						}
					}
					else
					{
						e.routeCount = 1;
						e.routeIndex = 0;
						ref Vector3 reference3 = ref e.route[0];
						reference3 = tmpVecTo;
						byte quant2 = 0;
						uint offset2 = 0u;
						HeightMapPhysics.GetQuantizedPosition(ref e.route[e.routeIndex], ref offset2, ref quant2);
						if (EGENetWorkNext.networkSession != null)
						{
							PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
							packetWriter2.Write((byte)121);
							packetWriter2.Write(e._uid);
							packetWriter2.Write(offset2);
							packetWriter2.Write(quant2);
							packetWriter2.Write(e.bAnimation);
							packetWriter2.Write(e.bState);
							EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter2, SendDataOptions.InOrder);
						}
					}
				}
			}
			if (e.routeIndex >= e.routeCount)
			{
				e.EnterState(AIBotStates.ZombieSearchPlayer);
			}
		}
		if (e.bTimer < 0f)
		{
			e.bTimer = 1f;
			byte quant3 = 0;
			uint offset3 = 0u;
			HeightMapPhysics.GetQuantizedPosition(ref e.pos, ref offset3, ref quant3);
			if (EGENetWorkNext.networkSession != null)
			{
				byte value = (byte)((e.dir.X + 1f) * 127f);
				byte value2 = (byte)((e.dir.Z + 1f) * 127f);
				PacketWriter packetWriter3 = EGENetWorkNext.packetWriter;
				packetWriter3.Write((byte)120);
				packetWriter3.Write(e._uid);
				packetWriter3.Write(offset3);
				packetWriter3.Write(quant3);
				packetWriter3.Write(e.bAnimation);
				packetWriter3.Write(e.bState);
				packetWriter3.Write(value);
				packetWriter3.Write(value2);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter3, SendDataOptions.InOrder);
			}
		}
		if (e.routeIndex < e.routeCount)
		{
			tmpDir = e.route[e.routeIndex] - e.pos;
			tmpDir.Y = 0f;
			float num2 = tmpDir.LengthSquared();
			if (num2 > 512f)
			{
				tmpDir.Normalize();
				float num3 = 1f - num2 * 0.0004883f;
				num3 = ((num3 < 0.5f) ? 0.5f : num3);
				e.dir = Vector3.Lerp(e.dir * 100f, tmpDir * 100f, num3);
				e.dir.Normalize();
				e.pos.X += e.dir.X * 8f;
				e.pos.Z += e.dir.Z * 8f;
				if ((e.zFlags & 0x40) == 0)
				{
					e.pos.Y = HeightMapPhysics.GetHeight(ref e.pos);
				}
				for (int j = 0; j < ZombieLODEntry.BotBotCollision.Count; j++)
				{
					if (ZombieLODEntry.BotBotCollision[j] != e)
					{
						tmpDir = e.pos - ZombieLODEntry.BotBotCollision[j].pos;
						float num4 = tmpDir.LengthSquared();
						if (num4 < 3600f)
						{
							float num5 = 1f - num4 / 3600f;
							e.pos += tmpDir * num5;
						}
					}
				}
			}
			else
			{
				e.routeIndex++;
				if (e.routeIndex < e.routeCount)
				{
					byte quant4 = 0;
					uint offset4 = 0u;
					HeightMapPhysics.GetQuantizedPosition(ref e.route[e.routeIndex], ref offset4, ref quant4);
					if (EGENetWorkNext.networkSession != null)
					{
						PacketWriter packetWriter4 = EGENetWorkNext.packetWriter;
						packetWriter4.Write((byte)121);
						packetWriter4.Write(e._uid);
						packetWriter4.Write(offset4);
						packetWriter4.Write(quant4);
						packetWriter4.Write(e.bAnimation);
						packetWriter4.Write(e.bState);
						EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter4, SendDataOptions.InOrder);
					}
				}
			}
		}
		if (e.bTimer < 0f)
		{
			e.bTimer = 1f;
		}
		return result;
	}

	public static bool UpdateSearch(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.bTimer < 0f)
		{
			e.EnterState(AIBotStates.ZombieWander);
			return result;
		}
		if (e.bTimer % 2f <= 0.04f)
		{
			if (e.pTarget != null && ZombieLODEntry.HuntPlayerEnabled && e.EnterState(AIBotStates.ZombieHuntPlayer))
			{
				return result;
			}
			byte quant = 0;
			uint offset = 0u;
			HeightMapPhysics.GetQuantizedPosition(ref e.pos, ref offset, ref quant);
			if (EGENetWorkNext.networkSession != null)
			{
				byte value = (byte)((e.dir.X + 1f) * 127f);
				byte value2 = (byte)((e.dir.Z + 1f) * 127f);
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)120);
				packetWriter.Write(e._uid);
				packetWriter.Write(offset);
				packetWriter.Write(quant);
				packetWriter.Write(e.bAnimation);
				packetWriter.Write(e.bState);
				packetWriter.Write(value);
				packetWriter.Write(value2);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
			}
		}
		if (e.routeIndex < e.routeCount)
		{
			tmpDir = e.route[e.routeIndex] - e.pos;
			tmpDir.Y = 0f;
			float num = tmpDir.LengthSquared();
			if (num > 16f)
			{
				tmpDir.Normalize();
				float num2 = 1f - num * 0.0004883f;
				num2 = ((num2 < 0.5f) ? 0.5f : num2);
				e.dir = Vector3.Lerp(e.dir * 100f, tmpDir * 100f, num2);
				e.dir.Normalize();
				e.pos.X += e.dir.X * 1f;
				e.pos.Z += e.dir.Z * 1f;
				if ((e.zFlags & 0x40) == 0)
				{
					e.pos.Y = HeightMapPhysics.GetHeight(ref e.pos);
				}
			}
			else
			{
				e.routeIndex++;
				if (e.routeIndex < e.routeCount)
				{
					byte quant2 = 0;
					uint offset2 = 0u;
					HeightMapPhysics.GetQuantizedPosition(ref e.route[e.routeIndex], ref offset2, ref quant2);
					if (EGENetWorkNext.networkSession != null)
					{
						PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
						packetWriter2.Write((byte)121);
						packetWriter2.Write(e._uid);
						packetWriter2.Write(offset2);
						packetWriter2.Write(quant2);
						packetWriter2.Write(e.bAnimation);
						packetWriter2.Write(e.bState);
						EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter2, SendDataOptions.InOrder);
					}
				}
			}
			return result;
		}
		TryGetRoute(e, randomDestination: true);
		return result;
	}

	public static bool UpdateAttack(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.pTarget == null)
		{
			e.EnterState(AIBotStates.ZombieWander);
			return result;
		}
		if (e.pTarget != null)
		{
			e.dir = e.pTarget.vecPosition - e.pos;
			e.dir.Y = 0f;
			e.dir.Normalize();
			if (e.DisSqrToTarget > 25600f && e.EnterState(AIBotStates.ZombieHuntPlayer))
			{
				return result;
			}
		}
		for (int i = 0; i < ZombieLODEntry.BotBotCollision.Count; i++)
		{
			if (ZombieLODEntry.BotBotCollision[i] != e)
			{
				tmpDir = e.pos - ZombieLODEntry.BotBotCollision[i].pos;
				float num = tmpDir.LengthSquared();
				if (num < 3600f)
				{
					float num2 = 1f - num / 3600f;
					e.pos += tmpDir * num2;
				}
			}
		}
		if (e.bTimer < 0f)
		{
			e.bTimer = 1f;
			byte quant = 0;
			uint offset = 0u;
			HeightMapPhysics.GetQuantizedPosition(ref e.pos, ref offset, ref quant);
			if (EGENetWorkNext.networkSession != null)
			{
				byte value = (byte)((e.dir.X + 1f) * 127f);
				byte value2 = (byte)((e.dir.Z + 1f) * 127f);
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)120);
				packetWriter.Write(e._uid);
				packetWriter.Write(offset);
				packetWriter.Write(quant);
				packetWriter.Write(e.bAnimation);
				packetWriter.Write(e.bState);
				packetWriter.Write(value);
				packetWriter.Write(value2);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
			}
			e.BotAttackTarget();
		}
		return result;
	}

	public static bool UpdateHit(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.bTimer < 0f)
		{
			e.bTimer = 1f;
			byte quant = 0;
			uint offset = 0u;
			HeightMapPhysics.GetQuantizedPosition(ref e.pos, ref offset, ref quant);
			if (EGENetWorkNext.networkSession != null)
			{
				byte value = (byte)((e.dir.X + 1f) * 127f);
				byte value2 = (byte)((e.dir.Z + 1f) * 127f);
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)120);
				packetWriter.Write(e._uid);
				packetWriter.Write(offset);
				packetWriter.Write(quant);
				packetWriter.Write(e.bAnimation);
				packetWriter.Write(e.bState);
				packetWriter.Write(value);
				packetWriter.Write(value2);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
			}
		}
		return result;
	}

	public static bool UpdateIdle(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.bTimer < 0f)
		{
			e.EnterState(AIBotStates.ZombieWander);
			return result;
		}
		if (e.bTimer % 2f <= 0.04f && e.pTarget != null && ZombieLODEntry.HuntPlayerEnabled && e.EnterState(AIBotStates.ZombieHuntPlayer))
		{
			return result;
		}
		return result;
	}

	private static void TryGetRoute(ZombieLODEntry e, bool randomDestination)
	{
		if (e.currentPathingData == null)
		{
			e.currentPathingData = AIBase.GetPathingReference(ref e.pos);
		}
		bool flag = true;
		bool flag2 = true;
		if (e.currentPathingData != null)
		{
			tmpVecFrom = e.pos;
			tmpVecFrom.Y = 0f;
			if (randomDestination)
			{
				int num = 5;
				int num2 = 0;
				do
				{
					tmpVecTo = tmpVecFrom;
					tmpVecTo.X += (float)(EndGameEngine.randGenerator.NextDouble() - 0.5) * 3000f;
					tmpVecTo.Z += (float)(EndGameEngine.randGenerator.NextDouble() - 0.5) * 3000f;
					tmpVecTo.X = ((tmpVecTo.X > e.currentPathingData.worldMin.X + 200f) ? tmpVecTo.X : (e.currentPathingData.worldMin.X + 200f));
					tmpVecTo.Z = ((tmpVecTo.Z > e.currentPathingData.worldMin.Z + 200f) ? tmpVecTo.Z : (e.currentPathingData.worldMin.Z + 200f));
					tmpVecTo.X = ((tmpVecTo.X < e.currentPathingData.worldMax.X - 200f) ? tmpVecTo.X : (e.currentPathingData.worldMax.X - 200f));
					tmpVecTo.Z = ((tmpVecTo.Z < e.currentPathingData.worldMax.Z - 200f) ? tmpVecTo.Z : (e.currentPathingData.worldMax.Z - 200f));
					tmpVecTo.X -= e.currentPathingData.worldOffset.X;
					tmpVecTo.Z -= e.currentPathingData.worldOffset.Z;
				}
				while (LevelBaseMenu.NavigationMesh.GetValidPathPosition(ref tmpVecTo, ref LevelBaseMenu.NavigationMesh.PickExtents) == 0 && --num > 0);
				tmpVecTo.X += e.currentPathingData.worldOffset.X;
				tmpVecTo.Z += e.currentPathingData.worldOffset.Z;
			}
			dtStatNavMesh.MaxRoutesThisUpdate = 1;
			e.routeCount = (byte)LevelBaseMenu.NavigationMesh.GetPath(e.currentPathingData, ref tmpVecFrom, ref tmpVecTo, BaseData.routePolys, tmpNavRoute, randomDestination: true);
			if (e.routeCount > 0)
			{
				e.routeIndex = 0;
				if (e.routeCount > ZombieLODEntry.maxRoute)
				{
					e.routeCount = (byte)ZombieLODEntry.maxRoute;
				}
				for (int i = 0; i < e.routeCount; i++)
				{
					ref Vector3 reference = ref e.route[i];
					reference = tmpNavRoute[i];
				}
			}
			else
			{
				flag2 = true;
			}
		}
		if (flag2 && e.currentPathingData != null)
		{
			int num3 = LevelBaseMenu.NavigationMesh.PathInBounds(e.currentPathingData, ref tmpVecFrom, ref tmpVecTo);
			if (num3 > 0)
			{
				e.routeCount = 1;
				e.routeIndex = 0;
				ref Vector3 reference2 = ref e.route[0];
				reference2 = tmpVecTo;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			flag = false;
		}
		if (flag)
		{
			byte quant = 0;
			uint offset = 0u;
			HeightMapPhysics.GetQuantizedPosition(ref e.route[e.routeIndex], ref offset, ref quant);
			if (EGENetWorkNext.networkSession != null)
			{
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)121);
				packetWriter.Write(e._uid);
				packetWriter.Write(offset);
				packetWriter.Write(quant);
				packetWriter.Write(e.bAnimation);
				packetWriter.Write(e.bState);
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
			}
		}
	}
}
