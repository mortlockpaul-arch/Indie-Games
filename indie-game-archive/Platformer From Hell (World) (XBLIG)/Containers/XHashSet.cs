using System;
using System.Collections;
using System.Collections.Generic;

namespace Containers;

public class XHashSet<T> : IEnumerable<T>, IEnumerable where T : class
{
	private struct ValueEnumerator : IEnumerator<T>, IDisposable, IEnumerator
	{
		private Dictionary<T, bool> data;

		private Dictionary<T, bool>.Enumerator values;

		public T Current => values.Current.Key;

		object IEnumerator.Current => values.Current;

		internal ValueEnumerator(Dictionary<T, bool> hashset)
		{
			data = hashset;
			values = data.GetEnumerator();
		}

		public void Dispose()
		{
			values.Dispose();
		}

		public void Reset()
		{
			values = data.GetEnumerator();
		}

		public bool MoveNext()
		{
			return values.MoveNext();
		}
	}

	private Dictionary<T, bool> data = new Dictionary<T, bool>();

	public int Count => data.Count;

	public void Add(T t)
	{
		if (!Contains(t))
		{
			data.Add(t, value: true);
		}
	}

	public void Clear()
	{
		data.Clear();
	}

	public bool Contains(T t)
	{
		return data.ContainsKey(t);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return new ValueEnumerator(data);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ValueEnumerator(data);
	}
}
