using Microsoft.Xna.Framework;

namespace IMAK3Z0MB1EGAEM.particles;

public class ParticleMan
{
	public static Particle[] particle;

	private static ParticleCatalog catalog;

	private static int addIdx;

	public static int count;

	public static void Init()
	{
		particle = new Particle[2048];
		catalog = new ParticleCatalog();
		for (int i = 0; i < particle.Length; i++)
		{
			particle[i] = new Particle();
		}
	}

	public static void AddParticle(int type, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		for (int i = 0; i < particle.Length; i++)
		{
			if (!particle[i].exists)
			{
				particle[i].alpha = false;
				catalog.catalog[type].Init(particle[i], loc, traj, owner, size, flags);
				particle[i].type = type;
				addIdx = i + 1;
				break;
			}
		}
	}

	public static void Update()
	{
		addIdx = 0;
		count = 0;
		for (int i = 0; i < ParticleMan.particle.Length; i++)
		{
			Particle particle = ParticleMan.particle[i];
			if (particle.exists)
			{
				catalog.catalog[particle.type].Update(particle);
				count++;
				if (!float.IsNaN(particle.loc.X) && !float.IsNaN(particle.loc.Y) && !float.IsNaN(particle.frame) && !float.IsNaN(particle.traj.X) && !float.IsNaN(particle.traj.Y))
				{
					float.IsNaN(particle.angle);
				}
			}
		}
	}

	public static void Draw(bool alpha)
	{
		for (int i = 0; i < ParticleMan.particle.Length; i++)
		{
			Particle particle = ParticleMan.particle[i];
			if (particle.exists && particle.alpha == alpha)
			{
				catalog.catalog[particle.type].Draw(particle);
			}
		}
	}
}
