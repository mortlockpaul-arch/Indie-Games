using System.Collections.Generic;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class CacheDict<TKey, TValue>
{
	private struct KeyInfo
	{
		internal readonly TValue Value;

		internal readonly LinkedListNode<TKey> List;

		internal KeyInfo(TValue v, LinkedListNode<TKey> l)
		{
			this = default(KeyInfo);
			Value = v;
			List = l;
		}
	}

	private readonly Dictionary<TKey, KeyInfo> _dict;

	private readonly LinkedList<TKey> _list;

	private readonly int _maxSize;

	internal CacheDict(int maxSize)
	{
		_dict = new Dictionary<TKey, KeyInfo>();
		_list = new LinkedList<TKey>();
		_maxSize = maxSize;
	}

	internal void Add(TKey key, TValue value)
	{
		KeyInfo value2 = default(KeyInfo);
		if (_dict.TryGetValue(key, out value2))
		{
			_list.Remove(value2.List);
		}
		else if (_list.Count == _maxSize)
		{
			LinkedListNode<TKey> last = _list.Last;
			_list.RemoveLast();
			_dict.Remove(last.Value);
		}
		LinkedListNode<TKey> linkedListNode = new LinkedListNode<TKey>(key);
		_list.AddFirst(linkedListNode);
		_dict[key] = new KeyInfo(value, linkedListNode);
	}

	internal bool TryGetValue(TKey key, out TValue value)
	{
		KeyInfo value2 = default(KeyInfo);
		if (_dict.TryGetValue(key, out value2))
		{
			LinkedListNode<TKey> list = value2.List;
			if (list.Previous != null)
			{
				_list.Remove(list);
				_list.AddFirst(list);
			}
			value = value2.Value;
			return true;
		}
		value = default(TValue);
		return false;
	}
}
