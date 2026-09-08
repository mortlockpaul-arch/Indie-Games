using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

internal static class ReferenceTrackerHost
{
	private struct IReferenceTrackerHostVftbl
	{
		public unsafe delegate* unmanaged[MemberFunction]<nint, Guid*, nint*, int> QueryInterface;

		public unsafe delegate* unmanaged[MemberFunction]<nint, uint> AddRef;

		public unsafe delegate* unmanaged[MemberFunction]<nint, uint> Release;

		public unsafe delegate* unmanaged[MemberFunction]<nint, uint, int> DisconnectUnusedReferenceSources;

		public unsafe delegate* unmanaged[MemberFunction]<nint, int> ReleaseDisconnectedReferenceSources;

		public unsafe delegate* unmanaged[MemberFunction]<nint, int> NotifyEndOfReferenceTrackingOnThread;

		public unsafe delegate* unmanaged[MemberFunction]<nint, nint, nint*, int> GetTrackerTarget;

		public unsafe delegate* unmanaged[MemberFunction]<nint, long, int> AddMemoryPressure;

		public unsafe delegate* unmanaged[MemberFunction]<nint, long, int> RemoveMemoryPressure;
	}

	private static class HostServices
	{
		[FixedAddressValueType]
		public static readonly IReferenceTrackerHostVftbl Vftbl;

		unsafe static HostServices()
		{
			Vftbl.QueryInterface = &IReferenceTrackerHost_QueryInterface;
			ComWrappers.GetUntrackedIUnknownImpl(out Vftbl.AddRef, out Vftbl.Release);
			Vftbl.DisconnectUnusedReferenceSources = &IReferenceTrackerHost_DisconnectUnusedReferenceSources;
			Vftbl.ReleaseDisconnectedReferenceSources = &IReferenceTrackerHost_ReleaseDisconnectedReferenceSources;
			Vftbl.NotifyEndOfReferenceTrackingOnThread = &IReferenceTrackerHost_NotifyEndOfReferenceTrackingOnThread;
			Vftbl.GetTrackerTarget = &IReferenceTrackerHost_GetTrackerTarget;
			Vftbl.AddMemoryPressure = &IReferenceTrackerHost_AddMemoryPressure;
			Vftbl.RemoveMemoryPressure = &IReferenceTrackerHost_RemoveMemoryPressure;
		}
	}

	[FixedAddressValueType]
	private unsafe static readonly nint s_globalHostServices = (nint)Unsafe.AsPointer(in HostServices.Vftbl);

	public unsafe static void SetReferenceTrackerHost(nint trackerManager)
	{
		IReferenceTrackerManager.SetReferenceTrackerHost(trackerManager, (nint)Unsafe.AsPointer(in s_globalHostServices));
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal static int IReferenceTrackerHost_DisconnectUnusedReferenceSources(nint pThis, uint flags)
	{
		try
		{
			if ((flags & 1) != 0)
			{
				GC.Collect(2, GCCollectionMode.Optimized, blocking: true, compacting: false, lowMemoryPressure: true);
			}
			else
			{
				GC.Collect();
			}
			return 0;
		}
		catch (Exception e)
		{
			return Marshal.GetHRForException(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal static int IReferenceTrackerHost_ReleaseDisconnectedReferenceSources(nint pThis)
	{
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal static int IReferenceTrackerHost_NotifyEndOfReferenceTrackingOnThread(nint pThis)
	{
		try
		{
			TrackerObjectManager.ReleaseExternalObjectsFromCurrentThread();
			return 0;
		}
		catch (Exception e)
		{
			return Marshal.GetHRForException(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int IReferenceTrackerHost_GetTrackerTarget(nint pThis, nint punk, nint* ppNewReference)
	{
		if (punk == IntPtr.Zero)
		{
			return -2147024809;
		}
		if (Marshal.QueryInterface(punk, in ComWrappers.IID_IUnknown, out var ppv) != 0)
		{
			return -2147467262;
		}
		try
		{
			using ComHolder comHolder = new ComHolder(ppv);
			using ComHolder comHolder2 = new ComHolder(ComWrappers.GetOrCreateTrackerTarget(comHolder.Ptr));
			return Marshal.QueryInterface(comHolder2.Ptr, in ComWrappers.IID_IReferenceTrackerTarget, out *ppNewReference);
		}
		catch (Exception e)
		{
			return Marshal.GetHRForException(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal static int IReferenceTrackerHost_AddMemoryPressure(nint pThis, long bytesAllocated)
	{
		try
		{
			GC.AddMemoryPressure(bytesAllocated);
			return 0;
		}
		catch (Exception e)
		{
			return Marshal.GetHRForException(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal static int IReferenceTrackerHost_RemoveMemoryPressure(nint pThis, long bytesAllocated)
	{
		try
		{
			GC.RemoveMemoryPressure(bytesAllocated);
			return 0;
		}
		catch (Exception e)
		{
			return Marshal.GetHRForException(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int IReferenceTrackerHost_QueryInterface(nint pThis, Guid* guid, nint* ppObject)
	{
		if (*guid == ComWrappers.IID_IReferenceTrackerHost || *guid == ComWrappers.IID_IUnknown)
		{
			*ppObject = pThis;
			Marshal.AddRef(pThis);
			return 0;
		}
		return -2147467262;
	}
}
