using System;
using System.Threading;

namespace BEPUphysics.Threading;

internal class ParallelLoopWorker : IDisposable
{
	private readonly ParallelLoopManager manager;

	internal bool disposed;

	internal object disposedLocker = new object();

	internal int finalIndex;

	internal AutoResetEvent getToWork;

	private object initializationInformation;

	internal int iterationsPerSteal;

	private Thread thread;

	private Action<object> threadStart;

	internal ParallelLoopWorker(ParallelLoopManager manager, Action<object> threadStart, object initializationInformation)
	{
		this.manager = manager;
		this.threadStart = threadStart;
		this.initializationInformation = initializationInformation;
		getToWork = new AutoResetEvent(initialState: false);
		thread = new Thread(Work)
		{
			IsBackground = true
		};
		thread.Start();
	}

	/// <summary>
	/// Releases resources used by the object.
	/// </summary>
	~ParallelLoopWorker()
	{
		Dispose();
	}

	/// <summary>
	/// Disposes the worker.
	/// </summary>
	public void Dispose()
	{
		lock (disposedLocker)
		{
			if (!disposed)
			{
				disposed = true;
				getToWork.Close();
				getToWork = null;
				thread = null;
				GC.SuppressFinalize(this);
			}
		}
	}

	internal void Work()
	{
		if (threadStart != null)
		{
			threadStart(initializationInformation);
		}
		threadStart = null;
		initializationInformation = null;
		while (true)
		{
			getToWork.WaitOne();
			if (manager.currentLoopBody == null)
			{
				break;
			}
			while (manager.jobIndex <= manager.maxJobIndex)
			{
				int num = Interlocked.Increment(ref manager.jobIndex);
				int num2 = num * iterationsPerSteal;
				int num3 = num2 - iterationsPerSteal;
				for (int i = num3; i < num2 && i < finalIndex; i++)
				{
					manager.currentLoopBody(i);
				}
			}
			manager.OnWorkerFinish();
		}
		manager.OnWorkerFinish();
	}
}
