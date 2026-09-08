namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprCall : ExprWithArgs
{
	public MethWithInst MethWithInst { get; set; }

	public PREDEFMETH PredefinedMethod { get; set; } = PREDEFMETH.PM_COUNT;

	public NullableCallLiftKind NullableCallLiftKind { get; set; }

	public Expr PConversions { get; set; }

	public Expr CastOfNonLiftedResultToLiftedType { get; set; }

	public ExprCall(CType type, EXPRFLAG flags, Expr arguments, ExprMemberGroup member, MethWithInst method)
		: base(ExpressionKind.Call, type)
	{
		base.Flags = flags;
		base.OptionalArguments = arguments;
		base.MemberGroup = member;
		NullableCallLiftKind = NullableCallLiftKind.NotLifted;
		MethWithInst = method;
	}

	public override SymWithType GetSymWithType()
	{
		return MethWithInst;
	}
}
