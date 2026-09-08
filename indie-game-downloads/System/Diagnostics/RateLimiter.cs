using System.Threading;

namespace System.Diagnostics;

internal sealed class RateLimiter
{
	private readonly int _maxOperationsPerSecond;

	private readonly Stopwatch _stopwatch;

	private readonly long _ticksPerSecond;

	private int _currentOperationCount;

	private long _intervalStartTicks;

	internal RateLimiter(int maxOperationsPerSecond)
	{
		_maxOperationsPerSecond = maxOperationsPerSecond;
		_stopwatch = new Stopwatch();
		_stopwatch.Start();
		_intervalStartTicks = _stopwatch.ElapsedTicks;
		_currentOperationCount = 0;
		_ticksPerSecond = Stopwatch.Frequency;
	}

	internal bool TryAcquire()
	{
		while (true)
		{
			long elapsedTicks = _stopwatch.ElapsedTicks;
			long num = Interlocked.Read(in _intervalStartTicks);
			int num2 = Volatile.Read(in _currentOperationCount);
			if (elapsedTicks - num < _ticksPerSecond)
			{
				break;
			}
			if (Interlocked.CompareExchange(ref _currentOperationCount, 1, num2) == num2)
			{
				Interlocked.CompareExchange(ref _intervalStartTicks, elapsedTicks, num);
				return true;
			}
		}
		return Interlocked.Increment(ref _currentOperationCount) <= _maxOperationsPerSecond;
	}
}
