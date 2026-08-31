namespace z;

internal struct K
{
	public byte[] P;

	public byte[] Q;

	public byte[] D;

	public byte[] DP;

	public byte[] DQ;

	public byte[] InverseQ;

	public byte[] Modulus;

	public byte[] Exponent;
}
internal abstract class k : Z
{
	protected k()
	{
		HashSizeValue = 160;
	}

	public new static k Create()
	{
		return new _0013();
	}
}
