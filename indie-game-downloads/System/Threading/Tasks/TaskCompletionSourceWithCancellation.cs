namespace System.Threading.Tasks;

internal class TaskCompletionSourceWithCancellation<T> : TaskCompletionSource<T>
{
	public TaskCompletionSourceWithCancellation()
		: base(TaskCreationOptions.RunContinuationsAsynchronously)
	{
	}

	public async ValueTask<T> WaitWithCancellationAsync(CancellationToken cancellationToken)
	{
		using (cancellationToken.UnsafeRegister(delegate(object s, CancellationToken cancellationToken2)
		{
			((TaskCompletionSourceWithCancellation<T>)s).TrySetCanceled(cancellationToken2);
		}, this))
		{
			return await base.Task.ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public T WaitWithCancellation(CancellationToken cancellationToken)
	{
		using (cancellationToken.UnsafeRegister(delegate(object s, CancellationToken cancellationToken2)
		{
			((TaskCompletionSourceWithCancellation<T>)s).TrySetCanceled(cancellationToken2);
		}, this))
		{
			return base.Task.GetAwaiter().GetResult();
		}
	}

	public ValueTask<T> WaitWithCancellationAsync(bool async, CancellationToken cancellationToken)
	{
		if (!async)
		{
			return new ValueTask<T>(WaitWithCancellation(cancellationToken));
		}
		return WaitWithCancellationAsync(cancellationToken);
	}
}
