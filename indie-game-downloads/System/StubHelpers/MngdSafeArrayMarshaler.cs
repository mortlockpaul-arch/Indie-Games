using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.StubHelpers;

internal static class MngdSafeArrayMarshaler
{
	[DllImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_CreateMarshaler", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_CreateMarshaler")]
	[SuppressGCTransition]
	internal static extern void CreateMarshaler(nint pMarshalState, nint pMT, int iRank, int dwFlags, nint pManagedMarshaler);

	internal static void ConvertSpaceToNative(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertSpaceToNative(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertSpaceToNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertSpaceToNative")]
	private static extern void ConvertSpaceToNative(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ConvertContentsToNative(nint pMarshalState, in object pManagedHome, nint pNativeHome, object pOriginalManagedObject)
	{
		object o = pManagedHome;
		object o2 = pOriginalManagedObject;
		ConvertContentsToNative(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome, ObjectHandleOnStack.Create(ref o2));
	}

	[DllImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertContentsToNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertContentsToNative")]
	private static extern void ConvertContentsToNative(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome, ObjectHandleOnStack pOriginalManagedObject);

	internal static void ConvertSpaceToManaged(nint pMarshalState, ref object pManagedHome, nint pNativeHome)
	{
		object o = null;
		ConvertSpaceToManaged(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
		pManagedHome = o;
	}

	[DllImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertSpaceToManaged", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertSpaceToManaged")]
	private static extern void ConvertSpaceToManaged(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ConvertContentsToManaged(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertContentsToManaged(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertContentsToManaged", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ConvertContentsToManaged")]
	private static extern void ConvertContentsToManaged(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ClearNative(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		ClearNative(pMarshalState, pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ClearNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdSafeArrayMarshaler_ClearNative")]
	private static extern void ClearNative(nint pMarshalState, nint pNativeHome);
}
