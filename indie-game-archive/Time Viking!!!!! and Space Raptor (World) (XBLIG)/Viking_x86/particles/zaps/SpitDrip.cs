using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;

namespace Viking_x86.particles.zaps;

public class SpitDrip : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.owner = owner;
		p.frame = Rand.GetRandomFloat(0.1f, 0.8f);
		p.size = Rand.GetRandomFloat(0.5f, 1f);
		p.r = Rand.GetRandomFloat(0f, 0.5f);
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
		p.traj.Y += Game1.frameTime * 900f;
		p.angle = Trig.GetAngle(default(Vector2), p.traj);
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(576, 256 + Rand.GetRandomInt(0, 3) * 64, 128, 64), new Color(p.r, 1f, 0.2f, 0.5f), p.angle + VScroll.angle, new Vector2(0f, 32f), 0.35f * VScroll.zoom * p.size * p.frame, SpriteEffects.None, 1f);
	}
}
