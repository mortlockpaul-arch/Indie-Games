using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Metrics;

[DefaultMember("Item")]
internal sealed class CircularBufferBuckets
{
	private long[] _trait;

	private int _begin;

	private int _end = -1;

	public int Capacity { get; }

	public int Size => _end - _begin + 1;

	public CircularBufferBuckets(int capacity)
	{
		if (capacity < 1)
		{
			throw new ArgumentOutOfRangeException("capacity", "Capacity must be greater than 0.");
		}
		Capacity = capacity;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int TryIncrement(int index, long value = 1L)
	{
		int capacity = Capacity;
		if (_trait == null)
		{
			_trait = new long[capacity];
			_begin = index;
			_end = index;
			_trait[ModuloIndex(index)] += value;
			return 0;
		}
		int num = _begin;
		int num2 = _end;
		if (index > num2)
		{
			num2 = index;
		}
		else
		{
			if (index >= num)
			{
				_trait[ModuloIndex(index)] += value;
				return 0;
			}
			num = index;
		}
		int num3 = num2 - num;
		if (num3 >= capacity || num3 < 0)
		{
			return CalculateScaleReduction(num, num2, capacity);
		}
		_begin = num;
		_end = num2;
		_trait[ModuloIndex(index)] += value;
		return 0;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int CalculateScaleReduction(int begin, int end, int num6)
		{
			int num4 = 0;
			int num5 = end - begin;
			while (num5 >= num6 || num5 < 0)
			{
				begin >>= 1;
				end >>= 1;
				num5 = end - begin;
				num4++;
			}
			return num4;
		}
	}

	public void ScaleDown(int level = 1)
	{
		if (_trait == null)
		{
			return;
		}
		uint capacity = (uint)Capacity;
		uint num = (uint)ModuloIndex(_begin);
		int num2 = _begin;
		int num3 = _end;
		for (int i = 0; i < level; i++)
		{
			int num4 = num2 >> 1;
			int num5 = num3 >> 1;
			if (num2 != num3)
			{
				if (num2 % 2 == 0)
				{
					ScaleDownInternal(_trait, num, num2, num3, capacity);
				}
				else
				{
					num2++;
					if (num2 != num3)
					{
						ScaleDownInternal(_trait, num + 1, num2, num3, capacity);
					}
				}
			}
			num2 = num4;
			num3 = num5;
		}
		_begin = num2;
		_end = num3;
		if (capacity > 1)
		{
			AdjustPosition(_trait, num, (uint)ModuloIndex(num2), (uint)(num3 - num2 + 1), capacity);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void AdjustPosition(long[] array, uint src, uint dst, uint size, uint num7)
		{
			uint num6 = (dst + num7 - src) % num7;
			if (num6 != 0)
			{
				if (size - 1 == num6 && num6 << 1 == num7)
				{
					Exchange(array, src++, dst++);
					size -= 2;
				}
				else if (num6 < size)
				{
					src = src + size - 1;
					dst = dst + size - 1;
					while (size-- != 0)
					{
						Move(array, src-- % num7, dst-- % num7);
					}
					return;
				}
				while (size-- != 0)
				{
					Move(array, src++ % num7, dst++ % num7);
				}
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Consolidate(long[] array, uint src, uint dst)
		{
			array[dst] += array[src];
			array[src] = 0L;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Exchange(long[] array, uint src, uint dst)
		{
			long num6 = array[dst];
			array[dst] = array[src];
			array[src] = num6;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void Move(long[] array, uint src, uint dst)
		{
			array[dst] = array[src];
			array[src] = 0L;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void ScaleDownInternal(long[] array, uint offset, int begin, int end, uint num6)
		{
			for (int j = begin + 1; j < end; j++)
			{
				Consolidate(array, (uint)((int)offset + (j - begin)) % num6, (uint)((int)offset + ((j >> 1) - (begin >> 1))) % num6);
			}
			Consolidate(array, (uint)((int)offset + (end - begin)) % num6, (uint)((int)offset + ((end >> 1) - (begin >> 1))) % num6);
		}
	}

	public long[] ToArray()
	{
		int size = Size;
		if (_trait == null || size <= 0)
		{
			return Array.Empty<long>();
		}
		long[] array = new long[size];
		for (int i = 0; i < size; i++)
		{
			array[i] = _trait[ModuloIndex(_begin + i)];
		}
		return array;
	}

	internal void Clear()
	{
		if (_trait != null)
		{
			Array.Clear(_trait);
		}
		_begin = 0;
		_end = -1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int ModuloIndex(int value)
	{
		return PositiveModulo32(value, Capacity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int PositiveModulo32(int value, int divisor)
	{
		value %= divisor;
		if (value < 0)
		{
			value += divisor;
		}
		return value;
	}
}
