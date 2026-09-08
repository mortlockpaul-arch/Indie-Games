using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace _6;

internal class _0018 : IEnumerator
{
	private IEnumerator _3A_0018;

	public object Current => ((KeyValuePair<string, SerializationEntry>)_3A_0018.Current).Value;

	public _0018(IEnumerator items)
	{
		_3A_0018 = items;
	}

	public bool MoveNext()
	{
		return _3A_0018.MoveNext();
	}

	public void Reset()
	{
		_3A_0018.Reset();
	}
}
