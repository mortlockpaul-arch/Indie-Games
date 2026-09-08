using System.Collections.Generic;

namespace System.Threading.Tasks;

public class TaskCompletionSource
{
	private readonly Task _task;

	public Task Task => _task;

	public TaskCompletionSource()
	{
		_task = new Task();
	}

	public TaskCompletionSource(TaskCreationOptions creationOptions)
		: this(null, creationOptions)
	{
	}

	public TaskCompletionSource(object? state)
		: this(state, TaskCreationOptions.None)
	{
	}

	public TaskCompletionSource(object? state, TaskCreationOptions creationOptions)
	{
		_task = new Task(state, creationOptions, promiseStyle: true);
	}

	public void SetException(Exception exception)
	{
		if (!TrySetException(exception))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public void SetException(IEnumerable<Exception> exceptions)
	{
		if (!TrySetException(exceptions))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetException(Exception exception)
	{
		if (exception == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.exception);
		}
		bool num = _task.TrySetException(exception);
		if (!num && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public bool TrySetException(IEnumerable<Exception> exceptions)
	{
		if (exceptions == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.exceptions);
		}
		List<Exception> list = new List<Exception>();
		foreach (Exception exception in exceptions)
		{
			if (exception == null)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.TaskCompletionSourceT_TrySetException_NullException, ExceptionArgument.exceptions);
			}
			list.Add(exception);
		}
		if (list.Count == 0)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.TaskCompletionSourceT_TrySetException_NoExceptions, ExceptionArgument.exceptions);
		}
		bool num = _task.TrySetException(list);
		if (!num && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public void SetResult()
	{
		if (!TrySetResult())
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetResult()
	{
		bool num = _task.TrySetResult();
		if (!num)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public void SetCanceled()
	{
		SetCanceled(default(CancellationToken));
	}

	public void SetCanceled(CancellationToken cancellationToken)
	{
		if (!TrySetCanceled(cancellationToken))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetCanceled()
	{
		return TrySetCanceled(default(CancellationToken));
	}

	public bool TrySetCanceled(CancellationToken cancellationToken)
	{
		bool num = _task.TrySetCanceled(cancellationToken);
		if (!num && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public void SetFromTask(Task completedTask)
	{
		if (!TrySetFromTask(completedTask))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetFromTask(Task completedTask)
	{
		ArgumentNullException.ThrowIfNull(completedTask, "completedTask");
		if (!completedTask.IsCompleted)
		{
			throw new ArgumentException(SR.Task_MustBeCompleted, "completedTask");
		}
		bool flag = false;
		switch (completedTask.Status)
		{
		case TaskStatus.RanToCompletion:
			flag = _task.TrySetResult();
			break;
		case TaskStatus.Canceled:
			flag = _task.TrySetCanceled(completedTask.CancellationToken, completedTask.GetCancellationExceptionDispatchInfo());
			break;
		case TaskStatus.Faulted:
			flag = _task.TrySetException(completedTask.GetExceptionDispatchInfos());
			break;
		}
		if (!flag && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return flag;
	}
}
public class TaskCompletionSource<TResult>
{
	private readonly Task<TResult> _task;

	public Task<TResult> Task => _task;

	public TaskCompletionSource()
	{
		_task = new Task<TResult>();
	}

	public TaskCompletionSource(TaskCreationOptions creationOptions)
		: this((object?)null, creationOptions)
	{
	}

	public TaskCompletionSource(object? state)
		: this(state, TaskCreationOptions.None)
	{
	}

	public TaskCompletionSource(object? state, TaskCreationOptions creationOptions)
	{
		_task = new Task<TResult>(state, creationOptions);
	}

	public void SetException(Exception exception)
	{
		if (!TrySetException(exception))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public void SetException(IEnumerable<Exception> exceptions)
	{
		if (!TrySetException(exceptions))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetException(Exception exception)
	{
		if (exception == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.exception);
		}
		bool num = _task.TrySetException(exception);
		if (!num && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public bool TrySetException(IEnumerable<Exception> exceptions)
	{
		if (exceptions == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.exceptions);
		}
		List<Exception> list = new List<Exception>();
		foreach (Exception exception in exceptions)
		{
			if (exception == null)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.TaskCompletionSourceT_TrySetException_NullException, ExceptionArgument.exceptions);
			}
			list.Add(exception);
		}
		if (list.Count == 0)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.TaskCompletionSourceT_TrySetException_NoExceptions, ExceptionArgument.exceptions);
		}
		bool num = _task.TrySetException(list);
		if (!num && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public void SetResult(TResult result)
	{
		if (!TrySetResult(result))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetResult(TResult result)
	{
		bool num = _task.TrySetResult(result);
		if (!num)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public void SetCanceled()
	{
		SetCanceled(default(CancellationToken));
	}

	public void SetCanceled(CancellationToken cancellationToken)
	{
		if (!TrySetCanceled(cancellationToken))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetCanceled()
	{
		return TrySetCanceled(default(CancellationToken));
	}

	public bool TrySetCanceled(CancellationToken cancellationToken)
	{
		bool num = _task.TrySetCanceled(cancellationToken);
		if (!num && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return num;
	}

	public void SetFromTask(Task<TResult> completedTask)
	{
		if (!TrySetFromTask(completedTask))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
		}
	}

	public bool TrySetFromTask(Task<TResult> completedTask)
	{
		ArgumentNullException.ThrowIfNull(completedTask, "completedTask");
		if (!completedTask.IsCompleted)
		{
			throw new ArgumentException(SR.Task_MustBeCompleted, "completedTask");
		}
		bool flag = false;
		switch (completedTask.Status)
		{
		case TaskStatus.RanToCompletion:
			flag = _task.TrySetResult(completedTask.Result);
			break;
		case TaskStatus.Canceled:
			flag = _task.TrySetCanceled(completedTask.CancellationToken, completedTask.GetCancellationExceptionDispatchInfo());
			break;
		case TaskStatus.Faulted:
			flag = _task.TrySetException(completedTask.GetExceptionDispatchInfos());
			break;
		}
		if (!flag && !_task.IsCompleted)
		{
			_task.SpinUntilCompleted();
		}
		return flag;
	}
}
