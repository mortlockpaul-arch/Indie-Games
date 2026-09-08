namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class NullType : CType
{
	public static readonly NullType Instance = new NullType();

	public override bool IsReferenceType => true;

	public override FUNDTYPE FundamentalType => FUNDTYPE.FT_REF;

	public override ConstValKind ConstValKind => ConstValKind.IntPtr;

	private NullType()
		: base(TypeKind.TK_NullType)
	{
	}
}
