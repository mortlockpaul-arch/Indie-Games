namespace Microsoft.CSharp.RuntimeBinder.Syntax;

internal sealed class NameTable
{
	private sealed class Entry
	{
		public readonly Name Name;

		public readonly int HashCode;

		public Entry Next;

		public Entry(Name name, int hashCode, Entry next)
		{
			Name = name;
			HashCode = hashCode;
			Next = next;
		}
	}

	private Entry[] _entries;

	private int _count;

	private int _mask;

	internal NameTable()
	{
		_mask = 31;
		_entries = new Entry[_mask + 1];
	}

	public Name Add(string key)
	{
		int num = ComputeHashCode(key);
		for (Entry entry = _entries[num & _mask]; entry != null; entry = entry.Next)
		{
			if (entry.HashCode == num && entry.Name.Text.Equals(key))
			{
				return entry.Name;
			}
		}
		return AddEntry(new Name(key), num);
	}

	public Name Add(string key, int length)
	{
		int num = ComputeHashCode(key, length);
		for (Entry entry = _entries[num & _mask]; entry != null; entry = entry.Next)
		{
			if (entry.HashCode == num && Equals(entry.Name.Text, key, length))
			{
				return entry.Name;
			}
		}
		return AddEntry(new Name(key.Substring(0, length)), num);
	}

	internal void Add(Name name)
	{
		int hashCode = ComputeHashCode(name.Text);
		AddEntry(name, hashCode);
	}

	private static int ComputeHashCode(string key)
	{
		int num = key.Length;
		for (int i = 0; i < key.Length; i++)
		{
			num += (num << 7) ^ key[i];
		}
		num -= num >> 17;
		num -= num >> 11;
		return num - (num >> 5);
	}

	private static int ComputeHashCode(string key, int length)
	{
		int num = length;
		for (int i = 0; i < length; i++)
		{
			num += (num << 7) ^ key[i];
		}
		num -= num >> 17;
		num -= num >> 11;
		return num - (num >> 5);
	}

	private static bool Equals(string candidate, string key, int length)
	{
		if (candidate.Length != length)
		{
			return false;
		}
		for (int i = 0; i < candidate.Length; i++)
		{
			if (candidate[i] != key[i])
			{
				return false;
			}
		}
		return true;
	}

	private Name AddEntry(Name name, int hashCode)
	{
		int num = hashCode & _mask;
		Entry entry = new Entry(name, hashCode, _entries[num]);
		_entries[num] = entry;
		if (_count++ == _mask)
		{
			Grow();
		}
		return entry.Name;
	}

	private void Grow()
	{
		int num = _mask * 2 + 1;
		Entry[] entries = _entries;
		Entry[] array = new Entry[num + 1];
		for (int i = 0; i < entries.Length; i++)
		{
			Entry entry = entries[i];
			while (entry != null)
			{
				int num2 = entry.HashCode & num;
				Entry next = entry.Next;
				entry.Next = array[num2];
				array[num2] = entry;
				entry = next;
			}
		}
		_entries = array;
		_mask = num;
	}
}
