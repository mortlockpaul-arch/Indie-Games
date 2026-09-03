namespace OluXNA;

internal class BoneModelObjectTarget : BoneModelTarget
{
	public object obj;

	public BoneModelObjectTarget()
	{
	}

	public BoneModelObjectTarget(BoneModelObjectTarget other)
		: base(other)
	{
		obj = other.obj;
	}
}
