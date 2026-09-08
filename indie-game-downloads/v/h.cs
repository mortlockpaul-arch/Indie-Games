using System;

namespace v;

internal abstract class h : IDisposable
{
	protected int KeySizeValue;

	protected _0018[] LegalKeySizesValue;

	public abstract string KeyExchangeAlgorithm { get; }

	public virtual int KeySize
	{
		get
		{
			return KeySizeValue;
		}
		set
		{
			if (!_0018.k(LegalKeySizesValue, value))
			{
				throw new _0006("Key size not supported by algorithm.");
			}
			KeySizeValue = value;
		}
	}

	public virtual _0018[] LegalKeySizes => LegalKeySizesValue;

	public abstract string SignatureAlgorithm { get; }

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Clear()
	{
		Dispose(disposing: false);
	}

	protected abstract void Dispose(bool disposing);

	public static h Create()
	{
		return new _000E();
	}
}
