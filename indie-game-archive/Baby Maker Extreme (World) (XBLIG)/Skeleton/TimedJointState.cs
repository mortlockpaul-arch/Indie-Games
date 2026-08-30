namespace Skeleton;

public class TimedJointState
{
	private JointState state;

	private int time;

	public int ActivationTime
	{
		get
		{
			return time;
		}
		set
		{
			time = value;
		}
	}

	public JointState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public TimedJointState(JointState s, int t)
	{
		state = s;
		time = t;
	}
}
