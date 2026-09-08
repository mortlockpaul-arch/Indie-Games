namespace Microsoft.XboxLive.MathUtilities;

public struct Vector4(float x, float y, float z, float w)
{
	public float X = x;

	public float Y = y;

	public float Z = z;

	public float W = w;

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		try
		{
			Vector4 vector = (Vector4)obj;
			if (vector.W == W && vector.X == X && vector.Y == Y && vector.Z == Z)
			{
				return true;
			}
		}
		catch
		{
			return false;
		}
		return false;
	}

	public static Vector4 Multiply(Vector4 vector, float scaleFactor)
	{
		return new Vector4
		{
			X = scaleFactor * vector.X,
			Y = scaleFactor * vector.Y,
			Z = scaleFactor * vector.Z,
			W = scaleFactor * vector.W
		};
	}

	public static float DistanceSquared(Vector4 vector1, Vector4 vector2)
	{
		float num = vector1.X - vector2.X;
		float num2 = vector1.Y - vector2.Y;
		float num3 = vector1.Z - vector2.Z;
		float num4 = vector1.W - vector2.W;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4;
	}
}
