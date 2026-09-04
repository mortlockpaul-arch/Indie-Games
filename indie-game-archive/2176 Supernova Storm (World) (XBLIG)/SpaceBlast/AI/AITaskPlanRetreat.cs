using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AITaskPlanRetreat : AITask
{
	private Player m_Enemy;

	private AIAsyncJobPlanPath m_PathPlanningJob;

	public AITaskPlanRetreat(AIBrain brain, Player enemy)
		: base(brain)
	{
		m_Enemy = enemy;
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		newTask = null;
		Ship theShip = m_Brain.Player.TheShip;
		if (m_PathPlanningJob == null)
		{
			if (terminate)
			{
				return true;
			}
			Vector2 from = new Vector2(theShip.Position.X, theShip.Position.Y);
			Vector2 to = MainGame.LevelData.Waypoints.SelectRandomWaypoint().Position;
			m_PathPlanningJob = new AIAsyncJobPlanPath(ref from, ref to, (int)theShip.Diameter);
			MainGame.JobMan.AddJobToStack(m_PathPlanningJob);
		}
		if (m_PathPlanningJob.IsComplete)
		{
			if (terminate)
			{
				return true;
			}
			PlannedPath plannedPath = m_PathPlanningJob.PlannedPath;
			newTask = new AITaskFollowRetreatPath(m_Brain, plannedPath, m_Enemy);
			m_PathPlanningJob = null;
			return true;
		}
		return false;
	}
}
