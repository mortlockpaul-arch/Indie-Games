using Microsoft.Xna.Framework;

namespace Platformer1;

internal struct Circle(Vector2 position, float radius)
{
	public Vector2 Center = position;

	public float Radius = radius;

	public bool Intersects(Rectangle rectangle)
	{
		Vector2 vector = new Vector2(MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right), MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom));
		float num = (Center - vector).LengthSquared();
		if (num > 0f)
		{
			return num < Radius * Radius;
		}
		return false;
	}
}
