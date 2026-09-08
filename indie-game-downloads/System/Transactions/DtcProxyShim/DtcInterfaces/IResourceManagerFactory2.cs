using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("6B369C21-FBD2-11d1-8F47-00C04F8EE57D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceImplementation>]
internal interface IResourceManagerFactory2
{
	internal void Create(in Guid pguidRM, [MarshalAs(UnmanagedType.LPStr)] string pszRMName, [MarshalAs(UnmanagedType.Interface)] IResourceManagerSink pIResMgrSink, [MarshalAs(UnmanagedType.Interface)] out IResourceManager rm);

	internal void CreateEx(in Guid pguidRM, [MarshalAs(UnmanagedType.LPStr)] string pszRMName, [MarshalAs(UnmanagedType.Interface)] IResourceManagerSink pIResMgrSink, in Guid riidRequested, [MarshalAs(UnmanagedType.Interface)] out object rm);
}
