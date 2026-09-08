using System;
using T;
using y;

namespace b;

internal sealed class L
{
	private static byte[] _3A_0018 = new byte[20]
	{
		218, 57, 163, 238, 94, 107, 75, 13, 50, 85,
		191, 239, 149, 96, 24, 144, 175, 216, 7, 9
	};

	private L()
	{
	}

	private static bool _31(byte[] P_0, byte[] P_1)
	{
		bool flag = P_0.Length == P_1.Length;
		if (flag)
		{
			for (int i = 0; i < P_0.Length; i++)
			{
				if (P_0[i] != P_1[i])
				{
					return false;
				}
			}
		}
		return flag;
	}

	private static byte[] _3G(byte[] P_0, byte[] P_1)
	{
		byte[] array = new byte[P_0.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(P_0[i] ^ P_1[i]);
		}
		return array;
	}

	private static byte[] _3n(T._0003 P_0)
	{
		if (P_0 is T.Z)
		{
			return _3A_0018;
		}
		return P_0.ComputeHash((byte[])null);
	}

	public static byte[] I2OSP(int x, int size)
	{
		byte[] bytes = BitConverter.GetBytes(x);
		return I2OSP(bytes, size);
	}

	public static byte[] I2OSP(byte[] x, int size)
	{
		byte[] array = new byte[size];
		Buffer.BlockCopy(x, 0, array, array.Length - x.Length, x.Length);
		return array;
	}

	public static byte[] OS2IP(byte[] x)
	{
		int num = 0;
		while (x[num++] == 0 && num < x.Length)
		{
		}
		num--;
		if (num > 0)
		{
			byte[] array = new byte[x.Length - num];
			Buffer.BlockCopy(x, num, array, 0, array.Length);
			return array;
		}
		return x;
	}

	public static byte[] RSAEP(T.L rsa, byte[] m)
	{
		return rsa.EncryptValue(m);
	}

	public static byte[] RSADP(T.L rsa, byte[] c)
	{
		return rsa.DecryptValue(c);
	}

	public static byte[] RSASP1(T.L rsa, byte[] m)
	{
		return rsa.DecryptValue(m);
	}

	public static byte[] RSAVP1(T.L rsa, byte[] s)
	{
		return rsa.EncryptValue(s);
	}

	public static byte[] Encrypt_OAEP(T.L rsa, T._0003 hash, T.t rng, byte[] M)
	{
		int num = rsa.KeySize / 8;
		int num2 = hash.HashSize / 8;
		if (M.Length > num - 2 * num2 - 2)
		{
			throw new T._6("message too long");
		}
		byte[] array = _3n(hash);
		int num3 = num - M.Length - 2 * num2 - 2;
		byte[] array2 = new byte[array.Length + num3 + 1 + M.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		array2[array.Length + num3] = 1;
		Buffer.BlockCopy(M, 0, array2, array2.Length - M.Length, M.Length);
		byte[] array3 = new byte[num2];
		rng.GetBytes(array3);
		byte[] array4 = MGF1(hash, array3, num - num2 - 1);
		byte[] array5 = _3G(array2, array4);
		byte[] array6 = MGF1(hash, array5, num2);
		byte[] array7 = _3G(array3, array6);
		byte[] dst = new byte[array7.Length + array5.Length + 1];
		Buffer.BlockCopy(array7, 0, dst, 1, array7.Length);
		Buffer.BlockCopy(array5, 0, dst, array7.Length + 1, array5.Length);
		byte[] m = OS2IP(dst);
		byte[] array8 = RSAEP(rsa, m);
		return I2OSP(array8, num);
	}

	public static byte[] Decrypt_OAEP(T.L rsa, T._0003 hash, byte[] C)
	{
		int num = rsa.KeySize / 8;
		int num2 = hash.HashSize / 8;
		if (num < 2 * num2 + 2 || C.Length != num)
		{
			throw new T._6("decryption error");
		}
		byte[] array = OS2IP(C);
		byte[] array2 = RSADP(rsa, array);
		byte[] array3 = I2OSP(array2, num);
		byte[] array4 = new byte[num2];
		Buffer.BlockCopy(array3, 1, array4, 0, array4.Length);
		byte[] array5 = new byte[num - num2 - 1];
		Buffer.BlockCopy(array3, array3.Length - array5.Length, array5, 0, array5.Length);
		byte[] array6 = MGF1(hash, array5, num2);
		byte[] mgfSeed = _3G(array4, array6);
		byte[] array7 = MGF1(hash, mgfSeed, num - num2 - 1);
		byte[] array8 = _3G(array5, array7);
		byte[] array9 = _3n(hash);
		byte[] array10 = new byte[array9.Length];
		Buffer.BlockCopy(array8, 0, array10, 0, array10.Length);
		bool flag = _31(array9, array10);
		int i;
		for (i = array9.Length; array8[i] == 0; i++)
		{
		}
		int num3 = array8.Length - i - 1;
		byte[] array11 = new byte[num3];
		Buffer.BlockCopy(array8, i + 1, array11, 0, num3);
		if (array3[0] != 0 || !flag || array8[i] != 1)
		{
			return null;
		}
		return array11;
	}

	public static byte[] Encrypt_v15(T.L rsa, T.t rng, byte[] M)
	{
		int num = rsa.KeySize / 8;
		if (M.Length > num - 11)
		{
			throw new T._6("message too long");
		}
		int num2 = Math.Max(8, num - M.Length - 3);
		byte[] array = new byte[num2];
		rng.GetNonZeroBytes(array);
		byte[] array2 = new byte[num];
		array2[1] = 2;
		Buffer.BlockCopy(array, 0, array2, 2, num2);
		Buffer.BlockCopy(M, 0, array2, num - M.Length, M.Length);
		byte[] m = OS2IP(array2);
		byte[] array3 = RSAEP(rsa, m);
		return I2OSP(array3, num);
	}

	public static byte[] Decrypt_v15(T.L rsa, byte[] C)
	{
		int num = rsa.KeySize >> 3;
		if (num < 11 || C.Length > num)
		{
			throw new T._6("decryption error");
		}
		byte[] array = OS2IP(C);
		byte[] array2 = RSADP(rsa, array);
		byte[] array3 = I2OSP(array2, num);
		if (array3[0] != 0 || array3[1] != 2)
		{
			return null;
		}
		int i;
		for (i = 10; array3[i] != 0 && i < array3.Length; i++)
		{
		}
		if (array3[i] != 0)
		{
			return null;
		}
		i++;
		byte[] array4 = new byte[array3.Length - i];
		Buffer.BlockCopy(array3, i, array4, 0, array4.Length);
		return array4;
	}

	public static byte[] Sign_v15(T.L rsa, T._0003 hash, byte[] hashValue)
	{
		int num = rsa.KeySize >> 3;
		byte[] array = Encode_v15(hash, hashValue, num);
		byte[] m = OS2IP(array);
		byte[] array2 = RSASP1(rsa, m);
		return I2OSP(array2, num);
	}

	public static bool Verify_v15(T.L rsa, T._0003 hash, byte[] hashValue, byte[] signature)
	{
		return Verify_v15(rsa, hash, hashValue, signature, tryNonStandardEncoding: false);
	}

	public static bool Verify_v15(T.L rsa, T._0003 hash, byte[] hashValue, byte[] signature, bool tryNonStandardEncoding)
	{
		int num = rsa.KeySize >> 3;
		byte[] s = OS2IP(signature);
		byte[] array = RSAVP1(rsa, s);
		byte[] array2 = I2OSP(array, num);
		byte[] array3 = Encode_v15(hash, hashValue, num);
		bool flag = _31(array3, array2);
		if (flag || !tryNonStandardEncoding)
		{
			return flag;
		}
		if (array2[0] != 0 || array2[1] != 1)
		{
			return false;
		}
		int i;
		for (i = 2; i < array2.Length - hashValue.Length - 1; i++)
		{
			if (array2[i] != byte.MaxValue)
			{
				return false;
			}
		}
		if (array2[i++] != 0)
		{
			return false;
		}
		byte[] array4 = new byte[hashValue.Length];
		Buffer.BlockCopy(array2, i, array4, 0, array4.Length);
		return _31(array4, hashValue);
	}

	public static byte[] Encode_v15(T._0003 hash, byte[] hashValue, int emLength)
	{
		if (hashValue.Length != hash.HashSize >> 3)
		{
			throw new T._6("bad hash length for " + hash.ToString());
		}
		byte[] array = null;
		string text = T._3.MapNameToOID(hash.ToString());
		if (text != null)
		{
			y._0018 obj = new y._0018(48);
			obj.Add(new y._0018(T._3.EncodeOID(text)));
			obj.Add(new y._0018(5));
			y._0018 asn = new y._0018(4, hashValue);
			y._0018 obj2 = new y._0018(48);
			obj2.Add(obj);
			obj2.Add(asn);
			array = obj2.GetBytes();
		}
		else
		{
			array = hashValue;
		}
		Buffer.BlockCopy(hashValue, 0, array, array.Length - hashValue.Length, hashValue.Length);
		int num = Math.Max(8, emLength - array.Length - 3);
		byte[] array2 = new byte[num + array.Length + 3];
		array2[1] = 1;
		for (int i = 2; i < num + 2; i++)
		{
			array2[i] = byte.MaxValue;
		}
		Buffer.BlockCopy(array, 0, array2, num + 3, array.Length);
		return array2;
	}

	public static byte[] MGF1(T._0003 hash, byte[] mgfSeed, int maskLen)
	{
		if (maskLen < 0)
		{
			throw new OverflowException();
		}
		int num = mgfSeed.Length;
		int num2 = hash.HashSize >> 3;
		int num3 = maskLen / num2;
		if (maskLen % num2 != 0)
		{
			num3++;
		}
		byte[] array = new byte[num3 * num2];
		byte[] array2 = new byte[num + 4];
		int num4 = 0;
		for (int i = 0; i < num3; i++)
		{
			byte[] src = I2OSP(i, 4);
			Buffer.BlockCopy(mgfSeed, 0, array2, 0, num);
			Buffer.BlockCopy(src, 0, array2, num, 4);
			byte[] src2 = hash.ComputeHash(array2);
			Buffer.BlockCopy(src2, 0, array, num4, num2);
			num4 += num;
		}
		byte[] array3 = new byte[maskLen];
		Buffer.BlockCopy(array, 0, array3, 0, maskLen);
		return array3;
	}
}
