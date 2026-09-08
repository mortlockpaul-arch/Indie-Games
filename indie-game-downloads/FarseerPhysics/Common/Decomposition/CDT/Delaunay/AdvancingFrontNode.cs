using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay;

public class AdvancingFrontNode
{
	public AdvancingFrontNode Next;

	public PolygonPoint Point;

	public AdvancingFrontNode Prev;

	public DelaunayTriangle Triangle;

	public double Value;

	public bool HasNext => Next != null;

	public bool HasPrev => Prev != null;

	public AdvancingFrontNode(PolygonPoint point)
	{
		Point = point;
		Value = point.X;
	}
}
