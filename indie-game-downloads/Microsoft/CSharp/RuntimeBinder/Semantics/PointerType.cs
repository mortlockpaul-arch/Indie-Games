using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class PointerType : CType
{
	public CType ReferentType { get; }

	public override Type AssociatedSystemType
	{
		[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
		get
		{
			return ReferentType.AssociatedSystemType.MakePointerType();
		}
	}

	public override CType BaseOrParameterOrElementType => ReferentType;

	public override FUNDTYPE FundamentalType => FUNDTYPE.FT_PTR;

	[ExcludeFromCodeCoverage(Justification = "Dynamic code can't contain constant pointers")]
	public override ConstValKind ConstValKind => ConstValKind.IntPtr;

	public PointerType(CType referentType)
		: base(TypeKind.TK_PointerType)
	{
		ReferentType = referentType;
	}

	public override bool IsUnsafe()
	{
		return true;
	}
}
