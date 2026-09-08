using System;
using _0004;
using _0014;
using D;
using E;
using L;
using Microsoft.Xna.Framework;
using N;
using P;
using i;
using l;
using r;
using y;

namespace _000F
{
	internal enum v
	{
		Standing,
		Crouching
	}
}
namespace _0002
{
	internal struct v
	{
		internal D.b a5h;

		internal _0004.b a5b;

		internal P.h a56;

		internal v(P.h P_0, D.b P_1, ref _0004.b P_2)
		{
			a56 = P_0;
			a5h = P_1;
			a5b = P_2;
		}
	}
}
namespace _000E
{
	internal class v : h
	{
		private float[,] a5h;

		private B a5b;

		public float[,] Heights
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
				OnShapeChanged();
			}
		}

		public B QuadTriangleOrganization
		{
			get
			{
				return a5b;
			}
			set
			{
				a5b = value;
				OnShapeChanged();
			}
		}

		public v(float[,] heights, B triangleOrganization)
		{
			if (heights.GetLength(0) <= 1 || heights.GetLength(1) <= 1)
			{
				throw new ArgumentException("Terrains must have a least 2x2 vertices (one quad).");
			}
			a5h = heights;
			a5b = triangleOrganization;
		}

		public v(float[,] heights)
			: this(heights, B.BottomLeftUpperRight)
		{
		}

		public void GetBoundingBox(ref N.h transform, out BoundingBox boundingBox)
		{
			boundingBox = default(BoundingBox);
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			float num4 = float.MinValue;
			float num5 = float.MaxValue;
			float num6 = float.MinValue;
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			Vector3 vector3 = default(Vector3);
			Vector3 vector4 = default(Vector3);
			Vector3 vector5 = default(Vector3);
			Vector3 vector6 = default(Vector3);
			for (int i = 0; i < a5h.GetLength(0); i++)
			{
				for (int j = 0; j < a5h.GetLength(1); j++)
				{
					Vector3 result = new Vector3(i, a5h[i, j], j);
					N._7.Transform(ref result, ref transform.LinearTransform, out result);
					if (result.X < num)
					{
						num = result.X;
						vector = result;
					}
					else if (result.X > num2)
					{
						num2 = result.X;
						vector2 = result;
					}
					if (result.Y < num3)
					{
						num3 = result.Y;
						vector3 = result;
					}
					else if (result.Y > num4)
					{
						num4 = result.Y;
						vector4 = result;
					}
					if (result.Z < num5)
					{
						num5 = result.Z;
						vector5 = result;
					}
					else if (result.Z > num6)
					{
						num6 = result.Z;
						vector6 = result;
					}
				}
			}
			boundingBox.Min.X = vector.X + transform.Translation.X;
			boundingBox.Min.Y = vector3.Y + transform.Translation.Y;
			boundingBox.Min.Z = vector5.Z + transform.Translation.Z;
			boundingBox.Max.X = vector2.X + transform.Translation.X;
			boundingBox.Max.Y = vector4.Y + transform.Translation.Y;
			boundingBox.Max.Z = vector6.Z + transform.Translation.Z;
		}

		public bool RayCast(ref Ray ray, float maximumLength, ref N.h transform, out r._0006 hit)
		{
			return RayCast(ref ray, maximumLength, ref transform, y._0006.Counterclockwise, out hit);
		}

		public bool RayCast(ref Ray ray, float maximumLength, ref N.h transform, y._0006 sidedness, out r._0006 hit)
		{
			hit = default(r._0006);
			N.h.Invert(ref transform, out var inverse);
			Ray ray2 = default(Ray);
			N._7.Transform(ref ray.Direction, ref inverse.LinearTransform, out ray2.Direction);
			N.h.Transform(ref ray.Position, ref inverse, out ray2.Position);
			float num = a5h.GetLength(0) - 1;
			float num2 = a5h.GetLength(1) - 1;
			Vector3 value = ray2.Position;
			float num3 = 0f;
			if (value.X < 0f)
			{
				if (!(ray2.Direction.X > 0f))
				{
					return false;
				}
				float num4 = (0f - value.X) / ray2.Direction.X;
				num3 += num4;
				Vector3.Multiply(ref ray2.Direction, num4, out var result);
				Vector3.Add(ref result, ref value, out value);
			}
			else if (value.X > num)
			{
				if (!(ray2.Direction.X < 0f))
				{
					return false;
				}
				float num5 = (0f - (value.X - num)) / ray2.Direction.X;
				num3 += num5;
				Vector3.Multiply(ref ray2.Direction, num5, out var result2);
				Vector3.Add(ref result2, ref value, out value);
			}
			if (value.Z < 0f)
			{
				if (!(ray2.Direction.Z > 0f))
				{
					return false;
				}
				float num6 = (0f - value.Z) / ray2.Direction.Z;
				num3 += num6;
				Vector3.Multiply(ref ray2.Direction, num6, out var result3);
				Vector3.Add(ref result3, ref value, out value);
			}
			else if (value.Z > num2)
			{
				if (!(ray2.Direction.Z < 0f))
				{
					return false;
				}
				float num7 = (0f - (value.Z - num2)) / ray2.Direction.Z;
				num3 += num7;
				Vector3.Multiply(ref ray2.Direction, num7, out var result4);
				Vector3.Add(ref result4, ref value, out value);
			}
			if (num3 > maximumLength)
			{
				return false;
			}
			int num8 = (int)value.X;
			int num9 = (int)value.Z;
			if (num8 == a5h.GetLength(0) - 1 && ray2.Direction.X < 0f)
			{
				num8 = a5h.GetLength(0) - 2;
			}
			if (num9 == a5h.GetLength(1) - 1 && ray2.Direction.Z < 0f)
			{
				num9 = a5h.GetLength(1) - 2;
			}
			while (true)
			{
				if (num8 < 0 || num9 < 0 || num8 >= a5h.GetLength(0) - 1 || num9 >= a5h.GetLength(1) - 1)
				{
					return false;
				}
				GetLocalPosition(num8, num9, out var vector);
				GetLocalPosition(num8 + 1, num9, out var vector2);
				GetLocalPosition(num8, num9 + 1, out var vector3);
				GetLocalPosition(num8 + 1, num9 + 1, out var vector4);
				float num10 = vector.Y;
				float num11 = vector.Y;
				if (vector2.Y > num10)
				{
					num10 = vector2.Y;
				}
				else if (vector2.Y < num11)
				{
					num11 = vector2.Y;
				}
				if (vector3.Y > num10)
				{
					num10 = vector3.Y;
				}
				else if (vector3.Y < num11)
				{
					num11 = vector3.Y;
				}
				if (vector4.Y > num10)
				{
					num10 = vector4.Y;
				}
				else if (vector4.Y < num11)
				{
					num11 = vector4.Y;
				}
				if ((!(value.Y > num10) || !(ray2.Direction.Y > 0f)) && (!(value.Y < num11) || !(ray2.Direction.Y < 0f)))
				{
					bool flag;
					r._0006 hit2;
					bool flag2;
					r._0006 hit3;
					if (a5b == B.BottomLeftUpperRight)
					{
						flag = r.X.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref vector, ref vector2, ref vector3, out hit2);
						flag2 = r.X.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref vector2, ref vector4, ref vector3, out hit3);
					}
					else
					{
						flag = r.X.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref vector, ref vector2, ref vector4, out hit2);
						flag2 = r.X.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref vector, ref vector4, ref vector3, out hit3);
					}
					if (flag && flag2)
					{
						if (hit2.T < hit3.T)
						{
							Vector3.Multiply(ref ray.Direction, hit2.T, out hit.Location);
							Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
							N._7.TransformTranspose(ref hit2.Normal, ref inverse.LinearTransform, out hit.Normal);
							hit.T = hit2.T;
							return true;
						}
						Vector3.Multiply(ref ray.Direction, hit3.T, out hit.Location);
						Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
						N._7.TransformTranspose(ref hit3.Normal, ref inverse.LinearTransform, out hit.Normal);
						hit.T = hit3.T;
					}
					else
					{
						if (flag)
						{
							Vector3.Multiply(ref ray.Direction, hit2.T, out hit.Location);
							Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
							N._7.TransformTranspose(ref hit2.Normal, ref inverse.LinearTransform, out hit.Normal);
							hit.T = hit2.T;
							return true;
						}
						if (flag2)
						{
							Vector3.Multiply(ref ray.Direction, hit3.T, out hit.Location);
							Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
							N._7.TransformTranspose(ref hit3.Normal, ref inverse.LinearTransform, out hit.Normal);
							hit.T = hit3.T;
							return true;
						}
					}
				}
				float num12 = ((ray2.Direction.X < 0f) ? ((0f - (value.X - (float)num8)) / ray2.Direction.X) : ((!(ray.Direction.X > 0f)) ? float.MaxValue : (((float)(num8 + 1) - value.X) / ray2.Direction.X)));
				float num13 = ((ray2.Direction.Z < 0f) ? ((0f - (value.Z - (float)num9)) / ray2.Direction.Z) : ((!(ray2.Direction.Z > 0f)) ? float.MaxValue : (((float)(num9 + 1) - value.Z) / ray2.Direction.Z)));
				if (num12 < num13)
				{
					num8 = ((!(ray2.Direction.X < 0f)) ? (num8 + 1) : (num8 - 1));
					num3 += num12;
					if (num3 > maximumLength)
					{
						return false;
					}
					Vector3.Multiply(ref ray2.Direction, num12, out var result5);
					Vector3.Add(ref result5, ref value, out value);
				}
				else
				{
					num9 = ((!(ray2.Direction.Z < 0f)) ? (num9 + 1) : (num9 - 1));
					num3 += num13;
					if (num3 > maximumLength)
					{
						break;
					}
					Vector3.Multiply(ref ray2.Direction, num13, out var result6);
					Vector3.Add(ref result6, ref value, out value);
				}
			}
			return false;
		}

		public void GetLocalPosition(int i, int j, out Vector3 v)
		{
			v = default(Vector3);
			v.X = i;
			v.Y = a5h[i, j];
			v.Z = j;
		}

		public void GetPosition(int i, int j, ref N.h transform, out Vector3 position)
		{
			if (i <= 0)
			{
				i = 0;
			}
			else if (i >= a5h.GetLength(0))
			{
				i = a5h.GetLength(0) - 1;
			}
			if (j <= 0)
			{
				j = 0;
			}
			else if (j >= a5h.GetLength(1))
			{
				j = a5h.GetLength(1) - 1;
			}
			position = default(Vector3);
			position.X = i;
			position.Y = a5h[i, j];
			position.Z = j;
			N.h.Transform(ref position, ref transform, out position);
		}

		public void GetNormal(int i, int j, ref N.h transform, out Vector3 normal)
		{
			if (i <= 0)
			{
				i = 0;
			}
			else if (i >= a5h.GetLength(0))
			{
				i = a5h.GetLength(0) - 1;
			}
			if (j <= 0)
			{
				j = 0;
			}
			else if (j >= a5h.GetLength(1))
			{
				j = a5h.GetLength(1) - 1;
			}
			GetPosition(i, Math.Min(j + 1, a5h.GetLength(1) - 1), ref transform, out var position);
			GetPosition(i, Math.Max(j - 1, 0), ref transform, out var position2);
			GetPosition(Math.Min(i + 1, a5h.GetLength(0) - 1), j, ref transform, out var position3);
			GetPosition(Math.Max(i - 1, 0), j, ref transform, out var position4);
			Vector3.Subtract(ref position, ref position2, out var result);
			Vector3.Subtract(ref position3, ref position4, out normal);
			Vector3.Cross(ref result, ref normal, out normal);
			normal.Normalize();
		}

		public bool GetOverlaps(BoundingBox localSpaceBoundingBox, l._7<global::i._0006._00065b> overlappedTriangles)
		{
			int length = a5h.GetLength(0);
			int num = Math.Max((int)localSpaceBoundingBox.Min.X, 0);
			int num2 = Math.Max((int)localSpaceBoundingBox.Min.Z, 0);
			int num3 = Math.Min((int)localSpaceBoundingBox.Max.X, length - 2);
			int num4 = Math.Min((int)localSpaceBoundingBox.Max.Z, a5h.GetLength(1) - 2);
			for (int i = num; i <= num3; i++)
			{
				for (int j = num2; j <= num4; j++)
				{
					float num5 = a5h[i, j];
					float num6 = a5h[i + 1, j];
					float num7 = a5h[i, j + 1];
					float num8 = a5h[i + 1, j + 1];
					float num9 = num5;
					float num10 = num5;
					if (num6 > num9)
					{
						num9 = num6;
					}
					else if (num6 < num10)
					{
						num10 = num6;
					}
					if (num7 > num9)
					{
						num9 = num7;
					}
					else if (num7 < num10)
					{
						num10 = num7;
					}
					if (num8 > num9)
					{
						num9 = num8;
					}
					else if (num8 < num10)
					{
						num10 = num8;
					}
					if (!(localSpaceBoundingBox.Max.Y < num10) && !(localSpaceBoundingBox.Min.Y > num9))
					{
						global::i._0006._00065b item = default(global::i._0006._00065b);
						if (a5b == B.BottomLeftUpperRight)
						{
							item.A = i + j * length;
							item.B = i + 1 + j * length;
							item.C = i + (j + 1) * length;
							overlappedTriangles.Add(item);
							item.A = i + 1 + j * length;
							item.B = i + 1 + (j + 1) * length;
							item.C = i + (j + 1) * length;
							overlappedTriangles.Add(item);
						}
						else
						{
							item.A = i + j * length;
							item.B = i + 1 + j * length;
							item.C = i + 1 + (j + 1) * length;
							overlappedTriangles.Add(item);
							item.A = i + j * length;
							item.B = i + 1 + (j + 1) * length;
							item.C = i + (j + 1) * length;
							overlappedTriangles.Add(item);
						}
					}
				}
			}
			return overlappedTriangles.a5h > 0;
		}

		public bool GetOverlaps(BoundingBox localBoundingBox, l._7<int> overlappedElements)
		{
			int length = a5h.GetLength(0);
			int num = Math.Max((int)localBoundingBox.Min.X, 0);
			int num2 = Math.Max((int)localBoundingBox.Min.Z, 0);
			int num3 = Math.Min((int)localBoundingBox.Max.X, length - 2);
			int num4 = Math.Min((int)localBoundingBox.Max.Z, a5h.GetLength(1) - 2);
			for (int i = num; i <= num3; i++)
			{
				for (int j = num2; j <= num4; j++)
				{
					float num5 = a5h[i, j];
					float num6 = a5h[i + 1, j];
					float num7 = a5h[i, j + 1];
					float num8 = a5h[i + 1, j + 1];
					float num9 = num5;
					float num10 = num5;
					if (num6 > num9)
					{
						num9 = num6;
					}
					else if (num6 < num10)
					{
						num10 = num6;
					}
					if (num7 > num9)
					{
						num9 = num7;
					}
					else if (num7 < num10)
					{
						num10 = num7;
					}
					if (num8 > num9)
					{
						num9 = num8;
					}
					else if (num8 < num10)
					{
						num10 = num8;
					}
					if (!(localBoundingBox.Max.Y < num10) && !(localBoundingBox.Min.Y > num9))
					{
						int num11 = (i + j * length) * 2;
						overlappedElements.Add(num11);
						overlappedElements.Add(num11 + 1);
					}
				}
			}
			return overlappedElements.a5h > 0;
		}

		public void GetTriangle(ref global::i._0006._00065b indices, ref N.h transform, out Vector3 a, out Vector3 b, out Vector3 c)
		{
			int length = a5h.GetLength(0);
			int num = indices.A / length;
			int i = indices.A - num * length;
			int num2 = indices.B / length;
			int i2 = indices.B - num2 * length;
			int num3 = indices.C / length;
			int i3 = indices.C - num3 * length;
			GetPosition(i, num, ref transform, out a);
			GetPosition(i2, num2, ref transform, out b);
			GetPosition(i3, num3, ref transform, out c);
		}

		public void GetTriangle(int index, ref N.h transform, out Vector3 a, out Vector3 b, out Vector3 c)
		{
			int num = index / 2;
			bool flag = num * 2 == index;
			int num2 = num / a5h.GetLength(0);
			int num3 = num - num2 * a5h.GetLength(0);
			if (a5b == B.BottomLeftUpperRight)
			{
				if (flag)
				{
					GetPosition(num3, num2, ref transform, out a);
					GetPosition(num3 + 1, num2, ref transform, out b);
					GetPosition(num3, num2 + 1, ref transform, out c);
				}
				else
				{
					GetPosition(num3, num2 + 1, ref transform, out a);
					GetPosition(num3 + 1, num2 + 1, ref transform, out b);
					GetPosition(num3 + 1, num2, ref transform, out c);
				}
			}
			else if (flag)
			{
				GetPosition(num3, num2, ref transform, out a);
				GetPosition(num3 + 1, num2, ref transform, out b);
				GetPosition(num3 + 1, num2 + 1, ref transform, out c);
			}
			else
			{
				GetPosition(num3, num2, ref transform, out a);
				GetPosition(num3, num2 + 1, ref transform, out b);
				GetPosition(num3 + 1, num2 + 1, ref transform, out c);
			}
		}
	}
}
namespace _0017
{
	internal struct v
	{
		public Vector3 A;

		public Vector3 B;

		public Vector3 C;

		public Vector3 D;

		public b State;

		public bool GetPointClosestToOrigin(out Vector3 point)
		{
			switch (State)
			{
			case b.Point:
				point = A;
				break;
			case b.Segment:
				GetPointOnSegmentClosestToOrigin(out point);
				break;
			case b.Triangle:
				GetPointOnTriangleClosestToOrigin(out point);
				break;
			case b.Tetrahedron:
				return GetPointOnTetrahedronClosestToOrigin(out point);
			default:
				point = r.X.ZeroVector;
				break;
			}
			return false;
		}

		public void GetPointOnSegmentClosestToOrigin(out Vector3 point)
		{
			Vector3.Subtract(ref B, ref A, out var result);
			Vector3.Dot(ref result, ref A, out var result2);
			float scaleFactor = (0f - result2) / result.LengthSquared();
			Vector3.Multiply(ref result, scaleFactor, out point);
			Vector3.Add(ref point, ref A, out point);
		}

		public void GetPointOnTriangleClosestToOrigin(out Vector3 point)
		{
			Vector3.Subtract(ref B, ref A, out var result);
			Vector3.Subtract(ref C, ref A, out var result2);
			Vector3.Dot(ref result, ref C, out var result3);
			Vector3.Dot(ref result2, ref C, out var result4);
			result3 = 0f - result3;
			result4 = 0f - result4;
			if (result4 >= 0f && result3 <= result4)
			{
				State = b.Point;
				A = C;
				point = A;
				return;
			}
			Vector3.Dot(ref result, ref A, out var result5);
			Vector3.Dot(ref result2, ref A, out var result6);
			result5 = 0f - result5;
			result6 = 0f - result6;
			float num = result3 * result6 - result5 * result4;
			if (num <= 0f && result6 > 0f && result4 < 0f)
			{
				State = b.Segment;
				B = C;
				float scaleFactor = result6 / (result6 - result4);
				Vector3.Multiply(ref result2, scaleFactor, out point);
				Vector3.Add(ref point, ref A, out point);
				return;
			}
			Vector3.Dot(ref result, ref B, out var result7);
			Vector3.Dot(ref result2, ref B, out var result8);
			result7 = 0f - result7;
			result8 = 0f - result8;
			float num2 = result7 * result4 - result3 * result8;
			float num3;
			float num4;
			if (num2 <= 0f && (num3 = result8 - result7) > 0f && (num4 = result3 - result4) > 0f)
			{
				State = b.Segment;
				A = C;
				float scaleFactor2 = num3 / (num3 + num4);
				Vector3.Subtract(ref C, ref B, out var result9);
				Vector3.Multiply(ref result9, scaleFactor2, out point);
				Vector3.Add(ref point, ref B, out point);
			}
			else
			{
				float num5 = result5 * result8 - result7 * result6;
				float num6 = 1f / (num2 + num + num5);
				float scaleFactor3 = num * num6;
				float scaleFactor4 = num5 * num6;
				Vector3.Multiply(ref result, scaleFactor3, out point);
				Vector3.Multiply(ref result2, scaleFactor4, out var result10);
				Vector3.Add(ref A, ref point, out point);
				Vector3.Add(ref point, ref result10, out point);
			}
		}

		public bool GetPointOnTetrahedronClosestToOrigin(out Vector3 point)
		{
			v v2 = default(v);
			point = default(Vector3);
			float num = float.MaxValue;
			if (_6X(ref A, ref C, ref D, ref B, out var v3, out var vector))
			{
				point = vector;
				v2 = v3;
				num = vector.LengthSquared();
			}
			float num2;
			if (_6X(ref C, ref B, ref D, ref A, out v3, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				v2 = v3;
				num = num2;
			}
			if (_6X(ref B, ref A, ref D, ref C, out v3, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				v2 = v3;
				num = num2;
			}
			if (num < float.MaxValue)
			{
				this = v2;
				return false;
			}
			return true;
		}

		private static bool _6X(ref Vector3 P_0, ref Vector3 P_1, ref Vector3 P_2, ref Vector3 P_3, out v P_4, out Vector3 P_5)
		{
			P_4 = default(v);
			P_5 = default(Vector3);
			Vector3.Subtract(ref P_1, ref P_0, out var result);
			Vector3.Subtract(ref P_2, ref P_0, out var result2);
			Vector3.Cross(ref result, ref result2, out var result3);
			Vector3.Subtract(ref P_3, ref P_0, out var result4);
			Vector3.Dot(ref P_0, ref result3, out var result5);
			Vector3.Dot(ref result4, ref result3, out var result6);
			if (result5 * result6 > 0f)
			{
				Vector3.Dot(ref result, ref P_2, out var result7);
				Vector3.Dot(ref result2, ref P_2, out var result8);
				result7 = 0f - result7;
				result8 = 0f - result8;
				if (result8 >= 0f && result7 <= result8)
				{
					P_4.State = b.Point;
					P_4.A = P_2;
					P_5 = P_2;
					return true;
				}
				Vector3.Dot(ref result, ref P_0, out var result9);
				Vector3.Dot(ref result2, ref P_0, out var result10);
				result9 = 0f - result9;
				result10 = 0f - result10;
				float num = result7 * result10 - result9 * result8;
				if (num <= 0f && result10 > 0f && result8 < 0f)
				{
					P_4.State = b.Segment;
					P_4.A = P_0;
					P_4.B = P_2;
					float scaleFactor = result10 / (result10 - result8);
					Vector3.Multiply(ref result2, scaleFactor, out P_5);
					Vector3.Add(ref P_5, ref P_0, out P_5);
					return true;
				}
				Vector3.Dot(ref result, ref P_1, out var result11);
				Vector3.Dot(ref result2, ref P_1, out var result12);
				result11 = 0f - result11;
				result12 = 0f - result12;
				float num2 = result11 * result8 - result7 * result12;
				float num3;
				float num4;
				if (num2 <= 0f && (num3 = result12 - result11) > 0f && (num4 = result7 - result8) > 0f)
				{
					P_4.State = b.Segment;
					P_4.A = P_1;
					P_4.B = P_2;
					float scaleFactor2 = num3 / (num3 + num4);
					Vector3.Subtract(ref P_2, ref P_1, out var result13);
					Vector3.Multiply(ref result13, scaleFactor2, out P_5);
					Vector3.Add(ref P_5, ref P_1, out P_5);
					return true;
				}
				float num5 = result9 * result12 - result11 * result10;
				P_4.A = P_0;
				P_4.B = P_1;
				P_4.C = P_2;
				P_4.State = b.Triangle;
				float num6 = 1f / (num2 + num + num5);
				float scaleFactor3 = num5 * num6;
				float scaleFactor4 = num * num6;
				Vector3.Multiply(ref result, scaleFactor4, out P_5);
				Vector3.Multiply(ref result2, scaleFactor3, out var result14);
				Vector3.Add(ref P_0, ref P_5, out P_5);
				Vector3.Add(ref P_5, ref result14, out P_5);
				return true;
			}
			return false;
		}

		public void AddNewSimplexPoint(ref Vector3 point)
		{
			switch (State)
			{
			case b.Empty:
				State = b.Point;
				A = point;
				break;
			case b.Point:
				State = b.Segment;
				B = point;
				break;
			case b.Segment:
				State = b.Triangle;
				C = point;
				break;
			case b.Triangle:
				State = b.Tetrahedron;
				D = point;
				break;
			}
		}

		public float GetErrorTolerance()
		{
			return State switch
			{
				b.Point => A.LengthSquared(), 
				b.Segment => MathHelper.Max(A.LengthSquared(), B.LengthSquared()), 
				b.Triangle => MathHelper.Max(A.LengthSquared(), MathHelper.Max(B.LengthSquared(), C.LengthSquared())), 
				b.Tetrahedron => MathHelper.Max(A.LengthSquared(), MathHelper.Max(B.LengthSquared(), MathHelper.Max(C.LengthSquared(), D.LengthSquared()))), 
				_ => 1f, 
			};
		}
	}
}
namespace _0010
{
	internal class v : L.h
	{
		private new _7 a5h;

		internal Vector2 a5b;

		internal N._6 a56;

		internal N._6 a5a;

		private int a57;

		private float a5_0006;

		internal N._6 a5v;

		private E.h a5B;

		private E.h a5X;

		private bool a5_0018;

		private bool a5W;

		private Vector3 a5_0002;

		private Vector3 a5_000E;

		private N.b a5y;

		internal Vector3 a5r;

		internal Vector3 a5_0001;

		public _7 ContactManifoldConstraint => a5h;

		public Vector3 FrictionDirectionX => new Vector3(a5v.M11, a5v.M12, a5v.M13);

		public Vector3 FrictionDirectionY => new Vector3(a5v.M21, a5v.M22, a5v.M23);

		public Vector2 TotalForce => a5b;

		public Vector2 RelativeVelocity
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				if (a5B != null)
				{
					num = a5B.a5a.X + a5B.a5_0006.Y * a5_0002.Z - a5B.a5_0006.Z * a5_0002.Y;
					num2 = a5B.a5a.Y + a5B.a5_0006.Z * a5_0002.X - a5B.a5_0006.X * a5_0002.Z;
					num3 = a5B.a5a.Z + a5B.a5_0006.X * a5_0002.Y - a5B.a5_0006.Y * a5_0002.X;
				}
				if (a5X != null)
				{
					num += 0f - a5X.a5a.X - a5X.a5_0006.Y * a5_000E.Z + a5X.a5_0006.Z * a5_000E.Y;
					num2 += 0f - a5X.a5a.Y - a5X.a5_0006.Z * a5_000E.X + a5X.a5_0006.X * a5_000E.Z;
					num3 += 0f - a5X.a5a.Z - a5X.a5_0006.X * a5_000E.Y + a5X.a5_0006.Y * a5_000E.X;
				}
				return new Vector2
				{
					X = num * a5v.M11 + num2 * a5v.M12 + num3 * a5v.M13,
					Y = num * a5v.M21 + num2 * a5v.M22 + num3 * a5v.M23
				};
			}
		}

		public v()
		{
			isActive = false;
		}

		public override float SolveIteration()
		{
			Vector2 relativeVelocity = RelativeVelocity;
			float num = relativeVelocity.X;
			relativeVelocity.X = num * a5y.M11 + relativeVelocity.Y * a5y.M21;
			relativeVelocity.Y = num * a5y.M12 + relativeVelocity.Y * a5y.M22;
			Vector2 vector = a5b;
			a5b.X += relativeVelocity.X;
			a5b.Y += relativeVelocity.Y;
			float num2 = a5b.LengthSquared();
			float num3 = 0f;
			for (int i = 0; i < a57; i++)
			{
				num3 += a5h.a56.Elements[i].a5b;
			}
			num3 *= a5_0006;
			if (num2 > num3 * num3)
			{
				num2 = num3 / (float)Math.Sqrt(num2);
				a5b.X *= num2;
				a5b.Y *= num2;
			}
			relativeVelocity.X = a5b.X - vector.X;
			relativeVelocity.Y = a5b.Y - vector.Y;
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			impulse.X = relativeVelocity.X * a5v.M11 + relativeVelocity.Y * a5v.M21;
			impulse.Y = relativeVelocity.X * a5v.M12 + relativeVelocity.Y * a5v.M22;
			impulse.Z = relativeVelocity.X * a5v.M13 + relativeVelocity.Y * a5v.M23;
			if (a5_0018)
			{
				impulse2.X = relativeVelocity.X * a56.M11 + relativeVelocity.Y * a56.M21;
				impulse2.Y = relativeVelocity.X * a56.M12 + relativeVelocity.Y * a56.M22;
				impulse2.Z = relativeVelocity.X * a56.M13 + relativeVelocity.Y * a56.M23;
				a5B.ApplyLinearImpulse(ref impulse);
				a5B.ApplyAngularImpulse(ref impulse2);
			}
			if (a5W)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				impulse2.X = relativeVelocity.X * a5a.M11 + relativeVelocity.Y * a5a.M21;
				impulse2.Y = relativeVelocity.X * a5a.M12 + relativeVelocity.Y * a5a.M22;
				impulse2.Z = relativeVelocity.X * a5a.M13 + relativeVelocity.Y * a5a.M23;
				a5X.ApplyLinearImpulse(ref impulse);
				a5X.ApplyAngularImpulse(ref impulse2);
			}
			return Math.Abs(relativeVelocity.X) + Math.Abs(relativeVelocity.Y);
		}

		public override void Update(float dt)
		{
			a57 = a5h.a56.a5h;
			switch (a57)
			{
			case 1:
				a5r = a5h.a56.Elements[0].a5h.Position;
				break;
			case 2:
				Vector3.Add(ref a5h.a56.Elements[0].a5h.Position, ref a5h.a56.Elements[1].a5h.Position, out a5r);
				a5r.X *= 0.5f;
				a5r.Y *= 0.5f;
				a5r.Z *= 0.5f;
				break;
			case 3:
				Vector3.Add(ref a5h.a56.Elements[0].a5h.Position, ref a5h.a56.Elements[1].a5h.Position, out a5r);
				Vector3.Add(ref a5h.a56.Elements[2].a5h.Position, ref a5r, out a5r);
				a5r.X *= 1f / 3f;
				a5r.Y *= 1f / 3f;
				a5r.Z *= 1f / 3f;
				break;
			case 4:
				Vector3.Add(ref a5h.a56.Elements[0].a5h.Position, ref a5h.a56.Elements[1].a5h.Position, out a5r);
				Vector3.Add(ref a5h.a56.Elements[2].a5h.Position, ref a5r, out a5r);
				Vector3.Add(ref a5h.a56.Elements[3].a5h.Position, ref a5r, out a5r);
				a5r.X *= 0.25f;
				a5r.Y *= 0.25f;
				a5r.Z *= 0.25f;
				break;
			default:
				a5r = r.X.NoVector;
				break;
			}
			Vector3 result;
			if (a5B != null)
			{
				Vector3.Subtract(ref a5r, ref a5B.a5h, out a5_0002);
				Vector3.Cross(ref a5B.a5_0006, ref a5_0002, out result);
				Vector3.Add(ref result, ref a5B.a5a, out result);
			}
			else
			{
				result = default(Vector3);
			}
			Vector3 result2;
			if (a5X != null)
			{
				Vector3.Subtract(ref a5r, ref a5X.a5h, out a5_000E);
				Vector3.Cross(ref a5X.a5_0006, ref a5_000E, out result2);
				Vector3.Add(ref result2, ref a5X.a5a, out result2);
			}
			else
			{
				result2 = default(Vector3);
			}
			Vector3.Subtract(ref result, ref result2, out a5_0001);
			Vector3 vector = a5h.a56.Elements[0].a5h.Normal;
			float num = vector.X * a5_0001.X + vector.Y * a5_0001.Y + vector.Z * a5_0001.Z;
			a5_0001.X -= num * vector.X;
			a5_0001.Y -= num * vector.Y;
			a5_0001.Z -= num * vector.Z;
			float num2 = a5_0001.LengthSquared();
			if (num2 > 1E-07f)
			{
				num2 = (float)Math.Sqrt(num2);
				float num3 = 1f / num2;
				a5v.M11 = a5_0001.X * num3;
				a5v.M12 = a5_0001.Y * num3;
				a5v.M13 = a5_0001.Z * num3;
				a5_0006 = ((num2 > _0014.b.StaticFrictionVelocityThreshold) ? ((b)a5h).a5h.KineticFriction : ((b)a5h).a5h.StaticFriction);
			}
			else
			{
				a5_0006 = ((b)a5h).a5h.StaticFriction;
				if (a5v.M11 == 0f && a5v.M12 == 0f && a5v.M13 == 0f)
				{
					Vector3.Cross(ref vector, ref r.X.RightVector, out var result3);
					num2 = result3.LengthSquared();
					if (num2 > 1E-07f)
					{
						num2 = (float)Math.Sqrt(num2);
						float num4 = 1f / num2;
						a5v.M11 = result3.X * num4;
						a5v.M12 = result3.Y * num4;
						a5v.M13 = result3.Z * num4;
					}
					else
					{
						Vector3.Cross(ref vector, ref r.X.UpVector, out result3);
						result3.Normalize();
						a5v.M11 = result3.X;
						a5v.M12 = result3.Y;
						a5v.M13 = result3.Z;
					}
				}
			}
			a5v.M21 = a5v.M12 * vector.Z - a5v.M13 * vector.Y;
			a5v.M22 = a5v.M13 * vector.X - a5v.M11 * vector.Z;
			a5v.M23 = a5v.M11 * vector.Y - a5v.M12 * vector.X;
			if (a5B != null)
			{
				a56.M11 = a5_0002.Y * a5v.M13 - a5_0002.Z * a5v.M12;
				a56.M12 = a5_0002.Z * a5v.M11 - a5_0002.X * a5v.M13;
				a56.M13 = a5_0002.X * a5v.M12 - a5_0002.Y * a5v.M11;
				a56.M21 = a5_0002.Y * a5v.M23 - a5_0002.Z * a5v.M22;
				a56.M22 = a5_0002.Z * a5v.M21 - a5_0002.X * a5v.M23;
				a56.M23 = a5_0002.X * a5v.M22 - a5_0002.Y * a5v.M21;
			}
			if (a5X != null)
			{
				a5a.M11 = a5v.M12 * a5_000E.Z - a5v.M13 * a5_000E.Y;
				a5a.M12 = a5v.M13 * a5_000E.X - a5v.M11 * a5_000E.Z;
				a5a.M13 = a5v.M11 * a5_000E.Y - a5v.M12 * a5_000E.X;
				a5a.M21 = a5v.M22 * a5_000E.Z - a5v.M23 * a5_000E.Y;
				a5a.M22 = a5v.M23 * a5_000E.X - a5v.M21 * a5_000E.Z;
				a5a.M23 = a5v.M21 * a5_000E.Y - a5v.M22 * a5_000E.X;
			}
			N._6 result4;
			N.a result5;
			N.b result6;
			if (a5_0018)
			{
				N._6.Multiply(ref a56, ref a5B.a5_0018, out result4);
				N._6.Transpose(ref a56, out result5);
				N.b.Multiply(ref result4, ref result5, out result6);
				result6.M11 += a5B.a5r;
				result6.M22 += a5B.a5r;
			}
			else
			{
				result6 = default(N.b);
			}
			N.b result7;
			if (a5W)
			{
				N._6.Multiply(ref a5a, ref a5X.a5_0018, out result4);
				N._6.Transpose(ref a5a, out result5);
				N.b.Multiply(ref result4, ref result5, out result7);
				result7.M11 += a5X.a5r;
				result7.M22 += a5X.a5r;
			}
			else
			{
				result7 = default(N.b);
			}
			a5y.M11 = 0f - result6.M11 - result7.M11;
			a5y.M12 = 0f - result6.M12 - result7.M12;
			a5y.M21 = 0f - result6.M21 - result7.M21;
			a5y.M22 = 0f - result6.M22 - result7.M22;
			N.b.Invert(ref a5y, out a5y);
		}

		public override void ExclusiveUpdate()
		{
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			impulse.X = a5b.X * a5v.M11 + a5b.Y * a5v.M21;
			impulse.Y = a5b.X * a5v.M12 + a5b.Y * a5v.M22;
			impulse.Z = a5b.X * a5v.M13 + a5b.Y * a5v.M23;
			if (a5_0018)
			{
				impulse2.X = a5b.X * a56.M11 + a5b.Y * a56.M21;
				impulse2.Y = a5b.X * a56.M12 + a5b.Y * a56.M22;
				impulse2.Z = a5b.X * a56.M13 + a5b.Y * a56.M23;
				a5B.ApplyLinearImpulse(ref impulse);
				a5B.ApplyAngularImpulse(ref impulse2);
			}
			if (a5W)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				impulse2.X = a5b.X * a5a.M11 + a5b.Y * a5a.M21;
				impulse2.Y = a5b.X * a5a.M12 + a5b.Y * a5a.M22;
				impulse2.Z = a5b.X * a5a.M13 + a5b.Y * a5a.M23;
				a5X.ApplyLinearImpulse(ref impulse);
				a5X.ApplyAngularImpulse(ref impulse2);
			}
		}

		internal void _6E(_7 P_0)
		{
			a5h = P_0;
			isActive = true;
			a5v = default(N._6);
			a5B = P_0.EntityA;
			a5X = P_0.EntityB;
			a5_0018 = a5B != null && a5B.a5B;
			a5W = a5X != null && a5X.a5B;
		}

		internal void bV()
		{
			a5b = default(Vector2);
			a5h = null;
			a5B = null;
			a5X = null;
			isActive = false;
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (a5B != null)
			{
				outputInvolvedEntities.Add(a5B);
			}
			if (a5X != null)
			{
				outputInvolvedEntities.Add(a5X);
			}
		}
	}
}
namespace _0001
{
	internal interface v : h, global::r.h
	{
		void Update(float dt);
	}
}
