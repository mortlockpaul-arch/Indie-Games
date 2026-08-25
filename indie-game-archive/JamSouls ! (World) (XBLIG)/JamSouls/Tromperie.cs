using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class Tromperie : SpecialCharacter
{
	public const float SPECIAL_DURATION = 12000f;

	public const float SPECIAL_START_DURATION = 100f;

	private const float HEAD_H_SPEED = 0.5f;

	private const float HEAD_START_Y = 20f;

	private const float DASH_SPEED = 2f;

	private const float MOUTH_MAX_LEFT = -(float)Math.PI / 18f;

	private const float MOUTH_MAX_RIGHT = (float)Math.PI / 18f;

	private const float BALANCE_SPEED = 4f;

	private MercuryParticle m_SpecialFx;

	private bool m_bStarted;

	private bool m_bDash;

	private AudioClip m_DashSound;

	private Vector2 m_CharLianePos;

	private Vector2 m_HeadLianePos;

	private Vector2 m_LeefPos;

	private Vector2 m_MouthPos;

	private float m_MouthRotation;

	private float m_MouthBalanceTime;

	private bool m_MouthBalanceLeft;

	private Texture2D m_LianeTex;

	private Texture2D m_Leef;

	private Texture2D m_MouthClose;

	private Texture2D m_MouthOpen;

	private Rectangle m_LianeRect;

	private Rectangle m_LeefRect;

	private Rectangle m_MouthCloseRect;

	private Rectangle m_MouthOpenRect;

	private float m_DashTime;

	public Tromperie(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/TromperieSpecial");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "MaladieSpecialFx", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
		m_LianeTex = m_Player.m_GameStateInstance.content.Load<Texture2D>("Char/Main/Tromperie/TromperieSpecialTige");
		m_Leef = m_Player.m_GameStateInstance.content.Load<Texture2D>("Char/Main/Tromperie/TromperieSpecialFeuille");
		m_MouthClose = m_Player.m_GameStateInstance.content.Load<Texture2D>("Char/Main/Tromperie/TromperieSpecialIdle");
		m_MouthOpen = m_Player.m_GameStateInstance.content.Load<Texture2D>("Char/Main/Tromperie/TromperieSpecialDash");
		m_DashSound = new AudioClip("Spe_TromperieDash");
		m_LianeRect = new Rectangle(0, 0, m_LianeTex.Width, m_LianeTex.Height);
		m_LeefRect = new Rectangle(0, 0, m_Leef.Width, m_Leef.Height);
		m_MouthOpenRect = new Rectangle(0, 0, m_MouthOpen.Width, m_MouthOpen.Height);
		m_MouthCloseRect = new Rectangle(0, 0, m_MouthClose.Width, m_MouthClose.Height);
	}

	public override void InitSpecial()
	{
		m_SpecialTime = 100f;
		m_bStarted = false;
		m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 0f;
		m_Player.m_bControlHorizontalMove = false;
		m_Player.GetBody().Active = false;
		m_Player.SetSpriteEffect(SpriteEffects.None);
		m_MouthPos.X = 640f;
		m_MouthPos.Y = 20f;
		m_Player.m_GameStateInstance.ScreenManager.StartFx(ScreenManager.FadeFx.FADE_FROM_WHITE, 0.2f);
	}

	public override void StopSpecial()
	{
		m_Player.m_bControlHorizontalMove = true;
		base.StopSpecial();
		m_Player.GetBody().Active = true;
		m_DashTime = 0f;
		m_bDash = false;
		m_MouthRotation = 0f;
		m_MouthBalanceTime = 0f;
		m_MouthBalanceLeft = false;
		m_Player.SetAnimation(Player.AnimStates.STAND);
	}

	public override void Update(GameTime gameTime)
	{
		m_SpecialTime -= gameTime.ElapsedGameTime.Milliseconds;
		m_SpecialFx.Trigger(m_Player.GetPosition());
		if (!m_bDash)
		{
			if (m_Player.m_bIsPlayerBot)
			{
				PlayerBot playerBot = (PlayerBot)m_Player;
				InputManager.SetKeyState(m_Player.m_PlayerNum, 1, pressed: false);
				InputManager.SetKeyState(m_Player.m_PlayerNum, 3, pressed: false);
				InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: false);
				if (playerBot.m_CurrentTarget != null)
				{
					Vector2 position = playerBot.m_CurrentTarget.GetPosition();
					if (position.X < m_HeadLianePos.X - (float)(m_MouthOpen.Width / 2))
					{
						InputManager.SetKeyState(m_Player.m_PlayerNum, 1, pressed: true);
						InputManager.SetKeyState(m_Player.m_PlayerNum, 3, pressed: false);
					}
					else if (position.X > m_HeadLianePos.X + (float)(m_MouthOpen.Width / 2))
					{
						InputManager.SetKeyState(m_Player.m_PlayerNum, 1, pressed: false);
						InputManager.SetKeyState(m_Player.m_PlayerNum, 3, pressed: true);
					}
					else
					{
						InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: true);
					}
				}
			}
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 1) == ButtonState.Pressed)
			{
				m_MouthPos.X -= 0.5f * (float)gameTime.ElapsedGameTime.Milliseconds;
			}
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 3) == ButtonState.Pressed)
			{
				m_MouthPos.X += 0.5f * (float)gameTime.ElapsedGameTime.Milliseconds;
			}
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_bDash = true;
				m_DashTime = 0f;
				m_DashSound.Play();
			}
			if (m_MouthPos.X - (float)(m_MouthCloseRect.Width / 2) < 0f)
			{
				m_MouthPos.X = m_MouthCloseRect.Width / 2;
			}
			else if (m_MouthPos.X + (float)(m_MouthCloseRect.Width / 2) > 1280f)
			{
				m_MouthPos.X = 1280 - m_MouthCloseRect.Width / 2;
			}
			m_MouthBalanceTime += (float)gameTime.ElapsedGameTime.Milliseconds / 1000f * 4f;
			if (m_MouthBalanceLeft)
			{
				if (m_MouthBalanceTime <= 1f)
				{
					m_MouthRotation = MathHelper.Lerp(0f, -(float)Math.PI / 18f, m_MouthBalanceTime);
				}
				else
				{
					m_MouthRotation = MathHelper.Lerp(-(float)Math.PI / 18f, 0f, m_MouthBalanceTime - 1f);
					if (m_MouthBalanceTime >= 2f)
					{
						m_MouthBalanceLeft = false;
						m_MouthBalanceTime = 0f;
					}
				}
			}
			else if (m_MouthBalanceTime <= 1f)
			{
				m_MouthRotation = MathHelper.Lerp(0f, (float)Math.PI / 18f, m_MouthBalanceTime);
			}
			else
			{
				m_MouthRotation = MathHelper.Lerp((float)Math.PI / 18f, 0f, m_MouthBalanceTime - 1f);
				if (m_MouthBalanceTime >= 2f)
				{
					m_MouthBalanceLeft = true;
					m_MouthBalanceTime = 0f;
				}
			}
		}
		else
		{
			if (m_DashTime <= 1f)
			{
				m_DashTime += (float)gameTime.ElapsedGameTime.Milliseconds / 1000f * 2f;
				m_MouthPos.Y = MathHelper.Lerp(20f, 700f, m_DashTime);
			}
			else
			{
				m_DashTime += (float)gameTime.ElapsedGameTime.Milliseconds / 1000f * 2f / 2f;
				if (m_DashTime >= 2f)
				{
					m_DashTime = 2f;
					m_bDash = false;
				}
				m_MouthPos.Y = MathHelper.Lerp(700f, 20f, m_DashTime - 1f);
			}
			for (int i = 0; i < m_Player.m_GameStateInstance.m_Players.Count; i++)
			{
				if (m_Player.m_GameStateInstance.m_Players[i] != m_Player && Vector2.Distance(m_Player.m_GameStateInstance.m_Players[i].GetPosition(), m_MouthPos) < (float)(m_MouthOpen.Width / 2) && m_Player.m_GameStateInstance.m_Players[i].m_Tag == 0)
				{
					m_Player.m_GameStateInstance.m_Players[i].m_Tag = 1;
					m_Player.IncreaseScore(1);
				}
			}
		}
		if (m_SpecialTime <= 0f)
		{
			if (m_bStarted)
			{
				StopSpecial();
				m_Player.m_GameStateInstance.ScreenManager.StartFx(ScreenManager.FadeFx.FADE_FROM_WHITE, 0.2f);
			}
			else
			{
				m_SpecialTime = 12000f;
				m_bStarted = true;
				m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 1000000f;
				m_Player.m_CurrentAnim = Player.AnimStates.SP_STAND;
			}
		}
		m_CharLianePos = m_Player.m_AnimPos;
		m_CharLianePos.Y -= m_LianeTex.Height;
		m_CharLianePos.X += m_LianeTex.Width / 2 + 10;
	}

	public override void Draw()
	{
		SpriteBatch spriteBatch = m_Player.m_GameStateInstance.ScreenManager.SpriteBatch;
		float z = m_Player.GetZ();
		spriteBatch.Draw(m_LianeTex, m_CharLianePos, m_LianeRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, z + 1E-06f);
		if (m_DashTime > 0f && m_DashTime <= 1f)
		{
			spriteBatch.Draw(m_MouthOpen, m_MouthPos, m_MouthOpenRect, Color.White, m_MouthRotation, new Vector2(m_MouthOpen.Width / 2, m_MouthOpen.Height / 2), 1f, SpriteEffects.None, 0.999999f);
		}
		else
		{
			spriteBatch.Draw(m_MouthClose, m_MouthPos, m_MouthCloseRect, Color.White, m_MouthRotation, new Vector2(m_MouthClose.Width / 2, m_MouthClose.Height / 2), 1f, SpriteEffects.None, 0.999999f);
		}
		m_LeefPos.X = m_MouthPos.X - (float)(m_Leef.Width / 2);
		m_LeefPos.Y = m_MouthPos.Y - (float)(m_MouthClose.Height / 2) - (float)m_Leef.Height / 1.1f;
		spriteBatch.Draw(m_Leef, m_LeefPos, m_LeefRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		m_HeadLianePos.X = m_MouthPos.X - (float)(m_LianeTex.Width / 2);
		m_HeadLianePos.Y = m_MouthPos.Y - (float)(m_MouthClose.Height / 2) - (float)m_LianeTex.Height;
		spriteBatch.Draw(m_LianeTex, m_HeadLianePos, m_LianeRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99999f);
		m_HeadLianePos.Y -= m_LianeTex.Height;
		spriteBatch.Draw(m_LianeTex, m_HeadLianePos, m_LianeRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99999f);
	}
}
