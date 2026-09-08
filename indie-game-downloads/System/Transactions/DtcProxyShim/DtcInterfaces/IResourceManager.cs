using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("13741D21-87EB-11CE-8081-0080C758527E")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementation>]
internal interface IResourceManager
{
	internal void Enlist([MarshalAs(UnmanagedType.Interface)] ITransaction pTransaction, [MarshalAs(UnmanagedType.Interface)] ITransactionResourceAsync pRes, out Guid pUOW, out OletxTransactionIsolationLevel pisoLevel, [MarshalAs(UnmanagedType.Interface)] out ITransactionEnlistmentAsync ppEnlist);

	internal void Reenlist([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pPrepInfo, uint cbPrepInfom, uint lTimeout, out OletxXactStat pXactStat);

	void ReenlistmentComplete();

	void GetDistributedTransactionManager(in Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
}
