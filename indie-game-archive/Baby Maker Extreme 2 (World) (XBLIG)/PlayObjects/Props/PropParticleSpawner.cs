using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Screens;

namespace PlayObjects.Props;

public class PropParticleSpawner : PropEffector
{
	private int m_iNumParticles;

	private List<ParticleEmitter> m_emitters;

	public PropParticleSpawner(int numParticles, List<ParticleEmitter> emitters)
	{
		m_emitters = emitters;
		m_iNumParticles = numParticles;
	}

	public override void CollisionResponse(Player p, Vector2 pos)
	{
		for (int i = 0; i < m_emitters.Count; i++)
		{
			m_emitters[i].Position = pos;
			m_emitters[i].CreateBurst(m_iNumParticles);
		}
	}
}
