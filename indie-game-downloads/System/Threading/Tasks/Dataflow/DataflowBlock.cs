using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow.Internal;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a set of static (Shared in Visual Basic) methods for working with dataflow blocks.</summary>
public static class DataflowBlock
{
	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	[DebuggerTypeProxy(typeof(FilteredLinkPropagator<>.DebugView))]
	private sealed class FilteredLinkPropagator<T> : IPropagatorBlock<T, T>, ITargetBlock<T>, IDataflowBlock, ISourceBlock<T>, IDebuggerDisplay
	{
		private sealed class DebugView
		{
			private readonly FilteredLinkPropagator<T> _filter;

			public ITargetBlock<T> LinkedTarget => _filter._target;

			public DebugView(FilteredLinkPropagator<T> filter)
			{
				_filter = filter;
			}
		}

		private readonly ISourceBlock<T> _source;

		private readonly ITargetBlock<T> _target;

		private readonly Predicate<T> _userProvidedPredicate;

		Task IDataflowBlock.Completion => _source.Completion;

		private object DebuggerDisplayContent
		{
			get
			{
				IDebuggerDisplay debuggerDisplay = _source as IDebuggerDisplay;
				IDebuggerDisplay debuggerDisplay2 = _target as IDebuggerDisplay;
				return $"{Common.GetNameForDebugger(this)} Source = \"{((debuggerDisplay != null) ? debuggerDisplay.Content : _source)}\", Target = \"{((debuggerDisplay2 != null) ? debuggerDisplay2.Content : _target)}\"";
			}
		}

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		internal FilteredLinkPropagator(ISourceBlock<T> source, ITargetBlock<T> target, Predicate<T> predicate)
		{
			_source = source;
			_target = target;
			_userProvidedPredicate = predicate;
		}

		private bool RunPredicate(T item)
		{
			return _userProvidedPredicate(item);
		}

		DataflowMessageStatus ITargetBlock<T>.OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (RunPredicate(messageValue))
			{
				return _target.OfferMessage(messageHeader, messageValue, this, consumeToAccept);
			}
			return DataflowMessageStatus.Declined;
		}

		T ISourceBlock<T>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
		{
			return _source.ConsumeMessage(messageHeader, this, out messageConsumed);
		}

		bool ISourceBlock<T>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
		{
			return _source.ReserveMessage(messageHeader, this);
		}

		void ISourceBlock<T>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
		{
			_source.ReleaseReservation(messageHeader, this);
		}

		void IDataflowBlock.Complete()
		{
			_target.Complete();
		}

		void IDataflowBlock.Fault(Exception exception)
		{
			_target.Fault(exception);
		}

		IDisposable ISourceBlock<T>.LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
		{
			throw new NotSupportedException(System.SR.NotSupported_MemberNotNeeded);
		}
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	[DebuggerTypeProxy(typeof(SendAsyncSource<>.DebugView))]
	private sealed class SendAsyncSource<TOutput> : TaskCompletionSource<bool>, ISourceBlock<TOutput>, IDataflowBlock, IDebuggerDisplay
	{
		private sealed class DebugView
		{
			private readonly SendAsyncSource<TOutput> _source;

			public ITargetBlock<TOutput> Target => _source._target;

			public TOutput Message => _source._messageValue;

			public Task<bool> Completion => _source.Task;

			public DebugView(SendAsyncSource<TOutput> source)
			{
				_source = source;
			}
		}

		private readonly ITargetBlock<TOutput> _target;

		private readonly TOutput _messageValue;

		private readonly CancellationToken _cancellationToken;

		private readonly CancellationTokenRegistration _cancellationRegistration;

		private int _cancellationState;

		private static readonly Action<object> _cancellationCallback = CancellationHandler;

		Task IDataflowBlock.Completion => base.Task;

		private object DebuggerDisplayContent
		{
			get
			{
				IDebuggerDisplay debuggerDisplay = _target as IDebuggerDisplay;
				return $"{Common.GetNameForDebugger(this)} Message = {_messageValue}, Target = \"{((debuggerDisplay != null) ? debuggerDisplay.Content : _target)}\"";
			}
		}

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		internal SendAsyncSource(ITargetBlock<TOutput> target, TOutput messageValue, CancellationToken cancellationToken)
		{
			_target = target;
			_messageValue = messageValue;
			if (cancellationToken.CanBeCanceled)
			{
				_cancellationToken = cancellationToken;
				_cancellationState = 1;
				try
				{
					_cancellationRegistration = cancellationToken.Register(_cancellationCallback, new WeakReference<SendAsyncSource<TOutput>>(this));
				}
				catch
				{
					GC.SuppressFinalize(this);
					throw;
				}
			}
		}

		~SendAsyncSource()
		{
			if (!Environment.HasShutdownStarted)
			{
				CompleteAsDeclined(runAsync: true);
			}
		}

		private void CompleteAsAccepted(bool runAsync)
		{
			RunCompletionAction(delegate(object state)
			{
				try
				{
					((SendAsyncSource<TOutput>)state).TrySetResult(result: true);
				}
				catch (ObjectDisposedException)
				{
				}
			}, this, runAsync);
		}

		private void CompleteAsDeclined(bool runAsync)
		{
			RunCompletionAction(delegate(object state)
			{
				try
				{
					((SendAsyncSource<TOutput>)state).TrySetResult(result: false);
				}
				catch (ObjectDisposedException)
				{
				}
			}, this, runAsync);
		}

		private void CompleteAsFaulted(Exception exception, bool runAsync)
		{
			RunCompletionAction(delegate(object state)
			{
				Tuple<SendAsyncSource<TOutput>, Exception> tuple = (Tuple<SendAsyncSource<TOutput>, Exception>)state;
				try
				{
					tuple.Item1.TrySetException(tuple.Item2);
				}
				catch (ObjectDisposedException)
				{
				}
			}, Tuple.Create(this, exception), runAsync);
		}

		private void CompleteAsCanceled(bool runAsync)
		{
			RunCompletionAction(delegate(object state)
			{
				SendAsyncSource<TOutput> sendAsyncSource = (SendAsyncSource<TOutput>)state;
				try
				{
					sendAsyncSource.TrySetCanceled(sendAsyncSource._cancellationToken);
				}
				catch (ObjectDisposedException)
				{
				}
			}, this, runAsync);
		}

		private void RunCompletionAction(Action<object> completionAction, object completionActionState, bool runAsync)
		{
			GC.SuppressFinalize(this);
			if (_cancellationState != 0)
			{
				_cancellationRegistration.Dispose();
			}
			if (runAsync)
			{
				System.Threading.Tasks.Task.Factory.StartNew(completionAction, completionActionState, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}
			else
			{
				completionAction(completionActionState);
			}
		}

		private void OfferToTargetAsync()
		{
			System.Threading.Tasks.Task.Factory.StartNew(delegate(object state)
			{
				((SendAsyncSource<TOutput>)state).OfferToTarget();
			}, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		private static void CancellationHandler(object state)
		{
			SendAsyncSource<TOutput> sendAsyncSource = Common.UnwrapWeakReference<SendAsyncSource<TOutput>>(state);
			if (sendAsyncSource != null && sendAsyncSource._cancellationState == 1 && Interlocked.CompareExchange(ref sendAsyncSource._cancellationState, 3, 1) == 1)
			{
				sendAsyncSource.CompleteAsCanceled(runAsync: true);
			}
		}

		internal void OfferToTarget()
		{
			try
			{
				bool flag = _cancellationState != 0;
				switch (_target.OfferMessage(Common.SingleMessageHeader, _messageValue, this, flag))
				{
				case DataflowMessageStatus.Accepted:
					if (!flag)
					{
						CompleteAsAccepted(runAsync: false);
					}
					break;
				case DataflowMessageStatus.Declined:
				case DataflowMessageStatus.DecliningPermanently:
					CompleteAsDeclined(runAsync: false);
					break;
				case DataflowMessageStatus.Postponed:
				case DataflowMessageStatus.NotAvailable:
					break;
				}
			}
			catch (Exception ex)
			{
				Common.StoreDataflowMessageValueIntoExceptionData(ex, _messageValue);
				CompleteAsFaulted(ex, runAsync: false);
			}
		}

		TOutput ISourceBlock<TOutput>.ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (base.Task.IsCompleted)
			{
				messageConsumed = false;
				return default(TOutput);
			}
			if (messageHeader.Id == 1)
			{
				int cancellationState = _cancellationState;
				if (cancellationState == 0 || (cancellationState != 3 && Interlocked.CompareExchange(ref _cancellationState, 3, cancellationState) == cancellationState))
				{
					CompleteAsAccepted(runAsync: true);
					messageConsumed = true;
					return _messageValue;
				}
			}
			messageConsumed = false;
			return default(TOutput);
		}

		bool ISourceBlock<TOutput>.ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (base.Task.IsCompleted)
			{
				return false;
			}
			if (messageHeader.Id == 1)
			{
				if (_cancellationState != 0)
				{
					return Interlocked.CompareExchange(ref _cancellationState, 2, 1) == 1;
				}
				return true;
			}
			return false;
		}

		void ISourceBlock<TOutput>.ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (messageHeader.Id != 1)
			{
				throw new InvalidOperationException(System.SR.InvalidOperation_MessageNotReservedByTarget);
			}
			if (base.Task.IsCompleted)
			{
				return;
			}
			if (_cancellationState != 0)
			{
				if (Interlocked.CompareExchange(ref _cancellationState, 1, 2) != 2)
				{
					throw new InvalidOperationException(System.SR.InvalidOperation_MessageNotReservedByTarget);
				}
				if (_cancellationToken.IsCancellationRequested)
				{
					CancellationHandler(new WeakReference<SendAsyncSource<TOutput>>(this));
				}
			}
			OfferToTargetAsync();
		}

		IDisposable ISourceBlock<TOutput>.LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
		{
			throw new NotSupportedException(System.SR.NotSupported_MemberNotNeeded);
		}

		void IDataflowBlock.Complete()
		{
			throw new NotSupportedException(System.SR.NotSupported_MemberNotNeeded);
		}

		void IDataflowBlock.Fault(Exception exception)
		{
			throw new NotSupportedException(System.SR.NotSupported_MemberNotNeeded);
		}
	}

	private enum ReceiveCoreByLinkingCleanupReason
	{
		Success,
		Timer,
		Cancellation,
		SourceCompletion,
		SourceProtocolError,
		ErrorDuringCleanup
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	private sealed class ReceiveTarget<T> : TaskCompletionSource<T>, ITargetBlock<T>, IDataflowBlock, IDebuggerDisplay
	{
		internal static readonly TimerCallback CachedLinkingTimerCallback = delegate(object state)
		{
			((ReceiveTarget<T>)state).TryCleanupAndComplete(ReceiveCoreByLinkingCleanupReason.Timer);
		};

		internal static readonly Action<object> CachedLinkingCancellationCallback = delegate(object state)
		{
			((ReceiveTarget<T>)state).TryCleanupAndComplete(ReceiveCoreByLinkingCleanupReason.Cancellation);
		};

		private T _receivedValue;

		internal readonly CancellationTokenSource _cts = new CancellationTokenSource();

		internal bool _cleanupReserved;

		internal CancellationToken _externalCancellationToken;

		internal CancellationTokenRegistration _regFromExternalCancellationToken;

		internal Timer _timer;

		internal IDisposable _unlink;

		internal Exception _receivedException;

		internal object IncomingLock => _cts;

		Task IDataflowBlock.Completion
		{
			get
			{
				throw new NotSupportedException(System.SR.NotSupported_MemberNotNeeded);
			}
		}

		private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this)} IsCompleted = {base.Task.IsCompleted}";

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		internal ReceiveTarget()
		{
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
			DataflowMessageStatus dataflowMessageStatus = DataflowMessageStatus.NotAvailable;
			if (Volatile.Read(in _cleanupReserved))
			{
				return DataflowMessageStatus.DecliningPermanently;
			}
			lock (IncomingLock)
			{
				if (_cleanupReserved)
				{
					return DataflowMessageStatus.DecliningPermanently;
				}
				try
				{
					bool messageConsumed = true;
					T receivedValue = (T)(consumeToAccept ? ((object)source.ConsumeMessage(messageHeader, this, out messageConsumed)) : ((object)messageValue));
					if (messageConsumed)
					{
						dataflowMessageStatus = DataflowMessageStatus.Accepted;
						_receivedValue = receivedValue;
						_cleanupReserved = true;
					}
				}
				catch (Exception ex)
				{
					dataflowMessageStatus = DataflowMessageStatus.DecliningPermanently;
					Common.StoreDataflowMessageValueIntoExceptionData(ex, messageValue);
					_receivedException = ex;
					_cleanupReserved = true;
				}
			}
			switch (dataflowMessageStatus)
			{
			case DataflowMessageStatus.Accepted:
				CleanupAndComplete(ReceiveCoreByLinkingCleanupReason.Success);
				break;
			case DataflowMessageStatus.DecliningPermanently:
				CleanupAndComplete(ReceiveCoreByLinkingCleanupReason.SourceProtocolError);
				break;
			}
			return dataflowMessageStatus;
		}

		internal bool TryCleanupAndComplete(ReceiveCoreByLinkingCleanupReason reason)
		{
			if (Volatile.Read(in _cleanupReserved))
			{
				return false;
			}
			lock (IncomingLock)
			{
				if (_cleanupReserved)
				{
					return false;
				}
				_cleanupReserved = true;
			}
			CleanupAndComplete(reason);
			return true;
		}

		private void CleanupAndComplete(ReceiveCoreByLinkingCleanupReason reason)
		{
			IDisposable unlink = _unlink;
			if (reason != ReceiveCoreByLinkingCleanupReason.SourceCompletion && unlink != null)
			{
				IDisposable disposable = Interlocked.CompareExchange(ref _unlink, null, unlink);
				if (disposable != null)
				{
					try
					{
						disposable.Dispose();
					}
					catch (Exception receivedException)
					{
						_receivedException = receivedException;
						reason = ReceiveCoreByLinkingCleanupReason.SourceProtocolError;
					}
				}
			}
			_timer?.Dispose();
			if (reason != ReceiveCoreByLinkingCleanupReason.Cancellation)
			{
				if (reason == ReceiveCoreByLinkingCleanupReason.SourceCompletion && (_externalCancellationToken.IsCancellationRequested || _cts.IsCancellationRequested))
				{
					reason = ReceiveCoreByLinkingCleanupReason.Cancellation;
				}
				_cts.Cancel();
			}
			_regFromExternalCancellationToken.Dispose();
			switch (reason)
			{
			case ReceiveCoreByLinkingCleanupReason.Success:
				System.Threading.Tasks.Task.Factory.StartNew(delegate(object state)
				{
					ReceiveTarget<T> receiveTarget = (ReceiveTarget<T>)state;
					try
					{
						receiveTarget.TrySetResult(receiveTarget._receivedValue);
					}
					catch (ObjectDisposedException)
					{
					}
				}, this, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
				return;
			default:
				System.Threading.Tasks.Task.Factory.StartNew(delegate(object state)
				{
					ReceiveTarget<T> receiveTarget = (ReceiveTarget<T>)state;
					try
					{
						receiveTarget.TrySetCanceled();
					}
					catch (ObjectDisposedException)
					{
					}
				}, this, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
				return;
			case ReceiveCoreByLinkingCleanupReason.SourceCompletion:
				if (_receivedException == null)
				{
					_receivedException = CreateExceptionForSourceCompletion();
				}
				break;
			case ReceiveCoreByLinkingCleanupReason.Timer:
				if (_receivedException == null)
				{
					_receivedException = CreateExceptionForTimeout();
				}
				break;
			case ReceiveCoreByLinkingCleanupReason.SourceProtocolError:
			case ReceiveCoreByLinkingCleanupReason.ErrorDuringCleanup:
				break;
			}
			System.Threading.Tasks.Task.Factory.StartNew(delegate(object state)
			{
				ReceiveTarget<T> receiveTarget = (ReceiveTarget<T>)state;
				try
				{
					receiveTarget.TrySetException(receiveTarget._receivedException ?? new InvalidOperationException(System.SR.InvalidOperation_ErrorDuringCleanup));
				}
				catch (ObjectDisposedException)
				{
				}
			}, this, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
		}

		internal static Exception CreateExceptionForSourceCompletion()
		{
			return Common.InitializeStackTrace(new InvalidOperationException(System.SR.InvalidOperation_DataNotAvailableForReceive));
		}

		internal static Exception CreateExceptionForTimeout()
		{
			return Common.InitializeStackTrace(new TimeoutException());
		}

		void IDataflowBlock.Complete()
		{
			TryCleanupAndComplete(ReceiveCoreByLinkingCleanupReason.SourceCompletion);
		}

		void IDataflowBlock.Fault(Exception exception)
		{
			((IDataflowBlock)this).Complete();
		}
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	private sealed class OutputAvailableAsyncTarget<T> : TaskCompletionSource<bool>, ITargetBlock<T>, IDataflowBlock, IDebuggerDisplay
	{
		Task IDataflowBlock.Completion
		{
			get
			{
				throw new NotSupportedException(System.SR.NotSupported_MemberNotNeeded);
			}
		}

		private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this)} IsCompleted = {base.Task.IsCompleted}";

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		public OutputAvailableAsyncTarget()
			: base(TaskCreationOptions.RunContinuationsAsynchronously)
		{
		}

		DataflowMessageStatus ITargetBlock<T>.OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			TrySetResult(result: true);
			return DataflowMessageStatus.DecliningPermanently;
		}

		void IDataflowBlock.Complete()
		{
			TrySetResult(result: false);
		}

		void IDataflowBlock.Fault(Exception exception)
		{
			ArgumentNullException.ThrowIfNull(exception, "exception");
			TrySetResult(result: false);
		}
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	[DebuggerTypeProxy(typeof(EncapsulatingPropagator<, >.DebugView))]
	private sealed class EncapsulatingPropagator<TInput, TOutput> : IPropagatorBlock<TInput, TOutput>, ITargetBlock<TInput>, IDataflowBlock, ISourceBlock<TOutput>, IReceivableSourceBlock<TOutput>, IDebuggerDisplay
	{
		private sealed class DebugView
		{
			private readonly EncapsulatingPropagator<TInput, TOutput> _propagator;

			public ITargetBlock<TInput> Target => _propagator._target;

			public ISourceBlock<TOutput> Source => _propagator._source;

			public DebugView(EncapsulatingPropagator<TInput, TOutput> propagator)
			{
				_propagator = propagator;
			}
		}

		private readonly ITargetBlock<TInput> _target;

		private readonly ISourceBlock<TOutput> _source;

		public Task Completion => _source.Completion;

		private object DebuggerDisplayContent
		{
			get
			{
				IDebuggerDisplay debuggerDisplay = _target as IDebuggerDisplay;
				IDebuggerDisplay debuggerDisplay2 = _source as IDebuggerDisplay;
				return $"{Common.GetNameForDebugger(this)} Target = \"{((debuggerDisplay != null) ? debuggerDisplay.Content : _target)}\", Source = \"{((debuggerDisplay2 != null) ? debuggerDisplay2.Content : _source)}\"";
			}
		}

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		public EncapsulatingPropagator(ITargetBlock<TInput> target, ISourceBlock<TOutput> source)
		{
			_target = target;
			_source = source;
		}

		public void Complete()
		{
			_target.Complete();
		}

		void IDataflowBlock.Fault(Exception exception)
		{
			ArgumentNullException.ThrowIfNull(exception, "exception");
			_target.Fault(exception);
		}

		public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
		{
			return _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
		}

		public IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
		{
			return _source.LinkTo(target, linkOptions);
		}

		public bool TryReceive(Predicate<TOutput> filter, [MaybeNullWhen(false)] out TOutput item)
		{
			if (_source is IReceivableSourceBlock<TOutput> receivableSourceBlock)
			{
				return receivableSourceBlock.TryReceive(filter, out item);
			}
			item = default(TOutput);
			return false;
		}

		public bool TryReceiveAll([NotNullWhen(true)] out IList<TOutput> items)
		{
			if (_source is IReceivableSourceBlock<TOutput> receivableSourceBlock)
			{
				return receivableSourceBlock.TryReceiveAll(out items);
			}
			items = null;
			return false;
		}

		public TOutput ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)
		{
			return _source.ConsumeMessage(messageHeader, target, out messageConsumed);
		}

		public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
		{
			return _source.ReserveMessage(messageHeader, target);
		}

		public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
		{
			_source.ReleaseReservation(messageHeader, target);
		}
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	private sealed class ChooseTarget<T> : TaskCompletionSource<T>, ITargetBlock<T>, IDataflowBlock, IDebuggerDisplay
	{
		internal static readonly Func<object, int> s_processBranchFunction = delegate(object state)
		{
			Tuple<Action<T>, T, int> tuple = (Tuple<Action<T>, T, int>)state;
			tuple.Item1(tuple.Item2);
			return tuple.Item3;
		};

		private readonly StrongBox<Task> _completed;

		Task IDataflowBlock.Completion
		{
			get
			{
				throw new NotSupportedException(System.SR.NotSupported_MemberNotNeeded);
			}
		}

		private object DebuggerDisplayContent => $"{Common.GetNameForDebugger(this)} IsCompleted = {base.Task.IsCompleted}";

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		internal ChooseTarget(StrongBox<Task> completed, CancellationToken cancellationToken)
		{
			_completed = completed;
			Common.WireCancellationToComplete(cancellationToken, base.Task, delegate(object state, CancellationToken cancellationToken2)
			{
				ChooseTarget<T> chooseTarget = (ChooseTarget<T>)state;
				lock (chooseTarget._completed)
				{
					chooseTarget.TrySetCanceled(cancellationToken2);
				}
			}, this);
		}

		public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if ((source == null) & consumeToAccept)
			{
				throw new ArgumentException(System.SR.Argument_CantConsumeFromANullSource, "consumeToAccept");
			}
			lock (_completed)
			{
				if (_completed.Value != null || base.Task.IsCompleted)
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
				TrySetResult(messageValue);
				_completed.Value = base.Task;
				return DataflowMessageStatus.Accepted;
			}
		}

		void IDataflowBlock.Complete()
		{
			lock (_completed)
			{
				TrySetCanceled();
			}
		}

		void IDataflowBlock.Fault(Exception exception)
		{
			((IDataflowBlock)this).Complete();
		}
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	[DebuggerTypeProxy(typeof(SourceObservable<>.DebugView))]
	private sealed class SourceObservable<TOutput> : IObservable<TOutput>, IDebuggerDisplay
	{
		private sealed class DebugView
		{
			private readonly SourceObservable<TOutput> _observable;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public IObserver<TOutput>[] Observers => _observable._observersState.Observers.ToArray();

			public DebugView(SourceObservable<TOutput> observable)
			{
				_observable = observable;
			}
		}

		private sealed class ObserversState
		{
			internal readonly SourceObservable<TOutput> Observable;

			internal readonly ActionBlock<TOutput> Target;

			internal readonly CancellationTokenSource Canceler = new CancellationTokenSource();

			internal ImmutableArray<IObserver<TOutput>> Observers = ImmutableArray<IObserver<TOutput>>.Empty;

			internal IDisposable Unlinker;

			private List<Task<bool>> _tempSendAsyncTaskList;

			internal ObserversState(SourceObservable<TOutput> observable)
			{
				Observable = observable;
				Target = new ActionBlock<TOutput>((Func<TOutput, Task>)ProcessItemAsync, _nonGreedyExecutionOptions);
				Target.Completion.ContinueWith(delegate(Task t, object state)
				{
					((ObserversState)state).NotifyObserversOfCompletion(t.Exception);
				}, this, CancellationToken.None, Common.GetContinuationOptions(TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously), TaskScheduler.Default);
				Common.GetPotentiallyNotSupportedCompletionTask(Observable._source)?.ContinueWith(delegate(Task _1, object state1)
				{
					ObserversState obj = (ObserversState)state1;
					obj.Target.Complete();
					obj.Target.Completion.ContinueWith(delegate(Task task, object obj2)
					{
						((ObserversState)obj2).NotifyObserversOfCompletion();
					}, state1, CancellationToken.None, Common.GetContinuationOptions(TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously), TaskScheduler.Default);
				}, this, Canceler.Token, Common.GetContinuationOptions(TaskContinuationOptions.ExecuteSynchronously), TaskScheduler.Default);
			}

			private Task ProcessItemAsync(TOutput item)
			{
				ImmutableArray<IObserver<TOutput>> observers;
				lock (Observable._SubscriptionLock)
				{
					observers = Observers;
				}
				try
				{
					foreach (IObserver<TOutput> item2 in observers)
					{
						if (item2 is TargetObserver<TOutput> targetObserver)
						{
							Task<bool> task = targetObserver.SendAsyncToTarget(item);
							if (task.Status != TaskStatus.RanToCompletion)
							{
								if (_tempSendAsyncTaskList == null)
								{
									_tempSendAsyncTaskList = new List<Task<bool>>();
								}
								_tempSendAsyncTaskList.Add(task);
							}
						}
						else
						{
							item2.OnNext(item);
						}
					}
					if (_tempSendAsyncTaskList != null && _tempSendAsyncTaskList.Count > 0)
					{
						Task<bool[]> result = Task.WhenAll(_tempSendAsyncTaskList);
						_tempSendAsyncTaskList.Clear();
						return result;
					}
				}
				catch (Exception exception)
				{
					return Common.CreateTaskFromException<VoidResult>(exception);
				}
				return Common.CompletedTaskWithTrueResult;
			}

			private void NotifyObserversOfCompletion(Exception targetException = null)
			{
				ImmutableArray<IObserver<TOutput>> observers;
				lock (Observable._SubscriptionLock)
				{
					observers = Observers;
					if (targetException != null)
					{
						Observable.ResetObserverState();
					}
					Observers = ImmutableArray<IObserver<TOutput>>.Empty;
				}
				if (observers.Count <= 0)
				{
					return;
				}
				Exception ex = targetException ?? Observable.GetCompletionError();
				try
				{
					if (ex != null)
					{
						foreach (IObserver<TOutput> item in observers)
						{
							item.OnError(ex);
						}
						return;
					}
					foreach (IObserver<TOutput> item2 in observers)
					{
						item2.OnCompleted();
					}
				}
				catch (Exception error)
				{
					Common.ThrowAsync(error);
				}
			}
		}

		private static readonly ConditionalWeakTable<ISourceBlock<TOutput>, SourceObservable<TOutput>> _table = new ConditionalWeakTable<ISourceBlock<TOutput>, SourceObservable<TOutput>>();

		private readonly object _SubscriptionLock = new object();

		private readonly ISourceBlock<TOutput> _source;

		private ObserversState _observersState;

		private object DebuggerDisplayContent
		{
			get
			{
				IDebuggerDisplay debuggerDisplay = _source as IDebuggerDisplay;
				return $"Observers = {_observersState.Observers.Count}, Block = \"{((debuggerDisplay != null) ? debuggerDisplay.Content : _source)}\"";
			}
		}

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		internal static SourceObservable<TOutput> From(ISourceBlock<TOutput> source)
		{
			return _table.GetValue(source, (ISourceBlock<TOutput> s) => new SourceObservable<TOutput>(s));
		}

		internal SourceObservable(ISourceBlock<TOutput> source)
		{
			_source = source;
			_observersState = new ObserversState(this);
		}

		private AggregateException GetCompletionError()
		{
			Task potentiallyNotSupportedCompletionTask = Common.GetPotentiallyNotSupportedCompletionTask(_source);
			if (potentiallyNotSupportedCompletionTask == null || !potentiallyNotSupportedCompletionTask.IsFaulted)
			{
				return null;
			}
			return potentiallyNotSupportedCompletionTask.Exception;
		}

		IDisposable IObservable<TOutput>.Subscribe(IObserver<TOutput> observer)
		{
			ArgumentNullException.ThrowIfNull(observer, "observer");
			Task potentiallyNotSupportedCompletionTask = Common.GetPotentiallyNotSupportedCompletionTask(_source);
			Exception ex = null;
			lock (_SubscriptionLock)
			{
				if (potentiallyNotSupportedCompletionTask == null || !potentiallyNotSupportedCompletionTask.IsCompleted || !_observersState.Target.Completion.IsCompleted)
				{
					_observersState.Observers = _observersState.Observers.Add(observer);
					if (_observersState.Observers.Count == 1)
					{
						_observersState.Unlinker = _source.LinkTo(_observersState.Target);
						if (_observersState.Unlinker == null)
						{
							_observersState.Observers = ImmutableArray<IObserver<TOutput>>.Empty;
							return Disposables.Nop;
						}
					}
					return Disposables.Create(delegate(SourceObservable<TOutput> s, IObserver<TOutput> o)
					{
						s.Unsubscribe(o);
					}, this, observer);
				}
				ex = GetCompletionError();
			}
			if (ex != null)
			{
				observer.OnError(ex);
			}
			else
			{
				observer.OnCompleted();
			}
			return Disposables.Nop;
		}

		private void Unsubscribe(IObserver<TOutput> observer)
		{
			lock (_SubscriptionLock)
			{
				ObserversState observersState = _observersState;
				if (observersState.Observers.Contains(observer))
				{
					if (observersState.Observers.Count == 1)
					{
						ResetObserverState();
					}
					else
					{
						observersState.Observers = observersState.Observers.Remove(observer);
					}
				}
			}
		}

		private ImmutableArray<IObserver<TOutput>> ResetObserverState()
		{
			ObserversState observersState = _observersState;
			ImmutableArray<IObserver<TOutput>> observers = observersState.Observers;
			_observersState = new ObserversState(this);
			observersState.Unlinker.Dispose();
			observersState.Canceler.Cancel();
			return observers;
		}
	}

	[DebuggerDisplay("{DebuggerDisplayContent,nq}")]
	private sealed class TargetObserver<TInput> : IObserver<TInput>, IDebuggerDisplay
	{
		private readonly ITargetBlock<TInput> _target;

		private object DebuggerDisplayContent
		{
			get
			{
				IDebuggerDisplay debuggerDisplay = _target as IDebuggerDisplay;
				return $"Block = \"{((debuggerDisplay != null) ? debuggerDisplay.Content : _target)}\"";
			}
		}

		object IDebuggerDisplay.Content => DebuggerDisplayContent;

		internal TargetObserver(ITargetBlock<TInput> target)
		{
			_target = target;
		}

		void IObserver<TInput>.OnNext(TInput value)
		{
			SendAsyncToTarget(value).GetAwaiter().GetResult();
		}

		void IObserver<TInput>.OnCompleted()
		{
			_target.Complete();
		}

		void IObserver<TInput>.OnError(Exception error)
		{
			_target.Fault(error);
		}

		internal Task<bool> SendAsyncToTarget(TInput value)
		{
			return _target.SendAsync(value);
		}
	}

	private sealed class NullTargetBlock<TInput> : ITargetBlock<TInput>, IDataflowBlock
	{
		private Task _completion;

		Task IDataflowBlock.Completion => LazyInitializer.EnsureInitialized(ref _completion, () => new TaskCompletionSource<VoidResult>().Task);

		DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
		{
			if (!messageHeader.IsValid)
			{
				throw new ArgumentException(System.SR.Argument_InvalidMessageHeader, "messageHeader");
			}
			if (consumeToAccept)
			{
				if (source == null)
				{
					throw new ArgumentException(System.SR.Argument_CantConsumeFromANullSource, "consumeToAccept");
				}
				source.ConsumeMessage(messageHeader, this, out var messageConsumed);
				if (!messageConsumed)
				{
					return DataflowMessageStatus.NotAvailable;
				}
			}
			return DataflowMessageStatus.Accepted;
		}

		void IDataflowBlock.Complete()
		{
		}

		void IDataflowBlock.Fault(Exception exception)
		{
		}
	}

	private static readonly Action<object> _cancelCts = delegate(object state)
	{
		((CancellationTokenSource)state).Cancel();
	};

	private static readonly ExecutionDataflowBlockOptions _nonGreedyExecutionOptions = new ExecutionDataflowBlockOptions
	{
		BoundedCapacity = 1
	};

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>An <see cref="T:System.IDisposable" /> that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="source">The source from which to link.</param>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.-or-The <paramref name="target" /> is null.</exception>
	public static IDisposable LinkTo<TOutput>(this ISourceBlock<TOutput> source, ITargetBlock<TOutput> target)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(target, "target");
		return source.LinkTo(target, DataflowLinkOptions.Default);
	}

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> using the specified filter.</summary>
	/// <returns>An <see cref="T:System.IDisposable" /> that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="source">The source from which to link.</param>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
	/// <param name="predicate">The filter a message must pass in order for it to propagate from the source to the target.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.-or-The <paramref name="target" /> is null.-or-The <paramref name="predicate" /> is null.</exception>
	public static IDisposable LinkTo<TOutput>(this ISourceBlock<TOutput> source, ITargetBlock<TOutput> target, Predicate<TOutput> predicate)
	{
		return source.LinkTo(target, DataflowLinkOptions.Default, predicate);
	}

	/// <summary>Links the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" /> to the specified <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> using the specified filter.</summary>
	/// <returns>An <see cref="T:System.IDisposable" /> that, upon calling Dispose, will unlink the source from the target.</returns>
	/// <param name="source">The source from which to link.</param>
	/// <param name="target">The <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> to which to connect the source.</param>
	/// <param name="linkOptions">One of the enumeration values that specifies how to configure a link between dataflow blocks.</param>
	/// <param name="predicate">The filter a message must pass in order for it to propagate from the source to the target.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null (Nothing in Visual Basic).-or-The <paramref name="target" /> is null (Nothing in Visual Basic).-or-The <paramref name="linkOptions" /> is null (Nothing in Visual Basic).-or-The <paramref name="predicate" /> is null (Nothing in Visual Basic).</exception>
	public static IDisposable LinkTo<TOutput>(this ISourceBlock<TOutput> source, ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions, Predicate<TOutput> predicate)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(target, "target");
		ArgumentNullException.ThrowIfNull(linkOptions, "linkOptions");
		ArgumentNullException.ThrowIfNull(predicate, "predicate");
		FilteredLinkPropagator<TOutput> target2 = new FilteredLinkPropagator<TOutput>(source, target, predicate);
		return source.LinkTo(target2, linkOptions);
	}

	/// <summary>Posts an item to the <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>true if the item was accepted by the target block; otherwise, false.</returns>
	/// <param name="target">The target block.</param>
	/// <param name="item">The item being offered to the target.</param>
	/// <typeparam name="TInput">Specifies the type of data accepted by the target block.</typeparam>
	public static bool Post<TInput>(this ITargetBlock<TInput> target, TInput item)
	{
		ArgumentNullException.ThrowIfNull(target, "target");
		return target.OfferMessage(Common.SingleMessageHeader, item, null, consumeToAccept: false) == DataflowMessageStatus.Accepted;
	}

	/// <summary>Asynchronously offers a message to the target message block, allowing for postponement.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous send. If the target accepts and consumes the offered element during the call to <see cref="M:System.Threading.Tasks.Dataflow.DataflowBlock.SendAsync``1(System.Threading.Tasks.Dataflow.ITargetBlock{``0},``0)" />, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task`1" /> will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will return true. If the target declines the offered element during the call, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task`1" /> will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> property will return false. If the target postpones the offered element, the element will be buffered until such time that the target consumes or releases it, at which point the task will complete, with its <see cref="P:System.Threading.Tasks.Task`1.Result" /> indicating whether the message was consumed. If the target never attempts to consume or release the message, the returned task will never complete.</returns>
	/// <param name="target">The target to which to post the data.</param>
	/// <param name="item">The item being offered to the target.</param>
	/// <typeparam name="TInput">Specifies the type of the data to post to the target.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="target" /> is null.</exception>
	public static Task<bool> SendAsync<TInput>(this ITargetBlock<TInput> target, TInput item)
	{
		return target.SendAsync(item, CancellationToken.None);
	}

	/// <summary>Asynchronously offers a message to the target message block, allowing for postponement.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task{Boolean}" /> that represents the asynchronous send.  If the target accepts and consumes the offered element during the call to SendAsync, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task{Boolean}" /> will be completed and its Result property will return true.  If the target declines the offered element during the call, upon return from the call the resulting <see cref="T:System.Threading.Tasks.Task{Boolean}" /> will be completed and its Result property will return false. If the target postpones the offered element, the element will be buffered until such time that the target consumes or releases it, at which point the Task will complete, with its Result indicating whether the message was consumed. If the target never attempts to consume or release the message, the returned task will never complete.If cancellation is requested before the target has successfully consumed the sent data, the returned task will complete in the Canceled state and the data will no longer be available to the target.</returns>
	/// <param name="target">The target to which to post the data.</param>
	/// <param name="item">The item being offered to the target.</param>
	/// <param name="cancellationToken">The cancellation token with which to request cancellation of the send operation.</param>
	/// <typeparam name="TInput">Specifies the type of the data to post to the target.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="target" /> is null (Nothing in Visual Basic).</exception>
	public static Task<bool> SendAsync<TInput>(this ITargetBlock<TInput> target, TInput item, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(target, "target");
		if (cancellationToken.IsCancellationRequested)
		{
			return Common.CreateTaskFromCancellation<bool>(cancellationToken);
		}
		SendAsyncSource<TInput> sendAsyncSource;
		try
		{
			switch (target.OfferMessage(Common.SingleMessageHeader, item, null, consumeToAccept: false))
			{
			case DataflowMessageStatus.Accepted:
				return Common.CompletedTaskWithTrueResult;
			case DataflowMessageStatus.DecliningPermanently:
				return Common.CompletedTaskWithFalseResult;
			default:
				sendAsyncSource = new SendAsyncSource<TInput>(target, item, cancellationToken);
				break;
			}
		}
		catch (Exception ex)
		{
			Common.StoreDataflowMessageValueIntoExceptionData(ex, item);
			return Common.CreateTaskFromException<bool>(ex);
		}
		sendAsyncSource.OfferToTarget();
		return sendAsyncSource.Task;
	}

	/// <summary>Attempts to synchronously receive an item from the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" />.</summary>
	/// <returns>true if an item could be received; otherwise, false.</returns>
	/// <param name="source">The source from which to receive.</param>
	/// <param name="item">The item received from the source.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	public static bool TryReceive<TOutput>(this IReceivableSourceBlock<TOutput> source, [MaybeNullWhen(false)] out TOutput item)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return source.TryReceive(null, out item);
	}

	/// <summary>Asynchronously receives a value from the specified source.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous receive operation. When an item is successfully received from the source, the returned task will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> will return the received item. If an item cannot be retrieved, because the source is empty and completed, the returned task will be canceled.</returns>
	/// <param name="source">The source from which to asynchronously receive.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	public static Task<TOutput> ReceiveAsync<TOutput>(this ISourceBlock<TOutput> source)
	{
		return source.ReceiveAsync(Common.InfiniteTimeSpan, CancellationToken.None);
	}

	/// <summary>Asynchronously receives a value from the specified source.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous receive operation. When an item is successfully received from the source, the returned task will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" />will return the received item. If an item cannot be retrieved, either because cancellation is requested or the source is empty and completed, the returned task will be canceled.</returns>
	/// <param name="source">The source from which to asynchronously receive.</param>
	/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> which may be used to cancel the receive operation.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	public static Task<TOutput> ReceiveAsync<TOutput>(this ISourceBlock<TOutput> source, CancellationToken cancellationToken)
	{
		return source.ReceiveAsync(Common.InfiniteTimeSpan, cancellationToken);
	}

	/// <summary>Asynchronously receives a value from the specified source.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous receive operation. When an item is successfully received from the source, the returned task will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> will return the received item. If an item cannot be retrieved, either because the timeout expires or the source is empty and completed, the returned task will be canceled.</returns>
	/// <param name="source">The source from which to asynchronously receive.</param>
	/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out-or-<paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static Task<TOutput> ReceiveAsync<TOutput>(this ISourceBlock<TOutput> source, TimeSpan timeout)
	{
		return source.ReceiveAsync(timeout, CancellationToken.None);
	}

	/// <summary>Asynchronously receives a value from the specified source.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous receive operation. When an item is successfully received from the source, the returned task will be completed and its <see cref="P:System.Threading.Tasks.Task`1.Result" /> will return the received item. If an item cannot be retrieved, either because the timeout expires, cancellation is requested, or the source is empty and completed, the returned task will be canceled.</returns>
	/// <param name="source">The source from which to asynchronously receive.</param>
	/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
	/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> which may be used to cancel the receive operation.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out-or-<paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
	public static Task<TOutput> ReceiveAsync<TOutput>(this ISourceBlock<TOutput> source, TimeSpan timeout, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!Common.IsValidTimeout(timeout))
		{
			throw new ArgumentOutOfRangeException("timeout", System.SR.ArgumentOutOfRange_NeedNonNegOrNegative1);
		}
		return source.ReceiveCore(attemptTryReceive: true, timeout, cancellationToken);
	}

	/// <summary>Synchronously receives an item from the source.</summary>
	/// <returns>The received item.</returns>
	/// <param name="source">The source from which to receive.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">No item could be received from the source.</exception>
	public static TOutput Receive<TOutput>(this ISourceBlock<TOutput> source)
	{
		return source.Receive(Common.InfiniteTimeSpan, CancellationToken.None);
	}

	/// <summary>Synchronously receives an item from the source.</summary>
	/// <returns>The received item.</returns>
	/// <param name="source">The source from which to receive.</param>
	/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> which may be used to cancel the receive operation.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">No item could be received from the source.</exception>
	/// <exception cref="T:System.OperationCanceledException">The operation was canceled before an item was received from the source.</exception>
	public static TOutput Receive<TOutput>(this ISourceBlock<TOutput> source, CancellationToken cancellationToken)
	{
		return source.Receive(Common.InfiniteTimeSpan, cancellationToken);
	}

	/// <summary>Synchronously receives an item from the source.</summary>
	/// <returns>The received item.</returns>
	/// <param name="source">The source from which to receive.</param>
	/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out-or-<paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.InvalidOperationException">No item could be received from the source.</exception>
	/// <exception cref="T:System.TimeoutException">The specified timeout expired before an item was received from the source.</exception>
	public static TOutput Receive<TOutput>(this ISourceBlock<TOutput> source, TimeSpan timeout)
	{
		return source.Receive(timeout, CancellationToken.None);
	}

	/// <summary>Synchronously receives an item from the source.</summary>
	/// <returns>The received item.</returns>
	/// <param name="source">The source from which to receive.</param>
	/// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
	/// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> which may be used to cancel the receive operation.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	///   <paramref name="timeout" /> is a negative number other than -1 milliseconds, which represents an infinite time-out-or-<paramref name="timeout" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
	/// <exception cref="T:System.InvalidOperationException">No item could be received from the source.</exception>
	/// <exception cref="T:System.TimeoutException">The specified timeout expired before an item was received from the source.</exception>
	/// <exception cref="T:System.OperationCanceledException">The operation was canceled before an item was received from the source.</exception>
	public static TOutput Receive<TOutput>(this ISourceBlock<TOutput> source, TimeSpan timeout, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		if (!Common.IsValidTimeout(timeout))
		{
			throw new ArgumentOutOfRangeException("timeout", System.SR.ArgumentOutOfRange_NeedNonNegOrNegative1);
		}
		cancellationToken.ThrowIfCancellationRequested();
		if (source is IReceivableSourceBlock<TOutput> receivableSourceBlock && receivableSourceBlock.TryReceive(null, out var item))
		{
			return item;
		}
		Task<TOutput> task = source.ReceiveCore(attemptTryReceive: false, timeout, cancellationToken);
		try
		{
			return task.GetAwaiter().GetResult();
		}
		catch
		{
			if (task.IsCanceled)
			{
				cancellationToken.ThrowIfCancellationRequested();
			}
			throw;
		}
	}

	private static Task<TOutput> ReceiveCore<TOutput>(this ISourceBlock<TOutput> source, bool attemptTryReceive, TimeSpan timeout, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Common.CreateTaskFromCancellation<TOutput>(cancellationToken);
		}
		if (attemptTryReceive && source is IReceivableSourceBlock<TOutput> receivableSourceBlock)
		{
			try
			{
				if (receivableSourceBlock.TryReceive(null, out var item))
				{
					return Task.FromResult(item);
				}
			}
			catch (Exception exception)
			{
				return Common.CreateTaskFromException<TOutput>(exception);
			}
		}
		int num = (int)timeout.TotalMilliseconds;
		if (num == 0)
		{
			return Common.CreateTaskFromException<TOutput>(ReceiveTarget<TOutput>.CreateExceptionForTimeout());
		}
		return ReceiveCoreByLinking(source, num, cancellationToken);
	}

	private static Task<TOutput> ReceiveCoreByLinking<TOutput>(ISourceBlock<TOutput> source, int millisecondsTimeout, CancellationToken cancellationToken)
	{
		ReceiveTarget<TOutput> receiveTarget = new ReceiveTarget<TOutput>();
		try
		{
			if (cancellationToken.CanBeCanceled)
			{
				receiveTarget._externalCancellationToken = cancellationToken;
				receiveTarget._regFromExternalCancellationToken = cancellationToken.Register(_cancelCts, receiveTarget._cts);
			}
			if (millisecondsTimeout > 0)
			{
				receiveTarget._timer = new Timer(ReceiveTarget<TOutput>.CachedLinkingTimerCallback, receiveTarget, millisecondsTimeout, -1);
			}
			if (receiveTarget._cts.Token.CanBeCanceled)
			{
				receiveTarget._cts.Token.Register(ReceiveTarget<TOutput>.CachedLinkingCancellationCallback, receiveTarget);
			}
			IDisposable comparand = (receiveTarget._unlink = source.LinkTo(receiveTarget, DataflowLinkOptions.UnlinkAfterOneAndPropagateCompletion));
			if (Volatile.Read(in receiveTarget._cleanupReserved))
			{
				Interlocked.CompareExchange(ref receiveTarget._unlink, null, comparand)?.Dispose();
			}
		}
		catch (Exception receivedException)
		{
			receiveTarget._receivedException = receivedException;
			receiveTarget.TryCleanupAndComplete(ReceiveCoreByLinkingCleanupReason.SourceProtocolError);
		}
		return receiveTarget.Task;
	}

	/// <summary>Provides a <see cref="T:System.Threading.Tasks.Task`1" /> that asynchronously monitors the source for available output.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that informs of whether and when more output is available. If, when the task completes, its <see cref="P:System.Threading.Tasks.Task`1.Result" /> is true, more output is available in the source (though another consumer of the source may retrieve the data).  If it returns false, more output is not and will never be available, due to the source completing prior to output being available.</returns>
	/// <param name="source">The source to monitor.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	public static Task<bool> OutputAvailableAsync<TOutput>(this ISourceBlock<TOutput> source)
	{
		return source.OutputAvailableAsync(CancellationToken.None);
	}

	/// <summary>Provides a <see cref="T:System.Threading.Tasks.Task`1" /> that asynchronously monitors the source for available output.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that informs of whether and when more output is available. If, when the task completes, its <see cref="P:System.Threading.Tasks.Task`1.Result" /> is true, more output is available in the source (though another consumer of the source may retrieve the data). If it returns false, more output is not and will never be available, due to the source completing prior to output being available. If it returns false, more output is not and will never be available, due to the source completing prior to output being available.</returns>
	/// <param name="source">The source to monitor.</param>
	/// <param name="cancellationToken">The cancellation token with which to cancel the asynchronous operation.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	public static Task<bool> OutputAvailableAsync<TOutput>(this ISourceBlock<TOutput> source, CancellationToken cancellationToken)
	{
		if (source != null)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return Impl(source, cancellationToken);
			}
			return Common.CreateTaskFromCancellation<bool>(cancellationToken);
		}
		throw new ArgumentNullException("source");
		static async Task<bool> Impl(ISourceBlock<TOutput> sourceBlock, CancellationToken cancellationToken2)
		{
			OutputAvailableAsyncTarget<TOutput> outputAvailableAsyncTarget = new OutputAvailableAsyncTarget<TOutput>();
			using (sourceBlock.LinkTo(outputAvailableAsyncTarget, DataflowLinkOptions.UnlinkAfterOneAndPropagateCompletion))
			{
				CancellationTokenRegistration registration = default(CancellationTokenRegistration);
				try
				{
					if (!outputAvailableAsyncTarget.Task.IsCompleted)
					{
						registration = cancellationToken2.UnsafeRegister(delegate(object state, CancellationToken cancellationToken3)
						{
							((OutputAvailableAsyncTarget<TOutput>)state).TrySetCanceled(cancellationToken3);
						}, outputAvailableAsyncTarget);
					}
					return await outputAvailableAsyncTarget.Task.ConfigureAwait(continueOnCapturedContext: false);
				}
				finally
				{
					registration.Dispose();
				}
			}
		}
	}

	/// <summary>Encapsulates a target and a source into a single propagator.</summary>
	/// <returns>The encapsulated target and source.</returns>
	/// <param name="target">The target to encapsulate.</param>
	/// <param name="source">The source to encapsulate.</param>
	/// <typeparam name="TInput">Specifies the type of input expected by the target.</typeparam>
	/// <typeparam name="TOutput">Specifies the type of output produced by the source.</typeparam>
	public static IPropagatorBlock<TInput, TOutput> Encapsulate<TInput, TOutput>(ITargetBlock<TInput> target, ISourceBlock<TOutput> source)
	{
		ArgumentNullException.ThrowIfNull(target, "target");
		ArgumentNullException.ThrowIfNull(source, "source");
		return new EncapsulatingPropagator<TInput, TOutput>(target, source);
	}

	/// <summary>Monitors two dataflow sources, invoking the provided handler for whichever source makes data available first.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous choice. If both sources are completed prior to the choice completing, the resulting task will be canceled. When one of the sources has data available and successfully propagates it to the choice, the resulting task will complete when the handler completes; if the handler throws an exception, the task will end in the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and will contain the unhandled exception. Otherwise, the task will end with its <see cref="P:System.Threading.Tasks.Task`1.Result" /> set to either 0 or 1 to represent the first or second source, respectively.This method will only consume an element from one of the two data sources, never both.</returns>
	/// <param name="source1">The first source.</param>
	/// <param name="action1">The handler to execute on data from the first source.</param>
	/// <param name="source2">The second source.</param>
	/// <param name="action2">The handler to execute on data from the second source.</param>
	/// <typeparam name="T1">Specifies type of data contained in the first source.</typeparam>
	/// <typeparam name="T2">Specifies type of data contained in the second source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source1" /> is null.-or-The <paramref name="action1" /> is null.-or-The <paramref name="source2" /> is null.-or-The <paramref name="action2" /> is null.</exception>
	public static Task<int> Choose<T1, T2>(ISourceBlock<T1> source1, Action<T1> action1, ISourceBlock<T2> source2, Action<T2> action2)
	{
		return Choose(source1, action1, source2, action2, DataflowBlockOptions.Default);
	}

	/// <summary>Monitors two dataflow sources, invoking the provided handler for whichever source makes data available first.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous choice. If both sources are completed prior to the choice completing, or if the <see cref="T:System.Threading.CancellationToken" /> provided as part of <paramref name="dataflowBlockOptions" /> is canceled prior to the choice completing, the resulting task will be canceled. When one of the sources has data available and successfully propagates it to the choice, the resulting task will complete when the handler completes; if the handler throws an exception, the task will end in the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and will contain the unhandled exception. Otherwise, the task will end with its <see cref="P:System.Threading.Tasks.Task`1.Result" /> set to either 0 or 1 to represent the first or second source, respectively.This method will only consume an element from one of the two data sources, never both. If cancellation is requested after an element has been received, the cancellation request will be ignored, and the relevant handler will be allowed to execute. </returns>
	/// <param name="source1">The first source.</param>
	/// <param name="action1">The handler to execute on data from the first source.</param>
	/// <param name="source2">The second source.</param>
	/// <param name="action2">The handler to execute on data from the second source.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this choice.</param>
	/// <typeparam name="T1">Specifies type of data contained in the first source.</typeparam>
	/// <typeparam name="T2">Specifies type of data contained in the second source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source1" /> is null.-or-The <paramref name="action1" /> is null.-or-The <paramref name="source2" /> is null.-or-The <paramref name="action2" /> is null.-or-The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public static Task<int> Choose<T1, T2>(ISourceBlock<T1> source1, Action<T1> action1, ISourceBlock<T2> source2, Action<T2> action2, DataflowBlockOptions dataflowBlockOptions)
	{
		ArgumentNullException.ThrowIfNull(source1, "source1");
		ArgumentNullException.ThrowIfNull(action1, "action1");
		ArgumentNullException.ThrowIfNull(source2, "source2");
		ArgumentNullException.ThrowIfNull(action2, "action2");
		ArgumentNullException.ThrowIfNull(dataflowBlockOptions, "dataflowBlockOptions");
		return ChooseCore<T1, T2, VoidResult>(source1, action1, source2, action2, null, null, dataflowBlockOptions);
	}

	/// <summary>Monitors three dataflow sources, invoking the provided handler for whichever source makes data available first.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous choice. If all sources are completed prior to the choice completing, the resulting task will be canceled. When one of the sources has data available and successfully propagates it to the choice, the resulting task will complete when the handler completes; if the handler throws an exception, the task will end in the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and will contain the unhandled exception. Otherwise, the task will end with its <see cref="P:System.Threading.Tasks.Task`1.Result" /> set to the 0-based index of the source.This method will only consume an element from one of the data sources, never more than one.</returns>
	/// <param name="source1">The first source.</param>
	/// <param name="action1">The handler to execute on data from the first source.</param>
	/// <param name="source2">The second source.</param>
	/// <param name="action2">The handler to execute on data from the second source.</param>
	/// <param name="source3">The third source.</param>
	/// <param name="action3">The handler to execute on data from the third source.</param>
	/// <typeparam name="T1">Specifies type of data contained in the first source.</typeparam>
	/// <typeparam name="T2">Specifies type of data contained in the second source.</typeparam>
	/// <typeparam name="T3">Specifies type of data contained in the third source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source1" /> is null.-or-The <paramref name="action1" /> is null.-or-The <paramref name="source2" /> is null.-or-The <paramref name="action2" /> is null.-or-The <paramref name="source3" /> is null.-or-The <paramref name="action3" /> is null.</exception>
	public static Task<int> Choose<T1, T2, T3>(ISourceBlock<T1> source1, Action<T1> action1, ISourceBlock<T2> source2, Action<T2> action2, ISourceBlock<T3> source3, Action<T3> action3)
	{
		return Choose(source1, action1, source2, action2, source3, action3, DataflowBlockOptions.Default);
	}

	/// <summary>Monitors three dataflow sources, invoking the provided handler for whichever source makes data available first.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the asynchronous choice. If all sources are completed prior to the choice completing, or if the <see cref="T:System.Threading.CancellationToken" /> provided as part of <paramref name="dataflowBlockOptions" /> is canceled prior to the choice completing, the resulting task will be canceled. When one of the sources has data available and successfully propagates it to the choice, the resulting task will complete when the handler completes; if the handler throws an exception, the task will end in the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and will contain the unhandled exception. Otherwise, the task will end with its <see cref="P:System.Threading.Tasks.Task`1.Result" /> set to the 0-based index of the source.This method will only consume an element from one of the data sources, never more than one. If cancellation is requested after an element has been received, the cancellation request will be ignored, and the relevant handler will be allowed to execute. </returns>
	/// <param name="source1">The first source.</param>
	/// <param name="action1">The handler to execute on data from the first source.</param>
	/// <param name="source2">The second source.</param>
	/// <param name="action2">The handler to execute on data from the second source.</param>
	/// <param name="source3">The third source.</param>
	/// <param name="action3">The handler to execute on data from the third source.</param>
	/// <param name="dataflowBlockOptions">The options with which to configure this choice.</param>
	/// <typeparam name="T1">Specifies type of data contained in the first source.</typeparam>
	/// <typeparam name="T2">Specifies type of data contained in the second source.</typeparam>
	/// <typeparam name="T3">Specifies type of data contained in the third source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source1" /> is null.-or-The <paramref name="action1" /> is null.-or-The <paramref name="source2" /> is null.-or-The <paramref name="action2" /> is null.-or-The <paramref name="source3" /> is null.-or-The <paramref name="action3" /> is null.-or-The <paramref name="dataflowBlockOptions" /> is null.</exception>
	public static Task<int> Choose<T1, T2, T3>(ISourceBlock<T1> source1, Action<T1> action1, ISourceBlock<T2> source2, Action<T2> action2, ISourceBlock<T3> source3, Action<T3> action3, DataflowBlockOptions dataflowBlockOptions)
	{
		ArgumentNullException.ThrowIfNull(source1, "source1");
		ArgumentNullException.ThrowIfNull(action1, "action1");
		ArgumentNullException.ThrowIfNull(source2, "source2");
		ArgumentNullException.ThrowIfNull(action2, "action2");
		ArgumentNullException.ThrowIfNull(source3, "source3");
		ArgumentNullException.ThrowIfNull(action3, "action3");
		ArgumentNullException.ThrowIfNull(dataflowBlockOptions, "dataflowBlockOptions");
		return ChooseCore(source1, action1, source2, action2, source3, action3, dataflowBlockOptions);
	}

	private static Task<int> ChooseCore<T1, T2, T3>(ISourceBlock<T1> source1, Action<T1> action1, ISourceBlock<T2> source2, Action<T2> action2, ISourceBlock<T3> source3, Action<T3> action3, DataflowBlockOptions dataflowBlockOptions)
	{
		bool flag = source3 != null;
		if (dataflowBlockOptions.CancellationToken.IsCancellationRequested)
		{
			return Common.CreateTaskFromCancellation<int>(dataflowBlockOptions.CancellationToken);
		}
		try
		{
			TaskScheduler taskScheduler = dataflowBlockOptions.TaskScheduler;
			if (TryChooseFromSource(source1, action1, 0, taskScheduler, out var task) || TryChooseFromSource(source2, action2, 1, taskScheduler, out task) || (flag && TryChooseFromSource(source3, action3, 2, taskScheduler, out task)))
			{
				return task;
			}
		}
		catch (Exception exception)
		{
			return Common.CreateTaskFromException<int>(exception);
		}
		return ChooseCoreByLinking(source1, action1, source2, action2, source3, action3, dataflowBlockOptions);
	}

	private static bool TryChooseFromSource<T>(ISourceBlock<T> source, Action<T> action, int branchId, TaskScheduler scheduler, [NotNullWhen(true)] out Task<int> task)
	{
		if (!(source is IReceivableSourceBlock<T> source2) || !source2.TryReceive(out var item))
		{
			task = null;
			return false;
		}
		task = Task.Factory.StartNew(ChooseTarget<T>.s_processBranchFunction, Tuple.Create(action, item, branchId), CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler);
		return true;
	}

	private static Task<int> ChooseCoreByLinking<T1, T2, T3>(ISourceBlock<T1> source1, Action<T1> action1, ISourceBlock<T2> source2, Action<T2> action2, ISourceBlock<T3> source3, Action<T3> action3, DataflowBlockOptions dataflowBlockOptions)
	{
		bool num = source3 != null;
		StrongBox<Task> boxedCompleted = new StrongBox<Task>();
		CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(dataflowBlockOptions.CancellationToken, CancellationToken.None);
		TaskScheduler taskScheduler = dataflowBlockOptions.TaskScheduler;
		Task<int>[] array = new Task<int>[num ? 3 : 2];
		array[0] = CreateChooseBranch(boxedCompleted, cts, taskScheduler, 0, source1, action1);
		array[1] = CreateChooseBranch(boxedCompleted, cts, taskScheduler, 1, source2, action2);
		if (num)
		{
			array[2] = CreateChooseBranch(boxedCompleted, cts, taskScheduler, 2, source3, action3);
		}
		TaskCompletionSource<int> result = new TaskCompletionSource<int>();
		Task.Factory.ContinueWhenAll(array, delegate(Task<int>[] tasks)
		{
			List<Exception> list = null;
			int num2 = -1;
			foreach (Task<int> task in tasks)
			{
				switch (task.Status)
				{
				case TaskStatus.Faulted:
					Common.AddException(ref list, task.Exception, unwrapInnerExceptions: true);
					break;
				case TaskStatus.RanToCompletion:
				{
					int result2 = task.Result;
					if (result2 >= 0)
					{
						num2 = result2;
					}
					break;
				}
				}
			}
			if (list != null)
			{
				result.TrySetException(list);
			}
			else if (num2 >= 0)
			{
				result.TrySetResult(num2);
			}
			else
			{
				result.TrySetCanceled(dataflowBlockOptions.CancellationToken);
			}
			cts.Dispose();
		}, CancellationToken.None, Common.GetContinuationOptions(), TaskScheduler.Default);
		return result.Task;
	}

	private static Task<int> CreateChooseBranch<T>(StrongBox<Task> boxedCompleted, CancellationTokenSource cts, TaskScheduler scheduler, int branchId, ISourceBlock<T> source, Action<T> action)
	{
		if (cts.IsCancellationRequested)
		{
			return Common.CreateTaskFromCancellation<int>(cts.Token);
		}
		ChooseTarget<T> chooseTarget = new ChooseTarget<T>(boxedCompleted, cts.Token);
		IDisposable unlink;
		try
		{
			unlink = source.LinkTo(chooseTarget, DataflowLinkOptions.UnlinkAfterOneAndPropagateCompletion);
		}
		catch (Exception exception)
		{
			cts.Cancel();
			return Common.CreateTaskFromException<int>(exception);
		}
		return chooseTarget.Task.ContinueWith(delegate(Task<T> completed)
		{
			try
			{
				if (completed.Status == TaskStatus.RanToCompletion)
				{
					cts.Cancel();
					action(completed.Result);
					return branchId;
				}
				return -1;
			}
			finally
			{
				unlink.Dispose();
			}
		}, CancellationToken.None, Common.GetContinuationOptions(), scheduler);
	}

	/// <summary>Creates a new <see cref="T:System.IObservable`1" /> abstraction over the <see cref="T:System.Threading.Tasks.Dataflow.ISourceBlock`1" />.</summary>
	/// <returns>An <see cref="T:System.IObservable`1" /> that enables observers to be subscribed to the source.</returns>
	/// <param name="source">The source to wrap.</param>
	/// <typeparam name="TOutput">Specifies the type of data contained in the source.</typeparam>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> is null.</exception>
	public static IObservable<TOutput> AsObservable<TOutput>(this ISourceBlock<TOutput> source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return SourceObservable<TOutput>.From(source);
	}

	/// <summary>Creates a new <see cref="T:System.IObserver`1" /> abstraction over the <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" />.</summary>
	/// <returns>An observer that wraps the target block.</returns>
	/// <param name="target">The target to wrap.</param>
	/// <typeparam name="TInput">Specifies the type of input accepted by the target block.</typeparam>
	public static IObserver<TInput> AsObserver<TInput>(this ITargetBlock<TInput> target)
	{
		ArgumentNullException.ThrowIfNull(target, "target");
		return new TargetObserver<TInput>(target);
	}

	/// <summary>Gets a target block that synchronously accepts all messages offered to it and drops them.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Dataflow.ITargetBlock`1" /> that accepts and subsequently drops all offered messages.</returns>
	/// <typeparam name="TInput">The type of the messages this block can accept.</typeparam>
	public static ITargetBlock<TInput> NullTarget<TInput>()
	{
		return new NullTargetBlock<TInput>();
	}

	public static IAsyncEnumerable<TOutput> ReceiveAllAsync<TOutput>(this IReceivableSourceBlock<TOutput> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return Core(source, cancellationToken);
		static async IAsyncEnumerable<TOutput> Core(IReceivableSourceBlock<TOutput> source2, [EnumeratorCancellation] CancellationToken cancellationToken2)
		{
			while (await source2.OutputAvailableAsync(cancellationToken2).ConfigureAwait(continueOnCapturedContext: false))
			{
				TOutput item;
				while (source2.TryReceive(out item))
				{
					yield return item;
				}
			}
		}
	}
}
