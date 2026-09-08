using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

internal abstract class DelegatedStream : Stream
{
	private readonly Stream _stream;

	protected Stream BaseStream => _stream;

	public override bool CanSeek => _stream.CanSeek;

	public abstract override bool CanRead { get; }

	public abstract override bool CanWrite { get; }

	public override long Length
	{
		get
		{
			if (!CanSeek)
			{
				throw new NotSupportedException(System.SR.SeekNotSupported);
			}
			return _stream.Length;
		}
	}

	public override long Position
	{
		get
		{
			if (!CanSeek)
			{
				throw new NotSupportedException(System.SR.SeekNotSupported);
			}
			return _stream.Position;
		}
		set
		{
			if (!CanSeek)
			{
				throw new NotSupportedException(System.SR.SeekNotSupported);
			}
			_stream.Position = value;
		}
	}

	protected DelegatedStream(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		_stream = stream;
	}

	public sealed override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		return TaskToAsyncResult.Begin(ReadAsync(buffer, offset, count, CancellationToken.None), callback, state);
	}

	public sealed override int EndRead(IAsyncResult asyncResult)
	{
		return TaskToAsyncResult.End<int>(asyncResult);
	}

	public sealed override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		return TaskToAsyncResult.Begin(WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);
	}

	public sealed override void EndWrite(IAsyncResult asyncResult)
	{
		TaskToAsyncResult.End(asyncResult);
	}

	public override void Close()
	{
		_stream.Close();
		base.Close();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_stream.Dispose();
		}
		base.Dispose(disposing);
	}

	public override void Flush()
	{
		_stream.Flush();
	}

	public override Task FlushAsync(CancellationToken cancellationToken)
	{
		return _stream.FlushAsync(cancellationToken);
	}

	protected abstract int ReadInternal(Span<byte> buffer);

	protected abstract ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken);

	protected abstract void WriteInternal(ReadOnlySpan<byte> buffer);

	protected abstract ValueTask WriteAsyncInternal(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);

	public sealed override int Read(Span<byte> buffer)
	{
		if (!CanRead)
		{
			throw new NotSupportedException(System.SR.ReadNotSupported);
		}
		return ReadInternal(buffer);
	}

	public sealed override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!CanRead)
		{
			throw new NotSupportedException(System.SR.ReadNotSupported);
		}
		return ReadAsyncInternal(buffer, cancellationToken);
	}

	public sealed override int Read(byte[] buffer, int offset, int count)
	{
		Stream.ValidateBufferArguments(buffer, offset, count);
		if (!CanRead)
		{
			throw new NotSupportedException(System.SR.ReadNotSupported);
		}
		return ReadInternal(buffer.AsSpan(offset, count));
	}

	public sealed override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		Stream.ValidateBufferArguments(buffer, offset, count);
		if (!CanRead)
		{
			throw new NotSupportedException(System.SR.ReadNotSupported);
		}
		return ReadAsyncInternal(buffer.AsMemory(offset, count), cancellationToken).AsTask();
	}

	public sealed override int ReadByte()
	{
		if (!CanRead)
		{
			throw new NotSupportedException(System.SR.ReadNotSupported);
		}
		byte reference = 0;
		if (ReadInternal(new Span<byte>(ref reference)) == 0)
		{
			return -1;
		}
		return reference;
	}

	public sealed override long Seek(long offset, SeekOrigin origin)
	{
		if (!CanSeek)
		{
			throw new NotSupportedException(System.SR.SeekNotSupported);
		}
		return _stream.Seek(offset, origin);
	}

	public sealed override void SetLength(long value)
	{
		if (!CanSeek)
		{
			throw new NotSupportedException(System.SR.SeekNotSupported);
		}
		_stream.SetLength(value);
	}

	public sealed override void Write(ReadOnlySpan<byte> buffer)
	{
		if (!CanWrite)
		{
			throw new NotSupportedException(System.SR.WriteNotSupported);
		}
		WriteInternal(buffer);
	}

	public sealed override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!CanWrite)
		{
			throw new NotSupportedException(System.SR.WriteNotSupported);
		}
		return WriteAsyncInternal(buffer, cancellationToken);
	}

	public sealed override void Write(byte[] buffer, int offset, int count)
	{
		Stream.ValidateBufferArguments(buffer, offset, count);
		if (!CanWrite)
		{
			throw new NotSupportedException(System.SR.WriteNotSupported);
		}
		WriteInternal(buffer.AsSpan(offset, count));
	}

	public sealed override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		Stream.ValidateBufferArguments(buffer, offset, count);
		if (!CanWrite)
		{
			throw new NotSupportedException(System.SR.WriteNotSupported);
		}
		return WriteAsyncInternal(buffer.AsMemory(offset, count), cancellationToken).AsTask();
	}
}
