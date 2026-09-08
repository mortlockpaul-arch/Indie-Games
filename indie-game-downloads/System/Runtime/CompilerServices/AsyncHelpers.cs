using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
[Experimental("SYSLIB5007", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public static class AsyncHelpers
{
	private struct RuntimeAsyncAwaitState
	{
		public Continuation SentinelContinuation;

		public INotifyCompletion Notifier;
	}

	private interface IThunkTaskOps<T>
	{
		static abstract Action GetContinuationAction(T task);

		static abstract Continuation GetContinuationState(T task);

		static abstract void SetContinuationState(T task, Continuation value);

		static abstract bool SetCompleted(T task, Continuation continuation);

		static abstract void PostToSyncContext(T task, SynchronizationContext syncCtx);
	}

	private sealed class ThunkTask<T> : Task<T>
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct Ops : IThunkTaskOps<ThunkTask<T>>
		{
			public static Action GetContinuationAction(ThunkTask<T> task)
			{
				return (Action)task.m_action;
			}

			public static Continuation GetContinuationState(ThunkTask<T> task)
			{
				return (Continuation)task.m_stateObject;
			}

			public static void SetContinuationState(ThunkTask<T> task, Continuation value)
			{
				task.m_stateObject = value;
			}

			public static bool SetCompleted(ThunkTask<T> task, Continuation continuation)
			{
				T result = ((!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) ? Unsafe.As<byte, T>(ref continuation.Data[0]) : ((!typeof(T).IsValueType) ? Unsafe.As<object, T>(ref continuation.GCData[0]) : Unsafe.As<byte, T>(ref continuation.GCData[0].GetRawData())));
				return task.TrySetResult(result);
			}

			public static void PostToSyncContext(ThunkTask<T> task, SynchronizationContext syncContext)
			{
				syncContext.Post(ThunkTask<T>.s_postCallback, task);
			}
		}

		private static readonly SendOrPostCallback s_postCallback = delegate(object state)
		{
			((ThunkTask<T>)state).MoveNext();
		};

		public ThunkTask()
		{
			m_action = new Action(MoveNext);
			m_stateFlags |= 2048;
		}

		internal override void ExecuteFromThreadPool(Thread threadPoolThread)
		{
			MoveNext();
		}

		private void MoveNext()
		{
			ThunkTaskCore.MoveNext<ThunkTask<T>, Ops>(this);
		}

		public void HandleSuspended()
		{
			ThunkTaskCore.HandleSuspended<ThunkTask<T>, Ops>(this);
		}
	}

	private sealed class ThunkTask : Task
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct Ops : IThunkTaskOps<ThunkTask>
		{
			public static Action GetContinuationAction(ThunkTask task)
			{
				return (Action)task.m_action;
			}

			public static Continuation GetContinuationState(ThunkTask task)
			{
				return (Continuation)task.m_stateObject;
			}

			public static void SetContinuationState(ThunkTask task, Continuation value)
			{
				task.m_stateObject = value;
			}

			public static bool SetCompleted(ThunkTask task, Continuation continuation)
			{
				return task.TrySetResult();
			}

			public static void PostToSyncContext(ThunkTask task, SynchronizationContext syncContext)
			{
				syncContext.Post(s_postCallback, task);
			}
		}

		private static readonly SendOrPostCallback s_postCallback = delegate(object state)
		{
			((ThunkTask)state).MoveNext();
		};

		public ThunkTask()
		{
			m_action = new Action(MoveNext);
			m_stateFlags |= 2048;
		}

		internal override void ExecuteFromThreadPool(Thread threadPoolThread)
		{
			MoveNext();
		}

		private void MoveNext()
		{
			ThunkTaskCore.MoveNext<ThunkTask, Ops>(this);
		}

		public void HandleSuspended()
		{
			ThunkTaskCore.HandleSuspended<ThunkTask, Ops>(this);
		}
	}

	private static class ThunkTaskCore
	{
		public unsafe static void MoveNext<T, TOps>(T task) where T : Task where TOps : IThunkTaskOps<T>
		{
			ExecutionAndSyncBlockStore executionAndSyncBlockStore = default(ExecutionAndSyncBlockStore);
			executionAndSyncBlockStore.Push();
			Continuation continuation = TOps.GetContinuationState(task);
			do
			{
				try
				{
					Continuation continuation2 = continuation.Resume(continuation);
					if (continuation2 != null)
					{
						continuation2.Next = continuation.Next;
						HandleSuspended<T, TOps>(task);
						executionAndSyncBlockStore.Pop();
						return;
					}
					continuation = continuation.Next;
				}
				catch (Exception ex)
				{
					Continuation continuation3 = UnwindToPossibleHandler(continuation);
					if (continuation3.Resume == (delegate*<Continuation, Continuation>)null)
					{
						bool num = ((ex is OperationCanceledException ex2) ? task.TrySetCanceled(ex2.CancellationToken, ex2) : task.TrySetException(ex));
						executionAndSyncBlockStore.Pop();
						if (!num)
						{
							ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
						}
						return;
					}
					continuation3.SetException(ex);
					continuation = continuation3;
				}
				if (continuation.Resume == (delegate*<Continuation, Continuation>)null)
				{
					bool num2 = TOps.SetCompleted(task, continuation);
					executionAndSyncBlockStore.Pop();
					if (!num2)
					{
						ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
					}
					return;
				}
			}
			while (!QueueContinuationFollowUpActionIfNecessary<T, TOps>(task, continuation));
			executionAndSyncBlockStore.Pop();
		}

		private static Continuation UnwindToPossibleHandler(Continuation continuation)
		{
			do
			{
				continuation = continuation.Next;
			}
			while ((continuation.Flags & CorInfoContinuationFlags.CORINFO_CONTINUATION_NEEDS_EXCEPTION) == 0);
			return continuation;
		}

		public static void HandleSuspended<T, TOps>(T task) where T : Task where TOps : IThunkTaskOps<T>
		{
			Continuation value = UnlinkHeadContinuation(out var notifier);
			TOps.SetContinuationState(task, value);
			try
			{
				if (notifier is ICriticalNotifyCompletion criticalNotifyCompletion)
				{
					criticalNotifyCompletion.UnsafeOnCompleted(TOps.GetContinuationAction(task));
				}
				else
				{
					notifier.OnCompleted(TOps.GetContinuationAction(task));
				}
			}
			catch (Exception exception)
			{
				Task.ThrowAsync(exception, null);
			}
		}

		private static Continuation UnlinkHeadContinuation(out INotifyCompletion notifier)
		{
			notifier = t_runtimeAsyncAwaitState.Notifier;
			t_runtimeAsyncAwaitState.Notifier = null;
			Continuation sentinelContinuation = t_runtimeAsyncAwaitState.SentinelContinuation;
			Continuation next = sentinelContinuation.Next;
			sentinelContinuation.Next = null;
			return next;
		}

		private static bool QueueContinuationFollowUpActionIfNecessary<T, TOps>(T task, Continuation continuation) where T : Task where TOps : IThunkTaskOps<T>
		{
			if ((continuation.Flags & CorInfoContinuationFlags.CORINFO_CONTINUATION_CONTINUE_ON_THREAD_POOL) != 0)
			{
				SynchronizationContext synchronizationContext = Thread.CurrentThreadAssumedInitialized._synchronizationContext;
				if (synchronizationContext == null || synchronizationContext.GetType() == typeof(SynchronizationContext))
				{
					TaskScheduler internalCurrent = TaskScheduler.InternalCurrent;
					if (internalCurrent == null || internalCurrent == TaskScheduler.Default)
					{
						return false;
					}
				}
				TOps.SetContinuationState(task, continuation);
				ThreadPool.UnsafeQueueUserWorkItemInternal(task, preferLocal: true);
				return true;
			}
			if ((continuation.Flags & CorInfoContinuationFlags.CORINFO_CONTINUATION_CONTINUE_ON_CAPTURED_SYNCHRONIZATION_CONTEXT) != 0)
			{
				SynchronizationContext synchronizationContext2 = (SynchronizationContext)continuation.GetContinuationContext();
				if (synchronizationContext2 == Thread.CurrentThreadAssumedInitialized._synchronizationContext)
				{
					return false;
				}
				TOps.SetContinuationState(task, continuation);
				try
				{
					TOps.PostToSyncContext(task, synchronizationContext2);
				}
				catch (Exception exception)
				{
					Task.ThrowAsync(exception, null);
				}
				return true;
			}
			if ((continuation.Flags & CorInfoContinuationFlags.CORINFO_CONTINUATION_CONTINUE_ON_CAPTURED_TASK_SCHEDULER) != 0)
			{
				TaskScheduler scheduler = (TaskScheduler)continuation.GetContinuationContext();
				TOps.SetContinuationState(task, continuation);
				new TaskSchedulerAwaitTaskContinuation(scheduler, TOps.GetContinuationAction(task), flowExecutionContext: false).Run(Task.CompletedTask, canInlineContinuationTask: true);
				return true;
			}
			return false;
		}
	}

	[ThreadStatic]
	private static RuntimeAsyncAwaitState t_runtimeAsyncAwaitState;

	[Intrinsic]
	private static void AsyncSuspend(Continuation continuation)
	{
		throw new UnreachableException();
	}

	private static Continuation AllocContinuation(Continuation prevContinuation, nuint numGCRefs, nuint dataSize)
	{
		return prevContinuation.Next = new Continuation
		{
			Data = new byte[dataSize],
			GCData = new object[numGCRefs]
		};
	}

	private unsafe static Continuation AllocContinuationMethod(Continuation prevContinuation, nuint numGCRefs, nuint dataSize, MethodDesc* method)
	{
		LoaderAllocator loaderAllocator = RuntimeMethodHandle.GetLoaderAllocator(new RuntimeMethodHandleInternal((nint)method));
		object[] array;
		if (loaderAllocator != null)
		{
			array = new object[numGCRefs + 1];
			array[numGCRefs] = loaderAllocator;
		}
		else
		{
			array = new object[numGCRefs];
		}
		return prevContinuation.Next = new Continuation
		{
			Data = new byte[dataSize],
			GCData = array
		};
	}

	private unsafe static Continuation AllocContinuationClass(Continuation prevContinuation, nuint numGCRefs, nuint dataSize, MethodTable* methodTable)
	{
		nint loaderAllocatorHandle = methodTable->GetLoaderAllocatorHandle();
		object[] array;
		if (loaderAllocatorHandle != IntPtr.Zero)
		{
			array = new object[numGCRefs + 1];
			array[numGCRefs] = GCHandle.FromIntPtr(loaderAllocatorHandle).Target;
		}
		else
		{
			array = new object[numGCRefs];
		}
		return prevContinuation.Next = new Continuation
		{
			Data = new byte[dataSize],
			GCData = array
		};
	}

	private unsafe static object AllocContinuationResultBox(void* ptr)
	{
		return RuntimeTypeHandle.InternalAllocNoChecks((MethodTable*)ptr);
	}

	private static Task<T> FinalizeTaskReturningThunk<T>(Continuation continuation)
	{
		Continuation continuation2 = new Continuation();
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			continuation2.Flags = CorInfoContinuationFlags.CORINFO_CONTINUATION_RESULT_IN_GCDATA | CorInfoContinuationFlags.CORINFO_CONTINUATION_NEEDS_EXCEPTION;
			continuation2.GCData = new object[1];
		}
		else
		{
			continuation2.Flags = CorInfoContinuationFlags.CORINFO_CONTINUATION_NEEDS_EXCEPTION;
			continuation2.Data = new byte[Unsafe.SizeOf<T>()];
		}
		continuation.Next = continuation2;
		ThunkTask<T> thunkTask = new ThunkTask<T>();
		thunkTask.HandleSuspended();
		return thunkTask;
	}

	private static Task FinalizeTaskReturningThunk(Continuation continuation)
	{
		Continuation next = new Continuation
		{
			Flags = CorInfoContinuationFlags.CORINFO_CONTINUATION_NEEDS_EXCEPTION
		};
		continuation.Next = next;
		ThunkTask thunkTask = new ThunkTask();
		thunkTask.HandleSuspended();
		return thunkTask;
	}

	private static ValueTask<T> FinalizeValueTaskReturningThunk<T>(Continuation continuation)
	{
		return new ValueTask<T>(FinalizeTaskReturningThunk<T>(continuation));
	}

	private static ValueTask FinalizeValueTaskReturningThunk(Continuation continuation)
	{
		return new ValueTask(FinalizeTaskReturningThunk(continuation));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ExecutionContext CaptureExecutionContext()
	{
		return Thread.CurrentThreadAssumedInitialized._executionContext;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void RestoreExecutionContext(ExecutionContext previousExecCtx)
	{
		Thread currentThreadAssumedInitialized = Thread.CurrentThreadAssumedInitialized;
		ExecutionContext executionContext = currentThreadAssumedInitialized._executionContext;
		if (previousExecCtx != executionContext)
		{
			ExecutionContext.RestoreChangedContextToThread(currentThreadAssumedInitialized, previousExecCtx, executionContext);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void CaptureContexts(out ExecutionContext execCtx, out SynchronizationContext syncCtx)
	{
		Thread currentThreadAssumedInitialized = Thread.CurrentThreadAssumedInitialized;
		execCtx = currentThreadAssumedInitialized._executionContext;
		syncCtx = currentThreadAssumedInitialized._synchronizationContext;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void RestoreContexts(bool suspended, ExecutionContext previousExecCtx, SynchronizationContext previousSyncCtx)
	{
		Thread currentThreadAssumedInitialized = Thread.CurrentThreadAssumedInitialized;
		if (!suspended && previousSyncCtx != currentThreadAssumedInitialized._synchronizationContext)
		{
			currentThreadAssumedInitialized._synchronizationContext = previousSyncCtx;
		}
		ExecutionContext executionContext = currentThreadAssumedInitialized._executionContext;
		if (previousExecCtx != executionContext)
		{
			ExecutionContext.RestoreChangedContextToThread(currentThreadAssumedInitialized, previousExecCtx, executionContext);
		}
	}

	private static void CaptureContinuationContext(SynchronizationContext syncCtx, ref object context, ref CorInfoContinuationFlags flags)
	{
		if (syncCtx != null && syncCtx.GetType() != typeof(SynchronizationContext))
		{
			flags |= CorInfoContinuationFlags.CORINFO_CONTINUATION_CONTINUE_ON_CAPTURED_SYNCHRONIZATION_CONTEXT;
			context = syncCtx;
			return;
		}
		TaskScheduler internalCurrent = TaskScheduler.InternalCurrent;
		if (internalCurrent != null && internalCurrent != TaskScheduler.Default)
		{
			flags |= CorInfoContinuationFlags.CORINFO_CONTINUATION_CONTINUE_ON_CAPTURED_TASK_SCHEDULER;
			context = internalCurrent;
		}
		else
		{
			flags |= CorInfoContinuationFlags.CORINFO_CONTINUATION_CONTINUE_ON_THREAD_POOL;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async void AwaitAwaiter<TAwaiter>(TAwaiter awaiter) where TAwaiter : INotifyCompletion
	{
		Continuation continuation = t_runtimeAsyncAwaitState.SentinelContinuation;
		if (continuation == null)
		{
			continuation = (t_runtimeAsyncAwaitState.SentinelContinuation = new Continuation());
		}
		t_runtimeAsyncAwaitState.Notifier = awaiter;
		AsyncSuspend(continuation);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async void UnsafeAwaitAwaiter<TAwaiter>(TAwaiter awaiter) where TAwaiter : ICriticalNotifyCompletion
	{
		Continuation continuation = t_runtimeAsyncAwaitState.SentinelContinuation;
		if (continuation == null)
		{
			continuation = (t_runtimeAsyncAwaitState.SentinelContinuation = new Continuation());
		}
		t_runtimeAsyncAwaitState.Notifier = awaiter;
		AsyncSuspend(continuation);
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async T Await<T>(Task<T> task)
	{
		return await task;
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async void Await(Task task)
	{
		await task;
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async T Await<T>(ValueTask<T> task)
	{
		return await task;
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async void Await(ValueTask task)
	{
		await task;
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async void Await(ConfiguredTaskAwaitable configuredAwaitable)
	{
		await configuredAwaitable;
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async void Await(ConfiguredValueTaskAwaitable configuredAwaitable)
	{
		await configuredAwaitable;
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async T Await<T>(ConfiguredTaskAwaitable<T> configuredAwaitable)
	{
		return await configuredAwaitable;
	}

	[Intrinsic]
	[BypassReadyToRun]
	[RequiresPreviewFeatures]
	public static async T Await<T>(ConfiguredValueTaskAwaitable<T> configuredAwaitable)
	{
		return await configuredAwaitable;
	}
}
