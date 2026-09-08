using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceImplementation : ITransactionExport
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionExport.Export(ITransaction punkTransaction, out uint pcbTransactionCookie)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionExport));
		delegate* unmanaged[MemberFunction]<void*, void*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, void*, uint*, int>)ptr4[3];
		pcbTransactionCookie = 0u;
		void* ptr5 = default(void*);
		int errorCode = 0;
		try
		{
			ptr5 = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(punkTransaction);
			fixed (uint* ptr6 = &pcbTransactionCookie)
			{
				errorCode = obj(ptr3, ptr5, ptr6);
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				165, 253, 65, 1, 192, 143, 206, 17, 189, 24,
				32, 76, 79, 79, 80, 32
			}), (nint)ptr3);
			GC.KeepAlive(this);
		}
		finally
		{
			ComInterfaceMarshaller<ITransaction>.Free(ptr5);
		}
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionExport.GetTransactionCookie(ITransaction pITransaction, uint cbTransactionCookie, byte[] rgbTransactionCookie, out uint pcbUsed)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionExport));
		delegate* unmanaged[MemberFunction]<void*, void*, uint, byte*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, void*, uint, byte*, uint*, int>)ptr4[4];
		pcbUsed = 0u;
		void* ptr5 = default(void*);
		int errorCode = 0;
		try
		{
			ptr5 = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(pITransaction);
			fixed (uint* ptr6 = &pcbUsed)
			{
				fixed (byte* ptr7 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(rgbTransactionCookie))
				{
					void* ptr8 = ptr7;
					errorCode = obj(ptr3, ptr5, cbTransactionCookie, (byte*)ptr8, ptr6);
				}
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				165, 253, 65, 1, 192, 143, 206, 17, 189, 24,
				32, 76, 79, 79, 80, 32
			}), (nint)ptr3);
			GC.KeepAlive(this);
		}
		finally
		{
			ComInterfaceMarshaller<ITransaction>.Free(ptr5);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Export(ComWrappers.ComInterfaceDispatch* __this_native, void* __punkTransaction_native, uint* __pcbTransactionCookie_native__param)
	{
		ITransaction transaction = null;
		ref uint reference = ref *__pcbTransactionCookie_native__param;
		uint pcbTransactionCookie = 0u;
		int num = 0;
		try
		{
			transaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(__punkTransaction_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionExport>(__this_native).Export(transaction, out pcbTransactionCookie);
			num = 0;
			reference = pcbTransactionCookie;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetTransactionCookie(ComWrappers.ComInterfaceDispatch* __this_native, void* __pITransaction_native, uint cbTransactionCookie, byte* __rgbTransactionCookie_native, uint* __pcbUsed_native__param)
	{
		ITransaction transaction = null;
		byte[] array = null;
		ref uint reference = ref *__pcbUsed_native__param;
		uint pcbUsed = 0u;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = checked((int)cbTransactionCookie);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__rgbTransactionCookie_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__rgbTransactionCookie_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			transaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(__pITransaction_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionExport>(__this_native).GetTransactionCookie(transaction, cbTransactionCookie, array, out pcbUsed);
			num = 0;
			reference = pcbUsed;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExport_003EFC6641784F50EFBA1BC5C5625F9FC78F627E7DA6B7AB61FA776EA58E0EBE44909__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Export_3 = &ABI_Export;
		Vtable.GetTransactionCookie_4 = &ABI_GetTransactionCookie;
	}
}
