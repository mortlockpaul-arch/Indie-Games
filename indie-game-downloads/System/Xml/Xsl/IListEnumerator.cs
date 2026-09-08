using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Xsl;

internal struct IListEnumerator<T>(IList<T> sequence) : IEnumerator<T>, IEnumerator, IDisposable
{
	private readonly IList<T> _sequence = sequence;

	private int _index = 0;

	private T _current = default(T);

	public T Current => _current;

	object IEnumerator.Current
	{
		get
		{
			if (_index == 0)
			{
				throw new InvalidOperationException(System.SR.Format(System.SR.Sch_EnumNotStarted, string.Empty));
			}
			if (_index > _sequence.Count)
			{
				throw new InvalidOperationException(System.SR.Format(System.SR.Sch_EnumFinished, string.Empty));
			}
			return _current;
		}
	}

	public void Dispose()
	{
	}

	public bool MoveNext()
	{
		if (_index < _sequence.Count)
		{
			_current = _sequence[_index];
			_index++;
			return true;
		}
		_current = default(T);
		return false;
	}

	void IEnumerator.Reset()
	{
		_index = 0;
		_current = default(T);
	}
}
