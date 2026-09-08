using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace System.Numerics;

[Intrinsic]
public struct Plane : IEquatable<Plane>
{
	public Vector3 Normal;

	public float D;

	[Intrinsic]
	public Plane(float x, float y, float z, float d)
	{
		this = Create(x, y, z, d);
	}

	[Intrinsic]
	public Plane(Vector3 normal, float d)
	{
		this = Create(normal, d);
	}

	[Intrinsic]
	public Plane(Vector4 value)
	{
		this = value.AsPlane();
	}

	[Intrinsic]
	public static Plane Create(Vector4 value)
	{
		return value.AsPlane();
	}

	[Intrinsic]
	public static Plane Create(Vector3 normal, float d)
	{
		return Vector4.Create(normal, d).AsPlane();
	}

	[Intrinsic]
	public static Plane Create(float x, float y, float z, float d)
	{
		return Vector128.Create(x, y, z, d).AsPlane();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Plane CreateFromVertices(Vector3 point1, Vector3 point2, Vector3 point3)
	{
		Vector3 vector = Vector3.Normalize(Vector3.Cross(point2 - point1, point3 - point1));
		return Create(vector, 0f - Vector3.Dot(vector, point1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static float Dot(Plane plane, Vector4 value)
	{
		return Vector128.Dot(plane.AsVector128(), value.AsVector128());
	}

	public static float DotCoordinate(Plane plane, Vector3 value)
	{
		return Dot(plane, Vector4.Create(value, 1f));
	}

	public static float DotNormal(Plane plane, Vector3 value)
	{
		return Vector3.Dot(plane.Normal, value);
	}

	public static Plane Normalize(Plane value)
	{
		Vector128<float> vector = Vector128.Create(value.Normal.LengthSquared());
		return Vector128.AndNot(value.AsVector128() / Vector128.Sqrt(vector), Vector128.Equals(vector, Vector128.Create(float.PositiveInfinity))).AsPlane();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Plane Transform(Plane plane, Matrix4x4 matrix)
	{
		Matrix4x4.Invert(matrix, out var result);
		return Vector4.Transform(plane.AsVector4(), Matrix4x4.Transpose(result)).AsPlane();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Plane Transform(Plane plane, Quaternion rotation)
	{
		return Vector4.Transform(plane.AsVector4(), rotation).AsPlane();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public static bool operator ==(Plane value1, Plane value2)
	{
		return value1.AsVector128() == value2.AsVector128();
	}

	[Intrinsic]
	public static bool operator !=(Plane value1, Plane value2)
	{
		return !(value1 == value2);
	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Plane other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Plane other)
	{
		return this.AsVector128().Equals(other.AsVector128());
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(Normal, D);
	}

	public override readonly string ToString()
	{
		return $"{{Normal:{Normal} D:{D}}}";
	}
}
