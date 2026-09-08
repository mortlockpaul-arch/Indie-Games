using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Pipelines;

internal sealed class PipeReaderStream : Stream
{
	private readonly PipeReader _pipeReader;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	internal bool LeaveOpen { get; set; }

	public PipeReaderStream(PipeReader pipeReader, bool leaveOpen)
	{
		_pipeReader = pipeReader;
		LeaveOpen = leaveOpen;
	}

	protected override void Dispose(bool disposing)
	{
		if (!LeaveOpen)
		{
			_pipeReader.Complete();
		}
		base.Dispose(disposing);
	}

	public override void Flush()
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.buffer);
		}
		return ReadInternal(new Span<byte>(buffer, offset, count));
	}

	public override int ReadByte()
	{
		Span<byte> buffer = stackalloc byte[1];
		if (ReadInternal(buffer) != 0)
		{
			return buffer[0];
		}
		return -1;
	}

	private int ReadInternal(Span<byte> buffer)
	{
		ValueTask<ReadResult> valueTask = _pipeReader.ReadAsync();
		ReadResult result = (valueTask.IsCompletedSuccessfully ? valueTask.Result : valueTask.AsTask().GetAwaiter().GetResult());
		return HandleReadResult(result, buffer);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}

	public sealed override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
	{
		return TaskToAsyncResult.Begin(ReadAsync(buffer, offset, count, default(CancellationToken)), callback, state);
	}

	public sealed override int EndRead(IAsyncResult asyncResult)
	{
		return TaskToAsyncResult.End<int>(asyncResult);
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.buffer);
		}
		return ReadAsyncInternal(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
	}

	public override int Read(Span<byte> buffer)
	{
		return ReadInternal(buffer);
	}

	public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ReadAsyncInternal(buffer, cancellationToken);
	}

	private async ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken)
	{
		return HandleReadResult(await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false), buffer.Span);
	}

	private int HandleReadResult(ReadResult result, Span<byte> buffer)
	{
		if (result.IsCanceled)
		{
			ThrowHelper.ThrowOperationCanceledException_ReadCanceled();
		}
		ReadOnlySequence<byte> buffer2 = result.Buffer;
		long length = buffer2.Length;
		SequencePosition consumed = buffer2.Start;
		try
		{
			if (length != 0L)
			{
				int num = (int)Math.Min(length, buffer.Length);
				ReadOnlySequence<byte> source = ((num == length) ? buffer2 : buffer2.Slice(0, num));
				consumed = source.End;
				source.CopyTo(buffer);
				return num;
			}
			if (result.IsCompleted)
			{
				return 0;
			}
		}
		finally
		{
			_pipeReader.AdvanceTo(consumed);
		}
		ThrowHelper.ThrowInvalidOperationException_InvalidZeroByteRead();
		return 0;
	}

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
	{
		StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
		return _pipeReader.CopyToAsync(destination, cancellationToken);
	}
}
