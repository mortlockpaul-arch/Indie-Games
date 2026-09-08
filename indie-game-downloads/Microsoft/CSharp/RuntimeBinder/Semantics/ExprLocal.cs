namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprLocal : Expr
{
	public LocalVariableSymbol Local { get; }

	public ExprLocal(LocalVariableSymbol local)
		: base(ExpressionKind.Local)
	{
		base.Flags = EXPRFLAG.EXF_LVALUE;
		Local = local;
		base.Type = local?.GetType();
	}
}
