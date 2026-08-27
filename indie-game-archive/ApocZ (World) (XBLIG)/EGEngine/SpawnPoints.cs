using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class SpawnPoints
{
	public static List<SpawnPointStruct> list = new List<SpawnPointStruct>();

	public static List<AttackPointStruct> AttackPointList = new List<AttackPointStruct>();

	private static int[] NumberSpawnPoints = new int[11];

	private static int[] CurrentSpawnIndices = new int[11];

	private static int MAX_RANDOM_INDICES = 24;

	private static int CurrentRandomIndice = 0;

	private static int[] RandomArray = new int[MAX_RANDOM_INDICES];

	public static void LoadContent(Model m)
	{
		Matrix[] array = new Matrix[m.Bones.Count];
		m.CopyAbsoluteBoneTransformsTo(array);
		Vector3 zero = Vector3.Zero;
		Random random = new Random(EndGameEngine.currentEleapsedTime.TotalGameTime.Seconds);
		for (int i = 0; i < MAX_RANDOM_INDICES; i++)
		{
			RandomArray[i] = random.Next(0, MAX_RANDOM_INDICES);
		}
		for (int j = 0; j < 11; j++)
		{
			NumberSpawnPoints[j] = 0;
			CurrentSpawnIndices[j] = 0;
		}
		list.Clear();
		AttackPointList.Clear();
		foreach (ModelMesh mesh in m.Meshes)
		{
			if (!mesh.Name.Contains("spawn"))
			{
				continue;
			}
			SpawnPointStruct spawnPointStruct = new SpawnPointStruct();
			spawnPointStruct.SpawnType = SpawnPointType.Undeclared;
			spawnPointStruct.SpawnRatio = 0f;
			spawnPointStruct.SpawnTimer = 0f;
			spawnPointStruct.OccupiedFlag = false;
			if (mesh.Name.Contains("box"))
			{
				NumberSpawnPoints[7]++;
				spawnPointStruct.SpawnType = SpawnPointType.Box;
				OOBB oOBB = new OOBB(MeshTools.GetPositionsFromMesh(mesh, VertexType.Basic), array[mesh.ParentBone.Index]);
				spawnPointStruct.Position = array[mesh.ParentBone.Index].Translation;
				Matrix matrix = array[mesh.ParentBone.Index];
				matrix.Translation = Vector3.Zero;
				spawnPointStruct.Direction = Vector3.Transform(oOBB.extents, matrix);
				list.Add(spawnPointStruct);
				continue;
			}
			if (mesh.Name.Contains("_attack_"))
			{
				AttackPointStruct attackPointStruct = new AttackPointStruct();
				attackPointStruct.NodeType = SpawnPointType.AttackPoint;
				attackPointStruct.Name = mesh.Name;
				attackPointStruct.Position = array[mesh.ParentBone.Index].Translation;
				attackPointStruct.Direction = array[mesh.ParentBone.Index].Right;
				attackPointStruct.Direction.Normalize();
				AttackPointList.Add(attackPointStruct);
				continue;
			}
			if (mesh.Name.Contains("bot"))
			{
				NumberSpawnPoints[6]++;
				spawnPointStruct.SpawnType = SpawnPointType.Bot;
			}
			else if (mesh.Name.Contains("teleport"))
			{
				NumberSpawnPoints[8]++;
				spawnPointStruct.SpawnType = SpawnPointType.Teleport;
			}
			else if (mesh.Name.Contains("player"))
			{
				NumberSpawnPoints[1]++;
				spawnPointStruct.SpawnType = SpawnPointType.Player;
			}
			else if (mesh.Name.Contains("team1"))
			{
				NumberSpawnPoints[2]++;
				spawnPointStruct.SpawnType = SpawnPointType.Team1;
			}
			else if (mesh.Name.Contains("team2"))
			{
				NumberSpawnPoints[3]++;
				spawnPointStruct.SpawnType = SpawnPointType.Team2;
			}
			else if (mesh.Name.Contains("deathmatch"))
			{
				NumberSpawnPoints[4]++;
				spawnPointStruct.SpawnType = SpawnPointType.Deathmatch;
			}
			else if (mesh.Name.Contains("_weapon_"))
			{
				NumberSpawnPoints[10]++;
				spawnPointStruct.SpawnType = SpawnPointType.WeaponLevitator;
			}
			spawnPointStruct.Position = array[mesh.ParentBone.Index].Translation;
			spawnPointStruct.Direction = array[mesh.ParentBone.Index].Right;
			spawnPointStruct.Direction.Normalize();
			list.Add(spawnPointStruct);
		}
		float num = 0f;
		float num2 = 1000000f;
		foreach (SpawnPointStruct item in list)
		{
			float num3 = (zero - item.Position).Length();
			if (num < num3)
			{
				num = num3;
			}
			if (num2 > num3)
			{
				num2 = num3;
			}
		}
		float num4 = num - num2;
		for (int k = 0; k < list.Count; k++)
		{
			float num5 = (zero - list[k].Position).Length();
			list[k].SpawnRatio = num5 / num4;
		}
	}

	public static void Update(GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SpawnTimer += num;
			if (list[i].OccupiedFlag)
			{
				list[i].SpawnTimer += num;
				if (list[i].SpawnTimer > 30f)
				{
					list[i].OccupiedFlag = false;
					list[i].SpawnTimer = 0f;
				}
			}
		}
	}

	public virtual void Draw()
	{
	}

	public static int GetNumberOfSpawnPoints(SpawnPointType e)
	{
		return NumberSpawnPoints[(int)e];
	}

	public static void GetNextSpawnPoint(SpawnPointType e, ref Vector3 p, ref Vector3 d)
	{
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].SpawnType != e)
			{
				continue;
			}
			if (CurrentSpawnIndices[(int)e] == num)
			{
				p = list[i].Position;
				d = list[i].Direction;
				CurrentSpawnIndices[(int)e]++;
				if (CurrentSpawnIndices[(int)e] >= NumberSpawnPoints[(int)e])
				{
					CurrentSpawnIndices[(int)e] = 0;
				}
				break;
			}
			num++;
		}
	}

	public static void SetOccupiedFlag(int index)
	{
		if (index >= 0 && index < list.Count)
		{
			list[index].OccupiedFlag = true;
			list[index].SpawnTimer = 0f;
		}
	}

	public static bool GetNextSpawnPoint(SpawnPointType e, ref Vector3 p, ref Vector3 d, int curIndex)
	{
		if (curIndex >= 0 && curIndex < list.Count && !list[curIndex].OccupiedFlag && list[curIndex].SpawnType == e)
		{
			p = list[curIndex].Position;
			d = list[curIndex].Direction;
			return true;
		}
		return false;
	}

	public static void ClearOccupiedSpawnPoint(SpawnPointType e)
	{
		float num = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[i].OccupiedFlag && list[i].SpawnType == e)
			{
				return;
			}
			if (list[i].SpawnTimer > num)
			{
				num = list[i].SpawnTimer;
			}
		}
		num -= 10f;
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].OccupiedFlag && list[j].SpawnType == e && list[j].SpawnTimer > num)
			{
				list[j].OccupiedFlag = false;
			}
		}
	}

	public static void GetRandomSpawnPoint(SpawnPointType e, ref Vector3 p, ref Vector3 d)
	{
		int num = RandomArray[CurrentRandomIndice];
		CurrentRandomIndice++;
		if (CurrentRandomIndice >= MAX_RANDOM_INDICES)
		{
			CurrentRandomIndice = 0;
		}
		while (num >= NumberSpawnPoints[(int)e])
		{
			num -= NumberSpawnPoints[(int)e] / 5;
		}
		int num2 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].SpawnType == e)
			{
				if (num == num2)
				{
					p = list[i].Position;
					d = list[i].Direction;
					break;
				}
				num2++;
			}
		}
	}

	public static void GetSpawnPointByIndex(SpawnPointType e, ref Vector3 p, ref Vector3 d, int index)
	{
		if (index >= NumberSpawnPoints[(int)e])
		{
			index = NumberSpawnPoints[(int)e] - 1;
		}
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].SpawnType == e)
			{
				if (num == index)
				{
					p = list[i].Position;
					d = list[i].Direction;
					break;
				}
				num++;
			}
		}
	}

	public static bool GetSpawnPointAtIndex(SpawnPointType e, ref Vector3 p, ref Vector3 d, int index)
	{
		if (index < 0 || index >= NumberSpawnPoints[(int)e])
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].SpawnType == e)
			{
				if (num == index)
				{
					p = list[i].Position;
					d = list[i].Direction;
					return true;
				}
				num++;
			}
		}
		return false;
	}

	public static int GetAttackClosestPoint(ref Vector3 targetPos, ref Vector3 attackPos)
	{
		int result = -1;
		float num = float.MaxValue;
		int count = AttackPointList.Count;
		for (int i = 0; i < count; i++)
		{
			if (!AttackPointList[i].OccupiedFlag)
			{
				float num2 = (AttackPointList[i].Position - targetPos).LengthSquared();
				if (num2 < num)
				{
					num = num2;
					result = i;
					attackPos = AttackPointList[i].Position;
				}
			}
		}
		return result;
	}

	public static int GetAttackPoint(ref Vector3 targetPos, float maxDisSqr, ref Vector3 attackPos)
	{
		int result = -1;
		float num = 0f;
		int count = AttackPointList.Count;
		for (int i = 0; i < count; i++)
		{
			if (!AttackPointList[i].OccupiedFlag)
			{
				float num2 = (AttackPointList[i].Position - targetPos).LengthSquared();
				if (num2 < maxDisSqr && num2 > num)
				{
					num = num2;
					result = i;
					attackPos = AttackPointList[i].Position;
				}
			}
		}
		return result;
	}

	public static void ToggleAttackPoint(int index, bool e)
	{
		AttackPointList[index].OccupiedFlag = e;
	}

	public static void GetAttackPosition(int index, ref Vector3 attackPos)
	{
		attackPos = AttackPointList[index].Position;
	}

	public static void ResetAttackPoints()
	{
		for (int i = 0; i < AttackPointList.Count; i++)
		{
			AttackPointList[i].OccupiedFlag = false;
		}
	}
}
