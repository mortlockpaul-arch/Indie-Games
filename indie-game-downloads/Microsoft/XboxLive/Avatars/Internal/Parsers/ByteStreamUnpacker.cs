using System.IO;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class ByteStreamUnpacker<Type>
{
	public int m_DataCount;

	public BitStream m_Stream;

	public DataUnpackerGeneric<Type> m_Unpacker;

	public ByteStreamUnpacker(Stream stream, DataUnpackerGeneric<Type> unpacker)
	{
		m_Unpacker = unpacker;
		m_Stream = new BitStream(stream);
	}

	public int Unpack(out Type[] result)
	{
		m_DataCount = m_Stream.ReadInt(32);
		m_Unpacker.UnpackHeader(m_Stream);
		result = new Type[m_DataCount];
		for (int i = 0; i < m_DataCount; i++)
		{
			m_Unpacker.UnpackData(m_Stream, out result[i]);
		}
		return m_DataCount;
	}

	public void UnpackHeader()
	{
		m_DataCount = m_Stream.ReadInt(32);
		m_Unpacker.UnpackHeader(m_Stream);
	}

	public int UnpackData(out Type[] result)
	{
		result = new Type[m_DataCount];
		for (int i = 0; i < m_DataCount; i++)
		{
			m_Unpacker.UnpackData(m_Stream, out result[i]);
		}
		return m_DataCount;
	}
}
