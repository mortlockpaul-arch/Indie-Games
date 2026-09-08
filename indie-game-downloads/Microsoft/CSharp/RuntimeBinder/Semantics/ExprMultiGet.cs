namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprMultiGet : ExprWithType
{
	public ExprMulti OptionalMulti { get; set; }

	public ExprMultiGet(CType type, EXPRFLAG flags, ExprMulti multi)
		: base(ExpressionKind.MultiGet, type)
	{
		base.Flags = flags;
		OptionalMulti = multi;
	}
}
