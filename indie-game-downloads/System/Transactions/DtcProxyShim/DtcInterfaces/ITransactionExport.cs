using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("0141fda5-8fc0-11ce-bd18-204c4f4f5020")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceImplementation>]
internal interface ITransactionExport
{
	void Export([MarshalAs(UnmanagedType.Interface)] ITransaction punkTransaction, out uint pcbTransactionCookie);

	void GetTransactionCookie([MarshalAs(UnmanagedType.Interface)] ITransaction pITransaction, uint cbTransactionCookie, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] rgbTransactionCookie, out uint pcbUsed);
}
