using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Diagnostics;

public struct TagList : IList<KeyValuePair<string, object?>>, ICollection<KeyValuePair<string, object?>>, IEnumerable<KeyValuePair<string, object?>>, IEnumerable, IReadOnlyList<KeyValuePair<string, object?>>, IReadOnlyCollection<KeyValuePair<string, object?>>
{
	[InlineArray(8)]
	private struct InlineTags
	{
		private KeyValuePair<string, object> _first;
	}

	public struct Enumerator : IEnumerator<KeyValuePair<string, object?>>, IEnumerator, IDisposable
	{
		private TagList _tagList;

		private int _index;

		public KeyValuePair<string, object?> Current => _tagList[_index];

		object IEnumerator.Current => _tagList[_index];

		internal Enumerator(in TagList tagList)
		{
			_index = -1;
			_tagList = tagList;
		}

		public void Dispose()
		{
			_index = _tagList.Count;
		}

		public bool MoveNext()
		{
			_index++;
			return _index < _tagList.Count;
		}

		public void Reset()
		{
			_index = -1;
		}
	}

	private InlineTags _tags;

	private KeyValuePair<string, object>[] _overflowTags;

	private int _tagsCount;

	public readonly int Count => _tagsCount;

	public readonly bool IsReadOnly => false;

	public KeyValuePair<string, object?> this[int index]
	{
		readonly get
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)_tagsCount, "index");
			if (_overflowTags != null)
			{
				return _overflowTags[index];
			}
			return _tags[index];
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)_tagsCount, "index");
			if (_overflowTags == null)
			{
				_tags[index] = value;
			}
			else
			{
				_overflowTags[index] = value;
			}
		}
	}

	[UnscopedRef]
	internal readonly ReadOnlySpan<KeyValuePair<string, object?>> Tags
	{
		get
		{
			if (_overflowTags == null)
			{
				return ((ReadOnlySpan<KeyValuePair<string, object>>)_tags).Slice(0, _tagsCount);
			}
			return _overflowTags.AsSpan(0, _tagsCount);
		}
	}

	public TagList(params ReadOnlySpan<KeyValuePair<string, object?>> tagList)
	{
		this = default(TagList);
		_tagsCount = tagList.Length;
		Span<KeyValuePair<string, object>> destination = ((_tagsCount <= 8) ? ((Span<KeyValuePair<string, object>>)_tags) : ((Span<KeyValuePair<string, object>>)(_overflowTags = new KeyValuePair<string, object>[_tagsCount + 8])));
		tagList.CopyTo(destination);
	}

	public void Add(string key, object? value)
	{
		Add(new KeyValuePair<string, object>(key, value));
	}

	public void Add(KeyValuePair<string, object?> tag)
	{
		int tagsCount = _tagsCount;
		if (_overflowTags == null && (uint)tagsCount < 8u)
		{
			_tags[tagsCount] = tag;
			_tagsCount++;
		}
		else
		{
			AddToOverflow(tag);
		}
	}

	private void AddToOverflow(KeyValuePair<string, object> tag)
	{
		if (_overflowTags == null)
		{
			_overflowTags = new KeyValuePair<string, object>[16];
			((ReadOnlySpan<KeyValuePair<string, object>>)_tags).CopyTo(_overflowTags);
		}
		else if (_tagsCount == _overflowTags.Length)
		{
			Array.Resize(ref _overflowTags, _tagsCount + 8);
		}
		_overflowTags[_tagsCount] = tag;
		_tagsCount++;
	}

	public readonly void CopyTo(Span<KeyValuePair<string, object?>> tags)
	{
		if (tags.Length < _tagsCount)
		{
			throw new ArgumentException(System.SR.Arg_BufferTooSmall);
		}
		Tags.CopyTo(tags);
	}

	public readonly void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)arrayIndex, (uint)array.Length, "arrayIndex");
		CopyTo(array.AsSpan(arrayIndex));
	}

	public void Insert(int index, KeyValuePair<string, object?> item)
	{
		if (index == _tagsCount)
		{
			Add(item);
			return;
		}
		ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)index, (uint)_tagsCount, "index");
		if (_tagsCount == 8 && _overflowTags == null)
		{
			_overflowTags = new KeyValuePair<string, object>[16];
			((ReadOnlySpan<KeyValuePair<string, object>>)_tags).CopyTo(_overflowTags);
		}
		if (_overflowTags != null)
		{
			if (_tagsCount == _overflowTags.Length)
			{
				Array.Resize(ref _overflowTags, _tagsCount + 8);
			}
			_overflowTags.AsSpan(index, _tagsCount - index).CopyTo(_overflowTags.AsSpan(index + 1));
			_overflowTags[index] = item;
		}
		else
		{
			Span<KeyValuePair<string, object>> span = _tags;
			span.Slice(index, _tagsCount - index).CopyTo(span.Slice(index + 1));
			span[index] = item;
		}
		_tagsCount++;
	}

	public void RemoveAt(int index)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)_tagsCount, "index");
		Span<KeyValuePair<string, object>> span = ((_overflowTags != null) ? ((Span<KeyValuePair<string, object>>)_overflowTags) : ((Span<KeyValuePair<string, object>>)_tags));
		span.Slice(index + 1, _tagsCount - index - 1).CopyTo(span.Slice(index));
		_tagsCount--;
	}

	public void Clear()
	{
		_tagsCount = 0;
	}

	public readonly bool Contains(KeyValuePair<string, object?> item)
	{
		return IndexOf(item) >= 0;
	}

	public bool Remove(KeyValuePair<string, object?> item)
	{
		int num = IndexOf(item);
		if (num >= 0)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	public readonly IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
	{
		return new Enumerator(this);
	}

	readonly IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}

	public readonly int IndexOf(KeyValuePair<string, object?> item)
	{
		ReadOnlySpan<KeyValuePair<string, object>> readOnlySpan = ((_overflowTags != null) ? ((ReadOnlySpan<KeyValuePair<string, object>>)_overflowTags) : ((ReadOnlySpan<KeyValuePair<string, object>>)_tags));
		readOnlySpan = readOnlySpan.Slice(0, _tagsCount);
		if (item.Value != null)
		{
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (item.Key == readOnlySpan[i].Key && item.Value.Equals(readOnlySpan[i].Value))
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < readOnlySpan.Length; j++)
			{
				if (item.Key == readOnlySpan[j].Key && readOnlySpan[j].Value == null)
				{
					return j;
				}
			}
		}
		return -1;
	}
}
