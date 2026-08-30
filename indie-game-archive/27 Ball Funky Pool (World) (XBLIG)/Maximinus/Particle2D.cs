using System;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class Particle2D
{
	public Color color = Color.White;

	public Vector2 Position;

	public Vector2 Velocity;

	public Vector2 Acceleration;

	private float lifetime;

	private float timeSinceStart;

	private float scale;

	private float rotation;

	private float rotationSpeed;

	public bool DataTag1;

	public float Lifetime
	{
		get
		{
			return lifetime;
		}
		set
		{
			lifetime = value;
		}
	}

	public float TimeSinceStart
	{
		get
		{
			return timeSinceStart;
		}
		set
		{
			timeSinceStart = value;
		}
	}

	public float Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
		}
	}

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
		}
	}

	public float RotationSpeed
	{
		get
		{
			return rotationSpeed;
		}
		set
		{
			rotationSpeed = value;
		}
	}

	public bool Active => TimeSinceStart < Lifetime;

	public void Initialize(Vector2 position, Vector2 velocity, Vector2 acceleration, float lifetime, float scale, float rotationSpeed)
	{
		Position = position;
		Velocity = velocity;
		Acceleration = acceleration;
		Lifetime = lifetime;
		Scale = scale;
		RotationSpeed = rotationSpeed;
		Rotation = Utils.RandomBetween(0f, (float)Math.PI * 2f);
		TimeSinceStart = 0f;
	}

	public void Update(float dt, float gravity)
	{
		Velocity += Acceleration * dt;
		Velocity += Vector2.UnitY * gravity * dt;
		Position += Velocity * dt;
		Rotation += RotationSpeed * dt;
		TimeSinceStart += dt;
	}
}
