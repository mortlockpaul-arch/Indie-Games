using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal abstract class ArgBuilder
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal abstract Expression Marshal(Expression parameter);

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal virtual Expression MarshalToRef(Expression parameter)
	{
		return Marshal(parameter);
	}

	internal virtual Expression UnmarshalFromRef(Expression newValue)
	{
		return newValue;
	}
}
