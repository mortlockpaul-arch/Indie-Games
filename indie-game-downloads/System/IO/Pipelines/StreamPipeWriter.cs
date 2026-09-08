using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines;

internal sealed class StreamPipeWriter : PipeWriter
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CFlushAsyncInternal_003Ed__39 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public PoolingAsyncValueTaskMethodBuilder<FlushResult> _003C_003Et__builder;

		public CancellationToken cancellationToken;

		public StreamPipeWriter _003C_003E4__this;

		public bool writeToStream;

		public ReadOnlyMemory<byte> data;

		private CancellationTokenRegistration _003C_003E7__wrap1;

		private CancellationToken _003ClocalToken_003E5__3;

		private BufferSegment _003Csegment_003E5__4;

		private BufferSegment _003CreturnSegment_003E5__5;

		private ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			StreamPipeWriter streamPipeWriter = _003C_003E4__this;
			FlushResult result;
			try
			{
				if ((uint)num > 2u)
				{
					CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
					if (cancellationToken.CanBeCanceled)
					{
						cancellationTokenRegistration = cancellationToken.UnsafeRegister(delegate(object state)
						{
							((StreamPipeWriter)state).Cancel();
						}, streamPipeWriter);
					}
					if (streamPipeWriter._tailBytesBuffered > 0)
					{
						streamPipeWriter._tail.End += streamPipeWriter._tailBytesBuffered;
						streamPipeWriter._tailBytesBuffered = 0;
					}
					_003C_003E7__wrap1 = cancellationTokenRegistration;
				}
				try
				{
					if ((uint)num > 2u)
					{
						_003ClocalToken_003E5__3 = streamPipeWriter.InternalTokenSource.Token;
					}
					try
					{
						ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter awaiter2;
						ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
						switch (num)
						{
						default:
							_003Csegment_003E5__4 = streamPipeWriter._head;
							goto IL_0194;
						case 0:
							awaiter2 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_016e;
						case 1:
							awaiter2 = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_022e;
						case 2:
							{
								awaiter = _003C_003Eu__2;
								_003C_003Eu__2 = default(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_02b9;
							}
							IL_0194:
							if (_003Csegment_003E5__4 != null)
							{
								_003CreturnSegment_003E5__5 = _003Csegment_003E5__4;
								_003Csegment_003E5__4 = _003Csegment_003E5__4.NextSegment;
								if ((_003CreturnSegment_003E5__5.Length > 0) & writeToStream)
								{
									awaiter2 = streamPipeWriter.InnerStream.WriteAsync(_003CreturnSegment_003E5__5.Memory, _003ClocalToken_003E5__3).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
									if (!awaiter2.IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter2;
										_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
										return;
									}
									goto IL_016e;
								}
								goto IL_0175;
							}
							if (!writeToStream)
							{
								break;
							}
							if (data.Length > 0)
							{
								awaiter2 = streamPipeWriter.InnerStream.WriteAsync(data, _003ClocalToken_003E5__3).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 1);
									_003C_003Eu__1 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref this);
									return;
								}
								goto IL_022e;
							}
							goto IL_0235;
							IL_0235:
							if (streamPipeWriter._bytesBuffered <= 0 && data.Length <= 0)
							{
								break;
							}
							awaiter = streamPipeWriter.InnerStream.FlushAsync(_003ClocalToken_003E5__3).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__2 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_02b9;
							IL_016e:
							awaiter2.GetResult();
							goto IL_0175;
							IL_0175:
							streamPipeWriter.ReturnSegmentUnsynchronized(_003CreturnSegment_003E5__5);
							streamPipeWriter._head = _003Csegment_003E5__4;
							_003CreturnSegment_003E5__5 = null;
							goto IL_0194;
							IL_02b9:
							awaiter.GetResult();
							break;
							IL_022e:
							awaiter2.GetResult();
							goto IL_0235;
						}
						streamPipeWriter._head = null;
						streamPipeWriter._tail = null;
						streamPipeWriter._tailMemory = default(Memory<byte>);
						streamPipeWriter._bytesBuffered = 0L;
						result = new FlushResult(isCanceled: false, isCompleted: false);
					}
					catch (OperationCanceledException)
					{
						object lockObject = streamPipeWriter._lockObject;
						bool lockTaken = false;
						try
						{
							Monitor.Enter(lockObject, ref lockTaken);
							streamPipeWriter._internalTokenSource = null;
						}
						finally
						{
							if (num < 0 && lockTaken)
							{
								Monitor.Exit(lockObject);
							}
						}
						if (!_003ClocalToken_003E5__3.IsCancellationRequested || cancellationToken.IsCancellationRequested)
						{
							throw;
						}
						result = new FlushResult(isCanceled: true, isCompleted: false);
					}
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
			_003C_003Et__builder.SetResult(result);
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

	private readonly int _minimumBufferSize;

	private BufferSegment _head;

	private BufferSegment _tail;

	private Memory<byte> _tailMemory;

	private int _tailBytesBuffered;

	private long _bytesBuffered;

	private readonly MemoryPool<byte> _pool;

	private readonly int _maxPooledBufferSize;

	private CancellationTokenSource _internalTokenSource;

	private bool _isCompleted;

	private readonly object _lockObject = new object();

	private BufferSegmentStack _bufferSegmentPool;

	private readonly bool _leaveOpen;

	private CancellationTokenSource InternalTokenSource
	{
		get
		{
			lock (_lockObject)
			{
				return _internalTokenSource ?? (_internalTokenSource = new CancellationTokenSource());
			}
		}
	}

	public Stream InnerStream { get; }

	public override bool CanGetUnflushedBytes => true;

	public override long UnflushedBytes => _bytesBuffered;

	public StreamPipeWriter(Stream writingStream, StreamPipeWriterOptions options)
	{
		if (writingStream == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.writingStream);
		}
		if (options == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.options);
		}
		InnerStream = writingStream;
		_minimumBufferSize = options.MinimumBufferSize;
		_pool = ((options.Pool == MemoryPool<byte>.Shared) ? null : options.Pool);
		_maxPooledBufferSize = _pool?.MaxBufferSize ?? (-1);
		_bufferSegmentPool = new BufferSegmentStack(4);
		_leaveOpen = options.LeaveOpen;
	}

	public override void Advance(int bytes)
	{
		if ((uint)bytes > (uint)_tailMemory.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.bytes);
		}
		_tailBytesBuffered += bytes;
		_bytesBuffered += bytes;
		_tailMemory = _tailMemory.Slice(bytes);
	}

	public override Memory<byte> GetMemory(int sizeHint = 0)
	{
		if (_isCompleted)
		{
			ThrowHelper.ThrowInvalidOperationException_NoWritingAllowed();
		}
		if (sizeHint < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sizeHint);
		}
		AllocateMemory(sizeHint);
		return _tailMemory;
	}

	public override Span<byte> GetSpan(int sizeHint = 0)
	{
		if (_isCompleted)
		{
			ThrowHelper.ThrowInvalidOperationException_NoWritingAllowed();
		}
		if (sizeHint < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sizeHint);
		}
		AllocateMemory(sizeHint);
		return _tailMemory.Span;
	}

	private void AllocateMemory(int sizeHint)
	{
		if (_head == null)
		{
			BufferSegment tail = AllocateSegment(sizeHint);
			_head = (_tail = tail);
			_tailBytesBuffered = 0;
			return;
		}
		int length = _tailMemory.Length;
		if (length == 0 || length < sizeHint)
		{
			if (_tailBytesBuffered > 0)
			{
				_tail.End += _tailBytesBuffered;
				_tailBytesBuffered = 0;
			}
			BufferSegment bufferSegment = AllocateSegment(sizeHint);
			_tail.SetNext(bufferSegment);
			_tail = bufferSegment;
		}
	}

	private BufferSegment AllocateSegment(int sizeHint)
	{
		BufferSegment bufferSegment = CreateSegmentUnsynchronized();
		int maxPooledBufferSize = _maxPooledBufferSize;
		if (sizeHint <= maxPooledBufferSize)
		{
			bufferSegment.SetOwnedMemory(_pool.Rent(GetSegmentSize(sizeHint, maxPooledBufferSize)));
		}
		else
		{
			int segmentSize = GetSegmentSize(sizeHint);
			bufferSegment.SetOwnedMemory(ArrayPool<byte>.Shared.Rent(segmentSize));
		}
		_tailMemory = bufferSegment.AvailableMemory;
		return bufferSegment;
	}

	private int GetSegmentSize(int sizeHint, int maxBufferSize = int.MaxValue)
	{
		sizeHint = Math.Max(_minimumBufferSize, sizeHint);
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

	public override void CancelPendingFlush()
	{
		Cancel();
	}

	public override void Complete(Exception? exception = null)
	{
		if (_isCompleted)
		{
			return;
		}
		_isCompleted = true;
		try
		{
			FlushInternal(exception == null);
		}
		finally
		{
			_internalTokenSource?.Dispose();
			if (!_leaveOpen)
			{
				InnerStream.Dispose();
			}
		}
	}

	public override async ValueTask CompleteAsync(Exception? exception = null)
	{
		if (_isCompleted)
		{
			return;
		}
		_isCompleted = true;
		try
		{
			await FlushAsyncInternal(exception == null, Memory<byte>.Empty).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			_internalTokenSource?.Dispose();
			if (!_leaveOpen)
			{
				await InnerStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}

	public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_bytesBuffered == 0L)
		{
			return new ValueTask<FlushResult>(new FlushResult(isCanceled: false, isCompleted: false));
		}
		return FlushAsyncInternal(writeToStream: true, Memory<byte>.Empty, cancellationToken);
	}

	public override ValueTask<FlushResult> WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default(CancellationToken))
	{
		return FlushAsyncInternal(writeToStream: true, source, cancellationToken);
	}

	private void Cancel()
	{
		InternalTokenSource.Cancel();
	}

	[AsyncStateMachine(typeof(_003CFlushAsyncInternal_003Ed__39))]
	[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
	private ValueTask<FlushResult> FlushAsyncInternal(bool writeToStream, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default(CancellationToken))
	{
		Unsafe.SkipInit(out _003CFlushAsyncInternal_003Ed__39 stateMachine);
		stateMachine._003C_003Et__builder = PoolingAsyncValueTaskMethodBuilder<FlushResult>.Create();
		stateMachine._003C_003E4__this = this;
		stateMachine.writeToStream = writeToStream;
		stateMachine.data = data;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	private void FlushInternal(bool writeToStream)
	{
		if (_tailBytesBuffered > 0)
		{
			_tail.End += _tailBytesBuffered;
			_tailBytesBuffered = 0;
		}
		BufferSegment bufferSegment = _head;
		while (bufferSegment != null)
		{
			BufferSegment bufferSegment2 = bufferSegment;
			bufferSegment = bufferSegment.NextSegment;
			if ((bufferSegment2.Length > 0) & writeToStream)
			{
				InnerStream.Write(bufferSegment2.Memory.Span);
			}
			ReturnSegmentUnsynchronized(bufferSegment2);
			_head = bufferSegment;
		}
		if ((_bytesBuffered > 0) & writeToStream)
		{
			InnerStream.Flush();
		}
		_head = null;
		_tail = null;
		_tailMemory = default(Memory<byte>);
		_bytesBuffered = 0L;
	}
}
