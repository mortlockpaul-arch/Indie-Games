namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class FieldWithType : SymWithType
{
	public FieldWithType(FieldSymbol field, AggregateType ats)
	{
		Set(field, ats);
	}
}
