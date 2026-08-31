using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Shape which can take any convex shape and use a linear transform to shear, scale, and rotate it.
/// </summary>
public class TransformableShape : ConvexShape
{
	protected ConvexShape shape;

	protected Matrix3X3 transform;

	/// <summary>
	///  Gets or sets the convex shape to be transformed.
	/// </summary>
	public ConvexShape Shape
	{
		get
		{
			return shape;
		}
		set
		{
			shape = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the linear transform used to transform the convex shape.
	/// </summary>
	public Matrix3X3 Transform
	{
		get
		{
			return transform;
		}
		set
		{
			transform = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Constructs a new transformable shape.
	/// </summary>
	/// <param name="shape">Base shape to transform.</param>
	/// <param name="transform">Transform to use.</param>
	public TransformableShape(ConvexShape shape, Matrix3X3 transform)
	{
		this.shape = shape;
		Transform = transform;
	}

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		Matrix3X3.TransformTranspose(ref direction, ref transform, out var result);
		shape.GetLocalExtremePoint(result, out extremePoint);
		Matrix3X3.Transform(ref extremePoint, ref transform, out extremePoint);
	}

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		RigidTransform shapeTransform = RigidTransform.Identity;
		GetBoundingBox(ref shapeTransform, out var boundingBox);
		Vector3.Subtract(ref boundingBox.Max, ref boundingBox.Min, out var result);
		return result.Length();
	}

	/// <summary>
	///  Computes the minimum radius of the shape.
	///  This is often smaller than the actual minimum radius;
	///  it is simply an approximation that avoids overestimating.
	/// </summary>
	/// <returns>Minimum radius of the shape.</returns>
	public override float ComputeMinimumRadius()
	{
		Vector3 direction = new Vector3(1f, 1f, 1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint);
		direction = new Vector3(-1f, -1f, 1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint2);
		direction = new Vector3(-1f, 1f, -1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint3);
		direction = new Vector3(1f, -1f, -1f);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint4);
		Vector3.Subtract(ref extremePoint2, ref extremePoint, out var result);
		Vector3.Subtract(ref extremePoint2, ref extremePoint3, out var result2);
		Vector3.Subtract(ref extremePoint3, ref extremePoint, out var result3);
		Vector3.Subtract(ref extremePoint4, ref extremePoint, out var result4);
		Vector3.Subtract(ref extremePoint4, ref extremePoint3, out var result5);
		Vector3.Cross(ref result3, ref result, out var result6);
		Vector3.Cross(ref result5, ref result2, out var result7);
		Vector3.Cross(ref result4, ref result3, out var result8);
		Vector3.Cross(ref result, ref result4, out var result9);
		Vector3.Dot(ref extremePoint, ref result6, out var result10);
		Vector3.Dot(ref extremePoint3, ref result7, out var result11);
		Vector3.Dot(ref extremePoint, ref result8, out var result12);
		Vector3.Dot(ref extremePoint, ref result9, out var result13);
		result10 /= result6.Length();
		result11 /= result7.Length();
		result12 /= result8.Length();
		result13 /= result9.Length();
		return collisionMargin + Math.Min(result10, Math.Min(result11, Math.Min(result12, result13)));
	}

	/// <summary>
	/// Computes the center of the shape.  This can be considered its 
	/// center of mass.
	/// </summary>
	/// <returns>Center of the shape.</returns>
	public override Vector3 ComputeCenter()
	{
		return Vector3.Zero;
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
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<TransformableShape>(this);
	}
}
