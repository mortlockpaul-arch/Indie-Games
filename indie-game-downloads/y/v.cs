using System;
using Microsoft.Xna.Framework;
using N;
using r;
using s;

namespace y;

internal class v : h
{
	internal Vector3 a5h;

	internal Vector3 a5b;

	internal Vector3 a56;

	internal _0006 a5a;

	public Vector3 VertexA
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

	public Vector3 VertexB
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

	public Vector3 VertexC
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = value;
			OnShapeChanged();
		}
	}

	public _0006 Sidedness
	{
		get
		{
			return a5a;
		}
		set
		{
			a5a = value;
			OnShapeChanged();
		}
	}

	public v()
	{
	}

	public v(Vector3 vA, Vector3 vB, Vector3 vC, out Vector3 center)
	{
		center = (vA + vB + vC) / 3f;
		a5h = vA - center;
		a5b = vB - center;
		a56 = vC - center;
		OnShapeChanged();
	}

	public v(Vector3 vA, Vector3 vB, Vector3 vC)
	{
		Vector3 vector = (vA + vB + vC) / 3f;
		a5h = vA - vector;
		a5b = vB - vector;
		a56 = vC - vector;
		OnShapeChanged();
	}

	public override void GetBoundingBox(ref N._0006 shapeTransform, out BoundingBox boundingBox)
	{
		Vector3.Transform(ref a5h, ref shapeTransform.Orientation, out var result);
		Vector3.Transform(ref a5b, ref shapeTransform.Orientation, out var result2);
		Vector3.Transform(ref a56, ref shapeTransform.Orientation, out var result3);
		Vector3.Min(ref result, ref result2, out boundingBox.Min);
		Vector3.Min(ref result3, ref boundingBox.Min, out boundingBox.Min);
		Vector3.Max(ref result, ref result2, out boundingBox.Max);
		Vector3.Max(ref result3, ref boundingBox.Max, out boundingBox.Max);
		boundingBox.Min.X += shapeTransform.Position.X - collisionMargin;
		boundingBox.Min.Y += shapeTransform.Position.Y - collisionMargin;
		boundingBox.Min.Z += shapeTransform.Position.Z - collisionMargin;
		boundingBox.Max.X += shapeTransform.Position.X + collisionMargin;
		boundingBox.Max.Y += shapeTransform.Position.Y + collisionMargin;
		boundingBox.Max.Z += shapeTransform.Position.Z + collisionMargin;
	}

	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		Vector3.Dot(ref direction, ref a5h, out var result);
		Vector3.Dot(ref direction, ref a5b, out var result2);
		Vector3.Dot(ref direction, ref a56, out var result3);
		if (result > result2 && result > result3)
		{
			extremePoint = a5h;
		}
		else if (result2 > result3)
		{
			extremePoint = a5b;
		}
		else
		{
			extremePoint = a56;
		}
	}

	public override float ComputeMaximumRadius()
	{
		Vector3 vector = ComputeCenter();
		return collisionMargin + Math.Max((a5h - vector).Length(), Math.Max((a5b - vector).Length(), (a56 - vector).Length()));
	}

	public override float ComputeMinimumRadius()
	{
		return 0f;
	}

	public override N._7 ComputeVolumeDistribution(out float volume)
	{
		Vector3 vector = ComputeCenter();
		volume = ComputeVolume();
		float num = a5h.X - vector.X;
		float num2 = a5h.Y - vector.Y;
		float num3 = a5h.Z - vector.Z;
		N._7 result = new N._7(1f / 3f * (num2 * num2 + num3 * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * (num * num + num3 * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * (num * num + num2 * num2));
		num = a5b.X - vector.X;
		num2 = a5b.Y - vector.Y;
		num3 = a5b.Z - vector.Z;
		N._7 obj = new N._7(1f / 3f * (num2 * num2 + num3 * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * (num * num + num3 * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * (num * num + num2 * num2));
		N._7.Add(ref result, ref obj, out result);
		num = a56.X - vector.X;
		num2 = a56.Y - vector.Y;
		num3 = a56.Z - vector.Z;
		obj = new N._7(1f / 3f * (num2 * num2 + num3 * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * (num * num + num3 * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * (num * num + num2 * num2));
		N._7.Add(ref result, ref obj, out result);
		return result;
	}

	public override Vector3 ComputeCenter()
	{
		return (a5h + a5b + a56) / 3f;
	}

	public override Vector3 ComputeCenter(out float volume)
	{
		volume = ComputeVolume();
		return ComputeCenter();
	}

	public override float ComputeVolume()
	{
		return Vector3.Cross(a5b - a5h, a56 - a5h).Length() * collisionMargin;
	}

	public Vector3 GetLocalNormal()
	{
		Vector3.Subtract(ref a5b, ref a5h, out var result);
		Vector3.Subtract(ref a56, ref a5h, out var result2);
		Vector3.Cross(ref result, ref result2, out var result3);
		result3.Normalize();
		return result3;
	}

	public Vector3 GetNormal(N._0006 transform)
	{
		Vector3 value = GetLocalNormal();
		Vector3.Transform(ref value, ref transform.Orientation, out value);
		return value;
	}

	public override bool RayTest(ref Ray ray, ref N._0006 transform, float maximumLength, out r._0006 hit)
	{
		N._7.CreateFromQuaternion(ref transform.Orientation, out var _);
		Quaternion.Conjugate(ref transform.Orientation, out var result2);
		Ray ray2 = default(Ray);
		Vector3.Transform(ref ray.Direction, ref result2, out ray2.Direction);
		Vector3.Subtract(ref ray.Position, ref transform.Position, out ray2.Position);
		Vector3.Transform(ref ray2.Position, ref result2, out ray2.Position);
		bool result3 = r.X.FindRayTriangleIntersection(ref ray2, maximumLength, a5a, ref a5h, ref a5b, ref a56, out hit);
		Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
		Vector3.Add(ref ray.Position, ref hit.Location, out hit.Location);
		Vector3.Transform(ref hit.Normal, ref transform.Orientation, out hit.Normal);
		return result3;
	}

	public override string ToString()
	{
		return string.Concat(a5h, ", ", a5b, ", ", a56);
	}

	public override s.b GetCollidableInstance()
	{
		return new s.B<v>(this);
	}
}
