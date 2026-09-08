using System.Collections.Generic;

namespace JamSouls;

public class PathLink
{
	public WayPoint m_Origin;

	public WayPoint m_Destination;

	public float m_Duration;

	public List<InputEvent> m_EventList;

	public PathLink(WayPoint origin, WayPoint destination)
	{
		m_Origin = origin;
		m_Destination = destination;
		m_EventList = new List<InputEvent>();
	}
}
