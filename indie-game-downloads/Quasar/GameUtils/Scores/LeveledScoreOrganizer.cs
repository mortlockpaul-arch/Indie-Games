using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Player;

namespace Quasar.GameUtils.Scores;

public class LeveledScoreOrganizer<T> : ScoreOrganizer<T> where T : Highscore, IEquatable<T>, IComparable<T>, ILeveledHighscore, new()
{
	private Dictionary<string, int> usedNames = new Dictionary<string, int>(50);

	private List<T> names = new List<T>(50);

	private Dictionary<int, List<T>>[] levels;

	private int levelFilter = 1;

	private List<T> tempList = new List<T>();

	public int LevelFilter
	{
		get
		{
			return levelFilter;
		}
		set
		{
			levelFilter = value;
		}
	}

	public LeveledScoreOrganizer()
	{
		usedNames = new Dictionary<string, int>(MaxEntryCount(ScorePeriod.AllTime));
		names = new List<T>(MaxEntryCount(ScorePeriod.AllTime));
		levels = new Dictionary<int, List<T>>[4];
		for (int i = 0; i < 4; i++)
		{
			levels[i] = new Dictionary<int, List<T>>(1);
		}
	}

	public override void CheckPrune(List<T> scores, ScorePeriod period, bool isAggregate)
	{
		if (isAggregate)
		{
			Dictionary<int, List<T>> dictionary = levels[(int)period];
			foreach (KeyValuePair<int, List<T>> item in dictionary)
			{
				item.Value.Clear();
			}
			foreach (T score in scores)
			{
				T current = score;
				if (!dictionary.TryGetValue(current.Level, out var value))
				{
					value = new List<T>(10);
					dictionary.Add(current.Level, value);
				}
				value.Add(current);
			}
			int num = 0;
			foreach (KeyValuePair<int, List<T>> item2 in dictionary)
			{
				usedNames.Clear();
				names.Clear();
				foreach (T item3 in item2.Value)
				{
					T current3 = item3;
					if (!current3.CompareGamer(Quasar.GameUtils.Player.Player.GuestName) && !usedNames.ContainsKey(current3.Gamer))
					{
						usedNames.Add(current3.Gamer, 0);
						names.Add(current3);
					}
				}
				num += names.Count;
			}
			if (num != scores.Count)
			{
				scores.Clear();
				scores.AddRange(names);
			}
		}
		base.CheckPrune(scores, period, isAggregate);
	}

	protected override int GetHighscores(List<T> space, SearchMode searchMode, PlayerIndex playerIndex, ScorePeriod scorePeriod)
	{
		base.GetHighscores(tempList, searchMode, playerIndex, scorePeriod);
		space.Clear();
		foreach (T temp in tempList)
		{
			T current = temp;
			if (current.Level == levelFilter)
			{
				space.Add(current);
			}
		}
		return space.Count;
	}
}
