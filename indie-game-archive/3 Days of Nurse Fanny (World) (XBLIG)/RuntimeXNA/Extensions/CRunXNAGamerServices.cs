using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using RuntimeXNA.Actions;
using RuntimeXNA.Conditions;
using RuntimeXNA.Expressions;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions;

internal class CRunXNAGamerServices : CRunExtension
{
	private const int CND_CANPURCHASE = 0;

	private const int CND_ISCONNECTED = 1;

	private const int CND_ISSIGNEDIN = 2;

	private const int CND_LAST = 3;

	private const int ACT_CONNECT = 0;

	private const int ACT_SHOWMARKETPLACE = 1;

	private const int ACT_SIGNIN = 2;

	private const int ACT_SETPRESENCE = 3;

	private const int ACT_SETPRESENCEVALUE = 4;

	private const int EXP_NSIGNEDIN = 0;

	private const int EXP_ALIAS = 1;

	private const int EXP_PRESENCE = 2;

	private const int EXP_PRESENCEVALUE = 3;

	private const int FLAG_CANPURCHASE = 1;

	private const int FLAG_CONNECTATSTART = 2;

	private const int FLAG_SIGNINATSTART = 4;

	private const int FLAG_SIGNINONLINE = 8;

	private int flags;

	private short signInPanes;

	private static GamerPresenceMode[] modes = new GamerPresenceMode[61]
	{
		GamerPresenceMode.ArcadeMode,
		GamerPresenceMode.AtMenu,
		GamerPresenceMode.BattlingBoss,
		GamerPresenceMode.CampaignMode,
		GamerPresenceMode.ChallengeMode,
		GamerPresenceMode.ConfiguringSettings,
		GamerPresenceMode.CoOpLevel,
		GamerPresenceMode.CoOpStage,
		GamerPresenceMode.CornflowerBlue,
		GamerPresenceMode.CustomizingPlayer,
		GamerPresenceMode.DifficultyEasy,
		GamerPresenceMode.DifficultyExtreme,
		GamerPresenceMode.DifficultyHard,
		GamerPresenceMode.DifficultyMedium,
		GamerPresenceMode.EditingLevel,
		GamerPresenceMode.ExplorationMode,
		GamerPresenceMode.FoundSecret,
		GamerPresenceMode.FreePlay,
		GamerPresenceMode.GameOver,
		GamerPresenceMode.InCombat,
		GamerPresenceMode.InGameStore,
		GamerPresenceMode.Level,
		GamerPresenceMode.LocalCoOp,
		GamerPresenceMode.LocalVersus,
		GamerPresenceMode.LookingForGames,
		GamerPresenceMode.Losing,
		GamerPresenceMode.Multiplayer,
		GamerPresenceMode.NearlyFinished,
		GamerPresenceMode.None,
		GamerPresenceMode.OnARoll,
		GamerPresenceMode.OnlineCoOp,
		GamerPresenceMode.OnlineVersus,
		GamerPresenceMode.Outnumbered,
		GamerPresenceMode.Paused,
		GamerPresenceMode.PlayingMinigame,
		GamerPresenceMode.PlayingWithFriends,
		GamerPresenceMode.PracticeMode,
		GamerPresenceMode.PuzzleMode,
		GamerPresenceMode.ScenarioMode,
		GamerPresenceMode.Score,
		GamerPresenceMode.ScoreIsTied,
		GamerPresenceMode.SettingUpMatch,
		GamerPresenceMode.SinglePlayer,
		GamerPresenceMode.Stage,
		GamerPresenceMode.StartingGame,
		GamerPresenceMode.StoryMode,
		GamerPresenceMode.StuckOnAHardBit,
		GamerPresenceMode.SurvivalMode,
		GamerPresenceMode.TimeAttack,
		GamerPresenceMode.TryingForRecord,
		GamerPresenceMode.TutorialMode,
		GamerPresenceMode.VersusComputer,
		GamerPresenceMode.VersusScore,
		GamerPresenceMode.WaitingForPlayers,
		GamerPresenceMode.WaitingInLobby,
		GamerPresenceMode.WastingTime,
		GamerPresenceMode.WatchingCredits,
		GamerPresenceMode.WatchingCutscene,
		GamerPresenceMode.Winning,
		GamerPresenceMode.WonTheGame,
		GamerPresenceMode.None
	};

	private static string[] names = new string[61]
	{
		"ArcadeMode", "AtMenu", "BattlingBoss", "CampaignMode", "ChallengeMode", "ConfiguringSettings", "CoOpLevel", "CoOpStage", "CornflowerBlue", "CustomizingPlayer",
		"DifficultyEasy", "DifficultyExtreme", "DifficultyHard", "DifficultyMedium", "EditingLevel", "ExplorationMode", "FoundSecret", "FreePlay", "GameOver", "InCombat",
		"InGameStore", "Level", "LocalCoOp", "LocalVersus", "LookingForGames", "Losing", "Multiplayer", "NearlyFinished", "None", "OnARoll",
		"OnlineCoOp", "OnlineVersus", "Outnumbered", "Paused", "PlayingMinigame", "PlayingWithFriends", "PracticeMode", "PuzzleMode", "ScenarioMode", "Score",
		"ScoreIsTied", "SettingUpMatch", "SinglePlayer", "Stage", "StartingGame", "StoryMode", "StuckOnAHardBit", "SurvivalMode", "TimeAttack", "TryingForRecord",
		"TutorialMode", "VersusComputer", "VersusScore", "WaitingForPlayers", "WaitingInLobby", "WastingTime", "WatchingCredits", "WatchingCutscene", "Winning", "WonTheGame",
		""
	};

	public override int getNumberOfConditions()
	{
		return 3;
	}

	public override bool createRunObject(CFile file, CCreateObjectInfo cob, int version)
	{
		flags = file.readAInt();
		signInPanes = file.readAShort();
		if ((flags & 2) != 0 && !GamerServicesDispatcher.IsInitialized)
		{
			flags &= -3;
			GamerServicesDispatcher.WindowHandle = rh.rhApp.game.Window.Handle;
			GamerServicesDispatcher.Initialize(rh.rhApp.game.Services);
		}
		return true;
	}

	public override int handleRunObject()
	{
		if ((flags & 4) != 0 && GamerServicesDispatcher.IsInitialized && !rh.rhApp.bSignedIn)
		{
			flags &= -5;
			Guide.ShowSignIn(signInPanes, (flags & 8) != 0);
			rh.rhApp.bSignedIn = true;
		}
		return 0;
	}

	public override bool condition(int num, CCndExtension cnd)
	{
		return num switch
		{
			0 => cndCanPurchase(cnd), 
			1 => GamerServicesDispatcher.IsInitialized, 
			2 => cndIsSignedIn(), 
			_ => false, 
		};
	}

	private bool cndIsSignedIn()
	{
		for (int i = 1; i <= 4; i++)
		{
			PlayerIndex player = getPlayer(i);
			if (Gamer.SignedInGamers[player] != null)
			{
				return true;
			}
		}
		return false;
	}

	private PlayerIndex getPlayer(int p)
	{
		PlayerIndex result = PlayerIndex.One;
		switch (p)
		{
		case 1:
			result = PlayerIndex.One;
			break;
		case 2:
			result = PlayerIndex.Two;
			break;
		case 3:
			result = PlayerIndex.Three;
			break;
		case 4:
			result = PlayerIndex.Four;
			break;
		}
		return result;
	}

	private bool cndCanPurchase(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		PlayerIndex player = getPlayer(paramExpression);
		if (Gamer.SignedInGamers[player] != null)
		{
			GamerPrivileges privileges = Gamer.SignedInGamers[player].Privileges;
			return privileges.AllowPurchaseContent;
		}
		return false;
	}

	public override void action(int num, CActExtension act)
	{
		switch (num)
		{
		case 0:
			actConnect();
			break;
		case 1:
			actShowMarketplace(act);
			break;
		case 2:
			actSignIn(act);
			break;
		case 3:
			actSetPresence(act);
			break;
		case 4:
			actSetPresenceValue(act);
			break;
		}
	}

	private void actConnect()
	{
		if (!GamerServicesDispatcher.IsInitialized)
		{
			GamerServicesDispatcher.WindowHandle = rh.rhApp.game.Window.Handle;
			GamerServicesDispatcher.Initialize(rh.rhApp.game.Services);
		}
	}

	private void actSignIn(CActExtension act)
	{
		int paramExpression = act.getParamExpression(rh, 0);
		bool onlineOnly = act.getParamExpression(rh, 1) != 0;
		if (GamerServicesDispatcher.IsInitialized)
		{
			Guide.ShowSignIn(paramExpression, onlineOnly);
			rh.rhApp.bSignedIn = cndIsSignedIn();
		}
	}

	private void actShowMarketplace(CActExtension act)
	{
		int paramExpression = act.getParamExpression(rh, 0);
		PlayerIndex player = getPlayer(paramExpression);
		Guide.ShowMarketplace(player);
	}

	private void actSetPresence(CActExtension act)
	{
		int paramExpression = act.getParamExpression(rh, 0);
		string paramExpString = act.getParamExpString(rh, 1);
		int i;
		for (i = 0; names[i] != "" && !(names[i] == paramExpString); i++)
		{
		}
		if (names[i] != "")
		{
			PlayerIndex player = getPlayer(paramExpression);
			SignedInGamer signedInGamer = Gamer.SignedInGamers[player];
			if (signedInGamer != null)
			{
				signedInGamer.Presence.PresenceMode = modes[i];
			}
		}
	}

	private void actSetPresenceValue(CActExtension act)
	{
		int paramExpression = act.getParamExpression(rh, 0);
		int paramExpression2 = act.getParamExpression(rh, 1);
		PlayerIndex player = getPlayer(paramExpression);
		SignedInGamer signedInGamer = Gamer.SignedInGamers[player];
		if (signedInGamer != null)
		{
			signedInGamer.Presence.PresenceValue = paramExpression2;
		}
	}

	public override CValue expression(int num)
	{
		return num switch
		{
			0 => expNSignedIn(), 
			1 => expAlias(), 
			2 => expPresence(), 
			3 => expPresenceValue(), 
			_ => new CValue(0), 
		};
	}

	private CValue expPresence()
	{
		int p = ho.getExpParam().getInt();
		PlayerIndex player = getPlayer(p);
		SignedInGamer signedInGamer = Gamer.SignedInGamers[player];
		if (signedInGamer != null)
		{
			GamerPresenceMode presenceMode = signedInGamer.Presence.PresenceMode;
			GamerPresenceMode gamerPresenceMode = GamerPresenceMode.None;
			for (int i = 0; modes[i] != gamerPresenceMode; i++)
			{
				if (presenceMode == modes[i])
				{
					return new CValue(names[i]);
				}
			}
		}
		return new CValue("");
	}

	private CValue expPresenceValue()
	{
		int p = ho.getExpParam().getInt();
		PlayerIndex player = getPlayer(p);
		SignedInGamer signedInGamer = Gamer.SignedInGamers[player];
		if (signedInGamer != null)
		{
			return new CValue(signedInGamer.Presence.PresenceValue);
		}
		return new CValue(0);
	}

	private CValue expNSignedIn()
	{
		int num = 0;
		for (int i = 1; i <= 4; i++)
		{
			PlayerIndex player = getPlayer(i);
			if (Gamer.SignedInGamers[player] != null)
			{
				num++;
			}
		}
		return new CValue(num);
	}

	private CValue expAlias()
	{
		int p = ho.getExpParam().getInt();
		PlayerIndex player = getPlayer(p);
		SignedInGamer signedInGamer = Gamer.SignedInGamers[player];
		if (signedInGamer != null)
		{
			return new CValue(signedInGamer.Gamertag);
		}
		return new CValue("");
	}
}
