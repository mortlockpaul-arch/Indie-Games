using System;
using Microsoft.Xna.Framework;

namespace RenegadeEngine.Cyclone;

public class RigidBody
{
	protected Vector3 forceAccum = Vector3.Zero;

	protected Vector3 acceleration = Vector3.Zero;

	protected Vector3 prevAcceleration = Vector3.Zero;

	protected Matrix transform = Matrix.Identity;

	protected double inverseMass = 1.0;

	public double linearDamping = 1.0;

	protected bool isAwake = true;

	protected bool canSleep;

	public Vector3 Position = Vector3.Zero;

	public Vector3 Rotation = Vector3.Zero;

	public Vector3 Velocity = Vector3.Zero;

	public Quaternion Orientation = Quaternion.Identity;

	public Matrix Transform
	{
		get
		{
			return transform;
		}
		set
		{
			transform = value;
		}
	}

	public Vector3 Acceleration
	{
		get
		{
			return acceleration;
		}
		set
		{
			acceleration = value;
		}
	}

	public Vector3 PreviousAcceleration
	{
		get
		{
			return prevAcceleration;
		}
		set
		{
			prevAcceleration = value;
		}
	}

	public double Mass
	{
		get
		{
			if (inverseMass == 0.0)
			{
				return 3.4028234663852886E+38;
			}
			return 1.0 / inverseMass;
		}
		set
		{
			inverseMass = 1.0 / value;
		}
	}

	public double InverseMass
	{
		get
		{
			return inverseMass;
		}
		set
		{
			inverseMass = value;
		}
	}

	public void GetTransform(out Matrix transform)
	{
		transform = this.transform;
	}

	public void SetTransform(ref Matrix transform)
	{
		this.transform = transform;
	}

	public void SetPosition(ref Vector3 position)
	{
		Position = position;
	}

	public void SetPosition(float x, float y, float z)
	{
		Position.X = x;
		Position.Y = y;
		Position.Z = z;
	}

	public void GetPosition(out Vector3 position)
	{
		position = Position;
	}

	public void SetAcceleration(float x, float y, float z)
	{
		acceleration.X = x;
		acceleration.Y = y;
		acceleration.Z = z;
	}

	public void AddRotation(ref Vector3 deltaRotation)
	{
		Rotation += deltaRotation;
	}

	public void AddVelocity(ref Vector3 deltaVelocity)
	{
		Velocity += deltaVelocity;
	}

	public void CalculateDerivedData()
	{
		Orientation.Normalize();
		transform = Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);
	}

	public void CalculateTransform()
	{
		Orientation = Quaternion.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
		Orientation.Normalize();
		transform = Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);
	}

	public void CalculateTransform(out Matrix transform)
	{
		Orientation = Quaternion.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
		Orientation.Normalize();
		transform = Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);
	}

	public void Integrate(float duration)
	{
		prevAcceleration = Acceleration;
		prevAcceleration += Vector3.Multiply(forceAccum, (float)inverseMass);
		Velocity += Vector3.Multiply(prevAcceleration, duration);
		Velocity *= (float)Math.Pow(linearDamping, duration);
		Vector3 vector = Vector3.Multiply(Velocity, duration);
		Position += vector;
		ClearAccumulators();
	}

	public void AddForce(ref Vector3 force)
	{
		Vector3.Add(ref forceAccum, ref force, out forceAccum);
	}

	public void AddForce(Vector3 force)
	{
		Vector3.Add(ref forceAccum, ref force, out forceAccum);
	}

	public void ClearAccumulators()
	{
		forceAccum = Vector3.Zero;
	}
}
