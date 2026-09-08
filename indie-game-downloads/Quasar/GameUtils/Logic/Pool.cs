using System.Collections;
using System.Collections.Generic;

namespace Quasar.GameUtils.Logic;

public static class Pool<T> where T : class, new()
{
	private static Stack<T> stack;

	private static int totalItems;

	public static int TotalItems => totalItems;

	public static int Count => stack.Count;

	public static bool ThreadSafe { get; set; }

	static Pool()
	{
		stack = new Stack<T>(10);
		for (int i = 0; i < 10; i++)
		{
			stack.Push(new T());
		}
		totalItems = 10;
	}

	public static void SetCapacity(int capacity)
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

	private static void setCapacity(int capacity)
	{
		for (int i = totalItems; i < capacity; i++)
		{
			stack.Push(new T());
			totalItems++;
		}
	}

	public static T Fetch()
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
		totalItems++;
		return new T();
	}

	public static void Insert(T item)
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
