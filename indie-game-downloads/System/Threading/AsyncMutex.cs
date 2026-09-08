using System.Net;
using System.Threading.Tasks;

namespace System.Threading;

internal sealed class AsyncMutex
{
	private sealed class Waiter : TaskCompletionSource
	{
		public AsyncMutex Owner { get; }

		public CancellationTokenRegistration CancellationRegistration { get; set; }

		public Waiter Next { get; set; }

		public Waiter Prev { get; set; }

		public Waiter(AsyncMutex owner)
			: base(TaskCreationOptions.RunContinuationsAsynchronously)
		{
			Owner = owner;
		}
	}

	private int _gate = 1;

	private bool _lockedSemaphoreFull = true;

	private Waiter _waitersTail;

	private object SyncObj => this;

	public Task EnterAsync(CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		int num = Interlocked.Decrement(ref _gate);
		if (num >= 0)
		{
			return Task.CompletedTask;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Trace(this, $"Waiting to enter, queue length {-num}", "EnterAsync");
		}
		return Contended(cancellationToken);
		Task Contended(CancellationToken cancellationToken2)
		{
			Waiter waiter = new Waiter(this);
			waiter.CancellationRegistration = cancellationToken2.UnsafeRegister(delegate(object s, CancellationToken token)
			{
				OnCancellation(s, token);
			}, waiter);
			lock (SyncObj)
			{
				if (!_lockedSemaphoreFull)
				{
					waiter.CancellationRegistration.Unregister();
					_lockedSemaphoreFull = true;
					return Task.CompletedTask;
				}
				if (cancellationToken2.IsCancellationRequested)
				{
					waiter.TrySetCanceled(cancellationToken2);
					return waiter.Task;
				}
				if (_waitersTail == null)
				{
					Waiter next = (waiter.Prev = waiter);
					waiter.Next = next;
				}
				else
				{
					waiter.Next = _waitersTail;
					waiter.Prev = _waitersTail.Prev;
					Waiter prev = waiter.Prev;
					Waiter next = (waiter.Next.Prev = waiter);
					prev.Next = next;
				}
				_waitersTail = waiter;
			}
			return waiter.Task;
		}
		static void OnCancellation(object state, CancellationToken cancellationToken2)
		{
			Waiter waiter = (Waiter)state;
			AsyncMutex owner = waiter.Owner;
			lock (owner.SyncObj)
			{
				if (waiter.Next != null)
				{
					Interlocked.Increment(ref owner._gate);
					if (waiter.Next == waiter)
					{
						owner._waitersTail = null;
					}
					else
					{
						waiter.Next.Prev = waiter.Prev;
						waiter.Prev.Next = waiter.Next;
						if (owner._waitersTail == waiter)
						{
							owner._waitersTail = waiter.Next;
						}
					}
					Waiter waiter2 = waiter;
					Waiter next = (waiter.Prev = null);
					waiter2.Next = next;
				}
				else
				{
					waiter = null;
				}
			}
			waiter?.TrySetCanceled(cancellationToken2);
		}
	}

	public void Exit()
	{
		if (Interlocked.Increment(ref _gate) < 1)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Trace(this, $"Unblocking next waiter on exit, remaining queue length {-_gate}", "Exit");
			}
			Contended();
		}
		void Contended()
		{
			Waiter waiter;
			lock (SyncObj)
			{
				waiter = _waitersTail;
				if (waiter == null)
				{
					_lockedSemaphoreFull = false;
				}
				else
				{
					if (waiter.Next == waiter)
					{
						_waitersTail = null;
					}
					else
					{
						waiter = waiter.Prev;
						waiter.Next.Prev = waiter.Prev;
						waiter.Prev.Next = waiter.Next;
					}
					Waiter waiter2 = waiter;
					Waiter next = (waiter.Prev = null);
					waiter2.Next = next;
				}
			}
			if (waiter != null)
			{
				waiter.CancellationRegistration.Unregister();
				waiter.TrySetResult();
			}
		}
	}
}
