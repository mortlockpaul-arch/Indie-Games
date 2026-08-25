namespace SpaceBlast.AI;

internal class AITaskPlanCombat : AITask
{
	private Player m_Enemy;

	public AITaskPlanCombat(AIBrain brain, Player enemy)
		: base(brain)
	{
		m_Enemy = enemy;
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		newTask = null;
		if (terminate)
		{
			return true;
		}
		switch (m_Brain.AttackOrRetreat(m_Enemy))
		{
		case EAttackOrRetreat.Attack:
			newTask = new AITaskPlanAttack(m_Brain, m_Enemy);
			break;
		case EAttackOrRetreat.Retreat:
			newTask = new AITaskPlanRetreat(m_Brain, m_Enemy);
			break;
		case EAttackOrRetreat.NotApplicable:
			return true;
		}
		return false;
	}
}
