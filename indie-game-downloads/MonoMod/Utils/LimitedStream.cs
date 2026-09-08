using System;
using System.IO;

namespace MonoMod.Utils;

public class LimitedStream : MemoryStream
{
	public Stream LimitStream;

	public long LimitOffset;

	public long LimitLength;

	public long? LimitPublicLength;

	public bool LimitStreamShared;

	private long _Position;

	protected byte[] CachedBuffer;

	protected long CachedOffset;

	protected long CachedLength;

	private bool _CacheBuffer = true;

	private readonly byte[] _ToArrayReadBuffer = new byte[2048];

	public bool CacheBuffer
	{
		get
		{
			return _CacheBuffer;
		}
		set
		{
			if (!value)
			{
				CachedBuffer = null;
			}
			_CacheBuffer = value;
		}
	}

	public override bool CanRead => LimitStream.CanRead;

	public override bool CanSeek => LimitStream.CanSeek;

	public override bool CanWrite => LimitStream.CanWrite;

	public override long Length => LimitPublicLength ?? LimitLength;

	public override long Position
	{
		get
		{
			if (!LimitStreamShared && CanSeek)
			{
				return LimitStream.Position - LimitOffset;
			}
			return _Position;
		}
		set
		{
			if (CanSeek)
			{
				LimitStream.Position = value + LimitOffset;
			}
			_Position = value;
		}
	}

	public LimitedStream(Stream stream, long offset, long length)
	{
		LimitStream = stream;
		LimitOffset = offset;
		LimitLength = length;
		if (LimitStream.CanSeek)
		{
			LimitStream.Seek(offset, SeekOrigin.Begin);
		}
	}

	public override void Flush()
	{
		LimitStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (LimitOffset + LimitLength <= Position)
		{
			return 0;
		}
		if (LimitOffset + LimitLength <= Position + count)
		{
			count = (int)(LimitLength - (Position - LimitOffset));
		}
		int num = LimitStream.Read(buffer, offset, count);
		_Position += num;
		return num;
	}

	public override int ReadByte()
	{
		if (LimitOffset + LimitLength <= Position)
		{
			return 0;
		}
		if (LimitOffset + LimitLength <= Position + 1)
		{
			return 0;
		}
		int num = LimitStream.ReadByte();
		if (num != -1)
		{
			_Position++;
		}
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		if (!CanSeek)
		{
			throw new NotSupportedException("This stream does not support seek operations.");
		}
		switch (origin)
		{
		case SeekOrigin.Begin:
			if (LimitOffset + LimitLength <= offset)
			{
				throw new Exception("out of something");
			}
			_Position = offset;
			return LimitStream.Seek(LimitOffset + offset, SeekOrigin.Begin);
		case SeekOrigin.Current:
			if (LimitOffset + LimitLength <= Position + offset)
			{
				throw new Exception("out of something");
			}
			_Position += offset;
			return LimitStream.Seek(offset, SeekOrigin.Current);
		case SeekOrigin.End:
			if (LimitLength - offset < 0)
			{
				throw new Exception("out of something");
			}
			_Position = LimitLength - offset;
			return LimitStream.Seek(LimitOffset + LimitLength - offset, SeekOrigin.Begin);
		default:
			return 0L;
		}
	}

	public override void SetLength(long value)
	{
		if (!CanSeek)
		{
			throw new NotSupportedException("This stream does not support seek operations.");
		}
		if (LimitStreamShared)
		{
			LimitLength = value;
		}
		else
		{
			LimitStream.SetLength(LimitOffset + value + LimitLength);
		}
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (LimitOffset + LimitLength <= Position + count)
		{
			throw new Exception("out of something");
		}
		LimitStream.Write(buffer, offset, count);
		_Position += count;
	}

	public override byte[] GetBuffer()
	{
		if (CachedBuffer != null && CachedOffset == LimitOffset && CachedLength == LimitLength)
		{
			return CachedBuffer;
		}
		if (!_CacheBuffer)
		{
			return ToArray();
		}
		CachedOffset = LimitOffset;
		CachedLength = LimitLength;
		return CachedBuffer = ToArray();
	}

	public override byte[] ToArray()
	{
		long position = LimitStream.Position;
		if (LimitStream.CanSeek)
		{
			LimitStream.Seek(LimitOffset, SeekOrigin.Begin);
		}
		long num = ((LimitLength == 0L) ? LimitStream.Length : LimitLength);
		num -= LimitStream.Position - LimitOffset;
		byte[] result;
		int count;
		if (num == 0L)
		{
			MemoryStream memoryStream = new MemoryStream();
			while (0 < (count = LimitStream.Read(_ToArrayReadBuffer, 0, _ToArrayReadBuffer.Length)))
			{
				base.Write(_ToArrayReadBuffer, 0, count);
			}
			LimitStream.Seek(position, SeekOrigin.Begin);
			result = base.ToArray();
			memoryStream.Close();
			return result;
		}
		result = new byte[num];
		for (int i = 0; i < num; i += count)
		{
			count = LimitStream.Read(result, i, result.Length - i);
		}
		if (LimitStream.CanSeek)
		{
			LimitStream.Seek(position, SeekOrigin.Begin);
		}
		return result;
	}

	public override void Close()
	{
		base.Close();
		if (!LimitStreamShared)
		{
			LimitStream.Close();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!LimitStreamShared)
		{
			LimitStream.Dispose();
			LimitStream.Close();
		}
		base.Dispose(disposing);
	}
}
