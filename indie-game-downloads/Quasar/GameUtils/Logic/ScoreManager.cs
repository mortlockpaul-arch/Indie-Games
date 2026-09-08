using System;
using System.Collections.Generic;
using Quasar.Global;

namespace Quasar.GameUtils.Logic;

public class ScoreManager : IComparer<ScoreManager.ScoreData>
{
	public class ScoreData
	{
		private uint playerId;

		private long score;

		public uint PlayerId => playerId;

		public long Score => score;

		public ScoreData(uint id)
		{
			playerId = id;
			score = 0L;
		}

		public void SetScore(long score)
		{
			this.score = score;
		}

		public void AddScore(long score)
		{
			this.score += score;
		}
	}

	private Func<long, string> scoreTextFunc;

	private Dictionary<uint, ScoreData> scores = new Dictionary<uint, ScoreData>();

	private List<ScoreData> temp = new List<ScoreData>(4);

	public Func<long, string> ScoreToStringFunc
	{
		set
		{
			scoreTextFunc = value;
		}
	}

	public Dictionary<uint, ScoreData> Scores => scores;

	public ScoreManager(List<uint> playerIds)
	{
		foreach (uint playerId in playerIds)
		{
			scores.Add(playerId, new ScoreData(playerId));
		}
	}

	public void GetSortedScores(List<ScoreData> list)
	{
		list.Clear();
		foreach (ScoreData value in scores.Values)
		{
			list.Add(value);
		}
		list.Sort(this);
	}

	public int GetPosition(uint pi)
	{
		GetSortedScores(temp);
		for (int i = 0; i < temp.Count; i++)
		{
			if (temp[i].PlayerId == pi)
			{
				return i;
			}
		}
		return -1;
	}

	public long GetScore(uint pi)
	{
		if (scores.TryGetValue(pi, out var value))
		{
			return value.Score;
		}
		return 0L;
	}

	public string GetScoreText(uint pi)
	{
		long score = GetScore(pi);
		if (scoreTextFunc != null)
		{
			return scoreTextFunc(score);
		}
		return GameMath.LongToString(score, "#,0");
	}

	public int Compare(ScoreData x, ScoreData y)
	{
		return y.Score.CompareTo(x.Score);
	}
}
