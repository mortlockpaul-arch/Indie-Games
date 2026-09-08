using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("82DC88E1-A954-11d1-8F88-00600895E7D5")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceImplementation>]
internal interface ITransactionPhase0EnlistmentAsync
{
	void Enable();

	void WaitForEnlistment();

	void Phase0Done();

	void Unenlist();

	void GetTransaction([MarshalAs(UnmanagedType.Interface)] out ITransaction ppITransaction);
}
