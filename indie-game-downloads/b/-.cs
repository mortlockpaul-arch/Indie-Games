using System;
using System.Globalization;
using System.Text;
using T;
using q;

namespace b;

internal sealed class _0018
{
	private _0018()
	{
	}

	private static int _35(byte[] P_0, int P_1)
	{
		return (P_0[P_1 + 3] << 24) | (P_0[P_1 + 2] << 16) | (P_0[P_1 + 1] << 8) | P_0[P_1];
	}

	private static uint _3r(byte[] P_0, int P_1)
	{
		return (uint)((P_0[P_1 + 3] << 24) | (P_0[P_1 + 2] << 16) | (P_0[P_1 + 1] << 8) | P_0[P_1]);
	}

	private static byte[] _3_0012(int P_0)
	{
		return new byte[4]
		{
			(byte)(P_0 & 0xFF),
			(byte)((P_0 >> 8) & 0xFF),
			(byte)((P_0 >> 16) & 0xFF),
			(byte)((P_0 >> 24) & 0xFF)
		};
	}

	private static byte[] _3o(byte[] P_0)
	{
		for (int i = 0; i < P_0.Length; i++)
		{
			if (P_0[i] != 0)
			{
				byte[] array = new byte[P_0.Length - i];
				Buffer.BlockCopy(P_0, i, array, 0, array.Length);
				return array;
			}
		}
		return null;
	}

	public static T.L FromCapiPrivateKeyBlob(byte[] blob)
	{
		return FromCapiPrivateKeyBlob(blob, 0);
	}

	public static T.L FromCapiPrivateKeyBlob(byte[] blob, int offset)
	{
		if (blob == null)
		{
			throw new ArgumentNullException("blob");
		}
		if (offset >= blob.Length)
		{
			throw new ArgumentException("blob is too small.");
		}
		try
		{
			if (blob[offset] != 7 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || _3r(blob, offset + 8) != 843141970)
			{
				throw new T._6("Invalid blob header");
			}
			int num = _35(blob, offset + 12);
			T.I parameters = default(T.I);
			byte[] array = new byte[4];
			Buffer.BlockCopy(blob, offset + 16, array, 0, 4);
			Array.Reverse(array);
			parameters.Exponent = _3o(array);
			int num2 = offset + 20;
			int num3 = num >> 3;
			parameters.Modulus = new byte[num3];
			Buffer.BlockCopy(blob, num2, parameters.Modulus, 0, num3);
			Array.Reverse(parameters.Modulus);
			num2 += num3;
			int num4 = num3 >> 1;
			parameters.P = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.P, 0, num4);
			Array.Reverse(parameters.P);
			num2 += num4;
			parameters.Q = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.Q, 0, num4);
			Array.Reverse(parameters.Q);
			num2 += num4;
			parameters.DP = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.DP, 0, num4);
			Array.Reverse(parameters.DP);
			num2 += num4;
			parameters.DQ = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.DQ, 0, num4);
			Array.Reverse(parameters.DQ);
			num2 += num4;
			parameters.InverseQ = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.InverseQ, 0, num4);
			Array.Reverse(parameters.InverseQ);
			num2 += num4;
			parameters.D = new byte[num3];
			if (num2 + num3 + offset <= blob.Length)
			{
				Buffer.BlockCopy(blob, num2, parameters.D, 0, num3);
				Array.Reverse(parameters.D);
			}
			T.L l2 = null;
			try
			{
				l2 = T.L.Create();
				l2.ImportParameters(parameters);
			}
			catch (T._6)
			{
			}
			return l2;
		}
		catch (Exception inner)
		{
			throw new T._6("Invalid blob.", inner);
		}
	}

	public static byte[] ToCapiPrivateKeyBlob(T.L rsa)
	{
		T.I i = rsa.ExportParameters(include: true);
		int num = i.Modulus.Length;
		byte[] array = new byte[20 + (num << 2) + (num >> 1)];
		array[0] = 7;
		array[1] = 2;
		array[5] = 36;
		array[8] = 82;
		array[9] = 83;
		array[10] = 65;
		array[11] = 50;
		byte[] array2 = _3_0012(num << 3);
		array[12] = array2[0];
		array[13] = array2[1];
		array[14] = array2[2];
		array[15] = array2[3];
		int num2 = 16;
		int num3 = i.Exponent.Length;
		while (num3 > 0)
		{
			array[num2++] = i.Exponent[--num3];
		}
		num2 = 20;
		byte[] modulus = i.Modulus;
		int num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		num2 += num4;
		modulus = i.P;
		num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		num2 += num4;
		modulus = i.Q;
		num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		num2 += num4;
		modulus = i.DP;
		num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		num2 += num4;
		modulus = i.DQ;
		num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		num2 += num4;
		modulus = i.InverseQ;
		num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		num2 += num4;
		modulus = i.D;
		num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		return array;
	}

	public static T.L FromCapiPublicKeyBlob(byte[] blob)
	{
		return FromCapiPublicKeyBlob(blob, 0);
	}

	public static T.L FromCapiPublicKeyBlob(byte[] blob, int offset)
	{
		if (blob == null)
		{
			throw new ArgumentNullException("blob");
		}
		if (offset >= blob.Length)
		{
			throw new ArgumentException("blob is too small.");
		}
		try
		{
			if (blob[offset] != 6 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || _3r(blob, offset + 8) != 826364754)
			{
				throw new T._6("Invalid blob header");
			}
			int num = _35(blob, offset + 12);
			T.I parameters = new T.I
			{
				Exponent = new byte[3]
			};
			parameters.Exponent[0] = blob[offset + 18];
			parameters.Exponent[1] = blob[offset + 17];
			parameters.Exponent[2] = blob[offset + 16];
			int srcOffset = offset + 20;
			int num2 = num >> 3;
			parameters.Modulus = new byte[num2];
			Buffer.BlockCopy(blob, srcOffset, parameters.Modulus, 0, num2);
			Array.Reverse(parameters.Modulus);
			T.L l2 = null;
			try
			{
				l2 = T.L.Create();
				l2.ImportParameters(parameters);
			}
			catch (T._6)
			{
			}
			return l2;
		}
		catch (Exception inner)
		{
			throw new T._6("Invalid blob.", inner);
		}
	}

	public static byte[] ToCapiPublicKeyBlob(T.L rsa)
	{
		T.I i = rsa.ExportParameters(include: false);
		int num = i.Modulus.Length;
		byte[] array = new byte[20 + num];
		array[0] = 6;
		array[1] = 2;
		array[5] = 36;
		array[8] = 82;
		array[9] = 83;
		array[10] = 65;
		array[11] = 49;
		byte[] array2 = _3_0012(num << 3);
		array[12] = array2[0];
		array[13] = array2[1];
		array[14] = array2[2];
		array[15] = array2[3];
		int num2 = 16;
		int num3 = i.Exponent.Length;
		while (num3 > 0)
		{
			array[num2++] = i.Exponent[--num3];
		}
		num2 = 20;
		byte[] modulus = i.Modulus;
		int num4 = modulus.Length;
		Array.Reverse(modulus, 0, num4);
		Buffer.BlockCopy(modulus, 0, array, num2, num4);
		num2 += num4;
		return array;
	}

	public static T.L FromCapiKeyBlob(byte[] blob)
	{
		return FromCapiKeyBlob(blob, 0);
	}

	public static T.L FromCapiKeyBlob(byte[] blob, int offset)
	{
		if (blob == null)
		{
			throw new ArgumentNullException("blob");
		}
		if (offset >= blob.Length)
		{
			throw new ArgumentException("blob is too small.");
		}
		switch (blob[offset])
		{
		case 0:
			if (blob[offset + 12] == 6)
			{
				return FromCapiPublicKeyBlob(blob, offset + 12);
			}
			break;
		case 6:
			return FromCapiPublicKeyBlob(blob, offset);
		case 7:
			return FromCapiPrivateKeyBlob(blob, offset);
		}
		throw new T._6("Unknown blob format.");
	}

	public static byte[] ToCapiKeyBlob(T._0018 keypair, bool includePrivateKey)
	{
		if (keypair == null)
		{
			throw new ArgumentNullException("keypair");
		}
		if (keypair is T.L)
		{
			return ToCapiKeyBlob((T.L)keypair, includePrivateKey);
		}
		return null;
	}

	public static byte[] ToCapiKeyBlob(T.L rsa, bool includePrivateKey)
	{
		if (rsa == null)
		{
			throw new ArgumentNullException("rsa");
		}
		if (includePrivateKey)
		{
			return ToCapiPrivateKeyBlob(rsa);
		}
		return ToCapiPublicKeyBlob(rsa);
	}

	public static string ToHex(byte[] input)
	{
		if (input == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
		foreach (byte b2 in input)
		{
			stringBuilder.Append(b2.ToString("X2", CultureInfo.InvariantCulture));
		}
		return stringBuilder.ToString();
	}

	private static byte _3f(char P_0)
	{
		if (P_0 >= 'a' && P_0 <= 'f')
		{
			return (byte)(P_0 - 97 + 10);
		}
		if (P_0 >= 'A' && P_0 <= 'F')
		{
			return (byte)(P_0 - 65 + 10);
		}
		if (P_0 >= '0' && P_0 <= '9')
		{
			return (byte)(P_0 - 48);
		}
		throw new ArgumentException("invalid hex char");
	}

	public static byte[] FromHex(string hex)
	{
		if (hex == null)
		{
			return null;
		}
		if ((hex.Length & 1) == 1)
		{
			throw new ArgumentException("Length must be a multiple of 2");
		}
		byte[] array = new byte[hex.Length >> 1];
		int num = 0;
		int num2 = 0;
		while (num < array.Length)
		{
			array[num] = (byte)(_3f(hex[num2++]) << 4);
			array[num++] += _3f(hex[num2++]);
		}
		return array;
	}
}
internal class _0019 : T.L
{
	public delegate void DA_0018(object sender, EventArgs e);

	private const int _3A_0018 = 1024;

	private bool _3AL;

	private bool _3A_0019 = true;

	private bool _3A3;

	private bool _3A6;

	private q._0018 _3AD;

	private q._0018 _3A_0017;

	private q._0018 _3A_0003;

	private q._0018 _3Al;

	private q._0018 _3At;

	private q._0018 _3AF;

	private q._0018 _3Ac;

	private q._0018 _3Ag;

	public override int KeySize
	{
		get
		{
			if (_3A3)
			{
				int num = _3Ac.BitCount();
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
			if (_3A3)
			{
				if (!(_3AD == null))
				{
					return _3Ac == null;
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
			return _3A_0019;
		}
		set
		{
			_3A_0019 = flag;
		}
	}

	internal bool IsCrtPossible
	{
		get
		{
			if (_3A3)
			{
				return _3AL;
			}
			return true;
		}
	}

	public _0019()
		: this(1024)
	{
	}

	public _0019(int keySize)
	{
		LegalKeySizesValue = new T.l[1];
		LegalKeySizesValue[0] = new T.l(384, 16384, 8);
		base.KeySize = keySize;
	}

	~_0019()
	{
		Dispose(disposing: false);
	}

	public override byte[] DecryptValue(byte[] rgb)
	{
		if (_3A6)
		{
			throw new ObjectDisposedException("private key");
		}
		if (!_3A3)
		{
			throw new Exception("not supported");
		}
		q._0018 obj = new q._0018(rgb);
		q._0018 obj2 = null;
		if (_3A_0019)
		{
			obj2 = q._0018.GenerateRandom(_3Ac.BitCount());
			obj = obj2.ModPow(_3Ag, _3Ac) * obj % _3Ac;
		}
		q._0018 obj6;
		if (_3AL)
		{
			q._0018 obj3 = obj.ModPow(_3Al, _3A_0017);
			q._0018 obj4 = obj.ModPow(_3At, _3A_0003);
			if (obj4 > obj3)
			{
				q._0018 obj5 = _3A_0017 - (obj4 - obj3) * _3AF % _3A_0017;
				obj6 = obj4 + _3A_0003 * obj5;
			}
			else
			{
				q._0018 obj5 = (obj3 - obj4) * _3AF % _3A_0017;
				obj6 = obj4 + _3A_0003 * obj5;
			}
		}
		else
		{
			if (PublicOnly)
			{
				throw new T._6("Missing private key to decrypt value.");
			}
			obj6 = obj.ModPow(_3AD, _3Ac);
		}
		if (_3A_0019)
		{
			obj6 = obj6 * obj2.ModInverse(_3Ac) % _3Ac;
			obj2.Clear();
		}
		byte[] result = _3_0005(obj6, KeySize >> 3);
		obj.Clear();
		obj6.Clear();
		return result;
	}

	public override byte[] EncryptValue(byte[] rgb)
	{
		if (_3A6)
		{
			throw new ObjectDisposedException("public key");
		}
		if (!_3A3)
		{
			throw new Exception("not supported");
		}
		q._0018 obj = new q._0018(rgb);
		q._0018 obj2 = obj.ModPow(_3Ag, _3Ac);
		byte[] result = _3_0005(obj2, KeySize >> 3);
		obj.Clear();
		obj2.Clear();
		return result;
	}

	public override T.I ExportParameters(bool includePrivateParameters)
	{
		if (_3A6)
		{
			throw new ObjectDisposedException("");
		}
		if (!_3A3)
		{
			throw new Exception("not supported");
		}
		T.I result = new T.I
		{
			Exponent = _3Ag.GetBytes(),
			Modulus = _3Ac.GetBytes()
		};
		if (includePrivateParameters)
		{
			if (_3AD == null)
			{
				throw new T._6("Missing private key");
			}
			result.D = _3AD.GetBytes();
			if (result.D.Length != result.Modulus.Length)
			{
				byte[] array = new byte[result.Modulus.Length];
				Buffer.BlockCopy(result.D, 0, array, array.Length - result.D.Length, result.D.Length);
				result.D = array;
			}
			if (_3A_0017 != null && _3A_0003 != null && _3Al != null && _3At != null && _3AF != null)
			{
				int num = KeySize >> 4;
				result.P = _3_0005(_3A_0017, num);
				result.Q = _3_0005(_3A_0003, num);
				result.DP = _3_0005(_3Al, num);
				result.DQ = _3_0005(_3At, num);
				result.InverseQ = _3_0005(_3AF, num);
			}
		}
		return result;
	}

	public override void ImportParameters(T.I parameters)
	{
		if (_3A6)
		{
			throw new ObjectDisposedException("");
		}
		if (parameters.Exponent == null)
		{
			throw new T._6("Missing Exponent");
		}
		if (parameters.Modulus == null)
		{
			throw new T._6("Missing Modulus");
		}
		_3Ag = new q._0018(parameters.Exponent);
		_3Ac = new q._0018(parameters.Modulus);
		if (parameters.D != null)
		{
			_3AD = new q._0018(parameters.D);
		}
		if (parameters.DP != null)
		{
			_3Al = new q._0018(parameters.DP);
		}
		if (parameters.DQ != null)
		{
			_3At = new q._0018(parameters.DQ);
		}
		if (parameters.InverseQ != null)
		{
			_3AF = new q._0018(parameters.InverseQ);
		}
		if (parameters.P != null)
		{
			_3A_0017 = new q._0018(parameters.P);
		}
		if (parameters.Q != null)
		{
			_3A_0003 = new q._0018(parameters.Q);
		}
		_3A3 = true;
		_3AL = _3A_0017 != null && _3A_0003 != null && _3Al != null && _3At != null && _3AF != null;
	}

	protected override void Dispose(bool disposing)
	{
		if (!_3A6)
		{
			if (_3AD != null)
			{
				_3AD.Clear();
				_3AD = null;
			}
			if (_3A_0017 != null)
			{
				_3A_0017.Clear();
				_3A_0017 = null;
			}
			if (_3A_0003 != null)
			{
				_3A_0003.Clear();
				_3A_0003 = null;
			}
			if (_3Al != null)
			{
				_3Al.Clear();
				_3Al = null;
			}
			if (_3At != null)
			{
				_3At.Clear();
				_3At = null;
			}
			if (_3AF != null)
			{
				_3AF.Clear();
				_3AF = null;
			}
			if (disposing)
			{
				if (_3Ag != null)
				{
					_3Ag.Clear();
					_3Ag = null;
				}
				if (_3Ac != null)
				{
					_3Ac.Clear();
					_3Ac = null;
				}
			}
		}
		_3A6 = true;
	}

	private byte[] _3_0005(q._0018 P_0, int P_1)
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
