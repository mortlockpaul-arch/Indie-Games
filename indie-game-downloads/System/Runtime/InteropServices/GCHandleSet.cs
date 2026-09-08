using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Runtime.InteropServices;

internal sealed class GCHandleSet : IEnumerable<GCHandle>, IEnumerable
{
	private sealed class Entry
	{
		public Entry _next;

		public GCHandle _value;
	}

	public struct Enumerator : IEnumerator<GCHandle>, IDisposable, IEnumerator
	{
		private readonly Entry[] _buckets;

		private Entry _currentEntry;

		private int _currentIdx;

		public GCHandle Current
		{
			get
			{
				if (_currentEntry == null)
				{
					throw new InvalidOperationException("InvalidOperation_EnumOpCantHappen");
				}
				return _currentEntry._value;
			}
		}

		object IEnumerator.Current => Current;

		public Enumerator(GCHandleSet set)
		{
			_currentEntry = null;
			_currentIdx = 0;
			_buckets = set._buckets;
			Reset();
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (_currentEntry != null)
			{
				_currentEntry = _currentEntry._next;
			}
			if (_currentEntry == null)
			{
				while (++_currentIdx != _buckets.Length)
				{
					_currentEntry = _buckets[_currentIdx];
					if (_currentEntry != null)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		public void Reset()
		{
			_currentIdx = -1;
			_currentEntry = null;
		}
	}

	private const int DefaultSize = 7;

	private Entry[] _buckets = new Entry[7];

	private int _numEntries;

	private readonly Lock _lock = new Lock(useTrivialWaits: true);

	public Lock ModificationLock => _lock;

	public void Add(GCHandle handle)
	{
		using (_lock.EnterScope())
		{
			int bucket = GetBucket(handle, _buckets.Length);
			Entry entry = null;
			for (Entry entry2 = _buckets[bucket]; entry2 != null; entry2 = entry2._next)
			{
				if (handle.Equals(entry2._value))
				{
					return;
				}
				entry = entry2;
			}
			Entry entry3 = new Entry
			{
				_value = handle
			};
			if (entry == null)
			{
				_buckets[bucket] = entry3;
			}
			else
			{
				entry._next = entry3;
			}
			_numEntries++;
			if (_numEntries > _buckets.Length * 2)
			{
				ExpandBuckets();
			}
		}
	}

	private void ExpandBuckets()
	{
		int num = _buckets.Length * 2 + 1;
		Entry[] array = new Entry[num];
		for (int i = 0; i < _buckets.Length; i++)
		{
			Entry entry = _buckets[i];
			while (entry != null)
			{
				Entry next = entry._next;
				int bucket = GetBucket(entry._value, num);
				Entry entry2 = new Entry
				{
					_value = entry._value,
					_next = array[bucket]
				};
				array[bucket] = entry2;
				entry = next;
			}
		}
		_buckets = array;
	}

	public void Remove(GCHandle handle)
	{
		using (_lock.EnterScope())
		{
			int bucket = GetBucket(handle, _buckets.Length);
			Entry entry = null;
			for (Entry entry2 = _buckets[bucket]; entry2 != null; entry2 = entry2._next)
			{
				if (handle.Equals(entry2._value))
				{
					if (entry == null)
					{
						_buckets[bucket] = entry2._next;
					}
					else
					{
						entry._next = entry2._next;
					}
					_numEntries--;
					break;
				}
				entry = entry2;
			}
		}
	}

	private static int GetBucket(GCHandle handle, int numBuckets)
	{
		return (int)((uint)handle.GetHashCode() % (uint)numBuckets);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<GCHandle> IEnumerable<GCHandle>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<GCHandle>)this).GetEnumerator();
	}
}
