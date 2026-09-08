using System.Collections;
using System.Collections.Generic;

namespace System.Linq;

internal sealed class PartialArrayEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
	private readonly T[] _array;

	private readonly int _count;

	private int _index = -1;

	public T Current => _array[_index];

	object IEnumerator.Current => Current;

	public PartialArrayEnumerator(T[] array, int count)
	{
		_array = array;
		_count = count;
	}

	public bool MoveNext()
	{
		if (_index + 1 < _count)
		{
			_index++;
			return true;
		}
		return false;
	}

	public void Dispose()
	{
	}

	public void Reset()
	{
		_index = -1;
	}
}
