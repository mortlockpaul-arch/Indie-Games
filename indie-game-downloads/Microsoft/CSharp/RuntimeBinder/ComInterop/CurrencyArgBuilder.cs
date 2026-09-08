using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class CurrencyArgBuilder : SimpleArgBuilder
{
	internal CurrencyArgBuilder(Type parameterType)
		: base(parameterType)
	{
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		return Expression.Property(Helpers.Convert(base.Marshal(parameter), typeof(CurrencyWrapper)), "WrappedObject");
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		return Expression.Call(typeof(decimal).GetMethod("ToOACurrency"), Marshal(parameter));
	}

	internal override Expression UnmarshalFromRef(Expression value)
	{
		return base.UnmarshalFromRef((Expression)Expression.New(typeof(CurrencyWrapper).GetConstructor(new Type[1] { typeof(decimal) }), Expression.Call(typeof(decimal).GetMethod("FromOACurrency"), value)));
	}
}
