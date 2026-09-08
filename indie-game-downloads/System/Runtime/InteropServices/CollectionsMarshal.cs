using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

public static class CollectionsMarshal
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> AsSpan<T>(List<T>? list)
	{
		Span<T> result = default(Span<T>);
		if (list != null)
		{
			int size = list._size;
			T[] items = list._items;
			if ((uint)size > (uint)items.Length)
			{
				ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
			}
			return new Span<T>(ref MemoryMarshal.GetArrayDataReference(items), size);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> AsBytes(BitArray? array)
	{
		if (array != null)
		{
			return array._array.AsSpan(0, BitArray.GetByteArrayLengthFromBitLength(array.Length));
		}
		return default(Span<byte>);
	}

	public static ref TValue GetValueRefOrNullRef<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
	{
		return ref dictionary.FindValue(key);
	}

	public static ref TValue GetValueRefOrNullRef<TKey, TValue, TAlternateKey>(Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> dictionary, TAlternateKey key) where TKey : notnull where TAlternateKey : notnull, allows ref struct
	{
		TKey actualKey;
		return ref dictionary.FindValue(key, out actualKey);
	}

	public static ref TValue? GetValueRefOrAddDefault<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, out bool exists) where TKey : notnull
	{
		return ref Dictionary<TKey, TValue>.CollectionsMarshalHelper.GetValueRefOrAddDefault(dictionary, key, out exists);
	}

	public static ref TValue? GetValueRefOrAddDefault<TKey, TValue, TAlternateKey>(Dictionary<TKey, TValue>.AlternateLookup<TAlternateKey> dictionary, TAlternateKey key, out bool exists) where TKey : notnull where TAlternateKey : notnull, allows ref struct
	{
		return ref dictionary.GetValueRefOrAddDefault(key, out exists);
	}

	public static void SetCount<T>(List<T> list, int count)
	{
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException_NeedNonNegNum("count");
		}
		list._version++;
		if (count > list.Capacity)
		{
			list.Grow(count);
		}
		else if (count < list._size && RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			Array.Clear(list._items, count, list._size - count);
		}
		list._size = count;
	}
}
