using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq;

[DebuggerDisplay("Count = 0")]
[DebuggerTypeProxy(typeof(SystemLinq_LookupDebugView<, >))]
internal sealed class EmptyLookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable, ICollection<IGrouping<TKey, TElement>>, IReadOnlyCollection<IGrouping<TKey, TElement>>
{
	public static readonly EmptyLookup<TKey, TElement> Instance = new EmptyLookup<TKey, TElement>();

	public IEnumerable<TElement> this[TKey key] => Array.Empty<TElement>();

	public int Count => 0;

	public bool IsReadOnly => true;

	public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
	{
		return Enumerable.Empty<IGrouping<TKey, TElement>>().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Contains(TKey key)
	{
		return false;
	}

	public bool Contains(IGrouping<TKey, TElement> item)
	{
		return false;
	}

	public void CopyTo(IGrouping<TKey, TElement>[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, "arrayIndex");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length, "arrayIndex");
	}

	public void Add(IGrouping<TKey, TElement> item)
	{
		throw new NotSupportedException();
	}

	public void Clear()
	{
		throw new NotSupportedException();
	}

	public bool Remove(IGrouping<TKey, TElement> item)
	{
		throw new NotSupportedException();
	}
}
