namespace JamSouls;

public class InputEvent
{
	public float m_Time;

	public bool[] m_InputState;

	public InputEvent()
	{
		m_Time = -1f;
		m_InputState = new bool[14];
	}
}
