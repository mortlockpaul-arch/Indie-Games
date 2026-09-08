namespace System.Threading;

internal static class TimeoutHelper
{
	public static long UpdateTimeOut(long startTime, long originalWaitMillisecondsTimeout)
	{
		ulong num = (ulong)(Environment.TickCount64 - startTime);
		if (num > long.MaxValue)
		{
			return 0L;
		}
		long num2 = originalWaitMillisecondsTimeout - (long)num;
		if (num2 <= 0)
		{
			return 0L;
		}
		return num2;
	}
}
