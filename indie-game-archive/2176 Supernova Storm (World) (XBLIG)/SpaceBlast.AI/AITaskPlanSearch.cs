using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AITaskPlanSearch(AIBrain brain) : AITask(brain)
{
	private const float constDestinationProximityDistance = 5000f;

	private const int constMaxReplansPerDest = 20;

	private AIAsyncJobPlanPath m_PathPlanningJob;

	private Vector2 m_Destination = Vector2.Zero;

	private int m_RouteReplans;

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		newTask = null;
		if (m_PathPlanningJob != null)
		{
			if (m_PathPlanningJob.IsComplete)
			{
				if (terminate)
				{
					return true;
				}
				PlannedPath plannedPath = m_PathPlanningJob.PlannedPath;
				if (plannedPath.Route.Count > 0)
				{
					newTask = new AITaskFollowSearchPath(m_Brain, plannedPath);
					m_PathPlanningJob = null;
				}
			}
		}
		else
		{
			if (terminate)
			{
				return true;
			}
			Ship theShip = m_Brain.Player.TheShip;
			Vector2 from = new Vector2(theShip.Position.X, theShip.Position.Y);
			bool flag = true;
			if (m_Destination != Vector2.Zero && (from - m_Destination).Length() > 5000f && m_RouteReplans++ < 20)
			{
				flag = false;
			}
			if (flag)
			{
				m_Destination = MainGame.LevelData.Waypoints.SelectRandomWaypoint().Position;
				m_RouteReplans = 0;
			}
			m_PathPlanningJob = new AIAsyncJobPlanPath(ref from, ref m_Destination, (int)theShip.Diameter);
			MainGame.JobMan.AddJobToStack(m_PathPlanningJob);
		}
		return false;
	}
}
