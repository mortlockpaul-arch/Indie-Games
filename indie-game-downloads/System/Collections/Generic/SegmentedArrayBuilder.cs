using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Collections.Generic;

internal ref struct SegmentedArrayBuilder<T>
{
	[InlineArray(27)]
	private struct Arrays
	{
		private T[] _values;
	}

	[InlineArray(8)]
	public struct ScratchBuffer
	{
		private T _item;
	}

	private Arrays _segments;

	private Span<T> _firstSegment;

	private Span<T> _currentSegment;

	private int _segmentsCount;

	private int _countInFinishedSegments;

	private int _countInCurrentSegment;

	public readonly int Count => checked(_countInFinishedSegments + _countInCurrentSegment);

	public SegmentedArrayBuilder(Span<T> scratchBuffer)
	{
		_segments = default(Arrays);
		_segmentsCount = 0;
		_countInFinishedSegments = 0;
		_countInCurrentSegment = 0;
		_currentSegment = (_firstSegment = scratchBuffer);
	}

	public void Dispose()
	{
		int segmentsCount = _segmentsCount;
		if (segmentsCount != 0)
		{
			ReturnArrays(segmentsCount);
		}
	}

	private void ReturnArrays(int segmentsCount)
	{
		ReadOnlySpan<T[]> readOnlySpan = _segments;
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			segmentsCount--;
			ReadOnlySpan<T[]> readOnlySpan2 = readOnlySpan.Slice(0, segmentsCount);
			for (int i = 0; i < readOnlySpan2.Length; i++)
			{
				T[] array = readOnlySpan2[i];
				Array.Clear(array);
				ArrayPool<T>.Shared.Return(array);
			}
			T[] array2 = readOnlySpan[segmentsCount];
			Array.Clear(array2, 0, _countInCurrentSegment);
			ArrayPool<T>.Shared.Return(array2);
			return;
		}
		for (int j = 0; j < readOnlySpan.Length; j++)
		{
			T[] array3 = readOnlySpan[j];
			if (array3 != null)
			{
				ArrayPool<T>.Shared.Return(array3);
				continue;
			}
			break;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T item)
	{
		Span<T> currentSegment = _currentSegment;
		int countInCurrentSegment = _countInCurrentSegment;
		if ((uint)countInCurrentSegment < (uint)currentSegment.Length)
		{
			currentSegment[countInCurrentSegment] = item;
			_countInCurrentSegment++;
		}
		else
		{
			AddSlow(item);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void AddSlow(T item)
	{
		Expand();
		_currentSegment[0] = item;
		_countInCurrentSegment = 1;
	}

	public void AddRange(IEnumerable<T> source)
	{
		if (source is ICollection<T> { Count: var count } collection)
		{
			if (count == 0)
			{
				return;
			}
			if (source.TryGetSpan(out var span))
			{
				int val = _currentSegment.Length - _countInCurrentSegment;
				ReadOnlySpan<T> readOnlySpan = span.Slice(0, Math.Min(val, span.Length));
				readOnlySpan.CopyTo(_currentSegment.Slice(_countInCurrentSegment));
				_countInCurrentSegment += readOnlySpan.Length;
				readOnlySpan = span.Slice(readOnlySpan.Length);
				if (!readOnlySpan.IsEmpty)
				{
					Expand(readOnlySpan.Length);
					readOnlySpan.CopyTo(_currentSegment);
					_countInCurrentSegment = readOnlySpan.Length;
				}
				return;
			}
			if (_segmentsCount != 0 || _countInCurrentSegment >= _currentSegment.Length)
			{
				int num = _currentSegment.Length - _countInCurrentSegment;
				if (num == 0)
				{
					Expand(count);
					collection.CopyTo(_segments[_segmentsCount - 1], 0);
					_countInCurrentSegment = count;
					return;
				}
				if (count <= num)
				{
					collection.CopyTo(_segments[_segmentsCount - 1], _countInCurrentSegment);
					_countInCurrentSegment += count;
					return;
				}
			}
		}
		AddNonICollectionRangeInlined(source);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public void AddNonICollectionRange(IEnumerable<T> source)
	{
		AddNonICollectionRangeInlined(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void AddNonICollectionRangeInlined(IEnumerable<T> source)
	{
		Span<T> currentSegment = _currentSegment;
		int num = _countInCurrentSegment;
		foreach (T item in source)
		{
			if ((uint)num < (uint)currentSegment.Length)
			{
				currentSegment[num] = item;
				num++;
				continue;
			}
			Expand();
			currentSegment = _currentSegment;
			currentSegment[0] = item;
			num = 1;
		}
		_countInCurrentSegment = num;
	}

	public readonly T[] ToArray()
	{
		int count = Count;
		T[] array;
		if (count != 0)
		{
			array = GC.AllocateUninitializedArray<T>(count);
			ToSpanInlined(array);
		}
		else
		{
			array = Array.Empty<T>();
		}
		return array;
	}

	public readonly List<T> ToList()
	{
		int count = Count;
		List<T> list;
		if (count != 0)
		{
			list = new List<T>(count);
			CollectionsMarshal.SetCount(list, count);
			ToSpanInlined(CollectionsMarshal.AsSpan(list));
		}
		else
		{
			list = new List<T>();
		}
		return list;
	}

	public readonly T[] ToArray(int additionalLength)
	{
		int num = checked(Count + additionalLength);
		T[] array;
		if (num != 0)
		{
			array = GC.AllocateUninitializedArray<T>(num);
			ToSpanInlined(array);
		}
		else
		{
			array = Array.Empty<T>();
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public readonly void ToSpan(Span<T> destination)
	{
		ToSpanInlined(destination);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private readonly void ToSpanInlined(Span<T> destination)
	{
		int segmentsCount = _segmentsCount;
		if (segmentsCount != 0)
		{
			ReadOnlySpan<T> readOnlySpan = _firstSegment;
			readOnlySpan.CopyTo(destination);
			destination = destination.Slice(readOnlySpan.Length);
			segmentsCount--;
			if (segmentsCount != 0)
			{
				ReadOnlySpan<T[]> readOnlySpan2 = ((ReadOnlySpan<T[]>)_segments).Slice(0, segmentsCount);
				for (int i = 0; i < readOnlySpan2.Length; i++)
				{
					ReadOnlySpan<T> readOnlySpan3 = readOnlySpan2[i];
					readOnlySpan3.CopyTo(destination);
					destination = destination.Slice(readOnlySpan3.Length);
				}
			}
		}
		_currentSegment.Slice(0, _countInCurrentSegment).CopyTo(destination);
	}

	private void Expand(int minimumRequired = 16)
	{
		if (minimumRequired < 16)
		{
			minimumRequired = 16;
		}
		int length = _currentSegment.Length;
		checked
		{
			_countInFinishedSegments += length;
			if (_countInFinishedSegments > Array.MaxLength)
			{
				throw new OutOfMemoryException();
			}
		}
		int minimumLength = (int)Math.Min(Math.Max(minimumRequired, (long)length * 2L), Array.MaxLength);
		_currentSegment = (_segments[_segmentsCount] = ArrayPool<T>.Shared.Rent(minimumLength));
		_segmentsCount++;
	}
}
