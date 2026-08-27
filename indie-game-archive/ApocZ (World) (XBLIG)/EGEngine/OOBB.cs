using System;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct OOBB
{
	public Vector3 extents;

	public Vector3 center;

	public Vector3 Min;

	public Vector3 Max;

	public MyBoundingSphere boundingShere;

	public Matrix objectRotation;

	public Matrix inversTransform;

	private static Vector3 vecUnitX = Vector3.UnitX;

	private static Vector3 vecUnitY = Vector3.UnitY;

	private static Vector3 vecUnitZ = Vector3.UnitZ;

	private static Vector3 tmpPoint = Vector3.Zero;

	private static Vector3 tmpClosetPoint = Vector3.Zero;

	private static Ray tmpRay = default(Ray);

	private static BoundingSphere tmpSphere = default(BoundingSphere);

	private static BoundingBox tmpCollisionAABB = default(BoundingBox);

	private static float tmpFResult = 0f;

	public OOBB(Vector3[] positions, Matrix transform)
	{
		objectRotation = transform;
		objectRotation.Translation = Vector3.Zero;
		inversTransform = Matrix.Invert(transform);
		extents = Vector3.Zero;
		center = Vector3.Zero;
		Min = Vector3.Zero;
		Max = Vector3.Zero;
		boundingShere = default(MyBoundingSphere);
		SetFromPositions(positions);
	}

	public void Initialize()
	{
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
			return true;
		}
		return false;
	}

	public bool CollisionRay(ref Ray e, ref CollisionStruct c)
	{
		float? num = null;
		Vector3.Transform(ref e.Position, ref inversTransform, out tmpRay.Position);
		if (num.HasValue)
		{
			c.hitPosition = tmpRay.Position + tmpRay.Direction * num.Value;
			GetNormalFromPoint(ref c.hitPosition, ref c.hitNormal);
			Vector3.Transform(ref c.hitNormal, ref objectRotation, out c.hitNormal);
			c.depth = (c.hitPosition - e.Position).Length();
			return true;
		}
		return false;
	}

	public float? CollisionRayInverted(ref Ray e, float scaling)
	{
		tmpCollisionAABB.Min = Min * scaling;
		tmpCollisionAABB.Max = Max * scaling;
		tmpCollisionAABB.Intersects(ref e, out var result);
		return result;
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

	public void SetFromeMesh(eMesh mesh, Matrix transform, VertexType v)
	{
		objectRotation = transform;
		objectRotation.Translation = Vector3.Zero;
		inversTransform = Matrix.Invert(transform);
		Vector3[] positionsFromeMesh = MeshTools.GetPositionsFromeMesh(mesh, v);
		SetFromPositions(positionsFromeMesh);
	}

	public void SetFromMesh(ModelMesh mesh, Matrix transform, VertexType v)
	{
		objectRotation = transform;
		objectRotation.Translation = Vector3.Zero;
		inversTransform = Matrix.Invert(transform);
		Vector3[] positionsFromMesh = MeshTools.GetPositionsFromMesh(mesh, v);
		SetFromPositions(positionsFromMesh);
	}

	public void SetFromPositions(Vector3[] positions)
	{
		int num = positions.Length;
		Vector3 zero = Vector3.Zero;
		Vector3 zero2 = Vector3.Zero;
		zero.X = 1000000f;
		zero.Y = 1000000f;
		zero.Z = 1000000f;
		zero2.X = -1000000f;
		zero2.Y = -1000000f;
		zero2.Z = -1000000f;
		for (int i = 0; i < num; i++)
		{
			zero.X = ((positions[i].X > zero.X) ? zero.X : positions[i].X);
			zero.Y = ((positions[i].Y > zero.Y) ? zero.Y : positions[i].Y);
			zero.Z = ((positions[i].Z > zero.Z) ? zero.Z : positions[i].Z);
			zero2.X = ((positions[i].X < zero2.X) ? zero2.X : positions[i].X);
			zero2.Y = ((positions[i].Y < zero2.Y) ? zero2.Y : positions[i].Y);
			zero2.Z = ((positions[i].Z < zero2.Z) ? zero2.Z : positions[i].Z);
		}
		Min = zero;
		Max = zero2;
		extents = (zero2 - zero) * 0.5f;
		boundingShere = default(MyBoundingSphere);
		boundingShere.Center = zero + extents;
		boundingShere.Radius = extents.X;
		if (boundingShere.Radius < extents.Y)
		{
			boundingShere.Radius = extents.Y;
		}
		if (boundingShere.Radius < extents.Z)
		{
			boundingShere.Radius = extents.Z;
		}
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
}
