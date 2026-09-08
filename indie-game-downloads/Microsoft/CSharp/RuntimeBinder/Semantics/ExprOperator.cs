namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal abstract class ExprOperator : ExprWithType
{
	public Expr OptionalUserDefinedCall { get; }

	public MethWithInst PredefinedMethodToCall { get; set; }

	public MethPropWithInst UserDefinedCallMethod { get; set; }

	protected ExprOperator(ExpressionKind kind, CType type)
		: base(kind, type)
	{
	}

	protected ExprOperator(ExpressionKind kind, CType type, Expr call, MethPropWithInst userDefinedMethod)
		: this(kind, type)
	{
		OptionalUserDefinedCall = call;
		UserDefinedCallMethod = userDefinedMethod;
	}
}
