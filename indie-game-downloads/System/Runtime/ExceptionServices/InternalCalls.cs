using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.ExceptionServices;

internal static class InternalCalls
{
	[LibraryImport("QCall", EntryPoint = "SfiInit")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool RhpSfiInit(ref StackFrameIterator pThis, void* pStackwalkCtx, [MarshalAs(UnmanagedType.U1)] bool instructionFault, bool* fIsExceptionIntercepted)
	{
		byte _instructionFault_native = (instructionFault ? ((byte)1) : ((byte)0));
		byte num;
		fixed (StackFrameIterator* _pThis_native = &pThis)
		{
			num = __PInvoke(_pThis_native, pStackwalkCtx, _instructionFault_native, fIsExceptionIntercepted);
		}
		return num != 0;
		[DllImport("QCall", EntryPoint = "SfiInit", ExactSpelling = true)]
		unsafe static extern byte __PInvoke(StackFrameIterator* __pThis_native, void* __pStackwalkCtx_native, byte __instructionFault_native, bool* __fIsExceptionIntercepted_native);
	}

	[LibraryImport("QCall", EntryPoint = "SfiNext")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool RhpSfiNext(ref StackFrameIterator pThis, uint* uExCollideClauseIdx, bool* fUnwoundReversePInvoke, bool* fIsExceptionIntercepted)
	{
		byte num;
		fixed (StackFrameIterator* _pThis_native = &pThis)
		{
			num = __PInvoke(_pThis_native, uExCollideClauseIdx, fUnwoundReversePInvoke, fIsExceptionIntercepted);
		}
		return num != 0;
		[DllImport("QCall", EntryPoint = "SfiNext", ExactSpelling = true)]
		unsafe static extern byte __PInvoke(StackFrameIterator* __pThis_native, uint* __uExCollideClauseIdx_native, bool* __fUnwoundReversePInvoke_native, bool* __fIsExceptionIntercepted_native);
	}

	[LibraryImport("QCall", EntryPoint = "CallFilterFunclet")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool RhpCallFilterFunclet(ObjectHandleOnStack exceptionObj, byte* pFilterIP, void* pvRegDisplay)
	{
		return __PInvoke(exceptionObj, pFilterIP, pvRegDisplay) != 0;
		[DllImport("QCall", EntryPoint = "CallFilterFunclet", ExactSpelling = true)]
		unsafe static extern byte __PInvoke(ObjectHandleOnStack __exceptionObj_native, byte* __pFilterIP_native, void* __pvRegDisplay_native);
	}

	[DllImport("QCall", EntryPoint = "AppendExceptionStackFrame", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "AppendExceptionStackFrame")]
	internal unsafe static extern void RhpAppendExceptionStackFrame(ObjectHandleOnStack exceptionObj, nint ip, nuint sp, int flags, EH.ExInfo* exInfo);

	[LibraryImport("QCall", EntryPoint = "EHEnumInitFromStackFrameIterator")]
	[SuppressGCTransition]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool RhpEHEnumInitFromStackFrameIterator(ref StackFrameIterator pFrameIter, out EH.MethodRegionInfo pMethodRegionInfo, void* pEHEnum)
	{
		pMethodRegionInfo = default(EH.MethodRegionInfo);
		byte num;
		fixed (EH.MethodRegionInfo* _pMethodRegionInfo_native = &pMethodRegionInfo)
		{
			fixed (StackFrameIterator* _pFrameIter_native = &pFrameIter)
			{
				num = __PInvoke(_pFrameIter_native, _pMethodRegionInfo_native, pEHEnum);
			}
		}
		return num != 0;
		[DllImport("QCall", EntryPoint = "EHEnumInitFromStackFrameIterator", ExactSpelling = true)]
		[SuppressGCTransition]
		unsafe static extern byte __PInvoke(StackFrameIterator* __pFrameIter_native, EH.MethodRegionInfo* __pMethodRegionInfo_native, void* __pEHEnum_native);
	}

	[LibraryImport("QCall", EntryPoint = "EHEnumNext")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool RhpEHEnumNext(void* pEHEnum, void* pEHClause)
	{
		return __PInvoke(pEHEnum, pEHClause) != 0;
		[DllImport("QCall", EntryPoint = "EHEnumNext", ExactSpelling = true)]
		unsafe static extern byte __PInvoke(void* __pEHEnum_native, void* __pEHClause_native);
	}
}
