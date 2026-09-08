using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionDispenser_003EF4636BE28F486AD166242976B62D2137F68C9EF0A003FD5347E9743D9211D8E44__InterfaceImplementation : ITransactionDispenser
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionDispenser_003EF4636BE28F486AD166242976B62D2137F68C9EF0A003FD5347E9743D9211D8E44__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionDispenser.GetOptionsObject(out ITransactionOptions ppOptions)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionDispenser));
		delegate* unmanaged[MemberFunction]<void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void**, int>)ptr4[3];
		bool flag = false;
		ppOptions = null;
		void* unmanaged = default(void*);
		try
		{
			Marshal.ThrowExceptionForHR(obj(ptr3, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				225, 217, 106, 58, 185, 35, 207, 17, 173, 96,
				0, 170, 0, 167, 76, 205
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppOptions = ComInterfaceMarshaller<ITransactionOptions>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransactionOptions>.Free(unmanaged);
			}
		}
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionDispenser.BeginTransaction(nint punkOuter, OletxTransactionIsolationLevel isoLevel, OletxTransactionIsoFlags isoFlags, ITransactionOptions pOptions, out ITransaction ppTransaction)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionDispenser));
		delegate* unmanaged[MemberFunction]<void*, nint, OletxTransactionIsolationLevel, OletxTransactionIsoFlags, void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, nint, OletxTransactionIsolationLevel, OletxTransactionIsoFlags, void*, void**, int>)ptr4[4];
		bool flag = false;
		ppTransaction = null;
		void* ptr5 = default(void*);
		void* unmanaged = default(void*);
		try
		{
			ptr5 = ComInterfaceMarshaller<ITransactionOptions>.ConvertToUnmanaged(pOptions);
			Marshal.ThrowExceptionForHR(obj(ptr3, punkOuter, isoLevel, isoFlags, ptr5, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				225, 217, 106, 58, 185, 35, 207, 17, 173, 96,
				0, 170, 0, 167, 76, 205
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppTransaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransaction>.Free(unmanaged);
			}
			ComInterfaceMarshaller<ITransactionOptions>.Free(ptr5);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetOptionsObject(ComWrappers.ComInterfaceDispatch* __this_native, void** __ppOptions_native__param)
	{
		ref void* reference = ref *__ppOptions_native__param;
		ITransactionOptions ppOptions = null;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionDispenser>(__this_native).GetOptionsObject(out ppOptions);
			num = 0;
			reference = ComInterfaceMarshaller<ITransactionOptions>.ConvertToUnmanaged(ppOptions);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_BeginTransaction(ComWrappers.ComInterfaceDispatch* __this_native, nint punkOuter, OletxTransactionIsolationLevel isoLevel, OletxTransactionIsoFlags isoFlags, void* __pOptions_native, void** __ppTransaction_native__param)
	{
		ITransactionOptions transactionOptions = null;
		ref void* reference = ref *__ppTransaction_native__param;
		ITransaction ppTransaction = null;
		int num = 0;
		try
		{
			transactionOptions = ComInterfaceMarshaller<ITransactionOptions>.ConvertToManaged(__pOptions_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionDispenser>(__this_native).BeginTransaction(punkOuter, isoLevel, isoFlags, transactionOptions, out ppTransaction);
			num = 0;
			reference = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(ppTransaction);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionDispenser_003EF4636BE28F486AD166242976B62D2137F68C9EF0A003FD5347E9743D9211D8E44__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionDispenser_003EF4636BE28F486AD166242976B62D2137F68C9EF0A003FD5347E9743D9211D8E44__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionDispenser_003EF4636BE28F486AD166242976B62D2137F68C9EF0A003FD5347E9743D9211D8E44__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionDispenser_003EF4636BE28F486AD166242976B62D2137F68C9EF0A003FD5347E9743D9211D8E44__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.GetOptionsObject_3 = &ABI_GetOptionsObject;
		Vtable.BeginTransaction_4 = &ABI_BeginTransaction;
	}
}
