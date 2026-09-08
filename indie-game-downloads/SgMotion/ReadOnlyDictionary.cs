using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SgMotion;

public class ReadOnlyDictionary<T, V> : IDictionary<T, V>, ICollection<KeyValuePair<T, V>>, IEnumerable<KeyValuePair<T, V>>, IEnumerable
{
	private static readonly string ReadOnlyException = "Collection is read-only.";

	private readonly IDictionary<T, V> items;

	public IDictionary<T, V> Items => items;

	V IDictionary<T, V>.this[T key]
	{
		get
		{
			return items[key];
		}
		set
		{
			throw new NotSupportedException(ReadOnlyException);
		}
	}

	ICollection<T> IDictionary<T, V>.Keys
	{
		get
		{
			throw new NotSupportedException(ReadOnlyException);
		}
	}

	ICollection<V> IDictionary<T, V>.Values
	{
		get
		{
			throw new NotSupportedException(ReadOnlyException);
		}
	}

	public V this[T key] => items[key];

	public ReadOnlyCollection<T> Keys => new ReadOnlyCollection<T>(new List<T>(items.Keys));

	public ReadOnlyCollection<V> Values => new ReadOnlyCollection<V>(new List<V>(items.Values));

	public int Count => items.Count;

	public bool IsReadOnly => true;

	public ReadOnlyDictionary(IDictionary<T, V> dictionary)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		items = dictionary;
	}

	void ICollection<KeyValuePair<T, V>>.CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
	{
	}

	void ICollection<KeyValuePair<T, V>>.Clear()
	{
		throw new NotSupportedException();
	}

	public bool Contains(KeyValuePair<T, V> item)
	{
		throw new NotSupportedException();
	}

	void ICollection<KeyValuePair<T, V>>.Add(KeyValuePair<T, V> item)
	{
		throw new NotSupportedException(ReadOnlyException);
	}

	void IDictionary<T, V>.Add(T key, V value)
	{
		throw new NotSupportedException(ReadOnlyException);
	}

	bool ICollection<KeyValuePair<T, V>>.Remove(KeyValuePair<T, V> item)
	{
		throw new NotSupportedException(ReadOnlyException);
	}

	bool IDictionary<T, V>.Remove(T key)
	{
		throw new NotSupportedException(ReadOnlyException);
	}

	public bool ContainsKey(T key)
	{
		return items.ContainsKey(key);
	}

	public bool TryGetValue(T key, out V value)
	{
		return items.TryGetValue(key, out value);
	}

	public IEnumerator GetEnumerator()
	{
		return items.GetEnumerator();
	}

	IEnumerator<KeyValuePair<T, V>> IEnumerable<KeyValuePair<T, V>>.GetEnumerator()
	{
		return items.GetEnumerator();
	}
}
