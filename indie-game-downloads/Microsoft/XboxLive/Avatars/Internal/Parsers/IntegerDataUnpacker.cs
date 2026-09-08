namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class IntegerDataUnpacker : DataUnpackerGeneric<int>
{
	public int m_MinValue;

	public int m_BitCount;

	public int m_ItemBitSize;

	public IntegerDataUnpacker()
	{
		m_ItemBitSize = 32;
	}

	public IntegerDataUnpacker(int headBitSize)
	{
		m_ItemBitSize = headBitSize;
	}

	public override int GetHeaderBitCount()
	{
		return 2 * m_ItemBitSize;
	}

	public override int GetPerDataBitCount()
	{
		return m_BitCount;
	}

	public override void UnpackHeader(BitStream bitStream)
	{
		m_MinValue = bitStream.ReadInt(m_ItemBitSize);
		m_BitCount = bitStream.ReadInt(m_ItemBitSize);
		if (m_BitCount > m_ItemBitSize)
		{
			Logger.Log(new DebugLog(this, Resources.CorruptedStreamText));
			throw new AvatarException(Resources.CorruptedStreamText);
		}
	}

	public override void UnpackData(BitStream bitStream, out int data)
	{
		data = bitStream.ReadInt(m_BitCount) + m_MinValue;
	}
}
