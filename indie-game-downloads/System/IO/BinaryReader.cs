using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.IO;

public class BinaryReader : IDisposable
{
	private readonly Stream _stream;

	private readonly Encoding _encoding;

	private Decoder _decoder;

	private char[] _charBuffer;

	private readonly int _maxCharsSize;

	private readonly bool _2BytesPerChar;

	private readonly bool _isMemoryStream;

	private readonly bool _leaveOpen;

	private bool _disposed;

	public virtual Stream BaseStream => _stream;

	public BinaryReader(Stream input)
		: this(input, Encoding.UTF8, leaveOpen: false)
	{
	}

	public BinaryReader(Stream input, Encoding encoding)
		: this(input, encoding, leaveOpen: false)
	{
	}

	public BinaryReader(Stream input, Encoding encoding, bool leaveOpen)
	{
		ArgumentNullException.ThrowIfNull(input, "input");
		ArgumentNullException.ThrowIfNull(encoding, "encoding");
		if (!input.CanRead)
		{
			throw new ArgumentException(SR.Argument_StreamNotReadable);
		}
		_stream = input;
		_encoding = encoding;
		_maxCharsSize = encoding.GetMaxCharCount(128);
		_2BytesPerChar = encoding is UnicodeEncoding;
		_isMemoryStream = _stream.GetType() == typeof(MemoryStream);
		_leaveOpen = leaveOpen;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing && !_leaveOpen)
			{
				_stream.Close();
			}
			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	public virtual void Close()
	{
		Dispose(disposing: true);
	}

	private void ThrowIfDisposed()
	{
		if (_disposed)
		{
			ThrowHelper.ThrowObjectDisposedException_FileClosed();
		}
	}

	public virtual int PeekChar()
	{
		ThrowIfDisposed();
		if (!_stream.CanSeek)
		{
			return -1;
		}
		long position = _stream.Position;
		int result = Read();
		_stream.Position = position;
		return result;
	}

	public virtual int Read()
	{
		ThrowIfDisposed();
		int num = 0;
		long num2 = 0L;
		if (_stream.CanSeek)
		{
			num2 = _stream.Position;
		}
		if (_decoder == null)
		{
			_decoder = _encoding.GetDecoder();
		}
		Span<byte> span = stackalloc byte[128];
		char reference = '\0';
		while (num == 0)
		{
			int num3 = ((!_2BytesPerChar) ? 1 : 2);
			int num4 = _stream.ReadByte();
			span[0] = (byte)num4;
			if (num4 == -1)
			{
				num3 = 0;
			}
			if (num3 == 2)
			{
				num4 = _stream.ReadByte();
				span[1] = (byte)num4;
				if (num4 == -1)
				{
					num3 = 1;
				}
			}
			if (num3 == 0)
			{
				return -1;
			}
			try
			{
				num = _decoder.GetChars(span.Slice(0, num3), new Span<char>(ref reference), flush: false);
			}
			catch
			{
				if (_stream.CanSeek)
				{
					_stream.Seek(num2 - _stream.Position, SeekOrigin.Current);
				}
				throw;
			}
		}
		return reference;
	}

	public virtual byte ReadByte()
	{
		return InternalReadByte();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private byte InternalReadByte()
	{
		ThrowIfDisposed();
		int num = _stream.ReadByte();
		if (num == -1)
		{
			ThrowHelper.ThrowEndOfFileException();
		}
		return (byte)num;
	}

	[CLSCompliant(false)]
	public virtual sbyte ReadSByte()
	{
		return (sbyte)InternalReadByte();
	}

	public virtual bool ReadBoolean()
	{
		return InternalReadByte() != 0;
	}

	public virtual char ReadChar()
	{
		int num = Read();
		if (num == -1)
		{
			ThrowHelper.ThrowEndOfFileException();
		}
		return (char)num;
	}

	public virtual short ReadInt16()
	{
		Span<byte> buffer = stackalloc byte[2];
		return BinaryPrimitives.ReadInt16LittleEndian(InternalRead(buffer));
	}

	[CLSCompliant(false)]
	public virtual ushort ReadUInt16()
	{
		Span<byte> buffer = stackalloc byte[2];
		return BinaryPrimitives.ReadUInt16LittleEndian(InternalRead(buffer));
	}

	public virtual int ReadInt32()
	{
		Span<byte> buffer = stackalloc byte[4];
		return BinaryPrimitives.ReadInt32LittleEndian(InternalRead(buffer));
	}

	[CLSCompliant(false)]
	public virtual uint ReadUInt32()
	{
		Span<byte> buffer = stackalloc byte[4];
		return BinaryPrimitives.ReadUInt32LittleEndian(InternalRead(buffer));
	}

	public virtual long ReadInt64()
	{
		Span<byte> buffer = stackalloc byte[8];
		return BinaryPrimitives.ReadInt64LittleEndian(InternalRead(buffer));
	}

	[CLSCompliant(false)]
	public virtual ulong ReadUInt64()
	{
		Span<byte> buffer = stackalloc byte[8];
		return BinaryPrimitives.ReadUInt64LittleEndian(InternalRead(buffer));
	}

	public unsafe virtual Half ReadHalf()
	{
		Span<byte> buffer = stackalloc byte[sizeof(Half)];
		return BinaryPrimitives.ReadHalfLittleEndian(InternalRead(buffer));
	}

	public virtual float ReadSingle()
	{
		Span<byte> buffer = stackalloc byte[4];
		return BinaryPrimitives.ReadSingleLittleEndian(InternalRead(buffer));
	}

	public virtual double ReadDouble()
	{
		Span<byte> buffer = stackalloc byte[8];
		return BinaryPrimitives.ReadDoubleLittleEndian(InternalRead(buffer));
	}

	public virtual decimal ReadDecimal()
	{
		Span<byte> buffer = stackalloc byte[16];
		ReadOnlySpan<byte> span = InternalRead(buffer);
		try
		{
			return decimal.ToDecimal(span);
		}
		catch (ArgumentException innerException)
		{
			throw new IOException(SR.Arg_DecBitCtor, innerException);
		}
	}

	public virtual string ReadString()
	{
		ThrowIfDisposed();
		int num = Read7BitEncodedInt();
		if (num < 0)
		{
			throw new IOException(SR.Format(SR.IO_InvalidStringLen_Len, num));
		}
		if (num == 0)
		{
			return string.Empty;
		}
		Span<byte> span = stackalloc byte[128];
		int num2 = 0;
		StringBuilder stringBuilder = null;
		do
		{
			int length = Math.Min(128, num - num2);
			int num3 = _stream.Read(span.Slice(0, length));
			if (num3 == 0)
			{
				ThrowHelper.ThrowEndOfFileException();
			}
			if (num2 == 0 && num3 == num)
			{
				return _encoding.GetString(span.Slice(0, num3));
			}
			if (_decoder == null)
			{
				_decoder = _encoding.GetDecoder();
			}
			if (_charBuffer == null)
			{
				_charBuffer = new char[_maxCharsSize];
			}
			int chars = _decoder.GetChars(span.Slice(0, num3), _charBuffer, flush: false);
			if (stringBuilder == null)
			{
				stringBuilder = StringBuilderCache.Acquire(Math.Min(num, 360));
			}
			stringBuilder.Append(_charBuffer, 0, chars);
			num2 += num3;
		}
		while (num2 < num);
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	public virtual int Read(char[] buffer, int index, int count)
	{
		ArgumentNullException.ThrowIfNull(buffer, "buffer");
		ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		if (buffer.Length - index < count)
		{
			throw new ArgumentException(SR.Argument_InvalidOffLen);
		}
		ThrowIfDisposed();
		return InternalReadChars(new Span<char>(buffer, index, count));
	}

	public virtual int Read(Span<char> buffer)
	{
		ThrowIfDisposed();
		return InternalReadChars(buffer);
	}

	private int InternalReadChars(Span<char> buffer)
	{
		if (_decoder == null)
		{
			_decoder = _encoding.GetDecoder();
		}
		int num = 0;
		Span<byte> span = stackalloc byte[128];
		while (!buffer.IsEmpty)
		{
			int num2 = buffer.Length;
			if (_2BytesPerChar)
			{
				num2 <<= 1;
			}
			if (num2 > 1 && !(_decoder is DecoderNLS { HasState: false }))
			{
				num2--;
				if (_2BytesPerChar && num2 > 2)
				{
					num2 -= 2;
				}
			}
			ReadOnlySpan<byte> bytes;
			if (_isMemoryStream)
			{
				MemoryStream memoryStream = Unsafe.As<MemoryStream>(_stream);
				int start = memoryStream.InternalGetPosition();
				num2 = memoryStream.InternalEmulateRead(num2);
				bytes = new ReadOnlySpan<byte>(memoryStream.InternalGetBuffer(), start, num2);
			}
			else
			{
				if (num2 > 128)
				{
					num2 = 128;
				}
				bytes = span[.._stream.Read(span.Slice(0, num2))];
			}
			if (bytes.IsEmpty)
			{
				break;
			}
			int chars = _decoder.GetChars(bytes, buffer, flush: false);
			buffer = buffer.Slice(chars);
			num += chars;
		}
		return num;
	}

	public virtual char[] ReadChars(int count)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		ThrowIfDisposed();
		if (count == 0)
		{
			return Array.Empty<char>();
		}
		char[] array = new char[count];
		int num = InternalReadChars(new Span<char>(array));
		if (num != count)
		{
			array = array[..num];
		}
		return array;
	}

	public virtual int Read(byte[] buffer, int index, int count)
	{
		ArgumentNullException.ThrowIfNull(buffer, "buffer");
		ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		if (buffer.Length - index < count)
		{
			throw new ArgumentException(SR.Argument_InvalidOffLen);
		}
		ThrowIfDisposed();
		return _stream.Read(buffer, index, count);
	}

	public virtual int Read(Span<byte> buffer)
	{
		ThrowIfDisposed();
		return _stream.Read(buffer);
	}

	public virtual byte[] ReadBytes(int count)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		ThrowIfDisposed();
		if (count == 0)
		{
			return Array.Empty<byte>();
		}
		byte[] array = new byte[count];
		int num = _stream.ReadAtLeast(array, array.Length, throwOnEndOfStream: false);
		if (num != array.Length)
		{
			array = array[..num];
		}
		return array;
	}

	public virtual void ReadExactly(Span<byte> buffer)
	{
		ThrowIfDisposed();
		_stream.ReadExactly(buffer);
	}

	private ReadOnlySpan<byte> InternalRead(Span<byte> buffer)
	{
		if (_isMemoryStream)
		{
			return Unsafe.As<MemoryStream>(_stream).InternalReadSpan(buffer.Length);
		}
		ThrowIfDisposed();
		_stream.ReadExactly(buffer);
		return buffer;
	}

	protected virtual void FillBuffer(int numBytes)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(numBytes, "numBytes");
		ThrowIfDisposed();
		switch (numBytes)
		{
		case 0:
			if (_stream.Read(Array.Empty<byte>(), 0, 0) == 0)
			{
				ThrowHelper.ThrowEndOfFileException();
			}
			return;
		case 1:
			if (_stream.ReadByte() == -1)
			{
				ThrowHelper.ThrowEndOfFileException();
			}
			return;
		}
		if (_stream.CanSeek)
		{
			_stream.Seek(numBytes, SeekOrigin.Current);
			return;
		}
		byte[] array = ArrayPool<byte>.Shared.Rent(numBytes);
		_stream.ReadExactly(array.AsSpan(0, numBytes));
		ArrayPool<byte>.Shared.Return(array);
	}

	public int Read7BitEncodedInt()
	{
		uint num = 0u;
		byte b;
		for (int i = 0; i < 28; i += 7)
		{
			b = ReadByte();
			num |= (uint)((b & 0x7F) << i);
			if ((uint)b <= 127u)
			{
				return (int)num;
			}
		}
		b = ReadByte();
		if ((uint)b > 15u)
		{
			throw new FormatException(SR.Format_Bad7BitInt);
		}
		return (int)num | (b << 28);
	}

	public long Read7BitEncodedInt64()
	{
		ulong num = 0uL;
		byte b;
		for (int i = 0; i < 63; i += 7)
		{
			b = ReadByte();
			num |= ((ulong)b & 0x7FuL) << i;
			if ((uint)b <= 127u)
			{
				return (long)num;
			}
		}
		b = ReadByte();
		if ((uint)b > 1u)
		{
			throw new FormatException(SR.Format_Bad7BitInt);
		}
		return (long)(num | ((ulong)b << 63));
	}
}
