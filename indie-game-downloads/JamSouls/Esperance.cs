using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class Esperance : SpecialCharacter
{
	private const float SPECIAL_DURATION = 15000f;

	private const float SPECIAL_START_DURATION = 100f;

	private const float IMPULSE_LATENCY = 500f;

	private const float SWORD_RANGE = 60f;

	private const float SWORD_SIDE = 10f;

	private const float Speed = 45f;

	private MercuryParticle m_SpecialFx;

	private bool m_bStarted;

	private float m_CharacterSpeed;

	private float m_DashTimer;

	private float AnimLatency;

	private AudioClip m_CutSound;

	public Esperance(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/EsperanceSpecial");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "EsperanceSpecial", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		m_CutSound = new AudioClip("Spe_Esperance");
		m_bStarted = false;
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
	}

	public override void InitSpecial()
	{
		m_SpecialTime = 100f;
		m_bStarted = false;
		m_SpecialFx.m_pe[1].MinimumTriggerPeriod = 0f;
		m_CharacterSpeed = m_Player.m_Speed;
		m_Player.m_Speed = 45f;
		AnimLatency = m_Player.m_PlayerSprite[11].m_Speed * (float)m_Player.m_PlayerSprite[11].m_TotalFrames;
	}

	public override void StopSpecial()
	{
		m_Player.m_Speed = m_CharacterSpeed;
		base.StopSpecial();
	}

	public override void Update(GameTime gameTime)
	{
		m_SpecialTime -= gameTime.ElapsedGameTime.Milliseconds;
		m_SpecialFx.Trigger(m_Player.GetPosition());
		if (m_Player != null)
		{
			if (m_Player.m_bIsPlayerBot)
			{
				PlayerBot playerBot = (PlayerBot)m_Player;
				if (playerBot.m_bCloseToTargetMode)
				{
					InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: true);
				}
				else
				{
					InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: false);
				}
			}
			if (m_DashTimer <= 0f && InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				m_DashTimer = 500f;
				AnimLatency = m_Player.m_PlayerSprite[11].m_Speed * (float)m_Player.m_PlayerSprite[11].m_TotalFrames;
				m_Player.m_PlayerSprite[11].Reset();
				m_CutSound.Play();
			}
			else if (m_DashTimer > 0f)
			{
				Vector2 position = m_Player.GetPosition();
				if (m_Player.m_SpriteEffect == SpriteEffects.FlipHorizontally)
				{
					position.X -= 10f;
				}
				else
				{
					position.X += 10f;
				}
				for (int i = 0; i < m_Player.m_GameStateInstance.m_Players.Count; i++)
				{
					if (m_Player.m_GameStateInstance.m_Players[i] != m_Player && Vector2.Distance(m_Player.m_GameStateInstance.m_Players[i].GetPosition(), position) < 60f && m_Player.m_GameStateInstance.m_Players[i].m_Tag == 0)
					{
						m_Player.m_GameStateInstance.m_Players[i].m_Tag = 1;
						m_Player.IncreaseScore(1);
					}
				}
				if (AnimLatency > 0f)
				{
					m_Player.SetAnimation(Player.AnimStates.EXPLODE);
				}
				AnimLatency -= gameTime.ElapsedGameTime.Milliseconds;
				m_DashTimer -= gameTime.ElapsedGameTime.Milliseconds;
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
}
