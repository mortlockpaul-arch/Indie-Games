namespace System.Diagnostics;

public sealed class UnreachableException : Exception
{
	public UnreachableException()
		: base(SR.Arg_UnreachableException)
	{
	}

	public UnreachableException(string? message)
		: base(message ?? SR.Arg_UnreachableException)
	{
	}

	public UnreachableException(string? message, Exception? innerException)
		: base(message ?? SR.Arg_UnreachableException, innerException)
	{
	}
}
