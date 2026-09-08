using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace System.IO.Pipelines;

[DebuggerDisplay("IsCompleted = {IsCompleted}")]
internal struct PipeCompletion
{
	private static readonly object s_completedSuccessfully = new object();

	private object _state;

	private List<PipeCompletionCallback> _callbacks;

	public bool IsCompleted => _state != null;

	public bool IsFaulted => _state is ExceptionDispatchInfo;

	public PipeCompletionCallbacks? TryComplete(Exception? exception = null)
	{
		if (_state == null)
		{
			if (exception != null)
			{
				_state = ExceptionDispatchInfo.Capture(exception);
			}
			else
			{
				_state = s_completedSuccessfully;
			}
		}
		return GetCallbacks();
	}

	public PipeCompletionCallbacks? AddCallback(Action<Exception?, object?> callback, object? state)
	{
		if (_callbacks == null)
		{
			_callbacks = new List<PipeCompletionCallback>();
		}
		_callbacks.Add(new PipeCompletionCallback(callback, state));
		if (IsCompleted)
		{
			return GetCallbacks();
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsCompletedOrThrow()
	{
		if (!IsCompleted)
		{
			return false;
		}
		if (_state is ExceptionDispatchInfo exceptionDispatchInfo)
		{
			exceptionDispatchInfo.Throw();
		}
		return true;
	}

	private PipeCompletionCallbacks GetCallbacks()
	{
		List<PipeCompletionCallback> callbacks = _callbacks;
		if (callbacks == null)
		{
			return null;
		}
		_callbacks = null;
		return new PipeCompletionCallbacks(callbacks, _state as ExceptionDispatchInfo);
	}

	public void Reset()
	{
		_state = null;
	}

	public override string ToString()
	{
		return $"{"IsCompleted"}: {IsCompleted}";
	}
}
