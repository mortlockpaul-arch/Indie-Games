using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class Maladie : SpecialCharacter
{
	public const float SPECIAL_DURATION = 15000f;

	public const float SPECIAL_START_DURATION = 100f;

	public const int INSTANT_KILL_DISTANCE = 80;

	public const int DAMAGE_DISTANCE = 220;

	public const int DAMAGE = 10;

	public const float SPEED = 600f;

	public const float DAMAGE_TIMER = 100f;

	public MercuryParticle m_SpecialFx;

	public bool m_bStarted;

	public float m_CharacterImpulse;

	public float m_CharacterSpeed;

	public float m_CharacterRestitution;

	public AudioClip m_AieSound;

	public float m_DamageTimer;

	private float m_zorder;

	public Maladie(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/MaladieSpecial");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "MaladieSpecialFx", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		m_AieSound = new AudioClip("Bullet_Flesh");
		m_bStarted = false;
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
	}

	public override void InitSpecial()
	{
		m_CharacterImpulse = m_Player.m_MaxJumpImpulse;
		m_CharacterSpeed = m_Player.m_Speed;
		m_CharacterRestitution = m_Player.GetFixture().Restitution;
		m_SpecialTime = 100f;
		m_bStarted = false;
		m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 0f;
		m_Player.m_MaxJumpImpulse = 0f;
		m_Player.m_Speed = 0f;
		m_Player.m_bControlHorizontalMove = false;
		m_Player.GetBody().IgnoreGravity = true;
		m_Player.GetFixture().Restitution = 0.6f;
		m_DamageTimer = 0f;
		m_zorder = m_Player.GetZ();
		m_Player.m_zOrder = 1f;
		m_Player.GetFixture().CollidesWith = CollisionCategory.Cat5 | CollisionCategory.Cat8;
	}

	public override void StopSpecial()
	{
		m_Player.GetBody().IgnoreGravity = false;
		m_Player.m_MaxJumpImpulse = m_CharacterImpulse;
		m_Player.m_Speed = m_CharacterSpeed;
		m_Player.m_bControlHorizontalMove = true;
		m_Player.GetFixture().Restitution = m_CharacterRestitution;
		m_Player.GetBody().IgnoreGravity = false;
		m_Player.GetFixture().CollidesWith = CollisionCategory.All;
		m_Player.m_zOrder = m_zorder;
		base.StopSpecial();
		m_Player.SetAnimation(Player.AnimStates.STAND);
		if (!m_Player.m_bLeftRelease || !m_Player.m_bRightRelease)
		{
			m_Player.SetAnimation(Player.AnimStates.WALK);
		}
	}

	public override void Update(GameTime gameTime)
	{
		m_SpecialTime -= gameTime.ElapsedGameTime.Milliseconds;
		m_DamageTimer -= gameTime.ElapsedGameTime.Milliseconds;
		m_SpecialFx.Trigger(m_Player.GetPosition());
		if (m_Player.m_bIsPlayerBot)
		{
			PlayerBot playerBot = (PlayerBot)m_Player;
			if (playerBot.m_CurrentTarget != null)
			{
				Vector2 position = m_Player.GetPosition();
				Vector2 position2 = playerBot.m_CurrentTarget.GetPosition();
				if (position2.X < position.X)
				{
					InputManager.SetKeyState(m_Player.m_PlayerNum, 1, pressed: true);
					InputManager.SetKeyState(m_Player.m_PlayerNum, 3, pressed: false);
				}
				else if (position2.X > position.X)
				{
					InputManager.SetKeyState(m_Player.m_PlayerNum, 1, pressed: false);
					InputManager.SetKeyState(m_Player.m_PlayerNum, 3, pressed: true);
				}
				if (position2.Y < position.Y)
				{
					InputManager.SetKeyState(m_Player.m_PlayerNum, 0, pressed: true);
					InputManager.SetKeyState(m_Player.m_PlayerNum, 2, pressed: false);
				}
				else if (position2.Y > position.Y)
				{
					InputManager.SetKeyState(m_Player.m_PlayerNum, 0, pressed: false);
					InputManager.SetKeyState(m_Player.m_PlayerNum, 2, pressed: true);
				}
			}
		}
		if (InputManager.GetKeyState(m_Player.m_PlayerNum, 2) == ButtonState.Pressed)
		{
			m_Player.GetBody().ApplyForce(new Vector2(0f, 600f));
		}
		if (InputManager.GetKeyState(m_Player.m_PlayerNum, 0) == ButtonState.Pressed)
		{
			m_Player.GetBody().ApplyForce(new Vector2(0f, -600f));
		}
		if (InputManager.GetKeyState(m_Player.m_PlayerNum, 1) == ButtonState.Pressed)
		{
			m_Player.GetBody().ApplyForce(new Vector2(-600f, 0f));
			m_Player.m_SpriteEffect = SpriteEffects.FlipHorizontally;
		}
		if (InputManager.GetKeyState(m_Player.m_PlayerNum, 3) == ButtonState.Pressed)
		{
			m_Player.GetBody().ApplyForce(new Vector2(600f, 0f));
			m_Player.m_SpriteEffect = SpriteEffects.None;
		}
		for (int i = 0; i < m_Player.m_GameStateInstance.m_Players.Count; i++)
		{
			if (m_Player.m_GameStateInstance.m_Players[i] == m_Player)
			{
				continue;
			}
			if (Vector2.Distance(m_Player.m_GameStateInstance.m_Players[i].GetPosition(), m_Player.GetPosition()) < 80f && m_Player.m_GameStateInstance.m_Players[i].m_Tag == 0)
			{
				m_Player.m_GameStateInstance.m_Players[i].m_Tag = 1;
				m_Player.IncreaseScore(1);
			}
			if (!(m_DamageTimer <= 0f) || !(Vector2.Distance(m_Player.m_GameStateInstance.m_Players[i].GetPosition(), m_Player.GetPosition()) < 220f))
			{
				continue;
			}
			if (m_Player.m_GameStateInstance.m_Players[i].m_Tag == 0)
			{
				m_Player.m_GameStateInstance.m_Players[i].m_life -= 10;
				m_Player.m_GameStateInstance.m_Players[i].m_BleedingEmitter.Trigger(m_Player.m_GameStateInstance.m_Players[i].GetPosition());
				m_AieSound.Play();
				if (m_Player.m_GameStateInstance.m_Players[i].m_life <= 0)
				{
					m_Player.IncreaseScore(1);
					m_Player.m_GameStateInstance.m_Players[i].m_Tag = 1;
				}
			}
			m_DamageTimer = 100f;
		}
		if (m_SpecialTime <= 0f)
		{
			if (m_bStarted)
			{
				StopSpecial();
				return;
			}
			m_SpecialTime = 15000f;
			m_bStarted = true;
			m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 1000000f;
			m_Player.m_CurrentAnim = Player.AnimStates.SP_STAND;
		}
	}
}
