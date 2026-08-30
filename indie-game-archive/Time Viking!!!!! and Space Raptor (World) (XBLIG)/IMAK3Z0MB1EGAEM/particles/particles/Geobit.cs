using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Geobit : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = Rand.GetRandomFloat(0.2f, 0.5f);
		p.loc = loc;
		p.traj = traj;
		p.owner = -1;
		p.size = size;
		p.alpha = true;
		p.r = Rand.GetRandomFloat(0f, 6.28f);
		p.g = Rand.GetRandomFloat(-5f, 5f);
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Update(Particle p)
	{
		p.angle += p.g * FMan.frameTime;
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(1024, 960, 192, 128), new Color(0.7f + 0.3f * (float)Math.Sin(p.r + p.frame), 0.7f + 0.3f * (float)Math.Sin(p.r + p.frame + 2f), 0.7f + 0.3f * (float)Math.Sin(p.r + p.frame + 4f), p.frame * 5f), p.angle, new Vector2(96f, 64f), p.size * ScrollMan.zoom * new Vector2(1f, 0.5f), SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
