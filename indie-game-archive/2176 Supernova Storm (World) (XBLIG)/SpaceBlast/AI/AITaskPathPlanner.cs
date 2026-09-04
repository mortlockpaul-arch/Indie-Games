using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal abstract class AITaskPathPlanner : AITask
{
	protected AIAsyncJobPlanPath m_PathPlanningJob;

	public bool IsPathReady => m_PathPlanningJob.IsComplete;

	public PlannedPath Route => m_PathPlanningJob.PlannedPath;

	public AITaskPathPlanner(AIBrain brain)
		: base(brain)
	{
	}

	public void PlanPath(ref Vector2 from, ref Vector2 to, int shipWidth)
	{
		m_PathPlanningJob = new AIAsyncJobPlanPath(ref from, ref to, shipWidth);
		MainGame.JobMan.AddJobToStack(m_PathPlanningJob);
	}
}
