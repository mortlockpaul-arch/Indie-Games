namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class ExprWithArgs : ExprWithType
{
	public ExprMemberGroup MemberGroup { get; set; }

	public Expr OptionalArguments { get; set; }

	protected ExprWithArgs(ExpressionKind kind, CType type)
		: base(kind, type)
	{
	}

	public abstract SymWithType GetSymWithType();
}
