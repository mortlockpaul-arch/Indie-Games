using System;
using System.Runtime.InteropServices;

namespace Microsoft.XboxLive.MathUtilities;

[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct Vector3(float positionX, float positionY, float positionZ)
{
	[FieldOffset(0)]
	public float X = positionX;

	[FieldOffset(4)]
	public float Y = positionY;

	[FieldOffset(8)]
	public float Z = positionZ;

	public static Vector3 Subtract(Vector3 vector1, Vector3 vector2)
	{
		return new Vector3
		{
			X = vector1.X - vector2.X,
			Y = vector1.Y - vector2.Y,
			Z = vector1.Z - vector2.Z
		};
	}

	public static Vector3 Add(Vector3 value1, Vector3 value2)
	{
		return new Vector3
		{
			X = value1.X + value2.X,
			Y = value1.Y + value2.Y,
			Z = value1.Z + value2.Z
		};
	}

	public static Vector3 Multiply(Vector3 value, float scaleFactor)
	{
		return new Vector3
		{
			X = scaleFactor * value.X,
			Y = scaleFactor * value.Y,
			Z = scaleFactor * value.Z
		};
	}

	public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
	{
		return new Vector3
		{
			X = vector1.Y * vector2.Z - vector1.Z * vector2.Y,
			Y = vector1.Z * vector2.X - vector1.X * vector2.Z,
			Z = vector1.X * vector2.Y - vector1.Y * vector2.X
		};
	}

	public static float Dot(Vector3 vector1, Vector3 vector2)
	{
		return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
	}

	public static Vector3 Normalize(Vector3 value)
	{
		float num = 1f / (float)Math.Sqrt(value.X * value.X + value.Y * value.Y + value.Z * value.Z);
		return new Vector3
		{
			X = value.X * num,
			Y = value.Y * num,
			Z = value.Z * num
		};
	}

	public float Length()
	{
		return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
	}
}
