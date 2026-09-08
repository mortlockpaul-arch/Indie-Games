using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal class SimpleArgBuilder : ArgBuilder
{
	protected Type ParameterType { get; }

	internal SimpleArgBuilder(Type parameterType)
	{
		ParameterType = parameterType;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		return Helpers.Convert(parameter, ParameterType);
	}

	internal override Expression UnmarshalFromRef(Expression newValue)
	{
		return base.UnmarshalFromRef(newValue);
	}
}
