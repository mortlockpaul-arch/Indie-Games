using System;
using System.Runtime.InteropServices;

namespace Microsoft.XboxLive.MathUtilities;

public sealed class QuaternionMath
{
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct FloatInt
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public uint i;
	}

	private QuaternionMath()
	{
	}

	public static Quaternion FastNormalizedLerp(ref Quaternion quaternionA, ref Quaternion quaternionB, float weight)
	{
		Quaternion result = new Quaternion
		{
			X = quaternionA.X + weight * (quaternionB.X - quaternionA.X),
			Y = quaternionA.Y + weight * (quaternionB.Y - quaternionA.Y),
			Z = quaternionA.Z + weight * (quaternionB.Z - quaternionA.Z),
			W = quaternionA.W + weight * (quaternionB.W - quaternionA.W)
		};
		float num = result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W;
		FloatInt floatInt = default(FloatInt);
		floatInt.f = num;
		floatInt.i = (uint)(-1100040948 - (int)floatInt.i) >> 1;
		floatInt.f = 0.5f * floatInt.f * (3f - num * floatInt.f * floatInt.f);
		result.X *= floatInt.f;
		result.Y *= floatInt.f;
		result.Z *= floatInt.f;
		result.W *= floatInt.f;
		return result;
	}

	public static Quaternion NormalizedLerp(ref Quaternion quaternionA, ref Quaternion quaternionB, float weight)
	{
		Quaternion result = new Quaternion
		{
			X = quaternionA.X + weight * (quaternionB.X - quaternionA.X),
			Y = quaternionA.Y + weight * (quaternionB.Y - quaternionA.Y),
			Z = quaternionA.Z + weight * (quaternionB.Z - quaternionA.Z),
			W = quaternionA.W + weight * (quaternionB.W - quaternionA.W)
		};
		float num = result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W;
		if (num > 0f)
		{
			num = (float)(1.0 / Math.Sqrt(num));
			result.X *= num;
			result.Y *= num;
			result.Z *= num;
			result.W *= num;
		}
		return result;
	}

	public static Quaternion Exp(Vector3 vector)
	{
		Quaternion result = default(Quaternion);
		float num = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
		if (num < 1E-07f)
		{
			result.X = 0f;
			result.Y = 0f;
			result.Z = 0f;
			result.W = 1f;
		}
		else
		{
			float num2 = (float)Math.Sin(num);
			float w = (float)Math.Cos(num);
			float num3 = num2 / num;
			result.X = vector.X * num3;
			result.Y = vector.Y * num3;
			result.Z = vector.Z * num3;
			result.W = w;
		}
		return result;
	}

	public static Vector3 Ln(Quaternion quat)
	{
		float w = quat.W;
		float num = (float)Math.Acos(quat.W);
		float num2 = (float)Math.Sin(num);
		Vector3 result = default(Vector3);
		if (Math.Abs(num2) < 1E-07f)
		{
			result.X = 0f;
			result.Y = 0f;
			result.Z = 0f;
		}
		else
		{
			float num3 = num / num2;
			result.X = quat.X * num3;
			result.Y = quat.Y * num3;
			result.Z = quat.Z * num3;
		}
		return result;
	}
}
