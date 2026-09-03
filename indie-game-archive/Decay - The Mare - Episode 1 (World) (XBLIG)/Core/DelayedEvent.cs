namespace Core;

public class DelayedEvent
{
	public string m_event = "";

	public float m_delay;

	public DelayedEvent(string s_event, float delay)
	{
		m_event = s_event;
		m_delay = delay;
	}
}
