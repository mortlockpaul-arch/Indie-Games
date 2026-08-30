using System.Collections.Generic;

namespace Skeleton;

public class TimedJointStateComparer : IComparer<TimedJointState>
{
	public int Compare(TimedJointState x, TimedJointState y)
	{
		if (x.ActivationTime > y.ActivationTime)
		{
			return 1;
		}
		if (x.ActivationTime == y.ActivationTime)
		{
			return 0;
		}
		return -1;
	}
}
