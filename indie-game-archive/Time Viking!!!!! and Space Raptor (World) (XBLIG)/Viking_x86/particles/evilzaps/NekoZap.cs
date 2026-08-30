using IMAK3Z0MB1EGAEM.audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Yuki_Win;

namespace Viking_x86.particles.evilzaps;

public class NekoZap : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.angle = Trig.GetAngle(default(Vector2), traj);
		p.alpha = true;
		p.owner = owner;
		p.frame = 3f;
		Sound.Play("catzap");
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		if (HitManager.CheckHit(p))
		{
			p.exists = false;
		}
		else if (Game1.vgame.world.TestCollision(p.loc))
		{
			p.exists = false;
		}
		if (!p.exists)
		{
			Game1.vgame.pMgr.AddParticle(12, p.loc, p.traj * Rand.GetRandomFloat(-0.5f, 0f), 0.5f, 0, 0);
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(128 + Rand.GetRandomInt(0, 6) * 128, 0, 128, 64), new Color(1f, ((int)(p.frame * 30f) % 2 == 0) ? 1f : 0f, 0f, 1f), p.angle + VScroll.angle + 3.14f, new Vector2(32f, 32f), 0.5f * VScroll.zoom, SpriteEffects.None, 1f);
	}
}
