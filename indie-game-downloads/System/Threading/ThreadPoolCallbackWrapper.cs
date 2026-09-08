namespace System.Threading;

internal struct ThreadPoolCallbackWrapper
{
	private Thread _currentThread;

	public static ThreadPoolCallbackWrapper Enter()
	{
		Thread currentThread = Thread.CurrentThread;
		if (!currentThread.IsThreadPoolThread)
		{
			currentThread.IsThreadPoolThread = true;
			ThreadPool.InitializeForThreadPoolThread();
		}
		return new ThreadPoolCallbackWrapper
		{
			_currentThread = currentThread
		};
	}

	public void Exit(bool resetThread = true)
	{
		if (resetThread)
		{
			_currentThread.ResetThreadPoolThread();
		}
	}
}
