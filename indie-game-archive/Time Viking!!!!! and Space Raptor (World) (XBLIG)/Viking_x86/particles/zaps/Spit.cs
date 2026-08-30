using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;

namespace Viking_x86.particles.zaps;

public class Spit : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.owner = owner;
		p.frame = 1f;
		p.size = Rand.GetRandomFloat(0.5f, 1f);
		p.r = Rand.GetRandomFloat(0f, 0.5f);
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		p.traj.Y += Game1.frameTime * 900f;
		p.angle = Trig.GetAngle(default(Vector2), p.traj);
		if (HitManager.CheckHit(p))
		{
			p.exists = false;
			for (int i = 0; i < 5; i++)
			{
				Game1.vgame.pMgr.AddParticle(22, p.loc, Rand.GetRandomVec2(-300f, 300f, -300f, 300f), 0.15f, 0, 0);
			}
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(576, 256 + Rand.GetRandomInt(0, 3) * 64, 128, 64), new Color(p.r, 1f, 0.2f, 0.5f), p.angle + VScroll.angle, new Vector2(0f, 32f), 0.35f * VScroll.zoom * p.size, SpriteEffects.None, 1f);
	}
}
