using System;
using _0013;
using A;
using z;

namespace X;

internal sealed class _0002
{
	private static byte[] HCB = new byte[20]
	{
		218, 57, 163, 238, 94, 107, 75, 13, 50, 85,
		191, 239, 149, 96, 24, 144, 175, 216, 7, 9
	};

	private _0002()
	{
	}

	private static bool He(byte[] P_0, byte[] P_1)
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

	private static byte[] H_0015(byte[] P_0, byte[] P_1)
	{
		byte[] array = new byte[P_0.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(P_0[i] ^ P_1[i]);
		}
		return array;
	}

	private static byte[] HU(z.Z P_0)
	{
		if (P_0 is z.k)
		{
			return HCB;
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

	public static byte[] RSAEP(z._0002 rsa, byte[] m)
	{
		return rsa.EncryptValue(m);
	}

	public static byte[] RSADP(z._0002 rsa, byte[] c)
	{
		return rsa.DecryptValue(c);
	}

	public static byte[] RSASP1(z._0002 rsa, byte[] m)
	{
		return rsa.DecryptValue(m);
	}

	public static byte[] RSAVP1(z._0002 rsa, byte[] s)
	{
		return rsa.EncryptValue(s);
	}

	public static byte[] Encrypt_OAEP(z._0002 rsa, z.Z hash, z.y rng, byte[] M)
	{
		int num = rsa.KeySize / 8;
		int num2 = hash.HashSize / 8;
		if (M.Length > num - 2 * num2 - 2)
		{
			throw new z._7("message too long");
		}
		byte[] array = HU(hash);
		int num3 = num - M.Length - 2 * num2 - 2;
		byte[] array2 = new byte[array.Length + num3 + 1 + M.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		array2[array.Length + num3] = 1;
		Buffer.BlockCopy(M, 0, array2, array2.Length - M.Length, M.Length);
		byte[] array3 = new byte[num2];
		rng.GetBytes(array3);
		byte[] array4 = MGF1(hash, array3, num - num2 - 1);
		byte[] array5 = H_0015(array2, array4);
		byte[] array6 = MGF1(hash, array5, num2);
		byte[] array7 = H_0015(array3, array6);
		byte[] array8 = new byte[array7.Length + array5.Length + 1];
		Buffer.BlockCopy(array7, 0, array8, 1, array7.Length);
		Buffer.BlockCopy(array5, 0, array8, array7.Length + 1, array5.Length);
		byte[] m = OS2IP(array8);
		byte[] x = RSAEP(rsa, m);
		return I2OSP(x, num);
	}

	public static byte[] Decrypt_OAEP(z._0002 rsa, z.Z hash, byte[] C)
	{
		int num = rsa.KeySize / 8;
		int num2 = hash.HashSize / 8;
		if (num < 2 * num2 + 2 || C.Length != num)
		{
			throw new z._7("decryption error");
		}
		byte[] c = OS2IP(C);
		byte[] x = RSADP(rsa, c);
		byte[] array = I2OSP(x, num);
		byte[] array2 = new byte[num2];
		Buffer.BlockCopy(array, 1, array2, 0, array2.Length);
		byte[] array3 = new byte[num - num2 - 1];
		Buffer.BlockCopy(array, array.Length - array3.Length, array3, 0, array3.Length);
		byte[] array4 = MGF1(hash, array3, num2);
		byte[] mgfSeed = H_0015(array2, array4);
		byte[] array5 = MGF1(hash, mgfSeed, num - num2 - 1);
		byte[] array6 = H_0015(array3, array5);
		byte[] array7 = HU(hash);
		byte[] array8 = new byte[array7.Length];
		Buffer.BlockCopy(array6, 0, array8, 0, array8.Length);
		bool flag = He(array7, array8);
		int i;
		for (i = array7.Length; array6[i] == 0; i++)
		{
		}
		int num3 = array6.Length - i - 1;
		byte[] array9 = new byte[num3];
		Buffer.BlockCopy(array6, i + 1, array9, 0, num3);
		if (array[0] != 0 || !flag || array6[i] != 1)
		{
			return null;
		}
		return array9;
	}

	public static byte[] Encrypt_v15(z._0002 rsa, z.y rng, byte[] M)
	{
		int num = rsa.KeySize / 8;
		if (M.Length > num - 11)
		{
			throw new z._7("message too long");
		}
		int num2 = Math.Max(8, num - M.Length - 3);
		byte[] array = new byte[num2];
		rng.GetNonZeroBytes(array);
		byte[] array2 = new byte[num];
		array2[1] = 2;
		Buffer.BlockCopy(array, 0, array2, 2, num2);
		Buffer.BlockCopy(M, 0, array2, num - M.Length, M.Length);
		byte[] m = OS2IP(array2);
		byte[] x = RSAEP(rsa, m);
		return I2OSP(x, num);
	}

	public static byte[] Decrypt_v15(z._0002 rsa, byte[] C)
	{
		int num = rsa.KeySize >> 3;
		if (num < 11 || C.Length > num)
		{
			throw new z._7("decryption error");
		}
		byte[] c = OS2IP(C);
		byte[] x = RSADP(rsa, c);
		byte[] array = I2OSP(x, num);
		if (array[0] != 0 || array[1] != 2)
		{
			return null;
		}
		int i;
		for (i = 10; array[i] != 0 && i < array.Length; i++)
		{
		}
		if (array[i] != 0)
		{
			return null;
		}
		i++;
		byte[] array2 = new byte[array.Length - i];
		Buffer.BlockCopy(array, i, array2, 0, array2.Length);
		return array2;
	}

	public static byte[] Sign_v15(z._0002 rsa, z.Z hash, byte[] hashValue)
	{
		int num = rsa.KeySize >> 3;
		byte[] x = Encode_v15(hash, hashValue, num);
		byte[] m = OS2IP(x);
		byte[] x2 = RSASP1(rsa, m);
		return I2OSP(x2, num);
	}

	public static bool Verify_v15(z._0002 rsa, z.Z hash, byte[] hashValue, byte[] signature)
	{
		return Verify_v15(rsa, hash, hashValue, signature, tryNonStandardEncoding: false);
	}

	public static bool Verify_v15(z._0002 rsa, z.Z hash, byte[] hashValue, byte[] signature, bool tryNonStandardEncoding)
	{
		int num = rsa.KeySize >> 3;
		byte[] array = OS2IP(signature);
		byte[] x = RSAVP1(rsa, array);
		byte[] array2 = I2OSP(x, num);
		byte[] array3 = Encode_v15(hash, hashValue, num);
		bool flag = He(array3, array2);
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
		return He(array4, hashValue);
	}

	public static byte[] Encode_v15(z.Z hash, byte[] hashValue, int emLength)
	{
		if (hashValue.Length != hash.HashSize >> 3)
		{
			throw new z._7("bad hash length for " + hash.ToString());
		}
		byte[] array = null;
		string text = z.H.MapNameToOID(hash.ToString());
		if (text != null)
		{
			A.B b = new A.B(48);
			b.Add(new A.B(z.H.EncodeOID(text)));
			b.Add(new A.B(5));
			A.B asn = new A.B(4, hashValue);
			A.B b2 = new A.B(48);
			b2.Add(b);
			b2.Add(asn);
			array = b2.GetBytes();
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

	public static byte[] MGF1(z.Z hash, byte[] mgfSeed, int maskLen)
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
internal class _0012 : z._0002
{
	public delegate void _0001CB(object sender, EventArgs e);

	private const int HCB = 1024;

	private bool HC_0002;

	private bool HC_0012 = true;

	private bool HCH;

	private bool HC7;

	private _0013.B HC_0001;

	private _0013.B HCw;

	private _0013.B HCZ;

	private _0013.B HC_000F;

	private _0013.B HCy;

	private _0013.B HC6;

	private _0013.B HCD;

	private _0013.B HC_0011;

	public override int KeySize
	{
		get
		{
			if (HCH)
			{
				int num = HCD.BitCount();
				if ((num & 7) != 0)
				{
					num += 8 - (num & 7);
				}
				return num;
			}
			return base.KeySize;
		}
	}

	public override string KeyExchangeAlgorithm => "RSA-PKCS1-KeyEx";

	public bool PublicOnly
	{
		get
		{
			if (HCH)
			{
				if (!(HC_0001 == null))
				{
					return HCD == null;
				}
				return true;
			}
			return false;
		}
	}

	public override string SignatureAlgorithm => "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

	internal bool UseKeyBlinding
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = flag;
		}
	}

	internal bool IsCrtPossible
	{
		get
		{
			if (HCH)
			{
				return HC_0002;
			}
			return true;
		}
	}

	public _0012()
		: this(1024)
	{
	}

	public _0012(int keySize)
	{
		LegalKeySizesValue = new z._000F[1];
		LegalKeySizesValue[0] = new z._000F(384, 16384, 8);
		base.KeySize = keySize;
	}

	~_0012()
	{
		Dispose(disposing: false);
	}

	public override byte[] DecryptValue(byte[] rgb)
	{
		if (HC7)
		{
			throw new ObjectDisposedException("private key");
		}
		if (!HCH)
		{
			throw new Exception("not supported");
		}
		_0013.B b = new _0013.B(rgb);
		_0013.B b2 = null;
		if (HC_0012)
		{
			b2 = _0013.B.GenerateRandom(HCD.BitCount());
			b = b2.ModPow(HC_0011, HCD) * b % HCD;
		}
		_0013.B b6;
		if (HC_0002)
		{
			_0013.B b3 = b.ModPow(HC_000F, HCw);
			_0013.B b4 = b.ModPow(HCy, HCZ);
			if (b4 > b3)
			{
				_0013.B b5 = HCw - (b4 - b3) * HC6 % HCw;
				b6 = b4 + HCZ * b5;
			}
			else
			{
				_0013.B b5 = (b3 - b4) * HC6 % HCw;
				b6 = b4 + HCZ * b5;
			}
		}
		else
		{
			if (PublicOnly)
			{
				throw new z._7("Missing private key to decrypt value.");
			}
			b6 = b.ModPow(HC_0001, HCD);
		}
		if (HC_0012)
		{
			b6 = b6 * b2.ModInverse(HCD) % HCD;
			b2.Clear();
		}
		byte[] result = H_0017(b6, KeySize >> 3);
		b.Clear();
		b6.Clear();
		return result;
	}

	public override byte[] EncryptValue(byte[] rgb)
	{
		if (HC7)
		{
			throw new ObjectDisposedException("public key");
		}
		if (!HCH)
		{
			throw new Exception("not supported");
		}
		_0013.B b = new _0013.B(rgb);
		_0013.B b2 = b.ModPow(HC_0011, HCD);
		byte[] result = H_0017(b2, KeySize >> 3);
		b.Clear();
		b2.Clear();
		return result;
	}

	public override z.K ExportParameters(bool includePrivateParameters)
	{
		if (HC7)
		{
			throw new ObjectDisposedException("");
		}
		if (!HCH)
		{
			throw new Exception("not supported");
		}
		z.K result = new z.K
		{
			Exponent = HC_0011.GetBytes(),
			Modulus = HCD.GetBytes()
		};
		if (includePrivateParameters)
		{
			if (HC_0001 == null)
			{
				throw new z._7("Missing private key");
			}
			result.D = HC_0001.GetBytes();
			if (result.D.Length != result.Modulus.Length)
			{
				byte[] array = new byte[result.Modulus.Length];
				Buffer.BlockCopy(result.D, 0, array, array.Length - result.D.Length, result.D.Length);
				result.D = array;
			}
			if (HCw != null && HCZ != null && HC_000F != null && HCy != null && HC6 != null)
			{
				int num = KeySize >> 4;
				result.P = H_0017(HCw, num);
				result.Q = H_0017(HCZ, num);
				result.DP = H_0017(HC_000F, num);
				result.DQ = H_0017(HCy, num);
				result.InverseQ = H_0017(HC6, num);
			}
		}
		return result;
	}

	public override void ImportParameters(z.K parameters)
	{
		if (HC7)
		{
			throw new ObjectDisposedException("");
		}
		if (parameters.Exponent == null)
		{
			throw new z._7("Missing Exponent");
		}
		if (parameters.Modulus == null)
		{
			throw new z._7("Missing Modulus");
		}
		HC_0011 = new _0013.B(parameters.Exponent);
		HCD = new _0013.B(parameters.Modulus);
		if (parameters.D != null)
		{
			HC_0001 = new _0013.B(parameters.D);
		}
		if (parameters.DP != null)
		{
			HC_000F = new _0013.B(parameters.DP);
		}
		if (parameters.DQ != null)
		{
			HCy = new _0013.B(parameters.DQ);
		}
		if (parameters.InverseQ != null)
		{
			HC6 = new _0013.B(parameters.InverseQ);
		}
		if (parameters.P != null)
		{
			HCw = new _0013.B(parameters.P);
		}
		if (parameters.Q != null)
		{
			HCZ = new _0013.B(parameters.Q);
		}
		HCH = true;
		HC_0002 = HCw != null && HCZ != null && HC_000F != null && HCy != null && HC6 != null;
	}

	protected override void Dispose(bool disposing)
	{
		if (!HC7)
		{
			if (HC_0001 != null)
			{
				HC_0001.Clear();
				HC_0001 = null;
			}
			if (HCw != null)
			{
				HCw.Clear();
				HCw = null;
			}
			if (HCZ != null)
			{
				HCZ.Clear();
				HCZ = null;
			}
			if (HC_000F != null)
			{
				HC_000F.Clear();
				HC_000F = null;
			}
			if (HCy != null)
			{
				HCy.Clear();
				HCy = null;
			}
			if (HC6 != null)
			{
				HC6.Clear();
				HC6 = null;
			}
			if (disposing)
			{
				if (HC_0011 != null)
				{
					HC_0011.Clear();
					HC_0011 = null;
				}
				if (HCD != null)
				{
					HCD.Clear();
					HCD = null;
				}
			}
		}
		HC7 = true;
	}

	private byte[] H_0017(_0013.B P_0, int P_1)
	{
		byte[] bytes = P_0.GetBytes();
		if (bytes.Length >= P_1)
		{
			return bytes;
		}
		byte[] array = new byte[P_1];
		Buffer.BlockCopy(bytes, 0, array, P_1 - bytes.Length, bytes.Length);
		Array.Clear(bytes, 0, bytes.Length);
		return array;
	}
}
