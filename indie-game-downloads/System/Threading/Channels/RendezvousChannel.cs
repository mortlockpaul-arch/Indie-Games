using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace System.Threading.Channels;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal sealed class RendezvousChannel<T> : Channel<T>
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	private sealed class RendezvousChannelReader : ChannelReader<T>
	{
		internal readonly RendezvousChannel<T> _parent;

		private readonly BlockedReadAsyncOperation<T> _readerSingleton;

		private readonly WaitingReadAsyncOperation _waiterSingleton;

		public override Task Completion => _parent._completion.Task;

		public override bool CanCount => true;

		public override bool CanPeek => true;

		public override int Count => 0;

		internal string DebuggerDisplay
		{
			get
			{
				long value;
				long value2;
				lock (_parent.SyncObj)
				{
					value = ChannelUtilities.CountOperations(_parent._blockedReadersHead);
					value2 = ChannelUtilities.CountOperations(_parent._waitingReadersHead);
				}
				return $"ReadAsync={value}, WaitToReadAsync={value2}";
			}
		}

		internal RendezvousChannelReader(RendezvousChannel<T> parent)
		{
			_parent = parent;
			_readerSingleton = new BlockedReadAsyncOperation<T>(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
			_waiterSingleton = new WaitingReadAsyncOperation(parent._runContinuationsAsynchronously, default(CancellationToken), pooled: true);
		}

		public override bool TryRead([MaybeNullWhen(false)] out T item)
		{
			RendezvousChannel<T> parent = _parent;
			BlockedWriteAsyncOperation<T> blockedWriteAsyncOperation = null;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting == null)
				{
					blockedWriteAsyncOperation = ChannelUtilities.TryDequeueAndReserveCompletionIfCancelable(ref parent._blockedWritersHead);
				}
			}
			if (blockedWriteAsyncOperation != null)
			{
				item = blockedWriteAsyncOperation.Item;
				blockedWriteAsyncOperation.DangerousSetResult(default(VoidResult));
				return true;
			}
			item = default(T);
			return false;
		}

		public override bool TryPeek([MaybeNullWhen(false)] out T item)
		{
			RendezvousChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting == null)
				{
					BlockedWriteAsyncOperation<T> blockedWritersHead = parent._blockedWritersHead;
					if (blockedWritersHead != null)
					{
						item = blockedWritersHead.Item;
						return true;
					}
				}
			}
			item = default(T);
			return false;
		}

		public override ValueTask<T> ReadAsync(CancellationToken cancellationToken)
		{
			RendezvousChannel<T> parent = _parent;
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<T>(Task.FromCanceled<T>(cancellationToken));
			}
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation = null;
			WaitingWriteAsyncOperation head = null;
			BlockedWriteAsyncOperation<T> blockedWriteAsyncOperation = null;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return ChannelUtilities.GetInvalidCompletionValueTask<T>(parent._doneWriting);
				}
				blockedWriteAsyncOperation = ChannelUtilities.TryDequeueAndReserveCompletionIfCancelable(ref parent._blockedWritersHead);
				if (blockedWriteAsyncOperation == null)
				{
					blockedReadAsyncOperation = ((!cancellationToken.CanBeCanceled && _readerSingleton.TryOwnAndReset()) ? _readerSingleton : new BlockedReadAsyncOperation<T>(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate));
					ChannelUtilities.Enqueue(ref parent._blockedReadersHead, blockedReadAsyncOperation);
					head = ChannelUtilities.TryReserveCompletionIfCancelable(ref parent._waitingWritersHead);
				}
			}
			if (blockedWriteAsyncOperation != null)
			{
				ValueTask<T> result = new ValueTask<T>(blockedWriteAsyncOperation.Item);
				blockedWriteAsyncOperation.DangerousSetResult(default(VoidResult));
				return result;
			}
			ChannelUtilities.DangerousSetOperations(head, result: true);
			return blockedReadAsyncOperation.ValueTaskOfT;
		}

		public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<bool>(Task.FromCanceled<bool>(cancellationToken));
			}
			RendezvousChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return (parent._doneWriting != ChannelUtilities.s_doneWritingSentinel) ? new ValueTask<bool>(Task.FromException<bool>(parent._doneWriting)) : default(ValueTask<bool>);
				}
				if (parent._blockedWritersHead != null)
				{
					return new ValueTask<bool>(result: true);
				}
				WaitingReadAsyncOperation waitingReadAsyncOperation = ((!cancellationToken.CanBeCanceled && _waiterSingleton.TryOwnAndReset()) ? _waiterSingleton : new WaitingReadAsyncOperation(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate));
				ChannelUtilities.Enqueue(ref parent._waitingReadersHead, waitingReadAsyncOperation);
				return waitingReadAsyncOperation.ValueTaskOfT;
			}
		}
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	private sealed class RendezvousChannelWriter : ChannelWriter<T>
	{
		internal readonly RendezvousChannel<T> _parent;

		private readonly BlockedWriteAsyncOperation<T> _writerSingleton;

		private readonly WaitingWriteAsyncOperation _waiterSingleton;

		internal string DebuggerDisplay
		{
			get
			{
				long value;
				long value2;
				lock (_parent.SyncObj)
				{
					value = ChannelUtilities.CountOperations(_parent._blockedWritersHead);
					value2 = ChannelUtilities.CountOperations(_parent._waitingWritersHead);
				}
				return $"WriteAsync={value}, WaitToWriteAsync={value2}";
			}
		}

		internal RendezvousChannelWriter(RendezvousChannel<T> parent)
		{
			_parent = parent;
			_writerSingleton = new BlockedWriteAsyncOperation<T>(runContinuationsAsynchronously: true, default(CancellationToken), pooled: true);
			_waiterSingleton = new WaitingWriteAsyncOperation(runContinuationsAsynchronously: true, default(CancellationToken), pooled: true);
		}

		public override bool TryComplete(Exception error)
		{
			RendezvousChannel<T> parent = _parent;
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
				blockedReadersHead = parent._blockedReadersHead;
				blockedWritersHead = parent._blockedWritersHead;
				waitingReadersHead = parent._waitingReadersHead;
				waitingWritersHead = parent._waitingWritersHead;
				parent._blockedReadersHead = null;
				parent._blockedWritersHead = null;
				parent._waitingReadersHead = null;
				parent._waitingWritersHead = null;
			}
			ChannelUtilities.Complete(parent._completion, error);
			ChannelUtilities.FailOperations(blockedReadersHead, ChannelUtilities.CreateInvalidCompletionException(error));
			ChannelUtilities.FailOperations(blockedWritersHead, ChannelUtilities.CreateInvalidCompletionException(error));
			ChannelUtilities.SetOrFailOperations(waitingReadersHead, result: false, error);
			ChannelUtilities.SetOrFailOperations(waitingWritersHead, result: false, error);
			return true;
		}

		public override bool TryWrite(T item)
		{
			RendezvousChannel<T> parent = _parent;
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation = null;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting == null)
				{
					blockedReadAsyncOperation = ChannelUtilities.TryDequeueAndReserveCompletionIfCancelable(ref parent._blockedReadersHead);
				}
			}
			if (blockedReadAsyncOperation != null)
			{
				blockedReadAsyncOperation.DangerousSetResult(item);
				return true;
			}
			if (parent._dropWrites)
			{
				parent._itemDropped?.Invoke(item);
				return true;
			}
			return false;
		}

		public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<bool>(Task.FromCanceled<bool>(cancellationToken));
			}
			RendezvousChannel<T> parent = _parent;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return (parent._doneWriting != ChannelUtilities.s_doneWritingSentinel) ? new ValueTask<bool>(Task.FromException<bool>(parent._doneWriting)) : default(ValueTask<bool>);
				}
				if (parent._blockedReadersHead != null || parent._dropWrites)
				{
					return new ValueTask<bool>(result: true);
				}
				WaitingWriteAsyncOperation waitingWriteAsyncOperation = ((!cancellationToken.CanBeCanceled && _waiterSingleton.TryOwnAndReset()) ? _waiterSingleton : new WaitingWriteAsyncOperation(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate));
				ChannelUtilities.Enqueue(ref parent._waitingWritersHead, waitingWriteAsyncOperation);
				return waitingWriteAsyncOperation.ValueTaskOfT;
			}
		}

		public override ValueTask WriteAsync(T item, CancellationToken cancellationToken)
		{
			RendezvousChannel<T> parent = _parent;
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask(Task.FromCanceled<T>(cancellationToken));
			}
			BlockedWriteAsyncOperation<T> blockedWriteAsyncOperation = null;
			WaitingReadAsyncOperation head = null;
			BlockedReadAsyncOperation<T> blockedReadAsyncOperation = null;
			lock (parent.SyncObj)
			{
				if (parent._doneWriting != null)
				{
					return new ValueTask(Task.FromException(ChannelUtilities.CreateInvalidCompletionException(parent._doneWriting)));
				}
				blockedReadAsyncOperation = ChannelUtilities.TryDequeueAndReserveCompletionIfCancelable(ref parent._blockedReadersHead);
				if (blockedReadAsyncOperation == null && !parent._dropWrites)
				{
					blockedWriteAsyncOperation = ((!cancellationToken.CanBeCanceled && _writerSingleton.TryOwnAndReset()) ? _writerSingleton : new BlockedWriteAsyncOperation<T>(parent._runContinuationsAsynchronously, cancellationToken, pooled: false, _parent.CancellationCallbackDelegate));
					blockedWriteAsyncOperation.Item = item;
					ChannelUtilities.Enqueue(ref parent._blockedWritersHead, blockedWriteAsyncOperation);
					head = ChannelUtilities.TryReserveCompletionIfCancelable(ref parent._waitingReadersHead);
				}
			}
			if (blockedWriteAsyncOperation != null)
			{
				ChannelUtilities.DangerousSetOperations(head, result: true);
				return blockedWriteAsyncOperation.ValueTask;
			}
			if (blockedReadAsyncOperation != null)
			{
				blockedReadAsyncOperation.DangerousSetResult(item);
			}
			else
			{
				parent._itemDropped?.Invoke(item);
			}
			return default(ValueTask);
		}
	}

	private readonly bool _dropWrites;

	private readonly Action<T> _itemDropped;

	private readonly TaskCompletionSource _completion;

	private BlockedReadAsyncOperation<T> _blockedReadersHead;

	private BlockedWriteAsyncOperation<T> _blockedWritersHead;

	private WaitingReadAsyncOperation _waitingReadersHead;

	private WaitingWriteAsyncOperation _waitingWritersHead;

	private readonly bool _runContinuationsAsynchronously;

	private Exception _doneWriting;

	private object SyncObj => _completion;

	private Action<object, CancellationToken> CancellationCallbackDelegate => delegate(object state, CancellationToken cancellationToken)
	{
		AsyncOperation asyncOperation = (AsyncOperation)state;
		if (asyncOperation.TrySetCanceled(cancellationToken))
		{
			ChannelUtilities.UnsafeQueueUserWorkItem(delegate(KeyValuePair<RendezvousChannel<T>, AsyncOperation> keyValuePair)
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
			}, new KeyValuePair<RendezvousChannel<T>, AsyncOperation>(this, asyncOperation));
		}
	};

	private string DebuggerDisplay => ((RendezvousChannelReader)base.Reader).DebuggerDisplay + ", " + ((RendezvousChannelWriter)base.Writer).DebuggerDisplay;

	internal RendezvousChannel(BoundedChannelFullMode mode, bool runContinuationsAsynchronously, Action<T> itemDropped)
	{
		_dropWrites = mode != BoundedChannelFullMode.Wait;
		_runContinuationsAsynchronously = runContinuationsAsynchronously;
		_itemDropped = itemDropped;
		_completion = new TaskCompletionSource(runContinuationsAsynchronously ? TaskCreationOptions.RunContinuationsAsynchronously : TaskCreationOptions.None);
		base.Reader = new RendezvousChannelReader(this);
		base.Writer = new RendezvousChannelWriter(this);
	}
}
