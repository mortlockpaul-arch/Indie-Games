namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprMulti : ExprWithType
{
	public Expr Left { get; set; }

	public Expr Operator { get; set; }

	public ExprMulti(CType type, EXPRFLAG flags, Expr left, Expr op)
		: base(ExpressionKind.Multi, type)
	{
		base.Flags = flags;
		Left = left;
		Operator = op;
	}
}
