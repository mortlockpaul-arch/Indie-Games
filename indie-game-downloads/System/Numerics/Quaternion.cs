using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace System.Numerics;

[Intrinsic]
public struct Quaternion : IEquatable<Quaternion>
{
	public float X;

	public float Y;

	public float Z;

	public float W;

	public static Quaternion Zero
	{
		[Intrinsic]
		get
		{
			return default(Quaternion);
		}
	}

	public static Quaternion Identity
	{
		[Intrinsic]
		get
		{
			return Create(0f, 0f, 0f, 1f);
		}
	}

	public float this[int index]
	{
		[Intrinsic]
		readonly get
		{
			return this.AsVector128().GetElement(index);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[Intrinsic]
		set
		{
			this = this.AsVector128().WithElement(index, value).AsQuaternion();
		}
	}

	public readonly bool IsIdentity => this == Identity;

	[Intrinsic]
	public Quaternion(float x, float y, float z, float w)
	{
		this = Create(x, y, z, w);
	}

	[Intrinsic]
	public Quaternion(Vector3 vectorPart, float scalarPart)
	{
		this = Create(vectorPart, scalarPart);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Quaternion operator +(Quaternion value1, Quaternion value2)
	{
		return (value1.AsVector128() + value2.AsVector128()).AsQuaternion();
	}

	public static Quaternion operator /(Quaternion value1, Quaternion value2)
	{
		return value1 * Inverse(value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Quaternion value1, Quaternion value2)
	{
		return value1.AsVector128() == value2.AsVector128();
	}

	[Intrinsic]
	public static bool operator !=(Quaternion value1, Quaternion value2)
	{
		return !(value1 == value2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion operator *(Quaternion value1, Quaternion value2)
	{
		Vector128<float> vector = value1.AsVector128();
		Vector128<float> vector2 = value2.AsVector128();
		Vector128<float> value3 = Vector128.MultiplyAddEstimate(addend: Vector128.MultiplyAddEstimate(addend: Vector128.MultiplyAddEstimate(addend: vector2 * vector.GetElement(3), left: Vector128.Shuffle(vector2, Vector128.Create(3, 2, 1, 0)) * vector.GetElement(0), right: Vector128.Create(1f, -1f, 1f, -1f)), left: Vector128.Shuffle(vector2, Vector128.Create(2, 3, 0, 1)) * vector.GetElement(1), right: Vector128.Create(1f, 1f, -1f, -1f)), left: Vector128.Shuffle(vector2, Vector128.Create(1, 0, 3, 2)) * vector.GetElement(2), right: Vector128.Create(-1f, 1f, 1f, -1f));
		return value3.AsQuaternion();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Quaternion operator *(Quaternion value1, float value2)
	{
		return (value1.AsVector128() * value2).AsQuaternion();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Quaternion operator -(Quaternion value1, Quaternion value2)
	{
		return (value1.AsVector128() - value2.AsVector128()).AsQuaternion();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Quaternion operator -(Quaternion value)
	{
		return (-value.AsVector128()).AsQuaternion();
	}

	[Intrinsic]
	public static Quaternion Add(Quaternion value1, Quaternion value2)
	{
		return value1 + value2;
	}

	public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
	{
		return value2 * value1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Quaternion Conjugate(Quaternion value)
	{
		return (value.AsVector128() * Vector128.Create(-1f, -1f, -1f, 1f)).AsQuaternion();
	}

	[Intrinsic]
	public static Quaternion Create(float x, float y, float z, float w)
	{
		return Vector128.Create(x, y, z, w).AsQuaternion();
	}

	[Intrinsic]
	public static Quaternion Create(Vector3 vectorPart, float scalarPart)
	{
		return Vector4.Create(vectorPart, scalarPart).AsQuaternion();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
	{
		var (value, w) = float.SinCos(angle * 0.5f);
		return (Vector4.Create(axis, 1f) * Vector4.Create(Vector3.Create(value), w)).AsQuaternion();
	}

	public static Quaternion CreateFromRotationMatrix(Matrix4x4 matrix)
	{
		float num = matrix.M11 + matrix.M22 + matrix.M33;
		Quaternion result = default(Quaternion);
		if (num > 0f)
		{
			float num2 = float.Sqrt(num + 1f);
			result.W = num2 * 0.5f;
			num2 = 0.5f / num2;
			result.X = (matrix.M23 - matrix.M32) * num2;
			result.Y = (matrix.M31 - matrix.M13) * num2;
			result.Z = (matrix.M12 - matrix.M21) * num2;
		}
		else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
		{
			float num3 = float.Sqrt(1f + matrix.M11 - matrix.M22 - matrix.M33);
			float num4 = 0.5f / num3;
			result.X = 0.5f * num3;
			result.Y = (matrix.M12 + matrix.M21) * num4;
			result.Z = (matrix.M13 + matrix.M31) * num4;
			result.W = (matrix.M23 - matrix.M32) * num4;
		}
		else if (matrix.M22 > matrix.M33)
		{
			float num5 = float.Sqrt(1f + matrix.M22 - matrix.M11 - matrix.M33);
			float num6 = 0.5f / num5;
			result.X = (matrix.M21 + matrix.M12) * num6;
			result.Y = 0.5f * num5;
			result.Z = (matrix.M32 + matrix.M23) * num6;
			result.W = (matrix.M31 - matrix.M13) * num6;
		}
		else
		{
			float num7 = float.Sqrt(1f + matrix.M33 - matrix.M11 - matrix.M22);
			float num8 = 0.5f / num7;
			result.X = (matrix.M31 + matrix.M13) * num8;
			result.Y = (matrix.M32 + matrix.M23) * num8;
			result.Z = 0.5f * num7;
			result.W = (matrix.M12 - matrix.M21) * num8;
		}
		return result;
	}

	public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
	{
		(Vector3 Sin, Vector3 Cos) tuple = Vector3.SinCos(Vector3.Create(roll, pitch, yaw) * 0.5f);
		Vector3 item = tuple.Sin;
		Vector3 item2 = tuple.Cos;
		float x = item.X;
		float x2 = item2.X;
		float num = x;
		float y = item.Y;
		float y2 = item2.Y;
		float num2 = y;
		float z = item.Z;
		float z2 = item2.Z;
		float num3 = z;
		Unsafe.SkipInit(out Quaternion result);
		result.X = z2 * num2 * x2 + num3 * y2 * num;
		result.Y = num3 * y2 * x2 - z2 * num2 * num;
		result.Z = z2 * y2 * num - num3 * num2 * x2;
		result.W = z2 * y2 * x2 + num3 * num2 * num;
		return result;
	}

	public static Quaternion Divide(Quaternion value1, Quaternion value2)
	{
		return value1 / value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Dot(Quaternion quaternion1, Quaternion quaternion2)
	{
		return Vector128.Dot(quaternion1.AsVector128(), quaternion2.AsVector128());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Quaternion Inverse(Quaternion value)
	{
		Vector128<float> vector = Vector128.Create(value.LengthSquared());
		return Vector128.AndNot(Conjugate(value).AsVector128() / vector, Vector128.LessThanOrEqual(vector, Vector128.Create(1.1920929E-07f))).AsQuaternion();
	}

	public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
	{
		Vector128<float> vector = quaternion2.AsVector128();
		vector = Vector128.ConditionalSelect(Vector128.GreaterThanOrEqual(Vector128.Create(Dot(quaternion1, quaternion2)), Vector128<float>.Zero), vector, -vector);
		return Normalize(Vector128.MultiplyAddEstimate(quaternion1.AsVector128(), Vector128.Create(1f - amount), vector * amount).AsQuaternion());
	}

	public static Quaternion Multiply(Quaternion value1, Quaternion value2)
	{
		return value1 * value2;
	}

	[Intrinsic]
	public static Quaternion Multiply(Quaternion value1, float value2)
	{
		return value1 * value2;
	}

	[Intrinsic]
	public static Quaternion Negate(Quaternion value)
	{
		return -value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static Quaternion Normalize(Quaternion value)
	{
		return (value.AsVector128() / value.Length()).AsQuaternion();
	}

	public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
	{
		float num = Dot(quaternion1, quaternion2);
		float num2 = 1f;
		if (num < 0f)
		{
			num = 0f - num;
			num2 = -1f;
		}
		float num3;
		float num4;
		if (num > 0.999999f)
		{
			num3 = 1f - amount;
			num4 = amount * num2;
		}
		else
		{
			float num5 = float.Acos(num);
			float num6 = 1f / float.Sin(num5);
			num3 = float.Sin((1f - amount) * num5) * num6;
			num4 = float.Sin(amount * num5) * num6 * num2;
		}
		return quaternion1 * num3 + quaternion2 * num4;
	}

	[Intrinsic]
	public static Quaternion Subtract(Quaternion value1, Quaternion value2)
	{
		return value1 - value2;
	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Quaternion other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Quaternion other)
	{
		return this.AsVector128().Equals(other.AsVector128());
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
		return $"{{X:{X} Y:{Y} Z:{Z} W:{W}}}";
	}
}
