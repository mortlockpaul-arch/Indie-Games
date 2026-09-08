using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitterFactory_003EF7D13ED641A1EE082D83AE24874009EB53F59050742EDCDDBC98F89AC7F5C507C__InterfaceImplementation : ITransactionTransmitterFactory
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitterFactory_003EF7D13ED641A1EE082D83AE24874009EB53F59050742EDCDDBC98F89AC7F5C507C__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionTransmitterFactory.Create(out ITransactionTransmitter pTxTransmitter)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionTransmitterFactory));
		delegate* unmanaged[MemberFunction]<void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void**, int>)ptr4[3];
		bool flag = false;
		pTxTransmitter = null;
		void* unmanaged = default(void*);
		try
		{
			Marshal.ThrowExceptionForHR(obj(ptr3, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				0, 62, 49, 89, 108, 179, 207, 17, 165, 57,
				0, 170, 0, 104, 135, 195
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			pTxTransmitter = ComInterfaceMarshaller<ITransactionTransmitter>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransactionTransmitter>.Free(unmanaged);
			}
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Create(ComWrappers.ComInterfaceDispatch* __this_native, void** __pTxTransmitter_native__param)
	{
		ref void* reference = ref *__pTxTransmitter_native__param;
		ITransactionTransmitter pTxTransmitter = null;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionTransmitterFactory>(__this_native).Create(out pTxTransmitter);
			num = 0;
			reference = ComInterfaceMarshaller<ITransactionTransmitter>.ConvertToUnmanaged(pTxTransmitter);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitterFactory_003EF7D13ED641A1EE082D83AE24874009EB53F59050742EDCDDBC98F89AC7F5C507C__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitterFactory_003EF7D13ED641A1EE082D83AE24874009EB53F59050742EDCDDBC98F89AC7F5C507C__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitterFactory_003EF7D13ED641A1EE082D83AE24874009EB53F59050742EDCDDBC98F89AC7F5C507C__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitterFactory_003EF7D13ED641A1EE082D83AE24874009EB53F59050742EDCDDBC98F89AC7F5C507C__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Create_3 = &ABI_Create;
	}
}
