using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Class used to hold and update an object's 2D Position, Velocity, and Acceleration
/// </summary>
public class Position2D
{
	/// <summary>
	/// The object's 2D Position
	/// </summary>
	public Vector2 Position = Vector2.Zero;

	/// <summary>
	/// The object's Position
	/// </summary>
	public Vector2 Velocity = Vector2.Zero;

	/// <summary>
	/// The object's Acceleration
	/// </summary>
	public Vector2 Acceleration = Vector2.Zero;

	/// <summary>
	/// Get / Set the object's 2D Position using a Vector3.
	/// <para>NOTE: The Z-value is ignored when Setting, and is given a value of zero when Getting.</para>
	/// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>
	/// </summary>
	public Vector3 PositionVector3
	{
		get
		{
			return new Vector3(Position.X, Position.Y, 0f);
		}
		set
		{
			Position = new Vector2(value.X, value.Y);
		}
	}

	/// <summary>
	/// Get / Set the object's 2D Velocity using a Vector3.
	/// <para>NOTE: The Z-value is ignored when Setting, and is given a value of zero when Getting.</para>
	/// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>para&gt;
	/// </summary>
	public Vector3 VelocityVector3
	{
		get
		{
			return new Vector3(Velocity.X, Velocity.Y, 0f);
		}
		set
		{
			Velocity = new Vector2(value.X, value.Y);
		}
	}

	/// <summary>
	/// Get / Set the object's 2D Acceleration using a Vector3.
	/// <para>NOTE: The Z-value is ignored when Setting, and is given a value of zero when Getting.</para>
	/// <para>NOTE: This function is provided for convenience when using 3D Vectors in 2D coordinate systems.</para>para&gt;
	/// </summary>
	public Vector3 AccelerationVector3
	{
		get
		{
			return new Vector3(Acceleration.X, Acceleration.Y, 0f);
		}
		set
		{
			Acceleration = new Vector2(value.X, value.Y);
		}
	}

	/// <summary>
	/// Default Constructor
	/// </summary>
	public Position2D()
	{
	}

	/// <summary>
	/// Copy Constructor
	/// </summary>
	/// <param name="cPositionToCopy">The Position2D object to copy</param>
	public Position2D(Position2D cPositionToCopy)
	{
		CopyFrom(cPositionToCopy);
	}

	/// <summary>
	/// Copies the given Position2D object's data into this objects data
	/// </summary>
	/// <param name="cPositionToCopy">The Position2D object to copy</param>
	public void CopyFrom(Position2D cPositionToCopy)
	{
		Position = cPositionToCopy.Position;
		Velocity = cPositionToCopy.Velocity;
		Acceleration = cPositionToCopy.Acceleration;
	}

	/// <summary>
	/// Update the Position and Velocity according to the Acceleration
	/// </summary>
	/// <param name="fElapsedTimeInSeconds">The time Elapsed in Seconds since the last Update</param>
	public virtual void Update(float fElapsedTimeInSeconds)
	{
		Velocity += Acceleration * fElapsedTimeInSeconds;
		Position += Velocity * fElapsedTimeInSeconds;
	}
}
