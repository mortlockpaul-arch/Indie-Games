using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Xml;

public abstract class XmlNodeList : IEnumerable
{
	public abstract int Count { get; }

	[IndexerName("ItemOf")]
	public virtual XmlNode this[int i]
	{
		get
		{
			/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
		}
	}

	public abstract XmlNode Item(int index);

	public abstract IEnumerator GetEnumerator();
}
