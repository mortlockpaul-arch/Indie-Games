namespace T;

internal sealed class q : Z
{
	private x _3A_0018;

	public q()
	{
		_3A_0018 = new x();
	}

	~q()
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
		_3A_0018.HashCore(rgb, start, size);
	}

	protected override byte[] HashFinal()
	{
		State = 0;
		return _3A_0018.HashFinal();
	}

	public override void Initialize()
	{
		_3A_0018.Initialize();
	}
}
