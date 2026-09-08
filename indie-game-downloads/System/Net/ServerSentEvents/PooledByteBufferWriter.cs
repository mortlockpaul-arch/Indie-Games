using System.Buffers;

namespace System.Net.ServerSentEvents;

internal sealed class PooledByteBufferWriter : IBufferWriter<byte>, IDisposable
{
	private ArrayBuffer _buffer = new ArrayBuffer(256, usePool: true);

	public ReadOnlyMemory<byte> WrittenMemory => _buffer.ActiveMemory;

	public void Advance(int count)
	{
		_buffer.Commit(count);
	}

	public Memory<byte> GetMemory(int sizeHint = 0)
	{
		_buffer.EnsureAvailableSpace(Math.Max(sizeHint, 256));
		return _buffer.AvailableMemory;
	}

	public Span<byte> GetSpan(int sizeHint = 0)
	{
		_buffer.EnsureAvailableSpace(Math.Max(sizeHint, 256));
		return _buffer.AvailableSpan;
	}

	public void Reset()
	{
		_buffer.Discard(_buffer.ActiveLength);
	}

	public void Dispose()
	{
		_buffer.Dispose();
	}
}
