using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceImplementation : ITransactionResourceAsync
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionResourceAsync.PrepareRequest(bool fRetaining, OletxXactRm grfRM, bool fWantMoniker, bool fSinglePhase)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionResourceAsync));
		delegate* unmanaged[MemberFunction]<void*, int, OletxXactRm, int, int, int> obj = (delegate* unmanaged[MemberFunction]<void*, int, OletxXactRm, int, int, int>)ptr4[3];
		int num = (fSinglePhase ? 1 : 0);
		int num2 = (fWantMoniker ? 1 : 0);
		int num3 = (fRetaining ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, num3, grfRM, num2, num), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			240, 113, 233, 105, 206, 35, 207, 17, 173, 96,
			0, 170, 0, 167, 76, 205
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionResourceAsync.CommitRequest(OletxXactRm grfRM, nint pNewUOW)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionResourceAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, OletxXactRm, nint, int>)ptr4[4])(ptr3, grfRM, pNewUOW), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			240, 113, 233, 105, 206, 35, 207, 17, 173, 96,
			0, 170, 0, 167, 76, 205
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionResourceAsync.AbortRequest(nint pboidReason, bool fRetaining, nint pNewUOW)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionResourceAsync));
		delegate* unmanaged[MemberFunction]<void*, nint, int, nint, int> obj = (delegate* unmanaged[MemberFunction]<void*, nint, int, nint, int>)ptr4[5];
		int num = (fRetaining ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, pboidReason, num, pNewUOW), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			240, 113, 233, 105, 206, 35, 207, 17, 173, 96,
			0, 170, 0, 167, 76, 205
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionResourceAsync.TMDown()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionResourceAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[6])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			240, 113, 233, 105, 206, 35, 207, 17, 173, 96,
			0, 170, 0, 167, 76, 205
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_PrepareRequest(ComWrappers.ComInterfaceDispatch* __this_native, int __fRetaining_native, OletxXactRm grfRM, int __fWantMoniker_native, int __fSinglePhase_native)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		int num = 0;
		try
		{
			flag3 = __fSinglePhase_native != 0;
			flag2 = __fWantMoniker_native != 0;
			flag = __fRetaining_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionResourceAsync>(__this_native).PrepareRequest(flag, grfRM, flag2, flag3);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_CommitRequest(ComWrappers.ComInterfaceDispatch* __this_native, OletxXactRm grfRM, nint pNewUOW)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionResourceAsync>(__this_native).CommitRequest(grfRM, pNewUOW);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_AbortRequest(ComWrappers.ComInterfaceDispatch* __this_native, nint pboidReason, int __fRetaining_native, nint pNewUOW)
	{
		bool flag = false;
		int num = 0;
		try
		{
			flag = __fRetaining_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionResourceAsync>(__this_native).AbortRequest(pboidReason, flag, pNewUOW);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_TMDown(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionResourceAsync>(__this_native).TMDown();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionResourceAsync_003EF0E894968E6D9A39BAED572C9331E47780FD8171D5CA14F3B67AB7C77C86248AC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.PrepareRequest_3 = &ABI_PrepareRequest;
		Vtable.CommitRequest_4 = &ABI_CommitRequest;
		Vtable.AbortRequest_5 = &ABI_AbortRequest;
		Vtable.TMDown_6 = &ABI_TMDown;
	}
}
