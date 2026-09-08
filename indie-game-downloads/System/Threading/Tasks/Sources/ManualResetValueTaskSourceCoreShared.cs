using System.Runtime.ExceptionServices;

namespace System.Threading.Tasks.Sources;

internal static class ManualResetValueTaskSourceCoreShared
{
	internal static readonly Action<object> s_sentinel = CompletionSentinel;

	private static void CompletionSentinel(object _)
	{
		ThrowHelper.ThrowInvalidOperationException();
	}

	internal static void ScheduleCapturedContext(object context, Action<object> continuation, object state)
	{
		if (!(context is SynchronizationContext sc))
		{
			if (context is TaskScheduler scheduler)
			{
				ScheduleTaskScheduler(scheduler, continuation, state);
				return;
			}
			CapturedSchedulerAndExecutionContext capturedSchedulerAndExecutionContext = (CapturedSchedulerAndExecutionContext)context;
			if (capturedSchedulerAndExecutionContext._scheduler is SynchronizationContext sc2)
			{
				ScheduleSynchronizationContext(sc2, continuation, state);
			}
			else
			{
				ScheduleTaskScheduler((TaskScheduler)capturedSchedulerAndExecutionContext._scheduler, continuation, state);
			}
		}
		else
		{
			ScheduleSynchronizationContext(sc, continuation, state);
		}
		static void ScheduleSynchronizationContext(SynchronizationContext synchronizationContext, Action<object> action, object state2)
		{
			synchronizationContext.Post(action.Invoke, state2);
		}
		static void ScheduleTaskScheduler(TaskScheduler scheduler2, Action<object> action, object state2)
		{
			Task.Factory.StartNew(action, state2, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler2);
		}
	}

	internal static void InvokeContinuationWithContext(object capturedContext, Action<object> continuation, object continuationState, bool runContinuationsAsynchronously)
	{
		ExecutionContext executionContext = ExecutionContext.CaptureForRestore();
		if (capturedContext is ExecutionContext executionContext2)
		{
			ExecutionContext.RestoreInternal(executionContext2);
			if (runContinuationsAsynchronously)
			{
				try
				{
					ThreadPool.QueueUserWorkItem(continuation, continuationState, preferLocal: true);
					return;
				}
				finally
				{
					ExecutionContext.RestoreInternal(executionContext);
				}
			}
			ExceptionDispatchInfo exceptionDispatchInfo = null;
			SynchronizationContext current = SynchronizationContext.Current;
			try
			{
				continuation(continuationState);
			}
			catch (Exception source)
			{
				exceptionDispatchInfo = ExceptionDispatchInfo.Capture(source);
			}
			finally
			{
				SynchronizationContext.SetSynchronizationContext(current);
				ExecutionContext.RestoreInternal(executionContext);
			}
			exceptionDispatchInfo?.Throw();
			return;
		}
		ExecutionContext.Restore(((CapturedSchedulerAndExecutionContext)capturedContext)._executionContext);
		try
		{
			ScheduleCapturedContext(capturedContext, continuation, continuationState);
		}
		finally
		{
			ExecutionContext.RestoreInternal(executionContext);
		}
	}
}
