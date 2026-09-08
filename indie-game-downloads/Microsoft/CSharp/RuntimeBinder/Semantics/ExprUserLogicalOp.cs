namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprUserLogicalOp : ExprWithType
{
	public Expr TrueFalseCall { get; set; }

	public ExprCall OperatorCall { get; set; }

	public Expr FirstOperandToExamine { get; set; }

	public ExprUserLogicalOp(CType type, Expr trueFalseCall, ExprCall operatorCall)
		: base(ExpressionKind.UserLogicalOp, type)
	{
		base.Flags = EXPRFLAG.EXF_ASSGOP;
		TrueFalseCall = trueFalseCall;
		OperatorCall = operatorCall;
		Expr optionalElement = ((ExprList)operatorCall.OptionalArguments).OptionalElement;
		FirstOperandToExamine = ((optionalElement is ExprWrap exprWrap) ? exprWrap.OptionalExpression : optionalElement);
	}
}
