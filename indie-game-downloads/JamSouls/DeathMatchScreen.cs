using System;
using FarseerPhysics.Dynamics;
using JamSouls.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ProjectMercury;

namespace JamSouls;

internal class DeathMatchScreen : GameState
{
	public const float TEXT_TIMER_LATENCTY = 30f;

	private const float MAX_TIME_TEXT_SCALE = 1.2f;

	public bool m_bMatchBegin;

	public float m_ReadyTimer = 2000f;

	public float m_TextTimer;

	private float m_ElapsedGameTime;

	private float m_TimeTextScale = 1f;

	private bool m_bScaleTimeText = true;

	private float m_GameOverTimer = 100f;

	private Vector2 ReadyPos = new Vector2(640f, 200f);

	public DeathMatchScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		m_PhysicManager = new World(new Vector2(0f, 150f));
		m_bAllowSoulSpawn = true;
	}

	public override void LoadContent()
	{
		base.LoadContent();
		content.Load<ParticleEffect>("Fx/Particle/JamsoulSpawn");
		m_SoulSpawner = new SoulSpawner(this);
		InitPowerUp();
		m_Level = new Level(this, GameContext.SelectedLevel, bGameLevel: true);
		InitHud(initPlayerButton: true);
		int num = 0;
		Player player = null;
		for (int i = 0; i < 4; i++)
		{
			switch (GameContext.Pinfo[i].Controller)
			{
			case PlayerController.PLAYER:
				player = new PlayerHuman(this, GameContext.Pinfo[i].CharacterIdx, GameContext.Pinfo[i].pIndex, GameContext.Pinfo[i].Name, GameContext.Pinfo[i].SbireDef);
				break;
			case PlayerController.PLAYER_BOT:
				player = new PlayerBot(this, GameContext.Pinfo[i].CharacterIdx, GameContext.Pinfo[i].pIndex, GameContext.Pinfo[i].Name, GameContext.Pinfo[i].SbireDef);
				break;
			}
			if (GameContext.Pinfo[i].Controller != PlayerController.NONE)
			{
				m_Entities.Add(player);
				m_Players.Add(player);
				switch (GameContext.Pinfo[i].SlotIdx)
				{
				case 0:
					player.SetTeam(PlayerConfig.RED_TEAM_COLOR);
					break;
				case 1:
					player.SetTeam(PlayerConfig.BLUE_TEAM_COLOR);
					break;
				case 2:
					player.SetTeam(Color.Green);
					break;
				case 3:
					player.SetTeam(Color.Yellow);
					break;
				}
				player.InitFx();
				num++;
			}
		}
		m_SplashHandler = new Splash(this, base.ScreenManager.SpriteBatch);
		m_BackgroundMusic = content.Load<Song>(GameContext.BACKGROUND_MUSIC[GameContext.CurrentMusic]);
		m_BattleMode = new BattleMode(this);
		Random random = new Random();
		m_GameOverTimer = random.Next(25, 100);
		m_TimePos.Y = GameContext.TileSafeTop + 30;
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public override void UnloadContent()
	{
		content.Unload();
		base.UnloadContent();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, m_bIsPaused | m_bGameOver | m_BattleMode.m_BattleMode, coveredByOtherScreen);
		if (!base.IsActive || m_bIsPaused || m_bGameOver || m_BattleMode.m_BattleMode)
		{
			return;
		}
		m_TextTimer += gameTime.ElapsedGameTime.Milliseconds;
		if (m_bMatchBegin)
		{
			float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
			UpdateEntities(gameTime);
			m_PhysicManager.Step(dt);
			m_ReadyTimer -= gameTime.ElapsedGameTime.Milliseconds;
			m_ElapsedGameTime += gameTime.ElapsedGameTime.Milliseconds;
		}
		if (m_bAllowSoulSpawn && SaveHandler.GetSaveData().bUseSouls == 0)
		{
			m_SoulSpawner.Update(gameTime);
		}
		m_SplashHandler.Update(gameTime);
		UpdateHud(gameTime);
		foreach (Player player in m_Players)
		{
			if (player.m_Score >= GameContext.PointLimit || (GameContext.TimeLimit != 0f && m_ElapsedGameTime >= GameContext.TimeLimit))
			{
				m_GameOverTimer -= gameTime.ElapsedGameTime.Milliseconds;
				if (m_GameOverTimer < 0f && !m_bGameOver)
				{
					InitRanking();
					bool flag = false;
					if (GameContext.m_bSuddentDeath)
					{
						int score = m_Ranking[0].m_Score;
						foreach (Player item in m_Ranking)
						{
							if (item != m_Ranking[0] && item.m_Score >= score)
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						StopGame();
					}
					else
					{
						m_BattleMode.StartBattle();
					}
				}
			}
			HandleCommonInput(player);
		}
	}

	public override void StopGame()
	{
		m_bGameOver = true;
		m_GameEndSfx.Play();
		base.ScreenManager.AddScreen(new ResultMenuScreen(this), null);
	}

	public override void HandleInput()
	{
		base.HandleInput();
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		if (!m_bMatchBegin && base.ScreenManager.ViewPort != null)
		{
			for (int i = 0; i < m_Players.Count; i++)
			{
				m_Players[i].SpawnPlayer();
			}
			m_bMatchBegin = true;
			MediaPlayer.Play(m_BackgroundMusic);
		}
		else
		{
			m_LightMgr.BuildLightMap();
			base.ScreenManager.GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
			m_SoulSpawner.Draw();
			DrawEntities();
			spriteBatch.End();
			DrawFx();
			PostDraw();
			m_LightMgr.DrawLightMap();
			DrawHud();
		}
		if (base.TransitionPosition > 0f)
		{
			base.ScreenManager.FadeBackBufferToBlack(255 - base.TransitionAlpha);
		}
	}

	public override void PostDraw()
	{
		base.PostDraw();
		m_SplashHandler.Draw();
	}

	public void DrawHud()
	{
		base.ScreenManager.SpriteBatch.Begin();
		if (m_ReadyTimer > 0f)
		{
			Vector2 readyPos = ReadyPos;
			readyPos.X -= m_GetReadyTex.Width / 2 + m_ReadyTextSize / 2 - 5;
			readyPos.Y -= m_GetReadyTex.Height / 2;
			m_GetReadyTex.Draw(readyPos, Color.White, SpriteEffects.None, 1f);
			base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.GET_READY), Color.Black, Color.White, 2f, 1f, 0f, ReadyPos);
		}
		else if (GameContext.TimeLimit != 0f && !m_bGameOver)
		{
			float num = GameContext.TimeLimit - m_ElapsedGameTime;
			_ = num / 1000f % 60f;
			_ = num / 60000f;
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(num);
			string text = timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2");
			Color frontColor = Color.White;
			if (num > 0f)
			{
				if (num < 10000f)
				{
					frontColor = Color.Red;
					frontColor.G = (byte)(m_TimeTextScale * 255f / 1.2f);
					frontColor.B = (byte)(m_TimeTextScale * 255f / 1.2f);
					if (m_TextTimer > 30f)
					{
						if (m_bScaleTimeText)
						{
							m_TimeTextScale += 0.05f;
						}
						else
						{
							m_TimeTextScale -= 0.05f;
						}
						if (m_TimeTextScale >= 1.2f)
						{
							m_bScaleTimeText = false;
						}
						if (m_TimeTextScale <= 0.8f)
						{
							m_bScaleTimeText = true;
						}
						m_TextTimer = 0f;
					}
				}
				base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomBig, text, Color.Black, frontColor, 1f, m_TimeTextScale, 0f, m_TimePos);
			}
		}
		foreach (Player player in m_Players)
		{
			player.DrawHud();
		}
		base.ScreenManager.SpriteBatch.End();
	}
}
