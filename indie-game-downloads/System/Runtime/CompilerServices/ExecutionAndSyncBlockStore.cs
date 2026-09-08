using System.Threading;

namespace System.Runtime.CompilerServices;

internal struct ExecutionAndSyncBlockStore
{
	public ExecutionContext _previousExecutionCtx;

	public SynchronizationContext _previousSyncCtx;

	public Thread _thread;

	public void Push()
	{
		_thread = Thread.CurrentThread;
		_previousExecutionCtx = _thread._executionContext;
		_previousSyncCtx = _thread._synchronizationContext;
	}

	public void Pop()
	{
		if (_previousSyncCtx != _thread._synchronizationContext)
		{
			_thread._synchronizationContext = _previousSyncCtx;
		}
		ExecutionContext executionContext = _thread._executionContext;
		if (_previousExecutionCtx != executionContext)
		{
			ExecutionContext.RestoreChangedContextToThread(_thread, _previousExecutionCtx, executionContext);
		}
	}
}
