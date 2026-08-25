namespace SpaceBlast.AI;

internal abstract class AITask
{
	protected AIBrain m_Brain;

	protected AITask(AIBrain brain)
	{
		m_Brain = brain;
	}

	public abstract bool UpdateTask(out AITask newTask, bool terminate);
}
