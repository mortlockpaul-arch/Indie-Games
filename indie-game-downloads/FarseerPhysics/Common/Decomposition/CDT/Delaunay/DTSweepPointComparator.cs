using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay;

public class DTSweepPointComparator : IComparer<PolygonPoint>
{
	public int Compare(PolygonPoint p1, PolygonPoint p2)
	{
		if (p1.Y < p2.Y)
		{
			return -1;
		}
		if (p1.Y > p2.Y)
		{
			return 1;
		}
		if (p1.X < p2.X)
		{
			return -1;
		}
		if (p1.X > p2.X)
		{
			return 1;
		}
		return 0;
	}
}
