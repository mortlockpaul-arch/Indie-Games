using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace System.Linq;

/// <summary>Provides a set of static (Shared in Visual Basic) methods for querying objects that implement <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
public static class Enumerable
{
	private abstract class AppendPrependIterator<TSource> : Iterator<TSource>
	{
		protected readonly IEnumerable<TSource> _source;

		protected IEnumerator<TSource> _enumerator;

		protected AppendPrependIterator(IEnumerable<TSource> source)
		{
			_source = source;
		}

		protected void GetSourceEnumerator()
		{
			_enumerator = _source.GetEnumerator();
		}

		public abstract AppendPrependIterator<TSource> Append(TSource item);

		public abstract AppendPrependIterator<TSource> Prepend(TSource item);

		protected bool LoadFromEnumerator()
		{
			if (_enumerator.MoveNext())
			{
				_current = _enumerator.Current;
				return true;
			}
			Dispose();
			return false;
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}
	}

	private sealed class AppendPrepend1Iterator<TSource> : AppendPrependIterator<TSource>
	{
		private readonly TSource _item;

		private readonly bool _appending;

		public AppendPrepend1Iterator(IEnumerable<TSource> source, TSource item, bool appending)
			: base(source)
		{
			_item = item;
			_appending = appending;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new AppendPrepend1Iterator<TSource>(_source, _item, _appending);
		}

		public override bool MoveNext()
		{
			switch (_state)
			{
			case 1:
				_state = 2;
				if (!_appending)
				{
					_current = _item;
					return true;
				}
				goto case 2;
			case 2:
				GetSourceEnumerator();
				_state = 3;
				goto case 3;
			case 3:
				if (LoadFromEnumerator())
				{
					return true;
				}
				if (_appending)
				{
					_current = _item;
					return true;
				}
				break;
			}
			Dispose();
			return false;
		}

		public override AppendPrependIterator<TSource> Append(TSource item)
		{
			if (_appending)
			{
				return new AppendPrependN<TSource>(_source, null, new SingleLinkedNode<TSource>(_item).Add(item), 0, 2);
			}
			return new AppendPrependN<TSource>(_source, new SingleLinkedNode<TSource>(_item), new SingleLinkedNode<TSource>(item), 1, 1);
		}

		public override AppendPrependIterator<TSource> Prepend(TSource item)
		{
			if (_appending)
			{
				return new AppendPrependN<TSource>(_source, new SingleLinkedNode<TSource>(item), new SingleLinkedNode<TSource>(_item), 1, 1);
			}
			return new AppendPrependN<TSource>(_source, new SingleLinkedNode<TSource>(_item).Add(item), null, 2, 0);
		}

		private TSource[] LazyToArray()
		{
			TSource[] array;
			if (_source is ICollection<TSource> collection)
			{
				array = new TSource[collection.Count + 1];
				if (_appending)
				{
					collection.CopyTo(array, 0);
					array[^1] = _item;
				}
				else
				{
					collection.CopyTo(array, 1);
					array[0] = _item;
				}
			}
			else
			{
				SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
				SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
				if (_appending)
				{
					segmentedArrayBuilder.AddNonICollectionRange(_source);
					segmentedArrayBuilder.Add(_item);
				}
				else
				{
					segmentedArrayBuilder.Add(_item);
					segmentedArrayBuilder.AddNonICollectionRange(_source);
				}
				array = segmentedArrayBuilder.ToArray();
				segmentedArrayBuilder.Dispose();
			}
			return array;
		}

		public override TSource[] ToArray()
		{
			int count = GetCount(onlyIfCheap: true);
			if (count == -1)
			{
				return LazyToArray();
			}
			TSource[] array = new TSource[count];
			int arrayIndex;
			if (_appending)
			{
				arrayIndex = 0;
			}
			else
			{
				array[0] = _item;
				arrayIndex = 1;
			}
			if (_source is ICollection<TSource> collection)
			{
				collection.CopyTo(array, arrayIndex);
			}
			else
			{
				foreach (TSource item in _source)
				{
					array[arrayIndex++] = item;
				}
			}
			if (_appending)
			{
				array[^1] = _item;
			}
			return array;
		}

		public override List<TSource> ToList()
		{
			int count = GetCount(onlyIfCheap: true);
			List<TSource> list;
			switch (count)
			{
			case 1:
				return new List<TSource>(1) { _item };
			default:
				list = new List<TSource>(count);
				break;
			case -1:
				list = new List<TSource>();
				break;
			}
			List<TSource> list2 = list;
			if (!_appending)
			{
				list2.Add(_item);
			}
			list2.AddRange(_source);
			if (_appending)
			{
				list2.Add(_item);
			}
			return list2;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (_source is Iterator<TSource> iterator)
			{
				int count = iterator.GetCount(onlyIfCheap);
				if (count != -1)
				{
					return count + 1;
				}
				return -1;
			}
			if (onlyIfCheap && !(_source is ICollection<TSource>))
			{
				return -1;
			}
			return _source.Count() + 1;
		}

		public override TSource TryGetFirst(out bool found)
		{
			if (_appending)
			{
				TSource result = _source.TryGetFirst(out found);
				if (found)
				{
					return result;
				}
			}
			found = true;
			return _item;
		}

		public override TSource TryGetLast(out bool found)
		{
			if (!_appending)
			{
				TSource result = _source.TryGetLast(out found);
				if (found)
				{
					return result;
				}
			}
			found = true;
			return _item;
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (!_appending)
			{
				if (index == 0)
				{
					found = true;
					return _item;
				}
				index--;
				return _source.TryGetElementAt(index, out found);
			}
			return base.TryGetElementAt(index, out found);
		}

		public override bool Contains(TSource value)
		{
			if (!EqualityComparer<TSource>.Default.Equals(_item, value))
			{
				return _source.Contains(value);
			}
			return true;
		}
	}

	private sealed class AppendPrependN<TSource> : AppendPrependIterator<TSource>
	{
		private readonly SingleLinkedNode<TSource> _prepended;

		private readonly SingleLinkedNode<TSource> _appended;

		private readonly int _prependCount;

		private readonly int _appendCount;

		private SingleLinkedNode<TSource> _node;

		public AppendPrependN(IEnumerable<TSource> source, SingleLinkedNode<TSource> prepended, SingleLinkedNode<TSource> appended, int prependCount, int appendCount)
			: base(source)
		{
			_prepended = prepended;
			_appended = appended;
			_prependCount = prependCount;
			_appendCount = appendCount;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new AppendPrependN<TSource>(_source, _prepended, _appended, _prependCount, _appendCount);
		}

		public override bool MoveNext()
		{
			switch (_state)
			{
			case 1:
				_node = _prepended;
				_state = 2;
				goto case 2;
			case 2:
				if (_node != null)
				{
					_current = _node.Item;
					_node = _node.Linked;
					return true;
				}
				GetSourceEnumerator();
				_state = 3;
				goto case 3;
			case 3:
				if (LoadFromEnumerator())
				{
					return true;
				}
				if (_appended == null)
				{
					return false;
				}
				_enumerator = ((IEnumerable<TSource>)_appended.ToArray(_appendCount)).GetEnumerator();
				_state = 4;
				goto case 4;
			case 4:
				return LoadFromEnumerator();
			default:
				Dispose();
				return false;
			}
		}

		public override AppendPrependIterator<TSource> Append(TSource item)
		{
			SingleLinkedNode<TSource> appended = ((_appended != null) ? _appended.Add(item) : new SingleLinkedNode<TSource>(item));
			return new AppendPrependN<TSource>(_source, _prepended, appended, _prependCount, _appendCount + 1);
		}

		public override AppendPrependIterator<TSource> Prepend(TSource item)
		{
			SingleLinkedNode<TSource> prepended = ((_prepended != null) ? _prepended.Add(item) : new SingleLinkedNode<TSource>(item));
			return new AppendPrependN<TSource>(_source, prepended, _appended, _prependCount + 1, _appendCount);
		}

		private TSource[] LazyToArray()
		{
			if (_source is ICollection<TSource> collection)
			{
				TSource[] array = new TSource[checked(_prependCount + collection.Count + _appendCount)];
				_prepended?.Fill(array);
				collection.CopyTo(array, _prependCount);
				_appended?.FillReversed(array);
				return array;
			}
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			for (SingleLinkedNode<TSource> singleLinkedNode = _prepended; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
			{
				segmentedArrayBuilder.Add(singleLinkedNode.Item);
			}
			segmentedArrayBuilder.AddNonICollectionRange(_source);
			TSource[] array2 = segmentedArrayBuilder.ToArray(_appendCount);
			segmentedArrayBuilder.Dispose();
			_appended?.FillReversed(array2);
			return array2;
		}

		public override TSource[] ToArray()
		{
			int count = GetCount(onlyIfCheap: true);
			if (count == -1)
			{
				return LazyToArray();
			}
			TSource[] array = new TSource[count];
			int num = 0;
			for (SingleLinkedNode<TSource> singleLinkedNode = _prepended; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
			{
				array[num] = singleLinkedNode.Item;
				num++;
			}
			if (_source is ICollection<TSource> collection)
			{
				collection.CopyTo(array, num);
			}
			else
			{
				foreach (TSource item in _source)
				{
					array[num] = item;
					num++;
				}
			}
			num = array.Length;
			for (SingleLinkedNode<TSource> singleLinkedNode2 = _appended; singleLinkedNode2 != null; singleLinkedNode2 = singleLinkedNode2.Linked)
			{
				num--;
				array[num] = singleLinkedNode2.Item;
			}
			return array;
		}

		public override List<TSource> ToList()
		{
			int count = GetCount(onlyIfCheap: true);
			List<TSource> list = ((count == -1) ? new List<TSource>() : new List<TSource>(count));
			_prepended?.Fill(SetCountAndGetSpan(list, _prependCount));
			list.AddRange(_source);
			_appended?.FillReversed(SetCountAndGetSpan(list, list.Count + _appendCount));
			return list;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (_source is Iterator<TSource> iterator)
			{
				int count = iterator.GetCount(onlyIfCheap);
				if (count != -1)
				{
					return count + _appendCount + _prependCount;
				}
				return -1;
			}
			if (onlyIfCheap && !(_source is ICollection<TSource>))
			{
				return -1;
			}
			return _source.Count() + _appendCount + _prependCount;
		}

		public override bool Contains(TSource value)
		{
			global::_003C_003Ey__InlineArray2<SingleLinkedNode<TSource>> buffer = default(global::_003C_003Ey__InlineArray2<SingleLinkedNode<TSource>>);
			buffer[0] = _appended;
			buffer[1] = _prepended;
			ReadOnlySpan<SingleLinkedNode<TSource>> readOnlySpan = buffer;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				for (SingleLinkedNode<TSource> singleLinkedNode = readOnlySpan[i]; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
				{
					if (EqualityComparer<TSource>.Default.Equals(singleLinkedNode.Item, value))
					{
						return true;
					}
				}
			}
			return _source.Contains(value);
		}
	}

	[DebuggerDisplay("Count = {Count}")]
	private sealed class CastICollectionIterator<TResult>(ICollection source) : Iterator<TResult>
	{
		private readonly ICollection _source = source;

		private IEnumerator _enumerator;

		private protected override Iterator<TResult> Clone()
		{
			return new CastICollectionIterator<TResult>(_source);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_0054;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			if (_enumerator.MoveNext())
			{
				_current = (TResult)_enumerator.Current;
				return true;
			}
			Dispose();
			goto IL_0054;
			IL_0054:
			return false;
		}

		public override void Dispose()
		{
			(_enumerator as IDisposable)?.Dispose();
			_enumerator = null;
			base.Dispose();
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return _source.Count;
		}

		public override TResult[] ToArray()
		{
			TResult[] array = new TResult[_source.Count];
			int num = 0;
			foreach (TResult item in _source)
			{
				array[num++] = item;
			}
			return array;
		}

		public override List<TResult> ToList()
		{
			List<TResult> list = new List<TResult>(_source.Count);
			foreach (TResult item in _source)
			{
				list.Add(item);
			}
			return list;
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				{
					IEnumerator enumerator = _source.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							if (index == 0)
							{
								found = true;
								return (TResult)enumerator.Current;
							}
							index--;
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			IEnumerator enumerator = _source.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					found = true;
					return (TResult)enumerator.Current;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			IEnumerator enumerator = _source.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					TResult result = (TResult)enumerator.Current;
					while (enumerator.MoveNext())
					{
						result = (TResult)enumerator.Current;
					}
					found = true;
					return result;
				}
				found = false;
				return default(TResult);
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public override bool Contains(TResult value)
		{
			foreach (TResult item in _source)
			{
				if (EqualityComparer<TResult>.Default.Equals(item, value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class Concat2Iterator<TSource> : ConcatIterator<TSource>
	{
		internal readonly IEnumerable<TSource> _first;

		internal readonly IEnumerable<TSource> _second;

		internal Concat2Iterator(IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			_first = first;
			_second = second;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new Concat2Iterator<TSource>(_first, _second);
		}

		internal override ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
		{
			bool hasOnlyCollections = next is ICollection<TSource> && _first is ICollection<TSource> && _second is ICollection<TSource>;
			return new ConcatNIterator<TSource>(this, next, 2, hasOnlyCollections);
		}

		internal override IEnumerable<TSource> GetEnumerable(int index)
		{
			return index switch
			{
				0 => _first, 
				1 => _second, 
				_ => null, 
			};
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!_first.TryGetNonEnumeratedCount(out var count))
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				count = _first.Count();
			}
			if (!_second.TryGetNonEnumeratedCount(out var count2))
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				count2 = _second.Count();
			}
			return checked(count + count2);
		}

		public override TSource[] ToArray()
		{
			ICollection<TSource> collection = _first as ICollection<TSource>;
			ICollection<TSource> collection2 = _second as ICollection<TSource>;
			checked
			{
				if (collection != null && collection2 != null)
				{
					int count = collection.Count;
					TSource[] array = new TSource[count + collection2.Count];
					collection.CopyTo(array, 0);
					collection2.CopyTo(array, count);
					return array;
				}
				SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
				SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
				TSource[] array2;
				if (collection != null)
				{
					int count2 = collection.Count;
					segmentedArrayBuilder.AddNonICollectionRange(_second);
					array2 = new TSource[count2 + segmentedArrayBuilder.Count];
					collection.CopyTo(array2, 0);
					segmentedArrayBuilder.ToSpan(array2.AsSpan(count2));
				}
				else if (collection2 != null)
				{
					int count3 = collection2.Count;
					segmentedArrayBuilder.AddNonICollectionRange(_first);
					array2 = new TSource[segmentedArrayBuilder.Count + count3];
					segmentedArrayBuilder.ToSpan(array2);
					collection2.CopyTo(array2, unchecked(array2.Length - count3));
				}
				else
				{
					segmentedArrayBuilder.AddNonICollectionRange(_first);
					segmentedArrayBuilder.AddNonICollectionRange(_second);
					array2 = segmentedArrayBuilder.ToArray();
				}
				segmentedArrayBuilder.Dispose();
				return array2;
			}
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				global::_003C_003Ey__InlineArray2<IEnumerable<TSource>> buffer = default(global::_003C_003Ey__InlineArray2<IEnumerable<TSource>>);
				buffer[0] = _first;
				buffer[1] = _second;
				ReadOnlySpan<IEnumerable<TSource>> readOnlySpan = buffer;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					IEnumerable<TSource> enumerable = readOnlySpan[i];
					if (enumerable.TryGetNonEnumeratedCount(out var count))
					{
						if (index < count)
						{
							found = true;
							return enumerable.ElementAt(index);
						}
						index -= count;
						continue;
					}
					using IEnumerator<TSource> enumerator = enumerable.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (index == 0)
						{
							found = true;
							return enumerator.Current;
						}
						index--;
					}
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetFirst(out bool found)
		{
			TSource result = _first.TryGetFirst(out found);
			if (!found)
			{
				result = _second.TryGetFirst(out found);
			}
			return result;
		}

		public override TSource TryGetLast(out bool found)
		{
			TSource result = _second.TryGetLast(out found);
			if (!found)
			{
				result = _first.TryGetLast(out found);
			}
			return result;
		}

		public override bool Contains(TSource value)
		{
			if (!_first.Contains(value))
			{
				return _second.Contains(value);
			}
			return true;
		}
	}

	private sealed class ConcatNIterator<TSource> : ConcatIterator<TSource>
	{
		private readonly ConcatIterator<TSource> _tail;

		private readonly IEnumerable<TSource> _head;

		private readonly int _headIndex;

		private readonly bool _hasOnlyCollections;

		private ConcatNIterator<TSource> PreviousN => _tail as ConcatNIterator<TSource>;

		internal ConcatNIterator(ConcatIterator<TSource> tail, IEnumerable<TSource> head, int headIndex, bool hasOnlyCollections)
		{
			_tail = tail;
			_head = head;
			_headIndex = headIndex;
			_hasOnlyCollections = hasOnlyCollections;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new ConcatNIterator<TSource>(_tail, _head, _headIndex, _hasOnlyCollections);
		}

		internal override ConcatIterator<TSource> Concat(IEnumerable<TSource> next)
		{
			if (_headIndex == 2147483645)
			{
				return new Concat2Iterator<TSource>(this, next);
			}
			bool hasOnlyCollections = _hasOnlyCollections && next is ICollection<TSource>;
			return new ConcatNIterator<TSource>(this, next, _headIndex + 1, hasOnlyCollections);
		}

		internal override IEnumerable<TSource> GetEnumerable(int index)
		{
			if (index > _headIndex)
			{
				return null;
			}
			ConcatNIterator<TSource> concatNIterator = this;
			ConcatNIterator<TSource> concatNIterator2;
			do
			{
				concatNIterator2 = concatNIterator;
				if (index == concatNIterator2._headIndex)
				{
					return concatNIterator2._head;
				}
			}
			while ((concatNIterator = concatNIterator2.PreviousN) != null);
			return concatNIterator2._tail.GetEnumerable(index);
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap && !_hasOnlyCollections)
			{
				return -1;
			}
			int num = 0;
			ConcatNIterator<TSource> concatNIterator = this;
			checked
			{
				ConcatNIterator<TSource> concatNIterator2;
				do
				{
					concatNIterator2 = concatNIterator;
					IEnumerable<TSource> head = concatNIterator2._head;
					int num2 = (head as ICollection<TSource>)?.Count ?? head.Count();
					num += num2;
				}
				while ((concatNIterator = concatNIterator2.PreviousN) != null);
				return num + concatNIterator2._tail.GetCount(onlyIfCheap);
			}
		}

		public override TSource[] ToArray()
		{
			if (!_hasOnlyCollections)
			{
				return LazyToArray();
			}
			return PreallocatingToArray();
		}

		private TSource[] LazyToArray()
		{
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			int num = 0;
			while (true)
			{
				IEnumerable<TSource> enumerable = GetEnumerable(num);
				if (enumerable == null)
				{
					break;
				}
				segmentedArrayBuilder.AddRange(enumerable);
				num++;
			}
			TSource[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		private TSource[] PreallocatingToArray()
		{
			int count = GetCount(onlyIfCheap: true);
			if (count == 0)
			{
				return Array.Empty<TSource>();
			}
			TSource[] array = new TSource[count];
			int num = array.Length;
			ConcatNIterator<TSource> concatNIterator = this;
			checked
			{
				ConcatNIterator<TSource> concatNIterator2;
				do
				{
					concatNIterator2 = concatNIterator;
					ICollection<TSource> collection = (ICollection<TSource>)concatNIterator2._head;
					int count2 = collection.Count;
					if (count2 > 0)
					{
						num -= count2;
						collection.CopyTo(array, num);
					}
				}
				while ((concatNIterator = concatNIterator2.PreviousN) != null);
				Concat2Iterator<TSource> concat2Iterator = (Concat2Iterator<TSource>)concatNIterator2._tail;
				ICollection<TSource> collection2 = (ICollection<TSource>)concat2Iterator._second;
				int count3 = collection2.Count;
				if (count3 > 0)
				{
					collection2.CopyTo(array, num - count3);
				}
				if (num > count3)
				{
					((ICollection<TSource>)concat2Iterator._first).CopyTo(array, 0);
				}
				return array;
			}
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				int num = 0;
				IEnumerable<TSource> enumerable;
				while ((enumerable = GetEnumerable(num)) != null)
				{
					if (enumerable.TryGetNonEnumeratedCount(out var count))
					{
						if (index < count)
						{
							found = true;
							return enumerable.ElementAt(index);
						}
						index -= count;
					}
					else
					{
						using IEnumerator<TSource> enumerator = enumerable.GetEnumerator();
						while (enumerator.MoveNext())
						{
							if (index == 0)
							{
								found = true;
								return enumerator.Current;
							}
							index--;
						}
					}
					num++;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetFirst(out bool found)
		{
			int num = 0;
			IEnumerable<TSource> enumerable;
			while ((enumerable = GetEnumerable(num)) != null)
			{
				TSource result = enumerable.TryGetFirst(out found);
				if (found)
				{
					return result;
				}
				num++;
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetLast(out bool found)
		{
			ConcatNIterator<TSource> concatNIterator = this;
			ConcatNIterator<TSource> concatNIterator2;
			do
			{
				concatNIterator2 = concatNIterator;
				TSource result = concatNIterator2._head.TryGetLast(out found);
				if (found)
				{
					return result;
				}
			}
			while ((concatNIterator = concatNIterator2.PreviousN) != null);
			return concatNIterator2._tail.TryGetLast(out found);
		}

		public override bool Contains(TSource value)
		{
			ConcatNIterator<TSource> concatNIterator = this;
			ConcatNIterator<TSource> concatNIterator2;
			do
			{
				concatNIterator2 = concatNIterator;
				if (concatNIterator2._head.Contains(value))
				{
					return true;
				}
			}
			while ((concatNIterator = concatNIterator2.PreviousN) != null);
			return concatNIterator2._tail.Contains(value);
		}
	}

	private abstract class ConcatIterator<TSource> : Iterator<TSource>
	{
		private IEnumerator<TSource> _enumerator;

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		internal abstract IEnumerable<TSource> GetEnumerable(int index);

		internal abstract ConcatIterator<TSource> Concat(IEnumerable<TSource> next);

		public override bool MoveNext()
		{
			if (_state == 1)
			{
				_enumerator = GetEnumerable(0).GetEnumerator();
				_state = 2;
			}
			if (_state > 1)
			{
				while (true)
				{
					if (_enumerator.MoveNext())
					{
						_current = _enumerator.Current;
						return true;
					}
					IEnumerable<TSource> enumerable = GetEnumerable(_state++ - 1);
					if (enumerable == null)
					{
						break;
					}
					_enumerator.Dispose();
					_enumerator = enumerable.GetEnumerator();
				}
				Dispose();
			}
			return false;
		}

		public override List<TSource> ToList()
		{
			int count = GetCount(onlyIfCheap: true);
			List<TSource> list = ((count != -1) ? new List<TSource>(count) : new List<TSource>());
			int num = 0;
			while (true)
			{
				IEnumerable<TSource> enumerable = GetEnumerable(num);
				if (enumerable == null)
				{
					break;
				}
				list.AddRange(enumerable);
				num++;
			}
			return list;
		}
	}

	private sealed class DefaultIfEmptyIterator<TSource> : Iterator<TSource>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly TSource _default;

		private IEnumerator<TSource> _enumerator;

		public DefaultIfEmptyIterator(IEnumerable<TSource> source, TSource defaultValue)
		{
			_source = source;
			_default = defaultValue;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new DefaultIfEmptyIterator<TSource>(_source, _default);
		}

		public override bool MoveNext()
		{
			switch (_state)
			{
			case 1:
				_enumerator = _source.GetEnumerator();
				if (_enumerator.MoveNext())
				{
					_current = _enumerator.Current;
					_state = 2;
				}
				else
				{
					_current = _default;
					_state = -1;
				}
				return true;
			case 2:
				if (_enumerator.MoveNext())
				{
					_current = _enumerator.Current;
					return true;
				}
				break;
			}
			Dispose();
			return false;
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		public override TSource[] ToArray()
		{
			TSource[] array = _source.ToArray();
			if (array.Length != 0)
			{
				return array;
			}
			return new TSource[1] { _default };
		}

		public override List<TSource> ToList()
		{
			List<TSource> list = _source.ToList();
			if (list.Count == 0)
			{
				list.Add(_default);
			}
			return list;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			int num = ((onlyIfCheap && !(_source is ICollection<TSource>) && !(_source is ICollection)) ? ((_source is Iterator<TSource> iterator) ? iterator.GetCount(onlyIfCheap: true) : (-1)) : _source.Count());
			if (num != 0)
			{
				return num;
			}
			return 1;
		}

		public override TSource TryGetFirst(out bool found)
		{
			TSource result = _source.TryGetFirst(out found);
			if (found)
			{
				return result;
			}
			found = true;
			return _default;
		}

		public override TSource TryGetLast(out bool found)
		{
			TSource result = _source.TryGetLast(out found);
			if (found)
			{
				return result;
			}
			found = true;
			return _default;
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			TSource result = _source.TryGetElementAt(index, out found);
			if (found)
			{
				return result;
			}
			if (index == 0)
			{
				found = true;
				return _default;
			}
			return default(TSource);
		}

		public override bool Contains(TSource value)
		{
			if (_source.TryGetNonEnumeratedCount(out var count))
			{
				if (count <= 0)
				{
					return EqualityComparer<TSource>.Default.Equals(value, _default);
				}
				return _source.Contains(value);
			}
			IEnumerator<TSource> enumerator = _source.GetEnumerator();
			try
			{
				if (!enumerator.MoveNext())
				{
					return EqualityComparer<TSource>.Default.Equals(value, _default);
				}
				do
				{
					if (EqualityComparer<TSource>.Default.Equals(enumerator.Current, value))
					{
						return true;
					}
				}
				while (enumerator.MoveNext());
				return false;
			}
			finally
			{
				enumerator.Dispose();
			}
		}
	}

	private sealed class DistinctIterator<TSource> : Iterator<TSource>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly IEqualityComparer<TSource> _comparer;

		private HashSet<TSource> _set;

		private IEnumerator<TSource> _enumerator;

		public DistinctIterator(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			_source = source;
			_comparer = comparer;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new DistinctIterator<TSource>(_source, _comparer);
		}

		public override bool MoveNext()
		{
			int state = _state;
			TSource current;
			if (state != 1)
			{
				if (state == 2)
				{
					while (_enumerator.MoveNext())
					{
						current = _enumerator.Current;
						if (_set.Add(current))
						{
							_current = current;
							return true;
						}
					}
				}
				Dispose();
				return false;
			}
			_enumerator = _source.GetEnumerator();
			if (!_enumerator.MoveNext())
			{
				Dispose();
				return false;
			}
			current = _enumerator.Current;
			_set = new HashSet<TSource>(7, _comparer);
			_set.Add(current);
			_current = current;
			_state = 2;
			return true;
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
				_set = null;
			}
			base.Dispose();
		}

		public override TSource[] ToArray()
		{
			return ICollectionToArray(new HashSet<TSource>(_source, _comparer));
		}

		public override List<TSource> ToList()
		{
			return new List<TSource>(new HashSet<TSource>(_source, _comparer));
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return new HashSet<TSource>(_source, _comparer).Count;
			}
			return -1;
		}

		public override TSource TryGetFirst(out bool found)
		{
			return _source.TryGetFirst(out found);
		}

		public override bool Contains(TSource value)
		{
			if (_comparer != null)
			{
				return base.Contains(value);
			}
			return _source.Contains(value);
		}
	}

	private sealed class GroupByResultIterator<TSource, TKey, TElement, TResult> : Iterator<TResult>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, TKey> _keySelector;

		private readonly Func<TSource, TElement> _elementSelector;

		private readonly IEqualityComparer<TKey> _comparer;

		private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

		private Lookup<TKey, TElement> _lookup;

		private Grouping<TKey, TElement> _g;

		public GroupByResultIterator(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			_source = source;
			_keySelector = keySelector;
			_elementSelector = elementSelector;
			_comparer = comparer;
			_resultSelector = resultSelector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new GroupByResultIterator<TSource, TKey, TElement, TResult>(_source, _keySelector, _elementSelector, _resultSelector, _comparer);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2 || _g == _lookup._lastGrouping)
				{
					goto IL_0069;
				}
			}
			else
			{
				_lookup = Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer);
				_g = _lookup._lastGrouping;
				if (_g == null)
				{
					goto IL_0069;
				}
				_state = 2;
			}
			_g = _g._next;
			_g.Trim();
			_current = _resultSelector(_g.Key, _g._elements);
			return true;
			IL_0069:
			Dispose();
			return false;
		}

		public override TResult[] ToArray()
		{
			return Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer).ToArray(_resultSelector);
		}

		public override List<TResult> ToList()
		{
			return Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer).ToList(_resultSelector);
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer).Count;
			}
			return -1;
		}
	}

	private sealed class GroupByResultIterator<TSource, TKey, TResult> : Iterator<TResult>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, TKey> _keySelector;

		private readonly IEqualityComparer<TKey> _comparer;

		private readonly Func<TKey, IEnumerable<TSource>, TResult> _resultSelector;

		private Lookup<TKey, TSource> _lookup;

		private Grouping<TKey, TSource> _g;

		public GroupByResultIterator(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			_source = source;
			_keySelector = keySelector;
			_resultSelector = resultSelector;
			_comparer = comparer;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new GroupByResultIterator<TSource, TKey, TResult>(_source, _keySelector, _resultSelector, _comparer);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2 || _g == _lookup._lastGrouping)
				{
					goto IL_0063;
				}
			}
			else
			{
				_lookup = Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer);
				_g = _lookup._lastGrouping;
				if (_g == null)
				{
					goto IL_0063;
				}
				_state = 2;
			}
			_g = _g._next;
			_g.Trim();
			_current = _resultSelector(_g.Key, _g._elements);
			return true;
			IL_0063:
			Dispose();
			return false;
		}

		public override TResult[] ToArray()
		{
			return Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer).ToArray(_resultSelector);
		}

		public override List<TResult> ToList()
		{
			return Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer).ToList(_resultSelector);
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer).Count;
			}
			return -1;
		}
	}

	private sealed class GroupByIterator<TSource, TKey, TElement> : Iterator<IGrouping<TKey, TElement>>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, TKey> _keySelector;

		private readonly Func<TSource, TElement> _elementSelector;

		private readonly IEqualityComparer<TKey> _comparer;

		private Lookup<TKey, TElement> _lookup;

		private Grouping<TKey, TElement> _g;

		public GroupByIterator(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			_source = source;
			_keySelector = keySelector;
			_elementSelector = elementSelector;
			_comparer = comparer;
		}

		private protected override Iterator<IGrouping<TKey, TElement>> Clone()
		{
			return new GroupByIterator<TSource, TKey, TElement>(_source, _keySelector, _elementSelector, _comparer);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2 || _g == _lookup._lastGrouping)
				{
					goto IL_0069;
				}
			}
			else
			{
				_lookup = Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer);
				_g = _lookup._lastGrouping;
				if (_g == null)
				{
					goto IL_0069;
				}
				_state = 2;
			}
			_g = _g._next;
			_current = _g;
			return true;
			IL_0069:
			Dispose();
			return false;
		}

		public override IGrouping<TKey, TElement>[] ToArray()
		{
			return ((IEnumerable<IGrouping<TKey, TElement>>)Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer)).ToArray();
		}

		public override List<IGrouping<TKey, TElement>> ToList()
		{
			return ((IEnumerable<IGrouping<TKey, TElement>>)Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer)).ToList();
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return Lookup<TKey, TElement>.Create(_source, _keySelector, _elementSelector, _comparer).Count;
			}
			return -1;
		}
	}

	private sealed class GroupByIterator<TSource, TKey> : Iterator<IGrouping<TKey, TSource>>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, TKey> _keySelector;

		private readonly IEqualityComparer<TKey> _comparer;

		private Lookup<TKey, TSource> _lookup;

		private Grouping<TKey, TSource> _g;

		public GroupByIterator(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			_source = source;
			_keySelector = keySelector;
			_comparer = comparer;
		}

		private protected override Iterator<IGrouping<TKey, TSource>> Clone()
		{
			return new GroupByIterator<TSource, TKey>(_source, _keySelector, _comparer);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2 || _g == _lookup._lastGrouping)
				{
					goto IL_0063;
				}
			}
			else
			{
				_lookup = Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer);
				_g = _lookup._lastGrouping;
				if (_g == null)
				{
					goto IL_0063;
				}
				_state = 2;
			}
			_g = _g._next;
			_current = _g;
			return true;
			IL_0063:
			Dispose();
			return false;
		}

		public override IGrouping<TKey, TSource>[] ToArray()
		{
			return ((IEnumerable<IGrouping<TKey, TSource>>)Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer)).ToArray();
		}

		public override List<IGrouping<TKey, TSource>> ToList()
		{
			return ((IEnumerable<IGrouping<TKey, TSource>>)Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer)).ToList();
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return Lookup<TKey, TSource>.Create(_source, _keySelector, _comparer).Count;
			}
			return -1;
		}
	}

	private abstract class Iterator<TSource> : IEnumerable<TSource>, IEnumerable, IEnumerator<TSource>, IEnumerator, IDisposable
	{
		private readonly int _threadId = Environment.CurrentManagedThreadId;

		private protected int _state;

		private protected TSource _current;

		public TSource Current => _current;

		object IEnumerator.Current => Current;

		private protected abstract Iterator<TSource> Clone();

		public virtual void Dispose()
		{
			_current = default(TSource);
			_state = -1;
		}

		public Iterator<TSource> GetEnumerator()
		{
			Iterator<TSource> obj = ((_state == 0 && _threadId == Environment.CurrentManagedThreadId) ? this : Clone());
			obj._state = 1;
			return obj;
		}

		public abstract bool MoveNext();

		public virtual IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
		{
			if (IsSizeOptimized)
			{
				return new IEnumerableSelectIterator<TSource, TResult>(this, selector);
			}
			return new IteratorSelectIterator<TSource, TResult>(this, selector);
		}

		public virtual IEnumerable<TSource> Where(Func<TSource, bool> predicate)
		{
			return new IEnumerableWhereIterator<TSource>(this, predicate);
		}

		IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void IEnumerator.Reset()
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		public abstract TSource[] ToArray();

		public abstract List<TSource> ToList();

		public abstract int GetCount(bool onlyIfCheap);

		public virtual Iterator<TSource> Skip(int count)
		{
			return new IEnumerableSkipTakeIterator<TSource>(this, count, -1);
		}

		public virtual Iterator<TSource> Take(int count)
		{
			return new IEnumerableSkipTakeIterator<TSource>(this, 0, count - 1);
		}

		public virtual TSource TryGetElementAt(int index, out bool found)
		{
			if (index != 0)
			{
				return TryGetElementAtNonIterator(this, index, out found);
			}
			return TryGetFirst(out found);
		}

		public virtual TSource TryGetFirst(out bool found)
		{
			return TryGetFirstNonIterator(this, out found);
		}

		public virtual TSource TryGetLast(out bool found)
		{
			return TryGetLastNonIterator(this, out found);
		}

		public virtual bool Contains(TSource value)
		{
			return ContainsIterate(this, value, null);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct MaxCalc<T> : IMinMaxCalc<T> where T : struct, IBinaryInteger<T>
	{
		public static bool Compare(T left, T right)
		{
			return left > right;
		}

		public static Vector128<T> Compare(Vector128<T> left, Vector128<T> right)
		{
			return Vector128.Max(left, right);
		}

		public static Vector256<T> Compare(Vector256<T> left, Vector256<T> right)
		{
			return Vector256.Max(left, right);
		}

		public static Vector512<T> Compare(Vector512<T> left, Vector512<T> right)
		{
			return Vector512.Max(left, right);
		}
	}

	private interface IMinMaxCalc<T> where T : struct, IBinaryInteger<T>
	{
		static abstract bool Compare(T left, T right);

		static abstract Vector128<T> Compare(Vector128<T> left, Vector128<T> right);

		static abstract Vector256<T> Compare(Vector256<T> left, Vector256<T> right);

		static abstract Vector512<T> Compare(Vector512<T> left, Vector512<T> right);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct MinCalc<T> : IMinMaxCalc<T> where T : struct, IBinaryInteger<T>
	{
		public static bool Compare(T left, T right)
		{
			return left < right;
		}

		public static Vector128<T> Compare(Vector128<T> left, Vector128<T> right)
		{
			return Vector128.Min(left, right);
		}

		public static Vector256<T> Compare(Vector256<T> left, Vector256<T> right)
		{
			return Vector256.Min(left, right);
		}

		public static Vector512<T> Compare(Vector512<T> left, Vector512<T> right)
		{
			return Vector512.Min(left, right);
		}
	}

	private sealed class OfTypeIterator<TResult>(IEnumerable source) : Iterator<TResult>
	{
		private readonly IEnumerable _source = source;

		private IEnumerator _enumerator;

		private protected override Iterator<TResult> Clone()
		{
			return new OfTypeIterator<TResult>(_source);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_0062;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			while (_enumerator.MoveNext())
			{
				if (_enumerator.Current is TResult current)
				{
					_current = current;
					return true;
				}
			}
			Dispose();
			goto IL_0062;
			IL_0062:
			return false;
		}

		public override void Dispose()
		{
			(_enumerator as IDisposable)?.Dispose();
			_enumerator = null;
			base.Dispose();
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			foreach (object item in _source)
			{
				if (item is TResult)
				{
					num = checked(num + 1);
				}
			}
			return num;
		}

		public override TResult[] ToArray()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			foreach (object item2 in _source)
			{
				if (item2 is TResult item)
				{
					segmentedArrayBuilder.Add(item);
				}
			}
			TResult[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TResult> ToList()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			foreach (object item2 in _source)
			{
				if (item2 is TResult item)
				{
					segmentedArrayBuilder.Add(item);
				}
			}
			List<TResult> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override TResult TryGetFirst(out bool found)
		{
			foreach (object item in _source)
			{
				if (item is TResult result)
				{
					found = true;
					return result;
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			IEnumerator enumerator = _source.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					do
					{
						if (!(enumerator.Current is TResult result))
						{
							continue;
						}
						found = true;
						while (enumerator.MoveNext())
						{
							if (enumerator.Current is TResult val)
							{
								result = val;
							}
						}
						return result;
					}
					while (enumerator.MoveNext());
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				foreach (object item in _source)
				{
					if (item is TResult result)
					{
						if (index == 0)
						{
							found = true;
							return result;
						}
						index--;
					}
				}
			}
			found = false;
			return default(TResult);
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			if (!typeof(TResult).IsValueType && _source is IEnumerable<object> enumerable)
			{
				Func<object, TResult2> selector2 = Unsafe.As<Func<object, TResult2>>(selector);
				Func<object, bool> predicate = (object o) => o is TResult;
				if (IsSizeOptimized || !(enumerable is object[] source))
				{
					return new IEnumerableWhereSelectIterator<object, TResult2>(enumerable, predicate, selector2);
				}
				return new ArrayWhereSelectIterator<object, TResult2>(source, predicate, selector2);
			}
			return base.Select(selector);
		}

		public override bool Contains(TResult value)
		{
			foreach (object item in _source)
			{
				if (item is TResult x && EqualityComparer<TResult>.Default.Equals(x, value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private abstract class OrderedIterator<TElement> : Iterator<TElement>, IOrderedEnumerable<TElement>, IEnumerable<TElement>, IEnumerable
	{
		internal readonly IEnumerable<TElement> _source;

		protected OrderedIterator(IEnumerable<TElement> source)
		{
			_source = source;
		}

		private protected int[] SortedMap(TElement[] buffer)
		{
			return GetEnumerableSorter().Sort(buffer, buffer.Length);
		}

		internal int[] SortedMap(TElement[] buffer, int minIdx, int maxIdx)
		{
			return GetEnumerableSorter().Sort(buffer, buffer.Length, minIdx, maxIdx);
		}

		internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next = null);

		internal abstract CachingComparer<TElement> GetComparer(CachingComparer<TElement> childComparer = null);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedIterator<TElement, TKey>(_source, keySelector, comparer, descending, this);
		}

		public TElement TryGetLast(Func<TElement, bool> predicate, out bool found)
		{
			CachingComparer<TElement> comparer = GetComparer();
			using IEnumerator<TElement> enumerator = _source.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TElement val = enumerator.Current;
				if (!predicate(val))
				{
					continue;
				}
				comparer.SetElement(val);
				while (enumerator.MoveNext())
				{
					TElement current = enumerator.Current;
					if (predicate(current) && comparer.Compare(current, cacheLower: false) >= 0)
					{
						val = current;
					}
				}
				found = true;
				return val;
			}
			found = false;
			return default(TElement);
		}

		public override TElement[] ToArray()
		{
			TElement[] array = _source.ToArray();
			if (array.Length <= 1)
			{
				return array;
			}
			TElement[] array2 = new TElement[array.Length];
			Fill(array, array2);
			return array2;
		}

		public override List<TElement> ToList()
		{
			TElement[] array = _source.ToArray();
			List<TElement> list = new List<TElement>(array.Length);
			if (array.Length >= 2)
			{
				Fill(array, SetCountAndGetSpan(list, array.Length));
			}
			else if (array.Length == 1)
			{
				list.Add(array[0]);
			}
			return list;
		}

		private void Fill(TElement[] buffer, Span<TElement> destination)
		{
			int[] array = SortedMap(buffer);
			for (int i = 0; i < destination.Length; i++)
			{
				destination[i] = buffer[array[i]];
			}
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (_source is Iterator<TElement> iterator)
			{
				return iterator.GetCount(onlyIfCheap);
			}
			if (onlyIfCheap && !(_source is ICollection<TElement>) && !(_source is ICollection))
			{
				return -1;
			}
			return _source.Count();
		}

		internal TElement[] ToArray(int minIdx, int maxIdx)
		{
			TElement[] array = _source.ToArray();
			if (array.Length <= minIdx)
			{
				return Array.Empty<TElement>();
			}
			if (array.Length <= maxIdx)
			{
				maxIdx = array.Length - 1;
			}
			if (minIdx == maxIdx)
			{
				return new TElement[1] { GetEnumerableSorter().ElementAt(array, array.Length, minIdx) };
			}
			TElement[] array2 = new TElement[maxIdx - minIdx + 1];
			Fill(minIdx, maxIdx, array, array2);
			return array2;
		}

		internal List<TElement> ToList(int minIdx, int maxIdx)
		{
			TElement[] array = _source.ToArray();
			if (array.Length <= minIdx)
			{
				return new List<TElement>();
			}
			if (array.Length <= maxIdx)
			{
				maxIdx = array.Length - 1;
			}
			if (minIdx == maxIdx)
			{
				return new List<TElement>(1) { GetEnumerableSorter().ElementAt(array, array.Length, minIdx) };
			}
			List<TElement> list = new List<TElement>();
			Fill(minIdx, maxIdx, array, SetCountAndGetSpan(list, maxIdx - minIdx + 1));
			return list;
		}

		private void Fill(int minIdx, int maxIdx, TElement[] buffer, Span<TElement> destination)
		{
			int[] array = SortedMap(buffer, minIdx, maxIdx);
			int num = 0;
			while (minIdx <= maxIdx)
			{
				destination[num] = buffer[array[minIdx]];
				num++;
				minIdx++;
			}
		}

		internal int GetCount(int minIdx, int maxIdx, bool onlyIfCheap)
		{
			int count = GetCount(onlyIfCheap);
			if (count <= 0)
			{
				return count;
			}
			if (count <= minIdx)
			{
				return 0;
			}
			return ((count <= maxIdx) ? count : (maxIdx + 1)) - minIdx;
		}

		public override Iterator<TElement> Skip(int count)
		{
			return new SkipTakeOrderedIterator<TElement>(this, count, int.MaxValue);
		}

		public override Iterator<TElement> Take(int count)
		{
			return new SkipTakeOrderedIterator<TElement>(this, 0, count - 1);
		}

		public override TElement TryGetElementAt(int index, out bool found)
		{
			if (index == 0)
			{
				return TryGetFirst(out found);
			}
			if (index > 0)
			{
				TElement[] array = _source.ToArray();
				if (index < array.Length)
				{
					found = true;
					return GetEnumerableSorter().ElementAt(array, array.Length, index);
				}
			}
			found = false;
			return default(TElement);
		}

		public override TElement TryGetFirst(out bool found)
		{
			CachingComparer<TElement> comparer = GetComparer();
			using IEnumerator<TElement> enumerator = _source.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				found = false;
				return default(TElement);
			}
			TElement val = enumerator.Current;
			comparer.SetElement(val);
			while (enumerator.MoveNext())
			{
				TElement current = enumerator.Current;
				if (comparer.Compare(current, cacheLower: true) < 0)
				{
					val = current;
				}
			}
			found = true;
			return val;
		}

		public override TElement TryGetLast(out bool found)
		{
			using IEnumerator<TElement> enumerator = _source.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				found = false;
				return default(TElement);
			}
			CachingComparer<TElement> comparer = GetComparer();
			TElement val = enumerator.Current;
			comparer.SetElement(val);
			while (enumerator.MoveNext())
			{
				TElement current = enumerator.Current;
				if (comparer.Compare(current, cacheLower: false) >= 0)
				{
					val = current;
				}
			}
			found = true;
			return val;
		}

		public TElement TryGetLast(int minIdx, int maxIdx, out bool found)
		{
			TElement[] array = _source.ToArray();
			if (minIdx < array.Length)
			{
				found = true;
				if (maxIdx >= array.Length - 1)
				{
					return Last(array);
				}
				return GetEnumerableSorter().ElementAt(array, array.Length, maxIdx);
			}
			found = false;
			return default(TElement);
		}

		public override bool Contains(TElement value)
		{
			return _source.Contains(value);
		}

		private TElement Last(TElement[] items)
		{
			CachingComparer<TElement> comparer = GetComparer();
			TElement val = items[0];
			comparer.SetElement(val);
			for (int i = 1; i < items.Length; i++)
			{
				TElement val2 = items[i];
				if (comparer.Compare(val2, cacheLower: false) >= 0)
				{
					val = val2;
				}
			}
			return val;
		}
	}

	private sealed class OrderedIterator<TElement, TKey> : OrderedIterator<TElement>
	{
		private readonly OrderedIterator<TElement> _parent;

		private readonly Func<TElement, TKey> _keySelector;

		private readonly IComparer<TKey> _comparer;

		private readonly bool _descending;

		private TElement[] _buffer;

		private int[] _map;

		internal OrderedIterator(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, OrderedIterator<TElement> parent)
			: base(source)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
			}
			if (keySelector == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
			}
			_parent = parent;
			_keySelector = keySelector;
			_comparer = comparer ?? Comparer<TKey>.Default;
			_descending = descending;
		}

		private protected override Iterator<TElement> Clone()
		{
			return new OrderedIterator<TElement, TKey>(_source, _keySelector, _comparer, _descending, _parent);
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

		internal override CachingComparer<TElement> GetComparer(CachingComparer<TElement> childComparer)
		{
			CachingComparer<TElement> cachingComparer = ((childComparer == null) ? new CachingComparer<TElement, TKey>(_keySelector, _comparer, _descending) : new CachingComparerWithChild<TElement, TKey>(_keySelector, _comparer, _descending, childComparer));
			if (_parent == null)
			{
				return cachingComparer;
			}
			return _parent.GetComparer(cachingComparer);
		}

		public override bool MoveNext()
		{
			int num = _state;
			while (true)
			{
				if (num > 1)
				{
					int[] map = _map;
					int num2 = num - 2;
					if ((uint)num2 >= (uint)map.Length)
					{
						break;
					}
					_current = _buffer[map[num2]];
					_state++;
					return true;
				}
				if (num != 1)
				{
					break;
				}
				TElement[] array = _source.ToArray();
				if (array.Length == 0)
				{
					break;
				}
				_map = SortedMap(array);
				_buffer = array;
				num = (_state = 2);
			}
			Dispose();
			return false;
		}

		public override void Dispose()
		{
			_buffer = null;
			_map = null;
			base.Dispose();
		}

		public override TElement TryGetFirst(out bool found)
		{
			if (_parent != null)
			{
				return base.TryGetFirst(out found);
			}
			using IEnumerator<TElement> enumerator = _source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				IComparer<TKey> comparer = _comparer;
				Func<TElement, TKey> keySelector = _keySelector;
				TElement val = enumerator.Current;
				TKey y = keySelector(val);
				if (_descending)
				{
					while (enumerator.MoveNext())
					{
						TElement current = enumerator.Current;
						TKey val2 = keySelector(current);
						if (comparer.Compare(val2, y) > 0)
						{
							y = val2;
							val = current;
						}
					}
				}
				else
				{
					while (enumerator.MoveNext())
					{
						TElement current2 = enumerator.Current;
						TKey val3 = keySelector(current2);
						if (comparer.Compare(val3, y) < 0)
						{
							y = val3;
							val = current2;
						}
					}
				}
				found = true;
				return val;
			}
			found = false;
			return default(TElement);
		}

		public override TElement TryGetLast(out bool found)
		{
			if (_parent != null)
			{
				return base.TryGetLast(out found);
			}
			using IEnumerator<TElement> enumerator = _source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				IComparer<TKey> comparer = _comparer;
				Func<TElement, TKey> keySelector = _keySelector;
				TElement val = enumerator.Current;
				TKey y = keySelector(val);
				if (_descending)
				{
					while (enumerator.MoveNext())
					{
						TElement current = enumerator.Current;
						TKey val2 = keySelector(current);
						if (comparer.Compare(val2, y) <= 0)
						{
							y = val2;
							val = current;
						}
					}
				}
				else
				{
					while (enumerator.MoveNext())
					{
						TElement current2 = enumerator.Current;
						TKey val3 = keySelector(current2);
						if (comparer.Compare(val3, y) >= 0)
						{
							y = val3;
							val = current2;
						}
					}
				}
				found = true;
				return val;
			}
			found = false;
			return default(TElement);
		}
	}

	private sealed class ImplicitlyStableOrderedIterator<TElement> : OrderedIterator<TElement>
	{
		private readonly bool _descending;

		private TElement[] _buffer;

		public ImplicitlyStableOrderedIterator(IEnumerable<TElement> source, bool descending)
			: base(source)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
			}
			_descending = descending;
		}

		private protected override Iterator<TElement> Clone()
		{
			return new ImplicitlyStableOrderedIterator<TElement>(_source, _descending);
		}

		internal override CachingComparer<TElement> GetComparer(CachingComparer<TElement> childComparer)
		{
			if (childComparer != null)
			{
				return new CachingComparerWithChild<TElement, TElement>(EnumerableSorter<TElement>.IdentityFunc, Comparer<TElement>.Default, _descending, childComparer);
			}
			return new CachingComparer<TElement, TElement>(EnumerableSorter<TElement>.IdentityFunc, Comparer<TElement>.Default, _descending);
		}

		internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next)
		{
			return new EnumerableSorter<TElement, TElement>(EnumerableSorter<TElement>.IdentityFunc, Comparer<TElement>.Default, _descending, next);
		}

		public override bool MoveNext()
		{
			int num = _state;
			while (true)
			{
				TElement[] buffer;
				if (num > 1)
				{
					buffer = _buffer;
					int num2 = num - 2;
					if ((uint)num2 >= (uint)buffer.Length)
					{
						break;
					}
					_current = buffer[num2];
					_state++;
					return true;
				}
				if (num != 1)
				{
					break;
				}
				buffer = _source.ToArray();
				if (buffer.Length == 0)
				{
					break;
				}
				Sort(buffer, _descending);
				_buffer = buffer;
				num = (_state = 2);
			}
			Dispose();
			return false;
		}

		public override void Dispose()
		{
			_buffer = null;
			base.Dispose();
		}

		private static void Sort(Span<TElement> span, bool descending)
		{
			if (descending)
			{
				span.Sort((TElement a, TElement b) => Comparer<TElement>.Default.Compare(b, a));
			}
			else
			{
				span.Sort();
			}
		}

		public override TElement[] ToArray()
		{
			TElement[] array = _source.ToArray();
			Sort(array, _descending);
			return array;
		}

		public override List<TElement> ToList()
		{
			List<TElement> list = _source.ToList();
			Sort(CollectionsMarshal.AsSpan(list), _descending);
			return list;
		}

		public override TElement TryGetFirst(out bool found)
		{
			return TryGetFirstOrLast(out found, !_descending);
		}

		public override TElement TryGetLast(out bool found)
		{
			return TryGetFirstOrLast(out found, _descending);
		}

		private TElement TryGetFirstOrLast(out bool found, bool first)
		{
			if (_source.TryGetSpan(out var span))
			{
				if (span.Length != 0)
				{
					found = true;
					if (!first)
					{
						return _source.Max();
					}
					return _source.Min();
				}
			}
			else
			{
				using IEnumerator<TElement> enumerator = _source.GetEnumerator();
				if (enumerator.MoveNext())
				{
					TElement val = enumerator.Current;
					if (first)
					{
						while (enumerator.MoveNext())
						{
							TElement current = enumerator.Current;
							if (Comparer<TElement>.Default.Compare(current, val) < 0)
							{
								val = current;
							}
						}
					}
					else
					{
						while (enumerator.MoveNext())
						{
							TElement current2 = enumerator.Current;
							if (Comparer<TElement>.Default.Compare(current2, val) >= 0)
							{
								val = current2;
							}
						}
					}
					found = true;
					return val;
				}
			}
			found = false;
			return default(TElement);
		}
	}

	private abstract class CachingComparer<TElement>
	{
		internal abstract int Compare(TElement element, bool cacheLower);

		internal abstract void SetElement(TElement element);
	}

	private class CachingComparer<TElement, TKey> : CachingComparer<TElement>
	{
		protected readonly Func<TElement, TKey> _keySelector;

		protected readonly IComparer<TKey> _comparer;

		protected readonly bool _descending;

		protected TKey _lastKey;

		public CachingComparer(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			_keySelector = keySelector;
			_comparer = comparer;
			_descending = descending;
		}

		internal override int Compare(TElement element, bool cacheLower)
		{
			TKey val = _keySelector(element);
			int num = (_descending ? _comparer.Compare(_lastKey, val) : _comparer.Compare(val, _lastKey));
			if (cacheLower == num < 0)
			{
				_lastKey = val;
			}
			return num;
		}

		internal override void SetElement(TElement element)
		{
			_lastKey = _keySelector(element);
		}
	}

	private sealed class CachingComparerWithChild<TElement, TKey> : CachingComparer<TElement, TKey>
	{
		private readonly CachingComparer<TElement> _child;

		public CachingComparerWithChild(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, CachingComparer<TElement> child)
			: base(keySelector, comparer, descending)
		{
			_child = child;
		}

		internal override int Compare(TElement element, bool cacheLower)
		{
			TKey val = _keySelector(element);
			int num = (_descending ? _comparer.Compare(_lastKey, val) : _comparer.Compare(val, _lastKey));
			if (num == 0)
			{
				return _child.Compare(element, cacheLower);
			}
			if (cacheLower == num < 0)
			{
				_lastKey = val;
				_child.SetElement(element);
			}
			return num;
		}

		internal override void SetElement(TElement element)
		{
			base.SetElement(element);
			_child.SetElement(element);
		}
	}

	private abstract class EnumerableSorter<TElement>
	{
		internal static readonly Func<TElement, TElement> IdentityFunc = (TElement e) => e;

		internal abstract void ComputeKeys(TElement[] elements, int count);

		internal abstract int CompareAnyKeys(int index1, int index2);

		private int[] ComputeMap(TElement[] elements, int count)
		{
			ComputeKeys(elements, count);
			int[] array = new int[count];
			FillIncrementing(array, 0);
			return array;
		}

		internal int[] Sort(TElement[] elements, int count)
		{
			int[] array = ComputeMap(elements, count);
			QuickSort(array, 0, count - 1);
			return array;
		}

		internal int[] Sort(TElement[] elements, int count, int minIdx, int maxIdx)
		{
			int[] array = ComputeMap(elements, count);
			PartialQuickSort(array, 0, count - 1, minIdx, maxIdx);
			return array;
		}

		internal TElement ElementAt(TElement[] elements, int count, int idx)
		{
			int[] map = ComputeMap(elements, count);
			if (idx != 0)
			{
				return elements[QuickSelect(map, count - 1, idx)];
			}
			return elements[Min(map, count)];
		}

		protected abstract void QuickSort(int[] map, int left, int right);

		protected abstract void PartialQuickSort(int[] map, int left, int right, int minIdx, int maxIdx);

		protected abstract int QuickSelect(int[] map, int right, int idx);

		protected abstract int Min(int[] map, int count);
	}

	private sealed class EnumerableSorter<TElement, TKey> : EnumerableSorter<TElement>
	{
		private readonly Func<TElement, TKey> _keySelector;

		private readonly IComparer<TKey> _comparer;

		private readonly bool _descending;

		private readonly EnumerableSorter<TElement> _next;

		private TKey[] _keys;

		internal EnumerableSorter(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, EnumerableSorter<TElement> next)
		{
			_keySelector = keySelector;
			_comparer = comparer;
			_descending = descending;
			_next = next;
		}

		internal override void ComputeKeys(TElement[] elements, int count)
		{
			Func<TElement, TKey> keySelector = _keySelector;
			if ((object)keySelector != EnumerableSorter<TElement>.IdentityFunc)
			{
				TKey[] array = new TKey[count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = keySelector(elements[i]);
				}
				_keys = array;
			}
			else
			{
				_keys = (TKey[])(object)elements;
			}
			_next?.ComputeKeys(elements, count);
		}

		internal override int CompareAnyKeys(int index1, int index2)
		{
			TKey[] keys = _keys;
			int num = _comparer.Compare(keys[index1], keys[index2]);
			if (num == 0)
			{
				if (_next == null)
				{
					return index1 - index2;
				}
				return _next.CompareAnyKeys(index1, index2);
			}
			if (_descending == num > 0)
			{
				return -1;
			}
			return 1;
		}

		private int CompareAnyKeys_DefaultComparer_NoNext_Ascending(int index1, int index2)
		{
			TKey[] keys = _keys;
			int num = Comparer<TKey>.Default.Compare(keys[index1], keys[index2]);
			if (num != 0)
			{
				return num;
			}
			return index1 - index2;
		}

		private int CompareAnyKeys_DefaultComparer_NoNext_Descending(int index1, int index2)
		{
			TKey[] keys = _keys;
			int num = Comparer<TKey>.Default.Compare(keys[index2], keys[index1]);
			if (num != 0)
			{
				return num;
			}
			return index1 - index2;
		}

		private int CompareKeys(int index1, int index2)
		{
			if (index1 != index2)
			{
				return CompareAnyKeys(index1, index2);
			}
			return 0;
		}

		protected override void QuickSort(int[] keys, int lo, int hi)
		{
			MemoryExtensions.Sort(comparison: (!typeof(TKey).IsValueType || _next != null || _comparer != Comparer<TKey>.Default) ? new Comparison<int>(CompareAnyKeys) : (_descending ? new Comparison<int>(CompareAnyKeys_DefaultComparer_NoNext_Descending) : new Comparison<int>(CompareAnyKeys_DefaultComparer_NoNext_Ascending)), span: new Span<int>(keys, lo, hi - lo + 1));
		}

		protected override void PartialQuickSort(int[] map, int left, int right, int minIdx, int maxIdx)
		{
			do
			{
				int num = left;
				int num2 = right;
				int index = map[num + (num2 - num >> 1)];
				while (true)
				{
					if (num < map.Length && CompareKeys(index, map[num]) > 0)
					{
						num++;
						continue;
					}
					while (num2 >= 0 && CompareKeys(index, map[num2]) < 0)
					{
						num2--;
					}
					if (num > num2)
					{
						break;
					}
					if (num < num2)
					{
						int num3 = map[num];
						map[num] = map[num2];
						map[num2] = num3;
					}
					num++;
					num2--;
					if (num > num2)
					{
						break;
					}
				}
				if (minIdx >= num)
				{
					left = num + 1;
				}
				else if (maxIdx <= num2)
				{
					right = num2 - 1;
				}
				if (num2 - left <= right - num)
				{
					if (left < num2)
					{
						PartialQuickSort(map, left, num2, minIdx, maxIdx);
					}
					left = num;
				}
				else
				{
					if (num < right)
					{
						PartialQuickSort(map, num, right, minIdx, maxIdx);
					}
					right = num2;
				}
			}
			while (left < right);
		}

		protected override int QuickSelect(int[] map, int right, int idx)
		{
			int num = 0;
			do
			{
				int num2 = num;
				int num3 = right;
				int index = map[num2 + (num3 - num2 >> 1)];
				while (true)
				{
					if (num2 < map.Length && CompareKeys(index, map[num2]) > 0)
					{
						num2++;
						continue;
					}
					while (num3 >= 0 && CompareKeys(index, map[num3]) < 0)
					{
						num3--;
					}
					if (num2 > num3)
					{
						break;
					}
					if (num2 < num3)
					{
						int num4 = map[num2];
						map[num2] = map[num3];
						map[num3] = num4;
					}
					num2++;
					num3--;
					if (num2 > num3)
					{
						break;
					}
				}
				if (num2 <= idx)
				{
					num = num2 + 1;
				}
				else
				{
					right = num3 - 1;
				}
				if (num3 - num <= right - num2)
				{
					if (num < num3)
					{
						right = num3;
					}
					num = num2;
				}
				else
				{
					if (num2 < right)
					{
						num = num2;
					}
					right = num3;
				}
			}
			while (num < right);
			return map[idx];
		}

		protected override int Min(int[] map, int count)
		{
			int num = 0;
			for (int i = 1; i < count; i++)
			{
				if (CompareKeys(map[i], map[num]) < 0)
				{
					num = i;
				}
			}
			return map[num];
		}
	}

	private sealed class SkipTakeOrderedIterator<TElement> : Iterator<TElement>
	{
		private readonly OrderedIterator<TElement> _source;

		private readonly int _minIndexInclusive;

		private readonly int _maxIndexInclusive;

		private TElement[] _buffer;

		private int[] _map;

		private int _maxIdx;

		public SkipTakeOrderedIterator(OrderedIterator<TElement> source, int minIdxInclusive, int maxIdxInclusive)
		{
			_source = source;
			_minIndexInclusive = minIdxInclusive;
			_maxIndexInclusive = maxIdxInclusive;
		}

		private protected override Iterator<TElement> Clone()
		{
			return new SkipTakeOrderedIterator<TElement>(_source, _minIndexInclusive, _maxIndexInclusive);
		}

		public override bool MoveNext()
		{
			int num = _state;
			while (true)
			{
				if (num > 1)
				{
					int[] map = _map;
					int num2 = num - 2 + _minIndexInclusive;
					if (num2 > _maxIdx)
					{
						break;
					}
					_current = _buffer[map[num2]];
					_state++;
					return true;
				}
				if (num != 1)
				{
					break;
				}
				TElement[] array = _source._source.ToArray();
				int num3 = array.Length;
				if (num3 <= _minIndexInclusive)
				{
					break;
				}
				_maxIdx = _maxIndexInclusive;
				if (num3 <= _maxIdx)
				{
					_maxIdx = num3 - 1;
				}
				if (_minIndexInclusive == _maxIdx)
				{
					_current = _source.GetEnumerableSorter().ElementAt(array, num3, _minIndexInclusive);
					_state = -1;
					return true;
				}
				_map = _source.SortedMap(array, _minIndexInclusive, _maxIdx);
				_buffer = array;
				num = (_state = 2);
			}
			Dispose();
			return false;
		}

		public override Iterator<TElement> Skip(int count)
		{
			int num = _minIndexInclusive + count;
			if ((uint)num <= (uint)_maxIndexInclusive)
			{
				return new SkipTakeOrderedIterator<TElement>(_source, num, _maxIndexInclusive);
			}
			return null;
		}

		public override Iterator<TElement> Take(int count)
		{
			int num = _minIndexInclusive + count - 1;
			if ((uint)num >= (uint)_maxIndexInclusive)
			{
				return this;
			}
			return new SkipTakeOrderedIterator<TElement>(_source, _minIndexInclusive, num);
		}

		public override TElement TryGetElementAt(int index, out bool found)
		{
			if ((uint)index <= (uint)(_maxIndexInclusive - _minIndexInclusive))
			{
				return _source.TryGetElementAt(index + _minIndexInclusive, out found);
			}
			found = false;
			return default(TElement);
		}

		public override TElement TryGetFirst(out bool found)
		{
			return _source.TryGetElementAt(_minIndexInclusive, out found);
		}

		public override TElement TryGetLast(out bool found)
		{
			return _source.TryGetLast(_minIndexInclusive, _maxIndexInclusive, out found);
		}

		public override TElement[] ToArray()
		{
			return _source.ToArray(_minIndexInclusive, _maxIndexInclusive);
		}

		public override List<TElement> ToList()
		{
			return _source.ToList(_minIndexInclusive, _maxIndexInclusive);
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return _source.GetCount(_minIndexInclusive, _maxIndexInclusive, onlyIfCheap);
		}
	}

	[DebuggerDisplay("Count = {CountForDebugger}")]
	private sealed class RangeIterator<T> : Iterator<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T> where T : INumber<T>
	{
		private readonly T _start;

		private readonly T _endExclusive;

		private int CountForDebugger => int.CreateTruncating(_endExclusive - _start);

		public int Count => int.CreateTruncating(_endExclusive - _start);

		public T this[int index]
		{
			get
			{
				if ((uint)index >= (uint)Count)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
				}
				return _start + T.CreateTruncating(index);
			}
			set
			{
				ThrowHelper.ThrowNotSupportedException();
			}
		}

		public bool IsReadOnly => true;

		public RangeIterator(T start, T endExclusive)
		{
			_start = start;
			_endExclusive = endExclusive;
		}

		private protected override Iterator<T> Clone()
		{
			return new RangeIterator<T>(_start, _endExclusive);
		}

		public override bool MoveNext()
		{
			switch (_state)
			{
			case 1:
				_current = _start;
				_state = 2;
				return true;
			case 2:
				if (!(++_current == _endExclusive))
				{
					return true;
				}
				break;
			}
			_state = -1;
			return false;
		}

		public override void Dispose()
		{
			_state = -1;
		}

		public override IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return new RangeSelectIterator<T, TResult>(_start, _endExclusive, selector);
		}

		public override T[] ToArray()
		{
			T start = _start;
			T[] array = new T[Count];
			FillIncrementing(array, start);
			return array;
		}

		public override List<T> ToList()
		{
			T start = _start;
			T endExclusive = _endExclusive;
			T val = start;
			int num = int.CreateTruncating(endExclusive - val);
			List<T> list = new List<T>(num);
			FillIncrementing(SetCountAndGetSpan(list, num), val);
			return list;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			FillIncrementing(array.AsSpan(arrayIndex, Count), _start);
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return Count;
		}

		public override Iterator<T> Skip(int count)
		{
			if (count < Count)
			{
				return new RangeIterator<T>(_start + T.CreateTruncating(count), _endExclusive);
			}
			return null;
		}

		public override Iterator<T> Take(int count)
		{
			if (count < Count)
			{
				return new RangeIterator<T>(_start, _start + T.CreateTruncating(count));
			}
			return this;
		}

		public override T TryGetElementAt(int index, out bool found)
		{
			if ((uint)index < (uint)Count)
			{
				found = true;
				return _start + T.CreateTruncating(index);
			}
			found = false;
			return T.Zero;
		}

		public override T TryGetFirst(out bool found)
		{
			found = true;
			return _start;
		}

		public override T TryGetLast(out bool found)
		{
			found = true;
			return _endExclusive - T.One;
		}

		public override bool Contains(T item)
		{
			return uint.CreateTruncating(item - _start) < (uint)Count;
		}

		public int IndexOf(T item)
		{
			uint num = uint.CreateTruncating(item - _start);
			if (num < (uint)Count)
			{
				return (int)num;
			}
			return -1;
		}

		void ICollection<T>.Add(T item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void IList<T>.Insert(int index, T item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		bool ICollection<T>.Remove(T item)
		{
			return ThrowHelper.ThrowNotSupportedException_Boolean();
		}

		void IList<T>.RemoveAt(int index)
		{
			ThrowHelper.ThrowNotSupportedException();
		}
	}

	[DebuggerDisplay("Count = {_count}")]
	private sealed class RepeatIterator<TResult> : Iterator<TResult>, IList<TResult>, ICollection<TResult>, IEnumerable<TResult>, IEnumerable, IReadOnlyList<TResult>, IReadOnlyCollection<TResult>
	{
		private readonly int _count;

		public int Count => _count;

		public TResult this[int index]
		{
			get
			{
				if ((uint)index >= (uint)_count)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
				}
				return _current;
			}
			set
			{
				ThrowHelper.ThrowNotSupportedException();
			}
		}

		public bool IsReadOnly => true;

		public RepeatIterator(TResult element, int count)
		{
			_current = element;
			_count = count;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new RepeatIterator<TResult>(_current, _count);
		}

		public override void Dispose()
		{
			_state = -1;
		}

		public override bool MoveNext()
		{
			int num = _state - 1;
			if (num >= 0 && num != _count)
			{
				_state++;
				return true;
			}
			Dispose();
			return false;
		}

		public override TResult[] ToArray()
		{
			TResult[] array = new TResult[_count];
			if (_current != null)
			{
				Array.Fill(array, _current);
			}
			return array;
		}

		public override List<TResult> ToList()
		{
			List<TResult> list = new List<TResult>(_count);
			SetCountAndGetSpan(list, _count).Fill(_current);
			return list;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return _count;
		}

		public override Iterator<TResult> Skip(int count)
		{
			if (count >= _count)
			{
				return null;
			}
			return new RepeatIterator<TResult>(_current, _count - count);
		}

		public override Iterator<TResult> Take(int count)
		{
			if (count >= _count)
			{
				return this;
			}
			return new RepeatIterator<TResult>(_current, count);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if ((uint)index < (uint)_count)
			{
				found = true;
				return _current;
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			found = true;
			return _current;
		}

		public override TResult TryGetLast(out bool found)
		{
			found = true;
			return _current;
		}

		public override bool Contains(TResult item)
		{
			return EqualityComparer<TResult>.Default.Equals(_current, item);
		}

		public int IndexOf(TResult item)
		{
			if (!Contains(item))
			{
				return -1;
			}
			return 0;
		}

		public void CopyTo(TResult[] array, int arrayIndex)
		{
			array.AsSpan(arrayIndex, _count).Fill(_current);
		}

		void ICollection<TResult>.Add(TResult item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void ICollection<TResult>.Clear()
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void IList<TResult>.Insert(int index, TResult item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		bool ICollection<TResult>.Remove(TResult item)
		{
			return ThrowHelper.ThrowNotSupportedException_Boolean();
		}

		void IList<TResult>.RemoveAt(int index)
		{
			ThrowHelper.ThrowNotSupportedException();
		}
	}

	private sealed class ShuffleIterator<TSource> : Iterator<TSource>
	{
		private readonly IEnumerable<TSource> _source;

		private TSource[] _buffer;

		public override TSource[] ToArray()
		{
			TSource[] array = _source.ToArray();
			Random.Shared.Shuffle(array);
			return array;
		}

		public override List<TSource> ToList()
		{
			List<TSource> list = _source.ToList();
			Random.Shared.Shuffle(CollectionsMarshal.AsSpan(list));
			return list;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				if (!_source.TryGetNonEnumeratedCount(out var count))
				{
					return -1;
				}
				return count;
			}
			return _source.Count();
		}

		public override TSource TryGetFirst(out bool found)
		{
			return TryGetElementAt(0, out found);
		}

		public override TSource TryGetLast(out bool found)
		{
			return TryGetElementAt(0, out found);
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (_source is Iterator<TSource> iterator)
			{
				int count = iterator.GetCount(onlyIfCheap: true);
				if (count >= 0)
				{
					if ((uint)index < (uint)count)
					{
						return iterator.TryGetElementAt(Random.Shared.Next(0, count), out found);
					}
					goto IL_008d;
				}
			}
			if (_source is IList<TSource> { Count: var count2 } list)
			{
				if ((uint)index < (uint)count2)
				{
					found = true;
					return list[Random.Shared.Next(0, count2)];
				}
			}
			else if (index >= 0)
			{
				List<TSource> list2 = ShuffleTakeIterator<TSource>.SampleToList(_source, 1, out var totalElementCount);
				if (list2 != null && index < totalElementCount)
				{
					found = true;
					return list2[0];
				}
			}
			goto IL_008d;
			IL_008d:
			found = false;
			return default(TSource);
		}

		public override Iterator<TSource> Take(int count)
		{
			if (_source.TryGetNonEnumeratedCount(out var count2) && count2 <= count)
			{
				return base.Take(count);
			}
			return new ShuffleTakeIterator<TSource>(_source, count);
		}

		public override bool Contains(TSource value)
		{
			return _source.Contains(value);
		}

		public ShuffleIterator(IEnumerable<TSource> source)
		{
			_source = source;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new ShuffleIterator<TSource>(_source);
		}

		public override bool MoveNext()
		{
			int num = _state;
			while (true)
			{
				if (num > 1)
				{
					TSource[] buffer = _buffer;
					int num2 = num - 2;
					if ((uint)num2 >= (uint)buffer.Length)
					{
						break;
					}
					_current = buffer[num2];
					_state++;
					return true;
				}
				if (num != 1)
				{
					break;
				}
				TSource[] array = _source.ToArray();
				if (array.Length == 0)
				{
					break;
				}
				Random.Shared.Shuffle(array);
				_buffer = array;
				num = (_state = 2);
			}
			Dispose();
			return false;
		}

		public override void Dispose()
		{
			_buffer = null;
			base.Dispose();
		}
	}

	private sealed class ShuffleTakeIterator<TSource> : Iterator<TSource>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly int _takeCount;

		private List<TSource> _buffer;

		public ShuffleTakeIterator(IEnumerable<TSource> source, int takeCount)
		{
			_source = source;
			_takeCount = takeCount;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new ShuffleTakeIterator<TSource>(_source, _takeCount);
		}

		public override bool MoveNext()
		{
			int num = _state;
			while (true)
			{
				if (num > 1)
				{
					List<TSource> buffer = _buffer;
					int num2 = num - 2;
					if (num2 >= buffer.Count)
					{
						break;
					}
					_current = buffer[num2];
					_state++;
					return true;
				}
				if (num != 1)
				{
					break;
				}
				List<TSource> list = SampleToList(_source, _takeCount, out var _);
				if (list == null)
				{
					break;
				}
				_buffer = list;
				num = (_state = 2);
			}
			Dispose();
			return false;
		}

		public override void Dispose()
		{
			_buffer = null;
			base.Dispose();
		}

		public override TSource[] ToArray()
		{
			long totalElementCount;
			return SampleToList(_source, _takeCount, out totalElementCount)?.ToArray() ?? Array.Empty<TSource>();
		}

		public override List<TSource> ToList()
		{
			long totalElementCount;
			return SampleToList(_source, _takeCount, out totalElementCount) ?? new List<TSource>();
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!_source.TryGetNonEnumeratedCount(out var count))
			{
				if (onlyIfCheap)
				{
					return -1;
				}
				return _source.Take(_takeCount).Count();
			}
			return Math.Min(_takeCount, count);
		}

		public override TSource TryGetFirst(out bool found)
		{
			return TryGetElementAt(0, out found);
		}

		public override TSource TryGetLast(out bool found)
		{
			return TryGetElementAt(0, out found);
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (_source is Iterator<TSource> iterator)
			{
				int count = iterator.GetCount(onlyIfCheap: true);
				if (count >= 0)
				{
					if ((uint)index < (uint)Math.Min(_takeCount, count))
					{
						return iterator.TryGetElementAt(Random.Shared.Next(0, count), out found);
					}
					goto IL_00b2;
				}
			}
			if (_source is IList<TSource> { Count: var count2 } list)
			{
				if ((uint)index < (uint)Math.Min(_takeCount, count2))
				{
					found = true;
					return list[Random.Shared.Next(0, count2)];
				}
			}
			else if (index >= 0)
			{
				List<TSource> list2 = SampleToList(_source, 1, out var totalElementCount);
				if (list2 != null && index < Math.Min(_takeCount, totalElementCount))
				{
					found = true;
					return list2[0];
				}
			}
			goto IL_00b2;
			IL_00b2:
			found = false;
			return default(TSource);
		}

		public override Iterator<TSource> Take(int count)
		{
			if (_takeCount > count)
			{
				return new ShuffleTakeIterator<TSource>(_source, count);
			}
			return this;
		}

		internal static List<TSource> SampleToList(IEnumerable<TSource> source, int takeCount, out long totalElementCount)
		{
			List<TSource> list = null;
			if (source is IList<TSource> { Count: var count } list2)
			{
				list = new List<TSource>(takeCount);
				for (int i = 0; i < takeCount; i++)
				{
					list.Add(list2[i]);
				}
				for (int j = takeCount; j < count; j++)
				{
					int num = Random.Shared.Next(j + 1);
					if (num < takeCount)
					{
						list[num] = list2[j];
					}
				}
				totalElementCount = count;
			}
			else
			{
				using IEnumerator<TSource> enumerator = source.GetEnumerator();
				if (enumerator.MoveNext())
				{
					list = new List<TSource>(Math.Min(takeCount, 4)) { enumerator.Current };
					while (true)
					{
						if (list.Count < takeCount)
						{
							if (!enumerator.MoveNext())
							{
								totalElementCount = list.Count;
								break;
							}
							list.Add(enumerator.Current);
							continue;
						}
						long num2 = takeCount;
						while (enumerator.MoveNext())
						{
							num2++;
							long num3 = Random.Shared.NextInt64(num2);
							if (num3 < takeCount)
							{
								list[(int)num3] = enumerator.Current;
							}
						}
						totalElementCount = num2;
						break;
					}
				}
				else
				{
					totalElementCount = 0L;
				}
			}
			if (list != null)
			{
				Random.Shared.Shuffle(CollectionsMarshal.AsSpan(list));
			}
			return list;
		}

		public override bool Contains(TSource value)
		{
			long num = 0L;
			long num2;
			if (_source is IList<TSource> list)
			{
				if (list.Count <= _takeCount)
				{
					return list.Contains(value);
				}
				num2 = list.Count;
				if (list.TryGetSpan(out var span))
				{
					num = span.Count(value, null);
				}
				else
				{
					for (int i = 0; i < num2; i++)
					{
						if (EqualityComparer<TSource>.Default.Equals(list[i], value))
						{
							num++;
						}
					}
				}
			}
			else
			{
				if (_source is Iterator<TSource> iterator)
				{
					int count = iterator.GetCount(onlyIfCheap: true);
					if (count >= 0)
					{
						if (count <= _takeCount)
						{
							return iterator.Contains(value);
						}
						num2 = count;
						foreach (TSource item in iterator)
						{
							if (EqualityComparer<TSource>.Default.Equals(item, value))
							{
								num++;
							}
						}
						goto IL_013b;
					}
				}
				num2 = 0L;
				foreach (TSource item2 in _source)
				{
					num2++;
					if (EqualityComparer<TSource>.Default.Equals(item2, value))
					{
						num++;
					}
				}
			}
			goto IL_013b;
			IL_013b:
			if (num == 0L)
			{
				return false;
			}
			if (num == num2)
			{
				return true;
			}
			if (num2 <= _takeCount)
			{
				return num > 0;
			}
			double num3 = 1.0;
			for (long num4 = 0L; num4 < _takeCount; num4++)
			{
				num3 *= (double)(num2 - num - num4) / (double)(num2 - num4);
			}
			return Random.Shared.NextDouble() > num3;
		}
	}

	private sealed class ReverseIterator<TSource> : Iterator<TSource>
	{
		private readonly IEnumerable<TSource> _source;

		private TSource[] _buffer;

		public ReverseIterator(IEnumerable<TSource> source)
		{
			_source = source;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new ReverseIterator<TSource>(_source);
		}

		public override bool MoveNext()
		{
			if (_state - 2 <= -2)
			{
				Dispose();
				return false;
			}
			if (_state == 1)
			{
				_state = (_buffer = _source.ToArray()).Length + 2;
			}
			int num = _state - 3;
			if (num != -1)
			{
				_current = _buffer[num];
				_state--;
				return true;
			}
			Dispose();
			return false;
		}

		public override void Dispose()
		{
			_buffer = null;
			base.Dispose();
		}

		public override TSource[] ToArray()
		{
			TSource[] array = _source.ToArray();
			Array.Reverse(array);
			return array;
		}

		public override List<TSource> ToList()
		{
			List<TSource> list = _source.ToList();
			list.Reverse();
			return list;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				if (!_source.TryGetNonEnumeratedCount(out var count))
				{
					return -1;
				}
				return count;
			}
			return _source.Count();
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (_source is IList<TSource> { Count: var count } list)
			{
				if ((uint)index < (uint)count)
				{
					found = true;
					return list[count - index - 1];
				}
			}
			else if (index >= 0)
			{
				TSource[] array = _source.ToArray();
				if (index < array.Length)
				{
					found = true;
					return array[array.Length - index - 1];
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetFirst(out bool found)
		{
			if (_source is Iterator<TSource> iterator)
			{
				return iterator.TryGetLast(out found);
			}
			if (_source is IList<TSource> { Count: var count } list)
			{
				if (count > 0)
				{
					found = true;
					return list[count - 1];
				}
			}
			else
			{
				using IEnumerator<TSource> enumerator = _source.GetEnumerator();
				if (enumerator.MoveNext())
				{
					TSource current;
					do
					{
						current = enumerator.Current;
					}
					while (enumerator.MoveNext());
					found = true;
					return current;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetLast(out bool found)
		{
			if (_source is Iterator<TSource> iterator)
			{
				return iterator.TryGetFirst(out found);
			}
			if (_source is IList<TSource> list)
			{
				if (list.Count > 0)
				{
					found = true;
					return list[0];
				}
			}
			else
			{
				using IEnumerator<TSource> enumerator = _source.GetEnumerator();
				if (enumerator.MoveNext())
				{
					found = true;
					return enumerator.Current;
				}
			}
			found = false;
			return default(TSource);
		}

		public override bool Contains(TSource value)
		{
			return _source.Contains(value);
		}
	}

	private sealed class IEnumerableSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, TResult> _selector;

		private IEnumerator<TSource> _enumerator;

		public IEnumerableSelectIterator(IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			_source = source;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new IEnumerableSelectIterator<TSource, TResult>(_source, _selector);
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_005a;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			if (_enumerator.MoveNext())
			{
				_current = _selector(_enumerator.Current);
				return true;
			}
			Dispose();
			goto IL_005a;
			IL_005a:
			return false;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new IEnumerableSelectIterator<TSource, TResult2>(_source, Utilities.CombineSelectors(_selector, selector));
		}

		public override TResult[] ToArray()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, TResult> selector = _selector;
			foreach (TSource item in _source)
			{
				segmentedArrayBuilder.Add(selector(item));
			}
			TResult[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TResult> ToList()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, TResult> selector = _selector;
			foreach (TSource item in _source)
			{
				segmentedArrayBuilder.Add(selector(item));
			}
			List<TResult> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			foreach (TSource item in _source)
			{
				_selector(item);
				num = checked(num + 1);
			}
			return num;
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				using IEnumerator<TSource> enumerator = _source.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (index == 0)
					{
						found = true;
						return _selector(enumerator.Current);
					}
					index--;
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			using IEnumerator<TSource> enumerator = _source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				found = true;
				return _selector(enumerator.Current);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			using IEnumerator<TSource> enumerator = _source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				found = true;
				TSource current = enumerator.Current;
				while (enumerator.MoveNext())
				{
					current = enumerator.Current;
				}
				return _selector(current);
			}
			found = false;
			return default(TResult);
		}
	}

	[DebuggerDisplay("Count = {CountForDebugger}")]
	private sealed class ArraySelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly TSource[] _source;

		private readonly Func<TSource, TResult> _selector;

		private int CountForDebugger => _source.Length;

		public ArraySelectIterator(TSource[] source, Func<TSource, TResult> selector)
		{
			_source = source;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new ArraySelectIterator<TSource, TResult>(_source, _selector);
		}

		public override bool MoveNext()
		{
			TSource[] source = _source;
			int num = _state - 1;
			if ((uint)num < (uint)source.Length)
			{
				_state++;
				_current = _selector(source[num]);
				return true;
			}
			Dispose();
			return false;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new ArraySelectIterator<TSource, TResult2>(_source, Utilities.CombineSelectors(_selector, selector));
		}

		public override TResult[] ToArray()
		{
			TSource[] source = _source;
			TResult[] array = new TResult[source.Length];
			Fill(source, array, _selector);
			return array;
		}

		public override List<TResult> ToList()
		{
			TSource[] source = _source;
			List<TResult> list = new List<TResult>(source.Length);
			Fill(source, SetCountAndGetSpan(list, source.Length), _selector);
			return list;
		}

		private static void Fill(ReadOnlySpan<TSource> source, Span<TResult> destination, Func<TSource, TResult> func)
		{
			for (int i = 0; i < destination.Length; i++)
			{
				destination[i] = func(source[i]);
			}
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				TSource[] source = _source;
				foreach (TSource arg in source)
				{
					_selector(arg);
				}
			}
			return _source.Length;
		}

		public override Iterator<TResult> Skip(int count)
		{
			if (count >= _source.Length)
			{
				return null;
			}
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, count, int.MaxValue);
		}

		public override Iterator<TResult> Take(int count)
		{
			if (count < _source.Length)
			{
				return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, 0, count - 1);
			}
			return this;
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			TSource[] source = _source;
			if ((uint)index < (uint)source.Length)
			{
				found = true;
				return _selector(source[index]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			found = true;
			return _selector(_source[0]);
		}

		public override TResult TryGetLast(out bool found)
		{
			found = true;
			return _selector(_source[^1]);
		}

		public override bool Contains(TResult value)
		{
			TSource[] source = _source;
			foreach (TSource arg in source)
			{
				if (EqualityComparer<TResult>.Default.Equals(_selector(arg), value))
				{
					return true;
				}
			}
			return false;
		}
	}

	[DebuggerDisplay("Count = {CountForDebugger}")]
	private sealed class ListSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly List<TSource> _source;

		private readonly Func<TSource, TResult> _selector;

		private List<TSource>.Enumerator _enumerator;

		private int CountForDebugger => _source.Count;

		public ListSelectIterator(List<TSource> source, Func<TSource, TResult> selector)
		{
			_source = source;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new ListSelectIterator<TSource, TResult>(_source, _selector);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_005a;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			if (_enumerator.MoveNext())
			{
				_current = _selector(_enumerator.Current);
				return true;
			}
			Dispose();
			goto IL_005a;
			IL_005a:
			return false;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new ListSelectIterator<TSource, TResult2>(_source, Utilities.CombineSelectors(_selector, selector));
		}

		public override TResult[] ToArray()
		{
			ReadOnlySpan<TSource> source = CollectionsMarshal.AsSpan(_source);
			if (source.Length == 0)
			{
				return Array.Empty<TResult>();
			}
			TResult[] array = new TResult[source.Length];
			Fill(source, array, _selector);
			return array;
		}

		public override List<TResult> ToList()
		{
			ReadOnlySpan<TSource> source = CollectionsMarshal.AsSpan(_source);
			List<TResult> list = new List<TResult>(source.Length);
			Fill(source, SetCountAndGetSpan(list, source.Length), _selector);
			return list;
		}

		private static void Fill(ReadOnlySpan<TSource> source, Span<TResult> destination, Func<TSource, TResult> func)
		{
			for (int i = 0; i < destination.Length; i++)
			{
				destination[i] = func(source[i]);
			}
		}

		public override int GetCount(bool onlyIfCheap)
		{
			int count = _source.Count;
			if (!onlyIfCheap)
			{
				for (int i = 0; i < count; i++)
				{
					_selector(_source[i]);
				}
			}
			return count;
		}

		public override Iterator<TResult> Skip(int count)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, count, int.MaxValue);
		}

		public override Iterator<TResult> Take(int count)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, 0, count - 1);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if ((uint)index < (uint)_source.Count)
			{
				found = true;
				return _selector(_source[index]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			if (_source.Count != 0)
			{
				found = true;
				return _selector(_source[0]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			int count = _source.Count;
			if (count != 0)
			{
				found = true;
				return _selector(_source[count - 1]);
			}
			found = false;
			return default(TResult);
		}

		public override bool Contains(TResult value)
		{
			int count = _source.Count;
			for (int i = 0; i < count; i++)
			{
				if (EqualityComparer<TResult>.Default.Equals(_selector(_source[i]), value))
				{
					return true;
				}
			}
			return false;
		}
	}

	[DebuggerDisplay("Count = {CountForDebugger}")]
	private sealed class IListSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly IList<TSource> _source;

		private readonly Func<TSource, TResult> _selector;

		private IEnumerator<TSource> _enumerator;

		private int CountForDebugger => _source.Count;

		public IListSelectIterator(IList<TSource> source, Func<TSource, TResult> selector)
		{
			_source = source;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new IListSelectIterator<TSource, TResult>(_source, _selector);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_005a;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			if (_enumerator.MoveNext())
			{
				_current = _selector(_enumerator.Current);
				return true;
			}
			Dispose();
			goto IL_005a;
			IL_005a:
			return false;
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new IListSelectIterator<TSource, TResult2>(_source, Utilities.CombineSelectors(_selector, selector));
		}

		public override TResult[] ToArray()
		{
			int count = _source.Count;
			if (count == 0)
			{
				return Array.Empty<TResult>();
			}
			TResult[] array = new TResult[count];
			Fill(_source, array, _selector);
			return array;
		}

		public override List<TResult> ToList()
		{
			IList<TSource> source = _source;
			int count = _source.Count;
			List<TResult> list = new List<TResult>(count);
			Fill(source, SetCountAndGetSpan(list, count), _selector);
			return list;
		}

		private static void Fill(IList<TSource> source, Span<TResult> results, Func<TSource, TResult> func)
		{
			for (int i = 0; i < results.Length; i++)
			{
				results[i] = func(source[i]);
			}
		}

		public override int GetCount(bool onlyIfCheap)
		{
			int count = _source.Count;
			if (!onlyIfCheap)
			{
				for (int i = 0; i < count; i++)
				{
					_selector(_source[i]);
				}
			}
			return count;
		}

		public override Iterator<TResult> Skip(int count)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, count, int.MaxValue);
		}

		public override Iterator<TResult> Take(int count)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, 0, count - 1);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if ((uint)index < (uint)_source.Count)
			{
				found = true;
				return _selector(_source[index]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			if (_source.Count != 0)
			{
				found = true;
				return _selector(_source[0]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			int count = _source.Count;
			if (count != 0)
			{
				found = true;
				return _selector(_source[count - 1]);
			}
			found = false;
			return default(TResult);
		}

		public override bool Contains(TResult value)
		{
			int count = _source.Count;
			for (int i = 0; i < count; i++)
			{
				if (EqualityComparer<TResult>.Default.Equals(_selector(_source[i]), value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class SizeOptIListSelectIterator<TSource, TResult>(IList<TSource> _source, Func<TSource, TResult> _selector) : Iterator<TResult>
	{
		private IEnumerator<TSource> _enumerator;

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			foreach (TSource item in _source)
			{
				_selector(item);
				num = checked(num + 1);
			}
			return num;
		}

		public override Iterator<TResult> Skip(int count)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, count, int.MaxValue);
		}

		public override Iterator<TResult> Take(int count)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, 0, count - 1);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_005a;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			if (_enumerator.MoveNext())
			{
				_current = _selector(_enumerator.Current);
				return true;
			}
			Dispose();
			goto IL_005a;
			IL_005a:
			return false;
		}

		public override TResult[] ToArray()
		{
			TResult[] array = new TResult[_source.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = _selector(_source[i]);
			}
			return array;
		}

		public override List<TResult> ToList()
		{
			List<TResult> list = new List<TResult>(_source.Count);
			for (int i = 0; i < list.Count; i++)
			{
				list.Add(_selector(_source[i]));
			}
			return list;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new SizeOptIListSelectIterator<TSource, TResult>(_source, _selector);
		}
	}

	private sealed class RangeSelectIterator<T, TResult> : Iterator<TResult> where T : INumber<T>
	{
		private readonly T _start;

		private readonly T _end;

		private readonly Func<T, TResult> _selector;

		private int Count => int.CreateTruncating(_end - _start);

		public RangeSelectIterator(T start, T end, Func<T, TResult> selector)
		{
			_start = start;
			_end = end;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new RangeSelectIterator<T, TResult>(_start, _end, _selector);
		}

		public override bool MoveNext()
		{
			if (_state < 1 || _state == Count + 1)
			{
				Dispose();
				return false;
			}
			int value = _state++ - 1;
			_current = _selector(_start + T.CreateTruncating(value));
			return true;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new RangeSelectIterator<T, TResult2>(_start, _end, Utilities.CombineSelectors(_selector, selector));
		}

		public override TResult[] ToArray()
		{
			TResult[] array = new TResult[Count];
			Fill(array, _start, _selector);
			return array;
		}

		public override List<TResult> ToList()
		{
			int count = Count;
			List<TResult> list = new List<TResult>(count);
			Fill(SetCountAndGetSpan(list, count), _start, _selector);
			return list;
		}

		private static void Fill(Span<TResult> results, T start, Func<T, TResult> func)
		{
			int num = 0;
			while (num < results.Length)
			{
				results[num] = func(start);
				num++;
				++start;
			}
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				for (T start = _start; start != _end; ++start)
				{
					_selector(start);
				}
			}
			return Count;
		}

		public override Iterator<TResult> Skip(int count)
		{
			if (count >= Count)
			{
				return null;
			}
			return new RangeSelectIterator<T, TResult>(_start + T.CreateTruncating(count), _end, _selector);
		}

		public override Iterator<TResult> Take(int count)
		{
			if (count >= Count)
			{
				return this;
			}
			return new RangeSelectIterator<T, TResult>(_start, _start + T.CreateTruncating(count), _selector);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if ((uint)index < (uint)Count)
			{
				found = true;
				return _selector(_start + T.CreateTruncating(index));
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			found = true;
			return _selector(_start);
		}

		public override TResult TryGetLast(out bool found)
		{
			found = true;
			return _selector(_end - T.One);
		}

		public override bool Contains(TResult value)
		{
			for (T start = _start; start != _end; ++start)
			{
				if (EqualityComparer<TResult>.Default.Equals(_selector(start), value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class IteratorSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly Iterator<TSource> _source;

		private readonly Func<TSource, TResult> _selector;

		private Iterator<TSource> _enumerator;

		public IteratorSelectIterator(Iterator<TSource> source, Func<TSource, TResult> selector)
		{
			_source = source;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new IteratorSelectIterator<TSource, TResult>(_source, _selector);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_005a;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			if (_enumerator.MoveNext())
			{
				_current = _selector(_enumerator.Current);
				return true;
			}
			Dispose();
			goto IL_005a;
			IL_005a:
			return false;
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new IteratorSelectIterator<TSource, TResult2>(_source, Utilities.CombineSelectors(_selector, selector));
		}

		public override Iterator<TResult> Skip(int count)
		{
			Iterator<TSource> iterator = _source.Skip(count);
			if (iterator != null)
			{
				return new IteratorSelectIterator<TSource, TResult>(iterator, _selector);
			}
			return null;
		}

		public override Iterator<TResult> Take(int count)
		{
			Iterator<TSource> iterator = _source.Take(count);
			if (iterator != null)
			{
				return new IteratorSelectIterator<TSource, TResult>(iterator, _selector);
			}
			return null;
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			TSource arg = _source.TryGetElementAt(index, out var found2);
			found = found2;
			if (!found2)
			{
				return default(TResult);
			}
			return _selector(arg);
		}

		public override TResult TryGetFirst(out bool found)
		{
			TSource arg = _source.TryGetFirst(out var found2);
			found = found2;
			if (!found2)
			{
				return default(TResult);
			}
			return _selector(arg);
		}

		public override TResult TryGetLast(out bool found)
		{
			TSource arg = _source.TryGetLast(out var found2);
			found = found2;
			if (!found2)
			{
				return default(TResult);
			}
			return _selector(arg);
		}

		private TResult[] ToArrayNoPresizing()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, TResult> selector = _selector;
			foreach (TSource item in _source)
			{
				segmentedArrayBuilder.Add(selector(item));
			}
			TResult[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		private TResult[] PreallocatingToArray(int count)
		{
			TResult[] array = new TResult[count];
			Fill(_source, array, _selector);
			return array;
		}

		public override TResult[] ToArray()
		{
			int count = _source.GetCount(onlyIfCheap: true);
			return count switch
			{
				-1 => ToArrayNoPresizing(), 
				0 => Array.Empty<TResult>(), 
				_ => PreallocatingToArray(count), 
			};
		}

		private List<TResult> ToListNoPresizing()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, TResult> selector = _selector;
			foreach (TSource item in _source)
			{
				segmentedArrayBuilder.Add(selector(item));
			}
			List<TResult> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TResult> ToList()
		{
			int count = _source.GetCount(onlyIfCheap: true);
			List<TResult> list;
			switch (count)
			{
			case -1:
				list = ToListNoPresizing();
				break;
			case 0:
				list = new List<TResult>();
				break;
			default:
				list = new List<TResult>(count);
				Fill(_source, SetCountAndGetSpan(list, count), _selector);
				break;
			}
			return list;
		}

		private static void Fill(Iterator<TSource> source, Span<TResult> results, Func<TSource, TResult> func)
		{
			int num = 0;
			foreach (TSource item in source)
			{
				results[num] = func(item);
				num++;
			}
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				int num = 0;
				{
					foreach (TSource item in _source)
					{
						_selector(item);
						num = checked(num + 1);
					}
					return num;
				}
			}
			return _source.GetCount(onlyIfCheap);
		}
	}

	[DebuggerDisplay("Count = {Count}")]
	private sealed class IListSkipTakeSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly IList<TSource> _source;

		private readonly Func<TSource, TResult> _selector;

		private readonly int _minIndexInclusive;

		private readonly int _maxIndexInclusive;

		private int Count
		{
			get
			{
				int count = _source.Count;
				if (count <= _minIndexInclusive)
				{
					return 0;
				}
				return Math.Min(count - 1, _maxIndexInclusive) - _minIndexInclusive + 1;
			}
		}

		public IListSkipTakeSelectIterator(IList<TSource> source, Func<TSource, TResult> selector, int minIndexInclusive, int maxIndexInclusive)
		{
			_source = source;
			_selector = selector;
			_minIndexInclusive = minIndexInclusive;
			_maxIndexInclusive = maxIndexInclusive;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, _minIndexInclusive, _maxIndexInclusive);
		}

		public override bool MoveNext()
		{
			int num = _state - 1;
			if ((uint)num <= (uint)(_maxIndexInclusive - _minIndexInclusive) && num < _source.Count - _minIndexInclusive)
			{
				_current = _selector(_source[_minIndexInclusive + num]);
				_state++;
				return true;
			}
			Dispose();
			return false;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult2>(_source, Utilities.CombineSelectors(_selector, selector), _minIndexInclusive, _maxIndexInclusive);
		}

		public override Iterator<TResult> Skip(int count)
		{
			int num = _minIndexInclusive + count;
			if ((uint)num <= (uint)_maxIndexInclusive)
			{
				return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, num, _maxIndexInclusive);
			}
			return null;
		}

		public override Iterator<TResult> Take(int count)
		{
			int num = _minIndexInclusive + count - 1;
			if ((uint)num < (uint)_maxIndexInclusive)
			{
				return new IListSkipTakeSelectIterator<TSource, TResult>(_source, _selector, _minIndexInclusive, num);
			}
			return this;
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if ((uint)index <= (uint)(_maxIndexInclusive - _minIndexInclusive) && index < _source.Count - _minIndexInclusive)
			{
				found = true;
				return _selector(_source[_minIndexInclusive + index]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetFirst(out bool found)
		{
			if (_source.Count > _minIndexInclusive)
			{
				found = true;
				return _selector(_source[_minIndexInclusive]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			int num = _source.Count - 1;
			if (num >= _minIndexInclusive)
			{
				found = true;
				return _selector(_source[Math.Min(num, _maxIndexInclusive)]);
			}
			found = false;
			return default(TResult);
		}

		public override TResult[] ToArray()
		{
			int count = Count;
			if (count == 0)
			{
				return Array.Empty<TResult>();
			}
			TResult[] array = new TResult[count];
			Fill(_source, array, _selector, _minIndexInclusive);
			return array;
		}

		public override List<TResult> ToList()
		{
			int count = Count;
			if (count == 0)
			{
				return new List<TResult>();
			}
			List<TResult> list = new List<TResult>(count);
			Fill(_source, SetCountAndGetSpan(list, count), _selector, _minIndexInclusive);
			return list;
		}

		private static void Fill(IList<TSource> source, Span<TResult> destination, Func<TSource, TResult> func, int sourceIndex)
		{
			int num = 0;
			while (num < destination.Length)
			{
				destination[num] = func(source[sourceIndex]);
				num++;
				sourceIndex++;
			}
		}

		public override int GetCount(bool onlyIfCheap)
		{
			int count = Count;
			if (!onlyIfCheap)
			{
				int num = _minIndexInclusive + count;
				for (int i = _minIndexInclusive; i != num; i++)
				{
					_selector(_source[i]);
				}
			}
			return count;
		}

		public override bool Contains(TResult value)
		{
			int count = Count;
			int num = _minIndexInclusive + count;
			for (int i = _minIndexInclusive; i != num; i++)
			{
				if (EqualityComparer<TResult>.Default.Equals(_selector(_source[i]), value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class SelectManySingleSelectorIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, IEnumerable<TResult>> _selector;

		private IEnumerator<TSource> _sourceEnumerator;

		private IEnumerator<TResult> _subEnumerator;

		internal SelectManySingleSelectorIterator(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			_source = source;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new SelectManySingleSelectorIterator<TSource, TResult>(_source, _selector);
		}

		public override void Dispose()
		{
			if (_subEnumerator != null)
			{
				_subEnumerator.Dispose();
				_subEnumerator = null;
			}
			if (_sourceEnumerator != null)
			{
				_sourceEnumerator.Dispose();
				_sourceEnumerator = null;
			}
			base.Dispose();
		}

		public override bool MoveNext()
		{
			switch (_state)
			{
			case 1:
				_sourceEnumerator = _source.GetEnumerator();
				_state = 2;
				goto case 2;
			case 2:
			{
				if (!_sourceEnumerator.MoveNext())
				{
					break;
				}
				TSource current = _sourceEnumerator.Current;
				_subEnumerator = _selector(current).GetEnumerator();
				_state = 3;
				goto case 3;
			}
			case 3:
				if (!_subEnumerator.MoveNext())
				{
					_subEnumerator.Dispose();
					_subEnumerator = null;
					_state = 2;
					goto case 2;
				}
				_current = _subEnumerator.Current;
				return true;
			}
			Dispose();
			return false;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			foreach (TSource item in _source)
			{
				num = checked(num + _selector(item).Count());
			}
			return num;
		}

		public override TResult[] ToArray()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, IEnumerable<TResult>> selector = _selector;
			foreach (TSource item in _source)
			{
				segmentedArrayBuilder.AddRange(selector(item));
			}
			TResult[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TResult> ToList()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, IEnumerable<TResult>> selector = _selector;
			foreach (TSource item in _source)
			{
				segmentedArrayBuilder.AddRange(selector(item));
			}
			List<TResult> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override bool Contains(TResult value)
		{
			foreach (TSource item in _source)
			{
				if (_selector(item).Contains(value))
				{
					return true;
				}
			}
			return false;
		}
	}

	[DebuggerDisplay("Count = {Count}")]
	private sealed class IListSkipTakeIterator<TSource> : Iterator<TSource>, IList<TSource>, ICollection<TSource>, IEnumerable<TSource>, IEnumerable, IReadOnlyList<TSource>, IReadOnlyCollection<TSource>
	{
		private readonly IList<TSource> _source;

		private readonly int _minIndexInclusive;

		private readonly int _maxIndexInclusive;

		public int Count => GetAdjustedCount(_minIndexInclusive, _maxIndexInclusive, _source.Count);

		public TSource this[int index]
		{
			get
			{
				if ((uint)index >= (uint)Count)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
				}
				return _source[_minIndexInclusive + index];
			}
			set
			{
				ThrowHelper.ThrowNotSupportedException();
			}
		}

		public bool IsReadOnly => true;

		public IListSkipTakeIterator(IList<TSource> source, int minIndexInclusive, int maxIndexInclusive)
		{
			_source = source;
			_minIndexInclusive = minIndexInclusive;
			_maxIndexInclusive = maxIndexInclusive;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new IListSkipTakeIterator<TSource>(_source, _minIndexInclusive, _maxIndexInclusive);
		}

		public override bool MoveNext()
		{
			int num = _state - 1;
			if ((uint)num <= (uint)(_maxIndexInclusive - _minIndexInclusive) && num < _source.Count - _minIndexInclusive)
			{
				_current = _source[_minIndexInclusive + num];
				_state++;
				return true;
			}
			Dispose();
			return false;
		}

		public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
		{
			return new IListSkipTakeSelectIterator<TSource, TResult>(_source, selector, _minIndexInclusive, _maxIndexInclusive);
		}

		public override Iterator<TSource> Skip(int count)
		{
			int num = _minIndexInclusive + count;
			if ((uint)num <= (uint)_maxIndexInclusive)
			{
				return new IListSkipTakeIterator<TSource>(_source, num, _maxIndexInclusive);
			}
			return null;
		}

		public override Iterator<TSource> Take(int count)
		{
			int num = _minIndexInclusive + count - 1;
			if ((uint)num < (uint)_maxIndexInclusive)
			{
				return new IListSkipTakeIterator<TSource>(_source, _minIndexInclusive, num);
			}
			return this;
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if ((uint)index <= (uint)(_maxIndexInclusive - _minIndexInclusive) && index < _source.Count - _minIndexInclusive)
			{
				found = true;
				return _source[_minIndexInclusive + index];
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetFirst(out bool found)
		{
			if (_source.Count > _minIndexInclusive)
			{
				found = true;
				return _source[_minIndexInclusive];
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetLast(out bool found)
		{
			int num = _source.Count - 1;
			if (num >= _minIndexInclusive)
			{
				found = true;
				return _source[Math.Min(num, _maxIndexInclusive)];
			}
			found = false;
			return default(TSource);
		}

		private static int GetAdjustedCount(int minIndexInclusive, int maxIndexInclusive, int sourceCount)
		{
			if (sourceCount <= minIndexInclusive)
			{
				return 0;
			}
			return Math.Min(sourceCount - 1, maxIndexInclusive) - minIndexInclusive + 1;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return Count;
		}

		public override TSource[] ToArray()
		{
			int count = Count;
			if (count == 0)
			{
				return Array.Empty<TSource>();
			}
			TSource[] array = new TSource[count];
			Fill(_source, array, _minIndexInclusive);
			return array;
		}

		public override List<TSource> ToList()
		{
			int count = Count;
			List<TSource> list = new List<TSource>();
			if (count != 0)
			{
				Fill(_source, SetCountAndGetSpan(list, count), _minIndexInclusive);
			}
			return list;
		}

		public void CopyTo(TSource[] array, int arrayIndex)
		{
			Fill(_source, array.AsSpan(arrayIndex, Count), _minIndexInclusive);
		}

		private static void Fill(IList<TSource> source, Span<TSource> destination, int sourceIndex)
		{
			if (source.TryGetSpan(out var span))
			{
				span.Slice(sourceIndex, destination.Length).CopyTo(destination);
				return;
			}
			int num = 0;
			while (num < destination.Length)
			{
				destination[num] = source[sourceIndex];
				num++;
				sourceIndex++;
			}
		}

		public override bool Contains(TSource item)
		{
			return IndexOf(item) >= 0;
		}

		public int IndexOf(TSource item)
		{
			IList<TSource> source = _source;
			if (source.TryGetSpan(out var span))
			{
				int minIndexInclusive = _minIndexInclusive;
				if (minIndexInclusive < span.Length)
				{
					return span.Slice(minIndexInclusive, GetAdjustedCount(minIndexInclusive, _maxIndexInclusive, span.Length)).IndexOf(item, null);
				}
			}
			else
			{
				int num = _minIndexInclusive + Count;
				for (int i = _minIndexInclusive; i < num; i++)
				{
					if (EqualityComparer<TSource>.Default.Equals(source[i], item))
					{
						return i - _minIndexInclusive;
					}
				}
			}
			return -1;
		}

		void ICollection<TSource>.Add(TSource item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void ICollection<TSource>.Clear()
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		void IList<TSource>.Insert(int index, TSource item)
		{
			ThrowHelper.ThrowNotSupportedException();
		}

		bool ICollection<TSource>.Remove(TSource item)
		{
			return ThrowHelper.ThrowNotSupportedException_Boolean();
		}

		void IList<TSource>.RemoveAt(int index)
		{
			ThrowHelper.ThrowNotSupportedException();
		}
	}

	private sealed class IEnumerableSkipTakeIterator<TSource> : Iterator<TSource>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly int _minIndexInclusive;

		private readonly int _maxIndexInclusive;

		private IEnumerator<TSource> _enumerator;

		private bool HasLimit => _maxIndexInclusive != -1;

		private int Limit => _maxIndexInclusive + 1 - _minIndexInclusive;

		internal IEnumerableSkipTakeIterator(IEnumerable<TSource> source, int minIndexInclusive, int maxIndexInclusive)
		{
			_source = source;
			_minIndexInclusive = minIndexInclusive;
			_maxIndexInclusive = maxIndexInclusive;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new IEnumerableSkipTakeIterator<TSource>(_source, _minIndexInclusive, _maxIndexInclusive);
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			if (!HasLimit)
			{
				return Math.Max(_source.Count() - _minIndexInclusive, 0);
			}
			using IEnumerator<TSource> en = _source.GetEnumerator();
			return Math.Max((int)SkipAndCount((uint)(_maxIndexInclusive + 1), en) - _minIndexInclusive, 0);
		}

		public override bool MoveNext()
		{
			int num = _state - 3;
			if (num < -2)
			{
				Dispose();
				return false;
			}
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_0054;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			if (SkipBeforeFirst(_enumerator))
			{
				_state = 3;
				goto IL_0054;
			}
			goto IL_009b;
			IL_009b:
			Dispose();
			return false;
			IL_0054:
			if ((!HasLimit || num < Limit) && _enumerator.MoveNext())
			{
				if (HasLimit)
				{
					_state++;
				}
				_current = _enumerator.Current;
				return true;
			}
			goto IL_009b;
		}

		public override Iterator<TSource> Skip(int count)
		{
			int num = _minIndexInclusive + count;
			if (!HasLimit)
			{
				if (num < 0)
				{
					return new IEnumerableSkipTakeIterator<TSource>(this, count, -1);
				}
			}
			else if ((uint)num > (uint)_maxIndexInclusive)
			{
				return null;
			}
			return new IEnumerableSkipTakeIterator<TSource>(_source, num, _maxIndexInclusive);
		}

		public override Iterator<TSource> Take(int count)
		{
			int num = _minIndexInclusive + count - 1;
			if (!HasLimit)
			{
				if (num < 0)
				{
					return new IEnumerableSkipTakeIterator<TSource>(this, 0, count - 1);
				}
			}
			else if ((uint)num >= (uint)_maxIndexInclusive)
			{
				return this;
			}
			return new IEnumerableSkipTakeIterator<TSource>(_source, _minIndexInclusive, num);
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (index >= 0 && (!HasLimit || index < Limit))
			{
				if (_source is Iterator<TSource> iterator)
				{
					return iterator.TryGetElementAt(_minIndexInclusive + index, out found);
				}
				using IEnumerator<TSource> enumerator = _source.GetEnumerator();
				if (SkipBefore(_minIndexInclusive + index, enumerator) && enumerator.MoveNext())
				{
					found = true;
					return enumerator.Current;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetFirst(out bool found)
		{
			if (_source is Iterator<TSource> iterator)
			{
				return iterator.TryGetElementAt(_minIndexInclusive, out found);
			}
			using (IEnumerator<TSource> enumerator = _source.GetEnumerator())
			{
				if (SkipBeforeFirst(enumerator) && enumerator.MoveNext())
				{
					found = true;
					return enumerator.Current;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetLast(out bool found)
		{
			if (_source is Iterator<TSource> iterator)
			{
				int count = iterator.GetCount(onlyIfCheap: true);
				if (count > _minIndexInclusive)
				{
					if ((uint)count > (uint)_maxIndexInclusive)
					{
						return iterator.TryGetElementAt(_maxIndexInclusive, out found);
					}
					return iterator.TryGetLast(out found);
				}
			}
			using (IEnumerator<TSource> enumerator = _source.GetEnumerator())
			{
				if (SkipBeforeFirst(enumerator) && enumerator.MoveNext())
				{
					int num = Limit - 1;
					int num2 = ((!HasLimit) ? int.MinValue : 0);
					TSource current;
					do
					{
						num--;
						current = enumerator.Current;
					}
					while (num >= num2 && enumerator.MoveNext());
					found = true;
					return current;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource[] ToArray()
		{
			using (IEnumerator<TSource> enumerator = _source.GetEnumerator())
			{
				if (SkipBeforeFirst(enumerator) && enumerator.MoveNext())
				{
					int num = Limit - 1;
					int num2 = ((!HasLimit) ? int.MinValue : 0);
					SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
					SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
					do
					{
						num--;
						segmentedArrayBuilder.Add(enumerator.Current);
					}
					while (num >= num2 && enumerator.MoveNext());
					TSource[] result = segmentedArrayBuilder.ToArray();
					segmentedArrayBuilder.Dispose();
					return result;
				}
			}
			return Array.Empty<TSource>();
		}

		public override List<TSource> ToList()
		{
			using (IEnumerator<TSource> enumerator = _source.GetEnumerator())
			{
				if (SkipBeforeFirst(enumerator) && enumerator.MoveNext())
				{
					int num = Limit - 1;
					int num2 = ((!HasLimit) ? int.MinValue : 0);
					SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
					SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
					do
					{
						num--;
						segmentedArrayBuilder.Add(enumerator.Current);
					}
					while (num >= num2 && enumerator.MoveNext());
					List<TSource> result = segmentedArrayBuilder.ToList();
					segmentedArrayBuilder.Dispose();
					return result;
				}
			}
			return new List<TSource>();
		}

		private bool SkipBeforeFirst(IEnumerator<TSource> en)
		{
			return SkipBefore(_minIndexInclusive, en);
		}

		private static bool SkipBefore(int index, IEnumerator<TSource> en)
		{
			return SkipAndCount(index, en) == index;
		}

		private static int SkipAndCount(int index, IEnumerator<TSource> en)
		{
			return (int)SkipAndCount((uint)index, en);
		}

		private static uint SkipAndCount(uint index, IEnumerator<TSource> en)
		{
			for (uint num = 0u; num < index; num++)
			{
				if (!en.MoveNext())
				{
					return num;
				}
			}
			return index;
		}
	}

	private abstract class UnionIterator<TSource> : Iterator<TSource>
	{
		internal readonly IEqualityComparer<TSource> _comparer;

		private IEnumerator<TSource> _enumerator;

		private HashSet<TSource> _set;

		protected UnionIterator(IEqualityComparer<TSource> comparer)
		{
			_comparer = comparer;
		}

		public sealed override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
				_set = null;
			}
			base.Dispose();
		}

		internal abstract IEnumerable<TSource> GetEnumerable(int index);

		internal abstract UnionIterator<TSource> Union(IEnumerable<TSource> next);

		private void SetEnumerator(IEnumerator<TSource> enumerator)
		{
			_enumerator?.Dispose();
			_enumerator = enumerator;
		}

		private void StoreFirst()
		{
			HashSet<TSource> hashSet = new HashSet<TSource>(7, _comparer);
			TSource current = _enumerator.Current;
			hashSet.Add(current);
			_current = current;
			_set = hashSet;
		}

		private bool GetNext()
		{
			HashSet<TSource> set = _set;
			while (_enumerator.MoveNext())
			{
				TSource current = _enumerator.Current;
				if (set.Add(current))
				{
					_current = current;
					return true;
				}
			}
			return false;
		}

		public sealed override bool MoveNext()
		{
			if (_state == 1)
			{
				for (IEnumerable<TSource> enumerable = GetEnumerable(0); enumerable != null; enumerable = GetEnumerable(_state - 1))
				{
					IEnumerator<TSource> enumerator = enumerable.GetEnumerator();
					SetEnumerator(enumerator);
					_state++;
					if (enumerator.MoveNext())
					{
						StoreFirst();
						return true;
					}
				}
			}
			else if (_state > 0)
			{
				while (true)
				{
					if (GetNext())
					{
						return true;
					}
					IEnumerable<TSource> enumerable2 = GetEnumerable(_state - 1);
					if (enumerable2 == null)
					{
						break;
					}
					SetEnumerator(enumerable2.GetEnumerator());
					_state++;
				}
			}
			Dispose();
			return false;
		}

		private HashSet<TSource> FillSet()
		{
			HashSet<TSource> hashSet = new HashSet<TSource>(_comparer);
			int num = 0;
			while (true)
			{
				IEnumerable<TSource> enumerable = GetEnumerable(num);
				if (enumerable == null)
				{
					break;
				}
				hashSet.UnionWith(enumerable);
				num++;
			}
			return hashSet;
		}

		public override TSource[] ToArray()
		{
			return ICollectionToArray(FillSet());
		}

		public override List<TSource> ToList()
		{
			return new List<TSource>(FillSet());
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return FillSet().Count;
			}
			return -1;
		}

		public override TSource TryGetFirst(out bool found)
		{
			int num = 0;
			IEnumerable<TSource> enumerable;
			while ((enumerable = GetEnumerable(num)) != null)
			{
				TSource result = enumerable.TryGetFirst(out found);
				if (found)
				{
					return result;
				}
				num++;
			}
			found = false;
			return default(TSource);
		}

		public override bool Contains(TSource value)
		{
			if (_comparer == null)
			{
				int num = 0;
				IEnumerable<TSource> enumerable;
				while ((enumerable = GetEnumerable(num)) != null)
				{
					if (enumerable.Contains(value))
					{
						return true;
					}
					num++;
				}
				return false;
			}
			return base.Contains(value);
		}
	}

	private sealed class UnionIterator2<TSource> : UnionIterator<TSource>
	{
		private readonly IEnumerable<TSource> _first;

		private readonly IEnumerable<TSource> _second;

		public UnionIterator2(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
			: base(comparer)
		{
			_first = first;
			_second = second;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new UnionIterator2<TSource>(_first, _second, _comparer);
		}

		internal override IEnumerable<TSource> GetEnumerable(int index)
		{
			return index switch
			{
				0 => _first, 
				1 => _second, 
				_ => null, 
			};
		}

		internal override UnionIterator<TSource> Union(IEnumerable<TSource> next)
		{
			return new UnionIteratorN<TSource>(new SingleLinkedNode<IEnumerable<TSource>>(_first).Add(_second).Add(next), 2, _comparer);
		}
	}

	private sealed class UnionIteratorN<TSource> : UnionIterator<TSource>
	{
		private readonly SingleLinkedNode<IEnumerable<TSource>> _sources;

		private readonly int _headIndex;

		public UnionIteratorN(SingleLinkedNode<IEnumerable<TSource>> sources, int headIndex, IEqualityComparer<TSource> comparer)
			: base(comparer)
		{
			_sources = sources;
			_headIndex = headIndex;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new UnionIteratorN<TSource>(_sources, _headIndex, _comparer);
		}

		internal override IEnumerable<TSource> GetEnumerable(int index)
		{
			if (index <= _headIndex)
			{
				return _sources.GetNode(_headIndex - index).Item;
			}
			return null;
		}

		internal override UnionIterator<TSource> Union(IEnumerable<TSource> next)
		{
			if (_headIndex == 2147483645)
			{
				return new UnionIterator2<TSource>(this, next, _comparer);
			}
			return new UnionIteratorN<TSource>(_sources.Add(next), _headIndex + 1, _comparer);
		}
	}

	private sealed class IEnumerableWhereIterator<TSource> : Iterator<TSource>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, bool> _predicate;

		private IEnumerator<TSource> _enumerator;

		public IEnumerableWhereIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			_source = source;
			_predicate = predicate;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new IEnumerableWhereIterator<TSource>(_source, _predicate);
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_0061;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			while (_enumerator.MoveNext())
			{
				TSource current = _enumerator.Current;
				if (_predicate(current))
				{
					_current = current;
					return true;
				}
			}
			Dispose();
			goto IL_0061;
			IL_0061:
			return false;
		}

		public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
		{
			return new IEnumerableWhereSelectIterator<TSource, TResult>(_source, _predicate, selector);
		}

		public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
		{
			return new IEnumerableWhereIterator<TSource>(_source, Utilities.CombinePredicates(_predicate, predicate));
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			foreach (TSource item in _source)
			{
				if (_predicate(item))
				{
					num = checked(num + 1);
				}
			}
			return num;
		}

		public override TSource[] ToArray()
		{
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			Func<TSource, bool> predicate = _predicate;
			foreach (TSource item in _source)
			{
				if (predicate(item))
				{
					segmentedArrayBuilder.Add(item);
				}
			}
			TSource[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TSource> ToList()
		{
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			Func<TSource, bool> predicate = _predicate;
			foreach (TSource item in _source)
			{
				if (predicate(item))
				{
					segmentedArrayBuilder.Add(item);
				}
			}
			List<TSource> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override TSource TryGetFirst(out bool found)
		{
			Func<TSource, bool> predicate = _predicate;
			foreach (TSource item in _source)
			{
				if (predicate(item))
				{
					found = true;
					return item;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetLast(out bool found)
		{
			using IEnumerator<TSource> enumerator = _source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				Func<TSource, bool> predicate = _predicate;
				TSource val = default(TSource);
				do
				{
					TSource current = enumerator.Current;
					if (!predicate(current))
					{
						continue;
					}
					val = current;
					found = true;
					while (enumerator.MoveNext())
					{
						current = enumerator.Current;
						if (predicate(current))
						{
							val = current;
						}
					}
					return val;
				}
				while (enumerator.MoveNext());
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				Func<TSource, bool> predicate = _predicate;
				foreach (TSource item in _source)
				{
					if (predicate(item))
					{
						if (index == 0)
						{
							found = true;
							return item;
						}
						index--;
					}
				}
			}
			found = false;
			return default(TSource);
		}

		public override bool Contains(TSource value)
		{
			Func<TSource, bool> predicate = _predicate;
			foreach (TSource item in _source)
			{
				if (predicate(item) && EqualityComparer<TSource>.Default.Equals(item, value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class ArrayWhereIterator<TSource> : Iterator<TSource>
	{
		private readonly TSource[] _source;

		private readonly Func<TSource, bool> _predicate;

		public ArrayWhereIterator(TSource[] source, Func<TSource, bool> predicate)
		{
			_source = source;
			_predicate = predicate;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new ArrayWhereIterator<TSource>(_source, _predicate);
		}

		public override bool MoveNext()
		{
			int num = _state - 1;
			TSource[] source = _source;
			while ((uint)num < (uint)source.Length)
			{
				TSource val = source[num];
				num = _state++;
				if (_predicate(val))
				{
					_current = val;
					return true;
				}
			}
			Dispose();
			return false;
		}

		public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
		{
			return new ArrayWhereSelectIterator<TSource, TResult>(_source, _predicate, selector);
		}

		public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
		{
			return new ArrayWhereIterator<TSource>(_source, Utilities.CombinePredicates(_predicate, predicate));
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return GetCount(onlyIfCheap, _source, _predicate);
		}

		public static int GetCount(bool onlyIfCheap, ReadOnlySpan<TSource> source, Func<TSource, bool> predicate)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg))
				{
					num = checked(num + 1);
				}
			}
			return num;
		}

		public override TSource[] ToArray()
		{
			return ToArray(_source, _predicate);
		}

		public static TSource[] ToArray(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate)
		{
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource val = readOnlySpan[i];
				if (predicate(val))
				{
					segmentedArrayBuilder.Add(val);
				}
			}
			TSource[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TSource> ToList()
		{
			return ToList(_source, _predicate);
		}

		public static List<TSource> ToList(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate)
		{
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource val = readOnlySpan[i];
				if (predicate(val))
				{
					segmentedArrayBuilder.Add(val);
				}
			}
			List<TSource> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override TSource TryGetFirst(out bool found)
		{
			Func<TSource, bool> predicate = _predicate;
			TSource[] source = _source;
			foreach (TSource val in source)
			{
				if (predicate(val))
				{
					found = true;
					return val;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetLast(out bool found)
		{
			TSource[] source = _source;
			Func<TSource, bool> predicate = _predicate;
			for (int num = source.Length - 1; num >= 0; num--)
			{
				if (predicate(source[num]))
				{
					found = true;
					return source[num];
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				Func<TSource, bool> predicate = _predicate;
				TSource[] source = _source;
				foreach (TSource val in source)
				{
					if (predicate(val))
					{
						if (index == 0)
						{
							found = true;
							return val;
						}
						index--;
					}
				}
			}
			found = false;
			return default(TSource);
		}

		public override bool Contains(TSource value)
		{
			Func<TSource, bool> predicate = _predicate;
			TSource[] source = _source;
			foreach (TSource val in source)
			{
				if (predicate(val) && EqualityComparer<TSource>.Default.Equals(val, value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class ListWhereIterator<TSource> : Iterator<TSource>
	{
		private readonly List<TSource> _source;

		private readonly Func<TSource, bool> _predicate;

		private List<TSource>.Enumerator _enumerator;

		public ListWhereIterator(List<TSource> source, Func<TSource, bool> predicate)
		{
			_source = source;
			_predicate = predicate;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new ListWhereIterator<TSource>(_source, _predicate);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_0061;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			while (_enumerator.MoveNext())
			{
				TSource current = _enumerator.Current;
				if (_predicate(current))
				{
					_current = current;
					return true;
				}
			}
			Dispose();
			goto IL_0061;
			IL_0061:
			return false;
		}

		public override IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> selector)
		{
			return new ListWhereSelectIterator<TSource, TResult>(_source, _predicate, selector);
		}

		public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
		{
			return new ListWhereIterator<TSource>(_source, Utilities.CombinePredicates(_predicate, predicate));
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return ArrayWhereIterator<TSource>.GetCount(onlyIfCheap, CollectionsMarshal.AsSpan(_source), _predicate);
		}

		public override TSource[] ToArray()
		{
			return ArrayWhereIterator<TSource>.ToArray(CollectionsMarshal.AsSpan(_source), _predicate);
		}

		public override List<TSource> ToList()
		{
			return ArrayWhereIterator<TSource>.ToList(CollectionsMarshal.AsSpan(_source), _predicate);
		}

		public override TSource TryGetFirst(out bool found)
		{
			Func<TSource, bool> predicate = _predicate;
			Span<TSource> span = CollectionsMarshal.AsSpan(_source);
			for (int i = 0; i < span.Length; i++)
			{
				TSource val = span[i];
				if (predicate(val))
				{
					found = true;
					return val;
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetLast(out bool found)
		{
			ReadOnlySpan<TSource> readOnlySpan = CollectionsMarshal.AsSpan(_source);
			Func<TSource, bool> predicate = _predicate;
			for (int num = readOnlySpan.Length - 1; num >= 0; num--)
			{
				if (predicate(readOnlySpan[num]))
				{
					found = true;
					return readOnlySpan[num];
				}
			}
			found = false;
			return default(TSource);
		}

		public override TSource TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				Func<TSource, bool> predicate = _predicate;
				Span<TSource> span = CollectionsMarshal.AsSpan(_source);
				for (int i = 0; i < span.Length; i++)
				{
					TSource val = span[i];
					if (predicate(val))
					{
						if (index == 0)
						{
							found = true;
							return val;
						}
						index--;
					}
				}
			}
			found = false;
			return default(TSource);
		}

		public override bool Contains(TSource value)
		{
			Func<TSource, bool> predicate = _predicate;
			foreach (TSource item in _source)
			{
				if (predicate(item) && EqualityComparer<TSource>.Default.Equals(item, value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class ArrayWhereSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly TSource[] _source;

		private readonly Func<TSource, bool> _predicate;

		private readonly Func<TSource, TResult> _selector;

		public ArrayWhereSelectIterator(TSource[] source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			_source = source;
			_predicate = predicate;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new ArrayWhereSelectIterator<TSource, TResult>(_source, _predicate, _selector);
		}

		public override bool MoveNext()
		{
			int num = _state - 1;
			TSource[] source = _source;
			while ((uint)num < (uint)source.Length)
			{
				TSource arg = source[num];
				num = _state++;
				if (_predicate(arg))
				{
					_current = _selector(arg);
					return true;
				}
			}
			Dispose();
			return false;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new ArrayWhereSelectIterator<TSource, TResult2>(_source, _predicate, Utilities.CombineSelectors(_selector, selector));
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return GetCount(onlyIfCheap, _source, _predicate, _selector);
		}

		public static int GetCount(bool onlyIfCheap, ReadOnlySpan<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg))
				{
					selector(arg);
					num = checked(num + 1);
				}
			}
			return num;
		}

		public override TResult[] ToArray()
		{
			return ToArray(_source, _predicate, _selector);
		}

		public static TResult[] ToArray(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg))
				{
					segmentedArrayBuilder.Add(selector(arg));
				}
			}
			TResult[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TResult> ToList()
		{
			return ToList(_source, _predicate, _selector);
		}

		public static List<TResult> ToList(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg))
				{
					segmentedArrayBuilder.Add(selector(arg));
				}
			}
			List<TResult> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override TResult TryGetFirst(out bool found)
		{
			return TryGetFirst(_source, _predicate, _selector, out found);
		}

		public static TResult TryGetFirst(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector, out bool found)
		{
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg))
				{
					found = true;
					return selector(arg);
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			return TryGetLast(_source, _predicate, _selector, out found);
		}

		public static TResult TryGetLast(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector, out bool found)
		{
			for (int num = source.Length - 1; num >= 0; num--)
			{
				if (predicate(source[num]))
				{
					found = true;
					return selector(source[num]);
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			return TryGetElementAt(_source, _predicate, _selector, index, out found);
		}

		public static TResult TryGetElementAt(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector, int index, out bool found)
		{
			if (index >= 0)
			{
				ReadOnlySpan<TSource> readOnlySpan = source;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					TSource arg = readOnlySpan[i];
					if (predicate(arg))
					{
						if (index == 0)
						{
							found = true;
							return selector(arg);
						}
						index--;
					}
				}
			}
			found = false;
			return default(TResult);
		}

		public override bool Contains(TResult value)
		{
			return Contains(_source, _predicate, _selector, value);
		}

		public static bool Contains(ReadOnlySpan<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector, TResult value)
		{
			ReadOnlySpan<TSource> readOnlySpan = source;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg) && EqualityComparer<TResult>.Default.Equals(selector(arg), value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class ListWhereSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly List<TSource> _source;

		private readonly Func<TSource, bool> _predicate;

		private readonly Func<TSource, TResult> _selector;

		private List<TSource>.Enumerator _enumerator;

		public ListWhereSelectIterator(List<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			_source = source;
			_predicate = predicate;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new ListWhereSelectIterator<TSource, TResult>(_source, _predicate, _selector);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_006c;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			while (_enumerator.MoveNext())
			{
				TSource current = _enumerator.Current;
				if (_predicate(current))
				{
					_current = _selector(current);
					return true;
				}
			}
			Dispose();
			goto IL_006c;
			IL_006c:
			return false;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new ListWhereSelectIterator<TSource, TResult2>(_source, _predicate, Utilities.CombineSelectors(_selector, selector));
		}

		public override int GetCount(bool onlyIfCheap)
		{
			return ArrayWhereSelectIterator<TSource, TResult>.GetCount(onlyIfCheap, CollectionsMarshal.AsSpan(_source), _predicate, _selector);
		}

		public override TResult[] ToArray()
		{
			return ArrayWhereSelectIterator<TSource, TResult>.ToArray(CollectionsMarshal.AsSpan(_source), _predicate, _selector);
		}

		public override List<TResult> ToList()
		{
			return ArrayWhereSelectIterator<TSource, TResult>.ToList(CollectionsMarshal.AsSpan(_source), _predicate, _selector);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			return ArrayWhereSelectIterator<TSource, TResult>.TryGetElementAt(CollectionsMarshal.AsSpan(_source), _predicate, _selector, index, out found);
		}

		public override TResult TryGetFirst(out bool found)
		{
			return ArrayWhereSelectIterator<TSource, TResult>.TryGetFirst(CollectionsMarshal.AsSpan(_source), _predicate, _selector, out found);
		}

		public override TResult TryGetLast(out bool found)
		{
			return ArrayWhereSelectIterator<TSource, TResult>.TryGetLast(CollectionsMarshal.AsSpan(_source), _predicate, _selector, out found);
		}

		public override bool Contains(TResult value)
		{
			return ArrayWhereSelectIterator<TSource, TResult>.Contains(CollectionsMarshal.AsSpan(_source), _predicate, _selector, value);
		}
	}

	private sealed class IEnumerableWhereSelectIterator<TSource, TResult> : Iterator<TResult>
	{
		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, bool> _predicate;

		private readonly Func<TSource, TResult> _selector;

		private IEnumerator<TSource> _enumerator;

		public IEnumerableWhereSelectIterator(IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			_source = source;
			_predicate = predicate;
			_selector = selector;
		}

		private protected override Iterator<TResult> Clone()
		{
			return new IEnumerableWhereSelectIterator<TSource, TResult>(_source, _predicate, _selector);
		}

		public override void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Dispose();
				_enumerator = null;
			}
			base.Dispose();
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_006c;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			while (_enumerator.MoveNext())
			{
				TSource current = _enumerator.Current;
				if (_predicate(current))
				{
					_current = _selector(current);
					return true;
				}
			}
			Dispose();
			goto IL_006c;
			IL_006c:
			return false;
		}

		public override IEnumerable<TResult2> Select<TResult2>(Func<TResult, TResult2> selector)
		{
			return new IEnumerableWhereSelectIterator<TSource, TResult2>(_source, _predicate, Utilities.CombineSelectors(_selector, selector));
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			foreach (TSource item in _source)
			{
				if (_predicate(item))
				{
					_selector(item);
					num = checked(num + 1);
				}
			}
			return num;
		}

		public override TResult[] ToArray()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, bool> predicate = _predicate;
			Func<TSource, TResult> selector = _selector;
			foreach (TSource item in _source)
			{
				if (predicate(item))
				{
					segmentedArrayBuilder.Add(selector(item));
				}
			}
			TResult[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TResult> ToList()
		{
			SegmentedArrayBuilder<TResult>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TResult>.ScratchBuffer);
			SegmentedArrayBuilder<TResult> segmentedArrayBuilder = new SegmentedArrayBuilder<TResult>(buffer);
			Func<TSource, bool> predicate = _predicate;
			Func<TSource, TResult> selector = _selector;
			foreach (TSource item in _source)
			{
				if (predicate(item))
				{
					segmentedArrayBuilder.Add(selector(item));
				}
			}
			List<TResult> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override TResult TryGetFirst(out bool found)
		{
			Func<TSource, bool> predicate = _predicate;
			foreach (TSource item in _source)
			{
				if (predicate(item))
				{
					found = true;
					return _selector(item);
				}
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetLast(out bool found)
		{
			using IEnumerator<TSource> enumerator = _source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				Func<TSource, bool> predicate = _predicate;
				TSource val = default(TSource);
				do
				{
					TSource current = enumerator.Current;
					if (!predicate(current))
					{
						continue;
					}
					val = current;
					found = true;
					while (enumerator.MoveNext())
					{
						current = enumerator.Current;
						if (predicate(current))
						{
							val = current;
						}
					}
					return _selector(val);
				}
				while (enumerator.MoveNext());
			}
			found = false;
			return default(TResult);
		}

		public override TResult TryGetElementAt(int index, out bool found)
		{
			if (index >= 0)
			{
				Func<TSource, bool> predicate = _predicate;
				foreach (TSource item in _source)
				{
					if (predicate(item))
					{
						if (index == 0)
						{
							found = true;
							return _selector(item);
						}
						index--;
					}
				}
			}
			found = false;
			return default(TResult);
		}

		public override bool Contains(TResult value)
		{
			Func<TSource, bool> predicate = _predicate;
			foreach (TSource item in _source)
			{
				if (predicate(item) && EqualityComparer<TResult>.Default.Equals(_selector(item), value))
				{
					return true;
				}
			}
			return false;
		}
	}

	private sealed class SizeOptIListWhereIterator<TSource> : Iterator<TSource>
	{
		private readonly IList<TSource> _source;

		private readonly Func<TSource, bool> _predicate;

		private IEnumerator<TSource> _enumerator;

		public SizeOptIListWhereIterator(IList<TSource> source, Func<TSource, bool> predicate)
		{
			_source = source;
			_predicate = predicate;
		}

		private protected override Iterator<TSource> Clone()
		{
			return new SizeOptIListWhereIterator<TSource>(_source, _predicate);
		}

		public override bool MoveNext()
		{
			int state = _state;
			if (state != 1)
			{
				if (state != 2)
				{
					goto IL_0061;
				}
			}
			else
			{
				_enumerator = _source.GetEnumerator();
				_state = 2;
			}
			while (_enumerator.MoveNext())
			{
				TSource current = _enumerator.Current;
				if (_predicate(current))
				{
					_current = current;
					return true;
				}
			}
			Dispose();
			goto IL_0061;
			IL_0061:
			return false;
		}

		public override IEnumerable<TSource> Where(Func<TSource, bool> predicate)
		{
			return new SizeOptIListWhereIterator<TSource>(_source, Utilities.CombinePredicates(_predicate, predicate));
		}

		public override TSource[] ToArray()
		{
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			foreach (TSource item in _source)
			{
				if (_predicate(item))
				{
					segmentedArrayBuilder.Add(item);
				}
			}
			TSource[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override List<TSource> ToList()
		{
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			foreach (TSource item in _source)
			{
				if (_predicate(item))
				{
					segmentedArrayBuilder.Add(item);
				}
			}
			List<TSource> result = segmentedArrayBuilder.ToList();
			segmentedArrayBuilder.Dispose();
			return result;
		}

		public override int GetCount(bool onlyIfCheap)
		{
			if (onlyIfCheap)
			{
				return -1;
			}
			int num = 0;
			foreach (TSource item in _source)
			{
				if (_predicate(item))
				{
					num = checked(num + 1);
				}
			}
			return num;
		}
	}

	[FeatureSwitchDefinition("System.Linq.Enumerable.IsSizeOptimized")]
	internal static bool IsSizeOptimized { get; } = AppContext.TryGetSwitch("System.Linq.Enumerable.IsSizeOptimized", out var isEnabled) && isEnabled;

	/// <summary>Applies an accumulator function over a sequence.</summary>
	/// <returns>The final accumulator value.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
	/// <param name="func">An accumulator function to be invoked on each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="func" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (func == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);
		}
		TSource val;
		if (source.TryGetSpan<TSource>(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			val = span[0];
			for (int i = 1; i < span.Length; i++)
			{
				val = func(val, span[i]);
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				ThrowHelper.ThrowNoElementsException();
			}
			val = enumerator.Current;
			while (enumerator.MoveNext())
			{
				val = func(val, enumerator.Current);
			}
		}
		return val;
	}

	/// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value.</summary>
	/// <returns>The final accumulator value.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
	/// <param name="seed">The initial accumulator value.</param>
	/// <param name="func">An accumulator function to be invoked on each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="func" /> is null.</exception>
	public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (func == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);
		}
		TAccumulate val = seed;
		if (source.TryGetSpan<TSource>(out var span))
		{
			ReadOnlySpan<TSource> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				val = func(val, arg);
			}
		}
		else
		{
			foreach (TSource item in source)
			{
				val = func(val, item);
			}
		}
		return val;
	}

	/// <summary>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.</summary>
	/// <returns>The transformed final accumulator value.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
	/// <param name="seed">The initial accumulator value.</param>
	/// <param name="func">An accumulator function to be invoked on each element.</param>
	/// <param name="resultSelector">A function to transform the final accumulator value into the result value.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
	/// <typeparam name="TResult">The type of the resulting value.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="func" /> or <paramref name="resultSelector" /> is null.</exception>
	public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (func == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		TAccumulate val = seed;
		if (source.TryGetSpan<TSource>(out var span))
		{
			ReadOnlySpan<TSource> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				val = func(val, arg);
			}
		}
		else
		{
			foreach (TSource item in source)
			{
				val = func(val, item);
			}
		}
		return resultSelector(val);
	}

	/// <summary>Determines whether a sequence contains any elements.</summary>
	/// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to check for emptiness.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static bool Any<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source is ICollection<TSource> collection)
		{
			return collection.Count != 0;
		}
		if (!IsSizeOptimized && source is Iterator<TSource> iterator)
		{
			int count = iterator.GetCount(onlyIfCheap: true);
			if (count >= 0)
			{
				return count != 0;
			}
			iterator.TryGetFirst(out var found);
			return found;
		}
		if (source is ICollection collection2)
		{
			return collection2.Count != 0;
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		return enumerator.MoveNext();
	}

	/// <summary>Determines whether any element of a sequence satisfies a condition.</summary>
	/// <returns>true if any elements in the source sequence pass the test in the specified predicate; otherwise, false.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to apply the predicate to.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (source.TryGetSpan<TSource>(out var span))
		{
			ReadOnlySpan<TSource> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg))
				{
					return true;
				}
			}
		}
		else
		{
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>Determines whether all elements of a sequence satisfy a condition.</summary>
	/// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements to apply the predicate to.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (source.TryGetSpan<TSource>(out var span))
		{
			ReadOnlySpan<TSource> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (!predicate(arg))
				{
					return false;
				}
			}
		}
		else
		{
			foreach (TSource item in source)
			{
				if (!predicate(item))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (!(source is AppendPrependIterator<TSource> appendPrependIterator))
		{
			return new AppendPrepend1Iterator<TSource>(source, element, appending: true);
		}
		return appendPrependIterator.Append(element);
	}

	public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (!(source is AppendPrependIterator<TSource> appendPrependIterator))
		{
			return new AppendPrepend1Iterator<TSource>(source, element, appending: false);
		}
		return appendPrependIterator.Prepend(element);
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Int32" /> values.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Average(this IEnumerable<int> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			long num = 0L;
			int i = 0;
			if (Vector.IsHardwareAccelerated && span.Length >= Vector<int>.Count)
			{
				Vector<long> value = default(Vector<long>);
				do
				{
					Vector.Widen(new Vector<int>(span.Slice(i)), out var low, out var high);
					value += low;
					value += high;
					i += Vector<int>.Count;
				}
				while (i <= span.Length - Vector<int>.Count);
				num += Vector.Sum(value);
			}
			for (; (uint)i < (uint)span.Length; i++)
			{
				num += span[i];
			}
			return (double)num / (double)span.Length;
		}
		using IEnumerator<int> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		long num2 = enumerator.Current;
		long num3 = 1L;
		while (enumerator.MoveNext())
		{
			num2 = checked(num2 + enumerator.Current);
			num3++;
		}
		return (double)num2 / (double)num3;
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Int64" /> values.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Average(this IEnumerable<long> source)
	{
		return source.Average<long, long, double>();
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Single" /> values.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Single" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static float Average(this IEnumerable<float> source)
	{
		return (float)source.Average<float, double, double>();
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Double" /> values.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Double" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Average(this IEnumerable<double> source)
	{
		return source.Average<double, double, double>();
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static decimal Average(this IEnumerable<decimal> source)
	{
		return source.Average<decimal, decimal, decimal>();
	}

	private static TResult Average<TSource, TAccumulator, TResult>(this IEnumerable<TSource> source) where TSource : struct, INumber<TSource> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			return TResult.CreateChecked(Sum<TSource, TAccumulator>(span)) / TResult.CreateChecked(span.Length);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		TAccumulator val = TAccumulator.CreateChecked(enumerator.Current);
		long num = 1L;
		while (enumerator.MoveNext())
		{
			val = checked(val + TAccumulator.CreateChecked(enumerator.Current));
			num++;
		}
		return TResult.CreateChecked(val) / TResult.CreateChecked(num);
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static double? Average(this IEnumerable<int?> source)
	{
		return source.Average<int, long, double>();
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static double? Average(this IEnumerable<long?> source)
	{
		return source.Average<long, long, double>();
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Single" /> values.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static float? Average(this IEnumerable<float?> source)
	{
		double? num = source.Average<float, double, double>();
		if (num.HasValue)
		{
			double valueOrDefault = num.GetValueOrDefault();
			return (float)valueOrDefault;
		}
		return null;
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Double" /> values.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static double? Average(this IEnumerable<double?> source)
	{
		return source.Average<double, double, double>();
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the average of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
	public static decimal? Average(this IEnumerable<decimal?> source)
	{
		return source.Average<decimal, decimal, decimal>();
	}

	private static TResult? Average<TSource, TAccumulator, TResult>(this IEnumerable<TSource?> source) where TSource : struct, INumber<TSource> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		using IEnumerator<TSource?> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TSource? current = enumerator.Current;
			if (!current.HasValue)
			{
				continue;
			}
			TAccumulator val = TAccumulator.CreateChecked(current.GetValueOrDefault());
			long num = 1L;
			while (enumerator.MoveNext())
			{
				current = enumerator.Current;
				if (current.HasValue)
				{
					val = checked(val + TAccumulator.CreateChecked(current.GetValueOrDefault()));
					num++;
				}
			}
			return TResult.CreateChecked(val) / TResult.CreateChecked(num);
		}
		return null;
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
	{
		return source.Average<TSource, int, long, double>(selector);
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
	{
		return source.Average<TSource, long, long, double>(selector);
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
	{
		return (float)source.Average<TSource, float, double, double>(selector);
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
	{
		return source.Average<TSource, double, double, double>(selector);
	}

	/// <summary>Computes the average of a sequence of <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values.</returns>
	/// <param name="source">A sequence of values that are used to calculate an average.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
	public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
	{
		return source.Average<TSource, decimal, decimal, decimal>(selector);
	}

	private static TResult Average<TSource, TSelector, TAccumulator, TResult>(this IEnumerable<TSource> source, Func<TSource, TSelector> selector) where TSelector : struct, INumber<TSelector> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		TAccumulator val = TAccumulator.CreateChecked(selector(enumerator.Current));
		long num = 1L;
		while (enumerator.MoveNext())
		{
			val = checked(val + TAccumulator.CreateChecked(selector(enumerator.Current)));
			num++;
		}
		return TResult.CreateChecked(val) / TResult.CreateChecked(num);
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
	{
		return source.Average<TSource, int, long, double>(selector);
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
	{
		return source.Average<TSource, long, long, double>(selector);
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
	{
		double? num = source.Average<TSource, float, double, double>(selector);
		if (num.HasValue)
		{
			double valueOrDefault = num.GetValueOrDefault();
			return (float)valueOrDefault;
		}
		return null;
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
	{
		return source.Average<TSource, double, double, double>(selector);
	}

	/// <summary>Computes the average of a sequence of nullable <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The average of the sequence of values, or null if the source sequence is empty or contains only values that are null.</returns>
	/// <param name="source">A sequence of values to calculate the average of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum of the elements in the sequence is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
	public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
	{
		return source.Average<TSource, decimal, decimal, decimal>(selector);
	}

	private static TResult? Average<TSource, TSelector, TAccumulator, TResult>(this IEnumerable<TSource> source, Func<TSource, TSelector?> selector) where TSelector : struct, INumber<TSelector> where TAccumulator : struct, INumber<TAccumulator> where TResult : struct, INumber<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TSelector? val = selector(enumerator.Current);
			if (!val.HasValue)
			{
				continue;
			}
			TAccumulator val2 = TAccumulator.CreateChecked(val.GetValueOrDefault());
			long num = 1L;
			while (enumerator.MoveNext())
			{
				val = selector(enumerator.Current);
				if (val.HasValue)
				{
					val2 = checked(val2 + TAccumulator.CreateChecked(val.GetValueOrDefault()));
					num++;
				}
			}
			return TResult.CreateChecked(val2) / TResult.CreateChecked(num);
		}
		return null;
	}

	/// <summary>Casts the elements of an <see cref="T:System.Collections.IEnumerable" /> to the specified type.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains each element of the source sequence cast to the specified type.</returns>
	/// <param name="source">The <see cref="T:System.Collections.IEnumerable" /> that contains the elements to be cast to type <paramref name="TResult" />.</param>
	/// <typeparam name="TResult">The type to cast the elements of <paramref name="source" /> to.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidCastException">An element in the sequence cannot be cast to type <paramref name="TResult" />.</exception>
	public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
	{
		if (source is IEnumerable<TResult> result)
		{
			return result;
		}
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source is ICollection source2)
		{
			return new CastICollectionIterator<TResult>(source2);
		}
		return CastIterator<TResult>(source);
	}

	private static IEnumerable<TResult> CastIterator<TResult>(IEnumerable source)
	{
		foreach (object item in source)
		{
			yield return (TResult)item;
		}
	}

	public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (size < 1)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.size);
		}
		if (source is TSource[] array)
		{
			if (array.Length == 0)
			{
				return Array.Empty<TSource[]>();
			}
			return ArrayChunkIterator(array, size);
		}
		return EnumerableChunkIterator(source, size);
	}

	private static IEnumerable<TSource[]> ArrayChunkIterator<TSource>(TSource[] source, int size)
	{
		int index = 0;
		while (index < source.Length)
		{
			TSource[] array = new ReadOnlySpan<TSource>(source, index, Math.Min(size, source.Length - index)).ToArray();
			index += array.Length;
			yield return array;
		}
	}

	private static IEnumerable<TSource[]> EnumerableChunkIterator<TSource>(IEnumerable<TSource> source, int size)
	{
		using IEnumerator<TSource> e = source.GetEnumerator();
		if (!e.MoveNext())
		{
			yield break;
		}
		int arraySize = Math.Min(size, 4);
		int i;
		do
		{
			TSource[] array = new TSource[arraySize];
			array[0] = e.Current;
			i = 1;
			if (size != array.Length)
			{
				for (; i < size; i++)
				{
					if (!e.MoveNext())
					{
						break;
					}
					if (i >= array.Length)
					{
						arraySize = (int)Math.Min((uint)size, (uint)(2 * array.Length));
						Array.Resize(ref array, arraySize);
					}
					array[i] = e.Current;
				}
			}
			else
			{
				for (TSource[] array2 = array; (uint)i < (uint)array2.Length; i++)
				{
					if (!e.MoveNext())
					{
						break;
					}
					array2[i] = e.Current;
				}
			}
			if (i != array.Length)
			{
				Array.Resize(ref array, i);
			}
			yield return array;
		}
		while (i >= size && e.MoveNext());
	}

	/// <summary>Concatenates two sequences.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the concatenated elements of the two input sequences.</returns>
	/// <param name="first">The first sequence to concatenate.</param>
	/// <param name="second">The sequence to concatenate to the first sequence.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (IsEmptyArray(first))
		{
			return second;
		}
		if (IsEmptyArray(second))
		{
			return first;
		}
		if (!(first is ConcatIterator<TSource> concatIterator))
		{
			return new Concat2Iterator<TSource>(first, second);
		}
		return concatIterator.Concat(second);
	}

	/// <summary>Determines whether a sequence contains a specified element by using the default equality comparer.</summary>
	/// <returns>true if the source sequence contains an element that has the specified value; otherwise, false.</returns>
	/// <param name="source">A sequence in which to locate a value.</param>
	/// <param name="value">The value to locate in the sequence.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
	{
		if (source is ICollection<TSource> collection)
		{
			return collection.Contains(value);
		}
		if (!IsSizeOptimized && source is Iterator<TSource> iterator)
		{
			return iterator.Contains(value);
		}
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		return ContainsIterate(source, value, null);
	}

	/// <summary>Determines whether a sequence contains a specified element by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
	/// <returns>true if the source sequence contains an element that has the specified value; otherwise, false.</returns>
	/// <param name="source">A sequence in which to locate a value.</param>
	/// <param name="value">The value to locate in the sequence.</param>
	/// <param name="comparer">An equality comparer to compare values.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan<TSource>(out var span))
		{
			return span.Contains(value, comparer);
		}
		return ContainsIterate(source, value, comparer);
	}

	private static bool ContainsIterate<TSource>(IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
	{
		if (comparer == null)
		{
			if (typeof(TSource).IsValueType)
			{
				foreach (TSource item in source)
				{
					if (EqualityComparer<TSource>.Default.Equals(item, value))
					{
						return true;
					}
				}
				return false;
			}
			comparer = EqualityComparer<TSource>.Default;
		}
		foreach (TSource item2 in source)
		{
			if (comparer.Equals(item2, value))
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (func == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<KeyValuePair<TKey, TAccumulate>>();
		}
		return AggregateByIterator(source, keySelector, seed, func, keyComparer);
	}

	public static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateBy<TSource, TKey, TAccumulate>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (seedSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.seedSelector);
		}
		if (func == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<KeyValuePair<TKey, TAccumulate>>();
		}
		return AggregateByIterator(source, keySelector, seedSelector, func, keyComparer);
	}

	private static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateByIterator<TSource, TKey, TAccumulate>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey> keyComparer)
	{
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				yield break;
			}
			foreach (KeyValuePair<TKey, TAccumulate> item in PopulateDictionary(enumerator, keySelector, seed, func, keyComparer))
			{
				yield return item;
			}
		}
		static Dictionary<TKey, TAccumulate> PopulateDictionary(IEnumerator<TSource> enumerator3, Func<TSource, TKey> func2, TAccumulate val, Func<TAccumulate, TSource, TAccumulate> func3, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, TAccumulate> dictionary = new Dictionary<TKey, TAccumulate>(comparer);
			do
			{
				TSource current = enumerator3.Current;
				TKey key = func2(current);
				ref TAccumulate valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
				valueRefOrAddDefault = func3(exists ? valueRefOrAddDefault : val, current);
			}
			while (enumerator3.MoveNext());
			return dictionary;
		}
	}

	private static IEnumerable<KeyValuePair<TKey, TAccumulate>> AggregateByIterator<TSource, TKey, TAccumulate>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TAccumulate> seedSelector, Func<TAccumulate, TSource, TAccumulate> func, IEqualityComparer<TKey> keyComparer)
	{
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
			{
				yield break;
			}
			foreach (KeyValuePair<TKey, TAccumulate> item in PopulateDictionary(enumerator, keySelector, seedSelector, func, keyComparer))
			{
				yield return item;
			}
		}
		static Dictionary<TKey, TAccumulate> PopulateDictionary(IEnumerator<TSource> enumerator3, Func<TSource, TKey> func2, Func<TKey, TAccumulate> func4, Func<TAccumulate, TSource, TAccumulate> func3, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, TAccumulate> dictionary = new Dictionary<TKey, TAccumulate>(comparer);
			do
			{
				TSource current = enumerator3.Current;
				TKey val = func2(current);
				ref TAccumulate valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, val, out var exists);
				valueRefOrAddDefault = func3(exists ? valueRefOrAddDefault : func4(val), current);
			}
			while (enumerator3.MoveNext());
			return dictionary;
		}
	}

	public static IEnumerable<KeyValuePair<TKey, int>> CountBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<KeyValuePair<TKey, int>>();
		}
		return CountByIterator(source, keySelector, keyComparer);
	}

	private static IEnumerable<KeyValuePair<TKey, int>> CountByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
	{
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			yield break;
		}
		foreach (KeyValuePair<TKey, int> item in BuildCountDictionary(enumerator, keySelector, keyComparer))
		{
			yield return item;
		}
	}

	private static Dictionary<TKey, int> BuildCountDictionary<TSource, TKey>(IEnumerator<TSource> enumerator, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> keyComparer)
	{
		Dictionary<TKey, int> dictionary = new Dictionary<TKey, int>(keyComparer);
		checked
		{
			do
			{
				TSource current = enumerator.Current;
				CollectionsMarshal.GetValueRefOrAddDefault(dictionary, keySelector(current), out var _)++;
			}
			while (enumerator.MoveNext());
			return dictionary;
		}
	}

	/// <summary>Returns the number of elements in a sequence.</summary>
	/// <returns>The number of elements in the input sequence.</returns>
	/// <param name="source">A sequence that contains elements to be counted.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static int Count<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source is ICollection<TSource> collection)
		{
			return collection.Count;
		}
		if (source is Iterator<TSource> iterator)
		{
			return iterator.GetCount(onlyIfCheap: false);
		}
		if (source is ICollection collection2)
		{
			return collection2.Count;
		}
		int num = 0;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num = checked(num + 1);
		}
		return num;
	}

	/// <summary>Returns a number that represents how many elements in the specified sequence satisfy a condition.</summary>
	/// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
	/// <param name="source">A sequence that contains elements to be tested and counted.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		int num = 0;
		if (source.TryGetSpan<TSource>(out var span))
		{
			ReadOnlySpan<TSource> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource arg = readOnlySpan[i];
				if (predicate(arg))
				{
					num++;
				}
			}
		}
		else
		{
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					num = checked(num + 1);
				}
			}
		}
		return num;
	}

	public static bool TryGetNonEnumeratedCount<TSource>(this IEnumerable<TSource> source, out int count)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source is ICollection<TSource> collection)
		{
			count = collection.Count;
			return true;
		}
		if (source is Iterator<TSource> iterator)
		{
			int count2 = iterator.GetCount(onlyIfCheap: true);
			if (count2 >= 0)
			{
				count = count2;
				return true;
			}
		}
		if (source is ICollection collection2)
		{
			count = collection2.Count;
			return true;
		}
		count = 0;
		return false;
	}

	/// <summary>Returns an <see cref="T:System.Int64" /> that represents the total number of elements in a sequence.</summary>
	/// <returns>The number of elements in the source sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements to be counted.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The number of elements exceeds <see cref="F:System.Int64.MaxValue" />.</exception>
	public static long LongCount<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		long num = 0L;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num = checked(num + 1);
		}
		return num;
	}

	/// <summary>Returns an <see cref="T:System.Int64" /> that represents how many elements in a sequence satisfy a condition.</summary>
	/// <returns>A number that represents how many elements in the sequence satisfy the condition in the predicate function.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements to be counted.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The number of matching elements exceeds <see cref="F:System.Int64.MaxValue" />.</exception>
	public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		long num = 0L;
		foreach (TSource item in source)
		{
			if (predicate(item))
			{
				num = checked(num + 1);
			}
		}
		return num;
	}

	/// <summary>Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> object that contains the default value for the <paramref name="TSource" /> type if <paramref name="source" /> is empty; otherwise, <paramref name="source" />.</returns>
	/// <param name="source">The sequence to return a default value for if it is empty.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static IEnumerable<TSource?> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
	{
		return source.DefaultIfEmpty(default(TSource));
	}

	/// <summary>Returns the elements of the specified sequence or the specified value in a singleton collection if the sequence is empty.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains <paramref name="defaultValue" /> if <paramref name="source" /> is empty; otherwise, <paramref name="source" />.</returns>
	/// <param name="source">The sequence to return the specified value for if it is empty.</param>
	/// <param name="defaultValue">The value to return if the sequence is empty.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source is TSource[] array && array.Length > 0)
		{
			return source;
		}
		return new DefaultIfEmptyIterator<TSource>(source, defaultValue);
	}

	/// <summary>Returns distinct elements from a sequence by using the default equality comparer to compare values.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains distinct elements from the source sequence.</returns>
	/// <param name="source">The sequence to remove duplicate elements from.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
	{
		return source.Distinct(null);
	}

	/// <summary>Returns distinct elements from a sequence by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains distinct elements from the source sequence.</returns>
	/// <param name="source">The sequence to remove duplicate elements from.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return new DistinctIterator<TSource>(source, comparer);
	}

	public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return source.DistinctBy(keySelector, null);
	}

	public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return DistinctByIterator(source, keySelector, comparer);
	}

	private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
	{
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			yield break;
		}
		HashSet<TKey> set = new HashSet<TKey>(7, comparer);
		do
		{
			TSource current = enumerator.Current;
			if (set.Add(keySelector(current)))
			{
				yield return current;
			}
		}
		while (enumerator.MoveNext());
	}

	/// <summary>Returns the element at a specified index in a sequence.</summary>
	/// <returns>The element at the specified position in the source sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
	/// <param name="index">The zero-based index of the element to retrieve.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="index" /> is less than 0 or greater than or equal to the number of elements in <paramref name="source" />.</exception>
	public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source is IList<TSource> list)
		{
			return list[index];
		}
		object result = ((source is Iterator<TSource> iterator) ? ((object)iterator.TryGetElementAt(index, out var found)) : ((object)TryGetElementAtNonIterator(source, index, out found)));
		if (!found)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return (TSource)result;
	}

	public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, Index index)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (!index.IsFromEnd)
		{
			return source.ElementAt(index.Value);
		}
		if (source.TryGetNonEnumeratedCount(out var count))
		{
			return source.ElementAt(count - index.Value);
		}
		if (!TryGetElementFromEnd<TSource>(source, index.Value, out var element))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return element;
	}

	/// <summary>Returns the element at a specified index in a sequence or a default value if the index is out of range.</summary>
	/// <returns>default(<paramref name="TSource" />) if the index is outside the bounds of the source sequence; otherwise, the element at the specified position in the source sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
	/// <param name="index">The zero-based index of the element to retrieve.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static TSource? ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		bool found;
		return source.TryGetElementAt(index, out found);
	}

	public static TSource? ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, Index index)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (!index.IsFromEnd)
		{
			return source.ElementAtOrDefault(index.Value);
		}
		if (source.TryGetNonEnumeratedCount(out var count))
		{
			return source.ElementAtOrDefault(count - index.Value);
		}
		TryGetElementFromEnd<TSource>(source, index.Value, out var element);
		return element;
	}

	private static TSource TryGetElementAt<TSource>(this IEnumerable<TSource> source, int index, out bool found)
	{
		if (source is IList<TSource> list)
		{
			if (!(found = (uint)index < (uint)list.Count))
			{
				return default(TSource);
			}
			return list[index];
		}
		if (!(source is Iterator<TSource> iterator))
		{
			return TryGetElementAtNonIterator(source, index, out found);
		}
		return iterator.TryGetElementAt(index, out found);
	}

	private static TSource TryGetElementAtNonIterator<TSource>(IEnumerable<TSource> source, int index, out bool found)
	{
		if (index >= 0)
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (index == 0)
				{
					found = true;
					return enumerator.Current;
				}
				index--;
			}
		}
		found = false;
		return default(TSource);
	}

	private static bool TryGetElementFromEnd<TSource>(IEnumerable<TSource> source, int indexFromEnd, [MaybeNullWhen(false)] out TSource element)
	{
		if (indexFromEnd > 0)
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				Queue<TSource> queue = new Queue<TSource>();
				queue.Enqueue(enumerator.Current);
				while (enumerator.MoveNext())
				{
					if (queue.Count == indexFromEnd)
					{
						queue.Dequeue();
					}
					queue.Enqueue(enumerator.Current);
				}
				if (queue.Count == indexFromEnd)
				{
					element = queue.Dequeue();
					return true;
				}
			}
		}
		element = default(TSource);
		return false;
	}

	/// <summary>Returns the input typed as <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
	/// <returns>The input sequence typed as <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
	/// <param name="source">The sequence to type as <see cref="T:System.Collections.Generic.IEnumerable`1" />.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
	{
		return source;
	}

	/// <summary>Returns an empty <see cref="T:System.Collections.Generic.IEnumerable`1" /> that has the specified type argument.</summary>
	/// <returns>An empty <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose type argument is <paramref name="TResult" />.</returns>
	/// <typeparam name="TResult">The type to assign to the type parameter of the returned generic <see cref="T:System.Collections.Generic.IEnumerable`1" />.</typeparam>
	public static IEnumerable<TResult> Empty<TResult>()
	{
		return Array.Empty<TResult>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsEmptyArray<TSource>(IEnumerable<TSource> source)
	{
		if (source is TSource[] array)
		{
			return array.Length == 0;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Span<T> SetCountAndGetSpan<T>(List<T> list, int count)
	{
		CollectionsMarshal.SetCount(list, count);
		return CollectionsMarshal.AsSpan(list);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetSpan<TSource>(this IEnumerable<TSource> source, out ReadOnlySpan<TSource> span)
	{
		bool result = true;
		if (source.GetType() == typeof(TSource[]))
		{
			span = Unsafe.As<TSource[]>(source);
		}
		else if (source.GetType() == typeof(List<TSource>))
		{
			span = CollectionsMarshal.AsSpan(Unsafe.As<List<TSource>>(source));
		}
		else
		{
			span = default(ReadOnlySpan<TSource>);
			result = false;
		}
		return result;
	}

	/// <summary>Produces the set difference of two sequences by using the default equality comparer to compare values.</summary>
	/// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that are not also in <paramref name="second" /> will be returned.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		return ExceptIterator(first, second, null);
	}

	/// <summary>Produces the set difference of two sequences by using the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</summary>
	/// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that are not also in <paramref name="second" /> will be returned.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		return ExceptIterator(first, second, comparer);
	}

	public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
	{
		return first.ExceptBy(second, keySelector, null);
	}

	public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		return ExceptByIterator(first, second, keySelector, comparer);
	}

	private static IEnumerable<TSource> ExceptIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
	{
		HashSet<TSource> set = new HashSet<TSource>(second, comparer);
		foreach (TSource item in first)
		{
			if (set.Add(item))
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<TSource> ExceptByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
	{
		HashSet<TKey> set = new HashSet<TKey>(second, comparer);
		foreach (TSource item in first)
		{
			if (set.Add(keySelector(item)))
			{
				yield return item;
			}
		}
	}

	/// <summary>Returns the first element of a sequence.</summary>
	/// <returns>The first element in the specified sequence.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the first element of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
	public static TSource First<TSource>(this IEnumerable<TSource> source)
	{
		TSource result = source.TryGetFirst(out var found);
		if (!found)
		{
			ThrowHelper.ThrowNoElementsException();
		}
		return result;
	}

	/// <summary>Returns the first element in a sequence that satisfies a specified condition.</summary>
	/// <returns>The first element in the sequence that passes the test in the specified predicate function.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
	public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		TSource result = source.TryGetFirst(predicate, out var found);
		if (!found)
		{
			ThrowHelper.ThrowNoMatchException();
		}
		return result;
	}

	/// <summary>Returns the first element of a sequence, or a default value if the sequence contains no elements.</summary>
	/// <returns>default(<paramref name="TSource" />) if <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the first element of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static TSource? FirstOrDefault<TSource>(this IEnumerable<TSource> source)
	{
		bool found;
		return source.TryGetFirst(out found);
	}

	public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
	{
		TSource result = source.TryGetFirst(out var found);
		if (!found)
		{
			return defaultValue;
		}
		return result;
	}

	/// <summary>Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.</summary>
	/// <returns>default(<paramref name="TSource" />) if <paramref name="source" /> is empty or if no element passes the test specified by <paramref name="predicate" />; otherwise, the first element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static TSource? FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		bool found;
		return source.TryGetFirst(predicate, out found);
	}

	public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
	{
		TSource result = source.TryGetFirst(predicate, out var found);
		if (!found)
		{
			return defaultValue;
		}
		return result;
	}

	private static TSource TryGetFirst<TSource>(this IEnumerable<TSource> source, out bool found)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (IsSizeOptimized || !(source is Iterator<TSource> iterator))
		{
			return TryGetFirstNonIterator(source, out found);
		}
		return iterator.TryGetFirst(out found);
	}

	private static TSource TryGetFirstNonIterator<TSource>(IEnumerable<TSource> source, out bool found)
	{
		if (source is IList<TSource> list)
		{
			if (list.Count > 0)
			{
				found = true;
				return list[0];
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				found = true;
				return enumerator.Current;
			}
		}
		found = false;
		return default(TSource);
	}

	private static TSource TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (source.TryGetSpan(out var span))
		{
			ReadOnlySpan<TSource> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				TSource val = readOnlySpan[i];
				if (predicate(val))
				{
					found = true;
					return val;
				}
			}
		}
		else
		{
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					found = true;
					return item;
				}
			}
		}
		found = false;
		return default(TSource);
	}

	/// <summary>Groups the elements of a sequence according to a specified key selector function.</summary>
	/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TSource&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TSource)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a sequence of objects and a key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return source.GroupBy(keySelector, null);
	}

	/// <summary>Groups the elements of a sequence according to a specified key selector function and compares the keys by using a specified comparer.</summary>
	/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TSource&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TSource)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a collection of objects and a key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<IGrouping<TKey, TSource>>();
		}
		return new GroupByIterator<TSource, TKey>(source, keySelector, comparer);
	}

	/// <summary>Groups the elements of a sequence according to a specified key selector function and projects the elements for each group by using a specified function.</summary>
	/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TElement&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TElement)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a collection of objects of type <paramref name="TElement" /> and a key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <param name="elementSelector">A function to map each source element to an element in the <see cref="T:System.Linq.IGrouping`2" />.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the elements in the <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is null.</exception>
	public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
	{
		return source.GroupBy(keySelector, elementSelector, null);
	}

	/// <summary>Groups the elements of a sequence according to a key selector function. The keys are compared by using a comparer and each group's elements are projected by using a specified function.</summary>
	/// <returns>An IEnumerable&lt;IGrouping&lt;TKey, TElement&gt;&gt; in C# or IEnumerable(Of IGrouping(Of TKey, TElement)) in Visual Basic where each <see cref="T:System.Linq.IGrouping`2" /> object contains a collection of objects of type <paramref name="TElement" /> and a key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2" />.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the elements in the <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is null.</exception>
	public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (elementSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.elementSelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<IGrouping<TKey, TElement>>();
		}
		return new GroupByIterator<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer);
	}

	/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key.</summary>
	/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <param name="resultSelector">A function to create a result value from each group.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
	public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
	{
		return source.GroupBy(keySelector, resultSelector, null);
	}

	/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The keys are compared by using a specified comparer.</summary>
	/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <param name="resultSelector">A function to create a result value from each group.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys with.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
	public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TResult>();
		}
		return new GroupByResultIterator<TSource, TKey, TResult>(source, keySelector, resultSelector, comparer);
	}

	/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. The elements of each group are projected by using a specified function.</summary>
	/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2" />.</param>
	/// <param name="resultSelector">A function to create a result value from each group.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the elements in each <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
	/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
	public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
	{
		return source.GroupBy(keySelector, elementSelector, resultSelector, null);
	}

	/// <summary>Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. Key values are compared by using a specified comparer, and the elements of each group are projected by using a specified function.</summary>
	/// <returns>A collection of elements of type <paramref name="TResult" /> where each element represents a projection over a group and its key.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements to group.</param>
	/// <param name="keySelector">A function to extract the key for each element.</param>
	/// <param name="elementSelector">A function to map each source element to an element in an <see cref="T:System.Linq.IGrouping`2" />.</param>
	/// <param name="resultSelector">A function to create a result value from each group.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys with.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the elements in each <see cref="T:System.Linq.IGrouping`2" />.</typeparam>
	/// <typeparam name="TResult">The type of the result value returned by <paramref name="resultSelector" />.</typeparam>
	public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (elementSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.elementSelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TResult>();
		}
		return new GroupByResultIterator<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer);
	}

	/// <summary>Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements of type <paramref name="TResult" /> that are obtained by performing a grouped join on two sequences.</returns>
	/// <param name="outer">The first sequence to join.</param>
	/// <param name="inner">The sequence to join to the first sequence.</param>
	/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
	/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
	/// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
	/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
	/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
	/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
	/// <typeparam name="TResult">The type of the result elements.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is null.</exception>
	public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
	{
		return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);
	}

	/// <summary>Correlates the elements of two sequences based on key equality and groups the results. A specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> is used to compare keys.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements of type <paramref name="TResult" /> that are obtained by performing a grouped join on two sequences.</returns>
	/// <param name="outer">The first sequence to join.</param>
	/// <param name="inner">The sequence to join to the first sequence.</param>
	/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
	/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
	/// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to hash and compare keys.</param>
	/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
	/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
	/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
	/// <typeparam name="TResult">The type of the result elements.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is null.</exception>
	public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
	{
		if (outer == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outer);
		}
		if (inner == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.inner);
		}
		if (outerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outerKeySelector);
		}
		if (innerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.innerKeySelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(outer))
		{
			return Array.Empty<TResult>();
		}
		return GroupJoinIterator(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	}

	private static IEnumerable<TResult> GroupJoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
	{
		using IEnumerator<TOuter> e = outer.GetEnumerator();
		if (e.MoveNext())
		{
			Lookup<TKey, TInner> lookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
			do
			{
				TOuter current = e.Current;
				yield return resultSelector(current, lookup[outerKeySelector(current)]);
			}
			while (e.MoveNext());
		}
	}

	public static IEnumerable<(int Index, TSource Item)> Index<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<(int, TSource)>();
		}
		return IndexIterator(source);
	}

	private static IEnumerable<(int Index, TSource Item)> IndexIterator<TSource>(IEnumerable<TSource> source)
	{
		int index = -1;
		foreach (TSource item in source)
		{
			index = checked(index + 1);
			yield return (Index: index, Item: item);
		}
	}

	public static IEnumerable<T> InfiniteSequence<T>(T start, T step) where T : IAdditionOperators<T, T, T>
	{
		if (start == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.start);
		}
		if (step == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.step);
		}
		return Iterator(start, step);
		static IEnumerable<T> Iterator(T val, T val2)
		{
			while (true)
			{
				yield return val;
				val += val2;
			}
		}
	}

	/// <summary>Produces the set intersection of two sequences by using the default equality comparer to compare values.</summary>
	/// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in <paramref name="second" /> will be returned.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in the first sequence will be returned.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
	{
		return first.Intersect(second, null);
	}

	/// <summary>Produces the set intersection of two sequences by using the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</summary>
	/// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in <paramref name="second" /> will be returned.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements that also appear in the first sequence will be returned.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		return IntersectIterator(first, second, comparer);
	}

	public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
	{
		return first.IntersectBy(second, keySelector, null);
	}

	public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		return IntersectByIterator(first, second, keySelector, comparer);
	}

	private static IEnumerable<TSource> IntersectIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
	{
		HashSet<TSource> set = new HashSet<TSource>(second, comparer);
		foreach (TSource item in first)
		{
			if (set.Remove(item))
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<TSource> IntersectByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
	{
		HashSet<TKey> set = new HashSet<TKey>(second, comparer);
		foreach (TSource item in first)
		{
			if (set.Remove(keySelector(item)))
			{
				yield return item;
			}
		}
	}

	/// <summary>Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that has elements of type <paramref name="TResult" /> that are obtained by performing an inner join on two sequences.</returns>
	/// <param name="outer">The first sequence to join.</param>
	/// <param name="inner">The sequence to join to the first sequence.</param>
	/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
	/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
	/// <param name="resultSelector">A function to create a result element from two matching elements.</param>
	/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
	/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
	/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
	/// <typeparam name="TResult">The type of the result elements.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is null.</exception>
	public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
	{
		return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);
	}

	/// <summary>Correlates the elements of two sequences based on matching keys. A specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> is used to compare keys.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that has elements of type <paramref name="TResult" /> that are obtained by performing an inner join on two sequences.</returns>
	/// <param name="outer">The first sequence to join.</param>
	/// <param name="inner">The sequence to join to the first sequence.</param>
	/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
	/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
	/// <param name="resultSelector">A function to create a result element from two matching elements.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to hash and compare keys.</param>
	/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
	/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
	/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
	/// <typeparam name="TResult">The type of the result elements.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="outer" /> or <paramref name="inner" /> or <paramref name="outerKeySelector" /> or <paramref name="innerKeySelector" /> or <paramref name="resultSelector" /> is null.</exception>
	public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
	{
		if (outer == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outer);
		}
		if (inner == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.inner);
		}
		if (outerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outerKeySelector);
		}
		if (innerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.innerKeySelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(outer))
		{
			return Array.Empty<TResult>();
		}
		return JoinIterator(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	}

	private static IEnumerable<TResult> JoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
	{
		using IEnumerator<TOuter> e = outer.GetEnumerator();
		if (!e.MoveNext())
		{
			yield break;
		}
		Lookup<TKey, TInner> lookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
		if (lookup.Count == 0)
		{
			yield break;
		}
		do
		{
			TOuter item = e.Current;
			Grouping<TKey, TInner> grouping = lookup.GetGrouping(outerKeySelector(item), create: false);
			if (grouping != null)
			{
				int count = grouping._count;
				TInner[] elements = grouping._elements;
				int i = 0;
				while (i != count)
				{
					yield return resultSelector(item, elements[i]);
					int num = i + 1;
					i = num;
				}
			}
		}
		while (e.MoveNext());
	}

	/// <summary>Returns the last element of a sequence.</summary>
	/// <returns>The value at the last position in the source sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the last element of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
	public static TSource Last<TSource>(this IEnumerable<TSource> source)
	{
		TSource result = source.TryGetLast(out var found);
		if (!found)
		{
			ThrowHelper.ThrowNoElementsException();
		}
		return result;
	}

	/// <summary>Returns the last element of a sequence that satisfies a specified condition.</summary>
	/// <returns>The last element in the sequence that passes the test in the specified predicate function.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
	public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		TSource result = source.TryGetLast(predicate, out var found);
		if (!found)
		{
			ThrowHelper.ThrowNoMatchException();
		}
		return result;
	}

	/// <summary>Returns the last element of a sequence, or a default value if the sequence contains no elements.</summary>
	/// <returns>default(<paramref name="TSource" />) if the source sequence is empty; otherwise, the last element in the <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the last element of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static TSource? LastOrDefault<TSource>(this IEnumerable<TSource> source)
	{
		bool found;
		return source.TryGetLast(out found);
	}

	public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
	{
		TSource result = source.TryGetLast(out var found);
		if (!found)
		{
			return defaultValue;
		}
		return result;
	}

	/// <summary>Returns the last element of a sequence that satisfies a condition or a default value if no such element is found.</summary>
	/// <returns>default(<paramref name="TSource" />) if the sequence is empty or if no elements pass the test in the predicate function; otherwise, the last element that passes the test in the predicate function.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return an element from.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static TSource? LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		bool found;
		return source.TryGetLast(predicate, out found);
	}

	public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
	{
		TSource result = source.TryGetLast(predicate, out var found);
		if (!found)
		{
			return defaultValue;
		}
		return result;
	}

	private static TSource TryGetLast<TSource>(this IEnumerable<TSource> source, out bool found)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (!(source is Iterator<TSource> iterator))
		{
			return TryGetLastNonIterator(source, out found);
		}
		return iterator.TryGetLast(out found);
	}

	private static TSource TryGetLastNonIterator<TSource>(IEnumerable<TSource> source, out bool found)
	{
		if (source is IList<TSource> { Count: var count } list)
		{
			if (count > 0)
			{
				found = true;
				return list[count - 1];
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (enumerator.MoveNext())
			{
				TSource current;
				do
				{
					current = enumerator.Current;
				}
				while (enumerator.MoveNext());
				found = true;
				return current;
			}
		}
		found = false;
		return default(TSource);
	}

	private static TSource TryGetLast<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (source is OrderedIterator<TSource> orderedIterator)
		{
			return orderedIterator.TryGetLast(predicate, out found);
		}
		if (source is IList<TSource> list)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				TSource val = list[num];
				if (predicate(val))
				{
					found = true;
					return val;
				}
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TSource val2 = enumerator.Current;
				if (!predicate(val2))
				{
					continue;
				}
				while (enumerator.MoveNext())
				{
					TSource current = enumerator.Current;
					if (predicate(current))
					{
						val2 = current;
					}
				}
				found = true;
				return val2;
			}
		}
		found = false;
		return default(TSource);
	}

	public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner?, TResult> resultSelector)
	{
		return outer.LeftJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);
	}

	public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner?, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
	{
		if (outer == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outer);
		}
		if (inner == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.inner);
		}
		if (outerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outerKeySelector);
		}
		if (innerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.innerKeySelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(outer))
		{
			return Array.Empty<TResult>();
		}
		return LeftJoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	}

	private static IEnumerable<TResult> LeftJoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
	{
		using IEnumerator<TOuter> e = outer.GetEnumerator();
		if (!e.MoveNext())
		{
			yield break;
		}
		Lookup<TKey, TInner> innerLookup = Lookup<TKey, TInner>.CreateForJoin(inner, innerKeySelector, comparer);
		do
		{
			TOuter item = e.Current;
			Grouping<TKey, TInner> grouping = innerLookup.GetGrouping(outerKeySelector(item), create: false);
			if (grouping == null)
			{
				yield return resultSelector(item, default(TInner));
				continue;
			}
			int count = grouping._count;
			TInner[] elements = grouping._elements;
			int i = 0;
			while (i != count)
			{
				yield return resultSelector(item, elements[i]);
				int num = i + 1;
				i = num;
			}
		}
		while (e.MoveNext());
	}

	/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function.</summary>
	/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains keys and values.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return source.ToLookup(keySelector, null);
	}

	/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function and key comparer.</summary>
	/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains keys and values.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (IsEmptyArray(source))
		{
			return EmptyLookup<TKey, TSource>.Instance;
		}
		return Lookup<TKey, TSource>.Create(source, keySelector, comparer);
	}

	/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to specified key selector and element selector functions.</summary>
	/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is null.</exception>
	public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
	{
		return source.ToLookup(keySelector, elementSelector, null);
	}

	/// <summary>Creates a <see cref="T:System.Linq.Lookup`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function, a comparer and an element selector function.</summary>
	/// <returns>A <see cref="T:System.Linq.Lookup`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Linq.Lookup`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is null.</exception>
	public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (elementSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.elementSelector);
		}
		if (IsEmptyArray(source))
		{
			return EmptyLookup<TKey, TElement>.Instance;
		}
		return Lookup<TKey, TElement>.Create(source, keySelector, elementSelector, comparer);
	}

	/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Int32" /> values.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static int Max(this IEnumerable<int> source)
	{
		return source.MinMaxInteger<int, MaxCalc<int>>();
	}

	/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Int64" /> values.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static long Max(this IEnumerable<long> source)
	{
		return source.MinMaxInteger<long, MaxCalc<long>>();
	}

	/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the maximum value in the sequence. </returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static int? Max(this IEnumerable<int?> source)
	{
		return source.MaxInteger();
	}

	/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the maximum value in the sequence. </returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static long? Max(this IEnumerable<long?> source)
	{
		return source.MaxInteger();
	}

	private static T? MaxInteger<T>(this IEnumerable<T?> source) where T : struct, IBinaryInteger<T>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		T? result = null;
		using IEnumerator<T?> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = enumerator.Current;
			if (!result.HasValue)
			{
				continue;
			}
			T val = result.GetValueOrDefault();
			if (val >= T.Zero)
			{
				while (enumerator.MoveNext())
				{
					T? current = enumerator.Current;
					T valueOrDefault = current.GetValueOrDefault();
					if (valueOrDefault > val)
					{
						val = valueOrDefault;
						result = current;
					}
				}
			}
			else
			{
				while (enumerator.MoveNext())
				{
					T? current2 = enumerator.Current;
					T valueOrDefault2 = current2.GetValueOrDefault();
					if (current2.HasValue & (valueOrDefault2 > val))
					{
						val = valueOrDefault2;
						result = current2;
					}
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Double" /> values.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Double" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Max(this IEnumerable<double> source)
	{
		return source.MaxFloat();
	}

	/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Double" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static double? Max(this IEnumerable<double?> source)
	{
		return source.MaxFloat();
	}

	/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Single" /> values.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Single" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static float Max(this IEnumerable<float> source)
	{
		return source.MaxFloat();
	}

	/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Single" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static float? Max(this IEnumerable<float?> source)
	{
		return source.MaxFloat();
	}

	private static T MaxFloat<T>(this IEnumerable<T> source) where T : struct, IFloatingPointIeee754<T>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			int i;
			for (i = 0; i < span.Length && T.IsNaN(span[i]); i++)
			{
			}
			if (i == span.Length)
			{
				return span[span.Length - 1];
			}
			T val = span[i];
			for (; (uint)i < (uint)span.Length; i++)
			{
				if (span[i] > val)
				{
					val = span[i];
				}
			}
			return val;
		}
		using IEnumerator<T> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		T val = enumerator.Current;
		while (T.IsNaN(val))
		{
			if (!enumerator.MoveNext())
			{
				return val;
			}
			val = enumerator.Current;
		}
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (current > val)
			{
				val = current;
			}
		}
		return val;
	}

	private static T? MaxFloat<T>(this IEnumerable<T?> source) where T : struct, IFloatingPointIeee754<T>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		T? result = null;
		using IEnumerator<T?> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = enumerator.Current;
			if (!result.HasValue)
			{
				continue;
			}
			T val = result.GetValueOrDefault();
			while (T.IsNaN(val))
			{
				if (!enumerator.MoveNext())
				{
					return result;
				}
				T? current = enumerator.Current;
				if (current.HasValue)
				{
					T? val2 = (result = current);
					val = val2.GetValueOrDefault();
				}
			}
			while (enumerator.MoveNext())
			{
				T? current2 = enumerator.Current;
				T valueOrDefault = current2.GetValueOrDefault();
				if (current2.HasValue & (valueOrDefault > val))
				{
					val = valueOrDefault;
					result = current2;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Returns the maximum value in a sequence of <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static decimal Max(this IEnumerable<decimal> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			decimal num = span[0];
			for (int i = 1; (uint)i < (uint)span.Length; i++)
			{
				if (span[i] > num)
				{
					num = span[i];
				}
			}
			return num;
		}
		using IEnumerator<decimal> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		decimal num = enumerator.Current;
		while (enumerator.MoveNext())
		{
			decimal current = enumerator.Current;
			if (current > num)
			{
				num = current;
			}
		}
		return num;
	}

	/// <summary>Returns the maximum value in a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the maximum value in the sequence. </returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to determine the maximum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static decimal? Max(this IEnumerable<decimal?> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		decimal? result = null;
		using IEnumerator<decimal?> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = enumerator.Current;
			if (!result.HasValue)
			{
				continue;
			}
			decimal num = result.GetValueOrDefault();
			while (enumerator.MoveNext())
			{
				decimal? current = enumerator.Current;
				decimal valueOrDefault = current.GetValueOrDefault();
				if (current.HasValue && valueOrDefault > num)
				{
					num = valueOrDefault;
					result = current;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Returns the maximum value in a generic sequence.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static TSource? Max<TSource>(this IEnumerable<TSource> source)
	{
		return source.Max((IComparer<TSource>?)null);
	}

	public static TSource? Max<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (comparer == null)
		{
			comparer = Comparer<TSource>.Default;
		}
		if (typeof(TSource) == typeof(byte) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<byte>)source).MinMaxInteger<byte, MaxCalc<byte>>();
		}
		if (typeof(TSource) == typeof(sbyte) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<sbyte>)source).MinMaxInteger<sbyte, MaxCalc<sbyte>>();
		}
		if (typeof(TSource) == typeof(ushort) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<ushort>)source).MinMaxInteger<ushort, MaxCalc<ushort>>();
		}
		if (typeof(TSource) == typeof(short) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<short>)source).MinMaxInteger<short, MaxCalc<short>>();
		}
		if (typeof(TSource) == typeof(char) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<char>)source).MinMaxInteger<char, MaxCalc<char>>();
		}
		if (typeof(TSource) == typeof(uint) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<uint>)source).MinMaxInteger<uint, MaxCalc<uint>>();
		}
		if (typeof(TSource) == typeof(int) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<int>)source).MinMaxInteger<int, MaxCalc<int>>();
		}
		if (typeof(TSource) == typeof(ulong) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<ulong>)source).MinMaxInteger<ulong, MaxCalc<ulong>>();
		}
		if (typeof(TSource) == typeof(long) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<long>)source).MinMaxInteger<long, MaxCalc<long>>();
		}
		if (typeof(TSource) == typeof(nuint) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<nuint>)source).MinMaxInteger<nuint, MaxCalc<nuint>>();
		}
		if (typeof(TSource) == typeof(nint) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<nint>)source).MinMaxInteger<nint, MaxCalc<nint>>();
		}
		if (typeof(TSource) == typeof(Int128) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<Int128>)source).MinMaxInteger<Int128, MaxCalc<Int128>>();
		}
		if (typeof(TSource) == typeof(UInt128) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<UInt128>)source).MinMaxInteger<UInt128, MaxCalc<UInt128>>();
		}
		TSource val = default(TSource);
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (val == null)
		{
			do
			{
				if (!enumerator.MoveNext())
				{
					return val;
				}
				val = enumerator.Current;
			}
			while (val == null);
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				if (current != null && comparer.Compare(current, val) > 0)
				{
					val = current;
				}
			}
		}
		else
		{
			if (!enumerator.MoveNext())
			{
				ThrowHelper.ThrowNoElementsException();
			}
			val = enumerator.Current;
			if (comparer == Comparer<TSource>.Default)
			{
				while (enumerator.MoveNext())
				{
					TSource current2 = enumerator.Current;
					if (Comparer<TSource>.Default.Compare(current2, val) > 0)
					{
						val = current2;
					}
				}
			}
			else
			{
				while (enumerator.MoveNext())
				{
					TSource current3 = enumerator.Current;
					if (comparer.Compare(current3, val) > 0)
					{
						val = current3;
					}
				}
			}
		}
		return val;
	}

	public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return source.MaxBy(keySelector, null);
	}

	public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (comparer == null)
		{
			comparer = Comparer<TKey>.Default;
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			if (default(TSource) == null)
			{
				return default(TSource);
			}
			ThrowHelper.ThrowNoElementsException();
		}
		TSource val = enumerator.Current;
		TKey val2 = keySelector(val);
		if (default(TKey) == null)
		{
			if (val2 == null)
			{
				TSource result = val;
				do
				{
					if (!enumerator.MoveNext())
					{
						return result;
					}
					val = enumerator.Current;
					val2 = keySelector(val);
				}
				while (val2 == null);
			}
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				TKey val3 = keySelector(current);
				if (val3 != null && comparer.Compare(val3, val2) > 0)
				{
					val2 = val3;
					val = current;
				}
			}
		}
		else if (comparer == Comparer<TKey>.Default)
		{
			while (enumerator.MoveNext())
			{
				TSource current2 = enumerator.Current;
				TKey val4 = keySelector(current2);
				if (Comparer<TKey>.Default.Compare(val4, val2) > 0)
				{
					val2 = val4;
					val = current2;
				}
			}
		}
		else
		{
			while (enumerator.MoveNext())
			{
				TSource current3 = enumerator.Current;
				TKey val5 = keySelector(current3);
				if (comparer.Compare(val5, val2) > 0)
				{
					val2 = val5;
					val = current3;
				}
			}
		}
		return val;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Int32" /> value.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
	{
		return source.MaxInteger(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Int32" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
	{
		return source.MaxInteger(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Int64" /> value.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
	{
		return source.MaxInteger(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Int64" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
	{
		return source.MaxInteger(selector);
	}

	private static TResult MaxInteger<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) where TResult : struct, IBinaryInteger<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		TResult val = selector(enumerator.Current);
		while (enumerator.MoveNext())
		{
			TResult val2 = selector(enumerator.Current);
			if (val2 > val)
			{
				val = val2;
			}
		}
		return val;
	}

	private static TResult? MaxInteger<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : struct, IBinaryInteger<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TResult? result = null;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = selector(enumerator.Current);
			if (!result.HasValue)
			{
				continue;
			}
			TResult val = result.GetValueOrDefault();
			if (val >= TResult.Zero)
			{
				while (enumerator.MoveNext())
				{
					TResult? val2 = selector(enumerator.Current);
					TResult valueOrDefault = val2.GetValueOrDefault();
					if (valueOrDefault > val)
					{
						val = valueOrDefault;
						result = val2;
					}
				}
			}
			else
			{
				while (enumerator.MoveNext())
				{
					TResult? val3 = selector(enumerator.Current);
					TResult valueOrDefault2 = val3.GetValueOrDefault();
					if (val3.HasValue & (valueOrDefault2 > val))
					{
						val = valueOrDefault2;
						result = val3;
					}
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Single" /> value.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
	{
		return source.MaxFloat(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Single" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
	{
		return source.MaxFloat(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Double" /> value.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
	{
		return source.MaxFloat(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Double" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
	{
		return source.MaxFloat(selector);
	}

	private static TResult MaxFloat<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) where TResult : struct, IFloatingPointIeee754<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		TResult val = selector(enumerator.Current);
		while (TResult.IsNaN(val))
		{
			if (!enumerator.MoveNext())
			{
				return val;
			}
			val = selector(enumerator.Current);
		}
		while (enumerator.MoveNext())
		{
			TResult val2 = selector(enumerator.Current);
			if (val2 > val)
			{
				val = val2;
			}
		}
		return val;
	}

	private static TResult? MaxFloat<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : struct, IFloatingPointIeee754<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TResult? result = null;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = selector(enumerator.Current);
			if (!result.HasValue)
			{
				continue;
			}
			TResult val = result.GetValueOrDefault();
			while (TResult.IsNaN(val))
			{
				if (!enumerator.MoveNext())
				{
					return result;
				}
				TResult? val2 = selector(enumerator.Current);
				if (val2.HasValue)
				{
					TResult? val3 = (result = val2);
					val = val3.GetValueOrDefault();
				}
			}
			while (enumerator.MoveNext())
			{
				TResult? val4 = selector(enumerator.Current);
				TResult valueOrDefault = val4.GetValueOrDefault();
				if (val4.HasValue & (valueOrDefault > val))
				{
					val = valueOrDefault;
					result = val4;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum <see cref="T:System.Decimal" /> value.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		decimal num = selector(enumerator.Current);
		while (enumerator.MoveNext())
		{
			decimal num2 = selector(enumerator.Current);
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the maximum nullable <see cref="T:System.Decimal" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		decimal? result = null;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = selector(enumerator.Current);
			if (!result.HasValue)
			{
				continue;
			}
			decimal num = result.GetValueOrDefault();
			while (enumerator.MoveNext())
			{
				decimal? num2 = selector(enumerator.Current);
				decimal valueOrDefault = num2.GetValueOrDefault();
				if (num2.HasValue && valueOrDefault > num)
				{
					num = valueOrDefault;
					result = num2;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.</summary>
	/// <returns>The maximum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the maximum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static TResult? Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TResult val = default(TResult);
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (val == null)
		{
			do
			{
				if (!enumerator.MoveNext())
				{
					return val;
				}
				val = selector(enumerator.Current);
			}
			while (val == null);
			Comparer<TResult> comparer = Comparer<TResult>.Default;
			while (enumerator.MoveNext())
			{
				TResult val2 = selector(enumerator.Current);
				if (val2 != null && comparer.Compare(val2, val) > 0)
				{
					val = val2;
				}
			}
		}
		else
		{
			if (!enumerator.MoveNext())
			{
				ThrowHelper.ThrowNoElementsException();
			}
			val = selector(enumerator.Current);
			while (enumerator.MoveNext())
			{
				TResult val3 = selector(enumerator.Current);
				if (Comparer<TResult>.Default.Compare(val3, val) > 0)
				{
					val = val3;
				}
			}
		}
		return val;
	}

	private static T MinMaxInteger<T, TMinMax>(this IEnumerable<T> source) where T : struct, IBinaryInteger<T> where TMinMax : IMinMaxCalc<T>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		T val;
		if (source.TryGetSpan(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			if (!Vector128.IsHardwareAccelerated || !Vector128<T>.IsSupported || span.Length < Vector128<T>.Count)
			{
				val = span[0];
				for (int i = 1; i < span.Length; i++)
				{
					if (TMinMax.Compare(span[i], val))
					{
						val = span[i];
					}
				}
			}
			else if (!Vector256.IsHardwareAccelerated || !Vector256<T>.IsSupported || span.Length < Vector256<T>.Count)
			{
				ref T reference = ref MemoryMarshal.GetReference(span);
				ref T reference2 = ref Unsafe.Add(ref reference, span.Length - Vector128<T>.Count);
				Vector128<T> left = Vector128.LoadUnsafe(in reference);
				reference = ref Unsafe.Add(ref reference, Vector128<T>.Count);
				while (Unsafe.IsAddressLessThan(in reference, in reference2))
				{
					left = TMinMax.Compare(left, Vector128.LoadUnsafe(in reference));
					reference = ref Unsafe.Add(ref reference, Vector128<T>.Count);
				}
				left = TMinMax.Compare(left, Vector128.LoadUnsafe(in reference2));
				val = left[0];
				for (int j = 1; j < Vector128<T>.Count; j++)
				{
					if (TMinMax.Compare(left[j], val))
					{
						val = left[j];
					}
				}
			}
			else if (!Vector512.IsHardwareAccelerated || !Vector512<T>.IsSupported || span.Length < Vector512<T>.Count)
			{
				ref T reference3 = ref MemoryMarshal.GetReference(span);
				ref T reference4 = ref Unsafe.Add(ref reference3, span.Length - Vector256<T>.Count);
				Vector256<T> left2 = Vector256.LoadUnsafe(in reference3);
				reference3 = ref Unsafe.Add(ref reference3, Vector256<T>.Count);
				while (Unsafe.IsAddressLessThan(in reference3, in reference4))
				{
					left2 = TMinMax.Compare(left2, Vector256.LoadUnsafe(in reference3));
					reference3 = ref Unsafe.Add(ref reference3, Vector256<T>.Count);
				}
				left2 = TMinMax.Compare(left2, Vector256.LoadUnsafe(in reference4));
				val = left2[0];
				for (int k = 1; k < Vector256<T>.Count; k++)
				{
					if (TMinMax.Compare(left2[k], val))
					{
						val = left2[k];
					}
				}
			}
			else
			{
				ref T reference5 = ref MemoryMarshal.GetReference(span);
				ref T reference6 = ref Unsafe.Add(ref reference5, span.Length - Vector512<T>.Count);
				Vector512<T> left3 = Vector512.LoadUnsafe(in reference5);
				reference5 = ref Unsafe.Add(ref reference5, Vector512<T>.Count);
				while (Unsafe.IsAddressLessThan(in reference5, in reference6))
				{
					left3 = TMinMax.Compare(left3, Vector512.LoadUnsafe(in reference5));
					reference5 = ref Unsafe.Add(ref reference5, Vector512<T>.Count);
				}
				left3 = TMinMax.Compare(left3, Vector512.LoadUnsafe(in reference6));
				val = left3[0];
				for (int l = 1; l < Vector512<T>.Count; l++)
				{
					if (TMinMax.Compare(left3[l], val))
					{
						val = left3[l];
					}
				}
			}
		}
		else
		{
			using IEnumerator<T> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				ThrowHelper.ThrowNoElementsException();
			}
			val = enumerator.Current;
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (TMinMax.Compare(current, val))
				{
					val = current;
				}
			}
		}
		return val;
	}

	/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Int32" /> values.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static int Min(this IEnumerable<int> source)
	{
		return source.MinMaxInteger<int, MinCalc<int>>();
	}

	/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Int64" /> values.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static long Min(this IEnumerable<long> source)
	{
		return source.MinMaxInteger<long, MinCalc<long>>();
	}

	/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static int? Min(this IEnumerable<int?> source)
	{
		return source.MinInteger();
	}

	/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static long? Min(this IEnumerable<long?> source)
	{
		return source.MinInteger();
	}

	private static T? MinInteger<T>(this IEnumerable<T?> source) where T : struct, IBinaryInteger<T>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		T? result = null;
		using IEnumerator<T?> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = enumerator.Current;
			if (!result.HasValue)
			{
				continue;
			}
			T val = result.GetValueOrDefault();
			while (enumerator.MoveNext())
			{
				T? current = enumerator.Current;
				T valueOrDefault = current.GetValueOrDefault();
				if (current.HasValue & (valueOrDefault < val))
				{
					val = valueOrDefault;
					result = current;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Single" /> values.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Single" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static float Min(this IEnumerable<float> source)
	{
		return source.MinFloat();
	}

	/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Single" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static float? Min(this IEnumerable<float?> source)
	{
		return source.MinFloat();
	}

	/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Double" /> values.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Double" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Min(this IEnumerable<double> source)
	{
		return source.MinFloat();
	}

	/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Double" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static double? Min(this IEnumerable<double?> source)
	{
		return source.MinFloat();
	}

	private static T MinFloat<T>(this IEnumerable<T> source) where T : struct, IFloatingPointIeee754<T>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			T val = span[0];
			for (int i = 1; (uint)i < (uint)span.Length; i++)
			{
				T val2 = span[i];
				if (val2 < val)
				{
					val = val2;
				}
				else if (T.IsNaN(val2))
				{
					return val2;
				}
			}
			return val;
		}
		using IEnumerator<T> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		T val = enumerator.Current;
		if (T.IsNaN(val))
		{
			return val;
		}
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			if (current < val)
			{
				val = current;
			}
			else if (T.IsNaN(current))
			{
				return current;
			}
		}
		return val;
	}

	private static T? MinFloat<T>(this IEnumerable<T?> source) where T : struct, IFloatingPointIeee754<T>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		T? result = null;
		using IEnumerator<T?> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = enumerator.Current;
			if (!result.HasValue)
			{
				continue;
			}
			T val = result.GetValueOrDefault();
			if (T.IsNaN(val))
			{
				return result;
			}
			while (enumerator.MoveNext())
			{
				T? current = enumerator.Current;
				if (current.HasValue)
				{
					T valueOrDefault = current.GetValueOrDefault();
					if (valueOrDefault < val)
					{
						val = valueOrDefault;
						result = current;
					}
					else if (T.IsNaN(valueOrDefault))
					{
						return current;
					}
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Returns the minimum value in a sequence of <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static decimal Min(this IEnumerable<decimal> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan(out var span))
		{
			if (span.IsEmpty)
			{
				ThrowHelper.ThrowNoElementsException();
			}
			decimal num = span[0];
			for (int i = 1; (uint)i < (uint)span.Length; i++)
			{
				if (span[i] < num)
				{
					num = span[i];
				}
			}
			return num;
		}
		using IEnumerator<decimal> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		decimal num = enumerator.Current;
		while (enumerator.MoveNext())
		{
			decimal current = enumerator.Current;
			if (current < num)
			{
				num = current;
			}
		}
		return num;
	}

	/// <summary>Returns the minimum value in a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>A value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to determine the minimum value of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static decimal? Min(this IEnumerable<decimal?> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		decimal? result = null;
		using IEnumerator<decimal?> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = enumerator.Current;
			if (!result.HasValue)
			{
				continue;
			}
			decimal num = result.GetValueOrDefault();
			while (enumerator.MoveNext())
			{
				decimal? current = enumerator.Current;
				decimal valueOrDefault = current.GetValueOrDefault();
				if (current.HasValue && valueOrDefault < num)
				{
					num = valueOrDefault;
					result = current;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Returns the minimum value in a generic sequence.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static TSource? Min<TSource>(this IEnumerable<TSource> source)
	{
		return source.Min((IComparer<TSource>?)null);
	}

	public static TSource? Min<TSource>(this IEnumerable<TSource> source, IComparer<TSource>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (comparer == null)
		{
			comparer = Comparer<TSource>.Default;
		}
		if (typeof(TSource) == typeof(byte) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<byte>)source).MinMaxInteger<byte, MinCalc<byte>>();
		}
		if (typeof(TSource) == typeof(sbyte) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<sbyte>)source).MinMaxInteger<sbyte, MinCalc<sbyte>>();
		}
		if (typeof(TSource) == typeof(ushort) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<ushort>)source).MinMaxInteger<ushort, MinCalc<ushort>>();
		}
		if (typeof(TSource) == typeof(short) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<short>)source).MinMaxInteger<short, MinCalc<short>>();
		}
		if (typeof(TSource) == typeof(char) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<char>)source).MinMaxInteger<char, MinCalc<char>>();
		}
		if (typeof(TSource) == typeof(uint) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<uint>)source).MinMaxInteger<uint, MinCalc<uint>>();
		}
		if (typeof(TSource) == typeof(int) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<int>)source).MinMaxInteger<int, MinCalc<int>>();
		}
		if (typeof(TSource) == typeof(ulong) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<ulong>)source).MinMaxInteger<ulong, MinCalc<ulong>>();
		}
		if (typeof(TSource) == typeof(long) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<long>)source).MinMaxInteger<long, MinCalc<long>>();
		}
		if (typeof(TSource) == typeof(nuint) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<nuint>)source).MinMaxInteger<nuint, MinCalc<nuint>>();
		}
		if (typeof(TSource) == typeof(nint) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<nint>)source).MinMaxInteger<nint, MinCalc<nint>>();
		}
		if (typeof(TSource) == typeof(Int128) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<Int128>)source).MinMaxInteger<Int128, MinCalc<Int128>>();
		}
		if (typeof(TSource) == typeof(UInt128) && comparer == Comparer<TSource>.Default)
		{
			return (TSource)(object)((IEnumerable<UInt128>)source).MinMaxInteger<UInt128, MinCalc<UInt128>>();
		}
		TSource val = default(TSource);
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (val == null)
		{
			do
			{
				if (!enumerator.MoveNext())
				{
					return val;
				}
				val = enumerator.Current;
			}
			while (val == null);
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				if (current != null && comparer.Compare(current, val) < 0)
				{
					val = current;
				}
			}
		}
		else
		{
			if (!enumerator.MoveNext())
			{
				ThrowHelper.ThrowNoElementsException();
			}
			val = enumerator.Current;
			if (comparer == Comparer<TSource>.Default)
			{
				while (enumerator.MoveNext())
				{
					TSource current2 = enumerator.Current;
					if (Comparer<TSource>.Default.Compare(current2, val) < 0)
					{
						val = current2;
					}
				}
			}
			else
			{
				while (enumerator.MoveNext())
				{
					TSource current3 = enumerator.Current;
					if (comparer.Compare(current3, val) < 0)
					{
						val = current3;
					}
				}
			}
		}
		return val;
	}

	public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return source.MinBy(keySelector, null);
	}

	public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (comparer == null)
		{
			comparer = Comparer<TKey>.Default;
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			if (default(TSource) == null)
			{
				return default(TSource);
			}
			ThrowHelper.ThrowNoElementsException();
		}
		TSource val = enumerator.Current;
		TKey val2 = keySelector(val);
		if (default(TKey) == null)
		{
			if (val2 == null)
			{
				TSource result = val;
				do
				{
					if (!enumerator.MoveNext())
					{
						return result;
					}
					val = enumerator.Current;
					val2 = keySelector(val);
				}
				while (val2 == null);
			}
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				TKey val3 = keySelector(current);
				if (val3 != null && comparer.Compare(val3, val2) < 0)
				{
					val2 = val3;
					val = current;
				}
			}
		}
		else if (comparer == Comparer<TKey>.Default)
		{
			while (enumerator.MoveNext())
			{
				TSource current2 = enumerator.Current;
				TKey val4 = keySelector(current2);
				if (Comparer<TKey>.Default.Compare(val4, val2) < 0)
				{
					val2 = val4;
					val = current2;
				}
			}
		}
		else
		{
			while (enumerator.MoveNext())
			{
				TSource current3 = enumerator.Current;
				TKey val5 = keySelector(current3);
				if (comparer.Compare(val5, val2) < 0)
				{
					val2 = val5;
					val = current3;
				}
			}
		}
		return val;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Int32" /> value.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
	{
		return source.MinInteger(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Int32" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Int32&gt; in C# or Nullable(Of Int32) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
	{
		return source.MinInteger(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Int64" /> value.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
	{
		return source.MinInteger(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Int64" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Int64&gt; in C# or Nullable(Of Int64) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
	{
		return source.MinInteger(selector);
	}

	private static TResult MinInteger<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) where TResult : struct, IBinaryInteger<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		TResult val = selector(enumerator.Current);
		while (enumerator.MoveNext())
		{
			TResult val2 = selector(enumerator.Current);
			if (val2 < val)
			{
				val = val2;
			}
		}
		return val;
	}

	private static TResult? MinInteger<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : struct, IBinaryInteger<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TResult? result = null;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = selector(enumerator.Current);
			if (!result.HasValue)
			{
				continue;
			}
			TResult val = result.GetValueOrDefault();
			while (enumerator.MoveNext())
			{
				TResult? val2 = selector(enumerator.Current);
				TResult valueOrDefault = val2.GetValueOrDefault();
				if (val2.HasValue & (valueOrDefault < val))
				{
					val = valueOrDefault;
					result = val2;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Single" /> value.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
	{
		return source.MinFloat(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Single" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Single&gt; in C# or Nullable(Of Single) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
	{
		return source.MinFloat(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Double" /> value.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
	{
		return source.MinFloat(selector);
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Double" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Double&gt; in C# or Nullable(Of Double) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
	{
		return source.MinFloat(selector);
	}

	private static TResult MinFloat<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) where TResult : struct, IFloatingPointIeee754<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		TResult val = selector(enumerator.Current);
		if (TResult.IsNaN(val))
		{
			return val;
		}
		while (enumerator.MoveNext())
		{
			TResult val2 = selector(enumerator.Current);
			if (val2 < val)
			{
				val = val2;
			}
			else if (TResult.IsNaN(val2))
			{
				return val2;
			}
		}
		return val;
	}

	private static TResult? MinFloat<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : struct, IFloatingPointIeee754<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TResult? result = null;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = selector(enumerator.Current);
			if (!result.HasValue)
			{
				continue;
			}
			TResult val = result.GetValueOrDefault();
			if (TResult.IsNaN(val))
			{
				return result;
			}
			while (enumerator.MoveNext())
			{
				TResult? val2 = selector(enumerator.Current);
				if (val2.HasValue)
				{
					TResult valueOrDefault = val2.GetValueOrDefault();
					if (valueOrDefault < val)
					{
						val = valueOrDefault;
						result = val2;
					}
					else if (TResult.IsNaN(valueOrDefault))
					{
						return val2;
					}
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum <see cref="T:System.Decimal" /> value.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">
	///   <paramref name="source" /> contains no elements.</exception>
	public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			ThrowHelper.ThrowNoElementsException();
		}
		decimal num = selector(enumerator.Current);
		while (enumerator.MoveNext())
		{
			decimal num2 = selector(enumerator.Current);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	/// <summary>Invokes a transform function on each element of a sequence and returns the minimum nullable <see cref="T:System.Decimal" /> value.</summary>
	/// <returns>The value of type Nullable&lt;Decimal&gt; in C# or Nullable(Of Decimal) in Visual Basic that corresponds to the minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		decimal? result = null;
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = selector(enumerator.Current);
			if (!result.HasValue)
			{
				continue;
			}
			decimal num = result.GetValueOrDefault();
			while (enumerator.MoveNext())
			{
				decimal? num2 = selector(enumerator.Current);
				decimal valueOrDefault = num2.GetValueOrDefault();
				if (num2.HasValue && valueOrDefault < num)
				{
					num = valueOrDefault;
					result = num2;
				}
			}
			return result;
		}
		return result;
	}

	/// <summary>Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.</summary>
	/// <returns>The minimum value in the sequence.</returns>
	/// <param name="source">A sequence of values to determine the minimum value of.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static TResult? Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TResult val = default(TResult);
		using IEnumerator<TSource> enumerator = source.GetEnumerator();
		if (val == null)
		{
			do
			{
				if (!enumerator.MoveNext())
				{
					return val;
				}
				val = selector(enumerator.Current);
			}
			while (val == null);
			Comparer<TResult> comparer = Comparer<TResult>.Default;
			while (enumerator.MoveNext())
			{
				TResult val2 = selector(enumerator.Current);
				if (val2 != null && comparer.Compare(val2, val) < 0)
				{
					val = val2;
				}
			}
		}
		else
		{
			if (!enumerator.MoveNext())
			{
				ThrowHelper.ThrowNoElementsException();
			}
			val = selector(enumerator.Current);
			while (enumerator.MoveNext())
			{
				TResult val3 = selector(enumerator.Current);
				if (Comparer<TResult>.Default.Compare(val3, val) < 0)
				{
					val = val3;
				}
			}
		}
		return val;
	}

	/// <summary>Filters the elements of an <see cref="T:System.Collections.IEnumerable" /> based on a specified type.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence of type <paramref name="TResult" />.</returns>
	/// <param name="source">The <see cref="T:System.Collections.IEnumerable" /> whose elements to filter.</param>
	/// <typeparam name="TResult">The type to filter the elements of the sequence on.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (default(TResult) != null && source is IEnumerable<TResult> result)
		{
			return result;
		}
		return new OfTypeIterator<TResult>(source);
	}

	public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source)
	{
		return source.Order(null);
	}

	public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source, IComparer<T>? comparer)
	{
		if (!TypeIsImplicitlyStable<T>() || (comparer != null && comparer != Comparer<T>.Default))
		{
			return source.OrderBy(EnumerableSorter<T>.IdentityFunc, comparer);
		}
		return new ImplicitlyStableOrderedIterator<T>(source, descending: false);
	}

	/// <summary>Sorts the elements of a sequence in ascending order according to a key.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
	/// <param name="source">A sequence of values to order.</param>
	/// <param name="keySelector">A function to extract a key from an element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return new OrderedIterator<TSource, TKey>(source, keySelector, null, descending: false, null);
	}

	/// <summary>Sorts the elements of a sequence in ascending order by using a specified comparer.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
	/// <param name="source">A sequence of values to order.</param>
	/// <param name="keySelector">A function to extract a key from an element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		return new OrderedIterator<TSource, TKey>(source, keySelector, comparer, descending: false, null);
	}

	public static IOrderedEnumerable<T> OrderDescending<T>(this IEnumerable<T> source)
	{
		return source.OrderDescending(null);
	}

	public static IOrderedEnumerable<T> OrderDescending<T>(this IEnumerable<T> source, IComparer<T>? comparer)
	{
		if (!TypeIsImplicitlyStable<T>() || (comparer != null && comparer != Comparer<T>.Default))
		{
			return source.OrderByDescending(EnumerableSorter<T>.IdentityFunc, comparer);
		}
		return new ImplicitlyStableOrderedIterator<T>(source, descending: true);
	}

	/// <summary>Sorts the elements of a sequence in descending order according to a key.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
	/// <param name="source">A sequence of values to order.</param>
	/// <param name="keySelector">A function to extract a key from an element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		return new OrderedIterator<TSource, TKey>(source, keySelector, null, descending: true, null);
	}

	/// <summary>Sorts the elements of a sequence in descending order by using a specified comparer.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
	/// <param name="source">A sequence of values to order.</param>
	/// <param name="keySelector">A function to extract a key from an element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		return new OrderedIterator<TSource, TKey>(source, keySelector, comparer, descending: true, null);
	}

	/// <summary>Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
	/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		return source.CreateOrderedEnumerable(keySelector, null, descending: false);
	}

	/// <summary>Performs a subsequent ordering of the elements in a sequence in ascending order by using a specified comparer.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted according to a key.</returns>
	/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		return source.CreateOrderedEnumerable(keySelector, comparer, descending: false);
	}

	/// <summary>Performs a subsequent ordering of the elements in a sequence in descending order, according to a key.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
	/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		return source.CreateOrderedEnumerable(keySelector, null, descending: true);
	}

	/// <summary>Performs a subsequent ordering of the elements in a sequence in descending order by using a specified comparer.</summary>
	/// <returns>An <see cref="T:System.Linq.IOrderedEnumerable`1" /> whose elements are sorted in descending order according to a key.</returns>
	/// <param name="source">An <see cref="T:System.Linq.IOrderedEnumerable`1" /> that contains elements to sort.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.</exception>
	public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		return source.CreateOrderedEnumerable(keySelector, comparer, descending: true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TypeIsImplicitlyStable<T>()
	{
		Type type = typeof(T);
		if (typeof(T).IsEnum)
		{
			type = typeof(T).GetEnumUnderlyingType();
		}
		if (!(type == typeof(sbyte)) && !(type == typeof(byte)) && !(type == typeof(bool)) && !(type == typeof(short)) && !(type == typeof(ushort)) && !(type == typeof(char)) && !(type == typeof(int)) && !(type == typeof(uint)) && !(type == typeof(long)) && !(type == typeof(ulong)) && !(type == typeof(Int128)) && !(type == typeof(UInt128)) && !(type == typeof(nint)))
		{
			return type == typeof(nuint);
		}
		return true;
	}

	/// <summary>Generates a sequence of integral numbers within a specified range.</summary>
	/// <returns>An IEnumerable&lt;Int32&gt; in C# or IEnumerable(Of Int32) in Visual Basic that contains a range of sequential integral numbers.</returns>
	/// <param name="start">The value of the first integer in the sequence.</param>
	/// <param name="count">The number of sequential integers to generate.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="count" /> is less than 0.-or-<paramref name="start" /> + <paramref name="count" /> -1 is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static IEnumerable<int> Range(int start, int count)
	{
		long num = (long)start + (long)count - 1;
		if (count < 0 || num > int.MaxValue)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
		}
		if (count == 0)
		{
			return Array.Empty<int>();
		}
		return new RangeIterator<int>(start, start + count);
	}

	private static void FillIncrementing<T>(Span<T> destination, T value) where T : INumber<T>
	{
		ref T reference = ref MemoryMarshal.GetReference(destination);
		ref T reference2 = ref Unsafe.Add(ref reference, destination.Length);
		if (Vector.IsHardwareAccelerated && Vector<T>.IsSupported && destination.Length >= Vector<T>.Count)
		{
			Vector<T> indices = Vector<T>.Indices;
			Vector<T> source = new Vector<T>(value) + indices;
			Vector<T> vector = new Vector<T>(T.CreateTruncating(Vector<T>.Count));
			ref T right = ref Unsafe.Subtract(ref reference2, Vector<T>.Count);
			do
			{
				source.StoreUnsafe(ref reference);
				source += vector;
				reference = ref Unsafe.Add(ref reference, Vector<T>.Count);
			}
			while (Unsafe.IsAddressLessThanOrEqualTo(in reference, in right));
			value = source[0];
		}
		while (Unsafe.IsAddressLessThan(in reference, in reference2))
		{
			reference = value++;
			reference = ref Unsafe.Add(ref reference, 1);
		}
	}

	/// <summary>Generates a sequence that contains one repeated value.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains a repeated value.</returns>
	/// <param name="element">The value to be repeated.</param>
	/// <param name="count">The number of times to repeat the value in the generated sequence.</param>
	/// <typeparam name="TResult">The type of the value to be repeated in the result sequence.</typeparam>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="count" /> is less than 0.</exception>
	public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
	{
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count);
		}
		if (count == 0)
		{
			return Array.Empty<TResult>();
		}
		return new RepeatIterator<TResult>(element, count);
	}

	public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return new ShuffleIterator<TSource>(source);
	}

	/// <summary>Inverts the order of the elements in a sequence.</summary>
	/// <returns>A sequence whose elements correspond to those of the input sequence in reverse order.</returns>
	/// <param name="source">A sequence of values to reverse.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return new ReverseIterator<TSource>(source);
	}

	public static IEnumerable<TSource> Reverse<TSource>(this TSource[] source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.Length == 0)
		{
			return Array.Empty<TSource>();
		}
		return new ReverseIterator<TSource>(source);
	}

	public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector)
	{
		return outer.RightJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);
	}

	public static IEnumerable<TResult> RightJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
	{
		if (outer == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outer);
		}
		if (inner == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.inner);
		}
		if (outerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.outerKeySelector);
		}
		if (innerKeySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.innerKeySelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(inner))
		{
			return Array.Empty<TResult>();
		}
		return RightJoinIterator<TOuter, TInner, TKey, TResult>(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
	}

	private static IEnumerable<TResult> RightJoinIterator<TOuter, TInner, TKey, TResult>(IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
	{
		using IEnumerator<TInner> e = inner.GetEnumerator();
		if (!e.MoveNext())
		{
			yield break;
		}
		Lookup<TKey, TOuter> outerLookup = Lookup<TKey, TOuter>.CreateForJoin(outer, outerKeySelector, comparer);
		do
		{
			TInner item = e.Current;
			Grouping<TKey, TOuter> grouping = outerLookup.GetGrouping(innerKeySelector(item), create: false);
			if (grouping == null)
			{
				yield return resultSelector(default(TOuter), item);
				continue;
			}
			int count = grouping._count;
			TOuter[] elements = grouping._elements;
			int i = 0;
			while (i != count)
			{
				yield return resultSelector(elements[i], item);
				int num = i + 1;
				i = num;
			}
		}
		while (e.MoveNext());
	}

	/// <summary>Projects each element of a sequence into a new form.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the transform function on each element of <paramref name="source" />.</returns>
	/// <param name="source">A sequence of values to invoke a transform function on.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		if (source is Iterator<TSource> iterator)
		{
			if (IsSizeOptimized && typeof(TResult).IsValueType)
			{
				if (!(source is IList<TSource> source2))
				{
					return new IEnumerableSelectIterator<TSource, TResult>(iterator, selector);
				}
				return new SizeOptIListSelectIterator<TSource, TResult>(source2, selector);
			}
			return iterator.Select(selector);
		}
		if (source is IList<TSource> source3)
		{
			if (IsSizeOptimized)
			{
				return new SizeOptIListSelectIterator<TSource, TResult>(source3, selector);
			}
			if (source is TSource[] array)
			{
				if (array.Length == 0)
				{
					return Array.Empty<TResult>();
				}
				return new ArraySelectIterator<TSource, TResult>(array, selector);
			}
			if (source is List<TSource> source4)
			{
				return new ListSelectIterator<TSource, TResult>(source4, selector);
			}
			return new IListSelectIterator<TSource, TResult>(source3, selector);
		}
		return new IEnumerableSelectIterator<TSource, TResult>(source, selector);
	}

	/// <summary>Projects each element of a sequence into a new form by incorporating the element's index.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the transform function on each element of <paramref name="source" />.</returns>
	/// <param name="source">A sequence of values to invoke a transform function on.</param>
	/// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TResult>();
		}
		return SelectIterator(source, selector);
	}

	private static IEnumerable<TResult> SelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
	{
		int index = -1;
		foreach (TSource item in source)
		{
			index = checked(index + 1);
			yield return selector(item, index);
		}
	}

	/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" /> and flattens the resulting sequences into one sequence.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.</returns>
	/// <param name="source">A sequence of values to project.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TResult">The type of the elements of the sequence returned by <paramref name="selector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TResult>();
		}
		return new SelectManySingleSelectorIterator<TSource, TResult>(source, selector);
	}

	/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" />, and flattens the resulting sequences into one sequence. The index of each source element is used in the projected form of that element.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function on each element of an input sequence.</returns>
	/// <param name="source">A sequence of values to project.</param>
	/// <param name="selector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TResult">The type of the elements of the sequence returned by <paramref name="selector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TResult>();
		}
		return SelectManyIterator(source, selector);
	}

	private static IEnumerable<TResult> SelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
	{
		int index = -1;
		foreach (TSource item in source)
		{
			index = checked(index + 1);
			foreach (TResult item2 in selector(item, index))
			{
				yield return item2;
			}
		}
	}

	/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" />, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein. The index of each source element is used in the intermediate projected form of that element.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function <paramref name="collectionSelector" /> on each element of <paramref name="source" /> and then mapping each of those sequence elements and their corresponding source element to a result element.</returns>
	/// <param name="source">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each source element; the second parameter of the function represents the index of the source element.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TCollection">The type of the intermediate elements collected by <paramref name="collectionSelector" />.</typeparam>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="collectionSelector" /> or <paramref name="resultSelector" /> is null.</exception>
	public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (collectionSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collectionSelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TResult>();
		}
		return SelectManyIterator(source, collectionSelector, resultSelector);
	}

	private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		int index = -1;
		foreach (TSource element in source)
		{
			index = checked(index + 1);
			foreach (TCollection item in collectionSelector(element, index))
			{
				yield return resultSelector(element, item);
			}
		}
	}

	/// <summary>Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IEnumerable`1" />, flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the one-to-many transform function <paramref name="collectionSelector" /> on each element of <paramref name="source" /> and then mapping each of those sequence elements and their corresponding source element to a result element.</returns>
	/// <param name="source">A sequence of values to project.</param>
	/// <param name="collectionSelector">A transform function to apply to each element of the input sequence.</param>
	/// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TCollection">The type of the intermediate elements collected by <paramref name="collectionSelector" />.</typeparam>
	/// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="collectionSelector" /> or <paramref name="resultSelector" /> is null.</exception>
	public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (collectionSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collectionSelector);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TResult>();
		}
		return SelectManyIterator(source, collectionSelector, resultSelector);
	}

	private static IEnumerable<TResult> SelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
	{
		foreach (TSource element in source)
		{
			foreach (TCollection item in collectionSelector(element))
			{
				yield return resultSelector(element, item);
			}
		}
	}

	public static IEnumerable<T> Sequence<T>(T start, T endInclusive, T step) where T : INumber<T>
	{
		if (start == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.start);
		}
		if (T.IsNaN(start))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
		}
		if (endInclusive == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.endInclusive);
		}
		if (T.IsNaN(endInclusive))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.endInclusive);
		}
		if (step == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.step);
		}
		if (T.IsNaN(step))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.step);
		}
		if (T.IsZero(step))
		{
			if (start != endInclusive)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.step);
			}
			return Repeat(start, 1);
		}
		if (T.IsPositive(step))
		{
			if (endInclusive < start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.endInclusive);
			}
			RangeIterator<T> result;
			if (typeof(T) == typeof(byte) && (result = TryUseRange<ushort>(start, endInclusive, step, 255)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(sbyte) && (result = TryUseRange<short>(start, endInclusive, step, 127)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(ushort) && (result = TryUseRange<uint>(start, endInclusive, step, 65535u)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(char) && (result = TryUseRange<uint>(start, endInclusive, step, 65535u)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(short) && (result = TryUseRange<int>(start, endInclusive, step, 32767)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(uint) && (result = TryUseRange<ulong>(start, endInclusive, step, 4294967295uL)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(int) && (result = TryUseRange<long>(start, endInclusive, step, 2147483647L)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(ulong) && (result = TryUseRange<UInt128>(start, endInclusive, step, ulong.MaxValue)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(long) && (result = TryUseRange<Int128>(start, endInclusive, step, long.MaxValue)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(nuint) && (result = TryUseRange<UInt128>(start, endInclusive, step, UIntPtr.MaxValue)) != null)
			{
				return result;
			}
			if (typeof(T) == typeof(nint) && (result = TryUseRange<Int128>(start, endInclusive, step, IntPtr.MaxValue)) != null)
			{
				return result;
			}
			return IncrementingIterator(start, endInclusive, step);
		}
		if (endInclusive > start)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.endInclusive);
		}
		return DecrementingIterator(start, endInclusive, step);
		static IEnumerable<T> DecrementingIterator(T current, T val2, T val)
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
		static IEnumerable<T> IncrementingIterator(T current, T val2, T val)
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
		static RangeIterator<T> TryUseRange<TLarger>(T val3, T val2, T val, TLarger maxValue) where TLarger : INumber<TLarger>
		{
			if (val == T.One && ((INumberBase<TLarger>)TLarger/*cast due to constrained. prefix*/).CreateTruncating(val2) - ((INumberBase<TLarger>)TLarger/*cast due to constrained. prefix*/).CreateTruncating(val3) + ((INumberBase<TLarger>)TLarger).One <= maxValue)
			{
				return new RangeIterator<T>(val3, val2 + T.One);
			}
			return null;
		}
	}

	/// <summary>Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.</summary>
	/// <returns>true if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type; otherwise, false.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to <paramref name="second" />.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to the first sequence.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
	{
		return first.SequenceEqual(second, null);
	}

	/// <summary>Determines whether two sequences are equal by comparing their elements by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
	/// <returns>true if the two source sequences are of equal length and their corresponding elements compare equal according to <paramref name="comparer" />; otherwise, false.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to <paramref name="second" />.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to compare to the first sequence.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to use to compare elements.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (first is ICollection<TSource> collection && second is ICollection<TSource> collection2)
		{
			if (first.TryGetSpan<TSource>(out var span) && second.TryGetSpan<TSource>(out var span2))
			{
				return span.SequenceEqual(span2, comparer);
			}
			if (collection.Count != collection2.Count)
			{
				return false;
			}
			if (collection is IList<TSource> list && collection2 is IList<TSource> list2)
			{
				if (comparer == null)
				{
					comparer = EqualityComparer<TSource>.Default;
				}
				int count = collection.Count;
				for (int i = 0; i < count; i++)
				{
					if (!comparer.Equals(list[i], list2[i]))
					{
						return false;
					}
				}
				return true;
			}
		}
		using IEnumerator<TSource> enumerator = first.GetEnumerator();
		using IEnumerator<TSource> enumerator2 = second.GetEnumerator();
		if (comparer == null)
		{
			comparer = EqualityComparer<TSource>.Default;
		}
		while (enumerator.MoveNext())
		{
			if (!enumerator2.MoveNext() || !comparer.Equals(enumerator.Current, enumerator2.Current))
			{
				return false;
			}
		}
		return !enumerator2.MoveNext();
	}

	/// <summary>Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.</summary>
	/// <returns>The single element of the input sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the single element of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.-or-The input sequence is empty.</exception>
	public static TSource Single<TSource>(this IEnumerable<TSource> source)
	{
		TSource result = source.TryGetSingle(out var found);
		if (!found)
		{
			ThrowHelper.ThrowNoElementsException();
		}
		return result;
	}

	/// <summary>Returns the only element of a sequence that satisfies a specified condition, and throws an exception if more than one such element exists.</summary>
	/// <returns>The single element of the input sequence that satisfies a condition.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return a single element from.</param>
	/// <param name="predicate">A function to test an element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-More than one element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
	public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		TSource result = source.TryGetSingle(predicate, out var found);
		if (!found)
		{
			ThrowHelper.ThrowNoMatchException();
		}
		return result;
	}

	/// <summary>Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
	/// <returns>The single element of the input sequence, or default(<paramref name="TSource" />) if the sequence contains no elements.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return the single element of.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
	public static TSource? SingleOrDefault<TSource>(this IEnumerable<TSource> source)
	{
		bool found;
		return source.TryGetSingle(out found);
	}

	public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
	{
		TSource result = source.TryGetSingle(out var found);
		if (!found)
		{
			return defaultValue;
		}
		return result;
	}

	/// <summary>Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists; this method throws an exception if more than one element satisfies the condition.</summary>
	/// <returns>The single element of the input sequence that satisfies the condition, or default(<paramref name="TSource" />) if no such element is found.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return a single element from.</param>
	/// <param name="predicate">A function to test an element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static TSource? SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		bool found;
		return source.TryGetSingle(predicate, out found);
	}

	public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
	{
		TSource result = source.TryGetSingle(predicate, out var found);
		if (!found)
		{
			return defaultValue;
		}
		return result;
	}

	private static TSource TryGetSingle<TSource>(this IEnumerable<TSource> source, out bool found)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source is IList<TSource> { Count: var count } list)
		{
			switch (count)
			{
			case 0:
				found = false;
				return default(TSource);
			case 1:
				found = true;
				return list[0];
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				found = false;
				return default(TSource);
			}
			TSource current = enumerator.Current;
			if (!enumerator.MoveNext())
			{
				found = true;
				return current;
			}
		}
		found = false;
		ThrowHelper.ThrowMoreThanOneElementException();
		return default(TSource);
	}

	private static TSource TryGetSingle<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (source.TryGetSpan(out var span))
		{
			for (int i = 0; i < span.Length; i++)
			{
				TSource val = span[i];
				if (!predicate(val))
				{
					continue;
				}
				for (i++; (uint)i < (uint)span.Length; i++)
				{
					if (predicate(span[i]))
					{
						ThrowHelper.ThrowMoreThanOneMatchException();
					}
				}
				found = true;
				return val;
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TSource current = enumerator.Current;
				if (!predicate(current))
				{
					continue;
				}
				while (enumerator.MoveNext())
				{
					if (predicate(enumerator.Current))
					{
						ThrowHelper.ThrowMoreThanOneMatchException();
					}
				}
				found = true;
				return current;
			}
		}
		found = false;
		return default(TSource);
	}

	/// <summary>Bypasses a specified number of elements in a sequence and then returns the remaining elements.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements that occur after the specified index in the input sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return elements from.</param>
	/// <param name="count">The number of elements to skip before returning the remaining elements.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		if (count <= 0)
		{
			if (source is Iterator<TSource>)
			{
				return source;
			}
			count = 0;
		}
		else if (source is Iterator<TSource> iterator)
		{
			IEnumerable<TSource> enumerable = iterator.Skip(count);
			return enumerable ?? Empty<TSource>();
		}
		return SpeedOptimizedSkipIterator(source, count);
	}

	/// <summary>Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from the input sequence starting at the first element in the linear series that does not pass the test specified by <paramref name="predicate" />.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return elements from.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return SkipWhileIterator(source, predicate);
	}

	private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		using IEnumerator<TSource> e = source.GetEnumerator();
		while (e.MoveNext())
		{
			TSource current = e.Current;
			if (!predicate(current))
			{
				yield return current;
				while (e.MoveNext())
				{
					yield return e.Current;
				}
				break;
			}
		}
	}

	/// <summary>Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements. The element's index is used in the logic of the predicate function.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from the input sequence starting at the first element in the linear series that does not pass the test specified by <paramref name="predicate" />.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to return elements from.</param>
	/// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return SkipWhileIterator(source, predicate);
	}

	private static IEnumerable<TSource> SkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		using IEnumerator<TSource> e = source.GetEnumerator();
		int num = -1;
		while (e.MoveNext())
		{
			num = checked(num + 1);
			TSource current = e.Current;
			if (!predicate(current, num))
			{
				yield return current;
				while (e.MoveNext())
				{
					yield return e.Current;
				}
				break;
			}
		}
	}

	public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (!IsEmptyArray(source))
		{
			if (count > 0)
			{
				return TakeRangeFromEndIterator(source, isStartIndexFromEnd: false, 0, isEndIndexFromEnd: true, count);
			}
			return source.Skip(0);
		}
		return Array.Empty<TSource>();
	}

	private static IEnumerable<TSource> SpeedOptimizedSkipIterator<TSource>(IEnumerable<TSource> source, int count)
	{
		if (!(source is IList<TSource> source2))
		{
			return new IEnumerableSkipTakeIterator<TSource>(source, count, -1);
		}
		return new IListSkipTakeIterator<TSource>(source2, count, int.MaxValue);
	}

	/// <summary>Computes the sum of a sequence of <see cref="T:System.Int32" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int32" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static int Sum(this IEnumerable<int> source)
	{
		return source.Sum<int, int>();
	}

	/// <summary>Computes the sum of a sequence of <see cref="T:System.Int64" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Int64" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static long Sum(this IEnumerable<long> source)
	{
		return source.Sum<long, long>();
	}

	/// <summary>Computes the sum of a sequence of <see cref="T:System.Single" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Single" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static float Sum(this IEnumerable<float> source)
	{
		return (float)source.Sum<float, double>();
	}

	/// <summary>Computes the sum of a sequence of <see cref="T:System.Double" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Double" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static double Sum(this IEnumerable<double> source)
	{
		return source.Sum<double, double>();
	}

	/// <summary>Computes the sum of a sequence of <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of <see cref="T:System.Decimal" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
	public static decimal Sum(this IEnumerable<decimal> source)
	{
		return source.Sum<decimal, decimal>();
	}

	private static TResult Sum<TSource, TResult>(this IEnumerable<TSource> source) where TSource : struct, INumber<TSource> where TResult : struct, INumber<TResult>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (source.TryGetSpan(out var span))
		{
			return Sum<TSource, TResult>(span);
		}
		TResult val = TResult.Zero;
		foreach (TSource item in source)
		{
			val = checked(val + TResult.CreateChecked(item));
		}
		return val;
	}

	private static TResult Sum<T, TResult>(ReadOnlySpan<T> span) where T : struct, INumber<T> where TResult : struct, INumber<TResult>
	{
		if (typeof(T) == typeof(TResult) && Vector<T>.IsSupported && Vector.IsHardwareAccelerated && Vector<T>.Count > 2 && span.Length >= Vector<T>.Count * 4)
		{
			if (typeof(T) == typeof(long))
			{
				return (TResult)(object)SumSignedIntegersVectorized(Unsafe.BitCast<ReadOnlySpan<T>, ReadOnlySpan<long>>(span));
			}
			if (typeof(T) == typeof(int))
			{
				return (TResult)(object)SumSignedIntegersVectorized(Unsafe.BitCast<ReadOnlySpan<T>, ReadOnlySpan<int>>(span));
			}
		}
		TResult val = TResult.Zero;
		ReadOnlySpan<T> readOnlySpan = span;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			T value = readOnlySpan[i];
			val = checked(val + TResult.CreateChecked(value));
		}
		return val;
	}

	private static T SumSignedIntegersVectorized<T>(ReadOnlySpan<T> span) where T : struct, IBinaryInteger<T>, ISignedNumber<T>, IMinMaxValue<T>
	{
		ref T reference = ref MemoryMarshal.GetReference(span);
		nuint num = (nuint)span.Length;
		Vector<T> vector = Vector<T>.Zero;
		Vector<T> vector2 = new Vector<T>(T.MinValue);
		nuint num2 = 0u;
		nuint num3 = num - (nuint)((nint)Vector<T>.Count * (nint)4);
		do
		{
			Vector<T> vector3 = Vector.LoadUnsafe(in reference, num2);
			Vector<T> vector4 = vector + vector3;
			Vector<T> vector5 = (vector4 ^ vector) & (vector4 ^ vector3);
			vector3 = Vector.LoadUnsafe(in reference, num2 + (nuint)Vector<T>.Count);
			vector = vector4 + vector3;
			Vector<T> vector6 = vector5 | ((vector ^ vector4) & (vector ^ vector3));
			vector3 = Vector.LoadUnsafe(in reference, num2 + (nuint)((nint)Vector<T>.Count * (nint)2));
			vector4 = vector + vector3;
			Vector<T> vector7 = vector6 | ((vector4 ^ vector) & (vector4 ^ vector3));
			vector3 = Vector.LoadUnsafe(in reference, num2 + (nuint)((nint)Vector<T>.Count * (nint)3));
			vector = vector4 + vector3;
			if (((vector7 | ((vector ^ vector4) & (vector ^ vector3))) & vector2) != Vector<T>.Zero)
			{
				ThrowHelper.ThrowOverflowException();
			}
			num2 += (nuint)((nint)Vector<T>.Count * (nint)4);
		}
		while (num2 < num3);
		num3 = num - (nuint)Vector<T>.Count;
		if (num2 < num3)
		{
			Vector<T> zero = Vector<T>.Zero;
			do
			{
				Vector<T> vector8 = Vector.LoadUnsafe(in reference, num2);
				Vector<T> vector9 = vector + vector8;
				zero |= (vector9 ^ vector) & (vector9 ^ vector8);
				vector = vector9;
				num2 += (nuint)Vector<T>.Count;
			}
			while (num2 < num3);
			if ((zero & vector2) != Vector<T>.Zero)
			{
				ThrowHelper.ThrowOverflowException();
			}
		}
		T val = T.Zero;
		for (int i = 0; i < Vector<T>.Count; i++)
		{
			val = checked(val + vector[i]);
		}
		for (; num2 < num; num2++)
		{
			val = checked(val + Unsafe.Add(ref reference, num2));
		}
		return val;
	}

	/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Int32" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int32" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static int? Sum(this IEnumerable<int?> source)
	{
		return source.Sum<int, int>();
	}

	/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Int64" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Int64" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static long? Sum(this IEnumerable<long?> source)
	{
		return source.Sum<long, long>();
	}

	/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Single" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Single" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static float? Sum(this IEnumerable<float?> source)
	{
		return source.Sum<float, double>();
	}

	/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Double" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Double" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static double? Sum(this IEnumerable<double?> source)
	{
		return source.Sum<double, double>();
	}

	/// <summary>Computes the sum of a sequence of nullable <see cref="T:System.Decimal" /> values.</summary>
	/// <returns>The sum of the values in the sequence.</returns>
	/// <param name="source">A sequence of nullable <see cref="T:System.Decimal" /> values to calculate the sum of.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
	public static decimal? Sum(this IEnumerable<decimal?> source)
	{
		return source.Sum<decimal, decimal>();
	}

	private static TSource? Sum<TSource, TAccumulator>(this IEnumerable<TSource?> source) where TSource : struct, INumber<TSource> where TAccumulator : struct, INumber<TAccumulator>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		TAccumulator val = TAccumulator.Zero;
		foreach (TSource? item in source)
		{
			if (item.HasValue)
			{
				val = checked(val + TAccumulator.CreateChecked(item.GetValueOrDefault()));
			}
		}
		return TSource.CreateTruncating(val);
	}

	/// <summary>Computes the sum of the sequence of <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
	{
		return source.Sum<TSource, int, int>(selector);
	}

	/// <summary>Computes the sum of the sequence of <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
	{
		return source.Sum<TSource, long, long>(selector);
	}

	/// <summary>Computes the sum of the sequence of <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
	{
		return source.Sum<TSource, float, double>(selector);
	}

	/// <summary>Computes the sum of the sequence of <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
	{
		return source.Sum<TSource, double, double>(selector);
	}

	/// <summary>Computes the sum of the sequence of <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
	public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
	{
		return source.Sum<TSource, decimal, decimal>(selector);
	}

	private static TResult Sum<TSource, TResult, TAccumulator>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) where TResult : struct, INumber<TResult> where TAccumulator : struct, INumber<TAccumulator>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TAccumulator val = TAccumulator.Zero;
		foreach (TSource item in source)
		{
			val = checked(val + TAccumulator.CreateChecked(selector(item)));
		}
		return TResult.CreateTruncating(val);
	}

	/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Int32" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
	{
		return source.Sum<TSource, int, int>(selector);
	}

	/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Int64" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
	public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
	{
		return source.Sum<TSource, long, long>(selector);
	}

	/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Single" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
	{
		return source.Sum<TSource, float, double>(selector);
	}

	/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Double" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
	{
		return source.Sum<TSource, double, double>(selector);
	}

	/// <summary>Computes the sum of the sequence of nullable <see cref="T:System.Decimal" /> values that are obtained by invoking a transform function on each element of the input sequence.</summary>
	/// <returns>The sum of the projected values.</returns>
	/// <param name="source">A sequence of values that are used to calculate a sum.</param>
	/// <param name="selector">A transform function to apply to each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="selector" /> is null.</exception>
	/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
	public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
	{
		return source.Sum<TSource, decimal, decimal>(selector);
	}

	private static TResult? Sum<TSource, TResult, TAccumulator>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector) where TResult : struct, INumber<TResult> where TAccumulator : struct, INumber<TAccumulator>
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (selector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.selector);
		}
		TAccumulator val = TAccumulator.Zero;
		foreach (TSource item in source)
		{
			TResult? val2 = selector(item);
			if (val2.HasValue)
			{
				TResult valueOrDefault = val2.GetValueOrDefault();
				val = checked(val + TAccumulator.CreateChecked(valueOrDefault));
			}
		}
		return TResult.CreateTruncating(val);
	}

	/// <summary>Returns a specified number of contiguous elements from the start of a sequence.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the specified number of elements from the start of the input sequence.</returns>
	/// <param name="source">The sequence to return elements from.</param>
	/// <param name="count">The number of elements to return.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (count <= 0 || IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return SpeedOptimizedTakeIterator(source, count);
	}

	public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, Range range)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
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
				return Array.Empty<TSource>();
			}
		}
		else if (!isFromEnd2)
		{
			if (value >= value2)
			{
				return Array.Empty<TSource>();
			}
			return SpeedOptimizedTakeRangeIterator(source, value, value2);
		}
		return TakeRangeFromEndIterator(source, isFromEnd, value, isFromEnd2, value2);
	}

	private static IEnumerable<TSource> TakeRangeFromEndIterator<TSource>(IEnumerable<TSource> source, bool isStartIndexFromEnd, int startIndex, bool isEndIndexFromEnd, int endIndex)
	{
		if (source.TryGetNonEnumeratedCount(out var count))
		{
			startIndex = CalculateStartIndex(isStartIndexFromEnd, startIndex, count);
			endIndex = CalculateEndIndex(isEndIndexFromEnd, endIndex, count);
			if (startIndex >= endIndex)
			{
				yield break;
			}
			IEnumerable<TSource> enumerable = SpeedOptimizedTakeRangeIterator(source, startIndex, endIndex);
			foreach (TSource item in enumerable)
			{
				yield return item;
			}
			yield break;
		}
		if (isStartIndexFromEnd)
		{
			Queue<TSource> queue;
			using (IEnumerator<TSource> enumerator2 = source.GetEnumerator())
			{
				if (!enumerator2.MoveNext())
				{
					yield break;
				}
				queue = new Queue<TSource>();
				queue.Enqueue(enumerator2.Current);
				count = 1;
				while (enumerator2.MoveNext())
				{
					if (count < startIndex)
					{
						queue.Enqueue(enumerator2.Current);
						count++;
						continue;
					}
					do
					{
						queue.Dequeue();
						queue.Enqueue(enumerator2.Current);
						count = checked(count + 1);
					}
					while (enumerator2.MoveNext());
					break;
				}
			}
			startIndex = CalculateStartIndex(isStartIndexFromEnd: true, startIndex, count);
			endIndex = CalculateEndIndex(isEndIndexFromEnd, endIndex, count);
			for (int rangeIndex = startIndex; rangeIndex < endIndex; rangeIndex++)
			{
				yield return queue.Dequeue();
			}
			yield break;
		}
		using (IEnumerator<TSource> enumerator = source.GetEnumerator())
		{
			for (count = 0; count < startIndex; count++)
			{
				if (!enumerator.MoveNext())
				{
					break;
				}
			}
			if (count != startIndex)
			{
				yield break;
			}
			Queue<TSource> queue = new Queue<TSource>();
			while (enumerator.MoveNext())
			{
				if (queue.Count == endIndex)
				{
					do
					{
						queue.Enqueue(enumerator.Current);
						yield return queue.Dequeue();
					}
					while (enumerator.MoveNext());
					break;
				}
				queue.Enqueue(enumerator.Current);
			}
		}
		static int CalculateEndIndex(bool flag, int num2, int num)
		{
			return Math.Min(num, flag ? (num - num2) : num2);
		}
		static int CalculateStartIndex(bool flag, int num2, int num)
		{
			return Math.Max(0, flag ? (num - num2) : num2);
		}
	}

	/// <summary>Returns elements from a sequence as long as a specified condition is true.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from the input sequence that occur before the element at which the test no longer passes.</returns>
	/// <param name="source">A sequence to return elements from.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return TakeWhileIterator(source, predicate);
	}

	private static IEnumerable<TSource> TakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		foreach (TSource item in source)
		{
			if (!predicate(item))
			{
				break;
			}
			yield return item;
		}
	}

	/// <summary>Returns elements from a sequence as long as a specified condition is true. The element's index is used in the logic of the predicate function.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence that occur before the element at which the test no longer passes.</returns>
	/// <param name="source">The sequence to return elements from.</param>
	/// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return TakeWhileIterator(source, predicate);
	}

	private static IEnumerable<TSource> TakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		int index = -1;
		foreach (TSource item in source)
		{
			index = checked(index + 1);
			if (!predicate(item, index))
			{
				break;
			}
			yield return item;
		}
	}

	public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (count > 0 && !IsEmptyArray(source))
		{
			return TakeRangeFromEndIterator(source, isStartIndexFromEnd: true, count, isEndIndexFromEnd: true, 0);
		}
		return Array.Empty<TSource>();
	}

	private static IEnumerable<TSource> SpeedOptimizedTakeIterator<TSource>(IEnumerable<TSource> source, int count)
	{
		if (!(source is Iterator<TSource> iterator))
		{
			if (!(source is IList<TSource> source2))
			{
				return new IEnumerableSkipTakeIterator<TSource>(source, 0, count - 1);
			}
			return new IListSkipTakeIterator<TSource>(source2, 0, count - 1);
		}
		IEnumerable<TSource> enumerable = iterator.Take(count);
		return enumerable ?? Empty<TSource>();
	}

	private static IEnumerable<TSource> SpeedOptimizedTakeRangeIterator<TSource>(IEnumerable<TSource> source, int startIndex, int endIndex)
	{
		if (!(source is Iterator<TSource> iterator))
		{
			if (!(source is IList<TSource> source2))
			{
				return new IEnumerableSkipTakeIterator<TSource>(source, startIndex, endIndex - 1);
			}
			return new IListSkipTakeIterator<TSource>(source2, startIndex, endIndex - 1);
		}
		return TakeIteratorRange(iterator, startIndex, endIndex);
		static IEnumerable<TSource> TakeIteratorRange(Iterator<TSource> iterator3, int num2, int num)
		{
			Iterator<TSource> iterator2;
			if (num != 0 && (iterator2 = iterator3.Take(num)) != null && (num2 == 0 || (iterator2 = iterator2.Skip(num2)) != null))
			{
				return iterator2;
			}
			return Array.Empty<TSource>();
		}
	}

	/// <summary>Creates an array from a <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
	/// <returns>An array that contains the elements from the input sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create an array from.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
	{
		if (!IsSizeOptimized && source is Iterator<TSource> iterator)
		{
			return iterator.ToArray();
		}
		if (source is ICollection<TSource> collection)
		{
			return ICollectionToArray(collection);
		}
		return EnumerableToArray(source);
		[MethodImpl(MethodImplOptions.NoInlining)]
		static TSource[] EnumerableToArray(IEnumerable<TSource> enumerable)
		{
			if (enumerable == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
			}
			SegmentedArrayBuilder<TSource>.ScratchBuffer buffer = default(SegmentedArrayBuilder<TSource>.ScratchBuffer);
			SegmentedArrayBuilder<TSource> segmentedArrayBuilder = new SegmentedArrayBuilder<TSource>(buffer);
			segmentedArrayBuilder.AddNonICollectionRangeInlined(enumerable);
			TSource[] result = segmentedArrayBuilder.ToArray();
			segmentedArrayBuilder.Dispose();
			return result;
		}
	}

	private static TSource[] ICollectionToArray<TSource>(ICollection<TSource> collection)
	{
		int count = collection.Count;
		if (count != 0)
		{
			TSource[] array = new TSource[count];
			collection.CopyTo(array, 0);
			return array;
		}
		return Array.Empty<TSource>();
	}

	/// <summary>Creates a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" />.</summary>
	/// <returns>A <see cref="T:System.Collections.Generic.List`1" /> that contains elements from the input sequence.</returns>
	/// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.List`1" /> from.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> is null.</exception>
	public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (!IsSizeOptimized && source is Iterator<TSource> iterator)
		{
			return iterator.ToList();
		}
		return new List<TSource>(source);
	}

	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) where TKey : notnull
	{
		return source.ToDictionary(null);
	}

	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? comparer) where TKey : notnull
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		return new Dictionary<TKey, TValue>(source, comparer);
	}

	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey Key, TValue Value)> source) where TKey : notnull
	{
		return source.ToDictionary(null);
	}

	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey Key, TValue Value)> source, IEqualityComparer<TKey>? comparer) where TKey : notnull
	{
		return source.ToDictionary<(TKey, TValue), TKey, TValue>(((TKey Key, TValue Value) vt) => vt.Key, ((TKey Key, TValue Value) vt) => vt.Value, comparer);
	}

	/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function.</summary>
	/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.-or-<paramref name="keySelector" /> produces a key that is null.</exception>
	/// <exception cref="T:System.ArgumentException">
	///   <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
	public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
	{
		return source.ToDictionary(keySelector, null);
	}

	/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function and key comparer.</summary>
	/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the keys returned by <paramref name="keySelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> is null.-or-<paramref name="keySelector" /> produces a key that is null.</exception>
	/// <exception cref="T:System.ArgumentException">
	///   <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
	public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer) where TKey : notnull
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (source.TryGetNonEnumeratedCount(out var count))
		{
			if (count == 0)
			{
				return new Dictionary<TKey, TSource>(comparer);
			}
			if (source is TSource[] array)
			{
				return SpanToDictionary(array, keySelector, comparer);
			}
			if (source is List<TSource> list)
			{
				return SpanToDictionary(CollectionsMarshal.AsSpan(list), keySelector, comparer);
			}
		}
		Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(count, comparer);
		foreach (TSource item in source)
		{
			dictionary.Add(keySelector(item), item);
		}
		return dictionary;
	}

	private static Dictionary<TKey, TSource> SpanToDictionary<TSource, TKey>(ReadOnlySpan<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
	{
		Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(source.Length, comparer);
		ReadOnlySpan<TSource> readOnlySpan = source;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			TSource val = readOnlySpan[i];
			dictionary.Add(keySelector(val), val);
		}
		return dictionary;
	}

	/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to specified key selector and element selector functions.</summary>
	/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is null.-or-<paramref name="keySelector" /> produces a key that is null.</exception>
	/// <exception cref="T:System.ArgumentException">
	///   <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
	public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) where TKey : notnull
	{
		return source.ToDictionary(keySelector, elementSelector, null);
	}

	/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to a specified key selector function, a comparer, and an element selector function.</summary>
	/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
	/// <param name="keySelector">A function to extract a key from each element.</param>
	/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
	/// <param name="comparer">An <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare keys.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
	/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is null.-or-<paramref name="keySelector" /> produces a key that is null.</exception>
	/// <exception cref="T:System.ArgumentException">
	///   <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
	public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer) where TKey : notnull
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		if (elementSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.elementSelector);
		}
		if (source.TryGetNonEnumeratedCount(out var count))
		{
			if (count == 0)
			{
				return new Dictionary<TKey, TElement>(comparer);
			}
			if (source is TSource[] array)
			{
				return SpanToDictionary(array, keySelector, elementSelector, comparer);
			}
			if (source is List<TSource> list)
			{
				return SpanToDictionary(CollectionsMarshal.AsSpan(list), keySelector, elementSelector, comparer);
			}
		}
		Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(count, comparer);
		foreach (TSource item in source)
		{
			dictionary.Add(keySelector(item), elementSelector(item));
		}
		return dictionary;
	}

	private static Dictionary<TKey, TElement> SpanToDictionary<TSource, TKey, TElement>(ReadOnlySpan<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
	{
		Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(source.Length, comparer);
		ReadOnlySpan<TSource> readOnlySpan = source;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			TSource arg = readOnlySpan[i];
			dictionary.Add(keySelector(arg), elementSelector(arg));
		}
		return dictionary;
	}

	public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
	{
		return source.ToHashSet(null);
	}

	public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		return new HashSet<TSource>(source, comparer);
	}

	/// <summary>Produces the set union of two sequences by using the default equality comparer.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from both input sequences, excluding duplicates.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the first set for the union.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the second set for the union.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
	{
		return first.Union(second, null);
	}

	/// <summary>Produces the set union of two sequences by using a specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from both input sequences, excluding duplicates.</returns>
	/// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the first set for the union.</param>
	/// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the second set for the union.</param>
	/// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
	/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource>? comparer)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (!(first is UnionIterator<TSource> unionIterator) || !Utilities.AreEqualityComparersEqual<TSource>(comparer, unionIterator._comparer))
		{
			return new UnionIterator2<TSource>(first, second, comparer);
		}
		return unionIterator.Union(second);
	}

	public static IEnumerable<TSource> UnionBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
	{
		return first.UnionBy(second, keySelector, null);
	}

	public static IEnumerable<TSource> UnionBy<TSource, TKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (keySelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.keySelector);
		}
		return UnionByIterator(first, second, keySelector, comparer);
	}

	private static IEnumerable<TSource> UnionByIterator<TSource, TKey>(IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
	{
		HashSet<TKey> set = new HashSet<TKey>(7, comparer);
		foreach (TSource item in first)
		{
			if (set.Add(keySelector(item)))
			{
				yield return item;
			}
		}
		foreach (TSource item2 in second)
		{
			if (set.Add(keySelector(item2)))
			{
				yield return item2;
			}
		}
	}

	/// <summary>Filters a sequence of values based on a predicate.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence that satisfy the condition.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to filter.</param>
	/// <param name="predicate">A function to test each element for a condition.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (source is Iterator<TSource> iterator)
		{
			return iterator.Where(predicate);
		}
		if (IsSizeOptimized && source is IList<TSource> source2)
		{
			return new SizeOptIListWhereIterator<TSource>(source2, predicate);
		}
		if (source is TSource[] array)
		{
			if (array.Length == 0)
			{
				return Array.Empty<TSource>();
			}
			return new ArrayWhereIterator<TSource>(array, predicate);
		}
		if (source is List<TSource> source3)
		{
			return new ListWhereIterator<TSource>(source3, predicate);
		}
		return new IEnumerableWhereIterator<TSource>(source, predicate);
	}

	/// <summary>Filters a sequence of values based on a predicate. Each element's index is used in the logic of the predicate function.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains elements from the input sequence that satisfy the condition.</returns>
	/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to filter.</param>
	/// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
	/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
	public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		if (source == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
		}
		if (predicate == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.predicate);
		}
		if (IsEmptyArray(source))
		{
			return Array.Empty<TSource>();
		}
		return WhereIterator(source, predicate);
	}

	private static IEnumerable<TSource> WhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
	{
		int index = -1;
		foreach (TSource item in source)
		{
			index = checked(index + 1);
			if (predicate(item, index))
			{
				yield return item;
			}
		}
	}

	/// <summary>Applies a specified function to the corresponding elements of two sequences, producing a sequence of the results.</summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains merged elements of two input sequences.</returns>
	/// <param name="first">The first sequence to merge.</param>
	/// <param name="second">The second sequence to merge.</param>
	/// <param name="resultSelector">A function that specifies how to merge the elements from the two sequences.</param>
	/// <typeparam name="TFirst">The type of the elements of the first input sequence.</typeparam>
	/// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
	/// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="first" /> or <paramref name="second" /> is null.</exception>
	public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (resultSelector == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);
		}
		return ZipIterator(first, second, resultSelector);
	}

	public static IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		return ZipIterator(first, second);
	}

	public static IEnumerable<(TFirst First, TSecond Second, TThird Third)> Zip<TFirst, TSecond, TThird>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third)
	{
		if (first == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.first);
		}
		if (second == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.second);
		}
		if (third == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.third);
		}
		return ZipIterator(first, second, third);
	}

	private static IEnumerable<(TFirst First, TSecond Second)> ZipIterator<TFirst, TSecond>(IEnumerable<TFirst> first, IEnumerable<TSecond> second)
	{
		using IEnumerator<TFirst> e1 = first.GetEnumerator();
		using IEnumerator<TSecond> e2 = second.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext())
		{
			yield return (First: e1.Current, Second: e2.Current);
		}
	}

	private static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
	{
		using IEnumerator<TFirst> e1 = first.GetEnumerator();
		using IEnumerator<TSecond> e2 = second.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext())
		{
			yield return resultSelector(e1.Current, e2.Current);
		}
	}

	private static IEnumerable<(TFirst First, TSecond Second, TThird Third)> ZipIterator<TFirst, TSecond, TThird>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third)
	{
		using IEnumerator<TFirst> e1 = first.GetEnumerator();
		using IEnumerator<TSecond> e2 = second.GetEnumerator();
		using IEnumerator<TThird> e3 = third.GetEnumerator();
		while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
		{
			yield return (First: e1.Current, Second: e2.Current, Third: e3.Current);
		}
	}
}
