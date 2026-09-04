using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AITaskPlanAttack : AITask
{
	private Player m_Enemy;

	private AIAsyncJobPlanPath m_PathPlanningJob;

	public AITaskPlanAttack(AIBrain brain, Player enemy)
		: base(brain)
	{
		m_Enemy = enemy;
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		newTask = null;
		Ship theShip = m_Brain.Player.TheShip;
		if (m_PathPlanningJob == null)
		{
			if (terminate)
			{
				return true;
			}
			Vector2 from = new Vector2(theShip.Position.X, theShip.Position.Y);
			Vector2 to = new Vector2(m_Enemy.TheShip.Position.X, m_Enemy.TheShip.Position.Y);
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
			newTask = new AITaskFollowAttackPath(m_Brain, plannedPath, m_Enemy);
			m_PathPlanningJob = null;
			return true;
		}
		return false;
	}
}
