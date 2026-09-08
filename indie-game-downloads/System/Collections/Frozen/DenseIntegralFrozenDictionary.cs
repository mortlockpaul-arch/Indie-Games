using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Collections.Frozen;

internal sealed class DenseIntegralFrozenDictionary
{
	[DebuggerTypeProxy(typeof(DebuggerProxy<, , >))]
	private sealed class WithFullValues<TKey, TKeyUnderlying, TValue>(TKey[] keys, TValue[] values) : FrozenDictionary<TKey, TValue>((IEqualityComparer<TKey>)EqualityComparer<TKey>.Default) where TKeyUnderlying : IBinaryInteger<TKeyUnderlying>
	{
		private readonly TKey[] _keys = keys;

		private readonly TValue[] _values = values;

		private protected override TKey[] KeysCore => _keys;

		private protected override TValue[] ValuesCore => _values;

		private protected override int CountCore => _keys.Length;

		private protected override Enumerator GetEnumeratorCore()
		{
			return new Enumerator(_keys, _values);
		}

		private protected override ref readonly TValue GetValueRefOrNullRefCore(TKey key)
		{
			int num = int.CreateTruncating((TKeyUnderlying)(object)key);
			TValue[] values = _values;
			if ((uint)num < (uint)values.Length)
			{
				return ref values[num];
			}
			return ref Unsafe.NullRef<TValue>();
		}
	}

	[DebuggerTypeProxy(typeof(DebuggerProxy<, , >))]
	private sealed class WithOptionalValues<TKey, TKeyUnderlying, TValue>(TKey[] keys, TValue[] values, Optional<TValue>[] optionalValues, int minInclusive) : FrozenDictionary<TKey, TValue>((IEqualityComparer<TKey>)EqualityComparer<TKey>.Default) where TKeyUnderlying : IBinaryInteger<TKeyUnderlying>
	{
		private readonly TKey[] _keys = keys;

		private readonly TValue[] _values = values;

		private readonly Optional<TValue>[] _optionalValues = optionalValues;

		private readonly int _minInclusive = minInclusive;

		private protected override TKey[] KeysCore => _keys;

		private protected override TValue[] ValuesCore => _values;

		private protected override int CountCore => _keys.Length;

		private protected override Enumerator GetEnumeratorCore()
		{
			return new Enumerator(_keys, _values);
		}

		private protected override ref readonly TValue GetValueRefOrNullRefCore(TKey key)
		{
			int num = int.CreateTruncating((TKeyUnderlying)(object)key) - _minInclusive;
			Optional<TValue>[] optionalValues = _optionalValues;
			if ((uint)num < (uint)optionalValues.Length)
			{
				ref Optional<TValue> reference = ref optionalValues[num];
				if (reference.HasValue)
				{
					return ref reference.Value;
				}
			}
			return ref Unsafe.NullRef<TValue>();
		}
	}

	private readonly struct Optional<TValue>(TValue value, bool hasValue)
	{
		public readonly TValue Value = value;

		public readonly bool HasValue = hasValue;
	}

	private sealed class DebuggerProxy<TKey, TKeyUnderlying, TValue> : ImmutableDictionaryDebuggerProxy<TKey, TValue>
	{
		public DebuggerProxy(IReadOnlyDictionary<TKey, TValue> dictionary)
			: base(dictionary)
		{
		}
	}

	public static FrozenDictionary<TKey, TValue> CreateIfValid<TKey, TValue>(Dictionary<TKey, TValue> source)
	{
		if (!(typeof(TKey) == typeof(byte)) && (!typeof(TKey).IsEnum || !(typeof(TKey).GetEnumUnderlyingType() == typeof(byte))))
		{
			if (!(typeof(TKey) == typeof(sbyte)) && (!typeof(TKey).IsEnum || !(typeof(TKey).GetEnumUnderlyingType() == typeof(sbyte))))
			{
				if (!(typeof(TKey) == typeof(ushort)) && (!typeof(TKey).IsEnum || !(typeof(TKey).GetEnumUnderlyingType() == typeof(ushort))))
				{
					if (!(typeof(TKey) == typeof(short)) && (!typeof(TKey).IsEnum || !(typeof(TKey).GetEnumUnderlyingType() == typeof(short))))
					{
						if (!(typeof(TKey) == typeof(char)))
						{
							if (!(typeof(TKey) == typeof(int)) && (!typeof(TKey).IsEnum || !(typeof(TKey).GetEnumUnderlyingType() == typeof(int))))
							{
								return null;
							}
							return CreateIfValid<TKey, int, TValue>(source);
						}
						return CreateIfValid<TKey, char, TValue>(source);
					}
					return CreateIfValid<TKey, short, TValue>(source);
				}
				return CreateIfValid<TKey, ushort, TValue>(source);
			}
			return CreateIfValid<TKey, sbyte, TValue>(source);
		}
		return CreateIfValid<TKey, byte, TValue>(source);
	}

	private static FrozenDictionary<TKey, TValue> CreateIfValid<TKey, TKeyUnderlying, TValue>(Dictionary<TKey, TValue> source) where TKeyUnderlying : unmanaged, IBinaryInteger<TKeyUnderlying>
	{
		int count = source.Count;
		Dictionary<TKey, TValue>.Enumerator enumerator = source.GetEnumerator();
		if (enumerator.MoveNext())
		{
			int num = int.CreateTruncating((TKeyUnderlying)(object)enumerator.Current.Key);
			int num2 = num;
			while (enumerator.MoveNext())
			{
				int num3 = int.CreateTruncating((TKeyUnderlying)(object)enumerator.Current.Key);
				if (num3 < num)
				{
					num = num3;
				}
				else if (num3 > num2)
				{
					num2 = num3;
				}
			}
			long num4 = Math.Min((long)count * 10L, Array.MaxLength);
			long num5 = (long)num2 - (long)num + 1;
			if (num5 <= num4)
			{
				TKey[] array = new TKey[count];
				TValue[] array2 = new TValue[array.Length];
				if (num == 0 && num5 == count)
				{
					foreach (KeyValuePair<TKey, TValue> item in source)
					{
						int num6 = int.CreateTruncating((TKeyUnderlying)(object)item.Key);
						array[num6] = item.Key;
						array2[num6] = item.Value;
					}
					return new WithFullValues<TKey, TKeyUnderlying, TValue>(array, array2);
				}
				Optional<TValue>[] array3 = new Optional<TValue>[num5];
				int num7 = 0;
				foreach (KeyValuePair<TKey, TValue> item2 in source)
				{
					array[num7] = item2.Key;
					array2[num7] = item2.Value;
					num7++;
					array3[int.CreateTruncating((TKeyUnderlying)(object)item2.Key) - num] = new Optional<TValue>(item2.Value, hasValue: true);
				}
				return new WithOptionalValues<TKey, TKeyUnderlying, TValue>(array, array2, array3, num);
			}
		}
		return null;
	}
}
