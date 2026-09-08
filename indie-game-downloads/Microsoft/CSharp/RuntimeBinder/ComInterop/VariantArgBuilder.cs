using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class VariantArgBuilder : SimpleArgBuilder
{
	private readonly bool _isWrapper;

	internal VariantArgBuilder(Type parameterType)
		: base(parameterType)
	{
		_isWrapper = parameterType == typeof(VariantWrapper);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		parameter = base.Marshal(parameter);
		if (_isWrapper)
		{
			parameter = Expression.Property(Helpers.Convert(parameter, typeof(VariantWrapper)), typeof(VariantWrapper).GetProperty("WrappedObject"));
		}
		return Helpers.Convert(parameter, typeof(object));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		parameter = Marshal(parameter);
		return Expression.Call(typeof(UnsafeMethods).GetMethod("GetVariantForObject", BindingFlags.Static | BindingFlags.NonPublic), parameter);
	}

	internal override Expression UnmarshalFromRef(Expression value)
	{
		Expression expression = Expression.Call(typeof(UnsafeMethods).GetMethod("GetObjectForVariant"), value);
		if (_isWrapper)
		{
			expression = Expression.New(typeof(VariantWrapper).GetConstructor(new Type[1] { typeof(object) }), expression);
		}
		return base.UnmarshalFromRef(expression);
	}
}
