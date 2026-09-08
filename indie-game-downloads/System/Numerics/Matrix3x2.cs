using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Numerics;

[Intrinsic]
public struct Matrix3x2 : IEquatable<Matrix3x2>
{
	internal struct Impl : IEquatable<Impl>
	{
		public Vector2 X;

		public Vector2 Y;

		public Vector2 Z;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[UnscopedRef]
		public ref Matrix3x2 AsM3x2()
		{
			return ref Unsafe.As<Impl, Matrix3x2>(ref this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator +(in Impl left, in Impl right)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Impl left, in Impl right)
		{
			if (left.X == right.X && left.Y == right.Y)
			{
				return left.Z == right.Z;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Impl left, in Impl right)
		{
			if (!(left.X != right.X) && !(left.Y != right.Y))
			{
				return left.Z != right.Z;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator *(in Impl left, in Impl right)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.Create(left.X.X * right.X.X + left.X.Y * right.Y.X, left.X.X * right.X.Y + left.X.Y * right.Y.Y);
			result.Y = Vector2.Create(left.Y.X * right.X.X + left.Y.Y * right.Y.X, left.Y.X * right.X.Y + left.Y.Y * right.Y.Y);
			result.Z = Vector2.Create(left.Z.X * right.X.X + left.Z.Y * right.Y.X + right.Z.X, left.Z.X * right.X.Y + left.Z.Y * right.Y.Y + right.Z.Y);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator *(in Impl left, float right)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = left.X * right;
			result.Y = left.Y * right;
			result.Z = left.Z * right;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator -(in Impl left, in Impl right)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator -(in Impl value)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotation(float radians)
		{
			radians = float.Ieee754Remainder(radians, (float)Math.PI * 2f);
			float num;
			float num2;
			if (radians > -1.7453294E-05f && radians < 1.7453294E-05f)
			{
				num = 1f;
				num2 = 0f;
			}
			else if (radians > 1.570779f && radians < 1.5708138f)
			{
				num = 0f;
				num2 = 1f;
			}
			else if (radians < -3.1415753f || radians > 3.1415753f)
			{
				num = -1f;
				num2 = 0f;
			}
			else if (!(radians > -1.5708138f) || !(radians < -1.570779f))
			{
				(num2, num) = float.SinCos(radians);
			}
			else
			{
				num = 0f;
				num2 = -1f;
			}
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.Create(num, num2);
			result.Y = Vector2.Create(0f - num2, num);
			result.Z = Vector2.Zero;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotation(float radians, Vector2 centerPoint)
		{
			radians = float.Ieee754Remainder(radians, (float)Math.PI * 2f);
			float num;
			float num2;
			if (radians > -1.7453294E-05f && radians < 1.7453294E-05f)
			{
				num = 1f;
				num2 = 0f;
			}
			else if (radians > 1.570779f && radians < 1.5708138f)
			{
				num = 0f;
				num2 = 1f;
			}
			else if (radians < -3.1415753f || radians > 3.1415753f)
			{
				num = -1f;
				num2 = 0f;
			}
			else if (!(radians > -1.5708138f) || !(radians < -1.570779f))
			{
				(num2, num) = float.SinCos(radians);
			}
			else
			{
				num = 0f;
				num2 = -1f;
			}
			float x = centerPoint.X * (1f - num) + centerPoint.Y * num2;
			float y = centerPoint.Y * (1f - num) - centerPoint.X * num2;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.Create(num, num2);
			result.Y = Vector2.Create(0f - num2, num);
			result.Z = Vector2.Create(x, y);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(Vector2 scales)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.CreateScalar(scales.X);
			result.Y = Vector2.Create(0f, scales.Y);
			result.Z = Vector2.Zero;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scaleX, float scaleY)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.CreateScalar(scaleX);
			result.Y = Vector2.Create(0f, scaleY);
			result.Z = Vector2.Zero;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scaleX, float scaleY, Vector2 centerPoint)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.CreateScalar(scaleX);
			result.Y = Vector2.Create(0f, scaleY);
			result.Z = centerPoint * (Vector2.One - Vector2.Create(scaleX, scaleY));
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(Vector2 scales, Vector2 centerPoint)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.CreateScalar(scales.X);
			result.Y = Vector2.Create(0f, scales.Y);
			result.Z = centerPoint * (Vector2.One - scales);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scale)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.CreateScalar(scale);
			result.Y = Vector2.Create(0f, scale);
			result.Z = Vector2.Zero;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scale, Vector2 centerPoint)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.CreateScalar(scale);
			result.Y = Vector2.Create(0f, scale);
			result.Z = centerPoint * (Vector2.One - Vector2.Create(scale));
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateSkew(float radiansX, float radiansY)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.Create(1f, float.Tan(radiansY));
			result.Y = Vector2.Create(float.Tan(radiansX), 1f);
			result.Z = Vector2.Zero;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateSkew(float radiansX, float radiansY, Vector2 centerPoint)
		{
			float num = float.Tan(radiansX);
			float num2 = float.Tan(radiansY);
			float x = (0f - centerPoint.Y) * num;
			float y = (0f - centerPoint.X) * num2;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.Create(1f, num2);
			result.Y = Vector2.Create(num, 1f);
			result.Z = Vector2.Create(x, y);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateTranslation(Vector2 position)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.UnitX;
			result.Y = Vector2.UnitY;
			result.Z = position;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateTranslation(float positionX, float positionY)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.UnitX;
			result.Y = Vector2.UnitY;
			result.Z = Vector2.Create(positionX, positionY);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Invert(in Impl matrix, out Impl result)
		{
			float num = matrix.X.X * matrix.Y.Y - matrix.Y.X * matrix.X.Y;
			if (float.Abs(num) < float.Epsilon)
			{
				result.Z = (result.Y = (result.X = Vector2.Create(float.NaN)));
				return false;
			}
			float num2 = 1f / num;
			result.X = Vector2.Create(matrix.Y.Y * num2, (0f - matrix.X.Y) * num2);
			result.Y = Vector2.Create((0f - matrix.Y.X) * num2, matrix.X.X * num2);
			result.Z = Vector2.Create((matrix.Y.X * matrix.Z.Y - matrix.Z.X * matrix.Y.Y) * num2, (matrix.Z.X * matrix.X.Y - matrix.X.X * matrix.Z.Y) * num2);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl Lerp(in Impl left, in Impl right, float amount)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector2.Lerp(left.X, right.X, amount);
			result.Y = Vector2.Lerp(left.Y, right.Y, amount);
			result.Z = Vector2.Lerp(left.Z, right.Z, amount);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals([NotNullWhen(true)] object obj)
		{
			if (obj is Matrix3x2 matrix3x)
			{
				return Equals(in matrix3x.AsImpl());
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Impl other)
		{
			if (X.Equals(other.X) && Y.Equals(other.Y))
			{
				return Z.Equals(other.Z);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float GetDeterminant()
		{
			return X.X * Y.Y - Y.X * X.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(X, Y, Z);
		}

		readonly bool IEquatable<Impl>.Equals(Impl other)
		{
			return Equals(in other);
		}
	}

	public float M11;

	public float M12;

	public float M21;

	public float M22;

	public float M31;

	public float M32;

	public static Matrix3x2 Identity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Create(Vector2.UnitX, Vector2.UnitY, Vector2.Zero);
		}
	}

	public readonly bool IsIdentity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (X == Vector2.UnitX && Y == Vector2.UnitY)
			{
				return Z == Vector2.Zero;
			}
			return false;
		}
	}

	public Vector2 Translation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Z;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Z = value;
		}
	}

	public Vector2 X
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return AsROImpl().X;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			AsImpl().X = value;
		}
	}

	public Vector2 Y
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return AsROImpl().Y;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			AsImpl().Y = value;
		}
	}

	public Vector2 Z
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return AsROImpl().Z;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			AsImpl().Z = value;
		}
	}

	public Vector2 this[int row]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			if (RuntimeHelpers.IsKnownConstant(row))
			{
				switch (row)
				{
				case 0:
					return X;
				case 1:
					return Y;
				case 2:
					return Z;
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					return default(Vector2);
				}
			}
			if ((uint)row >= 3u)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return Unsafe.Add(ref Unsafe.AsRef(in AsROImpl()).X, row);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (RuntimeHelpers.IsKnownConstant(row))
			{
				switch (row)
				{
				case 0:
					X = value;
					break;
				case 1:
					Y = value;
					break;
				case 2:
					Z = value;
					break;
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					break;
				}
			}
			else
			{
				if ((uint)row >= 3u)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				Unsafe.Add(ref AsImpl().X, row) = value;
			}
		}
	}

	public float this[int row, int column]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			if (RuntimeHelpers.IsKnownConstant(row) && RuntimeHelpers.IsKnownConstant(column))
			{
				switch (row)
				{
				case 0:
					return X.GetElement(column);
				case 1:
					return Y.GetElement(column);
				case 2:
					return Z.GetElement(column);
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					return 0f;
				}
			}
			if ((uint)row >= 3u || (uint)column >= 2u)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return Unsafe.Add(ref Unsafe.AsRef(in M11), row * 2 + column);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (RuntimeHelpers.IsKnownConstant(row) && RuntimeHelpers.IsKnownConstant(column))
			{
				switch (row)
				{
				case 0:
					X = X.WithElement(column, value);
					break;
				case 1:
					Y = Y.WithElement(column, value);
					break;
				case 2:
					Z = Z.WithElement(column, value);
					break;
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					break;
				}
			}
			else
			{
				if ((uint)row >= 3u || (uint)column >= 2u)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				Unsafe.Add(ref M11, row * 2 + column) = value;
			}
		}
	}

	public Matrix3x2(float m11, float m12, float m21, float m22, float m31, float m32)
	{
		this = Create(m11, m12, m21, m22, m31, m32);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 operator +(Matrix3x2 value1, Matrix3x2 value2)
	{
		return (value1.AsImpl() + value2.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Matrix3x2 value1, Matrix3x2 value2)
	{
		return value1.AsImpl() == value2.AsImpl();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Matrix3x2 value1, Matrix3x2 value2)
	{
		return value1.AsImpl() != value2.AsImpl();
	}

	public static Matrix3x2 operator *(Matrix3x2 value1, Matrix3x2 value2)
	{
		return (value1.AsImpl() * value2.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 operator *(Matrix3x2 value1, float value2)
	{
		return (value1.AsImpl() * value2).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 operator -(Matrix3x2 value1, Matrix3x2 value2)
	{
		return (value1.AsImpl() - value2.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 operator -(Matrix3x2 value)
	{
		return (-value.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Add(Matrix3x2 value1, Matrix3x2 value2)
	{
		return (value1.AsImpl() + value2.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Create(float value)
	{
		return Create(Vector2.Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Create(Vector2 value)
	{
		return Create(value, value, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Create(Vector2 x, Vector2 y, Vector2 z)
	{
		Unsafe.SkipInit<Matrix3x2>(out var value);
		value.X = x;
		value.Y = y;
		value.Z = z;
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Create(float m11, float m12, float m21, float m22, float m31, float m32)
	{
		return Create(Vector2.Create(m11, m12), Vector2.Create(m21, m22), Vector2.Create(m31, m32));
	}

	public static Matrix3x2 CreateRotation(float radians)
	{
		return Impl.CreateRotation(radians).AsM3x2();
	}

	public static Matrix3x2 CreateRotation(float radians, Vector2 centerPoint)
	{
		return Impl.CreateRotation(radians, centerPoint).AsM3x2();
	}

	public static Matrix3x2 CreateScale(Vector2 scales)
	{
		return Impl.CreateScale(scales).AsM3x2();
	}

	public static Matrix3x2 CreateScale(float xScale, float yScale)
	{
		return Impl.CreateScale(xScale, yScale).AsM3x2();
	}

	public static Matrix3x2 CreateScale(float xScale, float yScale, Vector2 centerPoint)
	{
		return Impl.CreateScale(xScale, yScale, centerPoint).AsM3x2();
	}

	public static Matrix3x2 CreateScale(Vector2 scales, Vector2 centerPoint)
	{
		return Impl.CreateScale(scales, centerPoint).AsM3x2();
	}

	public static Matrix3x2 CreateScale(float scale)
	{
		return Impl.CreateScale(scale).AsM3x2();
	}

	public static Matrix3x2 CreateScale(float scale, Vector2 centerPoint)
	{
		return Impl.CreateScale(scale, centerPoint).AsM3x2();
	}

	public static Matrix3x2 CreateSkew(float radiansX, float radiansY)
	{
		return Impl.CreateSkew(radiansX, radiansY).AsM3x2();
	}

	public static Matrix3x2 CreateSkew(float radiansX, float radiansY, Vector2 centerPoint)
	{
		return Impl.CreateSkew(radiansX, radiansY, centerPoint).AsM3x2();
	}

	public static Matrix3x2 CreateTranslation(Vector2 position)
	{
		return Impl.CreateTranslation(position).AsM3x2();
	}

	public static Matrix3x2 CreateTranslation(float xPosition, float yPosition)
	{
		return Impl.CreateTranslation(xPosition, yPosition).AsM3x2();
	}

	public static bool Invert(Matrix3x2 matrix, out Matrix3x2 result)
	{
		Unsafe.SkipInit<Matrix3x2>(out result);
		return Impl.Invert(in matrix.AsImpl(), out result.AsImpl());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Lerp(Matrix3x2 matrix1, Matrix3x2 matrix2, float amount)
	{
		return Impl.Lerp(in matrix1.AsImpl(), in matrix2.AsImpl(), amount).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Multiply(Matrix3x2 value1, Matrix3x2 value2)
	{
		return (value1.AsImpl() * value2.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Multiply(Matrix3x2 value1, float value2)
	{
		return (value1.AsImpl() * value2).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Negate(Matrix3x2 value)
	{
		return (-value.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix3x2 Subtract(Matrix3x2 value1, Matrix3x2 value2)
	{
		return (value1.AsImpl() - value2.AsImpl()).AsM3x2();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		return AsROImpl().Equals(obj);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Matrix3x2 other)
	{
		return AsROImpl().Equals(in other.AsImpl());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float GetDeterminant()
	{
		return AsROImpl().GetDeterminant();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float GetElement(int row, int column)
	{
		return this[row, column];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector2 GetRow(int index)
	{
		return this[index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return AsROImpl().GetHashCode();
	}

	public override readonly string ToString()
	{
		return $"{{ {{M11:{M11} M12:{M12}}} {{M21:{M21} M22:{M22}}} {{M31:{M31} M32:{M32}}} }}";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Matrix3x2 WithElement(int row, int column, float value)
	{
		Matrix3x2 result = this;
		result[row, column] = value;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Matrix3x2 WithRow(int index, Vector2 value)
	{
		Matrix3x2 result = this;
		result[index] = value;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[UnscopedRef]
	internal ref Impl AsImpl()
	{
		return ref Unsafe.As<Matrix3x2, Impl>(ref this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[UnscopedRef]
	internal readonly ref readonly Impl AsROImpl()
	{
		return ref Unsafe.As<Matrix3x2, Impl>(ref Unsafe.AsRef<Matrix3x2>(this));
	}
}
