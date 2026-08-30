#define DEBUG
using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class SineForceModifier : Modifier
{
	private float TotalSeconds;

	public float Frequency;

	public float Amplitude;

	private float AngleCos;

	private float AngleSin;

	public float Rotation
	{
		get
		{
			return Calculator.Atan2(AngleSin, AngleCos);
		}
		set
		{
			Guard.ArgumentNotFinite("Rotation", value);
			AngleCos = Calculator.Cos(value);
			AngleSin = Calculator.Sin(value);
		}
	}

	public override Modifier DeepCopy()
	{
		SineForceModifier sineForceModifier = new SineForceModifier();
		sineForceModifier.Amplitude = Amplitude;
		sineForceModifier.Frequency = Frequency;
		sineForceModifier.Rotation = Rotation;
		return sineForceModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		TotalSeconds += dt;
		float num = Amplitude * dt;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			float num2 = TotalSeconds - ptr->Inception;
			float x = Calculator.Cos(num2 * Frequency);
			Vector2 vector = new Vector2(x, 0f);
			vector.X = vector.X * AngleCos + vector.Y * (0f - AngleSin);
			vector.Y = vector.X * AngleSin + vector.Y * AngleCos;
			vector.X *= num;
			vector.Y *= num;
			ptr->Velocity.X += vector.X;
			ptr->Velocity.Y += vector.Y;
		}
	}
}
