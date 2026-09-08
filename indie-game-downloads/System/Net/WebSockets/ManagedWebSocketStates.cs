namespace System.Net.WebSockets;

[Flags]
internal enum ManagedWebSocketStates
{
	None = 0,
	Open = 4,
	CloseSent = 8,
	CloseReceived = 0x10,
	Closed = 0x20,
	Aborted = 0x40,
	All = Open | CloseSent | CloseReceived | Closed | Aborted
}
