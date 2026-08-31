using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
/// Contains helper methods for testing collisions between boxes.
/// </summary>
public static class BoxBoxCollider
{
	private struct BoxFace
	{
		public int Id1;

		public int Id2;

		public int Id3;

		public int Id4;

		public Vector3 V1;

		public Vector3 V2;

		public Vector3 V3;

		public Vector3 V4;

		public Vector3 Normal;

		public float Width;

		public float Height;

		public int GetId(int i)
		{
			return i switch
			{
				0 => Id1, 
				1 => Id2, 
				2 => Id3, 
				3 => Id4, 
				_ => -1, 
			};
		}

		public void GetVertex(int i, out Vector3 v)
		{
			switch (i)
			{
			case 0:
				v = V1;
				break;
			case 1:
				v = V2;
				break;
			case 2:
				v = V3;
				break;
			case 3:
				v = V4;
				break;
			default:
				v = Toolbox.NoVector;
				break;
			}
		}

		internal void GetEdge(int i, out FaceEdge clippingEdge)
		{
			Vector3 value;
			switch (i)
			{
			case 0:
				clippingEdge.A = V1;
				clippingEdge.B = V2;
				value = V3;
				clippingEdge.Id = GetEdgeId(Id1, Id2);
				break;
			case 1:
				clippingEdge.A = V2;
				clippingEdge.B = V3;
				value = V4;
				clippingEdge.Id = GetEdgeId(Id2, Id3);
				break;
			case 2:
				clippingEdge.A = V3;
				clippingEdge.B = V4;
				value = V1;
				clippingEdge.Id = GetEdgeId(Id3, Id4);
				break;
			case 3:
				clippingEdge.A = V4;
				clippingEdge.B = V1;
				value = V2;
				clippingEdge.Id = GetEdgeId(Id4, Id1);
				break;
			default:
				throw new IndexOutOfRangeException();
			}
			Vector3.Subtract(ref clippingEdge.B, ref clippingEdge.A, out var result);
			result.Normalize();
			Vector3.Cross(ref result, ref Normal, out clippingEdge.Perpendicular);
			Vector3.Subtract(ref value, ref clippingEdge.A, out var result2);
			Vector3.Dot(ref clippingEdge.Perpendicular, ref result2, out var result3);
			if (result3 > 0f)
			{
				clippingEdge.Perpendicular.X = 0f - clippingEdge.Perpendicular.X;
				clippingEdge.Perpendicular.Y = 0f - clippingEdge.Perpendicular.Y;
				clippingEdge.Perpendicular.Z = 0f - clippingEdge.Perpendicular.Z;
			}
			Vector3.Dot(ref clippingEdge.A, ref clippingEdge.Perpendicular, out clippingEdge.EdgeDistance);
		}
	}

	private struct FaceEdge : IEquatable<FaceEdge>
	{
		public Vector3 A;

		public Vector3 B;

		public float EdgeDistance;

		public int Id;

		public Vector3 Perpendicular;

		public bool Equals(FaceEdge other)
		{
			return other.Id == Id;
		}

		public bool IsPointInside(ref Vector3 point)
		{
			Vector3.Dot(ref point, ref Perpendicular, out var result);
			return result < EdgeDistance;
		}
	}

	/// <summary>
	/// Determines if the two boxes are colliding.
	/// </summary>
	/// <param name="a">First box to collide.</param>
	/// <param name="b">Second box to collide.</param>
	/// <param name="transformA">Transform to apply to shape a.</param>
	/// <param name="transformB">Transform to apply to shape b.</param>
	/// <returns>Whether or not the boxes collide.</returns>
	public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB)
	{
		float halfWidth = a.HalfWidth;
		float halfHeight = a.HalfHeight;
		float halfLength = a.HalfLength;
		float halfWidth2 = b.HalfWidth;
		float halfHeight2 = b.HalfHeight;
		float halfLength2 = b.HalfLength;
		Matrix3X3.CreateFromQuaternion(ref transformA.Orientation, out var result);
		Matrix3X3.CreateFromQuaternion(ref transformB.Orientation, out var result2);
		Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
		Matrix3X3 matrix3X = default(Matrix3X3);
		matrix3X.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
		matrix3X.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
		matrix3X.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
		Matrix3X3 matrix3X2 = default(Matrix3X3);
		matrix3X2.M11 = Math.Abs(matrix3X.M11) + 1E-07f;
		matrix3X2.M12 = Math.Abs(matrix3X.M12) + 1E-07f;
		matrix3X2.M13 = Math.Abs(matrix3X.M13) + 1E-07f;
		float x = result3.X;
		result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
		float num = halfWidth2 * matrix3X2.M11 + halfHeight2 * matrix3X2.M12 + halfLength2 * matrix3X2.M13;
		if (Math.Abs(result3.X) > halfWidth + num)
		{
			return false;
		}
		matrix3X.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
		matrix3X.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
		matrix3X.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
		matrix3X2.M21 = Math.Abs(matrix3X.M21) + 1E-07f;
		matrix3X2.M22 = Math.Abs(matrix3X.M22) + 1E-07f;
		matrix3X2.M23 = Math.Abs(matrix3X.M23) + 1E-07f;
		float y = result3.Y;
		result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
		num = halfWidth2 * matrix3X2.M21 + halfHeight2 * matrix3X2.M22 + halfLength2 * matrix3X2.M23;
		if (Math.Abs(result3.Y) > halfHeight + num)
		{
			return false;
		}
		matrix3X.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
		matrix3X.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
		matrix3X.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
		matrix3X2.M31 = Math.Abs(matrix3X.M31) + 1E-07f;
		matrix3X2.M32 = Math.Abs(matrix3X.M32) + 1E-07f;
		matrix3X2.M33 = Math.Abs(matrix3X.M33) + 1E-07f;
		result3.Z = x * result.M31 + y * result.M32 + result3.Z * result.M33;
		num = halfWidth2 * matrix3X2.M31 + halfHeight2 * matrix3X2.M32 + halfLength2 * matrix3X2.M33;
		if (Math.Abs(result3.Z) > halfLength + num)
		{
			return false;
		}
		float num2 = halfWidth * matrix3X2.M11 + halfHeight * matrix3X2.M21 + halfLength * matrix3X2.M31;
		if (Math.Abs(result3.X * matrix3X.M11 + result3.Y * matrix3X.M21 + result3.Z * matrix3X.M31) > num2 + halfWidth2)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M12 + halfHeight * matrix3X2.M22 + halfLength * matrix3X2.M32;
		if (Math.Abs(result3.X * matrix3X.M12 + result3.Y * matrix3X.M22 + result3.Z * matrix3X.M32) > num2 + halfHeight2)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M13 + halfHeight * matrix3X2.M23 + halfLength * matrix3X2.M33;
		if (Math.Abs(result3.X * matrix3X.M13 + result3.Y * matrix3X.M23 + result3.Z * matrix3X.M33) > num2 + halfLength2)
		{
			return false;
		}
		num2 = halfHeight * matrix3X2.M31 + halfLength * matrix3X2.M21;
		num = halfHeight2 * matrix3X2.M13 + halfLength2 * matrix3X2.M12;
		if (Math.Abs(result3.Z * matrix3X.M21 - result3.Y * matrix3X.M31) > num2 + num)
		{
			return false;
		}
		num2 = halfHeight * matrix3X2.M32 + halfLength * matrix3X2.M22;
		num = halfWidth2 * matrix3X2.M13 + halfLength2 * matrix3X2.M11;
		if (Math.Abs(result3.Z * matrix3X.M22 - result3.Y * matrix3X.M32) > num2 + num)
		{
			return false;
		}
		num2 = halfHeight * matrix3X2.M33 + halfLength * matrix3X2.M23;
		num = halfWidth2 * matrix3X2.M12 + halfHeight2 * matrix3X2.M11;
		if (Math.Abs(result3.Z * matrix3X.M23 - result3.Y * matrix3X.M33) > num2 + num)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M31 + halfLength * matrix3X2.M11;
		num = halfHeight2 * matrix3X2.M23 + halfLength2 * matrix3X2.M22;
		if (Math.Abs(result3.X * matrix3X.M31 - result3.Z * matrix3X.M11) > num2 + num)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M32 + halfLength * matrix3X2.M12;
		num = halfWidth2 * matrix3X2.M23 + halfLength2 * matrix3X2.M21;
		if (Math.Abs(result3.X * matrix3X.M32 - result3.Z * matrix3X.M12) > num2 + num)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M33 + halfLength * matrix3X2.M13;
		num = halfWidth2 * matrix3X2.M22 + halfHeight2 * matrix3X2.M21;
		if (Math.Abs(result3.X * matrix3X.M33 - result3.Z * matrix3X.M13) > num2 + num)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M21 + halfHeight * matrix3X2.M11;
		num = halfHeight2 * matrix3X2.M33 + halfLength2 * matrix3X2.M32;
		if (Math.Abs(result3.Y * matrix3X.M11 - result3.X * matrix3X.M21) > num2 + num)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M22 + halfHeight * matrix3X2.M12;
		num = halfWidth2 * matrix3X2.M33 + halfLength2 * matrix3X2.M31;
		if (Math.Abs(result3.Y * matrix3X.M12 - result3.X * matrix3X.M22) > num2 + num)
		{
			return false;
		}
		num2 = halfWidth * matrix3X2.M23 + halfHeight * matrix3X2.M13;
		num = halfWidth2 * matrix3X2.M32 + halfHeight2 * matrix3X2.M31;
		if (Math.Abs(result3.Y * matrix3X.M13 - result3.X * matrix3X.M23) > num2 + num)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Determines if the two boxes are colliding.
	/// </summary>
	/// <param name="a">First box to collide.</param>
	/// <param name="b">Second box to collide.</param>
	/// <param name="separationDistance">Distance of separation.</param>
	/// <param name="separatingAxis">Axis of separation.</param>
	/// <param name="transformA">Transform to apply to shape A.</param>
	/// <param name="transformB">Transform to apply to shape B.</param>
	/// <returns>Whether or not the boxes collide.</returns>
	public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB, out float separationDistance, out Vector3 separatingAxis)
	{
		float halfWidth = a.HalfWidth;
		float halfHeight = a.HalfHeight;
		float halfLength = a.HalfLength;
		float halfWidth2 = b.HalfWidth;
		float halfHeight2 = b.HalfHeight;
		float halfLength2 = b.HalfLength;
		Matrix3X3.CreateFromQuaternion(ref transformA.Orientation, out var result);
		Matrix3X3.CreateFromQuaternion(ref transformB.Orientation, out var result2);
		Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
		Matrix3X3 matrix3X = default(Matrix3X3);
		matrix3X.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
		matrix3X.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
		matrix3X.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
		Matrix3X3 matrix3X2 = default(Matrix3X3);
		matrix3X2.M11 = Math.Abs(matrix3X.M11) + 1E-07f;
		matrix3X2.M12 = Math.Abs(matrix3X.M12) + 1E-07f;
		matrix3X2.M13 = Math.Abs(matrix3X.M13) + 1E-07f;
		float x = result3.X;
		result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
		float num = halfWidth + halfWidth2 * matrix3X2.M11 + halfHeight2 * matrix3X2.M12 + halfLength2 * matrix3X2.M13;
		if (result3.X > num)
		{
			separationDistance = result3.X - num;
			separatingAxis = new Vector3(result.M11, result.M12, result.M13);
			return false;
		}
		if (result3.X < 0f - num)
		{
			separationDistance = 0f - result3.X - num;
			separatingAxis = new Vector3(0f - result.M11, 0f - result.M12, 0f - result.M13);
			return false;
		}
		matrix3X.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
		matrix3X.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
		matrix3X.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
		matrix3X2.M21 = Math.Abs(matrix3X.M21) + 1E-07f;
		matrix3X2.M22 = Math.Abs(matrix3X.M22) + 1E-07f;
		matrix3X2.M23 = Math.Abs(matrix3X.M23) + 1E-07f;
		float y = result3.Y;
		result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
		num = halfHeight + halfWidth2 * matrix3X2.M21 + halfHeight2 * matrix3X2.M22 + halfLength2 * matrix3X2.M23;
		if (result3.Y > num)
		{
			separationDistance = result3.Y - num;
			separatingAxis = new Vector3(result.M21, result.M22, result.M23);
			return false;
		}
		if (result3.Y < 0f - num)
		{
			separationDistance = 0f - result3.Y - num;
			separatingAxis = new Vector3(0f - result.M21, 0f - result.M22, 0f - result.M23);
			return false;
		}
		matrix3X.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
		matrix3X.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
		matrix3X.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
		matrix3X2.M31 = Math.Abs(matrix3X.M31) + 1E-07f;
		matrix3X2.M32 = Math.Abs(matrix3X.M32) + 1E-07f;
		matrix3X2.M33 = Math.Abs(matrix3X.M33) + 1E-07f;
		result3.Z = x * result.M31 + y * result.M32 + result3.Z * result.M33;
		num = halfLength + halfWidth2 * matrix3X2.M31 + halfHeight2 * matrix3X2.M32 + halfLength2 * matrix3X2.M33;
		if (result3.Z > num)
		{
			separationDistance = result3.Z - num;
			separatingAxis = new Vector3(result.M31, result.M32, result.M33);
			return false;
		}
		if (result3.Z < 0f - num)
		{
			separationDistance = 0f - result3.Z - num;
			separatingAxis = new Vector3(0f - result.M31, 0f - result.M32, 0f - result.M33);
			return false;
		}
		num = halfWidth2 + halfWidth * matrix3X2.M11 + halfHeight * matrix3X2.M21 + halfLength * matrix3X2.M31;
		float num2 = result3.X * matrix3X.M11 + result3.Y * matrix3X.M21 + result3.Z * matrix3X.M31;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result2.M11, result2.M12, result2.M13);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
			return false;
		}
		num = halfHeight2 + halfWidth * matrix3X2.M12 + halfHeight * matrix3X2.M22 + halfLength * matrix3X2.M32;
		num2 = result3.X * matrix3X.M12 + result3.Y * matrix3X.M22 + result3.Z * matrix3X.M32;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result2.M21, result2.M22, result2.M23);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
			return false;
		}
		num = halfLength2 + halfWidth * matrix3X2.M13 + halfHeight * matrix3X2.M23 + halfLength * matrix3X2.M33;
		num2 = result3.X * matrix3X.M13 + result3.Y * matrix3X.M23 + result3.Z * matrix3X.M33;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result2.M31, result2.M32, result2.M33);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
			return false;
		}
		num = halfHeight * matrix3X2.M31 + halfLength * matrix3X2.M21 + halfHeight2 * matrix3X2.M13 + halfLength2 * matrix3X2.M12;
		num2 = result3.Z * matrix3X.M21 - result3.Y * matrix3X.M31;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
			return false;
		}
		num = halfHeight * matrix3X2.M32 + halfLength * matrix3X2.M22 + halfWidth2 * matrix3X2.M13 + halfLength2 * matrix3X2.M11;
		num2 = result3.Z * matrix3X.M22 - result3.Y * matrix3X.M32;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
			return false;
		}
		num = halfHeight * matrix3X2.M33 + halfLength * matrix3X2.M23 + halfWidth2 * matrix3X2.M12 + halfHeight2 * matrix3X2.M11;
		num2 = result3.Z * matrix3X.M23 - result3.Y * matrix3X.M33;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
			return false;
		}
		num = halfWidth * matrix3X2.M31 + halfLength * matrix3X2.M11 + halfHeight2 * matrix3X2.M23 + halfLength2 * matrix3X2.M22;
		num2 = result3.X * matrix3X.M31 - result3.Z * matrix3X.M11;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
			return false;
		}
		num = halfWidth * matrix3X2.M32 + halfLength * matrix3X2.M12 + halfWidth2 * matrix3X2.M23 + halfLength2 * matrix3X2.M21;
		num2 = result3.X * matrix3X.M32 - result3.Z * matrix3X.M12;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
			return false;
		}
		num = halfWidth * matrix3X2.M33 + halfLength * matrix3X2.M13 + halfWidth2 * matrix3X2.M22 + halfHeight2 * matrix3X2.M21;
		num2 = result3.X * matrix3X.M33 - result3.Z * matrix3X.M13;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
			return false;
		}
		num = halfWidth * matrix3X2.M21 + halfHeight * matrix3X2.M11 + halfHeight2 * matrix3X2.M33 + halfLength2 * matrix3X2.M32;
		num2 = result3.Y * matrix3X.M11 - result3.X * matrix3X.M21;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
			return false;
		}
		num = halfWidth * matrix3X2.M22 + halfHeight * matrix3X2.M12 + halfWidth2 * matrix3X2.M33 + halfLength2 * matrix3X2.M31;
		num2 = result3.Y * matrix3X.M12 - result3.X * matrix3X.M22;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
			return false;
		}
		num = halfWidth * matrix3X2.M23 + halfHeight * matrix3X2.M13 + halfWidth2 * matrix3X2.M32 + halfHeight2 * matrix3X2.M31;
		num2 = result3.Y * matrix3X.M13 - result3.X * matrix3X.M23;
		if (num2 > num)
		{
			separationDistance = num2 - num;
			separatingAxis = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
			return false;
		}
		if (num2 < 0f - num)
		{
			separationDistance = 0f - num2 - num;
			separatingAxis = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
			return false;
		}
		separationDistance = 0f;
		separatingAxis = Vector3.Zero;
		return true;
	}

	/// <summary>
	/// Determines if the two boxes are colliding, including penetration depth data.
	/// </summary>
	/// <param name="a">First box to collide.</param>
	/// <param name="b">Second box to collide.</param>
	/// <param name="distance">Distance of separation or penetration.</param>
	/// <param name="axis">Axis of separation or penetration.</param>
	/// <param name="transformA">Transform to apply to shape A.</param>
	/// <param name="transformB">Transform to apply to shape B.</param>
	/// <returns>Whether or not the boxes collide.</returns>
	public static bool AreBoxesCollidingWithPenetration(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB, out float distance, out Vector3 axis)
	{
		float halfWidth = a.HalfWidth;
		float halfHeight = a.HalfHeight;
		float halfLength = a.HalfLength;
		float halfWidth2 = b.HalfWidth;
		float halfHeight2 = b.HalfHeight;
		float halfLength2 = b.HalfLength;
		Matrix3X3.CreateFromQuaternion(ref transformA.Orientation, out var result);
		Matrix3X3.CreateFromQuaternion(ref transformB.Orientation, out var result2);
		Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
		float num = float.MinValue;
		Vector3 vector = default(Vector3);
		Matrix3X3 matrix3X = default(Matrix3X3);
		matrix3X.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
		matrix3X.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
		matrix3X.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
		Matrix3X3 matrix3X2 = default(Matrix3X3);
		matrix3X2.M11 = Math.Abs(matrix3X.M11) + 1E-07f;
		matrix3X2.M12 = Math.Abs(matrix3X.M12) + 1E-07f;
		matrix3X2.M13 = Math.Abs(matrix3X.M13) + 1E-07f;
		float x = result3.X;
		result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
		float num2 = halfWidth + halfWidth2 * matrix3X2.M11 + halfHeight2 * matrix3X2.M12 + halfLength2 * matrix3X2.M13;
		if (result3.X > num2)
		{
			distance = result3.X - num2;
			axis = new Vector3(result.M11, result.M12, result.M13);
			return false;
		}
		if (result3.X < 0f - num2)
		{
			distance = 0f - result3.X - num2;
			axis = new Vector3(0f - result.M11, 0f - result.M12, 0f - result.M13);
			return false;
		}
		if (result3.X > 0f)
		{
			float num3 = result3.X - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(result.M11, result.M12, result.M13);
			}
		}
		else
		{
			float num3 = 0f - result3.X - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(0f - result.M11, 0f - result.M12, 0f - result.M13);
			}
		}
		matrix3X.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
		matrix3X.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
		matrix3X.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
		matrix3X2.M21 = Math.Abs(matrix3X.M21) + 1E-07f;
		matrix3X2.M22 = Math.Abs(matrix3X.M22) + 1E-07f;
		matrix3X2.M23 = Math.Abs(matrix3X.M23) + 1E-07f;
		float y = result3.Y;
		result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
		num2 = halfHeight + halfWidth2 * matrix3X2.M21 + halfHeight2 * matrix3X2.M22 + halfLength2 * matrix3X2.M23;
		if (result3.Y > num2)
		{
			distance = result3.Y - num2;
			axis = new Vector3(result.M21, result.M22, result.M23);
			return false;
		}
		if (result3.Y < 0f - num2)
		{
			distance = 0f - result3.Y - num2;
			axis = new Vector3(0f - result.M21, 0f - result.M22, 0f - result.M23);
			return false;
		}
		if (result3.Y > 0f)
		{
			float num3 = result3.Y - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(result.M21, result.M22, result.M23);
			}
		}
		else
		{
			float num3 = 0f - result3.Y - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(0f - result.M21, 0f - result.M22, 0f - result.M23);
			}
		}
		matrix3X.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
		matrix3X.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
		matrix3X.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
		matrix3X2.M31 = Math.Abs(matrix3X.M31) + 1E-07f;
		matrix3X2.M32 = Math.Abs(matrix3X.M32) + 1E-07f;
		matrix3X2.M33 = Math.Abs(matrix3X.M33) + 1E-07f;
		result3.Z = x * result.M31 + y * result.M32 + result3.Z * result.M33;
		num2 = halfLength + halfWidth2 * matrix3X2.M31 + halfHeight2 * matrix3X2.M32 + halfLength2 * matrix3X2.M33;
		if (result3.Z > num2)
		{
			distance = result3.Z - num2;
			axis = new Vector3(result.M31, result.M32, result.M33);
			return false;
		}
		if (result3.Z < 0f - num2)
		{
			distance = 0f - result3.Z - num2;
			axis = new Vector3(0f - result.M31, 0f - result.M32, 0f - result.M33);
			return false;
		}
		if (result3.Z > 0f)
		{
			float num3 = result3.Z - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(result.M31, result.M32, result.M33);
			}
		}
		else
		{
			float num3 = 0f - result3.Z - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(0f - result.M31, 0f - result.M32, 0f - result.M33);
			}
		}
		num2 = halfWidth2 + halfWidth * matrix3X2.M11 + halfHeight * matrix3X2.M21 + halfLength * matrix3X2.M31;
		float num4 = result3.X * matrix3X.M11 + result3.Y * matrix3X.M21 + result3.Z * matrix3X.M31;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M11, result2.M12, result2.M13);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
			return false;
		}
		if (num4 > 0f)
		{
			float num3 = num4 - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(result2.M11, result2.M12, result2.M13);
			}
		}
		else
		{
			float num3 = 0f - num4 - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
			}
		}
		num2 = halfHeight2 + halfWidth * matrix3X2.M12 + halfHeight * matrix3X2.M22 + halfLength * matrix3X2.M32;
		num4 = result3.X * matrix3X.M12 + result3.Y * matrix3X.M22 + result3.Z * matrix3X.M32;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M21, result2.M22, result2.M23);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
			return false;
		}
		if (num4 > 0f)
		{
			float num3 = num4 - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(result2.M21, result2.M22, result2.M23);
			}
		}
		else
		{
			float num3 = 0f - num4 - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
			}
		}
		num2 = halfLength2 + halfWidth * matrix3X2.M13 + halfHeight * matrix3X2.M23 + halfLength * matrix3X2.M33;
		num4 = result3.X * matrix3X.M13 + result3.Y * matrix3X.M23 + result3.Z * matrix3X.M33;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M31, result2.M32, result2.M33);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
			return false;
		}
		if (num4 > 0f)
		{
			float num3 = num4 - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(result2.M31, result2.M32, result2.M33);
			}
		}
		else
		{
			float num3 = 0f - num4 - num2;
			if (num3 > num)
			{
				num = num3;
				vector = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
			}
		}
		num2 = halfHeight * matrix3X2.M31 + halfLength * matrix3X2.M21 + halfHeight2 * matrix3X2.M13 + halfLength2 * matrix3X2.M12;
		num4 = result3.Z * matrix3X.M21 - result3.Y * matrix3X.M31;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfHeight * matrix3X2.M32 + halfLength * matrix3X2.M22 + halfWidth2 * matrix3X2.M13 + halfLength2 * matrix3X2.M11;
		num4 = result3.Z * matrix3X.M22 - result3.Y * matrix3X.M32;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfHeight * matrix3X2.M33 + halfLength * matrix3X2.M23 + halfWidth2 * matrix3X2.M12 + halfHeight2 * matrix3X2.M11;
		num4 = result3.Z * matrix3X.M23 - result3.Y * matrix3X.M33;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfWidth * matrix3X2.M31 + halfLength * matrix3X2.M11 + halfHeight2 * matrix3X2.M23 + halfLength2 * matrix3X2.M22;
		num4 = result3.X * matrix3X.M31 - result3.Z * matrix3X.M11;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfWidth * matrix3X2.M32 + halfLength * matrix3X2.M12 + halfWidth2 * matrix3X2.M23 + halfLength2 * matrix3X2.M21;
		num4 = result3.X * matrix3X.M32 - result3.Z * matrix3X.M12;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfWidth * matrix3X2.M33 + halfLength * matrix3X2.M13 + halfWidth2 * matrix3X2.M22 + halfHeight2 * matrix3X2.M21;
		num4 = result3.X * matrix3X.M33 - result3.Z * matrix3X.M13;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfWidth * matrix3X2.M21 + halfHeight * matrix3X2.M11 + halfHeight2 * matrix3X2.M33 + halfLength2 * matrix3X2.M32;
		num4 = result3.Y * matrix3X.M11 - result3.X * matrix3X.M21;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfWidth * matrix3X2.M22 + halfHeight * matrix3X2.M12 + halfWidth2 * matrix3X2.M33 + halfLength2 * matrix3X2.M31;
		num4 = result3.Y * matrix3X.M12 - result3.X * matrix3X.M22;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		num2 = halfWidth * matrix3X2.M23 + halfHeight * matrix3X2.M13 + halfWidth2 * matrix3X2.M32 + halfHeight2 * matrix3X2.M31;
		num4 = result3.Y * matrix3X.M13 - result3.X * matrix3X.M23;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector2 = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
			float num5 = 1f / vector2.Length();
			float num3 = (num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		else
		{
			Vector3 vector2 = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
			float num5 = 1f / vector2.Length();
			float num3 = (0f - num4 - num2) * num5;
			if (num3 > num)
			{
				num = num3;
				vector2.X *= num5;
				vector2.Y *= num5;
				vector2.Z *= num5;
				vector = vector2;
			}
		}
		distance = num;
		axis = vector;
		return true;
	}

	/// <summary>
	/// Determines if the two boxes are colliding and computes contact data.
	/// </summary>
	/// <param name="a">First box to collide.</param>
	/// <param name="b">Second box to collide.</param>
	/// <param name="distance">Distance of separation or penetration.</param>
	/// <param name="axis">Axis of separation or penetration.</param>
	/// <param name="contactData">Computed contact data.</param>
	/// <param name="transformA">Transform to apply to shape A.</param>
	/// <param name="transformB">Transform to apply to shape B.</param>
	/// <returns>Whether or not the boxes collide.</returns>
	public unsafe static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB, out float distance, out Vector3 axis, out TinyStructList<BoxContactData> contactData)
	{
		bool result = AreBoxesColliding(a, b, ref transformA, ref transformB, out distance, out axis, out BoxContactDataCache contactData2);
		BoxContactData* ptr = &contactData2.D1;
		contactData = default(TinyStructList<BoxContactData>);
		for (int i = 0; i < contactData2.Count; i++)
		{
			contactData.Add(ref ptr[i]);
		}
		return result;
	}

	/// <summary>
	/// Determines if the two boxes are colliding and computes contact data.
	/// </summary>
	/// <param name="a">First box to collide.</param>
	/// <param name="b">Second box to collide.</param>
	/// <param name="distance">Distance of separation or penetration.</param>
	/// <param name="axis">Axis of separation or penetration.</param>
	/// <param name="contactData">Contact positions, depths, and ids.</param>
	/// <param name="transformA">Transform to apply to shape A.</param>
	/// <param name="transformB">Transform to apply to shape B.</param>
	/// <returns>Whether or not the boxes collide.</returns>
	public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB, out float distance, out Vector3 axis, out BoxContactDataCache contactData)
	{
		float halfWidth = a.HalfWidth;
		float halfHeight = a.HalfHeight;
		float halfLength = a.HalfLength;
		float halfWidth2 = b.HalfWidth;
		float halfHeight2 = b.HalfHeight;
		float halfLength2 = b.HalfLength;
		contactData = default(BoxContactDataCache);
		Matrix3X3.CreateFromQuaternion(ref transformA.Orientation, out var result);
		Matrix3X3.CreateFromQuaternion(ref transformB.Orientation, out var result2);
		Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
		float num = float.MinValue;
		Vector3 mtd = default(Vector3);
		byte b2 = 2;
		Matrix3X3 matrix3X = default(Matrix3X3);
		matrix3X.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
		matrix3X.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
		matrix3X.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
		Matrix3X3 matrix3X2 = default(Matrix3X3);
		matrix3X2.M11 = Math.Abs(matrix3X.M11) + 1E-07f;
		matrix3X2.M12 = Math.Abs(matrix3X.M12) + 1E-07f;
		matrix3X2.M13 = Math.Abs(matrix3X.M13) + 1E-07f;
		float x = result3.X;
		result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
		float num2 = halfWidth + halfWidth2 * matrix3X2.M11 + halfHeight2 * matrix3X2.M12 + halfLength2 * matrix3X2.M13;
		if (result3.X > num2)
		{
			distance = result3.X - num2;
			axis = new Vector3(0f - result.M11, 0f - result.M12, 0f - result.M13);
			return false;
		}
		if (result3.X < 0f - num2)
		{
			distance = 0f - result3.X - num2;
			axis = new Vector3(result.M11, result.M12, result.M13);
			return false;
		}
		if (result3.X > 0f)
		{
			float num3 = result3.X - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(0f - result.M11, 0f - result.M12, 0f - result.M13);
				b2 = 0;
			}
		}
		else
		{
			float num3 = 0f - result3.X - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(result.M11, result.M12, result.M13);
				b2 = 0;
			}
		}
		matrix3X.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
		matrix3X.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
		matrix3X.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
		matrix3X2.M21 = Math.Abs(matrix3X.M21) + 1E-07f;
		matrix3X2.M22 = Math.Abs(matrix3X.M22) + 1E-07f;
		matrix3X2.M23 = Math.Abs(matrix3X.M23) + 1E-07f;
		float y = result3.Y;
		result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
		num2 = halfHeight + halfWidth2 * matrix3X2.M21 + halfHeight2 * matrix3X2.M22 + halfLength2 * matrix3X2.M23;
		if (result3.Y > num2)
		{
			distance = result3.Y - num2;
			axis = new Vector3(0f - result.M21, 0f - result.M22, 0f - result.M23);
			return false;
		}
		if (result3.Y < 0f - num2)
		{
			distance = 0f - result3.Y - num2;
			axis = new Vector3(result.M21, result.M22, result.M23);
			return false;
		}
		if (result3.Y > 0f)
		{
			float num3 = result3.Y - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(0f - result.M21, 0f - result.M22, 0f - result.M23);
				b2 = 0;
			}
		}
		else
		{
			float num3 = 0f - result3.Y - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(result.M21, result.M22, result.M23);
				b2 = 0;
			}
		}
		matrix3X.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
		matrix3X.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
		matrix3X.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
		matrix3X2.M31 = Math.Abs(matrix3X.M31) + 1E-07f;
		matrix3X2.M32 = Math.Abs(matrix3X.M32) + 1E-07f;
		matrix3X2.M33 = Math.Abs(matrix3X.M33) + 1E-07f;
		result3.Z = x * result.M31 + y * result.M32 + result3.Z * result.M33;
		num2 = halfLength + halfWidth2 * matrix3X2.M31 + halfHeight2 * matrix3X2.M32 + halfLength2 * matrix3X2.M33;
		if (result3.Z > num2)
		{
			distance = result3.Z - num2;
			axis = new Vector3(0f - result.M31, 0f - result.M32, 0f - result.M33);
			return false;
		}
		if (result3.Z < 0f - num2)
		{
			distance = 0f - result3.Z - num2;
			axis = new Vector3(result.M31, result.M32, result.M33);
			return false;
		}
		if (result3.Z > 0f)
		{
			float num3 = result3.Z - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(0f - result.M31, 0f - result.M32, 0f - result.M33);
				b2 = 0;
			}
		}
		else
		{
			float num3 = 0f - result3.Z - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(result.M31, result.M32, result.M33);
				b2 = 0;
			}
		}
		num += 0.01f;
		num2 = halfWidth2 + halfWidth * matrix3X2.M11 + halfHeight * matrix3X2.M21 + halfLength * matrix3X2.M31;
		float num4 = result3.X * matrix3X.M11 + result3.Y * matrix3X.M21 + result3.Z * matrix3X.M31;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M11, result2.M12, result2.M13);
			return false;
		}
		if (num4 > 0f)
		{
			float num3 = num4 - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
				b2 = 1;
			}
		}
		else
		{
			float num3 = 0f - num4 - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(result2.M11, result2.M12, result2.M13);
				b2 = 1;
			}
		}
		num2 = halfHeight2 + halfWidth * matrix3X2.M12 + halfHeight * matrix3X2.M22 + halfLength * matrix3X2.M32;
		num4 = result3.X * matrix3X.M12 + result3.Y * matrix3X.M22 + result3.Z * matrix3X.M32;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M21, result2.M22, result2.M23);
			return false;
		}
		if (num4 > 0f)
		{
			float num3 = num4 - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
				b2 = 1;
			}
		}
		else
		{
			float num3 = 0f - num4 - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(result2.M21, result2.M22, result2.M23);
				b2 = 1;
			}
		}
		num2 = halfLength2 + halfWidth * matrix3X2.M13 + halfHeight * matrix3X2.M23 + halfLength * matrix3X2.M33;
		num4 = result3.X * matrix3X.M13 + result3.Y * matrix3X.M23 + result3.Z * matrix3X.M33;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result2.M31, result2.M32, result2.M33);
			return false;
		}
		if (num4 > 0f)
		{
			float num3 = num4 - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
				b2 = 1;
			}
		}
		else
		{
			float num3 = 0f - num4 - num2;
			if (num3 > num)
			{
				num = num3;
				mtd = new Vector3(result2.M31, result2.M32, result2.M33);
				b2 = 1;
			}
		}
		if (b2 != 1)
		{
			num -= 0.01f;
		}
		float num5 = 0.01f;
		num += num5;
		num2 = halfHeight * matrix3X2.M31 + halfLength * matrix3X2.M21 + halfHeight2 * matrix3X2.M13 + halfLength2 * matrix3X2.M12;
		num4 = result3.Z * matrix3X.M21 - result3.Y * matrix3X.M31;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfHeight * matrix3X2.M32 + halfLength * matrix3X2.M22 + halfWidth2 * matrix3X2.M13 + halfLength2 * matrix3X2.M11;
		num4 = result3.Z * matrix3X.M22 - result3.Y * matrix3X.M32;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfHeight * matrix3X2.M33 + halfLength * matrix3X2.M23 + halfWidth2 * matrix3X2.M12 + halfHeight2 * matrix3X2.M11;
		num4 = result3.Z * matrix3X.M23 - result3.Y * matrix3X.M33;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfWidth * matrix3X2.M31 + halfLength * matrix3X2.M11 + halfHeight2 * matrix3X2.M23 + halfLength2 * matrix3X2.M22;
		num4 = result3.X * matrix3X.M31 - result3.Z * matrix3X.M11;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfWidth * matrix3X2.M32 + halfLength * matrix3X2.M12 + halfWidth2 * matrix3X2.M23 + halfLength2 * matrix3X2.M21;
		num4 = result3.X * matrix3X.M32 - result3.Z * matrix3X.M12;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfWidth * matrix3X2.M33 + halfLength * matrix3X2.M13 + halfWidth2 * matrix3X2.M22 + halfHeight2 * matrix3X2.M21;
		num4 = result3.X * matrix3X.M33 - result3.Z * matrix3X.M13;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfWidth * matrix3X2.M21 + halfHeight * matrix3X2.M11 + halfHeight2 * matrix3X2.M33 + halfLength2 * matrix3X2.M32;
		num4 = result3.Y * matrix3X.M11 - result3.X * matrix3X.M21;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfWidth * matrix3X2.M22 + halfHeight * matrix3X2.M12 + halfWidth2 * matrix3X2.M33 + halfLength2 * matrix3X2.M31;
		num4 = result3.Y * matrix3X.M12 - result3.X * matrix3X.M22;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		num2 = halfWidth * matrix3X2.M23 + halfHeight * matrix3X2.M13 + halfWidth2 * matrix3X2.M32 + halfHeight2 * matrix3X2.M31;
		num4 = result3.Y * matrix3X.M13 - result3.X * matrix3X.M23;
		if (num4 > num2)
		{
			distance = num4 - num2;
			axis = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
			return false;
		}
		if (num4 < 0f - num2)
		{
			distance = 0f - num4 - num2;
			axis = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
			return false;
		}
		if (num4 > 0f)
		{
			Vector3 vector = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
			float num6 = 1f / vector.Length();
			float num3 = (num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		else
		{
			Vector3 vector = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
			float num6 = 1f / vector.Length();
			float num3 = (0f - num4 - num2) * num6;
			if (num3 > num)
			{
				b2 = 2;
				num = num3;
				vector.X *= num6;
				vector.Y *= num6;
				vector.Z *= num6;
				mtd = vector;
			}
		}
		if (b2 == 2)
		{
			GetEdgeEdgeContact(a, b, ref transformA.Position, ref result, ref transformB.Position, ref result2, num, ref mtd, out contactData);
		}
		else
		{
			num -= num5;
			GetFaceContacts(a, b, ref transformA.Position, ref result, ref transformB.Position, ref result2, b2 == 0, ref mtd, out contactData);
		}
		distance = num;
		axis = mtd;
		return true;
	}

	internal unsafe static void GetEdgeEdgeContact(BoxShape a, BoxShape b, ref Vector3 positionA, ref Matrix3X3 orientationA, ref Vector3 positionB, ref Matrix3X3 orientationB, float depth, ref Vector3 mtd, out BoxContactDataCache contactData)
	{
		Vector3.Negate(ref mtd, out var result);
		Matrix3X3.TransformTranspose(ref result, ref orientationA, out var result2);
		Matrix3X3.TransformTranspose(ref mtd, ref orientationB, out var result3);
		Vector3 edgeStart = default(Vector3);
		Vector3 edgeEnd = default(Vector3);
		Vector3 edgeStart2 = default(Vector3);
		Vector3 edgeEnd2 = default(Vector3);
		Vector3 edgeStart3 = default(Vector3);
		Vector3 edgeEnd3 = default(Vector3);
		Vector3 edgeStart4 = default(Vector3);
		Vector3 edgeEnd4 = default(Vector3);
		float halfWidth = a.halfWidth;
		float halfHeight = a.halfHeight;
		float halfLength = a.halfLength;
		float halfWidth2 = b.halfWidth;
		float halfHeight2 = b.halfHeight;
		float halfLength2 = b.halfLength;
		int edgeStartId;
		int edgeEndId;
		int edgeStartId2;
		int edgeEndId2;
		if (Math.Abs(result2.X) < 1E-07f)
		{
			TinyList<float> dots = default(TinyList<float>);
			dots.Add((0f - halfHeight) * result2.Y - halfLength * result2.Z);
			dots.Add((0f - halfHeight) * result2.Y + halfLength * result2.Z);
			dots.Add(halfHeight * result2.Y - halfLength * result2.Z);
			dots.Add(halfHeight * result2.Y + halfLength * result2.Z);
			FindHighestIndices(ref dots, out var highestIndex, out var secondHighestIndex);
			GetEdgeData(highestIndex, 0, halfWidth, halfHeight, halfLength, out edgeStart, out edgeEnd, out edgeStartId, out edgeEndId);
			GetEdgeData(secondHighestIndex, 0, halfWidth, halfHeight, halfLength, out edgeStart2, out edgeEnd2, out edgeStartId2, out edgeEndId2);
		}
		else if (Math.Abs(result2.Y) < 1E-07f)
		{
			TinyList<float> dots2 = default(TinyList<float>);
			dots2.Add((0f - halfWidth) * result2.X - halfLength * result2.Z);
			dots2.Add((0f - halfWidth) * result2.X + halfLength * result2.Z);
			dots2.Add(halfWidth * result2.X - halfLength * result2.Z);
			dots2.Add(halfWidth * result2.X + halfLength * result2.Z);
			FindHighestIndices(ref dots2, out var highestIndex2, out var secondHighestIndex2);
			GetEdgeData(highestIndex2, 1, halfWidth, halfHeight, halfLength, out edgeStart, out edgeEnd, out edgeStartId, out edgeEndId);
			GetEdgeData(secondHighestIndex2, 1, halfWidth, halfHeight, halfLength, out edgeStart2, out edgeEnd2, out edgeStartId2, out edgeEndId2);
		}
		else
		{
			TinyList<float> dots3 = default(TinyList<float>);
			dots3.Add((0f - halfWidth) * result2.X - halfHeight * result2.Y);
			dots3.Add((0f - halfWidth) * result2.X + halfHeight * result2.Y);
			dots3.Add(halfWidth * result2.X - halfHeight * result2.Y);
			dots3.Add(halfWidth * result2.X + halfHeight * result2.Y);
			FindHighestIndices(ref dots3, out var highestIndex3, out var secondHighestIndex3);
			GetEdgeData(highestIndex3, 2, halfWidth, halfHeight, halfLength, out edgeStart, out edgeEnd, out edgeStartId, out edgeEndId);
			GetEdgeData(secondHighestIndex3, 2, halfWidth, halfHeight, halfLength, out edgeStart2, out edgeEnd2, out edgeStartId2, out edgeEndId2);
		}
		int edgeStartId3;
		int edgeEndId3;
		int edgeStartId4;
		int edgeEndId4;
		if (Math.Abs(result3.X) < 1E-07f)
		{
			TinyList<float> dots4 = default(TinyList<float>);
			dots4.Add((0f - halfHeight2) * result3.Y - halfLength2 * result3.Z);
			dots4.Add((0f - halfHeight2) * result3.Y + halfLength2 * result3.Z);
			dots4.Add(halfHeight2 * result3.Y - halfLength2 * result3.Z);
			dots4.Add(halfHeight2 * result3.Y + halfLength2 * result3.Z);
			FindHighestIndices(ref dots4, out var highestIndex4, out var secondHighestIndex4);
			GetEdgeData(highestIndex4, 0, halfWidth2, halfHeight2, halfLength2, out edgeStart3, out edgeEnd3, out edgeStartId3, out edgeEndId3);
			GetEdgeData(secondHighestIndex4, 0, halfWidth2, halfHeight2, halfLength2, out edgeStart4, out edgeEnd4, out edgeStartId4, out edgeEndId4);
		}
		else if (Math.Abs(result3.Y) < 1E-07f)
		{
			TinyList<float> dots5 = default(TinyList<float>);
			dots5.Add((0f - halfWidth2) * result3.X - halfLength2 * result3.Z);
			dots5.Add((0f - halfWidth2) * result3.X + halfLength2 * result3.Z);
			dots5.Add(halfWidth2 * result3.X - halfLength2 * result3.Z);
			dots5.Add(halfWidth2 * result3.X + halfLength2 * result3.Z);
			FindHighestIndices(ref dots5, out var highestIndex5, out var secondHighestIndex5);
			GetEdgeData(highestIndex5, 1, halfWidth2, halfHeight2, halfLength2, out edgeStart3, out edgeEnd3, out edgeStartId3, out edgeEndId3);
			GetEdgeData(secondHighestIndex5, 1, halfWidth2, halfHeight2, halfLength2, out edgeStart4, out edgeEnd4, out edgeStartId4, out edgeEndId4);
		}
		else
		{
			TinyList<float> dots6 = default(TinyList<float>);
			dots6.Add((0f - halfWidth2) * result3.X - halfHeight2 * result3.Y);
			dots6.Add((0f - halfWidth2) * result3.X + halfHeight2 * result3.Y);
			dots6.Add(halfWidth2 * result3.X - halfHeight2 * result3.Y);
			dots6.Add(halfWidth2 * result3.X + halfHeight2 * result3.Y);
			FindHighestIndices(ref dots6, out var highestIndex6, out var secondHighestIndex6);
			GetEdgeData(highestIndex6, 2, halfWidth2, halfHeight2, halfLength2, out edgeStart3, out edgeEnd3, out edgeStartId3, out edgeEndId3);
			GetEdgeData(secondHighestIndex6, 2, halfWidth2, halfHeight2, halfLength2, out edgeStart4, out edgeEnd4, out edgeStartId4, out edgeEndId4);
		}
		Matrix3X3.Transform(ref edgeStart, ref orientationA, out edgeStart);
		Matrix3X3.Transform(ref edgeEnd, ref orientationA, out edgeEnd);
		Matrix3X3.Transform(ref edgeStart3, ref orientationB, out edgeStart3);
		Matrix3X3.Transform(ref edgeEnd3, ref orientationB, out edgeEnd3);
		Matrix3X3.Transform(ref edgeStart2, ref orientationA, out edgeStart2);
		Matrix3X3.Transform(ref edgeEnd2, ref orientationA, out edgeEnd2);
		Matrix3X3.Transform(ref edgeStart4, ref orientationB, out edgeStart4);
		Matrix3X3.Transform(ref edgeEnd4, ref orientationB, out edgeEnd4);
		Vector3.Add(ref edgeStart, ref positionA, out edgeStart);
		Vector3.Add(ref edgeEnd, ref positionA, out edgeEnd);
		Vector3.Add(ref edgeStart3, ref positionB, out edgeStart3);
		Vector3.Add(ref edgeEnd3, ref positionB, out edgeEnd3);
		Vector3.Add(ref edgeStart2, ref positionA, out edgeStart2);
		Vector3.Add(ref edgeEnd2, ref positionA, out edgeEnd2);
		Vector3.Add(ref edgeStart4, ref positionB, out edgeStart4);
		Vector3.Add(ref edgeEnd4, ref positionB, out edgeEnd4);
		BoxContactDataCache boxContactDataCache = default(BoxContactDataCache);
		BoxContactData* ptr = &boxContactDataCache.D1;
		Vector3 result4;
		float result5;
		if (GetClosestPointsBetweenSegments(ref edgeStart, ref edgeEnd, ref edgeStart3, ref edgeEnd3, out var c, out var c2))
		{
			Vector3.Subtract(ref c, ref c2, out result4);
			Vector3.Dot(ref result4, ref mtd, out result5);
			if (result5 < 0f)
			{
				BoxContactData boxContactData = default(BoxContactData);
				boxContactData.Position = c;
				boxContactData.Depth = result5;
				boxContactData.Id = GetContactId(edgeStartId, edgeEndId, edgeStartId3, edgeEndId3);
				ptr[(int)boxContactDataCache.Count] = boxContactData;
				boxContactDataCache.Count++;
			}
		}
		if (GetClosestPointsBetweenSegments(ref edgeStart, ref edgeEnd, ref edgeStart4, ref edgeEnd4, out c, out c2))
		{
			Vector3.Subtract(ref c, ref c2, out result4);
			Vector3.Dot(ref result4, ref mtd, out result5);
			if (result5 < 0f)
			{
				BoxContactData boxContactData2 = default(BoxContactData);
				boxContactData2.Position = c;
				boxContactData2.Depth = result5;
				boxContactData2.Id = GetContactId(edgeStartId, edgeEndId, edgeStartId4, edgeEndId4);
				ptr[(int)boxContactDataCache.Count] = boxContactData2;
				boxContactDataCache.Count++;
			}
		}
		if (GetClosestPointsBetweenSegments(ref edgeStart2, ref edgeEnd2, ref edgeStart3, ref edgeEnd3, out c, out c2))
		{
			Vector3.Subtract(ref c, ref c2, out result4);
			Vector3.Dot(ref result4, ref mtd, out result5);
			if (result5 < 0f)
			{
				BoxContactData boxContactData3 = default(BoxContactData);
				boxContactData3.Position = c;
				boxContactData3.Depth = result5;
				boxContactData3.Id = GetContactId(edgeStartId2, edgeEndId2, edgeStartId3, edgeEndId3);
				ptr[(int)boxContactDataCache.Count] = boxContactData3;
				boxContactDataCache.Count++;
			}
		}
		if (GetClosestPointsBetweenSegments(ref edgeStart2, ref edgeEnd2, ref edgeStart4, ref edgeEnd4, out c, out c2))
		{
			Vector3.Subtract(ref c, ref c2, out result4);
			Vector3.Dot(ref result4, ref mtd, out result5);
			if (result5 < 0f)
			{
				BoxContactData boxContactData4 = default(BoxContactData);
				boxContactData4.Position = c;
				boxContactData4.Depth = result5;
				boxContactData4.Id = GetContactId(edgeStartId2, edgeEndId2, edgeStartId4, edgeEndId4);
				ptr[(int)boxContactDataCache.Count] = boxContactData4;
				boxContactDataCache.Count++;
			}
		}
		contactData = boxContactDataCache;
	}

	private static void GetEdgeData(int index, int axis, float x, float y, float z, out Vector3 edgeStart, out Vector3 edgeEnd, out int edgeStartId, out int edgeEndId)
	{
		edgeStart = default(Vector3);
		edgeEnd = default(Vector3);
		switch (index + axis * 4)
		{
		case 0:
			edgeStart.X = 0f - x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = 0f - z;
			edgeStartId = 0;
			edgeEnd.X = x;
			edgeEnd.Y = 0f - y;
			edgeEnd.Z = 0f - z;
			edgeEndId = 4;
			break;
		case 1:
			edgeStart.X = 0f - x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = z;
			edgeStartId = 1;
			edgeEnd.X = x;
			edgeEnd.Y = 0f - y;
			edgeEnd.Z = z;
			edgeEndId = 5;
			break;
		case 2:
			edgeStart.X = 0f - x;
			edgeStart.Y = y;
			edgeStart.Z = 0f - z;
			edgeStartId = 2;
			edgeEnd.X = x;
			edgeEnd.Y = y;
			edgeEnd.Z = 0f - z;
			edgeEndId = 6;
			break;
		case 3:
			edgeStart.X = 0f - x;
			edgeStart.Y = y;
			edgeStart.Z = z;
			edgeStartId = 3;
			edgeEnd.X = x;
			edgeEnd.Y = y;
			edgeEnd.Z = z;
			edgeEndId = 7;
			break;
		case 4:
			edgeStart.X = 0f - x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = 0f - z;
			edgeStartId = 0;
			edgeEnd.X = 0f - x;
			edgeEnd.Y = y;
			edgeEnd.Z = 0f - z;
			edgeEndId = 2;
			break;
		case 5:
			edgeStart.X = 0f - x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = z;
			edgeStartId = 1;
			edgeEnd.X = 0f - x;
			edgeEnd.Y = y;
			edgeEnd.Z = z;
			edgeEndId = 3;
			break;
		case 6:
			edgeStart.X = x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = 0f - z;
			edgeStartId = 4;
			edgeEnd.X = x;
			edgeEnd.Y = y;
			edgeEnd.Z = 0f - z;
			edgeEndId = 6;
			break;
		case 7:
			edgeStart.X = x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = z;
			edgeStartId = 5;
			edgeEnd.X = x;
			edgeEnd.Y = y;
			edgeEnd.Z = z;
			edgeEndId = 7;
			break;
		case 8:
			edgeStart.X = 0f - x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = 0f - z;
			edgeStartId = 0;
			edgeEnd.X = 0f - x;
			edgeEnd.Y = 0f - y;
			edgeEnd.Z = z;
			edgeEndId = 1;
			break;
		case 9:
			edgeStart.X = 0f - x;
			edgeStart.Y = y;
			edgeStart.Z = 0f - z;
			edgeStartId = 2;
			edgeEnd.X = 0f - x;
			edgeEnd.Y = y;
			edgeEnd.Z = z;
			edgeEndId = 3;
			break;
		case 10:
			edgeStart.X = x;
			edgeStart.Y = 0f - y;
			edgeStart.Z = 0f - z;
			edgeStartId = 4;
			edgeEnd.X = x;
			edgeEnd.Y = 0f - y;
			edgeEnd.Z = z;
			edgeEndId = 5;
			break;
		case 11:
			edgeStart.X = x;
			edgeStart.Y = y;
			edgeStart.Z = 0f - z;
			edgeStartId = 6;
			edgeEnd.X = x;
			edgeEnd.Y = y;
			edgeEnd.Z = z;
			edgeEndId = 7;
			break;
		default:
			throw new Exception("Invalid index or axis.");
		}
	}

	private static void FindHighestIndices(ref TinyList<float> dots, out int highestIndex, out int secondHighestIndex)
	{
		highestIndex = 0;
		float num = dots[0];
		for (int i = 1; i < 4; i++)
		{
			float num2 = dots[i];
			if (num2 > num)
			{
				highestIndex = i;
				num = num2;
			}
		}
		secondHighestIndex = 0;
		float num3 = float.MinValue;
		for (int j = 0; j < 4; j++)
		{
			float num4 = dots[j];
			if (j != highestIndex && num4 > num3)
			{
				secondHighestIndex = j;
				num3 = num4;
			}
		}
	}

	/// <summary>
	/// Computes closest points c1 and c2 betwen segments p1q1 and p2q2.
	/// </summary>
	/// <param name="p1">First point of first segment.</param>
	/// <param name="q1">Second point of first segment.</param>
	/// <param name="p2">First point of second segment.</param>
	/// <param name="q2">Second point of second segment.</param>
	/// <param name="c1">Closest point on first segment.</param>
	/// <param name="c2">Closest point on second segment.</param>
	private static bool GetClosestPointsBetweenSegments(ref Vector3 p1, ref Vector3 q1, ref Vector3 p2, ref Vector3 q2, out Vector3 c1, out Vector3 c2)
	{
		Vector3.Subtract(ref q1, ref p1, out var result);
		Vector3.Subtract(ref q2, ref p2, out var result2);
		Vector3.Subtract(ref p1, ref p2, out var result3);
		float num = result.LengthSquared();
		float num2 = result2.LengthSquared();
		Vector3.Dot(ref result2, ref result3, out var result4);
		if (num <= 1E-07f && num2 <= 1E-07f)
		{
			c1 = p1;
			c2 = p2;
			return false;
		}
		float num3;
		float num4;
		if (num <= 1E-07f)
		{
			num3 = 0f;
			num4 = result4 / num2;
			if (num4 < 0f || num4 > 1f)
			{
				c1 = default(Vector3);
				c2 = default(Vector3);
				return false;
			}
		}
		else
		{
			float num5 = Vector3.Dot(result, result3);
			if (num2 <= 1E-07f)
			{
				num4 = 0f;
				num3 = MathHelper.Clamp((0f - num5) / num, 0f, 1f);
			}
			else
			{
				float num6 = Vector3.Dot(result, result2);
				float num7 = num * num2 - num6 * num6;
				if (num7 != 0f)
				{
					num3 = (num6 * result4 - num5 * num2) / num7;
					if (num3 < 0f || num3 > 1f)
					{
						c1 = default(Vector3);
						c2 = default(Vector3);
						return false;
					}
				}
				else
				{
					num3 = 0.5f;
				}
				num4 = (num6 * num3 + result4) / num2;
				if (num4 < 0f || num4 > 1f)
				{
					c1 = default(Vector3);
					c2 = default(Vector3);
					return false;
				}
			}
		}
		Vector3.Multiply(ref result, num3, out c1);
		Vector3.Add(ref c1, ref p1, out c1);
		Vector3.Multiply(ref result2, num4, out c2);
		Vector3.Add(ref c2, ref p2, out c2);
		return true;
	}

	internal static void GetFaceContacts(BoxShape a, BoxShape b, ref Vector3 positionA, ref Matrix3X3 orientationA, ref Vector3 positionB, ref Matrix3X3 orientationB, bool aIsFaceOwner, ref Vector3 mtd, out BoxContactDataCache contactData)
	{
		float halfWidth = a.halfWidth;
		float halfHeight = a.halfHeight;
		float halfLength = a.halfLength;
		float halfWidth2 = b.halfWidth;
		float halfHeight2 = b.halfHeight;
		float halfLength2 = b.halfLength;
		Vector3.Negate(ref mtd, out var result);
		GetNearestFace(ref positionA, ref orientationA, ref result, halfWidth, halfHeight, halfLength, out var boxFace);
		GetNearestFace(ref positionB, ref orientationB, ref mtd, halfWidth2, halfHeight2, halfLength2, out var boxFace2);
		if (aIsFaceOwner)
		{
			ClipFacesDirect(ref boxFace, ref boxFace2, ref result, out contactData);
		}
		else
		{
			ClipFacesDirect(ref boxFace2, ref boxFace, ref mtd, out contactData);
		}
		if (contactData.Count > 4)
		{
			PruneContactsMaxDistance(ref mtd, contactData, out contactData);
		}
	}

	private unsafe static void PruneContactsMaxDistance(ref Vector3 mtd, BoxContactDataCache input, out BoxContactDataCache output)
	{
		BoxContactData* ptr = &input.D1;
		int count = input.Count;
		float num = -1f;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			if (ptr[i].Depth > num)
			{
				num = ptr[i].Depth;
				num2 = i;
			}
		}
		float num3 = -1f;
		int num4 = 0;
		for (int j = 0; j < count; j++)
		{
			Vector3.DistanceSquared(ref ptr[num2].Position, ref ptr[j].Position, out var result);
			if (result > num3)
			{
				num3 = result;
				num4 = j;
			}
		}
		Vector3.Subtract(ref ptr[num4].Position, ref ptr[num2].Position, out var result2);
		Vector3.Cross(ref mtd, ref result2, out var result3);
		int num5 = 0;
		int num6 = 0;
		Vector3.Dot(ref ptr->Position, ref result3, out var result4);
		float num7 = result4;
		for (int k = 1; k < count; k++)
		{
			Vector3.Dot(ref result3, ref ptr[k].Position, out var result5);
			if (result5 < result4)
			{
				result4 = result5;
				num5 = k;
			}
			else if (result5 > num7)
			{
				num7 = result5;
				num6 = k;
			}
		}
		output = new BoxContactDataCache
		{
			Count = 4,
			D1 = ptr[num2],
			D2 = ptr[num4],
			D3 = ptr[num5],
			D4 = ptr[num6]
		};
	}

	private unsafe static void ClipFacesDirect(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out BoxContactDataCache outputData)
	{
		BoxContactDataCache boxContactDataCache = default(BoxContactDataCache);
		BoxContactData* ptr = &boxContactDataCache.D1;
		BoxContactDataCache boxContactDataCache2 = default(BoxContactDataCache);
		BoxContactData* ptr2 = &boxContactDataCache2.D1;
		Vector3.Subtract(ref clipFace.V4, ref clipFace.V3, out var result);
		Vector3.Subtract(ref clipFace.V2, ref clipFace.V3, out var result2);
		float num = 1f / clipFace.Width;
		float num2 = 1f / clipFace.Height;
		float num3 = num * num;
		result.X *= num3;
		result.Y *= num3;
		result.Z *= num3;
		float num4 = num2 * num2;
		result2.X *= num4;
		result2.Y *= num4;
		result2.Z *= num4;
		Vector3.Subtract(ref face.V4, ref face.V3, out var result3);
		Vector3.Subtract(ref face.V2, ref face.V3, out var result4);
		float num5 = 1f / face.Width;
		float num6 = 1f / face.Height;
		float num7 = num5 * num5;
		result3.X *= num7;
		result3.Y *= num7;
		result3.Z *= num7;
		float num8 = num6 * num6;
		result4.X *= num8;
		result4.Y *= num8;
		result4.Z *= num8;
		Vector3.Add(ref clipFace.V1, ref clipFace.V3, out var result5);
		Vector3.Dot(ref result5, ref result, out var result6);
		Vector3.Dot(ref result5, ref result2, out var result7);
		result6 *= 0.5f;
		result7 *= 0.5f;
		Vector3.Add(ref face.V1, ref face.V3, out var result8);
		Vector3.Dot(ref result8, ref result3, out var result9);
		Vector3.Dot(ref result8, ref result4, out var result10);
		result9 *= 0.5f;
		result10 *= 0.5f;
		float num9 = 0.5f + 0.01f * num;
		float num10 = 0.5f + 0.01f * num2;
		float num11 = result6 + num9;
		float num12 = result7 + num10;
		float num13 = result6 - num9;
		float num14 = result7 - num10;
		num9 = 0.5f + 0.01f * num5;
		num10 = 0.5f + 0.01f * num6;
		float num15 = result9 + num9;
		float num16 = result10 + num10;
		float num17 = result9 - num9;
		float num18 = result10 - num10;
		Vector3.Dot(ref result, ref face.V1, out var result11);
		bool flag = result11 < num11;
		bool flag2 = result11 > num13;
		Vector3.Dot(ref result2, ref face.V1, out var result12);
		bool flag3 = result12 < num12;
		bool flag4 = result12 > num14;
		Vector3.Dot(ref result, ref face.V2, out result11);
		bool flag5 = result11 < num11;
		bool flag6 = result11 > num13;
		Vector3.Dot(ref result2, ref face.V2, out result12);
		bool flag7 = result12 < num12;
		bool flag8 = result12 > num14;
		Vector3.Dot(ref result, ref face.V3, out result11);
		bool flag9 = result11 < num11;
		bool flag10 = result11 > num13;
		Vector3.Dot(ref result2, ref face.V3, out result12);
		bool flag11 = result12 < num12;
		bool flag12 = result12 > num14;
		Vector3.Dot(ref result, ref face.V4, out result11);
		bool flag13 = result11 < num11;
		bool flag14 = result11 > num13;
		Vector3.Dot(ref result2, ref face.V4, out result12);
		bool flag15 = result12 < num12;
		bool flag16 = result12 > num14;
		Vector3.Dot(ref result3, ref clipFace.V1, out result11);
		bool flag17 = result11 < num15;
		bool flag18 = result11 > num17;
		Vector3.Dot(ref result4, ref clipFace.V1, out result12);
		bool flag19 = result12 < num16;
		bool flag20 = result12 > num18;
		Vector3.Dot(ref result3, ref clipFace.V2, out result11);
		bool flag21 = result11 < num15;
		bool flag22 = result11 > num17;
		Vector3.Dot(ref result4, ref clipFace.V2, out result12);
		bool flag23 = result12 < num16;
		bool flag24 = result12 > num18;
		Vector3.Dot(ref result3, ref clipFace.V3, out result11);
		bool flag25 = result11 < num15;
		bool flag26 = result11 > num17;
		Vector3.Dot(ref result4, ref clipFace.V3, out result12);
		bool flag27 = result12 < num16;
		bool flag28 = result12 > num18;
		Vector3.Dot(ref result3, ref clipFace.V4, out result11);
		bool flag29 = result11 < num15;
		bool flag30 = result11 > num17;
		Vector3.Dot(ref result4, ref clipFace.V4, out result12);
		bool flag31 = result12 < num16;
		bool flag32 = result12 > num18;
		if (flag2 && flag && flag4 && flag3)
		{
			ptr[(int)boxContactDataCache.Count].Position = face.V1;
			ptr[(int)boxContactDataCache.Count].Id = face.Id1;
			boxContactDataCache.Count++;
		}
		if (flag6 && flag5 && flag8 && flag7)
		{
			ptr[(int)boxContactDataCache.Count].Position = face.V2;
			ptr[(int)boxContactDataCache.Count].Id = face.Id2;
			boxContactDataCache.Count++;
		}
		if (flag10 && flag9 && flag12 && flag11)
		{
			ptr[(int)boxContactDataCache.Count].Position = face.V3;
			ptr[(int)boxContactDataCache.Count].Id = face.Id3;
			boxContactDataCache.Count++;
		}
		if (flag14 && flag13 && flag16 && flag15)
		{
			ptr[(int)boxContactDataCache.Count].Position = face.V4;
			ptr[(int)boxContactDataCache.Count].Id = face.Id4;
			boxContactDataCache.Count++;
		}
		boxContactDataCache2 = boxContactDataCache;
		boxContactDataCache.Count = 0;
		Vector3.Dot(ref clipFace.V1, ref mtd, out var result13);
		float result14;
		for (int i = 0; i < boxContactDataCache2.Count; i++)
		{
			Vector3.Dot(ref ptr2[i].Position, ref mtd, out result14);
			float num19 = result14 - result13;
			if (num19 <= 0f)
			{
				ptr[(int)boxContactDataCache.Count].Position = ptr2[i].Position;
				ptr[(int)boxContactDataCache.Count].Depth = num19;
				ptr[(int)boxContactDataCache.Count].Id = ptr2[i].Id;
				boxContactDataCache.Count++;
			}
		}
		byte count = boxContactDataCache.Count;
		if (count >= 4)
		{
			outputData = boxContactDataCache;
			return;
		}
		Vector3.Dot(ref face.V1, ref face.Normal, out var result15);
		float result16;
		Vector3 result17;
		if (flag18 && flag17 && flag20 && flag19)
		{
			Vector3.Dot(ref clipFace.V1, ref face.Normal, out result16);
			Vector3.Multiply(ref face.Normal, result16 - result15, out result17);
			Vector3.Subtract(ref clipFace.V1, ref result17, out result17);
			ptr[(int)boxContactDataCache.Count].Position = result17;
			ptr[(int)boxContactDataCache.Count].Id = clipFace.Id1 + 8;
			boxContactDataCache.Count++;
		}
		if (flag22 && flag21 && flag24 && flag23)
		{
			Vector3.Dot(ref clipFace.V2, ref face.Normal, out result16);
			Vector3.Multiply(ref face.Normal, result16 - result15, out result17);
			Vector3.Subtract(ref clipFace.V2, ref result17, out result17);
			ptr[(int)boxContactDataCache.Count].Position = result17;
			ptr[(int)boxContactDataCache.Count].Id = clipFace.Id2 + 8;
			boxContactDataCache.Count++;
		}
		if (flag26 && flag25 && flag28 && flag27)
		{
			Vector3.Dot(ref clipFace.V3, ref face.Normal, out result16);
			Vector3.Multiply(ref face.Normal, result16 - result15, out result17);
			Vector3.Subtract(ref clipFace.V3, ref result17, out result17);
			ptr[(int)boxContactDataCache.Count].Position = result17;
			ptr[(int)boxContactDataCache.Count].Id = clipFace.Id3 + 8;
			boxContactDataCache.Count++;
		}
		if (flag30 && flag29 && flag32 && flag31)
		{
			Vector3.Dot(ref clipFace.V4, ref face.Normal, out result16);
			Vector3.Multiply(ref face.Normal, result16 - result15, out result17);
			Vector3.Subtract(ref clipFace.V4, ref result17, out result17);
			ptr[(int)boxContactDataCache.Count].Position = result17;
			ptr[(int)boxContactDataCache.Count].Id = clipFace.Id4 + 8;
			boxContactDataCache.Count++;
		}
		boxContactDataCache2 = boxContactDataCache;
		boxContactDataCache.Count = count;
		for (int j = count; j < boxContactDataCache2.Count; j++)
		{
			Vector3.Dot(ref ptr2[j].Position, ref mtd, out result14);
			float num19 = result14 - result13;
			if (num19 <= 0f)
			{
				ptr[(int)boxContactDataCache.Count].Position = ptr2[j].Position;
				ptr[(int)boxContactDataCache.Count].Depth = num19;
				ptr[(int)boxContactDataCache.Count].Id = ptr2[j].Id;
				boxContactDataCache.Count++;
			}
		}
		count = boxContactDataCache.Count;
		if (count >= 4)
		{
			outputData = boxContactDataCache;
			return;
		}
		clipFace.GetEdge(0, out var clippingEdge);
		if (!flag3)
		{
			if (flag7 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag15 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag7)
		{
			if (flag3 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag11 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag11)
		{
			if (flag7 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag15 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag15)
		{
			if (flag3 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag11 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		clipFace.GetEdge(1, out clippingEdge);
		if (!flag2)
		{
			if (flag6 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag14 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag6)
		{
			if (flag2 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag10 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag10)
		{
			if (flag6 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag14 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag14)
		{
			if (flag2 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag10 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		clipFace.GetEdge(2, out clippingEdge);
		if (!flag4)
		{
			if (flag8 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag16 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag8)
		{
			if (flag4 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag12 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag12)
		{
			if (flag8 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag16 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag16)
		{
			if (flag12 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag4 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		clipFace.GetEdge(3, out clippingEdge);
		if (!flag)
		{
			if (flag5 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag13 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag5)
		{
			if (flag && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V1, ref face.V2, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id1, face.Id2, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag9 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag9)
		{
			if (flag5 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V2, ref face.V3, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id2, face.Id3, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag13 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		if (!flag13)
		{
			if (flag && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V4, ref face.V1, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id4, face.Id1, ref clippingEdge);
				boxContactDataCache.Count++;
			}
			if (flag9 && boxContactDataCache.Count < 8 && ComputeIntersection(ref face.V3, ref face.V4, ref clippingEdge, out result17))
			{
				ptr[(int)boxContactDataCache.Count].Position = result17;
				ptr[(int)boxContactDataCache.Count].Id = GetContactId(face.Id3, face.Id4, ref clippingEdge);
				boxContactDataCache.Count++;
			}
		}
		boxContactDataCache2 = boxContactDataCache;
		boxContactDataCache.Count = count;
		for (int k = count; k < boxContactDataCache2.Count; k++)
		{
			Vector3.Dot(ref ptr2[k].Position, ref mtd, out result14);
			float num19 = result14 - result13;
			if (num19 <= 0f)
			{
				ptr[(int)boxContactDataCache.Count].Position = ptr2[k].Position;
				ptr[(int)boxContactDataCache.Count].Depth = num19;
				ptr[(int)boxContactDataCache.Count].Id = ptr2[k].Id;
				boxContactDataCache.Count++;
			}
		}
		outputData = boxContactDataCache;
	}

	private static bool ComputeIntersection(ref Vector3 edgeA1, ref Vector3 edgeA2, ref FaceEdge clippingEdge, out Vector3 intersection)
	{
		Vector3.Subtract(ref clippingEdge.A, ref edgeA1, out var result);
		Vector3.Subtract(ref edgeA2, ref edgeA1, out var result2);
		Vector3.Dot(ref result, ref clippingEdge.Perpendicular, out var result3);
		Vector3.Dot(ref result2, ref clippingEdge.Perpendicular, out var result4);
		float result5 = result3 / result4;
		if (result5 < 0f || result5 > 1f)
		{
			intersection = default(Vector3);
			return false;
		}
		Vector3.Multiply(ref result2, result5, out result);
		Vector3.Add(ref result, ref edgeA1, out intersection);
		Vector3.Subtract(ref intersection, ref clippingEdge.A, out result);
		Vector3.Subtract(ref clippingEdge.B, ref clippingEdge.A, out result2);
		Vector3.Dot(ref result2, ref result, out result5);
		if (result5 < 0f || result5 > result2.LengthSquared())
		{
			return false;
		}
		return true;
	}

	private static void GetNearestFace(ref Vector3 position, ref Matrix3X3 orientation, ref Vector3 mtd, float halfWidth, float halfHeight, float halfLength, out BoxFace boxFace)
	{
		boxFace = default(BoxFace);
		float num = orientation.M11 * mtd.X + orientation.M12 * mtd.Y + orientation.M13 * mtd.Z;
		float num2 = orientation.M21 * mtd.X + orientation.M22 * mtd.Y + orientation.M23 * mtd.Z;
		float num3 = orientation.M31 * mtd.X + orientation.M32 * mtd.Y + orientation.M33 * mtd.Z;
		float num4 = Math.Abs(num);
		float num5 = Math.Abs(num2);
		float num6 = Math.Abs(num3);
		Matrix3X3.ToMatrix4X4(ref orientation, out var b);
		b.M41 = position.X;
		b.M42 = position.Y;
		b.M43 = position.Z;
		b.M44 = 1f;
		if (num4 > num5 && num4 > num6)
		{
			int num7;
			if (num < 0f)
			{
				halfWidth = 0f - halfWidth;
				num7 = 0;
			}
			else
			{
				num7 = 1;
			}
			Vector3 position2 = new Vector3(halfWidth, halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V1 = position2;
			position2 = new Vector3(halfWidth, 0f - halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V2 = position2;
			position2 = new Vector3(halfWidth, 0f - halfHeight, 0f - halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V3 = position2;
			position2 = new Vector3(halfWidth, halfHeight, 0f - halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V4 = position2;
			if (num < 0f)
			{
				boxFace.Normal = orientation.Left;
			}
			else
			{
				boxFace.Normal = orientation.Right;
			}
			boxFace.Width = halfHeight * 2f;
			boxFace.Height = halfLength * 2f;
			boxFace.Id1 = num7 + 2 + 4;
			boxFace.Id2 = num7 + 4;
			boxFace.Id3 = num7 + 2;
			boxFace.Id4 = num7;
		}
		else if (num5 > num4 && num5 > num6)
		{
			int num7;
			if (num2 < 0f)
			{
				halfHeight = 0f - halfHeight;
				num7 = 0;
			}
			else
			{
				num7 = 2;
			}
			Vector3 position2 = new Vector3(halfWidth, halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V1 = position2;
			position2 = new Vector3(0f - halfWidth, halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V2 = position2;
			position2 = new Vector3(0f - halfWidth, halfHeight, 0f - halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V3 = position2;
			position2 = new Vector3(halfWidth, halfHeight, 0f - halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V4 = position2;
			if (num2 < 0f)
			{
				boxFace.Normal = orientation.Down;
			}
			else
			{
				boxFace.Normal = orientation.Up;
			}
			boxFace.Width = halfWidth * 2f;
			boxFace.Height = halfLength * 2f;
			boxFace.Id1 = 1 + num7 + 4;
			boxFace.Id2 = num7 + 4;
			boxFace.Id3 = 1 + num7;
			boxFace.Id4 = num7;
		}
		else if (num6 > num4 && num6 > num5)
		{
			int num7;
			if (num3 < 0f)
			{
				halfLength = 0f - halfLength;
				num7 = 0;
			}
			else
			{
				num7 = 4;
			}
			Vector3 position2 = new Vector3(halfWidth, halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V1 = position2;
			position2 = new Vector3(0f - halfWidth, halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V2 = position2;
			position2 = new Vector3(0f - halfWidth, 0f - halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V3 = position2;
			position2 = new Vector3(halfWidth, 0f - halfHeight, halfLength);
			Vector3.Transform(ref position2, ref b, out position2);
			boxFace.V4 = position2;
			if (num3 < 0f)
			{
				boxFace.Normal = orientation.Forward;
			}
			else
			{
				boxFace.Normal = orientation.Backward;
			}
			boxFace.Width = halfWidth * 2f;
			boxFace.Height = halfHeight * 2f;
			boxFace.Id1 = 3 + num7;
			boxFace.Id2 = 2 + num7;
			boxFace.Id3 = 1 + num7;
			boxFace.Id4 = num7;
		}
	}

	private static int GetContactId(int vertexAEdgeA, int vertexBEdgeA, int vertexAEdgeB, int vertexBEdgeB)
	{
		return GetEdgeId(vertexAEdgeA, vertexBEdgeA) * 2549 + GetEdgeId(vertexAEdgeB, vertexBEdgeB) * 2857;
	}

	private static int GetContactId(int vertexAEdgeA, int vertexBEdgeA, ref FaceEdge clippingEdge)
	{
		return GetEdgeId(vertexAEdgeA, vertexBEdgeA) * 2549 + clippingEdge.Id * 2857;
	}

	private static int GetEdgeId(int id1, int id2)
	{
		return (id1 + 1) * 571 + (id2 + 1) * 577;
	}
}
