namespace System.Runtime.InteropServices.Marshalling;

[CLSCompliant(false)]
public unsafe readonly struct VirtualMethodTableInfo(void* thisPointer, void** virtualMethodTable)
{
	public unsafe void* ThisPointer { get; } = thisPointer;

	public unsafe void** VirtualMethodTable { get; } = virtualMethodTable;

	public unsafe void Deconstruct(out void* thisPointer, out void** virtualMethodTable)
	{
		thisPointer = ThisPointer;
		virtualMethodTable = VirtualMethodTable;
	}
}
