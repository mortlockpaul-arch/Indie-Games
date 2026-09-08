namespace System.Runtime.InteropServices;

internal static class IReferenceTracker
{
	public unsafe static void ConnectFromTrackerSource(nint pThis)
	{
		Marshal.ThrowExceptionForHR(((delegate* unmanaged<nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)pThis) + (nint)3 * (nint)sizeof(delegate* unmanaged<nint, int>))))(pThis));
	}

	public unsafe static void GetReferenceTrackerManager(nint pThis, out nint referenceTrackerManager)
	{
		fixed (nint* ptr = &referenceTrackerManager)
		{
			Marshal.ThrowExceptionForHR(((delegate* unmanaged<nint, nint*, int>)(*(IntPtr*)((nint)(*(IntPtr*)pThis) + (nint)6 * (nint)sizeof(delegate* unmanaged<nint, nint*, int>))))(pThis, ptr));
		}
	}

	public unsafe static void AddRefFromTrackerSource(nint pThis)
	{
		Marshal.ThrowExceptionForHR(((delegate* unmanaged<nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)pThis) + (nint)7 * (nint)sizeof(delegate* unmanaged<nint, int>))))(pThis));
	}

	public unsafe static void ReleaseFromTrackerSource(nint pThis)
	{
		Marshal.ThrowExceptionForHR(((delegate* unmanaged<nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)pThis) + (nint)8 * (nint)sizeof(delegate* unmanaged<nint, int>))))(pThis));
	}
}
