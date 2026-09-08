using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionEnlistmentAsync_003EF44716E76837B8A799F1AAF3871EF2F636FCB56F26148830D7E2D5321F73D34D3__InterfaceImplementation : ITransactionEnlistmentAsync
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionEnlistmentAsync_003EF44716E76837B8A799F1AAF3871EF2F636FCB56F26148830D7E2D5321F73D34D3__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionEnlistmentAsync.PrepareRequestDone(int hr, nint pmk, nint pboidReason)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionEnlistmentAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int, nint, nint, int>)ptr4[3])(ptr3, hr, pmk, pboidReason), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			129, 80, 177, 15, 65, 175, 206, 17, 189, 43,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionEnlistmentAsync.CommitRequestDone(int hr)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionEnlistmentAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int, int>)ptr4[4])(ptr3, hr), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			129, 80, 177, 15, 65, 175, 206, 17, 189, 43,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionEnlistmentAsync.AbortRequestDone(int hr)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionEnlistmentAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int, int>)ptr4[5])(ptr3, hr), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			129, 80, 177, 15, 65, 175, 206, 17, 189, 43,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_PrepareRequestDone(ComWrappers.ComInterfaceDispatch* __this_native, int hr, nint pmk, nint pboidReason)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionEnlistmentAsync>(__this_native).PrepareRequestDone(hr, pmk, pboidReason);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_CommitRequestDone(ComWrappers.ComInterfaceDispatch* __this_native, int hr)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionEnlistmentAsync>(__this_native).CommitRequestDone(hr);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_AbortRequestDone(ComWrappers.ComInterfaceDispatch* __this_native, int hr)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionEnlistmentAsync>(__this_native).AbortRequestDone(hr);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionEnlistmentAsync_003EF44716E76837B8A799F1AAF3871EF2F636FCB56F26148830D7E2D5321F73D34D3__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionEnlistmentAsync_003EF44716E76837B8A799F1AAF3871EF2F636FCB56F26148830D7E2D5321F73D34D3__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionEnlistmentAsync_003EF44716E76837B8A799F1AAF3871EF2F636FCB56F26148830D7E2D5321F73D34D3__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionEnlistmentAsync_003EF44716E76837B8A799F1AAF3871EF2F636FCB56F26148830D7E2D5321F73D34D3__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.PrepareRequestDone_3 = &ABI_PrepareRequestDone;
		Vtable.CommitRequestDone_4 = &ABI_CommitRequestDone;
		Vtable.AbortRequestDone_5 = &ABI_AbortRequestDone;
	}
}
