using b;

namespace T;

internal class _8 : _0019
{
	private L _3A_0018;

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

	public _8()
	{
		_3A_0018 = null;
	}

	public _8(_0018 key)
	{
		SetKey(key);
	}

	public override byte[] DecryptKeyExchange(byte[] rgbData)
	{
		if (_3A_0018 == null)
		{
			throw new D("No key pair available.");
		}
		byte[] array = global::b.L.Decrypt_v15(_3A_0018, rgbData);
		if (array != null)
		{
			return array;
		}
		throw new _6("PKCS1 decoding error.");
	}

	public override void SetKey(_0018 key)
	{
		_3A_0018 = (L)key;
	}
}
