using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExprZeroInit : ExprWithType
{
	public override object Object
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return Activator.CreateInstance(base.Type.AssociatedSystemType);
		}
	}

	public ExprZeroInit(CType type)
		: base(ExpressionKind.ZeroInit, type)
	{
	}
}
