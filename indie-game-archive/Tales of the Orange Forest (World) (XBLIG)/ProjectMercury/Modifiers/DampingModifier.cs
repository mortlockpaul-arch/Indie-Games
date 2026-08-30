namespace ProjectMercury.Modifiers;

public sealed class DampingModifier : Modifier
{
	public float DampingCoefficient;

	public override Modifier DeepCopy()
	{
		DampingModifier dampingModifier = new DampingModifier();
		dampingModifier.DampingCoefficient = DampingCoefficient;
		return dampingModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float num = DampingCoefficient * dt * -1f;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			ptr->Velocity.X += ptr->Momentum.X * num;
			ptr->Velocity.Y += ptr->Momentum.Y * num;
		}
	}
}
