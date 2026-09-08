using System;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

[Flags]
internal enum OpSigFlags
{
	None = 0,
	Convert = 1,
	CanLift = 2,
	AutoLift = 4,
	Value = Convert | CanLift | AutoLift,
	Reference = Convert,
	BoolBit = 3
}
