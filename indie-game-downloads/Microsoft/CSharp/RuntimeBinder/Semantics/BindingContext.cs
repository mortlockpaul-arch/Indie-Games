namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal readonly struct BindingContext
{
	public AggregateSymbol ContextForMemberLookup { get; }

	public bool Checked { get; }

	public BindingContext(AggregateSymbol context, bool isChecked)
	{
		ContextForMemberLookup = context;
		Checked = isChecked;
	}

	public BindingContext(BindingContext parent)
	{
		ContextForMemberLookup = parent.ContextForMemberLookup;
		Checked = false;
	}
}
