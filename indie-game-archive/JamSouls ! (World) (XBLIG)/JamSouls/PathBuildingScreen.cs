using System;
using FarseerPhysics.Dynamics;
using JamSouls.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjectMercury;

namespace JamSouls;

public class PathBuildingScreen : GameState
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

	private AudioClip m_PauseSound;

	private Vector2 ReadyPos = new Vector2(610f, 200f);

	private Vector2 TimePos = new Vector2(620f, 0f);

	public PathBuilder m_PathBuilder;

	public PathBuildingScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		m_bSpawnBonus = false;
		m_bDrawPath = true;
		m_PhysicManager = new World(new Vector2(0f, 150f));
		m_bAllowSoulSpawn = false;
	}

	public override void LoadContent()
	{
		base.LoadContent();
		content.Load<ParticleEffect>("Fx/Particle/JamsoulSpawn");
		m_SoulSpawner = new SoulSpawner(this);
		InitPowerUp();
		m_Level = new Level(this, GameContext.SelectedLevel, bGameLevel: true);
		InitHud(initPlayerButton: true);
		Player player = null;
		player = new PlayerHuman(this, GameContext.Pinfo[0].CharacterIdx, GameContext.Pinfo[0].pIndex, GameContext.Pinfo[0].Name, GameContext.Pinfo[0].SbireDef);
		m_Entities.Add(player);
		m_Players.Add(player);
		player.SetTeam(PlayerConfig.BLUE_TEAM_COLOR);
		player.InitFx();
		m_SplashHandler = new Splash(this, base.ScreenManager.SpriteBatch);
		m_PauseSound = new AudioClip("Menu_Pause");
		m_BackgroundMusic = content.Load<Song>(GameContext.BACKGROUND_MUSIC[GameContext.CurrentMusic]);
		Random random = new Random();
		m_GameOverTimer = random.Next(25, 100);
		TimePos.Y = GameContext.TileSafeTop;
		m_PathBuilder = new PathBuilder(this);
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, m_bIsPaused | m_bGameOver, coveredByOtherScreen);
		if (!base.IsActive || m_bIsPaused || m_bGameOver)
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
			m_PathBuilder.Update(gameTime);
			m_ElapsedGameTime += gameTime.ElapsedGameTime.Milliseconds;
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
					m_bGameOver = true;
					m_GameEndSfx.Play();
					base.ScreenManager.AddScreen(new ResultMenuScreen(this), PlayerIndex.One);
				}
			}
			if (InputManager.GetKeyState(player.m_PlayerNum, 8) == ButtonState.Pressed && !m_bGameOver)
			{
				MediaPlayer.Pause();
				m_PauseSound.Play();
				base.ScreenManager.AddScreen(new PauseMenuScreen(this), player.m_PlayerNum);
				m_bIsPaused = true;
			}
		}
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
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			DrawEntities();
			m_PathBuilder.Draw();
			spriteBatch.End();
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
			base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.GET_READY), Color.Black, Color.White, 2f, 1f, 0f, ReadyPos);
		}
		else if (GameContext.TimeLimit != 0f && !m_bGameOver)
		{
			float num = GameContext.TimeLimit - m_ElapsedGameTime;
			float num2 = num / 1000f % 60f;
			double num3 = (double)(num / 60000f) - 0.5;
			string text = $"{num3:00}" + " : " + $"{num2:F0}";
			Color frontColor = Color.Black;
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
				base.ScreenManager.DrawTextOutline(base.ScreenManager.BubbleFontBig, text, Color.White, frontColor, 2f, m_TimeTextScale, 0f, TimePos);
			}
		}
		foreach (Player player in m_Players)
		{
			player.DrawHud();
		}
		base.ScreenManager.SpriteBatch.End();
	}
}
