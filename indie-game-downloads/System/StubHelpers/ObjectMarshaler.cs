using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.StubHelpers;

internal static class ObjectMarshaler
{
	internal static void ConvertToNative(object objSrc, nint pDstVariant)
	{
		ConvertToNative(ObjectHandleOnStack.Create(ref objSrc), pDstVariant);
	}

	[DllImport("QCall", EntryPoint = "ObjectMarshaler_ConvertToNative", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ObjectMarshaler_ConvertToNative")]
	private static extern void ConvertToNative(ObjectHandleOnStack objSrc, nint pDstVariant);

	internal static object ConvertToManaged(nint pSrcVariant)
	{
		object o = null;
		ConvertToManaged(pSrcVariant, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "ObjectMarshaler_ConvertToManaged", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ObjectMarshaler_ConvertToManaged")]
	private static extern void ConvertToManaged(nint pSrcVariant, ObjectHandleOnStack retObject);

	internal unsafe static void ClearNative(nint pVariant)
	{
		if (pVariant != IntPtr.Zero)
		{
			Interop.OleAut32.VariantClear(pVariant);
			*(ComVariant*)pVariant = default(ComVariant);
		}
	}
}
