using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.eOOBB, DataContent")]
public struct eOOBB
{
	public string Name;

	public string Parent;

	public Vector3 extents;

	public Vector3 center;

	public Vector3 Min;

	public Vector3 Max;

	public eMyBoundingSphere boundingShere;

	public Matrix objectRotation;

	public Matrix inversTransform;

	public Vector3 AbsMin;

	public Vector3 AbsMax;

	public Vector3 AbsCenter;

	public Vector3 AbsExtents;

	private static Vector3 m = Vector3.Zero;

	private static Vector3 vecUnitX = Vector3.UnitX;

	private static Vector3 vecUnitY = Vector3.UnitY;

	private static Vector3 vecUnitZ = Vector3.UnitZ;

	private static Vector3 tmpPoint = Vector3.Zero;

	private static Vector3 tmpClosetPoint = Vector3.Zero;

	private static Ray tmpRay = default(Ray);

	private static BoundingSphere tmpSphere = default(BoundingSphere);

	private static float tmpFResult = 0f;

	private static Matrix tmpCollisionMat = Matrix.Identity;

	private static Ray tmpCollisionRay = default(Ray);

	private static BoundingBox tmpCollisionAABB = default(BoundingBox);

	public void Initialize()
	{
	}

	public bool TestSegment(ref IntersectSegmentParams segment)
	{
		m = segment.SegmentMidpoint - AbsCenter;
		float num = Math.Abs(segment.SegmentHalflength.X);
		if (Math.Abs(m.X) > AbsExtents.X + num)
		{
			return false;
		}
		float num2 = Math.Abs(segment.SegmentHalflength.Y);
		if (Math.Abs(m.Y) > AbsExtents.Y + num2)
		{
			return false;
		}
		float num3 = Math.Abs(segment.SegmentHalflength.Z);
		if (Math.Abs(m.Z) > AbsExtents.Z + num3)
		{
			return false;
		}
		num += 1E-05f;
		num2 += 1E-05f;
		num3 += 1E-05f;
		if (Math.Abs(m.Y * segment.SegmentHalflength.Z - m.Z * segment.SegmentHalflength.Y) > AbsExtents.Y * num3 + AbsExtents.Z * num2)
		{
			return false;
		}
		if (Math.Abs(m.Z * segment.SegmentHalflength.X - m.X * segment.SegmentHalflength.Z) > AbsExtents.X * num3 + AbsExtents.Z * num)
		{
			return false;
		}
		if (Math.Abs(m.X * segment.SegmentHalflength.Y - m.Y * segment.SegmentHalflength.X) > AbsExtents.X * num2 + AbsExtents.Y * num)
		{
			return false;
		}
		return true;
	}

	public bool IntersectSegment(ref IntersectSegmentParams segment)
	{
		float num = 0f;
		float num2 = 100000f;
		if (Math.Abs(segment.SegmentDirection.X) < 1E-06f)
		{
			if (segment.SegmentStart.X < Min.X || segment.SegmentStart.X > Max.X)
			{
				return false;
			}
		}
		else
		{
			float num3 = (Min.X - segment.SegmentStart.X) * segment.oodX;
			float num4 = (Max.X - segment.SegmentStart.X) * segment.oodX;
			if (num3 > num4)
			{
				float num5 = num3;
				num3 = num4;
				num4 = num5;
			}
			num = ((num > num3) ? num : num3);
			num2 = ((num2 < num4) ? num2 : num4);
			if (num > num2)
			{
				return false;
			}
		}
		if (Math.Abs(segment.SegmentDirection.Y) < 1E-06f)
		{
			if (segment.SegmentStart.Y < Min.Y || segment.SegmentStart.Y > Max.Y)
			{
				return false;
			}
		}
		else
		{
			float num3 = (Min.Y - segment.SegmentStart.Y) * segment.oodY;
			float num4 = (Max.Y - segment.SegmentStart.Y) * segment.oodY;
			if (num3 > num4)
			{
				float num5 = num3;
				num3 = num4;
				num4 = num5;
			}
			num = ((num > num3) ? num : num3);
			num2 = ((num2 < num4) ? num2 : num4);
			if (num > num2)
			{
				return false;
			}
		}
		if (Math.Abs(segment.SegmentDirection.Z) < 1E-06f)
		{
			if (segment.SegmentStart.Z < Min.Z || segment.SegmentStart.Z > Max.Z)
			{
				return false;
			}
		}
		else
		{
			float num3 = (Min.Z - segment.SegmentStart.Z) * segment.oodZ;
			float num4 = (Max.Z - segment.SegmentStart.Z) * segment.oodZ;
			if (num3 > num4)
			{
				float num5 = num3;
				num3 = num4;
				num4 = num5;
			}
			num = ((num > num3) ? num : num3);
			num2 = ((num2 < num4) ? num2 : num4);
			if (num > num2)
			{
				return false;
			}
		}
		segment.hitDistance = num;
		segment.hitPosition = segment.SegmentStart + segment.SegmentDirection * num;
		return true;
	}

	public bool ContainsPoint(ref Vector3 p)
	{
		Vector3.Transform(ref p, ref inversTransform, out tmpPoint);
		if (tmpPoint.X >= Min.X && tmpPoint.X <= Max.X && tmpPoint.Z >= Min.Z && tmpPoint.Z <= Max.Z && tmpPoint.Y >= Min.Y && tmpPoint.Y <= Max.Y)
		{
			return true;
		}
		return false;
	}

	public bool IntersectSphere(ref BoundingSphere e)
	{
		Vector3.Transform(ref e.Center, ref inversTransform, out tmpSphere.Center);
		ClosestPoint(ref tmpSphere, out tmpClosetPoint);
		tmpPoint.X = tmpSphere.Center.X - tmpClosetPoint.X;
		tmpPoint.Y = tmpSphere.Center.Y - tmpClosetPoint.Y;
		tmpPoint.Z = tmpSphere.Center.Z - tmpClosetPoint.Z;
		Vector3.Transform(ref tmpPoint, ref objectRotation, out tmpPoint);
		float num = tmpPoint.LengthSquared() / (e.Radius * e.Radius);
		if (num < 1.05f)
		{
			tmpPoint.Normalize();
			tmpFResult = 0f;
			Vector3.Dot(ref tmpPoint, ref vecUnitY, out tmpFResult);
			if (num < 1f)
			{
				float num2 = e.Radius - num * e.Radius;
				e.Center += tmpPoint * num2;
			}
			return true;
		}
		return false;
	}

	public bool TestSphere(ref BoundingSphere e)
	{
		Vector3.Transform(ref e.Center, ref inversTransform, out tmpSphere.Center);
		ClosestPoint(ref tmpSphere, out tmpClosetPoint);
		tmpPoint.X = tmpSphere.Center.X - tmpClosetPoint.X;
		tmpPoint.Y = tmpSphere.Center.Y - tmpClosetPoint.Y;
		tmpPoint.Z = tmpSphere.Center.Z - tmpClosetPoint.Z;
		Vector3.Transform(ref tmpPoint, ref objectRotation, out tmpPoint);
		float num = tmpPoint.LengthSquared() / (e.Radius * e.Radius);
		if (num < 1.05f)
		{
			return true;
		}
		return false;
	}

	public bool CollisionSphere(ref BoundingSphere e, ref CollisionStruct c)
	{
		Vector3.Transform(ref e.Center, ref inversTransform, out tmpSphere.Center);
		ClosestPoint(ref tmpSphere.Center, out tmpClosetPoint);
		c.hitNormal.X = tmpSphere.Center.X - tmpClosetPoint.X;
		c.hitNormal.Y = tmpSphere.Center.Y - tmpClosetPoint.Y;
		c.hitNormal.Z = tmpSphere.Center.Z - tmpClosetPoint.Z;
		Vector3.Transform(ref c.hitNormal, ref objectRotation, out c.hitNormal);
		float num = c.hitNormal.LengthSquared() / (e.Radius * e.Radius);
		if (num < 1.05f)
		{
			c.hitNormal.Normalize();
			tmpFResult = 0f;
			Vector3.Dot(ref c.hitNormal, ref vecUnitY, out tmpFResult);
			if (tmpFResult > 0.5f)
			{
				c.onWalkable = true;
			}
			if (num < 1f && c.applyResponse)
			{
				c.depth = e.Radius - num * e.Radius;
				e.Center += c.hitNormal * c.depth;
			}
			return true;
		}
		return false;
	}

	public bool CollisionRay(ref Ray e, ref CollisionStruct c)
	{
		float? result = null;
		Vector3.Transform(ref e.Position, ref inversTransform, out tmpRay.Position);
		tmpCollisionAABB.Min = Min;
		tmpCollisionAABB.Max = Max;
		tmpCollisionAABB.Intersects(ref e, out result);
		if (result.HasValue)
		{
			c.hitPosition = tmpRay.Position + tmpRay.Direction * result.Value;
			GetNormalFromPoint(ref c.hitPosition, ref c.hitNormal);
			Vector3.Transform(ref c.hitNormal, ref objectRotation, out c.hitNormal);
			c.depth = (c.hitPosition - e.Position).Length();
			return true;
		}
		return false;
	}

	public bool RayCast(ref IntersectSegmentParams e)
	{
		float? result = null;
		Vector3.Transform(ref e.SegmentStart, ref inversTransform, out tmpCollisionRay.Position);
		tmpCollisionMat = inversTransform;
		tmpCollisionMat.Translation = Vector3.Zero;
		Vector3.Transform(ref e.SegmentDirection, ref tmpCollisionMat, out tmpCollisionRay.Direction);
		tmpCollisionAABB.Min = Min;
		tmpCollisionAABB.Max = Max;
		tmpCollisionAABB.Intersects(ref tmpCollisionRay, out result);
		if (result.HasValue)
		{
			e.hitPosition = tmpCollisionRay.Position + tmpCollisionRay.Direction * result.Value;
			GetNormalFromPoint(ref e.hitPosition, ref e.hitNormal);
			Vector3.Transform(ref e.hitNormal, ref objectRotation, out e.hitNormal);
			e.hitDistance = (e.hitPosition - e.SegmentStart).Length();
			return true;
		}
		return false;
	}

	public bool CollisionRayInverted(ref Ray e)
	{
		tmpCollisionAABB.Min = Min;
		tmpCollisionAABB.Max = Max;
		tmpCollisionAABB.Intersects(ref e, out var result);
		if (result.HasValue)
		{
			return true;
		}
		return false;
	}

	public void SetFromPositions(string name, string parentName, Matrix transform, Vector3[] positions)
	{
		Name = name;
		Parent = parentName;
		objectRotation = transform;
		objectRotation.Translation = Vector3.Zero;
		inversTransform = Matrix.Invert(transform);
		int num = positions.Length;
		Min.X = 1000000f;
		Min.Y = 1000000f;
		Min.Z = 1000000f;
		Max.X = -1000000f;
		Max.Y = -1000000f;
		Max.Z = -1000000f;
		for (int i = 0; i < num; i++)
		{
			Min.X = ((positions[i].X > Min.X) ? Min.X : positions[i].X);
			Min.Y = ((positions[i].Y > Min.Y) ? Min.Y : positions[i].Y);
			Min.Z = ((positions[i].Z > Min.Z) ? Min.Z : positions[i].Z);
			Max.X = ((positions[i].X < Max.X) ? Max.X : positions[i].X);
			Max.Y = ((positions[i].Y < Max.Y) ? Max.Y : positions[i].Y);
			Max.Z = ((positions[i].Z < Max.Z) ? Max.Z : positions[i].Z);
		}
		center = (Min + Max) * 0.5f;
		extents = Max - center;
		Vector3 a = Vector3.Transform(Min, transform);
		Vector3 b = Vector3.Transform(Max, transform);
		AbsMin = MinVector(a, b);
		AbsMax = MaxVector(a, b);
		AbsCenter = (AbsMin + AbsMax) * 0.5f;
		AbsExtents = AbsMax - AbsCenter;
		boundingShere.Center = center;
		boundingShere.Radius = extents.Length();
	}

	public void GetNormalFromPoint(ref Vector3 point, ref Vector3 normal)
	{
		float num = float.MaxValue;
		normal.X = 0f;
		normal.Y = 0f;
		normal.Z = 0f;
		float num2 = Math.Abs(extents.X - Math.Abs(point.X));
		if (num2 < num)
		{
			num = num2;
			normal = Math.Sign(point.X) * Vector3.UnitX;
		}
		num2 = Math.Abs(extents.Y - Math.Abs(point.Y));
		if (num2 < num)
		{
			num = num2;
			normal = Math.Sign(point.Y) * Vector3.UnitY;
		}
		num2 = Math.Abs(extents.Z - Math.Abs(point.Z));
		if (num2 < num)
		{
			num = num2;
			normal = Math.Sign(point.Z) * Vector3.UnitZ;
		}
	}

	public void ClosestPoint(ref BoundingSphere e, out Vector3 c)
	{
		ClosestPoint(ref e.Center, out c);
	}

	public void ClosestPoint(ref Vector3 point, out Vector3 c)
	{
		c = point;
		if (point.X < Min.X)
		{
			c.X = Min.X;
		}
		else if (point.X > Max.X)
		{
			c.X = Max.X;
		}
		if (point.Z < Min.Z)
		{
			c.Z = Min.Z;
		}
		else if (point.Z > Max.Z)
		{
			c.Z = Max.Z;
		}
		if (point.Y < Min.Y)
		{
			c.Y = Min.Y;
		}
		else if (point.Y > Max.Y)
		{
			c.Y = Max.Y;
		}
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
}
