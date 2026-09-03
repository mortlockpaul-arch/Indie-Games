using System.Collections.Generic;

namespace OluXNA;

internal class ScoreGroup
{
	public int totalPoints;

	public List<int> scores;

	public ScoreGroup()
	{
		scores = new List<int>();
	}

	public ScoreGroup(ScoreGroup other)
	{
		totalPoints = other.totalPoints;
		scores = new List<int>();
		for (int i = 0; i < other.scores.Count; i++)
		{
			scores.Add(other.scores[i]);
		}
	}

	public ScoreGroup(int _total, int _score)
	{
		totalPoints = _total;
		scores = new List<int>();
		scores.Add(_score);
	}
}
