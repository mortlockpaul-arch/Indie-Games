namespace ProjectMercury.Modifiers;

public sealed class TrajectoryRotationModifier : Modifier
{
	public float RotationOffset;

	public override Modifier DeepCopy()
	{
		TrajectoryRotationModifier trajectoryRotationModifier = new TrajectoryRotationModifier();
		trajectoryRotationModifier.RotationOffset = RotationOffset;
		return trajectoryRotationModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particle, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particle - 1;
			if (particle->Momentum == ptr->Momentum)
			{
				particle->Rotation = ptr->Rotation;
				continue;
			}
			float num = Calculator.Atan2(particle->Momentum.Y, particle->Momentum.X);
			particle->Rotation = num + RotationOffset;
			if (particle->Rotation > 3.141593f)
			{
				particle->Rotation -= 6.283185f;
			}
			else if (particle->Rotation < -3.141593f)
			{
				particle->Rotation += 6.283185f;
			}
			particle++;
		}
	}
}
