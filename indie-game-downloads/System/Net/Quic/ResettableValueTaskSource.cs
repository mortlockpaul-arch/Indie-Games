using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace System.Net.Quic;

internal sealed class ResettableValueTaskSource : IValueTaskSource
{
	private enum State
	{
		None,
		Awaiting,
		Ready,
		Completed
	}

	private struct FinalTaskSource
	{
		private TaskCompletionSource _finalTaskSource = null;

		private bool _isCompleted = false;

		private bool _isSignaled = false;

		private Exception _exception = null;

		private Task _signaledTask = null;

		public FinalTaskSource()
		{
		}

		public Task GetTask(object keepAlive)
		{
			if (_finalTaskSource == null)
			{
				if (_isSignaled)
				{
					if (_signaledTask == null)
					{
						_signaledTask = ((_exception == null) ? Task.CompletedTask : Task.FromException(_exception));
					}
					_ = _signaledTask.Exception;
					return _signaledTask;
				}
				_finalTaskSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
				if (!_isCompleted)
				{
					GCHandle gCHandle = GCHandle.Alloc(keepAlive);
					_finalTaskSource.Task.ContinueWith(delegate(Task _, object state)
					{
						((GCHandle)state).Free();
					}, gCHandle, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
				}
			}
			return _finalTaskSource.Task;
		}

		public bool TryComplete(Exception exception = null)
		{
			if (_isCompleted)
			{
				return false;
			}
			_exception = exception;
			_isCompleted = true;
			return true;
		}

		public bool TrySignal(out Exception exception)
		{
			if (!_isCompleted)
			{
				exception = null;
				return false;
			}
			if (_finalTaskSource != null)
			{
				if (_exception != null)
				{
					_finalTaskSource.SetException(_exception);
					_ = _finalTaskSource.Task.Exception;
				}
				else
				{
					_finalTaskSource.SetResult();
				}
			}
			exception = _exception;
			_isSignaled = true;
			return true;
		}
	}

	private State _state;

	private bool _hasWaiter;

	private ManualResetValueTaskSourceCore<bool> _valueTaskSource;

	private CancellationTokenRegistration _cancellationRegistration;

	private CancellationToken _cancelledToken;

	private Action<object> _cancellationAction;

	private GCHandle _keepAlive;

	private FinalTaskSource _finalTaskSource;

	public Action<object> CancellationAction
	{
		init
		{
			_cancellationAction = value;
		}
	}

	public bool IsCompleted => Volatile.Read(in Unsafe.As<State, byte>(ref _state)) == 3;

	public ResettableValueTaskSource()
	{
		_state = State.None;
		_hasWaiter = false;
		_valueTaskSource = new ManualResetValueTaskSourceCore<bool>
		{
			RunContinuationsAsynchronously = true
		};
		_cancellationRegistration = default(CancellationTokenRegistration);
		_cancelledToken = default(CancellationToken);
		_keepAlive = default(GCHandle);
		_finalTaskSource = new FinalTaskSource();
	}

	public bool TryGetValueTask(out ValueTask valueTask, object keepAlive = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		bool flag;
		lock (this)
		{
			if (_state == State.None && cancellationToken.CanBeCanceled)
			{
				_cancellationRegistration = cancellationToken.UnsafeRegister(delegate(object obj, CancellationToken cancelledToken)
				{
					var (resettableValueTaskSource, obj2) = ((ResettableValueTaskSource, object))obj;
					lock (resettableValueTaskSource)
					{
						resettableValueTaskSource._cancelledToken = cancelledToken;
					}
					resettableValueTaskSource._cancellationAction?.Invoke(obj2);
				}, (this, keepAlive));
			}
			State state = _state;
			if (state == State.None)
			{
				if (keepAlive != null)
				{
					_keepAlive = GCHandle.Alloc(keepAlive);
				}
				_state = State.Awaiting;
			}
			flag = ((state == State.None || (uint)(state - 2) <= 1u) ? true : false);
			if (flag)
			{
				_hasWaiter = true;
				valueTask = new ValueTask(this, _valueTaskSource.Version);
				flag = true;
			}
			else
			{
				valueTask = default(ValueTask);
				flag = false;
			}
		}
		return flag;
	}

	public Task GetFinalTask(object keepAlive)
	{
		lock (this)
		{
			return _finalTaskSource.GetTask(keepAlive);
		}
	}

	private bool TryComplete(Exception exception, bool final)
	{
		CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
		lock (this)
		{
			cancellationTokenRegistration = _cancellationRegistration;
			_cancellationRegistration = default(CancellationTokenRegistration);
		}
		cancellationTokenRegistration.Dispose();
		lock (this)
		{
			try
			{
				State state = _state;
				int num;
				switch (state)
				{
				case State.Completed:
					return false;
				case State.Ready:
					num = ((!_hasWaiter) ? 1 : 0);
					break;
				default:
					num = 0;
					break;
				}
				if (((uint)num & (final ? 1u : 0u)) != 0)
				{
					_valueTaskSource.Reset();
					state = State.None;
				}
				if ((uint)state <= 1u)
				{
					_state = (final ? State.Completed : State.Ready);
				}
				if (exception != null)
				{
					exception = ((exception.StackTrace == null) ? ExceptionDispatchInfo.SetCurrentStackTrace(exception) : exception);
					if ((uint)state <= 1u)
					{
						_valueTaskSource.SetException(exception);
					}
				}
				else if ((uint)state <= 1u)
				{
					_valueTaskSource.SetResult(final);
				}
				if (final)
				{
					if (_finalTaskSource.TryComplete(exception))
					{
						if (state != State.Ready)
						{
							_finalTaskSource.TrySignal(out var _);
						}
						return true;
					}
					return false;
				}
				return state != State.Ready;
			}
			finally
			{
				if (_keepAlive.IsAllocated)
				{
					_keepAlive.Free();
				}
			}
		}
	}

	public bool TrySetResult(bool final = false)
	{
		return TryComplete(null, final);
	}

	public bool TrySetException(Exception exception)
	{
		return TryComplete(exception, final: true);
	}

	ValueTaskSourceStatus IValueTaskSource.GetStatus(short token)
	{
		return _valueTaskSource.GetStatus(token);
	}

	void IValueTaskSource.OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
	{
		_valueTaskSource.OnCompleted(continuation, state, token, flags);
	}

	void IValueTaskSource.GetResult(short token)
	{
		try
		{
			_cancelledToken.ThrowIfCancellationRequested();
			_valueTaskSource.GetResult(token);
		}
		finally
		{
			lock (this)
			{
				State state = _state;
				_hasWaiter = false;
				_cancelledToken = default(CancellationToken);
				if (state == State.Ready)
				{
					_valueTaskSource.Reset();
					_state = State.None;
					if (_finalTaskSource.TrySignal(out var exception))
					{
						_state = State.Completed;
						if (exception != null)
						{
							_valueTaskSource.SetException(exception);
						}
						else
						{
							_valueTaskSource.SetResult(result: true);
						}
					}
					else
					{
						_state = State.None;
					}
				}
			}
		}
	}
}
