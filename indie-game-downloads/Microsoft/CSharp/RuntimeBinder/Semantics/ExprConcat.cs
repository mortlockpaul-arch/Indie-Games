using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprConcat : ExprWithType
{
	public Expr FirstArgument { get; set; }

	public Expr SecondArgument { get; set; }

	public ExprConcat(Expr first, Expr second)
		: base(ExpressionKind.Concat, TypeFromOperands(first, second))
	{
		FirstArgument = first;
		SecondArgument = second;
	}

	private static CType TypeFromOperands(Expr first, Expr second)
	{
		CType type = first.Type;
		if (type.IsPredefType(PredefinedType.PT_STRING))
		{
			return type;
		}
		return second.Type;
	}
}
