using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace System.Runtime;

internal static class EH
{
	private enum RhEHClauseKind
	{
		RH_EH_CLAUSE_TYPED,
		RH_EH_CLAUSE_FAULT,
		RH_EH_CLAUSE_FILTER,
		RH_EH_CLAUSE_UNUSED
	}

	private struct RhEHClause
	{
		internal RhEHClauseKind _clauseKind;

		internal uint _tryStartOffset;

		internal uint _tryEndOffset;

		internal unsafe byte* _filterAddress;

		internal unsafe byte* _handlerAddress;

		internal unsafe void* _pTargetType;

		internal bool _isSameTry;

		public bool ContainsCodeOffset(uint codeOffset)
		{
			if (codeOffset >= _tryStartOffset)
			{
				return codeOffset < _tryEndOffset;
			}
			return false;
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = 32)]
	private struct EHEnum
	{
		[FieldOffset(0)]
		private nint _dummy;
	}

	internal struct MethodRegionInfo
	{
		internal unsafe byte* _hotStartAddress;

		internal nuint _hotSize;

		internal unsafe byte* _coldStartAddress;

		internal nuint _coldSize;
	}

	[StructLayout(LayoutKind.Explicit, Size = 1232)]
	public struct PAL_LIMITED_CONTEXT
	{
		[FieldOffset(248)]
		internal nint IP;

		[FieldOffset(160)]
		internal nuint FP;
	}

	[Flags]
	internal enum ExKind : byte
	{
		None = 0,
		Throw = 1,
		HardwareFault = 2,
		KindMask = Throw | HardwareFault,
		RethrowFlag = 4,
		SupersededFlag = 8,
		InstructionFaultFlag = 0x10
	}

	[StructLayout(LayoutKind.Explicit)]
	public ref struct ExInfo
	{
		[FieldOffset(0)]
		internal unsafe void* _pPrevExInfo;

		[FieldOffset(168)]
		internal unsafe PAL_LIMITED_CONTEXT* _pExContext;

		[FieldOffset(176)]
		private object _exception;

		[FieldOffset(184)]
		internal ExKind _kind;

		[FieldOffset(185)]
		internal byte _passNumber;

		[FieldOffset(188)]
		internal uint _idxCurClause;

		[FieldOffset(192)]
		internal StackFrameIterator _frameIter;

		[FieldOffset(520)]
		internal volatile nuint _notifyDebuggerSP;

		[FieldOffset(592)]
		internal unsafe volatile byte* _pCatchHandler;

		[FieldOffset(600)]
		internal volatile nuint _handlingFrameSP;

		internal object ThrownException => _exception;

		internal void Init(object exceptionObj, bool instructionFault = false)
		{
			_exception = exceptionObj;
			if (instructionFault)
			{
				_kind |= ExKind.InstructionFaultFlag;
			}
			_notifyDebuggerSP = UIntPtr.Zero;
		}

		internal void Init(object exceptionObj, ref ExInfo rethrownExInfo)
		{
			_exception = exceptionObj;
			_kind = rethrownExInfo._kind | ExKind.RethrowFlag;
			_notifyDebuggerSP = UIntPtr.Zero;
		}
	}

	internal static nuint MaxSP => unchecked((nuint)(-1));

	[DoesNotReturn]
	internal static void FallbackFailFast(RhFailFastReason reason, object unhandledException)
	{
		Debugger.Break();
		Environment.FailFast(reason.ToString());
	}

	[DoesNotReturn]
	internal static void FailFastViaClasslib(RhFailFastReason reason, object unhandledException, nint classlibAddress)
	{
		FallbackFailFast(reason, unhandledException);
	}

	private static void OnUnhandledExceptionViaClassLib(object exception)
	{
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void UnhandledExceptionFailFastViaClasslib(RhFailFastReason reason, object unhandledException, nint classlibAddress, ref ExInfo exInfo)
	{
		FailFastViaClasslib(reason, unhandledException, classlibAddress);
	}

	private unsafe static void AppendExceptionStackFrameViaClasslib(object exception, nint ip, nuint sp, ref ExInfo exInfo, ref bool isFirstRethrowFrame, ref bool isFirstFrame)
	{
		int flags = (isFirstFrame ? 1 : 0) | (isFirstRethrowFrame ? 2 : 0);
		fixed (ExInfo* exInfo2 = &exInfo)
		{
			InternalCalls.RhpAppendExceptionStackFrame(ObjectHandleOnStack.Create(ref exception), ip, sp, flags, exInfo2);
		}
		isFirstRethrowFrame = false;
		isFirstFrame = false;
	}

	internal static Exception GetClasslibException(ExceptionIDs id, nint address)
	{
		object obj = id switch
		{
			ExceptionIDs.AccessViolation => new AccessViolationException(), 
			ExceptionIDs.Arithmetic => new ArithmeticException(), 
			ExceptionIDs.AmbiguousImplementation => new AmbiguousImplementationException(), 
			ExceptionIDs.ArrayTypeMismatch => new ArrayTypeMismatchException(), 
			ExceptionIDs.DataMisaligned => new DataMisalignedException(), 
			ExceptionIDs.DivideByZero => new DivideByZeroException(), 
			ExceptionIDs.EntrypointNotFound => new EntryPointNotFoundException(), 
			ExceptionIDs.IndexOutOfRange => new IndexOutOfRangeException(), 
			ExceptionIDs.InvalidCast => new InvalidCastException(), 
			ExceptionIDs.NullReference => new NullReferenceException(), 
			ExceptionIDs.OutOfMemory => new OutOfMemoryException(), 
			ExceptionIDs.Overflow => new OverflowException(), 
			ExceptionIDs.IllegalInstruction => new ExecutionEngineException("Illegal instruction: Attempted to execute an instruction code not defined by the processor."), 
			ExceptionIDs.PrivilegedInstruction => new ExecutionEngineException("Privileged instruction: Attempted to execute an instruction code that cannot be executed in user mode."), 
			ExceptionIDs.InPageError => new ExecutionEngineException("In page error: Attempted to access a memory page that is not present, and the system is unable to load the page. For example, this exception might occur if a network connection is lost while running a program over a network."), 
			_ => null, 
		};
		if (obj == null)
		{
			FailFastViaClasslib(RhFailFastReason.InternalError, null, address);
		}
		return (Exception)obj;
	}

	[StackTraceHidden]
	public unsafe static void RhThrowHwEx(uint exceptionCode, ref ExInfo exInfo)
	{
		nint iP = exInfo._pExContext->IP;
		bool instructionFault = true;
		ExceptionIDs exceptionIDs = (ExceptionIDs)0;
		Exception exceptionObj = null;
		switch (exceptionCode)
		{
		case 0u:
			exceptionIDs = ExceptionIDs.NullReference;
			break;
		case 66u:
			instructionFault = false;
			exceptionIDs = ExceptionIDs.NullReference;
			break;
		case 2147483650u:
			exceptionIDs = ExceptionIDs.DataMisaligned;
			break;
		case 3221225477u:
			exceptionIDs = ExceptionIDs.AccessViolation;
			break;
		case 3221225620u:
			exceptionIDs = ExceptionIDs.DivideByZero;
			break;
		case 3221225621u:
			exceptionIDs = ExceptionIDs.Overflow;
			break;
		case 3221225501u:
			exceptionIDs = ExceptionIDs.IllegalInstruction;
			break;
		case 3221225478u:
			exceptionIDs = ExceptionIDs.InPageError;
			break;
		case 3221225622u:
			exceptionIDs = ExceptionIDs.PrivilegedInstruction;
			break;
		default:
			FailFastViaClasslib(RhFailFastReason.InternalError, null, iP);
			break;
		}
		if (exceptionIDs != 0)
		{
			exceptionObj = GetClasslibException(exceptionIDs, iP);
		}
		exInfo.Init(exceptionObj, instructionFault);
		DispatchEx(ref exInfo._frameIter, ref exInfo);
	}

	[StackTraceHidden]
	public unsafe static void RhThrowEx(object exceptionObj, ref ExInfo exInfo)
	{
		if (exceptionObj == null)
		{
			nint iP = exInfo._pExContext->IP;
			exceptionObj = GetClasslibException(ExceptionIDs.NullReference, iP);
		}
		exInfo.Init(exceptionObj);
		DispatchEx(ref exInfo._frameIter, ref exInfo);
	}

	[StackTraceHidden]
	public static void RhRethrow(ref ExInfo activeExInfo, ref ExInfo exInfo)
	{
		object thrownException = activeExInfo.ThrownException;
		exInfo.Init(thrownException, ref activeExInfo);
		DispatchEx(ref exInfo._frameIter, ref exInfo);
	}

	[StackTraceHidden]
	private unsafe static void DispatchEx(scoped ref StackFrameIterator frameIter, ref ExInfo exInfo)
	{
		object thrownException = exInfo.ThrownException;
		nuint handlingFrameSP = MaxSP;
		byte* ptr = null;
		uint tryRegionIdx = uint.MaxValue;
		bool isFirstRethrowFrame = (exInfo._kind & ExKind.RethrowFlag) != 0;
		bool isFirstFrame = true;
		bool flag = false;
		byte* ptr2 = null;
		byte* classlibAddress = null;
		nuint prevFramePtr = UIntPtr.Zero;
		bool flag2 = false;
		nint zero = IntPtr.Zero;
		_ = IntPtr.Zero;
		bool flag3 = frameIter.Init(exInfo._pExContext, (exInfo._kind & ExKind.InstructionFaultFlag) != 0, &flag);
		uint num = uint.MaxValue;
		while (flag3 && !flag2 && !flag)
		{
			ptr2 = frameIter.ControlPC;
			classlibAddress = frameIter.OriginalControlPC;
			if (num == uint.MaxValue)
			{
				UpdateStackTrace(thrownException, exInfo._frameIter.FramePointer, (nint)frameIter.OriginalControlPC, frameIter.SP, ref isFirstRethrowFrame, ref prevFramePtr, ref isFirstFrame, ref exInfo);
			}
			if (FindFirstPassHandler(thrownException, num, ref frameIter, out tryRegionIdx, out var pHandler))
			{
				handlingFrameSP = frameIter.SP;
				ptr = pHandler;
				break;
			}
			flag3 = frameIter.Next(&num, &flag2, &flag);
		}
		if (flag2)
		{
			handlingFrameSP = frameIter.SP;
			tryRegionIdx = uint.MaxValue;
		}
		if (ptr == null && zero == IntPtr.Zero && !flag && !flag2)
		{
			OnUnhandledExceptionViaClassLib(thrownException);
			UnhandledExceptionFailFastViaClasslib(RhFailFastReason.UnhandledException, thrownException, (nint)classlibAddress, ref exInfo);
		}
		exInfo._pCatchHandler = ptr;
		exInfo._handlingFrameSP = handlingFrameSP;
		exInfo._idxCurClause = tryRegionIdx;
	}

	private unsafe static uint CalculateCodeOffset(byte* pbControlPC, in MethodRegionInfo methodRegionInfo)
	{
		uint num = (uint)(pbControlPC - methodRegionInfo._hotStartAddress);
		if (methodRegionInfo._coldSize != 0 && num >= methodRegionInfo._hotSize)
		{
			num = (uint)(methodRegionInfo._hotSize + (nuint)(nint)(pbControlPC - methodRegionInfo._coldStartAddress));
		}
		return num;
	}

	private static void UpdateStackTrace(object exceptionObj, nuint curFramePtr, nint ip, nuint sp, ref bool isFirstRethrowFrame, ref nuint prevFramePtr, ref bool isFirstFrame, ref ExInfo exInfo)
	{
		AppendExceptionStackFrameViaClasslib(exceptionObj, ip, sp, ref exInfo, ref isFirstRethrowFrame, ref isFirstFrame);
		prevFramePtr = curFramePtr;
	}

	[StackTraceHidden]
	private unsafe static bool FindFirstPassHandler(object exception, uint idxStart, ref StackFrameIterator frameIter, out uint tryRegionIdx, out byte* pHandler)
	{
		pHandler = null;
		tryRegionIdx = uint.MaxValue;
		Unsafe.SkipInit(out EHEnum eHEnum);
		if (!InternalCalls.RhpEHEnumInitFromStackFrameIterator(ref frameIter, out var pMethodRegionInfo, &eHEnum))
		{
			return false;
		}
		uint codeOffset = CalculateCodeOffset(frameIter.ControlPC, in pMethodRegionInfo);
		uint num = 0u;
		uint num2 = 0u;
		Unsafe.SkipInit(out RhEHClause rhEHClause);
		for (uint num3 = 0u; InternalCalls.RhpEHEnumNext(&eHEnum, &rhEHClause); num3++)
		{
			if (idxStart != uint.MaxValue)
			{
				if (num3 <= idxStart)
				{
					num = rhEHClause._tryStartOffset;
					num2 = rhEHClause._tryEndOffset;
					continue;
				}
				if (rhEHClause._tryStartOffset == num && rhEHClause._tryEndOffset == num2 && rhEHClause._isSameTry)
				{
					continue;
				}
				idxStart = uint.MaxValue;
			}
			RhEHClauseKind clauseKind = rhEHClause._clauseKind;
			if ((clauseKind != RhEHClauseKind.RH_EH_CLAUSE_TYPED && clauseKind != RhEHClauseKind.RH_EH_CLAUSE_FILTER) || !rhEHClause.ContainsCodeOffset(codeOffset))
			{
				continue;
			}
			if (clauseKind == RhEHClauseKind.RH_EH_CLAUSE_TYPED)
			{
				if (ShouldTypedClauseCatchThisException(exception, (MethodTable*)rhEHClause._pTargetType, !frameIter.IsRuntimeWrappedExceptions))
				{
					pHandler = rhEHClause._handlerAddress;
					tryRegionIdx = num3;
					return true;
				}
				continue;
			}
			byte* filterAddress = rhEHClause._filterAddress;
			if (InternalCalls.RhpCallFilterFunclet(ObjectHandleOnStack.Create(ref exception), filterAddress, frameIter.RegisterSet))
			{
				pHandler = rhEHClause._handlerAddress;
				tryRegionIdx = num3;
				return true;
			}
		}
		return false;
	}

	private unsafe static bool ShouldTypedClauseCatchThisException(object exception, MethodTable* pClauseType, bool tryUnwrapException)
	{
		if (tryUnwrapException && exception is RuntimeWrappedException ex)
		{
			exception = ex.WrappedException;
		}
		for (MethodTable* ptr = RuntimeHelpers.GetMethodTable(exception); ptr != null; ptr = ptr->ParentMethodTable)
		{
			if (pClauseType == ptr)
			{
				return true;
			}
		}
		return false;
	}
}
