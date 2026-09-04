using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AITaskPlanSearch : AITask
{
	private const float constDestinationProximityDistance = 5000f;

	private const int constMaxReplansPerDest = 20;

	private AIAsyncJobPlanPath m_PathPlanningJob;

	private Vector2 m_Destination;

	private int m_RouteReplans;

	public AITaskPlanSearch(AIBrain brain)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_Destination = Vector2.Zero;
		base._002Ector(brain);
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
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
			if (m_Destination != Vector2.Zero)
			{
				Vector2 val = from - m_Destination;
				if (((Vector2)(ref val)).Length() > 5000f && m_RouteReplans++ < 20)
				{
					flag = false;
				}
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
