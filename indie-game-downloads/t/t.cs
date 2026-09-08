namespace T;

internal abstract class t
{
	public t()
	{
	}

	public static t Create()
	{
		return new F();
	}

	public abstract void GetBytes(byte[] data);

	public abstract void GetNonZeroBytes(byte[] data);
}
