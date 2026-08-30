using System.Collections.Generic;

namespace Renderer;

public static class ParticleManager
{
	private const int numParticles = 1000;

	private static AnimateParticle[] m_particles;

	private static Queue<AnimateParticle> m_freeParticles;

	public static int NumParticles => m_particles.Length - m_freeParticles.Count;

	public static void Initialize()
	{
		m_particles = new AnimateParticle[1000];
		m_freeParticles = new Queue<AnimateParticle>(1000);
		for (int i = 0; i < 1000; i++)
		{
			m_particles[i] = new AnimateParticle();
			m_freeParticles.Enqueue(m_particles[i]);
		}
	}

	public static AnimateParticle GetParticle()
	{
		if (m_freeParticles.Count > 0)
		{
			return m_freeParticles.Dequeue();
		}
		return m_particles[0];
	}

	public static void Update(TimeTracker gameTime)
	{
		AnimateParticle[] particles = m_particles;
		foreach (AnimateParticle animateParticle in particles)
		{
			if (animateParticle.Active)
			{
				animateParticle.Update(gameTime);
				if (!animateParticle.Active)
				{
					m_freeParticles.Enqueue(animateParticle);
				}
			}
		}
	}

	public static void Draw(TimeTracker gameTime)
	{
		AnimateParticle[] particles = m_particles;
		foreach (AnimateParticle animateParticle in particles)
		{
			if (animateParticle.Active)
			{
				animateParticle.Draw(gameTime);
			}
		}
	}
}
