using System;
using System.Collections.Generic;

namespace Quasar.Global;

public class FastRandom
{
	private const double REAL_UNIT_INT = 4.656612873077393E-10;

	private const float FLOAT_UNIT_INT = 4.656613E-10f;

	private const double REAL_UNIT_UINT = 2.3283064365386963E-10;

	private long x;

	private uint bitBuffer;

	private uint bitMask = 1u;

	public long CurrentSeed => x;

	public FastRandom()
	{
		Reinitialise(Environment.TickCount);
	}

	public FastRandom(long seed)
	{
		Reinitialise(seed);
	}

	public void Reinitialise(long seed)
	{
		x = seed;
	}

	public int Next()
	{
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		return (int)((x - 1) & 0x7FFFFFFF);
	}

	public int Next(int upperBound)
	{
		if (upperBound <= 0)
		{
			throw new ArgumentOutOfRangeException("upperBound must be >0");
		}
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		return (int)((0x7FFFFFFF & x) % upperBound);
	}

	public int Next(int lowerBound, int upperBound)
	{
		if (lowerBound > upperBound)
		{
			throw new ArgumentOutOfRangeException("upperBound must be >=lowerBound");
		}
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		long num = (long)upperBound - (long)lowerBound;
		return lowerBound + (int)((0x7FFFFFFF & (x - 1)) % num);
	}

	public double NextDouble()
	{
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		return 4.656612873077393E-10 * (double)(int)(0x7FFFFFFF & (x - 1));
	}

	public float NextFloat()
	{
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		return 4.656613E-10f * (float)(int)(0x7FFFFFFF & (x - 1));
	}

	public float NextFloat(float max)
	{
		return NextFloat() * max;
	}

	public float NextFloat(float min, float max)
	{
		return min + NextFloat() * (max - min);
	}

	public void NextBytes(byte[] buffer)
	{
		int num = 0;
		int num2 = buffer.Length - 3;
		while (num < num2)
		{
			x ^= x << 21;
			x ^= x >> 35;
			x ^= x << 4;
			buffer[num++] = (byte)(x - 1);
			buffer[num++] = (byte)(x - 1 >> 8);
			buffer[num++] = (byte)(x - 1 >> 16);
			buffer[num++] = (byte)(x - 1 >> 24);
		}
		if (num >= buffer.Length)
		{
			return;
		}
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		buffer[num++] = (byte)(x - 1);
		if (num >= buffer.Length)
		{
			return;
		}
		buffer[num++] = (byte)(x - 1 >> 8);
		if (num < buffer.Length)
		{
			buffer[num++] = (byte)(x - 1 >> 16);
			if (num < buffer.Length)
			{
				buffer[num] = (byte)(x - 1 >> 24);
			}
		}
	}

	public uint NextUInt()
	{
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		return (uint)((x - 1) & 0xFFFFFFFFu);
	}

	public int NextInt()
	{
		x ^= x << 21;
		x ^= x >> 35;
		x ^= x << 4;
		return (int)(0x7FFFFFFF & (x - 1));
	}

	public bool NextBool()
	{
		if (bitMask == 1)
		{
			bitBuffer = NextUInt();
			bitMask = 2147483648u;
			return (bitBuffer & bitMask) == 0;
		}
		return (bitBuffer & (bitMask >>= 1)) == 0;
	}

	public int NextSign()
	{
		if (bitMask == 1)
		{
			bitBuffer = NextUInt();
			bitMask = 2147483648u;
			if ((bitBuffer & bitMask) != 0)
			{
				return -1;
			}
			return 1;
		}
		if ((bitBuffer & (bitMask >>= 1)) != 0)
		{
			return -1;
		}
		return 1;
	}

	public bool RollDice(float successChance)
	{
		return NextFloat() < successChance;
	}

	public T RandomSelect<T>(List<KeyValuePair<float, T>> list)
	{
		if (list.Count == 1)
		{
			return list[0].Value;
		}
		float num = 0f;
		foreach (KeyValuePair<float, T> item in list)
		{
			num += item.Key;
		}
		float num2 = NextFloat(num);
		num = 0f;
		foreach (KeyValuePair<float, T> item2 in list)
		{
			num += item2.Key;
			if (num2 < num)
			{
				return item2.Value;
			}
		}
		return list[list.Count - 1].Value;
	}

	public void Pick<T>(List<T> list, int count)
	{
		int num = Math.Min(count, list.Count);
		for (int i = 0; i < num; i++)
		{
			int index = Next(list.Count - i) + i;
			T value = list[i];
			list[i] = list[index];
			list[index] = value;
		}
		list.RemoveRange(num, list.Count - num);
	}

	public T PickOne<T>(List<T> list)
	{
		return list[Next(list.Count)];
	}
}
