using System;
using _0004;
using I;
using Microsoft.Xna.Framework;
using N;
using P;
using l;
using r;
using s;
using y;

namespace I
{
	internal static class _6
	{
		private struct _00065h
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
					v = r.X.NoVector;
					break;
				}
			}

			internal void _67(int P_0, out _00065b P_1)
			{
				Vector3 value;
				switch (P_0)
				{
				case 0:
					P_1.A = V1;
					P_1.B = V2;
					value = V3;
					P_1.Id = _6a(Id1, Id2);
					break;
				case 1:
					P_1.A = V2;
					P_1.B = V3;
					value = V4;
					P_1.Id = _6a(Id2, Id3);
					break;
				case 2:
					P_1.A = V3;
					P_1.B = V4;
					value = V1;
					P_1.Id = _6a(Id3, Id4);
					break;
				case 3:
					P_1.A = V4;
					P_1.B = V1;
					value = V2;
					P_1.Id = _6a(Id4, Id1);
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				Vector3.Subtract(ref P_1.B, ref P_1.A, out var result);
				result.Normalize();
				Vector3.Cross(ref result, ref Normal, out P_1.Perpendicular);
				Vector3.Subtract(ref value, ref P_1.A, out var result2);
				Vector3.Dot(ref P_1.Perpendicular, ref result2, out var result3);
				if (result3 > 0f)
				{
					P_1.Perpendicular.X = 0f - P_1.Perpendicular.X;
					P_1.Perpendicular.Y = 0f - P_1.Perpendicular.Y;
					P_1.Perpendicular.Z = 0f - P_1.Perpendicular.Z;
				}
				Vector3.Dot(ref P_1.A, ref P_1.Perpendicular, out P_1.EdgeDistance);
			}
		}

		private struct _00065b : IEquatable<_00065b>
		{
			public Vector3 A;

			public Vector3 B;

			public float EdgeDistance;

			public int Id;

			public Vector3 Perpendicular;

			public bool Equals(_00065b other)
			{
				return other.Id == Id;
			}

			public bool IsPointInside(ref Vector3 point)
			{
				Vector3.Dot(ref point, ref Perpendicular, out var result);
				return result < EdgeDistance;
			}
		}

		public static bool AreBoxesColliding(y._6 a, y._6 b, ref N._0006 transformA, ref N._0006 transformB)
		{
			float halfWidth = a.HalfWidth;
			float halfHeight = a.HalfHeight;
			float halfLength = a.HalfLength;
			float halfWidth2 = b.HalfWidth;
			float halfHeight2 = b.HalfHeight;
			float halfLength2 = b.HalfLength;
			N._7.CreateFromQuaternion(ref transformA.Orientation, out var result);
			N._7.CreateFromQuaternion(ref transformB.Orientation, out var result2);
			Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
			N._7 obj = default(N._7);
			obj.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
			obj.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
			obj.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
			N._7 obj2 = default(N._7);
			obj2.M11 = Math.Abs(obj.M11) + 1E-07f;
			obj2.M12 = Math.Abs(obj.M12) + 1E-07f;
			obj2.M13 = Math.Abs(obj.M13) + 1E-07f;
			float x = result3.X;
			result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
			float num = halfWidth2 * obj2.M11 + halfHeight2 * obj2.M12 + halfLength2 * obj2.M13;
			if (Math.Abs(result3.X) > halfWidth + num)
			{
				return false;
			}
			obj.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
			obj.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
			obj.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
			obj2.M21 = Math.Abs(obj.M21) + 1E-07f;
			obj2.M22 = Math.Abs(obj.M22) + 1E-07f;
			obj2.M23 = Math.Abs(obj.M23) + 1E-07f;
			float num2 = result3.Y;
			result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
			num = halfWidth2 * obj2.M21 + halfHeight2 * obj2.M22 + halfLength2 * obj2.M23;
			if (Math.Abs(result3.Y) > halfHeight + num)
			{
				return false;
			}
			obj.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
			obj.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
			obj.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
			obj2.M31 = Math.Abs(obj.M31) + 1E-07f;
			obj2.M32 = Math.Abs(obj.M32) + 1E-07f;
			obj2.M33 = Math.Abs(obj.M33) + 1E-07f;
			result3.Z = x * result.M31 + num2 * result.M32 + result3.Z * result.M33;
			num = halfWidth2 * obj2.M31 + halfHeight2 * obj2.M32 + halfLength2 * obj2.M33;
			if (Math.Abs(result3.Z) > halfLength + num)
			{
				return false;
			}
			float num3 = halfWidth * obj2.M11 + halfHeight * obj2.M21 + halfLength * obj2.M31;
			if (Math.Abs(result3.X * obj.M11 + result3.Y * obj.M21 + result3.Z * obj.M31) > num3 + halfWidth2)
			{
				return false;
			}
			num3 = halfWidth * obj2.M12 + halfHeight * obj2.M22 + halfLength * obj2.M32;
			if (Math.Abs(result3.X * obj.M12 + result3.Y * obj.M22 + result3.Z * obj.M32) > num3 + halfHeight2)
			{
				return false;
			}
			num3 = halfWidth * obj2.M13 + halfHeight * obj2.M23 + halfLength * obj2.M33;
			if (Math.Abs(result3.X * obj.M13 + result3.Y * obj.M23 + result3.Z * obj.M33) > num3 + halfLength2)
			{
				return false;
			}
			num3 = halfHeight * obj2.M31 + halfLength * obj2.M21;
			num = halfHeight2 * obj2.M13 + halfLength2 * obj2.M12;
			if (Math.Abs(result3.Z * obj.M21 - result3.Y * obj.M31) > num3 + num)
			{
				return false;
			}
			num3 = halfHeight * obj2.M32 + halfLength * obj2.M22;
			num = halfWidth2 * obj2.M13 + halfLength2 * obj2.M11;
			if (Math.Abs(result3.Z * obj.M22 - result3.Y * obj.M32) > num3 + num)
			{
				return false;
			}
			num3 = halfHeight * obj2.M33 + halfLength * obj2.M23;
			num = halfWidth2 * obj2.M12 + halfHeight2 * obj2.M11;
			if (Math.Abs(result3.Z * obj.M23 - result3.Y * obj.M33) > num3 + num)
			{
				return false;
			}
			num3 = halfWidth * obj2.M31 + halfLength * obj2.M11;
			num = halfHeight2 * obj2.M23 + halfLength2 * obj2.M22;
			if (Math.Abs(result3.X * obj.M31 - result3.Z * obj.M11) > num3 + num)
			{
				return false;
			}
			num3 = halfWidth * obj2.M32 + halfLength * obj2.M12;
			num = halfWidth2 * obj2.M23 + halfLength2 * obj2.M21;
			if (Math.Abs(result3.X * obj.M32 - result3.Z * obj.M12) > num3 + num)
			{
				return false;
			}
			num3 = halfWidth * obj2.M33 + halfLength * obj2.M13;
			num = halfWidth2 * obj2.M22 + halfHeight2 * obj2.M21;
			if (Math.Abs(result3.X * obj.M33 - result3.Z * obj.M13) > num3 + num)
			{
				return false;
			}
			num3 = halfWidth * obj2.M21 + halfHeight * obj2.M11;
			num = halfHeight2 * obj2.M33 + halfLength2 * obj2.M32;
			if (Math.Abs(result3.Y * obj.M11 - result3.X * obj.M21) > num3 + num)
			{
				return false;
			}
			num3 = halfWidth * obj2.M22 + halfHeight * obj2.M12;
			num = halfWidth2 * obj2.M33 + halfLength2 * obj2.M31;
			if (Math.Abs(result3.Y * obj.M12 - result3.X * obj.M22) > num3 + num)
			{
				return false;
			}
			num3 = halfWidth * obj2.M23 + halfHeight * obj2.M13;
			num = halfWidth2 * obj2.M32 + halfHeight2 * obj2.M31;
			if (Math.Abs(result3.Y * obj.M13 - result3.X * obj.M23) > num3 + num)
			{
				return false;
			}
			return true;
		}

		public static bool AreBoxesColliding(y._6 a, y._6 b, ref N._0006 transformA, ref N._0006 transformB, out float separationDistance, out Vector3 separatingAxis)
		{
			float halfWidth = a.HalfWidth;
			float halfHeight = a.HalfHeight;
			float halfLength = a.HalfLength;
			float halfWidth2 = b.HalfWidth;
			float halfHeight2 = b.HalfHeight;
			float halfLength2 = b.HalfLength;
			N._7.CreateFromQuaternion(ref transformA.Orientation, out var result);
			N._7.CreateFromQuaternion(ref transformB.Orientation, out var result2);
			Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
			N._7 obj = default(N._7);
			obj.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
			obj.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
			obj.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
			N._7 obj2 = default(N._7);
			obj2.M11 = Math.Abs(obj.M11) + 1E-07f;
			obj2.M12 = Math.Abs(obj.M12) + 1E-07f;
			obj2.M13 = Math.Abs(obj.M13) + 1E-07f;
			float x = result3.X;
			result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
			float num = halfWidth + halfWidth2 * obj2.M11 + halfHeight2 * obj2.M12 + halfLength2 * obj2.M13;
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
			obj.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
			obj.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
			obj.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
			obj2.M21 = Math.Abs(obj.M21) + 1E-07f;
			obj2.M22 = Math.Abs(obj.M22) + 1E-07f;
			obj2.M23 = Math.Abs(obj.M23) + 1E-07f;
			float num2 = result3.Y;
			result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
			num = halfHeight + halfWidth2 * obj2.M21 + halfHeight2 * obj2.M22 + halfLength2 * obj2.M23;
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
			obj.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
			obj.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
			obj.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
			obj2.M31 = Math.Abs(obj.M31) + 1E-07f;
			obj2.M32 = Math.Abs(obj.M32) + 1E-07f;
			obj2.M33 = Math.Abs(obj.M33) + 1E-07f;
			result3.Z = x * result.M31 + num2 * result.M32 + result3.Z * result.M33;
			num = halfLength + halfWidth2 * obj2.M31 + halfHeight2 * obj2.M32 + halfLength2 * obj2.M33;
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
			num = halfWidth2 + halfWidth * obj2.M11 + halfHeight * obj2.M21 + halfLength * obj2.M31;
			float num3 = result3.X * obj.M11 + result3.Y * obj.M21 + result3.Z * obj.M31;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result2.M11, result2.M12, result2.M13);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
				return false;
			}
			num = halfHeight2 + halfWidth * obj2.M12 + halfHeight * obj2.M22 + halfLength * obj2.M32;
			num3 = result3.X * obj.M12 + result3.Y * obj.M22 + result3.Z * obj.M32;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result2.M21, result2.M22, result2.M23);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
				return false;
			}
			num = halfLength2 + halfWidth * obj2.M13 + halfHeight * obj2.M23 + halfLength * obj2.M33;
			num3 = result3.X * obj.M13 + result3.Y * obj.M23 + result3.Z * obj.M33;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result2.M31, result2.M32, result2.M33);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
				return false;
			}
			num = halfHeight * obj2.M31 + halfLength * obj2.M21 + halfHeight2 * obj2.M13 + halfLength2 * obj2.M12;
			num3 = result3.Z * obj.M21 - result3.Y * obj.M31;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
				return false;
			}
			num = halfHeight * obj2.M32 + halfLength * obj2.M22 + halfWidth2 * obj2.M13 + halfLength2 * obj2.M11;
			num3 = result3.Z * obj.M22 - result3.Y * obj.M32;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
				return false;
			}
			num = halfHeight * obj2.M33 + halfLength * obj2.M23 + halfWidth2 * obj2.M12 + halfHeight2 * obj2.M11;
			num3 = result3.Z * obj.M23 - result3.Y * obj.M33;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
				return false;
			}
			num = halfWidth * obj2.M31 + halfLength * obj2.M11 + halfHeight2 * obj2.M23 + halfLength2 * obj2.M22;
			num3 = result3.X * obj.M31 - result3.Z * obj.M11;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
				return false;
			}
			num = halfWidth * obj2.M32 + halfLength * obj2.M12 + halfWidth2 * obj2.M23 + halfLength2 * obj2.M21;
			num3 = result3.X * obj.M32 - result3.Z * obj.M12;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
				return false;
			}
			num = halfWidth * obj2.M33 + halfLength * obj2.M13 + halfWidth2 * obj2.M22 + halfHeight2 * obj2.M21;
			num3 = result3.X * obj.M33 - result3.Z * obj.M13;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
				return false;
			}
			num = halfWidth * obj2.M21 + halfHeight * obj2.M11 + halfHeight2 * obj2.M33 + halfLength2 * obj2.M32;
			num3 = result3.Y * obj.M11 - result3.X * obj.M21;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
				return false;
			}
			num = halfWidth * obj2.M22 + halfHeight * obj2.M12 + halfWidth2 * obj2.M33 + halfLength2 * obj2.M31;
			num3 = result3.Y * obj.M12 - result3.X * obj.M22;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
				return false;
			}
			num = halfWidth * obj2.M23 + halfHeight * obj2.M13 + halfWidth2 * obj2.M32 + halfHeight2 * obj2.M31;
			num3 = result3.Y * obj.M13 - result3.X * obj.M23;
			if (num3 > num)
			{
				separationDistance = num3 - num;
				separatingAxis = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
				return false;
			}
			if (num3 < 0f - num)
			{
				separationDistance = 0f - num3 - num;
				separatingAxis = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
				return false;
			}
			separationDistance = 0f;
			separatingAxis = Vector3.Zero;
			return true;
		}

		public static bool AreBoxesCollidingWithPenetration(y._6 a, y._6 b, ref N._0006 transformA, ref N._0006 transformB, out float distance, out Vector3 axis)
		{
			float halfWidth = a.HalfWidth;
			float halfHeight = a.HalfHeight;
			float halfLength = a.HalfLength;
			float halfWidth2 = b.HalfWidth;
			float halfHeight2 = b.HalfHeight;
			float halfLength2 = b.HalfLength;
			N._7.CreateFromQuaternion(ref transformA.Orientation, out var result);
			N._7.CreateFromQuaternion(ref transformB.Orientation, out var result2);
			Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
			float num = float.MinValue;
			Vector3 vector = default(Vector3);
			N._7 obj = default(N._7);
			obj.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
			obj.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
			obj.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
			N._7 obj2 = default(N._7);
			obj2.M11 = Math.Abs(obj.M11) + 1E-07f;
			obj2.M12 = Math.Abs(obj.M12) + 1E-07f;
			obj2.M13 = Math.Abs(obj.M13) + 1E-07f;
			float x = result3.X;
			result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
			float num2 = halfWidth + halfWidth2 * obj2.M11 + halfHeight2 * obj2.M12 + halfLength2 * obj2.M13;
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
			obj.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
			obj.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
			obj.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
			obj2.M21 = Math.Abs(obj.M21) + 1E-07f;
			obj2.M22 = Math.Abs(obj.M22) + 1E-07f;
			obj2.M23 = Math.Abs(obj.M23) + 1E-07f;
			float num4 = result3.Y;
			result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
			num2 = halfHeight + halfWidth2 * obj2.M21 + halfHeight2 * obj2.M22 + halfLength2 * obj2.M23;
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
			obj.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
			obj.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
			obj.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
			obj2.M31 = Math.Abs(obj.M31) + 1E-07f;
			obj2.M32 = Math.Abs(obj.M32) + 1E-07f;
			obj2.M33 = Math.Abs(obj.M33) + 1E-07f;
			result3.Z = x * result.M31 + num4 * result.M32 + result3.Z * result.M33;
			num2 = halfLength + halfWidth2 * obj2.M31 + halfHeight2 * obj2.M32 + halfLength2 * obj2.M33;
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
			num2 = halfWidth2 + halfWidth * obj2.M11 + halfHeight * obj2.M21 + halfLength * obj2.M31;
			float num5 = result3.X * obj.M11 + result3.Y * obj.M21 + result3.Z * obj.M31;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M11, result2.M12, result2.M13);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
				return false;
			}
			if (num5 > 0f)
			{
				float num3 = num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result2.M11, result2.M12, result2.M13);
				}
			}
			else
			{
				float num3 = 0f - num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
				}
			}
			num2 = halfHeight2 + halfWidth * obj2.M12 + halfHeight * obj2.M22 + halfLength * obj2.M32;
			num5 = result3.X * obj.M12 + result3.Y * obj.M22 + result3.Z * obj.M32;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M21, result2.M22, result2.M23);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
				return false;
			}
			if (num5 > 0f)
			{
				float num3 = num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result2.M21, result2.M22, result2.M23);
				}
			}
			else
			{
				float num3 = 0f - num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
				}
			}
			num2 = halfLength2 + halfWidth * obj2.M13 + halfHeight * obj2.M23 + halfLength * obj2.M33;
			num5 = result3.X * obj.M13 + result3.Y * obj.M23 + result3.Z * obj.M33;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M31, result2.M32, result2.M33);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
				return false;
			}
			if (num5 > 0f)
			{
				float num3 = num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result2.M31, result2.M32, result2.M33);
				}
			}
			else
			{
				float num3 = 0f - num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
				}
			}
			num2 = halfHeight * obj2.M31 + halfLength * obj2.M21 + halfHeight2 * obj2.M13 + halfLength2 * obj2.M12;
			num5 = result3.Z * obj.M21 - result3.Y * obj.M31;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfHeight * obj2.M32 + halfLength * obj2.M22 + halfWidth2 * obj2.M13 + halfLength2 * obj2.M11;
			num5 = result3.Z * obj.M22 - result3.Y * obj.M32;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfHeight * obj2.M33 + halfLength * obj2.M23 + halfWidth2 * obj2.M12 + halfHeight2 * obj2.M11;
			num5 = result3.Z * obj.M23 - result3.Y * obj.M33;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M31 + halfLength * obj2.M11 + halfHeight2 * obj2.M23 + halfLength2 * obj2.M22;
			num5 = result3.X * obj.M31 - result3.Z * obj.M11;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M32 + halfLength * obj2.M12 + halfWidth2 * obj2.M23 + halfLength2 * obj2.M21;
			num5 = result3.X * obj.M32 - result3.Z * obj.M12;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M33 + halfLength * obj2.M13 + halfWidth2 * obj2.M22 + halfHeight2 * obj2.M21;
			num5 = result3.X * obj.M33 - result3.Z * obj.M13;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M21 + halfHeight * obj2.M11 + halfHeight2 * obj2.M33 + halfLength2 * obj2.M32;
			num5 = result3.Y * obj.M11 - result3.X * obj.M21;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M22 + halfHeight * obj2.M12 + halfWidth2 * obj2.M33 + halfLength2 * obj2.M31;
			num5 = result3.Y * obj.M12 - result3.X * obj.M22;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M23 + halfHeight * obj2.M13 + halfWidth2 * obj2.M32 + halfHeight2 * obj2.M31;
			num5 = result3.Y * obj.M13 - result3.X * obj.M23;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
				float num6 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
				float num6 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num6;
				if (num3 > num)
				{
					num = num3;
					vector2.X *= num6;
					vector2.Y *= num6;
					vector2.Z *= num6;
					vector = vector2;
				}
			}
			distance = num;
			axis = vector;
			return true;
		}

		public static bool AreBoxesColliding(y._6 a, y._6 b, ref N._0006 transformA, ref N._0006 transformB, out float distance, out Vector3 axis, out l.W<h> contactData)
		{
			float halfWidth = a.HalfWidth;
			float halfHeight = a.HalfHeight;
			float halfLength = a.HalfLength;
			float halfWidth2 = b.HalfWidth;
			float halfHeight2 = b.HalfHeight;
			float halfLength2 = b.HalfLength;
			contactData = default(l.W<h>);
			N._7.CreateFromQuaternion(ref transformA.Orientation, out var result);
			N._7.CreateFromQuaternion(ref transformB.Orientation, out var result2);
			Vector3.Subtract(ref transformB.Position, ref transformA.Position, out var result3);
			float num = float.MinValue;
			Vector3 vector = default(Vector3);
			byte b2 = 2;
			N._7 obj = default(N._7);
			obj.M11 = result.M11 * result2.M11 + result.M12 * result2.M12 + result.M13 * result2.M13;
			obj.M12 = result.M11 * result2.M21 + result.M12 * result2.M22 + result.M13 * result2.M23;
			obj.M13 = result.M11 * result2.M31 + result.M12 * result2.M32 + result.M13 * result2.M33;
			N._7 obj2 = default(N._7);
			obj2.M11 = Math.Abs(obj.M11) + 1E-07f;
			obj2.M12 = Math.Abs(obj.M12) + 1E-07f;
			obj2.M13 = Math.Abs(obj.M13) + 1E-07f;
			float x = result3.X;
			result3.X = result3.X * result.M11 + result3.Y * result.M12 + result3.Z * result.M13;
			float num2 = halfWidth + halfWidth2 * obj2.M11 + halfHeight2 * obj2.M12 + halfLength2 * obj2.M13;
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
					vector = new Vector3(0f - result.M11, 0f - result.M12, 0f - result.M13);
					b2 = 0;
				}
			}
			else
			{
				float num3 = 0f - result3.X - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result.M11, result.M12, result.M13);
					b2 = 0;
				}
			}
			obj.M21 = result.M21 * result2.M11 + result.M22 * result2.M12 + result.M23 * result2.M13;
			obj.M22 = result.M21 * result2.M21 + result.M22 * result2.M22 + result.M23 * result2.M23;
			obj.M23 = result.M21 * result2.M31 + result.M22 * result2.M32 + result.M23 * result2.M33;
			obj2.M21 = Math.Abs(obj.M21) + 1E-07f;
			obj2.M22 = Math.Abs(obj.M22) + 1E-07f;
			obj2.M23 = Math.Abs(obj.M23) + 1E-07f;
			float num4 = result3.Y;
			result3.Y = x * result.M21 + result3.Y * result.M22 + result3.Z * result.M23;
			num2 = halfHeight + halfWidth2 * obj2.M21 + halfHeight2 * obj2.M22 + halfLength2 * obj2.M23;
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
					vector = new Vector3(0f - result.M21, 0f - result.M22, 0f - result.M23);
					b2 = 0;
				}
			}
			else
			{
				float num3 = 0f - result3.Y - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result.M21, result.M22, result.M23);
					b2 = 0;
				}
			}
			obj.M31 = result.M31 * result2.M11 + result.M32 * result2.M12 + result.M33 * result2.M13;
			obj.M32 = result.M31 * result2.M21 + result.M32 * result2.M22 + result.M33 * result2.M23;
			obj.M33 = result.M31 * result2.M31 + result.M32 * result2.M32 + result.M33 * result2.M33;
			obj2.M31 = Math.Abs(obj.M31) + 1E-07f;
			obj2.M32 = Math.Abs(obj.M32) + 1E-07f;
			obj2.M33 = Math.Abs(obj.M33) + 1E-07f;
			result3.Z = x * result.M31 + num4 * result.M32 + result3.Z * result.M33;
			num2 = halfLength + halfWidth2 * obj2.M31 + halfHeight2 * obj2.M32 + halfLength2 * obj2.M33;
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
					vector = new Vector3(0f - result.M31, 0f - result.M32, 0f - result.M33);
					b2 = 0;
				}
			}
			else
			{
				float num3 = 0f - result3.Z - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result.M31, result.M32, result.M33);
					b2 = 0;
				}
			}
			num += 0.01f;
			num2 = halfWidth2 + halfWidth * obj2.M11 + halfHeight * obj2.M21 + halfLength * obj2.M31;
			float num5 = result3.X * obj.M11 + result3.Y * obj.M21 + result3.Z * obj.M31;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M11, result2.M12, result2.M13);
				return false;
			}
			if (num5 > 0f)
			{
				float num3 = num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(0f - result2.M11, 0f - result2.M12, 0f - result2.M13);
					b2 = 1;
				}
			}
			else
			{
				float num3 = 0f - num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result2.M11, result2.M12, result2.M13);
					b2 = 1;
				}
			}
			num2 = halfHeight2 + halfWidth * obj2.M12 + halfHeight * obj2.M22 + halfLength * obj2.M32;
			num5 = result3.X * obj.M12 + result3.Y * obj.M22 + result3.Z * obj.M32;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M21, result2.M22, result2.M23);
				return false;
			}
			if (num5 > 0f)
			{
				float num3 = num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(0f - result2.M21, 0f - result2.M22, 0f - result2.M23);
					b2 = 1;
				}
			}
			else
			{
				float num3 = 0f - num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result2.M21, result2.M22, result2.M23);
					b2 = 1;
				}
			}
			num2 = halfLength2 + halfWidth * obj2.M13 + halfHeight * obj2.M23 + halfLength * obj2.M33;
			num5 = result3.X * obj.M13 + result3.Y * obj.M23 + result3.Z * obj.M33;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result2.M31, result2.M32, result2.M33);
				return false;
			}
			if (num5 > 0f)
			{
				float num3 = num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(0f - result2.M31, 0f - result2.M32, 0f - result2.M33);
					b2 = 1;
				}
			}
			else
			{
				float num3 = 0f - num5 - num2;
				if (num3 > num)
				{
					num = num3;
					vector = new Vector3(result2.M31, result2.M32, result2.M33);
					b2 = 1;
				}
			}
			if (b2 != 1)
			{
				num -= 0.01f;
			}
			float num6 = 0.01f;
			num += num6;
			num2 = halfHeight * obj2.M31 + halfLength * obj2.M21 + halfHeight2 * obj2.M13 + halfLength2 * obj2.M12;
			num5 = result3.Z * obj.M21 - result3.Y * obj.M31;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M12 * result.M13 - result2.M13 * result.M12, result2.M13 * result.M11 - result2.M11 * result.M13, result2.M11 * result.M12 - result2.M12 * result.M11);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M12 * result2.M13 - result.M13 * result2.M12, result.M13 * result2.M11 - result.M11 * result2.M13, result.M11 * result2.M12 - result.M12 * result2.M11);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfHeight * obj2.M32 + halfLength * obj2.M22 + halfWidth2 * obj2.M13 + halfLength2 * obj2.M11;
			num5 = result3.Z * obj.M22 - result3.Y * obj.M32;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M22 * result.M13 - result2.M23 * result.M12, result2.M23 * result.M11 - result2.M21 * result.M13, result2.M21 * result.M12 - result2.M22 * result.M11);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M12 * result2.M23 - result.M13 * result2.M22, result.M13 * result2.M21 - result.M11 * result2.M23, result.M11 * result2.M22 - result.M12 * result2.M21);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfHeight * obj2.M33 + halfLength * obj2.M23 + halfWidth2 * obj2.M12 + halfHeight2 * obj2.M11;
			num5 = result3.Z * obj.M23 - result3.Y * obj.M33;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M32 * result.M13 - result2.M33 * result.M12, result2.M33 * result.M11 - result2.M31 * result.M13, result2.M31 * result.M12 - result2.M32 * result.M11);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M12 * result2.M33 - result.M13 * result2.M32, result.M13 * result2.M31 - result.M11 * result2.M33, result.M11 * result2.M32 - result.M12 * result2.M31);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M31 + halfLength * obj2.M11 + halfHeight2 * obj2.M23 + halfLength2 * obj2.M22;
			num5 = result3.X * obj.M31 - result3.Z * obj.M11;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M12 * result.M23 - result2.M13 * result.M22, result2.M13 * result.M21 - result2.M11 * result.M23, result2.M11 * result.M22 - result2.M12 * result.M21);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M22 * result2.M13 - result.M23 * result2.M12, result.M23 * result2.M11 - result.M21 * result2.M13, result.M21 * result2.M12 - result.M22 * result2.M11);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M32 + halfLength * obj2.M12 + halfWidth2 * obj2.M23 + halfLength2 * obj2.M21;
			num5 = result3.X * obj.M32 - result3.Z * obj.M12;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M22 * result.M23 - result2.M23 * result.M22, result2.M23 * result.M21 - result2.M21 * result.M23, result2.M21 * result.M22 - result2.M22 * result.M21);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M22 * result2.M23 - result.M23 * result2.M22, result.M23 * result2.M21 - result.M21 * result2.M23, result.M21 * result2.M22 - result.M22 * result2.M21);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M33 + halfLength * obj2.M13 + halfWidth2 * obj2.M22 + halfHeight2 * obj2.M21;
			num5 = result3.X * obj.M33 - result3.Z * obj.M13;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M32 * result.M23 - result2.M33 * result.M22, result2.M33 * result.M21 - result2.M31 * result.M23, result2.M31 * result.M22 - result2.M32 * result.M21);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M22 * result2.M33 - result.M23 * result2.M32, result.M23 * result2.M31 - result.M21 * result2.M33, result.M21 * result2.M32 - result.M22 * result2.M31);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M21 + halfHeight * obj2.M11 + halfHeight2 * obj2.M33 + halfLength2 * obj2.M32;
			num5 = result3.Y * obj.M11 - result3.X * obj.M21;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M12 * result.M33 - result2.M13 * result.M32, result2.M13 * result.M31 - result2.M11 * result.M33, result2.M11 * result.M32 - result2.M12 * result.M31);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M32 * result2.M13 - result.M33 * result2.M12, result.M33 * result2.M11 - result.M31 * result2.M13, result.M31 * result2.M12 - result.M32 * result2.M11);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M22 + halfHeight * obj2.M12 + halfWidth2 * obj2.M33 + halfLength2 * obj2.M31;
			num5 = result3.Y * obj.M12 - result3.X * obj.M22;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M22 * result.M33 - result2.M23 * result.M32, result2.M23 * result.M31 - result2.M21 * result.M33, result2.M21 * result.M32 - result2.M22 * result.M31);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M32 * result2.M23 - result.M33 * result2.M22, result.M33 * result2.M21 - result.M31 * result2.M23, result.M31 * result2.M22 - result.M32 * result2.M21);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			num2 = halfWidth * obj2.M23 + halfHeight * obj2.M13 + halfWidth2 * obj2.M32 + halfHeight2 * obj2.M31;
			num5 = result3.Y * obj.M13 - result3.X * obj.M23;
			if (num5 > num2)
			{
				distance = num5 - num2;
				axis = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
				return false;
			}
			if (num5 < 0f - num2)
			{
				distance = 0f - num5 - num2;
				axis = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
				return false;
			}
			if (num5 > 0f)
			{
				Vector3 vector2 = new Vector3(result2.M32 * result.M33 - result2.M33 * result.M32, result2.M33 * result.M31 - result2.M31 * result.M33, result2.M31 * result.M32 - result2.M32 * result.M31);
				float num7 = 1f / vector2.Length();
				float num3 = (num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			else
			{
				Vector3 vector2 = new Vector3(result.M32 * result2.M33 - result.M33 * result2.M32, result.M33 * result2.M31 - result.M31 * result2.M33, result.M31 * result2.M32 - result.M32 * result2.M31);
				float num7 = 1f / vector2.Length();
				float num3 = (0f - num5 - num2) * num7;
				if (num3 > num)
				{
					b2 = 2;
					num = num3;
					vector2.X *= num7;
					vector2.Y *= num7;
					vector2.Z *= num7;
					vector = vector2;
				}
			}
			if (b2 == 2)
			{
				b0(a, b, ref transformA.Position, ref result, ref transformB.Position, ref result2, num, ref vector, out contactData);
			}
			else
			{
				num -= num6;
				b8(a, b, ref transformA.Position, ref result, ref transformB.Position, ref result2, b2 == 0, ref vector, out contactData);
			}
			distance = num;
			axis = vector;
			return true;
		}

		internal static void b0(y._6 P_0, y._6 P_1, ref Vector3 P_2, ref N._7 P_3, ref Vector3 P_4, ref N._7 P_5, float P_6, ref Vector3 P_7, out l.W<h> P_8)
		{
			Vector3.Negate(ref P_7, out var result);
			N._7.TransformTranspose(ref result, ref P_3, out var result2);
			N._7.TransformTranspose(ref P_7, ref P_5, out var result3);
			Vector3 result4 = default(Vector3);
			Vector3 result5 = default(Vector3);
			Vector3 result6 = default(Vector3);
			Vector3 result7 = default(Vector3);
			Vector3 result8 = default(Vector3);
			Vector3 result9 = default(Vector3);
			Vector3 result10 = default(Vector3);
			Vector3 result11 = default(Vector3);
			float a5h = P_0.a5h;
			float a5b = P_0.a5b;
			float a2 = P_0.a56;
			float a5h2 = P_1.a5h;
			float a5b2 = P_1.a5b;
			float a3 = P_1.a56;
			int num3;
			int num4;
			int num5;
			int num6;
			if (Math.Abs(result2.X) < 1E-07f)
			{
				l._0018<float> obj = default(l._0018<float>);
				obj.Add((0f - a5b) * result2.Y - a2 * result2.Z);
				obj.Add((0f - a5b) * result2.Y + a2 * result2.Z);
				obj.Add(a5b * result2.Y - a2 * result2.Z);
				obj.Add(a5b * result2.Y + a2 * result2.Z);
				bA(ref obj, out var num, out var num2);
				bU(num, 0, a5h, a5b, a2, out result4, out result5, out num3, out num4);
				bU(num2, 0, a5h, a5b, a2, out result6, out result7, out num5, out num6);
			}
			else if (Math.Abs(result2.Y) < 1E-07f)
			{
				l._0018<float> obj2 = default(l._0018<float>);
				obj2.Add((0f - a5h) * result2.X - a2 * result2.Z);
				obj2.Add((0f - a5h) * result2.X + a2 * result2.Z);
				obj2.Add(a5h * result2.X - a2 * result2.Z);
				obj2.Add(a5h * result2.X + a2 * result2.Z);
				bA(ref obj2, out var num7, out var num8);
				bU(num7, 1, a5h, a5b, a2, out result4, out result5, out num3, out num4);
				bU(num8, 1, a5h, a5b, a2, out result6, out result7, out num5, out num6);
			}
			else
			{
				l._0018<float> obj3 = default(l._0018<float>);
				obj3.Add((0f - a5h) * result2.X - a5b * result2.Y);
				obj3.Add((0f - a5h) * result2.X + a5b * result2.Y);
				obj3.Add(a5h * result2.X - a5b * result2.Y);
				obj3.Add(a5h * result2.X + a5b * result2.Y);
				bA(ref obj3, out var num9, out var num10);
				bU(num9, 2, a5h, a5b, a2, out result4, out result5, out num3, out num4);
				bU(num10, 2, a5h, a5b, a2, out result6, out result7, out num5, out num6);
			}
			int num13;
			int num14;
			int num15;
			int num16;
			if (Math.Abs(result3.X) < 1E-07f)
			{
				l._0018<float> obj4 = default(l._0018<float>);
				obj4.Add((0f - a5b2) * result3.Y - a3 * result3.Z);
				obj4.Add((0f - a5b2) * result3.Y + a3 * result3.Z);
				obj4.Add(a5b2 * result3.Y - a3 * result3.Z);
				obj4.Add(a5b2 * result3.Y + a3 * result3.Z);
				bA(ref obj4, out var num11, out var num12);
				bU(num11, 0, a5h2, a5b2, a3, out result8, out result9, out num13, out num14);
				bU(num12, 0, a5h2, a5b2, a3, out result10, out result11, out num15, out num16);
			}
			else if (Math.Abs(result3.Y) < 1E-07f)
			{
				l._0018<float> obj5 = default(l._0018<float>);
				obj5.Add((0f - a5h2) * result3.X - a3 * result3.Z);
				obj5.Add((0f - a5h2) * result3.X + a3 * result3.Z);
				obj5.Add(a5h2 * result3.X - a3 * result3.Z);
				obj5.Add(a5h2 * result3.X + a3 * result3.Z);
				bA(ref obj5, out var num17, out var num18);
				bU(num17, 1, a5h2, a5b2, a3, out result8, out result9, out num13, out num14);
				bU(num18, 1, a5h2, a5b2, a3, out result10, out result11, out num15, out num16);
			}
			else
			{
				l._0018<float> obj6 = default(l._0018<float>);
				obj6.Add((0f - a5h2) * result3.X - a5b2 * result3.Y);
				obj6.Add((0f - a5h2) * result3.X + a5b2 * result3.Y);
				obj6.Add(a5h2 * result3.X - a5b2 * result3.Y);
				obj6.Add(a5h2 * result3.X + a5b2 * result3.Y);
				bA(ref obj6, out var num19, out var num20);
				bU(num19, 2, a5h2, a5b2, a3, out result8, out result9, out num13, out num14);
				bU(num20, 2, a5h2, a5b2, a3, out result10, out result11, out num15, out num16);
			}
			N._7.Transform(ref result4, ref P_3, out result4);
			N._7.Transform(ref result5, ref P_3, out result5);
			N._7.Transform(ref result8, ref P_5, out result8);
			N._7.Transform(ref result9, ref P_5, out result9);
			N._7.Transform(ref result6, ref P_3, out result6);
			N._7.Transform(ref result7, ref P_3, out result7);
			N._7.Transform(ref result10, ref P_5, out result10);
			N._7.Transform(ref result11, ref P_5, out result11);
			Vector3.Add(ref result4, ref P_2, out result4);
			Vector3.Add(ref result5, ref P_2, out result5);
			Vector3.Add(ref result8, ref P_4, out result8);
			Vector3.Add(ref result9, ref P_4, out result9);
			Vector3.Add(ref result6, ref P_2, out result6);
			Vector3.Add(ref result7, ref P_2, out result7);
			Vector3.Add(ref result10, ref P_4, out result10);
			Vector3.Add(ref result11, ref P_4, out result11);
			P_8 = default(l.W<h>);
			Vector3 result12;
			float result13;
			if (b_0019(ref result4, ref result5, ref result8, ref result9, out var value, out var value2))
			{
				Vector3.Subtract(ref value, ref value2, out result12);
				Vector3.Dot(ref result12, ref P_7, out result13);
				if (result13 < 0f)
				{
					h item = default(h);
					item.Position = value;
					item.Depth = result13;
					item.Id = _66(num3, num4, num13, num14);
					P_8.Add(ref item);
				}
			}
			if (b_0019(ref result4, ref result5, ref result10, ref result11, out value, out value2))
			{
				Vector3.Subtract(ref value, ref value2, out result12);
				Vector3.Dot(ref result12, ref P_7, out result13);
				if (result13 < 0f)
				{
					h item2 = default(h);
					item2.Position = value;
					item2.Depth = result13;
					item2.Id = _66(num3, num4, num15, num16);
					P_8.Add(ref item2);
				}
			}
			if (b_0019(ref result6, ref result7, ref result8, ref result9, out value, out value2))
			{
				Vector3.Subtract(ref value, ref value2, out result12);
				Vector3.Dot(ref result12, ref P_7, out result13);
				if (result13 < 0f)
				{
					h item3 = default(h);
					item3.Position = value;
					item3.Depth = result13;
					item3.Id = _66(num5, num6, num13, num14);
					P_8.Add(ref item3);
				}
			}
			if (b_0019(ref result6, ref result7, ref result10, ref result11, out value, out value2))
			{
				Vector3.Subtract(ref value, ref value2, out result12);
				Vector3.Dot(ref result12, ref P_7, out result13);
				if (result13 < 0f)
				{
					h item4 = default(h);
					item4.Position = value;
					item4.Depth = result13;
					item4.Id = _66(num5, num6, num15, num16);
					P_8.Add(ref item4);
				}
			}
		}

		private static void bU(int P_0, int P_1, float P_2, float P_3, float P_4, out Vector3 P_5, out Vector3 P_6, out int P_7, out int P_8)
		{
			P_5 = default(Vector3);
			P_6 = default(Vector3);
			switch (P_0 + P_1 * 4)
			{
			case 0:
				P_5.X = 0f - P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = 0f - P_4;
				P_7 = 0;
				P_6.X = P_2;
				P_6.Y = 0f - P_3;
				P_6.Z = 0f - P_4;
				P_8 = 4;
				break;
			case 1:
				P_5.X = 0f - P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = P_4;
				P_7 = 1;
				P_6.X = P_2;
				P_6.Y = 0f - P_3;
				P_6.Z = P_4;
				P_8 = 5;
				break;
			case 2:
				P_5.X = 0f - P_2;
				P_5.Y = P_3;
				P_5.Z = 0f - P_4;
				P_7 = 2;
				P_6.X = P_2;
				P_6.Y = P_3;
				P_6.Z = 0f - P_4;
				P_8 = 6;
				break;
			case 3:
				P_5.X = 0f - P_2;
				P_5.Y = P_3;
				P_5.Z = P_4;
				P_7 = 3;
				P_6.X = P_2;
				P_6.Y = P_3;
				P_6.Z = P_4;
				P_8 = 7;
				break;
			case 4:
				P_5.X = 0f - P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = 0f - P_4;
				P_7 = 0;
				P_6.X = 0f - P_2;
				P_6.Y = P_3;
				P_6.Z = 0f - P_4;
				P_8 = 2;
				break;
			case 5:
				P_5.X = 0f - P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = P_4;
				P_7 = 1;
				P_6.X = 0f - P_2;
				P_6.Y = P_3;
				P_6.Z = P_4;
				P_8 = 3;
				break;
			case 6:
				P_5.X = P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = 0f - P_4;
				P_7 = 4;
				P_6.X = P_2;
				P_6.Y = P_3;
				P_6.Z = 0f - P_4;
				P_8 = 6;
				break;
			case 7:
				P_5.X = P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = P_4;
				P_7 = 5;
				P_6.X = P_2;
				P_6.Y = P_3;
				P_6.Z = P_4;
				P_8 = 7;
				break;
			case 8:
				P_5.X = 0f - P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = 0f - P_4;
				P_7 = 0;
				P_6.X = 0f - P_2;
				P_6.Y = 0f - P_3;
				P_6.Z = P_4;
				P_8 = 1;
				break;
			case 9:
				P_5.X = 0f - P_2;
				P_5.Y = P_3;
				P_5.Z = 0f - P_4;
				P_7 = 2;
				P_6.X = 0f - P_2;
				P_6.Y = P_3;
				P_6.Z = P_4;
				P_8 = 3;
				break;
			case 10:
				P_5.X = P_2;
				P_5.Y = 0f - P_3;
				P_5.Z = 0f - P_4;
				P_7 = 4;
				P_6.X = P_2;
				P_6.Y = 0f - P_3;
				P_6.Z = P_4;
				P_8 = 5;
				break;
			case 11:
				P_5.X = P_2;
				P_5.Y = P_3;
				P_5.Z = 0f - P_4;
				P_7 = 6;
				P_6.X = P_2;
				P_6.Y = P_3;
				P_6.Z = P_4;
				P_8 = 7;
				break;
			default:
				throw new Exception("Invalid index or axis.");
			}
		}

		private static void bA(ref l._0018<float> P_0, out int P_1, out int P_2)
		{
			P_1 = 0;
			float num = P_0[0];
			for (int i = 1; i < 4; i++)
			{
				float num2 = P_0[i];
				if (num2 > num)
				{
					P_1 = i;
					num = num2;
				}
			}
			P_2 = 0;
			float num3 = float.MinValue;
			for (int j = 0; j < 4; j++)
			{
				float num4 = P_0[j];
				if (j != P_1 && num4 > num3)
				{
					P_2 = j;
					num3 = num4;
				}
			}
		}

		private static bool b_0019(ref Vector3 P_0, ref Vector3 P_1, ref Vector3 P_2, ref Vector3 P_3, out Vector3 P_4, out Vector3 P_5)
		{
			Vector3.Subtract(ref P_1, ref P_0, out var result);
			Vector3.Subtract(ref P_3, ref P_2, out var result2);
			Vector3.Subtract(ref P_0, ref P_2, out var result3);
			float num = result.LengthSquared();
			float num2 = result2.LengthSquared();
			Vector3.Dot(ref result2, ref result3, out var result4);
			float num3;
			if (num <= 1E-07f && num2 <= 1E-07f)
			{
				num3 = 0f;
				P_4 = P_0;
				P_5 = P_2;
				return false;
			}
			float num4;
			if (num <= 1E-07f)
			{
				num4 = 0f;
				num3 = result4 / num2;
				if (num3 < 0f || num3 > 1f)
				{
					P_4 = default(Vector3);
					P_5 = default(Vector3);
					return false;
				}
			}
			else
			{
				float num5 = Vector3.Dot(result, result3);
				if (num2 <= 1E-07f)
				{
					num3 = 0f;
					num4 = MathHelper.Clamp((0f - num5) / num, 0f, 1f);
				}
				else
				{
					float num6 = Vector3.Dot(result, result2);
					float num7 = num * num2 - num6 * num6;
					if (num7 != 0f)
					{
						num4 = (num6 * result4 - num5 * num2) / num7;
						if (num4 < 0f || num4 > 1f)
						{
							P_4 = default(Vector3);
							P_5 = default(Vector3);
							return false;
						}
					}
					else
					{
						num4 = 0.5f;
					}
					num3 = (num6 * num4 + result4) / num2;
					if (num3 < 0f || num3 > 1f)
					{
						P_4 = default(Vector3);
						P_5 = default(Vector3);
						return false;
					}
				}
			}
			Vector3.Multiply(ref result, num4, out P_4);
			Vector3.Add(ref P_4, ref P_0, out P_4);
			Vector3.Multiply(ref result2, num3, out P_5);
			Vector3.Add(ref P_5, ref P_2, out P_5);
			return true;
		}

		internal static void b8(y._6 P_0, y._6 P_1, ref Vector3 P_2, ref N._7 P_3, ref Vector3 P_4, ref N._7 P_5, bool P_6, ref Vector3 P_7, out l.W<h> P_8)
		{
			float a5h = P_0.a5h;
			float a5b = P_0.a5b;
			float a2 = P_0.a56;
			float a5h2 = P_1.a5h;
			float a5b2 = P_1.a5b;
			float a3 = P_1.a56;
			Vector3.Negate(ref P_7, out var result);
			_6b(ref P_2, ref P_3, ref result, a5h, a5b, a2, out var obj);
			_6b(ref P_4, ref P_5, ref P_7, a5h2, a5b2, a3, out var obj2);
			if (P_6)
			{
				b5(ref obj, ref obj2, ref result, out P_8);
			}
			else
			{
				b5(ref obj2, ref obj, ref P_7, out P_8);
			}
			if (P_8.Count > 4)
			{
				bC(ref P_7, P_8, out P_8);
			}
		}

		private static void bC(ref Vector3 P_0, l.W<h> P_1, out l.W<h> P_2)
		{
			int count = P_1.Count;
			P_1.Get(0, out var item);
			h item2;
			for (int i = 1; i < count; i++)
			{
				P_1.Get(i, out item2);
				if (item2.Depth > item.Depth)
				{
					item = item2;
				}
			}
			P_1.Get(0, out var item3);
			Vector3.DistanceSquared(ref item.Position, ref item3.Position, out var result);
			for (int j = 1; j < count; j++)
			{
				P_1.Get(j, out item2);
				Vector3.DistanceSquared(ref item.Position, ref item2.Position, out var result2);
				if (result2 > result)
				{
					result = result2;
					item3 = item2;
				}
			}
			Vector3.Subtract(ref item3.Position, ref item.Position, out var result3);
			Vector3.Cross(ref P_0, ref result3, out var result4);
			P_1.Get(0, out var item4);
			h item5 = item4;
			Vector3.Dot(ref item4.Position, ref result4, out var result5);
			float num = result5;
			for (int k = 1; k < count; k++)
			{
				P_1.Get(k, out item2);
				Vector3.Dot(ref result4, ref item2.Position, out var result6);
				if (result6 < result5)
				{
					result5 = result6;
					item4 = item2;
				}
				else if (result6 > num)
				{
					num = result6;
					item5 = item2;
				}
			}
			P_2 = default(l.W<h>);
			P_2.Add(ref item);
			P_2.Add(ref item3);
			P_2.Add(ref item4);
			P_2.Add(ref item5);
		}

		private static void b5(ref _00065h P_0, ref _00065h P_1, ref Vector3 P_2, out l.W<h> P_3)
		{
			P_3 = default(l.W<h>);
			Vector3.Subtract(ref P_0.V4, ref P_0.V3, out var result);
			Vector3.Subtract(ref P_0.V2, ref P_0.V3, out var result2);
			float num = 1f / P_0.Width;
			float num2 = 1f / P_0.Height;
			float num3 = num * num;
			result.X *= num3;
			result.Y *= num3;
			result.Z *= num3;
			float num4 = num2 * num2;
			result2.X *= num4;
			result2.Y *= num4;
			result2.Z *= num4;
			Vector3.Subtract(ref P_1.V4, ref P_1.V3, out var result3);
			Vector3.Subtract(ref P_1.V2, ref P_1.V3, out var result4);
			float num5 = 1f / P_1.Width;
			float num6 = 1f / P_1.Height;
			float num7 = num5 * num5;
			result3.X *= num7;
			result3.Y *= num7;
			result3.Z *= num7;
			float num8 = num6 * num6;
			result4.X *= num8;
			result4.Y *= num8;
			result4.Z *= num8;
			Vector3.Add(ref P_0.V1, ref P_0.V3, out var result5);
			Vector3.Dot(ref result5, ref result, out var result6);
			Vector3.Dot(ref result5, ref result2, out var result7);
			result6 *= 0.5f;
			result7 *= 0.5f;
			Vector3.Add(ref P_1.V1, ref P_1.V3, out var result8);
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
			Vector3.Dot(ref result, ref P_1.V1, out var result11);
			bool flag = result11 < num11;
			bool flag2 = result11 > num13;
			Vector3.Dot(ref result2, ref P_1.V1, out var result12);
			bool flag3 = result12 < num12;
			bool flag4 = result12 > num14;
			Vector3.Dot(ref result, ref P_1.V2, out result11);
			bool flag5 = result11 < num11;
			bool flag6 = result11 > num13;
			Vector3.Dot(ref result2, ref P_1.V2, out result12);
			bool flag7 = result12 < num12;
			bool flag8 = result12 > num14;
			Vector3.Dot(ref result, ref P_1.V3, out result11);
			bool flag9 = result11 < num11;
			bool flag10 = result11 > num13;
			Vector3.Dot(ref result2, ref P_1.V3, out result12);
			bool flag11 = result12 < num12;
			bool flag12 = result12 > num14;
			Vector3.Dot(ref result, ref P_1.V4, out result11);
			bool flag13 = result11 < num11;
			bool flag14 = result11 > num13;
			Vector3.Dot(ref result2, ref P_1.V4, out result12);
			bool flag15 = result12 < num12;
			bool flag16 = result12 > num14;
			Vector3.Dot(ref result3, ref P_0.V1, out result11);
			bool flag17 = result11 < num15;
			bool flag18 = result11 > num17;
			Vector3.Dot(ref result4, ref P_0.V1, out result12);
			bool flag19 = result12 < num16;
			bool flag20 = result12 > num18;
			Vector3.Dot(ref result3, ref P_0.V2, out result11);
			bool flag21 = result11 < num15;
			bool flag22 = result11 > num17;
			Vector3.Dot(ref result4, ref P_0.V2, out result12);
			bool flag23 = result12 < num16;
			bool flag24 = result12 > num18;
			Vector3.Dot(ref result3, ref P_0.V3, out result11);
			bool flag25 = result11 < num15;
			bool flag26 = result11 > num17;
			Vector3.Dot(ref result4, ref P_0.V3, out result12);
			bool flag27 = result12 < num16;
			bool flag28 = result12 > num18;
			Vector3.Dot(ref result3, ref P_0.V4, out result11);
			bool flag29 = result11 < num15;
			bool flag30 = result11 > num17;
			Vector3.Dot(ref result4, ref P_0.V4, out result12);
			bool flag31 = result12 < num16;
			bool flag32 = result12 > num18;
			h item = default(h);
			if (flag2 && flag && flag4 && flag3)
			{
				item.Position = P_1.V1;
				item.Id = P_1.Id1;
				P_3.Add(ref item);
			}
			if (flag6 && flag5 && flag8 && flag7)
			{
				item.Position = P_1.V2;
				item.Id = P_1.Id2;
				P_3.Add(ref item);
			}
			if (flag10 && flag9 && flag12 && flag11)
			{
				item.Position = P_1.V3;
				item.Id = P_1.Id3;
				P_3.Add(ref item);
			}
			if (flag14 && flag13 && flag16 && flag15)
			{
				item.Position = P_1.V4;
				item.Id = P_1.Id4;
				P_3.Add(ref item);
			}
			l.W<h> w = P_3;
			P_3.Clear();
			Vector3.Dot(ref P_0.V1, ref P_2, out var result13);
			float result14;
			for (int i = 0; i < w.Count; i++)
			{
				w.Get(i, out item);
				Vector3.Dot(ref item.Position, ref P_2, out result14);
				item.Depth = result14 - result13;
				if (item.Depth <= 0f)
				{
					P_3.Add(ref item);
				}
			}
			int count = P_3.Count;
			if (count >= 4)
			{
				return;
			}
			Vector3.Dot(ref P_1.V1, ref P_1.Normal, out var result15);
			float result16;
			Vector3 result17;
			if (flag18 && flag17 && flag20 && flag19)
			{
				Vector3.Dot(ref P_0.V1, ref P_1.Normal, out result16);
				Vector3.Multiply(ref P_1.Normal, result16 - result15, out result17);
				Vector3.Subtract(ref P_0.V1, ref result17, out result17);
				item.Position = result17;
				item.Id = P_0.Id1 + 8;
				P_3.Add(ref item);
			}
			if (flag22 && flag21 && flag24 && flag23)
			{
				Vector3.Dot(ref P_0.V2, ref P_1.Normal, out result16);
				Vector3.Multiply(ref P_1.Normal, result16 - result15, out result17);
				Vector3.Subtract(ref P_0.V2, ref result17, out result17);
				item.Position = result17;
				item.Id = P_0.Id2 + 8;
				P_3.Add(ref item);
			}
			if (flag26 && flag25 && flag28 && flag27)
			{
				Vector3.Dot(ref P_0.V3, ref P_1.Normal, out result16);
				Vector3.Multiply(ref P_1.Normal, result16 - result15, out result17);
				Vector3.Subtract(ref P_0.V3, ref result17, out result17);
				item.Position = result17;
				item.Id = P_0.Id3 + 8;
				P_3.Add(ref item);
			}
			if (flag30 && flag29 && flag32 && flag31)
			{
				Vector3.Dot(ref P_0.V4, ref P_1.Normal, out result16);
				Vector3.Multiply(ref P_1.Normal, result16 - result15, out result17);
				Vector3.Subtract(ref P_0.V4, ref result17, out result17);
				item.Position = result17;
				item.Id = P_0.Id4 + 8;
				P_3.Add(ref item);
			}
			int count2 = P_3.Count;
			w = P_3;
			for (int num19 = count2 - 1; num19 >= count; num19--)
			{
				P_3.RemoveAt(num19);
			}
			for (int j = count; j < w.Count; j++)
			{
				w.Get(j, out item);
				Vector3.Dot(ref item.Position, ref P_2, out result14);
				item.Depth = result14 - result13;
				if (item.Depth <= 0f)
				{
					P_3.Add(ref item);
				}
			}
			count = P_3.Count;
			if (count >= 4)
			{
				return;
			}
			P_0._67(0, out var obj);
			if (!flag3)
			{
				if (flag7 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag15 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag7)
			{
				if (flag3 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag11 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag11)
			{
				if (flag7 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
				if (flag15 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag15)
			{
				if (flag3 && P_3.Count < 8 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
				if (flag11 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
			}
			P_0._67(1, out obj);
			if (!flag2)
			{
				if (flag6 && P_3.Count < 8 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag14 && P_3.Count < 8 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag6)
			{
				if (flag2 && P_3.Count < 8 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag10 && P_3.Count < 8 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag10)
			{
				if (flag6 && P_3.Count < 8 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
				if (flag14 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag14)
			{
				if (flag2 && P_3.Count < 8 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
				if (flag10 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
			}
			P_0._67(2, out obj);
			if (!flag4)
			{
				if (flag8 && P_3.Count < 8 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag16 && P_3.Count < 8 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag8)
			{
				if (flag4 && P_3.Count < 8 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag12 && P_3.Count < 8 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag12)
			{
				if (flag8 && P_3.Count < 8 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
				if (flag16 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag16)
			{
				if (flag12 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
				if (flag4 && P_3.Count < 8 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
			}
			P_0._67(3, out obj);
			if (!flag)
			{
				if (flag5 && P_3.Count < 8 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag13 && P_3.Count < 8 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag5)
			{
				if (flag && P_3.Count < 8 && _6h(ref P_1.V1, ref P_1.V2, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id1, P_1.Id2, ref obj);
					P_3.Add(ref item);
				}
				if (flag9 && P_3.Count < 8 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag9)
			{
				if (flag5 && P_3.Count < 8 && _6h(ref P_1.V2, ref P_1.V3, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id2, P_1.Id3, ref obj);
					P_3.Add(ref item);
				}
				if (flag13 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
			}
			if (!flag13)
			{
				if (flag && P_3.Count < 8 && _6h(ref P_1.V4, ref P_1.V1, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id4, P_1.Id1, ref obj);
					P_3.Add(ref item);
				}
				if (flag9 && P_3.Count < 8 && _6h(ref P_1.V3, ref P_1.V4, ref obj, out result17))
				{
					item.Position = result17;
					item.Id = _66(P_1.Id3, P_1.Id4, ref obj);
					P_3.Add(ref item);
				}
			}
			count2 = P_3.Count;
			w = P_3;
			for (int num20 = count2 - 1; num20 >= count; num20--)
			{
				P_3.RemoveAt(num20);
			}
			for (int k = count; k < w.Count; k++)
			{
				w.Get(k, out item);
				Vector3.Dot(ref item.Position, ref P_2, out result14);
				item.Depth = result14 - result13;
				if (item.Depth <= 0f)
				{
					P_3.Add(ref item);
				}
			}
		}

		private static bool _6h(ref Vector3 P_0, ref Vector3 P_1, ref _00065b P_2, out Vector3 P_3)
		{
			Vector3.Subtract(ref P_2.A, ref P_0, out var result);
			Vector3.Subtract(ref P_1, ref P_0, out var result2);
			Vector3.Dot(ref result, ref P_2.Perpendicular, out var result3);
			Vector3.Dot(ref result2, ref P_2.Perpendicular, out var result4);
			float result5 = result3 / result4;
			if (result5 < 0f || result5 > 1f)
			{
				P_3 = default(Vector3);
				return false;
			}
			Vector3.Multiply(ref result2, result5, out result);
			Vector3.Add(ref result, ref P_0, out P_3);
			Vector3.Subtract(ref P_3, ref P_2.A, out result);
			Vector3.Subtract(ref P_2.B, ref P_2.A, out result2);
			Vector3.Dot(ref result2, ref result, out result5);
			if (result5 < 0f || result5 > result2.LengthSquared())
			{
				return false;
			}
			return true;
		}

		private static void _6b(ref Vector3 P_0, ref N._7 P_1, ref Vector3 P_2, float P_3, float P_4, float P_5, out _00065h P_6)
		{
			P_6 = default(_00065h);
			float num = P_1.M11 * P_2.X + P_1.M12 * P_2.Y + P_1.M13 * P_2.Z;
			float num2 = P_1.M21 * P_2.X + P_1.M22 * P_2.Y + P_1.M23 * P_2.Z;
			float num3 = P_1.M31 * P_2.X + P_1.M32 * P_2.Y + P_1.M33 * P_2.Z;
			float num4 = Math.Abs(num);
			float num5 = Math.Abs(num2);
			float num6 = Math.Abs(num3);
			N._7.ToMatrix4X4(ref P_1, out var matrix);
			matrix.M41 = P_0.X;
			matrix.M42 = P_0.Y;
			matrix.M43 = P_0.Z;
			matrix.M44 = 1f;
			if (num4 > num5 && num4 > num6)
			{
				int num7;
				if (num < 0f)
				{
					P_3 = 0f - P_3;
					num7 = 0;
				}
				else
				{
					num7 = 1;
				}
				Vector3 position = new Vector3(P_3, P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V1 = position;
				position = new Vector3(P_3, 0f - P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V2 = position;
				position = new Vector3(P_3, 0f - P_4, 0f - P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V3 = position;
				position = new Vector3(P_3, P_4, 0f - P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V4 = position;
				if (num < 0f)
				{
					P_6.Normal = P_1.Left;
				}
				else
				{
					P_6.Normal = P_1.Right;
				}
				P_6.Width = P_4 * 2f;
				P_6.Height = P_5 * 2f;
				P_6.Id1 = num7 + 2 + 4;
				P_6.Id2 = num7 + 4;
				P_6.Id3 = num7 + 2;
				P_6.Id4 = num7;
			}
			else if (num5 > num4 && num5 > num6)
			{
				int num7;
				if (num2 < 0f)
				{
					P_4 = 0f - P_4;
					num7 = 0;
				}
				else
				{
					num7 = 2;
				}
				Vector3 position = new Vector3(P_3, P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V1 = position;
				position = new Vector3(0f - P_3, P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V2 = position;
				position = new Vector3(0f - P_3, P_4, 0f - P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V3 = position;
				position = new Vector3(P_3, P_4, 0f - P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V4 = position;
				if (num2 < 0f)
				{
					P_6.Normal = P_1.Down;
				}
				else
				{
					P_6.Normal = P_1.Up;
				}
				P_6.Width = P_3 * 2f;
				P_6.Height = P_5 * 2f;
				P_6.Id1 = 1 + num7 + 4;
				P_6.Id2 = num7 + 4;
				P_6.Id3 = 1 + num7;
				P_6.Id4 = num7;
			}
			else if (num6 > num4 && num6 > num5)
			{
				int num7;
				if (num3 < 0f)
				{
					P_5 = 0f - P_5;
					num7 = 0;
				}
				else
				{
					num7 = 4;
				}
				Vector3 position = new Vector3(P_3, P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V1 = position;
				position = new Vector3(0f - P_3, P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V2 = position;
				position = new Vector3(0f - P_3, 0f - P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V3 = position;
				position = new Vector3(P_3, 0f - P_4, P_5);
				Vector3.Transform(ref position, ref matrix, out position);
				P_6.V4 = position;
				if (num3 < 0f)
				{
					P_6.Normal = P_1.Forward;
				}
				else
				{
					P_6.Normal = P_1.Backward;
				}
				P_6.Width = P_3 * 2f;
				P_6.Height = P_4 * 2f;
				P_6.Id1 = 3 + num7;
				P_6.Id2 = 2 + num7;
				P_6.Id3 = 1 + num7;
				P_6.Id4 = num7;
			}
		}

		private static int _66(int P_0, int P_1, int P_2, int P_3)
		{
			return _6a(P_0, P_1) * 2549 + _6a(P_2, P_3) * 2857;
		}

		private static int _66(int P_0, int P_1, ref _00065b P_2)
		{
			return _6a(P_0, P_1) * 2549 + P_2.Id * 2857;
		}

		private static int _6a(int P_0, int P_1)
		{
			return (P_0 + 1) * 571 + (P_1 + 1) * 577;
		}
	}
}
namespace i
{
	internal class _6 : h
	{
		protected s.B<y._6> box;

		protected s.B<y._7> sphere;

		private _0004.h a5h = new _0004.h();

		private bool a5b;

		public s.B<y._6> CollidableA => box;

		public s.B<y._7> CollidableB => sphere;

		public _6()
		{
			contacts = new l._7<_0004.h>(1);
		}

		public override void Update(float dt)
		{
			bool flag = false;
			if (I.a.AreShapesColliding(box.Shape, sphere.Shape, ref box.worldTransform, ref sphere.worldTransform.Position, out var contact))
			{
				if (!a5b && contact.PenetrationDepth >= 0f)
				{
					Add(ref contact);
					flag = true;
				}
				else if (a5b)
				{
					a5h.Normal = contact.Normal;
					a5h.PenetrationDepth = contact.PenetrationDepth;
					a5h.Position = contact.Position;
					flag = true;
				}
			}
			else if (a5b)
			{
				Remove(0);
			}
			a5b = flag;
		}

		protected override void Add(ref _0004.b contactCandidate)
		{
			a5h.Normal = contactCandidate.Normal;
			a5h.PenetrationDepth = contactCandidate.PenetrationDepth;
			a5h.Position = contactCandidate.Position;
			contacts.Add(a5h);
			OnAdded(a5h);
		}

		protected override void Remove(int index)
		{
			contacts.RemoveAt(index);
			OnRemoved(a5h);
		}

		public override void Initialize(P.h newCollidableA, P.h newCollidableB)
		{
			box = newCollidableA as s.B<y._6>;
			sphere = newCollidableB as s.B<y._7>;
			if (box == null || sphere == null)
			{
				box = newCollidableB as s.B<y._6>;
				sphere = newCollidableA as s.B<y._7>;
				if (box == null || sphere == null)
				{
					throw new Exception("Inappropriate types used to initialize pair.");
				}
			}
		}

		public override void CleanUp()
		{
			contacts.Clear();
			box = null;
			sphere = null;
			a5b = false;
			base.CleanUp();
		}

		public override void ClearContacts()
		{
			a5b = false;
			base.ClearContacts();
		}
	}
}
