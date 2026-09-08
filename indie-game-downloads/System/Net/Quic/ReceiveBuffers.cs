using Microsoft.Quic;

namespace System.Net.Quic;

internal struct ReceiveBuffers
{
	private readonly object _syncRoot = new object();

	private MultiArrayBuffer _buffer = default(MultiArrayBuffer);

	private bool _final = false;

	public ReceiveBuffers()
	{
	}

	public void SetFinal()
	{
		lock (_syncRoot)
		{
			_final = true;
		}
	}

	public bool HasCapacity()
	{
		lock (_syncRoot)
		{
			return _buffer.ActiveMemory.Length < 65536;
		}
	}

	public int CopyFrom(ReadOnlySpan<QUIC_BUFFER> quicBuffers, int totalLength, bool final)
	{
		lock (_syncRoot)
		{
			if (_buffer.ActiveMemory.Length > 65536 - totalLength)
			{
				totalLength = 65536 - _buffer.ActiveMemory.Length;
				final = false;
			}
			_final = final;
			_buffer.EnsureAvailableSpace(totalLength);
			int num = 0;
			for (int i = 0; i < quicBuffers.Length; i++)
			{
				Span<byte> span = quicBuffers[i].Span;
				if (totalLength < span.Length)
				{
					span = span.Slice(0, totalLength);
				}
				_buffer.AvailableMemory.CopyFrom(span);
				_buffer.Commit(span.Length);
				num += span.Length;
				totalLength -= span.Length;
			}
			return num;
		}
	}

	public int CopyTo(Memory<byte> buffer, out bool completed, out bool empty)
	{
		lock (_syncRoot)
		{
			int num = 0;
			if (!_buffer.IsEmpty)
			{
				MultiMemory activeMemory = _buffer.ActiveMemory;
				num = Math.Min(buffer.Length, activeMemory.Length);
				activeMemory.Slice(0, num).CopyTo(buffer.Span);
				_buffer.Discard(num);
			}
			completed = _buffer.IsEmpty && _final;
			empty = _buffer.IsEmpty;
			return num;
		}
	}
}
