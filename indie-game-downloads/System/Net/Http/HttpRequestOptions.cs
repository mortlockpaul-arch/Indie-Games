using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http;

[DebuggerDisplay("{DebuggerToString(),nq}")]
[DebuggerTypeProxy(typeof(HttpRequestOptionsDebugView))]
public sealed class HttpRequestOptions : IDictionary<string, object?>, ICollection<KeyValuePair<string, object?>>, IEnumerable<KeyValuePair<string, object?>>, IEnumerable, IReadOnlyDictionary<string, object?>, IReadOnlyCollection<KeyValuePair<string, object?>>
{
	private sealed class HttpRequestOptionsDebugView(HttpRequestOptions options)
	{
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<string, object>[] Items
		{
			get
			{
				HttpRequestOptions httpRequestOptions = options;
				KeyValuePair<string, object>[] array = new KeyValuePair<string, object>[((ICollection<KeyValuePair<string, object>>)httpRequestOptions).Count];
				((ICollection<KeyValuePair<string, object>>)httpRequestOptions).CopyTo(array, 0);
				return array;
			}
		}
	}

	private Dictionary<string, object?> Options { get; } = new Dictionary<string, object>();

	object? IReadOnlyDictionary<string, object?>.this[string key] => Options[key];

	IEnumerable<string> IReadOnlyDictionary<string, object?>.Keys => Options.Keys;

	IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => Options.Values;

	object? IDictionary<string, object?>.this[string key]
	{
		get
		{
			return Options[key];
		}
		set
		{
			Options[key] = value;
		}
	}

	ICollection<string> IDictionary<string, object?>.Keys => Options.Keys;

	ICollection<object?> IDictionary<string, object?>.Values => Options.Values;

	int ICollection<KeyValuePair<string, object?>>.Count => Options.Count;

	bool ICollection<KeyValuePair<string, object?>>.IsReadOnly => ((ICollection<KeyValuePair<string, object>>)Options).IsReadOnly;

	int IReadOnlyCollection<KeyValuePair<string, object?>>.Count => Options.Count;

	bool IReadOnlyDictionary<string, object?>.TryGetValue(string key, out object value)
	{
		return Options.TryGetValue(key, out value);
	}

	void IDictionary<string, object?>.Add(string key, object value)
	{
		Options.Add(key, value);
	}

	void ICollection<KeyValuePair<string, object?>>.Add(KeyValuePair<string, object> item)
	{
		((ICollection<KeyValuePair<string, object>>)Options).Add(item);
	}

	void ICollection<KeyValuePair<string, object?>>.Clear()
	{
		Options.Clear();
	}

	bool ICollection<KeyValuePair<string, object?>>.Contains(KeyValuePair<string, object> item)
	{
		return ((ICollection<KeyValuePair<string, object>>)Options).Contains(item);
	}

	bool IDictionary<string, object?>.ContainsKey(string key)
	{
		return Options.ContainsKey(key);
	}

	void ICollection<KeyValuePair<string, object?>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<string, object>>)Options).CopyTo(array, arrayIndex);
	}

	IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object?>>.GetEnumerator()
	{
		return Options.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)Options).GetEnumerator();
	}

	bool IDictionary<string, object?>.Remove(string key)
	{
		return Options.Remove(key);
	}

	bool ICollection<KeyValuePair<string, object?>>.Remove(KeyValuePair<string, object> item)
	{
		return ((ICollection<KeyValuePair<string, object>>)Options).Remove(item);
	}

	bool IReadOnlyDictionary<string, object?>.ContainsKey(string key)
	{
		return Options.ContainsKey(key);
	}

	bool IDictionary<string, object?>.TryGetValue(string key, out object value)
	{
		return Options.TryGetValue(key, out value);
	}

	public bool TryGetValue<TValue>(HttpRequestOptionsKey<TValue> key, [MaybeNullWhen(false)] out TValue value)
	{
		if (Options.TryGetValue(key.Key, out object value2) && value2 is TValue val)
		{
			value = val;
			return true;
		}
		value = default(TValue);
		return false;
	}

	public void Set<TValue>(HttpRequestOptionsKey<TValue> key, TValue value)
	{
		Options[key.Key] = value;
	}

	private string DebuggerToString()
	{
		return $"Count = {Options.Count}";
	}
}
