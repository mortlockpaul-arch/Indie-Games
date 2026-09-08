using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Threading;

namespace System.Runtime.CompilerServices;

internal struct GenericCache<TKey, TValue> where TKey : struct, IEquatable<TKey>
{
	private struct Entry
	{
		internal EntryInfo _info;

		internal TKey _key;

		internal TValue _value;

		[UnscopedRef]
		public ref uint Version => ref _info._version;
	}

	private Entry[] _table;

	private Entry[] _sentinelTable;

	private int _lastFlushSize;

	private int _initialCacheSize;

	private int _maxCacheSize;

	public GenericCache(int initialCacheSize, int maxCacheSize)
	{
		_table = null;
		_sentinelTable = null;
		_lastFlushSize = 0;
		_initialCacheSize = initialCacheSize;
		_maxCacheSize = maxCacheSize;
		_sentinelTable = CreateCacheTable(2, throwOnFail: true);
		_table = CreateCacheTable(initialCacheSize) ?? _sentinelTable;
		_lastFlushSize = initialCacheSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int HashToBucket(Entry[] table, int hash)
	{
		byte b = HashShift(table);
		return (int)(hash * -7046029254386353131L >>> (int)b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ref Entry TableData(Entry[] table)
	{
		return ref Unsafe.As<byte, Entry>(ref Unsafe.As<RawArrayData>(table).Data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ref byte HashShift(Entry[] table)
	{
		return ref TableData(table)._info.hashShift;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ref byte VictimCounter(Entry[] table)
	{
		return ref TableData(table)._info.victimCounter;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int TableMask(Entry[] table)
	{
		return table.Length - 2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ref Entry Element(Entry[] table, int index)
	{
		return ref Unsafe.Add(ref Unsafe.As<byte, Entry>(ref Unsafe.As<RawArrayData>(table).Data), index + 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal bool TryGet(TKey key, out TValue value)
	{
		Entry[] table = _table;
		int hashCode = key.GetHashCode();
		int num = HashToBucket(table, hashCode);
		int num2 = 0;
		while (num2 < 8)
		{
			ref Entry reference = ref Element(table, num);
			uint num3 = Volatile.Read(in reference.Version);
			if (key.Equals(reference._key))
			{
				value = reference._value;
				Volatile.ReadBarrier();
				num3 &= 0xFFFFFFFEu;
				if (num3 != reference.Version)
				{
					break;
				}
				return true;
			}
			if (num3 == 0)
			{
				break;
			}
			num2++;
			num = (num + num2) & TableMask(table);
		}
		value = default(TValue);
		return false;
	}

	private Entry[] CreateCacheTable(int size, bool throwOnFail = false)
	{
		Entry[] array = null;
		try
		{
			array = new Entry[size + 1];
		}
		catch (OutOfMemoryException) when (!throwOnFail)
		{
		}
		if (array == null)
		{
			size = _initialCacheSize;
			try
			{
				array = new Entry[size + 1];
			}
			catch (OutOfMemoryException)
			{
			}
		}
		if (array == null)
		{
			return array;
		}
		TableData(array);
		byte b = (byte)BitOperations.LeadingZeroCount((nuint)(size - 1));
		HashShift(array) = b;
		return array;
	}

	internal void TrySet(TKey key, TValue value)
	{
		int hashCode = key.GetHashCode();
		int num;
		Entry[] table;
		do
		{
			table = _table;
			if (table.Length == 2)
			{
				MaybeReplaceCacheWithLarger(_lastFlushSize);
				return;
			}
			num = HashToBucket(table, hashCode);
			int num2 = num;
			ref Entry reference = ref Element(table, num2);
			int num3 = 0;
			while (num3 < 8)
			{
				uint version = reference.Version;
				version &= 0xFFFFFFFEu;
				if ((version & 0x1FFFFFFF) >= 536870909)
				{
					FlushCurrentCache();
					return;
				}
				if (version == 0 || version >> 29 > num3)
				{
					uint num4 = (uint)((num3 << 29) + (int)(version & 0x1FFFFFFF) + 1);
					if (Interlocked.CompareExchange(ref reference.Version, num4, version) == version)
					{
						reference._key = key;
						reference._value = value;
						Volatile.Write(ref reference.Version, num4 + 1);
						return;
					}
				}
				if (key.Equals(reference._key))
				{
					return;
				}
				num3++;
				num2 += num3;
				reference = ref Element(table, num2 & TableMask(table));
			}
		}
		while (TryGrow(table));
		table = _table;
		if (table.Length == 2)
		{
			return;
		}
		byte b = (byte)(VictimCounter(table)++ & 7);
		int num5 = (b * b + b) / 2;
		ref Entry reference2 = ref Element(table, (num + num5) & TableMask(table));
		uint version2 = reference2.Version;
		version2 &= 0xFFFFFFFEu;
		if ((version2 & 0x1FFFFFFF) >= 536870909)
		{
			FlushCurrentCache();
			return;
		}
		uint num6 = (uint)((b << 29) + (version2 & 0x1FFFFFFF) + 1);
		if (Interlocked.CompareExchange(ref reference2.Version, num6, version2) == version2)
		{
			reference2._key = key;
			reference2._value = value;
			Volatile.Write(ref reference2.Version, num6 + 1);
		}
	}

	private static int CacheElementCount(Entry[] table)
	{
		return table.Length - 1;
	}

	private void FlushCurrentCache()
	{
		int num = CacheElementCount(_table);
		if (num < _initialCacheSize)
		{
			num = _initialCacheSize;
		}
		_lastFlushSize = num;
		_table = _sentinelTable;
	}

	private bool MaybeReplaceCacheWithLarger(int size)
	{
		Entry[] array = CreateCacheTable(size);
		if (array == null)
		{
			return false;
		}
		_table = array;
		return true;
	}

	private bool TryGrow(Entry[] table)
	{
		int num = CacheElementCount(table) * 2;
		if (num <= _maxCacheSize)
		{
			return MaybeReplaceCacheWithLarger(num);
		}
		return false;
	}
}
