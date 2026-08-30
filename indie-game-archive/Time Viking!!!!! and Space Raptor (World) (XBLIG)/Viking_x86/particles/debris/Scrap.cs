using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Viking_x86.particles.debris;

public class Scrap : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.size = 0.15f;
		p.flags = Rand.GetRandomInt(4, 8);
		p.owner = owner;
		p.frame = 5f;
		p.rotationSpeed = Rand.GetRandomFloat(-35f, 35f);
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		p.traj.Y += 20f;
		p.angle += p.rotationSpeed * Game1.frameTime;
		if (Game1.vgame.world.TestCollision(p.loc))
		{
			Game1.vgame.world.AddDebris(p.loc, Rand.GetRandomInt(0, 4));
			Game1.vgame.pMgr.AddParticle(3, p.loc, Rand.GetRandomVec2(-10f, 10f, -40f, 0f), Rand.GetRandomFloat(0.2f, 0.5f), 0, 0);
			p.exists = false;
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		float foreBright = Game1.vgame.world.GetForeBright();
		SpriteTools.sprite.Draw(VikingGame.textures["scrap"].texture, VScroll.GetScreenLoc(p.loc, 1f), VikingGame.textures["scrap"].GetSpriteRect(p.flags), new Color(foreBright, foreBright, foreBright, 1f), p.angle, VikingGame.textures["scrap"].GetRelativeSpriteOrigin(p.flags), p.size * VScroll.zoom, SpriteEffects.None, 1f);
	}
}
