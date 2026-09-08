using d;
using r;

namespace m;

internal abstract class _6 : r.b
{
	protected r.B timeStepSettings;

	public r.B TimeStepSettings
	{
		get
		{
			return timeStepSettings;
		}
		set
		{
			timeStepSettings = value;
		}
	}

	protected _6(r.B P_0, d.b P_1)
		: this(P_0)
	{
		base.ThreadManager = P_1;
		base.AllowMultithreading = true;
	}

	protected _6(r.B P_0)
	{
		timeStepSettings = P_0;
		Enabled = true;
	}

	public abstract void Add(h updateable);

	public abstract void Remove(h updateable);
}
