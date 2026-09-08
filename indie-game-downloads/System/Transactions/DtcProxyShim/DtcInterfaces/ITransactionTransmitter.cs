using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("59313E01-B36C-11cf-A539-00AA006887C3")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceImplementation>]
internal interface ITransactionTransmitter
{
	void Set([MarshalAs(UnmanagedType.Interface)] ITransaction transaction);

	void GetPropagationTokenSize(out uint pcbToken);

	void MarshalPropagationToken(uint cbToken, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] rgbToken, out uint pcbUsed);

	void UnmarshalReturnToken(uint cbReturnToken, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] rgbToken);

	void Reset();
}
