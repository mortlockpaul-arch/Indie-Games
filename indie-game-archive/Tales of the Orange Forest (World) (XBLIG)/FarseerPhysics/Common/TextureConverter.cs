using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public static class TextureConverter
{
	private static readonly int[,] ClosePixels = new int[8, 2]
	{
		{ -1, -1 },
		{ 0, -1 },
		{ 1, -1 },
		{ 1, 0 },
		{ 1, 1 },
		{ 0, 1 },
		{ -1, 1 },
		{ -1, 0 }
	};

	public static Vertices CreateVertices(uint[] data, int width, int height)
	{
		PolygonCreationAssistance pca = new PolygonCreationAssistance(data, width, height);
		List<Vertices> list = CreateVertices(pca);
		return list[0];
	}

	public static Vertices CreateVertices(uint[] data, int width, int height, bool holeDetection)
	{
		PolygonCreationAssistance polygonCreationAssistance = new PolygonCreationAssistance(data, width, height);
		polygonCreationAssistance.HoleDetection = holeDetection;
		List<Vertices> list = CreateVertices(polygonCreationAssistance);
		return list[0];
	}

	public static List<Vertices> CreateVertices(uint[] data, int width, int height, float hullTolerance, byte alphaTolerance, bool multiPartDetection, bool holeDetection)
	{
		PolygonCreationAssistance polygonCreationAssistance = new PolygonCreationAssistance(data, width, height);
		polygonCreationAssistance.HullTolerance = hullTolerance;
		polygonCreationAssistance.AlphaTolerance = alphaTolerance;
		polygonCreationAssistance.MultipartDetection = multiPartDetection;
		polygonCreationAssistance.HoleDetection = holeDetection;
		return CreateVertices(polygonCreationAssistance);
	}

	private static List<Vertices> CreateVertices(PolygonCreationAssistance pca)
	{
		List<Vertices> list = new List<Vertices>();
		Vector2? startVertex = null;
		Vector2? entrance = null;
		List<Vector2> list2 = new List<Vector2>();
		if (pca.IsValid())
		{
			bool flag;
			do
			{
				Vertices vertices;
				if (list.Count == 0)
				{
					vertices = CreateSimplePolygon(pca, Vector2.Zero, Vector2.Zero);
					if (vertices != null && vertices.Count > 2)
					{
						entrance = GetTopMostVertex(vertices);
					}
				}
				else
				{
					if (!entrance.HasValue)
					{
						break;
					}
					vertices = CreateSimplePolygon(pca, entrance.Value, new Vector2(entrance.Value.X - 1f, entrance.Value.Y));
				}
				flag = false;
				if (vertices == null || vertices.Count <= 2)
				{
					continue;
				}
				if (pca.HoleDetection)
				{
					while (true)
					{
						startVertex = GetHoleHullEntrance(pca, vertices, startVertex);
						if (!startVertex.HasValue || list2.Contains(startVertex.Value))
						{
							break;
						}
						list2.Add(startVertex.Value);
						Vertices vertices2 = CreateSimplePolygon(pca, startVertex.Value, new Vector2(startVertex.Value.X + 1f, startVertex.Value.Y));
						if (vertices2 != null && vertices2.Count > 2)
						{
							vertices2.Add(vertices2[0]);
							if (SplitPolygonEdge(vertices, EdgeAlignment.Vertical, startVertex.Value, out var _, out var vertex2Index))
							{
								vertices.InsertRange(vertex2Index, vertices2);
							}
						}
					}
				}
				list.Add(vertices);
				if (!pca.MultipartDetection)
				{
					continue;
				}
				while (GetNextHullEntrance(pca, entrance.Value, out entrance))
				{
					bool flag2 = false;
					for (int i = 0; i < list.Count; i++)
					{
						vertices = list[i];
						if (InPolygon(pca, ref vertices, entrance.Value))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						flag = true;
						break;
					}
				}
			}
			while (flag);
			return list;
		}
		throw new Exception("Sizes don't match: Color array must contain texture width * texture height elements.");
	}

	private static Vector2? GetHoleHullEntrance(PolygonCreationAssistance pca, Vertices polygon, Vector2? startVertex)
	{
		List<CrossingEdgeInfo> list = new List<CrossingEdgeInfo>();
		int num = 0;
		if (polygon != null && polygon.Count > 0)
		{
			int num2 = ((!startVertex.HasValue) ? ((int)GetTopMostCoord(polygon)) : ((int)startVertex.Value.Y));
			int num3 = (int)GetBottomMostCoord(polygon);
			if (num2 > 0 && num2 < pca.Height && num3 > 0 && num3 < pca.Height)
			{
				for (int i = num2; i <= num3; i += pca.HoleDetectionLineStepSize)
				{
					list = GetCrossingEdges(polygon, EdgeAlignment.Vertical, i);
					if (list.Count <= 1 || list.Count % 2 != 0)
					{
						continue;
					}
					for (int j = 0; j < list.Count; j += 2)
					{
						bool flag = false;
						bool flag2 = false;
						for (int k = (int)list[j].CrossingPoint.X; k <= (int)list[j + 1].CrossingPoint.X; k++)
						{
							if (pca.IsSolid(k, i))
							{
								if (!flag2)
								{
									flag = true;
									num = k;
								}
								if (flag && flag2)
								{
									Vector2? result = new Vector2(num, i);
									if (DistanceToHullAcceptable(pca, polygon, result.Value, higherDetail: true))
									{
										return result;
									}
									result = null;
									break;
								}
							}
							else if (flag)
							{
								flag2 = true;
							}
						}
					}
				}
			}
		}
		return null;
	}

	private static bool DistanceToHullAcceptable(PolygonCreationAssistance pca, Vertices polygon, Vector2 point, bool higherDetail)
	{
		if (polygon != null && polygon.Count > 2)
		{
			Vector2 lineEndPoint = polygon[polygon.Count - 1];
			if (higherDetail)
			{
				for (int i = 0; i < polygon.Count; i++)
				{
					Vector2 lineEndPoint2 = polygon[i];
					if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint2, ref lineEndPoint) <= pca.HullTolerance || LineTools.DistanceBetweenPointAndPoint(ref point, ref lineEndPoint2) <= pca.HullTolerance)
					{
						return false;
					}
					lineEndPoint = polygon[i];
				}
				return true;
			}
			for (int j = 0; j < polygon.Count; j++)
			{
				Vector2 lineEndPoint2 = polygon[j];
				if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint2, ref lineEndPoint) <= pca.HullTolerance)
				{
					return false;
				}
				lineEndPoint = polygon[j];
			}
			return true;
		}
		return false;
	}

	private static bool InPolygon(PolygonCreationAssistance pca, ref Vertices polygon, Vector2 point)
	{
		if (DistanceToHullAcceptable(pca, polygon, point, higherDetail: true))
		{
			List<CrossingEdgeInfo> crossingEdges = GetCrossingEdges(polygon, EdgeAlignment.Vertical, (int)point.Y);
			if (crossingEdges.Count > 0 && crossingEdges.Count % 2 == 0)
			{
				for (int i = 0; i < crossingEdges.Count; i += 2)
				{
					if (crossingEdges[i].CrossingPoint.X <= point.X && crossingEdges[i + 1].CrossingPoint.X >= point.X)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		return true;
	}

	private static Vector2? GetTopMostVertex(Vertices vertices)
	{
		float num = float.MaxValue;
		Vector2? result = null;
		for (int i = 0; i < vertices.Count; i++)
		{
			if (num > vertices[i].Y)
			{
				num = vertices[i].Y;
				result = vertices[i];
			}
		}
		return result;
	}

	private static float GetTopMostCoord(Vertices vertices)
	{
		float num = float.MaxValue;
		for (int i = 0; i < vertices.Count; i++)
		{
			if (num > vertices[i].Y)
			{
				num = vertices[i].Y;
			}
		}
		return num;
	}

	private static float GetBottomMostCoord(Vertices vertices)
	{
		float num = float.MinValue;
		for (int i = 0; i < vertices.Count; i++)
		{
			if (num < vertices[i].Y)
			{
				num = vertices[i].Y;
			}
		}
		return num;
	}

	private static List<CrossingEdgeInfo> GetCrossingEdges(Vertices polygon, EdgeAlignment edgeAlign, int checkLine)
	{
		List<CrossingEdgeInfo> list = new List<CrossingEdgeInfo>();
		if (polygon.Count > 1)
		{
			Vector2 vector = polygon[polygon.Count - 1];
			switch (edgeAlign)
			{
			case EdgeAlignment.Vertical:
			{
				for (int i = 0; i < polygon.Count; i++)
				{
					Vector2 vector2 = polygon[i];
					if (((vector2.Y >= (float)checkLine && vector.Y <= (float)checkLine) || (vector2.Y <= (float)checkLine && vector.Y >= (float)checkLine)) && vector2.Y != vector.Y)
					{
						bool flag = true;
						Vector2 vector3 = vector - vector2;
						if (vector2.Y == (float)checkLine)
						{
							Vector2 vector4 = polygon[(i + 1) % polygon.Count];
							Vector2 vector5 = vector2 - vector4;
							flag = ((!(vector3.Y > 0f)) ? (vector5.Y >= 0f) : (vector5.Y <= 0f));
						}
						if (flag)
						{
							list.Add(new CrossingEdgeInfo(crossingPoint: new Vector2(((float)checkLine - vector2.Y) / vector3.Y * vector3.X + vector2.X, checkLine), edgeVertex1: vector2, edgeVertex2: vector, checkLineAlignment: edgeAlign));
						}
					}
					vector = vector2;
				}
				break;
			}
			case EdgeAlignment.Horizontal:
				throw new Exception("EdgeAlignment.Horizontal isn't implemented yet. Sorry.");
			}
		}
		list.Sort();
		return list;
	}

	private static bool SplitPolygonEdge(Vertices polygon, EdgeAlignment edgeAlign, Vector2 coordInsideThePolygon, out int vertex1Index, out int vertex2Index)
	{
		int num = 0;
		int index = 0;
		bool flag = false;
		float num2 = float.MaxValue;
		bool flag2 = false;
		Vector2 point = Vector2.Zero;
		vertex1Index = 0;
		vertex2Index = 0;
		switch (edgeAlign)
		{
		case EdgeAlignment.Vertical:
		{
			List<CrossingEdgeInfo> crossingEdges = GetCrossingEdges(polygon, EdgeAlignment.Vertical, (int)coordInsideThePolygon.Y);
			point.Y = coordInsideThePolygon.Y;
			if (crossingEdges == null || crossingEdges.Count <= 1 || crossingEdges.Count % 2 != 0)
			{
				break;
			}
			for (int i = 0; i < crossingEdges.Count; i++)
			{
				if (crossingEdges[i].CrossingPoint.X < coordInsideThePolygon.X)
				{
					float num3 = coordInsideThePolygon.X - crossingEdges[i].CrossingPoint.X;
					if (num3 < num2)
					{
						num2 = num3;
						point.X = crossingEdges[i].CrossingPoint.X;
						flag2 = true;
					}
				}
			}
			if (!flag2)
			{
				break;
			}
			num2 = float.MaxValue;
			int num4 = polygon.Count - 1;
			for (int j = 0; j < polygon.Count; j++)
			{
				Vector2 lineEndPoint = polygon[j];
				Vector2 lineEndPoint2 = polygon[num4];
				float num3 = LineTools.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint, ref lineEndPoint2);
				if (num3 < num2)
				{
					num2 = num3;
					num = j;
					index = num4;
					flag = true;
				}
				num4 = j;
			}
			if (flag)
			{
				Vector2 vector = polygon[index] - polygon[num];
				vector.Normalize();
				Vector2 point2 = polygon[num];
				float num3 = LineTools.DistanceBetweenPointAndPoint(ref point2, ref point);
				vertex1Index = num;
				vertex2Index = num + 1;
				polygon.Insert(num, num3 * vector + polygon[vertex1Index]);
				polygon.Insert(num, num3 * vector + polygon[vertex2Index]);
				return true;
			}
			break;
		}
		case EdgeAlignment.Horizontal:
			throw new Exception("EdgeAlignment.Horizontal isn't implemented yet. Sorry.");
		}
		return false;
	}

	private static Vertices CreateSimplePolygon(PolygonCreationAssistance pca, Vector2 entrance, Vector2 last)
	{
		bool flag = false;
		bool flag2 = false;
		Vertices vertices = new Vertices();
		Vertices vertices2 = new Vertices();
		Vertices vertices3 = new Vertices();
		Vector2 current = Vector2.Zero;
		if (entrance == Vector2.Zero || !pca.InBounds(entrance))
		{
			flag = GetHullEntrance(pca, out entrance);
			if (flag)
			{
				current = new Vector2(entrance.X - 1f, entrance.Y);
			}
		}
		else if (pca.IsSolid(entrance))
		{
			Vector2 foundPixel;
			if (IsNearPixel(pca, entrance, last))
			{
				current = last;
				flag = true;
			}
			else if (SearchNearPixels(pca, searchingForSolidPixel: false, entrance, out foundPixel))
			{
				current = foundPixel;
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			vertices.Add(entrance);
			vertices2.Add(entrance);
			Vector2 next = entrance;
			while (true)
			{
				if (SearchForOutstandingVertex(vertices2, pca.HullTolerance, out var outstanding))
				{
					if (flag2)
					{
						if (vertices3.Contains(outstanding))
						{
							vertices.Add(outstanding);
						}
						break;
					}
					vertices.Add(outstanding);
					vertices2.RemoveRange(0, vertices2.IndexOf(outstanding));
				}
				last = current;
				current = next;
				if (!GetNextHullPoint(pca, ref last, ref current, out next))
				{
					break;
				}
				vertices2.Add(next);
				if (next == entrance && !flag2)
				{
					flag2 = true;
					vertices3.AddRange(vertices2);
				}
			}
		}
		return vertices;
	}

	private static bool SearchNearPixels(PolygonCreationAssistance pca, bool searchingForSolidPixel, Vector2 current, out Vector2 foundPixel)
	{
		for (int i = 0; i < 8; i++)
		{
			int num = (int)current.X + ClosePixels[i, 0];
			int num2 = (int)current.Y + ClosePixels[i, 1];
			if (!searchingForSolidPixel ^ pca.IsSolid(num, num2))
			{
				foundPixel = new Vector2(num, num2);
				return true;
			}
		}
		foundPixel = Vector2.Zero;
		return false;
	}

	private static bool IsNearPixel(PolygonCreationAssistance pca, Vector2 current, Vector2 near)
	{
		for (int i = 0; i < 8; i++)
		{
			int num = (int)current.X + ClosePixels[i, 0];
			int num2 = (int)current.Y + ClosePixels[i, 1];
			if (num >= 0 && num <= pca.Width && num2 >= 0 && num2 <= pca.Height && num == (int)near.X && num2 == (int)near.Y)
			{
				return true;
			}
		}
		return false;
	}

	private static bool GetHullEntrance(PolygonCreationAssistance pca, out Vector2 entrance)
	{
		for (int i = 0; i <= pca.Height; i++)
		{
			for (int j = 0; j <= pca.Width; j++)
			{
				if (pca.IsSolid(j, i))
				{
					entrance = new Vector2(j, i);
					return true;
				}
			}
		}
		entrance = Vector2.Zero;
		return false;
	}

	private static bool GetNextHullEntrance(PolygonCreationAssistance pca, Vector2 start, out Vector2? entrance)
	{
		int num = pca.Height * pca.Width;
		bool flag = false;
		for (int i = (int)start.X + (int)start.Y * pca.Width; i <= num; i++)
		{
			if (pca.IsSolid(i))
			{
				if (flag)
				{
					int num2 = i % pca.Width;
					entrance = new Vector2(num2, (i - num2) / pca.Width);
					return true;
				}
			}
			else
			{
				flag = true;
			}
		}
		entrance = null;
		return false;
	}

	private static bool GetNextHullPoint(PolygonCreationAssistance pca, ref Vector2 last, ref Vector2 current, out Vector2 next)
	{
		int indexOfFirstPixelToCheck = GetIndexOfFirstPixelToCheck(last, current);
		for (int i = 0; i < 8; i++)
		{
			int num = (indexOfFirstPixelToCheck + i) % 8;
			int num2 = (int)current.X + ClosePixels[num, 0];
			int num3 = (int)current.Y + ClosePixels[num, 1];
			if (num2 >= 0 && num2 < pca.Width && num3 >= 0 && num3 <= pca.Height && pca.IsSolid(num2, num3))
			{
				next = new Vector2(num2, num3);
				return true;
			}
		}
		next = Vector2.Zero;
		return false;
	}

	private static bool SearchForOutstandingVertex(Vertices hullArea, float hullTolerance, out Vector2 outstanding)
	{
		Vector2 vector = Vector2.Zero;
		bool result = false;
		if (hullArea.Count > 2)
		{
			int num = hullArea.Count - 1;
			Vector2 lineEndPoint = hullArea[0];
			Vector2 lineEndPoint2 = hullArea[num];
			for (int i = 1; i < num; i++)
			{
				Vector2 point = hullArea[i];
				if (LineTools.DistanceBetweenPointAndLineSegment(ref point, ref lineEndPoint, ref lineEndPoint2) >= hullTolerance)
				{
					vector = hullArea[i];
					result = true;
					break;
				}
			}
		}
		outstanding = vector;
		return result;
	}

	private static int GetIndexOfFirstPixelToCheck(Vector2 last, Vector2 current)
	{
		switch ((int)(current.X - last.X))
		{
		case 1:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 1;
			case 0:
				return 0;
			case -1:
				return 7;
			}
			break;
		case 0:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 2;
			case -1:
				return 6;
			}
			break;
		case -1:
			switch ((int)(current.Y - last.Y))
			{
			case 1:
				return 3;
			case 0:
				return 4;
			case -1:
				return 5;
			}
			break;
		}
		return 0;
	}
}
