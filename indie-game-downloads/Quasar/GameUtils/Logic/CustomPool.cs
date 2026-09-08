using System.Collections;
using System.Collections.Generic;

namespace Quasar.GameUtils.Logic;

public class CustomPool<T> where T : class
{
	private Stack<T> stack;

	private int TotalItems;

	private ICreator<T> itemCreator;

	public int Count => stack.Count;

	public bool ThreadSafe { get; set; }

	public CustomPool(ICreator<T> itemCreator, int initialCapacity)
	{
		this.itemCreator = itemCreator;
		stack = new Stack<T>(initialCapacity);
		for (int i = 0; i < initialCapacity; i++)
		{
			stack.Push(itemCreator.Create());
		}
		TotalItems = initialCapacity;
	}

	public void SetCapacity(int capacity)
	{
		if (ThreadSafe)
		{
			lock (((ICollection)stack).SyncRoot)
			{
				setCapacity(capacity);
				return;
			}
		}
		setCapacity(capacity);
	}

	private void setCapacity(int capacity)
	{
		for (int i = TotalItems; i < capacity; i++)
		{
			stack.Push(itemCreator.Create());
			TotalItems++;
		}
	}

	public T Fetch()
	{
		if (ThreadSafe)
		{
			lock (((ICollection)stack).SyncRoot)
			{
				if (stack.Count > 0)
				{
					return stack.Pop();
				}
			}
		}
		else if (stack.Count > 0)
		{
			return stack.Pop();
		}
		TotalItems++;
		return itemCreator.Create();
	}

	public void Insert(T item)
	{
		if (ThreadSafe)
		{
			lock (((ICollection)stack).SyncRoot)
			{
				stack.Push(item);
				return;
			}
		}
		stack.Push(item);
	}
}
