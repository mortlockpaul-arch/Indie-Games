using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Threading.Channels;

[DebuggerDisplay("Items = {ItemsCountForDebugger}, Closed = {ChannelIsClosedForDebugger}")]
[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
internal sealed class SingleConsumerUnboundedChannel<T> : Channel<T>, IDebugEnumerable<T>
{
	[DebuggerDisplay("Items = {ItemsCountForDebugger}")]
	[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
	private sealed class UnboundedChannelReader : ChannelReader<T>, IDebugEnumerable<T>
	{
		internal readonly SingleConsumerUnboundedChannel<T> _parent;

		private readonly BlockedReadAsyncOperation<T> _readerSingleton;

		private readonly WaitingReadAsyncOperation _waiterSingleton;

		public override Task Completion => _parent._completion.Task;

		public override bool CanPeek => true;

		private int ItemsCountForDebugger => _parent._items.Count;

		internal UnboundedChannelReader(SingleConsumerUnboundedChannel<T> parent)
		{
			_parent = parent;
			_readerSingleton = new BlockedReadAsyncOperation<T>(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
			_waiterSingleton = new WaitingReadAsyncOperation(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
		}

		public override ValueTask<T> ReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<T>(Task.FromCanceled<T>(cancellationToken));
			}
			if (TryRead(out var item))
			{
				return new ValueTask<T>(item);
			}
			SingleConsumerUnboundedChannel<T> parent = _parent;
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation;
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation2;
			lock (parent.SyncObj)
			{
				if (TryRead(out item))
				{
					return new ValueTask<T>(item);
				}
				if (parent._doneWriting != null)
				{
					return ChannelUtilities.GetInvalidCompletionValueTask<T>(parent._doneWriting);
				}
				blockedReadAsyncOperation = parent._blockedReader;
				if (!cancellationToken.CanBeCanceled && _readerSingleton.TryOwnAndReset())
				{
					blockedReadAsyncOperation2 = _readerSingleton;
					if (blockedReadAsyncOperation2 == blockedReadAsyncOperation)
					{
						blockedReadAsyncOperation = null;
					}
				}
				else
				{
					blockedReadAsyncOperation2 = new BlockedReadAsyncOperation<T>(_parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate);
				}
				parent._blockedReader = blockedReadAsyncOperation2;
			}
			blockedReadAsyncOperation?.TrySetCanceled();
			return blockedReadAsyncOperation2.ValueTaskOfT;
		}

		public override bool TryRead([MaybeNullWhen(false)] out T item)
		{
			SingleConsumerUnboundedChannel<T> parent = _parent;
			if (parent._items.TryDequeue(out item))
			{
				if (parent._doneWriting != null && parent._items.IsEmpty)
				{
					ChannelUtilities.Complete(parent._completion, parent._doneWriting);
				}
				return true;
			}
			return false;
		}

		public override bool TryPeek([MaybeNullWhen(false)] out T item)
		{
			return _parent._items.TryPeek(out item);
		}

		public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<bool>(Task.FromCanceled<bool>(cancellationToken));
			}
			if (!_parent._items.IsEmpty)
			{
				return new ValueTask<bool>(result: true);
			}
			SingleConsumerUnboundedChannel<T> parent = _parent;
			WaitingReadAsyncOperation waitingReadAsyncOperation = null;
			WaitingReadAsyncOperation waitingReadAsyncOperation2;
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
				waitingReadAsyncOperation = parent._waitingReader;
				if (!cancellationToken.CanBeCanceled && _waiterSingleton.TryOwnAndReset())
				{
					waitingReadAsyncOperation2 = _waiterSingleton;
					if (waitingReadAsyncOperation2 == waitingReadAsyncOperation)
					{
						waitingReadAsyncOperation = null;
					}
				}
				else
				{
					waitingReadAsyncOperation2 = new WaitingReadAsyncOperation(_parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate);
				}
				parent._waitingReader = waitingReadAsyncOperation2;
			}
			waitingReadAsyncOperation?.TrySetCanceled();
			return waitingReadAsyncOperation2.ValueTaskOfT;
		}

		IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
		{
			return _parent._items.GetEnumerator();
		}
	}

	[DebuggerDisplay("Items = {ItemsCountForDebugger}")]
	[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
	private sealed class UnboundedChannelWriter : ChannelWriter<T>, IDebugEnumerable<T>
	{
		internal readonly SingleConsumerUnboundedChannel<T> _parent;

		private int ItemsCountForDebugger => _parent._items.Count;

		internal UnboundedChannelWriter(SingleConsumerUnboundedChannel<T> parent)
		{
			_parent = parent;
		}

		public override bool TryComplete(Exception error)
		{
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation = null;
			WaitingReadAsyncOperation waitingReadAsyncOperation = null;
			bool flag = false;
			SingleConsumerUnboundedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return false;
				}
				parent._doneWriting = error ?? ChannelUtilities.s_doneWritingSentinel;
				if (parent._items.IsEmpty)
				{
					flag = true;
					if (parent._blockedReader != null)
					{
						blockedReadAsyncOperation = parent._blockedReader;
						parent._blockedReader = null;
					}
					if (parent._waitingReader != null)
					{
						waitingReadAsyncOperation = parent._waitingReader;
						parent._waitingReader = null;
					}
				}
			}
			if (flag)
			{
				ChannelUtilities.Complete(parent._completion, error);
			}
			if (blockedReadAsyncOperation != null)
			{
				error = ChannelUtilities.CreateInvalidCompletionException(error);
				blockedReadAsyncOperation.TrySetException(error);
			}
			if (waitingReadAsyncOperation != null)
			{
				if (error != null)
				{
					waitingReadAsyncOperation.TrySetException(error);
				}
				else
				{
					waitingReadAsyncOperation.TrySetResult(result: false);
				}
			}
			return true;
		}

		public override bool TryWrite(T item)
		{
			SingleConsumerUnboundedChannel<T> parent = _parent;
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation;
			do
			{
				blockedReadAsyncOperation = null;
				WaitingReadAsyncOperation waitingReadAsyncOperation = null;
				lock (parent.SyncObj)
				{
					if (parent._doneWriting != null)
					{
						return false;
					}
					blockedReadAsyncOperation = parent._blockedReader;
					if (blockedReadAsyncOperation != null)
					{
						parent._blockedReader = null;
					}
					else
					{
						parent._items.Enqueue(item);
						waitingReadAsyncOperation = parent._waitingReader;
						if (waitingReadAsyncOperation == null)
						{
							return true;
						}
						parent._waitingReader = null;
					}
				}
				if (waitingReadAsyncOperation != null)
				{
					waitingReadAsyncOperation.TrySetResult(result: true);
					return true;
				}
			}
			while (!blockedReadAsyncOperation.TrySetResult(item));
			return true;
		}

		public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken)
		{
			Exception doneWriting = _parent._doneWriting;
			if (!cancellationToken.IsCancellationRequested)
			{
				if (doneWriting != null)
				{
					if (doneWriting == ChannelUtilities.s_doneWritingSentinel)
					{
						return default(ValueTask<bool>);
					}
					return new ValueTask<bool>(Task.FromException<bool>(doneWriting));
				}
				return new ValueTask<bool>(result: true);
			}
			return new ValueTask<bool>(Task.FromCanceled<bool>(cancellationToken));
		}

		public override ValueTask WriteAsync(T item, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				if (!TryWrite(item))
				{
					return new ValueTask(Task.FromException(ChannelUtilities.CreateInvalidCompletionException(_parent._doneWriting)));
				}
				return default(ValueTask);
			}
			return new ValueTask(Task.FromCanceled(cancellationToken));
		}

		IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
		{
			return _parent._items.GetEnumerator();
		}
	}

	private readonly TaskCompletionSource _completion;

	private readonly System.Collections.Concurrent.SingleProducerSingleConsumerQueue<T> _items = new System.Collections.Concurrent.SingleProducerSingleConsumerQueue<T>();

	private readonly bool _runContinuationsAsynchronously;

	private volatile Exception _doneWriting;

	private BlockedReadAsyncOperation<T> _blockedReader;

	private WaitingReadAsyncOperation _waitingReader;

	private object SyncObj => _items;

	private Action<object, CancellationToken> CancellationCallbackDelegate => delegate(object state, CancellationToken cancellationToken)
	{
		AsyncOperation asyncOperation = (AsyncOperation)state;
		if (asyncOperation.TrySetCanceled(cancellationToken))
		{
			ChannelUtilities.UnsafeQueueUserWorkItem(delegate(KeyValuePair<SingleConsumerUnboundedChannel<T>, AsyncOperation> keyValuePair)
			{
				lock (keyValuePair.Key.SyncObj)
				{
					AsyncOperation value = keyValuePair.Value;
					if (!(value is BlockedReadAsyncOperation<T> blockedReadAsyncOperation))
					{
						if (value is WaitingReadAsyncOperation waitingReadAsyncOperation && keyValuePair.Key._waitingReader == waitingReadAsyncOperation)
						{
							keyValuePair.Key._waitingReader = null;
						}
					}
					else if (keyValuePair.Key._blockedReader == blockedReadAsyncOperation)
					{
						keyValuePair.Key._blockedReader = null;
					}
				}
			}, new KeyValuePair<SingleConsumerUnboundedChannel<T>, AsyncOperation>(this, asyncOperation));
		}
	};

	private int ItemsCountForDebugger => _items.Count;

	private bool ChannelIsClosedForDebugger => _doneWriting != null;

	internal SingleConsumerUnboundedChannel(bool runContinuationsAsynchronously)
	{
		_runContinuationsAsynchronously = runContinuationsAsynchronously;
		_completion = new TaskCompletionSource(runContinuationsAsynchronously ? TaskCreationOptions.RunContinuationsAsynchronously : TaskCreationOptions.None);
		base.Reader = new UnboundedChannelReader(this);
		base.Writer = new UnboundedChannelWriter(this);
	}

	IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
	{
		return _items.GetEnumerator();
	}
}
