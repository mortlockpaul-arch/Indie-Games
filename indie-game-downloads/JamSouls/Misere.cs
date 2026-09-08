using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;

namespace JamSouls;

public class Misere : SpecialCharacter
{
	private const float SPECIAL_DURATION = 15000f;

	private const float SPECIAL_START_DURATION = 100f;

	private const float TRAP_TIMER = 1800f;

	private const int TRAP_DISTANCE = 400;

	private const float Speed = 40f;

	private MercuryParticle m_SpecialFx;

	private bool m_bStarted;

	private float m_CharacterSpeed;

	private Vector2 m_MisereJumpImpulse = new Vector2(0f, -30f);

	private AudioClip m_DashSound;

	private float m_zorder;

	private float[] m_TrapTime = new float[4];

	private MercuryParticle[] m_TrapFx = new MercuryParticle[4];

	private MercuryParticle[] m_TrapParticle = new MercuryParticle[4];

	public Misere(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/MisereSpecial");
		ParticleEffect particleEffect = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/MisereTrap");
		ParticleEffect particleEffect2 = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/TrapParticle");
		m_DashSound = new AudioClip("Spe_MisereDash");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "MisereSpecialFx", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		for (int i = 0; i < m_TrapFx.Length; i++)
		{
			m_TrapFx[i] = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, particleEffect.DeepCopy(), "MisereTrapFx", m_Player.GetZ(), bUseBlending: false);
			m_TrapFx[i].SetAutoTrigger(bAutoTrigger: false);
			m_Player.m_GameStateInstance.AddParticle(m_TrapFx[i]);
			m_TrapParticle[i] = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, particleEffect2.DeepCopy(), "TrapParticle", m_Player.GetZ(), bUseBlending: false);
			m_TrapParticle[i].SetAutoTrigger(bAutoTrigger: false);
			m_Player.m_GameStateInstance.AddParticle(m_TrapParticle[i]);
		}
		m_bStarted = false;
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
	}

	public override void InitSpecial()
	{
		m_SpecialTime = 100f;
		m_bStarted = false;
		m_SpecialFx.m_pe[1].MinimumTriggerPeriod = 0f;
		m_CharacterSpeed = m_Player.m_Speed;
		m_Player.m_Speed = 40f;
		m_zorder = m_Player.GetZ();
		m_Player.SetZ(1f);
		for (int i = 0; i < m_TrapFx.Length; i++)
		{
			m_TrapFx[i].m_zOrder = m_Player.GetZ() - 0.01f;
		}
	}

	public override void StopSpecial()
	{
		m_Player.m_Speed = m_CharacterSpeed;
		m_Player.GetBody().IsStatic = false;
		if (m_Player.m_bIsPlayerBot)
		{
			InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: false);
		}
		m_Player.SetZ(m_zorder);
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
				InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: false);
				for (int i = 0; i < m_Player.m_GameStateInstance.m_Players.Count; i++)
				{
					if (m_Player.m_GameStateInstance.m_Players[i] != m_Player)
					{
						Vector2 position = m_Player.m_GameStateInstance.m_Players[i].GetPosition();
						if (Vector2.Distance(position, m_Player.GetPosition()) < 400f)
						{
							InputManager.SetKeyState(m_Player.m_PlayerNum, 6, pressed: true);
							break;
						}
					}
				}
			}
			if (InputManager.GetKeyState(m_Player.m_PlayerNum, 6) == ButtonState.Pressed)
			{
				if (!m_Player.GetBody().IsStatic)
				{
					m_DashSound.Play();
					m_Player.GetBody().IsStatic = true;
				}
				m_Player.SetAnimation(Player.AnimStates.EXPLODE);
				for (int j = 0; j < m_Player.m_GameStateInstance.m_Players.Count; j++)
				{
					if (m_Player.m_GameStateInstance.m_Players[j] == m_Player)
					{
						continue;
					}
					Vector2 position2 = m_Player.m_GameStateInstance.m_Players[j].GetPosition();
					if (Vector2.Distance(position2, m_Player.GetPosition()) < 400f)
					{
						if (m_Player.m_GameStateInstance.m_Players[j].m_Tag == 0)
						{
							m_TrapTime[j] += gameTime.ElapsedGameTime.Milliseconds;
							m_TrapParticle[j].m_zOrder = m_Player.m_GameStateInstance.m_Players[j].GetZ() - 0.01f;
							m_TrapParticle[j].Trigger(position2);
							m_TrapFx[j].Trigger(Vector2.Lerp(position2, m_Player.GetPosition(), m_TrapTime[j] / 1800f));
							if (m_TrapTime[j] > 1800f)
							{
								m_Player.m_GameStateInstance.m_Players[j].DecreaseScore(1);
								m_Player.IncreaseScore(1);
								m_TrapTime[j] = 0f;
							}
						}
						else
						{
							m_TrapTime[j] = 0f;
						}
					}
					else
					{
						m_TrapTime[j] = 0f;
					}
				}
			}
			else
			{
				m_Player.GetBody().IsStatic = false;
				if (m_Player.m_bLockJump && InputManager.GetKeyState(m_Player.m_PlayerNum, 4) == ButtonState.Pressed)
				{
					m_Player.GetBody().ApplyLinearImpulse(ref m_MisereJumpImpulse);
					m_Player.SetAnimation(Player.AnimStates.JUMP);
				}
			}
			if (m_Player.GetBody().LinearVelocity.Y > 46f)
			{
				m_Player.GetBody().LinearVelocity = new Vector2(m_Player.GetBody().LinearVelocity.X, 50f);
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
