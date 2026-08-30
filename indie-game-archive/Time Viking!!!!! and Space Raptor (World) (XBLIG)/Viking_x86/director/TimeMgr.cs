using IMAK3Z0MB1EGAEM.director;

namespace Viking_x86.director;

public class TimeMgr
{
	public const int ZOMBIE = 0;

	public const int VIKING = 1;

	public static BaseTimeMgr[] timeMgr = new BaseTimeMgr[2]
	{
		new ZombieTimeMgr(),
		new VikingTimeMgr()
	};

	public static int time;

	public static BaseTimeMgr ZombieTMgr()
	{
		return timeMgr[0];
	}

	public static BaseTimeMgr VikingTMgr()
	{
		return timeMgr[1];
	}

	public static BaseTimeMgr CurTMgr()
	{
		if (time >= timeMgr.Length)
		{
			time = 0;
		}
		return timeMgr[time];
	}
}
