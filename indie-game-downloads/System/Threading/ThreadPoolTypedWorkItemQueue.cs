using System.Collections.Concurrent;

namespace System.Threading;

internal sealed class ThreadPoolTypedWorkItemQueue : IThreadPoolWorkItem
{
	private int _hasOutstandingThreadRequest;

	private readonly ConcurrentQueue<PortableThreadPool.IOCompletionPoller.Event> _workItems = new ConcurrentQueue<PortableThreadPool.IOCompletionPoller.Event>();

	public void BatchEnqueue(PortableThreadPool.IOCompletionPoller.Event workItem)
	{
		_workItems.Enqueue(workItem);
	}

	public void CompleteBatchEnqueue()
	{
		EnsureWorkerScheduled();
	}

	private void EnsureWorkerScheduled()
	{
		if (Interlocked.Exchange(ref _hasOutstandingThreadRequest, 1) == 0)
		{
			ThreadPool.UnsafeQueueHighPriorityWorkItemInternal(this);
		}
	}

	void IThreadPoolWorkItem.Execute()
	{
		_hasOutstandingThreadRequest = 0;
		Interlocked.MemoryBarrier();
		ThreadPoolWorkQueueThreadLocals threadLocals = ThreadPoolWorkQueueThreadLocals.threadLocals;
		if (!_workItems.TryDequeue(out var result))
		{
			ThreadInt64PersistentCounter.Decrement(threadLocals.threadLocalCompletionCountObject);
			return;
		}
		if (!_workItems.IsEmpty)
		{
			EnsureWorkerScheduled();
		}
		Thread currentThread = threadLocals.currentThread;
		uint num = 0u;
		int tickCount = Environment.TickCount;
		while (true)
		{
			result.Invoke();
			if (++num == uint.MaxValue || threadLocals.workStealingQueue.CanSteal || (uint)(Environment.TickCount - tickCount) >= 15u || !_workItems.TryDequeue(out result))
			{
				break;
			}
			ExecutionContext.ResetThreadPoolThread(currentThread);
			currentThread.ResetThreadPoolThread();
		}
		if (num > 1)
		{
			ThreadInt64PersistentCounter.Add(threadLocals.threadLocalCompletionCountObject, num - 1);
		}
	}
}
