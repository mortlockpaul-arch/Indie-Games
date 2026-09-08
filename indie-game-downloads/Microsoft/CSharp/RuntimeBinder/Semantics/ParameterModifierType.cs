using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ParameterModifierType : CType
{
	public bool IsOut { get; }

	public CType ParameterType { get; }

	public override Type AssociatedSystemType
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return ParameterType.AssociatedSystemType.MakeByRefType();
		}
	}

	public override CType BaseOrParameterOrElementType => ParameterType;

	public ParameterModifierType(CType parameterType, bool isOut)
		: base(TypeKind.TK_ParameterModifierType)
	{
		ParameterType = parameterType;
		IsOut = isOut;
	}
}
