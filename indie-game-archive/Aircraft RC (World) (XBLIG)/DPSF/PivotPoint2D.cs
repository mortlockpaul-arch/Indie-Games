using System;
using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Class to hold and update an object's 2D Pivot Point (point to rotate around), Pivot Velocity, and 
/// Pivot Acceleration. This class requires a Position2D object, and optionally a Orientation2D object,
/// that should be affected by rotations around the Pivot Point.
/// </summary>
public class PivotPoint2D
{
	/// <summary>
	/// The 2D Pivot Point that the object should rotate around.
	/// <para>NOTE: This only has effect when Rotational Pivot Velocity / Acceleration are used.</para>
	/// </summary>
	public Vector2 PivotPoint = Vector2.Zero;

	/// <summary>
	/// The object's Rotational Velocity around the Pivot Point (Position change).
	/// <para>NOTE: Rotations are specified in radians.</para>
	/// </summary>
	public float PivotRotationalVelocity;

	/// <summary>
	/// The object's Rotational Acceleration around the Pivot Point (Position change).
	/// <para>NOTE: Rotations are specified in radians.</para>
	/// </summary>
	public float PivotRotationalAcceleration;

	private Position2D mcPositionData;

	private Orientation2D mcOrientationData;

	private bool mbRotateOrientationToo = true;

	/// <summary>
	/// Get / Set the Position2D object that the Pivot Point should affect
	/// </summary>
	public Position2D PositionData
	{
		get
		{
			return mcPositionData;
		}
		set
		{
			mcPositionData = value;
		}
	}

	/// <summary>
	/// Get / Set the Orientation2D object that the Pivot Point should affect
	/// </summary>
	public Orientation2D OrientationData
	{
		get
		{
			return mcOrientationData;
		}
		set
		{
			mcOrientationData = value;
		}
	}

	/// <summary>
	/// Specify if the Update() function should Rotate the object's Orientation too when it
	/// rotates the object around the Pivot Point
	/// </summary>
	public bool RotateOrientationToo
	{
		get
		{
			return mbRotateOrientationToo;
		}
		set
		{
			mbRotateOrientationToo = value;
		}
	}

	/// <summary>
	/// Copy Constructor
	/// </summary>
	/// <param name="cPivotPointToCopy">The PivotPoint2D object to copy</param>
	public PivotPoint2D(PivotPoint2D cPivotPointToCopy)
	{
		CopyFrom(cPivotPointToCopy);
	}

	/// <summary>
	/// Copies the given PivotPoint2D object's data into this object's data
	/// </summary>
	/// <param name="cPivotPointToCopy"></param>
	public void CopyFrom(PivotPoint2D cPivotPointToCopy)
	{
		PivotPoint = cPivotPointToCopy.PivotPoint;
		PivotRotationalVelocity = cPivotPointToCopy.PivotRotationalVelocity;
		PivotRotationalAcceleration = cPivotPointToCopy.PivotRotationalAcceleration;
		mbRotateOrientationToo = cPivotPointToCopy.RotateOrientationToo;
		mcPositionData = new Position2D(cPivotPointToCopy.PositionData);
		mcOrientationData = new Orientation2D(cPivotPointToCopy.OrientationData);
	}

	/// <summary>
	/// Explicit Constructor. Set the Position2D object that should be affected by rotations around
	/// this Pivot Point.
	/// </summary>
	/// <param name="cPosition">Handle to the Position2D object to update</param>
	public PivotPoint2D(Position2D cPosition)
	{
		mcPositionData = cPosition;
		mcOrientationData = null;
	}

	/// <summary>
	/// Explicit Constructor. Set the Position2D and Orientation2D objects that should be affected by 
	/// rotational around this Pivot Point.
	/// </summary>
	/// <param name="cPosition">Handle to the Position2D object to update</param>
	/// <param name="cOrientation">Handle to the Orienetation2D object to update</param>
	public PivotPoint2D(Position2D cPosition, Orientation2D cOrientation)
	{
		mcPositionData = cPosition;
		mcOrientationData = cOrientation;
	}

	/// <summary>
	/// Rotates the object about its center, changing its Orientation, as well as around the 
	/// specified Pivot Point, changing its Position
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to apply to the object</param>
	/// <param name="sPivotPoint">The Point to rotate the object around</param>
	public void RotatePositionAndOrientation(float fRotation, Vector2 sPivotPoint)
	{
		if (mcOrientationData != null)
		{
			mcOrientationData.Rotate(fRotation);
		}
		RotatePosition(fRotation, sPivotPoint);
	}

	/// <summary>
	/// Rotates the object about its center, changing its Orientation, as well as around the 
	/// specified 3D Pivot Point, changing its 2D Position.
	/// <para>NOTE: The Pivot Point's Z-value is ignored.</para>
	/// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to apply to the object</param>
	/// <param name="sPivotPoint">The Point to rotate the object around. 
	/// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
	public void RotatePositionAndOrientationVector3(float fRotation, Vector3 sPivotPoint)
	{
		RotatePositionAndOrientation(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y));
	}

	/// <summary>
	/// Rotates the object around the specified Pivot Point, changing its Position, without 
	/// changing its Orientation.
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to apply to the object</param>
	/// <param name="sPivotPoint">The Point to rotate the object around</param>
	public void RotatePosition(float fRotation, Vector2 sPivotPoint)
	{
		mcPositionData.Position = RotatePosition(fRotation, sPivotPoint, mcPositionData.Position);
	}

	/// <summary>
	/// Rotates the object around the specified Pivot Point, changing its Position, without 
	/// changing its Orientation.
	/// <para>NOTE: The Pivot Point's Z-value is ignored.</para>
	/// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to apply to the object</param>
	/// <param name="sPivotPoint">The Point to rotate the object around.
	/// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
	public void RotatePositionVector3(float fRotation, Vector3 sPivotPoint)
	{
		RotatePosition(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y));
	}

	/// <summary>
	/// Update the Position and Orientation according to the Pivot Rotational Velocity / Acceleration
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
	public void Update(float fElapsedTimeInSeconds)
	{
		if (PivotRotationalAcceleration != 0f)
		{
			PivotRotationalVelocity += PivotRotationalAcceleration * fElapsedTimeInSeconds;
		}
		if (PivotRotationalVelocity != 0f)
		{
			float fRotation = PivotRotationalVelocity * fElapsedTimeInSeconds;
			if (mbRotateOrientationToo)
			{
				RotatePositionAndOrientation(fRotation, PivotPoint);
			}
			else
			{
				RotatePosition(fRotation, PivotPoint);
			}
		}
	}

	/// <summary>
	/// Rotates the given Position and Orientation around the Pivot Point, changing the Position and Orientation
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to apply to the object</param>
	/// <param name="sPivotPoint">The Point to rotate the object around</param>
	/// <param name="srPosition">The Position of the object to be modified</param>
	/// <param name="frOrientation">The Orientation (rotation) of the object to be modified</param>
	public static void RotatePositionAndOrientation(float fRotation, Vector2 sPivotPoint, ref Vector2 srPosition, ref float frOrientation)
	{
		frOrientation = Orientation2D.Rotate(fRotation, frOrientation);
		srPosition = RotatePosition(fRotation, sPivotPoint, srPosition);
	}

	/// <summary>
	/// Rotates the given Position and Orientation around the Pivot Point, changing the Position and Orientation.
	/// <para>NOTE: The Pivot Point and Position's Z-values are ignored.</para>
	/// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to apply to the object</param>
	/// <param name="sPivotPoint">The Point to rotate the object around.
	/// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
	/// <param name="srPosition">The Position of the object to be modified.
	/// NOTE: The Z-value is ignored and will not be changed, since this is a 2D rotation.</param>
	/// <param name="frOrientation">The Orientation (rotation) of the object to be modified</param>
	public static void RotatePositionAndOrientationVector3(float fRotation, Vector3 sPivotPoint, ref Vector3 srPosition, ref float frOrientation)
	{
		Vector2 srPosition2 = new Vector2(srPosition.X, srPosition.Y);
		RotatePositionAndOrientation(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y), ref srPosition2, ref frOrientation);
		srPosition.X = srPosition2.X;
		srPosition.Y = srPosition2.Y;
	}

	/// <summary>
	/// Returns the new Position after Rotating the given Position around the specified Pivot Point
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to rotate around the Pivot Point by</param>
	/// <param name="sPivotPoint">The Point to Rotate around</param>
	/// <param name="sPosition">The current Position of the object</param>
	/// <returns>Returns the new Position after Rotating the given Position around the specified Pivot Point</returns>
	public static Vector2 RotatePosition(float fRotation, Vector2 sPivotPoint, Vector2 sPosition)
	{
		if (sPivotPoint != sPosition)
		{
			Vector2 vector = sPosition - sPivotPoint;
			float num = (float)Math.Atan(vector.Y / vector.X);
			float num2 = num + fRotation;
			float num3 = vector.Length();
			if (vector.X < 0f)
			{
				num3 *= -1f;
			}
			float x = (float)Math.Cos(num2) * num3;
			float y = (float)Math.Sin(num2) * num3;
			sPosition = sPivotPoint + new Vector2(x, y);
		}
		return sPosition;
	}

	/// <summary>
	/// Returns the new Position after Rotating the given Position around the specified Pivot Point.
	/// <para>NOTE: The Pivot Point and Position's Z-values are ignored.</para>
	/// <para>This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
	/// </summary>
	/// <param name="fRotation">The Rotation in radians to rotate around the Pivot Point by</param>
	/// <param name="sPivotPoint">The Point to Rotate around.
	/// NOTE: The Z-value is ignored, since this is a 2D rotation.</param>
	/// <param name="sPosition">The current Position of the object.
	/// NOTE: The Z-value is ignored and will not be changed, since this is a 2D rotation.</param>
	/// <returns>Returns the new Position after Rotating the given Position around the specified Pivot Point.</returns>
	public static Vector3 RotatePositionVector3(float fRotation, Vector3 sPivotPoint, Vector3 sPosition)
	{
		Vector2 vector = RotatePosition(fRotation, new Vector2(sPivotPoint.X, sPivotPoint.Y), new Vector2(sPosition.X, sPosition.Y));
		return new Vector3(vector.X, vector.Y, sPosition.Z);
	}
}
