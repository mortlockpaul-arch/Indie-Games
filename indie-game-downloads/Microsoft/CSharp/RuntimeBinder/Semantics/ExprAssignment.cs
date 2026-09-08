namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprAssignment : Expr
{
	private Expr _lhs;

	public Expr LHS
	{
		get
		{
			return _lhs;
		}
		set
		{
			base.Type = (_lhs = value).Type;
		}
	}

	public Expr RHS { get; set; }

	public ExprAssignment(Expr lhs, Expr rhs)
		: base(ExpressionKind.Assignment)
	{
		LHS = lhs;
		RHS = rhs;
		base.Flags = EXPRFLAG.EXF_ASSGOP;
	}
}
