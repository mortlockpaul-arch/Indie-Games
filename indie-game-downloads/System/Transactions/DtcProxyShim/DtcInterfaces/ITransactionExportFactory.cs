using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("E1CF9B53-8745-11ce-A9BA-00AA006C3706")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceImplementation>]
internal interface ITransactionExportFactory
{
	void GetRemoteClassId(out Guid pclsid);

	void Create(uint cbWhereabouts, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] byte[] rgbWhereabouts, [MarshalAs(UnmanagedType.Interface)] out ITransactionExport ppExport);
}
