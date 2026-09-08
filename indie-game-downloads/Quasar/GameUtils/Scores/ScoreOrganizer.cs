using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;

namespace Quasar.GameUtils.Scores;

public class ScoreOrganizer<T> where T : Highscore, IEquatable<T>, IComparable<T>, new()
{
	protected enum SearchMode
	{
		Local,
		Global,
		Friends
	}

	private ScoreManager<T> scoreManager;

	private List<string> friends = new List<string>(10);

	public ScoreManager<T> ScoreManager
	{
		get
		{
			return scoreManager;
		}
		set
		{
			scoreManager = value;
		}
	}

	public virtual int MaxEntryCount(ScorePeriod period)
	{
		return period switch
		{
			ScorePeriod.Monthly => 5000, 
			ScorePeriod.Weekly => 2000, 
			ScorePeriod.Daily => 1000, 
			_ => 10000, 
		};
	}

	public virtual void NotifyScores(PlayerIndex who)
	{
	}

	public virtual T BestEntry(string gamertag, ScorePeriod period, bool isLocal, out uint worldPosition)
	{
		uint num = 0u;
		foreach (T item in isLocal ? scoreManager.LocalAggregateHighscores(period) : scoreManager.AggregateHighscores(period))
		{
			T current = item;
			if (current.CompareGamer(gamertag))
			{
				worldPosition = num;
				return current;
			}
			num++;
		}
		worldPosition = 0u;
		return null;
	}

	public T BestEntry(PlayerIndex playerIndex, ScorePeriod period, bool isLocal, out uint worldPosition)
	{
		if (!Quasar.GameUtils.Player.Player.IsSignedIn(playerIndex))
		{
			worldPosition = 0u;
			return null;
		}
		return BestEntry(Quasar.GameUtils.Player.Player.GetPlayerName(playerIndex), period, isLocal, out worldPosition);
	}

	public virtual void CheckPrune(List<T> scores, ScorePeriod period, bool isAggregate)
	{
		int num = MaxEntryCount(period);
		if (scores.Count <= num)
		{
			return;
		}
		if (!isAggregate)
		{
			for (int i = num; i < scores.Count; i++)
			{
				scoreManager.RemoveScore(scores[i]);
			}
		}
		scores.RemoveRange(num, scores.Count - num);
	}

	public int GetHighscores(List<T> space, bool isGlobal, ScorePeriod scorePeriod)
	{
		return GetHighscores(space, isGlobal ? SearchMode.Global : SearchMode.Local, PlayerIndex.One, scorePeriod);
	}

	public int GetFriendHighScores(List<T> space, PlayerIndex playerIndex, ScorePeriod scorePeriod)
	{
		return GetHighscores(space, SearchMode.Friends, playerIndex, scorePeriod);
	}

	protected virtual int GetHighscores(List<T> space, SearchMode searchMode, PlayerIndex playerIndex, ScorePeriod scorePeriod)
	{
		space.Clear();
		List<T> list = null;
		switch (searchMode)
		{
		default:
			list = scoreManager.LocalAggregateHighscores(scorePeriod);
			break;
		case SearchMode.Global:
		case SearchMode.Friends:
			list = scoreManager.AggregateHighscores(scorePeriod);
			break;
		}
		if (searchMode == SearchMode.Friends)
		{
			friends.Clear();
			ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(playerIndex);
			friends.Add(Quasar.GameUtils.Player.Player.GetPlayerName(playerIndex));
			if (gamer != null)
			{
				foreach (IFriendGamer friend in gamer.GetFriends())
				{
					friends.Add(friend.Gamertag);
				}
			}
		}
		space.Capacity = list.Count;
		foreach (T item in list)
		{
			T current2 = item;
			if (searchMode == SearchMode.Friends)
			{
				bool flag = false;
				for (int i = 0; i < friends.Count; i++)
				{
					_ = friends[i];
					if (current2.CompareGamer(friends[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
			}
			space.Add(current2);
		}
		return space.Count;
	}
}
