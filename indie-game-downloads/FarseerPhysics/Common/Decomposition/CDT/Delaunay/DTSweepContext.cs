using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay;

public class DTSweepContext
{
	private const float ALPHA = 0.3f;

	public readonly List<PolygonPoint> Points = new List<PolygonPoint>(200);

	public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();

	public List<DelaunayTriangle> trianglesCleaned = new List<DelaunayTriangle>();

	public DTSweepBasin Basin = new DTSweepBasin();

	public DTSweepEdgeEvent EdgeEvent = new DTSweepEdgeEvent();

	public AdvancingFront Front;

	private DTSweepPointComparator _comparator = new DTSweepPointComparator();

	private FarseerPhysics.Common.Decomposition.CDT.Polygon.Polygon Polygon { get; set; }

	public int StepCount { get; private set; }

	public PolygonPoint Head { get; set; }

	public PolygonPoint Tail { get; set; }

	public DTSweepContext()
	{
		Clear();
	}

	public void Done()
	{
		StepCount++;
	}

	public void MeshClean(DelaunayTriangle triangle)
	{
		MeshCleanReq(triangle);
	}

	private void MeshCleanReq(DelaunayTriangle triangle)
	{
		if (triangle == null || triangle.IsInterior)
		{
			return;
		}
		triangle.IsInterior = true;
		trianglesCleaned.Add(triangle);
		for (int i = 0; i < 3; i++)
		{
			if (!triangle.EdgeIsConstrained[i])
			{
				MeshCleanReq(triangle.Neighbors[i]);
			}
		}
	}

	private void Clear()
	{
		Points.Clear();
		StepCount = 0;
		Triangles.Clear();
	}

	public AdvancingFrontNode LocateNode(PolygonPoint point)
	{
		return Front.LocateNode(point);
	}

	public void CreateAdvancingFront()
	{
		DelaunayTriangle delaunayTriangle = new DelaunayTriangle(Points[0], Tail, Head);
		Triangles.Add(delaunayTriangle);
		AdvancingFrontNode advancingFrontNode = new AdvancingFrontNode(delaunayTriangle.Points[1]);
		advancingFrontNode.Triangle = delaunayTriangle;
		AdvancingFrontNode advancingFrontNode2 = new AdvancingFrontNode(delaunayTriangle.Points[0]);
		advancingFrontNode2.Triangle = delaunayTriangle;
		AdvancingFrontNode tail = new AdvancingFrontNode(delaunayTriangle.Points[2]);
		Front = new AdvancingFront(advancingFrontNode, tail);
		Front.Head.Next = advancingFrontNode2;
		advancingFrontNode2.Next = Front.Tail;
		advancingFrontNode2.Prev = Front.Head;
		Front.Tail.Prev = advancingFrontNode2;
	}

	public void MapTriangleToNodes(DelaunayTriangle t)
	{
		for (int i = 0; i < 3; i++)
		{
			if (t.Neighbors[i] == null)
			{
				AdvancingFrontNode advancingFrontNode = Front.LocatePoint(t.PointCWFrom(t.Points[i]));
				if (advancingFrontNode != null)
				{
					advancingFrontNode.Triangle = t;
				}
			}
		}
	}

	public void PrepareTriangulation(FarseerPhysics.Common.Decomposition.CDT.Polygon.Polygon t)
	{
		Polygon = t;
		for (int i = 0; i < Polygon.Points.Count - 1; i++)
		{
			DTSweep.CreateSweepConstraint(Polygon.Points[i], Polygon.Points[i + 1]);
		}
		DTSweep.CreateSweepConstraint(Polygon.Points[0], Polygon.Points[Polygon.Points.Count - 1]);
		Points.AddRange(Polygon.Points);
		double x;
		double num = (x = Points[0].X);
		double y;
		double num2 = (y = Points[0].Y);
		foreach (PolygonPoint point in Points)
		{
			if (point.X > num)
			{
				num = point.X;
			}
			if (point.X < x)
			{
				x = point.X;
			}
			if (point.Y > num2)
			{
				num2 = point.Y;
			}
			if (point.Y < y)
			{
				y = point.Y;
			}
		}
		double num3 = 0.30000001192092896 * (num - x);
		double num4 = 0.30000001192092896 * (num2 - y);
		PolygonPoint head = new PolygonPoint(num + num3, y - num4);
		PolygonPoint tail = new PolygonPoint(x - num3, y - num4);
		Head = head;
		Tail = tail;
		Points.Sort(_comparator);
	}
}
