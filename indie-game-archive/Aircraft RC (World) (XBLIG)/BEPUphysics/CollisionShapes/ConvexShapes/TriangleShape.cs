using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Triangle collision shape.
/// </summary>
public class TriangleShape : ConvexShape
{
	internal Vector3 vA;

	internal Vector3 vB;

	internal Vector3 vC;

	internal TriangleSidedness sidedness;

	/// <summary>
	///  Gets or sets the first vertex of the triangle shape.
	/// </summary>
	public Vector3 VertexA
	{
		get
		{
			return vA;
		}
		set
		{
			vA = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the second vertex of the triangle shape.
	/// </summary>
	public Vector3 VertexB
	{
		get
		{
			return vB;
		}
		set
		{
			vB = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the third vertex of the triangle shape.
	/// </summary>
	public Vector3 VertexC
	{
		get
		{
			return vC;
		}
		set
		{
			vC = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the sidedness of the triangle.
	/// </summary>
	public TriangleSidedness Sidedness
	{
		get
		{
			return sidedness;
		}
		set
		{
			sidedness = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Constructs a triangle shape without initializing it.
	///  This is useful for systems that re-use a triangle shape repeatedly.
	/// </summary>
	public TriangleShape()
	{
	}

	/// <summary>
	///  Constructs a triangle shape.
	///  The vertices will be recentered.
	/// </summary>
	/// <param name="vA">First vertex in the triangle.</param>
	/// <param name="vB">Second vertex in the triangle.</param>
	/// <param name="vC">Third vertex in the triangle.</param>
	/// <param name="center">Computed center of the triangle.</param>
	public TriangleShape(Vector3 vA, Vector3 vB, Vector3 vC, out Vector3 center)
	{
		center = (vA + vB + vC) / 3f;
		this.vA = vA - center;
		this.vB = vB - center;
		this.vC = vC - center;
		OnShapeChanged();
	}

	/// <summary>
	///  Constructs a triangle shape.
	///  The vertices will be recentered.  If the center is needed, use the other constructor.
	/// </summary>
	/// <param name="vA">First vertex in the triangle.</param>
	/// <param name="vB">Second vertex in the triangle.</param>
	/// <param name="vC">Third vertex in the triangle.</param>
	public TriangleShape(Vector3 vA, Vector3 vB, Vector3 vC)
	{
		Vector3 vector = (vA + vB + vC) / 3f;
		this.vA = vA - vector;
		this.vB = vB - vector;
		this.vC = vC - vector;
		OnShapeChanged();
	}

	/// <summary>
	/// Gets the bounding box of the shape given a transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use.</param>
	/// <param name="boundingBox">Bounding box of the transformed shape.</param>
	public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		Vector3.Transform(ref vA, ref shapeTransform.Orientation, out var result);
		Vector3.Transform(ref vB, ref shapeTransform.Orientation, out var result2);
		Vector3.Transform(ref vC, ref shapeTransform.Orientation, out var result3);
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

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		Vector3.Dot(ref direction, ref vA, out var result);
		Vector3.Dot(ref direction, ref vB, out var result2);
		Vector3.Dot(ref direction, ref vC, out var result3);
		if (result > result2 && result > result3)
		{
			extremePoint = vA;
		}
		else if (result2 > result3)
		{
			extremePoint = vB;
		}
		else
		{
			extremePoint = vC;
		}
	}

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		Vector3 vector = ComputeCenter();
		return collisionMargin + Math.Max((vA - vector).Length(), Math.Max((vB - vector).Length(), (vC - vector).Length()));
	}

	/// <summary>
	///  Computes the minimum radius of the shape.
	///  This is often smaller than the actual minimum radius;
	///  it is simply an approximation that avoids overestimating.
	/// </summary>
	/// <returns>Minimum radius of the shape.</returns>
	public override float ComputeMinimumRadius()
	{
		return 0f;
	}

	/// <summary>
	/// Computes the volume distribution of the shape as well as its volume.
	/// The volume distribution can be used to compute inertia tensors when
	/// paired with mass and other tuning factors.
	/// </summary>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Volume distribution of the shape.</returns>
	public override Matrix3X3 ComputeVolumeDistribution(out float volume)
	{
		Vector3 vector = ComputeCenter();
		volume = ComputeVolume();
		float num = vA.X - vector.X;
		float num2 = vA.Y - vector.Y;
		float num3 = vA.Z - vector.Z;
		Matrix3X3 a = new Matrix3X3(1f / 3f * (num2 * num2 + num3 * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * (num * num + num3 * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * (num * num + num2 * num2));
		num = vB.X - vector.X;
		num2 = vB.Y - vector.Y;
		num3 = vB.Z - vector.Z;
		Matrix3X3 b = new Matrix3X3(1f / 3f * (num2 * num2 + num3 * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * (num * num + num3 * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * (num * num + num2 * num2));
		Matrix3X3.Add(ref a, ref b, out a);
		num = vC.X - vector.X;
		num2 = vC.Y - vector.Y;
		num3 = vC.Z - vector.Z;
		b = new Matrix3X3(1f / 3f * (num2 * num2 + num3 * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num) * num2), 1f / 3f * (num * num + num3 * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * ((0f - num) * num3), 1f / 3f * ((0f - num2) * num3), 1f / 3f * (num * num + num2 * num2));
		Matrix3X3.Add(ref a, ref b, out a);
		return a;
	}

	/// <summary>
	/// Computes the center of the shape.  This can be considered its 
	/// center of mass.
	/// </summary>
	/// <returns>Center of the shape.</returns>
	public override Vector3 ComputeCenter()
	{
		return (vA + vB + vC) / 3f;
	}

	/// <summary>
	/// Computes the center of the shape.  This can be considered its 
	/// center of mass.  This calculation is often associated with the 
	/// volume calculation, which is given by this method as well.
	/// </summary>
	/// <param name="volume">Volume of the shape.</param>
	/// <returns>Center of the shape.</returns>
	public override Vector3 ComputeCenter(out float volume)
	{
		volume = ComputeVolume();
		return ComputeCenter();
	}

	/// <summary>
	/// Computes the volume of the shape.
	/// </summary>
	/// <returns>Volume of the shape.</returns>
	public override float ComputeVolume()
	{
		return Vector3.Cross(vB - vA, vC - vA).Length() * collisionMargin;
	}

	/// <summary>
	///  Gets the normal of the triangle shape in its local space.
	/// </summary>
	/// <returns>The local normal.</returns>
	public Vector3 GetLocalNormal()
	{
		Vector3.Subtract(ref vB, ref vA, out var result);
		Vector3.Subtract(ref vC, ref vA, out var result2);
		Vector3.Cross(ref result, ref result2, out var result3);
		result3.Normalize();
		return result3;
	}

	/// <summary>
	/// Gets the normal of the triangle in world space.
	/// </summary>
	/// <param name="transform">World transform.</param>
	/// <returns>Normal of the triangle in world space.</returns>
	public Vector3 GetNormal(RigidTransform transform)
	{
		Vector3 value = GetLocalNormal();
		Vector3.Transform(ref value, ref transform.Orientation, out value);
		return value;
	}

	/// <summary>
	/// Gets the intersection between the triangle and the ray.
	/// </summary>
	/// <param name="ray">Ray to test against the triangle.</param>
	/// <param name="transform">Transform to apply to the triangle shape for the test.</param>
	/// <param name="maximumLength">Maximum distance to travel in units of the direction vector's length.</param>
	/// <param name="hit">Hit data of the ray cast, if any.</param>
	/// <returns>Whether or not the ray hit the target.</returns>
	public override bool RayTest(ref Ray ray, ref RigidTransform transform, float maximumLength, out RayHit hit)
	{
		Matrix3X3.CreateFromQuaternion(ref transform.Orientation, out var _);
		Quaternion.Conjugate(ref transform.Orientation, out var result2);
		Ray ray2 = default(Ray);
		Vector3.Transform(ref ray.Direction, ref result2, out ray2.Direction);
		Vector3.Subtract(ref ray.Position, ref transform.Position, out ray2.Position);
		Vector3.Transform(ref ray2.Position, ref result2, out ray2.Position);
		bool result3 = Toolbox.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref vA, ref vB, ref vC, out hit);
		Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
		Vector3.Add(ref ray.Position, ref hit.Location, out hit.Location);
		Vector3.Transform(ref hit.Normal, ref transform.Orientation, out hit.Normal);
		return result3;
	}

	/// <summary>
	/// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
	/// </summary>
	/// <returns>
	/// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
	/// </returns>
	/// <filterpriority>2</filterpriority>
	public override string ToString()
	{
		return string.Concat(vA, ", ", vB, ", ", vC);
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<TriangleShape>(this);
	}
}
