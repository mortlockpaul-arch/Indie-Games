using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Ball-like shape.
/// </summary>
public class SphereShape : ConvexShape
{
	/// <summary>
	///  Gets or sets the radius of the sphere.
	/// </summary>
	public float Radius
	{
		get
		{
			return collisionMargin;
		}
		set
		{
			base.CollisionMargin = value;
		}
	}

	/// <summary>
	///  Constructs a new sphere shape.
	/// </summary>
	/// <param name="radius">Radius of the sphere.</param>
	public SphereShape(float radius)
	{
		Radius = radius;
	}

	/// <summary>
	/// Gets the bounding box of the shape given a transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use.</param>
	/// <param name="boundingBox">Bounding box of the transformed shape.</param>
	public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		boundingBox.Min.X = shapeTransform.Position.X - collisionMargin;
		boundingBox.Min.Y = shapeTransform.Position.Y - collisionMargin;
		boundingBox.Min.Z = shapeTransform.Position.Z - collisionMargin;
		boundingBox.Max.X = shapeTransform.Position.X + collisionMargin;
		boundingBox.Max.Y = shapeTransform.Position.Y + collisionMargin;
		boundingBox.Max.Z = shapeTransform.Position.Z + collisionMargin;
	}

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		extremePoint = Toolbox.ZeroVector;
	}

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		return Radius;
	}

	/// <summary>
	///  Computes the minimum radius of the shape.
	///  This is often smaller than the actual minimum radius;
	///  it is simply an approximation that avoids overestimating.
	/// </summary>
	/// <returns>Minimum radius of the shape.</returns>
	public override float ComputeMinimumRadius()
	{
		return Radius;
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
		Matrix3X3 result = default(Matrix3X3);
		result.M33 = (result.M22 = (result.M11 = 0.4f * Radius * Radius));
		volume = ComputeVolume();
		return result;
	}

	/// <summary>
	/// Gets the intersection between the sphere and the ray.
	/// </summary>
	/// <param name="ray">Ray to test against the sphere.</param>
	/// <param name="transform">Transform applied to the convex for the test.</param>
	/// <param name="maximumLength">Maximum distance to travel in units of the ray direction's length.</param>
	/// <param name="hit">Ray hit data, if any.</param>
	/// <returns>Whether or not the ray hit the target.</returns>
	public override bool RayTest(ref Ray ray, ref RigidTransform transform, float maximumLength, out RayHit hit)
	{
		return Toolbox.RayCastSphere(ref ray, ref transform.Position, collisionMargin, maximumLength, out hit);
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
	/// Computes the volume of the shape.
	/// </summary>
	/// <returns>Volume of the shape.</returns>
	public override float ComputeVolume()
	{
		return (float)(4.18878915758884 * (double)Radius * (double)Radius * (double)Radius);
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<SphereShape>(this);
	}
}
