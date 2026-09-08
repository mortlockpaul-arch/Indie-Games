using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImport_003EF136A2D433BF33251E76B0FFABABE418933B19D57854E75064EBC6CD25CA9D981__InterfaceImplementation : ITransactionImport
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImport_003EF136A2D433BF33251E76B0FFABABE418933B19D57854E75064EBC6CD25CA9D981__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionImport.Import(uint cbTransactionCookie, byte[] rgbTransactionCookie, in Guid piid, out object ppvTransaction)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionImport));
		delegate* unmanaged[MemberFunction]<void*, uint, byte*, Guid*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, byte*, Guid*, void**, int>)ptr4[3];
		bool flag = false;
		ppvTransaction = null;
		void* unmanaged = default(void*);
		int errorCode = 0;
		try
		{
			fixed (Guid* ptr5 = &piid)
			{
				fixed (byte* ptr6 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(rgbTransactionCookie))
				{
					void* ptr7 = ptr6;
					errorCode = obj(ptr3, cbTransactionCookie, (byte*)ptr7, ptr5, &unmanaged);
				}
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				90, 155, 207, 225, 69, 135, 206, 17, 169, 186,
				0, 170, 0, 108, 55, 6
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppvTransaction = ComInterfaceMarshaller<object>.ConvertToManaged(unmanaged);
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
	internal unsafe static int ABI_Import(ComWrappers.ComInterfaceDispatch* __this_native, uint cbTransactionCookie, byte* __rgbTransactionCookie_native, Guid* __piid_native__param, void** __ppvTransaction_native__param)
	{
		byte[] array = null;
		ref Guid reference = ref *__piid_native__param;
		Guid guid = default(Guid);
		ref void* reference2 = ref *__ppvTransaction_native__param;
		object ppvTransaction = null;
		int num = 0;
		int num2 = 0;
		try
		{
			guid = reference;
			num2 = checked((int)cbTransactionCookie);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__rgbTransactionCookie_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__rgbTransactionCookie_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionImport>(__this_native).Import(cbTransactionCookie, array, in guid, out ppvTransaction);
			num = 0;
			reference2 = ComInterfaceMarshaller<object>.ConvertToUnmanaged(ppvTransaction);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImport_003EF136A2D433BF33251E76B0FFABABE418933B19D57854E75064EBC6CD25CA9D981__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImport_003EF136A2D433BF33251E76B0FFABABE418933B19D57854E75064EBC6CD25CA9D981__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImport_003EF136A2D433BF33251E76B0FFABABE418933B19D57854E75064EBC6CD25CA9D981__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImport_003EF136A2D433BF33251E76B0FFABABE418933B19D57854E75064EBC6CD25CA9D981__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Import_3 = &ABI_Import;
	}
}
