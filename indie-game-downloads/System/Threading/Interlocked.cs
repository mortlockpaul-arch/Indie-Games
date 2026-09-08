using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Threading;

public static class Interlocked
{
	public static int Increment(ref int location)
	{
		return Add(ref location, 1);
	}

	public static long Increment(ref long location)
	{
		return Add(ref location, 1L);
	}

	public static int Decrement(ref int location)
	{
		return Add(ref location, -1);
	}

	public static long Decrement(ref long location)
	{
		return Add(ref location, -1L);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Exchange(ref int location1, int value)
	{
		return Exchange(ref location1, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static long Exchange(ref long location1, long value)
	{
		return Exchange(ref location1, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[return: NotNullIfNotNull("location1")]
	public static object? Exchange([NotNullIfNotNull("value")] ref object? location1, object? value)
	{
		if (Unsafe.IsNullRef(in location1))
		{
			ThrowHelper.ThrowNullReferenceException();
		}
		return ExchangeObject(ref location1, value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: NotNullIfNotNull("location1")]
	private static extern object ExchangeObject([NotNullIfNotNull("value")] ref object location1, object value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CompareExchange(ref int location1, int value, int comparand)
	{
		return CompareExchange(ref location1, value, comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static long CompareExchange(ref long location1, long value, long comparand)
	{
		return CompareExchange(ref location1, value, comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[return: NotNullIfNotNull("location1")]
	public static object? CompareExchange(ref object? location1, object? value, object? comparand)
	{
		if (Unsafe.IsNullRef(in location1))
		{
			ThrowHelper.ThrowNullReferenceException();
		}
		return CompareExchangeObject(ref location1, value, comparand);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: NotNullIfNotNull("location1")]
	private static extern object CompareExchangeObject(ref object location1, object value, object comparand);

	public static int Add(ref int location1, int value)
	{
		return ExchangeAdd(ref location1, value) + value;
	}

	public static long Add(ref long location1, long value)
	{
		return ExchangeAdd(ref location1, value) + value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	private static int ExchangeAdd(ref int location1, int value)
	{
		return ExchangeAdd(ref location1, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	private static long ExchangeAdd(ref long location1, long value)
	{
		return ExchangeAdd(ref location1, value);
	}

	public static long Read(ref readonly long location)
	{
		return CompareExchange(ref Unsafe.AsRef(in location), 0L, 0L);
	}

	[DllImport("QCall", EntryPoint = "Interlocked_MemoryBarrierProcessWide", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Interlocked_MemoryBarrierProcessWide")]
	private static extern void _MemoryBarrierProcessWide();

	public static void MemoryBarrierProcessWide()
	{
		_MemoryBarrierProcessWide();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint Increment(ref uint location)
	{
		return Add(ref location, 1u);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong Increment(ref ulong location)
	{
		return Add(ref location, 1uL);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint Decrement(ref uint location)
	{
		return (uint)Add(ref Unsafe.As<uint, int>(ref location), -1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong Decrement(ref ulong location)
	{
		return (ulong)Add(ref Unsafe.As<ulong, long>(ref location), -1L);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static sbyte Exchange(ref sbyte location1, sbyte value)
	{
		return (sbyte)Exchange(ref Unsafe.As<sbyte, byte>(ref location1), (byte)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static short Exchange(ref short location1, short value)
	{
		return (short)Exchange(ref Unsafe.As<short, ushort>(ref location1), (ushort)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static byte Exchange(ref byte location1, byte value)
	{
		return Exchange(ref location1, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static ushort Exchange(ref ushort location1, ushort value)
	{
		return Exchange(ref location1, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static uint Exchange(ref uint location1, uint value)
	{
		return (uint)Exchange(ref Unsafe.As<uint, int>(ref location1), (int)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static ulong Exchange(ref ulong location1, ulong value)
	{
		return (ulong)Exchange(ref Unsafe.As<ulong, long>(ref location1), (long)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Exchange(ref float location1, float value)
	{
		return Unsafe.BitCast<int, float>(Exchange(ref Unsafe.As<float, int>(ref location1), Unsafe.BitCast<float, int>(value)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Exchange(ref double location1, double value)
	{
		return Unsafe.BitCast<long, double>(Exchange(ref Unsafe.As<double, long>(ref location1), Unsafe.BitCast<double, long>(value)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static nint Exchange(ref nint location1, nint value)
	{
		return (nint)Exchange(ref Unsafe.As<nint, long>(ref location1), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static nuint Exchange(ref nuint location1, nuint value)
	{
		return (nuint)Exchange(ref Unsafe.As<nuint, long>(ref location1), (long)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[return: NotNullIfNotNull("location1")]
	public static T Exchange<T>([NotNullIfNotNull("value")] ref T location1, T value)
	{
		if (!typeof(T).IsValueType)
		{
			object source = Exchange(ref Unsafe.As<T, object>(ref location1), value);
			return Unsafe.As<object, T>(ref source);
		}
		if (!typeof(T).IsPrimitive && !typeof(T).IsEnum)
		{
			throw new NotSupportedException(SR.NotSupported_ReferenceEnumOrPrimitiveTypeRequired);
		}
		if (Unsafe.SizeOf<T>() == 1)
		{
			return Unsafe.BitCast<byte, T>(Exchange(ref Unsafe.As<T, byte>(ref location1), Unsafe.BitCast<T, byte>(value)));
		}
		if (Unsafe.SizeOf<T>() == 2)
		{
			return Unsafe.BitCast<ushort, T>(Exchange(ref Unsafe.As<T, ushort>(ref location1), Unsafe.BitCast<T, ushort>(value)));
		}
		if (Unsafe.SizeOf<T>() == 4)
		{
			return Unsafe.BitCast<int, T>(Exchange(ref Unsafe.As<T, int>(ref location1), Unsafe.BitCast<T, int>(value)));
		}
		return Unsafe.BitCast<long, T>(Exchange(ref Unsafe.As<T, long>(ref location1), Unsafe.BitCast<T, long>(value)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static sbyte CompareExchange(ref sbyte location1, sbyte value, sbyte comparand)
	{
		return (sbyte)CompareExchange(ref Unsafe.As<sbyte, byte>(ref location1), (byte)value, (byte)comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static short CompareExchange(ref short location1, short value, short comparand)
	{
		return (short)CompareExchange(ref Unsafe.As<short, ushort>(ref location1), (ushort)value, (ushort)comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static byte CompareExchange(ref byte location1, byte value, byte comparand)
	{
		return CompareExchange(ref location1, value, comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static ushort CompareExchange(ref ushort location1, ushort value, ushort comparand)
	{
		return CompareExchange(ref location1, value, comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static uint CompareExchange(ref uint location1, uint value, uint comparand)
	{
		return (uint)CompareExchange(ref Unsafe.As<uint, int>(ref location1), (int)value, (int)comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static ulong CompareExchange(ref ulong location1, ulong value, ulong comparand)
	{
		return (ulong)CompareExchange(ref Unsafe.As<ulong, long>(ref location1), (long)value, (long)comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CompareExchange(ref float location1, float value, float comparand)
	{
		return Unsafe.BitCast<int, float>(CompareExchange(ref Unsafe.As<float, int>(ref location1), Unsafe.BitCast<float, int>(value), Unsafe.BitCast<float, int>(comparand)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double CompareExchange(ref double location1, double value, double comparand)
	{
		return Unsafe.BitCast<long, double>(CompareExchange(ref Unsafe.As<double, long>(ref location1), Unsafe.BitCast<double, long>(value), Unsafe.BitCast<double, long>(comparand)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static nint CompareExchange(ref nint location1, nint value, nint comparand)
	{
		return (nint)CompareExchange(ref Unsafe.As<nint, long>(ref location1), value, comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static nuint CompareExchange(ref nuint location1, nuint value, nuint comparand)
	{
		return (nuint)CompareExchange(ref Unsafe.As<nuint, long>(ref location1), (long)value, (long)comparand);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[return: NotNullIfNotNull("location1")]
	public static T CompareExchange<T>(ref T location1, T value, T comparand)
	{
		if (!typeof(T).IsValueType)
		{
			object source = CompareExchange(ref Unsafe.As<T, object>(ref location1), value, comparand);
			return Unsafe.As<object, T>(ref source);
		}
		if (!typeof(T).IsPrimitive && !typeof(T).IsEnum)
		{
			throw new NotSupportedException(SR.NotSupported_ReferenceEnumOrPrimitiveTypeRequired);
		}
		if (Unsafe.SizeOf<T>() == 1)
		{
			return Unsafe.BitCast<byte, T>(CompareExchange(ref Unsafe.As<T, byte>(ref location1), Unsafe.BitCast<T, byte>(value), Unsafe.BitCast<T, byte>(comparand)));
		}
		if (Unsafe.SizeOf<T>() == 2)
		{
			return Unsafe.BitCast<ushort, T>(CompareExchange(ref Unsafe.As<T, ushort>(ref location1), Unsafe.BitCast<T, ushort>(value), Unsafe.BitCast<T, ushort>(comparand)));
		}
		if (Unsafe.SizeOf<T>() == 4)
		{
			return Unsafe.BitCast<int, T>(CompareExchange(ref Unsafe.As<T, int>(ref location1), Unsafe.BitCast<T, int>(value), Unsafe.BitCast<T, int>(comparand)));
		}
		return Unsafe.BitCast<long, T>(CompareExchange(ref Unsafe.As<T, long>(ref location1), Unsafe.BitCast<T, long>(value), Unsafe.BitCast<T, long>(comparand)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint Add(ref uint location1, uint value)
	{
		return (uint)Add(ref Unsafe.As<uint, int>(ref location1), (int)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong Add(ref ulong location1, ulong value)
	{
		return (ulong)Add(ref Unsafe.As<ulong, long>(ref location1), (long)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong Read(ref readonly ulong location)
	{
		return CompareExchange(ref Unsafe.AsRef(in location), 0uL, 0uL);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int And(ref int location1, int value)
	{
		int num = location1;
		int num2;
		while (true)
		{
			int value2 = num & value;
			num2 = CompareExchange(ref location1, value2, num);
			if (num2 == num)
			{
				break;
			}
			num = num2;
		}
		return num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint And(ref uint location1, uint value)
	{
		return (uint)And(ref Unsafe.As<uint, int>(ref location1), (int)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static long And(ref long location1, long value)
	{
		long num = location1;
		long num2;
		while (true)
		{
			long value2 = num & value;
			num2 = CompareExchange(ref location1, value2, num);
			if (num2 == num)
			{
				break;
			}
			num = num2;
		}
		return num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong And(ref ulong location1, ulong value)
	{
		return (ulong)And(ref Unsafe.As<ulong, long>(ref location1), (long)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Or(ref int location1, int value)
	{
		int num = location1;
		int num2;
		while (true)
		{
			int value2 = num | value;
			num2 = CompareExchange(ref location1, value2, num);
			if (num2 == num)
			{
				break;
			}
			num = num2;
		}
		return num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint Or(ref uint location1, uint value)
	{
		return (uint)Or(ref Unsafe.As<uint, int>(ref location1), (int)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static long Or(ref long location1, long value)
	{
		long num = location1;
		long num2;
		while (true)
		{
			long value2 = num | value;
			num2 = CompareExchange(ref location1, value2, num);
			if (num2 == num)
			{
				break;
			}
			num = num2;
		}
		return num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static ulong Or(ref ulong location1, ulong value)
	{
		return (ulong)Or(ref Unsafe.As<ulong, long>(ref location1), (long)value);
	}

	[Intrinsic]
	public static void MemoryBarrier()
	{
		MemoryBarrier();
	}
}
