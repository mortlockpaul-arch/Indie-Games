using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder.Errors;

internal sealed class ErrArgRefOnly : ErrArg
{
	public ErrArgRefOnly(Symbol sym)
		: base(sym)
	{
		eaf = ErrArgFlags.NoStr;
	}
}
