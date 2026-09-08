using System.CodeDom.Compiler;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.CustomMarshalers;
using System.Runtime.Versioning;

namespace System.StubHelpers;

internal static class StubHelpers
{
	[ThreadStatic]
	private static Exception s_pendingExceptionObject;

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern nint GetDelegateTarget(Delegate pThis);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void ClearLastError();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void SetLastError();

	[DllImport("QCall", EntryPoint = "StubHelpers_ThrowInteropParamException", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_ThrowInteropParamException")]
	internal static extern void ThrowInteropParamException(int resID, int paramIdx);

	internal static nint AddToCleanupList(ref CleanupWorkListElement pCleanupWorkList, SafeHandle handle)
	{
		SafeHandleCleanupWorkListElement safeHandleCleanupWorkListElement = new SafeHandleCleanupWorkListElement(handle);
		CleanupWorkListElement.AddToCleanupList(ref pCleanupWorkList, safeHandleCleanupWorkListElement);
		return safeHandleCleanupWorkListElement.AddRef();
	}

	internal static void KeepAliveViaCleanupList(ref CleanupWorkListElement pCleanupWorkList, object obj)
	{
		KeepAliveCleanupWorkListElement newElement = new KeepAliveCleanupWorkListElement(obj);
		CleanupWorkListElement.AddToCleanupList(ref pCleanupWorkList, newElement);
	}

	internal static void DestroyCleanupList(ref CleanupWorkListElement pCleanupWorkList)
	{
		if (pCleanupWorkList != null)
		{
			pCleanupWorkList.Destroy();
			pCleanupWorkList = null;
		}
	}

	internal static Exception GetHRExceptionObject(int hr)
	{
		Exception? exceptionForHR = Marshal.GetExceptionForHR(hr);
		exceptionForHR.InternalPreserveStackTrace();
		return exceptionForHR;
	}

	internal unsafe static Exception GetCOMHRExceptionObject(int hr, nint pCPCMD, nint pUnk)
	{
		RuntimeType runtimeType = RuntimeTypeHandle.GetRuntimeType(GetComInterfaceFromMethodDesc(pCPCMD));
		Exception? exceptionForHR = Marshal.GetExceptionForHR(hr, runtimeType.GUID, pUnk);
		exceptionForHR.InternalPreserveStackTrace();
		return exceptionForHR;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern MethodTable* GetComInterfaceFromMethodDesc(nint pCPCMD);

	internal static Exception GetPendingExceptionObject()
	{
		Exception ex = s_pendingExceptionObject;
		if (ex != null)
		{
			ex.InternalPreserveStackTrace();
			s_pendingExceptionObject = null;
		}
		return ex;
	}

	[DllImport("QCall", EntryPoint = "StubHelpers_CreateCustomMarshaler", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_CreateCustomMarshaler")]
	internal static extern void CreateCustomMarshaler(nint pMD, int paramToken, nint hndManagedType, ObjectHandleOnStack customMarshaler);

	[SupportedOSPlatform("windows")]
	internal static object GetIEnumeratorToEnumVariantMarshaler()
	{
		return EnumeratorToEnumVariantMarshaler.GetInstance(string.Empty);
	}

	internal static object CreateCustomMarshaler(nint pMD, int paramToken, nint hndManagedType)
	{
		_ = 1;
		if (hndManagedType == typeof(IEnumerator).TypeHandle.Value)
		{
			return GetIEnumeratorToEnumVariantMarshaler();
		}
		object o = null;
		CreateCustomMarshaler(pMD, paramToken, hndManagedType, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	internal static nint SafeHandleAddRef(SafeHandle pHandle, ref bool success)
	{
		if (pHandle == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.pHandle, ExceptionResource.ArgumentNull_SafeHandle);
		}
		pHandle.DangerousAddRef(ref success);
		return pHandle.DangerousGetHandle();
	}

	internal static void SafeHandleRelease(SafeHandle pHandle)
	{
		if (pHandle == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.pHandle, ExceptionResource.ArgumentNull_SafeHandle);
		}
		pHandle.DangerousRelease();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern nint GetCOMIPFromRCW(object objSrc, nint pCPCMD, out nint ppTarget);

	[LibraryImport("QCall", EntryPoint = "StubHelpers_GetCOMIPFromRCWSlow")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static nint GetCOMIPFromRCWSlow(ObjectHandleOnStack objSrc, nint pCPCMD, out nint ppTarget, [MarshalAs(UnmanagedType.Bool)] out bool pfNeedsRelease)
	{
		ppTarget = 0;
		pfNeedsRelease = false;
		nint result;
		Unsafe.SkipInit(out int num);
		fixed (nint* _ppTarget_native = &ppTarget)
		{
			result = __PInvoke(objSrc, pCPCMD, _ppTarget_native, &num);
		}
		pfNeedsRelease = num != 0;
		return result;
		[DllImport("QCall", EntryPoint = "StubHelpers_GetCOMIPFromRCWSlow", ExactSpelling = true)]
		unsafe static extern nint __PInvoke(ObjectHandleOnStack __objSrc_native, nint __pCPCMD_native, nint* __ppTarget_native, int* __pfNeedsRelease_native);
	}

	internal static nint GetCOMIPFromRCW(object objSrc, nint pCPCMD, out nint ppTarget, out bool pfNeedsRelease)
	{
		nint cOMIPFromRCW = GetCOMIPFromRCW(objSrc, pCPCMD, out ppTarget);
		if (cOMIPFromRCW != IntPtr.Zero)
		{
			pfNeedsRelease = false;
			return cOMIPFromRCW;
		}
		return GetCOMIPFromRCWWorker(objSrc, pCPCMD, out ppTarget, out pfNeedsRelease);
		[MethodImpl(MethodImplOptions.NoInlining)]
		static nint GetCOMIPFromRCWWorker(object o, nint pCPCMD2, out nint ppTarget2, out bool pfNeedsRelease2)
		{
			return GetCOMIPFromRCWSlow(ObjectHandleOnStack.Create(ref o), pCPCMD2, out ppTarget2, out pfNeedsRelease2);
		}
	}

	[DllImport("QCall", EntryPoint = "StubHelpers_ProfilerBeginTransitionCallback", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_ProfilerBeginTransitionCallback")]
	internal unsafe static extern void* ProfilerBeginTransitionCallback(void* pTargetMD);

	[DllImport("QCall", EntryPoint = "StubHelpers_ProfilerEndTransitionCallback", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_ProfilerEndTransitionCallback")]
	internal unsafe static extern void ProfilerEndTransitionCallback(void* pTargetMD);

	internal static void CheckStringLength(int length)
	{
		CheckStringLength((uint)length);
	}

	internal static void CheckStringLength(uint length)
	{
		if (length > 2147483632)
		{
			throw new MarshalDirectiveException(SR.Marshaler_StringTooLong);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool TryGetStringTrailByte(string str, out byte data);

	internal static void SetStringTrailByte(string str, byte data)
	{
		SetStringTrailByte(new StringHandleOnStack(ref str), data);
	}

	[DllImport("QCall", EntryPoint = "StubHelpers_SetStringTrailByte", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_SetStringTrailByte")]
	private static extern void SetStringTrailByte(StringHandleOnStack str, byte data);

	internal unsafe static void FmtClassUpdateNativeInternal(object obj, byte* pNative, ref CleanupWorkListElement pCleanupWorkList)
	{
		Unsafe.SkipInit(out delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void> obj2);
		Unsafe.SkipInit(out nuint len);
		Marshal.TryGetStructMarshalStub((nint)RuntimeHelpers.GetMethodTable(obj), &obj2, &len);
		if (obj2 != (delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>)null)
		{
			obj2(ref obj.GetRawData(), pNative, 0, ref pCleanupWorkList);
		}
		else
		{
			SpanHelpers.Memmove(ref *pNative, ref obj.GetRawData(), len);
		}
	}

	internal unsafe static void FmtClassUpdateCLRInternal(object obj, byte* pNative)
	{
		Unsafe.SkipInit(out delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void> obj2);
		Unsafe.SkipInit(out nuint len);
		Marshal.TryGetStructMarshalStub((nint)RuntimeHelpers.GetMethodTable(obj), &obj2, &len);
		if (obj2 != (delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>)null)
		{
			obj2(ref obj.GetRawData(), pNative, 1, ref Unsafe.NullRef<CleanupWorkListElement>());
		}
		else
		{
			SpanHelpers.Memmove(ref obj.GetRawData(), ref *pNative, len);
		}
	}

	internal unsafe static void LayoutDestroyNativeInternal(object obj, byte* pNative)
	{
		Unsafe.SkipInit(out delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void> obj2);
		Unsafe.SkipInit(out nuint num);
		Marshal.TryGetStructMarshalStub((nint)RuntimeHelpers.GetMethodTable(obj), &obj2, &num);
		if (obj2 != (delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>)null)
		{
			obj2(ref obj.GetRawData(), pNative, 2, ref Unsafe.NullRef<CleanupWorkListElement>());
		}
	}

	[DllImport("QCall", EntryPoint = "StubHelpers_MarshalToManagedVaList", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_MarshalToManagedVaList")]
	internal static extern void MarshalToManagedVaList(nint va_list, nint pArgIterator);

	[DllImport("QCall", EntryPoint = "StubHelpers_MarshalToUnmanagedVaList", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_MarshalToUnmanagedVaList")]
	internal static extern void MarshalToUnmanagedVaList(nint va_list, uint vaListSize, nint pArgIterator);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern uint CalcVaListSize(nint va_list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void LogPinnedArgument(nint localDesc, nint nativeArg);

	[DllImport("QCall", EntryPoint = "StubHelpers_ValidateObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_ValidateObject")]
	private static extern void ValidateObject(ObjectHandleOnStack obj, nint pMD);

	internal static void ValidateObject(object obj, nint pMD)
	{
		ValidateObject(ObjectHandleOnStack.Create(ref obj), pMD);
	}

	[DllImport("QCall", EntryPoint = "StubHelpers_ValidateByref", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_ValidateByref")]
	internal static extern void ValidateByref(nint byref, nint pMD);

	[Intrinsic]
	internal static nint GetStubContext()
	{
		throw new UnreachableException();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void MulticastDebuggerTraceHelper(object o, int count)
	{
		MulticastDebuggerTraceHelperQCall(ObjectHandleOnStack.Create(ref o), count);
	}

	[DllImport("QCall", EntryPoint = "StubHelpers_MulticastDebuggerTraceHelper", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "StubHelpers_MulticastDebuggerTraceHelper")]
	private static extern void MulticastDebuggerTraceHelperQCall(ObjectHandleOnStack obj, int count);

	[Intrinsic]
	internal static nint NextCallReturnAddress()
	{
		throw new UnreachableException();
	}

	[Intrinsic]
	internal static Continuation AsyncCallContinuation()
	{
		throw new UnreachableException();
	}
}
