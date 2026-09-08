using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionOptions_003EF0A6F6322F5E1B0310322FE13DBC019526DCE516A0CF8EB239EAAFB89D2C15F7E__InterfaceImplementation : ITransactionOptions
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionOptions_003EF0A6F6322F5E1B0310322FE13DBC019526DCE516A0CF8EB239EAAFB89D2C15F7E__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionOptions.SetOptions(Xactopt pOptions)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionOptions));
		delegate* unmanaged[MemberFunction]<void*, Xactopt.Marshaller.XactoptNative, int> obj = (delegate* unmanaged[MemberFunction]<void*, Xactopt.Marshaller.XactoptNative, int>)ptr4[3];
		Xactopt.Marshaller.XactoptNative xactoptNative = Xactopt.Marshaller.ConvertToUnmanaged(pOptions);
		Marshal.ThrowExceptionForHR(obj(ptr3, xactoptNative), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			224, 217, 106, 58, 185, 35, 207, 17, 173, 96,
			0, 170, 0, 167, 76, 205
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionOptions.GetOptions()
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionOptions));
		Marshal.ThrowExceptionForHR(((delegate* unmanaged[MemberFunction]<void*, int>)ptr4[4])(ptr3), new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			224, 217, 106, 58, 185, 35, 207, 17, 173, 96,
			0, 170, 0, 167, 76, 205
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_SetOptions(ComWrappers.ComInterfaceDispatch* __this_native, Xactopt.Marshaller.XactoptNative __pOptions_native)
	{
		Xactopt xactopt = default(Xactopt);
		int num = 0;
		try
		{
			xactopt = Xactopt.Marshaller.ConvertToManaged(__pOptions_native);
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionOptions>(__this_native).SetOptions(xactopt);
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetOptions(ComWrappers.ComInterfaceDispatch* __this_native)
	{
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionOptions>(__this_native).GetOptions();
			return 0;
		}
		catch (Exception e)
		{
			return ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionOptions_003EF0A6F6322F5E1B0310322FE13DBC019526DCE516A0CF8EB239EAAFB89D2C15F7E__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionOptions_003EF0A6F6322F5E1B0310322FE13DBC019526DCE516A0CF8EB239EAAFB89D2C15F7E__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionOptions_003EF0A6F6322F5E1B0310322FE13DBC019526DCE516A0CF8EB239EAAFB89D2C15F7E__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionOptions_003EF0A6F6322F5E1B0310322FE13DBC019526DCE516A0CF8EB239EAAFB89D2C15F7E__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.SetOptions_3 = &ABI_SetOptions;
		Vtable.GetOptions_4 = &ABI_GetOptions;
	}
}
