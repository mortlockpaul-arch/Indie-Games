namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprBoundLambda : ExprWithType
{
	public Expr Expression { get; }

	public AggregateType DelegateType => base.Type as AggregateType;

	public Scope ArgumentScope { get; }

	public ExprBoundLambda(AggregateType type, Scope argumentScope, Expr expression)
		: base(ExpressionKind.BoundLambda, type)
	{
		ArgumentScope = argumentScope;
		Expression = expression;
	}
}
