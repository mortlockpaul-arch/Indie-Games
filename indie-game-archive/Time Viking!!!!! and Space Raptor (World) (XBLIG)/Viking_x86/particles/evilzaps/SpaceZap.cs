using IMAK3Z0MB1EGAEM.audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Viking_x86.particles.evilzaps;

public class SpaceZap : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.alpha = true;
		p.owner = owner;
		p.frame = 3f;
		Sound.Play("ibeam");
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
			Sound.Play("ihit");
			for (int i = 0; i < 10; i++)
			{
				Game1.vgame.pMgr.AddParticle(11, p.loc, p.traj * -1f + Rand.GetRandomVec2(-50f, 50f, -50f, 50f), Rand.GetRandomFloat(0f, 10f), 0, 0);
			}
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(Game1.nullTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(0, 0, 1, 1), new Color(1f, ((int)(p.frame * 30f) % 2 == 0) ? 0.1f : 0f, 0f, 0.5f), VScroll.angle, new Vector2(0.5f, 0.5f), new Vector2(12f, 30f) * VScroll.zoom, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.nullTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(0, 0, 1, 1), new Color(1f, ((int)(p.frame * 30f) % 2 == 0) ? 1f : 0f, 0f, 1f), VScroll.angle, new Vector2(0.5f, 0.5f), new Vector2(3f, 30f) * VScroll.zoom, SpriteEffects.None, 1f);
	}
}
