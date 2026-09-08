using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public static class Logger
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	public static EventHandler<LogEventArgs> LogReceived;

	public static event EventHandler<LogEventArgs> LogReceived
	{
		[CompilerGenerated]
		add
		{
			EventHandler<LogEventArgs> eventHandler = Logger.LogReceived;
			EventHandler<LogEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<LogEventArgs> value2 = (EventHandler<LogEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref Logger.LogReceived, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler<LogEventArgs> eventHandler = Logger.LogReceived;
			EventHandler<LogEventArgs> eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler<LogEventArgs> value2 = (EventHandler<LogEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref Logger.LogReceived, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public static void Log(Log log)
	{
		if (log is DebugLog)
		{
		}
		OnLogReceived(log.Sender, log);
	}

	public static void OnLogReceived(object sender, Log log)
	{
		if (Logger.LogReceived != null)
		{
			Logger.LogReceived(sender, new LogEventArgs(log));
		}
	}
}
