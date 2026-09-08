using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition.CDT.Delaunay;

namespace FarseerPhysics.Common.Decomposition.CDT.Polygon;

public class PolygonPoint
{
	public double X;

	public double Y;

	public List<DTSweepConstraint> Edges { get; private set; }

	public bool HasEdges => Edges != null;

	public PolygonPoint(double x, double y)
	{
		X = x;
		Y = y;
	}

	public void AddEdge(DTSweepConstraint e)
	{
		if (Edges == null)
		{
			Edges = new List<DTSweepConstraint>();
		}
		Edges.Add(e);
	}
}
