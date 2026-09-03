namespace OluXNA;

internal class TimeCondition : ICondition
{
	public double timeLimit;

	public bool ConditionMet()
	{
		if (BaseGame.Get().totalTime >= timeLimit)
		{
			return true;
		}
		return false;
	}

	public TimeCondition()
	{
	}

	public TimeCondition(double _time)
	{
		timeLimit = _time;
	}

	public TimeCondition(TimeCondition other)
	{
		timeLimit = other.timeLimit;
	}

	public void Update()
	{
	}

	public void Start()
	{
		timeLimit += BaseGame.Get().totalTime;
	}
}
