using System.Threading;

namespace System.IO.Pipelines;

internal sealed class ThreadPoolScheduler : PipeScheduler
{
	public override void Schedule(Action<object?> action, object? state)
	{
		System.Threading.ThreadPool.QueueUserWorkItem<object>(action, state, preferLocal: false);
	}

	internal override void UnsafeSchedule(Action<object?> action, object? state)
	{
		System.Threading.ThreadPool.UnsafeQueueUserWorkItem<object>(action, state, preferLocal: false);
	}
}
