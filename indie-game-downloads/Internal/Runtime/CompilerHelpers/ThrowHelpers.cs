using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Internal.Runtime.CompilerHelpers;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class ThrowHelpers
{
	[DllImport("QCall", EntryPoint = "ExceptionNative_ThrowAmbiguousResolutionException", ExactSpelling = true)]
	[DoesNotReturn]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_ThrowAmbiguousResolutionException")]
	private unsafe static extern void ThrowAmbiguousResolutionException(MethodTable* targetType, MethodTable* interfaceType, void* methodDesc);

	[DoesNotReturn]
	[DebuggerHidden]
	internal unsafe static void ThrowAmbiguousResolutionException(void* method, void* interfaceType, void* targetType)
	{
		ThrowAmbiguousResolutionException((MethodTable*)targetType, (MethodTable*)interfaceType, method);
	}

	[DllImport("QCall", EntryPoint = "ExceptionNative_ThrowEntryPointNotFoundException", ExactSpelling = true)]
	[DoesNotReturn]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_ThrowEntryPointNotFoundException")]
	private unsafe static extern void ThrowEntryPointNotFoundException(MethodTable* targetType, MethodTable* interfaceType, void* methodDesc);

	[DoesNotReturn]
	[DebuggerHidden]
	internal unsafe static void ThrowEntryPointNotFoundException(void* method, void* interfaceType, void* targetType)
	{
		ThrowEntryPointNotFoundException((MethodTable*)targetType, (MethodTable*)interfaceType, method);
	}

	[DllImport("QCall", EntryPoint = "ExceptionNative_ThrowMethodAccessException", ExactSpelling = true)]
	[DoesNotReturn]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_ThrowMethodAccessException")]
	private unsafe static extern void ThrowMethodAccessExceptionInternal(void* caller, void* callee);

	[DoesNotReturn]
	[DebuggerHidden]
	internal unsafe static void ThrowMethodAccessException(void* caller, void* callee)
	{
		ThrowMethodAccessExceptionInternal(caller, callee);
	}

	[DllImport("QCall", EntryPoint = "ExceptionNative_ThrowFieldAccessException", ExactSpelling = true)]
	[DoesNotReturn]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_ThrowFieldAccessException")]
	private unsafe static extern void ThrowFieldAccessExceptionInternal(void* caller, void* callee);

	[DoesNotReturn]
	[DebuggerHidden]
	internal unsafe static void ThrowFieldAccessException(void* caller, void* callee)
	{
		ThrowFieldAccessExceptionInternal(caller, callee);
	}

	[DllImport("QCall", EntryPoint = "ExceptionNative_ThrowClassAccessException", ExactSpelling = true)]
	[DoesNotReturn]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_ThrowClassAccessException")]
	private unsafe static extern void ThrowClassAccessExceptionInternal(void* caller, void* callee);

	[DoesNotReturn]
	[DebuggerHidden]
	internal unsafe static void ThrowClassAccessException(void* caller, void* callee)
	{
		ThrowClassAccessExceptionInternal(caller, callee);
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowNullReferenceException()
	{
		throw new NullReferenceException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowArgumentException()
	{
		throw new ArgumentException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowArgumentOutOfRangeException()
	{
		throw new ArgumentOutOfRangeException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowDivideByZeroException()
	{
		throw new DivideByZeroException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowIndexOutOfRangeException()
	{
		throw new IndexOutOfRangeException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowOverflowException()
	{
		throw new OverflowException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowPlatformNotSupportedException()
	{
		throw new PlatformNotSupportedException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowNotImplementedException()
	{
		throw new NotImplementedException();
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowTypeNotSupportedException()
	{
		throw new NotSupportedException(SR.Arg_TypeNotSupported);
	}

	[DoesNotReturn]
	[DebuggerHidden]
	internal static void ThrowVerificationException(int ilOffset)
	{
		throw new VerificationException();
	}
}
