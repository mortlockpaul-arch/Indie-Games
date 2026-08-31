using System;

namespace z;

internal class s
{
	private const int HCB = 64;

	private const int HC_0002 = 20;

	private uint[] HC_0012;

	private ulong HCH;

	private byte[] HC7;

	private int HC_0001;

	private uint[] HCw;

	public s()
	{
		HC_0012 = new uint[5];
		HC7 = new byte[64];
		HCw = new uint[80];
		Initialize();
	}

	public void HashCore(byte[] rgb, int start, int size)
	{
		if (HC_0001 != 0)
		{
			if (size < 64 - HC_0001)
			{
				Buffer.BlockCopy(rgb, start, HC7, HC_0001, size);
				HC_0001 += size;
				return;
			}
			int num = 64 - HC_0001;
			Buffer.BlockCopy(rgb, start, HC7, HC_0001, num);
			H_0016(HC7, 0);
			HC_0001 = 0;
			start += num;
			size -= num;
		}
		for (int num = 0; num < size - size % 64; num += 64)
		{
			H_0016(rgb, start + num);
		}
		if (size % 64 != 0)
		{
			Buffer.BlockCopy(rgb, size - size % 64 + start, HC7, 0, size % 64);
			HC_0001 = size % 64;
		}
	}

	public byte[] HashFinal()
	{
		byte[] array = new byte[20];
		Hv(HC7, 0, HC_0001);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				array[i * 4 + j] = (byte)(HC_0012[i] >> 8 * (3 - j));
			}
		}
		return array;
	}

	public void Initialize()
	{
		HCH = 0uL;
		HC_0001 = 0;
		HC_0012[0] = 1732584193u;
		HC_0012[1] = 4023233417u;
		HC_0012[2] = 2562383102u;
		HC_0012[3] = 271733878u;
		HC_0012[4] = 3285377520u;
	}

	private void H_0016(byte[] P_0, int P_1)
	{
		HCH += 64uL;
		for (int i = 0; i < 16; i++)
		{
			HCw[i] = (uint)((P_0[P_1 + 4 * i] << 24) | (P_0[P_1 + 4 * i + 1] << 16) | (P_0[P_1 + 4 * i + 2] << 8) | P_0[P_1 + 4 * i + 3]);
		}
		for (int i = 16; i < 80; i++)
		{
			HCw[i] = ((HCw[i - 3] ^ HCw[i - 8] ^ HCw[i - 14] ^ HCw[i - 16]) << 1) | ((HCw[i - 3] ^ HCw[i - 8] ^ HCw[i - 14] ^ HCw[i - 16]) >> 31);
		}
		uint num = HC_0012[0];
		uint num2 = HC_0012[1];
		uint num3 = HC_0012[2];
		uint num4 = HC_0012[3];
		uint num5 = HC_0012[4];
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + HCw[0];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + HCw[1];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + HCw[2];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + HCw[3];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + HCw[4];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + HCw[5];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + HCw[6];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + HCw[7];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + HCw[8];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + HCw[9];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + HCw[10];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + HCw[11];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + HCw[12];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + HCw[13];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + HCw[14];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + HCw[15];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + HCw[16];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + HCw[17];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + HCw[18];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + HCw[19];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + HCw[20];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + HCw[21];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + HCw[22];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + HCw[23];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + HCw[24];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + HCw[25];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + HCw[26];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + HCw[27];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + HCw[28];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + HCw[29];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + HCw[30];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + HCw[31];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + HCw[32];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + HCw[33];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + HCw[34];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + HCw[35];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + HCw[36];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + HCw[37];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + HCw[38];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + HCw[39];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)HCw[40]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)HCw[41]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)HCw[42]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)HCw[43]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)HCw[44]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)HCw[45]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)HCw[46]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)HCw[47]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)HCw[48]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)HCw[49]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)HCw[50]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)HCw[51]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)HCw[52]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)HCw[53]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)HCw[54]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)HCw[55]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)HCw[56]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)HCw[57]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)HCw[58]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)HCw[59]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)HCw[60]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)HCw[61]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)HCw[62]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)HCw[63]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)HCw[64]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)HCw[65]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)HCw[66]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)HCw[67]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)HCw[68]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)HCw[69]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)HCw[70]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)HCw[71]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)HCw[72]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)HCw[73]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)HCw[74]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)HCw[75]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)HCw[76]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)HCw[77]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)HCw[78]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)HCw[79]);
		num3 = (num3 << 30) | (num3 >> 2);
		HC_0012[0] += num;
		HC_0012[1] += num2;
		HC_0012[2] += num3;
		HC_0012[3] += num4;
		HC_0012[4] += num5;
	}

	private void Hv(byte[] P_0, int P_1, int P_2)
	{
		ulong num = HCH + (ulong)P_2;
		int num2 = 56 - (int)(num % 64);
		if (num2 < 1)
		{
			num2 += 64;
		}
		int num3 = P_2 + num2 + 8;
		byte[] array = ((num3 == 64) ? HC7 : new byte[num3]);
		for (int i = 0; i < P_2; i++)
		{
			array[i] = P_0[i + P_1];
		}
		array[P_2] = 128;
		for (int j = P_2 + 1; j < P_2 + num2; j++)
		{
			array[j] = 0;
		}
		ulong num4 = num << 3;
		H2(num4, array, P_2 + num2);
		H_0016(array, 0);
		if (num3 == 128)
		{
			H_0016(array, 64);
		}
	}

	internal void H2(ulong P_0, byte[] P_1, int P_2)
	{
		P_1[P_2++] = (byte)(P_0 >> 56);
		P_1[P_2++] = (byte)(P_0 >> 48);
		P_1[P_2++] = (byte)(P_0 >> 40);
		P_1[P_2++] = (byte)(P_0 >> 32);
		P_1[P_2++] = (byte)(P_0 >> 24);
		P_1[P_2++] = (byte)(P_0 >> 16);
		P_1[P_2++] = (byte)(P_0 >> 8);
		P_1[P_2] = (byte)P_0;
	}
}
