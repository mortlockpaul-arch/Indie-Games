using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceImplementation : ITransactionPhase0EnlistmentAsync
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0EnlistmentAsync.Enable()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0EnlistmentAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[3])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			225, 136, 220, 130, 84, 169, 209, 17, 143, 136,
			0, 96, 8, 149, 231, 213
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0EnlistmentAsync.WaitForEnlistment()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0EnlistmentAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[4])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			225, 136, 220, 130, 84, 169, 209, 17, 143, 136,
			0, 96, 8, 149, 231, 213
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0EnlistmentAsync.Phase0Done()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0EnlistmentAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[5])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			225, 136, 220, 130, 84, 169, 209, 17, 143, 136,
			0, 96, 8, 149, 231, 213
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0EnlistmentAsync.Unenlist()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0EnlistmentAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[6])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			225, 136, 220, 130, 84, 169, 209, 17, 143, 136,
			0, 96, 8, 149, 231, 213
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0EnlistmentAsync.GetTransaction(out ITransaction ppITransaction)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0EnlistmentAsync));
		delegate* unmanaged[MemberFunction]<void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void**, int>)ptr4[7];
		bool flag = false;
		ppITransaction = null;
		void* unmanaged = default(void*);
		try
		{
			Marshal.ThrowExceptionForHR(obj(ptr3, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				225, 136, 220, 130, 84, 169, 209, 17, 143, 136,
				0, 96, 8, 149, 231, 213
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppITransaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransaction>.Free(unmanaged);
			}
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Enable(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0EnlistmentAsync>(__this_native).Enable();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_WaitForEnlistment(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0EnlistmentAsync>(__this_native).WaitForEnlistment();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Phase0Done(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0EnlistmentAsync>(__this_native).Phase0Done();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Unenlist(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0EnlistmentAsync>(__this_native).Unenlist();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetTransaction(ComWrappers.ComInterfaceDispatch* __this_native, void** __ppITransaction_native__param)
	{
		ref void* reference = ref *__ppITransaction_native__param;
		ITransaction ppITransaction = null;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0EnlistmentAsync>(__this_native).GetTransaction(out ppITransaction);
			num = 0;
			reference = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(ppITransaction);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0EnlistmentAsync_003EF82289DF62783F631BA86FBB8D750C46E86F5F42012F184218D2853BF0873A2B2__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Enable_3 = &ABI_Enable;
		Vtable.WaitForEnlistment_4 = &ABI_WaitForEnlistment;
		Vtable.Phase0Done_5 = &ABI_Phase0Done;
		Vtable.Unenlist_6 = &ABI_Unenlist;
		Vtable.GetTransaction_7 = &ABI_GetTransaction;
	}
}
