using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class ConvertibleArgBuilder : ArgBuilder
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression Marshal(Expression parameter)
	{
		return Helpers.Convert(parameter, typeof(IConvertible));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal override Expression MarshalToRef(Expression parameter)
	{
		throw new NotSupportedException();
	}
}
