namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprClass : ExprWithType
{
	public ExprClass(CType type)
		: base(ExpressionKind.Class, type)
	{
	}
}
