namespace System.Text.RegularExpressions.Symbolic;

[Flags]
internal enum StateFlags : byte
{
	None = 0,
	IsInitialFlag = 1,
	IsNullableFlag = 2,
	CanBeNullableFlag = 4,
	SimulatesBacktrackingFlag = 8
}
