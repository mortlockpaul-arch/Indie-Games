namespace RuntimeXNA.Services;

public class CFile
{
	private byte[] data;

	public int pointer;

	public bool bUnicode;

	public CFile()
	{
	}

	public CFile(CFile file)
	{
		data = file.data;
		pointer = 0;
	}

	public CFile(byte[] dt)
	{
		data = dt;
		pointer = 0;
	}

	public CFile(CFile source, int length)
	{
		data = new byte[length];
		int i;
		for (i = 0; i < length; i++)
		{
			data[i] = source.data[source.pointer + i];
		}
		source.pointer += i;
		bUnicode = source.bUnicode;
	}

	public bool isEOF()
	{
		return pointer >= data.Length;
	}

	public void adjustTo8()
	{
		if ((pointer & 7) != 0)
		{
			pointer += 8 - (pointer & 7);
		}
	}

	public int readUnsignedByte()
	{
		if (pointer < data.Length)
		{
			return data[pointer++] & 0xFF;
		}
		return 0;
	}

	public short readShort()
	{
		int num = readUnsignedByte();
		int num2 = readUnsignedByte();
		return (short)((num << 8) | num2);
	}

	public byte readByte()
	{
		if (pointer < data.Length)
		{
			return data[pointer++];
		}
		return 0;
	}

	public byte[] readArray(int size)
	{
		if (size < 0)
		{
			size = data.Length;
		}
		byte[] array = new byte[size];
		for (int i = 0; i < size; i++)
		{
			array[i] = data[pointer++];
		}
		return array;
	}

	public int read(byte[] dest, int size)
	{
		int i;
		for (i = 0; i < size; i++)
		{
			dest[i] = data[pointer++];
		}
		return i;
	}

	public int read(byte[] dest)
	{
		int i;
		for (i = 0; i < dest.Length; i++)
		{
			dest[i] = data[pointer++];
		}
		return i;
	}

	public void skipBytes(int n)
	{
		if (pointer + n >= data.Length)
		{
			n = data.Length - pointer;
		}
		pointer += n;
	}

	public void skipBack(int n)
	{
		int filePointer = getFilePointer();
		filePointer -= n;
		if (filePointer < 0)
		{
			filePointer = 0;
		}
		seek(filePointer);
	}

	public void seek(int pos)
	{
		if (pos >= data.Length)
		{
			pos = data.Length;
		}
		pointer = pos;
	}

	public int getFilePointer()
	{
		return pointer;
	}

	public void setUnicode(bool unicode)
	{
		bUnicode = unicode;
	}

	public byte readAByte()
	{
		return data[pointer++];
	}

	public short readAShort()
	{
		int num = readUnsignedByte();
		int num2 = readUnsignedByte();
		return (short)(num2 * 256 + num);
	}

	public char readAChar()
	{
		int num = readUnsignedByte();
		int num2 = readUnsignedByte();
		return (char)(num2 * 256 + num);
	}

	public void readAChar(char[] b)
	{
		for (int i = 0; i < b.Length; i++)
		{
			int num = readUnsignedByte();
			int num2 = readUnsignedByte();
			b[i] = (char)(num2 * 256 + num);
		}
	}

	public int readAInt()
	{
		int num = readUnsignedByte();
		int num2 = readUnsignedByte();
		int num3 = readUnsignedByte();
		int num4 = readUnsignedByte();
		return num4 * 16777216 + num3 * 65536 + num2 * 256 + num;
	}

	public int readAColor()
	{
		int num = readUnsignedByte();
		int num2 = readUnsignedByte();
		int num3 = readUnsignedByte();
		readUnsignedByte();
		return num * 65536 + num2 * 256 + num3;
	}

	public float readAFloat()
	{
		int num = readUnsignedByte();
		int num2 = readUnsignedByte();
		int num3 = readUnsignedByte();
		int num4 = readUnsignedByte();
		int num5 = num4 * 16777216 + num3 * 65536 + num2 * 256 + num;
		return (float)num5 / 65536f;
	}

	public double readADouble()
	{
		int num = readUnsignedByte();
		int num2 = readUnsignedByte();
		int num3 = readUnsignedByte();
		int num4 = readUnsignedByte();
		int num5 = readUnsignedByte();
		int num6 = readUnsignedByte();
		int num7 = readUnsignedByte();
		int num8 = readUnsignedByte();
		long num9 = (long)num4 * 16777216L + (long)num3 * 65536L + (long)num2 * 256L + num;
		long num10 = (long)num8 * 16777216L + (long)num7 * 65536L + (long)num6 * 256L + num5;
		long num11 = (num10 << 32) | num9;
		double num12 = (double)num11 / 65536.0;
		return num12 / 65536.0;
	}

	public string readAString(int size)
	{
		if (!bUnicode)
		{
			byte[] array = new byte[size];
			read(array);
			int i;
			for (i = 0; i < size && array[i] != 0; i++)
			{
			}
			char[] array2 = new char[i];
			for (int j = 0; j < i; j++)
			{
				array2[j] = (char)array[j];
			}
			return new string(array2, 0, i);
		}
		char[] array3 = new char[size];
		readAChar(array3);
		int k;
		for (k = 0; k < size && array3[k] != 0; k++)
		{
		}
		char[] array4 = new char[k];
		for (int l = 0; l < k; l++)
		{
			array4[l] = array3[l];
		}
		return new string(array4, 0, k);
	}

	public string readAString()
	{
		string result = "";
		int filePointer = getFilePointer();
		if (!bUnicode)
		{
			while (readUnsignedByte() != 0)
			{
			}
			int filePointer2 = getFilePointer();
			seek(filePointer);
			if (filePointer2 >= filePointer + 2)
			{
				int num = filePointer2 - filePointer - 1;
				char[] array = new char[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = (char)readUnsignedByte();
				}
				result = new string(array, 0, num);
			}
			skipBytes(1);
		}
		else
		{
			while (readAChar() != 0)
			{
			}
			int filePointer3 = getFilePointer();
			seek(filePointer);
			if (filePointer3 >= filePointer + 2)
			{
				int num2 = (filePointer3 - filePointer - 2) / 2;
				char[] array2 = new char[num2];
				readAChar(array2);
				result = new string(array2, 0, num2);
			}
			skipBytes(2);
		}
		return result;
	}

	public string readAStringEOL()
	{
		int filePointer = getFilePointer();
		string result = "";
		if (!bUnicode)
		{
			int num = readUnsignedByte();
			while (num != 10 && num != 13 && !isEOF())
			{
				num = readUnsignedByte();
			}
			int filePointer2 = getFilePointer();
			seek(filePointer);
			int num2 = 1;
			if (num != 10 && num != 13)
			{
				num2 = 0;
			}
			if (filePointer2 > filePointer + num2)
			{
				int num3 = filePointer2 - filePointer - num2;
				char[] array = new char[num3];
				for (int i = 0; i < num3; i++)
				{
					array[i] = (char)readUnsignedByte();
				}
				result = new string(array, 0, array.Length);
			}
			if (num == 10 || num == 13)
			{
				skipBytes(1);
				int num4 = readUnsignedByte();
				if (num == 10 && num4 != 13)
				{
					skipBack(1);
				}
				if (num == 13 && num4 != 10)
				{
					skipBack(1);
				}
			}
		}
		else
		{
			char c = readAChar();
			while (c != '\n' && c != '\r' && !isEOF())
			{
				c = readAChar();
			}
			int filePointer3 = getFilePointer();
			seek(filePointer);
			int num5 = 2;
			if (c != '\n' && c != '\r')
			{
				num5 = 0;
			}
			if (filePointer3 > filePointer + num5)
			{
				int num6 = (filePointer3 - filePointer - num5) / 2;
				char[] array2 = new char[num6];
				readAChar(array2);
				result = new string(array2, 0, array2.Length);
			}
			if (c == '\n' || c == '\r')
			{
				skipBytes(2);
				char c2 = readAChar();
				if (c == '\n' && c2 != '\r')
				{
					skipBack(2);
				}
				if (c == '\r' && c2 != '\n')
				{
					skipBack(2);
				}
			}
		}
		return result;
	}

	public void skipAString()
	{
		if (!bUnicode)
		{
			while (readUnsignedByte() != 0)
			{
			}
		}
		else
		{
			while (readShort() != 0)
			{
			}
		}
	}

	public CFontInfo readLogFont()
	{
		CFontInfo cFontInfo = new CFontInfo();
		cFontInfo.lfHeight = readAInt();
		if (cFontInfo.lfHeight < 0)
		{
			cFontInfo.lfHeight = -cFontInfo.lfHeight;
		}
		skipBytes(12);
		cFontInfo.lfWeight = readAInt();
		cFontInfo.lfItalic = readAByte();
		cFontInfo.lfUnderline = readAByte();
		cFontInfo.lfStrikeOut = readAByte();
		skipBytes(5);
		cFontInfo.lfFaceName = readAString(32);
		return cFontInfo;
	}

	public CFontInfo readLogFont16()
	{
		CFontInfo cFontInfo = new CFontInfo();
		cFontInfo.lfHeight = readAShort();
		if (cFontInfo.lfHeight < 0)
		{
			cFontInfo.lfHeight = -cFontInfo.lfHeight;
		}
		skipBytes(6);
		cFontInfo.lfWeight = readAShort();
		cFontInfo.lfItalic = readAByte();
		cFontInfo.lfUnderline = readAByte();
		cFontInfo.lfStrikeOut = readAByte();
		skipBytes(5);
		bool flag = bUnicode;
		bUnicode = false;
		cFontInfo.lfFaceName = readAString(32);
		bUnicode = flag;
		return cFontInfo;
	}
}
