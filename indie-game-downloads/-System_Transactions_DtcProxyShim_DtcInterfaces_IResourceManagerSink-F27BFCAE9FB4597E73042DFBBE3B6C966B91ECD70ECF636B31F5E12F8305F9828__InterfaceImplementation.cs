using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerSink_003EF27BFCAE9FB4597E73042DFBBE3B6C966B91ECD70ECF636B31F5E12F8305F9828__InterfaceImplementation : IResourceManagerSink
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerSink_003EF27BFCAE9FB4597E73042DFBBE3B6C966B91ECD70ECF636B31F5E12F8305F9828__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IResourceManagerSink.TMDown()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IResourceManagerSink));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[3])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			129, 49, 86, 13, 251, 222, 206, 17, 174, 209,
			0, 170, 0, 81, 226, 196
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_TMDown(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<IResourceManagerSink>(__this_native).TMDown();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerSink_003EF27BFCAE9FB4597E73042DFBBE3B6C966B91ECD70ECF636B31F5E12F8305F9828__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerSink_003EF27BFCAE9FB4597E73042DFBBE3B6C966B91ECD70ECF636B31F5E12F8305F9828__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerSink_003EF27BFCAE9FB4597E73042DFBBE3B6C966B91ECD70ECF636B31F5E12F8305F9828__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerSink_003EF27BFCAE9FB4597E73042DFBBE3B6C966B91ECD70ECF636B31F5E12F8305F9828__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.TMDown_3 = &ABI_TMDown;
	}
}
