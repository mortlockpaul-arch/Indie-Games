using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum UnaOpMask
{
	None = 0,
	Plus = 1,
	Minus = 2,
	Tilde = 4,
	Bang = 8,
	IncDec = 0x10,
	Signed = Plus | Minus | Tilde,
	Unsigned = 5,
	Real = 3,
	Bool = Bang
}
