using System;
using System.Collections.Generic;

namespace Eyehook.Framework;

public class WeightedRandomList<T>
{
	private class Entry
	{
		public T Value;

		public int Min;

		public int Max;

		public Entry(T val, int min, int max)
		{
			Value = val;
			Min = min;
			Max = max;
		}
	}

	private static Random rand = new Random();

	private List<WeightedRandomList<T>.Entry> entries = new List<Entry>();

	public void Clear()
	{
		entries.Clear();
	}

	public void Add(T value, int weight)
	{
		int num = 0;
		if (entries.Count > 0)
		{
			num = entries[entries.Count - 1].Max + 1;
		}
		int max = num + weight - 1;
		entries.Add(new Entry(value, num, max));
	}

	public T Random()
	{
		int num = rand.Next(entries[entries.Count - 1].Max + 1);
		int i;
		for (i = 0; num > entries[i].Max; i++)
		{
		}
		return entries[i].Value;
	}
}
