using System;
using Microsoft.Xna.Framework;

namespace DataContent;

public class MyMath
{
	public static Random m_Rand = new Random(7);

	private static Vector3 vecP1 = Vector3.Zero;

	private static Vector3 vecP2 = Vector3.Zero;

	private static Vector3 vecP3 = Vector3.Zero;

	public static Vector3[] PositionList = null;

	public static int numberIntSegTri = 0;

	private static Vector3 end = Vector3.Zero;

	private static Vector3 s = Vector3.Zero;

	private static Vector3 ab = Vector3.Zero;

	private static Vector3 ac = Vector3.Zero;

	private static Vector3 qp = Vector3.Zero;

	private static Vector3 n = Vector3.Zero;

	private static Vector3 ap = Vector3.Zero;

	private static Vector3 e = Vector3.Zero;

	private static Vector3 tmpVecA = Vector3.Zero;

	private static Vector3 bp = Vector3.Zero;

	private static Vector3 cp = Vector3.Zero;

	private static Vector4 tmpCol0 = Vector4.Zero;

	private static Vector4 tmpCol1 = Vector4.Zero;

	private static Vector4 tmpCol2 = Vector4.Zero;

	private static Vector3 boxcenter = Vector3.Zero;

	private static Vector3 boxhalfsize = Vector3.Zero;

	private static Vector3 v0 = Vector3.Zero;

	private static Vector3 v1 = Vector3.Zero;

	private static Vector3 v2 = Vector3.Zero;

	private static Vector3 axis = Vector3.Zero;

	private static Vector3 normal = Vector3.Zero;

	private static Vector3 e0 = Vector3.Zero;

	private static Vector3 e1 = Vector3.Zero;

	private static Vector3 e2 = Vector3.Zero;

	private static Vector3 vmin = Vector3.Zero;

	private static Vector3 vmax = Vector3.Zero;

	private static Vector3 rab = Vector3.Zero;

	private static Vector3 rac = Vector3.Zero;

	private static Vector3 rap = Vector3.Zero;

	private static Vector3 rbp = Vector3.Zero;

	private static Vector3 rcp = Vector3.Zero;

	public static bool CompareVector2(ref Vector2 v1, ref Vector2 v2, float tolerance)
	{
		if (v1.X == v2.X && v1.Y == v2.Y)
		{
			return true;
		}
		return false;
	}

	public static bool CompareVector3(ref Vector3 v1, ref Vector3 v2, float tolerance)
	{
		if (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z)
		{
			return true;
		}
		return false;
	}

	public static Vector3 Vec3Lerp(Vector3 cur, Vector3 dst, float time)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = cur.X + time * (dst.X - cur.X);
		zero.Y = cur.Y + time * (dst.Y - cur.Y);
		zero.Z = cur.Z + time * (dst.Z - cur.Z);
		return zero;
	}

	public static float fltLerp(float cur, float dst, float time)
	{
		return cur + time * (dst - cur);
	}

	public static Vector3 NormalFromARGB32(uint inNorm)
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		result.X = 2f * ((float)((inNorm >> 16) & 0xFF) / 255f) - 1f;
		result.Y = 2f * ((float)((inNorm >> 8) & 0xFF) / 255f) - 1f;
		result.Z = 2f * ((float)(inNorm & 0xFF) / 255f) - 1f;
		return result;
	}

	public static float NormalWFromARGB32(uint inNorm)
	{
		float num = 0f;
		return 2f * ((float)((inNorm >> 24) & 0xFF) / 255f) - 1f;
	}

	public static float HeightScalarFromARGB32(uint inNorm)
	{
		float num = 0f;
		return (inNorm >> 24) & 0xFF;
	}

	public static bool IntersectSegmentTriangle(ref Vector3 origin, ref Vector3 dir, ref TriangleData triangle, ref float lineParameter)
	{
		numberIntSegTri++;
		lineParameter = 1000000f;
		end = origin + dir * 5000f;
		float num = origin.X * triangle.Normal.X + origin.Y * triangle.Normal.Y + origin.Z * triangle.Normal.Z + triangle.Distance;
		if (num < 0f)
		{
			return false;
		}
		float num2 = end.X * triangle.Normal.X + end.Y * triangle.Normal.Y + end.Z * triangle.Normal.Z + triangle.Distance;
		if (num2 >= 0f)
		{
			return false;
		}
		float num3 = num - num2;
		float num4 = num / num3;
		s = origin + num4 * (end - origin);
		float num5 = s.X * triangle.NormalEdge1.X + s.Y * triangle.NormalEdge1.Y + s.Z * triangle.NormalEdge1.Z - triangle.DistEdge1;
		if (num5 < 0f || num5 > 1f)
		{
			return false;
		}
		float num6 = s.X * triangle.NormalEdge2.X + s.Y * triangle.NormalEdge2.Y + s.Z * triangle.NormalEdge2.Z - triangle.DistEdge2;
		if (num6 < 0f)
		{
			return false;
		}
		float num7 = 1f - num5 - num6;
		if (num7 < 0f)
		{
			return false;
		}
		lineParameter = (origin - s).Length();
		return true;
	}

	public static bool IntersectSegmentTriangle(ref IntersectSegmentParams segment, ref TriangleData triangle)
	{
		numberIntSegTri++;
		float num = segment.SegmentStart.X * triangle.Normal.X + segment.SegmentStart.Y * triangle.Normal.Y + segment.SegmentStart.Z * triangle.Normal.Z + triangle.Distance;
		if (num < 0f)
		{
			return false;
		}
		float num2 = segment.SegmentEnd.X * triangle.Normal.X + segment.SegmentEnd.Y * triangle.Normal.Y + segment.SegmentEnd.Z * triangle.Normal.Z + triangle.Distance;
		if (num2 >= 0f)
		{
			return false;
		}
		float num3 = num - num2;
		segment.Tparameter = num / num3;
		segment.hitPosition = segment.SegmentStart + segment.Tparameter * (segment.SegmentEnd - segment.SegmentStart);
		float num4 = segment.hitPosition.X * triangle.NormalEdge1.X + segment.hitPosition.Y * triangle.NormalEdge1.Y + segment.hitPosition.Z * triangle.NormalEdge1.Z - triangle.DistEdge1;
		if (num4 < 0f || num4 > 1f)
		{
			return false;
		}
		float num5 = segment.hitPosition.X * triangle.NormalEdge2.X + segment.hitPosition.Y * triangle.NormalEdge2.Y + segment.hitPosition.Z * triangle.NormalEdge2.Z - triangle.DistEdge2;
		if (num5 < 0f)
		{
			return false;
		}
		float num6 = 1f - num4 - num5;
		if (num6 < 0f)
		{
			return false;
		}
		segment.hitNormal = triangle.Normal;
		return true;
	}

	public static bool IntersectRayTriangle(ref Vector3 origin, ref Vector3 dir, ref TriangleData triangle, ref float lineParameter)
	{
		vecP1 = PositionList[triangle.p1];
		vecP2 = PositionList[triangle.p2];
		vecP3 = PositionList[triangle.p3];
		ab = vecP2 - vecP1;
		ac = vecP3 - vecP1;
		qp = origin - (origin + dir * 1000f);
		n.X = ab.Y * ac.Z - ab.Z * ac.Y;
		n.Y = ab.Z * ac.X - ab.X * ac.Z;
		n.Z = ab.X * ac.Y - ab.Y * ac.X;
		float num = qp.X * n.X + qp.Y * n.Y + qp.Z * n.Z;
		if (num <= 0f)
		{
			return false;
		}
		ap = origin - vecP1;
		float num2 = ap.X * n.X + ap.Y * n.Y + ap.Z * n.Z;
		if (num2 < 0f)
		{
			return false;
		}
		e.X = qp.Y * ap.Z - qp.Z * ap.Y;
		e.Y = qp.Z * ap.X - qp.X * ap.Z;
		e.Z = qp.X * ap.Y - qp.Y * ap.X;
		float num3 = ac.X * e.X + ac.Y * e.Y + ac.Z * e.Z;
		if (num3 < 0f || num3 > num)
		{
			return false;
		}
		float num4 = 0f - (ab.X * e.X + ab.Y * e.Y + ab.Z * e.Z);
		if (num4 < 0f || num3 + num4 > num)
		{
			return false;
		}
		float num5 = 1f / num;
		num2 *= num5;
		num3 *= num5;
		num4 *= num5;
		float num6 = 1f - num3 - num4;
		lineParameter = (origin - (vecP1 + num6 * ab + num3 * ac)).Length();
		return true;
	}

	public static bool IntersectRayTriangle(ref Vector3 origin, ref Vector3 dir, ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref float lineParameter)
	{
		lineParameter = 1000000f;
		Vector3 vector = v1 - v0;
		Vector3 vector2 = v2 - v0;
		Vector3 vector3 = Vector3.Cross(dir, vector2);
		float num = Vector3.Dot(vector, vector3);
		if (Math.Abs(num) < 1E-06f)
		{
			return false;
		}
		float num2 = 1f / num;
		Vector3 vector4 = origin - v0;
		float num3 = num2 * Vector3.Dot(vector4, vector3);
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		Vector3 vector5 = Vector3.Cross(vector4, vector);
		float num4 = num2 * Vector3.Dot(dir, vector5);
		if (num4 < 0f || num4 + num3 > 1f)
		{
			return false;
		}
		lineParameter = num2 * Vector3.Dot(vector2, vector5);
		return lineParameter > 0f;
	}

	public static bool IntersectSphereTriangle(ref Vector3 origin, float radiusSqr, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 closestPoint, ref float disSqr)
	{
		Vector3 vector = p2 - p1;
		Vector3 vector2 = p3 - p1;
		Vector3 vector3 = Vector3.Cross(vector2, vector);
		vector3.Normalize();
		tmpVecA = p1 - origin;
		float num = Vector3.Dot(vector3, tmpVecA);
		if (num * num > radiusSqr)
		{
			return false;
		}
		ClosestPTPointTriangle(ref origin, ref p1, ref p2, ref p3, ref closestPoint);
		closestPoint.X = origin.X - closestPoint.X;
		closestPoint.Y = origin.Y - closestPoint.Y;
		closestPoint.Z = origin.Z - closestPoint.Z;
		disSqr = closestPoint.LengthSquared();
		return disSqr <= radiusSqr;
	}

	public static bool IntersectSphereTriangle(ref Vector3 origin, float radiusSqr, ref TriangleData triangle, ref Vector3 closestPoint, ref float disSqr)
	{
		vecP1 = PositionList[triangle.p1];
		vecP2 = PositionList[triangle.p2];
		vecP3 = PositionList[triangle.p3];
		tmpVecA = vecP1 - origin;
		float num = Vector3.Dot(triangle.Normal, tmpVecA);
		if (num * num > radiusSqr)
		{
			return false;
		}
		ClosestPTPointTriangle(ref origin, ref vecP1, ref vecP2, ref vecP3, ref closestPoint);
		closestPoint.X = origin.X - closestPoint.X;
		closestPoint.Y = origin.Y - closestPoint.Y;
		closestPoint.Z = origin.Z - closestPoint.Z;
		disSqr = closestPoint.LengthSquared();
		return disSqr <= radiusSqr;
	}

	public static bool IntersectSphereTriangle(ref Vector3 origin, float radiusSqr, ref TriangleData triangle, ref Vector3 closestPoint, ref float disSqr, Vector3[] pointList)
	{
		vecP1 = pointList[triangle.p1];
		vecP2 = pointList[triangle.p2];
		vecP3 = pointList[triangle.p3];
		tmpVecA = vecP1 - origin;
		float num = Vector3.Dot(triangle.Normal, tmpVecA);
		if (num * num > radiusSqr)
		{
			return false;
		}
		ClosestPTPointTriangle(ref origin, ref vecP1, ref vecP2, ref vecP3, ref closestPoint);
		closestPoint.X = origin.X - closestPoint.X;
		closestPoint.Y = origin.Y - closestPoint.Y;
		closestPoint.Z = origin.Z - closestPoint.Z;
		disSqr = closestPoint.LengthSquared();
		return disSqr <= radiusSqr;
	}

	public static void ClosestPTPointTriangle(ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 closestPoint)
	{
		ab.X = b.X - a.X;
		ab.Y = b.Y - a.Y;
		ab.Z = b.Z - a.Z;
		ac.X = c.X - a.X;
		ac.Y = c.Y - a.Y;
		ac.Z = c.Z - a.Z;
		ap.X = p.X - a.X;
		ap.Y = p.Y - a.Y;
		ap.Z = p.Z - a.Z;
		float num = ab.X * ap.X + ab.Y * ap.Y + ab.Z * ap.Z;
		float num2 = ac.X * ap.X + ac.Y * ap.Y + ac.Z * ap.Z;
		if (num <= 0f && num2 <= 0f)
		{
			closestPoint = a;
			return;
		}
		bp.X = p.X - b.X;
		bp.Y = p.Y - b.Y;
		bp.Z = p.Z - b.Z;
		float num3 = ab.X * bp.X + ab.Y * bp.Y + ab.Z * bp.Z;
		float num4 = ac.X * bp.X + ac.Y * bp.Y + ac.Z * bp.Z;
		if (num3 >= 0f && num4 <= num3)
		{
			closestPoint = b;
			return;
		}
		float num5 = num * num4 - num3 * num2;
		if (num5 <= 0f && num >= 0f && num3 <= 0f)
		{
			float num6 = num / (num - num3);
			closestPoint = a + num6 * ab;
			return;
		}
		cp.X = p.X - c.X;
		cp.Y = p.Y - c.Y;
		cp.Z = p.Z - c.Z;
		float num7 = ab.X * cp.X + ab.Y * cp.Y + ab.Z * cp.Z;
		float num8 = ac.X * cp.X + ac.Y * cp.Y + ac.Z * cp.Z;
		if (num8 >= 0f && num7 <= num8)
		{
			closestPoint = c;
			return;
		}
		float num9 = num7 * num2 - num * num8;
		if (num9 <= 0f && num2 >= 0f && num8 <= 0f)
		{
			float num10 = num2 / (num2 - num8);
			closestPoint = a + num10 * ac;
			return;
		}
		float num11 = num3 * num8 - num7 * num4;
		if (num11 <= 0f && num4 - num3 >= 0f && num7 - num8 >= 0f)
		{
			float num12 = (num4 - num3) / (num4 - num3 + (num7 - num8));
			closestPoint = b + num12 * (c - b);
			return;
		}
		float num13 = 1f / (num11 + num9 + num5);
		float num14 = num9 * num13;
		float num15 = num5 * num13;
		closestPoint = a + ab * num14 + ac * num15;
	}

	public static float AngleBetweenVectors(Vector3 first, Vector3 second)
	{
		float num = Vector3.Dot(first, second);
		float num2 = first.Length() * second.Length();
		num = (float)Math.Acos(num / num2);
		if (!float.IsNaN(num))
		{
			return num;
		}
		return 0f;
	}

	public static double SignedAngle2DInPlaneXZ(ref Vector3 v0, ref Vector3 v1)
	{
		float num = v0.X * v1.Z - v0.Z * v1.X;
		return Math.Atan2(num, v0.X * v1.X + v0.Z * v1.Z);
	}

	public static void RandomVector(ref Vector3 vec)
	{
		vec.X = ((float)m_Rand.NextDouble() - 0.5f) * 2f;
		vec.Z = ((float)m_Rand.NextDouble() - 0.5f) * 2f;
		vec.Y = 0f;
	}

	public static void ClosestPointOnLineSegment(ref Vector3 p0, ref Vector3 p1, ref Vector3 point, out Vector3 hitPoint)
	{
		ab = p1 - p0;
		float num = ((point.X - p0.X) * ab.X + (point.Y - p0.Y) * ab.Y + (point.Z - p0.Z) * ab.Z) / (ab.X * ab.X + ab.Y * ab.Y + ab.Z * ab.Z);
		if (num < 0f)
		{
			num = 0f;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		hitPoint = p0 + num * ab;
	}

	public static float clamp(float x, float min, float max)
	{
		if (x < min)
		{
			return min;
		}
		if (x > max)
		{
			return max;
		}
		return x;
	}

	public static void ComputeBoundingShere(Vector3[] positions, ref BoundingSphere s)
	{
		SphereFromDistantPoints(positions, ref s);
		for (int i = 0; i < positions.Length; i++)
		{
			SphereFromSphereAndPoint(ref positions[i], ref s);
		}
	}

	public static void MostSeparatedPoints(ref int min, ref int max, Vector3[] positions)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		for (int i = 1; i < positions.Length; i++)
		{
			if (positions[i].X < positions[num].X)
			{
				num = i;
			}
			if (positions[i].X > positions[num2].X)
			{
				num2 = i;
			}
			if (positions[i].Y < positions[num3].Y)
			{
				num3 = i;
			}
			if (positions[i].Y > positions[num4].Y)
			{
				num4 = i;
			}
			if (positions[i].Z < positions[num5].Z)
			{
				num5 = i;
			}
			if (positions[i].Z > positions[num6].Z)
			{
				num6 = i;
			}
		}
		float num7 = Vector3.Dot(positions[num2] - positions[num], positions[num2] - positions[num]);
		float num8 = Vector3.Dot(positions[num4] - positions[num3], positions[num4] - positions[num3]);
		float num9 = Vector3.Dot(positions[num6] - positions[num5], positions[num6] - positions[num5]);
		min = num;
		max = num2;
		if (num8 > num7 && num8 > num9)
		{
			max = num4;
			min = num3;
		}
		if (num9 > num7 && num9 > num8)
		{
			max = num6;
			min = num5;
		}
	}

	public static void SphereFromDistantPoints(Vector3[] positions, ref BoundingSphere s)
	{
		int min = 0;
		int max = 0;
		MostSeparatedPoints(ref min, ref max, positions);
		s.Center = (positions[min] + positions[max]) * 0.5f;
		s.Radius = Vector3.Dot(positions[max] - s.Center, positions[max] - s.Center);
		s.Radius = (float)Math.Sqrt(s.Radius);
	}

	public static void SphereFromSphereAndPoint(ref Vector3 p, ref BoundingSphere s)
	{
		Vector3 vector = p - s.Center;
		float num = Vector3.Dot(vector, vector);
		if (num > s.Radius * s.Radius)
		{
			float num2 = (float)Math.Sqrt(num);
			float num3 = (s.Radius + num2) * 0.5f;
			float num4 = (num3 - s.Radius) / num2;
			s.Radius = num3;
			s.Center += vector * num4;
		}
	}

	public static void RemoveScaling(ref Matrix m)
	{
		tmpCol0.X = m.M11;
		tmpCol0.Y = m.M12;
		tmpCol0.Z = m.M13;
		tmpCol0.W = m.M14;
		tmpCol1.X = m.M21;
		tmpCol1.Y = m.M22;
		tmpCol1.Z = m.M23;
		tmpCol1.W = m.M24;
		tmpCol2.X = m.M31;
		tmpCol2.Y = m.M32;
		tmpCol2.Z = m.M33;
		tmpCol2.W = m.M34;
		tmpCol0.Normalize();
		tmpCol1.Normalize();
		tmpCol2.Normalize();
		m.M11 = tmpCol0.X;
		m.M12 = tmpCol0.Y;
		m.M13 = tmpCol0.Z;
		m.M14 = tmpCol0.W;
		m.M21 = tmpCol1.X;
		m.M22 = tmpCol1.Y;
		m.M23 = tmpCol1.Z;
		m.M24 = tmpCol1.W;
		m.M31 = tmpCol2.X;
		m.M32 = tmpCol2.Y;
		m.M33 = tmpCol2.Z;
		m.M34 = tmpCol2.W;
	}

	public static Vector3 MinVector(Vector3 a, Vector3 b)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = ((a.X < b.X) ? a.X : b.X);
		zero.Y = ((a.Y < b.Y) ? a.Y : b.Y);
		zero.Z = ((a.Z < b.Z) ? a.Z : b.Z);
		return zero;
	}

	public static Vector3 MaxVector(Vector3 a, Vector3 b)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = ((a.X > b.X) ? a.X : b.X);
		zero.Y = ((a.Y > b.Y) ? a.Y : b.Y);
		zero.Z = ((a.Z > b.Z) ? a.Z : b.Z);
		return zero;
	}

	public static bool TestTriangleAABB(ref Vector3 aabbMin, ref Vector3 aabbMax, ref TriangleData triangle)
	{
		vecP1 = PositionList[triangle.p1];
		vecP2 = PositionList[triangle.p2];
		vecP3 = PositionList[triangle.p3];
		boxcenter = (aabbMin + aabbMax) * 0.5f;
		boxhalfsize = (aabbMax - aabbMin) * 0.5f;
		v0 = vecP1 - boxcenter;
		v1 = vecP2 - boxcenter;
		v2 = vecP3 - boxcenter;
		e0 = v1 - v0;
		e1 = v2 - v1;
		e2 = v0 - v2;
		float num = Math.Abs(e0.X);
		float num2 = Math.Abs(e0.Y);
		float num3 = Math.Abs(e0.Z);
		float num4 = e0.Z * v0.Y - e0.Y * v0.Z;
		float num5 = e0.Z * v2.Y - e0.Y * v2.Z;
		float num6;
		float num7;
		if (num4 < num5)
		{
			num6 = num4;
			num7 = num5;
		}
		else
		{
			num6 = num5;
			num7 = num4;
		}
		float num8 = num3 * boxhalfsize.Y + num2 * boxhalfsize.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = (0f - e0.Z) * v0.X + e0.X * v0.Z;
		num5 = (0f - e0.Z) * v2.X + e0.X * v2.Z;
		if (num4 < num5)
		{
			num6 = num4;
			num7 = num5;
		}
		else
		{
			num6 = num5;
			num7 = num4;
		}
		num8 = num3 * boxhalfsize.X + num * boxhalfsize.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		float num9 = e0.Y * v1.X - e0.X * v1.Y;
		num5 = e0.Y * v2.X - e0.X * v2.Y;
		if (num5 < num9)
		{
			num6 = num5;
			num7 = num9;
		}
		else
		{
			num6 = num9;
			num7 = num5;
		}
		num8 = num2 * boxhalfsize.X + num * boxhalfsize.Y;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num = Math.Abs(e1.X);
		num2 = Math.Abs(e1.Y);
		num3 = Math.Abs(e1.Z);
		num4 = e1.Z * v0.Y - e1.Y * v0.Z;
		num5 = e1.Z * v2.Y - e1.Y * v2.Z;
		if (num4 < num5)
		{
			num6 = num4;
			num7 = num5;
		}
		else
		{
			num6 = num5;
			num7 = num4;
		}
		num8 = num3 * boxhalfsize.Y + num2 * boxhalfsize.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = (0f - e1.Z) * v0.X + e1.X * v0.Z;
		num5 = (0f - e1.Z) * v2.X + e1.X * v2.Z;
		if (num4 < num5)
		{
			num6 = num4;
			num7 = num5;
		}
		else
		{
			num6 = num5;
			num7 = num4;
		}
		num8 = num3 * boxhalfsize.X + num * boxhalfsize.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = e1.Y * v0.X - e1.X * v0.Y;
		num9 = e1.Y * v1.X - e1.X * v1.Y;
		if (num4 < num9)
		{
			num6 = num4;
			num7 = num9;
		}
		else
		{
			num6 = num9;
			num7 = num4;
		}
		num8 = num2 * boxhalfsize.X + num * boxhalfsize.Y;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num = Math.Abs(e2.X);
		num2 = Math.Abs(e2.Y);
		num3 = Math.Abs(e2.Z);
		num4 = e2.Z * v0.Y - e2.Y * v0.Z;
		num9 = e2.Z * v1.Y - e2.Y * v1.Z;
		if (num4 < num9)
		{
			num6 = num4;
			num7 = num9;
		}
		else
		{
			num6 = num9;
			num7 = num4;
		}
		num8 = num3 * boxhalfsize.Y + num2 * boxhalfsize.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = (0f - e2.Z) * v0.X + e2.X * v0.Z;
		num9 = (0f - e2.Z) * v1.X + e2.X * v1.Z;
		if (num4 < num9)
		{
			num6 = num4;
			num7 = num9;
		}
		else
		{
			num6 = num9;
			num7 = num4;
		}
		num8 = num3 * boxhalfsize.X + num * boxhalfsize.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num9 = e2.Y * v1.X - e2.X * v1.Y;
		num5 = e2.Y * v2.X - e2.X * v2.Y;
		if (num5 < num9)
		{
			num6 = num5;
			num7 = num9;
		}
		else
		{
			num6 = num9;
			num7 = num5;
		}
		num8 = num2 * boxhalfsize.X + num * boxhalfsize.Y;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num6 = (num7 = v0.X);
		if (v1.X < num6)
		{
			num6 = v1.X;
		}
		if (v1.X > num7)
		{
			num7 = v1.X;
		}
		if (v2.X < num6)
		{
			num6 = v2.X;
		}
		if (v2.X > num7)
		{
			num7 = v2.X;
		}
		if (num6 > boxhalfsize.X || num7 < 0f - boxhalfsize.X)
		{
			return false;
		}
		num6 = (num7 = v0.Y);
		if (v1.Y < num6)
		{
			num6 = v1.Y;
		}
		if (v1.Y > num7)
		{
			num7 = v1.Y;
		}
		if (v2.Y < num6)
		{
			num6 = v2.Y;
		}
		if (v2.Y > num7)
		{
			num7 = v2.Y;
		}
		if (num6 > boxhalfsize.Y || num7 < 0f - boxhalfsize.Y)
		{
			return false;
		}
		num6 = (num7 = v0.Z);
		if (v1.Z < num6)
		{
			num6 = v1.Z;
		}
		if (v1.Z > num7)
		{
			num7 = v1.Z;
		}
		if (v2.Z < num6)
		{
			num6 = v2.Z;
		}
		if (v2.Z > num7)
		{
			num7 = v2.Z;
		}
		if (num6 > boxhalfsize.Z || num7 < 0f - boxhalfsize.Z)
		{
			return false;
		}
		normal.X = v1.Y * v2.Z - v1.Z * v2.Y;
		normal.Y = v1.Z * v2.X - v1.X * v2.Z;
		normal.Z = v1.X * v2.Y - v1.Y * v2.X;
		float num10 = 0f - (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z);
		vmin = Vector3.Zero;
		vmax = Vector3.Zero;
		if (normal.X > 0f)
		{
			vmin.X = 0f - boxhalfsize.X;
			vmax.X = boxhalfsize.X;
		}
		else
		{
			vmin.X = boxhalfsize.X;
			vmax.X = 0f - boxhalfsize.X;
		}
		if (normal.Y > 0f)
		{
			vmin.Y = 0f - boxhalfsize.Y;
			vmax.Y = boxhalfsize.Y;
		}
		else
		{
			vmin.Y = boxhalfsize.Y;
			vmax.Y = 0f - boxhalfsize.Y;
		}
		if (normal.Z > 0f)
		{
			vmin.Z = 0f - boxhalfsize.Z;
			vmax.Z = boxhalfsize.Z;
		}
		else
		{
			vmin.Z = boxhalfsize.Z;
			vmax.Z = 0f - boxhalfsize.Z;
		}
		if (normal.X * vmin.X + normal.Y * vmin.Y + normal.Z * vmin.Z + num10 > 0f)
		{
			return false;
		}
		if (normal.X * vmax.X + normal.Y * vmax.Y + normal.Z * vmax.Z + num10 >= 0f)
		{
			return true;
		}
		return true;
	}

	public static bool IntersectRagdollSphereTriangle(ref Vector3 origin, float radiusSqr, ref TriangleData triangle, ref Vector3 closestPoint, ref float disSqr)
	{
		vecP1 = PositionList[triangle.p1];
		vecP2 = PositionList[triangle.p2];
		vecP3 = PositionList[triangle.p3];
		RagdollClosestPTPointTriangle(ref origin, ref vecP1, ref vecP2, ref vecP3, ref closestPoint);
		closestPoint.X = origin.X - closestPoint.X;
		closestPoint.Y = origin.Y - closestPoint.Y;
		closestPoint.Z = origin.Z - closestPoint.Z;
		disSqr = closestPoint.LengthSquared();
		return disSqr <= radiusSqr;
	}

	private static void RagdollClosestPTPointTriangle(ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 closestPoint)
	{
		rab.X = b.X - a.X;
		rab.Y = b.Y - a.Y;
		rab.Z = b.Z - a.Z;
		rac.X = c.X - a.X;
		rac.Y = c.Y - a.Y;
		rac.Z = c.Z - a.Z;
		rap.X = p.X - a.X;
		rap.Y = p.Y - a.Y;
		rap.Z = p.Z - a.Z;
		float num = rab.X * rap.X + rab.Y * rap.Y + rab.Z * rap.Z;
		float num2 = rac.X * rap.X + rac.Y * rap.Y + rac.Z * rap.Z;
		if (num <= 0f && num2 <= 0f)
		{
			closestPoint = a;
			return;
		}
		rbp.X = p.X - b.X;
		rbp.Y = p.Y - b.Y;
		rbp.Z = p.Z - b.Z;
		float num3 = rab.X * rbp.X + rab.Y * rbp.Y + rab.Z * rbp.Z;
		float num4 = rac.X * rbp.X + rac.Y * rbp.Y + rac.Z * rbp.Z;
		if (num3 >= 0f && num4 <= num3)
		{
			closestPoint = b;
			return;
		}
		float num5 = num * num4 - num3 * num2;
		if (num5 <= 0f && num >= 0f && num3 <= 0f)
		{
			float num6 = num / (num - num3);
			closestPoint = a + num6 * rab;
			return;
		}
		rcp.X = p.X - c.X;
		rcp.Y = p.Y - c.Y;
		rcp.Z = p.Z - c.Z;
		float num7 = rab.X * rcp.X + rab.Y * rcp.Y + rab.Z * rcp.Z;
		float num8 = rac.X * rcp.X + rac.Y * rcp.Y + rac.Z * rcp.Z;
		if (num8 >= 0f && num7 <= num8)
		{
			closestPoint = c;
			return;
		}
		float num9 = num7 * num2 - num * num8;
		if (num9 <= 0f && num2 >= 0f && num8 <= 0f)
		{
			float num10 = num2 / (num2 - num8);
			closestPoint = a + num10 * rac;
			return;
		}
		float num11 = num3 * num8 - num7 * num4;
		if (num11 <= 0f && num4 - num3 >= 0f && num7 - num8 >= 0f)
		{
			float num12 = (num4 - num3) / (num4 - num3 + (num7 - num8));
			closestPoint = b + num12 * (c - b);
			return;
		}
		float num13 = 1f / (num11 + num9 + num5);
		float num14 = num9 * num13;
		float num15 = num5 * num13;
		closestPoint = a + rab * num14 + rac * num15;
	}
}
