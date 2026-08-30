using System;
using IMAK3Z0MB1EGAEM.audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86.director;

namespace Viking_x86.vikinggame.character;

public class Moon
{
	public const float HP_MAX = 1500f;

	private const float DIST_SQR = 4356578f / (float)Math.PI;

	public const float VALID_HEIGHT = 300f;

	public Vector2 loc;

	public float hp;

	private float dyingFrame;

	private float hitFrame;

	public bool active;

	public float transFrame;

	public bool CheckHit(Vector2 v, int owner)
	{
		if (hp < 0f)
		{
			return false;
		}
		double time = TimeMgr.CurTMgr().time;
		double num = 857.0;
		double num2 = num - time;
		num2 *= 9.0;
		bool flag = (v - loc).LengthSquared() < 1331741.8f;
		if (flag)
		{
			if (hitFrame <= 0f)
			{
				hitFrame = 0.1f;
			}
			if (hp > (float)num2)
			{
				hp--;
			}
			Game1.vgame.charMgr.character[owner].score += 50L;
			Game1.vgame.pMgr.AddParticle(19, v, default(Vector2), 0f, 50, 0);
			if (hp < 0f)
			{
				Sound.Play("bomb");
				for (int i = 20; i < 40; i++)
				{
					VikingQuake.SetQuake(1f);
					Game1.vgame.pMgr.AddParticle(2, loc + Rand.GetRandomVec2(-40f, 40f, -20f, 20f) + new Vector2(0f, (float)i * 30f), default(Vector2), 2f, 0, 0);
				}
				for (int j = 0; j < 2; j++)
				{
					if (Game1.vgame.charMgr.character[j].exists)
					{
						Game1.vgame.charMgr.character[j].SetShield(2);
					}
				}
			}
		}
		return flag;
	}

	public float GetDif()
	{
		float num = 0f - GetMin();
		float height = Game1.vgame.world.height;
		return height - num;
	}

	public void Init()
	{
		dyingFrame = 0f;
		hp = 1500f;
		loc = Game1.vgame.world.risingBaseVec + new Vector2(0f, -2500f);
		active = true;
		transFrame = 0f;
	}

	public float GetMin()
	{
		return loc.Y + 1177.6f + 300f;
	}

	public void Update()
	{
		if (GetDif() >= 500f && transFrame < 1f)
		{
			transFrame += Game1.frameTime * 0.1f;
		}
		if (hp < 0f)
		{
			float num = dyingFrame;
			dyingFrame += Game1.frameTime;
			if ((float)(int)(num * 30f) != (float)(int)dyingFrame * 30f)
			{
				Game1.vgame.pMgr.AddParticle(2, loc + Rand.GetRandomVec2(-40f, 40f, -20f, 20f) + new Vector2(Rand.GetRandomFloat(-500f, 500f), Rand.GetRandomFloat(30f, 38f) * 30f), default(Vector2), Rand.GetRandomFloat(0.1f, 1f), 0, 0);
				Game1.vgame.pMgr.AddParticle(13, loc + Rand.GetRandomVec2(-40f, 40f, -20f, 20f) + new Vector2(0f, Rand.GetRandomFloat(30f, 38f) * 30f) + Rand.GetRandomVec2(-1f, 1.4f, 0f, 0f) * dyingFrame * 10f, Rand.GetRandomVec2(-30f, 30f, 100f, 120f), Rand.GetRandomFloat(0.1f, 1f), 0, 0);
				VikingQuake.SetQuake(0.5f);
			}
			if (dyingFrame > 30f)
			{
				active = false;
			}
		}
		if (hitFrame > 0f)
		{
			hitFrame -= Game1.frameTime;
		}
	}

	public void Draw()
	{
		if (hp >= 0f)
		{
			SpriteTools.sprite.Draw(Game1.vgame.moonTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle(0, (!(hp > 750f)) ? 705 : 0, 2048, 704), (hitFrame > 0.05f) ? Color.Red : Color.White, VScroll.angle + 3.14f, new Vector2(1024f, 1024f), VScroll.zoom * 1.15f, SpriteEffects.None, 1f);
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.moonTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle(i * 1024, 1409, 1024, 704), ((int)(dyingFrame * 30f) % 2 == 0) ? Color.Red : new Color(1f, 1f, 1f, 0.5f), VScroll.angle + 3.14f + dyingFrame * 0.02f * ((i == 0) ? (-1f) : 1f), new Vector2((i == 0) ? 960f : 64f, 1024f), VScroll.zoom * 1.15f, SpriteEffects.None, 1f);
		}
	}
}
