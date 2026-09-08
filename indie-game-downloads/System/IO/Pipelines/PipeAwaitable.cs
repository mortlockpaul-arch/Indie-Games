using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks.Sources;

namespace System.IO.Pipelines;

[DebuggerDisplay("CanceledState = {_awaitableState}, IsCompleted = {IsCompleted}")]
internal struct PipeAwaitable
{
	[Flags]
	private enum AwaitableState
	{
		None = 0,
		Completed = 1,
		Running = 2,
		Canceled = 4,
		UseSynchronizationContext = 8
	}

	private sealed class SchedulingContext
	{
		public SynchronizationContext SynchronizationContext { get; set; }

		public ExecutionContext ExecutionContext { get; set; }
	}

	private AwaitableState _awaitableState = (AwaitableState)((completed ? 1 : 0) | (useSynchronizationContext ? 8 : 0));

	private Action<object> _completion = null;

	private object _completionState = null;

	private SchedulingContext _schedulingContext = null;

	private CancellationTokenRegistration _cancellationTokenRegistration = default(CancellationTokenRegistration);

	private CancellationToken CancellationToken => _cancellationTokenRegistration.Token;

	public bool IsCompleted => (_awaitableState & (AwaitableState.Completed | AwaitableState.Canceled)) != 0;

	public bool IsRunning => (_awaitableState & AwaitableState.Running) != 0;

	public PipeAwaitable(bool completed, bool useSynchronizationContext)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void BeginOperation(CancellationToken cancellationToken, Action<object?> callback, object? state)
	{
		if (cancellationToken.CanBeCanceled && !IsCompleted)
		{
			_cancellationTokenRegistration = cancellationToken.UnsafeRegister(callback, state);
			if (_cancellationTokenRegistration == default(CancellationTokenRegistration))
			{
				cancellationToken.ThrowIfCancellationRequested();
			}
		}
		_awaitableState |= AwaitableState.Running;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Complete(out CompletionData completionData)
	{
		ExtractCompletion(out completionData);
		_awaitableState |= AwaitableState.Completed;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ExtractCompletion(out CompletionData completionData)
	{
		Action<object> completion = _completion;
		object completionState = _completionState;
		SchedulingContext schedulingContext = _schedulingContext;
		ExecutionContext executionContext = schedulingContext?.ExecutionContext;
		SynchronizationContext synchronizationContext = schedulingContext?.SynchronizationContext;
		_completion = null;
		_completionState = null;
		_schedulingContext = null;
		completionData = ((completion != null) ? new CompletionData(completion, completionState, executionContext, synchronizationContext) : default(CompletionData));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetUncompleted()
	{
		_awaitableState &= ~AwaitableState.Completed;
	}

	public void OnCompleted(Action<object?> continuation, object? state, ValueTaskSourceOnCompletedFlags flags, out CompletionData completionData, out bool doubleCompletion)
	{
		completionData = default(CompletionData);
		doubleCompletion = _completion != null;
		if (IsCompleted | doubleCompletion)
		{
			completionData = new CompletionData(continuation, state, _schedulingContext?.ExecutionContext, _schedulingContext?.SynchronizationContext);
			return;
		}
		_completion = continuation;
		_completionState = state;
		if ((_awaitableState & AwaitableState.UseSynchronizationContext) != AwaitableState.None && (flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != ValueTaskSourceOnCompletedFlags.None)
		{
			SynchronizationContext current = SynchronizationContext.Current;
			if (current != null && current.GetType() != typeof(SynchronizationContext))
			{
				if (_schedulingContext == null)
				{
					_schedulingContext = new SchedulingContext();
				}
				_schedulingContext.SynchronizationContext = current;
			}
		}
		if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != ValueTaskSourceOnCompletedFlags.None)
		{
			if (_schedulingContext == null)
			{
				_schedulingContext = new SchedulingContext();
			}
			_schedulingContext.ExecutionContext = ExecutionContext.Capture();
		}
	}

	public void Cancel(out CompletionData completionData)
	{
		ExtractCompletion(out completionData);
		_awaitableState |= AwaitableState.Canceled;
	}

	public void CancellationTokenFired(out CompletionData completionData)
	{
		if (CancellationToken.IsCancellationRequested)
		{
			Cancel(out completionData);
		}
		else
		{
			completionData = default(CompletionData);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ObserveCancellation()
	{
		bool result = (_awaitableState & AwaitableState.Canceled) == AwaitableState.Canceled;
		_awaitableState &= ~(AwaitableState.Running | AwaitableState.Canceled);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CancellationTokenRegistration ReleaseCancellationTokenRegistration(out CancellationToken cancellationToken)
	{
		cancellationToken = CancellationToken;
		CancellationTokenRegistration cancellationTokenRegistration = _cancellationTokenRegistration;
		_cancellationTokenRegistration = default(CancellationTokenRegistration);
		return cancellationTokenRegistration;
	}
}
