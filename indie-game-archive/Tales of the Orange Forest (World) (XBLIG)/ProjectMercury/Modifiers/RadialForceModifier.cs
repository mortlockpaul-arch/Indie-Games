#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public class RadialForceModifier : Modifier
{
	public Vector2 Position;

	private float _radius;

	private float SquareRadius;

	public Vector2 Force;

	public float Strength;

	public float Radius
	{
		get
		{
			return _radius;
		}
		set
		{
			Guard.ArgumentNotFinite("Radius", value);
			Guard.ArgumentLessThan("Radius", value, float.Epsilon);
			_radius = value;
			SquareRadius = value * value;
		}
	}

	public override Modifier DeepCopy()
	{
		RadialForceModifier radialForceModifier = new RadialForceModifier();
		radialForceModifier.Position = Position;
		radialForceModifier.Radius = Radius;
		radialForceModifier.Force = Force;
		radialForceModifier.Strength = Strength;
		return radialForceModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float num = Strength * dt;
		float num2 = Force.X * num;
		float num3 = Force.Y * num;
		Vector2 vector = default(Vector2);
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			vector.X = Position.X - ptr->Position.X;
			vector.Y = Position.Y - ptr->Position.Y;
			float num4 = vector.X * vector.X + vector.Y * vector.Y;
			if (num4 < SquareRadius)
			{
				ptr->Velocity.X += num2;
				ptr->Velocity.Y += num3;
			}
		}
	}
}
