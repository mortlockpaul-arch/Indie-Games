using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.character;

public class CharMan
{
	public static Hero[] hero;

	public static Monster[] monster;

	public static int monsterCount;

	public static bool areAlphas;

	public static void Init()
	{
		hero = new Hero[4];
		for (int i = 0; i < hero.Length; i++)
		{
			hero[i] = new Hero(i);
		}
		monster = new Monster[384];
		for (int j = 0; j < monster.Length; j++)
		{
			monster[j] = new Monster(j);
		}
	}

	public static void Update()
	{
		monsterCount = 0;
		for (int i = 0; i < hero.Length; i++)
		{
			hero[i].Update();
		}
		for (int j = 0; j < monster.Length; j++)
		{
			monster[j].Update();
			if (monster[j].exists)
			{
				monsterCount++;
			}
		}
	}

	public static void MakeMonster(Vector2 loc, int type)
	{
		MakeMonster(loc, type, midSpawn: false);
	}

	public static void MakeMonster(Vector2 loc, int type, bool midSpawn)
	{
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < monster.Length; i++)
		{
			if (!monster[i].exists)
			{
				monster[i].Init(loc, type, midSpawn);
				return;
			}
			if (monster[i].age > num2)
			{
				num = i;
				num2 = monster[i].age;
			}
		}
		monster[num].Init(loc, type, midSpawn);
	}

	public static void Draw()
	{
		areAlphas = false;
		for (int i = 0; i < hero.Length; i++)
		{
			if (hero[i].exists && hero[i].respawnFrame <= 0f)
			{
				DrawShadow(hero[i].loc, 1f);
				DrawUnderglow(hero[i].loc, i, 0.2f);
			}
		}
		for (int j = 0; j < monster.Length; j++)
		{
			if (monster[j].exists)
			{
				switch (monster[j].type)
				{
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 9:
				case 10:
					continue;
				}
				DrawShadow(monster[j].loc, (monster[j].spawnFrame > 0f) ? (1f - monster[j].spawnFrame) : 1f);
			}
		}
		for (int k = 0; k < hero.Length; k++)
		{
			hero[k].Draw();
		}
		for (int l = 0; l < monster.Length; l++)
		{
			if (monster[l].type != 10)
			{
				monster[l].Draw();
			}
			else
			{
				areAlphas = true;
			}
		}
	}

	public static void DrawAlphas()
	{
		for (int i = 0; i < monster.Length; i++)
		{
			if (monster[i].type == 10 && monster[i].exists)
			{
				monster[i].Draw();
			}
		}
	}

	private static void DrawShadow(Vector2 loc, float scale)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(576, 128, 192, 192), new Color(0f, 0f, 0f, 0.5f), 0f, new Vector2(96f, 96f), 0.4f * ScrollMan.zoom * scale, SpriteEffects.None, 1f);
	}

	public static void DrawUnderglow(Vector2 loc, int idx, float scale)
	{
		Color color = Color.White;
		switch (idx)
		{
		case 0:
			color = new Color(0.2f, 0.2f, 1f, 0.4f);
			break;
		case 1:
			color = new Color(1f, 0.2f, 0.2f, 0.4f);
			break;
		case 2:
			color = new Color(1f, 1f, 0.2f, 0.4f);
			break;
		case 3:
			color = new Color(0.2f, 1f, 0.2f, 0.4f);
			break;
		}
		for (int i = 0; i < 3; i++)
		{
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(0, 768, 256, 256), color, 0f, new Vector2(128f, 128f), (scale + (float)i * 0.04f) * ScrollMan.zoom, SpriteEffects.None, 1f);
		}
	}
}
