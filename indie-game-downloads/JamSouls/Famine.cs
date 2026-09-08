using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class Famine : SpecialCharacter
{
	public const float SPECIAL_DURATION = 12000f;

	public const float SPECIAL_START_DURATION = 250f;

	public const int INSTANT_KILL_DISTANCE = 70;

	public const float SPEED = 5f;

	public MercuryParticle m_SpecialFx;

	public bool m_bStarted;

	private float m_Zorder;

	public Famine(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/FamineSpecial");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "Famine", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		m_bStarted = false;
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
	}

	public override void InitSpecial()
	{
		m_SpecialTime = 250f;
		m_bStarted = false;
		m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 0f;
		m_Player.GetBody().IgnoreGravity = true;
		m_Player.GetFixture().CollidesWith = CollisionCategory.Cat5 | CollisionCategory.Cat8;
		m_Player.SetAnimation(Player.AnimStates.WALK);
		m_Player.m_Origin = new Vector2(m_Player.m_PlayerSprite[6].m_FrameWidth / 2, m_Player.m_PlayerSprite[6].m_FrameHeight / 2);
		m_Zorder = m_Player.m_zOrder;
		m_Player.m_zOrder = 1f;
	}

	public override void StopSpecial()
	{
		m_Player.m_Origin = Vector2.Zero;
		m_Player.m_Rotation = 0f;
		m_Player.m_zOrder = m_Zorder;
		m_Player.GetBody().IgnoreGravity = false;
		m_Player.GetFixture().CollidesWith = CollisionCategory.All;
		base.StopSpecial();
	}

	public override void Update(GameTime gameTime)
	{
		m_SpecialTime -= gameTime.ElapsedGameTime.Milliseconds;
		m_SpecialFx.Trigger(m_Player.GetPosition());
		m_Player.m_CurrentAnim = Player.AnimStates.SP_STAND;
		if (m_SpecialTime <= 0f)
		{
			if (m_bStarted)
			{
				StopSpecial();
			}
			else
			{
				m_SpecialTime = 12000f;
				m_bStarted = true;
				m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 1000000f;
				m_Player.m_CurrentAnim = Player.AnimStates.SP_STAND;
			}
		}
		else
		{
			if (m_Player.m_bIsPlayerBot)
			{
				PlayerBot playerBot = (PlayerBot)m_Player;
				if (playerBot.m_CurrentTarget != null)
				{
					Vector2 position = playerBot.m_CurrentTarget.GetPosition();
					Vector2 position2 = m_Player.GetPosition();
					if (position.X < position2.X - 70f)
					{
						InputManager.SetKeyState(m_Player.m_PlayerNum, 1, pressed: true);
						InputManager.SetKeyState(m_Player.m_PlayerNum, 3, pressed: false);
					}
					else if (position.X > position2.X + 70f)
					{
						InputManager.SetKeyState(m_Player.m_PlayerNum, 1, pressed: false);
						InputManager.SetKeyState(m_Player.m_PlayerNum, 3, pressed: true);
					}
					else if (position.Y < position2.Y - 70f)
					{
						InputManager.SetKeyState(m_Player.m_PlayerNum, 0, pressed: true);
						InputManager.SetKeyState(m_Player.m_PlayerNum, 2, pressed: false);
					}
					else if (position.Y > position2.Y + 70f)
					{
						InputManager.SetKeyState(m_Player.m_PlayerNum, 0, pressed: false);
						InputManager.SetKeyState(m_Player.m_PlayerNum, 2, pressed: true);
					}
				}
			}
			Vector2 zero = Vector2.Zero;
			if (!m_Player.m_bIsPlayerBot)
			{
				if (InputManager.GetKeyState(m_Player.m_PlayerNum, 2) == ButtonState.Pressed)
				{
					zero.Y += 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_Rotation = -80f;
					if (m_Player.m_SpriteEffect == SpriteEffects.FlipHorizontally)
					{
						m_Player.m_Rotation *= -1f;
					}
				}
				else if (InputManager.GetKeyState(m_Player.m_PlayerNum, 0) == ButtonState.Pressed)
				{
					zero.Y -= 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_Rotation = 80f;
					if (m_Player.m_SpriteEffect == SpriteEffects.FlipHorizontally)
					{
						m_Player.m_Rotation *= -1f;
					}
				}
				else if (InputManager.GetKeyState(m_Player.m_PlayerNum, 1) == ButtonState.Pressed)
				{
					zero.X -= 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_SpriteEffect = SpriteEffects.FlipHorizontally;
					m_Player.m_Rotation = 0f;
				}
				else if (InputManager.GetKeyState(m_Player.m_PlayerNum, 3) == ButtonState.Pressed)
				{
					zero.X += 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_SpriteEffect = SpriteEffects.None;
					m_Player.m_Rotation = 0f;
				}
			}
			else
			{
				PlayerBot playerBot2 = (PlayerBot)m_Player;
				if (InputManager.GetKeyState(playerBot2.m_PlayerNum, 2) == ButtonState.Pressed)
				{
					zero.Y += 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_Rotation = -80f;
					if (m_Player.m_SpriteEffect == SpriteEffects.FlipHorizontally)
					{
						m_Player.m_Rotation *= -1f;
					}
				}
				else if (InputManager.GetKeyState(playerBot2.m_PlayerNum, 0) == ButtonState.Pressed)
				{
					zero.Y -= 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_Rotation = 80f;
					if (m_Player.m_SpriteEffect == SpriteEffects.FlipHorizontally)
					{
						m_Player.m_Rotation *= -1f;
					}
				}
				else if (InputManager.GetKeyState(playerBot2.m_PlayerNum, 1) == ButtonState.Pressed)
				{
					zero.X -= 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_SpriteEffect = SpriteEffects.FlipHorizontally;
					m_Player.m_Rotation = 0f;
				}
				else if (InputManager.GetKeyState(playerBot2.m_PlayerNum, 3) == ButtonState.Pressed)
				{
					zero.X += 5f * (float)gameTime.ElapsedGameTime.Milliseconds;
					m_Player.m_SpriteEffect = SpriteEffects.None;
					m_Player.m_Rotation = 0f;
				}
			}
			m_Player.GetBody().LinearVelocity = zero;
		}
		for (int i = 0; i < m_Player.m_GameStateInstance.m_Players.Count; i++)
		{
			if (m_Player.m_GameStateInstance.m_Players[i] != m_Player && Vector2.Distance(m_Player.m_GameStateInstance.m_Players[i].GetPosition(), m_Player.GetPosition()) < 70f && m_Player.m_GameStateInstance.m_Players[i].m_Tag == 0)
			{
				m_Player.m_GameStateInstance.m_Players[i].m_Tag = 1;
				m_Player.IncreaseScore(1);
			}
		}
	}
}
