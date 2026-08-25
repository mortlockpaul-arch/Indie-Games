using Microsoft.Xna.Framework;

namespace SpaceBlast.AI;

internal class AITaskPlanPowerupPickup : AITaskPathPlanner
{
	private PowerUp m_TargetPowerUp;

	private bool m_AbortForCombat;

	public AITaskPlanPowerupPickup(AIBrain brain, PowerUp powerup, bool abortForCombat)
		: base(brain)
	{
		m_TargetPowerUp = powerup;
		m_AbortForCombat = abortForCombat;
		Ship theShip = brain.Player.TheShip;
		Vector2 from = new Vector2
		{
			X = theShip.Position.X,
			Y = theShip.Position.Y
		};
		Vector2 to = new Vector2
		{
			X = powerup.Position.X,
			Y = powerup.Position.Y
		};
		int shipWidth = (int)theShip.Diameter;
		PlanPath(ref from, ref to, shipWidth);
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		newTask = null;
		if (base.IsPathReady)
		{
			if (terminate)
			{
				return true;
			}
			newTask = new AITaskFollowPowerupPickupPath(m_Brain, base.Route, m_TargetPowerUp, m_AbortForCombat);
			return true;
		}
		return false;
	}
}
