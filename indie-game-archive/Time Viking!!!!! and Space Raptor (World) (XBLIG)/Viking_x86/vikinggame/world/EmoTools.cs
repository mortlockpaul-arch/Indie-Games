using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86.director;

namespace Viking_x86.vikinggame.world;

public class EmoTools
{
	private struct Heart
	{
		public Vector3 loc;

		public float size;

		public int type;

		public void Update()
		{
			if (loc.Y == 0f || loc.Y > VScroll.scroll.Y + 400f)
			{
				loc.Y = VScroll.scroll.Y - 500f - Rand.GetRandomFloat(0f, 600f);
				loc.X = Game1.vgame.world.GetBase().X + Rand.GetRandomFloat(-300f, 300f);
				loc.Z = Rand.GetRandomFloat(0.3f, 0.8f);
				size = Rand.GetRandomFloat(1f, 2f);
				type = Rand.GetRandomInt(0, 2);
			}
			loc.Y += Game1.frameTime * 120f;
		}

		public void Draw()
		{
			type = (int)(loc.X + (float)TimeMgr.CurTMgr().beat) % 2;
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(new Vector2(loc.X, loc.Y), loc.Z), new Rectangle((type == 0) ? 832 : 960, 256, (type == 0) ? 128 : 64, 128), new Color(1f, 0f, 0f, 0.125f), VScroll.angle, new Vector2((type % 2 == 0) ? 60f : 32f, 64f), size * VScroll.zoom * loc.Z, SpriteEffects.None, 1f);
		}
	}

	private struct Word
	{
		public Vector3 loc;

		public void Update()
		{
			if (loc.Y == 0f || loc.Y > VScroll.scroll.Y + 400f)
			{
				loc.Y = VScroll.scroll.Y - 500f - Rand.GetRandomFloat(0f, 800f);
				loc.X = Game1.vgame.world.GetBase().X + Rand.GetRandomFloat(-400f, 400f);
				loc.Z = Rand.GetRandomFloat(0.3f, 0.8f);
			}
			loc.Y += Game1.frameTime * 120f;
		}

		public void Draw(int type)
		{
			SpriteTools.sprite.Draw(Game1.vgame.heartTex, VScroll.GetScreenLoc(new Vector2(loc.X, loc.Y), loc.Z), new Rectangle(512, type * 256 + (int)((0f - loc.Y) / 5f) % 4 * 64, 128, 64), new Color(1f, 1f, 1f, 0.45f), VScroll.angle, new Vector2(64f, 32f), 1f * VScroll.zoom * loc.Z, SpriteEffects.None, 1f);
		}
	}

	private Heart[] heart = new Heart[32];

	private Word[] word = new Word[4];

	private float exclamationRowFrame;

	private float streamFrame;

	public void Update()
	{
		exclamationRowFrame += Game1.frameTime * 0.5f;
		if (exclamationRowFrame > 1f)
		{
			exclamationRowFrame--;
		}
		streamFrame += Game1.frameTime * 20f;
		streamFrame -= VScroll.scrollDif.Y / 10f;
		if (streamFrame < 0f)
		{
			streamFrame += 512f;
		}
		if (streamFrame > 512f)
		{
			streamFrame -= 512f;
		}
		for (int i = 0; i < heart.Length; i++)
		{
			heart[i].Update();
		}
		for (int j = 0; j < word.Length; j++)
		{
			word[j].Update();
		}
	}

	public void DrawBack()
	{
		SpriteTools.sprite.Draw(Game1.vgame.grayTex, VScroll.screenSize / 2f, new Rectangle(0, 0, 480, 480), new Color(1f, 1f, 1f, 1f), VScroll.angle, new Vector2(240f, 240f), 3f, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.heartTex, VScroll.GetScreenLoc(new Vector2(Game1.vgame.world.GetBase().X, VScroll.scroll.Y), 0.2f), new Rectangle(0, 512 - (int)streamFrame, 512, 512), new Color(1f, 1f, 1f, 0.5f), VScroll.angle, new Vector2(256f, 256f), new Vector2(1f, 1.5f) * VScroll.zoom, SpriteEffects.None, 1f);
		DrawExclamationLine(-100f, 0.3f);
		DrawExclamationLine(100f, 0.4f);
		DrawExclamationLine(-300f, 0.35f);
		DrawExclamationLine(300f, 0.45f);
		DrawExclamationLine(-500f, 0.25f);
		DrawExclamationLine(500f, 0.375f);
		DrawExclamationLine(-600f, 0.42f);
		DrawExclamationLine(600f, 0.475f);
		for (int i = 0; i < heart.Length; i++)
		{
			heart[i].Draw();
		}
		for (int j = 0; j < word.Length; j++)
		{
			word[j].Draw(j % 4);
		}
		if (TimeMgr.CurTMgr().trackTime < 2.0)
		{
			SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)VScroll.screenSize.X, (int)VScroll.screenSize.Y), new Color(1f, 1f, 1f, 1f - (float)(TimeMgr.CurTMgr().trackTime / 2.0)));
		}
	}

	public void DrawEmoBlast()
	{
		float num = (float)TimeMgr.CurTMgr().pulse;
		for (num *= 2f; num > 1f; num--)
		{
		}
		float num2;
		for (num2 = (float)TimeMgr.CurTMgr().pulse * 2f; num2 > 1f; num2--)
		{
		}
		SpriteTools.sprite.Draw(Game1.vgame.grayTex, VScroll.screenSize / 2f, new Rectangle(0, 0, 480, 480), new Color(0.2f + num2 / 2f, 0.2f + num2 / 2f, 0.2f + num2 / 2f, 1f), VScroll.angle, new Vector2(240f, 240f), 3f, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.heartTex, VScroll.GetScreenLoc(new Vector2(Game1.vgame.world.GetBase().X, VScroll.scroll.Y), 0.2f), new Rectangle(0, 512 - (int)streamFrame, 512, 512), new Color(1f, 0f, 0f, 0.5f), VScroll.angle, new Vector2(256f, 256f), new Vector2(1f, 1.5f) * VScroll.zoom, SpriteEffects.None, 1f);
		for (int i = 0; i < 2; i++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.heartTex, VScroll.screenSize / 2f, new Rectangle(640, 512 + Rand.GetRandomInt(0, 4) * 128, 384, 128), new Color(1f, Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 0.75f), VScroll.angle, new Vector2(192f, 64f), VScroll.zoom * new Vector2(1.65f, (1f - num2) * 6f + 0.1f), Rand.CoinToss(0.5f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
		}
		for (int j = 0; j < 2; j++)
		{
			float num3;
			for (num3 = num + (float)j * 0.5f; num3 > 1f; num3--)
			{
			}
			int num4 = (int)(num3 * 32f) % 2;
			Rectangle value = new Rectangle(704, 0, 128, 192);
			switch ((TimeMgr.CurTMgr().quadbeat + j) % 3)
			{
			case 1:
				value.X = 832;
				value.Width = 192;
				break;
			case 2:
				value.X = 768;
				value.Y = 192;
				value.Width = 256;
				break;
			}
			float num5 = num4;
			Vector2 vector = new Vector2(1f, 1f);
			if (j == 1)
			{
				vector += new Vector2(0.5f, 0.5f);
			}
			vector.X += (float)Math.Cos(num3 * 6.28f) * 0.25f;
			vector.Y += (float)Math.Sin(num3 * 6.28f) * 0.25f;
			SpriteTools.sprite.Draw(Game1.vgame.heartTex, VScroll.screenSize / 2f, value, new Color(num5, num5, num5, 0.4f), VScroll.angle, new Vector2((float)value.Width / 2f, (float)value.Height / 2f), VScroll.zoom * vector * 2f, (j % 2 == 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
		}
	}

	public void DrawEmoRed()
	{
		float num = (float)TimeMgr.CurTMgr().pulse;
		for (num *= 2f; num > 1f; num--)
		{
		}
		float num2;
		for (num2 = (float)TimeMgr.CurTMgr().pulse; num2 > 1f; num2--)
		{
		}
		SpriteTools.sprite.Draw(Game1.vgame.grayTex, VScroll.screenSize / 2f, new Rectangle(0, 0, 480, 480), new Color(1f, 0f, 0f, 1f), VScroll.angle, new Vector2(240f, 240f), 3f, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.heartTex, VScroll.GetScreenLoc(new Vector2(Game1.vgame.world.GetBase().X, VScroll.scroll.Y), 0.2f), new Rectangle(0, 512 - (int)streamFrame, 512, 512), new Color(0f, 0f, 0f, 0.5f), VScroll.angle, new Vector2(256f, 256f), new Vector2(1f, 1.5f) * VScroll.zoom, SpriteEffects.None, 1f);
		float num3;
		for (num3 = num2 * 2f; num3 > 1f; num3--)
		{
		}
		for (int i = 0; i < 2; i++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.heartTex, VScroll.screenSize / 2f, new Rectangle(640, 512 + Rand.GetRandomInt(0, 4) * 128, 384, 128), new Color(1f, 1f, 1f, 0.35f), VScroll.angle + (Rand.CoinToss(0.5f) ? 3.14f : 0f), new Vector2(192f, 64f), VScroll.zoom * new Vector2(1.65f, (1f - num3) * 4f + 0.3f), Rand.CoinToss(0.5f) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
		}
		if (TimeMgr.CurTMgr().trackLeft < 20.0)
		{
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(VScroll.scroll + new Vector2(0f, (8f - (float)TimeMgr.CurTMgr().trackLeft) * 100f), 0.45f), new Rectangle(577, 64, 447, 192), Color.White, -1.57f + VScroll.angle, new Vector2(224f, 96f), VScroll.zoom * 1.2f, SpriteEffects.None, 1f);
		}
		if (TimeMgr.CurTMgr().trackLeft < 13.0)
		{
			SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)VScroll.screenSize.X, (int)VScroll.screenSize.Y), new Color(1f, 1f, 1f, (float)(1.0 - TimeMgr.CurTMgr().trackLeft / 13.0)));
		}
		if (TimeMgr.CurTMgr().trackLeft < 9.0)
		{
			VikingQuake.SetQuake((float)(1.0 - TimeMgr.CurTMgr().trackLeft / 9.0) * 0.65f);
		}
	}

	private void DrawExclamationLine(float x, float z)
	{
		float y = VScroll.scroll.Y;
		float num = 140f;
		float num2 = 16f * num;
		_ = num2 / 2f;
		float num3 = y + num2 / 2f;
		for (int i = 0; i < 16; i++)
		{
			Vector2 loc = new Vector2(Game1.vgame.world.GetBase().X + x, 0f);
			loc.Y = (float)i * num + exclamationRowFrame * num * 2f;
			while (loc.Y > num3)
			{
				loc.Y -= num2;
			}
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(loc, z), new Rectangle((i % 2 == 0) ? 832 : 960, 256, (i % 2 == 0) ? 128 : 64, 128), new Color(1f, 0.9f, 0.9f, 0.15f), VScroll.angle, new Vector2((i % 2 == 0) ? 60f : 32f, 64f), z * 1.4f * VScroll.zoom, SpriteEffects.None, 1f);
		}
	}
}
