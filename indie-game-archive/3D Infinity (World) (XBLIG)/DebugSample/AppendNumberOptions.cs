using System;

namespace DebugSample;

[Flags]
public enum AppendNumberOptions
{
	None = 0,
	PositiveSign = 1,
	NumberGroup = 2
}
