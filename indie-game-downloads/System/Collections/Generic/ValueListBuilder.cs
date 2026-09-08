using System.Buffers;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

internal ref struct ValueListBuilder<T>(Span<T> scratchBuffer)
{
	private Span<T> _span = scratchBuffer;

	private T[] _arrayFromPool = null;

	private int _pos = 0;

	public int Length
	{
		get
		{
			return _pos;
		}
		set
		{
			_pos = value;
		}
	}

	public ref T this[int index] => ref _span[index];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(T item)
	{
		int pos = _pos;
		Span<T> span = _span;
		if ((uint)pos < (uint)span.Length)
		{
			span[pos] = item;
			_pos = pos + 1;
		}
		else
		{
			AddWithResize(item);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void AddWithResize(T item)
	{
		int pos = _pos;
		Grow();
		_span[pos] = item;
		_pos = pos + 1;
	}

	public ReadOnlySpan<T> AsSpan()
	{
		return _span.Slice(0, _pos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		T[] arrayFromPool = _arrayFromPool;
		if (arrayFromPool != null)
		{
			_arrayFromPool = null;
			if (!typeof(T).IsPrimitive)
			{
				Array.Clear(arrayFromPool, 0, _pos);
			}
			ArrayPool<T>.Shared.Return(arrayFromPool);
		}
	}

	private void Grow(int additionalCapacityRequired = 1)
	{
		int num = Math.Max((_span.Length != 0) ? (_span.Length * 2) : 4, _span.Length + additionalCapacityRequired);
		if ((uint)num > 2147483591u)
		{
			num = Math.Max(Math.Max(_span.Length + 1, 2147483591), _span.Length);
		}
		T[] array = ArrayPool<T>.Shared.Rent(num);
		_span.CopyTo(array);
		T[] arrayFromPool = _arrayFromPool;
		_span = (_arrayFromPool = array);
		if (arrayFromPool != null)
		{
			if (!typeof(T).IsPrimitive)
			{
				Array.Clear(arrayFromPool, 0, _pos);
			}
			ArrayPool<T>.Shared.Return(arrayFromPool);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Pop()
	{
		_pos--;
		return _span[_pos];
	}
}
