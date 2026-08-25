using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceBlast.PathFinding;

internal class WaypointSegment
{
	private WaypointList m_MainWaypointList;

	private List<short> m_WaypointIndicies = new List<short>();

	public WaypointSegment(WaypointList mainlist)
	{
		m_MainWaypointList = mainlist;
	}

	public void AddWaypointIndex(short waypointIndex)
	{
		m_WaypointIndicies.Add(waypointIndex);
	}

	public short FindNearestWaypoint(ref Vector2 point, out float distSquared)
	{
		short result = -1;
		float num = float.MaxValue;
		for (int i = 0; i < m_WaypointIndicies.Count; i++)
		{
			short num2 = m_WaypointIndicies[i];
			Waypoint waypoint = m_MainWaypointList[num2];
			float num3 = (waypoint.Position - point).LengthSquared();
			if (num3 < num)
			{
				result = num2;
				num = num3;
			}
		}
		distSquared = num;
		return result;
	}
}
