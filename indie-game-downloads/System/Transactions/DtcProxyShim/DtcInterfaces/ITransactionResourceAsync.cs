using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("69E971F0-23CE-11cf-AD60-00AA00A74CCD")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceImplementation>]
internal interface ITransactionResourceAsync
{
	void PrepareRequest([MarshalAs(UnmanagedType.Bool)] bool fRetaining, OletxXactRm grfRM, [MarshalAs(UnmanagedType.Bool)] bool fWantMoniker, [MarshalAs(UnmanagedType.Bool)] bool fSinglePhase);

	void CommitRequest(OletxXactRm grfRM, nint pNewUOW);

	void AbortRequest(nint pboidReason, [MarshalAs(UnmanagedType.Bool)] bool fRetaining, nint pNewUOW);

	void TMDown();
}
