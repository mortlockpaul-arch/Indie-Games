using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Diagnostics.Tracing;

public abstract class EventListener : IDisposable
{
	[CompilerGenerated]
	private EventHandler<EventSourceCreatedEventArgs> _EventSourceCreated;

	internal volatile EventListener m_Next;

	internal static EventListener s_Listeners;

	internal static List<WeakReference<EventSource>> s_EventSources;

	private static bool s_CreatingListener;

	internal static object EventListenersLock
	{
		get
		{
			if (s_EventSources == null)
			{
				Interlocked.CompareExchange(ref s_EventSources, new List<WeakReference<EventSource>>(2), null);
			}
			return s_EventSources;
		}
	}

	public event EventHandler<EventSourceCreatedEventArgs>? EventSourceCreated
	{
		add
		{
			CallBackForExistingEventSources(addToListenersList: false, value);
			_EventSourceCreated = (EventHandler<EventSourceCreatedEventArgs>)Delegate.Combine(_EventSourceCreated, value);
		}
		remove
		{
			_EventSourceCreated = (EventHandler<EventSourceCreatedEventArgs>)Delegate.Remove(_EventSourceCreated, value);
		}
	}

	public event EventHandler<EventWrittenEventArgs>? EventWritten;

	protected EventListener()
	{
		CallBackForExistingEventSources(addToListenersList: true, delegate(object obj, EventSourceCreatedEventArgs args)
		{
			args.EventSource.AddListener((EventListener)obj);
		});
	}

	public virtual void Dispose()
	{
		lock (EventListenersLock)
		{
			if (s_Listeners != null)
			{
				if (this == s_Listeners)
				{
					EventListener listenerToRemove = s_Listeners;
					s_Listeners = m_Next;
					RemoveReferencesToListenerInEventSources(listenerToRemove);
				}
				else
				{
					EventListener eventListener = s_Listeners;
					while (true)
					{
						EventListener next = eventListener.m_Next;
						if (next == null)
						{
							break;
						}
						if (next == this)
						{
							eventListener.m_Next = next.m_Next;
							RemoveReferencesToListenerInEventSources(next);
							break;
						}
						eventListener = next;
					}
				}
			}
		}
		EventPipeEventDispatcher.Instance.SendCommand(this, EventCommand.Update, enable: false, EventLevel.LogAlways, EventKeywords.None);
	}

	public void EnableEvents(EventSource eventSource, EventLevel level)
	{
		EnableEvents(eventSource, level, EventKeywords.None);
	}

	public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword)
	{
		EnableEvents(eventSource, level, matchAnyKeyword, null);
	}

	public void EnableEvents(EventSource eventSource, EventLevel level, EventKeywords matchAnyKeyword, IDictionary<string, string?>? arguments)
	{
		ArgumentNullException.ThrowIfNull(eventSource, "eventSource");
		eventSource.SendCommand(this, EventProviderType.None, 0, EventCommand.Update, enable: true, level, matchAnyKeyword, arguments);
		if (eventSource.GetType() == typeof(NativeRuntimeEventSource))
		{
			EventPipeEventDispatcher.Instance.SendCommand(this, EventCommand.Update, enable: true, level, matchAnyKeyword);
		}
	}

	public void DisableEvents(EventSource eventSource)
	{
		ArgumentNullException.ThrowIfNull(eventSource, "eventSource");
		eventSource.SendCommand(this, EventProviderType.None, 0, EventCommand.Update, enable: false, EventLevel.LogAlways, EventKeywords.None, null);
		if (eventSource.GetType() == typeof(NativeRuntimeEventSource))
		{
			EventPipeEventDispatcher.Instance.SendCommand(this, EventCommand.Update, enable: false, EventLevel.LogAlways, EventKeywords.None);
		}
	}

	protected internal static int EventSourceIndex(EventSource eventSource)
	{
		return eventSource.m_id;
	}

	protected internal virtual void OnEventSourceCreated(EventSource eventSource)
	{
		EventHandler<EventSourceCreatedEventArgs> eventHandler = _EventSourceCreated;
		if (eventHandler != null)
		{
			EventSourceCreatedEventArgs e = new EventSourceCreatedEventArgs();
			e.EventSource = eventSource;
			eventHandler(this, e);
		}
	}

	protected internal virtual void OnEventWritten(EventWrittenEventArgs eventData)
	{
		EventWritten?.Invoke(this, eventData);
	}

	internal static void AddEventSource(EventSource newEventSource)
	{
		lock (EventListenersLock)
		{
			int num = -1;
			if (s_EventSources.Count % 64 == 63)
			{
				int num2 = s_EventSources.Count;
				while (0 < num2)
				{
					num2--;
					WeakReference<EventSource> weakReference = s_EventSources[num2];
					if (!weakReference.TryGetTarget(out var _))
					{
						num = num2;
						weakReference.SetTarget(newEventSource);
						break;
					}
				}
			}
			if (num < 0)
			{
				num = s_EventSources.Count;
				s_EventSources.Add(new WeakReference<EventSource>(newEventSource));
			}
			newEventSource.m_id = num;
			for (EventListener next = s_Listeners; next != null; next = next.m_Next)
			{
				newEventSource.AddListener(next);
			}
		}
	}

	internal static void DisposeOnShutdown()
	{
		List<EventSource> list = new List<EventSource>();
		lock (EventListenersLock)
		{
			foreach (WeakReference<EventSource> s_EventSource in s_EventSources)
			{
				if (s_EventSource.TryGetTarget(out var target))
				{
					list.Add(target);
				}
			}
		}
		foreach (EventSource item in list)
		{
			item.Dispose();
		}
	}

	private static void CallDisableEventsIfNecessary(EventDispatcher eventDispatcher, EventSource eventSource)
	{
		if (eventDispatcher.m_EventEnabled == null)
		{
			return;
		}
		foreach (bool value in eventDispatcher.m_EventEnabled.Values)
		{
			if (value)
			{
				eventDispatcher.m_Listener.DisableEvents(eventSource);
			}
		}
	}

	private static void RemoveReferencesToListenerInEventSources(EventListener listenerToRemove)
	{
		WeakReference<EventSource>[] array = s_EventSources.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].TryGetTarget(out var target))
			{
				continue;
			}
			for (EventDispatcher eventDispatcher = target.m_Dispatchers; eventDispatcher != null; eventDispatcher = eventDispatcher.m_Next)
			{
				if (eventDispatcher.m_Listener == listenerToRemove)
				{
					CallDisableEventsIfNecessary(eventDispatcher, target);
				}
			}
		}
		foreach (WeakReference<EventSource> s_EventSource in s_EventSources)
		{
			if (!s_EventSource.TryGetTarget(out var target2) || target2.m_Dispatchers == null)
			{
				continue;
			}
			if (target2.m_Dispatchers.m_Listener == listenerToRemove)
			{
				target2.m_Dispatchers = target2.m_Dispatchers.m_Next;
				continue;
			}
			EventDispatcher eventDispatcher2 = target2.m_Dispatchers;
			while (true)
			{
				EventDispatcher next = eventDispatcher2.m_Next;
				if (next == null)
				{
					break;
				}
				if (next.m_Listener == listenerToRemove)
				{
					eventDispatcher2.m_Next = next.m_Next;
					break;
				}
				eventDispatcher2 = next;
			}
		}
	}

	private void CallBackForExistingEventSources(bool addToListenersList, EventHandler<EventSourceCreatedEventArgs> callback)
	{
		EventSourceInitHelper.EnsurePreregisteredEventSourcesExist();
		lock (EventListenersLock)
		{
			if (s_CreatingListener)
			{
				throw new InvalidOperationException(SR.EventSource_ListenerCreatedInsideCallback);
			}
			try
			{
				s_CreatingListener = true;
				if (addToListenersList)
				{
					m_Next = s_Listeners;
					s_Listeners = this;
				}
				if (callback == null)
				{
					return;
				}
				WeakReference<EventSource>[] array = s_EventSources.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].TryGetTarget(out var target))
					{
						EventSourceCreatedEventArgs e = new EventSourceCreatedEventArgs();
						e.EventSource = target;
						callback(this, e);
					}
				}
			}
			finally
			{
				s_CreatingListener = false;
			}
		}
	}
}
