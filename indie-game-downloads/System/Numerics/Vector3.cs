using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace System.Numerics;

[Intrinsic]
public struct Vector3 : IEquatable<Vector3>, IFormattable
{
	public float X;

	public float Y;

	public float Z;

	public static Vector3 AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector128<float>.AllBitsSet.AsVector3();
		}
	}

	public static Vector3 E
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.E);
		}
	}

	public static Vector3 Epsilon
	{
		[Intrinsic]
		get
		{
			return Create(float.Epsilon);
		}
	}

	public static Vector3 NaN
	{
		[Intrinsic]
		get
		{
			return Create(float.NaN);
		}
	}

	public static Vector3 NegativeInfinity
	{
		[Intrinsic]
		get
		{
			return Create(float.NegativeInfinity);
		}
	}

	public static Vector3 NegativeZero
	{
		[Intrinsic]
		get
		{
			return Create(-0f);
		}
	}

	public static Vector3 One
	{
		[Intrinsic]
		get
		{
			return Create(1f);
		}
	}

	public static Vector3 Pi
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.PI);
		}
	}

	public static Vector3 PositiveInfinity
	{
		[Intrinsic]
		get
		{
			return Create(float.PositiveInfinity);
		}
	}

	public static Vector3 Tau
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.PI * 2f);
		}
	}

	public static Vector3 UnitX
	{
		[Intrinsic]
		get
		{
			return CreateScalar(1f);
		}
	}

	public static Vector3 UnitY
	{
		[Intrinsic]
		get
		{
			return Create(0f, 1f, 0f);
		}
	}

	public static Vector3 UnitZ
	{
		[Intrinsic]
		get
		{
			return Create(0f, 0f, 1f);
		}
	}

	public static Vector3 Zero
	{
		[Intrinsic]
		get
		{
			return default(Vector3);
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
	public Vector3(float value)
	{
		this = Create(value);
	}

	[Intrinsic]
	public Vector3(Vector2 value, float z)
	{
		this = Create(value, z);
	}

	[Intrinsic]
	public Vector3(float x, float y, float z)
	{
		this = Create(x, y, z);
	}

	[Intrinsic]
	public Vector3(ReadOnlySpan<float> values)
	{
		this = Create(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator +(Vector3 left, Vector3 right)
	{
		return (left.AsVector128Unsafe() + right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator /(Vector3 left, Vector3 right)
	{
		return (left.AsVector128Unsafe() / right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator /(Vector3 value1, float value2)
	{
		return (value1.AsVector128Unsafe() / value2).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector3 left, Vector3 right)
	{
		return left.AsVector128() == right.AsVector128();
	}

	[Intrinsic]
	public static bool operator !=(Vector3 left, Vector3 right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator *(Vector3 left, Vector3 right)
	{
		return (left.AsVector128Unsafe() * right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator *(Vector3 left, float right)
	{
		return (left.AsVector128Unsafe() * right).AsVector3();
	}

	[Intrinsic]
	public static Vector3 operator *(float left, Vector3 right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator -(Vector3 left, Vector3 right)
	{
		return (left.AsVector128Unsafe() - right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator -(Vector3 value)
	{
		return (-value.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator &(Vector3 left, Vector3 right)
	{
		return (left.AsVector128Unsafe() & right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator |(Vector3 left, Vector3 right)
	{
		return (left.AsVector128Unsafe() | right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator ^(Vector3 left, Vector3 right)
	{
		return (left.AsVector128Unsafe() ^ right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator <<(Vector3 value, int shiftAmount)
	{
		return (value.AsVector128Unsafe() << shiftAmount).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator ~(Vector3 value)
	{
		return (~value.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator >>(Vector3 value, int shiftAmount)
	{
		return (value.AsVector128Unsafe() >> shiftAmount).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator +(Vector3 value)
	{
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 operator >>>(Vector3 value, int shiftAmount)
	{
		return (value.AsVector128Unsafe() >>> shiftAmount).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Abs(Vector3 value)
	{
		return Vector128.Abs(value.AsVector128Unsafe()).AsVector3();
	}

	[Intrinsic]
	public static Vector3 Add(Vector3 left, Vector3 right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All(Vector3 vector, float value)
	{
		return Vector128.All(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet(Vector3 vector)
	{
		return Vector128.AllWhereAllBitsSet(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 AndNot(Vector3 left, Vector3 right)
	{
		return Vector128.AndNot(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any(Vector3 vector, float value)
	{
		return Vector128.Any(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet(Vector3 vector)
	{
		return Vector128.AnyWhereAllBitsSet(vector);
	}

	[Intrinsic]
	public static Vector3 BitwiseAnd(Vector3 left, Vector3 right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector3 BitwiseOr(Vector3 left, Vector3 right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
	{
		return Vector128.Clamp(value1.AsVector128Unsafe(), min.AsVector128Unsafe(), max.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 ClampNative(Vector3 value1, Vector3 min, Vector3 max)
	{
		return Vector128.ClampNative(value1.AsVector128Unsafe(), min.AsVector128Unsafe(), max.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 ConditionalSelect(Vector3 condition, Vector3 left, Vector3 right)
	{
		return Vector128.ConditionalSelect(condition.AsVector128Unsafe(), left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 CopySign(Vector3 value, Vector3 sign)
	{
		return Vector128.CopySign(value.AsVector128Unsafe(), sign.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Cos(Vector3 vector)
	{
		return Vector128.Cos(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count(Vector3 vector, float value)
	{
		return Vector128.Count(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet(Vector3 vector)
	{
		return Vector128.CountWhereAllBitsSet(vector);
	}

	[Intrinsic]
	public static Vector3 Create(float value)
	{
		return Vector128.Create(value).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Create(Vector2 vector, float z)
	{
		return vector.AsVector128Unsafe().WithElement(2, z).AsVector3();
	}

	[Intrinsic]
	public static Vector3 Create(float x, float y, float z)
	{
		return Vector128.Create(x, y, z, 0f).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Create(ReadOnlySpan<float> values)
	{
		if (values.Length < 3)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return Unsafe.ReadUnaligned<Vector3>(in Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(values)));
	}

	[Intrinsic]
	public static Vector3 CreateScalar(float x)
	{
		return Vector128.CreateScalar(x).AsVector3();
	}

	[Intrinsic]
	public static Vector3 CreateScalarUnsafe(float x)
	{
		return Vector128.CreateScalarUnsafe(x).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
	{
		Vector128<float> vector3 = vector1.AsVector128Unsafe();
		Vector128<float> vector4 = vector2.AsVector128Unsafe();
		Vector128<float> vector5 = Vector128.Shuffle(vector3, Vector128.Create(1, 2, 0, 0)) * Vector128.Shuffle(vector4, Vector128.Create(2, 0, 1, 0));
		Vector128<float> vector6 = Vector128.Shuffle(vector3, Vector128.Create(2, 0, 1, 0)) * Vector128.Shuffle(vector4, Vector128.Create(1, 2, 0, 0));
		return (vector5 - vector6).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 DegreesToRadians(Vector3 degrees)
	{
		return Vector128.DegreesToRadians(degrees.AsVector128Unsafe()).AsVector3();
	}

	[Intrinsic]
	public static float Distance(Vector3 value1, Vector3 value2)
	{
		return float.Sqrt(DistanceSquared(value1, value2));
	}

	[Intrinsic]
	public static float DistanceSquared(Vector3 value1, Vector3 value2)
	{
		return (value1 - value2).LengthSquared();
	}

	[Intrinsic]
	public static Vector3 Divide(Vector3 left, Vector3 right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector3 Divide(Vector3 left, float divisor)
	{
		return left / divisor;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Dot(Vector3 vector1, Vector3 vector2)
	{
		return Vector128.Dot(vector1.AsVector128(), vector2.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Exp(Vector3 vector)
	{
		return Vector128.Exp(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Equals(Vector3 left, Vector3 right)
	{
		return Vector128.Equals(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAll(Vector3 left, Vector3 right)
	{
		return AllWhereAllBitsSet(Equals(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny(Vector3 left, Vector3 right)
	{
		return AnyWhereAllBitsSet(Equals(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 FusedMultiplyAdd(Vector3 left, Vector3 right, Vector3 addend)
	{
		return Vector128.FusedMultiplyAdd(left.AsVector128Unsafe(), right.AsVector128Unsafe(), addend.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 GreaterThan(Vector3 left, Vector3 right)
	{
		return Vector128.GreaterThan(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll(Vector3 left, Vector3 right)
	{
		return AllWhereAllBitsSet(GreaterThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAny(Vector3 left, Vector3 right)
	{
		return AnyWhereAllBitsSet(GreaterThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 GreaterThanOrEqual(Vector3 left, Vector3 right)
	{
		return Vector128.GreaterThanOrEqual(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll(Vector3 left, Vector3 right)
	{
		return AllWhereAllBitsSet(GreaterThanOrEqual(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAny(Vector3 left, Vector3 right)
	{
		return AnyWhereAllBitsSet(GreaterThanOrEqual(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Hypot(Vector3 x, Vector3 y)
	{
		return Vector128.Hypot(x.AsVector128Unsafe(), y.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf(Vector3 vector, float value)
	{
		return Vector128.IndexOf(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOfWhereAllBitsSet(Vector3 vector)
	{
		return Vector128.IndexOfWhereAllBitsSet(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsEvenInteger(Vector3 vector)
	{
		return Vector128.IsEvenInteger(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsFinite(Vector3 vector)
	{
		return Vector128.IsFinite(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsInfinity(Vector3 vector)
	{
		return Vector128.IsInfinity(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsInteger(Vector3 vector)
	{
		return Vector128.IsInteger(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsNaN(Vector3 vector)
	{
		return Vector128.IsNaN(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsNegative(Vector3 vector)
	{
		return Vector128.IsNegative(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsNegativeInfinity(Vector3 vector)
	{
		return Vector128.IsNegativeInfinity(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsNormal(Vector3 vector)
	{
		return Vector128.IsNormal(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsOddInteger(Vector3 vector)
	{
		return Vector128.IsOddInteger(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsPositive(Vector3 vector)
	{
		return Vector128.IsPositive(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsPositiveInfinity(Vector3 vector)
	{
		return Vector128.IsPositiveInfinity(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsSubnormal(Vector3 vector)
	{
		return Vector128.IsSubnormal(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 IsZero(Vector3 vector)
	{
		return Vector128.IsZero(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf(Vector3 vector, float value)
	{
		return Vector128.LastIndexOf(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet(Vector3 vector)
	{
		return Vector128.LastIndexOfWhereAllBitsSet(vector);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
	{
		return Lerp(value1, value2, Create(amount));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Lerp(Vector3 value1, Vector3 value2, Vector3 amount)
	{
		return Vector128.Lerp(value1.AsVector128Unsafe(), value2.AsVector128Unsafe(), amount.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 LessThan(Vector3 left, Vector3 right)
	{
		return Vector128.LessThan(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll(Vector3 left, Vector3 right)
	{
		return AllWhereAllBitsSet(LessThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAny(Vector3 left, Vector3 right)
	{
		return AnyWhereAllBitsSet(LessThan(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 LessThanOrEqual(Vector3 left, Vector3 right)
	{
		return Vector128.LessThanOrEqual(left.AsVector128Unsafe(), right.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll(Vector3 left, Vector3 right)
	{
		return AllWhereAllBitsSet(LessThanOrEqual(left, right));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAny(Vector3 left, Vector3 right)
	{
		return AnyWhereAllBitsSet(LessThanOrEqual(left, right));
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector3 Load(float* source)
	{
		return LoadUnsafe(in *source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector3 LoadAligned(float* source)
	{
		if ((nuint)source % (nuint)8u != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return *(Vector3*)source;
	}

	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector3 LoadAlignedNonTemporal(float* source)
	{
		return LoadAligned(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 LoadUnsafe(ref readonly float source)
	{
		return Unsafe.ReadUnaligned<Vector3>(in Unsafe.As<float, byte>(ref Unsafe.AsRef(in source)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector3 LoadUnsafe(ref readonly float source, nuint elementOffset)
	{
		return Unsafe.ReadUnaligned<Vector3>(in Unsafe.As<float, byte>(ref Unsafe.Add(ref Unsafe.AsRef(in source), (nint)elementOffset)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Log(Vector3 vector)
	{
		return Vector128.Log(Vector4.Create(vector, 1f).AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Log2(Vector3 vector)
	{
		return Vector128.Log2(Vector4.Create(vector, 1f).AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Max(Vector3 value1, Vector3 value2)
	{
		return Vector128.Max(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MaxMagnitude(Vector3 value1, Vector3 value2)
	{
		return Vector128.MaxMagnitude(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MaxMagnitudeNumber(Vector3 value1, Vector3 value2)
	{
		return Vector128.MaxMagnitudeNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MaxNative(Vector3 value1, Vector3 value2)
	{
		return Vector128.MaxNative(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MaxNumber(Vector3 value1, Vector3 value2)
	{
		return Vector128.MaxNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 Min(Vector3 value1, Vector3 value2)
	{
		return Vector128.Min(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MinMagnitude(Vector3 value1, Vector3 value2)
	{
		return Vector128.MinMagnitude(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MinMagnitudeNumber(Vector3 value1, Vector3 value2)
	{
		return Vector128.MinMagnitudeNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MinNative(Vector3 value1, Vector3 value2)
	{
		return Vector128.MinNative(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MinNumber(Vector3 value1, Vector3 value2)
	{
		return Vector128.MinNumber(value1.AsVector128Unsafe(), value2.AsVector128Unsafe()).AsVector3();
	}

	[Intrinsic]
	public static Vector3 Multiply(Vector3 left, Vector3 right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector3 Multiply(Vector3 left, float right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector3 Multiply(float left, Vector3 right)
	{
		return left * right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 MultiplyAddEstimate(Vector3 left, Vector3 right, Vector3 addend)
	{
		return Vector128.MultiplyAddEstimate(left.AsVector128Unsafe(), right.AsVector128Unsafe(), addend.AsVector128Unsafe()).AsVector3();
	}

	[Intrinsic]
	public static Vector3 Negate(Vector3 value)
	{
		return -value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None(Vector3 vector, float value)
	{
		return Vector128.None(vector, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet(Vector3 vector)
	{
		return Vector128.NoneWhereAllBitsSet(vector);
	}

	[Intrinsic]
	public static Vector3 Normalize(Vector3 value)
	{
		return value / value.Length();
	}

	[Intrinsic]
	public static Vector3 OnesComplement(Vector3 value)
	{
		return ~value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 RadiansToDegrees(Vector3 radians)
	{
		return Vector128.RadiansToDegrees(radians.AsVector128Unsafe()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Reflect(Vector3 vector, Vector3 normal)
	{
		Vector3 vector2 = Create(Dot(vector, normal));
		return MultiplyAddEstimate(-(vector2 + vector2), normal, vector);
	}

	[Intrinsic]
	public static Vector3 Round(Vector3 vector)
	{
		return Vector128.Round(vector.AsVector128Unsafe()).AsVector3();
	}

	[Intrinsic]
	public static Vector3 Round(Vector3 vector, MidpointRounding mode)
	{
		return Vector128.Round(vector.AsVector128Unsafe(), mode).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Shuffle(Vector3 vector, byte xIndex, byte yIndex, byte zIndex)
	{
		return Vector128.Shuffle(vector.AsVector128(), Vector128.Create(xIndex, yIndex, zIndex, 3)).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Sin(Vector3 vector)
	{
		return Vector128.Sin(vector.AsVector128()).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector3 Sin, Vector3 Cos) SinCos(Vector3 vector)
	{
		var (value, value2) = Vector128.SinCos(vector.AsVector128());
		return (Sin: value.AsVector3(), Cos: value2.AsVector3());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector3 SquareRoot(Vector3 value)
	{
		return Vector128.Sqrt(value.AsVector128Unsafe()).AsVector3();
	}

	[Intrinsic]
	public static Vector3 Subtract(Vector3 left, Vector3 right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Sum(Vector3 value)
	{
		return Vector128.Sum(value.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Transform(Vector3 position, Matrix4x4 matrix)
	{
		return Vector4.Transform(position, matrix).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Transform(Vector3 value, Quaternion rotation)
	{
		return Vector4.Transform(value, rotation).AsVector3();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 TransformNormal(Vector3 normal, Matrix4x4 matrix)
	{
		Vector4 addend = matrix.X * normal.X;
		addend = Vector4.MultiplyAddEstimate(matrix.Y, Vector4.Create(normal.Y), addend);
		addend = Vector4.MultiplyAddEstimate(matrix.Z, Vector4.Create(normal.Z), addend);
		return addend.AsVector3();
	}

	[Intrinsic]
	public static Vector3 Truncate(Vector3 vector)
	{
		return Vector128.Truncate(vector.AsVector128Unsafe()).AsVector3();
	}

	[Intrinsic]
	public static Vector3 Xor(Vector3 left, Vector3 right)
	{
		return left ^ right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(float[] array)
	{
		if (array.Length < 3)
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
		if (array.Length - index < 3)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref array[index]), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void CopyTo(Span<float> destination)
	{
		if (destination.Length < 3)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(destination)), this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool TryCopyTo(Span<float> destination)
	{
		if (destination.Length < 3)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(destination)), this);
		return true;
	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector3 other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Vector3 other)
	{
		return this.AsVector128().Equals(other.AsVector128());
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z);
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
		return $"<{X.ToString(format, formatProvider)}{numberGroupSeparator} {Y.ToString(format, formatProvider)}{numberGroupSeparator} {Z.ToString(format, formatProvider)}>";
	}
}
