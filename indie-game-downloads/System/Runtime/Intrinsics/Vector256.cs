using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace System.Runtime.Intrinsics;

public static class Vector256
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
	public static Vector256<T> Abs<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return vector;
		}
		return Create(Vector128.Abs(vector._lower), Vector128.Abs(vector._upper));
	}

	[Intrinsic]
	public static Vector256<T> Add<T>(Vector256<T> left, Vector256<T> right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> AddSaturate<T>(Vector256<T> left, Vector256<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left + right;
		}
		return Create(Vector128.AddSaturate(left._lower, right._lower), Vector128.AddSaturate(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All<T>(Vector256<T> vector, T value)
	{
		return vector == Create(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet<T>(Vector256<T> vector)
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
	public static Vector256<T> AndNot<T>(Vector256<T> left, Vector256<T> right)
	{
		return left & ~right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any<T>(Vector256<T> vector, T value)
	{
		return EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet<T>(Vector256<T> vector)
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
	public static Vector256<TTo> As<TFrom, TTo>(this Vector256<TFrom> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<TFrom>();
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<TTo>();
		return Unsafe.BitCast<Vector256<TFrom>, Vector256<TTo>>(vector);
	}

	[Intrinsic]
	public static Vector256<byte> AsByte<T>(this Vector256<T> vector)
	{
		return vector.As<T, byte>();
	}

	[Intrinsic]
	public static Vector256<double> AsDouble<T>(this Vector256<T> vector)
	{
		return vector.As<T, double>();
	}

	[Intrinsic]
	public static Vector256<short> AsInt16<T>(this Vector256<T> vector)
	{
		return vector.As<T, short>();
	}

	[Intrinsic]
	public static Vector256<int> AsInt32<T>(this Vector256<T> vector)
	{
		return vector.As<T, int>();
	}

	[Intrinsic]
	public static Vector256<long> AsInt64<T>(this Vector256<T> vector)
	{
		return vector.As<T, long>();
	}

	[Intrinsic]
	public static Vector256<nint> AsNInt<T>(this Vector256<T> vector)
	{
		return vector.As<T, nint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<nuint> AsNUInt<T>(this Vector256<T> vector)
	{
		return vector.As<T, nuint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> AsSByte<T>(this Vector256<T> vector)
	{
		return vector.As<T, sbyte>();
	}

	[Intrinsic]
	public static Vector256<float> AsSingle<T>(this Vector256<T> vector)
	{
		return vector.As<T, float>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> AsUInt16<T>(this Vector256<T> vector)
	{
		return vector.As<T, ushort>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> AsUInt32<T>(this Vector256<T> vector)
	{
		return vector.As<T, uint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> AsUInt64<T>(this Vector256<T> vector)
	{
		return vector.As<T, ulong>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> AsVector256<T>(this Vector<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		if (Vector<T>.Count >= Vector256<T>.Count)
		{
			return Unsafe.ReadUnaligned<Vector256<T>>(in Unsafe.As<Vector<T>, byte>(ref value));
		}
		Vector256<T> source = default(Vector256<T>);
		Unsafe.WriteUnaligned(ref Unsafe.As<Vector256<T>, byte>(ref source), value);
		return source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> AsVector<T>(this Vector256<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		if (Vector256<T>.Count >= Vector<T>.Count)
		{
			return Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<Vector256<T>, byte>(ref value));
		}
		Vector<T> source = default(Vector<T>);
		Unsafe.WriteUnaligned(ref Unsafe.As<Vector<T>, byte>(ref source), value);
		return source;
	}

	[Intrinsic]
	public static Vector256<T> BitwiseAnd<T>(Vector256<T> left, Vector256<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector256<T> BitwiseOr<T>(Vector256<T> left, Vector256<T> right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector256<T> Ceiling<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector128.Ceiling(vector._lower), Vector128.Ceiling(vector._upper));
	}

	[Intrinsic]
	public static Vector256<float> Ceiling(Vector256<float> vector)
	{
		return Vector256.Ceiling<float>(vector);
	}

	[Intrinsic]
	public static Vector256<double> Ceiling(Vector256<double> vector)
	{
		return Vector256.Ceiling<double>(vector);
	}

	[Intrinsic]
	public static Vector256<T> Clamp<T>(Vector256<T> value, Vector256<T> min, Vector256<T> max)
	{
		return Min(Max(value, min), max);
	}

	[Intrinsic]
	public static Vector256<T> ClampNative<T>(Vector256<T> value, Vector256<T> min, Vector256<T> max)
	{
		return MinNative(MaxNative(value, min), max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> ConditionalSelect<T>(Vector256<T> condition, Vector256<T> left, Vector256<T> right)
	{
		return (left & condition) | AndNot(right, condition);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> ConvertToDouble(Vector256<long> vector)
	{
		if (Avx2.IsSupported)
		{
			Vector256<int> left = vector.AsInt32();
			left = Avx2.Blend(left, Create(4841369599423283200L).AsInt32(), 170);
			return Avx.Add(Avx.Subtract(Avx2.Xor(Avx2.ShiftRightLogical(vector, 32), Create(4985484789646622720L)).AsDouble(), Create(4985484789647671296L).AsDouble()), left.AsDouble());
		}
		return Create(Vector128.ConvertToDouble(vector._lower), Vector128.ConvertToDouble(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<double> ConvertToDouble(Vector256<ulong> vector)
	{
		if (Avx2.IsSupported)
		{
			Vector256<uint> left = vector.AsUInt32();
			left = Avx2.Blend(left, Create(4841369599423283200uL).AsUInt32(), 170);
			return Avx.Add(Avx.Subtract(Avx2.Xor(Avx2.ShiftRightLogical(vector, 32), Create(4985484787499139072uL)).AsDouble(), Create(4985484787500187648uL).AsDouble()), left.AsDouble());
		}
		return Create(Vector128.ConvertToDouble(vector._lower), Vector128.ConvertToDouble(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<int> ConvertToInt32(Vector256<float> vector)
	{
		return Create(Vector128.ConvertToInt32(vector._lower), Vector128.ConvertToInt32(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<int> ConvertToInt32Native(Vector256<float> vector)
	{
		return Create(Vector128.ConvertToInt32Native(vector._lower), Vector128.ConvertToInt32Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<long> ConvertToInt64(Vector256<double> vector)
	{
		return Create(Vector128.ConvertToInt64(vector._lower), Vector128.ConvertToInt64(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<long> ConvertToInt64Native(Vector256<double> vector)
	{
		return Create(Vector128.ConvertToInt64Native(vector._lower), Vector128.ConvertToInt64Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> ConvertToSingle(Vector256<int> vector)
	{
		return Create(Vector128.ConvertToSingle(vector._lower), Vector128.ConvertToSingle(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<float> ConvertToSingle(Vector256<uint> vector)
	{
		if (Avx2.IsSupported)
		{
			Vector256<int> value = Avx2.And(vector, Create(65535u)).AsInt32();
			Vector256<int> value2 = Avx2.ShiftRightLogical(vector, 16).AsInt32();
			Vector256<float> vector2 = Avx.ConvertToVector256Single(value);
			Vector256<float> vector3 = Avx.ConvertToVector256Single(value2);
			if (Fma.IsSupported)
			{
				return Fma.MultiplyAdd(vector3, Create(65536f), vector2);
			}
			return Avx.Add(Avx.Multiply(vector3, Create(65536f)), vector2);
		}
		return Create(Vector128.ConvertToSingle(vector._lower), Vector128.ConvertToSingle(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> ConvertToUInt32(Vector256<float> vector)
	{
		return Create(Vector128.ConvertToUInt32(vector._lower), Vector128.ConvertToUInt32(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> ConvertToUInt32Native(Vector256<float> vector)
	{
		return Create(Vector128.ConvertToUInt32Native(vector._lower), Vector128.ConvertToUInt32Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> ConvertToUInt64(Vector256<double> vector)
	{
		return Create(Vector128.ConvertToUInt64(vector._lower), Vector128.ConvertToUInt64(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> ConvertToUInt64Native(Vector256<double> vector)
	{
		return Create(Vector128.ConvertToUInt64Native(vector._lower), Vector128.ConvertToUInt64Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> CopySign<T>(Vector256<T> value, Vector256<T> sign)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return value;
		}
		if (IsHardwareAccelerated)
		{
			return VectorMath.CopySign<Vector256<T>, T>(value, sign);
		}
		return Create(Vector128.CopySign(value._lower, sign._lower), Vector128.CopySign(value._upper, sign._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector256<T> vector, T[] destination)
	{
		if (destination.Length < Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[0]), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector256<T> vector, T[] destination, int startIndex)
	{
		if ((uint)startIndex >= (uint)destination.Length)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
		}
		if (destination.Length - startIndex < Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[startIndex]), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector256<T> vector, Span<T> destination)
	{
		if (destination.Length < Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<double> Cos(Vector256<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.CosDouble<Vector256<double>, Vector256<long>>(vector);
		}
		return Create(Vector128.Cos(vector._lower), Vector128.Cos(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<float> Cos(Vector256<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector512.IsHardwareAccelerated)
			{
				return VectorMath.CosSingle<Vector256<float>, Vector256<int>, Vector512<double>, Vector512<long>>(vector);
			}
			return VectorMath.CosSingle<Vector256<float>, Vector256<int>, Vector256<double>, Vector256<long>>(vector);
		}
		return Create(Vector128.Cos(vector._lower), Vector128.Cos(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count<T>(Vector256<T> vector, T value)
	{
		return BitOperations.PopCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet<T>(Vector256<T> vector)
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
	public static Vector256<T> Create<T>(T value)
	{
		Vector128<T> vector = Vector128.Create(value);
		return Create(vector, vector);
	}

	[Intrinsic]
	public static Vector256<byte> Create(byte value)
	{
		return Vector256.Create<byte>(value);
	}

	[Intrinsic]
	public static Vector256<double> Create(double value)
	{
		return Vector256.Create<double>(value);
	}

	[Intrinsic]
	public static Vector256<short> Create(short value)
	{
		return Vector256.Create<short>(value);
	}

	[Intrinsic]
	public static Vector256<int> Create(int value)
	{
		return Vector256.Create<int>(value);
	}

	[Intrinsic]
	public static Vector256<long> Create(long value)
	{
		return Vector256.Create<long>(value);
	}

	[Intrinsic]
	public static Vector256<nint> Create(nint value)
	{
		return Vector256.Create<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<nuint> Create(nuint value)
	{
		return Vector256.Create<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> Create(sbyte value)
	{
		return Vector256.Create<sbyte>(value);
	}

	[Intrinsic]
	public static Vector256<float> Create(float value)
	{
		return Vector256.Create<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> Create(ushort value)
	{
		return Vector256.Create<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> Create(uint value)
	{
		return Vector256.Create<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> Create(ulong value)
	{
		return Vector256.Create<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<T> Create<T>(T[] values)
	{
		if (values.Length < Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector256<T>>(in Unsafe.As<T, byte>(ref values[0]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<T> Create<T>(T[] values, int index)
	{
		if (index < 0 || values.Length - index < Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector256<T>>(in Unsafe.As<T, byte>(ref values[index]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<T> Create<T>(ReadOnlySpan<T> values)
	{
		if (values.Length < Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return Unsafe.ReadUnaligned<Vector256<T>>(in Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<byte> Create(byte e0, byte e1, byte e2, byte e3, byte e4, byte e5, byte e6, byte e7, byte e8, byte e9, byte e10, byte e11, byte e12, byte e13, byte e14, byte e15, byte e16, byte e17, byte e18, byte e19, byte e20, byte e21, byte e22, byte e23, byte e24, byte e25, byte e26, byte e27, byte e28, byte e29, byte e30, byte e31)
	{
		return Create(Vector128.Create(e0, e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15), Vector128.Create(e16, e17, e18, e19, e20, e21, e22, e23, e24, e25, e26, e27, e28, e29, e30, e31));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> Create(double e0, double e1, double e2, double e3)
	{
		return Create(Vector128.Create(e0, e1), Vector128.Create(e2, e3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<short> Create(short e0, short e1, short e2, short e3, short e4, short e5, short e6, short e7, short e8, short e9, short e10, short e11, short e12, short e13, short e14, short e15)
	{
		return Create(Vector128.Create(e0, e1, e2, e3, e4, e5, e6, e7), Vector128.Create(e8, e9, e10, e11, e12, e13, e14, e15));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<int> Create(int e0, int e1, int e2, int e3, int e4, int e5, int e6, int e7)
	{
		return Create(Vector128.Create(e0, e1, e2, e3), Vector128.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<long> Create(long e0, long e1, long e2, long e3)
	{
		return Create(Vector128.Create(e0, e1), Vector128.Create(e2, e3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> Create(sbyte e0, sbyte e1, sbyte e2, sbyte e3, sbyte e4, sbyte e5, sbyte e6, sbyte e7, sbyte e8, sbyte e9, sbyte e10, sbyte e11, sbyte e12, sbyte e13, sbyte e14, sbyte e15, sbyte e16, sbyte e17, sbyte e18, sbyte e19, sbyte e20, sbyte e21, sbyte e22, sbyte e23, sbyte e24, sbyte e25, sbyte e26, sbyte e27, sbyte e28, sbyte e29, sbyte e30, sbyte e31)
	{
		return Create(Vector128.Create(e0, e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15), Vector128.Create(e16, e17, e18, e19, e20, e21, e22, e23, e24, e25, e26, e27, e28, e29, e30, e31));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> Create(float e0, float e1, float e2, float e3, float e4, float e5, float e6, float e7)
	{
		return Create(Vector128.Create(e0, e1, e2, e3), Vector128.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> Create(ushort e0, ushort e1, ushort e2, ushort e3, ushort e4, ushort e5, ushort e6, ushort e7, ushort e8, ushort e9, ushort e10, ushort e11, ushort e12, ushort e13, ushort e14, ushort e15)
	{
		return Create(Vector128.Create(e0, e1, e2, e3, e4, e5, e6, e7), Vector128.Create(e8, e9, e10, e11, e12, e13, e14, e15));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> Create(uint e0, uint e1, uint e2, uint e3, uint e4, uint e5, uint e6, uint e7)
	{
		return Create(Vector128.Create(e0, e1, e2, e3), Vector128.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> Create(ulong e0, ulong e1, ulong e2, ulong e3)
	{
		return Create(Vector128.Create(e0, e1), Vector128.Create(e2, e3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<T> Create<T>(Vector64<T> value)
	{
		return Create(Vector128.Create(value, value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<T> Create<T>(Vector128<T> value)
	{
		return Create(value, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<T> Create<T>(Vector128<T> lower, Vector128<T> upper)
	{
		if (Avx.IsSupported)
		{
			return lower.ToVector256Unsafe().WithUpper(upper);
		}
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		Unsafe.SkipInit<Vector256<T>>(out var value);
		value.SetLowerUnsafe<T>(lower);
		value.SetUpperUnsafe<T>(upper);
		return value;
	}

	public static Vector256<byte> Create(Vector128<byte> lower, Vector128<byte> upper)
	{
		return Vector256.Create<byte>(lower, upper);
	}

	public static Vector256<double> Create(Vector128<double> lower, Vector128<double> upper)
	{
		return Vector256.Create<double>(lower, upper);
	}

	public static Vector256<short> Create(Vector128<short> lower, Vector128<short> upper)
	{
		return Vector256.Create<short>(lower, upper);
	}

	public static Vector256<int> Create(Vector128<int> lower, Vector128<int> upper)
	{
		return Vector256.Create<int>(lower, upper);
	}

	public static Vector256<long> Create(Vector128<long> lower, Vector128<long> upper)
	{
		return Vector256.Create<long>(lower, upper);
	}

	public static Vector256<nint> Create(Vector128<nint> lower, Vector128<nint> upper)
	{
		return Vector256.Create<nint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector256<nuint> Create(Vector128<nuint> lower, Vector128<nuint> upper)
	{
		return Vector256.Create<nuint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector256<sbyte> Create(Vector128<sbyte> lower, Vector128<sbyte> upper)
	{
		return Vector256.Create<sbyte>(lower, upper);
	}

	public static Vector256<float> Create(Vector128<float> lower, Vector128<float> upper)
	{
		return Vector256.Create<float>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector256<ushort> Create(Vector128<ushort> lower, Vector128<ushort> upper)
	{
		return Vector256.Create<ushort>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector256<uint> Create(Vector128<uint> lower, Vector128<uint> upper)
	{
		return Vector256.Create<uint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector256<ulong> Create(Vector128<ulong> lower, Vector128<ulong> upper)
	{
		return Vector256.Create<ulong>(lower, upper);
	}

	[Intrinsic]
	public static Vector256<T> CreateScalar<T>(T value)
	{
		return Vector128.CreateScalar(value).ToVector256();
	}

	[Intrinsic]
	public static Vector256<byte> CreateScalar(byte value)
	{
		return Vector256.CreateScalar<byte>(value);
	}

	[Intrinsic]
	public static Vector256<double> CreateScalar(double value)
	{
		return Vector256.CreateScalar<double>(value);
	}

	[Intrinsic]
	public static Vector256<short> CreateScalar(short value)
	{
		return Vector256.CreateScalar<short>(value);
	}

	[Intrinsic]
	public static Vector256<int> CreateScalar(int value)
	{
		return Vector256.CreateScalar<int>(value);
	}

	[Intrinsic]
	public static Vector256<long> CreateScalar(long value)
	{
		return Vector256.CreateScalar<long>(value);
	}

	[Intrinsic]
	public static Vector256<nint> CreateScalar(nint value)
	{
		return Vector256.CreateScalar<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<nuint> CreateScalar(nuint value)
	{
		return Vector256.CreateScalar<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> CreateScalar(sbyte value)
	{
		return Vector256.CreateScalar<sbyte>(value);
	}

	[Intrinsic]
	public static Vector256<float> CreateScalar(float value)
	{
		return Vector256.CreateScalar<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> CreateScalar(ushort value)
	{
		return Vector256.CreateScalar<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> CreateScalar(uint value)
	{
		return Vector256.CreateScalar<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> CreateScalar(ulong value)
	{
		return Vector256.CreateScalar<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> CreateScalarUnsafe<T>(T value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		Unsafe.SkipInit<Vector256<T>>(out var value2);
		SetElementUnsafe(in value2, 0, value);
		return value2;
	}

	[Intrinsic]
	public static Vector256<byte> CreateScalarUnsafe(byte value)
	{
		return Vector256.CreateScalarUnsafe<byte>(value);
	}

	[Intrinsic]
	public static Vector256<double> CreateScalarUnsafe(double value)
	{
		return Vector256.CreateScalarUnsafe<double>(value);
	}

	[Intrinsic]
	public static Vector256<short> CreateScalarUnsafe(short value)
	{
		return Vector256.CreateScalarUnsafe<short>(value);
	}

	[Intrinsic]
	public static Vector256<int> CreateScalarUnsafe(int value)
	{
		return Vector256.CreateScalarUnsafe<int>(value);
	}

	[Intrinsic]
	public static Vector256<long> CreateScalarUnsafe(long value)
	{
		return Vector256.CreateScalarUnsafe<long>(value);
	}

	[Intrinsic]
	public static Vector256<nint> CreateScalarUnsafe(nint value)
	{
		return Vector256.CreateScalarUnsafe<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<nuint> CreateScalarUnsafe(nuint value)
	{
		return Vector256.CreateScalarUnsafe<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> CreateScalarUnsafe(sbyte value)
	{
		return Vector256.CreateScalarUnsafe<sbyte>(value);
	}

	[Intrinsic]
	public static Vector256<float> CreateScalarUnsafe(float value)
	{
		return Vector256.CreateScalarUnsafe<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> CreateScalarUnsafe(ushort value)
	{
		return Vector256.CreateScalarUnsafe<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> CreateScalarUnsafe(uint value)
	{
		return Vector256.CreateScalarUnsafe<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> CreateScalarUnsafe(ulong value)
	{
		return Vector256.CreateScalarUnsafe<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> CreateSequence<T>(T start, T step)
	{
		return Vector256<T>.Indices * step + Create(start);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> DegreesToRadians(Vector256<double> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector256<double>, double>(degrees);
		}
		return Create(Vector128.DegreesToRadians(degrees._lower), Vector128.DegreesToRadians(degrees._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> DegreesToRadians(Vector256<float> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector256<float>, float>(degrees);
		}
		return Create(Vector128.DegreesToRadians(degrees._lower), Vector128.DegreesToRadians(degrees._upper));
	}

	[Intrinsic]
	public static Vector256<T> Divide<T>(Vector256<T> left, Vector256<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector256<T> Divide<T>(Vector256<T> left, T right)
	{
		return left / right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Dot<T>(Vector256<T> left, Vector256<T> right)
	{
		return Sum(left * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> Equals<T>(Vector256<T> left, Vector256<T> right)
	{
		return Create(Vector128.Equals(left._lower, right._lower), Vector128.Equals(left._upper, right._upper));
	}

	[Intrinsic]
	public static bool EqualsAll<T>(Vector256<T> left, Vector256<T> right)
	{
		return left == right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny<T>(Vector256<T> left, Vector256<T> right)
	{
		if (!Vector128.EqualsAny(left._lower, right._lower))
		{
			return Vector128.EqualsAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<double> Exp(Vector256<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.ExpDouble<Vector256<double>, Vector256<ulong>>(vector);
		}
		return Create(Vector128.Exp(vector._lower), Vector128.Exp(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<float> Exp(Vector256<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector512.IsHardwareAccelerated)
			{
				return VectorMath.ExpSingle<Vector256<float>, Vector256<uint>, Vector512<double>, Vector512<ulong>>(vector);
			}
			return VectorMath.ExpSingle<Vector256<float>, Vector256<uint>, Vector256<double>, Vector256<ulong>>(vector);
		}
		return Create(Vector128.Exp(vector._lower), Vector128.Exp(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static uint ExtractMostSignificantBits<T>(this Vector256<T> vector)
	{
		return vector._lower.ExtractMostSignificantBits() | (vector._upper.ExtractMostSignificantBits() << Vector128<T>.Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector256<T> Floor<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector128.Floor(vector._lower), Vector128.Floor(vector._upper));
	}

	[Intrinsic]
	public static Vector256<float> Floor(Vector256<float> vector)
	{
		return Vector256.Floor<float>(vector);
	}

	[Intrinsic]
	public static Vector256<double> Floor(Vector256<double> vector)
	{
		return Vector256.Floor<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> FusedMultiplyAdd(Vector256<double> left, Vector256<double> right, Vector256<double> addend)
	{
		return Create(Vector128.FusedMultiplyAdd(left._lower, right._lower, addend._lower), Vector128.FusedMultiplyAdd(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> FusedMultiplyAdd(Vector256<float> left, Vector256<float> right, Vector256<float> addend)
	{
		return Create(Vector128.FusedMultiplyAdd(left._lower, right._lower, addend._lower), Vector128.FusedMultiplyAdd(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T GetElement<T>(this Vector256<T> vector, int index)
	{
		if ((uint)index >= (uint)Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return GetElementUnsafe(in vector, index);
	}

	[Intrinsic]
	public static Vector128<T> GetLower<T>(this Vector256<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		return vector._lower;
	}

	[Intrinsic]
	public static Vector128<T> GetUpper<T>(this Vector256<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		return vector._upper;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> GreaterThan<T>(Vector256<T> left, Vector256<T> right)
	{
		return Create(Vector128.GreaterThan(left._lower, right._lower), Vector128.GreaterThan(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll<T>(Vector256<T> left, Vector256<T> right)
	{
		if (Vector128.GreaterThanAll(left._lower, right._lower))
		{
			return Vector128.GreaterThanAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAny<T>(Vector256<T> left, Vector256<T> right)
	{
		if (!Vector128.GreaterThanAny(left._lower, right._lower))
		{
			return Vector128.GreaterThanAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> GreaterThanOrEqual<T>(Vector256<T> left, Vector256<T> right)
	{
		return Create(Vector128.GreaterThanOrEqual(left._lower, right._lower), Vector128.GreaterThanOrEqual(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll<T>(Vector256<T> left, Vector256<T> right)
	{
		if (Vector128.GreaterThanOrEqualAll(left._lower, right._lower))
		{
			return Vector128.GreaterThanOrEqualAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAny<T>(Vector256<T> left, Vector256<T> right)
	{
		if (!Vector128.GreaterThanOrEqualAny(left._lower, right._lower))
		{
			return Vector128.GreaterThanOrEqualAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<double> Hypot(Vector256<double> x, Vector256<double> y)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.HypotDouble<Vector256<double>, Vector256<ulong>>(x, y);
		}
		return Create(Vector128.Hypot(x._lower, y._lower), Vector128.Hypot(x._upper, y._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<float> Hypot(Vector256<float> x, Vector256<float> y)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector512.IsHardwareAccelerated)
			{
				return VectorMath.HypotSingle<Vector256<float>, Vector512<double>>(x, y);
			}
			return VectorMath.HypotSingle<Vector256<float>, Vector256<double>>(x, y);
		}
		return Create(Vector128.Hypot(x._lower, y._lower), Vector128.Hypot(x._upper, y._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf<T>(Vector256<T> vector, T value)
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
	public static int IndexOfWhereAllBitsSet<T>(Vector256<T> vector)
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
	public static Vector256<T> IsEvenInteger<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsEvenIntegerSingle<Vector256<float>, Vector256<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsEvenIntegerDouble<Vector256<double>, Vector256<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return IsZero(vector & Vector256<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsFinite<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return ~IsZero(AndNot(Vector256.Create<uint>(2139095040u), vector.AsUInt32())).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return ~IsZero(AndNot(Vector256.Create<ulong>(9218868437227405312uL), vector.AsUInt64())).As<ulong, T>();
		}
		return Vector256<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsInfinity<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsPositiveInfinity(Abs(vector));
		}
		return Vector256<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsInteger<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsFinite(vector) & Equals(vector, Truncate(vector));
		}
		return Vector256<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsNaN<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return ~Equals(vector, vector);
		}
		return Vector256<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsNegative<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector256<T>.Zero;
		}
		if (typeof(T) == typeof(float))
		{
			return LessThan(vector.AsInt32(), Vector256<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(vector.AsInt64(), Vector256<long>.Zero).As<long, T>();
		}
		return LessThan(vector, Vector256<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsNegativeInfinity<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.NegativeInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.NegativeInfinity).As<double, T>());
		}
		return Vector256<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsNormal<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector256.Create<uint>(8388608u), Vector256.Create<uint>(2130706432u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector256.Create<ulong>(4503599627370496uL), Vector256.Create<ulong>(9214364837600034816uL)).As<ulong, T>();
		}
		return ~IsZero(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsOddInteger<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsOddIntegerSingle<Vector256<float>, Vector256<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsOddIntegerDouble<Vector256<double>, Vector256<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return ~IsZero(vector & Vector256<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsPositive<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector256<T>.AllBitsSet;
		}
		if (typeof(T) == typeof(float))
		{
			return GreaterThanOrEqual(vector.AsInt32(), Vector256<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return GreaterThanOrEqual(vector.AsInt64(), Vector256<long>.Zero).As<long, T>();
		}
		return GreaterThanOrEqual(vector, Vector256<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsPositiveInfinity<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.PositiveInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.PositiveInfinity).As<double, T>());
		}
		return Vector256<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsSubnormal<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector256<uint>.One, Vector256.Create<uint>(8388607u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector256<ulong>.One, Vector256.Create<ulong>(4503599627370495uL)).As<ulong, T>();
		}
		return Vector256<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> IsZero<T>(Vector256<T> vector)
	{
		return Equals(vector, Vector256<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf<T>(Vector256<T> vector, T value)
	{
		return 31 - BitOperations.LeadingZeroCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet<T>(Vector256<T> vector)
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
	public static Vector256<double> Lerp(Vector256<double> x, Vector256<double> y, Vector256<double> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector256<double>, double>(x, y, amount);
		}
		return Create(Vector128.Lerp(x._lower, y._lower, amount._lower), Vector128.Lerp(x._upper, y._upper, amount._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> Lerp(Vector256<float> x, Vector256<float> y, Vector256<float> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector256<float>, float>(x, y, amount);
		}
		return Create(Vector128.Lerp(x._lower, y._lower, amount._lower), Vector128.Lerp(x._upper, y._upper, amount._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> LessThan<T>(Vector256<T> left, Vector256<T> right)
	{
		return Create(Vector128.LessThan(left._lower, right._lower), Vector128.LessThan(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll<T>(Vector256<T> left, Vector256<T> right)
	{
		if (Vector128.LessThanAll(left._lower, right._lower))
		{
			return Vector128.LessThanAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAny<T>(Vector256<T> left, Vector256<T> right)
	{
		if (!Vector128.LessThanAny(left._lower, right._lower))
		{
			return Vector128.LessThanAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> LessThanOrEqual<T>(Vector256<T> left, Vector256<T> right)
	{
		return Create(Vector128.LessThanOrEqual(left._lower, right._lower), Vector128.LessThanOrEqual(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll<T>(Vector256<T> left, Vector256<T> right)
	{
		if (Vector128.LessThanOrEqualAll(left._lower, right._lower))
		{
			return Vector128.LessThanOrEqualAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAny<T>(Vector256<T> left, Vector256<T> right)
	{
		if (!Vector128.LessThanOrEqualAny(left._lower, right._lower))
		{
			return Vector128.LessThanOrEqualAny(left._upper, right._upper);
		}
		return true;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector256<T> Load<T>(T* source)
	{
		return LoadUnsafe(in *source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector256<T> LoadAligned<T>(T* source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		if ((nuint)source % (nuint)32u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return *(Vector256<T>*)source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector256<T> LoadAlignedNonTemporal<T>(T* source)
	{
		return LoadAligned(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> LoadUnsafe<T>(ref readonly T source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		return Unsafe.ReadUnaligned<Vector256<T>>(in Unsafe.As<T, byte>(ref Unsafe.AsRef(in source)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<T> LoadUnsafe<T>(ref readonly T source, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		return Unsafe.ReadUnaligned<Vector256<T>>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)));
	}

	internal static Vector256<ushort> LoadUnsafe(ref char source)
	{
		return LoadUnsafe(in Unsafe.As<char, ushort>(ref source));
	}

	internal static Vector256<ushort> LoadUnsafe(ref char source, nuint elementOffset)
	{
		return LoadUnsafe(in Unsafe.As<char, ushort>(ref source), elementOffset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<double> Log(Vector256<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogDouble<Vector256<double>, Vector256<long>, Vector256<ulong>>(vector);
		}
		return Create(Vector128.Log(vector._lower), Vector128.Log(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<float> Log(Vector256<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogSingle<Vector256<float>, Vector256<int>, Vector256<uint>>(vector);
		}
		return Create(Vector128.Log(vector._lower), Vector128.Log(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<double> Log2(Vector256<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Double<Vector256<double>, Vector256<long>, Vector256<ulong>>(vector);
		}
		return Create(Vector128.Log2(vector._lower), Vector128.Log2(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<float> Log2(Vector256<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Single<Vector256<float>, Vector256<int>, Vector256<uint>>(vector);
		}
		return Create(Vector128.Log2(vector._lower), Vector128.Log2(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> Max<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Max<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.Max(left._lower, right._lower), Vector128.Max(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MaxMagnitude<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitude<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.MaxMagnitude(left._lower, right._lower), Vector128.MaxMagnitude(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MaxMagnitudeNumber<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitudeNumber<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.MaxMagnitudeNumber(left._lower, right._lower), Vector128.MaxMagnitudeNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MaxNative<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(GreaterThan(left, right), left, right);
		}
		return Create(Vector128.MaxNative(left._lower, right._lower), Vector128.MaxNative(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MaxNumber<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxNumber<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.MaxNumber(left._lower, right._lower), Vector128.MaxNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> Min<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Min<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.Min(left._lower, right._lower), Vector128.Min(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MinMagnitude<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitude<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.MinMagnitude(left._lower, right._lower), Vector128.MinMagnitude(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MinMagnitudeNumber<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitudeNumber<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.MinMagnitudeNumber(left._lower, right._lower), Vector128.MinMagnitudeNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MinNative<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(LessThan(left, right), left, right);
		}
		return Create(Vector128.MinNative(left._lower, right._lower), Vector128.MinNative(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> MinNumber<T>(Vector256<T> left, Vector256<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinNumber<Vector256<T>, T>(left, right);
		}
		return Create(Vector128.MinNumber(left._lower, right._lower), Vector128.MinNumber(left._upper, right._upper));
	}

	[Intrinsic]
	public static Vector256<T> Multiply<T>(Vector256<T> left, Vector256<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector256<T> Multiply<T>(Vector256<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector256<T> Multiply<T>(T left, Vector256<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector256<T> MultiplyAddEstimate<T>(Vector256<T> left, Vector256<T> right, Vector256<T> addend)
	{
		return Create(Vector128.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector128.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> MultiplyAddEstimate(Vector256<double> left, Vector256<double> right, Vector256<double> addend)
	{
		return Create(Vector128.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector128.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> MultiplyAddEstimate(Vector256<float> left, Vector256<float> right, Vector256<float> addend)
	{
		return Create(Vector128.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector128.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector256<TResult> Narrow<TSource, TResult>(Vector256<TSource> lower, Vector256<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector256<TResult>>(out var value);
		for (int i = 0; i < Vector256<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateTruncating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector256<TSource>.Count; j < Vector256<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateTruncating(GetElementUnsafe(in upper, j - Vector256<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> Narrow(Vector256<double> lower, Vector256<double> upper)
	{
		return Narrow<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> Narrow(Vector256<short> lower, Vector256<short> upper)
	{
		return Narrow<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<short> Narrow(Vector256<int> lower, Vector256<int> upper)
	{
		return Narrow<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<int> Narrow(Vector256<long> lower, Vector256<long> upper)
	{
		return Narrow<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<byte> Narrow(Vector256<ushort> lower, Vector256<ushort> upper)
	{
		return Narrow<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> Narrow(Vector256<uint> lower, Vector256<uint> upper)
	{
		return Narrow<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> Narrow(Vector256<ulong> lower, Vector256<ulong> upper)
	{
		return Narrow<ulong, uint>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector256<TResult> NarrowWithSaturation<TSource, TResult>(Vector256<TSource> lower, Vector256<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector256<TResult>>(out var value);
		for (int i = 0; i < Vector256<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateSaturating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector256<TSource>.Count; j < Vector256<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateSaturating(GetElementUnsafe(in upper, j - Vector256<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> NarrowWithSaturation(Vector256<double> lower, Vector256<double> upper)
	{
		return NarrowWithSaturation<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> NarrowWithSaturation(Vector256<short> lower, Vector256<short> upper)
	{
		return NarrowWithSaturation<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<short> NarrowWithSaturation(Vector256<int> lower, Vector256<int> upper)
	{
		return NarrowWithSaturation<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<int> NarrowWithSaturation(Vector256<long> lower, Vector256<long> upper)
	{
		return NarrowWithSaturation<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<byte> NarrowWithSaturation(Vector256<ushort> lower, Vector256<ushort> upper)
	{
		return NarrowWithSaturation<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> NarrowWithSaturation(Vector256<uint> lower, Vector256<uint> upper)
	{
		return NarrowWithSaturation<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> NarrowWithSaturation(Vector256<ulong> lower, Vector256<ulong> upper)
	{
		return NarrowWithSaturation<ulong, uint>(lower, upper);
	}

	[Intrinsic]
	public static Vector256<T> Negate<T>(Vector256<T> vector)
	{
		return -vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None<T>(Vector256<T> vector, T value)
	{
		return !EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet<T>(Vector256<T> vector)
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
	public static Vector256<T> OnesComplement<T>(Vector256<T> vector)
	{
		return ~vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> RadiansToDegrees(Vector256<double> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector256<double>, double>(radians);
		}
		return Create(Vector128.RadiansToDegrees(radians._lower), Vector128.RadiansToDegrees(radians._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<float> RadiansToDegrees(Vector256<float> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector256<float>, float>(radians);
		}
		return Create(Vector128.RadiansToDegrees(radians._lower), Vector128.RadiansToDegrees(radians._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector256<T> Round<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector128.Round(vector._lower), Vector128.Round(vector._upper));
	}

	[Intrinsic]
	public static Vector256<double> Round(Vector256<double> vector)
	{
		return Vector256.Round<double>(vector);
	}

	[Intrinsic]
	public static Vector256<float> Round(Vector256<float> vector)
	{
		return Vector256.Round<float>(vector);
	}

	[Intrinsic]
	public static Vector256<double> Round(Vector256<double> vector, MidpointRounding mode)
	{
		return VectorMath.RoundDouble(vector, mode);
	}

	[Intrinsic]
	public static Vector256<float> Round(Vector256<float> vector, MidpointRounding mode)
	{
		return VectorMath.RoundSingle(vector, mode);
	}

	[Intrinsic]
	public static Vector256<byte> ShiftLeft(Vector256<byte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector256<short> ShiftLeft(Vector256<short> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector256<int> ShiftLeft(Vector256<int> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector256<long> ShiftLeft(Vector256<long> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector256<nint> ShiftLeft(Vector256<nint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<nuint> ShiftLeft(Vector256<nuint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> ShiftLeft(Vector256<sbyte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> ShiftLeft(Vector256<ushort> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> ShiftLeft(Vector256<uint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector256<uint> ShiftLeft(Vector256<uint> vector, Vector256<uint> shiftCount)
	{
		return Create(Vector128.ShiftLeft(vector._lower, shiftCount._lower), Vector128.ShiftLeft(vector._upper, shiftCount._upper));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> ShiftLeft(Vector256<ulong> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector256<ulong> ShiftLeft(Vector256<ulong> vector, Vector256<ulong> shiftCount)
	{
		return Create(Vector128.ShiftLeft(vector._lower, shiftCount._lower), Vector128.ShiftLeft(vector._upper, shiftCount._upper));
	}

	[Intrinsic]
	public static Vector256<short> ShiftRightArithmetic(Vector256<short> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector256<int> ShiftRightArithmetic(Vector256<int> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector256<long> ShiftRightArithmetic(Vector256<long> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector256<nint> ShiftRightArithmetic(Vector256<nint> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> ShiftRightArithmetic(Vector256<sbyte> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector256<byte> ShiftRightLogical(Vector256<byte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector256<short> ShiftRightLogical(Vector256<short> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector256<int> ShiftRightLogical(Vector256<int> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector256<long> ShiftRightLogical(Vector256<long> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector256<nint> ShiftRightLogical(Vector256<nint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<nuint> ShiftRightLogical(Vector256<nuint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> ShiftRightLogical(Vector256<sbyte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> ShiftRightLogical(Vector256<ushort> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> ShiftRightLogical(Vector256<uint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> ShiftRightLogical(Vector256<ulong> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	internal static Vector256<byte> ShuffleNativeFallback(Vector256<byte> vector, Vector256<byte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<sbyte> ShuffleNativeFallback(Vector256<sbyte> vector, Vector256<sbyte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<short> ShuffleNativeFallback(Vector256<short> vector, Vector256<short> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<ushort> ShuffleNativeFallback(Vector256<ushort> vector, Vector256<ushort> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<int> ShuffleNativeFallback(Vector256<int> vector, Vector256<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<uint> ShuffleNativeFallback(Vector256<uint> vector, Vector256<uint> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<float> ShuffleNativeFallback(Vector256<float> vector, Vector256<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<long> ShuffleNativeFallback(Vector256<long> vector, Vector256<long> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<ulong> ShuffleNativeFallback(Vector256<ulong> vector, Vector256<ulong> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector256<double> ShuffleNativeFallback(Vector256<double> vector, Vector256<long> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	public static Vector256<byte> Shuffle(Vector256<byte> vector, Vector256<byte> indices)
	{
		Unsafe.SkipInit<Vector256<byte>>(out var value);
		for (int i = 0; i < Vector256<byte>.Count; i++)
		{
			byte elementUnsafe = GetElementUnsafe(in indices, i);
			byte value2 = 0;
			if (elementUnsafe < Vector256<byte>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> Shuffle(Vector256<sbyte> vector, Vector256<sbyte> indices)
	{
		Unsafe.SkipInit<Vector256<sbyte>>(out var value);
		for (int i = 0; i < Vector256<sbyte>.Count; i++)
		{
			byte b = (byte)GetElementUnsafe(in indices, i);
			sbyte value2 = 0;
			if (b < Vector256<sbyte>.Count)
			{
				value2 = GetElementUnsafe(in vector, b);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector256<byte> ShuffleNative(Vector256<byte> vector, Vector256<byte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<sbyte> ShuffleNative(Vector256<sbyte> vector, Vector256<sbyte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector256<short> Shuffle(Vector256<short> vector, Vector256<short> indices)
	{
		Unsafe.SkipInit<Vector256<short>>(out var value);
		for (int i = 0; i < Vector256<short>.Count; i++)
		{
			ushort num = (ushort)GetElementUnsafe(in indices, i);
			short value2 = 0;
			if (num < Vector256<short>.Count)
			{
				value2 = GetElementUnsafe(in vector, num);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> Shuffle(Vector256<ushort> vector, Vector256<ushort> indices)
	{
		Unsafe.SkipInit<Vector256<ushort>>(out var value);
		for (int i = 0; i < Vector256<ushort>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in indices, i);
			ushort value2 = 0;
			if (elementUnsafe < Vector256<ushort>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector256<short> ShuffleNative(Vector256<short> vector, Vector256<short> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> ShuffleNative(Vector256<ushort> vector, Vector256<ushort> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector256<int> Shuffle(Vector256<int> vector, Vector256<int> indices)
	{
		Unsafe.SkipInit<Vector256<int>>(out var value);
		for (int i = 0; i < Vector256<int>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			int value2 = 0;
			if (elementUnsafe < Vector256<int>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> Shuffle(Vector256<uint> vector, Vector256<uint> indices)
	{
		Unsafe.SkipInit<Vector256<uint>>(out var value);
		for (int i = 0; i < Vector256<uint>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in indices, i);
			uint value2 = 0u;
			if (elementUnsafe < Vector256<uint>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector256<float> Shuffle(Vector256<float> vector, Vector256<int> indices)
	{
		Unsafe.SkipInit<Vector256<float>>(out var value);
		for (int i = 0; i < Vector256<float>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			float value2 = 0f;
			if (elementUnsafe < Vector256<float>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector256<int> ShuffleNative(Vector256<int> vector, Vector256<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> ShuffleNative(Vector256<uint> vector, Vector256<uint> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector256<float> ShuffleNative(Vector256<float> vector, Vector256<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector256<long> Shuffle(Vector256<long> vector, Vector256<long> indices)
	{
		Unsafe.SkipInit<Vector256<long>>(out var value);
		for (int i = 0; i < Vector256<long>.Count; i++)
		{
			ulong elementUnsafe = (ulong)GetElementUnsafe(in indices, i);
			long value2 = 0L;
			if (elementUnsafe < (uint)Vector256<long>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> Shuffle(Vector256<ulong> vector, Vector256<ulong> indices)
	{
		Unsafe.SkipInit<Vector256<ulong>>(out var value);
		for (int i = 0; i < Vector256<ulong>.Count; i++)
		{
			ulong elementUnsafe = GetElementUnsafe(in indices, i);
			ulong value2 = 0uL;
			if (elementUnsafe < (uint)Vector256<ulong>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector256<double> Shuffle(Vector256<double> vector, Vector256<long> indices)
	{
		Unsafe.SkipInit<Vector256<double>>(out var value);
		for (int i = 0; i < Vector256<double>.Count; i++)
		{
			ulong elementUnsafe = (ulong)GetElementUnsafe(in indices, i);
			double value2 = 0.0;
			if (elementUnsafe < (uint)Vector256<double>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector256<long> ShuffleNative(Vector256<long> vector, Vector256<long> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> ShuffleNative(Vector256<ulong> vector, Vector256<ulong> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector256<double> ShuffleNative(Vector256<double> vector, Vector256<long> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<double> Sin(Vector256<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinDouble<Vector256<double>, Vector256<long>>(vector);
		}
		return Create(Vector128.Sin(vector._lower), Vector128.Sin(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector256<float> Sin(Vector256<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector512.IsHardwareAccelerated)
			{
				return VectorMath.SinSingle<Vector256<float>, Vector256<int>, Vector512<double>, Vector512<long>>(vector);
			}
			return VectorMath.SinSingle<Vector256<float>, Vector256<int>, Vector256<double>, Vector256<long>>(vector);
		}
		return Create(Vector128.Sin(vector._lower), Vector128.Sin(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector256<double> Sin, Vector256<double> Cos) SinCos(Vector256<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinCosDouble<Vector256<double>, Vector256<long>>(vector);
		}
		var (lower, lower2) = Vector128.SinCos(vector._lower);
		var (upper, upper2) = Vector128.SinCos(vector._upper);
		return (Sin: Create(lower, upper), Cos: Create(lower2, upper2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector256<float> Sin, Vector256<float> Cos) SinCos(Vector256<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector512.IsHardwareAccelerated)
			{
				return VectorMath.SinCosSingle<Vector256<float>, Vector256<int>, Vector512<double>, Vector512<long>>(vector);
			}
			return VectorMath.SinCosSingle<Vector256<float>, Vector256<int>, Vector256<double>, Vector256<long>>(vector);
		}
		var (lower, lower2) = Vector128.SinCos(vector._lower);
		var (upper, upper2) = Vector128.SinCos(vector._upper);
		return (Sin: Create(lower, upper), Cos: Create(lower2, upper2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> Sqrt<T>(Vector256<T> vector)
	{
		return Create(Vector128.Sqrt(vector._lower), Vector128.Sqrt(vector._upper));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void Store<T>(this Vector256<T> source, T* destination)
	{
		source.StoreUnsafe(ref *destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned<T>(this Vector256<T> source, T* destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		if ((nuint)destination % (nuint)32u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		*(Vector256<T>*)destination = source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal<T>(this Vector256<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static void StoreUnsafe<T>(this Vector256<T> source, ref T destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static void StoreUnsafe<T>(this Vector256<T> source, ref T destination, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[Intrinsic]
	public static Vector256<T> Subtract<T>(Vector256<T> left, Vector256<T> right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> SubtractSaturate<T>(Vector256<T> left, Vector256<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left - right;
		}
		return Create(Vector128.SubtractSaturate(left._lower, right._lower), Vector128.SubtractSaturate(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Sum<T>(Vector256<T> vector)
	{
		return Scalar<T>.Add(Vector128.Sum(vector._lower), Vector128.Sum(vector._upper));
	}

	[Intrinsic]
	public static T ToScalar<T>(this Vector256<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		return GetElementUnsafe(in vector, 0);
	}

	[Intrinsic]
	public static Vector512<T> ToVector512<T>(this Vector256<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		Vector512<T> vector2 = default(Vector512<T>);
		vector2.SetLowerUnsafe<T>(vector);
		return vector2;
	}

	[Intrinsic]
	public static Vector512<T> ToVector512Unsafe<T>(this Vector256<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		Unsafe.SkipInit<Vector512<T>>(out var value);
		value.SetLowerUnsafe<T>(vector);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector256<T> Truncate<T>(Vector256<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector128.Truncate(vector._lower), Vector128.Truncate(vector._upper));
	}

	[Intrinsic]
	public static Vector256<double> Truncate(Vector256<double> vector)
	{
		return Vector256.Truncate<double>(vector);
	}

	[Intrinsic]
	public static Vector256<float> Truncate(Vector256<float> vector)
	{
		return Vector256.Truncate<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryCopyTo<T>(this Vector256<T> vector, Span<T> destination)
	{
		if (destination.Length < Vector256<T>.Count)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector256<ushort> Lower, Vector256<ushort> Upper) Widen(Vector256<byte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector256<int> Lower, Vector256<int> Upper) Widen(Vector256<short> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector256<long> Lower, Vector256<long> Upper) Widen(Vector256<int> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector256<short> Lower, Vector256<short> Upper) Widen(Vector256<sbyte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector256<double> Lower, Vector256<double> Upper) Widen(Vector256<float> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector256<uint> Lower, Vector256<uint> Upper) Widen(Vector256<ushort> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector256<ulong> Lower, Vector256<ulong> Upper) Widen(Vector256<uint> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> WidenLower(Vector256<byte> source)
	{
		Vector128<byte> lower = source._lower;
		return Create(Vector128.WidenLower(lower), Vector128.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<int> WidenLower(Vector256<short> source)
	{
		Vector128<short> lower = source._lower;
		return Create(Vector128.WidenLower(lower), Vector128.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<long> WidenLower(Vector256<int> source)
	{
		Vector128<int> lower = source._lower;
		return Create(Vector128.WidenLower(lower), Vector128.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<short> WidenLower(Vector256<sbyte> source)
	{
		Vector128<sbyte> lower = source._lower;
		return Create(Vector128.WidenLower(lower), Vector128.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> WidenLower(Vector256<float> source)
	{
		Vector128<float> lower = source._lower;
		return Create(Vector128.WidenLower(lower), Vector128.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> WidenLower(Vector256<ushort> source)
	{
		Vector128<ushort> lower = source._lower;
		return Create(Vector128.WidenLower(lower), Vector128.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> WidenLower(Vector256<uint> source)
	{
		Vector128<uint> lower = source._lower;
		return Create(Vector128.WidenLower(lower), Vector128.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ushort> WidenUpper(Vector256<byte> source)
	{
		Vector128<byte> upper = source._upper;
		return Create(Vector128.WidenLower(upper), Vector128.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<int> WidenUpper(Vector256<short> source)
	{
		Vector128<short> upper = source._upper;
		return Create(Vector128.WidenLower(upper), Vector128.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<long> WidenUpper(Vector256<int> source)
	{
		Vector128<int> upper = source._upper;
		return Create(Vector128.WidenLower(upper), Vector128.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<short> WidenUpper(Vector256<sbyte> source)
	{
		Vector128<sbyte> upper = source._upper;
		return Create(Vector128.WidenLower(upper), Vector128.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<double> WidenUpper(Vector256<float> source)
	{
		Vector128<float> upper = source._upper;
		return Create(Vector128.WidenLower(upper), Vector128.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<uint> WidenUpper(Vector256<ushort> source)
	{
		Vector128<ushort> upper = source._upper;
		return Create(Vector128.WidenLower(upper), Vector128.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector256<ulong> WidenUpper(Vector256<uint> source)
	{
		Vector128<uint> upper = source._upper;
		return Create(Vector128.WidenLower(upper), Vector128.WidenUpper(upper));
	}

	[Intrinsic]
	public static Vector256<T> WithElement<T>(this Vector256<T> vector, int index, T value)
	{
		if ((uint)index >= (uint)Vector256<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		Vector256<T> vector2 = vector;
		SetElementUnsafe(in vector2, index, value);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> WithLower<T>(this Vector256<T> vector, Vector128<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		Vector256<T> vector2 = vector;
		vector2.SetLowerUnsafe<T>(value);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> WithUpper<T>(this Vector256<T> vector, Vector128<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		Vector256<T> vector2 = vector;
		vector2.SetUpperUnsafe<T>(value);
		return vector2;
	}

	[Intrinsic]
	public static Vector256<T> Xor<T>(Vector256<T> left, Vector256<T> right)
	{
		return left ^ right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T GetElementUnsafe<T>(this in Vector256<T> vector, int index)
	{
		return Unsafe.Add(ref Unsafe.As<Vector256<T>, T>(ref Unsafe.AsRef(in vector)), index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetElementUnsafe<T>(this in Vector256<T> vector, int index, T value)
	{
		Unsafe.Add(ref Unsafe.As<Vector256<T>, T>(ref Unsafe.AsRef(in vector)), index) = value;
	}

	internal static void SetLowerUnsafe<T>(this in Vector256<T> vector, Vector128<T> value)
	{
		Unsafe.AsRef(in vector._lower) = value;
	}

	internal static void SetUpperUnsafe<T>(this in Vector256<T> vector, Vector128<T> value)
	{
		Unsafe.AsRef(in vector._upper) = value;
	}
}
[StructLayout(LayoutKind.Sequential, Size = 32)]
[Intrinsic]
[DebuggerDisplay("{DisplayString,nq}")]
[DebuggerTypeProxy(typeof(Vector256DebugView<>))]
public readonly struct Vector256<T> : ISimdVector<Vector256<T>, T>, IAdditionOperators<Vector256<T>, Vector256<T>, Vector256<T>>, IBitwiseOperators<Vector256<T>, Vector256<T>, Vector256<T>>, IDivisionOperators<Vector256<T>, Vector256<T>, Vector256<T>>, IEqualityOperators<Vector256<T>, Vector256<T>, bool>, IEquatable<Vector256<T>>, IMultiplyOperators<Vector256<T>, Vector256<T>, Vector256<T>>, IShiftOperators<Vector256<T>, int, Vector256<T>>, ISubtractionOperators<Vector256<T>, Vector256<T>, Vector256<T>>, IUnaryNegationOperators<Vector256<T>, Vector256<T>>, IUnaryPlusOperators<Vector256<T>, Vector256<T>>
{
	internal readonly Vector128<T> _lower;

	internal readonly Vector128<T> _upper;

	public static Vector256<T> AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector256.Create(Scalar<T>.AllBitsSet);
		}
	}

	public static int Count
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
			return 32 / Unsafe.SizeOf<T>();
		}
	}

	public static Vector256<T> Indices
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
			Unsafe.SkipInit<Vector256<T>>(out var value);
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

	public static Vector256<T> One
	{
		[Intrinsic]
		get
		{
			return Vector256.Create(Scalar<T>.One);
		}
	}

	public static Vector256<T> Zero
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
			return default(Vector256<T>);
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

	static int ISimdVector<Vector256<T>, T>.Alignment => 32;

	static int ISimdVector<Vector256<T>, T>.ElementCount => Count;

	static bool ISimdVector<Vector256<T>, T>.IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return Vector256.IsHardwareAccelerated;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator +(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Create(left._lower + right._lower, left._upper + right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator &(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Create(left._lower & right._lower, left._upper & right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator |(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Create(left._lower | right._lower, left._upper | right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator /(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Create(left._lower / right._lower, left._upper / right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator /(Vector256<T> left, T right)
	{
		return Vector256.Create(left._lower / right, left._upper / right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector256<T> left, Vector256<T> right)
	{
		if (left._lower == right._lower)
		{
			return left._upper == right._upper;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator ^(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Create(left._lower ^ right._lower, left._upper ^ right._upper);
	}

	[Intrinsic]
	public static bool operator !=(Vector256<T> left, Vector256<T> right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator <<(Vector256<T> value, int shiftCount)
	{
		return Vector256.Create(value._lower << shiftCount, value._upper << shiftCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator *(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Create(left._lower * right._lower, left._upper * right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator *(Vector256<T> left, T right)
	{
		return Vector256.Create(left._lower * right, left._upper * right);
	}

	[Intrinsic]
	public static Vector256<T> operator *(T left, Vector256<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator ~(Vector256<T> vector)
	{
		return Vector256.Create(~vector._lower, ~vector._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator >>(Vector256<T> value, int shiftCount)
	{
		return Vector256.Create(value._lower >> shiftCount, value._upper >> shiftCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator -(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Create(left._lower - right._lower, left._upper - right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator -(Vector256<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return vector ^ Vector256.Create(-0f).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return vector ^ Vector256.Create(-0.0).As<double, T>();
		}
		return Zero - vector;
	}

	[Intrinsic]
	public static Vector256<T> operator +(Vector256<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> operator >>>(Vector256<T> value, int shiftCount)
	{
		return Vector256.Create(value._lower >>> shiftCount, value._upper >>> shiftCount);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector256<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector256<T> other)
	{
		if (Vector256.IsHardwareAccelerated)
		{
			if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
			{
				return (Vector256.Equals(this, other) | ~(Vector256.Equals(this, this) | Vector256.Equals(other, other))).AsInt32() == Vector256<int>.AllBitsSet;
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
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector256BaseType<T>();
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
	static Vector256<T> ISimdVector<Vector256<T>, T>.Abs(Vector256<T> vector)
	{
		return Vector256.Abs(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Add(Vector256<T> left, Vector256<T> right)
	{
		return left + right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.All(Vector256<T> vector, T value)
	{
		return Vector256.All(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.AllWhereAllBitsSet(Vector256<T> vector)
	{
		return Vector256.AllWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.AndNot(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.AndNot(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.Any(Vector256<T> vector, T value)
	{
		return Vector256.Any(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.AnyWhereAllBitsSet(Vector256<T> vector)
	{
		return Vector256.AnyWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.BitwiseAnd(Vector256<T> left, Vector256<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.BitwiseOr(Vector256<T> left, Vector256<T> right)
	{
		return left | right;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Ceiling(Vector256<T> vector)
	{
		return Vector256.Ceiling(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Clamp(Vector256<T> value, Vector256<T> min, Vector256<T> max)
	{
		return Vector256.Clamp(value, min, max);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.ClampNative(Vector256<T> value, Vector256<T> min, Vector256<T> max)
	{
		return Vector256.ClampNative(value, min, max);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.ConditionalSelect(Vector256<T> condition, Vector256<T> left, Vector256<T> right)
	{
		return Vector256.ConditionalSelect(condition, left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.CopySign(Vector256<T> value, Vector256<T> sign)
	{
		return Vector256.CopySign(value, sign);
	}

	static void ISimdVector<Vector256<T>, T>.CopyTo(Vector256<T> vector, T[] destination)
	{
		vector.CopyTo(destination);
	}

	static void ISimdVector<Vector256<T>, T>.CopyTo(Vector256<T> vector, T[] destination, int startIndex)
	{
		vector.CopyTo(destination, startIndex);
	}

	static void ISimdVector<Vector256<T>, T>.CopyTo(Vector256<T> vector, Span<T> destination)
	{
		vector.CopyTo(destination);
	}

	[Intrinsic]
	static int ISimdVector<Vector256<T>, T>.Count(Vector256<T> vector, T value)
	{
		return Vector256.Count(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector256<T>, T>.CountWhereAllBitsSet(Vector256<T> vector)
	{
		return Vector256.CountWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Create(T value)
	{
		return Vector256.Create(value);
	}

	static Vector256<T> ISimdVector<Vector256<T>, T>.Create(T[] values)
	{
		return Vector256.Create(values);
	}

	static Vector256<T> ISimdVector<Vector256<T>, T>.Create(T[] values, int index)
	{
		return Vector256.Create(values, index);
	}

	static Vector256<T> ISimdVector<Vector256<T>, T>.Create(ReadOnlySpan<T> values)
	{
		return Vector256.Create(values);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.CreateScalar(T value)
	{
		return Vector256.CreateScalar(value);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.CreateScalarUnsafe(T value)
	{
		return Vector256.CreateScalarUnsafe(value);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Divide(Vector256<T> left, Vector256<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Divide(Vector256<T> left, T right)
	{
		return left / right;
	}

	[Intrinsic]
	static T ISimdVector<Vector256<T>, T>.Dot(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Dot(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Equals(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Equals(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.EqualsAll(Vector256<T> left, Vector256<T> right)
	{
		return left == right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.EqualsAny(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.EqualsAny(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Floor(Vector256<T> vector)
	{
		return Vector256.Floor(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector256<T>, T>.GetElement(Vector256<T> vector, int index)
	{
		return vector.GetElement(index);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.GreaterThan(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.GreaterThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.GreaterThanAll(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.GreaterThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.GreaterThanAny(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.GreaterThanAny(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.GreaterThanOrEqual(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.GreaterThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.GreaterThanOrEqualAll(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.GreaterThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.GreaterThanOrEqualAny(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.GreaterThanOrEqualAny(left, right);
	}

	[Intrinsic]
	static int ISimdVector<Vector256<T>, T>.IndexOf(Vector256<T> vector, T value)
	{
		return Vector256.IndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector256<T>, T>.IndexOfWhereAllBitsSet(Vector256<T> vector)
	{
		return Vector256.IndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsEvenInteger(Vector256<T> vector)
	{
		return Vector256.IsEvenInteger(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsFinite(Vector256<T> vector)
	{
		return Vector256.IsFinite(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsInfinity(Vector256<T> vector)
	{
		return Vector256.IsInfinity(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsInteger(Vector256<T> vector)
	{
		return Vector256.IsInteger(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsNaN(Vector256<T> vector)
	{
		return Vector256.IsNaN(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsNegative(Vector256<T> vector)
	{
		return Vector256.IsNegative(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsNegativeInfinity(Vector256<T> vector)
	{
		return Vector256.IsNegativeInfinity(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsNormal(Vector256<T> vector)
	{
		return Vector256.IsNormal(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsOddInteger(Vector256<T> vector)
	{
		return Vector256.IsOddInteger(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsPositive(Vector256<T> vector)
	{
		return Vector256.IsPositive(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsPositiveInfinity(Vector256<T> vector)
	{
		return Vector256.IsPositiveInfinity(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.IsSubnormal(Vector256<T> vector)
	{
		return Vector256.IsSubnormal(vector);
	}

	static Vector256<T> ISimdVector<Vector256<T>, T>.IsZero(Vector256<T> vector)
	{
		return Vector256.IsZero(vector);
	}

	[Intrinsic]
	static int ISimdVector<Vector256<T>, T>.LastIndexOf(Vector256<T> vector, T value)
	{
		return Vector256.LastIndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector256<T>, T>.LastIndexOfWhereAllBitsSet(Vector256<T> vector)
	{
		return Vector256.LastIndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.LessThan(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.LessThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.LessThanAll(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.LessThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.LessThanAny(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.LessThanAny(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.LessThanOrEqual(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.LessThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.LessThanOrEqualAll(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.LessThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.LessThanOrEqualAny(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.LessThanOrEqualAny(left, right);
	}

	[Intrinsic]
	unsafe static Vector256<T> ISimdVector<Vector256<T>, T>.Load(T* source)
	{
		return Vector256.Load(source);
	}

	[Intrinsic]
	unsafe static Vector256<T> ISimdVector<Vector256<T>, T>.LoadAligned(T* source)
	{
		return Vector256.LoadAligned(source);
	}

	[Intrinsic]
	unsafe static Vector256<T> ISimdVector<Vector256<T>, T>.LoadAlignedNonTemporal(T* source)
	{
		return Vector256.LoadAlignedNonTemporal(source);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.LoadUnsafe(ref readonly T source)
	{
		return Vector256.LoadUnsafe(in source);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.LoadUnsafe(ref readonly T source, nuint elementOffset)
	{
		return Vector256.LoadUnsafe(in source, elementOffset);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Max(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Max(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MaxMagnitude(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MaxMagnitude(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MaxMagnitudeNumber(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MaxMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MaxNative(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MaxNative(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MaxNumber(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MaxNumber(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Min(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.Min(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MinMagnitude(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MinMagnitude(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MinMagnitudeNumber(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MinMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MinNative(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MinNative(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MinNumber(Vector256<T> left, Vector256<T> right)
	{
		return Vector256.MinNumber(left, right);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Multiply(Vector256<T> left, Vector256<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Multiply(Vector256<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.MultiplyAddEstimate(Vector256<T> left, Vector256<T> right, Vector256<T> addend)
	{
		return Vector256.MultiplyAddEstimate(left, right, addend);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Negate(Vector256<T> vector)
	{
		return -vector;
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.None(Vector256<T> vector, T value)
	{
		return Vector256.None(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector256<T>, T>.NoneWhereAllBitsSet(Vector256<T> vector)
	{
		return Vector256.NoneWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.OnesComplement(Vector256<T> vector)
	{
		return ~vector;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Round(Vector256<T> vector)
	{
		return Vector256.Round(vector);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.ShiftLeft(Vector256<T> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.ShiftRightArithmetic(Vector256<T> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.ShiftRightLogical(Vector256<T> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Sqrt(Vector256<T> vector)
	{
		return Vector256.Sqrt(vector);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector256<T>, T>.Store(Vector256<T> source, T* destination)
	{
		source.Store(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector256<T>, T>.StoreAligned(Vector256<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector256<T>, T>.StoreAlignedNonTemporal(Vector256<T> source, T* destination)
	{
		source.StoreAlignedNonTemporal(destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector256<T>, T>.StoreUnsafe(Vector256<T> vector, ref T destination)
	{
		vector.StoreUnsafe(ref destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector256<T>, T>.StoreUnsafe(Vector256<T> vector, ref T destination, nuint elementOffset)
	{
		vector.StoreUnsafe(ref destination, elementOffset);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Subtract(Vector256<T> left, Vector256<T> right)
	{
		return left - right;
	}

	[Intrinsic]
	static T ISimdVector<Vector256<T>, T>.Sum(Vector256<T> vector)
	{
		return Vector256.Sum(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector256<T>, T>.ToScalar(Vector256<T> vector)
	{
		return vector.ToScalar();
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Truncate(Vector256<T> vector)
	{
		return Vector256.Truncate(vector);
	}

	static bool ISimdVector<Vector256<T>, T>.TryCopyTo(Vector256<T> vector, Span<T> destination)
	{
		return vector.TryCopyTo(destination);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.WithElement(Vector256<T> vector, int index, T value)
	{
		return vector.WithElement(index, value);
	}

	[Intrinsic]
	static Vector256<T> ISimdVector<Vector256<T>, T>.Xor(Vector256<T> left, Vector256<T> right)
	{
		return left ^ right;
	}
}
