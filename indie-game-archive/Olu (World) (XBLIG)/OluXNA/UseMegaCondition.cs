namespace OluXNA;

internal class UseMegaCondition : ICondition
{
	private bool _condMet;

	public bool ConditionMet()
	{
		return _condMet;
	}

	public void Update()
	{
		_condMet = _condMet || BaseGame.Get().MEGA_ON;
	}

	public void Start()
	{
	}
}
