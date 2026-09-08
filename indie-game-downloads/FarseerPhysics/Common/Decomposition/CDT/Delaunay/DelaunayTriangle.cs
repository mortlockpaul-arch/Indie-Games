using System;
using FarseerPhysics.Common.Decomposition.CDT.Polygon;
using FarseerPhysics.Common.Decomposition.CDT.Util;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay;

public class DelaunayTriangle
{
	public FixedBitArray3 EdgeIsConstrained;

	public FixedBitArray3 EdgeIsDelaunay;

	public CDTFixedArray3<DelaunayTriangle> Neighbors;

	public CDTFixedArray3<PolygonPoint> Points;

	public bool IsInterior { get; set; }

	public DelaunayTriangle(PolygonPoint p1, PolygonPoint p2, PolygonPoint p3)
	{
		Points[0] = p1;
		Points[1] = p2;
		Points[2] = p3;
	}

	public int IndexOf(PolygonPoint p)
	{
		int num = Points.IndexOf(p);
		if (num == -1)
		{
			throw new Exception("Calling index with a point that doesn't exist in triangle");
		}
		return num;
	}

	private int IndexCCWFrom(PolygonPoint p)
	{
		return (IndexOf(p) + 1) % 3;
	}

	public bool Contains(PolygonPoint p)
	{
		return Points.Contains(p);
	}

	private void MarkNeighbor(PolygonPoint p1, PolygonPoint p2, DelaunayTriangle t)
	{
		int num = EdgeIndex(p1, p2);
		if (num == -1)
		{
			throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");
		}
		Neighbors[num] = t;
	}

	public void MarkNeighbor(DelaunayTriangle t)
	{
		bool flag = t.Contains(Points[0]);
		bool flag2 = t.Contains(Points[1]);
		bool flag3 = t.Contains(Points[2]);
		if (flag2 && flag3)
		{
			Neighbors[0] = t;
			t.MarkNeighbor(Points[1], Points[2], this);
			return;
		}
		if (flag && flag3)
		{
			Neighbors[1] = t;
			t.MarkNeighbor(Points[0], Points[2], this);
			return;
		}
		if (flag && flag2)
		{
			Neighbors[2] = t;
			t.MarkNeighbor(Points[0], Points[1], this);
			return;
		}
		throw new Exception("Failed to mark neighbor, doesn't share an edge!");
	}

	public PolygonPoint OppositePoint(DelaunayTriangle t, PolygonPoint p)
	{
		return PointCWFrom(t.PointCWFrom(p));
	}

	public DelaunayTriangle NeighborCWFrom(PolygonPoint point)
	{
		return Neighbors[(Points.IndexOf(point) + 1) % 3];
	}

	public DelaunayTriangle NeighborCCWFrom(PolygonPoint point)
	{
		return Neighbors[(Points.IndexOf(point) + 2) % 3];
	}

	public DelaunayTriangle NeighborAcrossFrom(PolygonPoint point)
	{
		return Neighbors[Points.IndexOf(point)];
	}

	public PolygonPoint PointCCWFrom(PolygonPoint point)
	{
		return Points[(IndexOf(point) + 1) % 3];
	}

	public PolygonPoint PointCWFrom(PolygonPoint point)
	{
		return Points[(IndexOf(point) + 2) % 3];
	}

	private void RotateCW()
	{
		PolygonPoint value = Points[2];
		Points[2] = Points[1];
		Points[1] = Points[0];
		Points[0] = value;
	}

	public void Legalize(PolygonPoint oPoint, PolygonPoint nPoint)
	{
		RotateCW();
		Points[IndexCCWFrom(oPoint)] = nPoint;
	}

	public override string ToString()
	{
		return string.Concat(Points[0], ",", Points[1], ",", Points[2]);
	}

	public void MarkConstrainedEdge(int index)
	{
		EdgeIsConstrained[index] = true;
	}

	public void MarkConstrainedEdge(PolygonPoint p, PolygonPoint q)
	{
		int num = EdgeIndex(p, q);
		if (num != -1)
		{
			EdgeIsConstrained[num] = true;
		}
	}

	public int EdgeIndex(PolygonPoint p1, PolygonPoint p2)
	{
		int num = Points.IndexOf(p1);
		int num2 = Points.IndexOf(p2);
		bool flag = num == 0 || num2 == 0;
		bool flag2 = num == 1 || num2 == 1;
		bool flag3 = num == 2 || num2 == 2;
		if (flag2 && flag3)
		{
			return 0;
		}
		if (flag && flag3)
		{
			return 1;
		}
		if (flag && flag2)
		{
			return 2;
		}
		return -1;
	}

	public bool GetConstrainedEdgeCCW(PolygonPoint p)
	{
		return EdgeIsConstrained[(IndexOf(p) + 2) % 3];
	}

	public bool GetConstrainedEdgeCW(PolygonPoint p)
	{
		return EdgeIsConstrained[(IndexOf(p) + 1) % 3];
	}

	public void SetConstrainedEdgeCCW(PolygonPoint p, bool ce)
	{
		EdgeIsConstrained[(IndexOf(p) + 2) % 3] = ce;
	}

	public void SetConstrainedEdgeCW(PolygonPoint p, bool ce)
	{
		EdgeIsConstrained[(IndexOf(p) + 1) % 3] = ce;
	}

	public bool GetDelaunayEdgeCCW(PolygonPoint p)
	{
		return EdgeIsDelaunay[(IndexOf(p) + 2) % 3];
	}

	public bool GetDelaunayEdgeCW(PolygonPoint p)
	{
		return EdgeIsDelaunay[(IndexOf(p) + 1) % 3];
	}

	public void SetDelaunayEdgeCCW(PolygonPoint p, bool ce)
	{
		EdgeIsDelaunay[(IndexOf(p) + 2) % 3] = ce;
	}

	public void SetDelaunayEdgeCW(PolygonPoint p, bool ce)
	{
		EdgeIsDelaunay[(IndexOf(p) + 1) % 3] = ce;
	}
}
