namespace OluXNA;

internal class NeverCondition : ICondition
{
	public bool ConditionMet()
	{
		return false;
	}

	public void Update()
	{
	}

	public void Start()
	{
	}
}
