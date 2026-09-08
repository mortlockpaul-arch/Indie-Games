using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace System.Runtime.Intrinsics;

public static class Vector128
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
	public static Vector128<T> Abs<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return vector;
		}
		return Create(Vector64.Abs(vector._lower), Vector64.Abs(vector._upper));
	}

	[Intrinsic]
	public static Vector128<T> Add<T>(Vector128<T> left, Vector128<T> right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> AddSaturate<T>(Vector128<T> left, Vector128<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left + right;
		}
		return Create(Vector64.AddSaturate(left._lower, right._lower), Vector64.AddSaturate(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All<T>(Vector128<T> vector, T value)
	{
		return vector == Create(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet<T>(Vector128<T> vector)
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
	public static Vector128<T> AndNot<T>(Vector128<T> left, Vector128<T> right)
	{
		return left & ~right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any<T>(Vector128<T> vector, T value)
	{
		return EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet<T>(Vector128<T> vector)
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
	public static Vector128<TTo> As<TFrom, TTo>(this Vector128<TFrom> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<TFrom>();
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<TTo>();
		return Unsafe.BitCast<Vector128<TFrom>, Vector128<TTo>>(vector);
	}

	[Intrinsic]
	public static Vector128<byte> AsByte<T>(this Vector128<T> vector)
	{
		return vector.As<T, byte>();
	}

	[Intrinsic]
	public static Vector128<double> AsDouble<T>(this Vector128<T> vector)
	{
		return vector.As<T, double>();
	}

	[Intrinsic]
	public static Vector128<short> AsInt16<T>(this Vector128<T> vector)
	{
		return vector.As<T, short>();
	}

	[Intrinsic]
	public static Vector128<int> AsInt32<T>(this Vector128<T> vector)
	{
		return vector.As<T, int>();
	}

	[Intrinsic]
	public static Vector128<long> AsInt64<T>(this Vector128<T> vector)
	{
		return vector.As<T, long>();
	}

	[Intrinsic]
	public static Vector128<nint> AsNInt<T>(this Vector128<T> vector)
	{
		return vector.As<T, nint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<nuint> AsNUInt<T>(this Vector128<T> vector)
	{
		return vector.As<T, nuint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> AsSByte<T>(this Vector128<T> vector)
	{
		return vector.As<T, sbyte>();
	}

	[Intrinsic]
	public static Vector128<float> AsSingle<T>(this Vector128<T> vector)
	{
		return vector.As<T, float>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> AsUInt16<T>(this Vector128<T> vector)
	{
		return vector.As<T, ushort>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> AsUInt32<T>(this Vector128<T> vector)
	{
		return vector.As<T, uint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> AsUInt64<T>(this Vector128<T> vector)
	{
		return vector.As<T, ulong>();
	}

	[Intrinsic]
	public static Vector128<T> BitwiseAnd<T>(Vector128<T> left, Vector128<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector128<T> BitwiseOr<T>(Vector128<T> left, Vector128<T> right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector128<T> Ceiling<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector64.Ceiling(vector._lower), Vector64.Ceiling(vector._upper));
	}

	[Intrinsic]
	public static Vector128<float> Ceiling(Vector128<float> vector)
	{
		return Vector128.Ceiling<float>(vector);
	}

	[Intrinsic]
	public static Vector128<double> Ceiling(Vector128<double> vector)
	{
		return Vector128.Ceiling<double>(vector);
	}

	[Intrinsic]
	public static Vector128<T> Clamp<T>(Vector128<T> value, Vector128<T> min, Vector128<T> max)
	{
		return Min(Max(value, min), max);
	}

	[Intrinsic]
	public static Vector128<T> ClampNative<T>(Vector128<T> value, Vector128<T> min, Vector128<T> max)
	{
		return MinNative(MaxNative(value, min), max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> ConditionalSelect<T>(Vector128<T> condition, Vector128<T> left, Vector128<T> right)
	{
		return (left & condition) | AndNot(right, condition);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> ConvertToDouble(Vector128<long> vector)
	{
		if (Sse2.IsSupported)
		{
			Vector128<int> left;
			if (Avx2.IsSupported)
			{
				left = vector.AsInt32();
				left = Avx2.Blend(left, Create(4841369599423283200L).AsInt32(), 10);
			}
			else
			{
				left = Sse2.And(vector, Create(4294967295L)).AsInt32();
				left = Sse2.Or(left, Create(4841369599423283200L).AsInt32());
			}
			return Sse2.Add(Sse2.Subtract(Sse2.Xor(Sse2.ShiftRightLogical(vector, 32), Create(4985484789646622720L)).AsDouble(), Create(4985484789647671296L).AsDouble()), left.AsDouble());
		}
		return Create(Vector64.ConvertToDouble(vector._lower), Vector64.ConvertToDouble(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<double> ConvertToDouble(Vector128<ulong> vector)
	{
		if (Sse2.IsSupported)
		{
			Vector128<uint> left;
			if (Avx2.IsSupported)
			{
				left = vector.AsUInt32();
				left = Avx2.Blend(left, Create(4841369599423283200uL).AsUInt32(), 10);
			}
			else
			{
				left = Sse2.And(vector, Create(4294967295uL)).AsUInt32();
				left = Sse2.Or(left, Create(4841369599423283200uL).AsUInt32());
			}
			return Sse2.Add(Sse2.Subtract(Sse2.Xor(Sse2.ShiftRightLogical(vector, 32), Create(4985484787499139072uL)).AsDouble(), Create(4985484787500187648uL).AsDouble()), left.AsDouble());
		}
		return Create(Vector64.ConvertToDouble(vector._lower), Vector64.ConvertToDouble(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<int> ConvertToInt32(Vector128<float> vector)
	{
		return Create(Vector64.ConvertToInt32(vector._lower), Vector64.ConvertToInt32(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<int> ConvertToInt32Native(Vector128<float> vector)
	{
		return Create(Vector64.ConvertToInt32Native(vector._lower), Vector64.ConvertToInt32Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<long> ConvertToInt64(Vector128<double> vector)
	{
		return Create(Vector64.ConvertToInt64(vector._lower), Vector64.ConvertToInt64(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<long> ConvertToInt64Native(Vector128<double> vector)
	{
		return Create(Vector64.ConvertToInt64Native(vector._lower), Vector64.ConvertToInt64Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> ConvertToSingle(Vector128<int> vector)
	{
		return Create(Vector64.ConvertToSingle(vector._lower), Vector64.ConvertToSingle(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<float> ConvertToSingle(Vector128<uint> vector)
	{
		if (Sse2.IsSupported)
		{
			Vector128<int> value = Sse2.And(vector, Create(65535u)).AsInt32();
			Vector128<int> value2 = Sse2.ShiftRightLogical(vector, 16).AsInt32();
			Vector128<float> vector2 = Sse2.ConvertToVector128Single(value);
			Vector128<float> vector3 = Sse2.ConvertToVector128Single(value2);
			if (Fma.IsSupported)
			{
				return Fma.MultiplyAdd(vector3, Create(65536f), vector2);
			}
			return Sse.Add(Sse.Multiply(vector3, Create(65536f)), vector2);
		}
		return SoftwareFallback(vector);
		static Vector128<float> SoftwareFallback(Vector128<uint> vector4)
		{
			Unsafe.SkipInit<Vector128<float>>(out var value3);
			for (int i = 0; i < Vector128<float>.Count; i++)
			{
				float value4 = GetElementUnsafe(in vector4, i);
				value3.SetElementUnsafe(i, value4);
			}
			return value3;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> ConvertToUInt32(Vector128<float> vector)
	{
		return Create(Vector64.ConvertToUInt32(vector._lower), Vector64.ConvertToUInt32(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> ConvertToUInt32Native(Vector128<float> vector)
	{
		return Create(Vector64.ConvertToUInt32Native(vector._lower), Vector64.ConvertToUInt32Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> ConvertToUInt64(Vector128<double> vector)
	{
		return Create(Vector64.ConvertToUInt64(vector._lower), Vector64.ConvertToUInt64(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> ConvertToUInt64Native(Vector128<double> vector)
	{
		return Create(Vector64.ConvertToUInt64Native(vector._lower), Vector64.ConvertToUInt64Native(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> CopySign<T>(Vector128<T> value, Vector128<T> sign)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return value;
		}
		if (IsHardwareAccelerated)
		{
			return VectorMath.CopySign<Vector128<T>, T>(value, sign);
		}
		return Create(Vector64.CopySign(value._lower, sign._lower), Vector64.CopySign(value._upper, sign._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector128<T> vector, T[] destination)
	{
		if (destination.Length < Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[0]), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector128<T> vector, T[] destination, int startIndex)
	{
		if ((uint)startIndex >= (uint)destination.Length)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
		}
		if (destination.Length - startIndex < Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[startIndex]), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyTo<T>(this Vector128<T> vector, Span<T> destination)
	{
		if (destination.Length < Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<double> Cos(Vector128<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.CosDouble<Vector128<double>, Vector128<long>>(vector);
		}
		return Create(Vector64.Cos(vector._lower), Vector64.Cos(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<float> Cos(Vector128<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				return VectorMath.CosSingle<Vector128<float>, Vector128<int>, Vector256<double>, Vector256<long>>(vector);
			}
			return VectorMath.CosSingle<Vector128<float>, Vector128<int>, Vector128<double>, Vector128<long>>(vector);
		}
		return Create(Vector64.Cos(vector._lower), Vector64.Cos(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count<T>(Vector128<T> vector, T value)
	{
		return BitOperations.PopCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet<T>(Vector128<T> vector)
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
	public static Vector128<T> Create<T>(T value)
	{
		Vector64<T> vector = Vector64.Create(value);
		return Create(vector, vector);
	}

	[Intrinsic]
	public static Vector128<byte> Create(byte value)
	{
		return Vector128.Create<byte>(value);
	}

	[Intrinsic]
	public static Vector128<double> Create(double value)
	{
		return Vector128.Create<double>(value);
	}

	[Intrinsic]
	public static Vector128<short> Create(short value)
	{
		return Vector128.Create<short>(value);
	}

	[Intrinsic]
	public static Vector128<int> Create(int value)
	{
		return Vector128.Create<int>(value);
	}

	[Intrinsic]
	public static Vector128<long> Create(long value)
	{
		return Vector128.Create<long>(value);
	}

	[Intrinsic]
	public static Vector128<nint> Create(nint value)
	{
		return Vector128.Create<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<nuint> Create(nuint value)
	{
		return Vector128.Create<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> Create(sbyte value)
	{
		return Vector128.Create<sbyte>(value);
	}

	[Intrinsic]
	public static Vector128<float> Create(float value)
	{
		return Vector128.Create<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> Create(ushort value)
	{
		return Vector128.Create<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> Create(uint value)
	{
		return Vector128.Create<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> Create(ulong value)
	{
		return Vector128.Create<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<T> Create<T>(T[] values)
	{
		if (values.Length < Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector128<T>>(in Unsafe.As<T, byte>(ref values[0]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<T> Create<T>(T[] values, int index)
	{
		if (index < 0 || values.Length - index < Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		return Unsafe.ReadUnaligned<Vector128<T>>(in Unsafe.As<T, byte>(ref values[index]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<T> Create<T>(ReadOnlySpan<T> values)
	{
		if (values.Length < Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return Unsafe.ReadUnaligned<Vector128<T>>(in Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<byte> Create(byte e0, byte e1, byte e2, byte e3, byte e4, byte e5, byte e6, byte e7, byte e8, byte e9, byte e10, byte e11, byte e12, byte e13, byte e14, byte e15)
	{
		return Create(Vector64.Create(e0, e1, e2, e3, e4, e5, e6, e7), Vector64.Create(e8, e9, e10, e11, e12, e13, e14, e15));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> Create(double e0, double e1)
	{
		return Create(Vector64.Create(e0), Vector64.Create(e1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<short> Create(short e0, short e1, short e2, short e3, short e4, short e5, short e6, short e7)
	{
		return Create(Vector64.Create(e0, e1, e2, e3), Vector64.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<int> Create(int e0, int e1, int e2, int e3)
	{
		return Create(Vector64.Create(e0, e1), Vector64.Create(e2, e3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<long> Create(long e0, long e1)
	{
		return Create(Vector64.Create(e0), Vector64.Create(e1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> Create(sbyte e0, sbyte e1, sbyte e2, sbyte e3, sbyte e4, sbyte e5, sbyte e6, sbyte e7, sbyte e8, sbyte e9, sbyte e10, sbyte e11, sbyte e12, sbyte e13, sbyte e14, sbyte e15)
	{
		return Create(Vector64.Create(e0, e1, e2, e3, e4, e5, e6, e7), Vector64.Create(e8, e9, e10, e11, e12, e13, e14, e15));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> Create(float e0, float e1, float e2, float e3)
	{
		return Create(Vector64.Create(e0, e1), Vector64.Create(e2, e3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> Create(ushort e0, ushort e1, ushort e2, ushort e3, ushort e4, ushort e5, ushort e6, ushort e7)
	{
		return Create(Vector64.Create(e0, e1, e2, e3), Vector64.Create(e4, e5, e6, e7));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> Create(uint e0, uint e1, uint e2, uint e3)
	{
		return Create(Vector64.Create(e0, e1), Vector64.Create(e2, e3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> Create(ulong e0, ulong e1)
	{
		return Create(Vector64.Create(e0), Vector64.Create(e1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<T> Create<T>(Vector64<T> value)
	{
		return Create(value, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<T> Create<T>(Vector64<T> lower, Vector64<T> upper)
	{
		if (false)
		{
		}
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Unsafe.SkipInit<Vector128<T>>(out var value);
		value.SetLowerUnsafe<T>(lower);
		value.SetUpperUnsafe<T>(upper);
		return value;
	}

	public static Vector128<byte> Create(Vector64<byte> lower, Vector64<byte> upper)
	{
		return Vector128.Create<byte>(lower, upper);
	}

	public static Vector128<double> Create(Vector64<double> lower, Vector64<double> upper)
	{
		return Vector128.Create<double>(lower, upper);
	}

	public static Vector128<short> Create(Vector64<short> lower, Vector64<short> upper)
	{
		return Vector128.Create<short>(lower, upper);
	}

	public static Vector128<int> Create(Vector64<int> lower, Vector64<int> upper)
	{
		return Vector128.Create<int>(lower, upper);
	}

	public static Vector128<long> Create(Vector64<long> lower, Vector64<long> upper)
	{
		return Vector128.Create<long>(lower, upper);
	}

	public static Vector128<nint> Create(Vector64<nint> lower, Vector64<nint> upper)
	{
		return Vector128.Create<nint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector128<nuint> Create(Vector64<nuint> lower, Vector64<nuint> upper)
	{
		return Vector128.Create<nuint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector128<sbyte> Create(Vector64<sbyte> lower, Vector64<sbyte> upper)
	{
		return Vector128.Create<sbyte>(lower, upper);
	}

	public static Vector128<float> Create(Vector64<float> lower, Vector64<float> upper)
	{
		return Vector128.Create<float>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector128<ushort> Create(Vector64<ushort> lower, Vector64<ushort> upper)
	{
		return Vector128.Create<ushort>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector128<uint> Create(Vector64<uint> lower, Vector64<uint> upper)
	{
		return Vector128.Create<uint>(lower, upper);
	}

	[CLSCompliant(false)]
	public static Vector128<ulong> Create(Vector64<ulong> lower, Vector64<ulong> upper)
	{
		return Vector128.Create<ulong>(lower, upper);
	}

	[Intrinsic]
	public static Vector128<T> CreateScalar<T>(T value)
	{
		return Vector64.CreateScalar(value).ToVector128();
	}

	[Intrinsic]
	public static Vector128<byte> CreateScalar(byte value)
	{
		return Vector128.CreateScalar<byte>(value);
	}

	[Intrinsic]
	public static Vector128<double> CreateScalar(double value)
	{
		return Vector128.CreateScalar<double>(value);
	}

	[Intrinsic]
	public static Vector128<short> CreateScalar(short value)
	{
		return Vector128.CreateScalar<short>(value);
	}

	[Intrinsic]
	public static Vector128<int> CreateScalar(int value)
	{
		return Vector128.CreateScalar<int>(value);
	}

	[Intrinsic]
	public static Vector128<long> CreateScalar(long value)
	{
		return Vector128.CreateScalar<long>(value);
	}

	[Intrinsic]
	public static Vector128<nint> CreateScalar(nint value)
	{
		return Vector128.CreateScalar<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<nuint> CreateScalar(nuint value)
	{
		return Vector128.CreateScalar<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> CreateScalar(sbyte value)
	{
		return Vector128.CreateScalar<sbyte>(value);
	}

	[Intrinsic]
	public static Vector128<float> CreateScalar(float value)
	{
		return Vector128.CreateScalar<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> CreateScalar(ushort value)
	{
		return Vector128.CreateScalar<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> CreateScalar(uint value)
	{
		return Vector128.CreateScalar<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> CreateScalar(ulong value)
	{
		return Vector128.CreateScalar<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> CreateScalarUnsafe<T>(T value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Unsafe.SkipInit<Vector128<T>>(out var value2);
		SetElementUnsafe(in value2, 0, value);
		return value2;
	}

	[Intrinsic]
	public static Vector128<byte> CreateScalarUnsafe(byte value)
	{
		return Vector128.CreateScalarUnsafe<byte>(value);
	}

	[Intrinsic]
	public static Vector128<double> CreateScalarUnsafe(double value)
	{
		return Vector128.CreateScalarUnsafe<double>(value);
	}

	[Intrinsic]
	public static Vector128<short> CreateScalarUnsafe(short value)
	{
		return Vector128.CreateScalarUnsafe<short>(value);
	}

	[Intrinsic]
	public static Vector128<int> CreateScalarUnsafe(int value)
	{
		return Vector128.CreateScalarUnsafe<int>(value);
	}

	[Intrinsic]
	public static Vector128<long> CreateScalarUnsafe(long value)
	{
		return Vector128.CreateScalarUnsafe<long>(value);
	}

	[Intrinsic]
	public static Vector128<nint> CreateScalarUnsafe(nint value)
	{
		return Vector128.CreateScalarUnsafe<nint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<nuint> CreateScalarUnsafe(nuint value)
	{
		return Vector128.CreateScalarUnsafe<nuint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> CreateScalarUnsafe(sbyte value)
	{
		return Vector128.CreateScalarUnsafe<sbyte>(value);
	}

	[Intrinsic]
	public static Vector128<float> CreateScalarUnsafe(float value)
	{
		return Vector128.CreateScalarUnsafe<float>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> CreateScalarUnsafe(ushort value)
	{
		return Vector128.CreateScalarUnsafe<ushort>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> CreateScalarUnsafe(uint value)
	{
		return Vector128.CreateScalarUnsafe<uint>(value);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> CreateScalarUnsafe(ulong value)
	{
		return Vector128.CreateScalarUnsafe<ulong>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> CreateSequence<T>(T start, T step)
	{
		return Vector128<T>.Indices * step + Create(start);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> DegreesToRadians(Vector128<double> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector128<double>, double>(degrees);
		}
		return Create(Vector64.DegreesToRadians(degrees._lower), Vector64.DegreesToRadians(degrees._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> DegreesToRadians(Vector128<float> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector128<float>, float>(degrees);
		}
		return Create(Vector64.DegreesToRadians(degrees._lower), Vector64.DegreesToRadians(degrees._upper));
	}

	[Intrinsic]
	public static Vector128<T> Divide<T>(Vector128<T> left, Vector128<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector128<T> Divide<T>(Vector128<T> left, T right)
	{
		return left / right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Dot<T>(Vector128<T> left, Vector128<T> right)
	{
		return Sum(left * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> Equals<T>(Vector128<T> left, Vector128<T> right)
	{
		return Create(Vector64.Equals(left._lower, right._lower), Vector64.Equals(left._upper, right._upper));
	}

	[Intrinsic]
	public static bool EqualsAll<T>(Vector128<T> left, Vector128<T> right)
	{
		return left == right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny<T>(Vector128<T> left, Vector128<T> right)
	{
		if (!Vector64.EqualsAny(left._lower, right._lower))
		{
			return Vector64.EqualsAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<double> Exp(Vector128<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.ExpDouble<Vector128<double>, Vector128<ulong>>(vector);
		}
		return Create(Vector64.Exp(vector._lower), Vector64.Exp(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<float> Exp(Vector128<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				return VectorMath.ExpSingle<Vector128<float>, Vector128<uint>, Vector256<double>, Vector256<ulong>>(vector);
			}
			return VectorMath.ExpSingle<Vector128<float>, Vector128<uint>, Vector128<double>, Vector128<ulong>>(vector);
		}
		return Create(Vector64.Exp(vector._lower), Vector64.Exp(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static uint ExtractMostSignificantBits<T>(this Vector128<T> vector)
	{
		return vector._lower.ExtractMostSignificantBits() | (vector._upper.ExtractMostSignificantBits() << Vector64<T>.Count);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector128<T> Floor<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector64.Floor(vector._lower), Vector64.Floor(vector._upper));
	}

	[Intrinsic]
	public static Vector128<float> Floor(Vector128<float> vector)
	{
		return Vector128.Floor<float>(vector);
	}

	[Intrinsic]
	public static Vector128<double> Floor(Vector128<double> vector)
	{
		return Vector128.Floor<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> FusedMultiplyAdd(Vector128<double> left, Vector128<double> right, Vector128<double> addend)
	{
		return Create(Vector64.FusedMultiplyAdd(left._lower, right._lower, addend._lower), Vector64.FusedMultiplyAdd(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> FusedMultiplyAdd(Vector128<float> left, Vector128<float> right, Vector128<float> addend)
	{
		return Create(Vector64.FusedMultiplyAdd(left._lower, right._lower, addend._lower), Vector64.FusedMultiplyAdd(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T GetElement<T>(this Vector128<T> vector, int index)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		if ((uint)index >= (uint)Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return GetElementUnsafe(in vector, index);
	}

	[Intrinsic]
	public static Vector64<T> GetLower<T>(this Vector128<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		return vector._lower;
	}

	[Intrinsic]
	public static Vector64<T> GetUpper<T>(this Vector128<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		return vector._upper;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> GreaterThan<T>(Vector128<T> left, Vector128<T> right)
	{
		return Create(Vector64.GreaterThan(left._lower, right._lower), Vector64.GreaterThan(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll<T>(Vector128<T> left, Vector128<T> right)
	{
		if (Vector64.GreaterThanAll(left._lower, right._lower))
		{
			return Vector64.GreaterThanAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAny<T>(Vector128<T> left, Vector128<T> right)
	{
		if (!Vector64.GreaterThanAny(left._lower, right._lower))
		{
			return Vector64.GreaterThanAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> GreaterThanOrEqual<T>(Vector128<T> left, Vector128<T> right)
	{
		return Create(Vector64.GreaterThanOrEqual(left._lower, right._lower), Vector64.GreaterThanOrEqual(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll<T>(Vector128<T> left, Vector128<T> right)
	{
		if (Vector64.GreaterThanOrEqualAll(left._lower, right._lower))
		{
			return Vector64.GreaterThanOrEqualAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAny<T>(Vector128<T> left, Vector128<T> right)
	{
		if (!Vector64.GreaterThanOrEqualAny(left._lower, right._lower))
		{
			return Vector64.GreaterThanOrEqualAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<double> Hypot(Vector128<double> x, Vector128<double> y)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.HypotDouble<Vector128<double>, Vector128<ulong>>(x, y);
		}
		return Create(Vector64.Hypot(x._lower, y._lower), Vector64.Hypot(x._upper, y._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<float> Hypot(Vector128<float> x, Vector128<float> y)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				return VectorMath.HypotSingle<Vector128<float>, Vector256<double>>(x, y);
			}
			return VectorMath.HypotSingle<Vector128<float>, Vector128<double>>(x, y);
		}
		return Create(Vector64.Hypot(x._lower, y._lower), Vector64.Hypot(x._upper, y._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf<T>(Vector128<T> vector, T value)
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
	public static int IndexOfWhereAllBitsSet<T>(Vector128<T> vector)
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
	public static Vector128<T> IsEvenInteger<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsEvenIntegerSingle<Vector128<float>, Vector128<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsEvenIntegerDouble<Vector128<double>, Vector128<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return IsZero(vector & Vector128<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsFinite<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return ~IsZero(AndNot(Vector128.Create<uint>(2139095040u), vector.AsUInt32())).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return ~IsZero(AndNot(Vector128.Create<ulong>(9218868437227405312uL), vector.AsUInt64())).As<ulong, T>();
		}
		return Vector128<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsInfinity<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsPositiveInfinity(Abs(vector));
		}
		return Vector128<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsInteger<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsFinite(vector) & Equals(vector, Truncate(vector));
		}
		return Vector128<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsNaN<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return ~Equals(vector, vector);
		}
		return Vector128<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsNegative<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector128<T>.Zero;
		}
		if (typeof(T) == typeof(float))
		{
			return LessThan(vector.AsInt32(), Vector128<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(vector.AsInt64(), Vector128<long>.Zero).As<long, T>();
		}
		return LessThan(vector, Vector128<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsNegativeInfinity<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.NegativeInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.NegativeInfinity).As<double, T>());
		}
		return Vector128<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsNormal<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector128.Create<uint>(8388608u), Vector128.Create<uint>(2130706432u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector128.Create<ulong>(4503599627370496uL), Vector128.Create<ulong>(9214364837600034816uL)).As<ulong, T>();
		}
		return ~IsZero(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsOddInteger<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsOddIntegerSingle<Vector128<float>, Vector128<uint>>(vector.AsSingle()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsOddIntegerDouble<Vector128<double>, Vector128<ulong>>(vector.AsDouble()).As<double, T>();
		}
		return ~IsZero(vector & Vector128<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsPositive<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector128<T>.AllBitsSet;
		}
		if (typeof(T) == typeof(float))
		{
			return GreaterThanOrEqual(vector.AsInt32(), Vector128<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return GreaterThanOrEqual(vector.AsInt64(), Vector128<long>.Zero).As<long, T>();
		}
		return GreaterThanOrEqual(vector, Vector128<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsPositiveInfinity<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.PositiveInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.PositiveInfinity).As<double, T>());
		}
		return Vector128<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsSubnormal<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).AsUInt32() - Vector128<uint>.One, Vector128.Create<uint>(8388607u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).AsUInt64() - Vector128<ulong>.One, Vector128.Create<ulong>(4503599627370495uL)).As<ulong, T>();
		}
		return Vector128<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> IsZero<T>(Vector128<T> vector)
	{
		return Equals(vector, Vector128<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf<T>(Vector128<T> vector, T value)
	{
		return 31 - BitOperations.LeadingZeroCount(Equals(vector, Create(value)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet<T>(Vector128<T> vector)
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
	public static Vector128<double> Lerp(Vector128<double> x, Vector128<double> y, Vector128<double> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector128<double>, double>(x, y, amount);
		}
		return Create(Vector64.Lerp(x._lower, y._lower, amount._lower), Vector64.Lerp(x._upper, y._upper, amount._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> Lerp(Vector128<float> x, Vector128<float> y, Vector128<float> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector128<float>, float>(x, y, amount);
		}
		return Create(Vector64.Lerp(x._lower, y._lower, amount._lower), Vector64.Lerp(x._upper, y._upper, amount._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> LessThan<T>(Vector128<T> left, Vector128<T> right)
	{
		return Create(Vector64.LessThan(left._lower, right._lower), Vector64.LessThan(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll<T>(Vector128<T> left, Vector128<T> right)
	{
		if (Vector64.LessThanAll(left._lower, right._lower))
		{
			return Vector64.LessThanAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAny<T>(Vector128<T> left, Vector128<T> right)
	{
		if (!Vector64.LessThanAny(left._lower, right._lower))
		{
			return Vector64.LessThanAny(left._upper, right._upper);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> LessThanOrEqual<T>(Vector128<T> left, Vector128<T> right)
	{
		return Create(Vector64.LessThanOrEqual(left._lower, right._lower), Vector64.LessThanOrEqual(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll<T>(Vector128<T> left, Vector128<T> right)
	{
		if (Vector64.LessThanOrEqualAll(left._lower, right._lower))
		{
			return Vector64.LessThanOrEqualAll(left._upper, right._upper);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAny<T>(Vector128<T> left, Vector128<T> right)
	{
		if (!Vector64.LessThanOrEqualAny(left._lower, right._lower))
		{
			return Vector64.LessThanOrEqualAny(left._upper, right._upper);
		}
		return true;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector128<T> Load<T>(T* source)
	{
		return LoadUnsafe(in *source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector128<T> LoadAligned<T>(T* source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		if ((nuint)source % (nuint)16u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return *(Vector128<T>*)source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector128<T> LoadAlignedNonTemporal<T>(T* source)
	{
		return LoadAligned(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> LoadUnsafe<T>(ref readonly T source)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		return Unsafe.ReadUnaligned<Vector128<T>>(in Unsafe.As<T, byte>(ref Unsafe.AsRef(in source)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<T> LoadUnsafe<T>(ref readonly T source, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		return Unsafe.ReadUnaligned<Vector128<T>>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)));
	}

	internal static Vector128<ushort> LoadUnsafe(ref char source)
	{
		return LoadUnsafe(in Unsafe.As<char, ushort>(ref source));
	}

	internal static Vector128<ushort> LoadUnsafe(ref char source, nuint elementOffset)
	{
		return LoadUnsafe(in Unsafe.As<char, ushort>(ref source), elementOffset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<double> Log(Vector128<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogDouble<Vector128<double>, Vector128<long>, Vector128<ulong>>(vector);
		}
		return Create(Vector64.Log(vector._lower), Vector64.Log(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<float> Log(Vector128<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogSingle<Vector128<float>, Vector128<int>, Vector128<uint>>(vector);
		}
		return Create(Vector64.Log(vector._lower), Vector64.Log(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<double> Log2(Vector128<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Double<Vector128<double>, Vector128<long>, Vector128<ulong>>(vector);
		}
		return Create(Vector64.Log2(vector._lower), Vector64.Log2(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<float> Log2(Vector128<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Single<Vector128<float>, Vector128<int>, Vector128<uint>>(vector);
		}
		return Create(Vector64.Log2(vector._lower), Vector64.Log2(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> Max<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Max<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.Max(left._lower, right._lower), Vector64.Max(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MaxMagnitude<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitude<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.MaxMagnitude(left._lower, right._lower), Vector64.MaxMagnitude(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MaxMagnitudeNumber<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitudeNumber<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.MaxMagnitudeNumber(left._lower, right._lower), Vector64.MaxMagnitudeNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MaxNative<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(GreaterThan(left, right), left, right);
		}
		return Create(Vector64.MaxNative(left._lower, right._lower), Vector64.MaxNative(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MaxNumber<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxNumber<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.MaxNumber(left._lower, right._lower), Vector64.MaxNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> Min<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Min<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.Min(left._lower, right._lower), Vector64.Min(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MinMagnitude<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitude<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.MinMagnitude(left._lower, right._lower), Vector64.MinMagnitude(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MinMagnitudeNumber<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitudeNumber<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.MinMagnitudeNumber(left._lower, right._lower), Vector64.MinMagnitudeNumber(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MinNative<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(LessThan(left, right), left, right);
		}
		return Create(Vector64.MinNative(left._lower, right._lower), Vector64.MinNative(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> MinNumber<T>(Vector128<T> left, Vector128<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinNumber<Vector128<T>, T>(left, right);
		}
		return Create(Vector64.MinNumber(left._lower, right._lower), Vector64.MinNumber(left._upper, right._upper));
	}

	[Intrinsic]
	public static Vector128<T> Multiply<T>(Vector128<T> left, Vector128<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector128<T> Multiply<T>(Vector128<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector128<T> Multiply<T>(T left, Vector128<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector128<T> MultiplyAddEstimate<T>(Vector128<T> left, Vector128<T> right, Vector128<T> addend)
	{
		return Create(Vector64.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector64.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> MultiplyAddEstimate(Vector128<double> left, Vector128<double> right, Vector128<double> addend)
	{
		return Create(Vector64.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector64.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> MultiplyAddEstimate(Vector128<float> left, Vector128<float> right, Vector128<float> addend)
	{
		return Create(Vector64.MultiplyAddEstimate(left._lower, right._lower, addend._lower), Vector64.MultiplyAddEstimate(left._upper, right._upper, addend._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector128<TResult> Narrow<TSource, TResult>(Vector128<TSource> lower, Vector128<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector128<TResult>>(out var value);
		for (int i = 0; i < Vector128<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateTruncating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector128<TSource>.Count; j < Vector128<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateTruncating(GetElementUnsafe(in upper, j - Vector128<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> Narrow(Vector128<double> lower, Vector128<double> upper)
	{
		return Narrow<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> Narrow(Vector128<short> lower, Vector128<short> upper)
	{
		return Narrow<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<short> Narrow(Vector128<int> lower, Vector128<int> upper)
	{
		return Narrow<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<int> Narrow(Vector128<long> lower, Vector128<long> upper)
	{
		return Narrow<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<byte> Narrow(Vector128<ushort> lower, Vector128<ushort> upper)
	{
		return Narrow<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> Narrow(Vector128<uint> lower, Vector128<uint> upper)
	{
		return Narrow<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> Narrow(Vector128<ulong> lower, Vector128<ulong> upper)
	{
		return Narrow<ulong, uint>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector128<TResult> NarrowWithSaturation<TSource, TResult>(Vector128<TSource> lower, Vector128<TSource> upper) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector128<TResult>>(out var value);
		for (int i = 0; i < Vector128<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateSaturating(GetElementUnsafe(in lower, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector128<TSource>.Count; j < Vector128<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateSaturating(GetElementUnsafe(in upper, j - Vector128<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> NarrowWithSaturation(Vector128<double> lower, Vector128<double> upper)
	{
		return NarrowWithSaturation<double, float>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> NarrowWithSaturation(Vector128<short> lower, Vector128<short> upper)
	{
		return NarrowWithSaturation<short, sbyte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<short> NarrowWithSaturation(Vector128<int> lower, Vector128<int> upper)
	{
		return NarrowWithSaturation<int, short>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<int> NarrowWithSaturation(Vector128<long> lower, Vector128<long> upper)
	{
		return NarrowWithSaturation<long, int>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<byte> NarrowWithSaturation(Vector128<ushort> lower, Vector128<ushort> upper)
	{
		return NarrowWithSaturation<ushort, byte>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> NarrowWithSaturation(Vector128<uint> lower, Vector128<uint> upper)
	{
		return NarrowWithSaturation<uint, ushort>(lower, upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> NarrowWithSaturation(Vector128<ulong> lower, Vector128<ulong> upper)
	{
		return NarrowWithSaturation<ulong, uint>(lower, upper);
	}

	[Intrinsic]
	public static Vector128<T> Negate<T>(Vector128<T> vector)
	{
		return -vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None<T>(Vector128<T> vector, T value)
	{
		return !EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet<T>(Vector128<T> vector)
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
	public static Vector128<T> OnesComplement<T>(Vector128<T> vector)
	{
		return ~vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> RadiansToDegrees(Vector128<double> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector128<double>, double>(radians);
		}
		return Create(Vector64.RadiansToDegrees(radians._lower), Vector64.RadiansToDegrees(radians._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<float> RadiansToDegrees(Vector128<float> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector128<float>, float>(radians);
		}
		return Create(Vector64.RadiansToDegrees(radians._lower), Vector64.RadiansToDegrees(radians._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector128<T> Round<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector64.Round(vector._lower), Vector64.Round(vector._upper));
	}

	[Intrinsic]
	public static Vector128<double> Round(Vector128<double> vector)
	{
		return Vector128.Round<double>(vector);
	}

	[Intrinsic]
	public static Vector128<float> Round(Vector128<float> vector)
	{
		return Vector128.Round<float>(vector);
	}

	[Intrinsic]
	public static Vector128<double> Round(Vector128<double> vector, MidpointRounding mode)
	{
		return VectorMath.RoundDouble(vector, mode);
	}

	[Intrinsic]
	public static Vector128<float> Round(Vector128<float> vector, MidpointRounding mode)
	{
		return VectorMath.RoundSingle(vector, mode);
	}

	[Intrinsic]
	public static Vector128<byte> ShiftLeft(Vector128<byte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector128<short> ShiftLeft(Vector128<short> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector128<int> ShiftLeft(Vector128<int> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector128<long> ShiftLeft(Vector128<long> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	public static Vector128<nint> ShiftLeft(Vector128<nint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<nuint> ShiftLeft(Vector128<nuint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> ShiftLeft(Vector128<sbyte> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> ShiftLeft(Vector128<ushort> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> ShiftLeft(Vector128<uint> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector128<uint> ShiftLeft(Vector128<uint> vector, Vector128<uint> shiftCount)
	{
		return Create(Vector64.ShiftLeft(vector._lower, shiftCount._lower), Vector64.ShiftLeft(vector._upper, shiftCount._upper));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> ShiftLeft(Vector128<ulong> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	internal static Vector128<ulong> ShiftLeft(Vector128<ulong> vector, Vector128<ulong> shiftCount)
	{
		return Create(Vector64.ShiftLeft(vector._lower, shiftCount._lower), Vector64.ShiftLeft(vector._upper, shiftCount._upper));
	}

	[Intrinsic]
	public static Vector128<short> ShiftRightArithmetic(Vector128<short> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector128<int> ShiftRightArithmetic(Vector128<int> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector128<long> ShiftRightArithmetic(Vector128<long> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector128<nint> ShiftRightArithmetic(Vector128<nint> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> ShiftRightArithmetic(Vector128<sbyte> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	public static Vector128<byte> ShiftRightLogical(Vector128<byte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector128<short> ShiftRightLogical(Vector128<short> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector128<int> ShiftRightLogical(Vector128<int> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector128<long> ShiftRightLogical(Vector128<long> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	public static Vector128<nint> ShiftRightLogical(Vector128<nint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<nuint> ShiftRightLogical(Vector128<nuint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> ShiftRightLogical(Vector128<sbyte> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> ShiftRightLogical(Vector128<ushort> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> ShiftRightLogical(Vector128<uint> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> ShiftRightLogical(Vector128<ulong> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	internal static Vector128<byte> ShuffleNativeFallback(Vector128<byte> vector, Vector128<byte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<sbyte> ShuffleNativeFallback(Vector128<sbyte> vector, Vector128<sbyte> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<short> ShuffleNativeFallback(Vector128<short> vector, Vector128<short> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<ushort> ShuffleNativeFallback(Vector128<ushort> vector, Vector128<ushort> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<int> ShuffleNativeFallback(Vector128<int> vector, Vector128<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<uint> ShuffleNativeFallback(Vector128<uint> vector, Vector128<uint> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<float> ShuffleNativeFallback(Vector128<float> vector, Vector128<int> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<long> ShuffleNativeFallback(Vector128<long> vector, Vector128<long> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<ulong> ShuffleNativeFallback(Vector128<ulong> vector, Vector128<ulong> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	internal static Vector128<double> ShuffleNativeFallback(Vector128<double> vector, Vector128<long> indices)
	{
		return Shuffle(vector, indices);
	}

	[Intrinsic]
	public static Vector128<byte> Shuffle(Vector128<byte> vector, Vector128<byte> indices)
	{
		Unsafe.SkipInit<Vector128<byte>>(out var value);
		for (int i = 0; i < Vector128<byte>.Count; i++)
		{
			byte elementUnsafe = GetElementUnsafe(in indices, i);
			byte value2 = 0;
			if (elementUnsafe < Vector128<byte>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> Shuffle(Vector128<sbyte> vector, Vector128<sbyte> indices)
	{
		Unsafe.SkipInit<Vector128<sbyte>>(out var value);
		for (int i = 0; i < Vector128<sbyte>.Count; i++)
		{
			byte b = (byte)GetElementUnsafe(in indices, i);
			sbyte value2 = 0;
			if (b < Vector128<sbyte>.Count)
			{
				value2 = GetElementUnsafe(in vector, b);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector128<byte> ShuffleNative(Vector128<byte> vector, Vector128<byte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<sbyte> ShuffleNative(Vector128<sbyte> vector, Vector128<sbyte> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector128<short> Shuffle(Vector128<short> vector, Vector128<short> indices)
	{
		Unsafe.SkipInit<Vector128<short>>(out var value);
		for (int i = 0; i < Vector128<short>.Count; i++)
		{
			ushort num = (ushort)GetElementUnsafe(in indices, i);
			short value2 = 0;
			if (num < Vector128<short>.Count)
			{
				value2 = GetElementUnsafe(in vector, num);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> Shuffle(Vector128<ushort> vector, Vector128<ushort> indices)
	{
		Unsafe.SkipInit<Vector128<ushort>>(out var value);
		for (int i = 0; i < Vector128<ushort>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in indices, i);
			ushort value2 = 0;
			if (elementUnsafe < Vector128<ushort>.Count)
			{
				value2 = GetElementUnsafe(in vector, elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector128<short> ShuffleNative(Vector128<short> vector, Vector128<short> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> ShuffleNative(Vector128<ushort> vector, Vector128<ushort> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector128<int> Shuffle(Vector128<int> vector, Vector128<int> indices)
	{
		Unsafe.SkipInit<Vector128<int>>(out var value);
		for (int i = 0; i < Vector128<int>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			int value2 = 0;
			if (elementUnsafe < Vector128<int>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> Shuffle(Vector128<uint> vector, Vector128<uint> indices)
	{
		Unsafe.SkipInit<Vector128<uint>>(out var value);
		for (int i = 0; i < Vector128<uint>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in indices, i);
			uint value2 = 0u;
			if (elementUnsafe < Vector128<uint>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector128<float> Shuffle(Vector128<float> vector, Vector128<int> indices)
	{
		Unsafe.SkipInit<Vector128<float>>(out var value);
		for (int i = 0; i < Vector128<float>.Count; i++)
		{
			uint elementUnsafe = (uint)GetElementUnsafe(in indices, i);
			float value2 = 0f;
			if (elementUnsafe < Vector128<float>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector128<int> ShuffleNative(Vector128<int> vector, Vector128<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> ShuffleNative(Vector128<uint> vector, Vector128<uint> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector128<float> ShuffleNative(Vector128<float> vector, Vector128<int> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector128<long> Shuffle(Vector128<long> vector, Vector128<long> indices)
	{
		Unsafe.SkipInit<Vector128<long>>(out var value);
		for (int i = 0; i < Vector128<long>.Count; i++)
		{
			ulong elementUnsafe = (ulong)GetElementUnsafe(in indices, i);
			long value2 = 0L;
			if (elementUnsafe < (uint)Vector128<long>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> Shuffle(Vector128<ulong> vector, Vector128<ulong> indices)
	{
		Unsafe.SkipInit<Vector128<ulong>>(out var value);
		for (int i = 0; i < Vector128<ulong>.Count; i++)
		{
			ulong elementUnsafe = GetElementUnsafe(in indices, i);
			ulong value2 = 0uL;
			if (elementUnsafe < (uint)Vector128<ulong>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector128<double> Shuffle(Vector128<double> vector, Vector128<long> indices)
	{
		Unsafe.SkipInit<Vector128<double>>(out var value);
		for (int i = 0; i < Vector128<double>.Count; i++)
		{
			ulong elementUnsafe = (ulong)GetElementUnsafe(in indices, i);
			double value2 = 0.0;
			if (elementUnsafe < (uint)Vector128<double>.Count)
			{
				value2 = GetElementUnsafe(in vector, (int)elementUnsafe);
			}
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector128<long> ShuffleNative(Vector128<long> vector, Vector128<long> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> ShuffleNative(Vector128<ulong> vector, Vector128<ulong> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[Intrinsic]
	public static Vector128<double> ShuffleNative(Vector128<double> vector, Vector128<long> indices)
	{
		return ShuffleNativeFallback(vector, indices);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<double> Sin(Vector128<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinDouble<Vector128<double>, Vector128<long>>(vector);
		}
		return Create(Vector64.Sin(vector._lower), Vector64.Sin(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector128<float> Sin(Vector128<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				return VectorMath.SinSingle<Vector128<float>, Vector128<int>, Vector256<double>, Vector256<long>>(vector);
			}
			return VectorMath.SinSingle<Vector128<float>, Vector128<int>, Vector128<double>, Vector128<long>>(vector);
		}
		return Create(Vector64.Sin(vector._lower), Vector64.Sin(vector._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector128<double> Sin, Vector128<double> Cos) SinCos(Vector128<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinCosDouble<Vector128<double>, Vector128<long>>(vector);
		}
		var (lower, lower2) = Vector64.SinCos(vector._lower);
		var (upper, upper2) = Vector64.SinCos(vector._upper);
		return (Sin: Create(lower, upper), Cos: Create(lower2, upper2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector128<float> Sin, Vector128<float> Cos) SinCos(Vector128<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				return VectorMath.SinCosSingle<Vector128<float>, Vector128<int>, Vector256<double>, Vector256<long>>(vector);
			}
			return VectorMath.SinCosSingle<Vector128<float>, Vector128<int>, Vector128<double>, Vector128<long>>(vector);
		}
		var (lower, lower2) = Vector64.SinCos(vector._lower);
		var (upper, upper2) = Vector64.SinCos(vector._upper);
		return (Sin: Create(lower, upper), Cos: Create(lower2, upper2));
	}

	[Intrinsic]
	public static Vector128<T> Sqrt<T>(Vector128<T> vector)
	{
		return Create(Vector64.Sqrt(vector._lower), Vector64.Sqrt(vector._upper));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void Store<T>(this Vector128<T> source, T* destination)
	{
		source.StoreUnsafe(ref *destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned<T>(this Vector128<T> source, T* destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		if ((nuint)destination % (nuint)16u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		*(Vector128<T>*)destination = source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal<T>(this Vector128<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void StoreLowerUnsafe<T>(this Vector128<T> source, ref T destination, nuint elementOffset = 0u)
	{
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref Unsafe.Add(ref destination, elementOffset)), source.AsDouble().ToScalar());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static void StoreUnsafe<T>(this Vector128<T> source, ref T destination)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static void StoreUnsafe<T>(this Vector128<T> source, ref T destination, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[Intrinsic]
	public static Vector128<T> Subtract<T>(Vector128<T> left, Vector128<T> right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> SubtractSaturate<T>(Vector128<T> left, Vector128<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left - right;
		}
		return Create(Vector64.SubtractSaturate(left._lower, right._lower), Vector64.SubtractSaturate(left._upper, right._upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Sum<T>(Vector128<T> vector)
	{
		return Scalar<T>.Add(Vector64.Sum(vector._lower), Vector64.Sum(vector._upper));
	}

	[Intrinsic]
	public static T ToScalar<T>(this Vector128<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		return GetElementUnsafe(in vector, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> ToVector256<T>(this Vector128<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Vector256<T> vector2 = default(Vector256<T>);
		vector2.SetLowerUnsafe<T>(vector);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector256<T> ToVector256Unsafe<T>(this Vector128<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Unsafe.SkipInit<Vector256<T>>(out var value);
		value.SetLowerUnsafe<T>(vector);
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector128<T> Truncate<T>(Vector128<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		return Create(Vector64.Truncate(vector._lower), Vector64.Truncate(vector._upper));
	}

	[Intrinsic]
	public static Vector128<double> Truncate(Vector128<double> vector)
	{
		return Vector128.Truncate<double>(vector);
	}

	[Intrinsic]
	public static Vector128<float> Truncate(Vector128<float> vector)
	{
		return Vector128.Truncate<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryCopyTo<T>(this Vector128<T> vector, Span<T> destination)
	{
		if (destination.Length < Vector128<T>.Count)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), vector);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector128<ushort> Lower, Vector128<ushort> Upper) Widen(Vector128<byte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector128<int> Lower, Vector128<int> Upper) Widen(Vector128<short> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector128<long> Lower, Vector128<long> Upper) Widen(Vector128<int> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector128<short> Lower, Vector128<short> Upper) Widen(Vector128<sbyte> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector128<double> Lower, Vector128<double> Upper) Widen(Vector128<float> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector128<uint> Lower, Vector128<uint> Upper) Widen(Vector128<ushort> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static (Vector128<ulong> Lower, Vector128<ulong> Upper) Widen(Vector128<uint> source)
	{
		return (Lower: WidenLower(source), Upper: WidenUpper(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> WidenLower(Vector128<byte> source)
	{
		Vector64<byte> lower = source._lower;
		return Create(Vector64.WidenLower(lower), Vector64.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<int> WidenLower(Vector128<short> source)
	{
		Vector64<short> lower = source._lower;
		return Create(Vector64.WidenLower(lower), Vector64.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<long> WidenLower(Vector128<int> source)
	{
		Vector64<int> lower = source._lower;
		return Create(Vector64.WidenLower(lower), Vector64.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<short> WidenLower(Vector128<sbyte> source)
	{
		Vector64<sbyte> lower = source._lower;
		return Create(Vector64.WidenLower(lower), Vector64.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> WidenLower(Vector128<float> source)
	{
		Vector64<float> lower = source._lower;
		return Create(Vector64.WidenLower(lower), Vector64.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> WidenLower(Vector128<ushort> source)
	{
		Vector64<ushort> lower = source._lower;
		return Create(Vector64.WidenLower(lower), Vector64.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> WidenLower(Vector128<uint> source)
	{
		Vector64<uint> lower = source._lower;
		return Create(Vector64.WidenLower(lower), Vector64.WidenUpper(lower));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ushort> WidenUpper(Vector128<byte> source)
	{
		Vector64<byte> upper = source._upper;
		return Create(Vector64.WidenLower(upper), Vector64.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<int> WidenUpper(Vector128<short> source)
	{
		Vector64<short> upper = source._upper;
		return Create(Vector64.WidenLower(upper), Vector64.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<long> WidenUpper(Vector128<int> source)
	{
		Vector64<int> upper = source._upper;
		return Create(Vector64.WidenLower(upper), Vector64.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<short> WidenUpper(Vector128<sbyte> source)
	{
		Vector64<sbyte> upper = source._upper;
		return Create(Vector64.WidenLower(upper), Vector64.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<double> WidenUpper(Vector128<float> source)
	{
		Vector64<float> upper = source._upper;
		return Create(Vector64.WidenLower(upper), Vector64.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<uint> WidenUpper(Vector128<ushort> source)
	{
		Vector64<ushort> upper = source._upper;
		return Create(Vector64.WidenLower(upper), Vector64.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector128<ulong> WidenUpper(Vector128<uint> source)
	{
		Vector64<uint> upper = source._upper;
		return Create(Vector64.WidenLower(upper), Vector64.WidenUpper(upper));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> WithElement<T>(this Vector128<T> vector, int index, T value)
	{
		if ((uint)index >= (uint)Vector128<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		Vector128<T> vector2 = vector;
		SetElementUnsafe(in vector2, index, value);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> WithLower<T>(this Vector128<T> vector, Vector64<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Vector128<T> vector2 = vector;
		vector2.SetLowerUnsafe<T>(value);
		return vector2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> WithUpper<T>(this Vector128<T> vector, Vector64<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Vector128<T> vector2 = vector;
		vector2.SetUpperUnsafe<T>(value);
		return vector2;
	}

	[Intrinsic]
	public static Vector128<T> Xor<T>(Vector128<T> left, Vector128<T> right)
	{
		return left ^ right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T GetElementUnsafe<T>(this in Vector128<T> vector, int index)
	{
		return Unsafe.Add(ref Unsafe.As<Vector128<T>, T>(ref Unsafe.AsRef(in vector)), index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetElementUnsafe<T>(this in Vector128<T> vector, int index, T value)
	{
		Unsafe.Add(ref Unsafe.As<Vector128<T>, T>(ref Unsafe.AsRef(in vector)), index) = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetLowerUnsafe<T>(this in Vector128<T> vector, Vector64<T> value)
	{
		Unsafe.AsRef(in vector._lower) = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetUpperUnsafe<T>(this in Vector128<T> vector, Vector64<T> value)
	{
		Unsafe.AsRef(in vector._upper) = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal static Vector128<byte> UnpackLow(Vector128<byte> left, Vector128<byte> right)
	{
		if (Sse2.IsSupported)
		{
			return Sse2.UnpackLow(left, right);
		}
		_ = 0;
		ThrowHelper.ThrowNotSupportedException();
		return AdvSimd.Arm64.ZipLow(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal static Vector128<byte> UnpackHigh(Vector128<byte> left, Vector128<byte> right)
	{
		if (Sse2.IsSupported)
		{
			return Sse2.UnpackHigh(left, right);
		}
		_ = 0;
		ThrowHelper.ThrowNotSupportedException();
		return AdvSimd.Arm64.ZipHigh(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool All(Vector2 vector, float value)
	{
		return vector.AsVector128() == Vector2.Create(value).AsVector128();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool All(Vector3 vector, float value)
	{
		return vector.AsVector128() == Vector3.Create(value).AsVector128();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool AllWhereAllBitsSet(Vector2 vector)
	{
		return vector.AsVector128().AsInt32() == Vector2.AllBitsSet.AsVector128().AsInt32();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool AllWhereAllBitsSet(Vector3 vector)
	{
		return vector.AsVector128().AsInt32() == Vector3.AllBitsSet.AsVector128().AsInt32();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool Any(Vector2 vector, float value)
	{
		return EqualsAny(vector.AsVector128(), Create(value, value, -1f, -1f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool Any(Vector3 vector, float value)
	{
		return EqualsAny(vector.AsVector128(), Create(value, value, value, -1f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool AnyWhereAllBitsSet(Vector2 vector)
	{
		return EqualsAny(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool AnyWhereAllBitsSet(Vector3 vector)
	{
		return EqualsAny(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet);
	}

	[Intrinsic]
	public static Plane AsPlane(this Vector128<float> value)
	{
		return Unsafe.BitCast<Vector128<float>, Plane>(value);
	}

	[Intrinsic]
	public static Quaternion AsQuaternion(this Vector128<float> value)
	{
		return Unsafe.BitCast<Vector128<float>, Quaternion>(value);
	}

	[Intrinsic]
	public static Vector128<float> AsVector128(this Plane value)
	{
		return Unsafe.BitCast<Plane, Vector128<float>>(value);
	}

	[Intrinsic]
	public static Vector128<float> AsVector128(this Quaternion value)
	{
		return Unsafe.BitCast<Quaternion, Vector128<float>>(value);
	}

	[Intrinsic]
	public static Vector128<float> AsVector128(this Vector2 value)
	{
		return Vector4.Create(value, 0f, 0f).AsVector128();
	}

	[Intrinsic]
	public static Vector128<float> AsVector128(this Vector3 value)
	{
		return Vector4.Create(value, 0f).AsVector128();
	}

	[Intrinsic]
	public static Vector128<float> AsVector128(this Vector4 value)
	{
		return Unsafe.BitCast<Vector4, Vector128<float>>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> AsVector128<T>(this Vector<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		return Unsafe.ReadUnaligned<Vector128<T>>(in Unsafe.As<Vector<T>, byte>(ref value));
	}

	[Intrinsic]
	public static Vector128<float> AsVector128Unsafe(this Vector2 value)
	{
		Unsafe.SkipInit<Vector128<float>>(out var value2);
		Unsafe.WriteUnaligned(ref Unsafe.As<Vector128<float>, byte>(ref value2), value);
		return value2;
	}

	[Intrinsic]
	public static Vector128<float> AsVector128Unsafe(this Vector3 value)
	{
		Unsafe.SkipInit<Vector128<float>>(out var value2);
		Unsafe.WriteUnaligned(ref Unsafe.As<Vector128<float>, byte>(ref value2), value);
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 AsVector2(this Vector128<float> value)
	{
		return Unsafe.ReadUnaligned<Vector2>(in Unsafe.As<Vector128<float>, byte>(ref value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 AsVector3(this Vector128<float> value)
	{
		return Unsafe.ReadUnaligned<Vector3>(in Unsafe.As<Vector128<float>, byte>(ref value));
	}

	[Intrinsic]
	public static Vector4 AsVector4(this Vector128<float> value)
	{
		return Unsafe.BitCast<Vector128<float>, Vector4>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> AsVector<T>(this Vector128<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		Vector<T> source = default(Vector<T>);
		Unsafe.WriteUnaligned(ref Unsafe.As<Vector<T>, byte>(ref source), value);
		return source;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int Count(Vector2 vector, float value)
	{
		return BitOperations.PopCount(Equals(vector.AsVector128(), Create(value, value, -1f, -1f)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int Count(Vector3 vector, float value)
	{
		return BitOperations.PopCount(Equals(vector.AsVector128(), Create(value, value, value, -1f)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int CountWhereAllBitsSet(Vector2 vector)
	{
		return BitOperations.PopCount(Equals(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int CountWhereAllBitsSet(Vector3 vector)
	{
		return BitOperations.PopCount(Equals(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int IndexOf(Vector2 vector, float value)
	{
		int num = BitOperations.TrailingZeroCount(Equals(vector.AsVector128(), Create(value, value, -1f, -1f)).ExtractMostSignificantBits());
		if (num == 32)
		{
			return -1;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int IndexOf(Vector3 vector, float value)
	{
		int num = BitOperations.TrailingZeroCount(Equals(vector.AsVector128(), Create(value, value, value, -1f)).ExtractMostSignificantBits());
		if (num == 32)
		{
			return -1;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int IndexOfWhereAllBitsSet(Vector2 vector)
	{
		int num = BitOperations.TrailingZeroCount(Equals(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet).ExtractMostSignificantBits());
		if (num == 32)
		{
			return -1;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int IndexOfWhereAllBitsSet(Vector3 vector)
	{
		int num = BitOperations.TrailingZeroCount(Equals(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet).ExtractMostSignificantBits());
		if (num == 32)
		{
			return -1;
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int LastIndexOf(Vector2 vector, float value)
	{
		return 31 - BitOperations.LeadingZeroCount(Equals(vector.AsVector128(), Create(value, value, -1f, -1f)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int LastIndexOf(Vector3 vector, float value)
	{
		return 31 - BitOperations.LeadingZeroCount(Equals(vector.AsVector128(), Create(value, value, value, -1f)).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int LastIndexOfWhereAllBitsSet(Vector2 vector)
	{
		return 31 - BitOperations.LeadingZeroCount(Equals(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static int LastIndexOfWhereAllBitsSet(Vector3 vector)
	{
		return 31 - BitOperations.LeadingZeroCount(Equals(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet).ExtractMostSignificantBits());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool None(Vector2 vector, float value)
	{
		return !EqualsAny(vector.AsVector128(), Create(value, value, -1f, -1f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool None(Vector3 vector, float value)
	{
		return !EqualsAny(vector.AsVector128(), Create(value, value, value, -1f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool NoneWhereAllBitsSet(Vector2 vector)
	{
		return !EqualsAny(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool NoneWhereAllBitsSet(Vector3 vector)
	{
		return !EqualsAny(vector.AsVector128().AsInt32(), Vector128<int>.AllBitsSet);
	}
}
[StructLayout(LayoutKind.Sequential, Size = 16)]
[Intrinsic]
[DebuggerDisplay("{DisplayString,nq}")]
[DebuggerTypeProxy(typeof(Vector128DebugView<>))]
public readonly struct Vector128<T> : ISimdVector<Vector128<T>, T>, IAdditionOperators<Vector128<T>, Vector128<T>, Vector128<T>>, IBitwiseOperators<Vector128<T>, Vector128<T>, Vector128<T>>, IDivisionOperators<Vector128<T>, Vector128<T>, Vector128<T>>, IEqualityOperators<Vector128<T>, Vector128<T>, bool>, IEquatable<Vector128<T>>, IMultiplyOperators<Vector128<T>, Vector128<T>, Vector128<T>>, IShiftOperators<Vector128<T>, int, Vector128<T>>, ISubtractionOperators<Vector128<T>, Vector128<T>, Vector128<T>>, IUnaryNegationOperators<Vector128<T>, Vector128<T>>, IUnaryPlusOperators<Vector128<T>, Vector128<T>>
{
	internal readonly Vector64<T> _lower;

	internal readonly Vector64<T> _upper;

	public static Vector128<T> AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector128.Create(Scalar<T>.AllBitsSet);
		}
	}

	public static int Count
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
			return 16 / Unsafe.SizeOf<T>();
		}
	}

	public static Vector128<T> Indices
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
			Unsafe.SkipInit<Vector128<T>>(out var value);
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

	public static Vector128<T> One
	{
		[Intrinsic]
		get
		{
			return Vector128.Create(Scalar<T>.One);
		}
	}

	public static Vector128<T> Zero
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
			return default(Vector128<T>);
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

	static int ISimdVector<Vector128<T>, T>.Alignment => 16;

	static int ISimdVector<Vector128<T>, T>.ElementCount => Count;

	static bool ISimdVector<Vector128<T>, T>.IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return Vector128.IsHardwareAccelerated;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator +(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Create(left._lower + right._lower, left._upper + right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator &(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Create(left._lower & right._lower, left._upper & right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator |(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Create(left._lower | right._lower, left._upper | right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator /(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Create(left._lower / right._lower, left._upper / right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator /(Vector128<T> left, T right)
	{
		return Vector128.Create(left._lower / right, left._upper / right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector128<T> left, Vector128<T> right)
	{
		if (left._lower == right._lower)
		{
			return left._upper == right._upper;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator ^(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Create(left._lower ^ right._lower, left._upper ^ right._upper);
	}

	[Intrinsic]
	public static bool operator !=(Vector128<T> left, Vector128<T> right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator <<(Vector128<T> value, int shiftCount)
	{
		return Vector128.Create(value._lower << shiftCount, value._upper << shiftCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator *(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Create(left._lower * right._lower, left._upper * right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator *(Vector128<T> left, T right)
	{
		return Vector128.Create(left._lower * right, left._upper * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator *(T left, Vector128<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator ~(Vector128<T> vector)
	{
		return Vector128.Create(~vector._lower, ~vector._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator >>(Vector128<T> value, int shiftCount)
	{
		return Vector128.Create(value._lower >> shiftCount, value._upper >> shiftCount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator -(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Create(left._lower - right._lower, left._upper - right._upper);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator -(Vector128<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return vector ^ Vector128.Create(-0f).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return vector ^ Vector128.Create(-0.0).As<double, T>();
		}
		return Zero - vector;
	}

	[Intrinsic]
	public static Vector128<T> operator +(Vector128<T> value)
	{
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector128<T> operator >>>(Vector128<T> value, int shiftCount)
	{
		return Vector128.Create(value._lower >>> shiftCount, value._upper >>> shiftCount);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector128<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static bool EqualsFloatingPoint(Vector128<T> lhs, Vector128<T> rhs)
	{
		return (Vector128.Equals(lhs, rhs) | ~(Vector128.Equals(lhs, lhs) | Vector128.Equals(rhs, rhs))).AsInt32() == Vector128<int>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector128<T> other)
	{
		if (Vector128.IsHardwareAccelerated)
		{
			if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
			{
				return EqualsFloatingPoint(this, other);
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
		ThrowHelper.ThrowForUnsupportedIntrinsicsVector128BaseType<T>();
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
	static Vector128<T> ISimdVector<Vector128<T>, T>.Abs(Vector128<T> vector)
	{
		return Vector128.Abs(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Add(Vector128<T> left, Vector128<T> right)
	{
		return left + right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.All(Vector128<T> vector, T value)
	{
		return Vector128.All(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.AllWhereAllBitsSet(Vector128<T> vector)
	{
		return Vector128.AllWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.AndNot(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.AndNot(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.Any(Vector128<T> vector, T value)
	{
		return Vector128.Any(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.AnyWhereAllBitsSet(Vector128<T> vector)
	{
		return Vector128.AnyWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.BitwiseAnd(Vector128<T> left, Vector128<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.BitwiseOr(Vector128<T> left, Vector128<T> right)
	{
		return left | right;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Ceiling(Vector128<T> vector)
	{
		return Vector128.Ceiling(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Clamp(Vector128<T> value, Vector128<T> min, Vector128<T> max)
	{
		return Vector128.Clamp(value, min, max);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.ClampNative(Vector128<T> value, Vector128<T> min, Vector128<T> max)
	{
		return Vector128.ClampNative(value, min, max);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.ConditionalSelect(Vector128<T> condition, Vector128<T> left, Vector128<T> right)
	{
		return Vector128.ConditionalSelect(condition, left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.CopySign(Vector128<T> value, Vector128<T> sign)
	{
		return Vector128.CopySign(value, sign);
	}

	static void ISimdVector<Vector128<T>, T>.CopyTo(Vector128<T> vector, T[] destination)
	{
		vector.CopyTo(destination);
	}

	static void ISimdVector<Vector128<T>, T>.CopyTo(Vector128<T> vector, T[] destination, int startIndex)
	{
		vector.CopyTo(destination, startIndex);
	}

	static void ISimdVector<Vector128<T>, T>.CopyTo(Vector128<T> vector, Span<T> destination)
	{
		vector.CopyTo(destination);
	}

	[Intrinsic]
	static int ISimdVector<Vector128<T>, T>.Count(Vector128<T> vector, T value)
	{
		return Vector128.Count(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector128<T>, T>.CountWhereAllBitsSet(Vector128<T> vector)
	{
		return Vector128.CountWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Create(T value)
	{
		return Vector128.Create(value);
	}

	static Vector128<T> ISimdVector<Vector128<T>, T>.Create(T[] values)
	{
		return Vector128.Create(values);
	}

	static Vector128<T> ISimdVector<Vector128<T>, T>.Create(T[] values, int index)
	{
		return Vector128.Create(values, index);
	}

	static Vector128<T> ISimdVector<Vector128<T>, T>.Create(ReadOnlySpan<T> values)
	{
		return Vector128.Create(values);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.CreateScalar(T value)
	{
		return Vector128.CreateScalar(value);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.CreateScalarUnsafe(T value)
	{
		return Vector128.CreateScalarUnsafe(value);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Divide(Vector128<T> left, Vector128<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Divide(Vector128<T> left, T right)
	{
		return left / right;
	}

	[Intrinsic]
	static T ISimdVector<Vector128<T>, T>.Dot(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Dot(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Equals(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Equals(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.EqualsAll(Vector128<T> left, Vector128<T> right)
	{
		return left == right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.EqualsAny(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.EqualsAny(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Floor(Vector128<T> vector)
	{
		return Vector128.Floor(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector128<T>, T>.GetElement(Vector128<T> vector, int index)
	{
		return vector.GetElement(index);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.GreaterThan(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.GreaterThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.GreaterThanAll(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.GreaterThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.GreaterThanAny(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.GreaterThanAny(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.GreaterThanOrEqual(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.GreaterThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.GreaterThanOrEqualAll(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.GreaterThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.GreaterThanOrEqualAny(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.GreaterThanOrEqualAny(left, right);
	}

	[Intrinsic]
	static int ISimdVector<Vector128<T>, T>.IndexOf(Vector128<T> vector, T value)
	{
		return Vector128.IndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector128<T>, T>.IndexOfWhereAllBitsSet(Vector128<T> vector)
	{
		return Vector128.IndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsEvenInteger(Vector128<T> vector)
	{
		return Vector128.IsEvenInteger(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsFinite(Vector128<T> vector)
	{
		return Vector128.IsFinite(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsInfinity(Vector128<T> vector)
	{
		return Vector128.IsInfinity(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsInteger(Vector128<T> vector)
	{
		return Vector128.IsInteger(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsNaN(Vector128<T> vector)
	{
		return Vector128.IsNaN(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsNegative(Vector128<T> vector)
	{
		return Vector128.IsNegative(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsNegativeInfinity(Vector128<T> vector)
	{
		return Vector128.IsNegativeInfinity(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsNormal(Vector128<T> vector)
	{
		return Vector128.IsNormal(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsOddInteger(Vector128<T> vector)
	{
		return Vector128.IsOddInteger(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsPositive(Vector128<T> vector)
	{
		return Vector128.IsPositive(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsPositiveInfinity(Vector128<T> vector)
	{
		return Vector128.IsPositiveInfinity(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsSubnormal(Vector128<T> vector)
	{
		return Vector128.IsSubnormal(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.IsZero(Vector128<T> vector)
	{
		return Vector128.IsZero(vector);
	}

	[Intrinsic]
	static int ISimdVector<Vector128<T>, T>.LastIndexOf(Vector128<T> vector, T value)
	{
		return Vector128.LastIndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector128<T>, T>.LastIndexOfWhereAllBitsSet(Vector128<T> vector)
	{
		return Vector128.LastIndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.LessThan(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.LessThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.LessThanAll(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.LessThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.LessThanAny(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.LessThanAny(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.LessThanOrEqual(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.LessThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.LessThanOrEqualAll(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.LessThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.LessThanOrEqualAny(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.LessThanOrEqualAny(left, right);
	}

	[Intrinsic]
	unsafe static Vector128<T> ISimdVector<Vector128<T>, T>.Load(T* source)
	{
		return Vector128.Load(source);
	}

	[Intrinsic]
	unsafe static Vector128<T> ISimdVector<Vector128<T>, T>.LoadAligned(T* source)
	{
		return Vector128.LoadAligned(source);
	}

	[Intrinsic]
	unsafe static Vector128<T> ISimdVector<Vector128<T>, T>.LoadAlignedNonTemporal(T* source)
	{
		return Vector128.LoadAlignedNonTemporal(source);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.LoadUnsafe(ref readonly T source)
	{
		return Vector128.LoadUnsafe(in source);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.LoadUnsafe(ref readonly T source, nuint elementOffset)
	{
		return Vector128.LoadUnsafe(in source, elementOffset);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Max(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Max(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MaxMagnitude(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MaxMagnitude(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MaxMagnitudeNumber(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MaxMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MaxNative(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MaxNative(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MaxNumber(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MaxNumber(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Min(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.Min(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MinMagnitude(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MinMagnitude(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MinMagnitudeNumber(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MinMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MinNative(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MinNative(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MinNumber(Vector128<T> left, Vector128<T> right)
	{
		return Vector128.MinNumber(left, right);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Multiply(Vector128<T> left, Vector128<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Multiply(Vector128<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.MultiplyAddEstimate(Vector128<T> left, Vector128<T> right, Vector128<T> addend)
	{
		return Vector128.MultiplyAddEstimate(left, right, addend);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Negate(Vector128<T> vector)
	{
		return -vector;
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.None(Vector128<T> vector, T value)
	{
		return Vector128.None(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector128<T>, T>.NoneWhereAllBitsSet(Vector128<T> vector)
	{
		return Vector128.NoneWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.OnesComplement(Vector128<T> vector)
	{
		return ~vector;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Round(Vector128<T> vector)
	{
		return Vector128.Round(vector);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.ShiftLeft(Vector128<T> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.ShiftRightArithmetic(Vector128<T> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.ShiftRightLogical(Vector128<T> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Sqrt(Vector128<T> vector)
	{
		return Vector128.Sqrt(vector);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector128<T>, T>.Store(Vector128<T> source, T* destination)
	{
		source.Store(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector128<T>, T>.StoreAligned(Vector128<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector128<T>, T>.StoreAlignedNonTemporal(Vector128<T> source, T* destination)
	{
		source.StoreAlignedNonTemporal(destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector128<T>, T>.StoreUnsafe(Vector128<T> vector, ref T destination)
	{
		vector.StoreUnsafe(ref destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector128<T>, T>.StoreUnsafe(Vector128<T> vector, ref T destination, nuint elementOffset)
	{
		vector.StoreUnsafe(ref destination, elementOffset);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Subtract(Vector128<T> left, Vector128<T> right)
	{
		return left - right;
	}

	[Intrinsic]
	static T ISimdVector<Vector128<T>, T>.Sum(Vector128<T> vector)
	{
		return Vector128.Sum(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector128<T>, T>.ToScalar(Vector128<T> vector)
	{
		return vector.ToScalar();
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Truncate(Vector128<T> vector)
	{
		return Vector128.Truncate(vector);
	}

	static bool ISimdVector<Vector128<T>, T>.TryCopyTo(Vector128<T> vector, Span<T> destination)
	{
		return vector.TryCopyTo(destination);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.WithElement(Vector128<T> vector, int index, T value)
	{
		return vector.WithElement(index, value);
	}

	[Intrinsic]
	static Vector128<T> ISimdVector<Vector128<T>, T>.Xor(Vector128<T> left, Vector128<T> right)
	{
		return left ^ right;
	}
}
