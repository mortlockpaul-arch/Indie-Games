using System;

namespace Quasar.Global;

[Flags]
public enum AppendNumberOptions
{
	None = 0,
	PositiveSign = 1,
	NumberGroup = 2,
	FixedSize = 4,
	EmptySpaces = 8
}
