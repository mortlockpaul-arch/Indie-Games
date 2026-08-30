using System.Collections.Generic;
using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public class GameModeRules
{
	public enum Type
	{
		SinglePlayer,
		MultiPlayer
	}

	public enum SolidOrStrips
	{
		Solid,
		Strips
	}

	public class Team
	{
		public enum Index
		{
			TeamA,
			TeamB
		}

		private Index index;

		public static Color[] Colors = new Color[2]
		{
			Utils.ColorFromHexaString("257ef7"),
			Utils.ColorFromHexaString("e51c27")
		};

		public List<int> Pocketed;

		public SolidOrStrips color;

		public List<PlayerIndex> players;

		private int currentPlayer;

		public int number => (int)index;

		public PlayerIndex CurrentPlayer => players[currentPlayer];

		public bool AnyCpu
		{
			get
			{
				foreach (PlayerIndex player in players)
				{
					if (player == IndexCPU)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Team(Index index, List<PlayerIndex> players)
		{
			this.index = index;
			this.players = players;
			currentPlayer = 0;
			Pocketed = new List<int>();
			color = (SolidOrStrips)(-1);
		}

		public void NextPlayer()
		{
			currentPlayer = (currentPlayer + 1) % players.Count;
		}

		public static string NameOf(PlayerIndex pInd)
		{
			if (pInd == IndexCPU)
			{
				return "CPU";
			}
			return "P " + ((int)(pInd + 1)).ToString("0");
		}
	}

	public static Type type;

	private static bool solidOrStripsDecided;

	public static readonly PlayerIndex IndexCPU = (PlayerIndex)(-1);

	private static Team TeamA;

	private static Team TeamB;

	public static Team[] AllTeams = new Team[2];

	private static bool teamTurn;

	private static bool breakDone;

	private static List<int> pocketedThisTurn = new List<int>();

	private static bool anyMobileColl;

	private static int firstObjectBallHit = -1;

	private static int lowestNumericalObjectBall = -2;

	private static bool popupControlIsDone = false;

	private static bool changeTeam;

	private static bool wballPocketed;

	private static bool foulNineBall;

	public static bool deferredInitiateNextTurn = false;

	public static Team TeamCurrent
	{
		get
		{
			if (!teamTurn)
			{
				return TeamB;
			}
			return TeamA;
		}
	}

	private static Team TeamNotCurrent
	{
		get
		{
			if (teamTurn)
			{
				return TeamB;
			}
			return TeamA;
		}
	}

	public static PlayerIndex CurrentPlayer => TeamCurrent.CurrentPlayer;

	public static SolidOrStrips CurrentColor => TeamCurrent.color;

	public static bool BreakDone => breakDone;

	public static int LowestNumericalObjectBall => lowestNumericalObjectBall;

	public static string SolidOrStripsToString(SolidOrStrips s)
	{
		return s switch
		{
			SolidOrStrips.Solid => "Solids", 
			SolidOrStrips.Strips => "Strips", 
			_ => "", 
		};
	}

	private static void NextTeam()
	{
		teamTurn = !teamTurn;
		TeamCurrent.NextPlayer();
	}

	public static int TeamNumForBallNum(int ballNum)
	{
		if (BallColor(ballNum) == TeamA.color)
		{
			return TeamA.number;
		}
		return TeamB.number;
	}

	public static void Register_WhiteBall_ObjectBall_Collision(int objectBallId)
	{
		if (firstObjectBallHit == -1)
		{
			firstObjectBallHit = objectBallId;
		}
	}

	public static void InitializeMultiPlayer(GameTime gameTime, List<PlayerIndex> playersA, List<PlayerIndex> playersB, bool promptForAddCpu)
	{
		if (playersA.Count == 0)
		{
			playersA.Add(IndexCPU);
		}
		else if (playersB.Count == 0)
		{
			playersB.Add(IndexCPU);
		}
		else if (promptForAddCpu && playersA.Count + playersB.Count == 3)
		{
			Statics.menus.PromptAddCpu(gameTime, playersA, playersB);
			return;
		}
		TeamA = new Team(Team.Index.TeamA, playersA);
		TeamB = new Team(Team.Index.TeamB, playersB);
		InitializeStep1(gameTime, Type.MultiPlayer);
	}

	public static void InitializeSinglePlayer(GameTime gameTime, PlayerIndex pIndex)
	{
		TeamA = new Team(Team.Index.TeamA, new List<PlayerIndex> { pIndex });
		TeamB = new Team(Team.Index.TeamB, new List<PlayerIndex> { IndexCPU });
		InitializeStep1(gameTime, Type.SinglePlayer);
	}

	private static void InitializeStep1(GameTime gameTime, Type newType)
	{
		if (TeamA.AnyCpu || TeamB.AnyCpu)
		{
			Statics.menus.PromptCpuLevel(gameTime, newType);
		}
		else
		{
			InitializeFinal(gameTime, newType);
		}
	}

	public static void InitializeFinal(GameTime gameTime, Type newType)
	{
		AllTeams[0] = TeamA;
		AllTeams[1] = TeamB;
		type = newType;
		solidOrStripsDecided = false;
		teamTurn = true;
		Statics.callbacks.ResetBalls();
		GameState.ChangeWithTransition((MaximinusGame.Id != MaximinusGame.ID.Billard9Ball) ? GameState.Type.AIMING : GameState.Type.REPOSITION_WBALL, gameTime);
		ResetVar();
		InfoDisplay.Reset();
		breakDone = false;
		if (Trial.IsTrial && !popupControlIsDone)
		{
			popupControlIsDone = true;
			Statics.menus.popupControls.Enable(gameTime);
		}
		else
		{
			Statics.menus.YourTurn("");
		}
	}

	private static void ResetVar()
	{
		firstObjectBallHit = -1;
		lowestNumericalObjectBall = 28;
		foreach (Ball ball in Statics.balls)
		{
			if (ball.Number > 0 && ball.Alive && ball.Number < lowestNumericalObjectBall)
			{
				lowestNumericalObjectBall = ball.Number;
			}
		}
		pocketedThisTurn.Clear();
		anyMobileColl = false;
	}

	public static void NewTurn(GameTime gameTime)
	{
		string text = "";
		changeTeam = true;
		wballPocketed = pocketedThisTurn.Contains(0);
		foulNineBall = false;
		bool flag = pocketedThisTurn.Contains((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 9 : 8);
		bool flag2 = pocketedThisTurn.Contains(9);
		if (!breakDone)
		{
			if (MaximinusGame.Id != MaximinusGame.ID.Billard9Ball && flag)
			{
				text = "ILLEGAL BREAK" + Utils.newLine + "POCKETED THE 8 BALL";
				changeTeam = true;
			}
			else if (MaximinusGame.Id != MaximinusGame.ID.Billard9Ball && !anyMobileColl)
			{
				text = "ILLEGAL BREAK";
				changeTeam = true;
			}
			else
			{
				breakDone = true;
			}
		}
		if (!breakDone)
		{
			InfoDisplay.Reset();
			Statics.callbacks.ResetBalls();
		}
		else
		{
			bool flag3 = false;
			if (wballPocketed)
			{
				pocketedThisTurn.Remove(0);
				flag3 = true;
				changeTeam = true;
				text = "FOUL" + Utils.newLine + "POCKETED THE WHITE BALL";
			}
			if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball && firstObjectBallHit != lowestNumericalObjectBall)
			{
				flag3 = true;
				changeTeam = true;
				text = "FOUL" + Utils.newLine + "THE WHITE BALL FIRST CONTACT MUST BE" + Utils.newLine + "THE LOWEST COLOR BALL ON TABLE : " + lowestNumericalObjectBall;
			}
			if ((MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? flag2 : flag)
			{
				if (MaximinusGame.Id != MaximinusGame.ID.Billard9Ball)
				{
					pocketedThisTurn.Remove((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 9 : 8);
				}
				int number;
				if (flag3)
				{
					number = TeamNotCurrent.number;
					Statics.menus.Message(text);
				}
				else if (MaximinusGame.Id != MaximinusGame.ID.Billard9Ball && (TeamCurrent.Pocketed.Count != ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 13 : 7) || AnyPocketedThisTurnOfColor(TeamCurrent.color)))
				{
					number = TeamNotCurrent.number;
					Statics.menus.Message("FOUL" + Utils.newLine + "POCKETED THE BLACK BALL" + Utils.newLine + "BEFORE ALL THE COLOR BALLS");
				}
				else
				{
					number = TeamCurrent.number;
				}
				Statics.callbacks.GameOver(gameTime, number);
				return;
			}
			if (MaximinusGame.Id != MaximinusGame.ID.Billard9Ball)
			{
				int num = -1;
				foreach (int item in pocketedThisTurn)
				{
					if (!flag3 && TeamCurrent.color != BallColor(item))
					{
						num = item;
						flag3 = true;
					}
				}
				if (flag3 && num != -1)
				{
					text = "FOUL" + Utils.newLine + "POCKETED THE WRONG COLOR";
					changeTeam = true;
				}
			}
			else
			{
				foulNineBall = flag3;
			}
			if (!flag3 && pocketedThisTurn.Count > 0)
			{
				changeTeam = false;
			}
		}
		if (text != "")
		{
			Statics.menus.Message(text);
		}
		ResetVar();
		deferredInitiateNextTurn = true;
	}

	public static void Update(GameTime gameTime)
	{
		if (deferredInitiateNextTurn && Cue.obj.Alpha == 0f)
		{
			deferredInitiateNextTurn = false;
			if (changeTeam)
			{
				NextTeam();
				Statics.menus.YourTurn("");
			}
			GameState.ChangeWithTransition((!wballPocketed && !foulNineBall) ? GameState.Type.AIMING : GameState.Type.REPOSITION_WBALL, gameTime);
		}
	}

	private static bool AnyPocketedThisTurnOfColor(SolidOrStrips color)
	{
		foreach (int item in pocketedThisTurn)
		{
			if (BallColor(item) == color)
			{
				return true;
			}
		}
		return false;
	}

	public static List<PlayerIndex> PlayerIndexes(int teamNum)
	{
		Team team = ((teamNum == TeamA.number) ? TeamA : TeamB);
		return team.players;
	}

	public static SolidOrStrips BallColor(int ballNum)
	{
		if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			if (ballNum % 2 != 0)
			{
				return SolidOrStrips.Solid;
			}
			return SolidOrStrips.Strips;
		}
		if (ballNum >= 8)
		{
			return SolidOrStrips.Strips;
		}
		return SolidOrStrips.Solid;
	}

	private static SolidOrStrips BallColorInverse(int ballNum)
	{
		if (BallColor(ballNum) != SolidOrStrips.Solid)
		{
			return SolidOrStrips.Solid;
		}
		return SolidOrStrips.Strips;
	}

	public static void MobileCollHasOccured()
	{
		if (!anyMobileColl)
		{
			anyMobileColl = true;
		}
	}

	public static void PocketedThisTurn(int ballNum)
	{
		pocketedThisTurn.Add(ballNum);
		if (!solidOrStripsDecided)
		{
			bool num;
			if (MaximinusGame.Id != MaximinusGame.ID.FunkyPool)
			{
				num = ballNum % 8 != 0;
			}
			else
			{
				if (ballNum == 0)
				{
					goto IL_005d;
				}
				num = ballNum != 9;
			}
			if (num)
			{
				TeamCurrent.color = BallColor(ballNum);
				TeamNotCurrent.color = BallColorInverse(ballNum);
				solidOrStripsDecided = true;
			}
		}
		goto IL_005d;
		IL_005d:
		if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball)
		{
			if (ballNum != 0)
			{
				TeamCurrent.Pocketed.Add(ballNum);
				InfoDisplay.Pocketed(ballNum, TeamCurrent.number);
			}
			return;
		}
		bool num2;
		if (MaximinusGame.Id != MaximinusGame.ID.FunkyPool)
		{
			num2 = ballNum % 8 != 0;
		}
		else
		{
			if (ballNum == 9)
			{
				return;
			}
			num2 = ballNum != 0;
		}
		if (num2)
		{
			Team team = ((TeamCurrent.color == BallColor(ballNum)) ? TeamCurrent : TeamNotCurrent);
			team.Pocketed.Add(ballNum);
			InfoDisplay.Pocketed(ballNum, team.number);
		}
	}
}
