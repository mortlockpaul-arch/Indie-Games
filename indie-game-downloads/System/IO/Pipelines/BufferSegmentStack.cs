using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines;

internal struct BufferSegmentStack(int size)
{
	private readonly struct SegmentAsValueType
	{
		private readonly BufferSegment _value;

		private SegmentAsValueType(BufferSegment value)
		{
			_value = value;
		}

		public static implicit operator SegmentAsValueType(BufferSegment s)
		{
			return new SegmentAsValueType(s);
		}

		public static implicit operator BufferSegment(SegmentAsValueType s)
		{
			return s._value;
		}
	}

	private SegmentAsValueType[] _array = new SegmentAsValueType[size];

	private int _size = 0;

	public int Count => _size;

	public bool TryPop([NotNullWhen(true)] out BufferSegment? result)
	{
		int num = _size - 1;
		SegmentAsValueType[] array = _array;
		if ((uint)num >= (uint)array.Length)
		{
			result = null;
			return false;
		}
		_size = num;
		result = array[num];
		array[num] = default(SegmentAsValueType);
		return true;
	}

	public void Push(BufferSegment item)
	{
		int size = _size;
		SegmentAsValueType[] array = _array;
		if ((uint)size < (uint)array.Length)
		{
			array[size] = item;
			_size = size + 1;
		}
		else
		{
			PushWithResize(item);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void PushWithResize(BufferSegment item)
	{
		Array.Resize(ref _array, 2 * _array.Length);
		_array[_size] = item;
		_size++;
	}
}
