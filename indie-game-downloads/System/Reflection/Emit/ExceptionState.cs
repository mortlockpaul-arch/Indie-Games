namespace System.Reflection.Emit;

internal enum ExceptionState
{
	Undefined,
	Try,
	Filter,
	Catch,
	Finally,
	Fault,
	Done
}
