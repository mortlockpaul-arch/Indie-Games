using System;

namespace v;

internal class L
{
	private const int a5h = 64;

	private const int a5b = 20;

	private uint[] a56;

	private ulong a5a;

	private byte[] a57;

	private int a5_0006;

	private uint[] a5v;

	public L()
	{
		a56 = new uint[5];
		a57 = new byte[64];
		a5v = new uint[80];
		Initialize();
	}

	public void HashCore(byte[] rgb, int start, int size)
	{
		if (a5_0006 != 0)
		{
			if (size < 64 - a5_0006)
			{
				Buffer.BlockCopy(rgb, start, a57, a5_0006, size);
				a5_0006 += size;
				return;
			}
			int num = 64 - a5_0006;
			Buffer.BlockCopy(rgb, start, a57, a5_0006, num);
			V(a57, 0);
			a5_0006 = 0;
			start += num;
			size -= num;
		}
		for (int num = 0; num < size - size % 64; num += 64)
		{
			V(rgb, start + num);
		}
		if (size % 64 != 0)
		{
			Buffer.BlockCopy(rgb, size - size % 64 + start, a57, 0, size % 64);
			a5_0006 = size % 64;
		}
	}

	public byte[] HashFinal()
	{
		byte[] array = new byte[20];
		M(a57, 0, a5_0006);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				array[i * 4 + j] = (byte)(a56[i] >> 8 * (3 - j));
			}
		}
		return array;
	}

	public void Initialize()
	{
		a5a = 0uL;
		a5_0006 = 0;
		a56[0] = 1732584193u;
		a56[1] = 4023233417u;
		a56[2] = 2562383102u;
		a56[3] = 271733878u;
		a56[4] = 3285377520u;
	}

	private void V(byte[] P_0, int P_1)
	{
		a5a += 64uL;
		for (int i = 0; i < 16; i++)
		{
			a5v[i] = (uint)((P_0[P_1 + 4 * i] << 24) | (P_0[P_1 + 4 * i + 1] << 16) | (P_0[P_1 + 4 * i + 2] << 8) | P_0[P_1 + 4 * i + 3]);
		}
		for (int i = 16; i < 80; i++)
		{
			a5v[i] = ((a5v[i - 3] ^ a5v[i - 8] ^ a5v[i - 14] ^ a5v[i - 16]) << 1) | ((a5v[i - 3] ^ a5v[i - 8] ^ a5v[i - 14] ^ a5v[i - 16]) >> 31);
		}
		uint num = a56[0];
		uint num2 = a56[1];
		uint num3 = a56[2];
		uint num4 = a56[3];
		uint num5 = a56[4];
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + a5v[0];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + a5v[1];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + a5v[2];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + a5v[3];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + a5v[4];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + a5v[5];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + a5v[6];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + a5v[7];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + a5v[8];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + a5v[9];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + a5v[10];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + a5v[11];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + a5v[12];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + a5v[13];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + a5v[14];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + a5v[15];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + a5v[16];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + a5v[17];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + a5v[18];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + a5v[19];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + a5v[20];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + a5v[21];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + a5v[22];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + a5v[23];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + a5v[24];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + a5v[25];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + a5v[26];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + a5v[27];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + a5v[28];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + a5v[29];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + a5v[30];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + a5v[31];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + a5v[32];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + a5v[33];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + a5v[34];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + a5v[35];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + a5v[36];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + a5v[37];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + a5v[38];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + a5v[39];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)a5v[40]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)a5v[41]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)a5v[42]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)a5v[43]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)a5v[44]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)a5v[45]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)a5v[46]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)a5v[47]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)a5v[48]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)a5v[49]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)a5v[50]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)a5v[51]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)a5v[52]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)a5v[53]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)a5v[54]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)a5v[55]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)a5v[56]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)a5v[57]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)a5v[58]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)a5v[59]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)a5v[60]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)a5v[61]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)a5v[62]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)a5v[63]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)a5v[64]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)a5v[65]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)a5v[66]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)a5v[67]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)a5v[68]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)a5v[69]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)a5v[70]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)a5v[71]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)a5v[72]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)a5v[73]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)a5v[74]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)a5v[75]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)a5v[76]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)a5v[77]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)a5v[78]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)a5v[79]);
		num3 = (num3 << 30) | (num3 >> 2);
		a56[0] += num;
		a56[1] += num2;
		a56[2] += num3;
		a56[3] += num4;
		a56[4] += num5;
	}

	private void M(byte[] P_0, int P_1, int P_2)
	{
		ulong num = a5a + (ulong)P_2;
		int num2 = 56 - (int)(num % 64);
		if (num2 < 1)
		{
			num2 += 64;
		}
		int num3 = P_2 + num2 + 8;
		byte[] array = ((num3 == 64) ? a57 : new byte[num3]);
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
		_2(num4, array, P_2 + num2);
		V(array, 0);
		if (num3 == 128)
		{
			V(array, 64);
		}
	}

	internal void _2(ulong P_0, byte[] P_1, int P_2)
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
