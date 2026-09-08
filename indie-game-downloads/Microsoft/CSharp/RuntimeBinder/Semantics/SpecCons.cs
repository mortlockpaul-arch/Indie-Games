using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum SpecCons
{
	None = 0,
	New = 1,
	Ref = 2,
	Val = 4
}
