using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IPrepareInfo_003EF2B331A3475953ED21DD6863BAC5587A8F19ADB192318DF4A4782EEA8F78F6C33__InterfaceImplementation : IPrepareInfo
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IPrepareInfo_003EF2B331A3475953ED21DD6863BAC5587A8F19ADB192318DF4A4782EEA8F78F6C33__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IPrepareInfo.GetPrepareInfoSize(out uint pcbPrepInfo)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IPrepareInfo));
		delegate* unmanaged[MemberFunction]<void*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint*, int>)ptr4[3];
		pcbPrepInfo = 0u;
		int errorCode;
		fixed (uint* ptr5 = &pcbPrepInfo)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			208, 191, 199, 128, 238, 135, 206, 17, 128, 129,
			0, 128, 199, 88, 82, 126
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IPrepareInfo.GetPrepareInfo(byte[] pPrepInfo)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IPrepareInfo));
		delegate* unmanaged[MemberFunction]<void*, byte*, int> obj = (delegate* unmanaged[MemberFunction]<void*, byte*, int>)ptr4[4];
		int errorCode;
		fixed (byte* ptr5 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(pPrepInfo))
		{
			void* ptr6 = ptr5;
			errorCode = obj(ptr3, (byte*)ptr6);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			208, 191, 199, 128, 238, 135, 206, 17, 128, 129,
			0, 128, 199, 88, 82, 126
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetPrepareInfoSize(ComWrappers.ComInterfaceDispatch* __this_native, uint* __pcbPrepInfo_native__param)
	{
		ref uint reference = ref *__pcbPrepInfo_native__param;
		uint pcbPrepInfo = 0u;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<IPrepareInfo>(__this_native).GetPrepareInfoSize(out pcbPrepInfo);
			num = 0;
			reference = pcbPrepInfo;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetPrepareInfo(ComWrappers.ComInterfaceDispatch* __this_native, byte* __pPrepInfo_native)
	{
		byte[] array = null;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = -1;
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__pPrepInfo_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__pPrepInfo_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<IPrepareInfo>(__this_native).GetPrepareInfo(array);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IPrepareInfo_003EF2B331A3475953ED21DD6863BAC5587A8F19ADB192318DF4A4782EEA8F78F6C33__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IPrepareInfo_003EF2B331A3475953ED21DD6863BAC5587A8F19ADB192318DF4A4782EEA8F78F6C33__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IPrepareInfo_003EF2B331A3475953ED21DD6863BAC5587A8F19ADB192318DF4A4782EEA8F78F6C33__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IPrepareInfo_003EF2B331A3475953ED21DD6863BAC5587A8F19ADB192318DF4A4782EEA8F78F6C33__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.GetPrepareInfoSize_3 = &ABI_GetPrepareInfoSize;
		Vtable.GetPrepareInfo_4 = &ABI_GetPrepareInfo;
	}
}
