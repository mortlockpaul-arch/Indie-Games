using Microsoft.Xna.Framework;
using ProjectMercury;

namespace JamSouls;

public class Guerre : SpecialCharacter
{
	public const float SPECIAL_DURATION = 12000f;

	public const float SPECIAL_START_DURATION = 100f;

	public const int INSTANT_KILL_DISTANCE = 70;

	public const float Speed = 50f;

	public MercuryParticle m_SpecialFx;

	public bool m_bStarted;

	public float m_CharacterSpeed;

	public Guerre(Player pPlayer)
	{
		m_Player = pPlayer;
		m_SpecialTime = 0f;
		ParticleEffect pe = m_Player.m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/GuerreSpecial");
		m_SpecialFx = new MercuryParticle(m_Player.m_GameStateInstance, 0, 0, pe, "GuerreSpecialFx", m_Player.GetZ(), bUseBlending: true);
		m_SpecialFx.SetAutoTrigger(bAutoTrigger: false);
		m_bStarted = false;
		m_Player.m_GameStateInstance.AddParticle(m_SpecialFx);
	}

	public override void InitSpecial()
	{
		m_SpecialTime = 100f;
		m_bStarted = false;
		m_SpecialFx.m_pe[0].MinimumTriggerPeriod = 0f;
		m_CharacterSpeed = m_Player.m_Speed;
		m_Player.m_Speed = 50f;
	}

	public override void StopSpecial()
	{
		m_Player.m_Speed = m_CharacterSpeed;
		base.StopSpecial();
	}

	public override void Update(GameTime gameTime)
	{
		m_SpecialTime -= gameTime.ElapsedGameTime.Milliseconds;
		Vector2 position = m_Player.GetPosition();
		position.Y -= 30f;
		m_SpecialFx.Trigger(position);
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
