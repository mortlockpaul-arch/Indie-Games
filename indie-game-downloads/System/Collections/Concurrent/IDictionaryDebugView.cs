using System.Collections.Generic;
using System.Diagnostics;

namespace System.Collections.Concurrent;

internal sealed class IDictionaryDebugView<TKey, TValue>
{
	private readonly IDictionary<TKey, TValue> _dictionary;

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public System.Collections.Generic.DebugViewDictionaryItem<TKey, TValue>[] Items
	{
		get
		{
			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[_dictionary.Count];
			_dictionary.CopyTo(array, 0);
			System.Collections.Generic.DebugViewDictionaryItem<TKey, TValue>[] array2 = new System.Collections.Generic.DebugViewDictionaryItem<TKey, TValue>[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = new System.Collections.Generic.DebugViewDictionaryItem<TKey, TValue>(array[i]);
			}
			return array2;
		}
	}

	public IDictionaryDebugView(IDictionary<TKey, TValue> dictionary)
	{
		if (dictionary == null)
		{
			System.ThrowHelper.ThrowArgumentNullException("dictionary");
		}
		_dictionary = dictionary;
	}
}
