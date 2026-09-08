using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal sealed class MethPropWithInstMemo
{
	public Symbol sym;

	public AggregateType ats;

	public TypeArray typeArgs;
}
