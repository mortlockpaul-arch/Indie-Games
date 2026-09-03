using System.Collections.Generic;

namespace OluXNA;

internal class ConditionSet
{
	public List<ICondition> set;

	public ConditionSet()
	{
		set = new List<ICondition>();
	}

	public void Dispose()
	{
		set.Clear();
	}

	public void Update()
	{
		foreach (ICondition item in set)
		{
			item.Update();
		}
	}

	public void combineWith(ConditionSet other)
	{
		for (int i = 0; i < other.set.Count; i++)
		{
			set.Add(other.set[i]);
		}
	}

	public bool ConditionsMet()
	{
		if (set.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < set.Count; i++)
		{
			if (!set[i].ConditionMet())
			{
				return false;
			}
		}
		return true;
	}

	public void Start()
	{
		foreach (ICondition item in set)
		{
			item.Start();
		}
	}
}
