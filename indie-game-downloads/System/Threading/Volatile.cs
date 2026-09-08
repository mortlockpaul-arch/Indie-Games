using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System.Threading;

public static class Volatile
{
	private struct VolatileBoolean
	{
		public volatile bool Value;
	}

	private struct VolatileByte
	{
		public volatile byte Value;
	}

	private struct VolatileInt16
	{
		public volatile short Value;
	}

	private struct VolatileInt32
	{
		public volatile int Value;
	}

	private struct VolatileIntPtr
	{
		public volatile nint Value;
	}

	private struct VolatileSByte
	{
		public volatile sbyte Value;
	}

	private struct VolatileSingle
	{
		public volatile float Value;
	}

	private struct VolatileUInt16
	{
		public volatile ushort Value;
	}

	private struct VolatileUInt32
	{
		public volatile uint Value;
	}

	private struct VolatileUIntPtr
	{
		public volatile nuint Value;
	}

	private struct VolatileObject
	{
		public volatile object Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static bool Read(ref readonly bool location)
	{
		return Unsafe.As<bool, VolatileBoolean>(ref Unsafe.AsRef(in location)).Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref bool location, bool value)
	{
		Unsafe.As<bool, VolatileBoolean>(ref location).Value = value;
	}

	[Intrinsic]
	[NonVersionable]
	public static byte Read(ref readonly byte location)
	{
		return Unsafe.As<byte, VolatileByte>(ref Unsafe.AsRef(in location)).Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref byte location, byte value)
	{
		Unsafe.As<byte, VolatileByte>(ref location).Value = value;
	}

	[Intrinsic]
	[NonVersionable]
	public static double Read(ref readonly double location)
	{
		return BitConverter.Int64BitsToDouble(Read(in Unsafe.As<double, long>(ref Unsafe.AsRef(in location))));
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref double location, double value)
	{
		Write(ref Unsafe.As<double, long>(ref location), BitConverter.DoubleToInt64Bits(value));
	}

	[Intrinsic]
	[NonVersionable]
	public static short Read(ref readonly short location)
	{
		return Unsafe.As<short, VolatileInt16>(ref Unsafe.AsRef(in location)).Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref short location, short value)
	{
		Unsafe.As<short, VolatileInt16>(ref location).Value = value;
	}

	[Intrinsic]
	[NonVersionable]
	public static int Read(ref readonly int location)
	{
		return Unsafe.As<int, VolatileInt32>(ref Unsafe.AsRef(in location)).Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref int location, int value)
	{
		Unsafe.As<int, VolatileInt32>(ref location).Value = value;
	}

	[Intrinsic]
	[NonVersionable]
	public static long Read(ref readonly long location)
	{
		return Unsafe.As<long, VolatileIntPtr>(ref Unsafe.AsRef(in location)).Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref long location, long value)
	{
		Unsafe.As<long, VolatileIntPtr>(ref location).Value = (nint)value;
	}

	[Intrinsic]
	[NonVersionable]
	public static nint Read(ref readonly nint location)
	{
		return Unsafe.As<nint, VolatileIntPtr>(ref Unsafe.AsRef(in location)).Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref nint location, nint value)
	{
		Unsafe.As<nint, VolatileIntPtr>(ref location).Value = value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static sbyte Read(ref readonly sbyte location)
	{
		return Unsafe.As<sbyte, VolatileSByte>(ref Unsafe.AsRef(in location)).Value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static void Write(ref sbyte location, sbyte value)
	{
		Unsafe.As<sbyte, VolatileSByte>(ref location).Value = value;
	}

	[Intrinsic]
	[NonVersionable]
	public static float Read(ref readonly float location)
	{
		return Unsafe.As<float, VolatileSingle>(ref Unsafe.AsRef(in location)).Value;
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write(ref float location, float value)
	{
		Unsafe.As<float, VolatileSingle>(ref location).Value = value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static ushort Read(ref readonly ushort location)
	{
		return Unsafe.As<ushort, VolatileUInt16>(ref Unsafe.AsRef(in location)).Value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static void Write(ref ushort location, ushort value)
	{
		Unsafe.As<ushort, VolatileUInt16>(ref location).Value = value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static uint Read(ref readonly uint location)
	{
		return Unsafe.As<uint, VolatileUInt32>(ref Unsafe.AsRef(in location)).Value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static void Write(ref uint location, uint value)
	{
		Unsafe.As<uint, VolatileUInt32>(ref location).Value = value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static ulong Read(ref readonly ulong location)
	{
		return (ulong)Read(in Unsafe.As<ulong, long>(ref Unsafe.AsRef(in location)));
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static void Write(ref ulong location, ulong value)
	{
		Write(ref Unsafe.As<ulong, long>(ref location), (long)value);
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static nuint Read(ref readonly nuint location)
	{
		return Unsafe.As<nuint, VolatileUIntPtr>(ref Unsafe.AsRef(in location)).Value;
	}

	[CLSCompliant(false)]
	[Intrinsic]
	[NonVersionable]
	public static void Write(ref nuint location, nuint value)
	{
		Unsafe.As<nuint, VolatileUIntPtr>(ref location).Value = value;
	}

	[Intrinsic]
	[NonVersionable]
	[return: NotNullIfNotNull("location")]
	public static T Read<T>([NotNullIfNotNull("location")] ref readonly T location) where T : class?
	{
		return Unsafe.As<T>(Unsafe.As<T, VolatileObject>(ref Unsafe.AsRef(in location)).Value);
	}

	[Intrinsic]
	[NonVersionable]
	public static void Write<T>([NotNullIfNotNull("value")] ref T location, T value) where T : class?
	{
		Unsafe.As<T, VolatileObject>(ref location).Value = value;
	}

	[Intrinsic]
	public static void ReadBarrier()
	{
		ReadBarrier();
	}

	[Intrinsic]
	public static void WriteBarrier()
	{
		WriteBarrier();
	}
}
