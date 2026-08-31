namespace z;

internal class X : k
{
	private s HCB;

	public X()
	{
		HCB = new s();
	}

	protected override void HashCore(byte[] rgb, int start, int size)
	{
		State = 1;
		HCB.HashCore(rgb, start, size);
	}

	protected override byte[] HashFinal()
	{
		State = 0;
		return HCB.HashFinal();
	}

	public override void Initialize()
	{
		HCB.Initialize();
	}
}
