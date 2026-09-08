namespace System.Net.ServerSentEvents;

public readonly struct SseItem<T>
{
	internal readonly string _eventType;

	private readonly string _eventId = null;

	private readonly TimeSpan? _reconnectionInterval = null;

	public T Data { get; }

	public string EventType => _eventType ?? "message";

	public string? EventId
	{
		get
		{
			return _eventId;
		}
		init
		{
			if (value?.AsSpan().ContainsLineBreaks() ?? false)
			{
				ThrowHelper.ThrowArgumentException_CannotContainLineBreaks("EventId");
			}
			_eventId = value;
		}
	}

	public TimeSpan? ReconnectionInterval
	{
		get
		{
			return _reconnectionInterval;
		}
		init
		{
			if (value < TimeSpan.Zero)
			{
				ThrowHelper.ThrowArgumentException_CannotBeNegative("ReconnectionInterval");
			}
			_reconnectionInterval = value;
		}
	}

	public SseItem(T data, string? eventType = null)
	{
		if (eventType?.AsSpan().ContainsLineBreaks() ?? false)
		{
			ThrowHelper.ThrowArgumentException_CannotContainLineBreaks("eventType");
		}
		Data = data;
		_eventType = eventType;
	}
}
