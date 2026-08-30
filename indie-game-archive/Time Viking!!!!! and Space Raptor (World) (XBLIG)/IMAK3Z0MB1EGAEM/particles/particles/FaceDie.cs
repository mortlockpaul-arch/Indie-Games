using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Yuki_Win;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class FaceDie : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = Rand.GetRandomFloat(0.25f, 0.75f);
		p.loc = loc;
		p.traj = traj;
		p.owner = owner;
		p.flags = Rand.GetRandomInt(0, 2);
		p.angle = Trig.GetAngle(default(Vector2), traj);
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(Rand.GetRandomInt(0, 6) * 128, 1024, 128, 128), new Color(1f, p.flags, p.flags, p.frame * 5f), 0f, new Vector2(64f, 64f), 0.25f * ScrollMan.zoom, SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
