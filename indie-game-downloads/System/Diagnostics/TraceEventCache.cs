using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Diagnostics;

public class TraceEventCache
{
	private long _timeStamp = -1L;

	private DateTime _dateTime = DateTime.MinValue;

	[CompilerGenerated]
	private string _003CCallstack_003Ek__BackingField;

	public DateTime DateTime
	{
		get
		{
			if (_dateTime == DateTime.MinValue)
			{
				_dateTime = DateTime.UtcNow;
			}
			return _dateTime;
		}
	}

	public int ProcessId => Environment.ProcessId;

	public string ThreadId => Environment.CurrentManagedThreadId.ToString(CultureInfo.InvariantCulture);

	public long Timestamp
	{
		get
		{
			if (_timeStamp == -1)
			{
				_timeStamp = Stopwatch.GetTimestamp();
			}
			return _timeStamp;
		}
	}

	public string Callstack => _003CCallstack_003Ek__BackingField ?? (_003CCallstack_003Ek__BackingField = Environment.StackTrace);

	public Stack LogicalOperationStack => Trace.CorrelationManager.LogicalOperationStack;
}
