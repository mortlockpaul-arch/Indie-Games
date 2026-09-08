namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class MethodGroupType : CType
{
	public static readonly MethodGroupType Instance = new MethodGroupType();

	private MethodGroupType()
		: base(TypeKind.TK_MethodGroupType)
	{
	}
}
