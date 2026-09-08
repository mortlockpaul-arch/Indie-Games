using System;

namespace T;

internal class _3
{
	private const string _3A_0018 = "1.3.14.3.2.26";

	private static object _3AL;

	static _3()
	{
		_3AL = new object();
	}

	private static void _3E()
	{
	}

	private static byte[] _3S(long P_0)
	{
		if (P_0 > int.MaxValue || P_0 < int.MinValue)
		{
			throw new OverflowException("Part of OID doesn't fit in Int32");
		}
		long num = P_0;
		int num2 = 1;
		while (num > 127)
		{
			num >>= 7;
			num2++;
		}
		byte[] array = new byte[num2];
		for (int i = 0; i < num2; i++)
		{
			num = P_0 >> 7 * i;
			num &= 0x7F;
			if (i != 0)
			{
				num += 128;
			}
			array[num2 - i - 1] = Convert.ToByte(num);
		}
		return array;
	}

	public static byte[] EncodeOID(string str)
	{
		char[] separator = new char[1] { '.' };
		string[] array = str.Split(separator);
		if (array.Length < 2)
		{
			throw new D("OID must have at least two parts");
		}
		byte[] array2 = new byte[str.Length];
		try
		{
			byte b2 = Convert.ToByte(array[0]);
			byte b3 = Convert.ToByte(array[1]);
			array2[2] = Convert.ToByte(b2 * 40 + b3);
		}
		catch
		{
			throw new D("Invalid OID");
		}
		int num = 3;
		for (int i = 2; i < array.Length; i++)
		{
			long num2 = Convert.ToInt64(array[i]);
			if (num2 > 127)
			{
				byte[] array3 = _3S(num2);
				Buffer.BlockCopy(array3, 0, array2, num, array3.Length);
				num += array3.Length;
			}
			else
			{
				array2[num++] = Convert.ToByte(num2);
			}
		}
		int num3 = 2;
		byte[] array4 = new byte[num];
		array4[0] = 6;
		if (num > 127)
		{
			throw new D("OID > 127 bytes");
		}
		array4[1] = Convert.ToByte(num - 2);
		Buffer.BlockCopy(array2, num3, array4, num3, num - num3);
		return array4;
	}

	public static string MapNameToOID(string name)
	{
		return "1.3.14.3.2.26";
	}
}
