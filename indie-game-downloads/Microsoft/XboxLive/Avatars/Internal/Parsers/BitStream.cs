using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class BitStream
{
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	public struct FloatToInt
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int i;
	}

	public Stream m_stream;

	public int m_current;

	public int m_bitOfs;

	public BitStream(Stream stream)
	{
		m_stream = stream;
	}

	public byte ReadByte(int bitSize)
	{
		if (bitSize > 8)
		{
			Logger.Log(new DebugLog(this, Resources.InvalidWordSizeText));
			throw new AvatarException(Resources.InvalidWordSizeText);
		}
		return (byte)ReadUint(bitSize);
	}

	public uint ReadUint(int bitSize)
	{
		if (bitSize > 32)
		{
			Logger.Log(new DebugLog(this, Resources.InvalidWordSizeText));
			throw new AvatarException(Resources.InvalidWordSizeText);
		}
		uint num = (uint)((1L << bitSize) - 1);
		uint num2 = (uint)m_current >> 8 - m_bitOfs;
		int num3 = m_bitOfs;
		bitSize -= m_bitOfs;
		while (bitSize > 0)
		{
			m_current = m_stream.ReadByte();
			if (m_current == -1)
			{
				Logger.Log(new DebugLog(this, Resources.UnexpectedEofText));
				throw new AvatarException(Resources.UnexpectedEofText);
			}
			num2 += (uint)(m_current << num3);
			bitSize -= 8;
			num3 += 8;
		}
		m_bitOfs = -bitSize;
		return num2 & num;
	}

	public int ReadInt(int bitsize)
	{
		return (int)ReadUint(bitsize);
	}

	public bool ReadBool(int bitsize)
	{
		return ReadUint(bitsize) != 0;
	}

	public byte[] ReadByteArray(int size)
	{
		byte[] array = new byte[size];
		for (int i = 0; i < size; i++)
		{
			array[i] = ReadByte(8);
		}
		return array;
	}

	public float ReadFloat()
	{
		FloatToInt floatToInt = new FloatToInt
		{
			i = ReadInt(32)
		};
		return floatToInt.f;
	}
}
