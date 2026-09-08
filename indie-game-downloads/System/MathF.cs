using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace System;

public static class MathF
{
	public const float E = 2.7182817f;

	public const float PI = 3.1415927f;

	public const float Tau = (float)Math.PI * 2f;

	private static ReadOnlySpan<float> RoundPower10Single => new float[7] { 1f, 10f, 100f, 1000f, 10000f, 100000f, 1000000f };

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Acos(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Acosh(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Asin(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Asinh(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Atan(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Atanh(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Atan2(float y, float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Cbrt(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Ceiling(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Cos(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Cosh(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Exp(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Floor(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float FusedMultiplyAdd(float x, float y, float z);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Log(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Log2(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Log10(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Pow(float x, float y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Sin(float x);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static (float Sin, float Cos) SinCos(float x)
	{
		if (RuntimeHelpers.IsKnownConstant(x))
		{
			return (Sin: Sin(x), Cos: Cos(x));
		}
		Unsafe.SkipInit(out float item);
		Unsafe.SkipInit(out float item2);
		SinCos(x, &item, &item2);
		return (Sin: item, Cos: item2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Sinh(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Sqrt(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Tan(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern float Tanh(float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern float ModF(float x, float* intptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SinCos(float x, float* sin, float* cos);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Abs(float x)
	{
		return Math.Abs(x);
	}

	public static float BitDecrement(float x)
	{
		uint num = BitConverter.SingleToUInt32Bits(x);
		if (!float.IsFinite(x))
		{
			if (num != 2139095040)
			{
				return x;
			}
			return float.MaxValue;
		}
		if (num == 0)
		{
			return -1E-45f;
		}
		num = ((!float.IsNegative(x)) ? (num - 1) : (num + 1));
		return BitConverter.UInt32BitsToSingle(num);
	}

	public static float BitIncrement(float x)
	{
		uint num = BitConverter.SingleToUInt32Bits(x);
		if (!float.IsFinite(x))
		{
			if (num != 4286578688u)
			{
				return x;
			}
			return float.MinValue;
		}
		if (num == 2147483648u)
		{
			return float.Epsilon;
		}
		num = ((!float.IsNegative(x)) ? (num + 1) : (num - 1));
		return BitConverter.UInt32BitsToSingle(num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CopySign(float x, float y)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			return Vector128.ConditionalSelect(Vector128.CreateScalarUnsafe(-0f), Vector128.CreateScalarUnsafe(y), Vector128.CreateScalarUnsafe(x)).ToScalar();
		}
		return SoftwareFallback(x, y);
		static float SoftwareFallback(float value, float value2)
		{
			uint num = BitConverter.SingleToUInt32Bits(value);
			uint num2 = BitConverter.SingleToUInt32Bits(value2);
			return BitConverter.UInt32BitsToSingle((num & 0x7FFFFFFF) | (num2 & 0x80000000u));
		}
	}

	public static float IEEERemainder(float x, float y)
	{
		if (float.IsNaN(x))
		{
			return x;
		}
		if (float.IsNaN(y))
		{
			return y;
		}
		float num = x % y;
		if (float.IsNaN(num))
		{
			return float.NaN;
		}
		if (num == 0f && float.IsNegative(x))
		{
			return -0f;
		}
		float num2 = num - Abs(y) * (float)Sign(x);
		if (Abs(num2) == Abs(num))
		{
			float x2 = x / y;
			if (Abs(Round(x2)) > Abs(x2))
			{
				return num2;
			}
			return num;
		}
		if (Abs(num2) < Abs(num))
		{
			return num2;
		}
		return num;
	}

	public static int ILogB(float x)
	{
		if (!float.IsNormal(x))
		{
			if (float.IsZero(x))
			{
				return int.MinValue;
			}
			if (!float.IsFinite(x))
			{
				return int.MaxValue;
			}
			return -126 - (BitOperations.LeadingZeroCount(x.TrailingSignificand) - 8);
		}
		return x.Exponent;
	}

	public static float Log(float x, float y)
	{
		if (float.IsNaN(x))
		{
			return x;
		}
		if (float.IsNaN(y))
		{
			return y;
		}
		if (y == 1f)
		{
			return float.NaN;
		}
		if (x != 1f && (y == 0f || float.IsPositiveInfinity(y)))
		{
			return float.NaN;
		}
		return Log(x) / Log(y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Max(float x, float y)
	{
		return Math.Max(x, y);
	}

	[Intrinsic]
	public static float MaxMagnitude(float x, float y)
	{
		float num = Abs(x);
		float num2 = Abs(y);
		if (num > num2 || float.IsNaN(num))
		{
			return x;
		}
		if (num == num2)
		{
			if (!float.IsNegative(x))
			{
				return x;
			}
			return y;
		}
		return y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Min(float x, float y)
	{
		return Math.Min(x, y);
	}

	[Intrinsic]
	public static float MinMagnitude(float x, float y)
	{
		float num = Abs(x);
		float num2 = Abs(y);
		if (num < num2 || float.IsNaN(num))
		{
			return x;
		}
		if (num == num2)
		{
			if (!float.IsNegative(x))
			{
				return y;
			}
			return x;
		}
		return y;
	}

	[Intrinsic]
	public static float ReciprocalEstimate(float x)
	{
		return ReciprocalEstimate(x);
	}

	[Intrinsic]
	public static float ReciprocalSqrtEstimate(float x)
	{
		return ReciprocalSqrtEstimate(x);
	}

	[Intrinsic]
	public static float Round(float x)
	{
		if (Abs(x) >= 8388608f)
		{
			return x;
		}
		float num = CopySign(8388608f, x);
		return CopySign(x + num - num, x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Round(float x, int digits)
	{
		return Round(x, digits, MidpointRounding.ToEven);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Round(float x, MidpointRounding mode)
	{
		switch (mode)
		{
		case MidpointRounding.AwayFromZero:
			if (false)
			{
			}
			return Truncate(x + CopySign(0.49999997f, x));
		case MidpointRounding.ToEven:
			return Round(x);
		case MidpointRounding.ToZero:
			return Truncate(x);
		case MidpointRounding.ToNegativeInfinity:
			return Floor(x);
		case MidpointRounding.ToPositiveInfinity:
			return Ceiling(x);
		default:
			ThrowHelper.ThrowArgumentException_InvalidEnumValue(mode, "mode");
			return 0f;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Round(float x, int digits, MidpointRounding mode)
	{
		if ((uint)digits > 6u)
		{
			ThrowHelper.ThrowArgumentOutOfRange_RoundingDigits_MathF("digits");
		}
		if (Abs(x) < 100000000f)
		{
			float num = RoundPower10Single[digits];
			x = Round(x * num, mode) / num;
		}
		return x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(float x)
	{
		return Math.Sign(x);
	}

	[Intrinsic]
	public unsafe static float Truncate(float x)
	{
		ModF(x, &x);
		return x;
	}

	public static float ScaleB(float x, int n)
	{
		float num = x;
		if (n > 127)
		{
			num *= 1.7014118E+38f;
			n -= 127;
			if (n > 127)
			{
				num *= 1.7014118E+38f;
				n -= 127;
				if (n > 127)
				{
					n = 127;
				}
			}
		}
		else if (n < -126)
		{
			num *= 1.9721523E-31f;
			n += 102;
			if (n < -126)
			{
				num *= 1.9721523E-31f;
				n += 102;
				if (n < -126)
				{
					n = -126;
				}
			}
		}
		float num2 = BitConverter.Int32BitsToSingle(127 + n << 23);
		return num * num2;
	}
}
