using System.Diagnostics;

namespace System.Linq;

internal sealed class SystemLinq_GroupingDebugView<TKey, TElement>
{
	private readonly Grouping<TKey, TElement> _grouping;

	public TKey Key => _grouping.Key;

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public TElement[] Values => _003CValues_003Ek__BackingField ?? (_003CValues_003Ek__BackingField = _grouping.ToArray());

	public SystemLinq_GroupingDebugView(Grouping<TKey, TElement> grouping)
	{
		_grouping = grouping;
	}
}
