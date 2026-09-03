namespace OluXNA;

internal class ChanCondition : ICondition
{
	public float threshold;

	public int channel;

	public bool ready;

	public bool ConditionMet()
	{
		if (BaseGame.Get().channels[channel] > threshold && ready)
		{
			return true;
		}
		return false;
	}

	public ChanCondition()
	{
	}

	public ChanCondition(int _chan, float _thres)
	{
		channel = _chan;
		threshold = _thres;
		ready = false;
		if (BaseGame.Get().channels[channel] <= threshold)
		{
			ready = true;
		}
	}

	public ChanCondition(ChanCondition other)
	{
		threshold = other.threshold;
		channel = other.channel;
	}

	public void Update()
	{
		if (BaseGame.Get().channels[channel] <= threshold)
		{
			ready = true;
		}
	}

	public void Start()
	{
	}
}
