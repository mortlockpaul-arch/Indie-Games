namespace StarbeamDefenderGame;

public class UpgradePoints
{
	private int ongoingcount;

	private int totalcount;

	private int upgradepoints;

	public int AvailableUpgrades
	{
		get
		{
			return upgradepoints;
		}
		set
		{
			upgradepoints = value;
		}
	}

	public bool MaxedOut
	{
		get
		{
			if (totalcount > 45000)
			{
				return true;
			}
			return false;
		}
	}

	public UpgradePoints()
	{
		ongoingcount = 0;
		totalcount = 0;
		upgradepoints = 0;
	}

	public bool IncrementScore(int incrementby)
	{
		totalcount += incrementby;
		if (totalcount > 45000)
		{
			return true;
		}
		ongoingcount += incrementby;
		while (ongoingcount > 999)
		{
			ongoingcount -= 1000;
			upgradepoints++;
		}
		return false;
	}
}
