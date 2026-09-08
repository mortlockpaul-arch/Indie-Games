using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace System.Numerics;

[Intrinsic]
public static class Vector
{
	internal static int Alignment => Vector<byte>.Count;

	public static bool IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return IsHardwareAccelerated;
		}
	}

	[Intrinsic]
	public static Vector4 AsVector4(this Plane value)
	{
		return Unsafe.BitCast<Plane, Vector4>(value);
	}

	[Intrinsic]
	public static Vector4 AsVector4(this Quaternion value)
	{
		return Unsafe.BitCast<Quaternion, Vector4>(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> Abs<T>(Vector<T> value)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return value;
		}
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value3 = Scalar<T>.Abs(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	public static Vector<T> Add<T>(Vector<T> left, Vector<T> right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> AddSaturate<T>(Vector<T> left, Vector<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left + right;
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.AddSaturate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All<T>(Vector<T> vector, T value)
	{
		return vector == Create(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return All(vector.As<T, int>(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return All(vector.As<T, long>(), -1L);
		}
		return All(vector, Scalar<T>.AllBitsSet);
	}

	[Intrinsic]
	public static Vector<T> AndNot<T>(Vector<T> left, Vector<T> right)
	{
		return left & ~right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any<T>(Vector<T> vector, T value)
	{
		return EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Any(vector.As<T, int>(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return Any(vector.As<T, long>(), -1L);
		}
		return Any(vector, Scalar<T>.AllBitsSet);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<TTo> As<TFrom, TTo>(this Vector<TFrom> vector)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<TFrom>();
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<TTo>();
		return Unsafe.BitCast<Vector<TFrom>, Vector<TTo>>(vector);
	}

	[Intrinsic]
	public static Vector<byte> AsVectorByte<T>(Vector<T> value)
	{
		return value.As<T, byte>();
	}

	[Intrinsic]
	public static Vector<double> AsVectorDouble<T>(Vector<T> value)
	{
		return value.As<T, double>();
	}

	[Intrinsic]
	public static Vector<short> AsVectorInt16<T>(Vector<T> value)
	{
		return value.As<T, short>();
	}

	[Intrinsic]
	public static Vector<int> AsVectorInt32<T>(Vector<T> value)
	{
		return value.As<T, int>();
	}

	[Intrinsic]
	public static Vector<long> AsVectorInt64<T>(Vector<T> value)
	{
		return value.As<T, long>();
	}

	[Intrinsic]
	public static Vector<nint> AsVectorNInt<T>(Vector<T> value)
	{
		return value.As<T, nint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<nuint> AsVectorNUInt<T>(Vector<T> value)
	{
		return value.As<T, nuint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<sbyte> AsVectorSByte<T>(Vector<T> value)
	{
		return value.As<T, sbyte>();
	}

	[Intrinsic]
	public static Vector<float> AsVectorSingle<T>(Vector<T> value)
	{
		return value.As<T, float>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ushort> AsVectorUInt16<T>(Vector<T> value)
	{
		return value.As<T, ushort>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> AsVectorUInt32<T>(Vector<T> value)
	{
		return value.As<T, uint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ulong> AsVectorUInt64<T>(Vector<T> value)
	{
		return value.As<T, ulong>();
	}

	[Intrinsic]
	public static Vector<T> BitwiseAnd<T>(Vector<T> left, Vector<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector<T> BitwiseOr<T>(Vector<T> left, Vector<T> right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector<T> Ceiling<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.Ceiling(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> Ceiling(Vector<double> value)
	{
		Unsafe.SkipInit<Vector<double>>(out var value2);
		for (int i = 0; i < Vector<double>.Count; i++)
		{
			double value3 = Scalar<double>.Ceiling(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> Ceiling(Vector<float> value)
	{
		Unsafe.SkipInit<Vector<float>>(out var value2);
		for (int i = 0; i < Vector<float>.Count; i++)
		{
			float value3 = Scalar<float>.Ceiling(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	public static Vector<T> Clamp<T>(Vector<T> value, Vector<T> min, Vector<T> max)
	{
		return Min(Max(value, min), max);
	}

	[Intrinsic]
	public static Vector<T> ClampNative<T>(Vector<T> value, Vector<T> min, Vector<T> max)
	{
		return MinNative(MaxNative(value, min), max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> ConditionalSelect<T>(Vector<T> condition, Vector<T> left, Vector<T> right)
	{
		return (left & condition) | AndNot(right, condition);
	}

	[Intrinsic]
	public static Vector<float> ConditionalSelect(Vector<int> condition, Vector<float> left, Vector<float> right)
	{
		return ConditionalSelect(condition.As<int, float>(), left, right);
	}

	[Intrinsic]
	public static Vector<double> ConditionalSelect(Vector<long> condition, Vector<double> left, Vector<double> right)
	{
		return ConditionalSelect(condition.As<long, double>(), left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> ConvertToDouble(Vector<long> value)
	{
		if (Vector<double>.Count == Vector512<double>.Count)
		{
			return Vector512.ConvertToDouble(value.AsVector512()).AsVector();
		}
		if (Vector<double>.Count == Vector256<double>.Count)
		{
			return Vector256.ConvertToDouble(value.AsVector256()).AsVector();
		}
		return Vector128.ConvertToDouble(value.AsVector128()).AsVector();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<double> ConvertToDouble(Vector<ulong> value)
	{
		if (Vector<double>.Count == Vector512<double>.Count)
		{
			return Vector512.ConvertToDouble(value.AsVector512()).AsVector();
		}
		if (Vector<double>.Count == Vector256<double>.Count)
		{
			return Vector256.ConvertToDouble(value.AsVector256()).AsVector();
		}
		return Vector128.ConvertToDouble(value.AsVector128()).AsVector();
	}

	[Intrinsic]
	public static Vector<int> ConvertToInt32(Vector<float> value)
	{
		Unsafe.SkipInit<Vector<int>>(out var value2);
		for (int i = 0; i < Vector<int>.Count; i++)
		{
			int value3 = float.ConvertToInteger<int>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	public static Vector<int> ConvertToInt32Native(Vector<float> value)
	{
		Unsafe.SkipInit<Vector<int>>(out var value2);
		for (int i = 0; i < Vector<int>.Count; i++)
		{
			int value3 = float.ConvertToIntegerNative<int>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	public static Vector<long> ConvertToInt64(Vector<double> value)
	{
		Unsafe.SkipInit<Vector<long>>(out var value2);
		for (int i = 0; i < Vector<long>.Count; i++)
		{
			long value3 = double.ConvertToInteger<long>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	public static Vector<long> ConvertToInt64Native(Vector<double> value)
	{
		Unsafe.SkipInit<Vector<long>>(out var value2);
		for (int i = 0; i < Vector<long>.Count; i++)
		{
			long value3 = double.ConvertToIntegerNative<long>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	public static Vector<float> ConvertToSingle(Vector<int> value)
	{
		Unsafe.SkipInit<Vector<float>>(out var value2);
		for (int i = 0; i < Vector<float>.Count; i++)
		{
			float value3 = GetElementUnsafe(in value, i);
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<float> ConvertToSingle(Vector<uint> value)
	{
		if (Vector<float>.Count == Vector512<float>.Count)
		{
			return Vector512.ConvertToSingle(value.AsVector512()).AsVector();
		}
		if (Vector<float>.Count == Vector256<float>.Count)
		{
			return Vector256.ConvertToSingle(value.AsVector256()).AsVector();
		}
		return Vector128.ConvertToSingle(value.AsVector128()).AsVector();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> ConvertToUInt32(Vector<float> value)
	{
		Unsafe.SkipInit<Vector<uint>>(out var value2);
		for (int i = 0; i < Vector<uint>.Count; i++)
		{
			uint value3 = float.ConvertToInteger<uint>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> ConvertToUInt32Native(Vector<float> value)
	{
		Unsafe.SkipInit<Vector<uint>>(out var value2);
		for (int i = 0; i < Vector<uint>.Count; i++)
		{
			uint value3 = float.ConvertToIntegerNative<uint>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ulong> ConvertToUInt64(Vector<double> value)
	{
		Unsafe.SkipInit<Vector<ulong>>(out var value2);
		for (int i = 0; i < Vector<ulong>.Count; i++)
		{
			ulong value3 = double.ConvertToInteger<ulong>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ulong> ConvertToUInt64Native(Vector<double> value)
	{
		Unsafe.SkipInit<Vector<ulong>>(out var value2);
		for (int i = 0; i < Vector<ulong>.Count; i++)
		{
			ulong value3 = double.ConvertToIntegerNative<ulong>(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	internal static Vector<T> Cos<T>(Vector<T> vector) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.Cos(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<double> Cos(Vector<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.CosDouble<Vector<double>, Vector<long>>(vector);
		}
		return Vector.Cos<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<float> Cos(Vector<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.CosSingle<Vector<float>, Vector<int>, Vector<double>, Vector<long>>(vector);
		}
		return Vector.Cos<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> CopySign<T>(Vector<T> value, Vector<T> sign)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return value;
		}
		if (IsHardwareAccelerated)
		{
			return VectorMath.CopySign<Vector<T>, T>(value, sign);
		}
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value3 = Scalar<T>.CopySign(GetElementUnsafe(in value, i), GetElementUnsafe(in sign, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count<T>(Vector<T> vector, T value)
	{
		if (Vector<T>.Count == Vector512<T>.Count)
		{
			return Vector512.Count(vector.AsVector512(), value);
		}
		if (Vector<T>.Count == Vector256<T>.Count)
		{
			return Vector256.Count(vector.AsVector256(), value);
		}
		return Vector128.Count(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Count(vector.As<T, int>(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return Count(vector.As<T, long>(), -1L);
		}
		return Count(vector, Scalar<T>.AllBitsSet);
	}

	[Intrinsic]
	public static Vector<T> Create<T>(T value)
	{
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			SetElementUnsafe(in value2, i, value);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<T> Create<T>(ReadOnlySpan<T> values)
	{
		if (values.Length < Vector<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[Intrinsic]
	public static Vector<T> CreateScalar<T>(T value)
	{
		Vector<T> vector = Vector<T>.Zero;
		SetElementUnsafe(in vector, 0, value);
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> CreateScalarUnsafe<T>(T value)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		Unsafe.SkipInit<Vector<T>>(out var value2);
		SetElementUnsafe(in value2, 0, value);
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> CreateSequence<T>(T start, T step)
	{
		return Vector<T>.Indices * step + Create(start);
	}

	internal static Vector<T> DegreesToRadians<T>(Vector<T> degrees) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.DegreesToRadians(GetElementUnsafe(in degrees, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> DegreesToRadians(Vector<double> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector<double>, double>(degrees);
		}
		return Vector.DegreesToRadians<double>(degrees);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> DegreesToRadians(Vector<float> degrees)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.DegreesToRadians<Vector<float>, float>(degrees);
		}
		return Vector.DegreesToRadians<float>(degrees);
	}

	[Intrinsic]
	public static Vector<T> Divide<T>(Vector<T> left, Vector<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector<T> Divide<T>(Vector<T> left, T right)
	{
		return left / right;
	}

	[Intrinsic]
	public static T Dot<T>(Vector<T> left, Vector<T> right)
	{
		return Sum(left * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> Equals<T>(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = (Scalar<T>.Equals(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<long> Equals(Vector<double> left, Vector<double> right)
	{
		return Vector.Equals<double>(left, right).As<double, long>();
	}

	[Intrinsic]
	public static Vector<int> Equals(Vector<int> left, Vector<int> right)
	{
		return Vector.Equals<int>(left, right);
	}

	[Intrinsic]
	public static Vector<long> Equals(Vector<long> left, Vector<long> right)
	{
		return Vector.Equals<long>(left, right);
	}

	[Intrinsic]
	public static Vector<int> Equals(Vector<float> left, Vector<float> right)
	{
		return Vector.Equals<float>(left, right).As<float, int>();
	}

	[Intrinsic]
	public static bool EqualsAll<T>(Vector<T> left, Vector<T> right)
	{
		return left == right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			if (Scalar<T>.Equals(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return true;
			}
		}
		return false;
	}

	internal static Vector<T> Exp<T>(Vector<T> vector) where T : IExponentialFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.Exp(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<double> Exp(Vector<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.ExpDouble<Vector<double>, Vector<ulong>>(vector);
		}
		return Vector.Exp<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<float> Exp(Vector<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.ExpSingle<Vector<float>, Vector<uint>, Vector<double>, Vector<ulong>>(vector);
		}
		return Vector.Exp<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector<T> Floor<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.Floor(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> Floor(Vector<double> value)
	{
		Unsafe.SkipInit<Vector<double>>(out var value2);
		for (int i = 0; i < Vector<double>.Count; i++)
		{
			double value3 = Scalar<double>.Floor(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> Floor(Vector<float> value)
	{
		Unsafe.SkipInit<Vector<float>>(out var value2);
		for (int i = 0; i < Vector<float>.Count; i++)
		{
			float value3 = Scalar<float>.Floor(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> FusedMultiplyAdd(Vector<double> left, Vector<double> right, Vector<double> addend)
	{
		Unsafe.SkipInit<Vector<double>>(out var value);
		for (int i = 0; i < Vector<double>.Count; i++)
		{
			double value2 = double.FusedMultiplyAdd(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> FusedMultiplyAdd(Vector<float> left, Vector<float> right, Vector<float> addend)
	{
		Unsafe.SkipInit<Vector<float>>(out var value);
		for (int i = 0; i < Vector<float>.Count; i++)
		{
			float value2 = float.FusedMultiplyAdd(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T GetElement<T>(this Vector<T> vector, int index)
	{
		if ((uint)index >= (uint)Vector<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return GetElementUnsafe(in vector, index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> GreaterThan<T>(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = (Scalar<T>.GreaterThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<long> GreaterThan(Vector<double> left, Vector<double> right)
	{
		return Vector.GreaterThan<double>(left, right).As<double, long>();
	}

	[Intrinsic]
	public static Vector<int> GreaterThan(Vector<int> left, Vector<int> right)
	{
		return Vector.GreaterThan<int>(left, right);
	}

	[Intrinsic]
	public static Vector<long> GreaterThan(Vector<long> left, Vector<long> right)
	{
		return Vector.GreaterThan<long>(left, right);
	}

	[Intrinsic]
	public static Vector<int> GreaterThan(Vector<float> left, Vector<float> right)
	{
		return Vector.GreaterThan<float>(left, right).As<float, int>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
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
	public static bool GreaterThanAny<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
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
	public static Vector<T> GreaterThanOrEqual<T>(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = (Scalar<T>.GreaterThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<long> GreaterThanOrEqual(Vector<double> left, Vector<double> right)
	{
		return Vector.GreaterThanOrEqual<double>(left, right).As<double, long>();
	}

	[Intrinsic]
	public static Vector<int> GreaterThanOrEqual(Vector<int> left, Vector<int> right)
	{
		return Vector.GreaterThanOrEqual<int>(left, right);
	}

	[Intrinsic]
	public static Vector<long> GreaterThanOrEqual(Vector<long> left, Vector<long> right)
	{
		return Vector.GreaterThanOrEqual<long>(left, right);
	}

	[Intrinsic]
	public static Vector<int> GreaterThanOrEqual(Vector<float> left, Vector<float> right)
	{
		return Vector.GreaterThanOrEqual<float>(left, right).As<float, int>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
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
	public static bool GreaterThanOrEqualAny<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			if (Scalar<T>.GreaterThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)))
			{
				return true;
			}
		}
		return false;
	}

	internal static Vector<T> Hypot<T>(Vector<T> x, Vector<T> y) where T : IRootFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.Hypot(GetElementUnsafe(in x, i), GetElementUnsafe(in y, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<double> Hypot(Vector<double> x, Vector<double> y)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.HypotDouble<Vector<double>, Vector<ulong>>(x, y);
		}
		return Vector.Hypot<double>(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<float> Hypot(Vector<float> x, Vector<float> y)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.HypotSingle<Vector<float>, Vector<double>>(x, y);
		}
		return Vector.Hypot<float>(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf<T>(Vector<T> vector, T value)
	{
		if (Vector<T>.Count == Vector512<T>.Count)
		{
			return Vector512.IndexOf(vector.AsVector512(), value);
		}
		if (Vector<T>.Count == Vector256<T>.Count)
		{
			return Vector256.IndexOf(vector.AsVector256(), value);
		}
		return Vector128.IndexOf(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOfWhereAllBitsSet<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return IndexOf(vector.As<T, int>(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return IndexOf(vector.As<T, long>(), -1L);
		}
		return IndexOf(vector, Scalar<T>.AllBitsSet);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsEvenInteger<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsEvenIntegerSingle<Vector<float>, Vector<uint>>(vector.As<T, float>()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsEvenIntegerDouble<Vector<double>, Vector<ulong>>(vector.As<T, double>()).As<double, T>();
		}
		return IsZero(vector & Vector<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsFinite<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return ~IsZero(AndNot(Create(2139095040u), vector.As<T, uint>())).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return ~IsZero(AndNot(Create(9218868437227405312uL), vector.As<T, ulong>())).As<ulong, T>();
		}
		return Vector<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsInfinity<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsPositiveInfinity(Abs(vector));
		}
		return Vector<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsInteger<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return IsFinite(vector) & Equals(vector, Truncate(vector));
		}
		return Vector<T>.AllBitsSet;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsNaN<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return ~Equals(vector, vector);
		}
		return Vector<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsNegative<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector<T>.Zero;
		}
		if (typeof(T) == typeof(float))
		{
			return LessThan(vector.As<T, int>(), Vector<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(vector.As<T, long>(), Vector<long>.Zero).As<long, T>();
		}
		return LessThan(vector, Vector<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsNegativeInfinity<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.NegativeInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.NegativeInfinity).As<double, T>());
		}
		return Vector<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsNormal<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).As<T, uint>() - Create(8388608u), Create(2130706432u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).As<T, ulong>() - Create(4503599627370496uL), Create(9214364837600034816uL)).As<ulong, T>();
		}
		return ~IsZero(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsOddInteger<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return VectorMath.IsOddIntegerSingle<Vector<float>, Vector<uint>>(vector.As<T, float>()).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return VectorMath.IsOddIntegerDouble<Vector<double>, Vector<ulong>>(vector.As<T, double>()).As<double, T>();
		}
		return ~IsZero(vector & Vector<T>.One);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsPositive<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return Vector<T>.AllBitsSet;
		}
		if (typeof(T) == typeof(float))
		{
			return GreaterThanOrEqual(vector.As<T, int>(), Vector<int>.Zero).As<int, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return GreaterThanOrEqual(vector.As<T, long>(), Vector<long>.Zero).As<long, T>();
		}
		return GreaterThanOrEqual(vector, Vector<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsPositiveInfinity<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return Equals(vector, Create(float.PositiveInfinity).As<float, T>());
		}
		if (typeof(T) == typeof(double))
		{
			return Equals(vector, Create(double.PositiveInfinity).As<double, T>());
		}
		return Vector<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsSubnormal<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LessThan(Abs(vector).As<T, uint>() - Vector<uint>.One, Create(8388607u)).As<uint, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return LessThan(Abs(vector).As<T, ulong>() - Vector<ulong>.One, Create(4503599627370495uL)).As<ulong, T>();
		}
		return Vector<T>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> IsZero<T>(Vector<T> vector)
	{
		return Equals(vector, Vector<T>.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf<T>(Vector<T> vector, T value)
	{
		if (Vector<T>.Count == Vector512<T>.Count)
		{
			return Vector512.LastIndexOf(vector.AsVector512(), value);
		}
		if (Vector<T>.Count == Vector256<T>.Count)
		{
			return Vector256.LastIndexOf(vector.AsVector256(), value);
		}
		return Vector128.LastIndexOf(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return LastIndexOf(vector.As<T, int>(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return LastIndexOf(vector.As<T, long>(), -1L);
		}
		return LastIndexOf(vector, Scalar<T>.AllBitsSet);
	}

	internal static Vector<T> Lerp<T>(Vector<T> x, Vector<T> y, Vector<T> amount) where T : IFloatingPointIeee754<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.Lerp(GetElementUnsafe(in x, i), GetElementUnsafe(in y, i), GetElementUnsafe(in amount, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> Lerp(Vector<double> x, Vector<double> y, Vector<double> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector<double>, double>(x, y, amount);
		}
		return Vector.Lerp<double>(x, y, amount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> Lerp(Vector<float> x, Vector<float> y, Vector<float> amount)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Lerp<Vector<float>, float>(x, y, amount);
		}
		return Vector.Lerp<float>(x, y, amount);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> LessThan<T>(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = (Scalar<T>.LessThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<long> LessThan(Vector<double> left, Vector<double> right)
	{
		return Vector.LessThan<double>(left, right).As<double, long>();
	}

	[Intrinsic]
	public static Vector<int> LessThan(Vector<int> left, Vector<int> right)
	{
		return Vector.LessThan<int>(left, right);
	}

	[Intrinsic]
	public static Vector<long> LessThan(Vector<long> left, Vector<long> right)
	{
		return Vector.LessThan<long>(left, right);
	}

	[Intrinsic]
	public static Vector<int> LessThan(Vector<float> left, Vector<float> right)
	{
		return Vector.LessThan<float>(left, right).As<float, int>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
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
	public static bool LessThanAny<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
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
	public static Vector<T> LessThanOrEqual<T>(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = (Scalar<T>.LessThanOrEqual(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? Scalar<T>.AllBitsSet : default(T));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<long> LessThanOrEqual(Vector<double> left, Vector<double> right)
	{
		return Vector.LessThanOrEqual<double>(left, right).As<double, long>();
	}

	[Intrinsic]
	public static Vector<int> LessThanOrEqual(Vector<int> left, Vector<int> right)
	{
		return Vector.LessThanOrEqual<int>(left, right);
	}

	[Intrinsic]
	public static Vector<long> LessThanOrEqual(Vector<long> left, Vector<long> right)
	{
		return Vector.LessThanOrEqual<long>(left, right);
	}

	[Intrinsic]
	public static Vector<int> LessThanOrEqual(Vector<float> left, Vector<float> right)
	{
		return Vector.LessThanOrEqual<float>(left, right).As<float, int>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
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
	public static bool LessThanOrEqualAny<T>(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Vector<T>.Count; i++)
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
	public unsafe static Vector<T> Load<T>(T* source)
	{
		return LoadUnsafe(in *source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector<T> LoadAligned<T>(T* source)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		if ((nuint)source % (nuint)(uint)Alignment != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return *(Vector<T>*)source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector<T> LoadAlignedNonTemporal<T>(T* source)
	{
		return LoadAligned(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> LoadUnsafe<T>(ref readonly T source)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		return Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<T, byte>(ref Unsafe.AsRef(in source)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<T> LoadUnsafe<T>(ref readonly T source, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		return Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)));
	}

	internal static Vector<T> Log<T>(Vector<T> vector) where T : ILogarithmicFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.Log(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<double> Log(Vector<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogDouble<Vector<double>, Vector<long>, Vector<ulong>>(vector);
		}
		return Vector.Log<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<float> Log(Vector<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.LogSingle<Vector<float>, Vector<int>, Vector<uint>>(vector);
		}
		return Vector.Log<float>(vector);
	}

	internal static Vector<T> Log2<T>(Vector<T> vector) where T : ILogarithmicFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.Log2(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<double> Log2(Vector<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Double<Vector<double>, Vector<long>, Vector<ulong>>(vector);
		}
		return Vector.Log2<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<float> Log2(Vector<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Log2Single<Vector<float>, Vector<int>, Vector<uint>>(vector);
		}
		return Vector.Log2<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> Max<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Max<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.Max(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MaxMagnitude<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitude<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.MaxMagnitude(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MaxMagnitudeNumber<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxMagnitudeNumber<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.MaxMagnitudeNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MaxNative<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(GreaterThan(left, right), left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = (Scalar<T>.GreaterThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? GetElementUnsafe(in left, i) : GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MaxNumber<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MaxNumber<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.MaxNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> Min<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.Min<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.Min(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MinMagnitude<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitude<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.MinMagnitude(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MinMagnitudeNumber<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinMagnitudeNumber<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.MinMagnitudeNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MinNative<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return ConditionalSelect(LessThan(left, right), left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = (Scalar<T>.LessThan(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i)) ? GetElementUnsafe(in left, i) : GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> MinNumber<T>(Vector<T> left, Vector<T> right)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.MinNumber<Vector<T>, T>(left, right);
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.MinNumber(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<T> Multiply<T>(Vector<T> left, Vector<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector<T> Multiply<T>(Vector<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector<T> Multiply<T>(T left, Vector<T> right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector<T> MultiplyAddEstimate<T>(Vector<T> left, Vector<T> right, Vector<T> addend)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.MultiplyAddEstimate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> MultiplyAddEstimate(Vector<double> left, Vector<double> right, Vector<double> addend)
	{
		Unsafe.SkipInit<Vector<double>>(out var value);
		for (int i = 0; i < Vector<double>.Count; i++)
		{
			double value2 = double.MultiplyAddEstimate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> MultiplyAddEstimate(Vector<float> left, Vector<float> right, Vector<float> addend)
	{
		Unsafe.SkipInit<Vector<float>>(out var value);
		for (int i = 0; i < Vector<float>.Count; i++)
		{
			float value2 = float.MultiplyAddEstimate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i), GetElementUnsafe(in addend, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector<TResult> Narrow<TSource, TResult>(Vector<TSource> low, Vector<TSource> high) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector<TResult>>(out var value);
		for (int i = 0; i < Vector<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateTruncating(GetElementUnsafe(in low, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector<TSource>.Count; j < Vector<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateTruncating(GetElementUnsafe(in high, j - Vector<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> Narrow(Vector<double> low, Vector<double> high)
	{
		return Narrow<double, float>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<sbyte> Narrow(Vector<short> low, Vector<short> high)
	{
		return Narrow<short, sbyte>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<short> Narrow(Vector<int> low, Vector<int> high)
	{
		return Narrow<int, short>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<int> Narrow(Vector<long> low, Vector<long> high)
	{
		return Narrow<long, int>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<byte> Narrow(Vector<ushort> low, Vector<ushort> high)
	{
		return Narrow<ushort, byte>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ushort> Narrow(Vector<uint> low, Vector<uint> high)
	{
		return Narrow<uint, ushort>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> Narrow(Vector<ulong> low, Vector<ulong> high)
	{
		return Narrow<ulong, uint>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector<TResult> NarrowWithSaturation<TSource, TResult>(Vector<TSource> low, Vector<TSource> high) where TSource : INumber<TSource> where TResult : INumber<TResult>
	{
		Unsafe.SkipInit<Vector<TResult>>(out var value);
		for (int i = 0; i < Vector<TSource>.Count; i++)
		{
			TResult value2 = TResult.CreateSaturating(GetElementUnsafe(in low, i));
			value.SetElementUnsafe(i, value2);
		}
		for (int j = Vector<TSource>.Count; j < Vector<TResult>.Count; j++)
		{
			TResult value3 = TResult.CreateSaturating(GetElementUnsafe(in high, j - Vector<TSource>.Count));
			value.SetElementUnsafe(j, value3);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<float> NarrowWithSaturation(Vector<double> low, Vector<double> high)
	{
		return NarrowWithSaturation<double, float>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<sbyte> NarrowWithSaturation(Vector<short> low, Vector<short> high)
	{
		return NarrowWithSaturation<short, sbyte>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<short> NarrowWithSaturation(Vector<int> low, Vector<int> high)
	{
		return NarrowWithSaturation<int, short>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<int> NarrowWithSaturation(Vector<long> low, Vector<long> high)
	{
		return NarrowWithSaturation<long, int>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<byte> NarrowWithSaturation(Vector<ushort> low, Vector<ushort> high)
	{
		return NarrowWithSaturation<ushort, byte>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ushort> NarrowWithSaturation(Vector<uint> low, Vector<uint> high)
	{
		return NarrowWithSaturation<uint, ushort>(low, high);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> NarrowWithSaturation(Vector<ulong> low, Vector<ulong> high)
	{
		return NarrowWithSaturation<ulong, uint>(low, high);
	}

	[Intrinsic]
	public static Vector<T> Negate<T>(Vector<T> value)
	{
		return -value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None<T>(Vector<T> vector, T value)
	{
		return !EqualsAny(vector, Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(float))
		{
			return None(vector.As<T, int>(), -1);
		}
		if (typeof(T) == typeof(double))
		{
			return None(vector.As<T, long>(), -1L);
		}
		return None(vector, Scalar<T>.AllBitsSet);
	}

	[Intrinsic]
	public static Vector<T> OnesComplement<T>(Vector<T> value)
	{
		return ~value;
	}

	internal static Vector<T> RadiansToDegrees<T>(Vector<T> radians) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.RadiansToDegrees(GetElementUnsafe(in radians, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<double> RadiansToDegrees(Vector<double> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector<double>, double>(radians);
		}
		return Vector.RadiansToDegrees<double>(radians);
	}

	[Intrinsic]
	public static Vector<float> RadiansToDegrees(Vector<float> radians)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.RadiansToDegrees<Vector<float>, float>(radians);
		}
		return Vector.RadiansToDegrees<float>(radians);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector<T> Round<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.Round(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<double> Round(Vector<double> vector)
	{
		return Vector.Round<double>(vector);
	}

	[Intrinsic]
	public static Vector<float> Round(Vector<float> vector)
	{
		return Vector.Round<float>(vector);
	}

	[Intrinsic]
	public static Vector<double> Round(Vector<double> vector, MidpointRounding mode)
	{
		return VectorMath.RoundDouble(vector, mode);
	}

	[Intrinsic]
	public static Vector<float> Round(Vector<float> vector, MidpointRounding mode)
	{
		return VectorMath.RoundSingle(vector, mode);
	}

	[Intrinsic]
	public static Vector<byte> ShiftLeft(Vector<byte> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	public static Vector<short> ShiftLeft(Vector<short> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	public static Vector<int> ShiftLeft(Vector<int> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	public static Vector<long> ShiftLeft(Vector<long> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	public static Vector<nint> ShiftLeft(Vector<nint> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<nuint> ShiftLeft(Vector<nuint> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<sbyte> ShiftLeft(Vector<sbyte> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ushort> ShiftLeft(Vector<ushort> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> ShiftLeft(Vector<uint> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	internal static Vector<uint> ShiftLeft(Vector<uint> vector, Vector<uint> shiftCount)
	{
		if (Vector<uint>.Count == Vector512<uint>.Count)
		{
			return Vector512.ShiftLeft(vector.AsVector512(), shiftCount.AsVector512()).AsVector();
		}
		if (Vector<uint>.Count == Vector256<uint>.Count)
		{
			return Vector256.ShiftLeft(vector.AsVector256(), shiftCount.AsVector256()).AsVector();
		}
		return Vector128.ShiftLeft(vector.AsVector128(), shiftCount.AsVector128()).AsVector();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ulong> ShiftLeft(Vector<ulong> value, int shiftCount)
	{
		return value << shiftCount;
	}

	[Intrinsic]
	internal static Vector<ulong> ShiftLeft(Vector<ulong> vector, Vector<ulong> shiftCount)
	{
		if (Vector<ulong>.Count == Vector512<ulong>.Count)
		{
			return Vector512.ShiftLeft(vector.AsVector512(), shiftCount.AsVector512()).AsVector();
		}
		if (Vector<ulong>.Count == Vector256<ulong>.Count)
		{
			return Vector256.ShiftLeft(vector.AsVector256(), shiftCount.AsVector256()).AsVector();
		}
		return Vector128.ShiftLeft(vector.AsVector128(), shiftCount.AsVector128()).AsVector();
	}

	[Intrinsic]
	public static Vector<short> ShiftRightArithmetic(Vector<short> value, int shiftCount)
	{
		return value >> shiftCount;
	}

	[Intrinsic]
	public static Vector<int> ShiftRightArithmetic(Vector<int> value, int shiftCount)
	{
		return value >> shiftCount;
	}

	[Intrinsic]
	public static Vector<long> ShiftRightArithmetic(Vector<long> value, int shiftCount)
	{
		return value >> shiftCount;
	}

	[Intrinsic]
	public static Vector<nint> ShiftRightArithmetic(Vector<nint> value, int shiftCount)
	{
		return value >> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<sbyte> ShiftRightArithmetic(Vector<sbyte> value, int shiftCount)
	{
		return value >> shiftCount;
	}

	[Intrinsic]
	public static Vector<byte> ShiftRightLogical(Vector<byte> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	public static Vector<short> ShiftRightLogical(Vector<short> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	public static Vector<int> ShiftRightLogical(Vector<int> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	public static Vector<long> ShiftRightLogical(Vector<long> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	public static Vector<nint> ShiftRightLogical(Vector<nint> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<nuint> ShiftRightLogical(Vector<nuint> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<sbyte> ShiftRightLogical(Vector<sbyte> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ushort> ShiftRightLogical(Vector<ushort> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> ShiftRightLogical(Vector<uint> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ulong> ShiftRightLogical(Vector<ulong> value, int shiftCount)
	{
		return value >>> shiftCount;
	}

	internal static Vector<T> Sin<T>(Vector<T> vector) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = T.Sin(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<double> Sin(Vector<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinDouble<Vector<double>, Vector<long>>(vector);
		}
		return Vector.Sin<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<float> Sin(Vector<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinSingle<Vector<float>, Vector<int>, Vector<double>, Vector<long>>(vector);
		}
		return Vector.Sin<float>(vector);
	}

	internal static (Vector<T> Sin, Vector<T> Cos) SinCos<T>(Vector<T> vector) where T : ITrigonometricFunctions<T>
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			var (value3, value4) = T.SinCos(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value3);
			value2.SetElementUnsafe(i, value4);
		}
		return (Sin: value, Cos: value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector<double> Sin, Vector<double> Cos) SinCos(Vector<double> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinCosDouble<Vector<double>, Vector<long>>(vector);
		}
		return Vector.SinCos<double>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector<float> Sin, Vector<float> Cos) SinCos(Vector<float> vector)
	{
		if (IsHardwareAccelerated)
		{
			return VectorMath.SinCosSingle<Vector<float>, Vector<int>, Vector<double>, Vector<long>>(vector);
		}
		return Vector.SinCos<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> SquareRoot<T>(Vector<T> value)
	{
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value3 = Scalar<T>.Sqrt(GetElementUnsafe(in value, i));
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void Store<T>(this Vector<T> source, T* destination)
	{
		source.StoreUnsafe(ref *destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned<T>(this Vector<T> source, T* destination)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		if ((nuint)destination % (nuint)(uint)Alignment != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		*(Vector<T>*)destination = source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal<T>(this Vector<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static void StoreUnsafe<T>(this Vector<T> source, ref T destination)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static void StoreUnsafe<T>(this Vector<T> source, ref T destination, nuint elementOffset)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination), source);
	}

	[Intrinsic]
	public static Vector<T> Subtract<T>(Vector<T> left, Vector<T> right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> SubtractSaturate<T>(Vector<T> left, Vector<T> right)
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return left - right;
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.SubtractSaturate(GetElementUnsafe(in left, i), GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static T Sum<T>(Vector<T> value)
	{
		T val = default(T);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			val = Scalar<T>.Add(val, GetElementUnsafe(in value, i));
		}
		return val;
	}

	[Intrinsic]
	public static T ToScalar<T>(this Vector<T> vector)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		return GetElementUnsafe(in vector, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static Vector<T> Truncate<T>(Vector<T> vector)
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong))
		{
			return vector;
		}
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			T value2 = Scalar<T>.Truncate(GetElementUnsafe(in vector, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<double> Truncate(Vector<double> vector)
	{
		return Vector.Truncate<double>(vector);
	}

	[Intrinsic]
	public static Vector<float> Truncate(Vector<float> vector)
	{
		return Vector.Truncate<float>(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void Widen(Vector<byte> source, out Vector<ushort> low, out Vector<ushort> high)
	{
		low = WidenLower(source);
		high = WidenUpper(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Widen(Vector<short> source, out Vector<int> low, out Vector<int> high)
	{
		low = WidenLower(source);
		high = WidenUpper(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Widen(Vector<int> source, out Vector<long> low, out Vector<long> high)
	{
		low = WidenLower(source);
		high = WidenUpper(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void Widen(Vector<sbyte> source, out Vector<short> low, out Vector<short> high)
	{
		low = WidenLower(source);
		high = WidenUpper(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Widen(Vector<float> source, out Vector<double> low, out Vector<double> high)
	{
		low = WidenLower(source);
		high = WidenUpper(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void Widen(Vector<ushort> source, out Vector<uint> low, out Vector<uint> high)
	{
		low = WidenLower(source);
		high = WidenUpper(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void Widen(Vector<uint> source, out Vector<ulong> low, out Vector<ulong> high)
	{
		low = WidenLower(source);
		high = WidenUpper(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ushort> WidenLower(Vector<byte> source)
	{
		Unsafe.SkipInit<Vector<ushort>>(out var value);
		for (int i = 0; i < Vector<ushort>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<int> WidenLower(Vector<short> source)
	{
		Unsafe.SkipInit<Vector<int>>(out var value);
		for (int i = 0; i < Vector<int>.Count; i++)
		{
			int elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<long> WidenLower(Vector<int> source)
	{
		Unsafe.SkipInit<Vector<long>>(out var value);
		for (int i = 0; i < Vector<long>.Count; i++)
		{
			long value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<short> WidenLower(Vector<sbyte> source)
	{
		Unsafe.SkipInit<Vector<short>>(out var value);
		for (int i = 0; i < Vector<short>.Count; i++)
		{
			short elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> WidenLower(Vector<float> source)
	{
		Unsafe.SkipInit<Vector<double>>(out var value);
		for (int i = 0; i < Vector<double>.Count; i++)
		{
			double value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> WidenLower(Vector<ushort> source)
	{
		Unsafe.SkipInit<Vector<uint>>(out var value);
		for (int i = 0; i < Vector<uint>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ulong> WidenLower(Vector<uint> source)
	{
		Unsafe.SkipInit<Vector<ulong>>(out var value);
		for (int i = 0; i < Vector<ulong>.Count; i++)
		{
			ulong value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ushort> WidenUpper(Vector<byte> source)
	{
		Unsafe.SkipInit<Vector<ushort>>(out var value);
		for (int i = Vector<ushort>.Count; i < Vector<byte>.Count; i++)
		{
			ushort elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector<ushort>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<int> WidenUpper(Vector<short> source)
	{
		Unsafe.SkipInit<Vector<int>>(out var value);
		for (int i = Vector<int>.Count; i < Vector<short>.Count; i++)
		{
			int elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector<int>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<long> WidenUpper(Vector<int> source)
	{
		Unsafe.SkipInit<Vector<long>>(out var value);
		for (int i = Vector<long>.Count; i < Vector<int>.Count; i++)
		{
			long value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector<long>.Count, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<short> WidenUpper(Vector<sbyte> source)
	{
		Unsafe.SkipInit<Vector<short>>(out var value);
		for (int i = Vector<short>.Count; i < Vector<sbyte>.Count; i++)
		{
			short elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector<short>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<double> WidenUpper(Vector<float> source)
	{
		Unsafe.SkipInit<Vector<double>>(out var value);
		for (int i = Vector<double>.Count; i < Vector<float>.Count; i++)
		{
			double value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector<double>.Count, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<uint> WidenUpper(Vector<ushort> source)
	{
		Unsafe.SkipInit<Vector<uint>>(out var value);
		for (int i = Vector<uint>.Count; i < Vector<ushort>.Count; i++)
		{
			uint elementUnsafe = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector<uint>.Count, elementUnsafe);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector<ulong> WidenUpper(Vector<uint> source)
	{
		Unsafe.SkipInit<Vector<ulong>>(out var value);
		for (int i = Vector<ulong>.Count; i < Vector<uint>.Count; i++)
		{
			ulong value2 = GetElementUnsafe(in source, i);
			value.SetElementUnsafe(i - Vector<ulong>.Count, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector<T> WithElement<T>(this Vector<T> vector, int index, T value)
	{
		if ((uint)index >= (uint)Vector<T>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		Vector<T> vector2 = vector;
		SetElementUnsafe(in vector2, index, value);
		return vector2;
	}

	[Intrinsic]
	public static Vector<T> Xor<T>(Vector<T> left, Vector<T> right)
	{
		return left ^ right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static T GetElementUnsafe<T>(this in Vector<T> vector, int index)
	{
		return Unsafe.Add(ref Unsafe.As<Vector<T>, T>(ref Unsafe.AsRef(in vector)), index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetElementUnsafe<T>(this in Vector<T> vector, int index, T value)
	{
		Unsafe.Add(ref Unsafe.As<Vector<T>, T>(ref Unsafe.AsRef(in vector)), index) = value;
	}

	public static Vector3 AsVector3(this Vector2 value)
	{
		return value.AsVector128().AsVector3();
	}

	public static Vector3 AsVector3Unsafe(this Vector2 value)
	{
		return value.AsVector128Unsafe().AsVector3();
	}

	public static Vector4 AsVector4(this Vector2 value)
	{
		return value.AsVector128().AsVector4();
	}

	public static Vector4 AsVector4Unsafe(this Vector2 value)
	{
		return value.AsVector128Unsafe().AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint ExtractMostSignificantBits(this Vector2 vector)
	{
		return vector.AsVector128().ExtractMostSignificantBits();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetElement(this Vector2 vector, int index)
	{
		if ((uint)index >= 2u)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return vector.AsVector128Unsafe().GetElement(index);
	}

	[CLSCompliant(false)]
	public unsafe static void Store(this Vector2 source, float* destination)
	{
		source.StoreUnsafe(ref *destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned(this Vector2 source, float* destination)
	{
		if ((nuint)destination % (nuint)8u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		*(Vector2*)destination = source;
	}

	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal(this Vector2 source, float* destination)
	{
		source.StoreAligned(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void StoreUnsafe(this Vector2 source, ref float destination)
	{
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void StoreUnsafe(this Vector2 source, ref float destination, nuint elementOffset)
	{
		destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToScalar(this Vector2 vector)
	{
		return vector.AsVector128Unsafe().ToScalar();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 WithElement(this Vector2 vector, int index, float value)
	{
		if ((uint)index >= 2u)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return vector.AsVector128Unsafe().WithElement(index, value).AsVector2();
	}

	public static Vector2 AsVector2(this Vector3 value)
	{
		return value.AsVector128().AsVector2();
	}

	public static Vector4 AsVector4(this Vector3 value)
	{
		return value.AsVector128().AsVector4();
	}

	public static Vector4 AsVector4Unsafe(this Vector3 value)
	{
		return value.AsVector128Unsafe().AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint ExtractMostSignificantBits(this Vector3 vector)
	{
		return vector.AsVector128().ExtractMostSignificantBits();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetElement(this Vector3 vector, int index)
	{
		if ((uint)index >= 3u)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return vector.AsVector128Unsafe().GetElement(index);
	}

	[CLSCompliant(false)]
	public unsafe static void Store(this Vector3 source, float* destination)
	{
		source.StoreUnsafe(ref *destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned(this Vector3 source, float* destination)
	{
		if ((nuint)destination % (nuint)8u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		*(Vector3*)destination = source;
	}

	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal(this Vector3 source, float* destination)
	{
		source.StoreAligned(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void StoreUnsafe(this Vector3 source, ref float destination)
	{
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void StoreUnsafe(this Vector3 source, ref float destination, nuint elementOffset)
	{
		destination = ref Unsafe.Add(ref destination, (nint)elementOffset);
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref destination), source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToScalar(this Vector3 vector)
	{
		return vector.AsVector128Unsafe().ToScalar();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 WithElement(this Vector3 vector, int index, float value)
	{
		if ((uint)index >= 3u)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return vector.AsVector128Unsafe().WithElement(index, value).AsVector3();
	}

	public static Plane AsPlane(this Vector4 value)
	{
		return Unsafe.BitCast<Vector4, Plane>(value);
	}

	public static Quaternion AsQuaternion(this Vector4 value)
	{
		return Unsafe.BitCast<Vector4, Quaternion>(value);
	}

	public static Vector2 AsVector2(this Vector4 value)
	{
		return value.AsVector128().AsVector2();
	}

	public static Vector3 AsVector3(this Vector4 value)
	{
		return value.AsVector128().AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static uint ExtractMostSignificantBits(this Vector4 vector)
	{
		return vector.AsVector128().ExtractMostSignificantBits();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetElement(this Vector4 vector, int index)
	{
		return vector.AsVector128().GetElement(index);
	}

	[CLSCompliant(false)]
	public unsafe static void Store(this Vector4 source, float* destination)
	{
		source.AsVector128().Store(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public unsafe static void StoreAligned(this Vector4 source, float* destination)
	{
		source.AsVector128().StoreAligned(destination);
	}

	[CLSCompliant(false)]
	public unsafe static void StoreAlignedNonTemporal(this Vector4 source, float* destination)
	{
		source.AsVector128().StoreAlignedNonTemporal(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void StoreUnsafe(this Vector4 source, ref float destination)
	{
		source.AsVector128().StoreUnsafe(ref destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public static void StoreUnsafe(this Vector4 source, ref float destination, nuint elementOffset)
	{
		source.AsVector128().StoreUnsafe(ref destination, elementOffset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToScalar(this Vector4 vector)
	{
		return vector.AsVector128().ToScalar();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 WithElement(this Vector4 vector, int index, float value)
	{
		return vector.AsVector128().WithElement(index, value).AsVector4();
	}
}
[Intrinsic]
[DebuggerDisplay("{DisplayString,nq}")]
[DebuggerTypeProxy(typeof(VectorDebugView<>))]
public readonly struct Vector<T> : ISimdVector<Vector<T>, T>, IAdditionOperators<Vector<T>, Vector<T>, Vector<T>>, IBitwiseOperators<Vector<T>, Vector<T>, Vector<T>>, IDivisionOperators<Vector<T>, Vector<T>, Vector<T>>, IEqualityOperators<Vector<T>, Vector<T>, bool>, IEquatable<Vector<T>>, IMultiplyOperators<Vector<T>, Vector<T>, Vector<T>>, IShiftOperators<Vector<T>, int, Vector<T>>, ISubtractionOperators<Vector<T>, Vector<T>, Vector<T>>, IUnaryNegationOperators<Vector<T>, Vector<T>>, IUnaryPlusOperators<Vector<T>, Vector<T>>, IFormattable
{
	internal readonly ulong _00;

	internal readonly ulong _01;

	public static Vector<T> AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector.Create(Scalar<T>.AllBitsSet);
		}
	}

	public unsafe static int Count
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
			return sizeof(Vector<T>) / Unsafe.SizeOf<T>();
		}
	}

	public static Vector<T> Indices
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
			Unsafe.SkipInit<Vector<T>>(out var value);
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
			if (!(typeof(T) == typeof(byte)) && !(typeof(T) == typeof(double)) && !(typeof(T) == typeof(short)) && !(typeof(T) == typeof(int)) && !(typeof(T) == typeof(long)) && !(typeof(T) == typeof(nint)) && !(typeof(T) == typeof(nuint)) && !(typeof(T) == typeof(sbyte)) && !(typeof(T) == typeof(float)) && !(typeof(T) == typeof(ushort)) && !(typeof(T) == typeof(uint)))
			{
				return typeof(T) == typeof(ulong);
			}
			return true;
		}
	}

	public static Vector<T> One
	{
		[Intrinsic]
		get
		{
			return Vector.Create(Scalar<T>.One);
		}
	}

	public static Vector<T> Zero
	{
		[Intrinsic]
		get
		{
			ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
			return default(Vector<T>);
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

	public T this[int index]
	{
		[Intrinsic]
		get
		{
			return this.GetElement(index);
		}
	}

	static int ISimdVector<Vector<T>, T>.Alignment => Vector.Alignment;

	static int ISimdVector<Vector<T>, T>.ElementCount => Count;

	static bool ISimdVector<Vector<T>, T>.IsHardwareAccelerated
	{
		[Intrinsic]
		get
		{
			return Vector.IsHardwareAccelerated;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector(T value)
	{
		this = Vector.Create(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector(T[] values)
	{
		if (values.Length < Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		this = Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<T, byte>(ref values[0]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector(T[] values, int index)
	{
		if (index < 0 || values.Length - index < Count)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException();
		}
		this = Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<T, byte>(ref values[index]));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector(ReadOnlySpan<T> values)
	{
		if (values.Length < Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		this = Unsafe.ReadUnaligned<Vector<T>>(in Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector(ReadOnlySpan<byte> values)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		if (values.Length < Vector<byte>.Count)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		this = Unsafe.ReadUnaligned<Vector<T>>(in MemoryMarshal.GetReference(values));
	}

	[OverloadResolutionPriority(-1)]
	public Vector(Span<T> values)
		: this((ReadOnlySpan<T>)values)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator +(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Add(Vector.GetElementUnsafe(in left, i), Vector.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator &(Vector<T> left, Vector<T> right)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		Unsafe.SkipInit<Vector<ulong>>(out var value);
		Vector<ulong> vector = left.As<T, ulong>();
		Vector<ulong> vector2 = right.As<T, ulong>();
		for (int i = 0; i < Vector<ulong>.Count; i++)
		{
			ulong value2 = Vector.GetElementUnsafe(in vector, i) & Vector.GetElementUnsafe(in vector2, i);
			value.SetElementUnsafe(i, value2);
		}
		return value.As<ulong, T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator |(Vector<T> left, Vector<T> right)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		Unsafe.SkipInit<Vector<ulong>>(out var value);
		Vector<ulong> vector = left.As<T, ulong>();
		Vector<ulong> vector2 = right.As<T, ulong>();
		for (int i = 0; i < Vector<ulong>.Count; i++)
		{
			ulong value2 = Vector.GetElementUnsafe(in vector, i) | Vector.GetElementUnsafe(in vector2, i);
			value.SetElementUnsafe(i, value2);
		}
		return value.As<ulong, T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator /(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Divide(Vector.GetElementUnsafe(in left, i), Vector.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator /(Vector<T> left, T right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Divide(Vector.GetElementUnsafe(in left, i), right);
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector<T> left, Vector<T> right)
	{
		for (int i = 0; i < Count; i++)
		{
			if (!Scalar<T>.Equals(Vector.GetElementUnsafe(in left, i), Vector.GetElementUnsafe(in right, i)))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator ^(Vector<T> left, Vector<T> right)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		Unsafe.SkipInit<Vector<ulong>>(out var value);
		Vector<ulong> vector = left.As<T, ulong>();
		Vector<ulong> vector2 = right.As<T, ulong>();
		for (int i = 0; i < Vector<ulong>.Count; i++)
		{
			ulong value2 = Vector.GetElementUnsafe(in vector, i) ^ Vector.GetElementUnsafe(in vector2, i);
			value.SetElementUnsafe(i, value2);
		}
		return value.As<ulong, T>();
	}

	[Intrinsic]
	public static explicit operator Vector<byte>(Vector<T> value)
	{
		return value.As<T, byte>();
	}

	[Intrinsic]
	public static explicit operator Vector<double>(Vector<T> value)
	{
		return value.As<T, double>();
	}

	[Intrinsic]
	public static explicit operator Vector<short>(Vector<T> value)
	{
		return value.As<T, short>();
	}

	[Intrinsic]
	public static explicit operator Vector<int>(Vector<T> value)
	{
		return value.As<T, int>();
	}

	[Intrinsic]
	public static explicit operator Vector<long>(Vector<T> value)
	{
		return value.As<T, long>();
	}

	[Intrinsic]
	public static explicit operator Vector<nint>(Vector<T> value)
	{
		return value.As<T, nint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static explicit operator Vector<nuint>(Vector<T> value)
	{
		return value.As<T, nuint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static explicit operator Vector<sbyte>(Vector<T> value)
	{
		return value.As<T, sbyte>();
	}

	[Intrinsic]
	public static explicit operator Vector<float>(Vector<T> value)
	{
		return value.As<T, float>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static explicit operator Vector<ushort>(Vector<T> value)
	{
		return value.As<T, ushort>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static explicit operator Vector<uint>(Vector<T> value)
	{
		return value.As<T, uint>();
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public static explicit operator Vector<ulong>(Vector<T> value)
	{
		return value.As<T, ulong>();
	}

	[Intrinsic]
	public static bool operator !=(Vector<T> left, Vector<T> right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator <<(Vector<T> value, int shiftCount)
	{
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Count; i++)
		{
			T value3 = Scalar<T>.ShiftLeft(Vector.GetElementUnsafe(in value, i), shiftCount);
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator *(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Multiply(Vector.GetElementUnsafe(in left, i), Vector.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[Intrinsic]
	public static Vector<T> operator *(Vector<T> value, T factor)
	{
		return value * Vector.Create(factor);
	}

	[Intrinsic]
	public static Vector<T> operator *(T factor, Vector<T> value)
	{
		return value * factor;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator ~(Vector<T> value)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		Unsafe.SkipInit<Vector<ulong>>(out var value2);
		Vector<ulong> vector = value.As<T, ulong>();
		for (int i = 0; i < Vector<ulong>.Count; i++)
		{
			ulong value3 = ~Vector.GetElementUnsafe(in vector, i);
			value2.SetElementUnsafe(i, value3);
		}
		return value2.As<ulong, T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator >>(Vector<T> value, int shiftCount)
	{
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Count; i++)
		{
			T value3 = Scalar<T>.ShiftRightArithmetic(Vector.GetElementUnsafe(in value, i), shiftCount);
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator -(Vector<T> left, Vector<T> right)
	{
		Unsafe.SkipInit<Vector<T>>(out var value);
		for (int i = 0; i < Count; i++)
		{
			T value2 = Scalar<T>.Subtract(Vector.GetElementUnsafe(in left, i), Vector.GetElementUnsafe(in right, i));
			value.SetElementUnsafe(i, value2);
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator -(Vector<T> value)
	{
		if (typeof(T) == typeof(float))
		{
			return value ^ Vector.Create(-0f).As<float, T>();
		}
		if (typeof(T) == typeof(double))
		{
			return value ^ Vector.Create(-0.0).As<double, T>();
		}
		return Zero - value;
	}

	[Intrinsic]
	public static Vector<T> operator +(Vector<T> value)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector<T> operator >>>(Vector<T> value, int shiftCount)
	{
		Unsafe.SkipInit<Vector<T>>(out var value2);
		for (int i = 0; i < Count; i++)
		{
			T value3 = Scalar<T>.ShiftRightLogical(Vector.GetElementUnsafe(in value, i), shiftCount);
			value2.SetElementUnsafe(i, value3);
		}
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CopyTo(T[] destination)
	{
		if (destination.Length < Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[0]), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CopyTo(T[] destination, int startIndex)
	{
		if ((uint)startIndex >= (uint)destination.Length)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
		}
		if (destination.Length - startIndex < Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref destination[startIndex]), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CopyTo(Span<byte> destination)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		if (destination.Length < Vector<byte>.Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CopyTo(Span<T> destination)
	{
		if (destination.Length < Count)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), this);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector<T> other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector<T> other)
	{
		if (Vector.IsHardwareAccelerated)
		{
			if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
			{
				return (Vector.Equals(this, other) | ~(Vector.Equals(this, this) | Vector.Equals(other, other))).As<T, int>() == Vector<int>.AllBitsSet;
			}
			return this == other;
		}
		return SoftwareFallback(this, other);
		static bool SoftwareFallback(in Vector<T> self, Vector<T> vector)
		{
			for (int i = 0; i < Count; i++)
			{
				if (!Scalar<T>.ObjectEquals(Vector.GetElementUnsafe(in self, i), Vector.GetElementUnsafe(in vector, i)))
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
		return ToString("G", CultureInfo.CurrentCulture);
	}

	public string ToString([StringSyntax("NumericFormat")] string? format)
	{
		return ToString(format, CultureInfo.CurrentCulture);
	}

	public string ToString([StringSyntax("NumericFormat")] string? format, IFormatProvider? formatProvider)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryCopyTo(Span<byte> destination)
	{
		ThrowHelper.ThrowForUnsupportedNumericsVectorBaseType<T>();
		if (destination.Length < Vector<byte>.Count)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), this);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryCopyTo(Span<T> destination)
	{
		if (destination.Length < Count)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destination)), this);
		return true;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Abs(Vector<T> vector)
	{
		return Vector.Abs(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Add(Vector<T> left, Vector<T> right)
	{
		return left + right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.All(Vector<T> vector, T value)
	{
		return Vector.All(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.AllWhereAllBitsSet(Vector<T> vector)
	{
		return Vector.AllWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.AndNot(Vector<T> left, Vector<T> right)
	{
		return Vector.AndNot(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.Any(Vector<T> vector, T value)
	{
		return Vector.Any(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.AnyWhereAllBitsSet(Vector<T> vector)
	{
		return Vector.AnyWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.BitwiseAnd(Vector<T> left, Vector<T> right)
	{
		return left & right;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.BitwiseOr(Vector<T> left, Vector<T> right)
	{
		return left | right;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Ceiling(Vector<T> vector)
	{
		return Vector.Ceiling(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Clamp(Vector<T> value, Vector<T> min, Vector<T> max)
	{
		return Vector.Clamp(value, min, max);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.ClampNative(Vector<T> value, Vector<T> min, Vector<T> max)
	{
		return Vector.ClampNative(value, min, max);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.ConditionalSelect(Vector<T> condition, Vector<T> left, Vector<T> right)
	{
		return Vector.ConditionalSelect(condition, left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.CopySign(Vector<T> value, Vector<T> sign)
	{
		return Vector.CopySign(value, sign);
	}

	static void ISimdVector<Vector<T>, T>.CopyTo(Vector<T> vector, T[] destination)
	{
		vector.CopyTo(destination);
	}

	static void ISimdVector<Vector<T>, T>.CopyTo(Vector<T> vector, T[] destination, int startIndex)
	{
		vector.CopyTo(destination, startIndex);
	}

	static void ISimdVector<Vector<T>, T>.CopyTo(Vector<T> vector, Span<T> destination)
	{
		vector.CopyTo(destination);
	}

	[Intrinsic]
	static int ISimdVector<Vector<T>, T>.Count(Vector<T> vector, T value)
	{
		return Vector.Count(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector<T>, T>.CountWhereAllBitsSet(Vector<T> vector)
	{
		return Vector.CountWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Create(T value)
	{
		return Vector.Create(value);
	}

	static Vector<T> ISimdVector<Vector<T>, T>.Create(T[] values)
	{
		return new Vector<T>(values);
	}

	static Vector<T> ISimdVector<Vector<T>, T>.Create(T[] values, int index)
	{
		return new Vector<T>(values, index);
	}

	static Vector<T> ISimdVector<Vector<T>, T>.Create(ReadOnlySpan<T> values)
	{
		return Vector.Create(values);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.CreateScalar(T value)
	{
		return Vector.CreateScalar(value);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.CreateScalarUnsafe(T value)
	{
		return Vector.CreateScalarUnsafe(value);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Divide(Vector<T> left, Vector<T> right)
	{
		return left / right;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Divide(Vector<T> left, T right)
	{
		return left / right;
	}

	[Intrinsic]
	static T ISimdVector<Vector<T>, T>.Dot(Vector<T> left, Vector<T> right)
	{
		return Vector.Dot(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Equals(Vector<T> left, Vector<T> right)
	{
		return Vector.Equals(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.EqualsAll(Vector<T> left, Vector<T> right)
	{
		return left == right;
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.EqualsAny(Vector<T> left, Vector<T> right)
	{
		return Vector.EqualsAny(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Floor(Vector<T> vector)
	{
		return Vector.Floor(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector<T>, T>.GetElement(Vector<T> vector, int index)
	{
		return vector.GetElement(index);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.GreaterThan(Vector<T> left, Vector<T> right)
	{
		return Vector.GreaterThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.GreaterThanAll(Vector<T> left, Vector<T> right)
	{
		return Vector.GreaterThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.GreaterThanAny(Vector<T> left, Vector<T> right)
	{
		return Vector.GreaterThanAny(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.GreaterThanOrEqual(Vector<T> left, Vector<T> right)
	{
		return Vector.GreaterThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.GreaterThanOrEqualAll(Vector<T> left, Vector<T> right)
	{
		return Vector.GreaterThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.GreaterThanOrEqualAny(Vector<T> left, Vector<T> right)
	{
		return Vector.GreaterThanOrEqualAny(left, right);
	}

	[Intrinsic]
	static int ISimdVector<Vector<T>, T>.IndexOf(Vector<T> vector, T value)
	{
		return Vector.IndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector<T>, T>.IndexOfWhereAllBitsSet(Vector<T> vector)
	{
		return Vector.IndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsEvenInteger(Vector<T> vector)
	{
		return Vector.IsEvenInteger(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsFinite(Vector<T> vector)
	{
		return Vector.IsFinite(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsInfinity(Vector<T> vector)
	{
		return Vector.IsInfinity(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsInteger(Vector<T> vector)
	{
		return Vector.IsInteger(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsNaN(Vector<T> vector)
	{
		return Vector.IsNaN(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsNegative(Vector<T> vector)
	{
		return Vector.IsNegative(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsNegativeInfinity(Vector<T> vector)
	{
		return Vector.IsNegativeInfinity(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsNormal(Vector<T> vector)
	{
		return Vector.IsNormal(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsOddInteger(Vector<T> vector)
	{
		return Vector.IsOddInteger(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsPositive(Vector<T> vector)
	{
		return Vector.IsPositive(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsPositiveInfinity(Vector<T> vector)
	{
		return Vector.IsPositiveInfinity(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.IsSubnormal(Vector<T> vector)
	{
		return Vector.IsSubnormal(vector);
	}

	static Vector<T> ISimdVector<Vector<T>, T>.IsZero(Vector<T> vector)
	{
		return Vector.IsZero(vector);
	}

	[Intrinsic]
	static int ISimdVector<Vector<T>, T>.LastIndexOf(Vector<T> vector, T value)
	{
		return Vector.LastIndexOf(vector, value);
	}

	[Intrinsic]
	static int ISimdVector<Vector<T>, T>.LastIndexOfWhereAllBitsSet(Vector<T> vector)
	{
		return Vector.LastIndexOfWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.LessThan(Vector<T> left, Vector<T> right)
	{
		return Vector.LessThan(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.LessThanAll(Vector<T> left, Vector<T> right)
	{
		return Vector.LessThanAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.LessThanAny(Vector<T> left, Vector<T> right)
	{
		return Vector.LessThanAny(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.LessThanOrEqual(Vector<T> left, Vector<T> right)
	{
		return Vector.LessThanOrEqual(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.LessThanOrEqualAll(Vector<T> left, Vector<T> right)
	{
		return Vector.LessThanOrEqualAll(left, right);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.LessThanOrEqualAny(Vector<T> left, Vector<T> right)
	{
		return Vector.LessThanOrEqualAny(left, right);
	}

	[Intrinsic]
	unsafe static Vector<T> ISimdVector<Vector<T>, T>.Load(T* source)
	{
		return Vector.Load(source);
	}

	[Intrinsic]
	unsafe static Vector<T> ISimdVector<Vector<T>, T>.LoadAligned(T* source)
	{
		return Vector.LoadAligned(source);
	}

	[Intrinsic]
	unsafe static Vector<T> ISimdVector<Vector<T>, T>.LoadAlignedNonTemporal(T* source)
	{
		return Vector.LoadAlignedNonTemporal(source);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.LoadUnsafe(ref readonly T source)
	{
		return Vector.LoadUnsafe(in source);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.LoadUnsafe(ref readonly T source, nuint elementOffset)
	{
		return Vector.LoadUnsafe(in source, elementOffset);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Max(Vector<T> left, Vector<T> right)
	{
		return Vector.Max(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MaxMagnitude(Vector<T> left, Vector<T> right)
	{
		return Vector.MaxMagnitude(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MaxMagnitudeNumber(Vector<T> left, Vector<T> right)
	{
		return Vector.MaxMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MaxNative(Vector<T> left, Vector<T> right)
	{
		return Vector.MaxNative(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MaxNumber(Vector<T> left, Vector<T> right)
	{
		return Vector.MaxNumber(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Min(Vector<T> left, Vector<T> right)
	{
		return Vector.Min(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MinMagnitude(Vector<T> left, Vector<T> right)
	{
		return Vector.MinMagnitude(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MinMagnitudeNumber(Vector<T> left, Vector<T> right)
	{
		return Vector.MinMagnitudeNumber(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MinNative(Vector<T> left, Vector<T> right)
	{
		return Vector.MinNative(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MinNumber(Vector<T> left, Vector<T> right)
	{
		return Vector.MinNumber(left, right);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Multiply(Vector<T> left, Vector<T> right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Multiply(Vector<T> left, T right)
	{
		return left * right;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.MultiplyAddEstimate(Vector<T> left, Vector<T> right, Vector<T> addend)
	{
		return Vector.MultiplyAddEstimate(left, right, addend);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Negate(Vector<T> vector)
	{
		return -vector;
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.None(Vector<T> vector, T value)
	{
		return Vector.None(vector, value);
	}

	[Intrinsic]
	static bool ISimdVector<Vector<T>, T>.NoneWhereAllBitsSet(Vector<T> vector)
	{
		return Vector.NoneWhereAllBitsSet(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.OnesComplement(Vector<T> vector)
	{
		return ~vector;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Round(Vector<T> vector)
	{
		return Vector.Round(vector);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.ShiftLeft(Vector<T> vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.ShiftRightArithmetic(Vector<T> vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.ShiftRightLogical(Vector<T> vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Sqrt(Vector<T> vector)
	{
		return Vector.SquareRoot(vector);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector<T>, T>.Store(Vector<T> source, T* destination)
	{
		source.Store(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector<T>, T>.StoreAligned(Vector<T> source, T* destination)
	{
		source.StoreAligned(destination);
	}

	[Intrinsic]
	unsafe static void ISimdVector<Vector<T>, T>.StoreAlignedNonTemporal(Vector<T> source, T* destination)
	{
		source.StoreAlignedNonTemporal(destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector<T>, T>.StoreUnsafe(Vector<T> vector, ref T destination)
	{
		vector.StoreUnsafe(ref destination);
	}

	[Intrinsic]
	static void ISimdVector<Vector<T>, T>.StoreUnsafe(Vector<T> vector, ref T destination, nuint elementOffset)
	{
		vector.StoreUnsafe(ref destination, elementOffset);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Subtract(Vector<T> left, Vector<T> right)
	{
		return left - right;
	}

	[Intrinsic]
	static T ISimdVector<Vector<T>, T>.Sum(Vector<T> vector)
	{
		return Vector.Sum(vector);
	}

	[Intrinsic]
	static T ISimdVector<Vector<T>, T>.ToScalar(Vector<T> vector)
	{
		return vector.ToScalar();
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Truncate(Vector<T> vector)
	{
		return Vector.Truncate(vector);
	}

	static bool ISimdVector<Vector<T>, T>.TryCopyTo(Vector<T> vector, Span<T> destination)
	{
		return vector.TryCopyTo(destination);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.WithElement(Vector<T> vector, int index, T value)
	{
		return vector.WithElement(index, value);
	}

	[Intrinsic]
	static Vector<T> ISimdVector<Vector<T>, T>.Xor(Vector<T> left, Vector<T> right)
	{
		return left ^ right;
	}
}
