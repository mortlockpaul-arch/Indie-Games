#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class RectangleForceModifier : Modifier
{
	public Vector2 Position;

	private float HalfWidth;

	private float HalfHeight;

	public Vector2 Force;

	public float Strength;

	public float Width
	{
		get
		{
			return HalfWidth + HalfWidth;
		}
		set
		{
			Guard.ArgumentNotFinite("Width", value);
			Guard.ArgumentLessThan("Width", value, 0f);
			HalfWidth = value * 0.5f;
		}
	}

	public float Height
	{
		get
		{
			return HalfHeight + HalfHeight;
		}
		set
		{
			Guard.ArgumentNotFinite("Height", value);
			Guard.ArgumentLessThan("Height", value, 0f);
			HalfHeight = value * 0.5f;
		}
	}

	public float Left => Position.X - HalfWidth;

	public float Right => Position.X + HalfWidth;

	public float Top => Position.Y - HalfHeight;

	public float Bottom => Position.Y + HalfHeight;

	public override Modifier DeepCopy()
	{
		RectangleForceModifier rectangleForceModifier = new RectangleForceModifier();
		rectangleForceModifier.Position = Position;
		rectangleForceModifier.Width = Width;
		rectangleForceModifier.Height = Height;
		rectangleForceModifier.Force = Force;
		rectangleForceModifier.Strength = Strength;
		return rectangleForceModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float num = Force.X * (Strength * dt);
		float num2 = Force.Y * (Strength * dt);
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			if (ptr->Position.X > Left && ptr->Position.X < Right && ptr->Position.Y > Top && ptr->Position.Y < Bottom)
			{
				ptr->Velocity.X += num;
				ptr->Velocity.Y += num2;
			}
		}
	}
}
