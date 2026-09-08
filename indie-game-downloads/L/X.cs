using System;
using System.Collections;
using System.Collections.Generic;

namespace l;

internal struct X<T>(IList<T> wrappedList) : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public struct _00065h(IList<T> wrappedList) : IEnumerator<T>, IDisposable, IEnumerator
	{
		private IList<T> a5h = wrappedList;

		private int a5b = -1;

		public T Current => a5h[a5b];

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return ++a5b < a5h.Count;
		}

		public void Reset()
		{
			a5b = -1;
		}
	}

	private IList<T> a5h = wrappedList;

	public T this[int index]
	{
		get
		{
			return a5h[index];
		}
		set
		{
			throw new NotSupportedException("The list is read-only.");
		}
	}

	public int Count => a5h.Count;

	bool ICollection<T>.IsReadOnly => true;

	public int IndexOf(T item)
	{
		return a5h.IndexOf(item);
	}

	void IList<T>.Insert(int index, T item)
	{
		throw new NotSupportedException("The list is read-only.");
	}

	void IList<T>.RemoveAt(int index)
	{
		throw new NotSupportedException("The list is read-only.");
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException("The list is read-only.");
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException("The list is read-only.");
	}

	public bool Contains(T item)
	{
		return a5h.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		a5h.CopyTo(array, arrayIndex);
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException("The list is read-only.");
	}

	public _00065h GetEnumerator()
	{
		return new _00065h(a5h);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return a5h.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return a5h.GetEnumerator();
	}
}
