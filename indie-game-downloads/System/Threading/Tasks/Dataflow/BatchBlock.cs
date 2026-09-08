using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks.Dataflow.Internal;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a dataflow block that batches inputs into arrays.</summary>
/// <typeparam name="T">Specifies the type of data put into batches.</typeparam>
[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
[DebuggerTypeProxy(typeof(BatchBlock<>.DebugView))]
public sealed class BatchBlock<T> : IPropagatorBlock<T, T[]>, ITargetBlock<T>, IDataflowBlock, ISourceBlock<T[]>, IReceivableSourceBlock<T[]>, IDebuggerDisplay
{
	private sealed class DebugView
	{
		private readonly BatchBlock<T> _batchBlock;

		private readonly BatchBlockTargetCore.DebuggingInformation _targetDebuggingInformation;

		private readonly SourceCore<T[]>.DebuggingInformation _sourceDebuggingInformation;

		public IEnumerable<T> InputQueue => _targetDebuggingInformation.InputQueue;

		public IEnumerable<T[]> OutputQueue => _sourceDebuggingInformation.OutputQueue;

		public long BatchesCompleted => _targetDebuggingInformation.NumberOfBatchesCompleted;

		public Task TaskForInputProcessing => _targetDebuggingInformation.TaskForInputProcessing;

		public Task TaskForOutputProcessing => _sourceDebuggingInformation.TaskForOutputProcessing;

		public GroupingDataflowBlockOptions DataflowBlockOptions => _targetDebuggingInformation.DataflowBlockOptions;

		public int BatchSize => _batchBlock.BatchSize;

		public bool IsDecliningPermanently => _targetDebuggingInformation.IsDecliningPermanently;

		public bool IsCompleted => _sourceDebuggingInformation.IsCompleted;

		public int Id => Common.GetBlockId(_batchBlock);

		public QueuedMap<ISourceBlock<T>, DataflowMessageHeader> PostponedMessages => _targetDebuggingInformation.PostponedMessages;

		public TargetRegistry<T[]> LinkedTargets => _sourceDebuggingInformation.LinkedTargets;

		public ITargetBlock<T[]> NextMessageReservedFor => _sourceDebuggingInformation.NextMessageReservedFor;

		public DebugView(BatchBlock<T> batchBlock)
		{
			_batchBlock = batchBlock;
			_targetDebuggingInformation = batchBlock._target.GetDebuggingInformation();
			_sourceDebuggingInformation = batchBlock._source.GetDebuggingInformation();
		}
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	private sealed class BatchBlockTargetCore
	{
		private sealed class NonGreedyState
		{
			internal readonly QueuedMap<ISourceBlock<T>, DataflowMessageHeader> PostponedMessages;

			internal readonly KeyValuePair<ISourceBlock<T>, DataflowMessageHeader>[] PostponedMessagesTemp;

			internal readonly List<KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>> ReservedSourcesTemp;

			internal bool AcceptFewerThanBatchSize;

			internal Task TaskForInputProcessing;

			internal NonGreedyState(int batchSize)
			{
				PostponedMessages = new QueuedMap<ISourceBlock<T>, DataflowMessageHeader>(batchSize);
				PostponedMessagesTemp = new KeyValuePair<ISourceBlock<T>, DataflowMessageHeader>[batchSize];
				ReservedSourcesTemp = new List<KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>>(batchSize);
			}
		}

		internal sealed class DebuggingInformation
		{
			private readonly BatchBlockTargetCore _target;

			public IEnumerable<T> InputQueue => _target._messages.ToList();

			public Task TaskForInputProcessing => _target._nonGreedyState?.TaskForInputProcessing;

			public QueuedMap<ISourceBlock<T>, DataflowMessageHeader> PostponedMessages => _target._nonGreedyState?.PostponedMessages;

			public bool IsDecliningPermanently => _target._decliningPermanently;

			public GroupingDataflowBlockOptions DataflowBlockOptions => _target._dataflowBlockOptions;

			public long NumberOfBatchesCompleted => _target._batchesCompleted;

			public DebuggingInformation(BatchBlockTargetCore target)
			{
				_target = target;
			}
		}

		private readonly Queue<T> _messages = new Queue<T>();

		private readonly TaskCompletionSource<VoidResult> _completionTask = new TaskCompletionSource<VoidResult>();

		private readonly BatchBlock<T> _owningBatch;

		private readonly int _batchSize;

		private readonly NonGreedyState _nonGreedyState;

		private readonly BoundingState _boundingState;

		private readonly GroupingDataflowBlockOptions _dataflowBlockOptions;

		private readonly Action<T[]> _batchCompletedAction;

		private bool _decliningPermanently;

		private long _batchesCompleted;

		private bool _completionReserved;

		private object IncomingLock => _completionTask;

		internal Task Completion => _completionTask.Task;

		internal int BatchSize => _batchSize;

		private bool CanceledOrFaulted
		{
			get
			{
				if (!_dataflowBlockOptions.CancellationToken.IsCancellationRequested)
				{
					return _owningBatch._source.HasExceptions;
				}
				return true;
			}
		}

		private int BoundedCapacityAvailable
		{
			get
			{
				if (_boundingState == null)
				{
					return _batchSize;
				}
				return _dataflowBlockOptions.BoundedCapacity - _boundingState.CurrentCount;
			}
		}

		private bool BatchesNeedProcessing
		{
			get
			{
				bool num = _batchesCompleted >= _dataflowBlockOptions.ActualMaxNumberOfGroups;
				bool flag = _nonGreedyState != null && _nonGreedyState.TaskForInputProcessing != null;
				if ((num | flag) || CanceledOrFaulted)
				{
					return false;
				}
				int num2 = _batchSize - _messages.Count;
				int boundedCapacityAvailable = BoundedCapacityAvailable;
				if (num2 <= 0)
				{
					return true;
				}
				if (_nonGreedyState != null)
				{
					if (_nonGreedyState.AcceptFewerThanBatchSize && _messages.Count > 0)
					{
						return true;
					}
					if (_decliningPermanently)
					{
						return false;
					}
					if (_nonGreedyState.AcceptFewerThanBatchSize && _nonGreedyState.PostponedMessages.Count > 0 && boundedCapacityAvailable > 0)
					{
						return true;
					}
					if (_dataflowBlockOptions.Greedy)
					{
						if (_nonGreedyState.PostponedMessages.Count > 0 && boundedCapacityAvailable > 0)
						{
							return true;
						}
					}
					else if (_nonGreedyState.PostponedMessages.Count >= num2 && boundedCapacityAvailable >= num2)
					{
						return true;
					}
				}
				return false;
			}
		}

		private object DebuggerDisplayContent
		{
			get
			{
				IDebuggerDisplay owningBatch = _owningBatch;
				return $"Block = \"{((owningBatch != null) ? owningBatch.Content : _owningBatch)}\"";
			}
		}

		internal BatchBlockTargetCore(BatchBlock<T> owningBatch, int batchSize, Action<T[]> batchCompletedAction, GroupingDataflowBlockOptions dataflowBlockOptions)
		{
			_owningBatch = owningBatch;
			_batchSize = batchSize;
			_batchCompletedAction = batchCompletedAction;
			_dataflowBlockOptions = dataflowBlockOptions;
			bool flag = dataflowBlockOptions.BoundedCapacity > 0;
			if (!_dataflowBlockOptions.Greedy | flag)
			{
				_nonGreedyState = new NonGreedyState(batchSize);
			}
			if (flag)
			{
				_boundingState = new BoundingState(dataflowBlockOptions.BoundedCapacity);
			}
		}

		internal void TriggerBatch()
		{
			lock (IncomingLock)
			{
				if (!_decliningPermanently && !_dataflowBlockOptions.CancellationToken.IsCancellationRequested)
				{
					if (_nonGreedyState == null)
					{
						MakeBatchIfPossible(evenIfFewerThanBatchSize: true);
					}
					else
					{
						_nonGreedyState.AcceptFewerThanBatchSize = true;
						ProcessAsyncIfNecessary();
					}
				}
				CompleteBlockIfPossible();
			}
		}

		internal DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if ((source == null) & consumeToAccept)
			{
				throw new ArgumentException(System.SR.Argument_CantConsumeFromANullSource, "consumeToAccept");
			}
			lock (IncomingLock)
			{
				if (_decliningPermanently)
				{
					CompleteBlockIfPossible();
					return DataflowMessageStatus.DecliningPermanently;
				}
				if (_dataflowBlockOptions.Greedy && (_boundingState == null || (_boundingState.CountIsLessThanBound && _nonGreedyState.PostponedMessages.Count == 0 && _nonGreedyState.TaskForInputProcessing == null)))
				{
					if (consumeToAccept)
					{
						messageValue = source.ConsumeMessage(messageHeader, _owningBatch, out var messageConsumed);
						if (!messageConsumed)
						{
							return DataflowMessageStatus.NotAvailable;
						}
					}
					_messages.Enqueue(messageValue);
					if (_boundingState != null)
					{
						_boundingState.CurrentCount++;
					}
					if (!_decliningPermanently && _batchesCompleted + _messages.Count / _batchSize >= _dataflowBlockOptions.ActualMaxNumberOfGroups)
					{
						_decliningPermanently = true;
					}
					MakeBatchIfPossible(evenIfFewerThanBatchSize: false);
					CompleteBlockIfPossible();
					return DataflowMessageStatus.Accepted;
				}
				if (source != null)
				{
					_nonGreedyState.PostponedMessages.Push(source, messageHeader);
					if (!_dataflowBlockOptions.Greedy)
					{
						ProcessAsyncIfNecessary();
					}
					return DataflowMessageStatus.Postponed;
				}
				return DataflowMessageStatus.Declined;
			}
		}

		internal void Complete(Exception exception, bool dropPendingMessages, bool releaseReservedMessages, bool revertProcessingState = false)
		{
			lock (IncomingLock)
			{
				if (exception != null && (!_decliningPermanently | releaseReservedMessages))
				{
					_owningBatch._source.AddException(exception);
				}
				if (dropPendingMessages)
				{
					_messages.Clear();
				}
			}
			if (releaseReservedMessages)
			{
				try
				{
					ReleaseReservedMessages(throwOnFirstException: false);
				}
				catch (Exception exception2)
				{
					_owningBatch._source.AddException(exception2);
				}
			}
			lock (IncomingLock)
			{
				if (revertProcessingState)
				{
					_nonGreedyState.TaskForInputProcessing = null;
				}
				_decliningPermanently = true;
				CompleteBlockIfPossible();
			}
		}

		private void CompleteBlockIfPossible()
		{
			if (_completionReserved)
			{
				return;
			}
			bool num = _nonGreedyState != null && _nonGreedyState.TaskForInputProcessing != null;
			bool flag = _batchesCompleted >= _dataflowBlockOptions.ActualMaxNumberOfGroups;
			bool flag2 = _decliningPermanently && _messages.Count < _batchSize;
			if (num || (!(flag | flag2) && !CanceledOrFaulted))
			{
				return;
			}
			_completionReserved = true;
			_decliningPermanently = true;
			if (_messages.Count > 0)
			{
				MakeBatchIfPossible(evenIfFewerThanBatchSize: true);
			}
			Task.Factory.StartNew(delegate(object thisTargetCore)
			{
				BatchBlockTargetCore batchBlockTargetCore = (BatchBlockTargetCore)thisTargetCore;
				List<Exception> exceptions = null;
				if (batchBlockTargetCore._nonGreedyState != null)
				{
					Common.ReleaseAllPostponedMessages(batchBlockTargetCore._owningBatch, batchBlockTargetCore._nonGreedyState.PostponedMessages, ref exceptions);
				}
				if (exceptions != null)
				{
					batchBlockTargetCore._owningBatch._source.AddExceptions(exceptions);
				}
				batchBlockTargetCore._completionTask.TrySetResult(default(VoidResult));
			}, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		private void ProcessAsyncIfNecessary(bool isReplacementReplica = false)
		{
			if (BatchesNeedProcessing)
			{
				ProcessAsyncIfNecessary_Slow(isReplacementReplica);
			}
		}

		private void ProcessAsyncIfNecessary_Slow(bool isReplacementReplica)
		{
			_nonGreedyState.TaskForInputProcessing = new Task(delegate(object thisBatchTarget)
			{
				((BatchBlockTargetCore)thisBatchTarget).ProcessMessagesLoopCore();
			}, this, Common.GetCreationOptionsForTask(isReplacementReplica));
			DataflowEtwProvider log = DataflowEtwProvider.Log;
			if (log.IsEnabled())
			{
				log.TaskLaunchedForMessageHandling(_owningBatch, _nonGreedyState.TaskForInputProcessing, DataflowEtwProvider.TaskLaunchedReason.ProcessingInputMessages, _messages.Count + _nonGreedyState.PostponedMessages.Count);
			}
			Exception ex = Common.StartTaskSafe(_nonGreedyState.TaskForInputProcessing, _dataflowBlockOptions.TaskScheduler);
			if (ex != null)
			{
				Task.Factory.StartNew(delegate(object exc)
				{
					Complete((Exception)exc, dropPendingMessages: true, releaseReservedMessages: true, revertProcessingState: true);
				}, ex, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}
		}

		private void ProcessMessagesLoopCore()
		{
			try
			{
				int actualMaxMessagesPerTask = _dataflowBlockOptions.ActualMaxMessagesPerTask;
				int num = 0;
				bool flag2;
				do
				{
					bool flag = Volatile.Read(in _nonGreedyState.AcceptFewerThanBatchSize);
					if (!_dataflowBlockOptions.Greedy)
					{
						RetrievePostponedItemsNonGreedy(flag);
					}
					else
					{
						RetrievePostponedItemsGreedyBounded(flag);
					}
					lock (IncomingLock)
					{
						flag2 = MakeBatchIfPossible(flag);
						if (flag2 | flag)
						{
							_nonGreedyState.AcceptFewerThanBatchSize = false;
						}
					}
					num++;
				}
				while (flag2 && num < actualMaxMessagesPerTask);
			}
			catch (Exception exception)
			{
				Complete(exception, dropPendingMessages: false, releaseReservedMessages: true);
			}
			finally
			{
				lock (IncomingLock)
				{
					_nonGreedyState.TaskForInputProcessing = null;
					ProcessAsyncIfNecessary(isReplacementReplica: true);
					CompleteBlockIfPossible();
				}
			}
		}

		private bool MakeBatchIfPossible(bool evenIfFewerThanBatchSize)
		{
			bool flag = _messages.Count >= _batchSize;
			if (flag || (evenIfFewerThanBatchSize && _messages.Count > 0))
			{
				T[] array = new T[flag ? _batchSize : _messages.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = _messages.Dequeue();
				}
				_batchCompletedAction(array);
				_batchesCompleted++;
				if (_batchesCompleted >= _dataflowBlockOptions.ActualMaxNumberOfGroups)
				{
					_decliningPermanently = true;
				}
				return true;
			}
			return false;
		}

		private void RetrievePostponedItemsNonGreedy(bool allowFewerThanBatchSize)
		{
			QueuedMap<ISourceBlock<T>, DataflowMessageHeader> postponedMessages = _nonGreedyState.PostponedMessages;
			KeyValuePair<ISourceBlock<T>, DataflowMessageHeader>[] postponedMessagesTemp = _nonGreedyState.PostponedMessagesTemp;
			List<KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>> reservedSourcesTemp = _nonGreedyState.ReservedSourcesTemp;
			reservedSourcesTemp.Clear();
			int num;
			lock (IncomingLock)
			{
				int boundedCapacityAvailable = BoundedCapacityAvailable;
				if (_decliningPermanently || postponedMessages.Count == 0 || boundedCapacityAvailable <= 0 || (!allowFewerThanBatchSize && (postponedMessages.Count < _batchSize || boundedCapacityAvailable < _batchSize)))
				{
					return;
				}
				num = postponedMessages.PopRange(postponedMessagesTemp, 0, _batchSize);
			}
			for (int i = 0; i < num; i++)
			{
				KeyValuePair<ISourceBlock<T>, DataflowMessageHeader> keyValuePair = postponedMessagesTemp[i];
				if (keyValuePair.Key.ReserveMessage(keyValuePair.Value, _owningBatch))
				{
					KeyValuePair<DataflowMessageHeader, T> value = new KeyValuePair<DataflowMessageHeader, T>(keyValuePair.Value, default(T));
					KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> item = new KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>(keyValuePair.Key, value);
					reservedSourcesTemp.Add(item);
				}
			}
			Array.Clear(postponedMessagesTemp, 0, postponedMessagesTemp.Length);
			while (reservedSourcesTemp.Count < _batchSize)
			{
				KeyValuePair<ISourceBlock<T>, DataflowMessageHeader> item2;
				lock (IncomingLock)
				{
					if (!postponedMessages.TryPop(out item2))
					{
						break;
					}
				}
				if (item2.Key.ReserveMessage(item2.Value, _owningBatch))
				{
					KeyValuePair<DataflowMessageHeader, T> value2 = new KeyValuePair<DataflowMessageHeader, T>(item2.Value, default(T));
					KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> item3 = new KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>(item2.Key, value2);
					reservedSourcesTemp.Add(item3);
				}
			}
			if (reservedSourcesTemp.Count > 0)
			{
				bool flag = true;
				if (allowFewerThanBatchSize)
				{
					lock (IncomingLock)
					{
						if (!_decliningPermanently && _batchesCompleted + 1 >= _dataflowBlockOptions.ActualMaxNumberOfGroups)
						{
							flag = !_decliningPermanently;
							_decliningPermanently = true;
						}
					}
				}
				if (flag && (allowFewerThanBatchSize || reservedSourcesTemp.Count == _batchSize))
				{
					ConsumeReservedMessagesNonGreedy();
				}
				else
				{
					ReleaseReservedMessages(throwOnFirstException: true);
				}
			}
			reservedSourcesTemp.Clear();
		}

		private void RetrievePostponedItemsGreedyBounded(bool allowFewerThanBatchSize)
		{
			QueuedMap<ISourceBlock<T>, DataflowMessageHeader> postponedMessages = _nonGreedyState.PostponedMessages;
			KeyValuePair<ISourceBlock<T>, DataflowMessageHeader>[] postponedMessagesTemp = _nonGreedyState.PostponedMessagesTemp;
			List<KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>> reservedSourcesTemp = _nonGreedyState.ReservedSourcesTemp;
			reservedSourcesTemp.Clear();
			int num;
			int num2;
			lock (IncomingLock)
			{
				int boundedCapacityAvailable = BoundedCapacityAvailable;
				num = _batchSize - _messages.Count;
				if (_decliningPermanently || postponedMessages.Count == 0 || boundedCapacityAvailable <= 0)
				{
					return;
				}
				if (boundedCapacityAvailable < num)
				{
					num = boundedCapacityAvailable;
				}
				num2 = postponedMessages.PopRange(postponedMessagesTemp, 0, num);
			}
			for (int i = 0; i < num2; i++)
			{
				KeyValuePair<ISourceBlock<T>, DataflowMessageHeader> keyValuePair = postponedMessagesTemp[i];
				KeyValuePair<DataflowMessageHeader, T> value = new KeyValuePair<DataflowMessageHeader, T>(keyValuePair.Value, default(T));
				KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> item = new KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>(keyValuePair.Key, value);
				reservedSourcesTemp.Add(item);
			}
			Array.Clear(postponedMessagesTemp, 0, postponedMessagesTemp.Length);
			while (reservedSourcesTemp.Count < num)
			{
				KeyValuePair<ISourceBlock<T>, DataflowMessageHeader> item2;
				lock (IncomingLock)
				{
					if (!postponedMessages.TryPop(out item2))
					{
						break;
					}
				}
				KeyValuePair<DataflowMessageHeader, T> value2 = new KeyValuePair<DataflowMessageHeader, T>(item2.Value, default(T));
				KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> item3 = new KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>(item2.Key, value2);
				reservedSourcesTemp.Add(item3);
			}
			if (reservedSourcesTemp.Count > 0)
			{
				bool flag = true;
				if (allowFewerThanBatchSize)
				{
					lock (IncomingLock)
					{
						if (!_decliningPermanently && _batchesCompleted + 1 >= _dataflowBlockOptions.ActualMaxNumberOfGroups)
						{
							flag = !_decliningPermanently;
							_decliningPermanently = true;
						}
					}
				}
				if (flag)
				{
					ConsumeReservedMessagesGreedyBounded();
				}
			}
			reservedSourcesTemp.Clear();
		}

		private void ConsumeReservedMessagesNonGreedy()
		{
			List<KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>> reservedSourcesTemp = _nonGreedyState.ReservedSourcesTemp;
			for (int i = 0; i < reservedSourcesTemp.Count; i++)
			{
				KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> keyValuePair = reservedSourcesTemp[i];
				reservedSourcesTemp[i] = default(KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>);
				T value = keyValuePair.Key.ConsumeMessage(keyValuePair.Value.Key, _owningBatch, out var messageConsumed);
				if (!messageConsumed)
				{
					for (int j = 0; j < i; j++)
					{
						reservedSourcesTemp[j] = default(KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>);
					}
					throw new InvalidOperationException(System.SR.InvalidOperation_FailedToConsumeReservedMessage);
				}
				KeyValuePair<DataflowMessageHeader, T> value2 = new KeyValuePair<DataflowMessageHeader, T>(keyValuePair.Value.Key, value);
				KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> value3 = new KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>(keyValuePair.Key, value2);
				reservedSourcesTemp[i] = value3;
			}
			lock (IncomingLock)
			{
				if (_boundingState != null)
				{
					_boundingState.CurrentCount += reservedSourcesTemp.Count;
				}
				foreach (KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> item in reservedSourcesTemp)
				{
					_messages.Enqueue(item.Value.Value);
				}
			}
		}

		private void ConsumeReservedMessagesGreedyBounded()
		{
			int num = 0;
			List<KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>> reservedSourcesTemp = _nonGreedyState.ReservedSourcesTemp;
			for (int i = 0; i < reservedSourcesTemp.Count; i++)
			{
				KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> keyValuePair = reservedSourcesTemp[i];
				reservedSourcesTemp[i] = default(KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>);
				T value = keyValuePair.Key.ConsumeMessage(keyValuePair.Value.Key, _owningBatch, out var messageConsumed);
				if (messageConsumed)
				{
					KeyValuePair<DataflowMessageHeader, T> value2 = new KeyValuePair<DataflowMessageHeader, T>(keyValuePair.Value.Key, value);
					KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> value3 = new KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>(keyValuePair.Key, value2);
					reservedSourcesTemp[i] = value3;
					num++;
				}
			}
			lock (IncomingLock)
			{
				if (_boundingState != null)
				{
					_boundingState.CurrentCount += num;
				}
				foreach (KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> item in reservedSourcesTemp)
				{
					if (item.Key != null)
					{
						_messages.Enqueue(item.Value.Value);
					}
				}
			}
		}

		internal void ReleaseReservedMessages(bool throwOnFirstException)
		{
			List<Exception> list = null;
			List<KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>> reservedSourcesTemp = _nonGreedyState.ReservedSourcesTemp;
			for (int i = 0; i < reservedSourcesTemp.Count; i++)
			{
				KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>> keyValuePair = reservedSourcesTemp[i];
				reservedSourcesTemp[i] = default(KeyValuePair<ISourceBlock<T>, KeyValuePair<DataflowMessageHeader, T>>);
				ISourceBlock<T> key = keyValuePair.Key;
				KeyValuePair<DataflowMessageHeader, T> value = keyValuePair.Value;
				if (key == null || !value.Key.IsValid)
				{
					continue;
				}
				try
				{
					key.ReleaseReservation(value.Key, _owningBatch);
				}
				catch (Exception item)
				{
					if (throwOnFirstException)
					{
						throw;
					}
					if (list == null)
					{
						list = new List<Exception>(1);
					}
					list.Add(item);
				}
			}
			if (list != null)
			{
				throw new AggregateException(list);
			}
		}

		internal void OnItemsRemoved(int numItemsRemoved)
		{
			if (_boundingState != null)
			{
				lock (IncomingLock)
				{
					_boundingState.CurrentCount -= numItemsRemoved;
					ProcessAsyncIfNecessary();
					CompleteBlockIfPossible();
				}
			}
		}

		internal static int CountItems(T[] singleOutputItem, IList<T[]> multipleOutputItems)
		{
			if (multipleOutputItems == null)
			{
				return singleOutputItem.Length;
			}
			int num = 0;
			foreach (T[] multipleOutputItem in multipleOutputItems)
			{
				num += multipleOutputItem.Length;
			}
			return num;
		}

		internal DebuggingInformation GetDebuggingInformation()
		{
			return new DebuggingInformation(this);
		}
	}

	private readonly BatchBlockTargetCore _target;

	private readonly SourceCore<T[]> _source;

	/// <summary>Gets the number of output items available to be received from this block.</summary>
	/// <returns>The number of output items.</returns>
	public int OutputCount => _source.OutputCount;

	/// <summary>Gets a <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation and completion of the dataflow block.</summary>
	/// <returns>The task.</returns>
	public Task Completion => _source.Completion;

	/// <summary>Gets the size of the batches generated by this <see cref="T:System.Threading.Tasks.Dataflow.BatchBlock`1" />.</summary>
	/// <returns>The batch size.</returns>
	public int BatchSize => _target.BatchSize;

	private int OutputCountForDebugger => _source.GetDebuggingInformation().OutputCount;

	private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this, _source.DataflowBlockOptions)}, BatchSize = {BatchSize}, OutputCount = {OutputCountForDebugger}";

	object IDebuggerDisplay.Content => DebuggerDisplayContent;

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.BatchBlock`1" /> with the specified batch size.</summary>
	/// <param name="batchSize">The number of items to group into a batch.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="batchSize" /> must be positive.</exception>
	public BatchBlock(int batchSize)
		: this(batchSize, GroupingDataflowBlockOptions.Default)
	{
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.BatchBlock`1" /> with the specified batch size, declining option, and block options.</summary>
	/// <param name="batchSize">The number of items to group into a batch.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this <see cref="T:System.Threading.Tasks.Dataflow.BatchBlock`1" />.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="batchSize" /> must be positive.-or-The <paramref name="batchSize" /> must be smaller than the value of the <see cref="P:System.Threading.Tasks.Dataflow.DataflowBlockOptions.BoundedCapacity" /> option if a non-default value has been set.</exception>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public BatchBlock(int batchSize, GroupingDataflowBlockOptions dataflowBlockOptions)
	{
		if (batchSize < 1)
		{
			throw new ArgumentOutOfRangeException("batchSize", System.SR.ArgumentOutOfRange_GenericPositive);
		}
		if (dataflowBlockOptions == null)
		{
			throw new ArgumentNullException("dataflowBlockOptions");
		}
		if (dataflowBlockOptions.BoundedCapacity > 0 && dataflowBlockOptions.BoundedCapacity < batchSize)
		{
			throw new ArgumentOutOfRangeException("batchSize", System.SR.ArgumentOutOfRange_BatchSizeMustBeNoGreaterThanBoundedCapacity);
		}
		dataflowBlockOptions = dataflowBlockOptions.DefaultOrClone();
		Action<ISourceBlock<T[]>, int> itemsRemovedAction = null;
		Func<ISourceBlock<T[]>, T[], IList<T[]>, int> itemCountingFunc = null;
		if (dataflowBlockOptions.BoundedCapacity > 0)
		{
			itemsRemovedAction = delegate(ISourceBlock<T[]> owningSource, int count)
			{
				((BatchBlock<T>)owningSource)._target.OnItemsRemoved(count);
			};
			itemCountingFunc = (ISourceBlock<T[]> owningSource, T[] singleOutputItem, IList<T[]> multipleOutputItems) => BatchBlockTargetCore.CountItems(singleOutputItem, multipleOutputItems);
		}
		_source = new SourceCore<T[]>(this, dataflowBlockOptions, delegate(ISourceBlock<T[]> owningSource)
		{
			((BatchBlock<T>)owningSource)._target.Complete(null, dropPendingMessages: true, releaseReservedMessages: false);
		}, itemsRemovedAction, itemCountingFunc);
		_target = new BatchBlockTargetCore(this, batchSize, _source.AddMessage, dataflowBlockOptions);
		_target.Completion.ContinueWith(delegate
		{
			_source.Complete();
		}, CancellationToken.None, Common.GetContinuationOptions(), TaskScheduler.Default);
		_source.Completion.ContinueWith(delegate(Task completed, object state)
		{
			((IDataflowBlock)(BatchBlock<T>)state).Fault((Exception)completed.Exception);
		}, this, CancellationToken.None, Common.GetContinuationOptions() | TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
		Common.WireCancellationToComplete(dataflowBlockOptions.CancellationToken, _source.Completion, delegate(object state, CancellationToken _)
		{
			((BatchBlockTargetCore)state).Complete(null, dropPendingMessages: true, releaseReservedMessages: false);
		}, _target);
		DataflowEtwProvider log = DataflowEtwProvider.Log;
		if (log.IsEnabled())
		{
			log.DataflowBlockCreated(this, dataflowBlockOptions);
		}
	}

	/// <summary>Signals to the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> that it should not accept nor produce any more messages nor consume any more postponed messages.</summary>
	public void Complete()
	{
		_target.Complete(null, dropPendingMessages: false, releaseReservedMessages: false);
	}

	/// <summary>Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.</summary>
	/// <param name="exception">The <see cref="T:System.Exception" /> that caused the faulting.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> is null.</exception>
	void IDataflowBlock.Fault(Exception exception)
	{
		ArgumentNullException.ThrowIfNull(exception, "exception");
		_target.Complete(exception, dropPendingMessages: true, releaseReservedMessages: false);
	}

	/// <summary>Triggers the <see cref="T:System.Threading.Tasks.Dataflow.BatchBlock`1" /> to initiate a batching operation even if the number of currently queued or postponed items is less than the <see cref="P:System.Threading.Tasks.Dataflow.BatchBlock`1.BatchSize" />.</summary>
	public void TriggerBatch()
	{
		_target.TriggerBatch();
	}

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>An IDisposable that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect this source.</param>
	/// <param name="linkOptions">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" /> instance that configures the link.</param>
	/// <exception cref="T:System.ArgumentNullException">
	///   <paramref name="target" /> is null (Nothing in Visual Basic) or <paramref name="linkOptions" /> is null (Nothing in Visual Basic).</exception>
	public IDisposable LinkTo(ITargetBlock<T[]> target, DataflowLinkOptions linkOptions)
	{
		return _source.LinkTo(target, linkOptions);
	}

	/// <summary>Attempts to synchronously receive an available output item from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if an item could be received; otherwise, false.</returns>
	/// <param name="filter">The predicate a value must successfully pass in order for it to be received. <paramref name="filter" /> may be null, in which case all items will pass.</param>
	/// <param name="item">The item received from the source.</param>
	public bool TryReceive(Predicate<T[]>? filter, [NotNullWhen(true)] out T[]? item)
	{
		return _source.TryReceive(filter, out item);
	}

	/// <summary>Attempts to synchronously receive all available items from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if one or more items could be received; otherwise, false.</returns>
	/// <param name="items">The items received from the source.</param>
	public bool TryReceiveAll([NotNullWhen(true)] out IList<T[]>? items)
	{
		return _source.TryReceiveAll(out items);
	}

	DataflowMessageStatus ITargetBlock<T>.OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
	{
		return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
	}

	T[] ISourceBlock<T[]>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T[]> target, out bool messageConsumed)
	{
		return _source.ConsumeMessage(messageHeader, target, out messageConsumed);
	}

	bool ISourceBlock<T[]>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T[]> target)
	{
		return _source.ReserveMessage(messageHeader, target);
	}

	void ISourceBlock<T[]>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T[]> target)
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
