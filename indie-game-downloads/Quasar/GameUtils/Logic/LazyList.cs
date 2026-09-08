using System;
using System.Collections.Generic;

namespace Quasar.GameUtils.Logic;

public class LazyList<T> : List<T>
{
	private List<T> removalPendingItems = new List<T>(4);

	private List<T> addPendingItems = new List<T>(4);

	public List<T> AddPendingItems => addPendingItems;

	public int EffectiveCount => base.Count - removalPendingItems.Count + addPendingItems.Count;

	public event Action<T> OnItemRemoved;

	public event Action<T> OnItemAdded;

	public LazyList()
	{
	}

	public LazyList(int capacity)
		: base(capacity)
	{
	}

	public void RemoveLater(T item)
	{
		removalPendingItems.Add(item);
	}

	public void AddLater(T item)
	{
		addPendingItems.Add(item);
	}

	public void RemovePending()
	{
		foreach (T removalPendingItem in removalPendingItems)
		{
			Remove(removalPendingItem);
			if (OnItemRemoved != null)
			{
				OnItemRemoved(removalPendingItem);
			}
		}
		removalPendingItems.Clear();
	}

	public void AddPending()
	{
		foreach (T addPendingItem in addPendingItems)
		{
			Add(addPendingItem);
			if (OnItemAdded != null)
			{
				OnItemAdded(addPendingItem);
			}
		}
		addPendingItems.Clear();
	}

	public void RemoveAllNow()
	{
		RemovePending();
		for (int num = base.Count - 1; num >= 0; num--)
		{
			T obj = base[num];
			RemoveAt(num);
			if (OnItemRemoved != null)
			{
				OnItemRemoved(obj);
			}
		}
	}
}
