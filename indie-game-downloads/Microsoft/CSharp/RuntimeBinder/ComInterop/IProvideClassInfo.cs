using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("B196B283-BAB4-101A-B69C-00AA00341D07")]
internal interface IProvideClassInfo
{
	void GetClassInfo(out nint info);
}
