using System;

namespace v;

internal class v : _0006
{
	public v()
		: base("Unexpected error occured during a cryptographic operation.")
	{
		base.HResult = -2146233295;
	}

	public v(string message)
		: base(message)
	{
		base.HResult = -2146233295;
	}

	public v(string message, Exception inner)
		: base(message, inner)
	{
		base.HResult = -2146233295;
	}

	public v(string format, string insert)
		: base(string.Format(format, insert))
	{
		base.HResult = -2146233295;
	}
}
