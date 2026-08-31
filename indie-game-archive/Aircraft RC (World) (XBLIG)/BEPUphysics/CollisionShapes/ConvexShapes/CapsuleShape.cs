using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Sphere-expanded line segment.  Another way of looking at it is a cylinder with half-spheres on each end.
/// </summary>
public class CapsuleShape : ConvexShape
{
	private float halfLength;

	/// <summary>
	///  Gets or sets the length of the capsule's inner line segment.
	/// </summary>
	public float Length
	{
		get
		{
			return halfLength * 2f;
		}
		set
		{
			halfLength = value / 2f;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the radius of the capsule.
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
	///  Constructs a new capsule shape.
	/// </summary>
	/// <param name="length">Length of the capsule's inner line segment.</param>
	/// <param name="radius">Radius to expand the line segment width.</param>
	public CapsuleShape(float length, float radius)
	{
		halfLength = length * 0.5f;
		Radius = radius;
	}

	public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		Vector3 value = new Vector3(0f, halfLength, 0f);
		Vector3 value2 = new Vector3(0f, 0f - halfLength, 0f);
		Vector3.Transform(ref value, ref shapeTransform.Orientation, out value);
		Vector3.Transform(ref value2, ref shapeTransform.Orientation, out value2);
		if (value.X > value2.X)
		{
			boundingBox.Max.X = value.X;
			boundingBox.Min.X = value2.X;
		}
		else
		{
			boundingBox.Max.X = value2.X;
			boundingBox.Min.X = value.X;
		}
		if (value.Y > value2.Y)
		{
			boundingBox.Max.Y = value.Y;
			boundingBox.Min.Y = value2.Y;
		}
		else
		{
			boundingBox.Max.Y = value2.Y;
			boundingBox.Min.Y = value.Y;
		}
		if (value.Z > value2.Z)
		{
			boundingBox.Max.Z = value.Z;
			boundingBox.Min.Z = value2.Z;
		}
		else
		{
			boundingBox.Max.Z = value2.Z;
			boundingBox.Min.Z = value.Z;
		}
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
		if (direction.Y > 0f)
		{
			extremePoint = new Vector3(0f, halfLength, 0f);
		}
		else if (direction.Y < 0f)
		{
			extremePoint = new Vector3(0f, 0f - halfLength, 0f);
		}
		else
		{
			extremePoint = Toolbox.ZeroVector;
		}
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
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		return halfLength + Radius;
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
		volume = ComputeVolume();
		Matrix3X3 result = default(Matrix3X3);
		float num = Length + Radius / 2f;
		float m = (result.M11 = 1f / 12f * num * num + 0.25f * Radius * Radius);
		result.M22 = 0.5f * Radius * Radius;
		result.M33 = m;
		return result;
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
		return (float)(Math.PI * (double)Radius * (double)Radius * (double)Length + 4.18878915758884 * (double)Radius * (double)Radius * (double)Radius);
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<CapsuleShape>(this);
	}
}
