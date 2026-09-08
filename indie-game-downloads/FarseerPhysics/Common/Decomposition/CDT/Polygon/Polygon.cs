using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.CDT.Polygon;

public class Polygon
{
	private List<PolygonPoint> _points = new List<PolygonPoint>();

	public IList<PolygonPoint> Points => _points;
}
