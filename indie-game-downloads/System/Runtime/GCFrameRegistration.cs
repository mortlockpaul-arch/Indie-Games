using System.Runtime.CompilerServices;

namespace System.Runtime;

internal unsafe struct GCFrameRegistration(void** allocation, uint elemCount, bool areByRefs = true)
{
	private nuint _reserved1 = 0u;

	private nuint _reserved2 = 0u;

	private unsafe void** _pObjRefs = allocation;

	private uint _numObjRefs = elemCount;

	private int _maybeInterior = (areByRefs ? 1 : 0);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal unsafe static extern void RegisterForGCReporting(GCFrameRegistration* pRegistration);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal unsafe static extern void UnregisterForGCReporting(GCFrameRegistration* pRegistration);
}
