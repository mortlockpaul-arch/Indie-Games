using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

internal sealed class RequestBufferingStream : Stream
{
	private bool _disposed;

	private readonly MemoryStream _buffer = new MemoryStream();

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

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

	public override void Flush()
	{
		ThrowIfDisposed();
	}

	public override Task FlushAsync(CancellationToken cancellationToken)
	{
		ThrowIfDisposed();
		if (!cancellationToken.IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		return Task.FromCanceled(cancellationToken);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
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
		ThrowIfDisposed();
		Stream.ValidateBufferArguments(buffer, offset, count);
		_buffer.Write(buffer, offset, count);
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		ThrowIfDisposed();
		Stream.ValidateBufferArguments(buffer, offset, count);
		return _buffer.WriteAsync(buffer, offset, count, cancellationToken);
	}

	public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfDisposed();
		return _buffer.WriteAsync(buffer, cancellationToken);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
	{
		ThrowIfDisposed();
		Stream.ValidateBufferArguments(buffer, offset, count);
		return _buffer.BeginWrite(buffer, offset, count, asyncCallback, asyncState);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		ThrowIfDisposed();
		_buffer.EndWrite(asyncResult);
	}

	public ArraySegment<byte> GetBuffer()
	{
		_buffer.TryGetBuffer(out var buffer);
		return buffer;
	}

	public MemoryStream GetMemoryStream()
	{
		return _buffer;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && !_disposed)
		{
			_disposed = true;
		}
		base.Dispose(disposing);
	}

	private void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, this);
	}
}
