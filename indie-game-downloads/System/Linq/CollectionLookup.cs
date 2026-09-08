using System.Collections;
using System.Collections.Generic;

namespace System.Linq;

internal sealed class CollectionLookup<TKey, TElement> : Lookup<TKey, TElement>, ICollection<IGrouping<TKey, TElement>>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable, IReadOnlyCollection<IGrouping<TKey, TElement>>
{
	bool ICollection<IGrouping<TKey, TElement>>.IsReadOnly => true;

	internal CollectionLookup(IEqualityComparer<TKey> comparer)
		: base(comparer)
	{
	}

	void ICollection<IGrouping<TKey, TElement>>.CopyTo(IGrouping<TKey, TElement>[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, "arrayIndex");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length, "arrayIndex");
		ArgumentOutOfRangeException.ThrowIfLessThan(array.Length - arrayIndex, base.Count, "arrayIndex");
		Grouping<TKey, TElement> grouping = _lastGrouping;
		if (grouping != null)
		{
			do
			{
				grouping = (Grouping<TKey, TElement>)(array[arrayIndex] = grouping._next);
				arrayIndex++;
			}
			while (grouping != _lastGrouping);
		}
	}

	bool ICollection<IGrouping<TKey, TElement>>.Contains(IGrouping<TKey, TElement> item)
	{
		ArgumentNullException.ThrowIfNull(item, "item");
		Grouping<TKey, TElement> grouping = GetGrouping(item.Key, create: false);
		if (grouping != null)
		{
			return grouping == item;
		}
		return false;
	}

	void ICollection<IGrouping<TKey, TElement>>.Add(IGrouping<TKey, TElement> item)
	{
		throw new NotSupportedException();
	}

	void ICollection<IGrouping<TKey, TElement>>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<IGrouping<TKey, TElement>>.Remove(IGrouping<TKey, TElement> item)
	{
		throw new NotSupportedException();
	}
}
