using D;

namespace J;

internal abstract class _7
{
	protected bool allowOnDemandConstruction = true;

	public abstract int Count { get; set; }

	public bool AllowOnDemandConstruction
	{
		get
		{
			return allowOnDemandConstruction;
		}
		set
		{
			allowOnDemandConstruction = value;
		}
	}

	public abstract D.h GetNarrowPhasePair();

	public abstract void GiveBack(D.h pair);

	public void EnsureCount(int minimumCount)
	{
		if (Count < minimumCount)
		{
			Count = minimumCount;
		}
	}

	public void CapCount(int maximumCount)
	{
		if (Count > maximumCount)
		{
			Count = maximumCount;
		}
	}

	public abstract void Clear();
}
