using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ConvertArgBuilder : SimpleArgBuilder
{
	private readonly Type _marshalType;

	internal ConvertArgBuilder(Type parameterType, Type marshalType)
		: base(parameterType)
	{
		_marshalType = marshalType;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		parameter = base.Marshal(parameter);
		return Expression.Convert(parameter, _marshalType);
	}

	internal override Expression UnmarshalFromRef(Expression newValue)
	{
		return base.UnmarshalFromRef((Expression)Expression.Convert(newValue, base.ParameterType));
	}
}
