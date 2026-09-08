using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace System.IO.Pipelines;

internal sealed class PipeCompletionCallbacks
{
	private readonly List<PipeCompletionCallback> _callbacks;

	private readonly Exception _exception;

	public PipeCompletionCallbacks(List<PipeCompletionCallback> callbacks, ExceptionDispatchInfo? edi)
	{
		_callbacks = callbacks;
		_exception = edi?.SourceException;
	}

	public void Execute()
	{
		int count = _callbacks.Count;
		if (count != 0)
		{
			List<Exception> exceptions = null;
			for (int i = 0; i < count; i++)
			{
				PipeCompletionCallback callback = _callbacks[i];
				Execute(callback, ref exceptions);
			}
			if (exceptions != null)
			{
				throw new AggregateException(exceptions);
			}
		}
	}

	private void Execute(PipeCompletionCallback callback, ref List<Exception> exceptions)
	{
		try
		{
			callback.Callback(_exception, callback.State);
		}
		catch (Exception item)
		{
			if (exceptions == null)
			{
				exceptions = new List<Exception>();
			}
			exceptions.Add(item);
		}
	}
}
