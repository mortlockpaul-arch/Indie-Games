using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ConversionArgBuilder : ArgBuilder
{
	private readonly SimpleArgBuilder _innerBuilder;

	private readonly Type _parameterType;

	internal ConversionArgBuilder(Type parameterType, SimpleArgBuilder innerBuilder)
	{
		_parameterType = parameterType;
		_innerBuilder = innerBuilder;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		return _innerBuilder.Marshal(Helpers.Convert(parameter, _parameterType));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		throw new NotSupportedException();
	}
}
