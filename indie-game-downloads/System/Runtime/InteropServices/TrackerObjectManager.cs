using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

internal static class TrackerObjectManager
{
	internal static readonly GCHandleSet s_referenceTrackerNativeObjectWrapperCache = new GCHandleSet();

	private static bool HasReferenceTrackerManager => HasReferenceTrackerManagerInternal();

	[LibraryImport("QCall", EntryPoint = "TrackerObjectManager_HasReferenceTrackerManager")]
	[SuppressGCTransition]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.U1)]
	private static bool HasReferenceTrackerManagerInternal()
	{
		return __PInvoke() != 0;
		[DllImport("QCall", EntryPoint = "TrackerObjectManager_HasReferenceTrackerManager", ExactSpelling = true)]
		[SuppressGCTransition]
		static extern byte __PInvoke();
	}

	[LibraryImport("QCall", EntryPoint = "TrackerObjectManager_TryRegisterReferenceTrackerManager")]
	[SuppressGCTransition]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.U1)]
	private static bool TryRegisterReferenceTrackerManager(nint referenceTrackerManager)
	{
		return __PInvoke(referenceTrackerManager) != 0;
		[DllImport("QCall", EntryPoint = "TrackerObjectManager_TryRegisterReferenceTrackerManager", ExactSpelling = true)]
		[SuppressGCTransition]
		static extern byte __PInvoke(nint __referenceTrackerManager_native);
	}

	private static void RegisterGCCallbacks()
	{
		GCHandleSet o = s_referenceTrackerNativeObjectWrapperCache;
		RegisterNativeObjectWrapperCache(ObjectHandleOnStack.Create(ref o));
	}

	[DllImport("QCall", EntryPoint = "TrackerObjectManager_RegisterNativeObjectWrapperCache", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "TrackerObjectManager_RegisterNativeObjectWrapperCache")]
	private static extern void RegisterNativeObjectWrapperCache(ObjectHandleOnStack nativeObjectWrapperCache);

	internal static void OnIReferenceTrackerFound(nint referenceTracker)
	{
		if (!HasReferenceTrackerManager)
		{
			IReferenceTracker.GetReferenceTrackerManager(referenceTracker, out var referenceTrackerManager);
			if (TryRegisterReferenceTrackerManager(referenceTrackerManager))
			{
				ReferenceTrackerHost.SetReferenceTrackerHost(referenceTrackerManager);
				RegisterGCCallbacks();
			}
			else
			{
				Marshal.Release(referenceTrackerManager);
			}
		}
	}

	internal static void AfterWrapperCreated(nint referenceTracker)
	{
		IReferenceTracker.ConnectFromTrackerSource(referenceTracker);
		IReferenceTracker.AddRefFromTrackerSource(referenceTracker);
		IReferenceTracker.AddRefFromTrackerSource(referenceTracker);
	}

	internal static void ReleaseExternalObjectsFromCurrentThread()
	{
		if (ComWrappers.GlobalInstanceForTrackerSupport == null)
		{
			throw new NotSupportedException(SR.InvalidOperation_ComInteropRequireComWrapperTrackerInstance);
		}
		nint contextToken = GetContextToken();
		List<ComWrappers.ReferenceTrackerNativeObjectWrapper> list = new List<ComWrappers.ReferenceTrackerNativeObjectWrapper>();
		List<object> list2 = new List<object>();
		using (s_referenceTrackerNativeObjectWrapperCache.ModificationLock.EnterScope())
		{
			foreach (GCHandle item in s_referenceTrackerNativeObjectWrapperCache)
			{
				ComWrappers.ReferenceTrackerNativeObjectWrapper referenceTrackerNativeObjectWrapper = Unsafe.As<ComWrappers.ReferenceTrackerNativeObjectWrapper>(item.Target);
				if (referenceTrackerNativeObjectWrapper == null || referenceTrackerNativeObjectWrapper._contextToken != contextToken)
				{
					continue;
				}
				if (referenceTrackerNativeObjectWrapper.ComWrappers == ComWrappers.GlobalInstanceForTrackerSupport)
				{
					list.Add(referenceTrackerNativeObjectWrapper);
					object target = referenceTrackerNativeObjectWrapper.ProxyHandle.Target;
					if (target != null)
					{
						list2.Add(target);
					}
				}
				referenceTrackerNativeObjectWrapper.DisconnectTracker();
			}
		}
		ComWrappers.GlobalInstanceForTrackerSupport.RemoveWrappersFromCache(list);
		ComWrappers.GlobalInstanceForTrackerSupport.ReleaseObjects(list2);
	}

	internal static nint GetContextToken()
	{
		Interop.Ole32.CoGetContextToken(out var pToken);
		return pToken;
	}
}
