using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceImplementation : ITransactionReceiver
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionReceiver.UnmarshalPropagationToken(uint cbToken, byte[] rgbToken, out ITransaction ppTransaction)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionReceiver));
		delegate* unmanaged[MemberFunction]<void*, uint, byte*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, byte*, void**, int>)ptr4[3];
		bool flag = false;
		ppTransaction = null;
		void* unmanaged = default(void*);
		int errorCode = 0;
		try
		{
			fixed (byte* ptr5 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(rgbToken))
			{
				void* ptr6 = ptr5;
				errorCode = obj(ptr3, cbToken, (byte*)ptr6, &unmanaged);
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				3, 62, 49, 89, 108, 179, 207, 17, 165, 57,
				0, 170, 0, 104, 135, 195
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppTransaction = ComInterfaceMarshaller<ITransaction>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransaction>.Free(unmanaged);
			}
		}
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionReceiver.GetReturnTokenSize(out uint pcbReturnToken)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionReceiver));
		delegate* unmanaged[MemberFunction]<void*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint*, int>)ptr4[4];
		pcbReturnToken = 0u;
		int errorCode;
		fixed (uint* ptr5 = &pcbReturnToken)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			3, 62, 49, 89, 108, 179, 207, 17, 165, 57,
			0, 170, 0, 104, 135, 195
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionReceiver.MarshalReturnToken(uint cbReturnToken, out byte[] rgbReturnToken, out uint pcbUsed)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionReceiver));
		delegate* unmanaged[MemberFunction]<void*, uint, byte**, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, byte**, uint*, int>)ptr4[5];
		bool flag = false;
		rgbReturnToken = null;
		pcbUsed = 0u;
		byte* ptr5 = default(byte*);
		int errorCode = 0;
		Unsafe.SkipInit<int>(out var value);
		try
		{
			fixed (uint* ptr6 = &pcbUsed)
			{
				errorCode = obj(ptr3, cbReturnToken, &ptr5, ptr6);
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				3, 62, 49, 89, 108, 179, 207, 17, 165, 57,
				0, 170, 0, 104, 135, 195
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			value = checked((int)cbReturnToken);
			rgbReturnToken = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(ptr5, value);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(ptr5, value).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(rgbReturnToken));
		}
		finally
		{
			if (flag)
			{
				ArrayMarshaller<byte, byte>.Free(ptr5);
			}
		}
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionReceiver.Reset()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionReceiver));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[6])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			3, 62, 49, 89, 108, 179, 207, 17, 165, 57,
			0, 170, 0, 104, 135, 195
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_UnmarshalPropagationToken(ComWrappers.ComInterfaceDispatch* __this_native, uint cbToken, byte* __rgbToken_native, void** __ppTransaction_native__param)
	{
		byte[] array = null;
		ref void* reference = ref *__ppTransaction_native__param;
		ITransaction ppTransaction = null;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = checked((int)cbToken);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__rgbToken_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__rgbToken_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionReceiver>(__this_native).UnmarshalPropagationToken(cbToken, array, out ppTransaction);
			num = 0;
			reference = ComInterfaceMarshaller<ITransaction>.ConvertToUnmanaged(ppTransaction);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetReturnTokenSize(ComWrappers.ComInterfaceDispatch* __this_native, uint* __pcbReturnToken_native__param)
	{
		ref uint reference = ref *__pcbReturnToken_native__param;
		uint pcbReturnToken = 0u;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionReceiver>(__this_native).GetReturnTokenSize(out pcbReturnToken);
			num = 0;
			reference = pcbReturnToken;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_MarshalReturnToken(ComWrappers.ComInterfaceDispatch* __this_native, uint cbReturnToken, byte** __rgbReturnToken_native__param, uint* __pcbUsed_native__param)
	{
		ref byte* reference = ref *__rgbReturnToken_native__param;
		byte[] rgbReturnToken = null;
		ref uint reference2 = ref *__pcbUsed_native__param;
		uint pcbUsed = 0u;
		int num = 0;
		int numElements = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionReceiver>(__this_native).MarshalReturnToken(cbReturnToken, out rgbReturnToken, out pcbUsed);
			num = 0;
			reference2 = pcbUsed;
			reference = ArrayMarshaller<byte, byte>.AllocateContainerForUnmanagedElements(rgbReturnToken, out numElements);
			ArrayMarshaller<byte, byte>.GetManagedValuesSource(rgbReturnToken).CopyTo(ArrayMarshaller<byte, byte>.GetUnmanagedValuesDestination(reference, numElements));
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Reset(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionReceiver>(__this_native).Reset();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionReceiver_003EFAB549EE1B28E423434D7E6997F235C5E13FF339389DBE4A4F6D76D72F5B1A9D9__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.UnmarshalPropagationToken_3 = &ABI_UnmarshalPropagationToken;
		Vtable.GetReturnTokenSize_4 = &ABI_GetReturnTokenSize;
		Vtable.MarshalReturnToken_5 = &ABI_MarshalReturnToken;
		Vtable.Reset_6 = &ABI_Reset;
	}
}
