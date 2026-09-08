using System.Collections.Generic;

namespace System.Text.RegularExpressions.Symbolic;

internal sealed class BDDRangeConverter
{
	private readonly Dictionary<BDD, (uint, uint)[]> _rangeCache = new Dictionary<BDD, (uint, uint)[]>();

	private BDDRangeConverter()
	{
	}

	public static (uint, uint)[] ToRanges(BDD set)
	{
		if (set.IsEmpty)
		{
			return Array.Empty<(uint, uint)>();
		}
		if (set.IsFull)
		{
			return new(uint, uint)[1] { (0u, 65535u) };
		}
		BDDRangeConverter bDDRangeConverter = new BDDRangeConverter();
		return LiftRanges(16, 15 - set.Ordinal, bDDRangeConverter.ToRangesFromOrdinal(set));
	}

	private static (uint, uint)[] LiftRanges(int toBits, int newBits, (uint, uint)[] ranges)
	{
		if (newBits == 0)
		{
			return ranges;
		}
		int num = toBits - newBits;
		(uint, uint)[] array = new(uint, uint)[(1 << newBits) * ranges.Length];
		int num2 = 0;
		for (uint num3 = 0u; num3 < 1 << newBits; num3++)
		{
			uint num4 = num3 << num;
			for (int i = 0; i < ranges.Length; i++)
			{
				(uint, uint) tuple = ranges[i];
				array[num2++] = (tuple.Item1 | num4, tuple.Item2 | num4);
			}
		}
		uint num5 = (uint)((1 << num) - 1);
		if (ranges[0].Item1 == 0 && ranges[^1].Item2 == num5)
		{
			List<(uint, uint)> list = new List<(uint, uint)>();
			uint item = array[0].Item1;
			uint item2 = array[0].Item2;
			for (int j = 1; j < array.Length; j++)
			{
				if (item2 == array[j].Item1 - 1)
				{
					item2 = array[j].Item2;
					continue;
				}
				list.Add((item, item2));
				item = array[j].Item1;
				item2 = array[j].Item2;
			}
			list.Add((item, item2));
			array = list.ToArray();
		}
		return array;
	}

	private (uint, uint)[] ToRangesFromOrdinal(BDD set)
	{
		if (!_rangeCache.TryGetValue(set, out var value))
		{
			int ordinal = set.Ordinal;
			uint num = (uint)(1 << ordinal);
			if (set.Zero.IsEmpty)
			{
				if (set.One.IsFull)
				{
					value = new(uint, uint)[1] { (num, (num << 1) - 1) };
				}
				else
				{
					(uint, uint)[] array = LiftRanges(ordinal, ordinal - set.One.Ordinal - 1, ToRangesFromOrdinal(set.One));
					value = new(uint, uint)[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						value[i] = (array[i].Item1 | num, array[i].Item2 | num);
					}
				}
			}
			else if (set.Zero.IsFull)
			{
				if (set.One.IsEmpty)
				{
					value = new(uint, uint)[1] { (0u, num - 1) };
				}
				else
				{
					(uint, uint)[] array2 = LiftRanges(ordinal, ordinal - set.One.Ordinal - 1, ToRangesFromOrdinal(set.One));
					(uint, uint) tuple = array2[0];
					if (tuple.Item1 == 0)
					{
						value = new(uint, uint)[array2.Length];
						value[0] = (0u, tuple.Item2 | num);
						for (int j = 1; j < array2.Length; j++)
						{
							value[j] = (array2[j].Item1 | num, array2[j].Item2 | num);
						}
					}
					else
					{
						value = new(uint, uint)[array2.Length + 1];
						value[0] = (0u, num - 1);
						for (int k = 0; k < array2.Length; k++)
						{
							value[k + 1] = (array2[k].Item1 | num, array2[k].Item2 | num);
						}
					}
				}
			}
			else
			{
				(uint, uint)[] array3 = LiftRanges(ordinal, ordinal - set.Zero.Ordinal - 1, ToRangesFromOrdinal(set.Zero));
				(uint, uint) item = array3[^1];
				if (set.One.IsEmpty)
				{
					value = array3;
				}
				else if (set.One.IsFull)
				{
					List<(uint, uint)> list = new List<(uint, uint)>();
					for (int l = 0; l < array3.Length - 1; l++)
					{
						list.Add(array3[l]);
					}
					if (item.Item2 == num - 1)
					{
						list.Add((item.Item1, (num << 1) - 1));
					}
					else
					{
						list.Add(item);
						list.Add((num, (num << 1) - 1));
					}
					value = list.ToArray();
				}
				else
				{
					(uint, uint)[] ranges = ToRangesFromOrdinal(set.One);
					(uint, uint)[] array4 = LiftRanges(ordinal, ordinal - set.One.Ordinal - 1, ranges);
					(uint, uint) tuple2 = array4[0];
					if (item.Item2 == num - 1 && tuple2.Item1 == 0)
					{
						value = new(uint, uint)[array3.Length + array4.Length - 1];
						for (int m = 0; m < array3.Length - 1; m++)
						{
							value[m] = array3[m];
						}
						value[array3.Length - 1] = (item.Item1, tuple2.Item2 | num);
						for (int n = 1; n < array4.Length; n++)
						{
							value[array3.Length - 1 + n] = (array4[n].Item1 | num, array4[n].Item2 | num);
						}
					}
					else
					{
						value = new(uint, uint)[array3.Length + array4.Length];
						for (int num2 = 0; num2 < array3.Length; num2++)
						{
							value[num2] = array3[num2];
						}
						for (int num3 = 0; num3 < array4.Length; num3++)
						{
							value[array3.Length + num3] = (array4[num3].Item1 | num, array4[num3].Item2 | num);
						}
					}
				}
			}
			_rangeCache[set] = value;
		}
		return value;
	}
}
