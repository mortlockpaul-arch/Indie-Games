using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class BoolArgBuilder : SimpleArgBuilder
{
	internal BoolArgBuilder(Type parameterType)
		: base(parameterType)
	{
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		return Expression.Condition(Marshal(parameter), Expression.Constant((short)(-1)), Expression.Constant((short)0));
	}

	internal override Expression UnmarshalFromRef(Expression value)
	{
		return base.UnmarshalFromRef((Expression)Expression.NotEqual(value, Expression.Constant((short)0)));
	}
}
