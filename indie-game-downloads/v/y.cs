using _0006;

namespace v;

internal class y : _6
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

	public y()
	{
		a5h = null;
	}

	public y(h key)
	{
		SetKey(key);
	}

	public override byte[] DecryptKeyExchange(byte[] rgbData)
	{
		u hash = u.Create();
		byte[] array = global::_0006.b.Decrypt_OAEP(a5h, hash, rgbData);
		if (array != null)
		{
			return array;
		}
		throw new _0006("OAEP decoding error.");
	}

	public override void SetKey(h key)
	{
		a5h = (b)key;
	}
}
internal class Y : u
{
	private L a5h;

	public Y()
	{
		a5h = new L();
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
