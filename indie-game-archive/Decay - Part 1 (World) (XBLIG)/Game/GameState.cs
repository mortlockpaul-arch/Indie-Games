using System;

namespace Game;

[Serializable]
public struct GameState(string id, string state)
{
	public string m_id = id;

	public string m_state = state;
}
