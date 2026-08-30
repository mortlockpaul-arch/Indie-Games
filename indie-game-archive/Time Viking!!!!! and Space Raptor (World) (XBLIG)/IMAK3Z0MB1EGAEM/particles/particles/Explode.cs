using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Explode : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = Rand.GetRandomFloat(0.4f, 0.5f);
		p.loc = loc;
		p.traj = traj;
		p.owner = -1;
		p.size = size;
		p.r = Rand.GetRandomFloat(-1f, 1f);
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Update(Particle p)
	{
		p.angle += p.r * FMan.frameTime;
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		int num = (int)((0.5f - p.frame) * 2f * 9f);
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(1024, num * 64, 64, 64), new Color(1f, 1f, 1f, 0.7f), p.angle, new Vector2(32f, 32f), p.size * ScrollMan.zoom, SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
