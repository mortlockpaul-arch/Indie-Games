using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.StubHelpers;

internal static class InterfaceMarshaler
{
	internal static nint ConvertToNative(object objSrc, nint itfMT, nint classMT, int flags)
	{
		if (objSrc == null)
		{
			return IntPtr.Zero;
		}
		return ConvertToNative(ObjectHandleOnStack.Create(ref objSrc), itfMT, classMT, flags);
	}

	[DllImport("QCall", EntryPoint = "InterfaceMarshaler_ConvertToNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "InterfaceMarshaler_ConvertToNative")]
	private static extern nint ConvertToNative(ObjectHandleOnStack objSrc, nint itfMT, nint classMT, int flag);

	internal static object ConvertToManaged(ref nint ppUnk, nint itfMT, nint classMT, int flags)
	{
		if (ppUnk == IntPtr.Zero)
		{
			return null;
		}
		object o = null;
		ConvertToManaged(ref ppUnk, itfMT, classMT, flags, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[LibraryImport("QCall", EntryPoint = "InterfaceMarshaler_ConvertToManaged")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void ConvertToManaged(ref nint ppUnk, nint itfMT, nint classMT, int flags, ObjectHandleOnStack retObject)
	{
		fixed (nint* _ppUnk_native = &ppUnk)
		{
			__PInvoke(_ppUnk_native, itfMT, classMT, flags, retObject);
		}
		[DllImport("QCall", EntryPoint = "InterfaceMarshaler_ConvertToManaged", ExactSpelling = true)]
		unsafe static extern void __PInvoke(nint* __ppUnk_native, nint __itfMT_native, nint __classMT_native, int __flags_native, ObjectHandleOnStack __retObject_native);
	}

	internal static void ClearNative(nint pUnk)
	{
		if (pUnk != IntPtr.Zero)
		{
			Marshal.Release(pUnk);
		}
	}
}
