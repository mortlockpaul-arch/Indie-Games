using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementation : ITransactionCloner
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionCloner.Commit(bool fRetainingt, OletxXacttc grfTC, uint grfRM)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionCloner));
		delegate* unmanaged[MemberFunction]<void*, int, OletxXacttc, uint, int> obj = (delegate* unmanaged[MemberFunction]<void*, int, OletxXacttc, uint, int>)ptr4[3];
		int num = (fRetainingt ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, num, grfTC, grfRM), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			80, 105, 101, 2, 82, 33, 208, 17, 148, 76,
			0, 160, 201, 5, 65, 110
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionCloner.Abort(nint reason, bool retaining, bool async)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionCloner));
		delegate* unmanaged[MemberFunction]<void*, nint, int, int, int> obj = (delegate* unmanaged[MemberFunction]<void*, nint, int, int, int>)ptr4[4];
		int num = (async ? 1 : 0);
		int num2 = (retaining ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, reason, num2, num), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			80, 105, 101, 2, 82, 33, 208, 17, 148, 76,
			0, 160, 201, 5, 65, 110
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionCloner.GetTransactionInfo(out OletxXactTransInfo xactInfo)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionCloner));
		delegate* unmanaged[MemberFunction]<void*, OletxXactTransInfo*, int> obj = (delegate* unmanaged[MemberFunction]<void*, OletxXactTransInfo*, int>)ptr4[5];
		xactInfo = default(OletxXactTransInfo);
		int errorCode;
		fixed (OletxXactTransInfo* ptr5 = &xactInfo)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			80, 105, 101, 2, 82, 33, 208, 17, 148, 76,
			0, 160, 201, 5, 65, 110
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionCloner.CloneWithCommitDisabled(out ITransaction ppITransaction)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionCloner));
		delegate* unmanaged[MemberFunction]<void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void**, int>)ptr4[6];
		bool flag = false;
		ppITransaction = null;
		void* unmanaged = default(void*);
		try
		{
			Marshal.ThrowExceptionForHR(obj(ptr3, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				80, 105, 101, 2, 82, 33, 208, 17, 148, 76,
				0, 160, 201, 5, 65, 110
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
	internal unsafe static int ABI_Commit(ComWrappers.ComInterfaceDispatch* __this_native, int __fRetainingt_native, OletxXacttc grfTC, uint grfRM)
	{
		bool flag = false;
		int num = 0;
		try
		{
			flag = __fRetainingt_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionCloner>(__this_native).Commit(flag, grfTC, grfRM);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Abort(ComWrappers.ComInterfaceDispatch* __this_native, nint reason, int __retaining_native, int __async_native)
	{
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		try
		{
			flag2 = __async_native != 0;
			flag = __retaining_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionCloner>(__this_native).Abort(reason, flag, flag2);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetTransactionInfo(ComWrappers.ComInterfaceDispatch* __this_native, OletxXactTransInfo* __xactInfo_native__param)
	{
		ref OletxXactTransInfo reference = ref *__xactInfo_native__param;
		OletxXactTransInfo xactInfo = default(OletxXactTransInfo);
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionCloner>(__this_native).GetTransactionInfo(out xactInfo);
			num = 0;
			reference = xactInfo;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_CloneWithCommitDisabled(ComWrappers.ComInterfaceDispatch* __this_native, void** __ppITransaction_native__param)
	{
		ref void* reference = ref *__ppITransaction_native__param;
		ITransaction ppITransaction = null;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionCloner>(__this_native).CloneWithCommitDisabled(out ppITransaction);
			num = 0;
			reference = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(ppITransaction);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Commit_3 = &ABI_Commit;
		Vtable.Abort_4 = &ABI_Abort;
		Vtable.GetTransactionInfo_5 = &ABI_GetTransactionInfo;
		Vtable.CloneWithCommitDisabled_6 = &ABI_CloneWithCommitDisabled;
	}
}
