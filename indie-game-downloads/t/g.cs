using b;

namespace T;

internal class g : _0019
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

	public g()
	{
		_3A_0018 = null;
	}

	public g(_0018 key)
	{
		SetKey(key);
	}

	public override byte[] DecryptKeyExchange(byte[] rgbData)
	{
		Z hash = Z.Create();
		byte[] array = global::b.L.Decrypt_OAEP(_3A_0018, hash, rgbData);
		if (array != null)
		{
			return array;
		}
		throw new _6("OAEP decoding error.");
	}

	public override void SetKey(_0018 key)
	{
		_3A_0018 = (L)key;
	}
}
