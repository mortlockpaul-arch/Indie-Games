using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow.Internal;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a dataflow block that joins across multiple dataflow sources, not necessarily of the same type, waiting for one item to arrive for each type before they’re all released together as a tuple consisting of one item per type.</summary>
/// <typeparam name="T1">Specifies the type of data accepted by the block's first target.</typeparam>
/// <typeparam name="T2">Specifies the type of data accepted by the block's second target.</typeparam>
[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
[DebuggerTypeProxy(typeof(JoinBlock<, >.DebugView))]
public sealed class JoinBlock<T1, T2> : IReceivableSourceBlock<Tuple<T1, T2>>, ISourceBlock<Tuple<T1, T2>>, IDataflowBlock, IDebuggerDisplay
{
	private sealed class DebugView
	{
		private readonly JoinBlock<T1, T2> _joinBlock;

		private readonly SourceCore<Tuple<T1, T2>>.DebuggingInformation _sourceDebuggingInformation;

		public IEnumerable<Tuple<T1, T2>> OutputQueue => _sourceDebuggingInformation.OutputQueue;

		public long JoinsCreated => _joinBlock._sharedResources._joinsCreated;

		public Task TaskForInputProcessing => _joinBlock._sharedResources._taskForInputProcessing;

		public Task TaskForOutputProcessing => _sourceDebuggingInformation.TaskForOutputProcessing;

		public GroupingDataflowBlockOptions DataflowBlockOptions => (GroupingDataflowBlockOptions)_sourceDebuggingInformation.DataflowBlockOptions;

		public bool IsDecliningPermanently => _joinBlock._sharedResources._decliningPermanently;

		public bool IsCompleted => _sourceDebuggingInformation.IsCompleted;

		public int Id => Common.GetBlockId(_joinBlock);

		public ITargetBlock<T1> Target1 => _joinBlock._target1;

		public ITargetBlock<T2> Target2 => _joinBlock._target2;

		public TargetRegistry<Tuple<T1, T2>> LinkedTargets => _sourceDebuggingInformation.LinkedTargets;

		public ITargetBlock<Tuple<T1, T2>> NextMessageReservedFor => _sourceDebuggingInformation.NextMessageReservedFor;

		public DebugView(JoinBlock<T1, T2> joinBlock)
		{
			_joinBlock = joinBlock;
			_sourceDebuggingInformation = joinBlock._source.GetDebuggingInformation();
		}
	}

	private readonly JoinBlockTargetSharedResources _sharedResources;

	private readonly SourceCore<Tuple<T1, T2>> _source;

	private readonly JoinBlockTarget<T1> _target1;

	private readonly JoinBlockTarget<T2> _target2;

	/// <summary>Gets the number of output items available to be received from this block.</summary>
	/// <returns>The number of output items.</returns>
	public int OutputCount => _source.OutputCount;

	/// <summary>Gets a <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation and completion of the dataflow block.</summary>
	/// <returns>The task.</returns>
	public Task Completion => _source.Completion;

	/// <summary>Gets a target that may be used to offer messages of the first type.</summary>
	/// <returns>The target.</returns>
	public ITargetBlock<T1> Target1 => _target1;

	/// <summary>Gets a target that may be used to offer messages of the second type.</summary>
	/// <returns>The target.</returns>
	public ITargetBlock<T2> Target2 => _target2;

	private int OutputCountForDebugger => _source.GetDebuggingInformation().OutputCount;

	private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this, _source.DataflowBlockOptions)}, OutputCount = {OutputCountForDebugger}";

	object IDebuggerDisplay.Content => DebuggerDisplayContent;

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.JoinBlock`2" />.</summary>
	public JoinBlock()
		: this(GroupingDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.JoinBlock`2" />.</summary>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.JoinBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public JoinBlock(GroupingDataflowBlockOptions dataflowBlockOptions)
	{
		ArgumentNullException.ThrowIfNull(dataflowBlockOptions, "dataflowBlockOptions");
		dataflowBlockOptions = dataflowBlockOptions.DefaultOrClone();
		Action<ISourceBlock<Tuple<T1, T2>>, int> itemsRemovedAction = null;
		if (dataflowBlockOptions.BoundedCapacity > 0)
		{
			itemsRemovedAction = delegate(ISourceBlock<Tuple<T1, T2>> owningSource, int count)
			{
				((JoinBlock<T1, T2>)owningSource)._sharedResources.OnItemsRemoved(count);
			};
		}
		_source = new SourceCore<Tuple<T1, T2>>(this, dataflowBlockOptions, delegate(ISourceBlock<Tuple<T1, T2>> owningSource)
		{
			((JoinBlock<T1, T2>)owningSource)._sharedResources.CompleteEachTarget();
		}, itemsRemovedAction);
		JoinBlockTargetBase[] array = new JoinBlockTargetBase[2];
		_sharedResources = new JoinBlockTargetSharedResources(this, array, delegate
		{
			_source.AddMessage(Tuple.Create(_target1.GetOneMessage(), _target2.GetOneMessage()));
		}, delegate(Exception exception)
		{
			Volatile.Write(ref _sharedResources._hasExceptions, value: true);
			_source.AddException(exception);
		}, dataflowBlockOptions);
		array[0] = (_target1 = new JoinBlockTarget<T1>(_sharedResources));
		array[1] = (_target2 = new JoinBlockTarget<T2>(_sharedResources));
		Task.Factory.ContinueWhenAll(new Task[2] { _target1.CompletionTaskInternal, _target2.CompletionTaskInternal }, delegate
		{
			_source.Complete();
		}, CancellationToken.None, Common.GetContinuationOptions(), TaskScheduler.Default);
		_source.Completion.ContinueWith(delegate(Task completed, object state)
		{
			((IDataflowBlock)(JoinBlock<T1, T2>)state).Fault((Exception)completed.Exception);
		}, this, CancellationToken.None, Common.GetContinuationOptions() | TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
		Common.WireCancellationToComplete(dataflowBlockOptions.CancellationToken, _source.Completion, delegate(object state, CancellationToken _)
		{
			((JoinBlock<T1, T2>)state)._sharedResources.CompleteEachTarget();
		}, this);
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCreated(this, dataflowBlockOptions);
		}
	}

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect this source.</param>
	/// <param name="linkOptions">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" /> instance that configures the link.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="target" /> is null (Nothing in Visual Basic) or <paramref name="linkOptions" /> is null (Nothing in Visual Basic).</exception>
	public IDisposable LinkTo(ITargetBlock<Tuple<T1, T2>> target, DataflowLinkOptions linkOptions)
	{
		return _source.LinkTo(target, linkOptions);
	}

	/// <summary>Attempts to synchronously receive an available output item from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if an item could be received; otherwise, false.</returns>
	/// <param name="filter">The predicate value must successfully pass in order for it to be received. <paramref name="filter" /> may be null, in which case all items will pass.</param>
	/// <param name="item">The item received from the source.</param>
	public bool TryReceive(Predicate<Tuple<T1, T2>>? filter, [NotNullWhen(true)] out Tuple<T1, T2>? item)
	{
		return _source.TryReceive(filter, out item);
	}

	/// <summary>Attempts to synchronously receive all available items from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if one or more items could be received; otherwise, false.</returns>
	/// <param name="items">The items received from the source.</param>
	public bool TryReceiveAll([NotNullWhen(true)] out IList<Tuple<T1, T2>>? items)
	{
		return _source.TryReceiveAll(out items);
	}

	/// <summary>Signals to the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> that it should not accept nor produce any more messages nor consume any more postponed messages.</summary>
	public void Complete()
	{
		_target1.CompleteCore(null, dropPendingMessages: false, releaseReservedMessages: false);
		_target2.CompleteCore(null, dropPendingMessages: false, releaseReservedMessages: false);
	}

	/// <summary>Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.</summary>
	/// <param name="exception">The <see cref="T:System.Exception" /> that caused the faulting.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> is null.</exception>
	void IDataflowBlock.Fault(Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception, "exception");
		lock (_sharedResources.IncomingLock)
		{
			if (!_sharedResources._decliningPermanently)
			{
				_sharedResources._exceptionAction(exception);
			}
		}
		Complete();
	}

	Tuple<T1, T2> ISourceBlock<Tuple<T1, T2>>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<Tuple<T1, T2>> target, out bool messageConsumed)
	{
		return _source.ConsumeMessage(messageHeader, target, out messageConsumed);
	}

	bool ISourceBlock<Tuple<T1, T2>>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<Tuple<T1, T2>> target)
	{
		return _source.ReserveMessage(messageHeader, target);
	}

	void ISourceBlock<Tuple<T1, T2>>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<Tuple<T1, T2>> target)
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
/// <summary>Provides a dataflow block that joins across multiple dataflow sources, which are not necessarily of the same type, waiting for one item to arrive for each type before they’re all released together as a tuple that contains one item per type.</summary>
/// <typeparam name="T1">Specifies the type of data accepted by the block's first target.</typeparam>
/// <typeparam name="T2">Specifies the type of data accepted by the block's second target.</typeparam>
/// <typeparam name="T3">Specifies the type of data accepted by the block's third target.</typeparam>
[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
[DebuggerTypeProxy(typeof(JoinBlock<, , >.DebugView))]
public sealed class JoinBlock<T1, T2, T3> : IReceivableSourceBlock<Tuple<T1, T2, T3>>, ISourceBlock<Tuple<T1, T2, T3>>, IDataflowBlock, IDebuggerDisplay
{
	private sealed class DebugView
	{
		private readonly JoinBlock<T1, T2, T3> _joinBlock;

		private readonly SourceCore<Tuple<T1, T2, T3>>.DebuggingInformation _sourceDebuggingInformation;

		public IEnumerable<Tuple<T1, T2, T3>> OutputQueue => _sourceDebuggingInformation.OutputQueue;

		public long JoinsCreated => _joinBlock._sharedResources._joinsCreated;

		public Task TaskForInputProcessing => _joinBlock._sharedResources._taskForInputProcessing;

		public Task TaskForOutputProcessing => _sourceDebuggingInformation.TaskForOutputProcessing;

		public GroupingDataflowBlockOptions DataflowBlockOptions => (GroupingDataflowBlockOptions)_sourceDebuggingInformation.DataflowBlockOptions;

		public bool IsDecliningPermanently => _joinBlock._sharedResources._decliningPermanently;

		public bool IsCompleted => _sourceDebuggingInformation.IsCompleted;

		public int Id => Common.GetBlockId(_joinBlock);

		public ITargetBlock<T1> Target1 => _joinBlock._target1;

		public ITargetBlock<T2> Target2 => _joinBlock._target2;

		public ITargetBlock<T3> Target3 => _joinBlock._target3;

		public TargetRegistry<Tuple<T1, T2, T3>> LinkedTargets => _sourceDebuggingInformation.LinkedTargets;

		public ITargetBlock<Tuple<T1, T2, T3>> NextMessageReservedFor => _sourceDebuggingInformation.NextMessageReservedFor;

		public DebugView(JoinBlock<T1, T2, T3> joinBlock)
		{
			_joinBlock = joinBlock;
			_sourceDebuggingInformation = joinBlock._source.GetDebuggingInformation();
		}
	}

	private readonly JoinBlockTargetSharedResources _sharedResources;

	private readonly SourceCore<Tuple<T1, T2, T3>> _source;

	private readonly JoinBlockTarget<T1> _target1;

	private readonly JoinBlockTarget<T2> _target2;

	private readonly JoinBlockTarget<T3> _target3;

	/// <summary>Gets the number of output items available to be received from this block.</summary>
	/// <returns>The number of output items.</returns>
	public int OutputCount => _source.OutputCount;

	/// <summary>Gets a <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation and completion of the dataflow block.</summary>
	/// <returns>The task.</returns>
	public Task Completion => _source.Completion;

	/// <summary>Gets a target that may be used to offer messages of the first type.</summary>
	/// <returns>The target.</returns>
	public ITargetBlock<T1> Target1 => _target1;

	/// <summary>Gets a target that may be used to offer messages of the second type.</summary>
	/// <returns>The target.</returns>
	public ITargetBlock<T2> Target2 => _target2;

	/// <summary>Gets a target that may be used to offer messages of the third type.</summary>
	/// <returns>The target.</returns>
	public ITargetBlock<T3> Target3 => _target3;

	private int OutputCountForDebugger => _source.GetDebuggingInformation().OutputCount;

	private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this, _source.DataflowBlockOptions)} OutputCount = {OutputCountForDebugger}";

	object IDebuggerDisplay.Content => DebuggerDisplayContent;

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.JoinBlock`3" />.</summary>
	public JoinBlock()
		: this(GroupingDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.JoinBlock`3" />.</summary>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.JoinBlock`2" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public JoinBlock(GroupingDataflowBlockOptions dataflowBlockOptions)
	{
		ArgumentNullException.ThrowIfNull(dataflowBlockOptions, "dataflowBlockOptions");
		dataflowBlockOptions = dataflowBlockOptions.DefaultOrClone();
		Action<ISourceBlock<Tuple<T1, T2, T3>>, int> itemsRemovedAction = null;
		if (dataflowBlockOptions.BoundedCapacity > 0)
		{
			itemsRemovedAction = delegate(ISourceBlock<Tuple<T1, T2, T3>> owningSource, int count)
			{
				((JoinBlock<T1, T2, T3>)owningSource)._sharedResources.OnItemsRemoved(count);
			};
		}
		_source = new SourceCore<Tuple<T1, T2, T3>>(this, dataflowBlockOptions, delegate(ISourceBlock<Tuple<T1, T2, T3>> owningSource)
		{
			((JoinBlock<T1, T2, T3>)owningSource)._sharedResources.CompleteEachTarget();
		}, itemsRemovedAction);
		JoinBlockTargetBase[] array = new JoinBlockTargetBase[3];
		_sharedResources = new JoinBlockTargetSharedResources(this, array, delegate
		{
			_source.AddMessage(Tuple.Create(_target1.GetOneMessage(), _target2.GetOneMessage(), _target3.GetOneMessage()));
		}, delegate(Exception exception)
		{
			Volatile.Write(ref _sharedResources._hasExceptions, value: true);
			_source.AddException(exception);
		}, dataflowBlockOptions);
		array[0] = (_target1 = new JoinBlockTarget<T1>(_sharedResources));
		array[1] = (_target2 = new JoinBlockTarget<T2>(_sharedResources));
		array[2] = (_target3 = new JoinBlockTarget<T3>(_sharedResources));
		Task.Factory.ContinueWhenAll(new Task[3] { _target1.CompletionTaskInternal, _target2.CompletionTaskInternal, _target3.CompletionTaskInternal }, delegate
		{
			_source.Complete();
		}, CancellationToken.None, Common.GetContinuationOptions(), TaskScheduler.Default);
		_source.Completion.ContinueWith(delegate(Task completed, object state)
		{
			((IDataflowBlock)(JoinBlock<T1, T2, T3>)state).Fault((Exception)completed.Exception);
		}, this, CancellationToken.None, Common.GetContinuationOptions() | TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
		Common.WireCancellationToComplete(dataflowBlockOptions.CancellationToken, _source.Completion, delegate(object state, CancellationToken _)
		{
			((JoinBlock<T1, T2, T3>)state)._sharedResources.CompleteEachTarget();
		}, this);
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCreated(this, dataflowBlockOptions);
		}
	}

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect this source.</param>
	/// <param name="linkOptions">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" /> instance that configures the link.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="target" /> is null (Nothing in Visual Basic) or <paramref name="linkOptions" /> is null (Nothing in Visual Basic).</exception>
	public IDisposable LinkTo(ITargetBlock<Tuple<T1, T2, T3>> target, DataflowLinkOptions linkOptions)
	{
		return _source.LinkTo(target, linkOptions);
	}

	/// <summary>Attempts to synchronously receive an available output item from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if an item could be received; otherwise, false.</returns>
	/// <param name="filter">The predicate value must successfully pass in order for it to be received. <paramref name="filter" /> may be null, in which case all items will pass.</param>
	/// <param name="item">The item received from the source.</param>
	public bool TryReceive(Predicate<Tuple<T1, T2, T3>>? filter, [NotNullWhen(true)] out Tuple<T1, T2, T3>? item)
	{
		return _source.TryReceive(filter, out item);
	}

	/// <summary>Attempts to synchronously receive all available items from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if one or more items could be received; otherwise, false.</returns>
	/// <param name="items">The items received from the source.</param>
	public bool TryReceiveAll([NotNullWhen(true)] out IList<Tuple<T1, T2, T3>>? items)
	{
		return _source.TryReceiveAll(out items);
	}

	/// <summary>Signals to the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> that it should not accept nor produce any more messages nor consume any more postponed messages.</summary>
	public void Complete()
	{
		_target1.CompleteCore(null, dropPendingMessages: false, releaseReservedMessages: false);
		_target2.CompleteCore(null, dropPendingMessages: false, releaseReservedMessages: false);
		_target3.CompleteCore(null, dropPendingMessages: false, releaseReservedMessages: false);
	}

	/// <summary>Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.</summary>
	/// <param name="exception">The <see cref="T:System.Exception" /> that caused the faulting.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> is null.</exception>
	void IDataflowBlock.Fault(Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception, "exception");
		lock (_sharedResources.IncomingLock)
		{
			if (!_sharedResources._decliningPermanently)
			{
				_sharedResources._exceptionAction(exception);
			}
		}
		Complete();
	}

	Tuple<T1, T2, T3> ISourceBlock<Tuple<T1, T2, T3>>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<Tuple<T1, T2, T3>> target, out bool messageConsumed)
	{
		return _source.ConsumeMessage(messageHeader, target, out messageConsumed);
	}

	bool ISourceBlock<Tuple<T1, T2, T3>>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<Tuple<T1, T2, T3>> target)
	{
		return _source.ReserveMessage(messageHeader, target);
	}

	void ISourceBlock<Tuple<T1, T2, T3>>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<Tuple<T1, T2, T3>> target)
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
