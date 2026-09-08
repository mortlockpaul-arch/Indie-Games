using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Collections.Immutable;

internal class ImmutableDictionaryDebuggerProxy<TKey, TValue>
{
	private readonly IReadOnlyDictionary<TKey, TValue> _dictionary;

	private System.Collections.Generic.DebugViewDictionaryItem<TKey, TValue>[] _cachedContents;

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public System.Collections.Generic.DebugViewDictionaryItem<TKey, TValue>[] Contents => _cachedContents ?? (_cachedContents = _dictionary.Select((KeyValuePair<TKey, TValue> kv) => new System.Collections.Generic.DebugViewDictionaryItem<TKey, TValue>(kv)).ToArray(_dictionary.Count));

	public ImmutableDictionaryDebuggerProxy(IReadOnlyDictionary<TKey, TValue> dictionary)
	{
		Requires.NotNull(dictionary, "dictionary");
		_dictionary = dictionary;
	}
}
