using System;

namespace T;

internal class D : _6
{
	public D()
		: base("Unexpected error occured during a cryptographic operation.")
	{
		base.HResult = -2146233295;
	}

	public D(string message)
		: base(message)
	{
		base.HResult = -2146233295;
	}

	public D(string message, Exception inner)
		: base(message, inner)
	{
		base.HResult = -2146233295;
	}

	public D(string format, string insert)
		: base(string.Format(format, insert))
	{
		base.HResult = -2146233295;
	}
}
