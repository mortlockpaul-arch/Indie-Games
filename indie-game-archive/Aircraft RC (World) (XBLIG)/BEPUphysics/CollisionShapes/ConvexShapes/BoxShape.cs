using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Convex shape with width, length, and height.
/// </summary>
public class BoxShape : ConvexShape
{
	internal float halfWidth;

	internal float halfHeight;

	internal float halfLength;

	/// <summary>
	/// Width of the box divided by two.
	/// </summary>
	public float HalfWidth
	{
		get
		{
			return halfWidth;
		}
		set
		{
			halfWidth = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	/// Height of the box divided by two.
	/// </summary>
	public float HalfHeight
	{
		get
		{
			return halfHeight;
		}
		set
		{
			halfHeight = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	/// Length of the box divided by two.
	/// </summary>
	public float HalfLength
	{
		get
		{
			return halfLength;
		}
		set
		{
			halfLength = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	/// Width of the box.
	/// </summary>
	public float Width
	{
		get
		{
			return halfWidth * 2f;
		}
		set
		{
			halfWidth = value / 2f;
			OnShapeChanged();
		}
	}

	/// <summary>
	/// Height of the box.
	/// </summary>
	public float Height
	{
		get
		{
			return halfHeight * 2f;
		}
		set
		{
			halfHeight = value / 2f;
			OnShapeChanged();
		}
	}

	/// <summary>
	/// Length of the box.
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
	///  Constructs a new box shape.
	/// </summary>
	/// <param name="width">Width of the box.</param>
	/// <param name="height">Height of the box.</param>
	/// <param name="length">Length of the box.</param>
	public BoxShape(float width, float height, float length)
	{
		halfWidth = width * 0.5f;
		halfHeight = height * 0.5f;
		halfLength = length * 0.5f;
		OnShapeChanged();
	}

	/// <summary>
	/// Gets the bounding box of the shape given a transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use.</param>
	/// <param name="boundingBox">Bounding box of the transformed shape.</param>
	public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		Matrix3X3.CreateFromQuaternion(ref shapeTransform.Orientation, out var result);
		Vector3 v = new Vector3((float)Math.Sign(result.M11) * halfWidth, (float)Math.Sign(result.M21) * halfHeight, (float)Math.Sign(result.M31) * halfLength);
		Vector3 v2 = new Vector3((float)Math.Sign(result.M12) * halfWidth, (float)Math.Sign(result.M22) * halfHeight, (float)Math.Sign(result.M32) * halfLength);
		Vector3 v3 = new Vector3((float)Math.Sign(result.M13) * halfWidth, (float)Math.Sign(result.M23) * halfHeight, (float)Math.Sign(result.M33) * halfLength);
		Matrix3X3.Transform(ref v, ref result, out v);
		Matrix3X3.Transform(ref v2, ref result, out v2);
		Matrix3X3.Transform(ref v3, ref result, out v3);
		boundingBox.Max.X = shapeTransform.Position.X + v.X;
		boundingBox.Max.Y = shapeTransform.Position.Y + v2.Y;
		boundingBox.Max.Z = shapeTransform.Position.Z + v3.Z;
		boundingBox.Min.X = shapeTransform.Position.X - v.X;
		boundingBox.Min.Y = shapeTransform.Position.Y - v2.Y;
		boundingBox.Min.Z = shapeTransform.Position.Z - v3.Z;
	}

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		extremePoint = new Vector3((float)Math.Sign(direction.X) * (halfWidth - collisionMargin), (float)Math.Sign(direction.Y) * (halfHeight - collisionMargin), (float)Math.Sign(direction.Z) * (halfLength - collisionMargin));
	}

	/// <summary>
	///  Computes the minimum radius of the shape.
	///  This is often smaller than the actual minimum radius;
	///  it is simply an approximation that avoids overestimating.
	/// </summary>
	/// <returns>Minimum radius of the shape.</returns>
	public override float ComputeMinimumRadius()
	{
		return Math.Min(halfWidth, Math.Min(halfHeight, halfLength));
	}

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		return (float)Math.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight + halfLength * halfLength);
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
		float num = halfWidth * halfWidth;
		float num2 = halfHeight * halfHeight;
		float num3 = halfLength * halfLength;
		result.M11 = (num2 + num3) * (1f / 3f);
		result.M22 = (num + num3) * (1f / 3f);
		result.M33 = (num + num2) * (1f / 3f);
		volume = ComputeVolume();
		return result;
	}

	/// <summary>
	/// Gets the intersection between the box and the ray.
	/// </summary>
	/// <param name="ray">Ray to test against the box.</param>
	/// <param name="transform">Transform of the shape.</param>
	/// <param name="maximumLength">Maximum distance to travel in units of the direction vector's length.</param>
	/// <param name="hit">Hit data for the raycast, if any.</param>
	/// <returns>Whether or not the ray hit the target.</returns>
	public override bool RayTest(ref Ray ray, ref RigidTransform transform, float maximumLength, out RayHit hit)
	{
		hit = default(RayHit);
		Quaternion.Conjugate(ref transform.Orientation, out var result);
		Vector3.Subtract(ref ray.Position, ref transform.Position, out var result2);
		Vector3.Transform(ref result2, ref result, out result2);
		Vector3.Transform(ref ray.Direction, ref result, out var result3);
		Vector3 value = Toolbox.ZeroVector;
		float num = 0f;
		float val = maximumLength;
		if (Math.Abs(result3.X) < 1E-07f && (result2.X < 0f - halfWidth || result2.X > halfWidth))
		{
			return false;
		}
		float num2 = 1f / result3.X;
		float num3 = (0f - halfWidth - result2.X) * num2;
		float num4 = (halfWidth - result2.X) * num2;
		Vector3 vector = new Vector3(-1f, 0f, 0f);
		float num5;
		if (num3 > num4)
		{
			num5 = num3;
			num3 = num4;
			num4 = num5;
			vector *= -1f;
		}
		num5 = num;
		num = Math.Max(num, num3);
		if (num5 != num)
		{
			value = vector;
		}
		val = Math.Min(val, num4);
		if (num > val)
		{
			return false;
		}
		if (Math.Abs(result3.Y) < 1E-07f && (result2.Y < 0f - halfHeight || result2.Y > halfHeight))
		{
			return false;
		}
		num2 = 1f / result3.Y;
		num3 = (0f - halfHeight - result2.Y) * num2;
		num4 = (halfHeight - result2.Y) * num2;
		vector = new Vector3(0f, -1f, 0f);
		if (num3 > num4)
		{
			num5 = num3;
			num3 = num4;
			num4 = num5;
			vector *= -1f;
		}
		num5 = num;
		num = Math.Max(num, num3);
		if (num5 != num)
		{
			value = vector;
		}
		val = Math.Min(val, num4);
		if (num > val)
		{
			return false;
		}
		if (Math.Abs(result3.Z) < 1E-07f && (result2.Z < 0f - halfLength || result2.Z > halfLength))
		{
			return false;
		}
		num2 = 1f / result3.Z;
		num3 = (0f - halfLength - result2.Z) * num2;
		num4 = (halfLength - result2.Z) * num2;
		vector = new Vector3(0f, 0f, -1f);
		if (num3 > num4)
		{
			num5 = num3;
			num3 = num4;
			num4 = num5;
			vector *= -1f;
		}
		num5 = num;
		num = Math.Max(num, num3);
		if (num5 != num)
		{
			value = vector;
		}
		val = Math.Min(val, num4);
		if (num > val)
		{
			return false;
		}
		hit.T = num;
		Vector3.Multiply(ref ray.Direction, num, out hit.Location);
		Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
		Vector3.Transform(ref value, ref transform.Orientation, out value);
		hit.Normal = value;
		return true;
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
		return 8f * halfWidth * halfLength * halfHeight;
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<BoxShape>(this);
	}
}
