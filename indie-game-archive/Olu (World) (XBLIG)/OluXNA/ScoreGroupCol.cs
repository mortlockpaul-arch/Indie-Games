using System.Collections.Generic;

namespace OluXNA;

internal class ScoreGroupCol
{
	private List<ScoreGroup> groups;

	public ScoreGroupCol()
	{
		groups = new List<ScoreGroup>();
	}

	public ScoreGroup Pop()
	{
		ScoreGroup result = null;
		if (!isEmpty())
		{
			result = new ScoreGroup(groups[0].totalPoints, groups[0].scores[0]);
			groups[0].scores.RemoveAt(0);
			if (groups[0].scores.Count == 0)
			{
				groups.RemoveAt(0);
			}
		}
		return result;
	}

	public void Push(ScoreGroup toAdd)
	{
		groups.Add(toAdd);
	}

	public bool isEmpty()
	{
		return groups.Count == 0;
	}
}
