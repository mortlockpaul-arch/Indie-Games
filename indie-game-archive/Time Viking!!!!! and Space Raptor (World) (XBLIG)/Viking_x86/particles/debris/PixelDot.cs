using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Viking_x86.particles.debris;

public class PixelDot : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.size = size;
		p.alpha = true;
		p.owner = owner;
		p.frame = Rand.GetRandomFloat(0.2f, 0.5f);
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(Game1.nullTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(0, 0, 1, 1), new Color(1f, 1f, 1f, p.frame * 5f), 0f, new Vector2(0.5f, 0.5f), p.size * VScroll.zoom, SpriteEffects.None, 1f);
	}
}
