using System;

namespace BEPUphysics.DataStructures;

/// <summary>
/// Special datatype used for heapless lists without unsafe/stackalloc.
/// Designed for object types or reference-sized structs (int, float...).
/// Stores a maximum of 8 entries.
/// </summary>
/// <typeparam name="T">Struct type to use.</typeparam>
public struct TinyList<T> where T : IEquatable<T>
{
	private T entry1;

	private T entry2;

	private T entry3;

	private T entry4;

	private T entry5;

	private T entry6;

	private T entry7;

	private T entry8;

	internal int count;

	/// <summary>
	/// Gets the current number of elements in the list.
	/// </summary>
	public int Count => count;

	/// <summary>
	/// Gets the item at the specified index.
	/// </summary>
	/// <param name="index">Index to retrieve.</param>
	/// <returns>Retrieved item.</returns>
	public T this[int index]
	{
		get
		{
			if (index > count - 1 || index < 0)
			{
				return default(T);
			}
			return index switch
			{
				0 => entry1, 
				1 => entry2, 
				2 => entry3, 
				3 => entry4, 
				4 => entry5, 
				5 => entry6, 
				6 => entry7, 
				7 => entry8, 
				_ => default(T), 
			};
		}
		set
		{
			if (index <= count - 1)
			{
				switch (index)
				{
				case 0:
					entry1 = value;
					break;
				case 1:
					entry2 = value;
					break;
				case 2:
					entry3 = value;
					break;
				case 3:
					entry4 = value;
					break;
				case 4:
					entry5 = value;
					break;
				case 5:
					entry6 = value;
					break;
				case 6:
					entry7 = value;
					break;
				case 7:
					entry8 = value;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Creates a string representation of the list.
	/// </summary>
	/// <returns>String representation of the list.</returns>
	public override string ToString()
	{
		return string.Concat("TinyList<", typeof(T), ">, Count: ", count);
	}

	/// <summary>
	/// Tries to add an element to the list.
	/// </summary>
	/// <param name="item">Item to add.</param>
	/// <returns>Whether or not the item could be added.
	/// Will return false when the list is full.</returns>
	public bool Add(T item)
	{
		switch (count)
		{
		case 0:
			entry1 = item;
			break;
		case 1:
			entry2 = item;
			break;
		case 2:
			entry3 = item;
			break;
		case 3:
			entry4 = item;
			break;
		case 4:
			entry5 = item;
			break;
		case 5:
			entry6 = item;
			break;
		case 6:
			entry7 = item;
			break;
		case 7:
			entry8 = item;
			break;
		default:
			return false;
		}
		count++;
		return true;
	}

	/// <summary>
	/// Clears the list.
	/// </summary>
	public void Clear()
	{
		count = 0;
		entry1 = default(T);
		entry2 = default(T);
		entry3 = default(T);
		entry4 = default(T);
		entry5 = default(T);
		entry6 = default(T);
		entry7 = default(T);
		entry8 = default(T);
	}

	/// <summary>
	/// Gets the index of the item in the list, if it is present.
	/// </summary>
	/// <param name="item">Item to look for.</param>
	/// <returns>Index of the item, if present.  -1 otherwise.</returns>
	public int IndexOf(T item)
	{
		if (entry1.Equals(item))
		{
			return 0;
		}
		if (entry2.Equals(item))
		{
			return 1;
		}
		if (entry3.Equals(item))
		{
			return 2;
		}
		if (entry4.Equals(item))
		{
			return 3;
		}
		if (entry5.Equals(item))
		{
			return 4;
		}
		if (entry6.Equals(item))
		{
			return 5;
		}
		if (entry7.Equals(item))
		{
			return 6;
		}
		if (entry8.Equals(item))
		{
			return 7;
		}
		return -1;
	}

	/// <summary>
	/// Tries to remove an element from the list.
	/// </summary>
	/// <param name="item">Item to remove.</param>
	/// <returns>Whether or not the item existed in the list.</returns>
	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num != -1)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes the item at the specified index.
	/// </summary>
	/// <param name="index">Index of the element to remove.</param>
	/// <returns>Whether or not the item could be removed.
	/// Returns false if the index is out of bounds.</returns>
	public bool RemoveAt(int index)
	{
		if (index > count - 1 || index < 0)
		{
			return false;
		}
		switch (index)
		{
		case 0:
			entry1 = entry2;
			entry2 = entry3;
			entry3 = entry4;
			entry4 = entry5;
			entry5 = entry6;
			entry6 = entry7;
			entry7 = entry8;
			break;
		case 1:
			entry2 = entry3;
			entry3 = entry4;
			entry4 = entry5;
			entry5 = entry6;
			entry6 = entry7;
			entry7 = entry8;
			break;
		case 2:
			entry3 = entry4;
			entry4 = entry5;
			entry5 = entry6;
			entry6 = entry7;
			entry7 = entry8;
			break;
		case 3:
			entry4 = entry5;
			entry5 = entry6;
			entry6 = entry7;
			entry7 = entry8;
			break;
		case 4:
			entry5 = entry6;
			entry6 = entry7;
			entry7 = entry8;
			break;
		case 5:
			entry6 = entry7;
			entry7 = entry8;
			break;
		case 6:
			entry7 = entry8;
			break;
		}
		count--;
		return true;
	}
}
