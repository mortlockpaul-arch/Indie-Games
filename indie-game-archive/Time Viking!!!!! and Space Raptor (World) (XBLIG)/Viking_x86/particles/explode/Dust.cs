using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Viking_x86.particles.explode;

public class Dust : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.size = size;
		p.frame = 0.5f;
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		if (Game1.vgame.pMgr.count > 100)
		{
			p.frame -= Game1.frameTime;
		}
		if (Game1.vgame.pMgr.count > 200)
		{
			p.frame -= Game1.frameTime * 10f;
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(0, 0, 128, 128), new Color(0.15f, 0.15f, 0.15f, p.frame), 0f, new Vector2(64f, 64f), p.size * VScroll.zoom, SpriteEffects.None, 1f);
	}
}
