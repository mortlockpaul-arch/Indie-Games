namespace ProjectMercury.Modifiers;

public abstract class Modifier
{
	public abstract Modifier DeepCopy();

	protected internal unsafe abstract void Process(float elapsedSeconds, Particle* particle, int count);
}
