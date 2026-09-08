using System;
using System.Text;
using _0006;

namespace v;

internal class _0006 : SystemException
{
	public _0006()
		: base("Error occured during a cryptographic operation.")
	{
		base.HResult = -2146233296;
	}

	public _0006(int hr)
	{
		base.HResult = hr;
	}

	public _0006(string message)
		: base(message)
	{
		base.HResult = -2146233296;
	}

	public _0006(string message, Exception inner)
		: base(message, inner)
	{
		base.HResult = -2146233296;
	}

	public _0006(string format, string insert)
		: base(string.Format(format, insert))
	{
		base.HResult = -2146233296;
	}
}
internal sealed class _0018
{
	private int a5h;

	private int a5b;

	private int a56;

	public int MaxSize => a5h;

	public int MinSize => a5b;

	public int SkipSize => a56;

	public _0018(int minSize, int maxSize, int skipSize)
	{
		a5h = maxSize;
		a5b = minSize;
		a56 = skipSize;
	}

	internal bool j(int P_0)
	{
		int num = P_0 - MinSize;
		bool flag = num >= 0 && P_0 <= MaxSize;
		if (SkipSize != 0)
		{
			if (flag)
			{
				return num % SkipSize == 0;
			}
			return false;
		}
		return flag;
	}

	internal static bool k(_0018[] P_0, int P_1)
	{
		foreach (_0018 obj in P_0)
		{
			if (obj.j(P_1))
			{
				return true;
			}
		}
		return false;
	}
}
internal sealed class _0002 : W
{
	private static object a5h;

	private IntPtr a5b;

	private static Random a56;

	static _0002()
	{
		a56 = new Random();
		if (_0016())
		{
			a5h = new object();
		}
	}

	public _0002()
	{
		a5b = K(null);
		_0015();
	}

	public _0002(byte[] rgb)
	{
		a5b = K(rgb);
		_0015();
	}

	public _0002(string str)
	{
		if (str == null)
		{
			a5b = K(null);
		}
		else
		{
			a5b = K(Encoding.UTF8.GetBytes(str));
		}
		_0015();
	}

	private void _0015()
	{
		if (a5b == IntPtr.Zero)
		{
			throw new _0006("Couldn't access random source.");
		}
	}

	private static bool _0016()
	{
		return true;
	}

	private static IntPtr K(byte[] P_0)
	{
		if (P_0 == null || P_0.Length < 1)
		{
			return new IntPtr(1);
		}
		a56 = new Random(P_0[0]);
		return new IntPtr(1);
	}

	private static IntPtr O(IntPtr P_0, byte[] P_1)
	{
		a56.NextBytes(P_1);
		return new IntPtr(1);
	}

	private static void _3(IntPtr P_0)
	{
	}

	public override void GetBytes(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (a5h == null)
		{
			a5b = O(a5b, data);
		}
		else
		{
			lock (a5h)
			{
				a5b = O(a5b, data);
			}
		}
		_0015();
	}

	public override void GetNonZeroBytes(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		byte[] array = new byte[data.Length * 2];
		int num = 0;
		while (num < data.Length)
		{
			a5b = O(a5b, array);
			_0015();
			for (int i = 0; i < array.Length; i++)
			{
				if (num == data.Length)
				{
					break;
				}
				if (array[i] != 0)
				{
					data[num++] = array[i];
				}
			}
		}
	}

	~_0002()
	{
		if (a5b != IntPtr.Zero)
		{
			_3(a5b);
			a5b = IntPtr.Zero;
		}
	}
}
internal sealed class _000E : b
{
	private const int a5h = 1;

	private bool a5b = true;

	private bool a56;

	private global::_0006._6 a5a;

	public override string KeyExchangeAlgorithm => "RSA-PKCS1-KeyEx";

	public override int KeySize
	{
		get
		{
			if (a5a == null)
			{
				return KeySizeValue;
			}
			return a5a.KeySize;
		}
	}

	public bool PersistKeyInCsp
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	internal bool PublicOnly => a5a.PublicOnly;

	public override string SignatureAlgorithm => "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

	public _000E()
	{
		t(1024);
	}

	private void t(int P_0)
	{
		LegalKeySizesValue = new _0018[1];
		LegalKeySizesValue[0] = new _0018(384, 16384, 8);
		base.KeySize = P_0;
		a5a = new global::_0006._6(KeySize);
	}

	~_000E()
	{
		Dispose(disposing: false);
	}

	public byte[] Decrypt(byte[] rgb, bool fOAEP)
	{
		_6 obj = null;
		obj = ((!fOAEP) ? ((_6)new _000F(a5a)) : ((_6)new y(a5a)));
		return obj.DecryptKeyExchange(rgb);
	}

	public override byte[] DecryptValue(byte[] rgb)
	{
		if (!a5a.IsCrtPossible)
		{
			throw new _0006("Incomplete private key - missing CRT.");
		}
		return a5a.DecryptValue(rgb);
	}

	public override byte[] EncryptValue(byte[] rgb)
	{
		return a5a.EncryptValue(rgb);
	}

	public override _0001 ExportParameters(bool includePrivateParameters)
	{
		if (includePrivateParameters && !a5b)
		{
			throw new _0006("cannot export private key");
		}
		return a5a.ExportParameters(includePrivateParameters);
	}

	public override void ImportParameters(_0001 parameters)
	{
		a5a.ImportParameters(parameters);
	}

	private X F(object P_0)
	{
		if (P_0 == null)
		{
			throw new ArgumentNullException("halg");
		}
		X x = null;
		if (P_0 is string)
		{
			return new z();
		}
		if (P_0 is X)
		{
			return (X)P_0;
		}
		if (P_0 is Type)
		{
			return (X)Activator.CreateInstance((Type)P_0);
		}
		throw new ArgumentException("halg");
	}

	public byte[] SignHash(byte[] rgbHash, string str)
	{
		if (rgbHash == null)
		{
			throw new ArgumentNullException("rgbHash");
		}
		X hash = new z();
		return global::_0006.b.Sign_v15(this, hash, rgbHash);
	}

	public bool VerifyData(byte[] buffer, object halg, byte[] signature)
	{
		if (signature == null)
		{
			throw new ArgumentNullException("signature");
		}
		X x = F(halg);
		byte[] hashValue = x.ComputeHash(buffer);
		return global::_0006.b.Verify_v15(this, x, hashValue, signature);
	}

	public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
	{
		if (rgbHash == null)
		{
			throw new ArgumentNullException("rgbHash");
		}
		if (rgbSignature == null)
		{
			throw new ArgumentNullException("rgbSignature");
		}
		X hash = new z();
		return global::_0006.b.Verify_v15(this, hash, rgbHash, rgbSignature);
	}

	protected override void Dispose(bool disposing)
	{
		if (!a56)
		{
			if (a5a != null)
			{
				a5a.Clear();
			}
			a56 = true;
		}
	}

	private void _0005(object P_0, EventArgs P_1)
	{
	}

	public void ImportCspBlob(byte[] rawData)
	{
		if (rawData == null)
		{
			throw new ArgumentNullException("rawData");
		}
		b b2 = global::_0006.h.FromCapiKeyBlob(rawData);
		if (b2 is _000E)
		{
			_0001 parameters = b2.ExportParameters(!(b2 as _000E).PublicOnly);
			ImportParameters(parameters);
			return;
		}
		try
		{
			_0001 parameters2 = b2.ExportParameters(include: true);
			ImportParameters(parameters2);
		}
		catch
		{
			_0001 parameters3 = b2.ExportParameters(include: false);
			ImportParameters(parameters3);
		}
	}
}
internal struct _0001
{
	public byte[] P;

	public byte[] Q;

	public byte[] D;

	public byte[] DP;

	public byte[] DQ;

	public byte[] InverseQ;

	public byte[] Modulus;

	public byte[] Exponent;
}
internal class _000F : _6
{
	private b a5h;

	public override string Parameters
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public _000F()
	{
		a5h = null;
	}

	public _000F(h key)
	{
		SetKey(key);
	}

	public override byte[] DecryptKeyExchange(byte[] rgbData)
	{
		if (a5h == null)
		{
			throw new v("No key pair available.");
		}
		byte[] array = global::_0006.b.Decrypt_v15(a5h, rgbData);
		if (array != null)
		{
			return array;
		}
		throw new _0006("PKCS1 decoding error.");
	}

	public override void SetKey(h key)
	{
		a5h = (b)key;
	}
}
