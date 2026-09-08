using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines;

public abstract class PipeReader
{
	private PipeReaderStream _stream;

	public abstract bool TryRead(out ReadResult result);

	public abstract ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default(CancellationToken));

	public ValueTask<ReadResult> ReadAtLeastAsync(int minimumSize, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (minimumSize < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.minimumSize);
		}
		return ReadAtLeastAsyncCore(minimumSize, cancellationToken);
	}

	protected virtual async ValueTask<ReadResult> ReadAtLeastAsyncCore(int minimumSize, CancellationToken cancellationToken)
	{
		ReadResult result;
		while (true)
		{
			result = await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ReadOnlySequence<byte> buffer = result.Buffer;
			if (buffer.Length >= minimumSize || result.IsCompleted || result.IsCanceled)
			{
				break;
			}
			AdvanceTo(buffer.Start, buffer.End);
		}
		return result;
	}

	public abstract void AdvanceTo(SequencePosition consumed);

	public abstract void AdvanceTo(SequencePosition consumed, SequencePosition examined);

	public virtual Stream AsStream(bool leaveOpen = false)
	{
		if (_stream == null)
		{
			_stream = new PipeReaderStream(this, leaveOpen);
		}
		else if (leaveOpen)
		{
			_stream.LeaveOpen = leaveOpen;
		}
		return _stream;
	}

	public abstract void CancelPendingRead();

	public abstract void Complete(Exception? exception = null);

	public virtual ValueTask CompleteAsync(Exception? exception = null)
	{
		try
		{
			Complete(exception);
			return default(ValueTask);
		}
		catch (Exception exception2)
		{
			return new ValueTask(Task.FromException(exception2));
		}
	}

	[Obsolete("OnWriterCompleted has been deprecated and may not be invoked on all implementations of PipeReader.")]
	public virtual void OnWriterCompleted(Action<Exception?, object?> callback, object? state)
	{
	}

	public static PipeReader Create(Stream stream, StreamPipeReaderOptions? readerOptions = null)
	{
		return new StreamPipeReader(stream, readerOptions ?? StreamPipeReaderOptions.s_default);
	}

	public static PipeReader Create(ReadOnlySequence<byte> sequence)
	{
		return new SequencePipeReader(sequence);
	}

	public virtual Task CopyToAsync(PipeWriter destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (destination == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		return CopyToAsyncCore(destination, (PipeWriter pipeWriter, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken2) => pipeWriter.WriteAsync(memory, cancellationToken2), cancellationToken);
	}

	public virtual Task CopyToAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (destination == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.destination);
		}
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		return CopyToAsyncCore(destination, delegate(Stream stream, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken2)
		{
			ValueTask writeTask = stream.WriteAsync(memory, cancellationToken2);
			if (writeTask.IsCompletedSuccessfully)
			{
				writeTask.GetAwaiter().GetResult();
				return new ValueTask<FlushResult>(new FlushResult(isCanceled: false, isCompleted: false));
			}
			return Awaited(writeTask);
		}, cancellationToken);
		static async ValueTask<FlushResult> Awaited(ValueTask writeTask)
		{
			await writeTask.ConfigureAwait(continueOnCapturedContext: false);
			return new FlushResult(isCanceled: false, isCompleted: false);
		}
	}

	private async Task CopyToAsyncCore<TStream>(TStream destination, Func<TStream, ReadOnlyMemory<byte>, CancellationToken, ValueTask<FlushResult>> writeAsync, CancellationToken cancellationToken)
	{
		while (true)
		{
			ReadResult result = await ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			ReadOnlySequence<byte> buffer = result.Buffer;
			SequencePosition position = buffer.Start;
			SequencePosition consumed = position;
			try
			{
				if (result.IsCanceled)
				{
					ThrowHelper.ThrowOperationCanceledException_ReadCanceled();
				}
				ReadOnlyMemory<byte> memory;
				while (buffer.TryGet(ref position, out memory))
				{
					if (memory.IsEmpty)
					{
						consumed = position;
						continue;
					}
					FlushResult flushResult = await writeAsync(destination, memory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (flushResult.IsCanceled)
					{
						ThrowHelper.ThrowOperationCanceledException_FlushCanceled();
					}
					consumed = position;
					if (!flushResult.IsCompleted)
					{
						continue;
					}
					return;
				}
				consumed = buffer.End;
				if (result.IsCompleted)
				{
					break;
				}
			}
			finally
			{
				AdvanceTo(consumed);
			}
		}
	}
}
