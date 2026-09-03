namespace OluXNA;

internal class HPCondition : ICondition
{
	private int hp;

	private int limit;

	public bool ConditionMet()
	{
		return hp <= limit;
	}

	public HPCondition()
	{
	}

	public HPCondition(ref int _hp, int value)
	{
		hp = _hp;
		limit = value;
	}

	public void Update()
	{
	}

	public void Start()
	{
	}
}
