namespace OluXNA;

internal class UseFreezeCondition : ICondition
{
	private bool _condMet;

	public bool ConditionMet()
	{
		return _condMet;
	}

	public void Update()
	{
		_condMet = _condMet || BaseGame.Get().FREEZE_ON;
	}

	public void Start()
	{
	}
}
