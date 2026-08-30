using IMAK3Z0MB1EGAEM;
using IMAK3Z0MB1EGAEM.character;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.map;
using Microsoft.Xna.Framework;

namespace Viking_x86.zomb1es.endless;

public class EndlessUpdate
{
	public const int PHASE_ENTER = 0;

	public const int PHASE_BATTLE = 1;

	public const int PHASE_LEAVE = 2;

	public const int DOOR_TOP = 0;

	public const int DOOR_LEFT = 1;

	public const int DOOR_RIGHT = 2;

	public const int DOOR_BOTTOM = 3;

	public int x_room;

	public int y_room;

	public int x_dir;

	public int y_dir;

	private EndlessNode[,] room;

	public int phase;

	public float phaseFrame;

	public int round;

	public void Init()
	{
		phase = 1;
		phaseFrame = -5f;
		round = 0;
		NextRound();
		ScrollMan.scroll = new Vector2(1536f, 1152f) * 1.2f;
		MapMan.map.doorAlpha[0] = (MapMan.map.doorAlpha[1] = (MapMan.map.doorAlpha[2] = (MapMan.map.doorAlpha[3] = 1f)));
		bool[] doorLocked = MapMan.map.doorLocked;
		bool[] doorLocked2 = MapMan.map.doorLocked;
		bool flag;
		MapMan.map.doorLocked[2] = (flag = (MapMan.map.doorLocked[3] = true));
		bool flag2;
		doorLocked2[1] = (flag2 = flag);
		doorLocked[0] = flag2;
	}

	public void NextRound()
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				room[i, j].visited = false;
			}
		}
		round++;
	}

	public EndlessUpdate()
	{
		x_room = 1;
		y_room = 1;
		room = new EndlessNode[3, 3];
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				room[i, j] = new EndlessNode();
			}
		}
		room[0, 0].type = 7;
		room[1, 0].type = 6;
		room[2, 0].type = 4;
		room[0, 1].type = 1;
		room[1, 1].type = 0;
		room[2, 1].type = 2;
		room[0, 2].type = 3;
		room[1, 2].type = 8;
		room[2, 2].type = 5;
	}

	public void Update()
	{
		if (CamMan.endlessTransFrame > 0f)
		{
			for (int i = 0; i < CharMan.hero.Length; i++)
			{
				if (CharMan.hero[i].exists)
				{
					CharMan.hero[i].speedFrame = 0f;
					CharMan.hero[i].traj = new Vector2(x_dir, y_dir);
					CharMan.hero[i].shoot = default(Vector2);
				}
			}
		}
		if (MapMan.map.doorLocked[0] || y_room == 0)
		{
			SetCol(7, 0);
			SetCol(8, 0);
		}
		else
		{
			KillCol(7, 0);
			KillCol(8, 0);
			KillCol(7, -1);
			KillCol(8, -1);
		}
		if (MapMan.map.doorLocked[1] || x_room == 0)
		{
			SetCol(0, 5);
			SetCol(0, 6);
		}
		else
		{
			KillCol(0, 5);
			KillCol(0, 6);
			KillCol(-1, 5);
			KillCol(-1, 6);
		}
		if (MapMan.map.doorLocked[3] || y_room == 2)
		{
			SetCol(7, 11);
			SetCol(8, 11);
		}
		else
		{
			KillCol(7, 11);
			KillCol(8, 11);
			KillCol(7, 12);
			KillCol(8, 12);
		}
		if (MapMan.map.doorLocked[2] || x_room == 2)
		{
			SetCol(15, 5);
			SetCol(15, 6);
		}
		else
		{
			KillCol(15, 5);
			KillCol(15, 6);
			KillCol(16, 5);
			KillCol(16, 6);
		}
		float num = phaseFrame;
		for (int j = 0; j < 3; j++)
		{
			for (int k = 0; k < 3; k++)
			{
				if (x_room == j && y_room == k)
				{
					if (room[j, k].alpha < 1f)
					{
						room[j, k].alpha += FMan.frameTime;
					}
					if (room[j, k].alpha > 1f)
					{
						room[j, k].alpha = 1f;
					}
				}
				else
				{
					if (room[j, k].alpha > 0f)
					{
						room[j, k].alpha -= FMan.frameTime;
					}
					if (room[j, k].alpha < 0f)
					{
						room[j, k].alpha = 0f;
					}
				}
			}
		}
		phaseFrame += FMan.frameTime;
		switch (phase)
		{
		case 0:
		{
			bool[] doorLocked5 = MapMan.map.doorLocked;
			bool[] doorLocked6 = MapMan.map.doorLocked;
			bool flag6;
			MapMan.map.doorLocked[2] = (flag6 = (MapMan.map.doorLocked[3] = true));
			bool flag7;
			doorLocked6[1] = (flag7 = flag6);
			doorLocked5[0] = flag7;
			if (x_dir > 0)
			{
				MapMan.map.doorLocked[1] = false;
			}
			if (x_dir < 0)
			{
				MapMan.map.doorLocked[2] = false;
			}
			if (y_dir > 0)
			{
				MapMan.map.doorLocked[0] = false;
			}
			if (y_dir < 0)
			{
				MapMan.map.doorLocked[3] = false;
			}
			if (phaseFrame > 3f)
			{
				if (x_room == 1 && y_room == 1)
				{
					NextRound();
				}
				phase = 1;
				phaseFrame = 0f;
			}
			break;
		}
		case 1:
		{
			bool[] doorLocked3 = MapMan.map.doorLocked;
			bool[] doorLocked4 = MapMan.map.doorLocked;
			bool flag3;
			MapMan.map.doorLocked[2] = (flag3 = (MapMan.map.doorLocked[3] = true));
			bool flag4;
			doorLocked4[1] = (flag4 = flag3);
			doorLocked3[0] = flag4;
			if (phaseFrame > 30f)
			{
				bool flag5 = false;
				for (int n = 0; n < CharMan.monster.Length; n++)
				{
					if (CharMan.monster[n].exists)
					{
						flag5 = true;
						if (CharMan.monster[n].type == 4)
						{
							CharMan.monster[n].exists = false;
						}
						break;
					}
				}
				if (!flag5)
				{
					phase = 2;
					phaseFrame = 0f;
					room[x_room, y_room].visited = true;
				}
				else
				{
					phaseFrame = 29f;
				}
			}
			else if ((int)num != (int)phaseFrame && phaseFrame >= 0f)
			{
				PopMonster((int)phaseFrame);
			}
			break;
		}
		case 2:
		{
			int num2 = 0;
			for (int l = 0; l < 3; l++)
			{
				for (int m = 0; m < 3; m++)
				{
					if (room[l, m].visited)
					{
						num2++;
					}
				}
			}
			if (num2 >= 8)
			{
				bool[] doorLocked = MapMan.map.doorLocked;
				bool[] doorLocked2 = MapMan.map.doorLocked;
				bool flag;
				MapMan.map.doorLocked[2] = (flag = (MapMan.map.doorLocked[3] = true));
				bool flag2;
				doorLocked2[1] = (flag2 = flag);
				doorLocked[0] = flag2;
				if (x_room == 1 && y_room == 0)
				{
					MapMan.map.doorLocked[3] = false;
				}
				if (x_room == 0 && y_room == 1)
				{
					MapMan.map.doorLocked[2] = false;
				}
				if (x_room == 1 && y_room == 2)
				{
					MapMan.map.doorLocked[0] = false;
				}
				if (x_room == 2 && y_room == 1)
				{
					MapMan.map.doorLocked[1] = false;
				}
			}
			else
			{
				if (x_room > 0 && !room[x_room - 1, y_room].visited)
				{
					MapMan.map.doorLocked[1] = false;
				}
				if (x_room < 2 && !room[x_room + 1, y_room].visited)
				{
					MapMan.map.doorLocked[2] = false;
				}
				if (y_room > 0 && !room[x_room, y_room - 1].visited)
				{
					MapMan.map.doorLocked[0] = false;
				}
				if (y_room < 2 && !room[x_room, y_room + 1].visited)
				{
					MapMan.map.doorLocked[3] = false;
				}
			}
			if (CamMan.endlessTransFrame > 0f)
			{
				phase = 0;
				phaseFrame = 0f;
			}
			break;
		}
		}
		for (int num3 = 0; num3 < 4; num3++)
		{
			if (MapMan.map.doorLocked[num3])
			{
				if (MapMan.map.doorAlpha[num3] < 1f)
				{
					MapMan.map.doorAlpha[num3] += FMan.frameTime * 10f;
				}
				if (MapMan.map.doorAlpha[num3] > 1f)
				{
					MapMan.map.doorAlpha[num3] = 1f;
				}
			}
			if (!MapMan.map.doorLocked[num3])
			{
				if (MapMan.map.doorAlpha[num3] > 0f)
				{
					MapMan.map.doorAlpha[num3] -= FMan.frameTime * 2f;
				}
				if (MapMan.map.doorAlpha[num3] < 0f)
				{
					MapMan.map.doorAlpha[num3] = 0f;
				}
			}
		}
	}

	private void KillCol(int x, int y)
	{
		MapMan.map.col[x_room * 16 + x, y_room * 12 + y] = 0;
	}

	private void SetCol(int x, int y)
	{
		MapMan.map.col[x_room * 16 + x, y_room * 12 + y] = 1;
	}

	private void PopMonster(int i)
	{
		int num = round - 1;
		switch (room[x_room, y_room].type)
		{
		case 0:
			SpawnMgr.Pop(0, 4 + num * 2);
			if (phaseFrame > 15f)
			{
				SpawnMgr.Pop(1, 1 + num);
			}
			break;
		case 2:
			if (i % 2 == 0)
			{
				SpawnMgr.Pop(5, 1 + num);
			}
			if (i % 3 == 0)
			{
				SpawnMgr.Pop(7, 1 + num);
			}
			SpawnMgr.Pop(0, 2 + num);
			SpawnMgr.Pop(1, 1 + num);
			break;
		case 6:
			SpawnMgr.Pop(10, 4 + num * 2);
			if (phaseFrame > 15f && i % 2 == 0)
			{
				SpawnMgr.Pop(7, 1 + num);
			}
			if (phaseFrame > 15f)
			{
				SpawnMgr.Pop(10, 2 + num);
			}
			break;
		case 7:
			SpawnMgr.Pop(9, 7 + num * 2);
			SpawnMgr.Pop(0, 2 + num);
			if (phaseFrame > 15f && i % 2 == 0)
			{
				SpawnMgr.Pop(7, 1 + num);
				SpawnMgr.Pop(9, 3 + num);
			}
			break;
		case 1:
			SpawnMgr.Pop(8, 2 + num);
			SpawnMgr.Pop(0, 2 + num * 2);
			if (phaseFrame > 15f)
			{
				SpawnMgr.Pop(1, 1 + num);
			}
			break;
		case 4:
			SpawnMgr.Pop(5, 1 + num);
			SpawnMgr.Pop(0, 3 + num * 2);
			SpawnMgr.Pop(1, 1 + num);
			if (phaseFrame > 15f)
			{
				SpawnMgr.Pop(1, 1 + num);
				SpawnMgr.Pop(0, 2 + num);
			}
			break;
		case 3:
			SpawnMgr.Pop(0, 2 + num * 2);
			SpawnMgr.Pop(5, 2 + num);
			if (phaseFrame > 15f)
			{
				SpawnMgr.Pop(1, 1 + num);
			}
			break;
		case 8:
			if (phaseFrame < 15f)
			{
				SpawnMgr.Pop(2, 1 + num);
				if (i % 3 == 0)
				{
					SpawnMgr.Pop(2, 1 + num);
				}
			}
			break;
		case 5:
			SpawnMgr.Pop(8, 2 + num);
			SpawnMgr.Pop(1, 1 + num);
			SpawnMgr.Pop(0, 2 + num * 2);
			if (phaseFrame > 15f)
			{
				SpawnMgr.Pop(9, 1 + num);
			}
			break;
		default:
			SpawnMgr.Pop(0, 4 + num);
			break;
		}
	}

	internal void UpdateScroll(Vector2 scrollGoal)
	{
		if (CamMan.endlessTransFrame > 0f)
		{
			return;
		}
		int num = (int)(scrollGoal.X / 1228.8f);
		int num2 = (int)(scrollGoal.Y / 921.60004f);
		if (num != x_room || num2 != y_room)
		{
			int num3 = num - x_room;
			int num4 = num2 - y_room;
			x_room = num;
			y_room = num2;
			x_dir = num3;
			y_dir = num4;
			CamMan.endlessTransFrame = 1f;
			CamMan.endlessTransEnd = (CamMan.endlessTransStart = ScrollMan.scroll);
			if (num3 != 0)
			{
				CamMan.endlessTransEnd.X = ((float)num + 0.5f) * 64f * 16f * 1.2f;
			}
			if (num4 != 0)
			{
				CamMan.endlessTransEnd.Y = ((float)num2 + 0.5f) * 64f * 12f * 1.2f;
			}
			CamMan.endlessTransEnd.X -= (float)num3 * 115f;
			CamMan.endlessTransEnd.Y -= (float)num4 * 160f;
		}
	}

	internal EndlessNode GetRoom(int x, int y)
	{
		return room[x, y];
	}
}
