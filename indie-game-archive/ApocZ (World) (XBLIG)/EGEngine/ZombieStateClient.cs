using Microsoft.Xna.Framework;

namespace EGEngine;

public class ZombieStateClient
{
	private static Vector3 tmpDir = Vector3.Zero;

	private static Vector3 tmpNorm = Vector3.UnitY;

	private static Vector3 targetMoveDirection = Vector3.Zero;

	private static Vector3 tmpVecFrom = Vector3.Zero;

	private static Vector3 tmpVecTo = Vector3.Zero;

	private static Vector3[] tmpNavRoute = new Vector3[dtStatNavMesh.MAX_POLYS];

	public static bool UpdateWander(ZombieLODEntry e, int qIndex, float disToLocalSqr)
	{
		bool result = false;
		if (disToLocalSqr > 16000000f)
		{
			return result;
		}
		if (disToLocalSqr <= 4000000f)
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
		if (e.bTimer < 0f)
		{
			e.bTimer = 8f;
			e.pTarget = ZombieLODEntry.LocalPlayer;
			e.UpdateLOSToPlayer(qIndex);
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
			}
			return result;
		}
		return result;
	}

	public static bool UpdateHunt(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.bTimer < 0f)
		{
			e.bTimer = 1f;
			e.UpdateLOSToPlayer(qIndex);
		}
		if (e.routeIndex < e.routeCount)
		{
			tmpDir = e.route[e.routeIndex] - e.pos;
			tmpDir.Y = 0f;
			float num = tmpDir.LengthSquared();
			if (num > 512f)
			{
				tmpDir.Normalize();
				float num2 = 1f - num * 0.0004883f;
				num2 = ((num2 < 0.5f) ? 0.5f : num2);
				e.dir = Vector3.Lerp(e.dir * 100f, tmpDir * 100f, num2);
				e.dir.Normalize();
				e.pos.X += e.dir.X * 8f;
				e.pos.Z += e.dir.Z * 8f;
				if ((e.zFlags & 0x40) == 0)
				{
					e.pos.Y = HeightMapPhysics.GetHeight(ref e.pos);
				}
				for (int i = 0; i < ZombieLODEntry.BotBotCollision.Count; i++)
				{
					if (ZombieLODEntry.BotBotCollision[i] != e)
					{
						tmpDir = e.pos - ZombieLODEntry.BotBotCollision[i].pos;
						float num3 = tmpDir.LengthSquared();
						if (num3 < 3600f)
						{
							float num4 = 1f - num3 / 3600f;
							e.pos += tmpDir * num4;
						}
					}
				}
			}
			else
			{
				e.routeIndex++;
			}
		}
		return result;
	}

	public static bool UpdateSearch(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
		if (e.bTimer < 0f)
		{
			e.bTimer = 1f;
			e.UpdateLOSToPlayer(qIndex);
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
			}
			return result;
		}
		return result;
	}

	public static bool UpdateAttack(ZombieLODEntry e, int qIndex)
	{
		bool result = false;
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
		if (e.pTarget != null && e.bTimer < 0f)
		{
			e.bTimer = 1f;
			if (e.DisSqrToTarget < 10000f)
			{
				e.dir = e.pTarget.vecPosition - e.pos;
				e.dir.Y = 0f;
				e.dir.Normalize();
				e.BotAttackTarget();
			}
		}
		return result;
	}

	public static bool UpdateHit(ZombieLODEntry e, int qIndex)
	{
		return false;
	}

	public static bool UpdateIdle(ZombieLODEntry e, int qIndex)
	{
		return false;
	}
}
