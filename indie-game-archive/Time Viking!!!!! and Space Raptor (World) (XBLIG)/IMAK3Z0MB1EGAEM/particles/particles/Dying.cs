using Microsoft.Xna.Framework;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Dying : BaseParticleDef
{
	private const string dedz = "DEDZ!!1";

	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = 2f;
		p.loc = loc;
		p.traj = default(Vector2);
		p.owner = -1;
		p.size = size;
		p.flags = flags;
		p.g = Rand.GetRandomFloat(-10f, 10f);
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Update(Particle p)
	{
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		Text.DrawString("DEDZ!!1", ScrollMan.GetScreenLoc(p.loc, 1f), 5f, new Color(Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), Rand.GetRandomFloat(0.75f, 1f), 0.8f), Text.Justify.Center);
		base.Draw(p);
	}
}
