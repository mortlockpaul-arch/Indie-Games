using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public class LogEventArgs : EventArgs
{
	public Log Result { get; set; }

	public LogEventArgs(Log result)
	{
		Result = result;
	}
}
