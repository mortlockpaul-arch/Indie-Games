using System.IO;

namespace DataCompressor;

public class EncodeData
{
	public static string EncodeRow(byte[] srcData, int rowLength, FileStream fStream, int srcPos)
	{
		string text = "";
		int num = 0;
		while (num < rowLength)
		{
			int num2 = srcData[srcPos + num];
			int num3 = 1;
			int num4 = num + 1;
			while (num4 < rowLength && num3 < 255 && srcData[srcPos + num4] == num2)
			{
				num4++;
				num3++;
			}
			if (num3 > 6 || num2 == 27)
			{
				if (num3 == 27)
				{
					num3--;
				}
				fStream.WriteByte(27);
				fStream.WriteByte((byte)num3);
				fStream.WriteByte((byte)num2);
				num += num3;
				continue;
			}
			while (num < rowLength && num3 > 0)
			{
				fStream.WriteByte(srcData[srcPos + num]);
				if (srcData[srcPos + num] == 27)
				{
					text += "\n---------------------- ERROR: Escape Code In Raw Data!!";
					text = text + "\n---------------------- ERROR: 'runCount' = " + num3;
				}
				num++;
				num3--;
			}
		}
		return text;
	}

	public static void DecodeRow(byte[] buf, FileStream fStream, int dstPos, int rowLength)
	{
		int num = 0;
		while (num < rowLength)
		{
			int num2 = fStream.ReadByte();
			if (num2 == 27)
			{
				int num3 = fStream.ReadByte();
				byte b = (byte)fStream.ReadByte();
				while (num < rowLength && num3 > 0)
				{
					buf[dstPos + num] = b;
					num++;
					num3--;
				}
			}
			else
			{
				buf[dstPos + num] = (byte)num2;
				num++;
			}
		}
	}
}
