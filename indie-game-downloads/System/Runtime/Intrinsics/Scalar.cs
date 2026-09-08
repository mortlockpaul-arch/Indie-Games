using System.Runtime.CompilerServices;

namespace System.Runtime.Intrinsics;

internal static class Scalar<T>
{
	public static T AllBitsSet
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (typeof(T) == typeof(byte))
			{
				return (T)(object)byte.MaxValue;
			}
			if (typeof(T) == typeof(double))
			{
				return (T)(object)BitConverter.Int64BitsToDouble(-1L);
			}
			if (typeof(T) == typeof(short))
			{
				return (T)(object)(short)(-1);
			}
			if (typeof(T) == typeof(int))
			{
				return (T)(object)(-1);
			}
			if (typeof(T) == typeof(long))
			{
				return (T)(object)(-1L);
			}
			if (typeof(T) == typeof(nint))
			{
				return (T)(object)(nint)(-1);
			}
			if (typeof(T) == typeof(nuint))
			{
				return (T)(object)UIntPtr.MaxValue;
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (T)(object)(sbyte)(-1);
			}
			if (typeof(T) == typeof(float))
			{
				return (T)(object)BitConverter.Int32BitsToSingle(-1);
			}
			if (typeof(T) == typeof(ushort))
			{
				return (T)(object)ushort.MaxValue;
			}
			if (typeof(T) == typeof(uint))
			{
				return (T)(object)uint.MaxValue;
			}
			if (typeof(T) == typeof(ulong))
			{
				return (T)(object)ulong.MaxValue;
			}
			ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
			return default(T);
		}
	}

	public static T One
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (typeof(T) == typeof(byte))
			{
				return (T)(object)(byte)1;
			}
			if (typeof(T) == typeof(double))
			{
				return (T)(object)1.0;
			}
			if (typeof(T) == typeof(short))
			{
				return (T)(object)(short)1;
			}
			if (typeof(T) == typeof(int))
			{
				return (T)(object)1;
			}
			if (typeof(T) == typeof(long))
			{
				return (T)(object)1L;
			}
			if (typeof(T) == typeof(nint))
			{
				return (T)(object)(nint)1;
			}
			if (typeof(T) == typeof(nuint))
			{
				return (T)(object)(nuint)1u;
			}
			if (typeof(T) == typeof(sbyte))
			{
				return (T)(object)(sbyte)1;
			}
			if (typeof(T) == typeof(float))
			{
				return (T)(object)1f;
			}
			if (typeof(T) == typeof(ushort))
			{
				return (T)(object)(ushort)1;
			}
			if (typeof(T) == typeof(uint))
			{
				return (T)(object)1u;
			}
			if (typeof(T) == typeof(ulong))
			{
				return (T)(object)1uL;
			}
			ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
			return default(T);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Abs(T value)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)Math.Abs((double)(object)value);
		}
		if (typeof(T) == typeof(short))
		{
			short num = (short)(object)value;
			if (num < 0)
			{
				num = (short)(-num);
			}
			return (T)(object)num;
		}
		if (typeof(T) == typeof(int))
		{
			int num2 = (int)(object)value;
			if (num2 < 0)
			{
				num2 = -num2;
			}
			return (T)(object)num2;
		}
		if (typeof(T) == typeof(long))
		{
			long num3 = (long)(object)value;
			if (num3 < 0)
			{
				num3 = -num3;
			}
			return (T)(object)num3;
		}
		if (typeof(T) == typeof(nint))
		{
			nint num4 = (nint)(object)value;
			if (num4 < 0)
			{
				num4 = -num4;
			}
			return (T)(object)num4;
		}
		if (typeof(T) == typeof(sbyte))
		{
			sbyte b = (sbyte)(object)value;
			if (b < 0)
			{
				b = (sbyte)(-b);
			}
			return (T)(object)b;
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)Math.Abs((float)(object)value);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Add(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((byte)(object)left + (byte)(object)right);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)((double)(object)left + (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((short)(object)left + (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)left + (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)left + (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)left + (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)left + (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((sbyte)(object)left + (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)((float)(object)left + (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((ushort)(object)left + (ushort)(object)right);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)left + (uint)(object)right);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)left + (ulong)(object)right);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T AddSaturate(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			byte b = (byte)(object)left;
			byte b2 = (byte)(object)right;
			byte b3 = (byte)(b + b2);
			if (b3 < b)
			{
				b3 = byte.MaxValue;
			}
			return (T)(object)b3;
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)((double)(object)left + (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			short num = (short)(object)left;
			short num2 = (short)(object)right;
			short num3 = (short)(num + num2);
			if (((num3 ^ num) & ~(num ^ num2)) < 0)
			{
				num3 = ((num3 < 0) ? short.MaxValue : short.MinValue);
			}
			return (T)(object)num3;
		}
		if (typeof(T) == typeof(int))
		{
			int num4 = (int)(object)left;
			int num5 = (int)(object)right;
			int num6 = num4 + num5;
			if (((num6 ^ num4) & ~(num4 ^ num5)) < 0)
			{
				num6 = ((num6 < 0) ? int.MaxValue : int.MinValue);
			}
			return (T)(object)num6;
		}
		if (typeof(T) == typeof(long))
		{
			long num7 = (long)(object)left;
			long num8 = (long)(object)right;
			long num9 = num7 + num8;
			if (((num9 ^ num7) & ~(num7 ^ num8)) < 0)
			{
				num9 = ((num9 < 0) ? long.MaxValue : long.MinValue);
			}
			return (T)(object)num9;
		}
		if (typeof(T) == typeof(nint))
		{
			nint num10 = (nint)(object)left;
			nint num11 = (nint)(object)right;
			nint num12 = num10 + num11;
			if (((num12 ^ num10) & ~(num10 ^ num11)) < 0)
			{
				num12 = ((num12 < 0) ? IntPtr.MaxValue : IntPtr.MinValue);
			}
			return (T)(object)num12;
		}
		if (typeof(T) == typeof(nuint))
		{
			nuint num13 = (nuint)(object)left;
			nuint num14 = (nuint)(object)right;
			nuint num15 = num13 + num14;
			if (num15 < num13)
			{
				num15 = UIntPtr.MaxValue;
			}
			return (T)(object)num15;
		}
		if (typeof(T) == typeof(sbyte))
		{
			sbyte b4 = (sbyte)(object)left;
			sbyte b5 = (sbyte)(object)right;
			sbyte b6 = (sbyte)(b4 + b5);
			if (((b6 ^ b4) & ~(b4 ^ b5)) < 0)
			{
				b6 = ((b6 < 0) ? sbyte.MaxValue : sbyte.MinValue);
			}
			return (T)(object)b6;
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)((float)(object)left + (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			ushort num16 = (ushort)(object)left;
			ushort num17 = (ushort)(object)right;
			ushort num18 = (ushort)(num16 + num17);
			if (num18 < num16)
			{
				num18 = ushort.MaxValue;
			}
			return (T)(object)num18;
		}
		if (typeof(T) == typeof(uint))
		{
			uint num19 = (uint)(object)left;
			uint num20 = (uint)(object)right;
			uint num21 = num19 + num20;
			if (num21 < num19)
			{
				num21 = uint.MaxValue;
			}
			return (T)(object)num21;
		}
		if (typeof(T) == typeof(ulong))
		{
			ulong num22 = (ulong)(object)left;
			ulong num23 = (ulong)(object)right;
			ulong num24 = num22 + num23;
			if (num24 < num22)
			{
				num24 = ulong.MaxValue;
			}
			return (T)(object)num24;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Ceiling(T value)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)Math.Ceiling((double)(object)value);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)MathF.Ceiling((float)(object)value);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Convert(int value)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)value;
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)(double)value;
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)value;
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)value;
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)(long)value;
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)(nint)value;
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)(nuint)value;
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)value;
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)(float)value;
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)value;
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)(uint)value;
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)(ulong)value;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T CopySign(T value, T sign)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.CopySign((double)(object)value, (double)(object)sign);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)short.CopySign((short)(object)value, (short)(object)sign);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)int.CopySign((int)(object)value, (int)(object)sign);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)long.CopySign((long)(object)value, (long)(object)sign);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)IntPtr.CopySign((nint)(object)value, (nint)(object)sign);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)sbyte.CopySign((sbyte)(object)value, (sbyte)(object)sign);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.CopySign((float)(object)value, (float)(object)sign);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Divide(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((byte)(object)left / (byte)(object)right);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)((double)(object)left / (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((short)(object)left / (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)left / (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)left / (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)left / (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)left / (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((sbyte)(object)left / (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)((float)(object)left / (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((ushort)(object)left / (ushort)(object)right);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)left / (uint)(object)right);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)left / (ulong)(object)right);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Equals(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (byte)(object)left == (byte)(object)right;
		}
		if (typeof(T) == typeof(double))
		{
			return (double)(object)left == (double)(object)right;
		}
		if (typeof(T) == typeof(short))
		{
			return (short)(object)left == (short)(object)right;
		}
		if (typeof(T) == typeof(int))
		{
			return (int)(object)left == (int)(object)right;
		}
		if (typeof(T) == typeof(long))
		{
			return (long)(object)left == (long)(object)right;
		}
		if (typeof(T) == typeof(nint))
		{
			return (nint)(object)left == (nint)(object)right;
		}
		if (typeof(T) == typeof(nuint))
		{
			return (nuint)(object)left == (nuint)(object)right;
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (sbyte)(object)left == (sbyte)(object)right;
		}
		if (typeof(T) == typeof(float))
		{
			return (float)(object)left == (float)(object)right;
		}
		if (typeof(T) == typeof(ushort))
		{
			return (ushort)(object)left == (ushort)(object)right;
		}
		if (typeof(T) == typeof(uint))
		{
			return (uint)(object)left == (uint)(object)right;
		}
		if (typeof(T) == typeof(ulong))
		{
			return (ulong)(object)left == (ulong)(object)right;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ExtractMostSignificantBit(T value)
	{
		if (typeof(T) == typeof(byte))
		{
			return (uint)(byte)(object)value >> 7;
		}
		if (typeof(T) == typeof(double))
		{
			return (uint)(BitConverter.DoubleToUInt64Bits((double)(object)value) >> 63);
		}
		if (typeof(T) == typeof(short))
		{
			return (uint)(ushort)(short)(object)value >> 15;
		}
		if (typeof(T) == typeof(int))
		{
			return (uint)(int)(object)value >> 31;
		}
		if (typeof(T) == typeof(long))
		{
			return (uint)((ulong)(long)(object)value >> 63);
		}
		if (typeof(T) == typeof(nint))
		{
			return (uint)((ulong)(nint)(object)value >> 63);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (uint)((ulong)(nuint)(object)value >> 63);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (uint)(byte)(sbyte)(object)value >> 7;
		}
		if (typeof(T) == typeof(float))
		{
			return BitConverter.SingleToUInt32Bits((float)(object)value) >> 31;
		}
		if (typeof(T) == typeof(ushort))
		{
			return (uint)(ushort)(object)value >> 15;
		}
		if (typeof(T) == typeof(uint))
		{
			return (uint)(object)value >> 31;
		}
		if (typeof(T) == typeof(ulong))
		{
			return (uint)((ulong)(object)value >> 63);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return 0u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Floor(T value)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)Math.Floor((double)(object)value);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)MathF.Floor((float)(object)value);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool GreaterThan(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (byte)(object)left > (byte)(object)right;
		}
		if (typeof(T) == typeof(double))
		{
			return (double)(object)left > (double)(object)right;
		}
		if (typeof(T) == typeof(short))
		{
			return (short)(object)left > (short)(object)right;
		}
		if (typeof(T) == typeof(int))
		{
			return (int)(object)left > (int)(object)right;
		}
		if (typeof(T) == typeof(long))
		{
			return (long)(object)left > (long)(object)right;
		}
		if (typeof(T) == typeof(nint))
		{
			return (nint)(object)left > (nint)(object)right;
		}
		if (typeof(T) == typeof(nuint))
		{
			return (nuint)(object)left > (nuint)(object)right;
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (sbyte)(object)left > (sbyte)(object)right;
		}
		if (typeof(T) == typeof(float))
		{
			return (float)(object)left > (float)(object)right;
		}
		if (typeof(T) == typeof(ushort))
		{
			return (ushort)(object)left > (ushort)(object)right;
		}
		if (typeof(T) == typeof(uint))
		{
			return (uint)(object)left > (uint)(object)right;
		}
		if (typeof(T) == typeof(ulong))
		{
			return (ulong)(object)left > (ulong)(object)right;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool GreaterThanOrEqual(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (byte)(object)left >= (byte)(object)right;
		}
		if (typeof(T) == typeof(double))
		{
			return (double)(object)left >= (double)(object)right;
		}
		if (typeof(T) == typeof(short))
		{
			return (short)(object)left >= (short)(object)right;
		}
		if (typeof(T) == typeof(int))
		{
			return (int)(object)left >= (int)(object)right;
		}
		if (typeof(T) == typeof(long))
		{
			return (long)(object)left >= (long)(object)right;
		}
		if (typeof(T) == typeof(nint))
		{
			return (nint)(object)left >= (nint)(object)right;
		}
		if (typeof(T) == typeof(nuint))
		{
			return (nuint)(object)left >= (nuint)(object)right;
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (sbyte)(object)left >= (sbyte)(object)right;
		}
		if (typeof(T) == typeof(float))
		{
			return (float)(object)left >= (float)(object)right;
		}
		if (typeof(T) == typeof(ushort))
		{
			return (ushort)(object)left >= (ushort)(object)right;
		}
		if (typeof(T) == typeof(uint))
		{
			return (uint)(object)left >= (uint)(object)right;
		}
		if (typeof(T) == typeof(ulong))
		{
			return (ulong)(object)left >= (ulong)(object)right;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool LessThan(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (byte)(object)left < (byte)(object)right;
		}
		if (typeof(T) == typeof(double))
		{
			return (double)(object)left < (double)(object)right;
		}
		if (typeof(T) == typeof(short))
		{
			return (short)(object)left < (short)(object)right;
		}
		if (typeof(T) == typeof(int))
		{
			return (int)(object)left < (int)(object)right;
		}
		if (typeof(T) == typeof(long))
		{
			return (long)(object)left < (long)(object)right;
		}
		if (typeof(T) == typeof(nint))
		{
			return (nint)(object)left < (nint)(object)right;
		}
		if (typeof(T) == typeof(nuint))
		{
			return (nuint)(object)left < (nuint)(object)right;
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (sbyte)(object)left < (sbyte)(object)right;
		}
		if (typeof(T) == typeof(float))
		{
			return (float)(object)left < (float)(object)right;
		}
		if (typeof(T) == typeof(ushort))
		{
			return (ushort)(object)left < (ushort)(object)right;
		}
		if (typeof(T) == typeof(uint))
		{
			return (uint)(object)left < (uint)(object)right;
		}
		if (typeof(T) == typeof(ulong))
		{
			return (ulong)(object)left < (ulong)(object)right;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool LessThanOrEqual(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (byte)(object)left <= (byte)(object)right;
		}
		if (typeof(T) == typeof(double))
		{
			return (double)(object)left <= (double)(object)right;
		}
		if (typeof(T) == typeof(short))
		{
			return (short)(object)left <= (short)(object)right;
		}
		if (typeof(T) == typeof(int))
		{
			return (int)(object)left <= (int)(object)right;
		}
		if (typeof(T) == typeof(long))
		{
			return (long)(object)left <= (long)(object)right;
		}
		if (typeof(T) == typeof(nint))
		{
			return (nint)(object)left <= (nint)(object)right;
		}
		if (typeof(T) == typeof(nuint))
		{
			return (nuint)(object)left <= (nuint)(object)right;
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (sbyte)(object)left <= (sbyte)(object)right;
		}
		if (typeof(T) == typeof(float))
		{
			return (float)(object)left <= (float)(object)right;
		}
		if (typeof(T) == typeof(ushort))
		{
			return (ushort)(object)left <= (ushort)(object)right;
		}
		if (typeof(T) == typeof(uint))
		{
			return (uint)(object)left <= (uint)(object)right;
		}
		if (typeof(T) == typeof(ulong))
		{
			return (ulong)(object)left <= (ulong)(object)right;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Max(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)byte.Max((byte)(object)left, (byte)(object)right);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.Max((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)short.Max((short)(object)left, (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)int.Max((int)(object)left, (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)long.Max((long)(object)left, (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)IntPtr.Max((nint)(object)left, (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)UIntPtr.Max((nuint)(object)left, (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)sbyte.Max((sbyte)(object)left, (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.Max((float)(object)left, (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)ushort.Max((ushort)(object)left, (ushort)(object)right);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)uint.Max((uint)(object)left, (uint)(object)right);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)ulong.Max((ulong)(object)left, (ulong)(object)right);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T MaxMagnitude(T left, T right)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.MaxMagnitude((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)short.MaxMagnitude((short)(object)left, (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)int.MaxMagnitude((int)(object)left, (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)long.MaxMagnitude((long)(object)left, (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)IntPtr.MaxMagnitude((nint)(object)left, (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)UIntPtr.Max((nuint)(object)left, (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)sbyte.MaxMagnitude((sbyte)(object)left, (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.MaxMagnitude((float)(object)left, (float)(object)right);
		}
		return Max(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T MaxMagnitudeNumber(T left, T right)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.MaxMagnitudeNumber((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.MaxMagnitudeNumber((float)(object)left, (float)(object)right);
		}
		return MaxMagnitude(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T MaxNumber(T left, T right)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.MaxNumber((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.MaxNumber((float)(object)left, (float)(object)right);
		}
		return Max(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Min(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)byte.Min((byte)(object)left, (byte)(object)right);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.Min((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)short.Min((short)(object)left, (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)int.Min((int)(object)left, (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)long.Min((long)(object)left, (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)IntPtr.Min((nint)(object)left, (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)UIntPtr.Min((nuint)(object)left, (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)sbyte.Min((sbyte)(object)left, (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.Min((float)(object)left, (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)ushort.Min((ushort)(object)left, (ushort)(object)right);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)uint.Min((uint)(object)left, (uint)(object)right);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)ulong.Min((ulong)(object)left, (ulong)(object)right);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T MinMagnitude(T left, T right)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.MinMagnitude((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)short.MinMagnitude((short)(object)left, (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)int.MinMagnitude((int)(object)left, (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)long.MinMagnitude((long)(object)left, (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)IntPtr.MinMagnitude((nint)(object)left, (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)UIntPtr.Min((nuint)(object)left, (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)sbyte.MinMagnitude((sbyte)(object)left, (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.MinMagnitude((float)(object)left, (float)(object)right);
		}
		return Min(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T MinMagnitudeNumber(T left, T right)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.MinMagnitudeNumber((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.MinMagnitudeNumber((float)(object)left, (float)(object)right);
		}
		return MinMagnitude(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T MinNumber(T left, T right)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.MinNumber((double)(object)left, (double)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.MinNumber((float)(object)left, (float)(object)right);
		}
		return Min(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Multiply(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((byte)(object)left * (byte)(object)right);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)((double)(object)left * (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((short)(object)left * (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)left * (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)left * (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)left * (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)left * (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((sbyte)(object)left * (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)((float)(object)left * (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((ushort)(object)left * (ushort)(object)right);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)left * (uint)(object)right);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)left * (ulong)(object)right);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T MultiplyAddEstimate(T left, T right, T addend)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((byte)(object)left * (byte)(object)right + (byte)(object)addend);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)double.MultiplyAddEstimate((double)(object)left, (double)(object)right, (double)(object)addend);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((short)(object)left * (short)(object)right + (short)(object)addend);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)left * (int)(object)right + (int)(object)addend);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)left * (long)(object)right + (long)(object)addend);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)left * (nint)(object)right + (nint)(object)addend);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)left * (nuint)(object)right + (nuint)(object)addend);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((sbyte)(object)left * (sbyte)(object)right + (sbyte)(object)addend);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)float.MultiplyAddEstimate((float)(object)left, (float)(object)right, (float)(object)addend);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((ushort)(object)left * (ushort)(object)right + (ushort)(object)addend);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)left * (uint)(object)right + (uint)(object)addend);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)left * (ulong)(object)right + (ulong)(object)addend);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	public static bool ObjectEquals(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return ((byte)(object)left).Equals((byte)(object)right);
		}
		if (typeof(T) == typeof(double))
		{
			return ((double)(object)left).Equals((double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return ((short)(object)left).Equals((short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return ((int)(object)left).Equals((int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return ((long)(object)left).Equals((long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return ((IntPtr)(nint)(object)left).Equals((nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return ((UIntPtr)(nuint)(object)left).Equals((nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return ((sbyte)(object)left).Equals((sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return ((float)(object)left).Equals((float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			return ((ushort)(object)left).Equals((ushort)(object)right);
		}
		if (typeof(T) == typeof(uint))
		{
			return ((uint)(object)left).Equals((uint)(object)right);
		}
		if (typeof(T) == typeof(ulong))
		{
			return ((ulong)(object)left).Equals((ulong)(object)right);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Round(T value)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)Math.Round((double)(object)value);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)MathF.Round((float)(object)value);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ShiftLeft(T value, int shiftCount)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((byte)(object)value << (shiftCount & 7));
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits((double)(object)value) << shiftCount);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((short)(object)value << (shiftCount & 0xF));
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)value << shiftCount);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)value << shiftCount);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)value << (shiftCount & 0x3F));
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)value << (shiftCount & 0x3F));
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((sbyte)(object)value << (shiftCount & 7));
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits((float)(object)value) << shiftCount);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((ushort)(object)value << (shiftCount & 0xF));
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)value << shiftCount);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)value << shiftCount);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ShiftRightArithmetic(T value, int shiftCount)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((byte)(object)value >> (shiftCount & 7));
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits((double)(object)value) >> shiftCount);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((short)(object)value >> (shiftCount & 0xF));
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)value >> shiftCount);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)value >> shiftCount);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)value >> (shiftCount & 0x3F));
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)value >> (shiftCount & 0x3F));
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((sbyte)(object)value >> (shiftCount & 7));
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits((float)(object)value) >> shiftCount);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((ushort)(object)value >> (shiftCount & 0xF));
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)value >> shiftCount);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)value >> shiftCount);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T ShiftRightLogical(T value, int shiftCount)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((uint)(byte)(object)value >> (shiftCount & 7));
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits((double)(object)value) >>> shiftCount);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((ushort)(short)(object)value >>> (shiftCount & 0xF));
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)value >>> shiftCount);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)value >>> shiftCount);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)value >>> (shiftCount & 0x3F));
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)value >> (shiftCount & 0x3F));
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((byte)(sbyte)(object)value >>> (shiftCount & 7));
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits((float)(object)value) >>> shiftCount);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((uint)(ushort)(object)value >> (shiftCount & 0xF));
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)value >> shiftCount);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)value >> shiftCount);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Sqrt(T value)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)MathF.Sqrt((int)(byte)(object)value);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)Math.Sqrt((double)(object)value);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)MathF.Sqrt((short)(object)value);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)(int)Math.Sqrt((int)(object)value);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)(long)Math.Sqrt((long)(object)value);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)(nint)Math.Sqrt((nint)(object)value);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)(nuint)Math.Sqrt((nuint)(object)value);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)MathF.Sqrt((sbyte)(object)value);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)MathF.Sqrt((float)(object)value);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)MathF.Sqrt((int)(ushort)(object)value);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)(uint)Math.Sqrt((uint)(object)value);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)(ulong)Math.Sqrt((ulong)(object)value);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Subtract(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			return (T)(object)(byte)((byte)(object)left - (byte)(object)right);
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)((double)(object)left - (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			return (T)(object)(short)((short)(object)left - (short)(object)right);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)((int)(object)left - (int)(object)right);
		}
		if (typeof(T) == typeof(long))
		{
			return (T)(object)((long)(object)left - (long)(object)right);
		}
		if (typeof(T) == typeof(nint))
		{
			return (T)(object)((nint)(object)left - (nint)(object)right);
		}
		if (typeof(T) == typeof(nuint))
		{
			return (T)(object)((nuint)(object)left - (nuint)(object)right);
		}
		if (typeof(T) == typeof(sbyte))
		{
			return (T)(object)(sbyte)((sbyte)(object)left - (sbyte)(object)right);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)((float)(object)left - (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			return (T)(object)(ushort)((ushort)(object)left - (ushort)(object)right);
		}
		if (typeof(T) == typeof(uint))
		{
			return (T)(object)((uint)(object)left - (uint)(object)right);
		}
		if (typeof(T) == typeof(ulong))
		{
			return (T)(object)((ulong)(object)left - (ulong)(object)right);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T SubtractSaturate(T left, T right)
	{
		if (typeof(T) == typeof(byte))
		{
			byte b = (byte)(object)left;
			byte b2 = (byte)(object)right;
			byte b3 = (byte)(b - b2);
			if (b3 > b)
			{
				b3 = 0;
			}
			return (T)(object)b3;
		}
		if (typeof(T) == typeof(double))
		{
			return (T)(object)((double)(object)left - (double)(object)right);
		}
		if (typeof(T) == typeof(short))
		{
			short num = (short)(object)left;
			short num2 = (short)(object)right;
			short num3 = (short)(num - num2);
			if (((num3 ^ num) & (num ^ num2)) < 0)
			{
				num3 = ((num3 < 0) ? short.MaxValue : short.MinValue);
			}
			return (T)(object)num3;
		}
		if (typeof(T) == typeof(int))
		{
			int num4 = (int)(object)left;
			int num5 = (int)(object)right;
			int num6 = num4 - num5;
			if (((num6 ^ num4) & (num4 ^ num5)) < 0)
			{
				num6 = ((num6 < 0) ? int.MaxValue : int.MinValue);
			}
			return (T)(object)num6;
		}
		if (typeof(T) == typeof(long))
		{
			long num7 = (long)(object)left;
			long num8 = (long)(object)right;
			long num9 = num7 - num8;
			if (((num9 ^ num7) & (num7 ^ num8)) < 0)
			{
				num9 = ((num9 < 0) ? long.MaxValue : long.MinValue);
			}
			return (T)(object)num9;
		}
		if (typeof(T) == typeof(nint))
		{
			nint num10 = (nint)(object)left;
			nint num11 = (nint)(object)right;
			nint num12 = num10 - num11;
			if (((num12 ^ num10) & (num10 ^ num11)) < 0)
			{
				num12 = ((num12 < 0) ? IntPtr.MaxValue : IntPtr.MinValue);
			}
			return (T)(object)num12;
		}
		if (typeof(T) == typeof(nuint))
		{
			nuint num13 = (nuint)(object)left;
			nuint num14 = (nuint)(object)right;
			nuint num15 = num13 - num14;
			if (num15 > num13)
			{
				num15 = UIntPtr.MinValue;
			}
			return (T)(object)num15;
		}
		if (typeof(T) == typeof(sbyte))
		{
			sbyte b4 = (sbyte)(object)left;
			sbyte b5 = (sbyte)(object)right;
			sbyte b6 = (sbyte)(b4 - b5);
			if (((b6 ^ b4) & (b4 ^ b5)) < 0)
			{
				b6 = ((b6 < 0) ? sbyte.MaxValue : sbyte.MinValue);
			}
			return (T)(object)b6;
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)((float)(object)left - (float)(object)right);
		}
		if (typeof(T) == typeof(ushort))
		{
			ushort num16 = (ushort)(object)left;
			ushort num17 = (ushort)(object)right;
			ushort num18 = (ushort)(num16 - num17);
			if (num18 > num16)
			{
				num18 = 0;
			}
			return (T)(object)num18;
		}
		if (typeof(T) == typeof(uint))
		{
			uint num19 = (uint)(object)left;
			uint num20 = (uint)(object)right;
			uint num21 = num19 - num20;
			if (num21 > num19)
			{
				num21 = 0u;
			}
			return (T)(object)num21;
		}
		if (typeof(T) == typeof(ulong))
		{
			ulong num22 = (ulong)(object)left;
			ulong num23 = (ulong)(object)right;
			ulong num24 = num22 - num23;
			if (num24 > num22)
			{
				num24 = 0uL;
			}
			return (T)(object)num24;
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Truncate(T value)
	{
		if (typeof(T) == typeof(double))
		{
			return (T)(object)Math.Truncate((double)(object)value);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)MathF.Truncate((float)(object)value);
		}
		ThrowHelper.ThrowNotSupportedException(ExceptionResource.Arg_TypeNotSupported);
		return default(T);
	}
}
