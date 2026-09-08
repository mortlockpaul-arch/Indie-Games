using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class VoidType : CType
{
	public static readonly VoidType Instance = new VoidType();

	private VoidType()
		: base(TypeKind.TK_VoidType)
	{
	}

	public override bool IsPredefType(PredefinedType pt)
	{
		return pt == PredefinedType.PT_VOID;
	}
}
