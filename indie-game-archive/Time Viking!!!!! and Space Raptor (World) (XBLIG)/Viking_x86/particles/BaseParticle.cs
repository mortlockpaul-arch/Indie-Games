using Microsoft.Xna.Framework;

namespace Viking_x86.particles;

public class BaseParticle
{
	public virtual void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.exists = true;
	}

	public virtual void Update(Particle p)
	{
		p.loc += p.traj * Game1.frameTime;
		p.frame -= Game1.frameTime;
		if (p.frame < 0f)
		{
			p.exists = false;
		}
	}

	public virtual void Draw(Particle p)
	{
	}
}
