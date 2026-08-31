using System;
using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Class to hold and update an object's 3D Orientation, Rotational Velocity, and Rotational Acceleration
/// </summary>
public class Orientation3D
{
	/// <summary>
	/// The object's Orientation
	/// </summary>
	public Quaternion Orientation = Quaternion.Identity;

	/// <summary>
	/// The object's Rotational Velocity around its center.
	/// <para>NOTE: Rotations are specified by giving a 3D Vector, where the direction is the axis to 
	/// rotate around, and the vector length is the amount (angle in radians) to rotate.
	/// It can also be thought of as Vector(PitchVelocity, YawVelocity, RollVelocity).</para>
	/// </summary>
	public Vector3 RotationalVelocity = Vector3.Zero;

	/// <summary>
	/// The object's Rotational Acceleration around its center.
	/// <para>NOTE: Rotations are specified by giving a 3D Vector, where the direction is the axis to 
	///  rotate around, and the vector length is the amount (angle in radians) to rotate.
	///  It can also be thought of as Vector(PitchAcceleration, YawAcceleration, RollAcceleration).</para>
	/// </summary>
	public Vector3 RotationalAcceleration = Vector3.Zero;

	/// <summary>
	/// Get / Set the Normal (i.e. Forward) direction of the object (i.e. which direction it is facing)
	/// </summary>
	public Vector3 Normal
	{
		get
		{
			return GetNormalDirection(Orientation);
		}
		set
		{
			SetNormalDirection(ref Orientation, value);
		}
	}

	/// <summary>
	/// Get / Set the Up direction of the object
	/// </summary>
	public Vector3 Up
	{
		get
		{
			return GetUpDirection(Orientation);
		}
		set
		{
			SetUpDirection(ref Orientation, value);
		}
	}

	/// <summary>
	/// Get / Set the Right direction of the object
	/// </summary>
	public Vector3 Right
	{
		get
		{
			return GetRightDirection(Orientation);
		}
		set
		{
			SetRightDirection(ref Orientation, value);
		}
	}

	/// <summary>
	/// Default Constructor
	/// </summary>
	public Orientation3D()
	{
	}

	/// <summary>
	/// Copy Constructor
	/// </summary>
	/// <param name="cOrienationToCopy">The Orienation3D object to copy</param>
	public Orientation3D(Orientation3D cOrienationToCopy)
	{
		CopyFrom(cOrienationToCopy);
	}

	/// <summary>
	/// Copies the given Orientation3D object's data into this object's data
	/// </summary>
	/// <param name="cOrientationToCopy">The Orientation3D object to copy from</param>
	public void CopyFrom(Orientation3D cOrientationToCopy)
	{
		Orientation = cOrientationToCopy.Orientation;
		RotationalVelocity = cOrientationToCopy.RotationalVelocity;
		RotationalAcceleration = cOrientationToCopy.RotationalAcceleration;
	}

	/// <summary>
	/// Rotates the object about its center, changing its Orientation
	/// </summary>
	/// <param name="sRotationMatrix">The Rotation to apply to the object</param>
	public void Rotate(Matrix sRotationMatrix)
	{
		Orientation = Rotate(sRotationMatrix, Orientation);
	}

	/// <summary>
	/// Update the Position and Velocity according to the Acceleration, as well as the Orientation
	/// according to the Rotational Velocity and Rotational Acceleration
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
	public void Update(float fElapsedTimeInSeconds)
	{
		if (RotationalAcceleration != Vector3.Zero)
		{
			RotationalVelocity += RotationalAcceleration * fElapsedTimeInSeconds;
		}
		if (RotationalVelocity != Vector3.Zero)
		{
			Orientation.Normalize();
			Quaternion quaternion = new Quaternion(RotationalVelocity * (fElapsedTimeInSeconds * 0.5f), 0f);
			Orientation += Orientation * quaternion;
		}
	}

	/// <summary>
	/// Returns the given Quaternion rotated about its center, changing its Orientation
	/// </summary>
	/// <param name="sRotationMatrix">The Rotation to apply to the Quaternion</param>
	/// <param name="sQuaterionToRotate">The Quaternion that should be Rotated</param>
	/// <returns>Returns the given Quaternion rotated about its center, changing its Orientation</returns>
	public static Quaternion Rotate(Matrix sRotationMatrix, Quaternion sQuaterionToRotate)
	{
		sQuaterionToRotate.Normalize();
		return sQuaterionToRotate *= Quaternion.CreateFromRotationMatrix(sRotationMatrix);
	}

	/// <summary>
	/// Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
	/// be the same as the DestinationDirection.
	/// This method is based on Stan Melax's article in Game Programming Gems, and
	/// the code was referenced from OgreVector3.h of the Ogre library (www.Ogre3d.org)
	/// </summary>
	/// <param name="CurrentDirection">The current Direction the Vector is facing</param>
	/// <param name="DesiredDirection">The Direction we want the Vector to face</param>
	/// <returns>Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
	/// be the same as the DestinationDirection.</returns>
	public static Quaternion GetRotationTo(Vector3 CurrentDirection, Vector3 DesiredDirection)
	{
		return GetRotationTo(CurrentDirection, DesiredDirection, Vector3.Zero);
	}

	/// <summary>
	/// Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
	/// be the same as the DestinationDirection.
	/// This method is based on Stan Melax's article in Game Programming Gems, and
	/// the code was referenced from OgreVector3.h of the Ogre library (www.Ogre3d.org)
	/// </summary>
	/// <param name="CurrentDirection">The current Direction the Vector is facing</param>
	/// <param name="DesiredDirection">The Direction we want the Vector to face</param>
	/// <param name="sFallbackAxis">The Axis to rotate around if a 180 degree rotation is required</param>
	/// <returns>Returns the shortest arc Quaternion Rotation needed to rotate the CurrentDirection to
	/// be the same as the DestinationDirection.</returns>
	public static Quaternion GetRotationTo(Vector3 CurrentDirection, Vector3 DesiredDirection, Vector3 sFallbackAxis)
	{
		Quaternion result = default(Quaternion);
		Vector3 vector = CurrentDirection;
		Vector3 vector2 = DesiredDirection;
		vector.Normalize();
		vector2.Normalize();
		float num = Vector3.Dot(vector, vector2);
		if (num >= 1f)
		{
			return Quaternion.Identity;
		}
		if (num <= -0.999999f)
		{
			if (sFallbackAxis != Vector3.Zero)
			{
				sFallbackAxis.Normalize();
				return Quaternion.CreateFromAxisAngle(sFallbackAxis, (float)Math.PI);
			}
			Vector3 axis = Vector3.Cross(Vector3.UnitX, vector);
			if (axis.LengthSquared() < 0.999998f)
			{
				axis = Vector3.Cross(Vector3.UnitY, vector);
			}
			axis.Normalize();
			return Quaternion.CreateFromAxisAngle(axis, (float)Math.PI);
		}
		Vector3 vector3 = Vector3.Cross(vector, vector2);
		double num2 = Math.Sqrt((1f + num) * 2f);
		double num3 = 1.0 / num2;
		result.X = (float)((double)vector3.X * num3);
		result.Y = (float)((double)vector3.Y * num3);
		result.Z = (float)((double)vector3.Z * num3);
		result.W = (float)(num2 * 0.5);
		result.Normalize();
		return result;
	}

	/// <summary>
	/// Returns a Quaternion orientated according to the given Normal and Up Directions
	/// </summary>
	/// <param name="sNormalDirection">The Normal (forward) direction that the Quaternion should face</param>
	/// <param name="sUpDirection">The Up direction that the Quaternion should have</param>
	/// <returns>Returns a Quaternion orientated according to the given Normal and Up Directions</returns>
	public static Quaternion GetQuaternionWithOrientation(Vector3 sNormalDirection, Vector3 sUpDirection)
	{
		Quaternion identity = Quaternion.Identity;
		Quaternion rotationTo = GetRotationTo(Vector3.Forward, sNormalDirection);
		identity = rotationTo * identity;
		Vector3 currentDirection = Vector3.Transform(Vector3.Up, identity);
		Quaternion rotationTo2 = GetRotationTo(currentDirection, sUpDirection);
		return rotationTo2 * identity;
	}

	/// <summary>
	/// Returns the Normal (Forward) Direction of the given Quaternion
	/// </summary>
	/// <param name="sOrientation">The Quaternion whose Direction we want</param>
	/// <returns>Returns the Normal (Forward) Direction of the given Quaternion</returns>
	public static Vector3 GetNormalDirection(Quaternion sOrientation)
	{
		return Vector3.Normalize(Vector3.Transform(Vector3.Forward, sOrientation));
	}

	/// <summary>
	/// Sets the Normal direction of the given Quaternion to be the given New Normal Direction
	/// </summary>
	/// <param name="sOrientation">The Quaternion to modify</param>
	/// <param name="sNewNormalDirection">The New Normal Direction the Quaternion should have</param>
	public static void SetNormalDirection(ref Quaternion sOrientation, Vector3 sNewNormalDirection)
	{
		Quaternion rotationTo = GetRotationTo(GetNormalDirection(sOrientation), sNewNormalDirection);
		sOrientation.Normalize();
		sOrientation = rotationTo * sOrientation;
	}

	/// <summary>
	/// Returns the Up Direction of the given Quaternion
	/// </summary>
	/// <param name="sOrientation">The Quaternion whose Direction we want</param>
	/// <returns>Returns the Up Direction of the given Quaternion</returns>
	public static Vector3 GetUpDirection(Quaternion sOrientation)
	{
		return Vector3.Normalize(Vector3.Transform(Vector3.Up, sOrientation));
	}

	/// <summary>
	/// Sets the Up direction of the given Quaternion to be the given New Up Direction
	/// </summary>
	/// <param name="sOrientation">The Quaternion to modify</param>
	/// <param name="sNewUpDirection">The New Up Direction the Quaternion should have</param>
	public static void SetUpDirection(ref Quaternion sOrientation, Vector3 sNewUpDirection)
	{
		Quaternion rotationTo = GetRotationTo(GetUpDirection(sOrientation), sNewUpDirection);
		sOrientation.Normalize();
		sOrientation = rotationTo * sOrientation;
	}

	/// <summary>
	/// Returns the Right Direction of the given Quaternion
	/// </summary>
	/// <param name="sOrientation">The Quaternion whose Direction we want</param>
	/// <returns>Returns the Right Direction of the given Quaternion</returns>
	public static Vector3 GetRightDirection(Quaternion sOrientation)
	{
		return Vector3.Normalize(Vector3.Transform(Vector3.Right, sOrientation));
	}

	/// <summary>
	/// Sets the Right direction of the given Quaternion to be the given New Right Direction
	/// </summary>
	/// <param name="sOrientation">The Quaternion to modify</param>
	/// <param name="sNewRightDirection">The New Right Direction the Quaternion should have</param>
	public static void SetRightDirection(ref Quaternion sOrientation, Vector3 sNewRightDirection)
	{
		Quaternion rotationTo = GetRotationTo(GetRightDirection(sOrientation), sNewRightDirection);
		sOrientation.Normalize();
		sOrientation = rotationTo * sOrientation;
	}
}
