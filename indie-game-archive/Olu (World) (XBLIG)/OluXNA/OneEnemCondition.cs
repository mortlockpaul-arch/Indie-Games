namespace OluXNA;

internal class OneEnemCondition : ICondition
{
	public bool ConditionMet()
	{
		return BaseGame.Get().actualEnem == 1;
	}

	public void Update()
	{
	}

	public void Start()
	{
	}
}
