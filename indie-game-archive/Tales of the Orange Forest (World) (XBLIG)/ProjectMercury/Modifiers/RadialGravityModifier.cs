#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class RadialGravityModifier : Modifier
{
	public Vector2 Position;

	private float _radius;

	private float SquareRadius;

	private float _innerRadius;

	private float SquareInnerRadius;

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

	public float InnerRadius
	{
		get
		{
			return _innerRadius;
		}
		set
		{
			Guard.ArgumentNotFinite("InnerRadius", value);
			Guard.ArgumentLessThan("InnerRadius", value, 0f);
			Guard.ArgumentGreaterThan("InnerRadius", value, Radius);
			_innerRadius = value;
			SquareInnerRadius = value * value;
		}
	}

	public override Modifier DeepCopy()
	{
		RadialGravityModifier radialGravityModifier = new RadialGravityModifier();
		radialGravityModifier.InnerRadius = InnerRadius;
		radialGravityModifier.Position = Position;
		radialGravityModifier.Radius = Radius;
		radialGravityModifier.Strength = Strength;
		return radialGravityModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float num = Strength * dt;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			Vector2 vector = new Vector2
			{
				X = Position.X - ptr->Position.X,
				Y = Position.Y - ptr->Position.Y
			};
			float num2 = vector.X * vector.X + vector.Y * vector.Y;
			if (num2 < SquareRadius && num2 > SquareInnerRadius)
			{
				float num3 = Calculator.Sqrt(num2);
				Vector2 vector2 = new Vector2
				{
					X = vector.X / num3,
					Y = vector.Y / num3
				};
				float num4 = SquareRadius / num2;
				vector2.X *= num4;
				vector2.Y *= num4;
				vector2.X *= num;
				vector2.Y *= num;
				ptr->Velocity.X += vector2.X;
				ptr->Velocity.Y += vector2.Y;
			}
		}
	}
}
