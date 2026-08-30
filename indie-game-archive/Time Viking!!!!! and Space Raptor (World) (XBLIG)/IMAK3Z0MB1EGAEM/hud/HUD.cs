using IMAK3Z0MB1EGAEM.character;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Viking_x86.character;
using Viking_x86.director;

namespace IMAK3Z0MB1EGAEM.hud;

public class HUD
{
	private const string PAUSED = "PAUSED!!1";

	public static string[] playerName = new string[4] { "", "", "", "" };

	public static bool playersInited = false;

	public static Pause pauseMenu = new Pause();

	public static int pauseOwner = -1;

	private static string[] livesStr = new string[50]
	{
		"x0", "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8", "x9",
		"x10", "x11", "x12", "x13", "x14", "x15", "x16", "x17", "x18", "x19",
		"x20", "x21", "x22", "x23", "x24", "x25", "x26", "x27", "x28", "x29",
		"x30", "x31", "x32", "x33", "x34", "x35", "x36", "x37", "x38", "x39",
		"x40", "x41", "x42", "x43", "x44", "x45", "x46", "x47", "x48", "x49"
	};

	public static void InitPlayers()
	{
		playerName = new string[4];
		for (int i = 0; i < 2; i++)
		{
			playerName[i] = "Player " + (i + 1);
			if (VikingGame.mainPlayerIdx[i] <= -1)
			{
				continue;
			}
			foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
			{
				if (signedInGamer.PlayerIndex == (PlayerIndex)VikingGame.mainPlayerIdx[i])
				{
					playerName[i] = signedInGamer.Gamertag;
				}
			}
		}
	}

	public static void Update()
	{
	}

	public static void Draw()
	{
		if (TimeMgr.CurTMgr().playMode == BaseTimeMgr.PlayMode.Paused)
		{
			SpriteTools.End();
			SpriteTools.BeginAlpha();
			SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, 1280, 720), new Color(0f, 0f, 0f, 0.6f));
			SpriteTools.End();
			SpriteTools.BeginAdditive();
			pauseMenu.Draw(new Vector2(640f, 200f));
			Menu.DrawOkCancel();
			return;
		}
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		switch (GameState.state)
		{
		case GameState.State.VikingMenu:
		case GameState.State.VikingPlaying:
		{
			for (int i = 0; i < 2; i++)
			{
				Character character = Game1.vgame.charMgr.character[i];
				if (character.team == 0 && character.exists)
				{
					float num4 = (float)playerName[i].Length * 20f;
					if (num4 < 150f)
					{
						num4 = 150f;
					}
					num = i * 720 + 220;
					if (num + num4 > 1150f)
					{
						num -= num + num4 - 1150f;
					}
					num2 = 40f;
					Text.DrawString(playerName[i], new Vector2(num - 109f, 50f + num2), 4f, idToColor(i), Text.Justify.Left);
					num3 = character.lives;
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num3 > livesStr.Length - 1)
					{
						num3 = livesStr.Length - 1;
					}
					Text.DrawString(livesStr[num3], new Vector2(num + num4, 50f + num2), 4f, idToColor(i), Text.Justify.Right);
					Text.DrawScore(character.score, new Vector2(num + 205f, 80f + num2), 3f, Color.White, Text.Justify.Right);
					if (character.GetShot() != 0)
					{
						SpriteTools.sprite.Draw(Game1.vgame.spritesTex, new Vector2(num - 82f, 77f + num2), new Rectangle((character.GetShot() - 1) * 64 + 256, 704, 64, 64), Color.White, 0f, new Vector2(64f, 0f), 0.4f, SpriteEffects.None, 1f);
						Text.DrawScore(character.GetAmmo(), new Vector2(num - 80f, 80f + num2), 3f, Color.White, Text.Justify.Left);
					}
					if (character.nameIn > 0)
					{
						Text.DrawString(character.name, new Vector2(num, 150f + num2), 8f, Color.White, Text.Justify.Center, character.nameIn - 1);
					}
				}
			}
			return;
		}
		}
		for (int j = 0; j < CharMan.hero.Length; j++)
		{
			if (CharMan.hero[j].exists)
			{
				num = (float)(j + 1) / 5f * 1280f;
				num2 = 40f;
				num = 120f;
				num2 = -25f;
				Text.DrawString(playerName[j], new Vector2(num - 109f, 50f + num2), 4f, idToColor(j), Text.Justify.Left);
				num3 = CharMan.hero[j].lives;
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num3 > livesStr.Length - 1)
				{
					num3 = livesStr.Length - 1;
				}
				Text.DrawString(livesStr[num3], new Vector2(num + 105f, 50f + num2), 4f, idToColor(j), Text.Justify.Right);
				Text.DrawScore(CharMan.hero[j].score, new Vector2(num + 105f, 80f + num2), 3f, Color.White, Text.Justify.Right);
				if (CharMan.hero[j].weapon != Hero.Weapon.Rifle)
				{
					SpriteTools.sprite.Draw(ZombieGame.spritesTex, new Vector2(num - 82f, 77f + num2), new Rectangle((int)(CharMan.hero[j].weapon - 1) * 128, 1152, 128, 128), Color.White, 0f, new Vector2(128f, 0f), 0.2f, SpriteEffects.None, 1f);
					Text.DrawScore(CharMan.hero[j].specialAmmo, new Vector2(num - 80f, 80f + num2), 3f, Color.White, Text.Justify.Left);
				}
				if (CharMan.hero[j].nameIn > 0)
				{
					Text.DrawString(CharMan.hero[j].name, new Vector2(num, 150f + num2), 8f, Color.White, Text.Justify.Center, CharMan.hero[j].nameIn - 1);
				}
			}
		}
	}

	public static Color idToColor(int id)
	{
		return idToColor(id, 1f);
	}

	public static Color idToColor(int id, float a)
	{
		return id switch
		{
			0 => new Color(0.5f, 0.5f, 1f, a), 
			1 => new Color(1f, 0.5f, 0.5f, a), 
			2 => new Color(1f, 1f, 0.5f, a), 
			3 => new Color(0.5f, 1f, 0.5f, a), 
			_ => Color.White, 
		};
	}
}
