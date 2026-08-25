using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AITaskFollowPowerupPickupPath : AITaskPathFollower
{
	private PowerUp m_TargetPowerUp;

	private bool m_AbortForCombat;

	public AITaskFollowPowerupPickupPath(AIBrain brain, PlannedPath path, PowerUp powerup, bool abortForCombat)
		: base(brain, path)
	{
		m_TargetPowerUp = powerup;
		m_AbortForCombat = abortForCombat;
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		newTask = null;
		if (terminate)
		{
			return true;
		}
		if (!m_TargetPowerUp.IsActive)
		{
			return true;
		}
		if (m_AbortForCombat && m_Brain.TimeSlice == 1)
		{
			Player enemy = null;
			float num = MainGame.Players.FindNearestVisibleEnemy(m_Brain.Player, out enemy);
			if (num < (float)m_Brain.Personality.VisualRange)
			{
				float num2 = (m_Brain.Player.TheShip.Position - m_TargetPowerUp.Position).Length();
				if (num2 > 7500f)
				{
					return true;
				}
			}
		}
		return FollowPath(100f);
	}
}
