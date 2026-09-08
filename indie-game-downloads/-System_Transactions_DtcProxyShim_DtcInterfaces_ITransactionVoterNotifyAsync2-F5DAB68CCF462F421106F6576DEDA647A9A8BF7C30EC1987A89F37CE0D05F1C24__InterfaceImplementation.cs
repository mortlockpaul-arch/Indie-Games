using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementation : ITransactionVoterNotifyAsync2
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionVoterNotifyAsync2.Committed(bool fRetaining, nint pNewUOW, uint hresult)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionVoterNotifyAsync2));
		delegate* unmanaged[MemberFunction]<void*, int, nint, uint, int> obj = (delegate* unmanaged[MemberFunction]<void*, int, nint, uint, int>)ptr4[3];
		int num = (fRetaining ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, num, pNewUOW, hresult), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			107, 55, 51, 84, 77, 65, 211, 17, 178, 6,
			0, 192, 79, 194, 243, 239
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionVoterNotifyAsync2.Aborted(nint pboidReason, bool fRetaining, nint pNewUOW, uint hresult)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionVoterNotifyAsync2));
		delegate* unmanaged[MemberFunction]<void*, nint, int, nint, uint, int> obj = (delegate* unmanaged[MemberFunction]<void*, nint, int, nint, uint, int>)ptr4[4];
		int num = (fRetaining ? 1 : 0);
		Marshal.ThrowExceptionForHR(obj(ptr3, pboidReason, num, pNewUOW, hresult), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			107, 55, 51, 84, 77, 65, 211, 17, 178, 6,
			0, 192, 79, 194, 243, 239
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionVoterNotifyAsync2.HeuristicDecision(OletxTransactionHeuristic dwDecision, nint pboidReason, uint hresult)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionVoterNotifyAsync2));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, OletxTransactionHeuristic, nint, uint, int>)ptr4[5])(ptr3, dwDecision, pboidReason, hresult), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			107, 55, 51, 84, 77, 65, 211, 17, 178, 6,
			0, 192, 79, 194, 243, 239
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionVoterNotifyAsync2.Indoubt()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionVoterNotifyAsync2));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[6])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			107, 55, 51, 84, 77, 65, 211, 17, 178, 6,
			0, 192, 79, 194, 243, 239
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionVoterNotifyAsync2.VoteRequest()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionVoterNotifyAsync2));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[7])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			107, 55, 51, 84, 77, 65, 211, 17, 178, 6,
			0, 192, 79, 194, 243, 239
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Committed(ComWrappers.ComInterfaceDispatch* __this_native, int __fRetaining_native, nint pNewUOW, uint hresult)
	{
		bool flag = false;
		int num = 0;
		try
		{
			flag = __fRetaining_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionVoterNotifyAsync2>(__this_native).Committed(flag, pNewUOW, hresult);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Aborted(ComWrappers.ComInterfaceDispatch* __this_native, nint pboidReason, int __fRetaining_native, nint pNewUOW, uint hresult)
	{
		bool flag = false;
		int num = 0;
		try
		{
			flag = __fRetaining_native != 0;
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionVoterNotifyAsync2>(__this_native).Aborted(pboidReason, flag, pNewUOW, hresult);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_HeuristicDecision(ComWrappers.ComInterfaceDispatch* __this_native, OletxTransactionHeuristic dwDecision, nint pboidReason, uint hresult)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionVoterNotifyAsync2>(__this_native).HeuristicDecision(dwDecision, pboidReason, hresult);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Indoubt(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionVoterNotifyAsync2>(__this_native).Indoubt();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_VoteRequest(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionVoterNotifyAsync2>(__this_native).VoteRequest();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Committed_3 = &ABI_Committed;
		Vtable.Aborted_4 = &ABI_Aborted;
		Vtable.HeuristicDecision_5 = &ABI_HeuristicDecision;
		Vtable.Indoubt_6 = &ABI_Indoubt;
		Vtable.VoteRequest_7 = &ABI_VoteRequest;
	}
}
