using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace System.Runtime;

[StructLayout(LayoutKind.Explicit, Size = 328)]
internal struct StackFrameIterator
{
	[FieldOffset(32)]
	private unsafe REGDISPLAY* _pRegDisplay;

	[FieldOffset(320)]
	internal unsafe byte* ControlPC;

	[FieldOffset(298)]
	private byte _IsRuntimeWrappedExceptions;

	internal unsafe byte* OriginalControlPC => (byte*)_pRegDisplay->ControlPC;

	internal unsafe void* RegisterSet => _pRegDisplay;

	internal unsafe nuint SP => _pRegDisplay->SP;

	internal unsafe nuint FramePointer => _pRegDisplay->m_pCurrentContext->FP;

	internal bool IsRuntimeWrappedExceptions => _IsRuntimeWrappedExceptions != 0;

	[StackTraceHidden]
	internal unsafe bool Init(EH.PAL_LIMITED_CONTEXT* pStackwalkCtx, bool instructionFault = false, bool* fIsExceptionIntercepted = null)
	{
		return InternalCalls.RhpSfiInit(ref this, pStackwalkCtx, instructionFault, fIsExceptionIntercepted);
	}

	internal unsafe bool Next(uint* uExCollideClauseIdx, bool* fUnwoundReversePInvoke, bool* fIsExceptionIntercepted)
	{
		return InternalCalls.RhpSfiNext(ref this, uExCollideClauseIdx, fUnwoundReversePInvoke, fIsExceptionIntercepted);
	}
}
