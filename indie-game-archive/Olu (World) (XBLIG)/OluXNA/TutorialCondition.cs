namespace OluXNA;

internal class TutorialCondition : ICondition
{
	public bool ConditionMet()
	{
		return BaseGame.Get().player.level > 1;
	}

	public void Update()
	{
	}

	public void Start()
	{
	}
}
