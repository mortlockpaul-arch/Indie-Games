using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PolygonManipulation;

public static class BooleanTools
{
	public static Vertices Union(Vertices polygon1, Vertices polygon2, out PolyUnionError error)
	{
		int num = PreparePolygons(polygon1, polygon2, out var poly, out var poly2, out var intersections, out error);
		if (num == -1)
		{
			switch (error)
			{
			case PolyUnionError.NoIntersections:
				return null;
			case PolyUnionError.Poly1InsidePoly2:
				return polygon2;
			}
		}
		Vertices vertices = new Vertices();
		Vertices vertices2 = poly;
		Vertices vertices3 = poly2;
		Vector2 vector = poly[num];
		int index = num;
		do
		{
			vertices.Add(vertices2[index]);
			foreach (EdgeIntersectInfo item in intersections)
			{
				if (!(vertices2[index] == item.IntersectionPoint))
				{
					continue;
				}
				int num2 = vertices3.IndexOf(item.IntersectionPoint);
				if (!PointInPolygonAngle(vertices3[vertices3.NextIndex(num2)], vertices2))
				{
					if (vertices2 == poly)
					{
						vertices2 = poly2;
						vertices3 = poly;
					}
					else
					{
						vertices2 = poly;
						vertices3 = poly2;
					}
					index = num2;
					break;
				}
			}
			index = vertices2.NextIndex(index);
		}
		while (vertices2[index] != vector && vertices.Count <= poly.Count + poly2.Count);
		if (vertices.Count > poly.Count + poly2.Count)
		{
			error = PolyUnionError.InfiniteLoop;
		}
		return vertices;
	}

	public static Vertices Subtract(Vertices polygon1, Vertices polygon2, out PolyUnionError error)
	{
		int num = PreparePolygons(polygon1, polygon2, out var poly, out var poly2, out var intersections, out error);
		if (num == -1)
		{
			switch (error)
			{
			case PolyUnionError.NoIntersections:
				return null;
			case PolyUnionError.Poly1InsidePoly2:
				return null;
			}
		}
		Vertices vertices = new Vertices();
		Vertices vertices2 = poly;
		Vertices vertices3 = poly2;
		Vector2 vector = poly[num];
		int index = num;
		bool flag = true;
		do
		{
			vertices.Add(vertices2[index]);
			foreach (EdgeIntersectInfo item in intersections)
			{
				if (!(vertices2[index] == item.IntersectionPoint))
				{
					continue;
				}
				int num2 = vertices3.IndexOf(item.IntersectionPoint);
				Vector2 point;
				if (flag)
				{
					point = vertices3[vertices3.PreviousIndex(num2)];
					if (PointInPolygonAngle(point, vertices2))
					{
						if (vertices2 == poly)
						{
							vertices2 = poly2;
							vertices3 = poly;
						}
						else
						{
							vertices2 = poly;
							vertices3 = poly2;
						}
						index = num2;
						flag = !flag;
						break;
					}
					continue;
				}
				point = vertices3[vertices3.NextIndex(num2)];
				if (!PointInPolygonAngle(point, vertices2))
				{
					if (vertices2 == poly)
					{
						vertices2 = poly2;
						vertices3 = poly;
					}
					else
					{
						vertices2 = poly;
						vertices3 = poly2;
					}
					index = num2;
					flag = !flag;
					break;
				}
			}
			index = ((!flag) ? vertices2.PreviousIndex(index) : vertices2.NextIndex(index));
		}
		while (vertices2[index] != vector && vertices.Count <= poly.Count + poly2.Count);
		if (vertices.Count > poly.Count + poly2.Count)
		{
			error = PolyUnionError.InfiniteLoop;
		}
		return vertices;
	}

	public static Vertices Intersect(Vertices polygon1, Vertices polygon2, out PolyUnionError error)
	{
		error = PolyUnionError.None;
		int num = PreparePolygons(polygon1, polygon2, out var poly, out var poly2, out var intersections, out var error2);
		if (num == -1)
		{
			switch (error2)
			{
			case PolyUnionError.NoIntersections:
				return null;
			case PolyUnionError.Poly1InsidePoly2:
				return polygon2;
			}
		}
		Vertices vertices = new Vertices();
		Vertices vertices2 = poly;
		Vertices vertices3 = poly2;
		int index = poly.IndexOf(intersections[0].IntersectionPoint);
		Vector2 vector = poly[index];
		do
		{
			vertices.Add(vertices2[index]);
			foreach (EdgeIntersectInfo item in intersections)
			{
				if (!(vertices2[index] == item.IntersectionPoint))
				{
					continue;
				}
				int num2 = vertices3.IndexOf(item.IntersectionPoint);
				if (PointInPolygonAngle(vertices3[vertices3.NextIndex(num2)], vertices2))
				{
					if (vertices2 == poly)
					{
						vertices2 = poly2;
						vertices3 = poly;
					}
					else
					{
						vertices2 = poly;
						vertices3 = poly2;
					}
					index = num2;
					break;
				}
			}
			index = vertices2.NextIndex(index);
		}
		while (vertices2[index] != vector && vertices.Count <= poly.Count + poly2.Count);
		if (vertices.Count > poly.Count + poly2.Count)
		{
			error = PolyUnionError.InfiniteLoop;
		}
		return vertices;
	}

	private static int PreparePolygons(Vertices polygon1, Vertices polygon2, out Vertices poly1, out Vertices poly2, out List<EdgeIntersectInfo> intersections, out PolyUnionError error)
	{
		error = PolyUnionError.None;
		poly1 = Round(polygon1);
		poly2 = Round(polygon2);
		if (!VerticesIntersect(poly1, poly2, out intersections))
		{
			error = PolyUnionError.NoIntersections;
			return -1;
		}
		foreach (EdgeIntersectInfo intersection in intersections)
		{
			if (!poly1.Contains(intersection.IntersectionPoint))
			{
				poly1.Insert(poly1.IndexOf(intersection.EdgeOne.EdgeStart) + 1, intersection.IntersectionPoint);
			}
			if (!poly2.Contains(intersection.IntersectionPoint))
			{
				poly2.Insert(poly2.IndexOf(intersection.EdgeTwo.EdgeStart) + 1, intersection.IntersectionPoint);
			}
		}
		int num = -1;
		int num2 = 0;
		do
		{
			if (!PointInPolygonAngle(poly1[num2], poly2))
			{
				num = num2;
				break;
			}
			num2 = poly1.NextIndex(num2);
		}
		while (num2 != 0);
		if (num == -1)
		{
			error = PolyUnionError.Poly1InsidePoly2;
		}
		return num;
	}

	private static bool VerticesIntersect(Vertices polygon1, Vertices polygon2, out List<EdgeIntersectInfo> intersections)
	{
		intersections = new List<EdgeIntersectInfo>();
		for (int i = 0; i < polygon1.Count; i++)
		{
			Vector2 vector = polygon1[i];
			Vector2 vector2 = polygon1[polygon1.NextIndex(i)];
			for (int j = 0; j < polygon2.Count; j++)
			{
				Vector2 vector3 = polygon2[j];
				Vector2 vector4 = polygon2[polygon2.NextIndex(j)];
				if (LineTools.LineIntersect(vector, vector2, vector3, vector4, firstIsSegment: true, secondIsSegment: true, out var intersectionPoint))
				{
					intersectionPoint = new Vector2((float)Math.Round(intersectionPoint.X, 0), (float)Math.Round(intersectionPoint.Y, 0));
					intersections.Add(new EdgeIntersectInfo(new Edge(vector, vector2), new Edge(vector3, vector4), intersectionPoint));
				}
			}
		}
		return intersections.Count > 0;
	}

	private static bool PointInPolygonAngle(Vector2 point, Vertices polygon)
	{
		double num = 0.0;
		for (int i = 0; i < polygon.Count; i++)
		{
			Vector2 p = polygon[i] - point;
			Vector2 p2 = polygon[polygon.NextIndex(i)] - point;
			num += VectorAngle(p, p2);
		}
		if (Math.Abs(num) < Math.PI)
		{
			return false;
		}
		return true;
	}

	private static double VectorAngle(Vector2 p1, Vector2 p2)
	{
		double num = Math.Atan2(p1.Y, p1.X);
		double num2 = Math.Atan2(p2.Y, p2.X);
		double num3;
		for (num3 = num2 - num; num3 > Math.PI; num3 -= Math.PI * 2.0)
		{
		}
		for (; num3 < -Math.PI; num3 += Math.PI * 2.0)
		{
		}
		return num3;
	}

	public static Vertices Round(Vertices polygon)
	{
		Vertices vertices = new Vertices();
		for (int i = 0; i < polygon.Count; i++)
		{
			vertices.Add(new Vector2((float)Math.Round(polygon[i].X, 0), (float)Math.Round(polygon[i].Y, 0)));
		}
		return vertices;
	}
}
