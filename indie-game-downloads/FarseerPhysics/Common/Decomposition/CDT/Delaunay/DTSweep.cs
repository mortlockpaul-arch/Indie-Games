using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay;

public static class DTSweep
{
	private const double PI_div2 = Math.PI / 2.0;

	private const double PI_3div4 = Math.PI * 3.0 / 4.0;

	public static void Triangulate(DTSweepContext tcx)
	{
		tcx.CreateAdvancingFront();
		Sweep(tcx);
		FinalizationPolygon(tcx);
		tcx.Done();
	}

	private static void Sweep(DTSweepContext tcx)
	{
		List<PolygonPoint> points = tcx.Points;
		for (int i = 1; i < points.Count; i++)
		{
			PolygonPoint polygonPoint = points[i];
			AdvancingFrontNode node = PointEvent(tcx, polygonPoint);
			if (!polygonPoint.HasEdges)
			{
				continue;
			}
			foreach (DTSweepConstraint edge in polygonPoint.Edges)
			{
				EdgeEvent(tcx, edge, node);
			}
		}
	}

	private static void FinalizationPolygon(DTSweepContext tcx)
	{
		DelaunayTriangle delaunayTriangle = tcx.Front.Head.Next.Triangle;
		PolygonPoint point = tcx.Front.Head.Next.Point;
		while (!delaunayTriangle.GetConstrainedEdgeCW(point))
		{
			delaunayTriangle = delaunayTriangle.NeighborCCWFrom(point);
		}
		tcx.MeshClean(delaunayTriangle);
	}

	private static AdvancingFrontNode PointEvent(DTSweepContext tcx, PolygonPoint point)
	{
		AdvancingFrontNode advancingFrontNode = tcx.LocateNode(point);
		AdvancingFrontNode advancingFrontNode2 = NewFrontTriangle(tcx, point, advancingFrontNode);
		if (point.X <= advancingFrontNode.Point.X + 1E-12)
		{
			Fill(tcx, advancingFrontNode);
		}
		FillAdvancingFront(tcx, advancingFrontNode2);
		return advancingFrontNode2;
	}

	private static AdvancingFrontNode NewFrontTriangle(DTSweepContext tcx, PolygonPoint point, AdvancingFrontNode node)
	{
		DelaunayTriangle delaunayTriangle = new DelaunayTriangle(point, node.Point, node.Next.Point);
		delaunayTriangle.MarkNeighbor(node.Triangle);
		tcx.Triangles.Add(delaunayTriangle);
		AdvancingFrontNode advancingFrontNode = new AdvancingFrontNode(point);
		advancingFrontNode.Next = node.Next;
		advancingFrontNode.Prev = node;
		node.Next.Prev = advancingFrontNode;
		node.Next = advancingFrontNode;
		if (!Legalize(tcx, delaunayTriangle))
		{
			tcx.MapTriangleToNodes(delaunayTriangle);
		}
		return advancingFrontNode;
	}

	private static void EdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		tcx.EdgeEvent.ConstrainedEdge = edge;
		tcx.EdgeEvent.Right = edge.P.X > edge.Q.X;
		if (!IsEdgeSideOfTriangle(node.Triangle, edge.P, edge.Q))
		{
			FillEdgeEvent(tcx, edge, node);
			EdgeEvent(tcx, edge.P, edge.Q, node.Triangle, edge.Q);
		}
	}

	private static void FillEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		if (tcx.EdgeEvent.Right)
		{
			FillRightAboveEdgeEvent(tcx, edge, node);
		}
		else
		{
			FillLeftAboveEdgeEvent(tcx, edge, node);
		}
	}

	private static void FillRightConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		Fill(tcx, node.Next);
		if (node.Next.Point != edge.P && TriangulationUtil.Orient2D(edge.Q, node.Next.Point, edge.P) == Orientation.CCW && TriangulationUtil.Orient2D(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
		{
			FillRightConcaveEdgeEvent(tcx, edge, node);
		}
	}

	private static void FillRightConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		if (TriangulationUtil.Orient2D(node.Next.Point, node.Next.Next.Point, node.Next.Next.Next.Point) == Orientation.CCW)
		{
			FillRightConcaveEdgeEvent(tcx, edge, node.Next);
		}
		else if (TriangulationUtil.Orient2D(edge.Q, node.Next.Next.Point, edge.P) == Orientation.CCW)
		{
			FillRightConvexEdgeEvent(tcx, edge, node.Next);
		}
	}

	private static void FillRightBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		if (node.Point.X < edge.P.X)
		{
			if (TriangulationUtil.Orient2D(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
			{
				FillRightConcaveEdgeEvent(tcx, edge, node);
				return;
			}
			FillRightConvexEdgeEvent(tcx, edge, node);
			FillRightBelowEdgeEvent(tcx, edge, node);
		}
	}

	private static void FillRightAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		while (node.Next.Point.X < edge.P.X)
		{
			Orientation orientation = TriangulationUtil.Orient2D(edge.Q, node.Next.Point, edge.P);
			if (orientation == Orientation.CCW)
			{
				FillRightBelowEdgeEvent(tcx, edge, node);
			}
			else
			{
				node = node.Next;
			}
		}
	}

	private static void FillLeftConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		if (TriangulationUtil.Orient2D(node.Prev.Point, node.Prev.Prev.Point, node.Prev.Prev.Prev.Point) == Orientation.CW)
		{
			FillLeftConcaveEdgeEvent(tcx, edge, node.Prev);
		}
		else if (TriangulationUtil.Orient2D(edge.Q, node.Prev.Prev.Point, edge.P) == Orientation.CW)
		{
			FillLeftConvexEdgeEvent(tcx, edge, node.Prev);
		}
	}

	private static void FillLeftConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		Fill(tcx, node.Prev);
		if (node.Prev.Point != edge.P && TriangulationUtil.Orient2D(edge.Q, node.Prev.Point, edge.P) == Orientation.CW && TriangulationUtil.Orient2D(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
		{
			FillLeftConcaveEdgeEvent(tcx, edge, node);
		}
	}

	private static void FillLeftBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		if (node.Point.X > edge.P.X)
		{
			if (TriangulationUtil.Orient2D(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
			{
				FillLeftConcaveEdgeEvent(tcx, edge, node);
				return;
			}
			FillLeftConvexEdgeEvent(tcx, edge, node);
			FillLeftBelowEdgeEvent(tcx, edge, node);
		}
	}

	private static void FillLeftAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
	{
		while (node.Prev.Point.X > edge.P.X)
		{
			if (TriangulationUtil.Orient2D(edge.Q, node.Prev.Point, edge.P) == Orientation.CW)
			{
				FillLeftBelowEdgeEvent(tcx, edge, node);
			}
			else
			{
				node = node.Prev;
			}
		}
	}

	private static bool IsEdgeSideOfTriangle(DelaunayTriangle triangle, PolygonPoint ep, PolygonPoint eq)
	{
		int num = triangle.EdgeIndex(ep, eq);
		if (num == -1)
		{
			return false;
		}
		triangle.MarkConstrainedEdge(num);
		triangle = triangle.Neighbors[num];
		triangle?.MarkConstrainedEdge(ep, eq);
		return true;
	}

	private static void EdgeEvent(DTSweepContext tcx, PolygonPoint ep, PolygonPoint eq, DelaunayTriangle triangle, PolygonPoint point)
	{
		if (!IsEdgeSideOfTriangle(triangle, ep, eq))
		{
			PolygonPoint polygonPoint = triangle.PointCCWFrom(point);
			Orientation orientation = TriangulationUtil.Orient2D(eq, polygonPoint, ep);
			if (orientation == Orientation.Collinear)
			{
				throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", eq, polygonPoint, ep);
			}
			PolygonPoint polygonPoint2 = triangle.PointCWFrom(point);
			Orientation orientation2 = TriangulationUtil.Orient2D(eq, polygonPoint2, ep);
			if (orientation2 == Orientation.Collinear)
			{
				throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", eq, polygonPoint2, ep);
			}
			if (orientation == orientation2)
			{
				triangle = ((orientation != Orientation.CW) ? triangle.NeighborCWFrom(point) : triangle.NeighborCCWFrom(point));
				EdgeEvent(tcx, ep, eq, triangle, point);
			}
			else
			{
				FlipEdgeEvent(tcx, ep, eq, triangle, point);
			}
		}
	}

	private static void SplitEdge(PolygonPoint ep, PolygonPoint eq, PolygonPoint p)
	{
		DTSweepConstraint dTSweepConstraint = eq.Edges.First((DTSweepConstraint e) => e.Q == ep || e.P == ep);
		dTSweepConstraint.P = p;
		CreateSweepConstraint(ep, p);
	}

	public static void CreateSweepConstraint(PolygonPoint p1, PolygonPoint p2)
	{
		PolygonPoint p3 = p1;
		PolygonPoint polygonPoint = p2;
		if (p1.Y > p2.Y)
		{
			polygonPoint = p1;
			p3 = p2;
		}
		else if (p1.Y == p2.Y)
		{
			if (p1.X > p2.X)
			{
				polygonPoint = p1;
				p3 = p2;
			}
			else
			{
				_ = p1.X;
				_ = p2.X;
			}
		}
		DTSweepConstraint dTSweepConstraint = new DTSweepConstraint();
		dTSweepConstraint.P = p3;
		dTSweepConstraint.Q = polygonPoint;
		polygonPoint.AddEdge(dTSweepConstraint);
	}

	private static void FlipEdgeEvent(DTSweepContext tcx, PolygonPoint ep, PolygonPoint eq, DelaunayTriangle t, PolygonPoint p)
	{
		DelaunayTriangle delaunayTriangle = t.NeighborAcrossFrom(p);
		PolygonPoint polygonPoint = delaunayTriangle.OppositePoint(t, p);
		if (delaunayTriangle == null)
		{
			throw new InvalidOperationException("[BUG:FIXME] FLIP failed due to missing triangle");
		}
		if (TriangulationUtil.InScanArea(p, t.PointCCWFrom(p), t.PointCWFrom(p), polygonPoint))
		{
			RotateTrianglePair(t, p, delaunayTriangle, polygonPoint);
			tcx.MapTriangleToNodes(t);
			tcx.MapTriangleToNodes(delaunayTriangle);
			if (p == eq && polygonPoint == ep)
			{
				if (eq == tcx.EdgeEvent.ConstrainedEdge.Q && ep == tcx.EdgeEvent.ConstrainedEdge.P)
				{
					t.MarkConstrainedEdge(ep, eq);
					delaunayTriangle.MarkConstrainedEdge(ep, eq);
					Legalize(tcx, t);
					Legalize(tcx, delaunayTriangle);
				}
			}
			else
			{
				Orientation o = TriangulationUtil.Orient2D(eq, polygonPoint, ep);
				t = NextFlipTriangle(tcx, o, t, delaunayTriangle, p, polygonPoint);
				FlipEdgeEvent(tcx, ep, eq, t, p);
			}
		}
		else
		{
			PolygonPoint p2 = NextFlipPoint(ep, eq, delaunayTriangle, polygonPoint);
			FlipScanEdgeEvent(tcx, ep, eq, t, delaunayTriangle, p2);
			EdgeEvent(tcx, ep, eq, t, p);
		}
	}

	private static PolygonPoint NextFlipPoint(PolygonPoint ep, PolygonPoint eq, DelaunayTriangle ot, PolygonPoint op)
	{
		return TriangulationUtil.Orient2D(eq, op, ep) switch
		{
			Orientation.CW => ot.PointCCWFrom(op), 
			Orientation.CCW => ot.PointCWFrom(op), 
			Orientation.Collinear => throw new PointOnEdgeException("Point on constrained edge not supported yet", eq, op, ep), 
			_ => throw new NotImplementedException("Orientation not handled"), 
		};
	}

	private static DelaunayTriangle NextFlipTriangle(DTSweepContext tcx, Orientation o, DelaunayTriangle t, DelaunayTriangle ot, PolygonPoint p, PolygonPoint op)
	{
		int index;
		if (o == Orientation.CCW)
		{
			index = ot.EdgeIndex(p, op);
			ot.EdgeIsDelaunay[index] = true;
			Legalize(tcx, ot);
			ot.EdgeIsDelaunay.Clear();
			return t;
		}
		index = t.EdgeIndex(p, op);
		t.EdgeIsDelaunay[index] = true;
		Legalize(tcx, t);
		t.EdgeIsDelaunay.Clear();
		return ot;
	}

	private static void FlipScanEdgeEvent(DTSweepContext tcx, PolygonPoint ep, PolygonPoint eq, DelaunayTriangle flipTriangle, DelaunayTriangle t, PolygonPoint p)
	{
		DelaunayTriangle delaunayTriangle = t.NeighborAcrossFrom(p);
		PolygonPoint polygonPoint = delaunayTriangle.OppositePoint(t, p);
		if (delaunayTriangle == null)
		{
			throw new Exception("[BUG:FIXME] FLIP failed due to missing triangle");
		}
		if (TriangulationUtil.InScanArea(eq, flipTriangle.PointCCWFrom(eq), flipTriangle.PointCWFrom(eq), polygonPoint))
		{
			FlipEdgeEvent(tcx, eq, polygonPoint, delaunayTriangle, polygonPoint);
			return;
		}
		PolygonPoint p2 = NextFlipPoint(ep, eq, delaunayTriangle, polygonPoint);
		FlipScanEdgeEvent(tcx, ep, eq, flipTriangle, delaunayTriangle, p2);
	}

	private static void FillAdvancingFront(DTSweepContext tcx, AdvancingFrontNode n)
	{
		AdvancingFrontNode next = n.Next;
		while (next.HasNext)
		{
			double num = HoleAngle(next);
			if (num > Math.PI / 2.0 || num < -Math.PI / 2.0)
			{
				break;
			}
			Fill(tcx, next);
			next = next.Next;
		}
		next = n.Prev;
		while (next.HasPrev)
		{
			double num = HoleAngle(next);
			if (num > Math.PI / 2.0 || num < -Math.PI / 2.0)
			{
				break;
			}
			Fill(tcx, next);
			next = next.Prev;
		}
		if (n.HasNext && n.Next.HasNext)
		{
			double num = BasinAngle(n);
			if (num < Math.PI * 3.0 / 4.0)
			{
				FillBasin(tcx, n);
			}
		}
	}

	private static void FillBasin(DTSweepContext tcx, AdvancingFrontNode node)
	{
		if (TriangulationUtil.Orient2D(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
		{
			tcx.Basin.LeftNode = node;
		}
		else
		{
			tcx.Basin.LeftNode = node.Next;
		}
		tcx.Basin.BottomNode = tcx.Basin.LeftNode;
		while (tcx.Basin.BottomNode.HasNext && tcx.Basin.BottomNode.Point.Y >= tcx.Basin.BottomNode.Next.Point.Y)
		{
			tcx.Basin.BottomNode = tcx.Basin.BottomNode.Next;
		}
		if (tcx.Basin.BottomNode != tcx.Basin.LeftNode)
		{
			tcx.Basin.RightNode = tcx.Basin.BottomNode;
			while (tcx.Basin.RightNode.HasNext && tcx.Basin.RightNode.Point.Y < tcx.Basin.RightNode.Next.Point.Y)
			{
				tcx.Basin.RightNode = tcx.Basin.RightNode.Next;
			}
			if (tcx.Basin.RightNode != tcx.Basin.BottomNode)
			{
				tcx.Basin.Width = tcx.Basin.RightNode.Point.X - tcx.Basin.LeftNode.Point.X;
				tcx.Basin.LeftHighest = tcx.Basin.LeftNode.Point.Y > tcx.Basin.RightNode.Point.Y;
				FillBasinReq(tcx, tcx.Basin.BottomNode);
			}
		}
	}

	private static void FillBasinReq(DTSweepContext tcx, AdvancingFrontNode node)
	{
		if (IsShallow(tcx, node))
		{
			return;
		}
		Fill(tcx, node);
		if (node.Prev == tcx.Basin.LeftNode && node.Next == tcx.Basin.RightNode)
		{
			return;
		}
		if (node.Prev == tcx.Basin.LeftNode)
		{
			if (TriangulationUtil.Orient2D(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CW)
			{
				return;
			}
			node = node.Next;
		}
		else if (node.Next != tcx.Basin.RightNode)
		{
			node = ((!(node.Prev.Point.Y < node.Next.Point.Y)) ? node.Next : node.Prev);
		}
		else
		{
			Orientation orientation = TriangulationUtil.Orient2D(node.Point, node.Prev.Point, node.Prev.Prev.Point);
			if (orientation == Orientation.CCW)
			{
				return;
			}
			node = node.Prev;
		}
		FillBasinReq(tcx, node);
	}

	private static bool IsShallow(DTSweepContext tcx, AdvancingFrontNode node)
	{
		double num = ((!tcx.Basin.LeftHighest) ? (tcx.Basin.RightNode.Point.Y - node.Point.Y) : (tcx.Basin.LeftNode.Point.Y - node.Point.Y));
		if (tcx.Basin.Width > num)
		{
			return true;
		}
		return false;
	}

	private static double HoleAngle(AdvancingFrontNode node)
	{
		double x = node.Point.X;
		double y = node.Point.Y;
		double num = node.Next.Point.X - x;
		double num2 = node.Next.Point.Y - y;
		double num3 = node.Prev.Point.X - x;
		double num4 = node.Prev.Point.Y - y;
		return Math.Atan2(num * num4 - num2 * num3, num * num3 + num2 * num4);
	}

	private static double BasinAngle(AdvancingFrontNode node)
	{
		double x = node.Point.X - node.Next.Next.Point.X;
		double y = node.Point.Y - node.Next.Next.Point.Y;
		return Math.Atan2(y, x);
	}

	private static void Fill(DTSweepContext tcx, AdvancingFrontNode node)
	{
		DelaunayTriangle delaunayTriangle = new DelaunayTriangle(node.Prev.Point, node.Point, node.Next.Point);
		delaunayTriangle.MarkNeighbor(node.Prev.Triangle);
		delaunayTriangle.MarkNeighbor(node.Triangle);
		tcx.Triangles.Add(delaunayTriangle);
		node.Prev.Next = node.Next;
		node.Next.Prev = node.Prev;
		if (!Legalize(tcx, delaunayTriangle))
		{
			tcx.MapTriangleToNodes(delaunayTriangle);
		}
	}

	private static bool Legalize(DTSweepContext tcx, DelaunayTriangle t)
	{
		for (int i = 0; i < 3; i++)
		{
			if (t.EdgeIsDelaunay[i])
			{
				continue;
			}
			DelaunayTriangle delaunayTriangle = t.Neighbors[i];
			if (delaunayTriangle == null)
			{
				continue;
			}
			PolygonPoint polygonPoint = t.Points[i];
			PolygonPoint polygonPoint2 = delaunayTriangle.OppositePoint(t, polygonPoint);
			int index = delaunayTriangle.IndexOf(polygonPoint2);
			if (delaunayTriangle.EdgeIsConstrained[index] || delaunayTriangle.EdgeIsDelaunay[index])
			{
				t.EdgeIsConstrained[i] = delaunayTriangle.EdgeIsConstrained[index];
			}
			else if (TriangulationUtil.SmartIncircle(polygonPoint, t.PointCCWFrom(polygonPoint), t.PointCWFrom(polygonPoint), polygonPoint2))
			{
				t.EdgeIsDelaunay[i] = true;
				delaunayTriangle.EdgeIsDelaunay[index] = true;
				RotateTrianglePair(t, polygonPoint, delaunayTriangle, polygonPoint2);
				if (!Legalize(tcx, t))
				{
					tcx.MapTriangleToNodes(t);
				}
				if (!Legalize(tcx, delaunayTriangle))
				{
					tcx.MapTriangleToNodes(delaunayTriangle);
				}
				t.EdgeIsDelaunay[i] = false;
				delaunayTriangle.EdgeIsDelaunay[index] = false;
				return true;
			}
		}
		return false;
	}

	private static void RotateTrianglePair(DelaunayTriangle t, PolygonPoint p, DelaunayTriangle ot, PolygonPoint op)
	{
		DelaunayTriangle delaunayTriangle = t.NeighborCCWFrom(p);
		DelaunayTriangle delaunayTriangle2 = t.NeighborCWFrom(p);
		DelaunayTriangle delaunayTriangle3 = ot.NeighborCCWFrom(op);
		DelaunayTriangle delaunayTriangle4 = ot.NeighborCWFrom(op);
		bool constrainedEdgeCCW = t.GetConstrainedEdgeCCW(p);
		bool constrainedEdgeCW = t.GetConstrainedEdgeCW(p);
		bool constrainedEdgeCCW2 = ot.GetConstrainedEdgeCCW(op);
		bool constrainedEdgeCW2 = ot.GetConstrainedEdgeCW(op);
		bool delaunayEdgeCCW = t.GetDelaunayEdgeCCW(p);
		bool delaunayEdgeCW = t.GetDelaunayEdgeCW(p);
		bool delaunayEdgeCCW2 = ot.GetDelaunayEdgeCCW(op);
		bool delaunayEdgeCW2 = ot.GetDelaunayEdgeCW(op);
		t.Legalize(p, op);
		ot.Legalize(op, p);
		ot.SetDelaunayEdgeCCW(p, delaunayEdgeCCW);
		t.SetDelaunayEdgeCW(p, delaunayEdgeCW);
		t.SetDelaunayEdgeCCW(op, delaunayEdgeCCW2);
		ot.SetDelaunayEdgeCW(op, delaunayEdgeCW2);
		ot.SetConstrainedEdgeCCW(p, constrainedEdgeCCW);
		t.SetConstrainedEdgeCW(p, constrainedEdgeCW);
		t.SetConstrainedEdgeCCW(op, constrainedEdgeCCW2);
		ot.SetConstrainedEdgeCW(op, constrainedEdgeCW2);
		t.Neighbors.Clear();
		ot.Neighbors.Clear();
		if (delaunayTriangle != null)
		{
			ot.MarkNeighbor(delaunayTriangle);
		}
		if (delaunayTriangle2 != null)
		{
			t.MarkNeighbor(delaunayTriangle2);
		}
		if (delaunayTriangle3 != null)
		{
			t.MarkNeighbor(delaunayTriangle3);
		}
		if (delaunayTriangle4 != null)
		{
			ot.MarkNeighbor(delaunayTriangle4);
		}
		t.MarkNeighbor(ot);
	}
}
