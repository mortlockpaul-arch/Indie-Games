using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceImplementation : IResourceManagerFactory2
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IResourceManagerFactory2.Create(in Guid pguidRM, string pszRMName, IResourceManagerSink pIResMgrSink, out IResourceManager rm)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IResourceManagerFactory2));
		delegate* unmanaged[MemberFunction]<void*, Guid*, byte*, void*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, Guid*, byte*, void*, void**, int>)ptr4[3];
		bool flag = false;
		rm = null;
		byte* ptr5 = default(byte*);
		void* ptr6 = default(void*);
		void* unmanaged = default(void*);
		int errorCode = 0;
		AnsiStringMarshaller.ManagedToUnmanagedIn managedToUnmanagedIn = default(AnsiStringMarshaller.ManagedToUnmanagedIn);
		try
		{
			ptr6 = ComInterfaceMarshaller<IResourceManagerSink>.ConvertToUnmanaged(pIResMgrSink);
			Span<byte> buffer = stackalloc byte[AnsiStringMarshaller.ManagedToUnmanagedIn.BufferSize];
			managedToUnmanagedIn.FromManaged(pszRMName, buffer);
			fixed (Guid* ptr7 = &pguidRM)
			{
				ptr5 = managedToUnmanagedIn.ToUnmanaged();
				errorCode = obj(ptr3, ptr7, ptr5, ptr6, &unmanaged);
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				33, 156, 54, 107, 210, 251, 209, 17, 143, 71,
				0, 192, 79, 142, 229, 125
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			rm = ComInterfaceMarshaller<IResourceManager>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<IResourceManager>.Free(unmanaged);
			}
			ComInterfaceMarshaller<IResourceManagerSink>.Free(ptr6);
			managedToUnmanagedIn.Free();
		}
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void IResourceManagerFactory2.CreateEx(in Guid pguidRM, string pszRMName, IResourceManagerSink pIResMgrSink, in Guid riidRequested, out object rm)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(IResourceManagerFactory2));
		delegate* unmanaged[MemberFunction]<void*, Guid*, byte*, void*, Guid*, void**, int> obj = (delegate* unmanaged[MemberFunction]<void*, Guid*, byte*, void*, Guid*, void**, int>)ptr4[4];
		bool flag = false;
		rm = null;
		byte* ptr5 = default(byte*);
		void* ptr6 = default(void*);
		void* unmanaged = default(void*);
		int errorCode = 0;
		AnsiStringMarshaller.ManagedToUnmanagedIn managedToUnmanagedIn = default(AnsiStringMarshaller.ManagedToUnmanagedIn);
		try
		{
			ptr6 = ComInterfaceMarshaller<IResourceManagerSink>.ConvertToUnmanaged(pIResMgrSink);
			Span<byte> buffer = stackalloc byte[AnsiStringMarshaller.ManagedToUnmanagedIn.BufferSize];
			managedToUnmanagedIn.FromManaged(pszRMName, buffer);
			fixed (Guid* ptr7 = &riidRequested)
			{
				fixed (Guid* ptr8 = &pguidRM)
				{
					ptr5 = managedToUnmanagedIn.ToUnmanaged();
					errorCode = obj(ptr3, ptr8, ptr5, ptr6, ptr7, &unmanaged);
				}
			}
			Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				33, 156, 54, 107, 210, 251, 209, 17, 143, 71,
				0, 192, 79, 142, 229, 125
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			rm = ComInterfaceMarshaller<object>.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				ComInterfaceMarshaller<object>.Free(unmanaged);
			}
			ComInterfaceMarshaller<IResourceManagerSink>.Free(ptr6);
			managedToUnmanagedIn.Free();
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_Create(ComWrappers.ComInterfaceDispatch* __this_native, Guid* __pguidRM_native__param, byte* __pszRMName_native, void* __pIResMgrSink_native, void** __rm_native__param)
	{
		ref Guid reference = ref *__pguidRM_native__param;
		Guid guid = default(Guid);
		string text = null;
		IResourceManagerSink resourceManagerSink = null;
		ref void* reference2 = ref *__rm_native__param;
		IResourceManager rm = null;
		int num = 0;
		try
		{
			resourceManagerSink = ComInterfaceMarshaller<IResourceManagerSink>.ConvertToManaged(__pIResMgrSink_native);
			text = AnsiStringMarshaller.ConvertToManaged(__pszRMName_native);
			guid = reference;
			ComWrappers.ComInterfaceDispatch.GetInstance<IResourceManagerFactory2>(__this_native).Create(in guid, text, resourceManagerSink, out rm);
			num = 0;
			reference2 = ComInterfaceMarshaller<IResourceManager>.ConvertToUnmanaged(rm);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_CreateEx(ComWrappers.ComInterfaceDispatch* __this_native, Guid* __pguidRM_native__param, byte* __pszRMName_native, void* __pIResMgrSink_native, Guid* __riidRequested_native__param, void** __rm_native__param)
	{
		ref Guid reference = ref *__pguidRM_native__param;
		Guid guid = default(Guid);
		string text = null;
		IResourceManagerSink resourceManagerSink = null;
		ref Guid reference2 = ref *__riidRequested_native__param;
		Guid guid2 = default(Guid);
		ref void* reference3 = ref *__rm_native__param;
		object rm = null;
		int num = 0;
		try
		{
			guid2 = reference2;
			resourceManagerSink = ComInterfaceMarshaller<IResourceManagerSink>.ConvertToManaged(__pIResMgrSink_native);
			text = AnsiStringMarshaller.ConvertToManaged(__pszRMName_native);
			guid = reference;
			ComWrappers.ComInterfaceDispatch.GetInstance<IResourceManagerFactory2>(__this_native).CreateEx(in guid, text, resourceManagerSink, in guid2, out rm);
			num = 0;
			reference3 = ComInterfaceMarshaller<object>.ConvertToUnmanaged(rm);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManagerFactory2_003EFEAEADA07089F40240E793A881CA6F6D68F2303BA29103D9726CB2CAC42C1F892__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.Create_3 = &ABI_Create;
		Vtable.CreateEx_4 = &ABI_CreateEx;
	}
}
