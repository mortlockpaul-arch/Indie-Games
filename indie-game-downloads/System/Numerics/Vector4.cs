using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace System.Numerics;

[Intrinsic]
public struct Vector4 : IEquatable<Vector4>, IFormattable
{
	public float X;

	public float Y;

	public float Z;

	public float W;

	public static Vector4 AllBitsSet
	{
		[Intrinsic]
		get
		{
			return Vector128<float>.AllBitsSet.AsVector4();
		}
	}

	public static Vector4 E
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.E);
		}
	}

	public static Vector4 Epsilon
	{
		[Intrinsic]
		get
		{
			return Create(float.Epsilon);
		}
	}

	public static Vector4 NaN
	{
		[Intrinsic]
		get
		{
			return Create(float.NaN);
		}
	}

	public static Vector4 NegativeInfinity
	{
		[Intrinsic]
		get
		{
			return Create(float.NegativeInfinity);
		}
	}

	public static Vector4 NegativeZero
	{
		[Intrinsic]
		get
		{
			return Create(-0f);
		}
	}

	public static Vector4 One
	{
		[Intrinsic]
		get
		{
			return Create(1f);
		}
	}

	public static Vector4 Pi
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.PI);
		}
	}

	public static Vector4 PositiveInfinity
	{
		[Intrinsic]
		get
		{
			return Create(float.PositiveInfinity);
		}
	}

	public static Vector4 Tau
	{
		[Intrinsic]
		get
		{
			return Create((float)Math.PI * 2f);
		}
	}

	public static Vector4 UnitX
	{
		[Intrinsic]
		get
		{
			return CreateScalar(1f);
		}
	}

	public static Vector4 UnitY
	{
		[Intrinsic]
		get
		{
			return Create(0f, 1f, 0f, 0f);
		}
	}

	public static Vector4 UnitZ
	{
		[Intrinsic]
		get
		{
			return Create(0f, 0f, 1f, 0f);
		}
	}

	public static Vector4 UnitW
	{
		[Intrinsic]
		get
		{
			return Create(0f, 0f, 0f, 1f);
		}
	}

	public static Vector4 Zero
	{
		[Intrinsic]
		get
		{
			return default(Vector4);
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
	public Vector4(float value)
	{
		this = Create(value);
	}

	[Intrinsic]
	public Vector4(Vector2 value, float z, float w)
	{
		this = Create(value, z, w);
	}

	[Intrinsic]
	public Vector4(Vector3 value, float w)
	{
		this = Create(value, w);
	}

	[Intrinsic]
	public Vector4(float x, float y, float z, float w)
	{
		this = Create(x, y, z, w);
	}

	[Intrinsic]
	public Vector4(ReadOnlySpan<float> values)
	{
		this = Create(values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator +(Vector4 left, Vector4 right)
	{
		return (left.AsVector128() + right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator /(Vector4 left, Vector4 right)
	{
		return (left.AsVector128() / right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator /(Vector4 value1, float value2)
	{
		return (value1.AsVector128() / value2).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Vector4 left, Vector4 right)
	{
		return left.AsVector128() == right.AsVector128();
	}

	[Intrinsic]
	public static bool operator !=(Vector4 left, Vector4 right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator *(Vector4 left, Vector4 right)
	{
		return (left.AsVector128() * right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator *(Vector4 left, float right)
	{
		return (left.AsVector128() * right).AsVector4();
	}

	[Intrinsic]
	public static Vector4 operator *(float left, Vector4 right)
	{
		return right * left;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator -(Vector4 left, Vector4 right)
	{
		return (left.AsVector128() - right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator -(Vector4 value)
	{
		return (-value.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator &(Vector4 left, Vector4 right)
	{
		return (left.AsVector128() & right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator |(Vector4 left, Vector4 right)
	{
		return (left.AsVector128() | right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator ^(Vector4 left, Vector4 right)
	{
		return (left.AsVector128() ^ right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator <<(Vector4 value, int shiftAmount)
	{
		return (value.AsVector128() << shiftAmount).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator ~(Vector4 value)
	{
		return (~value.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator >>(Vector4 value, int shiftAmount)
	{
		return (value.AsVector128() >> shiftAmount).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator +(Vector4 value)
	{
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 operator >>>(Vector4 value, int shiftAmount)
	{
		return (value.AsVector128() >>> shiftAmount).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Abs(Vector4 value)
	{
		return Vector128.Abs(value.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Add(Vector4 left, Vector4 right)
	{
		return left + right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool All(Vector4 vector, float value)
	{
		return Vector128.All(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AllWhereAllBitsSet(Vector4 vector)
	{
		return Vector128.AllWhereAllBitsSet(vector.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 AndNot(Vector4 left, Vector4 right)
	{
		return Vector128.AndNot(left.AsVector128(), right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool Any(Vector4 vector, float value)
	{
		return Vector128.Any(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool AnyWhereAllBitsSet(Vector4 vector)
	{
		return Vector128.AnyWhereAllBitsSet(vector.AsVector128());
	}

	[Intrinsic]
	public static Vector4 BitwiseAnd(Vector4 left, Vector4 right)
	{
		return left & right;
	}

	[Intrinsic]
	public static Vector4 BitwiseOr(Vector4 left, Vector4 right)
	{
		return left | right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Clamp(Vector4 value1, Vector4 min, Vector4 max)
	{
		return Vector128.Clamp(value1.AsVector128(), min.AsVector128(), max.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 ClampNative(Vector4 value1, Vector4 min, Vector4 max)
	{
		return Vector128.ClampNative(value1.AsVector128(), min.AsVector128(), max.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 ConditionalSelect(Vector4 condition, Vector4 left, Vector4 right)
	{
		return Vector128.ConditionalSelect(condition.AsVector128(), left.AsVector128(), right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 CopySign(Vector4 value, Vector4 sign)
	{
		return Vector128.CopySign(value.AsVector128(), sign.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Cos(Vector4 vector)
	{
		return Vector128.Cos(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int Count(Vector4 vector, float value)
	{
		return Vector128.Count(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int CountWhereAllBitsSet(Vector4 vector)
	{
		return Vector128.CountWhereAllBitsSet(vector.AsVector128());
	}

	[Intrinsic]
	public static Vector4 Create(float value)
	{
		return Vector128.Create(value).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Create(Vector2 vector, float z, float w)
	{
		return vector.AsVector128Unsafe().WithElement(2, z).WithElement(3, w)
			.AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Create(Vector3 vector, float w)
	{
		return vector.AsVector128Unsafe().WithElement(3, w).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Create(float x, float y, float z, float w)
	{
		return Vector128.Create(x, y, z, w).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Create(ReadOnlySpan<float> values)
	{
		return Vector128.Create(values).AsVector4();
	}

	[Intrinsic]
	public static Vector4 CreateScalar(float x)
	{
		return Vector128.CreateScalar(x).AsVector4();
	}

	[Intrinsic]
	public static Vector4 CreateScalarUnsafe(float x)
	{
		return Vector128.CreateScalarUnsafe(x).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Cross(Vector4 vector1, Vector4 vector2)
	{
		Vector128<float> vector3 = vector1.AsVector128();
		Vector128<float> vector4 = vector2.AsVector128();
		return Vector128.MultiplyAddEstimate(addend: -(Vector128.Shuffle(vector3, Vector128.Create(2, 0, 1, 3)) * Vector128.Shuffle(vector4, Vector128.Create(1, 2, 0, 3))).WithElement(3, 0f), left: Vector128.Shuffle(vector3, Vector128.Create(1, 2, 0, 3)), right: Vector128.Shuffle(vector4, Vector128.Create(2, 0, 1, 3))).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 DegreesToRadians(Vector4 degrees)
	{
		return Vector128.DegreesToRadians(degrees.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static float Distance(Vector4 value1, Vector4 value2)
	{
		return float.Sqrt(DistanceSquared(value1, value2));
	}

	[Intrinsic]
	public static float DistanceSquared(Vector4 value1, Vector4 value2)
	{
		return (value1 - value2).LengthSquared();
	}

	[Intrinsic]
	public static Vector4 Divide(Vector4 left, Vector4 right)
	{
		return left / right;
	}

	[Intrinsic]
	public static Vector4 Divide(Vector4 left, float divisor)
	{
		return left / divisor;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Dot(Vector4 vector1, Vector4 vector2)
	{
		return Vector128.Dot(vector1.AsVector128(), vector2.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Exp(Vector4 vector)
	{
		return Vector128.Exp(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Equals(Vector4 left, Vector4 right)
	{
		return Vector128.Equals(left.AsVector128(), right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAll(Vector4 left, Vector4 right)
	{
		return Vector128.EqualsAll(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool EqualsAny(Vector4 left, Vector4 right)
	{
		return Vector128.EqualsAny(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 FusedMultiplyAdd(Vector4 left, Vector4 right, Vector4 addend)
	{
		return Vector128.FusedMultiplyAdd(left.AsVector128(), right.AsVector128(), addend.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 GreaterThan(Vector4 left, Vector4 right)
	{
		return Vector128.GreaterThan(left.AsVector128(), right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAll(Vector4 left, Vector4 right)
	{
		return Vector128.GreaterThanAll(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanAny(Vector4 left, Vector4 right)
	{
		return Vector128.GreaterThanAny(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 GreaterThanOrEqual(Vector4 left, Vector4 right)
	{
		return Vector128.GreaterThanOrEqual(left.AsVector128(), right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAll(Vector4 left, Vector4 right)
	{
		return Vector128.GreaterThanOrEqualAll(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool GreaterThanOrEqualAny(Vector4 left, Vector4 right)
	{
		return Vector128.GreaterThanOrEqualAny(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Hypot(Vector4 x, Vector4 y)
	{
		return Vector128.Hypot(x.AsVector128(), y.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOf(Vector4 vector, float value)
	{
		return Vector128.IndexOf(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int IndexOfWhereAllBitsSet(Vector4 vector)
	{
		return Vector128.IndexOfWhereAllBitsSet(vector.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsEvenInteger(Vector4 vector)
	{
		return Vector128.IsEvenInteger(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsFinite(Vector4 vector)
	{
		return Vector128.IsFinite(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsInfinity(Vector4 vector)
	{
		return Vector128.IsInfinity(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsInteger(Vector4 vector)
	{
		return Vector128.IsInteger(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsNaN(Vector4 vector)
	{
		return Vector128.IsNaN(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsNegative(Vector4 vector)
	{
		return Vector128.IsNegative(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsNegativeInfinity(Vector4 vector)
	{
		return Vector128.IsNegativeInfinity(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsNormal(Vector4 vector)
	{
		return Vector128.IsNormal(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsOddInteger(Vector4 vector)
	{
		return Vector128.IsOddInteger(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsPositive(Vector4 vector)
	{
		return Vector128.IsPositive(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsPositiveInfinity(Vector4 vector)
	{
		return Vector128.IsPositiveInfinity(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsSubnormal(Vector4 vector)
	{
		return Vector128.IsSubnormal(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 IsZero(Vector4 vector)
	{
		return Vector128.IsZero(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOf(Vector4 vector, float value)
	{
		return Vector128.LastIndexOf(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static int LastIndexOfWhereAllBitsSet(Vector4 vector)
	{
		return Vector128.LastIndexOfWhereAllBitsSet(vector.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Lerp(Vector4 value1, Vector4 value2, float amount)
	{
		return Lerp(value1, value2, Create(amount));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Lerp(Vector4 value1, Vector4 value2, Vector4 amount)
	{
		return Vector128.Lerp(value1.AsVector128(), value2.AsVector128(), amount.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 LessThan(Vector4 left, Vector4 right)
	{
		return Vector128.LessThan(left.AsVector128(), right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAll(Vector4 left, Vector4 right)
	{
		return Vector128.LessThanAll(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanAny(Vector4 left, Vector4 right)
	{
		return Vector128.LessThanAny(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 LessThanOrEqual(Vector4 left, Vector4 right)
	{
		return Vector128.LessThanOrEqual(left.AsVector128(), right.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAll(Vector4 left, Vector4 right)
	{
		return Vector128.LessThanOrEqualAll(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool LessThanOrEqualAny(Vector4 left, Vector4 right)
	{
		return Vector128.LessThanOrEqualAny(left.AsVector128(), right.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector4 Load(float* source)
	{
		return Vector128.Load(source).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector4 LoadAligned(float* source)
	{
		return Vector128.LoadAligned(source).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public unsafe static Vector4 LoadAlignedNonTemporal(float* source)
	{
		return Vector128.LoadAlignedNonTemporal(source).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 LoadUnsafe(ref readonly float source)
	{
		return Vector128.LoadUnsafe(in source).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	[CLSCompliant(false)]
	public static Vector4 LoadUnsafe(ref readonly float source, nuint elementOffset)
	{
		return Vector128.LoadUnsafe(in source, elementOffset).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Log(Vector4 vector)
	{
		return Vector128.Log(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Log2(Vector4 vector)
	{
		return Vector128.Log2(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Max(Vector4 value1, Vector4 value2)
	{
		return Vector128.Max(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MaxMagnitude(Vector4 value1, Vector4 value2)
	{
		return Vector128.MaxMagnitude(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MaxMagnitudeNumber(Vector4 value1, Vector4 value2)
	{
		return Vector128.MaxMagnitudeNumber(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MaxNative(Vector4 value1, Vector4 value2)
	{
		return Vector128.MaxNative(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MaxNumber(Vector4 value1, Vector4 value2)
	{
		return Vector128.MaxNumber(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 Min(Vector4 value1, Vector4 value2)
	{
		return Vector128.Min(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MinMagnitude(Vector4 value1, Vector4 value2)
	{
		return Vector128.MinMagnitude(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MinMagnitudeNumber(Vector4 value1, Vector4 value2)
	{
		return Vector128.MinMagnitudeNumber(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MinNative(Vector4 value1, Vector4 value2)
	{
		return Vector128.MinNative(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MinNumber(Vector4 value1, Vector4 value2)
	{
		return Vector128.MinNumber(value1.AsVector128(), value2.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Multiply(Vector4 left, Vector4 right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector4 Multiply(Vector4 left, float right)
	{
		return left * right;
	}

	[Intrinsic]
	public static Vector4 Multiply(float left, Vector4 right)
	{
		return left * right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 MultiplyAddEstimate(Vector4 left, Vector4 right, Vector4 addend)
	{
		return Vector128.MultiplyAddEstimate(left.AsVector128(), right.AsVector128(), addend.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Negate(Vector4 value)
	{
		return -value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool None(Vector4 vector, float value)
	{
		return Vector128.None(vector.AsVector128(), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool NoneWhereAllBitsSet(Vector4 vector)
	{
		return Vector128.NoneWhereAllBitsSet(vector.AsVector128());
	}

	[Intrinsic]
	public static Vector4 Normalize(Vector4 vector)
	{
		return vector / vector.Length();
	}

	[Intrinsic]
	public static Vector4 OnesComplement(Vector4 value)
	{
		return ~value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 RadiansToDegrees(Vector4 radians)
	{
		return Vector128.RadiansToDegrees(radians.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Round(Vector4 vector)
	{
		return Vector128.Round(vector.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Round(Vector4 vector, MidpointRounding mode)
	{
		return Vector128.Round(vector.AsVector128(), mode).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Shuffle(Vector4 vector, byte xIndex, byte yIndex, byte zIndex, byte wIndex)
	{
		return Vector128.Shuffle(vector.AsVector128(), Vector128.Create(xIndex, yIndex, zIndex, wIndex)).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Sin(Vector4 vector)
	{
		return Vector128.Sin(vector.AsVector128()).AsVector4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static (Vector4 Sin, Vector4 Cos) SinCos(Vector4 vector)
	{
		var (value, value2) = Vector128.SinCos(vector.AsVector128());
		return (Sin: value.AsVector4(), Cos: value2.AsVector4());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Vector4 SquareRoot(Vector4 value)
	{
		return Vector128.Sqrt(value.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Subtract(Vector4 left, Vector4 right)
	{
		return left - right;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Sum(Vector4 value)
	{
		return Vector128.Sum(value.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Transform(Vector2 position, Matrix4x4 matrix)
	{
		Vector4 addend = matrix.X * position.X;
		addend = MultiplyAddEstimate(matrix.Y, Create(position.Y), addend);
		return addend + matrix.W;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Transform(Vector2 value, Quaternion rotation)
	{
		return Transform(Create(value, 0f, 1f), rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Transform(Vector3 position, Matrix4x4 matrix)
	{
		Vector4 addend = matrix.X * position.X;
		addend = MultiplyAddEstimate(matrix.Y, Create(position.Y), addend);
		addend = MultiplyAddEstimate(matrix.Z, Create(position.Z), addend);
		return addend + matrix.W;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Transform(Vector3 value, Quaternion rotation)
	{
		return Transform(Create(value, 1f), rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Transform(Vector4 vector, Matrix4x4 matrix)
	{
		Vector4 addend = matrix.X * vector.X;
		addend = MultiplyAddEstimate(matrix.Y, Create(vector.Y), addend);
		addend = MultiplyAddEstimate(matrix.Z, Create(vector.Z), addend);
		return MultiplyAddEstimate(matrix.W, Create(vector.W), addend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Transform(Vector4 value, Quaternion rotation)
	{
		return Quaternion.Concatenate(Quaternion.Concatenate(Quaternion.Conjugate(rotation), value.AsQuaternion()), rotation).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Truncate(Vector4 vector)
	{
		return Vector128.Truncate(vector.AsVector128()).AsVector4();
	}

	[Intrinsic]
	public static Vector4 Xor(Vector4 left, Vector4 right)
	{
		return left ^ right;
	}

	public readonly void CopyTo(float[] array)
	{
		this.AsVector128().CopyTo(array);
	}

	public readonly void CopyTo(float[] array, int index)
	{
		this.AsVector128().CopyTo(array, index);
	}

	public readonly void CopyTo(Span<float> destination)
	{
		this.AsVector128().CopyTo(destination);
	}

	public readonly bool TryCopyTo(Span<float> destination)
	{
		return this.AsVector128().TryCopyTo(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Vector4 other)
	{
		return this.AsVector128().Equals(other.AsVector128());
	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Vector4 other)
		{
			return Equals(other);
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(X, Y, Z, W);
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
		return $"<{X.ToString(format, formatProvider)}{numberGroupSeparator} {Y.ToString(format, formatProvider)}{numberGroupSeparator} {Z.ToString(format, formatProvider)}{numberGroupSeparator} {W.ToString(format, formatProvider)}>";
	}
}
