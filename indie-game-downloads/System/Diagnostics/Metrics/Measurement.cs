using System.Collections.Generic;
using System.Linq;

namespace System.Diagnostics.Metrics;

public readonly struct Measurement<T> where T : struct
{
	private readonly KeyValuePair<string, object>[] _tags;

	public ReadOnlySpan<KeyValuePair<string, object?>> Tags => _tags.AsSpan();

	public T Value { get; }

	public Measurement(T value)
	{
		_tags = Instrument.EmptyTags;
		Value = value;
	}

	public Measurement(T value, IEnumerable<KeyValuePair<string, object?>>? tags)
	{
		_tags = tags?.ToArray() ?? Instrument.EmptyTags;
		Value = value;
	}

	public Measurement(T value, params KeyValuePair<string, object?>[]? tags)
	{
		if (tags != null)
		{
			_tags = new KeyValuePair<string, object>[tags.Length];
			tags.CopyTo(_tags, 0);
		}
		else
		{
			_tags = Instrument.EmptyTags;
		}
		Value = value;
	}

	public Measurement(T value, params ReadOnlySpan<KeyValuePair<string, object?>> tags)
	{
		_tags = tags.ToArray();
		Value = value;
	}

	public Measurement(T value, in TagList tags)
	{
		if (tags.Count > 0)
		{
			_tags = new KeyValuePair<string, object>[tags.Count];
			tags.CopyTo(_tags.AsSpan());
		}
		else
		{
			_tags = Instrument.EmptyTags;
		}
		Value = value;
	}
}
