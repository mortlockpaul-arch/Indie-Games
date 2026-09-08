using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class Vile : PowerUp
{
	public struct FireBall
	{
		public AnimatedSprite m_FireBall;

		public AnimatedSprite m_ExplodeAnim;

		public float m_SmokeAnimTime;

		public Vector2 m_SmokePos;

		public Vector2 m_FireBallPos;

		public AudioClip m_VileSound;

		public AudioClip m_FireSound;

		public bool m_bPlaySound;
	}

	private const float FIREBALL_SPEED = 0.5f;

	private const float SHOOT_LATENCY = 500f;

	private const float COLLISION_DISTANCE = 70f;

	private const float LAUNCH_OFFSET = 80f;

	private AnimatedSprite m_Sprite;

	private FireBall[] m_Fireballs = new FireBall[4];

	private float m_ShootLatency;

	private float m_SmokeTotalAnimTime;

	private int m_CurrentFireBalls;

	public Vile(GameState StateInstance, SpriteBatch spriteBatch)
	{
		m_StateInstance = StateInstance;
		m_Sprite = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Vile/PowerUp_Vile.xml", GameState.GameAtlas.GAME, "PowerUp_Vile");
		AnimatedSprite animatedSprite = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Vile/PowerUp_VileSmoke.xml", GameState.GameAtlas.GAME, "PowerUp_VileSmoke");
		AnimatedSprite animatedSprite2 = m_StateInstance.LoadAnimatedSpriteFromXml("PowerUp/Vile/PowerUp_VileFireBall.xml", GameState.GameAtlas.GAME, "PowerUp_VileFireBall");
		for (int i = 0; i < m_Fireballs.Length; i++)
		{
			m_Fireballs[i].m_ExplodeAnim = new AnimatedSprite(spriteBatch, animatedSprite.m_Texture, animatedSprite.m_TotalFrames, animatedSprite.m_FrameWidth, animatedSprite.m_FrameHeight, animatedSprite.m_Speed, 1, animatedSprite.GetOffsetX(), animatedSprite.GetOffsetY());
			m_Fireballs[i].m_FireBall = new AnimatedSprite(spriteBatch, animatedSprite2.m_Texture, animatedSprite2.m_TotalFrames, animatedSprite2.m_FrameWidth, animatedSprite2.m_FrameHeight, animatedSprite2.m_Speed, 0, animatedSprite2.GetOffsetX(), animatedSprite2.GetOffsetY());
			m_Fireballs[i].m_VileSound = new AudioClip("PowerUp_Vile");
			m_Fireballs[i].m_FireSound = new AudioClip("FireBall");
		}
		m_SmokeTotalAnimTime = (float)animatedSprite.m_TotalFrames * animatedSprite.m_Speed;
		InitPowerUp(m_Sprite.GetFrameWidth(), m_Sprite.GetFrameHeight(), spriteBatch);
	}

	public override void InitBonus()
	{
		for (int i = 0; i < m_Fireballs.Length; i++)
		{
			m_Fireballs[i].m_FireBallPos = m_Player.GetPosition();
			m_Fireballs[i].m_SmokePos = m_Player.GetPosition();
			m_Fireballs[i].m_ExplodeAnim.Reset();
			m_Fireballs[i].m_SmokeAnimTime = m_SmokeTotalAnimTime;
			m_Fireballs[i].m_bPlaySound = false;
		}
		m_CurrentFireBalls = 0;
		BONUS_DURATION = 4000f;
		m_ShootLatency = 0f;
		base.InitBonus();
	}

	public override void Update(GameTime gameTime)
	{
		if (m_Player == null)
		{
			return;
		}
		UpdatePosition(gameTime, m_Player.GetPosition());
		bool flag = m_CurrentFireBalls >= m_Fireballs.Length - 1;
		if (m_Player.m_bIsOnGround && m_ShootLatency <= 0f && !flag && InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
		{
			m_Fireballs[m_CurrentFireBalls].m_SmokePos = m_Player.GetPosition();
			m_Fireballs[m_CurrentFireBalls].m_FireBallPos = m_Fireballs[m_CurrentFireBalls].m_SmokePos;
			m_Fireballs[m_CurrentFireBalls].m_SmokePos.X -= m_Fireballs[m_CurrentFireBalls].m_ExplodeAnim.m_FrameWidth / 2;
			m_Fireballs[m_CurrentFireBalls].m_SmokePos.Y -= m_Fireballs[m_CurrentFireBalls].m_ExplodeAnim.m_FrameHeight / 2;
			m_Fireballs[m_CurrentFireBalls].m_FireBallPos.X -= m_Fireballs[m_CurrentFireBalls].m_FireBall.m_FrameWidth / 2;
			m_Fireballs[m_CurrentFireBalls].m_FireBallPos.Y -= m_Fireballs[m_CurrentFireBalls].m_FireBall.m_FrameHeight / 2;
			m_Fireballs[m_CurrentFireBalls].m_VileSound.Play();
			m_CurrentFireBalls++;
			m_ShootLatency = 500f;
		}
		if (m_ShootLatency > 0f)
		{
			m_ShootLatency -= gameTime.ElapsedGameTime.Milliseconds;
		}
		if (m_CurrentFireBalls > 0)
		{
			for (int i = 0; i < m_CurrentFireBalls; i++)
			{
				m_Fireballs[i].m_ExplodeAnim.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
				m_Fireballs[i].m_FireBall.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
				m_Fireballs[i].m_SmokeAnimTime -= gameTime.ElapsedGameTime.Milliseconds;
				if (m_Fireballs[i].m_FireBallPos.Y > 0f)
				{
					flag = false;
				}
				if (!(m_Fireballs[i].m_SmokeAnimTime <= m_SmokeTotalAnimTime / 2f))
				{
					continue;
				}
				if (!m_Fireballs[i].m_bPlaySound)
				{
					m_Fireballs[i].m_FireSound.Play();
					m_Fireballs[i].m_bPlaySound = true;
				}
				m_Fireballs[i].m_FireBallPos.Y -= 0.5f * (float)gameTime.ElapsedGameTime.Milliseconds;
				Vector2 fireBallPos = m_Fireballs[i].m_FireBallPos;
				fireBallPos.X += m_Fireballs[i].m_FireBall.m_FrameWidth / 2;
				fireBallPos.Y += m_Fireballs[i].m_FireBall.m_FrameHeight / 2;
				for (int j = 0; j < m_Player.m_GameStateInstance.m_Players.Count; j++)
				{
					Player player = m_Player.m_GameStateInstance.m_Players[j];
					if (player != m_Player && !player.m_bSpecialEnable && (player.m_Tag == 0 || player.m_Tag == 2) && Vector2.Distance(player.GetPosition(), fireBallPos) < 70f)
					{
						player.m_Tag = 1;
						m_Player.IncreaseScore(1);
					}
				}
			}
		}
		if (m_EffectTimer > 0f)
		{
			m_EffectTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		if (flag)
		{
			StopBonus();
		}
	}

	public override void StopBonus()
	{
		for (int i = 0; i < m_CurrentFireBalls; i++)
		{
			m_Fireballs[i].m_FireSound.Stop();
			m_Fireballs[i].m_VileSound.Stop();
			m_Player.m_GameStateInstance.m_BonusSpawnEffect.Trigger(m_Fireballs[i].m_FireBallPos);
		}
		m_ShootLatency = 0f;
		base.StopBonus();
	}

	public override void DrawBonus()
	{
		if (m_CurrentFireBalls > 0 && m_Player != null)
		{
			for (int i = 0; i < m_CurrentFireBalls; i++)
			{
				if (m_Fireballs[i].m_SmokeAnimTime >= 0f)
				{
					m_Fireballs[i].m_ExplodeAnim.Draw(ref m_Fireballs[i].m_SmokePos, SpriteEffects.None, Color.White, m_zorder);
				}
				if (m_Fireballs[i].m_SmokeAnimTime <= m_SmokeTotalAnimTime / 2f)
				{
					m_Fireballs[i].m_FireBall.Draw(ref m_Fireballs[i].m_FireBallPos, SpriteEffects.None, Color.White, m_zorder);
				}
			}
		}
		m_Sprite.Draw(ref m_MiddlePosition, m_Effect, Color.White, m_zorder);
		base.DrawBonus();
	}
}
