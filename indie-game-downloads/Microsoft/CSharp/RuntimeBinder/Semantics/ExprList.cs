namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprList : Expr
{
	public Expr OptionalElement { get; set; }

	public Expr OptionalNextListNode { get; set; }

	public ExprList(Expr optionalElement, Expr optionalNextListNode)
		: base(ExpressionKind.List)
	{
		OptionalElement = optionalElement;
		OptionalNextListNode = optionalNextListNode;
	}
}
