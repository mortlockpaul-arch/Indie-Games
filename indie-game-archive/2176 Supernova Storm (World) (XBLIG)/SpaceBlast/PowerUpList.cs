using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace SpaceBlast;

internal class PowerUpList
{
	private List<PowerUp> m_PowerUps;

	public PowerUpList()
	{
		m_PowerUps = new List<PowerUp>();
	}

	public void LoadLevel(XmlNodeList xmlPowerUps)
	{
		for (int i = 0; i < xmlPowerUps.Count; i++)
		{
			XmlNode node = xmlPowerUps.Item(i);
			m_PowerUps.Add(new PowerUp(node, i));
		}
	}

	public void Update()
	{
		foreach (PowerUp powerUp in m_PowerUps)
		{
			powerUp.Update();
		}
	}

	public void Draw()
	{
		foreach (PowerUp powerUp in m_PowerUps)
		{
			powerUp.Draw();
		}
	}

	public BoundingSphere GetBoundingSphere(int PowerUpNum)
	{
		return m_PowerUps[PowerUpNum].GetBoundingSphere();
	}

	public void PlayerCollisionTest(LocalPlayer player)
	{
		BoundingSphere boundingSphere = player.TheShip.GetBoundingSphere();
		foreach (PowerUp powerUp in m_PowerUps)
		{
			if (powerUp.IsActive)
			{
				powerUp.PlayerCollisionTest(player, boundingSphere);
			}
		}
	}

	public int GetPowerUpCount()
	{
		int num = 0;
		foreach (PowerUp powerUp in m_PowerUps)
		{
			if (powerUp.IsActive)
			{
				num++;
			}
		}
		return num;
	}

	public PowerUp GetPowerUp(int index)
	{
		return m_PowerUps[index];
	}

	public float FindNearestPowerup(Player player, out PowerUp powerup)
	{
		float result = float.MaxValue;
		PowerUp powerUp = null;
		powerup = powerUp;
		return result;
	}

	public void FindAllPowerupsInRange(ref Vector2 position, float maxDistance, out List<PowerUp> powerups)
	{
		powerups = new List<PowerUp>();
		foreach (PowerUp powerUp in m_PowerUps)
		{
			if (powerUp.IsActive)
			{
				float num = (new Vector3(position, 0f) - powerUp.Position).Length();
				if (num <= maxDistance)
				{
					powerups.Add(powerUp);
				}
			}
		}
	}
}
