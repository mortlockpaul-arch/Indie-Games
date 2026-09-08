using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.Threading.Channels;

internal abstract class AsyncOperation : IThreadPoolWorkItem
{
	private protected sealed class CapturedSchedulerAndExecutionContext
	{
		internal readonly object _scheduler;

		internal readonly ExecutionContext _executionContext;

		public CapturedSchedulerAndExecutionContext(object scheduler, ExecutionContext executionContext)
		{
			_scheduler = scheduler;
			_executionContext = executionContext;
		}
	}

	protected static readonly Action<object> s_availableSentinel = AvailableSentinel;

	protected static readonly Action<object> s_completedSentinel = CompletedSentinel;

	private readonly CancellationTokenRegistration _cancellationRegistration;

	private protected readonly bool _pooled;

	private volatile bool _completionReserved;

	private protected ExceptionDispatchInfo _error;

	private protected Action<object> _continuation;

	private protected object _continuationState;

	private protected object _capturedContext;

	private protected short _currentId;

	public bool RunContinuationsAsynchronously { get; }

	private CancellationToken CancellationToken => _cancellationRegistration.Token;

	internal bool IsCompleted => (object)_continuation == s_completedSentinel;

	private static void AvailableSentinel(object s)
	{
	}

	private static void CompletedSentinel(object s)
	{
	}

	protected static void ThrowIncompleteOperationException()
	{
		throw new InvalidOperationException(System.SR.InvalidOperation_IncompleteAsyncOperation);
	}

	protected static void ThrowMultipleContinuations()
	{
		throw new InvalidOperationException(System.SR.InvalidOperation_MultipleContinuations);
	}

	protected static void ThrowIncorrectCurrentIdException()
	{
		throw new InvalidOperationException(System.SR.InvalidOperation_IncorrectToken);
	}

	protected AsyncOperation(bool runContinuationsAsynchronously, CancellationToken cancellationToken, bool pooled, Action<object, CancellationToken> cancellationCallback)
	{
		_continuation = (pooled ? s_availableSentinel : null);
		_pooled = pooled;
		RunContinuationsAsynchronously = runContinuationsAsynchronously;
		if (cancellationToken.CanBeCanceled)
		{
			_cancellationRegistration = cancellationToken.UnsafeRegister(cancellationCallback, this);
		}
	}

	public bool TrySetException(Exception exception)
	{
		if (TryReserveCompletionIfCancelable())
		{
			_error = ExceptionDispatchInfo.Capture(exception);
			SignalCompletion();
			return true;
		}
		return false;
	}

	public bool TrySetCanceled(CancellationToken cancellationToken = default(CancellationToken))
	{
		if (TryReserveCompletionIfCancelable())
		{
			_error = ExceptionDispatchInfo.Capture(new OperationCanceledException(cancellationToken));
			SignalCompletion();
			return true;
		}
		return false;
	}

	public bool TryReserveCompletionIfCancelable()
	{
		if (CancellationToken.CanBeCanceled)
		{
			return !Interlocked.Exchange(ref _completionReserved, value: true);
		}
		return true;
	}

	private protected void SignalCompletion()
	{
		Unregister(_cancellationRegistration);
		if (_continuation == null && Interlocked.CompareExchange(ref _continuation, s_completedSentinel, null) == null)
		{
			return;
		}
		object capturedContext = _capturedContext;
		if ((capturedContext == null || capturedContext is ExecutionContext) ? true : false)
		{
			if (RunContinuationsAsynchronously)
			{
				UnsafeQueueSetCompletionAndInvokeContinuation();
				return;
			}
		}
		else
		{
			SynchronizationContext synchronizationContext = (capturedContext as SynchronizationContext) ?? ((capturedContext as CapturedSchedulerAndExecutionContext)?._scheduler as SynchronizationContext);
			if (synchronizationContext != null)
			{
				if (RunContinuationsAsynchronously || synchronizationContext != SynchronizationContext.Current)
				{
					synchronizationContext.Post(delegate(object s)
					{
						((AsyncOperation)s).SetCompletionAndInvokeContinuation();
					}, this);
					return;
				}
			}
			else
			{
				TaskScheduler taskScheduler = (capturedContext as TaskScheduler) ?? ((capturedContext as CapturedSchedulerAndExecutionContext)?._scheduler as TaskScheduler);
				if (RunContinuationsAsynchronously || taskScheduler != TaskScheduler.Current)
				{
					Task.Factory.StartNew(delegate(object s)
					{
						((AsyncOperation)s).SetCompletionAndInvokeContinuation();
					}, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, taskScheduler);
					return;
				}
			}
		}
		SetCompletionAndInvokeContinuation();
	}

	private void SetCompletionAndInvokeContinuation()
	{
		object capturedContext = _capturedContext;
		ExecutionContext executionContext = ((capturedContext == null) ? null : ((capturedContext as ExecutionContext) ?? (capturedContext as CapturedSchedulerAndExecutionContext)?._executionContext));
		if (executionContext == null)
		{
			Action<object> continuation = _continuation;
			_continuation = s_completedSentinel;
			continuation(_continuationState);
			return;
		}
		ExecutionContext.Run(executionContext, delegate(object s)
		{
			AsyncOperation asyncOperation = (AsyncOperation)s;
			Action<object> continuation2 = asyncOperation._continuation;
			asyncOperation._continuation = s_completedSentinel;
			continuation2(asyncOperation._continuationState);
		}, this);
	}

	public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
	{
		if (_currentId != token)
		{
			ThrowIncorrectCurrentIdException();
		}
		if (_continuationState != null)
		{
			ThrowMultipleContinuations();
		}
		_continuationState = state;
		if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != ValueTaskSourceOnCompletedFlags.None)
		{
			_capturedContext = ExecutionContext.Capture();
		}
		SynchronizationContext synchronizationContext = null;
		TaskScheduler taskScheduler = null;
		if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != ValueTaskSourceOnCompletedFlags.None)
		{
			synchronizationContext = SynchronizationContext.Current;
			if (synchronizationContext != null && synchronizationContext.GetType() != typeof(SynchronizationContext))
			{
				_capturedContext = ((_capturedContext == null) ? ((object)synchronizationContext) : ((object)new CapturedSchedulerAndExecutionContext(synchronizationContext, (ExecutionContext)_capturedContext)));
			}
			else
			{
				synchronizationContext = null;
				taskScheduler = TaskScheduler.Current;
				if (taskScheduler != TaskScheduler.Default)
				{
					_capturedContext = ((_capturedContext == null) ? ((object)taskScheduler) : ((object)new CapturedSchedulerAndExecutionContext(taskScheduler, (ExecutionContext)_capturedContext)));
				}
				else
				{
					taskScheduler = null;
				}
			}
		}
		Action<object> action = Interlocked.CompareExchange(ref _continuation, continuation, null);
		if (action == null)
		{
			return;
		}
		if ((object)action != s_completedSentinel)
		{
			ThrowMultipleContinuations();
		}
		if (_capturedContext == null)
		{
			ChannelUtilities.UnsafeQueueUserWorkItem(continuation, state);
		}
		else if (synchronizationContext != null)
		{
			synchronizationContext.Post(delegate(object s)
			{
				KeyValuePair<Action<object>, object> keyValuePair = (KeyValuePair<Action<object>, object>)s;
				keyValuePair.Key(keyValuePair.Value);
			}, new KeyValuePair<Action<object>, object>(continuation, state));
		}
		else if (taskScheduler != null)
		{
			Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, taskScheduler);
		}
		else
		{
			ChannelUtilities.QueueUserWorkItem(continuation, state);
		}
	}

	void IThreadPoolWorkItem.Execute()
	{
		SetCompletionAndInvokeContinuation();
	}

	private void UnsafeQueueSetCompletionAndInvokeContinuation()
	{
		ThreadPool.UnsafeQueueUserWorkItem(this, preferLocal: false);
	}

	private static void Unregister(CancellationTokenRegistration registration)
	{
		registration.Unregister();
	}
}
internal abstract class AsyncOperation<TSelf> : AsyncOperation, IValueTaskSource
{
	public TSelf Next { get; set; }

	public TSelf Previous { get; set; }

	public ValueTask ValueTask => new ValueTask(this, _currentId);

	protected AsyncOperation(bool runContinuationsAsynchronously, CancellationToken cancellationToken = default(CancellationToken), bool pooled = false, Action<object, CancellationToken> cancellationCallback = null)
		: base(runContinuationsAsynchronously, cancellationToken, pooled, cancellationCallback)
	{
	}

	public ValueTaskSourceStatus GetStatus(short token)
	{
		if (_currentId != token)
		{
			AsyncOperation.ThrowIncorrectCurrentIdException();
		}
		if (base.IsCompleted)
		{
			if (_error != null)
			{
				if (!(_error.SourceException is OperationCanceledException))
				{
					return ValueTaskSourceStatus.Faulted;
				}
				return ValueTaskSourceStatus.Canceled;
			}
			return ValueTaskSourceStatus.Succeeded;
		}
		return ValueTaskSourceStatus.Pending;
	}

	void IValueTaskSource.GetResult(short token)
	{
		if (_currentId != token)
		{
			AsyncOperation.ThrowIncorrectCurrentIdException();
		}
		if (!base.IsCompleted)
		{
			AsyncOperation.ThrowIncompleteOperationException();
		}
		ExceptionDispatchInfo error = _error;
		_currentId++;
		if (_pooled)
		{
			Volatile.Write(ref _continuation, AsyncOperation.s_availableSentinel);
		}
		error?.Throw();
	}
}
internal abstract class AsyncOperation<TSelf, TResult> : AsyncOperation<TSelf>, IValueTaskSource<TResult> where TSelf : AsyncOperation<TSelf, TResult>
{
	private TResult _result;

	public ValueTask<TResult> ValueTaskOfT => new ValueTask<TResult>(this, _currentId);

	public AsyncOperation(bool runContinuationsAsynchronously, CancellationToken cancellationToken = default(CancellationToken), bool pooled = false, Action<object, CancellationToken> cancellationCallback = null)
		: base(runContinuationsAsynchronously, cancellationToken, pooled, cancellationCallback)
	{
	}

	public TResult GetResult(short token)
	{
		if (_currentId != token)
		{
			AsyncOperation.ThrowIncorrectCurrentIdException();
		}
		if (!base.IsCompleted)
		{
			AsyncOperation.ThrowIncompleteOperationException();
		}
		ExceptionDispatchInfo error = _error;
		TResult result = _result;
		_currentId++;
		if (_pooled)
		{
			Volatile.Write(ref _continuation, AsyncOperation.s_availableSentinel);
		}
		error?.Throw();
		return result;
	}

	public bool TryOwnAndReset()
	{
		if ((object)Interlocked.CompareExchange(ref _continuation, null, AsyncOperation.s_availableSentinel) == AsyncOperation.s_availableSentinel)
		{
			_continuationState = null;
			_result = default(TResult);
			_error = null;
			_capturedContext = null;
			return true;
		}
		return false;
	}

	public bool TrySetResult(TResult result)
	{
		if (TryReserveCompletionIfCancelable())
		{
			DangerousSetResult(result);
			return true;
		}
		return false;
	}

	public void DangerousSetResult(TResult result)
	{
		_result = result;
		SignalCompletion();
	}
}
