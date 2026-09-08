using System.Diagnostics;

namespace System.Runtime.CompilerServices;

internal struct MethodDesc
{
	public ushort Flags3AndTokenRemainder;

	public byte ChunkIndex;

	public byte Flags4;

	public ushort SlotNumber;

	public ushort Flags;

	public nint CodeData;

	public unsafe MethodTable* MethodTable => GetMethodDescChunk()->MethodTable;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[DebuggerHidden]
	[DebuggerStepThrough]
	private unsafe MethodDescChunk* GetMethodDescChunk()
	{
		return (MethodDescChunk*)((byte*)Unsafe.AsPointer<MethodDesc>(this) - (sizeof(MethodDescChunk) + ChunkIndex * 8));
	}
}
