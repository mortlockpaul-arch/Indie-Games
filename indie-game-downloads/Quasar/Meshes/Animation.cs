namespace Quasar.Meshes;

public class Animation
{
	public const int STOP = -1;

	public int firstStep;

	public int nextId;

	public AnimationStep[] steps;

	public Animation(int stepNumber)
	{
		steps = new AnimationStep[stepNumber];
	}
}
