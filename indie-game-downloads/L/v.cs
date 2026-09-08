using System.Collections;
using System.Collections.Generic;

namespace l;

internal struct v<TKey, TValue>(IDictionary<TKey, TValue> dictionary) : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	private readonly IDictionary<TKey, TValue> a5h = dictionary;

	public int Count => a5h.Count;

	public bool IsReadOnly => true;

	public TValue this[TKey key] => a5h[key];

	public IEnumerable<TKey> Keys => new l.B<TKey>(a5h.Keys);

	public IEnumerable<TValue> Values => new l.B<TValue>(a5h.Values);

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return a5h.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return a5h.GetEnumerator();
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return a5h.Contains(item);
	}

	public bool ContainsKey(TKey key)
	{
		return a5h.ContainsKey(key);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		a5h.CopyTo(array, arrayIndex);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return a5h.TryGetValue(key, out value);
	}
}
