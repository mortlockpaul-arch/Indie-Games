namespace ProjectMercury.Modifiers;

public class ScaleModifier : Modifier
{
	public float InitialScale;

	public float UltimateScale;

	public override Modifier DeepCopy()
	{
		ScaleModifier scaleModifier = new ScaleModifier();
		scaleModifier.InitialScale = InitialScale;
		scaleModifier.UltimateScale = UltimateScale;
		return scaleModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particle, int count)
	{
		for (int i = 0; i < count; i++)
		{
			particle->Scale = InitialScale + (UltimateScale - InitialScale) * particle->Age;
			particle++;
		}
	}
}
