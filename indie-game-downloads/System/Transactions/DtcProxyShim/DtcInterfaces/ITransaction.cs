using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("0fb15084-af41-11ce-bd2b-204c4f4f5020")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementation>]
internal interface ITransaction
{
	void Commit([MarshalAs(UnmanagedType.Bool)] bool fRetaining, OletxXacttc grfTC, uint grfRM);

	void Abort(nint reason, [MarshalAs(UnmanagedType.Bool)] bool retaining, [MarshalAs(UnmanagedType.Bool)] bool async);

	void GetTransactionInfo(out OletxXactTransInfo xactInfo);
}
