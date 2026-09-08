using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00020400-0000-0000-C000-000000000046")]
internal interface IDispatch
{
	[PreserveSig]
	int TryGetTypeInfoCount(out uint pctinfo);

	[PreserveSig]
	int TryGetTypeInfo(uint iTInfo, int lcid, out nint info);

	[PreserveSig]
	int TryGetIDsOfNames(ref Guid iid, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)] string[] names, uint cNames, int lcid, [Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)] int[] rgDispId);

	[PreserveSig]
	int TryInvoke(int dispIdMember, ref Guid riid, int lcid, INVOKEKIND wFlags, ref DISPPARAMS pDispParams, nint VarResult, nint pExcepInfo, nint puArgErr);
}
