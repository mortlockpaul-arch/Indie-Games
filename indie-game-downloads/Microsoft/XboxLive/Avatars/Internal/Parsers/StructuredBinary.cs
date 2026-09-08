using System;
using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class StructuredBinary : IDisposable
{
	public EndianStream m_stream;

	public bool m_littleEndian;

	public byte m_blockIdSize;

	public byte m_blockSpanSize;

	public byte m_blockAlignment;

	public int m_blockStartOffset;

	public Guid m_namespace;

	public BlockIterator m_iterator;

	public Guid Namespace => m_namespace;

	public BlockIterator Iterator => m_iterator;

	public int BlockIdSize => m_blockIdSize;

	public int BlockStartOffset => m_blockStartOffset;

	public int BlockSpanSize => m_blockSpanSize;

	public int BlockHeaderSize => RoundUpToBlockAlignment(BlockIdSize + BlockSpanSize + BlockSpanSize);

	public Stream Stream => m_stream;

	public bool Open(Stream stream)
	{
		m_stream = new EndianStream(stream);
		if (ReadHeader())
		{
			m_iterator = new BlockIterator(this);
			return true;
		}
		return false;
	}

	public int RoundUpToBlockAlignment(int valueToRound)
	{
		if ((m_blockAlignment & (m_blockAlignment - 1)) != 0 || m_blockAlignment == 0)
		{
			Logger.Log(new DebugLog(this, Resources.InvalidStrbFileText));
			throw new AvatarException(Resources.InvalidStrbFileText);
		}
		int blockAlignment = m_blockAlignment;
		return (valueToRound + blockAlignment - 1) & ~(blockAlignment - 1);
	}

	public bool ReadHeader()
	{
		byte[] array = new byte[4];
		m_stream.Read(array, 0, 4);
		uint num = 0u;
		if (array[0] == 89 && array[1] == 84 && array[2] == 71 && array[3] == 82)
		{
			m_stream.LittleEndian = true;
			if (!VerifyXSigSignature(m_stream, out var signatureBlockSize))
			{
				return false;
			}
			if (!SearchSTRBHeader(m_stream, out signatureBlockSize))
			{
				return false;
			}
			array = new byte[4] { 83, 84, 82, 66 };
			num = (uint)((int)m_stream.Position - 4);
		}
		byte b = (byte)m_stream.ReadByte();
		if (b >= 2)
		{
			return false;
		}
		m_littleEndian = m_stream.ReadByte() > 0;
		m_stream.LittleEndian = m_littleEndian;
		if (array[0] == 83 && array[1] == 84 && array[2] == 82 && array[3] == 66)
		{
			byte[] array2 = new byte[8];
			int a = m_stream.ReadInt();
			short b2 = m_stream.ReadShort();
			short c = m_stream.ReadShort();
			m_stream.Read(array2, 0, 8);
			m_namespace = new Guid(a, b2, c, array2);
			m_blockIdSize = (byte)m_stream.ReadByte();
			m_blockSpanSize = (byte)m_stream.ReadByte();
			m_stream.ReadUShort();
			if (b == 1)
			{
				m_blockAlignment = (byte)m_stream.ReadByte();
			}
			else
			{
				m_blockAlignment = 1;
			}
			int valueToRound = ((b != 0) ? 30 : 26);
			m_blockStartOffset = (int)num + RoundUpToBlockAlignment(valueToRound);
			return b < 2;
		}
		m_blockStartOffset = 0;
		return false;
	}

	public bool SearchSTRBHeader(EndianStream stream, out uint strbOffset)
	{
		int num = 0;
		strbOffset = 0u;
		char[] array = new char[4] { 'S', 'T', 'R', 'B' };
		while (num < 4)
		{
			int num2 = stream.ReadByte();
			if (num2 < 0)
			{
				return false;
			}
			if ((byte)num2 == array[num])
			{
				num++;
			}
			else
			{
				num = 0;
				if ((byte)num2 == array[num])
				{
					num++;
				}
			}
			strbOffset++;
		}
		return true;
	}

	public virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (m_stream != null)
			{
				m_stream.Dispose();
			}
			if (m_iterator != null)
			{
				m_iterator.Dispose();
			}
		}
		m_stream = null;
		m_iterator = null;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public virtual bool VerifyXSigSignature(EndianStream stream, out uint signatureBlockSize)
	{
		uint num = stream.ReadUInt();
		uint num2 = stream.ReadUInt();
		stream.Seek(num2 - 12, SeekOrigin.Current);
		signatureBlockSize = num2;
		return true;
	}
}
