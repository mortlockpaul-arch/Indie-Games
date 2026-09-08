namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class LocalVariableSymbol : VariableSymbol
{
	public ExprWrap wrap;

	public void SetType(CType pType)
	{
		type = pType;
	}

	public new CType GetType()
	{
		return type;
	}
}
