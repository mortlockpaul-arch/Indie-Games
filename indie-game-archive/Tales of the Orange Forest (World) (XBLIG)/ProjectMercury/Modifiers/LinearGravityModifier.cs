using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class LinearGravityModifier : Modifier
{
	public Vector2 Gravity;

	public override Modifier DeepCopy()
	{
		LinearGravityModifier linearGravityModifier = new LinearGravityModifier();
		linearGravityModifier.Gravity = Gravity;
		return linearGravityModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float num = Gravity.X * dt;
		float num2 = Gravity.Y * dt;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			ptr->Velocity.X += num;
			ptr->Velocity.Y += num2;
		}
	}
}
