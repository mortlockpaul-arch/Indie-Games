using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using PlayObjects;
using Renderer;

namespace Scene;

public class SceneObjectSpawner
{
	public const float CEIL_DIST = 900f;

	private List<List<Prop>> m_EnemyPool;

	private List<Prop> m_ActiveEnemies;

	private List<Prop> m_RemoveEnemies;

	private int m_spawnPos;

	private List<ObstacleRect> m_floors;

	private List<ObstacleRect> m_ceilings;

	private Player m_player;

	private int m_iNumSpawned;

	private RoomSpawner m_roomSpawner;

	private List<AmbianceElement> m_ambiance;

	private int m_iWorldType;

	private int m_iDefaultWorld;

	private bool m_bWorldSet;

	private bool m_bInfiniteWorld;

	private int m_iLoadedIndex;

	private int m_iMaxLoad;

	private int m_iLoadedIndexNumPerType;

	private int m_iLoadedIndexType;

	private List<int> m_FillCounts;

	private int lastXPos;

	public SceneObjectSpawner(Player p, List<Prop> aciveObjs, List<ObstacleRect> floors, List<ObstacleRect> ceilings, List<AmbianceElement> ambiance)
	{
		m_ambiance = ambiance;
		m_player = p;
		m_roomSpawner = new RoomSpawner(this);
		m_iWorldType = 0;
		m_roomSpawner.SetRoomType(m_iWorldType);
		m_ActiveEnemies = aciveObjs;
		m_EnemyPool = new List<List<Prop>>();
		m_FillCounts = FillCounts();
		m_iLoadedIndex = 0;
		m_iLoadedIndexType = 0;
		m_iLoadedIndexNumPerType = 0;
		for (int i = 0; i < 171; i++)
		{
			m_EnemyPool.Add(new List<Prop>());
		}
		m_RemoveEnemies = new List<Prop>();
		m_floors = floors;
		m_ceilings = ceilings;
		Vector2 vector = new Vector2(0f, 500f);
		for (int j = 0; j < 5; j++)
		{
			m_floors.Add(new ObstacleRect(new Vector2(1000f, 200f), isCeil: false));
			m_floors[j].Position = vector;
			m_floors[j].Static = true;
			m_ceilings.Add(new ObstacleRect(new Vector2(1000f, 200f), isCeil: true));
			m_ceilings[j].Position = vector + new Vector2(0f, -1100f);
			m_ceilings[j].Static = true;
			vector += new Vector2(1000f, 0f);
		}
		m_player.CeilHeight = vector.Y - 900f;
		m_iNumSpawned = 0;
		m_iDefaultWorld = -1;
		m_bWorldSet = true;
		m_bInfiniteWorld = false;
	}

	public void UpdateLoad()
	{
		if (m_iLoadedIndexType < 171 && m_iLoadedIndexNumPerType < m_FillCounts[m_iLoadedIndexType] + 1)
		{
			m_iLoadedIndexNumPerType++;
			m_iLoadedIndex++;
			m_EnemyPool[m_iLoadedIndexType].Add(new Prop((PropType)m_iLoadedIndexType));
			m_EnemyPool[m_iLoadedIndexType].Last().ResetToLocation(new Vector2(-1000 - 1000 * m_iLoadedIndex, 0f));
			if (m_iLoadedIndexNumPerType == m_FillCounts[m_iLoadedIndexType] + 1)
			{
				m_iLoadedIndexNumPerType = 0;
				m_iLoadedIndexType++;
			}
		}
		if (m_iLoadedIndexType == 171)
		{
			m_spawnPos = (int)m_roomSpawner.GenerateRoom(0, 0f) + 200;
			SpawnAmbiance(m_spawnPos);
		}
	}

	public string PercentCompleteLoaded()
	{
		return (int)((float)m_iLoadedIndex / (float)m_iMaxLoad * 100f) + "%";
	}

	public bool IsFullyLoaded()
	{
		return m_iLoadedIndexType >= 171;
	}

	private void SpawnAmbiance(float roomWidth)
	{
		if (roomWidth < 0f)
		{
			roomWidth = m_spawnPos;
		}
		int num = m_iWorldType;
		if (m_spawnPos == 0 || (float)m_spawnPos == roomWidth)
		{
			num = m_iDefaultWorld;
		}
		float num2 = m_floors[0].Position.Y - 100f;
		switch (num)
		{
		case 0:
		case 2:
			m_ambiance.Add(new AmbianceElement(TextureContainer.GetSprite("images/bg", new Vector2(m_spawnPos, num2), -50f), 0f));
			m_ambiance.Last().Sprite.SurfaceScale = new Vector2(roomWidth, 1100f);
			m_ambiance.Last().Sprite.Origin = new Vector2(roomWidth / 2f, m_ambiance.Last().Sprite.SurfaceScale.Y / 2f);
			m_ambiance.Last().Sprite.GetSpriteImage().Width = roomWidth;
			m_ambiance.Last().Sprite.Color = m_roomSpawner.GetRoomColor();
			return;
		case 1:
		{
			m_ambiance.Add(new AmbianceElement(TextureContainer.GetSprite("images/sky", new Vector2(m_spawnPos, num2), -100f), 0f));
			m_ambiance.Last().Sprite.Color = Color.Blue;
			m_ambiance.Last().Sprite.SurfaceScale = new Vector2(roomWidth, 1300f);
			m_ambiance.Last().Sprite.Origin = new Vector2(roomWidth / 2f, m_ambiance.Last().Sprite.SurfaceScale.Y / 2f);
			m_ambiance.Last().Sprite.GetSpriteImage().Width = roomWidth;
			int num3 = 0;
			while ((float)num3 < roomWidth + 300f)
			{
				m_ambiance.Add(new AmbianceElement(TextureContainer.GetSprite("images/hill", new Vector2((float)m_spawnPos - roomWidth + (float)num3, num2), -75f + SceneRenderer.GetRand(0f, 1f)), 0f));
				m_ambiance.Last().Sprite.Color = Color.Gray;
				m_ambiance.Last().Sprite.WidthScale = SceneRenderer.GetRand(100f, 1000f);
				m_ambiance.Last().Sprite.Position += new Vector2(m_ambiance.Last().Sprite.WidthScale, 0f);
				num3 += (int)(m_ambiance.Last().Sprite.WidthScale * 0.8f);
				m_ambiance.Last().Sprite.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/hillNorm");
				m_ambiance.Last().Sprite.Origin = new Vector2(m_ambiance.Last().Sprite.WidthScale / 2f, m_ambiance.Last().Sprite.SurfaceScale.Y / 2f);
				num3++;
			}
			return;
		}
		}
		m_ambiance.Add(new AmbianceElement(TextureContainer.GetSprite("images/whitesquare", new Vector2(m_spawnPos, num2), -50f), 0f));
		m_ambiance.Last().Sprite.SurfaceScale = new Vector2(roomWidth, 1100f);
		m_ambiance.Last().Sprite.Origin = new Vector2(roomWidth / 2f, m_ambiance.Last().Sprite.SurfaceScale.Y / 2f);
		m_ambiance.Last().Sprite.GetSpriteImage().Width = roomWidth;
		m_ambiance.Last().Sprite.Color = Color.Black;
		m_ambiance.Last().Sprite.Alpha = 0.3f;
		int num4 = 0;
		while ((float)num4 < roomWidth + 300f)
		{
			float rand = SceneRenderer.GetRand(0.2f, 1f);
			m_ambiance.Add(new AmbianceElement(TextureContainer.GetSprite("images/whitesquare", new Vector2((float)m_spawnPos - roomWidth + (float)num4, SceneRenderer.GetRand(0f - num2, num2)), SceneRenderer.GetRand(0f, 1f)), rand));
			m_ambiance.Last().Sprite.Color = Color.Lime;
			m_ambiance.Last().Sprite.FlatColor = true;
			m_ambiance.Last().Sprite.Additive = true;
			m_ambiance.Last().Sprite.Alpha = SceneRenderer.GetRand(0.05f, 0.2f);
			m_ambiance.Last().Sprite.WidthScale = SceneRenderer.GetRand(20f, 100f);
			m_ambiance.Last().Sprite.Position += new Vector2(m_ambiance.Last().Sprite.WidthScale, 0f);
			num4 += (int)(m_ambiance.Last().Sprite.WidthScale * 2f);
			num4++;
		}
	}

	public void Reset()
	{
		Vector2 vector = new Vector2(0f, 500f);
		m_iWorldType = 0;
		m_roomSpawner.SetRoomType(m_iWorldType);
		lastXPos = 0;
		for (int i = 0; i < m_floors.Count; i++)
		{
			m_floors[i].Position = vector;
			m_floors[i].Static = true;
			if (m_iDefaultWorld == 1 || m_iDefaultWorld == 3)
			{
				m_ceilings[i].Position = vector + new Vector2(-10000f, -1100f);
			}
			else
			{
				m_ceilings[i].Position = vector + new Vector2(0f, -1100f);
			}
			m_ceilings[i].Static = true;
			vector += new Vector2(1000f, 0f);
		}
		for (int j = 0; j < m_ActiveEnemies.Count; j++)
		{
			m_RemoveEnemies.Add(m_ActiveEnemies[j]);
			m_ActiveEnemies[j].GetOutfit().Disable();
		}
		for (int k = 0; k < m_RemoveEnemies.Count; k++)
		{
			m_ActiveEnemies.Remove(m_RemoveEnemies[k]);
			m_EnemyPool[(int)m_RemoveEnemies[k].PropType].Add(m_RemoveEnemies[k]);
		}
		m_RemoveEnemies.Clear();
		if (m_iDefaultWorld == 0)
		{
			SceneRenderer.SetEffect(0);
			m_spawnPos = (int)m_roomSpawner.GenerateRoom(0, 0f) + 200;
		}
		else if (m_iDefaultWorld == 1)
		{
			SceneRenderer.SetEffect(0);
			m_spawnPos = (int)m_roomSpawner.GenerateParkStart(0f) + 200;
		}
		else if (m_iDefaultWorld == 2)
		{
			SceneRenderer.SetEffect(0);
			m_spawnPos = (int)m_roomSpawner.GenerateMallRoom(-1, 0f) + 200;
		}
		else if (m_iDefaultWorld == 3)
		{
			SceneRenderer.SetEffect(1);
			m_spawnPos = (int)m_roomSpawner.GenerateVirtualRoom(0f) + 200;
		}
		m_roomSpawner.Reset();
		SpawnAmbiance(m_spawnPos);
		m_bWorldSet = false;
	}

	public List<Prop> GetActiveList()
	{
		return m_ActiveEnemies;
	}

	public Prop SpawnEnemy(int i)
	{
		if (m_EnemyPool[i].Count > 0)
		{
			Prop result = m_EnemyPool[i][0];
			m_EnemyPool[i].RemoveAt(0);
			return result;
		}
		Game1.AddPropCount(i);
		return new Prop((PropType)i);
	}

	public void SetDefaultWorld(int i)
	{
		m_iDefaultWorld = i;
		m_bWorldSet = false;
	}

	public void SetInfiniteWorld(bool b)
	{
		m_bInfiniteWorld = b;
	}

	private bool SetCurrentWorldType()
	{
		int iWorldType = m_iWorldType;
		if (m_iDefaultWorld >= 0 && !m_bWorldSet)
		{
			m_iWorldType = m_iDefaultWorld;
			if (!m_bInfiniteWorld)
			{
				m_bWorldSet = true;
			}
		}
		else if (m_roomSpawner.WantsToSwitch())
		{
			m_iWorldType++;
		}
		m_roomSpawner.SetRoomType(m_iWorldType);
		return iWorldType != m_iWorldType;
	}

	public void Update(TimeTracker gameTime)
	{
		lastXPos = (int)m_player.Position.X;
		if (m_iWorldType == 1)
		{
			m_player.CamScale = true;
		}
		else
		{
			m_player.CamScale = false;
		}
		if (m_player.Position.X + 1428.5714f >= (float)m_spawnPos)
		{
			if (SetCurrentWorldType() && m_iWorldType != m_iDefaultWorld)
			{
				if (m_iWorldType < 3)
				{
					m_spawnPos = (int)m_roomSpawner.GenerateGlassPanel(m_spawnPos);
				}
				else
				{
					float num = m_spawnPos;
					m_spawnPos = (int)m_roomSpawner.GenerateGiantTV(m_spawnPos);
					SpawnAmbiance((float)m_spawnPos - num);
				}
				if (m_iWorldType == 0 || m_iWorldType == 2)
				{
					for (int i = 0; i < m_ceilings.Count; i++)
					{
						m_ceilings[i].Position = new Vector2(m_spawnPos + 500 + i * 1000, m_ceilings[i].Position.Y);
					}
				}
				else if (m_iDefaultWorld == m_iWorldType)
				{
					for (int j = 0; j < m_ceilings.Count; j++)
					{
						m_ceilings[j].Position = new Vector2(-14500 + j * 1000, m_ceilings[j].Position.Y);
					}
				}
				else
				{
					for (int k = 0; k < m_ceilings.Count; k++)
					{
						m_ceilings[k].Position = new Vector2(m_spawnPos + 500 - 5000 + k * 1000, m_ceilings[k].Position.Y);
					}
				}
			}
			float num2 = m_spawnPos;
			m_spawnPos = (int)m_roomSpawner.GenerateRandomRoom(m_spawnPos) + 200;
			float roomWidth = (float)m_spawnPos - num2;
			SpawnAmbiance(roomWidth);
		}
		else
		{
			float num3 = SceneRenderer.GetCameraPosition().X - 1400f;
			for (int l = 0; l < m_ActiveEnemies.Count; l++)
			{
				if (!(m_ActiveEnemies[l].GetOutfit().GetPhysicsObjects()[0].GetWorldCenter().X < num3))
				{
					continue;
				}
				List<PhysicalRepresentation> physicsObjects = m_ActiveEnemies[l].GetOutfit().GetPhysicsObjects();
				bool flag = true;
				for (int m = 0; m < physicsObjects.Count; m++)
				{
					if (physicsObjects[m].Enabled)
					{
						flag = false;
					}
				}
				if (flag)
				{
					m_RemoveEnemies.Add(m_ActiveEnemies[l]);
				}
			}
			for (int n = 0; n < m_RemoveEnemies.Count; n++)
			{
				m_ActiveEnemies.Remove(m_RemoveEnemies[n]);
				m_EnemyPool[(int)m_RemoveEnemies[n].PropType].Add(m_RemoveEnemies[n]);
			}
			m_RemoveEnemies.Clear();
		}
		if (m_player.Position.X > m_floors[m_floors.Count / 2].Position.X)
		{
			ObstacleRect obstacleRect = m_floors[0];
			m_floors.RemoveAt(0);
			obstacleRect.Position = m_floors.Last().Position + new Vector2(1000f, 0f);
			m_floors.Add(obstacleRect);
		}
		if (m_player.Position.X > m_ceilings[m_ceilings.Count / 2].Position.X && (m_iWorldType == 0 || m_iWorldType == 2) && ((m_iDefaultWorld != 1 && m_iDefaultWorld != 3) || m_spawnPos > 4000))
		{
			ObstacleRect obstacleRect2 = m_ceilings[0];
			m_ceilings.RemoveAt(0);
			obstacleRect2.Position = m_ceilings.Last().Position + new Vector2(1000f, 0f);
			m_ceilings.Add(obstacleRect2);
		}
	}

	public int GetWorldType()
	{
		return m_iWorldType;
	}

	public bool IsWorldInf()
	{
		return m_bInfiniteWorld;
	}

	public List<int> FillCounts()
	{
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(2);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(3);
		list.Add(12);
		list.Add(11);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(2);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(5);
		list.Add(2);
		list.Add(0);
		list.Add(3);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list[59]++;
		list[117]++;
		list[43]++;
		list[43]++;
		list[21]++;
		list[61]++;
		list[61]++;
		list[61]++;
		m_iMaxLoad = 0;
		for (int i = 0; i < list.Count; i++)
		{
			m_iMaxLoad += 1 + list[i];
		}
		return list;
	}
}
