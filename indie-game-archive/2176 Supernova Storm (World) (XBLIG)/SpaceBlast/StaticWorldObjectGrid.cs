using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace SpaceBlast;

internal class StaticWorldObjectGrid
{
	private List<StaticWorldObject>[,] m_WorldObjects;

	private int m_XSegments;

	private int m_YSegments;

	public void Draw(Vector3 worldTopLeft, Vector3 worldBottomRight)
	{
		int val = Math.Max(0, (int)Math.Floor(worldTopLeft.X / 10000f) - 2);
		int val2 = Math.Max(0, (int)Math.Ceiling(worldBottomRight.X / 10000f) + 2);
		int val3 = (int)Math.Max(0.0, Math.Floor(worldTopLeft.Y / 10000f) - 2.0);
		int val4 = (int)Math.Max(0.0, Math.Ceiling(worldBottomRight.Y / 10000f) + 2.0);
		val = Math.Min(m_XSegments - 1, val);
		val2 = Math.Min(m_XSegments - 1, val2);
		val3 = Math.Min(m_YSegments - 1, val3);
		val4 = Math.Min(m_YSegments - 1, val4);
		for (int i = val; i <= val2; i++)
		{
			for (int j = val3; j <= val4; j++)
			{
				if (m_WorldObjects[i, j] == null)
				{
					continue;
				}
				foreach (StaticWorldObject item in m_WorldObjects[i, j])
				{
					item.Draw();
				}
			}
		}
	}

	public bool CollisionTest(BoundingSphere sphere, ref Vector3 collisionNormal)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)Math.Floor(sphere.Center.X / 10000f);
		int num2 = (int)Math.Floor(sphere.Center.Y / 10000f);
		int num3 = Math.Max(0, num - 1);
		int num4 = Math.Min(m_XSegments - 1, num + 1);
		int num5 = Math.Max(0, num2 - 1);
		int num6 = Math.Min(m_YSegments - 1, num2 + 1);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num5; j <= num6; j++)
			{
				if (m_WorldObjects[i, j] == null)
				{
					continue;
				}
				foreach (StaticWorldObject item in m_WorldObjects[i, j])
				{
					if (item.CollisionTest(sphere, ref collisionNormal))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool CollisionTest(Line line)
	{
		float num = Math.Min(line.Point1.X, line.Point2.X);
		float num2 = Math.Min(line.Point1.Y, line.Point2.Y);
		float num3 = Math.Max(line.Point1.X, line.Point2.X);
		float num4 = Math.Max(line.Point1.Y, line.Point2.Y);
		int num5 = (int)Math.Floor(num / 10000f) - 1;
		if (num5 < 0)
		{
			num5 = 0;
		}
		int num6 = (int)Math.Floor(num2 / 10000f) - 1;
		if (num6 < 0)
		{
			num6 = 0;
		}
		int num7 = (int)Math.Floor(num3 / 10000f) + 1;
		if (num7 > m_XSegments - 1)
		{
			num7 = m_XSegments - 1;
		}
		int num8 = (int)Math.Floor(num4 / 10000f) + 1;
		if (num8 > m_YSegments - 1)
		{
			num8 = m_YSegments - 1;
		}
		for (int i = num5; i <= num7; i++)
		{
			for (int j = num6; j <= num8; j++)
			{
				if (m_WorldObjects[i, j] == null)
				{
					continue;
				}
				foreach (StaticWorldObject item in m_WorldObjects[i, j])
				{
					if (item.CollisionTest(line))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void LoadLevel(XmlNodeList xmlSObjects, int worldWidth, int worldHeight)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		m_XSegments = (int)Math.Ceiling((double)worldWidth / 10000.0);
		m_YSegments = (int)Math.Ceiling((double)worldHeight / 10000.0);
		m_WorldObjects = new List<StaticWorldObject>[m_XSegments, m_YSegments];
		for (int i = 0; i < xmlSObjects.Count; i++)
		{
			XmlNode node = xmlSObjects.Item(i);
			StaticWorldObject staticWorldObject = new StaticWorldObject(node);
			Vector3 position = staticWorldObject.GetPosition();
			int num = (int)Math.Floor(position.X / 10000f);
			int num2 = (int)Math.Floor(position.Y / 10000f);
			if (m_WorldObjects[num, num2] == null)
			{
				m_WorldObjects[num, num2] = new List<StaticWorldObject>();
			}
			m_WorldObjects[num, num2].Add(staticWorldObject);
		}
	}
}
