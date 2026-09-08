using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace System.Reflection;

internal sealed class LoaderAllocatorScout
{
	internal nint m_nativeLoaderAllocator;

	[LibraryImport("QCall", EntryPoint = "LoaderAllocator_Destroy")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool Destroy(nint nativeLoaderAllocator)
	{
		return __PInvoke(nativeLoaderAllocator) != 0;
		[DllImport("QCall", EntryPoint = "LoaderAllocator_Destroy", ExactSpelling = true)]
		static extern int __PInvoke(nint __nativeLoaderAllocator_native);
	}

	~LoaderAllocatorScout()
	{
		if (m_nativeLoaderAllocator != IntPtr.Zero && !Destroy(m_nativeLoaderAllocator))
		{
			GC.ReRegisterForFinalize(this);
		}
	}
}
