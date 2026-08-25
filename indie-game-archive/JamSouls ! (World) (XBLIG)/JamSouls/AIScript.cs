namespace JamSouls;

internal abstract class AIScript
{
	protected PlayerBot m_PlayerBot;

	public AIScript(PlayerBot p)
	{
		m_PlayerBot = p;
	}

	public virtual AITarget ChooseTarget()
	{
		return null;
	}

	public virtual void TakeDecision()
	{
	}
}
