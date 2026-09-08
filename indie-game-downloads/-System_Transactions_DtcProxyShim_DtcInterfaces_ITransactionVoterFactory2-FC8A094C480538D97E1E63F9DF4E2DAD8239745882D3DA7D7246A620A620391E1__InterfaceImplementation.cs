using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterFactory2_003EFC8A094C480538D97E1E63F9DF4E2DAD8239745882D3DA7D7246A620A620391E1__InterfaceImplementation : ITransactionVoterFactory2
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterFactory2_003EFC8A094C480538D97E1E63F9DF4E2DAD8239745882D3DA7D7246A620A620391E1__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionVoterFactory2.Create(ITransaction pITransaction, ITransactionVoterNotifyAsync2 pVoterNotify, out ITransactionVoterBallotAsync2 ppVoterBallot)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionVoterFactory2));
		delegate* unmanaged[MemberFunction]<void*, void*, void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void*, void*, void**, int>)ptr4[3];
		bool flag = false;
		ppVoterBallot = null;
		void* ptr5 = default(void*);
		void* ptr6 = default(void*);
		void* unmanaged = default(void*);
		try
		{
			ptr6 = ComInterfaceMarshaller<ITransactionVoterNotifyAsync2>.ConvertToUnmanaged(pVoterNotify);
			ptr5 = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(pITransaction);
			Marshal.ThrowExceptionForHR(obj(ptr3, ptr5, ptr6, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				106, 55, 51, 84, 77, 65, 211, 17, 178, 6,
				0, 192, 79, 194, 243, 239
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppVoterBallot = ComInterfaceMarshaller<ITransactionVoterBallotAsync2>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransactionVoterBallotAsync2>.Free(unmanaged);
			}
			ComInterfaceMarshaller<ITransactionVoterNotifyAsync2>.Free(ptr6);
			ComInterfaceMarshaller<ITransaction>.Free(ptr5);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Create(ComWrappers.ComInterfaceDispatch* __this_native, void* __pITransaction_native, void* __pVoterNotify_native, void** __ppVoterBallot_native__param)
	{
		ITransaction transaction = null;
		ITransactionVoterNotifyAsync2 transactionVoterNotifyAsync = null;
		ref void* reference = ref *__ppVoterBallot_native__param;
		ITransactionVoterBallotAsync2 ppVoterBallot = null;
		int num = 0;
		try
		{
			transactionVoterNotifyAsync = ComInterfaceMarshaller<ITransactionVoterNotifyAsync2>.ConvertToManaged(__pVoterNotify_native);
			transaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(__pITransaction_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionVoterFactory2>(__this_native).Create(transaction, transactionVoterNotifyAsync, out ppVoterBallot);
			num = 0;
			reference = ComInterfaceMarshaller<ITransactionVoterBallotAsync2>.ConvertToUnmanaged(ppVoterBallot);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterFactory2_003EFC8A094C480538D97E1E63F9DF4E2DAD8239745882D3DA7D7246A620A620391E1__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterFactory2_003EFC8A094C480538D97E1E63F9DF4E2DAD8239745882D3DA7D7246A620A620391E1__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterFactory2_003EFC8A094C480538D97E1E63F9DF4E2DAD8239745882D3DA7D7246A620A620391E1__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterFactory2_003EFC8A094C480538D97E1E63F9DF4E2DAD8239745882D3DA7D7246A620A620391E1__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Create_3 = &ABI_Create;
	}
}
