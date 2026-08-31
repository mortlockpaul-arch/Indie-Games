using System;
using BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Superclass of convex collision shapes.
/// </summary>
public abstract class ConvexShape : EntityShape
{
	protected internal float collisionMargin = CollisionDetectionSettings.DefaultMargin;

	protected internal float minimumRadius;

	protected internal float maximumRadius;

	/// <summary>
	///  Collision margin of the convex shape.  The margin is a small spherical expansion around
	///  entities which allows specialized collision detection algorithms to be used.
	///  It's recommended that this be left unchanged.
	/// </summary>
	public float CollisionMargin
	{
		get
		{
			return collisionMargin;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Collision margin must be nonnegative..");
			}
			collisionMargin = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	/// Gets or sets the minimum radius of the collidable's shape.  This is initialized to a value that is
	/// guaranteed to be equal to or smaller than the actual minimum radius.  When setting this property,
	/// ensure that the inner sphere formed by the new minimum radius is fully contained within the shape.
	/// </summary>
	public float MinimumRadius
	{
		get
		{
			return minimumRadius;
		}
		set
		{
			minimumRadius = value;
		}
	}

	/// <summary>
	/// Gets the maximum radius of the collidable's shape.  This is initialized to a value that is
	/// guaranteed to be equal to or larger than the actual maximum radius.
	/// </summary>
	public float MaximumRadius => maximumRadius;

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public abstract void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint);

	/// <summary>
	///  Gets the extreme point of the shape in world space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	///  <param name="shapeTransform">Transform to use for the shape.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public void GetExtremePointWithoutMargin(Vector3 direction, ref RigidTransform shapeTransform, out Vector3 extremePoint)
	{
		Quaternion.Conjugate(ref shapeTransform.Orientation, out var result);
		Vector3.Transform(ref direction, ref result, out direction);
		GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
		Vector3.Transform(ref extremePoint, ref shapeTransform.Orientation, out extremePoint);
		Vector3.Add(ref extremePoint, ref shapeTransform.Position, out extremePoint);
	}

	/// <summary>
	///  Gets the extreme point of the shape in world space in a given direction with margin expansion.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	///  <param name="shapeTransform">Transform to use for the shape.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public void GetExtremePoint(Vector3 direction, ref RigidTransform shapeTransform, out Vector3 extremePoint)
	{
		GetExtremePointWithoutMargin(direction, ref shapeTransform, out extremePoint);
		float num = direction.LengthSquared();
		if (num > 1E-07f)
		{
			Vector3.Multiply(ref direction, collisionMargin / (float)Math.Sqrt(num), out direction);
			Vector3.Add(ref extremePoint, ref direction, out extremePoint);
		}
	}

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction with margin expansion.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public void GetLocalExtremePoint(Vector3 direction, out Vector3 extremePoint)
	{
		GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
		float num = direction.LengthSquared();
		if (num > 1E-07f)
		{
			Vector3.Multiply(ref direction, collisionMargin / (float)Math.Sqrt(num), out direction);
			Vector3.Add(ref extremePoint, ref direction, out extremePoint);
		}
	}

	/// <summary>
	/// Gets the bounding box of the shape given a transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use.</param>
	/// <param name="boundingBox">Bounding box of the transformed shape.</param>
	public virtual void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		Matrix3X3.CreateFromQuaternion(ref shapeTransform.Orientation, out var result);
		Vector3 direction = new Vector3(result.M11, result.M21, result.M31);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint);
		direction = new Vector3(0f - result.M11, 0f - result.M21, 0f - result.M31);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint2);
		direction = new Vector3(result.M12, result.M22, result.M32);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint3);
		direction = new Vector3(0f - result.M12, 0f - result.M22, 0f - result.M32);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint4);
		direction = new Vector3(result.M13, result.M23, result.M33);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint5);
		direction = new Vector3(0f - result.M13, 0f - result.M23, 0f - result.M33);
		GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint6);
		Matrix3X3.Transform(ref extremePoint, ref result, out extremePoint);
		Matrix3X3.Transform(ref extremePoint2, ref result, out extremePoint2);
		Matrix3X3.Transform(ref extremePoint3, ref result, out extremePoint3);
		Matrix3X3.Transform(ref extremePoint4, ref result, out extremePoint4);
		Matrix3X3.Transform(ref extremePoint5, ref result, out extremePoint5);
		Matrix3X3.Transform(ref extremePoint6, ref result, out extremePoint6);
		boundingBox.Max.X = shapeTransform.Position.X + collisionMargin + extremePoint.X;
		boundingBox.Max.Y = shapeTransform.Position.Y + collisionMargin + extremePoint3.Y;
		boundingBox.Max.Z = shapeTransform.Position.Z + collisionMargin + extremePoint5.Z;
		boundingBox.Min.X = shapeTransform.Position.X - collisionMargin + extremePoint2.X;
		boundingBox.Min.Y = shapeTransform.Position.Y - collisionMargin + extremePoint4.Y;
		boundingBox.Min.Z = shapeTransform.Position.Z - collisionMargin + extremePoint6.Z;
	}

	/// <summary>
	/// Gets the intersection between the convex shape and the ray.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="transform">Transform of the convex shape.</param>
	/// <param name="maximumLength">Maximum distance to travel in units of the ray direction's length.</param>
	/// <param name="hit">Ray hit data, if any.</param>
	/// <returns>Whether or not the ray hit the target.</returns>
	public virtual bool RayTest(ref Ray ray, ref RigidTransform transform, float maximumLength, out RayHit hit)
	{
		return GJKToolbox.RayCast(ray, this, ref transform, maximumLength, out hit);
	}

	/// <summary>
	/// Computes the center of the shape.  This can be considered its 
	/// center of mass.
	/// </summary>
	/// <returns>Center of the shape.</returns>
	public override Vector3 ComputeCenter()
	{
		return InertiaHelper.ComputeCenter(this);
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
		return InertiaHelper.ComputeCenter(this, out volume);
	}

	/// <summary>
	/// Computes the volume of the shape.
	/// </summary>
	/// <returns>Volume of the shape.</returns>
	public override float ComputeVolume()
	{
		ComputeVolumeDistribution(out var volume);
		return volume;
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
		return InertiaHelper.ComputeVolumeDistribution(this, out volume);
	}

	protected override void OnShapeChanged()
	{
		base.OnShapeChanged();
		minimumRadius = ComputeMinimumRadius();
		maximumRadius = ComputeMaximumRadius();
	}

	/// <summary>
	/// Computes the volume distribution of the shape.
	/// The volume distribution can be used to compute inertia tensors when
	/// paired with mass and other tuning factors.
	/// </summary>
	/// <returns>Volume distribution of the shape.</returns>
	public override Matrix3X3 ComputeVolumeDistribution()
	{
		float volume;
		return ComputeVolumeDistribution(out volume);
	}

	public override void ComputeDistributionInformation(out ShapeDistributionInformation shapeInfo)
	{
		shapeInfo.VolumeDistribution = ComputeVolumeDistribution(out shapeInfo.Volume);
		shapeInfo.Center = ComputeCenter();
	}

	/// <summary>
	/// Gets the bounding box of the convex shape transformed first into world space, and then into the local space of another affine transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use to put the shape into world space.</param>
	/// <param name="spaceTransform">Used as the frame of reference to compute the bounding box.
	/// In effect, the shape is transformed by the inverse of the space transform to compute its bounding box in local space.</param>
	/// <param name="sweep">Vector to expand the bounding box with in local space.</param>
	/// <param name="boundingBox">Bounding box in the local space.</param>
	public void GetSweptLocalBoundingBox(ref RigidTransform shapeTransform, ref AffineTransform spaceTransform, ref Vector3 sweep, out BoundingBox boundingBox)
	{
		GetLocalBoundingBox(ref shapeTransform, ref spaceTransform, out boundingBox);
		Matrix3X3.TransformTranspose(ref sweep, ref spaceTransform.LinearTransform, out var result);
		Toolbox.ExpandBoundingBox(ref boundingBox, ref result);
	}

	/// <summary>
	/// Gets the bounding box of the convex shape transformed first into world space, and then into the local space of another affine transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use to put the shape into world space.</param>
	/// <param name="spaceTransform">Used as the frame of reference to compute the bounding box.
	/// In effect, the shape is transformed by the inverse of the space transform to compute its bounding box in local space.</param>
	/// <param name="boundingBox">Bounding box in the local space.</param>
	public void GetLocalBoundingBox(ref RigidTransform shapeTransform, ref AffineTransform spaceTransform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		AffineTransform.Invert(ref spaceTransform, out var inverse);
		AffineTransform.Multiply(ref shapeTransform, ref inverse, out inverse);
		Vector3 direction = new Vector3(inverse.LinearTransform.M11, inverse.LinearTransform.M21, inverse.LinearTransform.M31);
		GetLocalExtremePoint(direction, out var extremePoint);
		direction = new Vector3(0f - inverse.LinearTransform.M11, 0f - inverse.LinearTransform.M21, 0f - inverse.LinearTransform.M31);
		GetLocalExtremePoint(direction, out var extremePoint2);
		direction = new Vector3(inverse.LinearTransform.M12, inverse.LinearTransform.M22, inverse.LinearTransform.M32);
		GetLocalExtremePoint(direction, out var extremePoint3);
		direction = new Vector3(0f - inverse.LinearTransform.M12, 0f - inverse.LinearTransform.M22, 0f - inverse.LinearTransform.M32);
		GetLocalExtremePoint(direction, out var extremePoint4);
		direction = new Vector3(inverse.LinearTransform.M13, inverse.LinearTransform.M23, inverse.LinearTransform.M33);
		GetLocalExtremePoint(direction, out var extremePoint5);
		direction = new Vector3(0f - inverse.LinearTransform.M13, 0f - inverse.LinearTransform.M23, 0f - inverse.LinearTransform.M33);
		GetLocalExtremePoint(direction, out var extremePoint6);
		Matrix3X3.Transform(ref extremePoint, ref inverse.LinearTransform, out extremePoint);
		Matrix3X3.Transform(ref extremePoint2, ref inverse.LinearTransform, out extremePoint2);
		Matrix3X3.Transform(ref extremePoint3, ref inverse.LinearTransform, out extremePoint3);
		Matrix3X3.Transform(ref extremePoint4, ref inverse.LinearTransform, out extremePoint4);
		Matrix3X3.Transform(ref extremePoint5, ref inverse.LinearTransform, out extremePoint5);
		Matrix3X3.Transform(ref extremePoint6, ref inverse.LinearTransform, out extremePoint6);
		boundingBox.Max.X = inverse.Translation.X + extremePoint.X;
		boundingBox.Max.Y = inverse.Translation.Y + extremePoint3.Y;
		boundingBox.Max.Z = inverse.Translation.Z + extremePoint5.Z;
		boundingBox.Min.X = inverse.Translation.X + extremePoint2.X;
		boundingBox.Min.Y = inverse.Translation.Y + extremePoint4.Y;
		boundingBox.Min.Z = inverse.Translation.Z + extremePoint6.Z;
	}

	/// <summary>
	///  Computes the minimum radius of the shape.
	///  This is often smaller than the actual minimum radius;
	///  it is simply an approximation that avoids overestimating.
	/// </summary>
	/// <returns>Minimum radius of the shape.</returns>
	public abstract float ComputeMinimumRadius();

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public abstract float ComputeMaximumRadius();
}
