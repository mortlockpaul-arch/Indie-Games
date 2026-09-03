using Core;

namespace TheMare1;

public class Tutorial : Core.Tutorial
{
	public Tutorial(Game game)
		: base(game)
	{
		if (m_state == STATE.CHANGING_VIEW)
		{
			m_state = STATE.CHANGE_VIEW;
		}
	}

	public override void Clear()
	{
		base.Clear();
	}
}
