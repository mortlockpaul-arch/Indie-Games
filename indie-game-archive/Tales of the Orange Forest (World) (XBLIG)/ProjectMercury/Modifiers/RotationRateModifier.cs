namespace ProjectMercury.Modifiers;

public sealed class RotationRateModifier : Modifier
{
	public float InitialRate;

	public float FinalRate;

	public override Modifier DeepCopy()
	{
		RotationRateModifier rotationRateModifier = new RotationRateModifier();
		rotationRateModifier.InitialRate = InitialRate;
		rotationRateModifier.FinalRate = FinalRate;
		return rotationRateModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particle, int count)
	{
		float num = InitialRate * dt;
		float num2 = FinalRate * dt;
		for (int i = 0; i < count; i++)
		{
			float radians = num + (num2 - num) * particle->Age;
			particle->Rotate(radians);
			particle++;
		}
	}
}
