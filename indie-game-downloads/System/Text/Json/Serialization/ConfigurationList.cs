using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json.Serialization;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
[DebuggerTypeProxy(typeof(ConfigurationList<>.ConfigurationListDebugView))]
internal abstract class ConfigurationList<TItem> : IList<TItem>, ICollection<TItem>, IEnumerable<TItem>, IEnumerable
{
	private sealed class ConfigurationListDebugView(ConfigurationList<TItem> collection)
	{
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public TItem[] Items => collection._list.ToArray();
	}

	protected readonly List<TItem> _list;

	public abstract bool IsReadOnly { get; }

	public TItem this[int index]
	{
		get
		{
			return _list[index];
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			OnCollectionModifying();
			ValidateAddedValue(value);
			_list[index] = value;
			OnCollectionModified();
		}
	}

	public int Count => _list.Count;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DebuggerDisplay => $"Count = {Count}, IsReadOnly = {IsReadOnly}";

	public ConfigurationList(IEnumerable<TItem> source = null)
	{
		_list = ((source == null) ? new List<TItem>() : new List<TItem>(source));
	}

	protected abstract void OnCollectionModifying();

	protected virtual void OnCollectionModified()
	{
	}

	protected virtual void ValidateAddedValue(TItem item)
	{
	}

	public void Add(TItem item)
	{
		ArgumentNullException.ThrowIfNull(item, "item");
		OnCollectionModifying();
		ValidateAddedValue(item);
		_list.Add(item);
		OnCollectionModified();
	}

	public void Clear()
	{
		OnCollectionModifying();
		_list.Clear();
		OnCollectionModified();
	}

	public bool Contains(TItem item)
	{
		return _list.Contains(item);
	}

	public void CopyTo(TItem[] array, int arrayIndex)
	{
		_list.CopyTo(array, arrayIndex);
	}

	public List<TItem>.Enumerator GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	public int IndexOf(TItem item)
	{
		return _list.IndexOf(item);
	}

	public void Insert(int index, TItem item)
	{
		ArgumentNullException.ThrowIfNull(item, "item");
		OnCollectionModifying();
		ValidateAddedValue(item);
		_list.Insert(index, item);
		OnCollectionModified();
	}

	public bool Remove(TItem item)
	{
		OnCollectionModifying();
		bool num = _list.Remove(item);
		if (num)
		{
			OnCollectionModified();
		}
		return num;
	}

	public void RemoveAt(int index)
	{
		OnCollectionModifying();
		_list.RemoveAt(index);
		OnCollectionModified();
	}

	IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _list.GetEnumerator();
	}
}
