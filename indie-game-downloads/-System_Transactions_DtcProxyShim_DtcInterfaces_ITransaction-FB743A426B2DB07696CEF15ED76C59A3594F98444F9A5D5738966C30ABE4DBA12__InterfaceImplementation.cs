using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementation : ITransaction
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransaction.Commit(bool fRetaining, OletxXacttc grfTC, uint grfRM)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransaction));
		delegate* unmanaged[MemberFunction]<void*, int, OletxXacttc, uint, int> obj = (delegate* unmanaged[MemberFunction]<void*, int, OletxXacttc, uint, int>)ptr4[3];
		int num = (fRetaining ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, num, grfTC, grfRM), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			132, 80, 177, 15, 65, 175, 206, 17, 189, 43,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransaction.Abort(nint reason, bool retaining, bool async)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransaction));
		delegate* unmanaged[MemberFunction]<void*, nint, int, int, int> obj = (delegate* unmanaged[MemberFunction]<void*, nint, int, int, int>)ptr4[4];
		int num = (async ? 1 : 0);
		int num2 = (retaining ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, reason, num2, num), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			132, 80, 177, 15, 65, 175, 206, 17, 189, 43,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransaction.GetTransactionInfo(out OletxXactTransInfo xactInfo)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransaction));
		delegate* unmanaged[MemberFunction]<void*, OletxXactTransInfo*, int> obj = (delegate* unmanaged[MemberFunction]<void*, OletxXactTransInfo*, int>)ptr4[5];
		xactInfo = default(OletxXactTransInfo);
		int errorCode;
		fixed (OletxXactTransInfo* ptr5 = &xactInfo)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			132, 80, 177, 15, 65, 175, 206, 17, 189, 43,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Commit(ComWrappers.ComInterfaceDispatch* __this_native, int __fRetaining_native, OletxXacttc grfTC, uint grfRM)
	{
		bool flag = false;
		int num = 0;
		try
		{
			flag = __fRetaining_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransaction>(__this_native).Commit(flag, grfTC, grfRM);
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
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransaction>(__this_native).Abort(reason, flag, flag2);
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
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransaction>(__this_native).GetTransactionInfo(out xactInfo);
			num = 0;
			reference = xactInfo;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Commit_3 = &ABI_Commit;
		Vtable.Abort_4 = &ABI_Abort;
		Vtable.GetTransactionInfo_5 = &ABI_GetTransactionInfo;
	}
}
