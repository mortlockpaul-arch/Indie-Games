using System;
using FarseerPhysics.Dynamics;
using JamSouls.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjectMercury;

namespace JamSouls;

internal class JamBall : GameState
{
	public const float TEXT_TIMER_LATENCTY = 30f;

	private const float MAX_TIME_TEXT_SCALE = 1.2f;

	private const float SPAWN_BALL_TIMER = 3000f;

	public const int TEAM_LEFT = 280;

	public const int TEAM_RIGHT = 850;

	public const int TEAM_HEIGHT = 110;

	public const int TEAM_OFFSET = 45;

	public const float FADE_TIMER = 10f;

	public const int FADE_STEP = 10;

	public const int ARROW_OFFSET = 220;

	private const int GOAL_Y_OFFSET = 97;

	public bool m_bMatchBegin;

	public float m_ReadyTimer = 2000f;

	public bool m_FadeStart = true;

	public float m_TextTimer;

	private float m_ElapsedGameTime;

	private float m_TimeTextScale = 1f;

	private bool m_bScaleTimeText = true;

	public int m_RedTeamScore;

	public int m_BlueTeamScore;

	private float m_GameOverTimer = 100f;

	private bool m_bTeamChoosen;

	private float m_FadeTimer;

	private int m_FadeAlpha;

	private bool m_bIsFading;

	private bool[] m_LockedPlayer = new bool[4];

	private float m_StartLatencyTimer = 300f;

	private Ball m_Ball;

	private float m_TimeSpawnBall;

	private Vector2 m_SpawnBallPos;

	private Vector2 m_GoalPosition;

	public Rectangle m_BlueGoal;

	public Rectangle m_RedGoal;

	private Sprite m_BluePotTex;

	private Sprite m_RedPotTex;

	private Sprite m_RedArrow;

	private Sprite m_BlueArrow;

	private MercuryParticle m_RedTeamHalo;

	private MercuryParticle m_BlueTeamHalo;

	private MercuryParticle m_BallSpawn;

	private MercuryParticle m_Fireworks;

	private AudioClip m_Whistle;

	private AudioClip m_GoalSound;

	private Vector2 ReadyPos = new Vector2(640f, 200f);

	private Vector2 BlueScorePos = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeTop);

	private Vector2 RedScorePos = new Vector2(GameContext.TileSafeRight, GameContext.TileSafeTop);

	public JamBall()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		m_PhysicManager = new World(new Vector2(0f, 150f));
	}

	public override void LoadContent()
	{
		base.LoadContent();
		m_RedTeamHalo = new MercuryParticle(this, 280, 720, content.Load<ParticleEffect>("Fx/Particle/JamsoulSpawn").DeepCopy(), "halored", 1f, bUseBlending: true);
		m_RedTeamHalo.SetParticleColor(PlayerConfig.RED_TEAM_COLOR, Vector3.Zero);
		m_RedTeamHalo.SetAutoTrigger(bAutoTrigger: false);
		AddParticle(m_RedTeamHalo);
		m_BlueTeamHalo = new MercuryParticle(this, 850, 720, content.Load<ParticleEffect>("Fx/Particle/JamsoulSpawn").DeepCopy(), "haloblue", 1f, bUseBlending: true);
		m_BlueTeamHalo.SetParticleColor(PlayerConfig.BLUE_TEAM_COLOR, Vector3.Zero);
		m_BlueTeamHalo.SetAutoTrigger(bAutoTrigger: false);
		AddParticle(m_BlueTeamHalo);
		m_BallSpawn = new MercuryParticle(this, 850, 720, content.Load<ParticleEffect>("Fx/Ball/BallSpawn").DeepCopy(), "BallSpawn", 1f, bUseBlending: true);
		m_BallSpawn.SetAutoTrigger(bAutoTrigger: false);
		AddParticle(m_BallSpawn);
		m_Fireworks = new MercuryParticle(this, 0, 0, content.Load<ParticleEffect>("Fx/Particle/Fireworks"), "GoalFx", 0f, bUseBlending: true);
		AddParticle(m_Fireworks);
		m_Fireworks.SetAutoTrigger(bAutoTrigger: false);
		m_Level = new Level(this, GameContext.SelectedLevel, bGameLevel: true);
		m_SplashHandler = new Splash(this, base.ScreenManager.SpriteBatch);
		InitHud(initPlayerButton: true);
		m_BluePotTex = LoadSprite("BluePotBig", GameAtlas.GAME);
		m_RedPotTex = LoadSprite("RedPotBig", GameAtlas.GAME);
		m_RedArrow = LoadSprite("RedArrow", GameAtlas.GAME);
		m_BlueArrow = LoadSprite("BlueArrow", GameAtlas.GAME);
		m_Whistle = new AudioClip("Foot_Sifflet");
		m_GoalSound = new AudioClip("Foot_Goal");
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
				player = new PlayerBot(this, GameContext.Pinfo[i].CharacterIdx, GameContext.Pinfo[i].pIndex, GameContext.Pinfo[i].Name, PlayerConfig.SBIRE_DEF.NONE);
				break;
			}
			if (GameContext.Pinfo[i].Controller != PlayerController.NONE)
			{
				m_Entities.Add(player);
				m_Players.Add(player);
				if (GameContext.Pinfo[i].SlotIdx < 2)
				{
					player.SetTeam(PlayerConfig.BLUE_TEAM_COLOR);
				}
				else
				{
					player.SetTeam(PlayerConfig.RED_TEAM_COLOR);
				}
				player.InitFx();
				if (InputManager.GetKeyState(player.m_PlayerNum, 4) == ButtonState.Pressed)
				{
					m_LockedPlayer[(int)player.m_PlayerNum] = true;
				}
				num++;
			}
		}
		m_BackgroundMusic = content.Load<Song>(GameContext.BACKGROUND_MUSIC[GameContext.CurrentMusic]);
		Random random = new Random();
		m_GameOverTimer = random.Next(25, 100);
		m_TimePos.Y = GameContext.TileSafeTop + 30;
		m_SpawnBallPos = m_Level.GetDummyByName("BallSpawn").Position;
		m_TimeSpawnBall = 1500f;
		m_Ball = new Ball(new Vector2(640f, 360f), this, Color.Wheat);
		m_bAllowSoulSpawn = false;
		m_bSpawnBonus = false;
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public override void UnloadContent()
	{
		content.Unload();
		base.UnloadContent();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, m_bIsPaused | m_bGameOver, coveredByOtherScreen);
		m_FadeTimer -= gameTime.ElapsedGameTime.Milliseconds;
		if (m_bIsFading && m_FadeTimer <= 0f)
		{
			if (m_FadeStart)
			{
				m_FadeAlpha += 10;
				if (m_FadeAlpha >= 255)
				{
					m_FadeStart = false;
					m_FadeAlpha = 255;
				}
			}
			else
			{
				m_FadeAlpha -= 10;
			}
			m_FadeTimer = 10f;
		}
		if (!m_bTeamChoosen)
		{
			m_btSpriteSoft[0].UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			m_BlueTeamHalo.Update(gameTime);
			m_RedTeamHalo.Update(gameTime);
		}
		else if (base.IsActive && !m_bIsPaused && !m_bGameOver)
		{
			m_TextTimer += gameTime.ElapsedGameTime.Milliseconds;
			if (m_bMatchBegin)
			{
				float dt = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
				UpdateEntities(gameTime);
				m_PhysicManager.Step(dt);
				m_ReadyTimer -= gameTime.ElapsedGameTime.Milliseconds;
				m_ElapsedGameTime += gameTime.ElapsedGameTime.Milliseconds;
			}
			m_SplashHandler.Update(gameTime);
			UpdateHud(gameTime);
			foreach (Player player in m_Players)
			{
				HandleCommonInput(player);
			}
			if (m_TimeSpawnBall > 0f)
			{
				m_TimeSpawnBall -= gameTime.ElapsedGameTime.Milliseconds;
				m_BallSpawn.Trigger(m_SpawnBallPos);
				m_Ball.SetPosition(m_SpawnBallPos);
				if (m_TimeSpawnBall < 1500f)
				{
					if (!m_Ball.IsEnable())
					{
						m_Whistle.Play();
					}
					m_Ball.SetEnable(enable: true);
				}
				if (m_TimeSpawnBall > 2800f)
				{
					m_Fireworks.Trigger(m_GoalPosition);
				}
			}
			else
			{
				m_Ball.Update(gameTime);
				Vector2 position = m_Ball.GetPosition();
				if (m_RedGoal.Contains((int)position.X, (int)position.Y))
				{
					m_GoalPosition = position;
					m_TimeSpawnBall = 3000f;
					m_Ball.SetEnable(enable: false);
					m_Ball.ScoreGoal();
					m_GoalSound.Play();
					m_BlueTeamScore++;
				}
				if (m_BlueGoal.Contains((int)position.X, (int)position.Y))
				{
					m_GoalPosition = position;
					m_TimeSpawnBall = 3000f;
					m_Ball.SetEnable(enable: false);
					m_Ball.ScoreGoal();
					m_GoalSound.Play();
					m_RedTeamScore++;
				}
			}
		}
		if (m_RedTeamScore >= GameContext.PointLimit || m_BlueTeamScore >= GameContext.PointLimit || (GameContext.TimeLimit != 0f && m_ElapsedGameTime >= GameContext.TimeLimit))
		{
			m_GameOverTimer -= gameTime.ElapsedGameTime.Milliseconds;
			if (!m_bGameOver && m_GameOverTimer < 0f)
			{
				InitRanking();
				m_bGameOver = true;
				m_GameEndSfx.Play();
				base.ScreenManager.AddScreen(new ResultMenuScreen(this), null);
			}
		}
	}

	public override void HandleInput()
	{
		base.HandleInput();
	}

	public void ManageChooseTeam()
	{
		base.ScreenManager.GraphicsDevice.Clear(Color.Black);
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		int num = 0;
		int num2 = 85;
		int tileSafeTop = GameContext.TileSafeTop;
		m_BluePotTex.Draw(new Vector2(0f, 0f), Color.White);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.BLUETEAM), new Vector2(280f, tileSafeTop + 40), PlayerConfig.BLUE_TEAM_COLOR);
		m_RedPotTex.Draw(new Vector2(1280 - m_RedPotTex.Width, 0f), Color.White);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.REDTEAM), new Vector2(850f, tileSafeTop + 40), PlayerConfig.RED_TEAM_COLOR);
		bool flag = false;
		foreach (Player player in m_Players)
		{
			if (InputManager.GetKeyState(player.m_PlayerNum, 3) == ButtonState.Pressed && !m_bIsFading && player.GetTeam() == PlayerConfig.BLUE_TEAM_COLOR)
			{
				player.m_Team = PlayerConfig.RED_TEAM_COLOR;
			}
			if (InputManager.GetKeyState(player.m_PlayerNum, 1) == ButtonState.Pressed && !m_bIsFading && player.GetTeam() == PlayerConfig.RED_TEAM_COLOR)
			{
				player.m_Team = PlayerConfig.BLUE_TEAM_COLOR;
			}
			if (player.m_bIsPlayerBot && CanBalanceTeam(player.GetTeam()))
			{
				if (player.GetTeam() == PlayerConfig.BLUE_TEAM_COLOR)
				{
					player.m_Team = PlayerConfig.RED_TEAM_COLOR;
				}
				else
				{
					player.m_Team = PlayerConfig.BLUE_TEAM_COLOR;
				}
			}
			flag = IsTeamEmpty();
			if (InputManager.GetKeyState(player.m_PlayerNum, 4) == ButtonState.Pressed && !m_bIsFading && !m_bMatchBegin)
			{
				if (!flag && !m_LockedPlayer[(int)player.m_PlayerNum])
				{
					m_FadeTimer = 10f;
					m_FadeAlpha = 1;
					m_bIsFading = true;
					m_ParticleManager.Remove(m_BlueTeamHalo);
					m_ParticleManager.Remove(m_RedTeamHalo);
					MediaPlayer.Play(m_BackgroundMusic);
				}
			}
			else
			{
				m_LockedPlayer[(int)player.m_PlayerNum] = false;
			}
			num2 += 110;
			if (player.GetTeam() == PlayerConfig.BLUE_TEAM_COLOR)
			{
				num = 280;
				m_RedArrow.Draw(new Vector2(num + 440 - 90, num2), Color.White);
			}
			else
			{
				num = 850;
				m_BlueArrow.Draw(new Vector2(num - 220, num2), Color.White);
			}
			player.m_HudTexture.Draw(new Vector2(num, num2), Color.White);
			base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, player.Name, new Vector2(num + player.m_HudTexture.Width, num2), Color.White);
		}
		if (!flag)
		{
			Vector2 Position = new Vector2(530f, 500 + tileSafeTop);
			m_btSpriteSoft[0].Draw(ref Position, SpriteEffects.None, Color.White, 1f);
			Position.X += m_btSpriteSoft[0].GetFrameWidth();
			Position.Y += 20f;
			base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.START_GAME), Position, Color.White);
		}
		base.ScreenManager.SpriteBatch.End();
		if (!m_bIsFading)
		{
			m_BlueTeamHalo.Trigger(new Vector2(360f, 700f));
			m_RedTeamHalo.Trigger(new Vector2(930f, 700f));
		}
		PostDraw();
	}

	public bool IsTeamEmpty()
	{
		int num = 0;
		int num2 = 0;
		foreach (Player player in m_Players)
		{
			if (player.GetTeam() == PlayerConfig.BLUE_TEAM_COLOR)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		if (num != 0)
		{
			return num2 == 0;
		}
		return true;
	}

	public bool CanBalanceTeam(Color TeamColor)
	{
		int num = 0;
		int num2 = 0;
		foreach (Player player in m_Players)
		{
			if (player.GetTeam() == PlayerConfig.BLUE_TEAM_COLOR)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		if (num == num2)
		{
			return false;
		}
		if (TeamColor == PlayerConfig.BLUE_TEAM_COLOR)
		{
			if (num == 0 || num - 1 == num2 + 1)
			{
				return true;
			}
		}
		else if (num2 == 0 || num + 1 == num2 - 1)
		{
			return true;
		}
		return false;
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		if (!m_bMatchBegin && base.ScreenManager.ViewPort != null)
		{
			if (m_bTeamChoosen)
			{
				for (int i = 0; i < m_Players.Count; i++)
				{
					m_Players[i].SpawnPlayer();
				}
				m_bMatchBegin = true;
			}
			else if (base.IsActive)
			{
				if (m_StartLatencyTimer <= 0f)
				{
					ManageChooseTeam();
				}
				m_StartLatencyTimer -= gameTime.ElapsedGameTime.Milliseconds;
			}
		}
		else
		{
			m_LightMgr.BuildLightMap();
			base.ScreenManager.GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
			DrawEntities();
			m_Ball.Draw();
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
		if (m_bIsFading)
		{
			if (m_FadeAlpha >= 250)
			{
				m_bTeamChoosen = true;
			}
			if (m_FadeAlpha <= 0)
			{
				m_bIsFading = false;
				m_FadeTimer = 0f;
				m_FadeAlpha = 0;
			}
			base.ScreenManager.FadeBackBufferToBlack(m_FadeAlpha);
		}
	}

	public override void PostDraw()
	{
		base.PostDraw();
		if (m_bTeamChoosen)
		{
			m_SplashHandler.Draw();
		}
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
		else if (!m_bGameOver)
		{
			if (GameContext.TimeLimit != 0f)
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
			if (m_TimeSpawnBall > 0f && m_TimeSpawnBall < 1500f)
			{
				base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.KICKOFF), Color.Black, Color.White, 1f, 1f, 0f, ReadyPos);
			}
			base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.BLUETEAM).ToString() + " : " + m_BlueTeamScore, Color.Black, PlayerConfig.BLUE_TEAM_COLOR, 1f, BlueScorePos, ScreenManager.TextOrigin.top_Left);
			base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.REDTEAM).ToString() + " : " + m_RedTeamScore, Color.Black, PlayerConfig.RED_TEAM_COLOR, 1f, RedScorePos, ScreenManager.TextOrigin.top_right);
		}
		foreach (Player player in m_Players)
		{
			player.DrawHud();
		}
		base.ScreenManager.SpriteBatch.End();
	}
}
