using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace SpaceBlast.PathFinding;

internal class WaypointList : List<Waypoint>
{
	private const int constXSegments = 16;

	private const int constYSegments = 16;

	private WaypointSegment[,] m_Segments = new WaypointSegment[16, 16];

	private float m_SegmentWidth;

	private float m_SegmentHeight;

	private Random m_Random = new Random();

	public WaypointList()
	{
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				m_Segments[i, j] = new WaypointSegment(this);
			}
		}
	}

	private void AddWaypoint(Waypoint waypoint)
	{
		Add(waypoint);
		int num = base.Count - 1;
		int num2 = (int)(waypoint.Position.X / m_SegmentWidth);
		int num3 = (int)(waypoint.Position.Y / m_SegmentHeight);
		m_Segments[num2, num3].AddWaypointIndex((short)num);
	}

	public short FindNearestWaypoint(ref Vector2 point)
	{
		int num = (int)((point.X - m_SegmentWidth / 2f) / m_SegmentWidth);
		int num2 = (int)((point.Y - m_SegmentHeight / 2f) / m_SegmentWidth);
		int num3 = Math.Max(0, num);
		int num4 = Math.Min(15, num + 1);
		int num5 = Math.Max(0, num2);
		int num6 = Math.Min(15, num2 + 1);
		short result = 0;
		float num7 = float.MaxValue;
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num5; j <= num6; j++)
			{
				short num8 = m_Segments[i, j].FindNearestWaypoint(ref point, out var distSquared);
				if (num8 >= 0 && distSquared < num7)
				{
					num7 = distSquared;
					result = num8;
				}
			}
		}
		return result;
	}

	public void LoadWaypoints(XmlNodeList xmlSObjects, int worldWidth, int worldHeight)
	{
		m_SegmentWidth = (float)worldWidth / 16f;
		m_SegmentHeight = (float)worldHeight / 16f;
		for (int i = 0; i < xmlSObjects.Count; i++)
		{
			XmlNode node = xmlSObjects.Item(i);
			Waypoint waypoint = new Waypoint(node);
			AddWaypoint(waypoint);
		}
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Waypoint current = enumerator.Current;
			current.CalculateBaseLinkCosts(this);
		}
	}

	public Waypoint SelectRandomWaypoint()
	{
		int index = m_Random.Next(base.Count - 1);
		return base[index];
	}
}
