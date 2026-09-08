using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Collections.Frozen;

internal sealed class DefaultFrozenDictionary<TKey, TValue> : KeysAndValuesFrozenDictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	private static class AlternateLookupDelegateHolder<TAlternateKey> where TAlternateKey : allows ref struct
	{
		public static readonly AlternateLookupDelegate<TAlternateKey> Instance = (FrozenDictionary<TKey, TValue> dictionary, TAlternateKey key) => ref ((DefaultFrozenDictionary<TKey, TValue>)dictionary).GetValueRefOrNullRefCoreAlternate(key);
	}

	internal DefaultFrozenDictionary(Dictionary<TKey, TValue> source)
		: base(source, false)
	{
	}

	private protected override ref readonly TValue GetValueRefOrNullRefCore(TKey key)
	{
		IEqualityComparer<TKey> comparer = base.Comparer;
		int hashCode = comparer.GetHashCode(key);
		_hashTable.FindMatchingEntries(hashCode, out var i, out var endIndex);
		for (; i <= endIndex; i++)
		{
			if (hashCode == _hashTable.HashCodes[i] && comparer.Equals(key, _keys[i]))
			{
				return ref _values[i];
			}
		}
		return ref Unsafe.NullRef<TValue>();
	}

	private protected override AlternateLookupDelegate<TAlternateKey> GetAlternateLookupDelegate<TAlternateKey>()
	{
		return AlternateLookupDelegateHolder<TAlternateKey>.Instance;
	}

	private ref readonly TValue GetValueRefOrNullRefCoreAlternate<TAlternateKey>(TAlternateKey key) where TAlternateKey : allows ref struct
	{
		IAlternateEqualityComparer<TAlternateKey, TKey> alternateEqualityComparer = GetAlternateEqualityComparer<TAlternateKey>();
		int hashCode = alternateEqualityComparer.GetHashCode(key);
		_hashTable.FindMatchingEntries(hashCode, out var i, out var endIndex);
		for (; i <= endIndex; i++)
		{
			if (hashCode == _hashTable.HashCodes[i] && alternateEqualityComparer.Equals(key, _keys[i]))
			{
				return ref _values[i];
			}
		}
		return ref Unsafe.NullRef<TValue>();
	}
}
