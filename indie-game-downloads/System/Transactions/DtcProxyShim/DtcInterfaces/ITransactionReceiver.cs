using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("59313E03-B36C-11cf-A539-00AA006887C3")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceImplementation>]
internal interface ITransactionReceiver
{
	void UnmarshalPropagationToken(uint cbToken, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] rgbToken, [MarshalAs(UnmanagedType.Interface)] out ITransaction ppTransaction);

	void GetReturnTokenSize(out uint pcbReturnToken);

	void MarshalReturnToken(uint cbReturnToken, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] out byte[] rgbReturnToken, out uint pcbUsed);

	void Reset();
}
