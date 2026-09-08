namespace Microsoft.XboxLive.MathUtilities;

public struct Vector2(float positionX, float positionY)
{
	public float X = positionX;

	public float Y = positionY;

	public static Vector2 Add(Vector2 vector1, Vector2 vector2)
	{
		return new Vector2
		{
			X = vector1.X + vector2.X,
			Y = vector1.Y + vector2.Y
		};
	}

	public static Vector2 Subtract(Vector2 vector1, Vector2 vector2)
	{
		return new Vector2
		{
			X = vector1.X - vector2.X,
			Y = vector1.Y - vector2.Y
		};
	}

	public static Vector2 Multiply(Vector2 value, float scaleFactor)
	{
		return new Vector2
		{
			X = scaleFactor * value.X,
			Y = scaleFactor * value.Y
		};
	}

	public static float Dot(Vector2 vector1, Vector2 vector2)
	{
		return vector1.X * vector2.X + vector1.Y * vector2.Y;
	}
}
