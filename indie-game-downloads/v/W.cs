namespace v;

internal abstract class W
{
	public W()
	{
	}

	public static W Create()
	{
		return new _0002();
	}

	public abstract void GetBytes(byte[] data);

	public abstract void GetNonZeroBytes(byte[] data);
}
