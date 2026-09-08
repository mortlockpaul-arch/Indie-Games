using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Interpreter;

internal readonly struct RuntimeLabel(int index, int continuationStackDepth, int stackDepth)
{
	public readonly int Index = index;

	public readonly int StackDepth = stackDepth;

	public readonly int ContinuationStackDepth = continuationStackDepth;

	public override string ToString()
	{
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(10, 3, invariantCulture);
		handler.AppendLiteral("->");
		handler.AppendFormatted(Index);
		handler.AppendLiteral(" C(");
		handler.AppendFormatted(ContinuationStackDepth);
		handler.AppendLiteral(") S(");
		handler.AppendFormatted(StackDepth);
		handler.AppendLiteral(")");
		return string.Create(invariantCulture, ref handler);
	}
}
