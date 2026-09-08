using System.Collections;
using System.Collections.Generic;

namespace BEPUphysics.DataStructures;

/// <summary>
/// Provides basic .NET 3.5 HashSet functionality on non-Windows platforms.
/// </summary>
public class HashSet<T> : IEnumerable<T>, IEnumerable
{
	private Dictionary<T, bool> a5h;

	/// <summary>
	///  Constructs a new HashSet.
	/// </summary>
	public HashSet()
	{
		a5h = new Dictionary<T, bool>();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return a5h.Keys.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return a5h.Keys.GetEnumerator();
	}

	private Dictionary<T, bool>.KeyCollection.Enumerator bs()
	{
		return a5h.Keys.GetEnumerator();
	}

	/// <summary>
	///  Adds an element to the HashSet.
	/// </summary>
	/// <param name="item">Item to add.</param>
	/// <returns>Whether or not the item could be added.</returns>
	public bool Add(T item)
	{
		if (a5h.ContainsKey(item))
		{
			return false;
		}
		a5h.Add(item, value: false);
		return true;
	}

	/// <summary>
	///  Removes an element from the HashSet.
	/// </summary>
	/// <param name="item">Item to remove.</param>
	/// <returns>Whether or not the item could be removed.</returns>
	public bool Remove(T item)
	{
		return a5h.Remove(item);
	}

	/// <summary>
	///  Clears the HashSet.
	/// </summary>
	public void Clear()
	{
		a5h.Clear();
	}

	/// <summary>
	/// Determines if the set contains the item.
	/// </summary>
	/// <param name="item">Item to check for containment.</param>
	/// <returns>Whether or not the item was contained in the set.</returns>
	public bool Contains(T item)
	{
		return a5h.ContainsKey(item);
	}
}
