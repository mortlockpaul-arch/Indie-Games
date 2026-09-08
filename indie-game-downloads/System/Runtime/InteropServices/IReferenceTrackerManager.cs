namespace System.Runtime.InteropServices;

internal static class IReferenceTrackerManager
{
	public unsafe static void SetReferenceTrackerHost(nint pThis, nint referenceTrackerHost)
	{
		Marshal.ThrowExceptionForHR(((delegate* unmanaged<nint, nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)pThis) + (nint)6 * (nint)sizeof(delegate* unmanaged<nint, nint, int>))))(pThis, referenceTrackerHost));
	}
}
