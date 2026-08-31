using System;
using System.Collections;
using System.Collections.Generic;

namespace BEPUphysics.DataStructures;

/// <summary>
///  No-frills list that wraps an accessible array.
/// </summary>
/// <typeparam name="T">Type of elements contained by the list.</typeparam>
public class RawList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	/// <summary>
	///  Enumerator for the RawList.
	/// </summary>
	public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
	{
		private RawList<T> list;

		private int index;

		public T Current => list.Elements[index];

		object IEnumerator.Current => list.Elements[index];

		/// <summary>
		///  Constructs a new enumerator.
		/// </summary>
		/// <param name="list"></param>
		public Enumerator(RawList<T> list)
		{
			index = -1;
			this.list = list;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return ++index < list.count;
		}

		public void Reset()
		{
			index = -1;
		}
	}

	/// <summary>
	///  Direct access to the elements owned by the raw list.
	///  Be careful about the operations performed on this list;
	///  use the normal access methods if in doubt.
	/// </summary>
	public T[] Elements;

	internal int count;

	/// <summary>
	/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </summary>
	/// <returns>
	/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </returns>
	public int Count => count;

	/// <summary>
	///  Gets or sets the current size allocated for the list.
	/// </summary>
	public int Capacity
	{
		get
		{
			return Elements.Length;
		}
		set
		{
			T[] array = new T[value];
			Array.Copy(Elements, array, count);
			Elements = array;
		}
	}

	/// <summary>
	/// Gets or sets the element of the list at the given index.
	/// </summary>
	/// <param name="index">Index in the list.</param>
	/// <returns>Element at the given index.</returns>
	public T this[int index]
	{
		get
		{
			if (index < count && index >= 0)
			{
				return Elements[index];
			}
			throw new IndexOutOfRangeException("Index is outside of the list's bounds.");
		}
		set
		{
			if (index < count && index >= 0)
			{
				Elements[index] = value;
				return;
			}
			throw new IndexOutOfRangeException("Index is outside of the list's bounds.");
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	/// <summary>
	///  Constructs an empty list.
	/// </summary>
	public RawList()
	{
		Elements = new T[4];
	}

	/// <summary>
	///  Constructs an empty list.
	/// </summary>
	/// <param name="initialCapacity">Initial capacity to allocate for the list.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the initial capacity is zero or negative.</exception>
	public RawList(int initialCapacity)
	{
		if (initialCapacity <= 0)
		{
			throw new ArgumentException("Initial capacity must be positive.");
		}
		Elements = new T[initialCapacity];
	}

	/// <summary>
	///  Constructs a raw list from another list.
	/// </summary>
	/// <param name="elements">List to copy.</param>
	public RawList(IList<T> elements)
		: this(Math.Max(elements.Count, 4))
	{
		elements.CopyTo(Elements, 0);
		count = elements.Count;
	}

	/// <summary>
	/// Removes an element from the list.
	/// </summary>
	/// <param name="index">Index of the element to remove.</param>
	public void RemoveAt(int index)
	{
		if (index >= count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		count--;
		if (index < count)
		{
			Array.Copy(Elements, index + 1, Elements, index, count - index);
		}
		Elements[count] = default(T);
	}

	/// <summary>
	/// Removes an element from the list without maintaining order.
	/// </summary>
	/// <param name="index">Index of the element to remove.</param>
	public void FastRemoveAt(int index)
	{
		if (index >= count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		count--;
		if (index < count)
		{
			Elements[index] = Elements[count];
		}
		Elements[count] = default(T);
	}

	/// <summary>
	/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </summary>
	/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
	public void Add(T item)
	{
		if (count == Elements.Length)
		{
			Capacity = Elements.Length * 2;
		}
		Elements[count++] = item;
	}

	/// <summary>
	///  Adds a range of elements to the list from another list.
	/// </summary>
	/// <param name="items">Elements to add.</param>
	public void AddRange(RawList<T> items)
	{
		int num = count + items.count;
		if (num > Elements.Length)
		{
			int num2 = Elements.Length * 2;
			if (num2 < num)
			{
				num2 = num;
			}
			Capacity = num2;
		}
		Array.Copy(items.Elements, 0, Elements, count, items.count);
		count = num;
	}

	/// <summary>
	///  Adds a range of elements to the list from another list.
	/// </summary>
	/// <param name="items">Elements to add.</param>
	public void AddRange(List<T> items)
	{
		int num = count + items.Count;
		if (num > Elements.Length)
		{
			int num2 = Elements.Length * 2;
			if (num2 < num)
			{
				num2 = num;
			}
			Capacity = num2;
		}
		items.CopyTo(0, Elements, count, items.Count);
		count = num;
	}

	/// <summary>
	///  Adds a range of elements to the list from another list.
	/// </summary>
	/// <param name="items">Elements to add.</param>
	public void AddRange(IList<T> items)
	{
		int num = count + items.Count;
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
		count = num;
	}

	/// <summary>
	/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </summary>
	/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
	public void Clear()
	{
		Array.Clear(Elements, 0, count);
		count = 0;
	}

	/// <summary>
	/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </summary>
	/// <returns>
	/// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </returns>
	/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
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

	/// <summary>
	/// Removes the first occurrence of a specific object from the collection without maintaining element order.
	/// </summary>
	/// <returns>
	/// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
	/// </returns>
	/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
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

	/// <summary>
	/// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
	/// </summary>
	/// <returns>
	/// The index of <paramref name="item" /> if found in the list; otherwise, -1.
	/// </returns>
	/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
	public int IndexOf(T item)
	{
		return Array.IndexOf(Elements, item, 0, count);
	}

	/// <summary>
	/// Copies the elements from the list into an array.
	/// </summary>
	/// <returns>An array containing the elements in the list.</returns>
	public T[] ToArray()
	{
		T[] array = new T[count];
		Array.Copy(Elements, array, count);
		return array;
	}

	/// <summary>
	/// Inserts the element at the specified index.
	/// </summary>
	/// <param name="index">Index to insert the item.</param>
	/// <param name="item">Element to insert.</param>
	public void Insert(int index, T item)
	{
		if (index < count)
		{
			if (count == Elements.Length)
			{
				Capacity = Elements.Length * 2;
			}
			Array.Copy(Elements, index, Elements, index + 1, count - index);
			Elements[index] = item;
			count++;
		}
		else
		{
			Add(item);
		}
	}

	/// <summary>
	/// Inserts the element at the specified index without maintaining list order.
	/// </summary>
	/// <param name="index">Index to insert the item.</param>
	/// <param name="item">Element to insert.</param>
	public void FastInsert(int index, T item)
	{
		if (index < count)
		{
			if (count == Elements.Length)
			{
				Capacity = Elements.Length * 2;
			}
			Array.Copy(Elements, index, Elements, index + 1, count - index);
			Elements[count] = Elements[index];
			Elements[index] = item;
			count++;
		}
		else
		{
			Add(item);
		}
	}

	/// <summary>
	/// Determines if an item is present in the list.
	/// </summary>
	/// <param name="item">Item to be tested.</param>
	/// <returns>Whether or not the item was contained by the list.</returns>
	public bool Contains(T item)
	{
		return IndexOf(item) != -1;
	}

	/// <summary>
	/// Copies the list's contents to the array.
	/// </summary>
	/// <param name="array">Array to receive the list's contents.</param>
	/// <param name="arrayIndex">Index in the array to start the dump.</param>
	public void CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy(Elements, 0, array, arrayIndex, count);
	}

	/// <summary>
	///  Gets an enumerator for the list.
	/// </summary>
	/// <returns>Enumerator for the list.</returns>
	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}

	/// <summary>
	///  Sorts the list.
	/// </summary>
	/// <param name="comparer">Comparer to use to sort the list.</param>
	public void Sort(IComparer<T> comparer)
	{
		Array.Sort(Elements, 0, count, comparer);
	}
}
