using System;
using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class math
{
	private static Vector3 vecP1 = Vector3.Zero;

	private static Vector3 vecP2 = Vector3.Zero;

	private static Vector3 vecP3 = Vector3.Zero;

	public static Vector3[] PositionList = null;

	private static Random m_Rand = new Random(7);

	private static Vector3 projDirection = Vector3.Zero;

	private static Vector4 projectedPosition = Vector4.Zero;

	private static Vector3 projPos = Vector3.Zero;

	public static int numberIntSegTri = 0;

	private static Vector3 end = Vector3.Zero;

	private static Vector3 s = Vector3.Zero;

	private static Vector3 ab = Vector3.Zero;

	private static Vector3 ac = Vector3.Zero;

	private static Vector3 qp = Vector3.Zero;

	private static Vector3 n = Vector3.Zero;

	private static Vector3 ap = Vector3.Zero;

	private static Vector3 e = Vector3.Zero;

	private static Vector3 bp = Vector3.Zero;

	private static Vector3 cp = Vector3.Zero;

	private static Vector4 tmpCol0 = Vector4.Zero;

	private static Vector4 tmpCol1 = Vector4.Zero;

	private static Vector4 tmpCol2 = Vector4.Zero;

	private static Vector3 trsM = Vector3.Zero;

	public static bool Position3DToScreen2D(PlayerBase viewer, ref Vector3 pos3D, ref Vector2 pos2D, int qIndex)
	{
		projDirection.X = pos3D.X - viewer.vecPosition.X;
		projDirection.Y = pos3D.Z - viewer.vecPosition.Z;
		float num = projDirection.X * viewer.vecDirection.X + projDirection.Y * viewer.vecDirection.Z;
		if (num > 0f)
		{
			float num2 = projDirection.LengthSquared() / 1000000f;
			projectedPosition.X = pos3D.X;
			projectedPosition.Y = pos3D.Y;
			projectedPosition.Z = pos3D.Z;
			projectedPosition.W = 1f;
			Vector4.Transform(ref projectedPosition, ref viewer.mDataQueue[qIndex].view, out projectedPosition);
			Vector4.Transform(ref projectedPosition, ref viewer.mDataQueue[qIndex].projection, out projectedPosition);
			projectedPosition /= projectedPosition.W;
			pos2D.X = projectedPosition.X * 640f + 640f;
			pos2D.Y = (0f - projectedPosition.Y) * 360f + 360f;
			pos2D.X -= viewer.vpViewPort.X;
			pos2D.Y -= viewer.vpViewPort.Y;
			return true;
		}
		return false;
	}

	public static void Position3DToScreen2DNoZTest(PlayerBase viewer, ref Vector3 pos3D, ref Vector2 pos2D, int qIndex)
	{
		projPos = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Project(pos3D, viewer.mDataQueue[qIndex].projection, viewer.mDataQueue[qIndex].view, Matrix.Identity);
		pos2D.X = projPos.X - (float)viewer.vpViewPort.X;
		pos2D.Y = projPos.Y - (float)viewer.vpViewPort.Y;
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

	public static bool IntersectSphereTriangle(ref Vector3 origin, float radiusSqr, ref TriangleData triangle, ref Vector3 closestPoint, ref float disSqr)
	{
		vecP1 = PositionList[triangle.p1];
		vecP2 = PositionList[triangle.p2];
		vecP3 = PositionList[triangle.p3];
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

	public static double AngleBetweenVectors(Vector3 first, Vector3 second)
	{
		float num = Vector3.Dot(first, second);
		float num2 = first.Length() * second.Length();
		return Math.Acos(num / num2);
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

	public static bool TestRaySphere(ref Vector3 origin, ref Vector3 direction, ref Vector3 center, float radius)
	{
		trsM = origin - center;
		float num = trsM.X * trsM.X + trsM.Y * trsM.Y + trsM.Z * trsM.Z - radius * radius;
		if (num <= 0f)
		{
			return true;
		}
		float num2 = trsM.X * direction.X + trsM.Y * direction.Y + trsM.Z * direction.Z;
		if (num2 > 0f)
		{
			return false;
		}
		float num3 = num2 * num2 - num;
		if (num3 < 0f)
		{
			return false;
		}
		return true;
	}

	public static bool TestTriangleAABB(ref Vector3 aabbMin, ref Vector3 aabbMax, ref TriangleData triangle)
	{
		vecP1 = PositionList[triangle.p1];
		vecP2 = PositionList[triangle.p2];
		vecP3 = PositionList[triangle.p3];
		Vector3 vector = (aabbMin + aabbMax) * 0.5f;
		Vector3 vector2 = (aabbMax - aabbMin) * 0.5f;
		Vector3 zero = Vector3.Zero;
		Vector3 zero2 = Vector3.Zero;
		Vector3 zero3 = Vector3.Zero;
		_ = Vector3.Zero;
		Vector3 zero4 = Vector3.Zero;
		Vector3 zero5 = Vector3.Zero;
		Vector3 zero6 = Vector3.Zero;
		Vector3 zero7 = Vector3.Zero;
		zero = vecP1 - vector;
		zero2 = vecP2 - vector;
		zero3 = vecP3 - vector;
		zero5 = zero2 - zero;
		zero6 = zero3 - zero2;
		zero7 = zero - zero3;
		float num = Math.Abs(zero5.X);
		float num2 = Math.Abs(zero5.Y);
		float num3 = Math.Abs(zero5.Z);
		float num4 = zero5.Z * zero.Y - zero5.Y * zero.Z;
		float num5 = zero5.Z * zero3.Y - zero5.Y * zero3.Z;
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
		float num8 = num3 * vector2.Y + num2 * vector2.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = (0f - zero5.Z) * zero.X + zero5.X * zero.Z;
		num5 = (0f - zero5.Z) * zero3.X + zero5.X * zero3.Z;
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
		num8 = num3 * vector2.X + num * vector2.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		float num9 = zero5.Y * zero2.X - zero5.X * zero2.Y;
		num5 = zero5.Y * zero3.X - zero5.X * zero3.Y;
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
		num8 = num2 * vector2.X + num * vector2.Y;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num = Math.Abs(zero6.X);
		num2 = Math.Abs(zero6.Y);
		num3 = Math.Abs(zero6.Z);
		num4 = zero6.Z * zero.Y - zero6.Y * zero.Z;
		num5 = zero6.Z * zero3.Y - zero6.Y * zero3.Z;
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
		num8 = num3 * vector2.Y + num2 * vector2.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = (0f - zero6.Z) * zero.X + zero6.X * zero.Z;
		num5 = (0f - zero6.Z) * zero3.X + zero6.X * zero3.Z;
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
		num8 = num3 * vector2.X + num * vector2.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = zero6.Y * zero.X - zero6.X * zero.Y;
		num9 = zero6.Y * zero2.X - zero6.X * zero2.Y;
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
		num8 = num2 * vector2.X + num * vector2.Y;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num = Math.Abs(zero7.X);
		num2 = Math.Abs(zero7.Y);
		num3 = Math.Abs(zero7.Z);
		num4 = zero7.Z * zero.Y - zero7.Y * zero.Z;
		num9 = zero7.Z * zero2.Y - zero7.Y * zero2.Z;
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
		num8 = num3 * vector2.Y + num2 * vector2.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num4 = (0f - zero7.Z) * zero.X + zero7.X * zero.Z;
		num9 = (0f - zero7.Z) * zero2.X + zero7.X * zero2.Z;
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
		num8 = num3 * vector2.X + num * vector2.Z;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num9 = zero7.Y * zero2.X - zero7.X * zero2.Y;
		num5 = zero7.Y * zero3.X - zero7.X * zero3.Y;
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
		num8 = num2 * vector2.X + num * vector2.Y;
		if (num6 > num8 || num7 < 0f - num8)
		{
			return false;
		}
		num6 = (num7 = zero.X);
		if (zero2.X < num6)
		{
			num6 = zero2.X;
		}
		if (zero2.X > num7)
		{
			num7 = zero2.X;
		}
		if (zero3.X < num6)
		{
			num6 = zero3.X;
		}
		if (zero3.X > num7)
		{
			num7 = zero3.X;
		}
		if (num6 > vector2.X || num7 < 0f - vector2.X)
		{
			return false;
		}
		num6 = (num7 = zero.Y);
		if (zero2.Y < num6)
		{
			num6 = zero2.Y;
		}
		if (zero2.Y > num7)
		{
			num7 = zero2.Y;
		}
		if (zero3.Y < num6)
		{
			num6 = zero3.Y;
		}
		if (zero3.Y > num7)
		{
			num7 = zero3.Y;
		}
		if (num6 > vector2.Y || num7 < 0f - vector2.Y)
		{
			return false;
		}
		num6 = (num7 = zero.Z);
		if (zero2.Z < num6)
		{
			num6 = zero2.Z;
		}
		if (zero2.Z > num7)
		{
			num7 = zero2.Z;
		}
		if (zero3.Z < num6)
		{
			num6 = zero3.Z;
		}
		if (zero3.Z > num7)
		{
			num7 = zero3.Z;
		}
		if (num6 > vector2.Z || num7 < 0f - vector2.Z)
		{
			return false;
		}
		zero4.X = zero2.Y * zero3.Z - zero2.Z * zero3.Y;
		zero4.Y = zero2.Z * zero3.X - zero2.X * zero3.Z;
		zero4.Z = zero2.X * zero3.Y - zero2.Y * zero3.X;
		float num10 = 0f - (zero2.X * zero3.X + zero2.Y * zero3.Y + zero2.Z * zero3.Z);
		Vector3 zero8 = Vector3.Zero;
		Vector3 zero9 = Vector3.Zero;
		if (zero4.X > 0f)
		{
			zero8.X = 0f - vector2.X;
			zero9.X = vector2.X;
		}
		else
		{
			zero8.X = vector2.X;
			zero9.X = 0f - vector2.X;
		}
		if (zero4.Y > 0f)
		{
			zero8.Y = 0f - vector2.Y;
			zero9.Y = vector2.Y;
		}
		else
		{
			zero8.Y = vector2.Y;
			zero9.Y = 0f - vector2.Y;
		}
		if (zero4.Z > 0f)
		{
			zero8.Z = 0f - vector2.Z;
			zero9.Z = vector2.Z;
		}
		else
		{
			zero8.Z = vector2.Z;
			zero9.Z = 0f - vector2.Z;
		}
		if (zero4.X * zero8.X + zero4.Y * zero8.Y + zero4.Z * zero8.Z + num10 > 0f)
		{
			return false;
		}
		if (zero4.X * zero9.X + zero4.Y * zero9.Y + zero4.Z * zero9.Z + num10 >= 0f)
		{
			return true;
		}
		return true;
	}
}
