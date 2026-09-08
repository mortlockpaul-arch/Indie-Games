using System;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class Helpers
{
	internal static Expression Convert(Expression expression, Type type)
	{
		if (expression.Type == type)
		{
			return expression;
		}
		if (expression.Type == typeof(void))
		{
			return Expression.Block(expression, Expression.Default(type));
		}
		if (type == typeof(void))
		{
			return Expression.Block(expression, Expression.Empty());
		}
		return Expression.Convert(expression, type);
	}
}
