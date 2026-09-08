namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class ExpressionKindExtensions
{
	public static bool IsRelational(this ExpressionKind kind)
	{
		if (ExpressionKind.Eq <= kind)
		{
			return kind <= ExpressionKind.GreaterThanOrEqual;
		}
		return false;
	}

	public static bool IsUnaryOperator(this ExpressionKind kind)
	{
		switch (kind)
		{
		case ExpressionKind.True:
		case ExpressionKind.False:
		case ExpressionKind.Inc:
		case ExpressionKind.Dec:
		case ExpressionKind.LogicalNot:
		case ExpressionKind.Negate:
		case ExpressionKind.UnaryPlus:
		case ExpressionKind.BitwiseNot:
		case ExpressionKind.Addr:
		case ExpressionKind.DecimalNegate:
		case ExpressionKind.DecimalInc:
		case ExpressionKind.DecimalDec:
			return true;
		default:
			return false;
		}
	}
}
