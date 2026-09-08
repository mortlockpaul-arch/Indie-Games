using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0NotifyAsync_003EF3181783D2441E21D5F072414C65E2998DA16BC2BBDDCF46567A43D2C9321E184__InterfaceImplementation : ITransactionPhase0NotifyAsync
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0NotifyAsync_003EF3181783D2441E21D5F072414C65E2998DA16BC2BBDDCF46567A43D2C9321E184__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0NotifyAsync.Phase0Request(bool fAbortHint)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0NotifyAsync));
		delegate* unmanaged[MemberFunction]<void*, int, int> obj = (delegate* unmanaged[MemberFunction]<void*, int, int>)ptr4[3];
		int num = (fAbortHint ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, num), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			9, 24, 8, 239, 118, 12, 210, 17, 135, 166,
			0, 192, 79, 153, 15, 52
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionPhase0NotifyAsync.EnlistCompleted(int status)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionPhase0NotifyAsync));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int, int>)ptr4[4])(ptr3, status), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			9, 24, 8, 239, 118, 12, 210, 17, 135, 166,
			0, 192, 79, 153, 15, 52
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Phase0Request(ComWrappers.ComInterfaceDispatch* __this_native, int __fAbortHint_native)
	{
		bool flag = false;
		int num = 0;
		try
		{
			flag = __fAbortHint_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0NotifyAsync>(__this_native).Phase0Request(flag);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_EnlistCompleted(ComWrappers.ComInterfaceDispatch* __this_native, int status)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionPhase0NotifyAsync>(__this_native).EnlistCompleted(status);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0NotifyAsync_003EF3181783D2441E21D5F072414C65E2998DA16BC2BBDDCF46567A43D2C9321E184__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0NotifyAsync_003EF3181783D2441E21D5F072414C65E2998DA16BC2BBDDCF46567A43D2C9321E184__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0NotifyAsync_003EF3181783D2441E21D5F072414C65E2998DA16BC2BBDDCF46567A43D2C9321E184__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionPhase0NotifyAsync_003EF3181783D2441E21D5F072414C65E2998DA16BC2BBDDCF46567A43D2C9321E184__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Phase0Request_3 = &ABI_Phase0Request;
		Vtable.EnlistCompleted_4 = &ABI_EnlistCompleted;
	}
}
