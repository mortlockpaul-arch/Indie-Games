using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

internal sealed class ClosableStream : DelegatedStream
{
	private readonly EventHandler _onClose;

	private int _closed;

	public override bool CanRead => base.BaseStream.CanRead;

	public override bool CanWrite => base.BaseStream.CanWrite;

	internal ClosableStream(Stream stream, EventHandler onClose)
		: base(stream)
	{
		_onClose = onClose;
	}

	public override void Close()
	{
		if (Interlocked.Increment(ref _closed) == 1)
		{
			_onClose?.Invoke(this, new EventArgs());
		}
	}

	protected override void WriteInternal(ReadOnlySpan<byte> buffer)
	{
		base.BaseStream.Write(buffer);
	}

	protected override ValueTask WriteAsyncInternal(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		return base.BaseStream.WriteAsync(buffer, cancellationToken);
	}

	protected override int ReadInternal(Span<byte> buffer)
	{
		return base.BaseStream.Read(buffer);
	}

	protected override ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		return base.BaseStream.ReadAsync(buffer, cancellationToken);
	}
}
