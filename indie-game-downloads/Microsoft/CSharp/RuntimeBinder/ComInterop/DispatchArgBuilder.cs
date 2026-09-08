using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class DispatchArgBuilder : SimpleArgBuilder
{
	private readonly bool _isWrapper;

	internal DispatchArgBuilder(Type parameterType)
		: base(parameterType)
	{
		_isWrapper = parameterType == typeof(DispatchWrapper);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		parameter = base.Marshal(parameter);
		if (_isWrapper)
		{
			parameter = Expression.Property(Helpers.Convert(parameter, typeof(DispatchWrapper)), typeof(DispatchWrapper).GetProperty("WrappedObject"));
		}
		return Helpers.Convert(parameter, typeof(object));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		parameter = Marshal(parameter);
		return Expression.Condition(Expression.Equal(parameter, Expression.Constant(null)), Expression.Constant(IntPtr.Zero), Expression.Call(typeof(Marshal).GetMethod("GetIDispatchForObject"), parameter));
	}

	internal override Expression UnmarshalFromRef(Expression value)
	{
		Expression expression = Expression.Condition(Expression.Equal(value, Expression.Constant(IntPtr.Zero)), Expression.Constant(null), Expression.Call(typeof(Marshal).GetMethod("GetObjectForIUnknown"), value));
		if (_isWrapper)
		{
			expression = Expression.New(typeof(DispatchWrapper).GetConstructor(new Type[1] { typeof(object) }), expression);
		}
		return base.UnmarshalFromRef(expression);
	}
}
