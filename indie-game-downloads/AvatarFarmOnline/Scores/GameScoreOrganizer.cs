using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Scores;
using Quasar.GameUtils.XBLIG;
using Quasar.Language;

namespace AvatarFarmOnline.Scores;

internal class GameScoreOrganizer : ScoreOrganizer<AvatarFarmOnline.Scores.GameHighscore>
{
	private Dictionary<string, int> usedNames = new Dictionary<string, int>(50);

	private List<AvatarFarmOnline.Scores.GameHighscore> names = new List<AvatarFarmOnline.Scores.GameHighscore>(50);

	public override void NotifyScores(PlayerIndex who)
	{
		XBLIGUtils.SendToFriends(who, LanguageManager.Texts["SCORE_NOTIFY"]);
	}

	public override void CheckPrune(List<AvatarFarmOnline.Scores.GameHighscore> scores, ScorePeriod period, bool isAggregate)
	{
		usedNames.Clear();
		names.Clear();
		foreach (AvatarFarmOnline.Scores.GameHighscore score in scores)
		{
			if (isAggregate && score.CompareGamer(Player.GuestName))
			{
				continue;
			}
			if (usedNames.ContainsKey(score.Gamer))
			{
				if (!isAggregate)
				{
					base.ScoreManager.RemoveScore(score);
				}
			}
			else
			{
				usedNames.Add(score.Gamer, 0);
				names.Add(score);
			}
		}
		if (names.Count != scores.Count)
		{
			scores.Clear();
			scores.AddRange(names);
		}
		base.CheckPrune(scores, period, isAggregate);
	}
}
