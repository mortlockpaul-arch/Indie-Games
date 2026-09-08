using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceImplementation : ITransactionImportWhereabouts
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionImportWhereabouts.GetWhereaboutsSize(out uint pcbSize)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionImportWhereabouts));
		delegate* unmanaged[MemberFunction]<void*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint*, int>)ptr4[3];
		pcbSize = 0u;
		int errorCode;
		fixed (uint* ptr5 = &pcbSize)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			164, 253, 65, 1, 192, 143, 206, 17, 189, 24,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionImportWhereabouts.GetWhereabouts(uint cbWhereabouts, byte[] rgbWhereabouts, out uint pcbUsed)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionImportWhereabouts));
		delegate* unmanaged[MemberFunction]<void*, uint, byte*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, byte*, uint*, int>)ptr4[4];
		pcbUsed = 0u;
		int errorCode;
		fixed (uint* ptr5 = &pcbUsed)
		{
			fixed (byte* ptr6 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(rgbWhereabouts))
			{
				void* ptr7 = ptr6;
				errorCode = obj(ptr3, cbWhereabouts, (byte*)ptr7, ptr5);
			}
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			164, 253, 65, 1, 192, 143, 206, 17, 189, 24,
			32, 76, 79, 79, 80, 32
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetWhereaboutsSize(ComWrappers.ComInterfaceDispatch* __this_native, uint* __pcbSize_native__param)
	{
		ref uint reference = ref *__pcbSize_native__param;
		uint pcbSize = 0u;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionImportWhereabouts>(__this_native).GetWhereaboutsSize(out pcbSize);
			num = 0;
			reference = pcbSize;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetWhereabouts(ComWrappers.ComInterfaceDispatch* __this_native, uint cbWhereabouts, byte* __rgbWhereabouts_native, uint* __pcbUsed_native__param)
	{
		byte[] array = null;
		ref uint reference = ref *__pcbUsed_native__param;
		uint pcbUsed = 0u;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = checked((int)cbWhereabouts);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__rgbWhereabouts_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__rgbWhereabouts_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionImportWhereabouts>(__this_native).GetWhereabouts(cbWhereabouts, array, out pcbUsed);
			num = 0;
			reference = pcbUsed;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionImportWhereabouts_003EF692E874055F41035166A15EA963F11E89EFA6065B00774973E8142A772A36126__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.GetWhereaboutsSize_3 = &ABI_GetWhereaboutsSize;
		Vtable.GetWhereabouts_4 = &ABI_GetWhereabouts;
	}
}
