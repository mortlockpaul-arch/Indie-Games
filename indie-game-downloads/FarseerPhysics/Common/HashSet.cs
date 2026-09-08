using System;
using System.Collections;
using System.Collections.Generic;

namespace FarseerPhysics.Common;

public class HashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
	{
		private HashSet<T> set;

		private Dictionary<T, short>.Enumerator enumerator;

		public T Current => enumerator.Current.Key;

		object IEnumerator.Current => Current;

		internal Enumerator(HashSet<T> set, Dictionary<T, short>.Enumerator enumerator)
		{
			this.set = set;
			this.enumerator = enumerator;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			((IEnumerator)enumerator).Reset();
		}
	}

	private Dictionary<T, short> _dict;

	public int Count => _dict.Keys.Count;

	public bool IsReadOnly => false;

	public HashSet(int capacity)
	{
		_dict = new Dictionary<T, short>(capacity);
	}

	public HashSet()
	{
		_dict = new Dictionary<T, short>();
	}

	public void Add(T item)
	{
		_dict.Add(item, 0);
	}

	public void Clear()
	{
		_dict.Clear();
	}

	public bool Contains(T item)
	{
		return _dict.ContainsKey(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(T item)
	{
		return _dict.Remove(item);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this, _dict.GetEnumerator());
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(this, _dict.GetEnumerator());
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this, _dict.GetEnumerator());
	}
}
