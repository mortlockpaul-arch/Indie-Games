using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceImplementation : ITransactionTransmitter
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionTransmitter.Set(ITransaction transaction)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionTransmitter));
		delegate* unmanaged[MemberFunction]<void*, void*, int> obj = (delegate* unmanaged[MemberFunction]<void*, void*, int>)ptr4[3];
		void* ptr5 = default(void*);
		try
		{
			ptr5 = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(transaction);
			Marshal.ThrowExceptionForHR(obj(ptr3, ptr5), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				1, 62, 49, 89, 108, 179, 207, 17, 165, 57,
				0, 170, 0, 104, 135, 195
			}), (nint)ptr3);
			GC.KeepAlive(this);
		}
		finally
		{
			ComInterfaceMarshaller<ITransaction>.Free(ptr5);
		}
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionTransmitter.GetPropagationTokenSize(out uint pcbToken)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionTransmitter));
		delegate* unmanaged[MemberFunction]<void*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint*, int>)ptr4[4];
		pcbToken = 0u;
		int errorCode;
		fixed (uint* ptr5 = &pcbToken)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			1, 62, 49, 89, 108, 179, 207, 17, 165, 57,
			0, 170, 0, 104, 135, 195
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionTransmitter.MarshalPropagationToken(uint cbToken, byte[] rgbToken, out uint pcbUsed)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionTransmitter));
		delegate* unmanaged[MemberFunction]<void*, uint, byte*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, byte*, uint*, int>)ptr4[5];
		pcbUsed = 0u;
		int errorCode;
		fixed (uint* ptr5 = &pcbUsed)
		{
			fixed (byte* ptr6 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(rgbToken))
			{
				void* ptr7 = ptr6;
				errorCode = obj(ptr3, cbToken, (byte*)ptr7, ptr5);
			}
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			1, 62, 49, 89, 108, 179, 207, 17, 165, 57,
			0, 170, 0, 104, 135, 195
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionTransmitter.UnmarshalReturnToken(uint cbReturnToken, byte[] rgbToken)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionTransmitter));
		delegate* unmanaged[MemberFunction]<void*, uint, byte*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, byte*, int>)ptr4[6];
		int errorCode;
		fixed (byte* ptr5 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(rgbToken))
		{
			void* ptr6 = ptr5;
			errorCode = obj(ptr3, cbReturnToken, (byte*)ptr6);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			1, 62, 49, 89, 108, 179, 207, 17, 165, 57,
			0, 170, 0, 104, 135, 195
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionTransmitter.Reset()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionTransmitter));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[7])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			1, 62, 49, 89, 108, 179, 207, 17, 165, 57,
			0, 170, 0, 104, 135, 195
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Set(ComWrappers.ComInterfaceDispatch* __this_native, void* __transaction_native)
	{
		ITransaction transaction = null;
		int num = 0;
		try
		{
			transaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(__transaction_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionTransmitter>(__this_native).Set(transaction);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetPropagationTokenSize(ComWrappers.ComInterfaceDispatch* __this_native, uint* __pcbToken_native__param)
	{
		ref uint reference = ref *__pcbToken_native__param;
		uint pcbToken = 0u;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionTransmitter>(__this_native).GetPropagationTokenSize(out pcbToken);
			num = 0;
			reference = pcbToken;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_MarshalPropagationToken(ComWrappers.ComInterfaceDispatch* __this_native, uint cbToken, byte* __rgbToken_native, uint* __pcbUsed_native__param)
	{
		byte[] array = null;
		ref uint reference = ref *__pcbUsed_native__param;
		uint pcbUsed = 0u;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = checked((int)cbToken);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__rgbToken_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__rgbToken_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionTransmitter>(__this_native).MarshalPropagationToken(cbToken, array, out pcbUsed);
			num = 0;
			reference = pcbUsed;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_UnmarshalReturnToken(ComWrappers.ComInterfaceDispatch* __this_native, uint cbReturnToken, byte* __rgbToken_native)
	{
		byte[] array = null;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = checked((int)cbReturnToken);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__rgbToken_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__rgbToken_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionTransmitter>(__this_native).UnmarshalReturnToken(cbReturnToken, array);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Reset(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionTransmitter>(__this_native).Reset();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionTransmitter_003EF98629DE4E10FBFEBDF15BDD34C3F3B9E1F91F3504910897773399D67C6F93CDC__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Set_3 = &ABI_Set;
		Vtable.GetPropagationTokenSize_4 = &ABI_GetPropagationTokenSize;
		Vtable.MarshalPropagationToken_5 = &ABI_MarshalPropagationToken;
		Vtable.UnmarshalReturnToken_6 = &ABI_UnmarshalReturnToken;
		Vtable.Reset_7 = &ABI_Reset;
	}
}
