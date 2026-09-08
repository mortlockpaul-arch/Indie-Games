using System;
using System.Collections;
using System.Collections.Generic;

namespace l;

internal class _7<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public struct _00065h(l._7<T> list) : IEnumerator<T>, IDisposable, IEnumerator
	{
		private l._7<T> a5h = list;

		private int a5b = -1;

		public T Current => a5h.Elements[a5b];

		object IEnumerator.Current => a5h.Elements[a5b];

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return ++a5b < a5h.a5h;
		}

		public void Reset()
		{
			a5b = -1;
		}
	}

	public T[] Elements;

	internal int a5h;

	public int Count => a5h;

	public int Capacity
	{
		get
		{
			return Elements.Length;
		}
		set
		{
			T[] array = new T[value];
			Array.Copy(Elements, array, a5h);
			Elements = array;
		}
	}

	public T this[int index]
	{
		get
		{
			if (index < a5h && index >= 0)
			{
				return Elements[index];
			}
			throw new IndexOutOfRangeException("Index is outside of the list's bounds.");
		}
		set
		{
			if (index < a5h && index >= 0)
			{
				Elements[index] = value;
				return;
			}
			throw new IndexOutOfRangeException("Index is outside of the list's bounds.");
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	public _7()
	{
		Elements = new T[4];
	}

	public _7(int initialCapacity)
	{
		if (initialCapacity <= 0)
		{
			throw new ArgumentException("Initial capacity must be positive.");
		}
		Elements = new T[initialCapacity];
	}

	public _7(IList<T> elements)
		: this(Math.Max(elements.Count, 4))
	{
		elements.CopyTo(Elements, 0);
		a5h = elements.Count;
	}

	public void RemoveAt(int index)
	{
		if (index >= a5h)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		a5h--;
		if (index < a5h)
		{
			Array.Copy(Elements, index + 1, Elements, index, a5h - index);
		}
		Elements[a5h] = default(T);
	}

	public void FastRemoveAt(int index)
	{
		if (index >= a5h)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		a5h--;
		if (index < a5h)
		{
			Elements[index] = Elements[a5h];
		}
		Elements[a5h] = default(T);
	}

	public void Add(T item)
	{
		if (a5h == Elements.Length)
		{
			Capacity = Elements.Length * 2;
		}
		Elements[a5h++] = item;
	}

	public void AddRange(l._7<T> items)
	{
		int num = a5h + items.a5h;
		if (num > Elements.Length)
		{
			int num2 = Elements.Length * 2;
			if (num2 < num)
			{
				num2 = num;
			}
			Capacity = num2;
		}
		Array.Copy(items.Elements, 0, Elements, a5h, items.a5h);
		a5h = num;
	}

	public void AddRange(List<T> items)
	{
		int num = a5h + items.Count;
		if (num > Elements.Length)
		{
			int num2 = Elements.Length * 2;
			if (num2 < num)
			{
				num2 = num;
			}
			Capacity = num2;
		}
		items.CopyTo(0, Elements, a5h, items.Count);
		a5h = num;
	}

	public void AddRange(IList<T> items)
	{
		int num = a5h + items.Count;
		if (num > Elements.Length)
		{
			int num2 = Elements.Length * 2;
			if (num2 < num)
			{
				num2 = num;
			}
			Capacity = num2;
		}
		items.CopyTo(Elements, 0);
		a5h = num;
	}

	public void Clear()
	{
		Array.Clear(Elements, 0, a5h);
		a5h = 0;
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		RemoveAt(num);
		return true;
	}

	public bool FastRemove(T item)
	{
		int num = IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		FastRemoveAt(num);
		return true;
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf(Elements, item, 0, a5h);
	}

	public T[] ToArray()
	{
		T[] array = new T[a5h];
		Array.Copy(Elements, array, a5h);
		return array;
	}

	public void Insert(int index, T item)
	{
		if (index < a5h)
		{
			if (a5h == Elements.Length)
			{
				Capacity = Elements.Length * 2;
			}
			Array.Copy(Elements, index, Elements, index + 1, a5h - index);
			Elements[index] = item;
			a5h++;
		}
		else
		{
			Add(item);
		}
	}

	public void FastInsert(int index, T item)
	{
		if (index < a5h)
		{
			if (a5h == Elements.Length)
			{
				Capacity = Elements.Length * 2;
			}
			Array.Copy(Elements, index, Elements, index + 1, a5h - index);
			Elements[a5h] = Elements[index];
			Elements[index] = item;
			a5h++;
		}
		else
		{
			Add(item);
		}
	}

	public bool Contains(T item)
	{
		return IndexOf(item) != -1;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy(Elements, 0, array, arrayIndex, a5h);
	}

	public _00065h GetEnumerator()
	{
		return new _00065h(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new _00065h(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new _00065h(this);
	}

	public void Sort(IComparer<T> comparer)
	{
		Array.Sort(Elements, 0, a5h, comparer);
	}
}
