namespace OluXNA;

internal class BeatCondition : ICondition
{
	public double timeLimit;

	public Beats beatWait;

	public bool ConditionMet()
	{
		if (BaseGame.Get().totalTime >= timeLimit)
		{
			return true;
		}
		return false;
	}

	public BeatCondition()
	{
	}

	public BeatCondition(int _beats)
	{
		timeLimit = (float)_beats * BaseGame.BEAT;
		beatWait = Beats.Sixteenth;
	}

	public BeatCondition(int _beats, Beats _bWait)
	{
		timeLimit = (float)_beats * BaseGame.BEAT;
		beatWait = _bWait;
	}

	public BeatCondition(BeatCondition other)
	{
		timeLimit = other.timeLimit;
		beatWait = other.beatWait;
	}

	public void Update()
	{
	}

	public void Start()
	{
		timeLimit += BaseGame.Get().totalTime;
	}
}
