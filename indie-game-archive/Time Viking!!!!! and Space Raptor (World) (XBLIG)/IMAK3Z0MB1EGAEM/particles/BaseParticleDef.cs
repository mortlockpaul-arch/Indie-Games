using Microsoft.Xna.Framework;

namespace IMAK3Z0MB1EGAEM.particles;

public class BaseParticleDef
{
	public virtual void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.exists = true;
	}

	public virtual void Update(Particle p)
	{
		p.loc += p.traj * FMan.frameTime;
		p.frame -= FMan.frameTime;
		if (p.frame < 0f)
		{
			Destroy(p);
		}
	}

	public virtual void Draw(Particle p)
	{
	}

	public virtual void Destroy(Particle p)
	{
		p.exists = false;
	}
}
