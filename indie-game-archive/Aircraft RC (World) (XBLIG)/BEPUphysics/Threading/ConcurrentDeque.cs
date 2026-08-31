using System;

namespace BEPUphysics.Threading;

internal class ConcurrentDeque<T>
{
	private readonly SpinLock locker = new SpinLock();

	internal T[] array;

	private int count;

	internal int firstIndex;

	internal int lastIndex = -1;

	public int Count => count;

	public ConcurrentDeque(int capacity)
	{
		array = new T[capacity];
	}

	public ConcurrentDeque()
		: this(16)
	{
	}

	public override string ToString()
	{
		return "Count: " + count;
	}

	public void Enqueue(T item)
	{
		locker.Enter();
		try
		{
			if (count == this.array.Length)
			{
				T[] array = this.array;
				this.array = new T[Math.Max(4, array.Length * 2)];
				Array.Copy(array, firstIndex, this.array, 0, array.Length - firstIndex);
				Array.Copy(array, 0, this.array, array.Length - firstIndex, firstIndex);
				firstIndex = 0;
				lastIndex = count - 1;
			}
			lastIndex++;
			if (lastIndex == this.array.Length)
			{
				lastIndex = 0;
			}
			this.array[lastIndex] = item;
			count++;
		}
		finally
		{
			locker.Exit();
		}
	}

	public bool TryDequeueFirst(out T item)
	{
		locker.Enter();
		try
		{
			if (count > 0)
			{
				item = array[firstIndex];
				array[firstIndex] = default(T);
				firstIndex++;
				if (firstIndex == array.Length)
				{
					firstIndex = 0;
				}
				count--;
				return true;
			}
			item = default(T);
			return false;
		}
		finally
		{
			locker.Exit();
		}
	}

	public bool TryDequeueLast(out T item)
	{
		locker.Enter();
		try
		{
			if (count > 0)
			{
				item = array[lastIndex];
				array[lastIndex] = default(T);
				lastIndex--;
				if (lastIndex < 0)
				{
					lastIndex += array.Length;
				}
				count--;
				return true;
			}
			item = default(T);
			return false;
		}
		finally
		{
			locker.Exit();
		}
	}

	public bool TryUnsafeDequeueFirst(out T item)
	{
		if (count > 0)
		{
			item = array[firstIndex];
			array[firstIndex] = default(T);
			firstIndex++;
			if (firstIndex == array.Length)
			{
				firstIndex = 0;
			}
			count--;
			return true;
		}
		item = default(T);
		return false;
	}

	public bool TryUnsafeDequeueLast(out T item)
	{
		if (count > 0)
		{
			item = array[lastIndex];
			array[lastIndex] = default(T);
			lastIndex--;
			if (lastIndex < 0)
			{
				lastIndex += array.Length;
			}
			count--;
			return true;
		}
		item = default(T);
		return false;
	}

	public void UnsafeEnqueue(T item)
	{
		if (count == this.array.Length)
		{
			T[] array = this.array;
			this.array = new T[array.Length * 2];
			Array.Copy(array, firstIndex, this.array, 0, array.Length - firstIndex);
			Array.Copy(array, 0, this.array, array.Length - firstIndex, firstIndex);
			firstIndex = 0;
			lastIndex = count - 1;
		}
		lastIndex++;
		if (lastIndex == this.array.Length)
		{
			lastIndex = 0;
		}
		this.array[lastIndex] = item;
		count++;
	}
}
