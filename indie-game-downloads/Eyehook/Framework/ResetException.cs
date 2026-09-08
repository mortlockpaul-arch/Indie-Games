using System;

namespace Eyehook.Framework;

public class ResetException : Exception
{
	public ResetException(string message)
		: base(message)
	{
	}
}
