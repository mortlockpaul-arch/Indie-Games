using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceBlast.PathFinding;

internal class PlannedPath
{
	private class AStarListItem
	{
		public short ParentWaypointIndex;

		public float CostFromParent;

		public float CostToDest;

		public float TotalCost => CostFromParent + CostToDest;

		public AStarListItem(short parent, float costFromParent, float costToDest)
		{
			ParentWaypointIndex = parent;
			CostFromParent = costFromParent;
			CostToDest = costToDest;
		}
	}

	public List<Waypoint> Route = new List<Waypoint>();

	private Dictionary<short, AStarListItem> m_OpenList = new Dictionary<short, AStarListItem>();

	private Dictionary<short, AStarListItem> m_ClosedList = new Dictionary<short, AStarListItem>();

	public void CreatePath(ref Vector2 from, ref Vector2 to, int width)
	{
		CreateAStarPath(ref from, ref to);
		OptimisePath(ref from, width);
	}

	private void CreateAStarPath(ref Vector2 from, ref Vector2 to)
	{
		short num = MainGame.LevelData.Waypoints.FindNearestWaypoint(ref from);
		short num2 = MainGame.LevelData.Waypoints.FindNearestWaypoint(ref to);
		Vector2 destPosition = MainGame.LevelData.Waypoints[num2].Position;
		m_OpenList.Add(num, new AStarListItem(-1, 0f, CalcCostToDest(num, ref destPosition)));
		while (true)
		{
			short num3 = PickLowestCostOpenNode();
			AStarListItem aStarListItem = m_OpenList[num3];
			m_OpenList.Remove(num3);
			m_ClosedList.Add(num3, aStarListItem);
			if (num3 == num2)
			{
				break;
			}
			Waypoint waypoint = MainGame.LevelData.Waypoints[num3];
			WaypointLink[] links = waypoint.Links;
			foreach (WaypointLink waypointLink in links)
			{
				short destWaypoint = waypointLink.DestWaypoint;
				if (m_ClosedList.ContainsKey(destWaypoint))
				{
					continue;
				}
				float costFromParent = aStarListItem.CostFromParent + waypointLink.BaseCost.ToSingle();
				float costToDest = CalcCostToDest(destWaypoint, ref destPosition);
				if (m_OpenList.ContainsKey(destWaypoint))
				{
					AStarListItem aStarListItem2 = new AStarListItem(num3, costFromParent, costToDest);
					if (aStarListItem2.CostFromParent < m_OpenList[destWaypoint].CostFromParent)
					{
						m_OpenList[destWaypoint] = aStarListItem2;
					}
				}
				else
				{
					m_OpenList.Add(destWaypoint, new AStarListItem(num3, costFromParent, costToDest));
				}
			}
		}
		List<short> list = new List<short>();
		list.Add(num2);
		AStarListItem aStarListItem3 = m_ClosedList[num2];
		short parentWaypointIndex;
		while ((parentWaypointIndex = aStarListItem3.ParentWaypointIndex) >= 0)
		{
			list.Add(parentWaypointIndex);
			aStarListItem3 = m_ClosedList[parentWaypointIndex];
		}
		for (int num4 = list.Count - 1; num4 >= 0; num4--)
		{
			Route.Add(MainGame.LevelData.Waypoints[list[num4]]);
		}
		m_OpenList.Clear();
		m_ClosedList.Clear();
		MainGame.DebugPlannedPath = this;
	}

	private void OptimisePath(ref Vector2 shipPos, int width)
	{
	}

	private float CalcCostToDest(short wpIdx, ref Vector2 destPosition)
	{
		return (MainGame.LevelData.Waypoints[wpIdx].Position - destPosition).Length();
	}

	private short PickLowestCostOpenNode()
	{
		short result = -1;
		float num = float.MaxValue;
		foreach (KeyValuePair<short, AStarListItem> open in m_OpenList)
		{
			if (open.Value.TotalCost < num)
			{
				num = open.Value.TotalCost;
				result = open.Key;
			}
		}
		return result;
	}
}
