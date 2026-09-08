using System;

namespace l;

internal struct W<T> where T : struct, IEquatable<T>
{
	private T a5h;

	private T a5b;

	private T a56;

	private T a5a;

	private T a57;

	private T a5_0006;

	private T a5v;

	private T a5B;

	internal int a5X;

	public int Count => a5X;

	public override string ToString()
	{
		return string.Concat("TinyStructList<", typeof(T), ">, Count: ", a5X);
	}

	public bool Add(ref T item)
	{
		switch (a5X)
		{
		case 0:
			a5h = item;
			break;
		case 1:
			a5b = item;
			break;
		case 2:
			a56 = item;
			break;
		case 3:
			a5a = item;
			break;
		case 4:
			a57 = item;
			break;
		case 5:
			a5_0006 = item;
			break;
		case 6:
			a5v = item;
			break;
		case 7:
			a5B = item;
			break;
		default:
			return false;
		}
		a5X++;
		return true;
	}

	public void Clear()
	{
		a5X = 0;
	}

	public bool Get(int index, out T item)
	{
		if (index > a5X - 1 || index < 0)
		{
			item = default(T);
			return false;
		}
		switch (index)
		{
		case 0:
			item = a5h;
			return true;
		case 1:
			item = a5b;
			return true;
		case 2:
			item = a56;
			return true;
		case 3:
			item = a5a;
			return true;
		case 4:
			item = a57;
			return true;
		case 5:
			item = a5_0006;
			return true;
		case 6:
			item = a5v;
			return true;
		case 7:
			item = a5B;
			return true;
		default:
			item = default(T);
			return false;
		}
	}

	public int IndexOf(ref T item)
	{
		if (a5h.Equals(item))
		{
			return 0;
		}
		if (a5b.Equals(item))
		{
			return 1;
		}
		if (a56.Equals(item))
		{
			return 2;
		}
		if (a5a.Equals(item))
		{
			return 3;
		}
		if (a57.Equals(item))
		{
			return 4;
		}
		if (a5_0006.Equals(item))
		{
			return 5;
		}
		if (a5v.Equals(item))
		{
			return 6;
		}
		if (a5B.Equals(item))
		{
			return 7;
		}
		return -1;
	}

	public bool Remove(ref T item)
	{
		int num = IndexOf(ref item);
		if (num != -1)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public bool RemoveAt(int index)
	{
		if (index > a5X - 1 || index < 0)
		{
			return false;
		}
		switch (index)
		{
		case 0:
			a5h = a5b;
			a5b = a56;
			a56 = a5a;
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 1:
			a5b = a56;
			a56 = a5a;
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 2:
			a56 = a5a;
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 3:
			a5a = a57;
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 4:
			a57 = a5_0006;
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 5:
			a5_0006 = a5v;
			a5v = a5B;
			break;
		case 6:
			a5v = a5B;
			break;
		}
		a5X--;
		return true;
	}

	public bool Replace(int index, ref T item)
	{
		if (index > a5X - 1 || index < 0)
		{
			return false;
		}
		switch (index)
		{
		case 0:
			a5h = item;
			break;
		case 1:
			a5b = item;
			break;
		case 2:
			a56 = item;
			break;
		case 3:
			a5a = item;
			break;
		case 4:
			a57 = item;
			break;
		case 5:
			a5_0006 = item;
			break;
		case 6:
			a5v = item;
			break;
		case 7:
			a5B = item;
			break;
		default:
			return false;
		}
		return true;
	}
}
