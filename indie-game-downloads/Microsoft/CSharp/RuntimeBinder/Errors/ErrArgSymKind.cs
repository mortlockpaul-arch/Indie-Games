using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal sealed class ErrArgSymKind : ErrArg
{
	public ErrArgSymKind(Symbol sym)
	{
		eak = ErrArgKind.SymKind;
		eaf = ErrArgFlags.None;
		sk = sym.getKind();
	}
}
