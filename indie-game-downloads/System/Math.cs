using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Versioning;

namespace System;

public static class Math
{
	public const double E = 2.718281828459045;

	public const double PI = 3.141592653589793;

	public const double Tau = Math.PI * 2.0;

	private static ReadOnlySpan<double> RoundPower10Double => new double[16]
	{
		1.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 10000000.0, 100000000.0, 1000000000.0,
		10000000000.0, 100000000000.0, 1000000000000.0, 10000000000000.0, 100000000000000.0, 1000000000000000.0
	};

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Acos(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Acosh(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Asin(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Asinh(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Atan(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Atanh(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Atan2(double y, double x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Cbrt(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Ceiling(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Cos(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Cosh(double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Exp(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Floor(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double FusedMultiplyAdd(double x, double y, double z);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Log(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Log2(double x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Log10(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Pow(double x, double y);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Sin(double a);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static (double Sin, double Cos) SinCos(double x)
	{
		if (RuntimeHelpers.IsKnownConstant(x))
		{
			return (Sin: Sin(x), Cos: Cos(x));
		}
		Unsafe.SkipInit(out double item);
		Unsafe.SkipInit(out double item2);
		SinCos(x, &item, &item2);
		return (Sin: item, Cos: item2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Sinh(double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Sqrt(double d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Tan(double a);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Intrinsic]
	public static extern double Tanh(double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern double ModF(double x, double* intptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SinCos(double x, double* sin, double* cos);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Abs(short value)
	{
		if (value < 0)
		{
			value = (short)(-value);
			if (value < 0)
			{
				ThrowNegateTwosCompOverflow();
			}
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Abs(int value)
	{
		if (value < 0)
		{
			value = -value;
			if (value < 0)
			{
				ThrowNegateTwosCompOverflow();
			}
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Abs(long value)
	{
		if (value < 0)
		{
			value = -value;
			if (value < 0)
			{
				ThrowNegateTwosCompOverflow();
			}
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static nint Abs(nint value)
	{
		if (value < 0)
		{
			value = -value;
			if (value < 0)
			{
				ThrowNegateTwosCompOverflow();
			}
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static sbyte Abs(sbyte value)
	{
		if (value < 0)
		{
			value = (sbyte)(-value);
			if (value < 0)
			{
				ThrowNegateTwosCompOverflow();
			}
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Abs(decimal value)
	{
		return decimal.Abs(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static double Abs(double value)
	{
		return BitConverter.UInt64BitsToDouble(BitConverter.DoubleToUInt64Bits(value) & 0x7FFFFFFFFFFFFFFFL);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Abs(float value)
	{
		return BitConverter.UInt32BitsToSingle(BitConverter.SingleToUInt32Bits(value) & 0x7FFFFFFF);
	}

	[DoesNotReturn]
	[StackTraceHidden]
	internal static void ThrowNegateTwosCompOverflow()
	{
		throw new OverflowException(SR.Overflow_NegateTwosCompNum);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong BigMul(uint a, uint b)
	{
		return (ulong)a * (ulong)b;
	}

	public static long BigMul(int a, int b)
	{
		return (long)a * (long)b;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong BigMul(ulong a, uint b, out ulong low)
	{
		return BigMul(a, (ulong)b, out low);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong BigMul(uint a, ulong b, out ulong low)
	{
		return BigMul(b, a, out low);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public unsafe static ulong BigMul(ulong a, ulong b, out ulong low)
	{
		if (Bmi2.X64.IsSupported)
		{
			Unsafe.SkipInit(out ulong num);
			ulong result = Bmi2.X64.MultiplyNoFlags(a, b, &num);
			low = num;
			return result;
		}
		if (false)
		{
		}
		return SoftwareFallback(a, b, out low);
		static ulong SoftwareFallback(ulong num3, ulong num6, out ulong reference)
		{
			int num2 = (int)num3;
			uint num4 = (uint)(num3 >> 32);
			uint num5 = (uint)num6;
			uint num7 = (uint)(num6 >> 32);
			ulong num8 = (ulong)(uint)num2 * (ulong)num5;
			ulong num9 = (ulong)((long)num4 * (long)num5) + (num8 >> 32);
			ulong num10 = (ulong)((long)(uint)num2 * (long)num7 + (uint)num9);
			reference = (num10 << 32) | (uint)num8;
			return (ulong)((long)num4 * (long)num7 + (long)(num9 >> 32)) + (num10 >> 32);
		}
	}

	public static long BigMul(long a, long b, out long low)
	{
		if (false)
		{
		}
		ulong num = BigMul((ulong)a, (ulong)b, out var low2);
		low = (long)low2;
		return (long)num - ((a >> 63) & b) - ((b >> 63) & a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static UInt128 BigMul(ulong a, ulong b)
	{
		ulong low;
		return new UInt128(BigMul(a, b, out low), low);
	}

	public static Int128 BigMul(long a, long b)
	{
		long low;
		return new Int128((ulong)BigMul(a, b, out low), (ulong)low);
	}

	public static double BitDecrement(double x)
	{
		ulong num = BitConverter.DoubleToUInt64Bits(x);
		if (!double.IsFinite(x))
		{
			if (num != 9218868437227405312L)
			{
				return x;
			}
			return double.MaxValue;
		}
		if (num == 0L)
		{
			return -5E-324;
		}
		num = ((!double.IsNegative(x)) ? (num - 1) : (num + 1));
		return BitConverter.UInt64BitsToDouble(num);
	}

	public static double BitIncrement(double x)
	{
		ulong num = BitConverter.DoubleToUInt64Bits(x);
		if (!double.IsFinite(x))
		{
			if (num != 18442240474082181120uL)
			{
				return x;
			}
			return double.MinValue;
		}
		if (num == 9223372036854775808uL)
		{
			return double.Epsilon;
		}
		num = ((!double.IsNegative(x)) ? (num + 1) : (num - 1));
		return BitConverter.UInt64BitsToDouble(num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CopySign(double x, double y)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			return Vector128.ConditionalSelect(Vector128.CreateScalarUnsafe(-0.0), Vector128.CreateScalarUnsafe(y), Vector128.CreateScalarUnsafe(x)).ToScalar();
		}
		return SoftwareFallback(x, y);
		static double SoftwareFallback(double value, double value2)
		{
			ulong num = BitConverter.DoubleToUInt64Bits(value);
			ulong num2 = BitConverter.DoubleToUInt64Bits(value2);
			return BitConverter.UInt64BitsToDouble((num & 0x7FFFFFFFFFFFFFFFL) | (num2 & 0x8000000000000000uL));
		}
	}

	public static int DivRem(int a, int b, out int result)
	{
		int num = a / b;
		result = a - num * b;
		return num;
	}

	public static long DivRem(long a, long b, out long result)
	{
		long num = a / b;
		result = a - num * b;
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	[CLSCompliant(false)]
	public static (sbyte Quotient, sbyte Remainder) DivRem(sbyte left, sbyte right)
	{
		sbyte b = (sbyte)(left / right);
		return (Quotient: b, Remainder: (sbyte)(left - b * right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	public static (byte Quotient, byte Remainder) DivRem(byte left, byte right)
	{
		byte b = (byte)(left / right);
		return (Quotient: b, Remainder: (byte)(left - b * right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	public static (short Quotient, short Remainder) DivRem(short left, short right)
	{
		short num = (short)(left / right);
		return (Quotient: num, Remainder: (short)(left - num * right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	[CLSCompliant(false)]
	public static (ushort Quotient, ushort Remainder) DivRem(ushort left, ushort right)
	{
		ushort num = (ushort)(left / right);
		return (Quotient: num, Remainder: (ushort)(left - num * right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	public static (int Quotient, int Remainder) DivRem(int left, int right)
	{
		int num = left / right;
		return (Quotient: num, Remainder: left - num * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	[CLSCompliant(false)]
	public static (uint Quotient, uint Remainder) DivRem(uint left, uint right)
	{
		uint num = left / right;
		return (Quotient: num, Remainder: left - num * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	public static (long Quotient, long Remainder) DivRem(long left, long right)
	{
		long num = left / right;
		return (Quotient: num, Remainder: left - num * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	[CLSCompliant(false)]
	public static (ulong Quotient, ulong Remainder) DivRem(ulong left, ulong right)
	{
		ulong num = left / right;
		return (Quotient: num, Remainder: left - num * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	public static (nint Quotient, nint Remainder) DivRem(nint left, nint right)
	{
		nint num = left / right;
		return (Quotient: num, Remainder: left - num * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[NonVersionable]
	[CLSCompliant(false)]
	public static (nuint Quotient, nuint Remainder) DivRem(nuint left, nuint right)
	{
		nuint num = left / right;
		return (Quotient: num, Remainder: left - num * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Ceiling(decimal d)
	{
		return decimal.Ceiling(d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte Clamp(byte value, byte min, byte max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Clamp(decimal value, decimal min, decimal max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Clamp(double value, double min, double max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static short Clamp(short value, short min, short max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Clamp(int value, int min, int max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Clamp(long value, long min, long max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static nint Clamp(nint value, nint min, nint max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp(float value, float min, float max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ushort Clamp(ushort value, ushort min, ushort max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint Clamp(uint value, uint min, uint max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong Clamp(ulong value, ulong min, ulong max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static nuint Clamp(nuint value, nuint min, nuint max)
	{
		if (min > max)
		{
			ThrowMinMaxException(min, max);
		}
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Floor(decimal d)
	{
		return decimal.Floor(d);
	}

	public static double IEEERemainder(double x, double y)
	{
		if (double.IsNaN(x))
		{
			return x;
		}
		if (double.IsNaN(y))
		{
			return y;
		}
		double num = x % y;
		if (double.IsNaN(num))
		{
			return double.NaN;
		}
		if (num == 0.0 && double.IsNegative(x))
		{
			return -0.0;
		}
		double num2 = num - Abs(y) * (double)Sign(x);
		if (Abs(num2) == Abs(num))
		{
			double num3 = x / y;
			if (Abs(Round(num3)) > Abs(num3))
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

	public static int ILogB(double x)
	{
		if (!double.IsNormal(x))
		{
			if (double.IsZero(x))
			{
				return int.MinValue;
			}
			if (!double.IsFinite(x))
			{
				return int.MaxValue;
			}
			return -1022 - (BitOperations.LeadingZeroCount(x.TrailingSignificand) - 11);
		}
		return x.Exponent;
	}

	public static double Log(double a, double newBase)
	{
		if (double.IsNaN(a))
		{
			return a;
		}
		if (double.IsNaN(newBase))
		{
			return newBase;
		}
		if (newBase == 1.0)
		{
			return double.NaN;
		}
		if (a != 1.0 && (newBase == 0.0 || double.IsPositiveInfinity(newBase)))
		{
			return double.NaN;
		}
		return Log(a) / Log(newBase);
	}

	[Intrinsic]
	[NonVersionable]
	public static byte Max(byte val1, byte val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Max(decimal val1, decimal val2)
	{
		return decimal.Max(val1, val2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static double Max(double val1, double val2)
	{
		if (val1 != val2)
		{
			if (!double.IsNaN(val1))
			{
				if (!(val2 < val1))
				{
					return val2;
				}
				return val1;
			}
			return val1;
		}
		if (!double.IsNegative(val2))
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static short Max(short val1, short val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static int Max(int val1, int val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static long Max(long val1, long val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static nint Max(nint val1, nint val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static sbyte Max(sbyte val1, sbyte val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Max(float val1, float val2)
	{
		if (val1 != val2)
		{
			if (!float.IsNaN(val1))
			{
				if (!(val2 < val1))
				{
					return val2;
				}
				return val1;
			}
			return val1;
		}
		if (!float.IsNegative(val2))
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static ushort Max(ushort val1, ushort val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static uint Max(uint val1, uint val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static ulong Max(ulong val1, ulong val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static nuint Max(nuint val1, nuint val2)
	{
		if (val1 < val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	public static double MaxMagnitude(double x, double y)
	{
		double num = Abs(x);
		double num2 = Abs(y);
		if (num > num2 || double.IsNaN(num))
		{
			return x;
		}
		if (num == num2)
		{
			if (!double.IsNegative(x))
			{
				return x;
			}
			return y;
		}
		return y;
	}

	[Intrinsic]
	[NonVersionable]
	public static byte Min(byte val1, byte val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Min(decimal val1, decimal val2)
	{
		return decimal.Min(val1, val2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static double Min(double val1, double val2)
	{
		if (val1 != val2)
		{
			if (!double.IsNaN(val1))
			{
				if (!(val1 < val2))
				{
					return val2;
				}
				return val1;
			}
			return val1;
		}
		if (!double.IsNegative(val1))
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static short Min(short val1, short val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static int Min(int val1, int val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static long Min(long val1, long val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	[NonVersionable]
	public static nint Min(nint val1, nint val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static sbyte Min(sbyte val1, sbyte val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Min(float val1, float val2)
	{
		if (val1 != val2)
		{
			if (!float.IsNaN(val1))
			{
				if (!(val1 < val2))
				{
					return val2;
				}
				return val1;
			}
			return val1;
		}
		if (!float.IsNegative(val1))
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static ushort Min(ushort val1, ushort val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static uint Min(uint val1, uint val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static ulong Min(ulong val1, ulong val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static nuint Min(nuint val1, nuint val2)
	{
		if (val1 > val2)
		{
			return val2;
		}
		return val1;
	}

	[Intrinsic]
	public static double MinMagnitude(double x, double y)
	{
		double num = Abs(x);
		double num2 = Abs(y);
		if (num < num2 || double.IsNaN(num))
		{
			return x;
		}
		if (num == num2)
		{
			if (!double.IsNegative(x))
			{
				return y;
			}
			return x;
		}
		return y;
	}

	[Intrinsic]
	public static double ReciprocalEstimate(double d)
	{
		return ReciprocalEstimate(d);
	}

	[Intrinsic]
	public static double ReciprocalSqrtEstimate(double d)
	{
		return ReciprocalSqrtEstimate(d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Round(decimal d)
	{
		return decimal.Round(d, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Round(decimal d, int decimals)
	{
		return decimal.Round(d, decimals);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Round(decimal d, MidpointRounding mode)
	{
		return decimal.Round(d, 0, mode);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Round(decimal d, int decimals, MidpointRounding mode)
	{
		return decimal.Round(d, decimals, mode);
	}

	[Intrinsic]
	public static double Round(double a)
	{
		if (Abs(a) >= 4503599627370496.0)
		{
			return a;
		}
		double num = CopySign(4503599627370496.0, a);
		return CopySign(a + num - num, a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Round(double value, int digits)
	{
		return Round(value, digits, MidpointRounding.ToEven);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Round(double value, MidpointRounding mode)
	{
		switch (mode)
		{
		case MidpointRounding.AwayFromZero:
			if (false)
			{
			}
			return Truncate(value + CopySign(0.49999999999999994, value));
		case MidpointRounding.ToEven:
			return Round(value);
		case MidpointRounding.ToZero:
			return Truncate(value);
		case MidpointRounding.ToNegativeInfinity:
			return Floor(value);
		case MidpointRounding.ToPositiveInfinity:
			return Ceiling(value);
		default:
			ThrowHelper.ThrowArgumentException_InvalidEnumValue(mode, "mode");
			return 0.0;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Round(double value, int digits, MidpointRounding mode)
	{
		if ((uint)digits > 15u)
		{
			ThrowHelper.ThrowArgumentOutOfRange_RoundingDigits("digits");
		}
		if (Abs(value) < 10000000000000000.0)
		{
			double num = RoundPower10Double[digits];
			value = Round(value * num, mode) / num;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(decimal value)
	{
		return decimal.Sign(value);
	}

	public static int Sign(double value)
	{
		if (value < 0.0)
		{
			return -1;
		}
		if (value > 0.0)
		{
			return 1;
		}
		if (value == 0.0)
		{
			return 0;
		}
		throw new ArithmeticException(SR.Arithmetic_NaN);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Sign(short value)
	{
		return Sign((int)value);
	}

	public static int Sign(int value)
	{
		return (value >> 31) | (-value >>> 31);
	}

	public static int Sign(long value)
	{
		return (int)((value >> 63) | (-value >>> 63));
	}

	public static int Sign(nint value)
	{
		return (int)((long)(value >> (0x3F & 0x3F)) | (long)((ulong)(-value) >> 63));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static int Sign(sbyte value)
	{
		return Sign((int)value);
	}

	public static int Sign(float value)
	{
		if (value < 0f)
		{
			return -1;
		}
		if (value > 0f)
		{
			return 1;
		}
		if (value == 0f)
		{
			return 0;
		}
		throw new ArithmeticException(SR.Arithmetic_NaN);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal Truncate(decimal d)
	{
		return decimal.Truncate(d);
	}

	[Intrinsic]
	public unsafe static double Truncate(double d)
	{
		ModF(d, &d);
		return d;
	}

	[DoesNotReturn]
	internal static void ThrowMinMaxException<T>(T min, T max)
	{
		throw new ArgumentException(SR.Format(SR.Argument_MinMaxValue, min, max));
	}

	public static double ScaleB(double x, int n)
	{
		double num = x;
		if (n > 1023)
		{
			num *= 8.98846567431158E+307;
			n -= 1023;
			if (n > 1023)
			{
				num *= 8.98846567431158E+307;
				n -= 1023;
				if (n > 1023)
				{
					n = 1023;
				}
			}
		}
		else if (n < -1022)
		{
			num *= 2.004168360008973E-292;
			n += 969;
			if (n < -1022)
			{
				num *= 2.004168360008973E-292;
				n += 969;
				if (n < -1022)
				{
					n = -1022;
				}
			}
		}
		double num2 = BitConverter.Int64BitsToDouble((long)(1023 + n) << 52);
		return num * num2;
	}

	[StackTraceHidden]
	internal static int ConvertToInt32Checked(double value)
	{
		if (value > -2147483649.0 && value < 2147483648.0)
		{
			return double.ConvertToIntegerNative<int>(value);
		}
		ThrowHelper.ThrowOverflowException();
		return 0;
	}

	[StackTraceHidden]
	internal static uint ConvertToUInt32Checked(double value)
	{
		if (value > -1.0 && value < 4294967296.0)
		{
			return double.ConvertToIntegerNative<uint>(value);
		}
		ThrowHelper.ThrowOverflowException();
		return 0u;
	}

	[StackTraceHidden]
	internal static long ConvertToInt64Checked(double value)
	{
		if (value > -9.223372036854778E+18 && value < 9.223372036854776E+18)
		{
			return double.ConvertToIntegerNative<long>(value);
		}
		ThrowHelper.ThrowOverflowException();
		return 0L;
	}

	[StackTraceHidden]
	internal static ulong ConvertToUInt64Checked(double value)
	{
		if (value > -1.0 && value < 1.8446744073709552E+19)
		{
			return double.ConvertToIntegerNative<ulong>(value);
		}
		ThrowHelper.ThrowOverflowException();
		return 0uL;
	}
}
