namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class InterleavedDataUnpacker<Type, Unpacker> : DataUnpackerGeneric<Type[]> where Unpacker : DataUnpackerGeneric<Type>, new()
{
	public Unpacker[] m_Unpackers;

	public int m_PerFrameBitsCount;

	public int m_HeaderMaxItems;

	public Unpacker[] Unpackers => m_Unpackers;

	public InterleavedDataUnpacker(int headerMaxItems)
	{
		m_HeaderMaxItems = headerMaxItems;
	}

	public override int GetHeaderBitCount()
	{
		return 32 + m_HeaderMaxItems * m_Unpackers[0].GetHeaderBitCount();
	}

	public override int GetPerDataBitCount()
	{
		return m_PerFrameBitsCount;
	}

	public override void UnpackHeader(BitStream bitStream)
	{
		int num = bitStream.ReadInt(32);
		m_Unpackers = new Unpacker[num];
		int num2 = num;
		while (--num2 >= 0)
		{
			m_Unpackers[num2] = new Unpacker();
			m_Unpackers[num2].UnpackHeader(bitStream);
		}
		for (num2 = num; num2 < m_HeaderMaxItems; num2++)
		{
			Unpacker val = new Unpacker();
			val.UnpackHeader(bitStream);
		}
		int num3 = 0;
		for (num2 = 0; num2 < num; num2++)
		{
			num3 += m_Unpackers[num2].GetPerDataBitCount();
		}
		m_PerFrameBitsCount = num3;
	}

	public override void UnpackData(BitStream bitStream, out Type[] data)
	{
		int num = m_Unpackers.Length;
		data = new Type[num];
		for (int i = 0; i < num; i++)
		{
			m_Unpackers[i].UnpackData(bitStream, out data[i]);
		}
	}
}
