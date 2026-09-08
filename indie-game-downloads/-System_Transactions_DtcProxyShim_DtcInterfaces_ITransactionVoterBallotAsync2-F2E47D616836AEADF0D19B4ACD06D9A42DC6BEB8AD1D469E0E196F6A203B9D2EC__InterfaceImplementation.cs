using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterBallotAsync2_003EF2E47D616836AEADF0D19B4ACD06D9A42DC6BEB8AD1D469E0E196F6A203B9D2EC__InterfaceImplementation : ITransactionVoterBallotAsync2
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterBallotAsync2_003EF2E47D616836AEADF0D19B4ACD06D9A42DC6BEB8AD1D469E0E196F6A203B9D2EC__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionVoterBallotAsync2.VoteRequestDone(int hr, nint pboidReason)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionVoterBallotAsync2));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int, nint, int>)ptr4[3])(ptr3, hr, pboidReason), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			108, 55, 51, 84, 77, 65, 211, 17, 178, 6,
			0, 192, 79, 194, 243, 239
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_VoteRequestDone(ComWrappers.ComInterfaceDispatch* __this_native, int hr, nint pboidReason)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionVoterBallotAsync2>(__this_native).VoteRequestDone(hr, pboidReason);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterBallotAsync2_003EF2E47D616836AEADF0D19B4ACD06D9A42DC6BEB8AD1D469E0E196F6A203B9D2EC__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterBallotAsync2_003EF2E47D616836AEADF0D19B4ACD06D9A42DC6BEB8AD1D469E0E196F6A203B9D2EC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterBallotAsync2_003EF2E47D616836AEADF0D19B4ACD06D9A42DC6BEB8AD1D469E0E196F6A203B9D2EC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterBallotAsync2_003EF2E47D616836AEADF0D19B4ACD06D9A42DC6BEB8AD1D469E0E196F6A203B9D2EC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.VoteRequestDone_3 = &ABI_VoteRequestDone;
	}
}
