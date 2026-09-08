namespace ProjectMercury.Modifiers;

public sealed class RotationModifier : Modifier
{
	public float RotationRate;

	public override Modifier DeepCopy()
	{
		RotationModifier rotationModifier = new RotationModifier();
		rotationModifier.RotationRate = RotationRate;
		return rotationModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float radians = RotationRate * dt;
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			ptr->Rotate(radians);
		}
	}
}
