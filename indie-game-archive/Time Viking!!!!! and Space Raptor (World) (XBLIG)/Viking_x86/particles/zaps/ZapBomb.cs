using System;
using IMAK3Z0MB1EGAEM.audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;

namespace Viking_x86.particles.zaps;

public class ZapBomb : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.alpha = true;
		p.angle = Trig.GetAngle(default(Vector2), traj);
		p.owner = owner;
		p.frame = 1f;
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		float frame = p.frame;
		if (HitManager.CheckHit(p, 20f))
		{
			Sound.Play("zexplode");
			for (int i = 0; i < 32; i++)
			{
				float num = (float)i / 32f * 6.28f;
				Game1.vgame.pMgr.AddParticle(18, p.loc, new Vector2((float)Math.Cos(num), (float)Math.Sin(num)) * 800f, 0f, 0, p.owner);
			}
			p.exists = false;
			Game1.vgame.pMgr.AddParticle(5, p.loc, default(Vector2), 0.15f, 0, 0);
		}
		base.Update(p);
		if ((int)(frame * 30f) != (int)(p.frame * 30f))
		{
			Game1.vgame.pMgr.AddParticle(5, p.loc + Rand.GetRandomVec2(-10f, 10f, -10f, 10f), Rand.GetRandomVec2(-130f, 130f, -130f, 130f), Rand.GetRandomFloat(0.1f, 0.2f), 0, p.owner);
		}
	}

	public override void Draw(Particle p)
	{
		for (int i = 0; i < 3; i++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(128 + Rand.GetRandomInt(0, 6) * 128, 0, 128, 64), new Color(1f, 1f, 1f, 1f), Rand.GetRandomFloat(0f, 6.28f), new Vector2(64f, 32f), 0.65f * VScroll.zoom, SpriteEffects.None, 1f);
		}
	}
}
