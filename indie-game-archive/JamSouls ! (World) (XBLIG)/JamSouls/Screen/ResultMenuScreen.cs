using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace JamSouls.Screen;

internal class ResultMenuScreen : MenuScreen
{
	public const float TIME_OFFSET = 500f;

	private const int START_LEVEL = 9;

	private GameState gamestate;

	private Vector2 Apos = new Vector2(940f, 560f);

	private Vector2 Atextpos = new Vector2(1010f, 590f);

	public List<Vector2> pPosition = new List<Vector2>();

	public List<Vector2> JamPos = new List<Vector2>();

	public Vector2 m_StatOffset = Vector2.Zero;

	public int m_nElementToDisplay;

	public int m_nTotalElementToDisplay;

	private bool m_bIsExiting;

	public float currentTimer = 500f;

	public bool m_bGameIsDraw;

	public float m_ResultTimer = 1000f;

	public string m_Winner;

	public string m_Loser;

	public Color m_WinnerColor;

	private Sprite m_ResultLayer;

	public ResultMenuScreen(GameState thegamestate)
		: base("")
	{
		base.IsPopup = true;
		gamestate = thegamestate;
		MenuEntry menuEntry = new MenuEntry(TextManager.GetText(TextID.REMATCH));
		MenuEntry menuEntry2 = new MenuEntry(TextManager.GetText(TextID.REMATCH_RANDOM_MAP));
		MenuEntry menuEntry3 = new MenuEntry(TextManager.GetText(TextID.EXIT_MAIN_MENU));
		menuEntry.Selected += OnRematch;
		menuEntry2.Selected += OnRematchChangeMap;
		menuEntry3.Selected += OnExit;
		if (GameContext.GameMode != GAME_MODE.STORYMATCH)
		{
			base.MenuEntries.Add(menuEntry);
			base.MenuEntries.Add(menuEntry2);
		}
		base.MenuEntries.Add(menuEntry3);
		StartX = 500;
		StartY = 520;
		m_nTotalElementToDisplay = gamestate.m_Players.Count;
		int num = 4 - m_nTotalElementToDisplay;
		pPosition.Add(new Vector2(200 + num * 100, GameContext.TileSafeTop + 100));
		pPosition.Add(new Vector2(450 + num * 100, GameContext.TileSafeTop + 140));
		pPosition.Add(new Vector2(690 + num * 100, GameContext.TileSafeTop + 160));
		pPosition.Add(new Vector2(930 + num * 100, GameContext.TileSafeTop + 170));
		JamPos.Add(new Vector2(-50f, 42f));
		JamPos.Add(new Vector2(-45f, 28f));
		JamPos.Add(new Vector2(-30f, 26f));
		JamPos.Add(new Vector2(-26f, 3f));
		bLockMenu = true;
		gamestate.m_Ranking.Clear();
		gamestate.InitRanking();
		switch (GameContext.GameMode)
		{
		case GAME_MODE.DEATHMATCH:
		case GAME_MODE.STORYMATCH:
		{
			int score = gamestate.m_Ranking[0].m_Score;
			foreach (Player item in gamestate.m_Ranking)
			{
				if (item != gamestate.m_Ranking[0] && item.m_Score == score)
				{
					m_bGameIsDraw = true;
				}
			}
			break;
		}
		case GAME_MODE.CAPTURE_THE_JAM:
		{
			CaptureTheFlag captureTheFlag = (CaptureTheFlag)gamestate;
			if (captureTheFlag.m_BlueTeamScore > captureTheFlag.m_RedTeamScore)
			{
				m_WinnerColor = PlayerConfig.BLUE_TEAM_COLOR;
				m_Winner = TextManager.GetText(TextID.BLUETEAM);
				m_Loser = TextManager.GetText(TextID.REDTEAM);
			}
			else if (captureTheFlag.m_BlueTeamScore < captureTheFlag.m_RedTeamScore)
			{
				m_WinnerColor = PlayerConfig.RED_TEAM_COLOR;
				m_Winner = TextManager.GetText(TextID.REDTEAM);
				m_Loser = TextManager.GetText(TextID.BLUETEAM);
			}
			else
			{
				m_bGameIsDraw = true;
				m_Winner = "";
				m_Loser = "";
			}
			break;
		}
		case GAME_MODE.JAM_BALL:
		{
			JamBall jamBall = (JamBall)gamestate;
			if (jamBall.m_BlueTeamScore > jamBall.m_RedTeamScore)
			{
				m_WinnerColor = PlayerConfig.BLUE_TEAM_COLOR;
				m_Winner = TextManager.GetText(TextID.BLUETEAM);
				m_Loser = TextManager.GetText(TextID.REDTEAM);
			}
			else if (jamBall.m_BlueTeamScore < jamBall.m_RedTeamScore)
			{
				m_WinnerColor = PlayerConfig.RED_TEAM_COLOR;
				m_Winner = TextManager.GetText(TextID.REDTEAM);
				m_Loser = TextManager.GetText(TextID.BLUETEAM);
			}
			else
			{
				m_bGameIsDraw = true;
				m_Winner = "";
				m_Loser = "";
			}
			break;
		}
		}
		m_ResultLayer = gamestate.LoadSprite("Cartouche", GameState.GameAtlas.GAME);
		MediaPlayer.Stop();
	}

	public void ChooseScreen(bool ProcessLoading)
	{
		MediaPlayer.Stop();
		switch (GameContext.GameMode)
		{
		case GAME_MODE.DEATHMATCH:
			VersusLoadingScreen.Load(base.ScreenManager, ProcessLoading, null, new DeathMatchScreen());
			break;
		case GAME_MODE.CAPTURE_THE_JAM:
			LoadingScreen.Load(base.ScreenManager, ProcessLoading, null, new CaptureTheFlag());
			break;
		case GAME_MODE.JAM_BALL:
			LoadingScreen.Load(base.ScreenManager, ProcessLoading, null, new JamBall());
			break;
		}
	}

	protected void OnRematch(object sender, PlayerIndexEventArgs e)
	{
		m_bIsExiting = true;
		ChooseScreen(ProcessLoading: true);
	}

	protected void OnRematchChangeMap(object sender, PlayerIndexEventArgs e)
	{
		m_bIsExiting = true;
		Random random = new Random();
		int num = 0;
		if (GameContext.GameMode == GAME_MODE.JAM_BALL)
		{
			if (Guide.IsTrialMode)
			{
				num = 1;
			}
			else
			{
				do
				{
					num = random.Next(0, GameContext.BALL_LEVEL.Length);
				}
				while (GameContext.BALL_LEVEL[num].ToString() == GameContext.SelectedLevel);
			}
			GameContext.SelectedLevel = GameContext.BALL_LEVEL[num].ToString();
		}
		else
		{
			if (Guide.IsTrialMode)
			{
				num = 9;
			}
			else
			{
				do
				{
					num = random.Next(0, GameContext.SELECTABLE_LEVEL.Length);
				}
				while (GameContext.SELECTABLE_LEVEL[num].ToString() == GameContext.SelectedLevel);
			}
			GameContext.SelectedLevel = GameContext.SELECTABLE_LEVEL[num].ToString();
		}
		ChooseScreen(ProcessLoading: true);
	}

	protected void OnExit(object sender, PlayerIndexEventArgs e)
	{
		m_bIsExiting = true;
		MediaPlayer.Stop();
		LoadingScreen.Load(base.ScreenManager, true, null, new MultiPlayerMenuScreen(e.PlayerIndex));
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		AnimatedSprite[] btSpriteSoft = gamestate.m_btSpriteSoft;
		foreach (AnimatedSprite animatedSprite in btSpriteSoft)
		{
			animatedSprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		}
		currentTimer -= gameTime.ElapsedGameTime.Milliseconds;
		if (currentTimer < 0f && m_nElementToDisplay < gamestate.m_Players.Count)
		{
			m_nElementToDisplay++;
			currentTimer = 500f;
		}
		if (m_nTotalElementToDisplay == m_nElementToDisplay && m_ResultTimer > 0f)
		{
			m_ResultTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	public override void Draw(GameTime gameTime)
	{
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2 / 3);
		if (m_bIsExiting)
		{
			return;
		}
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		if (m_ResultTimer <= 0f)
		{
			gamestate.m_btSpriteSoft[0].Draw(ref Apos, SpriteEffects.None, Color.White, 1f);
			base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomMiddle, TextManager.GetText(TextID.VALID), Atextpos, Color.White);
			bLockMenu = false;
		}
		Vector2 zero = Vector2.Zero;
		for (int i = 0; i < m_nElementToDisplay; i++)
		{
			gamestate.m_Ranking[i].m_PlayerSprite[0].GetFrameHeight();
			Color color = gamestate.m_Ranking[i].m_PlayerColor;
			if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM || GameContext.GameMode == GAME_MODE.JAM_BALL)
			{
				color = gamestate.m_Ranking[i].m_Team;
			}
			Vector2 position;
			Vector2 vector;
			if (m_bGameIsDraw)
			{
				position = new Vector2(pPosition[i].X, pPosition[0].Y);
				vector = new Vector2(pPosition[i].X, pPosition[0].Y);
				zero = vector;
				Vector2 position2 = pPosition[i];
				position2.Y = pPosition[0].Y + 120f;
				position2.X -= 100f;
				m_ResultLayer.Draw(position2, Color.White);
				gamestate.m_ResultJamSprite[1].Draw(vector + JamPos[1], color, SpriteEffects.None, 1f);
				gamestate.m_ResultSprite[1].Draw(vector, Color.White, SpriteEffects.None, 1f);
				vector.Y += JamPos[0].Y;
			}
			else
			{
				position = new Vector2(pPosition[i].X, pPosition[0].Y);
				vector = new Vector2(pPosition[i].X, pPosition[i].Y);
				zero = vector;
				Vector2 position3 = pPosition[i];
				position3.Y = pPosition[0].Y + 120f;
				position3.X -= 100f;
				m_ResultLayer.Draw(position3, Color.White);
				gamestate.m_ResultJamSprite[i].Draw(vector + JamPos[i], color);
				gamestate.m_ResultSprite[i].Draw(vector, Color.White);
				vector.Y += JamPos[0].Y;
			}
			int num = 2;
			gamestate.m_Ranking[i].m_PlayerSprite[num].m_CurrentFrame = 1;
			if (i >= m_nTotalElementToDisplay - 1 || m_bGameIsDraw)
			{
				num = 5;
				gamestate.m_Ranking[i].m_PlayerSprite[num].m_CurrentFrame = 0;
				if (!m_bGameIsDraw && i == gamestate.m_Players.Count - 1)
				{
					zero.Y -= gamestate.m_Ranking[i].m_PlayerSprite[num].GetFrameHeight() * i / 10;
				}
			}
			if (!m_bGameIsDraw)
			{
				if (i == 0)
				{
					zero.Y += gamestate.m_ResultJamSprite[1].Height - gamestate.m_Ranking[i].m_PlayerSprite[num].GetFrameHeight();
				}
				else
				{
					zero.Y += gamestate.m_ResultJamSprite[i].Height - gamestate.m_Ranking[i].m_PlayerSprite[num].GetFrameHeight();
				}
			}
			zero.X += 70f;
			Vector2 position4 = vector;
			position4.Y -= 100f;
			gamestate.m_Ranking[i].m_PlayerSprite[num].Draw(ref zero, SpriteEffects.None, Color.White, 1f);
			if (GameContext.GameMode == GAME_MODE.DEATHMATCH || GameContext.GameMode == GAME_MODE.STORYMATCH)
			{
				if (!m_bGameIsDraw)
				{
					string text = "";
					switch (i)
					{
					case 0:
						text += "er";
						break;
					case 1:
						text += "nd";
						break;
					case 2:
						text += "rd";
						break;
					case 3:
						text += "th";
						break;
					}
					base.ScreenManager.DrawTextOutline(base.ScreenManager.BubbleFontVeryBig, i + 1 + text, Color.White, gamestate.m_Ranking[i].m_PlayerColor, 1f, 1.1f, 0f, new Vector2(position4.X + 25f, position4.Y));
					position4.Y += 20f;
					base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, gamestate.m_Ranking[i].Name, position4, gamestate.m_Ranking[i].m_PlayerColor);
					position.Y += 200f;
				}
				else
				{
					base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomVeryBig, TextManager.GetText(TextID.DRAW), Color.Black, Color.White, 1f, 1f, 0f, new Vector2(600f, 100f));
					position.Y += 200f;
				}
				base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, TextManager.GetText(TextID.FRAG_NUMBER) + gamestate.m_Ranking[i].m_Score, position, gamestate.m_Ranking[i].m_PlayerColor);
				position.Y += 30f;
				base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, TextManager.GetText(TextID.DEATH_NUMBER) + gamestate.m_Ranking[i].m_nDeathCount, position, gamestate.m_Ranking[i].m_PlayerColor);
				position.Y += 30f;
				base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, TextManager.GetText(TextID.USED_POWER_UP) + gamestate.m_Ranking[i].m_UsedPowerUp, position, gamestate.m_Ranking[i].m_PlayerColor);
			}
			else
			{
				if (m_bGameIsDraw)
				{
					base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomVeryBig, TextManager.GetText(TextID.DRAW), Color.Black, Color.White, 1f, 1f, 0f, new Vector2(600f, 100f));
				}
				else
				{
					base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomVeryBig, m_Winner.ToUpper() + " " + TextManager.GetText(TextID.WIN), Color.Black, m_WinnerColor, 1f, 1f, 0f, new Vector2(640f, 100f));
				}
				base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, gamestate.m_Ranking[i].Name, position4, gamestate.m_Ranking[i].m_Team);
				position.X -= 15f;
				position.Y += gamestate.m_ResultJamSprite[3].Height + 80;
				base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, TextManager.GetText(TextID.CAPTUREFLAG_NUMBER) + gamestate.m_Ranking[i].m_Score, position, gamestate.m_Ranking[i].m_Team);
				position.Y += 30f;
				base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, TextManager.GetText(TextID.FRAG_NUMBER) + gamestate.m_Ranking[i].m_Frag, position, gamestate.m_Ranking[i].m_Team);
				position.Y += 30f;
				base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, TextManager.GetText(TextID.DEATH_NUMBER) + gamestate.m_Ranking[i].m_nDeathCount, position, gamestate.m_Ranking[i].m_Team);
				if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM)
				{
					position.Y += 30f;
					base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, TextManager.GetText(TextID.USED_POWER_UP) + gamestate.m_Ranking[i].m_UsedPowerUp, position, gamestate.m_Ranking[i].m_Team);
				}
			}
		}
		base.ScreenManager.SpriteBatch.End();
		base.Draw(gameTime);
	}
}
