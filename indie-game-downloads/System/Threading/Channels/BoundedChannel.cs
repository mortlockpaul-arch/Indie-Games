using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Threading.Channels;

[DebuggerDisplay("Items = {ItemsCountForDebugger}, Capacity = {_bufferedCapacity}, Mode = {_mode}, Closed = {ChannelIsClosedForDebugger}")]
[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
internal sealed class BoundedChannel<T> : Channel<T>, IDebugEnumerable<T>
{
	[DebuggerDisplay("Items = {ItemsCountForDebugger}")]
	[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
	private sealed class BoundedChannelReader : ChannelReader<T>, IDebugEnumerable<T>
	{
		internal readonly BoundedChannel<T> _parent;

		private readonly BlockedReadAsyncOperation<T> _readerSingleton;

		private readonly WaitingReadAsyncOperation _waiterSingleton;

		public override Task Completion => _parent._completion.Task;

		public override bool CanCount => true;

		public override bool CanPeek => true;

		public override int Count
		{
			get
			{
				BoundedChannel<T> parent = _parent;
				lock (parent.SyncObj)
				{
					return parent._items.Count;
				}
			}
		}

		private int ItemsCountForDebugger => _parent._items.Count;

		internal BoundedChannelReader(BoundedChannel<T> parent)
		{
			_parent = parent;
			_readerSingleton = new BlockedReadAsyncOperation<T>(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
			_waiterSingleton = new WaitingReadAsyncOperation(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
		}

		public override bool TryRead([MaybeNullWhen(false)] out T item)
		{
			BoundedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (!parent._items.IsEmpty)
				{
					item = DequeueItemAndPostProcess();
					return true;
				}
			}
			item = default(T);
			return false;
		}

		public override bool TryPeek([MaybeNullWhen(false)] out T item)
		{
			BoundedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (!parent._items.IsEmpty)
				{
					item = parent._items.PeekHead();
					return true;
				}
			}
			item = default(T);
			return false;
		}

		public override ValueTask<T> ReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<T>(Task.FromCanceled<T>(cancellationToken));
			}
			BoundedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (!parent._items.IsEmpty)
				{
					return new ValueTask<T>(DequeueItemAndPostProcess());
				}
				if (parent._doneWriting != null)
				{
					return ChannelUtilities.GetInvalidCompletionValueTask<T>(parent._doneWriting);
				}
				if (!cancellationToken.CanBeCanceled)
				{
					BlockedReadAsyncOperation<T> readerSingleton = _readerSingleton;
					if (readerSingleton.TryOwnAndReset())
					{
						ChannelUtilities.Enqueue(ref parent._blockedReadersHead, readerSingleton);
						return readerSingleton.ValueTaskOfT;
					}
				}
				BlockedReadAsyncOperation<T> blockedReadAsyncOperation = new BlockedReadAsyncOperation<T>(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate);
				ChannelUtilities.Enqueue(ref parent._blockedReadersHead, blockedReadAsyncOperation);
				return blockedReadAsyncOperation.ValueTaskOfT;
			}
		}

		public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<bool>(Task.FromCanceled<bool>(cancellationToken));
			}
			BoundedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (!parent._items.IsEmpty)
				{
					return new ValueTask<bool>(result: true);
				}
				if (parent._doneWriting != null)
				{
					return (parent._doneWriting != ChannelUtilities.s_doneWritingSentinel) ? new ValueTask<bool>(Task.FromException<bool>(parent._doneWriting)) : default(ValueTask<bool>);
				}
				if (!cancellationToken.CanBeCanceled)
				{
					WaitingReadAsyncOperation waiterSingleton = _waiterSingleton;
					if (waiterSingleton.TryOwnAndReset())
					{
						ChannelUtilities.Enqueue(ref parent._waitingReadersHead, waiterSingleton);
						return waiterSingleton.ValueTaskOfT;
					}
				}
				WaitingReadAsyncOperation waitingReadAsyncOperation = new WaitingReadAsyncOperation(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate);
				ChannelUtilities.Enqueue(ref parent._waitingReadersHead, waitingReadAsyncOperation);
				return waitingReadAsyncOperation.ValueTaskOfT;
			}
		}

		private T DequeueItemAndPostProcess()
		{
			BoundedChannel<T> parent = _parent;
			T result = parent._items.DequeueHead();
			if (parent._doneWriting != null)
			{
				if (parent._items.IsEmpty)
				{
					ChannelUtilities.Complete(parent._completion, parent._doneWriting);
				}
			}
			else
			{
				BlockedWriteAsyncOperation<T> op;
				while (ChannelUtilities.TryDequeue(ref parent._blockedWritersHead, out op))
				{
					if (op.TrySetResult(default(VoidResult)))
					{
						parent._items.EnqueueTail(op.Item);
						return result;
					}
				}
				ChannelUtilities.SetOperations(ref parent._waitingWritersHead, result: true);
			}
			return result;
		}

		IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
		{
			return _parent._items.GetEnumerator();
		}
	}

	[DebuggerDisplay("Items = {ItemsCountForDebugger}, Capacity = {CapacityForDebugger}")]
	[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
	private sealed class BoundedChannelWriter : ChannelWriter<T>, IDebugEnumerable<T>
	{
		internal readonly BoundedChannel<T> _parent;

		private readonly BlockedWriteAsyncOperation<T> _writerSingleton;

		private readonly WaitingWriteAsyncOperation _waiterSingleton;

		private int ItemsCountForDebugger => _parent._items.Count;

		private int CapacityForDebugger => _parent._bufferedCapacity;

		internal BoundedChannelWriter(BoundedChannel<T> parent)
		{
			_parent = parent;
			_writerSingleton = new BlockedWriteAsyncOperation<T>(runContinuationsAsynchronously: true, default(CancellationToken), pooled: true);
			_waiterSingleton = new WaitingWriteAsyncOperation(runContinuationsAsynchronously: true, default(CancellationToken), pooled: true);
		}

		public override bool TryComplete(Exception error)
		{
			BoundedChannel<T> parent = _parent;
			bool isEmpty;
			BlockedReadAsyncOperation<T> blockedReadersHead;
			BlockedWriteAsyncOperation<T> blockedWritersHead;
			WaitingReadAsyncOperation waitingReadersHead;
			WaitingWriteAsyncOperation waitingWritersHead;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return false;
				}
				parent._doneWriting = error ?? ChannelUtilities.s_doneWritingSentinel;
				isEmpty = parent._items.IsEmpty;
				blockedReadersHead = parent._blockedReadersHead;
				blockedWritersHead = parent._blockedWritersHead;
				waitingReadersHead = parent._waitingReadersHead;
				waitingWritersHead = parent._waitingWritersHead;
				parent._blockedReadersHead = null;
				parent._blockedWritersHead = null;
				parent._waitingReadersHead = null;
				parent._waitingWritersHead = null;
			}
			if (isEmpty)
			{
				ChannelUtilities.Complete(parent._completion, error);
			}
			ChannelUtilities.FailOperations(blockedReadersHead, ChannelUtilities.CreateInvalidCompletionException(error));
			ChannelUtilities.FailOperations(blockedWritersHead, ChannelUtilities.CreateInvalidCompletionException(error));
			ChannelUtilities.SetOrFailOperations(waitingReadersHead, result: false, error);
			ChannelUtilities.SetOrFailOperations(waitingWritersHead, result: false, error);
			return true;
		}

		public override bool TryWrite(T item)
		{
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation = null;
			WaitingReadAsyncOperation waitingReadAsyncOperation = null;
			BoundedChannel<T> parent = _parent;
			bool lockTaken = false;
			try
			{
				Monitor.Enter(parent.SyncObj, ref lockTaken);
				if (parent._doneWriting != null)
				{
					return false;
				}
				int count = parent._items.Count;
				if (count != 0)
				{
					if (count < parent._bufferedCapacity)
					{
						parent._items.EnqueueTail(item);
						return true;
					}
					if (parent._mode == BoundedChannelFullMode.Wait)
					{
						return false;
					}
					if (parent._mode == BoundedChannelFullMode.DropWrite)
					{
						Monitor.Exit(parent.SyncObj);
						lockTaken = false;
						parent._itemDropped?.Invoke(item);
						return true;
					}
					T obj = ((parent._mode == BoundedChannelFullMode.DropNewest) ? parent._items.DequeueTail() : parent._items.DequeueHead());
					parent._items.EnqueueTail(item);
					Monitor.Exit(parent.SyncObj);
					lockTaken = false;
					parent._itemDropped?.Invoke(obj);
					return true;
				}
				blockedReadAsyncOperation = ChannelUtilities.TryDequeueAndReserveCompletionIfCancelable(ref parent._blockedReadersHead);
				if (blockedReadAsyncOperation == null)
				{
					parent._items.EnqueueTail(item);
					waitingReadAsyncOperation = ChannelUtilities.TryReserveCompletionIfCancelable(ref parent._waitingReadersHead);
					if (waitingReadAsyncOperation == null)
					{
						return true;
					}
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(parent.SyncObj);
				}
			}
			if (blockedReadAsyncOperation != null)
			{
				blockedReadAsyncOperation.DangerousSetResult(item);
			}
			else
			{
				ChannelUtilities.DangerousSetOperations(waitingReadAsyncOperation, result: true);
			}
			return true;
		}

		public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<bool>(Task.FromCanceled<bool>(cancellationToken));
			}
			BoundedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return (parent._doneWriting != ChannelUtilities.s_doneWritingSentinel) ? new ValueTask<bool>(Task.FromException<bool>(parent._doneWriting)) : default(ValueTask<bool>);
				}
				if (parent._items.Count < parent._bufferedCapacity || parent._mode != BoundedChannelFullMode.Wait)
				{
					return new ValueTask<bool>(result: true);
				}
				if (!cancellationToken.CanBeCanceled)
				{
					WaitingWriteAsyncOperation waiterSingleton = _waiterSingleton;
					if (waiterSingleton.TryOwnAndReset())
					{
						ChannelUtilities.Enqueue(ref parent._waitingWritersHead, waiterSingleton);
						return waiterSingleton.ValueTaskOfT;
					}
				}
				WaitingWriteAsyncOperation waitingWriteAsyncOperation = new WaitingWriteAsyncOperation(runContinuationsAsynchronously: true, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate);
				ChannelUtilities.Enqueue(ref parent._waitingWritersHead, waitingWriteAsyncOperation);
				return waitingWriteAsyncOperation.ValueTaskOfT;
			}
		}

		public override ValueTask WriteAsync(T item, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask(Task.FromCanceled(cancellationToken));
			}
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation = null;
			WaitingReadAsyncOperation waitingReadAsyncOperation = null;
			BoundedChannel<T> parent = _parent;
			bool lockTaken = false;
			try
			{
				Monitor.Enter(parent.SyncObj, ref lockTaken);
				if (parent._doneWriting != null)
				{
					return new ValueTask(Task.FromException(ChannelUtilities.CreateInvalidCompletionException(parent._doneWriting)));
				}
				int count = parent._items.Count;
				if (count != 0)
				{
					if (count < parent._bufferedCapacity)
					{
						parent._items.EnqueueTail(item);
						return default(ValueTask);
					}
					if (parent._mode == BoundedChannelFullMode.Wait)
					{
						if (!cancellationToken.CanBeCanceled)
						{
							BlockedWriteAsyncOperation<T> writerSingleton = _writerSingleton;
							if (writerSingleton.TryOwnAndReset())
							{
								writerSingleton.Item = item;
								ChannelUtilities.Enqueue(ref parent._blockedWritersHead, writerSingleton);
								return writerSingleton.ValueTask;
							}
						}
						BlockedWriteAsyncOperation<T> blockedWriteAsyncOperation = new BlockedWriteAsyncOperation<T>(runContinuationsAsynchronously: true, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate)
						{
							Item = item
						};
						ChannelUtilities.Enqueue(ref parent._blockedWritersHead, blockedWriteAsyncOperation);
						return blockedWriteAsyncOperation.ValueTask;
					}
					if (parent._mode == BoundedChannelFullMode.DropWrite)
					{
						Monitor.Exit(parent.SyncObj);
						lockTaken = false;
						parent._itemDropped?.Invoke(item);
						return default(ValueTask);
					}
					T obj = ((parent._mode == BoundedChannelFullMode.DropNewest) ? parent._items.DequeueTail() : parent._items.DequeueHead());
					parent._items.EnqueueTail(item);
					Monitor.Exit(parent.SyncObj);
					lockTaken = false;
					parent._itemDropped?.Invoke(obj);
					return default(ValueTask);
				}
				blockedReadAsyncOperation = ChannelUtilities.TryDequeueAndReserveCompletionIfCancelable(ref parent._blockedReadersHead);
				if (blockedReadAsyncOperation == null)
				{
					parent._items.EnqueueTail(item);
					waitingReadAsyncOperation = ChannelUtilities.TryReserveCompletionIfCancelable(ref parent._waitingReadersHead);
					if (waitingReadAsyncOperation == null)
					{
						return default(ValueTask);
					}
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(parent.SyncObj);
				}
			}
			if (blockedReadAsyncOperation != null)
			{
				blockedReadAsyncOperation.DangerousSetResult(item);
			}
			else
			{
				ChannelUtilities.DangerousSetOperations(waitingReadAsyncOperation, result: true);
			}
			return default(ValueTask);
		}

		IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
		{
			return _parent._items.GetEnumerator();
		}
	}

	private readonly BoundedChannelFullMode _mode;

	private readonly Action<T> _itemDropped;

	private readonly TaskCompletionSource _completion;

	private readonly int _bufferedCapacity;

	private readonly Deque<T> _items = new Deque<T>();

	private BlockedReadAsyncOperation<T> _blockedReadersHead;

	private BlockedWriteAsyncOperation<T> _blockedWritersHead;

	private WaitingReadAsyncOperation _waitingReadersHead;

	private WaitingWriteAsyncOperation _waitingWritersHead;

	private readonly bool _runContinuationsAsynchronously;

	private Exception _doneWriting;

	private object SyncObj => _items;

	private Action<object, CancellationToken> CancellationCallbackDelegate => delegate(object state, CancellationToken cancellationToken)
	{
		AsyncOperation asyncOperation = (AsyncOperation)state;
		if (asyncOperation.TrySetCanceled(cancellationToken))
		{
			ChannelUtilities.UnsafeQueueUserWorkItem(delegate(KeyValuePair<BoundedChannel<T>, AsyncOperation> keyValuePair)
			{
				lock (keyValuePair.Key.SyncObj)
				{
					AsyncOperation value = keyValuePair.Value;
					if (!(value is BlockedReadAsyncOperation<T> op))
					{
						if (!(value is BlockedWriteAsyncOperation<T> op2))
						{
							if (!(value is WaitingReadAsyncOperation op3))
							{
								if (value is WaitingWriteAsyncOperation op4)
								{
									ChannelUtilities.Remove(ref keyValuePair.Key._waitingWritersHead, op4);
								}
							}
							else
							{
								ChannelUtilities.Remove(ref keyValuePair.Key._waitingReadersHead, op3);
							}
						}
						else
						{
							ChannelUtilities.Remove(ref keyValuePair.Key._blockedWritersHead, op2);
						}
					}
					else
					{
						ChannelUtilities.Remove(ref keyValuePair.Key._blockedReadersHead, op);
					}
				}
			}, new KeyValuePair<BoundedChannel<T>, AsyncOperation>(this, asyncOperation));
		}
	};

	private int ItemsCountForDebugger => _items.Count;

	private bool ChannelIsClosedForDebugger => _doneWriting != null;

	internal BoundedChannel(int bufferedCapacity, BoundedChannelFullMode mode, bool runContinuationsAsynchronously, Action<T> itemDropped)
	{
		_bufferedCapacity = bufferedCapacity;
		_mode = mode;
		_runContinuationsAsynchronously = runContinuationsAsynchronously;
		_itemDropped = itemDropped;
		_completion = new TaskCompletionSource(runContinuationsAsynchronously ? TaskCreationOptions.RunContinuationsAsynchronously : TaskCreationOptions.None);
		base.Reader = new BoundedChannelReader(this);
		base.Writer = new BoundedChannelWriter(this);
	}

	IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
	{
		return _items.GetEnumerator();
	}
}
