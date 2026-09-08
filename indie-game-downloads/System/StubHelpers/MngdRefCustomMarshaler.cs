using System.Runtime.InteropServices;

namespace System.StubHelpers;

internal static class MngdRefCustomMarshaler
{
	internal unsafe static void ConvertContentsToNative(ICustomMarshaler marshaler, in object pManagedHome, nint* pNativeHome)
	{
		if (pManagedHome == null)
		{
			*pNativeHome = IntPtr.Zero;
		}
		else
		{
			*pNativeHome = marshaler.MarshalManagedToNative(pManagedHome);
		}
	}

	internal unsafe static void ConvertContentsToManaged(ICustomMarshaler marshaler, ref object pManagedHome, nint* pNativeHome)
	{
		if (*pNativeHome == IntPtr.Zero)
		{
			pManagedHome = null;
		}
		else
		{
			pManagedHome = marshaler.MarshalNativeToManaged(*pNativeHome);
		}
	}

	internal unsafe static void ClearNative(ICustomMarshaler marshaler, ref object pManagedHome, nint* pNativeHome)
	{
		if (*pNativeHome == IntPtr.Zero)
		{
			return;
		}
		try
		{
			marshaler.CleanUpNativeData(*pNativeHome);
		}
		catch
		{
		}
	}

	internal unsafe static void ClearManaged(ICustomMarshaler marshaler, in object pManagedHome, nint* pNativeHome)
	{
		if (pManagedHome != null)
		{
			marshaler.CleanUpManagedData(pManagedHome);
		}
	}
}
