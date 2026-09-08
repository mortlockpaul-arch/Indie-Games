using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum CONVERTTYPE
{
	NOUDC = 1,
	STANDARD = 2,
	ISEXPLICIT = 4,
	CHECKOVERFLOW = 8,
	FORCECAST = 0x10,
	STANDARDANDNOUDC = NOUDC | STANDARD
}
