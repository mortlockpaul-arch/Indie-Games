using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectMercury;

namespace JamSouls;

public class SoulSpawner
{
	private const int MIN_SOUL_SPAWN_PERIOD = 5000;

	private const int MAX_SOUL_SPAWN_PERIOD = 10000;

	public List<Soul> m_SoulList = new List<Soul>();

	private float m_SoulTimer = 10000f;

	private float m_SoulAppearEffectTimer;

	private int m_lastSoul = -1;

	private int m_SpawnId = -2;

	private MercuryParticle m_JamsoulSpawn;

	private AudioClip m_SoundEffect;

	private GameState m_State;

	public SoulSpawner(GameState state)
	{
		m_State = state;
		ParticleEffect particleEffect = m_State.content.Load<ParticleEffect>("Fx/Particle/JamsoulSpawn");
		m_JamsoulSpawn = new MercuryParticle(state, 0, 0, particleEffect.DeepCopy(), "Jamsouls Spawn", 1f, bUseBlending: true);
		state.AddParticle(m_JamsoulSpawn);
		m_JamsoulSpawn.SetAutoTrigger(bAutoTrigger: false);
		m_SoundEffect = new AudioClip("Soul_Spawn");
		Random random = new Random();
		for (int i = 0; i < 6; i++)
		{
			m_SoulList.Add(new Soul(m_State, random.Next(90, 200)));
		}
	}

	public void ResetSouls()
	{
		m_State.m_bAllowSoulSpawn = false;
		foreach (Soul soul in m_SoulList)
		{
			soul.Reset();
		}
	}

	public Soul GetAvailableSoul()
	{
		for (int i = 0; i < m_SoulList.Count; i++)
		{
			if (m_SoulList[i].m_bSpawned && m_SoulList[i].GetOwner() == null)
			{
				return m_SoulList[i];
			}
		}
		return null;
	}

	public void Update(GameTime gameTime)
	{
		if (!m_State.m_bAllowSoulSpawn)
		{
			return;
		}
		bool flag = false;
		if (m_SoulTimer <= 0f)
		{
			flag = true;
			m_SoulTimer = m_State.m_Randomizer.Next(5000, 10000);
		}
		else
		{
			m_SoulTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		if (m_SoulAppearEffectTimer > 0f)
		{
			m_JamsoulSpawn.Trigger(m_SoulList[m_lastSoul].GetPosition());
			m_SoulAppearEffectTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		for (int i = 0; i < 6; i++)
		{
			m_SoulList[i].Update(gameTime);
			if (m_SoulList[i].GetOwner() != null)
			{
				Player owner = m_SoulList[i].GetOwner();
				if (owner.m_Tag == 1)
				{
					m_SoulList[i].Spawn(PlayerConfig.SoulPokeX[i], PlayerConfig.SoulPokeY[i]);
				}
			}
			else if (flag && !m_SoulList[i].m_SoulBody.Active)
			{
				flag = false;
				m_SpawnId++;
				if (m_SpawnId > m_State.m_SoulSpawnPoint.Count - 1 || m_SpawnId == -1)
				{
					m_SpawnId = m_State.m_Randomizer.Next(0, m_State.m_SoulSpawnPoint.Count - 1);
				}
				m_SoulAppearEffectTimer = 2000f;
				m_lastSoul = i;
				m_SoulList[i].Appear(m_State.m_SoulSpawnPoint[m_SpawnId].X, m_State.m_SoulSpawnPoint[m_SpawnId].Y);
				m_SoulList[i].SpawnFromHeaven();
				m_SoundEffect.Play();
			}
		}
	}

	public void Draw()
	{
		for (int i = 0; i < 6; i++)
		{
			m_SoulList[i].Draw();
		}
	}
}
