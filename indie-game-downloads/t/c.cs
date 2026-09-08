using System;
using b;

namespace T;

internal sealed class c : L
{
	private const int _3A_0018 = 1;

	private bool _3AL = true;

	private bool _3A_0019;

	private global::b._0019 _3A3;

	public override string KeyExchangeAlgorithm => "RSA-PKCS1-KeyEx";

	public override int KeySize
	{
		get
		{
			if (_3A3 == null)
			{
				return KeySizeValue;
			}
			return _3A3.KeySize;
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

	internal bool PublicOnly => _3A3.PublicOnly;

	public override string SignatureAlgorithm => "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

	public c()
	{
		_3Y(1024);
	}

	private void _3Y(int P_0)
	{
		LegalKeySizesValue = new l[1];
		LegalKeySizesValue[0] = new l(384, 16384, 8);
		base.KeySize = P_0;
		_3A3 = new global::b._0019(KeySize);
	}

	~c()
	{
		Dispose(disposing: false);
	}

	public byte[] Decrypt(byte[] rgb, bool fOAEP)
	{
		_0019 obj = null;
		obj = ((!fOAEP) ? ((_0019)new _8(_3A3)) : ((_0019)new g(_3A3)));
		return obj.DecryptKeyExchange(rgb);
	}

	public override byte[] DecryptValue(byte[] rgb)
	{
		if (!_3A3.IsCrtPossible)
		{
			throw new _6("Incomplete private key - missing CRT.");
		}
		return _3A3.DecryptValue(rgb);
	}

	public override byte[] EncryptValue(byte[] rgb)
	{
		return _3A3.EncryptValue(rgb);
	}

	public override I ExportParameters(bool includePrivateParameters)
	{
		if (includePrivateParameters && !_3AL)
		{
			throw new _6("cannot export private key");
		}
		return _3A3.ExportParameters(includePrivateParameters);
	}

	public override void ImportParameters(I parameters)
	{
		_3A3.ImportParameters(parameters);
	}

	private _0003 _0019A(object P_0)
	{
		if (P_0 == null)
		{
			throw new ArgumentNullException("halg");
		}
		_0003 obj = null;
		if (P_0 is string)
		{
			return new q();
		}
		if (P_0 is _0003)
		{
			return (_0003)P_0;
		}
		if (P_0 is Type)
		{
			return (_0003)Activator.CreateInstance((Type)P_0);
		}
		throw new ArgumentException("halg");
	}

	public byte[] SignHash(byte[] rgbHash, string str)
	{
		if (rgbHash == null)
		{
			throw new ArgumentNullException("rgbHash");
		}
		_0003 hash = new q();
		return global::b.L.Sign_v15(this, hash, rgbHash);
	}

	public bool VerifyData(byte[] buffer, object halg, byte[] signature)
	{
		if (signature == null)
		{
			throw new ArgumentNullException("signature");
		}
		_0003 obj = _0019A(halg);
		byte[] hashValue = obj.ComputeHash(buffer);
		return global::b.L.Verify_v15(this, obj, hashValue, signature);
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
		_0003 hash = new q();
		return global::b.L.Verify_v15(this, hash, rgbHash, rgbSignature);
	}

	protected override void Dispose(bool disposing)
	{
		if (!_3A_0019)
		{
			if (_3A3 != null)
			{
				_3A3.Clear();
			}
			_3A_0019 = true;
		}
	}

	private void _3_000E(object P_0, EventArgs P_1)
	{
	}

	public void ImportCspBlob(byte[] rawData)
	{
		if (rawData == null)
		{
			throw new ArgumentNullException("rawData");
		}
		L l2 = global::b._0018.FromCapiKeyBlob(rawData);
		if (l2 is c)
		{
			I parameters = l2.ExportParameters(!(l2 as c).PublicOnly);
			ImportParameters(parameters);
			return;
		}
		try
		{
			I parameters2 = l2.ExportParameters(include: true);
			ImportParameters(parameters2);
		}
		catch
		{
			I parameters3 = l2.ExportParameters(include: false);
			ImportParameters(parameters3);
		}
	}
}
