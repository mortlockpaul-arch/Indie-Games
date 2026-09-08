using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay;
using Poly2Tri.Triangulation.Delaunay.Sweep;
using Poly2Tri.Triangulation.Polygon;

namespace FarseerPhysics.Common.Decomposition;

public static class CDTDecomposer
{
	public static List<Vertices> ConvexPartition(Vertices vertices)
	{
		Polygon polygon = new Polygon();
		foreach (Vector2 vertex in vertices)
		{
			polygon.Points.Add(new TriangulationPoint(vertex.X, vertex.Y));
		}
		DTSweepContext dTSweepContext = new DTSweepContext();
		dTSweepContext.PrepareTriangulation(polygon);
		DTSweep.Triangulate(dTSweepContext);
		List<Vertices> list = new List<Vertices>();
		foreach (DelaunayTriangle triangle in polygon.Triangles)
		{
			Vertices vertices2 = new Vertices();
			foreach (TriangulationPoint point in triangle.Points)
			{
				vertices2.Add(new Vector2((float)point.X, (float)point.Y));
			}
			list.Add(vertices2);
		}
		return list;
	}
}
