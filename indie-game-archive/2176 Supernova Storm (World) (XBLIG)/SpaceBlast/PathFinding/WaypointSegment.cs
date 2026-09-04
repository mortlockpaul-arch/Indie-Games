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
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		short result = -1;
		float num = float.MaxValue;
		for (int i = 0; i < m_WaypointIndicies.Count; i++)
		{
			short num2 = m_WaypointIndicies[i];
			Waypoint waypoint = m_MainWaypointList[num2];
			Vector2 val = waypoint.Position - point;
			float num3 = ((Vector2)(ref val)).LengthSquared();
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
