using System.Diagnostics;

namespace System.Linq;

internal sealed class SystemLinq_LookupDebugView<TKey, TElement>
{
	private readonly ILookup<TKey, TElement> _lookup;

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public IGrouping<TKey, TElement>[] Groupings => _003CGroupings_003Ek__BackingField ?? (_003CGroupings_003Ek__BackingField = _lookup.ToArray());

	public SystemLinq_LookupDebugView(ILookup<TKey, TElement> lookup)
	{
		_lookup = lookup;
	}
}
