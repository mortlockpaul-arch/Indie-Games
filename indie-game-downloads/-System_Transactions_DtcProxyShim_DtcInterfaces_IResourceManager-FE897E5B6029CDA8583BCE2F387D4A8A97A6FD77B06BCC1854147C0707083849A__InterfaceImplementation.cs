using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementation : IResourceManager
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IResourceManager.Enlist(ITransaction pTransaction, ITransactionResourceAsync pRes, out Guid pUOW, out OletxTransactionIsolationLevel pisoLevel, out ITransactionEnlistmentAsync ppEnlist)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IResourceManager));
		delegate* unmanaged[MemberFunction]<void*, void*, void*, Guid*, OletxTransactionIsolationLevel*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, void*, void*, Guid*, OletxTransactionIsolationLevel*, void**, int>)ptr4[3];
		bool flag = false;
		pUOW = default(Guid);
		pisoLevel = (OletxTransactionIsolationLevel)0;
		ppEnlist = null;
		void* ptr5 = default(void*);
		void* ptr6 = default(void*);
		void* unmanaged = default(void*);
		int errorCode = 0;
		try
		{
			ptr6 = ComInterfaceMarshaller<ITransactionResourceAsync>.ConvertToUnmanaged(pRes);
			ptr5 = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(pTransaction);
			fixed (OletxTransactionIsolationLevel* ptr7 = &pisoLevel)
			{
				fixed (Guid* ptr8 = &pUOW)
				{
					errorCode = obj(ptr3, ptr5, ptr6, ptr8, ptr7, &unmanaged);
				}
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				33, 29, 116, 19, 235, 135, 206, 17, 128, 129,
				0, 128, 199, 88, 82, 126
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppEnlist = ComInterfaceMarshaller<ITransactionEnlistmentAsync>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransactionEnlistmentAsync>.Free(unmanaged);
			}
			ComInterfaceMarshaller<ITransactionResourceAsync>.Free(ptr6);
			ComInterfaceMarshaller<ITransaction>.Free(ptr5);
		}
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IResourceManager.Reenlist(byte[] pPrepInfo, uint cbPrepInfom, uint lTimeout, out OletxXactStat pXactStat)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IResourceManager));
		delegate* unmanaged[MemberFunction]<void*, byte*, uint, uint, OletxXactStat*, int> obj = (delegate* unmanaged[MemberFunction]<void*, byte*, uint, uint, OletxXactStat*, int>)ptr4[4];
		pXactStat = OletxXactStat.XACTSTAT_NONE;
		int errorCode;
		fixed (OletxXactStat* ptr5 = &pXactStat)
		{
			fixed (byte* ptr6 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(pPrepInfo))
			{
				void* ptr7 = ptr6;
				errorCode = obj(ptr3, (byte*)ptr7, cbPrepInfom, lTimeout, ptr5);
			}
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			33, 29, 116, 19, 235, 135, 206, 17, 128, 129,
			0, 128, 199, 88, 82, 126
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IResourceManager.ReenlistmentComplete()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IResourceManager));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[5])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			33, 29, 116, 19, 235, 135, 206, 17, 128, 129,
			0, 128, 199, 88, 82, 126
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IResourceManager.GetDistributedTransactionManager(in Guid riid, out object ppvObject)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IResourceManager));
		delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int>)ptr4[6];
		bool flag = false;
		ppvObject = null;
		void* unmanaged = default(void*);
		int errorCode = 0;
		try
		{
			fixed (Guid* ptr5 = &riid)
			{
				errorCode = obj(ptr3, ptr5, &unmanaged);
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				33, 29, 116, 19, 235, 135, 206, 17, 128, 129,
				0, 128, 199, 88, 82, 126
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppvObject = ComInterfaceMarshaller<object>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<object>.Free(unmanaged);
			}
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Enlist(ComWrappers.ComInterfaceDispatch* __this_native, void* __pTransaction_native, void* __pRes_native, Guid* __pUOW_native__param, OletxTransactionIsolationLevel* __pisoLevel_native__param, void** __ppEnlist_native__param)
	{
		ITransaction transaction = null;
		ITransactionResourceAsync transactionResourceAsync = null;
		ref Guid reference = ref *__pUOW_native__param;
		Guid pUOW = default(Guid);
		ref OletxTransactionIsolationLevel reference2 = ref *__pisoLevel_native__param;
		OletxTransactionIsolationLevel pisoLevel = (OletxTransactionIsolationLevel)0;
		ref void* reference3 = ref *__ppEnlist_native__param;
		ITransactionEnlistmentAsync ppEnlist = null;
		int num = 0;
		try
		{
			transactionResourceAsync = ComInterfaceMarshaller<ITransactionResourceAsync>.ConvertToManaged(__pRes_native);
			transaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(__pTransaction_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<IResourceManager>(__this_native).Enlist(transaction, transactionResourceAsync, out pUOW, out pisoLevel, out ppEnlist);
			num = 0;
			reference3 = ComInterfaceMarshaller<ITransactionEnlistmentAsync>.ConvertToUnmanaged(ppEnlist);
			reference2 = pisoLevel;
			reference = pUOW;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Reenlist(ComWrappers.ComInterfaceDispatch* __this_native, byte* __pPrepInfo_native, uint cbPrepInfom, uint lTimeout, OletxXactStat* __pXactStat_native__param)
	{
		byte[] array = null;
		ref OletxXactStat reference = ref *__pXactStat_native__param;
		OletxXactStat pXactStat = OletxXactStat.XACTSTAT_NONE;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = checked((int)cbPrepInfom);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__pPrepInfo_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__pPrepInfo_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<IResourceManager>(__this_native).Reenlist(array, cbPrepInfom, lTimeout, out pXactStat);
			num = 0;
			reference = pXactStat;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_ReenlistmentComplete(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<IResourceManager>(__this_native).ReenlistmentComplete();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetDistributedTransactionManager(ComWrappers.ComInterfaceDispatch* __this_native, Guid* __riid_native__param, void** __ppvObject_native__param)
	{
		ref Guid reference = ref *__riid_native__param;
		Guid guid = default(Guid);
		ref void* reference2 = ref *__ppvObject_native__param;
		object ppvObject = null;
		int num = 0;
		try
		{
			guid = reference;
			ComWrappers.ComInterfaceDispatch.GetInstance<IResourceManager>(__this_native).GetDistributedTransactionManager(in guid, out ppvObject);
			num = 0;
			reference2 = ComInterfaceMarshaller<object>.ConvertToUnmanaged(ppvObject);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Enlist_3 = &ABI_Enlist;
		Vtable.Reenlist_4 = &ABI_Reenlist;
		Vtable.ReenlistmentComplete_5 = &ABI_ReenlistmentComplete;
		Vtable.GetDistributedTransactionManager_6 = &ABI_GetDistributedTransactionManager;
	}
}
