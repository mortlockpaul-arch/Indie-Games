namespace JamSouls;

internal class AIGrabSoul : AIScript
{
	public AIGrabSoul(PlayerBot p)
		: base(p)
	{
	}

	public override AITarget ChooseTarget()
	{
		AITarget aITarget = null;
		if (m_PlayerBot.m_GameStateInstance.m_SoulSpawner != null)
		{
			Soul availableSoul = m_PlayerBot.m_GameStateInstance.m_SoulSpawner.GetAvailableSoul();
			if (availableSoul != null)
			{
				aITarget = new AITarget(availableSoul);
			}
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
		if (m_PlayerBot.m_CurrentTarget == null || (object)m_PlayerBot.m_CurrentTarget.TargetObject.GetType() != typeof(Soul))
		{
			Soul availableSoul = m_PlayerBot.m_GameStateInstance.m_SoulSpawner.GetAvailableSoul();
			if (availableSoul != null)
			{
				m_PlayerBot.m_CurrentTarget = null;
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 1, pressed: false);
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 3, pressed: false);
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 4, pressed: false);
				m_PlayerBot.m_Path.Clear();
				m_PlayerBot.m_bCloseToTargetMode = false;
			}
		}
		else if ((object)m_PlayerBot.m_CurrentTarget.TargetObject.GetType() == typeof(Soul))
		{
			Soul soul = (Soul)m_PlayerBot.m_CurrentTarget.TargetObject;
			if (soul.GetOwner() != null || !soul.m_bSpawned)
			{
				m_PlayerBot.m_CurrentTarget = null;
				m_PlayerBot.m_CurrentTarget = null;
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 1, pressed: false);
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 3, pressed: false);
				InputManager.SetKeyState(m_PlayerBot.m_PlayerNum, 4, pressed: false);
				m_PlayerBot.m_Path.Clear();
				m_PlayerBot.m_bCloseToTargetMode = false;
			}
		}
	}
}
