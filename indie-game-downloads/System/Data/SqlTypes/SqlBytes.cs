using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes;

[XmlSchemaProvider("GetXsdType")]
public sealed class SqlBytes : INullable, IXmlSerializable, ISerializable
{
	internal byte[] _rgbBuf;

	private long _lCurLen;

	internal Stream _stream;

	private SqlBytesCharsState _state;

	public bool IsNull => _state == SqlBytesCharsState.Null;

	public byte[]? Buffer
	{
		get
		{
			if (FStream())
			{
				CopyStreamToBuffer();
			}
			return _rgbBuf;
		}
	}

	public long Length => _state switch
	{
		SqlBytesCharsState.Null => throw new SqlNullValueException(), 
		SqlBytesCharsState.Stream => _stream.Length, 
		_ => _lCurLen, 
	};

	public long MaxLength
	{
		get
		{
			if (_state == SqlBytesCharsState.Stream)
			{
				return -1L;
			}
			return (_rgbBuf == null) ? (-1) : _rgbBuf.Length;
		}
	}

	public byte[] Value
	{
		get
		{
			byte[] array;
			switch (_state)
			{
			case SqlBytesCharsState.Null:
				throw new SqlNullValueException();
			case SqlBytesCharsState.Stream:
				if (_stream.Length > int.MaxValue)
				{
					throw new SqlTypeException(System.SR.SqlMisc_BufferInsufficientMessage);
				}
				array = new byte[_stream.Length];
				if (_stream.Position != 0L)
				{
					_stream.Seek(0L, SeekOrigin.Begin);
				}
				_stream.ReadExactly(array, 0, checked((int)_stream.Length));
				break;
			default:
				array = new byte[_lCurLen];
				Array.Copy(_rgbBuf, array, (int)_lCurLen);
				break;
			}
			return array;
		}
	}

	public byte this[long offset]
	{
		get
		{
			ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(offset, Length, "offset");
			byte reference = 0;
			Read(offset, new Span<byte>(ref reference));
			return reference;
		}
		set
		{
			Write(offset, new ReadOnlySpan<byte>(in value));
		}
	}

	public StorageState Storage => _state switch
	{
		SqlBytesCharsState.Null => throw new SqlNullValueException(), 
		SqlBytesCharsState.Stream => StorageState.Stream, 
		SqlBytesCharsState.Buffer => StorageState.Buffer, 
		_ => StorageState.UnmanagedBuffer, 
	};

	public Stream Stream
	{
		get
		{
			if (!FStream())
			{
				return new StreamOnSqlBytes(this);
			}
			return _stream;
		}
		set
		{
			_lCurLen = -1L;
			_stream = value;
			_state = ((value != null) ? SqlBytesCharsState.Stream : SqlBytesCharsState.Null);
		}
	}

	public static SqlBytes Null => new SqlBytes((byte[]?)null);

	public SqlBytes()
	{
		SetNull();
	}

	public SqlBytes(byte[]? buffer)
	{
		_rgbBuf = buffer;
		_stream = null;
		if (_rgbBuf == null)
		{
			_state = SqlBytesCharsState.Null;
			_lCurLen = -1L;
		}
		else
		{
			_state = SqlBytesCharsState.Buffer;
			_lCurLen = _rgbBuf.Length;
		}
	}

	public SqlBytes(SqlBinary value)
		: this(value.IsNull ? null : value.Value)
	{
	}

	public SqlBytes(Stream? s)
	{
		_rgbBuf = null;
		_lCurLen = -1L;
		_stream = s;
		_state = ((s != null) ? SqlBytesCharsState.Stream : SqlBytesCharsState.Null);
	}

	public void SetNull()
	{
		_lCurLen = -1L;
		_stream = null;
		_state = SqlBytesCharsState.Null;
	}

	public void SetLength(long value)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(value, "value");
		if (FStream())
		{
			_stream.SetLength(value);
			return;
		}
		if (_rgbBuf == null)
		{
			throw new SqlTypeException(System.SR.SqlMisc_NoBufferMessage);
		}
		ArgumentOutOfRangeException.ThrowIfGreaterThan(value, _rgbBuf.Length, "value");
		if (IsNull)
		{
			_state = SqlBytesCharsState.Buffer;
		}
		_lCurLen = value;
	}

	internal long Read(long offset, Span<byte> buffer)
	{
		if (IsNull)
		{
			throw new SqlNullValueException();
		}
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, Length, "offset");
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		return ReadNoValidation(offset, buffer);
	}

	public long Read(long offset, byte[] buffer, int offsetInBuffer, int count)
	{
		if (IsNull)
		{
			throw new SqlNullValueException();
		}
		ArgumentNullException.ThrowIfNull(buffer, "buffer");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, Length, "offset");
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offsetInBuffer, Length, "offsetInBuffer");
		ArgumentOutOfRangeException.ThrowIfNegative(offsetInBuffer, "offsetInBuffer");
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(count, buffer.Length - offsetInBuffer, "count");
		return ReadNoValidation(offset, buffer.AsSpan(offsetInBuffer, count));
	}

	private long ReadNoValidation(long offset, Span<byte> buffer)
	{
		if (_state == SqlBytesCharsState.Stream)
		{
			if (_stream.Position != offset)
			{
				_stream.Seek(offset, SeekOrigin.Begin);
			}
			return _stream.Read(buffer);
		}
		int length = Math.Min(buffer.Length, (int)(Length - offset));
		ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>(_rgbBuf, (int)offset, length);
		readOnlySpan.CopyTo(buffer);
		return readOnlySpan.Length;
	}

	internal void Write(long offset, ReadOnlySpan<byte> buffer)
	{
		if (FStream())
		{
			if (_stream.Position != offset)
			{
				_stream.Seek(offset, SeekOrigin.Begin);
			}
			_stream.Write(buffer);
			return;
		}
		if (_rgbBuf == null)
		{
			throw new SqlTypeException(System.SR.SqlMisc_NoBufferMessage);
		}
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		if (offset > _rgbBuf.Length)
		{
			throw new SqlTypeException(System.SR.SqlMisc_BufferInsufficientMessage);
		}
		if (buffer.Length > _rgbBuf.Length - offset)
		{
			throw new SqlTypeException(System.SR.SqlMisc_BufferInsufficientMessage);
		}
		WriteNoValidation(offset, buffer);
	}

	public void Write(long offset, byte[] buffer, int offsetInBuffer, int count)
	{
		if (FStream())
		{
			if (_stream.Position != offset)
			{
				_stream.Seek(offset, SeekOrigin.Begin);
			}
			_stream.Write(buffer, offsetInBuffer, count);
			return;
		}
		ArgumentNullException.ThrowIfNull(buffer, "buffer");
		if (_rgbBuf == null)
		{
			throw new SqlTypeException(System.SR.SqlMisc_NoBufferMessage);
		}
		ArgumentOutOfRangeException.ThrowIfNegative(offset, "offset");
		if (offset > _rgbBuf.Length)
		{
			throw new SqlTypeException(System.SR.SqlMisc_BufferInsufficientMessage);
		}
		ArgumentOutOfRangeException.ThrowIfNegative(offsetInBuffer, "offsetInBuffer");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offsetInBuffer, buffer.Length, "offsetInBuffer");
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(count, buffer.Length - offsetInBuffer, "count");
		if (count > _rgbBuf.Length - offset)
		{
			throw new SqlTypeException(System.SR.SqlMisc_BufferInsufficientMessage);
		}
		WriteNoValidation(offset, buffer.AsSpan(offsetInBuffer, count));
	}

	private void WriteNoValidation(long offset, ReadOnlySpan<byte> buffer)
	{
		if (IsNull)
		{
			if (offset != 0L)
			{
				throw new SqlTypeException(System.SR.SqlMisc_WriteNonZeroOffsetOnNullMessage);
			}
			_lCurLen = 0L;
			_state = SqlBytesCharsState.Buffer;
		}
		else if (offset > _lCurLen)
		{
			throw new SqlTypeException(System.SR.SqlMisc_WriteOffsetLargerThanLenMessage);
		}
		if (buffer.Length != 0)
		{
			Span<byte> destination = _rgbBuf.AsSpan((int)offset, buffer.Length);
			buffer.CopyTo(destination);
			if (_lCurLen < offset + buffer.Length)
			{
				_lCurLen = offset + buffer.Length;
			}
		}
	}

	public SqlBinary ToSqlBinary()
	{
		if (!IsNull)
		{
			return new SqlBinary(Value);
		}
		return SqlBinary.Null;
	}

	public static explicit operator SqlBinary(SqlBytes value)
	{
		return value.ToSqlBinary();
	}

	public static explicit operator SqlBytes(SqlBinary value)
	{
		return new SqlBytes(value);
	}

	private void CopyStreamToBuffer()
	{
		long length = _stream.Length;
		if (length >= int.MaxValue)
		{
			throw new SqlTypeException(System.SR.SqlMisc_WriteOffsetLargerThanLenMessage);
		}
		if (_rgbBuf == null || _rgbBuf.Length < length)
		{
			_rgbBuf = new byte[length];
		}
		if (_stream.Position != 0L)
		{
			_stream.Seek(0L, SeekOrigin.Begin);
		}
		_stream.ReadExactly(_rgbBuf, 0, (int)length);
		_stream = null;
		_lCurLen = length;
		_state = SqlBytesCharsState.Buffer;
	}

	internal bool FStream()
	{
		return _state == SqlBytesCharsState.Stream;
	}

	private void SetBuffer(byte[] buffer)
	{
		_rgbBuf = buffer;
		_lCurLen = ((_rgbBuf == null) ? (-1) : _rgbBuf.Length);
		_stream = null;
		_state = ((_rgbBuf != null) ? SqlBytesCharsState.Buffer : SqlBytesCharsState.Null);
	}

	XmlSchema IXmlSerializable.GetSchema()
	{
		return null;
	}

	void IXmlSerializable.ReadXml(XmlReader r)
	{
		byte[] buffer = null;
		string attribute = r.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
		if (attribute != null && XmlConvert.ToBoolean(attribute))
		{
			r.ReadElementString();
			SetNull();
		}
		else
		{
			string text = r.ReadElementString();
			if (text == null)
			{
				buffer = Array.Empty<byte>();
			}
			else
			{
				text = text.Trim();
				buffer = ((text.Length != 0) ? Convert.FromBase64String(text) : Array.Empty<byte>());
			}
		}
		SetBuffer(buffer);
	}

	void IXmlSerializable.WriteXml(XmlWriter writer)
	{
		if (IsNull)
		{
			writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			return;
		}
		byte[] buffer = Buffer;
		writer.WriteString(Convert.ToBase64String(buffer, 0, (int)Length));
	}

	public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
	{
		return new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema");
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new PlatformNotSupportedException();
	}
}
