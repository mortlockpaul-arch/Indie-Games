using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace System.Numerics;

[Intrinsic]
public struct Vector2 : IEquatable<Vector2>, IFormattable
{
	public float X;

	public float Y;

	public static Vector2 AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector128<float>.AllBitsSet.AsVector2();
		}
	}

	public static Vector2 E
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.E);
		}
	}

	public static Vector2 Epsilon
	{
		[Intrinsic]
		get
		{
			return Create(float.Epsilon);
		}
	}

	public static Vector2 NaN
	{
		[Intrinsic]
		get
		{
			return Create(float.NaN);
		}
	}

	public static Vector2 NegativeInfinity
	{
		[Intrinsic]
		get
		{
			return Create(float.NegativeInfinity);
		}
	}

	public static Vector2 NegativeZero
	{
		[Intrinsic]
		get
		{
			return Create(-0f);
		}
	}

	public static Vector2 One
	{
		[Intrinsic]
		get
		{
			return Create(1f);
		}
	}

	public static Vector2 Pi
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.PI);
		}
	}

	public static Vector2 PositiveInfinity
	{
		[Intrinsic]
		get
		{
			return Create(float.PositiveInfinity);
		}
	}

	public static Vector2 Tau
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.PI * 2f);
		}
	}

	public static Vector2 UnitX
	{
		[Intrinsic]
		get
		{
			return CreateScalar(1f);
		}
	}

	public static Vector2 UnitY
	{
		[Intrinsic]
		get
		{
			return Create(0f, 1f);
		}
	}

	public static Vector2 Zero
	{
		[Intrinsic]
		get
		{
			return default(Vector2);
		}
	}

	public float this[int index]
	{
		[Intrinsic]
		readonly get
		{
			return this.GetElement(index);
		}
		[Intrinsic]
		set
		{
			this = this.WithElement(index, value);
		}
	}

	[Intrinsic]
	public Vector2(float value)
	{
		this = Create(value);
	}

	[Intrinsic]
	public Vector2(float x, float y)
	{
		this = Create(x, y);
	}

	[Intrinsic]
	public Vector2(ReadOnlySpan<float> values)
	{
		this = Create(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator +(Vector2 left, Vector2 right)
	{
		return (left.AsVector128Unsafe() + right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator /(Vector2 left, Vector2 right)
	{
		return (left.AsVector128Unsafe() / right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator /(Vector2 value1, float value2)
	{
		return (value1.AsVector128Unsafe() / value2).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector2 left, Vector2 right)
	{
		return left.AsVector128() == right.AsVector128();
	}

	[Intrinsic]
	public static bool operator !=(Vector2 left, Vector2 right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator *(Vector2 left, Vector2 right)
	{
		return (left.AsVector128Unsafe() * right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator *(Vector2 left, float right)
	{
		return (left.AsVector128Unsafe() * right).AsVector2();
	}

	[Intrinsic]
	public static Vector2 operator *(float left, Vector2 right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator -(Vector2 left, Vector2 right)
	{
		return (left.AsVector128Unsafe() - right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator -(Vector2 value)
	{
		return (-value.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator &(Vector2 left, Vector2 right)
	{
		return (left.AsVector128Unsafe() & right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator |(Vector2 left, Vector2 right)
	{
		return (left.AsVector128Unsafe() | right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator ^(Vector2 left, Vector2 right)
	{
		return (left.AsVector128Unsafe() ^ right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator <<(Vector2 value, int shiftAmount)
	{
		return (value.AsVector128Unsafe() << shiftAmount).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator ~(Vector2 value)
	{
		return (~value.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator >>(Vector2 value, int shiftAmount)
	{
		return (value.AsVector128Unsafe() >> shiftAmount).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator +(Vector2 value)
	{
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 operator >>>(Vector2 value, int shiftAmount)
	{
		return (value.AsVector128Unsafe() >>> shiftAmount).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Abs(Vector2 value)
	{
		return Vector128.Abs(value.AsVector128Unsafe()).AsVector2();
	}

	[Intrinsic]
	public static Vector2 Add(Vector2 left, Vector2 right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All(Vector2 vector, float value)
	{
		return Vector128.All(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet(Vector2 vector)
	{
		return Vector128.AllWhereAllBitsSet(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 AndNot(Vector2 left, Vector2 right)
	{
		return Vector128.AndNot(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any(Vector2 vector, float value)
	{
		return Vector128.Any(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet(Vector2 vector)
	{
		return Vector128.AnyWhereAllBitsSet(vector);
	}

	[Intrinsic]
	public static Vector2 BitwiseAnd(Vector2 left, Vector2 right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector2 BitwiseOr(Vector2 left, Vector2 right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
	{
		return Vector128.Clamp(value1.AsVector128Unsafe(), min.AsVector128Unsafe(), max.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 ClampNative(Vector2 value1, Vector2 min, Vector2 max)
	{
		return Vector128.ClampNative(value1.AsVector128Unsafe(), min.AsVector128Unsafe(), max.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 ConditionalSelect(Vector2 condition, Vector2 left, Vector2 right)
	{
		return Vector128.ConditionalSelect(condition.AsVector128Unsafe(), left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 CopySign(Vector2 value, Vector2 sign)
	{
		return Vector128.CopySign(value.AsVector128Unsafe(), sign.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Cos(Vector2 vector)
	{
		return Vector128.Cos(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count(Vector2 vector, float value)
	{
		return Vector128.Count(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet(Vector2 vector)
	{
		return Vector128.CountWhereAllBitsSet(vector);
	}

	[Intrinsic]
	public static Vector2 Create(float value)
	{
		return Vector128.Create(value).AsVector2();
	}

	[Intrinsic]
	public static Vector2 Create(float x, float y)
	{
		return Vector128.Create(x, y, 0f, 0f).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Create(ReadOnlySpan<float> values)
	{
		if (values.Length < 2)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return Unsafe.ReadUnaligned<Vector2>(in Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[Intrinsic]
	public static Vector2 CreateScalar(float x)
	{
		return Vector128.CreateScalar(x).AsVector2();
	}

	[Intrinsic]
	public static Vector2 CreateScalarUnsafe(float x)
	{
		return Vector128.CreateScalarUnsafe(x).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Cross(Vector2 value1, Vector2 value2)
	{
		Vector128<float> vector = Vector128.Shuffle(value1.AsVector128Unsafe(), Vector128.Create(0, 1, 0, 1)) * Vector128.Shuffle(value2.AsVector128Unsafe(), Vector128.Create(1, 0, 1, 0));
		return (vector - Vector128.Shuffle(vector, Vector128.Create(1, 0, 1, 0))).ToScalar();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 DegreesToRadians(Vector2 degrees)
	{
		return Vector128.DegreesToRadians(degrees.AsVector128Unsafe()).AsVector2();
	}

	[Intrinsic]
	public static float Distance(Vector2 value1, Vector2 value2)
	{
		return float.Sqrt(DistanceSquared(value1, value2));
	}

	[Intrinsic]
	public static float DistanceSquared(Vector2 value1, Vector2 value2)
	{
		return (value1 - value2).LengthSquared();
	}

	[Intrinsic]
	public static Vector2 Divide(Vector2 left, Vector2 right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector2 Divide(Vector2 left, float divisor)
	{
		return left / divisor;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Dot(Vector2 value1, Vector2 value2)
	{
		return Vector128.Dot(value1.AsVector128(), value2.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Exp(Vector2 vector)
	{
		return Vector128.Exp(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Equals(Vector2 left, Vector2 right)
	{
		return Vector128.Equals(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAll(Vector2 left, Vector2 right)
	{
		return AllWhereAllBitsSet(Equals(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny(Vector2 left, Vector2 right)
	{
		return AnyWhereAllBitsSet(Equals(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 FusedMultiplyAdd(Vector2 left, Vector2 right, Vector2 addend)
	{
		return Vector128.FusedMultiplyAdd(left.AsVector128Unsafe(), right.AsVector128Unsafe(), addend.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 GreaterThan(Vector2 left, Vector2 right)
	{
		return Vector128.GreaterThan(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll(Vector2 left, Vector2 right)
	{
		return AllWhereAllBitsSet(GreaterThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAny(Vector2 left, Vector2 right)
	{
		return AnyWhereAllBitsSet(GreaterThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 GreaterThanOrEqual(Vector2 left, Vector2 right)
	{
		return Vector128.GreaterThanOrEqual(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll(Vector2 left, Vector2 right)
	{
		return AllWhereAllBitsSet(GreaterThanOrEqual(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAny(Vector2 left, Vector2 right)
	{
		return AnyWhereAllBitsSet(GreaterThanOrEqual(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Hypot(Vector2 x, Vector2 y)
	{
		return Vector128.Hypot(x.AsVector128Unsafe(), y.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf(Vector2 vector, float value)
	{
		return Vector128.IndexOf(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOfWhereAllBitsSet(Vector2 vector)
	{
		return Vector128.IndexOfWhereAllBitsSet(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsEvenInteger(Vector2 vector)
	{
		return Vector128.IsEvenInteger(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsFinite(Vector2 vector)
	{
		return Vector128.IsFinite(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsInfinity(Vector2 vector)
	{
		return Vector128.IsInfinity(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsInteger(Vector2 vector)
	{
		return Vector128.IsInteger(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsNaN(Vector2 vector)
	{
		return Vector128.IsNaN(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsNegative(Vector2 vector)
	{
		return Vector128.IsNegative(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsNegativeInfinity(Vector2 vector)
	{
		return Vector128.IsNegativeInfinity(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsNormal(Vector2 vector)
	{
		return Vector128.IsNormal(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsOddInteger(Vector2 vector)
	{
		return Vector128.IsOddInteger(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsPositive(Vector2 vector)
	{
		return Vector128.IsPositive(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsPositiveInfinity(Vector2 vector)
	{
		return Vector128.IsPositiveInfinity(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsSubnormal(Vector2 vector)
	{
		return Vector128.IsSubnormal(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 IsZero(Vector2 vector)
	{
		return Vector128.IsZero(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf(Vector2 vector, float value)
	{
		return Vector128.LastIndexOf(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet(Vector2 vector)
	{
		return Vector128.LastIndexOfWhereAllBitsSet(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
	{
		return Lerp(value1, value2, Create(amount));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Lerp(Vector2 value1, Vector2 value2, Vector2 amount)
	{
		return Vector128.Lerp(value1.AsVector128Unsafe(), value2.AsVector128Unsafe(), amount.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 LessThan(Vector2 left, Vector2 right)
	{
		return Vector128.LessThan(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll(Vector2 left, Vector2 right)
	{
		return AllWhereAllBitsSet(LessThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAny(Vector2 left, Vector2 right)
	{
		return AnyWhereAllBitsSet(LessThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 LessThanOrEqual(Vector2 left, Vector2 right)
	{
		return Vector128.LessThanOrEqual(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll(Vector2 left, Vector2 right)
	{
		return AllWhereAllBitsSet(LessThanOrEqual(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAny(Vector2 left, Vector2 right)
	{
		return AnyWhereAllBitsSet(LessThanOrEqual(left, right));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector2 Load(float* source)
	{
		return LoadUnsafe(in *source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector2 LoadAligned(float* source)
	{
		if ((nuint)source % (nuint)8u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return *(Vector2*)source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector2 LoadAlignedNonTemporal(float* source)
	{
		return LoadAligned(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 LoadUnsafe(ref readonly float source)
	{
		return Unsafe.ReadUnaligned<Vector2>(in Unsafe.As<float, byte>(ref Unsafe.AsRef(in source)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector2 LoadUnsafe(ref readonly float source, nuint elementOffset)
	{
		return Unsafe.ReadUnaligned<Vector2>(in Unsafe.As<float, byte>(ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Log(Vector2 vector)
	{
		return Vector128.Log(Vector4.Create(vector, 1f, 1f).AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Log2(Vector2 vector)
	{
		return Vector128.Log2(Vector4.Create(vector, 1f, 1f).AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Max(Vector2 value1, Vector2 value2)
	{
		return Vector128.Max(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MaxMagnitude(Vector2 value1, Vector2 value2)
	{
		return Vector128.MaxMagnitude(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MaxMagnitudeNumber(Vector2 value1, Vector2 value2)
	{
		return Vector128.MaxMagnitudeNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MaxNative(Vector2 value1, Vector2 value2)
	{
		return Vector128.MaxNative(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MaxNumber(Vector2 value1, Vector2 value2)
	{
		return Vector128.MaxNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 Min(Vector2 value1, Vector2 value2)
	{
		return Vector128.Min(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MinMagnitude(Vector2 value1, Vector2 value2)
	{
		return Vector128.MinMagnitude(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MinMagnitudeNumber(Vector2 value1, Vector2 value2)
	{
		return Vector128.MinMagnitudeNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MinNative(Vector2 value1, Vector2 value2)
	{
		return Vector128.MinNative(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MinNumber(Vector2 value1, Vector2 value2)
	{
		return Vector128.MinNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector2();
	}

	[Intrinsic]
	public static Vector2 Multiply(Vector2 left, Vector2 right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector2 Multiply(Vector2 left, float right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector2 Multiply(float left, Vector2 right)
	{
		return left * right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 MultiplyAddEstimate(Vector2 left, Vector2 right, Vector2 addend)
	{
		return Vector128.MultiplyAddEstimate(left.AsVector128Unsafe(), right.AsVector128Unsafe(), addend.AsVector128Unsafe()).AsVector2();
	}

	[Intrinsic]
	public static Vector2 Negate(Vector2 value)
	{
		return -value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None(Vector2 vector, float value)
	{
		return Vector128.None(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet(Vector2 vector)
	{
		return Vector128.NoneWhereAllBitsSet(vector);
	}

	[Intrinsic]
	public static Vector2 Normalize(Vector2 value)
	{
		return value / value.Length();
	}

	[Intrinsic]
	public static Vector2 OnesComplement(Vector2 value)
	{
		return ~value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 RadiansToDegrees(Vector2 radians)
	{
		return Vector128.RadiansToDegrees(radians.AsVector128Unsafe()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Reflect(Vector2 vector, Vector2 normal)
	{
		Vector2 vector2 = Create(Dot(vector, normal));
		return MultiplyAddEstimate(-(vector2 + vector2), normal, vector);
	}

	[Intrinsic]
	public static Vector2 Round(Vector2 vector)
	{
		return Vector128.Round(vector.AsVector128Unsafe()).AsVector2();
	}

	[Intrinsic]
	public static Vector2 Round(Vector2 vector, MidpointRounding mode)
	{
		return Vector128.Round(vector.AsVector128Unsafe(), mode).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Shuffle(Vector2 vector, byte xIndex, byte yIndex)
	{
		return Vector128.Shuffle(vector.AsVector128(), Vector128.Create(xIndex, yIndex, 2, 3)).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Sin(Vector2 vector)
	{
		return Vector128.Sin(vector.AsVector128()).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector2 Sin, Vector2 Cos) SinCos(Vector2 vector)
	{
		var (value, value2) = Vector128.SinCos(vector.AsVector128());
		return (Sin: value.AsVector2(), Cos: value2.AsVector2());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector2 SquareRoot(Vector2 value)
	{
		return Vector128.Sqrt(value.AsVector128Unsafe()).AsVector2();
	}

	[Intrinsic]
	public static Vector2 Subtract(Vector2 left, Vector2 right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Sum(Vector2 value)
	{
		return Vector128.Sum(value.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Transform(Vector2 position, Matrix3x2 matrix)
	{
		Vector2 addend = matrix.X * position.X;
		addend = MultiplyAddEstimate(matrix.Y, Create(position.Y), addend);
		return addend + matrix.Z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Transform(Vector2 position, Matrix4x4 matrix)
	{
		return Vector4.Transform(position, matrix).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 Transform(Vector2 value, Quaternion rotation)
	{
		return Vector4.Transform(value, rotation).AsVector2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 TransformNormal(Vector2 normal, Matrix3x2 matrix)
	{
		Vector2 addend = matrix.X * normal.X;
		return MultiplyAddEstimate(matrix.Y, Create(normal.Y), addend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 TransformNormal(Vector2 normal, Matrix4x4 matrix)
	{
		Vector4 addend = matrix.X * normal.X;
		addend = Vector4.MultiplyAddEstimate(matrix.Y, Vector4.Create(normal.Y), addend);
		return addend.AsVector2();
	}

	[Intrinsic]
	public static Vector2 Truncate(Vector2 vector)
	{
		return Vector128.Truncate(vector.AsVector128Unsafe()).AsVector2();
	}

	[Intrinsic]
	public static Vector2 Xor(Vector2 left, Vector2 right)
	{
		return left ^ right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(float[] array)
	{
		if (array.Length < 2)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref array[0]), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(float[] array, int index)
	{
		if ((uint)index >= (uint)array.Length)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess();
		}
		if (array.Length - index < 2)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref array[index]), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(Span<float> destination)
	{
		if (destination.Length < 2)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(destination)), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool TryCopyTo(Span<float> destination)
	{
		if (destination.Length < 2)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(destination)), this);
		return true;
	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector2 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Vector2 other)
	{
		return this.AsVector128().Equals(other.AsVector128());
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	[Intrinsic]
	public readonly float Length()
	{
		return float.Sqrt(LengthSquared());
	}

	[Intrinsic]
	public readonly float LengthSquared()
	{
		return Dot(this, this);
	}

	public override readonly string ToString()
	{
		return ToString("G", CultureInfo.CurrentCulture);
	}

	public readonly string ToString([StringSyntax("NumericFormat")] string? format)
	{
		return ToString(format, CultureInfo.CurrentCulture);
	}

	public readonly string ToString([StringSyntax("NumericFormat")] string? format, IFormatProvider? formatProvider)
	{
		string numberGroupSeparator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
		return $"<{X.ToString(format, formatProvider)}{numberGroupSeparator} {Y.ToString(format, formatProvider)}>";
	}
}
