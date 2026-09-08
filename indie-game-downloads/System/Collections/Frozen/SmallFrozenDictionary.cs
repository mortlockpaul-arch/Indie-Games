using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.Frozen;

internal sealed class SmallFrozenDictionary<TKey, TValue> : FrozenDictionary<TKey, TValue>
{
	private static class AlternateLookupDelegateHolder<TAlternateKey> where TAlternateKey : allows ref struct
	{
		public static readonly AlternateLookupDelegate<TAlternateKey> Instance = (FrozenDictionary<TKey, TValue> dictionary, TAlternateKey key) => ref ((SmallFrozenDictionary<TKey, TValue>)dictionary).GetValueRefOrNullRefCoreAlternate(key);
	}

	private readonly TKey[] _keys;

	private readonly TValue[] _values;

	private protected override TKey[] KeysCore => _keys;

	private protected override TValue[] ValuesCore => _values;

	private protected override int CountCore => _keys.Length;

	internal SmallFrozenDictionary(Dictionary<TKey, TValue> source)
		: base(source.Comparer)
	{
		_keys = source.Keys.ToArray();
		_values = source.Values.ToArray();
	}

	private protected sealed override Enumerator GetEnumeratorCore()
	{
		return new Enumerator(_keys, _values);
	}

	private protected override ref readonly TValue GetValueRefOrNullRefCore(TKey key)
	{
		IEqualityComparer<TKey> comparer = base.Comparer;
		TKey[] keys = _keys;
		for (int i = 0; i < keys.Length; i++)
		{
			if (comparer.Equals(keys[i], key))
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
		TKey[] keys = _keys;
		for (int i = 0; i < keys.Length; i++)
		{
			if (alternateEqualityComparer.Equals(key, keys[i]))
			{
				return ref _values[i];
			}
		}
		return ref Unsafe.NullRef<TValue>();
	}
}
