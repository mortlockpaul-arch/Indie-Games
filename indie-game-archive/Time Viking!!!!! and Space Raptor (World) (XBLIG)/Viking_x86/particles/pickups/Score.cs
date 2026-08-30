using IMAK3Z0MB1EGAEM;
using Microsoft.Xna.Framework;

namespace Viking_x86.particles.pickups;

public class Score : BaseParticle
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.frame = 0.5f;
		p.loc = loc;
		p.traj = new Vector2(0f, -150f);
		p.owner = -1;
		p.size = size;
		p.flags = flags;
		p.g = Rand.GetRandomFloat(-10f, 10f);
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		if (p.frame < 0.4f)
		{
			p.traj.Y = 0f;
		}
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
		Text.DrawScore(p.flags, VScroll.GetScreenLoc(p.loc, 1f), 2f, new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.8f), Text.Justify.Center);
		base.Draw(p);
	}
}
