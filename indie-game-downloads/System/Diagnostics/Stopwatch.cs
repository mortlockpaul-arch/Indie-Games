using System.Runtime.CompilerServices;

namespace System.Diagnostics;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Stopwatch
{
	private long _elapsed;

	private long _startTimeStamp;

	private bool _isRunning;

	public static readonly long Frequency = GetFrequency();

	public static readonly bool IsHighResolution = true;

	private static readonly double s_tickFrequency = 10000000.0 / (double)Frequency;

	public bool IsRunning => _isRunning;

	public TimeSpan Elapsed => new TimeSpan(ElapsedTimeSpanTicks);

	public long ElapsedMilliseconds => ElapsedTimeSpanTicks / 10000;

	public long ElapsedTicks
	{
		get
		{
			long num = _elapsed;
			if (_isRunning)
			{
				num += GetTimestamp() - _startTimeStamp;
			}
			return num;
		}
	}

	private long ElapsedTimeSpanTicks => (long)((double)ElapsedTicks * s_tickFrequency);

	private string DebuggerDisplay => $"{Elapsed} (IsRunning = {_isRunning})";

	public void Start()
	{
		if (!_isRunning)
		{
			_startTimeStamp = GetTimestamp();
			_isRunning = true;
		}
	}

	public static Stopwatch StartNew()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		return stopwatch;
	}

	public void Stop()
	{
		if (_isRunning)
		{
			_elapsed += GetTimestamp() - _startTimeStamp;
			_isRunning = false;
		}
	}

	public void Reset()
	{
		_elapsed = 0L;
		_startTimeStamp = 0L;
		_isRunning = false;
	}

	public void Restart()
	{
		_elapsed = 0L;
		_startTimeStamp = GetTimestamp();
		_isRunning = true;
	}

	public override string ToString()
	{
		return Elapsed.ToString();
	}

	public static TimeSpan GetElapsedTime(long startingTimestamp)
	{
		return GetElapsedTime(startingTimestamp, GetTimestamp());
	}

	public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp)
	{
		return new TimeSpan((long)((double)(endingTimestamp - startingTimestamp) * s_tickFrequency));
	}

	private unsafe static long GetFrequency()
	{
		Unsafe.SkipInit(out long result);
		Interop.Kernel32.QueryPerformanceFrequency(&result);
		return result;
	}

	public unsafe static long GetTimestamp()
	{
		Unsafe.SkipInit(out long result);
		Interop.Kernel32.QueryPerformanceCounter(&result);
		return result;
	}
}
