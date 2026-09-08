using System;
using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay;

public class PointOnEdgeException : NotImplementedException
{
	public readonly PolygonPoint A;

	public readonly PolygonPoint B;

	public readonly PolygonPoint C;

	public PointOnEdgeException(string message, PolygonPoint a, PolygonPoint b, PolygonPoint c)
		: base(message)
	{
		A = a;
		B = b;
		C = c;
	}
}
