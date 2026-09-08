using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public sealed class BlockIterator : EndianStream
{
	public int m_offset;

	public StructuredBinaryBlockId m_blockId;

	public int m_dataLength;

	public int m_fieldSize;

	public StructuredBinary m_strb;

	public StructuredBinaryBlockId BlockID => m_blockId;

	public bool IsEnd => m_blockId == StructuredBinaryBlockId.Eof;

	public override long Length => m_dataLength;

	public override long Position
	{
		get
		{
			return m_stream.Position - m_offset;
		}
		set
		{
			m_stream.Position = value + m_offset;
		}
	}

	public BlockIterator(StructuredBinary structuredBinary)
	{
		Initialize(structuredBinary.Stream, inLittleEndian: false);
		m_strb = structuredBinary;
		FirstBlock();
	}

	public bool FindFirst(StructuredBinaryBlockId blockId)
	{
		FirstBlock();
		while (!IsEnd)
		{
			if (m_blockId == blockId)
			{
				return true;
			}
			if (!NextBlock())
			{
				return false;
			}
		}
		return false;
	}

	public bool Find(StructuredBinaryBlockId blockId)
	{
		while (!IsEnd)
		{
			if (m_blockId == blockId)
			{
				return true;
			}
			if (!NextBlock())
			{
				return false;
			}
		}
		return false;
	}

	public bool FirstBlock()
	{
		m_dataLength = 0;
		m_offset = m_strb.BlockStartOffset;
		m_blockId = StructuredBinaryBlockId.Invalid;
		return NextBlock();
	}

	public bool NextBlock()
	{
		if (m_blockId == StructuredBinaryBlockId.Eof)
		{
			return false;
		}
		int num = m_offset + m_strb.RoundUpToBlockAlignment(m_dataLength);
		if (num + m_strb.BlockHeaderSize <= m_stream.Length)
		{
			Seek(num, SeekOrigin.Begin);
			m_blockId = (StructuredBinaryBlockId)ReadMultiByte(m_strb.BlockIdSize);
			m_dataLength = (int)ReadMultiByte(m_strb.BlockSpanSize);
			m_fieldSize = (int)ReadMultiByte(m_strb.BlockSpanSize);
			if (m_dataLength < 0)
			{
				Logger.Log(new DebugLog(this, Resources.InvalidStrbFileText));
				throw new AvatarException(Resources.InvalidStrbFileText);
			}
			if (m_fieldSize == 0)
			{
				Logger.Log(new DebugLog(this, Resources.InvalidStrbFileText));
				throw new AvatarException(Resources.InvalidStrbFileText);
			}
			if (m_dataLength % m_fieldSize != 0)
			{
				Logger.Log(new DebugLog(this, Resources.InvalidStrbFileText));
				throw new AvatarException(Resources.InvalidStrbFileText);
			}
			m_offset = num + m_strb.BlockHeaderSize;
			Seek(m_offset, SeekOrigin.Begin);
			return true;
		}
		m_blockId = StructuredBinaryBlockId.Eof;
		m_dataLength = 0;
		m_fieldSize = 0;
		return false;
	}
}
