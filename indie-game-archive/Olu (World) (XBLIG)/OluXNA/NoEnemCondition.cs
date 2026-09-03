namespace OluXNA;

internal class NoEnemCondition : ICondition
{
	public bool ConditionMet()
	{
		return BaseGame.Get().actualEnem == 0;
	}

	public void Update()
	{
	}

	public void Start()
	{
	}
}
