using System;
using _0006;

namespace v;

internal class r : a
{
	private b a5h;

	private W a5b;

	private byte[] a56;

	public byte[] Parameter
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = value;
		}
	}

	public override string Parameters => null;

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

	public r()
	{
		a5h = null;
	}

	public r(h key)
	{
		SetKey(key);
	}

	public override byte[] CreateKeyExchange(byte[] rgbData)
	{
		if (a5b == null)
		{
			a5b = W.Create();
		}
		u hash = u.Create();
		return global::_0006.b.Encrypt_OAEP(a5h, hash, a5b, rgbData);
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
