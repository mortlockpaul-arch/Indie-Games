using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.StubHelpers;

internal static class MngdNativeArrayMarshaler
{
	internal struct MarshalerState
	{
		internal nint m_pElementMT;

		internal TypeHandle m_Array;

		internal nint m_pManagedNativeArrayMarshaler;

		internal int m_NativeDataValid;

		internal int m_BestFitMap;

		internal int m_ThrowOnUnmappableChar;

		internal short m_vt;
	}

	internal unsafe static void CreateMarshaler(nint pMarshalState, nint pMT, int dwFlags, bool nativeDataValid, nint pManagedMarshaler)
	{
		((MarshalerState*)pMarshalState)->m_pElementMT = pMT;
		((MarshalerState*)pMarshalState)->m_Array = default(TypeHandle);
		((MarshalerState*)pMarshalState)->m_pManagedNativeArrayMarshaler = pManagedMarshaler;
		((MarshalerState*)pMarshalState)->m_NativeDataValid = (nativeDataValid ? 1 : 0);
		((MarshalerState*)pMarshalState)->m_BestFitMap = (byte)(dwFlags >> 16);
		((MarshalerState*)pMarshalState)->m_ThrowOnUnmappableChar = (byte)(dwFlags >> 24);
		((MarshalerState*)pMarshalState)->m_vt = (short)dwFlags;
	}

	internal static void ConvertSpaceToNative(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertSpaceToNative(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertSpaceToNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertSpaceToNative")]
	private static extern void ConvertSpaceToNative(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ConvertContentsToNative(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertContentsToNative(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertContentsToNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertContentsToNative")]
	private static extern void ConvertContentsToNative(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ConvertSpaceToManaged(nint pMarshalState, ref object pManagedHome, nint pNativeHome, int cElements)
	{
		object o = null;
		ConvertSpaceToManaged(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome, cElements);
		pManagedHome = o;
	}

	[DllImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertSpaceToManaged", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertSpaceToManaged")]
	private static extern void ConvertSpaceToManaged(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome, int cElements);

	internal static void ConvertContentsToManaged(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertContentsToManaged(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertContentsToManaged", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ConvertContentsToManaged")]
	private static extern void ConvertContentsToManaged(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal unsafe static void ClearNative(nint pMarshalState, nint pNativeHome, int cElements)
	{
		nint num = *(nint*)pNativeHome;
		if (num != IntPtr.Zero)
		{
			ClearNativeContents(pMarshalState, pNativeHome, cElements);
			Marshal.FreeCoTaskMem(num);
		}
	}

	[DllImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ClearNativeContents", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdNativeArrayMarshaler_ClearNativeContents")]
	internal static extern void ClearNativeContents(nint pMarshalState, nint pNativeHome, int cElements);
}
