using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net;

internal sealed class BufferedReadStream : DelegatedStream
{
	private byte[] _storedBuffer;

	private int _storedLength;

	private int _storedOffset;

	private readonly bool _readMore;

	public override bool CanWrite => false;

	public override bool CanRead => base.BaseStream.CanRead;

	public override bool CanSeek => false;

	internal BufferedReadStream(Stream stream)
		: this(stream, readMore: false)
	{
	}

	internal BufferedReadStream(Stream stream, bool readMore)
		: base(stream)
	{
		_readMore = readMore;
	}

	protected override int ReadInternal(Span<byte> buffer)
	{
		if (_storedOffset < _storedLength)
		{
			int num = Math.Min(buffer.Length, _storedLength - _storedOffset);
			_storedBuffer.AsSpan(_storedOffset, num).CopyTo(buffer);
			_storedOffset += num;
			if (num == buffer.Length || !_readMore)
			{
				return num;
			}
			return num + base.BaseStream.Read(buffer.Slice(num));
		}
		return base.BaseStream.Read(buffer);
	}

	protected override ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_storedOffset >= _storedLength)
		{
			return base.BaseStream.ReadAsync(buffer, cancellationToken);
		}
		int num = Math.Min(buffer.Length, _storedLength - _storedOffset);
		_storedBuffer.AsMemory(_storedOffset, num).CopyTo(buffer);
		_storedOffset += num;
		if (num == buffer.Length || !_readMore)
		{
			return new ValueTask<int>(num);
		}
		return ReadMoreAsync(num, buffer.Slice(num), cancellationToken);
	}

	private async ValueTask<int> ReadMoreAsync(int bytesAlreadyRead, Memory<byte> buffer, CancellationToken cancellationToken)
	{
		return bytesAlreadyRead + await base.BaseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	protected override void WriteInternal(ReadOnlySpan<byte> buffer)
	{
		throw new NotImplementedException();
	}

	protected override ValueTask WriteAsyncInternal(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		throw new NotImplementedException();
	}

	internal void Push(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length == 0)
		{
			return;
		}
		int length = buffer.Length;
		if (_storedOffset == _storedLength)
		{
			if (_storedBuffer == null || _storedBuffer.Length < length)
			{
				_storedBuffer = new byte[length];
			}
			_storedOffset = 0;
			_storedLength = length;
		}
		else if (length <= _storedOffset)
		{
			_storedOffset -= length;
		}
		else if (length <= _storedBuffer.Length - _storedLength + _storedOffset)
		{
			Buffer.BlockCopy(_storedBuffer, _storedOffset, _storedBuffer, length, _storedLength - _storedOffset);
			_storedLength += length - _storedOffset;
			_storedOffset = 0;
		}
		else
		{
			byte[] array = new byte[length + _storedLength - _storedOffset];
			Buffer.BlockCopy(_storedBuffer, _storedOffset, array, length, _storedLength - _storedOffset);
			_storedLength += length - _storedOffset;
			_storedOffset = 0;
			_storedBuffer = array;
		}
		buffer.CopyTo(_storedBuffer.AsSpan(_storedOffset));
	}
}
