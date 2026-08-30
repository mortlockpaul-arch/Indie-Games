using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PolygonManipulation;

public class Edge
{
	public Vector2 EdgeStart { get; private set; }

	public Vector2 EdgeEnd { get; private set; }

	public Edge(Vector2 edgeStart, Vector2 edgeEnd)
	{
		EdgeStart = edgeStart;
		EdgeEnd = edgeEnd;
	}
}
