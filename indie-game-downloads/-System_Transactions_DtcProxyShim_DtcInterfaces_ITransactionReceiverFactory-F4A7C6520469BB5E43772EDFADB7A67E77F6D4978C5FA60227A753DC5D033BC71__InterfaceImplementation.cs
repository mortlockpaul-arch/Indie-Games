using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiverFactory_003EF4A7C6520469BB5E43772EDFADB7A67E77F6D4978C5FA60227A753DC5D033BC71__InterfaceImplementation : ITransactionReceiverFactory
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiverFactory_003EF4A7C6520469BB5E43772EDFADB7A67E77F6D4978C5FA60227A753DC5D033BC71__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionReceiverFactory.Create(out ITransactionReceiver pTxReceiver)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionReceiverFactory));
		delegate* unmanaged[MemberFunction]<void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void**, int>)ptr4[3];
		bool flag = false;
		pTxReceiver = null;
		void* unmanaged = default(void*);
		try
		{
			Marshal.ThrowExceptionForHR(obj(ptr3, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				2, 62, 49, 89, 108, 179, 207, 17, 165, 57,
				0, 170, 0, 104, 135, 195
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			pTxReceiver = ComInterfaceMarshaller<ITransactionReceiver>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransactionReceiver>.Free(unmanaged);
			}
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Create(ComWrappers.ComInterfaceDispatch* __this_native, void** __pTxReceiver_native__param)
	{
		ref void* reference = ref *__pTxReceiver_native__param;
		ITransactionReceiver pTxReceiver = null;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionReceiverFactory>(__this_native).Create(out pTxReceiver);
			num = 0;
			reference = ComInterfaceMarshaller<ITransactionReceiver>.ConvertToUnmanaged(pTxReceiver);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiverFactory_003EF4A7C6520469BB5E43772EDFADB7A67E77F6D4978C5FA60227A753DC5D033BC71__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiverFactory_003EF4A7C6520469BB5E43772EDFADB7A67E77F6D4978C5FA60227A753DC5D033BC71__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiverFactory_003EF4A7C6520469BB5E43772EDFADB7A67E77F6D4978C5FA60227A753DC5D033BC71__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiverFactory_003EF4A7C6520469BB5E43772EDFADB7A67E77F6D4978C5FA60227A753DC5D033BC71__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Create_3 = &ABI_Create;
	}
}
