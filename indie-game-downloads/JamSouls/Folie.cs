using Microsoft.Xna.Framework;
using ProjectMercury;

namespace JamSouls;

public class Folie : SpecialCharacter
{
	public const float SPECIAL_DURATION = 12000f;

	public const float SPECIAL_START_DURATION = 250f;

	public const int INSTANT_KILL_DISTANCE = 90;

	public const float SpecialRestitution = 1f;

	public const float SpecialLinearDamping = 0.5f;

	public const float SpecialFriction = 0f;

	public const float VELOCITY_CLAMP = 150f;

	public const float Speed = 50f;

	public MercuryParticle m_SpecialFx;

	public bool m_bStarted;

	public float Normalrestitution;

	public float NormalLinearDamping;

	public float NormalFriction;

	public float NormalJumpImpulse;

	public float m_PlayerSpeed;

	public Folie(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/FolieSpecial");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "Folie", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		m_bStarted = false;
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
	}

	public override void InitSpecial()
	{
		m_SpecialTime = 250f;
		m_bStarted = false;
		NormalFriction = m_Player.GetBody().FixtureList[0].Friction;
		NormalLinearDamping = m_Player.GetBody().LinearDamping;
		Normalrestitution = m_Player.GetBody().FixtureList[0].Restitution;
		NormalJumpImpulse = m_Player.m_MaxJumpImpulse;
		m_Player.GetBody().FixtureList[0].Restitution = 1f;
		m_Player.GetBody().FixtureList[0].Friction = 0f;
		m_Player.GetBody().LinearDamping = 0.5f;
		m_Player.m_MaxJumpImpulse -= 4f;
		m_Player.m_bDampingEnable = true;
		m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 0f;
		m_PlayerSpeed = m_Player.m_Speed;
		m_Player.m_Speed = 50f;
	}

	public override void StopSpecial()
	{
		m_Player.GetBody().FixtureList[0].Restitution = Normalrestitution;
		m_Player.GetBody().FixtureList[0].Friction = NormalFriction;
		m_Player.GetBody().LinearDamping = NormalLinearDamping;
		m_Player.m_MaxJumpImpulse = NormalJumpImpulse;
		m_Player.m_bLockJump = false;
		m_Player.m_bIsOnGround = true;
		m_Player.m_bDampingEnable = false;
		m_Player.m_Speed = m_PlayerSpeed;
		base.StopSpecial();
	}

	public override void Update(GameTime gameTime)
	{
		m_SpecialTime -= gameTime.ElapsedGameTime.Milliseconds;
		m_SpecialFx.Trigger(m_Player.GetPosition());
		if (m_Player.m_bRightRelease && m_Player.m_bLeftRelease)
		{
			m_Player.GetBody().LinearVelocity = new Vector2(0f, m_Player.GetBody().LinearVelocity.Y);
			m_Player.SetAnimation(Player.AnimStates.STAND);
		}
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
		Vector2 position = m_Player.GetPosition();
		position.Y -= 25f;
		for (int i = 0; i < m_Player.m_GameStateInstance.m_Players.Count; i++)
		{
			if (m_Player.m_GameStateInstance.m_Players[i] != m_Player && Vector2.Distance(m_Player.m_GameStateInstance.m_Players[i].GetPosition(), position) < 90f && m_Player.m_GameStateInstance.m_Players[i].m_Tag == 0)
			{
				m_Player.m_GameStateInstance.m_Players[i].m_Tag = 1;
				m_Player.IncreaseScore(1);
			}
		}
	}
}
