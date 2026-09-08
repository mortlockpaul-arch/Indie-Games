#define DEBUG
using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class EndianStream : Stream
{
	public Stream m_stream;

	public bool m_needRotate;

	public bool m_streamEndian;

	public override bool CanRead => m_stream.CanRead;

	public override bool CanSeek => m_stream.CanSeek;

	public override bool CanTimeout => m_stream.CanTimeout;

	public override bool CanWrite => m_stream.CanWrite;

	public override long Length => m_stream.Length;

	public override long Position
	{
		get
		{
			return m_stream.Position;
		}
		set
		{
			m_stream.Position = value;
		}
	}

	public override int ReadTimeout
	{
		get
		{
			return m_stream.ReadTimeout;
		}
		set
		{
			m_stream.ReadTimeout = value;
		}
	}

	public override int WriteTimeout
	{
		get
		{
			return m_stream.WriteTimeout;
		}
		set
		{
			m_stream.WriteTimeout = value;
		}
	}

	public bool LittleEndian
	{
		get
		{
			return m_streamEndian;
		}
		set
		{
			bool flag = IsMachineLittleEndian();
			m_streamEndian = value;
			m_needRotate = flag != m_streamEndian;
		}
	}

	public EndianStream()
	{
	}

	public EndianStream(Stream inputStream)
	{
		m_stream = inputStream;
		bool flag = (m_needRotate = IsMachineLittleEndian());
	}

	public void Initialize(Stream inputStream, bool inLittleEndian)
	{
		m_stream = inputStream;
		bool flag = IsMachineLittleEndian();
		m_needRotate = flag != inLittleEndian;
		m_streamEndian = inLittleEndian;
	}

	public virtual EndianSwapper Read16b()
	{
		EndianSwapper result = default(EndianSwapper);
		if (m_needRotate)
		{
			result.llh = (byte)m_stream.ReadByte();
			result.lll = (byte)m_stream.ReadByte();
		}
		else
		{
			result.lll = (byte)m_stream.ReadByte();
			result.llh = (byte)m_stream.ReadByte();
		}
		return result;
	}

	public virtual EndianSwapper Read32b()
	{
		EndianSwapper result = default(EndianSwapper);
		if (m_needRotate)
		{
			result.lhh = (byte)m_stream.ReadByte();
			result.lhl = (byte)m_stream.ReadByte();
			result.llh = (byte)m_stream.ReadByte();
			result.lll = (byte)m_stream.ReadByte();
		}
		else
		{
			result.lll = (byte)m_stream.ReadByte();
			result.llh = (byte)m_stream.ReadByte();
			result.lhl = (byte)m_stream.ReadByte();
			result.lhh = (byte)m_stream.ReadByte();
		}
		return result;
	}

	public virtual EndianSwapper Read64b()
	{
		EndianSwapper result = default(EndianSwapper);
		if (m_needRotate)
		{
			result.hhh = (byte)m_stream.ReadByte();
			result.hhl = (byte)m_stream.ReadByte();
			result.hlh = (byte)m_stream.ReadByte();
			result.hll = (byte)m_stream.ReadByte();
			result.lhh = (byte)m_stream.ReadByte();
			result.lhl = (byte)m_stream.ReadByte();
			result.llh = (byte)m_stream.ReadByte();
			result.lll = (byte)m_stream.ReadByte();
		}
		else
		{
			result.lll = (byte)m_stream.ReadByte();
			result.llh = (byte)m_stream.ReadByte();
			result.lhl = (byte)m_stream.ReadByte();
			result.lhh = (byte)m_stream.ReadByte();
			result.hll = (byte)m_stream.ReadByte();
			result.hlh = (byte)m_stream.ReadByte();
			result.hhl = (byte)m_stream.ReadByte();
			result.hhh = (byte)m_stream.ReadByte();
		}
		return result;
	}

	public override int ReadByte()
	{
		return m_stream.ReadByte();
	}

	public ushort ReadUShort()
	{
		return Read16b().word;
	}

	public short ReadShort()
	{
		return Read16b().aShort;
	}

	public int ReadInt()
	{
		return Read32b().aInt;
	}

	public float ReadFloat()
	{
		return Read32b().aFloat;
	}

	public uint ReadUInt()
	{
		return Read32b().aUint;
	}

	public long ReadLong()
	{
		return Read64b().aLong;
	}

	public ulong ReadULong()
	{
		return Read64b().aLonglong;
	}

	public double ReadDouble()
	{
		return Read64b().aDouble;
	}

	public Guid ReadGuid()
	{
		byte[] array = new byte[8];
		int a = ReadInt();
		short b = ReadShort();
		short c = ReadShort();
		m_stream.Read(array, 0, 8);
		return new Guid(a, b, c, array);
	}

	public long ReadMultiByte(int size)
	{
		switch (size)
		{
		case 1:
			return ReadByte();
		case 2:
			return ReadShort();
		case 4:
			return ReadInt();
		case 8:
			return ReadLong();
		default:
			Logger.Log(new DebugLog(this, Resources.InvalidWordSizeText));
			throw new AvatarException(Resources.InvalidWordSizeText);
		}
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		m_stream.Write(buffer, offset, count);
	}

	public override void WriteByte(byte value)
	{
		m_stream.WriteByte(value);
	}

	public virtual void Write16b(EndianSwapper value)
	{
		if (m_needRotate)
		{
			m_stream.WriteByte(value.llh);
			m_stream.WriteByte(value.lll);
		}
		else
		{
			m_stream.WriteByte(value.lll);
			m_stream.WriteByte(value.llh);
		}
	}

	public virtual void Write32b(EndianSwapper value)
	{
		if (m_needRotate)
		{
			m_stream.WriteByte(value.lhh);
			m_stream.WriteByte(value.lhl);
			m_stream.WriteByte(value.llh);
			m_stream.WriteByte(value.lll);
		}
		else
		{
			m_stream.WriteByte(value.lll);
			m_stream.WriteByte(value.llh);
			m_stream.WriteByte(value.lhl);
			m_stream.WriteByte(value.lhh);
		}
	}

	public virtual void Write64b(EndianSwapper value)
	{
		if (m_needRotate)
		{
			m_stream.WriteByte(value.hhh);
			m_stream.WriteByte(value.hhl);
			m_stream.WriteByte(value.hlh);
			m_stream.WriteByte(value.hll);
			m_stream.WriteByte(value.lhh);
			m_stream.WriteByte(value.lhl);
			m_stream.WriteByte(value.llh);
			m_stream.WriteByte(value.lll);
		}
		else
		{
			m_stream.WriteByte(value.lll);
			m_stream.WriteByte(value.llh);
			m_stream.WriteByte(value.lhl);
			m_stream.WriteByte(value.lhh);
			m_stream.WriteByte(value.hll);
			m_stream.WriteByte(value.hlh);
			m_stream.WriteByte(value.hhl);
			m_stream.WriteByte(value.hhh);
		}
	}

	public void WriteArray(byte[] buffer, int offset, int count)
	{
		if (m_needRotate)
		{
			for (int i = 1; i <= count; i++)
			{
				WriteByte(buffer[offset + count - i]);
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				WriteByte(buffer[offset + j]);
			}
		}
	}

	public void WriteFloat(float value)
	{
		Write32b(new EndianSwapper
		{
			aFloat = value
		});
	}

	public void WriteUint(uint value)
	{
		Write32b(new EndianSwapper
		{
			aUint = value
		});
	}

	public void WriteShort(short value)
	{
		Write16b(new EndianSwapper
		{
			aShort = value
		});
	}

	public void WriteLong(long value)
	{
		Write64b(new EndianSwapper
		{
			aLong = value
		});
	}

	public void WriteGuid(Guid value)
	{
		byte[] array = value.ToByteArray();
		WriteArray(array, 0, 4);
		WriteArray(array, 4, 2);
		WriteArray(array, 6, 2);
		for (int i = 8; i < 16; i++)
		{
			WriteByte(array[i]);
		}
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		Debug.Assert(condition: false);
		return m_stream.BeginRead(buffer, offset, count, callback, state);
	}

	public override int EndRead(IAsyncResult asyncResult)
	{
		Debug.Assert(condition: false);
		return m_stream.EndRead(asyncResult);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
	{
		Debug.Assert(condition: false);
		return m_stream.BeginWrite(buffer, offset, count, callback, state);
	}

	public override void EndWrite(IAsyncResult asyncResult)
	{
		Debug.Assert(condition: false);
		m_stream.EndWrite(asyncResult);
	}

	public override void Close()
	{
		Flush();
		m_stream.Close();
	}

	public override void Flush()
	{
		m_stream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return m_stream.Read(buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return m_stream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		m_stream.SetLength(value);
	}

	public static bool IsMachineLittleEndian()
	{
		return BitConverter.IsLittleEndian;
	}

	public static EndianSwapper Read16b(Stream stream, bool streamEndianLittle)
	{
		EndianSwapper result = default(EndianSwapper);
		if (streamEndianLittle ^ BitConverter.IsLittleEndian)
		{
			result.llh = (byte)stream.ReadByte();
			result.lll = (byte)stream.ReadByte();
		}
		else
		{
			result.lll = (byte)stream.ReadByte();
			result.llh = (byte)stream.ReadByte();
		}
		return result;
	}

	public static EndianSwapper Read32b(Stream stream, bool streamEndianLittle)
	{
		EndianSwapper result = default(EndianSwapper);
		if (streamEndianLittle ^ BitConverter.IsLittleEndian)
		{
			result.lhh = (byte)stream.ReadByte();
			result.lhl = (byte)stream.ReadByte();
			result.llh = (byte)stream.ReadByte();
			result.lll = (byte)stream.ReadByte();
		}
		else
		{
			result.lll = (byte)stream.ReadByte();
			result.llh = (byte)stream.ReadByte();
			result.lhl = (byte)stream.ReadByte();
			result.lhh = (byte)stream.ReadByte();
		}
		return result;
	}

	public static EndianSwapper Read64b(Stream stream, bool streamEndianLittle)
	{
		EndianSwapper result = default(EndianSwapper);
		if (streamEndianLittle ^ BitConverter.IsLittleEndian)
		{
			result.hhh = (byte)stream.ReadByte();
			result.hhl = (byte)stream.ReadByte();
			result.hlh = (byte)stream.ReadByte();
			result.hll = (byte)stream.ReadByte();
			result.lhh = (byte)stream.ReadByte();
			result.lhl = (byte)stream.ReadByte();
			result.llh = (byte)stream.ReadByte();
			result.lll = (byte)stream.ReadByte();
		}
		else
		{
			result.lll = (byte)stream.ReadByte();
			result.llh = (byte)stream.ReadByte();
			result.lhl = (byte)stream.ReadByte();
			result.lhh = (byte)stream.ReadByte();
			result.hll = (byte)stream.ReadByte();
			result.hlh = (byte)stream.ReadByte();
			result.hhl = (byte)stream.ReadByte();
			result.hhh = (byte)stream.ReadByte();
		}
		return result;
	}

	public static int ReadByte(Stream stream)
	{
		return stream.ReadByte();
	}

	public static ushort ReadUShort(Stream stream, bool streamEndianLittle)
	{
		return Read16b(stream, streamEndianLittle).word;
	}

	public static short ReadShort(Stream stream, bool streamEndianLittle)
	{
		return Read16b(stream, streamEndianLittle).aShort;
	}

	public static int ReadInt(Stream stream, bool streamEndianLittle)
	{
		return Read32b(stream, streamEndianLittle).aInt;
	}

	public static float ReadFloat(Stream stream, bool streamEndianLittle)
	{
		return Read32b(stream, streamEndianLittle).aFloat;
	}

	public static uint ReadUInt(Stream stream, bool streamEndianLittle)
	{
		return Read32b(stream, streamEndianLittle).aUint;
	}

	public static long ReadLong(Stream stream, bool streamEndianLittle)
	{
		return Read64b(stream, streamEndianLittle).aLong;
	}

	public static ulong ReadULong(Stream stream, bool streamEndianLittle)
	{
		return Read64b(stream, streamEndianLittle).aLonglong;
	}

	public static double ReadDouble(Stream stream, bool streamEndianLittle)
	{
		return Read64b(stream, streamEndianLittle).aDouble;
	}

	public static long ReadMultiByte(Stream stream, bool streamEndianLittle, int size)
	{
		switch (size)
		{
		case 1:
			return ReadByte(stream);
		case 2:
			return ReadShort(stream, streamEndianLittle);
		case 4:
			return ReadInt(stream, streamEndianLittle);
		case 8:
			return ReadLong(stream, streamEndianLittle);
		default:
			Debug.Assert(condition: false, "Unsupported out size");
			return 0L;
		}
	}
}
