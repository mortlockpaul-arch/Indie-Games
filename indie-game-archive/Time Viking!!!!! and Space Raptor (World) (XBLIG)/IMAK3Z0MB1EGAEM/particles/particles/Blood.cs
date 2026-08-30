using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Blood : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = Rand.GetRandomFloat(0.2f, 0.5f);
		p.loc = loc;
		p.traj = traj;
		p.owner = -1;
		p.size = size;
		p.flags = Rand.GetRandomInt(0, 3);
		p.r = Rand.GetRandomFloat(0f, 0.4f);
		p.g = Rand.GetRandomFloat(-10f, 10f);
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Update(Particle p)
	{
		p.angle += p.g * FMan.frameTime;
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(p.flags * 128, 128, 128, 128), new Color(p.r, 0f, 0f, p.frame * 5f), p.angle, new Vector2(64f, 64f), p.size * ScrollMan.zoom, SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
