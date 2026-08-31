using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace _7;

internal class B : IEnumerator
{
	private IEnumerator HCB;

	public object Current => ((KeyValuePair<string, SerializationEntry>)HCB.Current).Value;

	public B(IEnumerator items)
	{
		HCB = items;
	}

	public bool MoveNext()
	{
		return HCB.MoveNext();
	}

	public void Reset()
	{
		HCB.Reset();
	}
}
