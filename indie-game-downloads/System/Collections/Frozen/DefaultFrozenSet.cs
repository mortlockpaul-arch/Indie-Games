using System.Collections.Generic;

namespace System.Collections.Frozen;

internal sealed class DefaultFrozenSet<T> : ItemsFrozenSet<T, DefaultFrozenSet<T>.GSW>
{
	internal struct GSW : IGenericSpecializedWrapper
	{
		private DefaultFrozenSet<T> _set;

		public int Count => _set.Count;

		public IEqualityComparer<T> Comparer => _set.Comparer;

		public void Store(FrozenSet<T> set)
		{
			_set = (DefaultFrozenSet<T>)set;
		}

		public int FindItemIndex(T item)
		{
			return _set.FindItemIndex(item);
		}

		public Enumerator GetEnumerator()
		{
			return _set.GetEnumerator();
		}
	}

	private static class AlternateLookupDelegateHolder<TAlternate> where TAlternate : allows ref struct
	{
		public static readonly AlternateLookupDelegate<TAlternate> Instance = (FrozenSet<T> set, TAlternate item) => ((DefaultFrozenSet<T>)set).FindItemIndexAlternate(item);
	}

	internal DefaultFrozenSet(HashSet<T> source)
		: base(source, false)
	{
	}

	private protected override int FindItemIndex(T item)
	{
		IEqualityComparer<T> comparer = base.Comparer;
		int num = ((item != null) ? comparer.GetHashCode(item) : 0);
		_hashTable.FindMatchingEntries(num, out var i, out var endIndex);
		for (; i <= endIndex; i++)
		{
			if (num == _hashTable.HashCodes[i] && comparer.Equals(item, _items[i]))
			{
				return i;
			}
		}
		return -1;
	}

	private protected override AlternateLookupDelegate<TAlternate> GetAlternateLookupDelegate<TAlternate>()
	{
		return AlternateLookupDelegateHolder<TAlternate>.Instance;
	}

	private int FindItemIndexAlternate<TAlternate>(TAlternate item) where TAlternate : allows ref struct
	{
		IAlternateEqualityComparer<TAlternate, T> alternateEqualityComparer = GetAlternateEqualityComparer<TAlternate>();
		int num = ((item != null) ? alternateEqualityComparer.GetHashCode(item) : 0);
		_hashTable.FindMatchingEntries(num, out var i, out var endIndex);
		for (; i <= endIndex; i++)
		{
			if (num == _hashTable.HashCodes[i] && alternateEqualityComparer.Equals(item, _items[i]))
			{
				return i;
			}
		}
		return -1;
	}
}
