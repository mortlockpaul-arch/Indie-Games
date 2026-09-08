using System;
using System.Collections.Generic;

namespace Quasar.GameUtils.Logic;

public class LazyPooledList<T> : List<T> where T : class
{
	private List<T> removalPendingItems = new List<T>(4);

	private List<T> addPendingItems = new List<T>(4);

	private CustomPool<T> pool;

	public int EffectiveCount => base.Count - removalPendingItems.Count;

	public event Action<T> OnItemRemoved;

	public event Action<T> OnItemAdded;

	public LazyPooledList(ICreator<T> creator, int initialPoolCapacity)
	{
		pool = new CustomPool<T>(creator, initialPoolCapacity);
	}

	public LazyPooledList(int capacity, ICreator<T> creator, int initialPoolCapacity)
		: base(capacity)
	{
		pool = new CustomPool<T>(creator, initialPoolCapacity);
	}

	public T FetchAndAdd()
	{
		T val = pool.Fetch();
		Add(val);
		return val;
	}

	public T FetchAndAddLater()
	{
		T val = pool.Fetch();
		addPendingItems.Add(val);
		return val;
	}

	public void RemoveLater(T item)
	{
		if (Contains(item) && !removalPendingItems.Contains(item))
		{
			removalPendingItems.Add(item);
		}
	}

	public void RemovePending()
	{
		foreach (T removalPendingItem in removalPendingItems)
		{
			if (Remove(removalPendingItem))
			{
				if (OnItemRemoved != null)
				{
					OnItemRemoved(removalPendingItem);
				}
				pool.Insert(removalPendingItem);
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
			T val = base[num];
			RemoveAt(num);
			if (OnItemRemoved != null)
			{
				OnItemRemoved(val);
			}
			pool.Insert(val);
		}
	}

	public void RemoveNow(T item)
	{
		if (Remove(item))
		{
			if (OnItemRemoved != null)
			{
				OnItemRemoved(item);
			}
			pool.Insert(item);
		}
	}

	public void RemoveAllLater()
	{
		removalPendingItems.AddRange(this);
	}
}
