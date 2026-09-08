using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;

namespace Quasar.GameUtils.Scores;

public class SimpleScoreOrganizer<T> : ScoreOrganizer<T> where T : Highscore, IEquatable<T>, IComparable<T>, ISimpleHighscore, new()
{
	private Dictionary<string, int> usedNames = new Dictionary<string, int>(50);

	private List<T> names = new List<T>(50);

	private List<string> friends = new List<string>(10);

	public SimpleScoreOrganizer()
	{
		usedNames = new Dictionary<string, int>(MaxEntryCount(ScorePeriod.AllTime));
		names = new List<T>(MaxEntryCount(ScorePeriod.AllTime));
	}

	public override void CheckPrune(List<T> scores, ScorePeriod period, bool isAggregate)
	{
		if (isAggregate)
		{
			usedNames.Clear();
			names.Clear();
			foreach (T score in scores)
			{
				T current = score;
				if (!current.CompareGamer(Quasar.GameUtils.Player.Player.GuestName) && !usedNames.ContainsKey(current.Gamer))
				{
					usedNames.Add(current.Gamer, 0);
					names.Add(current);
				}
			}
			if (names.Count != scores.Count)
			{
				scores.Clear();
				scores.AddRange(names);
			}
		}
		base.CheckPrune(scores, period, isAggregate);
	}

	public int PositionForScore(long score, ScorePeriod scorePeriod)
	{
		int i = 0;
		for (List<T> list = base.ScoreManager.AggregateHighscores(scorePeriod); i < list.Count; i++)
		{
			T val = list[i];
			if (!Highscore.ReverseOrder)
			{
				if (val.Score <= score)
				{
					return i;
				}
			}
			else if (val.Score >= score)
			{
				return i;
			}
		}
		return i;
	}

	public int FriendsPositionForScore(PlayerIndex playerIndex, long score, ScorePeriod scorePeriod)
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
		int i = 0;
		int num = 0;
		for (List<T> list = base.ScoreManager.AggregateHighscores(scorePeriod); i < list.Count; i++)
		{
			T val = list[i];
			bool flag = false;
			for (int j = 0; j < friends.Count; j++)
			{
				_ = friends[j];
				string gamertag = friends[j];
				if (val.CompareGamer(gamertag))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			if (!Highscore.ReverseOrder)
			{
				if (val.Score <= score)
				{
					return num;
				}
			}
			else if (val.Score >= score)
			{
				return num;
			}
			num++;
		}
		return num;
	}
}
