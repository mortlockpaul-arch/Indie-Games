namespace System.Net.Http.HPack;

internal sealed class DynamicTable
{
	private HeaderField[] _buffer;

	private int _maxSize;

	private int _size;

	private int _count;

	private int _insertIndex;

	private int _removeIndex;

	public ref readonly HeaderField this[int index]
	{
		get
		{
			if (index >= _count)
			{
				throw new IndexOutOfRangeException();
			}
			index = _insertIndex - index - 1;
			if (index < 0)
			{
				index += _buffer.Length;
			}
			return ref _buffer[index];
		}
	}

	public DynamicTable(int maxSize)
	{
		_buffer = Array.Empty<HeaderField>();
		_maxSize = maxSize;
	}

	public void Insert(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
	{
		Insert(null, name, value);
	}

	public void Insert(int? staticTableIndex, ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
	{
		int length = HeaderField.GetLength(name.Length, value.Length);
		EnsureAvailable(length);
		if (length <= _maxSize)
		{
			if (_count == _buffer.Length)
			{
				int val = _maxSize / 32;
				HeaderField[] array = new HeaderField[Math.Min(Math.Max(16, _buffer.Length * 2), val)];
				int num = Math.Min(_buffer.Length - _removeIndex, _count);
				int length2 = _count - num;
				Array.Copy(_buffer, _removeIndex, array, 0, num);
				Array.Copy(_buffer, 0, array, num, length2);
				_buffer = array;
				_removeIndex = 0;
				_insertIndex = _count;
			}
			HeaderField headerField = new HeaderField(staticTableIndex, name, value);
			_buffer[_insertIndex] = headerField;
			if (++_insertIndex == _buffer.Length)
			{
				_insertIndex = 0;
			}
			_size += headerField.Length;
			_count++;
		}
	}

	public void UpdateMaxSize(int maxSize)
	{
		int maxSize2 = _maxSize;
		_maxSize = maxSize;
		if (maxSize < maxSize2)
		{
			EnsureAvailable(0);
		}
	}

	private void EnsureAvailable(int available)
	{
		while (_count > 0 && _maxSize - _size < available)
		{
			ref HeaderField reference = ref _buffer[_removeIndex];
			_size -= reference.Length;
			reference = default(HeaderField);
			_count--;
			if (++_removeIndex == _buffer.Length)
			{
				_removeIndex = 0;
			}
		}
	}
}
