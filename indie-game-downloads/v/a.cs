using System;

namespace v;

internal abstract class a
{
	public abstract string Parameters { get; }

	public a()
	{
	}

	public abstract byte[] CreateKeyExchange(byte[] data);

	public abstract byte[] CreateKeyExchange(byte[] data, Type symAlgType);

	public abstract void SetKey(h key);
}
