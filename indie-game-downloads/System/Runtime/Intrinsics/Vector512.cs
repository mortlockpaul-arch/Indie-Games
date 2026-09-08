using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.Intrinsics;

public static class Vector512
{
	public static bool IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return IsHardwareAccelerated;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> Abs<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return vector;
		}
		return Create(Vector256.Abs(vector._lower), Vector256.Abs(vector._upper));
	}

	[Intrinsic]
	public static Vector512<T> Add<T>(Vector512<T> left, Vector512<T> right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> AddSaturate<T>(Vector512<T> left, Vector512<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left + right;
		}
		return Create(Vector256.AddSaturate(left._lower, right._lower), Vector256.AddSaturate(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All<T>(Vector512<T> vector, T value)
	{
		return vector == Create(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet<T>(Vector512<T> vector)
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> AndNot<T>(Vector512<T> left, Vector512<T> right)
	{
		return left & ~right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any<T>(Vector512<T> vector, T value)
	{
		return EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet<T>(Vector512<T> vector)
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
	public static Vector512<TTo> As<TFrom, TTo>(this Vector512<TFrom> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<TFrom>();
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<TTo>();
		return Unsafe.BitCast<Vector512<TFrom>, Vector512<TTo>>(vector);
	}

	[Intrinsic]
	public static Vector512<byte> AsByte<T>(this Vector512<T> vector)
	{
		return vector.As<T, byte>();
	}

	[Intrinsic]
	public static Vector512<double> AsDouble<T>(this Vector512<T> vector)
	{
		return vector.As<T, double>();
	}

	[Intrinsic]
	public static Vector512<short> AsInt16<T>(this Vector512<T> vector)
	{
		return vector.As<T, short>();
	}

	[Intrinsic]
	public static Vector512<int> AsInt32<T>(this Vector512<T> vector)
	{
		return vector.As<T, int>();
	}

	[Intrinsic]
	public static Vector512<long> AsInt64<T>(this Vector512<T> vector)
	{
		return vector.As<T, long>();
	}

	[Intrinsic]
	public static Vector512<nint> AsNInt<T>(this Vector512<T> vector)
	{
		return vector.As<T, nint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<nuint> AsNUInt<T>(this Vector512<T> vector)
	{
		return vector.As<T, nuint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> AsSByte<T>(this Vector512<T> vector)
	{
		return vector.As<T, sbyte>();
	}

	[Intrinsic]
	public static Vector512<float> AsSingle<T>(this Vector512<T> vector)
	{
		return vector.As<T, float>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> AsUInt16<T>(this Vector512<T> vector)
	{
		return vector.As<T, ushort>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> AsUInt32<T>(this Vector512<T> vector)
	{
		return vector.As<T, uint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> AsUInt64<T>(this Vector512<T> vector)
	{
		return vector.As<T, ulong>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> AsVector512<T>(this Vector<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		Vector512<T> source = default(Vector512<T>);
		Unsafe.WriteUnaligned(ref Unsafe.As<Vector512<T>, byte>(ref source), value);
		return source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> AsVector<T>(this Vector512<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		return Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<Vector512<T>, byte>(ref value));
	}

	[Intrinsic]
	public static Vector512<T> BitwiseAnd<T>(Vector512<T> left, Vector512<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector512<T> BitwiseOr<T>(Vector512<T> left, Vector512<T> right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector512<T> Ceiling<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector256.Ceiling(vector._lower), Vector256.Ceiling(vector._upper));
	}

	[Intrinsic]
	public static Vector512<float> Ceiling(Vector512<float> vector)
	{
		return Vector512.Ceiling<float>(vector);
	}

	[Intrinsic]
	public static Vector512<double> Ceiling(Vector512<double> vector)
	{
		return Vector512.Ceiling<double>(vector);
	}

	[Intrinsic]
	public static Vector512<T> Clamp<T>(Vector512<T> value, Vector512<T> min, Vector512<T> max)
	{
		return Min(Max(value, min), max);
	}

	[Intrinsic]
	public static Vector512<T> ClampNative<T>(Vector512<T> value, Vector512<T> min, Vector512<T> max)
	{
		return MinNative(MaxNative(value, min), max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> ConditionalSelect<T>(Vector512<T> condition, Vector512<T> left, Vector512<T> right)
	{
		return (left & condition) | AndNot(right, condition);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> ConvertToDouble(Vector512<long> vector)
	{
		return Create(Vector256.ConvertToDouble(vector._lower), Vector256.ConvertToDouble(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<double> ConvertToDouble(Vector512<ulong> vector)
	{
		return Create(Vector256.ConvertToDouble(vector._lower), Vector256.ConvertToDouble(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<int> ConvertToInt32(Vector512<float> vector)
	{
		return Create(Vector256.ConvertToInt32(vector._lower), Vector256.ConvertToInt32(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<int> ConvertToInt32Native(Vector512<float> vector)
	{
		return Create(Vector256.ConvertToInt32Native(vector._lower), Vector256.ConvertToInt32Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<long> ConvertToInt64(Vector512<double> vector)
	{
		return Create(Vector256.ConvertToInt64(vector._lower), Vector256.ConvertToInt64(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<long> ConvertToInt64Native(Vector512<double> vector)
	{
		return Create(Vector256.ConvertToInt64Native(vector._lower), Vector256.ConvertToInt64Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> ConvertToSingle(Vector512<int> vector)
	{
		return Create(Vector256.ConvertToSingle(vector._lower), Vector256.ConvertToSingle(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<float> ConvertToSingle(Vector512<uint> vector)
	{
		return Create(Vector256.ConvertToSingle(vector._lower), Vector256.ConvertToSingle(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> ConvertToUInt32(Vector512<float> vector)
	{
		return Create(Vector256.ConvertToUInt32(vector._lower), Vector256.ConvertToUInt32(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> ConvertToUInt32Native(Vector512<float> vector)
	{
		return Create(Vector256.ConvertToUInt32Native(vector._lower), Vector256.ConvertToUInt32Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> ConvertToUInt64(Vector512<double> vector)
	{
		return Create(Vector256.ConvertToUInt64(vector._lower), Vector256.ConvertToUInt64(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> ConvertToUInt64Native(Vector512<double> vector)
	{
		return Create(Vector256.ConvertToUInt64Native(vector._lower), Vector256.ConvertToUInt64Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> CopySign<T>(Vector512<T> value, Vector512<T> sign)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return value;
		}
		if (IsHardwareAccelerated)
		{
			return VectorMath.CopySign<Vector512<T>, T>(value, sign);
		}
		return Create(Vector256.CopySign(value._lower, sign._lower), Vector256.CopySign(value._upper, sign._upper));
	}

	public static void CopyTo<T>(this Vector512<T> vector, T[] destination)
	{
		if (destination.Length < Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[0]), vector);
	}

	public static void CopyTo<T>(this Vector512<T> vector, T[] destination, int startIndex)
	{
		if ((uint)startIndex >= (uint)destination.Length)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
		}
		if (destination.Length - startIndex < Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[startIndex]), vector);
	}

	public static void CopyTo<T>(this Vector512<T> vector, Span<T> destination)
	{
		if ((uint)destination.Length < (uint)Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<double> Cos(Vector512<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.CosDouble<Vector512<double>, Vector512<long>>(vector);
		}
		return Create(Vector256.Cos(vector._lower), Vector256.Cos(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<float> Cos(Vector512<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.CosSingle<Vector512<float>, Vector512<int>, Vector512<double>, Vector512<long>>(vector);
		}
		return Create(Vector256.Cos(vector._lower), Vector256.Cos(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count<T>(Vector512<T> vector, T value)
	{
		return BitOperations.PopCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet<T>(Vector512<T> vector)
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

	[Intrinsic]
	public static Vector512<T> Create<T>(T value)
	{
		Vector256<T> vector = Vector256.Create(value);
		return Create(vector, vector);
	}

	[Intrinsic]
	public static Vector512<byte> Create(byte value)
	{
		return Vector512.Create<byte>(value);
	}

	[Intrinsic]
	public static Vector512<double> Create(double value)
	{
		return Vector512.Create<double>(value);
	}

	[Intrinsic]
	public static Vector512<short> Create(short value)
	{
		return Vector512.Create<short>(value);
	}

	[Intrinsic]
	public static Vector512<int> Create(int value)
	{
		return Vector512.Create<int>(value);
	}

	[Intrinsic]
	public static Vector512<long> Create(long value)
	{
		return Vector512.Create<long>(value);
	}

	[Intrinsic]
	public static Vector512<nint> Create(nint value)
	{
		return Vector512.Create<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<nuint> Create(nuint value)
	{
		return Vector512.Create<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> Create(sbyte value)
	{
		return Vector512.Create<sbyte>(value);
	}

	[Intrinsic]
	public static Vector512<float> Create(float value)
	{
		return Vector512.Create<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> Create(ushort value)
	{
		return Vector512.Create<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> Create(uint value)
	{
		return Vector512.Create<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> Create(ulong value)
	{
		return Vector512.Create<ulong>(value);
	}

	public static Vector512<T> Create<T>(T[] values)
	{
		if (values.Length < Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector512<T>>(in Unsafe.As<T, byte>(ref values[0]));
	}

	public static Vector512<T> Create<T>(T[] values, int index)
	{
		if (index < 0 || values.Length - index < Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector512<T>>(in Unsafe.As<T, byte>(ref values[index]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<T> Create<T>(ReadOnlySpan<T> values)
	{
		if (values.Length < Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return Unsafe.ReadUnaligned<Vector512<T>>(in Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<byte> Create(byte e0, byte e1, byte e2, byte e3, byte e4, byte e5, byte e6, byte e7, byte e8, byte e9, byte e10, byte e11, byte e12, byte e13, byte e14, byte e15, byte e16, byte e17, byte e18, byte e19, byte e20, byte e21, byte e22, byte e23, byte e24, byte e25, byte e26, byte e27, byte e28, byte e29, byte e30, byte e31, byte e32, byte e33, byte e34, byte e35, byte e36, byte e37, byte e38, byte e39, byte e40, byte e41, byte e42, byte e43, byte e44, byte e45, byte e46, byte e47, byte e48, byte e49, byte e50, byte e51, byte e52, byte e53, byte e54, byte e55, byte e56, byte e57, byte e58, byte e59, byte e60, byte e61, byte e62, byte e63)
	{
		return Create(Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16, e17, e18, e19, e20, e21, e22, e23, e24, e25, e26, e27, e28, e29, e30, e31), Vector256.Create(e32, e33, e34, e35, e36, e37, e38, e39, e40, e41, e42, e43, e44, e45, e46, e47, e48, e49, e50, e51, e52, e53, e54, e55, e56, e57, e58, e59, e60, e61, e62, e63));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> Create(double e0, double e1, double e2, double e3, double e4, double e5, double e6, double e7)
	{
		return Create(Vector256.Create(e0, e1, e2, e3), Vector256.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<short> Create(short e0, short e1, short e2, short e3, short e4, short e5, short e6, short e7, short e8, short e9, short e10, short e11, short e12, short e13, short e14, short e15, short e16, short e17, short e18, short e19, short e20, short e21, short e22, short e23, short e24, short e25, short e26, short e27, short e28, short e29, short e30, short e31)
	{
		return Create(Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15), Vector256.Create(e16, e17, e18, e19, e20, e21, e22, e23, e24, e25, e26, e27, e28, e29, e30, e31));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<int> Create(int e0, int e1, int e2, int e3, int e4, int e5, int e6, int e7, int e8, int e9, int e10, int e11, int e12, int e13, int e14, int e15)
	{
		return Create(Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7), Vector256.Create(e8, e9, e10, e11, e12, e13, e14, e15));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<long> Create(long e0, long e1, long e2, long e3, long e4, long e5, long e6, long e7)
	{
		return Create(Vector256.Create(e0, e1, e2, e3), Vector256.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> Create(sbyte e0, sbyte e1, sbyte e2, sbyte e3, sbyte e4, sbyte e5, sbyte e6, sbyte e7, sbyte e8, sbyte e9, sbyte e10, sbyte e11, sbyte e12, sbyte e13, sbyte e14, sbyte e15, sbyte e16, sbyte e17, sbyte e18, sbyte e19, sbyte e20, sbyte e21, sbyte e22, sbyte e23, sbyte e24, sbyte e25, sbyte e26, sbyte e27, sbyte e28, sbyte e29, sbyte e30, sbyte e31, sbyte e32, sbyte e33, sbyte e34, sbyte e35, sbyte e36, sbyte e37, sbyte e38, sbyte e39, sbyte e40, sbyte e41, sbyte e42, sbyte e43, sbyte e44, sbyte e45, sbyte e46, sbyte e47, sbyte e48, sbyte e49, sbyte e50, sbyte e51, sbyte e52, sbyte e53, sbyte e54, sbyte e55, sbyte e56, sbyte e57, sbyte e58, sbyte e59, sbyte e60, sbyte e61, sbyte e62, sbyte e63)
	{
		return Create(Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16, e17, e18, e19, e20, e21, e22, e23, e24, e25, e26, e27, e28, e29, e30, e31), Vector256.Create(e32, e33, e34, e35, e36, e37, e38, e39, e40, e41, e42, e43, e44, e45, e46, e47, e48, e49, e50, e51, e52, e53, e54, e55, e56, e57, e58, e59, e60, e61, e62, e63));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> Create(float e0, float e1, float e2, float e3, float e4, float e5, float e6, float e7, float e8, float e9, float e10, float e11, float e12, float e13, float e14, float e15)
	{
		return Create(Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7), Vector256.Create(e8, e9, e10, e11, e12, e13, e14, e15));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> Create(ushort e0, ushort e1, ushort e2, ushort e3, ushort e4, ushort e5, ushort e6, ushort e7, ushort e8, ushort e9, ushort e10, ushort e11, ushort e12, ushort e13, ushort e14, ushort e15, ushort e16, ushort e17, ushort e18, ushort e19, ushort e20, ushort e21, ushort e22, ushort e23, ushort e24, ushort e25, ushort e26, ushort e27, ushort e28, ushort e29, ushort e30, ushort e31)
	{
		return Create(Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15), Vector256.Create(e16, e17, e18, e19, e20, e21, e22, e23, e24, e25, e26, e27, e28, e29, e30, e31));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> Create(uint e0, uint e1, uint e2, uint e3, uint e4, uint e5, uint e6, uint e7, uint e8, uint e9, uint e10, uint e11, uint e12, uint e13, uint e14, uint e15)
	{
		return Create(Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7), Vector256.Create(e8, e9, e10, e11, e12, e13, e14, e15));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> Create(ulong e0, ulong e1, ulong e2, ulong e3, ulong e4, ulong e5, ulong e6, ulong e7)
	{
		return Create(Vector256.Create(e0, e1, e2, e3), Vector256.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<T> Create<T>(Vector64<T> value)
	{
		return Create(Vector128.Create(value, value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<T> Create<T>(Vector128<T> value)
	{
		return Create(Vector256.Create(value, value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<T> Create<T>(Vector256<T> value)
	{
		return Create(value, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<T> Create<T>(Vector256<T> lower, Vector256<T> upper)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		Unsafe.SkipInit<Vector512<T>>(out var value);
		value.SetLowerUnsafe<T>(lower);
		value.SetUpperUnsafe<T>(upper);
		return value;
	}

	public static Vector512<byte> Create(Vector256<byte> lower, Vector256<byte> upper)
	{
		return Vector512.Create<byte>(lower, upper);
	}

	public static Vector512<double> Create(Vector256<double> lower, Vector256<double> upper)
	{
		return Vector512.Create<double>(lower, upper);
	}

	public static Vector512<short> Create(Vector256<short> lower, Vector256<short> upper)
	{
		return Vector512.Create<short>(lower, upper);
	}

	public static Vector512<int> Create(Vector256<int> lower, Vector256<int> upper)
	{
		return Vector512.Create<int>(lower, upper);
	}

	public static Vector512<long> Create(Vector256<long> lower, Vector256<long> upper)
	{
		return Vector512.Create<long>(lower, upper);
	}

	public static Vector512<nint> Create(Vector256<nint> lower, Vector256<nint> upper)
	{
		return Vector512.Create<nint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector512<nuint> Create(Vector256<nuint> lower, Vector256<nuint> upper)
	{
		return Vector512.Create<nuint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector512<sbyte> Create(Vector256<sbyte> lower, Vector256<sbyte> upper)
	{
		return Vector512.Create<sbyte>(lower, upper);
	}

	public static Vector512<float> Create(Vector256<float> lower, Vector256<float> upper)
	{
		return Vector512.Create<float>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector512<ushort> Create(Vector256<ushort> lower, Vector256<ushort> upper)
	{
		return Vector512.Create<ushort>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector512<uint> Create(Vector256<uint> lower, Vector256<uint> upper)
	{
		return Vector512.Create<uint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector512<ulong> Create(Vector256<ulong> lower, Vector256<ulong> upper)
	{
		return Vector512.Create<ulong>(lower, upper);
	}

	[Intrinsic]
	public static Vector512<T> CreateScalar<T>(T value)
	{
		return Vector256.CreateScalar(value).ToVector512();
	}

	[Intrinsic]
	public static Vector512<byte> CreateScalar(byte value)
	{
		return Vector512.CreateScalar<byte>(value);
	}

	[Intrinsic]
	public static Vector512<double> CreateScalar(double value)
	{
		return Vector512.CreateScalar<double>(value);
	}

	[Intrinsic]
	public static Vector512<short> CreateScalar(short value)
	{
		return Vector512.CreateScalar<short>(value);
	}

	[Intrinsic]
	public static Vector512<int> CreateScalar(int value)
	{
		return Vector512.CreateScalar<int>(value);
	}

	[Intrinsic]
	public static Vector512<long> CreateScalar(long value)
	{
		return Vector512.CreateScalar<long>(value);
	}

	[Intrinsic]
	public static Vector512<nint> CreateScalar(nint value)
	{
		return Vector512.CreateScalar<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<nuint> CreateScalar(nuint value)
	{
		return Vector512.CreateScalar<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> CreateScalar(sbyte value)
	{
		return Vector512.CreateScalar<sbyte>(value);
	}

	[Intrinsic]
	public static Vector512<float> CreateScalar(float value)
	{
		return Vector512.CreateScalar<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> CreateScalar(ushort value)
	{
		return Vector512.CreateScalar<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> CreateScalar(uint value)
	{
		return Vector512.CreateScalar<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> CreateScalar(ulong value)
	{
		return Vector512.CreateScalar<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> CreateScalarUnsafe<T>(T value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		Unsafe.SkipInit<Vector512<T>>(out var value2);
		SetElementUnsafe(in value2, 0, value);
		return value2;
	}

	[Intrinsic]
	public static Vector512<byte> CreateScalarUnsafe(byte value)
	{
		return Vector512.CreateScalarUnsafe<byte>(value);
	}

	[Intrinsic]
	public static Vector512<double> CreateScalarUnsafe(double value)
	{
		return Vector512.CreateScalarUnsafe<double>(value);
	}

	[Intrinsic]
	public static Vector512<short> CreateScalarUnsafe(short value)
	{
		return Vector512.CreateScalarUnsafe<short>(value);
	}

	[Intrinsic]
	public static Vector512<int> CreateScalarUnsafe(int value)
	{
		return Vector512.CreateScalarUnsafe<int>(value);
	}

	[Intrinsic]
	public static Vector512<long> CreateScalarUnsafe(long value)
	{
		return Vector512.CreateScalarUnsafe<long>(value);
	}

	[Intrinsic]
	public static Vector512<nint> CreateScalarUnsafe(nint value)
	{
		return Vector512.CreateScalarUnsafe<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<nuint> CreateScalarUnsafe(nuint value)
	{
		return Vector512.CreateScalarUnsafe<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> CreateScalarUnsafe(sbyte value)
	{
		return Vector512.CreateScalarUnsafe<sbyte>(value);
	}

	[Intrinsic]
	public static Vector512<float> CreateScalarUnsafe(float value)
	{
		return Vector512.CreateScalarUnsafe<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> CreateScalarUnsafe(ushort value)
	{
		return Vector512.CreateScalarUnsafe<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> CreateScalarUnsafe(uint value)
	{
		return Vector512.CreateScalarUnsafe<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> CreateScalarUnsafe(ulong value)
	{
		return Vector512.CreateScalarUnsafe<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> CreateSequence<T>(T start, T step)
	{
		return Vector512<T>.Indices * step + Create(start);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> DegreesToRadians(Vector512<double> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector512<double>, double>(degrees);
		}
		return Create(Vector256.DegreesToRadians(degrees._lower), Vector256.DegreesToRadians(degrees._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> DegreesToRadians(Vector512<float> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector512<float>, float>(degrees);
		}
		return Create(Vector256.DegreesToRadians(degrees._lower), Vector256.DegreesToRadians(degrees._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> Divide<T>(Vector512<T> left, Vector512<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector512<T> Divide<T>(Vector512<T> left, T right)
	{
		return left / right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Dot<T>(Vector512<T> left, Vector512<T> right)
	{
		return Sum(left * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> Equals<T>(Vector512<T> left, Vector512<T> right)
	{
		return Create(Vector256.Equals(left._lower, right._lower), Vector256.Equals(left._upper, right._upper));
	}

	[Intrinsic]
	public static bool EqualsAll<T>(Vector512<T> left, Vector512<T> right)
	{
		return left == right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny<T>(Vector512<T> left, Vector512<T> right)
	{
		if (!Vector256.EqualsAny(left._lower, right._lower))
		{
			return Vector256.EqualsAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<double> Exp(Vector512<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.ExpDouble<Vector512<double>, Vector512<ulong>>(vector);
		}
		return Create(Vector256.Exp(vector._lower), Vector256.Exp(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<float> Exp(Vector512<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.ExpSingle<Vector512<float>, Vector512<uint>, Vector512<double>, Vector512<ulong>>(vector);
		}
		return Create(Vector256.Exp(vector._lower), Vector256.Exp(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static ulong ExtractMostSignificantBits<T>(this Vector512<T> vector)
	{
		return vector._lower.ExtractMostSignificantBits() | ((ulong)vector._upper.ExtractMostSignificantBits() << Vector256<T>.Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector512<T> Floor<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector256.Floor(vector._lower), Vector256.Floor(vector._upper));
	}

	[Intrinsic]
	public static Vector512<float> Floor(Vector512<float> vector)
	{
		return Vector512.Floor<float>(vector);
	}

	[Intrinsic]
	public static Vector512<double> Floor(Vector512<double> vector)
	{
		return Vector512.Floor<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> FusedMultiplyAdd(Vector512<double> left, Vector512<double> right, Vector512<double> addend)
	{
		return Create(Vector256.FusedMultiplyAdd(left._lower, right._lower, addend._lower), Vector256.FusedMultiplyAdd(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> FusedMultiplyAdd(Vector512<float> left, Vector512<float> right, Vector512<float> addend)
	{
		return Create(Vector256.FusedMultiplyAdd(left._lower, right._lower, addend._lower), Vector256.FusedMultiplyAdd(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T GetElement<T>(this Vector512<T> vector, int index)
	{
		if ((uint)index >= (uint)Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return GetElementUnsafe(in vector, index);
	}

	[Intrinsic]
	public static Vector256<T> GetLower<T>(this Vector512<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		return vector._lower;
	}

	[Intrinsic]
	public static Vector256<T> GetUpper<T>(this Vector512<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		return vector._upper;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> GreaterThan<T>(Vector512<T> left, Vector512<T> right)
	{
		return Create(Vector256.GreaterThan(left._lower, right._lower), Vector256.GreaterThan(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll<T>(Vector512<T> left, Vector512<T> right)
	{
		if (Vector256.GreaterThanAll(left._lower, right._lower))
		{
			return Vector256.GreaterThanAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAny<T>(Vector512<T> left, Vector512<T> right)
	{
		if (!Vector256.GreaterThanAny(left._lower, right._lower))
		{
			return Vector256.GreaterThanAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> GreaterThanOrEqual<T>(Vector512<T> left, Vector512<T> right)
	{
		return Create(Vector256.GreaterThanOrEqual(left._lower, right._lower), Vector256.GreaterThanOrEqual(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll<T>(Vector512<T> left, Vector512<T> right)
	{
		if (Vector256.GreaterThanOrEqualAll(left._lower, right._lower))
		{
			return Vector256.GreaterThanOrEqualAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAny<T>(Vector512<T> left, Vector512<T> right)
	{
		if (!Vector256.GreaterThanOrEqualAny(left._lower, right._lower))
		{
			return Vector256.GreaterThanOrEqualAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<double> Hypot(Vector512<double> x, Vector512<double> y)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.HypotDouble<Vector512<double>, Vector512<ulong>>(x, y);
		}
		return Create(Vector256.Hypot(x._lower, y._lower), Vector256.Hypot(x._upper, y._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<float> Hypot(Vector512<float> x, Vector512<float> y)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.HypotSingle<Vector512<float>, Vector512<double>>(x, y);
		}
		return Create(Vector256.Hypot(x._lower, y._lower), Vector256.Hypot(x._upper, y._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf<T>(Vector512<T> vector, T value)
	{
		int num = BitOperations.TrailingZeroCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
		if (num == 64)
		{
			return -1;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOfWhereAllBitsSet<T>(Vector512<T> vector)
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
	public static Vector512<T> IsEvenInteger<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsEvenIntegerSingle<Vector512<float>, Vector512<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsEvenIntegerDouble<Vector512<double>, Vector512<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return IsZero(vector & Vector512<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsFinite<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return ~IsZero(AndNot(Vector512.Create<uint>(2139095040u), vector.AsUInt32())).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return ~IsZero(AndNot(Vector512.Create<ulong>(9218868437227405312uL), vector.AsUInt64())).As<ulong, T>();
		}
		return Vector512<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsInfinity<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsPositiveInfinity(Abs(vector));
		}
		return Vector512<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsInteger<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsFinite(vector) & Equals(vector, Truncate(vector));
		}
		return Vector512<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsNaN<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return ~Equals(vector, vector);
		}
		return Vector512<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsNegative<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector512<T>.Zero;
		}
		if (typeof(T) == typeof(float))
		{
			return LessThan(vector.AsInt32(), Vector512<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(vector.AsInt64(), Vector512<long>.Zero).As<long, T>();
		}
		return LessThan(vector, Vector512<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsNegativeInfinity<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.NegativeInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.NegativeInfinity).As<double, T>());
		}
		return Vector512<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsNormal<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector512.Create<uint>(8388608u), Vector512.Create<uint>(2130706432u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector512.Create<ulong>(4503599627370496uL), Vector512.Create<ulong>(9214364837600034816uL)).As<ulong, T>();
		}
		return ~IsZero(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsOddInteger<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsOddIntegerSingle<Vector512<float>, Vector512<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsOddIntegerDouble<Vector512<double>, Vector512<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return ~IsZero(vector & Vector512<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsPositive<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector512<T>.AllBitsSet;
		}
		if (typeof(T) == typeof(float))
		{
			return GreaterThanOrEqual(vector.AsInt32(), Vector512<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return GreaterThanOrEqual(vector.AsInt64(), Vector512<long>.Zero).As<long, T>();
		}
		return GreaterThanOrEqual(vector, Vector512<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsPositiveInfinity<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.PositiveInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.PositiveInfinity).As<double, T>());
		}
		return Vector512<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsSubnormal<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector512<uint>.One, Vector512.Create<uint>(8388607u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector512<ulong>.One, Vector512.Create<ulong>(4503599627370495uL)).As<ulong, T>();
		}
		return Vector512<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> IsZero<T>(Vector512<T> vector)
	{
		return Equals(vector, Vector512<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf<T>(Vector512<T> vector, T value)
	{
		return 63 - BitOperations.LeadingZeroCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet<T>(Vector512<T> vector)
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> Lerp(Vector512<double> x, Vector512<double> y, Vector512<double> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector512<double>, double>(x, y, amount);
		}
		return Create(Vector256.Lerp(x._lower, y._lower, amount._lower), Vector256.Lerp(x._upper, y._upper, amount._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> Lerp(Vector512<float> x, Vector512<float> y, Vector512<float> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector512<float>, float>(x, y, amount);
		}
		return Create(Vector256.Lerp(x._lower, y._lower, amount._lower), Vector256.Lerp(x._upper, y._upper, amount._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> LessThan<T>(Vector512<T> left, Vector512<T> right)
	{
		return Create(Vector256.LessThan(left._lower, right._lower), Vector256.LessThan(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll<T>(Vector512<T> left, Vector512<T> right)
	{
		if (Vector256.LessThanAll(left._lower, right._lower))
		{
			return Vector256.LessThanAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAny<T>(Vector512<T> left, Vector512<T> right)
	{
		if (!Vector256.LessThanAny(left._lower, right._lower))
		{
			return Vector256.LessThanAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> LessThanOrEqual<T>(Vector512<T> left, Vector512<T> right)
	{
		return Create(Vector256.LessThanOrEqual(left._lower, right._lower), Vector256.LessThanOrEqual(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll<T>(Vector512<T> left, Vector512<T> right)
	{
		if (Vector256.LessThanOrEqualAll(left._lower, right._lower))
		{
			return Vector256.LessThanOrEqualAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAny<T>(Vector512<T> left, Vector512<T> right)
	{
		if (!Vector256.LessThanOrEqualAny(left._lower, right._lower))
		{
			return Vector256.LessThanOrEqualAny(left._upper, right._upper);
		}
		return true;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector512<T> Load<T>(T* source)
	{
		return LoadUnsafe(in *source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector512<T> LoadAligned<T>(T* source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		if ((nuint)source % (nuint)64u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return *(Vector512<T>*)source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector512<T> LoadAlignedNonTemporal<T>(T* source)
	{
		return LoadAligned(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> LoadUnsafe<T>(ref readonly T source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		return Unsafe.ReadUnaligned<Vector512<T>>(in Unsafe.As<T, byte>(ref Unsafe.AsRef(in source)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<T> LoadUnsafe<T>(ref readonly T source, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		return Unsafe.ReadUnaligned<Vector512<T>>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)));
	}

	internal static Vector512<ushort> LoadUnsafe(ref char source)
	{
		return LoadUnsafe(in Unsafe.As<char, ushort>(ref source));
	}

	internal static Vector512<ushort> LoadUnsafe(ref char source, nuint elementOffset)
	{
		return LoadUnsafe(in Unsafe.As<char, ushort>(ref source), elementOffset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<double> Log(Vector512<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogDouble<Vector512<double>, Vector512<long>, Vector512<ulong>>(vector);
		}
		return Create(Vector256.Log(vector._lower), Vector256.Log(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<float> Log(Vector512<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogSingle<Vector512<float>, Vector512<int>, Vector512<uint>>(vector);
		}
		return Create(Vector256.Log(vector._lower), Vector256.Log(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<double> Log2(Vector512<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Double<Vector512<double>, Vector512<long>, Vector512<ulong>>(vector);
		}
		return Create(Vector256.Log2(vector._lower), Vector256.Log2(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<float> Log2(Vector512<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Single<Vector512<float>, Vector512<int>, Vector512<uint>>(vector);
		}
		return Create(Vector256.Log2(vector._lower), Vector256.Log2(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> Max<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Max<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.Max(left._lower, right._lower), Vector256.Max(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MaxMagnitude<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitude<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.MaxMagnitude(left._lower, right._lower), Vector256.MaxMagnitude(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MaxMagnitudeNumber<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitudeNumber<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.MaxMagnitudeNumber(left._lower, right._lower), Vector256.MaxMagnitudeNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MaxNative<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(GreaterThan(left, right), left, right);
		}
		return Create(Vector256.MaxNative(left._lower, right._lower), Vector256.MaxNative(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MaxNumber<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxNumber<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.MaxNumber(left._lower, right._lower), Vector256.MaxNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> Min<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Min<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.Min(left._lower, right._lower), Vector256.Min(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MinMagnitude<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitude<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.MinMagnitude(left._lower, right._lower), Vector256.MinMagnitude(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MinMagnitudeNumber<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitudeNumber<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.MinMagnitudeNumber(left._lower, right._lower), Vector256.MinMagnitudeNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MinNative<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(LessThan(left, right), left, right);
		}
		return Create(Vector256.MinNative(left._lower, right._lower), Vector256.MinNative(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> MinNumber<T>(Vector512<T> left, Vector512<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinNumber<Vector512<T>, T>(left, right);
		}
		return Create(Vector256.MinNumber(left._lower, right._lower), Vector256.MinNumber(left._upper, right._upper));
	}

	[Intrinsic]
	public static Vector512<T> Multiply<T>(Vector512<T> left, Vector512<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector512<T> Multiply<T>(Vector512<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector512<T> Multiply<T>(T left, Vector512<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector512<T> MultiplyAddEstimate<T>(Vector512<T> left, Vector512<T> right, Vector512<T> addend)
	{
		return Create(Vector256.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector256.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> MultiplyAddEstimate(Vector512<double> left, Vector512<double> right, Vector512<double> addend)
	{
		return Create(Vector256.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector256.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[Intrinsic]
	public static Vector512<float> MultiplyAddEstimate(Vector512<float> left, Vector512<float> right, Vector512<float> addend)
	{
		return Create(Vector256.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector256.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector512<TResult> Narrow<TSource, TResult>(Vector512<TSource> lower, Vector512<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector512<TResult>>(out var value);
		for (int i = 0; i < Vector512<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateTruncating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector512<TSource>.Count; j < Vector512<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateTruncating(GetElementUnsafe(in upper, j - Vector512<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> Narrow(Vector512<double> lower, Vector512<double> upper)
	{
		return Narrow<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> Narrow(Vector512<short> lower, Vector512<short> upper)
	{
		return Narrow<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<short> Narrow(Vector512<int> lower, Vector512<int> upper)
	{
		return Narrow<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<int> Narrow(Vector512<long> lower, Vector512<long> upper)
	{
		return Narrow<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<byte> Narrow(Vector512<ushort> lower, Vector512<ushort> upper)
	{
		return Narrow<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> Narrow(Vector512<uint> lower, Vector512<uint> upper)
	{
		return Narrow<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> Narrow(Vector512<ulong> lower, Vector512<ulong> upper)
	{
		return Narrow<ulong, uint>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector512<TResult> NarrowWithSaturation<TSource, TResult>(Vector512<TSource> lower, Vector512<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector512<TResult>>(out var value);
		for (int i = 0; i < Vector512<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateSaturating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector512<TSource>.Count; j < Vector512<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateSaturating(GetElementUnsafe(in upper, j - Vector512<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> NarrowWithSaturation(Vector512<double> lower, Vector512<double> upper)
	{
		return NarrowWithSaturation<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> NarrowWithSaturation(Vector512<short> lower, Vector512<short> upper)
	{
		return NarrowWithSaturation<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<short> NarrowWithSaturation(Vector512<int> lower, Vector512<int> upper)
	{
		return NarrowWithSaturation<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<int> NarrowWithSaturation(Vector512<long> lower, Vector512<long> upper)
	{
		return NarrowWithSaturation<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<byte> NarrowWithSaturation(Vector512<ushort> lower, Vector512<ushort> upper)
	{
		return NarrowWithSaturation<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> NarrowWithSaturation(Vector512<uint> lower, Vector512<uint> upper)
	{
		return NarrowWithSaturation<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> NarrowWithSaturation(Vector512<ulong> lower, Vector512<ulong> upper)
	{
		return NarrowWithSaturation<ulong, uint>(lower, upper);
	}

	[Intrinsic]
	public static Vector512<T> Negate<T>(Vector512<T> vector)
	{
		return -vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None<T>(Vector512<T> vector, T value)
	{
		return !EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet<T>(Vector512<T> vector)
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
	public static Vector512<T> OnesComplement<T>(Vector512<T> vector)
	{
		return ~vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> RadiansToDegrees(Vector512<double> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector512<double>, double>(radians);
		}
		return Create(Vector256.RadiansToDegrees(radians._lower), Vector256.RadiansToDegrees(radians._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<float> RadiansToDegrees(Vector512<float> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector512<float>, float>(radians);
		}
		return Create(Vector256.RadiansToDegrees(radians._lower), Vector256.RadiansToDegrees(radians._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector512<T> Round<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector256.Round(vector._lower), Vector256.Round(vector._upper));
	}

	[Intrinsic]
	public static Vector512<double> Round(Vector512<double> vector)
	{
		return Vector512.Round<double>(vector);
	}

	[Intrinsic]
	public static Vector512<float> Round(Vector512<float> vector)
	{
		return Vector512.Round<float>(vector);
	}

	[Intrinsic]
	public static Vector512<double> Round(Vector512<double> vector, MidpointRounding mode)
	{
		return VectorMath.RoundDouble(vector, mode);
	}

	[Intrinsic]
	public static Vector512<float> Round(Vector512<float> vector, MidpointRounding mode)
	{
		return VectorMath.RoundSingle(vector, mode);
	}

	[Intrinsic]
	public static Vector512<byte> ShiftLeft(Vector512<byte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector512<short> ShiftLeft(Vector512<short> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector512<int> ShiftLeft(Vector512<int> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector512<long> ShiftLeft(Vector512<long> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector512<nint> ShiftLeft(Vector512<nint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<nuint> ShiftLeft(Vector512<nuint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> ShiftLeft(Vector512<sbyte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> ShiftLeft(Vector512<ushort> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> ShiftLeft(Vector512<uint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector512<uint> ShiftLeft(Vector512<uint> vector, Vector512<uint> shiftCount)
	{
		return Create(Vector256.ShiftLeft(vector._lower, shiftCount._lower), Vector256.ShiftLeft(vector._upper, shiftCount._upper));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> ShiftLeft(Vector512<ulong> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector512<ulong> ShiftLeft(Vector512<ulong> vector, Vector512<ulong> shiftCount)
	{
		return Create(Vector256.ShiftLeft(vector._lower, shiftCount._lower), Vector256.ShiftLeft(vector._upper, shiftCount._upper));
	}

	[Intrinsic]
	public static Vector512<short> ShiftRightArithmetic(Vector512<short> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector512<int> ShiftRightArithmetic(Vector512<int> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector512<long> ShiftRightArithmetic(Vector512<long> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector512<nint> ShiftRightArithmetic(Vector512<nint> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> ShiftRightArithmetic(Vector512<sbyte> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector512<byte> ShiftRightLogical(Vector512<byte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector512<short> ShiftRightLogical(Vector512<short> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector512<int> ShiftRightLogical(Vector512<int> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector512<long> ShiftRightLogical(Vector512<long> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector512<nint> ShiftRightLogical(Vector512<nint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<nuint> ShiftRightLogical(Vector512<nuint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> ShiftRightLogical(Vector512<sbyte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> ShiftRightLogical(Vector512<ushort> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> ShiftRightLogical(Vector512<uint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> ShiftRightLogical(Vector512<ulong> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	internal static Vector512<byte> ShuffleNativeFallback(Vector512<byte> vector, Vector512<byte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<sbyte> ShuffleNativeFallback(Vector512<sbyte> vector, Vector512<sbyte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<short> ShuffleNativeFallback(Vector512<short> vector, Vector512<short> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<ushort> ShuffleNativeFallback(Vector512<ushort> vector, Vector512<ushort> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<int> ShuffleNativeFallback(Vector512<int> vector, Vector512<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<uint> ShuffleNativeFallback(Vector512<uint> vector, Vector512<uint> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<float> ShuffleNativeFallback(Vector512<float> vector, Vector512<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<long> ShuffleNativeFallback(Vector512<long> vector, Vector512<long> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<ulong> ShuffleNativeFallback(Vector512<ulong> vector, Vector512<ulong> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector512<double> ShuffleNativeFallback(Vector512<double> vector, Vector512<long> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	public static Vector512<byte> Shuffle(Vector512<byte> vector, Vector512<byte> indices)
	{
		Unsafe.SkipInit<Vector512<byte>>(out var value);
		for (int i = 0; i < Vector512<byte>.Count; i++)
		{
			byte elementUnsafe = GetElementUnsafe(in indices, i);
			byte value2 = 0;
			if (elementUnsafe < Vector512<byte>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> Shuffle(Vector512<sbyte> vector, Vector512<sbyte> indices)
	{
		Unsafe.SkipInit<Vector512<sbyte>>(out var value);
		for (int i = 0; i < Vector512<sbyte>.Count; i++)
		{
			byte b = (byte)GetElementUnsafe(in indices, i);
			sbyte value2 = 0;
			if (b < Vector512<sbyte>.Count)
			{
				value2 = GetElementUnsafe(in vector, b);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector512<byte> ShuffleNative(Vector512<byte> vector, Vector512<byte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<sbyte> ShuffleNative(Vector512<sbyte> vector, Vector512<sbyte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector512<short> Shuffle(Vector512<short> vector, Vector512<short> indices)
	{
		Unsafe.SkipInit<Vector512<short>>(out var value);
		for (int i = 0; i < Vector512<short>.Count; i++)
		{
			ushort num = (ushort)GetElementUnsafe(in indices, i);
			short value2 = 0;
			if (num < Vector512<short>.Count)
			{
				value2 = GetElementUnsafe(in vector, num);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> Shuffle(Vector512<ushort> vector, Vector512<ushort> indices)
	{
		Unsafe.SkipInit<Vector512<ushort>>(out var value);
		for (int i = 0; i < Vector512<ushort>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in indices, i);
			ushort value2 = 0;
			if (elementUnsafe < Vector512<ushort>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector512<short> ShuffleNative(Vector512<short> vector, Vector512<short> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> ShuffleNative(Vector512<ushort> vector, Vector512<ushort> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector512<int> Shuffle(Vector512<int> vector, Vector512<int> indices)
	{
		Unsafe.SkipInit<Vector512<int>>(out var value);
		for (int i = 0; i < Vector512<int>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			int value2 = 0;
			if (elementUnsafe < Vector512<int>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> Shuffle(Vector512<uint> vector, Vector512<uint> indices)
	{
		Unsafe.SkipInit<Vector512<uint>>(out var value);
		for (int i = 0; i < Vector512<uint>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in indices, i);
			uint value2 = 0u;
			if (elementUnsafe < Vector512<uint>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector512<float> Shuffle(Vector512<float> vector, Vector512<int> indices)
	{
		Unsafe.SkipInit<Vector512<float>>(out var value);
		for (int i = 0; i < Vector512<float>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			float value2 = 0f;
			if (elementUnsafe < Vector512<float>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector512<int> ShuffleNative(Vector512<int> vector, Vector512<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> ShuffleNative(Vector512<uint> vector, Vector512<uint> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector512<float> ShuffleNative(Vector512<float> vector, Vector512<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector512<long> Shuffle(Vector512<long> vector, Vector512<long> indices)
	{
		Unsafe.SkipInit<Vector512<long>>(out var value);
		for (int i = 0; i < Vector512<long>.Count; i++)
		{
			ulong elementUnsafe = (ulong)GetElementUnsafe(in indices, i);
			long value2 = 0L;
			if (elementUnsafe < (uint)Vector512<long>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> Shuffle(Vector512<ulong> vector, Vector512<ulong> indices)
	{
		Unsafe.SkipInit<Vector512<ulong>>(out var value);
		for (int i = 0; i < Vector512<ulong>.Count; i++)
		{
			ulong elementUnsafe = GetElementUnsafe(in indices, i);
			ulong value2 = 0uL;
			if (elementUnsafe < (uint)Vector512<ulong>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector512<double> Shuffle(Vector512<double> vector, Vector512<long> indices)
	{
		Unsafe.SkipInit<Vector512<double>>(out var value);
		for (int i = 0; i < Vector512<double>.Count; i++)
		{
			ulong elementUnsafe = (ulong)GetElementUnsafe(in indices, i);
			double value2 = 0.0;
			if (elementUnsafe < (uint)Vector512<double>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector512<long> ShuffleNative(Vector512<long> vector, Vector512<long> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> ShuffleNative(Vector512<ulong> vector, Vector512<ulong> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector512<double> ShuffleNative(Vector512<double> vector, Vector512<long> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<double> Sin(Vector512<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinDouble<Vector512<double>, Vector512<long>>(vector);
		}
		return Create(Vector256.Sin(vector._lower), Vector256.Sin(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector512<float> Sin(Vector512<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinSingle<Vector512<float>, Vector512<int>, Vector512<double>, Vector512<long>>(vector);
		}
		return Create(Vector256.Sin(vector._lower), Vector256.Sin(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector512<double> Sin, Vector512<double> Cos) SinCos(Vector512<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinCosDouble<Vector512<double>, Vector512<long>>(vector);
		}
		var (lower, lower2) = Vector256.SinCos(vector._lower);
		var (upper, upper2) = Vector256.SinCos(vector._upper);
		return (Sin: Create(lower, upper), Cos: Create(lower2, upper2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector512<float> Sin, Vector512<float> Cos) SinCos(Vector512<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinCosSingle<Vector512<float>, Vector512<int>, Vector512<double>, Vector512<long>>(vector);
		}
		var (lower, lower2) = Vector256.SinCos(vector._lower);
		var (upper, upper2) = Vector256.SinCos(vector._upper);
		return (Sin: Create(lower, upper), Cos: Create(lower2, upper2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> Sqrt<T>(Vector512<T> vector)
	{
		return Create(Vector256.Sqrt(vector._lower), Vector256.Sqrt(vector._upper));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void Store<T>(this Vector512<T> source, T* destination)
	{
		source.StoreUnsafe(ref *destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned<T>(this Vector512<T> source, T* destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		if ((nuint)destination % (nuint)64u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		*(Vector512<T>*)destination = source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal<T>(this Vector512<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static void StoreUnsafe<T>(this Vector512<T> source, ref T destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static void StoreUnsafe<T>(this Vector512<T> source, ref T destination, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[Intrinsic]
	public static Vector512<T> Subtract<T>(Vector512<T> left, Vector512<T> right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> SubtractSaturate<T>(Vector512<T> left, Vector512<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left - right;
		}
		return Create(Vector256.SubtractSaturate(left._lower, right._lower), Vector256.SubtractSaturate(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Sum<T>(Vector512<T> vector)
	{
		return Scalar<T>.Add(Vector256.Sum(vector._lower), Vector256.Sum(vector._upper));
	}

	[Intrinsic]
	public static T ToScalar<T>(this Vector512<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		return GetElementUnsafe(in vector, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector512<T> Truncate<T>(Vector512<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector256.Truncate(vector._lower), Vector256.Truncate(vector._upper));
	}

	[Intrinsic]
	public static Vector512<double> Truncate(Vector512<double> vector)
	{
		return Vector512.Truncate<double>(vector);
	}

	[Intrinsic]
	public static Vector512<float> Truncate(Vector512<float> vector)
	{
		return Vector512.Truncate<float>(vector);
	}

	public static bool TryCopyTo<T>(this Vector512<T> vector, Span<T> destination)
	{
		if ((uint)destination.Length < (uint)Vector512<T>.Count)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector512<ushort> Lower, Vector512<ushort> Upper) Widen(Vector512<byte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector512<int> Lower, Vector512<int> Upper) Widen(Vector512<short> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector512<long> Lower, Vector512<long> Upper) Widen(Vector512<int> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector512<short> Lower, Vector512<short> Upper) Widen(Vector512<sbyte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector512<double> Lower, Vector512<double> Upper) Widen(Vector512<float> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector512<uint> Lower, Vector512<uint> Upper) Widen(Vector512<ushort> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector512<ulong> Lower, Vector512<ulong> Upper) Widen(Vector512<uint> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> WidenLower(Vector512<byte> source)
	{
		Vector256<byte> lower = source._lower;
		return Create(Vector256.WidenLower(lower), Vector256.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<int> WidenLower(Vector512<short> source)
	{
		Vector256<short> lower = source._lower;
		return Create(Vector256.WidenLower(lower), Vector256.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<long> WidenLower(Vector512<int> source)
	{
		Vector256<int> lower = source._lower;
		return Create(Vector256.WidenLower(lower), Vector256.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<short> WidenLower(Vector512<sbyte> source)
	{
		Vector256<sbyte> lower = source._lower;
		return Create(Vector256.WidenLower(lower), Vector256.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> WidenLower(Vector512<float> source)
	{
		Vector256<float> lower = source._lower;
		return Create(Vector256.WidenLower(lower), Vector256.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> WidenLower(Vector512<ushort> source)
	{
		Vector256<ushort> lower = source._lower;
		return Create(Vector256.WidenLower(lower), Vector256.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> WidenLower(Vector512<uint> source)
	{
		Vector256<uint> lower = source._lower;
		return Create(Vector256.WidenLower(lower), Vector256.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ushort> WidenUpper(Vector512<byte> source)
	{
		Vector256<byte> upper = source._upper;
		return Create(Vector256.WidenLower(upper), Vector256.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<int> WidenUpper(Vector512<short> source)
	{
		Vector256<short> upper = source._upper;
		return Create(Vector256.WidenLower(upper), Vector256.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<long> WidenUpper(Vector512<int> source)
	{
		Vector256<int> upper = source._upper;
		return Create(Vector256.WidenLower(upper), Vector256.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<short> WidenUpper(Vector512<sbyte> source)
	{
		Vector256<sbyte> upper = source._upper;
		return Create(Vector256.WidenLower(upper), Vector256.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<double> WidenUpper(Vector512<float> source)
	{
		Vector256<float> upper = source._upper;
		return Create(Vector256.WidenLower(upper), Vector256.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<uint> WidenUpper(Vector512<ushort> source)
	{
		Vector256<ushort> upper = source._upper;
		return Create(Vector256.WidenLower(upper), Vector256.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector512<ulong> WidenUpper(Vector512<uint> source)
	{
		Vector256<uint> upper = source._upper;
		return Create(Vector256.WidenLower(upper), Vector256.WidenUpper(upper));
	}

	[Intrinsic]
	public static Vector512<T> WithElement<T>(this Vector512<T> vector, int index, T value)
	{
		if ((uint)index >= (uint)Vector512<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		Vector512<T> vector2 = vector;
		SetElementUnsafe(in vector2, index, value);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> WithLower<T>(this Vector512<T> vector, Vector256<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		Vector512<T> vector2 = vector;
		vector2.SetLowerUnsafe<T>(value);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> WithUpper<T>(this Vector512<T> vector, Vector256<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		Vector512<T> vector2 = vector;
		vector2.SetUpperUnsafe<T>(value);
		return vector2;
	}

	[Intrinsic]
	public static Vector512<T> Xor<T>(Vector512<T> left, Vector512<T> right)
	{
		return left ^ right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T GetElementUnsafe<T>(this in Vector512<T> vector, int index)
	{
		return Unsafe.Add(ref Unsafe.As<Vector512<T>, T>(ref Unsafe.AsRef(in vector)), index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetElementUnsafe<T>(this in Vector512<T> vector, int index, T value)
	{
		Unsafe.Add(ref Unsafe.As<Vector512<T>, T>(ref Unsafe.AsRef(in vector)), index) = value;
	}

	internal static void SetLowerUnsafe<T>(this in Vector512<T> vector, Vector256<T> value)
	{
		Unsafe.AsRef(in vector._lower) = value;
	}

	internal static void SetUpperUnsafe<T>(this in Vector512<T> vector, Vector256<T> value)
	{
		Unsafe.AsRef(in vector._upper) = value;
	}
}
[StructLayout(LayoutKind.Sequential, Size = 64)]
[Intrinsic]
[DebuggerDisplay("{DisplayString,nq}")]
[DebuggerTypeProxy(typeof(Vector512DebugView<>))]
public readonly struct Vector512<T> : ISimdVector<Vector512<T>, T>, IAdditionOperators<Vector512<T>, Vector512<T>, Vector512<T>>, IBitwiseOperators<Vector512<T>, Vector512<T>, Vector512<T>>, IDivisionOperators<Vector512<T>, Vector512<T>, Vector512<T>>, IEqualityOperators<Vector512<T>, Vector512<T>, bool>, IEquatable<Vector512<T>>, IMultiplyOperators<Vector512<T>, Vector512<T>, Vector512<T>>, IShiftOperators<Vector512<T>, int, Vector512<T>>, ISubtractionOperators<Vector512<T>, Vector512<T>, Vector512<T>>, IUnaryNegationOperators<Vector512<T>, Vector512<T>>, IUnaryPlusOperators<Vector512<T>, Vector512<T>>
{
	internal readonly Vector256<T> _lower;

	internal readonly Vector256<T> _upper;

	public static Vector512<T> AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector512.Create(Scalar<T>.AllBitsSet);
		}
	}

	public static int Count
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
			return 64 / Unsafe.SizeOf<T>();
		}
	}

	public static Vector512<T> Indices
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
			Unsafe.SkipInit<Vector512<T>>(out var value);
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

	public static Vector512<T> One
	{
		[Intrinsic]
		get
		{
			return Vector512.Create(Scalar<T>.One);
		}
	}

	public static Vector512<T> Zero
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
			return default(Vector512<T>);
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

	static int ISimdVector<Vector512<T>, T>.Alignment => 64;

	static int ISimdVector<Vector512<T>, T>.ElementCount => Count;

	static bool ISimdVector<Vector512<T>, T>.IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return Vector512.IsHardwareAccelerated;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator +(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Create(left._lower + right._lower, left._upper + right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator &(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Create(left._lower & right._lower, left._upper & right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator |(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Create(left._lower | right._lower, left._upper | right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator /(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Create(left._lower / right._lower, left._upper / right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator /(Vector512<T> left, T right)
	{
		return Vector512.Create(left._lower / right, left._upper / right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector512<T> left, Vector512<T> right)
	{
		if (left._lower == right._lower)
		{
			return left._upper == right._upper;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator ^(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Create(left._lower ^ right._lower, left._upper ^ right._upper);
	}

	[Intrinsic]
	public static bool operator !=(Vector512<T> left, Vector512<T> right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator <<(Vector512<T> value, int shiftCount)
	{
		return Vector512.Create(value._lower << shiftCount, value._upper << shiftCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator *(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Create(left._lower * right._lower, left._upper * right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator *(Vector512<T> left, T right)
	{
		return Vector512.Create(left._lower * right, left._upper * right);
	}

	[Intrinsic]
	public static Vector512<T> operator *(T left, Vector512<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator ~(Vector512<T> vector)
	{
		return Vector512.Create(~vector._lower, ~vector._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator >>(Vector512<T> value, int shiftCount)
	{
		return Vector512.Create(value._lower >> shiftCount, value._upper >> shiftCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator -(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Create(left._lower - right._lower, left._upper - right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator -(Vector512<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return vector ^ Vector512.Create(-0f).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return vector ^ Vector512.Create(-0.0).As<double, T>();
		}
		return Zero - vector;
	}

	[Intrinsic]
	public static Vector512<T> operator +(Vector512<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector512<T> operator >>>(Vector512<T> value, int shiftCount)
	{
		return Vector512.Create(value._lower >>> shiftCount, value._upper >>> shiftCount);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector512<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector512<T> other)
	{
		if (Vector512.IsHardwareAccelerated)
		{
			if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
			{
				return (Vector512.Equals(this, other) | ~(Vector512.Equals(this, this) | Vector512.Equals(other, other))).AsInt32() == Vector512<int>.AllBitsSet;
			}
			return this == other;
		}
		if (_lower.Equals(other._lower))
		{
			return _upper.Equals(other._upper);
		}
		return false;
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
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector512BaseType<T>();
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
	static Vector512<T> ISimdVector<Vector512<T>, T>.Abs(Vector512<T> vector)
	{
		return Vector512.Abs(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Add(Vector512<T> left, Vector512<T> right)
	{
		return left + right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.All(Vector512<T> vector, T value)
	{
		return Vector512.All(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.AllWhereAllBitsSet(Vector512<T> vector)
	{
		return Vector512.AllWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.AndNot(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.AndNot(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.Any(Vector512<T> vector, T value)
	{
		return Vector512.Any(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.AnyWhereAllBitsSet(Vector512<T> vector)
	{
		return Vector512.AnyWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.BitwiseAnd(Vector512<T> left, Vector512<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.BitwiseOr(Vector512<T> left, Vector512<T> right)
	{
		return left | right;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Ceiling(Vector512<T> vector)
	{
		return Vector512.Ceiling(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Clamp(Vector512<T> value, Vector512<T> min, Vector512<T> max)
	{
		return Vector512.Clamp(value, min, max);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.ClampNative(Vector512<T> value, Vector512<T> min, Vector512<T> max)
	{
		return Vector512.ClampNative(value, min, max);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.ConditionalSelect(Vector512<T> condition, Vector512<T> left, Vector512<T> right)
	{
		return Vector512.ConditionalSelect(condition, left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.CopySign(Vector512<T> value, Vector512<T> sign)
	{
		return Vector512.CopySign(value, sign);
	}

	static void ISimdVector<Vector512<T>, T>.CopyTo(Vector512<T> vector, T[] destination)
	{
		vector.CopyTo(destination);
	}

	static void ISimdVector<Vector512<T>, T>.CopyTo(Vector512<T> vector, T[] destination, int startIndex)
	{
		vector.CopyTo(destination, startIndex);
	}

	static void ISimdVector<Vector512<T>, T>.CopyTo(Vector512<T> vector, Span<T> destination)
	{
		vector.CopyTo(destination);
	}

	[Intrinsic]
	static int ISimdVector<Vector512<T>, T>.Count(Vector512<T> vector, T value)
	{
		return Vector512.Count(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector512<T>, T>.CountWhereAllBitsSet(Vector512<T> vector)
	{
		return Vector512.CountWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Create(T value)
	{
		return Vector512.Create(value);
	}

	static Vector512<T> ISimdVector<Vector512<T>, T>.Create(T[] values)
	{
		return Vector512.Create(values);
	}

	static Vector512<T> ISimdVector<Vector512<T>, T>.Create(T[] values, int index)
	{
		return Vector512.Create(values, index);
	}

	static Vector512<T> ISimdVector<Vector512<T>, T>.Create(ReadOnlySpan<T> values)
	{
		return Vector512.Create(values);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.CreateScalar(T value)
	{
		return Vector512.CreateScalar(value);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.CreateScalarUnsafe(T value)
	{
		return Vector512.CreateScalarUnsafe(value);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Divide(Vector512<T> left, Vector512<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Divide(Vector512<T> left, T right)
	{
		return left / right;
	}

	[Intrinsic]
	static T ISimdVector<Vector512<T>, T>.Dot(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Dot(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Equals(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Equals(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.EqualsAll(Vector512<T> left, Vector512<T> right)
	{
		return left == right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.EqualsAny(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.EqualsAny(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Floor(Vector512<T> vector)
	{
		return Vector512.Floor(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector512<T>, T>.GetElement(Vector512<T> vector, int index)
	{
		return vector.GetElement(index);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.GreaterThan(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.GreaterThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.GreaterThanAll(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.GreaterThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.GreaterThanAny(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.GreaterThanAny(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.GreaterThanOrEqual(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.GreaterThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.GreaterThanOrEqualAll(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.GreaterThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.GreaterThanOrEqualAny(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.GreaterThanOrEqualAny(left, right);
	}

	[Intrinsic]
	static int ISimdVector<Vector512<T>, T>.IndexOf(Vector512<T> vector, T value)
	{
		return Vector512.IndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector512<T>, T>.IndexOfWhereAllBitsSet(Vector512<T> vector)
	{
		return Vector512.IndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsEvenInteger(Vector512<T> vector)
	{
		return Vector512.IsEvenInteger(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsFinite(Vector512<T> vector)
	{
		return Vector512.IsFinite(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsInfinity(Vector512<T> vector)
	{
		return Vector512.IsInfinity(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsInteger(Vector512<T> vector)
	{
		return Vector512.IsInteger(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsNaN(Vector512<T> vector)
	{
		return Vector512.IsNaN(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsNegative(Vector512<T> vector)
	{
		return Vector512.IsNegative(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsNegativeInfinity(Vector512<T> vector)
	{
		return Vector512.IsNegativeInfinity(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsNormal(Vector512<T> vector)
	{
		return Vector512.IsNormal(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsOddInteger(Vector512<T> vector)
	{
		return Vector512.IsOddInteger(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsPositive(Vector512<T> vector)
	{
		return Vector512.IsPositive(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsPositiveInfinity(Vector512<T> vector)
	{
		return Vector512.IsPositiveInfinity(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.IsSubnormal(Vector512<T> vector)
	{
		return Vector512.IsSubnormal(vector);
	}

	static Vector512<T> ISimdVector<Vector512<T>, T>.IsZero(Vector512<T> vector)
	{
		return Vector512.IsZero(vector);
	}

	[Intrinsic]
	static int ISimdVector<Vector512<T>, T>.LastIndexOf(Vector512<T> vector, T value)
	{
		return Vector512.LastIndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector512<T>, T>.LastIndexOfWhereAllBitsSet(Vector512<T> vector)
	{
		return Vector512.LastIndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.LessThan(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.LessThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.LessThanAll(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.LessThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.LessThanAny(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.LessThanAny(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.LessThanOrEqual(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.LessThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.LessThanOrEqualAll(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.LessThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.LessThanOrEqualAny(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.LessThanOrEqualAny(left, right);
	}

	[Intrinsic]
	unsafe static Vector512<T> ISimdVector<Vector512<T>, T>.Load(T* source)
	{
		return Vector512.Load(source);
	}

	[Intrinsic]
	unsafe static Vector512<T> ISimdVector<Vector512<T>, T>.LoadAligned(T* source)
	{
		return Vector512.LoadAligned(source);
	}

	[Intrinsic]
	unsafe static Vector512<T> ISimdVector<Vector512<T>, T>.LoadAlignedNonTemporal(T* source)
	{
		return Vector512.LoadAlignedNonTemporal(source);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.LoadUnsafe(ref readonly T source)
	{
		return Vector512.LoadUnsafe(in source);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.LoadUnsafe(ref readonly T source, nuint elementOffset)
	{
		return Vector512.LoadUnsafe(in source, elementOffset);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Max(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Max(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MaxMagnitude(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MaxMagnitude(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MaxMagnitudeNumber(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MaxMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MaxNative(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MaxNative(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MaxNumber(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MaxNumber(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Min(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.Min(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MinMagnitude(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MinMagnitude(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MinMagnitudeNumber(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MinMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MinNative(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MinNative(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MinNumber(Vector512<T> left, Vector512<T> right)
	{
		return Vector512.MinNumber(left, right);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Multiply(Vector512<T> left, Vector512<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Multiply(Vector512<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.MultiplyAddEstimate(Vector512<T> left, Vector512<T> right, Vector512<T> addend)
	{
		return Vector512.MultiplyAddEstimate(left, right, addend);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Negate(Vector512<T> vector)
	{
		return -vector;
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.None(Vector512<T> vector, T value)
	{
		return Vector512.None(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector512<T>, T>.NoneWhereAllBitsSet(Vector512<T> vector)
	{
		return Vector512.NoneWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.OnesComplement(Vector512<T> vector)
	{
		return ~vector;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Round(Vector512<T> vector)
	{
		return Vector512.Round(vector);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.ShiftLeft(Vector512<T> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.ShiftRightArithmetic(Vector512<T> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.ShiftRightLogical(Vector512<T> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Sqrt(Vector512<T> vector)
	{
		return Vector512.Sqrt(vector);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector512<T>, T>.Store(Vector512<T> source, T* destination)
	{
		source.Store(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector512<T>, T>.StoreAligned(Vector512<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector512<T>, T>.StoreAlignedNonTemporal(Vector512<T> source, T* destination)
	{
		source.StoreAlignedNonTemporal(destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector512<T>, T>.StoreUnsafe(Vector512<T> vector, ref T destination)
	{
		vector.StoreUnsafe(ref destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector512<T>, T>.StoreUnsafe(Vector512<T> vector, ref T destination, nuint elementOffset)
	{
		vector.StoreUnsafe(ref destination, elementOffset);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Subtract(Vector512<T> left, Vector512<T> right)
	{
		return left - right;
	}

	[Intrinsic]
	static T ISimdVector<Vector512<T>, T>.Sum(Vector512<T> vector)
	{
		return Vector512.Sum(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector512<T>, T>.ToScalar(Vector512<T> vector)
	{
		return vector.ToScalar();
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Truncate(Vector512<T> vector)
	{
		return Vector512.Truncate(vector);
	}

	static bool ISimdVector<Vector512<T>, T>.TryCopyTo(Vector512<T> vector, Span<T> destination)
	{
		return vector.TryCopyTo(destination);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.WithElement(Vector512<T> vector, int index, T value)
	{
		return vector.WithElement(index, value);
	}

	[Intrinsic]
	static Vector512<T> ISimdVector<Vector512<T>, T>.Xor(Vector512<T> left, Vector512<T> right)
	{
		return left ^ right;
	}
}
