namespace System.Runtime.CompilerServices;

internal struct MethodDescChunk
{
	public unsafe MethodTable* MethodTable;

	public unsafe MethodDescChunk* Next;

	public byte Size;

	public byte Count;

	public ushort FlagsAndTokenRange;
}
