using System;
using _0006;

namespace v;

internal class Z : a
{
	private b a5h;

	private W a5b;

	public W Rng
	{
		get
		{
			return a5b;
		}
		set
		{
			a5b = value;
		}
	}

	public override string Parameters => "<enc:KeyEncryptionMethod enc:Algorithm=\"http://www.microsoft.com/xml/security/algorithm/PKCS1-v1.5-KeyEx\" xmlns:enc=\"http://www.microsoft.com/xml/security/encryption/v1.0\" />";

	public Z()
	{
	}

	public Z(h key)
	{
		SetKey(key);
	}

	public override byte[] CreateKeyExchange(byte[] rgbData)
	{
		if (rgbData == null)
		{
			throw new ArgumentNullException("rgbData");
		}
		if (a5b == null)
		{
			a5b = W.Create();
		}
		return global::_0006.b.Encrypt_v15(a5h, a5b, rgbData);
	}

	public override byte[] CreateKeyExchange(byte[] rgbData, Type symAlgType)
	{
		return CreateKeyExchange(rgbData);
	}

	public override void SetKey(h key)
	{
		a5h = (b)key;
	}
}
internal sealed class z : u
{
	private L a5h;

	public z()
	{
		a5h = new L();
	}

	~z()
	{
		Dispose(disposing: false);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected override void HashCore(byte[] rgb, int start, int size)
	{
		State = 1;
		a5h.HashCore(rgb, start, size);
	}

	protected override byte[] HashFinal()
	{
		State = 0;
		return a5h.HashFinal();
	}

	public override void Initialize()
	{
		a5h.Initialize();
	}
}
