using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json;

internal sealed class PooledByteBufferWriter : PipeWriter, IDisposable
{
	private ArrayBuffer _buffer;

	private readonly Stream _stream;

	public ReadOnlySpan<byte> WrittenSpan => _buffer.ActiveSpan;

	public ReadOnlyMemory<byte> WrittenMemory => _buffer.ActiveMemory;

	public int Capacity => _buffer.Capacity;

	public override bool CanGetUnflushedBytes => true;

	public override long UnflushedBytes => _buffer.ActiveLength;

	public PooledByteBufferWriter(int initialCapacity)
	{
		_buffer = new ArrayBuffer(initialCapacity, usePool: true);
	}

	public PooledByteBufferWriter(int initialCapacity, Stream stream)
		: this(initialCapacity)
	{
		_stream = stream;
	}

	public void Clear()
	{
		_buffer.Discard(_buffer.ActiveLength);
	}

	public void ClearAndReturnBuffers()
	{
		_buffer.ClearAndReturnBuffer();
	}

	public void Dispose()
	{
		_buffer.Dispose();
	}

	public void InitializeEmptyInstance(int initialCapacity)
	{
		_buffer.EnsureAvailableSpace(initialCapacity);
	}

	public static PooledByteBufferWriter CreateEmptyInstanceForCaching()
	{
		return new PooledByteBufferWriter(0);
	}

	public override void Advance(int count)
	{
		_buffer.Commit(count);
	}

	public override Memory<byte> GetMemory(int sizeHint = 256)
	{
		_buffer.EnsureAvailableSpace(sizeHint);
		return _buffer.AvailableMemory;
	}

	public override Span<byte> GetSpan(int sizeHint = 256)
	{
		_buffer.EnsureAvailableSpace(sizeHint);
		return _buffer.AvailableSpan;
	}

	internal void WriteToStream(Stream destination)
	{
		destination.Write(_buffer.ActiveSpan);
	}

	public override async ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		await _stream.WriteAsync(WrittenMemory, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		Clear();
		return new FlushResult(isCanceled: false, isCompleted: false);
	}

	public override void CancelPendingFlush()
	{
		throw new NotImplementedException();
	}

	public override void Complete(Exception exception = null)
	{
		throw new NotImplementedException();
	}
}
