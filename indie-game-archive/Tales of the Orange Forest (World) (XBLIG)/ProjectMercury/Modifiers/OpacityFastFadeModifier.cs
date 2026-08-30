namespace ProjectMercury.Modifiers;

public sealed class OpacityFastFadeModifier : Modifier
{
	public override Modifier DeepCopy()
	{
		return new OpacityFastFadeModifier();
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			ptr->Colour.W = 1f - ptr->Age;
		}
	}
}
