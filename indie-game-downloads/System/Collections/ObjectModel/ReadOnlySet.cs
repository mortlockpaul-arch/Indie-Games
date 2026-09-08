using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Collections.ObjectModel;

[CollectionBuilder(typeof(ReadOnlyCollection), "CreateSet")]
[DebuggerDisplay("Count = {Count}")]
public class ReadOnlySet<T> : IReadOnlySet<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable, ISet<T>, ICollection<T>, ICollection
{
	private readonly ISet<T> _set;

	public static ReadOnlySet<T> Empty { get; } = new ReadOnlySet<T>(new HashSet<T>());

	protected ISet<T> Set => _set;

	public int Count => _set.Count;

	bool ICollection<T>.IsReadOnly => true;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (!(_set is ICollection collection))
			{
				return this;
			}
			return collection.SyncRoot;
		}
	}

	public ReadOnlySet(ISet<T> set)
	{
		ArgumentNullException.ThrowIfNull(set, "set");
		_set = set;
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (_set.Count != 0)
		{
			return _set.GetEnumerator();
		}
		return ((IEnumerable<T>)Array.Empty<T>()).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool Contains(T item)
	{
		return _set.Contains(item);
	}

	public bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return _set.IsProperSubsetOf(other);
	}

	public bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return _set.IsProperSupersetOf(other);
	}

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		return _set.IsSubsetOf(other);
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		return _set.IsSupersetOf(other);
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		return _set.Overlaps(other);
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		return _set.SetEquals(other);
	}

	void ICollection<T>.CopyTo(T[] array, int arrayIndex)
	{
		_set.CopyTo(array, arrayIndex);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		CollectionHelpers.CopyTo(_set, array, index);
	}

	bool ISet<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ISet<T>.ExceptWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	void ISet<T>.IntersectWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	void ISet<T>.UnionWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	void ICollection<T>.Clear()
	{
		throw new NotSupportedException();
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}
}
