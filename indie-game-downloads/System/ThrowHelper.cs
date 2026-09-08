using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System;

[StackTraceHidden]
internal static class ThrowHelper
{
	[DoesNotReturn]
	internal static void ThrowOverflowException()
	{
		throw new OverflowException();
	}

	[DoesNotReturn]
	internal static void ThrowNotSupportedException()
	{
		throw new NotSupportedException();
	}

	[DoesNotReturn]
	internal static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
	{
		throw new ArgumentOutOfRangeException("value", System.SR.ArgumentOutOfRange_NeedNonNegNum);
	}

	[DoesNotReturn]
	internal static void ThrowFormatException_BadFormatSpecifier()
	{
		throw new FormatException(System.SR.Argument_BadFormatSpecifier);
	}
}
