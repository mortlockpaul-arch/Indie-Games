using System;
using System.Collections.Generic;
using System.Threading;

namespace BEPUphysics.Threading;

/// <summary>
/// Manages parallel for loops.
/// Cannot handle general task-based parallelism.
/// </summary>
public class ParallelLoopManager : IDisposable
{
	private readonly AutoResetEvent loopFinished;

	private int workerCount;

	internal List<ParallelLoopWorker> workers = new List<ParallelLoopWorker>();

	internal int currentBeginIndex;

	internal int currentEndIndex;

	internal Action<int> currentLoopBody;

	internal int iterationsPerSteal;

	private int minimumTasksPerThread = 3;

	private int maximumIterationsPerTask = 80;

	internal int jobIndex;

	internal int maxJobIndex;

	private bool disposed;

	private readonly object disposedLocker = new object();

	/// <summary>
	/// Gets or sets the minimum number of tasks to be allocated to each thread
	/// per loop.
	/// </summary>
	public int MinimumTasksPerThread
	{
		get
		{
			return minimumTasksPerThread;
		}
		set
		{
			minimumTasksPerThread = value;
		}
	}

	/// <summary>
	/// Gets or sets the maximum number of loop iterations
	/// per individual task.
	/// </summary>
	public int MaximumIterationsPerTask
	{
		get
		{
			return maximumIterationsPerTask;
		}
		set
		{
			maximumIterationsPerTask = value;
		}
	}

	/// <summary>
	/// Constructs a new parallel loop manager.
	/// </summary>
	public ParallelLoopManager()
	{
		loopFinished = new AutoResetEvent(initialState: false);
	}

	internal void AddThread()
	{
		AddThread(null, null);
	}

	internal void AddThread(Action<object> threadStart, object threadStartInformation)
	{
		workers.Add(new ParallelLoopWorker(this, threadStart, threadStartInformation));
	}

	internal void RemoveThread()
	{
		if (workers.Count <= 0)
		{
			return;
		}
		lock (workers[0].disposedLocker)
		{
			if (!workers[0].disposed)
			{
				currentLoopBody = null;
				workerCount = 1;
				workers[0].getToWork.Set();
				loopFinished.WaitOne();
				workers[0].Dispose();
			}
		}
		workers.RemoveAt(0);
	}

	/// <summary>
	/// Iterates over the interval.
	/// </summary>
	/// <param name="beginIndex">Starting index of the iteration.</param>
	/// <param name="endIndex">Ending index of the iteration.</param>
	/// <param name="loopBody">Function to call on each iteration.</param>
	public void ForLoop(int beginIndex, int endIndex, Action<int> loopBody)
	{
		workerCount = workers.Count;
		int num = endIndex - beginIndex;
		int num2 = Math.Max(minimumTasksPerThread, num / maximumIterationsPerTask);
		int num3 = workerCount * num2;
		currentBeginIndex = beginIndex;
		currentEndIndex = endIndex;
		currentLoopBody = loopBody;
		iterationsPerSteal = Math.Max(1, num / num3);
		jobIndex = 0;
		float num4 = (float)num / (float)iterationsPerSteal;
		if (num4 % 1f == 0f)
		{
			maxJobIndex = (int)num4;
		}
		else
		{
			maxJobIndex = 1 + (int)num4;
		}
		for (int i = 0; i < workers.Count; i++)
		{
			workers[i].finalIndex = endIndex;
			workers[i].iterationsPerSteal = iterationsPerSteal;
			workers[i].getToWork.Set();
		}
		loopFinished.WaitOne();
	}

	internal void OnWorkerFinish()
	{
		if (Interlocked.Decrement(ref workerCount) == 0)
		{
			loopFinished.Set();
		}
	}

	/// <summary>
	/// Releases resources used by the object.
	/// </summary>
	public void Dispose()
	{
		lock (disposedLocker)
		{
			if (!disposed)
			{
				disposed = true;
				while (workers.Count > 0)
				{
					RemoveThread();
				}
				loopFinished.Close();
				GC.SuppressFinalize(this);
			}
		}
	}

	/// <summary>
	/// Releases resources used by the object.
	/// </summary>
	~ParallelLoopManager()
	{
		Dispose();
	}
}
