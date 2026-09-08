using System.Collections.Generic;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class CacheSet<T>
{
	private readonly Dictionary<T, LinkedListNode<T>> _dict;

	private readonly LinkedList<T> _list;

	private readonly int _maxSize;

	internal CacheSet(int maxSize)
	{
		_dict = new Dictionary<T, LinkedListNode<T>>();
		_list = new LinkedList<T>();
		_maxSize = maxSize;
	}

	internal T GetExistingOrAdd(T key)
	{
		lock (this)
		{
			LinkedListNode<T> value = null;
			if (_dict.TryGetValue(key, out value))
			{
				if (value.Previous != null)
				{
					_list.Remove(value);
					_list.AddFirst(value);
				}
				return value.Value;
			}
			if (_dict.Count == _maxSize)
			{
				_dict.Remove(_list.Last.Value);
				_list.RemoveLast();
			}
			value = new LinkedListNode<T>(key);
			_dict.Add(key, value);
			_list.AddFirst(value);
			return key;
		}
	}
}
