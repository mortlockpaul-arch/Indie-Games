using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Pixel : BaseParticleDef
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

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(448, 896, 128, 128), new Color(1f, 1f, 1f, p.frame * 5f), 0f, new Vector2(64f, 64f), p.size * ScrollMan.zoom, SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
