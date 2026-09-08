using System.Data.Common;
using System.IO;
using System.Runtime.CompilerServices;

namespace System.Data.SqlTypes;

internal sealed class StreamOnSqlBytes : Stream
{
	private SqlBytes _sb;

	private long _lPosition;

	public override bool CanRead
	{
		get
		{
			if (_sb != null)
			{
				return !_sb.IsNull;
			}
			return false;
		}
	}

	public override bool CanSeek => _sb != null;

	public override bool CanWrite
	{
		get
		{
			if (_sb != null)
			{
				if (_sb.IsNull)
				{
					return _sb._rgbBuf != null;
				}
				return true;
			}
			return false;
		}
	}

	public override long Length
	{
		get
		{
			CheckIfStreamClosed("get_Length");
			return _sb.Length;
		}
	}

	public override long Position
	{
		get
		{
			CheckIfStreamClosed("get_Position");
			return _lPosition;
		}
		set
		{
			CheckIfStreamClosed("set_Position");
			ArgumentOutOfRangeException.ThrowIfNegative(value, "value");
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, _sb.Length, "value");
			_lPosition = value;
		}
	}

	internal StreamOnSqlBytes(SqlBytes sb)
	{
		_sb = sb;
		_lPosition = 0L;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		CheckIfStreamClosed("Seek");
		switch (origin)
		{
		case SeekOrigin.Begin:
			ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
			ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, _sb.Length, "offset");
			_lPosition = offset;
			break;
		case SeekOrigin.Current:
		{
			long num = _lPosition + offset;
			ArgumentOutOfRangeException.ThrowIfNegative(num, "offset");
			ArgumentOutOfRangeException.ThrowIfGreaterThan(num, _sb.Length, "offset");
			_lPosition = num;
			break;
		}
		case SeekOrigin.End:
		{
			long num = _sb.Length + offset;
			ArgumentOutOfRangeException.ThrowIfNegative(num, "offset");
			ArgumentOutOfRangeException.ThrowIfGreaterThan(num, _sb.Length, "offset");
			_lPosition = num;
			break;
		}
		default:
			throw ADP.InvalidSeekOrigin("offset");
		}
		return _lPosition;
	}

	public override int Read(Span<byte> buffer)
	{
		CheckIfStreamClosed("Read");
		return ReadNoValidation(buffer);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		CheckIfStreamClosed("Read");
		Stream.ValidateBufferArguments(buffer, offset, count);
		return ReadNoValidation(buffer.AsSpan(offset, count));
	}

	private int ReadNoValidation(Span<byte> buffer)
	{
		int num = (int)_sb.Read(_lPosition, buffer);
		_lPosition += num;
		return num;
	}

	public override void Write(ReadOnlySpan<byte> buffer)
	{
		CheckIfStreamClosed("Write");
		WriteNoValidation(buffer);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		CheckIfStreamClosed("Write");
		Stream.ValidateBufferArguments(buffer, offset, count);
		WriteNoValidation(buffer);
	}

	private void WriteNoValidation(ReadOnlySpan<byte> buffer)
	{
		_sb.Write(_lPosition, buffer);
		_lPosition += buffer.Length;
	}

	public override int ReadByte()
	{
		CheckIfStreamClosed("ReadByte");
		if (_lPosition >= _sb.Length)
		{
			return -1;
		}
		byte result = _sb[_lPosition];
		_lPosition++;
		return result;
	}

	public override void WriteByte(byte value)
	{
		CheckIfStreamClosed("WriteByte");
		_sb[_lPosition] = value;
		_lPosition++;
	}

	public override void SetLength(long value)
	{
		CheckIfStreamClosed("SetLength");
		_sb.SetLength(value);
		if (_lPosition > value)
		{
			_lPosition = value;
		}
	}

	public override void Flush()
	{
		if (_sb.FStream())
		{
			_sb._stream.Flush();
		}
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			_sb = null;
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	private bool FClosed()
	{
		return _sb == null;
	}

	private void CheckIfStreamClosed([CallerMemberName] string methodname = "")
	{
		if (FClosed())
		{
			throw ADP.StreamClosed(methodname);
		}
	}
}
