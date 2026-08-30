using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;

namespace Viking_x86.particles.zaps;

public class Zapnel : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.alpha = true;
		p.angle = Trig.GetAngle(default(Vector2), traj);
		p.owner = owner;
		p.frame = 0.3f;
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		if (HitManager.CheckHit(p))
		{
			p.exists = false;
			Game1.vgame.pMgr.AddParticle(5, p.loc, default(Vector2), 0.15f, 0, 0);
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		float num = p.frame * 7f;
		if (num > 1f)
		{
			num = 1f;
		}
		SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(128 + Rand.GetRandomInt(0, 6) * 128, 0, 128, 64), new Color(1f, 1f, 1f, num), p.angle + VScroll.angle + 3.14f, new Vector2(0f, 32f), 0.5f * VScroll.zoom, SpriteEffects.None, 1f);
	}
}
