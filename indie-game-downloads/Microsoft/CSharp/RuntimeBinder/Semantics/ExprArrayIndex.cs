namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprArrayIndex : ExprWithType
{
	public Expr Array { get; set; }

	public Expr Index { get; set; }

	public ExprArrayIndex(CType type, Expr array, Expr index)
		: base(ExpressionKind.ArrayIndex, type)
	{
		Array = array;
		Index = index;
		base.Flags = EXPRFLAG.EXF_ASSGOP | EXPRFLAG.EXF_LVALUE;
	}
}
