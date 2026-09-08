using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ErrorArgBuilder : SimpleArgBuilder
{
	internal ErrorArgBuilder(Type parameterType)
		: base(parameterType)
	{
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		return Expression.Property(Helpers.Convert(base.Marshal(parameter), typeof(ErrorWrapper)), "ErrorCode");
	}

	internal override Expression UnmarshalFromRef(Expression value)
	{
		return base.UnmarshalFromRef((Expression)Expression.New(typeof(ErrorWrapper).GetConstructor(new Type[1] { typeof(int) }), value));
	}
}
