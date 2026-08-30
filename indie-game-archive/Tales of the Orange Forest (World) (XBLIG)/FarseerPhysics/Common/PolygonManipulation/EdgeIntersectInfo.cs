using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PolygonManipulation;

public class EdgeIntersectInfo
{
	public Edge EdgeOne { get; private set; }

	public Edge EdgeTwo { get; private set; }

	public Vector2 IntersectionPoint { get; private set; }

	public EdgeIntersectInfo(Edge edgeOne, Edge edgeTwo, Vector2 intersectionPoint)
	{
		EdgeOne = edgeOne;
		EdgeTwo = edgeTwo;
		IntersectionPoint = intersectionPoint;
	}
}
