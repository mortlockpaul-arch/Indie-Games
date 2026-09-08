using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class Mort : SpecialCharacter
{
	public const float SPECIAL_DURATION = 15000f;

	public const float SPECIAL_START_DURATION = 100f;

	private const int INSTANT_KILL_DISTANCE = 70;

	private const float DASH_TIME = 200f;

	private const float DASH_LATENCY = 200f;

	private const float DASH_IMPULSE = 3000f;

	private const float Speed = 40f;

	public MercuryParticle m_SpecialFx;

	public bool m_bStarted;

	private float m_CharacterSpeed;

	private float m_DashTimer;

	private float m_DashLatency;

	private float m_MaxJumpImpulse;

	private Vector2 m_DeathDashImpulse;

	private AudioClip m_DashSound;

	public Mort(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/DeathSpecial");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "DeathSpecialFx", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		m_DashSound = new AudioClip("Spe_DeathDash");
		m_bStarted = false;
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
		m_DeathDashImpulse = new Vector2(3000f, 0f);
	}

	public override void InitSpecial()
	{
		m_SpecialTime = 100f;
		m_bStarted = false;
		m_SpecialFx.m_pe[1].MinimumTriggerPeriod = 0f;
		m_CharacterSpeed = m_Player.m_Speed;
		m_Player.m_Speed = 40f;
		m_MaxJumpImpulse = m_Player.m_MaxJumpImpulse;
		m_Player.m_MaxJumpImpulse *= 1.1f;
	}

	public override void StopSpecial()
	{
		m_Player.m_Speed = m_CharacterSpeed;
		m_Player.m_MaxJumpImpulse = m_MaxJumpImpulse;
		base.StopSpecial();
	}

	public override void Update(GameTime gameTime)
	{
		m_SpecialTime -= gameTime.ElapsedGameTime.Milliseconds;
		m_SpecialFx.Trigger(m_Player.GetPosition());
		if (m_Player == null)
		{
			return;
		}
		if (m_Player.m_bIsPlayerBot)
		{
			PlayerBot playerBot = (PlayerBot)m_Player;
			if (playerBot.m_CurrentTarget != null && playerBot.m_bCloseToTargetMode)
			{
				InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: true);
			}
			else
			{
				InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: false);
			}
		}
		if (m_DashLatency < 0f)
		{
			if (m_DashTimer <= 0f && InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_DashSound.Play();
				m_DashTimer = 200f;
			}
			else if (m_DashTimer > 0f)
			{
				m_DashTimer -= gameTime.ElapsedGameTime.Milliseconds;
				if (m_Player.m_SpriteEffect == SpriteEffects.None)
				{
					m_Player.GetBody().ApplyLinearImpulse(ref m_DeathDashImpulse);
				}
				else
				{
					Vector2 impulse = m_DeathDashImpulse * -1f;
					m_Player.GetBody().ApplyLinearImpulse(ref impulse);
				}
				for (int i = 0; i < m_Player.m_GameStateInstance.m_Players.Count; i++)
				{
					if (m_Player.m_GameStateInstance.m_Players[i] != m_Player && Vector2.Distance(m_Player.m_GameStateInstance.m_Players[i].GetPosition(), m_Player.GetPosition()) < 70f && m_Player.m_GameStateInstance.m_Players[i].m_Tag == 0)
					{
						m_Player.m_GameStateInstance.m_Players[i].m_Tag = 1;
						m_Player.IncreaseScore(1);
					}
				}
				m_Player.SetAnimation(Player.AnimStates.EXPLODE);
				if (m_DashTimer <= 0f)
				{
					m_DashLatency = 200f;
				}
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
				m_SpecialFx.m_pe[1].MinimumTriggerPeriod = 1000000f;
				m_Player.m_CurrentAnim = Player.AnimStates.SP_STAND;
			}
		}
		else
		{
			m_DashLatency -= gameTime.ElapsedGameTime.Milliseconds;
		}
	}
}
