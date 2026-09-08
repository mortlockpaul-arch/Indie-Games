using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Threading.Channels;

[DebuggerDisplay("Items = {ItemsCountForDebugger}, Closed = {ChannelIsClosedForDebugger}")]
[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
internal sealed class UnboundedPrioritizedChannel<T> : Channel<T>, IDebugEnumerable<T>
{
	[DebuggerDisplay("Items = {Count}")]
	[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
	private sealed class UnboundedPrioritizedChannelReader : ChannelReader<T>, IDebugEnumerable<T>
	{
		internal readonly UnboundedPrioritizedChannel<T> _parent;

		private readonly BlockedReadAsyncOperation<T> _readerSingleton;

		private readonly WaitingReadAsyncOperation _waiterSingleton;

		public override Task Completion => _parent._completion.Task;

		public override bool CanCount => true;

		public override bool CanPeek => true;

		public override int Count => _parent._items.Count;

		internal UnboundedPrioritizedChannelReader(UnboundedPrioritizedChannel<T> parent)
		{
			_parent = parent;
			_readerSingleton = new BlockedReadAsyncOperation<T>(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
			_waiterSingleton = new WaitingReadAsyncOperation(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
		}

		public override ValueTask<T> ReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ValueTask.FromCanceled<T>(cancellationToken);
			}
			UnboundedPrioritizedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._items.TryDequeue(out var _, out var priority))
				{
					CompleteIfDone(parent);
					return new ValueTask<T>(priority);
				}
				if (parent._doneWriting != null)
				{
					return ChannelUtilities.GetInvalidCompletionValueTask<T>(parent._doneWriting);
				}
				BlockedReadAsyncOperation<T> blockedReadAsyncOperation = ((!cancellationToken.CanBeCanceled && _readerSingleton.TryOwnAndReset()) ? _readerSingleton : new BlockedReadAsyncOperation<T>(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate));
				ChannelUtilities.Enqueue(ref parent._blockedReadersHead, blockedReadAsyncOperation);
				return blockedReadAsyncOperation.ValueTaskOfT;
			}
		}

		public override bool TryRead([MaybeNullWhen(false)] out T item)
		{
			UnboundedPrioritizedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._items.TryDequeue(out var _, out item))
				{
					CompleteIfDone(parent);
					return true;
				}
				item = default(T);
				return false;
			}
		}

		public override bool TryPeek([MaybeNullWhen(false)] out T item)
		{
			UnboundedPrioritizedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				bool element;
				return parent._items.TryPeek(out element, out item);
			}
		}

		private static void CompleteIfDone(UnboundedPrioritizedChannel<T> parent)
		{
			if (parent._doneWriting != null && parent._items.Count == 0)
			{
				ChannelUtilities.Complete(parent._completion, parent._doneWriting);
			}
		}

		public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return ValueTask.FromCanceled<bool>(cancellationToken);
			}
			UnboundedPrioritizedChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._items.Count != 0)
				{
					return new ValueTask<bool>(result: true);
				}
				if (parent._doneWriting != null)
				{
					return (parent._doneWriting != ChannelUtilities.s_doneWritingSentinel) ? ValueTask.FromException<bool>(parent._doneWriting) : default(ValueTask<bool>);
				}
				WaitingReadAsyncOperation waitingReadAsyncOperation = ((!cancellationToken.CanBeCanceled && _waiterSingleton.TryOwnAndReset()) ? _waiterSingleton : new WaitingReadAsyncOperation(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate));
				ChannelUtilities.Enqueue(ref parent._waitingReadersHead, waitingReadAsyncOperation);
				return waitingReadAsyncOperation.ValueTaskOfT;
			}
		}

		IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
		{
			return _parent.GetEnumerator();
		}
	}

	[DebuggerDisplay("Items = {ItemsCountForDebugger}")]
	[DebuggerTypeProxy(typeof(DebugEnumeratorDebugView<>))]
	private sealed class UnboundedPrioritizedChannelWriter : ChannelWriter<T>, IDebugEnumerable<T>
	{
		internal readonly UnboundedPrioritizedChannel<T> _parent;

		private int ItemsCountForDebugger => _parent._items.Count;

		internal UnboundedPrioritizedChannelWriter(UnboundedPrioritizedChannel<T> parent)
		{
			_parent = parent;
		}

		public override bool TryComplete(Exception error)
		{
			UnboundedPrioritizedChannel<T> parent = _parent;
			bool flag;
			BlockedReadAsyncOperation<T> blockedReadersHead;
			WaitingReadAsyncOperation waitingReadersHead;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return false;
				}
				parent._doneWriting = error ?? ChannelUtilities.s_doneWritingSentinel;
				flag = parent._items.Count == 0;
				blockedReadersHead = parent._blockedReadersHead;
				waitingReadersHead = parent._waitingReadersHead;
				parent._blockedReadersHead = null;
				parent._waitingReadersHead = null;
			}
			if (flag)
			{
				ChannelUtilities.Complete(parent._completion, error);
			}
			ChannelUtilities.FailOperations(blockedReadersHead, ChannelUtilities.CreateInvalidCompletionException(error));
			ChannelUtilities.SetOrFailOperations(waitingReadersHead, result: false, error);
			return true;
		}

		public override bool TryWrite(T item)
		{
			UnboundedPrioritizedChannel<T> parent = _parent;
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation = null;
			WaitingReadAsyncOperation waitingReadAsyncOperation = null;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return false;
				}
				blockedReadAsyncOperation = ChannelUtilities.TryDequeueAndReserveCompletionIfCancelable(ref parent._blockedReadersHead);
				if (blockedReadAsyncOperation == null)
				{
					parent._items.Enqueue(element: true, item);
					waitingReadAsyncOperation = ChannelUtilities.TryReserveCompletionIfCancelable(ref parent._waitingReadersHead);
				}
			}
			if (blockedReadAsyncOperation != null)
			{
				blockedReadAsyncOperation.DangerousSetResult(item);
			}
			else if (waitingReadAsyncOperation != null)
			{
				ChannelUtilities.DangerousSetOperations(waitingReadAsyncOperation, result: true);
			}
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
					return ValueTask.FromException<bool>(doneWriting);
				}
				return new ValueTask<bool>(result: true);
			}
			return ValueTask.FromCanceled<bool>(cancellationToken);
		}

		public override ValueTask WriteAsync(T item, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				if (!TryWrite(item))
				{
					return ValueTask.FromException(ChannelUtilities.CreateInvalidCompletionException(_parent._doneWriting));
				}
				return default(ValueTask);
			}
			return ValueTask.FromCanceled(cancellationToken);
		}

		IEnumerator<T> IDebugEnumerable<T>.GetEnumerator()
		{
			return _parent.GetEnumerator();
		}
	}

	private readonly TaskCompletionSource _completion;

	private readonly PriorityQueue<bool, T> _items;

	private readonly bool _runContinuationsAsynchronously;

	private BlockedReadAsyncOperation<T> _blockedReadersHead;

	private WaitingReadAsyncOperation _waitingReadersHead;

	private Exception _doneWriting;

	private object SyncObj => _items;

	private Action<object, CancellationToken> CancellationCallbackDelegate => delegate(object state, CancellationToken cancellationToken)
	{
		AsyncOperation asyncOperation = (AsyncOperation)state;
		if (asyncOperation.TrySetCanceled(cancellationToken))
		{
			ChannelUtilities.UnsafeQueueUserWorkItem(delegate(KeyValuePair<UnboundedPrioritizedChannel<T>, AsyncOperation> keyValuePair)
			{
				lock (keyValuePair.Key.SyncObj)
				{
					AsyncOperation value = keyValuePair.Value;
					if (!(value is BlockedReadAsyncOperation<T> op))
					{
						if (value is WaitingReadAsyncOperation op2)
						{
							ChannelUtilities.Remove(ref keyValuePair.Key._waitingReadersHead, op2);
						}
					}
					else
					{
						ChannelUtilities.Remove(ref keyValuePair.Key._blockedReadersHead, op);
					}
				}
			}, new KeyValuePair<UnboundedPrioritizedChannel<T>, AsyncOperation>(this, asyncOperation));
		}
	};

	private int ItemsCountForDebugger => _items.Count;

	private bool ChannelIsClosedForDebugger => _doneWriting != null;

	internal UnboundedPrioritizedChannel(bool runContinuationsAsynchronously, IComparer<T> comparer)
	{
		_runContinuationsAsynchronously = runContinuationsAsynchronously;
		_completion = new TaskCompletionSource(runContinuationsAsynchronously ? TaskCreationOptions.RunContinuationsAsynchronously : TaskCreationOptions.None);
		_items = new PriorityQueue<bool, T>(comparer);
		base.Reader = new UnboundedPrioritizedChannelReader(this);
		base.Writer = new UnboundedPrioritizedChannelWriter(this);
	}

	public IEnumerator<T> GetEnumerator()
	{
		List<T> list = new List<T>();
		foreach (var unorderedItem in _items.UnorderedItems)
		{
			list.Add(unorderedItem.Priority);
		}
		list.Sort(_items.Comparer);
		return list.GetEnumerator();
	}
}
