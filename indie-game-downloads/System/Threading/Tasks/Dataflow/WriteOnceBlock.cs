using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow.Internal;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a buffer for receiving and storing at most one element in a network of dataflow blocks.</summary>
/// <typeparam name="T">Specifies the type of the data buffered by this dataflow block.</typeparam>
[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
[DebuggerTypeProxy(typeof(WriteOnceBlock<>.DebugView))]
public sealed class WriteOnceBlock<T> : IPropagatorBlock<T, T>, ITargetBlock<T>, IDataflowBlock, ISourceBlock<T>, IReceivableSourceBlock<T>, IDebuggerDisplay
{
	private sealed class DebugView
	{
		private readonly WriteOnceBlock<T> _writeOnceBlock;

		public bool IsCompleted => _writeOnceBlock.Completion.IsCompleted;

		public int Id => Common.GetBlockId(_writeOnceBlock);

		public bool HasValue => _writeOnceBlock.HasValue;

		public T Value => _writeOnceBlock.Value;

		public DataflowBlockOptions DataflowBlockOptions => _writeOnceBlock._dataflowBlockOptions;

		public TargetRegistry<T> LinkedTargets => _writeOnceBlock._targetRegistry;

		public DebugView(WriteOnceBlock<T> writeOnceBlock)
		{
			_writeOnceBlock = writeOnceBlock;
		}
	}

	private readonly TargetRegistry<T> _targetRegistry;

	private readonly Func<T, T> _cloningFunction;

	private readonly DataflowBlockOptions _dataflowBlockOptions;

	private TaskCompletionSource<VoidResult> _lazyCompletionTaskSource;

	private bool _decliningPermanently;

	private bool _completionReserved;

	private DataflowMessageHeader _header;

	private T _value;

	private object ValueLock => _targetRegistry;

	/// <summary>Gets a <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation and completion of the dataflow block.</summary>
	/// <returns>The task.</returns>
	public Task Completion => CompletionTaskSource.Task;

	private TaskCompletionSource<VoidResult> CompletionTaskSource
	{
		get
		{
			if (_lazyCompletionTaskSource == null)
			{
				Interlocked.CompareExchange(ref _lazyCompletionTaskSource, new TaskCompletionSource<VoidResult>(TaskCreationOptions.RunContinuationsAsynchronously), null);
			}
			return _lazyCompletionTaskSource;
		}
	}

	private bool HasValue => _header.IsValid;

	private T? Value
	{
		get
		{
			if (!_header.IsValid)
			{
				return default(T);
			}
			return _value;
		}
	}

	private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this, _dataflowBlockOptions)}, HasValue = {HasValue}, Value = {Value}";

	object IDebuggerDisplay.Content => DebuggerDisplayContent;

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.WriteOnceBlock`1" />.</summary>
	/// <param name="cloningFunction">The function to use to clone the data when offered to other blocks.</param>
	public WriteOnceBlock(Func<T, T>? cloningFunction)
		: this(cloningFunction, DataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.WriteOnceBlock`1" /> with the specified <see cref="T:System.Threading.Tasks.Dataflow.DataflowBlockOptions" />.</summary>
	/// <param name="cloningFunction">The function to use to clone the data when offered to other blocks.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.WriteOnceBlock`1" />.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public WriteOnceBlock(Func<T, T>? cloningFunction, DataflowBlockOptions dataflowBlockOptions)
	{
		ArgumentNullException.ThrowIfNull(dataflowBlockOptions, "dataflowBlockOptions");
		_cloningFunction = cloningFunction;
		_dataflowBlockOptions = dataflowBlockOptions.DefaultOrClone();
		_targetRegistry = new TargetRegistry<T>(this);
		if (dataflowBlockOptions.CancellationToken.CanBeCanceled)
		{
			_lazyCompletionTaskSource = new TaskCompletionSource<VoidResult>(TaskCreationOptions.RunContinuationsAsynchronously);
			if (dataflowBlockOptions.CancellationToken.IsCancellationRequested)
			{
				_completionReserved = (_decliningPermanently = true);
				_lazyCompletionTaskSource.TrySetCanceled(dataflowBlockOptions.CancellationToken);
			}
			else
			{
				Common.WireCancellationToComplete(dataflowBlockOptions.CancellationToken, _lazyCompletionTaskSource.Task, delegate(object state, CancellationToken _)
				{
					((WriteOnceBlock<T>)state).Complete();
				}, this);
			}
		}
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCreated(this, dataflowBlockOptions);
		}
	}

	private void CompleteBlockAsync(IList<Exception> exceptions)
	{
		if (exceptions == null)
		{
			Task task = new Task(delegate(object state)
			{
				((WriteOnceBlock<T>)state).OfferToTargetsAndCompleteBlock();
			}, this, TaskCreationOptions.DenyChildAttach);
			DataflowEtwProvider log = DataflowEtwProvider.Log;
			if (log.IsEnabled())
			{
				log.TaskLaunchedForMessageHandling(this, task, DataflowEtwProvider.TaskLaunchedReason.OfferingOutputMessages, _header.IsValid ? 1 : 0);
			}
			Exception ex = Common.StartTaskSafe(task, _dataflowBlockOptions.TaskScheduler);
			if (ex != null)
			{
				CompleteCore(ex, storeExceptionEvenIfAlreadyCompleting: true);
			}
		}
		else
		{
			Task.Factory.StartNew(delegate(object state)
			{
				Tuple<WriteOnceBlock<T>, IList<Exception>> tuple = (Tuple<WriteOnceBlock<T>, IList<Exception>>)state;
				tuple.Item1.CompleteBlock(tuple.Item2);
			}, Tuple.Create(this, exceptions), CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}
	}

	private void OfferToTargetsAndCompleteBlock()
	{
		List<Exception> exceptions = OfferToTargets();
		CompleteBlock(exceptions);
	}

	private void CompleteBlock(IList<Exception> exceptions)
	{
		TargetRegistry<T>.LinkedTargetInfo firstTarget = _targetRegistry.ClearEntryPoints();
		if (exceptions != null && exceptions.Count > 0)
		{
			CompletionTaskSource.TrySetException(exceptions);
		}
		else if (_dataflowBlockOptions.CancellationToken.IsCancellationRequested)
		{
			CompletionTaskSource.TrySetCanceled(_dataflowBlockOptions.CancellationToken);
		}
		else if (Interlocked.CompareExchange(ref _lazyCompletionTaskSource, Common.CompletedVoidResultTaskCompletionSource, null) != null)
		{
			_lazyCompletionTaskSource.TrySetResult(default(VoidResult));
		}
		_targetRegistry.PropagateCompletion(firstTarget);
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCompleted(this);
		}
	}

	/// <summary>Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.</summary>
	/// <param name="exception">The <see cref="T:System.Exception" /> that caused the faulting.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> is null.</exception>
	void IDataflowBlock.Fault(Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception, "exception");
		CompleteCore(exception, storeExceptionEvenIfAlreadyCompleting: false);
	}

	/// <summary>Signals to the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> that it should not accept nor produce any more messages nor consume any more postponed messages.</summary>
	public void Complete()
	{
		CompleteCore(null, storeExceptionEvenIfAlreadyCompleting: false);
	}

	private void CompleteCore(Exception exception, bool storeExceptionEvenIfAlreadyCompleting)
	{
		bool flag = false;
		lock (ValueLock)
		{
			if (_decliningPermanently && !storeExceptionEvenIfAlreadyCompleting)
			{
				return;
			}
			_decliningPermanently = true;
			if (!_completionReserved | storeExceptionEvenIfAlreadyCompleting)
			{
				flag = (_completionReserved = true);
			}
		}
		if (flag)
		{
			List<Exception> list = null;
			if (exception != null)
			{
				list = new List<Exception>();
				list.Add(exception);
			}
			CompleteBlockAsync(list);
		}
	}

	/// <summary>Attempts to synchronously receive an available output item from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if an item could be received; otherwise, false.</returns>
	/// <param name="filter">The predicate value must successfully pass in order for it to be received. <paramref name="filter" /> may be null, in which case all items will pass.</param>
	/// <param name="item">The item received from the source.</param>
	public bool TryReceive(Predicate<T>? filter, [MaybeNullWhen(false)] out T item)
	{
		if (_header.IsValid && (filter == null || filter(_value)))
		{
			item = CloneItem(_value);
			return true;
		}
		item = default(T);
		return false;
	}

	bool IReceivableSourceBlock<T>.TryReceiveAll([NotNullWhen(true)] out IList<T> items)
	{
		if (TryReceive(null, out var item))
		{
			items = new T[1] { item };
			return true;
		}
		items = null;
		return false;
	}

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect this source.</param>
	/// <param name="linkOptions">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" /> instance that configures the link.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="target" /> is null (Nothing in Visual Basic) or <paramref name="linkOptions" /> is null (Nothing in Visual Basic).</exception>
	public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
	{
		ArgumentNullException.ThrowIfNull(target, "target");
		ArgumentNullException.ThrowIfNull(linkOptions, "linkOptions");
		bool hasValue;
		lock (ValueLock)
		{
			hasValue = HasValue;
			bool completionReserved = _completionReserved;
			if (!hasValue && !completionReserved)
			{
				_targetRegistry.Add(ref target, linkOptions);
				return Common.CreateUnlinker<T>(ValueLock, _targetRegistry, target);
			}
		}
		if (hasValue)
		{
			bool consumeToAccept = _cloningFunction != null;
			target.OfferMessage(_header, _value, this, consumeToAccept);
		}
		if (linkOptions.PropagateCompletion)
		{
			Common.PropagateCompletionOnceCompleted(Completion, target);
		}
		return Disposables.Nop;
	}

	DataflowMessageStatus ITargetBlock<T>.OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
	{
		if (!messageHeader.IsValid)
		{
			throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
		}
		if ((source == null) & consumeToAccept)
		{
			throw new ArgumentException(System.SR.Argument_CantConsumeFromANullSource, "consumeToAccept");
		}
		bool flag = false;
		lock (ValueLock)
		{
			if (_decliningPermanently)
			{
				return DataflowMessageStatus.DecliningPermanently;
			}
			if (consumeToAccept)
			{
				messageValue = source.ConsumeMessage(messageHeader, this, out var messageConsumed);
				if (!messageConsumed)
				{
					return DataflowMessageStatus.NotAvailable;
				}
			}
			_header = Common.SingleMessageHeader;
			_value = messageValue;
			_decliningPermanently = true;
			if (!_completionReserved)
			{
				flag = (_completionReserved = true);
			}
		}
		if (flag)
		{
			CompleteBlockAsync(null);
		}
		return DataflowMessageStatus.Accepted;
	}

	T ISourceBlock<T>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
	{
		if (!messageHeader.IsValid)
		{
			throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (_header.Id == messageHeader.Id)
		{
			messageConsumed = true;
			return CloneItem(_value);
		}
		messageConsumed = false;
		return default(T);
	}

	bool ISourceBlock<T>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
	{
		if (!messageHeader.IsValid)
		{
			throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		return _header.Id == messageHeader.Id;
	}

	void ISourceBlock<T>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
	{
		if (!messageHeader.IsValid)
		{
			throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (_header.Id != messageHeader.Id)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_MessageNotReservedByTarget);
		}
		bool consumeToAccept = _cloningFunction != null;
		target.OfferMessage(_header, _value, this, consumeToAccept);
	}

	private T CloneItem(T item)
	{
		if (_cloningFunction == null)
		{
			return item;
		}
		return _cloningFunction(item);
	}

	private List<Exception> OfferToTargets()
	{
		List<Exception> list = null;
		if (HasValue)
		{
			TargetRegistry<T>.LinkedTargetInfo linkedTargetInfo = _targetRegistry.FirstTargetNode;
			while (linkedTargetInfo != null)
			{
				TargetRegistry<T>.LinkedTargetInfo next = linkedTargetInfo.Next;
				ITargetBlock<T> target = linkedTargetInfo.Target;
				try
				{
					bool consumeToAccept = _cloningFunction != null;
					target.OfferMessage(_header, _value, this, consumeToAccept);
				}
				catch (Exception ex)
				{
					Common.StoreDataflowMessageValueIntoExceptionData(ex, _value);
					Common.AddException(ref list, ex);
				}
				linkedTargetInfo = next;
			}
		}
		return list;
	}

	/// <summary>Returns a string that represents the formatted name of this <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> instance.</summary>
	/// <returns>A string that represents the formatted name of this <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> instance.</returns>
	public override string ToString()
	{
		return Common.GetNameForDebugger(this, _dataflowBlockOptions);
	}
}
