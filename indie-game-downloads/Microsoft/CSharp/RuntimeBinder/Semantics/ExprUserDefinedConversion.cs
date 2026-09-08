namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprUserDefinedConversion : Expr
{
	private Expr _userDefinedCall;

	public Expr Argument { get; set; }

	public Expr UserDefinedCall
	{
		get
		{
			return _userDefinedCall;
		}
		set
		{
			base.Type = (_userDefinedCall = value).Type;
		}
	}

	public MethWithInst UserDefinedCallMethod { get; }

	public ExprUserDefinedConversion(Expr argument, Expr call, MethWithInst method)
		: base(ExpressionKind.UserDefinedConversion)
	{
		Argument = argument;
		UserDefinedCall = call;
		UserDefinedCallMethod = method;
	}
}
