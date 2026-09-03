namespace OluXNA;

internal class NotTutorialCondition : ICondition
{
	public bool ConditionMet()
	{
		return BaseGame.Get().player.level == 1;
	}

	public void Update()
	{
	}

	public void Start()
	{
	}
}
