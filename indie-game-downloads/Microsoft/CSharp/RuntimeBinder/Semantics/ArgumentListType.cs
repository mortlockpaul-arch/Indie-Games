namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ArgumentListType : CType
{
	public static readonly ArgumentListType Instance = new ArgumentListType();

	private ArgumentListType()
		: base(TypeKind.TK_ArgumentListType)
	{
	}
}
