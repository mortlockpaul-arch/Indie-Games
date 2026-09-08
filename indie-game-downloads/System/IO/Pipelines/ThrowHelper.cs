using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.IO.Pipelines;

internal static class ThrowHelper
{
	[DoesNotReturn]
	internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
	{
		throw CreateArgumentOutOfRangeException(argument);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(ExceptionArgument argument)
	{
		return new ArgumentOutOfRangeException(argument.ToString());
	}

	[DoesNotReturn]
	internal static void ThrowArgumentNullException(ExceptionArgument argument)
	{
		throw CreateArgumentNullException(argument);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static ArgumentNullException CreateArgumentNullException(ExceptionArgument argument)
	{
		return new ArgumentNullException(argument.ToString());
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_AlreadyReading()
	{
		throw CreateInvalidOperationException_AlreadyReading();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_AlreadyReading()
	{
		return new InvalidOperationException(System.SR.ReadingIsInProgress);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_NoReadToComplete()
	{
		throw CreateInvalidOperationException_NoReadToComplete();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_NoReadToComplete()
	{
		return new InvalidOperationException(System.SR.NoReadingOperationToComplete);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_NoConcurrentOperation()
	{
		throw CreateInvalidOperationException_NoConcurrentOperation();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_NoConcurrentOperation()
	{
		return new InvalidOperationException(System.SR.ConcurrentOperationsNotSupported);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_GetResultNotCompleted()
	{
		throw CreateInvalidOperationException_GetResultNotCompleted();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_GetResultNotCompleted()
	{
		return new InvalidOperationException(System.SR.GetResultBeforeCompleted);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_NoWritingAllowed()
	{
		throw CreateInvalidOperationException_NoWritingAllowed();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_NoWritingAllowed()
	{
		return new InvalidOperationException(System.SR.WritingAfterCompleted);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_NoReadingAllowed()
	{
		throw CreateInvalidOperationException_NoReadingAllowed();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_NoReadingAllowed()
	{
		return new InvalidOperationException(System.SR.ReadingAfterCompleted);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_InvalidExaminedPosition()
	{
		throw CreateInvalidOperationException_InvalidExaminedPosition();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_InvalidExaminedPosition()
	{
		return new InvalidOperationException(System.SR.InvalidExaminedPosition);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_InvalidExaminedOrConsumedPosition()
	{
		throw CreateInvalidOperationException_InvalidExaminedOrConsumedPosition();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_InvalidExaminedOrConsumedPosition()
	{
		return new InvalidOperationException(System.SR.InvalidExaminedOrConsumedPosition);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_AdvanceToInvalidCursor()
	{
		throw CreateInvalidOperationException_AdvanceToInvalidCursor();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_AdvanceToInvalidCursor()
	{
		return new InvalidOperationException(System.SR.AdvanceToInvalidCursor);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_ResetIncompleteReaderWriter()
	{
		throw CreateInvalidOperationException_ResetIncompleteReaderWriter();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_ResetIncompleteReaderWriter()
	{
		return new InvalidOperationException(System.SR.ReaderAndWriterHasToBeCompleted);
	}

	[DoesNotReturn]
	public static void ThrowOperationCanceledException_ReadCanceled()
	{
		throw CreateOperationCanceledException_ReadCanceled();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static OperationCanceledException CreateOperationCanceledException_ReadCanceled()
	{
		return new OperationCanceledException(System.SR.ReadCanceledOnPipeReader);
	}

	[DoesNotReturn]
	public static void ThrowOperationCanceledException_FlushCanceled()
	{
		throw CreateOperationCanceledException_FlushCanceled();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static OperationCanceledException CreateOperationCanceledException_FlushCanceled()
	{
		return new OperationCanceledException(System.SR.FlushCanceledOnPipeWriter);
	}

	[DoesNotReturn]
	public static void ThrowInvalidOperationException_InvalidZeroByteRead()
	{
		throw CreateInvalidOperationException_InvalidZeroByteRead();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static InvalidOperationException CreateInvalidOperationException_InvalidZeroByteRead()
	{
		return new InvalidOperationException(System.SR.InvalidZeroByteRead);
	}

	[DoesNotReturn]
	public static void ThrowNotSupported_UnflushedBytes()
	{
		throw CreateNotSupportedException_UnflushedBytes();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static NotSupportedException CreateNotSupportedException_UnflushedBytes()
	{
		return new NotSupportedException(System.SR.UnflushedBytesNotSupported);
	}
}
