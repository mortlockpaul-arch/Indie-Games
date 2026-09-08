using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace System.Numerics;

[Intrinsic]
public struct Matrix4x4 : IEquatable<Matrix4x4>
{
	internal struct Impl : IEquatable<Impl>
	{
		public Vector4 X;

		public Vector4 Y;

		public Vector4 Z;

		public Vector4 W;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[UnscopedRef]
		public ref Matrix4x4 AsM4x4()
		{
			return ref Unsafe.As<Impl, Matrix4x4>(ref this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator +(in Impl left, in Impl right)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
			result.W = left.W + right.W;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in Impl left, in Impl right)
		{
			if (left.X == right.X && left.Y == right.Y && left.Z == right.Z)
			{
				return left.W == right.W;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in Impl left, in Impl right)
		{
			if (!(left.X != right.X) && !(left.Y != right.Y) && !(left.Z != right.Z))
			{
				return left.W != right.W;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator *(in Impl left, float right)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = left.X * right;
			result.Y = left.Y * right;
			result.Z = left.Z * right;
			result.W = left.W * right;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator -(in Impl left, in Impl right)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
			result.W = left.W - right.W;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl operator -(in Impl value)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			result.W = -value.W;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateBillboard(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 cameraUpVector, in Vector3 cameraForwardVector)
		{
			Vector3 value = objectPosition - cameraPosition;
			value = ((!(value.LengthSquared() < 0.0001f)) ? Vector3.Normalize(value) : (-cameraForwardVector));
			Vector3 vector = Vector3.Normalize(Vector3.Cross(cameraUpVector, value));
			Vector3 value2 = Vector3.Cross(value, vector);
			Unsafe.SkipInit(out Impl result);
			result.X = vector.AsVector4();
			result.Y = value2.AsVector4();
			result.Z = value.AsVector4();
			result.W = Vector4.Create(objectPosition, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateBillboardLeftHanded(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 cameraUpVector, in Vector3 cameraForwardVector)
		{
			Vector3 value = cameraPosition - objectPosition;
			value = ((!(value.LengthSquared() < 0.0001f)) ? Vector3.Normalize(value) : cameraForwardVector);
			Vector3 vector = Vector3.Normalize(Vector3.Cross(cameraUpVector, value));
			Vector3 value2 = Vector3.Cross(value, vector);
			Unsafe.SkipInit(out Impl result);
			result.X = vector.AsVector4();
			result.Y = value2.AsVector4();
			result.Z = value.AsVector4();
			result.W = Vector4.Create(objectPosition, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateConstrainedBillboard(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 rotateAxis, in Vector3 cameraForwardVector, in Vector3 objectForwardVector)
		{
			Vector3 value = objectPosition - cameraPosition;
			value = ((!(value.LengthSquared() < 0.0001f)) ? Vector3.Normalize(value) : (-cameraForwardVector));
			Vector3 vector = rotateAxis;
			if (float.Abs(Vector3.Dot(vector, value)) > 0.99825466f)
			{
				value = objectForwardVector;
				if (float.Abs(Vector3.Dot(vector, value)) > 0.99825466f)
				{
					value = ((float.Abs(vector.Z) > 0.99825466f) ? Vector3.UnitX : Vector3.Create(0f, 0f, -1f));
				}
			}
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(vector, value));
			Vector3 value2 = Vector3.Normalize(Vector3.Cross(vector2, vector));
			Unsafe.SkipInit(out Impl result);
			result.X = vector2.AsVector4();
			result.Y = vector.AsVector4();
			result.Z = value2.AsVector4();
			result.W = Vector4.Create(objectPosition, 1f);
			return result;
		}

		public static Impl CreateConstrainedBillboardLeftHanded(in Vector3 objectPosition, in Vector3 cameraPosition, in Vector3 rotateAxis, in Vector3 cameraForwardVector, in Vector3 objectForwardVector)
		{
			Vector3 value = cameraPosition - objectPosition;
			value = ((!(value.LengthSquared() < 0.0001f)) ? Vector3.Normalize(value) : cameraForwardVector);
			Vector3 vector = rotateAxis;
			if (float.Abs(Vector3.Dot(vector, value)) > 0.99825466f)
			{
				value = -objectForwardVector;
				if (float.Abs(Vector3.Dot(vector, value)) > 0.99825466f)
				{
					value = ((float.Abs(vector.Z) > 0.99825466f) ? Vector3.Create(-1f, 0f, 0f) : Vector3.Create(0f, 0f, -1f));
				}
			}
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(vector, value));
			Vector3 value2 = Vector3.Normalize(Vector3.Cross(vector2, vector));
			Unsafe.SkipInit(out Impl result);
			result.X = vector2.AsVector4();
			result.Y = vector.AsVector4();
			result.Z = value2.AsVector4();
			result.W = Vector4.Create(objectPosition, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateFromAxisAngle(in Vector3 axis, float angle)
		{
			return CreateFromQuaternion(Quaternion.CreateFromAxisAngle(axis, angle));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateFromQuaternion(in Quaternion quaternion)
		{
			float num = quaternion.X * quaternion.X;
			float num2 = quaternion.Y * quaternion.Y;
			float num3 = quaternion.Z * quaternion.Z;
			float num4 = quaternion.X * quaternion.Y;
			float num5 = quaternion.Z * quaternion.W;
			float num6 = quaternion.Z * quaternion.X;
			float num7 = quaternion.Y * quaternion.W;
			float num8 = quaternion.Y * quaternion.Z;
			float num9 = quaternion.X * quaternion.W;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(1f - 2f * (num2 + num3), 2f * (num4 + num5), 2f * (num6 - num7), 0f);
			result.Y = Vector4.Create(2f * (num4 - num5), 1f - 2f * (num3 + num), 2f * (num8 + num9), 0f);
			result.Z = Vector4.Create(2f * (num6 + num7), 2f * (num8 - num9), 1f - 2f * (num2 + num), 0f);
			result.W = Vector4.UnitW;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			return CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateLookTo(in Vector3 cameraPosition, in Vector3 cameraDirection, in Vector3 cameraUpVector)
		{
			return CreateLookToLeftHanded(in cameraPosition, -cameraDirection, in cameraUpVector);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateLookToLeftHanded(in Vector3 cameraPosition, in Vector3 cameraDirection, in Vector3 cameraUpVector)
		{
			Vector3 vector = Vector3.Normalize(cameraDirection);
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Vector3 vector4 = -cameraPosition;
			Unsafe.SkipInit(out Impl matrix);
			matrix.X = Vector4.Create(vector2, Vector3.Dot(vector2, vector4));
			matrix.Y = Vector4.Create(vector3, Vector3.Dot(vector3, vector4));
			matrix.Z = Vector4.Create(vector, Vector3.Dot(vector, vector4));
			matrix.W = Vector4.UnitW;
			return Transpose(in matrix);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
		{
			float num = 1f / (zNearPlane - zFarPlane);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(2f / width, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, 2f / height, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num, 0f);
			result.W = Vector4.Create(0f, 0f, num * zNearPlane, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateOrthographicLeftHanded(float width, float height, float zNearPlane, float zFarPlane)
		{
			float num = 1f / (zFarPlane - zNearPlane);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(2f / width, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, 2f / height, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num, 0f);
			result.W = Vector4.Create(0f, 0f, (0f - num) * zNearPlane, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			float num = 1f / (right - left);
			float num2 = 1f / (top - bottom);
			float num3 = 1f / (zNearPlane - zFarPlane);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(num + num, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num2 + num2, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num3, 0f);
			result.W = Vector4.Create((0f - (left + right)) * num, (0f - (top + bottom)) * num2, num3 * zNearPlane, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateOrthographicOffCenterLeftHanded(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			float num = 1f / (right - left);
			float num2 = 1f / (top - bottom);
			float num3 = 1f / (zFarPlane - zNearPlane);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(num + num, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num2 + num2, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num3, 0f);
			result.W = Vector4.Create((0f - (left + right)) * num, (0f - (top + bottom)) * num2, (0f - num3) * zNearPlane, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, 0f, "nearPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, 0f, "farPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance, "nearPlaneDistance");
			float num = nearPlaneDistance + nearPlaneDistance;
			float num2 = (float.IsPositiveInfinity(farPlaneDistance) ? (-1f) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance)));
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(num / width, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num / height, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num2, -1f);
			result.W = Vector4.Create(0f, 0f, num2 * nearPlaneDistance, 0f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreatePerspectiveLeftHanded(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, 0f, "nearPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, 0f, "farPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance, "nearPlaneDistance");
			float num = nearPlaneDistance + nearPlaneDistance;
			float num2 = (float.IsPositiveInfinity(farPlaneDistance) ? 1f : (farPlaneDistance / (farPlaneDistance - nearPlaneDistance)));
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(num / width, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num / height, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num2, 1f);
			result.W = Vector4.Create(0f, 0f, (0f - num2) * nearPlaneDistance, 0f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, 0f, "fieldOfView");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, (float)Math.PI, "fieldOfView");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, 0f, "nearPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, 0f, "farPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance, "nearPlaneDistance");
			float num = 1f / float.Tan(fieldOfView * 0.5f);
			float x = num / aspectRatio;
			float num2 = (float.IsPositiveInfinity(farPlaneDistance) ? (-1f) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance)));
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(x, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num2, -1f);
			result.W = Vector4.Create(0f, 0f, num2 * nearPlaneDistance, 0f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreatePerspectiveFieldOfViewLeftHanded(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, 0f, "fieldOfView");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, (float)Math.PI, "fieldOfView");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, 0f, "nearPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, 0f, "farPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance, "nearPlaneDistance");
			float num = 1f / float.Tan(fieldOfView * 0.5f);
			float x = num / aspectRatio;
			float num2 = (float.IsPositiveInfinity(farPlaneDistance) ? 1f : (farPlaneDistance / (farPlaneDistance - nearPlaneDistance)));
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(x, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, num2, 1f);
			result.W = Vector4.Create(0f, 0f, (0f - num2) * nearPlaneDistance, 0f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, 0f, "nearPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, 0f, "farPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance, "nearPlaneDistance");
			float num = nearPlaneDistance + nearPlaneDistance;
			float num2 = 1f / (right - left);
			float num3 = 1f / (top - bottom);
			float num4 = (float.IsPositiveInfinity(farPlaneDistance) ? (-1f) : (farPlaneDistance / (nearPlaneDistance - farPlaneDistance)));
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(num * num2, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num * num3, 0f, 0f);
			result.Z = Vector4.Create((left + right) * num2, (top + bottom) * num3, num4, -1f);
			result.W = Vector4.Create(0f, 0f, num4 * nearPlaneDistance, 0f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreatePerspectiveOffCenterLeftHanded(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlaneDistance, 0f, "nearPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlaneDistance, 0f, "farPlaneDistance");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(nearPlaneDistance, farPlaneDistance, "nearPlaneDistance");
			float num = nearPlaneDistance + nearPlaneDistance;
			float num2 = 1f / (right - left);
			float num3 = 1f / (top - bottom);
			float num4 = (float.IsPositiveInfinity(farPlaneDistance) ? 1f : (farPlaneDistance / (farPlaneDistance - nearPlaneDistance)));
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(num * num2, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, num * num3, 0f, 0f);
			result.Z = Vector4.Create((0f - (left + right)) * num2, (0f - (top + bottom)) * num3, num4, 1f);
			result.W = Vector4.Create(0f, 0f, (0f - num4) * nearPlaneDistance, 0f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateReflection(in Plane value)
		{
			Vector4 vector = Plane.Normalize(value).AsVector4();
			Vector4 right = vector * Vector4.Create(-2f, -2f, -2f, 0f);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.MultiplyAddEstimate(Vector4.Create(vector.X), right, Vector4.UnitX);
			result.Y = Vector4.MultiplyAddEstimate(Vector4.Create(vector.Y), right, Vector4.UnitY);
			result.Z = Vector4.MultiplyAddEstimate(Vector4.Create(vector.Z), right, Vector4.UnitZ);
			result.W = Vector4.MultiplyAddEstimate(Vector4.Create(vector.W), right, Vector4.UnitW);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotationX(float radians)
		{
			(float Sin, float Cos) tuple = float.SinCos(radians);
			float item = tuple.Sin;
			float item2 = tuple.Cos;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.UnitX;
			result.Y = Vector4.Create(0f, item2, item, 0f);
			result.Z = Vector4.Create(0f, 0f - item, item2, 0f);
			result.W = Vector4.UnitW;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotationX(float radians, in Vector3 centerPoint)
		{
			(float Sin, float Cos) tuple = float.SinCos(radians);
			float item = tuple.Sin;
			float item2 = tuple.Cos;
			float y = float.MultiplyAddEstimate(centerPoint.Y, 1f - item2, centerPoint.Z * item);
			float z = float.MultiplyAddEstimate(centerPoint.Z, 1f - item2, (0f - centerPoint.Y) * item);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.UnitX;
			result.Y = Vector4.Create(0f, item2, item, 0f);
			result.Z = Vector4.Create(0f, 0f - item, item2, 0f);
			result.W = Vector4.Create(0f, y, z, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotationY(float radians)
		{
			(float Sin, float Cos) tuple = float.SinCos(radians);
			float item = tuple.Sin;
			float item2 = tuple.Cos;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(item2, 0f, 0f - item, 0f);
			result.Y = Vector4.UnitY;
			result.Z = Vector4.Create(item, 0f, item2, 0f);
			result.W = Vector4.UnitW;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotationY(float radians, in Vector3 centerPoint)
		{
			(float Sin, float Cos) tuple = float.SinCos(radians);
			float item = tuple.Sin;
			float item2 = tuple.Cos;
			float x = float.MultiplyAddEstimate(centerPoint.X, 1f - item2, (0f - centerPoint.Z) * item);
			float z = float.MultiplyAddEstimate(centerPoint.Z, 1f - item2, centerPoint.X * item);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(item2, 0f, 0f - item, 0f);
			result.Y = Vector4.UnitY;
			result.Z = Vector4.Create(item, 0f, item2, 0f);
			result.W = Vector4.Create(x, 0f, z, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotationZ(float radians)
		{
			(float Sin, float Cos) tuple = float.SinCos(radians);
			float item = tuple.Sin;
			float item2 = tuple.Cos;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(item2, item, 0f, 0f);
			result.Y = Vector4.Create(0f - item, item2, 0f, 0f);
			result.Z = Vector4.UnitZ;
			result.W = Vector4.UnitW;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateRotationZ(float radians, in Vector3 centerPoint)
		{
			(float Sin, float Cos) tuple = float.SinCos(radians);
			float item = tuple.Sin;
			float item2 = tuple.Cos;
			float x = float.MultiplyAddEstimate(centerPoint.X, 1f - item2, centerPoint.Y * item);
			float y = float.MultiplyAddEstimate(centerPoint.Y, 1f - item2, (0f - centerPoint.X) * item);
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(item2, item, 0f, 0f);
			result.Y = Vector4.Create(0f - item, item2, 0f, 0f);
			result.Z = Vector4.UnitZ;
			result.W = Vector4.Create(x, y, 0f, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scaleX, float scaleY, float scaleZ)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(scaleX, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, scaleY, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, scaleZ, 0f);
			result.W = Vector4.UnitW;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scaleX, float scaleY, float scaleZ, in Vector3 centerPoint)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(scaleX, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, scaleY, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, scaleZ, 0f);
			result.W = Vector4.Create(centerPoint * (Vector3.One - Vector3.Create(scaleX, scaleY, scaleZ)), 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(in Vector3 scales)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(scales.X, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, scales.Y, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, scales.Z, 0f);
			result.W = Vector4.UnitW;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(in Vector3 scales, in Vector3 centerPoint)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(scales.X, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, scales.Y, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, scales.Z, 0f);
			result.W = Vector4.Create(centerPoint * (Vector3.One - scales), 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scale)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(scale, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, scale, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, scale, 0f);
			result.W = Vector4.UnitW;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateScale(float scale, in Vector3 centerPoint)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(scale, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, scale, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, scale, 0f);
			result.W = Vector4.Create(centerPoint * (Vector3.One - Vector3.Create(scale)), 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateShadow(in Vector3 lightDirection, in Plane plane)
		{
			Vector4 vector = Plane.Normalize(plane).AsVector4();
			Vector4 vector2 = lightDirection.AsVector4();
			float num = Vector4.Dot(vector, vector2);
			vector = -vector;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.MultiplyAddEstimate(vector2, Vector4.Create(vector.X), Vector4.Create(num, 0f, 0f, 0f));
			result.Y = Vector4.MultiplyAddEstimate(vector2, Vector4.Create(vector.Y), Vector4.Create(0f, num, 0f, 0f));
			result.Z = Vector4.MultiplyAddEstimate(vector2, Vector4.Create(vector.Z), Vector4.Create(0f, 0f, num, 0f));
			result.W = Vector4.MultiplyAddEstimate(vector2, Vector4.Create(vector.W), Vector4.Create(0f, 0f, 0f, num));
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateTranslation(in Vector3 position)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.UnitX;
			result.Y = Vector4.UnitY;
			result.Z = Vector4.UnitZ;
			result.W = Vector4.Create(position, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateTranslation(float positionX, float positionY, float positionZ)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.UnitX;
			result.Y = Vector4.UnitY;
			result.Z = Vector4.UnitZ;
			result.W = Vector4.Create(positionX, positionY, positionZ, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
		{
			Unsafe.SkipInit(out Impl result);
			result.W = Vector4.Create(width, height, 0f, 0f);
			result.W *= Vector4.Create(0.5f, 0.5f, 0f, 0f);
			result.X = Vector4.Create(result.W.X, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, 0f - result.W.Y, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, minDepth - maxDepth, 0f);
			result.W += Vector4.Create(x, y, minDepth, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateViewportLeftHanded(float x, float y, float width, float height, float minDepth, float maxDepth)
		{
			Unsafe.SkipInit(out Impl result);
			result.W = Vector4.Create(width, height, 0f, 0f);
			result.W *= Vector4.Create(0.5f, 0.5f, 0f, 0f);
			result.X = Vector4.Create(result.W.X, 0f, 0f, 0f);
			result.Y = Vector4.Create(0f, 0f - result.W.Y, 0f, 0f);
			result.Z = Vector4.Create(0f, 0f, maxDepth - minDepth, 0f);
			result.W += Vector4.Create(x, y, minDepth, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl CreateWorld(in Vector3 position, in Vector3 forward, in Vector3 up)
		{
			Vector3 vector = Vector3.Normalize(-forward);
			Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
			Vector3 value = Vector3.Cross(vector, vector2);
			Unsafe.SkipInit(out Impl result);
			result.X = vector2.AsVector4();
			result.Y = value.AsVector4();
			result.Z = vector.AsVector4();
			result.W = Vector4.Create(position, 1f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool Decompose(in Impl matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
		{
			Impl impl = Identity.AsImpl();
			Vector3* ptr = stackalloc Vector3[3]
			{
				Vector3.UnitX,
				Vector3.UnitY,
				Vector3.UnitZ
			};
			translation = matrix.W.AsVector3();
			Vector3** num = stackalloc Vector3*[3];
			*num = (Vector3*)(&impl.X);
			num[1] = (Vector3*)(&impl.Y);
			num[2] = (Vector3*)(&impl.Z);
			Vector3** ptr2 = num;
			*(*ptr2) = matrix.X.AsVector3();
			*ptr2[1] = matrix.Y.AsVector3();
			*ptr2[2] = matrix.Z.AsVector3();
			float* ptr3 = stackalloc float[3]
			{
				(*ptr2)->Length(),
				ptr2[1]->Length(),
				ptr2[2]->Length()
			};
			float num2 = *ptr3;
			float num3 = ptr3[1];
			float num4 = ptr3[2];
			uint num5;
			uint num6;
			uint num7;
			if (num2 < num3)
			{
				if (num3 < num4)
				{
					num5 = 2u;
					num6 = 1u;
					num7 = 0u;
				}
				else
				{
					num5 = 1u;
					if (num2 < num4)
					{
						num6 = 2u;
						num7 = 0u;
					}
					else
					{
						num6 = 0u;
						num7 = 2u;
					}
				}
			}
			else if (num2 < num4)
			{
				num5 = 2u;
				num6 = 0u;
				num7 = 1u;
			}
			else
			{
				num5 = 0u;
				if (num3 < num4)
				{
					num6 = 2u;
					num7 = 1u;
				}
				else
				{
					num6 = 1u;
					num7 = 2u;
				}
			}
			if (ptr3[num5] < 0.0001f)
			{
				*ptr2[num5] = ptr[num5];
			}
			*ptr2[num5] = Vector3.Normalize(*ptr2[num5]);
			if (ptr3[num6] < 0.0001f)
			{
				float num8 = float.Abs(ptr2[num5]->X);
				float num9 = float.Abs(ptr2[num5]->Y);
				float num10 = float.Abs(ptr2[num5]->Z);
				uint num11 = ((num8 < num9) ? ((!(num9 < num10)) ? ((!(num8 < num10)) ? 2u : 0u) : 0u) : ((num8 < num10) ? 1u : ((num9 < num10) ? 1u : 2u)));
				*ptr2[num6] = Vector3.Cross(*ptr2[num5], ptr[num11]);
			}
			*ptr2[num6] = Vector3.Normalize(*ptr2[num6]);
			if (ptr3[num7] < 0.0001f)
			{
				*ptr2[num7] = Vector3.Cross(*ptr2[num5], *ptr2[num6]);
			}
			*ptr2[num7] = Vector3.Normalize(*ptr2[num7]);
			float num12 = impl.GetDeterminant();
			if (num12 < 0f)
			{
				ptr3[num5] = 0f - ptr3[num5];
				*ptr2[num5] = -(*ptr2[num5]);
				num12 = 0f - num12;
			}
			num12--;
			num12 *= num12;
			bool result;
			if (0.0001f < num12)
			{
				rotation = Quaternion.Identity;
				result = false;
			}
			else
			{
				rotation = Quaternion.CreateFromRotationMatrix(impl.AsM4x4());
				result = true;
			}
			scale = Unsafe.ReadUnaligned<Vector3>(ptr3);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Invert(in Impl matrix, out Impl result)
		{
			if (Sse.IsSupported)
			{
				return SseImpl(in matrix, out result);
			}
			return SoftwareFallback(in matrix, out result);
			static bool SoftwareFallback(in Impl reference, out Impl reference2)
			{
				float x = reference.X.X;
				float y = reference.X.Y;
				float z = reference.X.Z;
				float w = reference.X.W;
				float x2 = reference.Y.X;
				float y2 = reference.Y.Y;
				float z2 = reference.Y.Z;
				float w2 = reference.Y.W;
				float x3 = reference.Z.X;
				float y3 = reference.Z.Y;
				float z3 = reference.Z.Z;
				float w3 = reference.Z.W;
				float x4 = reference.W.X;
				float y4 = reference.W.Y;
				float z4 = reference.W.Z;
				float w4 = reference.W.W;
				float num = z3 * w4 - w3 * z4;
				float num2 = y3 * w4 - w3 * y4;
				float num3 = y3 * z4 - z3 * y4;
				float num4 = x3 * w4 - w3 * x4;
				float num5 = x3 * z4 - z3 * x4;
				float num6 = x3 * y4 - y3 * x4;
				float num7 = y2 * num - z2 * num2 + w2 * num3;
				float num8 = 0f - (x2 * num - z2 * num4 + w2 * num5);
				float num9 = x2 * num2 - y2 * num4 + w2 * num6;
				float num10 = 0f - (x2 * num3 - y2 * num5 + z2 * num6);
				float num11 = x * num7 + y * num8 + z * num9 + w * num10;
				if (float.Abs(num11) < float.Epsilon)
				{
					reference2.W = (reference2.Z = (reference2.Y = (reference2.X = Vector4.Create(float.NaN))));
					return false;
				}
				float num12 = 1f / num11;
				reference2.X.X = num7 * num12;
				reference2.Y.X = num8 * num12;
				reference2.Z.X = num9 * num12;
				reference2.W.X = num10 * num12;
				reference2.X.Y = (0f - (y * num - z * num2 + w * num3)) * num12;
				reference2.Y.Y = (x * num - z * num4 + w * num5) * num12;
				reference2.Z.Y = (0f - (x * num2 - y * num4 + w * num6)) * num12;
				reference2.W.Y = (x * num3 - y * num5 + z * num6) * num12;
				float num13 = z2 * w4 - w2 * z4;
				float num14 = y2 * w4 - w2 * y4;
				float num15 = y2 * z4 - z2 * y4;
				float num16 = x2 * w4 - w2 * x4;
				float num17 = x2 * z4 - z2 * x4;
				float num18 = x2 * y4 - y2 * x4;
				reference2.X.Z = (y * num13 - z * num14 + w * num15) * num12;
				reference2.Y.Z = (0f - (x * num13 - z * num16 + w * num17)) * num12;
				reference2.Z.Z = (x * num14 - y * num16 + w * num18) * num12;
				reference2.W.Z = (0f - (x * num15 - y * num17 + z * num18)) * num12;
				float num19 = z2 * w3 - w2 * z3;
				float num20 = y2 * w3 - w2 * y3;
				float num21 = y2 * z3 - z2 * y3;
				float num22 = x2 * w3 - w2 * x3;
				float num23 = x2 * z3 - z2 * x3;
				float num24 = x2 * y3 - y2 * x3;
				reference2.X.W = (0f - (y * num19 - z * num20 + w * num21)) * num12;
				reference2.Y.W = (x * num19 - z * num22 + w * num23) * num12;
				reference2.Z.W = (0f - (x * num20 - y * num22 + w * num24)) * num12;
				reference2.W.W = (x * num21 - y * num23 + z * num24) * num12;
				return true;
			}
			[CompExactlyDependsOn(typeof(Sse))]
			static bool SseImpl(in Impl reference, out Impl reference2)
			{
				if (!Sse.IsSupported)
				{
					ThrowPlatformNotSupportedException();
				}
				Vector128<float> left = reference.X.AsVector128();
				Vector128<float> right = reference.Y.AsVector128();
				Vector128<float> left2 = reference.Z.AsVector128();
				Vector128<float> right2 = reference.W.AsVector128();
				Vector128<float> left3 = Sse.Shuffle(left, right, 68);
				Vector128<float> left4 = Sse.Shuffle(left, right, 238);
				Vector128<float> right3 = Sse.Shuffle(left2, right2, 68);
				Vector128<float> right4 = Sse.Shuffle(left2, right2, 238);
				left = Sse.Shuffle(left3, right3, 136);
				right = Sse.Shuffle(left3, right3, 221);
				left2 = Sse.Shuffle(left4, right4, 136);
				right2 = Sse.Shuffle(left4, right4, 221);
				Vector128<float> vector = Vector128.Shuffle(left2, Vector128.Create(0, 0, 1, 1));
				Vector128<float> vector2 = Vector128.Shuffle(right2, Vector128.Create(2, 3, 2, 3));
				Vector128<float> vector3 = Vector128.Shuffle(left, Vector128.Create(0, 0, 1, 1));
				Vector128<float> vector4 = Vector128.Shuffle(right, Vector128.Create(2, 3, 2, 3));
				Vector128<float> vector5 = Sse.Shuffle(left2, left, 136);
				Vector128<float> vector6 = Sse.Shuffle(right2, right, 221);
				Vector128<float> addend = vector * vector2;
				Vector128<float> addend2 = vector3 * vector4;
				Vector128<float> addend3 = vector5 * vector6;
				vector = Vector128.Shuffle(left2, Vector128.Create(2, 3, 2, 3));
				vector2 = Vector128.Shuffle(right2, Vector128.Create(0, 0, 1, 1));
				vector3 = Vector128.Shuffle(left, Vector128.Create(2, 3, 2, 3));
				vector4 = Vector128.Shuffle(right, Vector128.Create(0, 0, 1, 1));
				vector5 = Sse.Shuffle(left2, left, 221);
				vector6 = Sse.Shuffle(right2, right, 136);
				addend = Vector128.MultiplyAddEstimate(-vector, vector2, addend);
				addend2 = Vector128.MultiplyAddEstimate(-vector3, vector4, addend2);
				addend3 = Vector128.MultiplyAddEstimate(-vector5, vector6, addend3);
				vector4 = Sse.Shuffle(addend, addend3, 93);
				vector = Vector128.Shuffle(right, Vector128.Create(1, 2, 0, 1));
				vector2 = Sse.Shuffle(vector4, addend, 50);
				vector3 = Vector128.Shuffle(left, Vector128.Create(2, 0, 1, 0));
				vector4 = Sse.Shuffle(vector4, addend, 153);
				Vector128<float> left5 = Sse.Shuffle(addend2, addend3, 253);
				vector5 = Vector128.Shuffle(right2, Vector128.Create(1, 2, 0, 1));
				vector6 = Sse.Shuffle(left5, addend2, 50);
				Vector128<float> vector7 = Vector128.Shuffle(left2, Vector128.Create(2, 0, 1, 0));
				left5 = Sse.Shuffle(left5, addend2, 153);
				Vector128<float> addend4 = vector * vector2;
				Vector128<float> addend5 = vector3 * vector4;
				Vector128<float> addend6 = vector5 * vector6;
				Vector128<float> addend7 = vector7 * left5;
				vector4 = Sse.Shuffle(addend, addend3, 4);
				vector = Vector128.Shuffle(right, Vector128.Create(2, 3, 1, 2));
				vector2 = Sse.Shuffle(addend, vector4, 147);
				vector3 = Vector128.Shuffle(left, Vector128.Create(3, 2, 3, 1));
				vector4 = Sse.Shuffle(addend, vector4, 38);
				left5 = Sse.Shuffle(addend2, addend3, 164);
				vector5 = Vector128.Shuffle(right2, Vector128.Create(2, 3, 1, 2));
				vector6 = Sse.Shuffle(addend2, left5, 147);
				vector7 = Vector128.Shuffle(left2, Vector128.Create(3, 2, 3, 1));
				left5 = Sse.Shuffle(addend2, left5, 38);
				addend4 = Vector128.MultiplyAddEstimate(-vector, vector2, addend4);
				addend5 = Vector128.MultiplyAddEstimate(-vector3, vector4, addend5);
				addend6 = Vector128.MultiplyAddEstimate(-vector5, vector6, addend6);
				addend7 = Vector128.MultiplyAddEstimate(-vector7, left5, addend7);
				vector = Vector128.Shuffle(right, Vector128.Create(3, 0, 3, 0));
				vector2 = Sse.Shuffle(addend, addend3, 74);
				vector2 = Vector128.Shuffle(vector2, Vector128.Create(0, 3, 2, 0));
				vector3 = Vector128.Shuffle(left, Vector128.Create(1, 3, 0, 2));
				vector4 = Sse.Shuffle(addend, addend3, 76);
				vector4 = Vector128.Shuffle(vector4, Vector128.Create(3, 0, 1, 2));
				vector5 = Vector128.Shuffle(right2, Vector128.Create(3, 0, 3, 0));
				vector6 = Sse.Shuffle(addend2, addend3, 234);
				vector6 = Vector128.Shuffle(vector6, Vector128.Create(0, 3, 2, 0));
				vector7 = Vector128.Shuffle(left2, Vector128.Create(1, 3, 0, 2));
				left5 = Sse.Shuffle(addend2, addend3, 236);
				left5 = Vector128.Shuffle(left5, Vector128.Create(3, 0, 1, 2));
				vector *= vector2;
				vector3 *= vector4;
				vector5 *= vector6;
				vector7 *= left5;
				Vector128<float> right5 = addend4 - vector;
				addend4 += vector;
				Vector128<float> right6 = addend5 + vector3;
				addend5 -= vector3;
				Vector128<float> right7 = addend6 - vector5;
				addend6 += vector5;
				Vector128<float> right8 = addend7 + vector7;
				addend7 -= vector7;
				addend4 = Sse.Shuffle(addend4, right5, 216);
				addend5 = Sse.Shuffle(addend5, right6, 216);
				addend6 = Sse.Shuffle(addend6, right7, 216);
				addend7 = Sse.Shuffle(addend7, right8, 216);
				addend4 = Vector128.Shuffle(addend4, Vector128.Create(0, 2, 1, 3));
				addend5 = Vector128.Shuffle(addend5, Vector128.Create(0, 2, 1, 3));
				addend6 = Vector128.Shuffle(addend6, Vector128.Create(0, 2, 1, 3));
				addend7 = Vector128.Shuffle(addend7, Vector128.Create(0, 2, 1, 3));
				float num = Vector4.Dot(addend4.AsVector4(), left.AsVector4());
				if (float.Abs(num) < float.Epsilon)
				{
					reference2.W = (reference2.Z = (reference2.Y = (reference2.X = Vector4.Create(float.NaN))));
					return false;
				}
				Vector128<float> vector8 = Vector128<float>.One / num;
				reference2.X = (addend4 * vector8).AsVector4();
				reference2.Y = (addend5 * vector8).AsVector4();
				reference2.Z = (addend6 * vector8).AsVector4();
				reference2.W = (addend7 * vector8).AsVector4();
				return true;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl Lerp(in Impl left, in Impl right, float amount)
		{
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Lerp(left.X, right.X, amount);
			result.Y = Vector4.Lerp(left.Y, right.Y, amount);
			result.Z = Vector4.Lerp(left.Z, right.Z, amount);
			result.W = Vector4.Lerp(left.W, right.W, amount);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl Transform(in Impl value, in Quaternion rotation)
		{
			float num = rotation.X + rotation.X;
			float num2 = rotation.Y + rotation.Y;
			float num3 = rotation.Z + rotation.Z;
			float num4 = rotation.W * num;
			float num5 = rotation.W * num2;
			float num6 = rotation.W * num3;
			float num7 = rotation.X * num;
			float num8 = rotation.X * num2;
			float num9 = rotation.X * num3;
			float num10 = rotation.Y * num2;
			float num11 = rotation.Y * num3;
			float num12 = rotation.Z * num3;
			float num13 = 1f - num10 - num12;
			float num14 = num8 - num6;
			float num15 = num9 + num5;
			float num16 = num8 + num6;
			float num17 = 1f - num7 - num12;
			float num18 = num11 - num4;
			float num19 = num9 - num5;
			float num20 = num11 + num4;
			float num21 = 1f - num7 - num10;
			Unsafe.SkipInit(out Impl result);
			result.X = Vector4.Create(value.X.X * num13 + value.X.Y * num14 + value.X.Z * num15, value.X.X * num16 + value.X.Y * num17 + value.X.Z * num18, value.X.X * num19 + value.X.Y * num20 + value.X.Z * num21, value.X.W);
			result.Y = Vector4.Create(value.Y.X * num13 + value.Y.Y * num14 + value.Y.Z * num15, value.Y.X * num16 + value.Y.Y * num17 + value.Y.Z * num18, value.Y.X * num19 + value.Y.Y * num20 + value.Y.Z * num21, value.Y.W);
			result.Z = Vector4.Create(value.Z.X * num13 + value.Z.Y * num14 + value.Z.Z * num15, value.Z.X * num16 + value.Z.Y * num17 + value.Z.Z * num18, value.Z.X * num19 + value.Z.Y * num20 + value.Z.Z * num21, value.Z.W);
			result.W = Vector4.Create(value.W.X * num13 + value.W.Y * num14 + value.W.Z * num15, value.W.X * num16 + value.W.Y * num17 + value.W.Z * num18, value.W.X * num19 + value.W.Y * num20 + value.W.Z * num21, value.W.W);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Impl Transpose(in Impl matrix)
		{
			if (false)
			{
			}
			Unsafe.SkipInit(out Impl result);
			if (Sse.IsSupported)
			{
				Vector128<float> left = matrix.X.AsVector128();
				Vector128<float> left2 = matrix.Y.AsVector128();
				Vector128<float> right = matrix.Z.AsVector128();
				Vector128<float> right2 = matrix.W.AsVector128();
				Vector128<float> left3 = Sse.UnpackLow(left, right);
				Vector128<float> right3 = Sse.UnpackLow(left2, right2);
				Vector128<float> left4 = Sse.UnpackHigh(left, right);
				Vector128<float> right4 = Sse.UnpackHigh(left2, right2);
				result.X = Sse.UnpackLow(left3, right3).AsVector4();
				result.Y = Sse.UnpackHigh(left3, right3).AsVector4();
				result.Z = Sse.UnpackLow(left4, right4).AsVector4();
				result.W = Sse.UnpackHigh(left4, right4).AsVector4();
			}
			else
			{
				result.X = Vector4.Create(matrix.X.X, matrix.Y.X, matrix.Z.X, matrix.W.X);
				result.Y = Vector4.Create(matrix.X.Y, matrix.Y.Y, matrix.Z.Y, matrix.W.Y);
				result.Z = Vector4.Create(matrix.X.Z, matrix.Y.Z, matrix.Z.Z, matrix.W.Z);
				result.W = Vector4.Create(matrix.X.W, matrix.Y.W, matrix.Z.W, matrix.W.W);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly bool Equals([NotNullWhen(true)] object obj)
		{
			if (obj is Matrix4x4 matrix4x)
			{
				return Equals(in matrix4x.AsImpl());
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(in Impl other)
		{
			if (X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z))
			{
				return W.Equals(other.W);
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly float GetDeterminant()
		{
			float x = X.X;
			float y = X.Y;
			float z = X.Z;
			float w = X.W;
			float x2 = Y.X;
			float y2 = Y.Y;
			float z2 = Y.Z;
			float w2 = Y.W;
			float x3 = Z.X;
			float y3 = Z.Y;
			float z3 = Z.Z;
			float w3 = Z.W;
			float x4 = W.X;
			float y4 = W.Y;
			float z4 = W.Z;
			float w4 = W.W;
			float num = z3 * w4 - w3 * z4;
			float num2 = y3 * w4 - w3 * y4;
			float num3 = y3 * z4 - z3 * y4;
			float num4 = x3 * w4 - w3 * x4;
			float num5 = x3 * z4 - z3 * x4;
			float num6 = x3 * y4 - y3 * x4;
			return x * (y2 * num - z2 * num2 + w2 * num3) - y * (x2 * num - z2 * num4 + w2 * num5) + z * (x2 * num2 - y2 * num4 + w2 * num6) - w * (x2 * num3 - y2 * num5 + z2 * num6);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(X, Y, Z, W);
		}

		readonly bool IEquatable<Impl>.Equals(Impl other)
		{
			return Equals(in other);
		}

		private static void ThrowPlatformNotSupportedException()
		{
			throw new PlatformNotSupportedException();
		}
	}

	public float M11;

	public float M12;

	public float M13;

	public float M14;

	public float M21;

	public float M22;

	public float M23;

	public float M24;

	public float M31;

	public float M32;

	public float M33;

	public float M34;

	public float M41;

	public float M42;

	public float M43;

	public float M44;

	public static Matrix4x4 Identity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Create(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);
		}
	}

	public readonly bool IsIdentity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (X == Vector4.UnitX && Y == Vector4.UnitY && Z == Vector4.UnitZ)
			{
				return W == Vector4.UnitW;
			}
			return false;
		}
	}

	public Vector3 Translation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return W.AsVector3();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			W = Vector4.Create(value, W.W);
		}
	}

	public Vector4 X
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

	public Vector4 Y
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

	public Vector4 Z
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

	public Vector4 W
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return AsROImpl().W;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			AsImpl().W = value;
		}
	}

	public Vector4 this[int row]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			ref readonly Impl reference = ref AsROImpl();
			if (RuntimeHelpers.IsKnownConstant(row))
			{
				switch (row)
				{
				case 0:
					return reference.X;
				case 1:
					return reference.Y;
				case 2:
					return reference.Z;
				case 3:
					return reference.W;
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					return default(Vector4);
				}
			}
			if ((uint)row >= 4u)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return Unsafe.Add(ref Unsafe.AsRef(in reference.X), row);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			ref Impl reference = ref AsImpl();
			if (RuntimeHelpers.IsKnownConstant(row))
			{
				switch (row)
				{
				case 0:
					reference.X = value;
					break;
				case 1:
					reference.Y = value;
					break;
				case 2:
					reference.Z = value;
					break;
				case 3:
					reference.W = value;
					break;
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					break;
				}
			}
			else
			{
				if ((uint)row >= 4u)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				Unsafe.Add(ref Unsafe.AsRef(in reference.X), row) = value;
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
				ref readonly Impl reference = ref AsROImpl();
				switch (row)
				{
				case 0:
					return reference.X.GetElement(column);
				case 1:
					return reference.Y.GetElement(column);
				case 2:
					return reference.Z.GetElement(column);
				case 3:
					return reference.W.GetElement(column);
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					return 0f;
				}
			}
			if ((uint)row >= 4u || (uint)column >= 4u)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return Unsafe.Add(ref Unsafe.AsRef(in M11), row * 4 + column);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (RuntimeHelpers.IsKnownConstant(row) && RuntimeHelpers.IsKnownConstant(column))
			{
				ref Impl reference = ref AsImpl();
				switch (row)
				{
				case 0:
					reference.X = reference.X.WithElement(column, value);
					break;
				case 1:
					reference.Y = reference.Y.WithElement(column, value);
					break;
				case 2:
					reference.Z = reference.Z.WithElement(column, value);
					break;
				case 3:
					reference.W = reference.W.WithElement(column, value);
					break;
				default:
					ThrowHelper.ThrowArgumentOutOfRangeException();
					break;
				}
			}
			else
			{
				if ((uint)row >= 4u || (uint)column >= 4u)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				Unsafe.Add(ref Unsafe.AsRef(in M11), row * 4 + column) = value;
			}
		}
	}

	public Matrix4x4(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
	{
		this = Create(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
	}

	public Matrix4x4(Matrix3x2 value)
	{
		this = Create(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 operator +(Matrix4x4 value1, Matrix4x4 value2)
	{
		return (value1.AsImpl() + value2.AsImpl()).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Matrix4x4 value1, Matrix4x4 value2)
	{
		return value1.AsImpl() == value2.AsImpl();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Matrix4x4 value1, Matrix4x4 value2)
	{
		return value1.AsImpl() != value2.AsImpl();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 operator *(Matrix4x4 value1, Matrix4x4 value2)
	{
		return Create(Vector4.Transform(value1.X, value2), Vector4.Transform(value1.Y, value2), Vector4.Transform(value1.Z, value2), Vector4.Transform(value1.W, value2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 operator *(Matrix4x4 value1, float value2)
	{
		return (value1.AsImpl() * value2).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 operator -(Matrix4x4 value1, Matrix4x4 value2)
	{
		return (value1.AsImpl() - value2.AsImpl()).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 operator -(Matrix4x4 value)
	{
		return (-value.AsImpl()).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Add(Matrix4x4 value1, Matrix4x4 value2)
	{
		return (value1.AsImpl() + value2.AsImpl()).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Create(float value)
	{
		return Create(Vector4.Create(value));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Create(Matrix3x2 value)
	{
		return Create(value.X.AsVector4(), value.Y.AsVector4(), Vector4.UnitZ, Vector4.Create(value.Z, 0f, 1f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Create(Vector4 value)
	{
		return Create(value, value, value, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Create(Vector4 x, Vector4 y, Vector4 z, Vector4 w)
	{
		Unsafe.SkipInit<Matrix4x4>(out var value);
		value.X = x;
		value.Y = y;
		value.Z = z;
		value.W = w;
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Create(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
	{
		return Create(Vector4.Create(m11, m12, m13, m14), Vector4.Create(m21, m22, m23, m24), Vector4.Create(m31, m32, m33, m34), Vector4.Create(m41, m42, m43, m44));
	}

	public static Matrix4x4 CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
	{
		return Impl.CreateBillboard(in objectPosition, in cameraPosition, in cameraUpVector, in cameraForwardVector).AsM4x4();
	}

	public static Matrix4x4 CreateBillboardLeftHanded(Vector3 objectPosition, Vector3 cameraPosition, Vector3 cameraUpVector, Vector3 cameraForwardVector)
	{
		return Impl.CreateBillboardLeftHanded(in objectPosition, in cameraPosition, in cameraUpVector, in cameraForwardVector).AsM4x4();
	}

	public static Matrix4x4 CreateConstrainedBillboard(Vector3 objectPosition, Vector3 cameraPosition, Vector3 rotateAxis, Vector3 cameraForwardVector, Vector3 objectForwardVector)
	{
		return Impl.CreateConstrainedBillboard(in objectPosition, in cameraPosition, in rotateAxis, in cameraForwardVector, in objectForwardVector).AsM4x4();
	}

	public static Matrix4x4 CreateConstrainedBillboardLeftHanded(Vector3 objectPosition, Vector3 cameraPosition, Vector3 rotateAxis, Vector3 cameraForwardVector, Vector3 objectForwardVector)
	{
		return Impl.CreateConstrainedBillboardLeftHanded(in objectPosition, in cameraPosition, in rotateAxis, in cameraForwardVector, in objectForwardVector).AsM4x4();
	}

	public static Matrix4x4 CreateFromAxisAngle(Vector3 axis, float angle)
	{
		return Impl.CreateFromAxisAngle(in axis, angle).AsM4x4();
	}

	public static Matrix4x4 CreateFromQuaternion(Quaternion quaternion)
	{
		return Impl.CreateFromQuaternion(in quaternion).AsM4x4();
	}

	public static Matrix4x4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
	{
		return Impl.CreateFromYawPitchRoll(yaw, pitch, roll).AsM4x4();
	}

	public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
	{
		return Impl.CreateLookTo(in cameraPosition, cameraTarget - cameraPosition, in cameraUpVector).AsM4x4();
	}

	public static Matrix4x4 CreateLookAtLeftHanded(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
	{
		return Impl.CreateLookToLeftHanded(in cameraPosition, cameraTarget - cameraPosition, in cameraUpVector).AsM4x4();
	}

	public static Matrix4x4 CreateLookTo(Vector3 cameraPosition, Vector3 cameraDirection, Vector3 cameraUpVector)
	{
		return Impl.CreateLookTo(in cameraPosition, in cameraDirection, in cameraUpVector).AsM4x4();
	}

	public static Matrix4x4 CreateLookToLeftHanded(Vector3 cameraPosition, Vector3 cameraDirection, Vector3 cameraUpVector)
	{
		return Impl.CreateLookToLeftHanded(in cameraPosition, in cameraDirection, in cameraUpVector).AsM4x4();
	}

	public static Matrix4x4 CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
	{
		return Impl.CreateOrthographic(width, height, zNearPlane, zFarPlane).AsM4x4();
	}

	public static Matrix4x4 CreateOrthographicLeftHanded(float width, float height, float zNearPlane, float zFarPlane)
	{
		return Impl.CreateOrthographicLeftHanded(width, height, zNearPlane, zFarPlane).AsM4x4();
	}

	public static Matrix4x4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
	{
		return Impl.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane).AsM4x4();
	}

	public static Matrix4x4 CreateOrthographicOffCenterLeftHanded(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
	{
		return Impl.CreateOrthographicOffCenterLeftHanded(left, right, bottom, top, zNearPlane, zFarPlane).AsM4x4();
	}

	public static Matrix4x4 CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
	{
		return Impl.CreatePerspective(width, height, nearPlaneDistance, farPlaneDistance).AsM4x4();
	}

	public static Matrix4x4 CreatePerspectiveLeftHanded(float width, float height, float nearPlaneDistance, float farPlaneDistance)
	{
		return Impl.CreatePerspectiveLeftHanded(width, height, nearPlaneDistance, farPlaneDistance).AsM4x4();
	}

	public static Matrix4x4 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
	{
		return Impl.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance).AsM4x4();
	}

	public static Matrix4x4 CreatePerspectiveFieldOfViewLeftHanded(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
	{
		return Impl.CreatePerspectiveFieldOfViewLeftHanded(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance).AsM4x4();
	}

	public static Matrix4x4 CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
	{
		return Impl.CreatePerspectiveOffCenter(left, right, bottom, top, nearPlaneDistance, farPlaneDistance).AsM4x4();
	}

	public static Matrix4x4 CreatePerspectiveOffCenterLeftHanded(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
	{
		return Impl.CreatePerspectiveOffCenterLeftHanded(left, right, bottom, top, nearPlaneDistance, farPlaneDistance).AsM4x4();
	}

	public static Matrix4x4 CreateReflection(Plane value)
	{
		return Impl.CreateReflection(in value).AsM4x4();
	}

	public static Matrix4x4 CreateRotationX(float radians)
	{
		return Impl.CreateRotationX(radians).AsM4x4();
	}

	public static Matrix4x4 CreateRotationX(float radians, Vector3 centerPoint)
	{
		return Impl.CreateRotationX(radians, in centerPoint).AsM4x4();
	}

	public static Matrix4x4 CreateRotationY(float radians)
	{
		return Impl.CreateRotationY(radians).AsM4x4();
	}

	public static Matrix4x4 CreateRotationY(float radians, Vector3 centerPoint)
	{
		return Impl.CreateRotationY(radians, in centerPoint).AsM4x4();
	}

	public static Matrix4x4 CreateRotationZ(float radians)
	{
		return Impl.CreateRotationZ(radians).AsM4x4();
	}

	public static Matrix4x4 CreateRotationZ(float radians, Vector3 centerPoint)
	{
		return Impl.CreateRotationZ(radians, in centerPoint).AsM4x4();
	}

	public static Matrix4x4 CreateScale(float xScale, float yScale, float zScale)
	{
		return Impl.CreateScale(xScale, yScale, zScale).AsM4x4();
	}

	public static Matrix4x4 CreateScale(float xScale, float yScale, float zScale, Vector3 centerPoint)
	{
		return Impl.CreateScale(xScale, yScale, zScale, in centerPoint).AsM4x4();
	}

	public static Matrix4x4 CreateScale(Vector3 scales)
	{
		return Impl.CreateScale(in scales).AsM4x4();
	}

	public static Matrix4x4 CreateScale(Vector3 scales, Vector3 centerPoint)
	{
		return Impl.CreateScale(in scales, in centerPoint).AsM4x4();
	}

	public static Matrix4x4 CreateScale(float scale)
	{
		return Impl.CreateScale(scale).AsM4x4();
	}

	public static Matrix4x4 CreateScale(float scale, Vector3 centerPoint)
	{
		return Impl.CreateScale(scale, in centerPoint).AsM4x4();
	}

	public static Matrix4x4 CreateShadow(Vector3 lightDirection, Plane plane)
	{
		return Impl.CreateShadow(in lightDirection, in plane).AsM4x4();
	}

	public static Matrix4x4 CreateTranslation(Vector3 position)
	{
		return Impl.CreateTranslation(in position).AsM4x4();
	}

	public static Matrix4x4 CreateTranslation(float xPosition, float yPosition, float zPosition)
	{
		return Impl.CreateTranslation(xPosition, yPosition, zPosition).AsM4x4();
	}

	public static Matrix4x4 CreateViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
	{
		return Impl.CreateViewport(x, y, width, height, minDepth, maxDepth).AsM4x4();
	}

	public static Matrix4x4 CreateViewportLeftHanded(float x, float y, float width, float height, float minDepth, float maxDepth)
	{
		return Impl.CreateViewportLeftHanded(x, y, width, height, minDepth, maxDepth).AsM4x4();
	}

	public static Matrix4x4 CreateWorld(Vector3 position, Vector3 forward, Vector3 up)
	{
		return Impl.CreateWorld(in position, in forward, in up).AsM4x4();
	}

	public static bool Decompose(Matrix4x4 matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
	{
		return Impl.Decompose(in matrix.AsImpl(), out scale, out rotation, out translation);
	}

	public static bool Invert(Matrix4x4 matrix, out Matrix4x4 result)
	{
		Unsafe.SkipInit<Matrix4x4>(out result);
		return Impl.Invert(in matrix.AsImpl(), out result.AsImpl());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Lerp(Matrix4x4 matrix1, Matrix4x4 matrix2, float amount)
	{
		return Impl.Lerp(in matrix1.AsImpl(), in matrix2.AsImpl(), amount).AsM4x4();
	}

	public static Matrix4x4 Multiply(Matrix4x4 value1, Matrix4x4 value2)
	{
		return value1 * value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Multiply(Matrix4x4 value1, float value2)
	{
		return (value1.AsImpl() * value2).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Negate(Matrix4x4 value)
	{
		return (-value.AsImpl()).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Subtract(Matrix4x4 value1, Matrix4x4 value2)
	{
		return (value1.AsImpl() - value2.AsImpl()).AsM4x4();
	}

	public static Matrix4x4 Transform(Matrix4x4 value, Quaternion rotation)
	{
		return Impl.Transform(in value.AsImpl(), in rotation).AsM4x4();
	}

	public static Matrix4x4 Transpose(Matrix4x4 matrix)
	{
		return Impl.Transpose(in matrix.AsImpl()).AsM4x4();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		return AsROImpl().Equals(obj);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Matrix4x4 other)
	{
		return AsROImpl().Equals(in other.AsImpl());
	}

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
	public readonly Vector4 GetRow(int index)
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
		return $"{{ {{M11:{M11} M12:{M12} M13:{M13} M14:{M14}}} {{M21:{M21} M22:{M22} M23:{M23} M24:{M24}}} {{M31:{M31} M32:{M32} M33:{M33} M34:{M34}}} {{M41:{M41} M42:{M42} M43:{M43} M44:{M44}}} }}";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Matrix4x4 WithElement(int row, int column, float value)
	{
		Matrix4x4 result = this;
		result[row, column] = value;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Matrix4x4 WithRow(int index, Vector4 value)
	{
		Matrix4x4 result = this;
		result[index] = value;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[UnscopedRef]
	internal ref Impl AsImpl()
	{
		return ref Unsafe.As<Matrix4x4, Impl>(ref this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[UnscopedRef]
	internal readonly ref readonly Impl AsROImpl()
	{
		return ref Unsafe.As<Matrix4x4, Impl>(ref Unsafe.AsRef<Matrix4x4>(this));
	}
}
