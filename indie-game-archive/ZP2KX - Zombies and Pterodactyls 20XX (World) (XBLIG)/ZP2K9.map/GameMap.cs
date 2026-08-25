using System;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using yMapEdit.map;
using yMapEdit.segdef;
using Yuki_Win;
using ZP2K9.ai;
using ZP2K9.characters;
using ZP2K9.debug;
using ZP2K9.menu;

namespace ZP2K9.map;

public class GameMap
{
	public Entity[] entity = new Entity[256];

	public int entityCount;

	public Map map;

	public SpawnMgr spawnMgr;

	public int[,] node;

	public int[,] mapEntity;

	public MapWater water;

	private float frame;

	private float crateFrame;

	public Vector2 redFlagHome;

	public Vector2 blueFlagHome;

	public Vector2 hill;

	public string imNotInsaneString;

	public float tR = 1f;

	public float tG = 1f;

	public float tB = 1f;

	public float bR = 1f;

	public float bG = 1f;

	public float bB = 1f;

	public GameMap(SegDefManager segDefMgr)
	{
		entity = new Entity[256];
		for (int i = 0; i < entity.Length; i++)
		{
			entity[i] = new Entity();
		}
		map = new Map(segDefMgr);
		spawnMgr = new SpawnMgr(this);
		node = new int[256, 256];
		mapEntity = new int[256, 256];
		water = new MapWater();
		Reset();
	}

	public bool IsColFull(int x, int y)
	{
		if (map.collision[x, y] == 1)
		{
			return true;
		}
		return false;
	}

	public bool IsColRise(int x, int y)
	{
		if (map.collision[x, y] == 2 || map.collision[x, y] == 6)
		{
			return true;
		}
		return false;
	}

	public bool IsColFall(int x, int y)
	{
		if (map.collision[x, y] == 3 || map.collision[x, y] == 5)
		{
			return true;
		}
		return false;
	}

	public bool IsColNone(int x, int y)
	{
		if (map.collision[x, y] == 0 || map.collision[x, y] == 4)
		{
			return true;
		}
		return false;
	}

	public float GetMinY(Vector2 v)
	{
		int num = (int)(v.X / 64f);
		int num2 = (int)(v.Y / 32f);
		float num3 = (v.X - (float)num * 64f) / 64f;
		float num4 = (v.Y - (float)num2 * 32f) / 32f;
		if (num >= 0 && num2 > 0 && num < 256 && num2 < 256 && map.collision[num, num2] == 1 && (map.collision[num, num2 - 1] == 2 || map.collision[num, num2 - 1] == 3) && num4 < 10f)
		{
			num2--;
			num4++;
		}
		float num5 = num4;
		if (num >= 0 && num2 >= 0 && num < 256 && num2 < 256)
		{
			switch (map.collision[num, num2])
			{
			case 1:
				num5 = 0f;
				break;
			case 2:
				if (num4 >= 1f - num3)
				{
					num5 = 1f - num3;
				}
				break;
			case 3:
				if (num4 >= num3)
				{
					num5 = num3;
				}
				break;
			}
		}
		return (float)num2 * 32f + num5 * 32f;
	}

	private void Reset()
	{
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				node[i, j] = -1;
			}
		}
		for (int k = 0; k < 256; k++)
		{
			for (int l = 0; l < 256; l++)
			{
				mapEntity[k, l] = -1;
			}
		}
		entity = new Entity[256];
		for (int m = 0; m < entity.Length; m++)
		{
			entity[m] = new Entity();
		}
		if (Game1.pMan != null)
		{
			for (int n = 0; n < Game1.pMan.particle.Length; n++)
			{
				Game1.pMan.particle[n].exists = false;
			}
		}
	}

	internal void DrawEntities(SpriteBatch sprite, Texture2D spritesTex)
	{
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.gameType == 4)
		{
			int playerOne = Game1.netSession.GetPlayerOne();
			if (playerOne > -1 && Game1.character[playerOne] != null && Game1.character[playerOne].team == 1)
			{
				return;
			}
		}
		Color val = default(Color);
		for (int i = 0; i < entityCount; i++)
		{
			if (!entity[i].exists)
			{
				continue;
			}
			int x = entity[i].x;
			int y = entity[i].y;
			switch (entity[i].GetAdjustedType())
			{
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
			case 47:
			case 48:
			case 49:
				if (entity[i].respawn < 0.5f)
				{
					float num = 1f;
					if (entity[i].respawn > 0f)
					{
						num = (0.5f - entity[i].respawn) * 2f;
					}
					int num2 = entity[i].GetAdjustedType() - 1;
					sprite.Draw(Game1.spritesTex, Scroll.GetLoc(new Vector2((float)x * 64f + 32f, (float)y * 32f - 10f * (float)Math.Cos(2.0 * (double)(entity[i].frame + entity[i].respawn)))), (Rectangle?)new Rectangle(num2 % 16 * 64, 320 + num2 / 16 * 64, 64, 64), Color.White, (float)Math.Cos((double)(entity[i].frame + entity[i].respawn) * 4.0) * 0.3f - 0.7f, new Vector2(32f, 32f), num * Scroll.zoom, (SpriteEffects)0, 1f);
				}
				break;
			case 4:
			case 5:
			{
				bool flag = false;
				if (GameState.gameType == 2)
				{
					if (entity[i].type == 4 && Game1.netSession.blueFlagState == 200)
					{
						flag = true;
					}
					if (entity[i].type == 5 && Game1.netSession.redFlagState == 200)
					{
						flag = true;
					}
					if (flag)
					{
						sprite.Draw(Game1.spritesTex, Scroll.GetLoc(new Vector2((float)x * 64f + 32f, (float)y * 32f - 10f - 10f * (float)Math.Cos(2.0 * (double)entity[i].frame))), (Rectangle?)new Rectangle(448, 0, 96, 96), (entity[i].type == 4) ? new Color(new Vector4(0.5f, 0.5f, 1f, 1f)) : new Color(new Vector4(1f, 0.5f, 0.5f, 1f)), (float)Math.Cos((double)entity[i].frame * 4.0) * 0.2f, new Vector2(48f, 48f), 0.85f * Scroll.zoom, (SpriteEffects)0, 1f);
					}
				}
				break;
			}
			case 6:
				if (GameState.gameType == 3)
				{
					((Color)(ref val))._002Ector(new Vector4(1f, 1f, 0f, 0.3f));
					switch (Game1.netSession.hillState)
					{
					case 2:
						((Color)(ref val))._002Ector(new Vector4(1f, 0.2f, 0.2f, 0.3f));
						break;
					case 1:
						((Color)(ref val))._002Ector(new Vector4(0.2f, 0.5f, 1f, 0.3f));
						break;
					}
					sprite.Draw(Game1.spritesTex, Scroll.GetLoc(new Vector2((float)x * 64f + 32f, (float)y * 32f - 10f)), (Rectangle?)new Rectangle(64, 0, 64, 64), val, frame, new Vector2(32f, 32f), 5f, (SpriteEffects)0, 1f);
					sprite.Draw(Game1.spritesTex, Scroll.GetLoc(new Vector2((float)x * 64f + 32f, (float)y * 32f - 10f)), (Rectangle?)new Rectangle(64, 0, 64, 64), val, (0f - frame) * 2f, new Vector2(32f, 32f), 3f, (SpriteEffects)0, 1f);
				}
				break;
			}
		}
	}

	internal void Read(BinaryReader binaryReader)
	{
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		Game1.hud.scoreBoard.SetMapName();
		map.Read(binaryReader);
		binaryReader.Close();
		map.CalculateSegRects();
		Reset();
		entityCount = 0;
		for (int i = 0; i < map.special.Length; i++)
		{
			if (map.special[i].exists)
			{
				int x = map.special[i].x;
				int y = map.special[i].y;
				mapEntity[x, y] = i;
				entity[i].Init(x, y, (byte)(map.special[i].type + 1));
				entityCount = i + 1;
				switch ((int)map.special[i].type)
				{
				case 4:
					redFlagHome = new Vector2((float)x * 64f + 32f, (float)y * 32f + 16f);
					break;
				case 3:
					blueFlagHome = new Vector2((float)x * 64f + 32f, (float)y * 32f + 16f);
					break;
				case 5:
					hill = new Vector2((float)x * 64f + 32f, (float)y * 32f + 16f);
					break;
				}
			}
		}
		for (int j = 0; j < 256; j++)
		{
			for (int k = 0; k < 256; k++)
			{
				water.water[j, k] = false;
				switch (map.collision[j, k])
				{
				case 4:
					map.collision[j, k] = 0;
					water.water[j, k] = true;
					break;
				case 6:
					map.collision[j, k] = 2;
					water.water[j, k] = true;
					break;
				case 5:
					map.collision[j, k] = 3;
					water.water[j, k] = true;
					break;
				}
			}
		}
		water.waterLevel = 140;
		Game1.nodeMgr = new NodeMgr();
		Game1.nodeMgr.Refresh(this);
		ReadMapScript(map);
		Game1.hud.scoreBoard.Reset();
	}

	private void ReadMapScript(Map map)
	{
		map.bg = 0;
		map.bgR = 1f;
		map.bgG = 1f;
		map.bgB = 1f;
		for (int i = 0; i < map.script.Length; i++)
		{
			if (map.script[i] == null)
			{
				continue;
			}
			string[] array = map.script[i].Split(' ');
			switch (array[0])
			{
			case "waterlev":
				water.waterLevel = Convert.ToInt32(array[1]);
				break;
			case "back":
				try
				{
					map.bg = Convert.ToInt32(array[1]);
				}
				catch
				{
				}
				break;
			case "color":
				if (array[6].Length > 0)
				{
					try
					{
						tR = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
						tG = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
						tB = float.Parse(array[3], CultureInfo.InvariantCulture.NumberFormat);
						bR = float.Parse(array[4], CultureInfo.InvariantCulture.NumberFormat);
						bG = float.Parse(array[5], CultureInfo.InvariantCulture.NumberFormat);
						bB = float.Parse(array[6], CultureInfo.InvariantCulture.NumberFormat);
					}
					catch
					{
					}
				}
				break;
			case "bgcolor":
				try
				{
					map.bgR = float.Parse(array[1], CultureInfo.InvariantCulture.NumberFormat);
					map.bgG = float.Parse(array[2], CultureInfo.InvariantCulture.NumberFormat);
					map.bgB = float.Parse(array[3], CultureInfo.InvariantCulture.NumberFormat);
				}
				catch
				{
				}
				break;
			}
		}
	}

	public void GetSpawn(int team, Character c)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (c == null)
		{
			return;
		}
		if (team == 0)
		{
			switch (GameState.gameType)
			{
			default:
				team = 1 + c.team;
				break;
			case 0:
			case 1:
			case 4:
				break;
			}
		}
		int spawn = spawnMgr.GetSpawn(team);
		if (spawn > -1)
		{
			c.loc = new Vector2((float)entity[spawn].x * 64f + 32f, (float)entity[spawn].y * 32f + 16f);
			c.lastNode = entity[spawn].node;
			c.drawVec = c.loc;
		}
		else
		{
			c.loc = new Vector2(8192f, 500f);
			c.drawVec = c.loc;
			c.lastNode = -1;
		}
		c.Reset();
	}

	public byte GetCol(Vector2 t)
	{
		int num = (int)(t.X / 64f);
		int num2 = (int)(t.Y / 32f);
		if (num >= 0 && num2 >= 0 && num < 256 && num2 < 256)
		{
			return map.collision[num, num2];
		}
		return 0;
	}

	public bool GetIsCol(Vector2 t)
	{
		int num = (int)(t.X / 64f);
		int num2 = (int)(t.Y / 32f);
		if (num >= 0 && num2 >= 0 && num < 256 && num2 < 256)
		{
			switch (map.collision[num, num2])
			{
			case 1:
				return true;
			case 2:
			case 3:
			{
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(t.X - (float)num * 64f, t.Y - (float)num2 * 32f);
				switch (map.collision[num, num2])
				{
				case 2:
					if (val.Y > (64f - val.X) / 2f)
					{
						return true;
					}
					return false;
				case 3:
					if (val.Y > val.X / 2f)
					{
						return true;
					}
					return false;
				}
				break;
			}
			case 0:
				return false;
			}
		}
		return true;
	}

	public int GetWeapEntity()
	{
		if (entityCount <= 0)
		{
			return -1;
		}
		int randomInt = Rand.GetRandomInt(0, entityCount);
		if (entity[randomInt].type >= 17)
		{
			return randomInt;
		}
		for (int i = 0; i < entityCount; i++)
		{
			_ = (randomInt + i) % entityCount;
			if (entity[randomInt].type >= 17)
			{
				return randomInt;
			}
		}
		return -1;
	}

	public void Update()
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		frame += Game1.frameTime;
		if (frame > 6.28f)
		{
			frame -= 6.28f;
		}
		for (int i = 0; i < entityCount; i++)
		{
			if (entity[i].exists)
			{
				entity[i].Update();
			}
		}
		map.Update();
		if (!Mutators.GetCrates(Game1.netSession.mutator))
		{
			return;
		}
		crateFrame -= Game1.frameTime;
		if (!(crateFrame < 0f))
		{
			return;
		}
		crateFrame = 15f;
		int randomInt = Rand.GetRandomInt(Game1.nodeMgr.xMin, Game1.nodeMgr.xMax + 1);
		int num = Game1.nodeMgr.yMin - 1;
		if (num < 0)
		{
			num = 0;
		}
		bool flag = false;
		for (int j = num; j < Game1.nodeMgr.yMax; j++)
		{
			if (map.collision[randomInt, j] != 0)
			{
				if (map.collision[randomInt, j] == 8)
				{
					flag = false;
					break;
				}
				j--;
				flag = mapEntity[randomInt, j] < 0;
				break;
			}
		}
		if (flag)
		{
			Game1.pMan.AddParticle(43, new Vector2((float)randomInt * 64f + 32f, (float)num * 32f - 500f), default(Vector2), 0f, 0, 0);
		}
		else
		{
			crateFrame = 1f;
		}
	}

	public void Draw(SpriteBatch sprite, int s, int e, Texture2D nullTex, Texture2D[] mapsTex, Texture2D[] bgTex, float scale)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		ScrollManager.scroll = Scroll.scroll;
		ScrollManager.zoom = Scroll.zoom;
		map.UpdateLayerZoom();
		if (s == 3)
		{
			map.DrawWater(sprite, mapsTex, nullTex, water);
		}
		map.Draw(sprite, s, e, nullTex, mapsTex, bgTex, 1f, scale);
		Vector2 realLoc = ScrollManager.GetRealLoc(default(Vector2), 1f);
		Vector2 realLoc2 = ScrollManager.GetRealLoc(new Vector2(1280f, 720f), 1f);
		if (!DebugManager.showNodeIndices || e != 5)
		{
			return;
		}
		for (int i = (int)(realLoc.X / 64f); (float)i < realLoc2.X / 64f; i++)
		{
			for (int j = (int)(realLoc.Y * 2f / 64f); (float)j < realLoc2.Y * 2f / 64f; j++)
			{
				if (i >= 0 && j > 0 && i < 256 && j < 256 && node[i, j] > -1)
				{
					Game1.text.size = 1f;
					Game1.text.color = new Color(new Vector4(1f, 1f, 1f, 0.8f));
					Game1.text.DrawString(Scroll.GetLoc(new Vector2((float)i * 64f, (float)j * 64f * 0.5f) + new Vector2(32f, 16f)), node[i, j].ToString(), 1, -1f, Game1.impact, sprite);
					for (int k = 0; k < Game1.nodeMgr.node[node[i, j]].neighbors; k++)
					{
						int idx = Game1.nodeMgr.node[node[i, j]].neighbor[k].idx;
						int type = Game1.nodeMgr.node[node[i, j]].neighbor[k].type;
						Game1.text.DrawString(Scroll.GetLoc(new Vector2((float)i * 64f, (float)j * 64f * 0.5f) + new Vector2(32f, 16f - (float)(k + 1) * 16f)), idx + "-" + type, 1, -1f, Game1.impact, sprite);
						DrawArc(sprite, new Vector2((float)i * 64f, (float)j * 64f * 0.5f) + new Vector2(32f, 16f), new Vector2((float)Game1.nodeMgr.node[idx].x * 64f, (float)Game1.nodeMgr.node[idx].y * 64f * 0.5f) + new Vector2(32f, 16f));
					}
				}
			}
		}
	}

	public void DrawAIPaths(Character[] c, SpriteBatch sprite)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i] != null && c[i].ai != null && c[i].ai.trailNode > -1 && Game1.nodeMgr.node[c[i].ai.trail.trail[c[i].ai.trailNode]] != null)
			{
				try
				{
					DrawArc(sprite, c[i].loc - new Vector2(0f, 50f), new Vector2((float)Game1.nodeMgr.node[c[i].ai.trail.trail[c[i].ai.trailNode]].x * 64f + 32f, (float)Game1.nodeMgr.node[c[i].ai.trail.trail[c[i].ai.trailNode]].y * 32f));
				}
				catch
				{
				}
			}
		}
	}

	public void DrawArc(SpriteBatch sprite, Vector2 s, Vector2 e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		Vector2 loc = Scroll.GetLoc(s);
		if (loc.X < 0f || loc.Y < 0f || loc.X > 1280f || loc.Y > 720f)
		{
			loc = Scroll.GetLoc(e);
			if (loc.X < 0f || loc.Y < 0f || loc.X > 1280f || loc.Y > 720f)
			{
				return;
			}
		}
		Vector2 val = e - s;
		int num = (int)((Vector2)(ref val)).Length() / 30;
		if (num < 9)
		{
			num = 9;
		}
		float num2 = 0f;
		if (frame < 0f)
		{
			frame = 0f;
		}
		float num3;
		for (num3 = frame * 4f; num3 > 6.28f; num3 -= 6.28f)
		{
		}
		num2 += num3 / 6.28f / (float)num;
		Vector2 val2 = (e - s) * num2 + s;
		bool flag = e.X > s.X;
		if (flag)
		{
			val2.Y -= (float)Math.Sin(num2 * 3.14f) * 60f;
		}
		else
		{
			val2.Y -= (float)Math.Sin(num2 * 3.14f) * 40f;
		}
		for (int i = 0; i < num; i++)
		{
			float num4 = (float)(i + 1) / (float)num;
			num4 += num3 / 6.28f / (float)num;
			Vector2 val3 = (e - s) * num4 + s;
			if (flag)
			{
				val3.Y -= (float)Math.Sin(num4 * 3.14f) * 60f;
			}
			else
			{
				val3.Y -= (float)Math.Sin(num4 * 3.14f) * 40f;
			}
			float num5 = 1f;
			if (num2 < 0.1f)
			{
				num5 = num2 * 10f;
			}
			if (num4 > 0.9f)
			{
				num5 = (1f - num4) * 10f;
			}
			Texture2D spritesTex = Game1.spritesTex;
			Vector2 loc2 = Scroll.GetLoc((val2 + val3) / 2f);
			Rectangle? val4 = new Rectangle(64, 0, 64, 64);
			Color val5 = (flag ? new Color(new Vector4(1f, 0.5f, 1f, num5 * 0.95f)) : new Color(new Vector4(0.5f, 1f, 0.5f, num5 * 0.95f)));
			float angle = Trig.GetAngle(val2, val3);
			Vector2 val6 = new Vector2(32f, 32f);
			Vector2 val7 = val3 - val2;
			sprite.Draw(spritesTex, loc2, val4, val5, angle, val6, new Vector2(((Vector2)(ref val7)).Length() * 0.4f, 4f) * 0.05f * Scroll.zoom, (SpriteEffects)0, 1f);
			val2 = val3;
			num2 = num4;
		}
	}
}
