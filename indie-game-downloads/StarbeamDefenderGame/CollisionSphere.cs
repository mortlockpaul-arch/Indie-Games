using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class CollisionSphere
{
	private float radius;

	private Vector2 offset;

	public CollisionSphere(float radius, Vector2 positionoffset)
	{
		this.radius = radius;
		offset = positionoffset;
	}

	public static bool Intersects(CollisionSphere col1, Vector2 pos1, CollisionSphere col2, Vector2 pos2)
	{
		if ((pos1 + col1.offset - (pos2 + col2.offset)).Length() < col1.radius + col2.radius)
		{
			return true;
		}
		return false;
	}
}
