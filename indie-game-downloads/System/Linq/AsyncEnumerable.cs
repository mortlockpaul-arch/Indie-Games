using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq;

public static class AsyncEnumerable
{
	private sealed class EmptyAsyncEnumerable<TResult> : IAsyncEnumerable<TResult>, IAsyncEnumerator<TResult>, IAsyncDisposable, IOrderedAsyncEnumerable<TResult>
	{
		public static readonly EmptyAsyncEnumerable<TResult> Instance = new EmptyAsyncEnumerable<TResult>();

		public TResult Current => default(TResult);

		public IAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return this;
		}

		public ValueTask<bool> MoveNextAsync()
		{
			return default(ValueTask<bool>);
		}

		public ValueTask DisposeAsync()
		{
			return default(ValueTask);
		}

		public IOrderedAsyncEnumerable<TResult> CreateOrderedAsyncEnumerable<TKey>(Func<TResult, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
			return this;
		}

		public IOrderedAsyncEnumerable<TResult> CreateOrderedAsyncEnumerable<TKey>(Func<TResult, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey> comparer, bool descending)
		{
			ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
			return this;
		}
	}

	internal sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>, IEnumerable<TElement>, IEnumerable, IList<TElement>, ICollection<TElement>
	{
		internal readonly TKey _key;

		internal readonly int _hashCode;

		internal TElement[] _elements;

		internal int _count;

		internal Grouping<TKey, TElement> _hashNext;

		internal Grouping<TKey, TElement> _next;

		public TKey Key => _key;

		int ICollection<TElement>.Count => _count;

		bool ICollection<TElement>.IsReadOnly => true;

		TElement IList<TElement>.this[int index]
		{
			get
			{
				if ((uint)index >= (uint)_count)
				{
					System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("index");
				}
				return _elements[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal Grouping(TKey key, int hashCode)
		{
			_key = key;
			_hashCode = hashCode;
			_elements = new TElement[1];
		}

		internal void Add(TElement element)
		{
			if (_elements.Length == _count)
			{
				Array.Resize(ref _elements, checked(_count * 2));
			}
			_elements[_count] = element;
			_count++;
		}

		internal void Trim()
		{
			if (_elements.Length != _count)
			{
				Array.Resize(ref _elements, _count);
			}
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			for (int i = 0; i < _count; i++)
			{
				yield return _elements[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection<TElement>.Add(TElement item)
		{
			throw new NotSupportedException();
		}

		void ICollection<TElement>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<TElement>.Contains(TElement item)
		{
			return Array.IndexOf(_elements, item, 0, _count) >= 0;
		}

		void ICollection<TElement>.CopyTo(TElement[] array, int arrayIndex)
		{
			Array.Copy(_elements, 0, array, arrayIndex, _count);
		}

		bool ICollection<TElement>.Remove(TElement item)
		{
			throw new NotSupportedException();
		}

		int IList<TElement>.IndexOf(TElement item)
		{
			return Array.IndexOf(_elements, item, 0, _count);
		}

		void IList<TElement>.Insert(int index, TElement item)
		{
			throw new NotSupportedException();
		}

		void IList<TElement>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}

	private abstract class OrderedIterator<TElement> : IOrderedAsyncEnumerable<TElement>, IAsyncEnumerable<TElement>
	{
		internal readonly IAsyncEnumerable<TElement> _source;

		protected OrderedIterator(IAsyncEnumerable<TElement> source)
		{
			_source = source;
		}

		private protected ValueTask<int[]> CreateSortedMapAsync(TElement[] buffer, CancellationToken cancellationToken)
		{
			return GetEnumerableSorter().SortAsync(buffer, buffer.Length, cancellationToken);
		}

		internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next = null);

		public IOrderedAsyncEnumerable<TElement> CreateOrderedAsyncEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedIterator<TElement, TKey>(_source, keySelector, comparer, descending, this);
		}

		public IOrderedAsyncEnumerable<TElement> CreateOrderedAsyncEnumerable<TKey>(Func<TElement, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedIterator<TElement, TKey>(_source, keySelector, comparer, descending, this);
		}

		public abstract IAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken);
	}

	private sealed class OrderedIterator<TElement, TKey> : OrderedIterator<TElement>
	{
		private readonly OrderedIterator<TElement> _parent;

		private readonly object _keySelector;

		private readonly IComparer<TKey> _comparer;

		private readonly bool _descending;

		internal OrderedIterator(IAsyncEnumerable<TElement> source, object keySelector, IComparer<TKey> comparer, bool descending, OrderedIterator<TElement> parent)
			: base(source)
		{
			_parent = parent;
			_keySelector = keySelector;
			_comparer = comparer ?? Comparer<TKey>.Default;
			_descending = descending;
		}

		internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next)
		{
			IComparer<TKey> comparer = _comparer;
			if (typeof(TKey) == typeof(string) && comparer == Comparer<string>.Default)
			{
				comparer = (IComparer<TKey>)StringComparer.CurrentCulture;
			}
			EnumerableSorter<TElement> enumerableSorter = new EnumerableSorter<TElement, TKey>(_keySelector, comparer, _descending, next);
			if (_parent != null)
			{
				enumerableSorter = _parent.GetEnumerableSorter(enumerableSorter);
			}
			return enumerableSorter;
		}

		public override async IAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken)
		{
			TElement[] buffer = await _source.ToArrayAsync(cancellationToken);
			if (buffer.Length != 0)
			{
				int[] map = await CreateSortedMapAsync(buffer, cancellationToken);
				for (int i = 0; i < map.Length; i++)
				{
					yield return buffer[map[i]];
				}
			}
		}
	}

	private abstract class EnumerableSorter<TElement> : IComparer<int>
	{
		internal static readonly Func<TElement, TElement> IdentityFunc = (TElement e) => e;

		internal abstract Task ComputeKeysAsync(TElement[] elements, int count, CancellationToken cancellationToken);

		public abstract int Compare(int index1, int index2);

		internal async ValueTask<int[]> SortAsync(TElement[] elements, int count, CancellationToken cancellationToken)
		{
			await ComputeKeysAsync(elements, count, cancellationToken);
			int[] array = new int[count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			QuickSort(array, 0, count - 1);
			return array;
		}

		protected abstract void QuickSort(int[] map, int left, int right);
	}

	private sealed class EnumerableSorter<TElement, TKey> : EnumerableSorter<TElement>, IComparer<int>
	{
		private readonly object _keySelector;

		private readonly IComparer<TKey> _comparer;

		private readonly bool _descending;

		private readonly EnumerableSorter<TElement> _next;

		private TKey[] _keys;

		internal EnumerableSorter(object keySelector, IComparer<TKey> comparer, bool descending, EnumerableSorter<TElement> next)
		{
			_keySelector = keySelector;
			_comparer = comparer;
			_descending = descending;
			_next = next;
		}

		internal override async Task ComputeKeysAsync(TElement[] elements, int count, CancellationToken cancellationToken)
		{
			object keySelector = _keySelector;
			if (keySelector == EnumerableSorter<TElement>.IdentityFunc)
			{
				_keys = (TKey[])(object)elements;
			}
			else
			{
				TKey[] keys = new TKey[count];
				if (keySelector is Func<TElement, TKey> func)
				{
					for (int i = 0; i < keys.Length; i++)
					{
						keys[i] = func(elements[i]);
					}
				}
				else
				{
					Func<TElement, CancellationToken, ValueTask<TKey>> asyncSelector = (Func<TElement, CancellationToken, ValueTask<TKey>>)keySelector;
					for (int j = 0; j < keys.Length; j++)
					{
						TKey[] array = keys;
						int num = j;
						array[num] = await asyncSelector(elements[j], cancellationToken);
					}
				}
				_keys = keys;
			}
			_next?.ComputeKeysAsync(elements, count, cancellationToken);
		}

		public override int Compare(int index1, int index2)
		{
			TKey[] keys = _keys;
			int num = _comparer.Compare(keys[index1], keys[index2]);
			if (num == 0)
			{
				if (_next == null)
				{
					return index1 - index2;
				}
				return _next.Compare(index1, index2);
			}
			if (_descending == num > 0)
			{
				return -1;
			}
			return 1;
		}

		protected override void QuickSort(int[] keys, int lo, int hi)
		{
			if (typeof(TKey).IsValueType && _next == null && _comparer == Comparer<TKey>.Default)
			{
				new Span<int>(keys, lo, hi - lo + 1).Sort((!_descending) ? new Comparison<int>(Compare_DefaultComparer_NoNext_Ascending) : new Comparison<int>(Compare_DefaultComparer_NoNext_Descending));
			}
			else
			{
				new Span<int>(keys, lo, hi - lo + 1).Sort(Compare);
			}
			int Compare_DefaultComparer_NoNext_Ascending(int index1, int index2)
			{
				TKey[] keys2 = _keys;
				int num = Comparer<TKey>.Default.Compare(keys2[index1], keys2[index2]);
				if (num != 0)
				{
					return num;
				}
				return index1 - index2;
			}
			int Compare_DefaultComparer_NoNext_Descending(int index1, int index2)
			{
				TKey[] keys2 = _keys;
				int num = Comparer<TKey>.Default.Compare(keys2[index2], keys2[index1]);
				if (num != 0)
				{
					return num;
				}
				return index1 - index2;
			}
		}
	}

	[DebuggerDisplay("Count = 0")]
	private sealed class EmptyLookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable, IList<IGrouping<TKey, TElement>>, ICollection<IGrouping<TKey, TElement>>, IReadOnlyCollection<IGrouping<TKey, TElement>>
	{
		public static readonly EmptyLookup<TKey, TElement> Instance = new EmptyLookup<TKey, TElement>();

		public bool IsReadOnly => true;

		public int Count => 0;

		public IEnumerable<TElement> this[TKey key] => Array.Empty<TElement>();

		public IGrouping<TKey, TElement> this[int index]
		{
			get
			{
				throw new ArgumentOutOfRangeException("index");
			}
			set
			{
				throw new NotSupportedException();
			}
		}

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
			if ((uint)arrayIndex > (uint)array.Length)
			{
				System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("arrayIndex");
			}
		}

		public int IndexOf(IGrouping<TKey, TElement> item)
		{
			return -1;
		}

		public void Add(IGrouping<TKey, TElement> item)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public void Insert(int index, IGrouping<TKey, TElement> item)
		{
			throw new NotSupportedException();
		}

		public bool Remove(IGrouping<TKey, TElement> item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}

	[DebuggerDisplay("Count = {Count}")]
	private sealed class AsyncLookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
	{
		private readonly IEqualityComparer<TKey> _comparer;

		private Grouping<TKey, TElement>[] _groupings;

		internal Grouping<TKey, TElement> _lastGrouping;

		private int _count;

		public int Count => _count;

		public IEnumerable<TElement> this[TKey key]
		{
			get
			{
				IEnumerable<TElement> grouping = GetGrouping(key, create: false);
				return grouping ?? Enumerable.Empty<TElement>();
			}
		}

		internal AsyncLookup(IEqualityComparer<TKey> comparer)
		{
			_comparer = comparer ?? EqualityComparer<TKey>.Default;
			_groupings = new Grouping<TKey, TElement>[7];
		}

		internal static async ValueTask<AsyncLookup<TKey, TElement>> CreateForJoinAsync(IAsyncEnumerable<TElement> source, Func<TElement, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			AsyncLookup<TKey, TElement> lookup = new AsyncLookup<TKey, TElement>(comparer);
			await foreach (TElement item in source.WithCancellation(cancellationToken))
			{
				TKey val = keySelector(item);
				if (val != null)
				{
					lookup.GetGrouping(val, create: true).Add(item);
				}
			}
			return lookup;
		}

		internal static async ValueTask<AsyncLookup<TKey, TElement>> CreateForJoinAsync(IAsyncEnumerable<TElement> source, Func<TElement, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
		{
			AsyncLookup<TKey, TElement> lookup = new AsyncLookup<TKey, TElement>(comparer);
			await foreach (TElement item in source.WithCancellation(cancellationToken))
			{
				TKey val = await keySelector(item, cancellationToken);
				if (val != null)
				{
					lookup.GetGrouping(val, create: true).Add(item);
				}
			}
			return lookup;
		}

		public bool Contains(TKey key)
		{
			return GetGrouping(key, create: false) != null;
		}

		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
		{
			Grouping<TKey, TElement> g = _lastGrouping;
			if (g != null)
			{
				do
				{
					g = g._next;
					yield return g;
				}
				while (g != _lastGrouping);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal Grouping<TKey, TElement> GetGrouping(TKey key, bool create)
		{
			int num = ((key != null) ? (_comparer.GetHashCode(key) & 0x7FFFFFFF) : 0);
			for (Grouping<TKey, TElement> grouping = _groupings[(uint)num % _groupings.Length]; grouping != null; grouping = grouping._hashNext)
			{
				if (grouping._hashCode == num && _comparer.Equals(grouping._key, key))
				{
					return grouping;
				}
			}
			if (create)
			{
				if (_count == _groupings.Length)
				{
					Resize();
				}
				int num2 = num % _groupings.Length;
				Grouping<TKey, TElement> grouping2 = new Grouping<TKey, TElement>(key, num)
				{
					_hashNext = _groupings[num2]
				};
				_groupings[num2] = grouping2;
				if (_lastGrouping == null)
				{
					grouping2._next = grouping2;
				}
				else
				{
					grouping2._next = _lastGrouping._next;
					_lastGrouping._next = grouping2;
				}
				_lastGrouping = grouping2;
				_count++;
				return grouping2;
			}
			return null;
		}

		private void Resize()
		{
			int num = checked(_count * 2 + 1);
			Grouping<TKey, TElement>[] array = new Grouping<TKey, TElement>[num];
			Grouping<TKey, TElement> grouping = _lastGrouping;
			do
			{
				grouping = grouping._next;
				int num2 = grouping._hashCode % num;
				grouping._hashNext = array[num2];
				array[num2] = grouping;
			}
			while (grouping != _lastGrouping);
			_groupings = array;
		}

		internal IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			Grouping<TKey, TElement> g = _lastGrouping;
			if (g != null)
			{
				do
				{
					g = g._next;
					g.Trim();
					yield return resultSelector(g._key, g._elements);
				}
				while (g != _lastGrouping);
			}
		}

		internal async IAsyncEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, CancellationToken, ValueTask<TResult>> resultSelector, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			Grouping<TKey, TElement> g = _lastGrouping;
			if (g != null)
			{
				do
				{
					g = g._next;
					g.Trim();
					yield return await resultSelector(g._key, g._elements, cancellationToken);
				}
				while (g != _lastGrouping);
			}
		}
	}

	public static ValueTask<TSource> AggregateAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, TSource, TSource> func, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(func, "func");
		return Impl(source, func, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TSource, TSource> func2, CancellationToken cancellationToken2)
		{
			TSource result2;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					System.Linq.ThrowHelper.ThrowNoElementsException();
				}
				TSource result = e.Current;
				while (await e.MoveNextAsync())
				{
					result = func2(result, e.Current);
				}
				result2 = result;
			}
			return result2;
		}
	}

	public static ValueTask<TSource> AggregateAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, TSource, CancellationToken, ValueTask<TSource>> func, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(func, "func");
		return Impl(source, func, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TSource, CancellationToken, ValueTask<TSource>> func2, CancellationToken cancellationToken2)
		{
			TSource result2;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					System.Linq.ThrowHelper.ThrowNoElementsException();
				}
				TSource result = e.Current;
				while (await e.MoveNextAsync())
				{
					result = await func2(result, e.Current, cancellationToken2);
				}
				result2 = result;
			}
			return result2;
		}
	}

	public static ValueTask<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(func, "func");
		return Impl(source.WithCancellation(cancellationToken), seed, func);
		static async ValueTask<TAccumulate> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, TAccumulate val, Func<TAccumulate, TSource, TAccumulate> func2)
		{
			TAccumulate result = val;
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				result = func2(result, item);
			}
			return result;
		}
	}

	public static ValueTask<TAccumulate> AggregateAsync<TSource, TAccumulate>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(func, "func");
		return Impl(source, seed, func, cancellationToken);
		static async ValueTask<TAccumulate> Impl(IAsyncEnumerable<TSource> source2, TAccumulate val, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func2, CancellationToken cancellationToken2 = default(CancellationToken))
		{
			TAccumulate result = val;
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				result = await func2(result, item, cancellationToken2);
			}
			return result;
		}
	}

	public static ValueTask<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(func, "func");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		return Impl(source.WithCancellation(cancellationToken), seed, func, resultSelector);
		static async ValueTask<TResult> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, TAccumulate val, Func<TAccumulate, TSource, TAccumulate> func2, Func<TAccumulate, TResult> func3)
		{
			TAccumulate result = val;
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				result = func2(result, item);
			}
			return func3(result);
		}
	}

	public static ValueTask<TResult> AggregateAsync<TSource, TAccumulate, TResult>(this IAsyncEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func, Func<TAccumulate, CancellationToken, ValueTask<TResult>> resultSelector, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(func, "func");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		return Impl(source, seed, func, resultSelector, cancellationToken);
		static async ValueTask<TResult> Impl(IAsyncEnumerable<TSource> source2, TAccumulate val, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func2, Func<TAccumulate, CancellationToken, ValueTask<TResult>> func3, CancellationToken cancellationToken2)
		{
			TAccumulate result = val;
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				result = await func2(result, item, cancellationToken2);
			}
			return await func3(result, cancellationToken2);
		}
	}

	public static IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(func, "func");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, seed, func, keyComparer, default(CancellationToken));
		}
		return Empty<KeyValuePair<TKey, TAccumulate>>();
		static async IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TKey> func2, TAccumulate val, Func<TAccumulate, TSource, TAccumulate> func3, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await e.MoveNextAsync())
				{
					Dictionary<TKey, TAccumulate> dict = new Dictionary<TKey, TAccumulate>(comparer);
					do
					{
						TSource current = e.Current;
						TKey key = func2(current);
						ref TAccumulate valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
						valueRefOrAddDefault = func3(exists ? valueRefOrAddDefault : val, current);
					}
					while (await e.MoveNextAsync());
					foreach (KeyValuePair<TKey, TAccumulate> item in dict)
					{
						yield return item;
					}
					yield break;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, TAccumulate seed, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(func, "func");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, seed, func, keyComparer, default(CancellationToken));
		}
		return Empty<KeyValuePair<TKey, TAccumulate>>();
		static async IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func2, TAccumulate val2, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func3, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await e.MoveNextAsync())
				{
					Dictionary<TKey, TAccumulate> dict = new Dictionary<TKey, TAccumulate>(comparer);
					do
					{
						TSource value = e.Current;
						TKey val = await func2(value, cancellationToken);
						Dictionary<TKey, TAccumulate> dictionary = dict;
						TKey key = val;
						dictionary[key] = await func3(dict.TryGetValue(val, out var value2) ? value2 : val2, value, cancellationToken);
					}
					while (await e.MoveNextAsync());
					foreach (KeyValuePair<TKey, TAccumulate> item in dict)
					{
						yield return item;
					}
					yield break;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(seedSelector, "seedSelector");
		ArgumentNullException.ThrowIfNull(func, "func");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, seedSelector, func, keyComparer, default(CancellationToken));
		}
		return Empty<KeyValuePair<TKey, TAccumulate>>();
		static async IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TKey> func2, Func<TKey, TAccumulate> func4, Func<TAccumulate, TSource, TAccumulate> func3, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await e.MoveNextAsync())
				{
					Dictionary<TKey, TAccumulate> dict = new Dictionary<TKey, TAccumulate>(comparer);
					do
					{
						TSource current = e.Current;
						TKey val = func2(current);
						ref TAccumulate valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, val, out var exists);
						valueRefOrAddDefault = func3(exists ? valueRefOrAddDefault : func4(val), current);
					}
					while (await e.MoveNextAsync());
					foreach (KeyValuePair<TKey, TAccumulate> item in dict)
					{
						yield return item;
					}
					yield break;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TKey, CancellationToken, ValueTask<TAccumulate>> seedSelector, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(seedSelector, "seedSelector");
		ArgumentNullException.ThrowIfNull(func, "func");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, seedSelector, func, keyComparer, default(CancellationToken));
		}
		return Empty<KeyValuePair<TKey, TAccumulate>>();
		static async IAsyncEnumerable<KeyValuePair<TKey, TAccumulate>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func2, Func<TKey, CancellationToken, ValueTask<TAccumulate>> func3, Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func4, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				Dictionary<TKey, TAccumulate> dict = new Dictionary<TKey, TAccumulate>(comparer);
				do
				{
					TSource value = e.Current;
					TKey val = await func2(value, cancellationToken);
					Dictionary<TKey, TAccumulate> dictionary = dict;
					TKey key = val;
					TAccumulate arg = ((!dict.TryGetValue(val, out var value2)) ? (await func3(val, cancellationToken)) : value2);
					dictionary[key] = await func4(arg, value, cancellationToken);
				}
				while (await e.MoveNextAsync());
				foreach (KeyValuePair<TKey, TAccumulate> item in dict)
				{
					yield return item;
				}
			}
		}
	}

	public static ValueTask<bool> AllAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source.WithCancellation(cancellationToken), predicate);
		static async ValueTask<bool> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, bool> func)
		{
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				if (!func(item))
				{
					return false;
				}
			}
			return true;
		}
	}

	public static ValueTask<bool> AllAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<bool> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, CancellationToken cancellationToken2)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				if (!(await func(item, cancellationToken2)))
				{
					return false;
				}
			}
			return true;
		}
	}

	public static ValueTask<bool> AnyAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, cancellationToken);
		static async ValueTask<bool> Impl(IAsyncEnumerable<TSource> asyncEnumerable, CancellationToken cancellationToken2)
		{
			bool result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				result = await e.MoveNextAsync();
			}
			return result;
		}
	}

	public static ValueTask<bool> AnyAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source.WithCancellation(cancellationToken), predicate);
		static async ValueTask<bool> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, bool> func)
		{
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				if (func(item))
				{
					return true;
				}
			}
			return false;
		}
	}

	public static ValueTask<bool> AnyAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<bool> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, CancellationToken cancellationToken2)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				if (await func(item, cancellationToken2))
				{
					return true;
				}
			}
			return false;
		}
	}

	public static IAsyncEnumerable<TSource> Append<TSource>(this IAsyncEnumerable<TSource> source, TSource element)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, element, default(CancellationToken));
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, TSource val, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				yield return item;
			}
			yield return val;
		}
	}

	public static ValueTask<double> AverageAsync(this IAsyncEnumerable<int> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double> Impl(ConfiguredCancelableAsyncEnumerable<int> configuredCancelableAsyncEnumerable)
		{
			long sum = 0L;
			long count = 0L;
			await foreach (int item in configuredCancelableAsyncEnumerable)
			{
				sum = checked(sum + item);
				count++;
			}
			if (count == 0L)
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			return (double)sum / (double)count;
		}
	}

	public static ValueTask<double> AverageAsync(this IAsyncEnumerable<long> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double> Impl(ConfiguredCancelableAsyncEnumerable<long> configuredCancelableAsyncEnumerable)
		{
			long sum = 0L;
			long count = 0L;
			await foreach (long item in configuredCancelableAsyncEnumerable)
			{
				sum = checked(sum + item);
				count++;
			}
			if (count == 0L)
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			return (double)sum / (double)count;
		}
	}

	public static ValueTask<float> AverageAsync(this IAsyncEnumerable<float> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<float> Impl(ConfiguredCancelableAsyncEnumerable<float> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			long count = 0L;
			await foreach (float item in configuredCancelableAsyncEnumerable)
			{
				double num = item;
				sum += num;
				count++;
			}
			if (count == 0L)
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			return (float)(sum / (double)count);
		}
	}

	public static ValueTask<double> AverageAsync(this IAsyncEnumerable<double> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double> Impl(ConfiguredCancelableAsyncEnumerable<double> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			long count = 0L;
			await foreach (double item in configuredCancelableAsyncEnumerable)
			{
				sum += item;
				count++;
			}
			if (count == 0L)
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			return sum / (double)count;
		}
	}

	public static ValueTask<decimal> AverageAsync(this IAsyncEnumerable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<decimal> Impl(ConfiguredCancelableAsyncEnumerable<decimal> configuredCancelableAsyncEnumerable)
		{
			decimal sum = 0m;
			long count = 0L;
			await foreach (decimal item in configuredCancelableAsyncEnumerable)
			{
				sum += item;
				count++;
			}
			if (count == 0L)
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			return sum / (decimal)count;
		}
	}

	public static ValueTask<double?> AverageAsync(this IAsyncEnumerable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double?> Impl(ConfiguredCancelableAsyncEnumerable<int?> configuredCancelableAsyncEnumerable)
		{
			long sum = 0L;
			long count = 0L;
			await foreach (int? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					int valueOrDefault = item.GetValueOrDefault();
					sum = checked(sum + valueOrDefault);
					count++;
				}
			}
			return (count != 0L) ? new double?((double)sum / (double)count) : ((double?)null);
		}
	}

	public static ValueTask<double?> AverageAsync(this IAsyncEnumerable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double?> Impl(ConfiguredCancelableAsyncEnumerable<long?> configuredCancelableAsyncEnumerable)
		{
			long sum = 0L;
			long count = 0L;
			await foreach (long? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					long valueOrDefault = item.GetValueOrDefault();
					sum = checked(sum + valueOrDefault);
					count++;
				}
			}
			return (count != 0L) ? new double?((double)sum / (double)count) : ((double?)null);
		}
	}

	public static ValueTask<float?> AverageAsync(this IAsyncEnumerable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<float?> Impl(ConfiguredCancelableAsyncEnumerable<float?> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			long count = 0L;
			await foreach (float? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					float valueOrDefault = item.GetValueOrDefault();
					sum += (double)valueOrDefault;
					count++;
				}
			}
			return (count != 0L) ? new float?((float)(sum / (double)count)) : ((float?)null);
		}
	}

	public static ValueTask<double?> AverageAsync(this IAsyncEnumerable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double?> Impl(ConfiguredCancelableAsyncEnumerable<double?> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			long count = 0L;
			await foreach (double? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					double valueOrDefault = item.GetValueOrDefault();
					sum += valueOrDefault;
					count++;
				}
			}
			return (count != 0L) ? new double?(sum / (double)count) : ((double?)null);
		}
	}

	public static ValueTask<decimal?> AverageAsync(this IAsyncEnumerable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<decimal?> Impl(ConfiguredCancelableAsyncEnumerable<decimal?> configuredCancelableAsyncEnumerable)
		{
			decimal sum = 0m;
			long count = 0L;
			await foreach (decimal? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					decimal valueOrDefault = item.GetValueOrDefault();
					sum += valueOrDefault;
					count++;
				}
			}
			return (count != 0L) ? new decimal?(sum / (decimal)count) : ((decimal?)null);
		}
	}

	public static IAsyncEnumerable<TResult> Cast<TResult>(this IAsyncEnumerable<object?> source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		object obj;
		if (!source.IsKnownEmpty())
		{
			obj = source as IAsyncEnumerable<TResult>;
			if (obj == null)
			{
				return Impl(source, default(CancellationToken));
			}
		}
		else
		{
			obj = Empty<TResult>();
		}
		return (IAsyncEnumerable<TResult>)obj;
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<object> source2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (object item in source2.WithCancellation(cancellationToken))
			{
				yield return (TResult)item;
			}
		}
	}

	public static IAsyncEnumerable<TSource[]> Chunk<TSource>(this IAsyncEnumerable<TSource> source, int size)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		System.Linq.ThrowHelper.ThrowIfNegativeOrZero(size, "size");
		if (!source.IsKnownEmpty())
		{
			return Chunk(source, size, default(CancellationToken));
		}
		return Empty<TSource[]>();
		static async IAsyncEnumerable<TSource[]> Chunk(IAsyncEnumerable<TSource> asyncEnumerable, int num, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				int arraySize = Math.Min(num, 4);
				bool flag;
				do
				{
					TSource[] array = new TSource[arraySize];
					array[0] = e.Current;
					int i = 1;
					if (num != array.Length)
					{
						while (true)
						{
							flag = i < num;
							if (flag)
							{
								flag = await e.MoveNextAsync();
							}
							if (!flag)
							{
								break;
							}
							if (i >= array.Length)
							{
								arraySize = (int)Math.Min((uint)num, (uint)(2 * array.Length));
								Array.Resize(ref array, arraySize);
							}
							array[i] = e.Current;
							i++;
						}
					}
					else
					{
						TSource[] local = array;
						while (true)
						{
							flag = (uint)i < (uint)local.Length;
							if (flag)
							{
								flag = await e.MoveNextAsync();
							}
							if (!flag)
							{
								break;
							}
							local[i] = e.Current;
							i++;
						}
					}
					if (i != array.Length)
					{
						Array.Resize(ref array, i);
					}
					yield return array;
					flag = i >= num;
					if (flag)
					{
						flag = await e.MoveNextAsync();
					}
				}
				while (flag);
			}
		}
	}

	public static IAsyncEnumerable<TSource> Concat<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		if (!first.IsKnownEmpty())
		{
			if (!second.IsKnownEmpty())
			{
				return Impl(first, second, default(CancellationToken));
			}
			return first;
		}
		return second;
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source, IAsyncEnumerable<TSource> source2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source.WithCancellation(cancellationToken))
			{
				yield return item;
			}
			await foreach (TSource item2 in source2.WithCancellation(cancellationToken))
			{
				yield return item2;
			}
		}
	}

	public static ValueTask<bool> ContainsAsync<TSource>(this IAsyncEnumerable<TSource> source, TSource value, IEqualityComparer<TSource>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken), value, comparer ?? EqualityComparer<TSource>.Default);
		static async ValueTask<bool> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, TSource y, IEqualityComparer<TSource> equalityComparer)
		{
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				if (equalityComparer.Equals(item, y))
				{
					return true;
				}
			}
			return false;
		}
	}

	public static ValueTask<int> CountAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, cancellationToken);
		static async ValueTask<int> Impl(IAsyncEnumerable<TSource> asyncEnumerable, CancellationToken cancellationToken2 = default(CancellationToken))
		{
			int result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				int count = 0;
				while (await e.MoveNextAsync())
				{
					count = checked(count + 1);
				}
				result = count;
			}
			return result;
		}
	}

	public static ValueTask<int> CountAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source.WithCancellation(cancellationToken), predicate);
		static async ValueTask<int> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, bool> func)
		{
			int count = 0;
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				if (func(item))
				{
					count = checked(count + 1);
				}
			}
			return count;
		}
	}

	public static ValueTask<int> CountAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<int> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, CancellationToken cancellationToken2 = default(CancellationToken))
		{
			int count = 0;
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				if (await func(item, cancellationToken2))
				{
					count = checked(count + 1);
				}
			}
			return count;
		}
	}

	public static ValueTask<long> LongCountAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, cancellationToken);
		static async ValueTask<long> Impl(IAsyncEnumerable<TSource> asyncEnumerable, CancellationToken cancellationToken2 = default(CancellationToken))
		{
			long result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				long count = 0L;
				while (await e.MoveNextAsync())
				{
					count++;
				}
				result = count;
			}
			return result;
		}
	}

	public static ValueTask<long> LongCountAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source.WithCancellation(cancellationToken), predicate);
		static async ValueTask<long> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, bool> func)
		{
			long count = 0L;
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				if (func(item))
				{
					count++;
				}
			}
			return count;
		}
	}

	public static ValueTask<long> LongCountAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<long> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, CancellationToken cancellationToken2 = default(CancellationToken))
		{
			long count = 0L;
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				if (await func(item, cancellationToken2))
				{
					count++;
				}
			}
			return count;
		}
	}

	public static IAsyncEnumerable<KeyValuePair<TKey, int>> CountBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, keyComparer, default(CancellationToken));
		}
		return Empty<KeyValuePair<TKey, int>>();
		static async IAsyncEnumerable<KeyValuePair<TKey, int>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			checked
			{
				await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
				if (await e.MoveNextAsync())
				{
					Dictionary<TKey, int> countsBy = new Dictionary<TKey, int>(comparer);
					do
					{
						TSource current = e.Current;
						CollectionsMarshal.GetValueRefOrAddDefault(countsBy, func(current), out var _)++;
					}
					while (await e.MoveNextAsync());
					foreach (KeyValuePair<TKey, int> item in countsBy)
					{
						yield return item;
					}
				}
			}
		}
	}

	public static IAsyncEnumerable<KeyValuePair<TKey, int>> CountBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, keyComparer, default(CancellationToken));
		}
		return Empty<KeyValuePair<TKey, int>>();
		static async IAsyncEnumerable<KeyValuePair<TKey, int>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func, IEqualityComparer<TKey> comparer, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			checked
			{
				await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
				if (await e.MoveNextAsync())
				{
					Dictionary<TKey, int> countsBy = new Dictionary<TKey, int>(comparer);
					do
					{
						TSource current = e.Current;
						CollectionsMarshal.GetValueRefOrAddDefault(countsBy, await func(current, cancellationToken), out var _)++;
					}
					while (await e.MoveNextAsync());
					foreach (KeyValuePair<TKey, int> item in countsBy)
					{
						yield return item;
					}
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource?> DefaultIfEmpty<TSource>(this IAsyncEnumerable<TSource> source)
	{
		return source.DefaultIfEmpty(default(TSource));
	}

	public static IAsyncEnumerable<TSource> DefaultIfEmpty<TSource>(this IAsyncEnumerable<TSource> source, TSource defaultValue)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, defaultValue, default(CancellationToken));
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, TSource val, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				do
				{
					yield return e.Current;
				}
				while (await e.MoveNextAsync());
			}
			else
			{
				yield return val;
			}
		}
	}

	public static IAsyncEnumerable<TSource> Distinct<TSource>(this IAsyncEnumerable<TSource> source, IEqualityComparer<TSource>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, IEqualityComparer<TSource> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				HashSet<TSource> set = new HashSet<TSource>(comparer2);
				do
				{
					TSource current = e.Current;
					if (set.Add(current))
					{
						yield return current;
					}
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static IAsyncEnumerable<TSource> DistinctBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				HashSet<TKey> set = new HashSet<TKey>(comparer2);
				do
				{
					TSource current = e.Current;
					if (set.Add(func(current)))
					{
						yield return current;
					}
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static IAsyncEnumerable<TSource> DistinctBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				HashSet<TKey> set = new HashSet<TKey>(comparer2);
				do
				{
					TSource element = e.Current;
					HashSet<TKey> hashSet = set;
					if (hashSet.Add(await func(element, cancellationToken)))
					{
						yield return element;
					}
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static ValueTask<TSource> ElementAtAsync<TSource>(this IAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ElementAtOrDefaultAsync(source, index, throwIfNotFound: true, cancellationToken);
	}

	public static ValueTask<TSource?> ElementAtOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, int index, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ElementAtOrDefaultAsync(source, index, throwIfNotFound: false, cancellationToken);
	}

	public static ValueTask<TSource> ElementAtAsync<TSource>(this IAsyncEnumerable<TSource> source, Index index, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!index.IsFromEnd)
		{
			return source.ElementAtAsync(index.Value, cancellationToken);
		}
		ArgumentNullException.ThrowIfNull(source, "source");
		return ElementAtFromEndOrDefault(source, index.Value, throwIfNotFound: true, cancellationToken);
	}

	public static ValueTask<TSource?> ElementAtOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Index index, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!index.IsFromEnd)
		{
			return source.ElementAtOrDefaultAsync(index.Value, cancellationToken);
		}
		ArgumentNullException.ThrowIfNull(source, "source");
		return ElementAtFromEndOrDefault(source, index.Value, throwIfNotFound: false, cancellationToken);
	}

	private static async ValueTask<TSource> ElementAtOrDefaultAsync<TSource>(IAsyncEnumerable<TSource> source, int index, bool throwIfNotFound, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (index >= 0)
		{
			await using IAsyncEnumerator<TSource> e = source.GetAsyncEnumerator(cancellationToken);
			while (await e.MoveNextAsync())
			{
				if (index == 0)
				{
					return e.Current;
				}
				index--;
			}
		}
		if (throwIfNotFound)
		{
			System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("index");
		}
		return default(TSource);
	}

	private static async ValueTask<TSource> ElementAtFromEndOrDefault<TSource>(IAsyncEnumerable<TSource> source, int indexFromEnd, bool throwIfNotFound, CancellationToken cancellationToken)
	{
		if (indexFromEnd > 0)
		{
			await using IAsyncEnumerator<TSource> e = source.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				Queue<TSource> queue = new Queue<TSource>();
				queue.Enqueue(e.Current);
				while (await e.MoveNextAsync())
				{
					if (queue.Count == indexFromEnd)
					{
						queue.Dequeue();
					}
					queue.Enqueue(e.Current);
				}
				if (queue.Count == indexFromEnd)
				{
					return queue.Dequeue();
				}
			}
		}
		if (throwIfNotFound)
		{
			System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("index");
		}
		return default(TSource);
	}

	public static IAsyncEnumerable<TResult> Empty<TResult>()
	{
		return EmptyAsyncEnumerable<TResult>.Instance;
	}

	private static bool IsKnownEmpty<TResult>(this IAsyncEnumerable<TResult> source)
	{
		return source == EmptyAsyncEnumerable<TResult>.Instance;
	}

	public static IAsyncEnumerable<TSource> Except<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		if (!first.IsKnownEmpty())
		{
			return Impl(first, second, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, IAsyncEnumerable<TSource> source, IEqualityComparer<TSource> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> firstEnumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await firstEnumerator.MoveNextAsync())
				{
					HashSet<TSource> set = new HashSet<TSource>(comparer2);
					await foreach (TSource item in source.WithCancellation(cancellationToken))
					{
						set.Add(item);
					}
					do
					{
						TSource current2 = firstEnumerator.Current;
						if (set.Add(current2))
						{
							yield return current2;
						}
					}
					while (await firstEnumerator.MoveNextAsync());
					yield break;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<TSource> ExceptBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!first.IsKnownEmpty())
		{
			return Impl(first, second, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, IAsyncEnumerable<TKey> source, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> firstEnumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await firstEnumerator.MoveNextAsync())
				{
					HashSet<TKey> set = new HashSet<TKey>(comparer2);
					await foreach (TKey item in source.WithCancellation(cancellationToken))
					{
						set.Add(item);
					}
					do
					{
						TSource current2 = firstEnumerator.Current;
						if (set.Add(func(current2)))
						{
							yield return current2;
						}
					}
					while (await firstEnumerator.MoveNextAsync());
					yield break;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<TSource> ExceptBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!first.IsKnownEmpty())
		{
			return Impl(first, second, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, IAsyncEnumerable<TKey> source, Func<TSource, CancellationToken, ValueTask<TKey>> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> firstEnumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await firstEnumerator.MoveNextAsync())
				{
					HashSet<TKey> set = new HashSet<TKey>(comparer2);
					await foreach (TKey item in source.WithCancellation(cancellationToken))
					{
						set.Add(item);
					}
					do
					{
						TSource firstElement = firstEnumerator.Current;
						HashSet<TKey> hashSet = set;
						if (hashSet.Add(await func(firstElement, cancellationToken)))
						{
							yield return firstElement;
						}
					}
					while (await firstEnumerator.MoveNextAsync());
					yield break;
				}
			}
			throw null;
		}
	}

	public static ValueTask<TSource> FirstAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, CancellationToken cancellationToken2)
		{
			TSource current;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					System.Linq.ThrowHelper.ThrowNoElementsException();
				}
				current = e.Current;
			}
			return current;
		}
	}

	public static ValueTask<TSource> FirstAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source.WithCancellation(cancellationToken), predicate);
		static async ValueTask<TSource> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, bool> func)
		{
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				if (func(item))
				{
					return item;
				}
			}
			System.Linq.ThrowHelper.ThrowNoElementsException();
			return default(TSource);
		}
	}

	public static ValueTask<TSource> FirstAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, CancellationToken cancellationToken2)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				if (await func(item, cancellationToken2))
				{
					return item;
				}
			}
			System.Linq.ThrowHelper.ThrowNoElementsException();
			return default(TSource);
		}
	}

	public static ValueTask<TSource?> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.FirstOrDefaultAsync(default(TSource), cancellationToken);
	}

	public static ValueTask<TSource> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, TSource val, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				result = ((await e.MoveNextAsync()) ? e.Current : val);
			}
			return result;
		}
	}

	public static ValueTask<TSource?> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.FirstOrDefaultAsync(predicate, default(TSource), cancellationToken);
	}

	public static ValueTask<TSource?> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.FirstOrDefaultAsync(predicate, default(TSource), cancellationToken);
	}

	public static ValueTask<TSource> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source.WithCancellation(cancellationToken), predicate, defaultValue);
		static async ValueTask<TSource> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, bool> func, TSource result)
		{
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				if (func(item))
				{
					return item;
				}
			}
			return result;
		}
	}

	public static ValueTask<TSource> FirstOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, TSource result, CancellationToken cancellationToken2)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken2))
			{
				if (await func(item, cancellationToken2))
				{
					return item;
				}
			}
			return result;
		}
	}

	public static IAsyncEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, comparer, default(CancellationToken));
		}
		return Empty<IGrouping<TKey, TSource>>();
		static async IAsyncEnumerable<IGrouping<TKey, TSource>> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, TKey> keySelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			foreach (IGrouping<TKey, TSource> item in await source2.ToLookupAsync(keySelector2, comparer2, cancellationToken))
			{
				yield return item;
			}
		}
	}

	public static IAsyncEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, comparer, default(CancellationToken));
		}
		return Empty<IGrouping<TKey, TSource>>();
		static async IAsyncEnumerable<IGrouping<TKey, TSource>> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			foreach (IGrouping<TKey, TSource> item in await source2.ToLookupAsync(keySelector2, comparer2, cancellationToken))
			{
				yield return item;
			}
		}
	}

	public static IAsyncEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, elementSelector, comparer, default(CancellationToken));
		}
		return Empty<IGrouping<TKey, TElement>>();
		static async IAsyncEnumerable<IGrouping<TKey, TElement>> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, TKey> keySelector2, Func<TSource, TElement> elementSelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			foreach (IGrouping<TKey, TElement> item in await source2.ToLookupAsync(keySelector2, elementSelector2, comparer2, cancellationToken))
			{
				yield return item;
			}
		}
	}

	public static IAsyncEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, elementSelector, comparer, default(CancellationToken));
		}
		return Empty<IGrouping<TKey, TElement>>();
		static async IAsyncEnumerable<IGrouping<TKey, TElement>> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector2, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			foreach (IGrouping<TKey, TElement> item in await source2.ToLookupAsync(keySelector2, elementSelector2, comparer2, cancellationToken))
			{
				yield return item;
			}
		}
	}

	public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, TKey> keySelector2, Func<TKey, IEnumerable<TSource>, TResult> resultSelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (await source2.ToLookupAsync(keySelector2, comparer2, cancellationToken) is AsyncLookup<TKey, TSource> asyncLookup)
			{
				foreach (TResult item in asyncLookup.ApplyResultSelector(resultSelector2))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TKey, IEnumerable<TSource>, CancellationToken, ValueTask<TResult>> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector2, Func<TKey, IEnumerable<TSource>, CancellationToken, ValueTask<TResult>> resultSelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (await source2.ToLookupAsync(keySelector2, comparer2, cancellationToken) is AsyncLookup<TKey, TSource> asyncLookup)
			{
				await foreach (TResult item in asyncLookup.ApplyResultSelector(resultSelector2, cancellationToken))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, elementSelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, TKey> keySelector2, Func<TSource, TElement> elementSelector2, Func<TKey, IEnumerable<TElement>, TResult> resultSelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (await source2.ToLookupAsync(keySelector2, elementSelector2, comparer2, cancellationToken) is AsyncLookup<TKey, TElement> asyncLookup)
			{
				foreach (TResult item in asyncLookup.ApplyResultSelector(resultSelector2))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector, Func<TKey, IEnumerable<TElement>, CancellationToken, ValueTask<TResult>> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, keySelector, elementSelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector2, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector2, Func<TKey, IEnumerable<TElement>, CancellationToken, ValueTask<TResult>> resultSelector2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (await source2.ToLookupAsync(keySelector2, elementSelector2, comparer2, cancellationToken) is AsyncLookup<TKey, TElement> asyncLookup)
			{
				await foreach (TResult item in asyncLookup.ApplyResultSelector(resultSelector2, cancellationToken))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!outer.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> asyncEnumerable, IAsyncEnumerable<TInner> source, Func<TOuter, TKey> func2, Func<TInner, TKey> keySelector, Func<TOuter, IEnumerable<TInner>, TResult> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TOuter> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TInner> lookup = await AsyncLookup<TKey, TInner>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				do
				{
					TOuter current = e.Current;
					yield return func(current, lookup[func2(current)]);
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static IAsyncEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, ValueTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, ValueTask<TKey>> innerKeySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, ValueTask<TResult>> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!outer.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> asyncEnumerable, IAsyncEnumerable<TInner> source, Func<TOuter, CancellationToken, ValueTask<TKey>> func2, Func<TInner, CancellationToken, ValueTask<TKey>> keySelector, Func<TOuter, IEnumerable<TInner>, CancellationToken, ValueTask<TResult>> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TOuter> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TInner> lookup = await AsyncLookup<TKey, TInner>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				do
				{
					TOuter current = e.Current;
					TOuter arg = current;
					AsyncLookup<TKey, TInner> asyncLookup = lookup;
					yield return await func(arg, asyncLookup[await func2(current, cancellationToken)], cancellationToken);
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static IAsyncEnumerable<(int Index, TSource Item)> Index<TSource>(this IAsyncEnumerable<TSource> source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, default(CancellationToken));
		}
		return Empty<(int, TSource)>();
		static async IAsyncEnumerable<(int Index, TSource Item)> Impl(IAsyncEnumerable<TSource> source2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				yield return (Index: num, Item: item);
			}
		}
	}

	public static IAsyncEnumerable<TSource> Intersect<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		if (!first.IsKnownEmpty() && !second.IsKnownEmpty())
		{
			return Impl(first, second, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source, IAsyncEnumerable<TSource> asyncEnumerable, IEqualityComparer<TSource> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			HashSet<TSource> set = default(HashSet<TSource>);
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await e.MoveNextAsync())
				{
					set = new HashSet<TSource>(comparer2);
					do
					{
						set.Add(e.Current);
					}
					while (await e.MoveNextAsync());
				}
			}
			await foreach (TSource item in source.WithCancellation(cancellationToken))
			{
				if (set.Remove(item))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!first.IsKnownEmpty() && !second.IsKnownEmpty())
		{
			return Impl(first, second, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source, IAsyncEnumerable<TKey> asyncEnumerable, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			HashSet<TKey> set = default(HashSet<TKey>);
			await using (IAsyncEnumerator<TKey> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await e.MoveNextAsync())
				{
					set = new HashSet<TKey>(comparer2);
					do
					{
						set.Add(e.Current);
					}
					while (await e.MoveNextAsync());
				}
			}
			await foreach (TSource item in source.WithCancellation(cancellationToken))
			{
				if (set.Remove(func(item)))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> IntersectBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TKey> second, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!first.IsKnownEmpty() && !second.IsKnownEmpty())
		{
			return Impl(first, second, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source, IAsyncEnumerable<TKey> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			HashSet<TKey> set = default(HashSet<TKey>);
			await using (IAsyncEnumerator<TKey> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				if (await e.MoveNextAsync())
				{
					set = new HashSet<TKey>(comparer2);
					do
					{
						set.Add(e.Current);
					}
					while (await e.MoveNextAsync());
				}
			}
			await foreach (TSource element in source.WithCancellation(cancellationToken))
			{
				HashSet<TKey> hashSet = set;
				if (hashSet.Remove(await func(element, cancellationToken)))
				{
					yield return element;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!outer.IsKnownEmpty() && !inner.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> asyncEnumerable, IAsyncEnumerable<TInner> source, Func<TOuter, TKey> func, Func<TInner, TKey> keySelector, Func<TOuter, TInner, TResult> func2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TOuter> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TInner> lookup = await AsyncLookup<TKey, TInner>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				if (lookup.Count != 0)
				{
					do
					{
						TOuter item = e.Current;
						Grouping<TKey, TInner> grouping = lookup.GetGrouping(func(item), create: false);
						if (grouping != null)
						{
							int count = grouping._count;
							TInner[] elements = grouping._elements;
							int i = 0;
							while (i != count)
							{
								yield return func2(item, elements[i]);
								int num = i + 1;
								i = num;
							}
						}
					}
					while (await e.MoveNextAsync());
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, ValueTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, ValueTask<TKey>> innerKeySelector, Func<TOuter, TInner, CancellationToken, ValueTask<TResult>> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!outer.IsKnownEmpty() && !inner.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> asyncEnumerable, IAsyncEnumerable<TInner> source, Func<TOuter, CancellationToken, ValueTask<TKey>> func, Func<TInner, CancellationToken, ValueTask<TKey>> keySelector, Func<TOuter, TInner, CancellationToken, ValueTask<TResult>> func2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TOuter> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TInner> lookup = await AsyncLookup<TKey, TInner>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				if (lookup.Count != 0)
				{
					do
					{
						TOuter item = e.Current;
						AsyncLookup<TKey, TInner> asyncLookup = lookup;
						Grouping<TKey, TInner> grouping = asyncLookup.GetGrouping(await func(item, cancellationToken), create: false);
						if (grouping != null)
						{
							int count = grouping._count;
							TInner[] elements = grouping._elements;
							int i = 0;
							while (i != count)
							{
								yield return await func2(item, elements[i], cancellationToken);
								int num = i + 1;
								i = num;
							}
						}
					}
					while (await e.MoveNextAsync());
				}
			}
		}
	}

	public static ValueTask<TSource> LastAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, CancellationToken cancellationToken2)
		{
			TSource result2;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					System.Linq.ThrowHelper.ThrowNoElementsException();
				}
				TSource result;
				do
				{
					result = e.Current;
				}
				while (await e.MoveNextAsync());
				result2 = result;
			}
			return result2;
		}
	}

	public static ValueTask<TSource> LastAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, bool> func, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						System.Linq.ThrowHelper.ThrowNoMatchException();
						result = default(TSource);
						break;
					}
					TSource current = e.Current;
					if (func(current))
					{
						TSource result2 = current;
						while (await e.MoveNextAsync())
						{
							current = e.Current;
							if (func(current))
							{
								result2 = current;
							}
						}
						result = result2;
						break;
					}
				}
			}
			return result;
		}
	}

	public static ValueTask<TSource> LastAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<bool>> func, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						System.Linq.ThrowHelper.ThrowNoMatchException();
						result = default(TSource);
						break;
					}
					TSource element = e.Current;
					if (await func(element, cancellationToken2))
					{
						TSource result2 = element;
						while (await e.MoveNextAsync())
						{
							element = e.Current;
							if (await func(element, cancellationToken2))
							{
								result2 = element;
							}
						}
						result = result2;
						break;
					}
				}
			}
			return result;
		}
	}

	public static ValueTask<TSource?> LastOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.LastOrDefaultAsync(default(TSource), cancellationToken);
	}

	public static ValueTask<TSource> LastOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, TSource val, CancellationToken cancellationToken2)
		{
			TSource result2;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource result = val;
				if (await e.MoveNextAsync())
				{
					do
					{
						result = e.Current;
					}
					while (await e.MoveNextAsync());
				}
				result2 = result;
			}
			return result2;
		}
	}

	public static ValueTask<TSource?> LastOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.LastOrDefaultAsync(predicate, default(TSource), cancellationToken);
	}

	public static ValueTask<TSource?> LastOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.LastOrDefaultAsync(predicate, default(TSource), cancellationToken);
	}

	public static ValueTask<TSource> LastOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, bool> func, TSource val, CancellationToken cancellationToken2)
		{
			TSource result2;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource result = val;
				while (await e.MoveNextAsync())
				{
					TSource current = e.Current;
					if (func(current))
					{
						result = current;
						while (await e.MoveNextAsync())
						{
							current = e.Current;
							if (func(current))
							{
								result = current;
							}
						}
						break;
					}
				}
				result2 = result;
			}
			return result2;
		}
	}

	public static ValueTask<TSource> LastOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<bool>> func, TSource val, CancellationToken cancellationToken2)
		{
			TSource result2;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource result = val;
				while (await e.MoveNextAsync())
				{
					TSource element = e.Current;
					if (await func(element, cancellationToken2))
					{
						result = element;
						while (await e.MoveNextAsync())
						{
							element = e.Current;
							if (await func(element, cancellationToken2))
							{
								result = element;
							}
						}
						break;
					}
				}
				result2 = result;
			}
			return result2;
		}
	}

	public static IAsyncEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner?, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!outer.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> asyncEnumerable, IAsyncEnumerable<TInner> source, Func<TOuter, TKey> func, Func<TInner, TKey> keySelector, Func<TOuter, TInner, TResult> func2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TOuter> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TInner> innerLookup = await AsyncLookup<TKey, TInner>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				do
				{
					TOuter item = e.Current;
					Grouping<TKey, TInner> grouping = innerLookup.GetGrouping(func(item), create: false);
					if (grouping == null)
					{
						yield return func2(item, default(TInner));
					}
					else
					{
						int count = grouping._count;
						TInner[] elements = grouping._elements;
						int i = 0;
						while (i != count)
						{
							yield return func2(item, elements[i]);
							int num = i + 1;
							i = num;
						}
					}
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static IAsyncEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, ValueTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, ValueTask<TKey>> innerKeySelector, Func<TOuter, TInner?, CancellationToken, ValueTask<TResult>> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!outer.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> asyncEnumerable, IAsyncEnumerable<TInner> source, Func<TOuter, CancellationToken, ValueTask<TKey>> func, Func<TInner, CancellationToken, ValueTask<TKey>> keySelector, Func<TOuter, TInner, CancellationToken, ValueTask<TResult>> func2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TOuter> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TInner> innerLookup = await AsyncLookup<TKey, TInner>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				do
				{
					TOuter item = e.Current;
					AsyncLookup<TKey, TInner> asyncLookup = innerLookup;
					Grouping<TKey, TInner> grouping = asyncLookup.GetGrouping(await func(item, cancellationToken), create: false);
					if (grouping == null)
					{
						yield return await func2(item, default(TInner), cancellationToken);
					}
					else
					{
						int count = grouping._count;
						TInner[] elements = grouping._elements;
						int i = 0;
						while (i != count)
						{
							yield return await func2(item, elements[i], cancellationToken);
							int num = i + 1;
							i = num;
						}
					}
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static ValueTask<TSource?> MaxAsync<TSource>(this IAsyncEnumerable<TSource> source, IComparer<TSource>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (comparer == null)
		{
			comparer = Comparer<TSource>.Default;
		}
		if (typeof(TSource) == typeof(float) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)((IAsyncEnumerable<float>)source).MaxAsync(cancellationToken);
		}
		if (typeof(TSource) == typeof(double) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)((IAsyncEnumerable<double>)source).MaxAsync(cancellationToken);
		}
		if (typeof(TSource) == typeof(float?) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)MaxAsync((IAsyncEnumerable<float?>)source, cancellationToken);
		}
		if (typeof(TSource) == typeof(double?) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)MaxAsync((IAsyncEnumerable<double?>)source, cancellationToken);
		}
		return Impl(source, comparer, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, IComparer<TSource> comparer2, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource value = default(TSource);
				if (default(TSource) != null)
				{
					if (!(await e.MoveNextAsync()))
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
					}
					value = e.Current;
					if (comparer2 == Comparer<TSource>.Default)
					{
						while (await e.MoveNextAsync())
						{
							TSource current = e.Current;
							if (Comparer<TSource>.Default.Compare(current, value) > 0)
							{
								value = current;
							}
						}
					}
					else
					{
						while (await e.MoveNextAsync())
						{
							TSource current2 = e.Current;
							if (comparer2.Compare(current2, value) > 0)
							{
								value = current2;
							}
						}
					}
					goto IL_0367;
				}
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						result = value;
						break;
					}
					value = e.Current;
					if (value == null)
					{
						continue;
					}
					while (await e.MoveNextAsync())
					{
						TSource current3 = e.Current;
						if (current3 != null && comparer2.Compare(current3, value) > 0)
						{
							value = current3;
						}
					}
					goto IL_0367;
				}
				goto end_IL_0052;
				IL_0367:
				result = value;
				end_IL_0052:;
			}
			return result;
		}
	}

	private static async ValueTask<float> MaxAsync(this IAsyncEnumerable<float> source, CancellationToken cancellationToken)
	{
		float result;
		await using (IAsyncEnumerator<float> e = source.GetAsyncEnumerator(cancellationToken))
		{
			if (!(await e.MoveNextAsync()))
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			float value = e.Current;
			while (true)
			{
				if (float.IsNaN(value))
				{
					if (!(await e.MoveNextAsync()))
					{
						result = value;
						break;
					}
					value = e.Current;
					continue;
				}
				while (await e.MoveNextAsync())
				{
					float current = e.Current;
					if (current > value)
					{
						value = current;
					}
				}
				result = value;
				break;
			}
		}
		return result;
	}

	private static async ValueTask<double> MaxAsync(this IAsyncEnumerable<double> source, CancellationToken cancellationToken)
	{
		double result;
		await using (IAsyncEnumerator<double> e = source.GetAsyncEnumerator(cancellationToken))
		{
			if (!(await e.MoveNextAsync()))
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			double value = e.Current;
			while (true)
			{
				if (double.IsNaN(value))
				{
					if (!(await e.MoveNextAsync()))
					{
						result = value;
						break;
					}
					value = e.Current;
					continue;
				}
				while (await e.MoveNextAsync())
				{
					double current = e.Current;
					if (current > value)
					{
						value = current;
					}
				}
				result = value;
				break;
			}
		}
		return result;
	}

	private static async ValueTask<float?> MaxAsync(IAsyncEnumerable<float?> source, CancellationToken cancellationToken)
	{
		float? value = null;
		await foreach (float? item in source.WithCancellation(cancellationToken))
		{
			if (item.HasValue && (!value.HasValue || item > value || float.IsNaN(value.Value)))
			{
				value = item;
			}
		}
		return value;
	}

	private static async ValueTask<double?> MaxAsync(IAsyncEnumerable<double?> source, CancellationToken cancellationToken)
	{
		double? value = null;
		await foreach (double? item in source.WithCancellation(cancellationToken))
		{
			if (item.HasValue && (!value.HasValue || item > value || double.IsNaN(value.Value)))
			{
				value = item;
			}
		}
		return value;
	}

	public static ValueTask<TSource?> MaxByAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source, keySelector, comparer ?? Comparer<TKey>.Default, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TKey> func, IComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource value;
				TKey key;
				if (!(await e.MoveNextAsync()))
				{
					if (default(TSource) != null)
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
					}
					result = default(TSource);
				}
				else
				{
					value = e.Current;
					key = func(value);
					if (default(TKey) != null)
					{
						if (comparer2 == Comparer<TKey>.Default)
						{
							while (await e.MoveNextAsync())
							{
								TSource current = e.Current;
								TKey val = func(current);
								if (Comparer<TKey>.Default.Compare(val, key) > 0)
								{
									key = val;
									value = current;
								}
							}
						}
						else
						{
							while (await e.MoveNextAsync())
							{
								TSource current2 = e.Current;
								TKey val2 = func(current2);
								if (comparer2.Compare(val2, key) > 0)
								{
									key = val2;
									value = current2;
								}
							}
						}
						goto IL_0414;
					}
					if (key != null)
					{
						goto IL_023d;
					}
					TSource firstValue = value;
					while (true)
					{
						if (!(await e.MoveNextAsync()))
						{
							result = firstValue;
							break;
						}
						value = e.Current;
						key = func(value);
						if (key == null)
						{
							continue;
						}
						goto IL_023d;
					}
				}
				goto end_IL_0052;
				IL_023d:
				while (await e.MoveNextAsync())
				{
					TSource current3 = e.Current;
					TKey val3 = func(current3);
					if (val3 != null && comparer2.Compare(val3, key) > 0)
					{
						key = val3;
						value = current3;
					}
				}
				goto IL_0414;
				IL_0414:
				result = value;
				end_IL_0052:;
			}
			return result;
		}
	}

	public static ValueTask<TSource?> MaxByAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source, keySelector, comparer ?? Comparer<TKey>.Default, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func, IComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource value;
				TKey key;
				if (!(await e.MoveNextAsync()))
				{
					if (default(TSource) != null)
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
					}
					result = default(TSource);
				}
				else
				{
					value = e.Current;
					key = await func(value, cancellationToken2);
					TSource firstValue;
					if (default(TKey) != null)
					{
						if (comparer2 == Comparer<TKey>.Default)
						{
							while (await e.MoveNextAsync())
							{
								firstValue = e.Current;
								TKey val = await func(firstValue, cancellationToken2);
								if (Comparer<TKey>.Default.Compare(val, key) > 0)
								{
									key = val;
									value = firstValue;
								}
							}
						}
						else
						{
							while (await e.MoveNextAsync())
							{
								firstValue = e.Current;
								TKey val2 = await func(firstValue, cancellationToken2);
								if (comparer2.Compare(val2, key) > 0)
								{
									key = val2;
									value = firstValue;
								}
							}
						}
						goto IL_066b;
					}
					if (key != null)
					{
						goto IL_039c;
					}
					firstValue = value;
					while (true)
					{
						if (!(await e.MoveNextAsync()))
						{
							result = firstValue;
							break;
						}
						value = e.Current;
						key = await func(value, cancellationToken2);
						if (key == null)
						{
							continue;
						}
						goto IL_039c;
					}
				}
				goto end_IL_0068;
				IL_039c:
				while (await e.MoveNextAsync())
				{
					TSource firstValue = e.Current;
					TKey val3 = await func(firstValue, cancellationToken2);
					if (val3 != null && comparer2.Compare(val3, key) > 0)
					{
						key = val3;
						value = firstValue;
					}
				}
				goto IL_066b;
				IL_066b:
				result = value;
				end_IL_0068:;
			}
			return result;
		}
	}

	public static ValueTask<TSource?> MinAsync<TSource>(this IAsyncEnumerable<TSource> source, IComparer<TSource>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (comparer == null)
		{
			comparer = Comparer<TSource>.Default;
		}
		if (typeof(TSource) == typeof(float) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)MinAsync((IAsyncEnumerable<float>)source, cancellationToken);
		}
		if (typeof(TSource) == typeof(double) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)MinAsync((IAsyncEnumerable<double>)source, cancellationToken);
		}
		if (typeof(TSource) == typeof(float?) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)MinAsync((IAsyncEnumerable<float?>)source, cancellationToken);
		}
		if (typeof(TSource) == typeof(double?) && comparer == Comparer<TSource>.Default)
		{
			return (ValueTask<TSource>)(object)MinAsync((IAsyncEnumerable<double?>)source, cancellationToken);
		}
		return Impl(source, comparer, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, IComparer<TSource> comparer2, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource value = default(TSource);
				if (default(TSource) != null)
				{
					if (!(await e.MoveNextAsync()))
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
					}
					value = e.Current;
					if (comparer2 == Comparer<TSource>.Default)
					{
						while (await e.MoveNextAsync())
						{
							TSource current = e.Current;
							if (Comparer<TSource>.Default.Compare(current, value) < 0)
							{
								value = current;
							}
						}
					}
					else
					{
						while (await e.MoveNextAsync())
						{
							TSource current2 = e.Current;
							if (comparer2.Compare(current2, value) < 0)
							{
								value = current2;
							}
						}
					}
					goto IL_0367;
				}
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						result = value;
						break;
					}
					value = e.Current;
					if (value == null)
					{
						continue;
					}
					while (await e.MoveNextAsync())
					{
						TSource current3 = e.Current;
						if (current3 != null && comparer2.Compare(current3, value) < 0)
						{
							value = current3;
						}
					}
					goto IL_0367;
				}
				goto end_IL_0052;
				IL_0367:
				result = value;
				end_IL_0052:;
			}
			return result;
		}
	}

	private static async ValueTask<float> MinAsync(IAsyncEnumerable<float> source, CancellationToken cancellationToken)
	{
		float result;
		await using (IAsyncEnumerator<float> e = source.GetAsyncEnumerator(cancellationToken))
		{
			if (!(await e.MoveNextAsync()))
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			float value = e.Current;
			if (float.IsNaN(value))
			{
				result = value;
			}
			else
			{
				while (true)
				{
					if (await e.MoveNextAsync())
					{
						float current = e.Current;
						if (current < value)
						{
							value = current;
						}
						else if (float.IsNaN(current))
						{
							result = current;
							break;
						}
						continue;
					}
					result = value;
					break;
				}
			}
		}
		return result;
	}

	private static async ValueTask<double> MinAsync(IAsyncEnumerable<double> source, CancellationToken cancellationToken)
	{
		double result;
		await using (IAsyncEnumerator<double> e = source.GetAsyncEnumerator(cancellationToken))
		{
			if (!(await e.MoveNextAsync()))
			{
				System.Linq.ThrowHelper.ThrowNoElementsException();
			}
			double value = e.Current;
			if (double.IsNaN(value))
			{
				result = value;
			}
			else
			{
				while (true)
				{
					if (await e.MoveNextAsync())
					{
						double current = e.Current;
						if (current < value)
						{
							value = current;
						}
						else if (double.IsNaN(current))
						{
							result = current;
							break;
						}
						continue;
					}
					result = value;
					break;
				}
			}
		}
		return result;
	}

	private static async ValueTask<float?> MinAsync(IAsyncEnumerable<float?> source, CancellationToken cancellationToken)
	{
		float? value = null;
		await foreach (float? item in source.WithCancellation(cancellationToken))
		{
			if (item.HasValue && (!value.HasValue || item < value || float.IsNaN(item.GetValueOrDefault())))
			{
				value = item;
			}
		}
		return value;
	}

	private static async ValueTask<double?> MinAsync(IAsyncEnumerable<double?> source, CancellationToken cancellationToken)
	{
		double? value = null;
		await foreach (double? item in source.WithCancellation(cancellationToken))
		{
			if (item.HasValue && (!value.HasValue || item < value || double.IsNaN(item.GetValueOrDefault())))
			{
				value = item;
			}
		}
		return value;
	}

	public static ValueTask<TSource?> MinByAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source, keySelector, comparer ?? Comparer<TKey>.Default, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, TKey> func, IComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource value;
				TKey key;
				if (!(await e.MoveNextAsync()))
				{
					if (default(TSource) != null)
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
					}
					result = default(TSource);
				}
				else
				{
					value = e.Current;
					key = func(value);
					if (default(TKey) != null)
					{
						if (comparer2 == Comparer<TKey>.Default)
						{
							while (await e.MoveNextAsync())
							{
								TSource current = e.Current;
								TKey val = func(current);
								if (Comparer<TKey>.Default.Compare(val, key) < 0)
								{
									key = val;
									value = current;
								}
							}
						}
						else
						{
							while (await e.MoveNextAsync())
							{
								TSource current2 = e.Current;
								TKey val2 = func(current2);
								if (comparer2.Compare(val2, key) < 0)
								{
									key = val2;
									value = current2;
								}
							}
						}
						goto IL_0414;
					}
					if (key != null)
					{
						goto IL_023d;
					}
					TSource firstValue = value;
					while (true)
					{
						if (!(await e.MoveNextAsync()))
						{
							result = firstValue;
							break;
						}
						value = e.Current;
						key = func(value);
						if (key == null)
						{
							continue;
						}
						goto IL_023d;
					}
				}
				goto end_IL_0052;
				IL_023d:
				while (await e.MoveNextAsync())
				{
					TSource current3 = e.Current;
					TKey val3 = func(current3);
					if (val3 != null && comparer2.Compare(val3, key) < 0)
					{
						key = val3;
						value = current3;
					}
				}
				goto IL_0414;
				IL_0414:
				result = value;
				end_IL_0052:;
			}
			return result;
		}
	}

	public static ValueTask<TSource?> MinByAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source, keySelector, comparer ?? Comparer<TKey>.Default, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func, IComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				TSource value;
				TKey key;
				if (!(await e.MoveNextAsync()))
				{
					if (default(TSource) != null)
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
					}
					result = default(TSource);
				}
				else
				{
					value = e.Current;
					key = await func(value, cancellationToken2);
					TSource firstValue;
					if (default(TKey) != null)
					{
						if (comparer2 == Comparer<TKey>.Default)
						{
							while (await e.MoveNextAsync())
							{
								firstValue = e.Current;
								TKey val = await func(firstValue, cancellationToken2);
								if (Comparer<TKey>.Default.Compare(val, key) < 0)
								{
									key = val;
									value = firstValue;
								}
							}
						}
						else
						{
							while (await e.MoveNextAsync())
							{
								firstValue = e.Current;
								TKey val2 = await func(firstValue, cancellationToken2);
								if (comparer2.Compare(val2, key) < 0)
								{
									key = val2;
									value = firstValue;
								}
							}
						}
						goto IL_066b;
					}
					if (key != null)
					{
						goto IL_039c;
					}
					firstValue = value;
					while (true)
					{
						if (!(await e.MoveNextAsync()))
						{
							result = firstValue;
							break;
						}
						value = e.Current;
						key = await func(value, cancellationToken2);
						if (key == null)
						{
							continue;
						}
						goto IL_039c;
					}
				}
				goto end_IL_0068;
				IL_039c:
				while (await e.MoveNextAsync())
				{
					TSource firstValue = e.Current;
					TKey val3 = await func(firstValue, cancellationToken2);
					if (val3 != null && comparer2.Compare(val3, key) < 0)
					{
						key = val3;
						value = firstValue;
					}
				}
				goto IL_066b;
				IL_066b:
				result = value;
				end_IL_0068:;
			}
			return result;
		}
	}

	public static IAsyncEnumerable<TResult> OfType<TResult>(this IAsyncEnumerable<object?> source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<object> source2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (object item in source2.WithCancellation(cancellationToken))
			{
				if (item is TResult)
				{
					yield return (TResult)item;
				}
			}
		}
	}

	public static IOrderedAsyncEnumerable<T> Order<T>(this IAsyncEnumerable<T> source, IComparer<T>? comparer = null)
	{
		return source.OrderBy(EnumerableSorter<T>.IdentityFunc, comparer);
	}

	public static IOrderedAsyncEnumerable<TSource> OrderBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return new OrderedIterator<TSource, TKey>(source, keySelector, comparer, descending: false, null);
		}
		return EmptyAsyncEnumerable<TSource>.Instance;
	}

	public static IOrderedAsyncEnumerable<TSource> OrderBy<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return new OrderedIterator<TSource, TKey>(source, keySelector, comparer, descending: false, null);
		}
		return EmptyAsyncEnumerable<TSource>.Instance;
	}

	public static IOrderedAsyncEnumerable<T> OrderDescending<T>(this IAsyncEnumerable<T> source, IComparer<T>? comparer = null)
	{
		return source.OrderByDescending(EnumerableSorter<T>.IdentityFunc, comparer);
	}

	public static IOrderedAsyncEnumerable<TSource> OrderByDescending<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return new OrderedIterator<TSource, TKey>(source, keySelector, comparer, descending: true, null);
		}
		return EmptyAsyncEnumerable<TSource>.Instance;
	}

	public static IOrderedAsyncEnumerable<TSource> OrderByDescending<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!source.IsKnownEmpty())
		{
			return new OrderedIterator<TSource, TKey>(source, keySelector, comparer, descending: true, null);
		}
		return EmptyAsyncEnumerable<TSource>.Instance;
	}

	public static IOrderedAsyncEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return source.CreateOrderedAsyncEnumerable(keySelector, comparer, descending: false);
	}

	public static IOrderedAsyncEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return source.CreateOrderedAsyncEnumerable(keySelector, comparer, descending: false);
	}

	public static IOrderedAsyncEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return source.CreateOrderedAsyncEnumerable(keySelector, comparer, descending: true);
	}

	public static IOrderedAsyncEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return source.CreateOrderedAsyncEnumerable(keySelector, comparer, descending: true);
	}

	public static IAsyncEnumerable<TSource> Prepend<TSource>(this IAsyncEnumerable<TSource> source, TSource element)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, element, default(CancellationToken));
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, TSource val, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			yield return val;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				yield return item;
			}
		}
	}

	public static IAsyncEnumerable<int> Range(int start, int count)
	{
		if (count == 0)
		{
			return Empty<int>();
		}
		if (count < 0 || (long)start + (long)count - 1 > int.MaxValue)
		{
			System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("count");
		}
		return Impl(start, count);
		static async IAsyncEnumerable<int> Impl(int num2, int num)
		{
			for (int i = 0; i < num; i++)
			{
				yield return num2 + i;
			}
		}
	}

	public static IAsyncEnumerable<TResult> Repeat<TResult>(TResult element, int count)
	{
		if (count == 0)
		{
			return Empty<TResult>();
		}
		System.Linq.ThrowHelper.ThrowIfNegative(count, "count");
		return Impl(element, count);
		static async IAsyncEnumerable<TResult> Impl(TResult val, int num)
		{
			while (num-- != 0)
			{
				yield return val;
			}
		}
	}

	public static IAsyncEnumerable<TSource> Reverse<TSource>(this IAsyncEnumerable<TSource> source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			TSource[] array = await source2.ToArrayAsync(cancellationToken);
			for (int i = array.Length - 1; i >= 0; i--)
			{
				yield return array[i];
			}
		}
	}

	public static IAsyncEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!inner.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> source, IAsyncEnumerable<TInner> asyncEnumerable, Func<TOuter, TKey> keySelector, Func<TInner, TKey> func, Func<TOuter, TInner, TResult> func2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TInner> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TOuter> outerLookup = await AsyncLookup<TKey, TOuter>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				do
				{
					TInner item = e.Current;
					Grouping<TKey, TOuter> grouping = outerLookup.GetGrouping(func(item), create: false);
					if (grouping == null)
					{
						yield return func2(default(TOuter), item);
					}
					else
					{
						int count = grouping._count;
						TOuter[] elements = grouping._elements;
						int i = 0;
						while (i != count)
						{
							yield return func2(elements[i], item);
							int num = i + 1;
							i = num;
						}
					}
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static IAsyncEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IAsyncEnumerable<TOuter> outer, IAsyncEnumerable<TInner> inner, Func<TOuter, CancellationToken, ValueTask<TKey>> outerKeySelector, Func<TInner, CancellationToken, ValueTask<TKey>> innerKeySelector, Func<TOuter?, TInner, CancellationToken, ValueTask<TResult>> resultSelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(outer, "outer");
		ArgumentNullException.ThrowIfNull(inner, "inner");
		ArgumentNullException.ThrowIfNull(outerKeySelector, "outerKeySelector");
		ArgumentNullException.ThrowIfNull(innerKeySelector, "innerKeySelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!inner.IsKnownEmpty())
		{
			return Impl(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TOuter> source, IAsyncEnumerable<TInner> asyncEnumerable, Func<TOuter, CancellationToken, ValueTask<TKey>> keySelector, Func<TInner, CancellationToken, ValueTask<TKey>> func, Func<TOuter, TInner, CancellationToken, ValueTask<TResult>> func2, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TInner> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			if (await e.MoveNextAsync())
			{
				AsyncLookup<TKey, TOuter> outerLookup = await AsyncLookup<TKey, TOuter>.CreateForJoinAsync(source, keySelector, comparer2, cancellationToken);
				do
				{
					TInner item = e.Current;
					AsyncLookup<TKey, TOuter> asyncLookup = outerLookup;
					Grouping<TKey, TOuter> grouping = asyncLookup.GetGrouping(await func(item, cancellationToken), create: false);
					if (grouping == null)
					{
						yield return await func2(default(TOuter), item, cancellationToken);
					}
					else
					{
						int count = grouping._count;
						TOuter[] elements = grouping._elements;
						int i = 0;
						while (i != count)
						{
							yield return await func2(elements[i], item, cancellationToken);
							int num = i + 1;
							i = num;
						}
					}
				}
				while (await e.MoveNextAsync());
			}
		}
	}

	public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, TResult> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				yield return func(item);
			}
		}
	}

	public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TResult>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TResult>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				yield return await func(item, cancellationToken);
			}
		}
	}

	public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, TResult> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, TResult> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				yield return func(item, num);
			}
		}
	}

	public static IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<TResult>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, CancellationToken, ValueTask<TResult>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				yield return await func(item, num, cancellationToken);
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, IEnumerable<TResult>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				foreach (TResult item2 in func(item))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<IEnumerable<TResult>>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<IEnumerable<TResult>>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				foreach (TResult item2 in await func(item, cancellationToken))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IAsyncEnumerable<TResult>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, IAsyncEnumerable<TResult>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				await foreach (TResult item2 in func(item).WithCancellation(cancellationToken))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, IEnumerable<TResult>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				foreach (TResult item2 in func(item, num))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<IEnumerable<TResult>>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, CancellationToken, ValueTask<IEnumerable<TResult>>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				foreach (TResult item2 in await func(item, num, cancellationToken))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, IAsyncEnumerable<TResult>> selector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(selector, "selector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, selector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, IAsyncEnumerable<TResult>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				await foreach (TResult item2 in func(item, num).WithCancellation(cancellationToken))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(collectionSelector, "collectionSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, collectionSelector, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, IEnumerable<TCollection>> func, Func<TSource, TCollection, TResult> func2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				foreach (TCollection item in func(element))
				{
					yield return func2(element, item);
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<IEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(collectionSelector, "collectionSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, collectionSelector, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<IEnumerable<TCollection>>> func, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> func2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				foreach (TCollection item in await func(element, cancellationToken))
				{
					yield return await func2(element, item, cancellationToken);
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IAsyncEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(collectionSelector, "collectionSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, collectionSelector, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, IAsyncEnumerable<TCollection>> func, Func<TSource, TCollection, TResult> func2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				await foreach (TCollection item in func(element).WithCancellation(cancellationToken))
				{
					yield return func2(element, item);
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, IAsyncEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(collectionSelector, "collectionSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, collectionSelector, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, IAsyncEnumerable<TCollection>> func, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> func2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				await foreach (TCollection item in func(element).WithCancellation(cancellationToken))
				{
					yield return await func2(element, item, cancellationToken);
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(collectionSelector, "collectionSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, collectionSelector, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, IEnumerable<TCollection>> func, Func<TSource, TCollection, TResult> func2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				foreach (TCollection item in func(element, num))
				{
					yield return func2(element, item);
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<IEnumerable<TCollection>>> collectionSelector, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(collectionSelector, "collectionSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, collectionSelector, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, CancellationToken, ValueTask<IEnumerable<TCollection>>> func, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> func2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				foreach (TCollection item in await func(element, num, cancellationToken))
				{
					yield return await func2(element, item, cancellationToken);
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IAsyncEnumerable<TSource> source, Func<TSource, int, IAsyncEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(collectionSelector, "collectionSelector");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, collectionSelector, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, IAsyncEnumerable<TCollection>> func, Func<TSource, TCollection, CancellationToken, ValueTask<TResult>> func2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				await foreach (TCollection item in func(element, num).WithCancellation(cancellationToken))
				{
					yield return await func2(element, item, cancellationToken);
				}
			}
		}
	}

	public static ValueTask<bool> SequenceEqualAsync<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		return Impl(first, second, comparer ?? EqualityComparer<TSource>.Default, cancellationToken);
		static async ValueTask<bool> Impl(IAsyncEnumerable<TSource> asyncEnumerable, IAsyncEnumerable<TSource> asyncEnumerable2, IEqualityComparer<TSource> equalityComparer, CancellationToken cancellationToken2)
		{
			bool result;
			await using (IAsyncEnumerator<TSource> e1 = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				bool flag;
				await using (IAsyncEnumerator<TSource> e2 = asyncEnumerable2.GetAsyncEnumerator(cancellationToken2))
				{
					while (true)
					{
						if (!(await e1.MoveNextAsync()))
						{
							flag = !(await e2.MoveNextAsync());
							break;
						}
						if (!(await e2.MoveNextAsync()) || !equalityComparer.Equals(e1.Current, e2.Current))
						{
							flag = false;
							break;
						}
					}
				}
				result = flag;
			}
			return result;
		}
	}

	public static IAsyncEnumerable<TSource> Shuffle<TSource>(this IAsyncEnumerable<TSource> source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			TSource[] array = await source2.ToArrayAsync(cancellationToken);
			Random.Shared.Shuffle(array);
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[i];
			}
		}
	}

	public static ValueTask<TSource> SingleAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, CancellationToken cancellationToken2)
		{
			TSource result2;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					System.Linq.ThrowHelper.ThrowNoElementsException();
				}
				TSource result = e.Current;
				if (await e.MoveNextAsync())
				{
					System.Linq.ThrowHelper.ThrowMoreThanOneElementException();
				}
				result2 = result;
			}
			return result2;
		}
	}

	public static ValueTask<TSource> SingleAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, bool> func, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
						result = default(TSource);
						break;
					}
					TSource result2 = e.Current;
					if (func(result2))
					{
						while (await e.MoveNextAsync())
						{
							if (func(e.Current))
							{
								System.Linq.ThrowHelper.ThrowMoreThanOneMatchException();
							}
						}
						result = result2;
						break;
					}
				}
			}
			return result;
		}
	}

	public static ValueTask<TSource> SingleAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<bool>> func, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						System.Linq.ThrowHelper.ThrowNoElementsException();
						result = default(TSource);
						break;
					}
					TSource result2 = e.Current;
					if (await func(result2, cancellationToken2))
					{
						while (await e.MoveNextAsync())
						{
							if (await func(e.Current, cancellationToken2))
							{
								System.Linq.ThrowHelper.ThrowMoreThanOneMatchException();
							}
						}
						result = result2;
						break;
					}
				}
			}
			return result;
		}
	}

	public static ValueTask<TSource?> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SingleOrDefaultAsync(default(TSource), cancellationToken);
	}

	public static ValueTask<TSource> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, TSource val, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					result = val;
				}
				else
				{
					TSource result2 = e.Current;
					if (await e.MoveNextAsync())
					{
						System.Linq.ThrowHelper.ThrowMoreThanOneElementException();
					}
					result = result2;
				}
			}
			return result;
		}
	}

	public static ValueTask<TSource?> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SingleOrDefaultAsync(predicate, default(TSource), cancellationToken);
	}

	public static ValueTask<TSource?> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
	{
		return source.SingleOrDefaultAsync(predicate, default(TSource), cancellationToken);
	}

	public static ValueTask<TSource> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, bool> func, TSource val, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						result = val;
						break;
					}
					TSource result2 = e.Current;
					if (func(result2))
					{
						while (await e.MoveNextAsync())
						{
							if (func(e.Current))
							{
								System.Linq.ThrowHelper.ThrowMoreThanOneMatchException();
							}
						}
						result = result2;
						break;
					}
				}
			}
			return result;
		}
	}

	public static ValueTask<TSource> SingleOrDefaultAsync<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate, TSource defaultValue, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		return Impl(source, predicate, defaultValue, cancellationToken);
		static async ValueTask<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<bool>> func, TSource val, CancellationToken cancellationToken2)
		{
			TSource result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				while (true)
				{
					if (!(await e.MoveNextAsync()))
					{
						result = val;
						break;
					}
					TSource result2 = e.Current;
					if (await func(result2, cancellationToken2))
					{
						while (await e.MoveNextAsync())
						{
							if (await func(e.Current, cancellationToken2))
							{
								System.Linq.ThrowHelper.ThrowMoreThanOneMatchException();
							}
						}
						result = result2;
						break;
					}
				}
			}
			return result;
		}
	}

	public static IAsyncEnumerable<TSource> Skip<TSource>(this IAsyncEnumerable<TSource> source, int count)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty())
		{
			if (count > 0)
			{
				return Impl(source, count, default(CancellationToken));
			}
			return source;
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, int num, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			while (true)
			{
				bool flag = num > 0;
				if (flag)
				{
					flag = await e.MoveNextAsync();
				}
				if (!flag)
				{
					break;
				}
				num--;
			}
			if (num <= 0)
			{
				while (await e.MoveNextAsync())
				{
					yield return e.Current;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> SkipLast<TSource>(this IAsyncEnumerable<TSource> source, int count)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty())
		{
			if (count > 0)
			{
				return TakeRangeFromEndIterator(source, isStartIndexFromEnd: false, 0, isEndIndexFromEnd: true, count, default(CancellationToken));
			}
			return source;
		}
		return Empty<TSource>();
	}

	public static IAsyncEnumerable<TSource> SkipWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, bool> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				TSource current;
				do
				{
					if (!(await e.MoveNextAsync()))
					{
						yield break;
					}
					current = e.Current;
				}
				while (func(current));
				yield return current;
				while (await e.MoveNextAsync())
				{
					yield return e.Current;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<TSource> SkipWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<bool>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				TSource element;
				do
				{
					if (!(await e.MoveNextAsync()))
					{
						yield break;
					}
					element = e.Current;
				}
				while (await func(element, cancellationToken));
				yield return element;
				while (await e.MoveNextAsync())
				{
					yield return e.Current;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<TSource> SkipWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, int, bool> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				int index = -1;
				TSource current;
				int num;
				do
				{
					if (!(await e.MoveNextAsync()))
					{
						yield break;
					}
					current = e.Current;
					num = checked(index + 1);
					index = num;
				}
				while (func(current, num));
				yield return current;
				while (await e.MoveNextAsync())
				{
					yield return e.Current;
				}
			}
			throw null;
		}
	}

	public static IAsyncEnumerable<TSource> SkipWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<bool>> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, int, CancellationToken, ValueTask<bool>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				int index = -1;
				TSource element;
				int num;
				do
				{
					if (!(await e.MoveNextAsync()))
					{
						yield break;
					}
					element = e.Current;
					num = checked(index + 1);
					index = num;
				}
				while (await func(element, num, cancellationToken));
				yield return element;
				while (await e.MoveNextAsync())
				{
					yield return e.Current;
				}
			}
			throw null;
		}
	}

	public static ValueTask<int> SumAsync(this IAsyncEnumerable<int> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<int> Impl(ConfiguredCancelableAsyncEnumerable<int> configuredCancelableAsyncEnumerable)
		{
			int sum = 0;
			await foreach (int item in configuredCancelableAsyncEnumerable)
			{
				sum = checked(sum + item);
			}
			return sum;
		}
	}

	public static ValueTask<long> SumAsync(this IAsyncEnumerable<long> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<long> Impl(ConfiguredCancelableAsyncEnumerable<long> configuredCancelableAsyncEnumerable)
		{
			long sum = 0L;
			await foreach (long item in configuredCancelableAsyncEnumerable)
			{
				sum = checked(sum + item);
			}
			return sum;
		}
	}

	public static ValueTask<float> SumAsync(this IAsyncEnumerable<float> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<float> Impl(ConfiguredCancelableAsyncEnumerable<float> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			await foreach (float item in configuredCancelableAsyncEnumerable)
			{
				sum += (double)item;
			}
			return (float)sum;
		}
	}

	public static ValueTask<double> SumAsync(this IAsyncEnumerable<double> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double> Impl(ConfiguredCancelableAsyncEnumerable<double> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			await foreach (double item in configuredCancelableAsyncEnumerable)
			{
				sum += item;
			}
			return sum;
		}
	}

	public static ValueTask<decimal> SumAsync(this IAsyncEnumerable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<decimal> Impl(ConfiguredCancelableAsyncEnumerable<decimal> configuredCancelableAsyncEnumerable)
		{
			decimal sum = 0m;
			await foreach (decimal item in configuredCancelableAsyncEnumerable)
			{
				sum += item;
			}
			return sum;
		}
	}

	public static ValueTask<int?> SumAsync(this IAsyncEnumerable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<int?> Impl(ConfiguredCancelableAsyncEnumerable<int?> configuredCancelableAsyncEnumerable)
		{
			int sum = 0;
			await foreach (int? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					sum = checked(sum + item.GetValueOrDefault());
				}
			}
			return sum;
		}
	}

	public static ValueTask<long?> SumAsync(this IAsyncEnumerable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<long?> Impl(ConfiguredCancelableAsyncEnumerable<long?> configuredCancelableAsyncEnumerable)
		{
			long sum = 0L;
			await foreach (long? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					sum = checked(sum + item.GetValueOrDefault());
				}
			}
			return sum;
		}
	}

	public static ValueTask<float?> SumAsync(this IAsyncEnumerable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<float?> Impl(ConfiguredCancelableAsyncEnumerable<float?> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			await foreach (float? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					sum += (double)item.GetValueOrDefault();
				}
			}
			return (float)sum;
		}
	}

	public static ValueTask<double?> SumAsync(this IAsyncEnumerable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<double?> Impl(ConfiguredCancelableAsyncEnumerable<double?> configuredCancelableAsyncEnumerable)
		{
			double sum = 0.0;
			await foreach (double? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					sum += item.GetValueOrDefault();
				}
			}
			return sum;
		}
	}

	public static ValueTask<decimal?> SumAsync(this IAsyncEnumerable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<decimal?> Impl(ConfiguredCancelableAsyncEnumerable<decimal?> configuredCancelableAsyncEnumerable)
		{
			decimal sum = 0m;
			await foreach (decimal? item in configuredCancelableAsyncEnumerable)
			{
				if (item.HasValue)
				{
					sum += item.GetValueOrDefault();
				}
			}
			return sum;
		}
	}

	public static IAsyncEnumerable<TSource> Take<TSource>(this IAsyncEnumerable<TSource> source, int count)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty() && count > 0)
		{
			return Impl(source, count, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, int num2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				yield return item;
				int num = num2 - 1;
				num2 = num;
				if (num == 0)
				{
					break;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> Take<TSource>(this IAsyncEnumerable<TSource> source, Range range)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (source.IsKnownEmpty())
		{
			return Empty<TSource>();
		}
		Index start = range.Start;
		Index end = range.End;
		bool isFromEnd = start.IsFromEnd;
		bool isFromEnd2 = end.IsFromEnd;
		int value = start.Value;
		int value2 = end.Value;
		if (isFromEnd)
		{
			if (value == 0 || (isFromEnd2 && value2 >= value))
			{
				return Empty<TSource>();
			}
		}
		else if (!isFromEnd2)
		{
			if (value < value2)
			{
				return Impl(source, value, value2, default(CancellationToken));
			}
			return Empty<TSource>();
		}
		return TakeRangeFromEndIterator(source, isFromEnd, value, isFromEnd2, value2, default(CancellationToken));
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> asyncEnumerable, int startIndex, int endIndex, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken))
			{
				int index = 0;
				while (true)
				{
					bool flag = index < startIndex;
					if (flag)
					{
						flag = await e.MoveNextAsync();
					}
					if (!flag)
					{
						break;
					}
					int num = index + 1;
					index = num;
				}
				if (index >= startIndex)
				{
					while (true)
					{
						bool flag = index < endIndex;
						if (flag)
						{
							flag = await e.MoveNextAsync();
						}
						if (!flag)
						{
							break;
						}
						yield return e.Current;
						int num = index + 1;
						index = num;
					}
					yield break;
				}
			}
			throw null;
		}
	}

	private static async IAsyncEnumerable<TSource> TakeRangeFromEndIterator<TSource>(IAsyncEnumerable<TSource> source, bool isStartIndexFromEnd, int startIndex, bool isEndIndexFromEnd, int endIndex, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		if (isStartIndexFromEnd)
		{
			Queue<TSource> queue = default(Queue<TSource>);
			int count = default(int);
			await using (IAsyncEnumerator<TSource> e = source.GetAsyncEnumerator(cancellationToken))
			{
				if (await e.MoveNextAsync())
				{
					queue = new Queue<TSource>();
					queue.Enqueue(e.Current);
					count = 1;
					while (await e.MoveNextAsync())
					{
						if (count < startIndex)
						{
							queue.Enqueue(e.Current);
							int num = count + 1;
							count = num;
							continue;
						}
						do
						{
							queue.Dequeue();
							queue.Enqueue(e.Current);
							int num = checked(count + 1);
							count = num;
						}
						while (await e.MoveNextAsync());
						break;
					}
				}
			}
			startIndex = CalculateStartIndexFromEnd(startIndex, count);
			endIndex = CalculateEndIndex(isEndIndexFromEnd, endIndex, count);
			for (int i = startIndex; i < endIndex; i++)
			{
				yield return queue.Dequeue();
			}
			yield break;
		}
		await using (IAsyncEnumerator<TSource> e = source.GetAsyncEnumerator(cancellationToken))
		{
			int count = 0;
			while (true)
			{
				bool flag = count < startIndex;
				if (flag)
				{
					flag = await e.MoveNextAsync();
				}
				if (!flag)
				{
					break;
				}
				int num = count + 1;
				count = num;
			}
			if (count != startIndex)
			{
				yield break;
			}
			Queue<TSource> queue = new Queue<TSource>();
			while (await e.MoveNextAsync())
			{
				if (queue.Count == endIndex)
				{
					do
					{
						queue.Enqueue(e.Current);
						yield return queue.Dequeue();
					}
					while (await e.MoveNextAsync());
					break;
				}
				queue.Enqueue(e.Current);
			}
		}
		static int CalculateEndIndex(bool flag2, int num3, int num2)
		{
			return Math.Min(num2, flag2 ? (num2 - num3) : num3);
		}
		static int CalculateStartIndexFromEnd(int num3, int num2)
		{
			return Math.Max(0, num2 - num3);
		}
	}

	public static IAsyncEnumerable<TSource> TakeLast<TSource>(this IAsyncEnumerable<TSource> source, int count)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!source.IsKnownEmpty() && count > 0)
		{
			return TakeRangeFromEndIterator(source, isStartIndexFromEnd: true, count, isEndIndexFromEnd: true, 0, default(CancellationToken));
		}
		return Empty<TSource>();
	}

	public static IAsyncEnumerable<TSource> TakeWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, bool> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				if (!func(item))
				{
					break;
				}
				yield return item;
			}
		}
	}

	public static IAsyncEnumerable<TSource> TakeWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				if (!(await func(element, cancellationToken)))
				{
					break;
				}
				yield return element;
			}
		}
	}

	public static IAsyncEnumerable<TSource> TakeWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, bool> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				if (!func(item, num))
				{
					break;
				}
				yield return item;
			}
		}
	}

	public static IAsyncEnumerable<TSource> TakeWhile<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<bool>> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, CancellationToken, ValueTask<bool>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				if (!(await func(element, num, cancellationToken)))
				{
					break;
				}
				yield return element;
			}
		}
	}

	public static ValueTask<TSource[]> ToArrayAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<TSource[]> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable)
		{
			ConfiguredCancelableAsyncEnumerable<TSource>.Enumerator e = configuredCancelableAsyncEnumerable.GetAsyncEnumerator();
			TSource[] result;
			try
			{
				if (await e.MoveNextAsync())
				{
					List<TSource> list = new List<TSource>();
					do
					{
						list.Add(e.Current);
					}
					while (await e.MoveNextAsync());
					result = list.ToArray();
				}
				else
				{
					result = Array.Empty<TSource>();
				}
			}
			finally
			{
				IAsyncDisposable asyncDisposable = e as IAsyncDisposable;
				if (asyncDisposable != null)
				{
					await asyncDisposable.DisposeAsync();
				}
			}
			return result;
		}
	}

	public static IAsyncEnumerable<TSource> ToAsyncEnumerable<TSource>(this IEnumerable<TSource> source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!(source is TSource[] array))
		{
			if (!(source is List<TSource> source2))
			{
				if (source is IList<TSource> source3)
				{
					return FromIList(source3);
				}
				if (source == Enumerable.Empty<TSource>())
				{
					return Empty<TSource>();
				}
				return FromIterator(source);
			}
			return FromList(source2);
		}
		return (array.Length == 0) ? Empty<TSource>() : FromArray(array);
		static async IAsyncEnumerable<TSource> FromArray(TSource[] array2)
		{
			int i = 0;
			while (true)
			{
				int num = i;
				if ((uint)num >= (uint)array2.Length)
				{
					break;
				}
				yield return array2[num];
				i++;
			}
		}
		static async IAsyncEnumerable<TSource> FromIList(IList<TSource> list)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				yield return list[i];
			}
		}
		static async IAsyncEnumerable<TSource> FromIterator(IEnumerable<TSource> enumerable)
		{
			foreach (TSource item in enumerable)
			{
				yield return item;
			}
		}
		static async IAsyncEnumerable<TSource> FromList(List<TSource> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				yield return list[i];
			}
		}
	}

	public static ValueTask<Dictionary<TKey, TValue>> ToDictionaryAsync<TKey, TValue>(this IAsyncEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken)) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken), comparer);
		static async ValueTask<Dictionary<TKey, TValue>> Impl(ConfiguredCancelableAsyncEnumerable<KeyValuePair<TKey, TValue>> configuredCancelableAsyncEnumerable, IEqualityComparer<TKey> comparer2)
		{
			Dictionary<TKey, TValue> d = new Dictionary<TKey, TValue>(comparer2);
			await foreach (KeyValuePair<TKey, TValue> item in configuredCancelableAsyncEnumerable)
			{
				d.Add(item.Key, item.Value);
			}
			return d;
		}
	}

	public static ValueTask<Dictionary<TKey, TValue>> ToDictionaryAsync<TKey, TValue>(this IAsyncEnumerable<(TKey Key, TValue Value)> source, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken)) where TKey : notnull
	{
		return source.ToDictionaryAsync<(TKey, TValue), TKey, TValue>(((TKey Key, TValue Value) vt) => vt.Key, ((TKey Key, TValue Value) vt) => vt.Value, comparer, cancellationToken);
	}

	public static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken)) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source.WithCancellation(cancellationToken), keySelector, comparer);
		static async ValueTask<Dictionary<TKey, TSource>> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer2)
		{
			Dictionary<TKey, TSource> d = new Dictionary<TKey, TSource>(comparer2);
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				d.Add(func(item), item);
			}
			return d;
		}
	}

	public static ValueTask<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken)) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source, keySelector, comparer, cancellationToken);
		static async ValueTask<Dictionary<TKey, TSource>> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TKey>> func, IEqualityComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			Dictionary<TKey, TSource> d = new Dictionary<TKey, TSource>(comparer2);
			await foreach (TSource element in source2.WithCancellation(cancellationToken2))
			{
				Dictionary<TKey, TSource> dictionary = d;
				dictionary.Add(await func(element, cancellationToken2), element);
			}
			return d;
		}
	}

	public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken)) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		return Impl(source.WithCancellation(cancellationToken), keySelector, elementSelector, comparer);
		static async ValueTask<Dictionary<TKey, TElement>> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, TKey> func, Func<TSource, TElement> func2, IEqualityComparer<TKey> comparer2)
		{
			Dictionary<TKey, TElement> d = new Dictionary<TKey, TElement>(comparer2);
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				d.Add(func(item), func2(item));
			}
			return d;
		}
	}

	public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken)) where TKey : notnull
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		return Impl(source, keySelector, elementSelector, comparer, cancellationToken);
		static async ValueTask<Dictionary<TKey, TElement>> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TKey>> func, Func<TSource, CancellationToken, ValueTask<TElement>> func2, IEqualityComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			Dictionary<TKey, TElement> d = new Dictionary<TKey, TElement>(comparer2);
			await foreach (TSource element in source2.WithCancellation(cancellationToken2))
			{
				Dictionary<TKey, TElement> dictionary = d;
				dictionary.Add(await func(element, cancellationToken2), await func2(element, cancellationToken2));
			}
			return d;
		}
	}

	public static ValueTask<HashSet<TSource>> ToHashSetAsync<TSource>(this IAsyncEnumerable<TSource> source, IEqualityComparer<TSource>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken), comparer);
		static async ValueTask<HashSet<TSource>> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, IEqualityComparer<TSource> comparer2)
		{
			HashSet<TSource> set = new HashSet<TSource>(comparer2);
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				set.Add(item);
			}
			return set;
		}
	}

	public static ValueTask<List<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Impl(source.WithCancellation(cancellationToken));
		static async ValueTask<List<TSource>> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable)
		{
			List<TSource> list = new List<TSource>();
			await foreach (TSource item in configuredCancelableAsyncEnumerable)
			{
				list.Add(item);
			}
			return list;
		}
	}

	public static ValueTask<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source.WithCancellation(cancellationToken), keySelector, comparer);
		static async ValueTask<ILookup<TKey, TSource>> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer2)
		{
			ConfiguredCancelableAsyncEnumerable<TSource>.Enumerator e = configuredCancelableAsyncEnumerable.GetAsyncEnumerator();
			ILookup<TKey, TSource> result;
			try
			{
				if (!(await e.MoveNextAsync()))
				{
					result = EmptyLookup<TKey, TSource>.Instance;
				}
				else
				{
					AsyncLookup<TKey, TSource> lookup = new AsyncLookup<TKey, TSource>(comparer2);
					do
					{
						TSource current = e.Current;
						lookup.GetGrouping(func(current), create: true).Add(current);
					}
					while (await e.MoveNextAsync());
					result = lookup;
				}
			}
			finally
			{
				IAsyncDisposable asyncDisposable = e as IAsyncDisposable;
				if (asyncDisposable != null)
				{
					await asyncDisposable.DisposeAsync();
				}
			}
			return result;
		}
	}

	public static ValueTask<ILookup<TKey, TSource>> ToLookupAsync<TSource, TKey>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		return Impl(source, keySelector, comparer, cancellationToken);
		static async ValueTask<ILookup<TKey, TSource>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func, IEqualityComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			ILookup<TKey, TSource> result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					result = EmptyLookup<TKey, TSource>.Instance;
				}
				else
				{
					AsyncLookup<TKey, TSource> lookup = new AsyncLookup<TKey, TSource>(comparer2);
					do
					{
						TSource item = e.Current;
						AsyncLookup<TKey, TSource> asyncLookup = lookup;
						asyncLookup.GetGrouping(await func(item, cancellationToken2), create: true).Add(item);
					}
					while (await e.MoveNextAsync());
					result = lookup;
				}
			}
			return result;
		}
	}

	public static ValueTask<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		return Impl(source.WithCancellation(cancellationToken), keySelector, elementSelector, comparer);
		static async ValueTask<ILookup<TKey, TElement>> Impl(ConfiguredCancelableAsyncEnumerable<TSource> configuredCancelableAsyncEnumerable, Func<TSource, TKey> func, Func<TSource, TElement> func2, IEqualityComparer<TKey> comparer2)
		{
			ConfiguredCancelableAsyncEnumerable<TSource>.Enumerator e = configuredCancelableAsyncEnumerable.GetAsyncEnumerator();
			ILookup<TKey, TElement> result;
			try
			{
				if (!(await e.MoveNextAsync()))
				{
					result = EmptyLookup<TKey, TElement>.Instance;
				}
				else
				{
					AsyncLookup<TKey, TElement> lookup = new AsyncLookup<TKey, TElement>(comparer2);
					do
					{
						TSource current = e.Current;
						lookup.GetGrouping(func(current), create: true).Add(func2(current));
					}
					while (await e.MoveNextAsync());
					result = lookup;
				}
			}
			finally
			{
				IAsyncDisposable asyncDisposable = e as IAsyncDisposable;
				if (asyncDisposable != null)
				{
					await asyncDisposable.DisposeAsync();
				}
			}
			return result;
		}
	}

	public static ValueTask<ILookup<TKey, TElement>> ToLookupAsync<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, Func<TSource, CancellationToken, ValueTask<TElement>> elementSelector, IEqualityComparer<TKey>? comparer = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		ArgumentNullException.ThrowIfNull(elementSelector, "elementSelector");
		return Impl(source, keySelector, elementSelector, comparer, cancellationToken);
		static async ValueTask<ILookup<TKey, TElement>> Impl(IAsyncEnumerable<TSource> asyncEnumerable, Func<TSource, CancellationToken, ValueTask<TKey>> func, Func<TSource, CancellationToken, ValueTask<TElement>> func2, IEqualityComparer<TKey> comparer2, CancellationToken cancellationToken2)
		{
			ILookup<TKey, TElement> result;
			await using (IAsyncEnumerator<TSource> e = asyncEnumerable.GetAsyncEnumerator(cancellationToken2))
			{
				if (!(await e.MoveNextAsync()))
				{
					result = EmptyLookup<TKey, TElement>.Instance;
				}
				else
				{
					AsyncLookup<TKey, TElement> lookup = new AsyncLookup<TKey, TElement>(comparer2);
					do
					{
						TSource item = e.Current;
						AsyncLookup<TKey, TElement> asyncLookup = lookup;
						Grouping<TKey, TElement> grouping = asyncLookup.GetGrouping(await func(item, cancellationToken2), create: true);
						grouping.Add(await func2(item, cancellationToken2));
					}
					while (await e.MoveNextAsync());
					result = lookup;
				}
			}
			return result;
		}
	}

	public static IAsyncEnumerable<TSource> Union<TSource>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, IEqualityComparer<TSource>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		if (!first.IsKnownEmpty() || !second.IsKnownEmpty())
		{
			return Impl(first, second, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source, IAsyncEnumerable<TSource> source2, IEqualityComparer<TSource> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			HashSet<TSource> set = new HashSet<TSource>(comparer2);
			await foreach (TSource item in source.WithCancellation(cancellationToken))
			{
				if (set.Add(item))
				{
					yield return item;
				}
			}
			await foreach (TSource item2 in source2.WithCancellation(cancellationToken))
			{
				if (set.Add(item2))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> UnionBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!first.IsKnownEmpty() || !second.IsKnownEmpty())
		{
			return Impl(first, second, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source, IAsyncEnumerable<TSource> source2, Func<TSource, TKey> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			HashSet<TKey> set = new HashSet<TKey>(comparer2);
			await foreach (TSource item in source.WithCancellation(cancellationToken))
			{
				if (set.Add(func(item)))
				{
					yield return item;
				}
			}
			await foreach (TSource item2 in source2.WithCancellation(cancellationToken))
			{
				if (set.Add(func(item2)))
				{
					yield return item2;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> UnionBy<TSource, TKey>(this IAsyncEnumerable<TSource> first, IAsyncEnumerable<TSource> second, Func<TSource, CancellationToken, ValueTask<TKey>> keySelector, IEqualityComparer<TKey>? comparer = null)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		if (!first.IsKnownEmpty() || !second.IsKnownEmpty())
		{
			return Impl(first, second, keySelector, comparer, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source, IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<TKey>> func, IEqualityComparer<TKey> comparer2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			HashSet<TKey> set = new HashSet<TKey>(comparer2);
			await foreach (TSource element in source.WithCancellation(cancellationToken))
			{
				HashSet<TKey> hashSet = set;
				if (hashSet.Add(await func(element, cancellationToken)))
				{
					yield return element;
				}
			}
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				HashSet<TKey> hashSet = set;
				if (hashSet.Add(await func(element, cancellationToken)))
				{
					yield return element;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> Where<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, bool> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				if (func(item))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> Where<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, ValueTask<bool>> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, CancellationToken, ValueTask<bool>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				if (await func(element, cancellationToken))
				{
					yield return element;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> Where<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, bool> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource item in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				if (func(item, num))
				{
					yield return item;
				}
			}
		}
	}

	public static IAsyncEnumerable<TSource> Where<TSource>(this IAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, ValueTask<bool>> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		if (!source.IsKnownEmpty())
		{
			return Impl(source, predicate, default(CancellationToken));
		}
		return Empty<TSource>();
		static async IAsyncEnumerable<TSource> Impl(IAsyncEnumerable<TSource> source2, Func<TSource, int, CancellationToken, ValueTask<bool>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			int index = -1;
			await foreach (TSource element in source2.WithCancellation(cancellationToken))
			{
				int num = checked(index + 1);
				index = num;
				if (await func(element, num, cancellationToken))
				{
					yield return element;
				}
			}
		}
	}

	public static IAsyncEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IAsyncEnumerable<TFirst> first, IAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!first.IsKnownEmpty() && !second.IsKnownEmpty())
		{
			return Impl(first, second, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TFirst> asyncEnumerable, IAsyncEnumerable<TSecond> asyncEnumerable2, Func<TFirst, TSecond, TResult> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TFirst> e1 = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			await using IAsyncEnumerator<TSecond> e2 = asyncEnumerable2.GetAsyncEnumerator(cancellationToken);
			while (true)
			{
				bool flag = await e1.MoveNextAsync();
				if (flag)
				{
					flag = await e2.MoveNextAsync();
				}
				if (!flag)
				{
					break;
				}
				yield return func(e1.Current, e2.Current);
			}
		}
	}

	public static IAsyncEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IAsyncEnumerable<TFirst> first, IAsyncEnumerable<TSecond> second, Func<TFirst, TSecond, CancellationToken, ValueTask<TResult>> resultSelector)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(resultSelector, "resultSelector");
		if (!first.IsKnownEmpty() && !second.IsKnownEmpty())
		{
			return Impl(first, second, resultSelector, default(CancellationToken));
		}
		return Empty<TResult>();
		static async IAsyncEnumerable<TResult> Impl(IAsyncEnumerable<TFirst> asyncEnumerable, IAsyncEnumerable<TSecond> asyncEnumerable2, Func<TFirst, TSecond, CancellationToken, ValueTask<TResult>> func, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TFirst> e1 = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			await using IAsyncEnumerator<TSecond> e2 = asyncEnumerable2.GetAsyncEnumerator(cancellationToken);
			while (true)
			{
				bool flag = await e1.MoveNextAsync();
				if (flag)
				{
					flag = await e2.MoveNextAsync();
				}
				if (!flag)
				{
					break;
				}
				yield return await func(e1.Current, e2.Current, cancellationToken);
			}
		}
	}

	public static IAsyncEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IAsyncEnumerable<TFirst> first, IAsyncEnumerable<TSecond> second)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		if (!first.IsKnownEmpty() && !second.IsKnownEmpty())
		{
			return Impl(first, second, default(CancellationToken));
		}
		return Empty<(TFirst, TSecond)>();
		static async IAsyncEnumerable<(TFirst First, TSecond Second)> Impl(IAsyncEnumerable<TFirst> asyncEnumerable, IAsyncEnumerable<TSecond> asyncEnumerable2, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TFirst> e1 = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			await using IAsyncEnumerator<TSecond> e2 = asyncEnumerable2.GetAsyncEnumerator(cancellationToken);
			while (true)
			{
				bool flag = await e1.MoveNextAsync();
				if (flag)
				{
					flag = await e2.MoveNextAsync();
				}
				if (!flag)
				{
					break;
				}
				yield return (First: e1.Current, Second: e2.Current);
			}
		}
	}

	public static IAsyncEnumerable<(TFirst First, TSecond Second, TThird Third)> Zip<TFirst, TSecond, TThird>(this IAsyncEnumerable<TFirst> first, IAsyncEnumerable<TSecond> second, IAsyncEnumerable<TThird> third)
	{
		ArgumentNullException.ThrowIfNull(first, "first");
		ArgumentNullException.ThrowIfNull(second, "second");
		ArgumentNullException.ThrowIfNull(third, "third");
		if (!first.IsKnownEmpty() && !second.IsKnownEmpty() && !third.IsKnownEmpty())
		{
			return Impl(first, second, third, default(CancellationToken));
		}
		return Empty<(TFirst, TSecond, TThird)>();
		static async IAsyncEnumerable<(TFirst First, TSecond Second, TThird)> Impl(IAsyncEnumerable<TFirst> asyncEnumerable, IAsyncEnumerable<TSecond> asyncEnumerable2, IAsyncEnumerable<TThird> asyncEnumerable3, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<TFirst> e1 = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
			await using IAsyncEnumerator<TSecond> e2 = asyncEnumerable2.GetAsyncEnumerator(cancellationToken);
			await using IAsyncEnumerator<TThird> e3 = asyncEnumerable3.GetAsyncEnumerator(cancellationToken);
			while (true)
			{
				bool flag = await e1.MoveNextAsync();
				if (flag)
				{
					flag = await e2.MoveNextAsync();
				}
				bool flag2 = flag;
				if (flag2)
				{
					flag2 = await e3.MoveNextAsync();
				}
				if (!flag2)
				{
					break;
				}
				yield return (First: e1.Current, Second: e2.Current, e3.Current);
			}
		}
	}

	public static IAsyncEnumerable<T> InfiniteSequence<T>(T start, T step) where T : IAdditionOperators<T, T, T>
	{
		if (start == null)
		{
			System.Linq.ThrowHelper.ThrowArgumentNullException("start");
		}
		if (step == null)
		{
			System.Linq.ThrowHelper.ThrowArgumentNullException("step");
		}
		return Iterator(start, step);
		static async IAsyncEnumerable<T> Iterator(T val, T val2)
		{
			while (true)
			{
				yield return val;
				val += val2;
			}
		}
	}

	public static IAsyncEnumerable<T> Sequence<T>(T start, T endInclusive, T step) where T : INumber<T>
	{
		if (start == null)
		{
			System.Linq.ThrowHelper.ThrowArgumentNullException("start");
		}
		if (T.IsNaN(start))
		{
			System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("start");
		}
		if (endInclusive == null)
		{
			System.Linq.ThrowHelper.ThrowArgumentNullException("endInclusive");
		}
		if (T.IsNaN(endInclusive))
		{
			System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("endInclusive");
		}
		if (step == null)
		{
			System.Linq.ThrowHelper.ThrowArgumentNullException("step");
		}
		if (T.IsNaN(step))
		{
			System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("step");
		}
		if (T.IsZero(step))
		{
			if (start != endInclusive)
			{
				System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("step");
			}
			return Repeat(start, 1);
		}
		if (T.IsPositive(step))
		{
			if (endInclusive < start)
			{
				System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("endInclusive");
			}
			return IncrementingIterator(start, endInclusive, step);
		}
		if (endInclusive > start)
		{
			System.Linq.ThrowHelper.ThrowArgumentOutOfRangeException("endInclusive");
		}
		return DecrementingIterator(start, endInclusive, step);
		static async IAsyncEnumerable<T> DecrementingIterator(T current, T val2, T val)
		{
			yield return current;
			T next;
			while (true)
			{
				next = current + val;
				if (next <= val2 || next >= current)
				{
					break;
				}
				yield return next;
				current = next;
			}
			if (next == val2 && current != next)
			{
				yield return next;
			}
		}
		static async IAsyncEnumerable<T> IncrementingIterator(T current, T val2, T val)
		{
			yield return current;
			T next;
			while (true)
			{
				next = current + val;
				if (next >= val2 || next <= current)
				{
					break;
				}
				yield return next;
				current = next;
			}
			if (next == val2 && current != next)
			{
				yield return next;
			}
		}
	}
}
