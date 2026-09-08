using System;
using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class DecompressStream : EndianStream
{
	public const int MAX_CACHED_WINDOW_LENGTH = 32768;

	public int m_size;

	public int m_winStart;

	public int m_winLength;

	public int m_streamSize;

	public int m_offs;

	public MemoryStream m_block;

	public LZXDeflate m_deflate;

	public DecompressStream()
	{
	}

	public DecompressStream(Stream instream, int maxsize)
	{
		m_streamSize = maxsize;
		if (m_streamSize <= 0)
		{
			throw new ArgumentException("input stream is empty");
		}
		Initialize(instream, inLittleEndian: true);
		m_deflate = new LZXDeflate(32768);
	}

	public void ReadBlock()
	{
		if (m_stream.Position >= m_streamSize)
		{
			Logger.Log(new DebugLog(this, Resources.UnexpectedEofText));
			throw new AvatarException(Resources.UnexpectedEofText);
		}
		m_size = EndianStream.ReadInt(m_stream, streamEndianLittle: true);
		m_winStart = EndianStream.ReadInt(m_stream, streamEndianLittle: true);
		m_winLength = EndianStream.ReadInt(m_stream, streamEndianLittle: true);
		if (m_winStart < 0)
		{
			Logger.Log(new DebugLog(this, Resources.InvalidStrbFileText));
			throw new AvatarException(Resources.InvalidStrbFileText);
		}
		if ((uint)m_size > 40000u)
		{
			Logger.Log(new DebugLog(this, Resources.InvalidStrbFileText));
			throw new AvatarException(Resources.InvalidStrbFileText);
		}
		if ((uint)m_winLength > 32768u)
		{
			Logger.Log(new DebugLog(this, Resources.InvalidStrbFileText));
			throw new AvatarException(Resources.InvalidStrbFileText);
		}
		byte[] src = new byte[m_size];
		m_stream.Read(src, 0, m_size);
		byte[] tg = new byte[m_winLength];
		try
		{
			int num = m_deflate.Decompress(ref src, ref tg);
			if (num < 0 || !m_deflate.Reset())
			{
				Logger.Log(new DebugLog(this, Resources.DecompressionFailedText));
				throw new AvatarException(Resources.DecompressionFailedText);
			}
		}
		catch (IndexOutOfRangeException)
		{
			Logger.Log(new DebugLog(this, Resources.DecompressionFailedText));
			throw new AvatarException(Resources.DecompressionFailedText);
		}
		m_block = new MemoryStream(tg);
	}

	public override EndianSwapper Read16b()
	{
		EndianSwapper result = default(EndianSwapper);
		if (m_needRotate)
		{
			result.llh = (byte)ReadByte();
			result.lll = (byte)ReadByte();
		}
		else
		{
			result.lll = (byte)ReadByte();
			result.llh = (byte)ReadByte();
		}
		return result;
	}

	public override EndianSwapper Read32b()
	{
		EndianSwapper result = default(EndianSwapper);
		if (m_needRotate)
		{
			result.lhh = (byte)ReadByte();
			result.lhl = (byte)ReadByte();
			result.llh = (byte)ReadByte();
			result.lll = (byte)ReadByte();
		}
		else
		{
			result.lll = (byte)ReadByte();
			result.llh = (byte)ReadByte();
			result.lhl = (byte)ReadByte();
			result.lhh = (byte)ReadByte();
		}
		return result;
	}

	public override EndianSwapper Read64b()
	{
		EndianSwapper result = default(EndianSwapper);
		if (m_needRotate)
		{
			result.hhh = (byte)ReadByte();
			result.hhl = (byte)ReadByte();
			result.hlh = (byte)ReadByte();
			result.hll = (byte)ReadByte();
			result.lhh = (byte)ReadByte();
			result.lhl = (byte)ReadByte();
			result.llh = (byte)ReadByte();
			result.lll = (byte)ReadByte();
		}
		else
		{
			result.lll = (byte)ReadByte();
			result.llh = (byte)ReadByte();
			result.lhl = (byte)ReadByte();
			result.lhh = (byte)ReadByte();
			result.hll = (byte)ReadByte();
			result.hlh = (byte)ReadByte();
			result.hhl = (byte)ReadByte();
			result.hhh = (byte)ReadByte();
		}
		return result;
	}

	public override int ReadByte()
	{
		if (m_offs < m_winStart)
		{
			m_offs++;
			return 0;
		}
		while (m_winStart + m_winLength <= m_offs)
		{
			ReadBlock();
		}
		m_offs++;
		return m_block.ReadByte();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = 0;
		while (count > 0 && m_winStart > m_offs)
		{
			buffer[num] = 0;
			num++;
			count--;
			m_offs++;
		}
		if (count == 0)
		{
			return num;
		}
		while (m_winStart + m_winLength <= m_offs)
		{
			ReadBlock();
		}
		while (true)
		{
			bool flag = true;
			int num2 = Math.Min(m_winLength - (m_offs - m_winStart), count);
			int num3 = m_block.Read(buffer, num, num2);
			num += num3;
			m_offs += num3;
			count -= num3;
			if (num3 != num2 || count == 0)
			{
				break;
			}
			ReadBlock();
		}
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		m_offs = (int)offset;
		if (m_offs < m_winStart)
		{
			m_stream.Seek(0L, SeekOrigin.Begin);
		}
		while (m_offs > m_winStart + m_winLength)
		{
			ReadBlock();
		}
		return m_offs;
	}
}
