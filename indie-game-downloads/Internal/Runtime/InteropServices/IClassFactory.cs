using System;
using System.Runtime.InteropServices;

namespace Internal.Runtime.InteropServices;

[ComImport]
[ComVisible(false)]
[Guid("00000001-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IClassFactory
{
	void CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, ref Guid riid, out nint ppvObject);

	void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
}
