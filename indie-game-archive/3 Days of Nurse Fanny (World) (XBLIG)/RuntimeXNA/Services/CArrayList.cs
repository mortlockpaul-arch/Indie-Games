namespace RuntimeXNA.Services;

public class CArrayList
{
	private const int GROWTH_STEP = 5;

	private object[] array;

	private int numberOfEntries;

	private void getArray(int max)
	{
		if (this.array == null)
		{
			this.array = new object[max + 5];
		}
		else if (max >= this.array.Length)
		{
			object[] array = new object[max + 5];
			for (int i = 0; i < this.array.Length; i++)
			{
				array[i] = this.array[i];
			}
			this.array = array;
		}
	}

	public void ensureCapacity(int max)
	{
		getArray(max);
	}

	public bool isEmpty()
	{
		return numberOfEntries == 0;
	}

	public void add(object o)
	{
		getArray(numberOfEntries);
		array[numberOfEntries++] = o;
	}

	public void add(int index, object o)
	{
		getArray(numberOfEntries);
		for (int num = numberOfEntries; num > index; num--)
		{
			array[num] = array[num - 1];
		}
		array[index] = o;
		numberOfEntries++;
	}

	public object get(int index)
	{
		if (array != null && index < array.Length)
		{
			return array[index];
		}
		return null;
	}

	public void set(int index, object o)
	{
		if (array != null && index < array.Length)
		{
			array[index] = o;
		}
	}

	public void insert(int index, object o)
	{
		getArray(numberOfEntries);
		for (int num = numberOfEntries; num > index; num--)
		{
			array[num] = array[num - 1];
		}
		array[index] = o;
		numberOfEntries++;
	}

	public void swap(int index1, int index2)
	{
		if (array != null)
		{
			object obj = array[index1];
			array[index1] = array[index2];
			array[index2] = obj;
		}
	}

	public void swap(object o1, object o2)
	{
		if (array != null)
		{
			int num = indexOf(o1);
			int num2 = indexOf(o2);
			if (num >= 0 && num2 >= 0)
			{
				swap(num, num2);
			}
		}
	}

	public void remove(int index)
	{
		if (array != null && index < array.Length && numberOfEntries > 0)
		{
			for (int i = index; i < numberOfEntries - 1; i++)
			{
				array[i] = array[i + 1];
			}
			numberOfEntries--;
			array[numberOfEntries] = null;
		}
	}

	public int indexOf(object o)
	{
		for (int i = 0; i < numberOfEntries; i++)
		{
			if (array[i] == o)
			{
				return i;
			}
		}
		return -1;
	}

	public void remove(object o)
	{
		int num = indexOf(o);
		if (num >= 0)
		{
			remove(num);
		}
	}

	public int size()
	{
		return numberOfEntries;
	}

	public void clear()
	{
		numberOfEntries = 0;
	}
}
