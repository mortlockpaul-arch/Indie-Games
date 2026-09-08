using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

[DynamicInterfaceCastableImplementation]
internal interface _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITmNodeName_003EF83A5FD8143BEEC85074C07122F0B8F7539C69880D2FFFC413D71F3163F0A0BDF__InterfaceImplementation : ITmNodeName
{
	[FixedAddressValueType]
	static readonly _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITmNodeName_003EF83A5FD8143BEEC85074C07122F0B8F7539C69880D2FFFC413D71F3163F0A0BDF__InterfaceImplementationVtable Vtable;

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITmNodeName.GetNodeNameSize(out uint pcbNodeNameSize)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITmNodeName));
		delegate* unmanaged[MemberFunction]<void*, uint*, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint*, int>)ptr4[3];
		pcbNodeNameSize = 0u;
		int errorCode;
		fixed (uint* ptr5 = &pcbNodeNameSize)
		{
			errorCode = obj(ptr3, ptr5);
		}
		Marshal.ThrowExceptionForHR(errorCode, new Guid((ReadOnlySpan<byte>)new byte[16]
		{
			136, 79, 39, 48, 228, 110, 78, 71, 155, 149,
			120, 7, 188, 158, 248, 207
		}), (nint)ptr3);
		GC.KeepAlive(this);
	}

	[GeneratedCode("Microsoft.Interop.ComInterfaceGenerator", "10.0.14.37416")]
	unsafe void ITmNodeName.GetNodeName(uint cbNodeNameBufferSize, out string pcbNodeSize)
	{
		var (ptr3, ptr4) = ((IUnmanagedVirtualMethodTableProvider)this).GetVirtualMethodTableInfoForKey(typeof(ITmNodeName));
		delegate* unmanaged[MemberFunction]<void*, uint, ushort**, int> obj = (delegate* unmanaged[MemberFunction]<void*, uint, ushort**, int>)ptr4[4];
		bool flag = false;
		pcbNodeSize = null;
		ushort* unmanaged = default(ushort*);
		try
		{
			Marshal.ThrowExceptionForHR(obj(ptr3, cbNodeNameBufferSize, &unmanaged), new Guid((ReadOnlySpan<byte>)new byte[16]
			{
				136, 79, 39, 48, 228, 110, 78, 71, 155, 149,
				120, 7, 188, 158, 248, 207
			}), (nint)ptr3);
			GC.KeepAlive(this);
			flag = true;
			pcbNodeSize = Utf16StringMarshaller.ConvertToManaged(unmanaged);
		}
		finally
		{
			if (flag)
			{
				Utf16StringMarshaller.Free(unmanaged);
			}
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetNodeNameSize(ComWrappers.ComInterfaceDispatch* __this_native, uint* __pcbNodeNameSize_native__param)
	{
		ref uint reference = ref *__pcbNodeNameSize_native__param;
		uint pcbNodeNameSize = 0u;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITmNodeName>(__this_native).GetNodeNameSize(out pcbNodeNameSize);
			num = 0;
			reference = pcbNodeNameSize;
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvMemberFunction) })]
	internal unsafe static int ABI_GetNodeName(ComWrappers.ComInterfaceDispatch* __this_native, uint cbNodeNameBufferSize, ushort** __pcbNodeSize_native__param)
	{
		ref ushort* reference = ref *__pcbNodeSize_native__param;
		string pcbNodeSize = null;
		int num = 0;
		try
		{
			ComWrappers.ComInterfaceDispatch.GetInstance<ITmNodeName>(__this_native).GetNodeName(cbNodeNameBufferSize, out pcbNodeSize);
			num = 0;
			reference = Utf16StringMarshaller.ConvertToUnmanaged(pcbNodeSize);
		}
		catch (Exception e)
		{
			num = ExceptionAsHResultMarshaller<int>.ConvertToUnmanaged(e);
		}
		return num;
	}

	unsafe static _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITmNodeName_003EF83A5FD8143BEEC85074C07122F0B8F7539C69880D2FFFC413D71F3163F0A0BDF__InterfaceImplementation()
	{
		ComWrappers.GetIUnknownImpl(out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITmNodeName_003EF83A5FD8143BEEC85074C07122F0B8F7539C69880D2FFFC413D71F3163F0A0BDF__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->QueryInterface_0), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITmNodeName_003EF83A5FD8143BEEC85074C07122F0B8F7539C69880D2FFFC413D71F3163F0A0BDF__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->AddRef_1), out *(nint*)(&((_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITmNodeName_003EF83A5FD8143BEEC85074C07122F0B8F7539C69880D2FFFC413D71F3163F0A0BDF__InterfaceImplementationVtable*)Unsafe.AsPointer(in Vtable))->Release_2));
		Vtable.GetNodeNameSize_3 = &ABI_GetNodeNameSize;
		Vtable.GetNodeName_4 = &ABI_GetNodeName;
	}
}
