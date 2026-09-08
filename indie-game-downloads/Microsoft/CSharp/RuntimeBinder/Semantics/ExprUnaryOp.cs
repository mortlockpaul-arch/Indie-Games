namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprUnaryOp : ExprOperator
{
	public Expr Child { get; set; }

	public ExprUnaryOp(ExpressionKind kind, CType type, Expr operand)
		: base(kind, type)
	{
		Child = operand;
	}

	public ExprUnaryOp(ExpressionKind kind, CType type, Expr operand, Expr call, MethPropWithInst userMethod)
		: base(kind, type, call, userMethod)
	{
		Child = operand;
	}
}
