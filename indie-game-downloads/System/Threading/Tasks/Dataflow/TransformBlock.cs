using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow.Internal;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a dataflow block that invokes a provided <see cref="T:System.Func`2" /> delegate for every data element received.</summary>
/// <typeparam name="TInput">Specifies the type of data received and operated on by this <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" />.</typeparam>
/// <typeparam name="TOutput">Specifies the type of data output by this <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" />.</typeparam>
[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
[DebuggerTypeProxy(typeof(TransformBlock<, >.DebugView))]
public sealed class TransformBlock<TInput, TOutput> : IPropagatorBlock<TInput, TOutput>, ITargetBlock<TInput>, IDataflowBlock, ISourceBlock<TOutput>, IReceivableSourceBlock<TOutput>, IDebuggerDisplay
{
	private sealed class DebugView
	{
		private readonly TransformBlock<TInput, TOutput> _transformBlock;

		private readonly TargetCore<TInput>.DebuggingInformation _targetDebuggingInformation;

		private readonly SourceCore<TOutput>.DebuggingInformation _sourceDebuggingInformation;

		public IEnumerable<TInput> InputQueue => _targetDebuggingInformation.InputQueue;

		public QueuedMap<ISourceBlock<TInput>, DataflowMessageHeader> PostponedMessages => _targetDebuggingInformation.PostponedMessages;

		public IEnumerable<TOutput> OutputQueue => _sourceDebuggingInformation.OutputQueue;

		public int CurrentDegreeOfParallelism => _targetDebuggingInformation.CurrentDegreeOfParallelism;

		public Task TaskForOutputProcessing => _sourceDebuggingInformation.TaskForOutputProcessing;

		public ExecutionDataflowBlockOptions DataflowBlockOptions => _targetDebuggingInformation.DataflowBlockOptions;

		public bool IsDecliningPermanently => _targetDebuggingInformation.IsDecliningPermanently;

		public bool IsCompleted => _sourceDebuggingInformation.IsCompleted;

		public int Id => Common.GetBlockId(_transformBlock);

		public TargetRegistry<TOutput> LinkedTargets => _sourceDebuggingInformation.LinkedTargets;

		public ITargetBlock<TOutput> NextMessageReservedFor => _sourceDebuggingInformation.NextMessageReservedFor;

		public DebugView(TransformBlock<TInput, TOutput> transformBlock)
		{
			_transformBlock = transformBlock;
			_targetDebuggingInformation = transformBlock._target.GetDebuggingInformation();
			_sourceDebuggingInformation = transformBlock._source.GetDebuggingInformation();
		}
	}

	private readonly TargetCore<TInput> _target;

	private readonly ReorderingBuffer<TOutput> _reorderingBuffer;

	private readonly SourceCore<TOutput> _source;

	private object ParallelSourceLock => _source;

	/// <summary>Gets a <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation and completion of the dataflow block.</summary>
	/// <returns>The task.</returns>
	public Task Completion => _source.Completion;

	/// <summary>Gets the number of input items waiting to be processed by this block.</summary>
	/// <returns>The number of input items.</returns>
	public int InputCount => _target.InputCount;

	/// <summary>Gets the number of output items available to be received from this block.</summary>
	/// <returns>The number of output items.</returns>
	public int OutputCount => _source.OutputCount;

	private int InputCountForDebugger => _target.GetDebuggingInformation().InputCount;

	private int OutputCountForDebugger => _source.GetDebuggingInformation().OutputCount;

	private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this, _source.DataflowBlockOptions)}, InputCount = {InputCountForDebugger}, OutputCount = {OutputCountForDebugger}";

	object IDebuggerDisplay.Content => DebuggerDisplayContent;

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" />.</summary>
	/// <param name="transform">The function to invoke with each data element received.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.</exception>
	public TransformBlock(Func<TInput, TOutput> transform)
		: this(transform, (Func<TInput, Task<TOutput>>)null, ExecutionDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" /> and <see cref="T:System.Threading.Tasks.Dataflow.ExecutionDataflowBlockOptions" />.</summary>
	/// <param name="transform">The function to invoke with each data element received.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.-or-The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public TransformBlock(Func<TInput, TOutput> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)
		: this(transform, (Func<TInput, Task<TOutput>>)null, dataflowBlockOptions)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" />.</summary>
	/// <param name="transform">The function to invoke with each data element received.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.</exception>
	public TransformBlock(Func<TInput, Task<TOutput>> transform)
		: this((Func<TInput, TOutput>)null, transform, ExecutionDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" /> with the specified <see cref="T:System.Func`2" /> and <see cref="T:System.Threading.Tasks.Dataflow.ExecutionDataflowBlockOptions" />.</summary>
	/// <param name="transform">The function to invoke with each data element received.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.-or-The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public TransformBlock(Func<TInput, Task<TOutput>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)
		: this((Func<TInput, TOutput>)null, transform, dataflowBlockOptions)
	{
	}

	private TransformBlock(Func<TInput, TOutput> transformSync, Func<TInput, Task<TOutput>> transformAsync, ExecutionDataflowBlockOptions dataflowBlockOptions)
	{
		TransformBlock<TInput, TOutput> transformBlock = this;
		ArgumentNullException.ThrowIfNull(dataflowBlockOptions, "dataflowBlockOptions");
		if (transformSync == null && transformAsync == null)
		{
			throw new ArgumentNullException("transform");
		}
		if (dataflowBlockOptions == null)
		{
			throw new ArgumentNullException("dataflowBlockOptions");
		}
		dataflowBlockOptions = dataflowBlockOptions.DefaultOrClone();
		Action<ISourceBlock<TOutput>, int> itemsRemovedAction = null;
		if (dataflowBlockOptions.BoundedCapacity > 0)
		{
			itemsRemovedAction = delegate(ISourceBlock<TOutput> owningSource, int count)
			{
				((TransformBlock<TInput, TOutput>)owningSource)._target.ChangeBoundingCount(-count);
			};
		}
		_source = new SourceCore<TOutput>(this, dataflowBlockOptions, delegate(ISourceBlock<TOutput> owningSource)
		{
			((TransformBlock<TInput, TOutput>)owningSource)._target.Complete(null, dropPendingMessages: true);
		}, itemsRemovedAction);
		if (dataflowBlockOptions.SupportsParallelExecution && dataflowBlockOptions.EnsureOrdered)
		{
			_reorderingBuffer = new ReorderingBuffer<TOutput>(this, delegate(object owningSource, TOutput message)
			{
				((TransformBlock<TInput, TOutput>)owningSource)._source.AddMessage(message);
			});
		}
		if (transformSync != null)
		{
			_target = new TargetCore<TInput>(this, delegate(KeyValuePair<TInput, long> messageWithId)
			{
				transformBlock.ProcessMessage(transformSync, messageWithId);
			}, _reorderingBuffer, dataflowBlockOptions, TargetCoreOptions.None);
		}
		else
		{
			_target = new TargetCore<TInput>(this, delegate(KeyValuePair<TInput, long> messageWithId)
			{
				transformBlock.ProcessMessageWithTask(transformAsync, messageWithId);
			}, _reorderingBuffer, dataflowBlockOptions, TargetCoreOptions.UsesAsyncCompletion);
		}
		_target.Completion.ContinueWith(delegate(Task completed, object state)
		{
			SourceCore<TOutput> sourceCore = (SourceCore<TOutput>)state;
			if (completed.IsFaulted)
			{
				sourceCore.AddAndUnwrapAggregateException(completed.Exception);
			}
			sourceCore.Complete();
		}, _source, CancellationToken.None, Common.GetContinuationOptions(), TaskScheduler.Default);
		_source.Completion.ContinueWith(delegate(Task completed, object state)
		{
			((IDataflowBlock)(TransformBlock<TInput, TOutput>)state).Fault((Exception)completed.Exception);
		}, this, CancellationToken.None, Common.GetContinuationOptions() | TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
		Common.WireCancellationToComplete(dataflowBlockOptions.CancellationToken, Completion, delegate(object state, CancellationToken _)
		{
			((TargetCore<TInput>)state).Complete(null, dropPendingMessages: true);
		}, _target);
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCreated(this, dataflowBlockOptions);
		}
	}

	private void ProcessMessage(Func<TInput, TOutput> transform, KeyValuePair<TInput, long> messageWithId)
	{
		TOutput item = default(TOutput);
		bool flag = false;
		try
		{
			item = transform(messageWithId.Key);
			flag = true;
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
			if (!flag)
			{
				_target.ChangeBoundingCount(-1);
			}
			if (_reorderingBuffer == null)
			{
				if (flag)
				{
					if (_target.DataflowBlockOptions.MaxDegreeOfParallelism == 1)
					{
						_source.AddMessage(item);
					}
					else
					{
						lock (ParallelSourceLock)
						{
							_source.AddMessage(item);
						}
					}
				}
			}
			else
			{
				_reorderingBuffer.AddItem(messageWithId.Value, item, flag);
			}
		}
	}

	private void ProcessMessageWithTask(Func<TInput, Task<TOutput>> transform, KeyValuePair<TInput, long> messageWithId)
	{
		Task<TOutput> task = null;
		Exception ex = null;
		try
		{
			task = transform(messageWithId.Key);
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
				_target.Complete(ex, dropPendingMessages: true, storeExceptionEvenIfAlreadyCompleting: true);
			}
			_reorderingBuffer?.IgnoreItem(messageWithId.Value);
			_target.SignalOneAsyncMessageCompleted(-1);
		}
		else
		{
			task.ContinueWith(delegate(Task<TOutput> completed, object state)
			{
				Tuple<TransformBlock<TInput, TOutput>, KeyValuePair<TInput, long>> tuple = (Tuple<TransformBlock<TInput, TOutput>, KeyValuePair<TInput, long>>)state;
				tuple.Item1.AsyncCompleteProcessMessageWithTask(completed, tuple.Item2);
			}, Tuple.Create(this, messageWithId), CancellationToken.None, Common.GetContinuationOptions(TaskContinuationOptions.ExecuteSynchronously), TaskScheduler.Default);
		}
	}

	private void AsyncCompleteProcessMessageWithTask(Task<TOutput> completed, KeyValuePair<TInput, long> messageWithId)
	{
		bool isBounded = _target.IsBounded;
		bool flag = false;
		TOutput item = default(TOutput);
		switch (completed.Status)
		{
		case TaskStatus.RanToCompletion:
			item = completed.Result;
			flag = true;
			break;
		case TaskStatus.Faulted:
		{
			AggregateException exception = completed.Exception;
			Common.StoreDataflowMessageValueIntoExceptionData(exception, messageWithId.Key, targetInnerExceptions: true);
			_target.Complete(exception, dropPendingMessages: true, storeExceptionEvenIfAlreadyCompleting: true, unwrapInnerExceptions: true);
			break;
		}
		}
		if (!flag & isBounded)
		{
			_target.ChangeBoundingCount(-1);
		}
		if (_reorderingBuffer == null)
		{
			if (flag)
			{
				if (_target.DataflowBlockOptions.MaxDegreeOfParallelism == 1)
				{
					_source.AddMessage(item);
				}
				else
				{
					lock (ParallelSourceLock)
					{
						_source.AddMessage(item);
					}
				}
			}
		}
		else
		{
			_reorderingBuffer.AddItem(messageWithId.Value, item, flag);
		}
		_target.SignalOneAsyncMessageCompleted();
	}

	/// <summary>Signals to the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> that it should not accept nor produce any more messages nor consume any more postponed messages.</summary>
	public void Complete()
	{
		_target.Complete(null, dropPendingMessages: false);
	}

	/// <summary>Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.</summary>
	/// <param name="exception">The <see cref="T:System.Exception" /> that caused the faulting.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> is null.</exception>
	void IDataflowBlock.Fault(Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception, "exception");
		_target.Complete(exception, dropPendingMessages: true);
	}

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect this source.</param>
	/// <param name="linkOptions">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" /> instance that configures the link.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="target" /> is null (Nothing in Visual Basic) or <paramref name="linkOptions" /> is null (Nothing in Visual Basic).</exception>
	public IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
	{
		return _source.LinkTo(target, linkOptions);
	}

	/// <summary>Attempts to synchronously receive an available output item from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if an item could be received; otherwise, false.</returns>
	/// <param name="filter">The predicate value must successfully pass in order for it to be received. <paramref name="filter" /> may be null, in which case all items will pass.</param>
	/// <param name="item">The item received from the source.</param>
	public bool TryReceive(Predicate<TOutput>? filter, [MaybeNullWhen(false)] out TOutput item)
	{
		return _source.TryReceive(filter, out item);
	}

	/// <summary>Attempts to synchronously receive all available items from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if one or more items could be received; otherwise, false.</returns>
	/// <param name="items">The items received from the source.</param>
	public bool TryReceiveAll([NotNullWhen(true)] out IList<TOutput>? items)
	{
		return _source.TryReceiveAll(out items);
	}

	/// <summary>Offers a message to the <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />, giving the target the opportunity to consume or postpone the message.</summary>
	/// <returns>The status of the offered message. If the message was accepted by the target, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Accepted" /> is returned, and the source should no longer use the offered message, because it is now owned by the target. If the message was postponed by the target, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Postponed" /> is returned as a notification that the target may later attempt to consume or reserve the message; in the meantime, the source still owns the message and may offer it to other blocks. If the target would have otherwise postponed but source was null, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Declined" /> is instead returned.  If the target tried to accept the message but missed it due to the source delivering the message to another target or simply discarding it, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.NotAvailable" /> is returned. If the target chose not to accept the message, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Declined" /> is returned. If the target chose not to accept the message and will never accept another message from this source, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.DecliningPermanently" /> is returned.</returns>
	/// <param name="messageHeader">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance that represents the header of the message being offered.</param>
	/// <param name="messageValue">The value of the message being offered.</param>
	/// <param name="source">The <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> offering the message. This may be null.</param>
	/// <param name="consumeToAccept">true if the target must call <see cref="M:System.Threading.Tasks.Dataflow.ISourceBlock`1.ConsumeMessage()" /> synchronously during the call to <see cref="M:System.Threading.Tasks.Dataflow.TransformBlock`2.System#Threading#Tasks#Dataflow#ITargetBlock{TInput}#OfferMessage()" />, prior to returning <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Accepted" />, in order to consume the message.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="messageHeader" /> is not valid.-or-<paramref name="consumeToAccept" /> may only be true if provided with a non-null <paramref name="source" />.</exception>
	DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
	{
		return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
	}

	/// <summary>Called by a linked <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to accept and consume a <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> previously offered by this <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" />.</summary>
	/// <returns>The value of the consumed message. This may correspond to a different <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance than was previously reserved and passed as the <paramref name="messageHeader" /> to <see cref="M:System.Threading.Tasks.Dataflow.ISourceBlock`1.ConsumeMessage()" />. The consuming <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> must use the returned value instead of the value passed as <paramref name="messageValue" /> through <see cref="M:System.Threading.Tasks.Dataflow.TransformBlock`2.System#Threading#Tasks#Dataflow#ITargetBlock{TInput}#OfferMessage()" />.If the message requested is not available, the return value will be null.</returns>
	/// <param name="messageHeader">The <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> of the message being consumed.</param>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> consuming the message.</param>
	/// <param name="messageConsumed">true if the message was successfully consumed; otherwise, false.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="messageHeader" /> is not valid.</exception>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="target" /> is null.</exception>
	TOutput ISourceBlock<TOutput>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)
	{
		return _source.ConsumeMessage(messageHeader, target, out messageConsumed);
	}

	/// <summary>Called by a linked <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to reserve a previously offered <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> by this <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" />.</summary>
	/// <returns>true if the message was successfully reserved; otherwise, false.</returns>
	/// <param name="messageHeader">The <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> of the message being reserved.</param>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> reserving the message.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="messageHeader" /> is not valid.</exception>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="target" /> is null.</exception>
	bool ISourceBlock<TOutput>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
	{
		return _source.ReserveMessage(messageHeader, target);
	}

	/// <summary>Called by a linked <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to release a previously reserved <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> by this <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" />.</summary>
	/// <param name="messageHeader">The <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> of the reserved message being released.</param>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> releasing the message it previously reserved.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="messageHeader" /> is not valid.</exception>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="target" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">The <paramref name="target" /> did not have the message reserved.</exception>
	void ISourceBlock<TOutput>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
	{
		_source.ReleaseReservation(messageHeader, target);
	}

	/// <summary>Returns a string that represents the formatted name of this <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> instance.</summary>
	/// <returns>A string that represents the formatted name of this <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> instance.</returns>
	public override string ToString()
	{
		return Common.GetNameForDebugger(this, _source.DataflowBlockOptions);
	}
}
