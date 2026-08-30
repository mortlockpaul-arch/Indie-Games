using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public static class RayHelper
{
	private const float _epsilon = 1E-05f;

	private static GeomPointPair _tempPair;

	public static bool FloatEquals(float value1, float value2, float delta)
	{
		return FloatInRange(value1, value2 - delta, value2 + delta);
	}

	public static bool FloatInRange(float value, float min, float max)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}

	public static bool LineIntersect(ref Vector2 point1, ref Vector2 point2, ref Vector2 point3, ref Vector2 point4, bool firstIsSegment, bool secondIsSegment, float floatTolerance, out Vector2 point)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		point = default(Vector2);
		float num = point4.Y - point3.Y;
		float num2 = point2.X - point1.X;
		float num3 = point4.X - point3.X;
		float num4 = point2.Y - point1.Y;
		float num5 = num * num2 - num3 * num4;
		if (!(num5 >= -1E-05f) || !(num5 <= 1E-05f))
		{
			float num6 = point1.Y - point3.Y;
			float num7 = point1.X - point3.X;
			float num8 = 1f / num5;
			float num9 = num3 * num6 - num * num7;
			num9 *= num8;
			if (!firstIsSegment || (num9 >= 0f && num9 <= 1f))
			{
				float num10 = num2 * num6 - num4 * num7;
				num10 *= num8;
				if ((!secondIsSegment || (num10 >= 0f && num10 <= 1f)) && num9 != 0f && num10 != 0f)
				{
					point.X = point1.X + num9 * num2;
					point.Y = point1.Y + num9 * num4;
					return true;
				}
			}
		}
		return false;
	}

	public static bool LineIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, bool firstIsSegment, bool secondIsSegment, float floatTolerance, out Vector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment, secondIsSegment, floatTolerance, out intersectionPoint);
	}

	public static bool LineIntersect(ref Vector2 point1, ref Vector2 point2, ref Vector2 point3, ref Vector2 point4, out Vector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment: true, secondIsSegment: true, 1E-05f, out intersectionPoint);
	}

	public static bool LineIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, out Vector2 intersectionPoint)
	{
		return LineIntersect(ref point1, ref point2, ref point3, ref point4, firstIsSegment: true, secondIsSegment: true, 1E-05f, out intersectionPoint);
	}

	public static void LineSegmentVerticesIntersect(ref Vector2 point1, ref Vector2 point2, Vertices vertices, ref List<Vector2> intersectionPoints)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < vertices.Count; i++)
		{
			if (LineIntersect(vertices[i], vertices[vertices.NextIndex(i)], point1, point2, firstIsSegment: true, secondIsSegment: true, 1E-05f, out var intersectionPoint))
			{
				intersectionPoints.Add(intersectionPoint);
			}
		}
	}

	public static void LineSegmentAABBIntersect(ref Vector2 point1, ref Vector2 point2, AABB aabb, ref List<Vector2> intersectionPoints)
	{
		LineSegmentVerticesIntersect(ref point1, ref point2, aabb.GetVertices(), ref intersectionPoints);
	}

	public static void LineSegmentGeomIntersect(Vector2 point1, Vector2 point2, Geom geom, bool detectUsingAABB, ref List<Vector2> intersectionPoints)
	{
		LineSegmentGeomIntersect(ref point1, ref point2, geom, detectUsingAABB, ref intersectionPoints);
	}

	public static void LineSegmentGeomIntersect(ref Vector2 point1, ref Vector2 point2, Geom geom, bool detectUsingAABB, ref List<Vector2> intersectionPoints)
	{
		if (detectUsingAABB)
		{
			LineSegmentAABBIntersect(ref point1, ref point2, geom.AABB, ref intersectionPoints);
		}
		else
		{
			LineSegmentVerticesIntersect(ref point1, ref point2, geom.WorldVertices, ref intersectionPoints);
		}
	}

	public static List<GeomPointPair> LineSegmentAllGeomsIntersect(Vector2 point1, Vector2 point2, PhysicsSimulator simulator, bool detectUsingAABB)
	{
		return LineSegmentAllGeomsIntersect(ref point1, ref point2, simulator, detectUsingAABB);
	}

	public static List<GeomPointPair> LineSegmentAllGeomsIntersect(ref Vector2 point1, ref Vector2 point2, PhysicsSimulator simulator, bool detectUsingAABB)
	{
		List<Vector2> intersectionPoints = new List<Vector2>();
		List<GeomPointPair> list = new List<GeomPointPair>();
		foreach (Geom geom in simulator.GeomList)
		{
			intersectionPoints.Clear();
			if (detectUsingAABB)
			{
				LineSegmentAABBIntersect(ref point1, ref point2, geom.AABB, ref intersectionPoints);
			}
			else
			{
				LineSegmentVerticesIntersect(ref point1, ref point2, geom.WorldVertices, ref intersectionPoints);
			}
			if (intersectionPoints.Count > 0)
			{
				_tempPair = default(GeomPointPair);
				_tempPair.Geom = geom;
				_tempPair.Points = new List<Vector2>(intersectionPoints);
				list.Add(_tempPair);
			}
		}
		return list;
	}
}
