using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceImplementation : ITransactionExportFactory
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionExportFactory.GetRemoteClassId(out Guid pclsid)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionExportFactory));
		delegate* unmanaged[MemberFunction]<void*, Guid*, int> obj = (delegate* unmanaged[MemberFunction]<void*, Guid*, int>)ptr4[3];
		pclsid = default(Guid);
		int errorCode;
		fixed (Guid* ptr5 = &pclsid)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			83, 155, 207, 225, 69, 135, 206, 17, 169, 186,
			0, 170, 0, 108, 55, 6
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITransactionExportFactory.Create(uint cbWhereabouts, byte[] rgbWhereabouts, out ITransactionExport ppExport)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITransactionExportFactory));
		delegate* unmanaged[MemberFunction]<void*, uint, byte*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, byte*, void**, int>)ptr4[4];
		bool flag = false;
		ppExport = null;
		void* unmanaged = default(void*);
		int errorCode = 0;
		try
		{
			fixed (byte* ptr5 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(rgbWhereabouts))
			{
				void* ptr6 = ptr5;
				errorCode = obj(ptr3, cbWhereabouts, (byte*)ptr6, &unmanaged);
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				83, 155, 207, 225, 69, 135, 206, 17, 169, 186,
				0, 170, 0, 108, 55, 6
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			ppExport = ComInterfaceMarshaller<ITransactionExport>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<ITransactionExport>.Free(unmanaged);
			}
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetRemoteClassId(ComWrappers.ComInterfaceDispatch* __this_native, Guid* __pclsid_native__param)
	{
		ref Guid reference = ref *__pclsid_native__param;
		Guid pclsid = default(Guid);
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionExportFactory>(__this_native).GetRemoteClassId(out pclsid);
			num = 0;
			reference = pclsid;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Create(ComWrappers.ComInterfaceDispatch* __this_native, uint cbWhereabouts, byte* __rgbWhereabouts_native, void** __ppExport_native__param)
	{
		byte[] array = null;
		ref void* reference = ref *__ppExport_native__param;
		ITransactionExport ppExport = null;
		int num = 0;
		int num2 = 0;
		try
		{
			num2 = checked((int)cbWhereabouts);
			array = ArrayMarshaller<byte, byte>.AllocateContainerForManagedElements(__rgbWhereabouts_native, num2);
			ArrayMarshaller<byte, byte>.GetUnmanagedValuesSource(__rgbWhereabouts_native, num2).CopyTo(ArrayMarshaller<byte, byte>.GetManagedValuesDestination(array));
			ComWrappers.ComInterfaceDispatch.GetInstance<ITransactionExportFactory>(__this_native).Create(cbWhereabouts, array, out ppExport);
			num = 0;
			reference = ComInterfaceMarshaller<ITransactionExport>.ConvertToUnmanaged(ppExport);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionExportFactory_003EF0630CFDC64B09FCCD09618B860A78E37333C0C9BA079F9F751EBEF36FC60B9CF__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.GetRemoteClassId_3 = &ABI_GetRemoteClassId;
		Vtable.Create_4 = &ABI_Create;
	}
}
