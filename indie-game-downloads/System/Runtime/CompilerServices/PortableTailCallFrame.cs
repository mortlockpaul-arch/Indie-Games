namespace System.Runtime.CompilerServices;

internal struct PortableTailCallFrame
{
	public nint TailCallAwareReturnAddress;

	public unsafe delegate*<TailCallArgBuffer*, ref byte, PortableTailCallFrame*, void> NextCall;
}
