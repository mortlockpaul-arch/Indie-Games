using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal sealed class SymWithTypeMemo
{
	public Symbol sym;

	public AggregateType ats;
}
