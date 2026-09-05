using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using PlayObjects;
using Renderer;

namespace Scene;

public class SpawnStrategy
{
	private List<float> m_weights;

	private List<int> m_maxSpawn;

	private List<int> m_types;

	private List<int> m_spawnsRemaining;

	private List<float> m_preSpace;

	private List<float> m_postSpace;

	private SceneObjectSpawner m_spawner;

	public int NumLeft
	{
		get
		{
			int num = 0;
			for (int i = 0; i < m_spawnsRemaining.Count; i++)
			{
				num += m_spawnsRemaining[i];
			}
			return num;
		}
	}

	public SpawnStrategy(SceneObjectSpawner spawner)
	{
		m_spawner = spawner;
		m_weights = new List<float>();
		m_maxSpawn = new List<int>();
		m_types = new List<int>();
		m_spawnsRemaining = new List<int>();
		m_preSpace = new List<float>();
		m_postSpace = new List<float>();
	}

	public void AddType(PropType type, int maxSpawn, float weight, float preSpace, float postSpace)
	{
		m_weights.Add(weight);
		m_maxSpawn.Add(maxSpawn);
		m_types.Add((int)type);
		m_spawnsRemaining.Add(maxSpawn);
		m_preSpace.Add(preSpace);
		m_postSpace.Add(postSpace);
	}

	public void Reset()
	{
		for (int i = 0; i < m_maxSpawn.Count; i++)
		{
			m_spawnsRemaining[i] = m_maxSpawn[i];
		}
	}

	public float SpawnFlat(float startPos)
	{
		float num = 0f;
		for (int i = 0; i < m_types.Count; i++)
		{
			num += m_preSpace[i];
			num += SpawnType(m_types[i], startPos + num) + m_postSpace[i];
		}
		return num;
	}

	public float SpawnEnemies(int count, float startPos)
	{
		float num = 0f;
		for (int i = 0; i < count; i++)
		{
			float num2 = 0f;
			for (int j = 0; j < m_spawnsRemaining.Count; j++)
			{
				if (m_spawnsRemaining[j] > 0)
				{
					num2 += m_weights[j];
				}
			}
			float rand = SceneRenderer.GetRand(0f, num2);
			float num3 = 0f;
			for (int k = 0; k < m_spawnsRemaining.Count; k++)
			{
				if (m_spawnsRemaining[k] > 0)
				{
					num3 += m_weights[k];
					if (rand <= num3)
					{
						m_spawnsRemaining[k]--;
						num += m_preSpace[k];
						num += SpawnType(m_types[k], startPos + num) + m_postSpace[k];
						break;
					}
				}
			}
		}
		return num;
	}

	public float SpawnType(int type, float position)
	{
		Prop prop = m_spawner.SpawnEnemy(type);
		prop.GetOutfit().CollisionCategory = PhysicsObjectManager.WallCollisionGroup();
		float result = prop.GetOutfit().MaxPos() - prop.GetOutfit().MinPos();
		prop.ResetToLocation(new Vector2(position - prop.GetOutfit().MinPos(), 400f));
		m_spawner.GetActiveList().Add(prop);
		return result;
	}

	public void ChangeWeight(int type, float weight)
	{
		m_weights[m_types.IndexOf(type)] = weight;
	}
}
