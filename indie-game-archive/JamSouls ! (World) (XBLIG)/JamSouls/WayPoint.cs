using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace JamSouls;

public class WayPoint
{
	public const int WAYPOINT_REACH_DISTANCE_X = 40;

	public const int WAYPOINT_REACH_DISTANCE = 40;

	public const int MAX_NEIGHTBOUR = 6;

	public Vector2 m_Position;

	public int m_nId;

	public bool m_GroundWayPoint;

	public List<WayPoint> m_NeightBour = new List<WayPoint>();

	public List<List<BotInputEvent>> m_NeightBourRoadMap = new List<List<BotInputEvent>>();

	public PlayerBot.PathNodes m_Parent;

	public WayPoint(Vector2 Position, int nId, bool bGroundWayPoint)
	{
		m_GroundWayPoint = bGroundWayPoint;
		m_Position = Position;
		m_nId = nId;
	}

	public bool IsWayPointReached(Player p)
	{
		if (Vector2.Distance(m_Position, p.GetPosition()) <= 40f)
		{
			if (m_GroundWayPoint)
			{
				return p.m_bIsOnGround;
			}
			return true;
		}
		return false;
	}

	public float GetRoadTime(WayPoint wp)
	{
		for (int i = 0; i < m_NeightBour.Count; i++)
		{
			if (m_NeightBour[i] != wp)
			{
				continue;
			}
			float num = 0f;
			{
				foreach (BotInputEvent item in m_NeightBourRoadMap[i])
				{
					num += item.duration;
				}
				return num;
			}
		}
		return float.PositiveInfinity;
	}

	public int GetPathIndex(WayPoint wp)
	{
		for (int i = 0; i < m_NeightBour.Count; i++)
		{
			if (m_NeightBour[i] == wp)
			{
				return i;
			}
		}
		return -1;
	}

	public void AddNeightBour(WayPoint wp, List<BotInputEvent> InputEvent)
	{
		if (wp == null || wp == this)
		{
			return;
		}
		List<BotInputEvent> list = new List<BotInputEvent>();
		foreach (BotInputEvent item in InputEvent)
		{
			list.Add(new BotInputEvent(item.bLeft, item.bRight, item.bDown, item.bJump, item.bRun));
			list[list.Count - 1].duration = item.duration;
		}
		int pathIndex = GetPathIndex(wp);
		if (pathIndex != -1)
		{
			m_NeightBourRoadMap[pathIndex] = list;
			return;
		}
		m_NeightBourRoadMap.Add(list);
		m_NeightBour.Add(wp);
	}
}
