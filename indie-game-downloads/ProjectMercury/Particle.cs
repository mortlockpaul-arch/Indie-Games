using System;
using Microsoft.Xna.Framework;

namespace ProjectMercury;

public struct Particle
{
	public Vector2 Position;

	public float Scale;

	public float Rotation;

	public Vector4 Colour;

	public Vector2 Momentum;

	public Vector2 Velocity;

	public float Inception;

	public float Age;

	public void ApplyForce(ref Vector2 force)
	{
		Velocity.X += force.X;
		Velocity.Y += force.Y;
	}

	public void Rotate(float radians)
	{
		Rotation += radians;
		if (Rotation > 3.141593f)
		{
			Rotation -= 6.283185f;
		}
		else if (Rotation < -3.141593f)
		{
			Rotation += 6.283185f;
		}
	}

	[Obsolete("No longer used!")]
	internal void Update(float deltaSeconds)
	{
		Momentum.X += Velocity.X;
		Momentum.Y += Velocity.Y;
		Velocity.X = (Velocity.Y = 0f);
		Vector2 vector = default(Vector2);
		vector.X = Momentum.X * deltaSeconds;
		vector.Y = Momentum.Y * deltaSeconds;
		Position.X += vector.X;
		Position.Y += vector.Y;
	}
}
