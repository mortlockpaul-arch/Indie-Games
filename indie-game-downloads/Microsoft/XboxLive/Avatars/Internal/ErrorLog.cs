using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class ErrorLog : Log
{
	public Exception Error { get; set; }

	public ErrorLog(Exception error)
	{
		Error = error;
	}
}
