namespace OluXNA;

internal class Hit8Condition : ICondition
{
	private bool _condMet;

	public bool ConditionMet()
	{
		return _condMet;
	}

	public void Update()
	{
		_condMet = _condMet || BaseGame.Get().numTargeted == 8;
	}

	public void Start()
	{
	}
}
