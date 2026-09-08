using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Collections.Generic;

[DebuggerTypeProxy(typeof(System.Collections.Generic.IDictionaryDebugView<, >))]
[DebuggerDisplay("Count = {Count}")]
public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IList<KeyValuePair<TKey, TValue>>, IReadOnlyList<KeyValuePair<TKey, TValue>>, IList where TKey : notnull
{
	private struct Entry
	{
		public int Next;

		public uint HashCode;

		public TKey Key;

		public TValue Value;
	}

	[StructLayout(LayoutKind.Auto)]
	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable, IDictionaryEnumerator
	{
		private readonly OrderedDictionary<TKey, TValue> _dictionary;

		private readonly int _version;

		private readonly bool _useDictionaryEntry;

		private int _index;

		public KeyValuePair<TKey, TValue> Current { get; private set; }

		readonly object IEnumerator.Current
		{
			get
			{
				if (!_useDictionaryEntry)
				{
					return Current;
				}
				return new DictionaryEntry(Current.Key, Current.Value);
			}
		}

		readonly DictionaryEntry IDictionaryEnumerator.Entry => new DictionaryEntry(Current.Key, Current.Value);

		readonly object IDictionaryEnumerator.Key => Current.Key;

		readonly object? IDictionaryEnumerator.Value => Current.Value;

		internal Enumerator(OrderedDictionary<TKey, TValue> dictionary, bool useDictionaryEntry)
		{
			_index = 0;
			Current = default(KeyValuePair<TKey, TValue>);
			_dictionary = dictionary;
			_version = _dictionary._version;
			_useDictionaryEntry = useDictionaryEntry;
		}

		public bool MoveNext()
		{
			OrderedDictionary<TKey, TValue> dictionary = _dictionary;
			if (_version != dictionary._version)
			{
				ThrowHelper.ThrowVersionCheckFailed();
			}
			if (_index < dictionary._count)
			{
				ref Entry reference = ref dictionary._entries[_index];
				Current = new KeyValuePair<TKey, TValue>(reference.Key, reference.Value);
				_index++;
				return true;
			}
			Current = default(KeyValuePair<TKey, TValue>);
			return false;
		}

		void IEnumerator.Reset()
		{
			if (_version != _dictionary._version)
			{
				ThrowHelper.ThrowVersionCheckFailed();
			}
			_index = 0;
			Current = default(KeyValuePair<TKey, TValue>);
		}

		readonly void IDisposable.Dispose()
		{
		}
	}

	[DebuggerTypeProxy(typeof(System.Collections.Generic.ICollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	public sealed class KeyCollection : IList<TKey>, ICollection<TKey>, IEnumerable<TKey>, IEnumerable, IReadOnlyList<TKey>, IReadOnlyCollection<TKey>, IList, ICollection
	{
		public struct Enumerator : IEnumerator<TKey>, IEnumerator, IDisposable
		{
			private OrderedDictionary<TKey, TValue>.Enumerator _enumerator;

			public TKey Current => _enumerator.Current.Key;

			object IEnumerator.Current => Current;

			internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
			{
				_enumerator = dictionary.GetEnumerator();
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			void IEnumerator.Reset()
			{
				System.Collections.Generic.EnumerableHelpers.Reset(ref _enumerator);
			}

			readonly void IDisposable.Dispose()
			{
			}
		}

		private readonly OrderedDictionary<TKey, TValue> _dictionary;

		public int Count => _dictionary.Count;

		bool ICollection<TKey>.IsReadOnly => true;

		bool IList.IsReadOnly => true;

		bool IList.IsFixedSize => false;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

		TKey IList<TKey>.this[int index]
		{
			get
			{
				return _dictionary.GetAt(index).Key;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		object? IList.this[int index]
		{
			get
			{
				return _dictionary.GetAt(index).Key;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		TKey IReadOnlyList<TKey>.this[int index] => _dictionary.GetAt(index).Key;

		internal KeyCollection(OrderedDictionary<TKey, TValue> dictionary)
		{
			_dictionary = dictionary;
		}

		public bool Contains(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		bool IList.Contains(object value)
		{
			if (value is TKey key)
			{
				return Contains(key);
			}
			return false;
		}

		public void CopyTo(TKey[] array, int arrayIndex)
		{
			ArgumentNullException.ThrowIfNull(array, "array");
			ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, "arrayIndex");
			OrderedDictionary<TKey, TValue> dictionary = _dictionary;
			int count = dictionary._count;
			if (array.Length - arrayIndex < count)
			{
				throw new ArgumentException(System.SR.Arg_ArrayPlusOffTooSmall, "array");
			}
			Entry[] entries = dictionary._entries;
			for (int i = 0; i < count; i++)
			{
				array[arrayIndex++] = entries[i].Key;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			ArgumentNullException.ThrowIfNull(array, "array");
			if (array.Rank != 1)
			{
				throw new ArgumentException(System.SR.Arg_RankMultiDimNotSupported, "array");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException(System.SR.Arg_NonZeroLowerBound, "array");
			}
			ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
			if (array.Length - index < _dictionary.Count)
			{
				throw new ArgumentException(System.SR.Arg_ArrayPlusOffTooSmall);
			}
			if (array is TKey[] array2)
			{
				CopyTo(array2, index);
				return;
			}
			try
			{
				if (!(array is object[] array3))
				{
					throw new ArgumentException(System.SR.Argument_IncompatibleArrayType, "array");
				}
				using Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					TKey current = enumerator.Current;
					array3[index++] = current;
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException(System.SR.Argument_IncompatibleArrayType, "array");
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(_dictionary);
		}

		IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
		{
			if (Count != 0)
			{
				return GetEnumerator();
			}
			return System.Collections.Generic.EnumerableHelpers.GetEmptyEnumerator<TKey>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TKey>)this).GetEnumerator();
		}

		int IList<TKey>.IndexOf(TKey item)
		{
			return _dictionary.IndexOf(item);
		}

		void ICollection<TKey>.Add(TKey item)
		{
			throw new NotSupportedException();
		}

		void ICollection<TKey>.Clear()
		{
			throw new NotSupportedException();
		}

		void IList<TKey>.Insert(int index, TKey item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<TKey>.Remove(TKey item)
		{
			throw new NotSupportedException();
		}

		void IList<TKey>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		int IList.IndexOf(object value)
		{
			if (!(value is TKey key))
			{
				return -1;
			}
			return _dictionary.IndexOf(key);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}

	[DebuggerTypeProxy(typeof(System.Collections.Generic.ICollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	public sealed class ValueCollection : IList<TValue>, ICollection<TValue>, IEnumerable<TValue>, IEnumerable, IReadOnlyList<TValue>, IReadOnlyCollection<TValue>, IList, ICollection
	{
		public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
		{
			private OrderedDictionary<TKey, TValue>.Enumerator _enumerator;

			public TValue Current => _enumerator.Current.Value;

			object? IEnumerator.Current => Current;

			internal Enumerator(OrderedDictionary<TKey, TValue> dictionary)
			{
				_enumerator = dictionary.GetEnumerator();
			}

			public bool MoveNext()
			{
				return _enumerator.MoveNext();
			}

			void IEnumerator.Reset()
			{
				System.Collections.Generic.EnumerableHelpers.Reset(ref _enumerator);
			}

			readonly void IDisposable.Dispose()
			{
			}
		}

		private readonly OrderedDictionary<TKey, TValue> _dictionary;

		public int Count => _dictionary.Count;

		bool ICollection<TValue>.IsReadOnly => true;

		bool IList.IsReadOnly => true;

		bool IList.IsFixedSize => false;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

		TValue IList<TValue>.this[int index]
		{
			get
			{
				return _dictionary.GetAt(index).Value;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		TValue IReadOnlyList<TValue>.this[int index] => _dictionary.GetAt(index).Value;

		object? IList.this[int index]
		{
			get
			{
				return _dictionary.GetAt(index).Value;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal ValueCollection(OrderedDictionary<TKey, TValue> dictionary)
		{
			_dictionary = dictionary;
		}

		public void CopyTo(TValue[] array, int arrayIndex)
		{
			ArgumentNullException.ThrowIfNull(array, "array");
			ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, "arrayIndex");
			OrderedDictionary<TKey, TValue> dictionary = _dictionary;
			int count = dictionary._count;
			if (array.Length - arrayIndex < count)
			{
				throw new ArgumentException(System.SR.Arg_ArrayPlusOffTooSmall, "array");
			}
			Entry[] entries = dictionary._entries;
			for (int i = 0; i < count; i++)
			{
				array[arrayIndex++] = entries[i].Value;
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(_dictionary);
		}

		bool ICollection<TValue>.Contains(TValue item)
		{
			return _dictionary.ContainsValue(item);
		}

		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
		{
			if (Count != 0)
			{
				return GetEnumerator();
			}
			return System.Collections.Generic.EnumerableHelpers.GetEmptyEnumerator<TValue>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TValue>)this).GetEnumerator();
		}

		int IList<TValue>.IndexOf(TValue item)
		{
			Entry[] entries = _dictionary._entries;
			if (entries != null)
			{
				int count = _dictionary._count;
				for (int i = 0; i < count; i++)
				{
					if (EqualityComparer<TValue>.Default.Equals(item, entries[i].Value))
					{
						return i;
					}
				}
			}
			return -1;
		}

		void ICollection<TValue>.Add(TValue item)
		{
			throw new NotSupportedException();
		}

		void ICollection<TValue>.Clear()
		{
			throw new NotSupportedException();
		}

		void IList<TValue>.Insert(int index, TValue item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<TValue>.Remove(TValue item)
		{
			throw new NotSupportedException();
		}

		void IList<TValue>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object value)
		{
			if (value != null || default(TValue) != null)
			{
				if (value is TValue value2)
				{
					return _dictionary.ContainsValue(value2);
				}
				return false;
			}
			return _dictionary.ContainsValue(default(TValue));
		}

		int IList.IndexOf(object value)
		{
			Entry[] entries = _dictionary._entries;
			if (entries != null)
			{
				int count = _dictionary._count;
				if (value == null && default(TValue) == null)
				{
					for (int i = 0; i < count; i++)
					{
						if (entries[i].Value == null)
						{
							return i;
						}
					}
				}
				else if (value is TValue x)
				{
					for (int j = 0; j < count; j++)
					{
						if (EqualityComparer<TValue>.Default.Equals(x, entries[j].Value))
						{
							return j;
						}
					}
				}
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			ArgumentNullException.ThrowIfNull(array, "array");
			if (array.Rank != 1)
			{
				throw new ArgumentException(System.SR.Arg_RankMultiDimNotSupported, "array");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException(System.SR.Arg_NonZeroLowerBound, "array");
			}
			ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
			if (array.Length - index < _dictionary.Count)
			{
				throw new ArgumentException(System.SR.Arg_ArrayPlusOffTooSmall);
			}
			if (array is TValue[] array2)
			{
				CopyTo(array2, index);
				return;
			}
			try
			{
				if (!(array is object[] array3))
				{
					throw new ArgumentException(System.SR.Argument_IncompatibleArrayType, "array");
				}
				using Enumerator enumerator = GetEnumerator();
				while (enumerator.MoveNext())
				{
					TValue current = enumerator.Current;
					array3[index++] = current;
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException(System.SR.Argument_IncompatibleArrayType, "array");
			}
		}
	}

	private IEqualityComparer<TKey> _comparer;

	private int[] _buckets;

	private Entry[] _entries;

	private int _count;

	private int _version;

	private ulong _fastModMultiplier;

	public int Capacity
	{
		get
		{
			Entry[] entries = _entries;
			if (entries == null)
			{
				return 0;
			}
			return entries.Length;
		}
	}

	public IEqualityComparer<TKey> Comparer
	{
		get
		{
			IEqualityComparer<TKey> comparer = _comparer;
			if (typeof(TKey) == typeof(string) && (comparer as NonRandomizedStringEqualityComparer)?.GetUnderlyingEqualityComparer() is IEqualityComparer<TKey> result)
			{
				return result;
			}
			return comparer ?? EqualityComparer<TKey>.Default;
		}
	}

	public int Count => _count;

	bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

	bool IDictionary.IsReadOnly => false;

	bool IList.IsReadOnly => false;

	bool IDictionary.IsFixedSize => false;

	bool IList.IsFixedSize => false;

	public KeyCollection Keys => _003CKeys_003Ek__BackingField ?? (_003CKeys_003Ek__BackingField = new KeyCollection(this));

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

	ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

	ICollection IDictionary.Keys => Keys;

	public ValueCollection Values => _003CValues_003Ek__BackingField ?? (_003CValues_003Ek__BackingField = new ValueCollection(this));

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

	ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

	ICollection IDictionary.Values => Values;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object? IList.this[int index]
	{
		get
		{
			return GetAt(index);
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			if (!(value is KeyValuePair<TKey, TValue> keyValuePair))
			{
				throw new ArgumentException(System.SR.Format(System.SR.Arg_WrongType, value, typeof(KeyValuePair<TKey, TValue>)), "value");
			}
			SetAt(index, keyValuePair.Key, keyValuePair.Value);
		}
	}

	object? IDictionary.this[object key]
	{
		get
		{
			ArgumentNullException.ThrowIfNull(key, "key");
			if (key is TKey key2 && TryGetValue(key2, out var value))
			{
				return value;
			}
			return null;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(key, "key");
			if (default(TValue) != null)
			{
				ArgumentNullException.ThrowIfNull(value, "value");
			}
			if (!(key is TKey key2))
			{
				throw new ArgumentException(System.SR.Format(System.SR.Arg_WrongType, key, typeof(TKey)), "key");
			}
			TValue value2 = default(TValue);
			if (value != null)
			{
				if (!(value is TValue val))
				{
					throw new ArgumentException(System.SR.Format(System.SR.Arg_WrongType, value, typeof(TValue)), "value");
				}
				value2 = val;
			}
			this[key2] = value2;
		}
	}

	KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
	{
		get
		{
			return GetAt(index);
		}
		set
		{
			SetAt(index, value.Key, value.Value);
		}
	}

	KeyValuePair<TKey, TValue> IReadOnlyList<KeyValuePair<TKey, TValue>>.this[int index] => GetAt(index);

	public TValue this[TKey key]
	{
		get
		{
			if (!TryGetValue(key, out var value))
			{
				ThrowHelper.ThrowKeyNotFound(key);
			}
			return value;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(key, "key");
			TryInsert(-1, key, value, System.Collections.Generic.InsertionBehavior.OverwriteExisting, out var _);
		}
	}

	public OrderedDictionary()
		: this(0, (IEqualityComparer<TKey>?)null)
	{
	}

	public OrderedDictionary(int capacity)
		: this(capacity, (IEqualityComparer<TKey>?)null)
	{
	}

	public OrderedDictionary(IEqualityComparer<TKey>? comparer)
		: this(0, comparer)
	{
	}

	public OrderedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity, "capacity");
		if (capacity > 0)
		{
			EnsureBucketsAndEntriesInitialized(capacity);
		}
		if (!typeof(TKey).IsValueType)
		{
			_comparer = comparer ?? EqualityComparer<TKey>.Default;
			if (typeof(TKey) == typeof(string))
			{
				IEqualityComparer<string> stringComparer = NonRandomizedStringEqualityComparer.GetStringComparer(_comparer);
				if (stringComparer != null)
				{
					_comparer = (IEqualityComparer<TKey>)stringComparer;
				}
			}
		}
		else if (comparer != null && comparer != EqualityComparer<TKey>.Default)
		{
			_comparer = comparer;
		}
	}

	public OrderedDictionary(IDictionary<TKey, TValue> dictionary)
		: this(dictionary, (IEqualityComparer<TKey>?)null)
	{
	}

	public OrderedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
		: this(dictionary?.Count ?? 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(dictionary, "dictionary");
		AddRange(dictionary);
	}

	public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
		: this(collection, (IEqualityComparer<TKey>?)null)
	{
	}

	public OrderedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
		: this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer)
	{
		ArgumentNullException.ThrowIfNull(collection, "collection");
		AddRange(collection);
	}

	[MemberNotNull("_buckets")]
	[MemberNotNull("_entries")]
	private void EnsureBucketsAndEntriesInitialized(int capacity)
	{
		Resize(System.Collections.HashHelpers.GetPrime(capacity));
	}

	private bool TryInsert(int index, TKey key, TValue value, System.Collections.Generic.InsertionBehavior behavior, out int keyIndex)
	{
		uint outHashCode = 0u;
		uint outCollisionCount = 0u;
		int num = IndexOf(key, ref outHashCode, ref outCollisionCount);
		if (num >= 0)
		{
			keyIndex = num;
			switch (behavior)
			{
			case System.Collections.Generic.InsertionBehavior.OverwriteExisting:
				_entries[num].Value = value;
				return true;
			case System.Collections.Generic.InsertionBehavior.ThrowOnExisting:
				break;
			default:
				return false;
			}
			ThrowHelper.ThrowDuplicateKey(key);
		}
		if (index < 0)
		{
			index = _count;
		}
		if (_buckets == null)
		{
			EnsureBucketsAndEntriesInitialized(0);
		}
		Entry[] entries = _entries;
		if (entries.Length == _count)
		{
			Resize(System.Collections.HashHelpers.ExpandPrime(entries.Length));
			entries = _entries;
		}
		for (num = _count - 1; num >= index; num--)
		{
			entries[num + 1] = entries[num];
			UpdateBucketIndex(num, 1);
		}
		ref Entry reference = ref entries[index];
		reference.HashCode = outHashCode;
		reference.Key = key;
		reference.Value = value;
		PushEntryIntoBucket(ref reference, index);
		_count++;
		_version++;
		RehashIfNecessary(outCollisionCount, entries);
		keyIndex = index;
		return true;
	}

	public void Add(TKey key, TValue value)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		TryInsert(-1, key, value, System.Collections.Generic.InsertionBehavior.ThrowOnExisting, out var _);
	}

	public bool TryAdd(TKey key, TValue value)
	{
		int index;
		return TryAdd(key, value, out index);
	}

	public bool TryAdd(TKey key, TValue value, out int index)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		return TryInsert(-1, key, value, System.Collections.Generic.InsertionBehavior.IgnoreInsertion, out index);
	}

	private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
	{
		if (collection is KeyValuePair<TKey, TValue>[] array)
		{
			KeyValuePair<TKey, TValue>[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				KeyValuePair<TKey, TValue> keyValuePair = array2[i];
				Add(keyValuePair.Key, keyValuePair.Value);
			}
			return;
		}
		foreach (KeyValuePair<TKey, TValue> item in collection)
		{
			Add(item.Key, item.Value);
		}
	}

	public void Clear()
	{
		if (_buckets != null && _count != 0)
		{
			Array.Clear(_buckets, 0, _buckets.Length);
			Array.Clear(_entries, 0, _count);
			_count = 0;
			_version++;
		}
	}

	public bool ContainsKey(TKey key)
	{
		return IndexOf(key) >= 0;
	}

	public bool ContainsValue(TValue value)
	{
		int count = _count;
		Entry[] entries = _entries;
		if (entries == null)
		{
			return false;
		}
		if (typeof(TValue).IsValueType)
		{
			for (int i = 0; i < count; i++)
			{
				if (EqualityComparer<TValue>.Default.Equals(value, entries[i].Value))
				{
					return true;
				}
			}
		}
		else
		{
			EqualityComparer<TValue> equalityComparer = EqualityComparer<TValue>.Default;
			for (int j = 0; j < count; j++)
			{
				if (equalityComparer.Equals(value, entries[j].Value))
				{
					return true;
				}
			}
		}
		return false;
	}

	public KeyValuePair<TKey, TValue> GetAt(int index)
	{
		if ((uint)index >= (uint)_count)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange();
		}
		ref Entry reference = ref _entries[index];
		return new KeyValuePair<TKey, TValue>(reference.Key, reference.Value);
	}

	public int IndexOf(TKey key)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		uint outHashCode = 0u;
		return IndexOf(key, ref outHashCode, ref outHashCode);
	}

	private int IndexOf(TKey key, ref uint outHashCode, ref uint outCollisionCount)
	{
		uint num = 0u;
		IEqualityComparer<TKey> comparer = _comparer;
		uint num2;
		int num3;
		if (_buckets == null)
		{
			num2 = (uint)(comparer?.GetHashCode(key) ?? key.GetHashCode());
			num = 0u;
		}
		else
		{
			num3 = -1;
			ref Entry reference = ref Unsafe.NullRef<Entry>();
			Entry[] entries = _entries;
			if (typeof(TKey).IsValueType && comparer == null)
			{
				num2 = (uint)key.GetHashCode();
				num3 = GetBucket(num2) - 1;
				while ((uint)num3 < (uint)entries.Length)
				{
					reference = ref entries[num3];
					if (reference.HashCode != num2 || !EqualityComparer<TKey>.Default.Equals(reference.Key, key))
					{
						num3 = reference.Next;
						num++;
						if (num <= (uint)entries.Length)
						{
							continue;
						}
						goto IL_0111;
					}
					goto IL_0116;
				}
			}
			else
			{
				num2 = (uint)comparer.GetHashCode(key);
				num3 = GetBucket(num2) - 1;
				while ((uint)num3 < (uint)entries.Length)
				{
					reference = ref entries[num3];
					if (reference.HashCode != num2 || !comparer.Equals(reference.Key, key))
					{
						num3 = reference.Next;
						num++;
						if (num <= (uint)entries.Length)
						{
							continue;
						}
						goto IL_0111;
					}
					goto IL_0116;
				}
			}
		}
		num3 = -1;
		outCollisionCount = num;
		goto IL_0116;
		IL_0116:
		outHashCode = num2;
		return num3;
		IL_0111:
		ThrowHelper.ThrowConcurrentOperation();
		goto IL_0116;
	}

	public void Insert(int index, TKey key, TValue value)
	{
		if ((uint)index > (uint)_count)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange();
		}
		ArgumentNullException.ThrowIfNull(key, "key");
		TryInsert(index, key, value, System.Collections.Generic.InsertionBehavior.ThrowOnExisting, out var _);
	}

	public bool Remove(TKey key)
	{
		TValue value;
		return Remove(key, out value);
	}

	public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		int num = IndexOf(key);
		if (num >= 0)
		{
			value = _entries[num].Value;
			RemoveAt(num);
			return true;
		}
		value = default(TValue);
		return false;
	}

	public void RemoveAt(int index)
	{
		int count = _count;
		if ((uint)index >= (uint)count)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange();
		}
		RemoveEntryFromBucket(index);
		Entry[] entries = _entries;
		for (int i = index + 1; i < count; i++)
		{
			entries[i - 1] = entries[i];
			UpdateBucketIndex(i, -1);
		}
		entries[--_count] = default(Entry);
		_version++;
	}

	public void SetAt(int index, TValue value)
	{
		if ((uint)index >= (uint)_count)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange();
		}
		_entries[index].Value = value;
	}

	public void SetAt(int index, TKey key, TValue value)
	{
		if ((uint)index >= (uint)_count)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange();
		}
		ArgumentNullException.ThrowIfNull(key, "key");
		ref Entry reference = ref _entries[index];
		if (typeof(TKey).IsValueType && _comparer == null)
		{
			if (EqualityComparer<TKey>.Default.Equals(key, reference.Key))
			{
				reference.Value = value;
				return;
			}
		}
		else if (_comparer.Equals(key, reference.Key))
		{
			reference.Value = value;
			return;
		}
		uint outHashCode = 0u;
		uint outCollisionCount = 0u;
		if (IndexOf(key, ref outHashCode, ref outCollisionCount) >= 0)
		{
			ThrowHelper.ThrowDuplicateKey(key);
		}
		RemoveEntryFromBucket(index);
		reference.HashCode = outHashCode;
		reference.Key = key;
		reference.Value = value;
		PushEntryIntoBucket(ref reference, index);
		_version++;
		RehashIfNecessary(outCollisionCount, _entries);
	}

	public int EnsureCapacity(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(capacity, "capacity");
		if (Capacity < capacity)
		{
			if (_buckets == null)
			{
				EnsureBucketsAndEntriesInitialized(capacity);
			}
			else
			{
				Resize(System.Collections.HashHelpers.GetPrime(capacity));
			}
			_version++;
		}
		return Capacity;
	}

	public void TrimExcess()
	{
		TrimExcess(_count);
	}

	public void TrimExcess(int capacity)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(capacity, Count, "capacity");
		Entry[] entries = _entries;
		int num = ((entries != null) ? entries.Length : 0);
		capacity = System.Collections.HashHelpers.GetPrime(capacity);
		if (capacity < num)
		{
			Resize(capacity);
		}
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		int index;
		return TryGetValue(key, out value, out index);
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value, out int index)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		index = IndexOf(key);
		if (index >= 0)
		{
			value = _entries[index].Value;
			return true;
		}
		value = default(TValue);
		return false;
	}

	private void PushEntryIntoBucket(ref Entry entry, int entryIndex)
	{
		ref int bucket = ref GetBucket(entry.HashCode);
		entry.Next = bucket - 1;
		bucket = entryIndex + 1;
	}

	private void RemoveEntryFromBucket(int entryIndex)
	{
		Entry[] entries = _entries;
		Entry entry = entries[entryIndex];
		ref int bucket = ref GetBucket(entry.HashCode);
		if (bucket == entryIndex + 1)
		{
			bucket = entry.Next + 1;
			return;
		}
		int num = bucket - 1;
		int num2 = 0;
		ref Entry reference;
		while (true)
		{
			reference = ref entries[num];
			if (reference.Next == entryIndex)
			{
				break;
			}
			num = reference.Next;
			if (++num2 > entries.Length)
			{
				ThrowHelper.ThrowConcurrentOperation();
			}
		}
		reference.Next = entry.Next;
	}

	private void UpdateBucketIndex(int entryIndex, int shiftAmount)
	{
		Entry[] entries = _entries;
		Entry entry = entries[entryIndex];
		ref int bucket = ref GetBucket(entry.HashCode);
		if (bucket == entryIndex + 1)
		{
			bucket += shiftAmount;
			return;
		}
		int num = bucket - 1;
		int num2 = 0;
		ref Entry reference;
		while (true)
		{
			reference = ref entries[num];
			if (reference.Next == entryIndex)
			{
				break;
			}
			num = reference.Next;
			if (++num2 > entries.Length)
			{
				ThrowHelper.ThrowConcurrentOperation();
			}
		}
		reference.Next += shiftAmount;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void RehashIfNecessary(uint collisionCount, Entry[] entries)
	{
		if (!typeof(TKey).IsValueType && collisionCount > 100 && _comparer is NonRandomizedStringEqualityComparer)
		{
			Resize(entries.Length, forceNewHashCodes: true);
		}
	}

	[MemberNotNull("_buckets")]
	[MemberNotNull("_entries")]
	private void Resize(int newSize, bool forceNewHashCodes = false)
	{
		int[] buckets = new int[newSize];
		Entry[] array = new Entry[newSize];
		if (IntPtr.Size == 8)
		{
			_fastModMultiplier = System.Collections.HashHelpers.GetFastModMultiplier((uint)newSize);
		}
		int count = _count;
		if (_entries != null)
		{
			Array.Copy(_entries, array, count);
		}
		if (!typeof(TKey).IsValueType & forceNewHashCodes)
		{
			IEqualityComparer<TKey> equalityComparer = (_comparer = (IEqualityComparer<TKey>)((NonRandomizedStringEqualityComparer)_comparer).GetUnderlyingEqualityComparer());
			for (int i = 0; i < count; i++)
			{
				array[i].HashCode = (uint)equalityComparer.GetHashCode(array[i].Key);
			}
		}
		_buckets = buckets;
		for (int j = 0; j < count; j++)
		{
			PushEntryIntoBucket(ref array[j], j);
		}
		_entries = array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ref int GetBucket(uint hashCode)
	{
		int[] buckets = _buckets;
		if (IntPtr.Size == 8)
		{
			return ref buckets[System.Collections.HashHelpers.FastMod(hashCode, (uint)buckets.Length, _fastModMultiplier)];
		}
		return ref buckets[hashCode % buckets.Length];
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this, useDictionaryEntry: false);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		if (Count != 0)
		{
			return GetEnumerator();
		}
		return System.Collections.Generic.EnumerableHelpers.GetEmptyEnumerator<KeyValuePair<TKey, TValue>>();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return new Enumerator(this, useDictionaryEntry: true);
	}

	int IList<KeyValuePair<TKey, TValue>>.IndexOf(KeyValuePair<TKey, TValue> item)
	{
		ArgumentNullException.ThrowIfNull(item.Key, "item");
		int num = IndexOf(item.Key);
		if (num >= 0 && EqualityComparer<TValue>.Default.Equals(item.Value, _entries[num].Value))
		{
			return num;
		}
		return -1;
	}

	void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> item)
	{
		Insert(index, item.Key, item.Value);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		ArgumentNullException.ThrowIfNull(item.Key, "item");
		if (TryGetValue(item.Key, out var value))
		{
			return EqualityComparer<TValue>.Default.Equals(value, item.Value);
		}
		return false;
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, "arrayIndex");
		if (array.Length - arrayIndex < _count)
		{
			throw new ArgumentException(System.SR.Arg_ArrayPlusOffTooSmall);
		}
		for (int i = 0; i < _count; i++)
		{
			ref Entry reference = ref _entries[i];
			array[arrayIndex++] = new KeyValuePair<TKey, TValue>(reference.Key, reference.Value);
		}
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		if (TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
		{
			return Remove(item.Key);
		}
		return false;
	}

	void IDictionary.Add(object key, object value)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		if (default(TValue) != null)
		{
			ArgumentNullException.ThrowIfNull(value, "value");
		}
		if (!(key is TKey key2))
		{
			throw new ArgumentException(System.SR.Format(System.SR.Arg_WrongType, key, typeof(TKey)), "key");
		}
		if (default(TValue) != null)
		{
			ArgumentNullException.ThrowIfNull(value, "value");
		}
		TValue value2 = default(TValue);
		if (value != null)
		{
			if (!(value is TValue val))
			{
				throw new ArgumentException(System.SR.Format(System.SR.Arg_WrongType, value, typeof(TValue)), "value");
			}
			value2 = val;
		}
		Add(key2, value2);
	}

	bool IDictionary.Contains(object key)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		if (key is TKey key2)
		{
			return ContainsKey(key2);
		}
		return false;
	}

	void IDictionary.Remove(object key)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		if (key is TKey key2)
		{
			Remove(key2);
		}
	}

	void ICollection.CopyTo(Array array, int index)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		if (array.Rank != 1)
		{
			throw new ArgumentException(System.SR.Arg_RankMultiDimNotSupported, "array");
		}
		if (array.GetLowerBound(0) != 0)
		{
			throw new ArgumentException(System.SR.Arg_NonZeroLowerBound, "array");
		}
		ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
		if (array.Length - index < _count)
		{
			throw new ArgumentException(System.SR.Arg_ArrayPlusOffTooSmall);
		}
		if (array is KeyValuePair<TKey, TValue>[] array2)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)this).CopyTo(array2, index);
			return;
		}
		try
		{
			if (!(array is object[] array3))
			{
				throw new ArgumentException(System.SR.Argument_IncompatibleArrayType, "array");
			}
			using Enumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TValue> current = enumerator.Current;
				array3[index++] = current;
			}
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException(System.SR.Argument_IncompatibleArrayType, "array");
		}
	}

	int IList.Add(object value)
	{
		if (!(value is KeyValuePair<TKey, TValue> keyValuePair))
		{
			throw new ArgumentException(System.SR.Format(System.SR.Arg_WrongType, value, typeof(KeyValuePair<TKey, TValue>)), "value");
		}
		Add(keyValuePair.Key, keyValuePair.Value);
		return Count - 1;
	}

	bool IList.Contains(object value)
	{
		if (value is KeyValuePair<TKey, TValue> keyValuePair && TryGetValue(keyValuePair.Key, out var value2))
		{
			return EqualityComparer<TValue>.Default.Equals(value2, keyValuePair.Value);
		}
		return false;
	}

	int IList.IndexOf(object value)
	{
		if (value is KeyValuePair<TKey, TValue> item)
		{
			return ((IList<KeyValuePair<TKey, TValue>>)this).IndexOf(item);
		}
		return -1;
	}

	void IList.Insert(int index, object value)
	{
		if (!(value is KeyValuePair<TKey, TValue> keyValuePair))
		{
			throw new ArgumentException(System.SR.Format(System.SR.Arg_WrongType, value, typeof(KeyValuePair<TKey, TValue>)), "value");
		}
		Insert(index, keyValuePair.Key, keyValuePair.Value);
	}

	void IList.Remove(object value)
	{
		if (value is KeyValuePair<TKey, TValue> item)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)this).Remove(item);
		}
	}
}
