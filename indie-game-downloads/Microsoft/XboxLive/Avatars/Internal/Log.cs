using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public abstract class Log
{
	public object Sender { get; set; }

	public string Message { get; set; }

	public DateTime TimeStamp { get; set; }

	public Log()
	{
		TimeStamp = DateTime.Now;
	}

	public Log(object sender, string message)
	{
		TimeStamp = DateTime.Now;
		Sender = sender;
		Message = message;
	}
}
