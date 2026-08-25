using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

internal class ScenaricBonusSpawner
{
	private const int MIN_DURATION = 5000;

	private const int MAX_DURATION = 8000;

	private const int DURATION = 3000;

	private GameState m_State;

	private WhiteSugar m_Bonus;

	private float m_TimeToSpawn;

	private float m_Duration;

	public bool m_Enable;

	public ScenaricBonusSpawner(GameState state, SpriteBatch batch)
	{
		m_Bonus = new WhiteSugar(state, batch);
		m_State = state;
	}

	public void Update(GameTime gametime)
	{
		if (!m_Enable)
		{
			return;
		}
		if (m_Bonus.IsSpawned())
		{
			if (m_Duration <= 0f)
			{
				m_State.PowerUpSpawnList.Add(m_Bonus.m_Position);
				m_Bonus.StopBonus();
				m_State.m_BonusSpawnEffect.Update(gametime);
				m_State.m_BonusSpawnEffect.Trigger(m_Bonus.m_Position);
			}
			else
			{
				m_Duration -= gametime.ElapsedGameTime.Milliseconds;
			}
			if (m_Bonus.Update())
			{
				m_State.m_BonusSpawnEffect.Update(gametime);
				m_State.m_BonusSpawnEffect.Trigger(m_Bonus.m_Position);
			}
		}
		else if (m_TimeToSpawn <= 0f)
		{
			if (m_State.PowerUpSpawnList.Count > 0)
			{
				Random random = new Random();
				int index = random.Next(0, m_State.PowerUpSpawnList.Count - 1);
				m_Bonus.Spawn(m_State.PowerUpSpawnList[index]);
				m_TimeToSpawn = random.Next(5000, 8000);
				m_Duration = 3000f;
				m_State.m_BonusSpawnEffect.Update(gametime);
				m_State.m_BonusSpawnEffect.Trigger(m_State.PowerUpSpawnList[index]);
				m_State.PowerUpSpawnList.RemoveAt(index);
			}
		}
		else
		{
			m_TimeToSpawn -= gametime.ElapsedGameTime.Milliseconds;
		}
	}

	public void Draw()
	{
		if (m_Enable)
		{
			m_Bonus.DrawBonus();
		}
	}
}
