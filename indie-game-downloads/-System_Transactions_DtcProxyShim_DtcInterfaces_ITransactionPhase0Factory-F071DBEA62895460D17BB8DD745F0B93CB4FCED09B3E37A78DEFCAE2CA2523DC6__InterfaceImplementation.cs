using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0Factory_003EF071DBEA62895460D17BB8DD745F0B93CB4FCED09B3E37A78DEFCAE2CA2523DC6__InterfaceImplementation : ITransactionPhase0Factory
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0Factory_003EF071DBEA62895460D17BB8DD745F0B93CB4FCED09B3E37A78DEFCAE2CA2523DC6__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0Factory.Create(ITransactionPhase0NotifyAsync pITransactionPhase0Notify, out ITransactionPhase0EnlistmentAsync ppITransactionPhase0Enlistment)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0Factory));
		delegate* unmanaged[MemberFunction]<void*, void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void*, void**, int>)ptr4[3];
		bool flag = false;
		ppITransactionPhase0Enlistment = null;
		void* ptr5 = default(void*);
		void* unmanaged = default(void*);
		try
		{
			ptr5 = ComInterfaceMarshaller<ITransactionPhase0NotifyAsync>.ConvertToUnmanaged(pITransactionPhase0Notify);
			Marshal.ThrowExceptionForHR(obj(ptr3, ptr5, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				224, 136, 220, 130, 84, 169, 209, 17, 143, 136,
				0, 96, 8, 149, 231, 213
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppITransactionPhase0Enlistment = ComInterfaceMarshaller<ITransactionPhase0EnlistmentAsync>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransactionPhase0EnlistmentAsync>.Free(unmanaged);
			}
			ComInterfaceMarshaller<ITransactionPhase0NotifyAsync>.Free(ptr5);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Create(ComWrappers.ComInterfaceDispatch* __this_native, void* __pITransactionPhase0Notify_native, void** __ppITransactionPhase0Enlistment_native__param)
	{
		ITransactionPhase0NotifyAsync transactionPhase0NotifyAsync = null;
		ref void* reference = ref *__ppITransactionPhase0Enlistment_native__param;
		ITransactionPhase0EnlistmentAsync ppITransactionPhase0Enlistment = null;
		int num = 0;
		try
		{
			transactionPhase0NotifyAsync = ComInterfaceMarshaller<ITransactionPhase0NotifyAsync>.ConvertToManaged(__pITransactionPhase0Notify_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0Factory>(__this_native).Create(transactionPhase0NotifyAsync, out ppITransactionPhase0Enlistment);
			num = 0;
			reference = ComInterfaceMarshaller<ITransactionPhase0EnlistmentAsync>.ConvertToUnmanaged(ppITransactionPhase0Enlistment);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0Factory_003EF071DBEA62895460D17BB8DD745F0B93CB4FCED09B3E37A78DEFCAE2CA2523DC6__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0Factory_003EF071DBEA62895460D17BB8DD745F0B93CB4FCED09B3E37A78DEFCAE2CA2523DC6__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0Factory_003EF071DBEA62895460D17BB8DD745F0B93CB4FCED09B3E37A78DEFCAE2CA2523DC6__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0Factory_003EF071DBEA62895460D17BB8DD745F0B93CB4FCED09B3E37A78DEFCAE2CA2523DC6__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Create_3 = &ABI_Create;
	}
}
