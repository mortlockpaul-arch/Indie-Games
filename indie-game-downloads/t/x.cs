using System;

namespace T;

internal class x
{
	private const int _3A_0018 = 64;

	private const int _3AL = 20;

	private uint[] _3A_0019;

	private ulong _3A3;

	private byte[] _3A6;

	private int _3AD;

	private uint[] _3A_0017;

	public x()
	{
		_3A_0019 = new uint[5];
		_3A6 = new byte[64];
		_3A_0017 = new uint[80];
		Initialize();
	}

	public void HashCore(byte[] rgb, int start, int size)
	{
		if (_3AD != 0)
		{
			if (size < 64 - _3AD)
			{
				Buffer.BlockCopy(rgb, start, _3A6, _3AD, size);
				_3AD += size;
				return;
			}
			int num = 64 - _3AD;
			Buffer.BlockCopy(rgb, start, _3A6, _3AD, num);
			_39(_3A6, 0);
			_3AD = 0;
			start += num;
			size -= num;
		}
		for (int num = 0; num < size - size % 64; num += 64)
		{
			_39(rgb, start + num);
		}
		if (size % 64 != 0)
		{
			Buffer.BlockCopy(rgb, size - size % 64 + start, _3A6, 0, size % 64);
			_3AD = size % 64;
		}
	}

	public byte[] HashFinal()
	{
		byte[] array = new byte[20];
		_3_0004(_3A6, 0, _3AD);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				array[i * 4 + j] = (byte)(_3A_0019[i] >> 8 * (3 - j));
			}
		}
		return array;
	}

	public void Initialize()
	{
		_3A3 = 0uL;
		_3AD = 0;
		_3A_0019[0] = 1732584193u;
		_3A_0019[1] = 4023233417u;
		_3A_0019[2] = 2562383102u;
		_3A_0019[3] = 271733878u;
		_3A_0019[4] = 3285377520u;
	}

	private void _39(byte[] P_0, int P_1)
	{
		_3A3 += 64uL;
		for (int i = 0; i < 16; i++)
		{
			_3A_0017[i] = (uint)((P_0[P_1 + 4 * i] << 24) | (P_0[P_1 + 4 * i + 1] << 16) | (P_0[P_1 + 4 * i + 2] << 8) | P_0[P_1 + 4 * i + 3]);
		}
		for (int i = 16; i < 80; i++)
		{
			_3A_0017[i] = ((_3A_0017[i - 3] ^ _3A_0017[i - 8] ^ _3A_0017[i - 14] ^ _3A_0017[i - 16]) << 1) | ((_3A_0017[i - 3] ^ _3A_0017[i - 8] ^ _3A_0017[i - 14] ^ _3A_0017[i - 16]) >> 31);
		}
		uint num = _3A_0019[0];
		uint num2 = _3A_0019[1];
		uint num3 = _3A_0019[2];
		uint num4 = _3A_0019[3];
		uint num5 = _3A_0019[4];
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + _3A_0017[0];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + _3A_0017[1];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + _3A_0017[2];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + _3A_0017[3];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + _3A_0017[4];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + _3A_0017[5];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + _3A_0017[6];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + _3A_0017[7];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + _3A_0017[8];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + _3A_0017[9];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + _3A_0017[10];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + _3A_0017[11];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + _3A_0017[12];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + _3A_0017[13];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + _3A_0017[14];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (((num3 ^ num4) & num2) ^ num4) + 1518500249 + _3A_0017[15];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (((num2 ^ num3) & num) ^ num3) + 1518500249 + _3A_0017[16];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (((num ^ num2) & num5) ^ num2) + 1518500249 + _3A_0017[17];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (((num5 ^ num) & num4) ^ num) + 1518500249 + _3A_0017[18];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (((num4 ^ num5) & num3) ^ num5) + 1518500249 + _3A_0017[19];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + _3A_0017[20];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + _3A_0017[21];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + _3A_0017[22];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + _3A_0017[23];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + _3A_0017[24];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + _3A_0017[25];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + _3A_0017[26];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + _3A_0017[27];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + _3A_0017[28];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + _3A_0017[29];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + _3A_0017[30];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + _3A_0017[31];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + _3A_0017[32];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + _3A_0017[33];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + _3A_0017[34];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += ((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4) + 1859775393 + _3A_0017[35];
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += ((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3) + 1859775393 + _3A_0017[36];
		num = (num << 30) | (num >> 2);
		num3 += ((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2) + 1859775393 + _3A_0017[37];
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += ((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num) + 1859775393 + _3A_0017[38];
		num4 = (num4 << 30) | (num4 >> 2);
		num += ((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5) + 1859775393 + _3A_0017[39];
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)_3A_0017[40]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)_3A_0017[41]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)_3A_0017[42]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)_3A_0017[43]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)_3A_0017[44]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)_3A_0017[45]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)_3A_0017[46]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)_3A_0017[47]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)_3A_0017[48]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)_3A_0017[49]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)_3A_0017[50]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)_3A_0017[51]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)_3A_0017[52]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)_3A_0017[53]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)_3A_0017[54]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + ((num2 & num3) | (num2 & num4) | (num3 & num4))) + -1894007588 + (int)_3A_0017[55]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + ((num & num2) | (num & num3) | (num2 & num3))) + -1894007588 + (int)_3A_0017[56]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + ((num5 & num) | (num5 & num2) | (num & num2))) + -1894007588 + (int)_3A_0017[57]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + ((num4 & num5) | (num4 & num) | (num5 & num))) + -1894007588 + (int)_3A_0017[58]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + ((num3 & num4) | (num3 & num5) | (num4 & num5))) + -1894007588 + (int)_3A_0017[59]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)_3A_0017[60]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)_3A_0017[61]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)_3A_0017[62]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)_3A_0017[63]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)_3A_0017[64]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)_3A_0017[65]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)_3A_0017[66]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)_3A_0017[67]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)_3A_0017[68]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)_3A_0017[69]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)_3A_0017[70]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)_3A_0017[71]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)_3A_0017[72]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)_3A_0017[73]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)_3A_0017[74]);
		num3 = (num3 << 30) | (num3 >> 2);
		num5 += (uint)((int)(((num << 5) | (num >> 27)) + (num2 ^ num3 ^ num4)) + -899497514 + (int)_3A_0017[75]);
		num2 = (num2 << 30) | (num2 >> 2);
		num4 += (uint)((int)(((num5 << 5) | (num5 >> 27)) + (num ^ num2 ^ num3)) + -899497514 + (int)_3A_0017[76]);
		num = (num << 30) | (num >> 2);
		num3 += (uint)((int)(((num4 << 5) | (num4 >> 27)) + (num5 ^ num ^ num2)) + -899497514 + (int)_3A_0017[77]);
		num5 = (num5 << 30) | (num5 >> 2);
		num2 += (uint)((int)(((num3 << 5) | (num3 >> 27)) + (num4 ^ num5 ^ num)) + -899497514 + (int)_3A_0017[78]);
		num4 = (num4 << 30) | (num4 >> 2);
		num += (uint)((int)(((num2 << 5) | (num2 >> 27)) + (num3 ^ num4 ^ num5)) + -899497514 + (int)_3A_0017[79]);
		num3 = (num3 << 30) | (num3 >> 2);
		_3A_0019[0] += num;
		_3A_0019[1] += num2;
		_3A_0019[2] += num3;
		_3A_0019[3] += num4;
		_3A_0019[4] += num5;
	}

	private void _3_0004(byte[] P_0, int P_1, int P_2)
	{
		ulong num = _3A3 + (ulong)P_2;
		int num2 = 56 - (int)(num % 64);
		if (num2 < 1)
		{
			num2 += 64;
		}
		int num3 = P_2 + num2 + 8;
		byte[] array = ((num3 == 64) ? _3A6 : new byte[num3]);
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
		_3J(num4, array, P_2 + num2);
		_39(array, 0);
		if (num3 == 128)
		{
			_39(array, 64);
		}
	}

	internal void _3J(ulong P_0, byte[] P_1, int P_2)
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
