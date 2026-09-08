using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("02656950-2152-11d0-944C-00A0C905416E")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementation>]
internal interface ITransactionCloner
{
	void Commit([MarshalAs(UnmanagedType.Bool)] bool fRetainingt, OletxXacttc grfTC, uint grfRM);

	void Abort(nint reason, [MarshalAs(UnmanagedType.Bool)] bool retaining, [MarshalAs(UnmanagedType.Bool)] bool async);

	void GetTransactionInfo(out OletxXactTransInfo xactInfo);

	void CloneWithCommitDisabled([MarshalAs(UnmanagedType.Interface)] out ITransaction ppITransaction);
}
