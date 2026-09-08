using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.Intrinsics;

public static class Vector64
{
	public static bool IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> Abs<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Abs(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<T> Add<T>(Vector64<T> left, Vector64<T> right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> AddSaturate<T>(Vector64<T> left, Vector64<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left + right;
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.AddSaturate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All<T>(Vector64<T> vector, T value)
	{
		return vector == Create(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return All(vector.AsInt32(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return All(vector.AsInt64(), -1L);
		}
		return All(vector, Scalar<T>.AllBitsSet);
	}

	[Intrinsic]
	public static Vector64<T> AndNot<T>(Vector64<T> left, Vector64<T> right)
	{
		return left & ~right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any<T>(Vector64<T> vector, T value)
	{
		return EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Any(vector.AsInt32(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return Any(vector.AsInt64(), -1L);
		}
		return Any(vector, Scalar<T>.AllBitsSet);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<TTo> As<TFrom, TTo>(this Vector64<TFrom> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<TFrom>();
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<TTo>();
		return Unsafe.BitCast<Vector64<TFrom>, Vector64<TTo>>(vector);
	}

	[Intrinsic]
	public static Vector64<byte> AsByte<T>(this Vector64<T> vector)
	{
		return vector.As<T, byte>();
	}

	[Intrinsic]
	public static Vector64<double> AsDouble<T>(this Vector64<T> vector)
	{
		return vector.As<T, double>();
	}

	[Intrinsic]
	public static Vector64<short> AsInt16<T>(this Vector64<T> vector)
	{
		return vector.As<T, short>();
	}

	[Intrinsic]
	public static Vector64<int> AsInt32<T>(this Vector64<T> vector)
	{
		return vector.As<T, int>();
	}

	[Intrinsic]
	public static Vector64<long> AsInt64<T>(this Vector64<T> vector)
	{
		return vector.As<T, long>();
	}

	[Intrinsic]
	public static Vector64<nint> AsNInt<T>(this Vector64<T> vector)
	{
		return vector.As<T, nint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<nuint> AsNUInt<T>(this Vector64<T> vector)
	{
		return vector.As<T, nuint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> AsSByte<T>(this Vector64<T> vector)
	{
		return vector.As<T, sbyte>();
	}

	[Intrinsic]
	public static Vector64<float> AsSingle<T>(this Vector64<T> vector)
	{
		return vector.As<T, float>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> AsUInt16<T>(this Vector64<T> vector)
	{
		return vector.As<T, ushort>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> AsUInt32<T>(this Vector64<T> vector)
	{
		return vector.As<T, uint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> AsUInt64<T>(this Vector64<T> vector)
	{
		return vector.As<T, ulong>();
	}

	[Intrinsic]
	public static Vector64<T> BitwiseAnd<T>(Vector64<T> left, Vector64<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector64<T> BitwiseOr<T>(Vector64<T> left, Vector64<T> right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector64<T> Ceiling<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Ceiling(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<float> Ceiling(Vector64<float> vector)
	{
		return Vector64.Ceiling<float>(vector);
	}

	[Intrinsic]
	public static Vector64<double> Ceiling(Vector64<double> vector)
	{
		return Vector64.Ceiling<double>(vector);
	}

	[Intrinsic]
	public static Vector64<T> Clamp<T>(Vector64<T> value, Vector64<T> min, Vector64<T> max)
	{
		return Min(Max(value, min), max);
	}

	[Intrinsic]
	public static Vector64<T> ClampNative<T>(Vector64<T> value, Vector64<T> min, Vector64<T> max)
	{
		return MinNative(MaxNative(value, min), max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> ConditionalSelect<T>(Vector64<T> condition, Vector64<T> left, Vector64<T> right)
	{
		return (left & condition) | AndNot(right, condition);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> ConvertToDouble(Vector64<long> vector)
	{
		Unsafe.SkipInit<Vector64<double>>(out var value);
		for (int i = 0; i < Vector64<double>.Count; i++)
		{
			double value2 = GetElementUnsafe(in vector, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<double> ConvertToDouble(Vector64<ulong> vector)
	{
		Unsafe.SkipInit<Vector64<double>>(out var value);
		for (int i = 0; i < Vector64<double>.Count; i++)
		{
			double value2 = GetElementUnsafe(in vector, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<int> ConvertToInt32(Vector64<float> vector)
	{
		Unsafe.SkipInit<Vector64<int>>(out var value);
		for (int i = 0; i < Vector64<int>.Count; i++)
		{
			int value2 = float.ConvertToInteger<int>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<int> ConvertToInt32Native(Vector64<float> vector)
	{
		Unsafe.SkipInit<Vector64<int>>(out var value);
		for (int i = 0; i < Vector64<int>.Count; i++)
		{
			int value2 = float.ConvertToIntegerNative<int>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<long> ConvertToInt64(Vector64<double> vector)
	{
		Unsafe.SkipInit<Vector64<long>>(out var value);
		for (int i = 0; i < Vector64<long>.Count; i++)
		{
			long value2 = double.ConvertToInteger<long>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<long> ConvertToInt64Native(Vector64<double> vector)
	{
		Unsafe.SkipInit<Vector64<long>>(out var value);
		for (int i = 0; i < Vector64<long>.Count; i++)
		{
			long value2 = double.ConvertToIntegerNative<long>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> ConvertToSingle(Vector64<int> vector)
	{
		Unsafe.SkipInit<Vector64<float>>(out var value);
		for (int i = 0; i < Vector64<float>.Count; i++)
		{
			float value2 = GetElementUnsafe(in vector, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<float> ConvertToSingle(Vector64<uint> vector)
	{
		Unsafe.SkipInit<Vector64<float>>(out var value);
		for (int i = 0; i < Vector64<float>.Count; i++)
		{
			float value2 = GetElementUnsafe(in vector, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> ConvertToUInt32(Vector64<float> vector)
	{
		Unsafe.SkipInit<Vector64<uint>>(out var value);
		for (int i = 0; i < Vector64<uint>.Count; i++)
		{
			uint value2 = float.ConvertToInteger<uint>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> ConvertToUInt32Native(Vector64<float> vector)
	{
		Unsafe.SkipInit<Vector64<uint>>(out var value);
		for (int i = 0; i < Vector64<uint>.Count; i++)
		{
			uint value2 = float.ConvertToIntegerNative<uint>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> ConvertToUInt64(Vector64<double> vector)
	{
		Unsafe.SkipInit<Vector64<ulong>>(out var value);
		for (int i = 0; i < Vector64<ulong>.Count; i++)
		{
			ulong value2 = double.ConvertToInteger<ulong>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> ConvertToUInt64Native(Vector64<double> vector)
	{
		Unsafe.SkipInit<Vector64<ulong>>(out var value);
		for (int i = 0; i < Vector64<ulong>.Count; i++)
		{
			ulong value2 = double.ConvertToIntegerNative<ulong>(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> CopySign<T>(Vector64<T> value, Vector64<T> sign)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return value;
		}
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value2);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value3 = Scalar<T>.CopySign(GetElementUnsafe(in value, i), GetElementUnsafe(in sign, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector64<T> vector, T[] destination)
	{
		if (destination.Length < Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[0]), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector64<T> vector, T[] destination, int startIndex)
	{
		if ((uint)startIndex >= (uint)destination.Length)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
		}
		if (destination.Length - startIndex < Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[startIndex]), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector64<T> vector, Span<T> destination)
	{
		if (destination.Length < Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
	}

	internal static Vector64<T> Cos<T>(Vector64<T> vector) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.Cos(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<double> Cos(Vector64<double> vector)
	{
		if (false)
		{
		}
		return Vector64.Cos<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<float> Cos(Vector64<float> vector)
	{
		if (false)
		{
		}
		return Vector64.Cos<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count<T>(Vector64<T> vector, T value)
	{
		return BitOperations.PopCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Count(vector.AsInt32(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return Count(vector.AsInt64(), -1L);
		}
		return Count(vector, Scalar<T>.AllBitsSet);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> Create<T>(T value)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value2);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			SetElementUnsafe(in value2, i, value);
		}
		return value2;
	}

	[Intrinsic]
	public static Vector64<byte> Create(byte value)
	{
		return Vector64.Create<byte>(value);
	}

	[Intrinsic]
	public static Vector64<double> Create(double value)
	{
		return Vector64.Create<double>(value);
	}

	[Intrinsic]
	public static Vector64<short> Create(short value)
	{
		return Vector64.Create<short>(value);
	}

	[Intrinsic]
	public static Vector64<int> Create(int value)
	{
		return Vector64.Create<int>(value);
	}

	[Intrinsic]
	public static Vector64<long> Create(long value)
	{
		return Vector64.Create<long>(value);
	}

	[Intrinsic]
	public static Vector64<nint> Create(nint value)
	{
		return Vector64.Create<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<nuint> Create(nuint value)
	{
		return Vector64.Create<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> Create(sbyte value)
	{
		return Vector64.Create<sbyte>(value);
	}

	[Intrinsic]
	public static Vector64<float> Create(float value)
	{
		return Vector64.Create<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> Create(ushort value)
	{
		return Vector64.Create<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> Create(uint value)
	{
		return Vector64.Create<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> Create(ulong value)
	{
		return Vector64.Create<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<T> Create<T>(T[] values)
	{
		if (values.Length < Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector64<T>>(in Unsafe.As<T, byte>(ref values[0]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<T> Create<T>(T[] values, int index)
	{
		if (index < 0 || values.Length - index < Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector64<T>>(in Unsafe.As<T, byte>(ref values[index]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<T> Create<T>(ReadOnlySpan<T> values)
	{
		if (values.Length < Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return Unsafe.ReadUnaligned<Vector64<T>>(in Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<byte> Create(byte e0, byte e1, byte e2, byte e3, byte e4, byte e5, byte e6, byte e7)
	{
		Unsafe.SkipInit<Vector64<byte>>(out var value);
		value.SetElementUnsafe(0, e0);
		value.SetElementUnsafe(1, e1);
		value.SetElementUnsafe(2, e2);
		value.SetElementUnsafe(3, e3);
		value.SetElementUnsafe(4, e4);
		value.SetElementUnsafe(5, e5);
		value.SetElementUnsafe(6, e6);
		value.SetElementUnsafe(7, e7);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<short> Create(short e0, short e1, short e2, short e3)
	{
		Unsafe.SkipInit<Vector64<short>>(out var value);
		value.SetElementUnsafe(0, e0);
		value.SetElementUnsafe(1, e1);
		value.SetElementUnsafe(2, e2);
		value.SetElementUnsafe(3, e3);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<int> Create(int e0, int e1)
	{
		Unsafe.SkipInit<Vector64<int>>(out var value);
		value.SetElementUnsafe(0, e0);
		value.SetElementUnsafe(1, e1);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> Create(sbyte e0, sbyte e1, sbyte e2, sbyte e3, sbyte e4, sbyte e5, sbyte e6, sbyte e7)
	{
		Unsafe.SkipInit<Vector64<sbyte>>(out var value);
		value.SetElementUnsafe(0, e0);
		value.SetElementUnsafe(1, e1);
		value.SetElementUnsafe(2, e2);
		value.SetElementUnsafe(3, e3);
		value.SetElementUnsafe(4, e4);
		value.SetElementUnsafe(5, e5);
		value.SetElementUnsafe(6, e6);
		value.SetElementUnsafe(7, e7);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> Create(float e0, float e1)
	{
		Unsafe.SkipInit<Vector64<float>>(out var value);
		value.SetElementUnsafe(0, e0);
		value.SetElementUnsafe(1, e1);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> Create(ushort e0, ushort e1, ushort e2, ushort e3)
	{
		Unsafe.SkipInit<Vector64<ushort>>(out var value);
		value.SetElementUnsafe(0, e0);
		value.SetElementUnsafe(1, e1);
		value.SetElementUnsafe(2, e2);
		value.SetElementUnsafe(3, e3);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> Create(uint e0, uint e1)
	{
		Unsafe.SkipInit<Vector64<uint>>(out var value);
		value.SetElementUnsafe(0, e0);
		value.SetElementUnsafe(1, e1);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> CreateScalar<T>(T value)
	{
		Vector64<T> vector = Vector64<T>.Zero;
		SetElementUnsafe(in vector, 0, value);
		return vector;
	}

	[Intrinsic]
	public static Vector64<byte> CreateScalar(byte value)
	{
		return Vector64.CreateScalar<byte>(value);
	}

	[Intrinsic]
	public static Vector64<double> CreateScalar(double value)
	{
		return Vector64.CreateScalar<double>(value);
	}

	[Intrinsic]
	public static Vector64<short> CreateScalar(short value)
	{
		return Vector64.CreateScalar<short>(value);
	}

	[Intrinsic]
	public static Vector64<int> CreateScalar(int value)
	{
		return Vector64.CreateScalar<int>(value);
	}

	[Intrinsic]
	public static Vector64<long> CreateScalar(long value)
	{
		return Vector64.CreateScalar<long>(value);
	}

	[Intrinsic]
	public static Vector64<nint> CreateScalar(nint value)
	{
		return Vector64.CreateScalar<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<nuint> CreateScalar(nuint value)
	{
		return Vector64.CreateScalar<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> CreateScalar(sbyte value)
	{
		return Vector64.CreateScalar<sbyte>(value);
	}

	[Intrinsic]
	public static Vector64<float> CreateScalar(float value)
	{
		return Vector64.CreateScalar<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> CreateScalar(ushort value)
	{
		return Vector64.CreateScalar<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> CreateScalar(uint value)
	{
		return Vector64.CreateScalar<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> CreateScalar(ulong value)
	{
		return Vector64.CreateScalar<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> CreateScalarUnsafe<T>(T value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Unsafe.SkipInit<Vector64<T>>(out var value2);
		SetElementUnsafe(in value2, 0, value);
		return value2;
	}

	[Intrinsic]
	public static Vector64<byte> CreateScalarUnsafe(byte value)
	{
		return Vector64.CreateScalarUnsafe<byte>(value);
	}

	[Intrinsic]
	public static Vector64<double> CreateScalarUnsafe(double value)
	{
		return Vector64.CreateScalarUnsafe<double>(value);
	}

	[Intrinsic]
	public static Vector64<short> CreateScalarUnsafe(short value)
	{
		return Vector64.CreateScalarUnsafe<short>(value);
	}

	[Intrinsic]
	public static Vector64<int> CreateScalarUnsafe(int value)
	{
		return Vector64.CreateScalarUnsafe<int>(value);
	}

	[Intrinsic]
	public static Vector64<long> CreateScalarUnsafe(long value)
	{
		return Vector64.CreateScalarUnsafe<long>(value);
	}

	[Intrinsic]
	public static Vector64<nint> CreateScalarUnsafe(nint value)
	{
		return Vector64.CreateScalarUnsafe<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<nuint> CreateScalarUnsafe(nuint value)
	{
		return Vector64.CreateScalarUnsafe<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> CreateScalarUnsafe(sbyte value)
	{
		return Vector64.CreateScalarUnsafe<sbyte>(value);
	}

	[Intrinsic]
	public static Vector64<float> CreateScalarUnsafe(float value)
	{
		return Vector64.CreateScalarUnsafe<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> CreateScalarUnsafe(ushort value)
	{
		return Vector64.CreateScalarUnsafe<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> CreateScalarUnsafe(uint value)
	{
		return Vector64.CreateScalarUnsafe<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> CreateScalarUnsafe(ulong value)
	{
		return Vector64.CreateScalarUnsafe<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> CreateSequence<T>(T start, T step)
	{
		return Vector64<T>.Indices * step + Create(start);
	}

	internal static Vector64<T> DegreesToRadians<T>(Vector64<T> degrees) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.DegreesToRadians(GetElementUnsafe(in degrees, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> DegreesToRadians(Vector64<double> degrees)
	{
		if (false)
		{
		}
		return Vector64.DegreesToRadians<double>(degrees);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> DegreesToRadians(Vector64<float> degrees)
	{
		if (false)
		{
		}
		return Vector64.DegreesToRadians<float>(degrees);
	}

	[Intrinsic]
	public static Vector64<T> Divide<T>(Vector64<T> left, Vector64<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector64<T> Divide<T>(Vector64<T> left, T right)
	{
		return left / right;
	}

	[Intrinsic]
	public static T Dot<T>(Vector64<T> left, Vector64<T> right)
	{
		return Sum(left * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> Equals<T>(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = (Scalar<T>.Equals(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static bool EqualsAll<T>(Vector64<T> left, Vector64<T> right)
	{
		return left == right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (Scalar<T>.Equals(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return true;
			}
		}
		return false;
	}

	internal static Vector64<T> Exp<T>(Vector64<T> vector) where T : IExponentialFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.Exp(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<double> Exp(Vector64<double> vector)
	{
		if (false)
		{
		}
		return Vector64.Exp<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<float> Exp(Vector64<float> vector)
	{
		if (false)
		{
		}
		return Vector64.Exp<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static uint ExtractMostSignificantBits<T>(this Vector64<T> vector)
	{
		uint num = 0u;
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			uint num2 = Scalar<T>.ExtractMostSignificantBit(GetElementUnsafe(in vector, i));
			num |= num2 << i;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector64<T> Floor<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Floor(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<float> Floor(Vector64<float> vector)
	{
		return Vector64.Floor<float>(vector);
	}

	[Intrinsic]
	public static Vector64<double> Floor(Vector64<double> vector)
	{
		return Vector64.Floor<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> FusedMultiplyAdd(Vector64<double> left, Vector64<double> right, Vector64<double> addend)
	{
		Unsafe.SkipInit<Vector64<double>>(out var value);
		for (int i = 0; i < Vector64<double>.Count; i++)
		{
			double value2 = double.FusedMultiplyAdd(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> FusedMultiplyAdd(Vector64<float> left, Vector64<float> right, Vector64<float> addend)
	{
		Unsafe.SkipInit<Vector64<float>>(out var value);
		for (int i = 0; i < Vector64<float>.Count; i++)
		{
			float value2 = float.FusedMultiplyAdd(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T GetElement<T>(this Vector64<T> vector, int index)
	{
		if ((uint)index >= (uint)Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return GetElementUnsafe(in vector, index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> GreaterThan<T>(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = (Scalar<T>.GreaterThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (!Scalar<T>.GreaterThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAny<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (Scalar<T>.GreaterThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> GreaterThanOrEqual<T>(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = (Scalar<T>.GreaterThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (!Scalar<T>.GreaterThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAny<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (Scalar<T>.GreaterThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return true;
			}
		}
		return false;
	}

	internal static Vector64<T> Hypot<T>(Vector64<T> x, Vector64<T> y) where T : IRootFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.Hypot(GetElementUnsafe(in x, i), GetElementUnsafe(in y, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<double> Hypot(Vector64<double> x, Vector64<double> y)
	{
		if (false)
		{
		}
		return Vector64.Hypot<double>(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<float> Hypot(Vector64<float> x, Vector64<float> y)
	{
		if (false)
		{
		}
		return Vector64.Hypot<float>(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf<T>(Vector64<T> vector, T value)
	{
		int num = BitOperations.TrailingZeroCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
		if (num == 32)
		{
			return -1;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOfWhereAllBitsSet<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return IndexOf(vector.AsInt32(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return IndexOf(vector.AsInt64(), -1L);
		}
		return IndexOf(vector, Scalar<T>.AllBitsSet);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsEvenInteger<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsEvenIntegerSingle<Vector64<float>, Vector64<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsEvenIntegerDouble<Vector64<double>, Vector64<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return IsZero(vector & Vector64<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsFinite<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return ~IsZero(AndNot(Vector64.Create<uint>(2139095040u), vector.AsUInt32())).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return ~IsZero(AndNot(Vector64.Create<ulong>(9218868437227405312uL), vector.AsUInt64())).As<ulong, T>();
		}
		return Vector64<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsInfinity<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsPositiveInfinity(Abs(vector));
		}
		return Vector64<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsInteger<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsFinite(vector) & Equals(vector, Truncate(vector));
		}
		return Vector64<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsNaN<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return ~Equals(vector, vector);
		}
		return Vector64<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsNegative<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector64<T>.Zero;
		}
		if (typeof(T) == typeof(float))
		{
			return LessThan(vector.AsInt32(), Vector64<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(vector.AsInt64(), Vector64<long>.Zero).As<long, T>();
		}
		return LessThan(vector, Vector64<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsNegativeInfinity<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.NegativeInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.NegativeInfinity).As<double, T>());
		}
		return Vector64<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsNormal<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector64.Create<uint>(8388608u), Vector64.Create<uint>(2130706432u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector64.Create<ulong>(4503599627370496uL), Vector64.Create<ulong>(9214364837600034816uL)).As<ulong, T>();
		}
		return ~IsZero(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsOddInteger<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsOddIntegerSingle<Vector64<float>, Vector64<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsOddIntegerDouble<Vector64<double>, Vector64<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return ~IsZero(vector & Vector64<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsPositive<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector64<T>.AllBitsSet;
		}
		if (typeof(T) == typeof(float))
		{
			return GreaterThanOrEqual(vector.AsInt32(), Vector64<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return GreaterThanOrEqual(vector.AsInt64(), Vector64<long>.Zero).As<long, T>();
		}
		return GreaterThanOrEqual(vector, Vector64<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsPositiveInfinity<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.PositiveInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.PositiveInfinity).As<double, T>());
		}
		return Vector64<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsSubnormal<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector64<uint>.One, Vector64.Create<uint>(8388607u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector64<ulong>.One, Vector64.Create<ulong>(4503599627370495uL)).As<ulong, T>();
		}
		return Vector64<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> IsZero<T>(Vector64<T> vector)
	{
		return Equals(vector, Vector64<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf<T>(Vector64<T> vector, T value)
	{
		return 31 - BitOperations.LeadingZeroCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LastIndexOf(vector.AsInt32(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return LastIndexOf(vector.AsInt64(), -1L);
		}
		return LastIndexOf(vector, Scalar<T>.AllBitsSet);
	}

	internal static Vector64<T> Lerp<T>(Vector64<T> x, Vector64<T> y, Vector64<T> amount) where T : IFloatingPointIeee754<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.Lerp(GetElementUnsafe(in x, i), GetElementUnsafe(in y, i), GetElementUnsafe(in amount, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> Lerp(Vector64<double> x, Vector64<double> y, Vector64<double> amount)
	{
		if (false)
		{
		}
		return Vector64.Lerp<double>(x, y, amount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> Lerp(Vector64<float> x, Vector64<float> y, Vector64<float> amount)
	{
		if (false)
		{
		}
		return Vector64.Lerp<float>(x, y, amount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> LessThan<T>(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = (Scalar<T>.LessThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (!Scalar<T>.LessThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAny<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (Scalar<T>.LessThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> LessThanOrEqual<T>(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = (Scalar<T>.LessThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (!Scalar<T>.LessThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAny<T>(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			if (Scalar<T>.LessThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return true;
			}
		}
		return false;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector64<T> Load<T>(T* source)
	{
		return LoadUnsafe(in *source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector64<T> LoadAligned<T>(T* source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		if ((nuint)source % (nuint)8u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return *(Vector64<T>*)source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector64<T> LoadAlignedNonTemporal<T>(T* source)
	{
		return LoadAligned(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> LoadUnsafe<T>(ref readonly T source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		return Unsafe.ReadUnaligned<Vector64<T>>(in Unsafe.As<T, byte>(ref Unsafe.AsRef(in source)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<T> LoadUnsafe<T>(ref readonly T source, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		return Unsafe.ReadUnaligned<Vector64<T>>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)));
	}

	internal static Vector64<T> Log<T>(Vector64<T> vector) where T : ILogarithmicFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.Log(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<double> Log(Vector64<double> vector)
	{
		if (false)
		{
		}
		return Vector64.Log<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<float> Log(Vector64<float> vector)
	{
		if (false)
		{
		}
		return Vector64.Log<float>(vector);
	}

	internal static Vector64<T> Log2<T>(Vector64<T> vector) where T : ILogarithmicFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.Log2(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<double> Log2(Vector64<double> vector)
	{
		if (false)
		{
		}
		return Vector64.Log2<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<float> Log2(Vector64<float> vector)
	{
		if (false)
		{
		}
		return Vector64.Log2<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> Max<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Max(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MaxMagnitude<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.MaxMagnitude(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MaxMagnitudeNumber<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.MaxMagnitudeNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MaxNative<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = (Scalar<T>.GreaterThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? GetElementUnsafe(in left, i) : GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MaxNumber<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.MaxNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> Min<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Min(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MinMagnitude<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.MinMagnitude(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MinMagnitudeNumber<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.MinMagnitudeNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MinNative<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = (Scalar<T>.LessThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? GetElementUnsafe(in left, i) : GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> MinNumber<T>(Vector64<T> left, Vector64<T> right)
	{
		if (false)
		{
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.MinNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<T> Multiply<T>(Vector64<T> left, Vector64<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector64<T> Multiply<T>(Vector64<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector64<T> Multiply<T>(T left, Vector64<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector64<T> MultiplyAddEstimate<T>(Vector64<T> left, Vector64<T> right, Vector64<T> addend)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.MultiplyAddEstimate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> MultiplyAddEstimate(Vector64<double> left, Vector64<double> right, Vector64<double> addend)
	{
		Unsafe.SkipInit<Vector64<double>>(out var value);
		for (int i = 0; i < Vector64<double>.Count; i++)
		{
			double value2 = double.MultiplyAddEstimate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> MultiplyAddEstimate(Vector64<float> left, Vector64<float> right, Vector64<float> addend)
	{
		Unsafe.SkipInit<Vector64<float>>(out var value);
		for (int i = 0; i < Vector64<float>.Count; i++)
		{
			float value2 = float.MultiplyAddEstimate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector64<TResult> Narrow<TSource, TResult>(Vector64<TSource> lower, Vector64<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector64<TResult>>(out var value);
		for (int i = 0; i < Vector64<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateTruncating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector64<TSource>.Count; j < Vector64<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateTruncating(GetElementUnsafe(in upper, j - Vector64<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> Narrow(Vector64<double> lower, Vector64<double> upper)
	{
		return Narrow<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> Narrow(Vector64<short> lower, Vector64<short> upper)
	{
		return Narrow<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<short> Narrow(Vector64<int> lower, Vector64<int> upper)
	{
		return Narrow<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<int> Narrow(Vector64<long> lower, Vector64<long> upper)
	{
		return Narrow<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<byte> Narrow(Vector64<ushort> lower, Vector64<ushort> upper)
	{
		return Narrow<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> Narrow(Vector64<uint> lower, Vector64<uint> upper)
	{
		return Narrow<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> Narrow(Vector64<ulong> lower, Vector64<ulong> upper)
	{
		return Narrow<ulong, uint>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector64<TResult> NarrowWithSaturation<TSource, TResult>(Vector64<TSource> lower, Vector64<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector64<TResult>>(out var value);
		for (int i = 0; i < Vector64<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateSaturating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector64<TSource>.Count; j < Vector64<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateSaturating(GetElementUnsafe(in upper, j - Vector64<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> NarrowWithSaturation(Vector64<double> lower, Vector64<double> upper)
	{
		return NarrowWithSaturation<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> NarrowWithSaturation(Vector64<short> lower, Vector64<short> upper)
	{
		return NarrowWithSaturation<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<short> NarrowWithSaturation(Vector64<int> lower, Vector64<int> upper)
	{
		return NarrowWithSaturation<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<int> NarrowWithSaturation(Vector64<long> lower, Vector64<long> upper)
	{
		return NarrowWithSaturation<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<byte> NarrowWithSaturation(Vector64<ushort> lower, Vector64<ushort> upper)
	{
		return NarrowWithSaturation<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> NarrowWithSaturation(Vector64<uint> lower, Vector64<uint> upper)
	{
		return NarrowWithSaturation<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> NarrowWithSaturation(Vector64<ulong> lower, Vector64<ulong> upper)
	{
		return NarrowWithSaturation<ulong, uint>(lower, upper);
	}

	[Intrinsic]
	public static Vector64<T> Negate<T>(Vector64<T> vector)
	{
		return -vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None<T>(Vector64<T> vector, T value)
	{
		return !EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return None(vector.AsInt32(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return None(vector.AsInt64(), -1L);
		}
		return None(vector, Scalar<T>.AllBitsSet);
	}

	[Intrinsic]
	public static Vector64<T> OnesComplement<T>(Vector64<T> vector)
	{
		return ~vector;
	}

	internal static Vector64<T> RadiansToDegrees<T>(Vector64<T> radians) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.RadiansToDegrees(GetElementUnsafe(in radians, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> RadiansToDegrees(Vector64<double> radians)
	{
		if (false)
		{
		}
		return Vector64.RadiansToDegrees<double>(radians);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<float> RadiansToDegrees(Vector64<float> radians)
	{
		if (false)
		{
		}
		return Vector64.RadiansToDegrees<float>(radians);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector64<T> Round<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Round(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<double> Round(Vector64<double> vector)
	{
		return Vector64.Round<double>(vector);
	}

	[Intrinsic]
	public static Vector64<float> Round(Vector64<float> vector)
	{
		return Vector64.Round<float>(vector);
	}

	[Intrinsic]
	public static Vector64<double> Round(Vector64<double> vector, MidpointRounding mode)
	{
		return VectorMath.RoundDouble(vector, mode);
	}

	[Intrinsic]
	public static Vector64<float> Round(Vector64<float> vector, MidpointRounding mode)
	{
		return VectorMath.RoundSingle(vector, mode);
	}

	[Intrinsic]
	public static Vector64<byte> ShiftLeft(Vector64<byte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector64<short> ShiftLeft(Vector64<short> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector64<int> ShiftLeft(Vector64<int> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector64<long> ShiftLeft(Vector64<long> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector64<nint> ShiftLeft(Vector64<nint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<nuint> ShiftLeft(Vector64<nuint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> ShiftLeft(Vector64<sbyte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> ShiftLeft(Vector64<ushort> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> ShiftLeft(Vector64<uint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector64<uint> ShiftLeft(Vector64<uint> vector, Vector64<uint> shiftCount)
	{
		Unsafe.SkipInit<Vector64<uint>>(out var value);
		for (int i = 0; i < Vector64<uint>.Count; i++)
		{
			uint value2 = GetElementUnsafe(in vector, i) << (int)GetElementUnsafe(in shiftCount, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> ShiftLeft(Vector64<ulong> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector64<ulong> ShiftLeft(Vector64<ulong> vector, Vector64<ulong> shiftCount)
	{
		Unsafe.SkipInit<Vector64<ulong>>(out var value);
		for (int i = 0; i < Vector64<ulong>.Count; i++)
		{
			ulong value2 = GetElementUnsafe(in vector, i) << (int)GetElementUnsafe(in shiftCount, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<short> ShiftRightArithmetic(Vector64<short> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector64<int> ShiftRightArithmetic(Vector64<int> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector64<long> ShiftRightArithmetic(Vector64<long> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector64<nint> ShiftRightArithmetic(Vector64<nint> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> ShiftRightArithmetic(Vector64<sbyte> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector64<byte> ShiftRightLogical(Vector64<byte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector64<short> ShiftRightLogical(Vector64<short> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector64<int> ShiftRightLogical(Vector64<int> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector64<long> ShiftRightLogical(Vector64<long> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector64<nint> ShiftRightLogical(Vector64<nint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<nuint> ShiftRightLogical(Vector64<nuint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> ShiftRightLogical(Vector64<sbyte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> ShiftRightLogical(Vector64<ushort> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> ShiftRightLogical(Vector64<uint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> ShiftRightLogical(Vector64<ulong> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	internal static Vector64<byte> ShuffleNativeFallback(Vector64<byte> vector, Vector64<byte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector64<sbyte> ShuffleNativeFallback(Vector64<sbyte> vector, Vector64<sbyte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector64<short> ShuffleNativeFallback(Vector64<short> vector, Vector64<short> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector64<ushort> ShuffleNativeFallback(Vector64<ushort> vector, Vector64<ushort> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector64<int> ShuffleNativeFallback(Vector64<int> vector, Vector64<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector64<uint> ShuffleNativeFallback(Vector64<uint> vector, Vector64<uint> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector64<float> ShuffleNativeFallback(Vector64<float> vector, Vector64<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	public static Vector64<byte> Shuffle(Vector64<byte> vector, Vector64<byte> indices)
	{
		Unsafe.SkipInit<Vector64<byte>>(out var value);
		for (int i = 0; i < Vector64<byte>.Count; i++)
		{
			byte elementUnsafe = GetElementUnsafe(in indices, i);
			byte value2 = 0;
			if (elementUnsafe < Vector64<byte>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> Shuffle(Vector64<sbyte> vector, Vector64<sbyte> indices)
	{
		Unsafe.SkipInit<Vector64<sbyte>>(out var value);
		for (int i = 0; i < Vector64<sbyte>.Count; i++)
		{
			byte b = (byte)GetElementUnsafe(in indices, i);
			sbyte value2 = 0;
			if (b < Vector64<sbyte>.Count)
			{
				value2 = GetElementUnsafe(in vector, b);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<byte> ShuffleNative(Vector64<byte> vector, Vector64<byte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<sbyte> ShuffleNative(Vector64<sbyte> vector, Vector64<sbyte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector64<short> Shuffle(Vector64<short> vector, Vector64<short> indices)
	{
		Unsafe.SkipInit<Vector64<short>>(out var value);
		for (int i = 0; i < Vector64<short>.Count; i++)
		{
			ushort num = (ushort)GetElementUnsafe(in indices, i);
			short value2 = 0;
			if (num < Vector64<short>.Count)
			{
				value2 = GetElementUnsafe(in vector, num);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> Shuffle(Vector64<ushort> vector, Vector64<ushort> indices)
	{
		Unsafe.SkipInit<Vector64<ushort>>(out var value);
		for (int i = 0; i < Vector64<ushort>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in indices, i);
			ushort value2 = 0;
			if (elementUnsafe < Vector64<ushort>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<short> ShuffleNative(Vector64<short> vector, Vector64<short> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> ShuffleNative(Vector64<ushort> vector, Vector64<ushort> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector64<int> Shuffle(Vector64<int> vector, Vector64<int> indices)
	{
		Unsafe.SkipInit<Vector64<int>>(out var value);
		for (int i = 0; i < Vector64<int>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			int value2 = 0;
			if (elementUnsafe < Vector64<int>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> Shuffle(Vector64<uint> vector, Vector64<uint> indices)
	{
		Unsafe.SkipInit<Vector64<uint>>(out var value);
		for (int i = 0; i < Vector64<uint>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in indices, i);
			uint value2 = 0u;
			if (elementUnsafe < Vector64<uint>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<float> Shuffle(Vector64<float> vector, Vector64<int> indices)
	{
		Unsafe.SkipInit<Vector64<float>>(out var value);
		for (int i = 0; i < Vector64<float>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			float value2 = 0f;
			if (elementUnsafe < Vector64<float>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<int> ShuffleNative(Vector64<int> vector, Vector64<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> ShuffleNative(Vector64<uint> vector, Vector64<uint> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector64<float> ShuffleNative(Vector64<float> vector, Vector64<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	internal static Vector64<T> Sin<T>(Vector64<T> vector) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = T.Sin(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<double> Sin(Vector64<double> vector)
	{
		if (false)
		{
		}
		return Vector64.Sin<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector64<float> Sin(Vector64<float> vector)
	{
		if (false)
		{
		}
		return Vector64.Sin<float>(vector);
	}

	internal static (Vector64<T> Sin, Vector64<T> Cos) SinCos<T>(Vector64<T> vector) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		Unsafe.SkipInit<Vector64<T>>(out var value2);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			var (value3, value4) = T.SinCos(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value3);
			value2.SetElementUnsafe(i, value4);
		}
		return (Sin: value, Cos: value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector64<double> Sin, Vector64<double> Cos) SinCos(Vector64<double> vector)
	{
		if (false)
		{
		}
		return Vector64.SinCos<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector64<float> Sin, Vector64<float> Cos) SinCos(Vector64<float> vector)
	{
		if (false)
		{
		}
		return Vector64.SinCos<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> Sqrt<T>(Vector64<T> vector)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Sqrt(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void Store<T>(this Vector64<T> source, T* destination)
	{
		source.StoreUnsafe(ref *destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned<T>(this Vector64<T> source, T* destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		if ((nuint)destination % (nuint)8u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		*(Vector64<T>*)destination = source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal<T>(this Vector64<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static void StoreUnsafe<T>(this Vector64<T> source, ref T destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static void StoreUnsafe<T>(this Vector64<T> source, ref T destination, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[Intrinsic]
	public static Vector64<T> Subtract<T>(Vector64<T> left, Vector64<T> right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> SubtractSaturate<T>(Vector64<T> left, Vector64<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left - right;
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.SubtractSaturate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Sum<T>(Vector64<T> vector)
	{
		T val = default(T);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			val = Scalar<T>.Add(val, GetElementUnsafe(in vector, i));
		}
		return val;
	}

	[Intrinsic]
	public static T ToScalar<T>(this Vector64<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		return GetElementUnsafe(in vector, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> ToVector128<T>(this Vector64<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Vector128<T> vector2 = default(Vector128<T>);
		vector2.SetLowerUnsafe<T>(vector);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> ToVector128Unsafe<T>(this Vector64<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Unsafe.SkipInit<Vector128<T>>(out var value);
		value.SetLowerUnsafe<T>(vector);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector64<T> Truncate<T>(Vector64<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Vector64<T>.Count; i++)
		{
			T value2 = Scalar<T>.Truncate(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<double> Truncate(Vector64<double> vector)
	{
		return Vector64.Truncate<double>(vector);
	}

	[Intrinsic]
	public static Vector64<float> Truncate(Vector64<float> vector)
	{
		return Vector64.Truncate<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryCopyTo<T>(this Vector64<T> vector, Span<T> destination)
	{
		if (destination.Length < Vector64<T>.Count)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector64<ushort> Lower, Vector64<ushort> Upper) Widen(Vector64<byte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector64<int> Lower, Vector64<int> Upper) Widen(Vector64<short> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector64<long> Lower, Vector64<long> Upper) Widen(Vector64<int> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector64<short> Lower, Vector64<short> Upper) Widen(Vector64<sbyte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector64<double> Lower, Vector64<double> Upper) Widen(Vector64<float> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector64<uint> Lower, Vector64<uint> Upper) Widen(Vector64<ushort> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector64<ulong> Lower, Vector64<ulong> Upper) Widen(Vector64<uint> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> WidenLower(Vector64<byte> source)
	{
		Unsafe.SkipInit<Vector64<ushort>>(out var value);
		for (int i = 0; i < Vector64<ushort>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<int> WidenLower(Vector64<short> source)
	{
		Unsafe.SkipInit<Vector64<int>>(out var value);
		for (int i = 0; i < Vector64<int>.Count; i++)
		{
			int elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<long> WidenLower(Vector64<int> source)
	{
		Unsafe.SkipInit<Vector64<long>>(out var value);
		for (int i = 0; i < Vector64<long>.Count; i++)
		{
			long value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<short> WidenLower(Vector64<sbyte> source)
	{
		Unsafe.SkipInit<Vector64<short>>(out var value);
		for (int i = 0; i < Vector64<short>.Count; i++)
		{
			short elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> WidenLower(Vector64<float> source)
	{
		Unsafe.SkipInit<Vector64<double>>(out var value);
		for (int i = 0; i < Vector64<double>.Count; i++)
		{
			double value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> WidenLower(Vector64<ushort> source)
	{
		Unsafe.SkipInit<Vector64<uint>>(out var value);
		for (int i = 0; i < Vector64<uint>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> WidenLower(Vector64<uint> source)
	{
		Unsafe.SkipInit<Vector64<ulong>>(out var value);
		for (int i = 0; i < Vector64<ulong>.Count; i++)
		{
			ulong value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ushort> WidenUpper(Vector64<byte> source)
	{
		Unsafe.SkipInit<Vector64<ushort>>(out var value);
		for (int i = Vector64<ushort>.Count; i < Vector64<byte>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector64<ushort>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<int> WidenUpper(Vector64<short> source)
	{
		Unsafe.SkipInit<Vector64<int>>(out var value);
		for (int i = Vector64<int>.Count; i < Vector64<short>.Count; i++)
		{
			int elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector64<int>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<long> WidenUpper(Vector64<int> source)
	{
		Unsafe.SkipInit<Vector64<long>>(out var value);
		for (int i = Vector64<long>.Count; i < Vector64<int>.Count; i++)
		{
			long value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector64<long>.Count, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<short> WidenUpper(Vector64<sbyte> source)
	{
		Unsafe.SkipInit<Vector64<short>>(out var value);
		for (int i = Vector64<short>.Count; i < Vector64<sbyte>.Count; i++)
		{
			short elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector64<short>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<double> WidenUpper(Vector64<float> source)
	{
		Unsafe.SkipInit<Vector64<double>>(out var value);
		for (int i = Vector64<double>.Count; i < Vector64<float>.Count; i++)
		{
			double value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector64<double>.Count, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<uint> WidenUpper(Vector64<ushort> source)
	{
		Unsafe.SkipInit<Vector64<uint>>(out var value);
		for (int i = Vector64<uint>.Count; i < Vector64<ushort>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector64<uint>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector64<ulong> WidenUpper(Vector64<uint> source)
	{
		Unsafe.SkipInit<Vector64<ulong>>(out var value);
		for (int i = Vector64<ulong>.Count; i < Vector64<uint>.Count; i++)
		{
			ulong value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector64<ulong>.Count, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> WithElement<T>(this Vector64<T> vector, int index, T value)
	{
		if ((uint)index >= (uint)Vector64<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		Vector64<T> vector2 = vector;
		SetElementUnsafe(in vector2, index, value);
		return vector2;
	}

	[Intrinsic]
	public static Vector64<T> Xor<T>(Vector64<T> left, Vector64<T> right)
	{
		return left ^ right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T GetElementUnsafe<T>(this in Vector64<T> vector, int index)
	{
		return Unsafe.Add(ref Unsafe.As<Vector64<T>, T>(ref Unsafe.AsRef(in vector)), index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetElementUnsafe<T>(this in Vector64<T> vector, int index, T value)
	{
		Unsafe.Add(ref Unsafe.As<Vector64<T>, T>(ref Unsafe.AsRef(in vector)), index) = value;
	}
}
[StructLayout(LayoutKind.Sequential, Size = 8)]
[Intrinsic]
[DebuggerDisplay("{DisplayString,nq}")]
[DebuggerTypeProxy(typeof(Vector64DebugView<>))]
public readonly struct Vector64<T> : ISimdVector<Vector64<T>, T>, IAdditionOperators<Vector64<T>, Vector64<T>, Vector64<T>>, IBitwiseOperators<Vector64<T>, Vector64<T>, Vector64<T>>, IDivisionOperators<Vector64<T>, Vector64<T>, Vector64<T>>, IEqualityOperators<Vector64<T>, Vector64<T>, bool>, IEquatable<Vector64<T>>, IMultiplyOperators<Vector64<T>, Vector64<T>, Vector64<T>>, IShiftOperators<Vector64<T>, int, Vector64<T>>, ISubtractionOperators<Vector64<T>, Vector64<T>, Vector64<T>>, IUnaryNegationOperators<Vector64<T>, Vector64<T>>, IUnaryPlusOperators<Vector64<T>, Vector64<T>>
{
	internal readonly ulong _00;

	public static Vector64<T> AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector64.Create(Scalar<T>.AllBitsSet);
		}
	}

	public static int Count
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
			return 8 / Unsafe.SizeOf<T>();
		}
	}

	public static Vector64<T> Indices
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
			Unsafe.SkipInit<Vector64<T>>(out var value);
			for (int i = 0; i < Count; i++)
			{
				value.SetElementUnsafe(i, Scalar<T>.Convert(i));
			}
			return value;
		}
	}

	public static bool IsSupported
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Intrinsic]
		get
		{
			if (!(typeof(T) == typeof(byte)) && !(typeof(T) == typeof(double)) && !(typeof(T) == typeof(short)) && !(typeof(T) == typeof(int)) && !(typeof(T) == typeof(long)) && !(typeof(T) == typeof(nint)) && !(typeof(T) == typeof(sbyte)) && !(typeof(T) == typeof(float)) && !(typeof(T) == typeof(ushort)) && !(typeof(T) == typeof(uint)) && !(typeof(T) == typeof(ulong)))
			{
				return typeof(T) == typeof(nuint);
			}
			return true;
		}
	}

	public static Vector64<T> One
	{
		[Intrinsic]
		get
		{
			return Vector64.Create(Scalar<T>.One);
		}
	}

	public static Vector64<T> Zero
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
			return default(Vector64<T>);
		}
	}

	internal string DisplayString
	{
		get
		{
			if (!IsSupported)
			{
				return SR.NotSupported_Type;
			}
			return ToString();
		}
	}

	public T this[int index] => this.GetElement(index);

	static int ISimdVector<Vector64<T>, T>.Alignment => 8;

	static int ISimdVector<Vector64<T>, T>.ElementCount => Count;

	static bool ISimdVector<Vector64<T>, T>.IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator +(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Add(Vector64.GetElementUnsafe(in left, i), Vector64.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator &(Vector64<T> left, Vector64<T> right)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Unsafe.SkipInit<Vector64<T>>(out var value);
		Unsafe.AsRef(in value._00) = left._00 & right._00;
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator |(Vector64<T> left, Vector64<T> right)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Unsafe.SkipInit<Vector64<T>>(out var value);
		Unsafe.AsRef(in value._00) = left._00 | right._00;
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator /(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Divide(Vector64.GetElementUnsafe(in left, i), Vector64.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator /(Vector64<T> left, T right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Divide(Vector64.GetElementUnsafe(in left, i), right);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector64<T> left, Vector64<T> right)
	{
		for (int i = 0; i < Count; i++)
		{
			if (!Scalar<T>.Equals(Vector64.GetElementUnsafe(in left, i), Vector64.GetElementUnsafe(in right, i)))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator ^(Vector64<T> left, Vector64<T> right)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Unsafe.SkipInit<Vector64<T>>(out var value);
		Unsafe.AsRef(in value._00) = left._00 ^ right._00;
		return value;
	}

	[Intrinsic]
	public static bool operator !=(Vector64<T> left, Vector64<T> right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator <<(Vector64<T> value, int shiftCount)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value2);
		for (int i = 0; i < Count; i++)
		{
			T value3 = Scalar<T>.ShiftLeft(Vector64.GetElementUnsafe(in value, i), shiftCount);
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator *(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Multiply(Vector64.GetElementUnsafe(in left, i), Vector64.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator *(Vector64<T> left, T right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Multiply(Vector64.GetElementUnsafe(in left, i), right);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector64<T> operator *(T left, Vector64<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator ~(Vector64<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Unsafe.SkipInit<Vector64<T>>(out var value);
		Unsafe.AsRef(in value._00) = ~vector._00;
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator >>(Vector64<T> value, int shiftCount)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value2);
		for (int i = 0; i < Count; i++)
		{
			T value3 = Scalar<T>.ShiftRightArithmetic(Vector64.GetElementUnsafe(in value, i), shiftCount);
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator -(Vector64<T> left, Vector64<T> right)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Subtract(Vector64.GetElementUnsafe(in left, i), Vector64.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator -(Vector64<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return vector ^ Vector64.Create(-0f).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return vector ^ Vector64.Create(-0.0).As<double, T>();
		}
		return Zero - vector;
	}

	[Intrinsic]
	public static Vector64<T> operator +(Vector64<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector64<T> operator >>>(Vector64<T> value, int shiftCount)
	{
		Unsafe.SkipInit<Vector64<T>>(out var value2);
		for (int i = 0; i < Count; i++)
		{
			T value3 = Scalar<T>.ShiftRightLogical(Vector64.GetElementUnsafe(in value, i), shiftCount);
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector64<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector64<T> other)
	{
		if (false)
		{
		}
		return SoftwareFallback(this, other);
		static bool SoftwareFallback(in Vector64<T> self, Vector64<T> vector)
		{
			for (int i = 0; i < Count; i++)
			{
				if (!Scalar<T>.ObjectEquals(Vector64.GetElementUnsafe(in self, i), Vector64.GetElementUnsafe(in vector, i)))
				{
					return false;
				}
			}
			return true;
		}
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		for (int i = 0; i < Count; i++)
		{
			T elementUnsafe = this.GetElementUnsafe<T>(i);
			hashCode.Add(elementUnsafe);
		}
		return hashCode.ToHashCode();
	}

	public override string ToString()
	{
		return ToString("G", CultureInfo.InvariantCulture);
	}

	private string ToString([StringSyntax("NumericFormat")] string format, IFormatProvider formatProvider)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector64BaseType<T>();
		Span<char> initialBuffer = stackalloc char[64];
		ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
		string numberGroupSeparator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
		valueStringBuilder.Append('<');
		valueStringBuilder.Append(((IFormattable)(object)this.GetElementUnsafe<T>(0)).ToString(format, formatProvider));
		for (int i = 1; i < Count; i++)
		{
			valueStringBuilder.Append(numberGroupSeparator);
			valueStringBuilder.Append(' ');
			valueStringBuilder.Append(((IFormattable)(object)this.GetElementUnsafe<T>(i)).ToString(format, formatProvider));
		}
		valueStringBuilder.Append('>');
		return valueStringBuilder.ToString();
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Abs(Vector64<T> vector)
	{
		return Vector64.Abs(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Add(Vector64<T> left, Vector64<T> right)
	{
		return left + right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.All(Vector64<T> vector, T value)
	{
		return Vector64.All(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.AllWhereAllBitsSet(Vector64<T> vector)
	{
		return Vector64.AllWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.AndNot(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.AndNot(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.Any(Vector64<T> vector, T value)
	{
		return Vector64.Any(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.AnyWhereAllBitsSet(Vector64<T> vector)
	{
		return Vector64.AnyWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.BitwiseAnd(Vector64<T> left, Vector64<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.BitwiseOr(Vector64<T> left, Vector64<T> right)
	{
		return left | right;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Ceiling(Vector64<T> vector)
	{
		return Vector64.Ceiling(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Clamp(Vector64<T> value, Vector64<T> min, Vector64<T> max)
	{
		return Vector64.Clamp(value, min, max);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.ClampNative(Vector64<T> value, Vector64<T> min, Vector64<T> max)
	{
		return Vector64.ClampNative(value, min, max);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.ConditionalSelect(Vector64<T> condition, Vector64<T> left, Vector64<T> right)
	{
		return Vector64.ConditionalSelect(condition, left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.CopySign(Vector64<T> value, Vector64<T> sign)
	{
		return Vector64.CopySign(value, sign);
	}

	static void ISimdVector<Vector64<T>, T>.CopyTo(Vector64<T> vector, T[] destination)
	{
		vector.CopyTo(destination);
	}

	static void ISimdVector<Vector64<T>, T>.CopyTo(Vector64<T> vector, T[] destination, int startIndex)
	{
		vector.CopyTo(destination, startIndex);
	}

	static void ISimdVector<Vector64<T>, T>.CopyTo(Vector64<T> vector, Span<T> destination)
	{
		vector.CopyTo(destination);
	}

	[Intrinsic]
	static int ISimdVector<Vector64<T>, T>.Count(Vector64<T> vector, T value)
	{
		return Vector64.Count(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector64<T>, T>.CountWhereAllBitsSet(Vector64<T> vector)
	{
		return Vector64.CountWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Create(T value)
	{
		return Vector64.Create(value);
	}

	static Vector64<T> ISimdVector<Vector64<T>, T>.Create(T[] values)
	{
		return Vector64.Create(values);
	}

	static Vector64<T> ISimdVector<Vector64<T>, T>.Create(T[] values, int index)
	{
		return Vector64.Create(values, index);
	}

	static Vector64<T> ISimdVector<Vector64<T>, T>.Create(ReadOnlySpan<T> values)
	{
		return Vector64.Create(values);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.CreateScalar(T value)
	{
		return Vector64.CreateScalar(value);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.CreateScalarUnsafe(T value)
	{
		return Vector64.CreateScalarUnsafe(value);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Divide(Vector64<T> left, Vector64<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Divide(Vector64<T> left, T right)
	{
		return left / right;
	}

	[Intrinsic]
	static T ISimdVector<Vector64<T>, T>.Dot(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.Dot(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Equals(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.Equals(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.EqualsAll(Vector64<T> left, Vector64<T> right)
	{
		return left == right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.EqualsAny(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.EqualsAny(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Floor(Vector64<T> vector)
	{
		return Vector64.Floor(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector64<T>, T>.GetElement(Vector64<T> vector, int index)
	{
		return vector.GetElement(index);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.GreaterThan(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.GreaterThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.GreaterThanAll(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.GreaterThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.GreaterThanAny(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.GreaterThanAny(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.GreaterThanOrEqual(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.GreaterThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.GreaterThanOrEqualAll(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.GreaterThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.GreaterThanOrEqualAny(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.GreaterThanOrEqualAny(left, right);
	}

	[Intrinsic]
	static int ISimdVector<Vector64<T>, T>.IndexOf(Vector64<T> vector, T value)
	{
		return Vector64.IndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector64<T>, T>.IndexOfWhereAllBitsSet(Vector64<T> vector)
	{
		return Vector64.IndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsEvenInteger(Vector64<T> vector)
	{
		return Vector64.IsEvenInteger(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsFinite(Vector64<T> vector)
	{
		return Vector64.IsFinite(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsInfinity(Vector64<T> vector)
	{
		return Vector64.IsInfinity(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsInteger(Vector64<T> vector)
	{
		return Vector64.IsInteger(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsNaN(Vector64<T> vector)
	{
		return Vector64.IsNaN(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsNegative(Vector64<T> vector)
	{
		return Vector64.IsNegative(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsNegativeInfinity(Vector64<T> vector)
	{
		return Vector64.IsNegativeInfinity(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsNormal(Vector64<T> vector)
	{
		return Vector64.IsNormal(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsOddInteger(Vector64<T> vector)
	{
		return Vector64.IsOddInteger(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsPositive(Vector64<T> vector)
	{
		return Vector64.IsPositive(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsPositiveInfinity(Vector64<T> vector)
	{
		return Vector64.IsPositiveInfinity(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsSubnormal(Vector64<T> vector)
	{
		return Vector64.IsSubnormal(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.IsZero(Vector64<T> vector)
	{
		return Vector64.IsZero(vector);
	}

	[Intrinsic]
	static int ISimdVector<Vector64<T>, T>.LastIndexOf(Vector64<T> vector, T value)
	{
		return Vector64.LastIndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector64<T>, T>.LastIndexOfWhereAllBitsSet(Vector64<T> vector)
	{
		return Vector64.LastIndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.LessThan(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.LessThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.LessThanAll(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.LessThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.LessThanAny(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.LessThanAny(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.LessThanOrEqual(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.LessThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.LessThanOrEqualAll(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.LessThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.LessThanOrEqualAny(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.LessThanOrEqualAny(left, right);
	}

	[Intrinsic]
	unsafe static Vector64<T> ISimdVector<Vector64<T>, T>.Load(T* source)
	{
		return Vector64.Load(source);
	}

	[Intrinsic]
	unsafe static Vector64<T> ISimdVector<Vector64<T>, T>.LoadAligned(T* source)
	{
		return Vector64.LoadAligned(source);
	}

	[Intrinsic]
	unsafe static Vector64<T> ISimdVector<Vector64<T>, T>.LoadAlignedNonTemporal(T* source)
	{
		return Vector64.LoadAlignedNonTemporal(source);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.LoadUnsafe(ref readonly T source)
	{
		return Vector64.LoadUnsafe(in source);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.LoadUnsafe(ref readonly T source, nuint elementOffset)
	{
		return Vector64.LoadUnsafe(in source, elementOffset);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Max(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.Max(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MaxMagnitude(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MaxMagnitude(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MaxMagnitudeNumber(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MaxMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MaxNative(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MaxNative(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MaxNumber(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MaxNumber(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Min(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.Min(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MinMagnitude(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MinMagnitude(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MinMagnitudeNumber(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MinMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MinNative(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MinNative(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MinNumber(Vector64<T> left, Vector64<T> right)
	{
		return Vector64.MinNumber(left, right);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Multiply(Vector64<T> left, Vector64<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Multiply(Vector64<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.MultiplyAddEstimate(Vector64<T> left, Vector64<T> right, Vector64<T> addend)
	{
		return Vector64.MultiplyAddEstimate(left, right, addend);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Negate(Vector64<T> vector)
	{
		return -vector;
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.None(Vector64<T> vector, T value)
	{
		return Vector64.None(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector64<T>, T>.NoneWhereAllBitsSet(Vector64<T> vector)
	{
		return Vector64.NoneWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.OnesComplement(Vector64<T> vector)
	{
		return ~vector;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Round(Vector64<T> vector)
	{
		return Vector64.Round(vector);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.ShiftLeft(Vector64<T> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.ShiftRightArithmetic(Vector64<T> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.ShiftRightLogical(Vector64<T> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Sqrt(Vector64<T> vector)
	{
		return Vector64.Sqrt(vector);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector64<T>, T>.Store(Vector64<T> source, T* destination)
	{
		source.Store(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector64<T>, T>.StoreAligned(Vector64<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector64<T>, T>.StoreAlignedNonTemporal(Vector64<T> source, T* destination)
	{
		source.StoreAlignedNonTemporal(destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector64<T>, T>.StoreUnsafe(Vector64<T> vector, ref T destination)
	{
		vector.StoreUnsafe(ref destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector64<T>, T>.StoreUnsafe(Vector64<T> vector, ref T destination, nuint elementOffset)
	{
		vector.StoreUnsafe(ref destination, elementOffset);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Subtract(Vector64<T> left, Vector64<T> right)
	{
		return left - right;
	}

	[Intrinsic]
	static T ISimdVector<Vector64<T>, T>.Sum(Vector64<T> vector)
	{
		return Vector64.Sum(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector64<T>, T>.ToScalar(Vector64<T> vector)
	{
		return vector.ToScalar();
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Truncate(Vector64<T> vector)
	{
		return Vector64.Truncate(vector);
	}

	static bool ISimdVector<Vector64<T>, T>.TryCopyTo(Vector64<T> vector, Span<T> destination)
	{
		return vector.TryCopyTo(destination);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.WithElement(Vector64<T> vector, int index, T value)
	{
		return vector.WithElement(index, value);
	}

	[Intrinsic]
	static Vector64<T> ISimdVector<Vector64<T>, T>.Xor(Vector64<T> left, Vector64<T> right)
	{
		return left ^ right;
	}
}
