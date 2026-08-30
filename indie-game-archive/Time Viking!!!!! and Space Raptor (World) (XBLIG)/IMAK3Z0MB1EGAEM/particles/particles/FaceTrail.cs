using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class FaceTrail : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = 1f;
		p.loc = loc;
		p.traj = traj;
		p.owner = owner;
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(128, 1024, 128, 128), new Color(1f, 1f, 1f, p.frame * 5f), 0f, new Vector2(64f, 64f), 0.25f * ScrollMan.zoom, SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
