namespace System.Timers;

public sealed class ElapsedEventArgs : EventArgs
{
	public DateTime SignalTime { get; }

	public ElapsedEventArgs(DateTime signalTime)
	{
		SignalTime = signalTime;
	}
}
