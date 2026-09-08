using System;
using System.Collections;
using System.Collections.Generic;

namespace Quasar.Global;

public class PriorityQueue<TPriority, TValue> : ICollection, IEnumerable<KeyValuePair<TPriority, TValue>>, IEnumerable
{
	private const int DefaultCapacity = 16;

	private KeyValuePair<TPriority, TValue>[] items;

	private int capacity;

	private int numItems;

	private Comparison<TPriority> compareFunc;

	public int Count => numItems;

	public int Capacity
	{
		get
		{
			return items.Length;
		}
		set
		{
			SetCapacity(value);
		}
	}

	public bool IsSynchronized => false;

	public object SyncRoot => items.SyncRoot;

	public PriorityQueue()
		: this(16, (IComparer<TPriority>)Comparer<TPriority>.Default)
	{
	}

	public PriorityQueue(int initialCapacity)
		: this(initialCapacity, (IComparer<TPriority>)Comparer<TPriority>.Default)
	{
	}

	public PriorityQueue(IComparer<TPriority> comparer)
		: this(16, comparer)
	{
	}

	public PriorityQueue(int initialCapacity, IComparer<TPriority> comparer)
	{
		Init(initialCapacity, comparer.Compare);
	}

	public PriorityQueue(Comparison<TPriority> comparison)
		: this(16, comparison)
	{
	}

	public PriorityQueue(int initialCapacity, Comparison<TPriority> comparison)
	{
		Init(initialCapacity, comparison);
	}

	private void Init(int initialCapacity, Comparison<TPriority> comparison)
	{
		numItems = 0;
		compareFunc = comparison;
		SetCapacity(initialCapacity);
	}

	private void SetCapacity(int newCapacity)
	{
		int num = newCapacity;
		if (num < 16)
		{
			num = 16;
		}
		if (num < numItems)
		{
			throw new ArgumentOutOfRangeException("newCapacity", "New capacity is less than Count");
		}
		capacity = num;
		if (items == null)
		{
			items = new KeyValuePair<TPriority, TValue>[num];
		}
		else
		{
			Array.Resize(ref items, num);
		}
	}

	public void Enqueue(KeyValuePair<TPriority, TValue> newItem)
	{
		if (numItems == capacity)
		{
			SetCapacity(3 * Capacity / 2);
		}
		int num = numItems;
		numItems++;
		while (num > 0 && compareFunc(items[(num - 1) / 2].Key, newItem.Key) < 0)
		{
			ref KeyValuePair<TPriority, TValue> reference = ref items[num];
			reference = items[(num - 1) / 2];
			num = (num - 1) / 2;
		}
		items[num] = newItem;
	}

	public void Enqueue(TPriority priority, TValue value)
	{
		Enqueue(new KeyValuePair<TPriority, TValue>(priority, value));
	}

	private KeyValuePair<TPriority, TValue> RemoveAt(int index)
	{
		KeyValuePair<TPriority, TValue> result = items[index];
		numItems--;
		KeyValuePair<TPriority, TValue> keyValuePair = items[numItems];
		items[numItems] = default(KeyValuePair<TPriority, TValue>);
		if (numItems > 0 && index != numItems)
		{
			int num = index;
			int num2 = (num - 1) / 2;
			while (compareFunc(keyValuePair.Key, items[num2].Key) > 0)
			{
				ref KeyValuePair<TPriority, TValue> reference = ref items[num];
				reference = items[num2];
				num = num2;
				num2 = (num - 1) / 2;
			}
			if (num == index)
			{
				while (num < numItems / 2)
				{
					int num3 = 2 * num + 1;
					if (num3 < numItems - 1 && compareFunc(items[num3].Key, items[num3 + 1].Key) < 0)
					{
						num3++;
					}
					if (compareFunc(items[num3].Key, keyValuePair.Key) <= 0)
					{
						break;
					}
					ref KeyValuePair<TPriority, TValue> reference2 = ref items[num];
					reference2 = items[num3];
					num = num3;
				}
			}
			items[num] = keyValuePair;
		}
		return result;
	}

	public bool VerifyQueue()
	{
		for (int i = 0; i < numItems / 2; i++)
		{
			int num = 2 * i + 1;
			int num2 = num + 1;
			if (compareFunc(items[i].Key, items[num].Key) < 0)
			{
				return false;
			}
			if (num2 < numItems && compareFunc(items[i].Key, items[num2].Key) < 0)
			{
				return false;
			}
		}
		return true;
	}

	public KeyValuePair<TPriority, TValue> Dequeue()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException("The queue is empty");
		}
		return RemoveAt(0);
	}

	public void Remove(TValue item, IEqualityComparer comparer)
	{
		for (int i = 0; i < numItems; i++)
		{
			if (comparer.Equals(item, items[i].Value))
			{
				RemoveAt(i);
				break;
			}
		}
	}

	public void Remove(TValue item)
	{
		Remove(item, EqualityComparer<TValue>.Default);
	}

	public KeyValuePair<TPriority, TValue> Peek()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException("The queue is empty");
		}
		return items[0];
	}

	public void Clear()
	{
		for (int i = 0; i < numItems; i++)
		{
			items[i] = default(KeyValuePair<TPriority, TValue>);
		}
		numItems = 0;
		TrimExcess();
	}

	public void TrimExcess()
	{
		if ((float)numItems < 0.9f * (float)capacity)
		{
			SetCapacity(numItems);
		}
	}

	public bool Contains(TValue o)
	{
		KeyValuePair<TPriority, TValue>[] array = items;
		foreach (KeyValuePair<TPriority, TValue> keyValuePair in array)
		{
			if (keyValuePair.Value.Equals(o))
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(KeyValuePair<TPriority, TValue>[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
		}
		if (array.Rank > 1)
		{
			throw new ArgumentException("array is multidimensional.");
		}
		if (numItems != 0)
		{
			if (arrayIndex >= array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than the length of the array.");
			}
			if (numItems > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source ICollection is greater than the available space from arrayIndex to the end of the destination array.");
			}
			for (int i = 0; i < numItems; i++)
			{
				ref KeyValuePair<TPriority, TValue> reference = ref array[arrayIndex + i];
				reference = items[i];
			}
		}
	}

	public void CopyTo(Array array, int index)
	{
		CopyTo((KeyValuePair<TPriority, TValue>[])array, index);
	}

	public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
	{
		for (int i = 0; i < numItems; i++)
		{
			yield return items[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
