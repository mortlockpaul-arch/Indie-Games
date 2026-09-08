using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.StubHelpers;

internal static class MngdFixedArrayMarshaler
{
	private struct MarshalerState
	{
		internal nint m_pElementMT;

		internal nint m_pManagedElementMarshaler;

		internal nint m_Array;

		internal int m_BestFitMap;

		internal int m_ThrowOnUnmappableChar;

		internal ushort m_vt;

		internal uint m_cElements;
	}

	internal unsafe static void CreateMarshaler(nint pMarshalState, nint pMT, int dwFlags, int cElements, nint pManagedMarshaler)
	{
		((MarshalerState*)pMarshalState)->m_pElementMT = pMT;
		((MarshalerState*)pMarshalState)->m_Array = 0;
		((MarshalerState*)pMarshalState)->m_pManagedElementMarshaler = pManagedMarshaler;
		((MarshalerState*)pMarshalState)->m_BestFitMap = (byte)(dwFlags >> 16);
		((MarshalerState*)pMarshalState)->m_ThrowOnUnmappableChar = (byte)(dwFlags >> 24);
		((MarshalerState*)pMarshalState)->m_vt = (ushort)dwFlags;
		((MarshalerState*)pMarshalState)->m_cElements = (uint)cElements;
	}

	internal unsafe static void ConvertSpaceToNative(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		Array array = (Array)pManagedHome;
		if (array != null && (uint)array.Length < ((MarshalerState*)pMarshalState)->m_cElements)
		{
			throw new ArgumentException(SR.Argument_WrongSizeArrayInNativeStruct);
		}
	}

	internal static void ConvertContentsToNative(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertContentsToNative(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ConvertContentsToNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ConvertContentsToNative")]
	private static extern void ConvertContentsToNative(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ConvertSpaceToManaged(nint pMarshalState, ref object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertSpaceToManaged(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
		pManagedHome = o;
	}

	[DllImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ConvertSpaceToManaged", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ConvertSpaceToManaged")]
	private static extern void ConvertSpaceToManaged(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ConvertContentsToManaged(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		object o = pManagedHome;
		ConvertContentsToManaged(pMarshalState, ObjectHandleOnStack.Create(ref o), pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ConvertContentsToManaged", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ConvertContentsToManaged")]
	private static extern void ConvertContentsToManaged(nint pMarshalState, ObjectHandleOnStack pManagedHome, nint pNativeHome);

	internal static void ClearNativeContents(nint pMarshalState, in object pManagedHome, nint pNativeHome)
	{
		ClearNativeContents(pMarshalState, pNativeHome);
	}

	[DllImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ClearNativeContents", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MngdFixedArrayMarshaler_ClearNativeContents")]
	private static extern void ClearNativeContents(nint pMarshalState, nint pNativeHome);
}
