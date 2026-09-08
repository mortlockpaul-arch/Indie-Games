using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Collections.Frozen;

internal sealed class Int32FrozenDictionary<TValue> : FrozenDictionary<int, TValue>
{
	private static class AlternateLookupDelegateHolder<TAlternateKey> where TAlternateKey : allows ref struct
	{
		public static readonly AlternateLookupDelegate<TAlternateKey> Instance = (FrozenDictionary<int, TValue> dictionary, TAlternateKey key) => ref ((Int32FrozenDictionary<TValue>)dictionary).GetValueRefOrNullRefCoreAlternate(key);
	}

	private readonly FrozenHashTable _hashTable;

	private readonly TValue[] _values;

	private protected override int[] KeysCore => _hashTable.HashCodes;

	private protected override TValue[] ValuesCore => _values;

	private protected override int CountCore => _hashTable.Count;

	internal Int32FrozenDictionary(Dictionary<int, TValue> source)
		: base((IEqualityComparer<int>)EqualityComparer<int>.Default)
	{
		KeyValuePair<int, TValue>[] array = new KeyValuePair<int, TValue>[source.Count];
		((ICollection<KeyValuePair<int, TValue>>)source).CopyTo(array, 0);
		_values = new TValue[array.Length];
		int[] array2 = ArrayPool<int>.Shared.Rent(array.Length);
		Span<int> hashCodes = array2.AsSpan(0, array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			hashCodes[i] = array[i].Key;
		}
		_hashTable = FrozenHashTable.Create(hashCodes, hashCodesAreUnique: true);
		for (int j = 0; j < hashCodes.Length; j++)
		{
			int num = hashCodes[j];
			_values[num] = array[j].Value;
		}
		ArrayPool<int>.Shared.Return(array2);
	}

	private protected override Enumerator GetEnumeratorCore()
	{
		return new Enumerator(_hashTable.HashCodes, _values);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected override ref readonly TValue GetValueRefOrNullRefCore(int key)
	{
		_hashTable.FindMatchingEntries(key, out var i, out var endIndex);
		int[] hashCodes = _hashTable.HashCodes;
		for (; i <= endIndex; i++)
		{
			if (key == hashCodes[i])
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
		IAlternateEqualityComparer<TAlternateKey, int> alternateEqualityComparer = GetAlternateEqualityComparer<TAlternateKey>();
		_hashTable.FindMatchingEntries(alternateEqualityComparer.GetHashCode(key), out var i, out var endIndex);
		int[] hashCodes = _hashTable.HashCodes;
		for (; i <= endIndex; i++)
		{
			if (alternateEqualityComparer.Equals(key, hashCodes[i]))
			{
				return ref _values[i];
			}
		}
		return ref Unsafe.NullRef<TValue>();
	}
}
