using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class StringArgBuilder : SimpleArgBuilder
{
	private readonly bool _isWrapper;

	internal StringArgBuilder(Type parameterType)
		: base(parameterType)
	{
		_isWrapper = parameterType == typeof(BStrWrapper);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		parameter = base.Marshal(parameter);
		if (_isWrapper)
		{
			parameter = Expression.Property(Helpers.Convert(parameter, typeof(BStrWrapper)), typeof(BStrWrapper).GetProperty("WrappedObject"));
		}
		return parameter;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		parameter = Marshal(parameter);
		return Expression.Call(typeof(Marshal).GetMethod("StringToBSTR"), parameter);
	}

	internal override Expression UnmarshalFromRef(Expression value)
	{
		Expression expression = Expression.Condition(Expression.Equal(value, Expression.Constant(IntPtr.Zero)), Expression.Constant(null, typeof(string)), Expression.Call(typeof(Marshal).GetMethod("PtrToStringBSTR"), value));
		if (_isWrapper)
		{
			expression = Expression.New(typeof(BStrWrapper).GetConstructor(new Type[1] { typeof(string) }), expression);
		}
		return base.UnmarshalFromRef(expression);
	}
}
