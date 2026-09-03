namespace Core;

public class Tutorial
{
	public enum STATE
	{
		MOVE_CURSOR,
		DECREASE_SPEED,
		INCREASE_SPEED,
		CHANGE_VIEW,
		CHANGING_VIEW,
		USE,
		WAIT_FOR_PICKUP,
		INVENTORY,
		NONE
	}

	protected STATE m_state;

	protected Game m_game;

	public Tutorial(Game game)
	{
		m_game = game;
		if (m_game != null && m_game.m_game_data != null && m_game.m_game_data.GetState("TutorialState") != "")
		{
			m_state = (STATE)int.Parse(m_game.m_game_data.GetState("TutorialState"));
			if (m_state == STATE.CHANGING_VIEW)
			{
				m_state = STATE.CHANGE_VIEW;
			}
		}
	}

	public virtual void Clear()
	{
		m_game = null;
		m_state = STATE.NONE;
	}
}
