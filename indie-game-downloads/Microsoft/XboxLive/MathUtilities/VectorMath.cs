namespace Microsoft.XboxLive.MathUtilities;

public sealed class VectorMath
{
	private VectorMath()
	{
	}

	public static Vector3 Transform(Vector3 vector, Matrix43 matrix)
	{
		return new Vector3
		{
			X = vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31 + matrix.M41,
			Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32 + matrix.M42,
			Z = vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33 + matrix.M43
		};
	}

	public static float Cross(Vector2 vector1, Vector2 vector2)
	{
		return vector1.X * vector2.Y - vector2.X * vector1.Y;
	}

	public static Vector3 CreateFromPackedNormal(int packedNormal)
	{
		if (packedNormal == 0)
		{
			return new Vector3(0.01f, 0.01f, 0.01f);
		}
		return Vector3.Normalize(new Vector3
		{
			X = (float)((packedNormal & 0x7FF) << 21 >> 21) / 1023f,
			Y = (float)((packedNormal & 0x3FF800) << 10 >> 21) / 1023f,
			Z = (float)(packedNormal >> 22) / 511f
		});
	}

	public static Vector3 CreateFromPackedNormalFlipCoordinates(int packedNormal)
	{
		if (packedNormal == 0)
		{
			return new Vector3(0.01f, 0.01f, -0.01f);
		}
		return Vector3.Normalize(new Vector3
		{
			X = (float)((packedNormal & 0x7FF) << 21 >> 21) / 1023f,
			Y = (float)((packedNormal & 0x3FF800) << 10 >> 21) / 1023f,
			Z = (float)(packedNormal >> 22) / -511f
		});
	}

	public static float FullAngle(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
	{
		float num = Vector3.Dot(vectorA, vectorB);
		float num2 = Vector3.Dot(Vector3.Cross(vectorA, vectorB), normal);
		if (num2 >= 1E-05f)
		{
			num = 2f - num;
		}
		return num;
	}

	public static float DotSat(Vector3 vectorA, Vector3 vectorB)
	{
		float num = vectorA.X * vectorB.X + vectorA.Y * vectorB.Y + vectorA.Z * vectorB.Z;
		if (num < 0f)
		{
			return 0f;
		}
		return num;
	}

	public static int VectorOrientation(Vector3 value)
	{
		int num = 0;
		float num2 = value.X;
		float num3 = value.X * value.X;
		float num4;
		if ((num4 = value.Y * value.Y) > num3)
		{
			num3 = num4;
			num2 = value.Y;
			num = 2;
		}
		if ((num4 = value.Z * value.Z) > num3)
		{
			num3 = num4;
			num2 = value.Z;
			num = 4;
		}
		if (num2 < 0f)
		{
			num |= 1;
		}
		return num;
	}
}
