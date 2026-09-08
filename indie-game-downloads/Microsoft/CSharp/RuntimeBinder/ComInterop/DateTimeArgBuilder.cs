using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class DateTimeArgBuilder : SimpleArgBuilder
{
	internal DateTimeArgBuilder(Type parameterType)
		: base(parameterType)
	{
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		return Expression.Call(Marshal(parameter), typeof(DateTime).GetMethod("ToOADate"));
	}

	internal override Expression UnmarshalFromRef(Expression value)
	{
		return base.UnmarshalFromRef((Expression)Expression.Call(typeof(DateTime).GetMethod("FromOADate"), value));
	}
}
