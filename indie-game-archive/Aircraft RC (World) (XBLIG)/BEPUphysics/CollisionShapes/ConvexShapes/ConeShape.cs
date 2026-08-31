using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Symmetrical shape with a circular base and a point at the top.
/// </summary>
public class ConeShape : ConvexShape
{
	private float radius;

	private float height;

	/// <summary>
	///  Gets or sets the height of the cone.
	/// </summary>
	public float Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the radius of the cone base.
	/// </summary>
	public float Radius
	{
		get
		{
			return radius;
		}
		set
		{
			radius = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Constructs a new cone shape.
	/// </summary>
	/// <param name="height">Height of the cone.</param>
	/// <param name="radius">Radius of the cone base.</param>
	public ConeShape(float height, float radius)
	{
		this.height = height;
		Radius = radius;
	}

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		float num = radius * radius / (radius * radius + height * height);
		if (direction.Y > 0f && direction.Y * direction.Y >= direction.LengthSquared() * num)
		{
			extremePoint = new Vector3(0f, 0.75f * height, 0f);
			return;
		}
		float num2 = direction.X * direction.X + direction.Z * direction.Z;
		if (num2 > 1E-07f)
		{
			double num3 = (double)radius / Math.Sqrt(num2);
			extremePoint = new Vector3((float)(num3 * (double)direction.X), -0.25f * height, (float)(num3 * (double)direction.Z));
		}
		else
		{
			extremePoint = new Vector3(0f, -0.25f * height, 0f);
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
		double num = radius / height;
		num /= Math.Sqrt(num * num + 1.0);
		return (float)((double)collisionMargin + Math.Min(0.25f * height, num * 0.75 * (double)height));
	}

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		return (float)((double)collisionMargin + Math.Max(0.75 * (double)Height, Math.Sqrt(0.0625f * Height * Height + Radius * Radius)));
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
		float m = (result.M11 = 0.1f * Height * Height + 0.15f * Radius * Radius);
		result.M22 = 0.3f * Radius * Radius;
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
		return (float)(1.0471965039990465 * (double)Radius * (double)Radius * (double)Height);
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<ConeShape>(this);
	}
}
