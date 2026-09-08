using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

internal class BattleScreen : GameScreen
{
	public const float SPEED = 10f;

	public const float REACH_RADIUS = 100f;

	public const float PLAYER_DISPLAY_LATENCY = 1200f;

	public const float END_TIME = 1500f;

	private GameState gamestate;

	private float m_PlayerDisplayTimer;

	private int m_SelectedPlayer;

	private bool m_bBattleBegin;

	private int m_BestPlayer = -1;

	private float m_EndTimer;

	private Vector2 POINT_TO_REACH = new Vector2(640f, 360f);

	private Vector2 m_ScreenCenter = new Vector2(472f, 200f);

	public Vector2[] AvatarPosition = new Vector2[4];

	public Vector2[] PLAYER_POSITION = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(817f, 430f),
		new Vector2(817f, 0f),
		new Vector2(0f, 430f)
	};

	public Vector2[] PLAYER_DIRECTION = new Vector2[4]
	{
		new Vector2(1f, 0.5f),
		new Vector2(-1f, -0.5f),
		new Vector2(-1f, 0.5f),
		new Vector2(1f, -0.5f)
	};

	public ButtonState[] PLAYER_BUTTON = new ButtonState[4];

	public int[] PLAYER_STRENGTH = new int[4];

	public float[] BOT_SPEED = new float[5] { 40f, 80f, 120f, 160f, 200f };

	public float[] m_BotStep = new float[5] { 40f, 80f, 120f, 160f, 200f };

	public BattleScreen(GameState thegamestate)
	{
		base.IsPopup = true;
		gamestate = thegamestate;
		m_SelectedPlayer = 0;
		m_PlayerDisplayTimer = 0f;
		m_bBattleBegin = false;
		Vector2 zero = Vector2.Zero;
		zero.X = 0f;
		zero.Y = PLAYER_POSITION[m_SelectedPlayer].Y - (float)(gamestate.m_Players[m_SelectedPlayer].m_BigAvatar.GetFrameHeight() / 4);
		AvatarPosition[m_SelectedPlayer] = zero;
		gamestate.m_Players[m_SelectedPlayer].m_BigAvatar.m_CurrentFrame = 0;
	}

	public override void HandleInput()
	{
		if (!gamestate.m_BattleMode.m_BattleMode || base.IsExiting || !m_bBattleBegin || m_BestPlayer != -1)
		{
			return;
		}
		for (int i = 0; i < m_SelectedPlayer; i++)
		{
			Player player = gamestate.m_Players[i];
			if (!gamestate.m_BattleMode.m_JamRun.IsPlaying())
			{
				gamestate.m_BattleMode.m_JamRun.Play();
			}
			if (player.m_bIsPlayerBot && m_BotStep[i] <= 0f)
			{
				AvatarPosition[i] += PLAYER_DIRECTION[i] * 10f;
				PLAYER_STRENGTH[i]++;
				m_BotStep[i] = BOT_SPEED[gamestate.m_Randomizer.Next(1, BOT_SPEED.Length)];
			}
			if (InputManager.GetKeyState(player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				if (PLAYER_BUTTON[i] == ButtonState.Released)
				{
					PLAYER_BUTTON[i] = ButtonState.Pressed;
					AvatarPosition[i] += PLAYER_DIRECTION[i] * 10f;
					PLAYER_STRENGTH[i]++;
				}
			}
			else
			{
				PLAYER_BUTTON[i] = ButtonState.Released;
			}
		}
		base.HandleInput();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		m_PlayerDisplayTimer -= gameTime.ElapsedGameTime.Milliseconds;
		if (!m_bBattleBegin)
		{
			if (m_PlayerDisplayTimer <= 0f)
			{
				m_SelectedPlayer++;
				gamestate.m_BattleMode.m_JamsoulAppearChar.Play();
				if (m_SelectedPlayer < gamestate.m_Players.Count)
				{
					m_PlayerDisplayTimer = 1200f;
					Vector2 zero = Vector2.Zero;
					gamestate.m_Players[m_SelectedPlayer].m_BigAvatar.m_CurrentFrame = 0;
					if (m_SelectedPlayer == 1 || m_SelectedPlayer == 2)
					{
						zero.X = PLAYER_POSITION[m_SelectedPlayer].X + (float)(gamestate.m_Players[m_SelectedPlayer].m_BigAvatar.GetFrameWidth() / 4);
						zero.Y = PLAYER_POSITION[m_SelectedPlayer].Y - (float)(gamestate.m_Players[m_SelectedPlayer].m_BigAvatar.GetFrameHeight() / 4);
					}
					else
					{
						zero.X = 0f;
						zero.Y = PLAYER_POSITION[m_SelectedPlayer].Y - (float)(gamestate.m_Players[m_SelectedPlayer].m_BigAvatar.GetFrameHeight() / 4);
					}
					AvatarPosition[m_SelectedPlayer] = zero;
				}
				else
				{
					gamestate.m_BattleMode.m_JamsoulAppearCenter.Play();
					m_bBattleBegin = true;
				}
			}
		}
		else
		{
			gamestate.m_BattleMode.m_Flux_Lr.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			gamestate.m_BattleMode.m_Flux_rL.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			gamestate.m_BattleMode.m_BattleBt.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			Vector2 zero2 = Vector2.Zero;
			int num = 0;
			if (m_BestPlayer == -1)
			{
				foreach (Player player in gamestate.m_Players)
				{
					zero2.X = AvatarPosition[num].X + (float)(player.m_BigAvatar.GetFrameWidth() / 2);
					zero2.Y = AvatarPosition[num].Y + (float)(player.m_BigAvatar.GetFrameHeight() / 2);
					player.m_BubbleEffect.Update(gameTime);
					player.m_BigAvatar.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
					if (Vector2.Distance(zero2, POINT_TO_REACH) < 100f)
					{
						gamestate.m_BattleMode.m_JamRun.Stop();
						m_BestPlayer = num;
						m_EndTimer = 1500f;
						gamestate.m_BattleMode.m_JamsoulAppearCenter.Play();
					}
					player.m_BubbleEffect.Trigger(zero2);
					m_BotStep[num] -= gameTime.ElapsedGameTime.Milliseconds;
					num++;
				}
			}
			else
			{
				m_EndTimer -= gameTime.ElapsedGameTime.Milliseconds;
				foreach (Player player2 in gamestate.m_Players)
				{
					zero2.X = AvatarPosition[num].X + (float)(player2.m_BigAvatar.GetFrameWidth() / 2);
					zero2.Y = AvatarPosition[num].Y + (float)(player2.m_BigAvatar.GetFrameHeight() / 2);
					player2.m_BubbleEffect.Update(gameTime);
					player2.m_BigAvatar.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
					player2.m_BubbleEffect.Trigger(zero2);
					num++;
				}
				if (m_EndTimer <= 0f)
				{
					StopBattle();
				}
			}
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	public void StopBattle()
	{
		for (int i = 0; i < gamestate.m_Players.Count; i++)
		{
			gamestate.m_Players[i].GetBody().Active = true;
			if (i != m_BestPlayer)
			{
				gamestate.m_Players[i].m_Tag = 1;
			}
		}
		gamestate.m_Players[m_BestPlayer].m_Score++;
		gamestate.m_BattleMode.Stop();
		gamestate.StopGame();
		ExitScreen();
	}

	public override void Draw(GameTime gameTime)
	{
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2 / 3);
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
		for (int i = 0; i < m_SelectedPlayer; i++)
		{
			Player player = gamestate.m_Players[i];
			SpriteEffects spe = SpriteEffects.None;
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			if (m_bBattleBegin)
			{
				gamestate.m_BattleMode.m_BattleBt.Draw(ref m_ScreenCenter, SpriteEffects.None, Color.White, 0.6f);
			}
			switch (i)
			{
			case 0:
				spe = SpriteEffects.FlipHorizontally;
				zero = new Vector2(70f, 0f);
				zero2 = PLAYER_POSITION[i];
				if (m_bBattleBegin)
				{
					gamestate.m_BattleMode.m_Flux_Lr.Draw(ref zero2, SpriteEffects.FlipVertically, player.m_PlayerColor, 0.1f);
				}
				gamestate.m_BattleMode.m_Splash.Draw(PLAYER_POSITION[i], player.m_PlayerColor, SpriteEffects.None, 0.9f);
				break;
			case 1:
				spe = SpriteEffects.None;
				zero = new Vector2(gamestate.m_BattleMode.m_Splash.Width, gamestate.m_BattleMode.m_Splash.Height);
				zero2 = PLAYER_POSITION[i] - new Vector2(175f, 140f);
				if (m_bBattleBegin)
				{
					gamestate.m_BattleMode.m_Flux_Lr.Draw(ref zero2, 0f, player.m_Origin, SpriteEffects.FlipHorizontally, player.m_PlayerColor, 0.1f);
				}
				gamestate.m_BattleMode.m_Splash.Draw(PLAYER_POSITION[i] + zero, player.m_PlayerColor, SpriteEffects.None, 0.9f, (float)Math.PI, 1f);
				break;
			case 2:
				spe = SpriteEffects.None;
				zero = new Vector2(175f, 0f);
				zero2 = PLAYER_POSITION[i] - zero;
				if (m_bBattleBegin)
				{
					gamestate.m_BattleMode.m_Flux_rL.Draw(ref zero2, SpriteEffects.None, player.m_PlayerColor, 0.1f);
				}
				gamestate.m_BattleMode.m_Splash.Draw(PLAYER_POSITION[i], player.m_PlayerColor, SpriteEffects.FlipHorizontally, 0.9f, 0f, 1f);
				break;
			case 3:
				spe = SpriteEffects.FlipHorizontally;
				zero = new Vector2(0f, gamestate.m_BattleMode.m_Splash.Height);
				zero2 = PLAYER_POSITION[i] + new Vector2(0f, -140f);
				if (m_bBattleBegin)
				{
					gamestate.m_BattleMode.m_Flux_Lr.Draw(ref zero2, SpriteEffects.None, player.m_PlayerColor, 0.1f);
				}
				gamestate.m_BattleMode.m_Splash.Draw(PLAYER_POSITION[i], player.m_PlayerColor, SpriteEffects.FlipVertically, 0.9f, 0f, 1f);
				break;
			}
			if (m_BestPlayer != -1)
			{
				if (i == m_BestPlayer)
				{
					player.m_BigAvatar.Draw(ref AvatarPosition[i], spe, Color.White, 1f);
				}
			}
			else
			{
				player.m_BigAvatar.Draw(ref AvatarPosition[i], spe, Color.White, 1f);
			}
		}
		base.ScreenManager.SpriteBatch.End();
		if (m_EndTimer < 1400f && m_EndTimer > 1300f)
		{
			base.ScreenManager.FadeBackBufferToWhite(255);
		}
		foreach (Player player2 in gamestate.m_Players)
		{
			player2.m_BubbleEffect.DrawEffect();
		}
		base.Draw(gameTime);
	}
}
