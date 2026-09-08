namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprWrap : Expr
{
	public Expr OptionalExpression { get; }

	public ExprWrap(Expr expression)
		: base(ExpressionKind.Wrap)
	{
		OptionalExpression = expression;
		base.Type = expression?.Type;
		base.Flags = EXPRFLAG.EXF_LVALUE;
	}
}
