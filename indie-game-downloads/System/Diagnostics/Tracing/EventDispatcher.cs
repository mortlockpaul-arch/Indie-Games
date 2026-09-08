using System.Collections.Generic;

namespace System.Diagnostics.Tracing;

internal sealed class EventDispatcher
{
	internal readonly EventListener m_Listener;

	internal Dictionary<int, bool> m_EventEnabled;

	internal EventDispatcher m_Next;

	internal EventDispatcher(EventDispatcher next, Dictionary<int, bool> eventEnabled, EventListener listener)
	{
		m_Next = next;
		m_EventEnabled = eventEnabled;
		m_Listener = listener;
	}
}
