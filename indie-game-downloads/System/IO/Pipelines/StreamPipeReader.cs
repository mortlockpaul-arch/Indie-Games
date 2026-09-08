using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines;

internal sealed class StreamPipeReader : PipeReader
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003CReadInternalAsync_003Eg__Core_007C39_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public PoolingAsyncValueTaskMethodBuilder<ReadResult> _003C_003Et__builder;

		public CancellationToken cancellationToken;

		public StreamPipeReader reader;

		public CancellationTokenSource tokenSource;

		public int? minimumSize;

		private CancellationTokenRegistration _003C_003E7__wrap1;

		private bool _003CisCanceled_003E5__3;

		private bool _003CisCompleted_003E5__4;

		private ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			ReadResult result2;
			try
			{
				if ((uint)num > 1u)
				{
					CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
					if (cancellationToken.CanBeCanceled)
					{
						cancellationTokenRegistration = cancellationToken.UnsafeRegister(delegate(object state)
						{
							((StreamPipeReader)state).Cancel();
						}, reader);
					}
					_003C_003E7__wrap1 = cancellationTokenRegistration;
				}
				try
				{
					if ((uint)num > 1u)
					{
						_003CisCanceled_003E5__3 = false;
						_003CisCompleted_003E5__4 = false;
					}
					try
					{
						ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter awaiter;
						if (num != 0)
						{
							if (num == 1)
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_01d2;
							}
							if (!reader.UseZeroByteReads || reader._bufferedBytes != 0L)
							{
								goto IL_011d;
							}
							awaiter = reader.InnerStream.ReadAsync(Memory<byte>.Empty, tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable<int>.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						awaiter.GetResult();
						goto IL_011d;
						IL_011d:
						reader.AllocateReadTail(minimumSize);
						Memory<byte> buffer = reader._readTail.AvailableMemory.Slice(reader._readTail.End);
						awaiter = reader.InnerStream.ReadAsync(buffer, tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_01d2;
						IL_01d2:
						int result = awaiter.GetResult();
						reader._readTail.End += result;
						reader._bufferedBytes += result;
						if (result == 0)
						{
							_003CisCompleted_003E5__4 = true;
						}
						else if (minimumSize.HasValue && reader._bufferedBytes < minimumSize)
						{
							goto IL_011d;
						}
					}
					catch (OperationCanceledException ex)
					{
						reader.ClearCancellationToken();
						if (cancellationToken.IsCancellationRequested)
						{
							throw new OperationCanceledException(ex.Message, ex, cancellationToken);
						}
						if (!tokenSource.IsCancellationRequested)
						{
							throw;
						}
						_003CisCanceled_003E5__3 = true;
					}
					result2 = new ReadResult(reader.GetCurrentReadOnlySequence(), _003CisCanceled_003E5__3, _003CisCompleted_003E5__4);
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)_003C_003E7__wrap1/*cast due to constrained. prefix*/).Dispose();
					}
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	internal const int InitialSegmentPoolSize = 4;

	internal const int MaxSegmentPoolSize = 256;

	private CancellationTokenSource _internalTokenSource;

	private bool _isReaderCompleted;

	private BufferSegment _readHead;

	private int _readIndex;

	private BufferSegment _readTail;

	private long _bufferedBytes;

	private bool _examinedEverything;

	private readonly object _lock = new object();

	private BufferSegmentStack _bufferSegmentPool;

	private readonly StreamPipeReaderOptions _options;

	private bool LeaveOpen => _options.LeaveOpen;

	private bool UseZeroByteReads => _options.UseZeroByteReads;

	private int BufferSize => _options.BufferSize;

	private int MaxBufferSize => _options.MaxBufferSize;

	private int MinimumReadThreshold => _options.MinimumReadSize;

	public Stream InnerStream { get; }

	private CancellationTokenSource InternalTokenSource
	{
		get
		{
			lock (_lock)
			{
				return _internalTokenSource ?? (_internalTokenSource = new CancellationTokenSource());
			}
		}
	}

	public StreamPipeReader(Stream readingStream, StreamPipeReaderOptions options)
	{
		if (readingStream == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.readingStream);
		}
		if (options == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.options);
		}
		InnerStream = readingStream;
		_options = options;
		_bufferSegmentPool = new BufferSegmentStack(4);
	}

	public override void AdvanceTo(SequencePosition consumed)
	{
		AdvanceTo(consumed, consumed);
	}

	public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
	{
		ThrowIfCompleted();
		AdvanceTo((BufferSegment)consumed.GetObject(), consumed.GetInteger(), (BufferSegment)examined.GetObject(), examined.GetInteger());
	}

	private void AdvanceTo(BufferSegment consumedSegment, int consumedIndex, BufferSegment examinedSegment, int examinedIndex)
	{
		if (consumedSegment != null && examinedSegment != null)
		{
			if (_readHead == null)
			{
				ThrowHelper.ThrowInvalidOperationException_AdvanceToInvalidCursor();
			}
			BufferSegment bufferSegment = _readHead;
			BufferSegment bufferSegment2 = consumedSegment;
			long length = BufferSegment.GetLength(bufferSegment, _readIndex, consumedSegment, consumedIndex);
			_bufferedBytes -= length;
			_examinedEverything = false;
			if (examinedSegment == _readTail)
			{
				_examinedEverything = examinedIndex == _readTail.End;
			}
			if (_bufferedBytes == 0L)
			{
				bufferSegment2 = null;
				_readHead = null;
				_readTail = null;
				_readIndex = 0;
			}
			else if (consumedIndex == bufferSegment2.Length)
			{
				BufferSegment bufferSegment3 = (_readHead = bufferSegment2.NextSegment);
				_readIndex = 0;
				bufferSegment2 = bufferSegment3;
			}
			else
			{
				_readHead = consumedSegment;
				_readIndex = consumedIndex;
			}
			while (bufferSegment != bufferSegment2)
			{
				BufferSegment? nextSegment = bufferSegment.NextSegment;
				ReturnSegmentUnsynchronized(bufferSegment);
				bufferSegment = nextSegment;
			}
		}
	}

	public override void CancelPendingRead()
	{
		InternalTokenSource.Cancel();
	}

	public override void Complete(Exception? exception = null)
	{
		if (CompleteAndGetNeedsDispose())
		{
			InnerStream.Dispose();
		}
	}

	public override ValueTask CompleteAsync(Exception? exception = null)
	{
		if (!CompleteAndGetNeedsDispose())
		{
			return default(ValueTask);
		}
		return InnerStream.DisposeAsync();
	}

	private bool CompleteAndGetNeedsDispose()
	{
		if (_isReaderCompleted)
		{
			return false;
		}
		_isReaderCompleted = true;
		BufferSegment bufferSegment = _readHead;
		while (bufferSegment != null)
		{
			BufferSegment bufferSegment2 = bufferSegment;
			bufferSegment = bufferSegment.NextSegment;
			bufferSegment2.Reset();
		}
		return !LeaveOpen;
	}

	public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return ReadInternalAsync(null, cancellationToken);
	}

	protected override ValueTask<ReadResult> ReadAtLeastAsyncCore(int minimumSize, CancellationToken cancellationToken)
	{
		return ReadInternalAsync(minimumSize, cancellationToken);
	}

	private ValueTask<ReadResult> ReadInternalAsync(int? minimumSize, CancellationToken cancellationToken)
	{
		ThrowIfCompleted();
		if (cancellationToken.IsCancellationRequested)
		{
			return new ValueTask<ReadResult>(Task.FromCanceled<ReadResult>(cancellationToken));
		}
		CancellationTokenSource internalTokenSource = InternalTokenSource;
		if (TryReadInternal(internalTokenSource, out var result) && (!minimumSize.HasValue || result.Buffer.Length >= minimumSize || result.IsCompleted || result.IsCanceled))
		{
			return new ValueTask<ReadResult>(result);
		}
		return Core(this, minimumSize, internalTokenSource, cancellationToken);
		[AsyncStateMachine(typeof(_003C_003CReadInternalAsync_003Eg__Core_007C39_0_003Ed))]
		[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
		static ValueTask<ReadResult> Core(StreamPipeReader reader, int? minimumSize2, CancellationTokenSource tokenSource, CancellationToken cancellationToken2)
		{
			Unsafe.SkipInit(out _003C_003CReadInternalAsync_003Eg__Core_007C39_0_003Ed stateMachine);
			stateMachine._003C_003Et__builder = PoolingAsyncValueTaskMethodBuilder<ReadResult>.Create();
			stateMachine.reader = reader;
			stateMachine.minimumSize = minimumSize2;
			stateMachine.tokenSource = tokenSource;
			stateMachine.cancellationToken = cancellationToken2;
			stateMachine._003C_003E1__state = -1;
			stateMachine._003C_003Et__builder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}
	}

	public override async Task CopyToAsync(PipeWriter destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfCompleted();
		CancellationTokenSource tokenSource = InternalTokenSource;
		if (tokenSource.IsCancellationRequested)
		{
			ThrowHelper.ThrowOperationCanceledException_ReadCanceled();
		}
		CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
		if (cancellationToken.CanBeCanceled)
		{
			cancellationTokenRegistration = cancellationToken.UnsafeRegister(delegate(object state)
			{
				((StreamPipeReader)state).Cancel();
			}, this);
		}
		using (cancellationTokenRegistration)
		{
			_ = 1;
			try
			{
				BufferSegment segment = _readHead;
				int start = _readIndex;
				try
				{
					while (segment != null)
					{
						FlushResult flushResult = await destination.WriteAsync(segment.Memory.Slice(start), tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
						if (flushResult.IsCanceled)
						{
							ThrowHelper.ThrowOperationCanceledException_FlushCanceled();
						}
						segment = segment.NextSegment;
						start = 0;
						if (flushResult.IsCompleted)
						{
							return;
						}
					}
				}
				finally
				{
					if (segment != null)
					{
						AdvanceTo(segment, segment.End, segment, segment.End);
					}
				}
				await InnerStream.CopyToAsync(destination, tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
				ClearCancellationToken();
				throw;
			}
		}
	}

	public override async Task CopyToAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfCompleted();
		CancellationTokenSource tokenSource = InternalTokenSource;
		if (tokenSource.IsCancellationRequested)
		{
			ThrowHelper.ThrowOperationCanceledException_ReadCanceled();
		}
		CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
		if (cancellationToken.CanBeCanceled)
		{
			cancellationTokenRegistration = cancellationToken.UnsafeRegister(delegate(object state)
			{
				((StreamPipeReader)state).Cancel();
			}, this);
		}
		using (cancellationTokenRegistration)
		{
			_ = 1;
			try
			{
				BufferSegment segment = _readHead;
				int start = _readIndex;
				try
				{
					while (segment != null)
					{
						await destination.WriteAsync(segment.Memory.Slice(start), tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
						segment = segment.NextSegment;
						start = 0;
					}
				}
				finally
				{
					if (segment != null)
					{
						AdvanceTo(segment, segment.End, segment, segment.End);
					}
				}
				await InnerStream.CopyToAsync(destination, tokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
				ClearCancellationToken();
				throw;
			}
		}
	}

	private void ClearCancellationToken()
	{
		lock (_lock)
		{
			_internalTokenSource = null;
		}
	}

	private void ThrowIfCompleted()
	{
		if (_isReaderCompleted)
		{
			ThrowHelper.ThrowInvalidOperationException_NoReadingAllowed();
		}
	}

	public override bool TryRead(out ReadResult result)
	{
		ThrowIfCompleted();
		return TryReadInternal(InternalTokenSource, out result);
	}

	private bool TryReadInternal(CancellationTokenSource source, out ReadResult result)
	{
		bool isCancellationRequested = source.IsCancellationRequested;
		if (isCancellationRequested || (_bufferedBytes > 0 && !_examinedEverything))
		{
			if (isCancellationRequested)
			{
				ClearCancellationToken();
			}
			ReadOnlySequence<byte> currentReadOnlySequence = GetCurrentReadOnlySequence();
			result = new ReadResult(currentReadOnlySequence, isCancellationRequested, isCompleted: false);
			return true;
		}
		result = default(ReadResult);
		return false;
	}

	private ReadOnlySequence<byte> GetCurrentReadOnlySequence()
	{
		if (_readHead != null)
		{
			return new ReadOnlySequence<byte>(_readHead, _readIndex, _readTail, _readTail.End);
		}
		return default(ReadOnlySequence<byte>);
	}

	private void AllocateReadTail(int? minimumSize = null)
	{
		if (_readHead == null)
		{
			_readHead = AllocateSegment(minimumSize);
			_readTail = _readHead;
		}
		else if (_readTail.WritableBytes < MinimumReadThreshold)
		{
			BufferSegment bufferSegment = AllocateSegment(minimumSize);
			_readTail.SetNext(bufferSegment);
			_readTail = bufferSegment;
		}
	}

	private BufferSegment AllocateSegment(int? minimumSize = null)
	{
		BufferSegment bufferSegment = CreateSegmentUnsynchronized();
		int num = minimumSize ?? BufferSize;
		int num2 = ((!_options.IsDefaultSharedMemoryPool) ? _options.Pool.MaxBufferSize : (-1));
		if (num <= num2)
		{
			int segmentSize = GetSegmentSize(num, num2);
			bufferSegment.SetOwnedMemory(_options.Pool.Rent(segmentSize));
		}
		else
		{
			int segmentSize2 = GetSegmentSize(num, MaxBufferSize);
			bufferSegment.SetOwnedMemory(ArrayPool<byte>.Shared.Rent(segmentSize2));
		}
		return bufferSegment;
	}

	private int GetSegmentSize(int sizeHint, int maxBufferSize)
	{
		sizeHint = Math.Max(BufferSize, sizeHint);
		return Math.Min(maxBufferSize, sizeHint);
	}

	private BufferSegment CreateSegmentUnsynchronized()
	{
		if (_bufferSegmentPool.TryPop(out BufferSegment result))
		{
			return result;
		}
		return new BufferSegment();
	}

	private void ReturnSegmentUnsynchronized(BufferSegment segment)
	{
		segment.Reset();
		if (_bufferSegmentPool.Count < 256)
		{
			_bufferSegmentPool.Push(segment);
		}
	}

	private void Cancel()
	{
		InternalTokenSource.Cancel();
	}
}
