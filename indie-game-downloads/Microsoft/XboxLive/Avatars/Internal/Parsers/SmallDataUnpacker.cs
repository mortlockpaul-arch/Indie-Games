using System;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class SmallDataUnpacker<Type> : DataUnpackerGeneric<Type> where Type : new()
{
	public int m_BitsCount;

	public SmallDataUnpacker(int byteSize)
	{
		m_BitsCount = 8 * byteSize;
	}

	public override int GetHeaderBitCount()
	{
		return 0;
	}

	public override int GetPerDataBitCount()
	{
		return m_BitsCount;
	}

	public override void UnpackHeader(BitStream bitStream)
	{
	}

	public void UnpackData(BitStream bitStream, out byte data)
	{
		data = (byte)bitStream.ReadInt(m_BitsCount);
	}

	public void UnpackData(BitStream bitStream, out short data)
	{
		data = (short)bitStream.ReadInt(m_BitsCount);
	}

	public void UnpackData(BitStream bitStream, out int data)
	{
		data = bitStream.ReadInt(m_BitsCount);
	}

	public override void UnpackData(BitStream bitStream, out Type data)
	{
		throw new NotImplementedException();
	}
}
