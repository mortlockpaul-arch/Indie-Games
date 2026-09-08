using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks.Dataflow.Internal;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a dataflow block that invokes a provided <see cref="T:System.Func`2" /> delegate for every data element received.</summary>
/// <typeparam name="TInput">Specifies the type of data received and operated on by this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</typeparam>
/// <typeparam name="TOutput">Specifies the type of data output by this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</typeparam>
[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
[DebuggerTypeProxy(typeof(TransformManyBlock<, >.DebugView))]
public sealed class TransformManyBlock<TInput, TOutput> : IPropagatorBlock<TInput, TOutput>, ITargetBlock<TInput>, IDataflowBlock, ISourceBlock<TOutput>, IReceivableSourceBlock<TOutput>, IDebuggerDisplay
{
	private sealed class DebugView
	{
		private readonly TransformManyBlock<TInput, TOutput> _transformManyBlock;

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

		public int Id => Common.GetBlockId(_transformManyBlock);

		public TargetRegistry<TOutput> LinkedTargets => _sourceDebuggingInformation.LinkedTargets;

		public ITargetBlock<TOutput> NextMessageReservedFor => _sourceDebuggingInformation.NextMessageReservedFor;

		public DebugView(TransformManyBlock<TInput, TOutput> transformManyBlock)
		{
			_transformManyBlock = transformManyBlock;
			_targetDebuggingInformation = transformManyBlock._target.GetDebuggingInformation();
			_sourceDebuggingInformation = transformManyBlock._source.GetDebuggingInformation();
		}
	}

	private readonly TargetCore<TInput> _target;

	private readonly ReorderingBuffer<IEnumerable<TOutput>> _reorderingBuffer;

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

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function.</summary>
	/// <param name="transform">The function to invoke with each data element received. All of the data from the returned <see cref="T:System.Collections.Generic.IEnumerable`1" /> will be made available as output from this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.</exception>
	public TransformManyBlock(Func<TInput, IEnumerable<TOutput>> transform)
		: this(transform, ExecutionDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function and <see cref="T:System.Threading.Tasks.Dataflow.ExecutionDataflowBlockOptions" />.</summary>
	/// <param name="transform">The function to invoke with each data element received. All of the data from the returned in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> will be made available as output from this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.-or-The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public TransformManyBlock(Func<TInput, IEnumerable<TOutput>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)
	{
		TransformManyBlock<TInput, TOutput> transformManyBlock = this;
		if (transform == null)
		{
			throw new ArgumentNullException("transform");
		}
		Initialize(delegate(KeyValuePair<TInput, long> messageWithId)
		{
			transformManyBlock.ProcessMessage(transform, messageWithId);
		}, dataflowBlockOptions, ref _source, ref _target, ref _reorderingBuffer, TargetCoreOptions.None);
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function.</summary>
	/// <param name="transform">The function to invoke with each data element received. All of the data asynchronously returned in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> will be made available as output from this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.</exception>
	public TransformManyBlock(Func<TInput, Task<IEnumerable<TOutput>>> transform)
		: this(transform, ExecutionDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" /> with the specified function and <see cref="T:System.Threading.Tasks.Dataflow.ExecutionDataflowBlockOptions" />.</summary>
	/// <param name="transform">The function to invoke with each data element received. All of the data asynchronously returned in the <see cref="T:System.Collections.Generic.IEnumerable`1" /> will be made available as output from this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.TransformManyBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="transform" /> is null.-or-The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public TransformManyBlock(Func<TInput, Task<IEnumerable<TOutput>>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)
	{
		TransformManyBlock<TInput, TOutput> transformManyBlock = this;
		if (transform == null)
		{
			throw new ArgumentNullException("transform");
		}
		Initialize(delegate(KeyValuePair<TInput, long> messageWithId)
		{
			transformManyBlock.ProcessMessageWithTask(transform, messageWithId);
		}, dataflowBlockOptions, ref _source, ref _target, ref _reorderingBuffer, TargetCoreOptions.UsesAsyncCompletion);
	}

	private void Initialize(Action<KeyValuePair<TInput, long>> processMessageAction, ExecutionDataflowBlockOptions dataflowBlockOptions, [NotNull] ref SourceCore<TOutput> source, [NotNull] ref TargetCore<TInput> target, ref ReorderingBuffer<IEnumerable<TOutput>> reorderingBuffer, TargetCoreOptions targetCoreOptions)
	{
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
				((TransformManyBlock<TInput, TOutput>)owningSource)._target.ChangeBoundingCount(-count);
			};
		}
		source = new SourceCore<TOutput>(this, dataflowBlockOptions, delegate(ISourceBlock<TOutput> owningSource)
		{
			((TransformManyBlock<TInput, TOutput>)owningSource)._target.Complete(null, dropPendingMessages: true);
		}, itemsRemovedAction);
		if (dataflowBlockOptions.SupportsParallelExecution && dataflowBlockOptions.EnsureOrdered)
		{
			reorderingBuffer = new ReorderingBuffer<IEnumerable<TOutput>>(this, delegate(object obj, IEnumerable<TOutput> messages)
			{
				((TransformManyBlock<TInput, TOutput>)obj)._source.AddMessages(messages);
			});
		}
		target = new TargetCore<TInput>(this, processMessageAction, _reorderingBuffer, dataflowBlockOptions, targetCoreOptions);
		target.Completion.ContinueWith(delegate(Task completed, object state)
		{
			SourceCore<TOutput> sourceCore = (SourceCore<TOutput>)state;
			if (completed.IsFaulted)
			{
				sourceCore.AddAndUnwrapAggregateException(completed.Exception);
			}
			sourceCore.Complete();
		}, source, CancellationToken.None, Common.GetContinuationOptions(), TaskScheduler.Default);
		source.Completion.ContinueWith(delegate(Task completed, object state)
		{
			((IDataflowBlock)(TransformManyBlock<TInput, TOutput>)state).Fault((Exception)completed.Exception);
		}, this, CancellationToken.None, Common.GetContinuationOptions() | TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
		Common.WireCancellationToComplete(dataflowBlockOptions.CancellationToken, Completion, delegate(object state, CancellationToken _)
		{
			((TargetCore<TInput>)state).Complete(null, dropPendingMessages: true);
		}, target);
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCreated(this, dataflowBlockOptions);
		}
	}

	private void ProcessMessage(Func<TInput, IEnumerable<TOutput>> transformFunction, KeyValuePair<TInput, long> messageWithId)
	{
		bool flag = false;
		try
		{
			IEnumerable<TOutput> outputItems = transformFunction(messageWithId.Key);
			flag = true;
			StoreOutputItems(messageWithId, outputItems);
		}
		catch (Exception exception) when (Common.IsCooperativeCancellation(exception))
		{
		}
		finally
		{
			if (!flag)
			{
				StoreOutputItems(messageWithId, null);
			}
		}
	}

	private void ProcessMessageWithTask(Func<TInput, Task<IEnumerable<TOutput>>> function, KeyValuePair<TInput, long> messageWithId)
	{
		Task<IEnumerable<TOutput>> task = null;
		Exception ex = null;
		try
		{
			task = function(messageWithId.Key);
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
			if (_reorderingBuffer != null)
			{
				StoreOutputItems(messageWithId, null);
				_target.SignalOneAsyncMessageCompleted();
			}
			else
			{
				_target.SignalOneAsyncMessageCompleted(-1);
			}
		}
		else
		{
			task.ContinueWith(delegate(Task<IEnumerable<TOutput>> completed, object state)
			{
				Tuple<TransformManyBlock<TInput, TOutput>, KeyValuePair<TInput, long>> tuple = (Tuple<TransformManyBlock<TInput, TOutput>, KeyValuePair<TInput, long>>)state;
				tuple.Item1.AsyncCompleteProcessMessageWithTask(completed, tuple.Item2);
			}, Tuple.Create(this, messageWithId), CancellationToken.None, Common.GetContinuationOptions(TaskContinuationOptions.ExecuteSynchronously), _source.DataflowBlockOptions.TaskScheduler);
		}
	}

	private void AsyncCompleteProcessMessageWithTask(Task<IEnumerable<TOutput>> completed, KeyValuePair<TInput, long> messageWithId)
	{
		switch (completed.Status)
		{
		case TaskStatus.RanToCompletion:
		{
			IEnumerable<TOutput> result = completed.Result;
			try
			{
				StoreOutputItems(messageWithId, result);
			}
			catch (Exception ex)
			{
				if (!Common.IsCooperativeCancellation(ex))
				{
					Common.StoreDataflowMessageValueIntoExceptionData(ex, messageWithId.Key);
					_target.Complete(ex, dropPendingMessages: true, storeExceptionEvenIfAlreadyCompleting: true);
				}
			}
			break;
		}
		case TaskStatus.Faulted:
		{
			AggregateException exception = completed.Exception;
			Common.StoreDataflowMessageValueIntoExceptionData(exception, messageWithId.Key, targetInnerExceptions: true);
			_target.Complete(exception, dropPendingMessages: true, storeExceptionEvenIfAlreadyCompleting: true, unwrapInnerExceptions: true);
			goto case TaskStatus.Canceled;
		}
		case TaskStatus.Canceled:
			StoreOutputItems(messageWithId, null);
			break;
		}
		_target.SignalOneAsyncMessageCompleted();
	}

	private void StoreOutputItems(KeyValuePair<TInput, long> messageWithId, IEnumerable<TOutput> outputItems)
	{
		if (_reorderingBuffer != null)
		{
			StoreOutputItemsReordered(messageWithId.Value, outputItems);
		}
		else if (outputItems != null)
		{
			if (outputItems is TOutput[] || outputItems is List<TOutput>)
			{
				StoreOutputItemsNonReorderedAtomic(outputItems);
			}
			else
			{
				StoreOutputItemsNonReorderedWithIteration(outputItems);
			}
		}
		else if (_target.IsBounded)
		{
			_target.ChangeBoundingCount(-1);
		}
	}

	private void StoreOutputItemsReordered(long id, IEnumerable<TOutput> item)
	{
		TargetCore<TInput> target = _target;
		bool isBounded = target.IsBounded;
		if (item == null)
		{
			_reorderingBuffer.AddItem(id, null, itemIsValid: false);
			if (isBounded)
			{
				target.ChangeBoundingCount(-1);
			}
			return;
		}
		IList<TOutput> list = item as TOutput[];
		IList<TOutput> list2 = list ?? (item as List<TOutput>);
		if ((list2 != null) & isBounded)
		{
			UpdateBoundingCountWithOutputCount(list2.Count);
		}
		bool? flag = _reorderingBuffer.AddItemIfNextAndTrusted(id, list2, list2 != null);
		if (!flag.HasValue)
		{
			return;
		}
		bool value = flag.Value;
		List<TOutput> list3 = null;
		try
		{
			if (value)
			{
				StoreOutputItemsNonReorderedWithIteration(item);
				return;
			}
			if (list2 != null)
			{
				list3 = list2.ToList();
				return;
			}
			int count = 0;
			try
			{
				list3 = item.ToList();
				count = list3.Count;
			}
			finally
			{
				if (isBounded)
				{
					UpdateBoundingCountWithOutputCount(count);
				}
			}
		}
		finally
		{
			_reorderingBuffer.AddItem(id, list3, list3 != null);
		}
	}

	private void StoreOutputItemsNonReorderedAtomic(IEnumerable<TOutput> outputItems)
	{
		if (_target.IsBounded)
		{
			UpdateBoundingCountWithOutputCount(((ICollection<TOutput>)outputItems).Count);
		}
		if (_target.DataflowBlockOptions.MaxDegreeOfParallelism == 1)
		{
			_source.AddMessages(outputItems);
			return;
		}
		lock (ParallelSourceLock)
		{
			_source.AddMessages(outputItems);
		}
	}

	private void StoreOutputItemsNonReorderedWithIteration(IEnumerable<TOutput> outputItems)
	{
		bool flag = _target.DataflowBlockOptions.MaxDegreeOfParallelism == 1 || _reorderingBuffer != null;
		if (_target.IsBounded)
		{
			bool flag2 = false;
			try
			{
				foreach (TOutput outputItem in outputItems)
				{
					if (flag2)
					{
						_target.ChangeBoundingCount(1);
					}
					else
					{
						flag2 = true;
					}
					if (flag)
					{
						_source.AddMessage(outputItem);
						continue;
					}
					lock (ParallelSourceLock)
					{
						_source.AddMessage(outputItem);
					}
				}
				return;
			}
			finally
			{
				if (!flag2)
				{
					_target.ChangeBoundingCount(-1);
				}
			}
		}
		if (flag)
		{
			foreach (TOutput outputItem2 in outputItems)
			{
				_source.AddMessage(outputItem2);
			}
			return;
		}
		foreach (TOutput outputItem3 in outputItems)
		{
			lock (ParallelSourceLock)
			{
				_source.AddMessage(outputItem3);
			}
		}
	}

	private void UpdateBoundingCountWithOutputCount(int count)
	{
		if (count > 1)
		{
			_target.ChangeBoundingCount(count - 1);
		}
		else if (count == 0)
		{
			_target.ChangeBoundingCount(-1);
		}
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
	/// <returns>The status of the offered message. If the message was accepted by the target, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Accepted" /> is returned, and the source should no longer use the offered message, as it is now owned by the target. If the message was postponed by the target, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Postponed" /> is returned as a notification that the target may later attempt to consume or reserve the message; in the meantime, the source still owns the message and may offer it to other blocks. If the target would have otherwise postponed but source was null, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Declined" /> is instead returned.  If the target tried to accept the message but missed it due to the source delivering the message to another target or simply discarding it, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.NotAvailable" /> is returned. If the target chose not to accept the message, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Declined" /> is returned. If the target chose not to accept the message and will never accept another message from this source, <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.DecliningPermanently" /> is returned.</returns>
	/// <param name="messageHeader">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance that represents the header of the message being offered.</param>
	/// <param name="messageValue">The value of the message being offered.</param>
	/// <param name="source">The <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> offering the message. This may be null.</param>
	/// <param name="consumeToAccept">true if the target must call <see cref="M:System.Threading.Tasks.Dataflow.ISourceBlock`1.ConsumeMessage()" /> synchronously during the call to <see cref="M:System.Threading.Tasks.Dataflow.TransformManyBlock`2.System#Threading#Tasks#Dataflow#ITargetBlock{TInput}#OfferMessage()" />, prior to returning <see cref="F:System.Threading.Tasks.Dataflow.DataflowMessageStatus.Accepted" />, in order to consume the message.</param>
	/// <exception cref="T:System.ArgumentException">The <paramref name="messageHeader" /> is not valid.-or-<paramref name="consumeToAccept" /> may only be true if provided with a non-null <paramref name="source" />.</exception>
	DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
	{
		return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
	}

	/// <summary>Called by a linked <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to accept and consume a <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> previously offered by this <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" />.</summary>
	/// <returns>The value of the consumed message. This may correspond to a different <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance than was previously reserved and passed as the <paramref name="messageHeader" /> to <see cref="M:System.Threading.Tasks.Dataflow.ISourceBlock`1.ConsumeMessage()" />. The consuming <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> must use the returned value instead of the value passed as <paramref name="messageValue" /> through <see cref="M:System.Threading.Tasks.Dataflow.TransformManyBlock`2.System#Threading#Tasks#Dataflow#ITargetBlock{TInput}#OfferMessage()" />.If the message requested is not available, the return value will be null.</returns>
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

	public TransformManyBlock(Func<TInput, IAsyncEnumerable<TOutput>> transform)
		: this(transform, ExecutionDataflowBlockOptions.Default)
	{
	}

	public TransformManyBlock(Func<TInput, IAsyncEnumerable<TOutput>> transform, ExecutionDataflowBlockOptions dataflowBlockOptions)
	{
		TransformManyBlock<TInput, TOutput> transformManyBlock = this;
		ArgumentNullException.ThrowIfNull(transform, "transform");
		Initialize(delegate(KeyValuePair<TInput, long> messageWithId)
		{
			transformManyBlock.ProcessMessageAsync(transform, messageWithId);
		}, dataflowBlockOptions, ref _source, ref _target, ref _reorderingBuffer, TargetCoreOptions.UsesAsyncCompletion);
	}

	private async Task ProcessMessageAsync(Func<TInput, IAsyncEnumerable<TOutput>> transformFunction, KeyValuePair<TInput, long> messageWithId)
	{
		try
		{
			IAsyncEnumerable<TOutput> outputItems = transformFunction(messageWithId.Key);
			await StoreOutputItemsAsync(messageWithId, outputItems).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex)
		{
			if (!Common.IsCooperativeCancellation(ex))
			{
				Common.StoreDataflowMessageValueIntoExceptionData(ex, messageWithId.Key);
				_target.Complete(ex, dropPendingMessages: true, storeExceptionEvenIfAlreadyCompleting: true);
			}
		}
		finally
		{
			_target.SignalOneAsyncMessageCompleted();
		}
	}

	private async Task StoreOutputItemsAsync(KeyValuePair<TInput, long> messageWithId, IAsyncEnumerable<TOutput> outputItems)
	{
		if (_reorderingBuffer != null)
		{
			await StoreOutputItemsReorderedAsync(messageWithId.Value, outputItems).ConfigureAwait(continueOnCapturedContext: false);
		}
		else if (outputItems != null)
		{
			await StoreOutputItemsNonReorderedWithIterationAsync(outputItems).ConfigureAwait(continueOnCapturedContext: false);
		}
		else if (_target.IsBounded)
		{
			_target.ChangeBoundingCount(-1);
		}
	}

	private async Task StoreOutputItemsReorderedAsync(long id, IAsyncEnumerable<TOutput> item)
	{
		TargetCore<TInput> target = _target;
		bool isBounded = target.IsBounded;
		if (item == null)
		{
			_reorderingBuffer.AddItem(id, null, itemIsValid: false);
			if (isBounded)
			{
				target.ChangeBoundingCount(-1);
			}
			return;
		}
		List<TOutput> itemCopy = null;
		try
		{
			if (_reorderingBuffer.IsNext(id))
			{
				await StoreOutputItemsNonReorderedWithIterationAsync(item).ConfigureAwait(continueOnCapturedContext: false);
				return;
			}
			int itemCount = 0;
			try
			{
				itemCopy = new List<TOutput>();
				await foreach (TOutput item2 in item.ConfigureAwait(continueOnCapturedContext: true))
				{
					itemCopy.Add(item2);
				}
				itemCount = itemCopy.Count;
			}
			finally
			{
				if (isBounded)
				{
					UpdateBoundingCountWithOutputCount(itemCount);
				}
			}
		}
		finally
		{
			_reorderingBuffer.AddItem(id, itemCopy, itemCopy != null);
		}
	}

	private async Task StoreOutputItemsNonReorderedWithIterationAsync(IAsyncEnumerable<TOutput> outputItems)
	{
		bool isSerial = _target.DataflowBlockOptions.MaxDegreeOfParallelism == 1 || _reorderingBuffer != null;
		if (_target.IsBounded)
		{
			bool outputFirstItem = false;
			try
			{
				await foreach (TOutput item in outputItems.ConfigureAwait(continueOnCapturedContext: true))
				{
					if (outputFirstItem)
					{
						_target.ChangeBoundingCount(1);
					}
					outputFirstItem = true;
					if (isSerial)
					{
						_source.AddMessage(item);
						continue;
					}
					lock (ParallelSourceLock)
					{
						_source.AddMessage(item);
					}
				}
				return;
			}
			finally
			{
				if (!outputFirstItem)
				{
					_target.ChangeBoundingCount(-1);
				}
			}
		}
		if (isSerial)
		{
			await foreach (TOutput item2 in outputItems.ConfigureAwait(continueOnCapturedContext: true))
			{
				_source.AddMessage(item2);
			}
			return;
		}
		await foreach (TOutput item3 in outputItems.ConfigureAwait(continueOnCapturedContext: true))
		{
			lock (ParallelSourceLock)
			{
				_source.AddMessage(item3);
			}
		}
	}
}
