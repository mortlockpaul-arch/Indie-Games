using System.Collections.Generic;
using System.ComponentModel;

namespace System.Diagnostics.Metrics;

public abstract class Instrument
{
	internal readonly DiagLinkedList<ListenerSubscription> _subscriptions = new DiagLinkedList<ListenerSubscription>();

	internal static KeyValuePair<string, object?>[] EmptyTags => Array.Empty<KeyValuePair<string, object>>();

	internal static object SyncObject { get; } = new object();

	public Meter Meter { get; }

	public string Name { get; }

	public string? Description { get; }

	public string? Unit { get; }

	public IEnumerable<KeyValuePair<string, object?>>? Tags { get; }

	public bool Enabled => _subscriptions.First != null;

	public virtual bool IsObservable => false;

	protected Instrument(Meter meter, string name)
		: this(meter, name, null, null, null)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected Instrument(Meter meter, string name, string? unit, string? description)
		: this(meter, name, unit, description, null)
	{
	}

	protected Instrument(Meter meter, string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null)
	{
		Meter = meter ?? throw new ArgumentNullException("meter");
		Name = name ?? throw new ArgumentNullException("name");
		Description = description;
		Unit = unit;
		if (tags != null)
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>(tags);
			list.Sort((KeyValuePair<string, object> left, KeyValuePair<string, object> right) => string.Compare(left.Key, right.Key, StringComparison.Ordinal));
			Tags = list;
		}
	}

	protected void Publish()
	{
		if (!System.Diagnostics.Metrics.Meter.IsSupported)
		{
			return;
		}
		List<MeterListener> list = null;
		lock (SyncObject)
		{
			if (Meter.Disposed || !Meter.AddInstrument(this))
			{
				return;
			}
			list = MeterListener.GetAllListeners();
		}
		if (list == null)
		{
			return;
		}
		foreach (MeterListener item in list)
		{
			item.InstrumentPublished?.Invoke(this, item);
		}
	}

	internal void NotifyForUnpublishedInstrument()
	{
		for (DiagNode<ListenerSubscription> diagNode = _subscriptions.First; diagNode != null; diagNode = diagNode.Next)
		{
			diagNode.Value.Listener.DisableMeasurementEvents(this);
		}
		_subscriptions.Clear();
	}

	internal static void ValidateTypeParameter<T>()
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle != typeof(byte) && typeFromHandle != typeof(short) && typeFromHandle != typeof(int) && typeFromHandle != typeof(long) && typeFromHandle != typeof(double) && typeFromHandle != typeof(float) && typeFromHandle != typeof(decimal))
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.UnsupportedType, typeFromHandle));
		}
	}

	internal object EnableMeasurement(ListenerSubscription subscription, out bool oldStateStored)
	{
		oldStateStored = false;
		if (!_subscriptions.AddIfNotExist(subscription, (ListenerSubscription s1, ListenerSubscription s2) => s1.Listener == s2.Listener))
		{
			ListenerSubscription listenerSubscription = _subscriptions.Remove(subscription, (ListenerSubscription s1, ListenerSubscription s2) => s1.Listener == s2.Listener);
			_subscriptions.AddIfNotExist(subscription, (ListenerSubscription s1, ListenerSubscription s2) => s1.Listener == s2.Listener);
			oldStateStored = listenerSubscription.Listener == subscription.Listener;
			return listenerSubscription.State;
		}
		return false;
	}

	internal object DisableMeasurements(MeterListener listener)
	{
		return _subscriptions.Remove(new ListenerSubscription(listener), (ListenerSubscription s1, ListenerSubscription s2) => s1.Listener == s2.Listener).State;
	}

	internal virtual void Observe(MeterListener listener)
	{
		throw new InvalidOperationException();
	}

	internal object GetSubscriptionState(MeterListener listener)
	{
		for (DiagNode<ListenerSubscription> diagNode = _subscriptions.First; diagNode != null; diagNode = diagNode.Next)
		{
			if (listener == diagNode.Value.Listener)
			{
				return diagNode.Value.State;
			}
		}
		return null;
	}
}
[DebuggerDisplay("Name = {Name}, Meter = {Meter.Name}")]
public abstract class Instrument<T> : Instrument where T : struct
{
	public InstrumentAdvice<T>? Advice { get; }

	protected Instrument(Meter meter, string name)
		: this(meter, name, (string?)null, (string?)null, (IEnumerable<KeyValuePair<string, object?>>?)null, (InstrumentAdvice<T>?)null)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected Instrument(Meter meter, string name, string? unit, string? description)
		: this(meter, name, unit, description, (IEnumerable<KeyValuePair<string, object?>>?)null, (InstrumentAdvice<T>?)null)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected Instrument(Meter meter, string name, string? unit, string? description, IEnumerable<KeyValuePair<string, object?>>? tags)
		: this(meter, name, unit, description, tags, (InstrumentAdvice<T>?)null)
	{
	}

	protected Instrument(Meter meter, string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, InstrumentAdvice<T>? advice = null)
		: base(meter, name, unit, description, tags)
	{
		Advice = advice;
		Instrument.ValidateTypeParameter<T>();
	}

	protected void RecordMeasurement(T measurement)
	{
		RecordMeasurement(measurement, Instrument.EmptyTags.AsSpan());
	}

	protected void RecordMeasurement(T measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags)
	{
		for (DiagNode<ListenerSubscription> diagNode = _subscriptions.First; diagNode != null; diagNode = diagNode.Next)
		{
			diagNode.Value.Listener.NotifyMeasurement(this, measurement, tags, diagNode.Value.State);
		}
	}

	protected void RecordMeasurement(T measurement, KeyValuePair<string, object?> tag)
	{
		RecordMeasurement(measurement, new ReadOnlySpan<KeyValuePair<string, object>>((KeyValuePair<string, object>)tag));
	}

	protected void RecordMeasurement(T measurement, KeyValuePair<string, object?> tag1, KeyValuePair<string, object?> tag2)
	{
		global::_003C_003Ey__InlineArray2<KeyValuePair<string, object>> buffer = default(global::_003C_003Ey__InlineArray2<KeyValuePair<string, object>>);
		buffer[0] = tag1;
		buffer[1] = tag2;
		RecordMeasurement(measurement, buffer);
	}

	protected void RecordMeasurement(T measurement, KeyValuePair<string, object?> tag1, KeyValuePair<string, object?> tag2, KeyValuePair<string, object?> tag3)
	{
		global::_003C_003Ey__InlineArray3<KeyValuePair<string, object>> buffer = default(global::_003C_003Ey__InlineArray3<KeyValuePair<string, object>>);
		buffer[0] = tag1;
		buffer[1] = tag2;
		buffer[2] = tag3;
		RecordMeasurement(measurement, buffer);
	}

	protected void RecordMeasurement(T measurement, in TagList tagList)
	{
		RecordMeasurement(measurement, tagList.Tags);
	}
}
