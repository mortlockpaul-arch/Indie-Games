namespace JamSouls;

internal class AIGrabPowerUp : AIScript
{
	public AIGrabPowerUp(PlayerBot p)
		: base(p)
	{
	}

	public override AITarget ChooseTarget()
	{
		AITarget aITarget = null;
		if (m_PlayerBot.m_CurrentPowerUp == null && m_PlayerBot.m_GameStateInstance.m_CurrentBonus != null)
		{
			aITarget = new AITarget(m_PlayerBot.m_GameStateInstance.m_CurrentBonus);
		}
		if (GameContext.GameMode == GAME_MODE.STORYMATCH && aITarget == null)
		{
			foreach (Player player in m_PlayerBot.m_GameStateInstance.m_Players)
			{
				if (player.m_Tag == 0 && player.GetTeam() != m_PlayerBot.m_Team)
				{
					aITarget = new AITarget(player);
					break;
				}
			}
		}
		return aITarget;
	}

	public override void TakeDecision()
	{
		if (m_PlayerBot.m_CurrentPowerUp != null)
		{
			return;
		}
		if (m_PlayerBot.m_CurrentTarget == null || (object)m_PlayerBot.m_CurrentTarget.TargetObject.GetType().BaseType != typeof(PowerUp))
		{
			PowerUp currentBonus = m_PlayerBot.m_GameStateInstance.m_CurrentBonus;
			if (currentBonus != null && currentBonus.IsAvailable())
			{
				m_PlayerBot.m_CurrentTarget = null;
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 1, pressed: false);
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 3, pressed: false);
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 4, pressed: false);
				m_PlayerBot.m_Path.Clear();
				m_PlayerBot.m_bCloseToTargetMode = false;
			}
		}
		else if ((object)m_PlayerBot.m_CurrentTarget.TargetObject.GetType().BaseType == typeof(PowerUp))
		{
			PowerUp currentBonus2 = m_PlayerBot.m_GameStateInstance.m_CurrentBonus;
			if (currentBonus2 == null || !currentBonus2.IsAvailable())
			{
				m_PlayerBot.m_CurrentTarget = null;
			}
		}
	}
}
