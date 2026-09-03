namespace OluXNA;

internal class ZReversePosCondition : ICondition
{
	public float zLimit;

	public Enemy enem;

	public bool ConditionMet()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (enem.getPos().Z < zLimit)
		{
			return true;
		}
		return false;
	}

	public ZReversePosCondition()
	{
	}

	public ZReversePosCondition(float _limit, Enemy _enem)
	{
		zLimit = _limit;
		enem = _enem;
	}

	private ZReversePosCondition(ZPosCondition other)
	{
		zLimit = other.zLimit;
		enem = other.enem;
	}

	public void Update()
	{
	}

	public void Start()
	{
	}
}
