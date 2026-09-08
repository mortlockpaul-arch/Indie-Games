using System;

namespace Microsoft.XboxLive.MathUtilities;

public struct Quaternion
{
	public float X;

	public float Y;

	public float Z;

	public float W;

	public static Quaternion Add(Quaternion quaternionA, Quaternion quaternionB)
	{
		return new Quaternion
		{
			X = quaternionA.X + quaternionB.X,
			Y = quaternionA.Y + quaternionB.Y,
			Z = quaternionA.Z + quaternionB.Z,
			W = quaternionA.W + quaternionB.W
		};
	}

	public static Quaternion Multiply(Quaternion quaternion, float scaleFactor)
	{
		return new Quaternion
		{
			X = scaleFactor * quaternion.X,
			Y = scaleFactor * quaternion.Y,
			Z = scaleFactor * quaternion.Z,
			W = scaleFactor * quaternion.W
		};
	}

	public static Quaternion CreateFromRotationMatrix(Matrix matrix)
	{
		float num = matrix.M11 + matrix.M22 + matrix.M33 + 1f;
		Quaternion result = default(Quaternion);
		if (num > 0.001f)
		{
			float num2 = 0.5f / (float)Math.Sqrt(num);
			result.W = 0.25f / num2;
			result.X = (matrix.M23 - matrix.M32) * num2;
			result.Y = (matrix.M31 - matrix.M13) * num2;
			result.Z = (matrix.M12 - matrix.M21) * num2;
		}
		else if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M33)
		{
			float num3 = (float)Math.Sqrt(1f + matrix.M11 - matrix.M22 - matrix.M33);
			result.X = 0.5f * num3;
			num3 = 1f / (num3 + num3);
			result.Y = (matrix.M12 + matrix.M21) * num3;
			result.Z = (matrix.M13 + matrix.M31) * num3;
			result.W = (matrix.M23 - matrix.M32) * num3;
		}
		else if (matrix.M22 > matrix.M33)
		{
			float num4 = (float)Math.Sqrt(1.0 + (double)matrix.M22 - (double)matrix.M11 - (double)matrix.M33);
			result.Y = 0.5f * num4;
			num4 = 1f / (num4 + num4);
			result.X = (matrix.M12 + matrix.M21) * num4;
			result.Z = (matrix.M23 + matrix.M32) * num4;
			result.W = (matrix.M31 - matrix.M13) * num4;
		}
		else
		{
			float num5 = (float)Math.Sqrt(1.0 + (double)matrix.M33 - (double)matrix.M11 - (double)matrix.M22);
			result.Z = 0.5f * num5;
			num5 = 1f / (num5 + num5);
			result.X = (matrix.M13 + matrix.M31) * num5;
			result.Y = (matrix.M23 + matrix.M32) * num5;
			result.W = (matrix.M12 - matrix.M21) * num5;
		}
		return result;
	}
}
