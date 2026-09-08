using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("0141fda4-8fc0-11ce-bd18-204c4f4f5020")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceImplementation>]
internal interface ITransactionImportWhereabouts
{
	internal void GetWhereaboutsSize(out uint pcbSize);

	internal void GetWhereabouts(uint cbWhereabouts, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] rgbWhereabouts, out uint pcbUsed);
}
