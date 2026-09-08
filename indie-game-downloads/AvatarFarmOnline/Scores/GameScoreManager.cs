using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Scores;
using Quasar.GameUtils.XBLIG.Scores;

namespace AvatarFarmOnline.Scores;

internal class GameScoreManager : XBLIGScoreManager<AvatarFarmOnline.Scores.GameHighscore>
{
	private static AvatarFarmOnline.Scores.GameScoreManager instance;

	private static bool enableP2P = true;

	public static AvatarFarmOnline.Scores.GameScoreManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AvatarFarmOnline.Scores.GameScoreManager(enableP2P);
			}
			return instance;
		}
	}

	public new AvatarFarmOnline.Scores.GameScoreOrganizer ScoreOrganizer => base.ScoreOrganizer as AvatarFarmOnline.Scores.GameScoreOrganizer;

	public static bool EnableP2P
	{
		get
		{
			return enableP2P;
		}
		set
		{
			if (instance != null)
			{
				instance.Enabled = value;
			}
			enableP2P = value;
		}
	}

	public void SetNewScore(PlayerIndex player, uint maxLevel, int coins, int cash, int ticks)
	{
		base.SetNewScore(new AvatarFarmOnline.Scores.GameHighscore(Player.GetPlayerName(player), DateTime.Now, maxLevel, coins, cash, ticks, IsLocal: true));
	}

	protected GameScoreManager(bool enabled)
		: base(new KeyValuePair<int, int>(0, 1), (ScoreOrganizer<AvatarFarmOnline.Scores.GameHighscore>)new AvatarFarmOnline.Scores.GameScoreOrganizer())
	{
		base.Enabled = enabled;
	}
}
