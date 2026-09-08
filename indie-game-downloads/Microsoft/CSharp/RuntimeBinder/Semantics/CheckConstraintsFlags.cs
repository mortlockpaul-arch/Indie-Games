using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum CheckConstraintsFlags
{
	None = 0,
	Outer = 1,
	NoErrors = 4
}
