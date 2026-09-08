using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow.Internal;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a dataflow block that invokes a provided <see cref="T:System.Action`1" /> delegate for every data element received.</summary>
/// <typeparam name="TInput">The type of data that this <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> operates on.</typeparam>
[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
[DebuggerTypeProxy(typeof(ActionBlock<>.DebugView))]
public sealed class ActionBlock<TInput> : ITargetBlock<TInput>, IDataflowBlock, IDebuggerDisplay
{
	private sealed class DebugView
	{
		private readonly ActionBlock<TInput> _actionBlock;

		private readonly TargetCore<TInput>.DebuggingInformation _defaultDebugInfo;

		private readonly SpscTargetCore<TInput>.DebuggingInformation _spscDebugInfo;

		public IEnumerable<TInput> InputQueue
		{
			get
			{
				if (_defaultDebugInfo == null)
				{
					return _spscDebugInfo.InputQueue;
				}
				return _defaultDebugInfo.InputQueue;
			}
		}

		public QueuedMap<ISourceBlock<TInput>, DataflowMessageHeader> PostponedMessages => _defaultDebugInfo?.PostponedMessages;

		public int CurrentDegreeOfParallelism
		{
			get
			{
				if (_defaultDebugInfo == null)
				{
					return _spscDebugInfo.CurrentDegreeOfParallelism;
				}
				return _defaultDebugInfo.CurrentDegreeOfParallelism;
			}
		}

		public ExecutionDataflowBlockOptions DataflowBlockOptions
		{
			get
			{
				if (_defaultDebugInfo == null)
				{
					return _spscDebugInfo.DataflowBlockOptions;
				}
				return _defaultDebugInfo.DataflowBlockOptions;
			}
		}

		public bool IsDecliningPermanently
		{
			get
			{
				if (_defaultDebugInfo == null)
				{
					return _spscDebugInfo.IsDecliningPermanently;
				}
				return _defaultDebugInfo.IsDecliningPermanently;
			}
		}

		public bool IsCompleted
		{
			get
			{
				if (_defaultDebugInfo == null)
				{
					return _spscDebugInfo.IsCompleted;
				}
				return _defaultDebugInfo.IsCompleted;
			}
		}

		public int Id => Common.GetBlockId(_actionBlock);

		public DebugView(ActionBlock<TInput> actionBlock)
		{
			_actionBlock = actionBlock;
			if (_actionBlock._defaultTarget != null)
			{
				_defaultDebugInfo = actionBlock._defaultTarget.GetDebuggingInformation();
			}
			else
			{
				_spscDebugInfo = actionBlock._spscTarget.GetDebuggingInformation();
			}
		}
	}

	private readonly TargetCore<TInput> _defaultTarget;

	private readonly SpscTargetCore<TInput> _spscTarget;

	/// <summary>Gets a <see cref="T:System.Threading.Tasks.Task" /> object that represents the asynchronous operation and completion of the dataflow block.</summary>
	/// <returns>The completed task.</returns>
	public Task Completion
	{
		get
		{
			if (_defaultTarget == null)
			{
				return _spscTarget.Completion;
			}
			return _defaultTarget.Completion;
		}
	}

	/// <summary>Gets the number of input items waiting to be processed by this block.</summary>
	/// <returns>The number of input items waiting to be processed by this block.</returns>
	public int InputCount
	{
		get
		{
			if (_defaultTarget == null)
			{
				return _spscTarget.InputCount;
			}
			return _defaultTarget.InputCount;
		}
	}

	private int InputCountForDebugger
	{
		get
		{
			if (_defaultTarget == null)
			{
				return _spscTarget.InputCount;
			}
			return _defaultTarget.GetDebuggingInformation().InputCount;
		}
	}

	private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this, (_defaultTarget != null) ? _defaultTarget.DataflowBlockOptions : _spscTarget.DataflowBlockOptions)}, InputCount = {InputCountForDebugger}";

	object IDebuggerDisplay.Content => DebuggerDisplayContent;

	/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action.</summary>
	/// <param name="action">The action to invoke with each data element received.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="action" /> is null.</exception>
	public ActionBlock(Action<TInput> action)
		: this((Delegate)action, ExecutionDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action and configuration options.</summary>
	/// <param name="action">The action to invoke with each data element received.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" />.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="action" /> is null.-or-<paramref name="dataflowBlockOptions" /> is null.</exception>
	public ActionBlock(Action<TInput> action, ExecutionDataflowBlockOptions dataflowBlockOptions)
		: this((Delegate)action, dataflowBlockOptions)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action.</summary>
	/// <param name="action">The action to invoke with each data element received.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="action" /> is null.</exception>
	public ActionBlock(Func<TInput, Task> action)
		: this((Delegate)action, ExecutionDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> class with the specified action and configuration options.</summary>
	/// <param name="action">The action to invoke with each data element received.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" />.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="action" /> is null.-or-<paramref name="dataflowBlockOptions" /> is null.</exception>
	public ActionBlock(Func<TInput, Task> action, ExecutionDataflowBlockOptions dataflowBlockOptions)
		: this((Delegate)action, dataflowBlockOptions)
	{
	}

	private ActionBlock(Delegate action, ExecutionDataflowBlockOptions dataflowBlockOptions)
	{
		ActionBlock<TInput> actionBlock = this;
		ArgumentNullException.ThrowIfNull(action, "action");
		ArgumentNullException.ThrowIfNull(dataflowBlockOptions, "dataflowBlockOptions");
		dataflowBlockOptions = dataflowBlockOptions.DefaultOrClone();
		Action<TInput> syncAction = action as Action<TInput>;
		if (syncAction != null && dataflowBlockOptions.SingleProducerConstrained && dataflowBlockOptions.MaxDegreeOfParallelism == 1 && !dataflowBlockOptions.CancellationToken.CanBeCanceled && dataflowBlockOptions.BoundedCapacity == -1)
		{
			_spscTarget = new SpscTargetCore<TInput>(this, syncAction, dataflowBlockOptions);
		}
		else
		{
			if (syncAction != null)
			{
				_defaultTarget = new TargetCore<TInput>(this, delegate(KeyValuePair<TInput, long> messageWithId)
				{
					actionBlock.ProcessMessage(syncAction, messageWithId);
				}, null, dataflowBlockOptions, TargetCoreOptions.RepresentsBlockCompletion);
			}
			else
			{
				Func<TInput, Task> asyncAction = action as Func<TInput, Task>;
				_defaultTarget = new TargetCore<TInput>(this, delegate(KeyValuePair<TInput, long> messageWithId)
				{
					actionBlock.ProcessMessageWithTask(asyncAction, messageWithId);
				}, null, dataflowBlockOptions, TargetCoreOptions.UsesAsyncCompletion | TargetCoreOptions.RepresentsBlockCompletion);
			}
			Common.WireCancellationToComplete(dataflowBlockOptions.CancellationToken, Completion, delegate(object state, CancellationToken _)
			{
				((TargetCore<TInput>)state).Complete(null, dropPendingMessages: true);
			}, _defaultTarget);
		}
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCreated(this, dataflowBlockOptions);
		}
	}

	private void ProcessMessage(Action<TInput> action, KeyValuePair<TInput, long> messageWithId)
	{
		try
		{
			action(messageWithId.Key);
		}
		catch (Exception exception)
		{
			if (!Common.IsCooperativeCancellation(exception))
			{
				throw;
			}
		}
		finally
		{
			if (_defaultTarget.IsBounded)
			{
				_defaultTarget.ChangeBoundingCount(-1);
			}
		}
	}

	private void ProcessMessageWithTask(Func<TInput, Task> action, KeyValuePair<TInput, long> messageWithId)
	{
		Task task = null;
		Exception ex = null;
		try
		{
			task = action(messageWithId.Key);
		}
		catch (Exception ex2)
		{
			ex = ex2;
		}
		if (task == null)
		{
			if (ex != null && !Common.IsCooperativeCancellation(ex))
			{
				Common.StoreDataflowMessageValueIntoExceptionData(ex, messageWithId.Key);
				_defaultTarget.Complete(ex, dropPendingMessages: true, storeExceptionEvenIfAlreadyCompleting: true);
			}
			_defaultTarget.SignalOneAsyncMessageCompleted(-1);
		}
		else if (task.IsCompleted)
		{
			AsyncCompleteProcessMessageWithTask(task);
		}
		else
		{
			task.ContinueWith(delegate(Task completed, object state)
			{
				((ActionBlock<TInput>)state).AsyncCompleteProcessMessageWithTask(completed);
			}, this, CancellationToken.None, Common.GetContinuationOptions(TaskContinuationOptions.ExecuteSynchronously), TaskScheduler.Default);
		}
	}

	private void AsyncCompleteProcessMessageWithTask(Task completed)
	{
		if (completed.IsFaulted)
		{
			_defaultTarget.Complete(completed.Exception, dropPendingMessages: true, storeExceptionEvenIfAlreadyCompleting: true, unwrapInnerExceptions: true);
		}
		_defaultTarget.SignalOneAsyncMessageCompleted(-1);
	}

	/// <summary>Signals to the dataflow block that it shouldn't accept or produce any more messages and shouldn't consume any more postponed messages.</summary>
	public void Complete()
	{
		if (_defaultTarget != null)
		{
			_defaultTarget.Complete(null, dropPendingMessages: false);
		}
		else
		{
			_spscTarget.Complete(null);
		}
	}

	/// <summary>Causes the dataflow block to complete in a faulted state.</summary>
	/// <param name="exception">The exception that caused the faulting.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="exception" /> is null.</exception>
	void IDataflowBlock.Fault(Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception, "exception");
		if (_defaultTarget != null)
		{
			_defaultTarget.Complete(exception, dropPendingMessages: true);
		}
		else
		{
			_spscTarget.Complete(exception);
		}
	}

	/// <summary>Posts an item to the target dataflow block.</summary>
	/// <returns>The number of input items.</returns>
	/// <param name="item">The item being offered to the target.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Post(TInput item)
	{
		if (_defaultTarget == null)
		{
			return _spscTarget.Post(item);
		}
		return _defaultTarget.OfferMessage(Common.SingleMessageHeader, item, null, consumeToAccept: false) == DataflowMessageStatus.Accepted;
	}

	/// <summary>Offers a message to the dataflow block, and gives it the opportunity to consume or postpone the message.</summary>
	/// <returns>The status of the offered message. If the message was accepted by the target, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Accepted" /> is returned, and the source should no longer use the offered message, because it is now owned by the target. If the message was postponed by the target, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Postponed" /> is returned as a notification that the target may later attempt to consume or reserve the message; in the meantime, the source still owns the message and may offer it to other blocks.If the target would have otherwise postponed message, but <paramref name="source" /> was null, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Declined" /> is returned. If the target tried to accept the message but missed it due to the source delivering the message to another target or simply discarding it, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.NotAvailable" /> is returned. If the target chose not to accept the message, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Declined" /> is returned. If the target chose not to accept the message and will never accept another message from this source, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.DecliningPermanently" /> is returned.</returns>
	/// <param name="messageHeader">The header of the message being offered.</param>
	/// <param name="messageValue">The value of the message being offered.</param>
	/// <param name="source">The dataflow block that is offering the message. This may be null.</param>
	/// <param name="consumeToAccept">true to instruct the target to call <see cref="M:System.Threading.Tasks.Dataflow.ISourceBlock`1.ConsumeMessage()" /> synchronously during the call to <see cref="M:System.Threading.Tasks.Dataflow.ITargetBlock`1.OfferMessage()" />, prior to returning <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Accepted" />, in order to consume the message.</param>
	/// <exception cref="T:System.ArgumentException">
	///   <paramref name="messageHeader" /> is not valid.-or-<paramref name="consumeToAccept" /> may be true only if provided with a non-null <paramref name="source" />.</exception>
	DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
	{
		if (_defaultTarget == null)
		{
			return _spscTarget.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
		}
		return _defaultTarget.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
	}

	/// <summary>Returns a string that represents the formatted name of this <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> instance.</summary>
	/// <returns>A string that represents the formatted name of this <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> nstance.</returns>
	public override string ToString()
	{
		return Common.GetNameForDebugger(this, (_defaultTarget != null) ? _defaultTarget.DataflowBlockOptions : _spscTarget.DataflowBlockOptions);
	}
}
